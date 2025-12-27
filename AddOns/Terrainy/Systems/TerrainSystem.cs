using Latios.Systems;
using Latios.Terrainy.Commands;
using Latios.Terrainy.Components;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if LATIOS_TRANSFORMS_UNITY
using LocalTransform = Unity.Transforms.LocalTransform;
using Unity.Transforms;
#else
using Latios.Transforms;
#endif


namespace Latios.Terrainy.Systems
{
	[UpdateInGroup(typeof(LatiosWorldSyncGroup))]
	[RequireMatchingQueriesForUpdate]
	[DisableAutoCreation]
	public partial struct TerrainSystem : ISystem
	{
		LatiosWorldUnmanaged _latiosWorld;

		UnsafeHashSet<UnityObjectRef<Terrain>> _terrainsToDestroyOnShutdown;

		public void OnCreate(ref SystemState state)
		{
			this._latiosWorld = state.GetLatiosWorldUnmanaged();

			this._terrainsToDestroyOnShutdown = new UnsafeHashSet<UnityObjectRef<Terrain>>(32, Allocator.Persistent);
		}

		public void OnDestroy(ref SystemState state)
		{
			foreach (var terrain in this._terrainsToDestroyOnShutdown)
			{
				terrain.Value.gameObject.DestroySafelyFromAnywhere();
			}
			this._terrainsToDestroyOnShutdown.Dispose();
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
					var go = new GameObject("DOTS Terrain");
					go.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
					var terrain = go.AddComponent<Terrain>();
					terrain.drawTreesAndFoliage = false;
					this._terrainsToDestroyOnShutdown.Add(terrain);

					var td = state.EntityManager.GetComponentData<TerrainDataComponent>(entity);
					terrain.terrainData = td.TerrainData;
					terrain.materialTemplate = td.TerrainMat;

					var wt = state.EntityManager.GetAspect<WorldTransformReadOnlyAspect>(entity).worldTransformQvvs;
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
						this._terrainsToDestroyOnShutdown.Remove(terrainComp.Terrain);
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
				request.Terrain.Value.gameObject.SetActive(request.DesiredEnabledState);
			}
		}

		[BurstCompile]
		void DoCreateVegetationAndDetailEntities(ref SystemState state, ref TerrainSystem thisSystem, NativeArray<Entity> terrainEntities)
		{
			thisSystem.CreateVegetationAndDetailEntities(ref state, terrainEntities);
		}

		void CreateVegetationAndDetailEntities(ref SystemState state, NativeArray<Entity> terrainEntities)
		{
			var entityManager = state.EntityManager;
#if LATIOS_TRANSFORMS_UNITY
			var commandBuffer = new InstantiateCommandBufferCommand1<LocalToWorld, TerrainInstantiationCommand>(state.WorldUpdateAllocator);
			var commandBufferLocal = new InstantiateCommandBufferCommand1<LocalTransform, TerrainInstantiationCommand>(state.WorldUpdateAllocator);
#else
			var commandBuffer = new InstantiateCommandBufferCommand1<WorldTransform, TerrainInstantiationCommand>(state.WorldUpdateAllocator);
#endif
			foreach (var terrainEntity in terrainEntities)
			{
				var detailCellElements = SystemAPI.GetBuffer<DetailCellElement>(terrainEntity);
				var detailInstanceElements = SystemAPI.GetBuffer<DetailsInstanceElement>(terrainEntity);
				var treeInstances = SystemAPI.GetBuffer<TreeInstanceElement>(terrainEntity);
#if LATIOS_TRANSFORMS_UNITY
				var wt = SystemAPI.GetComponent<LocalToWorld>(terrainEntity); 
				CreateDetailAndTreeInstances(ref state, ref detailCellElements, ref detailInstanceElements, ref entityManager, wt, ref commandBuffer, ref commandBufferLocal, terrainEntity, ref treeInstances);
				commandBuffer.Playback(entityManager);
				commandBufferLocal.Playback(entityManager);
#else
				var wt = SystemAPI.GetComponent<WorldTransform>(terrainEntity);
				CreateDetailAndTreeInstances(ref state, ref detailCellElements, ref detailInstanceElements, ref entityManager, wt, ref commandBuffer, terrainEntity, ref treeInstances);
				commandBuffer.Playback(entityManager);
#endif

			}
		}

