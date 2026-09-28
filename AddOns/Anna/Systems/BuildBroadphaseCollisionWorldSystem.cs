using Latios.Psyshock;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Latios.Anna.Systems
{
    [UpdateInGroup(typeof(ConstraintWritingSuperSystem), OrderFirst = true)]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct BuildBroadphaseCollisionWorldSystem : ISystem, ISystemNewScene, ILatiosApi
    {
        EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);
            m_query = state.Fluent().WithAnyEnabled<EnvironmentCollisionTag, KinematicCollisionTag, RigidBody>(true).PatchQueryForBuildingCollisionWorld().Build();
        }

        public void OnNewScene(ref SystemState state)
        {
            this.GetApi(ref state).sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<BroadphaseCollisionWorld>(default);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var api             = this.GetApi(ref state);
            var physicsSettings = api.latiosWorld.GetPhysicsSettings();
            var handles         = api.Get<BuildCollisionWorldTypeHandles>();
            state.Dependency    = Physics.BuildCollisionWorld(m_query, in handles).WithSettings(physicsSettings.collisionLayerSettings).WithWorldIndex(2)
                                  .ScheduleParallel(out var collisionWorld, state.WorldUpdateAllocator, state.Dependency);

            api.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new BroadphaseCollisionWorld
            {
                collisionWorld = collisionWorld
            });
        }
    }
}

