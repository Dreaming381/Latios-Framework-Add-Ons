using Unity.Entities;

namespace Latios.MecanimV2
{
    public static class MecanimV2Bootstrap
    {
        /// <summary>
        /// Installs the Mecanim v2 state machine runtime systems. This should only be installed in the runtime world.
        /// </summary>
        /// <param name="world"></param>
        public static void InstallMecanimV2Addon(World world)
        {
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<UpdateMecanimSystem>(), world);
        }
    }
}