		#if LATIOS_TRANSFORMS_UNITY
		void CreateDetailAndTreeInstances(ref SystemState state, ref DynamicBuffer<DetailCellElement> detailCellElements, ref DynamicBuffer<DetailsInstanceElement> detailInstanceElements, ref EntityManager entityManager, LocalToWorld wt, ref InstantiateCommandBufferCommand1<LocalToWorld, TerrainInstantiationCommand> commandBuffer, ref InstantiateCommandBufferCommand1<LocalTransform, TerrainInstantiationCommand> commandBufferLocal, Entity terrainEntity, ref DynamicBuffer<TreeInstanceElement> treeInstances)
		{
			var terrainInstantiationCommand = new TerrainInstantiationCommand
			{
				terrainEntity = terrainEntity
			};
			if(!detailInstanceElements.IsEmpty) {
				foreach (DetailCellElement detailCellElement in detailCellElements)
				{
					DetailsInstanceElement correspondingInstance = detailInstanceElements[detailCellElement.PrototypeIndex];

					float3 worldPos = detailCellElement.Coord;

					var rotation = new quaternion();
					if (correspondingInstance.RenderMode != DetailRenderMode.GrassBillboard)
					{
						rotation = quaternion.RotateY(detailCellElement.RotationY);
					}
					var scale = detailCellElement.Scale.x;
					wt.Value = float4x4.TRS(worldPos, rotation, scale);
					commandBuffer.Add(correspondingInstance.Prefab, wt, terrainInstantiationCommand);
				}
			}
			var treePrototypes = SystemAPI.GetBuffer<TreePrototypeElement>(terrainEntity);
			if(!treePrototypes.IsEmpty) {
				foreach (TreeInstanceElement tree in treeInstances)
				{
					TreePrototypeElement correspondingInstance = treePrototypes[tree.PrototypeIndex];
					
					var lt = entityManager.GetComponentData<LocalTransform>(correspondingInstance.Prefab);
						
					float3 worldPos = tree.Position;

					quaternion rotation = quaternion.RotateY(tree.Rotation);
					var scale = tree.Scale;
					lt.Position = worldPos;
					lt.Scale *= scale.x;
					lt.Rotation = math.mul(rotation, lt.Rotation);
					commandBufferLocal.Add(correspondingInstance.Prefab, lt, terrainInstantiationCommand);
				}
			}
		}
#else
		void CreateDetailAndTreeInstances(ref SystemState state, ref DynamicBuffer<DetailCellElement> detailCellElements, ref DynamicBuffer<DetailsInstanceElement> detailInstanceElements, ref EntityManager entityManager, WorldTransform wt, ref InstantiateCommandBufferCommand1<WorldTransform, TerrainInstantiationCommand> commandBuffer, Entity terrainEntity, ref DynamicBuffer<TreeInstanceElement> treeInstances)
		{
			var terrainInstantiationCommand = new TerrainInstantiationCommand
			{
				terrainEntity = terrainEntity
			};
			if (!detailInstanceElements.IsEmpty)
			{
				foreach (DetailCellElement detailCellElement in detailCellElements)
				{
					DetailsInstanceElement correspondingInstance = detailInstanceElements[detailCellElement.PrototypeIndex];

					float3 worldPos = detailCellElement.Coord;

					var rotation = new quaternion();
					if (correspondingInstance.RenderMode != DetailRenderMode.GrassBillboard)
					{
						rotation = quaternion.RotateY(detailCellElement.RotationY);
					}
					var scale = detailCellElement.Scale.x;
					wt.worldTransform = new TransformQvvs(worldPos, rotation, scale, 1f);
					commandBuffer.Add(correspondingInstance.Prefab, wt, terrainInstantiationCommand);
				}
			}
			var treePrototypes = SystemAPI.GetBuffer<TreePrototypeElement>(terrainEntity);
			if (!treePrototypes.IsEmpty)
			{
				var treeToWorld = new NativeHashMap<Entity, WorldTransform>(treePrototypes.Length, Allocator.Temp);
				foreach (TreeInstanceElement tree in treeInstances)
				{
					TreePrototypeElement correspondingInstance = treePrototypes[tree.PrototypeIndex];
					WorldTransform wtLocal;
					if (treeToWorld.ContainsKey(correspondingInstance.Prefab))
					{
						wtLocal = treeToWorld[correspondingInstance.Prefab];
					}
					else
					{
						wtLocal = entityManager.GetComponentData<WorldTransform>(correspondingInstance.Prefab);
						treeToWorld.Add(correspondingInstance.Prefab, wtLocal);
					}

					float3 worldPos = tree.Position;

					var wtLocalQvvs = wtLocal.worldTransform;
					quaternion rotation = quaternion.RotateY(tree.Rotation);
					var scale = tree.Scale;
					wtLocalQvvs.position = worldPos;
					wtLocalQvvs.scale *= scale.x;
					wtLocalQvvs.rotation = math.mul(rotation, wtLocalQvvs.rotation);
					wtLocal.worldTransform = wtLocalQvvs;
					commandBuffer.Add(correspondingInstance.Prefab, wtLocal, terrainInstantiationCommand);
				}
			}
		}
#endif

		[BurstCompile]
		static void DoDestroyVegetationAndDetailEntities(ref SystemState state, ref TerrainSystem thisSystem) => thisSystem.DestroyVegetationAndDetailEntities(ref state);

		void DestroyVegetationAndDetailEntities(ref SystemState state)
		{
			var dcb = new DestroyCommandBuffer(Allocator.Temp);
			foreach (var terrainComp in SystemAPI.Query<TerrainComponent>().WithNone<TerrainDataComponent>())
			{
				dcb.Add(terrainComp.DecorationsGroupEntity);
			}
			dcb.Playback(state.EntityManager);
		}

		struct ManagedTerrainEnableRequest
		{
			public UnityObjectRef<Terrain> Terrain;
			public bool DesiredEnabledState;
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
			var ecb = new EnableCommandBuffer(Allocator.Temp);
			var dcb = new DisableCommandBuffer(Allocator.Temp);

			// Todo: We would really like Order-Version filtering on this, but idiomatic foreach doesn't support that currently
			foreach (var (terrainComp, entity) in SystemAPI.Query<TerrainComponent>().WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
			{
				var hasDisabled = SystemAPI.HasComponent<Disabled>(entity);
				var hasLiveBaked = SystemAPI.HasComponent<TerrainLiveBakedTag>(entity);
				bool show = !hasDisabled && (!hasLiveBaked || showLiveBaked);
				if (!SystemAPI.HasComponent<Disabled>(terrainComp.DecorationsGroupEntity) != show)
				{
					enableRequestsList.Add(new ManagedTerrainEnableRequest
					{
						Terrain = terrainComp.Terrain,
						DesiredEnabledState = show
					});
					if (show)
						ecb.Add(terrainComp.DecorationsGroupEntity);
					else
						dcb.Add(terrainComp.DecorationsGroupEntity);

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