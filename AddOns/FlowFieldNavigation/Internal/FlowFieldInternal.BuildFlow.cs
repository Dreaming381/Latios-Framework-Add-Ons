using System.Collections.Generic;
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
        internal struct CalculateCostsWithPriorityQueueJob : IJob
        {
            const float HeapCapacityFactor = 0.35f;

            [ReadOnly] internal NativeArray<int> PassabilityMap;
            [ReadOnly] internal NativeHashSet<int2> GoalCells;
            internal int Width, Height;

            internal NativeArray<float> Costs;

            public void Execute()
            {
                for (var i = 0; i < Costs.Length; i++) Costs[i] = FlowSettings.PassabilityLimit + 1;
                int initialHeapCapacity = math.max(64, (int)(Width * Height * HeapCapacityFactor));
                var queue = new NativePriorityQueue<CostEntry, CostComparer>(initialHeapCapacity, Allocator.Temp);
                var visited = new NativeBitArray(Width * Height, Allocator.Temp);

                foreach (var goal in GoalCells)
                {
                    queue.Enqueue(new(goal, 0));
                    Costs[Grid.CellToIndex(Width, goal)] = 0;
                }

                while (queue.TryDequeue(out var current))
                {
                    var currentCost = Costs[Grid.CellToIndex(Width, current.Pos)];

                    for (var dir = Grid.Direction.Up; dir <= Grid.Direction.DownRight; dir++)
                    {
                        if (!Grid.TryGetNeighborCell(Width, Height, current.Pos, dir, out var neighbor)) continue;

                        var neighborIndex = Grid.CellToIndex(Width, neighbor);
                        if (visited.IsSet(neighborIndex)) continue;

                        if (PassabilityMap[neighborIndex] < 0) continue;

                        var moveCost = PassabilityMap[neighborIndex] + (dir > Grid.Direction.Right ? math.SQRT2 : 1);

                        var newCost = currentCost + moveCost;

                        if (newCost < Costs[neighborIndex])
                        {
                            Costs[neighborIndex] = newCost;
                            queue.Enqueue(new(neighbor, newCost));
                            visited.Set(neighborIndex, true);
                        }
                    }
                }

                queue.Dispose();
                visited.Dispose();
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
                var wave = new NativeList<int2>(GoalCells.Count, Allocator.Temp);

                foreach (var goal in GoalCells)
                {
                    wave.Add(goal);
                }

                var nextWave = new NativeList<int2>(wave.Capacity, Allocator.Temp);

                while (wave.Length > 0)
                {
                    for (var i = 0; i < wave.Length; i++)
                    {
                        var cell = wave[i];
                        var cellIndex = Grid.CellToIndex(Width, cell);
                        
                        var currentCost = Costs[cellIndex];

                        for (var dir = Grid.Direction.Up; dir <= Grid.Direction.DownRight; dir++)
                        {
                            if (!Grid.TryGetNeighborCell(Width, Height, cell, dir, out var neighbor)) continue;

                            var neighborIndex = Grid.CellToIndex(Width, neighbor);
                            var passability = PassabilityMap[neighborIndex];

                            if (passability < 0 || passability >= FlowSettings.PassabilityLimit) continue;

                            var moveCost = passability + (dir > Grid.Direction.Right ? math.SQRT2 : 1f);
                            var newCost = currentCost + moveCost;

                            if (newCost < Costs[neighborIndex])
                            {
                                Costs[neighborIndex] = newCost;
                                nextWave.Add(neighbor);
                            }
                        }
                    }

                    (wave, nextWave) = (nextWave, wave);
                    nextWave.Clear();
                }

                wave.Dispose();
                nextWave.Dispose();
            }
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

        internal struct CostComparer : IComparer<CostEntry>
        {
            public int Compare(CostEntry x, CostEntry y) => x.Cost.CompareTo(y.Cost);
        }

        internal readonly struct CostEntry
        {
            public readonly float Cost;
            public readonly int2 Pos;

            public CostEntry(int2 pos, float cost)
            {
                Pos = pos;
                Cost = cost;
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
                var cell = Grid.IndexToCell(index, Width);
                var gradient = float2.zero;
                var currentCost = CostField[index];

                if (currentCost <= 0)
                {
                    DirectionMap[index] = float2.zero;
                    return;
                }

                var current = currentCost + DensityField[index] * Settings.DensityInfluence;

                for (var dir = Grid.Direction.Up; dir <= Grid.Direction.Right; dir++)
                {
                    if (!Grid.TryGetNeighborCell(Width, Height, cell, dir, out var neighbor)) continue;

                    var neighborIndex = Grid.CellToIndex(Width, neighbor);
                    var neighborCost = CostField[neighborIndex];
                    if (neighborCost > FlowSettings.PassabilityLimit) continue;
                    var resultCost = neighborCost + DensityField[neighborIndex] * Settings.DensityInfluence;

                    var costDifference = resultCost - current;
                    var addGradient = costDifference * dir.ToVector();
                    gradient += addGradient;
                }

                DirectionMap[index] = math.normalizesafe(-gradient);
            }
        }
    }
}