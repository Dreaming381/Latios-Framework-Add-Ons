using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;

namespace Latios.Peekaboo
{
    /// <summary>
    /// A screen-space convex plate ready to rasterize, along with the linear function for its
    /// closeness anywhere on screen.
    /// </summary>
    internal struct ScreenPlate
    {
        public int    vertexStart;
        public int    vertexCount;
        public float2 gradient;
        public float  constant;
        public int    rowMin;
        public int    rowMax;
        public int    viewIndex;
        // Where this plate's row crossings start, or -1 if they didn't fit and rows must find their own.
        public int    crossingStart;
    }

    /// <summary>
    /// One plate's part of one pixel row, stored as where its left and right edges cross the top and
    /// bottom of the row.
    /// </summary>
    /// <remarks>
    /// Keeping both crossings, instead of just the range they share, is what makes welding work along
    /// a slanted seam. Two plates sharing an edge agree on that edge at every height, so comparing
    /// them at the top and the bottom proves there's no gap between them. Comparing only the ranges
    /// each one fully covers would find a gap as wide as the seam's slant across the row.
    /// </remarks>
    internal struct PlateSpan
    {
        public float  leftAtTop;
        public float  rightAtTop;
        public float  leftAtBottom;
        public float  rightAtBottom;
        public float2 gradient;
        public float  constant;

        /// <summary>The x range this plate covers over the full height of the row, on its own.</summary>
        public float innerMin => math.max(leftAtTop, leftAtBottom);
        public float innerMax => math.min(rightAtTop, rightAtBottom);

        /// <summary>The x range this plate touches anywhere within the row.</summary>
        public float outerMin => math.min(leftAtTop, leftAtBottom);
        public float outerMax => math.max(rightAtTop, rightAtBottom);

        /// <summary>
        /// Whether this plate's left edge stays at or left of the other's right edge across the whole
        /// row. If so, together they cover everything between them.
        /// </summary>
        public bool ConnectsAfter(in PlateSpan previous, float tolerance) =>
        leftAtTop <= previous.rightAtTop + tolerance && leftAtBottom <= previous.rightAtBottom + tolerance;
    }

    /// <summary>
    /// Conservative scanline rasterization of convex plates into a software depth buffer.
    /// </summary>
    /// <remarks>
    /// A pixel only gets a depth when occluder geometry covers all of it. That's what lets the results
    /// work against a render target of unknown and much higher resolution. If one of these pixels is
    /// fully covered, that's true in world space, no matter how finely the real target samples it.
    ///
    /// Splitting a quad into two triangles would break this, since a pixel on the shared diagonal
    /// isn't fully covered by either half. Two things handle that. Plates are convex polygons instead
    /// of triangles, so the baker never puts a seam like that inside a flat face. And spans that touch
    /// or overlap in a row get welded. If the spans together cover a pixel that no single span does,
    /// the pixel gets the farthest of their depths, which every part of the pixel is still in front
    /// of.
    /// </remarks>
    internal static unsafe class PeekabooRasterizer
    {
        public const int kMaxSpansPerRow   = 512;
        public const int kMaxPlateVertices = 16;

        /// <summary>
        /// How many rows plates get binned by, and how many rows one rasterizing work item covers.
        /// </summary>
        /// <remarks>
        /// Binning by row costs a bitfield write per plate per row. A band of four cuts that by four,
        /// and a plate that only grazes a band gets rejected by its row range.
        /// Resolutions are powers of two of at least eight, so views always start on a band boundary.
        /// </remarks>
        public const int kRowsPerBand = 4;

        static readonly ProfilerMarker s_sortMarker    = new ProfilerMarker("Peekaboo Raster Sort");
        static readonly ProfilerMarker s_coveredMarker = new ProfilerMarker("Peekaboo Raster Covered");
        static readonly ProfilerMarker s_weldMarker    = new ProfilerMarker("Peekaboo Raster Weld");

        /// <summary>
        /// How many vertices a plate can have after clipping. Every buffer that holds clipped output
        /// needs to be this big.
        /// </summary>
        /// <remarks>
        /// Clipping a convex polygon against a half space adds at most one vertex, and a plate gets
        /// clipped five times: the near plane, then the four screen edges. Baked plates have at most
        /// eight vertices, so they never get close. But a plate from PeekabooOccluderBuilder can have
        /// all sixteen.
        /// </remarks>
        public const int kMaxClippedVertices = kMaxPlateVertices + 5;

