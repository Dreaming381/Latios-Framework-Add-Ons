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
    public partial struct BuildRigidBodyCollisionLayerSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged latiosWorld;
        EntityQuery          m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();

            m_query = state.Fluent().With<RigidBody>(false).Without<KinematicCollisionTag>().PatchQueryForBuildingCollisionLayer().Build();
        }

        public void OnNewScene(ref SystemState state)
        {
            latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<RigidBodyCollisionLayer>(  default);
            latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<CapturedRigidBodies>(      default);
            latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<BodyConstraintsPairStream>(default);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsSettings = latiosWorld.GetPhysicsSettings();
            var count           = m_query.CalculateEntityCountWithoutFiltering();
            if (count == 0)
            {
                latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new RigidBodyCollisionLayer
                {
                    layer = CollisionLayer.CreateEmptyCollisionLayer(physicsSettings.collisionLayerSettings, state.WorldUpdateAllocator)
                });
                latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new CapturedRigidBodies
                {
                    entityToSrcIndexMap = new NativeParallelHashMap<Entity, int>(1, state.WorldUpdateAllocator),
                    states              = CollectionHelper.CreateNativeArray<CapturedRigidBodyState>(0, state.WorldUpdateAllocator)
                });
                latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new BodyConstraintsPairStream
                {
                    pairStream = new PairStream(physicsSettings.collisionLayerSettings, state.WorldUpdateAllocator)
                });
                return;
            }

            var startIndices          = m_query.CalculateBaseEntityIndexArrayAsync(state.WorldUpdateAllocator, default, out var jh);
            var colliderBodies        = CollectionHelper.CreateNativeArray<ColliderBody>(count, state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
            var aabbs                 = CollectionHelper.CreateNativeArray<Aabb>(count, state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
            var states                = CollectionHelper.CreateNativeArray<CapturedRigidBodyState>(count, state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
            var entityToIndexMap      = new NativeParallelHashMap<Entity, int>(count, state.WorldUpdateAllocator);
            var lockConstraintsStream = new NativeStream(startIndices.Length, state.WorldUpdateAllocator);

            jh = new BuildJob
            {
                aabbs                    = aabbs,
                addImpulseHandle         = GetBufferTypeHandle<AddImpulse>(false),
                bucketCalculator         = new CollisionLayerBucketIndexCalculator(in physicsSettings.collisionLayerSettings),
                colliderBodies           = colliderBodies,
                colliderHandle           = GetComponentTypeHandle<Collider>(true),
                dt                       = Time.DeltaTime,
                entityHandle             = GetEntityTypeHandle(),
                entityToIndexMap         = entityToIndexMap.AsParallelWriter(),
                lockConstraintStream     = lockConstraintsStream.AsWriter(),
                lockWorldAxesFlagsHandle = GetComponentTypeHandle<LockWorldAxesFlags>(true),
                physicsSettings          = physicsSettings,
                rigidBodyHandle          = GetComponentTypeHandle<RigidBody>(false),
                startIndices             = startIndices,
                states                   = states,
                transformHandle          = GetComponentTypeHandle<WorldTransform>(true)
            }.ScheduleParallel(m_query, JobHandle.CombineDependencies(state.Dependency, jh));

            var jhA = Physics.BuildCollisionLayer(colliderBodies, aabbs).WithSettings(physicsSettings.collisionLayerSettings)
                      .ScheduleParallel(out var layer, state.WorldUpdateAllocator, jh);

            var pairStream = new PairStream(in physicsSettings.collisionLayerSettings, state.WorldUpdateAllocator);
            UnitySim.ConstraintTauAndDampingFrom(UnitySim.kStiffSpringFrequency,
                                                 UnitySim.kStiffDampingRatio,
                                                 Time.DeltaTime,
                                                 physicsSettings.numIterations,
                                                 out var tau,
                                                 out var damping);
            var jhB = new LockJob
            {
                lockConstraintStream = lockConstraintsStream.AsReader(),
                pairStream           = pairStream,
                lockDamping          = damping,
                lockTau              = tau,
            }.Schedule(jh);

            latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new RigidBodyCollisionLayer
            {
                layer = layer
            });
            latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new CapturedRigidBodies
            {
                entityToSrcIndexMap = entityToIndexMap,
                states              = states
            });
            latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new BodyConstraintsPairStream
            {
                pairStream = pairStream
            });
            state.Dependency = JobHandle.CombineDependencies(jhA, jhB);
        }

        struct LockConstraintData
        {
            public Entity             entity;
            public RigidTransform     inertialPoseWorldTransform;
            public int                bucketIndex;
            public int                srcIndex;
            public LockWorldAxesFlags lockFlags;
        }

        [BurstCompile]
        partial struct BuildJob : IJobChunk
        {
            [ReadOnly] public EntityTypeHandle                        entityHandle;
            [ReadOnly] public ComponentTypeHandle<WorldTransform>     transformHandle;
            [ReadOnly] public ComponentTypeHandle<Collider>           colliderHandle;
            [ReadOnly] public ComponentTypeHandle<LockWorldAxesFlags> lockWorldAxesFlagsHandle;

            [ReadOnly] public NativeArray<int> startIndices;

            public ComponentTypeHandle<RigidBody> rigidBodyHandle;
            public BufferTypeHandle<AddImpulse>   addImpulseHandle;

            [NativeDisableParallelForRestriction] public NativeArray<CapturedRigidBodyState> states;
            [NativeDisableParallelForRestriction] public NativeArray<ColliderBody>           colliderBodies;
            [NativeDisableParallelForRestriction] public NativeArray<Aabb>                   aabbs;
            public NativeParallelHashMap<Entity, int>.ParallelWriter                         entityToIndexMap;
            public NativeStream.Writer                                                       lockConstraintStream;

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
                var lockAxes    = chunk.GetComponentDataPtrRO(ref lockWorldAxesFlagsHandle);

                lockConstraintStream.BeginForEachIndex(unfilteredChunkIndex);

                for (int i = 0, index = startIndices[unfilteredChunkIndex]; i < chunk.Count; i++, index++)
                {
                    entityToIndexMap.TryAdd(entities[i], index);

                    var     entity    = entities[i];
                    ref var transform = ref transforms[i];
                    ref var collider  = ref colliders[i];

                    colliderBodies[index] = new ColliderBody
                    {
                        collider  = collider,
                        transform = transform.worldTransform,
                        entity    = entity,
                    };

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
                            if (impulse.pointImpulseOrZero.Equals(float3.zero))
                                UnitySim.ApplyFieldImpulse(ref rigidBody.velocity, in mass, impulse.pointOrField);
                            else
                                UnitySim.ApplyImpulseAtWorldPoint(ref rigidBody.velocity, in mass, in inertialPoseWorldTransform, impulse.pointOrField, impulse.pointImpulseOrZero);
                        }
                        impulses[i].Clear();
                    }

                    var motionExpansion = new UnitySim.MotionExpansion(in rigidBody.velocity, dt, angularExpansion);
                    aabb                = motionExpansion.ExpandAabb(aabb);
                    var bucketIndex     = bucketCalculator.BucketIndexFrom(in aabb);

                    aabbs[index] = aabb;

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

                    if (lockAxes != null)
                    {
                        lockConstraintStream.Write(new LockConstraintData
                        {
                            bucketIndex                = bucketIndex,
                            entity                     = entity,
                            inertialPoseWorldTransform = inertialPoseWorldTransform,
                            lockFlags                  = lockAxes[i],
                            srcIndex                   = index
                        });
                    }
                }

                lockConstraintStream.EndForEachIndex();
            }
        }

        [BurstCompile]
        struct LockJob : IJob
        {
            [ReadOnly] public NativeStream.Reader lockConstraintStream;
            public PairStream                     pairStream;

            public float lockTau;
            public float lockDamping;

            public void Execute()
            {
                int chunkCount = lockConstraintStream.ForEachCount;
                for (int chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
                {
                    var itemsLeft = lockConstraintStream.BeginForEachIndex(chunkIndex);
                    for (int i = 0; i < itemsLeft; i++)
                    {
                        var data                       = lockConstraintStream.Read<LockConstraintData>();
                        var entity                     = data.entity;
                        var bucketIndex                = data.bucketIndex;
                        var index                      = data.srcIndex;
                        var inertialPoseWorldTransform = data.inertialPoseWorldTransform;
                        var axes                       = data.lockFlags;
                        var positions                  = axes.packedFlags & 0x7;
                        if (positions != 0)
                        {
                            var bools = (positions & new int3(1, 2, 4)) != 0; // you told me to change the line here
                            ref var streamData = ref pairStream.AddPairAndGetRef<PositionConstraintData>(entity,
                                                                                                         bucketIndex,
                                                                                                         true,
                                                                                                         Entity.Null,
                                                                                                         bucketIndex,
                                                                                                         false,
                                                                                                         out var pair);
                            pair.userByte     = SolveByteCodes.positionConstraint;
                            streamData.indexA = index;
                            streamData.indexB = -1;
                            UnitySim.BuildJacobian(out streamData.parameters,
                                                   inertialPoseWorldTransform,
                                                   float3.zero,
                                                   RigidTransform.identity,
                                                   new RigidTransform(quaternion.identity, inertialPoseWorldTransform.pos),
                                                   0f,
                                                   0f,
                                                   lockTau,
                                                   lockDamping,
                                                   bools);
                        }
                        var rotations = axes.packedFlags & 0x38;
                        if (rotations != 0)
                        {
                            rotations           >>= 3;
                            var constraintCount   = math.countbits(rotations);
                            if (constraintCount == 3)
                            {
                                ref var streamData = ref pairStream.AddPairAndGetRef<Rotation3ConstraintData>(entity,
                                                                                                              bucketIndex,
                                                                                                              true,
                                                                                                              Entity.Null,
                                                                                                              bucketIndex,
                                                                                                              false,
                                                                                                              out var pair);
                                pair.userByte     = SolveByteCodes.rotationConstraint3;
                                streamData.indexA = index;
                                streamData.indexB = -1;
                                UnitySim.BuildJacobian(out streamData.parameters,
                                                       inertialPoseWorldTransform.rot,
                                                       quaternion.identity,
                                                       inertialPoseWorldTransform.rot,
                                                       quaternion.identity,
                                                       0f,
                                                       0f,
                                                       lockTau,
                                                       lockDamping);
                            }
                            else if (constraintCount == 2)
                            {
                                ref var streamData = ref pairStream.AddPairAndGetRef<Rotation2ConstraintData>(entity,
                                                                                                              bucketIndex,
                                                                                                              true,
                                                                                                              Entity.Null,
                                                                                                              bucketIndex,
                                                                                                              false,
                                                                                                              out var pair);
                                pair.userByte     = SolveByteCodes.rotationConstraint2;
                                streamData.indexA = index;
                                streamData.indexB = -1;
                                UnitySim.BuildJacobian(out streamData.parameters,
                                                       inertialPoseWorldTransform.rot,
                                                       quaternion.identity,
                                                       inertialPoseWorldTransform.rot,
                                                       quaternion.identity,
                                                       0f,
                                                       0f,
                                                       lockTau,
                                                       lockDamping,
                                                       math.tzcnt(~rotations));
                            }
                            else
                            {
                                ref var streamData = ref pairStream.AddPairAndGetRef<Rotation1ConstraintData>(entity,
                                                                                                              bucketIndex,
                                                                                                              true,
                                                                                                              Entity.Null,
                                                                                                              bucketIndex,
                                                                                                              false,
                                                                                                              out var pair);
                                pair.userByte     = SolveByteCodes.rotationConstraint1;
                                streamData.indexA = index;
                                streamData.indexB = -1;
                                UnitySim.BuildJacobian(out streamData.parameters,
                                                       inertialPoseWorldTransform.rot,
                                                       quaternion.identity,
                                                       inertialPoseWorldTransform.rot,
                                                       quaternion.identity,
                                                       0f,
                                                       0f,
                                                       lockTau,
                                                       lockDamping,
                                                       math.tzcnt(rotations));
                            }
                        }
                    }
                }
            }
        }
    }
}

