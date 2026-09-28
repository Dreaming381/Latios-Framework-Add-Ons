# Validation

Peekaboo gets tested a few different ways, and each catches things the others
can't. The tests live in the workspace project under
`Assets/Validation/Peekaboo/` and `Assets/Tests/`, not in the package.

The validation code compiles *into* the add-on's assembly through an `.asmref`,
so it drives the real view builder, rasterizer, and depth test. A test that
reimplemented any of those would just be testing the reimplementation.

## Ray Casting

`PeekabooOracle` casts a dense grid of rays through a target's screen footprint
and reports whether any of them reach it without hitting an occluder first. It
shares nothing with the rasterizer except the scene.

The randomized tests build scenes of boxes and targets, both perspective and
orthographic. Half the targets are placed right behind an occluder, since the
line between culled and kept is the only place a mistake can show up. Every
target the rasterizer culls gets checked, and if any ray reaches a culled target,
the test fails.

**The tests also check that some things got culled and some were visible.**
Without that, a system that culls nothing would pass every correctness check, and
an oracle that sees nothing would never complain.

## Image Comparison

`PeekabooRenderComparison` renders the scene twice from the same pose, once with
occlusion culling and once without, and compares the images. Culling may only
remove renderers that contribute nothing, so the two images have to match exactly.

This is the only test that catches **a shadow caster culled by mistake**. That
leaves a hole in a shadow somewhere else on screen, and no counter would show it.
It's also the only test that runs the whole pipeline: baking, blob serialization,
the real culling context, the cascade masks, and Kinemation's draw command
generation.

**It can't tell correct culling from no culling.** If culling does nothing, the
two images still match. So a passing image comparison only proves nothing visible
was culled. Always check the stats alongside it to make sure culling actually
happened.

## Coverage

`PeekabooCoverageProbe` measures how much of a mesh its plates actually stand in
for. It views the mesh from a spread of directions, finds the real silhouette by
casting a ray per texel at the mesh's own triangles, rasterizes the plates into
the same view, and compares.

-   **Coverage** is the fraction of the silhouette the plates claim. Higher is
    better.
-   **Overclaim** is anything the plates claim where the mesh isn't. That would be
    a false occlusion, so it must always be zero.

This fills a gap the other tests leave. A baker that cut no plates at all would
pass every correctness test. `PeekabooPlateCoverageTests` checks both columns
across the shapes in `PeekabooShapeLibrary`, with a coverage floor per shape.
The floors sit a bit below what's currently measured, so small drift won't fail
the suite but a real regression will.

When you write a new baking test, check coverage too, not just where plates
aren't allowed to be.

## Plate Validity

`PeekabooFootprintTests`, `PeekabooRegionBakingTests`, and `PeekabooRule4Tests`
cover the pieces of the validity rules: the footprint a plate leaves on the near
plane, the region each kind of plate gets baked with, and the check between them.
`PeekabooInsideOccluderTests` runs all three together with the camera inside an
occluder's bounds. That's where the tricky cases live: a room that should occlude
from inside, and solids the camera has clipped into that shouldn't occlude at all.

## Direct Geometry Checks

These are cheap checks on what the baker produces. When something breaks, these
pin down where much better than the other tests.

-   A cube bakes to exactly six plates, each the right area.
-   A subdivided flat quad with 128 triangles bakes to exactly one plate.
-   A sphere's plates all stay within its radius.
-   A sphere hides a target directly behind it from 24 random directions.
-   A wall with a window never gets a plate over the window, even when the window
    is smaller than a grid cell.

## Stress Scenes

**`PeekabooStressScene`** is a city of 1,809 box-shaped towers and four million
small drifting cubes to cull.

**`PeekabooShapesScene`** is 728 occluders of eight different shapes, including
rings stood on edge that the camera flies through, over 1.2 million cubes. It's
there because a box is the one shape every part of the baker handles perfectly.
Watch for anything that disappears while visible through a ring's hole or a
wall's window.

`PeekabooStressBenchmark` parks the camera at fixed poses, lets the scene settle,
and reports the median frame time and counts over 120 frames. Two things to know
if you measure these scenes yourself:

-   Calling `Camera.Render()` manually adds passes to a frame whose stats have
    already reset, so reading stats afterward gives inflated totals.
-   The first frames after entering play mode aren't reproducible, since the swarm
    is still spawning. Let the scene settle before sampling.

## What's Still Not Covered

**Real production content.** Imported architecture, alpha-clipped foliage, and LOD
groups would exercise the baker's rules much harder than any of this does.

**Non-uniform and mirrored transforms.** The rasterizer handles them (Newell's
method for the plane, a determinant check for winding), but no test uses one.
