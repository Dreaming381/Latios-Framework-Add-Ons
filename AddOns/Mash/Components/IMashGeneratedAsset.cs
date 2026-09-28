using System;
using Latios.Mash.Authoring;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace Latios.Mash
{
    /// <summary>
    /// Generated once per .inputactions asset. MashAuthoring's baker finds the one whose
    /// <see cref="sourceAssetGuid"/> matches the assigned asset and lets it add the generated
    /// components. MashReceiverAndPollSystem uses the types it lists. You shouldn't need to
    /// implement this yourself.
    /// </summary>
    public interface IMashGeneratedAsset
    {
        /// <summary>The AssetDatabase GUID of the .inputactions asset this was generated for</summary>
        string sourceAssetGuid { get; }

        /// <summary>The generated receiver type, which implements IMashReceiver</summary>
        Type receiverType { get; }

        /// <summary>The generated AssetRef component type, which implements IMashAssetRef</summary>
        Type assetRefType { get; }

        /// <summary>The generated InitializationFailed tag component type</summary>
        Type initializationFailedType { get; }

        /// <summary>
        /// Adds the AssetRef, state components, event buffers, and the receiver's ExistComponent
        /// to the entity.
        /// </summary>
        void Bake(Baker<MashAuthoring> baker, Entity entity, InputActionAsset sourceAsset);
    }
}
