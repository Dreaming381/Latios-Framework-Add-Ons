# VfxWaitress Design

This document covers how VfxWaitress works and why. For what the tool does and
how to use it, see `VfxWaitress-README.md` and the tool's own `help`. The shared
rules both tools follow are in `Design Rules.md`.

## The Problem

VFX Graph has the same problems as Shader Graph, in a different container, with
a worse failure mode.

-   **Size.** A nine-node effect is 2511 lines. LifeFX's `Multiply
    QVVS.vfxoperator` is 6907 lines for what the Editor shows as one node.
-   **Fanout.** Most of the bytes are slot objects. One `float4` slot is five
    MonoBehaviours of about 25 lines each: the master slot plus one child per
    component.
-   **Indirection.** Nothing is nested. A node references its slots by local file
    ID, and a slot references its owner, its master, and the slots it links to.
-   **No API.** VFX Graph's authoring model is entirely internal.
-   **No edge list.** VFX Graph stores each link twice, once on each slot. A link
    written on only one end is a broken graph that nothing reports.
-   **Silent failure.** This is the one that matters. VFX Graph's per-node error
    reporters stay quiet on graphs that can't compile. A failed compile still
    gives you an asset that imports, that `LoadAssetAtPath` returns, and that
    reports no error. It just has every exposed property missing. Nothing at
    runtime can tell it apart from a working effect that isn't being fed.

## Goals

1.  Read and write the file format losslessly, byte-identical when nothing
    changed.
2.  Show a graph as dense text that keeps everything that affects the effect,
    including state that lives beside the graph rather than in it.
3.  Answer structural questions in one call with chainable queries.
4.  Edit graphs without the caller ever handling a file ID or a slot object.
5.  Own layout, using VFX Graph's geometry rather than Shader Graph's.
6.  Never write something the Editor will silently ignore. Refuse instead.
7.  Be able to ask the Editor whether the result compiles.

Reimplementing VFX Graph's compiler is not a goal. `--editor` asks the real one.

## The File Format

It's Unity YAML, the same dialect as a scene or prefab:

-   A `%YAML 1.1` and `%TAG` header, then documents that each start with
    `--- !u!<classId> &<fileId>` and a type line such as `MonoBehaviour:`.
-   File IDs are local to the file. Every reference is `{fileID: n}`. A
    reference to another asset adds a `guid` and a `type`.
-   Node types are identified only by their script guid. That's why the catalog
    is required rather than a nice-to-have.

### The Object Model

| Object | Role |
| --- | --- |
| `VisualEffectResource` | The asset root. Points at the graph and holds runtime settings like the initial event, culling, and instancing |
| `VFXGraph` | `m_Children` holds contexts, operators, and parameters. Also holds custom attribute declarations and the UI object reference |
| `VFXContext` | `m_Children` are its blocks. `m_InputFlowSlot` and `m_OutputFlowSlot` hold flow links. `m_Data` is the system it belongs to |
| `VFXData*` | **Per-system** state: capacity, bounds mode, space, and title. `m_Owners` lists the contexts that share it |
| `VFXBlock` | A block inside a context. Its stored position is never used, since the context stacks its blocks |
| `VFXSlot*` | One port. Composite types are a tree. The master slot holds the whole value as a JSON blob, and children exist so each part can be wired |
| `VFXParameter` | An exposed input or a sub-graph output. `m_Nodes` holds its on-canvas placements, each with its own position and links |
| `VFXUI` | Groups and sticky notes |

Three facts shape everything else:

1.  **Capacity and bounds aren't on the context.** They're on the shared
    `VFXData`. A graph read context by context can't see them at all, and a
    setting written to the context is accepted by the file and ignored by
    everything that reads it.
2.  **A flow link between particle contexts is also a data merge.**
    `VFXContext.InnerLink` calls `to.InnerSetData(from.GetData())`. Writing only
    the flow edge leaves contexts wired together but each in its own system. The
    Editor never produces that shape, and no validator notices it.
3.  **Settings reshape slots.** `SetAttribute` has five slot shapes depending on
    its attribute and random mode. `VFXSpawnerBurst` has a `float` Count when
    `spawnMode=Constant` and a `Vector2` Count when it's `Random`. Unity doesn't
    repair a node whose slots belong to a different configuration.

## Fidelity

Every YAML node keeps the exact text it was parsed from. A node nobody wrote to
is written back verbatim, and only edited nodes get regenerated. So the tool
can't reformat a field it didn't touch. That matters because Unity folds long
plain scalars at a width that isn't worth reproducing.

Three YAML shapes are easy to get wrong: an empty scalar keeps the trailing space
after its colon, a quoted scalar runs to its closing quote even across blank
lines, and a sequence can sit at its key's own indent instead of deeper. An empty
list is written as a flow `[]` on the key line, so it parses as a scalar and has
to be rewritten as a block before items can be added.

`roundtrip` checks all of this, both on files as parsed and with every node
forced through the path an edit takes.

## Short IDs

A kind letter plus the shortest unique **tail** of the local file ID: `n` for a
node, `c` for a context, `b` for a block, `p` for a parameter, and `d` for system
data. It uses tails rather than prefixes because IDs in one file share a long
common prefix. They come only from the file ID, so adding or removing a node
doesn't renumber the others.

New file IDs count up from the largest one in the file, skipping any whose last
four digits are already taken. So a node the tool creates gets a four-digit short
ID, the ID `node add` reports is the one a reload gives, and no existing ID has
to grow. Only a node added in the Editor can still collide, and then the table
lengthens the later one.

## Dense Text

This is what `show` prints. It's one line per entity, inputs only, with literals
inline and no coordinates or slot objects.

```
graph "LifeFXTrackedTransforms" kind=vfx contexts=4 blocks=7 operators=6 params=4 edges=11
resource initialEvent=OnPlay updateMode=0 culling=3 instancing=64
attr TransformIndex int "LifeFX tracked transform buffer index"

