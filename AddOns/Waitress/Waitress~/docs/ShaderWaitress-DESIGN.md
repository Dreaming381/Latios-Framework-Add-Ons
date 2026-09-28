# ShaderWaitress Design

This document covers how ShaderWaitress works and why. For what the tool does and
how to use it, see `ShaderWaitress-README.md` and the tool's own `help`.

## The Problem

Shader Graph assets are JSON, but they're a poor fit for an agent:

-   **Size.** A trivial 18-node graph is 51 KB and 69 objects. Real production
    graphs pass 400 KB, which is more than 100k tokens.
-   **Fanout.** Every port of every node is its own top-level object with a
    32-hex `m_ObjectId`. About 75% of the bytes are ports and layout data that
    say nothing about what the shader does.
-   **Indirection.** Nothing is nested. Finding out what feeds Base Color means
    chasing object IDs through a flat, sorted list.
-   **No API.** Shader Graph has no public authoring API, so there's no supported
    way to script a change.
-   **Layout.** Node positions are stored in the file, and a graph written
    carelessly opens as a pile of overlapping boxes.

## Goals

1.  Read and write the file format losslessly, byte-identical when nothing
    changed.
2.  Show a graph as dense text that keeps everything that affects the shader.
3.  Answer structural questions in one call with chainable queries.
4.  Edit graphs without the caller ever handling a coordinate or an object ID.
5.  Own layout, and produce graphs a human is happy to open.
6.  Handle partial, disconnected, and messy human-authored graphs.
7.  Teach itself to an agent through `help` alone.

Compiling shaders and checking HLSL are not goals. The Editor does that.

## The File Format

Shader Graph calls its format MultiJson. Everything here comes from reading
`Editor/Serialization/MultiJson*.cs` in `com.unity.shadergraph@17.3.0`.

-   A file is a sequence of pretty-printed JSON objects separated by `"\n\n"`,
    with a trailing `"\n\n"`. It's not a JSON array, and the file as a whole is
    not valid JSON.
-   Every object has `m_SGVersion`, `m_Type`, and `m_ObjectId` (32 lowercase hex
    characters).
-   The root object comes first. **Everything else is sorted by `m_ObjectId`**,
    ordinal ascending. Matching this order is what keeps diffs clean.
-   Objects reference each other with `{"m_Id": "<objectId>"}`. An empty `m_Id`
    means null.
-   Objects are printed the way `JsonUtility` prints them: 4-space indent, `": "`
    between key and value, and `[]` for empty arrays.
-   Unity keeps objects with an unknown `m_Type` verbatim, so the tool does too.

Files from before Shader Graph 10.0 use an older format with no object IDs. A few
still ship inside URP. Unity upgrades them on import. The tool recognizes them
and says so instead of failing.

### The Object Model

| Object | Role |
| --- | --- |
| `GraphData` | The root. Nodes, edges, properties, keywords, dropdowns, categories, groups, sticky notes, contexts, targets, and settings |
| `*Node` | A node. `m_Name`, `m_Group`, `m_DrawState.m_Position`, `m_Slots`, plus fields specific to its type |
| `*MaterialSlot` | A port. Its integer `m_Id` is **local to its node**. `m_SlotType` is 0 for input and 1 for output |
| `GroupData` | A labeled group. Membership lives on each node's `m_Group`, not on the group |
| `StickyNoteData` | A free text box |
| `CategoryData` | A blackboard section |
| `*ShaderProperty`, `ShaderKeyword`, `ShaderDropdown` | Blackboard inputs |
| `UniversalTarget`, `*SubTarget` | The render pipeline target and its settings |
| `BlockNode` | An output block. Membership lives in `GraphData.m_VertexContext` and `m_FragmentContext` |

Edges only live in `GraphData.m_Edges`, as
`{outputSlot:{node,slotId}, inputSlot:{node,slotId}}`. Those are slot IDs, not
object IDs.

A sub-graph uses the same `GraphData` root. The difference is that it names an
output node instead of carrying render targets.

### Two Facts That Make Authoring Possible

