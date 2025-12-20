using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Terrainy.Components
{
	[InternalBufferCapacity(0)]
	public struct DetailCellElement : IBufferElementData
	{
		public float3  Coord;          // (x, y, z) in world space
		public float2  Scale; // ((x, z), y)
		public float  RotationY;
		public ushort PrototypeIndex; // which detail DetailsInstanceElement this belongs to
	}
}