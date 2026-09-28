RECIPES

Understand a graph you have never seen
  shaderwaitress show Fire.shadergraph

What actually drives the output
  shaderwaitress q Fire.shadergraph "block:BaseColor | in*"

Find dead work
  shaderwaitress q Fire.shadergraph "is:unreachable + is:orphan"

Everything a property feeds
  shaderwaitress q Fire.shadergraph "prop:_Speed | out*"

See a node's ports before wiring it
  shaderwaitress nodes --show TilingAndOffset
  shaderwaitress node show Fire.shadergraph "id:n42d5"

Make an unlit transparent graph from scratch
  shaderwaitress new Fx.shadergraph --target universal/unlit
  shaderwaitress target set Fx.shadergraph surface=transparent blend=additive

Set up a graph the way a particle effect wants it
  shaderwaitress target set Fx.shadergraph surface=transparent blend=additive \
        renderface=both zwrite=off castshadows=off receiveshadows=off

  A new graph starts on Unity's own defaults, which are opaque and single-sided.
  This line is the usual first edit; put it at the top of an apply script rather
  than running it separately.

Splice a node into an existing wire, in every graph that needs it
  shaderwaitress node insert Fx.shadergraph Multiply --after "type:VertexColor" --set B=2

  Every consumer of the Vertex Color node moves onto the Multiply. Because
  --after takes a selector, the same line patches any graph with that node.

Scroll a texture over time, in one invocation
  shaderwaitress apply Fx.shadergraph scroll.sw

  where scroll.sw is:

    prop add Vector2 --name "Speed" --ref _Speed --value 0.1,0 --as speed
    prop add Texture2D --name "Main Tex" --ref _MainTex --as tex
    node add Property --property $speed --as speedNode
    node add Property --property $tex --as texNode
    node add Time --as time
    node add Multiply --as scroll --wire A=$speedNode --wire B=$time.Time
    node add TilingAndOffset --as uv --wire Offset=$scroll
    node add SampleTexture2D --as sample --wire Texture=$texNode --wire UV=$uv
    wire $sample.RGBA SurfaceDescription.BaseColor
    wire $sample.A SurfaceDescription.Alpha
    group new "Scrolling UV" "id:$scroll,$uv,$time,$speedNode"

Swap every Multiply for an Add
  shaderwaitress node replace Fx.shadergraph "type:Multiply" --with Add

Retune many nodes at once
  shaderwaitress node set Fx.shadergraph "type:TilingAndOffset" --set Tiling=2

Delete a pass-through without breaking the chain
  shaderwaitress node rm Fx.shadergraph "id:n42d5" --reconnect

Tidy a graph a human made a mess of
  shaderwaitress layout Fx.shadergraph --report

Check before handing the file back
  shaderwaitress validate Fx.shadergraph

Preview an edit without touching the file
  shaderwaitress node add Fx.shadergraph Lerp --dry-run

Make a property that can receive an Entities Graphics override
  shaderwaitress prop add Fx.shadergraph Color --name "Mesh Color" --ref _MeshColor \
        --declaration hybrid-per-instance

  Without this the property is UnityPerMaterial and the override silently does
  nothing. `prop list` shows the effective declaration for every property.

Add hand-written HLSL
  shaderwaitress apply Fx.shadergraph fn.sw

  where fn.sw is:

    node add CustomFunction --as fn \
        --set SourceType=String --set FunctionName=Fade \
        --set FunctionBody="Out = In * Amount;"
    node port add $fn --name In     --type Vector4 --direction in
    node port add $fn --name Amount --type Float   --direction in
    node port add $fn --name Out    --type Vector4 --direction out
    wire $fn.Out SurfaceDescription.BaseColor

  A Custom Function node has no ports until you declare them, and it is the only
  way to get an #include or hand-written HLSL into a generated shader.

Turn a graph transparent and keep it working
  shaderwaitress target set Fx.shadergraph surface=transparent blend=alpha --sync-blocks

  The block list does not follow the target on its own, so without --sync-blocks
  the graph keeps the opaque blocks and there is no SurfaceDescription.Alpha to
  wire a fade into. `validate` reports the mismatch; `block list` shows it.

  --sync-blocks only adds. `block rm` is the way to take one away.

Stop an unassigned normal map from tilting every normal
  shaderwaitress prop add Fx.shadergraph Texture2D --name "Normal Map" --ref _BumpMap \
      --default normal-map

  A texture property samples as White until something is assigned, and White read
  as a normal map unpacks to a tilted normal. Use black for a mask. `prop list`
  prints the default for every texture property, which is how you spot the ones
  still on White.

Give a sub-graph its own material property
  shaderwaitress prop add Fade.shadersubgraph Color --name "Tint" --ref _Tint \
      --declaration hybrid-per-instance --promote on

  Without --promote the property becomes an input port on the Sub Graph node and
  never reaches the parent shader, so `Material.HasProperty("_Tint")` is false
  and there is nothing for a per-instance override to bind to. With it, the
  property is declared on the parent and the port disappears.

  Shader Graph stores promotion as the sub-graph's own asset guid, so the file
  has to have been imported by Unity once. Everything promotes except Gradient,
  Sampler State and Virtual Texture.

Branch on a keyword and actually skip the work
  shaderwaitress keyword add Fx.shadergraph boolean --name "Use Detail" --ref _USE_DETAIL \
      --definition multicompile --allow-definition-override off
  shaderwaitress node add Fx.shadergraph Keyword --keyword "Use Detail"

  Two separate traps here. "Allow Definition Override" is ON by default, and
  while it is on the Keyword node compiles to a runtime ternary instead of an
  #if — so turn it off, and check with `keyword list`.

  Even then, Shader Graph does not strip the nodes feeding the branch that the
  #if discards. Both sides are still evaluated and the #if only picks between
  the two results, so gating an expensive subtree — a texture sample, a noise
  chain — off a Keyword node saves nothing. Put the #if inside a Custom Function
  node instead, with the expensive work inside it.
