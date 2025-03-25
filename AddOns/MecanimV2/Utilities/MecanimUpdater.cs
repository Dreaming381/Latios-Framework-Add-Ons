using System;
using Latios.Kinemation;
using Latios.Transforms;
using Latios.Unsafe;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.MecanimV2
{
    public static unsafe class MecanimUpdater
    {
        public static void Update(ref ThreadStackAllocator threadStackAllocator,
                                  ref MecanimController controller,
                                  Span<MecanimStateMachineActiveStates>      activeStates,
                                  ReadOnlySpan<LayerWeights>                 layerWeights,
                                  Span<MecanimParameter>                     parameters,
                                  OptimizedSkeletonAspect skeleton,
                                  DynamicBuffer<MecanimStateTransitionEvent> transitionEvents,
                                  DynamicBuffer<MecanimClipEvent>            clipEvents,
                                  double elapsedTime,
                                  float deltaTime,
                                  int maxStateMachineIterations = 32)
        {
            using var allocator = threadStackAllocator.CreateChildAllocator();

            transitionEvents.Clear();
            clipEvents.Clear();

            ref var blob              = ref controller.controllerBlob.Value;
            ref var clips             = ref controller.skeletonClipsBlob.Value;
            bool    isVeryFirstUpdate = activeStates[0].currentStateIndex < 0;

            var passagesBuffer    = allocator.Allocate<StateMachineEvaluation.StatePassage>(maxStateMachineIterations * blob.stateMachines.Length);
            var passagesByMachine = allocator.Allocate<UnsafeList<StateMachineEvaluation.StatePassage> >(blob.stateMachines.Length);
            for (int i = 0; i < blob.stateMachines.Length; i++)
            {
                passagesByMachine[i] = new UnsafeList<StateMachineEvaluation.StatePassage>(passagesBuffer + i * maxStateMachineIterations, maxStateMachineIterations);
            }
            var triggersToResetUlongLength = CollectionHelper.Align(parameters.Length, 64) / 64;
            var triggersToReset            = new Span<BitField64>(allocator.Allocate<BitField64>(triggersToResetUlongLength), triggersToResetUlongLength);
            triggersToReset.Clear();

            double lastElapsedTime          = elapsedTime - deltaTime;
            bool   startedNewInertialBlend  = false;
            float  newInertialBlendDuration = -1f;
            float  scaledDeltaTime          = deltaTime * controller.speed;

            for (int stateMachineIndex = 0; stateMachineIndex < blob.stateMachines.Length; stateMachineIndex++)
            {
                ref var passages = ref passagesByMachine[stateMachineIndex];
                StateMachineEvaluation.Evaluate(ref activeStates[stateMachineIndex],
                                                ref blob,
                                                ref clips,
                                                stateMachineIndex,
                                                scaledDeltaTime,
                                                layerWeights,
                                                parameters,
                                                triggersToReset,
                                                new Span<StateMachineEvaluation.StatePassage>(passages.Ptr, passages.Length),
                                                out var outputPassagesCount,
                                                out var newInertialBlendProgressRealtime,
                                                out var newInertialBlendDurationRealtime);

                passages.Length = outputPassagesCount;

                if (newInertialBlendDurationRealtime >= 0f)
                {
                    // If we are not as far along in our inertial blend, that means this is the newer inertial blend.
                    if (!startedNewInertialBlend || newInertialBlendProgressRealtime < controller.realtimeInInertialBlend)
                    {
                        controller.realtimeInInertialBlend = newInertialBlendProgressRealtime;
                        newInertialBlendDuration           = newInertialBlendDurationRealtime;
                    }
                    startedNewInertialBlend = true;
                }

                float accumulatedDeltaTime = 0f;
                for (int i = 1; i < passages.Length; i++)
                {
                    ref var previous                = ref passages.ElementAt(i - 1);
                    ref var current                 = ref passages.ElementAt(i);
                    accumulatedDeltaTime           += previous.fractionOfDeltaTimeInState;
                    var transitionEventElapsedTime  = math.lerp(lastElapsedTime, elapsedTime, accumulatedDeltaTime);
                    // Todo: Because MecanimStateTransitionEvent has both currentState and nextState as well as a `completed`,
                    // it is unclear what the expected event generations are.
                    if (previous.nextState < 0)
                    {
                        // We went from playing a single state to starting a transition.
                    }
                    else if (previous.nextState != current.currentState)
                    {
                        // We interrupted an ongoing transition with a completely new state.
                        // We jumped to this new state immediately and have completely stopped
                        // playing either of the old states. The interrupt may have smoothed things
                        // out with inertial blending.
                    }
                    else
                    {
                        // We completed the ongoing transition, and are now back to a single state.
                    }
                }
            }

            //********************

            if (blob.layers.Length == 1)
            {
                var bones         = skeleton.rawLocalTransformsRW;
                var motionBlender = new MotionBlender
                {
                    clips      = controller.skeletonClipsBlob,
                    masks      = default,
                    blender    = new BufferPoseBlender(bones),
                    rootMotion = default,
                    events     = clipEvents,
                    maskIndex  = -1,
                    sampleRoot = true,
                };

                var passages = passagesByMachine[0];
                BlendAllPassages(ref motionBlender, passages, ref blob, ref clips, parameters, 0, false, isVeryFirstUpdate);
                bones[0] = motionBlender.rootMotionResult;
            }
            else
            {
                // Todo: I need sleep.
            }
            if (isVeryFirstUpdate)
            {
                controller.realtimeInInertialBlend = -1;
                startedNewInertialBlend            = false;
            }
            if (startedNewInertialBlend)
            {
                skeleton.StartNewInertialBlend(deltaTime, newInertialBlendDuration - deltaTime);
            }
            if (controller.realtimeInInertialBlend >= 0f)
            {
                // Todo: This method should return a bool whether any blending actually occurred.
                // That way, we can detect the termination of the blend.
                skeleton.InertialBlend(controller.realtimeInInertialBlend);
            }
            skeleton.EndSamplingAndSync();
        }

        static void BlendAllPassages(ref MotionBlender blender,
                                     UnsafeList<StateMachineEvaluation.StatePassage> passages,
                                     ref MecanimControllerBlob blob,
                                     ref SkeletonClipSetBlob clips,
                                     ReadOnlySpan<MecanimParameter>                  parameters,
                                     int layerIndex,
                                     bool eventsOnly,
                                     bool isFirstUpdate)
        {
            ref var layer = ref blob.layers[layerIndex];

            TransformQvvs root = TransformQvvs.identity;
            for (int i = 0; i < passages.Length; i++)
            {
                blender.sampleSkeleton = !eventsOnly && (i + 1) == passages.Length;
                ref var current        = ref passages.ElementAt(i);

                // Current
                {
                    if (i == 0)
                        blender.includeStartEvents = isFirstUpdate;
                    else
                    {
                        var     currentState       = current.currentState;
                        ref var previous           = ref passages.ElementAt(i - 1);
                        blender.includeStartEvents = currentState != previous.currentState && currentState != previous.nextState;
                    }
                    blender.stateWeight = math.select(1f, 1f - current.transitionProgress, current.nextState >= 0);

                    var     motionIndex = layer.motionIndices[current.currentState];
                    ref var state       = ref blob.stateMachines[layer.stateMachineIndex].states[current.currentState];
                    float   startTime   = current.currentStateStartTime;
                    float   endTime     = current.currentStateEndTime;
                    PatchMotionTimes(ref state, parameters, ref startTime, ref endTime);
                    MotionEvaluation.Evaluate(startTime, endTime, ref blob, ref clips, parameters, motionIndex, ref blender);
                }

                // Next
                if (current.nextState >= 0)
                {
                    blender.includeStartEvents = isFirstUpdate || i > 0;
                    blender.stateWeight        = current.transitionProgress;

                    var     motionIndex = layer.motionIndices[current.nextState];
                    ref var state       = ref blob.stateMachines[layer.stateMachineIndex].states[current.nextState];
                    float   startTime   = current.nextStateStartTime;
                    float   endTime     = current.nextStateEndTime;
                    PatchMotionTimes(ref state, parameters, ref startTime, ref endTime);
                    MotionEvaluation.Evaluate(startTime, endTime, ref blob, ref clips, parameters, motionIndex, ref blender);
                }

                var newRoot        = blender.rootMotion.normalizedDelta;
                root               = RootMotionTools.ConcatenateDeltas(root, newRoot);
                blender.rootMotion = default;
            }
            blender.rootMotionResult = root;
        }

        static void PatchMotionTimes(ref MecanimControllerBlob.State state, ReadOnlySpan<MecanimParameter> parameters, ref float start, ref float end)
        {
            if (state.motionTimeOverrideParameterIndex >= 0)
            {
                var t = parameters[state.motionTimeOverrideParameterIndex].floatParam;
                start = t;
                end   = t;
            }
            else if (state.motionCycleOffsetParameterIndex >= 0)
            {
                var o  = parameters[state.motionCycleOffsetParameterIndex].floatParam;
                start += o;
                end   += o;
            }
            else
            {
                start += state.motionCycleOffset;
                end   += state.motionCycleOffset;
            }
        }

        struct MotionBlender : MotionEvaluation.IProcessor
        {
            public BlobAssetReference<SkeletonClipSetBlob>     clips;
            public BlobAssetReference<SkeletonBoneMaskSetBlob> masks;
            public BufferPoseBlender                           blender;
            public RootMotionDeltaAccumulator                  rootMotion;
            public TransformQvvs                               rootMotionResult;
            public DynamicBuffer<MecanimClipEvent>             events;
            public float                                       stateWeight;
            public int                                         maskIndex;
            public bool                                        sampleRoot;
            public bool                                        sampleSkeleton;
            public bool                                        includeStartEvents;

            public void Execute(in MotionEvaluation.ClipResult result)
            {
                ref var clip         = ref clips.Value.clips[result.clipIndex];
                var     clipDuration = clip.duration;

                if (clip.events.times.Length > 0)
                {
                    // Todo: Kinemation's built-in event sampling doesn't account for looping or transition start events correctly.
                    // This should probably be improved.
                }

                var weight = stateWeight * result.weight;
                if (weight <= 0f)
                    return;

                if (sampleSkeleton)
                {
                    var clipEndTime = clip.LoopToClipTime(result.currentNormalizedLoopTime * clipDuration);
                    if (maskIndex >= 0)
                        clip.SamplePose(ref blender, masks.Value[maskIndex], clipEndTime, weight);
                    else
                        clip.SamplePose(ref blender, clipEndTime, weight);
                    if (sampleRoot)
                    {
                        var clipStartTime = clip.LoopToClipTime(result.currentNormalizedLoopTime * clipDuration);
                        var loopCycles    = clip.CountLoopCycleTransitions(result.currentNormalizedLoopTime * clipDuration, result.previousNormalizedLoopTime * clipDuration);
                        rootMotion.Accumulate(ref blender, ref clip, clipStartTime, loopCycles);
                    }
                }
                else if (sampleRoot)
                {
                    rootMotion.SampleAccumulate(ref clip, result.currentNormalizedLoopTime * clipDuration, result.previousNormalizedLoopTime * clipDuration, weight);
                }
            }
        }
    }
}

