# Ribbons – Line and Trail Renderers

Did you know that Unity’s *Line Renderer* and *Trail Renderer* are **not**
whitelisted as ECS companion components? You probably did if you tried to use
them in ECS. So here’s a pure ECS runtime implementation for them built on
Kinemation’s Unique Mesh.

## Features

-   Bakes *Line Renderer* and *Trail Renderer* out-of-the-box
-   Supports width curve and width multiplier
-   Supports color gradients
-   Supports *Stretch*, *Tile*, and *Distribute Per Segment* texture modes
-   Supports *View* and *Transform Z* alignment
-   Supports rounded corners and end caps (*Corner Vertices* and *End Cap
    Vertices*)
-   Supports closed loops (*Line Renderer* only)
-   Trails emit and fade points on their own using *Time* and *Min Vertex
    Distance*
-   Runtime control through ECS components

### Known Limitations

-   Only the first material is used.
-   *Repeat Per Segment* and *Static* texture modes fall back to *Tile*.
-   *Autodestruct* on *Trail Renderer* does nothing.
-   Sorting layers and sorting order are ignored.
-   *Use World Space* on *Line Renderer* only affects baking. At runtime, points
    are always in local space.
-   *View* alignment only faces `Camera.main`. The mesh is built once per frame,
    not once per camera. With an orthographic camera, lines that leave the plane
    the camera faces come out slightly different than Unity.

## Getting Started

**Scripting Define:** LATIOS_ADDON_RIBBONS

**Requirements:**

-   Requires Latios Framework 0.16.1 or newer
-   Requires Kinemation
-   Supports both QVVS Transforms and Unity Transforms

**Main Author(s):** Dreaming I’m Latios

**Support:** Feel free to reach me through any of the same channels you would
use for the Latios Framework!

**Disclaimer:** This add-on was developed by AI with conversational assistance.
If it works for you, great! If not, ¯\\*(ツ)*/¯

### Installing

Add the following installer line to your bootstrap after Kinemation for both the
`LatiosBootstrap` and the `LatiosEditorBootstrap`:

```csharp
Latios.Ribbons.RibbonsBootstrap.InstallRibbons(world);
```

You don’t need a baking bootstrap installer.

### Authoring and Baking

Add a *Line Renderer* or *Trail Renderer* to any subscene or referenced prefab
GameObject, and set it up like you normally would.

To preview your ribbons, go to your *Preferences* window and in the *Entities*
tab, set the *Scene View Mode* to *Runtime Data*. You can also preview in the
*Game* tab.

A *Trail Renderer* bakes with no points. It starts emitting once the entity
moves at runtime.

### Picking a Material

The material’s shader needs to support DOTS Instancing. If it doesn’t, the
ribbon silently renders as a placeholder mesh. *Universal Render
Pipeline/Particles/Unlit* and *Lit* shaders work well, because they support
vertex colors.

### Rounded Corners and Caps

*Corner Vertices* rounds the outside of each corner. The inside stays a sharp
miter, matching Unity’s implementation. *End Cap Vertices* rounds each end.
Larger values have a performance cost associated.

### Runtime

**Line:** Modify `DynamicBuffer<RibbonPoint>` to change the points the rendered
line passes through. Points are in the entity’s local space. You can also change
the settings in `RibbonLineConfig`. After editing the points or modifying any
other settings, enable `RibbonLineConfig` so that Ribbons rebuilds the mesh.
Ribbons disables the component again after it rebuilds the mesh.

```csharp
var points = SystemAPI.GetBuffer<RibbonPoint>(lineEntity);
points.Add(new RibbonPoint { position = newLocalPoint });
SystemAPI.SetComponentEnabled<RibbonLineConfig>(lineEntity, true);
```

**Trail:** A trail draws from the entity’s current position through its
`DynamicBuffer<RibbonTrailPoint>`, and rebuilds its mesh every frame
automatically. Set `RibbonTrailConfig.emitting` to false to leave a gap. The
trail still adds points while it isn’t emitting, but they have zero width. Old
points continue to fade out.

**Width and Color:** Both lines and trails use the same buffers for modifying
width and color over progression. The width curve lives in
`DynamicBuffer<RibbonWidthKeyframe>`, which holds Calci keyframes. The gradient
lives in `DynamicBuffer<RibbonColorKey>`. Progression is a normalized value from
0 to 1 based on distance along the line or trail. In managed contexts, you can
use `RibbonCurves.SetWidthCurve()` and `RibbonCurves.SetGradient()` to fill
these buffers from an `AnimationCurve` or `Gradient`.

Ribbons updates the meshes inside `PresentationSystemGroup`.
