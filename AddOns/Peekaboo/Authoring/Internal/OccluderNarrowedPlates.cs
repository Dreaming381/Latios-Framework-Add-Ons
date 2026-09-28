using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Cuts plates that depend on a single submesh, by narrowing their validity region to a cone of
    /// view directions. This is how a curved submesh gets plates without depending on the rest of the
    /// mesh, and how an open curved mesh like a terrain chunk gets plates at all.
    /// </summary>
    /// <remarks>
    /// The plate sits flat below a submesh's surface, facing one of 26 directions d. For a cone
    /// half-angle θ, the surface is every triangle of the submesh facing within 90° - θ of d. Those
    /// triangles face toward any ray that arrives within θ of -d.
    ///
    /// Why a ray from the cone has to hit that surface before the plate:
    ///
    /// 1.  The plate is below every surface vertex, and the viewer is above every surface vertex.
    /// 2.  Look straight down d. The surface's outline is its boundary edges, meaning edges used by
    ///     an odd number of its triangles. Where the outline winds around a point an odd number of
    ///     times, a vertical line through that point crosses the surface an odd number of times.
    ///     That's a mod-2 degree argument, so it holds for any triangle soup, folds and all.
    /// 3.  Only cells the outline never touches, and with odd winding, are kept. Call them U.
    /// 4.  A ray tilted at most θ drifts sideways at most (height difference) * tan θ while it climbs
    ///     from the plate to just above the surface. Plate cells are eroded by that much, so the
    ///     whole climb stays over U, and never crosses the outline.
    /// 5.  Sliding that climb sideways until it's vertical never touches the outline, so the number
    ///     of surface crossings keeps its parity. It stays odd, so it's at least one.
    ///
    /// Only this submesh's triangles are used. Rule 5 only needs the ray to hit a rendered front face
    /// at or before the plate, and other geometry can only add hits, never remove one.
    /// </remarks>
    internal static unsafe class OccluderNarrowedPlates
    {
        const int   kGridResolution  = 48;
        const float kDedupeCos       = 0.1f;
        // Tried widest first. A wider cone works from more angles, but erodes more and keeps fewer
        // triangles as front-facing.
        static readonly float[] kHalfAnglesDegrees = { 35f, 25f, 15f };

        public static void Build(NativeArray<float3> points,
                                 NativeArray<int>    welded,
                                 NativeArray<int3>   triangles,
                                 NativeArray<int>    triangleSubmeshes,
                                 ulong submeshes,
                                 float minArea,
                                 float scale,
                                 int existingPlateStart,
                                 ref UnsafeList<OccluderPlate> outPlates,
                                 ref UnsafeList<float3>        outVertices)
        {
            int existingPlateEnd = outPlates.Length;
            var members          = new NativeList<int>(triangles.Length, Allocator.Temp);
            for (int s = 0; s < 64; s++)
            {
                if ((submeshes & (1ul << s)) == 0)
                    continue;
                members.Clear();
                for (int i = 0; i < triangles.Length; i++)
                {
                    if (triangleSubmeshes[i] == s)
                        members.Add(i);
                }
                if (members.Length < 2)
                    continue;

                for (int axis = 0; axis < OccluderInteriorPlates.kAxisCount; axis++)
                {
                    for (int sign = 0; sign < 2; sign++)
                    {
                        var direction = OccluderInteriorPlates.Axis(axis) * (sign == 0 ? 1f : -1f);
                        float covered = SurfaceCoverage(ref outPlates, existingPlateStart, existingPlateEnd, s, direction);
                        for (int attempt = 0; attempt < kHalfAnglesDegrees.Length; attempt++)
                        {
                            if (TryBuildPlate(points, welded, triangles, members.AsArray(), s, direction, math.radians(kHalfAnglesDegrees[attempt]),
                                              minArea, covered, scale, ref outPlates, ref outVertices))
                                break;
                        }
                    }
                }
            }
            members.Dispose();
        }

        // Surface plates on this submesh already facing this way.
        static float SurfaceCoverage(ref UnsafeList<OccluderPlate> plates, int start, int end, int submesh, float3 direction)
        {
            float covered = 0f;
            for (int i = start; i < end; i++)
            {
                if (plates[i].submesh != submesh)
                    continue;
                float facing = math.dot(plates[i].planeNormal, direction);
                if (facing >= kDedupeCos)
                    covered += plates[i].area * facing;
            }
            return covered;
        }

        static bool TryBuildPlate(NativeArray<float3> points,
                                  NativeArray<int>    welded,
                                  NativeArray<int3>   triangles,
                                  NativeArray<int>    members,
                                  int submesh,
                                  float3 direction,
                                  float halfAngle,
                                  float minArea,
                                  float alreadyCovered,
                                  float scale,
                                  ref UnsafeList<OccluderPlate> outPlates,
                                  ref UnsafeList<float3>        outVertices)
        {
            math.sincos(halfAngle, out float sinHalf, out float cosHalf);
            // A little past sin θ, so rounding can't let a triangle that's edge-on to the cone's
            // widest ray through.
            float frontThreshold = sinHalf + 1e-3f;

            var seed     = math.abs(direction.x) < 0.9f ? new float3(1f, 0f, 0f) : new float3(0f, 1f, 0f);
            var tangentU = math.normalize(math.cross(direction, seed));
            var tangentV = math.cross(direction, tangentU);

            // The surface, and its outline as edges used an odd number of times.
            var edges     = new NativeHashMap<int2, int>(members.Length * 3, Allocator.Temp);
            float lowest  = float.PositiveInfinity;
            float highest = float.NegativeInfinity;
            var lower     = (float2)float.PositiveInfinity;
            var upper     = (float2)float.NegativeInfinity;
            int surfaceCount = 0;
            for (int m = 0; m < members.Length; m++)
            {
                var t = triangles[members[m]];
                var w = new int3(welded[t.x], welded[t.y], welded[t.z]);
                if (w.x == w.y || w.y == w.z || w.z == w.x)
                    continue;
                var a      = points[t.x];
                var b      = points[t.y];
                var c      = points[t.z];
                var normal = math.normalizesafe(math.cross(b - a, c - a));
                if (!(math.dot(normal, direction) > frontThreshold))
                    continue;
                surfaceCount++;
                ToggleEdge(ref edges, w.x, w.y);
                ToggleEdge(ref edges, w.y, w.z);
                ToggleEdge(ref edges, w.z, w.x);
                for (int k = 0; k < 3; k++)
                {
                    var p   = points[t[k]];
                    float h = math.dot(p, direction);
                    lowest  = math.min(lowest, h);
                    highest = math.max(highest, h);
                    var q   = new float2(math.dot(p, tangentU), math.dot(p, tangentV));
                    lower   = math.min(lower, q);
                    upper   = math.max(upper, q);
                }
            }

            var outline = new NativeList<float4>(64, Allocator.Temp);
            foreach (var pair in edges)
            {
                if ((pair.Value & 1) == 0)
                    continue;
                // Welded ids index the representative vertex, which sits at the same position.
                var pa = points[pair.Key.x];
                var pb = points[pair.Key.y];
                outline.Add(new float4(math.dot(pa, tangentU), math.dot(pa, tangentV), math.dot(pb, tangentU), math.dot(pb, tangentV)));
            }
            edges.Dispose();

            var extent = upper - lower;
            if (surfaceCount < 2 || outline.Length < 3 || !math.all(extent > 0f) || !math.all(math.isfinite(extent)) || extent.x * extent.y < minArea)
            {
                outline.Dispose();
                return false;
            }

            // Square cells, so erosion distances mean the same thing on both axes.
            float cellSize = math.cmax(extent) / kGridResolution;
            var   cells    = math.clamp((int2)math.ceil(extent / cellSize), 1, kGridResolution);

            var blocked = new NativeArray<bool>(cells.x * cells.y, Allocator.Temp, NativeArrayOptions.ClearMemory);
            for (int i = 0; i < outline.Length; i++)
                MarkSegment(blocked, cells, outline[i], lower, cellSize);
            var inside = new NativeArray<bool>(cells.x * cells.y, Allocator.Temp, NativeArrayOptions.ClearMemory);
            FillOddWinding(inside, blocked, cells, outline.AsArray(), lower, cellSize);
            outline.Dispose();
            blocked.Dispose();

            // The plate sits just below the lowest surface vertex. The climb only has to reach just
            // above the highest one.
            float margin     = OccluderPlateBaker.kRegionEpsilon * scale;
            float plateLevel = lowest - margin;
            float climb      = highest + margin - plateLevel;
            float drift      = climb * sinHalf / cosHalf;

            // A cell is usable when every point in it is at least `drift` from every point of a cell
            // outside U. Center to center distance loses up to a half diagonal on each end.
            var squared = new NativeArray<float>(cells.x * cells.y, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            OccluderGrid.SquaredDistanceToUnset(inside, cells.x, cells.y, squared);
            float needed   = drift / cellSize + math.SQRT2 + 0.01f;
            var   eligible = new NativeArray<bool>(cells.x * cells.y, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < eligible.Length; i++)
                eligible[i] = inside[i] && squared[i] >= needed * needed;
            squared.Dispose();
            inside.Dispose();

            bool built = false;
            if (OccluderGrid.LargestRectangle(eligible, cells.x, cells.y, out var rect))
            {
                var corner0 = lower + new float2(rect.x, rect.y) * cellSize;
                var corner1 = lower + new float2(rect.z + 1, rect.w + 1) * cellSize;
                float area  = (corner1.x - corner0.x) * (corner1.y - corner0.y);
                if (area >= minArea && area > alreadyCovered)
                {
                    Emit(corner0, corner1, tangentU, tangentV, direction, plateLevel, climb, halfAngle, area, submesh, margin, ref outPlates, ref outVertices);
                    built = true;
                }
            }
            eligible.Dispose();
            return built;
        }

        static void Emit(float2 corner0, float2 corner1, float3 tangentU, float3 tangentV, float3 direction, float plateLevel, float climb,
                         float halfAngle, float area, int submesh, float margin, ref UnsafeList<OccluderPlate> outPlates, ref UnsafeList<float3> outVertices)
        {
            int start = outVertices.Length;
            // Counter-clockwise seen from +direction, since tangentU x tangentV = direction.
            outVertices.Add(tangentU * corner0.x + tangentV * corner0.y + direction * plateLevel);
            outVertices.Add(tangentU * corner1.x + tangentV * corner0.y + direction * plateLevel);
            outVertices.Add(tangentU * corner1.x + tangentV * corner1.y + direction * plateLevel);
            outVertices.Add(tangentU * corner0.x + tangentV * corner1.y + direction * plateLevel);

            var middle   = (corner0 + corner1) * 0.5f;
            var centroid = tangentU * middle.x + tangentV * middle.y + direction * plateLevel;
            float radius = math.length(corner1 - corner0) * 0.5f;

            // Every point of the plate has to see the viewer within the cone, which pushes the tip
            // back radius / tan θ. The viewer also has to be above the whole surface.
            math.sincos(halfAngle, out float sinHalf, out float cosHalf);
            float push = math.max(radius * cosHalf / sinHalf, climb) + margin;

            outPlates.Add(new OccluderPlate
            {
                planeNormal      = direction,
                planeDistance    = plateLevel,
                coneAxis         = direction,
                coneCosHalfAngle = cosHalf,
                regionKind       = PeekabooRegionKind.Cone,
                regionA          = new float4(direction, cosHalf),
                regionB          = new float4(centroid + direction * push, 0f),
                area             = area,
                submesh          = (short)submesh,
                vertexStart      = start,
                vertexCount      = 4,
            });
        }

        static void ToggleEdge(ref NativeHashMap<int2, int> edges, int a, int b)
        {
            var key = new int2(math.min(a, b), math.max(a, b));
            edges.TryGetValue(key, out var count);
            edges[key] = count + 1;
        }

        /// <summary>
        /// Blocks every cell an outline edge passes through, plus its direct neighbors.
        /// </summary>
        static void MarkSegment(NativeArray<bool> blocked, int2 cells, float4 segment, float2 lower, float cellSize)
        {
            var a     = (segment.xy - lower) / cellSize;
            var b     = (segment.zw - lower) / cellSize;
            int steps = (int)math.ceil(math.max(math.abs(b.x - a.x), math.abs(b.y - a.y))) * 2 + 1;
            for (int s = 0; s <= steps; s++)
            {
                var p  = math.lerp(a, b, s / (float)steps);
                int cx = (int)math.floor(p.x);
                int cy = (int)math.floor(p.y);
                for (int dy = -1; dy <= 1; dy++)
                {
                    int y = cy + dy;
                    if (y < 0 || y >= cells.y)
                        continue;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int x = cx + dx;
                        if (x >= 0 && x < cells.x)
                            blocked[y * cells.x + x] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Marks unblocked cells whose center the outline winds around an odd number of times.
        /// </summary>
        static void FillOddWinding(NativeArray<bool> inside, NativeArray<bool> blocked, int2 cells, NativeArray<float4> outline, float2 lower, float cellSize)
        {
            var crossings = new NativeList<float>(64, Allocator.Temp);
            for (int row = 0; row < cells.y; row++)
            {
                float y = lower.y + (row + 0.5f) * cellSize;
                crossings.Clear();
                for (int i = 0; i < outline.Length; i++)
                {
                    var s = outline[i];
                    if ((s.y <= y) == (s.w <= y))
                        continue;
                    float t = (y - s.y) / (s.w - s.y);
                    crossings.Add(s.x + t * (s.z - s.x));
                }
                if (crossings.Length < 2)
                    continue;
                crossings.Sort();
                for (int x = 0; x < cells.x; x++)
                {
                    int index = row * cells.x + x;
                    if (blocked[index])
                        continue;
                    float centerX = lower.x + (x + 0.5f) * cellSize;
                    int   count   = 0;
                    while (count < crossings.Length && crossings[count] < centerX)
                        count++;
                    inside[index] = (count & 1) == 1;
                }
            }
            crossings.Dispose();
        }
    }
}
