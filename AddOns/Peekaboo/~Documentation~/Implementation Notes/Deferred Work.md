# Deferred Work

Things left for later, roughly in order of how much they'd pay off.

## Temporal Reprojection

Reprojecting last frame's depth buffer into this frame's view gives you occluders
for free before anything gets rasterized. It's how most shipping CPU occlusion
systems get their coverage up.

It's left out on purpose for now. Temporal state makes the output depend on
history, so a wrong cull can only be reproduced by replaying the history that
caused it. The right time to add it is after the non-temporal path has been tested
on real content.

## Welding Between Separate Objects

Welding joins plates that meet along a shared edge, which handles seams within one
mesh. But two separate objects that just touch, like floor tiles or modular wall
pieces, aren't welded. So a strip of pixels along every join goes unclaimed, and
that's one of the most common ways big occluders get built.

The welding rule itself would work fine. The proof that two plates have no gap is
about their edges, not which mesh they came from. What's missing is comparing
plates from different entities, since they're gathered per candidate. The row
binning already gathers spans per row across all candidates, so the pieces are
mostly there.

## Shadow Cascade Resolution

Every cascade gets the same resolution. The near cascade covers a small volume at
high precision, and the far one covers a huge volume at low precision. So the far
cascades cull the least, even though they usually have the most entities. Sizing
each cascade's buffer by its volume would put the memory where it helps.

## Precomputed Edge Chains

Most of the rasterizer's own cost is per plate per row: finding where the plate's
edges cross that row, by scanning every edge with a divide per crossing.
Precomputing the left and right edge chains once per plate, each an ordered list of
`(yStart, yEnd, x, dx/dy)`, would turn that into a binary search over at most eight
entries and one multiply-add.

This still works with rows rasterized in parallel, which a classic active edge
table wouldn't, because a chain gets indexed rather than advanced. The chains could
live in the same slots the screen-space vertices use now.

## Better Plates for Rings and Tubes

Rings get the weakest plates of any shape, covering about half their silhouette
from a random direction. The slices are axis-aligned, so they only cut a tube
squarely where one of the thirteen axes happens to line up with it. Slicing along
directions fitted to the mesh, instead of fixed axes, would help. Raising the plate
budget also helps a bit, but it levels off quickly past 64.

## Occludees Straddling the Near Plane

An occludee whose bounds cross the near plane is never culled, since its corners
can't all be projected. Those are big things right in front of the camera, so
they're rarely hidden anyway. But clipping the bounds to the near plane before
projecting would let Peekaboo test them.

Occluders in the same position are already handled. They get scored as covering
the whole view if the eye is inside their bounds, and each plate's validity check
decides what actually gets rasterized.
