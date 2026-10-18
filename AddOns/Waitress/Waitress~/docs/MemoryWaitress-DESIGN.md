# MemoryWaitress Design

## Goals

An agent investigating memory needs three answers, each in one call: where the
memory is, what keeps a particular thing alive, and what changed between two
moments. The Memory Profiler window answers them for a person through tables
and a references panel. MemoryWaitress answers them as text, reading the
snapshot file directly, so the Editor, the package and the project are all
optional when reading.

## Reading the File

A `.snap` file is a set of chapters (Unity calls them entries), one per kind of
data: native object names, native object sizes, managed heap section bytes,
type names, field offsets, and so on. The format version is stored as the first
chapter (MemoryWaitress reads 11 and newer; Unity 6000.3 writes 17).

-   The last 12 bytes of the file are the directory's address and a footer
    signature. The directory holds a signature, a version, the block section's
    address, then one address per chapter (zero when a chapter is absent).
-   Each chapter has an 18-byte header: format (single value, fixed-size array,
    or variable-size array), block index, and two meta fields whose meaning
    depends on the format (element size and count, or count and first offset).
    Variable-size arrays are followed by the end offset of every element; the
    header holds the first.
-   A chapter's bytes live in a block. A block is split into equal-size chunks
    scattered through the file, so a read maps block offsets to chunk addresses
    and can span chunks.
-   Instance ids grew from 4 to 8 bytes in format 18; elements are widened
    from whatever size the chapter declares.
