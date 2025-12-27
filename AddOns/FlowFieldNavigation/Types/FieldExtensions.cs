using System.Runtime.CompilerServices;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    [BurstCompile]
    public static class FieldExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCell(this Field field, int2 cell)
        {
            return Grid.IsValidCell(cell, field.Width, field.Height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetLocalPosition(this Field field, float3 worldPosition)
        {
            var gridOffset = field.GetGridOffset();
            var localPos = worldPosition - field.Transform.Value.position;
            var invRotation = math.inverse(field.Transform.Value.rotation);
            var unrotatedPos = math.rotate(invRotation, localPos);
            var adjustedPos = unrotatedPos - gridOffset;
            return adjustedPos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSpeedFactor(this Field field, int index)
        {
            var density = field.GetDensity(index);
            var velocity = field.MeanVelocityMap[index];
            var count = field.UnitsCountMap[index];
            var speed = math.length(velocity / count);
            return math.lerp(1, math.saturate(speed), math.saturate(density / FlowSettings.MaxDensity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDensity(this Field field, int index)
        {
            return math.select(field.DensityMap[index], 0, field.UnitsCountMap[index] <= 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetGridOffset(this Field field)
        {
            return new float3(-field.Width * field.CellSize.x * field.Pivot.x, 0, -field.Height * field.CellSize.y * field.Pivot.y);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetGridOffset(this FieldSettings field)
        {
            return new float3(-field.FieldSize.x * field.CellSize.x * field.Pivot.x, 0, -field.FieldSize.y * field.CellSize.y * field.Pivot.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 IndexToCell(this Field field, int index)
        {
            return Grid.IndexToCell(index, field.Width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CellToIndex(this Field field, int2 cell)
        {
            return Grid.CellToIndex(field.Width, cell);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 CellToWorld(this Field field, int2 cell)
        {
            var index = Grid.CellToIndex(field.Width, cell);
            return field.TransformsMap[index].position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 IndexToWorld(this Field field, int index)
        {
            var cell = Grid.IndexToCell(index, field.Width);
            return CellToWorld(cell, field.GetGridOffset(), field.CellSize, field.Transform.Value.position, field.Transform.Value.rotation);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 IndexToWorld(this FieldSettings field, TransformQvvs fieldTransform, int index)
        {
            var cell = Grid.IndexToCell(index, field.FieldSize.x);
            return CellToWorld(cell, field.GetGridOffset(), field.CellSize, fieldTransform.position, fieldTransform.rotation);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWorldToCell(this Field field, float3 worldPosition, out int2 cell)
        {
            cell = WorldToCell(worldPosition, field.GetGridOffset(), field.CellSize, field.Transform.Value.position, field.Transform.Value.rotation);
            return Grid.IsValidCell(cell, field.Width, field.Height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWorldToFootprint(this Field field, float3 worldPosition, int2 footprintSize, out int4 footprint)
        {
            var localPos = worldPosition - field.Transform.Value.position;
            var invRotation = math.inverse(field.Transform.Value.rotation);
            var unrotatedPos = math.rotate(invRotation, localPos);
            var adjustedPos = unrotatedPos - field.GetGridOffset();

            var gridCoords = new float2(
                adjustedPos.x / field.CellSize.x,
                adjustedPos.z / field.CellSize.y
            );

            var centerOffset = math.select(0f, 0.5f, footprintSize % 2 == 0);
            var centerCell = (int2)math.floor(gridCoords + centerOffset);
    
            var minX = centerCell.x - footprintSize.x / 2;
            var minY = centerCell.y - footprintSize.y / 2;
            var maxX = minX + footprintSize.x - 1;
            var maxY = minY + footprintSize.y - 1;

            if (minX >= field.Width || maxX < 0 || minY >= field.Height || maxY < 0)
            {
                footprint = default;
                return false;
            }

            footprint = new int4(minX, minY, maxX, maxY);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWorldToIndex(this Field field, float3 worldPosition, out int index)
        {
            index = WorldToIndex(worldPosition, field.GetGridOffset(), field.Width, field.CellSize, field.Transform.Value.position, field.Transform.Value.rotation);
            return Grid.IsValidIndex(index, field.Width, field.Height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 CellToWorld(int2 cell, float3 gridOffset, float2 cellSize, float3 gridPosition, quaternion gridRotation)
        {
            var cellPosition = new float3(cell.x * cellSize.x, 0, cell.y * cellSize.y);
            cellPosition += new float3(cellSize.x * 0.5f, 0, cellSize.y * 0.5f);
            var localPosition = gridOffset + cellPosition;
            var rotatedPosition = math.rotate(gridRotation, localPosition);
            return gridPosition + rotatedPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 WorldToCell(float3 worldPosition, float3 gridOffset, float2 cellSize, float3 gridPosition, quaternion gridRotation)
        {
            var localPos = worldPosition - gridPosition;
            var invRotation = math.inverse(gridRotation);
            var unrotatedPos = math.rotate(invRotation, localPos);
            var adjustedPos = unrotatedPos - gridOffset;
            return new int2((int)(adjustedPos.x / cellSize.x), (int)(adjustedPos.z / cellSize.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WorldToIndex(float3 worldPosition, float3 gridOffset, int width, float2 cellSize, float3 gridPosition, quaternion gridRotation)
        {
            var cell = WorldToCell(worldPosition, gridOffset, cellSize, gridPosition, gridRotation);
            return Grid.CellToIndex(width, cell);
        }

        public static Aabb GetAabb(this Field field)
        {
            var tranform = field.Transform.Value;
            var halfSize = new float2(field.Width, field.Height) * 0.5f;
            var localAabb = new Aabb
            {
                min = new float3(-halfSize.x, 0f, -halfSize.y),
                max = new float3(halfSize.x, 0f, halfSize.y)
            };
            return Physics.TransformAabb(tranform, localAabb);
        }
    }
}