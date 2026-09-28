# Culling

Peekaboo runs inside Kinemation's culling loop, after LOD selection and before
draw commands get generated. It gets there through
`KinemationBootstrap.InstallOcclusionCullingSystem()`. LOD selection has to come
first, because which mesh an entity is drawing decides which plates it occludes
with.

## What a Renderer Is Drawing

For each candidate, Kinemation's `OcclusionCullingContextAspect` resolves the
entity's `MaterialMeshInfo` into the mesh, material, and submesh combos it's about
to draw. That handles ranges, range LODs, crossfades, and override meshes, using the
same data draw command generation uses. `PeekabooDrawFilter` then merges those per
mesh into a mask of which submeshes are drawn with an opaque material. A mesh with
no opaque submesh gets dropped.

When plates get built, each one checks its submesh dependency against that mask.
A surface plate needs its own submesh, and an interior plate needs every submesh
in the mesh's shell. This is the only place materials matter at runtime. It all
lives in `PeekabooDrawFilter` and the gather job, so a change to Kinemation's API
stays local.

This runs after the screen size check, so only entities that could become
occluders ever get resolved.

## Choosing Occluders

Candidates are entities that survived frustum culling, have a plate blob, and
aren't excluded by the occluder query, a crossfade, or a mesh LOD other than 0.
Each one gets scored per view:

```
score = coveredAreaFraction * depthWeight * min(1, largestPlateArea / boundsCrossSectionArea)
```

-   **coveredAreaFraction** is the fraction of the buffer's texels the entity's
    bounds fill *completely*.
-   **depthWeight** is how much of the view lies behind it, since an occluder can
    only hide what's behind it.
-   The last factor is how much of its own silhouette its plates can fill. It's
    what stops a mesh with one small plate from outranking a solid one.

A candidate that scores below the coverage floor for its view type gets dropped.

### Filled, Not Touched

Projecting bounds gives a floating-point screen rectangle, and occluders and
occludees round it in opposite directions. An occludee has to be tested over
every texel it reaches into, so its rectangle rounds outward. An occluder can only
claim texels it fills completely, so its rectangle rounds inward:
`[ceil(lo), floor(hi) - 1]` on each axis, and empty when `floor(hi) <= ceil(lo)`.

This matters most right around the coverage floor. Bounds from 3.2 to 4.8 touch
two texels but fill none. Scored outward, they'd look twice as useful as a
candidate that really fills one.

Rounding inward also means the geometry itself keeps the candidate count down,
not just the floor. At four million renderers with the floor at zero, there are
about 15,000 candidates, and lowering the floor further finds nothing more. Scored
outward, the same floor lets in almost a million candidates and culls less.

So a floor of zero is meaningful. It means an occluder has to fill at least one
whole texel, which is the least it needs to hide anything.

### The Depth Weight

The weight is one minus the occluder's depth after projection, running from 0 at
the far plane to 1 at the near plane. Peekaboo has planes rather than a matrix, so
that's `(far - z) / (far - near)`. Under orthographic projection, that's exactly a
linear remap of view depth. Under perspective, it matches to within the near
plane's share of the range, which is tiny for a camera.

The other option would be weighting by world volume. But that's only right if
renderers are spread evenly through the scene, and they aren't. LOD schemes put
far more of them near the camera, so the near end of a view is worth more than its
share of the volume. The clip-space curve leans that way and is steeper, which
lets the coverage floor turn away a distant chunk in one go instead of opening it
up and checking each entity.

Both kinds of weight pick nearly the same occluders. What differs is how much the
floor can reject. In the city stress scene, the clip-space weight cuts camera
candidates from about 3,500 to about 2,000 and makes the gather step roughly 13%
cheaper, with the same occluders chosen and the same renderers left over.

One formula works for both projections. The only difference is how view depth
comes back out of closeness: the reciprocal for perspective, the negation for
orthographic. Don't drop the near plane from the formula. An orthographic view is
centered on its volume, so its near distance is *negative*, and dropping it would
give the whole near half of every cascade a weight of one.

Without a depth weight, a caster at the back of a cascade would score the same as
one at the front while shadowing almost nothing. An orthographic rectangle doesn't
shrink with distance, and cascades are four of the five views in a typical frame.

The weight uses the occluder's *nearest* point. Occlusion starts at the surface
facing the viewer, which is what gets rasterized.

The weight only reorders candidates when they sit at different depths. With a high
sun, cascade occluders are all at about the same depth and nothing changes. With
the sun four degrees above the horizon, a cascade's depth runs along the ground,
and the same budget leaves about 1% fewer renderers to draw.

