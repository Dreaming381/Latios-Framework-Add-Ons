using Latios.Terrainy.Components;
using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Authoring
{
	public class TerrainAuthoring : Baker<Terrain>
	{
		public override void Bake(Terrain authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Renderable);

			TerrainData src = authoring.terrainData;

			var clonedTerrainData = new TerrainData();
			clonedTerrainData.name = src.name;

#region Heights and size

			clonedTerrainData.heightmapResolution = src.heightmapResolution;
			clonedTerrainData.size = src.size;
			int heightmapResolution = src.heightmapResolution;
			float[,] heights = src.GetHeights(0, 0, heightmapResolution, heightmapResolution);
			clonedTerrainData.SetHeights(0, 0, heights);

#endregion Heights and size

#region Terrain layers, splatmaps (alphamaps) and holes

			clonedTerrainData.terrainLayers = src.terrainLayers;
			clonedTerrainData.alphamapResolution = src.alphamapResolution;
			int aWidth = src.alphamapWidth;
			int aHeight = src.alphamapHeight;
			float[,,] alphamaps = src.GetAlphamaps(0, 0, aWidth, aHeight);
			clonedTerrainData.SetAlphamaps(0, 0, alphamaps);
			int holesRes = src.holesResolution;
			if (holesRes > 0)
			{
				bool[,] holes = src.GetHoles(0, 0, holesRes, holesRes);
				clonedTerrainData.SetHoles(0, 0, holes);
			}

#endregion

			AddComponent(entity, new TerrainDataComponent
			{
				TerrainData = clonedTerrainData
			});
		}
	}
}