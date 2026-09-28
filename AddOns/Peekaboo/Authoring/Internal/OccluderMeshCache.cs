using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Plates already built for a mesh, kept between bakes so a live update only rebuilds the meshes
    /// that changed.
    /// </summary>
    /// <remarks>
    /// A mesh in the asset database is keyed by its asset's dependency hash, its local file ID, and
    /// its dirty count. So reimporting the asset, changing its import settings, or editing it in memory
    /// all change the key, without reading any mesh data. Any other mesh is keyed by a hash of what the
    /// plate baker reads from it.
    ///
    /// The cache is static, so it also carries over between subscenes. It's cleared by a domain reload,
    /// which also covers changes to the plate baker itself.
    /// </remarks>
    internal static class OccluderMeshCache
    {
        public sealed class Entry
        {
            public OccluderMesh    mesh;  // plateStart is zero.
            public OccluderPlate[] plates;
            public float3[]        vertices;
            public long            lastUsed;
            public long            bytes;
        }

        // Plates for a few thousand typical meshes.
        const long kBudgetBytes = 64L * 1024 * 1024;

        static readonly Dictionary<Hash128, Entry> s_entries = new Dictionary<Hash128, Entry>();
        static long                                s_bytes;
        static long                                s_clock;

        public static int count => s_entries.Count;

        public static bool TryGet(Hash128 key, out Entry entry)
        {
            if (!s_entries.TryGetValue(key, out entry))
                return false;
            entry.lastUsed = ++s_clock;
            return true;
        }

        /// <summary>
        /// Copies one mesh's plates out of the plate baker's output, keeping only the plates that
        /// survived ranking and the vertices they use.
        /// </summary>
        public static unsafe Entry Add(Hash128 key, in OccluderPlateBaker.Result result, in UnsafeList<OccluderPlate> plates, in UnsafeList<float3> vertices)
        {
            var entry = new Entry
            {
                mesh = new OccluderMesh
                {
                    plateStart             = 0,
                    plateCount             = result.plateCount,
                    largestPlateArea       = result.largestPlateArea,
                    boundsCrossSectionArea = result.boundsCrossSectionArea,
                    hull                   = result.hull,
                    shellSubmeshes         = result.shellSubmeshes,
                },
                plates   = new OccluderPlate[result.plateCount],
                lastUsed = ++s_clock,
            };

            int vertexCount = 0;
            for (int p = 0; p < result.plateCount; p++)
                vertexCount += plates[result.plateStart + p].vertexCount;
            entry.vertices = new float3[vertexCount];

            int cursor = 0;
            for (int p = 0; p < result.plateCount; p++)
            {
                var plate         = plates[result.plateStart + p];
                int source        = plate.vertexStart;
                plate.vertexStart = cursor;
                for (int v = 0; v < plate.vertexCount; v++)
                    entry.vertices[cursor++] = vertices[source + v];
                entry.plates[p] = plate;
            }

            entry.bytes = (long)entry.plates.Length * UnsafeUtility.SizeOf<OccluderPlate>() + (long)vertexCount * UnsafeUtility.SizeOf<float3>() + 128;
            if (s_entries.TryGetValue(key, out var old))
                s_bytes -= old.bytes;
            s_entries[key]  = entry;
            s_bytes        += entry.bytes;
            return entry;
        }

        /// <summary>
        /// Drops the least recently used meshes until the cache fits its budget.
        /// </summary>
        public static void Trim()
        {
            if (s_bytes <= kBudgetBytes)
                return;
            var byAge = new List<KeyValuePair<Hash128, Entry> >(s_entries);
            byAge.Sort((a, b) => a.Value.lastUsed.CompareTo(b.Value.lastUsed));
            for (int i = 0; i < byAge.Count && s_bytes > kBudgetBytes; i++)
            {
                s_entries.Remove(byAge[i].Key);
                s_bytes -= byAge[i].Value.bytes;
            }
        }

        public static void Clear()
        {
            s_entries.Clear();
            s_bytes = 0;
        }

        /// <summary>
        /// The key for a mesh saved in the asset database, without reading its data. Returns false for
        /// any other mesh, including built-in ones, which need their contents hashed instead.
        /// </summary>
        public static unsafe bool TryGetAssetKey(Mesh mesh, out Hash128 key)
        {
            key = default;
#if UNITY_EDITOR
            if (!UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh, out string guid, out long localId))
                return false;
            var dependencyHash = UnityEditor.AssetDatabase.GetAssetDependencyHash(new UnityEditor.GUID(guid));
            if (!dependencyHash.isValid)
                return false;
            var input = new AssetKeyInput
            {
                dependencyHash = dependencyHash,
                localId        = localId,
                dirtyCount     = UnityEditor.EditorUtility.GetDirtyCount(mesh),
            };
            key = new Hash128(xxHash3.Hash128(&input, UnsafeUtility.SizeOf<AssetKeyInput>(), 0x9eb0a5u));
            return true;
#else
            return false;
#endif
        }

        struct AssetKeyInput
        {
            public UnityEngine.Hash128 dependencyHash;
            public long                localId;
            public int                 dirtyCount;
            public int                 padding;
        }
    }
}
