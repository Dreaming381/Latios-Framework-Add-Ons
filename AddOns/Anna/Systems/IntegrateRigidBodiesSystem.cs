using System;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
        EntityQuery          m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
            m_query     = state.Fluent().With<WorldTransform, RigidBody>(false).Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var captured        = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<CapturedRigidBodies>(true);
            var transformHandle = new TransformAspectParallelChunkHandle(SystemAPI.GetComponentLookup<WorldTransform>(false),
                                                                         SystemAPI.GetComponentTypeHandle<RootReference>(true),
                                                                         SystemAPI.GetBufferLookup<EntityInHierarchy>(true),
                                                                         SystemAPI.GetBufferLookup<EntityInHierarchyCleanup>(true),
                                                                         SystemAPI.GetEntityStorageInfoLookup(),
                                                                         ref state);
            state.Dependency = transformHandle.ScheduleChunkCaptureForQuery(m_query, state.Dependency);
            state.Dependency = transformHandle.ScheduleChunkGrouping(state.Dependency);
            state.Dependency = new IntegrateRigidBodiesInHierarchiesJob
            {
                entityToIndexMap = captured.entityToSrcIndexMap,
                states           = captured.states,
                deltaTime        = Time.DeltaTime,
                entityHandle     = GetEntityTypeHandle(),
                rigidBodyHandle  = GetComponentTypeHandle<RigidBody>(false),
                transformHandle  = transformHandle,
            }.ScheduleParallel(transformHandle, state.Dependency);
        }

        [BurstCompile]
        unsafe struct IntegrateRigidBodiesInHierarchiesJob : IJobParallelForDefer
        {
            public ComponentTypeHandle<RigidBody>                 rigidBodyHandle;
            public TransformAspectParallelChunkHandle             transformHandle;
            [ReadOnly] public EntityTypeHandle                    entityHandle;
            [ReadOnly] public NativeParallelHashMap<Entity, int>  entityToIndexMap;
            [ReadOnly] public NativeArray<CapturedRigidBodyState> states;
            public float                                          deltaTime;

            HasChecker<RootReference>              rootReferenceChecker;
            UnsafeList<BodyRef>                    bodyRefs;
            UnsafeList<TransformBatchWriteCommand> writeCommands;

            public void Execute(int index)
            {
                int chunkCount = transformHandle.GetChunkCountForIJobParallelForDeferIndex(index);
                if (chunkCount == 1)
                {
                    transformHandle.GetChunkInGroupForIJobParallelForDefer(index, 0, out var chunk, out _, out _, out _);
                    if (!rootReferenceChecker[chunk])
                    {
                        transformHandle.SetActiveChunkForIJobParallelForDefer(index, 0);
                        EvaluateRootsOnly(in chunk);
                        return;
                    }
                }
                EvaluateMixedHierarchies(index, chunkCount);
            }

            void EvaluateRootsOnly(in ArchetypeChunk chunk)
            {
                var entities    = chunk.GetEntityDataPtrRO(entityHandle);
                var rigidBodies = chunk.GetComponentDataPtrRW(ref rigidBodyHandle);
                for (int i = 0; i < chunk.Count; i++)
                {
                    var transform = transformHandle[i];
                    var wt        = transform.worldTransform;
                    Integrate(entities[i], ref wt, ref rigidBodies[i]);
                    transform.worldTransform = wt;
                }
            }

            // This part gets a little complicated, because we want to write the WorldTransforms in such a way that none
            // of the transforms are overwritten by an ancestor in the same the job.
            void EvaluateMixedHierarchies(int index, int chunkCount)
            {
                int neededBodies = 0;
                for (int i = 0; i < chunkCount; i++)
                {
                    transformHandle.GetChunkInGroupForIJobParallelForDefer(index, i, out var chunk, out _, out _, out _);
                    neededBodies += chunk.Count;
                }

                if (!bodyRefs.IsCreated)
                {
                    bodyRefs      = new UnsafeList<BodyRef>(neededBodies, Allocator.Temp);
                    writeCommands = new UnsafeList<TransformBatchWriteCommand>(32, Allocator.Temp);
                }
                else if (bodyRefs.Capacity < neededBodies)
                {
                    bodyRefs.Clear();
                    bodyRefs.Capacity = math.ceilpow2(neededBodies);
                }
                else
                    bodyRefs.Clear();

                for (int chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
                {
                    transformHandle.GetChunkInGroupForIJobParallelForDefer(index, chunkIndex, out var chunk, out _, out _, out _);
                    transformHandle.SetActiveChunkForIJobParallelForDefer(index, chunkIndex);
                    var entities    = chunk.GetEntityDataPtrRO(entityHandle);
                    var rigidBodies = chunk.GetComponentDataPtrRW(ref rigidBodyHandle);
                    for (int i = 0; i < chunk.Count; i++)
                    {
                        var transform = transformHandle[i];
                        var wt        = transform.worldTransform;
                        if (!Integrate(entities[i], ref wt, ref rigidBodies[i]))
                            continue;
                        var handle = transform.entityInHierarchyHandle;
                        bodyRefs.AddNoResize(new BodyRef
                        {
                            indexInHierarchy = handle.indexInHierarchy,
                            rootIndex        = handle.root.entity.Index,
                            transform        = transform,
                            transformToApply = wt
                        });
                    }
                }
                bodyRefs.Sort();

                int currentIndex = bodyRefs[0].rootIndex;
                int start        = 0;
                for (int i = 1; i < bodyRefs.Length + 1; i++)
                {
                    if (i == bodyRefs.Length || bodyRefs[i].rootIndex != currentIndex)
                    {
                        var count = i - start;
                        writeCommands.Clear();
                        for (int j = start; j < i; j++)
                        {
                            var body = bodyRefs[j];
                            writeCommands.Add(TransformBatchWriteCommand.SetWorldTransform(body.transform, in body.transformToApply));
                        }
                        var writeSpan = new Span<TransformBatchWriteCommand>(writeCommands.Ptr, writeCommands.Length);
                        writeSpan.ApplyTransforms();
                        start = i;
                        if (i < bodyRefs.Length)
                            currentIndex = bodyRefs[i].rootIndex;
                    }
                }
            }

            bool Integrate(Entity entity, ref TransformQvvs transform, ref RigidBody rigidBody)
            {
                if (!entityToIndexMap.TryGetValue(entity, out var index))
                    return false;
                var state                = states.AsReadOnlySpan()[index];
                var previousInertialPose = state.inertialPoseWorldTransform;
                if (!math.all(math.isfinite(state.velocity.linear)))
                    rigidBody.velocity.linear = float3.zero;
                if (!math.all(math.isfinite(state.velocity.angular)))
                    rigidBody.velocity.angular = float3.zero;
                UnitySim.Integrate(ref state.inertialPoseWorldTransform, ref state.velocity, state.linearDamping, state.angularDamping, deltaTime);
                transform = UnitySim.ApplyInertialPoseWorldTransformDeltaToWorldTransform(transform,
                                                                                          in previousInertialPose,
                                                                                          in state.inertialPoseWorldTransform);
                transform.rotation = math.normalize(transform.rotation);
                rigidBody.velocity = state.velocity;
                return true;
            }

            struct BodyRef : IComparable<BodyRef>
            {
                public int             rootIndex;
                public int             indexInHierarchy;
                public TransformAspect transform;
                public TransformQvvs   transformToApply;

                public int CompareTo(BodyRef other)
                {
                    int result = rootIndex.CompareTo(other.rootIndex);
                    if (result == 0)
                        result = indexInHierarchy.CompareTo(other.indexInHierarchy);
                    return result;
                }
            }
        }
    }
}

