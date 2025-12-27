using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Components
{
    public struct TerrainComponent : ICleanupComponentData
    {
        public UnityObjectRef<Terrain> Terrain;
        // We make a separate entity to hold the LinkedEntityGroup for details and vegetation,
        // because we don't want changes to terrain visibility in the editor to affect any
        // children objects of the terrain.
        public EntityWithBuffer<LinkedEntityGroup> DecorationsGroupEntity;
    }
}

