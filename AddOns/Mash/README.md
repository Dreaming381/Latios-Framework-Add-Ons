# Mash – Input System Bridge

Mash brings Unity’s Input System into ECS without all the boilerplate. It
automatically generates code to poll an `.inputactions` asset and store the
results inside an `IComponentData` plus a `DynamicMultiList` for events at the
start of `SimulationSystemGroup`. You use authoring components to decide where
the runtime components should land. Mash also provides access to the runtime
engine input in case you need to configure bindings.

## Features

-   Generates a component per action map, with a typed field per action
-   Provides event buffers for Button and Pass-Through actions
-   No hand-written bakers or systems are required, and no need for Unity’s
    *Generate C\# Class* option
-   Each entity baked with an asset gets its own clone of the asset at runtime,
    so that local multiplayer works out of the box
-   Rebind at runtime is fully supported using Unity’s own APIs
-   Works with prefabs and `EntityManager.Instantiate()`
-   Works with both QVVS Transforms and Unity Transforms

### Known Limitations

-   Only runtime worlds are supported. Mash doesn’t run in the Editor world.
-   Actions with a Control Type of *Any*, or a type that isn’t a single value
    (*Touch*, *Pose*, *Bone*, *Eyes*, *TouchPhase*), are not supported.
-   Mash doesn’t pair devices to players for you.

## Getting Started

**Scripting Define:** LATIOS_ADDON_MASH

**Requirements:**

-   Requires Latios Framework 0.16.0 or newer
-   Requires the Input System package, with *Active Input Handling* set to
    *Input System Package (New)* or *Both* in *Player Settings*

**Main Author(s):** Dreaming I’m Latios

**Support:** Feel free to reach me through any of the same channels you would
use for the Latios Framework!

**Disclaimer:** This add-on was developed by AI with conversational assistance.
If it works for you, great! If not, ¯\\*(ツ)*/¯

### Installing

Add the following installer line to your `ICustomBootstrap`:

```csharp
Latios.Mash.MashBootstrap.InstallMash(world);
```

Don’t add it to your `ICustomEditorBootstrap`. If you do, Mash logs a warning
and skips installing.

### Authoring and Baking

Create your `.inputactions` asset like normal. Whenever you save it, Mash writes
a C\# file next to it, such as `PlayerControls.Mash.g.cs`. That file holds all
the generated types, which live in the `Latios.Mash.Generated` namespace. Mash
moves and deletes this file along with the asset. Commit it to version control
with the asset.

The generated file compiles into whichever assembly owns its folder. If that’s
one of your own assembly definitions, it needs references to `Latios.Core`,
`Latios.Mash`, `Unity.Entities`, `Unity.Entities.Hybrid`, `Unity.Collections`,
`Unity.Mathematics`, and `Unity.InputSystem`.

Next, add the *Mash Input (Mash)* component to a GameObject in a subscene or a
prefab (*Add Component → Latios → Mash → Mash Input (Mash)*), and assign your
asset to its *Input Actions* field.

#### What Gets Generated

Say you have a `PlayerControls` asset with a `Player` map containing `Move`
(Value, Vector2) and `Jump` (Button). Mash generates:

-   `PlayerControls_PlayerState` – An `IComponentData` with a `float2 move`
    field and a `bool jump` field
-   `PlayerControls_PlayerEventHeader` and `PlayerControls_PlayerEventElement` –
    The buffers that store the events for each Button and Pass-Through action
-   `PlayerControls_PlayerEventChannels` – Constants for indexing those events,
    one per Button and Pass-Through action (in this case, just `jump`)
-   `PlayerControls_AssetRef` – The asset the entity was baked from, and the
    entity’s private copy of it for rebinding
-   `PlayerControls_InitializationFailed` – A tag Mash adds if it can’t set up
    this asset on the entity

You’ll also see a receiver and a descriptor type in the generated file. Those
are for Mash’s internal use.

#### Control Types

Every action needs a concrete *Control Type*. Mash picks the field type from the
value type of that control:

| Control value type | Generated field | Example Control Types       |
|--------------------|-----------------|-----------------------------|
| `float`            | `float`         | Axis, Analog, Key           |
| `int`              | `int`           | Integer, Digital            |
| `double`           | `double`        | Double                      |
| `Vector2`          | `float2`        | Vector2, Stick, Dpad, Delta |
| `Vector3`          | `float3`        | Vector3                     |
| `Quaternion`       | `quaternion`    | Quaternion                  |

An action whose *Action Type* is *Button*, or whose Control Type is *Button*,
always becomes a `bool`.

Mash looks up the value type from the Input System’s layout registry, so custom
control layouts work too.

If an action uses an unsupported Control Type, Mash logs an error naming the
action and skips generating code for the whole asset.

Make sure each binding matches its action’s Control Type. The Input Actions
editor’s control picker does this for you. But if you hand-edit a binding and
pair, say, a `Vector3` action with a `Vector2` control, the Input System throws
when Mash reads the action.

#### Naming

