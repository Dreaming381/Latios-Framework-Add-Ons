using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ShaderWaitress.Cli;
using ShaderWaitress.Model;

namespace ShaderWaitress.Edit
{
    /// <summary>
    /// The batch language behind `apply`. One command per line, the same verbs as the CLI but
    /// with the graph path omitted, plus `$label` bindings so a node created on one line can be
    /// wired on the next. The whole script runs against one in-memory document and is written
    /// once, so a failure part-way leaves the file untouched.
    /// </summary>
    public sealed class Script
    {
        readonly ScriptText m_Text = new ScriptText(message => new ShaderWaitressException(message));

        public void Run(ShaderGraphDocument doc, GraphEditor editor, string text, TextWriter output)
        {
            foreach (var statement in m_Text.Statements(text))
            {
                try
                {
                    Execute(doc, editor, statement.Text, output);
                }
                catch (ShaderWaitressException e)
                {
                    throw new ShaderWaitressException($"{statement.Where}: {statement.Text}{Environment.NewLine}  {e.Message}");
                }
            }
        }

        void Execute(ShaderGraphDocument doc, GraphEditor editor, string line, TextWriter output)
        {
            var argv = m_Text.Arguments(line);
            if (argv.Count == 0)
                return;

            var verb = argv[0].ToLowerInvariant();
            var sub = argv.Count > 1 ? argv[1].ToLowerInvariant() : string.Empty;

            switch (verb)
            {
                case "node":
                {
                    var args = Args.Parse(argv.Skip(2).ToList(), "reconnect");
                    var rest = args.Positional;
                    switch (sub)
                    {
                        case "add":
                        {
                            var node = Ops.NodeAdd(doc, editor, args, rest);
                            Bind(args.Get("as"), node.ObjectId);
                            args.RejectUnknown();
                            return;
                        }
                        case "insert":
                        {
                            var inserted = Ops.NodeInsert(doc, editor, args, rest);
                            if (args.Get("as") != null && inserted.Count != 1)
                                throw new ShaderWaitressException(
                                    $"--as needs exactly one new node, but --after matched {inserted.Count}");
                            if (inserted.Count == 1)
                                Bind(args.Get("as"), inserted[0].ObjectId);
                            args.RejectUnknown();
                            return;
                        }
                        case "rm":
                        case "remove":
                            Ops.NodeRemove(doc, editor, args, rest);
                            args.RejectUnknown();
                            return;
                        case "set":
                            Ops.NodeSet(doc, editor, args, rest);
                            args.RejectUnknown();
                            return;
                        case "replace":
                            Ops.NodeReplace(doc, editor, args, rest);
                            args.RejectUnknown();
                            return;
                        case "port":
                        {
                            var action = rest.Count > 0 ? rest[0].ToLowerInvariant() : string.Empty;
                            var remainder = rest.Skip(1).ToList();
                            if (action == "add")
                                Ops.PortAdd(doc, editor, args, remainder);
                            else if (action is "rm" or "remove")
                                Ops.PortRemove(doc, editor, args, remainder);
                            else
                                throw new ShaderWaitressException($"unknown 'node port' action '{action}'; use add or rm");
                            args.RejectUnknown();
                            return;
                        }
                        default:
                            throw new ShaderWaitressException($"unknown 'node' subcommand '{sub}'");
                    }
                }

                case "wire":
                    Ops.Wire(doc, editor, Args.Parse(argv.Skip(1).ToList()).Positional);
                    return;

                case "unwire":
                    Ops.Unwire(doc, editor, Args.Parse(argv.Skip(1).ToList()).Positional);
                    return;

                case "group":
                {
                    var args = Args.Parse(argv.Skip(2).ToList(), "with-nodes");
                    var rest = args.Positional;
                    switch (sub)
                    {
                        case "new":
                        {
                            var group = Ops.GroupNew(doc, editor, rest);
                            Bind(args.Get("as"), group.ObjectId);
                            args.RejectUnknown();
                            return;
                        }
                        case "rm":
                        case "remove":
                            editor.RemoveGroup(doc.ResolveGroup(rest[0]), args.Flag("with-nodes"));
                            args.RejectUnknown();
                            return;
                        case "rename":
                        {
                            var group = doc.ResolveGroup(rest[0]);
                            group.Title = rest[1];
                            editor.Log.Add($"renamed group to \"{rest[1]}\"");
                            args.RejectUnknown();
                            return;
                        }
                        case "add":
                        {
                            var group = doc.ResolveGroup(rest[0]);
                            foreach (var node in Ops.Select(doc, rest.Skip(1).ToList(), "group add"))
                                editor.Assign(node, group);
                            args.RejectUnknown();
                            return;
                        }
                        case "clear":
                            foreach (var node in Ops.Select(doc, rest, "group clear"))
                                editor.Assign(node, null);
                            args.RejectUnknown();
                            return;
                        default:
                            throw new ShaderWaitressException($"unknown 'group' subcommand '{sub}'");
                    }
                }

                case "prop":
                case "property":
                {
                    var args = Args.Parse(argv.Skip(2).ToList(), "with-nodes");
                    var rest = args.Positional;
                    switch (sub)
                    {
                        case "add":
                        {
                            var property = Ops.PropertyAdd(doc, editor, args, rest);
                            Bind(args.Get("as"), property.ObjectId);
                            args.RejectUnknown();
                            return;
                        }
                        case "rm":
                        case "remove":
                            editor.RemoveProperty(doc.ResolveProperty(rest[0]), args.Flag("with-nodes"));
                            args.RejectUnknown();
                            return;
                        case "set":
                            Ops.PropertySet(doc, editor, args, rest);
                            args.RejectUnknown();
                            return;
                        default:
                            throw new ShaderWaitressException($"unknown 'prop' subcommand '{sub}'");
                    }
                }

                case "block":
                {
                    var args = Args.Parse(argv.Skip(2).ToList());
                    var rest = args.Positional;
                    switch (sub)
                    {
                        case "add":
                            foreach (var descriptor in rest)
                                Bind(args.Get("as"), Ops.BlockAdd(doc, editor, descriptor).ObjectId);
                            args.RejectUnknown();
                            return;
                        case "rm":
                        case "remove":
                            Ops.BlockRemove(doc, editor, rest);
                            args.RejectUnknown();
                            return;
                        default:
                            throw new ShaderWaitressException($"unknown 'block' subcommand '{sub}'");
                    }
                }

                case "keyword":
                {
                    var args = Args.Parse(argv.Skip(2).ToList(), "with-nodes");
                    var rest = args.Positional;
                    switch (sub)
                    {
                        case "add":
                        {
                            var keyword = Ops.KeywordAdd(doc, editor, args, rest);
                            Bind(args.Get("as"), keyword.ObjectId);
                            args.RejectUnknown();
                            return;
                        }
                        case "rm":
                        case "remove":
                            editor.RemoveKeyword(doc.ResolveKeyword(rest[0]), args.Flag("with-nodes"));
                            args.RejectUnknown();
                            return;
                        case "set":
                            Ops.KeywordSet(doc, editor, args, rest);
                            args.RejectUnknown();
                            return;
                        default:
                            throw new ShaderWaitressException($"unknown 'keyword' subcommand '{sub}'");
                    }
                }

                case "target":
                {
                    if (sub != "set")
                        throw new ShaderWaitressException("only 'target set' is available in a script");
                    var args = Args.Parse(argv.Skip(2).ToList(), "sync-blocks");
                    Ops.TargetSet(doc, editor, args, args.Positional);
                    args.RejectUnknown();
                    return;
                }

                case "settings":
                {
                    if (sub != "set")
                        throw new ShaderWaitressException("only 'settings set' is available in a script");
                    Ops.SettingsSet(doc, editor, Args.Parse(argv.Skip(2).ToList()).Positional);
                    return;
                }

                case "echo":
                    output.WriteLine(string.Join(" ", argv.Skip(1)));
                    return;

                default:
                    if (verb.StartsWith("-", StringComparison.Ordinal))
                        throw new ShaderWaitressException(
                            $"a script line cannot start with '{argv[0]}'. If this is meant to continue the line above, " +
                            "end that line with a backslash.");
                    throw new ShaderWaitressException($"unknown command '{verb}'. See 'shaderwaitress help edit'.");
            }
        }

        void Bind(string label, string objectId) => m_Text.Bind(label, objectId);
    }
}
