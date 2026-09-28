using Unity.Mathematics;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Rule 4 of plate validity: a plate is a candidate for a view only when every point on the near
    /// plane that a ray can reach it from is inside the plate's validity region.
    /// </summary>
    /// <remarks>
    /// This is all of the correctness at rasterization time. Baking proves rule 5 for a region, which
    /// says a ray from inside it that reaches the plate has already hit an outward-facing triangle no
    /// farther away. This checks that the viewer is somewhere that proof applies. A viewer outside
    /// every region rasterizes nothing, which is why a mesh the camera walks into just stops occluding
    /// instead of occluding wrongly.
    ///
    /// The test runs in the occluder's local space, since that's where regions are baked. A half space
    /// would transform to world space fine, but a cone loses its angle under non-uniform scale and a box
    /// stops being axis aligned under rotation. So the footprint gets moved into the region's space
    /// instead.
    /// </remarks>
    internal static unsafe class PeekabooPlateRegion
    {
        /// <summary>
        /// Whether all of a footprint lies inside a plate's validity region.
        /// </summary>
        /// <param name="localFootprint">The footprint in the occluder's local space</param>
        public static bool Contains(PeekabooRegionKind kind, float4 regionA, float4 regionB, in OccluderHull hull,
                                    float3* localFootprint, int count)
        {
            if (count < 1)
                return false;

            switch (kind)
            {
                case PeekabooRegionKind.HalfSpace:
                {
                    // The region is convex, so the footprint is inside whenever its vertices are.
                    for (int i = 0; i < count; i++)
                    {
                        if (math.dot(regionA.xyz, localFootprint[i]) <= regionA.w)
                            return false;
                    }
                    return true;
                }
                case PeekabooRegionKind.Cone:
                    // Also convex for any half angle up to 90 degrees, which is all the builder accepts,
                    // so checking vertices is enough here too.
                    return IsInsideCone(regionB.xyz, regionA.xyz, regionA.w, localFootprint, count);
                case PeekabooRegionKind.DoubleCone:
                    // The two cones together aren't convex, so the whole footprint has to be in one.
                    return IsInsideCone(regionB.xyz, regionA.xyz, regionA.w, localFootprint, count) ||
                           IsInsideCone(regionB.xyz - regionA.xyz * regionB.w, -regionA.xyz, regionA.w, localFootprint, count);
                case PeekabooRegionKind.MeshHullExterior:
                {
                    // The outside of a hull isn't convex. Testing each vertex on its own would accept a
                    // footprint with its ends on either side of the hull and its middle passing through.
                    // Instead, find one plane the whole footprint is beyond. The half space outside a
                    // hull plane is convex and entirely outside the hull.
                    bool3 axisBelow     = true, axisAbove     = true;
                    bool4 diagonalBelow = true, diagonalAbove = true;
                    bool3 edgeBelowA    = true, edgeAboveA    = true;
                    bool3 edgeBelowB    = true, edgeAboveB    = true;
                    for (int i = 0; i < count; i++)
                    {
                        var point = localFootprint[i];
                        OccluderHull.Project(point, out var diagonal, out var edgeA, out var edgeB);
                        axisBelow     &= point < hull.axisMin;
                        axisAbove     &= point > hull.axisMax;
                        diagonalBelow &= diagonal < hull.diagonalMin;
                        diagonalAbove &= diagonal > hull.diagonalMax;
                        edgeBelowA    &= edgeA < hull.edgeMinA;
                        edgeAboveA    &= edgeA > hull.edgeMaxA;
                        edgeBelowB    &= edgeB < hull.edgeMinB;
                        edgeAboveB    &= edgeB > hull.edgeMaxB;
                    }
                    return math.any(axisBelow | axisAbove) || math.any(diagonalBelow | diagonalAbove) ||
                           math.any(edgeBelowA | edgeAboveA) || math.any(edgeBelowB | edgeAboveB);
                }
                default:
                    // None, or anything a future baker leaves unset. Never a candidate, so a gap in
                    // baking only costs culling.
                    return false;
            }
        }

        static bool IsInsideCone(float3 apex, float3 axis, float cosHalfAngle, float3* localFootprint, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var toPoint = localFootprint[i] - apex;
                var length  = math.length(toPoint);
                if (!(length > 0f))
                    return false;
                if (math.dot(toPoint / length, axis) <= cosHalfAngle)
                    return false;
            }
            return true;
        }
    }
}
