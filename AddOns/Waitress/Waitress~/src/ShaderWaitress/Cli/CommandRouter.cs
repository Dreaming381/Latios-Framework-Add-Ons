using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShaderWaitress.Testing;

namespace ShaderWaitress.Cli
{
    public static class CommandRouter
    {
        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            if (argv.Count == 0)
            {
                Help.WriteIndex(output);
                return 1;
            }

            // --offline and --catalog apply to every command and can come before it.
            var words = argv.ToList();
            while (words.Remove("--offline"))
                Edit.EditorLink.Bridge.Offline = true;
            var catalogAt = words.IndexOf("--catalog");
            if (catalogAt >= 0 && catalogAt + 1 < words.Count)
            {
                Catalog.NodeCatalog.OverridePath = words[catalogAt + 1];
                words.RemoveRange(catalogAt, 2);
            }
            if (words.Count == 0)
            {
                Help.WriteIndex(output);
                return 1;
            }

            var command = words[0];
            var rest = words.Skip(1).ToList();

            if (command is "-h" or "--help" or "help")
            {
                Help.WriteTopic(rest.Count > 0 ? rest[0] : null, output);
                return 0;
            }
            if (command is "--version" or "version")
            {
                output.WriteLine(Help.Version);
                return 0;
            }

            switch (command)
            {
                case "show":
                case "dump":
                    return Commands.Show(Args.Parse(rest), output);
                case "q":
                case "query":
                    return Commands.Query(Args.Parse(rest), output);
                case "node":
                    return Commands.Node(rest, output);
                case "wire":
                    return Commands.Wire(Args.Parse(rest), output);
                case "unwire":
                    return Commands.Unwire(Args.Parse(rest), output);
                case "block":
                    return Commands.Block(rest, output);
                case "group":
                    return Commands.Group(rest, output);
                case "prop":
                case "property":
                    return Commands.Property(rest, output);
                case "keyword":
                    return Commands.Keyword(rest, output);
                case "target":
                    return Commands.Target(rest, output);
                case "settings":
                    return Commands.Settings(rest, output);
                case "layout":
                    return Commands.Layout(Args.Parse(rest), output);
                case "validate":
                    return Commands.Validate(Args.Parse(rest, "editor"), output);
                case "new":
                    return Commands.New(Args.Parse(rest), output);
                case "apply":
                    return Commands.Apply(Args.Parse(rest), output);
                case "nodes":
                case "catalog":
                    return Commands.Catalog(Args.Parse(rest), output);
                case "skill":
                    return Commands.Skill(Args.Parse(rest), output);
                case "roundtrip":
                {
                    var args = Args.Parse(rest);
                    var roots = args.Positional.Count > 0 ? args.Positional : new List<string> { Directory.GetCurrentDirectory() };
                    var code = RoundTripCheck.Run(roots, output, args.Flag("verbose") || args.Flag("v"));
                    args.RejectUnknown();
                    return code;
                }
                case "selftest":
                    Edit.EditorLink.Bridge.Offline = true;
                    return SelfTest.Run(Args.Parse(rest), output);
                case "sweep":
                    return CorpusSweep.Run(Args.Parse(rest), output);
                default:
                    error.WriteLine($"error: unknown command '{command}'");
                    Help.WriteIndex(error);
                    return 1;
            }
        }
    }
}
