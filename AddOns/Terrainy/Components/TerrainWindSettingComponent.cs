using Unity.Entities;
using UnityEngine;

namespace Latios.Terrainy.Components
{
	/// <summary>
	/// Only applies to details, trees are different.
	/// TODO: Make this somehow material properties?
	/// </summary>
	public struct TerrainWindSettingComponent : IComponentData
	{
		public float Speed; // How quickly the wind moves the grass from side to side 0-1
		public float Size; // The size of ripples on grassy areas. Lower values create a uniform movement; higher values create waves of motion in different directions 0-1
		public float Bending; // 0 = stop, 1 moves the grass halfway towards the ground 0-1
		public Color GrassTint; // The final color for each grass is the Grass Tint multiplied by the grass’s Healthy Color and Dry Color values, which you can set for each grass type individually
	}
}