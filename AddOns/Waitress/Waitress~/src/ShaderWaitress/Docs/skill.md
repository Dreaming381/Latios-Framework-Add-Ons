---
name: shaderwaitress
description: Read, query, edit and build Unity Shader Graph assets (.shadergraph, .shadersubgraph) from the command line without opening the Editor. Use whenever a task involves inspecting or changing a shader graph file, because loading the raw JSON costs 100k+ tokens and there is no Unity API for editing one.
---

# ShaderWaitress

A CLI for Unity Shader Graph files. Shader Graph assets are JSON, but a trivial
18-node graph is 51 KB across 69 flat objects and real ones run past 400 KB;
about three quarters of that is port objects and 2D coordinates. Never read a
`.shadergraph` file directly — run `shaderwaitress show` on it instead.

## First three commands

```
shaderwaitress show <graph>                 # the whole graph, ~150 lines
shaderwaitress q <graph> "<selector>"       # chainable structural query
shaderwaitress help <topic>                 # format select edit layout catalog recipes ids
```

## Reading a graph

`show` prints one line per entity, inputs only, no coordinates:

```
graph "Ripple" kind=shadergraph nodes=13 blocks=5 edges=15 props=3 precision=single
target t22b9 Universal/Unlit surface=transparent blend=alpha renderface=both ztest=lequal

prop p8e6e Float     "Heat Distortion"  ref=_HeatDistortion =0.4
prop pb5d6 Texture2D "Mask"             ref=_Mask

group g1734 "Soft particles" members=4

node naa20 Property pb2bb "Speed"
node nf3bd Time
node nc21c Multiply         A<-naa20  B<-nf3bd.Time
node na7b9 TilingAndOffset  Tiling=(1, 1)  Offset<-nc21c   @g1734

block b6978 SurfaceDescription.BaseColor  BaseColor<-n45b3

# blocks with no input: b6ca1 b1b80
# clusters: 2
```

- `A<-nc21c` — port `A` is fed by node `nc21c`. The `.Port` half is omitted when
  the source has exactly one output.
- `T=0.25` — an unconnected input and the literal it holds. **Every** unconnected
  input shows its value, including the node type's own default, because that
  value feeds the shader whether or not anyone typed it. (Multiply's B defaults
  to 2, so an untouched Multiply doubles.) `--brief` hides the ones that match
  the type default.
- `DepthSamplingMode:Eye` — a **node setting**: a dropdown or toggle on the node
  body rather than on a port. Written with a colon. These change what the shader
  computes and no wire reveals them, so they are always shown. See below.
- `@g1734` — group membership.
- Short ids (`n42d5`, `p8e6e`, `g1734`) are **stable across invocations**. They
  come from the object id, so edits don't renumber them. Only a node someone adds
  in the Editor can collide with an existing id and lengthen it.

## Querying

Steps separated by `|`; pipelines combine with `+ - &`. One invocation answers
a multi-hop question:

```
shaderwaitress q g.shadergraph "type:Multiply | in | type:Property"
shaderwaitress q g.shadergraph "block:BaseColor | in*"
shaderwaitress q g.shadergraph "is:unreachable + is:orphan"
shaderwaitress q g.shadergraph "prop:_Speed | out*" --format ids
```

Matchers: `all type: name: id: group: prop: block: port: rank: is:` and a bare
word (name-or-type-or-id). Tags for `is:`: `orphan root leaf block property
keyword grouped ungrouped reachable unreachable node`. Traversals: `in out in*
out* in:Port out:Port both`. Globs `*` `?` and `,` alternation everywhere.

## Editing

You never supply a coordinate — the tool owns layout and re-runs it after every
structural edit.

