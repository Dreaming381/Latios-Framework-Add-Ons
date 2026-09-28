using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Peekaboo
{
    /// <summary>
    /// The shape of a plate's validity region, which is the volume the viewer must be inside for
    /// the plate to be used.
    /// </summary>
    /// <remarks>
    /// A plate is only safe to use from certain viewpoints. Every ray from inside the region that
    /// reaches the plate is guaranteed to pass a real surface first, so the region records where
    /// that guarantee holds.
    ///
    /// None is the default on purpose. A plate the baker couldn't find a region for is never used,
    /// so a gap in baking only costs culling, never correctness.
    /// </remarks>
    public enum PeekabooRegionKind : byte
    {
        /// <summary>Never used.</summary>
        None = 0,
        /// <summary>
        /// The open half space where dot(regionA.xyz, p) is greater than regionA.w. Plates lying on a
        /// surface get this, since a ray from the outward side hits that surface right at the plate.
        /// </summary>
        HalfSpace = 1,
        /// <summary>
        /// The open cone with its tip at regionB.xyz, axis regionA.xyz, and the cosine of its half
        /// angle in regionA.w. Plates set back inside a solid get this, since the solid only covers
        /// them from a limited range of directions.
        /// </summary>
        Cone = 2,
        /// <summary>
        /// Everywhere outside the mesh's <see cref="OccluderHull"/>. Outside the hull means outside the
        /// solid, so a ray to a point inside has to cross the surface on the way in. Slices through the
        /// middle of a mesh get this, since any part of the surface could be the one a ray crosses.
        /// </summary>
        MeshHullExterior = 3,
        /// <summary>
        /// Two open cones mirrored across the plate, for plates that are valid from both sides. The
        /// front cone is the same as <see cref="Cone"/>. The back cone has its tip regionB.w behind the
        /// front tip along -regionA.xyz, and points the other way.
        /// </summary>
        DoubleCone = 4,
    }

    /// <summary>
    /// A convex shape around a mesh, made of the slab between two planes along each of 13 directions:
    /// the 3 axes, the 4 body diagonals, and the 6 edge directions of a cube. It's what
    /// <see cref="PeekabooRegionKind.MeshHullExterior"/> plates are valid outside of.
    /// </summary>
    /// <remarks>
    /// Any shape works as long as outside it means outside the solid. The mesh's bounding box works,
    /// but a round mesh leaves big empty corners in it, and a camera standing in one of those corners
    /// can't use the mesh's interior plates. The diagonal slabs cut those corners off.
    /// </remarks>
    public struct OccluderHull
    {
        /// <summary>The slabs along x, y, and z. This is the bounding box.</summary>
        public float3 axisMin;
        /// <inheritdoc cref="axisMin"/>
        public float3 axisMax;
        /// <summary>The slabs along (1,1,1), (-1,1,1), (1,-1,1), and (1,1,-1), normalized.</summary>
        public float4 diagonalMin;
        /// <inheritdoc cref="diagonalMin"/>
        public float4 diagonalMax;
        /// <summary>The slabs along (1,1,0), (1,-1,0), and (1,0,1), normalized.</summary>
        public float3 edgeMinA;
        /// <inheritdoc cref="edgeMinA"/>
        public float3 edgeMaxA;
        /// <summary>The slabs along (1,0,-1), (0,1,1), and (0,1,-1), normalized.</summary>
        public float3 edgeMinB;
        /// <inheritdoc cref="edgeMinB"/>
        public float3 edgeMaxB;

        /// <summary>
        /// A hull containing nothing, ready for <see cref="Include"/>.
        /// </summary>
        public static OccluderHull Empty => new OccluderHull
        {
            axisMin     = float.PositiveInfinity,
            axisMax     = float.NegativeInfinity,
            diagonalMin = float.PositiveInfinity,
            diagonalMax = float.NegativeInfinity,
            edgeMinA    = float.PositiveInfinity,
            edgeMaxA    = float.NegativeInfinity,
            edgeMinB    = float.PositiveInfinity,
            edgeMaxB    = float.NegativeInfinity,
        };

        /// <summary>
        /// The hull of a box, which is just the box.
        /// </summary>
        public static OccluderHull FromBox(float3 min, float3 max)
        {
            var hull = Empty;
            for (int corner = 0; corner < 8; corner++)
                hull.Include(new float3((corner & 1) != 0 ? max.x : min.x, (corner & 2) != 0 ? max.y : min.y, (corner & 4) != 0 ? max.z : min.z));
            return hull;
        }

        /// <summary>
        /// Grows the hull to contain a point.
        /// </summary>
        public void Include(float3 point)
        {
            Project(point, out var diagonal, out var edgeA, out var edgeB);
            axisMin     = math.min(axisMin, point);
            axisMax     = math.max(axisMax, point);
            diagonalMin = math.min(diagonalMin, diagonal);
            diagonalMax = math.max(diagonalMax, diagonal);
            edgeMinA    = math.min(edgeMinA, edgeA);
            edgeMaxA    = math.max(edgeMaxA, edgeA);
            edgeMinB    = math.min(edgeMinB, edgeB);
            edgeMaxB    = math.max(edgeMaxB, edgeB);
        }

        /// <summary>
        /// Pushes every plane out by the given distance.
        /// </summary>
        public void Grow(float distance)
        {
            axisMin     -= distance;
            axisMax     += distance;
            diagonalMin -= distance;
            diagonalMax += distance;
            edgeMinA    -= distance;
            edgeMaxA    += distance;
            edgeMinB    -= distance;
            edgeMaxB    += distance;
        }

        /// <summary>
        /// Where a point lands along the diagonal and edge directions.
        /// </summary>
        public static void Project(float3 point, out float4 diagonal, out float3 edgeA, out float3 edgeB)
        {
            const float invSqrt3 = 0.57735026919f;
            const float invSqrt2 = 0.70710678118f;
            diagonal = new float4(point.x + point.y + point.z, -point.x + point.y + point.z, point.x - point.y + point.z, point.x + point.y - point.z) * invSqrt3;
            edgeA    = new float3(point.x + point.y, point.x - point.y, point.x + point.z) * invSqrt2;
            edgeB    = new float3(point.x - point.z, point.y + point.z, point.y - point.z) * invSqrt2;
        }

        /// <summary>
        /// Whether a point is inside the axis-aligned slabs, meaning inside the bounding box.
        /// </summary>
        public bool ContainsAxisAligned(float3 point) => math.all(point >= axisMin & point <= axisMax);

        /// <summary>
        /// Whether the hull contains any volume. Empty hulls come from meshes without plates.
        /// </summary>
        public bool isValid => math.all(axisMin <= axisMax) && math.all(math.isfinite(axisMin)) && math.all(math.isfinite(axisMax));
    }

    /// <summary>
    /// A convex polygon baked inside the solid volume of an occluder mesh, in mesh-local space.
    /// Since the polygon never pokes outside the mesh, its depth is always at or behind the real
    /// surface, which is what keeps occlusion culling from hiding anything visible.
    /// </summary>
    public struct OccluderPlate
    {
        /// <summary>
        /// Which shape <see cref="regionA"/> and <see cref="regionB"/> describe.
        /// </summary>
        public PeekabooRegionKind regionKind;
        /// <summary>
        /// For HalfSpace, the plane as a normal in xyz and a distance in w. For Cone, the axis in xyz
        /// and the cosine of the half angle in w.
        /// </summary>
        public float4 regionA;
        /// <summary>
        /// For Cone, the tip in xyz. Unused otherwise.
        /// </summary>
        public float4 regionB;
        /// <summary>
        /// The unit normal of the polygon's plane, in mesh-local space.
        /// </summary>
        public float3 planeNormal;
        /// <summary>
        /// Every point p on the plate satisfies dot(planeNormal, p) == planeDistance.
        /// </summary>
        public float planeDistance;
        /// <summary>
        /// The axis of the cone of view directions this plate is worth rasterizing for, in mesh-local
        /// space. The cone only affects performance. Rasterizing a plate outside its cone is still
        /// correct, just wasteful.
        /// </summary>
        public float3 coneAxis;
        /// <summary>
        /// The cosine of the cone's half angle. The plate is used when the direction from the plate
        /// toward the viewer has a dot product with coneAxis of at least this value.
        /// </summary>
        public float coneCosHalfAngle;
        /// <summary>
        /// Whether the cone points both ways. Surface plates only face outward, but a slice through a
        /// solid works from either side.
        /// </summary>
        public bool doubleSided;
        /// <summary>
        /// The polygon's area in mesh-local space, used to rank plates against each other.
        /// </summary>
        public float area;
        /// <summary>
        /// Which submesh has to be drawn opaque for this plate to be used. -1 means every submesh in
        /// <see cref="OccluderMesh.shellSubmeshes"/> does.
        /// </summary>
        /// <remarks>
        /// A plate lying on a surface only depends on the submesh it was cut from. A slice through the
        /// middle of a solid could be reached through any part of the closed surface around it, so it
        /// depends on all of it.
        /// </remarks>
        public short submesh;
        /// <summary>
        /// The index of this plate's first vertex in PeekabooOccluderBlob.vertices.
        /// </summary>
        public int vertexStart;
        /// <summary>
        /// The number of vertices in this plate. Vertices wind counter-clockwise when viewed from the
        /// side planeNormal points toward.
        /// </summary>
        public int vertexCount;
    }

    /// <summary>
    /// The plates baked for one mesh in a RenderMeshArray.
    /// </summary>
    public struct OccluderMesh
    {
        /// <summary>
        /// The index of this mesh's first plate in PeekabooOccluderBlob.plates.
        /// </summary>
        public int plateStart;
        /// <summary>
        /// The number of plates for this mesh. Zero means the mesh never occludes.
        /// </summary>
        public int plateCount;
        /// <summary>
        /// The area of the largest plate. Along with boundsCrossSectionArea, this estimates how much
        /// of the mesh's silhouette its plates can fill, which helps rank occluders.
        /// </summary>
        public float largestPlateArea;
        /// <summary>
        /// The hull a <see cref="PeekabooRegionKind.MeshHullExterior"/> plate is valid outside of.
        /// </summary>
        public OccluderHull hull;
        /// <summary>
        /// The largest cross-section area of the mesh's local bounds. See largestPlateArea.
        /// </summary>
        public float boundsCrossSectionArea;
        /// <summary>
        /// One bit per submesh, set for the submeshes that together form the closed surface interior
        /// plates were cut from. Submeshes left out, like a decal quad, don't have to be opaque for
        /// those plates to be used.
        /// </summary>
        public ulong shellSubmeshes;
    }

    /// <summary>
    /// The plates baked for a whole RenderMeshArray. Every entity sharing that RenderMeshArray
    /// references the same blob, and looks up its plates by the mesh index its MaterialMeshInfo
    /// resolves to.
    /// </summary>
    public struct PeekabooOccluderBlob
    {
        /// <summary>
        /// Lines up with RenderMeshArray.MeshReferences.
        /// </summary>
        public BlobArray<OccluderMesh> meshes;
        /// <summary>
        /// Every plate for every mesh, referenced by OccluderMesh ranges.
        /// </summary>
        public BlobArray<OccluderPlate> plates;
        /// <summary>
        /// Every plate vertex in mesh-local space, referenced by OccluderPlate ranges.
        /// </summary>
        public BlobArray<float3> vertices;
        /// <summary>
        /// Lines up with RenderMeshArray.MaterialReferences. True for materials that fully hide what's
        /// behind them: opaque, depth-writing, no alpha clipping, and not culling front faces.
        /// </summary>
        /// <remarks>
        /// A submesh only counts as drawn opaque when it's drawn with one of these. Runtime registered
        /// materials aren't in the table, so they never count.
        /// </remarks>
        public BlobArray<bool> opaqueMaterials;
        /// <summary>
        /// True for blobs made with PeekabooOccluderBuilder. The caller vouches for those plates, so
        /// every draw counts as opaque, and a single-mesh blob also works for a renderer drawing a
        /// runtime registered mesh.
        /// </summary>
        public bool handBuilt;
    }

    /// <summary>
    /// Baked onto every renderable entity whose RenderMeshArray produced plates.
    /// Usage: Read Only
    /// </summary>
    public struct PeekabooOccluder : IComponentData
    {
        /// <summary>
        /// The plates for the entity's whole RenderMeshArray.
        /// </summary>
        public BlobAssetReference<PeekabooOccluderBlob> blob;
    }

    /// <summary>
    /// Add this to a renderable entity to stop it from occluding anything. The entity can still be
    /// culled by other occluders.
    /// Usage: Add or Remove
    /// </summary>
    public struct PeekabooDisableOccluderTag : IComponentData { }

    /// <summary>
    /// Add this to a renderable entity to stop occlusion culling from ever culling it. The entity can
    /// still occlude other things.
    /// Usage: Add or Remove
    /// </summary>
    public struct PeekabooDisableOccludeeTag : IComponentData { }

    /// <summary>
    /// Settings for occlusion culling, added to the worldBlackboardEntity by PeekabooBootstrap.
    /// Usage: Read or Write. Values are read at the start of each culling pass.
    /// </summary>
    public struct PeekabooSettings : IComponentData
    {
        /// <summary>
        /// The size of the depth buffer for camera views. Both values get rounded up to a power of
        /// two and clamped to [8, 2048]. This has the biggest effect on how much gets culled. If you
        /// raise it, you'll probably want to raise the coverage floors too.
        /// </summary>
        public int2 cameraResolution;
        /// <summary>
        /// The size of the depth buffer for each shadow cascade. Both values get rounded up to a power
        /// of two and clamped to [8, 2048].
        /// </summary>
        public int2 lightResolution;
        /// <summary>
        /// The most entities rasterized as occluders in a single view. Candidates are ranked by how
        /// much of the screen they cover, and the rest get dropped.
        /// </summary>
        public int maxOccludersPerView;
        /// <summary>
        /// A candidate in a camera view that fills less than this fraction of the buffer's texels
        /// (weighted by how much of the view lies behind it) never becomes an occluder. Raising it
        /// saves time by rejecting distant chunks early, at the cost of some occluders.
        /// </summary>
        public float minCameraOccluderCoverage;
        /// <summary>
        /// The same as minCameraOccluderCoverage, but for shadow cascades. It's separate because the
        /// two projections weight depth differently, so one value can't work well for both.
        /// </summary>
        public float minLightOccluderCoverage;
        /// <summary>
        /// Whether occlusion culling runs for camera views.
        /// </summary>
        public bool cullCameras;
        /// <summary>
        /// Whether occlusion culling runs for shadow-casting lights.
        /// </summary>
        public bool cullLights;
        /// <summary>
        /// Whether occlusion culling may run without Burst. Without Burst, software rasterization is
        /// slow enough to cost more than it saves, so Peekaboo skips culling unless this is set. Use
        /// it when you need to step through the culling code in a debugger.
        /// </summary>
        public bool debugOverrideAllowWithoutBurst;

        /// <summary>
        /// The settings Peekaboo installs by default.
        /// </summary>
        public static PeekabooSettings Default => new PeekabooSettings
        {
            cameraResolution                = new int2(512, 256),
            lightResolution                 = new int2(256, 256),
            maxOccludersPerView             = 256,
            minCameraOccluderCoverage       = 0.003f,
            minLightOccluderCoverage        = 0.001f,
            cullCameras                     = true,
            cullLights                      = true,
            debugOverrideAllowWithoutBurst  = false,
        };
    }

    /// <summary>
    /// Counters added up over every culling pass in the frame, for profiling and debugging.
    /// Usage: Read Only. Lives on the worldBlackboardEntity whenever Peekaboo is installed.
    /// </summary>
    /// <remarks>
    /// A frame has several passes: one per camera, and one per shadow-casting light. Reporting only
    /// the last one would hide whichever ran first, so the counters reset on the frame's first pass
    /// and add up across the rest.
    ///
    /// These only update in the editor and development builds, unless PEEKABOO_ENABLE_STATS is
    /// defined. PEEKABOO_DISABLE_STATS turns them off everywhere. When they're off, the counting
    /// compiles out, and this stays at its defaults.
    /// </remarks>
    public struct PeekabooStats : IComponentData
    {
        /// <summary>
        /// The number of culling passes Peekaboo ran this frame.
        /// </summary>
        public int passCount;
        /// <summary>
        /// How many of those passes were for shadow-casting lights instead of cameras.
        /// </summary>
        public int lightPassCount;
        /// <summary>
        /// The number of views rasterized this frame. That's one per camera and one per shadow
        /// cascade.
        /// </summary>
        public int viewCount;
        /// <summary>
        /// The number of entity and view pairs that passed the coverage floor and competed for an
        /// occluder slot. If this is well above occluderCount, maxOccludersPerView is the limit. If
        /// it's zero, the coverage floors are.
        /// </summary>
        public int consideredCount;
        /// <summary>
        /// The most candidates any one worker held for a single view. If this reaches
        /// maxOccludersPerView, workers were dropping candidates to stay within budget.
        /// </summary>
        public int peakWorkerCandidateCount;
        /// <summary>
        /// The number of entities rasterized as occluders, added up over views.
        /// </summary>
        public int occluderCount;
        /// <summary>
        /// The number of plates rasterized, added up over views.
        /// </summary>
        public int plateCount;
        /// <summary>
        /// The number of entities that survived frustum culling and then got occlusion culled.
        /// </summary>
        public int culledCount;
        /// <summary>
        /// The number of entities that survived both frustum culling and occlusion culling. Compare
        /// it against culledCount + survivingCount, which is how many made it past frustum culling.
        /// </summary>
        public int survivingCount;
    }
}
