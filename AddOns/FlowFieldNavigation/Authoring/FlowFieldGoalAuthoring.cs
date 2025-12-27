using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.FlowFieldNavigation.Hybrid
{
    public class FlowFieldGoalAuthoring : MonoBehaviour
    {
        [Range(1, FlowSettings.MaxFootprintSize)]
        public int FootprintSizeX = 3;
        [Range(1, FlowSettings.MaxFootprintSize)]
        public int FootprintSizeY = 3;
        
        class Baker : Baker<FlowFieldGoalAuthoring>
        {
            public override void Bake(FlowFieldGoalAuthoring authoring)
            {
                var size = new int2(authoring.FootprintSizeX, authoring.FootprintSizeY);
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new FlowField.Goal { Size = size });
            }
        }
    }
}