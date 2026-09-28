using Latios.Kinemation;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Turns a culling pass into the views that occlusion culling rasterizes into.
    /// </summary>
    /// <remarks>
    /// A culling pass gives us planes, not a projection matrix, and for shadows not even the size of
    /// the shadow map. So instead of rebuilding the real projection, we intersect each split's
    /// culling planes into the convex volume they bound, and fit a projection to exactly that
    /// volume. Everything the split renders is inside that volume, so the fitted projection covers
    /// everything that matters and spends all its resolution there.
    /// </remarks>
    internal static unsafe class PeekabooViewBuilder
    {
        const int kMaxPlanesPerSplit = 16;
        const int kMaxVolumeVertices = 128;

        public static void BuildViews(in CullingContext context,
                                      NativeArray<Plane>        cullingPlanes,
                                      NativeArray<CullingSplit> cullingSplits,
                                      in PeekabooSettings settings,
                                      ref NativeList<PeekabooView> views)
        {
            bool isLight       = context.viewType == BatchCullingViewType.Light;
            bool isPerspective = context.projectionType == BatchCullingProjectionType.Perspective;
            var  resolution    = isLight ? settings.lightResolution : settings.cameraResolution;
            resolution         = math.ceilpow2(math.clamp(resolution, new int2(8, 8), new int2(2048, 2048)));

            // A camera pass leaves localToWorldMatrix as identity, so only the LOD parameters and the
            // planes can be trusted. A light pass is the opposite. The matrix holds the light
            // direction, and the depth axis has to match it or the view would face the wrong way.
            float3 eye = context.lodParameters.cameraPosition;

            var vertices  = stackalloc float3[kMaxVolumeVertices];
            int texelBase = 0;
            for (int i = 0; i < cullingSplits.Length && i < 8; i++)
            {
                var split      = cullingSplits[i];
                int planeCount = math.min(split.cullingPlaneCount, kMaxPlanesPerSplit);
                if (planeCount < 4 || split.cullingPlaneOffset + planeCount > cullingPlanes.Length)
                    continue;

                int vertexCount = IntersectPlanes(cullingPlanes, split.cullingPlaneOffset, planeCount, vertices, kMaxVolumeVertices);
                if (vertexCount < 4)
                    continue;

                float3 centroid = 0f;
                for (int v = 0; v < vertexCount; v++)
                    centroid += vertices[v];
                centroid /= vertexCount;

                float3 forward;
                if (isLight)
                    forward = math.normalizesafe(context.localToWorldMatrix.c2.xyz, new float3(0f, 0f, 1f));
                else
                    forward = SnapToNearPlaneNormal(cullingPlanes, split.cullingPlaneOffset, planeCount, math.normalizesafe(centroid - eye, new float3(0f, 0f, 1f)));
                BuildBasisFromSidePlanes(cullingPlanes, split.cullingPlaneOffset, planeCount, forward, out var right, out var up);

                PeekabooView view;
                if (isPerspective)
                {
                    if (!FitPerspective(vertices, vertexCount, eye, right, up, forward, resolution, out view))
                        continue;
                }
                else if (!FitOrthographic(vertices, vertexCount, right, up, forward, resolution, out view))
                {
                    continue;
                }

                view.splitIndex     = i;
                view.bufferOffset   = texelBase;
                view.mipChainOffset = texelBase;
                view.mipLevels      = math.min(PeekabooDepthBuffer.LevelCount(resolution), PeekabooView.kMaxLevels);
                view.resolutionMaxTexel = new int4(resolution.x - 1, resolution.y - 1, resolution.x - 1, resolution.y - 1);
                for (int level = 0; level < view.mipLevels; level++)
                {
                    var levelResolution      = PeekabooDepthBuffer.LevelResolution(resolution, level);
                    view.levelOffsets[level] = PeekabooDepthBuffer.LevelOffset(resolution, level);
                    view.levelWidths[level]  = levelResolution.x;
                }
                texelBase          += PeekabooDepthBuffer.TotalTexels(resolution);
                views.Add(view);
            }
        }

        /// <summary>
        /// Swaps a rough view direction for the normal of the plane closest to it. For a frustum or a
        /// box, that's the near plane, which gives the exact view direction.
        /// </summary>
        static float3 SnapToNearPlaneNormal(NativeArray<Plane> planes, int offset, int count, float3 direction)
        {
            float  best      = 0.5f;
            float3 candidate = direction;
            for (int i = 0; i < count; i++)
            {
                float3 normal = planes[offset + i].normal;
                float  score  = math.dot(normal, direction);
                if (score > best)
                {
                    best      = score;
                    candidate = normal;
                }
            }
            return math.normalizesafe(candidate, direction);
        }

        /// <summary>
        /// Takes the screen axes from a side plane, so the buffer lines up with the view volume
        /// instead of sitting rotated inside it and wasting resolution on empty corners.
        /// </summary>
        static void BuildBasisFromSidePlanes(NativeArray<Plane> planes, int offset, int count, float3 forward, out float3 right, out float3 up)
        {
            float3 chosen    = default;
            float  bestScore = 0.1f;
            for (int i = 0; i < count; i++)
            {
                float3 normal  = planes[offset + i].normal;
                float3 lateral = normal - forward * math.dot(normal, forward);
                float  score   = math.length(lateral);
                if (score > bestScore)
                {
                    bestScore = score;
                    chosen    = lateral / score;
                }
            }
            if (bestScore <= 0.1f)
                chosen = math.abs(forward.y) > 0.99f ? new float3(1f, 0f, 0f) : math.normalize(math.cross(new float3(0f, 1f, 0f), forward));
            right = chosen;
            up    = math.cross(forward, right);
        }

        /// <summary>
        /// Puts the volume's longer axis on the buffer's longer axis. A side plane tells us the axes,
        /// but not which one is longer, and guessing wrong squeezes a wide view into the buffer's
        /// short side.
        /// </summary>
        static void MatchAspect(ref float3 right, ref float3 up, ref float2 lowerBound, ref float2 upperBound, int2 resolution)
        {
            var extent = upperBound - lowerBound;
            if (extent.x > extent.y == resolution.x > resolution.y)
                return;
            var swappedAxis = right;
            right           = up;
            up              = swappedAxis;
            lowerBound      = lowerBound.yx;
            upperBound      = upperBound.yx;
        }

        /// <summary>
        /// Finds the corners of the convex volume bounded by the planes. Each corner is where three
        /// planes meet, and it has to be inside all the others.
        /// </summary>
        static int IntersectPlanes(NativeArray<Plane> planes, int offset, int count, float3* output, int maxOutput)
        {
            float scale = 0f;
            for (int i = 0; i < count; i++)
                scale = math.max(scale, math.abs(planes[offset + i].distance));
            float tolerance = math.max(1e-3f, scale * 1e-4f);

            int found = 0;
            for (int a = 0; a < count && found < maxOutput; a++)
            {
                var pa = planes[offset + a];
                for (int b = a + 1; b < count && found < maxOutput; b++)
                {
                    var pb = planes[offset + b];
                    for (int c = b + 1; c < count && found < maxOutput; c++)
                    {
                        var pc = planes[offset + c];

                        float3 na  = pa.normal, nb = pb.normal, nc = pc.normal;
                        var    bxc = math.cross(nb, nc);
                        float  det = math.dot(na, bxc);
                        if (math.abs(det) < 1e-6f)
                            continue;

                        // Unity's culling planes are inward facing, so the boundary is dot(n, p) == -distance.
                        var point = (-pa.distance * bxc - pb.distance * math.cross(nc, na) - pc.distance * math.cross(na, nb)) / det;
                        if (!math.all(math.isfinite(point)))
                            continue;

                        bool inside = true;
                        for (int t = 0; t < count; t++)
                        {
                            var pt = planes[offset + t];
                            if (math.dot((float3)pt.normal, point) + pt.distance < -tolerance)
                            {
                                inside = false;
                                break;
                            }
                        }
                        if (inside)
                            output[found++] = point;
                    }
                }
            }
            return found;
        }

        static bool FitPerspective(float3* vertices, int vertexCount, float3 eye, float3 right, float3 up, float3 forward, int2 resolution, out PeekabooView view)
        {
            view = default;

            float nearest  = float.MaxValue;
            float farthest = 0f;
            for (int i = 0; i < vertexCount; i++)
            {
                float z  = math.dot(vertices[i] - eye, forward);
                nearest  = math.min(nearest, z);
                farthest = math.max(farthest, z);
            }
            if (farthest <= 0f)
                return false;
            // A frustum with no near plane has a corner at the eye, which can't be projected. So pull
            // the near distance forward off zero.
            nearest = math.max(nearest, farthest * 1e-5f);

            float2 lowerBound = float.MaxValue;
            float2 upperBound = float.MinValue;
            for (int i = 0; i < vertexCount; i++)
            {
                var   local = vertices[i] - eye;
                float z     = math.max(math.dot(local, forward), nearest);
                var   uv    = new float2(math.dot(local, right), math.dot(local, up)) / z;
                lowerBound  = math.min(lowerBound, uv);
                upperBound  = math.max(upperBound, uv);
            }
            MatchAspect(ref right, ref up, ref lowerBound, ref upperBound, resolution);
            var extent = upperBound - lowerBound;
            if (math.any(extent <= 1e-9f) || !math.all(math.isfinite(extent)))
                return false;

            view = new PeekabooView
            {
                origin        = eye,
                axisRight     = right,
                axisUp        = up,
                axisForward   = forward,
                axisRightScaled = right * (resolution.x / extent.x),
                axisUpScaled    = up * (resolution.y / extent.y),
                basis         = math.transpose(new float3x3(right * (resolution.x / extent.x), up * (resolution.y / extent.y), forward)),
                screenScale   = (float2)resolution / extent,
                screenOffset  = -(float2)resolution * lowerBound / extent,
                resolution    = resolution,
                nearDistance  = nearest,
                farDistance   = farthest,
                isPerspective = true,
            };
            return true;
        }

        static bool FitOrthographic(float3* vertices, int vertexCount, float3 right, float3 up, float3 forward, int2 resolution, out PeekabooView view)
        {
            view = default;

            // The light can be far from the cascade it's culling, so use the volume's centroid as the
            // origin to keep view space coordinates small.
            float3 centroid = 0f;
            for (int i = 0; i < vertexCount; i++)
                centroid += vertices[i];
            centroid /= vertexCount;

            float2 lowerBound = float.MaxValue;
            float2 upperBound = float.MinValue;
            float  nearest    = float.MaxValue;
            float  farthest   = float.MinValue;
            for (int i = 0; i < vertexCount; i++)
            {
                var local  = vertices[i] - centroid;
                var uv     = new float2(math.dot(local, right), math.dot(local, up));
                lowerBound = math.min(lowerBound, uv);
                upperBound = math.max(upperBound, uv);
                nearest    = math.min(nearest, math.dot(local, forward));
                farthest   = math.max(farthest, math.dot(local, forward));
            }
            MatchAspect(ref right, ref up, ref lowerBound, ref upperBound, resolution);
            var extent = upperBound - lowerBound;
            if (math.any(extent <= 1e-9f) || !math.all(math.isfinite(extent)))
                return false;

            view = new PeekabooView
            {
                origin        = centroid,
                axisRight     = right,
                axisUp        = up,
                axisForward   = forward,
                axisRightScaled = right * (resolution.x / extent.x),
                axisUpScaled    = up * (resolution.y / extent.y),
                basis         = math.transpose(new float3x3(right * (resolution.x / extent.x), up * (resolution.y / extent.y), forward)),
                screenScale   = (float2)resolution / extent,
                screenOffset  = -(float2)resolution * lowerBound / extent,
                resolution    = resolution,
                nearDistance  = nearest,
                farDistance   = farthest,
                isPerspective = false,
            };
            return true;
        }
    }
}
