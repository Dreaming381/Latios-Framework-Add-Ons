using Unity.Mathematics;

namespace Latios.Peekaboo
{
    /// <summary>
    /// One software depth buffer and the projection that fills it. A camera pass has one view, and a
    /// shadow pass has one view per culling split.
    /// </summary>
    /// <remarks>
    /// BatchCullingContext never tells us the render target's size, and for shadow maps there's no way
    /// to look it up. So a view doesn't try to match the real target. It builds its own projection
    /// that covers exactly the split's culling volume, at its own resolution. That's safe because a
    /// pixel only stores a depth when occluder geometry covers the whole pixel. That's a fact about
    /// world space, not about any particular resolution.
    /// </remarks>
    internal unsafe struct PeekabooView
    {
        public const int kMaxLevels = 16;

        // Precomputed because the query runs once per entity per view, and in a big scene a loop to
        // find the level offset becomes most of the cost.
        public fixed int levelOffsets[kMaxLevels];
        public fixed int levelWidths[kMaxLevels];

        public float3 origin;
        public float3 axisRight;
        public float3 axisUp;
        public float3 axisForward;

        // The lateral axes with screenScale already multiplied in, which saves four multiplies per
        // entity per view when projecting bounds.
        public float3 axisRightScaled;
        public float3 axisUpScaled;

        // The same three axes as one matrix, so moving a bounding box into view space is three
        // broadcasts and two multiply-adds with no horizontal sums.
        public float3x3 basis;

        // Perspective: screen = k * (cameraSpace.xy / cameraSpace.z) + o
        // Orthographic: screen = k * cameraSpace.xy + o
        public float2 screenScale;
        public float2 screenOffset;

        public int2 resolution;

        // The clamp bound for a whole buffer query, precomputed since it's on the per entity per view
        // path.
        public int4 resolutionMaxTexel;

        public int  bufferOffset;
        public int  mipChainOffset;
        public int  mipLevels;
        public int  splitIndex;

        public float nearDistance;
        public float farDistance;
        public bool  isPerspective;

        public int pixelCount => resolution.x * resolution.y;

        public float3 ToViewSpace(float3 worldPoint)
        {
            var p = worldPoint - origin;
            return new float3(math.dot(p, axisRight), math.dot(p, axisUp), math.dot(p, axisForward));
        }

        /// <summary>
        /// The inverse of <see cref="ToViewSpace"/>.
        /// </summary>
        public float3 FromViewSpace(float3 viewPoint)
        {
            return origin + axisRight * viewPoint.x + axisUp * viewPoint.y + axisForward * viewPoint.z;
        }

        /// <summary>
        /// Projects a view-space point. Returns false if the point is closer than the near distance,
        /// where a perspective projection doesn't mean anything.
        /// </summary>
        public bool ProjectViewSpace(float3 viewPoint, out float2 screen, out float closeness)
        {
            if (isPerspective)
            {
                if (viewPoint.z < nearDistance)
                {
                    screen    = default;
                    closeness = default;
                    return false;
                }
                screen    = screenScale * (viewPoint.xy / viewPoint.z) + screenOffset;
                closeness = 1f / viewPoint.z;
                return true;
            }
            screen    = screenScale * viewPoint.xy + screenOffset;
            closeness = -viewPoint.z;
            return true;
        }

        public bool Project(float3 worldPoint, out float2 screen, out float closeness) => ProjectViewSpace(ToViewSpace(worldPoint), out screen, out closeness);

        /// <summary>
        /// Turns the closeness of a world-space plane into a linear function of screen coordinates.
        /// Closeness is 1/z for perspective and -z for orthographic. Both are linear in screen space
        /// across a plane, which is what lets the rasterizer step a plate's depth along each scanline.
        /// </summary>
        /// <param name="planeNormal">The unit normal of the world-space plane</param>
        /// <param name="planeDistance">The plane satisfies dot(planeNormal, p) == planeDistance</param>
        /// <param name="gradient">The x and y coefficients of the resulting linear function</param>
        /// <param name="constant">The constant term of the resulting linear function</param>
        /// <returns>False if the plane is degenerate for this view and must not be rasterized</returns>
        public bool PlaneToLinearCloseness(float3 planeNormal, float planeDistance, out float2 gradient, out float constant)
        {
            var n = new float3(math.dot(planeNormal, axisRight), math.dot(planeNormal, axisUp), math.dot(planeNormal, axisForward));
            var c = planeDistance - math.dot(planeNormal, origin);

            // The divisor is zero when the plane is edge-on to this view. That's when it passes
            // through the eye for perspective, or runs parallel to the view direction for
            // orthographic. Either way, the plate covers no pixels and gets dropped.
            float divisor = math.select(n.z, c, isPerspective);
            if (math.abs(divisor) < 1e-20f)
            {
                gradient = default;
                constant = default;
                return false;
            }

            float2 scaled = n.xy / (screenScale * divisor);
            gradient      = scaled;
            if (isPerspective)
                constant = n.z / divisor - math.dot(scaled, screenOffset);
            else
                constant = -c / divisor - math.dot(scaled, screenOffset);
            return math.all(math.isfinite(gradient)) && math.isfinite(constant);
        }

        /// <summary>
        /// The smallest closeness of a plate over a whole pixel, which is the value the pixel can
        /// safely store. A linear function hits its extremes at the pixel's corners.
        /// </summary>
        public static float MinClosenessOverPixel(float2 gradient, float constant, int x, int y)
        {
            float2 corner = new float2(math.select(x, x + 1, gradient.x < 0f), math.select(y, y + 1, gradient.y < 0f));
            return math.dot(gradient, corner) + constant;
        }
    }
}
