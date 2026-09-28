# Plate Validity

These are the rules that decide when a plate is safe to rasterize. The baker and
the runtime are both built against them.

The short version: a plate isn't just safe or unsafe. It's safe *from some
viewpoints*. Each plate carries the volume it's safe from, and Peekaboo only uses
it when the camera is somewhere in that volume.

## The Rules

1.  A plate is a 2D polygon in 3D space **and an open volume in 3D space that is
    its validity region**.
2.  The **camera near rectangle** is the rectangle formed by clipping the
    camera's near plane with all other camera planes except the far plane.
3.  An **outward-facing triangle** is a triangle of the mesh renderable by an
    incident eye ray. For a typical mesh that is the side the normal points
    toward. A double-sided rendered triangle is outward-facing on both sides.
4.  A plate is a **candidate** when the entirety of the near rectangle is inside
    its validity region.
5.  **Any ray starting at any point inside the validity region that is able to
    hit the plate must also hit an outward-facing triangle at a distance at or
    nearer than the distance at which it hit the plate.**

The validity region can be a half space, a cone, a cone with a sphere or plane
clipped off its tip, or any other shape that's cheap to test.

Rule 5 is what the baker has to guarantee. Rule 4 is what the runtime checks.
Together, they mean every ray from the camera that reaches a plate has already
passed a *rendered* outward-facing triangle that's no farther away. So the plate's
depth can only understate what's hidden.

The rules test the near rectangle instead of the eye on purpose. Anything between
the eye and the near plane doesn't get rendered, so reasoning from the eye proves
nothing about what's on screen. Reasoning from the near rectangle does, since
everything a ray reaches from there is past the near plane.

**All of the correctness lives in rule 4, checked per plate right before
rasterizing.** Choosing which occluders to rasterize is just a ranking heuristic
and doesn't guarantee anything. Keep it that way. Don't move the guarantee back
into occluder selection.

## Amendment A: Rule 4 Only Checks the Plate's Footprint

*You can revert this by testing the whole near rectangle instead. That's strictly
more cautious and only costs culling.*

Rule 4 only needs **the points of the near rectangle a ray can reach the plate
from**. That's the plate's footprint on the near plane, clipped to the near
rectangle.

This is safe because rule 5 only talks about rays that hit the plate. A point on
the near rectangle that can't reach the plate never triggers rule 5, so there's
nothing to check there.

Without this amendment, rule 4 doesn't work for orthographic views. A
perspective near rectangle is small, so requiring all of it costs little. But an
orthographic cascade's near rectangle is its entire cross section, often hundreds
of meters across. Under a sun at 45 degrees, a roof plate's near rectangle can
span seventy meters of height, which crosses the roof's own half space, so the
plate gets rejected. Roofs under a slanted sun cast most of the shadows, so
without the amendment most shadow occlusion would disappear. With it, the roof's
footprint sits entirely above the roof, toward the light, and passes.

It also tightens the perspective case, since a distant plate's footprint on the
near plane is tiny.

Orthographic footprints aren't clipped to the near plane. An orthographic view's
near distance is the front of the volume Peekaboo fitted, not a plane the shadow
map clips against. Unity positions the shadow camera so that everything along the
view axis renders. So casters between the light and the cascade still count, and
their rays cross the near plane at the same spot either way.

## Amendment B: Baking Doesn't Look at Materials

*You can revert this by skipping plates for submeshes whose materials aren't
opaque, like the baker did originally.*

Plates get cut for every submesh, no matter what material it's drawn with,
including alpha-tested and transparent ones. Each plate records enough to be
filtered later:

-   Which mesh it belongs to
-   Which submesh it depends on (see below)
-   Which mesh LOD level it came from, which is always 0 for now

Then the runtime filters by the materials actually in use. That keeps the blob
independent of any particular mesh and material pairing, so the same plates work
no matter what a renderer is drawn with.

Skipping plates that no material could ever use would be a nice optimization
later, but it isn't needed for correctness.

## What Rule 3 Requires of a Material

Rule 3 says "renderable". The intended meaning is stronger, and it's what the rest
of this page assumes: a triangle only counts if it **fully hides what's behind
it**. That means opaque, depth-writing, and no per-fragment discard. Just being
drawn isn't enough. A blended triangle renders but hides nothing, and an
alpha-tested one might discard the exact fragment you're relying on.

So the runtime filter is: a plate is usable only while every submesh that could
satisfy its rule 5 is being drawn opaquely.

## What Each Kind of Plate Gets

