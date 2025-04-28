using Latios.Kinemation;
using Latios.Unsafe;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Latios.Mecanim
{
    public readonly partial struct MecanimAspect : IAspect
    {
        readonly RefRW<MecanimController>        m_controller;
        readonly EnabledRefRW<MecanimController> m_controllerEnabled;

        readonly DynamicBuffer<MecanimStateMachineActiveStates> m_activeStates;
        readonly DynamicBuffer<LayerWeights>                    m_layerWeights;
        readonly DynamicBuffer<MecanimParameter>                m_parameters;
        readonly DynamicBuffer<MecanimStateTransitionEvent>     m_stateTransitionEvents;
        readonly DynamicBuffer<MecanimClipEvent>                m_clipEvents;

        private DynamicBuffer<MecanimParameter> parameters => m_parameters;
        private DynamicBuffer<LayerWeights> layerWeights => m_layerWeights;
        private ref MecanimControllerBlob GetControllerBlob() => ref m_controller.ValueRO.controllerBlob.Value;

        #region Controller
        /// <summary>
        /// Sets whether the animator controller state machine is enabled and allowed to update
        /// </summary>
        public bool enabled
        {
            get => m_controllerEnabled.ValueRO;
            set => m_controllerEnabled.ValueRW = value;
        }

        /// <summary>
        /// The speed of the controller relative to normal Time.deltaTime
        /// </summary>
        public float speed
        {
            get => m_controller.ValueRO.speed;
            set => m_controller.ValueRW.speed = value;
        }

        /// <summary>
        /// Determines if root motion should be applied to the root bone automatically
        /// </summary>
        public bool applyRootMotion
        {
            get => m_controller.ValueRO.applyRootMotion;
            set => m_controller.ValueRW.applyRootMotion = value;
        }

        /// <summary>
        /// Advances all state machines for this animation controller by deltaTime
        /// </summary>
        public void Update(OptimizedSkeletonAspect optimizedSkeletonAspect, double elapsedTime, float deltaTime)
        {
            var threadStackAllocator = ThreadStackAllocator.GetAllocator();

            MecanimUpdater.Update(
                ref threadStackAllocator,
                ref m_controller.ValueRW,
                m_activeStates.AsNativeArray().AsSpan(),
                m_layerWeights.AsNativeArray().AsReadOnlySpan(),
                m_parameters.AsNativeArray().AsSpan(),
                optimizedSkeletonAspect,
                m_stateTransitionEvents,
                m_clipEvents,
                elapsedTime,
                deltaTime);

            threadStackAllocator.Dispose();
        }

        /// <summary>
        /// Gets the weight of the given layer (slow)
        /// </summary>
        /// <param name="layerName">The layer name in the Mecanim Controller.</param>
        /// <returns>The current weight for the layer.</returns>
        public float GetLayerWeight(FixedString128Bytes layerName)
        {
            return GetLayerWeight(GetLayerIndex(layerName));
        }

        /// <summary>
        /// Gets the weight of the layer at the given index (fastest)
        /// </summary>
        /// <param name="index">The layer index in the Mecanim Controller's list of layers.</param>
        /// <returns>The current weight for the layer.</returns>
        public float GetLayerWeight(short index)
        {
            if (index < 0)
            {
                Debug.LogWarning($"Layer not found in Mecanim animation controller.");
                return 0f;
            }
            return layerWeights.ElementAt(index).weight;
        }

        /// <summary>
        /// Sets the weight of the layer at the given index (slow)
        /// </summary>
        /// <param name="layerName">The layer name in the Mecanim Controller.</param>
        /// <param name="weight">The weight to set for the layer. </param>
        public void SetLayerWeight(FixedString128Bytes layerName, float weight)
        {
            SetLayerWeight(GetLayerIndex(layerName), weight);
        }

        /// <summary>
        /// Sets the weight of the layer at the given index (fastest)
        /// </summary>
        /// <param name="index">The layer index in the Mecanim Controller's list of layers.</param>
        /// <param name="weight">The weight to set for the layer. </param>
        public void SetLayerWeight(short index, float weight)
        {
            if (index < 0)
            {
                Debug.LogWarning($"Layer not found in Mecanim animation controller.");
                return;
            }
            if (weight < 0 || weight > 1)
            {
                Debug.LogWarning($"Layer weight must be between 0 and 1. It was {weight}.");
                return;
            }
            layerWeights.ElementAt(index).weight = weight;
        }
        #endregion

        #region StateHandles

        /**
         * This is an expensive call. Please cache and re-use the result.
         * fullStateName must include all parent state machines names separated by dots. Example: "MySubMachine.MyChildSubMachine.Jump"
         */
        public StateHandle GetStateHandle(FixedString128Bytes layerName, FixedString128Bytes fullStateName)
        {
            var stateMachineIndex                      = GetControllerBlob().GetStateMachineIndex(layerName);
            var stateIndex                             = GetControllerBlob().GetStateIndex(stateMachineIndex, fullStateName);
            return new StateHandle { StateMachineIndex = stateMachineIndex, StateIndex = stateIndex };
        }

        /**
         * This is an expensive method. Please cache and re-use the result.
         * fullStateName must include all parent state machines names separated by dots and be hashed using GetHashCode()
         * Example: "MySubMachine.MyChildSubMachine.Jump".GetHashCode()
         */
        public StateHandle GetStateHandle(FixedString128Bytes layerName, int fullStateNameHashCode)
        {
            var stateMachineIndex                      = GetControllerBlob().GetStateMachineIndex(layerName);
            var stateIndex                             = GetControllerBlob().GetStateIndex(stateMachineIndex, fullStateNameHashCode);
            return new StateHandle { StateMachineIndex = stateMachineIndex, StateIndex = stateIndex };
        }

        #endregion

        #region Indices

        /**
         * This is an expensive method. Please cache and re-use the result.
         */
        public short GetLayerIndex(FixedString128Bytes layerName) => GetControllerBlob().GetLayerIndex(layerName);

        /**
         * This is an expensive method. Please cache and re-use the result.
         */
        public short GetParameterIndex(FixedString64Bytes parameterName) => GetControllerBlob().GetParameterIndex(parameterName);
        public short GetParameterIndex(int parameterNameHashCode) => GetControllerBlob().GetParameterIndex(parameterNameHashCode);

        #endregion

        #region Parameters

        /// <summary>
        /// Returns the value of the given float parameter.
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters. (fastest)</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public float GetFloatParameter(short index)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Float))
                return 0f;
            return parameters[index].floatParam;
        }

        /// <summary>
        /// Returns the value of the given float parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public float GetFloatParameter(FixedString64Bytes name)
        {
            return GetFloatParameter(GetParameterIndex(name));
        }

        /// <summary>
        /// Sets the value of the float parameter at index (fastest)
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters.</param>
        /// <param name="value">The value to set for the parameter. </param>
        public void SetFloatParameter(short index, float value)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Float))
                return;
            parameters.ElementAt(index).floatParam = value;
        }

        /// <summary>
        /// Sets the value of the given float parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to set for the parameter. </param>
        public void SetFloatParameter(FixedString64Bytes name, float value)
        {
            SetFloatParameter(GetParameterIndex(name), value);
        }

        /// <summary>
        /// Returns the value of the given int parameter.
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters. (fastest)</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public int GetIntParameter(short index)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Int))
                return 0;
            return parameters[index].intParam;
        }

        /// <summary>
        /// Returns the value of the given int parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public int GetIntParameter(FixedString64Bytes name)
        {
            return GetIntParameter(GetParameterIndex(name));
        }

        /// <summary>
        /// Sets the value of the int parameter at index (fastest)
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters.</param>
        /// <param name="value">The value to set for the parameter. </param>
        public void SetIntParameter(short index, int value)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Int))
                return;
            parameters.ElementAt(index).intParam = value;
        }

        /// <summary>
        /// Sets the value of the given int parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to set for the parameter. </param>
        public void SetIntParameter(FixedString64Bytes name, int value)
        {
            SetIntParameter(GetParameterIndex(name), value);
        }

        /// <summary>
        /// Returns the value of the given bool parameter.
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters. (fastest)</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public bool GetBoolParameter(short index)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Bool))
                return false;
            return parameters[index].boolParam;
        }

        /// <summary>
        /// Returns the value of the given bool parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public bool GetBoolParameter(FixedString64Bytes name)
        {
            return GetBoolParameter(GetParameterIndex(name));
        }

        /// <summary>
        /// Sets the value of the bool parameter at index (fastest)
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters.</param>
        /// <param name="value">The value to set for the parameter. </param>
        public void SetBoolParameter(short index, bool value)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Bool))
                return;
            parameters.ElementAt(index).boolParam = value;
        }

        /// <summary>
        /// Sets the value of the given bool parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to set for the parameter. </param>
        public void SetBoolParameter(FixedString64Bytes name, bool value)
        {
            SetBoolParameter(GetParameterIndex(name), value);
        }

        /// <summary>
        /// Returns the value of the given trigger parameter.
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters. (fastest)</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public bool GetTriggerParameter(short index)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Trigger))
                return false;
            return parameters[index].triggerParam;
        }

        /// <summary>
        /// Returns the value of the given trigger parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public bool GetTriggerParameter(FixedString64Bytes name)
        {
            return GetTriggerParameter(GetParameterIndex(name));
        }

        /// <summary>
        /// Sets the trigger parameter at index (fastest)
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters.</param>
        public void SetTrigger(short index)
        {
            SetTriggerValue(index, true);
        }

        /// <summary>
        /// Sets the given trigger parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        public void SetTrigger(FixedString64Bytes name)
        {
            SetTriggerValue(GetParameterIndex(name), true);
        }

        /// <summary>
        /// Clears the trigger parameter at index (fastest)
        /// </summary>
        /// <param name="index">The parameter index in the Mecanim controller's list of parameters.</param>
        public void ClearTrigger(short index)
        {
            SetTriggerValue(index, false);
        }

        /// <summary>
        /// Clears the given trigger parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        public void ClearTrigger(FixedString64Bytes name)
        {
            SetTriggerValue(GetParameterIndex(name), false);
        }

        private void SetTriggerValue(short index, bool value)
        {
            if (!IsValidParameterOfType(index, MecanimControllerBlob.ParameterTypes.Type.Trigger))
                return;
            parameters.ElementAt(index).triggerParam = value;
        }

        private bool IsValidParameterOfType(short index, MecanimControllerBlob.ParameterTypes.Type parameterType)
        {
            if (index < 0)
            {
                Debug.LogError("The Mecanim parameter was not found.");
                return false;
            }
            if (GetControllerBlob().parameterTypes[index] != parameterType)
            {
                Debug.LogError($"The Mecanim parameter at index {index} is not the expected type. It's a '{GetControllerBlob().parameterNames[index]}'");
                return false;
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Orders Mecanim to start a new inertial blend of the given duration
        /// </summary>
        /// <param name="durationSeconds">Total duration of the inertial blend (in seconds).</param>
        #region Inertial Blend
        public void StartInertialBlend(float durationSeconds)
        {
            m_controller.ValueRW.StartInertialBlend(durationSeconds);
        }
        #endregion
    }

    public struct StateHandle
    {
        public short StateMachineIndex;
        public short StateIndex;
    }
}

