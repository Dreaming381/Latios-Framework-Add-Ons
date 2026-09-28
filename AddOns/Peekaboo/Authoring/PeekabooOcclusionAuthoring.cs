using Unity.Entities;
using UnityEngine;

namespace Latios.Peekaboo.Authoring
{
    /// <summary>
    /// Opts a renderer out of occluding, being occluded, or both. Occlusion culling is automatic, so
    /// you only need this when the default is wrong for a renderer.
    /// </summary>
    [AddComponentMenu("Latios/Peekaboo/Occlusion Settings (Peekaboo)")]
    [DisallowMultipleComponent]
    public class PeekabooOcclusionAuthoring : MonoBehaviour
    {
        [Tooltip("When false, this renderer never hides anything else. Useful when the mesh's solid shape doesn't " +
                 "match what actually gets drawn.")]
        public bool occludeOthers = true;

        [Tooltip("When false, occlusion culling never culls this renderer, no matter what's in front of it.")]
        public bool canBeOccluded = true;
    }

    public class PeekabooOcclusionAuthoringBaker : Baker<PeekabooOcclusionAuthoring>
    {
        public override void Bake(PeekabooOcclusionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            if (!authoring.occludeOthers)
                AddComponent<PeekabooDisableOccluderTag>(entity);
            if (!authoring.canBeOccluded)
                AddComponent<PeekabooDisableOccludeeTag>(entity);
        }
    }
}