param p4985 in  GraphicsBuffer "TransformBuffer" exposed

node n5040 SampleBuffer  Type=VfxQvvs  buffer<-p4985  index<-n5037
node n5060 VFXSubgraphOperator  Subgraph="Get QVVS Properties"  qvvs<-n5040.s

system d5007  capacity=4096  boundsMode=Manual
  ctx c4993 VFXBasicInitialize  bounds=(0,0,0,500,500,500)  flow>c5035
    blk b5020 SetAttribute  attribute=TransformIndex  _TransformIndex<-n5015
```

-   `Port<-node.Port` means the port is fed from that port of that node. The
    `.Port` part is dropped when the source has only one output.
-   `system d…` groups the contexts that share a `VFXData`, and carries its
    settings.
-   `attr` lines are the graph's own custom attribute declarations. Without them,
    a custom attribute looks the same as a built-in one.
-   Asset references are resolved from guid to name by reading `.meta` files
    under `Assets/`, `Packages/`, and `Library/PackageCache`. A cached package
    is indexed as `Packages/<name>/...`, the path Unity itself uses. The cache
    holds tens of thousands of `.meta` files, so it's only scanned when a guid
    misses the first two. Folders ending in `~` are skipped, because a
    `Samples~` copy shares its guid with the imported sample.
-   A setting prints when it differs from the **class default**, which is the
    value a fresh instance has before any variant is applied. Every catalog
    entry is a variant of something, so its own "default" is just its own value.
    Comparing against that would hide `attribute=lifetime`, the very setting
    that says what the node is.

`show --node <sel>` prints the whole slot tree. That's where a composite hides
the child that actually holds the value.

## Selectors

A pipeline of steps separated by `|`, starting from the first step. The matchers
are `all`, `type:`, `id:`, `name:`, `kind:`, `context:`, `system:`, `param:`,
`setting:k=v`, `port:`, and `is:…`. The traversals are `in`, `out`, `both`,
`in*`, `out*`, `blocks`, and `context`. Whole pipelines combine with `+`, `-`,
and `&`.

## Editing

### One Verb, Two Ways In

Every edit verb (`node add`, `block add`, `wire`, `unwire`, `set`, `link`, `rm`,
and `system set`) runs through one dispatcher in `Edit/Script.cs`. A command-line
edit is a one-line script with the graph path taken out, and `apply` runs a
whole file of them against one in-memory asset, written once. The text layer
(continuation lines, quoting, comments, and `$label` bindings) is shared with
ShaderWaitress through `Waitress.Common`. A label binds to the new node's raw
file ID, which the selector accepts anywhere a node is expected.

Verbs that go through the Editor (`attr set`, `resync`) or rearrange the whole
graph (`layout`, `repair`) are refused inside a script, since they can't join an
in-memory batch.

### Nodes Come From Templates

For every model, the catalog stores the YAML of a freshly created instance **and
its whole slot tree**, captured with
`InternalEditorUtility.SaveToSerializedFileAndForget`. Adding a node clones that
and remaps its local file IDs. Building a slot tree by hand isn't practical,
since every slot carries its own assembly-qualified type, master reference, and
owner.

Since settings reshape slots, **every variant gets its own template**. A setting
that any variant is keyed on selects a variant instead of being patched onto the
node. A value that no variant covers is refused, with the list of values that do
exist. Patching instead is the worst possible failure: `attribute=size` patched
onto the `oldPosition` template gives a block whose only input is `_OldPosition`,
that displays as "Set Size", that passes every validator, and that has no way to
set a size.

A new context also gets a `VFXData` beside it, since capacity and bounds mode
have nowhere else to live.

### Writes That Would Be Ignored Are Refused

A setting whose serialized form isn't a bare scalar can't be written as one.
Unity loads the mismatch as unset and says nothing. `SerializableType` settings
are written nested, using a type table the exporter provides. The catalog records
each setting's shape so the tool can tell which is which.

Asset references need the main object's local file ID, which is a constant per
importer and isn't stored anywhere in the file. The tool knows a few of these
constants. For anything else it asks the Editor.

### Slot Names Match Loosely

A `SetAttribute` configured with `position` names its slot `_Position`. Turn on
random mode and the slots become `A` and `B`. Matching ignores underscores,
spaces, and case, and **a miss lists the slots the node does have**, which turns
a mystery into a one-line fix.

### Values

A composite slot stores its whole value as one JSON blob on the master slot.
`set A 1,2,3` fills the tree by following the slot's own children, not a table of
known types. Setting a child slot directly is refused, because VFX Graph would
ignore it.

### Wiring

`wire` writes the reference on both slots and reports the resolved link, along
with any type difference VFX Graph will convert. `link` writes the flow edge
**and** merges the two contexts into one system.

### Repair

VFX Graph silently fixes a few shapes the first time a graph is opened, like
duplicate blackboard order indices or a wired parameter with no placement on the
canvas. Each fix marks the asset dirty and produces a diff nobody asked for.
`repair` makes those fixes ahead of time, and `validate` reports them. A repair
never moves anything except the node it's placing.

## Talking to the Editor

Some things can't be decided from the file alone. They all go through the
VfxWaitress Editor Bridge sample. Its `VfxWaitressValidator.cs` compiles into VFX
Graph's editor assembly through an `.asmref`, and a separate asmdef registers
each operation as a `vfxwaitress_*` Pipeline command. That asmdef only compiles
when the Pipeline package is installed. The tool calls them with
`unity command <name> --path <asset> --project-path <project>`, which is a
direct method call in the Editor, with no script compiled per call. Passing the
project path means the call reaches the right Editor when several are open.

| Operation | Why the file can't answer |
| --- | --- |
| `validate --editor` | Whether the graph compiles. Nothing offline can tell |
| `resync` | Which slots a node has. The rule lives in the node's own C# |
| `attr set` | A custom attribute's type reshapes every block that uses it |
| Asset references | The main object's local file ID is a per-importer constant |
| Node sizes | They aren't stored anywhere in the file |
| Every write | See below |

`resync` is the general escape hatch. When the tool refuses a setting because it
decides the slot list, writing it anyway and then resyncing gets you there.
Resync is also how an edited Custom HLSL file reaches the graphs that use it,
since Unity caches the parsed function per node.

### Writes Go Through the Editor

An open VFX Graph window keeps its own copy of the graph. Unsaved edits in it are
invisible to a tool reading the file, and a file written behind its back gets
overwritten the next time the window saves. So every write:

1.  Flushes whatever the window hasn't saved, so the tool reads the same graph
    the person is looking at.
2.  Makes its edit and writes the file.
3.  Hands the file back and has VFX Graph write the final asset itself, twice.
    Some stored fields are derived from the graph view, which doesn't exist until
    something builds it. The first write settles the graph, and the second
    settles what the view derives from it.

What lands on disk is then exactly what the Editor would have written. Opening
the graph doesn't mark it dirty, and no unsaved-changes dialog is left waiting.

If no Editor answers, or the sample isn't imported, all of this is skipped and
the write says why. The first failure disables it for the rest of the run, so it
costs one attempt. `--offline` skips it on purpose, at the cost of estimated node
sizes and a window that might still show the old graph.

## Layout

VFX Graph is read differently from Shader Graph. Each system is a vertical column
of contexts in flow order, and systems sit side by side. Contexts run top to
bottom, so a left-to-right dataflow layout would produce something nobody can
read.

-   Operators and parameters are ranked leftward from what they feed, and each
    one is placed in the left margin of the leftmost system it serves. Pooling
    them all left of the first column would send every wire into the second or
    third system straight through the columns before it.
-   Within a margin, nodes are stacked **in the order of the ports they feed**,
    not the order their own top edges want. Those disagree when two nodes have
    different heights, and following the top edges crosses their wires. No
    amount of moving them up and down afterwards uncrosses them.
-   A packed stack is then shifted as a whole to where its wires are closest to
    level. The shift that leaves the least total error wins, not the average,
    so one badly placed node can't drag the rest off with it.
-   A column whose first context is a GPU event starts level with the block that
    triggers it. A column fed by a context in another column starts below it.
-   Blocks aren't positioned. A parameter is drawn once per placement record,
    each with its own position, and its own `m_UIPosition` is always zero.

Placement reads positions to decide positions. An operator wants the height of
what it feeds, and on the first pass half of that hasn't been placed yet. So the
passes run several times, feeding each result into the next, and the context
positions carry between passes. Everything runs twice, once starting from the
positions the graph arrived with and once from a neutral start, so one bad
starting point can't trap the graph. The arrangement the graph arrived in is also
a candidate, and the best score wins.

An operator that feeds more than one system is the hard case. See *Known Issue*
in `Design Rules.md`. `layout` tries both spots for it, in the margin or in a
lane above the columns, and keeps whichever scores better.

### New Nodes

A template carries no position. So `node add` first parks the new node left of
the whole graph, where it overlaps nothing. When the command or script finishes
and all its wiring is known, every node it added is placed beside what it's wired
to. Nothing that was already there moves.

-   A context goes below the context that flows into it, in the same column, or
    above the one it flows into. A new context nothing flows into starts a new
    column where it was parked, and its chain stacks below it.
-   Anything else goes left of what it feeds, level with the port it feeds, or
    else right of what feeds it.
-   Placement resolves from the placed end of a chain outward, so a chain of new
    operators lines up in order. Each placed node slides down until it clears
    everything already placed.

A node wired to nothing stays where it was parked. `layout` is still there for a
full rearrangement, but a graph built by `apply` shouldn't need one.

### Sizes

VFX Graph stores no node sizes, so `layout` reads real node and port rectangles
from the live graph view whenever an Editor answers. The rectangles only exist
after the panel has run a layout pass, which happens between calls, so an empty
first answer is normal.

Without an Editor, sizes are estimated from these measurements:

| Node | Measured |
| --- | --- |
| Every context | 424 wide |
| Initialize with 5 blocks | 637 tall |
| Output with no blocks | 297 tall |
| Update with no blocks | 150 tall |
| GPU event | 158 tall |
| Operators | 143 to 210 wide, 68 to 127 tall |

A context is estimated as a base height per kind, plus a title and one row per
input slot for each block. Estimating high is the safe direction, since extra
whitespace is cosmetic and an overlap hides a node completely. `layout` reports
any pair that overlaps by its own estimate.

## The Catalog

`UserSettings/Waitress/catalog.json` is exported from a running Editor by
`VfxWaitressCatalogExporter.cs`, compiled into `Unity.VisualEffectGraph.Editor`
through an `.asmref`. Reflection isn't practical here because the exporter needs
the whole authoring model.

It covers more models than the node menu shows:

-   **Experimental** models are hidden from the public `VFXLibrary` accessors
    behind a user preference, but they show up in graphs Unity itself ships. The
    exporter reads the unfiltered backing lists instead.
-   **Unlisted** models, like sub-graph nodes, deprecated models, and
    `VFXDataParticle`, are never registered at all. Every context needs a
    `VFXDataParticle` beside it, so they're exported too.

For each model it stores the type, menu name, category, synonyms, variant
settings, script guid, settings (with enum values, serialized shape, and class
default), the slot tree, and the template. It also stores the slot type table
with assembly-qualified names and the list of built-in attributes.

The catalog isn't built into the tool, because it describes one project. URP and
HDRP register different nodes, and projects add their own. It lives in the
project's `UserSettings/`, which stays writable however the add-on was installed
and is excluded by Unity's default `.gitignore`. At 14 MB it's not worth sharing
when it takes seconds to regenerate. The tool looks for it in this order:
`--catalog <path>`, `$VFXWAITRESS_CATALOG`, then
`UserSettings/Waitress/catalog.json` in the project found by walking up from the
first path argument, or from the working directory when there isn't one.

## Verification

1.  **Self-test.** `vfxwaitress selftest` builds graphs in a throwaway project
    folder with the Editor link off. It covers the container, project and guid
    lookup, every edit verb, `apply` atomicity, id stability, and placement.
2.  **Round trip.** Every VFX asset under the given roots, byte-identical.
3.  **Sweep.** Render every graph and report anything the catalog can't name.
4.  **Editor acceptance.** A graph the tool edited offline, rewritten by VFX
    Graph's own serializer, comes back byte-identical.
5.  **`validate --editor`.** A real compile through the connected Editor. Every
    offline check has passed on graphs that couldn't compile, and on graphs that
    compiled into something other than what was asked for. This is the only
    check that proves anything.
