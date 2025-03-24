using System.Runtime.InteropServices;
using Latios.Kinemation;
using Unity.Collections;
using Unity.Entities;

namespace Latios.MecanimV2
{
    public struct MecanimController : IComponentData, IEnableableComponent
    {
        public BlobAssetReference<MecanimControllerBlob>   controllerBlob;
        public BlobAssetReference<SkeletonClipSetBlob>     skeletonClipsBlob;
        public BlobAssetReference<SkeletonBoneMaskSetBlob> boneMasksBlob;
        /// <summary>
        /// The speed at which the animator controller will play and progress through states
        /// </summary>
        public float speed;
        /// <summary>
        /// The time since the last inertial blend start, or -1f if no active inertial blending is happening.
        /// </summary>
        public float realtimeInInertialBlend;
    }

    /// <summary>
    /// The dynamic data for a state machine. Multiple layers can share the same state machine. 
    /// </summary>
    [InternalBufferCapacity(1)]
    public struct MecanimStateMachineActiveStates : IBufferElementData
    {
        // Note: By using a current-next representation rather than a current-previous representation,
        // we can represent one of the state indices implicitly through the transition index, saving chunk memory
        public float                                 currentStateNormalizedTime;
        public float                                 nextStateNormalizedTime;
        public float                                 transitionNormalizedTime;
        public short                                 currentStateIndex;
        public MecanimControllerBlob.TransitionIndex nextStateTransitionIndex;  // Only when transition is active

        public static MecanimStateMachineActiveStates CreateInitialState()
        {
            return new MecanimStateMachineActiveStates
            {
                currentStateNormalizedTime = 0f,
                nextStateNormalizedTime    = 0f,
                transitionNormalizedTime   = 0f,
                currentStateIndex          = -1,
                nextStateTransitionIndex   = new MecanimControllerBlob.TransitionIndex
                {
                    index                = 0x7fff,
                    isAnyStateTransition = false
                }
            };
        }
    }

    /// <summary>
    /// Weights for each layer, including sync layers. Not present if there's only a single layer.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct LayerWeights : IBufferElementData
    {
        public float weight;
    }

    /// <summary>
    /// An animator parameter value.  The index of this state in the buffer is synchronized with the index of the parameter in the controller blob asset reference
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [InternalBufferCapacity(0)]
    public struct MecanimParameter : IBufferElementData
    {
        [FieldOffset(0)]
        public float floatParam;

        [FieldOffset(0)]
        public int intParam;

        [FieldOffset(0)]
        public bool boolParam;

        [FieldOffset(0)]
        public bool triggerParam;
    }
}

