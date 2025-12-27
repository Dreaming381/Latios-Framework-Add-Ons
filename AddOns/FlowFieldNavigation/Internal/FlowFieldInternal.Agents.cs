using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.FlowFieldNavigation
{
    internal static partial class FlowFieldInternal
    {
        [BurstCompile]
        internal struct CalculateAgentsDirectionsJob : IJobChunk
        {
            [ReadOnly] internal Flow Flow;
            [ReadOnly] internal Field Field;
            internal FlowFieldAgentsTypeHandles TypeHandles;
            internal float DeltaTime;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var chunkTransforms = TypeHandles.WorldTransform.Resolve(chunk);
                var controls = chunk.GetNativeArray(ref TypeHandles.AgentDirection);
                var prevPositions = chunk.GetNativeArray(ref TypeHandles.PrevPosition);
                var velocities = chunk.GetNativeArray(ref TypeHandles.Velocity);
                var footprints = chunk.GetNativeArray(ref TypeHandles.AgentFootprint);
                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

                while (enumerator.NextEntityIndex(out var i))
                {
                    var position = chunkTransforms[i].position;
                    var prevPosition = prevPositions[i].Value;
                    var footprint = footprints[i].Size;
                    var newVelocity = (position.xz - prevPosition) / DeltaTime;
                    velocities[i] = new FlowField.Velocity { Value = newVelocity };
                    prevPositions[i] = new FlowField.PrevPosition { Value = position.xz };

                    var newPos = CalculateFootprintDirection(position, footprint, in Field, in Flow);
                    controls[i] = new FlowField.AgentDirection { Value = newPos };
                }
            }
        }

        static float2 CalculateFootprintDirection(float3 worldPos,int footprintSize, in Field field, in Flow flow)
        {
            if (!field.TryWorldToFootprint(worldPos, footprintSize, out var footprint))
            {
                return float2.zero;
            }

            var localPos = worldPos - field.Transform.Value.position;
            var invRotation = math.inverse(field.Transform.Value.rotation);
            var unrotatedPos = math.rotate(invRotation, localPos);
            var adjustedPos = unrotatedPos - field.GetGridOffset();

            var gridCoords = new float2(
                adjustedPos.x / field.CellSize.x,
                adjustedPos.z / field.CellSize.y
            );

            var totalDirection = float2.zero;
            var totalWeight = 0f;
            
            var totalGradient = float2.zero;
            var maxDensity = 0f;

            for (var x = footprint.x; x <= footprint.z; x++)
            {
                for (var y = footprint.y; y <= footprint.w; y++)
                {
                    if (!field.IsValidCell(new int2(x, y)))
                        continue;

                    var index = Grid.CellToIndex(field.Width, new int2(x, y));

                    var cellCenter = new float2(
                        x * field.CellSize.x + field.CellSize.x * 0.5f,
                        y * field.CellSize.y + field.CellSize.y * 0.5f
                    );
                    
                    var distance = math.distance(gridCoords, cellCenter);
                    var weight = 1f / (1f + distance);
                    var direction = flow.GetDirection(index);
                    var speedFactor = field.GetSpeedFactor(index);
                    totalDirection += direction * speedFactor * weight;
                    totalWeight += weight;

                    var density = field.GetDensity(index);
                    maxDensity = math.max(maxDensity, density);
                    var toAgent = gridCoords - cellCenter;
                    totalGradient += toAgent * (density / FlowSettings.MaxDensity);
                }
            }
            
            if (totalWeight > 0)
            {
                var flowDirection = totalDirection / totalWeight;
                var flowLength = math.length(flowDirection);

                var avoidanceDirection = math.normalizesafe(totalGradient);
                var avoidanceStrength = math.saturate(maxDensity / FlowSettings.MaxDensity);
                var blendedDirection = math.lerp(flowDirection, avoidanceDirection, avoidanceStrength);

                return math.normalizesafe(blendedDirection) * flowLength;
            }

            return float2.zero;
        }
    }
}