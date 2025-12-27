using Unity.Entities;

namespace Latios.Mecanim
{
    [InternalBufferCapacity(0)]
    public struct MecanimClipEvent : IBufferElementData
    {
        public int    nameHash;
        public int    parameter;
        public double elapsedTime;
    }

    [InternalBufferCapacity(0)]
    public struct MecanimStateTransitionEvent : IBufferElementData
    {
        public short  stateMachineIndex;
        public short  currentState;
        public short  nextState;
        public bool   completed;
        public double elapsedTime;
    }
}

