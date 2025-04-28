using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Latios.Anna.Systems
{
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct CreateRigidBodyAxesLockConstraintsSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;
        BlackboardEntity     systemBlackboard;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld      = state.GetLatiosWorldUnmanaged();
            systemBlackboard = new BlackboardEntity(state.SystemHandle, latiosWorld);
            systemBlackboard.AddOrSetCollectionComponentAndDisposeOld(new ConstraintWriter());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var infoLookup       = latiosWorld.sceneBlackboardEntity.GetCollectionAspect<ConstraintEntityInfoLookup>();
            var constraintWriter = new ConstraintWriter(ref state, latiosWorld);
            new Job
            {
                infoLookup       = infoLookup,
                constraintWriter = constraintWriter
            }.Schedule();

            systemBlackboard.SetCollectionComponentAndDisposeOld(constraintWriter);
        }

        [WithAll(typeof(RigidBody), typeof(WorldTransform))]
        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly] public ConstraintEntityInfoLookup infoLookup;
            public ConstraintWriter                      constraintWriter;

            public void Execute(Entity entity, LockWorldAxesFlags axes)
            {
                if (axes.packedFlags == default)
                    return;

                infoLookup.TryGetRigidBodyHandle(entity, out var handle);
                constraintWriter.LockWorldAxes(ref infoLookup, handle, axes);
            }
        }
    }
}