Mash turns asset, map, and action names into C\# names. It removes every
character that isn’t a letter or digit, then uses **UpperCamelCase** for types
and **camelCase** for fields. For example, `Move Forward` becomes the type
`MoveForward` and the field `moveForward`. A leading acronym is lowercased as a
whole, so a `UI` map gives you `ui`.

**Every name must still be a valid C\# identifier after the other characters are
removed.** That means it can’t be empty or start with a digit. `2D Look` and
`!!!` won’t work, but `Look At Target` is fine. Action names also can’t turn
into C\# keywords, so name your action `Continue Game` instead of `Continue`.

Names in the same asset (map names) or the same map (action names) must differ
by more than punctuation, spaces, or capitalization. `Move Forward`,
`Move-Forward`, and `move forward` all count as the same name.

If a name breaks these rules, Mash logs an error and skips generating code for
the whole asset.

Generated types include the asset name, so two assets with the same name (or
names that differ only by punctuation) will clash if their generated files end
up in the same assembly.

### Runtime

Every action’s value lives on the generated state component. A `bool` field is
`true` while its action is pressed.

```csharp
using Latios.Mash.Generated;

public partial struct PlayerMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var playerState in SystemAPI.Query<RefRO<PlayerControls_PlayerState>>())
        {
            float2 move = playerState.ValueRO.move;
            // ...
        }
    }
}
```

When you need every event since the last frame, read the event buffers instead.
This covers multiple presses in one frame, as well as the phases and durations
from interactions like *Hold*, *Tap*, and *Multi Tap*. The buffers form a
`DynamicMultiList`. Index it using the generated channel constants.

```csharp
public partial struct PlayerJumpSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (headers, elements) in
                 SystemAPI.Query<DynamicBuffer<PlayerControls_PlayerEventHeader>,
                                 DynamicBuffer<PlayerControls_PlayerEventElement> >())
        {
            var events = new DynamicMultiList<MashActionEvent>(
                headers.Reinterpret<MultiListHeader<MashActionEvent> >(),
                elements.Reinterpret<MultiListElement<MashActionEvent> >());

            foreach (var jumpEvent in events[PlayerControls_PlayerEventChannels.jump])
            {
                if (jumpEvent.phase == InputActionPhase.Performed)
                {
                    // ...
                }
            }
        }
    }
}
```

`MashActionEvent.value` holds the action’s value when the event happened, packed
into a `float4`. Unused components are zero.

Mash updates at the very start of `SimulationSystemGroup`, after Unity’s own
Input System update for the frame. The event buffers only hold events from the
latest update, so read them every frame if you care about them.

Mash doesn’t support `FixedStepSimulationSystemGroup`. A fixed step can run
several times in one frame or not at all, so it would see the same events twice
or miss them. If you need fixed-rate gameplay, use Core’s Ticking instead. Mash
always updates before the ticking loop, so your systems in
`TickedInputSuperSystem` can read Mash’s components and use `TickingState` to
decide how to combine, sustain, or reset input across ticks.

#### Rebinding

Mash doesn’t have its own rebinding API. Instead, grab the entity’s copy of the
asset from the generated `AssetRef` component and use Unity’s APIs on it. Mash
sees the change on its next update.

```csharp
var runtimeAsset = SystemAPI.GetComponent<PlayerControls_AssetRef>(entity).runtimeAsset.Value;
runtimeAsset.FindActionMap("Player").FindAction("Jump").PerformInteractiveRebinding().Start();
```

#### Multiple Players and Instantiation

Every Mash entity gets its own private runtime clone of the asset. That means
you can put the Mash component on several GameObjects (for example, one per
local player) and they won’t interfere with each other. Pairing each copy to a
specific device is up to you.

`EntityManager.Instantiate()` works on Mash entities too, whether you
instantiate a baked prefab or a live entity. The new entity gets its own copy of
the asset the next time Mash updates. Until then, its `AssetRef.runtimeAsset`
still points to the original entity’s copy, so don’t touch it before Mash runs.

#### Sharing Input with a Blackboard Entity

Want the same input to drive both your character and your camera? Add Core’s
*Blackboard Entity Data* component next to the Mash component. The Mash
components get merged onto the world or scene blackboard entity, and your
systems can read them from there.

```csharp
var move = latiosWorld.worldBlackboardEntity.GetComponentData<PlayerControls_PlayerState>().move;
```

You can merge more than one asset into the same blackboard entity, like one for
gameplay and one for menus. If several subscenes each merge the same asset with
*Overwrite* or *Keep Existing*, the blackboard entity keeps one working copy of
the asset.

#### Failures

If Mash can’t set up an asset on an entity, either because no asset is assigned
or because the generated code threw an exception, Mash logs the error once and
adds the generated `InitializationFailed` tag (such as
`PlayerControls_InitializationFailed`) to the entity. After that, Mash ignores
that asset on that entity. Remove the tag if you want Mash to try again.

## Troubleshooting

*Q: A Mash entity I spawned isn’t set up until the next frame.*

A: Mash only sets up an entity after Core’s `LatiosWorldSyncGroup` has seen it.
If you spawn it after that group but before Mash, such as from
`BeginSimulationEntityCommandBufferSystem`, Mash waits a frame.
