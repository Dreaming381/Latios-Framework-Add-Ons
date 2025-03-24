using Unity.Entities;

namespace Latios.MecanimV2
{
    public struct MecanimClipEvent : IBufferElementData
    {
        public int    nameHash;
        public int    parameter;
        public double elapsedTime;
    }

    public struct MecanimStateTransitionEvent : IBufferElementData
    {
        public short  stateMachineIndex;
        public short  currentState;
        public short  nextState;
        public bool   completed;
        public double elapsedTime;
    }
}