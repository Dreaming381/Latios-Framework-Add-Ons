using System;
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
    public partial struct SolveSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;
        EntityQuery          constraintWritersQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld            = state.GetLatiosWorldUnmanaged();
            constraintWritersQuery = state.Fluent().With<ConstraintWriter.ExistComponent>().IncludeSystemEntities().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsSettings = latiosWorld.GetPhysicsSettings();
            var states          = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<CapturedRigidBodies>(false).states;
            var kinematics      = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<CapturedKinematics>(true).kinematics;
            var jh              = state.Dependency;

            var constraintWriterEntities = constraintWritersQuery.ToEntityArray(Allocator.Temp);
            var pairStreams              = new NativeArray<ConstraintPairStream>(constraintWriterEntities.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            int validCount               = 0;
            for (int i = 0; i < constraintWriterEntities.Length; i++)
            {
                var entity = constraintWriterEntities[i];
                var writer = latiosWorld.GetCollectionComponent<ConstraintWriter>(entity, out JobHandle constraintJh);
                if (!writer.pairStream.isCreated)
                    continue;

                // Reset all PairStreams for the next update so that we don't get old instances leftover from systems that stopped updating.
                latiosWorld.SetCollectionComponentAndDisposeOld<ConstraintWriter>(entity, default);
                pairStreams[validCount] = new ConstraintPairStream
                {
                    priority   = writer.systemScheduleOrder,
                    jobHandle  = constraintJh,
                    pairStream = writer.pairStream
                };
                validCount++;
            }
            if (validCount == 0)
                return;

            pairStreams = pairStreams.GetSubArray(0, validCount);
            pairStreams.Sort();
            var aggregatedPairStream = pairStreams[0].pairStream;
            var pairStreamJh         = pairStreams[0].jobHandle;
            if (validCount > 1)
            {
                for (int i = 1; i < pairStreams.Length; i++)
                {
                    pairStreamJh = JobHandle.CombineDependencies(pairStreamJh, pairStreams[i].jobHandle);
                    pairStreamJh = new CombineStreamsJob { a = aggregatedPairStream, b = pairStreams[i].pairStream }.Schedule(pairStreamJh);
                }
            }
            jh = JobHandle.CombineDependencies(jh, pairStreamJh);

            int numIterations  = physicsSettings.numIterations;
            var solveProcessor = new SolveBodiesProcessor
            {
                states                 = states,
                kinematics             = kinematics,
                invNumSolverIterations = math.rcp(numIterations),
                deltaTime              = Time.DeltaTime,
                inverseDeltaTime       = math.rcp(Time.DeltaTime),
                firstIteration         = true,
                lastIteration          = false,
                icb                    = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<WorldTransform>().AsParallelWriter(),
            };
            var stabilizerJob = new StabilizeRigidBodiesJob
            {
                states         = states,
                firstIteration = true,
                dt             = Time.DeltaTime
            };
            for (int i = 0; i < numIterations; i++)
            {
                solveProcessor.lastIteration = i + 1 == numIterations;

                jh = Physics.ForEachPair(in aggregatedPairStream, in solveProcessor).ScheduleParallel(jh);
                jh = stabilizerJob.ScheduleParallel(states.Length, 128, jh);

                solveProcessor.firstIteration = false;
                stabilizerJob.firstIteration  = false;
            }
            state.Dependency = jh;
        }

        struct ConstraintPairStream : IComparable<ConstraintPairStream>
        {
            public uint       priority;
            public PairStream pairStream;
            public JobHandle  jobHandle;

            public int CompareTo(ConstraintPairStream other)
            {
                return priority.CompareTo(other.priority);
            }
        }

        [BurstCompile]
        struct CombineStreamsJob : IJob
        {
            public PairStream a;
            public PairStream b;

            public void Execute()
            {
                a.ConcatenateFrom(ref b);
            }
        }

        [BurstCompile]
        struct StabilizeRigidBodiesJob : IJobFor
        {
            [NativeDisableParallelForRestriction] public NativeArray<CapturedRigidBodyState> states;

            public float dt;
            public bool  firstIteration;

            public void Execute(int index)
            {
                ref var rigidBody = ref states.AsSpan()[index];
                UnitySim.UpdateStabilizationAfterSolverIteration(ref rigidBody.motionStabilizer,
                                                                 ref rigidBody.velocity,
                                                                 rigidBody.mass.inverseMass,
                                                                 rigidBody.angularExpansion,
                                                                 rigidBody.numOtherSignificantBodiesInContact,
                                                                 dt * rigidBody.gravity,
                                                                 math.normalize(rigidBody.gravity),
                                                                 UnitySim.kDefaultVelocityClippingFactor,
                                                                 UnitySim.kDefaultInertialScalingFactor,
                                                                 firstIteration);
            }
        }
    }
}

