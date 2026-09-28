using Latios.Kinemation;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Latios.Ribbons.Systems
{
    /// <summary>
    /// Emits and expires trail points, then rebuilds the mesh of every trail.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(Latios.Kinemation.Systems.KinemationPostRenderSuperSystem))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    public partial struct RibbonTrailSystem : ISystem, ILatiosApi
    {
        EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);

            m_query = state.Fluent()
                      .With<UniqueMeshConfig, RenderBounds>(                       false)
                      .With<UniqueMeshVertexRawData, UniqueMeshIndex>(             false)
                      .With<RibbonTrailPoint, RibbonTrailEmitterState>(            false)
                      .With<RibbonWidthKeyframe, RibbonColorKey>(                   true)
                      .With<RibbonTrailConfig>(                                    true)
                      .WithWorldTransformReadOnly()
                      .Build();
        }

        public void OnUpdate(ref SystemState state)
        {
            var    api            = this.GetApi(ref state);
            RibbonMeshBuilder.GetMainCamera(out var cameraPosition, out var cameraForward);

            state.Dependency = new Job
            {
                currentTime      = (float)state.WorldUnmanaged.Time.ElapsedTime,
                cameraPosition   = cameraPosition,
                cameraForward    = cameraForward,
                linearColorSpace = RibbonMeshBuilder.IsLinearColorSpace()
            }.ScheduleParallel(api, m_query, state.Dependency);
        }

        [BurstCompile]
        partial struct Job : IJobEach
        {
            public float  currentTime;
            public float3 cameraPosition;
            public float3 cameraForward;
            public bool   linearColorSpace;

            public void Execute(EnabledRefRW<UniqueMeshConfig>             meshDirty,
                                ref DynamicBuffer<UniqueMeshVertexRawData> outVertices,
                                ref DynamicBuffer<UniqueMeshIndex>         outIndices,
                                ref RenderBounds bounds,
                                ref DynamicBuffer<RibbonTrailPoint>        trailPoints,
                                ref RibbonTrailEmitterState emitterState,
                                in DynamicBuffer<RibbonWidthKeyframe>      widthKeyframes,
                                in DynamicBuffer<RibbonColorKey>           colorKeys,
                                in RibbonTrailConfig config,
                                WorldTransformReadOnlyAspect worldTransform)
            {
                var headPos = worldTransform.position;

                // Like TrailRenderer, points keep being added while not emitting. They just have zero width.
                if (!emitterState.hasEmitted || math.distance(headPos, emitterState.lastEmitWorldPosition) >= config.minVertexDistance)
                {
                    trailPoints.Add(new RibbonTrailPoint { worldPosition = headPos, creationTime = currentTime, isGap = !config.emitting });
                    emitterState.lastEmitWorldPosition = headPos;
                    emitterState.hasEmitted            = true;
                }

                float minCreationTime = currentTime - config.time;
                int   removeCount     = 0;
                while (removeCount < trailPoints.Length && trailPoints[removeCount].creationTime < minCreationTime)
                    removeCount++;
                if (removeCount > 0)
                    trailPoints.RemoveRange(0, removeCount);

                meshDirty.ValueRW = true;
                var output        = new RibbonMeshBuilder.MeshOutput
                {
                    vertices = outVertices,
                    indices  = outIndices,
                };

                // The head is a live point at the entity's position, unless the newest point is already there.
                int  storedCount = trailPoints.Length;
                bool hasHead     = storedCount > 0 && math.distancesq(headPos, trailPoints[storedCount - 1].worldPosition) > 1e-10f;
                int  n           = storedCount + (hasHead ? 1 : 0);
                if (n < 2)
                {
                    output.Clear();
                    return;
                }

                // The mesh is built from the head to the oldest point, so that the curves and U start at the head.
                var              positions   = new NativeArray<float3>(n, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var              widthScales = new NativeArray<float>(n, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                bool             anyVisible  = false;
                int              dst         = 0;
                if (hasHead)
                {
                    positions[0]   = worldTransform.InverseTransformPoint(headPos);
                    widthScales[0] = config.emitting ? 1f : 0f;
                    anyVisible     = config.emitting;
                    dst            = 1;
                }
                for (int src = storedCount - 1; src >= 0; src--, dst++)
                {
                    var point         = trailPoints[src];
                    positions[dst]    = worldTransform.InverseTransformPoint(point.worldPosition);
                    widthScales[dst]  = point.isGap ? 0f : 1f;
                    anyVisible       |= !point.isGap;
                }
                if (!anyVisible)
                {
                    output.Clear();
                    return;
                }

                var facing = config.alignment == RibbonAlignment.View ?
                             RibbonMeshBuilder.Facing.View(in worldTransform, cameraPosition, cameraForward) :
                             RibbonMeshBuilder.Facing.TransformZ;
                RibbonMeshBuilder.Build(positions,
                                        widthScales,
                                        widthKeyframes,
                                        config.widthMultiplier,
                                        colorKeys,
                                        linearColorSpace,
                                        false,
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
    }
}