        /// <summary>
        /// Turns one world-space convex plate into a screen-space plate with a linear depth function.
        /// Returns false if it adds nothing to this view.
        /// </summary>
        /// <param name="windingSign">-1 if the occluder's transform is mirrored, which flips the winding</param>
        /// <param name="screenOut">Receives the clipped screen-space polygon, at least kMaxClippedVertices long</param>
        public static bool TryBuildScreenPlate(in PeekabooView view,
                                               float3* worldVertices,
                                               int count,
                                               float coneCosHalfAngle,
                                               bool doubleSided,
                                               float windingSign,
                                               float2* screenOut,
                                               out ScreenPlate plate)
        {
            plate = default;
            if (count < 3 || count > kMaxPlateVertices)
                return false;

            float3 centroid = 0f;
            for (int v = 0; v < count; v++)
                centroid += worldVertices[v];
            centroid /= count;

            // Newell's method gets the transformed polygon's plane without needing the inverse
            // transpose of the transform.
            float3 accumulated = 0f;
            for (int v = 0; v < count; v++)
                accumulated += math.cross(worldVertices[v] - centroid, worldVertices[(v + 1) % count] - centroid);
            float normalLength = math.length(accumulated);
            if (!(normalLength > 1e-20f))
                return false;
            var worldNormal   = accumulated * (windingSign / normalLength);
            var worldDistance = math.dot(worldNormal, centroid);

            var   viewerDirection = view.isPerspective ? math.normalizesafe(view.origin - centroid) : -view.axisForward;
            float facing          = math.dot(viewerDirection, worldNormal);
            if (doubleSided)
                facing = math.abs(facing);
            if (facing < coneCosHalfAngle)
                return false;

            if (!view.PlaneToLinearCloseness(worldNormal, worldDistance, out var gradient, out var constant))
                return false;

            var viewVertices = stackalloc float3[kMaxClippedVertices];
            var scratch      = stackalloc float3[kMaxClippedVertices];
            for (int v = 0; v < count; v++)
                viewVertices[v] = view.ToViewSpace(worldVertices[v]);

            int clipped = ClipAndProject(in view, viewVertices, count, screenOut, scratch);
            if (clipped < 3)
                return false;

            float minY = float.MaxValue;
            float maxY = float.MinValue;
            for (int v = 0; v < clipped; v++)
            {
                minY = math.min(minY, screenOut[v].y);
                maxY = math.max(maxY, screenOut[v].y);
            }
            int rowMin = math.max((int)math.floor(minY), 0);
            int rowMax = math.min((int)math.ceil(maxY) - 1, view.resolution.y - 1);
            if (rowMax < rowMin)
                return false;

            plate = new ScreenPlate
            {
                vertexCount = clipped,
                gradient    = gradient,
                constant    = constant,
                rowMin      = rowMin,
                rowMax      = rowMax,
            };
            return true;
        }

        /// <summary>
        /// Clips a view-space convex polygon against the near plane, then projects it to screen space
        /// and clips it to the screen. Returns the vertex count, or zero if nothing is left.
        /// </summary>
        public static int ClipAndProject(in PeekabooView view, float3* viewSpace, int count, float2* screenOut, float3* scratch)
        {
            if (view.isPerspective)
            {
                count = ClipToNearPlane(viewSpace, count, view.nearDistance, scratch);
                if (count < 3)
                    return 0;
                viewSpace = scratch;
            }

            for (int i = 0; i < count; i++)
            {
                if (!view.ProjectViewSpace(viewSpace[i], out var screen, out _))
                    return 0;
                screenOut[i] = screen;
            }

            // Parts of a plate outside the view can't occlude anything the view renders, so dropping
            // them is free and keeps coordinates in range.
            var temp = (float2*)scratch;
            count    = ClipToHalfPlane(screenOut, count, new float2(1f, 0f), 0f, temp);
            if (count < 3)
                return 0;
            count = ClipToHalfPlane(temp, count, new float2(-1f, 0f), view.resolution.x, screenOut);
            if (count < 3)
                return 0;
            count = ClipToHalfPlane(screenOut, count, new float2(0f, 1f), 0f, temp);
            if (count < 3)
                return 0;
            count = ClipToHalfPlane(temp, count, new float2(0f, -1f), view.resolution.y, screenOut);
            return count < 3 ? 0 : count;
        }

