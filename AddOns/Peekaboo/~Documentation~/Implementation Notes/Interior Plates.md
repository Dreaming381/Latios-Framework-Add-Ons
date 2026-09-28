# Interior Plates

A plate has to stay inside the solid, but nothing says it has to touch the
surface. For anything round, that's the key idea.

A sphere has no flat faces, so grouping triangles gives it one tiny plate per
triangle, all too small to keep. But a disc through the middle of the sphere is
completely inside it, and it covers the sphere's whole silhouette. Most of that
disc sits well behind the surface. That's fine. Being behind the surface is
exactly what makes an interior plate safe.

## Which Voxels Count as Inside

The mesh gets voxelized with up to 48 voxels along its longest side. A voxel
counts as **interior** when both of these are true:

-   No triangle passes through it (tested with the separating axis theorem
    against the voxel's box).
-   Its center is inside the mesh.

Together, these are stronger than they look. If no triangle crosses the voxel,
the surface doesn't either, so the whole voxel is on one side of it. The center
test tells you which side. So an interior voxel is *entirely* inside the mesh,
and any polygon covered only by interior voxels is inside too. No distance fields
or fudge factors needed.

To decide whether a voxel center is inside, Peekaboo counts winding along columns
of voxel centers. Each triangle records where it crosses each column and which
way it faces, and adding those up along the column tells you what's inside.
Winding works no matter how many shells the mesh has or how they're nested.

Peekaboo does this sweep along all three axes and only keeps voxels all three
agree on. A single sweep can go wrong when a row of voxel centers lines up
exactly with a flat ring of mesh edges. That's what happens with a ring lying on
its side, since the grid is centered on the bounds and the ring's middle edges
land right on a row. The crossings don't cancel properly, and the hole in the
middle gets marked as solid. A problem like that depends on the sweep direction,
so the other two sweeps catch it. Combining sweeps this way can only shrink the
interior, which is the safe direction.

## Slicing

Peekaboo slices along thirteen axes: a cube's three face normals, its four body
diagonals, and its six edge directions. Treating them as lines rather than
arrows, no view direction is more than 22.5 degrees from one of them. So the
worst a plate ever gets squished by viewing angle is down to 92% of its area.
Each plate's cone is 32 degrees around its axis and points both ways, so together
the thirteen cover every direction with room to spare.

Each axis gets five parallel slices, spread across how far the mesh reaches along
that axis. One goes through the middle of the interior voxels, and the other four
are offset to either side. The offsets are what give a ring good plates. A ring's
middle is in its hole, so a slice through the middle only catches the tube at a
glancing angle. An offset slice cuts through the tube squarely.

Each slice gets gridded at half a voxel per cell. A cell is usable when every
voxel its bounds touch is interior. Since a cell is tested against *all* the
voxels it touches, no cell size can skip one. That's what lets a diagonal slice,
which covers more ground, use bigger cells instead of giving up.

## Cutting Pieces From a Slice

From the usable cells, Peekaboo tries two shapes and keeps the bigger one:

-   **The largest rectangle**, found with the classic largest-rectangle-in-a-histogram
    sweep.
-   **An octagon inside the largest circle.** The circle comes from an exact
    distance transform (Felzenszwalb's separable method), not an approximation,
    since an oversized radius would put the plate outside the region. The octagon
    then gets pulled in by one more cell, to cover the gap between a cell's
    center (where distance is measured) and its corners.

A slice through a wall or a box is roughly rectangular, so a rectangle gets
almost all of it. A slice through anything round is roughly a disc, so the
octagon wins there.

Then the cells that piece used get cleared, and Peekaboo asks again, up to six
pieces per slice. A single piece isn't always enough. Slice a ring across its
middle and you get an annulus, and the biggest disc that fits in an annulus only
spans the width of the wall. Cutting several pieces lets the slice cover the
whole annulus.

## Skipping Slices a Box Doesn't Need

A box's faces already cover every direction, so slicing it too would double the
rasterizer's work for nothing. Before keeping a slice, Peekaboo adds up the
surface plates facing along the slice axis (scaled by how directly they face it),
then halves that total. It halves because a closed mesh has a front face and a back
face for every direction, and only one of them is the silhouette. If that already
beats the slice, the slice gets dropped.

So a cube bakes to exactly six plates. A cylinder keeps its two caps as surface
plates and gets slices for the directions the caps don't cover.

## Requirements

The slice pass needs a closed mesh, since "inside" doesn't mean anything
otherwise. Every edge has to be shared by exactly two triangles once coincident
vertices are welded.

Degenerate triangles get dropped instead of counting against the mesh. A UV
sphere with a pole vertex has a whole ring of triangles with two corners in the
same spot. They have no area, so ignoring them leaves the mesh closed. Counting
them as holes would reject the most obvious shape this pass is for.
