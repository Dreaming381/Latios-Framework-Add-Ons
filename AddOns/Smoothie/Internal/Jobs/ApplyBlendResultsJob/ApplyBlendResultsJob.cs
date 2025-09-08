using Latios.Transforms;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal unsafe struct ApplyBlendResultsJob : IJobParallelForDefer
    {
        [ReadOnly] public NativeArray<OutputChunkData>     outputChunkDataList;
        [ReadOnly] public NativeArray<OutputComponentData> outputComponentDataList;
        [ReadOnly] public NativeArray<OutputValueData>     outputValueDataList;

        public ComponentBroker broker;

        public void Execute(int chunkIndex)
        {
#if !LATIOS_TRANSFORMS_UNITY
            var worldTransformTypeIndex = TypeManager.GetTypeIndex<WorldTransform>();
#endif

            var chunkData = outputChunkDataList[chunkIndex];
            for (int componentIndex = 0; componentIndex < chunkData.componentCount; componentIndex++)
            {
                var componentData = outputComponentDataList[chunkData.componentStartIndex + componentIndex];

#if !LATIOS_TRANSFORMS_UNITY
                if (componentData.typeIndex == worldTransformTypeIndex)
                {
                    var worldTransforms      = chunkData.targetChunk.GetNativeArray<WorldTransform>(ref broker);
                    var hasLocalTransforms   = chunkData.targetChunk.Has<LocalTransform>();  // The type might not be in the broker.
                    var localTransforms      = hasLocalTransforms ? chunkData.targetChunk.GetNativeArray<LocalTransform>(ref broker) : default;
                    var parentTransforms     = hasLocalTransforms ? chunkData.targetChunk.GetNativeArray<ParentToWorldTransform>(ref broker) : default;
                    var transformAspectArray = new TransformAspect.ResolvedChunk
                    {
                        Length                                      = worldTransforms.Length,
                        TransformAspect_m_worldTransformNaC         = worldTransforms,
                        TransformAspect_m_localTransformNaC         = localTransforms,
                        TransformAspect_m_parentToWorldTransformNaC = parentTransforms,
                    };

                    for (int i = 0; i < componentData.outputValueCount; i++)
                    {
                        var  index                  = outputValueDataList[componentData.outputValueStartIndex + i].entityIndex;
                        var  transform              = transformAspectArray[index];
                        var  worldTransform         = transform.worldTransform;
                        var  localTransform         = transform.localTransform;
                        var  worldTransformPtr      = (byte*)&worldTransform;
                        var  localTransformPtr      = (byte*)&localTransform;
                        bool worldTransformModified = false;
                        bool localTransformModified = false;
                        for (int j = 0; j < componentData.outputValueCount; i++, j++)
                        {
                            var outputData = outputValueDataList[componentData.outputValueStartIndex + j];
                            if (outputData.entityIndex != index)
                            {
                                i--;
                                break;
                            }
                            if (outputData.componentBindingOffset >= 48)
                            {
                                localTransformModified = true;
                                UnsafeUtility.MemCpy(localTransformPtr + outputData.componentBindingOffset - 48, outputData.outputPtr, outputData.componentBindingByteCount);
                            }
                            else
                            {
                                worldTransformModified = true;
                                UnsafeUtility.MemCpy(worldTransformPtr + outputData.componentBindingOffset, outputData.outputPtr, outputData.componentBindingByteCount);
                            }
                        }
                        if (worldTransformModified)
                            transform.worldTransform = worldTransform;
                        if (localTransformModified)
                            transform.localTransform = localTransform;
                    }
                }
                else
#endif
                {
                    var componentArray = (byte*)chunkData.targetChunk.GetDynamicComponentDataPtrRW(ref broker, componentData.typeIndex);
                    var stride         = TypeManager.GetTypeInfo(componentData.typeIndex).SizeInChunk;
                    for (int i = 0; i < componentData.outputValueCount; i++)
                    {
                        var outputData = outputValueDataList[componentData.outputValueStartIndex + i];
                        var dst        = componentArray + stride * outputData.entityIndex + outputData.componentBindingOffset;
                        UnsafeUtility.MemCpy(dst, outputData.outputPtr, outputData.componentBindingByteCount);
                    }
                }
            }
        }
    }
}