        /// <summary>
        /// The plate's footprint on the near plane, which is where the eye rays that can reach the
        /// plate cross the near plane. Returns the vertex count, or zero if none of the plate is past
        /// the near plane.
        /// </summary>
        /// <remarks>
        /// Validity regions get tested against this instead of the whole near rectangle. Rule 5 only
        /// talks about rays that hit the plate, so a point on the near rectangle that can't reach it
        /// doesn't matter. For orthographic views, this is what makes the rule usable at all. A
        /// cascade's near rectangle is its whole cross section, so a roof plate under a slanted sun
        /// would cross its own half space and get rejected.
        ///
        /// The footprint isn't clipped to the near rectangle. That can only make it bigger than rule 4
        /// needs, which costs culling, not correctness.
        ///
        /// Projecting the vertices is enough. The footprint of a convex polygon is the convex hull of
        /// its projected vertices, and a convex region that contains the vertices contains the hull.
        /// </remarks>
        public static int BuildNearPlaneFootprint(in PeekabooView view, float3* worldVertices, int count, float3* footprintOut, float3* scratch)
        {
            if (count < 3 || count > kMaxPlateVertices)
                return 0;

            for (int v = 0; v < count; v++)
                scratch[v] = view.ToViewSpace(worldVertices[v]);

            var clipped = scratch;
            if (view.isPerspective)
            {
                // Rays meet at the eye, so a vertex closer than the near plane never crosses it on the
                // way out, and could project through the eye to the wrong side. Clipping first keeps
                // the footprint made of real ray starting points.
                clipped = footprintOut;
                count   = ClipToNearPlane(scratch, count, view.nearDistance, clipped);
                if (count < 3)
                    return 0;
            }
            // Orthographic views don't get clipped, since their near distance isn't a real near plane.
            // It's just the front of the volume Peekaboo fitted to the culling planes. Unity places the
            // shadow camera so everything along the view axis renders. Clipping here would throw away
            // casters between the light and the cascade, which are the tall ones, and their rays cross
            // the plane at the same spot either way.

            for (int v = 0; v < count; v++)
            {
                var p = clipped[v];
                // Perspective rays meet at the origin, so the crossing is the vertex scaled back along
                // its ray. Orthographic rays are parallel, so it just moves along the view axis.
                var onPlane = view.isPerspective ? p * (view.nearDistance / p.z) : new float3(p.xy, view.nearDistance);
                footprintOut[v] = view.FromViewSpace(onPlane);
            }
            return count;
        }

        static int ClipToNearPlane(float3* input, int count, float near, float3* output)
        {
            int outCount = 0;
            for (int i = 0; i < count; i++)
            {
                var a  = input[i];
                var b  = input[(i + 1) % count];
                var da = a.z - near;
                var db = b.z - near;
                if (da >= 0f)
                    output[outCount++] = a;
                if ((da >= 0f) != (db >= 0f))
                {
                    var t              = da / (da - db);
                    output[outCount++] = a + t * (b - a);
                }
                if (outCount >= kMaxPlateVertices)
                    break;
            }
            return outCount;
        }

        // Keeps the half plane dot(normal, p) + distance >= 0.
        static int ClipToHalfPlane(float2* input, int count, float2 normal, float distance, float2* output)
        {
            int outCount = 0;
            for (int i = 0; i < count; i++)
            {
                var a  = input[i];
                var b  = input[(i + 1) % count];
                var da = math.dot(normal, a) + distance;
                var db = math.dot(normal, b) + distance;
                if (da >= 0f)
                    output[outCount++] = a;
                if ((da >= 0f) != (db >= 0f))
                {
                    var t              = da / (da - db);
                    output[outCount++] = a + t * (b - a);
                }
                if (outCount >= kMaxPlateVertices)
                    break;
            }
            return outCount;
        }

        /// <summary>
        /// Where the polygon's left and right edges cross the top and bottom of the pixel row. A
        /// convex polygon that reaches both covers wherever both crossings agree.
        /// </summary>
        public static bool RowSpan(float2* verts, int count, int row, out PlateSpan span)
        {
            span = default;
            if (!HorizontalExtent(verts, count, row, out var topLeft, out var topRight))
                return false;
            if (!HorizontalExtent(verts, count, row + 1, out var bottomLeft, out var bottomRight))
                return false;
            span.leftAtTop     = topLeft;
            span.rightAtTop    = topRight;
            span.leftAtBottom  = bottomLeft;
            span.rightAtBottom = bottomRight;
            return true;
        }

