using Latios.Authoring;
using Latios.Psyshock;
using Latios.Psyshock.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Collider = Latios.Psyshock.Collider;
using TerrainCollider = UnityEngine.TerrainCollider;

namespace Latios.Terrainy.Authoring
{
	[DisableAutoCreation]
	public class TerrainColliderBaker : SmartBaker<TerrainCollider, TerrainColliderBakeItem> { }

	[TemporaryBakingType]
	public struct TerrainColliderBakeItem : ISmartBakeItem<TerrainCollider>
	{
		private SmartBlobberHandle<TerrainColliderBlob> _blobberHandle;
		private float3 _scale;

		public bool Bake(TerrainCollider authoring, IBaker baker)
		{
			Entity entity = baker.GetEntity(TransformUsageFlags.Renderable);
			baker.AddComponent<Collider>(entity);
			TerrainData terrainData = authoring.terrainData;
			int heightmapResolution = terrainData.heightmapResolution;
			int holesResolution = terrainData.holesResolution;
			int quadsPerRow = heightmapResolution - 1;
			Vector3 size = terrainData.size;

			float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
			bool[,] holes = terrainData.GetHoles(0, 0, holesResolution, holesResolution);

			var heightsRowMajor = new NativeArray<short>(heightmapResolution * heightmapResolution, Allocator.Temp);
			for (var y = 0; y < heightmapResolution; y++)
			{
				for (var x = 0; x < heightmapResolution; x++)
				{
					float h = heights[y, x];
					var converted = (short)(math.clamp(h, 0f, 1f) * size.y);
					heightsRowMajor[x + y * heightmapResolution] = converted;
				}
			}

			int quadCount = quadsPerRow * (quadsPerRow + 1);

			NativeArray<BitField32> quadTriangleSplitParities = GenerateParitiesFromHeights(heightsRowMajor, quadsPerRow);

			int triangleWords = (quadCount + 31) / 32;
			var trianglesValid = new NativeArray<BitField64>(triangleWords, Allocator.Temp);

			for (var i = 0; i < triangleWords; i++)
				trianglesValid[i] = new BitField64(~0ul);
			for (var y = 0; y < quadsPerRow; y++)
			{
				for (var x = 0; x < quadsPerRow; x++)
				{
					// holes[y, x] is true if the quad is solid, false if it is a hole
					if (holes[y, x]) continue;

					int quadIndex = y * quadsPerRow + x;
					int wordIndex = quadIndex / 32;
					int bitIndex = quadIndex % 32;
					bitIndex *= 2;

					var field = trianglesValid[wordIndex];
					field.SetBits(bitIndex, false, 2);
					trianglesValid[wordIndex] = field;
				}
			}

			var fixedName = new FixedString128Bytes(terrainData.name);

			_blobberHandle = baker.RequestCreateTerrainBlobAsset(quadsPerRow, heightsRowMajor, quadTriangleSplitParities, trianglesValid, fixedName);
			this._scale = ComputeTerrainScale(size, quadsPerRow, heights.Length, heightsInMeters: true, heightsNormalized01: false);
			return _blobberHandle.IsValid;
		}

		// TODO i think its everytime in meters, but needs testing probably
		private static float ComputeSy(Vector3 terrainDataSize, bool heightsInMeters, bool heightsNormalized01)
		{
			if (heightsInMeters) return 1f;
			if (heightsNormalized01) return terrainDataSize.y;
			return terrainDataSize.y / 32767f;
		}

		private static float3 ComputeTerrainScale(Vector3 terrainDataSize, int quadsPerRow, int heightsLength, bool heightsInMeters, bool heightsNormalized01)
		{
			int rowCount = heightsLength / (quadsPerRow + 1);
			int quadsPerCol = math.max(1, rowCount - 1);

			float sx = terrainDataSize.x / quadsPerRow;
			float sz = terrainDataSize.z / quadsPerCol;
			float sy = ComputeSy(terrainDataSize, heightsInMeters, heightsNormalized01);

			return new float3(sx, sy, sz);
		}

		private static NativeArray<BitField32> GenerateParitiesFromHeights(NativeArray<short> heights, int quadsPerRow)
		{
			int vertsPerRow = quadsPerRow + 1;
			int totalQuads = quadsPerRow * (quadsPerRow + 1);
			int bitfieldCount = (totalQuads + 31) / 32;

			var parities = new NativeArray<BitField32>(bitfieldCount, Allocator.Temp);

			for (var y = 0; y < quadsPerRow; y++)
			{
				for (var x = 0; x < quadsPerRow; x++)
				{
					int topLeft = y * vertsPerRow + x;
					int topRight = y * vertsPerRow + (x + 1);
					int bottomLeft = (y + 1) * vertsPerRow + x;
					int bottomRight = (y + 1) * vertsPerRow + (x + 1);

					short hTL = heights[topLeft];
					short hTR = heights[topRight];
					short hBL = heights[bottomLeft];
					short hBR = heights[bottomRight];

					// Diagonal A: TL -> BR (parity = 0)
					int flatnessA = math.abs(hTL - hBR) + math.abs(hTR - hBL);

					// Diagonal B: BL -> TR (parity = 1)
					int flatnessB = math.abs(hBL - hTR) + math.abs(hTL - hBR);

					bool useParity1 = flatnessB < flatnessA;

					int quadIndex = x + y * quadsPerRow;
					int wordIndex = quadIndex / 32;
					int bitOffset = quadIndex % 32;

					if (!useParity1) continue;

					BitField32 current = parities[wordIndex];
					current.Value |= (1u << bitOffset);
					parities[wordIndex] = current;
				}
			}

			return parities;
		}

		public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
		{
			Collider collider = new Latios.Psyshock.TerrainCollider()
			{
				terrainColliderBlob = this._blobberHandle.Resolve(entityManager),
				scale = this._scale,
				baseHeightOffset = 0,
			};
			entityManager.SetComponentData(entity, collider);
		}
	}
}