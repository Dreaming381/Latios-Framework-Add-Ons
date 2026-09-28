EDITING

You never supply a coordinate. Every structural edit re-runs layout before the
file is written; pass --no-layout to keep the positions that are already there.

NODES

  node add <graph> <Type> [--name N] [--group G] [--property P]
                          [--set Port=value]... [--wire Port=source]...
                          [--feed Port=destination]...

  Prints the new node's short id. `<Type>` may be the class name (MultiplyNode),
  the display title (Multiply), a menu path (Math/Basic/Multiply), or a synonym
  (times). `nodes <pattern>` lists what is available.

    shaderwaitress node add g.shadergraph Multiply --wire A=n42d5 --set B=2

  --property binds a Property node to a blackboard property:

    shaderwaitress node add g.shadergraph Property --property _Mask

  node rm <graph> "<selector>" [--reconnect]
        --reconnect splices the node's first input through to each of its
        consumers, so removing a pass-through does not orphan a branch.

  node set <graph> "<selector>" [--name N] [--group G|none] [--set Port=v]...
                                [--preview on|off]

        --preview collapses or expands the node's preview swatch. Collapsing
        takes ~184 px off the node, which is the cheapest way to make a dense
        graph readable, and the layout accounts for it.

  node replace <graph> "<selector>" --with <Type> [--map Old=New]...
        Keeps every wire whose port name exists on both types and reports the
        ones it had to drop. --map renames a port across the swap.

  node insert <graph> <Type> --after "<selector>" [--from Port] [--port Port]
                             [--out Port] [--set P=v]... [--wire P=src]...
        The inverse of `node rm --reconnect`: splices a new node into what an
        existing one already feeds. Every consumer of the intercepted output is
        moved onto the new node, and the intercepted output becomes its input.

          node insert g.shadergraph Multiply --after "type:VertexColor" --set B=2

        --after takes a selector, so one script can patch every graph that has
        the node in question. --from names the intercepted output when the node
        has more than one; --port names the new node's input (by default the
        first one no --wire is filling).

WIRES

  wire <graph> <src[.Port]> <dst[.Port]>
  unwire <graph> <dst[.Port]>
  unwire <graph> <src[.Port]> <dst[.Port]>

  The `.Port` half may be dropped when the node has exactly one port in the
  needed direction. Wiring into an input that is already fed replaces the old
  wire, because Shader Graph allows only one.

VALUES

  Literals follow the port type: `0.5`, `1,0,0`, `(1, 0, 0, 1)`, `true`. A
  single number fills every component: `--set Tiling=2` means (2, 2).

PROPERTIES

  prop add <graph> <Type> --name "Display name" [--ref _Reference] [--value v]
                          [--declaration per-material|global|hybrid-per-instance]
                          [--promote on|off]
  prop set <graph> <property> [--value v] [--name N] [--ref R] [--exposed|--hidden]
                              [--declaration ...|default] [--promote on|off]
                              [--default white|black|grey|normal-map|linear-grey|red]
  prop rm  <graph> <property> [--with-nodes]

  --declaration is where the property lands in the generated HLSL. Left alone,
  Shader Graph picks UnityPerMaterial for an exposed property and Global
  otherwise. A property that has to receive an Entities Graphics per-instance
  override must be hybrid-per-instance; with the default the override compiles,
  runs, and quietly does nothing. `prop list` always prints the effective
  declaration; `show` prints it only when it was overridden.

  Not every type accepts every declaration -- a texture cannot be
  hybrid-per-instance -- and the allowed set comes from Shader Graph itself, so
  an impossible choice is refused with the real options.

  --default is what a TEXTURE property samples as while nothing is assigned to
  it. It is White unless you say otherwise, and a White texture read as a normal
  map unpacks to a tilted normal, so a normal map slot left on the default
  lights every unassigned material wrongly. `prop list` prints it for every
  texture property, set or not.

  Types: Float, Vector2, Vector3, Vector4, Color, Boolean, Texture2D,
  Texture2DArray, Texture3D, Cubemap, Gradient, SamplerState, Matrix2/3/4,
  VirtualTexture. `prop list` shows what a graph already has.

PROMOTING SUB-GRAPH INPUTS

  prop set    <sub> <property> --promote on|off
  keyword set <sub> <keyword>  --promote on|off

  An ordinary sub-graph property becomes an INPUT PORT on the Sub Graph node and
  never reaches the parent shader. A promoted one is declared on the parent
  instead, so the material really has it and Material.HasProperty is true. The
  node loses the input port for it as a side effect -- that is how you can tell
  which you have, and `prop list` on a sub-graph prints promote= either way.

  Use it when a sub-graph is meant to be dropped into somebody else's graph and
  still surface its own material property, including a hybrid-per-instance one
  for Entities Graphics:

    prop add <sub> Color --name "Tint" --ref _Tint \
        --declaration hybrid-per-instance --promote on

  Shader Graph stores this as the sub-graph's own asset guid rather than a flag,
  so the file must have been imported by Unity once, and only a sub-graph can
  promote. Every property type is promotable except Gradient, Sampler State and
  Virtual Texture.

BLOCKS

  block list <graph>
  block add  <graph> <Descriptor>      e.g. SurfaceDescription.Alpha, or just Alpha
  block rm   <graph> "<selector>"
  target set <graph> surface=transparent --sync-blocks

  Blocks are the graph's outputs, and which ones belong is a function of the
  target and its settings. A graph created opaque and then switched to
  transparent keeps the opaque block list, so there is no Alpha to wire a fade
  into and nothing says so. --sync-blocks adds what the new settings need, and
  `validate` reports the mismatch either way.

  --sync-blocks only ever ADDS. Deciding a block is surplus needs the active
  list for the target, which only the Editor can produce, so a block is never
  removed behind your back -- use `block rm` for that.

