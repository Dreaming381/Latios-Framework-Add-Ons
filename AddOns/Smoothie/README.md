# Smoothie – Entities Data-Driven Tweening

Smoothie is a tweening library for ECS designed for expressiveness using a
data-driven paradigm. Every tween is an independent entity from the target, and
uses a type-agnostic binding system to call out fields within components to
tween, without requiring explicit code for tweening each component type (other
than adding the type to a `ComponentBroker`).

## Features

Smoothie is still in development, and offers no functionality at this time.

## Getting Started

**Scripting Define:** LATIOS_ADDON_SMOOTHIE

**Requirements:**

-   Requires Latios Framework 0.13.6 or newer

**Main Author(s):** Dreaming I’m Latios

**Additional Contributors:**

**Support:** This add-on is a proof-of-concept and is not ready for production
yet. If you would like to help this add-on reach production-viability, please
reach out on the official framework discord.

### Creating a Blend Entity

The following will create a smoothstep tween on the target entity’s x-axis
position, starting from world space position 1f, and ending at wherever goal
entity’s x-axis position will be at the end of the 2 second duration.

```csharp
// In a job
WorldTransform bindableTransform = default; // A struct used for bindings. It does not have to be initialized.
var ecbBlendEntity = Blend.Target(targetEntity, ref bindableTransform, ref bindableTransform.worldTransform.position.x)
    .From(1f) // Constant endpoint
    .To(goalEntity, ref bindableTransform, ref bindableTransform.worldTransform.position.x) // Dynamic binding endpoint (the endpoint moves over time)
    .Smoothstepped().OverDuration(2f).Build(ref ecb, unfilteredChunkIndex);
```

### Blending

There is no automatic system to perform blending. Instead, use the
`BlendScheduler` helper struct in your own system to perform the blending.

```csharp
partial struct ExampleBlendSystem : ISystem
{
    EntityQuery    m_blendQuery;
    BlendScheduler m_blendScheduler;

    public void OnCreate(ref SystemState state)
    {
        m_blendQuery = state.Fluent().WithRequiredSmoothieBlendingComponents().Build();

        var readTypes = new FixedList512Bytes<TypeIndex>()
        {
            // Add any types you want to read from in bindings (such as start and end value bindings) here
            TypeManager.GetTypeIndex<WorldTransform>()
        };
        var writeTypes = new FixedList512Bytes<TypeIndex>()
        {
            // Add any types whose values you want to blend here
            TypeManager.GetTypeIndex<WorldTransform>()
        };
        m_blendScheduler = new BlendScheduler(ref state, in readTypes, in writeTypes);
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = m_blendScheduler.Schedule(ref state, m_blendQuery, SystemAPI.Time.DeltaTime, state.Dependency);
    }

    public void OnDestroy(ref SystemState state) => m_blendScheduler.Dispose();
}
```

### Aliasing Rules

Smoothie allows multiple blends to target the same component on the same entity,
as long as the fields targeted do not overlap. By default, Smoothie performs
aliasing detection checks in the editor when safety checks are enabled, though
this can be disabled in the `BlendScheduler`’s constructor.

Smoothie special-cases bindings to `WorldTransform` and `LocalTransform` and
works how you probably want it to.

## Known Limitations (That Will Be Resolved in the Future)

-   Only allows binding to singular `float` fields per blend
-   Only supports `IComponentData` for bindings
-   Only provides `lerp` and `smoothstep` blends
-   Does not provide any APIs for fast-forwarding a blend to completion, or
    reverting a blend
-   Does not provide any sequencing capabilities
-   Does not have a bulk blend creation API
-   Only supports `EntityCommandBuffer.ParallelWriter` for creating entities
