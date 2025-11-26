using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Terrainy.Components
{
	[InternalBufferCapacity(0)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct DetailCellElement : IBufferElementData
	{
		[FieldOffset(0)]  public int2   Coord;          // (x, y) in detail-resolution grid
		[FieldOffset(8)]  public ushort Count;          // density at cell
		[FieldOffset(10)] public ushort PrototypeIndex; // which detail DetailsInstanceElement this belongs to
	}
}