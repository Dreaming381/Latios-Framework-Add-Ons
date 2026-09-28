# Surface Plates

Surface plates are cut from the flat faces of a mesh. This is where boxes, walls,
floors, and most buildings get their occluders. They're as tight as a plate can
be, since each one lies right on the surface it stands for.

## Grouping Triangles

First, vertices within 1e-5 of the mesh size get welded together, so triangles
meeting along a seam are known to share an edge. Then triangles that share an
edge and lie in the same plane get merged into one patch. "Same plane" means
normals within `kCoplanarCosine` (0.99999) and offsets within 1e-5 of the mesh
size.

The shared edge requirement matters just as much as the plane check. Two walls on
opposite sides of a building might lie in the same plane, but they shouldn't
become one patch.

The tolerance is strict on purpose. The plate gets snapped onto a plane fitted to
the patch, and any slack there lets the plate drift off the real surface. Geometry
that really is flat, which is nearly all of it, matches exactly.

## Fitting and Flatness

Peekaboo fits a plane to the patch, weighted by triangle area, and then measures
how far any vertex strays from it. If that's more than 1e-4 of the mesh size, the
patch is **dropped**. A patch that isn't flat can't be turned into a plane without
moving the plate somewhere the geometry isn't.

Otherwise, the plane gets pushed back behind every vertex by the amount measured,
so the plate can't poke out the front.

## The Outline

A patch's outline is every edge that belongs to exactly one of its triangles. That
includes the edges around any holes.

**The easy case (which is most of them).** If those edges form one closed loop
that's convex, with at most eight vertices after removing collinear ones, that
loop becomes the plate. No grid and no approximation. Box faces, walls, floor
tiles, and n-gon faces all land here.

A patch with a hole in it has two loops, so it can never take this path. That's
what stops a wall with a window from getting a plate that covers the window.

**The fallback.** An L-shaped wall, or a wall with a window cut out of it, doesn't
have a single convex outline. Those go to a grid.

## The Grid Fallback

The patch gets projected onto its plane. The grid's axes line up with the patch's
main direction, found with a 2x2 covariance fit around the middle of the outline.
That way, rectangles follow the shape instead of cutting across it at an angle.

A cell is **blocked** if any outline edge passes through it or through one of its
eight neighbors. Then a scanline fill finds which cells are inside the outline. A
cell that's inside and not blocked is definitely inside the patch, because the
outline doesn't come anywhere near it. The one-cell border is what makes that true
no matter where the outline falls between cells. It's also why a hole smaller than
a cell still blocks that cell.

Up to four overlapping rectangles get cut out. Each one is the largest rectangle
left in the grid, then grown back out as far as the full grid allows. That growing
is where the overlap comes from, and it means a pixel on the seam between two
rectangles is fully covered by at least one of them.

## Skipping Tiny Patches

A patch smaller than the minimum plate size gets skipped before any of the work
above, since no plate cut from it could be bigger than the patch itself. On a
detailed mesh, almost every patch is a single tiny triangle, and this check keeps
the baker from extracting outlines half a million times.
