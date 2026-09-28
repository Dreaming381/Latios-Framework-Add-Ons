using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Cuts convex plates out of one flat patch of coplanar triangles.
    /// </summary>
    /// <remarks>
    /// The easy and most common case is a patch whose outline is already a convex polygon, like a
    /// box face, a wall, or a floor tile. That outline becomes a plate as-is, with no approximation.
    ///
    /// Everything else, like an L-shaped wall or a face with a window cut out, falls back to carving
    /// rectangles out of a grid over the patch. A cell only counts if the patch outline doesn't run
    /// through it or next to it. So a rectangle of counted cells is always inside the patch, even
    /// where the outline wanders between cells. The rectangles then grow back out and can overlap,
    /// so a pixel on the seam between two of them is still covered by at least one.
    /// </remarks>
    internal static unsafe class OccluderPlatePatch
    {
        const int   kMaxRectanglesPerPatch = 4;
        const float kFlatnessFactor        = 1e-4f;
        const float kConeCosHalfAngle      = 0.1f;
        // Relative to the mesh size. A viewer exactly in a plate's plane lands outside its region
        // instead of on the edge. Rule 5 needs an open volume, and a grazing ray is the one case where
        // the boundary behaves differently from just inside it.
        const float kRegionEpsilon         = OccluderPlateBaker.kRegionEpsilon;

        public static void Build(NativeArray<float3> points,
                                 NativeArray<int>    welded,
                                 NativeArray<int3>   triangles,
                                 NativeArray<int>    members,
                                 float scale,
                                 float minArea,
                                 ref UnsafeList<OccluderPlate> outPlates,
                                 ref UnsafeList<float3>        outVertices)
        {
            if (members.Length == 0)
                return;

            if (!FitPlane(points, triangles, members, out var normal, out var distance, out var patchArea))
                return;
            // No plate can be bigger than its patch, so skip small patches before paying for boundary
            // extraction. Detailed meshes are mostly small patches.
            if (patchArea < minArea)
                return;

            // A patch that isn't really flat can't become a plane without moving the plate off the
            // surface, so it gets dropped instead of approximated.
            float deviation = 0f;
            for (int m = 0; m < members.Length; m++)
            {
                var t = triangles[members[m]];
                deviation = math.max(deviation, math.abs(math.dot(points[t.x], normal) - distance));
                deviation = math.max(deviation, math.abs(math.dot(points[t.y], normal) - distance));
                deviation = math.max(deviation, math.abs(math.dot(points[t.z], normal) - distance));
            }
            if (deviation > scale * kFlatnessFactor)
                return;
            // Push the plane back behind every vertex of the patch, so the plate can't poke out the
            // front, which is the side that matters.
            distance -= deviation;

            var boundary = ExtractBoundaryEdges(welded, triangles, members, Allocator.Temp);
            if (boundary.Length < 3)
            {
                boundary.Dispose();
                return;
            }

            BuildTangentBasis(points, welded, boundary, normal, out var origin, out var tangentU, out var tangentV);

            var loop = new NativeList<int>(boundary.Length, Allocator.Temp);
            if (TryBuildSingleLoop(boundary, ref loop))
            {
                var polygon = new NativeList<float2>(loop.Length, Allocator.Temp);
                for (int i = 0; i < loop.Length; i++)
                {
                    var p = points[loop[i]] - origin;
                    polygon.Add(new float2(math.dot(p, tangentU), math.dot(p, tangentV)));
                }
                RemoveCollinear(ref polygon, scale * 1e-4f);
                if (polygon.Length >= 3 && polygon.Length <= OccluderPlateBaker.kMaxPlateVertices && IsConvex(polygon.AsArray()))
                {
                    EmitPolygon(polygon.AsArray(), origin, tangentU, tangentV, normal, distance, minArea, scale, ref outPlates, ref outVertices);
                    polygon.Dispose();
                    loop.Dispose();
                    boundary.Dispose();
                    return;
                }
                polygon.Dispose();
            }
            loop.Dispose();

            EmitRectangles(points, boundary, origin, tangentU, tangentV, normal, distance, minArea, scale, ref outPlates, ref outVertices);
            boundary.Dispose();
        }

        static bool FitPlane(NativeArray<float3> points, NativeArray<int3> triangles, NativeArray<int> members,
                             out float3 normal, out float distance, out float area)
        {
            float3 accumulated = 0f;
            float3 anchor      = 0f;
            float  totalArea   = 0f;
            for (int m = 0; m < members.Length; m++)
            {
                var t     = triangles[members[m]];
                var a     = points[t.x];
                var cross = math.cross(points[t.y] - a, points[t.z] - a);
                float len = math.length(cross);
                if (!(len > 1e-20f))
                    continue;
                accumulated += cross;
                anchor      += a * len;
                totalArea   += len;
            }
            if (!(totalArea > 0f) || math.lengthsq(accumulated) < 1e-30f)
            {
                normal   = default;
                distance = default;
                area     = default;
                return false;
            }
            normal   = math.normalize(accumulated);
            distance = math.dot(normal, anchor / totalArea);
            area     = totalArea * 0.5f;
            return true;
        }

        /// <summary>
        /// The edges used by exactly one triangle in the patch. Together, they form the patch's
        /// outline, including the outlines of any holes.
        /// </summary>
        static NativeList<int2> ExtractBoundaryEdges(NativeArray<int> welded, NativeArray<int3> triangles, NativeArray<int> members, AllocatorManager.AllocatorHandle allocator)
        {
            var counts = new NativeHashMap<int2, int>(members.Length * 3, Allocator.Temp);
            for (int m = 0; m < members.Length; m++)
            {
                var t = triangles[members[m]];
                CountEdge(ref counts, welded[t.x], welded[t.y]);
                CountEdge(ref counts, welded[t.y], welded[t.z]);
                CountEdge(ref counts, welded[t.z], welded[t.x]);
            }

            var result = new NativeList<int2>(64, allocator);
            for (int m = 0; m < members.Length; m++)
            {
                var t = triangles[members[m]];
                AddIfBoundary(ref counts, result, welded[t.x], welded[t.y]);
                AddIfBoundary(ref counts, result, welded[t.y], welded[t.z]);
                AddIfBoundary(ref counts, result, welded[t.z], welded[t.x]);
            }
            counts.Dispose();
            return result;
        }

        static void CountEdge(ref NativeHashMap<int2, int> counts, int a, int b)
        {
            if (a == b)
                return;
            var key = new int2(math.min(a, b), math.max(a, b));
            counts.TryGetValue(key, out var value);
            counts[key] = value + 1;
        }

        static void AddIfBoundary(ref NativeHashMap<int2, int> counts, NativeList<int2> result, int a, int b)
        {
            if (a == b)
                return;
            var key = new int2(math.min(a, b), math.max(a, b));
            if (counts.TryGetValue(key, out var value) && value == 1)
                result.Add(new int2(a, b));
        }

        static void BuildTangentBasis(NativeArray<float3> points, NativeArray<int> welded, NativeList<int2> boundary, float3 normal,
                                      out float3 origin, out float3 tangentU, out float3 tangentV)
        {
            origin = points[boundary[0].x];

            var seed = math.abs(normal.x) < 0.9f ? new float3(1f, 0f, 0f) : new float3(0f, 1f, 0f);
            var u    = math.normalize(math.cross(normal, seed));
            var v    = math.cross(normal, u);

            // Lining the basis up with the patch's main direction lets the grid's rectangles follow the
            // shape instead of cutting across it.
            //
            // This has to be measured around the boundary's own center. Second moments around any
            // other point aren't a covariance. The origin here is a corner of the patch, so every
            // point sits in one quadrant, and a symmetric shape ends up rotated 45 degrees with every
            // rectangle cut diagonally across it.
            float2 middle = 0f;
            for (int i = 0; i < boundary.Length; i++)
            {
                var p   = points[boundary[i].x] - origin;
                middle += new float2(math.dot(p, u), math.dot(p, v));
            }
            middle /= boundary.Length;

            float2 covariance = 0f;
            float  crossTerm  = 0f;
            for (int i = 0; i < boundary.Length; i++)
            {
                var p          = points[boundary[i].x] - origin;
                var projected  = new float2(math.dot(p, u), math.dot(p, v)) - middle;
                covariance    += projected * projected;
                crossTerm     += projected.x * projected.y;
            }
            float angle = 0.5f * math.atan2(2f * crossTerm, covariance.x - covariance.y);
            math.sincos(angle, out var sin, out var cos);
            tangentU = u * cos + v * sin;
            tangentV = math.cross(normal, tangentU);
        }

        /// <summary>
        /// Walks the boundary edges as one closed loop. Fails if the patch has holes or a pinched
        /// vertex, which sends the patch to the grid path instead.
        /// </summary>
        static bool TryBuildSingleLoop(NativeList<int2> boundary, ref NativeList<int> loop)
        {
            var next = new NativeHashMap<int, int>(boundary.Length, Allocator.Temp);
            for (int i = 0; i < boundary.Length; i++)
            {
                if (!next.TryAdd(boundary[i].x, boundary[i].y))
                {
                    next.Dispose();
                    return false;
                }
            }

            int start   = boundary[0].x;
            int current = start;
            for (int guard = 0; guard <= boundary.Length; guard++)
            {
                loop.Add(current);
                if (!next.TryGetValue(current, out var following))
                {
                    next.Dispose();
                    return false;
                }
                current = following;
                if (current == start)
                {
                    next.Dispose();
                    return loop.Length == boundary.Length;
                }
            }
            next.Dispose();
            return false;
        }

        static void RemoveCollinear(ref NativeList<float2> polygon, float epsilon)
        {
            for (int i = 0; i < polygon.Length && polygon.Length > 3; )
            {
                var previous = polygon[(i + polygon.Length - 1) % polygon.Length];
                var current  = polygon[i];
                var following = polygon[(i + 1) % polygon.Length];
                var cross     = Cross(current - previous, following - current);
                if (math.abs(cross) <= epsilon * math.max(math.length(current - previous), math.length(following - current)))
                {
                    polygon.RemoveAt(i);
                    i = math.max(0, i - 1);
                }
                else
                {
                    i++;
                }
            }
        }

        static float Cross(float2 a, float2 b) => a.x * b.y - a.y * b.x;

        static bool IsConvex(NativeArray<float2> polygon)
        {
            int sign = 0;
            for (int i = 0; i < polygon.Length; i++)
            {
                var a     = polygon[i];
                var b     = polygon[(i + 1) % polygon.Length];
                var c     = polygon[(i + 2) % polygon.Length];
                var cross = Cross(b - a, c - b);
                int s     = cross > 0f ? 1 : (cross < 0f ? -1 : 0);
                if (s == 0)
                    continue;
                if (sign == 0)
                    sign = s;
                else if (sign != s)
                    return false;
            }
            return sign != 0;
        }

        static void EmitPolygon(NativeArray<float2> polygon, float3 origin, float3 tangentU, float3 tangentV, float3 normal, float distance,
                                float minArea, float scale, ref UnsafeList<OccluderPlate> outPlates, ref UnsafeList<float3> outVertices)
        {
            float signedArea = 0f;
            for (int i = 0; i < polygon.Length; i++)
            {
                var a       = polygon[i];
                var b       = polygon[(i + 1) % polygon.Length];
                signedArea += Cross(a, b);
            }
            signedArea *= 0.5f;
            float area  = math.abs(signedArea);
            if (area < minArea)
                return;

            int start = outVertices.Length;
            // Wind counter-clockwise as seen from the normal side.
            bool flip = signedArea < 0f;
            for (int i = 0; i < polygon.Length; i++)
            {
                var source = polygon[flip ? polygon.Length - 1 - i : i];
                var world  = origin + tangentU * source.x + tangentV * source.y;
                outVertices.Add(world - normal * (math.dot(normal, world) - distance));
            }

            outPlates.Add(new OccluderPlate
            {
                planeNormal      = normal,
                planeDistance    = distance,
                coneAxis         = normal,
                coneCosHalfAngle = kConeCosHalfAngle,
                // A plate lying in its surface is valid from anywhere that surface faces. A ray from
                // the outward side hits the triangle right at the plate, so rule 5 holds with equal
                // distances. The epsilon keeps the region open, so a viewer exactly in the plane is
                // outside it instead of on its edge.
                regionKind       = PeekabooRegionKind.HalfSpace,
                regionA          = new float4(normal, distance + kRegionEpsilon * scale),
                area             = area,
                vertexStart      = start,
                vertexCount      = polygon.Length,
            });
        }

        static void EmitRectangles(NativeArray<float3> points,
                                   NativeList<int2>    boundary,
                                   float3 origin, float3 tangentU, float3 tangentV, float3 normal, float distance,
                                   float minArea,
                                   float scale,
                                   ref UnsafeList<OccluderPlate> outPlates,
                                   ref UnsafeList<float3>        outVertices)
        {
            int resolution = OccluderPlateBaker.kGridResolution;
            var segments   = new NativeArray<float4>(boundary.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var lowerBound = (float2)float.MaxValue;
            var upperBound = (float2)float.MinValue;
            for (int i = 0; i < boundary.Length; i++)
            {
                var a       = points[boundary[i].x] - origin;
                var b       = points[boundary[i].y] - origin;
                var pa      = new float2(math.dot(a, tangentU), math.dot(a, tangentV));
                var pb      = new float2(math.dot(b, tangentU), math.dot(b, tangentV));
                segments[i] = new float4(pa, pb);
                lowerBound  = math.min(lowerBound, math.min(pa, pb));
                upperBound  = math.max(upperBound, math.max(pa, pb));
            }
            var extent = upperBound - lowerBound;
            if (math.any(extent <= 0f) || !math.all(math.isfinite(extent)))
            {
                segments.Dispose();
                return;
            }
            var cellSize = extent / resolution;

            var blocked = new NativeArray<bool>(resolution * resolution, Allocator.Temp, NativeArrayOptions.ClearMemory);
            for (int i = 0; i < segments.Length; i++)
                MarkSegment(blocked, resolution, segments[i], lowerBound, cellSize);

            var inside = new NativeArray<bool>(resolution * resolution, Allocator.Temp, NativeArrayOptions.ClearMemory);
            FillInterior(inside, blocked, resolution, segments, lowerBound, cellSize);
            segments.Dispose();
            blocked.Dispose();

            var remaining = new NativeArray<bool>(resolution * resolution, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            remaining.CopyFrom(inside);

            var polygon = new NativeList<float2>(4, Allocator.Temp);
            for (int attempt = 0; attempt < kMaxRectanglesPerPatch; attempt++)
            {
                if (!OccluderGrid.LargestRectangle(remaining, resolution, resolution, out var rect))
                    break;
                OccluderGrid.GrowRectangle(inside, resolution, resolution, ref rect);
                for (int y = rect.y; y <= rect.w; y++)
                {
                    for (int x = rect.x; x <= rect.z; x++)
                        remaining[y * resolution + x] = false;
                }

                var corner0 = lowerBound + new float2(rect.x,     rect.y) * cellSize;
                var corner1 = lowerBound + new float2(rect.z + 1, rect.w + 1) * cellSize;
                polygon.Clear();
                polygon.Add(new float2(corner0.x, corner0.y));
                polygon.Add(new float2(corner1.x, corner0.y));
                polygon.Add(new float2(corner1.x, corner1.y));
                polygon.Add(new float2(corner0.x, corner1.y));
                EmitPolygon(polygon.AsArray(), origin, tangentU, tangentV, normal, distance, minArea, scale, ref outPlates, ref outVertices);
            }
            polygon.Dispose();
            inside.Dispose();
            remaining.Dispose();
        }

        /// <summary>
        /// Blocks every cell the outline passes through, plus its direct neighbors. That way, any cell
        /// left over is definitely clear of the outline, no matter where in the cell it runs.
        /// </summary>
        static void MarkSegment(NativeArray<bool> blocked, int resolution, float4 segment, float2 lowerBound, float2 cellSize)
        {
            var a     = (segment.xy - lowerBound) / cellSize;
            var b     = (segment.zw - lowerBound) / cellSize;
            int steps = (int)math.ceil(math.max(math.abs(b.x - a.x), math.abs(b.y - a.y))) * 2 + 1;
            for (int s = 0; s <= steps; s++)
            {
                var p  = math.lerp(a, b, s / (float)steps);
                int cx = (int)math.floor(p.x);
                int cy = (int)math.floor(p.y);
                for (int dy = -1; dy <= 1; dy++)
                {
                    int y = cy + dy;
                    if (y < 0 || y >= resolution)
                        continue;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int x = cx + dx;
                        if (x < 0 || x >= resolution)
                            continue;
                        blocked[y * resolution + x] = true;
                    }
                }
            }
        }

        static void FillInterior(NativeArray<bool> inside, NativeArray<bool> blocked, int resolution, NativeArray<float4> segments, float2 lowerBound, float2 cellSize)
        {
            var crossings = new NativeList<float>(64, Allocator.Temp);
            for (int row = 0; row < resolution; row++)
            {
                float y = lowerBound.y + (row + 0.5f) * cellSize.y;
                crossings.Clear();
                for (int i = 0; i < segments.Length; i++)
                {
                    var s  = segments[i];
                    var y0 = s.y;
                    var y1 = s.w;
                    if ((y0 <= y) == (y1 <= y))
                        continue;
                    float t = (y - y0) / (y1 - y0);
                    crossings.Add(s.x + t * (s.z - s.x));
                }
                if (crossings.Length < 2)
                    continue;
                crossings.Sort();
                for (int c = 0; c + 1 < crossings.Length; c += 2)
                {
                    int x0 = (int)math.floor((crossings[c] - lowerBound.x) / cellSize.x);
                    int x1 = (int)math.ceil((crossings[c + 1] - lowerBound.x) / cellSize.x);
                    x0     = math.max(x0, 0);
                    x1     = math.min(x1, resolution - 1);
                    for (int x = x0; x <= x1; x++)
                    {
                        int index = row * resolution + x;
                        if (!blocked[index])
                            inside[index] = true;
                    }
                }
            }
            crossings.Dispose();
        }

    }
}
