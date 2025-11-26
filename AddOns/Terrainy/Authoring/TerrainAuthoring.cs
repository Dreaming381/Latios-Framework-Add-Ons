using System;
using System.Runtime.CompilerServices;
using Latios.Terrainy.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Latios.Terrainy.Authoring
{
	[DisableAutoCreation]
	public class TerrainAuthoring : Baker<Terrain>
	{
		public override void Bake(Terrain authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Renderable);

			TerrainData data = authoring.terrainData;
			DependsOn(data);

			// Modifying the heightmap in the editor does not cause TerrainData to propagate as a changed object and trigger a rebake.
			// Therefore, we need to use the same TerrainData for runtime that we use for authoring while in the editor and can only
			// strip the trees and details for a build.
			NativeArray<TreeInstanceElement> treeInstanceComponents = new NativeArray<TreeInstanceElement>(data.treeInstances.Length, Allocator.Temp);
			NativeArray<TreePrototypeElement> entitiesPrototypes = new NativeArray<TreePrototypeElement>(data.treePrototypes.Length, Allocator.Temp);
			NativeArray<DetailsInstanceElement> detailPrototypesArray = new NativeArray<DetailsInstanceElement>(data.detailPrototypes.Length, Allocator.Temp);
			NativeList<DetailCellElement> detailCells = new NativeList<DetailCellElement>(Allocator.Temp);;

			if (!IsBakingForEditor())
			{
				data = Object.Instantiate(data);
				// TODO Probably defer this to a system, so that we can have one global list instead of multiple which might share the same entities 
				for (int i = 0; i < data.treePrototypes.Length; i++)
				{
					ref readonly TreePrototype treePrototype = ref data.treePrototypes[i];
					var entityPrototype = GetEntity(treePrototype.prefab, TransformUsageFlags.Dynamic);
					entitiesPrototypes[i] = new TreePrototypeElement
					{
						Prefab = entityPrototype,
					};
				}

				for (var i = 0; i < data.treeInstances.Length; i++)
				{
					ref readonly TreeInstance treeInstance = ref data.treeInstances[i];
					treeInstanceComponents[i] = new TreeInstanceElement
					{
						position = treeInstance.position,
						scale = new half2(new half(treeInstance.widthScale),  new half(treeInstance.heightScale)),
						packedRotation = EncodeRotation(treeInstance.rotation),
						prototypeIndex = (ushort)math.clamp(treeInstance.prototypeIndex, ushort.MinValue, ushort.MaxValue),
						color = PackColor32(treeInstance.color),
						lightmapColor = PackColor32(treeInstance.lightmapColor),
					};
				}
				var detailResolution = data.detailResolution;
				var detailPrototypeCount = data.detailPrototypes.Length;

				for (var i = 0; i < detailPrototypeCount; i++)
				{
					ref readonly DetailPrototype detailPrototype = ref data.detailPrototypes[i];

					// Map prototype prefab to entity if using mesh-based detail; Entity.Null otherwise
					Entity detailPrefabEntity;
					if (detailPrototype.usePrototypeMesh && detailPrototype.prototype != null)
					{
						detailPrefabEntity = GetEntity(detailPrototype.prototype, TransformUsageFlags.Dynamic);
					}
					else
					{
						//detailPrototype.prototypeTexture;
						detailPrefabEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
						
					}

					detailPrototypesArray[i] = new DetailsInstanceElement()
					{
						Prefab = detailPrefabEntity,
						HealthyColor = PackColor32(detailPrototype.healthyColor),
						DryColor = PackColor32(detailPrototype.dryColor),
						MinSize = new half2((half)detailPrototype.minWidth, (half)detailPrototype.minHeight),
						MaxSize = new half2((half)detailPrototype.maxWidth, (half)detailPrototype.maxHeight),
						NoiseSpread = (half)detailPrototype.noiseSpread,
						UseMesh = (byte)(detailPrototype.usePrototypeMesh ? 1 : 0),
						RenderMode = (byte)detailPrototype.renderMode,
						AlignToGround = new half(detailPrototype.alignToGround),
					};
					
					int[,] layer = data.GetDetailLayer(0, 0, detailResolution, detailResolution, i);
					// Convert dense 2D array to sparse cell elements (skip zeros)
					for (int y = 0; y < detailResolution; y++)
					{
						for (int x = 0; x < detailResolution; x++)
						{
							int count = layer[y, x];
							if (count <= 0)
								continue;

							detailCells.Add(new DetailCellElement
							{
								Coord = new int2(x, y),
								Count = (ushort)math.clamp(count, 0, ushort.MaxValue),
								PrototypeIndex = (ushort)math.clamp(i, 0, ushort.MaxValue),
							});
						}
					}
				}

				// Todo: This probably needs more testing and iteration.
				data.detailPrototypes = null;
				data.treeInstances = Array.Empty<TreeInstance>();
				data.treePrototypes = null;
			}

			if (detailPrototypesArray.IsCreated)
			{
				var detailProtoBuffer = AddBuffer<DetailsInstanceElement>(entity);
				detailProtoBuffer.AddRange(detailPrototypesArray);
			}
			if (detailCells.IsCreated)
			{
				var detailCellBuffer = AddBuffer<DetailCellElement>(entity);
				detailCellBuffer.AddRange(detailCells.AsArray());
			}

			var treeInstanceBuffer = AddBuffer<TreeInstanceElement>(entity);
			treeInstanceBuffer.AddRange(treeInstanceComponents);
			var treePrototypeBuffer = AddBuffer<TreePrototypeElement>(entity);
			treePrototypeBuffer.AddRange(entitiesPrototypes);
			AddComponent(entity, new TerrainDataComponent
			{
				TerrainData = data,
				TerrainMat = authoring.materialTemplate
			});
			AddComponent<TerrainLiveBakedTag>(entity);
		}

		private static ushort EncodeRotation(float radians)
		{
			float a = math.fmod(radians, 2f * math.PI);
			if (a < 0) a += 2f * math.PI;
			return (ushort)math.round(a * (65535f / (2f * math.PI)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint PackColor32(Color32 c)
		{
			return (uint)(c.r | (c.g << 8) | (c.b << 16) | (c.a << 24));
		}
	}

}