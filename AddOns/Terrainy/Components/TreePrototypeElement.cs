using Unity.Entities;

namespace Latios.Terrainy.Components
{
	[InternalBufferCapacity(32)]
	public struct TreePrototypeElement : IBufferElementData
	{
		public Entity Prefab;
	}
}