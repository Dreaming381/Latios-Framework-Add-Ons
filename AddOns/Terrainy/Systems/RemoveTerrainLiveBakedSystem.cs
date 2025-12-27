using Latios.Terrainy.Components;
using Unity.Burst;
using Unity.Entities;

namespace Latios.Terrainy.Systems
{
	[DisableAutoCreation]
	[RequireMatchingQueriesForUpdate]
	[WorldSystemFilter(WorldSystemFilterFlags.EntitySceneOptimizations)]
	public partial struct RemoveTerrainLiveBakedSystem : ISystem
	{
		public void OnCreate(ref SystemState state) { }

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var query =
				SystemAPI.QueryBuilder().WithPresent<TerrainLiveBakedTag>().WithOptions(EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IncludePrefab).Build();
			state.EntityManager.RemoveComponent<TerrainLiveBakedTag>(query);
		}


		public void OnDestroy(ref SystemState state) { }
	}
}