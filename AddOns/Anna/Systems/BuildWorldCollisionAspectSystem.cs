#if LATIOS_ADDON_SHOCKWAVE

using Latios.Psyshock;
using Latios.Shockwave;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Transforms.Systems.TransformSuperSystem))]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct BuildWorldCollisionAspectSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged latiosWorld;
        EntityQuery m_rigidBodyQuery;
        EntityQuery m_kinematicQuery;
        EntityQuery m_allCollidersQuery;
        BuildCollisionWorldTypeHandles m_handles;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld      = state.GetLatiosWorldUnmanaged();
            m_rigidBodyQuery = state.Fluent().With<RigidBody>(true).With<CollisionWorldAabb>(false)
                               .Without<KinematicCollisionTag, ExcludeFromWorldCollisionAspectTag>().PatchQueryForBuildingCollisionWorld().Build();
            m_kinematicQuery = state.Fluent().With<KinematicCollisionTag, PreviousTransform>(true).With<CollisionWorldAabb>(false)
                               .Without<ExcludeFromWorldCollisionAspectTag>().PatchQueryForBuildingCollisionWorld().Build();
            m_allCollidersQuery = state.Fluent().Without<ExcludeFromWorldCollisionAspectTag>().PatchQueryForBuildingCollisionWorld().Build();
            m_handles           = new BuildCollisionWorldTypeHandles(ref state);
        }

        public void OnNewScene(ref SystemState state)
        {
            var settings = latiosWorld.GetPhysicsSettings();
            latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld(new ShockwaveWorldCollision
            {
                collisionWorld = CollisionWorld.CreateEmptyCollisionWorld(settings.collisionLayerSettings, state.WorldUpdateAllocator)
            });
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = latiosWorld.GetPhysicsSettings();

            if (m_allCollidersQuery.IsEmptyIgnoreFilter)
            {
                latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new ShockwaveWorldCollision
                {
                    collisionWorld = CollisionWorld.CreateEmptyCollisionWorld(settings.collisionLayerSettings, state.WorldUpdateAllocator)
                });
                return;
            }

            new UpdateAabbsJob().ScheduleParallel(m_rigidBodyQuery);
            new UpdateAabbsJob().ScheduleParallel(m_kinematicQuery);

            m_handles.Update(ref state);

            state.Dependency = Physics.BuildCollisionWorld(m_allCollidersQuery, in m_handles).WithSettings(settings.collisionLayerSettings)
                               .ScheduleParallel(out var world, state.WorldUpdateAllocator, state.Dependency);
            latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new ShockwaveWorldCollision { collisionWorld = world });
        }

        [BurstCompile]
        partial struct UpdateAabbsJob : IJobEntity
        {
            public void Execute(ref CollisionWorldAabb aabb, in WorldTransform transform, in Collider collider)
            {
                var newAabb = Physics.AabbFrom(in collider, in transform.worldTransform);
                aabb.aabb = Physics.CombineAabb(aabb.aabb, newAabb);
            }
        }
    }
}
#endif

