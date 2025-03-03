using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.MecanimV2
{
    public struct MecanimControllerBlob
    {
        public ParameterTypes                parameterTypes;
        public BlobArray<int>                parameterNameHashes;
        public BlobArray<int>                parameterEditorNameHashes;  // Todo: Is it possible to match this to name hashes so we can omit this?
        public BlobArray<FixedString64Bytes> parameterNames;

        public BlobArray<Layer>        layers;
        public BlobArray<StateMachine> stateMachines;

        public BlobArray<BlendTree> blendTrees;

        public FixedString128Bytes name;

        public struct ParameterTypes
        {
            public BlobArray<int> packedTypes;
            public Type this[int index] => (Type) Bits.GetBits(packedTypes[index >> 4], (index & 0xf) << 1, 2);

            public enum Type : byte
            {
                Float = 0,
                Int = 1,
                Bool = 2,
                Trigger = 3,
            }

            // Calculates the number of ints needed in the packedTypes blob array to represent all the parameter types
            internal static int PackedTypesArrayLength(int parametersCount)
            {
                return (parametersCount + 15) >> 4;
            }

            // Packs a UnityEngine.AnimatorControllerParameterType as 2 bits in the right position of a packedTypes blob array builder
            internal static void PackTypeIntoBlobBuilder(ref BlobBuilderArray<int> packedTypesBlobBuilderArray, int parameterIndex, AnimatorControllerParameterType parameterType)
            {
                Bits.SetBits(ref packedTypesBlobBuilderArray[parameterIndex >> 4], (parameterIndex & 0xf) << 1, 2, (byte) FromAnimatorParameterType(parameterType));
            }

            // Maps Unity animator parameter types to our ParameterTypes.Type enum values.
            private static Type FromAnimatorParameterType(AnimatorControllerParameterType unityType) {
                switch (unityType)
                {
                    case AnimatorControllerParameterType.Bool: return Type.Bool;
                    case AnimatorControllerParameterType.Int: return Type.Int;
                    case AnimatorControllerParameterType.Float: return Type.Float;
                    case AnimatorControllerParameterType.Trigger: return Type.Trigger;
                }
                throw new System.Exception(
                    $"Encountered unknown animator parameter type {unityType}. If you see this, please report a bug to the Latios Framework developers.");
            }
        }

        public struct MotionIndex
        {
            private const ushort InvalidIndex = 0x7fff;

            private ushort packed;
            public ushort index  // Either blend tree array index or index into Skeleton/ParameterClipSetBlob
            {
                get => Bits.GetBits(packed, 0, 15);
                set => Bits.SetBits(ref packed, 0, 15, value);
            }
            public bool isBlendTree
            {
                get => Bits.GetBit(packed, 15);
                set => Bits.SetBit(ref packed, 15, value);
            }
            public bool invalid => index == InvalidIndex;

            public static MotionIndex Invalid = new MotionIndex
            {
                isBlendTree = false,
                index       = InvalidIndex,
            };
        }

        public struct Layer
        {
            public float originalLayerWeight;  // TODO: should this just be called weight and represent the weight of the current layer now?
            public short syncLayerIndex;  // The index of the layer we are syncing with (-1 when this is not a sync layer)
            public short stateMachineIndex;  // The state machine index this layer uses
            public short boneMaskIndex;  // Index in BoneMaskSetBlob
            public bool  performIKPass;
            public bool  isSyncLayer;
            public bool  syncLayerUsesBlendedTimings;
            public bool  useAdditiveBlending;  // If false, use override. Override is more common.

            public BlobArray<MotionIndex> motionIndices;  // Sync layers share the same state machine with another layer but can have different motions.
            public FixedString128Bytes    name;
        }

        public struct Condition
        {
            public MecanimParameter compareValue;
            public short            parameterIndex;
            public ConditionType    mode;

            public enum ConditionType : byte
            {
                If = 1,
                IfNot = 2,
                Greater = 3,
                Less = 4,
                Equals = 6,
                NotEqual = 7,
            }
        }

        public struct Transition
        {
            public BlobArray<Condition> conditions;
            // This value can either be a fraction, or a realtime in seconds. If the former, the fraction is relative to the source (current) state.
            public float duration;
            // This is the normalized state time that the next state starts playing from at the start of the transition.
            public float normalizedOffset;
            // If looping and less than 1, then this is evaluated every loop. If greater than 1, then it is evaluated exactly once.
            // This is a precise crossover threshold, not a window to the end of the state.
            public float              normalizedExitTime;
            public short              destinationStateIndex;
            public InterruptionSource interruptionSource;
            public byte               packedFlags;
            public bool hasExitTime
            {
                get => Bits.GetBit(packedFlags, 0);
                set => Bits.SetBit(ref packedFlags, 0, value);
            }
            public bool usesRealtimeDuration
            {
                get => Bits.GetBit(packedFlags, 1);
                set => Bits.SetBit(ref packedFlags, 1, value);
            }
            public bool usesOrderedInterruptions
            {
                get => Bits.GetBit(packedFlags, 2);
                set => Bits.SetBit(ref packedFlags, 2, value);
            }
            public bool canTransitionToSelf  // Only applies to Any State transitions
            {
                get => Bits.GetBit(packedFlags, 3);
                set => Bits.SetBit(ref packedFlags, 3, value);
            }

            public enum InterruptionSource : byte
            {
                None,
                Source,
                Destination,
                SourceThenDestination,
                DestinationThenSource,
            }
        }

        // This is used externally by ECS components.
        public struct TransitionIndex
        {
            // Currently, we use a single bit to represent Any State.
            // But we could instead assume an index concatenation of local state transitions
            // followed by any state transitions from local sub-state machine to root if we
            // wanted to support localized Any States.
            public ushort packed;
            public ushort index
            {
                get => Bits.GetBits(packed, 0, 15);
                set => Bits.SetBits(ref packed, 0, 15, value);
            }
            public bool isAnyStateTransition
            {
                get => Bits.GetBit(packed, 15);
                set => Bits.SetBit(ref packed, 15, value);
            }
            public bool invalid => index == 0x7fff;

            public static TransitionIndex Null => new TransitionIndex { packed = 0x7fff };
        }

        public struct State
        {
            public BlobArray<Transition> transitions;

            // Things are about to get really confusing here, especially if you try to parse Unity's documentation.
            // There are two different time values associated with a state, which both operate in "normalized time".
            // First, there's state time, which is what is used to drive state transitions. Then, there's motion time
            // which is used to sample the motion and blend trees. By default, state time is used as motion time,
            // but motion time can be overridden and augmented using parameters and constants. Such changes do not
            // reflect back to the state time.

            public float baseStateSpeed;
            // I think Unity's documentation is wrong. This value does affect motion time. It doesn't affect state time.
            public float motionCycleOffset;

            public short stateSpeedMultiplierParameterIndex;
            public short motionCycleOffsetParameterIndex;
            public short mirrorParameterIndex;
            public short motionTimeOverrideParameterIndex;

            // Index of this state in the layers that use it, to access its motion,
            // we can use layer.motionIndices[state.stateIndexInLayer]
            public short stateIndexInStateMachine;

            private ushort packedFlags;
            public bool useFootIK  // Not supported at runtime yet
            {
                get => Bits.GetBit(packedFlags, 0);
                set => Bits.SetBit(ref packedFlags, 0, value);
            }
            public bool useMirror  // Not supported at runtime yet
            {
                get => Bits.GetBit(packedFlags, 1);
                set => Bits.SetBit(ref packedFlags, 1, value);
            }
            // There's 46 bits to spare to pad out to 32 bytes.
        }

        public struct StateMachine
        {
            // These are only the primary state machine layer and sync layers whose weights affect timings.
            public BlobArray<short> influencingLayers;
            // Note: We flatten out sub-state machines.
            public BlobArray<State> states;

            // According to this post: https://discussions.unity.com/t/transition-to-sub-state-machine-from-any-state/605174/5
            // the Any State in each sub-state machine refers to the exact same global any state, rather than any kind of hierarcy.
            // If for whatever reason we wanted to change this, we could have a transition array for each sub-state machine along
            // with parent sub-state machine indices. Each state would then index a sub-state machine and we'd walk back to the root.
            // Such a strategy would still support our flattened representation.
            public BlobArray<Transition> anyStateTransitions;

            // These only have destination state indices and conditions. There's no timing. And we only care about this on the very first update.
            // In baking, we need to identify the default transition and add it at the end, after all other entry transitions.
            // Also in baking, we should convert each state -> exit -> entry -> state permutation to direct state -> state transitions.
            // This might sound like a potential issue of overflowing 15 bits, but we have a unique set of transition indices per state.
            // Without this flattening, we run into issues with our transition data being spread across two different transition instances.
            // The exit transition (or transition into a sub-state machine) contains the blending info and interrupts, while the entry
            // transition contains the target state.
            public BlobArray<Transition>          initializationEntryStateTransitions;
            public BlobArray<int>                 stateNameHashes;
            public BlobArray<int>                 stateNameEditorHashes;
            public BlobArray<FixedString128Bytes> stateNames;
            public BlobArray<FixedString128Bytes> stateTags;
        }

        public struct BlendTree
        {
            public BlendTreeType    blendTreeType;
            public BlobArray<Child> children;
            public BlobArray<short> parameterIndices;
            // For freeform trees, sized by (childCount - 1)^2, z is 1 / lengthSq
            // For simple directional, sized by childCount, x is atan2(childPosition), y at index 0 is the index of the center child if any (asint)
            public BlobArray<float3> pipjs;

            public enum BlendTreeType
            {
                Simple1D,
                SimpleDirectional2D,
                FreeformDirectional2D,
                FreeformCartesian2D,
                Direct
            }

            public struct Child
            {
                public float2      position;
                public float       cycleOffset;
                public float       timeScale;
                public float       threshold;
                public MotionIndex motionIndex;
                public ushort      packedFlags;
                public bool isLooping  // I don't think we can support this
                {
                    get => Bits.GetBit(packedFlags, 0);
                    set => Bits.SetBit(ref packedFlags, 0, value);
                }
                public bool mirrored  // Not supported yet
                {
                    get => Bits.GetBit(packedFlags, 1);
                    set => Bits.SetBit(ref packedFlags, 1, value);
                }
            }

#if UNITY_EDITOR
            // Maps Unity animator parameter types to our ParameterTypes.Type enum values.
            internal static BlendTreeType FromUnityBlendTreeType(UnityEditor.Animations.BlendTreeType unityBlendTreeType) {
                switch (unityBlendTreeType)
                {
                    case UnityEditor.Animations.BlendTreeType.Simple1D: return BlendTreeType.Simple1D;
                    case UnityEditor.Animations.BlendTreeType.SimpleDirectional2D: return BlendTreeType.SimpleDirectional2D;
                    case UnityEditor.Animations.BlendTreeType.FreeformDirectional2D: return BlendTreeType.FreeformDirectional2D;
                    case UnityEditor.Animations.BlendTreeType.FreeformCartesian2D: return BlendTreeType.FreeformCartesian2D;
                    case UnityEditor.Animations.BlendTreeType.Direct: return BlendTreeType.Direct;
                }
                throw new System.Exception(
                    $"Encountered unknown blend tree type {unityBlendTreeType}. If you see this, please report a bug to the Latios Framework developers.");
            }
#endif

            internal static int PipjIndex(int i, int j, int childCount)
            {
                return (childCount - 1) * i + math.select(j, j - 1, j > i);
            }

            internal const float kFreeformDirectionalBias = 2f;
        }
    }
}

