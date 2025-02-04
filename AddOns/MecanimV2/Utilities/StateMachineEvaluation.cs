using System;
using Latios.Kinemation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using Blob = Latios.MecanimV2.MecanimControllerBlob;

namespace Latios.MecanimV2
{
    public static class StateMachineEvaluation
    {
        // Todo: State machine updates are trickier than they seem at first, because with a large enough timestep,
        // we can transition through multiple states in a single update. Root motion needs to know all these passages
        // in order to work correctly, otherwise we can get acceleration jumps on transitions that don't make sense.
        // The big question here is whether we also need to broadcast state enter/exit/update events in-time, perhaps
        // in a way that allows for parameters to be modified. And how might such events interact with code-driven
        // state transitions, if at all?
        //
        // For now, we will simply evaluate the entire timestep without interruptions.

        public struct StatePassage
        {
            public float currentStateStartTime;
            public float currentStateEndTime;
            public float nextStateStartTime;
            public float nextStateEndTime;
            public float fractionOfDeltaTimeInState;
            public float transitionProgress;
            public short currentState;
            public short nextState;
        }

        public static void Evaluate(ref MecanimStateMachineActiveStates state,
                                    ref Blob controllerBlob,
                                    ref SkeletonClipSetBlob clipsBlob,
                                    int stateMachineIndex,
                                    float scaledDeltaTime,
                                    ReadOnlySpan<float>                 layerWeights,
                                    ReadOnlySpan<MecanimParameter>      parameters,
                                    Span<BitField64>                    triggersToReset,
                                    Span<StatePassage>                  outputPassages,
                                    out int outputPassagesCount,
                                    out float newInertialBlendProgress)
        {
            outputPassagesCount      = 0;
            newInertialBlendProgress = -1f;

            Span<BitField64> localTriggersToReset = stackalloc BitField64[triggersToReset.Length];
            localTriggersToReset.Clear();

            if (state.currentStateIndex < 0)
                state = SetupInitialState(ref controllerBlob, stateMachineIndex, parameters, localTriggersToReset);

            var                       normalizedDeltaTimeRemaining = 1f;
            ref var                   stateMachineBlob             = ref controllerBlob.stateMachines[stateMachineIndex];
            Span<CachedStateDuration> cachedStateDurations         = stackalloc CachedStateDuration[outputPassages.Length * 2];
            int                       cachedStateDurationsCount    = 0;

            while (normalizedDeltaTimeRemaining > 0f && outputPassagesCount < outputPassages.Length)
            {
                if (state.nextStateTransitionIndex.invalid)
                {
                    // There's no crossfades right now. Try to find an initial transition.
                    ref var stateBlob            = ref stateMachineBlob.states[state.currentStateIndex];
                    float   currentStateDuration = -1f;

                    // Loop twice, first over local state transitions, and then through anyState transitions
                    for (int transitionArrayType = 0; transitionArrayType < 2; transitionArrayType++)
                    {
                        ref var transitions = ref (transitionArrayType == 0 ? ref stateBlob.transitions : ref stateMachineBlob.anyStateTransitions);
                        for (int i = 0; i < transitions.Length; i++)
                        {
                            ref var transition = ref transitions[i];
                            if (!MatchesConditions(ref transition.conditions, ref controllerBlob.parameterTypes, parameters, localTriggersToReset))
                                continue;

                            float stateNormalizedEndTime = 0f;
                            if (transition.hasExitTime)
                            {
                                if (currentStateDuration < 0f)
                                {
                                    EvaluateAndCacheDuration(ref controllerBlob,
                                                             ref clipsBlob,
                                                             parameters,
                                                             layerWeights,
                                                             stateMachineIndex,
                                                             state.currentStateIndex,
                                                             ref currentStateDuration,
                                                             ref cachedStateDurationsCount,
                                                             cachedStateDurations);
                                    stateNormalizedEndTime = state.currentStateNormalizedTime + normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                                }

                                var exitTime = transition.normalizedExitTime;
                                if (math.abs(state.currentStateNormalizedTime) > exitTime || math.abs(stateNormalizedEndTime) < exitTime)
                                    continue;
                            }

                            // This is our new transition
                            ConsumeTriggers(ref transition.conditions, ref controllerBlob.parameterTypes, localTriggersToReset);

                            float fractionOfDeltaTimeInState = 0f;
                            if (transition.hasExitTime)
                            {
                                var stateTime              = transition.normalizedExitTime - math.abs(state.currentStateNormalizedTime);
                                var stateDeltaTime         = stateTime * currentStateDuration;
                                fractionOfDeltaTimeInState = stateDeltaTime / math.abs(scaledDeltaTime);
                            }

                            var passage = new StatePassage
                            {
                                currentState               = state.currentStateIndex,
                                nextState                  = -1,
                                currentStateStartTime      = state.currentStateNormalizedTime,
                                currentStateEndTime        = transition.hasExitTime ? transition.normalizedExitTime : 0f,
                                fractionOfDeltaTimeInState = fractionOfDeltaTimeInState,
                                nextStateStartTime         = 0f,
                                nextStateEndTime           = 0f,
                                transitionProgress         = 0f,
                            };
                            outputPassages[outputPassagesCount] = passage;
                            outputPassagesCount++;

                            state.currentStateNormalizedTime = passage.currentStateEndTime;
                            state.nextStateNormalizedTime    = 0f;
                            state.transitionNormalizedTime   = 0f;
                            state.nextStateTransitionIndex   = new Blob.TransitionIndex { index = (ushort)i, isAnyStateTransition = transitionArrayType == 1 };

                            normalizedDeltaTimeRemaining -= fractionOfDeltaTimeInState;
                            transitionArrayType           = 2;

                            break;  // Transitions loop
                        }
                    }

                    if (state.nextStateTransitionIndex.invalid)
                    {
                        // There were no transitions. Just play the time in the state.
                        float stateDuration = 0f;
                        EvaluateAndCacheDuration(ref controllerBlob,
                                                 ref clipsBlob,
                                                 parameters,
                                                 layerWeights,
                                                 stateMachineIndex,
                                                 state.currentStateIndex,
                                                 ref stateDuration,
                                                 ref cachedStateDurationsCount,
                                                 cachedStateDurations);
                        var stateNormalizedEndTime = state.currentStateNormalizedTime + normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                        var passage                = new StatePassage
                        {
                            currentState               = state.currentStateIndex,
                            nextState                  = -1,
                            currentStateStartTime      = state.currentStateNormalizedTime,
                            currentStateEndTime        = stateNormalizedEndTime,
                            nextStateStartTime         = 0f,
                            nextStateEndTime           = 0f,
                            transitionProgress         = 0f,
                            fractionOfDeltaTimeInState = normalizedDeltaTimeRemaining
                        };
                        outputPassages[outputPassagesCount] = passage;
                        outputPassagesCount++;

                        state.currentStateNormalizedTime = stateNormalizedEndTime;
                        normalizedDeltaTimeRemaining     = 0f;
                    }
                }
                else
                {
                    // Todo: Evaluate transition interruptions (and update to latest inertial blend progression), then transition expirations, and finally transition progression.
                    throw new NotImplementedException();
                }
            }

            // Write back triggers we consumed
            for (int i = 0; i < triggersToReset.Length; i++)
            {
                triggersToReset[i].Value |= localTriggersToReset[i].Value;
            }
        }

