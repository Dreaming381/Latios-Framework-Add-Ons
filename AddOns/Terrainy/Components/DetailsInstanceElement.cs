using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Terrainy.Components
{
	[InternalBufferCapacity(32)]
	public struct DetailsInstanceElement : IBufferElementData
	{
		public Entity Prefab;     // Entity for mesh-based details, Entity.Null for texture-based
		public uint HealthyColor; // Packed RGBA
		public uint DryColor;     // Packed RGBA
		public half2 MinSize;     // (minWidth, minHeight)
		public half2 MaxSize;     // (maxWidth, maxHeight)
		public half NoiseSpread;
		public byte UseMesh;       // 1 if mesh prototype, 0 otherwise
		public byte RenderMode;    // UnityEngine.DetailRenderMode
		public half AlignToGround; // Rotate detail axis parallel to the ground's normal direction, so that the detail is perpendicular to the ground.
	}
}