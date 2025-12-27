using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal unsafe struct CapturedChunkData
    {
        public v128           enabledMask;
        public bool           useEnabledMask;
        public byte           countInChunk;
        public byte           enabledCount;
        public void*          outputPtr;
        public OutputBinding* bindingPtr;
        public OutputEntity*  entityPtr;
        public short          outputByteCountPerBlend;
    }

    internal struct OutputChunkData
    {
        public ArchetypeChunk targetChunk;
        public int            componentStartIndex;
        public int            componentCount;
    }

    internal struct OutputComponentData
    {
        public TypeIndex typeIndex;
        public int       outputValueStartIndex;
        public int       outputValueCount;
    }

    internal unsafe struct OutputValueData
    {
        public void* outputPtr;
        public int   componentBindingOffset;
        public short componentBindingByteCount;
        public byte  entityIndex;
    }
}

