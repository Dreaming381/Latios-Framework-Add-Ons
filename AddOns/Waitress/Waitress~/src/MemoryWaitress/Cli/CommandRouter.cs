using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryWaitress.Commands;
using MemoryWaitress.Snap;

namespace MemoryWaitress.Cli
{
    public static class CommandRouter
    {
        static readonly string[] s_Switches = { "play", "editor", "all", "native", "managed", "version", "no-info", "callstacks" };

        delegate int SnapCommand(Model m, Args args, TextWriter w);

        static readonly Dictionary<string, (SnapCommand run, int positionals)> s_SnapCommands = new Dictionary<string, (SnapCommand, int)>(StringComparer.OrdinalIgnoreCase)
        {
            ["info"] = (Info.Run, 1),
            ["dump"] = (Dump.Run, 1),
            ["objects"] = (Objects.Run, 1),
            ["dupes"] = (Objects.Dupes, 1),
            ["types"] = (Types.Run, 1),
            ["native"] = (Native.Run, 1),
            ["managed"] = (Managed.Run, 1),
            ["leaks"] = (Managed.Leaks, 1),
            ["why"] = (Why.Run, 2),
        };

        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            var words = argv.ToList();
            for (var i = 0; i < words.Count - 1; i++)
            {
                if (words[i] != "--project")
                    continue;
                SnapStore.ProjectOverride = words[i + 1];
                words.RemoveRange(i, 2);
                break;
            }
            if (words.Count == 0)
            {
                Help.Index(output);
                return 0;
            }
            var verb = words[0];
            var rest = words.Skip(1).ToList();
            if (verb is "--version" or "version")
            {
                output.WriteLine("memorywaitress " + typeof(CommandRouter).Assembly.GetName().Version);
                return 0;
            }
            if (verb != "help" && (rest.Contains("--help") || rest.Contains("-h")))
            {
                Help.Topic(output, verb);
                return 0;
            }
            if (s_SnapCommands.TryGetValue(verb, out var command))
            {
                var args = Args.Parse(rest, s_Switches);
                args.RejectExtraPositionals(command.positionals);
                var model = Load(args.Positional0("snapshot (a .snap file, a folder, 'latest', or part of a name in MemoryCaptures/)"), error);
                return command.run(model, args, output);
            }
            switch (verb)
            {
                case "help":
                case "--help":
                case "-h":
                    Help.Topic(output, rest.FirstOrDefault());
                    return 0;
                case "skill":
                {
                    var args = Args.Parse(rest);
                    var install = args.Get("install");
                    args.RejectUnknown();
                    return Help.Skill(install, output);
                }
                case "diff":
                {
                    var args = Args.Parse(rest, s_Switches);
                    args.RejectExtraPositionals(2);
                    var a = Load(args.PositionalAt(0, "first snapshot"), error);
                    var b = Load(args.PositionalAt(1, "second snapshot"), error);
                    return Diff.Run(a, b, args, output);
                }
                case "capture":
                    return Capture.Run(Args.Parse(rest, s_Switches), output, error);
                case "list":
                    return Capture.List(Args.Parse(rest, s_Switches), output);
                case "selftest":
                    return Testing.SelfTest.Run(output);
                default:
                    error.WriteLine($"unknown command '{verb}'");
                    Help.Index(error);
                    return 2;
            }
        }

        public static Model Load(string what, TextWriter log)
        {
            var path = SnapStore.Resolve(what);
            return new Model(Snapshot.Load(path));
        }
    }
}
