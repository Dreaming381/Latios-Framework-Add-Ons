using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace Latios.Anna.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct IntegrateRigidBodiesSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var captured = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<CapturedRigidBodies>(true);
            new IntegrateRigidBodiesJob
            {
                entityToIndexMap = captured.entityToSrcIndexMap,
                states           = captured.states,
                deltaTime        = Time.DeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct IntegrateRigidBodiesJob : IJobEntity
        {
            [ReadOnly] public NativeParallelHashMap<Entity, int>  entityToIndexMap;
            [ReadOnly] public NativeArray<CapturedRigidBodyState> states;
            public float                                          deltaTime;

            public void Execute(Entity entity, TransformAspect transform, ref RigidBody rigidBody)
            {
                if (!entityToIndexMap.TryGetValue(entity, out var index))
                    return;
                var state                = states.AsReadOnlySpan()[index];
                var previousInertialPose = state.inertialPoseWorldTransform;
                if (!math.all(math.isfinite(state.velocity.linear)))
                    rigidBody.velocity.linear = float3.zero;
                if (!math.all(math.isfinite(state.velocity.angular)))
                    rigidBody.velocity.angular = float3.zero;
                UnitySim.Integrate(ref state.inertialPoseWorldTransform, ref state.velocity, state.linearDamping, state.angularDamping, deltaTime);
                transform.worldTransform = UnitySim.ApplyInertialPoseWorldTransformDeltaToWorldTransform(transform.worldTransform,
                                                                                                         in previousInertialPose,
                                                                                                         in state.inertialPoseWorldTransform);
                rigidBody.velocity = state.velocity;
            }
        }
    }
}

