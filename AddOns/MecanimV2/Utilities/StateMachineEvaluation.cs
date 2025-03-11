using System;
using Latios.Kinemation;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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

        /// <summary>
        /// Evaluates and updates a Mecanim state machine.
        /// </summary>
        /// <param name="state">The runtime state machine state containing the initial state and will contain the final state after evaluation</param>
        /// <param name="controllerBlob">The Mecanim controller blob asset</param>
        /// <param name="clipsBlob">The animation clips to read durations from to know how quickly to advance the states</param>
        /// <param name="stateMachineIndex">The index of the state machine in the Mecanim controller blob</param>
        /// <param name="scaledDeltaTime">The timestep scaled by any global speed multiplier</param>
        /// <param name="allLayerWeights">The layer weights of all layers in the Mecanim controller. Index zero must have a weight of 1f.</param>
        /// <param name="parameters">The parameters of the Mecanim controller at the current time of evaluation.</param>
        /// <param name="triggersToReset">A cumulative bit array of trigger parameters that have been consumed and should be applied after all layers
        /// have been evaluated. This way, a trigger can affect multiple layers in a single frame, potentially aiding to keep layers in sync with global events.
        /// This array should be allocated to fit 1 bit per parameter.</param>
        /// <param name="outputPassages">An array of passages to fill. The length of this dictates how many passages can be processed before the loop terminates
        /// as detecting a potential infinite loop.</param>
        /// <param name="outputPassagesCount">The number of passages this method recorded after evaluation.</param>
        /// <param name="newInertialBlendProgressRealtime">Progress on any blend, positive or negative. Caller must add to the current progress and then send
        /// absolute value to inertial blend methods</param>
        /// <param name="newInertialBlendDurationRealtime">The new duration of an inertial blend if needed. Will be negative if no new blend is supposed to start.</param>
        public static void Evaluate(ref MecanimStateMachineActiveStates state,
                                    ref Blob controllerBlob,
                                    ref SkeletonClipSetBlob clipsBlob,
                                    int stateMachineIndex,
                                    float scaledDeltaTime,
                                    ReadOnlySpan<float>                 allLayerWeights,
                                    ReadOnlySpan<MecanimParameter>      parameters,
                                    Span<BitField64>                    triggersToReset,
                                    Span<StatePassage>                  outputPassages,
                                    out int outputPassagesCount,
                                    out float newInertialBlendProgressRealtime,
                                    out float newInertialBlendDurationRealtime)
        {
            outputPassagesCount              = 0;
            newInertialBlendProgressRealtime = 0f;
            newInertialBlendDurationRealtime = -1f;

            Span<BitField64> localTriggersToReset = stackalloc BitField64[triggersToReset.Length];
            localTriggersToReset.Clear();

            if (state.currentStateIndex < 0)
                state = SetupInitialState(ref controllerBlob, stateMachineIndex, parameters, localTriggersToReset);

            var                       normalizedDeltaTimeRemaining = 1f;
            ref var                   stateMachineBlob             = ref controllerBlob.stateMachines[stateMachineIndex];
            Span<CachedStateDuration> cachedStateDurations         = stackalloc CachedStateDuration[outputPassages.Length * 2];
            int                       cachedStateDurationsCount    = 0;

            // Set up relative layer weights that influence state durations.
            // Each layer splits its influence with all the layers below it based on the layer's weight.
            // That is, if the last layer has a weight of 80%, then it influences 80% of the duration and
            // all the other layers can only influence up to 20% combined. The 0th layer should always have
            // a weight of 1f. The weights we record do not need to sum to 1f.
            Span<float> influencingLayerRelativeWeights = stackalloc float[stateMachineBlob.influencingLayers.Length];
            {
                float weightRemaining = 1f;
                int   targetIndex     = stateMachineBlob.influencingLayers.Length - 1;
                bool  hasInfluencer   = false;
                for (int i = stateMachineBlob.influencingLayers[targetIndex]; i >= stateMachineBlob.influencingLayers[0]; i--)
                {
                    var weight = allLayerWeights[i];
                    if (i == stateMachineBlob.influencingLayers[targetIndex])
                    {
                        influencingLayerRelativeWeights[targetIndex] = weight * weightRemaining;
                        targetIndex--;
                        hasInfluencer |= weight > 0f;
                    }
                    // In case our last layer has a weight of 0f, and some non-influencing layer has a weight
                    // of 1f, we still want to be able to progress our state machine. So we only start dividing
                    // weights once we have an influencing weight.
                    if (hasInfluencer)
                        weightRemaining *= 1f - weight;
                }
            }

            // This is our main loop. We have a normalizedDeltaTimeRemaining, which is the fraction of the
            // time step leftover after each iteration. In each iteration, states may consume parts or the
            // entirety of the timestep. For example, transitions that have exit times or transition expirations
            // will consume only part of the time step.
            // If we somehow pass through too many states and exceed our passage buffer, we also quit the loop.
            while (normalizedDeltaTimeRemaining > 0f && outputPassagesCount < outputPassages.Length)
            {
                if (state.nextStateTransitionIndex.invalid)
                {
                    // There's no crossfades right now. Try to find an initial transition.
                    ref var stateBlob = ref stateMachineBlob.states[state.currentStateIndex];
                    // Lazily evaluated
                    float currentStateDuration   = -1f;
                    float stateNormalizedEndTime = 0f;

                    foreach (var matchedTransitionIndex in new TransitionEnumerator(ref stateMachineBlob.anyStateTransitions,
                                                                                    ref stateBlob.transitions,
                                                                                    ref controllerBlob.parameterTypes,
                                                                                    parameters,
                                                                                    localTriggersToReset))
                    {
                        ref var transition =
                            ref (matchedTransitionIndex.isAnyStateTransition ? ref stateMachineBlob.anyStateTransitions[matchedTransitionIndex.index] : ref stateBlob.transitions[
                                     matchedTransitionIndex.index]);
                        if (transition.hasExitTime)
                        {
                            if (currentStateDuration < 0f)
                            {
                                // Update lazy evaluations
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         state.currentStateIndex,
                                                         ref currentStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                                // current normalized time [0, 1] + realtime in timestep / duration = new normalized time
                                // If duration is 0, set the end time to the end of the state (1)
                                float scaledStateTimeDelta = stateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                                if (stateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                    scaledStateTimeDelta *= parameters[stateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                                stateNormalizedEndTime    = math.select(state.currentStateNormalizedTime + scaledStateTimeDelta,
                                                                        1f,
                                                                        currentStateDuration == 0f);
                            }

                            // Check if our timestep in state wraps the exit time
                            var exitTime = transition.normalizedExitTime;
                            if (currentStateDuration != 0f && (math.abs(state.currentStateNormalizedTime) > exitTime || math.abs(stateNormalizedEndTime) < exitTime))
                                continue;
                        }

                        // This is our new transition
                        ConsumeTriggers(ref transition.conditions, ref controllerBlob.parameterTypes, localTriggersToReset);

                        // If our transition is immediate, we spend no time in the old state and all the time in the new state.
                        float fractionOfDeltaTimeInState = 0f;
                        if (transition.hasExitTime)
                        {
                            // Compute how much realtime we spent in the state, and then figure out its fraction relative to our scaledDeltaTime
                            var stateTime              = transition.normalizedExitTime - math.abs(state.currentStateNormalizedTime);
                            var stateDeltaTime         = stateTime * currentStateDuration;
                            fractionOfDeltaTimeInState = stateDeltaTime / math.abs(scaledDeltaTime);
                        }

                        // Record the passage for the single current state configuration we are leaving
                        var passage = new StatePassage
                        {
                            currentState               = state.currentStateIndex,
                            nextState                  = -1,
                            currentStateStartTime      = state.currentStateNormalizedTime,
                            currentStateEndTime        = transition.hasExitTime ? transition.normalizedExitTime : state.currentStateNormalizedTime,
                            fractionOfDeltaTimeInState = fractionOfDeltaTimeInState,
                            nextStateStartTime         = 0f,
                            nextStateEndTime           = 0f,
                            transitionProgress         = 0f,
                        };
                        outputPassages[outputPassagesCount] = passage;
                        outputPassagesCount++;

                        // Update the state machine state
                        state.currentStateNormalizedTime = passage.currentStateEndTime;
                        state.nextStateNormalizedTime    = 0f;
                        state.transitionNormalizedTime   = 0f;
                        state.nextStateTransitionIndex   = matchedTransitionIndex;

                        // Accumulate time in our inertial blend, and consume from our delta time
                        newInertialBlendProgressRealtime += fractionOfDeltaTimeInState * scaledDeltaTime;
                        normalizedDeltaTimeRemaining     -= fractionOfDeltaTimeInState;

                        // Quit iterating transitions
                        break;
                    }

                    if (state.nextStateTransitionIndex.invalid)
                    {
                        // There were no transitions. Just play the time in the state.
                        if (currentStateDuration < 0f)
                        {
                            EvaluateAndCacheDuration(ref controllerBlob,
                                                     ref clipsBlob,
                                                     parameters,
                                                     influencingLayerRelativeWeights,
                                                     stateMachineIndex,
                                                     state.currentStateIndex,
                                                     ref currentStateDuration,
                                                     ref cachedStateDurationsCount,
                                                     cachedStateDurations);
                            float scaledStateTimeDelta = stateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                            if (stateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                scaledStateTimeDelta *= parameters[stateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                            stateNormalizedEndTime    = state.currentStateNormalizedTime + scaledStateTimeDelta;
                        }

                        // Record the passage of us being in this state through the rest of deltaTime
                        var passage = new StatePassage
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

                        // Update the state machine state
                        state.currentStateNormalizedTime = stateNormalizedEndTime;

                        // Accumulate time in our inertial blend, and consume the rest of our delta time
                        newInertialBlendProgressRealtime += normalizedDeltaTimeRemaining * scaledDeltaTime;
                        normalizedDeltaTimeRemaining      = 0f;
                    }
                }
                else
                {
                    // We are in the middle of a transition, which means we have both current and next states
                    ref var currentStateBlob     = ref stateMachineBlob.states[state.currentStateIndex];
                    bool    activeTransIsAny     = state.nextStateTransitionIndex.isAnyStateTransition;
                    ref var activeTransitionBlob = ref (activeTransIsAny ? ref stateMachineBlob.anyStateTransitions[state.nextStateTransitionIndex.index] :
                                                        ref currentStateBlob.transitions[state.nextStateTransitionIndex.index]);
                    ref var nextStateBlob = ref stateMachineBlob.states[activeTransitionBlob.destinationStateIndex];

                    // These are lazily evaluated
                    float currentStateDuration = -1f;
                    float currentStateEndTime  = 0f;
                    float nextStateDuration    = -1f;
                    float nextStateEndTime     = 0f;

                    // Check for interruptions to our existing transition
                    foreach (var matchedInterruption in new InterruptEnumerator(ref activeTransitionBlob,
                                                                                ref stateMachineBlob.anyStateTransitions,
                                                                                ref currentStateBlob.transitions,
                                                                                ref nextStateBlob.transitions,
                                                                                ref controllerBlob.parameterTypes,
                                                                                parameters,
                                                                                localTriggersToReset))
                    {
                        // Find our transition for this interruption
                        ref var transitionArray = ref stateMachineBlob.anyStateTransitions;
                        if (matchedInterruption.from == MatchedInterrupt.From.Source)
                            transitionArray = ref currentStateBlob.transitions;
                        else if (matchedInterruption.from == MatchedInterrupt.From.Destination)
                            transitionArray = ref nextStateBlob.transitions;
                        ref var transition  = ref transitionArray[matchedInterruption.index.index];

                        // By default, we transition immediately without consuming any of the deltaTime
                        float transitionTime             = math.abs(state.transitionNormalizedTime);
                        float fractionOfDeltaTimeInState = 0f;
                        if (transition.hasExitTime)
                        {
                            // We have an exit time. We need to check if we satisfy it. However, we have two separate state times.
                            // So we need to determine which one we care about based on where our interrupt comes from.
                            // For an Any interrupt, we can satisfy either exit time.
                            if (matchedInterruption.from != MatchedInterrupt.From.Destination && currentStateDuration < 0f)
                            {
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         state.currentStateIndex,
                                                         ref currentStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                                float scaledStateTimeDelta = currentStateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                                if (currentStateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                    scaledStateTimeDelta *= parameters[currentStateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                                currentStateEndTime       = state.currentStateNormalizedTime + scaledStateTimeDelta;
                            }
                            if (matchedInterruption.from != MatchedInterrupt.From.Source && nextStateDuration < 0f)
                            {
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         activeTransitionBlob.destinationStateIndex,
                                                         ref nextStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                                float scaledStateTimeDelta = nextStateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / nextStateDuration;
                                if (nextStateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                    scaledStateTimeDelta *= parameters[nextStateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                                nextStateEndTime          = state.nextStateNormalizedTime + scaledStateTimeDelta;
                            }
                            bool missedCurrentExitTime = math.abs(state.currentStateNormalizedTime) > transition.normalizedExitTime || math.abs(
                                currentStateEndTime) < transition.normalizedExitTime;
                            bool missedNextExitTime = math.abs(state.nextStateNormalizedTime) > transition.normalizedExitTime ||
                                                      math.abs(nextStateEndTime) < transition.normalizedExitTime;
                            bool missedExitTime;
                            if (matchedInterruption.from == MatchedInterrupt.From.Any)
                                missedExitTime = missedCurrentExitTime && missedNextExitTime;
                            else if (matchedInterruption.from == MatchedInterrupt.From.Source)
                                missedExitTime = missedCurrentExitTime;
                            else
                                missedExitTime = missedNextExitTime;

                            if (missedExitTime)
                            {
                                continue;
                            }

                            // We are locking in to this interrupt with an exit time, so we need to find any missing end times
                            if (currentStateDuration < 0f)
                            {
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         state.currentStateIndex,
                                                         ref currentStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                                float scaledStateTimeDelta = currentStateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                                if (currentStateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                    scaledStateTimeDelta *= parameters[currentStateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                                currentStateEndTime       = state.currentStateNormalizedTime + scaledStateTimeDelta;
                            }
                            if (nextStateDuration < 0f)
                            {
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         activeTransitionBlob.destinationStateIndex,
                                                         ref nextStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                                float scaledStateTimeDelta = nextStateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / nextStateDuration;
                                if (nextStateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                    scaledStateTimeDelta *= parameters[nextStateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                                nextStateEndTime          = state.nextStateNormalizedTime + scaledStateTimeDelta;
                            }

                            // We also need to know which state time to use to find the normalized delta time
                            // consumed between the state time and the exit time. We'll calculate both normalized
                            // times first.
                            float currentFractionBeforeExit = math.unlerp(state.currentStateNormalizedTime, currentStateEndTime, transition.normalizedExitTime);
                            float nextFractionBeforeExit    = math.unlerp(state.nextStateNormalizedTime, nextStateEndTime, transition.normalizedExitTime);

                            if (matchedInterruption.from == MatchedInterrupt.From.Any)
                            {
                                // If only one of the states matched the exit time, pick that.
                                // Otherwise, pick whichever happened first.
                                if (missedCurrentExitTime)
                                    fractionOfDeltaTimeInState = nextFractionBeforeExit;
                                else if (missedNextExitTime)
                                    fractionOfDeltaTimeInState = currentFractionBeforeExit;
                                else
                                    fractionOfDeltaTimeInState = math.min(currentFractionBeforeExit, nextFractionBeforeExit);
                            }
                            else if (matchedInterruption.from == MatchedInterrupt.From.Source)
                                fractionOfDeltaTimeInState = currentFractionBeforeExit;
                            else
                                fractionOfDeltaTimeInState = nextFractionBeforeExit;

                            // Overwrite our end times to the exit time
                            if (activeTransitionBlob.usesRealtimeDuration)
                                transitionTime += normalizedDeltaTimeRemaining * math.abs(scaledDeltaTime);
                            else
                                transitionTime  = math.unlerp(activeTransitionBlob.duration, 1f, math.abs(currentStateEndTime));
                            transitionTime      = math.saturate(transitionTime);
                            currentStateEndTime = math.lerp(state.currentStateNormalizedTime, currentStateEndTime, fractionOfDeltaTimeInState);
                            nextStateEndTime    = math.lerp(state.nextStateNormalizedTime, nextStateEndTime, fractionOfDeltaTimeInState);
                        }
                        else
                        {
                            // We are transitioning immediately, so set the end times to the current times
                            currentStateEndTime = state.currentStateNormalizedTime;
                            nextStateEndTime    = state.nextStateNormalizedTime;
                        }

                        // This is our new transition
                        ConsumeTriggers(ref transition.conditions, ref controllerBlob.parameterTypes, localTriggersToReset);

                        // Record the passage for the dual state configuration we are leaving by interrupting
                        var passage = new StatePassage
                        {
                            currentState               = state.currentStateIndex,
                            nextState                  = activeTransitionBlob.destinationStateIndex,
                            currentStateStartTime      = state.currentStateNormalizedTime,
                            currentStateEndTime        = currentStateEndTime,
                            fractionOfDeltaTimeInState = fractionOfDeltaTimeInState,
                            nextStateStartTime         = state.nextStateNormalizedTime,
                            nextStateEndTime           = nextStateEndTime,
                            transitionProgress         = transitionTime,
                        };
                        outputPassages[outputPassagesCount] = passage;
                        outputPassagesCount++;

                        // Update the state machine state to our new interrupted state without any active transitions
                        state.currentStateIndex          = transition.destinationStateIndex;
                        state.currentStateNormalizedTime = 0f;
                        state.nextStateNormalizedTime    = 0f;
                        state.transitionNormalizedTime   = 0f;
                        state.nextStateTransitionIndex   = Blob.TransitionIndex.Null;

                        // Reset the inertial blend progress and assign the inertial blend duration in realtime
                        newInertialBlendProgressRealtime = 0f;
                        // If realtime duration, just propagate the realtime directly.
                        // Otherwise, Source or Any interrupts use the proportion of the Current state while
                        // Destination interrupts use the proportion of the Next state. We will need the durations
                        // for these. But we don't need end times.
                        if (transition.usesRealtimeDuration)
                            newInertialBlendDurationRealtime = transition.duration;
                        else if (matchedInterruption.from != MatchedInterrupt.From.Destination)
                        {
                            if (currentStateDuration < 0f)
                            {
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         state.currentStateIndex,
                                                         ref currentStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                            }
                            newInertialBlendDurationRealtime = transition.duration * currentStateDuration;
                        }
                        else
                        {
                            if (nextStateDuration < 0f)
                            {
                                EvaluateAndCacheDuration(ref controllerBlob,
                                                         ref clipsBlob,
                                                         parameters,
                                                         influencingLayerRelativeWeights,
                                                         stateMachineIndex,
                                                         activeTransitionBlob.destinationStateIndex,
                                                         ref nextStateDuration,
                                                         ref cachedStateDurationsCount,
                                                         cachedStateDurations);
                            }
                            newInertialBlendDurationRealtime = transition.duration * nextStateDuration;
                        }

                        // Consume the amount of delta time we used up to the exit time (if any) and exit the loop through all interruptions
                        normalizedDeltaTimeRemaining -= fractionOfDeltaTimeInState;
                        break;
                    }

                    if (!state.nextStateTransitionIndex.invalid)
                    {
                        // We didn't erase our transition, which means there wasn't an interruption.
                        // Determine our end times and our transition time.
                        if (currentStateDuration < 0f)
                        {
                            EvaluateAndCacheDuration(ref controllerBlob,
                                                     ref clipsBlob,
                                                     parameters,
                                                     influencingLayerRelativeWeights,
                                                     stateMachineIndex,
                                                     state.currentStateIndex,
                                                     ref currentStateDuration,
                                                     ref cachedStateDurationsCount,
                                                     cachedStateDurations);
                            float scaledStateTimeDelta = currentStateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / currentStateDuration;
                            if (currentStateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                scaledStateTimeDelta *= parameters[currentStateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                            currentStateEndTime       = state.currentStateNormalizedTime + scaledStateTimeDelta;
                        }
                        if (nextStateDuration < 0f)
                        {
                            EvaluateAndCacheDuration(ref controllerBlob,
                                                     ref clipsBlob,
                                                     parameters,
                                                     influencingLayerRelativeWeights,
                                                     stateMachineIndex,
                                                     activeTransitionBlob.destinationStateIndex,
                                                     ref nextStateDuration,
                                                     ref cachedStateDurationsCount,
                                                     cachedStateDurations);
                            float scaledStateTimeDelta = nextStateBlob.baseStateSpeed * normalizedDeltaTimeRemaining * scaledDeltaTime / nextStateDuration;
                            if (nextStateBlob.stateSpeedMultiplierParameterIndex >= 0)
                                scaledStateTimeDelta *= parameters[nextStateBlob.stateSpeedMultiplierParameterIndex].floatParam;
                            nextStateEndTime          = state.nextStateNormalizedTime + scaledStateTimeDelta;
                        }
                        float transitionTime;
                        if (activeTransitionBlob.usesRealtimeDuration)
                            transitionTime = state.transitionNormalizedTime + normalizedDeltaTimeRemaining * math.abs(scaledDeltaTime);
                        else
                            transitionTime = math.unlerp(activeTransitionBlob.duration, 1f, math.abs(currentStateEndTime));

                        var transitionEndTime = math.min(1f, math.abs(transitionTime));
                        if (transitionEndTime >= 1f)
                        {
                            // We finished our transition.
                            var usedTransitionDeltaFraction = 1f / transitionEndTime;
                            var fractionOfDeltaTimeInState  = normalizedDeltaTimeRemaining * usedTransitionDeltaFraction;

                            currentStateEndTime = math.lerp(state.currentStateNormalizedTime, currentStateEndTime, usedTransitionDeltaFraction);
                            nextStateEndTime    = math.lerp(state.nextStateNormalizedTime, nextStateEndTime, usedTransitionDeltaFraction);

                            // Record the passage for the dual state configuration we are leaving by expiration
                            var passage = new StatePassage
                            {
                                currentState               = state.currentStateIndex,
                                nextState                  = activeTransitionBlob.destinationStateIndex,
                                currentStateStartTime      = state.currentStateNormalizedTime,
                                currentStateEndTime        = currentStateEndTime,
                                fractionOfDeltaTimeInState = fractionOfDeltaTimeInState,
                                nextStateStartTime         = state.nextStateNormalizedTime,
                                nextStateEndTime           = nextStateEndTime,
                                transitionProgress         = 1f,
                            };
                            outputPassages[outputPassagesCount] = passage;
                            outputPassagesCount++;

                            // Update the state machine state to our new interrupted state without any active transitions
                            state.currentStateIndex          = nextStateBlob.stateIndexInStateMachine;
                            state.currentStateNormalizedTime = nextStateEndTime;
                            state.nextStateNormalizedTime    = 0f;
                            state.transitionNormalizedTime   = 0f;
                            state.nextStateTransitionIndex   = Blob.TransitionIndex.Null;

                            // Accumulate time in our inertial blend, and consume from our deltaTime
                            newInertialBlendProgressRealtime += fractionOfDeltaTimeInState * scaledDeltaTime;
                            normalizedDeltaTimeRemaining     -= fractionOfDeltaTimeInState;
                        }
                        else
                        {
                            // We use up all the remaining deltaTime in this active transition.
                            // Record the passage for the dual state configuration.
                            var passage = new StatePassage
                            {
                                currentState               = state.currentStateIndex,
                                nextState                  = activeTransitionBlob.destinationStateIndex,
                                currentStateStartTime      = state.currentStateNormalizedTime,
                                currentStateEndTime        = currentStateEndTime,
                                fractionOfDeltaTimeInState = normalizedDeltaTimeRemaining,
                                nextStateStartTime         = state.nextStateNormalizedTime,
                                nextStateEndTime           = nextStateEndTime,
                                transitionProgress         = 1f,
                            };
                            outputPassages[outputPassagesCount] = passage;
                            outputPassagesCount++;

                            // Accumulate time in our inertial blend, and consume the remaining deltaTime
                            newInertialBlendProgressRealtime += normalizedDeltaTimeRemaining * scaledDeltaTime;
                            normalizedDeltaTimeRemaining      = 0f;
                        }
                    }
                }
            }

            if (normalizedDeltaTimeRemaining > 0f)
            {
                UnityEngine.Debug.LogWarning(
                    $"StateMachine {stateMachineIndex} evaluation passed through too many states and did not consume all of deltaTime. Fraction remaining: {normalizedDeltaTimeRemaining}");
            }

            // Write back triggers we consumed
            for (int i = 0; i < triggersToReset.Length; i++)
            {
                triggersToReset[i].Value |= localTriggersToReset[i].Value;
            }
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

        struct CachedStateDuration
        {
            public int   stateIndex;
            public float duration;
        }

        static void EvaluateAndCacheDuration(ref Blob controllerBlob,
                                             ref SkeletonClipSetBlob clipsBlob,
                                             ReadOnlySpan<MecanimParameter> parameters,
                                             ReadOnlySpan<float>            influencingLayerRelativeWeights,
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
                var motionIndex = controllerBlob.layers[stateMachine.influencingLayers[i]].motionIndices[stateIndex];
                var weight      = influencingLayerRelativeWeights[i];
                if (weight > 0f)
                {
                    accumulatedWeight   += weight;
                    accumulatedDuration += weight * MotionEvaluation.GetBlendedMotionDuration(ref controllerBlob, ref clipsBlob, parameters, motionIndex);
                }
            }

            duration          = math.select(accumulatedDuration / accumulatedWeight, 0f, accumulatedWeight == 0f);
            cache[cacheCount] = new CachedStateDuration { duration = duration, stateIndex = stateIndex };
            cacheCount++;
        }

        unsafe ref struct TransitionEnumerator
        {
            Blob.ParameterTypes*           parameterTypes;
            ReadOnlySpan<MecanimParameter> parameters;
            ReadOnlySpan<BitField64>       usedTriggers;
            Blob.Transition*               anyTransitions;
            Blob.Transition*               sourceTransitions;
            int                            anyTransitionsCount;
            int                            sourceTransitionsCount;
            int                            index;
            bool                           isAny;

            public TransitionEnumerator GetEnumerator() => this;

            public TransitionEnumerator(ref BlobArray<Blob.Transition> anyTransitions,
                                        ref BlobArray<Blob.Transition> sourceTransitions,
                                        ref Blob.ParameterTypes parameterTypes,
                                        ReadOnlySpan<MecanimParameter> parameters,
                                        ReadOnlySpan<BitField64>       usedTriggers)
            {
                this.parameterTypes    = (Blob.ParameterTypes*)UnsafeUtility.AddressOf(ref parameterTypes);
                this.parameters        = parameters;
                this.usedTriggers      = usedTriggers;
                this.anyTransitions    = (Blob.Transition*)anyTransitions.GetUnsafePtr();
                anyTransitionsCount    = anyTransitions.Length;
                this.sourceTransitions = (Blob.Transition*)sourceTransitions.GetUnsafePtr();
                sourceTransitionsCount = sourceTransitions.Length;
                index                  = -1;
                isAny                  = true;
            }

            public Blob.TransitionIndex Current => new Blob.TransitionIndex
            {
                index                = (ushort)index,
                isAnyStateTransition = isAny
            };

            public bool MoveNext()
            {
                // Loop twice, first over any state transitions, and then through local transitions
                // Todo: Should this order be flipped?
                while (true)
                {
                    index++;
                    Blob.Transition* transitionToTest = null;
                    if (isAny)
                    {
                        if (index >= anyTransitionsCount)
                        {
                            index = -1;
                            isAny = false;
                            continue;
                        }
                        transitionToTest = anyTransitions + index;
                    }
                    else
                    {
                        if (index >= sourceTransitionsCount)
                            return false;
                        transitionToTest = sourceTransitions + index;
                    }

                    if (MatchesConditions(ref transitionToTest->conditions, ref *parameterTypes, parameters, usedTriggers))
                    {
                        return true;
                    }
                }
            }
        }

        struct MatchedInterrupt
        {
            public Blob.TransitionIndex index;
            public From                 from;

            public enum From : byte
            {
                Any,
                Source,
                Destination
            }
        }

        unsafe ref struct InterruptEnumerator
        {
            Blob.Transition*               activeTransition;
            Blob.ParameterTypes*           parameterTypes;
            ReadOnlySpan<MecanimParameter> parameters;
            ReadOnlySpan<BitField64>       usedTriggers;
            Blob.Transition*               anyTransitions;
            Blob.Transition*               sourceTransitions;
            Blob.Transition*               destinationTransitions;
            int                            anyTransitionsCount;
            int                            sourceTransitionsCount;
            int                            destinationTransitionsCount;
            int                            index;
            byte                           arrayIndex;
            byte                           arrayCount;
            bool                           sourceFirst;
            bool                           orderedInterruptions;

            public InterruptEnumerator GetEnumerator() => this;

            public InterruptEnumerator(ref Blob.Transition activeTransition,
                                       ref BlobArray<Blob.Transition> anyTransitions,
                                       ref BlobArray<Blob.Transition> sourceTransitions,
                                       ref BlobArray<Blob.Transition> destinationTransitions,
                                       ref Blob.ParameterTypes parameterTypes,
                                       ReadOnlySpan<MecanimParameter> parameters,
                                       ReadOnlySpan<BitField64>       usedTriggers)
            {
                this.activeTransition       = (Blob.Transition*)UnsafeUtility.AddressOf(ref activeTransition);
                this.parameterTypes         = (Blob.ParameterTypes*)UnsafeUtility.AddressOf(ref parameterTypes);
                this.parameters             = parameters;
                this.usedTriggers           = usedTriggers;
                this.anyTransitions         = (Blob.Transition*)anyTransitions.GetUnsafePtr();
                anyTransitionsCount         = anyTransitions.Length;
                this.sourceTransitions      = (Blob.Transition*)sourceTransitions.GetUnsafePtr();
                sourceTransitionsCount      = sourceTransitions.Length;
                this.destinationTransitions = (Blob.Transition*)destinationTransitions.GetUnsafePtr();
                destinationTransitionsCount = destinationTransitions.Length;
                index                       = -1;
                arrayIndex                  = 0;
                arrayCount                  = activeTransition.interruptionSource switch
                {
                    Blob.Transition.InterruptionSource.None => 1,
                    Blob.Transition.InterruptionSource.Source => 2,
                    Blob.Transition.InterruptionSource.Destination => 2,
                    Blob.Transition.InterruptionSource.SourceThenDestination => 3,
                    Blob.Transition.InterruptionSource.DestinationThenSource => 3,
                    _ => 0,
                };
                sourceFirst = activeTransition.interruptionSource == Blob.Transition.InterruptionSource.Source ||
                              activeTransition.interruptionSource == Blob.Transition.InterruptionSource.SourceThenDestination;
                orderedInterruptions = activeTransition.usesOrderedInterruptions;
            }

            public MatchedInterrupt Current => new MatchedInterrupt
            {
                index = new Blob.TransitionIndex
                {
                    index                = (ushort)index,
                    isAnyStateTransition = arrayIndex == 0
                },
                from = arrayIndex switch
                {
                    0 => MatchedInterrupt.From.Any,
                    1 => sourceFirst ? MatchedInterrupt.From.Source : MatchedInterrupt.From.Destination,
                    2 => sourceFirst ? MatchedInterrupt.From.Destination : MatchedInterrupt.From.Source,
                    _ => MatchedInterrupt.From.Any,
                }
            };

            public bool MoveNext()
            {
                // Loop twice, first over any state transitions, and then through local transitions
                // Todo: Should this order be flipped?
                while (true)
                {
                    if (arrayIndex >= arrayCount)
                        return false;

                    index++;
                    Blob.Transition* transitionArrayToTest = null;
                    int              count                 = 0;
                    switch (arrayIndex)
                    {
                        case 0:
                            transitionArrayToTest = anyTransitions;
                            count                 = anyTransitionsCount;
                            break;
                        case 1:
                            if (sourceFirst)
                            {
                                transitionArrayToTest = sourceTransitions;
                                count                 = sourceTransitionsCount;
                            }
                            else
                            {
                                transitionArrayToTest = destinationTransitions;
                                count                 = destinationTransitionsCount;
                            }
                            break;
                        case 2:
                            if (sourceFirst)
                            {
                                transitionArrayToTest = destinationTransitions;
                                count                 = destinationTransitionsCount;
                            }
                            else
                            {
                                transitionArrayToTest = sourceTransitions;
                                count                 = sourceTransitionsCount;
                            }
                            break;
                    }
                    if (index < count)
                    {
                        index = -1;
                        arrayIndex++;
                        continue;
                    }

                    if (orderedInterruptions && transitionArrayToTest + index == activeTransition)
                    {
                        if (!activeTransition->canTransitionToSelf)
                            return false;

                        // Kill off future iterations, but evaluate self-interruption
                        arrayCount = 0;
                    }

                    if (MatchesConditions(ref transitionArrayToTest[index].conditions, ref *parameterTypes, parameters, usedTriggers))
                    {
                        return true;
                    }
                }
            }
        }
    }
}