        struct CachedStateDuration
        {
            public int   stateIndex;
            public float duration;
        }

        public static MecanimStateMachineActiveStates SetupInitialState(ref Blob controllerBlob,
                                                                 int stateMachineIndex,
                                                                 ReadOnlySpan<MecanimParameter> parameters,
                                                                 Span<BitField64>               triggersToReset)
        {
            ref var stateMachine = ref controllerBlob.stateMachines[stateMachineIndex];
            ref var candidates   = ref stateMachine.initializationEntryStateTransitions;
            for (int i = 0; i < candidates.Length; i++)
            {
                ref var candidate = ref candidates[i];
                if (MatchesConditions(ref candidate.conditions, ref controllerBlob.parameterTypes, parameters, triggersToReset))
                {
                    ConsumeTriggers(ref candidate.conditions, ref controllerBlob.parameterTypes, triggersToReset);
                    return new MecanimStateMachineActiveStates
                    {
                        currentStateIndex          = candidate.destinationStateIndex,
                        currentStateNormalizedTime = 0f,
                        nextStateNormalizedTime    = 0f,
                        nextStateTransitionIndex   = Blob.TransitionIndex.Null,
                        transitionNormalizedTime   = 0f,
                    };
                }
            }
            return new MecanimStateMachineActiveStates
            {
                currentStateIndex          = candidates[0].destinationStateIndex,
                currentStateNormalizedTime = 0f,
                nextStateNormalizedTime    = 0f,
                nextStateTransitionIndex   = Blob.TransitionIndex.Null,
                transitionNormalizedTime   = 0f,
            };
        }

