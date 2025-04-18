# Shockwave – Unified Physics Engine Query API

Shockwave is an API surface and protocol for allowing add-ons to be compatible
with multiple different Psyshock-based physics engines simultaneously. Shockwave
provides a partially-defined `WorldCollisionAspect` which other add-ons can
leverage. Any Psyshock-based physics engine can then use asmref to populate the
`WorldCollisionAspect` with implementation details specific for that engine.
This allows engines to use their own partitioning and caching schemes, while
still remaining compatible with other add-ons.

## Features

Shockwave is still under construction. Help define its API by making a pull
request!

## Getting Started

**Scripting Define:** LATIOS_ADDON_SHOCKWAVE

**Requirements:**

-   Requires Latios Framework 0.13.0 or newer

**Main Author(s):** Dreaming I’m Latios

**Additional Contributors:**

**Support:** Please make feature requests for features you would like to see
added! You can use any of the Latios Framework support channels to make
requests.

### Installing

Shockwave has no installation requirement other than being installed alongside a
single physics engine which supports its API.

### Usage

You can retrieve the `WorldCollisionAspect` via

```csharp
latiosWorld.sceneBlackboardEntity.GetCollectionAspect<WorldCollisionAspect>()
```

You can then pass this structure to a job. In a job, you can call `CreateMask()`
to create temporary mask instances based on entity queries. You can use these
masks in various APIs to reduce the number of entities you search for.