1.  When Shader Graph loads a graph, it calls `UpdateNodeAfterDeserialization()`
    on every node, and each node type calls `AddSlot(...)` for every port it
    expects. `AddSlot` keeps an existing slot with the same ID and creates the
    rest. **So a node written with no slots at all is legal.** Shader Graph
    rebuilds them with their default values. Slots only need writing when a
    value differs from the default.
2.  Shader Graph measures nodes in its UI and writes the size into
    `m_DrawState.m_Position`. It's the best size data there is, so the tool uses
    it when present and estimates otherwise. Block nodes always store a zero
    rect, because their context stacks them.

## Fidelity

The tool never reformats what it didn't change.

-   Parsing splits the file into entries of object ID, type, and raw text.
-   An entry's JSON only gets parsed when something reads or writes it.
-   On save, untouched entries are written from their raw text. Touched entries
    go through a writer that mimics `JsonUtility`. Numbers nobody assigned are
    written from their original text via `JsonElement.GetRawText()`, so float
    formatting survives exactly.
-   Unity always writes LF, but a checkout can turn a file into CRLF. Parsing
    works on LF and the original line endings come back on save.

`roundtrip` checks all of this against every graph it can find, both untouched
and with every object forced through the writer. A mismatch in the second mode
means the file isn't in the exact format Unity writes, which is common for
hand-edited files. Those still round-trip untouched, and they'd only be
normalized if the tool edited them.

## Short IDs

32-hex object IDs are unreadable and cost tokens. Every entity gets a **short
ID** instead: a kind letter plus the first four hex characters of its object ID.
Only IDs that collide get extended, one character at a time.

```
n42d5  node        b6978  block        g1734  group
p8e6e  property    k0a1c  keyword      t22b9  target
```

Short IDs are **stable**. They come only from the object ID, so adding or
removing a node doesn't renumber anything else, and an ID from one call still
works in the next. When the tool creates an object, it picks an object ID whose
first four characters nothing else in the file starts with, so a new node never
collides and nothing existing has to grow. Only a node added in the Editor can
still collide, and then both IDs get a character longer. Any command that takes
an entity also accepts the full object ID, an unambiguous node name, a block
descriptor, or a property's name or reference. `help ids` lists them all.

The same scheme is shared with VfxWaitress through `Waitress.Common`, except
VfxWaitress uses the tail of the ID, since Unity file IDs in one file share a
long common prefix.

## Dense Text

This is what `show` prints. It's one line per entity, inputs only, with literals
inline and no coordinates. `help format` is the reference. The key choices:

-   `A<-nf3bd.Time` means port `A` is fed by the `Time` port of `nf3bd`. The
    `.Time` part is dropped when the source has only one output.
-   Property and Keyword nodes print the blackboard item they read, so a wire can
    point at the node and the reader still knows which property it is.
-   **Every unconnected input prints its value**, even when it matches the node
    type's default. That value feeds the shader whether anyone typed it or not.
    Multiply's B defaults to 2, so hiding it would hide a doubling. `--brief`
    hides defaults for pure structure scans.
-   Node settings print with a colon, before the ports: `DepthSamplingMode:Eye`.
    They change what the shader computes and no wire shows them.
-   Port names print without spaces so each field is one token. Both spellings
    work as input.
-   Blocks come last, in context order.
-   A trailing comment block lists dead branches, unconnected nodes, blocks with
    no input, the cluster count, cycles, and load warnings.

## Selectors

A selector is a pipeline of steps separated by `|`. The first step picks a set of
nodes, and each later step filters it or walks the wires. Whole pipelines
combine with `+`, `-`, and `&`. `help select` has the full list of matchers,
`is:` tags, and traversals.

```
shaderwaitress q graph.shadergraph "type:Multiply | in | type:Property"
```

Anywhere a command takes a node, it takes a selector, so one `node set` can
retune every node a query finds.

## Editing

No command takes a coordinate. `help edit` documents every verb. A few design
points worth knowing:

-   `node rm --reconnect` splices the node's first input through to its
    consumers. `node insert` is the reverse, and since its `--after` takes a
    selector, one script can patch every graph that has a certain node.
-   `node replace` keeps every wire whose port name exists on both types, and
    reports the ones it had to drop.
-   Property, Keyword, and SubGraph nodes have no ports until they're bound to
    what they read, so binding always happens before any port gets resolved.
