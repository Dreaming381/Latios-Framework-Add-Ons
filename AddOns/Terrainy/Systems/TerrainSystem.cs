using Latios.Systems;
using Latios.Terrainy.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Systems
{
	[UpdateInGroup(typeof(LatiosWorldSyncGroup))]
	[DisableAutoCreation]
	public partial class TerrainSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			// 1) Create: Entities that have TerrainDataComponent but no TerrainComponent yet.
			foreach (var (terrainDataComp, entity) in SystemAPI
				         .Query<RefRO<TerrainDataComponent>>()
				         .WithNone<TerrainComponent>()
				         .WithEntityAccess())
			{
				var go = new GameObject("DOTS Terrain");
				var terrain = go.AddComponent<Terrain>();

				var td = terrainDataComp.ValueRO;
				terrain.terrainData = td.TerrainData;
				terrain.materialTemplate = td.TerrainMat;

				ecb.AddComponent(entity, new TerrainComponent
				{
					Terrain = terrain
				});
			}
			
			// 2) Teardown: Entities that have a TerrainComponent but no TerrainDataComponent.
			foreach (var (terrainComp, entity) in SystemAPI
				         .Query<RefRO<TerrainComponent>>()
				         .WithNone<TerrainDataComponent>()
				         .WithEntityAccess())
			{
				if (terrainComp.ValueRO.Terrain.Value != null)
				{
					// Destroy the GameObject that hosts the Terrain
					Object.Destroy(terrainComp.ValueRO.Terrain.Value);
				}
				
				ecb.RemoveComponent<TerrainComponent>(entity);
			}

			ecb.Playback(EntityManager);
			ecb.Dispose();

		}
	}
}