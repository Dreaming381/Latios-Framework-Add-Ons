﻿#if UNITY_EDITOR
using System.Collections.Generic;
using Latios.Authoring;
using Latios.Authoring.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.LowLevel.Unsafe;
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
            blobAnimatorCondition.mode      = (MecanimControllerBlob.Condition.ConditionType) condition.mode;

            parameters.TryGetParameter(condition.parameter, out short conditionParameterIndex);
            blobAnimatorCondition.parameterIndex = conditionParameterIndex;
            blobAnimatorCondition.compareValue = new MecanimParameter { floatParam = condition.threshold };
        }

        private void BakeAnimatorStateTransition(ref BlobBuilder builder,
            ref MecanimControllerBlob.Transition blobTransition,
            AnimatorTransitionBase transition,
            int destinationStateIndex, 
            AnimatorControllerParameter[] parameters)
        {
            blobTransition.destinationStateIndex = (short) destinationStateIndex;
            
            if (transition is AnimatorStateTransition animatorStateTransition)
            {
                blobTransition.hasExitTime = animatorStateTransition.hasExitTime;
                blobTransition.normalizedExitTime = animatorStateTransition.exitTime / animatorStateTransition.duration;
                blobTransition.normalizedOffset = animatorStateTransition.offset / animatorStateTransition.duration;
                blobTransition.duration = animatorStateTransition.duration;
                blobTransition.interruptionSource = (MecanimControllerBlob.Transition.InterruptionSource) animatorStateTransition.interruptionSource;
                blobTransition.usesOrderedInterruptions = animatorStateTransition.orderedInterruption;
            }

            // Bake conditions
            BlobBuilderArray<MecanimControllerBlob.Condition> conditionsBuilder =
                builder.Allocate(ref blobTransition.conditions, transition.conditions.Length);
            
            for (int i = 0; i < transition.conditions.Length; i++)
            {
                BakeAnimatorCondition(ref conditionsBuilder[i], transition.conditions[i], parameters);
            }
        }

        private static int FindDestinationStateIndexForTransition(AnimatorTransitionBase transition, List<StateInfo> states)
        {
            AnimatorState transitionDestinationState = transition.destinationState;
            
            // If it doesn't have a destination state, it's pointing to a state machine and we can get its default state
            if (transition.destinationState == null)
            {
                transitionDestinationState = transition.destinationStateMachine.defaultState;
            }
            
            var destinationStateIndex = -1;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].animationState == transitionDestinationState)
                {
                    destinationStateIndex = i;
                    break;
                }
            }

            return destinationStateIndex;
        }
        
        private static int FindStateIndexForState(AnimatorState state, List<StateInfo> states)
        {
            var destinationStateIndex = -1;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].animationState == state)
                {
                    destinationStateIndex = i;
                    break;
                }
            }
            return destinationStateIndex;
        }


        private void BakeState(
            ref MecanimControllerBlob.State blobState,
            short stateIndexInStateMachine,
            AnimatorState state,
            AnimatorControllerParameter[] parameters)
        {
            blobState.baseStateSpeed                = state.speed;
            blobState.motionCycleOffset             = state.cycleOffset;
            
            blobState.useMirror                     = state.mirror;
            blobState.useFootIK                     = state.iKOnFeet;

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
                                                                       AnimatorControllerParameter[]  parameters)
        {
            // Gather all states in the state machine (including nested state machines)
            List<StateInfo> stateInfos = new List<StateInfo>();
            CollectStateInfosRecursivelyForStateMachine(ref stateInfos, layer.stateMachine);

            //States
            BlobBuilderArray<MecanimControllerBlob.State> statesBuilder =
                builder.Allocate(ref stateMachineBlob.states, stateInfos.Count);
            
            BlobBuilderArray<int> stateNameHashesBuilder =
                builder.Allocate(ref stateMachineBlob.stateNameHashes, stateInfos.Count);
            BlobBuilderArray<int> stateNameEditorHashesBuilder =
                builder.Allocate(ref stateMachineBlob.stateNameEditorHashes, stateInfos.Count);
            BlobBuilderArray<FixedString128Bytes> stateNamesBuilder =
                builder.Allocate(ref stateMachineBlob.stateNames, stateInfos.Count);
            BlobBuilderArray<FixedString128Bytes> stateTagsBuilder =
                builder.Allocate(ref stateMachineBlob.stateTags, stateInfos.Count);
            
            for (short i = 0; i < stateInfos.Count; i++)
            {
                BakeState(ref statesBuilder[i], i, stateInfos[i].animationState, parameters);

                stateNameHashesBuilder[i] = stateInfos[i].fullPathName.GetHashCode();
                stateNameEditorHashesBuilder[i] = stateInfos[i].fullPathName.GetHashCode();
                stateNamesBuilder[i] = stateInfos[i].fullPathName;
                stateTagsBuilder[i] = stateInfos[i].animationState.tag;
            }
            
            // TODO: bake all transitions (collapsing exits and entries and logging permutations as needed)
            
            
            // Entry state transitions
            short defaultStateIndex = (short)FindStateIndexForState(layer.stateMachine.defaultState, stateInfos);
            BlobBuilderArray<MecanimControllerBlob.Transition> entryTransitionsBuilder =
                builder.Allocate(ref stateMachineBlob.initializationEntryStateTransitions, layer.stateMachine.entryTransitions.Length + 1); // we allocate one more for the default in position 0

            // Add a dummy transition to the default state in position 0 of the array
            entryTransitionsBuilder[0].destinationStateIndex = defaultStateIndex;
            
            for (int i = 0; i < layer.stateMachine.entryTransitions.Length; i++)
            {
                AnimatorTransition stateMachineEntryTransition = layer.stateMachine.entryTransitions[i];
                
                int stateDestinationIndex = FindDestinationStateIndexForTransition(stateMachineEntryTransition, stateInfos);
                Debug.Log(stateDestinationIndex);
                
                // Default state dummy transition is in index 0. We add the rest of the entry transitions after that one.
                BakeAnimatorStateTransition(ref builder, ref entryTransitionsBuilder[i+1], stateMachineEntryTransition, stateDestinationIndex, parameters);
            }
            
            // Any state transitions
            BlobBuilderArray<MecanimControllerBlob.Transition> anyStateTransitionsBuilder =
                builder.Allocate(ref stateMachineBlob.anyStateTransitions, layer.stateMachine.anyStateTransitions.Length);
            for (int i = 0; i < layer.stateMachine.anyStateTransitions.Length; i++)
            {
                int stateDestinationIndex = FindDestinationStateIndexForTransition(layer.stateMachine.anyStateTransitions[i], stateInfos);
                
                BakeAnimatorStateTransition(ref builder, ref anyStateTransitionsBuilder[i], layer.stateMachine.anyStateTransitions[i], stateDestinationIndex, parameters);
            }

            return stateMachineBlob;
        }

        private void CollectStateInfosRecursivelyForStateMachine(ref List<StateInfo> states, AnimatorStateMachine stateMachine, string prefix = "")
        {
            // Add direct children in current state machine
            foreach (var childState in stateMachine.states)
            {
                states.Add(new StateInfo
                {
                    animationState = childState.state,
                    fullPathName = prefix + childState.state.name, 
                });
            }
            
            // Process sub state machines recursively
            if (stateMachine.stateMachines.Length == 0) return;
            
            foreach (var childStateMachine in stateMachine.stateMachines)
            {
                string prefixForChildStateMachine = prefix + childStateMachine.stateMachine.name + ".";
                CollectStateInfosRecursivelyForStateMachine(ref states, childStateMachine.stateMachine, prefixForChildStateMachine);
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

            layerBlob.stateMachineIndex           = stateMachineIndex;
            layerBlob.isSyncLayer                 = layer.syncedLayerIndex != -1;
            layerBlob.syncLayerIndex              = (short) layer.syncedLayerIndex;

            layerBlob.boneMaskIndex = boneMaskIndex;
            
            // Gather all states in the state machine (including nested state machines)
            List<StateInfo> stateInfos = new List<StateInfo>();
            CollectStateInfosRecursivelyForStateMachine(ref stateInfos, layer.stateMachine);

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
                        index = (ushort) blendTreeIndicesHashMap[blendTree],
                    };
                }
                else if (stateMotion is AnimationClip animationClip)
                {
                    motionIndicesArrayBuilder[index] = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = false,
                        index = (ushort) animationClipsIndicesHashMap[animationClip],
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
            var builder                    = new BlobBuilder(Allocator.Temp);
            ref var blobAnimatorController = ref builder.ConstructRoot<MecanimControllerBlob>();
            blobAnimatorController.name    = animatorController.name;

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
            foreach (var clip in animatorController.animationClips)
            {
                animationClipIndicesHashMap.TryAdd(clip, animationClipIndicesHashMap.Count);
            }
        }
        
        private void BakeBlendTrees(ref BlobBuilder builder, ref MecanimControllerBlob blobAnimatorController, AnimatorController animatorController, ref UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap, in UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap)
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

            BlobBuilderArray<MecanimControllerBlob.BlendTree> blendTreesBuilder = builder.Allocate(ref blobAnimatorController.blendTrees, blendTreeIndicesHashMap.Count);
            
            // Now bake all the blend trees using the populated blend tree hashmap
            foreach (var keyPair in blendTreeIndicesHashMap)
            {
                BlendTree blendTree = keyPair.Key;
                int blendTreeIndex = keyPair.Value;
                
                BakeBlendTree(animatorController, blendTree, blendTreeIndex, ref builder, ref blendTreesBuilder, in blendTreeIndicesHashMap, in animationClipIndicesHashMap);
            }
        }

        private void BakeBlendTree(AnimatorController animatorController, BlendTree blendTree, int blendTreeIndex, ref BlobBuilder builder, ref BlobBuilderArray<MecanimControllerBlob.BlendTree> blendTreesBuilder, in UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap, in UnsafeHashMap<UnityObjectRef<AnimationClip>, int> animationClipIndicesHashMap)
        {
            ref MecanimControllerBlob.BlendTree blendTreeBlob = ref blendTreesBuilder[blendTreeIndex];
            
            blendTreeBlob.blendTreeType = MecanimControllerBlob.BlendTree.FromUnityBlendTreeType(blendTree.blendType);
            
            // Bake blend tree parameters
            NativeList<short> parameterIndices = new NativeList<short>(1, Allocator.Temp);
            if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.Simple1D)
            {
                animatorController.parameters.TryGetParameter(blendTree.blendParameter, out short conditionParameterIndex);
                parameterIndices.Add(conditionParameterIndex);
            }
            else if (blendTreeBlob.blendTreeType != MecanimControllerBlob.BlendTree.BlendTreeType.Direct)
            {
                animatorController.parameters.TryGetParameter(blendTree.blendParameter, out short conditionParameterIndex);
                parameterIndices.Add(conditionParameterIndex);
                animatorController.parameters.TryGetParameter(blendTree.blendParameterY, out short conditionParameterIndexY);
                parameterIndices.Add(conditionParameterIndexY);
            }
            
            var childrenBuilder = builder.Allocate(ref blendTreeBlob.children, blendTree.children.Length);

            for (var childIndex = 0; childIndex < blendTree.children.Length; childIndex++)
            {
                var childMotion = blendTree.children[childIndex];
                MecanimControllerBlob.BlendTree.Child childBlob = childrenBuilder[childIndex];

                childBlob.cycleOffset = childMotion.cycleOffset;
                childBlob.mirrored = childMotion.mirror;
                childBlob.position = childMotion.position;
                childBlob.timeScale = childMotion.timeScale;
                childBlob.threshold = childMotion.threshold;

                // TODO: childBlob.isLooping  // This doesn't seem to be available in childMotion data

                // Bake the parameter index for each child into the parameters array if the blend tree is Direct type
                if (blendTreeBlob.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.Direct)
                {
                    animatorController.parameters.TryGetParameter(childMotion.directBlendParameter, out short childParameterIndex);
                    parameterIndices.Add(childParameterIndex);
                }
                
                // Set child motion indices for a blend tree or an animation clip
                if (childMotion.motion is BlendTree childBlendTree)
                {
                    // Link to a blend tree
                    childBlob.motionIndex = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = true,
                        index = (ushort)blendTreeIndicesHashMap[childBlendTree],
                    };
                }
                else if (childMotion.motion is AnimationClip childAnimationClip)
                {
                    // Link to an animation clip
                    childBlob.motionIndex = new MecanimControllerBlob.MotionIndex
                    {
                        isBlendTree = false,
                        index = (ushort)animationClipIndicesHashMap[childAnimationClip],
                    };
                }

                childrenBuilder[childIndex] = childBlob;
            }

            builder.ConstructFromNativeArray(ref blendTreeBlob.parameterIndices, parameterIndices.ToArray(Allocator.Temp));
        }

        private void AddBlendTreesToIndicesHashMapRecursively(BlendTree blendTree, ref UnsafeHashMap<UnityObjectRef<BlendTree>, int> blendTreeIndicesHashMap)
        {
            // If we already have this blend tree in our hashmap, we don't need to process it again
            if (!blendTreeIndicesHashMap.TryAdd(blendTree, blendTreeIndicesHashMap.Count)) return;
            
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
                } else if (layer.syncedLayerAffectsTiming)
                {
                    // This is a synced layer that affects timings. Save it as an influencing layer for the layer it's syncing with.
                    layersInfluencingTimingsByAffectedLayer.Add((short) layer.syncedLayerIndex, i);
                }
            }
            
            PopulateLayersAffectingStateMachinesTimings(animatorController, owningLayerToStateMachine, ref stateMachinesBuilder, builder, layersInfluencingTimingsByAffectedLayer);
            
            return builder;
        }

        private static void PopulateLayersAffectingStateMachinesTimings(
            AnimatorController animatorController,
            NativeHashMap<short, short> owningLayerToStateMachine,
            ref BlobBuilderArray<MecanimControllerBlob.StateMachine> stateMachinesBuilder,
            BlobBuilder builder,
            NativeParallelMultiHashMap<short, short> layersInfluencingTimingsByAffectedLayer)
        {
            // Populate list of layers that affects each state machine timings.
            for (short i = 0; i < animatorController.layers.Length; i++)
            {
                if (!owningLayerToStateMachine.ContainsKey(i)) continue;

                int influencingLayersCount = layersInfluencingTimingsByAffectedLayer.CountValuesForKey(i);
                if (influencingLayersCount == 0) continue;

                // Get associated state machine blob
                ref var stateMachineBlob = ref stateMachinesBuilder[owningLayerToStateMachine[i]];

                // initialize the blob array for layers affecting timings on the state machine blob
                BlobBuilderArray<short> influencingLayersBuilder = builder.Allocate(ref stateMachineBlob.influencingSyncLayers, influencingLayersCount);

                var indexInArrayOfInfluencingLayers = 0;
                if (layersInfluencingTimingsByAffectedLayer.TryGetFirstValue(i, out short influencingLayer, out NativeParallelMultiHashMapIterator<short> it))
                {
                    do
                    {
                        influencingLayersBuilder[indexInArrayOfInfluencingLayers] = influencingLayer;
                        indexInArrayOfInfluencingLayers++;
                    } while (layersInfluencingTimingsByAffectedLayer.TryGetNextValue(out influencingLayer, ref it));
                }

                stateMachinesBuilder[owningLayerToStateMachine[i]] = stateMachineBlob;
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

            BlobBuilderArray<int> parameterNameHashes = builder.Allocate(ref blobAnimatorController.parameterNameHashes, parametersCount);
            BlobBuilderArray<int> parameterEditorNameHashes = builder.Allocate(ref blobAnimatorController.parameterEditorNameHashes, parametersCount);
            BlobBuilderArray<FixedString64Bytes> parameterNames = builder.Allocate(ref blobAnimatorController.parameterNames, parametersCount);
            
            // Bake parameter types names and hashes
            for (int i = 0; i < parametersCount; i++)
            {
                var parameter = animatorController.parameters[i];

                int nameHash = parameter.name.GetHashCode();

                MecanimControllerBlob.ParameterTypes.PackTypeIntoBlobBuilder(ref packedTypes, i, parameter.type);
                
                parameterNameHashes[i] = nameHash;
                parameterEditorNameHashes[i] = nameHash;
                parameterNames[i] = parameter.name;
            }
        }

        private class StateInfo
        {
            public AnimatorState animationState;
            public string fullPathName;
        }
    }
}
#endif