# Plates

A **plate** is a flat convex polygon in mesh-local space. It sits inside the
solid the mesh encloses, and it's the only thing the rasterizer knows how to
draw. A mesh's plates are both its occluder and its simplified shape.

```csharp
struct OccluderPlate
{
    PeekabooRegionKind regionKind;  // Which viewpoints this plate is valid from
    float4 regionA, regionB;        // The shape of that region
    float3 planeNormal;             // Unit length, mesh-local
    float  planeDistance;           // dot(planeNormal, p) == planeDistance on the plate
    float3 coneAxis;                // The view directions worth rasterizing this plate for
    float  coneCosHalfAngle;
    bool   doubleSided;             // Whether the cone points both ways
    float  area;                    // Used for ranking
    int    vertexStart;             // Index into the blob's shared vertex array
    int    vertexCount;
}
```

## Why Not Triangles?

A pixel only counts as covered when something covers all of it. If you split a
quad into two triangles, you put a diagonal seam through the middle of a flat
face. Every pixel along that diagonal is only partly covered by each half, so
neither half claims it. A wall made of triangle pairs would leak a line of
unclaimed pixels down every quad.

The fix is to never split the face in the first place. A flat face is one plate.
The welding rule in [Rasterizing](Rasterizing.md) still exists, but it's for
seams between genuinely different faces, like two sides of a box. It isn't
there to patch up seams Peekaboo made itself.

Plates are also way fewer than triangles. A wall of two hundred triangles is one
plate, and a box is six. So the simplification comes for free.

## The Validity Region

Each plate knows which viewpoints it's safe to use from. Only a plate whose
region contains the camera's view gets rasterized. This is what keeps a plate
from being used when the camera has moved past the surface it stands for, like
when the camera clips into a wall. See [Plate Validity](Plate%20Validity.md) for
the rules and how each kind of plate gets its region.

## The Cone

Each plate also carries a cone of view directions it's *worth* rasterizing for,
stored as an axis and the cosine of a half angle. A plate gets skipped when the
direction from the plate to the viewer falls outside the cone.

**The cone only affects performance.** Correctness comes from the validity
region. So a bug in the cone test can cost you some culling, but it can't make
something visible disappear.

For a surface plate, the cone is the outward hemisphere, so the test works like
backface culling. For an interior plate, the cone is narrow and centered on the
slice axis. It's marked `doubleSided`, since a slice through a solid works from
either side. See [Interior Plates](Interior%20Plates.md).

## Two Ways to Cut a Plate

**Off the surface.** Triangles that lie in the same plane get merged into a flat
patch, and a convex polygon is cut from it. The plate lies right on the surface,
which is as tight as it gets. This is where boxes, walls, floors, and most
buildings get their plates. See [Surface Plates](Surface%20Plates.md).

**Through the inside.** The mesh gets voxelized and sliced, and a polygon is cut
from the part of each slice that's definitely inside. Most of such a plate sits
well behind the surface, and that's fine. It's the only way anything round gets
an occluder. See [Interior Plates](Interior%20Plates.md).

Both run on every mesh that qualifies. The interior pass skips a slice when the
surface plates already cover that direction.

## The Budget

A mesh keeps at most `kMaxPlatesPerMesh` (64) plates. Any plate smaller than 1%
of the mesh's biggest bounds cross-section gets dropped. The floor matters more
than the cap. A sphere with five hundred triangles produces five hundred tiny flat
patches, and each one would be a valid plate that isn't worth drawing.

When a mesh has more plates than the budget, Peekaboo doesn't just keep the
biggest ones. It groups plates by which way they face and which side of the mesh
they're on, then deals out the budget one group at a time. Every direction gets
its best plate before any direction gets a second one.

This matters for hollow meshes. The outside of a shell always has bigger patches
than the inside, because it has the bigger radius. Keeping only the biggest plates
would throw away every inward-facing plate, and a camera standing inside a dome
would get no occlusion at all.

`largestPlateArea` and `boundsCrossSectionArea` are stored per mesh. At runtime,
Peekaboo uses their ratio to estimate how much of its own silhouette a mesh can
fill, which helps rank occluders against each other. See [Culling](Culling.md).
