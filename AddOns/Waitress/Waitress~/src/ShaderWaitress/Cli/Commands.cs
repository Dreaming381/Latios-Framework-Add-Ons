using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ShaderWaitress.Catalog;
using ShaderWaitress.Edit;
using ShaderWaitress.Layout;
using ShaderWaitress.Model;
using ShaderWaitress.Query;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Cli
{
    /// <summary>
    /// CLI surface. Each command loads a graph, calls into <see cref="Ops"/>, re-lays the
    /// graph out if the structure changed, and writes once.
    /// </summary>
    public static class Commands
    {
        // ------------------------------------------------------------- read only

        public static int Show(Args args, TextWriter output)
        {
            var doc = Open(args, out _);
            var options = new DenseTextOptions
            {
                ShowPositions = args.Flag("positions"),
                ShowOutputs = args.Flag("outputs"),
                Brief = args.Flag("brief"),
                ShowNotes = args.Flag("notes"),
            };
            args.RejectUnknown();
            output.Write(DenseText.Render(doc, options));
            return 0;
        }

        public static int Query(Args args, TextWriter output)
        {
            var doc = Open(args, out var rest);
            if (rest.Count == 0)
                throw new ShaderWaitressException("missing selector, e.g. shaderwaitress q g.shadergraph 'type:Multiply | in'");
            var format = args.Get("format", "text");
            var matches = Selector.Evaluate(doc, string.Join(" ", rest));
            var showOutputs = args.Flag("outputs");
            var showPositions = args.Flag("positions");
            var brief = args.Flag("brief");
            args.RejectUnknown();

            switch (format)
            {
                case "ids":
                    foreach (var node in matches)
                        output.WriteLine(node.ShortId);
                    return 0;
                case "count":
                    output.WriteLine(matches.Count.ToString(CultureInfo.InvariantCulture));
                    return 0;
                case "text":
                    break;
                default:
                    throw new ShaderWaitressException($"unknown --format '{format}'. Use text, ids or count.");
            }

            if (matches.Count == 0)
            {
                output.WriteLine("# no matches");
                return 0;
            }
            output.Write(DenseText.Render(doc, new DenseTextOptions
            {
                ShowHeader = false,
                ShowAnalysis = false,
                Only = matches,
                ShowOutputs = showOutputs,
                ShowPositions = showPositions,
                Brief = brief,
            }));
            output.WriteLine($"# {matches.Count} match(es)");
            return 0;
        }

        public static int Validate(Args args, TextWriter output)
        {
            var doc = Open(args, out _);
            var useEditor = args.Flag("editor");
            args.RejectUnknown();
            var problems = Validator.Run(doc);
            foreach (var problem in problems)
                output.WriteLine(problem);
            output.WriteLine(problems.Count == 0 ? "ok: no problems found" : $"{problems.Count} problem(s)");
            if (!useEditor)
                return problems.Count == 0 ? 0 : 1;

            var report = EditorLink.Bridge.Require(doc.Path, "Compile");
            output.WriteLine(report);
            // A one-line error means the compile never ran.
            if (report.StartsWith("error:", StringComparison.Ordinal) && report.IndexOf('\n') < 0)
                return 2;
            var editorErrors = report.Split('\n').Any(l => l.TrimStart().StartsWith("error:", StringComparison.Ordinal));
            return problems.Count == 0 && !editorErrors ? 0 : 1;
        }

        public static int Catalog(Args args, TextWriter output)
        {
            var catalog = NodeCatalog.Shared;
            var show = args.Get("show");
            var starters = args.Flag("starters");
            var pattern = args.Positional.Count > 0 ? args.Positional[0] : null;
            args.RejectUnknown();

            if (catalog.IsEmpty)
            {
                output.WriteLine("no catalog is loaded; see 'shaderwaitress help catalog'");
                return 1;
            }

            if (starters)
            {
                foreach (var starter in catalog.Starters)
                    output.WriteLine(starter.Name);
                return 0;
            }

            if (show != null)
            {
                var entry = catalog.Require(show);
                output.WriteLine($"{entry.Title}   type={entry.TypeName}");
                if (entry.Path.Length > 0)
                    output.WriteLine($"  menu: {entry.PathLabel}/{entry.Title}");
                if (entry.Synonyms.Length > 0)
                    output.WriteLine($"  synonyms: {string.Join(", ", entry.Synonyms)}");
                output.WriteLine($"  preview: {(entry.HasPreview ? "yes" : "no")}");
                foreach (var setting in entry.Settings)
                {
                    var options = setting.Values is { Length: > 0 } ? "   one of: " + string.Join(", ", setting.Values) : string.Empty;
                    output.WriteLine($"  set {setting.Name,-24} {setting.Kind}{options}");
                }
                foreach (var port in entry.Ports)
                    output.WriteLine($"  {(port.IsInput ? "in " : "out")} {port.Name,-24} {SlotTypes.Label(port.Kind)}");
                return 0;
            }

            var results = catalog.Search(pattern)
                .OrderBy(n => n.PathLabel, StringComparer.OrdinalIgnoreCase)
                .ThenBy(n => n.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (var entry in results)
                output.WriteLine($"{entry.Title,-28} {entry.PathLabel,-30} {string.Join(" ", entry.Ports.Where(p => p.IsInput).Select(p => p.Name))}");
            output.WriteLine($"# {results.Count} type(s); catalog from Unity {catalog.UnityVersion}, Shader Graph {catalog.ShaderGraphVersion}");
            return 0;
        }

        // ---------------------------------------------------------------- create

        public static int New(Args args, TextWriter output)
        {
            var path = args.Positional0("output path");
            var starterName = args.Get("target", "universal/lit");
            var force = args.Flag("force");
            var graphPath = args.Get("path", "Shader Graphs");
            args.RejectUnknown();

            var catalog = NodeCatalog.Shared;
            // "new x.shadersubgraph" means a subgraph even when no target was named.
            if (!args.Has("target") && path.EndsWith(".shadersubgraph", StringComparison.OrdinalIgnoreCase))
                starterName = "subgraph";
            var starter = catalog.FindStarter(starterName);
            if (starter == null)
                throw new ShaderWaitressException($"unknown target '{starterName}'. Available: " + string.Join(", ", catalog.Starters.Select(s => s.Name)));

            var wantsSubGraph = string.Equals(starter.Kind, "subgraph", StringComparison.OrdinalIgnoreCase);
            var extension = wantsSubGraph ? ".shadersubgraph" : ".shadergraph";
            if (!path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                throw new ShaderWaitressException($"a {starter.Name} graph must be written to a {extension} file, not '{Path.GetFileName(path)}'");
            if (wantsSubGraph && !args.Has("path"))
                graphPath = "Sub Graphs";

            if (File.Exists(path) && !force)
                throw new ShaderWaitressException($"{path} already exists; pass --force to overwrite");

            // Fresh object ids, so two graphs made from the same starter are not the same
            // objects as far as Shader Graph is concerned.
            var text = Reidentify(starter.Text);
            var doc = ShaderGraphDocument.FromRaw(MultiJsonDocument.Parse(text), path, catalog);
            doc.Raw.Root.Edit()["m_Path"] = graphPath;
            doc.Rebuild();

            new LayoutEngine(doc).Run();
            var directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            Save(doc, output, path);
            output.WriteLine($"created {path} ({starter.Name})");
            output.Write(DenseText.Render(doc));
            return 0;
        }

        static string Reidentify(string text)
        {
            var source = MultiJsonDocument.Parse(text);
            var scratch = new MultiJsonDocument();
            var result = text;
            foreach (var entry in source.Entries)
                result = result.Replace(entry.ObjectId, scratch.NewObjectId(), StringComparison.Ordinal);
            return result;
        }

        // ----------------------------------------------------------------- nodes

        public static int Node(IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                throw new ShaderWaitressException("usage: shaderwaitress node <add|insert|rm|set|replace|show> <graph> ...");
            var sub = argv[0].ToLowerInvariant();

            // "node port add <graph> ..." puts two words before the path, unlike every other
            // subcommand, so the action comes off the front before the path is read.
            var portAction = string.Empty;
            var tail = argv.Skip(1).ToList();
            if (sub == "port")
            {
                portAction = tail.Count > 0 ? tail[0].ToLowerInvariant() : string.Empty;
                tail = tail.Skip(1).ToList();
            }

            var args = Args.Parse(tail, "with-nodes");
            var doc = Open(args, out var rest, forEdit: sub != "show");
            var editor = new GraphEditor(doc);

            switch (sub)
            {
                case "add":
                {
                    var node = Ops.NodeAdd(doc, editor, args, rest);
                    output.WriteLine(node.ShortId);
                    return Finish(doc, args, editor, output);
                }
                case "insert":
                {
                    foreach (var node in Ops.NodeInsert(doc, editor, args, rest))
                        output.WriteLine(node.ShortId);
                    return Finish(doc, args, editor, output);
                }
                case "rm":
                case "remove":
                case "delete":
                    Ops.NodeRemove(doc, editor, args, rest);
                    return Finish(doc, args, editor, output);
                case "set":
                    Ops.NodeSet(doc, editor, args, rest);
                    return Finish(doc, args, editor, output);
                case "replace":
                    Ops.NodeReplace(doc, editor, args, rest);
                    return Finish(doc, args, editor, output);
                case "port":
                    return NodePort(doc, args, rest, portAction, editor, output);
                case "show":
                    return NodeShow(doc, args, rest, output);
                default:
                    throw new ShaderWaitressException($"unknown 'node' subcommand '{sub}'");
            }
        }

        static int NodePort(ShaderGraphDocument doc, Args args, IReadOnlyList<string> rest, string action, GraphEditor editor, TextWriter output)
        {
            var remainder = rest;
            switch (action)
            {
                case "add":
                    Ops.PortAdd(doc, editor, args, remainder);
                    return Finish(doc, args, editor, output);
                case "rm":
                case "remove":
                    Ops.PortRemove(doc, editor, args, remainder);
                    return Finish(doc, args, editor, output);
                default:
                    throw new ShaderWaitressException(
                        "usage: shaderwaitress node port <add|rm> <graph> \"<selector>\" --name N [--type T] [--direction in|out]");
            }
        }

        static int NodeShow(ShaderGraphDocument doc, Args args, IReadOnlyList<string> rest, TextWriter output)
        {
            var matches = Ops.Select(doc, rest, "node show");
            args.RejectUnknown();
            foreach (var node in matches)
            {
                output.WriteLine($"{node.ShortId} {node.TypeLabel}   type={node.TypeName}");
                foreach (var setting in node.Settings)
                {
                    var options = setting.Values is { Length: > 0 } ? "   one of: " + string.Join(", ", setting.Values) : string.Empty;
                    output.WriteLine($"  set {setting.Name,-24} {setting.Kind,-14} ={setting.Display}{options}");
                }
                foreach (var port in node.Ports)
                {
                    var wired = port.IsInput
                        ? doc.EdgesIn(node).Where(e => e.ToSlot == port.Id).Select(e => e.From.ShortId).ToList()
                        : doc.EdgesOut(node).Where(e => e.FromSlot == port.Id).Select(e => e.To.ShortId).ToList();
                    output.Write($"  {(port.IsInput ? "in " : "out")} {DenseText.PortToken(port),-24} {SlotTypes.Label(port.Kind),-14}");
                    var value = port.FormattedValue;
                    if (value != null)
                        output.Write($" ={value}");
                    if (wired.Count > 0)
                        output.Write($" [{string.Join(" ", wired)}]");
                    output.WriteLine();
                }
            }
            return 0;
        }

        // ----------------------------------------------------------------- wires

        public static int Wire(Args args, TextWriter output)
        {
            var doc = Open(args, out var rest, forEdit: true);
            var editor = new GraphEditor(doc);
            Ops.Wire(doc, editor, rest);
            return Finish(doc, args, editor, output);
        }

        public static int Unwire(Args args, TextWriter output)
        {
            var doc = Open(args, out var rest, forEdit: true);
            var editor = new GraphEditor(doc);
            Ops.Unwire(doc, editor, rest);
            return Finish(doc, args, editor, output);
        }

        // ---------------------------------------------------------------- groups

        /// <summary>
        /// Lists, adds, and removes output blocks. A graph switched to transparent keeps its
        /// opaque block list, so it has no Alpha block to wire a fade into until one is added.
        /// </summary>
        public static int Block(IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                throw new ShaderWaitressException("usage: shaderwaitress block <list|add|rm> <graph> ...");
            var sub = argv[0].ToLowerInvariant();
            var args = Args.Parse(argv.Skip(1).ToList());
            var doc = Open(args, out var rest, forEdit: sub != "list");
            var editor = new GraphEditor(doc);

            switch (sub)
            {
                case "list":
                {
                    args.RejectUnknown();
                    foreach (var block in doc.Nodes.Where(n => n.IsBlock))
                    {
                        var fed = doc.EdgesIn(block).Count > 0 ? "wired" : "unwired";
                        output.WriteLine($"{block.ShortId} {block.BlockDescriptor} {fed}");
                    }
                    var missing = BlockAdvice.Missing(doc).ToList();
                    foreach (var line in missing)
                        output.WriteLine("# " + line);
                    if (missing.Count == 0)
                        output.WriteLine("# the block list matches the target settings");
                    return 0;
                }
                case "add":
                {
                    if (rest.Count < 1)
                        throw new ShaderWaitressException(
                            "usage: shaderwaitress block add <graph> <Descriptor>, e.g. SurfaceDescription.Alpha");
                    foreach (var descriptor in rest)
                    {
                        var block = Ops.BlockAdd(doc, editor, descriptor);
                        output.WriteLine($"{block.ShortId} {block.BlockDescriptor}");
                    }
                    return Finish(doc, args, editor, output);
                }
                case "rm":
                case "remove":
                {
                    if (rest.Count < 1)
                        throw new ShaderWaitressException("usage: shaderwaitress block rm <graph> \"<selector>\"");
                    Ops.BlockRemove(doc, editor, rest);
                    return Finish(doc, args, editor, output);
                }
                default:
                    throw new ShaderWaitressException($"unknown block subcommand '{sub}'. Use list, add or rm.");
            }
        }

        public static int Group(IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                throw new ShaderWaitressException("usage: shaderwaitress group <list|new|rm|rename|add|clear> <graph> ...");
            var sub = argv[0].ToLowerInvariant();
            var args = Args.Parse(argv.Skip(1).ToList(), "with-nodes");
            var doc = Open(args, out var rest, forEdit: sub != "list");
            var editor = new GraphEditor(doc);

            switch (sub)
            {
                case "list":
                    args.RejectUnknown();
                    foreach (var group in doc.Groups)
                    {
                        var members = doc.Nodes.Where(n => n.GroupId == group.ObjectId).Select(n => n.ShortId).ToList();
                        output.WriteLine($"{group.ShortId} \"{group.Title}\"  {members.Count} member(s): {string.Join(" ", members)}");
                    }
                    if (doc.Groups.Count == 0)
                        output.WriteLine("# no groups");
                    return 0;
                case "new":
                {
                    var group = Ops.GroupNew(doc, editor, rest);
                    output.WriteLine(group.ShortId);
                    return Finish(doc, args, editor, output);
                }
                case "rm":
                case "remove":
                    if (rest.Count < 1)
                        throw new ShaderWaitressException("usage: shaderwaitress group rm <graph> <group> [--with-nodes]");
                    editor.RemoveGroup(doc.ResolveGroup(rest[0]), args.Flag("with-nodes"));
                    return Finish(doc, args, editor, output);
                case "rename":
                {
                    if (rest.Count < 2)
                        throw new ShaderWaitressException("usage: shaderwaitress group rename <graph> <group> \"New title\"");
                    var group = doc.ResolveGroup(rest[0]);
                    var old = group.Title;
                    group.Title = rest[1];
                    editor.Log.Add($"renamed group \"{old}\" to \"{rest[1]}\"");
                    return Finish(doc, args, editor, output);
                }
                case "add":
                {
                    if (rest.Count < 2)
                        throw new ShaderWaitressException("usage: shaderwaitress group add <graph> <group> <selector>");
                    var group = doc.ResolveGroup(rest[0]);
                    foreach (var node in Ops.Select(doc, rest.Skip(1).ToList(), "group add"))
                        editor.Assign(node, group);
                    return Finish(doc, args, editor, output);
                }
                case "clear":
                    foreach (var node in Ops.Select(doc, rest, "group clear"))
                        editor.Assign(node, null);
                    return Finish(doc, args, editor, output);
                default:
                    throw new ShaderWaitressException($"unknown 'group' subcommand '{sub}'");
            }
        }

        // ------------------------------------------------------------ properties

        public static int Property(IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                throw new ShaderWaitressException("usage: shaderwaitress prop <list|add|rm|set> <graph> ...");
            var sub = argv[0].ToLowerInvariant();
            var args = Args.Parse(argv.Skip(1).ToList(), "with-nodes");
            var doc = Open(args, out var rest, forEdit: sub != "list");
            var editor = new GraphEditor(doc);

            switch (sub)
            {
                case "list":
                    args.RejectUnknown();
                    foreach (var property in doc.Properties)
                    {
                        var readers = doc.Nodes.Count(n => n.IsProperty && n.PropertyId == property.ObjectId);
                        output.WriteLine($"{property.ShortId} {property.TypeLabel,-14} \"{property.Name}\" ref={property.ReferenceName} " +
                                         $"decl={HlslDeclarations.Name(property.Declaration)}{(property.DeclarationOverridden ? string.Empty : " (default)")} " +
                                         (property.HasTextureDefault ? $"default={TextureDefaults.Name(property.TextureDefault)} " : string.Empty) +
                                         (doc.IsSubGraph ? $"promote={(property.Promoted ? "on (on the parent shader)" : "off (a port on the node)")} " : string.Empty) +
                                         $"nodes={readers}");
                    }
                    if (doc.Properties.Count == 0)
                        output.WriteLine("# no properties");
                    return 0;
                case "add":
                {
                    var property = Ops.PropertyAdd(doc, editor, args, rest);
                    output.WriteLine(property.ShortId);
                    return Finish(doc, args, editor, output, structural: false);
                }
                case "rm":
                case "remove":
                    if (rest.Count < 1)
                        throw new ShaderWaitressException("usage: shaderwaitress prop rm <graph> <property> [--with-nodes]");
                    editor.RemoveProperty(doc.ResolveProperty(rest[0]), args.Flag("with-nodes"));
                    return Finish(doc, args, editor, output);
                case "set":
                    Ops.PropertySet(doc, editor, args, rest);
                    return Finish(doc, args, editor, output, structural: false);
                default:
                    throw new ShaderWaitressException($"unknown 'prop' subcommand '{sub}'");
            }
        }

        // -------------------------------------------------------------- keywords

        public static int Keyword(IReadOnlyList<string> argv, TextWriter output)
        {
            var sub = argv.Count > 0 ? argv[0].ToLowerInvariant() : "list";
            var known = sub is "list" or "add" or "rm" or "remove" or "set";
            var args = Args.Parse((known ? argv.Skip(1) : argv).ToList(), "with-nodes");
            var doc = Open(args, out var rest, forEdit: known && sub != "list");
            var editor = new GraphEditor(doc);

            switch (known ? sub : "list")
            {
                case "add":
                {
                    var keyword = Ops.KeywordAdd(doc, editor, args, rest);
                    output.WriteLine(keyword.ShortId);
                    return Finish(doc, args, editor, output, structural: false);
                }
                case "rm":
                case "remove":
                    if (rest.Count < 1)
                        throw new ShaderWaitressException("usage: shaderwaitress keyword rm <graph> <keyword> [--with-nodes]");
                    editor.RemoveKeyword(doc.ResolveKeyword(rest[0]), args.Flag("with-nodes"));
                    return Finish(doc, args, editor, output);
                case "set":
                    Ops.KeywordSet(doc, editor, args, rest);
                    return Finish(doc, args, editor, output, structural: false);
                default:
                    args.RejectUnknown();
                    foreach (var keyword in doc.Keywords)
                    {
                        var readers = doc.Nodes.Count(n => n.IsKeyword && n.KeywordId == keyword.ObjectId);
                        output.WriteLine($"{keyword.ShortId} \"{keyword.Name}\" ref={keyword.ReferenceName} " +
                                         $"type={(keyword.KeywordType == 0 ? "boolean" : "enum")} " +
                                         $"entries=[{string.Join(", ", keyword.Entries)}] default={keyword.Value} " +
                                         $"allow-override={(keyword.AllowDefinitionOverride ? "on (runtime branch)" : "off (#if)")} " +
                                         (doc.IsSubGraph ? $"promote={(keyword.Promoted ? "on (on the parent shader)" : "off (a port on the node)")} " : string.Empty) +
                                         $"nodes={readers}");
                    }
                    if (doc.Keywords.Count == 0)
                        output.WriteLine("# no keywords");
                    return 0;
            }
        }

        // --------------------------------------------------------------- targets

        public static int Target(IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                throw new ShaderWaitressException("usage: shaderwaitress target <show|set> <graph> [key=value]...");
            var sub = argv[0].ToLowerInvariant();
            var args = Args.Parse(argv.Skip(1).ToList(), "sync-blocks");
            var doc = Open(args, out var rest, forEdit: sub != "show");
            var editor = new GraphEditor(doc);

            switch (sub)
            {
                case "show":
                    args.RejectUnknown();
                    foreach (var target in doc.Targets)
                    {
                        output.WriteLine($"{target.ShortId} {target.TypeLabel}/{target.SubTargetLabel}");
                        foreach (var line in TargetSettings.Describe(target))
                            output.WriteLine("  " + line);
                    }
                    if (doc.Targets.Count == 0)
                        output.WriteLine("# no targets");
                    return 0;
                case "set":
                    Ops.TargetSet(doc, editor, args, rest);
                    return Finish(doc, args, editor, output, structural: false);
                default:
                    throw new ShaderWaitressException($"unknown 'target' subcommand '{sub}'");
            }
        }

        // -------------------------------------------------------------- settings

        public static int Settings(IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                throw new ShaderWaitressException("usage: shaderwaitress settings <show|set> <graph> [key=value]...");
            var sub = argv[0].ToLowerInvariant();
            var args = Args.Parse(argv.Skip(1).ToList());
            var doc = Open(args, out var rest, forEdit: sub != "show");
            var editor = new GraphEditor(doc);

            switch (sub)
            {
                case "show":
                    args.RejectUnknown();
                    foreach (var pair in Ops.GraphSettings)
                        output.WriteLine($"{pair.Key} = {doc.RootNode[pair.Value]?.ToJsonString() ?? "(unset)"}");
                    return 0;
                case "set":
                    Ops.SettingsSet(doc, editor, rest);
                    return Finish(doc, args, editor, output, structural: false);
                default:
                    throw new ShaderWaitressException($"unknown 'settings' subcommand '{sub}'");
            }
        }

        // ---------------------------------------------------------------- layout

        public static int Layout(Args args, TextWriter output)
        {
            var doc = Open(args, out _, forEdit: true);
            var settings = new LayoutSettings
            {
                Refine = !args.Flag("no-refine"),
                Force = args.Flag("force"),
                RemeasureAll = args.Flag("remeasure"),
                ColumnGutter = args.GetDouble("gutter", 96),
                RowGap = args.GetDouble("row-gap", 44),
            };
            var verbose = args.Flag("verbose") || args.Flag("v");
            var report = args.Flag("report") || verbose;
            var dryRun = args.Flag("dry-run");
            args.RejectUnknown();

            var engine = new LayoutEngine(doc, settings);
            engine.Run();
            if (report)
            {
                output.WriteLine("before: " + (engine.Before?.Describe() ?? "(empty)"));
                output.WriteLine("after:  " + (engine.After?.Describe() ?? "(empty)"));
                output.WriteLine("method: " + (engine.Kept ? "kept the existing layout" : engine.Repaired ? "nudged the existing layout clear of a wire" : engine.Incremental ? "placed only the new nodes" : "full relayout"));
                foreach (var note in engine.Notes)
                    output.WriteLine("note:   " + note);
                if (verbose)
                {
                    foreach (var line in (engine.After ?? engine.Before)?.Detail ?? Enumerable.Empty<string>())
                        output.WriteLine("  " + line);
                }
            }
            if (engine.Kept)
            {
                output.WriteLine("kept the existing layout: it is sound and scores better than any relayout.");
                return 0;
            }
            if (dryRun)
            {
                output.WriteLine("(dry run, not written)");
                return 0;
            }
            Save(doc, output);
            output.WriteLine($"laid out {doc.Nodes.Count} node(s) in {doc.Path}");
            return 0;
        }

        // ----------------------------------------------------------------- batch

        public static int Apply(Args args, TextWriter output)
        {
            var doc = Open(args, out var rest, forEdit: true);
            var scriptPath = args.Get("script");
            string text;
            if (scriptPath == "-" || args.Flag("stdin"))
                text = Console.In.ReadToEnd();
            else if (scriptPath != null)
                text = File.ReadAllText(scriptPath);
            else if (rest.Count > 0)
                text = File.Exists(rest[0]) ? File.ReadAllText(rest[0]) : string.Join("\n", rest);
            else
                throw new ShaderWaitressException("usage: shaderwaitress apply <graph> <script-file> | --script -");

            var editor = new GraphEditor(doc);
            new Script().Run(doc, editor, text, output);
            return Finish(doc, args, editor, output);
        }

        // ------------------------------------------------------------------ docs

        public static int Skill(Args args, TextWriter output)
        {
            var install = args.Get("install");
            args.RejectUnknown();
            return Waitress.Skill.Write(Help.SkillText(), install, output);
        }

        // --------------------------------------------------------------- helpers

        /// <summary>
        /// Loads the graph named by the first positional argument. With <paramref name="forEdit"/>,
        /// an open Shader Graph window's unsaved edits are saved first, so the tool edits the graph
        /// the person is looking at and their work isn't lost.
        /// </summary>
        public static ShaderGraphDocument Open(Args args, out List<string> rest, bool forEdit = false)
        {
            if (args.Positional.Count == 0)
                throw new ShaderWaitressException("missing graph path");
            var path = args.Positional[0];
            rest = args.Positional.Skip(1).ToList();
            if (forEdit && !args.Flag("dry-run"))
                EditorLink.Prepare(path);
            return ShaderGraphDocument.Load(path);
        }

        /// <summary>Writes the graph, then has the Editor import it and reload any open window.</summary>
        static void Save(ShaderGraphDocument doc, TextWriter output, string path = null)
        {
            doc.Save(path);
            var refreshed = EditorLink.Refresh(path ?? doc.Path);
            if (refreshed != null && refreshed.StartsWith("error", StringComparison.Ordinal))
                output.WriteLine("# warning: " + refreshed);
            else if (refreshed != null && refreshed.Contains("unsaved", StringComparison.Ordinal))
                output.WriteLine("# " + refreshed);
        }

        static int Finish(ShaderGraphDocument doc, Args args, GraphEditor editor, TextWriter output, bool structural = true)
        {
            var dryRun = args.Flag("dry-run");
            var noLayout = args.Flag("no-layout");
            args.RejectUnknown();

            foreach (var entry in editor.Log)
                output.WriteLine(entry);

            if (structural && editor.StructureChanged && !noLayout)
                new LayoutEngine(doc).Run();

            if (dryRun)
            {
                output.WriteLine("--- dry run, not written ---");
                output.Write(DenseText.Render(doc));
                return 0;
            }
            Save(doc, output);
            return 0;
        }
    }
}
