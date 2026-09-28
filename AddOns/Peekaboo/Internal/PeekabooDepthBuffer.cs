using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Layout and queries for the software depth buffer and its hierarchical minimum pyramid.
    /// </summary>
    /// <remarks>
    /// A texel holds the closeness of the nearest occluder surface that fully covers it, or negative
    /// infinity if nothing does. A point is hidden when its closeness is below the texel's. So the
    /// pyramid reduces with min, and a coarse texel only claims what every texel under it claims.
    /// That makes it safe to read a level whose texels hang past the queried rectangle. The extra
    /// area can only lower the minimum and lose a cull, never create one.
    /// </remarks>
    internal static unsafe class PeekabooDepthBuffer
    {
        public const float kEmpty = float.NegativeInfinity;

        // How many texels a rectangle query can span per axis before it moves to a coarser level.
        //
        // Coarser isn't cheaper, even though it looks like it should be. A coarse level hangs past the
        // rectangle, picks up empty space, and loses the cull. A lost cull at the chunk level means
        // testing the whole chunk one entity at a time. Always reading a single texel, the cheapest
        // possible read, makes the pass about 50% slower because of this.
        const int kMaxQueryTexels = 16;

        // How far a depth test leans away from culling, relative to the closeness being tested. Big
        // enough to cover rounding differences between the reciprocal here and the plane equation the
        // rasterizer uses, and much smaller than a texel's worth of depth.
        const float kSelfOcclusionBias = 1e-6f;

        // Spare texels past the end of a chain, so a query reading eight at a time can start on the
        // very last texel and still stay inside the allocation. Whatever it reads there gets masked
        // off. The padding only exists to make the read legal.
        public const int kOverreadPadding = 8;

        public static int2 LevelResolution(int2 resolution, int level) => math.max(1, resolution >> level);

        public static int LevelCount(int2 resolution)
        {
            int largest = math.max(resolution.x, resolution.y);
            int levels  = 1;
            while (largest > 1)
            {
                largest = (largest + 1) / 2;
                levels++;
            }
            return levels;
        }

        public static int TotalTexels(int2 resolution)
        {
            int levels = LevelCount(resolution);
            int total  = 0;
            for (int i = 0; i < levels; i++)
            {
                var r  = LevelResolution(resolution, i);
                total += r.x * r.y;
            }
            return total;
        }

        public static int LevelOffset(int2 resolution, int level)
        {
            int offset = 0;
            for (int i = 0; i < level; i++)
            {
                var r   = LevelResolution(resolution, i);
                offset += r.x * r.y;
            }
            return offset;
        }

        /// <summary>
        /// Reduces one parent row of the finest level into level one of both pyramids at once.
        /// </summary>
        /// <remarks>
        /// Most of a pyramid's work is this first reduction, since each later level is a quarter the
        /// size of the one before. Parent rows never overlap, so this is the part worth spreading
        /// across workers. The min and max come from the same four texels, so one read serves both.
        ///
        /// The max pyramid has no level zero. It would just be a copy of the min pyramid's level zero,
        /// and copying the whole buffer every pass costs more than reading the original in the one
        /// query that needs it.
        /// </remarks>
        public static void ReduceFirstLevel(float* minChain, float* maxChain, int2 resolution, int parentY)
        {
            var childRes   = resolution;
            var parentRes  = LevelResolution(resolution, 1);
            int parentBase = LevelOffset(resolution, 1);

            int y0 = math.min(parentY * 2,     childRes.y - 1);
            int y1 = math.min(parentY * 2 + 1, childRes.y - 1);
            int r0 = y0 * childRes.x;
            int r1 = y1 * childRes.x;

            int row = parentBase + parentY * parentRes.x;
            int x   = 0;

            // Four parents at a time, from two loads per child row. A child row of at least eight
            // texels has no odd column to clamp. The operations match the loop below exactly.
            if (childRes.x >= 8 && (childRes.x & 1) == 0)
            {
                for (; x + 4 <= parentRes.x; x += 4)
                {
                    var top0      = *(float4*)(minChain + r0 + x * 2);
                    var top1      = *(float4*)(minChain + r0 + x * 2 + 4);
                    var bottom0   = *(float4*)(minChain + r1 + x * 2);
                    var bottom1   = *(float4*)(minChain + r1 + x * 2 + 4);
                    var topLeft   = new float4(top0.xz, top1.xz);
                    var topRight  = new float4(top0.yw, top1.yw);
                    var botLeft   = new float4(bottom0.xz, bottom1.xz);
                    var botRight  = new float4(bottom0.yw, bottom1.yw);
                    *(float4*)(minChain + row + x) = math.min(math.min(topLeft, topRight), math.min(botLeft, botRight));
                    *(float4*)(maxChain + row + x) = math.max(math.max(topLeft, topRight), math.max(botLeft, botRight));
                }
            }

            for (; x < parentRes.x; x++)
            {
                int x0 = math.min(x * 2,     childRes.x - 1);
                int x1 = math.min(x * 2 + 1, childRes.x - 1);

                // One read serves both chains, since level one of each reduces the same texels.
                var gathered      = new float4(minChain[r0 + x0], minChain[r0 + x1], minChain[r1 + x0], minChain[r1 + x1]);
                minChain[row + x] = math.cmin(gathered);
                maxChain[row + x] = math.cmax(gathered);
            }
        }

        /// <summary>
        /// Builds levels two and up. They're about a twelfth of the work, so they stay on one worker
        /// per view instead of paying for a dispatch per level.
        /// </summary>
        public static void BuildPyramidsFromLevel2(float* minChain, float* maxChain, int2 resolution)
        {
            int levels = LevelCount(resolution);
            for (int level = 2; level < levels; level++)
            {
                var childRes   = LevelResolution(resolution, level - 1);
                var parentRes  = LevelResolution(resolution, level);
                int childBase  = LevelOffset(resolution, level - 1);
                int parentBase = LevelOffset(resolution, level);
                for (int y = 0; y < parentRes.y; y++)
                {
                    int y0 = math.min(y * 2,     childRes.y - 1) * childRes.x;
                    int y1 = math.min(y * 2 + 1, childRes.y - 1) * childRes.x;
                    for (int x = 0; x < parentRes.x; x++)
                    {
                        int x0 = math.min(x * 2,     childRes.x - 1);
                        int x1 = math.min(x * 2 + 1, childRes.x - 1);

                        var gathered = new float4(minChain[childBase + y0 + x0], minChain[childBase + y0 + x1],
                                                  minChain[childBase + y1 + x0], minChain[childBase + y1 + x1]);
                        minChain[parentBase + y * parentRes.x + x] = math.cmin(gathered);

                        gathered = new float4(maxChain[childBase + y0 + x0], maxChain[childBase + y0 + x1],
                                              maxChain[childBase + y1 + x0], maxChain[childBase + y1 + x1]);
                        maxChain[parentBase + y * parentRes.x + x] = math.cmax(gathered);
                    }
                }
            }
        }

        /// <summary>
        /// Builds both pyramids over the rasterized buffer. The min pyramid answers "is all of this
        /// covered?", and the max pyramid answers "is any of it?".
        /// </summary>
        /// <remarks>
        /// The max pyramid is there for the second question. In a big scene, most renderers that
        /// survive frustum culling aren't behind anything, and testing each one to find that out is
        /// the cost that grows. One read of the max pyramid over a chunk's bounds rules out all 128 at
        /// once.
        ///
        /// This builds the whole chain on one thread, including the level zero copy the culling pass
        /// skips. The validation harness uses it. The culling pass splits the work up instead.
        /// </remarks>
        public static void BuildPyramids(float* minChain, float* maxChain, int2 resolution)
        {
            int texels = resolution.x * resolution.y;
            UnsafeUtility.MemCpy(maxChain, minChain, texels * sizeof(float));

            int levels = LevelCount(resolution);
            for (int level = 1; level < levels; level++)
            {
                var childRes   = LevelResolution(resolution, level - 1);
                var parentRes  = LevelResolution(resolution, level);
                int childBase  = LevelOffset(resolution, level - 1);
                int parentBase = LevelOffset(resolution, level);
                for (int y = 0; y < parentRes.y; y++)
                {
                    int y0 = math.min(y * 2,     childRes.y - 1) * childRes.x;
                    int y1 = math.min(y * 2 + 1, childRes.y - 1) * childRes.x;
                    for (int x = 0; x < parentRes.x; x++)
                    {
                        int x0 = math.min(x * 2,     childRes.x - 1);
                        int x1 = math.min(x * 2 + 1, childRes.x - 1);

                        var gathered = new float4(minChain[childBase + y0 + x0], minChain[childBase + y0 + x1],
                                                  minChain[childBase + y1 + x0], minChain[childBase + y1 + x1]);
                        minChain[parentBase + y * parentRes.x + x] = math.cmin(gathered);

                        gathered = new float4(maxChain[childBase + y0 + x0], maxChain[childBase + y0 + x1],
                                              maxChain[childBase + y1 + x0], maxChain[childBase + y1 + x1]);
                        maxChain[parentBase + y * parentRes.x + x] = math.cmax(gathered);
                    }
                }
            }
        }

        /// <summary>
        /// Whether every texel in the inclusive pixel rectangle stores a depth closer than the given
        /// closeness. If so, anything at that closeness inside the rectangle is hidden.
        /// </summary>
        /// <remarks>
        /// An occluder is also an occludee, so it gets tested against a buffer its own plates wrote
        /// into. With exact math, it can't hide itself. Its plates are inside the bounds it's tested
        /// with, so their depth is never closer than the bounds' nearest corner. And the rasterizer
        /// writes the smallest closeness over a whole texel, not the nearest. But for a flat surface
        /// seen head-on and filling its rectangle, both of those margins drop to zero. Then both sides
        /// compute the same 1/z in different ways: a reciprocal here, and a plane equation in the
        /// rasterizer. Nothing makes them agree exactly. A few parts per million of bias against
        /// culling fixes it, and costs one operation per query instead of per texel.
        /// </remarks>
        public static bool IsRectOccluded(in PeekabooView view, float* chain, int4 rect, float closeness)
        {
            if (!TryResolveQuery(in view, ref rect, out int levelWidth, out int levelOffset, out _))
                return false;

            // Scaled by the magnitude and added, not multiplied, because orthographic closeness is
            // -depth and is often negative.
            closeness    += math.abs(closeness) * kSelfOcclusionBias;
            var levelBase = chain + levelOffset + rect.x;
            int width     = rect.z - rect.x + 1;

            if (X86.Avx.IsAvxSupported)
            {
                var wanted = new v256(closeness);
                for (int y = rect.y; y <= rect.w; y++)
                {
                    var row = levelBase + y * levelWidth;
                    for (int left = width; left > 0; left -= 8, row += 8)
                    {
                        // Always read eight. A row is only a few texels and the read costs the same,
                        // so the extra ones get read and then masked out.
                        int found = X86.Avx.mm256_movemask_ps(X86.Avx.mm256_cmp_ps(X86.Avx.mm256_loadu_ps(row), wanted, (int)X86.Avx.CMP.LE_OQ));
                        if ((found & Mask(left)) != 0)
                            return false;
                    }
                }
                return true;
            }

            for (int y = rect.y; y <= rect.w; y++)
            {
                var row = levelBase + y * levelWidth;
                for (int x = 0; x < width; x++)
                {
                    if (row[x] <= closeness)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Which of eight lanes are texels the query actually wanted.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Mask(int wanted) => wanted >= 8 ? 0xff : (1 << wanted) - 1;

        /// <summary>
        /// Whether no texel in the rectangle stores a depth closer than the given closeness. If so,
        /// nothing at that closeness or closer inside the rectangle can be hidden.
        /// </summary>
        /// <remarks>
        /// Reading a coarser level than needed raises the max, which makes this harder to pass, not
        /// easier. So the extra area can only make us skip less, never skip wrongly.
        /// </remarks>
        public static bool IsRectUnoccluded(in PeekabooView view, float* minChain, float* maxChain, int4 rect, float closeness)
        {
            if (!TryResolveQuery(in view, ref rect, out int levelWidth, out int levelOffset, out int level))
                return false;
            // The max chain starts at level one. Its level zero would be a copy of the min chain's,
            // so this reads the original instead.
            var levelBase = (level == 0 ? minChain : maxChain) + levelOffset + rect.x;
            int width     = rect.z - rect.x + 1;

            if (X86.Avx.IsAvxSupported)
            {
                var wanted = new v256(closeness);
                for (int y = rect.y; y <= rect.w; y++)
                {
                    var row = levelBase + y * levelWidth;
                    for (int left = width; left > 0; left -= 8, row += 8)
                    {
                        int found = X86.Avx.mm256_movemask_ps(X86.Avx.mm256_cmp_ps(X86.Avx.mm256_loadu_ps(row), wanted, (int)X86.Avx.CMP.GT_OQ));
                        if ((found & Mask(left)) != 0)
                            return false;
                    }
                }
                return true;
            }

            for (int y = rect.y; y <= rect.w; y++)
            {
                var row = levelBase + y * levelWidth;
                for (int x = 0; x < width; x++)
                {
                    if (row[x] > closeness)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Clamps a pixel rectangle to the buffer and converts it to a texel rectangle at a level fine
        /// enough to stay close to its real size.
        /// </summary>
        /// <remarks>
        /// The coarsest level where the rectangle fits in two texels is the cheapest to read. But a
        /// small rectangle sitting on a texel boundary then gets tested over four times its own area,
        /// and loses the cull to the empty space it picks up. A finer level reads a few more texels
        /// and stays close to the rectangle's real size.
        /// </remarks>
        static bool TryResolveQuery(in PeekabooView view, ref int4 rect, out int levelWidth, out int levelOffset, out int level)
        {
            // No emptiness check. Every rectangle comes from TouchedRect, which never produces an
            // inverted one, and clamping keeps the ordering.
            rect = math.clamp(rect, int4.zero, view.resolutionMaxTexel);

            // Level zero is by far the most common, so it gets a branch. Otherwise, counting the
            // span's leading zeros gives the smallest level where span >> level fits.
            int span = math.max(rect.z - rect.x, rect.w - rect.y);
            level    = span < kMaxQueryTexels
                       ? 0
                       : math.min(32 - math.lzcnt(span) - math.tzcnt(kMaxQueryTexels), view.mipLevels - 1);

            levelWidth      = view.levelWidths[level];
            levelOffset     = view.levelOffsets[level];
            // No second clamp needed. Resolutions are powers of two, so (res - 1) >> level is exactly
            // (res >> level) - 1, the last index in the level.
            rect            = rect >> level;
            return true;
        }
    }
}
