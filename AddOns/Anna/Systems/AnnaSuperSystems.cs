using Latios.Psyshock;
using Latios.Transforms.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSuperSystem))]
    public partial class AnnaSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            EnableSystemSorting = false;

            GetOrCreateAndAddUnmanagedSystem<CollectRigidBodiesSystem>();
            GetOrCreateAndAddUnmanagedSystem<CollectKinematicCollidersSystem>();
            GetOrCreateAndAddManagedSystem<ConstraintWritingSuperSystem>();
            GetOrCreateAndAddUnmanagedSystem<SolveSystem>();
            GetOrCreateAndAddUnmanagedSystem<IntegrateRigidBodiesSystem>();
        }
    }

    public partial class ConstraintWritingSuperSystem : SuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddUnmanagedSystem<BuildBroadphaseCollisionWorldSystem>();
            GetOrCreateAndAddUnmanagedSystem<CreateRigidBodyAxesLockConstraintsSystem>();
            GetOrCreateAndAddUnmanagedSystem<FindCollisionsSystem>();

            EnableSystemSorting = true;
        }

        public override void OnNewScene()
        {
            sceneBlackboardEntity.AddComponent<ConstraintWritingConstants>();
        }

        protected override void OnUpdate()
        {
            var settings = latiosWorldUnmanaged.GetPhysicsSettings();
            var dt       = SystemAPI.Time.DeltaTime;
            UnitySim.ConstraintTauAndDampingFrom(UnitySim.kStiffSpringFrequency, UnitySim.kStiffDampingRatio, dt, settings.numIterations, out var tau, out var damping);
            sceneBlackboardEntity.SetComponentData(new ConstraintWritingConstants
            {
                constraintStartGlobalVersion = GlobalSystemVersion,
                deltaTime                    = dt,
                inverseDeltaTime             = 1f / dt,
                isInConstraintWritingPhase   = true,
                numSubSteps                  = 1,
                stiffDamping                 = damping,
                stiffTau                     = tau
            });
            base.OnUpdate();
            sceneBlackboardEntity.SetComponentData<ConstraintWritingConstants>(default);
        }
    }
}

