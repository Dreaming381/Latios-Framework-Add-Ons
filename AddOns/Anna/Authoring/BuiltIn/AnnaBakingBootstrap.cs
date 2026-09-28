using Latios.Authoring;
using Unity.Entities;

namespace Latios.Anna.Authoring
{
    /// <summary>
    /// Static class containing installers for Anna's optional baking features
    /// </summary>
    public static class AnnaBakingBootstrap
    {
        /// <summary>
        /// Bakes Unity's built-in physics components into Anna.
        /// </summary>
        /// <param name="context">The context passed into ICustomBakingBootstrap</param>
        /// <param name="disableGameObjectPhysicsInPlayMode">If true, Game Object physics switches to SimulationMode.Script
        /// while a subscene is open in play mode, as otherwise the engine tries to simulate the authoring rigid bodies.</param>
        public static void InstallUnityRigidBodyBakers(ref CustomBakingBootstrapContext context, bool disableGameObjectPhysicsInPlayMode = true)
        {
            context.filteredBakerTypes.Add(typeof(RigidbodyBaker));
            context.filteredBakerTypes.Add(typeof(RigidbodyJointsBaker));

            CollisionTagAuthoringBaker.sEnableUnityRigidBodyBaking = true;

#if UNITY_EDITOR
            if (disableGameObjectPhysicsInPlayMode)
                context.bakingSystemTypesToInject.Add(TypeManager.GetSystemTypeIndex<Systems.DisableGameObjectPhysicsBakingSystem>());
#endif
        }
    }
}

