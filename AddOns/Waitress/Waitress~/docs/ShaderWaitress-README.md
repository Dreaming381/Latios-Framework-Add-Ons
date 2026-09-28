# ShaderWaitress

ShaderWaitress is a command-line tool for querying, editing, and building Unity
Shader Graph assets (`.shadergraph` and `.shadersubgraph`) without opening the
Editor. It's built for AI agents.

Shader Graph assets are JSON, but not JSON anyone wants to read. A trivial
18-node graph is 51 KB spread across 69 flat objects, and real graphs pass
400 KB. About three quarters of that is port objects and 2D coordinates. There's
also no public authoring API. ShaderWaitress prints a whole graph in about 150
lines, answers structural questions in one call, edits graphs without the caller
ever touching a coordinate, and takes care of layout.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

All you need is the .NET SDK. There are no NuGet packages. The binary is
`UserSettings/Waitress/build/bin/ShaderWaitress/release/shaderwaitress[.exe]`, and the same
build makes VfxWaitress. `--artifacts-path` keeps the build out of the package
folder, which may be read-only.

## Start Here

```
shaderwaitress show <graph>            # the whole graph, densely
shaderwaitress q <graph> "<selector>"  # chainable structural query
shaderwaitress help                    # command index
shaderwaitress skill                   # the whole tool on one page, for an agent
```

`shaderwaitress help <topic>` covers `format`, `select`, `edit`, `layout`,
`catalog`, `recipes`, and `ids`.

## What It Looks Like

```
$ shaderwaitress show Assets/.../HeatDisstortionWobble.shadergraph
graph "HeatDisstortionWobble" kind=shadergraph nodes=13 blocks=5 edges=15 props=3 precision=single
target t22b9 Universal/Unlit surface=transparent blend=alpha renderface=both ztest=lequal

prop p8e6e Float     "Heat Distortion"  ref=_Heat_Distortion =0
prop pb2bb Vector2   "Distortino Speed" ref=_Distortino_Speed =(0, 0)
prop pb5d6 Texture2D "Mask"             ref=_Mask

node naa20 Property pb2bb "Distortino Speed"
node nf3bd Time
node nc21c Multiply         A<-naa20  B<-nf3bd.Time
node na7b9 TilingAndOffset  Tiling=(1, 1)  Offset<-nc21c
node nd0c6 SampleTexture2D  Texture<-n83e3
node n77b9 Noise "Simple Noise"  UV<-na7b9  Scale=50
node n45b3 SceneColor       UV<-n95d4

block b6978 SurfaceDescription.BaseColor  BaseColor<-n45b3
block bf614 SurfaceDescription.Alpha      Alpha<-nd0c6.A

# blocks with no input: b6ca1 b1b80 ba456
# clusters: 4
```

That's 51 KB of JSON in 26 lines of text, and nothing that affects the shader got
lost.

## Layout

The tool owns every node, group, and context position, and re-runs layout after
any structural edit. It never lets nodes overlap, keeps wires flowing left to
right, and keeps groups apart and cleanly wrapped around their members. Within
those rules it minimizes crossings, shallow crossing angles, piled-up crossings,
and wires drawn over nodes. `layout --report` prints the score before and after.

Two behaviors matter most in practice:

-   **Adding to an arranged graph doesn't rearrange it.** If the existing layout
    is sound, only the new nodes get placed, and every other node stays exactly
    where it was.
-   **A worse layout is never written.** If the layout already in the file scores
    better than the one the tool computed, the file is left alone and the tool
    says so.

## The Node Catalog

The catalog holds every node type's menu path, synonyms, ports, and the exact
JSON Shader Graph writes for a fresh instance. When the tool adds a node,
it clones that JSON, so the result is exactly what the Editor would have made.
The catalog also lists each node's settings (the dropdowns and toggles drawn on
the node body) with the names of their enum values, so `DepthSamplingMode` reads
as `Eye` instead of `2`. And it records how many control rows each node draws,
since that decides the node's height.

You export the catalog from your own project with the *ShaderWaitress Editor
Bridge* package sample. With it imported and the Editor open, run
`unity command shaderwaitress_export_catalog`, or use *Tools \> ShaderWaitress
\> Export Node Catalog*. Without an Editor running, export it headless:

```
Unity -batchmode -quit -projectPath <project> \
      -executeMethod ShaderWaitress.CatalogExporter.ExportToDefaultPath
```

