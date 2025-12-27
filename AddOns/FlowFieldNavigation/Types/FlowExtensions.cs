using Unity.Collections;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    public static class FlowExtensions
    {
        public static float2 GetDirection(this Flow flow, int index)
        {
            var direction = flow.DirectionMap[index];
            var rotatedDirection = math.mul(flow.Transform.Value.rotation, direction.x0y());
            return rotatedDirection.xz;
        }
    }
}