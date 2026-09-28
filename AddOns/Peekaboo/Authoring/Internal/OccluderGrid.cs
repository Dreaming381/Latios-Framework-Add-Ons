using Unity.Collections;
using Unity.Mathematics;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Finds shapes on a 2D grid of cells that are known to be safe to cover.
    /// </summary>
    /// <remarks>
    /// It finds two shapes and keeps whichever is bigger, since regions come in two kinds. A slice
    /// through a wall or a box is rectangular, so a rectangle covers all of it and a polygon in the
    /// largest circle only covers part. A slice through anything round is a disc, which is the other
    /// way around.
    /// </remarks>
    internal static unsafe class OccluderGrid
    {
        /// <summary>
        /// The largest axis-aligned rectangle of set cells, as inclusive cell bounds.
        /// </summary>
        public static bool LargestRectangle(NativeArray<bool> mask, int width, int height, out int4 rect)
        {
            rect         = default;
            int bestArea = 0;
            var heights  = new NativeArray<int>(width, Allocator.Temp, NativeArrayOptions.ClearMemory);
            var stackX   = new NativeArray<int>(width + 1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var stackH   = new NativeArray<int>(width + 1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (int row = 0; row < height; row++)
            {
                for (int x = 0; x < width; x++)
                    heights[x] = mask[row * width + x] ? heights[x] + 1 : 0;

                int top = 0;
                for (int x = 0; x <= width; x++)
                {
                    int columnHeight = x < width ? heights[x] : 0;
                    int start        = x;
                    while (top > 0 && stackH[top - 1] >= columnHeight)
                    {
                        top--;
                        int candidateHeight = stackH[top];
                        int candidateStart  = stackX[top];
                        int area            = candidateHeight * (x - candidateStart);
                        if (area > bestArea && candidateHeight > 0)
                        {
                            bestArea = area;
                            rect     = new int4(candidateStart, row - candidateHeight + 1, x - 1, row);
                        }
                        start = candidateStart;
                    }
                    stackX[top] = start;
                    stackH[top] = columnHeight;
                    top++;
                }
            }
            heights.Dispose();
            stackX.Dispose();
            stackH.Dispose();
            return bestArea > 0;
        }

        /// <summary>
        /// Grows a rectangle outward as far as the mask allows. That's how plates get to overlap: after
        /// a rectangle claims its region, it grows back over its neighbors.
        /// </summary>
        public static void GrowRectangle(NativeArray<bool> mask, int width, int height, ref int4 rect)
        {
            bool grew = true;
            while (grew)
            {
                grew = false;
                if (rect.x > 0 && ColumnIsSet(mask, width, rect.x - 1, rect.y, rect.w))
                {
                    rect.x--;
                    grew = true;
                }
                if (rect.z < width - 1 && ColumnIsSet(mask, width, rect.z + 1, rect.y, rect.w))
                {
                    rect.z++;
                    grew = true;
                }
                if (rect.y > 0 && RowIsSet(mask, width, rect.y - 1, rect.x, rect.z))
                {
                    rect.y--;
                    grew = true;
                }
                if (rect.w < height - 1 && RowIsSet(mask, width, rect.w + 1, rect.x, rect.z))
                {
                    rect.w++;
                    grew = true;
                }
            }
        }

        public static bool ColumnIsSet(NativeArray<bool> mask, int width, int x, int y0, int y1)
        {
            for (int y = y0; y <= y1; y++)
            {
                if (!mask[y * width + x])
                    return false;
            }
            return true;
        }

        public static bool RowIsSet(NativeArray<bool> mask, int width, int y, int x0, int x1)
        {
            for (int x = x0; x <= x1; x++)
            {
                if (!mask[y * width + x])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// The largest circle of set cells, as a center in cell coordinates and a radius in cells.
        /// The radius is exact, not a chamfer approximation, since the plate's size comes from it and
        /// an overestimate would put the plate outside the region.
        /// </summary>
        public static bool LargestInscribedCircle(NativeArray<bool> mask, int width, int height, out float2 center, out float radius)
        {
            center = default;
            radius = 0f;

            var squared = new NativeArray<float>(width * height, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            SquaredDistanceToUnset(mask, width, height, squared);

            float best = 0f;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = squared[y * width + x];
                    if (value > best)
                    {
                        best   = value;
                        center = new float2(x + 0.5f, y + 0.5f);
                    }
                }
            }
            squared.Dispose();
            radius = math.sqrt(best);
            return radius > 0f;
        }

        /// <summary>
        /// For each set cell, the squared distance from its center to the nearest unset cell's center.
        /// Cells outside the grid count as unset, so a region touching the edge is bounded there too.
        /// </summary>
        /// <remarks>
        /// This is Felzenszwalb's separable transform. It gets the exact answer in two linear passes by
        /// treating each column's distances as parabolas and taking their lower envelope.
        /// </remarks>
        public static void SquaredDistanceToUnset(NativeArray<bool> mask, int width, int height, NativeArray<float> squared)
        {
            const float kFar = 1e10f;
            for (int i = 0; i < width * height; i++)
                squared[i] = mask[i] ? kFar : 0f;

            int maxSpan = math.max(width, height);
            var source  = new NativeArray<float>(maxSpan, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var result  = new NativeArray<float>(maxSpan, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var hull    = new NativeArray<int>(maxSpan, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var breaks  = new NativeArray<float>(maxSpan + 1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    source[y] = squared[y * width + x];
                LowerEnvelope(source, result, hull, breaks, height);
                for (int y = 0; y < height; y++)
                    squared[y * width + x] = result[y];
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    source[x] = squared[y * width + x];
                LowerEnvelope(source, result, hull, breaks, width);
                for (int x = 0; x < width; x++)
                    squared[y * width + x] = result[x];
            }

            // Nothing bounds a region at the grid's edge, so cap the distance by how far the cell is
            // from the edge.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float edge                = math.min(math.min(x + 0.5f, width - 0.5f - x), math.min(y + 0.5f, height - 0.5f - y));
                    int   index               = y * width + x;
                    squared[index]            = math.min(squared[index], edge * edge);
                }
            }

            source.Dispose();
            result.Dispose();
            hull.Dispose();
            breaks.Dispose();
        }

        static void LowerEnvelope(NativeArray<float> source, NativeArray<float> result, NativeArray<int> hull, NativeArray<float> breaks, int count)
        {
            int top   = 0;
            hull[0]   = 0;
            breaks[0] = -1e20f;
            breaks[1] = 1e20f;
            for (int q = 1; q < count; q++)
            {
                int   p            = hull[top];
                float intersection = ((source[q] + q * q) - (source[p] + p * p)) / (2f * q - 2f * p);
                // The first break is effectively negative infinity, so this always stops at the bottom
                // of the hull.
                while (intersection <= breaks[top])
                {
                    top--;
                    p            = hull[top];
                    intersection = ((source[q] + q * q) - (source[p] + p * p)) / (2f * q - 2f * p);
                }
                top++;
                hull[top]       = q;
                breaks[top]     = intersection;
                breaks[top + 1] = 1e20f;
            }

            int cursor = 0;
            for (int q = 0; q < count; q++)
            {
                while (breaks[cursor + 1] < q)
                    cursor++;
                int p     = hull[cursor];
                result[q] = (q - p) * (q - p) + source[p];
            }
        }
    }
}
