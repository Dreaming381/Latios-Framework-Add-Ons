using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Components
{
	public struct TerrainComponent : ICleanupComponentData
	{
		public UnityObjectRef<Terrain> Terrain;
	}
}