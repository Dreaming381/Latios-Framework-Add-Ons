using UnityEngine;
using UnityEngine.InputSystem;

namespace Latios.Mash
{
    /// <summary>
    /// Creates and destroys the private copy of an InputActionAsset that each Mash entity owns.
    /// </summary>
    public static class MashRuntimeAsset
    {
        /// <summary>
        /// Creates a private copy of the source asset for one entity.
        /// </summary>
        public static InputActionAsset Create(InputActionAsset sourceAsset)
        {
            return Object.Instantiate(sourceAsset);
        }

        /// <summary>
        /// Disables and destroys a copy made by <see cref="Create(InputActionAsset)"/>. Passing null
        /// is fine. So is calling this while exiting play mode, when Object.Destroy is no longer allowed.
        /// </summary>
        public static void Destroy(InputActionAsset runtimeAsset)
        {
            if (runtimeAsset == null)
                return;

            runtimeAsset.Disable();
            if (Application.isPlaying)
                Object.Destroy(runtimeAsset);
            else
                Object.DestroyImmediate(runtimeAsset);
        }
    }
}
