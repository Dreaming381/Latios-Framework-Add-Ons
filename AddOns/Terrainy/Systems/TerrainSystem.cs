using Latios.Systems;
using Latios.Terrainy.Components;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Systems
{
    [UpdateInGroup(typeof(LatiosWorldSyncGroup))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct TerrainSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;

        UnsafeHashSet<UnityObjectRef<Terrain> > terrainsToDestroyOnShutdown;

        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();

            terrainsToDestroyOnShutdown = new UnsafeHashSet<UnityObjectRef<Terrain> >(32, Allocator.Persistent);
        }

        public void OnDestroy(ref SystemState state)
        {
            foreach (var terrain in  terrainsToDestroyOnShutdown)
            {
                terrain.Value.gameObject.DestroySafelyFromAnywhere();
            }
            terrainsToDestroyOnShutdown.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            // 1) Create: Entities that have TerrainDataComponent but no TerrainComponent yet.
            var newQuery = SystemAPI.QueryBuilder().WithPresent<TerrainDataComponent>().WithAbsent<TerrainComponent>().Build();
            if (!newQuery.IsEmptyIgnoreFilter)
            {
                var newEntities = newQuery.ToEntityArray(Allocator.Temp);
                state.EntityManager.AddComponent(newQuery, new TypePack<TerrainComponent, LinkedEntityGroup>());
                foreach (var entity in newEntities)
                {
                    var go                      = new GameObject("DOTS Terrain");
                    go.hideFlags                = HideFlags.NotEditable | HideFlags.DontSave;
                    var terrain                 = go.AddComponent<Terrain>();
                    terrain.drawTreesAndFoliage = false;
                    terrainsToDestroyOnShutdown.Add(terrain);

                    var td                   = state.EntityManager.GetComponentData<TerrainDataComponent>(entity);
                    terrain.terrainData      = td.TerrainData;
                    terrain.materialTemplate = td.TerrainMat;

                    var wt                       = state.EntityManager.GetAspect<WorldTransformReadOnlyAspect>(entity).worldTransformQvvs;
                    terrain.transform.localScale = wt.scale * wt.stretch;
                    terrain.transform.SetPositionAndRotation(wt.position, wt.rotation);

                    state.EntityManager.SetComponentData(entity, new TerrainComponent
                    {
                        Terrain = terrain,
                    });
                }

                // 2) Instantiate decorations for new terrain entities.
                DoCreateVegetationAndDetailEntities(ref state, ref this, newEntities);
            }

            // 3) Teardown: Entities that have a TerrainComponent but no TerrainDataComponent.
            var deadQuery = SystemAPI.QueryBuilder().WithPresent<TerrainComponent>().WithAbsent<TerrainDataComponent>().Build();
            if (!deadQuery.IsEmptyIgnoreFilter)
            {
                foreach (var terrainComp in SystemAPI.Query<TerrainComponent>().WithNone<TerrainDataComponent>())
                {
                    if (terrainComp.Terrain.Value != null)
                    {
                        // Destroy the GameObject that hosts the Terrain
                        terrainsToDestroyOnShutdown.Remove(terrainComp.Terrain);
                        terrainComp.Terrain.Value.gameObject.DestroySafelyFromAnywhere();
                    }
                }
                state.EntityManager.RemoveComponent<TerrainComponent>(deadQuery);
                DoDestroyVegetationAndDetailEntities(ref state, ref this);
            }

            // 4) Toggle terrain and vegetation enabled states based on terrain entity state and scene view mode
            bool isAuthoringSceneViewOutsideOfPlayMode = false;
#if UNITY_EDITOR
            isAuthoringSceneViewOutsideOfPlayMode |= !UnityEditor.EditorApplication.isPlaying &&
                                                     Unity.Scenes.Editor.LiveConversionEditorSettings.LiveConversionMode == Unity.Scenes.LiveConversionMode.SceneViewShowsAuthoring;
#endif
            DoUpdateTerrainEnabledStates(ref state, ref this, !isAuthoringSceneViewOutsideOfPlayMode, out var enableRequests);
            foreach (var request in enableRequests)
            {
                request.terrain.Value.gameObject.SetActive(request.desiredEnabledState);
            }
        }

        [BurstCompile]
        void DoCreateVegetationAndDetailEntities(ref SystemState state, ref TerrainSystem thisSystem, NativeArray<Entity> terrainEntities)
        {
            thisSystem.CreateVegetationAndDetailEntities(ref state, terrainEntities);
        }

        void CreateVegetationAndDetailEntities(ref SystemState state, NativeArray<Entity> terrainEntities)
        {
            foreach (var terrainEntity in terrainEntities)
            {
                // Todo: Create decoration entities.

                var decorationsGroupEntity = state.EntityManager.CreateEntity();
                var leg                    = state.EntityManager.AddBuffer<LinkedEntityGroup>(decorationsGroupEntity).Reinterpret<Entity>();
                // Todo: Assign decoration entities to leg

                var terrainComp                    = state.EntityManager.GetComponentData<TerrainComponent>(terrainEntity);
                terrainComp.decorationsGroupEntity = decorationsGroupEntity;
                state.EntityManager.SetComponentData(terrainEntity, terrainComp);
            }
        }

        [BurstCompile]
        static void DoDestroyVegetationAndDetailEntities(ref SystemState state, ref TerrainSystem thisSystem) => thisSystem.DestroyVegetationAndDetailEntities(ref state);

        void DestroyVegetationAndDetailEntities(ref SystemState state)
        {
            var dcb = new DestroyCommandBuffer(Allocator.Temp);
            foreach (var terrainComp in SystemAPI.Query<TerrainComponent>().WithNone<TerrainDataComponent>())
            {
                dcb.Add(terrainComp.decorationsGroupEntity);
            }
            dcb.Playback(state.EntityManager);
        }

        struct ManagedTerrainEnableRequest
        {
            public UnityObjectRef<Terrain> terrain;
            public bool                    desiredEnabledState;
        }

        [BurstCompile]
        static void DoUpdateTerrainEnabledStates(ref SystemState state,
                                                 ref TerrainSystem thisSystem,
                                                 bool showLiveBaked,
                                                 out NativeArray<ManagedTerrainEnableRequest> enableRequests)
        {
            thisSystem.UpdateTerrainEnabledStates(ref state, showLiveBaked, out enableRequests);
        }

        void UpdateTerrainEnabledStates(ref SystemState state, bool showLiveBaked, out NativeArray<ManagedTerrainEnableRequest> enableRequests)
        {
            var enableRequestsList = new NativeList<ManagedTerrainEnableRequest>(Allocator.Temp);
            var ecb                = new EnableCommandBuffer(Allocator.Temp);
            var dcb                = new DisableCommandBuffer(Allocator.Temp);

            // Todo: We would really like Order-Version filtering on this, but idiomatic foreach doesn't support that currently
            foreach (var (terrainComp, entity) in SystemAPI.Query<TerrainComponent>().WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {
                var  hasDisabled  = SystemAPI.HasComponent<Disabled>(entity);
                var  hasLiveBaked = SystemAPI.HasComponent<TerrainLiveBakedTag>(entity);
                bool show         = !hasDisabled && (!hasLiveBaked || showLiveBaked);
                if (!SystemAPI.HasComponent<Disabled>(terrainComp.decorationsGroupEntity) != show)
                {
                    if (show)
                        ecb.Add(terrainComp.decorationsGroupEntity);
                    else
                        dcb.Add(terrainComp.decorationsGroupEntity);
                    enableRequestsList.Add(new ManagedTerrainEnableRequest
                    {
                        terrain             = terrainComp.Terrain,
                        desiredEnabledState = show
                    });
                }
            }

            if (!enableRequestsList.IsEmpty)
            {
                dcb.Playback(state.EntityManager, SystemAPI.GetBufferLookup<LinkedEntityGroup>(true));
                ecb.Playback(state.EntityManager, SystemAPI.GetBufferLookup<LinkedEntityGroup>(true));
            }

            enableRequests = enableRequestsList.AsArray();
        }
    }
}

