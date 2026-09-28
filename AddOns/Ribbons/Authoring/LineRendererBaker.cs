using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Ribbons.Authoring
{
    public class LineRendererBaker : Baker<LineRenderer>
    {
        public override void Bake(LineRenderer authoring)
        {
            int count = math.max(authoring.positionCount, 0);
            var raw   = new Vector3[count];
            if (count > 0)
                authoring.GetPositions(raw);

            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);

            float3 boundsMin = float3.zero;
            float3 boundsMax = float3.zero;
            if (count > 0)
            {
                bool     worldSpace   = authoring.useWorldSpace;
                float4x4 worldToLocal = worldSpace ? (float4x4)authoring.transform.worldToLocalMatrix : float4x4.identity;

                var pointBuffer = AddBuffer<RibbonPoint>(entity);
                for (int i = 0; i < count; i++)
                {
                    float3 p                                   = worldSpace ? math.transform(worldToLocal, raw[i]) : (float3)raw[i];
                    pointBuffer.Add(new RibbonPoint { position = p });
                    boundsMin                                  = i == 0 ? p : math.min(boundsMin, p);
                    boundsMax                                  = i == 0 ? p : math.max(boundsMax, p);
                }
            }
            else
            {
                AddBuffer<RibbonPoint>(entity);
            }

            BakeCurvesAndMaterial(this, authoring, entity, authoring.widthCurve, authoring.colorGradient, boundsMin, boundsMax,
                                  authoring.sharedMaterial, authoring.sharedMaterials.Length);

            var ribbonAlignment = RibbonAuthoringUtility.ConvertAlignment(authoring.alignment);
            AddComponent(entity, new RibbonLineConfig
            {
                alignment         = ribbonAlignment,
                textureMode       = RibbonAuthoringUtility.ConvertTextureMode(authoring.textureMode, authoring),
                widthMultiplier   = authoring.widthMultiplier,
                loop              = authoring.loop,
                numCornerVertices = authoring.numCornerVertices,
                numCapVertices    = authoring.numCapVertices,
            });
            SetComponentEnabled<RibbonLineConfig>(entity, true);

            if (ribbonAlignment == RibbonAlignment.View)
                AddComponent<RibbonViewAlignedTag>(entity);
        }

        internal static void BakeCurvesAndMaterial(IBaker baker,
                                                   Renderer authoring,
                                                   Entity entity,
                                                   AnimationCurve widthCurve,
                                                   Gradient colorGradient,
                                                   float3 boundsMin,
                                                   float3 boundsMax,
                                                   Material material,
                                                   int materialCount)
        {
            if (materialCount > 1)
                Debug.LogWarning($"Ribbons: {authoring.gameObject.name} has {materialCount} materials, but Ribbons only uses the first one.", authoring);

            RibbonCurves.SetWidthCurve(widthCurve, baker.AddBuffer<RibbonWidthKeyframe>(entity));
            RibbonCurves.SetGradient(colorGradient, baker.AddBuffer<RibbonColorKey>(entity));

            var settings = new MeshRendererBakeSettings
            {
                targetEntity                = entity,
                renderMeshDescription       = new Unity.Rendering.RenderMeshDescription(authoring),
                isDeforming                 = false,
                isStatic                    = false,
                lightmapIndex               = authoring.lightmapIndex,
                lightmapScaleOffset         = authoring.lightmapScaleOffset,
                useLightmapsIfPossible      = false,
                suppressDeformationWarnings = false,
                localBounds                 = new Bounds((boundsMin + boundsMax) * 0.5f, boundsMax - boundsMin),
            };
            baker.BakeMeshAndMaterial(settings, RenderingBakingTools.uniqueMeshPlaceholder, material);

            baker.AddComponent(entity, new UniqueMeshConfig { });
            baker.AddComponent(entity, Systems.RibbonMeshBuilder.VertexLayout());
            baker.AddBuffer<UniqueMeshVertexRawData>(entity);
            baker.AddBuffer<UniqueMeshIndex>(entity);
        }
    }

    internal static class RibbonAuthoringUtility
    {
        public static RibbonAlignment ConvertAlignment(LineAlignment alignment)
        {
            return alignment == LineAlignment.TransformZ ? RibbonAlignment.TransformZ : RibbonAlignment.View;
        }

        public static RibbonTextureMode ConvertTextureMode(LineTextureMode mode, Renderer context)
        {
            switch (mode)
            {
                case LineTextureMode.Stretch: return RibbonTextureMode.Stretch;
                case LineTextureMode.Tile: return RibbonTextureMode.Tile;
                case LineTextureMode.DistributePerSegment: return RibbonTextureMode.DistributeEvenly;
                default:
                    Debug.LogWarning($"Ribbons: {context.gameObject.name} uses texture mode {mode}, which Ribbons doesn't support. Using Tile instead.", context);
                    return RibbonTextureMode.Tile;
            }
        }
    }
}

