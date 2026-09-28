using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace Latios.Ribbons.Systems
{
    /// <summary>
    /// Enables RibbonLineConfig on lines that live baking changed, so that they rebuild.
    /// </summary>
    [UpdateInGroup(typeof(Latios.Systems.AfterLiveBakingSuperSystem))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct LiveBakingEnableChangedRibbonLineSystem : ISystem, ILatiosApi
    {
        EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);
            m_query = state.Fluent()
                      .With<LiveBakedTag>(                     true)
                      .With<RibbonLineConfig>(                 false)
                      .With<RibbonPoint, RibbonWidthKeyframe>(true)
                      .With<RibbonColorKey>(                  true)
                      .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var api          = this.GetApi(ref state);
            var lastVersion  = api.worldBlackboardEntity.GetComponentData<SystemVersionBeforeLiveBake>().version;
            state.Dependency = new Job
            {
                pointHandle       = api.GetBufferHandle<RibbonPoint>(true),
                widthHandle       = api.GetBufferHandle<RibbonWidthKeyframe>(true),
                colorHandle       = api.GetBufferHandle<RibbonColorKey>(true),
                configHandle      = api.GetComponentHandle<RibbonLineConfig>(false),
                lastSystemVersion = lastVersion,
            }.ScheduleParallel(m_query, state.Dependency);
        }

        [BurstCompile]
        partial struct Job : IJobChunk
        {
            [ReadOnly] public BufferTypeHandle<RibbonPoint>         pointHandle;
            [ReadOnly] public BufferTypeHandle<RibbonWidthKeyframe> widthHandle;
            [ReadOnly] public BufferTypeHandle<RibbonColorKey>      colorHandle;
            public ComponentTypeHandle<RibbonLineConfig>            configHandle;
            public uint                                             lastSystemVersion;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                bool anythingChanged = chunk.DidOrderChange(lastSystemVersion) ||
                                       chunk.DidChange(ref pointHandle, lastSystemVersion) ||
                                       chunk.DidChange(ref widthHandle, lastSystemVersion) ||
                                       chunk.DidChange(ref colorHandle, lastSystemVersion) ||
                                       chunk.DidChange(ref configHandle, lastSystemVersion);
                if (anythingChanged)
                    chunk.SetComponentEnabledForAll(ref configHandle, true);
            }
        }
    }
}

