# Peekaboo – CPU Occlusion Culling

Are you GPU-bound? Are too many entities being drawn each frame? Peekaboo is a
CPU occlusion culling algorithm that stops hidden objects from even reaching the
GPU.

## Features

-   Hooks directly into Kinemation’s culling pipeline, culling for the camera
    and shadow maps
-   Strongly conservative algorithm that only culls what is truly hidden
-   Works fully out-of-the-box once installed with zero authoring required
-   Allows authoring to override whether an entity can be an occludee or occlude
-   Handles extreme scene scales especially well
-   Uses polygonal plates to CPU-rasterize, avoiding high-resolution geometry
-   Build plates yourself for runtime geometry with `PeekabooOccluderBuilder`
-   Reports what each pass did through `PeekabooStats`

### Known Limitations

-   This technique costs CPU performance during the culling loop, and can do
    more harm than good for your overall framerate if your project doesn’t
    benefit from it. Peekaboo scales up especially well. It is smaller scenes
    where it struggles to pay off.
-   Peekaboo processes all meshes in each subscene’s `RenderMeshArray` every
    bake, which can be a lot.
-   Peekaboo does not run at runtime when Burst is disabled unless requested to,
    because without Burst, it is way too expensive.
-   Two separate renderers that touch aren't joined together. A pixel on the
    seam between them isn't treated as covered. Modular walls and floor tiles
    can fail to occlude because of this.

## Getting Started

**Scripting Define:** LATIOS_ADDON_PEEKABOO

**Requirements:**

-   Requires Latios Framework 0.16.1 or newer
-   Requires Kinemation
-   Requires Burst is enabled

**Main Author(s):** Dreaming I’m Latios

**Support:** Feel free to reach me through any of the same channels you would
use for the Latios Framework!

**Disclaimer:** This add-on was developed by AI with conversational assistance.
If it works for you, great! If not, ¯\\*(ツ)*/¯

### Installing

Add the baking installer after Kinemation's:

```csharp
public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
{
    Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallKinemation(ref context);
    Latios.Peekaboo.Authoring.PeekabooBakingBootstrap.InstallPeekaboo(ref context);
}
```

Then add the runtime installer after Kinemation for both the `LatiosBootstrap`
and the `LatiosEditorBootstrap`:

```csharp
Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
Latios.Peekaboo.PeekabooBootstrap.InstallPeekaboo(world);
```

Rebake your subscenes and you're done.

Occlusion culling is software rasterization, and without Burst it costs far more
than it saves. So if Burst is off, Peekaboo turns itself off instead of slowing
your frame down. If you need to step through the culling code in a debugger,
pass `debugOverrideAllowWithoutBurst: true` to `InstallPeekaboo()`.

## Tuning

`PeekabooSettings` lives on the `worldBlackboardEntity`. You can change it at
any time. Here are the most impactful:

-   `cameraResolution` and `lightResolution` – The size of the depth buffer. A
    bigger buffer culls more but costs more. The defaults are 512x256 for
    cameras and 256x256 for each shadow map and cascade.
-   `maxOccludersPerView` – How many occluders get rasterized in each view. The
    default is 256.
-   `minCameraOccluderCoverage` and `minLightOccluderCoverage` – The minimum
    fraction of the clip space volume a renderer must hide in order to be
    considered as an occluder. Higher values improve CPU performance, especially
    with heavily-populated scenes, but may reduce culling.
-   `cullCameras` and `cullLights` – Toggle occlusion culling for each kind of
    pass.

`PeekabooStats` on the `worldBlackboardEntity` contains the tallies from all
passes in the previous frame. Be sure to compare how many renderers survived
versus how many passed frustum culling. The difference is how many draws
Peekaboo saved.

Stats only update in the editor and in development builds, so counting them
costs nothing in a release build. Add the scripting define
`PEEKABOO_ENABLE_STATS` if you need them in a release build too, or
`PEEKABOO_DISABLE_STATS` to turn them off everywhere, including the editor, so
you can profile without them.

