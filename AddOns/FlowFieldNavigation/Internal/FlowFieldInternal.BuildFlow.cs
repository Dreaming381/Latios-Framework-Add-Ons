using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    internal static partial class FlowFieldInternal
    {
        [BurstCompile]
        internal struct CollectGoalsJob : IJobChunk
        {
            [ReadOnly] internal Field Field;
            [ReadOnly] internal FlowGoalTypeHandles TypeHandles;

            internal NativeHashSet<int2> GoalCells;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var chunkTransforms = TypeHandles.WorldTransform.Resolve(chunk);
                var chunkGoals = chunk.GetNativeArray(ref TypeHandles.GoalTypeHandle);

                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

                while (enumerator.NextEntityIndex(out var i))
                {
                    var position = chunkTransforms[i].position;
                    var footprintSize = chunkGoals[i].Size;

                    if (!Field.TryWorldToFootprint(position, footprintSize, out var footprint)) continue;

                    var minCell = footprint.xy;
                    var maxCell = footprint.zw;

                    for (var x = minCell.x; x <= maxCell.x; x++)
                    for (var y = minCell.y; y <= maxCell.y; y++)
                    {
                        var cell = new int2(x, y);
                        if (!Field.IsValidCell(cell)) continue;
                        GoalCells.Add(cell);
                    }
                }
            }
        }

        [BurstCompile]
        internal struct CalculateCostsWavefrontJob : IJob
        {
            [ReadOnly] internal NativeArray<int> PassabilityMap;
            [ReadOnly] internal NativeHashSet<int2> GoalCells;
            internal int Width, Height;

            [NativeDisableContainerSafetyRestriction]
            internal NativeArray<float> Costs;

            public void Execute()
            {
                var totalCells = Width * Height;
                var wave = new NativeList<int>(GoalCells.Count * 4, Allocator.Temp);
                var nextWave = new NativeList<int>(GoalCells.Count * 4, Allocator.Temp);
                var inQueue = new NativeBitArray(totalCells, Allocator.Temp);

                foreach (var goal in GoalCells)
                {
                    var idx = goal.y * Width + goal.x;
                    wave.Add(idx);
                    inQueue.Set(idx, true);
                }

                while (wave.Length > 0)
                {
                    for (var i = 0; i < wave.Length; i++)
                    {
                        var cellIndex = wave[i];
                        var currentCost = Costs[cellIndex];
                        var cellX = cellIndex % Width;
                        var cellY = cellIndex / Width;

                        ProcessNeighbor(cellIndex, cellX, cellY, 0, Width, currentCost, 1f, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 1, -Width, currentCost, 1f, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 2, -1, currentCost, 1f, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 3, 1, currentCost, 1f, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 4, Width - 1, currentCost, math.SQRT2, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 5, Width + 1, currentCost, math.SQRT2, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 6, -Width - 1, currentCost, math.SQRT2, ref nextWave, ref inQueue);
                        ProcessNeighbor(cellIndex, cellX, cellY, 7, -Width + 1, currentCost, math.SQRT2, ref nextWave, ref inQueue);
                    }

                    (wave, nextWave) = (nextWave, wave);
                    nextWave.Clear();
                    inQueue.Clear();
                }

                wave.Dispose();
                nextWave.Dispose();
                inQueue.Dispose();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void ProcessNeighbor(int cellIndex, int cellX, int cellY, int dir, int offset, float currentCost, float baseCost, ref NativeList<int> nextWave, ref NativeBitArray inQueue)
            {
                if (!IsNeighborValid(cellX, cellY, dir)) return;

                var neighborIndex = cellIndex + offset;
                var passability = PassabilityMap[neighborIndex];

                if ((uint)passability >= FlowSettings.PassabilityLimit) return;

                var newCost = currentCost + passability + baseCost;

                if (newCost < Costs[neighborIndex])
                {
                    Costs[neighborIndex] = newCost;

                    if (!inQueue.IsSet(neighborIndex))
                    {
                        nextWave.Add(neighborIndex);
                        inQueue.Set(neighborIndex, true);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsNeighborValid(int x, int y, int dir) => dir switch
            {
                0 => y < Height - 1,
                1 => y > 0,
                2 => x > 0,
                3 => x < Width - 1,
                4 => x > 0 && y < Height - 1,
                5 => x < Width - 1 && y < Height - 1,
                6 => x > 0 && y > 0,
                7 => x < Width - 1 && y > 0,
                _ => false
            };
        }
        
        [BurstCompile]
        internal struct ResetJob : IJob
        {
            internal NativeArray<float> Costs;
            internal NativeHashSet<int2> GoalCells;
            internal int Width, Height;

            public void Execute()
            {
                for (var index = 0; index < Costs.Length; index++)
                {
                    Costs[index] = FlowSettings.PassabilityLimit + 1;
                }

                foreach (var goal in GoalCells)
                {
                    Costs[Grid.CellToIndex(Width, goal)] = 0;
                }
            }
        }
        
        [BurstCompile]
        internal struct CalculateDirectionJob : IJobFor
        {
            [ReadOnly] internal NativeArray<float> CostField;
            [ReadOnly] internal NativeArray<float> DensityField;

            internal FlowSettings Settings;
            internal NativeArray<float2> DirectionMap;
            internal int Width;
            internal int Height;

            public void Execute(int index)
            {
                var currentCost = CostField[index];

                if (currentCost <= 0)
                {
                    DirectionMap[index] = float2.zero;
                    return;
                }

                var cellX = index % Width;
                var cellY = index / Width;
                var densityInfluence = Settings.DensityInfluence;
                var current = currentCost + DensityField[index] * densityInfluence;

                var gradient = float2.zero;

                AccumulateGradient(index, cellX, cellY, 0, Width, new float2(0, 1), current, densityInfluence, ref gradient);
                AccumulateGradient(index, cellX, cellY, 1, -Width, new float2(0, -1), current, densityInfluence, ref gradient);
                AccumulateGradient(index, cellX, cellY, 2, -1, new float2(-1, 0), current, densityInfluence, ref gradient);
                AccumulateGradient(index, cellX, cellY, 3, 1, new float2(1, 0), current, densityInfluence, ref gradient);

                DirectionMap[index] = math.normalizesafe(-gradient);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void AccumulateGradient(int cellIndex, int cellX, int cellY, int dir, int offset, float2 dirVector, float current, float densityInfluence, ref float2 gradient)
            {
                if (!IsNeighborValid(cellX, cellY, dir)) return;

                var neighborIndex = cellIndex + offset;
                var neighborCost = CostField[neighborIndex];

                if (neighborCost > FlowSettings.PassabilityLimit) return;

                var resultCost = neighborCost + DensityField[neighborIndex] * densityInfluence;
                var costDifference = resultCost - current;
                gradient += costDifference * dirVector;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsNeighborValid(int x, int y, int dir) => dir switch
            {
                0 => y < Height - 1,  // Up
                1 => y > 0,           // Down
                2 => x > 0,           // Left
                3 => x < Width - 1,   // Right
                _ => false
            };
        }
    }
}