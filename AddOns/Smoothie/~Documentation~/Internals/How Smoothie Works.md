# How Smoothie Works

Smoothie performs blending based on entities, with specific archetypes and
shared components. These blending entities are separate from the main simulation
entities. Builder APIs construct these entities.

## Bindings

A binding is an attachment to a specific field of a specific component, stored
in a type-agnostic way. Smoothie uses bindings to drive outputs, but also allows
bindings for various inputs, allowing Smoothie to incorporate dynamically
changing values in its evaluation. Every component type in ECS is assigned a
`TypeIndex`. A `TypeIndex` can be obtained from the `TypeManager` generically.
However, a `TypeIndex` can also be passed into the `TypeManager` to retrieve
useful metadata about the type, such as its size. That is all that is needed to
index arrays and perform `memcpy` operations.

If you pass a reference to a struct, as well as a reference to a field within
that struct, you can convert those references into temporary pointers. The
reference to the struct holds the address of the first field in the struct,
while the passed in reference to an arbitrary field will be the address for that
specific field. The difference in these memory addresses is the byte offset of
the passed in field within the struct. Thus, for any pointer to the same type of
struct, this offset can be added to the pointer to reach the specific field.

`ComponentBroker` is a type that provides access to many component types within
a job. Internally, it works by storing a hashmap that maps `TypeIndex` to
indices into its large array of `DynamicComponentTypeHandles`. This means that
it can fetch components only given a `TypeIndex`, as long as the type defined by
the `TypeIndex` was in the list of types the `ComponentBroker` was constructed
with.

With these three details, it is possible to create a Burst-compatible binding
system. Bindings are used to describe the output, in which the binding field
type matches the blended result type. Bindings can also be used for various
inputs, in which the binding does not necessarily need to match the required
input type. In such cases, conversions from the binding type to the required
type can be made strictly using metadata and reinterpreting pointers. Output
bindings to QVVS Transforms components are special-cased (by checking the
`TypeIndex`) and handled with `TransformAspect` to ensure correctness.

## Blend Jobs Flow

Blending happens with four different jobs. There are three main reasons for
this. First, while technically possible to perform everything in a single job,
it is impossible to detect aliased outputs, resulting in race conditions.
Second, the split jobs workflow is slightly more efficient with memory access.
And third, this avoids race conditions where the output also happens to be an
input to another blend (the blend will always use the old result).

Aliasing detection is inherently a single-threaded operation, so the question
is, can we detect aliasing of the outputs in one thread at the same time the
remaining threads compute the blends? Yes we can. But moreso, if we are doing
this aliasing detection, that means this single thread has identified which
blend outputs go to the same components of the same entities (but different
fields). Can we take advantage of that so that results can be written
efficiently?

We can store the pointer to each blend result in a struct, and then sort the
structs by the target entities’ chunks. Then we can use a job that iterates the
output chunks and fetches the data at each result pointer to patch the results.
However, when the single thread is running alongside the main blending job,
Unity’s safety system won’t let it capture the pointers to the results. Thus, an
initial parallel job is needed to cache these pointers and extra metadata. We
thus end up with four jobs that operate in three phases:

-   Phase 1
    -   CapturePointersJob (parallel)
-   Phase 2
    -   GroupResultsJob (single-threaded)
    -   BlendJob (parallel)
-   Phase 3
    -   ApplyResultsJob

Assuming that sorting is a cache-friendly action, this pattern only creates two
random access per blend. The first access is to find each target entity’s
`EntityStorageInfo` which is then used in the sorting process. The second access
is when applying the results, the target entity needs to read the result value
from a pointer it is given. The cost of this is similar to a single
`ComponentLookup`, except there’s no dealing with the archetype lookup cache at
the entity level. In addition, because the `EntityStorageInfos` are the only
random lookup `GroupResultsJob` performs, there is strong potential for cache
efficiency gains due to accidental locality within the array all accessed by the
same thread.

## Instructions and Procedures

Every chunk of blending entities has a `BlendInstructions`, which is an
`ISharedComponentData`. This component defines the various details needed to
properly perform blending for all entities in the chunk. This skips having to
discover the combination of components in the chunk and allows the blend job to
access precisely the components it needs to.

The topmost enumeration in the `BlendInstructions` is the `Procedure`. The
Procedure defines the high-level pattern of steps needed to perform a blend. For
example, many blending operations follow the `ProgressCurveInterpolate` pattern.
In this pattern, there is a `Progression` value in the range [0, 1] and a
`Duration` in seconds. This `Progression` is updated using the passed in
`deltaTime`. Then, the progress value is remapped to another value also in the
range [0, 1] (though overshoot is allowed here) using some kind of curve
function. This remapped value is then used to linearly interpolate the output
between start and end values of the output’s type.

Other things the `BlendInstructions` contains are the output type, the various
input modes (bindings or constants), the curve function, and some conditional
flags for handling the blend entity lifecycle.

## SIMD Blending

Leveraging SIMD is tricky. It isn’t as simple as just compiling things with
Burst (most people assume Burst gets its speed-up from vectorization, but
actually it is rare that’s where the speedup comes from). To fully utilize SIMD,
multiple blending entities need to be evaluated together. But complicating
things is that some of those entities can be disabled.

There are two facts Smoothie uses for its SIMD implementation. First, it is
faster to calculate values for disabled entities in-place than it is to try and
realign the data into compact temporary working buffers. Second, the true length
in bytes of every ECS component array in a chunk is exactly some multiple of the
cache line size (where a cache line size is defined to be 64 bytes) and aligned
to a cache line. The cache line size is always a multiple of the SIMD vector
width (16 or 32 bytes). No matter how many entities are in a chunk, if we start
from the beginning of a component array and access elements a SIMD vector at a
time, no SIMD vector will ever contain both this component array and elements
from the next component array.

We can store in a byte array a list of SIMD vectors counted from the beginning
of a component array which have at least one enabled entity requiring blending.
Then we can write a loop like this:

```csharp
for (int i = 0; i < simdIndices.Length; i++)
{
    for (int j = 0; j < kSimdStride; j++)
    {
        var index    = simdIndices[i] + j;
        inout[index] = math.smoothstep(0f, 1f, inout[index]);
    }
}
```

The `kSimdStride` is measured in `float` elements, which is 8 for AVX CPUs, and
4 for everything else. As long as there are no branches or other oddities, Burst
will replace the inner loop with vector instructions.

For situations where an operation doesn’t work nicely with SIMD, such as reading
input bindings or writing outputs to components in the chunk,
`ChunkEntityEnumerator` is a viable alternative.
