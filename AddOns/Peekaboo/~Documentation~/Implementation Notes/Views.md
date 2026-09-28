# Views

A **view** is one software depth buffer plus the projection that fills it. A
camera pass has one view. A shadow pass has one view per cascade.

## Why Peekaboo Makes Its Own Projection

`BatchCullingContext` doesn't give you a projection matrix. It gives you planes.
For a camera, you could rebuild the real projection from the camera component.
But for a shadow map, there's no way to do that. Unity doesn't expose the shadow
map's resolution anywhere, and the cascade's projection is internal.

So Peekaboo doesn't try to match it. It intersects each cascade's culling planes
into the convex volume they enclose, then fits its own projection to exactly that
volume at its own resolution. Anything the cascade renders is inside that volume,
so the fitted projection covers everything that matters and doesn't waste any
resolution.

This only works because Peekaboo never claims a pixel unless it's fully covered.
If a pixel could be claimed just because a sample point landed on a triangle, a
buffer with a different resolution from the real target would be meaningless. But
when a claimed pixel means its whole slice of space is covered, the resolution
doesn't matter. See [The Conservative Rule](The%20Conservative%20Rule.md).

To find the volume's corners, Peekaboo solves every group of three planes and
keeps the points that satisfy all the other planes. There are fewer than ten
planes per cascade, so that's a few hundred small solves on the main thread. If
the volume ends up with fewer than four corners, it's unbounded or degenerate, and
the view gets dropped. That turns off culling for that cascade instead of fitting
a projection to nothing.

## What You Can Trust in the Culling Context

Two fields in the culling context are easy to get wrong.

**`localToWorldMatrix` is the identity for camera passes.** It only holds a real
transform for light passes, where it's the light's orientation. Kinemation's own
frustum culling uses it for exactly that. It's easy to assume it's the camera's
transform, but it isn't.

**`lodParameters.cameraPosition` is the only place a camera pass stores the
eye.** If you miss this, you'll build a view around the world origin instead. And
if your test scene happens to be centered on the origin, the results will look
right while being wrong, so test with the camera somewhere else.

So the view's axes get picked per pass:

-   **Light:** Forward is `localToWorldMatrix.c2`, the light direction. It has to
    be, because shadow occlusion happens along the light's axis. Guessing it from
    the geometry could pick the wrong box axis and describe a different viewpoint
    entirely.
-   **Camera:** Forward starts as the direction from
    `lodParameters.cameraPosition` to the middle of the volume, then snaps to
    whichever plane normal points most the same way. For a frustum or a box,
    that's the near plane, so you get the exact view direction.

The screen axes come from one of the side planes, flattened perpendicular to
forward. That way the pixels line up with the volume instead of sitting rotated
inside it and wasting resolution on empty corners. A side plane tells you the
volume's axes but not which is which, so Peekaboo matches the volume's longer axis
to the buffer's longer axis.

## Closeness

Depth is stored as **closeness**, which gets bigger the closer something is to
the viewer. That way one comparison works for both kinds of projection:

-   Perspective: `1 / viewZ`
-   Orthographic: `-viewZ`

Both change linearly across a plane in screen space. That's what lets a plate's
depth over a whole row of pixels become a single line. See
[Rasterizing](Rasterizing.md).

This also means an empty pixel holds negative infinity. That's the right starting
value for the max the rasterizer takes, and it doubles as a free "nothing wrote
here" marker, which the welding pass relies on.
