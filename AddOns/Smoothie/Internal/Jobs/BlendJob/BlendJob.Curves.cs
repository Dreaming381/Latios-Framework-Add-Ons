using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal partial struct BlendJob
    {
        void EvaluateSmoothstepCurve(Span<float> inout, ReadOnlySpan<byte> simdIndices)
        {
            for (int i = 0; i < simdIndices.Length; i++)
            {
                for (int j = 0; j < kSimdStride; j++)
                {
                    var index    = simdIndices[i] + j;
                    inout[index] = math.smoothstep(0f, 1f, inout[index]);
                }
            }
        }
    }
}

