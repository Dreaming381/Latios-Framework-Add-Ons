using Latios.Authoring;
using Unity.Entities;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Static class containing installers for Peekaboo's baking
    /// </summary>
    public static class PeekabooBakingBootstrap
    {
        /// <summary>
        /// Adds the baking system that bakes occluder plates for every RenderMeshArray.
        /// </summary>
        /// <param name="context">The baking context to install the Peekaboo baking system into</param>
        /// <remarks>
        /// Install this after Kinemation, since plates are baked from the RenderMeshArray that
        /// Kinemation's renderer baking produces.
        /// </remarks>
        public static void InstallPeekaboo(ref CustomBakingBootstrapContext context)
        {
            context.bakingSystemTypesToInject.Add(TypeManager.GetSystemTypeIndex<Systems.BakeOccluderPlatesSystem>());
        }
    }
}