```
shaderwaitress new Fx.shadergraph --target universal/unlit
shaderwaitress node add Fx.shadergraph Multiply --wire A=n42d5 --set B=2
shaderwaitress node rm  Fx.shadergraph "id:n42d5" --reconnect
shaderwaitress node set Fx.shadergraph "type:TilingAndOffset" --set Tiling=2
shaderwaitress node replace Fx.shadergraph "type:Multiply" --with Add
shaderwaitress wire Fx.shadergraph n42d5.RGBA BaseColor
shaderwaitress prop add Fx.shadergraph Float --name "Speed" --ref _Speed --value 1
shaderwaitress group new Fx.shadergraph "Noise" "type:*Noise*"
shaderwaitress target set Fx.shadergraph surface=transparent blend=additive
shaderwaitress node insert Fx.shadergraph Multiply --after "type:VertexColor" --set B=2
```

`node insert` is the inverse of `node rm --reconnect`: every consumer of the
intercepted output moves onto the new node. `--after` takes a selector, so one
line patches every graph that has the node in question.

`renderface=` is named after what Unity stores — which faces to **render**, not
which to cull. `renderface=both` is the two-sided setting particles usually want.

`--dry-run` prints the resulting dense text without writing. `--no-layout`
keeps existing positions.

### Node settings — check these

Several nodes carry their most important choice in a dropdown, not a port. Soft
particles need Scene Depth on `Eye` and Screen Position on `Raw`; a graph with
the defaults compiles, validates, and is silently wrong. `--set` takes either a
port or a setting:

```
shaderwaitress node add Fx.shadergraph SceneDepth     --set DepthSamplingMode=Eye
shaderwaitress node add Fx.shadergraph ScreenPosition --set ScreenSpaceType=Raw
shaderwaitress node show Fx.shadergraph "type:SceneDepth"   # lists valid values
shaderwaitress nodes --show SceneDepth                      # before it exists
```

An invalid value is refused with the real options listed. Settings are found
even on node types the catalog has never seen, straight from the file.

### Keywords and sub-graphs

```
shaderwaitress prop add Fx.shadergraph Color --name "Tint" --ref _Tint \
      --declaration hybrid-per-instance          # or it will not receive a DOTS override
shaderwaitress node port add Fx.shadergraph "type:CustomFunction" --name In --type Vector4
```

A property's **HLSL declaration** decides where it lands in the generated shader.
Left alone Shader Graph picks `UnityPerMaterial` for an exposed property, so an
Entities Graphics per-instance override against it compiles, runs and silently
does nothing. `prop list` always shows the effective declaration; `show` shows it
when it was overridden. Not every type accepts every value — a texture cannot be
per-instance — and the allowed set comes from Shader Graph itself.

**Custom Function nodes have no ports of their own**; they are the author's to
declare with `node port add`, and the node does nothing until they exist. It is
also the only route to hand-written HLSL or an `#include`.

```
shaderwaitress keyword add Fx.shadergraph boolean --name "Use Detail" --ref _USE_DETAIL --definition multicompile
shaderwaitress keyword add Fx.shadergraph enum --name "Quality" --entries Low,Medium,High --default Medium
shaderwaitress node add Fx.shadergraph Keyword --keyword "Use Detail"   # gets On/Off inputs
shaderwaitress keyword set Fx.shadergraph "Use Detail" --allow-definition-override off

shaderwaitress new Thing.shadersubgraph                                 # a sub-graph
shaderwaitress node add Fx.shadergraph SubGraph --subgraph Thing.shadersubgraph
```

A sub-graph reference uses the asset guid from its `.meta`, so Unity must have
imported it once.

**Blocks are the graph's outputs, and they do not follow the target.** A graph
created opaque and switched to transparent keeps the opaque block list, so there
is no `SurfaceDescription.Alpha` to wire a fade into. `validate` reports it. Use `target set … surface=transparent --sync-blocks`, or `block add
<graph> SurfaceDescription.Alpha`. `block list` shows what the graph has and what
its settings imply.

**A texture property samples as White while nothing is assigned**, and a White
texture read as a normal map unpacks to a tilted normal — so every material that
leaves the slot empty lights wrongly. `prop add Texture2D … --default normal-map`
(or `black` for a mask). `prop list` prints it for every texture property.

**A sub-graph property is an input port on the node, not a material property**,
unless you promote it. `prop set <sub> _Tint --promote on` declares it on the
parent shader instead, so `Material.HasProperty` is true and the node loses the
port for it. That is the only way a sub-graph dropped into somebody else's graph
can surface its own property — including a `hybrid-per-instance` one. Keywords
promote the same way. `prop list` on a sub-graph prints `promote=` either way.