-   A sub-graph node is referenced by the asset guid in its `.meta` file, so the
    sub-graph has to have been imported by Unity at least once. The same goes for
    promoting a sub-graph input, since Shader Graph stores promotion as the
    sub-graph's own guid.
-   `--sync-blocks` only ever adds blocks. Deciding a block is extra would need
    the target's active block list, and only the Editor can produce that.
-   A new slot is written with only the fields that differ from the slot class's
    defaults. Shader Graph loads slots with `FromJsonOverwrite` onto a
    default-constructed slot, so missing fields keep their code defaults.

### Batches

Agents pay per call, so every edit verb also works in a script:

```
shaderwaitress apply graph.shadergraph script.sw
shaderwaitress apply graph.shadergraph --script -    # read from stdin
```

The whole script runs against one in-memory document and gets written once. Any
error aborts the batch and leaves the file untouched. `--as <label>` binds what a
line creates so later lines can refer to it as `$label`.

## The Node Catalog

Creating a node means knowing its ports, and node types define their ports in
C#. So the catalog is exported by reflection from inside the Editor and shipped
as data.

The exporter, `ShaderWaitressCatalogExporter.cs` in the ShaderWaitress Editor
Bridge sample, creates an instance of every concrete node type with a `[Title]`
attribute, every block descriptor, every property type, and every keyword type,
and serializes each one the way Shader Graph would. It writes
`UserSettings/Waitress/nodes.json` with, per type:

-   the type name, menu path, and synonyms
-   the ports, read back out of the serialized template
-   the settings, with enum names
-   how many control rows the node draws, which is fewer than its settings when
    some settings have no control (Sample Texture 2D has four settings and two
    controls)
-   the exact JSON of a fresh instance

It also writes whole-file starter templates for `new`. The exporter goes through
reflection, so it would work from any Editor assembly.

`UserSettings/` is per project and per machine, stays writable however the
add-on was installed, and is excluded by Unity's default `.gitignore`. The
catalog is regenerated in seconds, so there's no reason to share it. The tool
looks for it in this order: `--catalog <path>`, `$SHADERWAITRESS_CATALOG`, then
`UserSettings/Waitress/nodes.json` in the project found by walking up from the
first path argument, or from the working directory when there isn't one.

Adding a node clones its template with fresh object IDs. The template's zero
draw state is removed, so layout sees a new node as unplaced instead of placed at
the origin. Block nodes keep it, since their context positions them and Shader
Graph writes the zero rect for them too.

## The Editor Link

Shader Graph's window keeps its own copy of the graph. A file rewritten behind
its back triggers a "Graph has changed on disk" dialog on the window's next
update, and when Unity runs with `-automated`, nobody can answer it. So writes go
through the ShaderWaitress Editor Bridge sample, which registers
`shaderwaitress_*` commands with the Pipeline package:

-   **Prepare**, before reading the file for an edit. If a window showing the
    graph has unsaved edits, it calls the window's `SaveAsset`. The tool then
    edits what the person sees, and their work is kept. "Unsaved edits" means
    the window's asterisk (`hasUnsavedChanges`) or a graph object marked dirty
    by an edit the window hasn't processed yet. It doesn't mean
    `GraphHasChangedSinceLastSerialization`, which is also true when Shader
    Graph would merely serialize the same graph differently than the file.
-   **Refresh**, after writing. It notes which windows have unsaved edits, then
    imports the file and sets every other window's `graphObject` to null. The
    window's `Update` reloads a missing graph from disk before it checks for
    changes on disk, so the dialog never comes up. This is the same path the
    window's own "Reload" button takes. The check has to come first: the import
    makes each window compare its graph to the new file, and after that even an
    untouched window looks edited. A window that gained edits after Prepare is
    left alone to ask.

    Before importing, Refresh also loads the file the way the window does and,
    if Shader Graph would save it differently, writes Shader Graph's own form.
    Otherwise the reloaded window compares its graph to a file in another form,
    shows an asterisk, and asks to save when closed.
-   **Compile**, for `validate --editor`. It imports the graph and reports
    `ShaderUtil.GetShaderMessages` for the generated shader.

