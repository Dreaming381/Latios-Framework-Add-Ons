using System.Runtime.CompilerServices;
using Latios.Kinemation;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Exposed;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Latios.Peekaboo.Systems
{
    internal struct OccluderCandidate
    {
        public float4x4                                 objectToWorld;
        public BlobAssetReference<PeekabooOccluderBlob> blob;
        public ulong                                    opaqueSubmeshes;
        public int                                      meshIndex;
        public int                                      viewIndex;
        public float                                    score;
        public int                                      plateReserveStart;
        public int                                      plateReserveCount;
        public int                                      vertexReserveStart;
    }

    internal static unsafe class PeekabooProjection
    {
        /// <summary>
        /// The screen span the bounds cover, in texels, and the closeness of the nearest and farthest
        /// corners. An entity can only be culled by a depth closer than its nearest corner. The
        /// farthest corner is what lets a whole chunk be ruled out at once.
        /// </summary>
        /// <remarks>
        /// The span stays in floating point because it gets rounded two opposite ways. An occludee has
        /// to be tested over every texel it touches, and an occluder can only claim texels it fully
        /// covers. A span narrower than a texel touches one but covers none.
        ///
        /// This runs once per entity per view in both jobs that scale with the scene, so it's the one
        /// place where the math's layout matters more than the code's. The eight corners go in eight
        /// lanes, which makes the whole projection about a dozen eight-wide operations with one
        /// divide.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryProjectBounds(in PeekabooView view, AABB aabb, out float4 span, out float nearestCloseness, out float farthestCloseness)
        {
            // With no divide, orthographic screen coordinates are linear in the box. So the center and
            // extents give the exact same answer as projecting eight corners, in about half the
            // instructions. A shadow pass runs four of these for each one a camera pass runs.
            if (!view.isPerspective)
                return TryProjectBoundsAxial(in view, aabb, out span, out nearestCloseness, out farthestCloseness);
            if (X86.Avx.IsAvxSupported)
                return TryProjectBoundsWide(in view, aabb, out span, out nearestCloseness, out farthestCloseness);
            return TryProjectBoundsPortable(in view, aabb, out span, out nearestCloseness, out farthestCloseness);
        }

        /// <summary>
        /// Projects the eight corners in eight lanes for a perspective view, so the near and far faces
        /// go through every operation together.
        /// </summary>
        /// <remarks>
        /// Raw minps and maxps don't care about NaN, but math.min and math.max each compile to a
        /// compare and a blend to handle it. That costs about a third of the projection, mostly in the
        /// six horizontal reductions, where the NaN-safe version is a 12-step serial chain and the raw
        /// version is 4. Nothing here needs it, since one finite check over every projected value
        /// covers it. That check has to happen before the reduction, because minps drops a NaN instead
        /// of passing it through.
        /// </remarks>
        static bool TryProjectBoundsWide(in PeekabooView view, AABB aabb, out float4 span, out float nearestCloseness,
                                         out float farthestCloseness)
        {
            span              = default;
            nearestCloseness  = float.NegativeInfinity;
            farthestCloseness = float.PositiveInfinity;

            var min = aabb.Min;
            var max = aabb.Max;

            // Lanes 0 to 3 are the min z face and lanes 4 to 7 the max z face. Each has the four
            // combinations of x and y.
            var cornerX = X86.Avx.mm256_sub_ps(new v256(min.x, max.x, min.x, max.x, min.x, max.x, min.x, max.x), new v256(view.origin.x));
            var cornerY = X86.Avx.mm256_sub_ps(new v256(min.y, min.y, max.y, max.y, min.y, min.y, max.y, max.y), new v256(view.origin.y));
            var cornerZ = X86.Avx.mm256_sub_ps(new v256(min.z, min.z, min.z, min.z, max.z, max.z, max.z, max.z), new v256(view.origin.z));

            var viewX = ProjectAxis(cornerX, cornerY, cornerZ, view.axisRightScaled);
            var viewY = ProjectAxis(cornerX, cornerY, cornerZ, view.axisUpScaled);
            var viewZ = ProjectAxis(cornerX, cornerY, cornerZ, view.axisForward);

            if (X86.Avx.mm256_movemask_ps(X86.Avx.mm256_cmp_ps(viewZ, new v256(view.nearDistance), (int)X86.Avx.CMP.LT_OQ)) != 0)
                return false;
            var closeness = X86.Avx.mm256_div_ps(new v256(1f), viewZ);
            var screenX   = X86.Avx.mm256_add_ps(X86.Avx.mm256_mul_ps(viewX, closeness), new v256(view.screenOffset.x));
            var screenY   = X86.Avx.mm256_add_ps(X86.Avx.mm256_mul_ps(viewY, closeness), new v256(view.screenOffset.y));

            var absolute = new v256(0x7fffffff);
            var infinity = new v256(float.PositiveInfinity);
            var finite   = X86.Avx.mm256_and_ps(
                X86.Avx.mm256_cmp_ps(X86.Avx.mm256_and_ps(screenX, absolute), infinity, (int)X86.Avx.CMP.LT_OQ),
                X86.Avx.mm256_cmp_ps(X86.Avx.mm256_and_ps(screenY, absolute), infinity, (int)X86.Avx.CMP.LT_OQ));
            if (X86.Avx.mm256_movemask_ps(finite) != 0xff)
                return false;

            span              = ReduceCorners(screenX, screenY);
            nearestCloseness  = ReduceMax(closeness);
            farthestCloseness = ReduceMin(closeness);
            return true;
        }

        /// <summary>
        /// The min and max of both axes, laid out like the span, in two reductions instead of four.
        /// </summary>
        /// <remarks>
        /// An eight lane reduction folds the two halves together, then reduces the remaining four. That
        /// first fold wastes most of the register. So instead, the two axes go in the two halves. One
        /// half holds the min z face of both axes, and the other holds the max z face. AVX shuffles
        /// work within each 128-bit half, which is exactly what a four lane reduction needs. Each
        /// answer ends up in the first lane of its half, and one unpack brings them together.
        ///
        /// The same pair is used for both min and max, so it's built once. Each value goes through
        /// the same comparisons in the same order as reducing each axis separately, so the results
        /// match to the bit.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float4 ReduceCorners(v256 screenX, v256 screenY)
        {
            var near = X86.Avx.mm256_permute2f128_ps(screenX, screenY, 0x20);
            var far  = X86.Avx.mm256_permute2f128_ps(screenX, screenY, 0x31);

            var low = X86.Avx.mm256_min_ps(near, far);
            low     = X86.Avx.mm256_min_ps(low, X86.Avx.mm256_shuffle_ps(low, low, X86.Sse.SHUFFLE(1, 0, 3, 2)));
            low     = X86.Avx.mm256_min_ps(low, X86.Avx.mm256_shuffle_ps(low, low, X86.Sse.SHUFFLE(2, 3, 0, 1)));

            var high = X86.Avx.mm256_max_ps(near, far);
            high     = X86.Avx.mm256_max_ps(high, X86.Avx.mm256_shuffle_ps(high, high, X86.Sse.SHUFFLE(1, 0, 3, 2)));
            high     = X86.Avx.mm256_max_ps(high, X86.Avx.mm256_shuffle_ps(high, high, X86.Sse.SHUFFLE(2, 3, 0, 1)));

            var lower  = X86.Sse.unpacklo_ps(X86.Avx.mm256_castps256_ps128(low),  X86.Avx.mm256_extractf128_ps(low, 1));
            var upper  = X86.Sse.unpacklo_ps(X86.Avx.mm256_castps256_ps128(high), X86.Avx.mm256_extractf128_ps(high, 1));
            var packed = X86.Sse.movelh_ps(lower, upper);
            return UnsafeUtility.As<v128, float4>(ref packed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static v256 ProjectAxis(v256 x, v256 y, v256 z, float3 axis)
        {
            var accumulated = X86.Avx.mm256_mul_ps(x, new v256(axis.x));
            accumulated     = X86.Avx.mm256_add_ps(accumulated, X86.Avx.mm256_mul_ps(y, new v256(axis.y)));
            return X86.Avx.mm256_add_ps(accumulated, X86.Avx.mm256_mul_ps(z, new v256(axis.z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ReduceMin(v256 v)
        {
            var q = X86.Sse.min_ps(X86.Avx.mm256_castps256_ps128(v), X86.Avx.mm256_extractf128_ps(v, 1));
            q     = X86.Sse.min_ps(q, X86.Sse.shuffle_ps(q, q, X86.Sse.SHUFFLE(1, 0, 3, 2)));
            q     = X86.Sse.min_ps(q, X86.Sse.shuffle_ps(q, q, X86.Sse.SHUFFLE(2, 3, 0, 1)));
            return q.Float0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ReduceMax(v256 v)
        {
            var q = X86.Sse.max_ps(X86.Avx.mm256_castps256_ps128(v), X86.Avx.mm256_extractf128_ps(v, 1));
            q     = X86.Sse.max_ps(q, X86.Sse.shuffle_ps(q, q, X86.Sse.SHUFFLE(1, 0, 3, 2)));
            q     = X86.Sse.max_ps(q, X86.Sse.shuffle_ps(q, q, X86.Sse.SHUFFLE(2, 3, 0, 1)));
            return q.Float0;
        }

        /// <summary>
        /// The same projection without intrinsics, for hardware without AVX and for managed runs in
        /// the validation harness.
        /// </summary>
        static bool TryProjectBoundsPortable(in PeekabooView view, AABB aabb, out float4 span, out float nearestCloseness,
                                             out float farthestCloseness)
        {
            span              = default;
            nearestCloseness  = float.NegativeInfinity;
            farthestCloseness = float.PositiveInfinity;

            var min = aabb.Min;
            var max = aabb.Max;

            // Lane layout: the four combinations of x and y, once at min.z and once at max.z.
            var cornerX = new float4(min.x, max.x, min.x, max.x) - view.origin.x;
            var cornerY = new float4(min.y, min.y, max.y, max.y) - view.origin.y;
            var nearZ   = min.z - view.origin.z;
            var farZ    = max.z - view.origin.z;

            // The x and y part of each axis is the same for both faces, and the z part is a scalar. So
            // each axis costs one four-wide pair and two broadcast adds.
            var baseX = cornerX * view.axisRightScaled.x + cornerY * view.axisRightScaled.y;
            var baseY = cornerX * view.axisUpScaled.x + cornerY * view.axisUpScaled.y;
            var baseZ = cornerX * view.axisForward.x + cornerY * view.axisForward.y;

            var viewXa = baseX + nearZ * view.axisRightScaled.z;
            var viewXb = baseX + farZ * view.axisRightScaled.z;
            var viewYa = baseY + nearZ * view.axisUpScaled.z;
            var viewYb = baseY + farZ * view.axisUpScaled.z;
            var viewZa = baseZ + nearZ * view.axisForward.z;
            var viewZb = baseZ + farZ * view.axisForward.z;

            if (math.any(math.min(viewZa, viewZb) < view.nearDistance))
                return false;
            var closenessA = math.rcp(viewZa);
            var closenessB = math.rcp(viewZb);
            var screenXa   = viewXa * closenessA + view.screenOffset.x;
            var screenXb   = viewXb * closenessB + view.screenOffset.x;
            var screenYa   = viewYa * closenessA + view.screenOffset.y;
            var screenYb   = viewYb * closenessB + view.screenOffset.y;

            // Reduce across the two faces first, then across the four lanes. A vertical min is one
            // instruction, but a horizontal one is a chain of shuffles. There are six of them, so
            // pairing the faces first halves the shuffles.
            var lower = new float2(math.cmin(math.min(screenXa, screenXb)), math.cmin(math.min(screenYa, screenYb)));
            var upper = new float2(math.cmax(math.max(screenXa, screenXb)), math.cmax(math.max(screenYa, screenYb)));
            if (!math.all(math.isfinite(lower)) || !math.all(math.isfinite(upper)))
                return false;

            nearestCloseness  = math.cmax(math.max(closenessA, closenessB));
            farthestCloseness = math.cmin(math.min(closenessA, closenessB));
            span              = new float4(lower, upper);
            return true;
        }

        /// <summary>
        /// Gives the same answer as the eight corner version for an orthographic view, in about half the
        /// instructions. Doesn't work for perspective.
        /// </summary>
        /// <remarks>
        /// Instead of projecting eight corners and reducing them, this moves the whole box into view
        /// space the way AABB.Transform does: the center goes through the basis, and the extents go
        /// through its absolute value. That gives each axis its exact range with no reduction, which
        /// is where the eight corner version spends most of its time.
        ///
        /// It's exact, not an approximation. Without a divide, both the screen position and the
        /// closeness are linear in the box, and a linear function's range over a box is exactly what
        /// the center and absolute extents give. Perspective can't do this. There, the corner that
        /// reaches furthest sideways isn't always the one closest to the eye, and bounding the two
        /// separately widens the rectangle enough to cost more than it saves.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryProjectBoundsAxial(in PeekabooView view, AABB aabb, out float4 span, out float nearestCloseness,
                                                 out float farthestCloseness)
        {
            // The absolute value goes on the product instead of the basis. Extents are never negative,
            // so this costs three ands instead of storing a second matrix.
            var offset = aabb.Center - view.origin;
            var centre = view.basis.c0 * offset.x + view.basis.c1 * offset.y + view.basis.c2 * offset.z;
            var extent = math.abs(view.basis.c0 * aabb.Extents.x) +
                         math.abs(view.basis.c1 * aabb.Extents.y) +
                         math.abs(view.basis.c2 * aabb.Extents.z);

            var low  = centre - extent;
            var high = centre + extent;

            nearestCloseness  = -low.z;
            farthestCloseness = -high.z;
            span              = new float4(low.xy, high.xy) + new float4(view.screenOffset, view.screenOffset);
            return math.all(math.isfinite(span));
        }

        /// <summary>
        /// How many entities <see cref="ProjectEightPerspective"/> takes at a time.
        /// </summary>
        public const int kLanes = 8;

        /// <summary>
        /// Projects eight entities at once, one per lane, instead of one entity across eight lanes.
        /// </summary>
        /// <remarks>
        /// Putting eight corners in eight lanes means reducing across the register at the end, and
        /// that's shuffles, which is most of the single entity version's cost. With eight entities in
        /// eight lanes, there's no reduction. Each entity's lowest corner comes from the min of eight
        /// whole registers, one per corner, and every lane keeps its own answer throughout.
        ///
        /// The math is the same and in the same order, so the results match to the bit. A corner's
        /// view position is still the x and y parts summed and then z added, and min and max don't
        /// depend on order.
        ///
        /// The caller passes in six rows of eight: center then extent, one row per axis. They're
        /// gathered, not transposed, because the source bounds aren't laid out this way and gathering
        /// eight values is cheaper than shuffling.
        /// </remarks>
        public static void ProjectEightPerspective(in PeekabooView view, float* soa, float4* spans, float* nearest, out uint accepted)
        {
            var originX = new v256(view.origin.x);
            var originY = new v256(view.origin.y);
            var originZ = new v256(view.origin.z);

            var centreX = X86.Avx.mm256_loadu_ps(soa);
            var centreY = X86.Avx.mm256_loadu_ps(soa + 8);
            var centreZ = X86.Avx.mm256_loadu_ps(soa + 16);
            var extentX = X86.Avx.mm256_loadu_ps(soa + 24);
            var extentY = X86.Avx.mm256_loadu_ps(soa + 32);
            var extentZ = X86.Avx.mm256_loadu_ps(soa + 40);

            // Both faces of the box on each axis, relative to the eye. A corner is just picking one
            // from each pair, no math needed.
            var lowX  = X86.Avx.mm256_sub_ps(X86.Avx.mm256_sub_ps(centreX, extentX), originX);
            var highX = X86.Avx.mm256_sub_ps(X86.Avx.mm256_add_ps(centreX, extentX), originX);
            var lowY  = X86.Avx.mm256_sub_ps(X86.Avx.mm256_sub_ps(centreY, extentY), originY);
            var highY = X86.Avx.mm256_sub_ps(X86.Avx.mm256_add_ps(centreY, extentY), originY);
            var lowZ  = X86.Avx.mm256_sub_ps(X86.Avx.mm256_sub_ps(centreZ, extentZ), originZ);
            var highZ = X86.Avx.mm256_sub_ps(X86.Avx.mm256_add_ps(centreZ, extentZ), originZ);

            var screenLowX  = new v256(float.PositiveInfinity);
            var screenLowY  = new v256(float.PositiveInfinity);
            var screenHighX = new v256(float.NegativeInfinity);
            var screenHighY = new v256(float.NegativeInfinity);
            var nearestSoFar = new v256(float.NegativeInfinity);
            var refused      = new v256(0);

            var absolute = new v256(0x7fffffff);
            var infinity = new v256(float.PositiveInfinity);
            var nearPlane = new v256(view.nearDistance);
            var one       = new v256(1f);

            for (int corner = 0; corner < 8; corner++)
            {
                var cornerX = (corner & 1) != 0 ? highX : lowX;
                var cornerY = (corner & 2) != 0 ? highY : lowY;
                var cornerZ = (corner & 4) != 0 ? highZ : lowZ;

                var viewX = Axis(cornerX, cornerY, cornerZ, view.axisRightScaled);
                var viewY = Axis(cornerX, cornerY, cornerZ, view.axisUpScaled);
                var viewZ = Axis(cornerX, cornerY, cornerZ, view.axisForward);

                refused = X86.Avx.mm256_or_ps(refused, X86.Avx.mm256_cmp_ps(viewZ, nearPlane, (int)X86.Avx.CMP.LT_OQ));

                var closeness = X86.Avx.mm256_div_ps(one, viewZ);
                var screenX   = X86.Avx.mm256_add_ps(X86.Avx.mm256_mul_ps(viewX, closeness), new v256(view.screenOffset.x));
                var screenY   = X86.Avx.mm256_add_ps(X86.Avx.mm256_mul_ps(viewY, closeness), new v256(view.screenOffset.y));

                // Tracks refusals instead of acceptances, so a NaN lane gets refused the same as an
                // infinite one. An ordered compare would let NaN through.
                refused = X86.Avx.mm256_or_ps(refused, X86.Avx.mm256_cmp_ps(X86.Avx.mm256_and_ps(screenX, absolute), infinity, (int)X86.Avx.CMP.NLT_UQ));
                refused = X86.Avx.mm256_or_ps(refused, X86.Avx.mm256_cmp_ps(X86.Avx.mm256_and_ps(screenY, absolute), infinity, (int)X86.Avx.CMP.NLT_UQ));

                screenLowX   = X86.Avx.mm256_min_ps(screenLowX,  screenX);
                screenHighX  = X86.Avx.mm256_max_ps(screenHighX, screenX);
                screenLowY   = X86.Avx.mm256_min_ps(screenLowY,  screenY);
                screenHighY  = X86.Avx.mm256_max_ps(screenHighY, screenY);
                nearestSoFar = X86.Avx.mm256_max_ps(nearestSoFar, closeness);
            }

            accepted = (uint)(~X86.Avx.mm256_movemask_ps(refused)) & 0xffu;
            X86.Avx.mm256_storeu_ps(nearest, nearestSoFar);

            // Transposes the four values into one float4 per entity. Each register holds two entities,
            // one per half, which is why each Store writes two.
            var pairLow  = X86.Avx.mm256_unpacklo_ps(screenLowX,  screenLowY);
            var pairHigh = X86.Avx.mm256_unpackhi_ps(screenLowX,  screenLowY);
            var farLow   = X86.Avx.mm256_unpacklo_ps(screenHighX, screenHighY);
            var farHigh  = X86.Avx.mm256_unpackhi_ps(screenHighX, screenHighY);

            Store(spans, 0, X86.Avx.mm256_shuffle_ps(pairLow,  farLow,  X86.Sse.SHUFFLE(1, 0, 1, 0)));
            Store(spans, 1, X86.Avx.mm256_shuffle_ps(pairLow,  farLow,  X86.Sse.SHUFFLE(3, 2, 3, 2)));
            Store(spans, 2, X86.Avx.mm256_shuffle_ps(pairHigh, farHigh, X86.Sse.SHUFFLE(1, 0, 1, 0)));
            Store(spans, 3, X86.Avx.mm256_shuffle_ps(pairHigh, farHigh, X86.Sse.SHUFFLE(3, 2, 3, 2)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static v256 Axis(v256 x, v256 y, v256 z, float3 axis)
        {
            var lateral = X86.Avx.mm256_add_ps(X86.Avx.mm256_mul_ps(x, new v256(axis.x)), X86.Avx.mm256_mul_ps(y, new v256(axis.y)));
            return X86.Avx.mm256_add_ps(lateral, X86.Avx.mm256_mul_ps(z, new v256(axis.z)));
        }

        /// <summary>
        /// Writes the two entities a register holds, one per half.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Store(float4* spans, int slot, v256 pair)
        {
            X86.Sse.storeu_ps(spans + slot,     X86.Avx.mm256_castps256_ps128(pair));
            X86.Sse.storeu_ps(spans + slot + 4, X86.Avx.mm256_extractf128_ps(pair, 1));
        }

        /// <summary>
        /// Every texel the span touches, inclusive on both corners and not clamped to the view. It's
        /// never empty, since a span thinner than a texel is still inside one. Occludees get tested
        /// over this, since they can only be culled once every texel they could show up in is covered.
        /// </summary>
        public static int4 TouchedRect(float4 span)
        {
            int x0 = (int)math.floor(span.x);
            int y0 = (int)math.floor(span.y);
            return new int4(x0, y0, math.max(x0, (int)math.ceil(span.z) - 1), math.max(y0, (int)math.ceil(span.w) - 1));
        }

        /// <summary>
        /// Only the texels the span fully covers, inclusive on both corners and clamped to the view.
        /// Returns false if there are none, which happens when the span is too thin to cross a texel
        /// boundary on either axis, no matter how wide it is on the other.
        /// </summary>
        public static bool TryCoveredRect(in PeekabooView view, float4 span, out int4 rect)
        {
            // Clamp as floats before the cast, so a span far outside the view can't overflow.
            int x0 = (int)math.ceil(math.max(span.x, 0f));
            int y0 = (int)math.ceil(math.max(span.y, 0f));
            int x1 = (int)math.floor(math.min(span.z, view.resolution.x)) - 1;
            int y1 = (int)math.floor(math.min(span.w, view.resolution.y)) - 1;
            rect   = new int4(x0, y0, x1, y1);
            return x1 >= x0 && y1 >= y0;
        }

        /// <summary>
        /// The fraction of the view's texels the span fully covers. An occluder is only worth the
        /// texels it can fill, so this is what scores it. A candidate that fills none scores zero, no
        /// matter how much of the screen its bounds reach across.
        /// </summary>
        public static float CoveredAreaFraction(in PeekabooView view, float4 span)
        {
            if (!TryCoveredRect(in view, span, out var rect))
                return 0f;
            return (rect.z - rect.x + 1) * (rect.w - rect.y + 1) / (float)(view.resolution.x * view.resolution.y);
        }

        /// <summary>
        /// How much of the view is behind an occluder whose nearest point has this closeness, as a
        /// fraction of the depth range. The occluder's screen coverage gets scaled by this.
        /// </summary>
        /// <remarks>
        /// It's a linear remap of view depth from the far plane (0) to the near plane (1). For
        /// orthographic views, that's exactly one minus clip space depth. For perspective, it's
        /// close to that within the near plane's share of the range, which for a camera is about a
        /// ten-thousandth.
        ///
        /// This uses depth range instead of world volume. World volume would only be right if
        /// renderers were spread evenly through the scene, and they aren't. LOD puts way more of them
        /// near the camera, so the near end of a view is worth more than its share of the volume.
        ///
        /// Without this, a caster at the back of a cascade would score the same as an identical one at
        /// the front while shadowing almost nothing, since orthographic rectangles don't shrink with
        /// distance. That's four of the five views in a frame with four cascades.
        ///
        /// It uses the nearest point, not the farthest, because the surface facing the viewer is what
        /// gets rasterized. Scoring by the back face would undervalue something that occludes fine
        /// from its front.
        /// </remarks>
        public static float DepthWeight(in PeekabooView view, float nearestCloseness)
        {
            // Closeness is 1/depth for perspective and -depth for orthographic.
            float depth = view.isPerspective ? math.rcp(nearestCloseness) : -nearestCloseness;
            float span  = view.farDistance - view.nearDistance;
            if (!(span > 0f) || !math.isfinite(depth))
                return 1f;
            return math.saturate((view.farDistance - depth) / span);
        }

        /// <summary>
        /// How much of a view an occluder with these bounds could cover, which ranks it against other
        /// candidates. Returns false if it's worth nothing.
        /// </summary>
        /// <remarks>
        /// Bounds that can't be projected aren't all worthless, and this tells the two cases apart. A
        /// mesh big enough to stand inside, like a castle, a stadium, or a tunnel, has bounds that
        /// cross the near plane whenever the camera is inside them. That's exactly when it occludes
        /// the most, since its far side fills most of the view. So it scores as covering everything.
        ///
        /// Bounds that can't be projected and don't contain the eye are something small that came
        /// within the near distance. Those score nothing. Otherwise, a few of them drifting past the
        /// camera would take every slot from the occluders that matter.
        ///
        /// This is only ranking. It doesn't decide correctness. Rule 4 handles that per plate, by
        /// rejecting any plate whose validity region doesn't contain its near plane footprint. A mesh
        /// the camera walks into ranks high, and then only rasterizes the plates that are valid from
        /// inside. For a room, that's its inner walls. For a solid, that's none of them. Don't move the
        /// guarantee back into ranking. It was tried, and a distance measured at a plate can't answer a
        /// question about the viewer.
        /// </remarks>
        public static bool TryScoreOccluder(in PeekabooView view, AABB bounds, out float fraction)
        {
            if (TryProjectBounds(in view, bounds, out var span, out var nearest, out _))
            {
                fraction = CoveredAreaFraction(in view, span) * DepthWeight(in view, nearest);
                return fraction > 0f;
            }

            fraction = 1f;
            return math.all(math.abs(view.origin - bounds.Center) <= bounds.Extents);
        }
    }

    /// <summary>
    /// A chunk the occluder gather will visit, and the chunk components it reads.
    /// </summary>
    internal unsafe struct OccluderChunk
    {
        public ArchetypeChunk                   chunk;
        public ChunkPerCameraCullingMask*       mask;
        public ChunkWorldRenderBounds*          bounds;
        public ChunkPerCameraCullingSplitsMask* splits;
    }

    /// <summary>
    /// Finds the chunks the occluder gather has work to do in, by walking the meta chunks that hold
    /// their masks instead of the chunks themselves.
    /// </summary>
    /// <remarks>
    /// Same idea as the cull job's version. By the time occlusion culling runs, most chunks in a big
    /// scene are already culled. When most renderers are occluders, visiting every chunk just to
    /// read its mask and move on was 70% of the gather's cost.
    ///
    /// A meta query can't see the components inside a chunk, so the occluder query gets checked here
    /// as a mask over the chunk's archetype. That's one comparison instead of a dozen, and it always
    /// matches the query since it's built from it.
    /// </remarks>
    [BurstCompile]
    internal unsafe partial struct FindOccluderChunksJob : IJobChunk, IInjectable
    {
        [ReadOnly, Inject] ComponentTypeHandle<ChunkPerCameraCullingMask>       perCameraMaskHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkPerCameraCullingSplitsMask> perCameraSplitsMaskHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkWorldRenderBounds>          chunkBoundsHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkHeader>                     chunkHeaderHandle;

        public EntityQueryMask                          occluders;
        public NativeList<OccluderChunk>.ParallelWriter chunksToProcess;

        [Unity.Burst.CompilerServices.SkipLocalsInit]
        public void Execute(in ArchetypeChunk metaChunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var found = stackalloc OccluderChunk[128];
            int count = 0;

            var maskBase   = metaChunk.GetComponentDataPtrRO(ref perCameraMaskHandle);
            var boundsBase = metaChunk.GetComponentDataPtrRO(ref chunkBoundsHandle);
            var heads      = metaChunk.GetNativeArray(ref chunkHeaderHandle);
            var splitsBase = metaChunk.Has(ref perCameraSplitsMaskHandle) ? metaChunk.GetComponentDataPtrRO(ref perCameraSplitsMaskHandle) : null;

            for (int i = 0; i < metaChunk.Count; i++)
            {
                var mask = maskBase[i];
                if ((mask.lower.Value | mask.upper.Value) == 0)
                    continue;
                var chunk = heads[i].ArchetypeChunk;
                if (!occluders.MatchesIgnoreFilter(chunk))
                    continue;
                found[count++] = new OccluderChunk
                {
                    chunk  = chunk,
                    mask   = maskBase + i,
                    bounds = boundsBase + i,
                    splits = splitsBase == null ? null : splitsBase + i,
                };
            }
            if (count > 0)
                chunksToProcess.AddRangeNoResize(found, count);
        }
    }

    /// <summary>
    /// Finds the entities worth rasterizing as occluders, and scores them by how much of the view
    /// they're likely to fill.
    /// </summary>
    [BurstCompile]
    internal unsafe partial struct GatherOccluderCandidatesJob : IJobParallelForDefer, IInjectable
    {
        [ReadOnly, Inject] ComponentTypeHandle<WorldRenderBounds>               boundsHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkWorldRenderBounds>          chunkBoundsHandle;
        [ReadOnly, Inject] ComponentTypeHandle<PeekabooOccluder>                occluderHandle;
        [ReadOnly, Inject] ComponentTypeHandle<MaterialMeshInfo>                mmiHandle;
        [ReadOnly, Inject] ComponentTypeHandle<PostProcessMatrix>               postProcessMatrixHandle;
        [ReadOnly, Inject] ComponentTypeHandle<LodCrossfade>                    lodCrossfadeHandle;
        [ReadOnly, Inject] ComponentTypeHandle<MeshLod>                         meshLodHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkPerCameraCullingMask>       perCameraMaskHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkPerCameraCullingSplitsMask> perCameraSplitsMaskHandle;
        [ReadOnly, Inject] WorldTransformReadOnlyAspect.TypeHandle              worldTransformHandle;

        [ReadOnly] public SharedComponentTypeHandle<RenderMeshArray> renderMeshArrayHandle;
        [ReadOnly] public OcclusionCullingContextAspect             drawContext;

        [ReadOnly] public NativeArray<OccluderChunk> chunksToProcess;

        [ReadOnly] public NativeArray<PeekabooView> views;

        /// <summary>
        /// A min heap of the best candidates each worker has seen, one per worker and view.
        /// </summary>
        /// <remarks>
        /// Keeping the best instead of the first is what makes the chosen occluders depend on the
        /// scene instead of on thread timing. Any entity in the overall top K is also in the top K of
        /// whichever worker saw it. So all the heaps together always contain the right set, no matter
        /// how the chunks got split up.
        ///
        /// It's a heap instead of a sorted list because only the weakest member ever gets read. That's
        /// what a new candidate has to beat. Once the heap is full, it's also a floor that rises during
        /// the pass and lets whole chunks get skipped.
        /// </remarks>
        [NativeDisableParallelForRestriction] public NativeArray<OccluderCandidate> threadHeaps;
        [NativeDisableParallelForRestriction] public NativeArray<int>               threadHeapCounts;
        public PeekabooStatsCounters stats;

        /// <summary>
        /// The highest floor any worker has published, per view, as raw float bits.
        /// </summary>
        /// <remarks>
        /// A worker with a full heap knows its weakest member is a floor it'll never drop below. But
        /// with the work split many ways, one worker's Kth best is pretty low compared to the whole
        /// scene's. The highest floor across all workers is much better, and still safe. A worker with
        /// K candidates at or above its floor proves the scene has at least K there too, so nothing
        /// below it can make the overall top K.
        ///
        /// The value here depends on the order chunks got handed out, but the chosen set doesn't. It
        /// only ever rejects candidates that couldn't have been selected anyway.
        /// </remarks>
        [NativeDisableParallelForRestriction] public NativeArray<int> sharedFloorBits;

        public int   maxOccludersPerView;
        public float minOccluderCoverage;
        public bool  isLightView;

        [NativeSetThreadIndex] int threadIndex;

        // A renderer drawing more meshes than this is very rare, and skipping one only makes it a
        // weaker occluder.
        const int kMaxMeshesPerEntity = 8;

        /// <summary>
        /// Raises a shared floor to at least the given score, which must not be negative.
        /// </summary>
        /// <remarks>
        /// Compares raw bits instead of floats. For non-negative floats, the bit pattern grows with the
        /// value, so an integer compare and exchange works. Scores are an area fraction times a
        /// clamped coverage, so they're never negative.
        /// </remarks>
        static void RaiseFloor(int* target, float score)
        {
            int desired = math.asint(score);
            int current = *target;
            while (desired > current)
            {
                int prior = System.Threading.Interlocked.CompareExchange(ref *target, desired, current);
                if (prior == current)
                    return;
                current = prior;
            }
        }

        public void Execute(int index) => Execute(chunksToProcess[index]);

        // The mask is known to be non-zero, and the chunk component pointers came from the meta chunk
        // that FindOccluderChunksJob found it in.
        void Execute(in OccluderChunk visit)
        {
            var chunk = visit.chunk;
            ref readonly var mask = ref *visit.mask;

            // A chunk's bounds contain all its entities, so no entity covers more of the screen than
            // its chunk, and coverage never goes above one. That makes the chunk's screen fraction an
            // upper bound on every score inside it. If the chunk is too small to be worth rasterizing,
            // so is everything in it. This mirrors the two chunk level tests in the cull job, and it's
            // what lets the gather skip most of a big scene.
            var  chunkAabb         = visit.bounds->Value;
            var  heaps             = (OccluderCandidate*)threadHeaps.GetUnsafePtr() + (long)threadIndex * views.Length * maxOccludersPerView;
            var  heapCounts        = (int*)threadHeapCounts.GetUnsafePtr() + threadIndex * views.Length;
            var  sharedFloors      = (int*)sharedFloorBits.GetUnsafePtr();
            uint viewsWorthTesting = 0;

            // Indexing the array copies the whole view, but indexing a span doesn't. A view is about
            // 300 bytes and gets read per entity per view, so the copy would be most of the cost.
            var viewSpan = views.AsReadOnlySpan();

            // Kept for the per entity tests below. Reading the floor once per chunk instead of once
            // per entity only means a few extra candidates slip in while the chunk is being walked.
            var floors = stackalloc float[views.Length];
            for (int v = 0; v < views.Length; v++)
            {
                // Publish once per chunk instead of on every eviction. The floor only goes up, so a
                // stale value only costs a chunk that could've been skipped.
                if (heapCounts[v] == maxOccludersPerView)
                    RaiseFloor(sharedFloors + v, heaps[v * maxOccludersPerView].score);
                float floor = math.max(minOccluderCoverage, math.asfloat(sharedFloors[v]));
                floors[v]   = floor;

                // Bounds that cross the near plane can't be projected. Test their entities one by one
                // instead of skipping them, since some might project fine.
                if (!PeekabooProjection.TryProjectBounds(in viewSpan[v], chunkAabb, out var chunkSpan, out var chunkNearest, out _))
                {
                    viewsWorthTesting |= 1u << v;
                    continue;
                }
                float chunkArea = PeekabooProjection.CoveredAreaFraction(in viewSpan[v], chunkSpan) *
                                  PeekabooProjection.DepthWeight(in viewSpan[v], chunkNearest);
                if (chunkArea > 0f && chunkArea >= floor)
                    viewsWorthTesting |= 1u << v;
            }
            if (viewsWorthTesting == 0)
                return;

            var  occluders   = chunk.GetComponentDataPtrRO(ref occluderHandle);
            var  mmis        = chunk.GetComponentDataPtrRO(ref mmiHandle);
            var  bounds      = chunk.GetComponentDataPtrRO(ref boundsHandle);
            var  postProcess = chunk.GetComponentDataPtrRO(ref postProcessMatrixHandle);
            var  transforms  = worldTransformHandle.Resolve(chunk);
            var  resolver    = drawContext.GetResolver(in chunk, ref renderMeshArrayHandle);
            var  drawn       = default(UnsafeList<OcclusionCullingContextAspect.MaterialMeshSubmesh>);

            // Which splits each entity is actually in. An entity that isn't in a split won't be in
            // that split's shadow map. Using it as an occluder there would claim a depth the real
            // shadow map never gets, and cull a caster whose shadow would then go missing.
            var splits = isLightView ? *visit.splits : default;

            // A chunk without LodCrossfade still answers GetEnabledMask, so check the data pointer to
            // know whether the mask means anything. Kinemation's draw command job does the same.
            var crossfadeData    = chunk.GetComponentDataPtrRO(ref lodCrossfadeHandle);
            var crossfadeEnabled = chunk.GetEnabledMask(ref lodCrossfadeHandle);

            // Plates are cut from mesh LOD 0. A coarser LOD is a different surface, and LOD 0's plates
            // can poke out of it.
            var meshLods = chunk.GetComponentDataPtrRO(ref meshLodHandle);

            var meshDraws = stackalloc OccluderMeshDraw[kMaxMeshesPerEntity];
            var fractions   = stackalloc float[views.Length];

            var enumerator = new ChunkEntityBatchEnumerator(true, new v128(mask.lower.Value, mask.upper.Value), chunk.Count);
            while (enumerator.NextRange(out var rangeStart, out var rangeCount))
            {
                for (int i = rangeStart, end = rangeStart + rangeCount; i < end; i++)
                {
                    if (!occluders[i].blob.IsCreated)
                        continue;
                    // A crossfading renderer is drawn dithered, so parts of it are missing.
                    if (crossfadeData != null && crossfadeEnabled[i])
                        continue;
                    if (meshLods != null && meshLods[i].lodLevel != 0)
                        continue;

                    // Only the views this entity is drawn in. That's the one view for a camera pass,
                    // and whichever cascades it reaches for a light pass.
                    uint entityViews = viewsWorthTesting;
                    if (isLightView)
                    {
                        uint splitMask = splits.splitMasks[i];
                        uint inSplits  = 0;
                        for (int v = 0; v < views.Length; v++)
                        {
                            if ((splitMask & (1u << viewSpan[v].splitIndex)) != 0)
                                inSplits |= 1u << v;
                        }
                        entityViews &= inSplits;
                    }
                    if (entityViews == 0)
                        continue;

                    // Check screen size before looking anything else up. Coverage never goes above
                    // one, so the screen fraction alone bounds the score. An entity too small in every
                    // view can't become a candidate no matter what meshes it has. In a big scene, that's
                    // almost every entity, so skipping the mesh lookup and matrix for them matters.
                    uint scoredViews = 0;
                    for (int v = 0; v < views.Length; v++)
                    {
                        if ((entityViews & (1u << v)) == 0)
                            continue;
                        if (!PeekabooProjection.TryScoreOccluder(in viewSpan[v], bounds[i].Value, out float fraction))
                            continue;
                        if (fraction < floors[v])
                            continue;
                        fractions[v]  = fraction;
                        scoredViews  |= 1u << v;
                    }
                    if (scoredViews == 0)
                        continue;

                    ref var blob = ref occluders[i].blob.Value;
                    if (!drawn.IsCreated)
                        drawn = new UnsafeList<OcclusionCullingContextAspect.MaterialMeshSubmesh>(8, Allocator.Temp);
                    drawn.Clear();
                    resolver.Resolve(mmis[i], ref drawn);
                    int meshes = PeekabooDrawFilter.CollectOccluderMeshes(ref blob, in drawn, meshDraws, kMaxMeshesPerEntity);
                    if (meshes == 0)
                        continue;

                    var objectToWorld = transforms[i].matrix4x4;
                    if (postProcess != null)
                    {
                        var extra = new float4x4(new float4(postProcess[i].postProcessMatrix.c0, 0f),
                                                 new float4(postProcess[i].postProcessMatrix.c1, 0f),
                                                 new float4(postProcess[i].postProcessMatrix.c2, 0f),
                                                 new float4(postProcess[i].postProcessMatrix.c3, 1f));
                        objectToWorld = math.mul(extra, objectToWorld);
                    }

                    for (int v = 0; v < views.Length; v++)
                    {
                        if ((scoredViews & (1u << v)) == 0)
                            continue;
                        float fraction = fractions[v];

                        for (int m = 0; m < meshes; m++)
                        {
                            ref var entry    = ref blob.meshes[meshDraws[m].meshIndex];
                            float   coverage = entry.boundsCrossSectionArea > 0f ? math.saturate(entry.largestPlateArea / entry.boundsCrossSectionArea) : 0f;
                            if (fraction * coverage <= 0f || fraction * coverage < floors[v])
                                continue;

                            stats.CountConsidered(threadIndex);
                            OccluderHeap.Offer(heaps + v * maxOccludersPerView, ref heapCounts[v], maxOccludersPerView, new OccluderCandidate
                            {
                                objectToWorld   = objectToWorld,
                                blob            = occluders[i].blob,
                                opaqueSubmeshes = meshDraws[m].opaqueSubmeshes,
                                meshIndex       = meshDraws[m].meshIndex,
                                viewIndex       = v,
                                score           = fraction * coverage,
                            });
                        }
                    }
                }
            }
        }
    }

    internal static unsafe class OccluderHeap
    {
        /// <summary>
        /// Adds a candidate if it beats the weakest one held, keeping the weakest at the root. The
        /// array only becomes a heap once it's full.
        /// </summary>
        public static void Offer(OccluderCandidate* heap, ref int count, int capacity, in OccluderCandidate candidate)
        {
            if (count < capacity)
            {
                // Nothing gets evicted until it's full, so there's no need to keep it ordered yet.
                // Heapifying once when it fills is linear. Sifting up on every insert costs a log per
                // candidate, for an ordering most workers never fill up enough to use.
                heap[count++] = candidate;
                if (count == capacity)
                {
                    for (int node = (capacity >> 1) - 1; node >= 0; node--)
                        SiftDown(heap, node, capacity);
                }
                return;
            }

            if (candidate.score <= heap[0].score)
                return;
            heap[0] = candidate;
            SiftDown(heap, 0, capacity);
        }

        static void SiftDown(OccluderCandidate* heap, int node, int capacity)
        {
            while (true)
            {
                int left    = node * 2 + 1;
                int right   = left + 1;
                int weakest = node;
                if (left < capacity && heap[left].score < heap[weakest].score)
                    weakest = left;
                if (right < capacity && heap[right].score < heap[weakest].score)
                    weakest = right;
                if (weakest == node)
                    return;
                (heap[node], heap[weakest]) = (heap[weakest], heap[node]);
                node                        = weakest;
            }
        }
    }

    /// <summary>
    /// Keeps the best scoring occluders per view and reserves output slots for their plates, so the
    /// plate building job can write in parallel without any bookkeeping.
    /// </summary>
    [BurstCompile]
    internal unsafe struct SelectOccludersJob : IJob
    {
        // The average plate spans far fewer rows than this. Plates past the budget fall back to
        // finding their crossings per row.
        const int kCrossingsPerPlate = 32;

        [ReadOnly] public NativeArray<OccluderCandidate> threadHeaps;
        [ReadOnly] public NativeArray<int>               threadHeapCounts;

        public int threadCount;

        public int maxOccludersPerView;
        public int viewCount;
        public int totalRows;

        public NativeArray<int>  wordsPerBand;  // One element, so later jobs can read it.
        public NativeArray<int2> viewPlateRanges;
        public NativeList<ulong> bandPlateBits;

        public NativeList<OccluderCandidate> selected;
        public NativeArray<int2>             viewRanges;
        public NativeList<ScreenPlate>       plates;
        public NativeList<float2>            plateVertices;
        public NativeList<float2>            plateCrossings;

        public void Execute()
        {
            // Each worker kept its best per view, so the views are already separate and just need to
            // be joined. Every candidate worth choosing is in here somewhere.
            var viewStart = stackalloc int[viewCount + 1];
            viewStart[0]  = 0;
            for (int v = 0; v < viewCount; v++)
            {
                int total = 0;
                for (int t = 0; t < threadCount; t++)
                    total        += threadHeapCounts[t * viewCount + v];
                viewStart[v + 1]  = viewStart[v] + total;
            }
            int count = viewStart[viewCount];

            var grouped = new NativeArray<OccluderCandidate>(math.max(count, 1), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int v = 0; v < viewCount; v++)
            {
                int cursor = viewStart[v];
                for (int t = 0; t < threadCount; t++)
                {
                    int held  = threadHeapCounts[t * viewCount + v];
                    int start = (t * viewCount + v) * maxOccludersPerView;
                    for (int i = 0; i < held; i++)
                        grouped[cursor++] = threadHeaps[start + i];
                }
            }

            // We only need to know which occluders are best, not their order. So each view gets
            // partitioned around its budget instead of sorted, which is about 2n instead of n log n.
            var items = (OccluderCandidate*)grouped.GetUnsafePtr();
            for (int v = 0; v < viewCount; v++)
            {
                int start  = viewStart[v];
                int length = viewStart[v + 1] - start;
                int taken  = math.min(length, maxOccludersPerView);
                if (taken < length)
                    PartitionByRank(items + start, length, taken);

                int outStart = selected.Length;
                for (int i = 0; i < taken; i++)
                    selected.Add(items[start + i]);
                viewRanges[v] = new int2(outStart, taken);
            }
            grouped.Dispose();

            int plateCursor  = 0;
            int vertexCursor = 0;
            for (int i = 0; i < selected.Length; i++)
            {
                var     candidate             = selected[i];
                ref var entry                 = ref candidate.blob.Value.meshes[candidate.meshIndex];
                candidate.plateReserveStart   = plateCursor;
                candidate.plateReserveCount   = entry.plateCount;
                candidate.vertexReserveStart  = vertexCursor;
                plateCursor                  += entry.plateCount;
                vertexCursor                 += entry.plateCount * PeekabooRasterizer.kMaxClippedVertices;
                selected[i]                   = candidate;
            }
            plates.Resize(plateCursor, NativeArrayOptions.ClearMemory);
            plateVertices.Resize(math.max(vertexCursor, 1), NativeArrayOptions.UninitializedMemory);
            plateCrossings.Resize(math.max(plateCursor * kCrossingsPerPlate, 1), NativeArrayOptions.UninitializedMemory);

            // The band-plate bitfield gets sized here, since this is where the plate count is known.
            // One bit per band of rows and plate, band major, so each band's bits are contiguous. It's
            // kept all zero between passes, because the rasterizer clears every word it reads. So only
            // newly grown memory needs clearing.
            int words      = (plateCursor + 63) / 64;
            wordsPerBand[0] = words;
            int bands = totalRows / PeekabooRasterizer.kRowsPerBand;
            if (bandPlateBits.Length < bands * words)
                bandPlateBits.Resize(bands * words, NativeArrayOptions.ClearMemory);

            // A view's plates are contiguous, since candidates are grouped by view and reserve plates
            // in order. So a row only needs to read its own view's words.
            for (int v = 0; v < viewCount; v++)
            {
                var range = viewRanges[v];
                if (range.y == 0)
                {
                    viewPlateRanges[v] = int2.zero;
                    continue;
                }
                var last           = selected[range.x + range.y - 1];
                viewPlateRanges[v] = new int2(selected[range.x].plateReserveStart, last.plateReserveStart + last.plateReserveCount);
            }
        }

        /// <summary>
        /// Moves the <paramref name="rank"/> highest scoring items into the first slots, in no
        /// particular order.
        /// </summary>
        /// <remarks>
        /// A quicksort that skips any subrange entirely on one side of the rank, since which side it's
        /// on is already decided. That's linear on average instead of n log n, and it moves far fewer
        /// of these 96-byte candidates than a full sort.
        /// </remarks>
        static void PartitionByRank(OccluderCandidate* items, int length, int rank)
        {
            int lo = 0, hi = length - 1;
            while (lo < hi)
            {
                float pivot = MedianOfThree(items[lo].score, items[(int)(((uint)lo + (uint)hi) >> 1)].score, items[hi].score);
                int   i     = lo, j = hi;
                while (i <= j)
                {
                    while (items[i].score > pivot)
                        i++;
                    while (items[j].score < pivot)
                        j--;
                    if (i > j)
                        break;
                    (items[i], items[j]) = (items[j], items[i]);
                    i++;
                    j--;
                }

                // Everything before i already beats everything after j, so only the side the rank is
                // on still needs work. If the rank lands between them, the cut is in a run of equal
                // scores, and any of them will do.
                if (rank - 1 <= j)
                    hi = j;
                else if (rank - 1 >= i)
                    lo = i;
                else
                    return;
            }
        }

        static float MedianOfThree(float a, float b, float c) => math.max(math.min(a, b), math.min(math.max(a, b), c));
    }

    /// <summary>
    /// Turns each selected occluder's plates into screen-space polygons with a linear depth function,
    /// and marks which rows each plate crosses. One bit per row and plate.
    /// </summary>
    /// <remarks>
    /// Building and binning are one job because at light workloads the pass is bound by latency, not
    /// work. Every dependent dispatch adds a wait, so fewer stages means a shorter pass.
    ///
    /// Plates from neighboring candidates share bitfield words, so bits get set with an atomic OR.
    /// The bitfield starts every pass at zero, since the rasterizer clears each word after reading
    /// it, so there's no separate clear either.
    ///
    /// Without the bitfield, every row would walk the whole plate list to find the few that reach it.
    /// That grows as plates times rows, while the real work only grows with the plates' total height.
    /// Reading the bits back gives each row's plates in ascending order for free.
    /// </remarks>
    [BurstCompile]
    internal unsafe struct BuildAndBinPlatesJob : IJobParallelForDefer
    {
        [ReadOnly] public NativeArray<PeekabooView>      views;
        [ReadOnly] public NativeArray<OccluderCandidate> selected;
        [ReadOnly] public NativeArray<int>               wordsPerBand;
        [ReadOnly] public NativeArray<int>               viewRowOffsets;

        [NativeDisableParallelForRestriction] public NativeArray<ScreenPlate> plates;
        [NativeDisableParallelForRestriction] public NativeArray<float2>      plateVertices;
        [NativeDisableParallelForRestriction] public NativeArray<ulong>       bandPlateBits;
        [NativeDisableParallelForRestriction] public NativeArray<float2>      plateCrossings;
        [NativeDisableParallelForRestriction] public NativeArray<int>         crossingCursor;

        public void Execute(int index)
        {
            var              candidate = selected[index];
            ref readonly var view      = ref views.AsReadOnlySpan()[candidate.viewIndex];
            ref var          mesh      = ref candidate.blob.Value.meshes[candidate.meshIndex];

            // A mirrored transform flips the winding, and with it the normal from Newell's method.
            // Without this, every front-facing plate would fail the backface test.
            var   rotation    = new float3x3(candidate.objectToWorld.c0.xyz, candidate.objectToWorld.c1.xyz, candidate.objectToWorld.c2.xyz);
            float windingSign = math.determinant(rotation) < 0f ? -1f : 1f;

            // Regions are baked in the occluder's local space, and not every shape survives being
            // transformed out of it. So the footprint gets moved into local space instead.
            var worldToObject = math.inverse(candidate.objectToWorld);

            // Single-submesh plates only add rasterizing when the shell's plates are usable and the
            // camera is outside the mesh, since the shell's plates are stronger there.
            bool skipSingleSubmesh = PeekabooDrawFilter.IsShellUsable(in mesh, candidate.opaqueSubmeshes) &&
                                     !(view.isPerspective && mesh.hull.ContainsAxisAligned(math.transform(worldToObject, view.origin)));

            var worldVertices = stackalloc float3[PeekabooRasterizer.kMaxPlateVertices];
            var screen        = stackalloc float2[PeekabooRasterizer.kMaxClippedVertices];
            var footprint     = stackalloc float3[PeekabooRasterizer.kMaxClippedVertices];
            var footScratch   = stackalloc float3[PeekabooRasterizer.kMaxClippedVertices];

            int stride    = wordsPerBand[0];
            var bits      = (long*)bandPlateBits.GetUnsafePtr();
            int rowOffset = viewRowOffsets[candidate.viewIndex];

            for (int p = 0; p < candidate.plateReserveCount; p++)
            {
                if (!TryBuild(p, in candidate, in view, in worldToObject, windingSign, skipSingleSubmesh, worldVertices, screen, footprint, footScratch,
                              out var screenPlate))
                    continue;

                int slot = candidate.vertexReserveStart + p * PeekabooRasterizer.kMaxClippedVertices;
                for (int v = 0; v < screenPlate.vertexCount; v++)
                    plateVertices[slot + v] = screen[v];
                screenPlate.vertexStart     = slot;
                screenPlate.viewIndex       = candidate.viewIndex;

                // Each plate finds where its edges cross every row boundary it covers in one walk,
                // instead of every row walking its edges again.
                int boundaries = screenPlate.rowMax - screenPlate.rowMin + 2;
                int start      = System.Threading.Interlocked.Add(ref ((int*)crossingCursor.GetUnsafePtr())[0], boundaries) - boundaries;
                if (start + boundaries <= plateCrossings.Length)
                {
                    PeekabooRasterizer.RowCrossings(screen, screenPlate.vertexCount, screenPlate.rowMin, boundaries - 1,
                                                    (float2*)plateCrossings.GetUnsafePtr() + start);
                    screenPlate.crossingStart = start;
                }
                else
                    screenPlate.crossingStart = -1;

                int plateIndex     = candidate.plateReserveStart + p;
                plates[plateIndex] = screenPlate;

                // A plate's rows are numbered within its view, but the bitfield counts bands across all
                // views. Views start on a band boundary, since resolutions are powers of two of at least
                // eight.
                int  word      = plateIndex >> 6;
                long bit       = 1L << (plateIndex & 63);
                int  firstBand = (rowOffset + screenPlate.rowMin) / PeekabooRasterizer.kRowsPerBand;
                int  lastBand  = (rowOffset + screenPlate.rowMax) / PeekabooRasterizer.kRowsPerBand;
                for (int band = firstBand; band <= lastBand; band++)
                    AtomicOr(bits + band * stride + word, bit);
            }
        }

        static void AtomicOr(long* target, long bit)
        {
            long current = *target;
            while ((current & bit) == 0)
            {
                long prior = System.Threading.Interlocked.CompareExchange(ref *target, current | bit, current);
                if (prior == current)
                    return;
                current = prior;
            }
        }

        static bool TryBuild(int plateIndex, in OccluderCandidate candidate, in PeekabooView view, in float4x4 worldToObject, float windingSign,
                             bool skipSingleSubmesh, float3* worldVertices, float2* screen, float3* footprint, float3* footScratch, out ScreenPlate screenPlate)
        {
            screenPlate   = default;
            ref var blob  = ref candidate.blob.Value;
            ref var entry = ref blob.meshes[candidate.meshIndex];
            ref var plate = ref blob.plates[entry.plateStart + plateIndex];

            int count = math.min(plate.vertexCount, PeekabooRasterizer.kMaxPlateVertices);
            if (count < 3)
                return false;
            if (!PeekabooDrawFilter.IsPlateUsable(in plate, in entry, candidate.opaqueSubmeshes))
                return false;
            if (skipSingleSubmesh && PeekabooDrawFilter.IsNarrowed(in plate))
                return false;
            for (int v = 0; v < count; v++)
                worldVertices[v] = math.transform(candidate.objectToWorld, blob.vertices[plate.vertexStart + v]);

            // Rule 4. Nothing closer than the near plane gets drawn, so where the rays that can reach
            // this plate cross the near plane is effectively where the viewer stands. The plate can
            // only be trusted if that's inside its region.
            int footCount = PeekabooRasterizer.BuildNearPlaneFootprint(in view, worldVertices, count, footprint, footScratch);
            if (footCount < 3)
                return false;
            for (int v = 0; v < footCount; v++)
                footprint[v] = math.transform(worldToObject, footprint[v]);
            if (!PeekabooPlateRegion.Contains(plate.regionKind, plate.regionA, plate.regionB, in entry.hull, footprint, footCount))
                return false;

            return PeekabooRasterizer.TryBuildScreenPlate(in view, worldVertices, count, plate.coneCosHalfAngle, plate.doubleSided, windingSign, screen, out screenPlate);
        }
    }

    /// <summary>
    /// Clears and rasterizes one pair of pixel rows in one view, then reduces it into a row of level
    /// one of both pyramids. Whichever pair finishes a view last builds the rest of its pyramids.
    /// </summary>
    /// <remarks>
    /// Clearing, rasterizing, and the first reduction share rows, so each pair does all three while
    /// the rows are still in cache. The last pair per view takes the small tail of the pyramid, found
    /// with a counter instead of a separate job. Together that removes three dependent dispatches from
    /// the pass, which is most of what the occluder side costs at light workloads.
    ///
    /// A pair is the smallest item the first reduction allows. Bigger items made the rows around the
    /// horizon, which hold most of the spans, pile up on a few workers. The two pairs in a band share
    /// its bitfield words, and whichever reads them second clears them.
    ///
    /// Each plate's edge crossings come from the build job, which finds all of a plate's rows in one
    /// walk of its edges. A row just reads its two.
    /// </remarks>
    [BurstCompile]
    internal unsafe struct RasterizeAndReduceJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<PeekabooView> views;
        [ReadOnly] public NativeArray<int>          viewRowOffsets;
        [ReadOnly] public NativeArray<ScreenPlate>  plates;
        [ReadOnly] public NativeArray<float2>       plateVertices;
        [ReadOnly] public NativeArray<float2>       plateCrossings;
        [ReadOnly] public NativeArray<int>          wordsPerBand;
        [ReadOnly] public NativeArray<int2>         viewPlateRanges;

        [NativeDisableParallelForRestriction] public NativeArray<ulong> bandPlateBits;
        [NativeDisableParallelForRestriction] public NativeArray<float> depth;
        [NativeDisableParallelForRestriction] public NativeArray<float> depthMax;
        [NativeDisableParallelForRestriction] public NativeArray<int>   pairsDonePerView;
        [NativeDisableParallelForRestriction] public NativeArray<int>   pairsDonePerBand;

        // Work items are visited in a scattered order, so the heavy rows around the horizon, which are
        // neighbors, don't all land in one worker's range. Coprime with the item count, so every item
        // is still visited exactly once.
        public int itemCount;
        public int bandStride;

        const int kPairsPerBand = PeekabooRasterizer.kRowsPerBand / 2;

        static readonly ProfilerMarker s_clearMarker  = new ProfilerMarker("Peekaboo Raster Clear");
        static readonly ProfilerMarker s_spansMarker  = new ProfilerMarker("Peekaboo Raster Row Spans");
        static readonly ProfilerMarker s_reduceMarker = new ProfilerMarker("Peekaboo Raster Reduce");
        static readonly ProfilerMarker s_tailMarker   = new ProfilerMarker("Peekaboo Pyramid Tail");

        /// <summary>
        /// How many work items a pass with this many rows has. Each item is a pair of rows.
        /// </summary>
        public static int ItemCount(int totalRows) => totalRows / 2;

        public static int ChooseStride(int itemCount)
        {
            int stride = 97;
            while (stride > 1 && Gcd(stride, itemCount) != 1)
                stride -= 2;
            return math.max(stride, 1);
        }

        static int Gcd(int a, int b)
        {
            while (b != 0)
                (a, b) = (b, a % b);
            return a;
        }

        public void Execute(int item)
        {
            // Each item is one pair of rows, which is one row of pyramid level one.
            int pair      = (int)((long)item * bandStride % itemCount);
            int band      = pair / kPairsPerBand;
            int globalRow = pair * 2;
            int viewIndex = 0;
            while (viewIndex + 1 < views.Length && globalRow >= viewRowOffsets[viewIndex + 1])
                viewIndex++;
            ref readonly var view = ref views.AsReadOnlySpan()[viewIndex];
            int firstRow = globalRow - viewRowOffsets[viewIndex];

            var row0 = (float*)depth.GetUnsafePtr() + view.bufferOffset + firstRow * view.resolution.x;
            var row1 = row0 + view.resolution.x;
            using (PeekabooProfiling.Auto(in s_clearMarker))
            {
                for (int x = 0; x < view.resolution.x * 2; x++)
                    row0[x] = PeekabooDepthBuffer.kEmpty;
            }

            // A view's plates are contiguous, so this band only reads the words covering them, not the
            // whole bitfield. A view's plates only mark its own bands, so these are the only words
            // this band can have set.
            var plateRange = viewPlateRanges[viewIndex];
            if (plateRange.y > plateRange.x)
            {
                int stride    = wordsPerBand[0];
                int firstWord = plateRange.x >> 6;
                int lastWord  = (plateRange.y - 1) >> 6;
                var bandBits  = (ulong*)bandPlateBits.GetUnsafePtr() + band * stride;
                RasterizePair(in view, firstRow, row0, row1, bandBits, firstWord, lastWord);

                // Cleared once both pairs in the band have read it, which leaves the bitfield all zero
                // for the next pass. The increment is a full fence, so the other pair is done reading.
                if (System.Threading.Interlocked.Increment(ref ((int*)pairsDonePerBand.GetUnsafePtr())[band]) == kPairsPerBand)
                {
                    for (int word = firstWord; word <= lastWord; word++)
                        bandBits[word] = 0ul;
                }
            }

            var chain    = (float*)depth.GetUnsafePtr() + view.mipChainOffset;
            var maxChain = (float*)depthMax.GetUnsafePtr() + view.mipChainOffset;
            using (PeekabooProfiling.Auto(in s_reduceMarker))
                PeekabooDepthBuffer.ReduceFirstLevel(chain, maxChain, view.resolution, firstRow / 2);

            // The increment is a full fence, so the last pair sees every other pair's level one rows.
            int done = System.Threading.Interlocked.Increment(ref ((int*)pairsDonePerView.GetUnsafePtr())[viewIndex]);
            if (done == view.resolution.y / 2)
            {
                using (PeekabooProfiling.Auto(in s_tailMarker))
                    PeekabooDepthBuffer.BuildPyramidsFromLevel2(chain, maxChain, view.resolution);
            }
        }

        // Both rows share one walk of the band's bits, and each plate is read once for the pair.
        [Unity.Burst.CompilerServices.SkipLocalsInit]
        void RasterizePair(in PeekabooView view, int firstRow, float* row0, float* row1, ulong* bandBits, int firstWord, int lastWord)
        {
            var spansMarker = PeekabooProfiling.Auto(in s_spansMarker);
            int capacity    = 0;
            for (int word = firstWord; word <= lastWord; word++)
                capacity += math.countbits(bandBits[word]);
            capacity       = math.max(math.min(capacity, PeekabooRasterizer.kMaxSpansPerRow), 1);
            var spans0     = stackalloc PlateSpan[capacity];
            var spans1     = stackalloc PlateSpan[capacity];
            int count0     = 0;
            int count1     = 0;
            var vertices   = (float2*)plateVertices.GetUnsafeReadOnlyPtr();
            var crossings  = (float2*)plateCrossings.GetUnsafeReadOnlyPtr();
            var platesPtr  = (ScreenPlate*)plates.GetUnsafeReadOnlyPtr();

            // Ascending plate order, which is what everything downstream expects. Each row keeps the
            // first plates up to the cap.
            for (int word = firstWord; word <= lastWord; word++)
            {
                ulong bits = bandBits[word];
                while (bits != 0)
                {
                    if (count0 >= PeekabooRasterizer.kMaxSpansPerRow && count1 >= PeekabooRasterizer.kMaxSpansPerRow)
                        goto Sweep;
                    int index  = (word << 6) + math.tzcnt(bits);
                    bits      &= bits - 1;
                    var plate  = platesPtr + index;
                    if (firstRow + 1 < plate->rowMin || firstRow > plate->rowMax)
                        continue;

                    if (count0 < PeekabooRasterizer.kMaxSpansPerRow && TryGetSpan(plate, firstRow, vertices, crossings, out var span0))
                        spans0[count0++] = span0;
                    if (count1 < PeekabooRasterizer.kMaxSpansPerRow && TryGetSpan(plate, firstRow + 1, vertices, crossings, out var span1))
                        spans1[count1++] = span1;
                }
            }

            Sweep:
            spansMarker.Dispose();
            if (count0 > 0)
                PeekabooRasterizer.SweepRow(spans0, count0, firstRow, view.resolution.x, row0);
            if (count1 > 0)
                PeekabooRasterizer.SweepRow(spans1, count1, firstRow + 1, view.resolution.x, row1);
        }

        static bool TryGetSpan(ScreenPlate* plate, int row, float2* vertices, float2* crossings, out PlateSpan span)
        {
            if (row < plate->rowMin || row > plate->rowMax)
            {
                span = default;
                return false;
            }
            if (plate->crossingStart >= 0)
            {
                var top    = crossings[plate->crossingStart + row - plate->rowMin];
                var bottom = crossings[plate->crossingStart + row + 1 - plate->rowMin];
                span       = new PlateSpan { leftAtTop = top.x, rightAtTop = top.y, leftAtBottom = bottom.x, rightAtBottom = bottom.y };
                if (!(top.x <= top.y && bottom.x <= bottom.y))
                    return false;
            }
            else if (!PeekabooRasterizer.RowSpan(vertices + plate->vertexStart, plate->vertexCount, row, out span))
                return false;
            span.gradient = plate->gradient;
            span.constant = plate->constant;
            return true;
        }
    }

    /// <summary>
    /// A chunk the cull job will visit, and pointers to everything it needs from that chunk.
    /// </summary>
    /// <remarks>
    /// These all live in chunk components, which are stored in meta chunks. FindChunksToCullJob is
    /// already walking those meta chunks, so it grabs the pointers once instead of leaving tens of
    /// thousands of lookups for later. Nothing changes structurally during a pass, so the pointers
    /// stay valid.
    /// </remarks>
    internal unsafe struct OccludeeChunk
    {
        public ChunkPerCameraCullingMask*       mask;
        public ChunkPerCameraCullingSplitsMask* splits;
        public WorldRenderBounds*               entityBounds;

        // One bit per view, set where the bounds could be projected. Bounds crossing the near plane
        // can't be, and that chunk has to be tested entity by entity. This lives here instead of in
        // each projection because it fits in the padding after the pointers, and projections get read
        // far more often.
        public uint projectedViews;
    }

    /// <summary>
    /// Where a chunk's bounds land in one view, computed ahead of time by FindChunksToCullJob.
    /// </summary>
    /// <remarks>
    /// The rectangle is what the chunk gets tested over, and the two closenesses bound everything
    /// inside it. That's all either chunk level test needs. FindChunksToCullJob has time to spare,
    /// and the bounds never need to be read again.
    /// </remarks>
    internal struct ChunkProjection
    {
        // Clamped to the view before storing, which a query would do first anyway, and which lets
        // texel coordinates fit in 16 bits. This is written once and read once per chunk per view,
        // so its size is most of its cost.
        public ushort x0, y0, x1, y1;
        public float  nearestCloseness;
        public float  farthestCloseness;

        public int4 rect => new int4(x0, y0, x1, y1);
    }

    /// <summary>
    /// Collects the chunks the cull job has work to do in, reading their masks from meta chunks
    /// instead of visiting every chunk.
    /// </summary>
    /// <remarks>
    /// By the time occlusion culling runs, frustum culling has already dropped most chunks in a big
    /// scene. Visiting them just to read a mask and move on cost more than the chunks with actual
    /// work. A meta chunk holds the masks of 128 chunks side by side, so this reads them densely and
    /// only passes on the survivors.
    ///
    /// The occludee opt-out tag gets checked here, since the query is over meta chunks and can't
    /// express it. The tag isn't enableable, so one check covers the whole chunk.
    /// </remarks>
    [BurstCompile]
    internal unsafe partial struct FindChunksToCullJob : IJobChunk, IInjectable
    {
        // Write access, because the cull job writes the mask through the pointer taken here, and
        // someone has to bump the change version. Getting the array as writable bumps it on the meta
        // chunk, which is the same thing a per chunk GetChunkComponentRefRW would do.
        [Inject]           ComponentTypeHandle<ChunkPerCameraCullingMask>       perCameraMaskHandle;
        [Inject]           ComponentTypeHandle<ChunkPerCameraCullingSplitsMask> perCameraSplitsMaskHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkWorldRenderBounds>          chunkBoundsHandle;
        [ReadOnly, Inject] ComponentTypeHandle<WorldRenderBounds>               entityBoundsHandle;
        [ReadOnly, Inject] ComponentTypeHandle<ChunkHeader>                     chunkHeaderHandle;
        [ReadOnly, Inject] ComponentTypeHandle<PeekabooDisableOccludeeTag>      disableOccludeeHandle;

        [ReadOnly] public NativeArray<PeekabooView> views;

        // Written at indices this job reserves for itself, so an entry and its projections share an
        // index. Two parallel writers couldn't promise that, but one reservation for both can.
        [NativeDisableParallelForRestriction] public NativeArray<OccludeeChunk>   entries;
        [NativeDisableParallelForRestriction] public NativeArray<ChunkProjection> projections;
        [NativeDisableUnsafePtrRestriction]   public int*                         entryCount;

        [Unity.Burst.CompilerServices.SkipLocalsInit]
        public void Execute(in ArchetypeChunk metaChunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var found       = stackalloc OccludeeChunk[128];
            var foundBounds = stackalloc AABB[128];
            int count       = 0;

            // These are all chunk components, which live side by side in this meta chunk. So grab the
            // pointers once here.
            var maskBase   = metaChunk.GetComponentDataPtrRO(ref perCameraMaskHandle);
            var boundsBase = metaChunk.GetComponentDataPtrRO(ref chunkBoundsHandle);
            var heads      = metaChunk.GetNativeArray(ref chunkHeaderHandle);
            var splitsBase = metaChunk.Has(ref perCameraSplitsMaskHandle) ? metaChunk.GetComponentDataPtrRO(ref perCameraSplitsMaskHandle) : null;

            for (int i = 0; i < metaChunk.Count; i++)
            {
                var mask = maskBase[i];
                if ((mask.lower.Value | mask.upper.Value) == 0)
                    continue;
                var chunk = heads[i].ArchetypeChunk;
                if (chunk.Has(ref disableOccludeeHandle))
                    continue;
                foundBounds[count] = boundsBase[i].Value;
                found[count++]     = new OccludeeChunk
                {
                    mask         = maskBase + i,
                    splits       = splitsBase == null ? null : splitsBase + i,
                    entityBounds = chunk.GetComponentDataPtrRO(ref entityBoundsHandle),
                };
            }
            if (count == 0)
                return;

            // Asking for write access bumps the change version, and the cull job is about to write
            // through the pointers above. The returned pointer isn't needed. A meta chunk that found
            // nothing gets skipped, so it isn't marked changed for nothing.
            metaChunk.GetComponentDataPtrRW(ref perCameraMaskHandle);
            if (splitsBase != null)
                metaChunk.GetComponentDataPtrRW(ref perCameraSplitsMaskHandle);

            int viewCount = views.Length;
            var viewSpan  = views.AsReadOnlySpan();
            int start     = System.Threading.Interlocked.Add(ref UnsafeUtility.AsRef<int>(entryCount), count) - count;
            for (int i = 0; i < count; i++)
            {
                var  entry     = found[i];
                uint projected = 0;
                for (int v = 0; v < viewCount; v++)
                {
                    ref readonly var view = ref viewSpan[v];
                    var slot = default(ChunkProjection);
                    if (PeekabooProjection.TryProjectBounds(in view, foundBounds[i], out var span, out var nearest, out var farthest))
                    {
                        var rect                = math.clamp(PeekabooProjection.TouchedRect(span), int4.zero, view.resolutionMaxTexel);
                        slot.x0                 = (ushort)rect.x;
                        slot.y0                 = (ushort)rect.y;
                        slot.x1                 = (ushort)rect.z;
                        slot.y1                 = (ushort)rect.w;
                        slot.nearestCloseness   = nearest;
                        slot.farthestCloseness  = farthest;
                        projected              |= 1u << v;
                    }
                    projections[(start + i) * viewCount + v] = slot;
                }
                entry.projectedViews = projected;
                entries[start + i]   = entry;
            }
        }
    }

    /// <summary>
    /// Clears the culling mask bits of entities hidden in every view they were visible in.
    /// </summary>
    [BurstCompile]
    internal unsafe partial struct CullOccludeesJob : IJobParallelForDefer, IInjectable
    {
        [Inject] ComponentTypeHandle<ChunkPerCameraCullingMask>       perCameraMaskHandle;
        [Inject] ComponentTypeHandle<ChunkPerCameraCullingSplitsMask> perCameraSplitsMaskHandle;

        [ReadOnly] public NativeArray<OccludeeChunk>   chunksToProcess;
        [ReadOnly] public NativeArray<ChunkProjection> projections;

        [ReadOnly] public NativeArray<PeekabooView> views;
        [ReadOnly] public NativeArray<float>        depth;
        [ReadOnly] public NativeArray<float>        depthMax;

        public bool isLightView;

        public PeekabooStatsCounters stats;
        [NativeSetThreadIndex] int   threadIndex;

        public void Execute(int index) =>
            Execute(chunksToProcess[index], (ChunkProjection*)projections.GetUnsafeReadOnlyPtr() + (long)index * views.Length);

        // The mask is known to be non-zero. FindChunksToCullJob skipped the ones that weren't, and
        // nothing writes it in between.
        void Execute(in OccludeeChunk entry, ChunkProjection* projected)
        {
            ref var mask   = ref *entry.mask;
            var     bounds = entry.entityBounds;
            if (bounds == null)
                return;

            var chain    = (float*)depth.GetUnsafeReadOnlyPtr();
            var maxChain = (float*)depthMax.GetUnsafeReadOnlyPtr();

            // A whole chunk hidden in every view is the cheap, common case near a big occluder.
            if (IsHiddenInAllViews(chain, projected, entry.projectedViews))
            {
                stats.CountCulled(threadIndex, math.countbits(mask.lower.Value) + math.countbits(mask.upper.Value), 0);
                mask = default;
                return;
            }

            // The opposite case, and the one that decides how this scales. In a big scene, most chunks
            // that survive frustum culling aren't behind anything. One read of the max pyramid over the
            // chunk's bounds settles that for every entity in it.
            if (IsUnoccludedInAllViews(chain, maxChain, projected, entry.projectedViews))
            {
                stats.CountCulled(threadIndex, 0, math.countbits(mask.lower.Value) + math.countbits(mask.upper.Value));
                return;
            }

            var splits = isLightView ? entry.splits : null;
            var tally  = new CullTally();
            if (splits == null)
                TestEntitiesInOneView(chain, bounds, ref mask, ref tally);
            else
                TestEntitiesInSplits(chain, bounds, ref mask, splits, ref tally);
            stats.CountCulled(threadIndex, in tally);
        }

        bool IsHiddenInAllViews(float* chain, ChunkProjection* projected, uint projectedViews)
        {
            var viewSpan = views.AsReadOnlySpan();
            for (int v = 0; v < views.Length; v++)
            {
                // Can't say anything about a view the bounds didn't project in.
                if ((projectedViews & (1u << v)) == 0)
                    return false;
                ref readonly var view = ref viewSpan[v];
                if (!PeekabooDepthBuffer.IsRectOccluded(in view, chain + view.mipChainOffset, projected[v].rect, projected[v].nearestCloseness))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Whether nothing inside these bounds can be hidden in any view.
        /// </summary>
        /// <remarks>
        /// Everything inside the bounds is at least as close as the farthest corner, and covers no more
        /// of the screen than the bounds do. So if no texel under the bounds stores a depth closer than
        /// that corner, nothing inside can be hidden either.
        /// </remarks>
        bool IsUnoccludedInAllViews(float* chainMin, float* maxChain, ChunkProjection* projected, uint projectedViews)
        {
            var viewSpan = views.AsReadOnlySpan();
            for (int v = 0; v < views.Length; v++)
            {
                if ((projectedViews & (1u << v)) == 0)
                    return false; // Can't say anything about this view, so the chunk takes the slow path.
                ref readonly var view = ref viewSpan[v];
                if (!PeekabooDepthBuffer.IsRectUnoccluded(in view, chainMin + view.mipChainOffset, maxChain + view.mipChainOffset,
                                                          projected[v].rect, projected[v].farthestCloseness))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests every entity still visible in a chunk against a camera pass's one view. It projects
        /// all of them before reading the pyramid for any of them.
        /// </summary>
        /// <remarks>
        /// Doing one entity at a time puts a branch on the pyramid read between one projection and the
        /// next. That branch predicts badly, since whether an entity is hidden is exactly what we're
        /// trying to find out. Every miss throws away the CPU's work on the next projection. A
        /// projection is a long dependency chain with a divide in the middle, so overlapping them is
        /// where the savings are. A second one costs about a quarter of what its instructions suggest.
        ///
        /// Split in two, the first loop is only projections. They don't depend on each other, and only
        /// branch when bounds cross the near plane, which is rare and predicts well. So they pipeline.
        /// The second loop keeps the unpredictable branch, but only the pyramid read is behind it,
        /// which is a few instructions instead of about 150.
        /// </remarks>
        [Unity.Burst.CompilerServices.SkipLocalsInit]
        void TestEntitiesInOneView(float* chain, WorldRenderBounds* bounds, ref ChunkPerCameraCullingMask mask, ref CullTally tally)
        {
            var spans   = stackalloc float4[128];
            var nearest = stackalloc float[128];
            var slots   = stackalloc int[128];
            int pending = 0;

            ref readonly var view = ref views.AsReadOnlySpan()[0];

            // List the entities to test up front, so the projection gets whole groups instead of
            // finding them one bit at a time.
            int wanted = 0;
            for (int word = 0; word < 2; word++)
            {
                ulong bits = word == 0 ? mask.lower.Value : mask.upper.Value;
                for (int i = math.tzcnt(bits); i < 64; bits ^= 1ul << i, i = math.tzcnt(bits))
                    slots[wanted++] = i + word * 64;
            }

            int taken = 0;
            if (X86.Avx.IsAvxSupported && view.isPerspective)
            {
                var soa       = stackalloc float[6 * PeekabooProjection.kLanes];
                var groupSpan = stackalloc float4[PeekabooProjection.kLanes];
                var groupNear = stackalloc float[PeekabooProjection.kLanes];

                for (; taken + PeekabooProjection.kLanes <= wanted; taken += PeekabooProjection.kLanes)
                {
                    for (int k = 0; k < PeekabooProjection.kLanes; k++)
                    {
                        var b = bounds[slots[taken + k]].Value;
                        soa[k]      = b.Center.x;
                        soa[8 + k]  = b.Center.y;
                        soa[16 + k] = b.Center.z;
                        soa[24 + k] = b.Extents.x;
                        soa[32 + k] = b.Extents.y;
                        soa[40 + k] = b.Extents.z;
                    }

                    PeekabooProjection.ProjectEightPerspective(in view, soa, groupSpan, groupNear, out uint accepted);
                    for (int k = 0; k < PeekabooProjection.kLanes; k++)
                    {
                        if ((accepted & (1u << k)) == 0)
                        {
                            // Couldn't project it, so it stays.
                            tally.Survived();
                            continue;
                        }
                        spans[pending]   = groupSpan[k];
                        nearest[pending] = groupNear[k];
                        slots[pending]   = slots[taken + k];
                        pending++;
                    }
                }
            }

            for (; taken < wanted; taken++)
            {
                int entity = slots[taken];
                if (!PeekabooProjection.TryProjectBounds(in view, bounds[entity].Value, out var span, out var near, out _))
                {
                    tally.Survived();
                    continue;
                }
                spans[pending]   = span;
                nearest[pending] = near;
                slots[pending]   = entity;
                pending++;
            }

            var levelBase = chain + view.mipChainOffset;
            for (int p = 0; p < pending; p++)
            {
                if (!PeekabooDepthBuffer.IsRectOccluded(in view, levelBase, PeekabooProjection.TouchedRect(spans[p]), nearest[p]))
                {
                    tally.Survived();
                    continue;
                }
                int entity = slots[p];
                if (entity < 64)
                    mask.lower.SetBits(entity, false);
                else
                    mask.upper.SetBits(entity - 64, false);
                tally.Culled();
            }
        }

        /// <summary>
        /// The same split, for a shadow pass. An entity gets tested against each split it's in, and
        /// only gets culled once it's hidden in all of them.
        /// </summary>
        /// <remarks>
        /// This goes one view at a time instead of one entity at a time, so each pass over the chunk
        /// is again a run of independent projections. The splits mask tracks which splits each entity
        /// is still visible in. Each view's pass clears bits from it, and the last pass reads it to
        /// decide.
        /// </remarks>
        [Unity.Burst.CompilerServices.SkipLocalsInit]
        void TestEntitiesInSplits(float* chain, WorldRenderBounds* bounds, ref ChunkPerCameraCullingMask mask,
                                  ChunkPerCameraCullingSplitsMask* splits, ref CullTally tally)
        {
            var spans    = stackalloc float4[128];
            var nearest  = stackalloc float[128];
            var slots    = stackalloc int[128];
            var viewSpan = views.AsReadOnlySpan();

            for (int v = 0; v < views.Length; v++)
            {
                ref readonly var view = ref viewSpan[v];
                uint bit     = 1u << view.splitIndex;
                int  pending = 0;

                for (int word = 0; word < 2; word++)
                {
                    ulong bits = word == 0 ? mask.lower.Value : mask.upper.Value;
                    for (int i = math.tzcnt(bits); i < 64; bits ^= 1ul << i, i = math.tzcnt(bits))
                    {
                        int entity = i + word * 64;
                        if ((splits->splitMasks[entity] & bit) == 0)
                            continue;
                        if (!PeekabooProjection.TryProjectBounds(in view, bounds[entity].Value, out var span, out var near, out _))
                            continue;
                        spans[pending]   = span;
                        nearest[pending] = near;
                        slots[pending]   = entity;
                        pending++;
                    }
                }

                var levelBase = chain + view.mipChainOffset;
                for (int p = 0; p < pending; p++)
                {
                    if (PeekabooDepthBuffer.IsRectOccluded(in view, levelBase, PeekabooProjection.TouchedRect(spans[p]), nearest[p]))
                        splits->splitMasks[slots[p]] &= (byte)~bit;
                }
            }

            for (int word = 0; word < 2; word++)
            {
                ulong bits = word == 0 ? mask.lower.Value : mask.upper.Value;
                for (int i = math.tzcnt(bits); i < 64; bits ^= 1ul << i, i = math.tzcnt(bits))
                {
                    if (splits->splitMasks[i + word * 64] != 0)
                    {
                        tally.Survived();
                        continue;
                    }
                    if (word == 0)
                        mask.lower.SetBits(i, false);
                    else
                        mask.upper.SetBits(i, false);
                    tally.Culled();
                }
            }
        }
    }

    /// <summary>
    /// Adds this pass to PeekabooStats. Only scheduled when stats are compiled in.
    /// </summary>
    [BurstCompile]
    internal struct AccumulateStatsJob : IJob
    {
        [ReadOnly] public NativeArray<PeekabooView>      views;
        [ReadOnly] public NativeArray<OccluderCandidate> selected;
        [ReadOnly] public NativeArray<ScreenPlate>       plates;
        [ReadOnly] public NativeArray<int>               threadHeapCounts;
        public PeekabooStatsCounters                     counters;

        public ComponentLookup<PeekabooStats> statsLookup;
        public Entity                         blackboardEntity;
        public bool                           isFirstPassOfFrame;
        public bool                           isLightView;

        public void Execute()
        {
            var stats = isFirstPassOfFrame ? default : statsLookup[blackboardEntity];
            stats.passCount++;
            stats.lightPassCount += isLightView ? 1 : 0;
            stats.viewCount      += views.Length;
            counters.AddTo(ref stats);
            for (int i = 0; i < threadHeapCounts.Length; i++)
                stats.peakWorkerCandidateCount  = math.max(stats.peakWorkerCandidateCount, threadHeapCounts[i]);
            stats.occluderCount                += selected.Length;
            for (int i = 0; i < plates.Length; i++)
            {
                if (plates[i].vertexCount >= 3)
                    stats.plateCount++;
            }
            statsLookup[blackboardEntity] = stats;
        }
    }
}

