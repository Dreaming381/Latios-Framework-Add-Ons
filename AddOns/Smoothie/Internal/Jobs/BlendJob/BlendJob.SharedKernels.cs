using System;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal partial struct BlendJob
    {
        static readonly int kSimdStride = X86.Avx2.IsAvx2Supported ? 8 : 4;

        unsafe void BuildSimdIndices(ref Span<byte> simdIndices, int chunkCount, bool useEnabledMask, v128 chunkEnabledMask)
        {
            if (useEnabledMask)
            {
                int   indexCount = 0;
                byte* bits       = (byte*)&chunkEnabledMask;
                for (int i = 0; i < 16; i++)
                {
                    if (kSimdStride == 8)
                    {
                        if (bits[i] != 0)
                        {
                            simdIndices[indexCount] = (byte)(i * 8);
                            indexCount++;
                        }
                    }
                    else
                    {
                        if ((bits[i] & 0xf) != 0)
                        {
                            simdIndices[indexCount] = (byte)(i * 8);
                            indexCount++;
                        }
                        if ((bits[i] & 0xf0) != 0)
                        {
                            simdIndices[indexCount] = (byte)(i * 8 + 4);
                            indexCount++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < chunkCount; i += kSimdStride)
                {
                    simdIndices[i / kSimdStride] = (byte)i;
                }
                simdIndices = simdIndices.Slice(0, CollectionHelper.Align(chunkCount, kSimdStride) / kSimdStride);
            }
        }

        unsafe void WriteMasked<T>(ReadOnlySpan<T> src, T* dst, int chunkCount, bool useEnabledMask, v128 chunkEnabledMask) where T : unmanaged
        {
            if (useEnabledMask)
            {
                var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunkCount);
                while (enumerator.NextEntityIndex(out var i))
                    dst[i] = src[i];
            }
            else
            {
                src.Slice(0, chunkCount).CopyTo(new Span<T>(dst, chunkCount));
            }
        }

        unsafe void ReadBindingsMasked<T>(TypeIndex bindingTypeIndex, T* dst, in ArchetypeChunk chunk, int chunkCount, bool useEnabledMask,
                                          v128 chunkEnabledMask, BindingPrimitiveType outputType) where T : unmanaged
        {
            var bindingsArray = (EntityComponentBinding*)chunk.GetDynamicComponentDataPtrRO(ref broker, bindingTypeIndex);
            var enumerator    = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunkCount);
            while (enumerator.NextEntityIndex(out int i))
            {
                var binding      = bindingsArray[i];
                var componentPtr = (byte*)broker.GetUnsafeComponentPtrRO(binding.entity, binding.binding.typeIndex);
                dst[i]           = ComponentBinding.ConvertBindingTo<T>(componentPtr + binding.binding.offset, binding.binding.primitiveType, outputType);
            }
        }
    }
}

