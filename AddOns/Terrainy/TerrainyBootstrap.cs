using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Latios;
using Latios.Terrainy.Systems;

namespace Latios.Terrainy
{
	public static class TerrainyBootstrap
	{
		/// <summary>
		/// Install Terrainy systems into the World.
		/// </summary>
		/// <param name="world">The world to install Terrainy into.</param>
		public static void InstallTerrainy(World world)
		{
			BootstrapTools.InjectSystem(TypeManager.GetSystemTypeIndex<TerrainSystem>(), world);
		}
	}
}