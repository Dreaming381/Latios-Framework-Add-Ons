using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    public static class AnnaBootstrap
    {
        /// <summary>
        /// Installs Anna Physics and returns the main AnnaSuperSystem in case you want to attach an IRateManager to it
        /// (such as SubstepRateManager)
        /// </summary>
        public static Systems.AnnaSuperSystem InstallAnna(World world)
        {
            var result = BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.AnnaSuperSystem>(), world);
            return result.systemManaged as Systems.AnnaSuperSystem;
        }
    }
}