All of these members are `internal` to `Unity.ShaderGraph.Editor`, so the
sample's `.asmref` compiles the Editor link into that assembly, and the Pipeline
commands sit in a separate asmdef that only compiles when the Pipeline package is
installed. That's a small, stable surface, about as much as a Shader Graph
window's own menu uses, rather than driving the graph itself through
reflection.

## Layout

The tool owns every position and re-runs layout after any structural edit,
unless the caller passes `--no-layout`.

### Units

First the graph is reduced to layout units, since not everything on screen moves
on its own:

-   One unit per ordinary node.
-   One unit per labeled group, so a group gets laid out as a whole and nothing
    can interleave with it.
-   One unit per vertex or fragment context. Shader Graph stacks a context's
    blocks itself and ignores their stored rects, so the tool positions the
    context and works out the block rects from it.

Group membership also counts as a connection. Otherwise a group whose members
share no wire would get laid out once per piece, and its bounding box would
stretch across the gap and swallow whatever sat between.

### Pipeline

1.  **Try incremental placement.** If the existing layout is sound and only some
    nodes lack a position, place just those. A new node goes one gutter left of
    its earliest consumer, or right of its producers, then slides down until it
    clears every node and every foreign group box. A node joining a group lands
    beside the group's other members. Several starting heights are tried and the
    best is kept. This only counts if every new node lands without an overlap or
    a backward wire.
2.  **Split into components.** Weakly connected pieces get laid out separately.
    The piece that reaches the outputs goes first, and the rest are stacked below
    in the order they already sit, so an addition appends instead of reshuffling.
3.  **Rank.** Longest path from the outputs, so a node sits one column left of
    the deepest thing it feeds. Output contexts are pinned to the rightmost
    column, as is a sub-graph's output node. Two groups that feed each other form
    a cycle between their units. The ranker breaks cycles with a depth-first
    search, and the tool reports which groups caused the backward wire, since
    regrouping is the only real fix.
4.  **Insert dummies.** A wire spanning several columns gets a dummy per column,
    so it travels through an empty lane instead of over nodes.
5.  **Order each column.** Median barycenter sweeps plus adjacent swaps. A
    neighbor's position is its index plus how far down it the wire attaches, so
    two wires between the same pair of nodes get ordered by port. Two starting
    orders are optimized, the order already in the file and plain insertion
    order, and the better one wins.
6.  **Assign coordinates.** X comes from column widths plus a gutter. Columns are
    right-aligned, so outputs line up. Y comes from repeated median alignment on
    port positions, not node centers, with a hard minimum gap.
7.  **Expand groups and contexts.** Each group's internal layout is placed inside
    its unit's box. Contexts go to the right edge with Vertex above Fragment, the
    way Shader Graph places them. The two stacks line up by where the blocks
    draw, not by where the unit boxes start.
8.  **Refine.** A hill-climb over the penalty model below. It tries re-sorting
    each column by the real heights of what the nodes connect to, swapping
    neighbors, moving a node into a nearby column, opening a lane for a wire
    drawn across a node, small vertical nudges, and the shift that makes a
    node's own wires level. A node travels with any source that feeds only it,
    so a sampler and its texture property move together. Last of all, each node
    slides right as far as its consumers allow, which rescues a property that
    ranking stranded columns away from its only consumer.
9.  **Choose.** Steps 2 through 8 run with port-aware ordering on and off, and
    while anything still crosses, the best result seeds another round. Then the
    best full layout is compared against the incremental placement from step 1
    and against a repair of the existing layout, which only slides nodes out
    from under wires. Either of those wins unless the full layout is
    dramatically better, because rearranging a graph someone has already read
    costs more than the score can see. If the layout already in the file scores
    better than all of them, nothing is written. `--force` skips incremental
    placement, but still never writes a worse layout.

Above 60 nodes, the searching refinement passes stop trying every option,
because each move is judged by re-measuring the whole graph. A large graph can
end up short of its best arrangement, but never worse than it started.

### The Penalty Model

Shader Graph draws its own wires and the tool can't route them. Only node
positions are the tool's to choose. So readability is expressed as a penalty and
minimized by moving nodes:

