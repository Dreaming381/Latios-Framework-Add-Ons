using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Terrainy.Components
{
	[InternalBufferCapacity(0)]
	public struct TreeInstanceElement : IBufferElementData
	{
		public float3 Position;         // world position
		public half2 Scale;            // (width, height)
		public float Rotation;		// rotation
		public ushort PrototypeIndex;  // index for BufferElement in TreePrototypeElement
	}
}