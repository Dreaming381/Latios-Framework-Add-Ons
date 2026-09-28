using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Builds the set of occluder plates for a single mesh.
    /// </summary>
    /// <remarks>
    /// Every plate has to be inside the solid the mesh encloses. Occlusion culling depends on that: a
    /// ray that reaches a plate has already passed through the real surface, so the plate is never
    /// closer than the surface. The safest way to get there is to take plates right off the surface.
    /// Coplanar triangles get merged into one flat patch, and a convex polygon gets cut out of it.
    ///
    /// Merging also simplifies things. A wall of 200 triangles becomes one polygon, and a box becomes
    /// six. Each plate carries the cone of directions it faces, so picking plates at runtime also
    /// does backface culling.
    /// </remarks>
    internal static class OccluderPlateBaker
    {
        public const int kMaxPlateVertices = 8;
        // A plate's region is open, so a viewer exactly on its boundary is outside it.
        public const float kRegionEpsilon  = 1e-4f;
        public const int kMaxPlatesPerMesh = 64;
        public const int kGridResolution   = 48;

        // Coplanarity is strict on purpose. A plate gets snapped onto its cluster's fitted plane, and
        // any slack here is room for the plate to drift off the real surface.
        const float kCoplanarCosine       = 0.99999f;
        const float kCoplanarOffsetFactor = 1e-5f;
        const float kWeldFactor           = 1e-5f;

        // A plate too small to cover a pixel in any realistic buffer costs more to rasterize than it
        // saves.
        const float kMinPlateAreaFraction = 0.01f;

        public struct Result
        {
            public int    plateStart;
            public int    plateCount;
            public float  largestPlateArea;
            public float  boundsCrossSectionArea;
            public OccluderHull hull;
            public ulong  shellSubmeshes;
        }

        public static unsafe Result Build(in Mesh.MeshData meshData,
                                          ref UnsafeList<OccluderPlate> outPlates,
                                          ref UnsafeList<float3>        outVertices)
        {
            var result     = new Result { plateStart = outPlates.Length };
            int vertexCount = meshData.vertexCount;
            if (vertexCount < 3)
                return result;

            var positions = new NativeArray<Vector3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            meshData.GetVertices(positions);
            var points = positions.Reinterpret<float3>();

            var triangles        = new NativeList<int3>(1024, Allocator.Temp);
            var triangleSubmeshes = new NativeList<int>(1024, Allocator.Temp);
            GatherTriangles(meshData, ref triangles, ref triangleSubmeshes);
            if (triangles.Length == 0)
                return result;

            // Only the vertices LOD 0 uses. The vertex buffer can also hold other LODs' vertices.
            var hull = OccluderHull.Empty;
            for (int i = 0; i < triangles.Length; i++)
            {
                var t = triangles[i];
                hull.Include(points[t.x]);
                hull.Include(points[t.y]);
                hull.Include(points[t.z]);
            }
            var aabbMin      = hull.axisMin;
            var aabbMax      = hull.axisMax;
            var boundsExtent = aabbMax - aabbMin;
            float scale      = math.cmax(boundsExtent);
            if (!(scale > 0f) || !math.all(math.isfinite(boundsExtent)))
                return result;
            // Grown by the same epsilon as the half spaces, so a viewer exactly on the bounds counts
            // as inside the hull and the slices get rejected.
            hull.Grow(kRegionEpsilon * scale);
            result.hull = hull;
            result.boundsCrossSectionArea = math.cmax(new float3(boundsExtent.x * boundsExtent.y,
                                                                 boundsExtent.y * boundsExtent.z,
                                                                 boundsExtent.z * boundsExtent.x));

            var welded = WeldVertices(points, scale * kWeldFactor, Allocator.Temp);
            var minArea = result.boundsCrossSectionArea * kMinPlateAreaFraction;

            var clusters = ClusterCoplanarTriangles(points, welded, triangles.AsArray(), triangleSubmeshes.AsArray(), scale, Allocator.Temp);
            BuildPlatesFromClusters(points, welded, triangles.AsArray(), triangleSubmeshes.AsArray(), clusters, scale, minArea, ref outPlates, ref outVertices);

            // Slices through the inside of the mesh cover what the surface can't. Anything round has
            // no flat face to take a plate from, but plenty of room inside for one. Slices need a
            // closed surface, or "inside" doesn't mean anything.
            result.shellSubmeshes = FindShellSubmeshes(welded, triangles.AsArray(), triangleSubmeshes.AsArray());
            if (result.shellSubmeshes != 0)
            {
                var solid = new NativeList<int3>(triangles.Length, Allocator.Temp);
                CollectShellTriangles(welded, triangles.AsArray(), triangleSubmeshes.AsArray(), result.shellSubmeshes, ref solid);
                int interiorStart = outPlates.Length;
                OccluderInteriorPlates.Build(points, solid.AsArray(), aabbMin, aabbMax, minArea, result.plateStart, ref outPlates, ref outVertices);
                for (int i = interiorStart; i < outPlates.Length; i++)
                {
                    var plate      = outPlates[i];
                    plate.submesh  = -1;
                    outPlates[i]   = plate;
                }
                solid.Dispose();
            }

            // Plates that depend on one submesh, for when the shell's plates can't be used. That's a
            // mesh with several submeshes, where one of them might be drawn transparent, or an open
            // mesh with no shell at all. A closed mesh with one submesh gets nothing from these.
            ulong drawnSubmeshes = 0;
            for (int i = 0; i < triangleSubmeshes.Length; i++)
                drawnSubmeshes |= 1ul << triangleSubmeshes[i];
            if (math.countbits(drawnSubmeshes) > 1 || result.shellSubmeshes == 0)
            {
                OccluderNarrowedPlates.Build(points, welded, triangles.AsArray(), triangleSubmeshes.AsArray(), drawnSubmeshes, minArea, scale,
                                             result.plateStart, ref outPlates, ref outVertices);
            }
            triangleSubmeshes.Dispose();

            result.plateCount = outPlates.Length - result.plateStart;
            if (result.plateCount > kMaxPlatesPerMesh)
            {
                KeepLargestPlates(ref outPlates, result.plateStart, kMaxPlatesPerMesh);
                result.plateCount = kMaxPlatesPerMesh;
            }
            for (int i = 0; i < result.plateCount; i++)
                result.largestPlateArea = math.max(result.largestPlateArea, outPlates[result.plateStart + i].area);
            return result;
        }

        /// <summary>
        /// Collects the triangles of every submesh at mesh LOD 0, along with which submesh each came
        /// from.
        /// </summary>
        /// <remarks>
        /// With mesh LODs, a submesh descriptor spans the index ranges of every LOD, but reading the
        /// indices only writes LOD 0's. So the count comes from LOD 0's range instead. Submeshes past 64
        /// are skipped, since a plate's submesh dependency is checked against a 64-bit mask.
        /// </remarks>
        static void GatherTriangles(in Mesh.MeshData meshData, ref NativeList<int3> triangles, ref NativeList<int> triangleSubmeshes)
        {
            for (int s = 0; s < meshData.subMeshCount && s < 64; s++)
            {
                if (meshData.GetSubMesh(s).topology != MeshTopology.Triangles)
                    continue;
                var indices = ReadLod0Indices(in meshData, s);
                for (int i = 0; i + 2 < indices.Length; i += 3)
                {
                    triangles.Add(new int3(indices[i], indices[i + 1], indices[i + 2]));
                    triangleSubmeshes.Add(s);
                }
                indices.Dispose();
            }
        }

        static NativeArray<int> ReadLod0Indices(in Mesh.MeshData meshData, int submesh)
        {
            int indexCount = meshData.lodCount > 1 ? (int)meshData.GetLod(submesh, 0).indexCount : meshData.GetSubMesh(submesh).indexCount;
            var indices    = new NativeArray<int>(indexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            if (meshData.lodCount > 1)
                meshData.GetIndices(indices, submesh, 0);
            else
                meshData.GetIndices(indices, submesh);
            return indices;
        }

        /// <summary>
        /// A hash of everything <see cref="Build"/> reads from a mesh. Two meshes with the same hash
        /// get the same plates.
        /// </summary>
        /// <remarks>
        /// That's the vertex positions, and the LOD 0 indices of the first 64 triangle submeshes. Keep
        /// this in sync with what Build and GatherTriangles read.
        /// </remarks>
        public static unsafe Unity.Entities.Hash128 HashInputs(in Mesh.MeshData meshData)
        {
            var state       = new xxHash3.StreamingState(false);
            int vertexCount = meshData.vertexCount;
            int submeshes   = math.min(meshData.subMeshCount, 64);
            state.Update(&vertexCount, sizeof(int));
            state.Update(&submeshes, sizeof(int));

            var positions = new NativeArray<Vector3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            meshData.GetVertices(positions);
            state.Update(positions.GetUnsafeReadOnlyPtr(), vertexCount * UnsafeUtility.SizeOf<Vector3>());
            positions.Dispose();

            for (int s = 0; s < submeshes; s++)
            {
                int triangles = meshData.GetSubMesh(s).topology == MeshTopology.Triangles ? 1 : 0;
                state.Update(&triangles, sizeof(int));
                if (triangles == 0)
                    continue;
                var indices    = ReadLod0Indices(in meshData, s);
                int indexCount = indices.Length;
                state.Update(&indexCount, sizeof(int));
                state.Update(indices.GetUnsafeReadOnlyPtr(), indexCount * sizeof(int));
                indices.Dispose();
            }
            return new Unity.Entities.Hash128(state.DigestHash128());
        }

        /// <summary>
        /// Maps every vertex to one representative from its group of coincident vertices, so
        /// triangles meeting along a UV or normal seam are seen as sharing an edge.
        /// </summary>
        static NativeArray<int> WeldVertices(NativeArray<float3> points, float epsilon, AllocatorManager.AllocatorHandle allocator)
        {
            int count   = points.Length;
            var mapping = CollectionHelper.CreateNativeArray<int>(count, allocator, NativeArrayOptions.UninitializedMemory);
            float cell  = math.max(epsilon, 1e-9f);
            var buckets = new NativeParallelMultiHashMap<int3, int>(count * 2, Allocator.Temp);

            for (int i = 0; i < count; i++)
            {
                var key   = (int3)math.floor(points[i] / cell);
                int match = -1;
                for (int dz = -1; dz <= 1 && match < 0; dz++)
                {
                    for (int dy = -1; dy <= 1 && match < 0; dy++)
                    {
                        for (int dx = -1; dx <= 1 && match < 0; dx++)
                        {
                            if (!buckets.TryGetFirstValue(key + new int3(dx, dy, dz), out var candidate, out var iterator))
                                continue;
                            do
                            {
                                if (math.distancesq(points[candidate], points[i]) <= epsilon * epsilon)
                                {
                                    match = candidate;
                                    break;
                                }
                            }
                            while (buckets.TryGetNextValue(out candidate, ref iterator));
                        }
                    }
                }
                if (match < 0)
                {
                    buckets.Add(key, i);
                    mapping[i] = i;
                }
                else
                {
                    mapping[i] = mapping[match];
                }
            }
            buckets.Dispose();
            return mapping;
        }

        struct TrianglePlane
        {
            public float3 normal;
            public float  distance;
            public float  area;
        }

        static bool ComputePlane(NativeArray<float3> points, int3 triangle, out TrianglePlane plane)
        {
            var a      = points[triangle.x];
            var b      = points[triangle.y];
            var c      = points[triangle.z];
            var cross  = math.cross(b - a, c - a);
            float norm = math.length(cross);
            if (!(norm > 1e-20f))
            {
                plane = default;
                return false;
            }
            plane = new TrianglePlane
            {
                normal   = cross / norm,
                area     = norm * 0.5f,
            };
            plane.distance = math.dot(plane.normal, a);
            return true;
        }

        /// <summary>
        /// Groups triangles that share an edge and lie in the same plane, which turns a subdivided flat
        /// face back into a single patch.
        /// </summary>
        static NativeArray<int> ClusterCoplanarTriangles(NativeArray<float3> points,
                                                        NativeArray<int>    welded,
                                                        NativeArray<int3>   triangles,
                                                        NativeArray<int>    triangleSubmeshes,
                                                        float scale,
                                                        AllocatorManager.AllocatorHandle allocator)
        {
            int count  = triangles.Length;
            var parent = CollectionHelper.CreateNativeArray<int>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < count; i++)
                parent[i] = i;

            var planes = new NativeArray<TrianglePlane>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
            var valid  = new NativeArray<bool>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
            for (int i = 0; i < count; i++)
            {
                if (ComputePlane(points, WeldTriangle(welded, triangles[i]), out var plane))
                {
                    planes[i] = plane;
                    valid[i]  = true;
                }
            }

            float offsetEpsilon = scale * kCoplanarOffsetFactor;
            var   edgeOwners    = new NativeParallelMultiHashMap<int2, int>(count * 3, Allocator.Temp);
            for (int i = 0; i < count; i++)
            {
                if (!valid[i])
                    continue;
                var t = WeldTriangle(welded, triangles[i]);
                AddEdge(ref edgeOwners, t.x, t.y, i);
                AddEdge(ref edgeOwners, t.y, t.z, i);
                AddEdge(ref edgeOwners, t.z, t.x, i);
            }

            var keys = edgeOwners.GetKeyArray(Allocator.Temp);
            for (int k = 0; k < keys.Length; k++)
            {
                if (!edgeOwners.TryGetFirstValue(keys[k], out var first, out var iterator))
                    continue;
                while (edgeOwners.TryGetNextValue(out var other, ref iterator))
                {
                    // A patch stays within one submesh, so its plates depend on only that submesh.
                    if (triangleSubmeshes[first] != triangleSubmeshes[other])
                        continue;
                    var pa = planes[first];
                    var pb = planes[other];
                    if (math.dot(pa.normal, pb.normal) >= kCoplanarCosine && math.abs(pa.distance - pb.distance) <= offsetEpsilon)
                        Union(parent, first, other);
                }
            }
            keys.Dispose();
            edgeOwners.Dispose();
            planes.Dispose();

            for (int i = 0; i < count; i++)
                parent[i] = Find(parent, i);
            for (int i = 0; i < count; i++)
            {
                if (!valid[i])
                    parent[i] = -1;
            }
            valid.Dispose();
            return parent;
        }

        static int3 WeldTriangle(NativeArray<int> welded, int3 triangle) => new int3(welded[triangle.x], welded[triangle.y], welded[triangle.z]);

        static bool IsDegenerate(int3 welded) => welded.x == welded.y || welded.y == welded.z || welded.z == welded.x;

        /// <summary>
        /// Finds the submeshes that together form a closed surface, as a bitmask. Zero means there
        /// isn't one. That's the minimum needed for "inside the mesh" to mean anything.
        /// </summary>
        /// <remarks>
        /// This starts with every submesh, then repeatedly drops any submesh with a triangle on an edge
        /// that isn't shared by exactly two triangles. A decal quad or a detail strip gets dropped,
        /// and the solid body is left. Dropping never adds edges, so this stops once nothing changes.
        ///
        /// Degenerate triangles get ignored instead of counting as open edges. A sphere with a vertex
        /// fan at each pole has a ring of them, and they don't carry any surface.
        /// </remarks>
        static ulong FindShellSubmeshes(NativeArray<int> welded, NativeArray<int3> triangles, NativeArray<int> triangleSubmeshes)
        {
            ulong shell = 0;
            for (int i = 0; i < triangles.Length; i++)
            {
                if (!IsDegenerate(WeldTriangle(welded, triangles[i])))
                    shell |= 1ul << triangleSubmeshes[i];
            }

            var edges = new NativeHashMap<int2, int2>(triangles.Length * 3, Allocator.Temp);
            while (shell != 0)
            {
                // Per edge: how many shell triangles use it, and which submeshes they came from.
                edges.Clear();
                int triangleCount = 0;
                for (int i = 0; i < triangles.Length; i++)
                {
                    var t = WeldTriangle(welded, triangles[i]);
                    if (IsDegenerate(t) || (shell & (1ul << triangleSubmeshes[i])) == 0)
                        continue;
                    triangleCount++;
                    CountEdge(ref edges, t.x, t.y, triangleSubmeshes[i]);
                    CountEdge(ref edges, t.y, t.z, triangleSubmeshes[i]);
                    CountEdge(ref edges, t.z, t.x, triangleSubmeshes[i]);
                }
                if (triangleCount < 4)
                {
                    shell = 0;
                    break;
                }

                ulong open = 0;
                for (int i = 0; i < triangles.Length; i++)
                {
                    var t = WeldTriangle(welded, triangles[i]);
                    if (IsDegenerate(t) || (shell & (1ul << triangleSubmeshes[i])) == 0)
                        continue;
                    if (!IsClosedEdge(ref edges, t.x, t.y) || !IsClosedEdge(ref edges, t.y, t.z) || !IsClosedEdge(ref edges, t.z, t.x))
                        open |= 1ul << triangleSubmeshes[i];
                }
                if (open == 0)
                    break;
                shell &= ~open;
            }
            edges.Dispose();
            return shell;
        }

        static void CollectShellTriangles(NativeArray<int> welded, NativeArray<int3> triangles, NativeArray<int> triangleSubmeshes, ulong shell, ref NativeList<int3> solid)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                if ((shell & (1ul << triangleSubmeshes[i])) != 0 && !IsDegenerate(WeldTriangle(welded, triangles[i])))
                    solid.Add(triangles[i]);
            }
        }

        static void CountEdge(ref NativeHashMap<int2, int2> edges, int a, int b, int submesh)
        {
            var key = new int2(math.min(a, b), math.max(a, b));
            edges.TryGetValue(key, out var value);
            edges[key] = new int2(value.x + 1, submesh);
        }

        static bool IsClosedEdge(ref NativeHashMap<int2, int2> edges, int a, int b)
        {
            edges.TryGetValue(new int2(math.min(a, b), math.max(a, b)), out var value);
            return value.x == 2;
        }

        static void AddEdge(ref NativeParallelMultiHashMap<int2, int> map, int a, int b, int triangle)
        {
            if (a == b)
                return;
            map.Add(new int2(math.min(a, b), math.max(a, b)), triangle);
        }

        static int Find(NativeArray<int> parent, int index)
        {
            while (parent[index] != index)
            {
                parent[index] = parent[parent[index]];
                index         = parent[index];
            }
            return index;
        }

        static void Union(NativeArray<int> parent, int a, int b)
        {
            int ra = Find(parent, a);
            int rb = Find(parent, b);
            if (ra == rb)
                return;
            if (ra < rb)
                parent[rb] = ra;
            else
                parent[ra] = rb;
        }

        /// <summary>
        /// Trims a mesh to the plate budget, keeping a spread of directions instead of just the
        /// biggest plates.
        /// </summary>
        /// <remarks>
        /// Area alone is the wrong ranking, and a hollow mesh shows why. Each patch on the outside of
        /// a shell is bigger than the matching patch on the inside, since the outside has a bigger
        /// radius. Ranking by area gives every slot to outward-facing plates and throws away all the
        /// inward ones. Then a dome with perfectly good plates for a camera inside it ends up with
        /// none.
        ///
        /// A plate is worth the viewpoints it serves. So plates get grouped by the direction they face
        /// and which side of the mesh they're on, and the budget gets dealt out round-robin across
        /// those groups. Every direction gets its best plate before any direction gets a second. A
        /// mesh with fewer plates than the budget isn't touched, which is almost every mesh.
        /// </remarks>
        static void KeepLargestPlates(ref UnsafeList<OccluderPlate> plates, int start, int keep)
        {
            int count = plates.Length - start;
            if (count <= keep)
                return;

            for (int i = 1; i < count; i++)
            {
                var value = plates[start + i];
                int j     = i - 1;
                while (j >= 0 && plates[start + j].area < value.area)
                {
                    plates[start + j + 1] = plates[start + j];
                    j--;
                }
                plates[start + j + 1] = value;
            }

            var taken  = new NativeArray<bool>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
            var rounds = new NativeArray<int>(kDirectionBuckets, Allocator.Temp, NativeArrayOptions.ClearMemory);
            var kept   = new UnsafeList<OccluderPlate>(keep, Allocator.Temp);

            for (int round = 0; kept.Length < keep; round++)
            {
                bool progressed = false;
                // Plates are sorted by descending area, so the first unclaimed plate in a group is its
                // largest remaining one.
                for (int i = 0; i < count && kept.Length < keep; i++)
                {
                    if (taken[i])
                        continue;
                    int bucket = DirectionBucket(plates[start + i]);
                    if (rounds[bucket] != round)
                        continue;
                    rounds[bucket]++;
                    taken[i] = true;
                    kept.Add(plates[start + i]);
                    progressed = true;
                }
                if (!progressed)
                    break;
            }

            for (int i = 0; i < kept.Length; i++)
                plates[start + i] = kept[i];
            plates.Resize(start + kept.Length, NativeArrayOptions.UninitializedMemory);

            taken.Dispose();
            rounds.Dispose();
            kept.Dispose();
        }

        const int kDirectionBuckets = 27 * 2 * 2;

        /// <summary>
        /// Which group a plate competes in. That's the direction it faces, rounded to one of 27, which
        /// side of the mesh's center it's on, and whether it depends on a single submesh through a
        /// narrowed cone.
        /// </summary>
        /// <remarks>
        /// The side is what separates the two faces of a shell. A patch on the outside at +X and a
        /// patch on the inside at -X both face +X. By direction alone, they'd be in the same group and
        /// the bigger outer one would always win. But they serve opposite halves of space, so they need
        /// to compete separately.
        ///
        /// Narrowed plates compete separately for the same reason. They're only used when the shell's
        /// plates can't be, like when a car's glass is drawn transparent, so bigger slices facing the
        /// same way can't replace them.
        /// </remarks>
        static int DirectionBucket(in OccluderPlate plate)
        {
            var normal    = math.normalizesafe(plate.planeNormal);
            var quantized = math.clamp((int3)math.round(normal), -1, 1);
            int direction = (quantized.x + 1) + 3 * (quantized.y + 1) + 9 * (quantized.z + 1);
            int narrowed  = plate.submesh >= 0 && plate.regionKind == PeekabooRegionKind.Cone ? 1 : 0;
            return (direction * 2 + (plate.planeDistance < 0f ? 1 : 0)) * 2 + narrowed;
        }

        struct ClusterComparer : System.Collections.Generic.IComparer<int2>
        {
            public int Compare(int2 a, int2 b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y);
        }

        static void BuildPlatesFromClusters(NativeArray<float3> points,
                                            NativeArray<int>    welded,
                                            NativeArray<int3>   triangles,
                                            NativeArray<int>    triangleSubmeshes,
                                            NativeArray<int>    clusters,
                                            float scale,
                                            float minArea,
                                            ref UnsafeList<OccluderPlate> outPlates,
                                            ref UnsafeList<float3>        outVertices)
        {
            var ordered = new NativeList<int2>(clusters.Length, Allocator.Temp);
            for (int i = 0; i < clusters.Length; i++)
            {
                if (clusters[i] >= 0)
                    ordered.Add(new int2(clusters[i], i));
            }
            ordered.Sort(new ClusterComparer());

            var members = new NativeList<int>(64, Allocator.Temp);
            int cursor  = 0;
            while (cursor < ordered.Length)
            {
                int root = ordered[cursor].x;
                members.Clear();
                while (cursor < ordered.Length && ordered[cursor].x == root)
                {
                    members.Add(ordered[cursor].y);
                    cursor++;
                }
                int patchStart = outPlates.Length;
                OccluderPlatePatch.Build(points, welded, triangles, members.AsArray(), scale, minArea, ref outPlates, ref outVertices);
                for (int i = patchStart; i < outPlates.Length; i++)
                {
                    var plate     = outPlates[i];
                    plate.submesh = (short)triangleSubmeshes[members[0]];
                    outPlates[i]  = plate;
                }
            }
            members.Dispose();
            ordered.Dispose();
        }
    }
}