        static bool HorizontalExtent(float2* verts, int count, float y, out float xMin, out float xMax)
        {
            xMin       = float.MaxValue;
            xMax       = float.MinValue;
            bool found = false;
            for (int i = 0; i < count; i++)
            {
                var a = verts[i];
                var b = verts[(i + 1) % count];
                if (math.min(a.y, b.y) > y || math.max(a.y, b.y) < y)
                    continue;
                found = true;
                if (a.y == b.y)
                {
                    xMin = math.min(xMin, math.min(a.x, b.x));
                    xMax = math.max(xMax, math.max(a.x, b.x));
                }
                else
                {
                    var t = (y - a.y) / (b.y - a.y);
                    var x = a.x + t * (b.x - a.x);
                    xMin  = math.min(xMin, x);
                    xMax  = math.max(xMax, x);
                }
            }
            return found && xMin <= xMax;
        }

        /// <summary>
        /// The closeness a plate can store at each pixel of this row, as a straight line in x.
        /// </summary>
        /// <remarks>
        /// Closeness over a plate is linear in screen space. The safe value for a pixel is its minimum
        /// over the pixel, which is at a corner. The gradient's sign picks the same corner for every
        /// pixel, so the whole row is one line, and writing a run of pixels is one add per pixel with
        /// no branches.
        /// </remarks>
        static float RowRampBase(in PlateSpan span, int row)
        {
            return span.constant
                   + span.gradient.y * (span.gradient.y < 0f ? row + 1 : row)
                   + (span.gradient.x < 0f ? span.gradient.x : 0f);
        }

        /// <summary>
        /// Writes one row's spans into the depth buffer row. Spans are sorted in place.
        /// </summary>
        /// <remarks>
        /// This takes two passes, because the two cases cost very different amounts. Almost every
        /// covered pixel is covered by a single plate, and those get written as one run per plate with
        /// no per-pixel bookkeeping. Only pixels that no single plate covers need welding, and there
        /// are just a few per seam. The second pass finds them by looking for what the first left
        /// empty.
        /// </remarks>
        [Unity.Burst.CompilerServices.SkipLocalsInit]
        public static void SweepRow(PlateSpan* spans, int spanCount, int row, int width, float* bufferRow)
        {
            if (spanCount <= 0)
                return;
            using (PeekabooProfiling.Auto(in s_sortMarker))
                SortSpansByStart(spans, spanCount);

            // Two plates from the same mesh share an edge exactly, but each computes it separately, so
            // their crossings differ in the last few bits. Without some slack, every shared edge would
            // look like a gap. The slack is a thousandth of a pixel, far smaller than any real render
            // target's sub-pixel precision.
            const float kSeamTolerance = 1e-3f;

            // Pass one: what each plate covers by itself, which is almost everything.
            using (PeekabooProfiling.Auto(in s_coveredMarker))
            {
                for (int i = 0; i < spanCount; i++)
                {
                    int first = math.max((int)math.ceil(spans[i].innerMin), 0);
                    int last  = math.min((int)math.floor(spans[i].innerMax) - 1, width - 1);
                    if (last < first)
                        continue;
                    float rampBase     = RowRampBase(in spans[i], row);
                    float rampGradient = spans[i].gradient.x;
                    for (int x = first; x <= last; x++)
                        bufferRow[x] = math.max(bufferRow[x], rampBase + rampGradient * x);
                }
            }

            // Pass two: the seams. A run is a chain of plates where each meets the one before it across
            // the whole row. So together they cover everything from the first plate's span to the
            // farthest any of them reaches.
            using var weldMarker = PeekabooProfiling.Auto(in s_weldMarker);
            var welded    = stackalloc float[width];
            var pending   = stackalloc ulong[(width + 63) >> 6];
            var runs      = stackalloc int2[spanCount];
            var runBounds = stackalloc float2[spanCount];
            int runCount  = 0;
            {
                float2 current  = new float2(spans[0].innerMin, spans[0].innerMax);
                int    firstIdx = 0;
                for (int i = 1; i < spanCount; i++)
                {
                    if (spans[i].ConnectsAfter(in spans[i - 1], kSeamTolerance))
                    {
                        current.y = math.max(current.y, spans[i].innerMax);
                        continue;
                    }
                    runs[runCount]        = new int2(firstIdx, i - 1);
                    runBounds[runCount++] = current;
                    firstIdx              = i;
                    current               = new float2(spans[i].innerMin, spans[i].innerMax);
                }
                runs[runCount]        = new int2(firstIdx, spanCount - 1);
                runBounds[runCount++] = current;
            }

            for (int r = 0; r < runCount; r++)
            {
                if (runs[r].x == runs[r].y)
                    continue;  // A run of one plate has no seam in it.
                int first = math.max((int)math.ceil(runBounds[r].x), 0);
                int last  = math.min((int)math.floor(runBounds[r].y) - 1, width - 1);
                if (last < first)
                    continue;

                // A bit per pixel marks which ones some edge reached, so the run's interior is never
                // touched.
                int firstWord = first >> 6;
                int lastWord  = last >> 6;
                for (int w = firstWord; w <= lastWord; w++)
                    pending[w] = 0ul;

                // A pixel still empty after pass one isn't fully covered by any plate, so every plate
                // touching it only touches it with an edge. So each plate only needs visiting at its
                // edge pixels, not the other way around.
                for (int i = runs[r].x; i <= runs[r].y; i++)
                {
                    int outerFirst = (int)math.floor(spans[i].outerMin);
                    int outerLast  = (int)math.ceil(spans[i].outerMax) - 1;
                    int innerFirst = (int)math.ceil(spans[i].innerMin);
                    int innerLast  = (int)math.floor(spans[i].innerMax) - 1;
                    if (innerLast < innerFirst)
                    {
                        innerFirst = outerLast + 1;
                        innerLast  = outerLast;
                    }
                    float rampBase     = RowRampBase(in spans[i], row);
                    float rampGradient = spans[i].gradient.x;
                    for (int x = math.max(outerFirst, first); x <= math.min(innerFirst - 1, last); x++)
                    {
                        if (bufferRow[x] == PeekabooDepthBuffer.kEmpty)
                            Weld(welded, pending, x, rampBase + rampGradient * x);
                    }
                    for (int x = math.max(innerLast + 1, first); x <= math.min(outerLast, last); x++)
                    {
                        if (bufferRow[x] == PeekabooDepthBuffer.kEmpty)
                            Weld(welded, pending, x, rampBase + rampGradient * x);
                    }
                }

                // Only pixels pass one left empty were marked, since anything it wrote is covered by a
                // single plate, and that's always at least as good as the welded value.
                for (int w = firstWord; w <= lastWord; w++)
                {
                    ulong bits = pending[w];
                    while (bits != 0)
                    {
                        int x  = (w << 6) + math.tzcnt(bits);
                        bits  &= bits - 1;
                        if (!float.IsPositiveInfinity(welded[x]))
                            bufferRow[x] = welded[x];
                    }
                }
            }
        }

