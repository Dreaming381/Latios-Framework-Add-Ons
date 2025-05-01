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

    [DisableAutoCreation]
    [BurstCompile]
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
            var     w     = latiosWorldUnmanaged;
            ref var state = ref CheckedStateRef;
            BeforeUpdate(ref w, ref state);
            base.OnUpdate();
            AfterUpdate(ref w);
        }

        [BurstCompile]
        static void BeforeUpdate(ref LatiosWorldUnmanaged world, ref SystemState state)
        {
            var settings = world.GetPhysicsSettings();
            var dt       = state.WorldUnmanaged.Time.DeltaTime;
            UnitySim.ConstraintTauAndDampingFrom(UnitySim.kStiffSpringFrequency, UnitySim.kStiffDampingRatio, dt, settings.numIterations, out var tau, out var damping);
            world.sceneBlackboardEntity.SetComponentData(new ConstraintWritingConstants
            {
                constraintStartGlobalVersion = state.GlobalSystemVersion,
                deltaTime                    = dt,
                inverseDeltaTime             = 1f / dt,
                isInConstraintWritingPhase   = true,
                numIterations                = settings.numIterations,
                numSubSteps                  = 1,
                stiffDamping                 = damping,
                stiffTau                     = tau
            });
        }

        [BurstCompile]
        static void AfterUpdate(ref LatiosWorldUnmanaged world)
        {
            world.sceneBlackboardEntity.SetComponentData<ConstraintWritingConstants>(default);
        }
    }
}

