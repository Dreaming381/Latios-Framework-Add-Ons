---
name: vfxwaitress
description: Read, query, edit and validate Unity VFX Graph assets (.vfx, .vfxoperator, .vfxblock) from the command line. Use whenever a task involves inspecting or changing a VFX graph, because the raw YAML runs to thousands of lines and there is no public API for editing one.
---

# VfxWaitress

A CLI for Unity VFX Graph files. A nine-node effect is 2500 lines of YAML, most
of it slot objects. Never read a `.vfx` directly. Run `vfxwaitress show` on it
instead.

## First commands

```
vfxwaitress show <vfx>                  # the whole graph, a dozen lines
vfxwaitress show <vfx> --node <sel>     # one node in full, with its slot tree
vfxwaitress q <vfx> "<selector>"        # chainable structural query
vfxwaitress validate <vfx> --editor     # the only check that proves it compiles
vfxwaitress help <topic>                # format select edit catalog ids editor
```

## Reading a graph

```
param p5142 in  Int32 "SpawnCount" =0 exposed

node n5168 Add  a<-p5140  b<-n5165
system d5164  capacity=16384  boundsMode=Manual
  ctx c5150 VFXBasicInitialize  bounds=(0,0,0,500,500,500)  flow>c5211
    blk b5180 SetAttribute  attribute=position  _Position<-n5172.s
```

- `a<-p5140`: port `a` is fed by `p5140`. `.Port` is dropped when the source has
  one output.
- `system d…` groups the contexts sharing one set of per-system settings
  (capacity, bounds mode, space). Change those with `system set`, not on a
  context.
- Ids are a kind letter plus the tail of the file id: `n` node, `c` context,
  `b` block, `p` parameter, `d` system. They stay the same across calls.

## Editing — prefer one `apply`

Every edit makes Editor round trips when an Editor is running, so put a whole
session in one script. It edits one in-memory copy and writes once. Any error
aborts the whole script and leaves the file untouched.

```
vfxwaitress apply Fx.vfx edits.vfxw
vfxwaitress apply Fx.vfx --script -       # read from stdin
```

A script is the CLI verbs with the graph path left out. `--as label` names what
a line creates, and later lines use `$label`:

```
node add "Sample Graphics Buffer" --as sample --wire buffer=p4985 --wire index=n5037
block add c4993 "Set Position" --as setPos --wire _Position=$sample.s
set $setPos._Position 0,1,0
link c5150 c5211
system set d5164 capacity=4096
```

Verbs: `node add`, `block add <context>`, `wire <src>[.port] <dst>.<port>`,
`unwire <dst>.<port>`, `set <node>.<port> <value>`, `set <node> <value>
--setting <name>`, `link <ctxA> <ctxB>`, `rm <sel>`, `system set <d-id> k=v`,
`echo`. The same verbs work one at a time as `vfxwaitress <verb> <vfx> ...`.

`node add` and `block add` take `--wire Port=source[.Port]` for their inputs and
`--feed [Port=]destination.Port` for their outputs. A new node is placed beside
what it's wired to once the command or script finishes. Nothing else moves.

You never supply a coordinate. `vfxwaitress layout <vfx>` rearranges the whole
graph, so only run it when the human wants that.

## Things that bite

- **Settings can reshape slots.** `SetAttribute attribute=size` has different
  slots from `attribute=position`. Give such settings on `node add`/`block add`,
  where they pick the right variant. `vfxwaitress model <type>` lists settings,
  values, and slots. A value with no variant is refused.
- **Slot names don't match what you configured.** `SetAttribute` set to
  `position` names its slot `_Position`, and random mode renames slots to `A`
  and `B`. Matching ignores case, underscores and spaces, and a miss lists the
  real slots.
- **A composite value goes on the whole slot**: `set $n.A 1,2,3`, never on a
  child like `A.x`.
- **`link` also merges two particle contexts into one system.** Always use it
  rather than writing the flow by hand.
- **VFX Graph fails silently.** A graph that can't compile still imports and
  reports no error, with every exposed property missing. Finish with
  `vfxwaitress validate <vfx> --editor`.

## Discovering node types

```
vfxwaitress catalog "*burst*"            search by type or menu name
vfxwaitress catalog "*Quad*" --kind context
vfxwaitress model VFXSpawnerBurst        settings, values and slots
```

Menu names aren't type names: "Periodic Burst" is `VFXSpawnerBurst` with
`repeat=Periodic`.

## Commands that need the Editor

`validate --editor`, `resync`, `attr set`, and asset references go through the
running Editor, via Pipeline commands that the "VfxWaitress Editor Bridge"
package sample registers. Every write also flushes and hands back any open VFX
Graph window, so the human's unsaved work is kept and the graph doesn't open
dirty. `--offline` turns all of that off.

## Getting the binary and catalog

The tool ships in the Latios Framework Addons package under
`AddOns/Waitress/Waitress~/`. Build it once with the .NET SDK:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The node catalog lives in the project's `UserSettings/Waitress/catalog.json`.
Export it with `unity command vfxwaitress_export_catalog` once the Editor Bridge
sample is imported. `Waitress~/docs/VfxWaitress-README.md` covers the rest.
