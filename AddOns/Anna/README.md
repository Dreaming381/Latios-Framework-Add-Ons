# Anna – Physics Engine

Anna is a simple out-of-the-box rigid body physics engine built on Psyshock’s
UnitySim APIs. It is intended to be general-purpose, striking a balance between
performance, flexibility, and most importantly, ease-of-use.

## Features

Currently, Anna provides basic rigid bodies which can collide with each other
and the environment. Additionally, rigid bodies can have particular position and
rotation axes locked. Anna supports Shockwave for spatial queries. And it
provides an API for feeding constraints directly to the solver.

## Getting Started

**Scripting Define:** LATIOS_ADDON_ANNA

**Requirements:**

-   Requires Latios Framework 0.13.0 or newer

**Main Author(s):** Dreaming I’m Latios

**Additional Contributors:**

**Support:** Please make feature requests for features you would like to see
added! You can use any of the Latios Framework support channels to make
requests.

### Installing

Add the following to `LatiosBootstrap` (only the runtime is necessary):

```csharp
Latios.Anna.AnnaBootstrap.InstallAnna(world);
```

### Basic Usage

Use the `CollisionTagAuthoring` component to specify static environment and
kinematic colliders in your scene. And use the `AnnaRigidBodyAuthoring`
component to set up rigid bodies. Use the `AnnaSettingsAuthoring` to configure
scene properties.

At runtime, you can either directly modify the `RigidBody` values, or you can
use the `AddImpulse` dynamic buffer.

You can disable collision between pairs of entities by adding a
`DynamicBuffer<CollisionExclusionPair>` buffer to any entities, including system
entities. Neither order within a pair, or order amongst pairs matter (that is,
it behaves in the way you would expect if you don’t overthink it).

### Shockwave Integration

Anna builds the Shockwave `WorldCollisionAspect` after `TransformSuperSystem`
within a frame. AABBs will be expanded to include the full motion or rigid
bodies and kinematic colliders since the start of the frame. This only occurs if
Shockwave is installed. `WorldCollisionAspect` lives on the
`sceneBlackboardEntity`.

### Adding Constraints

Anna provides an API that allows you to feed constraints directly into the
solver. In fact, the built-in constraints (contacts and locking) exclusively use
this public API.

To write constraints, your system must update within
`ConstraintWritingSuperSystem`. You will need to create a `ConstraintWriter`,
which is an `ICollectionComponent`. You can add this to any entity, including
system entities. You will also typically want to access
`ConstraintEntityInfoLookup` and possibly `ConstraintCollisionWorld`, which both
live on the `sceneBlackboardEntity`.

`ConstraintEntityInfoLookup` provides handles to rigid bodies, kinematic
colliders, and metadata for describing springs. These three things are only
valid for a single update. *In case you were wondering, springs are
timestep-dependent.*

## Known Issue

Anna is still missing APIs for receiving feedback about collisions and impulses.
I’ve yet to decide on an API for them. If this is something you would like to
see, feel free to discuss your specific needs on the framework Discord.
