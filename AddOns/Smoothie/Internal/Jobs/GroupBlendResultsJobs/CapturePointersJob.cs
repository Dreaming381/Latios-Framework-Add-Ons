using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    [BurstCompile]
    internal unsafe struct CapturePointersJob : IJobChunk
    {
        [ReadOnly] public SharedComponentTypeHandle<BlendInstructions> blendInstructionsHandle;
        [ReadOnly] public ComponentTypeHandle<OutputBinding>           outputBindingHandle;
        [ReadOnly] public ComponentTypeHandle<OutputEntity>            outputEntityHandle;
        [ReadOnly] public ComponentTypeHandle<OutputFloat>             outputFloatHandle;

        public NativeArray<CapturedChunkData> captures;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var               enabledCount = useEnabledMask ? chunk.Count : (math.countbits(chunkEnabledMask.ULong0) + math.countbits(chunkEnabledMask.ULong1));
            CapturedChunkData capture      = new CapturedChunkData
            {
                countInChunk   = (byte)chunk.Count,
                enabledCount   = (byte)enabledCount,
                enabledMask    = chunkEnabledMask,
                useEnabledMask = useEnabledMask,
            };
            var instructions = chunk.GetSharedComponent(blendInstructionsHandle);
            var procedure    = InstructionSet.GetProcedure(instructions);
            switch (procedure)
            {
                case InstructionSet.Procedure.Noop:
                    capture = default;
                    break;
                case InstructionSet.Procedure.ProgressCurveInterpolate:
                    GatherProgressCurveInterpolate(in chunk, instructions, ref capture);
                    break;
            }
            captures[unfilteredChunkIndex] = capture;
        }

        void GatherProgressCurveInterpolate(in ArchetypeChunk chunk, BlendInstructions instructions, ref CapturedChunkData capture)
        {
            capture.bindingPtr = chunk.GetComponentDataPtrRO(ref outputBindingHandle);
            capture.entityPtr  = chunk.GetComponentDataPtrRO(ref outputEntityHandle);
            var outputType     = InstructionSet.GetInterpolatedOutputType(instructions);
            switch (outputType)
            {
                case InstructionSet.InterpolatedOutputType.Float:
                {
                    capture.outputPtr               = chunk.GetComponentDataPtrRO(ref outputFloatHandle);
                    capture.outputByteCountPerBlend = 4;
                    break;
                }
            }
        }
    }
}

