using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Terrainy.Components
{
	[StructLayout(LayoutKind.Explicit, Size = 28)]
	[InternalBufferCapacity(0)]
	public struct TreeInstanceElement : IBufferElementData
	{
		[FieldOffset(0)] public float3 position;         // position
		[FieldOffset(12)] public half2 scale;            // (width, height)
		[FieldOffset(16)] public ushort packedRotation;  // packed rotation
		[FieldOffset(18)] public ushort prototypeIndex;  // index for BufferElement in TreePrototypeElement
		[FieldOffset(20)] public uint color;             // packed color
		[FieldOffset(24)] public uint lightmapColor;     // packed lightmap Color
	}
}