---
name: memorywaitress
description: Read Unity Memory Profiler snapshots (.snap) from the command line, and take new ones from the Editor. Use whenever a task involves memory usage, leaks, what keeps an object or asset alive, which textures/meshes/assets cost the most, managed (C#) heap contents, destroyed Unity objects still referenced, duplicate assets, or comparing memory before and after a change, because .snap files are binaries only the Memory Profiler window can open.
---

# MemoryWaitress

A CLI over Unity Memory Profiler snapshots. It reads `.snap` files directly
(no package, no Editor needed to read) and walks the managed heap itself, so
every command is one call: a 1.6 GB Editor snapshot loads in about 4 s.
Sizes are bytes, printed as B/KB/MB/GB.

## First commands

```
memorywaitress capture --play --name before   # Play Mode snapshot into MemoryCaptures/, prints info
memorywaitress info <snap>                    # totals, biggest types, leak hints
memorywaitress objects <snap> --type Texture2D
memorywaitress why <snap> <#id | @address | name | Type>
memorywaitress diff before after              # what grew
memorywaitress help                           # every command and option
```

`<snap>` is a path, a folder (its newest snapshot), `latest`, or part of a file
name in `<project>/MemoryCaptures/`. Run from inside the Unity project.

## Answering the usual questions

| Question | Command |
|---|---|
| How much memory, and where? | `info`, `types`, `native --by area` |
| Which textures/meshes/assets are biggest? | `objects --type Texture2D` (or Mesh, AudioClip...), `types --native` |
| What keeps this asset/object alive? | `why <snap> #instanceId` (Unity object) or `why <snap> @address` (managed); it prints the managed holder path too |
| Why are there so many instances of my class? | `why <snap> MyClass` (instances grouped by the root that holds them) |
| Which statics hold the most managed memory? | `managed <snap> --by root` |
| What does this static hold? | `why <snap> s_Cache` (field, or Type.field) |
| Biggest managed objects (arrays, caches)? | `managed <snap> [--type "System.Byte[]"]` |
| Is something leaking? | `capture` A, repeat the action, `capture` B, `diff A B` |
| Destroyed objects still referenced from C#? | `leaks [--root MyNamespace]` |
| Same asset loaded twice? | `dupes [--type Texture2D]` |
| Native memory by system/allocator? | `native --by area`, `--by root`, `--by allocator` |
| Where is my NativeArray / Malloc memory? | `native --by allocation --filter UnsafeUtility` (largest single allocations, exact bytes) |

## Taking snapshots

```
memorywaitress capture                       # the Editor as it is now (edit mode or Play Mode)
memorywaitress capture --play --wait 5       # enter Play Mode, wait 5 s, snapshot, leave Play Mode
memorywaitress capture --name after --no-info
```

An Editor snapshot is the whole Editor process (1-2 GB for a DOTS project):
Editor tooling and everything it has loaded is mixed in with the game. Filter by
your own names, types and roots (`--type`, `--name`, `--root`, `--filter`). For
a leak hunt, take both snapshots in the same Play Mode session: instance ids
stay the same, so `diff` lists exactly the new Unity objects. `capture --play`
leaves Play Mode again only if it entered it, so for two snapshots in one session
enter Play Mode first (`unity command editor_play`) and capture without `--play`.

## Reading the output

- Unity objects show `Type "name" #instanceId`. Pass `#id` to `why`.
- Managed objects show `Type @address`. Pass `@address` to `why`.
- `held by` / roots: `static Type.field`, `GC handle of <Unity object>` (a Unity
  object's own managed shell), or `GC handle to a T (held by native code)` (a GCHandle.Alloc or an
  engine-held object such as an ECS system).
- `native` / `gfx` / `managed` per Unity object match the Memory Profiler
  window's Unity Objects table exactly. `help numbers` defines each one.
- A big managed heap with small live objects isn't a leak. It's free space and
  garbage not yet collected. Watch live object size and counts in `diff`.

## Notes

- Live managed objects are those reachable from GC handles and static fields,
  the same roots as the Memory Profiler package. MemoryWaitress also follows
  references inside struct-typed statics, which the package misses, so its
  counts can be a few hundred objects higher.
- Names: a Unity object name, or a managed type by full name, name without
  namespace, or as printed (`List<LeakNode>`). When a Unity object and a managed
  type share a name, `why` picks the object and says so; add `--managed`.
- `--type` filters match exactly when a type has that name (`Texture2D` excludes
  `Texture2DArray`); otherwise by substring; `*` and `?` glob.
- Native sizes are the snapshot's, as in the Memory Profiler window.
  `Profiler.GetRuntimeMemorySizeLong` agrees for textures and meshes but reports
  Materials about 25% higher.
- Roots are GC handles and statics only. Objects held only by thread stacks,
  `AppDomain.SetData` or raw native pointers are invisible.
- ECS chunks: `native --by size` shows them as 1048576-byte `Malloc(Persistent)`
  blocks (64 chunks each).
- `dupes` is a hint (same type, name and size). Confirm with `why` on each id.
- `help numbers`, `help why`, `help leaks`, `help snapshots`, `help format` for
  details.
