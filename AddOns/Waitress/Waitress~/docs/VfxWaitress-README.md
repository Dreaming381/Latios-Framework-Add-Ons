# VfxWaitress

VfxWaitress is a command-line tool for querying, editing, and building Unity VFX
Graph assets (`.vfx`, `.vfxoperator`, and `.vfxblock`) without opening the
Editor. It's built for AI agents, and it's ShaderWaitress's sibling.

VFX Graph assets are Unity YAML, and most of the bytes are slot objects. A
nine-node effect is 2500 lines, and the `Multiply QVVS.vfxoperator` that ships
with LifeFX is 6907 lines for one node wrapping one HLSL function. There's no
public authoring API either. VfxWaitress prints a graph in a dozen lines, answers
structural questions in one call, and edits graphs without the caller ever
handling a file ID. And because VFX Graph can fail without saying anything, it
knows how to ask the Editor whether the result actually compiles.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

All you need is the .NET SDK. There are no NuGet packages. The binary is
`UserSettings/Waitress/build/bin/VfxWaitress/release/vfxwaitress[.exe]`, and the same build
makes ShaderWaitress. `--artifacts-path` keeps the build out of the package
folder, which may be read-only.

## Start Here

```
vfxwaitress show <vfx>               # the whole graph, densely
vfxwaitress q <vfx> "<selector>"     # chainable structural query
vfxwaitress apply <vfx> <script>     # a whole editing session in one call
vfxwaitress validate <vfx> --editor  # the only check that proves the graph compiles
vfxwaitress help                     # command index
vfxwaitress skill                    # the whole tool on one page, for an agent
```

`vfxwaitress help <topic>` covers `format`, `select`, `edit`, `catalog`, `ids`,
and `editor`.

## Editing in One Call

Every write makes a few Editor round trips, so put a whole session in one
`apply`. A script is the edit verbs with the graph path left out. `--as` names
what a line creates, and later lines use it as `$label`:

```
node add Spawn --as spawn
block add $spawn "Constant Spawn Rate"
node add "Initialize Particle" --as init
node add "Update Particle" --as update
node add "Output Particle Unlit Quad" --as output
link $spawn $init
link $init $update
link $update $output
block add $init "Set Color" --as color
node add "Random Color" --feed $color._Color
```

That builds a working effect in one call, which writes the file once. If any line
fails, the file isn't touched. New nodes are placed once all the wiring is done:
contexts stack below whatever flows into them, and operators go left of what they
feed, level with the port they feed. Nothing that was already there moves.

## What It Looks Like

```
$ vfxwaitress show Assets/Validation/LifeFX/LifeFXSpawnEvents.vfx   # 2511 lines of YAML
graph "LifeFXSpawnEvents" kind=vfx contexts=4 blocks=6 operators=3 params=3 edges=6
resource initialEvent=OnPlay updateMode=0 culling=3 instancing=64

param p5138 in  GraphicsBuffer "SpawnBuffer" exposed
param p5140 in  Int32 "SpawnStart" =0 exposed
param p5142 in  Int32 "SpawnCount" =0 exposed

node n5165 VFXAttributeParameter  attribute=spawnIndex
node n5168 Add  a<-p5140  b<-n5165
node n5172 SampleBuffer  buffer<-p5138  index<-n5168

system d5145
  ctx c5144 VFXBasicSpawner  flow>c5150
    blk b5146 VFXSpawnerBurst  repeat=Periodic  Count<-p5142  Delay=0
system d5164  capacity=16384  boundsMode=Manual
  ctx c5150 VFXBasicInitialize  bounds=(0,0,0,500,500,500)  flow>c5211
    blk b5180 SetAttribute  attribute=position  _Position<-n5172.s
    blk b5187 SetAttribute  attribute=lifetime  _Lifetime=2.5
    blk b5190 SetAttribute  attribute=size  _Size=0.25
    blk b5193 SetAttribute  attribute=velocity  Random=Uniform  A=(-1,1,-1)  B=(1,3,1)
    blk b5205 SetAttribute  attribute=color  _Color=(1,0.55,0.1)
  ctx c5211 VFXBasicUpdate  flow>c5213
  ctx c5213 VFXPlanarPrimitiveOutput  blendMode=Additive

# flow roots: c5144
```

## The Catalog and the Editor Bridge

Import the *VfxWaitress Editor Bridge* package sample. It contains:

-   `Unity.VisualEffectGraph.Editor.asmref`, which compiles the scripts beside it
    into VFX Graph's own editor assembly. VFX Graph's authoring model is entirely
    internal, and the tool needs far too much of it to reach by reflection.
-   `VfxWaitressCatalogExporter.cs`, which exports the catalog.
-   `VfxWaitressValidator.cs`, the Editor side of `validate --editor`, `resync`,
    `attr set`, asset references, node measurement, and writing through the
    Editor.
-   `Commands/`, which registers all of that as `vfxwaitress_*` Pipeline commands.
    It only compiles when the Pipeline package is installed.

With the Editor open, export the catalog with:

```
unity command vfxwaitress_export_catalog --timeout 120
```

The export takes several seconds, hence the longer timeout. *Tools \>
VfxWaitress \> Export Catalog* and
`-executeMethod VfxWaitress.CatalogExporter.ExportToDefaultPath` do the same.
The catalog lands in the project's `UserSettings/Waitress/catalog.json`. The tool
finds it by walking up from the graph path you give it, or from the working
directory for commands like `catalog` that take no graph. `--catalog <path>` or
`VFXWAITRESS_CATALOG` point it somewhere else.

Asset names in `show` come from the `.meta` files under `Assets/`, `Packages/`,
and `Library/PackageCache`. The package cache is only scanned when a guid isn't
found in the first two.

## Verifying Changes

-   `vfxwaitress selftest` builds graphs in a throwaway project folder with the
    Editor link off, and covers the YAML container, project and guid lookup,
    every edit verb, `apply` atomicity, id stability, and placement of new
    nodes. Run it from a project that has a catalog; the tests that create nodes
    are skipped without one.
-   `vfxwaitress roundtrip <roots>` parses and re-serializes every VFX asset
    under the roots and requires identical bytes, both untouched and with every
    node forced through the path an edit takes. The last full run covered 2020
    files and 294 MB with no differences.
-   `vfxwaitress sweep <roots>` renders every graph and reports anything the
    catalog can't name. The same 2020 graphs shrank from 300 MB to 3.3 MB of
    dense text.
-   `vfxwaitress validate` flags known problems offline, such as a Custom HLSL
    function with more than four inputs, which breaks compilation of the whole
    graph.
-   For Editor acceptance, a graph the tool edited offline was rewritten by VFX
    Graph's own serializer and came back byte for byte identical, and a graph
    built entirely by one `apply` passes `validate --editor`.

When a graph uses a node type the catalog doesn't cover, `show` prints the
script's guid instead of failing. That usually means the node comes from a
package that isn't installed in the project the catalog was exported from.
