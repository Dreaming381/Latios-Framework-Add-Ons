using Latios.Kinemation;
using Latios.Kinemation.Systems;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Latios.Peekaboo.Systems
{
    /// <summary>
    /// Clears the per-camera culling mask for entities that are entirely behind other geometry.
    /// </summary>
    /// <remarks>
    /// This must never cull something visible, so every step errs in the same direction. Plates sit
    /// inside the geometry they stand for. A pixel only gets a depth when a plate covers all of it.
    /// And an entity only gets culled when the nearest corner of its bounds is behind every pixel its
    /// bounds touch. Each step can lose a cull, but never create one. Missing a cull is a number to
    /// improve, not a bug.
    /// </remarks>
    [DisableAutoCreation]
    [DontSyncPreviousUpdatesThisFrame(32)]
    [BurstCompile]
    public partial struct PeekabooOcclusionCullingSystem : ISystem, ILatiosApi
    {
        EntityQuery m_occluderQuery;
        EntityQuery m_occludeeMetaQuery;

        // The meta pass checks chunk archetypes against this, since a meta query can't see the
        // components inside a chunk. It's built from the occluder query, so the two always match.
        EntityQueryMask m_occluderMask;
        int         m_lastCullIndexThisFrame;

        // Kept between passes and always left all zero, so no pass has to clear it. See
        // BuildAndBinPlatesJob.
        NativeList<ulong> m_bandPlateBits;


        public void OnCreate(ref SystemState state)
        {
            var api = this.OnCreateForLatios(ref state);

            m_occluderQuery = state.Fluent()
                              .With<PeekabooOccluder>(         true)
                              .With<MaterialMeshInfo>(         true)
                              .With<WorldRenderBounds>(        true)
                              .With<ChunkWorldRenderBounds>(   true, true)
                              .With<ChunkPerCameraCullingMask>(true, true)
                              .WithWorldTransformReadOnly()
                              // Depth sorted means transparent, and a deforming or shader-displaced
                              // mesh doesn't match the geometry the plates were cut from.
                              .Without<DepthSorted_Tag>()
                              .Without<PeekabooDisableOccluderTag>()
                              .Without<SkeletonDependent, CopyDeformFromEntity>()
                              .Without<ShaderEffectRadialBounds, DynamicMeshState>()
                              .Without<BlendShapeState, UniqueMeshConfig>()
                              .Build();

            // Runs over meta chunks, where the chunk masks are regular components, so the pass reads
            // 128 at a time instead of visiting every chunk. The occludee opt-out tag can't be
            // expressed here, so it gets checked per chunk.
            m_occludeeMetaQuery = state.Fluent()
                                  .With<ChunkHeader>(             true)
                                  .With<ChunkWorldRenderBounds>(  true)
                                  .With<ChunkPerCameraCullingMask>(true)
                                  .Build();

            m_occluderMask = m_occluderQuery.GetEntityQueryMask();

            m_bandPlateBits = new NativeList<ulong>(Allocator.Persistent);
            PeekabooPassProfiler.Initialize();

            api.worldBlackboardEntity.AddComponentDataIfMissing(PeekabooSettings.Default);
            api.worldBlackboardEntity.AddComponentDataIfMissing(default(PeekabooStats));
        }

        [BurstCompile]
        public unsafe void OnUpdate(ref SystemState state)
        {
            var api      = this.GetApi(ref state);
            var settings = api.worldBlackboardEntity.GetComponentData<PeekabooSettings>();
            settings.maxOccludersPerView = math.max(1, settings.maxOccludersPerView);
            if (!settings.debugOverrideAllowWithoutBurst && !IsBurstEnabled())
                return;

            var  context = api.worldBlackboardEntity.GetComponentData<CullingContext>();
            bool wanted  = context.viewType switch
            {
                BatchCullingViewType.Camera => settings.cullCameras,
                BatchCullingViewType.Light => settings.cullLights,
                _ => false
            };
            if (!wanted)
                return;

            var allocator = state.WorldUpdateAllocator;
            var views     = new NativeList<PeekabooView>(8, allocator);
            {
                var cullingPlanes = api.worldBlackboardEntity.GetBuffer<CullingPlane>(true).Reinterpret<Plane>().AsNativeArray();
                var cullingSplits = api.worldBlackboardEntity.GetBuffer<CullingSplitElement>(true).Reinterpret<CullingSplit>().AsNativeArray();
                PeekabooViewBuilder.BuildViews(in context, cullingPlanes, cullingSplits, in settings, ref views);
            }
            if (views.IsEmpty)
                return;

            int totalTexels    = 0;
            int totalRows      = 0;
            var viewRowOffsets = CollectionHelper.CreateNativeArray<int>(views.Length, allocator, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < views.Length; i++)
            {
                viewRowOffsets[i]  = totalRows;
                totalTexels       += PeekabooDepthBuffer.TotalTexels(views[i].resolution);
                totalRows         += views[i].resolution.y;
            }

            // Each worker keeps its own best candidates per view instead of racing for slots in one
            // array. That way nothing gets dropped for arriving late, and the chosen occluders don't
            // depend on how chunks were split up. Only the counts need clearing.
            int threadCount      = JobsUtility.ThreadIndexCount;
            int heapStride       = views.Length * settings.maxOccludersPerView;
            var threadHeaps      = CollectionHelper.CreateNativeArray<OccluderCandidate>(threadCount * heapStride, allocator, NativeArrayOptions.UninitializedMemory);
            var threadHeapCounts = CollectionHelper.CreateNativeArray<int>(threadCount * views.Length, allocator, NativeArrayOptions.ClearMemory);
            var statsCounters    = PeekabooStatsCounters.Create(threadCount, allocator);
            var sharedFloorBits  = CollectionHelper.CreateNativeArray<int>(views.Length, allocator, NativeArrayOptions.ClearMemory);
            var depth             = CollectionHelper.CreateNativeArray<float>(totalTexels + PeekabooDepthBuffer.kOverreadPadding, allocator, NativeArrayOptions.UninitializedMemory);
            var depthMax          = CollectionHelper.CreateNativeArray<float>(totalTexels + PeekabooDepthBuffer.kOverreadPadding, allocator, NativeArrayOptions.UninitializedMemory);


            var viewArray   = views.AsArray();
            long passStart  = PeekabooPassProfiler.BeginPass();

            // Which chunks the cull job touches doesn't depend on anything rasterized, so finding them
            // runs alongside the rest of the pass instead of waiting for the pyramid.
            // This isn't a list with a parallel writer, since the find job writes two things per chunk
            // that need to share an index. It claims a range of both at once, and the count it hands
            // out is also the cull job's deferred length.
            int chunkCeiling   = m_occludeeMetaQuery.CalculateEntityCountWithoutFiltering();
            var chunksToCull   = CollectionHelper.CreateNativeArray<OccludeeChunk>(chunkCeiling, allocator, NativeArrayOptions.UninitializedMemory);
            var chunkProjected = CollectionHelper.CreateNativeArray<ChunkProjection>(chunkCeiling * views.Length, allocator, NativeArrayOptions.UninitializedMemory);
            var chunkCount     = CollectionHelper.CreateNativeArray<int>(1, allocator, NativeArrayOptions.ClearMemory);

            // How many chunks a worker claims at a time. Claiming one at a time costs an atomic per
            // chunk, and for a fully hidden chunk that's most of the cost. Claiming too many leaves
            // workers idle at the end, and chunk costs vary enough for that to matter. 64 measured no
            // better than 1. This aims for a few dozen batches per worker, using the count before
            // frustum culling, since the count after isn't known until the job runs.
            int cullBatchCount = math.clamp(chunkCeiling / (threadCount * 32), 1, 16);

            // Wait out whatever the culling loop still has running, so the first stage's time only
            // counts this system's work.
            var upstream = state.Dependency;
            PeekabooPassProfiler.Sample(PeekabooPassProfiler.kUpstream, ref upstream);


            // Most renderers in a big scene are occluders, but most chunks are already culled by the
            // time this runs. Visiting every chunk just to read its mask and move on was 70% of the
            // gather's cost. So the masks get read from meta chunks, 128 at a time, and the occluder
            // query is passed as a mask to check archetypes against.
            var occluderChunks = new NativeList<OccluderChunk>(chunkCeiling, allocator);
            var findOccluders  = new FindOccluderChunksJob
            {
                occluders       = m_occluderMask,
                chunksToProcess = occluderChunks.AsParallelWriter(),
            }.Inject(api).ScheduleParallel(m_occludeeMetaQuery, state.Dependency);

            var gatherHandle = new GatherOccluderCandidatesJob
            {
                chunksToProcess               = occluderChunks.AsDeferredJobArray(),
                views                         = viewArray,
                threadHeaps                   = threadHeaps,
                threadHeapCounts              = threadHeapCounts,
                stats                         = statsCounters,
                sharedFloorBits               = sharedFloorBits,
                maxOccludersPerView           = settings.maxOccludersPerView,
                minOccluderCoverage           = context.viewType == BatchCullingViewType.Light ? settings.minLightOccluderCoverage : settings.minCameraOccluderCoverage,
                isLightView                   = context.viewType == BatchCullingViewType.Light,
                renderMeshArrayHandle         = Unity.Entities.SystemAPI.ManagedAPI.GetSharedComponentTypeHandle<RenderMeshArray>(),
                drawContext                   = api.worldBlackboardEntity.GetCollectionAspect<OcclusionCullingContextAspect>(),
            }.Inject(api).Schedule(occluderChunks, 1, findOccluders);
            PeekabooPassProfiler.Sample(PeekabooPassProfiler.kGather, ref gatherHandle);

            var selected       = new NativeList<OccluderCandidate>(allocator);
            var viewRanges     = CollectionHelper.CreateNativeArray<int2>(views.Length, allocator, NativeArrayOptions.ClearMemory);
            var plates         = new NativeList<ScreenPlate>(allocator);
            var plateVerts     = new NativeList<float2>(allocator);
            var crossings      = new NativeList<float2>(allocator);
            var crossingCursor = CollectionHelper.CreateNativeArray<int>(1, allocator, NativeArrayOptions.ClearMemory);

            // One bit per band of rows and plate. Select sizes it once it knows the plate count, so
            // there's no per band capacity to guess.
            var bandPlateBits    = m_bandPlateBits;
            var wordsPerBand     = CollectionHelper.CreateNativeArray<int>(1, allocator, NativeArrayOptions.ClearMemory);
            var viewPlateRanges = CollectionHelper.CreateNativeArray<int2>(views.Length, allocator, NativeArrayOptions.ClearMemory);

            var selectHandle = new SelectOccludersJob
            {
                threadHeaps         = threadHeaps,
                threadHeapCounts    = threadHeapCounts,

                threadCount         = threadCount,
                maxOccludersPerView = settings.maxOccludersPerView,
                viewCount           = views.Length,
                totalRows           = totalRows,
                selected            = selected,
                viewRanges          = viewRanges,
                plates              = plates,
                plateVertices       = plateVerts,
                plateCrossings      = crossings,
                wordsPerBand         = wordsPerBand,
                viewPlateRanges     = viewPlateRanges,
                bandPlateBits        = bandPlateBits,
            }.Schedule(gatherHandle);
            PeekabooPassProfiler.Sample(PeekabooPassProfiler.kSelect, ref selectHandle);

            // Runs after the gather, not alongside it, because this writes the two masks and the
            // gather reads them. Scheduled after Select, since the job system hands out work in order,
            // so this is what the other workers pick up while Select runs alone. The cull job waits on
            // the pyramids, which are two stages further on, so this is long done by then.
            var findHandle = new FindChunksToCullJob
            {
                views       = viewArray,
                entries     = chunksToCull,
                projections = chunkProjected,
                entryCount  = (int*)chunkCount.GetUnsafePtr(),
            }.Inject(api).ScheduleParallel(m_occludeeMetaQuery, gatherHandle);

            var buildHandle = new BuildAndBinPlatesJob
            {
                views          = viewArray,
                selected       = selected.AsDeferredJobArray(),
                wordsPerBand   = wordsPerBand,
                viewRowOffsets = viewRowOffsets,
                plates         = plates.AsDeferredJobArray(),
                plateVertices  = plateVerts.AsDeferredJobArray(),
                bandPlateBits  = bandPlateBits.AsDeferredJobArray(),
                plateCrossings = crossings.AsDeferredJobArray(),
                crossingCursor = crossingCursor,
            }.Schedule(selected, 4, selectHandle);
            PeekabooPassProfiler.Sample(PeekabooPassProfiler.kBuildPlates, ref buildHandle);

            int rasterItems   = RasterizeAndReduceJob.ItemCount(totalRows);
            var pairsDone     = CollectionHelper.CreateNativeArray<int>(views.Length, allocator, NativeArrayOptions.ClearMemory);
            var bandPairsDone = CollectionHelper.CreateNativeArray<int>(totalRows / PeekabooRasterizer.kRowsPerBand, allocator, NativeArrayOptions.ClearMemory);
            var pyramidHandle = new RasterizeAndReduceJob
            {
                views            = viewArray,
                viewRowOffsets   = viewRowOffsets,
                plates           = plates.AsDeferredJobArray(),
                plateVertices    = plateVerts.AsDeferredJobArray(),
                plateCrossings   = crossings.AsDeferredJobArray(),
                bandPlateBits    = bandPlateBits.AsDeferredJobArray(),
                wordsPerBand     = wordsPerBand,
                viewPlateRanges  = viewPlateRanges,
                depth            = depth,
                depthMax         = depthMax,
                pairsDonePerView = pairsDone,
                pairsDonePerBand = bandPairsDone,
                itemCount        = rasterItems,
                bandStride       = RasterizeAndReduceJob.ChooseStride(rasterItems),
            }.Schedule(rasterItems, 1, buildHandle);
            PeekabooPassProfiler.Sample(PeekabooPassProfiler.kRasterize, ref pyramidHandle);

            var cullHandle = new CullOccludeesJob
            {
                views              = viewArray,
                depth              = depth,
                depthMax           = depthMax,
                isLightView        = context.viewType == BatchCullingViewType.Light,
                stats              = statsCounters,
                chunksToProcess    = chunksToCull,
                projections        = chunkProjected,
            }.Inject(api).Schedule((int*)chunkCount.GetUnsafePtr(), cullBatchCount, JobHandle.CombineDependencies(pyramidHandle, findHandle));
            PeekabooPassProfiler.Sample(PeekabooPassProfiler.kCull, ref cullHandle);

            // Stats cover the whole frame, which has several passes. The pass index resets to zero each
            // frame. If it goes down, that also means a new frame started, which catches the case
            // where an earlier pass bailed out before resetting the stats.
            bool isFirstPassOfFrame  = context.cullIndexThisFrame == 0 || context.cullIndexThisFrame <= m_lastCullIndexThisFrame;
            m_lastCullIndexThisFrame = context.cullIndexThisFrame;

            var finalHandle = cullHandle;
            if (PeekabooStatsCounters.isEnabled)
            {
                finalHandle = new AccumulateStatsJob
                {
                    views              = viewArray,
                    selected           = selected.AsDeferredJobArray(),
                    plates             = plates.AsDeferredJobArray(),
                    threadHeapCounts   = threadHeapCounts,
                    counters           = statsCounters,
                    statsLookup        = state.GetComponentLookup<PeekabooStats>(false),
                    blackboardEntity   = api.worldBlackboardEntity,
                    isFirstPassOfFrame = isFirstPassOfFrame,
                    isLightView        = context.viewType == BatchCullingViewType.Light,
                }.Schedule(cullHandle);
                PeekabooPassProfiler.Sample(PeekabooPassProfiler.kStats, ref finalHandle);
            }
            PeekabooPassProfiler.EndPass(context.viewType == BatchCullingViewType.Light, ref finalHandle, passStart);

            state.Dependency = finalHandle;
        }

        public void OnDestroy(ref SystemState state)
        {
            state.CompleteDependency();
            if (m_bandPlateBits.IsCreated)
                m_bandPlateBits.Dispose();
        }

        static bool IsBurstEnabled()
        {
            bool burst = true;
            DisableIfManaged(ref burst);
            return burst;
        }

        // Software rasterization without Burst costs far more than it saves, so the system turns
        // itself off instead.
        [BurstDiscard]
        static void DisableIfManaged(ref bool burst) => burst = false;
    }
}

