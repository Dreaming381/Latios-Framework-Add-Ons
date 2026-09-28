ShaderWaitress — query, edit and build Unity Shader Graph files from the command line.

USAGE
  shaderwaitress <command> <graph.shadergraph> [arguments]

READING
  show <graph> [--outputs] [--positions] [--brief] [--notes]
        Print the whole graph in the dense text form. Start here: a 400 KB
        graph becomes about 150 lines.
  q|query <graph> "<selector>" [--format text|ids|count]
        Run a chainable selector. See 'help select'.
  node show <graph> "<selector>"
        Every port of the matched nodes, with types, values and wiring.
  prop list <graph> | group list <graph> | keyword list <graph> | target show <graph>
  settings show <graph>
  validate <graph> [--editor]
        Structural and layout problems. Exit code 1 if any were found.
        --editor also imports and compiles it in the running Editor.

EDITING                                     (all of these re-run layout)
  new <path> [--target universal/lit|subgraph] [--force] [--path "Shader Graphs"]
  node add <graph> <Type> [--name N] [--group G]
                          [--property P] [--keyword K] [--subgraph FILE]
                          [--set PortOrSetting=value]... [--setting Name=value]...
                          [--wire Port=src]... [--feed Port=dst]...
  node insert <graph> <Type> --after "<selector>" [--port P] [--set P=v]...
        Splice a node into what an existing node already feeds.
  node rm <graph> "<selector>" [--reconnect]
  node set <graph> "<selector>" [--name N] [--group G|none]
                                [--set PortOrSetting=v]... [--setting Name=v]...
  node replace <graph> "<selector>" --with <Type> [--map Old=New]...
  wire <graph> <src[.Port]> <dst[.Port]>
  unwire <graph> <dst[.Port]> | <src[.Port]> <dst[.Port]>
  group new|rm|rename|add|clear <graph> ...
  node port add|rm <graph> "<selector>" --name N [--type T] [--direction in|out]
        Author a port. Custom Function nodes have none until you do.
  prop add|rm|set <graph> ... [--declaration per-material|global|hybrid-per-instance]
  keyword add|rm|set <graph> ...
  block list|add|rm <graph> ...
  target set <graph> key=value...
  settings set <graph> key=value...
  layout <graph> [--report] [--no-refine] [--remeasure]
  apply <graph> <script-file>|--script -
        Run many edits in one invocation, written once. Every editing verb above
        works in a script, with the graph path omitted. See 'help edit'.

CATALOG
  nodes [pattern] [--show <Type>] [--starters]

DOCS
  help <topic>     format, select, edit, layout, catalog, recipes, ids
  skill [--install <dir>]
        Print (or install) the complete agent guide in one page.

GLOBAL OPTIONS
  --dry-run          Show the result without writing the file.
  --no-layout        Keep existing positions after a structural edit.
  --format           Output format where a command supports one.
  --catalog <path>   Use a specific node catalog (also SHADERWAITRESS_CATALOG).
  --offline          Never talk to the Editor, even when writing.

THE EDITOR

When the Editor is running with the "ShaderWaitress Editor Bridge" sample
imported, every write first saves any unsaved edits in an open Shader Graph
window, then has the window reload the result without the changed-on-disk
dialog. Reads never need the Editor.

Every command that names an entity accepts a short id (n42d5), a full 32-hex
object id, a node display name, or a block descriptor such as BaseColor.
