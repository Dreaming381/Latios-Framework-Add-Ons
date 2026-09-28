using Latios.Kinemation;
using Unity.Entities;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Installs Peekaboo's occlusion culling. Use it in both the editor and runtime worlds, after
    /// Kinemation.
    /// </summary>
    public static class PeekabooBootstrap
    {
        /// <summary>
        /// Installs occlusion culling into the World.
        /// </summary>
        /// <param name="world">The world to install Peekaboo into. Must be a LatiosWorld with Kinemation installed.</param>
        /// <param name="debugOverrideAllowWithoutBurst">
        /// Whether occlusion culling can run without Burst. Normally it turns itself off, since software
        /// rasterization without Burst costs far more than it saves. Turn this on to step through the
        /// culling code in a debugger.
        /// </param>
        public static void InstallPeekaboo(World world, bool debugOverrideAllowWithoutBurst = false)
        {
            var latiosWorld = world as LatiosWorld;
            if (latiosWorld == null)
                throw new System.InvalidOperationException("Peekaboo must be installed in a LatiosWorld.");
            if (latiosWorld.worldBlackboardEntity.HasComponent<NoGraphicsTag>())
                return;

            var settings                            = PeekabooSettings.Default;
            settings.debugOverrideAllowWithoutBurst = debugOverrideAllowWithoutBurst;
            latiosWorld.worldBlackboardEntity.AddComponentData(settings);

            var system = world.GetOrCreateSystem<Systems.PeekabooOcclusionCullingSystem>();
            KinemationBootstrap.InstallOcclusionCullingSystem(world, system);
        }
    }
}
