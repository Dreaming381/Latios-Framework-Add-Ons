using System;
using System.IO;

namespace VfxWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("vfxwaitress — query, edit and build Unity VFX Graph assets from the command line");
            w.WriteLine();
            w.WriteLine("read");
            w.WriteLine("  show <vfx> [--node <sel>] [--positions]   the whole graph densely, or one node in full");
            w.WriteLine("  q <vfx> \"<selector>\" [--format ids|long|count]");
            w.WriteLine("  catalog [pattern] [--kind K] [--category C] search the node catalog");
            w.WriteLine("  model <type|menu name>                    settings, enum values and slots of a type");
            w.WriteLine("  attrs <vfx|dir>...                        custom attribute declarations; given several");
            w.WriteLine("                                            paths, reports where graphs disagree on a type");
            w.WriteLine();
            w.WriteLine("edit");
            w.WriteLine("  apply <vfx> <script>|--script -           many edits in one call, written once (help edit)");
            w.WriteLine("  new <path.vfx|.vfxoperator>               an empty asset");
            w.WriteLine("  node add <vfx> <type> [setting=value]... [--wire Port=src]... [--feed [Port=]dst.Port]...");
            w.WriteLine("  block add <vfx> <context> <type> [setting=value]... [--wire Port=src]...");
            w.WriteLine("  wire <vfx> <src>[.<port>] <dst>[.<port>]");
            w.WriteLine("  unwire <vfx> <dst>.<port>");
            w.WriteLine("  set <vfx> <node>.<port> <value>           or --setting <name> for a node setting");
            w.WriteLine("  link <vfx> <ctxA> <ctxB> [--from-slot N] [--to-slot N]");
            w.WriteLine("  rm <vfx> <sel>                            delete nodes, their slots and every link to them");
            w.WriteLine("  attr set <vfx> <name> <Type> [desc]       declare a custom attribute, or correct its type");
            w.WriteLine("  system set <vfx> <d-id> capacity=4096 ...  per-system settings, shared by its contexts");
            w.WriteLine("  layout <vfx>                              arrange the whole graph");
            w.WriteLine("  repair <vfx>|<dir>...                     fix what the Editor would silently repair on open");
            w.WriteLine("  (every edit takes --dry-run)");
            w.WriteLine();
            w.WriteLine("check");
            w.WriteLine("  resync <vfx>                              have the Editor re-derive every node's slots");
            w.WriteLine("  validate <vfx> [--editor]                 structural checks; --editor compiles it for real");
            w.WriteLine("  roundtrip <roots>...                      parse and re-serialize, requiring identical bytes");
            w.WriteLine("  sweep <roots>...                          render every graph, report anything unnameable");
            w.WriteLine();
            w.WriteLine("options");
            w.WriteLine("  --catalog <path>  use a specific catalog (also VFXWAITRESS_CATALOG)");
            w.WriteLine("  --offline         never talk to the Editor, even when writing (see 'help editor')");
            w.WriteLine();
            w.WriteLine("  help <topic>   topics: format, select, edit, catalog, ids, editor");
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        public static int Skill(string install, TextWriter output) => Waitress.Skill.Write(SkillText(), install, output);

        static string SkillText()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resource = System.Linq.Enumerable.FirstOrDefault(assembly.GetManifestResourceNames(),
                n => n.EndsWith(".skill.md", StringComparison.OrdinalIgnoreCase));
            if (resource == null)
                return "(skill.md is missing from this build)\n";
            using var stream = assembly.GetManifestResourceStream(resource);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static void Topic(TextWriter w, string topic)
        {
            switch (topic)
            {
                case null:
                    Index(w);
                    return;

                case "format":
                    w.WriteLine("The dense form (what `show` prints)");
                    w.WriteLine();
                    w.WriteLine("  graph \"Name\" kind=vfx contexts=4 blocks=6 operators=3 params=3 edges=6");
                    w.WriteLine("  resource initialEvent=OnPlay ...        the VisualEffectResource settings");
                    w.WriteLine("  param p5140 in  Int32 \"SpawnStart\" =0 exposed");
                    w.WriteLine("  node n5168 Add  a<-p5140  b<-n5165     an operator, inputs only");
                    w.WriteLine("  system d5164  capacity=16384 boundsMode=Manual");
                    w.WriteLine("    ctx c5150 VFXBasicInitialize  flow>c5211");
                    w.WriteLine("      blk b5180 SetAttribute  attribute=position  _Position<-n5172.s");
                    w.WriteLine();
                    w.WriteLine("  Port<-node.Port   the port is fed from that port of that node.");
                    w.WriteLine("                    The '.Port' half is dropped when the source has one output.");
                    w.WriteLine("  Port=value        unconnected input with a non-default literal.");
                    w.WriteLine("  flow>c5211        context flow link. An index pair is shown only when non-zero.");
                    w.WriteLine("  system d…         contexts are grouped by the VFXData they share, because");
                    w.WriteLine("                    capacity, bounds mode and space live there and on no context.");
                    w.WriteLine();
                    w.WriteLine("  Settings equal to the type's fresh default are omitted, except the ones that");
                    w.WriteLine("  identify a variant (attribute=position), which say what the node is.");
                    w.WriteLine("  `show --node <sel>` prints the whole slot tree, which is where a composite");
                    w.WriteLine("  hides the child that actually holds the value.");
                    return;

                case "select":
                    w.WriteLine("Selectors: steps separated by |, seeded by the first step.");
                    w.WriteLine();
                    w.WriteLine("  all                         every node");
                    w.WriteLine("  type:Add,Multiply           node type, short or full name, globs allowed");
                    w.WriteLine("  id:n5168                    short id or raw file id");
                    w.WriteLine("  name:Spawn*                 menu name, label or exposed parameter name");
                    w.WriteLine("  kind:context|block|operator|parameter");
                    w.WriteLine("  context:c5150               blocks inside that context");
                    w.WriteLine("  system:d5164                contexts sharing that VFXData");
                    w.WriteLine("  param:SpawnBuffer           parameter by exposed name");
                    w.WriteLine("  setting:attribute=position  a serialized setting's value");
                    w.WriteLine("  port:UV                     has a port by that name (loose match)");
                    w.WriteLine("  is:root|leaf|orphan|param|input|output|unknown");
                    w.WriteLine();
                    w.WriteLine("  | in    | out    | both     direct neighbours through wires");
                    w.WriteLine("  | in*   | out*             transitive closure");
                    w.WriteLine("  | blocks | context         descend into a context, or climb out of a block");
                    w.WriteLine();
                    w.WriteLine("  Whole pipelines combine:  expr + expr,  expr - expr,  expr & expr");
                    return;

                case "edit":
                    w.WriteLine("Editing");
                    w.WriteLine();
                    w.WriteLine("  Batch a whole session with `apply <vfx> <script>`. A script is the edit verbs");
                    w.WriteLine("  with the graph path left out, one per line. `--as label` names what a line");
                    w.WriteLine("  creates, and later lines refer to it as `$label`. Lines starting with # are");
                    w.WriteLine("  comments, and a trailing backslash continues a line. The script edits one");
                    w.WriteLine("  in-memory copy and writes once; any error leaves the file untouched.");
                    w.WriteLine();
                    w.WriteLine("    node add \"Sample Graphics Buffer\" --as sample --wire buffer=p4985 --wire index=n5037");
                    w.WriteLine("    block add c4993 \"Set Position\" --wire _Position=$sample.s");
                    w.WriteLine("    system set d5164 capacity=4096");
                    w.WriteLine();
                    w.WriteLine("  `node add` and `block add` take --wire Port=source[.Port] for their inputs and");
                    w.WriteLine("  --feed [Port=]destination.Port for their outputs. New nodes are placed beside");
                    w.WriteLine("  what they're wired to when the command or script finishes; nothing else moves.");
                    w.WriteLine();
                    w.WriteLine("  A node is written by cloning the catalog's template of a freshly created");
                    w.WriteLine("  instance — the model plus its whole slot tree — and remapping its file ids.");
                    w.WriteLine("  Settings are given by name; enums and bools are translated to what the file");
                    w.WriteLine("  stores, so `repeat=Periodic` is what you type.");
                    w.WriteLine();
                    w.WriteLine("  Slot names rarely match the name you configured the node with: a SetAttribute");
                    w.WriteLine("  set to `position` calls its slot `_Position`, and turning random mode on");
                    w.WriteLine("  renames its slots to `A` and `B` entirely. Matching ignores underscores,");
                    w.WriteLine("  spaces and case, and a miss lists the slots the node does have.");
                    w.WriteLine();
                    w.WriteLine("  A composite slot stores its value as one blob on the master slot. Setting");
                    w.WriteLine("  `A 1,2,3` fills the whole tree; setting a child is refused, because VFX Graph");
                    w.WriteLine("  would ignore it.");
                    w.WriteLine();
                    w.WriteLine("  A setting that decides a node's slots selects a catalog variant rather than being");
                    w.WriteLine("  patched on: SetAttribute has five slot shapes, and Unity does not repair a node");
                    w.WriteLine("  built from the wrong one. A value with no variant is refused, not guessed.");
                    w.WriteLine();
                    w.WriteLine("  Capacity, bounds mode and space belong to the system, not the context; `set` on a");
                    w.WriteLine("  context routes them there, and `system set` writes them for every context at once.");
                    w.WriteLine();
                    w.WriteLine("  A flow link between two particle contexts also merges them into one system. `link`");
                    w.WriteLine("  does both halves; `validate` flags a graph where only the flow edge was written.");
                    w.WriteLine();
                    w.WriteLine("  Wiring writes the reference on both slots. There is no edge list in");
                    w.WriteLine("  the file; a link recorded on one end only is a broken graph, and `validate`");
                    w.WriteLine("  checks for exactly that.");
                    return;

                case "catalog":
                    w.WriteLine("Catalog");
                    w.WriteLine();
                    w.WriteLine("  Exported from a running Editor, so it describes the packages actually");
                    w.WriteLine("  installed. With the \"VfxWaitress Editor Bridge\" sample imported, run");
                    w.WriteLine("  `unity command vfxwaitress_export_catalog --timeout 120`. It lands in the");
                    w.WriteLine("  project's UserSettings/Waitress/catalog.json, which the tool finds from the");
                    w.WriteLine("  graph path you give it, or the working directory. --catalog or");
                    w.WriteLine("  VFXWAITRESS_CATALOG point it elsewhere.");
                    w.WriteLine();
                    w.WriteLine("  It covers models the node menu hides, like experimental ones and subgraph");
                    w.WriteLine("  and deprecated models that are never registered, because real graphs use them.");
                    w.WriteLine();
                    w.WriteLine("  Menu names are not type names: `Periodic Burst` is VFXSpawnerBurst with");
                    w.WriteLine("  repeat=Periodic, and the quad output is `Output Particle Unlit Quad`. Search");
                    w.WriteLine("  by either; `catalog '*Quad*' --kind context` narrows it.");
                    w.WriteLine();
                    w.WriteLine("  `model <type>` prints the settings with their alternatives and the slots. A");
                    w.WriteLine("  setting can reshape the slot list — VFXSpawnerBurst with spawnMode=Constant");
                    w.WriteLine("  has a float Count, with Random it has a Vector2 — so check the slots again");
                    w.WriteLine("  after changing one.");
                    return;

                case "ids":
                    w.WriteLine("Identifiers");
                    w.WriteLine();
                    w.WriteLine("  Every entity gets a kind letter plus the shortest unique tail of its Unity");
                    w.WriteLine("  local file id: n for a node, c context, b block, p parameter, d system data.");
                    w.WriteLine("  Ids within one file share a long leading run, so the tail is what tells them");
                    w.WriteLine("  apart. They come only from the file id, so adding or removing a node doesn't");
                    w.WriteLine("  renumber the others. New nodes get file ids whose tail nothing else has, so");
                    w.WriteLine("  the id `node add` reports is the one it keeps. Any command that takes a node");
                    w.WriteLine("  also accepts its raw file id or an unambiguous type name.");
                    return;

                case "editor":
                    w.WriteLine("Why --editor exists");
                    w.WriteLine();
                    w.WriteLine("  VFX Graph's per-node error reporters stay quiet on graphs that can't");
                    w.WriteLine("  compile. A failed compile still leaves an asset that imports, that");
                    w.WriteLine("  LoadAssetAtPath returns, and that reports no error. It just has every");
                    w.WriteLine("  exposed property missing. Nothing at runtime can tell it apart from a");
                    w.WriteLine("  working effect that isn't being fed.");
                    w.WriteLine();
                    w.WriteLine("  So this tool's own checks are never proof. `validate --editor` runs a real");
                    w.WriteLine("  compile in the connected Editor through the unity CLI and reports what it");
                    w.WriteLine("  says. It needs the \"VfxWaitress Editor Bridge\" sample from the");
                    w.WriteLine("  Latios Framework Addons package imported into the project.");
                    w.WriteLine();
                    w.WriteLine("  Every command that writes uses the Editor too, without being asked. An");
                    w.WriteLine("  open VFX Graph window keeps its own copy of the graph, so a write first");
                    w.WriteLine("  flushes whatever that copy hasn't saved, and afterwards has VFX Graph");
                    w.WriteLine("  write the final asset itself. The bytes on disk are then the Editor's own,");
                    w.WriteLine("  so the graph doesn't open dirty and no unsaved-changes dialog is left");
                    w.WriteLine("  waiting for an answer.");
                    w.WriteLine();
                    w.WriteLine("  `layout` also asks the Editor for real node and port rectangles, which a");
                    w.WriteLine("  .vfx doesn't store. Without them, it's guessing where a wire lands.");
                    w.WriteLine();
                    w.WriteLine("  --offline turns all of that off. Reads never need the Editor either way.");
                    return;

                default:
                    w.WriteLine($"no help topic '{topic}'");
                    Index(w);
                    return;
            }
        }
    }
}
