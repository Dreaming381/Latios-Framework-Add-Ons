using Unity.Entities;

namespace Latios.Mash
{
    public static class MashBootstrap
    {
        /// <summary>
        /// Installs Mash into a runtime World. Mash only works in play mode and in builds, so this
        /// logs a warning and does nothing if you pass it an Editor World.
        /// </summary>
        /// <param name="world">The runtime World to install Mash into</param>
        public static void InstallMash(World world)
        {
            if ((world.Flags & WorldFlags.Editor) == WorldFlags.Editor)
            {
                UnityEngine.Debug.LogWarning(
                    $"Mash: Skipping install into Editor world \"{world.Name}\". Mash only runs in play mode and in builds. Install it from your ICustomBootstrap, not your ICustomEditorBootstrap.");
                return;
            }

            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.MashReceiverAndPollSystem>(), world);
        }
    }
}
