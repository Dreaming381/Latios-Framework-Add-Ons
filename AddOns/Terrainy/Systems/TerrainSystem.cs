using Latios.Systems;
using Latios.Terrainy.Components;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
			var commandBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);
		
			foreach (var terrainEntity in terrainEntities)
			{
				var detailCellElements = SystemAPI.GetBuffer<DetailCellElement>(terrainEntity);
				var detailInstanceElements = SystemAPI.GetBuffer<DetailsInstanceElement>(terrainEntity);
				var treeInstances = SystemAPI.GetBuffer<TreeInstanceElement>(terrainEntity);
				var createdDetails = new NativeList<Entity>(detailCellElements.Length + treeInstances.Length, Allocator.Temp);
#if LATIOS_TRANSFORMS_UNITY
				// TODO make this work with qvvs
				var wt = SystemAPI.GetComponent<LocalToWorld>(terrainEntity);
#endif

				foreach (var detailCellElement in detailCellElements)
				{
						DetailsInstanceElement correspondingInstance = detailInstanceElements[detailCellElement.PrototypeIndex];
						Entity instance = entityManager.Instantiate(correspondingInstance.Prefab);
						createdDetails.Add(instance);
						
						float3 worldPos = detailCellElement.Coord;
						//Debug.Log($"X: {cords.x}, Y: {cords.y}, Z: {cords.z}");
						
						// Build final transform
						quaternion rotation = new quaternion();
						if(correspondingInstance.RenderMode != DetailRenderMode.GrassBillboard) {
							rotation = quaternion.RotateY(detailCellElement.RotationY);
						}
						var scale =  detailCellElement.Scale.x;
						wt.Value = float4x4.TRS(worldPos, rotation, scale);
						commandBuffer.SetComponent(instance, wt);
				}

				var treePrototypes = SystemAPI.GetBuffer<TreePrototypeElement>(terrainEntity);
				foreach (var tree in treeInstances)
				{
					TreePrototypeElement correspondingInstance = treePrototypes[tree.PrototypeIndex];
					Entity instance = entityManager.Instantiate(correspondingInstance.Prefab);
					createdDetails.Add(instance);
					var lt = entityManager.GetComponentData<LocalTransform>(instance);
					
					
					float3 worldPos = tree.Position;
						
					// Build final transform
					quaternion rotation = PackedRotationToQuaternion(tree.PackedRotation);
					var scale = tree.Scale;
					//wt.Value = float4x4.TRS(worldPos, rotation, new float3(scale.x, scale.x, scale.y));
					lt.Position  = worldPos;
					//lt.Rotation = rotation;
					//commandBuffer.SetComponent(instance, wt);
					commandBuffer.SetComponent(instance, lt);
				}


				var decorationsGroupEntity = state.EntityManager.CreateEntity();
				var leg = state.EntityManager.AddBuffer<LinkedEntityGroup>(decorationsGroupEntity).Reinterpret<Entity>();
				leg.AddRange(createdDetails.AsArray());

				var terrainComp = state.EntityManager.GetComponentData<TerrainComponent>(terrainEntity);
				terrainComp.DecorationsGroupEntity = decorationsGroupEntity;
				commandBuffer.Playback(entityManager);
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
		
		private static float DecodeRotation(ushort packedRotation)
		{
			return packedRotation * (2f * math.PI / 65535f);
		}

		private static quaternion PackedRotationToQuaternion(ushort packedRotation)
		{
			float radians = DecodeRotation(packedRotation);
			return quaternion.RotateY(radians);
		}
	}
	
	
}