using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Cuts plates out of the inside of a closed mesh, instead of off its surface.
    /// </summary>
    /// <remarks>
    /// A plate only has to stay inside the solid. Nothing says it has to touch the surface, and for
    /// anything round, that's what makes it work. A sphere has no flat face to take a plate from, but
    /// a disc through its middle is completely inside it and covers its whole silhouette.
    ///
    /// The mesh gets voxelized, and a voxel only counts as interior if its center is inside and no
    /// triangle passes through it. That makes the whole voxel cube interior, so any polygon covered
    /// by interior voxels is inside the solid. Then slices get taken along 13 axes at several offsets.
    /// Each plate's cone is wide enough that the 13 axes cover every direction.
    /// </remarks>
    internal static unsafe class OccluderInteriorPlates
    {
        public const  int   kAxisCount          = 13;
        private const int   kMaxVoxels          = 48;
        private const int   kMaxSliceCells      = 96;
        private const int   kPolygonVertices    = 8;
        private const float kConeCosHalfAngle   = 0.85f;  // 32 degrees, against a worst gap of 22.5.
        private const float kDedupeCosHalfAngle = 0.1f;

        /// <summary>
        /// Where along each axis a slice is taken, as fractions of how far the mesh reaches that way.
        /// Zero is through the middle.
        /// </summary>
        static readonly float[] kSliceOffsets = { -0.55f, -0.275f, 0f, 0.275f, 0.55f };

        /// <summary>
        /// The 13 directions plates are sliced along: a cube's 3 face normals, 4 body diagonals, and 6
        /// edge directions. As undirected lines, no view direction is more than 22.5 degrees from one
        /// of them, so a plate never gets foreshortened below 92% of its area.
        /// </summary>
        public static float3 Axis(int index)
        {
            switch (index)
            {
                case 0:  return new float3(1f, 0f, 0f);
                case 1:  return new float3(0f, 1f, 0f);
                case 2:  return new float3(0f, 0f, 1f);
                case 3:  return math.normalize(new float3(1f, 1f, 1f));
                case 4:  return math.normalize(new float3(-1f, 1f, 1f));
                case 5:  return math.normalize(new float3(1f, -1f, 1f));
                case 6:  return math.normalize(new float3(1f, 1f, -1f));
                case 7:  return math.normalize(new float3(1f, 1f, 0f));
                case 8:  return math.normalize(new float3(1f, -1f, 0f));
                case 9:  return math.normalize(new float3(1f, 0f, 1f));
                case 10: return math.normalize(new float3(1f, 0f, -1f));
                case 11: return math.normalize(new float3(0f, 1f, 1f));
                default: return math.normalize(new float3(0f, 1f, -1f));
            }
        }

        public static void Build(NativeArray<float3> points,
                                 NativeArray<int3>   triangles,
                                 float3 aabbMin,
                                 float3 aabbMax,
                                 float minArea,
                                 int existingPlateStart,
                                 ref UnsafeList<OccluderPlate> outPlates,
                                 ref UnsafeList<float3>        outVertices)
        {
            var extent = aabbMax - aabbMin;
            if (math.cmax(extent) <= 0f)
                return;

            float voxelSize = math.cmax(extent) / (kMaxVoxels - 4);
            var   resolution = math.clamp((int3)math.ceil(extent / voxelSize) + 2, 4, kMaxVoxels);
            var   gridOrigin = (aabbMin + aabbMax) * 0.5f - (float3)resolution * voxelSize * 0.5f;

            var interior = new NativeArray<bool>(resolution.x * resolution.y * resolution.z, Allocator.Temp, NativeArrayOptions.ClearMemory);
            if (!VoxelizeAllAxes(points, triangles, gridOrigin, voxelSize, resolution, interior))
            {
                interior.Dispose();
                return;
            }

            float3 centroid = 0f;
            int    filled   = 0;
            for (int z = 0; z < resolution.z; z++)
            {
                for (int y = 0; y < resolution.y; y++)
                {
                    for (int x = 0; x < resolution.x; x++)
                    {
                        if (!interior[(z * resolution.y + y) * resolution.x + x])
                            continue;
                        centroid += gridOrigin + (new float3(x, y, z) + 0.5f) * voxelSize;
                        filled++;
                    }
                }
            }
            if (filled == 0)
            {
                interior.Dispose();
                return;
            }
            centroid /= filled;

            int surfacePlateEnd = outPlates.Length;
            var halfExtent      = (aabbMax - aabbMin) * 0.5f;
            for (int axis = 0; axis < kAxisCount; axis++)
            {
                var direction = Axis(axis);
                // How far the mesh reaches along this axis, which is how far apart slices can go.
                float reach = math.dot(math.abs(direction), halfExtent);

                // Slices are spread across several offsets, not just the middle. The middle works for
                // a solid that surrounds it, but not for one that doesn't. A ring's centroid is in its
                // hole, so a slice through it catches the tube edge-on and finds very little. An
                // offset slice cuts the tube straight across.
                for (int step = 0; step < kSliceOffsets.Length; step++)
                {
                    BuildSlice(direction, centroid + direction * (reach * kSliceOffsets[step]), aabbMin, aabbMax, gridOrigin, voxelSize,
                               resolution, interior, minArea, existingPlateStart, surfacePlateEnd, ref outPlates, ref outVertices);
                }
            }

            interior.Dispose();
        }

        /// <summary>
        /// The voxels that all three independent sweeps agree are inside the solid.
        /// </summary>
        /// <remarks>
        /// One sweep isn't enough. It sums winding along columns of voxel centers, and those columns
        /// can line up badly with the mesh. Take a ring standing on the Y axis. Its whole equator edge
        /// loop lies in one plane, and since the grid is centered on the bounds, a row of voxel centers
        /// lands exactly in that plane. Every column in that row grazes a ring of edges at once. Their
        /// crossings don't cancel, the winding never closes, and the ring's hole gets marked solid. A
        /// plate cut from that would hide things standing in open air.
        ///
        /// Sweeping along each axis and keeping only what all three agree on fixes it. The problem
        /// depends on the sweep direction, so a plane that fools one sweep gets crossed cleanly by the
        /// other two. Intersecting can only shrink the interior, which is the safe way to be wrong.
        /// A smaller interior means a smaller plate, never one reaching somewhere it shouldn't.
        /// </remarks>
        static bool VoxelizeAllAxes(NativeArray<float3> points, NativeArray<int3> triangles, float3 gridOrigin, float voxelSize, int3 resolution,
                                    NativeArray<bool> interior)
        {
            var permuted = new NativeArray<float3>(points.Length, Allocator.Temp);
            var sweep    = new NativeArray<bool>(interior.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
            bool ok      = true;

            for (int axis = 0; axis < 3 && ok; axis++)
            {
                for (int i = 0; i < points.Length; i++)
                    permuted[i] = Permute(points[i], axis);
                var sweepOrigin     = Permute(gridOrigin, axis);
                var sweepResolution = (int3)Permute(resolution, axis);

                if (axis > 0)
                {
                    for (int i = 0; i < sweep.Length; i++)
                        sweep[i] = false;
                }
                ok = Voxelize(permuted, triangles, sweepOrigin, voxelSize, sweepResolution, sweep);
                if (!ok)
                    break;

                for (int z = 0; z < resolution.z; z++)
                {
                    for (int y = 0; y < resolution.y; y++)
                    {
                        for (int x = 0; x < resolution.x; x++)
                        {
                            var cell  = (int3)Permute(new float3(x, y, z), axis);
                            bool set  = sweep[(cell.z * sweepResolution.y + cell.y) * sweepResolution.x + cell.x];
                            int  flat = (z * resolution.y + y) * resolution.x + x;
                            interior[flat] = axis == 0 ? set : (interior[flat] && set);
                        }
                    }
                }
            }

            permuted.Dispose();
            sweep.Dispose();
            return ok;
        }

        /// <summary>
        /// Rotates the components so the sweep axis becomes z, which is the axis
        /// <see cref="Voxelize"/> sums winding along.
        /// </summary>
        static float3 Permute(float3 value, int axis)
        {
            switch (axis)
            {
                case 1:  return new float3(value.y, value.z, value.x);
                case 2:  return new float3(value.z, value.x, value.y);
                default: return value;
            }
        }

        /// <summary>
        /// Marks the voxels that are definitely inside the solid: the center is inside, and no triangle
        /// touches the voxel at all.
        /// </summary>
        static bool Voxelize(NativeArray<float3> points, NativeArray<int3> triangles, float3 gridOrigin, float voxelSize, int3 resolution, NativeArray<bool> interior)
        {
            int  voxelCount = resolution.x * resolution.y * resolution.z;
            var  blocked    = new NativeArray<bool>(voxelCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
            var  crossings  = new NativeList<Crossing>(triangles.Length * 2, Allocator.Temp);
            bool ok         = true;

            for (int t = 0; t < triangles.Length; t++)
            {
                var tri = triangles[t];
                var a   = points[tri.x];
                var b   = points[tri.y];
                var c   = points[tri.z];

                MarkTriangle(a, b, c, gridOrigin, voxelSize, resolution, blocked);
                CollectColumnCrossings(a, b, c, gridOrigin, voxelSize, resolution, crossings);
                if (crossings.Length > triangles.Length * 8 + 4096)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                crossings.Sort(new CrossingComparer());
                FillInside(crossings, gridOrigin, voxelSize, resolution, blocked, interior);
            }

            blocked.Dispose();
            crossings.Dispose();
            return ok;
        }

        struct Crossing
        {
            public int   column;
            public float z;
            public float winding;
        }

        struct CrossingComparer : System.Collections.Generic.IComparer<Crossing>
        {
            public int Compare(Crossing a, Crossing b) => a.column != b.column ? a.column.CompareTo(b.column) : a.z.CompareTo(b.z);
        }

        static void MarkTriangle(float3 a, float3 b, float3 c, float3 gridOrigin, float voxelSize, int3 resolution, NativeArray<bool> blocked)
        {
            var low  = math.min(a, math.min(b, c));
            var high = math.max(a, math.max(b, c));
            var lowVoxel  = math.clamp((int3)math.floor((low - gridOrigin) / voxelSize), 0, resolution - 1);
            var highVoxel = math.clamp((int3)math.floor((high - gridOrigin) / voxelSize), 0, resolution - 1);

            var halfSize = (float3)(voxelSize * 0.5f);
            for (int z = lowVoxel.z; z <= highVoxel.z; z++)
            {
                for (int y = lowVoxel.y; y <= highVoxel.y; y++)
                {
                    for (int x = lowVoxel.x; x <= highVoxel.x; x++)
                    {
                        var center = gridOrigin + (new float3(x, y, z) + 0.5f) * voxelSize;
                        if (TriangleOverlapsBox(a - center, b - center, c - center, halfSize))
                            blocked[(z * resolution.y + y) * resolution.x + x] = true;
                    }
                }
            }
        }

        /// <summary>
        /// The separating axis test for a triangle against an axis-aligned box centered on the origin.
        /// </summary>
        static bool TriangleOverlapsBox(float3 a, float3 b, float3 c, float3 halfSize)
        {
            if (math.any(math.min(a, math.min(b, c)) > halfSize) || math.any(math.max(a, math.max(b, c)) < -halfSize))
                return false;

            var normal = math.cross(b - a, c - a);
            if (math.abs(math.dot(normal, a)) > math.dot(math.abs(normal), halfSize))
                return false;

            var edges = stackalloc float3[3] { b - a, c - b, a - c };
            var verts = stackalloc float3[3] { a, b, c };
            for (int e = 0; e < 3; e++)
            {
                for (int axis = 0; axis < 3; axis++)
                {
                    var unit = new float3(axis == 0 ? 1f : 0f, axis == 1 ? 1f : 0f, axis == 2 ? 1f : 0f);
                    var test = math.cross(unit, edges[e]);
                    if (math.lengthsq(test) < 1e-18f)
                        continue;
                    float p0    = math.dot(test, verts[0]);
                    float p1    = math.dot(test, verts[1]);
                    float p2    = math.dot(test, verts[2]);
                    float reach = math.dot(math.abs(test), halfSize);
                    if (math.min(p0, math.min(p1, p2)) > reach || math.max(p0, math.max(p1, p2)) < -reach)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Records where the triangle crosses each column of voxel centers, with the sign of its
        /// facing. Summing those signs along a column gives the winding number, which says inside or
        /// outside no matter how many shells the mesh has.
        /// </summary>
        static void CollectColumnCrossings(float3 a, float3 b, float3 c, float3 gridOrigin, float voxelSize, int3 resolution, NativeList<Crossing> crossings)
        {
            var normal = math.cross(b - a, c - a);
            if (math.abs(normal.z) < 1e-20f)
                return;
            float winding = normal.z > 0f ? 1f : -1f;

            var low  = math.min(a.xy, math.min(b.xy, c.xy));
            var high = math.max(a.xy, math.max(b.xy, c.xy));
            var lowCell  = math.clamp((int2)math.floor((low - gridOrigin.xy) / voxelSize), 0, resolution.xy - 1);
            var highCell = math.clamp((int2)math.floor((high - gridOrigin.xy) / voxelSize), 0, resolution.xy - 1);

            for (int y = lowCell.y; y <= highCell.y; y++)
            {
                for (int x = lowCell.x; x <= highCell.x; x++)
                {
                    var point = gridOrigin.xy + (new float2(x, y) + 0.5f) * voxelSize;
                    if (!PointInTriangle2D(point, a.xy, b.xy, c.xy))
                        continue;
                    float z = a.z + (-math.dot(normal.xy, point - a.xy)) / normal.z;
                    crossings.Add(new Crossing { column = y * resolution.x + x, z = z, winding = winding });
                }
            }
        }

        static bool PointInTriangle2D(float2 p, float2 a, float2 b, float2 c)
        {
            float d0 = Cross(b - a, p - a);
            float d1 = Cross(c - b, p - b);
            float d2 = Cross(a - c, p - c);
            bool  negative = d0 < 0f || d1 < 0f || d2 < 0f;
            bool  positive = d0 > 0f || d1 > 0f || d2 > 0f;
            return !(negative && positive);
        }

        static float Cross(float2 a, float2 b) => a.x * b.y - a.y * b.x;

        static void FillInside(NativeList<Crossing> crossings, float3 gridOrigin, float voxelSize, int3 resolution, NativeArray<bool> blocked, NativeArray<bool> interior)
        {
            int index = 0;
            while (index < crossings.Length)
            {
                int column = crossings[index].column;
                int end    = index;
                while (end < crossings.Length && crossings[end].column == column)
                    end++;

                float winding = 0f;
                for (int i = index; i < end; i++)
                {
                    float previousWinding = winding;
                    winding              += crossings[i].winding;
                    if (previousWinding != 0f || winding == 0f)
                        continue;

                    // A span opens here and closes at the next crossing that brings the winding back
                    // to zero. Everything in between is inside the solid.
                    float spanStart  = crossings[i].z;
                    float spanEnd    = spanStart;
                    float inner      = winding;
                    int   j          = i + 1;
                    for (; j < end; j++)
                    {
                        inner += crossings[j].winding;
                        if (inner == 0f)
                        {
                            spanEnd = crossings[j].z;
                            break;
                        }
                    }
                    if (j >= end)
                        break;

                    int z0 = (int)math.ceil((spanStart - gridOrigin.z) / voxelSize - 0.5f);
                    int z1 = (int)math.floor((spanEnd - gridOrigin.z) / voxelSize - 0.5f);
                    z0     = math.max(z0, 0);
                    z1     = math.min(z1, resolution.z - 1);
                    for (int z = z0; z <= z1; z++)
                    {
                        int voxel = z * resolution.x * resolution.y + column;
                        if (!blocked[voxel])
                            interior[voxel] = true;
                    }

                    winding = 0f;
                    i       = j;
                }
                index = end;
            }
        }

        static void BuildSlice(float3 axis,
                               float3 centroid,
                               float3 aabbMin,
                               float3 aabbMax,
                               float3 gridOrigin,
                               float voxelSize,
                               int3 resolution,
                               NativeArray<bool> interior,
                               float minArea,
                               int surfacePlateStart,
                               int surfacePlateEnd,
                               ref UnsafeList<OccluderPlate> outPlates,
                               ref UnsafeList<float3>        outVertices)
        {
            var seed      = math.abs(axis.x) < 0.9f ? new float3(1f, 0f, 0f) : new float3(0f, 1f, 0f);
            var tangentU  = math.normalize(math.cross(axis, seed));
            var tangentV  = math.cross(axis, tangentU);

            // A cell gets tested against every voxel its bounds touch, so no cell size can skip a
            // voxel. Half a voxel just keeps the plate from being too coarse.
            float cellSize = voxelSize * 0.5f;
            float2 low     = float.MaxValue;
            float2 high    = float.MinValue;
            for (int corner = 0; corner < 8; corner++)
            {
                var point = new float3((corner & 1) != 0 ? aabbMax.x : aabbMin.x,
                                       (corner & 2) != 0 ? aabbMax.y : aabbMin.y,
                                       (corner & 4) != 0 ? aabbMax.z : aabbMin.z) - centroid;
                var projected = new float2(math.dot(point, tangentU), math.dot(point, tangentV));
                low           = math.min(low, projected);
                high          = math.max(high, projected);
            }
            var cells = (int2)math.ceil((high - low) / cellSize);
            if (math.any(cells < 3))
                return;
            if (math.any(cells > kMaxSliceCells))
            {
                // A diagonal slice spans further than an axis-aligned one, so its cells grow instead of
                // its grid. A coarser plate is a smaller plate, never an unsafe one.
                cellSize = math.cmax(high - low) / kMaxSliceCells;
                cells    = math.min((int2)math.ceil((high - low) / cellSize), kMaxSliceCells);
                if (math.any(cells < 3))
                    return;
            }

            var usable = new NativeArray<bool>(cells.x * cells.y, Allocator.Temp, NativeArrayOptions.ClearMemory);
            for (int y = 0; y < cells.y; y++)
            {
                for (int x = 0; x < cells.x; x++)
                    usable[y * cells.x + x] = CellIsInterior(low + new float2(x, y) * cellSize, cellSize, centroid, tangentU, tangentV, gridOrigin, voxelSize, resolution, interior);
            }

            // Surface plates facing this way already cover the silhouette the slice would, and a box
            // is nothing but those. Their areas get foreshortened onto the axis, then halved, because a
            // closed mesh has a front and back face for every direction and only one is the
            // silhouette.
            float surfaceCoverage = 0f;
            for (int i = surfacePlateStart; i < surfacePlateEnd; i++)
            {
                float facing = math.abs(math.dot(outPlates[i].planeNormal, axis));
                if (facing >= kDedupeCosHalfAngle)
                    surfaceCoverage += outPlates[i].area * facing;
            }

            var polygon = new NativeList<float2>(kPolygonVertices, Allocator.Temp);

            // A cross section isn't always one convex piece. Slice a torus and you get an annulus,
            // where the biggest disc that fits only spans the wall thickness, while the ring is many
            // times wider. Taking only the biggest piece leaves a ring covering a twentieth of itself.
            // So cut the best piece, strike it out, and ask again, just like the surface path does
            // with a patch.
            for (int piece = 0; piece < kMaxPlatesPerSlice; piece++)
            {
                polygon.Clear();
                float  bestArea   = 0f;
                bool   bestRound  = false;
                float2 bestCentre = default;
                float  bestRadius = 0f;
                int4   bestRect   = default;

                if (OccluderGrid.LargestInscribedCircle(usable, cells.x, cells.y, out var circleCenter, out var circleRadius))
                {
                    // Pull the polygon in by one cell to cover the gap between a cell's center, where
                    // the distance was measured from, and its corners.
                    float usableRadius = (circleRadius - 1f) * cellSize;
                    if (usableRadius > 0f)
                    {
                        var candidate = new NativeList<float2>(kPolygonVertices, Allocator.Temp);
                        var origin    = low + circleCenter * cellSize;
                        for (int i = 0; i < kPolygonVertices; i++)
                        {
                            math.sincos(i * 2f * math.PI / kPolygonVertices, out var sin, out var cos);
                            candidate.Add(origin + new float2(cos, sin) * usableRadius);
                        }
                        float area = PolygonArea(candidate.AsArray());
                        if (area > bestArea)
                        {
                            bestArea   = area;
                            bestRound  = true;
                            bestCentre = circleCenter;
                            bestRadius = circleRadius;
                            polygon.Clear();
                            for (int i = 0; i < candidate.Length; i++)
                                polygon.Add(candidate[i]);
                        }
                        candidate.Dispose();
                    }
                }

                if (OccluderGrid.LargestRectangle(usable, cells.x, cells.y, out var rect))
                {
                    var corner0 = low + new float2(rect.x,     rect.y) * cellSize;
                    var corner1 = low + new float2(rect.z + 1, rect.w + 1) * cellSize;
                    float area  = (corner1.x - corner0.x) * (corner1.y - corner0.y);
                    if (area > bestArea)
                    {
                        bestArea  = area;
                        bestRound = false;
                        bestRect  = rect;
                        polygon.Clear();
                        polygon.Add(new float2(corner0.x, corner0.y));
                        polygon.Add(new float2(corner1.x, corner0.y));
                        polygon.Add(new float2(corner1.x, corner1.y));
                        polygon.Add(new float2(corner0.x, corner1.y));
                    }
                }

                // Only compared against the largest piece. A direction the surface already covers is
                // covered, whether the slice would be cut into one piece or six.
                if (piece == 0 && surfaceCoverage * 0.5f >= bestArea)
                    break;
                if (bestArea < minArea || polygon.Length < 3)
                    break;

                Emit(polygon.AsArray(), centroid, tangentU, tangentV, axis, bestArea, ref outPlates, ref outVertices);
                StrikeOut(usable, cells, bestRound, bestCentre, bestRadius, bestRect);
            }

            polygon.Dispose();
            usable.Dispose();
        }

        /// <summary>
        /// How many pieces one slice can be cut into before leaving the rest to other axes.
        /// </summary>
        private const int kMaxPlatesPerSlice = 6;

        /// <summary>
        /// Strikes out the cells a plate just claimed, so the next pass over the same cross section
        /// finds somewhere else.
        /// </summary>
        /// <remarks>
        /// This strikes out generously. Cells the plate only touches get cleared along with the ones it
        /// covers, so two pieces can't share a border cell and stack plates on the same sliver.
        /// </remarks>
        static void StrikeOut(NativeArray<bool> usable, int2 cells, bool round, float2 centre, float radius, int4 rect)
        {
            if (round)
            {
                int x0 = math.max((int)math.floor(centre.x - radius), 0);
                int x1 = math.min((int)math.ceil(centre.x + radius), cells.x - 1);
                int y0 = math.max((int)math.floor(centre.y - radius), 0);
                int y1 = math.min((int)math.ceil(centre.y + radius), cells.y - 1);
                for (int y = y0; y <= y1; y++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        if (math.lengthsq(new float2(x, y) - centre) <= radius * radius)
                            usable[y * cells.x + x] = false;
                    }
                }
                return;
            }

            for (int y = math.max(rect.y, 0); y <= math.min(rect.w, cells.y - 1); y++)
            {
                for (int x = math.max(rect.x, 0); x <= math.min(rect.z, cells.x - 1); x++)
                    usable[y * cells.x + x] = false;
            }
        }

        static bool CellIsInterior(float2 cellLow, float cellSize, float3 centroid, float3 tangentU, float3 tangentV,
                                   float3 gridOrigin, float voxelSize, int3 resolution, NativeArray<bool> interior)
        {
            var low  = (float3)float.MaxValue;
            var high = (float3)float.MinValue;
            for (int corner = 0; corner < 4; corner++)
            {
                var uv    = cellLow + new float2((corner & 1) != 0 ? cellSize : 0f, (corner & 2) != 0 ? cellSize : 0f);
                var point = centroid + tangentU * uv.x + tangentV * uv.y;
                low       = math.min(low, point);
                high      = math.max(high, point);
            }

            var lowVoxel  = (int3)math.floor((low - gridOrigin) / voxelSize);
            var highVoxel = (int3)math.floor((high - gridOrigin) / voxelSize);
            if (math.any(lowVoxel < 0) || math.any(highVoxel >= resolution))
                return false;

            for (int z = lowVoxel.z; z <= highVoxel.z; z++)
            {
                for (int y = lowVoxel.y; y <= highVoxel.y; y++)
                {
                    for (int x = lowVoxel.x; x <= highVoxel.x; x++)
                    {
                        if (!interior[(z * resolution.y + y) * resolution.x + x])
                            return false;
                    }
                }
            }
            return true;
        }

        static float PolygonArea(NativeArray<float2> polygon)
        {
            float doubled = 0f;
            for (int i = 0; i < polygon.Length; i++)
            {
                var a    = polygon[i];
                var b    = polygon[(i + 1) % polygon.Length];
                doubled += Cross(a, b);
            }
            return math.abs(doubled) * 0.5f;
        }

        static void Emit(NativeArray<float2> polygon, float3 centroid, float3 tangentU, float3 tangentV, float3 axis, float area,
                         ref UnsafeList<OccluderPlate> outPlates, ref UnsafeList<float3> outVertices)
        {
            float doubled = 0f;
            for (int i = 0; i < polygon.Length; i++)
                doubled += Cross(polygon[i], polygon[(i + 1) % polygon.Length]);

            int  start = outVertices.Length;
            bool flip  = doubled < 0f;
            for (int i = 0; i < polygon.Length; i++)
            {
                var source = polygon[flip ? polygon.Length - 1 - i : i];
                outVertices.Add(centroid + tangentU * source.x + tangentV * source.y);
            }

            outPlates.Add(new OccluderPlate
            {
                planeNormal      = axis,
                planeDistance    = math.dot(axis, centroid),
                coneAxis         = axis,
                coneCosHalfAngle = kConeCosHalfAngle,
                // A slice sits inside the solid, and a ray could enter through any part of the surface
                // to reach it. So the region is everywhere outside the mesh's hull. The cone above only
                // decides which slices are worth rasterizing, not which are valid.
                regionKind       = PeekabooRegionKind.MeshHullExterior,
                area             = area,
                vertexStart      = start,
                vertexCount      = polygon.Length,
                doubleSided      = true,
            });
        }
    }
}
