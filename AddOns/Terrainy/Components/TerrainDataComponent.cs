using Latios.Terrainy.Systems;
using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Components
{
    public struct TerrainDataComponent : IComponentData
    {
        public UnityObjectRef<TerrainData> TerrainData;
        public UnityObjectRef<Material> TerrainMat;
    }
}
