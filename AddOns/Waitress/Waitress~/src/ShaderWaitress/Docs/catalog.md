THE NODE CATALOG

Shader Graph node types define their ports in C#, and there's no public
authoring API. So the tool uses a catalog exported from a real Editor. For each
node type it holds the menu path, synonyms, whether it has a preview, its ports,
its settings, and the exact JSON Shader Graph writes for a fresh instance.
Adding a node clones that JSON with fresh object ids, so the node is exactly
what the Editor would have made.

  shaderwaitress nodes                  list everything
  shaderwaitress nodes "*texture*"      search titles, types, paths, synonyms
  shaderwaitress nodes --show Multiply  one node's ports
  shaderwaitress nodes --starters       the graph templates 'new' can use

WHERE IT COMES FROM

Import the "ShaderWaitress Editor Bridge" sample from the Latios Framework
Addons package. With the Editor open, run:

  unity command shaderwaitress_export_catalog

or use Tools > ShaderWaitress > Export Node Catalog. Without a running Editor:

  Unity -batchmode -quit -projectPath <project> \
        -executeMethod ShaderWaitress.CatalogExporter.ExportToDefaultPath

It writes the project's UserSettings/Waitress/nodes.json. The exporter picks up
nodes from any package or from the project itself.

WHICH CATALOG IS USED

  1. --catalog <path>, if given
  2. $SHADERWAITRESS_CATALOG, if set
  3. UserSettings/Waitress/nodes.json in the Unity project that holds the graph
     you named, or the working directory when a command names no graph

There's no catalog built into the binary.

WITHOUT A CATALOG

Reading, querying, wiring, layout, and validation all still work. Only creating
nodes and properties, and checking port names, need it, because only those need
to know what a type looks like before it exists in the file.

RE-EXPORT IT WHEN

The Unity version changes, the render pipeline package changes, or the project
gains custom nodes you want to create from the command line.
