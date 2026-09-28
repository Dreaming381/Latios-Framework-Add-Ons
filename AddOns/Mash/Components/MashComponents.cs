using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace Latios.Mash
{
    /// <summary>
    /// One event from a Button or Pass-Through action. Each action map stores these in a
    /// DynamicMultiList, with one list per action.
    /// </summary>
    public struct MashActionEvent
    {
        /// <summary>The action's channel index within its map's generated EventChannels class</summary>
        public int actionIndex;
        public InputActionPhase phase;
        public double time;
        public double duration;
        /// <summary>The action's value when the event happened. Unused components are zero.</summary>
        public float4 value;
    }

    /// <summary>
    /// Implemented by each generated AssetRef component, which holds the InputActionAsset an entity
    /// was baked from and the entity's private copy of it.
    /// </summary>
    public interface IMashAssetRef
    {
        UnityObjectRef<InputActionAsset> sourceAsset { get; }
        UnityObjectRef<InputActionAsset> runtimeAsset { get; set; }
    }

    /// <summary>
    /// Implemented by each generated receiver. MashReceiverAndPollSystem finds every generated asset
    /// with reflection and drives its receiver.
    /// </summary>
    public interface IMashReceiver
    {
        /// <summary>
        /// True between a successful <see cref="Initialize(InputActionAsset)"/> and
        /// <see cref="Uninitialize"/>. This must come from reference-typed state owned by the
        /// receiver, not from an entity component. Instantiating an entity copies its components,
        /// and the copy must not think it owns the original's asset.
        /// </summary>
        bool isInitialized { get; }

        /// <summary>
        /// The private asset copy passed to <see cref="Initialize(InputActionAsset)"/>, or null if
        /// not initialized
        /// </summary>
        InputActionAsset runtimeAsset { get; }

        /// <summary>
        /// Called once with a fresh copy of the source asset. Subscribe callbacks and enable the
        /// action maps here. If this throws, Mash calls <see cref="Uninitialize"/>, so that needs
        /// to handle a half-finished Initialize.
        /// </summary>
        void Initialize(InputActionAsset runtimeAsset);

        /// <summary>
        /// Called every frame at the start of SimulationSystemGroup. Write action values to the
        /// state components, and move buffered events into the event buffers.
        /// </summary>
        void PollAndDrain(EntityManager entityManager, Entity entity);

        /// <summary>
        /// Unsubscribe callbacks and destroy the asset copy with
        /// <see cref="MashRuntimeAsset.Destroy(InputActionAsset)"/>. This must be safe to call
        /// twice, and after a half-finished Initialize.
        /// </summary>
        void Uninitialize();
    }
}
