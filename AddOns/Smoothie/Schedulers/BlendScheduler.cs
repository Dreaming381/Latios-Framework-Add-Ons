using System;
using Latios.Transforms;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    public struct BlendScheduler : IDisposable
    {
        CapturePointersJob   m_captureJob;
        GroupResultsJob      m_groupJob;
        BlendJob             m_blendJob;
        ApplyBlendResultsJob m_applyBlendResultsJob;

        public BlendScheduler(ref SystemState state,
                              in FixedList512Bytes<TypeIndex> inputComponentTypes,
                              in FixedList512Bytes<TypeIndex> outputComponentTypes,
                              bool enableAliasing = false)
        {
            var inputBuilder = new ComponentBrokerBuilder(Allocator.Temp).With<BlendInstructions, Duration>(true)
                               .With<ComponentBindingStart, ComponentBindingEnd, ConstantStartFloat, ConstantEndFloat>(true)
                               .With<Progression, IncompleteFlag, OutputFloat>(                                        false);
            foreach (var t in inputComponentTypes)
                inputBuilder = inputBuilder.With(ComponentType.ReadOnly(t));

            var outputBuilder = new ComponentBrokerBuilder(Allocator.Temp);
            foreach (var t in outputComponentTypes)
            {
                outputBuilder = outputBuilder.With(ComponentType.ReadWrite(t));
#if !LATIOS_TRANSFORMS_UNITY
                if (t == TypeManager.GetTypeIndex<LocalTransform>())
                {
                    outputBuilder.With(ComponentType.ReadOnly<ParentToWorldTransform>());
                }
#endif
            }

            m_captureJob = new CapturePointersJob
            {
                blendInstructionsHandle = state.GetSharedComponentTypeHandle<BlendInstructions>(),
                outputBindingHandle     = state.GetComponentTypeHandle<OutputBinding>(true),
                outputEntityHandle      = state.GetComponentTypeHandle<OutputEntity>(true),
                outputFloatHandle       = state.GetComponentTypeHandle<OutputFloat>(true),
            };
            m_groupJob = new GroupResultsJob
            {
                disableAliasingChecks = enableAliasing,
                esil                  = state.GetEntityStorageInfoLookup(),
            };
            m_blendJob = new BlendJob
            {
                broker = inputBuilder.Build(ref state, Allocator.Persistent)
            };
            m_applyBlendResultsJob = new ApplyBlendResultsJob
            {
                broker = outputBuilder.Build(ref state, Allocator.Persistent)
            };
        }

        public void Dispose()
        {
            m_blendJob.broker.Dispose();
            m_applyBlendResultsJob.broker.Dispose();
        }

        public JobHandle Schedule(ref SystemState state, EntityQuery query, float deltaTime, JobHandle inputDeps)
        {
            m_captureJob.blendInstructionsHandle.Update(ref state);
            m_captureJob.outputFloatHandle.Update(ref state);
            m_captureJob.outputEntityHandle.Update(ref state);
            m_captureJob.outputBindingHandle.Update(ref state);
            m_groupJob.esil.Update(ref state);
            m_blendJob.broker.Update(ref state);
            m_applyBlendResultsJob.broker.Update(ref state);

            var captures = CollectionHelper.CreateNativeArray<CapturedChunkData>(query.CalculateChunkCountWithoutFiltering(),
                                                                                 state.WorldUpdateAllocator,
                                                                                 NativeArrayOptions.ClearMemory);
            m_captureJob.captures = captures;
            var captureJh         = m_captureJob.ScheduleParallel(query, inputDeps);

            var outputChunkDataList            = new NativeList<OutputChunkData>(state.WorldUpdateAllocator);
            var outputComponentDataList        = new NativeList<OutputComponentData>(state.WorldUpdateAllocator);
            var outputValueDataList            = new NativeList<OutputValueData>(state.WorldUpdateAllocator);
            m_groupJob.captures                = captures;
            m_groupJob.outputChunkDataList     = outputChunkDataList;
            m_groupJob.outputComponentDataList = outputComponentDataList;
            m_groupJob.outputValueDataList     = outputValueDataList;
            var groupJh                        = m_groupJob.Schedule(captureJh);

            m_blendJob.deltaTime = deltaTime;
            var blendJh          = m_blendJob.ScheduleParallelByRef(query, captureJh);

            m_applyBlendResultsJob.outputChunkDataList     = outputChunkDataList.AsDeferredJobArray();
            m_applyBlendResultsJob.outputComponentDataList = outputComponentDataList.AsDeferredJobArray();
            m_applyBlendResultsJob.outputValueDataList     = outputValueDataList.AsDeferredJobArray();
            return m_applyBlendResultsJob.ScheduleByRef(outputChunkDataList, 1, JobHandle.CombineDependencies(groupJh, blendJh));
        }
    }

    public static class BlendFluentQueryExtensions
    {
        public static FluentQuery WithRequiredSmoothieBlendingComponents(this FluentQuery query)
        {
            return query.With<BlendInstructions>(true).WithEnabled<EnabledFlag>(true);
        }
    }
}

