using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Terrainy.Authoring
{
    public static class TerrainyBakingBootstrap
    {
        /// <summary>
        /// Adds Terrainy bakers and baking systems into baking world
        /// </summary>
        /// <param name="context">The baking context in which to install the Terrainy bakers and baking systems</param>
        public static void InstallTerrainy(ref CustomBakingBootstrapContext context)
        {
            context.filteredBakerTypes.Add(typeof(TerrainAuthoring));
            context.optimizationSystemTypesToInject.Add(TypeManager.GetSystemTypeIndex<RemoveTerrainLiveBakedSystem>());
        }
    }
}

