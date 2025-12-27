using Unity.Entities;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    public static partial class FlowField
    {
        public struct Goal : IComponentData
        {
            public int2 Size;
        }

        public struct AgentDirection : IComponentData
        {
            public float2 Value;
        }

        public struct AgentFootprint : IComponentData
        {
            public int Size;
        }
        
        public struct AgentDensity : IComponentData
        {
            public float MinWeight;
            public float MaxWeight;
        }

        public struct PrevPosition : IComponentData
        {
            public float2 Value;
        }

        public struct Velocity : IComponentData
        {
            public float2 Value;
        }
    }
}