        static void Weld(float* welded, ulong* pending, int x, float value)
        {
            ulong bit = 1ul << (x & 63);
            if ((pending[x >> 6] & bit) == 0)
            {
                pending[x >> 6] |= bit;
                welded[x]        = math.min(float.PositiveInfinity, value);
            }
            else
                welded[x] = math.min(welded[x], value);
        }

        // Sorting by where each plate first touches the row keeps plates starting in order, and puts
        // plates that share a seam next to each other so the run walk can join them. A row near the
        // horizon can hold hundreds of spans, so this sorts small keys and then moves each span once.
        //
        // Each key is the start's ordered bits above the span's index. The index breaks ties, so the
        // order matches a stable sort of the spans.
        [Unity.Burst.CompilerServices.SkipLocalsInit]
        static void SortSpansByStart(PlateSpan* spans, int count)
        {
            var  keys   = stackalloc ulong[count];
            bool sorted = true;
            for (int i = 0; i < count; i++)
            {
                keys[i]  = ((ulong)OrderedBits(spans[i].outerMin) << 32) | (uint)i;
                sorted  &= i == 0 || keys[i] > keys[i - 1];
            }
            if (sorted)
                return;
            if (count >= kRadixSortThreshold)
                RadixSortKeys(keys, count);
            else
                NativeSortExtension.Sort(keys, count);

            var ordered = stackalloc PlateSpan[count];
            for (int i = 0; i < count; i++)
                ordered[i] = spans[(int)(uint)keys[i]];
            UnsafeUtility.MemCpy(spans, ordered, count * UnsafeUtility.SizeOf<PlateSpan>());
        }

        /// <summary>
        /// A float's bits, flipped so they order as unsigned integers the way the floats do. Both
        /// zeros map to the same value, since they compare equal.
        /// </summary>
        public static uint OrderedBits(float value)
        {
            uint bits = math.asuint(value);
            return value == 0f ? 0x80000000u : (bits & 0x80000000u) != 0 ? ~bits : bits | 0x80000000u;
        }

        /// <summary>
        /// Rows with at least this many spans sort them by radix instead of by comparison.
        /// </summary>
        public const int kRadixSortThreshold = 40;

