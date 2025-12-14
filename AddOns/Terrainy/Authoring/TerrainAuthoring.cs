using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Latios.Kinemation.Authoring;
using Latios.Terrainy.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Latios.Terrainy.Authoring
{
	[DisableAutoCreation]
	public class TerrainAuthoring : Baker<Terrain>
	{
		private static readonly int HealthyColor = Shader.PropertyToID("_HealthyColor");
		private static readonly int DryColor = Shader.PropertyToID("_DryColor");
		private static readonly int Billboard = Shader.PropertyToID("_Billboard");
		private static readonly int Lerp = Shader.PropertyToID("_Lerp");
		private static readonly int Speed = Shader.PropertyToID("_Speed");
		private static readonly int Bending = Shader.PropertyToID("_Bending");
		private static readonly int Size = Shader.PropertyToID("_Size");
		private static readonly int GrassTint = Shader.PropertyToID("_GrassTint");
		
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

			//if (!IsBakingForEditor())
			//{
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
				var quadMesh = new Mesh();
				quadMesh.SetVertices(new List<Vector3> {
					new Vector3(-0.5f, 0f, 0f), new Vector3(0.5f, 0f, 0f), new Vector3(-0.5f, 1f, 0f), new Vector3(0.5f, 1f, 0f)
				});
				quadMesh.SetUVs(0, new List<Vector2> {
					new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1)
				});
				quadMesh.SetIndices(new[]{0,2,1, 1,2,3}, MeshTopology.Triangles, 0, true);
				quadMesh.RecalculateBounds();
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
						// TODO this renders at 0,0, which should not happen
						detailPrefabEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
						var shader = Shader.Find("Shader Graphs/GrasLatiosShader");
						var material = new Material(shader)
						{
							enableInstancing = true,
							mainTexture = detailPrototype.prototypeTexture,
							name = $"GrasMat_{i}"
						};
						#if UNITY_EDITOR
						// Fixes Unity Editor bug with an open subscene
						material.SetKeyword(new LocalKeyword(shader, "_SURFACE_TYPE_TRANSPARENT"), true);
						material.SetKeyword(new LocalKeyword(shader, "_ALPHATEST_ON"), true);
						#endif
						// this is deactivated on the billboard? why tho
						material.SetColor(HealthyColor,  detailPrototype.healthyColor);
						material.SetColor(DryColor,  detailPrototype.dryColor);
						material.SetFloat(Billboard, detailPrototype.renderMode == DetailRenderMode.GrassBillboard ? 1 : 0);
						material.SetFloat(Lerp, detailPrototype.renderMode == DetailRenderMode.GrassBillboard ? 0 : 1);
						// don't worry I don't understand why speed = strength and size is speed, unity naming I guess lol
						material.SetFloat(Speed, authoring.terrainData.wavingGrassStrength);
						material.SetFloat(Bending, authoring.terrainData.wavingGrassAmount);
						material.SetFloat(Size, authoring.terrainData.wavingGrassSpeed);
						material.SetColor(GrassTint, authoring.terrainData.wavingGrassTint);
						var meshRendererBakeSettings = new MeshRendererBakeSettings()
						{
							targetEntity = detailPrefabEntity,
							isDeforming = false,
							isStatic = true,
							lightmapIndex = 0,
							lightmapScaleOffset = float4.zero,
							localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 0)),
							renderMeshDescription = new RenderMeshDescription(ShadowCastingMode.Off),
							suppressDeformationWarnings = true,
							useLightmapsIfPossible = true
						};
						this.BakeMeshAndMaterial(meshRendererBakeSettings, quadMesh, material);
					}

					detailPrototypesArray[i] = new DetailsInstanceElement()
					{
						Prefab = detailPrefabEntity,
						MinSize = new half2((half)detailPrototype.minWidth, (half)detailPrototype.minHeight),
						MaxSize = new half2((half)detailPrototype.maxWidth, (half)detailPrototype.maxHeight),
						UseMesh = (byte)(detailPrototype.usePrototypeMesh ? 1 : 0),
						RenderMode = detailPrototype.renderMode,
					};
					// Get details per patch, since the method is c++ internal and im not sure how they compute, get it over the method
					for (int y = 0; y < detailResolution; y += data.detailResolutionPerPatch)
					{
						for (int x = 0; x < detailResolution; x += data.detailResolutionPerPatch)
						{
							var transforms = data.ComputeDetailInstanceTransforms(x / data.detailResolutionPerPatch, y / data.detailResolutionPerPatch, i, detailPrototype.density, out Bounds bounds);
							foreach (var transform in transforms)
							{
								detailCells.Add(new DetailCellElement
								{
									Coord = new float3(transform.posX, transform.posY, transform.posZ),
									Scale = new float2(transform.scaleXZ, transform.scaleY),
									RotationY = transform.rotationY,
									PrototypeIndex = (ushort)math.clamp(i, 0, ushort.MaxValue),
								});
							}
						}
					}
				}

				// Todo: This probably needs more testing and iteration.
				data.detailPrototypes = null;
				data.treeInstances = Array.Empty<TreeInstance>();
				data.treePrototypes = null;
			//}

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