**Surface plates** lie in the flat patch they were cut from. So a ray that
reaches the plate reaches the triangle at the same point, and rule 5 holds with
the distances equal. Their region is the open half space on the outward side.
(Both sides would work for a double-sided material, but see What's Built.) Only
their own submesh matters for rule 3, which is the benefit of tracking submeshes.

A surface plate also has to lie within the **triangles it was cut from**, not
just in their plane. Otherwise, a patch with a hole could produce a plate that
covers the hole. The baker already guarantees this. A patch with a hole has two
outline loops, so it can't become a single plate directly, and the grid fallback
blocks every cell an outline edge touches.

**Interior plates** are cut from the middle of the solid, and any submesh could be
the first surface a ray crosses on its way in. Their region is everything outside
a conservative outer bound of the mesh. Outside that bound means outside the solid,
so a ray to a point inside has to cross the surface, and the first crossing is an
entry, which is outward-facing.

That bound is an `OccluderHull`: the slab between two planes along each of the 13
directions the slicer uses. It's built from LOD 0's vertices, so it contains the
convex hull of the mesh, which contains the solid. The test is the same as for a
box. The whole footprint has to be beyond one of the 26 planes. Compared to the
bounding box, the diagonal planes cut off the empty corners around round meshes,
so a camera standing in one of those corners can still use the slices.

**Hand-built plates** get the cone the caller passes to `TryAddPlate()`. A viewer
has to see every point of the plate within that angle, not just its middle, so the
cone's tip gets pushed back `radius / tan(angle)` from the plate. At 90 degrees
that push is zero, and the cone becomes the plate's own half space. A double-sided
plate gets the same cone mirrored onto its back side, and the whole footprint has
to fit in one of the two.

## Which Submesh a Plate Depends On

A plate records **one** submesh dependency: either a submesh index, or -1. The
runtime then checks either "is that submesh drawn opaquely?" or "are all the -1
submeshes drawn opaquely?"

-1 doesn't mean every submesh. It points to a **per-mesh list of the submeshes -1
covers**, which is the set of submeshes that make up the closed surface the slice
was cut from. That list belongs to the mesh, not to any one plate, so it's stored
once and costs nothing per plate. Without it, a mesh with a submesh that isn't
part of its shell, like a decal or a detail quad, would have every slice disabled
by a submesh that could never matter.

Sets of two or more specific submeshes can't be represented, on purpose. Nothing
produces one. Rule 5 is either satisfied by a single patch of triangles, which
means one submesh, or by the whole closed surface, which means all of them. A
region that spans two patches can always be narrowed down to one or widened to
-1.

A whole-mesh slice and a narrowed single-submesh plate can both exist for the same
geometry. The runtime uses whichever is usable. A mesh whose submeshes are all
opaque keeps the stronger whole-mesh plate, and one with a transparent submesh
falls back to the narrower one.

### Narrowing With a Cone

To tie rule 5 to a single submesh, intersect the validity region with a cone of
view directions, chosen so every ray reaching the plate has to pass through that
submesh's patch.

**Check the plate as if every other submesh didn't exist.** If the plate is valid
that way, it stays valid when the others are added back. That's because rule 3 is
about a triangle and a ray direction, not the rest of the scene. A triangle is
outward-facing when the ray hits its renderable side, whether or not something
else is in front of it. Adding geometry back can't undo the hit or flip the
facing. And if the added geometry does hide the patch, it does so with opaque
triangles that are even closer, so the pixel is covered at a closer depth than
claimed.

This is what makes narrowing practical on a concave mesh. If the patch had to be
the *first* thing each ray crossed, the cone would need a visibility calculation
over the whole mesh. It doesn't. It just has to be hit, front-on, at or before the
plate.

For a plate a distance `d` behind a patch, a ray at angle `t` from the patch
normal lands `d * tan(t)` sideways from where a straight-on ray would. So a plate
inset from the patch's edge by a margin `m` can only be reached through that patch
for any cone half-angle up to `atan(m / d)`. Shrink the patch by `m`, place the
plate inside that, and you've got your cone.

This doesn't help on a **flat** patch. A surface plate right on the patch is
closer, works from more angles, and costs the same. It only helps where there's no
flat patch to sit on: a curved patch belonging to one submesh, on a mesh whose
other submeshes might not be opaque. A car with glass windows is the classic case.
Without narrowing, a car like that gets no interior plates at all. It also gives
an open curved mesh, like a terrain chunk, plates for the first time, since it has
no shell.

#### How the Baker Builds Them

`OccluderNarrowedPlates` makes the argument above exact for curved patches, where
"distance behind the patch" isn't one number. For one submesh, one of the 26
directions `d` the slicer uses, and a half-angle `θ`:

1.  The patch is every triangle of the submesh facing within `90° - θ` of `d`.
    Those face toward every ray that arrives within `θ` of `-d`.
2.  The plate sits below the patch's lowest vertex. The region's tip sits above
    the highest one, so every viewer is above the whole patch.
3.  Looking straight down `d`, the patch's outline is its edges used an odd number
    of times. Wherever that outline winds around a point an odd number of times, a
    vertical line through the point crosses the patch an odd number of times. This
    is a mod-2 degree argument, so it holds for any triangle soup, with folds and
    overlaps.
4.  A grid keeps the cells the outline never touches that have odd winding. The
    plate goes where every point is at least `(height of the climb) * tan θ` from
    anything else.
5.  A ray from the region climbs from the plate to above the patch while drifting
    sideways less than that, so it never crosses the outline. Sliding it sideways
    until it's vertical keeps its crossing count odd, so it hits the patch at
    least once, on a front face.

The baker tries 35, then 25, then 15 degrees, and keeps the first one that gives a
plate bigger than the minimum area and bigger than what surface plates facing that
way already cover. It only runs for meshes with more than one submesh, or with no
shell. A closed mesh with one submesh always has usable shell plates, so narrowed
ones would only add rasterizing.

The plate budget gives narrowed plates their own direction groups, so bigger
slices facing the same way can't push them out.

A region shape is only worth supporting if you can test a convex polygon against
it using just the polygon's vertices. A half space works, and so does "outside at
least one of k planes". A sphere's outside doesn't. A footprint could have every
vertex outside the sphere while its middle passes right through.

### Which Plates to Try

When both kinds exist for the same geometry, the runtime picks instead of testing
everything. If the camera is outside the instance's bounds and the whole-mesh
plates are usable, it only uses those. They're the stronger plates, and the
narrowed ones would add rasterizing without adding coverage. If the camera is
inside the bounds, it considers both, since that's exactly when the whole-mesh
region might say no. Shadow views count as outside.

That's about cost, not correctness. Skipping the narrowed plates can only lose
culling.

## What the Material Filter Assumes

Material property overrides can't change keywords, render queues, or transparency
modes, so they can't turn an opaque material into a transparent one. That makes
the material asset the source of truth. The only way to fool the filter is to edit
the material asset itself at runtime, and **a project that does that is responsible
for adding `PeekabooDisableOccluderTag` to the affected renderers**.

Kinemation's `OcclusionCullingContextAspect` tells Peekaboo which mesh, material,
and submesh each entity is drawing right now, including LOD ranges and crossfades.
Keep the filtering inside one or two jobs, so if Kinemation's API changes, the fix
stays local.

## Checking Rule 5

Rule 5 is a claim about every possible ray. You can't prove it by sampling rays,
and a sampled check that's right almost every time is just a false occlusion
waiting to happen. The region shapes above are all provably correct by how
they're built. Any new region shape needs its own proof, not just a test that
failed to find a problem.

A dense random-ray *falsifier* is still useful: a ray that breaks rule 5 proves
there's a bug. Passing it proves nothing, though, and it should never be the thing
that decides a region is safe.

## What's Built

-   Rules 1 to 5, Amendment A, and Amendment B are implemented and enforced.
-   Surface plates get the half space on their outward side. Interior slices get
    everything outside the mesh's 13-direction hull. Hand-built plates get the
    caller's cone, mirrored onto the back side for double-sided plates.
-   Plates record their submesh, or -1 for the shell. The -1 list is a 64-bit
    mask per mesh. The runtime filters by material through
    `OcclusionCullingContextAspect`. See [Baking Pipeline](Baking%20Pipeline.md)
    and [Culling](Culling.md).
-   Only mesh LOD 0 is baked, so plates don't need to record a LOD level. A
    renderer drawing any other mesh LOD doesn't occlude.

-   Narrowed single-submesh plates are built for meshes with several submeshes or
    no shell, with the runtime skipping them when the shell's plates are usable
    and the camera is outside the mesh.

Still to do:

-   **Baking other mesh LODs.** Each LOD would need its own plates and budget.
-   **Double-sided surface plates.** A surface plate drawn with a double-sided
    material is valid from behind too, but only if it sits exactly on the
    surface. The baker pushes a surface plate slightly behind its patch so it
    can't poke out the front, which puts it slightly in front from the back side.
    So surface plates only get their outward side for now.
