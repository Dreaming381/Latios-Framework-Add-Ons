using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.FlowFieldNavigation.Hybrid
{
    public class FlowFieldAgentAuthoring : MonoBehaviour
    {
        [Range(1, FlowSettings.MaxFootprintSize)]
        public int FootprintSize = 3;
        
        [Header("Density Settings")]
        [Range(0, FlowSettings.MaxDensity)]
        public float MinDensity;
        [Range(0, FlowSettings.MaxDensity)]
        public float MaxDensity;

        class Baker : Baker<FlowFieldAgentAuthoring>
        {
            public override void Bake(FlowFieldAgentAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<FlowField.AgentDirection>(entity);
                AddComponent<FlowField.PrevPosition>(entity);
                AddComponent<FlowField.Velocity>(entity);
                AddComponent(entity, new FlowField.AgentFootprint { Size = authoring.FootprintSize });
                AddComponent(entity, new FlowField.AgentDensity
                {
                    MinWeight = authoring.MinDensity,
                    MaxWeight = authoring.MaxDensity,
                });
            }
        }
    }
}