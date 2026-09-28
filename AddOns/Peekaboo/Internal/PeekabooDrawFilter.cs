using Latios.Kinemation;
using Unity.Collections.LowLevel.Unsafe;

namespace Latios.Peekaboo
{
    /// <summary>
    /// One blob mesh a renderer is drawing, and which of its submeshes are drawn opaque.
    /// </summary>
    internal struct OccluderMeshDraw
    {
        public int   meshIndex;
        public ulong opaqueSubmeshes;
    }

    /// <summary>
    /// Turns what Kinemation says a renderer is drawing this pass into the blob meshes it can occlude
    /// with, and which of their plates are usable.
    /// </summary>
    /// <remarks>
    /// Rule 3 only counts triangles that fully hide what's behind them. So a plate is only usable
    /// while the submesh it depends on is being drawn with an opaque material. This is the only place
    /// materials get looked at. Baking cuts plates for every submesh no matter what it's drawn with.
    ///
    /// A MaterialMeshInfo range draws several mesh, material, and submesh combos at one transform.
    /// Every mesh among them is real geometry in the same place, so each one can occlude. They're
    /// usually the same mesh repeated for different submeshes, so draws get merged per mesh.
    /// </remarks>
    internal static unsafe class PeekabooDrawFilter
    {
        /// <summary>
        /// Writes the distinct blob meshes the renderer is drawing that have plates and at least one
        /// opaque submesh, and returns how many.
        /// </summary>
        /// <param name="drawn">What OcclusionCullingContextAspect resolved for the renderer</param>
        /// <param name="meshes">Receives the meshes, at least maxMeshes long</param>
        public static int CollectOccluderMeshes(ref PeekabooOccluderBlob blob,
                                                in UnsafeList<OcclusionCullingContextAspect.MaterialMeshSubmesh> drawn,
                                                OccluderMeshDraw* meshes,
                                                int maxMeshes)
        {
            int count = 0;
            for (int d = 0; d < drawn.Length; d++)
            {
                var draw      = drawn[d];
                int meshIndex = draw.meshRmaIndex;
                // A runtime registered mesh has no RenderMeshArray index. Only a hand-built blob with a
                // single mesh can stand in for one.
                if (meshIndex < 0 && blob.handBuilt && blob.meshes.Length == 1)
                    meshIndex = 0;
                if (!HasPlates(ref blob, meshIndex))
                    continue;

                ulong opaque = IsOpaque(ref blob, draw.materialRmaIndex) && draw.submeshIndex < 64 ? 1ul << draw.submeshIndex : 0ul;
                if (blob.handBuilt)
                    opaque = ulong.MaxValue;

                int slot = 0;
                while (slot < count && meshes[slot].meshIndex != meshIndex)
                    slot++;
                if (slot == count)
                {
                    if (count == maxMeshes)
                        continue;
                    meshes[count++] = new OccluderMeshDraw { meshIndex = meshIndex };
                }
                meshes[slot].opaqueSubmeshes |= opaque;
            }

            int kept = 0;
            for (int m = 0; m < count; m++)
            {
                if (meshes[m].opaqueSubmeshes != 0)
                    meshes[kept++] = meshes[m];
            }
            return kept;
        }

        /// <summary>
        /// Whether a plate's submesh dependency is drawn opaque.
        /// </summary>
        public static bool IsPlateUsable(in OccluderPlate plate, in OccluderMesh mesh, ulong opaqueSubmeshes)
        {
            if (plate.submesh < 0)
                return (mesh.shellSubmeshes & ~opaqueSubmeshes) == 0;
            return plate.submesh < 64 && (opaqueSubmeshes & (1ul << plate.submesh)) != 0;
        }

        /// <summary>
        /// Whether every submesh in the mesh's shell is drawn opaque, so its interior plates can be
        /// used.
        /// </summary>
        public static bool IsShellUsable(in OccluderMesh mesh, ulong opaqueSubmeshes)
        {
            return mesh.shellSubmeshes != 0 && (mesh.shellSubmeshes & ~opaqueSubmeshes) == 0;
        }

        /// <summary>
        /// Whether a baked plate depends on one submesh through a narrowed cone, instead of lying on a
        /// surface or depending on the whole shell.
        /// </summary>
        public static bool IsNarrowed(in OccluderPlate plate) => plate.submesh >= 0 && plate.regionKind == PeekabooRegionKind.Cone;

        static bool IsOpaque(ref PeekabooOccluderBlob blob, int materialIndex)
        {
            return materialIndex >= 0 && materialIndex < blob.opaqueMaterials.Length && blob.opaqueMaterials[materialIndex];
        }

        static bool HasPlates(ref PeekabooOccluderBlob blob, int meshIndex)
        {
            return meshIndex >= 0 && meshIndex < blob.meshes.Length && blob.meshes[meshIndex].plateCount > 0;
        }
    }
}
