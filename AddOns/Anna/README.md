# Anna – Physics Engine

Anna is a simple out-of-the-box rigid body physics engine built on Psyshock’s
UnitySim APIs. It is intended to be general-purpose, striking a balance between
performance, flexibility, and most importantly, ease-of-use.

## Features

Currently, Anna provides basic rigid bodies which can collide with each other
and the environment. It also provides a full constraint system with a runtime
API for feeding the constraint solver. Optionally, Anna can bake all Unity
built-in rigid bodies and joints.

## Getting Started

**Scripting Define:** LATIOS_ADDON_ANNA

**Requirements:**

-   Requires Latios Framework 0.16.1 or newer
-   Requires using QVVS Transforms

**Main Author(s):** Dreaming I’m Latios

**Additional Contributors:** Obrazy, aqscithe

**Support:** Please make feature requests for features you would like to see
added! You can use any of the Latios Framework support channels to make
requests.

### Installing

Add the following to `LatiosBootstrap` (only the runtime is necessary):

```csharp
Latios.Anna.AnnaBootstrap.InstallAnna(world);
```

This method returns an `AnnaSuperSystem`, which you can use to install an
`IRateManager` like this:

```csharp
var anna = Latios.Anna.AnnaBootstrap.InstallAnna(world);
anna.SetRateManagerCreateAllocator(new SubstepRateManager(1f / 60f, 8));
```

You can also inject `AnnaSuperSystem` manually, as that system performs the
entire update process.

If you want Anna to bake Unity’s built-in physics components, add the following
to `LatiosBakingBootstrap`:

```csharp
Latios.Anna.Authoring.AnnaBakingBootstrap.InstallAnnaBakers(ref context);
```

### Basic Usage

Use the `CollisionTagAuthoring` component to specify static environment and
kinematic colliders in your scene. And use the `AnnaRigidBodyAuthoring`
component to set up rigid bodies. Use the `AnnaSettings` component to configure
scene properties. Put a `BlackboardEntityDataAuthoring` set to the Scene scope
on the same GameObject so that the settings end up on the
`sceneBlackboardEntity`.

At runtime, you can either directly modify the `RigidBody` values, or you can
use the `AddImpulse` dynamic buffer.

You can disable collision between pairs of entity queries by adding a
`DynamicBuffer<CollisionExclusionPair>` buffer to any entity, including system
entities. An exclusion will match independent of the ordering of entities found
in a pair (that is, it behaves in the way you would expect if you don’t
overthink it).

### Baking Unity’s Physics Components

With `InstallAnnaBakers()` in your baking bootstrap, you can use built-in
components to author physics:

-   A `Rigidbody` becomes an Anna rigid body. Mass, gravity, frozen axes, and
    custom center of mass and inertia carry over. Friction and bounciness come
    from the collider’s `PhysicsMaterial`.
-   A `Rigidbody` with `isKinematic` becomes a kinematic collider, and all other
    properties are ignored.
-   `FixedJoint`, `HingeJoint`, `CharacterJoint`, `ConfigurableJoint`, and
    `SpringJoint` become joints. A ragdoll made of nested *Rigidbodies* and
    *CharacterJoints* works out-of-the-box.

Colliders without a `Rigidbody` stay out of the simulation until you tag them
with `CollisionTagAuthoring`. `AnnaRigidBodyAuthoring` and
`CollisionTagAuthoring` take precedence over the built-in components.

**Warning:** Unity treats a collider on a child of a `Rigidbody` as part of that
body. Anna doesn’t, so merge those colliders into the body with Psyshock’s
`ColliderAuthoring` compound.

`HingeJoint` motors and `ConfigurableJoint` drives become Anna motors. Anna’s
motors currently are single-axis like Unity Physics. When several rotation axes
are driven at once, the target rotation gets split into per-axis angles, which
is only an approximation. Slerp drives aren’t supported.

A few joint features don’t carry over. Joints never break, `HingeJoint` ignores
`freeSpin`, `ConfigurableJoint` ignores `swapBodies`, and every joint ignores
the mass scale properties.

While a subscene is open in play mode, Anna switches Game Object physics to
`SimulationMode.Script`. Otherwise Game Object physics would move the authoring
Rigidbodies around, and the subscene would rebake every frame. The original mode
comes back when you exit play mode. If you need Game Object physics running
alongside an open subscene, pass `false` for
`disableGameObjectPhysicsInPlayMode`.

### Joints

A joint is a `DynamicBuffer<JointConstraint>` on any entity. Each element
contains a joint *weld* for both `entityA` and `entityB`. A *weld* is like a
child transform to the entity which has position and rotation, but no scale. The
constraint enforces rules about the position or rotation between the welds.
`entityA` must be a rigid body. `entityB` can be another rigid body, a kinematic
collider, any other entity with a transform, or `Entity.Null` for a fixed point
in the world.

Each joint is enforced with a simulated spring specified by the
`springFrequency` and `dampingRatio`. Use `UnitySim.kStiffSpringFrequency` and
`UnitySim.kStiffDampingRatio` for a hard limit, or a lower frequency for a soft
one. Jointed bodies don’t collide with each other unless you set
`enableCollision`.

Motors are `JointContraints` too. A motor drives a single axis toward its
`target`, which is an angle, an angular velocity, an offset, or a velocity
depending on the motor type. `maxForce` caps how hard it can push. Use
`float.PositiveInfinity` if it shouldn’t be capped at all. If you’d rather write
motors yourself, `ConstraintWriter` has matching `Drive*()` methods.

### Adding Constraints

Anna provides an API that allows you to feed constraints directly into the
solver. In fact, the built-in constraints (contacts, joints, and locking)
exclusively use this public API.

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

### Other Customizations

You can override the center of mass and the inertia tensor by adding
`LocalCenterOfMassOverride` and `LocalInertiaOverride` components to the rigid
body. Similarly, you can override the gravity value applied to a single rigid
body with the `GravityOverride` component.

## Known Issue

Anna is still missing APIs for receiving feedback about collisions and impulses.
I’ve yet to decide on an API for them. If this is something you would like to
see, feel free to discuss your specific needs on the framework Discord.
