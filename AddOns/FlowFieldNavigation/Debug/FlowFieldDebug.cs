using Latios.Psyshock;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using BoxCollider = Latios.Psyshock.BoxCollider;
using Physics = Latios.Psyshock.Physics;

namespace Latios.FlowFieldNavigation
{
    [BurstCompile]
    public static class FlowFieldDebug
    {
        public static JobHandle DrawCells(in Field field, in Flow flow, JobHandle inputDeps)
        {
            return new DrawCostsJob
            {
                Field = field,
                Flow = flow
            }.ScheduleParallel(field.PassabilityMap.Length, 8, inputDeps);
        }
        
        [BurstCompile]
        public struct DrawCostsJob : IJobFor
        {
            [ReadOnly] public Field Field;
            [ReadOnly] public Flow Flow;

            public void Execute(int index)
            {
                var color = Color.red;
                var offset = float3.zero;
                if (Field.PassabilityMap[index] >= 0)
                {
                    color = Color.green;
                    offset = new float3(0, 0.1f, 0);
                }

                if (Field.GetDensity(index) > 0)
                {
                    offset = new float3(0, 0.15f, 0);
                    color = Color.Lerp(Color.white, Color.blue, Field.GetDensity(index) / FlowSettings.MaxDensity);
                }
                
                DrawCell(Field, Flow, index, offset, color);
            }
        }
        
        public static void DrawCell(in Field field, in Flow flow, int index, float3 offset, Color color)
        {
            var body = field.TransformsMap[index];
            
            // var c = body.collider;
            // Physics.ScaleStretchCollider(ref c, body.transform.scale, body.transform.stretch);
            //
            // var transform = new RigidTransform(
            //     body.transform.rotation,
            //     body.transform.position + offset
            // );
            //
            // BoxCollider box = c;
            //
            // var aabb = new Aabb( box.center - box.halfSize,  box.center + box.halfSize);
            //
            // var leftTopFront     = math.transform(transform, new float3(aabb.min.x, aabb.max.y, aabb.min.z));
            // var rightTopFront    = math.transform(transform, new float3(aabb.max.x, aabb.max.y, aabb.min.z));
            // var leftTopBack      = math.transform(transform, new float3(aabb.min.x, aabb.max.y, aabb.max.z));
            // var rightTopBack     = math.transform(transform, new float3(aabb.max.x, aabb.max.y, aabb.max.z));

            // Debug.DrawLine(leftTopFront,     rightTopFront,    color);
            // Debug.DrawLine(leftTopBack,      rightTopBack,     color);
            // Debug.DrawLine(leftTopFront,     leftTopBack,      color);
            // Debug.DrawLine(rightTopFront,    rightTopBack,     color);

            var direction = flow.GetDirection(index);
            if (direction.x != 0 || direction.y != 0)
            {
                Debug.DrawRay(body.position + offset, direction.x0y()/2f, Color.yellow);
            }
        }
    }
}