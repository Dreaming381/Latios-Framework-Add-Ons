using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    [BurstCompile]
    public static class Grid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCell(int2 cell, int width, int height)
        {
            return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex(int index, int width, int height)
        {
            return index >= 0 && index < width * height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 IndexToCell(int index, int width)
        {
            return new int2(index % width, index / width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CellToIndex(int width, int2 cell)
        {
            return cell.y * width + cell.x;
        }
    }
}