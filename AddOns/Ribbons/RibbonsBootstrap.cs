using Unity.Entities;

namespace Latios.Ribbons
{
    public static class RibbonsBootstrap
    {
        /// <summary>
        /// Installs Ribbons into the World. Install it in both the editor and runtime worlds, after Kinemation.
        /// </summary>
        /// <param name="world">The world to install Ribbons into</param>
        public static void InstallRibbons(World world)
        {
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.BuildLineRibbonMeshSystem>(),               world);
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.RibbonTrailSystem>(),                       world);

#if UNITY_EDITOR
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.LiveBakingEnableChangedRibbonLineSystem>(), world);
#endif
        }
    }
}

