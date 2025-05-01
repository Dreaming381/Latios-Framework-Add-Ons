using Latios.Psyshock;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct FindCollisionsSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;
        BlackboardEntity     systemBlackboard;
        EntityQueryMask      rigidBodyQueryMask;
        EntityQueryMask      kinematicQueryMask;
        EntityQueryMask      anyMask;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld      = state.GetLatiosWorldUnmanaged();
            systemBlackboard = new BlackboardEntity(state.SystemHandle, latiosWorld);
            systemBlackboard.AddOrSetCollectionComponentAndDisposeOld(new ConstraintWriter());

            var rigidBodyQuery = state.Fluent().With<RigidBody>(true).Without<KinematicCollisionTag>().Build();
            state.RequireForUpdate(rigidBodyQuery);
            rigidBodyQueryMask = rigidBodyQuery.GetEntityQueryMask();
            kinematicQueryMask = state.Fluent().With<KinematicCollisionTag>().Build().GetEntityQueryMask();
            anyMask            = state.Fluent().With<Collider>().Build().GetEntityQueryMask();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collisionWorld   = latiosWorld.sceneBlackboardEntity.GetCollectionAspect<ConstraintCollisionWorld>().collisionWorld;
            var infoLookup       = latiosWorld.sceneBlackboardEntity.GetCollectionAspect<ConstraintEntityInfoLookup>();
            var constraintWriter = new ConstraintWriter(ref state, latiosWorld);

            var exclusionSet = new NativeHashSet<uint>(64, state.WorldUpdateAllocator);
            var jhA          = new BuildExclusionSetJob
            {
                collisionWorld = collisionWorld,
                exclusionSet   = exclusionSet
            }.Schedule(state.Dependency);

            var archetypeClassifications = new NativeList<Classification>(state.WorldUpdateAllocator);
            var jhB                      = new BuildClassificationJob
            {
                collisionWorld           = collisionWorld,
                archetypeClassifications = archetypeClassifications,
                rigidBodyQueryMask       = rigidBodyQueryMask,
                kinematicQueryMask       = kinematicQueryMask
            }.Schedule(state.Dependency);
            var jh = JobHandle.CombineDependencies(jhA, jhB);

            var processor = new Processor
            {
                collisionWorld           = collisionWorld,
                lookup                   = infoLookup,
                exclusionSet             = exclusionSet,
                archetypeClassifications = archetypeClassifications.AsDeferredJobArray(),
                writer                   = constraintWriter.AsParallelWriter(),
            };
            state.Dependency = Physics.FindPairs(in collisionWorld, rigidBodyQueryMask, anyMask, in processor).ScheduleParallelUnsafe(jh);
            systemBlackboard.SetCollectionComponentAndDisposeOld(constraintWriter);
        }

        enum Classification : byte
        {
            Environment = 0,
            RigidBody,
            Kinematic,
        }

        [WithOptions(EntityQueryOptions.IncludeSystems)]
        [BurstCompile]
        partial struct BuildExclusionSetJob : IJobEntity
        {
            [ReadOnly] public CollisionWorld collisionWorld;
            public NativeHashSet<uint>       exclusionSet;

            public void Execute(in DynamicBuffer<CollisionExclusionPair> pairBuffer)
            {
                foreach (var pair in pairBuffer)
                {
                    // Unfortunately, EntityQueryMask doesn't support IEquatable, so we can't keep caches.
                    var maskA = collisionWorld.CreateMask(pair.queryA);
                    var maskB = collisionWorld.CreateMask(pair.queryB);

                    foreach (var indexA in maskA)
                    {
                        uint bottomFirst = (uint)indexA;
                        uint topFirst    = bottomFirst << 16;
                        foreach (var indexB in maskB)
                        {
                            var bottomSecond = (uint)indexB;
                            var topSecond    = bottomSecond << 16;
                            exclusionSet.Add(bottomFirst | topSecond);
                            exclusionSet.Add(topFirst | bottomSecond);
                        }
                    }
                }
            }
        }

        [BurstCompile]
        struct BuildClassificationJob : IJob
        {
            [ReadOnly] public CollisionWorld  collisionWorld;
            public NativeList<Classification> archetypeClassifications;
            public EntityQueryMask            rigidBodyQueryMask;
            public EntityQueryMask            kinematicQueryMask;

            public void Execute()
            {
                archetypeClassifications.Resize(collisionWorld.archetypeCount, NativeArrayOptions.ClearMemory);
                var kinematicMask = collisionWorld.CreateMask(kinematicQueryMask);
                foreach (var index in kinematicMask)
                    archetypeClassifications[index] = Classification.Kinematic;
                var rigidBodyMask                   = collisionWorld.CreateMask(rigidBodyQueryMask);
                foreach (var index in rigidBodyMask)
                    archetypeClassifications[index] = Classification.RigidBody;
            }
        }

        struct Processor : IFindPairsProcessor
        {
            [ReadOnly] public CollisionWorld              collisionWorld;
            [ReadOnly] public ConstraintEntityInfoLookup  lookup;
            [ReadOnly] public NativeHashSet<uint>         exclusionSet;
            [ReadOnly] public NativeArray<Classification> archetypeClassifications;

            public ConstraintWriter.ParallelWriter writer;

            DistanceBetweenAllCache distanceBetweenAllCache;

            public void Execute(in FindPairsResult result)
            {
                var archetypeIndexA = (uint)collisionWorld.archetypeIndices[result.bodyIndexA];
                var archetypeIndexB = (uint)collisionWorld.archetypeIndices[result.bodyIndexB];
                var key             = archetypeIndexA | (archetypeIndexB << 16);
                if (exclusionSet.Contains(key))
                    return;

                var classificationB = archetypeClassifications[(int)archetypeIndexB];
                switch (classificationB)
                {
                    case Classification.RigidBody:
                    {
                        // RigidBody vs RigidBody will be reported twice. Discard one of them.
                        if (result.bodyIndexB < result.bodyIndexA)
                            return;

                        lookup.TryGetRigidBodyHandle(result.entityA, out var handleA);
                        lookup.TryGetRigidBodyHandle(result.entityB, out var handleB);
                        var maxDistance = lookup.GetCollisionMaxDistanceBetween(in handleA, in handleB);
                        Physics.DistanceBetweenAll(result.colliderA, result.transformA, result.colliderB, result.transformB, maxDistance, ref distanceBetweenAllCache);
                        foreach (var distanceResult in distanceBetweenAllCache)
                        {
                            var contacts                 = UnitySim.ContactsBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, in distanceResult);
                            var coefficientOfFriction    = math.sqrt(lookup.GetCoefficientOfFriction(in handleA) * lookup.GetCoefficientOfFriction(in handleB));
                            var coefficientOfRestitution = math.sqrt(lookup.GetCoefficientOfRestitution(in handleA) * lookup.GetCoefficientOfRestitution(in handleB));
                            writer.SpeculateContactsBetween(ref lookup,
                                                            in result,
                                                            in handleA,
                                                            in handleB,
                                                            contacts.contactNormal,
                                                            contacts.AsSpan(),
                                                            coefficientOfRestitution,
                                                            coefficientOfFriction);
                        }
                        break;
                    }
                    case Classification.Kinematic:
                    {
                        lookup.TryGetRigidBodyHandle(result.entityA, out var handleA);
                        lookup.TryGetKinematicHandle(result.entityB, out var handleB);
                        var maxDistance = lookup.GetCollisionMaxDistanceBetween(in handleA, in handleB);
                        Physics.DistanceBetweenAll(result.colliderA, result.transformA, result.colliderB, result.transformB, maxDistance, ref distanceBetweenAllCache);
                        foreach (var distanceResult in distanceBetweenAllCache)
                        {
                            var contacts                 = UnitySim.ContactsBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, in distanceResult);
                            var coefficientOfFriction    = lookup.GetCoefficientOfFriction(in handleA);
                            var coefficientOfRestitution = lookup.GetCoefficientOfRestitution(in handleA);
                            writer.SpeculateContactsBetween(ref lookup,
                                                            in result,
                                                            in handleA,
                                                            in handleB,
                                                            contacts.contactNormal,
                                                            contacts.AsSpan(),
                                                            coefficientOfRestitution,
                                                            coefficientOfFriction);
                        }
                        break;
                    }
                    case Classification.Environment:
                    {
                        lookup.TryGetRigidBodyHandle(result.entityA, out var handleA);
                        var maxDistance = lookup.GetCollisionMaxDistanceBetween(in handleA);
                        Physics.DistanceBetweenAll(result.colliderA, result.transformA, result.colliderB, result.transformB, maxDistance, ref distanceBetweenAllCache);
                        foreach (var distanceResult in distanceBetweenAllCache)
                        {
                            var contacts = UnitySim.ContactsBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, in distanceResult);
                            //if (distanceResult.distance > 0.5f)
                            //{
                            //    var vel = lookup.rigidBodies.states[handleA.index].velocity;
                            //    UnityEngine.Debug.Log(
                            //        $"subcollider: {distanceResult.subColliderIndexB} distance: {distanceResult.distance}, normal: {contacts.contactNormal}, velocity: {vel.linear}, {vel.angular}");
                            //    foreach (var contact in contacts.AsSpan())
                            //    {
                            //        UnityEngine.Debug.Log($"contact: {contact.location}, {contact.distanceToA}");
                            //    }
                            //    var print = PhysicsDebug.LogDistanceBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, maxDistance);
                            //    //PhysicsDebug.LogWarning(print);
                            //}

                            var coefficientOfFriction    = lookup.GetCoefficientOfFriction(in handleA);
                            var coefficientOfRestitution = lookup.GetCoefficientOfRestitution(in handleA);
                            writer.SpeculateContactsBetween(ref lookup,
                                                            in result,
                                                            in handleA,
                                                            contacts.contactNormal,
                                                            contacts.AsSpan(),
                                                            coefficientOfRestitution,
                                                            coefficientOfFriction);
                        }
                        break;
                    }
                }
            }
        }
    }
}

