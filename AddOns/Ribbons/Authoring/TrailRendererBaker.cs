using Latios.Kinemation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Ribbons.Authoring
{
    public class TrailRendererBaker : Baker<TrailRenderer>
    {
        public override void Bake(TrailRenderer authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);

            if (authoring.autodestruct)
                Debug.LogWarning(
                    $"Ribbons: {authoring.gameObject.name} has autodestruct enabled, which Ribbons doesn't support. The entity will stay alive after the trail fades.",
                    authoring);

            LineRendererBaker.BakeCurvesAndMaterial(this,
                                                      authoring,
                                                      entity,
                                                      authoring.widthCurve,
                                                      authoring.colorGradient,
                                                      float3.zero,
                                                      float3.zero,
                                                      authoring.sharedMaterial,
                                                      authoring.sharedMaterials.Length);

            AddComponent(entity, new RibbonTrailConfig
            {
                alignment          = RibbonAuthoringUtility.ConvertAlignment(authoring.alignment),
                textureMode        = RibbonAuthoringUtility.ConvertTextureMode(authoring.textureMode, authoring),
                widthMultiplier    = authoring.widthMultiplier,
                time               = authoring.time,
                minVertexDistance  = authoring.minVertexDistance,
                emitting           = authoring.emitting,
                numCornerVertices  = authoring.numCornerVertices,
                numCapVertices     = authoring.numCapVertices,
            });
            AddComponent<RibbonTrailEmitterState>(entity, default);
            AddBuffer<RibbonTrailPoint>(entity);
        }
    }
}