| Term | Weight |
| --- | --- |
| Node overlap | 5000 each |
| Overlapping group boxes | 2000 each |
| Wire running right to left | 1000 each |
| Foreign node inside a group box | 800 each |
| Value editor drawn over a node | 500 each |
| Three crossings within 24 px | 400 each |
| Vertex and fragment stacks in different columns | 300 once |
| Crossing at less than 25 degrees | 250 each |
| Wire drawn over a node | 200 each |
| Any crossing | 100 each |
| Empty band inside a group box | 0.5 per px |
| Total wire length | 0.01 per px |

A wire over a node outranks a crossing because it hides part of the node. At 200,
the layout pays one crossing to lift a wire off a node but not two. A value
editor outranks both, because it's an opaque box and hides what it lands on.

Node overlaps, group overlaps, foreign nodes in groups, and crowding are hard
rules as well as penalties. Refinement never takes a move that breaks one of
them, however much it improves the total. Crowding means two boxes closer than
24 px. They don't technically overlap, but a stored size can be older than a
dropdown or preview turned on since, so they can overlap on screen.

The empty-band term exists because Unity draws a group box around whatever its
members span. One member far from the rest shows up as a big empty rectangle,
while the wires stay short and nothing overlaps or crosses. Only gaps wider than
normal spacing count, so a group laid out as a chain scores zero however wide it
is.

`layout --report` prints the breakdown before and after, and `--verbose` names
every crossing and every wire drawn over a node.

### Geometry

Every wire endpoint is a node box plus a port offset, so an error in either one
shows up on every wire in the graph. These numbers were measured off a live
Shader Graph window.

**Wires.** A wire is a cubic curve with horizontal tangents at both ends. The
tangent is a fixed 28 px however long the wire is, so a wire is almost the
straight line between its ports, bending only at the ends. Crossings are counted
whenever two wires meet anywhere other than a shared port. Two wires leaving
different outputs of one node, or entering different inputs of one node, still
cross whenever their far ends are in the opposite order.

**Ports.** A connector sits 16 px inside the node edge. Rows start below the
header, so the port in row `i` is centered at `46 + 24 * (i + 0.5)` px. An input
shares its row with the output beside it.

**Node heights.** Nodes the Editor has never drawn are estimated. The header is
46 px, each row is 24 px, and the bottom padding is 7 px. The row count is the
longer of the input and output lists, not their total. The strip holding the
preview's collapse arrow adds 17 px, and a visible preview adds 184 px. Controls
add 8 px of padding plus 26 px each, and only settings that draw a control
count.

**Other shapes.** Block nodes are 200 by 41. Redirect dots are 56 by 24. A
property token is one 34 px row whose width follows its label, including the type
suffix like `(1)`. That width estimate deliberately comes in under every measured
width. The token's right edge is where its wire starts, and guessing too far right
makes the tool expect a steeper wire than Shader Graph draws. A keyword dropped
into a graph is a full node with a port per entry and a preview, not a token.

**Value editors.** Every unconnected input draws a value editor 220 px to the
left of its node, at that port's row, and it vanishes once the port is wired.
It measures x = -224, w = 232, h = 22 on every slot type. Layout treats that
space as occupied.

**Contexts.** A context is 224 px wide. It draws its first block 2 px below its
top, stacks blocks with no gap between them, and leaves 46 px below the last one
for the add-block button. An unconnected block input draws its value editor
220 px left of the context, so each stack reserves a lane on its left as wide as
the widest editor any of its blocks needs.

**Sizes are never written back.** Only the Editor can measure a node, so layout
writes a position and leaves the size alone. A guessed size written to the file
would be trusted by the next run as if Unity had measured it. `--remeasure`
ignores stored sizes and estimates everything, which helps after changing a
node's ports.

### Messy Input

Human-authored graphs break every rule above. They have backward wires,
overlapping nodes, and nodes far outside their groups. All of that loads without
complaint. Layout only runs when asked for or after an edit, and `--no-layout`
always opts out.

## Documentation

-   `help` prints the command index, and `help <topic>` covers `format`,
    `select`, `edit`, `layout`, `catalog`, `recipes`, and `ids`.
-   `skill` prints a complete one-page guide for agents, and `skill --install
    <dir>` writes it as a `SKILL.md` for agent harnesses that support
    installable skills.

All help text lives in `src/ShaderWaitress/Docs/` and is embedded in the binary.