### Tuning the Floor and Resolution

**Judge settings by what's left to draw, not by what got culled.** Culling 90%
versus 95% sounds close, but it's half as many draw calls.

**Resolution buys more than the floor does.** In the city stress scene with the
camera parked, at four million renderers:

| camera / light | camera floor | left to draw | Peekaboo's own time |
|---|---|---|---|
| 256x128 / 128x128 | 0.0005 | 287,338 | 1.216 ms |
| 512x256 / 256x256 | 0.0015 | 274,399 | 1.193 ms |
| **512x256 / 256x256** | **0.001** | **258,652** | **1.250 ms** |
| 512x256 / 256x256 | 0.00075 | 255,923 | 1.338 ms |
| 1024x512 / 512x512 | 0.001 | 228,923 | 1.774 ms |

The bold row uses the default resolutions. (The default camera floor is higher,
at 0.003, for reasons covered below.) These times are only Peekaboo's own work. The whole pass also waits around 0.6 ms
for whatever the culling loop still has running, and counting that would make
every difference here look smaller than it is.

The two knobs interact in a way that isn't obvious. A finer buffer wants a
*higher* floor. Coverage is counted in whole texels, so at low resolution a small
occluder rounds down to nothing and gets dropped for free. At high resolution the
same occluder fills a few texels, clears a low floor, and has to be scored, ranked,
and rasterized. Raising the floor is what pays for the higher resolution.

The camera floor default is 0.003. It's a broad sweet spot: anywhere from 0.0003
to 0.01 leaves the same renderers. The light floor default is 0.001.

Going to four times the default resolution keeps helping, but slowly: about
30,000 more renderers culled for another half millisecond. It's there for scenes
where the GPU is the bottleneck.

One case is given up on purpose. Several objects in a row, each too thin to fill a
texel on its own, can weld together into something that fully covers texels.
Scoring each one alone misses that, so they get dropped. Nothing gets culled
wrongly because of it. The occluder set is just smaller than it could be.

The survivors get grouped by view, and each group is partitioned around
`maxOccludersPerView`, which picks the best that many without fully sorting them.

### A Camera Inside an Occluder

When the camera is inside an occluder's bounds, those bounds straddle the near
plane and can't be projected. If the eye is inside the bounds, the occluder
scores the maximum, since from in there it could cover the whole view. If the eye
is outside the bounds, the bounds just crossed the near plane, which means
something small is right in front of the camera. That scores nothing, so a few
of them can't steal every slot.

Scoring the enclosing occluder highly is only safe because scoring doesn't decide
what gets culled. Each plate still has to pass its validity check before it's
rasterized (see below). So a room the camera is standing in draws its inner walls,
while a solid the camera has clipped into draws nothing.

### Why Frustum-Culled Candidates Are Enough

Picking occluders only from what survived frustum culling doesn't lose anything,
even though it seems like it should. The four side planes of a frustum all pass
through the eye. For any visible point, the line from the eye to that point stays
inside all four of those half-spaces, since each is convex and contains both ends.
So an occluder outside a side plane isn't on any such line and can't block it, no
matter how close to the edge of the screen it is. For orthographic views it's even
simpler, since a parallel ray through a point in the volume shares that point's
screen position.

The other two ways out of the frustum are just as safe. Beyond the far plane is
behind everything inside. In front of the near plane isn't drawn at all, so using
it as an occluder would cull things you can see right through it. That exclusion
is required.

### No Frame-to-Frame State

Nothing carries over between frames. There's no reprojecting last frame's buffer
and no memory of which occluders worked last time. That's deliberate. Temporal
state makes a culling system's output depend on its history, and then a wrong
cull can only be reproduced by replaying that history. Reprojection is the obvious
next step if Peekaboo needs to cull more. [Deferred Work](Deferred%20Work.md)
covers what it would take.

Rasterization order doesn't matter either. The buffer keeps a max per pixel, which
doesn't care about order, so there's no front-to-back sort.

## Plate Validity at Rasterization

Before a plate gets rasterized, Peekaboo checks that the viewer is somewhere the
plate is valid from. It projects the plate onto the near plane, which gives the
spots a ray to the plate has to pass through, moves those points into the
occluder's local space, and checks them against the plate's baked validity region.
Plates that fail are skipped. [Plate Validity](Plate%20Validity.md) has the full
rules.

## Testing Occludees

