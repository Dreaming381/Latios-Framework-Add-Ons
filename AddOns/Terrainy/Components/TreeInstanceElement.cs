using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Terrainy.Components
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	[InternalBufferCapacity(0)]
	public struct TreeInstanceElement : IBufferElementData
	{
		[FieldOffset(0)] public float3 Position;         // position
		[FieldOffset(12)] public half2 Scale;            // (width, height)
		[FieldOffset(16)] public ushort PackedRotation;  // packed rotation
		[FieldOffset(18)] public ushort PrototypeIndex;  // index for BufferElement in TreePrototypeElement
	}
}