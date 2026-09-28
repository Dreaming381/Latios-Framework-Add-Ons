using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxWaitress.Catalog;
using VfxWaitress.Model;
using VfxWaitress.Query;

namespace VfxWaitress.Edit
{
    /// <summary>
    /// The edit verbs, run either one at a time from the command line or many at once by `apply`.
    /// A script is the CLI verbs with the graph path left out, one per line, plus <c>--as label</c>
    /// on a line that creates something so later lines can refer to it as <c>$label</c>. The whole
    /// script edits one in-memory asset, which is written once at the end, so a failure part-way
    /// leaves the file untouched.
    /// </summary>
    public sealed class Script
    {
        readonly ScriptText m_Text = new ScriptText(message => new VfxWaitressException(message));

        public void Run(VfxAsset asset, GraphEditor editor, string text, TextWriter output)
        {
            foreach (var statement in m_Text.Statements(text))
            {
                try
                {
                    Execute(asset, editor, m_Text.Arguments(statement.Text), output);
                }
                catch (VfxWaitressException e)
                {
                    throw new VfxWaitressException($"{statement.Where}: {statement.Text}{Environment.NewLine}  {e.Message}");
                }
            }
        }

        /// <summary>Runs one verb against the asset. <paramref name="argv"/> starts with the verb.</summary>
        public void Execute(VfxAsset asset, GraphEditor editor, IReadOnlyList<string> argv, TextWriter output)
        {
            if (argv.Count == 0)
                return;
            var verb = argv[0].ToLowerInvariant();
            var args = Args.Parse(argv.Skip(1).ToList());
            var rest = args.Positional;
            switch (verb)
            {
                case "node":
                case "block":
                {
                    if (rest.Count == 0 || rest[0] != "add")
                        throw new VfxWaitressException($"unknown '{verb}' subcommand '{rest.FirstOrDefault()}'; use '{verb} add'");
                    VfxNode context = null;
                    var index = 1;
                    if (verb == "block")
                    {
                        context = Node(asset, At(rest, 1, "context"));
                        if (context.Kind != "context")
                            throw new VfxWaitressException($"{context.ShortId} is a {context.Kind}, not a context");
                        index = 2;
                    }
                    var model = ResolveModel(asset.Catalog, At(rest, index, "model type or menu name"), verb == "block" ? "block" : null);
                    var node = editor.AddNode(model, context, KeyValues(rest.Skip(index + 1)));
                    foreach (var wire in args.GetAll("wire"))
                        WireInto(asset, editor, node, wire);
                    foreach (var feed in args.GetAll("feed"))
                        FeedFrom(asset, editor, node, feed);
                    m_Text.Bind(args.Get("as"), node.FileId.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    break;
                }

                case "rm":
                {
                    var hits = Selector.Resolve(asset, At(rest, 0, "node selector")).ToList();
                    if (hits.Count == 0)
                        throw new VfxWaitressException($"no node matches '{rest[0]}'");
                    foreach (var hit in hits)
                        editor.RemoveNode(hit);
                    break;
                }

                case "wire":
                    editor.Wire(SlotRef.Resolve(asset, At(rest, 0, "source node.port"), false),
                                SlotRef.Resolve(asset, At(rest, 1, "destination node.port"), true));
                    break;

                case "unwire":
                    editor.Unwire(SlotRef.Resolve(asset, At(rest, 0, "node.port"), true));
                    break;

                case "set":
                {
                    var target = At(rest, 0, "node.port or node");
                    var value = At(rest, 1, "value");
                    var setting = args.Get("setting");
                    if (setting != null)
                    {
                        var node = Node(asset, target);
                        editor.SetSetting(node, node.Model, setting, value);
                    }
                    else
                    {
                        editor.SetValue(SlotRef.Resolve(asset, target, true), value);
                    }
                    break;
                }

                case "link":
                    editor.LinkContexts(Node(asset, At(rest, 0, "source context")), args.GetInt("from-slot", 0),
                                        Node(asset, At(rest, 1, "destination context")), args.GetInt("to-slot", 0));
                    break;

                case "system":
                {
                    if (rest.Count == 0 || rest[0] != "set")
                        throw new VfxWaitressException($"unknown 'system' subcommand '{rest.FirstOrDefault()}'; use 'system set'");
                    var systemRef = At(rest, 1, "system id");
                    var data = asset.Datas.Values.FirstOrDefault(d => string.Equals(d.ShortId, systemRef, StringComparison.OrdinalIgnoreCase))
                               ?? throw new VfxWaitressException($"no system matches '{systemRef}'; `show` lists them as `system d…`");
                    foreach (var pair in KeyValues(rest.Skip(2)))
                        editor.SetDataSetting(data, pair.Key, pair.Value);
                    break;
                }

                case "echo":
                    output.WriteLine(string.Join(" ", rest));
                    break;

                case "attr":
                case "resync":
                case "layout":
                case "repair":
                    throw new VfxWaitressException(
                        $"'{verb}' can't run inside a script, because it goes through the Editor or rearranges the whole graph. " +
                        "Run it as its own command before or after the script.");

                default:
                    if (verb.StartsWith("-", StringComparison.Ordinal))
                        throw new VfxWaitressException(
                            $"a script line can't start with '{argv[0]}'. If this continues the line above, end that line with a backslash.");
                    throw new VfxWaitressException($"unknown command '{verb}'. See 'vfxwaitress help edit'.");
            }
            args.RejectUnknown();
        }

        /// <summary><c>--wire Port=source[.Port]</c>: feeds one of the new node's inputs.</summary>
        static void WireInto(VfxAsset asset, GraphEditor editor, VfxNode node, string spec)
        {
            var eq = spec.IndexOf('=');
            if (eq <= 0)
                throw new VfxWaitressException($"--wire expects Port=source, got '{spec}'");
            var input = SlotRef.Resolve(asset, node.FileId + "." + spec.Substring(0, eq), true);
            editor.Wire(SlotRef.Resolve(asset, spec.Substring(eq + 1), false), input);
        }

        /// <summary><c>--feed [Port=]destination.Port</c>: wires one of the new node's outputs onward.</summary>
        static void FeedFrom(VfxAsset asset, GraphEditor editor, VfxNode node, string spec)
        {
            var eq = spec.IndexOf('=');
            var own = eq > 0 ? node.FileId + "." + spec.Substring(0, eq) : node.FileId.ToString(System.Globalization.CultureInfo.InvariantCulture);
            editor.Wire(SlotRef.Resolve(asset, own, false), SlotRef.Resolve(asset, spec.Substring(eq + 1), true));
        }

        static string At(IReadOnlyList<string> rest, int index, string what) =>
            index < rest.Count ? rest[index] : throw new VfxWaitressException("missing " + what);

        static VfxNode Node(VfxAsset asset, string reference)
        {
            var hits = Selector.Resolve(asset, reference).ToList();
            if (hits.Count == 0)
                throw new VfxWaitressException($"no node matches '{reference}'");
            if (hits.Count > 1)
                throw new VfxWaitressException($"'{reference}' matches {hits.Count} nodes: {string.Join(" ", hits.Select(n => n.ShortId))}");
            return hits[0];
        }

        public static Dictionary<string, string> KeyValues(IEnumerable<string> tokens)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var token in tokens)
            {
                var eq = token.IndexOf('=');
                if (eq < 0)
                    throw new VfxWaitressException($"expected setting=value, got '{token}'");
                result[token.Substring(0, eq)] = token.Substring(eq + 1);
            }
            return result;
        }

        public static CatalogModel ResolveModel(NodeCatalog catalog, string name, string kind)
        {
            catalog.Require();
            var exact = catalog.ByType(name);
            if (exact != null && (kind == null || exact.Kind == kind))
                return exact;
            var candidates = catalog.Search(name).Where(m => kind == null || m.Kind == kind).ToList();
            if (candidates.Count == 1)
                return candidates[0];
            if (candidates.Count == 0)
                throw new VfxWaitressException($"no model matches '{name}'; try: vfxwaitress catalog '*{name}*'");
            // Prefer the base entry when the variants of one type all match.
            var bases = candidates.Where(m => m.VariantOf == null).ToList();
            if (bases.Count == 1)
                return bases[0];
            throw new VfxWaitressException(
                $"'{name}' matches {candidates.Count} models; name one exactly:" + Environment.NewLine +
                string.Join(Environment.NewLine, candidates.Take(15).Select(c => $"  {c.Type}   {c.Name}   [{c.Category}]")));
        }
    }
}
