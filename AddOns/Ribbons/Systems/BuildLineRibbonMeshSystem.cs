using Latios.Kinemation;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Latios.Ribbons.Systems
{
    /// <summary>
    /// Rebuilds the mesh of each line with RibbonLineConfig enabled, then disables it.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(Latios.Kinemation.Systems.KinemationPostRenderSuperSystem))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    public partial struct BuildLineRibbonMeshSystem : ISystem, ILatiosApi
    {
        EntityQuery m_query;
        EntityQuery m_viewAlignedQuery;
        float3      m_lastCameraPosition;
        float3      m_lastCameraForward;

        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);

            m_query = state.Fluent()
                      .With<UniqueMeshConfig, RenderBounds>(                       false)
                      .With<UniqueMeshVertexRawData, UniqueMeshIndex>(             false)
                      .With<RibbonPoint, RibbonWidthKeyframe>(                     true)
                      .With<RibbonColorKey>(                                       true)
                      .WithWorldTransformReadOnly()
                      .WithEnabled<RibbonLineConfig>(false)
                      .Build();

            // View-aligned lines need to rebuild when they or the camera move, even if nothing else changed.
            m_viewAlignedQuery = state.Fluent()
                                 .With<RibbonLineConfig>(    false)
                                 .With<RibbonViewAlignedTag>(true)
                                 .WithWorldTransformReadOnly()
                                 .Build();

            // Forces view-aligned lines to rebuild on the first update.
            m_lastCameraPosition = float.MinValue;
        }

        public void OnUpdate(ref SystemState state)
        {
            var api = this.GetApi(ref state);
            RibbonMeshBuilder.GetMainCamera(out var cameraPosition, out var cameraForward);

            state.Dependency = new EnableViewAlignedJob
            {
                cameraMoved       = math.any(cameraPosition != m_lastCameraPosition) || math.any(cameraForward != m_lastCameraForward),
                lastSystemVersion = state.LastSystemVersion
            }.Inject(api).Schedule(m_viewAlignedQuery, state.Dependency);
            m_lastCameraPosition = cameraPosition;
            m_lastCameraForward  = cameraForward;

            state.Dependency = new Job
            {
                cameraPosition   = cameraPosition,
                cameraForward    = cameraForward,
                linearColorSpace = RibbonMeshBuilder.IsLinearColorSpace()
            }.ScheduleParallel(api, m_query, state.Dependency);
            state.Dependency = new CleanupJob().Inject(api).Schedule(m_query, state.Dependency);
        }

        [BurstCompile]
        partial struct Job : IJobEach
        {
            public float3 cameraPosition;
            public float3 cameraForward;
            public bool   linearColorSpace;

            public void Execute(EnabledRefRW<UniqueMeshConfig>             meshDirty,
                                ref DynamicBuffer<UniqueMeshVertexRawData> outVertices,
                                ref DynamicBuffer<UniqueMeshIndex>         outIndices,
                                ref RenderBounds bounds,
                                in DynamicBuffer<RibbonPoint>              points,
                                in DynamicBuffer<RibbonWidthKeyframe>      widthKeyframes,
                                in DynamicBuffer<RibbonColorKey>           colorKeys,
                                in RibbonLineConfig config,
                                WorldTransformReadOnlyAspect worldTransform)
            {
                meshDirty.ValueRW = true;

                var output = new RibbonMeshBuilder.MeshOutput
                {
                    vertices = outVertices,
                    indices  = outIndices,
                };

                if (points.Length < 2)
                {
                    output.Clear();
                    return;
                }

                var facing = config.alignment == RibbonAlignment.View ?
                             RibbonMeshBuilder.Facing.View(in worldTransform, cameraPosition, cameraForward) :
                             RibbonMeshBuilder.Facing.TransformZ;
                RibbonMeshBuilder.Build(points.Reinterpret<float3>().AsNativeArray(),
                                        default,
                                        widthKeyframes,
                                        config.widthMultiplier,
                                        colorKeys,
                                        linearColorSpace,
                                        config.loop,
                                        config.textureMode,
                                        config.numCornerVertices,
                                        config.numCapVertices,
                                        in facing,
                                        ref output,
                                        out var min,
                                        out var max);
                bounds.SetMinMax(min, max);
            }
        }

        [BurstCompile]
        partial struct CleanupJob : IJobChunk, IInjectable
        {
            [Inject] ComponentTypeHandle<RibbonLineConfig> handle;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                chunk.SetComponentEnabledForAll(ref handle, false);
            }
        }

        [BurstCompile]
        partial struct EnableViewAlignedJob : IJobChunk, IInjectable
        {
            [Inject] ComponentTypeHandle<RibbonLineConfig>           handle;
            [Inject] WorldTransformReadOnlyAspect.TypeHandle         transformHandle;
            public bool                                              cameraMoved;
            public uint                                              lastSystemVersion;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                if (cameraMoved || transformHandle.DidChange(in chunk, lastSystemVersion))
                    chunk.SetComponentEnabledForAll(ref handle, true);
            }
        }
    }
}
