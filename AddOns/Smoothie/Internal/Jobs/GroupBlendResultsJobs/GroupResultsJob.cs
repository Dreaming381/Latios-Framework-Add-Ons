using System;
using System.Diagnostics;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Exposed;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    [BurstCompile]
    internal struct GroupResultsJob : IJob
    {
        [ReadOnly] public NativeArray<CapturedChunkData> captures;
        [ReadOnly] public EntityStorageInfoLookup        esil;
        public NativeList<OutputChunkData>               outputChunkDataList;
        public NativeList<OutputComponentData>           outputComponentDataList;
        public NativeList<OutputValueData>               outputValueDataList;

        public bool disableAliasingChecks;

        public unsafe void Execute()
        {
            int total = 0;
            foreach (var capture in captures)
                total += capture.enabledCount;

            if (total == 0)
                return;

#if !LATIOS_TRANSFORMS_UNITY
            var localTransformTypeIndex = TypeManager.GetTypeIndex<LocalTransform>();
            var worldTransformTypeIndex = TypeManager.GetTypeIndex<WorldTransform>();
#endif

            // Step 1: Gather the unique output data
            var sortableOutputs = new NativeArray<SortableOutput>(total, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var sortableIndex   = 0;
            foreach (var capture in captures)
            {
                if (capture.enabledCount == 0)
                    continue;

                var enumerator = new ChunkEntityEnumerator(capture.useEnabledMask, capture.enabledMask, capture.countInChunk);
                while (enumerator.NextEntityIndex(out var i))
                {
                    var esi     = esil[capture.entityPtr[i].entity];
                    var binding = capture.bindingPtr[i].binding;

#if !LATIOS_TRANSFORMS_UNITY
                    if (binding.typeIndex == localTransformTypeIndex)
                    {
                        // We want to use TransformAspect on transform bindings, which means we want to combine local and world bindings.
                        // We do this by changing the typeIndex to WorldTransform and adding 48 bytes to the offset.
                        binding.typeIndex  = worldTransformTypeIndex;
                        binding.offset    += 48;
                    }
#endif

                    sortableOutputs[sortableIndex] = new SortableOutput
                    {
                        chunk         = esi.Chunk,
                        typeIndex     = binding.typeIndex,
                        entityIndex   = esi.IndexInChunk,
                        bindingOffset = binding.offset,
                        ptr           = capture.outputPtr,
                        byteCount     = capture.outputByteCountPerBlend
                    };
                }
            }

            // Step 2: Sort and validate
            sortableOutputs.Sort();
            if (!disableAliasingChecks)
            {
                CheckAliasing(sortableOutputs);
            }

            // Step 3: Count the required list sizes and allocate
            int            outputChunkCount     = 0;
            int            outputComponentCount = 0;
            ArchetypeChunk previousChunk        = default;
            TypeIndex      previousTypeIndex    = default;
            foreach (var sortable in sortableOutputs)
            {
                if (previousChunk != sortable.chunk)
                {
                    outputChunkCount++;
                    outputComponentCount++;
                    previousChunk     = sortable.chunk;
                    previousTypeIndex = sortable.typeIndex;
                }
                else if (previousTypeIndex != sortable.typeIndex)
                {
                    outputComponentCount++;
                    previousTypeIndex = sortable.typeIndex;
                }
            }

            outputChunkDataList.ResizeUninitialized(outputChunkCount);
            outputComponentDataList.ResizeUninitialized(outputComponentCount);
            outputValueDataList.ResizeUninitialized(sortableOutputs.Length);
            var chunkSpan     = outputChunkDataList.AsArray().AsSpan();
            var componentSpan = outputComponentDataList.AsArray().AsSpan();
            var valueSpan     = outputValueDataList.AsArray().AsSpan();

            // Step 4: Populate output lists
            int currentChunkIndex     = -1;
            int currentComponentIndex = -1;
            int currentValueIndex     = 0;
            previousChunk             = default;
            previousTypeIndex         = default;
            foreach (var sortable in sortableOutputs)
            {
                if (sortable.chunk != previousChunk)
                {
                    currentChunkIndex++;
                    currentComponentIndex++;
                    chunkSpan[currentChunkIndex] = new OutputChunkData
                    {
                        targetChunk         = sortable.chunk,
                        componentStartIndex = currentComponentIndex,
                        componentCount      = 1
                    };
                    componentSpan[currentComponentIndex] = new OutputComponentData
                    {
                        typeIndex             = sortable.typeIndex,
                        outputValueStartIndex = currentValueIndex,
                        outputValueCount      = 0
                    };
                    previousChunk     = sortable.chunk;
                    previousTypeIndex = sortable.typeIndex;
                }
                else if (sortable.typeIndex != previousTypeIndex)
                {
                    chunkSpan[currentChunkIndex].componentCount++;
                    currentComponentIndex++;
                    componentSpan[currentComponentIndex] = new OutputComponentData
                    {
                        typeIndex             = sortable.typeIndex,
                        outputValueStartIndex = currentValueIndex,
                        outputValueCount      = 0
                    };
                    previousTypeIndex = sortable.typeIndex;
                }
                valueSpan[currentValueIndex] = new OutputValueData
                {
                    entityIndex               = (byte)sortable.entityIndex,
                    componentBindingOffset    = sortable.bindingOffset,
                    outputPtr                 = sortable.ptr,
                    componentBindingByteCount = (short)sortable.byteCount,
                };
                componentSpan[currentComponentIndex].outputValueCount++;
                currentValueIndex++;
            }
        }

        unsafe struct SortableOutput : IComparable<SortableOutput>
        {
            public ArchetypeChunk chunk;
            public void*          ptr;
            public int            byteCount;
            public TypeIndex      typeIndex;
            public int            entityIndex;
            public int            bindingOffset;

            public int CompareTo(SortableOutput other)
            {
                var result = chunk.GetChunkIndexAsUint().CompareTo(other.chunk.GetChunkIndexAsUint());
                if (result == 0)
                {
                    result = typeIndex.CompareTo(other.typeIndex);
                    if (result == 0)
                    {
                        result = entityIndex.CompareTo(other.entityIndex);
                        if (result == 0)
                            result = bindingOffset.CompareTo(other.bindingOffset);
                    }
                }
                return result;
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        unsafe void CheckAliasing(NativeArray<SortableOutput> array)
        {
            BitField64 transformMask = default;

#if !LATIOS_TRANSFORMS_UNITY
            var worldTransformTypeIndex = TypeManager.GetTypeIndex<WorldTransform>();
#endif

            for (int i = 1; i < array.Length; i++)
            {
                var previous = array[i - 1];
                var current  = array[i];
                if (previous.chunk != current.chunk || previous.typeIndex != current.typeIndex || previous.entityIndex != current.entityIndex)
                {
                    transformMask = default;
                    continue;
                }

#if !LATIOS_TRANSFORMS_UNITY
                if (current.typeIndex == worldTransformTypeIndex)
                {
                    if (current.bindingOffset >= 48)
                    {
                        // The local transform binding may alias the global transform binding.
                        var localTransformStart = current.bindingOffset - 48;
                        if (localTransformStart < 28)
                        {
                            var positionRotationClampedByteCount = math.min(current.byteCount, 28 - localTransformStart);
                            if (transformMask.TestAny(localTransformStart, positionRotationClampedByteCount))
                            {
                                var entity = current.chunk.GetEntityDataPtrRO(esil.AsEntityTypeHandle())[current.entityIndex];
                                throw new InvalidOperationException(
                                    $"The LocalTransform blend targeting {current.byteCount} bytes from offset {localTransformStart} targeting entity {entity.ToFixedString()} aliases with another blend targeting similar fields of the WorldTransform.");
                            }
                        }
                        if (localTransformStart + current.byteCount > 28)
                        {
                            var scaleStart = math.max(44, localTransformStart + 16);
                            var byteCount  = current.byteCount - (math.max(0, 28 - localTransformStart));
                            if (transformMask.TestAny(scaleStart, byteCount))
                            {
                                var entity = current.chunk.GetEntityDataPtrRO(esil.AsEntityTypeHandle())[current.entityIndex];
                                throw new InvalidOperationException(
                                    $"The LocalTransform blend targeting {current.byteCount} bytes from offset {localTransformStart} targeting entity {entity.ToFixedString()} aliases with another blend targeting similar fields of the WorldTransform.");
                            }
                        }

                        // For better diagnostics, we also do the LocalTransform aliasing check here to compensate for the fudged offsets
                        if (previous.bindingOffset - 48 + previous.byteCount > current.bindingOffset)
                        {
                            var entity = current.chunk.GetEntityDataPtrRO(esil.AsEntityTypeHandle())[current.entityIndex];
                            throw new InvalidOperationException(
                                $"The LocalTransform blend targeting {current.byteCount} bytes from offset {localTransformStart} targeting entity {entity.ToFixedString()} aliases with another blend targeting {previous.byteCount} bytes from offset {previous.bindingOffset - 48}.");
                        }
                    }
                    else
                    {
                        // Check here before we add to the transformMask
                        if (previous.bindingOffset + previous.byteCount > current.bindingOffset)
                        {
                            var entity = current.chunk.GetEntityDataPtrRO(esil.AsEntityTypeHandle())[current.entityIndex];
                            throw new InvalidOperationException(
                                $"The WorldTransform blend targeting {current.byteCount} bytes from offset {current.bindingOffset} targeting entity {entity.ToFixedString()} aliases with another blend targeting {previous.byteCount} bytes from offset {previous.bindingOffset}.");
                        }
                        transformMask.SetBits(current.bindingOffset, true, current.byteCount);
                    }
                    continue;
                }
#endif
                if (previous.bindingOffset + previous.byteCount > current.bindingOffset)
                {
                    var                 entity             = current.chunk.GetEntityDataPtrRO(esil.AsEntityTypeHandle())[current.entityIndex];
                    var                 componentName      = TypeManager.GetTypeInfo(current.typeIndex).DebugTypeName;
                    FixedString512Bytes componentNameFixed = new FixedString512Bytes(componentName);
                    throw new InvalidOperationException(
                        $"The {componentNameFixed} blend targeting {current.byteCount} bytes from offset {current.bindingOffset} targeting entity {entity.ToFixedString()} aliases with another blend targeting {previous.byteCount} bytes from offset {previous.bindingOffset}.");
                }
            }
        }
    }
}