It lands in the project's `UserSettings/Waitress/nodes.json`. The tool finds it
by walking up from the graph path you give it (or the working directory, for
commands without one). `--catalog <path>` or `SHADERWAITRESS_CATALOG` point it
somewhere else.

Re-export whenever you change Unity versions, change render pipelines, or add
custom nodes. The catalog stores a `schema` number and the Unity and Shader Graph
versions it came from, so a stale one is easy to spot.

## The Editor Link

With the sample imported and the Editor running, every write:

1.  Saves any unsaved edits in a Shader Graph window showing the graph, so the
    tool edits what the person is looking at.
2.  Writes the file.
3.  Has the Editor rewrite the file in Shader Graph's own form if it differs,
    import it, and reload the window. The window is reloaded by dropping its
    graph, which skips the "Graph has changed on disk" dialog. That dialog can't
    be answered when Unity runs with `-automated`.

`validate --editor` also imports and compiles the graph and reports the shader's
errors and warnings. `--offline` turns all of this off, and reads never need the
Editor.

### Working Without a Matching Catalog

The catalog is the only part of the tool tied to a specific Unity version,
render pipeline, or project. So everything that can keep working without it
does:

-   Reading, querying, wiring, layout, and validation never use it.
-   Node types the catalog has never seen still show their settings and let you
    change them. They're read straight from the file. You just get raw numbers
    instead of enum names, so a value reads as `3` instead of `Tiled`.
-   Only creating nodes, properties, and keywords truly needs the catalog, since
    only those need to know what a type looks like before it exists in the file.

## Verifying Changes

If you change the tool, run these from a Unity project that has a catalog:

| Command | What it checks |
| --- | --- |
| `shaderwaitress selftest` | The container format, the JSON writer, IDs, the catalog, node settings, every edit command, batch atomicity, selectors, and layout rules. Runs offline in a temp folder. |
| `shaderwaitress roundtrip <roots>` | Every graph under the roots parses and re-serializes byte for byte, both untouched and with every object forced through the write path. |
| `shaderwaitress sweep <roots>` | Runs the whole pipeline over every graph without writing, and fails on any layout the tool produced that has an overlap, a group problem, or more backward wires than it started with. Also reports the total layout score before and after. |

Point `roundtrip` and `sweep` at as many real graphs as you can find. The Shader
Graph package samples and `Library/PackageCache` make a good corpus. The last
full sweep covered 708 graphs with no failures, and layout improved 548 of them
without making any worse.

The sweep allows one kind of backward wire. When two groups feed each other, one
wire has to run backward no matter what, and the tool reports which groups
caused it.

Some things only the Editor can confirm. Graphs built entirely by the tool pass
`validate --editor`, and Shader Graph's own serializer produces the same dense
form from them. A graph edited while open in a window keeps the window's unsaved
changes, reloads without a dialog, and opens clean afterwards. Beyond that, each
of these was checked against the compiled shader:

-   A `hybrid-per-instance` property emits `UNITY_DOTS_INSTANCED_PROP`.
-   A Custom Function node authored with `node port add` gets its HLSL into the
    generated passes.
-   A keyword with `--allow-definition-override off` compiles to `#if` blocks
    instead of ternaries.
-   A sub-graph property with `--promote on` shows up as a material property on
    the parent shader.
-   A `--default normal-map` texture property defaults to `bump`.
-   `target set surface=transparent --sync-blocks` gives a working transparent
    shader with the extra Alpha blocks.

## Source Layout

```
docs/                            this file and the design document
src/Waitress.Common/             plumbing shared with VfxWaitress: arguments, globs, short IDs,
                                 the batch script text, project lookup, and the Editor bridge
src/ShaderWaitress/
  Serialization/                 the MultiJson container, byte-exact
  Model/                         the graph model, dense text, validation
  Query/                         the selector language
  Edit/                          edit commands and the batch script language
  Layout/                        layered layout and the readability score
  Catalog/                       node templates and port metadata
  Cli/                           commands and embedded help
  Docs/                          the help topics and the agent skill
  Testing/                       self tests, the round-trip check, the corpus sweep
```

The Editor side lives in the package's `Samples~/ShaderWaitress Editor Bridge/`
folder. The `.asmref` there compiles the exporter and the Editor link into
`Unity.ShaderGraph.Editor`, and `Commands/` registers them as Pipeline commands
when the Pipeline package is installed.
