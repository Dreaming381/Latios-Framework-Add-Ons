using System.Collections.Generic;
using Latios.Kinemation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Hash128 = Unity.Entities.Hash128;

namespace Latios.Peekaboo.Authoring.Systems
{
    /// <summary>
    /// Marks a PeekabooOccluder that BakeOccluderPlatesSystem added, along with the blob it set.
    /// </summary>
    /// <remarks>
    /// Components a baking system adds stay on an entity through later incremental bakes, so the
    /// system has to remove its own when an entity stops qualifying. This tells its occluders apart
    /// from ones a user's baker added with PeekabooOccluderBuilder, which it never touches. If
    /// something else replaces the blob, the entity stops being the system's.
    /// </remarks>
    [BakingType]
    internal struct PeekabooBakedOccluder : IComponentData
    {
        public BlobAssetReference<PeekabooOccluderBlob> blob;
    }

    /// <summary>
    /// Bakes one occluder blob per RenderMeshArray and hands it to every entity sharing that array.
    /// </summary>
    /// <remarks>
    /// Baking per RenderMeshArray instead of per renderer means each mesh's plates get built once, no
    /// matter how many entities use it. At runtime, the lookup is just the mesh index the entity's
    /// MaterialMeshInfo resolves to. Nothing needs authoring. A renderer occludes unless its geometry
    /// rules it out, or the user opts it out.
    ///
    /// Baking doesn't look at which materials a mesh is drawn with. Every submesh gets plates, and
    /// each plate records the submesh it depends on. The blob also records which of the array's
    /// materials are opaque, and the culling pass only uses a plate while its submesh is drawn with
    /// one of those. A cutout or transparent material has holes the geometry doesn't, so a plate on
    /// it would hide things you can see through it.
    ///
    /// This runs on every incremental bake, over the whole subscene. So plates are cached per mesh in
    /// OccluderMeshCache, and each array's blob is kept in the BlobAssetStore under a key made from
    /// its meshes' keys and its materials. Only meshes that changed get rebuilt, and only arrays that
    /// changed get a new blob.
    /// </remarks>
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(PostBakingSystemGroup))]
    [DisableAutoCreation]
    public partial class BakeOccluderPlatesSystem : SystemBase
    {
        // Bump when the blob layout or what goes into it changes.
        const uint kBlobKeyVersion = 1;

        // Entities that could get an occluder, and don't have one from someone else.
        EntityQuery m_freeQuery;
        // Entities with an occluder this system added, that still qualify for one.
        EntityQuery m_ownedQuery;
        EntityQuery m_ownedWithOccluderQuery;
        EntityQuery m_ownedWithoutOccluderQuery;
        EntityQuery m_ownedWithoutMeshInfoQuery;
        EntityQuery m_ownedWithoutArrayQuery;
        EntityQuery m_ownedExcludedQuery;

        ComponentTypeSet m_ownedTypes;
        BakingSystem     m_bakingSystem;

        // Scratch, kept to avoid allocating every bake.
        readonly List<RenderMeshArray>        m_renderMeshArrays = new List<RenderMeshArray>();
        readonly Dictionary<Mesh, Hash128>    m_meshKeys         = new Dictionary<Mesh, Hash128>();
        readonly List<Mesh>                   m_meshesToRead     = new List<Mesh>();
        readonly List<Hash128>                m_keysToBuild      = new List<Hash128>();
        readonly HashSet<Hash128>             m_keysQueued       = new HashSet<Hash128>();

        /// <summary>
        /// How many meshes had their plates built during the last bake, instead of coming from the
        /// cache. For tests.
        /// </summary>
        internal int meshesBuiltLastUpdate { get; private set; }

        /// <summary>
        /// How many RenderMeshArrays needed a new blob during the last bake. For tests.
        /// </summary>
        internal int blobsBuiltLastUpdate { get; private set; }

        struct ArrayWork
        {
            public RenderMeshArray renderMeshArray;
            public bool[]          used;
            public Hash128         key;
            public BlobAssetReference<PeekabooOccluderBlob> blob;
        }

        protected override void OnCreate()
        {
            const EntityQueryOptions options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities;

            // These mirror what PeekabooOcclusionCullingSystem skips, so nothing gets an occluder the
            // culling pass would never use.
            var excluded = new FixedList128Bytes<ComponentType>
            {
                ComponentType.ReadOnly<PeekabooDisableOccluderTag>(),
                ComponentType.ReadOnly<DepthSorted_Tag>(),
                ComponentType.ReadOnly<SkeletonDependent>(),
                ComponentType.ReadOnly<CopyDeformFromEntity>(),
                ComponentType.ReadOnly<ShaderEffectRadialBounds>(),
                ComponentType.ReadOnly<DynamicMeshState>(),
                ComponentType.ReadOnly<BlendShapeState>(),
                ComponentType.ReadOnly<UniqueMeshConfig>(),
            };
            var noneFree = excluded;
            noneFree.Add(ComponentType.ReadOnly<PeekabooOccluder>());

            m_freeQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<MaterialMeshInfo, RenderMeshArray>().WithNone(ref noneFree).WithOptions(options).Build(this);
            m_ownedQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<MaterialMeshInfo, RenderMeshArray>().WithAll<PeekabooOccluder, PeekabooBakedOccluder>()
                           .WithNone(ref excluded).WithOptions(options).Build(this);

            m_ownedWithOccluderQuery    = new EntityQueryBuilder(Allocator.Temp).WithAll<PeekabooBakedOccluder, PeekabooOccluder>().WithOptions(options).Build(this);
            m_ownedWithoutOccluderQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PeekabooBakedOccluder>().WithNone<PeekabooOccluder>().WithOptions(options).Build(this);
            m_ownedWithoutMeshInfoQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PeekabooBakedOccluder>().WithNone<MaterialMeshInfo>().WithOptions(options).Build(this);
            m_ownedWithoutArrayQuery    = new EntityQueryBuilder(Allocator.Temp).WithAll<PeekabooBakedOccluder>().WithNone<RenderMeshArray>().WithOptions(options).Build(this);
            m_ownedExcludedQuery        = new EntityQueryBuilder(Allocator.Temp).WithAll<PeekabooBakedOccluder>().WithAny(ref excluded).WithOptions(options).Build(this);

            m_ownedTypes   = new ComponentTypeSet(ComponentType.ReadWrite<PeekabooOccluder>(), ComponentType.ReadWrite<PeekabooBakedOccluder>());
            m_bakingSystem = World.GetExistingSystemManaged<BakingSystem>();
        }

        protected override void OnUpdate()
        {
            CompleteDependency();
            meshesBuiltLastUpdate = 0;
            blobsBuiltLastUpdate  = 0;

            ReleaseReplacedOccluders();
            EntityManager.RemoveComponent<PeekabooBakedOccluder>(m_ownedWithoutOccluderQuery);
            EntityManager.RemoveComponent(m_ownedWithoutMeshInfoQuery, m_ownedTypes);
            EntityManager.RemoveComponent(m_ownedWithoutArrayQuery,    m_ownedTypes);
            EntityManager.RemoveComponent(m_ownedExcludedQuery,        m_ownedTypes);

            if (m_freeQuery.IsEmptyIgnoreFilter && m_ownedQuery.IsEmptyIgnoreFilter)
                return;

            m_renderMeshArrays.Clear();
            EntityManager.GetAllUniqueSharedComponentsManaged(m_renderMeshArrays);

            var work = new List<ArrayWork>(m_renderMeshArrays.Count);
            foreach (var renderMeshArray in m_renderMeshArrays)
            {
                var meshes = renderMeshArray.MeshReferences;
                if (meshes == null || meshes.Length == 0)
                {
                    SetFilter(renderMeshArray);
                    EntityManager.RemoveComponent(m_ownedQuery, m_ownedTypes);
                    ResetFilter();
                    continue;
                }

                SetFilter(renderMeshArray);
                if (m_freeQuery.IsEmpty && m_ownedQuery.IsEmpty)
                {
                    ResetFilter();
                    continue;
                }
                var used = new bool[meshes.Length];
                MarkUsedMeshes(renderMeshArray, m_freeQuery, used);
                MarkUsedMeshes(renderMeshArray, m_ownedQuery, used);
                ResetFilter();
                work.Add(new ArrayWork { renderMeshArray = renderMeshArray, used = used });
            }

            ComputeMeshKeys(work);
            BuildMissingMeshes();

            for (int i = 0; i < work.Count; i++)
            {
                var item = work[i];
                item.key = ComputeArrayKey(item.renderMeshArray, item.used, out var opaqueMaterials);
                if (!m_bakingSystem.BlobAssetStore.TryGet(item.key, out item.blob))
                {
                    item.blob = AssembleBlob(item.renderMeshArray, item.used, opaqueMaterials);
                    if (item.blob.IsCreated)
                    {
                        m_bakingSystem.BlobAssetStore.TryAdd(item.key, ref item.blob);
                        blobsBuiltLastUpdate++;
                    }
                }
                opaqueMaterials.Dispose();
                Assign(item.renderMeshArray, item.blob);
            }

            OccluderMeshCache.Trim();
        }

        void SetFilter(RenderMeshArray renderMeshArray)
        {
            m_freeQuery.SetSharedComponentFilterManaged(renderMeshArray);
            m_ownedQuery.SetSharedComponentFilterManaged(renderMeshArray);
        }

        void ResetFilter()
        {
            m_freeQuery.ResetFilter();
            m_ownedQuery.ResetFilter();
        }

        /// <summary>
        /// Gives up entities whose occluder something else has replaced since this system set it.
        /// </summary>
        void ReleaseReplacedOccluders()
        {
            if (m_ownedWithOccluderQuery.IsEmptyIgnoreFilter)
                return;
            var entities  = m_ownedWithOccluderQuery.ToEntityArray(Allocator.Temp);
            var occluders = m_ownedWithOccluderQuery.ToComponentDataArray<PeekabooOccluder>(Allocator.Temp);
            var marks     = m_ownedWithOccluderQuery.ToComponentDataArray<PeekabooBakedOccluder>(Allocator.Temp);
            var replaced  = new NativeList<Entity>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (occluders[i].blob != marks[i].blob)
                    replaced.Add(entities[i]);
            }
            if (!replaced.IsEmpty)
                EntityManager.RemoveComponent<PeekabooBakedOccluder>(replaced.AsArray());
        }

        /// <summary>
        /// Gives every qualifying entity of the array the blob, or takes this system's occluders away
        /// if the array has no plates.
        /// </summary>
        void Assign(RenderMeshArray renderMeshArray, BlobAssetReference<PeekabooOccluderBlob> blob)
        {
            SetFilter(renderMeshArray);
            if (!blob.IsCreated)
            {
                EntityManager.RemoveComponent(m_ownedQuery, m_ownedTypes);
                ResetFilter();
                return;
            }

            // Newly added components start out with a null blob, so the check below sets them too.
            EntityManager.AddComponent(m_freeQuery, m_ownedTypes);

            var  marks      = m_ownedQuery.ToComponentDataArray<PeekabooBakedOccluder>(Allocator.Temp);
            bool upToDate   = true;
            for (int i = 0; i < marks.Length && upToDate; i++)
                upToDate = marks[i].blob == blob;
            if (!upToDate)
            {
                var occluders = new NativeArray<PeekabooOccluder>(marks.Length, Allocator.Temp);
                for (int i = 0; i < marks.Length; i++)
                {
                    marks[i]     = new PeekabooBakedOccluder { blob = blob };
                    occluders[i] = new PeekabooOccluder { blob = blob };
                }
                m_ownedQuery.CopyFromComponentDataArray(occluders);
                m_ownedQuery.CopyFromComponentDataArray(marks);
            }
            ResetFilter();
        }

        /// <summary>
        /// Marks which meshes in the array some entity draws. A mesh nothing references costs no bake
        /// time.
        /// </summary>
        static void MarkUsedMeshes(RenderMeshArray renderMeshArray, EntityQuery query, bool[] used)
        {
            var indices = renderMeshArray.MaterialMeshIndices;
            var mmis    = query.ToComponentDataArray<MaterialMeshInfo>(Allocator.Temp);
            for (int i = 0; i < mmis.Length; i++)
            {
                var mmi = mmis[i];
                // A range can still carry a mesh in Mesh, as the override for OverrideMeshInRangeTag.
                if (mmi.Mesh < 0)
                    MarkUsed(used, MaterialMeshInfo.StaticIndexToArrayIndex(mmi.Mesh));
                if (!mmi.HasMaterialMeshIndexRange || indices == null)
                    continue;
                var range = mmi.MaterialMeshIndexRange;
                for (int r = 0; r < range.length; r++)
                {
                    int index = range.start + r;
                    if (index >= 0 && index < indices.Length)
                        MarkUsed(used, indices[index].MeshIndex);
                }
            }
        }

        static void MarkUsed(bool[] used, int meshIndex)
        {
            if (meshIndex >= 0 && meshIndex < used.Length)
                used[meshIndex] = true;
        }

        /// <summary>
        /// Finds the cache key of every used mesh. Asset meshes are keyed without reading them. The
        /// rest get their contents hashed together in one job.
        /// </summary>
        void ComputeMeshKeys(List<ArrayWork> work)
        {
            m_meshKeys.Clear();
            m_meshesToRead.Clear();
            foreach (var item in work)
            {
                var meshes = item.renderMeshArray.MeshReferences;
                for (int i = 0; i < meshes.Length; i++)
                {
                    var mesh = item.used[i] ? meshes[i].Value : null;
                    if (mesh == null || m_meshKeys.ContainsKey(mesh))
                        continue;
                    if (OccluderMeshCache.TryGetAssetKey(mesh, out var key))
                        m_meshKeys.Add(mesh, key);
                    else
                    {
                        m_meshKeys.Add(mesh, default);
                        m_meshesToRead.Add(mesh);
                    }
                }
            }
            if (m_meshesToRead.Count == 0)
                return;

            var meshData = AcquireMeshData(m_meshesToRead);
            var hashes   = new NativeArray<Hash128>(m_meshesToRead.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            new HashMeshesJob { meshes = meshData, hashes = hashes }.Schedule(m_meshesToRead.Count, 1).Complete();
            meshData.Dispose();
            for (int i = 0; i < m_meshesToRead.Count; i++)
                m_meshKeys[m_meshesToRead[i]] = hashes[i];
            hashes.Dispose();
        }

        /// <summary>
        /// Builds plates for every used mesh the cache doesn't have yet, in one parallel job.
        /// </summary>
        void BuildMissingMeshes()
        {
            m_meshesToRead.Clear();
            m_keysToBuild.Clear();
            m_keysQueued.Clear();
            foreach (var pair in m_meshKeys)
            {
                if (OccluderMeshCache.TryGet(pair.Value, out _) || !m_keysQueued.Add(pair.Value))
                    continue;
                m_meshesToRead.Add(pair.Key);
                m_keysToBuild.Add(pair.Value);
            }
            int count = m_meshesToRead.Count;
            meshesBuiltLastUpdate = count;
            if (count == 0)
                return;

            var meshData    = AcquireMeshData(m_meshesToRead);
            var plateLists  = new NativeArray<UnsafeList<OccluderPlate> >(count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            var vertexLists = new NativeArray<UnsafeList<float3> >(count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            var summaries   = new NativeArray<OccluderPlateBaker.Result>(count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            new BuildPlatesJob
            {
                meshes    = meshData,
                plates    = plateLists,
                vertices  = vertexLists,
                summaries = summaries,
            }.Schedule(count, 1).Complete();
            meshData.Dispose();

            for (int i = 0; i < count; i++)
            {
                var plateList  = plateLists[i];
                var vertexList = vertexLists[i];
                OccluderMeshCache.Add(m_keysToBuild[i], summaries[i], in plateList, in vertexList);
                plateList.Dispose();
                vertexList.Dispose();
            }
            plateLists.Dispose();
            vertexLists.Dispose();
            summaries.Dispose();
        }

        static Mesh.MeshDataArray AcquireMeshData(List<Mesh> meshes)
        {
#if UNITY_EDITOR
            return UnityEditor.MeshUtility.AcquireReadOnlyMeshData(meshes);
#else
            return Mesh.AcquireReadOnlyMeshData(meshes);
#endif
        }

        /// <summary>
        /// A key for the blob an array bakes to: which meshes are used and what they hold, and which
        /// materials are opaque. Arrays that bake to the same blob share it.
        /// </summary>
        unsafe Hash128 ComputeArrayKey(RenderMeshArray renderMeshArray, bool[] used, out NativeArray<bool> opaqueMaterials)
        {
            var meshes    = renderMeshArray.MeshReferences;
            var materials = renderMeshArray.MaterialReferences;
            opaqueMaterials = new NativeArray<bool>(materials == null ? 0 : materials.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < opaqueMaterials.Length; i++)
                opaqueMaterials[i] = IsOpaque(materials[i].Value);

            var  state   = new xxHash3.StreamingState(false, 0x7eeba800u);
            uint version = kBlobKeyVersion;
            int  counts  = meshes.Length;
            state.Update(&version, sizeof(uint));
            state.Update(&counts, sizeof(int));
            for (int i = 0; i < meshes.Length; i++)
            {
                var mesh = used[i] ? meshes[i].Value : null;
                var key  = mesh != null ? m_meshKeys[mesh] : default;
                state.Update(&key, UnsafeUtility.SizeOf<Hash128>());
            }
            counts = opaqueMaterials.Length;
            state.Update(&counts, sizeof(int));
            if (counts > 0)
                state.Update(opaqueMaterials.GetUnsafeReadOnlyPtr(), counts * sizeof(bool));
            return new Hash128(state.DigestHash128());
        }

        /// <summary>
        /// Whether a material fully hides what's behind it. It has to be opaque, write depth, not clip
        /// alpha, and draw front faces.
        /// </summary>
        /// <remarks>
        /// Material property overrides can't change any of this, so the material asset is the source of
        /// truth. A project that edits a material asset at runtime is responsible for adding
        /// PeekabooDisableOccluderTag to the renderers using it.
        /// </remarks>
        static bool IsOpaque(Material material)
        {
            if (material == null)
                return false;
            if (material.renderQueue > (int)RenderQueue.GeometryLast)
                return false;
            if (material.IsKeywordEnabled("_ALPHATEST_ON") || material.IsKeywordEnabled("_ALPHABLEND_ON") || material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"))
                return false;
            if (material.HasFloat("_AlphaClip") && material.GetFloat("_AlphaClip") > 0.5f)
                return false;
            if (material.HasFloat("_Surface") && material.GetFloat("_Surface") > 0.5f)
                return false;
            if (material.HasFloat("_ZWrite") && material.GetFloat("_ZWrite") < 0.5f)
                return false;
            // The outward faces are what hide things, so a material that culls them hides nothing.
            if (material.HasFloat("_Cull") && (int)material.GetFloat("_Cull") == (int)CullMode.Front)
                return false;
            return true;
        }

        /// <summary>
        /// Lays out the cached plates of the array's used meshes as one blob. Returns a null blob if
        /// none of them have plates.
        /// </summary>
        BlobAssetReference<PeekabooOccluderBlob> AssembleBlob(RenderMeshArray renderMeshArray, bool[] used, NativeArray<bool> opaqueMaterials)
        {
            var meshRefs      = renderMeshArray.MeshReferences;
            var entries       = new OccluderMeshCache.Entry[meshRefs.Length];
            int totalPlates   = 0;
            int totalVertices = 0;
            for (int i = 0; i < meshRefs.Length; i++)
            {
                var mesh = used[i] ? meshRefs[i].Value : null;
                if (mesh == null || !OccluderMeshCache.TryGet(m_meshKeys[mesh], out var entry))
                    continue;
                entries[i]     = entry;
                totalPlates   += entry.plates.Length;
                totalVertices += entry.vertices.Length;
            }
            if (totalPlates == 0)
                return default;

            var meshes   = new NativeArray<OccluderMesh>(meshRefs.Length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            var plates   = new NativeArray<OccluderPlate>(totalPlates, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var vertices = new NativeArray<float3>(totalVertices, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            int plateCursor  = 0;
            int vertexCursor = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry == null)
                    continue;
                var mesh        = entry.mesh;
                mesh.plateStart = plateCursor;
                meshes[i]       = mesh;
                for (int p = 0; p < entry.plates.Length; p++)
                {
                    var plate          = entry.plates[p];
                    plate.vertexStart += vertexCursor;
                    plates[plateCursor++] = plate;
                }
                NativeArray<float3>.Copy(entry.vertices, 0, vertices, vertexCursor, entry.vertices.Length);
                vertexCursor += entry.vertices.Length;
            }

            var result = new NativeReference<BlobAssetReference<PeekabooOccluderBlob> >(Allocator.TempJob);
            new AssembleBlobJob
            {
                meshes          = meshes,
                plates          = plates,
                vertices        = vertices,
                opaqueMaterials = opaqueMaterials,
                result          = result,
            }.Run();
            var blob = result.Value;
            result.Dispose();
            meshes.Dispose();
            plates.Dispose();
            vertices.Dispose();
            return blob;
        }

        [BurstCompile]
        struct HashMeshesJob : IJobParallelFor
        {
            [ReadOnly] public Mesh.MeshDataArray meshes;
            public NativeArray<Hash128>          hashes;

            public void Execute(int index) => hashes[index] = OccluderPlateBaker.HashInputs(meshes[index]);
        }

        [BurstCompile]
        struct BuildPlatesJob : IJobParallelFor
        {
            [ReadOnly] public Mesh.MeshDataArray meshes;

            public NativeArray<UnsafeList<OccluderPlate> > plates;
            public NativeArray<UnsafeList<float3> >        vertices;
            public NativeArray<OccluderPlateBaker.Result>  summaries;

            public void Execute(int index)
            {
                var plateList  = new UnsafeList<OccluderPlate>(OccluderPlateBaker.kMaxPlatesPerMesh, Allocator.Persistent);
                var vertexList = new UnsafeList<float3>(OccluderPlateBaker.kMaxPlatesPerMesh * 4, Allocator.Persistent);
                var summary    = OccluderPlateBaker.Build(meshes[index], ref plateList, ref vertexList);
                summaries[index] = summary;
                plates[index]    = plateList;
                vertices[index]  = vertexList;
            }
        }

        [BurstCompile]
        struct AssembleBlobJob : IJob
        {
            [ReadOnly] public NativeArray<OccluderMesh>  meshes;
            [ReadOnly] public NativeArray<OccluderPlate> plates;
            [ReadOnly] public NativeArray<float3>        vertices;
            [ReadOnly] public NativeArray<bool>          opaqueMaterials;

            public NativeReference<BlobAssetReference<PeekabooOccluderBlob> > result;

            public void Execute()
            {
                var     builder = new BlobBuilder(Allocator.Temp);
                ref var root    = ref builder.ConstructRoot<PeekabooOccluderBlob>();
                Copy(ref builder, ref root.meshes,          meshes);
                Copy(ref builder, ref root.plates,          plates);
                Copy(ref builder, ref root.vertices,        vertices);
                Copy(ref builder, ref root.opaqueMaterials, opaqueMaterials);
                result.Value = builder.CreateBlobAssetReference<PeekabooOccluderBlob>(Allocator.Persistent);
                builder.Dispose();
            }

            static void Copy<T>(ref BlobBuilder builder, ref BlobArray<T> target, NativeArray<T> source) where T : unmanaged
            {
                var array = builder.Allocate(ref target, source.Length);
                for (int i = 0; i < source.Length; i++)
                    array[i] = source[i];
            }
        }
    }
}