To see where the rasterizer spends its time in the Unity Profiler, add the
scripting define `PEEKABOO_PROFILE_RASTER`. It adds markers for each phase of
rasterizing a scanline. They're off by default due to their overhead.

## Opting Out

Add an *Occlusion Settings (Peekaboo)* component to a renderer to stop it from
occluding, being occluded, or both. In code, add `PeekabooDisableOccluderTag` or
`PeekabooDisableOccludeeTag`.

A renderer will never be considered as an occluder if it is any of the
following:

-   depth sorted
-   deforming
-   uses a runtime-registered mesh (and therefore, has no occluder data)
-   has the PeekabooDisableOccluderTag
-   is crossfading between LODs
-   uses a mesh LOD above level 0 (this may change in the future)

Peekaboo generates occluder data for individual submeshes in addition to the
full mesh, and will check materials at runtime to determine which parts of the
mesh it can use for occlusion. This allows a car’s body to occlude, but not its
windshield. The following materials disqualify a submesh:

-   transparency
-   alpha clipping
-   front face culling (draws only backfaces)
-   a runtime-registered material

If you change a material asset at runtime so that it becomes transparent or
alpha-clipped, add `PeekabooDisableOccluderTag` to the renderers using it.
Peekaboo can't see that change on its own. Material property overrides require
no maintenance, since they can't change the shader’s operating mode.

## Building Your Own Occluders

Peekaboo builds plates by reading meshes. So anything without a mesh to read,
like heightmap terrain, a procedural surface, or anything generated at runtime,
never occludes by default. `PeekabooOccluderBuilder` lets you build the same
data from polygons you provide:

```csharp
using var builder = new PeekabooOccluderBuilder(meshCount: 1, Allocator.Temp);

// A convex 2D polygon in the renderer's local space. Either winding works.
var result = builder.TryAddPlate(0, quad);
if (result != PeekabooPlateResult.Added)
    Debug.LogWarning($"Plate rejected: {result}");

builder.SetMeshBounds(0, rendererBounds);   // Optional, see below
var blob = builder.CreateBlobAssetReference(Allocator.Persistent);
entityManager.AddComponentData(entity, new PeekabooOccluder { blob = blob });
```

**Artifact-free occlusion is only valid when plates conform to the formal
specification.** Peekaboo only checks whether plates are safe for the
rasterizer. It does not check whether plates conform with the specification.

A plate is defined by a 2D polygon, and an open space referred to as a *validity
region*. Every ray cast out from the camera’s view (parallel for orthographic,
dispersing for perspective) that can hit a plate must hit the real mesh first or
at the same time or else false occlusion can occur. The *validity region*
defines the space where rays originate for that property to be preserved.

Currently, the builder only provides support for specifying the region based on
ray angle relative to the plate normal. A small angle requires a camera view the
plate from head-on. A 90 degree angle is viable for a plate that lies directly
on a flat surface of a mesh. The default is 90.

The builder does check everything that would break the CPU rasterizer. A plate
has to be flat, convex, not collapsed to a line, and within the vertex limit. If
a plate is rejected, you get a reason back instead of just `false`, so you can
find out why your terrain isn't occluding.

`SetMeshBounds()` tells Peekaboo how big the whole renderer is, which it uses to
decide which occluders are worth rasterizing. If you skip it, Peekaboo uses the
plates' own size instead. That can make the occluder look more useful than it
is, but it only affects which occluders get picked, never what gets culled.

The builder provides a blob asset which you can attach to any renderer via the
`PeekabooOccluder` component. The blob-asset is internally marked hand-built,
which disables material checking. It is up to you not to attach the blob to
something see-through. The most common use case is to build a blob containing a
single mesh, and attaching it to a Kinemation UniqueMesh entity.

If you use the `PeekabooOccluderBuilder` and add a `PeekabooOccluder` component
during baking yourself, Peekaboo will respect it and not overwrite it in its
baking system.

## How It Works

If you want the details, the *\~Documentation\~/Implementation Notes* folder
provides the AI-generated notes on the rules plates have to follow, how they're
baked, how the rasterizer stays safe, and a bunch of other things discovered
through various optimization passes.
