using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    [BurstCompile]
    internal partial struct BlendJob : IJobChunk
    {
        public ComponentBroker broker;
        public float           deltaTime;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            chunk.TryGetSharedComponent<BlendInstructions>(ref broker, out var instructions);
            var procedure = InstructionSet.GetProcedure(instructions);
            switch (procedure)
            {
                case InstructionSet.Procedure.Noop: break;
                case InstructionSet.Procedure.ProgressCurveInterpolate:
                    DoProgressCurveInterpolateProcedure(instructions, in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
                    break;
            }
        }
    }
}

