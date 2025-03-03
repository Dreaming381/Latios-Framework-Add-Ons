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
                                    ReadOnlySpan<float>                 computedLayerWeights,
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

                    // Loop twice, first over any state transitions, and then through local transitions
                    // Todo: Should this order be flipped?
                    for (int transitionArrayType = 0; transitionArrayType < 2; transitionArrayType++)
                    {
                        ref var transitions = ref (transitionArrayType == 0 ? ref stateMachineBlob.anyStateTransitions : ref stateBlob.transitions);
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
                                                             computedLayerWeights,
                                                             stateMachineIndex,
                                                             state.currentStateIndex,
                                                             ref currentStateDuration,
                                                             ref cachedStateDurationsCount,
                                                             cachedStateDurations);
                                    stateNormalizedEndTime = math.select(state.currentStateNormalizedTime + normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration,
                                                                         1f,
                                                                         currentStateDuration == 0f);
                                }

                                var exitTime = transition.normalizedExitTime;
                                if (currentStateDuration != 0f && (math.abs(state.currentStateNormalizedTime) > exitTime || math.abs(stateNormalizedEndTime) < exitTime))
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
                            state.nextStateTransitionIndex   = new Blob.TransitionIndex { index = (ushort)i, isAnyStateTransition = transitionArrayType == 0 };

                            normalizedDeltaTimeRemaining -= fractionOfDeltaTimeInState;
                            transitionArrayType           = 2;

                            break;  // i loop
                        }
                    }  // transitionArrayType loop

                    if (state.nextStateTransitionIndex.invalid)
                    {
                        // There were no transitions. Just play the time in the state.
                        float stateDuration = 0f;
                        EvaluateAndCacheDuration(ref controllerBlob,
                                                 ref clipsBlob,
                                                 parameters,
                                                 computedLayerWeights,
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
                    ref var stateBlob            = ref stateMachineBlob.states[state.currentStateIndex];
                    float   currentStateDuration = -1f;
                    float   nextStateDuration    = -1f;
                    bool    activeTransIsAny     = state.nextStateTransitionIndex.isAnyStateTransition;
                    ref var activeTransitionBlob =
                        ref (activeTransIsAny ? ref stateMachineBlob.anyStateTransitions[state.nextStateTransitionIndex.index] : ref stateBlob.transitions[state.
                                                                                                                                                           nextStateTransitionIndex.
                                                                                                                                                           index]);

                    // We are in the transition, so first, we need to check for interruptions.
                    // Determine how many times we have to loop:
                    var transitionLoopsRequired = activeTransitionBlob.interruptionSource switch
                    {
                        Blob.Transition.InterruptionSource.None => 1,
                        Blob.Transition.InterruptionSource.Source => 2,
                        Blob.Transition.InterruptionSource.Destination => 2,
                        Blob.Transition.InterruptionSource.SourceThenDestination => 3,
                        Blob.Transition.InterruptionSource.DestinationThenSource => 3,
                        _ => 0,
                    };

                    for (int transitionArrayType = 0; transitionArrayType < transitionLoopsRequired; transitionArrayType++)
                    {
                        ref var transitions                 = ref stateMachineBlob.anyStateTransitions;
                        bool    isActiveTransitionTypeArray = transitionArrayType == 0 && activeTransIsAny;
                        bool    isSourceArrayType           = false;
                        if (transitionArrayType == 1)
                        {
                            if (activeTransitionBlob.interruptionSource == Blob.Transition.InterruptionSource.Source ||
                                activeTransitionBlob.interruptionSource == Blob.Transition.InterruptionSource.SourceThenDestination)
                            {
                                transitions                 = ref stateBlob.transitions;
                                isActiveTransitionTypeArray = !activeTransIsAny;
                                isSourceArrayType           = true;
                            }
                            else
                                transitions = ref stateMachineBlob.states[activeTransitionBlob.destinationStateIndex].transitions;
                        }
                        else if (transitionArrayType == 2)
                        {
                            if (activeTransitionBlob.interruptionSource == Blob.Transition.InterruptionSource.Source ||
                                activeTransitionBlob.interruptionSource == Blob.Transition.InterruptionSource.SourceThenDestination)
                                transitions = ref stateMachineBlob.states[activeTransitionBlob.destinationStateIndex].transitions;
                            else
                            {
                                transitions                 = ref stateBlob.transitions;
                                isActiveTransitionTypeArray = !activeTransIsAny;
                                isSourceArrayType           = true;
                            }
                        }

                        for (int i = 0; i < transitions.Length; i++)
                        {
                            ref var transition     = ref transitions[i];
                            bool    quitAfterCheck = false;
                            if (activeTransitionBlob.usesOrderedInterruptions && isActiveTransitionTypeArray && state.nextStateTransitionIndex.index == i)
                            {
                                if (!activeTransitionBlob.canTransitionToSelf ||
                                    !MatchesConditions(ref transition.conditions, ref controllerBlob.parameterTypes, parameters, localTriggersToReset))
                                {
                                    // We have exhausted all transitions ahead of us in the queue. Quite the loop.
                                    transitionArrayType = 4;
                                    break;
                                }
                                quitAfterCheck = true;
                            }
                            else if (!MatchesConditions(ref transition.conditions, ref controllerBlob.parameterTypes, parameters, localTriggersToReset))
                                continue;

                            if (transition.hasExitTime)
                            {
                                if (currentStateDuration < 0f)
                                {
                                    EvaluateAndCacheDuration(ref controllerBlob,
                                                             ref clipsBlob,
                                                             parameters,
                                                             computedLayerWeights,
                                                             stateMachineIndex,
                                                             state.currentStateIndex,
                                                             ref currentStateDuration,
                                                             ref cachedStateDurationsCount,
                                                             cachedStateDurations);
                                    EvaluateAndCacheDuration(ref controllerBlob,
                                                             ref clipsBlob,
                                                             parameters,
                                                             computedLayerWeights,
                                                             stateMachineIndex,
                                                             activeTransitionBlob.destinationStateIndex,
                                                             ref nextStateDuration,
                                                             ref cachedStateDurationsCount,
                                                             cachedStateDurations);
                                }
                                float transitionNormalizedDelta;
                                if (activeTransitionBlob.usesRealtimeDuration)
                                {
                                    transitionNormalizedDelta = normalizedDeltaTimeRemaining * scaledDeltaTime / activeTransitionBlob.duration;
                                }
                                else
                                {
                                    // We have an awkward situation here. How much we advance the transition is based on the shared duration.
                                    // But the shared duration is based on the transition factor. Can we derive something?
                                    // t0 = current transition factor, t1 = final transition factor, dt = t1 - t0
                                    // c0 = current state normalized time, c1 = current state final normalized time, dc = c1 - c0
                                    // lc = current state duration, ln = next state duration, x = shared duration
                                    // tr = time remaining, f = fraction of current state used for duration
                                    //
                                    // x = lc + (ln - lc) * t1
                                    // dt = dc / f
                                    // dc = tr / x
                                    // t1 = tr / (x * f) + t0
                                    // t1 = tr / ((lc + (ln - lc) * t1) * f) + t0
                                    // t1 = tr / (lc * f + (ln - lc) * t1 * f) + t0
                                    // t1 * (lc * f + (ln - lc) * t1 * f) = tr + t0 * (lc * f + (ln - lc) * t1 * f)
                                    // t1 * f * lc + t1^2 * f * (ln - lc) = tr + t0 * lc * f + t1 * t0 * f * (ln - lc)
                                    // Solving for t1 is a quadratic of the form
                                    // a = f * (ln - lc)
                                    // b = f * lc - t0 * f * (ln - lc)
                                    // c = -tr - t0 * lc * f
                                    float transitionEndTime;
                                    var   flc                 = activeTransitionBlob.duration * currentStateDuration;
                                    var   a                   = activeTransitionBlob.duration * (nextStateDuration - currentStateDuration);
                                    var   b                   = flc - state.transitionNormalizedTime * a;
                                    var   c                   = -normalizedDeltaTimeRemaining * scaledDeltaTime - state.transitionNormalizedTime * flc;
                                    var   rad                 = b * b - 4f * a * c;
                                    var   t0TransitionEndTime = normalizedDeltaTimeRemaining * scaledDeltaTime /
                                                              (activeTransitionBlob.duration *
                                                               math.lerp(currentStateDuration, nextStateDuration,
                                                                         state.transitionNormalizedTime)) + state.transitionNormalizedTime;
                                    if (rad < 0f)
                                    {
                                        // Something went wrong with our numbers. Fall back to using t0 to advance our transition time.
                                        transitionEndTime = t0TransitionEndTime;
                                    }
                                    else
                                    {
                                        var radRoot = math.sqrt(rad);
                                        var posRoot = 0.5f * (radRoot - b) / a;
                                        var negRoot = 0.5f * (-radRoot - b) / a;
                                        // If the roots minus our current transition time have opposite signs, pick the sign that matches scaledDeltaTime.
                                        // Otherwise, pick the closest to the t0 delta time.
                                        var dtA = posRoot - state.transitionNormalizedTime;
                                        var dtB = negRoot - state.transitionNormalizedTime;
                                        if (math.sign(dtA) != math.sign(dtB))
                                            transitionEndTime = math.select(negRoot, posRoot, math.sign(dtA) == math.sign(scaledDeltaTime));
                                        else
                                        {
                                            var t0dt          = t0TransitionEndTime - state.transitionNormalizedTime;
                                            transitionEndTime = math.select(negRoot, posRoot, math.distance(dtA, t0dt) <= math.distance(dtB, t0dt));
                                        }
                                    }
                                    transitionNormalizedDelta = transitionEndTime - state.transitionNormalizedTime;
                                }
                                var sharedDuration =
                                    math.lerp(currentStateDuration, nextStateDuration, math.saturate(math.abs(transitionNormalizedDelta + state.transitionNormalizedTime)));
                                var currentStateNormalizedEndTime = math.select(state.currentStateNormalizedTime + normalizedDeltaTimeRemaining * scaledDeltaTime / sharedDuration,
                                                                                1f,
                                                                                sharedDuration == 0f);
                                var nextStateNormalizedEndTime = math.select(state.nextStateNormalizedTime + normalizedDeltaTimeRemaining * scaledDeltaTime / sharedDuration,
                                                                             1f,
                                                                             sharedDuration == 0f);
                                bool missedCurrentExitTime = math.abs(state.currentStateNormalizedTime) > transition.normalizedExitTime || math.abs(
                                    currentStateNormalizedEndTime) < transition.normalizedExitTime;
                                bool missedNextExitTime = math.abs(state.nextStateNormalizedTime) > transition.normalizedExitTime ||
                                                          math.abs(nextStateNormalizedEndTime) < transition.normalizedExitTime;
                                bool missedExitTime;
                                if (transitionArrayType == 0)
                                    missedExitTime = missedCurrentExitTime && missedNextExitTime;
                                else if (isSourceArrayType)
                                    missedExitTime = missedCurrentExitTime;
                                else
                                    missedExitTime = missedNextExitTime;

                                if (missedExitTime)
                                {
                                    if (quitAfterCheck)
                                    {
                                        transitionArrayType = 4;
                                        break;
                                    }
                                    continue;
                                }
                            }

                            // Todo: We have identified our interruption transition. Now we need to generate Passages and update to the latest inertial blend progression.
                        }
                    }

                    if (!state.nextStateTransitionIndex.invalid)
                    {
                        // We didn't erase our transition, which means there wasn't an interruption.
                        // Todo: Evaluate transition expiration, and update transition progression.
                        throw new NotImplementedException();
                    }
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
                                             ReadOnlySpan<float>            computedLayerWeights,
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

            ref var stateMachine        = ref controllerBlob.stateMachines[stateMachineIndex];
            float   accumulatedDuration = 0f;
            float   accumulatedWeight   = 0f;
            for (int i = 0; i < stateMachine.influencingLayers.Length; i++)
            {
                var motionIndex      = controllerBlob.layers[stateMachine.influencingLayers[i]].motionIndices[stateIndex];
                var weight           = computedLayerWeights[stateMachine.influencingLayers[i]];
                accumulatedWeight   += weight;
                accumulatedDuration += weight * MotionEvaluation.GetBlendedMotionDuration(ref controllerBlob, ref clipsBlob, parameters, motionIndex);
            }

            duration          = math.select(accumulatedDuration / accumulatedWeight, 0f, accumulatedWeight == 0f);
            cache[cacheCount] = new CachedStateDuration { duration = duration, stateIndex = stateIndex };
            cacheCount++;
        }
    }
}

