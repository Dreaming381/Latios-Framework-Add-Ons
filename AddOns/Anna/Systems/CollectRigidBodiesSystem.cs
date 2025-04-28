using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace Latios.Anna.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct CollectRigidBodiesSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged latiosWorld;
        EntityQuery          m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();

            m_query = state.Fluent().With<WorldTransform>(true).With<RigidBody>(false).Without<KinematicCollisionTag>().Build();
        }

        public void OnNewScene(ref SystemState state)
        {
            latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<CapturedRigidBodies>( default);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsSettings = latiosWorld.GetPhysicsSettings();
            var count           = m_query.CalculateEntityCountWithoutFiltering();
            if (count == 0)
            {
                latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new CapturedRigidBodies
                {
                    entityToSrcIndexMap = new NativeParallelHashMap<Entity, int>(1, state.WorldUpdateAllocator),
                    states              = CollectionHelper.CreateNativeArray<CapturedRigidBodyState>(0, state.WorldUpdateAllocator)
                });
                return;
            }

            var startIndices     = m_query.CalculateBaseEntityIndexArrayAsync(state.WorldUpdateAllocator, default, out var jh);
            var states           = CollectionHelper.CreateNativeArray<CapturedRigidBodyState>(count, state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
            var entityToIndexMap = new NativeParallelHashMap<Entity, int>(count, state.WorldUpdateAllocator);

            jh = new BuildJob
            {
                aabbHandle       = GetComponentTypeHandle<CollisionWorldAabb>(false),
                addImpulseHandle = GetBufferTypeHandle<AddImpulse>(false),
                bucketCalculator = new CollisionLayerBucketIndexCalculator(in physicsSettings.collisionLayerSettings),
                colliderHandle   = GetComponentTypeHandle<Collider>(true),
                dt               = Time.DeltaTime,
                entityHandle     = GetEntityTypeHandle(),
                entityToIndexMap = entityToIndexMap.AsParallelWriter(),
                physicsSettings  = physicsSettings,
                rigidBodyHandle  = GetComponentTypeHandle<RigidBody>(false),
                startIndices     = startIndices,
                states           = states,
                transformHandle  = GetComponentTypeHandle<WorldTransform>(true)
            }.ScheduleParallel(m_query, JobHandle.CombineDependencies(state.Dependency, jh));

            latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new CapturedRigidBodies
            {
                entityToSrcIndexMap = entityToIndexMap,
                states              = states
            });
            state.Dependency = jh;
        }

        [BurstCompile]
        partial struct BuildJob : IJobChunk
        {
            [ReadOnly] public EntityTypeHandle                    entityHandle;
            [ReadOnly] public ComponentTypeHandle<WorldTransform> transformHandle;
            [ReadOnly] public ComponentTypeHandle<Collider>       colliderHandle;
            [ReadOnly] public NativeArray<int>                    startIndices;

            public ComponentTypeHandle<RigidBody>          rigidBodyHandle;
            public BufferTypeHandle<AddImpulse>            addImpulseHandle;
            public ComponentTypeHandle<CollisionWorldAabb> aabbHandle;

            [NativeDisableParallelForRestriction] public NativeArray<CapturedRigidBodyState> states;
            public NativeParallelHashMap<Entity, int>.ParallelWriter                         entityToIndexMap;

            public PhysicsSettings                     physicsSettings;
            public CollisionLayerBucketIndexCalculator bucketCalculator;
            public float                               dt;

            public unsafe void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var entities    = chunk.GetEntityDataPtrRO(entityHandle);
                var transforms  = (WorldTransform*)chunk.GetRequiredComponentDataPtrRO(ref transformHandle);
                var colliders   = (Collider*)chunk.GetRequiredComponentDataPtrRO(ref colliderHandle);
                var rigidBodies = (RigidBody*)chunk.GetRequiredComponentDataPtrRW(ref rigidBodyHandle);
                var impulses    = chunk.GetBufferAccessor(ref addImpulseHandle);
                var aabbs       = (Aabb*)chunk.GetRequiredComponentDataPtrRW(ref aabbHandle);

                for (int i = 0, index = startIndices[unfilteredChunkIndex]; i < chunk.Count; i++, index++)
                {
                    entityToIndexMap.TryAdd(entities[i], index);

                    var     entity    = entities[i];
                    ref var transform = ref transforms[i];
                    ref var collider  = ref colliders[i];
                    ref var rigidBody = ref rigidBodies[i];

                    var aabb              = Physics.AabbFrom(in collider, in transform.worldTransform);
                    var angularExpansion  = UnitySim.AngularExpansionFactorFrom(in collider);
                    var localCenterOfMass = UnitySim.LocalCenterOfMassFrom(in collider);
                    var localInertia      = UnitySim.LocalInertiaTensorFrom(in collider, transform.stretch);
                    UnitySim.ConvertToWorldMassInertia(in transform.worldTransform,
                                                       in localInertia,
                                                       localCenterOfMass,
                                                       rigidBody.inverseMass,
                                                       out var mass,
                                                       out var inertialPoseWorldTransform);

                    rigidBody.velocity.linear += physicsSettings.gravity * dt;
                    if (impulses.Length > 0)
                    {
                        foreach (var impulse in impulses[i])
                        {
                            if (math.all(math.isnan(impulse.pointOrAxis)))
                                UnitySim.ApplyFieldImpulse(ref rigidBody.velocity, in mass, impulse.impulse);
                            else if (math.all(math.isnan(impulse.impulse.yz)))
                                UnitySim.ApplyAngularImpulse(ref rigidBody.velocity, in mass, in inertialPoseWorldTransform, impulse.pointOrAxis, impulse.impulse.x);
                            else
                                UnitySim.ApplyImpulseAtWorldPoint(ref rigidBody.velocity, in mass, in inertialPoseWorldTransform, impulse.pointOrAxis, impulse.impulse);
                        }
                        impulses[i].Clear();
                    }

                    var motionExpansion = new UnitySim.MotionExpansion(in rigidBody.velocity, dt, angularExpansion);
                    aabb                = motionExpansion.ExpandAabb(aabb);
                    var bucketIndex     = bucketCalculator.BucketIndexFrom(in aabb);

                    aabbs[i] = aabb;

                    states[index] = new CapturedRigidBodyState
                    {
                        angularDamping                     = physicsSettings.angularDamping,
                        angularExpansion                   = angularExpansion,
                        bucketIndex                        = bucketIndex,
                        coefficientOfFriction              = rigidBody.coefficientOfFriction,
                        coefficientOfRestitution           = rigidBody.coefficientOfRestitution,
                        gravity                            = physicsSettings.gravity,
                        inertialPoseWorldTransform         = inertialPoseWorldTransform,
                        linearDamping                      = physicsSettings.linearDamping,
                        mass                               = mass,
                        motionExpansion                    = motionExpansion,
                        motionStabilizer                   = UnitySim.MotionStabilizer.kDefault,
                        numOtherSignificantBodiesInContact = 0,
                        velocity                           = rigidBody.velocity
                    };
                }
            }
        }
    }
}