GROUPS

  group new <graph> "Title" ["<selector>"]
  group add <graph> <group> "<selector>"
  group clear <graph> "<selector>"
  group rename <graph> <group> "New title"
  group rm <graph> <group> [--with-nodes]

TARGET AND GRAPH SETTINGS

  target show <graph>
  target set <graph> surface=transparent blend=additive cull=off zwrite=off
  settings set <graph> precision=half path="My/Shaders"

  Target keys: surface, blend, renderface, zwrite, ztest, alphaclip,
  castshadows, receiveshadows, materialoverride, lodcrossfade, vfx,
  customeditor. Any raw field name of the target object also works, for
  pipelines this table has never seen.

  renderface is named after what Unity stores: which faces to RENDER, not which
  to cull. renderface=front culls the back, renderface=both culls nothing.
  "cull=off" is accepted as an alias for renderface=both; "cull=front" is
  refused, because it reads as the opposite of what it would do.

BATCHES — the important one

Agents pay per invocation, so put a whole edit session in one script. It runs
against a single in-memory document and is written once; any error aborts the
batch and leaves the file untouched.

  shaderwaitress apply g.shadergraph edits.sw
  shaderwaitress apply g.shadergraph --script -      # read from stdin

Script syntax is the CLI verbs with the graph path removed, one per line.
`--as <label>` binds the thing a line creates so later lines can use `$label`:

  # scroll a texture over time
  prop add Vector2 --name "Speed" --ref _Speed --value 0.1,0 --as speed
  node add Property --property $speed --as speedNode
  node add Time --as time
  node add Multiply --as scroll --wire A=$speedNode --wire B=$time.Time
  node add TilingAndOffset --as uv --wire Offset=$scroll
  node add SampleTexture2D --as sample --wire UV=$uv
  wire $sample.RGBA SurfaceDescription.BaseColor
  group new "Scrolling UV" "id:$scroll,$uv,$time"

Every editing verb works in a script, with the graph path omitted:

  node add|insert|rm|set|replace     wire        unwire
  group new|add|clear|rename|rm      prop add|rm|set
  keyword add|rm|set                 target set  settings set
  block add|rm                       echo <text>

A line ending in a backslash continues on the next one, so a node with many
options stays readable. Blank lines and lines starting with # or // are ignored.

Use --dry-run on any command to see the resulting dense text without writing.

NODE SETTINGS

  node add <graph> <Type> --set DepthSamplingMode=Eye
  node set <graph> "<selector>" --setting ScreenSpaceType=Raw

`--set` accepts a port name or a setting name; `--setting` is the explicit form.
An unknown value is refused with the list of real ones. `node show` and
`nodes --show <Type>` both list a node's settings and their options.

KEYWORDS

  keyword list <graph>
  keyword add <graph> boolean --name "Use Detail" --ref _USE_DETAIL
                              [--definition shaderfeature|multicompile|predefined]
                              [--scope local|global] [--default on|off]
                              [--allow-definition-override on|off]
  keyword add <graph> enum --name "Quality" --entries Low,Medium,High --default Medium
  keyword set <graph> <keyword> [--name N] [--ref R] [--definition D] [--scope S] [--default V]
                                [--allow-definition-override on|off]
  keyword rm  <graph> <keyword> [--with-nodes]

  node add <graph> Keyword --keyword "Use Detail"

  A Keyword node gets On and Off inputs for a boolean keyword, or one input per
  entry for an enum, matching what Shader Graph builds.

  --promote is the Editor's "Promote to final Shader", and only a sub-graph has
  it. See PROMOTING SUB-GRAPH INPUTS above.

  --allow-definition-override is the Editor's "Allow Definition Override", and
  it is ON by default. While it is on, Shader Graph compiles the Keyword node to
  a runtime ternary rather than an #if with keyword permutations, so a branch
  that reads as compile-time is not one and costs both sides at runtime. Turn it
  off for a real #if. `keyword list` reports which one you have.

PORTS

  node port add <graph> "<selector>" --name In --type Vector4 [--direction in|out]
                                     [--shader-name N] [--id N] [--value v]
  node port rm  <graph> "<selector>" --name In

  Most node types build their ports in code and need none of this. Custom
  Function is the exception: its ports are the author's to declare, so the node
  is inert until they exist. Since a Custom Function node is the only way to get
  hand-written HLSL or an #include into a generated shader, this is the entry
  point for that too.

    node add g.shadergraph CustomFunction --set SourceType=String \
        --set FunctionName=Fade --set FunctionBody="Out = In * 2;"
    node port add g.shadergraph "type:CustomFunction" --name In  --type Vector4
    node port add g.shadergraph "type:CustomFunction" --name Out --type Vector4 --direction out

  Removing a port takes its wires with it. A port the node type built in code
  cannot be removed, and says so.

SUB-GRAPHS

  new <path>.shadersubgraph                  # the extension is enough
  new <path>.shadersubgraph --target subgraph

  A subgraph starts with a Sub Graph Output node instead of render targets. Its
  ports are named directly, the way a block descriptor is:

    wire $product OutVector4

  node add <graph> SubGraph --subgraph path/to/Thing.shadersubgraph

  The reference is by Unity asset guid, read from the subgraph's .meta file, so
  the subgraph has to have been imported at least once. The node's ports are
  taken from the subgraph's own properties and output slots.
