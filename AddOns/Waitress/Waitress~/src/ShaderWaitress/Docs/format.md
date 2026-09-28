THE DENSE TEXT FORM

`show` prints one line per entity, inputs only, no coordinates. Outputs are
implied by the other end of each wire, so nothing is repeated.

  graph "Ripple" kind=shadergraph nodes=13 blocks=5 edges=15 props=3 precision=single path="Shader Graphs"
  target t22b9 Universal/Unlit surface=transparent blend=alpha renderface=both ztest=lequal

  prop p8e6e Float     "Heat Distortion"  ref=_HeatDistortion =0.4
  prop pb2bb Vector2   "Speed"            ref=_Speed          =(0.1, 0)
  prop pb5d6 Texture2D "Mask"             ref=_Mask

  group g1734 "Soft particles" members=4

  node naa20 Property pb2bb "Speed"
  node nf3bd Time
  node nc21c Multiply         A<-naa20  B<-nf3bd.Time
  node na7b9 TilingAndOffset  Tiling=(1, 1)  Offset<-nc21c   @g1734
  node n45b3 SceneColor       UV<-n95d4

  block b6978 SurfaceDescription.BaseColor  BaseColor<-n45b3
  block bf614 SurfaceDescription.Alpha      Alpha<-nd0c6.A

  # blocks with no input: b6ca1 b1b80 ba456
  # clusters: 4

HOW TO READ A NODE LINE

  node <id> <Type> [<extra>] ["Custom name"] <inputs...> [@group]

  A<-nc21c        port A is fed by node nc21c
  A<-nd0c6.RGBA   port A is fed by the RGBA output of nd0c6
  T=0.25          port T is unconnected and holds a literal
  (omitted)       port is unconnected and still at its default
  @g1734          the node belongs to that group

  The `.Port` half of a source is omitted whenever the source node has exactly
  one output, which covers most nodes. Both spellings are accepted on input.

  Port names are printed without spaces ("BaseColor", not "Base Color"). Either
  form is accepted when you type one.

  `<extra>` is the blackboard item a Property or Keyword node reads, the
  sub-graph guid of a SubGraph node, or the function name of a Custom Function
  node.

THE TRAILING COMMENT BLOCK

  # not feeding any block: <ids>   dead branches
  # unconnected: <ids>             nodes with no wires at all
  # blocks with no input: <ids>    outputs still at their default
  # clusters: N                    weakly connected components
  # cycles: N edge(s) close a loop Shader Graph does not allow this
  # warning: ...                   something did not resolve while loading

OPTIONS

  --outputs            also print Port->dst.Port for every outgoing wire
  --include-defaults   print unconnected inputs even at their default
  --positions          append #(x, y WxH) to each node
  --notes              include sticky notes

WHAT IS DELIBERATELY NOT SHOWN

2D positions, node sizes, object ids, slot objects, categories and preview
state. None of it changes what the shader computes, and together it is roughly
three quarters of the bytes in the file. Use `--positions` when you are
specifically checking layout.

NODE SETTINGS

Some of a node's controls are drawn on its body rather than on a port: Scene
Depth's sampling mode, Screen Position's space, Sample Texture 2D's type, a
Color node's colour. They change what the shader computes and no wire reveals
them, so they are printed with a colon, before the ports:

  node n21df SceneDepth      DepthSamplingMode:Eye  UV=(0, 0, 0, 0)
  node n6c8b ScreenPosition  ScreenSpaceType:Raw
  node n5436 Color           Color:(1, 0.5, 0, 1)  Color.mode:0

`node show <graph> "<selector>"` lists them with their allowed values, and
`nodes --show <Type>` does the same before the node exists. Set one with
`--set Name=Value` or `--setting Name=Value`; an invalid value is refused and
the real options are listed.

Settings are also found on node types the catalog has never seen, straight from
the serialized object. Those show raw numbers instead of names, but they are
still visible and still settable.

WHY EVERY LITERAL IS SHOWN

An unconnected input feeds the shader whether or not anyone typed the number.
Multiply's B defaults to 2, so an untouched Multiply doubles. If defaults were
hidden, that doubling would be invisible. So every unconnected input prints its
value, including the node type's own default.

`--brief` hides values that match the type default. Use it when scanning
structure, not when checking what a graph computes.

HLSL DECLARATION ON A PROPERTY

Nothing in the graph shows where a property gets declared in the generated
shader, and the default isn't what a DOTS project wants:

  prop p2c50 Color "Mesh Color" ref=_ShurikenMeshColor =(1, 1, 1, 1) decl=hybrid-per-instance

`show` prints `decl=` only when the declaration was overridden, because the
default is derived rather than stored — Shader Graph picks UnityPerMaterial for
an exposed property and Global otherwise. `prop list` always prints the
effective value and marks the derived one:

  p2c50 Color  "Mesh Color" ref=_ShurikenMeshColor decl=hybrid-per-instance nodes=1
  p910f Float  "Fade"       ref=_Fade              decl=per-material (default) nodes=0

This matters because an Entities Graphics per-instance override against a
per-material property compiles, runs, and does nothing at all.
