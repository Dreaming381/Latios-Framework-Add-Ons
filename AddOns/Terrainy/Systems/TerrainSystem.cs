using Latios.Systems;
using Latios.Terrainy.Components;
using Latios.Transforms.Abstract;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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
		
			foreach (var terrainEntity in terrainEntities)
			{
				var detailCellElements = SystemAPI.GetBuffer<DetailCellElement>(terrainEntity);
				var detailInstanceElements = SystemAPI.GetBuffer<DetailsInstanceElement>(terrainEntity);
				var createdDetails = new NativeList<Entity>((int)(detailCellElements.Length * 1.5f), Allocator.Temp);

				foreach (var detailCellElement in detailCellElements)
				{
					DetailsInstanceElement correspondingInstance = detailInstanceElements[detailCellElement.PrototypeIndex];
					// Todo: Create decoration entities for textures.
					if (correspondingInstance.UseMesh == 0) continue;
					for (var index = 0; index < detailCellElement.Count; index++)
					{
						Entity instance = entityManager.Instantiate(correspondingInstance.Prefab);
						createdDetails.Add(instance);
						// Position the instance
						#if LATIOS_TRANSFORMS_UNITY
						var wt = SystemAPI.GetComponent<LocalToWorld>(terrainEntity);
						if (!entityManager.HasComponent<LocalTransform>(instance)) continue;
						var lt = entityManager.GetComponentData<LocalTransform>(instance);
						#else
						// TODO make it work with qvvs
						var wt = SystemAPI.GetComponent<WorldTransform>();
						var lt;
						#endif
						// Local cell-space placement (XZ plane), 1 unit per cell
						const float cellSize = 1.0f;

						// Stable hash for per-instance randomness
						int2 cords = detailCellElement.Coord;
						
						uint seed = math.hash(new uint3((uint)cords.x, (uint)cords.y, (uint)index));
						var random = new Random(seed);

						// Random jitter within the cell
						float jx = random.NextFloat(0, 1);
						float jz = random.NextFloat(0, 1);

						// Random yaw
						float yaw = random.NextFloat(0, 1) * (2f * math.PI);

						// Random size between min/max
						float2 minSize = correspondingInstance.MinSize;
						float2 maxSize = correspondingInstance.MaxSize;
						float rw = math.lerp(minSize.x * lt.Scale, maxSize.x * lt.Scale, random.NextFloat(0, 1));
						float rh = math.lerp(minSize.y * lt.Scale, maxSize.y * lt.Scale, random.NextFloat(0, 1));

						// Choose a uniform scale representative (mesh: height-driven, else width-driven)
						float uniformScale = correspondingInstance.UseMesh != 0 ? rh : rw;

						// Compose local position inside terrain local space
						var localPos = new float3(
							(cords.x + jx) * cellSize,
							0f,
							(cords.y + jz) * cellSize
						);

						// Transform to world using terrain's LocalToWorld
						float3 worldPos = math.transform(wt.Value, localPos);

						// Build final transform
						lt.Position = worldPos;
						lt.Rotation = quaternion.AxisAngle(math.up(), yaw);
						lt.Scale = uniformScale;
						entityManager.SetComponentData(instance, lt);

					}
				}


				var decorationsGroupEntity = state.EntityManager.CreateEntity();
				var leg = state.EntityManager.AddBuffer<LinkedEntityGroup>(decorationsGroupEntity).Reinterpret<Entity>();
				// Todo: Assign decoration entities to leg
				leg.AddRange(createdDetails.AsArray());

				var terrainComp = state.EntityManager.GetComponentData<TerrainComponent>(terrainEntity);
				terrainComp.DecorationsGroupEntity = decorationsGroupEntity;
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
	}
}