-   Managed heap section start addresses carry a flag in bit 63 for sections of
    runtime metadata (Mono's VM memory, where vtables live) rather than GC heap.

`SnapFile` implements this with `RandomAccess` reads. `Snapshot` loads every
chapter the commands use into flat arrays. Managed heap sections are kept as
one byte array each, since an Editor heap can exceed 2 GB in total.

## Finding Live Managed Objects

The snapshot holds raw heap bytes plus type and field descriptions, not a list
of objects. `Crawl` finds the objects the same way the Memory Profiler package
does:

-   **Roots** are static fields (from each type's static field bytes) and GC
    handle targets. Native Unity objects record the GC handle of their managed
    shell, which links shells to native objects.
-   **An object's type** comes from its first word. If that is a known type info
    address, the object points at its class directly (IL2CPP). Otherwise it is a
    vtable, and the vtable's first word is the class (Mono).
-   **Sizes:** classes use the type's size, which includes the header. Boxed
    value types add the header to the type's size. Arrays are the array header
    plus length times element size (a pointer for reference elements). The
    length is the int at the size offset, or the product of the bounds when the
    bounds pointer is set. Strings are header + 4 + 2 × length + 2.
-   **Fields:** instance field offsets include the object header, for classes
    and for structs alike. Fields of a struct embedded at offset `p` (in an
    object, an array element or a static) are at `p + offset − header`. Every
    type's reference fields, including those of its base types and of nested
    structs, are flattened once into a list of offsets.
-   **The walk is breadth-first.** Static roots are queued before GC handles.
    Each object records the object or root that first reached it and the field
    or array index it came through, so following parents gives a shortest path
    to a root. Every reference is also kept as an edge, for listing all of an
    object's referrers.

An object whose type can't be resolved, or whose computed size runs past its
heap section, is counted as unresolved and skipped. The crawl of a 1.6 GB
Editor snapshot (half a million live objects) takes about 0.5 s; reading the
file takes about 3 s.

## The Numbers

-   **Unity object sizes** match the Memory Profiler window's Unity Objects
    table:
    -   *native* is the accumulated size of the object's native root, which is
        every allocation Unity rooted to it;
    -   *graphics* is the sum of the graphics resources reported under the same
        root id;
    -   *managed* is the size of the managed shell reached through the object's
        GC handle.
-   **Native totals** sum every native allocation. `native --by area` groups
    root accumulated sizes by area name, so it covers rooted memory only.
-   **Unity used** is Unity's own memory statistics struct stored in the
    snapshot (the Memory module's totals).
-   **Managed heap vs live objects:** the heap sections are everything the GC
    has reserved. Live objects are what the crawl reached. The rest is free
    space and garbage not yet collected, which is why "used" in Unity's stats
    can be twenty times the live object size. Leaks show up as live size and
    counts growing, not as heap size.
-   **Destroyed shells** (`leaks`) are managed objects deriving from
    `UnityEngine.Object` with no native object and a null `m_CachedPtr`.

## Verification

The package itself was the oracle. A throwaway project in a temporary folder
embedded the Memory Profiler package source (1.1.9), plus a dump script compiled
into its Editor assembly that loads a snapshot with `CachedSnapshot`, runs
`PostProcess`, and writes per-type counts and sizes, every object address, and
every Unity object's native, managed and graphics size. It ran in batch mode on
the same 1.6 GB Editor snapshot:

-   All 14,484 Unity objects matched exactly in all three sizes.
-   Every managed type the package found was found, with the same count and size.
-   MemoryWaitress found 242 more objects (13 KB). Every one was referenced
    directly by a static field whose type is a struct holding a reference, for
    example `ImmutableArray<T>.Empty` and Roslyn's `AttributeDescription`
    statics. Those objects are reachable from a root, so they are live. The
    package's static-field layout skips references inside struct-typed statics.

`selftest` builds snapshots with the format writer in `Testing/SnapWriter.cs`
(chunks of 48 bytes, interleaved between blocks) and a hand-laid heap covering:

-   cycles;
-   reference and struct arrays;
-   strings and boxed values;
-   an object resolved through a vtable;
-   a struct-typed static;
-   destroyed and live shells;
-   unreferenced garbage.

Removing the vtable fallback, the struct-static walk, or the header adjustment
of field offsets each fails at least three tests.

`Assets/Validation/MemoryWaitress/MemoryScenario.unity` was captured twice in
one Play Mode session, with `MemoryScenario.Grow(500)` between the two captures:

-   `types` found exactly 1000 `LeakNode`s of 32 bytes.
-   `why` traced them to `MemoryScenario.s_Leak`.
-   `leaks --root Latios` isolated the destroyed material.
-   `dupes` found the three meshes.
-   The texture showed 4 MB native plus 4 MB graphics.
-   The struct static's `int[777]` was found.
-   `diff` reported +500 `LeakNode` and +506 `byte[]`.

Two subagent rounds then checked numbers against the live Editor through
`run_script`:

-   `Resources.FindObjectsOfTypeAll` counts matched for every type checked.
-   `Profiler.GetRuntimeMemorySizeLong` matched for textures, RenderTextures
    and meshes. Materials read about 25% higher at runtime.
-   Objects the subagents planted were found with exact sizes and the right
    holders, including a 12,345,678-byte NativeArray (`native --by allocation`).
    Objects held only through `AppDomain.SetData` were not found, which is a
    known limit.
-   A snapshot cut short is reported as incomplete.

## Known Unsupported

-   **Native allocation callstacks** (`capture --callstacks`) are recorded but
    not printed.
-   **Snapshots of players** are read the same way, but `capture` only snapshots
    the Editor. A player snapshot comes from the Memory Profiler window
    connected to it, or from `MemoryProfiler.TakeSnapshot` in game code.
-   **IL2CPP snapshots** follow the same format and the crawler accepts
    direct class pointers, but none were tested.
-   **Managed stacks, AppDomain data and raw native pointers** aren't roots.
    The package doesn't scan them either. An object held only by a running
    method's locals, by `AppDomain.SetData`, or by native code without a GC
    handle isn't found at all.
-   **Texture dimensions, formats, asset paths and field values** aren't in
    the snapshot or aren't printed. Use the live Editor for those.
-   **Retained sizes** (what would be freed if an object went away) aren't
    computed exactly. `why` on a managed object prints what it reaches first,
    meaning the objects whose shortest path runs through it. `managed --by root` groups each object under the first root that
    reaches it, which approximates ownership for trees but not for shared data.
-   **The memory map** (resident vs committed per region, untracked memory)
    isn't broken down beyond the process totals in `info`.
