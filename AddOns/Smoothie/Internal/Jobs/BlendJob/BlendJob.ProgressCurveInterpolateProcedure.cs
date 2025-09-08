using System;
using Unity.Burst.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal partial struct BlendJob
    {
        [SkipLocalsInit]
        unsafe void DoProgressCurveInterpolateProcedure(BlendInstructions instructions,
                                                        in ArchetypeChunk chunk,
                                                        int unfilteredChunkIndex,
                                                        bool useEnabledMask,
                                                        in v128 chunkEnabledMask)
        {
            Span<byte>  simdIndices  = stackalloc byte[128 / kSimdStride];
            Span<float> blendFactors = stackalloc float[128];
            int         chunkCount   = chunk.Count;
            Hint.Assume(chunkCount <= 128);
            BuildSimdIndices(ref simdIndices, chunkCount, useEnabledMask, chunkEnabledMask);
            Hint.Assume(simdIndices.Length > 0);

            var progressionArray = (float*)chunk.GetComponentDataPtrRW<Progression>(ref broker);
            var durationArray    = (float*)chunk.GetComponentDataPtrRO<Duration>(ref broker);
            for (int i = 0; i < simdIndices.Length; i++)
            {
                for (int j = 0; j < kSimdStride; j++)
                {
                    var index           = simdIndices[i] + j;
                    blendFactors[index] = math.min(1f, progressionArray[index] + deltaTime / durationArray[index]);
                }
            }
            WriteMasked(blendFactors, progressionArray, chunkCount, useEnabledMask, chunkEnabledMask);

            if (InstructionSet.GetUsesIncompleteFlag(instructions))
            {
                var mask       = chunk.GetEnabledMask<IncompleteFlag>(ref broker);
                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunkCount);
                while (enumerator.NextEntityIndex(out int i))
                {
                    if (blendFactors[i] >= 1f)
                        mask[i] = false;
                }
            }

            var curveFunction = InstructionSet.GetCurveFunction(instructions);
            switch (curveFunction)
            {
                case InstructionSet.CurveFunction.Passthrough:
                    break;
                case InstructionSet.CurveFunction.Smoothstep:
                    EvaluateSmoothstepCurve(blendFactors, simdIndices);
                    break;
            }

            var interpolatedOutputType = InstructionSet.GetInterpolatedOutputType(instructions);
            switch (interpolatedOutputType)
            {
                case InstructionSet.InterpolatedOutputType.Float:
                {
                    float* startArray = stackalloc float[128];
                    if (InstructionSet.GetInterpolationStartIsBinding(instructions))
                        ReadBindingsMasked(TypeManager.GetTypeIndex<ComponentBindingStart>(),
                                           startArray,
                                           in chunk,
                                           chunkCount,
                                           useEnabledMask,
                                           chunkEnabledMask,
                                           BindingPrimitiveType.Float);
                    else
                        startArray = (float*)chunk.GetComponentDataPtrRO<ConstantStartFloat>(ref broker);

                    float* endArray = stackalloc float[128];
                    if (InstructionSet.GetInterpolationStartIsBinding(instructions))
                        ReadBindingsMasked(TypeManager.GetTypeIndex<ComponentBindingEnd>(),
                                           endArray,
                                           in chunk,
                                           chunkCount,
                                           useEnabledMask,
                                           chunkEnabledMask,
                                           BindingPrimitiveType.Float);
                    else
                        endArray = (float*)chunk.GetComponentDataPtrRO<ConstantEndFloat>(ref broker);

                    float* interpolated = stackalloc float[128];
                    for (int i = 0; i < simdIndices.Length; i++)
                    {
                        for (int j = 0; j < kSimdStride; j++)
                        {
                            var index           = simdIndices[i] + j;
                            interpolated[index] = math.lerp(startArray[index], endArray[index], blendFactors[index]);
                        }
                    }

                    var resultArray = (float*)chunk.GetComponentDataPtrRO<OutputFloat>(ref broker);
                    WriteMasked(new ReadOnlySpan<float>(interpolated, 128), resultArray, chunkCount, useEnabledMask, chunkEnabledMask);
                    break;
                }
            }
        }
    }
}

