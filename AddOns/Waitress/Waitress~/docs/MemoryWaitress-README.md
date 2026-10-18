# MemoryWaitress

MemoryWaitress is a command-line tool for reading Unity Memory Profiler
snapshots (`.snap` files) and for taking new ones from the Editor. It's built
for AI agents, and it's the fourth Waitress tool next to ShaderWaitress,
VfxWaitress and ProfilerWaitress.

A snapshot is a binary dump of a whole process: every native allocation, every
Unity object, every graphics resource, and the raw bytes of the managed heap.
Only the Memory Profiler package opens one, through a window an agent can't use,
and an Editor snapshot of a DOTS project is 1 to 2 GB. MemoryWaitress reads the
file itself, walks the managed heap to find the live objects, and answers
questions like "what is using memory", "what keeps this texture alive", "why are
there 1500 of my class" or "what grew between these two snapshots" in one call
and a few dozen lines.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/MemoryWaitress/release/memorywaitress[.exe]`.

Reading a snapshot needs nothing else: no package, no Editor, no project. Taking
one needs the Editor open with the Pipeline package (0.7 or newer). The tool
carries its Editor script inside itself, writes it to
`UserSettings/Waitress/MemoryWaitress/`, and runs it through `run_script`, so
nothing is imported into the project. It uses only Unity's public
`MemoryProfiler.TakeSnapshot` API, so the Memory Profiler package isn't needed
for that either.

## Start Here

```
memorywaitress capture --play --name before   # Play Mode snapshot, then prints info
memorywaitress info <snap>                    # totals, biggest types, leak hints
memorywaitress objects <snap> --type Texture2D
memorywaitress why <snap> <#id | @address | name | Type>
memorywaitress diff before after
memorywaitress help                           # command index
memorywaitress skill                          # the whole tool on one page, for an agent
```

`<snap>` is a path, a folder (its newest snapshot), `latest`, or part of a file
name in `<project>/MemoryCaptures/`.

## What It Looks Like

```
$ memorywaitress info scenarioA --top 5
snapshot  LF-AI-Workspace_2026-10-15_14-43-11_scenarioA.snap  1.65GB, taken 2026-10-15 19:43:12, format 17
target    LF-AI-Workspace, WindowsEditor, Unity 6000.3.24f1, Direct3D12, 37.4 min after start
          Editor capture: Editor tooling, assets it has loaded and the Editor's own heap are mixed in; ...
unity     used 2.35GB of 3.14GB reserved: managed heap 1.04GB used / 1.40GB, graphics 604MB, audio 1.3MB, ...
native    717MB in 503967 allocations; 109MB of it under 13040 Unity objects
          by area: System 533MB, UnsafeUtility 305MB, Objects 109MB, UIElements 46MB, Managers 37MB
graphics  604MB in 1262 resources; 422MB of it owned by Unity objects
managed   59MB in 519198 live objects; heap 1.40GB in 388487 sections (96% free space or garbage ...), ...
unity objects by type:
  native  graphics  count  type
    28KB     377MB     27  RenderTexture
    37MB      40MB    833  Texture2D
...
leaks     247 destroyed Unity objects are still referenced from managed code (...); see `leaks`
# read in 2.9 s, crawled in 0.5 s
```

```
$ memorywaitress why scenarioB LeakNode
1500 live instances of Latios.Validation.MemoryWaitress.LeakNode, 47KB, held through 1 root:
  1500 instances, 47KB: static Latios.Validation.MemoryWaitress.MemoryScenario.s_Leak
    e.g. static ...MemoryScenario.s_Leak  -> System.Collections.Generic.List<LeakNode> @1f9110ceba0
         ._items -> ...LeakNode[] @1f911805000 [2048]  [0] -> ...LeakNode @1f8c1ffece0
```

## Finding Leaks

Take a snapshot, repeat the suspect action a few times, take another, and diff:

```
unity command editor_play
memorywaitress capture --name before --no-info
# ... repeat the action ...
memorywaitress capture --name after --no-info
memorywaitress diff before after
```

Within one Editor run, Unity objects keep their instance ids, so `diff` lists
exactly the objects that are new. It also lists the managed types whose count
or size changed. `why <snap> <Type>` then says which static field or handle
holds the new instances. `leaks` lists destroyed Unity objects that C# still
references, and `dupes` lists objects with the same type, name and size.

An Editor snapshot is the whole Editor process. Its own tooling, caches and
leaks are mixed in with the game's, so filter by your own names, types and
roots (`--type`, `--name`, `--root`, `--filter`). For game-only numbers, take
the snapshot in a development player and read the `.snap` here.

## Verifying Changes

-   The numbers were checked against the Memory Profiler package itself (1.1.9,
    run in batch mode on the same snapshots). Every one of 14,484 Unity objects
    matched exactly in native, graphics and managed size, and so did every
    managed type's count and size. The one difference is deliberate:
    MemoryWaitress also follows references inside struct-typed static fields,
    which the package misses, so it finds a few hundred more live objects in an
    Editor snapshot.
-   `memorywaitress selftest` writes synthetic snapshots (with data split across
    small scattered chunks) and checks the reader, the crawler, object sizes,
    shortest paths, Unity object sizes and destroyed-object detection. The tests
    are mutation-checked.
-   `Assets/Validation/MemoryWaitress/MemoryScenario.unity` creates known memory
    in Play Mode: 1000 `LeakNode`s with 1000-byte payloads in a static list, a
    4 MB texture, a destroyed but referenced material, three identical meshes,
    an 8 MB NativeArray and a struct-typed static holding an array.
    `MemoryScenario.Grow(n)` adds more nodes, for checking `diff`.

[MemoryWaitress-DESIGN.md](MemoryWaitress-DESIGN.md) covers the file format, how
the crawler finds live objects, the exact meaning of every number, and what
isn't supported.
