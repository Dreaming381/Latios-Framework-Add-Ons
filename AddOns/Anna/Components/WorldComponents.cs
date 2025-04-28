using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna
{
    public struct PhysicsSettings : IComponentData
    {
        public CollisionLayerSettings collisionLayerSettings;
        public float3                 gravity;
        public half                   linearDamping;
        public half                   angularDamping;
        public byte                   numIterations;
    }

    public struct EnvironmentCollisionTag : IComponentData { }

    public struct KinematicCollisionTag : IComponentData { }
}

