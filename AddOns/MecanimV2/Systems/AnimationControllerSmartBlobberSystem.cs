#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Latios.Authoring;
using Latios.Authoring.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;

namespace Latios.MecanimV2.Authoring.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(SmartBlobberBakingGroup))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    public partial class AnimationControllerSmartBlobberSystem : SystemBase
    {
        protected override void OnCreate()
        {
            new SmartBlobberTools<MecanimControllerBlob>().Register(World);
        }

        protected override void OnUpdate()
        {
            int count = SystemAPI.QueryBuilder().WithAll<MecanimControllerBlobRequest>().WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                        .Build().CalculateEntityCountWithoutFiltering();
            var hashmap = new NativeParallelHashMap<UnityObjectRef<AnimatorController>, BlobAssetReference<MecanimControllerBlob> >(count * 2, WorldUpdateAllocator);

            new GatherJob { hashmap = hashmap.AsParallelWriter() }.ScheduleParallel();
            CompleteDependency();

            foreach (var pair in hashmap)
            {
                pair.Value = BakeAnimatorController(pair.Key.Value);
            }

            Entities.WithReadOnly(hashmap).ForEach((ref SmartBlobberResult result, in MecanimControllerBlobRequest request) =>
            {
                var controllerBlob = hashmap[request.animatorController];
                result.blob        = UnsafeUntypedBlobAssetReference.Create(controllerBlob);
            }).WithEntityQueryOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities).ScheduleParallel();
        }

        [WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)]
        [BurstCompile]
        partial struct GatherJob : IJobEntity
        {
            public NativeParallelHashMap<UnityObjectRef<AnimatorController>, BlobAssetReference<MecanimControllerBlob> >.ParallelWriter hashmap;

            public void Execute(in MecanimControllerBlobRequest request)
            {
                hashmap.TryAdd(request.animatorController, default);
            }
        }

        private void BakeAnimatorCondition(ref MecanimControllerBlob.Condition blobAnimatorCondition, AnimatorCondition condition,
                                           AnimatorControllerParameter[] parameters)
        {
            blobAnimatorCondition.mode = (MecanimControllerBlob.Condition.ConditionType)condition.mode;

            parameters.TryGetParameter(condition.parameter, out short conditionParameterIndex);
            blobAnimatorCondition.parameterIndex = conditionParameterIndex;
            blobAnimatorCondition.compareValue   = new MecanimParameter { floatParam = condition.threshold };
        }

        private void BakeAnimatorStateTransition(ref BlobBuilder builder,
                                                 ref MecanimControllerBlob.Transition blobTransition,
                                                 AnimatorTransitionBase transition,
                                                 int destinationStateIndex,
                                                 AnimatorControllerParameter[]        parameters)
        {
            SetTransitionDestinationAndSettings(ref blobTransition, transition, destinationStateIndex);

            // Bake conditions
            BlobBuilderArray<MecanimControllerBlob.Condition> conditionsBuilder =
                builder.Allocate(ref blobTransition.conditions, transition.conditions.Length);

            for (int i = 0; i < transition.conditions.Length; i++)
            {
                BakeAnimatorCondition(ref conditionsBuilder[i], transition.conditions[i], parameters);
            }
        }

        private void BakeAnimatorStateTransitionWithCustomConditions(ref BlobBuilder builder,
                                                                     ref MecanimControllerBlob.Transition blobTransition,
                                                                     AnimatorTransitionBase transition,
                                                                     List<AnimatorCondition>              animatorConditions,
                                                                     int destinationStateIndex,
                                                                     AnimatorControllerParameter[]        parameters)
        {
            SetTransitionDestinationAndSettings(ref blobTransition, transition, destinationStateIndex);

            // Bake conditions
            BlobBuilderArray<MecanimControllerBlob.Condition> conditionsBuilder =
                builder.Allocate(ref blobTransition.conditions, animatorConditions.Count);

            for (int i = 0; i < animatorConditions.Count; i++)
            {
                BakeAnimatorCondition(ref conditionsBuilder[i], animatorConditions[i], parameters);
            }
        }

        private static void SetTransitionDestinationAndSettings(ref MecanimControllerBlob.Transition blobTransition, AnimatorTransitionBase transition, int destinationStateIndex)
        {
            blobTransition.destinationStateIndex = (short)destinationStateIndex;

            if (transition is AnimatorStateTransition animatorStateTransition)
            {
                blobTransition.hasExitTime              = animatorStateTransition.hasExitTime;
                blobTransition.normalizedExitTime       = animatorStateTransition.exitTime;
                blobTransition.normalizedOffset         = animatorStateTransition.offset;
                blobTransition.duration                 = animatorStateTransition.duration;
                blobTransition.interruptionSource       = (MecanimControllerBlob.Transition.InterruptionSource)animatorStateTransition.interruptionSource;
                blobTransition.usesOrderedInterruptions = animatorStateTransition.orderedInterruption;
                blobTransition.usesRealtimeDuration     = animatorStateTransition.hasFixedDuration;
            }
        }

        private void BakeState(
            ref MecanimControllerBlob.State blobState,
            short stateIndexInStateMachine,
            AnimatorState state,
            AnimatorControllerParameter[]   parameters)
        {
            blobState.baseStateSpeed    = state.speed;
            blobState.motionCycleOffset = state.cycleOffset;

            blobState.useMirror = state.mirror;
            blobState.useFootIK = state.iKOnFeet;

            blobState.stateSpeedMultiplierParameterIndex = -1;

            blobState.stateIndexInStateMachine = stateIndexInStateMachine;

            if (state.speedParameterActive)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].name == state.speedParameter)
                    {
                        blobState.stateSpeedMultiplierParameterIndex = (short)i;
                        break;
                    }
                }
            }

            blobState.motionCycleOffsetParameterIndex = -1;
            if (state.cycleOffsetParameterActive)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].name == state.cycleOffsetParameter)
                    {
                        blobState.motionCycleOffsetParameterIndex = (short)i;
                        break;
                    }
                }
            }

            blobState.mirrorParameterIndex = -1;
            if (state.mirrorParameterActive)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].name == state.mirrorParameter)
                    {
                        blobState.mirrorParameterIndex = (short)i;
                        break;
                    }
                }
            }

            blobState.motionTimeOverrideParameterIndex = -1;
            if (state.timeParameterActive)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].name == state.timeParameter)
                    {
                        blobState.motionTimeOverrideParameterIndex = (short)i;
                        break;
                    }
                }
            }
        }

        private MecanimControllerBlob.StateMachine BakeStateMachine(ref BlobBuilder builder,
                                                                    ref MecanimControllerBlob.StateMachine stateMachineBlob,
                                                                    AnimatorControllerLayer layer,
                                                                    AnimatorControllerParameter[]          parameters)
        {
            // Gather all states in the state machine (including nested state machines)
            List<StateInfo>                                   stateInfos           = new List<StateInfo>();
            UnsafeHashMap<UnityObjectRef<AnimatorState>, int> statesIndicesHashMap =
                new UnsafeHashMap<UnityObjectRef<AnimatorState>, int>(10, Allocator.Temp);
            UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>, UnityObjectRef<AnimatorStateMachine> > stateMachineParentHashMap =
                new UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>, UnityObjectRef<AnimatorStateMachine> >(1, Allocator.Temp);

            CollectStateInfosRecursivelyForStateMachine(ref stateInfos, ref statesIndicesHashMap, ref stateMachineParentHashMap, layer.stateMachine);

            //States
            BlobBuilderArray<MecanimControllerBlob.State> statesBuilder = builder.Allocate(ref stateMachineBlob.states, stateInfos.Count);

            BlobBuilderArray<int>                 stateNameHashesBuilder       = builder.Allocate(ref stateMachineBlob.stateNameHashes, stateInfos.Count);
            BlobBuilderArray<int>                 stateNameEditorHashesBuilder = builder.Allocate(ref stateMachineBlob.stateNameEditorHashes, stateInfos.Count);
            BlobBuilderArray<FixedString128Bytes> stateNamesBuilder            = builder.Allocate(ref stateMachineBlob.stateNames, stateInfos.Count);
            BlobBuilderArray<FixedString128Bytes> stateTagsBuilder             = builder.Allocate(ref stateMachineBlob.stateTags, stateInfos.Count);

            // Build BlobArray of states
            for (short i = 0; i < stateInfos.Count; i++)
            {
                // Bake transitions for this state
                BakeTransitionsForState(stateInfos[i], ref builder, ref statesBuilder[i], ref statesIndicesHashMap, ref stateMachineParentHashMap, parameters);

                BakeState(ref statesBuilder[i], i, stateInfos[i].animationState, parameters);

                stateNameHashesBuilder[i]       = stateInfos[i].fullPathName.GetHashCode();
                stateNameEditorHashesBuilder[i] = stateInfos[i].fullPathName.GetHashCode();
                stateNamesBuilder[i]            = stateInfos[i].fullPathName;
                stateTagsBuilder[i]             = stateInfos[i].animationState.tag;
            }

            // Bake Entry transitions
            BakeStateMachineSpecialTransitions(ref builder,
                                               layer.stateMachine.entryTransitions,
                                               ref stateMachineBlob.initializationEntryStateTransitions,
                                               layer,
                                               parameters,
                                               statesIndicesHashMap,
                                               stateMachineParentHashMap,
                                               mustAddDefaultStateTransition: true);

            // Bake AnyState transitions
            BakeStateMachineSpecialTransitions(ref builder,
                                               layer.stateMachine.anyStateTransitions,
                                               ref stateMachineBlob.anyStateTransitions,
                                               layer,
                                               parameters,
                                               statesIndicesHashMap,
                                               stateMachineParentHashMap,
                                               mustAddDefaultStateTransition: false);

            return stateMachineBlob;
        }

        private void BakeStateMachineSpecialTransitions(ref BlobBuilder builder,
                                                        AnimatorTransitionBase[] specialTransitionsArray,
                                                        ref BlobArray<MecanimControllerBlob.Transition> transitionsBlobArray,
                                                        AnimatorControllerLayer layer,
                                                        AnimatorControllerParameter[] parameters,
                                                        UnsafeHashMap<UnityObjectRef<AnimatorState>, int> statesIndicesHashMap,
                                                        UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>, UnityObjectRef<AnimatorStateMachine> >
                                                        stateMachineParentHashMap,
                                                        bool mustAddDefaultStateTransition)
        {
            // We use this list to return all the collapsed transitions we find for a specific starting transition
            List<CollapsedTransition> collapsedTransitions = new List<CollapsedTransition>();
            // We use this list to accumulate conditions recursively so we can add the list of conditions to a transition when we find its destination state
            List<AnimatorCondition> accumulatedConditions = new List<AnimatorCondition>();

            // BAKE ENTRY TRANSITIONS
            foreach (var specialTransition in specialTransitionsArray)
            {
                accumulatedConditions.Clear();

                // Process entry transitions. Must find the destination state recursively in case they are pointing to a hierarchy of sub state machines.
                FindCollapsedTransitions(specialTransition,
                                         specialTransition,
                                         layer.stateMachine,
                                         accumulatedConditions,
                                         collapsedTransitions,
                                         ref statesIndicesHashMap,
                                         ref stateMachineParentHashMap);
            }

            int transitionsCount = collapsedTransitions.Count;

            bool addDefaultState = mustAddDefaultStateTransition && layer.stateMachine.defaultState != null;

            if (addDefaultState)
                transitionsCount++; // we allocate one more for the default entry state transition in position 0 when needed

            BlobBuilderArray<MecanimControllerBlob.Transition> entryTransitionsBuilder = builder.Allocate(ref transitionsBlobArray, transitionsCount);

            for (var i = 0; i < collapsedTransitions.Count; i++)
            {
                var collapsedTransition = collapsedTransitions[i];
                BakeAnimatorStateTransitionWithCustomConditions(ref builder,
                                                                ref entryTransitionsBuilder[i],
                                                                collapsedTransition.transition,
                                                                collapsedTransition.conditions,
                                                                collapsedTransition.destinationStateIndex,
                                                                parameters);
            }

            if (addDefaultState)
            {
                // Add a dummy transition to the default state in the last position of the blob array
                entryTransitionsBuilder[collapsedTransitions.Count].destinationStateIndex = (short) statesIndicesHashMap[layer.stateMachine.defaultState];
            }
        }

        private void BakeTransitionsForState(StateInfo stateInfo,
                                             ref BlobBuilder blobBuilder,
                                             ref MecanimControllerBlob.State stateBlob,
                                             ref UnsafeHashMap<UnityObjectRef<AnimatorState>, int> statesIndicesHashMap,
                                             ref UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>, UnityObjectRef<AnimatorStateMachine> > stateMachineParentHashMap,
                                             AnimatorControllerParameter[] parameters)
        {
            List<CollapsedTransition> collapsedTransitions = new List<CollapsedTransition>();

            // We use this list to accumulate conditions recursively so we can add the list of conditions when we complete it.
            List<AnimatorCondition> accumulatedConditions = new List<AnimatorCondition>();

            // Recursively find all collapsed transitions leaving this state and add them to collapsedTransitions list
            for (var index = 0; index < stateInfo.animationState.transitions.Length; index++)
            {
                var transition = stateInfo.animationState.transitions[index];

                FindCollapsedTransitions(transition,
                                         transition,
                                         stateInfo.stateMachine,
                                         accumulatedConditions,
                                         collapsedTransitions,
                                         ref statesIndicesHashMap,
                                         ref stateMachineParentHashMap);
            }

            // Actually bake all the transitions and conditions we collected
            BlobBuilderArray<MecanimControllerBlob.Transition> transitionsBuilder = blobBuilder.Allocate(ref stateBlob.transitions,
                                                                                                         collapsedTransitions.Count);
            for (var i = 0; i < collapsedTransitions.Count; i++)
            {
                var collapsedTransition = collapsedTransitions[i];
                BakeAnimatorStateTransitionWithCustomConditions(ref blobBuilder,
                                                                ref transitionsBuilder[i],
                                                                collapsedTransition.transition,
                                                                collapsedTransition.conditions,
                                                                collapsedTransition.destinationStateIndex,
                                                                parameters);
            }
        }

        private void FindCollapsedTransitions(AnimatorTransitionBase startTransition,
                                              AnimatorTransitionBase currentTransition,
                                              AnimatorStateMachine currentStateMachine,
                                              List<AnimatorCondition> accumulatedConditions,
                                              List<CollapsedTransition> collapsedTransitions,
                                              ref UnsafeHashMap<UnityObjectRef<AnimatorState>, int> statesIndicesHashMap,
                                              ref UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>,
                                                                UnityObjectRef<AnimatorStateMachine> > stateMachineParentHashMap)
        {
            int addedConditions = currentTransition.conditions.Length;
            accumulatedConditions.AddRange(currentTransition.conditions);

            if (currentTransition.destinationState != null)
            {
                // We found the end state, we can save the collapsed transition using:
                // * The initial transition out of the origin state
                // * The conditions we accumulated recursively
                // * The destination state index in the root state machine's flattened array of states
                collapsedTransitions.Add(new CollapsedTransition
                {
                    transition            = startTransition,
                    conditions            = new List<AnimatorCondition>(accumulatedConditions),
                    destinationStateIndex = statesIndicesHashMap[currentTransition.destinationState],
                });
            }
            else if (currentTransition.destinationStateMachine != null)
            {
                // Transition points to a state machine. We need to iterate through all the entry transitions and the default transition.
                foreach (var destinationMachineEntryTransition in currentTransition.destinationStateMachine.entryTransitions)
                {
                    FindCollapsedTransitions(startTransition,
                                             destinationMachineEntryTransition,
                                             currentTransition.destinationStateMachine,
                                             accumulatedConditions,
                                             collapsedTransitions,
                                             ref statesIndicesHashMap,
                                             ref stateMachineParentHashMap);
                }

                // Add a transition to the destination machine default state if needed too
                if (currentTransition.destinationStateMachine.defaultState != null)
                {
                    collapsedTransitions.Add(new CollapsedTransition
                    {
                        transition            = startTransition,
                        conditions            = new List<AnimatorCondition>(accumulatedConditions),
                        destinationStateIndex = statesIndicesHashMap[currentTransition.destinationStateMachine.defaultState],
                    });
                }
            }
            else
            {
                // Transition points to an exit state in the state machine. We iterate through all the transitions going out of this state machine in the parent state machine.
                if (stateMachineParentHashMap.TryGetValue(currentStateMachine, out UnityObjectRef<AnimatorStateMachine> parentStateMachine))
                {
                    // We are exiting a sub-state machine, we need to iterate through the transitions coming out of this state machine in the parent's state machine
                    foreach (var transitionOutOfThisStateMachine in parentStateMachine.Value.GetStateMachineTransitions(currentStateMachine))
                    {
                        FindCollapsedTransitions(startTransition,
                                                 transitionOutOfThisStateMachine,
                                                 parentStateMachine,
                                                 accumulatedConditions,
                                                 collapsedTransitions,
                                                 ref statesIndicesHashMap,
                                                 ref stateMachineParentHashMap);
                    }
                }
                else
                {
                    // We reached an exit state in the root state machine. We need to iterate through the entry states of the root machine
                    foreach (var rootStateMachineEntryTransition in currentStateMachine.entryTransitions)
                    {
                        FindCollapsedTransitions(startTransition,
                                                 rootStateMachineEntryTransition,
                                                 currentStateMachine,
                                                 accumulatedConditions,
                                                 collapsedTransitions,
                                                 ref statesIndicesHashMap,
                                                 ref stateMachineParentHashMap);
                    }

                    // Add a transition to the default state in the parent machine too
                    if (currentStateMachine.defaultState != null)
                    {
                        collapsedTransitions.Add(new CollapsedTransition
                        {
                            transition            = startTransition,
                            conditions            = new List<AnimatorCondition>(accumulatedConditions),
                            destinationStateIndex = statesIndicesHashMap[currentStateMachine.defaultState],
                        });
                    }
                }
            }

            // Remove any conditions I added in this step so it doesn't affect other branches of this recursive tree of transitions
            if (addedConditions > 0)
            {
                accumulatedConditions.RemoveRange(accumulatedConditions.Count - addedConditions, addedConditions);
            }
        }

        public class CollapsedTransition
        {
            public int destinationStateIndex;
            public AnimatorTransitionBase transition;  // starting transition with all settings to be used for the collapsed transition
            public List<AnimatorCondition> conditions;
        }

        private void CollectStateInfosRecursivelyForStateMachine(ref List<StateInfo> states,
                                                                 ref UnsafeHashMap<UnityObjectRef<AnimatorState>, int> statesIndicesHashMap,
                                                                 ref UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>,
                                                                                   UnityObjectRef<AnimatorStateMachine> > stateMachineParentHashMap,
                                                                 AnimatorStateMachine stateMachine,
                                                                 string prefix = "")
        {
            // Add direct children in current state machine
            foreach (var childState in stateMachine.states)
            {
                statesIndicesHashMap.Add(childState.state, statesIndicesHashMap.Count);

                states.Add(new StateInfo
                {
                    animationState = childState.state,
                    stateMachine   = stateMachine,
                    fullPathName   = prefix + childState.state.name,
                });
            }

            // Process sub state machines recursively
            foreach (var childStateMachine in stateMachine.stateMachines)
            {
                stateMachineParentHashMap.Add(childStateMachine.stateMachine, stateMachine);
                string prefixForChildStateMachine = prefix + childStateMachine.stateMachine.name + ".";
                CollectStateInfosRecursivelyForStateMachine(ref states,
                                                            ref statesIndicesHashMap,
                                                            ref stateMachineParentHashMap,
                                                            childStateMachine.stateMachine,
                                                            prefixForChildStateMachine);
            }
        }

        private MecanimControllerBlob.Layer BakeLayer(ref BlobBuilder builder,
                                                      ref MecanimControllerBlob.Layer layerBlob,
                                                      AnimatorControllerLayer layer,
                                                      short stateMachineIndex,
                                                      short boneMaskIndex,
                                                      UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipsIndicesHashMap,
                                                      UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap)
        {
            layerBlob.name                        = layer.name;
            layerBlob.originalLayerWeight         = layer.defaultWeight;
            layerBlob.performIKPass               = layer.iKPass;
            layerBlob.useAdditiveBlending         = layer.blendingMode == AnimatorLayerBlendingMode.Additive;
            layerBlob.syncLayerUsesBlendedTimings = layer.syncedLayerAffectsTiming;

            layerBlob.stateMachineIndex = stateMachineIndex;
            layerBlob.isSyncLayer       = layer.syncedLayerIndex != -1;
            layerBlob.syncLayerIndex    = (short) layer.syncedLayerIndex;

            layerBlob.boneMaskIndex = boneMaskIndex;

            // Gather all states in the state machine (including nested state machines)
            List<StateInfo>                                   stateInfos           = new List<StateInfo>();
            UnsafeHashMap<UnityObjectRef<AnimatorState>, int> statesIndicesHashMap = new UnsafeHashMap<UnityObjectRef<AnimatorState>,
                                                                                                       int>(10, Allocator.Temp);
            UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>, UnityObjectRef<AnimatorStateMachine> > stateMachineParentHashMap =
                new UnsafeHashMap<UnityObjectRef<AnimatorStateMachine>, UnityObjectRef<AnimatorStateMachine> >(1, Allocator.Temp);
            CollectStateInfosRecursivelyForStateMachine(ref stateInfos, ref statesIndicesHashMap, ref stateMachineParentHashMap, layer.stateMachine);

            // Bake all motion indices for this layer, matching the order of the states in its state machine
            var motionIndicesArrayBuilder = builder.Allocate(ref layerBlob.motionIndices, stateInfos.Count);

            for (var index = 0; index < stateInfos.Count; index++)
            {
                Motion stateMotion = stateInfos[index].animationState.motion;

                if (stateMotion is BlendTree blendTree)
                {
                    motionIndicesArrayBuilder[index] = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = true,
                        index       = (ushort) blendTreeIndicesHashMap[blendTree],
                    };
                }
                else if (stateMotion is AnimationClip animationClip)
                {
                    motionIndicesArrayBuilder[index] = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = false,
                        index       = (ushort) animationClipsIndicesHashMap[animationClip],
                    };
                }
                else
                {
                    motionIndicesArrayBuilder[index] = MecanimControllerBlob.MotionIndex.Invalid;
                }
            }

            return layerBlob;
        }

        private BlobAssetReference<MecanimControllerBlob> BakeAnimatorController(AnimatorController animatorController)
        {
            var builder                = new BlobBuilder(Allocator.Temp);
            ref var blobAnimatorController = ref builder.ConstructRoot<MecanimControllerBlob>();
            blobAnimatorController.name = animatorController.name;

            UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap = new UnsafeHashMap<UnityObjectRef<AnimationClip>, int>(1, Allocator.Temp);
            BuildClipMotionIndicesHashes(animatorController, ref animationClipIndicesHashMap);

            UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap = new UnsafeHashMap<UnityObjectRef<BlendTree>, int>(1, Allocator.Temp);
            BakeBlendTrees(ref builder, ref blobAnimatorController, animatorController, ref blendTreeIndicesHashMap, in animationClipIndicesHashMap);

            BlobBuilderArray<MecanimControllerBlob.StateMachine> stateMachinesBuilder =
                builder.Allocate(ref blobAnimatorController.stateMachines, GetStateMachinesCount(animatorController.layers));
            BlobBuilderArray<MecanimControllerBlob.Layer> layersBuilder =
                builder.Allocate(ref blobAnimatorController.layers, animatorController.layers.Length);

            NativeHashMap<short, short> owningLayerToStateMachine = new NativeHashMap<short, short>(1, Allocator.Temp);

            builder = BakeStateMachines(animatorController, ref owningLayerToStateMachine, ref stateMachinesBuilder, ref builder);

            builder = BakeLayers(animatorController, ref animationClipIndicesHashMap, ref blendTreeIndicesHashMap, ref owningLayerToStateMachine, ref layersBuilder, ref builder);

            BakeParameters(animatorController, ref builder, ref blobAnimatorController);

            var result = builder.CreateBlobAssetReference<MecanimControllerBlob>(Allocator.Persistent);

            return result;
        }

        private void BuildClipMotionIndicesHashes(AnimatorController animatorController, ref UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap)
        {
            // Save indices for each AnimationClip in a hashmap for easy lookups while baking layers and blend trees
            for (var index = 0; index < animatorController.animationClips.Length; index++)
            {
                var clip = animatorController.animationClips[index];
                animationClipIndicesHashMap.TryAdd(clip, index);
            }
        }

        private void BakeBlendTrees(ref BlobBuilder builder,
                                    ref MecanimControllerBlob blobAnimatorController,
                                    AnimatorController animatorController,
                                    ref UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap,
                                    in UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap)
        {
            // Save BlendTrees and their indices for all blend trees for easy lookups while baking layers and blend trees
            foreach (var layer in animatorController.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    Motion stateMotion = state.state.motion;

                    if (stateMotion is BlendTree blendTree)
                    {
                        AddBlendTreesToIndicesHashMapRecursively(blendTree, ref blendTreeIndicesHashMap);
                    }
                }
            }

            BlobBuilderArray<MecanimControllerBlob.BlendTree> blendTreesBuilder = builder.Allocate(ref blobAnimatorController.blendTrees,
                                                                                                   blendTreeIndicesHashMap.Count);

            // Now bake all the blend trees using the populated blend tree hashmap
            foreach (var keyPair in blendTreeIndicesHashMap)
            {
                BlendTree blendTree      = keyPair.Key;
                int blendTreeIndex = keyPair.Value;

                BakeBlendTree(animatorController, blendTree, blendTreeIndex, ref builder, ref blendTreesBuilder, in blendTreeIndicesHashMap, in animationClipIndicesHashMap);
            }
        }

        private void BakeBlendTree(AnimatorController animatorController,
                                   BlendTree blendTree,
                                   int blendTreeIndex,
                                   ref BlobBuilder builder,
                                   ref BlobBuilderArray<MecanimControllerBlob.BlendTree> blendTreesBuilder,
                                   in UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap,
                                   in UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap)
        {
            ref MecanimControllerBlob.BlendTree blendTreeBlob = ref blendTreesBuilder[blendTreeIndex];

            blendTreeBlob.blendTreeType = MecanimControllerBlob.BlendTree.FromUnityBlendTreeType(blendTree.blendType);

            Span<short>                                 parameterIndices      = stackalloc short[math.max(2, blendTree.children.Length)];
            int parameterIndicesCount = 0;
            Span<MecanimControllerBlob.BlendTree.Child> children              = stackalloc MecanimControllerBlob.BlendTree.Child[blendTree.children.Length];
            int childCount            = 0;

            // Bake blend tree parameters for non-direct types
            bool useThresholds = false;
            if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.Simple1D)
            {
                animatorController.parameters.TryGetParameter(blendTree.blendParameter, out short conditionParameterIndex);
                parameterIndices[parameterIndicesCount] = conditionParameterIndex;
                parameterIndicesCount++;
                useThresholds = true;
            }
            else if (blendTreeBlob.blendTreeType != MecanimControllerBlob.BlendTree.BlendTreeType.Direct)
            {
                animatorController.parameters.TryGetParameter(blendTree.blendParameter,  out short conditionParameterIndex);
                parameterIndices[parameterIndicesCount] = conditionParameterIndex;
                parameterIndicesCount++;
                animatorController.parameters.TryGetParameter(blendTree.blendParameterY, out short conditionParameterIndexY);
                parameterIndices[parameterIndicesCount] = conditionParameterIndexY;
                parameterIndicesCount++;
            }

            // Bake blend tree children and direct parameters
            for (var childIndex = 0; childIndex < blendTree.children.Length; childIndex++)
            {
                var childMotion = blendTree.children[childIndex];
                MecanimControllerBlob.BlendTree.Child childBlob   = default;

                childBlob.cycleOffset = childMotion.cycleOffset;
                childBlob.mirrored    = childMotion.mirror;
                childBlob.position    = useThresholds ? childMotion.threshold : childMotion.position;
                childBlob.timeScale   = childMotion.timeScale;

                // TODO: childBlob.isLooping  // This doesn't seem to be available in childMotion data

                // Set child motion indices for a blend tree or an animation clip
                if (childMotion.motion is BlendTree childBlendTree)
                {
                    // Link to a blend tree
                    childBlob.motionIndex = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = true,
                        index       = (ushort)blendTreeIndicesHashMap[childBlendTree],
                    };
                }
                else if (childMotion.motion is AnimationClip childAnimationClip)
                {
                    // Link to an animation clip
                    childBlob.motionIndex = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = false,
                        index       = (ushort)animationClipIndicesHashMap[childAnimationClip],
                    };
                }
                else
                    continue;

                // Bake the parameter index for each child into the parameters array if the blend tree is Direct type
                if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.Direct)
                {
                    animatorController.parameters.TryGetParameter(childMotion.directBlendParameter, out short childParameterIndex);
                    parameterIndices[parameterIndicesCount] = childParameterIndex;
                    parameterIndicesCount++;
                }

                children[childCount] = childBlob;
                childCount++;
            }

            var childrenBuilder = builder.Allocate(ref blendTreeBlob.children, childCount);
            for (int i = 0; i < childCount; i++)
                childrenBuilder[i] = children[i];

            var parameterBuilder = builder.Allocate(ref blendTreeBlob.parameterIndices, parameterIndicesCount);
            for (int i = 0; i < parameterIndicesCount; i++)
                parameterBuilder[i] = parameterIndices[i];

            if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.FreeformDirectional2D ||
                blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.FreeformCartesian2D)
            {
                // See https://runevision.com/thesis/rune_skovbo_johansen_thesis.pdf at 6.3 (p58) for details.
                // Because atan2 is expensive at runtime for FreeformDirectional2D, and we need to do it with O(n^2) complexity,
                // we precompute the results in the blob.
                var pipjBuilder = builder.Allocate(ref blendTreeBlob.pipjs, childrenBuilder.Length * (childrenBuilder.Length - 1));
                for (int i = 0; i < childCount - 1; i++)
                {
                    var pi    = childrenBuilder[i].position;
                    var pimag = math.length(pi);

                    for (int j = i + 1; j < childCount; j++)
                    {
                        var pj   = childrenBuilder[j].position;
                        float4 pipj = default;
                        if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.FreeformDirectional2D)
                        {
                            var pjmag = math.length(pj);
                            pipj.x = (pjmag - pimag) / (0.5f * (pimag + pjmag));
                            var direction = LatiosMath.ComplexMul(pi, new float2(pj.x, -pj.y));
                            var directionAtan = math.select(math.atan2(direction.y, direction.x), 0, pi.Equals(float2.zero) || pj.Equals(float2.zero));
                            pipj.y = MecanimControllerBlob.BlendTree.kFreeformDirectionalBias * directionAtan;
                            pipj.w = 1f / (0.5f * (pimag + pjmag));
                        }
                        else
                        {
                            pipj.xy = pj - pi;
                            pipj.w  = 1f;
                        }
                        pipj.z                                                                   = 1f / math.lengthsq(pipj.xy);
                        pipjBuilder[MecanimControllerBlob.BlendTree.PipjIndex(i, j, childCount)] = pipj;
                        pipjBuilder[MecanimControllerBlob.BlendTree.PipjIndex(j, i, childCount)] = pipj * new float4(-1f, -1f, 1f, 1f);
                    }
                }
            }
            else if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.SimpleDirectional2D)
            {
                var pipjBuilder = builder.Allocate(ref blendTreeBlob.pipjs, childCount);
                for (int i = 0; i < childCount; i++)
                {
                    var pos = childrenBuilder[i].position;
                    pipjBuilder[i] = new float4(math.atan2(pos.y, pos.x), 0f, 0f, 0f);
                    if (pos.Equals(float2.zero))
                        pipjBuilder[0].y = math.asfloat(i);
                    else if (i == 0)
                        pipjBuilder[0].y = math.asfloat(-1);
                }
            }
        }

        private void AddBlendTreesToIndicesHashMapRecursively(BlendTree blendTree, ref UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap)
        {
            // If we already have this blend tree in our hashmap, we don't need to process it again
            if (!blendTreeIndicesHashMap.TryAdd(blendTree, blendTreeIndicesHashMap.Count))
                return;

            foreach (var blendTreeChild in blendTree.children)
            {
                if (blendTreeChild.motion is BlendTree childBlendTree)
                {
                    AddBlendTreesToIndicesHashMapRecursively(childBlendTree, ref blendTreeIndicesHashMap);
                }
            }
        }

        private BlobBuilder BakeLayers(AnimatorController animatorController,
                                       ref UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap,
                                       ref UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap,
                                       ref NativeHashMap<short, short> owningLayerToStateMachine,
                                       ref BlobBuilderArray<MecanimControllerBlob.Layer> layersBuilder,
                                       ref BlobBuilder builder)
        {
            short boneMasksFound = 0;
            for (short i = 0; i < animatorController.layers.Length; i++)
            {
                var layer = animatorController.layers[i];

                // Get the state machine index using the current layer index, or the layer index we are syncing with
                short stateMachineIndex = owningLayerToStateMachine[(short) (layer.syncedLayerIndex == -1 ? i : layer.syncedLayerIndex)];

                short boneMaskIndex = -1;
                if (layer.avatarMask != null)
                {
                    boneMaskIndex = boneMasksFound;
                    boneMasksFound++;
                }

                BakeLayer(ref builder, ref layersBuilder[i], layer, stateMachineIndex, boneMaskIndex, animationClipIndicesHashMap, blendTreeIndicesHashMap);
            }

            return builder;
        }

        private int GetStateMachinesCount(AnimatorControllerLayer[] animatorControllerLayers)
        {
            int stateMachinesCount = 0;
            foreach (var animatorControllerLayer in animatorControllerLayers)
            {
                if (animatorControllerLayer.syncedLayerIndex == -1)
                {
                    stateMachinesCount++;
                }
            }

            return stateMachinesCount;
        }

        private BlobBuilder BakeStateMachines(
            AnimatorController animatorController,
            ref NativeHashMap<short, short> owningLayerToStateMachine,
            ref BlobBuilderArray<MecanimControllerBlob.StateMachine> stateMachinesBuilder,
            ref BlobBuilder builder)
        {
            NativeParallelMultiHashMap<short, short> layersInfluencingTimingsByAffectedLayer = new NativeParallelMultiHashMap<short, short>(1, Allocator.Temp);

            short stateMachinesAdded = 0;
            for (short i = 0; i < animatorController.layers.Length; i++)
            {
                var layer = animatorController.layers[i];

                if (layer.syncedLayerIndex == -1)
                {
                    // Not a synced layer, add new state machine
                    BakeStateMachine(ref builder, ref stateMachinesBuilder[stateMachinesAdded], layer, animatorController.parameters);

                    // Associate the index of this layer with its state machine index for easier lookups
                    owningLayerToStateMachine.Add(i, stateMachinesAdded);

                    stateMachinesAdded++;
                }
                else if (layer.syncedLayerAffectsTiming)
                {
                    // This is a synced layer that affects timings. Save it as an influencing layer for the layer it's syncing with.
                    layersInfluencingTimingsByAffectedLayer.Add((short) layer.syncedLayerIndex, i);
                }
            }

            PopulateLayersAffectingStateMachinesTimings(animatorController, owningLayerToStateMachine, ref stateMachinesBuilder, builder, layersInfluencingTimingsByAffectedLayer);

            return builder;
        }

        private static unsafe void PopulateLayersAffectingStateMachinesTimings(
            AnimatorController animatorController,
            NativeHashMap<short, short> owningLayerToStateMachine,
            ref BlobBuilderArray<MecanimControllerBlob.StateMachine> stateMachinesBuilder,
            BlobBuilder builder,
            NativeParallelMultiHashMap<short, short> layersInfluencingTimingsByAffectedLayer)
        {
            // Populate list of layers that affects each state machine timings.
            for (short i = 0; i < animatorController.layers.Length; i++)
            {
                if (!owningLayerToStateMachine.ContainsKey(i))
                    continue;

                int influencingLayersCount = layersInfluencingTimingsByAffectedLayer.CountValuesForKey(i);

                // Get associated state machine blob
                var stateMachineForThisLayer = owningLayerToStateMachine[i];
                ref var stateMachineBlob         = ref stateMachinesBuilder[stateMachineForThisLayer];

                // initialize the blob array for layers affecting timings on the state machine blob
                BlobBuilderArray<short> influencingLayersBuilder = builder.Allocate(ref stateMachineBlob.influencingLayers, influencingLayersCount + 1);

                influencingLayersBuilder[0] = i;

                var indexInArrayOfInfluencingLayers = 1;
                if (layersInfluencingTimingsByAffectedLayer.TryGetFirstValue(i, out short influencingLayer, out NativeParallelMultiHashMapIterator<short> it))
                {
                    do
                    {
                        influencingLayersBuilder[indexInArrayOfInfluencingLayers] = influencingLayer;
                        indexInArrayOfInfluencingLayers++;
                    }
                    while (layersInfluencingTimingsByAffectedLayer.TryGetNextValue(out influencingLayer, ref it));
                }

                NativeSortExtension.Sort((short*)influencingLayersBuilder.GetUnsafePtr(), influencingLayersBuilder.Length);

                stateMachinesBuilder[stateMachineForThisLayer] = stateMachineBlob;
            }
        }

        private static void BakeParameters(AnimatorController animatorController, ref BlobBuilder builder, ref MecanimControllerBlob blobAnimatorController)
        {
            var parametersCount = animatorController.parameters.Length;

            blobAnimatorController.parameterTypes = new MecanimControllerBlob.ParameterTypes();
            BlobBuilderArray<int> packedTypes =
                builder.Allocate(
                    ref blobAnimatorController.parameterTypes.packedTypes,
                    MecanimControllerBlob.ParameterTypes.PackedTypesArrayLength(parametersCount));

            BlobBuilderArray<int>                parameterNameHashes       = builder.Allocate(ref blobAnimatorController.parameterNameHashes, parametersCount);
            BlobBuilderArray<int>                parameterEditorNameHashes = builder.Allocate(ref blobAnimatorController.parameterEditorNameHashes, parametersCount);
            BlobBuilderArray<FixedString64Bytes> parameterNames            = builder.Allocate(ref blobAnimatorController.parameterNames, parametersCount);

            // Bake parameter types names and hashes
            for (int i = 0; i < parametersCount; i++)
            {
                var parameter = animatorController.parameters[i];

                int nameHash = parameter.name.GetHashCode();

                MecanimControllerBlob.ParameterTypes.PackTypeIntoBlobBuilder(ref packedTypes, i, parameter.type);

                parameterNameHashes[i]       = nameHash;
                parameterEditorNameHashes[i] = nameHash;
                parameterNames[i]            = parameter.name;
            }
        }

        private class StateInfo
        {
            public AnimatorState animationState;
            public AnimatorStateMachine stateMachine;
            public string fullPathName;
        }
    }
}
#endif