**A keyword branch is a runtime branch by default.** "Allow Definition Override"
is on unless you turn it off, and while it is on the Keyword node compiles to a
ternary, not an `#if` — both sides run. Even with it off, Shader Graph still
evaluates the nodes feeding the branch that the `#if` discards; the `#if` only
picks between two already-computed values. To actually skip work, put the `#if`
inside a Custom Function node.

### Batch — prefer this

Put a whole edit session in one invocation. It runs against a single in-memory
document and writes once; any error aborts and leaves the file untouched.
`--as` binds a label so a later line can use `$label`. A line ending in a
backslash continues on the next one.

```
shaderwaitress apply Fx.shadergraph edits.sw
```

Every editing verb works in a script, with the graph path omitted: `node
add|insert|rm|set|replace`, `wire`, `unwire`, `group`, `prop`, `keyword`,
`target set`, `settings set`, `echo`. So a graph that needs a keyword and a
target change is still one invocation.

```
prop add Vector2 --name "Speed" --ref _Speed --value 0.1,0 --as speed
node add Property --property $speed --as speedNode
node add Time --as time
node add Multiply --as scroll --wire A=$speedNode --wire B=$time.Time
node add TilingAndOffset --as uv --wire Offset=$scroll
node add SampleTexture2D --as sample --wire UV=$uv
wire $sample.RGBA SurfaceDescription.BaseColor
group new "Scrolling UV" "id:$scroll,$uv,$time"
```

## Discovering node types

```
shaderwaitress nodes "*noise*"          search the catalog
shaderwaitress nodes --show Multiply    one node's ports
shaderwaitress nodes --starters         templates for 'new'
```

A type may be named by class (`MultiplyNode`), title (`Multiply`), menu path
(`Math/Basic/Multiply`) or synonym (`times`).

## Finishing

```
shaderwaitress layout <graph> --report      # objective before/after quality score
shaderwaitress validate <graph> --editor    # structural checks, then a real compile
```

`validate` catches orphaned objects, wires to ports that do not exist, two
wires into one input, Property nodes with no property, blocks missing from
their context, cycles, and layout problems. `--editor` also compiles the graph
in the running Editor and reports the shader's errors and warnings.

## The graph may be open in the Editor

That's fine. With the "ShaderWaitress Editor Bridge" sample imported, every
write saves the Shader Graph window's unsaved edits first, then has the window
reload the result without a dialog. You don't need to do anything. `--offline`
turns it off.

## Guarantees worth knowing

- A read/write round trip with no edits is **byte-identical**. The tool never
  reformats what it didn't change, so diffs contain only the edit.
- A created node is the exact JSON the Editor writes for that type, cloned from
  a catalog exported from a real Unity Editor.
- Partial, disconnected, cyclic, and badly laid out graphs all load without
  complaint. Layout only happens when you ask for it or after an edit.
- **Adding to a graph a human arranged doesn't rearrange it.** When the
  existing layout is sound, only the new nodes get placed and every other node
  stays exactly where it was.
- **A worse layout is never written.** If the arrangement already in the file
  scores better than anything the tool computes, the file is left alone and the
  tool says so. `layout --force` still won't write a worse one.

## When NOT to use it

Seeing a preview or judging how a shader looks still needs the Editor. This
tool is about the graph's structure.

## Getting the binary and catalog

The tool ships in the Latios Framework Addons package, under
`AddOns/Waitress/Waitress~/` (the trailing `~` keeps Unity from importing it).
It shares a solution with VfxWaitress. Build it once from the project root with
the .NET SDK. It has no package dependencies:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The node catalog lives in the project's `UserSettings/Waitress/nodes.json`. With
the "ShaderWaitress Editor Bridge" sample imported, export it with
`unity command shaderwaitress_export_catalog`. The tool warns on stderr when it
can't find one. `Waitress~/docs/ShaderWaitress-README.md` covers the rest.
