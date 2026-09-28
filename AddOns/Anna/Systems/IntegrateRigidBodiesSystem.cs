using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace Latios.Anna.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct IntegrateRigidBodiesSystem : ISystem, ILatiosApi
    {
        LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);
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
                deltaTime        = Time.DeltaTime,
            }.ScheduleParallel(this.GetApi(ref state));
        }

        [BurstCompile]
        partial struct IntegrateRigidBodiesJob : IJobEach
        {
            [ReadOnly] public NativeParallelHashMap<Entity, int>  entityToIndexMap;
            [ReadOnly] public NativeArray<CapturedRigidBodyState> states;
            public float                                          deltaTime;

            public void Execute(Entity entity, TransformDeferableAspect transform, ref RigidBody rigidBody)
            {
                if (!entityToIndexMap.TryGetValue(entity, out var index))
                    return;
                var state                = states[index];
                var previousInertialPose = state.inertialPoseWorldTransform;
                if (!math.all(math.isfinite(state.velocity.linear)))
                    state.velocity.linear = float3.zero;
                if (!math.all(math.isfinite(state.velocity.angular)))
                    state.velocity.angular = float3.zero;
                UnitySim.Integrate(ref state.inertialPoseWorldTransform, ref state.velocity, state.linearDamping, state.angularDamping, deltaTime);
                var worldTransform = UnitySim.ApplyInertialPoseWorldTransformDeltaToWorldTransform(transform.worldTransform,
                                                                                                   in previousInertialPose,
                                                                                                   in state.inertialPoseWorldTransform);
                worldTransform.rotation  = math.normalize(worldTransform.rotation);
                transform.worldTransform = worldTransform;
                rigidBody.velocity       = state.velocity;
            }
        }
    }
}

