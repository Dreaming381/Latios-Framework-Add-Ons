using Latios.Psyshock;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    public struct RigidBody : IComponentData
    {
        public UnitySim.Velocity velocity;
        public float             inverseMass;
        public half              coefficientOfFriction;
        public half              coefficientOfRestitution;
    }


    [InternalBufferCapacity(0)]
    public struct AddImpulse : IBufferElementData
    {
        internal float3 pointOrAxis;
        internal float3 impulse;

        public AddImpulse(float3 fieldImpulse)
        {
            pointOrAxis = float.NaN;
            impulse     = fieldImpulse;
        }

        public AddImpulse(float3 worldPoint, float3 impulse)
        {
            pointOrAxis  = worldPoint;
            this.impulse = impulse;
        }

        public AddImpulse(float3 worldAxis, float angularImpulse)
        {
            pointOrAxis = worldAxis;
            impulse.x   = angularImpulse;
            impulse.y   = float.NaN;
            impulse.z   = float.NaN;
        }
    }

    public struct LocalCenterOfMassOverride : IComponentData
    {
        public float3 centerOfMass;
    }

    public struct LocalInertiaOverride : IComponentData
    {
        public UnitySim.LocalInertiaTensorDiagonal inertiaDiagonal;
    }

    public struct GravityOverride : IComponentData
    {
        public float3 gravity;
    }
}
