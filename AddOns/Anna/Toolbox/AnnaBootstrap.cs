using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    public static class AnnaBootstrap
    {
        public static void InstallAnna(World world)
        {
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.AnnaSuperSystem>(),                 world);

#if LATIOS_ADDON_SHOCKWAVE
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<Systems.BuildWorldCollisionAspectSystem>(), world);
#endif
        }
    }
}

