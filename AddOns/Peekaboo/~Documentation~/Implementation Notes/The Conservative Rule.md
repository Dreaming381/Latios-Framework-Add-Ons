# The Conservative Rule

**Peekaboo may only cull a renderer when nothing it draws could reach the
screen.** Everything else in the add-on is up for negotiation. This isn't.

The rule is one-sided, and that's what keeps the design manageable. Every
approximation Peekaboo makes is allowed to be wrong in only one direction. It can
miss that something is hidden, which costs some performance. It can never decide
that something visible is hidden, which would be a bug. Since each step is
one-sided on its own, all of them together are one-sided too. No step has to
worry about what the others did.

Four places enforce the rule.

## 1. A Plate Sits Inside the Solid, and Is Only Used Where It's Valid

A plate stands in for an occluder's geometry. Any ray that reaches a plate has
already passed through the real surface, so the real surface is at or in front of
the plate. That means a plate's depth can only ever understate how much is
hidden.

That argument only works if the ray actually crossed the surface on its way in.
If the camera is on the wrong side of the surface, it didn't. So every plate also
carries the volume it's valid from, and Peekaboo skips the plate for any view
where that doesn't hold. [Plate Validity](Plate%20Validity.md) has the full rules.
The rest of this section is about how a plate ends up inside the solid in the
first place.

Surface plates lie right on the surface, which counts as inside. Interior plates
are built only from voxels that no triangle touches and whose centers are inside
the mesh. That makes the whole voxel inside, and so any plate covered by those
voxels is inside too.

There's one approximation here. A surface plate gets snapped to the plane that
best fits its patch of triangles. Triangles count as coplanar if they're within
1e-5 of each other (relative to the mesh size). The fitted plane then gets pushed
back behind every vertex of the patch, and a patch that isn't flat to within 1e-4
gets dropped instead of approximated. In practice almost every patch is perfectly
flat, so the plate doesn't move at all.

## 2. A Pixel Only Counts When It's Fully Covered

A pixel in the depth buffer only gets a depth when plates cover all of it. Just
overlapping it isn't enough.

This is what makes the buffer usable at all. Peekaboo doesn't know the resolution
of the real render target, and for shadow maps it can't know. But if a plate fully
covers one of Peekaboo's pixels, then everything in that pixel's slice of space is
behind the plate. That stays true no matter how finely the real target samples
it.

Partial coverage wouldn't work. One half-covered pixel at 256 wide turns into a
hundred half-covered pixels at 4K, and a sliver of geometry could show through any
of them.

## 3. A Pixel Takes Its Farthest Depth

Inside one pixel, a tilted plate covers a range of depths. The pixel stores the
*farthest* of them, so anything behind that point is behind the plate across the
whole pixel. Storing the nearest or the middle would let something peek out along
the near edge.

Depth varies linearly across a plane in screen space, so the farthest point in a
pixel is always at one of its corners. The sign of the depth slope tells you
which one. See [Rasterizing](Rasterizing.md).

## 4. A Renderer Is Tested by Its Nearest Corner

A renderer only gets culled when every pixel its bounds could touch holds a depth
closer than the *nearest* corner of those bounds. The bounds are bigger than the
real geometry, the nearest corner is closer than any real point, and the pixel
rectangle gets rounded outward. All three push in the same safe direction.

The depth pyramid is built with min, so a coarse texel only claims what every
finer texel under it claims. Reading a coarser level than needed can only lower
the result, which just loses a cull.

## What the Rule Doesn't Cover

The rule is about geometry. A few other things can break it:

-   **Materials that aren't opaque.** A cutout or transparent material has holes
    the mesh doesn't have. Plates on it would hide things you can see right
    through it. Peekaboo checks this per submesh at runtime, and only uses a
    plate while its submesh is drawn opaque. See
    [Plate Validity](Plate%20Validity.md).
-   **Geometry that moves after baking.** A skinned, blend-shaped, or
    shader-displaced mesh doesn't match the plates cut from its rest pose. Those
    renderers never occlude.
-   **A different mesh LOD.** Plates come from mesh LOD 0, so a renderer drawing
    a coarser LOD never occludes.

There's also one deliberate exception, described in
[Rasterizing](Rasterizing.md). When two plates share an edge, Peekaboo compares
their edges with a tolerance of a thousandth of a pixel. The two plates reach
that edge through different math and disagree in the last few bits. Without the
tolerance, every shared edge would look like a gap.
