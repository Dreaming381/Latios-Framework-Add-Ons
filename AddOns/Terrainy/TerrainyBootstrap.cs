using Latios.Terrainy.Systems;
using Unity.Entities;

namespace Latios.Terrainy
{
    public static class TerrainyBootstrap
    {
        /// <summary>
        /// Install Terrainy systems into the World. Required for both Editor and Runtime worlds.
        /// </summary>
        /// <param name="world">The world to install Terrainy into.</param>
        public static void InstallTerrainy(World world)
        {
            BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<TerrainSystem>(), world);
        }
    }
}

