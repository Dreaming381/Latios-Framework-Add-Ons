using Latios.Psyshock;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Latios.Anna.Systems
{
    [UpdateInGroup(typeof(ConstraintWritingSuperSystem), OrderFirst = true)]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct BuildBroadphaseCollisionWorldSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged latiosWorld;

        BuildCollisionWorldTypeHandles m_handles;
        EntityQuery                    m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
            m_handles   = new BuildCollisionWorldTypeHandles(ref state);
            m_query     = state.Fluent().WithAnyEnabled<EnvironmentCollisionTag, KinematicCollisionTag, RigidBody>(true).PatchQueryForBuildingCollisionWorld().Build();
        }

        public void OnNewScene(ref SystemState state)
        {
            latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<BroadphaseCollisionWorld>(default);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_handles.Update(ref state);
            var physicsSettings = latiosWorld.GetPhysicsSettings();

            state.Dependency = Physics.BuildCollisionWorld(m_query, in m_handles).WithSettings(physicsSettings.collisionLayerSettings)
                               .ScheduleParallel(out var collisionWorld, state.WorldUpdateAllocator, state.Dependency);

            latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new BroadphaseCollisionWorld
            {
                collisionWorld = collisionWorld
            });
        }
    }
}

