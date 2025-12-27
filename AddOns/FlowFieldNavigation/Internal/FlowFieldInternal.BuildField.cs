using System.Runtime.CompilerServices;
using Latios.Psyshock;
using Latios.Transforms;
using Latios.Unsafe;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Physics = Latios.Psyshock.Physics;

namespace Latios.FlowFieldNavigation
{
    [BurstCompile]
    internal static partial class FlowFieldInternal
    {
        [BurstCompile]
        internal struct BuildCellsJob : IJobFor
        {
            internal TransformQvvs FieldTransform;
            internal FieldSettings FieldSettings;
            
            [NativeDisableParallelForRestriction]
            internal NativeArray<TransformQvvs> Transforms;
            
            [NativeDisableParallelForRestriction]
            internal NativeArray<Aabb> Aabbs;

            public void Execute(int index)
            {
                var position = FieldSettings.IndexToWorld(FieldTransform, index);
                var rotation = FieldTransform.rotation;
                var tranform = new TransformQvvs(position, rotation);
                var halfSize = FieldSettings.CellSize * 0.5f;
                var localAabb = new Aabb
                {
                    min = new float3(-halfSize.x, 0f, -halfSize.y),
                    max = new float3(halfSize.x, 0f, halfSize.y)
                };

                Transforms[index] = tranform;
                Aabbs[index] = Physics.TransformAabb(tranform, localAabb);
            }
        }

        internal struct ObstaclesProcessor : IFindObjectsProcessor
        {
            [NativeDisableParallelForRestriction]
            internal Field Field;

            public void Execute(in FindObjectsResult result)
            {
                var list = new NativeList<int>(allocator: Allocator.Temp);
                
                FilterCells(Field, result.aabb, ref list);
                foreach (var i in list)
                {
                    var transform = Field.TransformsMap[i];
                    var maxDistance = math.max(Field.CellSize.x, Field.CellSize.y) * 0.5f;
                    if (!Physics.DistanceBetween(transform.position, result.collider, result.transform, maxDistance, out _))
                        continue;

                    Field.PassabilityMap[i] = -1;
                }

                list.Dispose();
            }

            static void FilterCells(Field field, Aabb aabb, ref NativeList<int> cells)
            {
                var fieldTransform = field.Transform.Value;
                var cellSize = field.CellSize;
                var gridOffset = field.GetGridOffset();
                var invRotation = math.inverse(fieldTransform.rotation);

                var worldCorners = new float3x4
                {
                    c0 = new float3(aabb.min.x, 0f, aabb.min.z),
                    c1 = new float3(aabb.max.x, 0f, aabb.min.z),
                    c2 = new float3(aabb.max.x, 0f, aabb.max.z),
                    c3 = new float3(aabb.min.x, 0f, aabb.max.z)
                };

                float2 minLocal = float.MaxValue;
                float2 maxLocal = float.MinValue;
                for (var i = 0; i < 4; i++)
                {
                    var localPos = worldCorners[i] - fieldTransform.position;
                    var unrotatedPos = math.rotate(invRotation, localPos);
                    var adjustedPos = unrotatedPos - gridOffset;
                    var localPoint = new float2(adjustedPos.x, adjustedPos.z);

                    minLocal = math.min(minLocal, localPoint);
                    maxLocal = math.max(maxLocal, localPoint);
                }

                var minCell = (int2)math.floor(minLocal / cellSize);
                var maxCell = (int2)math.floor(maxLocal / cellSize);

                minCell = math.clamp(minCell, 0, new int2(field.Width - 1, field.Height - 1));
                maxCell = math.clamp(maxCell, 0, new int2(field.Width - 1, field.Height - 1));

                for (var y = minCell.y; y <= maxCell.y; y++)
                {
                    for (var x = minCell.x; x <= maxCell.x; x++)
                    {
                        var cell = new int2(x, y);
                        var index = field.CellToIndex(cell);
                        var cellAabb = field.AabbMap[index];

                        if (AabbOverlap(aabb, cellAabb))
                        {
                            cells.Add(index);
                        }
                    }
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static bool AabbOverlap(Aabb a, Aabb b)
                {
                    return a.max.x >= b.min.x && a.min.x <= b.max.x &&
                           a.max.z >= b.min.z && a.min.z <= b.max.z;
                }
            }
        }

        internal struct AgentInfluenceData
        {
            public int Index;
            public float2 Velocity;
            public float Weight;
        }

        [BurstCompile]
        internal struct AgentsInfluenceJob : IJobChunk
        {
            [ReadOnly] internal Field Field;
            [ReadOnly] internal FlowFieldAgentsTypeHandles TypeHandles;
            internal UnsafeParallelBlockList Stream;

            [NativeSetThreadIndex] int threadIndex;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var chunkTransforms = TypeHandles.WorldTransform.Resolve(chunk);
                var chunkVelocities = chunk.GetNativeArray(ref TypeHandles.Velocity);
                var chunkFootprints = chunk.GetNativeArray(ref TypeHandles.AgentFootprint);
                var chunkDensities = chunk.GetNativeArray(ref TypeHandles.AgentDensity);

                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

                while (enumerator.NextEntityIndex(out var i))
                {
                    var position = chunkTransforms[i].position;
                    var footprintSize = chunkFootprints[i].Size;
                    var velocity = chunkVelocities[i].Value;
                    var densityData = chunkDensities[i];

                    if (!Field.TryWorldToFootprint(position, footprintSize, out var footprint)) continue;

                    var minCell = footprint.xy;
                    var maxCell = footprint.zw;
                    var radius = footprintSize / 2f;
                    var radiusSq = radius * radius;

                    for (var x = minCell.x; x <= maxCell.x; x++)
                    for (var y = minCell.y; y <= maxCell.y; y++)
                    {
                        var cell = new int2(x, y);
                        if (!Field.IsValidCell(cell)) continue;

                        var cellCenter = Field.CellToWorld(cell);

                        var distanceSq = math.distancesq(position.xz, cellCenter.xz);
                        if (distanceSq > radiusSq) continue;

                        var normalizedDistance = distanceSq / radiusSq;
                        var t = 1 - normalizedDistance;
                        var weight = math.lerp(densityData.MinWeight, densityData.MaxWeight, t * t);

                        var index = Field.CellToIndex(cell);
                        Stream.Write(new AgentInfluenceData
                        {
                            Index = index,
                            Velocity = velocity,
                            Weight = weight,
                        }, threadIndex);
                    }
                }
            }
        }

        [BurstCompile]
        internal struct AgentsPostProcessJob : IJob
        {
            [ReadOnly] internal UnsafeParallelBlockList Stream;
            internal NativeArray<float> DensityMap;
            internal NativeArray<float2> MeanVelocityMap;
            internal NativeArray<int> UnitsCountMap;

            public void Execute()
            {
                for (int i = 0; i < DensityMap.Length; i++)
                {
                    DensityMap[i] = 0;
                    MeanVelocityMap[i] = 0;
                    UnitsCountMap[i] = 0;
                }

                var enumerator = Stream.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.GetCurrentAsRef<AgentInfluenceData>();
                    var index = current.Index;
                    var density = DensityMap[index] + current.Weight;
                    var totalVelocity = MeanVelocityMap[index] + current.Velocity;
                    var totalCount = UnitsCountMap[index] + 1;
                    DensityMap[index] = math.min(density, FlowSettings.MaxDensity);
                    MeanVelocityMap[index] = totalVelocity;
                    UnitsCountMap[index] = totalCount;
                }
            }
        }
    }
}