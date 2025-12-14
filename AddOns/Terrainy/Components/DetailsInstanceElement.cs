using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Terrainy.Components
{
	[InternalBufferCapacity(32)]
	public struct DetailsInstanceElement : IBufferElementData
	{
		public Entity Prefab;                           // Entity for mesh-based details, Entity.Null for texture-based
		public half2 MinSize;                           // (minWidth, minHeight)
		public half2 MaxSize;                           // (maxWidth, maxHeight)
		public byte UseMesh;                            // 1 if mesh prototype, 0 otherwise
		public UnityEngine.DetailRenderMode RenderMode; // UnityEngine.DetailRenderMode
	}
}