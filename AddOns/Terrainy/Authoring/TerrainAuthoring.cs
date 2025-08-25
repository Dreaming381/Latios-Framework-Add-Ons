using Latios.Terrainy.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Authoring
{
    [DisableAutoCreation]
    public class TerrainAuthoring : Baker<Terrain>
    {
        public override void Bake(Terrain authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);

            TerrainData data = authoring.terrainData;
            DependsOn(data);

            // Modifying the heightmap in the editor does not cause TerrainData to propagate as a changed object and trigger a rebake.
            // Therefore, we need to use the same TerrainData for runtime that we use for authoring while in the editor, and can only
            // strip the trees and details for a build.
            if (!IsBakingForEditor())
            {
                data = Object.Instantiate(data);

                // Todo: This probably needs more testing and iteration.
                data.detailPrototypes = null;
                data.treeInstances    = new TreeInstance[0];
                data.treePrototypes   = null;
            }

            AddComponent(entity, new TerrainDataComponent
            {
                TerrainData = data,
                TerrainMat  = authoring.materialTemplate
            });
            AddComponent<TerrainLiveBakedTag>(entity);
        }
    }

    [DisableAutoCreation]
    [RequireMatchingQueriesForUpdate]
    [WorldSystemFilter(WorldSystemFilterFlags.EntitySceneOptimizations)]
    public partial struct RemoveTerrainLiveBakedSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var query =
                SystemAPI.QueryBuilder().WithPresent<TerrainLiveBakedTag>().WithOptions(EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IncludePrefab).Build();
            state.EntityManager.RemoveComponent<TerrainLiveBakedTag>(query);
        }
    }
}

