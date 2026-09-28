#if UNITY_EDITOR
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Latios.Anna.Authoring.Systems
{
    /// <summary>
    /// Switches Game Object physics to script mode while a subscene is open in play mode. Otherwise Game Object physics
    /// moves the authoring Rigidbodies, which rebakes the subscene every frame.
    /// </summary>
    [DisableAutoCreation]
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    internal partial class DisableGameObjectPhysicsBakingSystem : SystemBase
    {
        static bool           s_overridden;
        static bool           s_subscribed;
        static SimulationMode s_savedMode;

        protected override void OnUpdate()
        {
            if (s_overridden || !Application.isPlaying)
                return;

            if (!s_subscribed)
            {
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                s_subscribed                            = true;
            }

            s_savedMode  = Physics.simulationMode;
            s_overridden = true;
            if (s_savedMode == SimulationMode.Script)
                return;

            Physics.simulationMode = SimulationMode.Script;
            Debug.Log(
                "Anna switched Game Object physics to SimulationMode.Script because a subscene is open in play mode. It switches back when you exit play mode. To keep Game Object physics running, pass false for disableGameObjectPhysicsInPlayMode in AnnaBakingBootstrap.InstallAnnaBakers().");
        }

        static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingPlayMode || !s_overridden)
                return;
            Physics.simulationMode = s_savedMode;
            s_overridden           = false;
        }
    }
}
#endif
