using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Why a plate was rejected. Anything other than <see cref="Added"/> means the plate was
    /// discarded and the builder didn't change.
    /// </summary>
    public enum PeekabooPlateResult
    {
        /// <summary>The plate was accepted.</summary>
        Added,
        /// <summary>The mesh index was negative or past the builder's mesh count.</summary>
        MeshIndexOutOfRange,
        /// <summary>Fewer than three vertices, so it isn't a polygon.</summary>
        TooFewVertices,
        /// <summary>More vertices than <see cref="PeekabooOccluderBuilder.kMaxPlateVertices"/>.</summary>
        TooManyVertices,
        /// <summary>A vertex was infinite or NaN.</summary>
        NotFinite,
        /// <summary>The vertices are collinear or all in one spot, so the polygon has no area.</summary>
        Degenerate,
        /// <summary>The vertices don't lie flat enough in one plane.</summary>
        NotPlanar,
        /// <summary>The polygon bends both ways. Only convex polygons can be rasterized.</summary>
        NotConvex,
    }

    /// <summary>
    /// Builds an occluder blob from plates you provide, for geometry Peekaboo can't bake from a mesh.
    /// That includes heightmap terrain, procedural surfaces, and anything generated at runtime.
    /// </summary>
    /// <remarks>
    /// A plate is a convex polygon that sits <em>entirely inside</em> the solid it stands for. That's
    /// the one rule, and it's the one thing this builder can't check for you. A plate is allowed to
    /// claim that pixels are covered, so if it pokes outside its geometry, it will hide things that
    /// are clearly visible. Cut plates from inside your surface, not across it, and leave some margin
    /// if you're unsure.
    ///
    /// What the builder does check is everything that would break the rasterizer: a plate has to be
    /// flat, convex, not collapsed to a line, and within the vertex limit. Convexity really matters.
    /// The rasterizer fills the span between the two edges each scanline crosses, so a polygon that
    /// bends both ways would get its dent filled in. That would claim occlusion where there's no
    /// geometry.
    ///
    /// The cone angle is for plates that don't sit right on a surface. A plate buried in the middle
    /// of a solid is fully covered when seen head-on, but at a shallow enough angle the solid ends
    /// before the plate does. Passing that angle keeps the plate from being used past it. A plate
    /// lying flat on a surface works from anywhere in front of it, which is the default of 90 degrees.
    ///
    /// You can add plates for several meshes in any order. They get grouped by mesh when the blob is
    /// built. The mesh index is the renderer's RenderMeshArray mesh index. A renderer drawing a
    /// runtime registered mesh can only use a blob with a single mesh, at index zero.
    ///
    /// Peekaboo doesn't check materials for these plates. Whatever the renderer draws with, it
    /// assumes you built plates that match what's on screen.
    /// </remarks>
    public struct PeekabooOccluderBuilder : INativeDisposable
    {
        /// <summary>
        /// The most vertices one plate can have.
        /// </summary>
        public const int kMaxPlateVertices = 16;
        const float kRegionEpsilon = 1e-4f;

        // How far a vertex can sit off the plane, relative to the polygon's size, before the plane no
        // longer describes the polygon well enough to rasterize depth along it.
        const float kPlanarTolerance = 1e-3f;

        struct Pending
        {
            public int   meshIndex;
            public int   vertexStart;
            public int   vertexCount;
            public float coneCosHalfAngle;
            public float area;
            public bool  doubleSided;
        }

        NativeList<Pending> m_plates;
        NativeList<float3>  m_vertices;
        NativeArray<AABB>   m_bounds;
        NativeArray<bool>   m_boundsGiven;

        /// <summary>How many mesh slots this builder has.</summary>
        public int meshCount => m_bounds.Length;

        /// <summary>How many plates have been accepted so far.</summary>
        public int plateCount => m_plates.Length;

        /// <summary>
        /// Creates a builder for a blob with <paramref name="meshCount"/> mesh slots.
        /// </summary>
        /// <param name="meshCount">How many meshes the renderer using this blob can draw. Usually one,
        /// unless the renderer draws a range.</param>
        public PeekabooOccluderBuilder(int meshCount, AllocatorManager.AllocatorHandle allocator)
        {
            meshCount     = math.max(meshCount, 1);
            m_plates      = new NativeList<Pending>(16, allocator);
            m_vertices    = new NativeList<float3>(16 * 4, allocator);
            m_bounds      = CollectionHelper.CreateNativeArray<AABB>(meshCount, allocator, NativeArrayOptions.ClearMemory);
            m_boundsGiven = CollectionHelper.CreateNativeArray<bool>(meshCount, allocator, NativeArrayOptions.ClearMemory);
        }

        /// <summary>
        /// Adds a plate for a mesh, in the same local space as the renderer's mesh.
        /// </summary>
        /// <param name="meshIndex">Which of the renderer's meshes this plate belongs to</param>
        /// <param name="vertices">A flat convex polygon lying inside the solid. Either winding works.</param>
        /// <param name="coneHalfAngleDegrees">How far from the plate's normal a viewer can be and still
        /// use it. The default of 90 means anywhere in front of the plate, which is right for a plate
        /// lying on a surface. For a plate buried inside a solid, pass the angle where the solid stops
        /// covering it.</param>
        /// <param name="doubleSided">Whether the plate can also be used from behind. Only true for a
        /// surface that's solid on both sides.</param>
        public PeekabooPlateResult TryAddPlate(int meshIndex,
                                               NativeArray<float3> vertices,
                                               float coneHalfAngleDegrees = 90f,
                                               bool doubleSided = false)
        {
            if (meshIndex < 0 || meshIndex >= m_bounds.Length)
                return PeekabooPlateResult.MeshIndexOutOfRange;
            if (vertices.Length < 3)
                return PeekabooPlateResult.TooFewVertices;
            if (vertices.Length > kMaxPlateVertices)
                return PeekabooPlateResult.TooManyVertices;

            var centroid = float3.zero;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (!math.all(math.isfinite(vertices[i])))
                    return PeekabooPlateResult.NotFinite;
                centroid += vertices[i];
            }
            centroid /= vertices.Length;

            // Newell's method gives a good plane even for a polygon that's only nearly flat, instead of
            // picking three vertices and hoping.
            var accumulated = float3.zero;
            for (int i = 0; i < vertices.Length; i++)
                accumulated += math.cross(vertices[i] - centroid, vertices[(i + 1) % vertices.Length] - centroid);

            float doubleArea = math.length(accumulated);
            if (!(doubleArea > 1e-20f))
                return PeekabooPlateResult.Degenerate;
            var normal = accumulated / doubleArea;
            float area = doubleArea * 0.5f;

            // Relative to the polygon's own size, so the same shape gets the same answer at any scale.
            float extent = math.sqrt(area);
            for (int i = 0; i < vertices.Length; i++)
            {
                if (math.abs(math.dot(vertices[i] - centroid, normal)) > kPlanarTolerance * extent)
                    return PeekabooPlateResult.NotPlanar;
            }

            if (!IsConvex(vertices, normal))
                return PeekabooPlateResult.NotConvex;

            int start = m_vertices.Length;
            for (int i = 0; i < vertices.Length; i++)
                m_vertices.Add(vertices[i]);

            m_plates.Add(new Pending
            {
                meshIndex        = meshIndex,
                vertexStart      = start,
                vertexCount      = vertices.Length,
                coneCosHalfAngle = math.cos(math.radians(math.clamp(coneHalfAngleDegrees, 0f, 90f))),
                area             = area,
                doubleSided      = doubleSided,
            });
            return PeekabooPlateResult.Added;
        }

        /// <summary>
        /// Tells the builder the renderer's real bounds for a mesh. Peekaboo compares plates against
        /// these bounds when ranking occluders.
        /// </summary>
        /// <remarks>
        /// This is optional. Without it, the plates' own bounds get used instead. If your plates don't
        /// reach the renderer's full size, that makes the biggest plate look like it covers more of the
        /// renderer than it really does. That only moves the occluder up in the ranking, never changes
        /// what gets culled, but it's better to be accurate.
        /// </remarks>
        public void SetMeshBounds(int meshIndex, AABB bounds)
        {
            if (meshIndex < 0 || meshIndex >= m_bounds.Length)
                return;
            m_bounds[meshIndex]      = bounds;
            m_boundsGiven[meshIndex] = true;
        }

        /// <summary>
        /// Builds the blob. Returns a null reference if no plates were ever accepted, which is what a
        /// renderer with nothing to occlude with should get.
        /// </summary>
        public BlobAssetReference<PeekabooOccluderBlob> CreateBlobAssetReference(AllocatorManager.AllocatorHandle allocator)
        {
            if (m_plates.Length == 0)
                return default;

            var builder     = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<PeekabooOccluderBlob>();
            var meshArray   = builder.Allocate(ref root.meshes,   m_bounds.Length);
            var plateArray  = builder.Allocate(ref root.plates,   m_plates.Length);
            var vertexArray = builder.Allocate(ref root.vertices, m_vertices.Length);
            builder.Allocate(ref root.opaqueMaterials, 0);
            root.handBuilt = true;

            // Plates get grouped by mesh here, so callers can add them in any order.
            int plateCursor  = 0;
            int vertexCursor = 0;
            for (int mesh = 0; mesh < m_bounds.Length; mesh++)
            {
                int   start    = plateCursor;
                float largest  = 0f;
                var   low      = new float3(float.PositiveInfinity);
                var   high     = new float3(float.NegativeInfinity);

                for (int i = 0; i < m_plates.Length; i++)
                {
                    var pending = m_plates[i];
                    if (pending.meshIndex != mesh)
                        continue;

                    plateArray[plateCursor] = new OccluderPlate
                    {
                        coneCosHalfAngle = pending.coneCosHalfAngle,
                        doubleSided      = pending.doubleSided,
                        area             = pending.area,
                        submesh          = -1,
                        vertexStart      = vertexCursor,
                        vertexCount      = pending.vertexCount,
                    };

                    var centroid = float3.zero;
                    for (int v = 0; v < pending.vertexCount; v++)
                    {
                        var vertex                = m_vertices[pending.vertexStart + v];
                        vertexArray[vertexCursor++] = vertex;
                        centroid                 += vertex;
                        low                       = math.min(low,  vertex);
                        high                      = math.max(high, vertex);
                    }
                    centroid /= pending.vertexCount;

                    // Computed from the stored vertices, so the blob's plane matches the polygon the
                    // rasterizer actually draws.
                    var accumulated = float3.zero;
                    for (int v = 0; v < pending.vertexCount; v++)
                    {
                        var a        = m_vertices[pending.vertexStart + v] - centroid;
                        var b        = m_vertices[pending.vertexStart + (v + 1) % pending.vertexCount] - centroid;
                        accumulated += math.cross(a, b);
                    }
                    var normal                     = math.normalizesafe(accumulated);
                    plateArray[plateCursor].planeNormal   = normal;
                    plateArray[plateCursor].planeDistance = math.dot(normal, centroid);
                    plateArray[plateCursor].coneAxis      = normal;

                    // The validity region comes from the caller's cone angle. A viewer has to see every
                    // point of the plate within that angle, not just its middle. For a plate of radius
                    // r, that works out to one cone with the same angle, with its tip pushed back
                    // r / tan(angle) along the normal. At 90 degrees the push is zero, and the region
                    // becomes the plate's own half space.
                    float radius = 0f;
                    for (int v = 0; v < pending.vertexCount; v++)
                        radius = math.max(radius, math.length(m_vertices[pending.vertexStart + v] - centroid));

                    float cosHalf = pending.coneCosHalfAngle;
                    float sinHalf = math.sqrt(math.max(1f - cosHalf * cosHalf, 0f));
                    float push    = sinHalf > 1e-4f ? radius * cosHalf / sinHalf : 0f;

                    // A double-sided plate gets the same cone mirrored onto its back side.
                    float offset                       = push + radius * kRegionEpsilon;
                    plateArray[plateCursor].regionKind = pending.doubleSided ? PeekabooRegionKind.DoubleCone : PeekabooRegionKind.Cone;
                    plateArray[plateCursor].regionA    = new float4(normal, cosHalf);
                    plateArray[plateCursor].regionB    = new float4(centroid + normal * offset, 2f * offset);

                    largest = math.max(largest, pending.area);
                    plateCursor++;
                }

                var extent = m_boundsGiven[mesh] ? m_bounds[mesh].Extents * 2f : math.max(high - low, 0f);
                if (!math.all(math.isfinite(extent)))
                    extent = 0f;
                float crossSection = math.cmax(new float3(extent.x * extent.y, extent.y * extent.z, extent.z * extent.x));

                var hullLow  = m_boundsGiven[mesh] ? m_bounds[mesh].Center - m_bounds[mesh].Extents : low;
                var hullHigh = m_boundsGiven[mesh] ? m_bounds[mesh].Center + m_bounds[mesh].Extents : high;
                if (!math.all(math.isfinite(hullLow)) || !math.all(math.isfinite(hullHigh)))
                {
                    hullLow  = float3.zero;
                    hullHigh = float3.zero;
                }

                meshArray[mesh] = new OccluderMesh
                {
                    plateStart             = start,
                    plateCount             = plateCursor - start,
                    largestPlateArea       = largest,
                    boundsCrossSectionArea = crossSection,
                    hull                   = OccluderHull.FromBox(hullLow, hullHigh),
                };
            }

            var blob = builder.CreateBlobAssetReference<PeekabooOccluderBlob>(allocator);
            builder.Dispose();
            return blob;
        }

        static bool IsConvex(NativeArray<float3> polygon, float3 normal)
        {
            int sign = 0;
            for (int i = 0; i < polygon.Length; i++)
            {
                var a    = polygon[i];
                var b    = polygon[(i + 1) % polygon.Length];
                var c    = polygon[(i + 2) % polygon.Length];
                float turn = math.dot(math.cross(b - a, c - b), normal);
                int   s    = turn > 0f ? 1 : (turn < 0f ? -1 : 0);
                if (s == 0)
                    continue;
                if (sign == 0)
                    sign = s;
                else if (sign != s)
                    return false;
            }
            return sign != 0;
        }

        public void Dispose()
        {
            m_plates.Dispose();
            m_vertices.Dispose();
            m_bounds.Dispose();
            m_boundsGiven.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            return JobHandle.CombineDependencies(m_plates.Dispose(inputDeps),
                                                 m_vertices.Dispose(inputDeps),
                                                 JobHandle.CombineDependencies(m_bounds.Dispose(inputDeps), m_boundsGiven.Dispose(inputDeps)));
        }
    }
}
