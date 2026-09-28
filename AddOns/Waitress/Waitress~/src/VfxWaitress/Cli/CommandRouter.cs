using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VfxWaitress.Cli
{
    public static class CommandRouter
    {
        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            if (argv.Count == 0)
            {
                Help.Index(output);
                return 0;
            }

            // --offline applies to every verb and can come before the verb, so strip it from the
            // whole line.
            var words = argv.ToList();
            while (words.Remove("--offline"))
                Edit.EditorLink.Offline = true;
            if (words.Count == 0)
            {
                Help.Index(output);
                return 0;
            }

            var verb = words[0];
            var rest = words.Skip(1).ToList();

            // --catalog applies to every verb, so it is stripped before the verb parses its own.
            for (var i = 0; i < rest.Count - 1; i++)
            {
                if (rest[i] != "--catalog")
                    continue;
                Catalog.NodeCatalog.OverridePath = rest[i + 1];
                rest.RemoveRange(i, 2);
                break;
            }

            switch (verb)
            {
                case "help":
                case "--help":
                case "-h":
                    Help.Topic(output, rest.FirstOrDefault());
                    return 0;

                case "show":
                {
                    var args = Args.Parse(rest);
                    var path = args.Positional0("graph path");
                    var positions = args.Flag("positions");
                    var node = args.Get("node");
                    args.RejectUnknown();
                    var asset = Model.VfxAsset.Load(path);
                    if (node == null)
                    {
                        output.Write(Model.DenseText.Render(asset, positions));
                        return 0;
                    }
                    var hits = Query.Selector.Resolve(asset, node).ToList();
                    if (hits.Count == 0)
                        throw new VfxWaitressException($"no node matches '{node}'");
                    foreach (var hit in hits)
                        output.Write(Model.NodeDetail.Render(asset, hit));
                    return 0;
                }

                case "catalog":
                case "nodes":
                {
                    var args = Args.Parse(rest);
                    var pattern = args.Positional.Count > 0 ? args.Positional[0] : "*";
                    var kind = args.Get("kind");
                    var category = args.Get("category");
                    args.RejectUnknown();
                    var catalog = Catalog.NodeCatalog.Default;
                    catalog.Require();
                    var hits = catalog.Search(pattern)
                        .Where(m => kind == null || string.Equals(m.Kind, kind, StringComparison.OrdinalIgnoreCase))
                        .Where(m => category == null || Glob.CompileLoose("*" + category + "*")(m.Category))
                        .OrderBy(m => m.Category, StringComparer.Ordinal).ThenBy(m => m.Name, StringComparer.Ordinal).ToList();
                    foreach (var m in hits)
                    {
                        var variant = m.VariantSettings.Count == 0 ? "" : "  " + string.Join(" ", m.VariantSettings.Select(kv => kv.Key + "=" + kv.Value));
                        output.WriteLine($"{m.Kind,-9} {m.ShortType,-32} {m.Name}   [{m.Category}]{variant}");
                    }
                    output.WriteLine($"# {hits.Count} of {catalog.Models.Count} (VFX Graph {catalog.VfxGraphVersion}, Unity {catalog.UnityVersion})");
                    return 0;
                }

                case "model":
                {
                    var args = Args.Parse(rest);
                    var name = args.Positional0("model type or menu name");
                    args.RejectUnknown();
                    var catalog = Catalog.NodeCatalog.Default;
                    catalog.Require();
                    var model = catalog.ByType(name);
                    if (model == null)
                    {
                        var candidates = catalog.Search(name).Take(12).ToList();
                        if (candidates.Count == 0)
                            throw new VfxWaitressException($"no model matches '{name}'; try: vfxwaitress catalog '*{name}*'");
                        if (candidates.Count > 1)
                        {
                            error.WriteLine($"'{name}' matches {candidates.Count}:");
                            foreach (var c in candidates)
                                error.WriteLine($"  {c.Type}   {c.Name}");
                            return 2;
                        }
                        model = candidates[0];
                    }
                    output.Write(Model.NodeDetail.RenderModel(model));
                    return 0;
                }

                case "node":
                case "block":
                case "system":
                    return RunEdit(words, 2, output);

                case "rm":
                case "wire":
                case "unwire":
                case "set":
                case "link":
                    return RunEdit(words, 1, output);

                case "apply":
                {
                    var args = Args.Parse(rest);
                    var path = args.Positional0("graph path");
                    var scriptPath = args.Get("script") ?? (args.Positional.Count > 1 ? args.Positional[1] : null);
                    var dryRun = args.Flag("dry-run");
                    args.RejectUnknown();
                    if (scriptPath == null)
                        throw new VfxWaitressException("usage: vfxwaitress apply <vfx> <script-file> | --script -");
                    var text = scriptPath == "-" ? Console.In.ReadToEnd() : File.ReadAllText(scriptPath);
                    var asset = LoadForEdit(path);
                    var editor = new Edit.GraphEditor(asset);
                    new Edit.Script().Run(asset, editor, text, output);
                    return Finish(editor, output, dryRun, path);
                }

                case "skill":
                {
                    var args = Args.Parse(rest);
                    var install = args.Get("install");
                    args.RejectUnknown();
                    return Help.Skill(install, output);
                }

                case "new":
                {
                    var args = Args.Parse(rest);
                    var path = args.Positional0("path for the new asset");
                    args.RejectUnknown();
                    if (File.Exists(path))
                        throw new VfxWaitressException(path + " already exists");
                    var extension = Path.GetExtension(path).ToLowerInvariant();
                    if (extension != ".vfx" && extension != ".vfxoperator" && extension != ".vfxblock")
                        throw new VfxWaitressException("a new asset needs a .vfx, .vfxoperator or .vfxblock extension");
                    Edit.GraphEditor.CreateEmpty(path, extension != ".vfx");
                    output.WriteLine("created " + path);
                    return 0;
                }

                case "attr":
                {
                    var args = Args.Parse(rest);
                    var sub = args.Positional0("subcommand (add)");
                    if (sub != "add" && sub != "set")
                        throw new VfxWaitressException($"unknown 'attr' subcommand '{sub}'");
                    var path = args.PositionalAt(1, "graph path");
                    var name = args.PositionalAt(2, "attribute name");
                    var type = args.PositionalAt(3, "attribute type");
                    var description = args.Positional.Count > 4 ? args.Positional[4] : "";
                    args.RejectUnknown();
                    var report = Edit.EditorLink.Bridge.Require(path, "AddAttribute", $"{name},{type},{description}");
                    output.WriteLine(report);
                    return report.StartsWith("error", StringComparison.Ordinal) ? 2 : 0;
                }

                case "attrs":
                {
                    var args = Args.Parse(rest);
                    if (args.Positional.Count == 0)
                        throw new VfxWaitressException("missing graph path");
                    args.RejectUnknown();

                    // Across several graphs, this also reports where they disagree on an
                    // attribute's type.
                    var declared = new Dictionary<string, List<(string graph, string type)>>(StringComparer.Ordinal);
                    foreach (var path in Testing.RoundTrip.Find(args.Positional))
                    {
                        var asset = Model.VfxAsset.Load(path);
                        var lines = Model.DenseText.CustomAttributeLines(asset).ToList();
                        if (args.Positional.Count == 1 && File.Exists(args.Positional[0]))
                        {
                            foreach (var line in lines)
                                output.WriteLine(line);
                            if (lines.Count == 0)
                                output.WriteLine("# this graph declares no custom attributes");
                        }
                        foreach (var line in lines)
                        {
                            var parts = line.Split(' ');
                            if (parts.Length < 3)
                                continue;
                            if (!declared.TryGetValue(parts[1], out var list))
                                declared[parts[1]] = list = new List<(string, string)>();
                            list.Add((Path.GetFileName(path), parts[2]));
                        }
                    }

                    if (args.Positional.Count > 1 || !File.Exists(args.Positional[0]))
                    {
                        foreach (var kvp in declared.OrderBy(k => k.Key, StringComparer.Ordinal))
                            output.WriteLine($"attr {kvp.Key} {string.Join(", ", kvp.Value.Select(v => v.type + " in " + v.graph))}");
                    }

                    var conflicts = declared.Where(k => k.Value.Select(v => v.type).Distinct().Count() > 1).ToList();
                    foreach (var conflict in conflicts)
                    {
                        output.WriteLine($"error: {conflict.Key} is declared as " +
                                         string.Join(" and ", conflict.Value.Select(v => v.type + " in " + v.graph)) +
                                         "; graphs sharing an attribute name should agree on its type");
                    }
                    return conflicts.Count == 0 ? 0 : 1;
                }

                case "repair":
                {
                    var args = Args.Parse(rest);
                    var dryRun = args.Flag("dry-run");
                    args.RejectUnknown();
                    if (args.Positional.Count == 0)
                        throw new VfxWaitressException("missing graph path");
                    var touched = 0;
                    foreach (var path in Testing.RoundTrip.Find(args.Positional))
                    {
                        var asset = LoadForEdit(path);
                        var editor = new Edit.GraphEditor(asset);
                        if (editor.Repair() == 0)
                            continue;
                        touched++;
                        output.WriteLine(path);
                        foreach (var line in editor.Log)
                            output.WriteLine("  " + line);
                        if (!dryRun)
                        {
                            editor.Save();
                            Edit.EditorLink.Refresh(path);
                        }
                    }
                    output.WriteLine(touched == 0
                        ? "# nothing to repair"
                        : $"# repaired {touched} asset(s)" + (dryRun ? " (--dry-run: nothing written)" : ""));
                    return 0;
                }
                case "layout":
                {
                    var args = Args.Parse(rest);
                    var path = args.Positional0("graph path");
                    var dryRun = args.Flag("dry-run");
                    args.RejectUnknown();
                    // The file stores no node sizes, so ask the graph view for real ones.
                    Edit.EditorLink.Prepare(path);
                    var measured = Edit.EditorLink.Sizes(path);
                    Layout.LayoutEngine.Measured = measured;

                    // An operator feeding several systems can sit in the margin of the first, or
                    // in a lane above the columns. Lay out both and keep the better one.
                    Edit.GraphEditor editor = null;
                    var best = int.MaxValue;
                    var tried = new List<string>();
                    foreach (var lift in new[] { false, true })
                    {
                        var candidate = Plan(path, lift, out var quality);
                        tried.Add((lift ? "lifted " : "inline ") + quality.cost);
                        if (quality.cost >= best)
                            continue;
                        best = quality.cost;
                        editor = candidate;
                    }
                    editor.Note("arrangements measured: " + string.Join(", ", tried));
                    editor.Note(measured.IsEmpty
                        ? "warning: no Editor answered, so node sizes are estimates and wires will not meet their ports exactly"
                        : $"sized {measured.Boxes.Count} node(s) and {measured.PortY.Count} port(s) from the live graph view");
                    return Finish(editor, output, dryRun, path);
                }

                case "resync":
                {
                    var args = Args.Parse(rest);
                    var path = args.Positional0("graph path");
                    args.RejectUnknown();
                    var report = Edit.EditorLink.Bridge.Require(path, "Resync");
                    output.WriteLine(report);
                    return report.StartsWith("error", StringComparison.Ordinal) ? 2 : 0;
                }

                case "validate":
                {
                    var args = Args.Parse(rest, "editor");
                    var path = args.Positional0("graph path");
                    var useEditor = args.Flag("editor");
                    args.RejectUnknown();
                    var asset = Model.VfxAsset.Load(path);
                    var findings = Model.Validator.Check(asset);
                    foreach (var f in findings)
                        output.WriteLine(f.ToString());
                    var errors = findings.Count(f => f.Severity == "error");

                    if (!useEditor)
                    {
                        output.WriteLine(findings.Count == 0
                            ? "no structural problems found — run with --editor to compile it, which is the only real test"
                            : $"{errors} error(s), {findings.Count - errors} warning(s); run with --editor to compile it too");
                        return errors == 0 ? 0 : 1;
                    }

                    var report = Edit.EditorLink.Bridge.Require(path, "Validate");
                    output.WriteLine(report);
                    // A one-line error means the compile never ran.
                    if (report.StartsWith("error:", StringComparison.Ordinal) && report.IndexOf('\n') < 0)
                        return 2;
                    var editorErrors = report.Split('\n').Any(l => l.TrimStart().StartsWith("error:", StringComparison.Ordinal));
                    return errors == 0 && !editorErrors ? 0 : 1;
                }

                case "q":
                case "query":
                {
                    var args = Args.Parse(rest);
                    var path = args.Positional0("graph path");
                    var expression = args.PositionalAt(1, "selector");
                    var format = args.Get("format", "ids");
                    args.RejectUnknown();
                    var asset = Model.VfxAsset.Load(path);
                    var hits = Query.Selector.Resolve(asset, expression).ToList();
                    switch (format)
                    {
                        case "count":
                            output.WriteLine(hits.Count);
                            break;
                        case "long":
                            foreach (var n in hits)
                                output.Write(Model.NodeDetail.Render(asset, n));
                            break;
                        default:
                            foreach (var n in hits)
                                output.WriteLine($"{n.ShortId} {n.Kind} {n.DisplayType}{(n.Label != null ? " \"" + n.Label + "\"" : "")}{(n.ExposedName != null ? " \"" + n.ExposedName + "\"" : "")}");
                            break;
                    }
                    return 0;
                }

                case "selftest":
                    return Testing.SelfTest.Run(Args.Parse(rest), output);

                case "roundtrip":
                case "sweep":
                {
                    var args = Args.Parse(rest);
                    var roots = args.Positional.Count > 0 ? args.Positional.ToList() : new List<string> { "." };
                    var verbose = args.Flag("verbose") || args.Flag("v");
                    args.RejectUnknown();
                    return verb == "sweep"
                        ? Testing.Sweep.Run(roots, output, verbose)
                        : Testing.RoundTrip.Run(roots, output, verbose);
                }

                default:
                    error.WriteLine($"unknown command '{verb}'");
                    Help.Index(error);
                    return 2;
            }
        }

        /// <summary>
        /// Runs one edit verb from the command line. The graph path is the positional argument at
        /// <paramref name="pathPosition"/> (counting from 1 after the verb). Taking it out leaves
        /// exactly the line `apply` would run.
        /// </summary>
        static int RunEdit(List<string> words, int pathPosition, TextWriter output)
        {
            var dryRun = words.RemoveAll(w => w == "--dry-run") > 0;
            var positional = Args.Parse(words.Skip(1).ToList()).Positional;
            if (positional.Count < pathPosition)
                throw new VfxWaitressException("missing graph path");
            var path = positional[pathPosition - 1];
            var argv = new List<string>(words);
            argv.RemoveAt(argv.IndexOf(path, 1));

            var asset = LoadForEdit(path);
            var editor = new Edit.GraphEditor(asset);
            new Edit.Script().Execute(asset, editor, argv, output);
            return Finish(editor, output, dryRun, path);
        }

        /// <summary>
        /// Lays one graph out one way, on its own copy, so two arrangements can be compared
        /// before either is written.
        /// </summary>
        static Edit.GraphEditor Plan(string path, bool lift, out (int crossings, int steep, int cost) quality)
        {
            Layout.LayoutEngine.Lift = lift;
            var asset = Model.VfxAsset.Load(path);
            var editor = new Edit.GraphEditor(asset);
            foreach (var line in Layout.LayoutEngine.Apply(asset, out quality))
                editor.Note(line);
            return editor;
        }

        /// <summary>
        /// Loads a graph that's about to be written. Whatever an open VFX Graph window hasn't
        /// saved gets flushed first, so the tool edits the graph the person is looking at and
        /// their unsaved work isn't lost.
        /// </summary>
        static Model.VfxAsset LoadForEdit(string path)
        {
            Edit.EditorLink.Prepare(path);
            return Model.VfxAsset.Load(path);
        }

        static int Finish(Edit.GraphEditor editor, TextWriter output, bool dryRun, string path = null)
        {
            editor.PlaceAdded();
            foreach (var line in editor.Log)
                output.WriteLine(line);
            if (dryRun)
            {
                output.WriteLine("# --dry-run: nothing written");
                return 0;
            }
            editor.Save();
            // An open window still holds its old copy and would overwrite this file on its next
            // save. Hand the file back so the Editor picks it up.
            if (path != null)
            {
                var settled = Edit.EditorLink.Settle(path);
                if (settled != null)
                    output.WriteLine("# the Editor wrote the final file: " + settled);
                else if (!Edit.EditorLink.Offline)
                    output.WriteLine("# warning: the file is the tool's own bytes, because " +
                                     (Edit.EditorLink.Bridge.LastProblem ?? "no Editor answered"));
            }
            return 0;
        }
    }
}
