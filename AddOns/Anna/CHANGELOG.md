# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic
Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.3] – 2025-9-13

### Added

-   Added optional `GravityOverride` component which can be attached to rigid
    bodies

### Changed

-   Enabled `UnitySim` friction velocity heuristic to counteract unwanted motion
    when rigid bodies collide with multiple triangles within a tri-mesh or
    terrain collider

## [0.2.2] – 2025-8-30

### Changed

-   Updated to Shockwave 0.2.0

## [0.2.1] – 2025-8-9

### Added

-   Added optional components `LocalCenterOfMassOverride` and
    `LocalInertiaOverride` which can be attached to rigid bodies

## [0.2.0] – 2025-7-6

### Added

-   *New Feature:* Added `ConstraintWriter` API and inject-target
    `ConstraintWritingSuperSystem` to allow custom collision and joints to be
    fed into Anna’s solver
-   *New Feature:* Added the ability to exclude pairs of entity queries from
    collision via `CollisionExclusionPair` dynamic buffer which can be assigned
    to any number of entities including system entities
-   Added `rigidBodyVsRigidBodyMaxDepenetrationVelocity` to `AnnaSettings`
-   Added new angular-only `AddImpulse` constructor
-   `AnnaBootstrap` now returns the `AnnaSuperSystem` so that users can install
    an `IRateManager` for it, such as a `SubstepRateManager`

### Changed

-   **Breaking:** Spatial queries now need to use either Shockwave or
    `ConstraintCollisionWorld`

### Fixed

-   Fixed position locking on multiple axes

### Improved

-   Improved collision behavior in many use cases with the upgrade to Psyshock
    0.13.0

### Removed

-   Removed `EnvironmentCollisionLayer`, `KinematicCollisionLayer`, and
    `RigidBodyCollisionLayer` collection components

## [0.1.2] – 2025-3-29

### Fixed

-   Fixed collision layer allocations failing to dispose if the
    `worldUpdateAllocator` rewound

## [0.1.1] – 2025-2-8

### Fixed

-   Fixed system order in projects using injection-based system ordering
-   Fixed rigid bodies and kinematic colliders with the environment tag showing
    up in multiple collision layers by explicitly excluding them from the
    environment layer
-   Fixed position axis locking forcing the center of mass world-position to
    zero along the axis, when it is supposed to lock to the initial position in
    the frame
-   Fixed single-axis rotation locking accidentally using dual-axis locking

## [0.1.0] - 2025-1-18

### This is the first release of *Anna* as an add-on.
