using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Components
{
    public struct TerrainDataComponent : IComponentData
    {
        public UnityObjectRef<TerrainData> TerrainData;
    }
}