        /// <summary>
        /// Sorts span keys by their upper 32 bits, keeping keys with equal upper halves in their
        /// current order.
        /// </summary>
        /// <remarks>
        /// The lower half is the span's index, and the keys start in index order. So a stable sort on
        /// the upper half gives exactly what a comparison sort of the whole key does. A pass whose
        /// digit is the same for every key would leave the order alone, so it gets skipped.
        /// </remarks>
        [Unity.Burst.CompilerServices.SkipLocalsInit]
        public static void RadixSortKeys(ulong* keys, int count)
        {
            const int kBits    = 8;
            const int kBuckets = 1 << kBits;
            const int kPasses  = 32 / kBits;
            const int kMask    = kBuckets - 1;

            var counts = stackalloc int[kBuckets * kPasses];
            UnsafeUtility.MemClear(counts, kBuckets * kPasses * sizeof(int));
            for (int i = 0; i < count; i++)
            {
                uint key = (uint)(keys[i] >> 32);
                for (int pass = 0; pass < kPasses; pass++)
                    counts[pass * kBuckets + (int)((key >> (pass * kBits)) & kMask)]++;
            }

            var    scratch = stackalloc ulong[count];
            ulong* source  = keys;
            ulong* target  = scratch;
            for (int pass = 0; pass < kPasses; pass++)
            {
                int shift  = 32 + pass * kBits;
                var bucket = counts + pass * kBuckets;
                if (bucket[(int)((source[0] >> shift) & kMask)] == count)
                    continue;

                int offset = 0;
                for (int b = 0; b < kBuckets; b++)
                {
                    int n      = bucket[b];
                    bucket[b]  = offset;
                    offset    += n;
                }
                for (int i = 0; i < count; i++)
                {
                    ulong key   = source[i];
                    int   digit = (int)((key >> shift) & kMask);
                    target[bucket[digit]++] = key;
                }
                var swap = source;
                source   = target;
                target   = swap;
            }
            if (source != keys)
                UnsafeUtility.MemCpy(keys, source, count * sizeof(ulong));
        }

        /// <summary>
        /// Where a polygon's left and right edges cross each of <paramref name="rowCount"/> rows, at
        /// every row boundary from the first row's top to the last row's bottom. Bit <c>k</c> of the
        /// returned mask is set when the polygon reaches boundary <c>k</c>.
        /// </summary>
        /// <remarks>
        /// Each row's bottom boundary is the next row's top, so n rows only need n + 1 crossings, and
        /// one walk of the polygon's edges finds them all. This uses the same math as
        /// <see cref="RowSpan"/>, so the results match it exactly.
        /// </remarks>
        public static uint BandExtents(float2* verts, int count, int firstRow, int rowCount, float2* extents)
        {
            RowCrossings(verts, count, firstRow, rowCount, extents);
            uint valid = 0;
            for (int k = 0; k <= rowCount; k++)
            {
                if (extents[k].x <= extents[k].y)
                    valid |= 1u << k;
            }
            return valid;
        }

        /// <summary>
        /// Where a polygon's left and right edges cross each of <paramref name="rowCount"/> rows, at
        /// every row boundary from the first row's top to the last row's bottom. A boundary the polygon
        /// doesn't reach is left as (MaxValue, MinValue), so its x is never at most its y.
        /// </summary>
        public static void RowCrossings(float2* verts, int count, int firstRow, int rowCount, float2* extents)
        {
            for (int k = 0; k <= rowCount; k++)
                extents[k] = new float2(float.MaxValue, float.MinValue);

            var a = verts[count - 1];
            for (int i = 0; i < count; i++)
            {
                var   b    = verts[i];
                float lowY  = math.min(a.y, b.y);
                float highY = math.max(a.y, b.y);
                int   kMin  = math.max((int)math.ceil(lowY) - firstRow, 0);
                int   kMax  = math.min((int)math.floor(highY) - firstRow, rowCount);
                for (int k = kMin; k <= kMax; k++)
                {
                    float y = firstRow + k;
                    if (a.y == b.y)
                    {
                        extents[k].x = math.min(extents[k].x, math.min(a.x, b.x));
                        extents[k].y = math.max(extents[k].y, math.max(a.x, b.x));
                    }
                    else
                    {
                        var t        = (y - a.y) / (b.y - a.y);
                        var x        = a.x + t * (b.x - a.x);
                        extents[k].x = math.min(extents[k].x, x);
                        extents[k].y = math.max(extents[k].y, x);
                    }
                }
                a = b;
            }
        }
    }
}