First, each chunk's own bounds get tested. A whole chunk hidden in every view is
the common case next to a big occluder, and it clears up to 128 entities at once.

Otherwise, Peekaboo tests each entity. It projects the eight corners of its
`WorldRenderBounds`, rounds the screen rectangle outward, and takes the closeness
of the nearest corner. The entity gets culled when every texel of that rectangle
holds a depth closer than that corner.

An entity whose bounds cross the near plane is skipped, since projecting a point
behind the eye doesn't mean anything. Those are big things right in front of the
camera, which are the least likely to be hidden anyway.

## The Depth Pyramid

Each view's buffer has a min pyramid. A coarse texel holds the minimum of the four
under it, so it only claims what all of them claim. Reading a coarser level than
needed can only lower the result, which just loses a cull.

**Picking the right level matters a lot, and not just for quality.** The cheapest
query picks the coarsest level where the rectangle still spans two texels, which is
two to four reads. But then a ten-pixel rectangle straddling a texel boundary gets
tested over a thirty-two-pixel box, and loses its cull to whatever empty space the
overhang covers. Reading a finer level, so the rectangle spans at most a fixed
number of texels per axis, costs more reads. But on the randomized test scenes it
raises how many hidden targets actually get culled from 51% to 70% under
perspective, and from 8% to 36% under orthographic.

**Coarser isn't cheaper either.** A lost cull at the chunk level means the whole
chunk has to be tested entity by entity, so precision here decides how much work
the rest of the pass gets. Always reading a single texel, the cheapest option,
makes the camera pass 60% to 150% more expensive, because the surviving set nearly
doubles and each survivor gets tested on its own.

The limit is sixteen texels per axis. Compared to eight, it's about 6% cheaper for
the camera pass from above the rooftops and 10% cheaper looking across open
ground, and it culls a little more everywhere. Thirty-two is better from above but
worse elsewhere. Street-level views barely care, since almost nothing reaches the
per-entity path there.

## Shadow Cascades

A light pass gets one view per cascade. They're all rasterized into one buffer
array with per-view offsets, and all their rows run as a single parallel job so
the worker threads stay busy.

Each entity has a bitmask of which cascades it's in, stored in
`ChunkPerCameraCullingSplitsMask`. Peekaboo tests each cascade the entity is in,
clears that cascade's bit if it's hidden there, and only marks the entity culled
once no cascades are left. Culling a caster from one cascade while keeping it in
another is completely normal.

Occlusion culling works for shadows for the same reason it works for cameras. A
shadow map only keeps the nearest depth, so a caster completely behind another
caster (from the light's point of view) adds nothing to it.

**Gathering occluders uses the same mask**, so an occluder is only offered to the
cascades it's actually drawn in. That's about picking better occluders, not about
correctness, and it's worth being precise about which.

Offering an occluder to a cascade it isn't in can't cause a wrong cull. Either
it's off to the side of that cascade, so its rectangle clamps to nothing and the
floor drops it. Or it's past the cascade's far plane, so it's farther from the
light than everything in the cascade and can't hide anything there. The near side
can't cause one either, since Kinemation extends each cascade's caster range toward
the light, so anything that can shadow the cascade is already in its mask.

What it does cost is budget. Candidates that can't occlude anything in a cascade
still compete for its `maxOccludersPerView` slots and push out ones that could.
Filtering by the mask saves about 20% of gather time at four million renderers,
and the better occluder set helps draw command generation too.

## Stats

`PeekabooStats` on the `worldBlackboardEntity` adds up the whole frame instead of
reporting just the last pass. A frame is a camera pass plus one pass per
shadow-casting light, and reporting only the last would hide whichever ran first.
The counters reset on the frame's first pass, detected by `cullIndexThisFrame`
being zero or smaller than the last pass that ran. The second check catches the
case where an earlier pass bailed out before resetting them.

`culledCount + survivingCount` is how many renderers made it past frustum
culling, which is the baseline to judge survivors against.

If `consideredCount` is well above `occluderCount`, `maxOccludersPerView` is the
limit. If `consideredCount` is zero, a coverage floor is.

`peakWorkerCandidateCount` is the most candidates any one worker held for a single
view. If it reaches `maxOccludersPerView`, workers were evicting candidates. Don't
trust the averages here. At four million renderers it hits 256 even with only
about 2,700 candidates in the whole frame, since the chunks holding occluders
aren't spread evenly across workers. Evicting is still safe, because anything in
the overall best K is outranked by fewer than K candidates anywhere, so it's
outranked by fewer than K on its own worker too.