        static bool MatchesConditions(ref BlobArray<Blob.Condition>  conditions,
                                      ref Blob.ParameterTypes parameterTypes,
                                      ReadOnlySpan<MecanimParameter> parameters,
                                      ReadOnlySpan<BitField64>       consumedTriggers)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                var condition = conditions[i];
                var parameter = parameters[condition.parameterIndex];
                switch (condition.mode)
                {
                    // Note: We want to early-out when the condition is false, so a lot of the checks are backwards here.
                    case Blob.Condition.ConditionType.If:
                    {
                        if (!parameter.boolParam)
                            return false;
                        if (consumedTriggers[condition.parameterIndex >> 6].IsSet(condition.parameterIndex & 0x3f))
                            return false;
                        break;
                    }
                    case Blob.Condition.ConditionType.IfNot:
                    {
                        if (parameter.boolParam)
                            return false;
                        break;
                    }
                    case Blob.Condition.ConditionType.Greater:
                    {
                        if (!math.select(parameter.intParam > condition.compareValue.intParam,
                                         parameter.floatParam > condition.compareValue.floatParam,
                                         parameterTypes[condition.parameterIndex] == Blob.ParameterTypes.Type.Float))
                            return false;
                        break;
                    }
                    case Blob.Condition.ConditionType.Less:
                    {
                        if (!math.select(parameter.intParam < condition.compareValue.intParam,
                                         parameter.floatParam < condition.compareValue.floatParam,
                                         parameterTypes[condition.parameterIndex] == Blob.ParameterTypes.Type.Float))
                            return false;
                        break;
                    }
                    case Blob.Condition.ConditionType.Equals:
                    {
                        if (parameter.intParam != condition.compareValue.intParam)
                            return false;
                        break;
                    }
                    case Blob.Condition.ConditionType.NotEqual:
                    {
                        if (parameter.intParam == condition.compareValue.intParam)
                            return false;
                        break;
                    }
                    default: return false;
                }
            }
            return true;
        }

        static void ConsumeTriggers(ref BlobArray<Blob.Condition> conditions,
                                    ref Blob.ParameterTypes parameterTypes,
                                    Span<BitField64>              triggersToReset)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].mode == Blob.Condition.ConditionType.If && parameterTypes[conditions[i].parameterIndex] == Blob.ParameterTypes.Type.Trigger)
                {
                    var index = conditions[i].parameterIndex;
                    triggersToReset[index >> 6].SetBits(index & 0x3f, true);
                }
            }
        }

        static void EvaluateAndCacheDuration(ref Blob controllerBlob,
                                             ref SkeletonClipSetBlob clipsBlob,
                                             ReadOnlySpan<MecanimParameter> parameters,
                                             ReadOnlySpan<float>            layerWeights,
                                             int stateMachineIndex,
                                             int stateIndex,
                                             ref float duration,
                                             ref int cacheCount,
                                             Span<CachedStateDuration>      cache)
        {
            // First, check if we've already cached the duration
            for (int i = 0; i < cacheCount; i++)
            {
                if (cache[i].stateIndex == stateIndex)
                {
                    duration = cache[i].duration;
                    return;
                }
            }

            ref var     stateMachine     = ref controllerBlob.stateMachines[stateMachineIndex];
            Span<float> durationsByLayer = stackalloc float[stateMachine.influencingLayers.Length];
            for (int i = 0; i < durationsByLayer.Length; i++)
            {
                var motionIndex     = controllerBlob.layers[stateMachine.influencingLayers[i]].motionIndices[stateIndex];
                durationsByLayer[i] = MotionEvaluation.GetBlendedMotionDuration(ref controllerBlob, ref clipsBlob, parameters, motionIndex);
            }

            if (durationsByLayer.Length == 1)
            {
                duration = durationsByLayer[0];
            }
            else
            {
                // Todo: What is the weighting function for multiple layers?
                // The base layer always has a weight of 1f, so if it was weighted sums, then the most a synced layer can override would be 50%.
                // But also, what about sync layers that target a different layer with a fractional weight, and what happens if multiple sync layers
                // target the same layer?
                throw new NotImplementedException();
            }
            cache[cacheCount] = new CachedStateDuration { duration = duration, stateIndex = stateIndex };
            cacheCount++;
        }
    }
}

