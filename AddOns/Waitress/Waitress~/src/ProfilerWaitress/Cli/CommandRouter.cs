using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProfilerWaitress.Commands;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Cli
{
    public static class CommandRouter
    {
        static readonly string[] s_Switches = { "each", "self", "idle", "meta", "no-collapse", "editor", "deep", "play", "no-extract", "internal", "asc", "version", "flows" };

        static readonly HashSet<string> s_TwoPositionals = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "counters", "marker", "find", "tree", "timeline", "bubbles" };

        delegate int CaptureCommand(CaptureFile capture, Args args, TextWriter w);

        static readonly Dictionary<string, CaptureCommand> s_CaptureCommands = new Dictionary<string, CaptureCommand>(StringComparer.OrdinalIgnoreCase)
        {
            ["info"] = Overview.Info,
            ["frames"] = Overview.Frames,
            ["threads"] = Overview.Threads,
            ["counters"] = Overview.Counters,
            ["top"] = Markers.Top,
            ["marker"] = Markers.Marker,
            ["find"] = Markers.Find,
            ["tree"] = Trees.Tree,
            ["timeline"] = Trees.Timeline,
            ["hitches"] = Hitches.Run,
            ["jobs"] = Jobs.List,
            ["waits"] = Jobs.Waits,
            ["bubbles"] = Bubbles.Run,
            ["gc"] = Gc.Run,
            ["verify"] = Verify.Run,
        };

        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            var words = argv.ToList();
            for (var i = 0; i < words.Count - 1; i++)
            {
                if (words[i] != "--project")
                    continue;
                CaptureStore.ProjectOverride = words[i + 1];
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
            CaptureStore.Log = error;
            if (verb is "--version" or "version")
            {
                output.WriteLine("profilerwaitress " + typeof(CommandRouter).Assembly.GetName().Version + ", cache format " + CaptureFile.FormatVersion);
                return 0;
            }
            if (verb != "help" && (rest.Contains("--help") || rest.Contains("-h")))
            {
                Help.Topic(output, verb);
                return 0;
            }

            if (s_CaptureCommands.TryGetValue(verb, out var command))
            {
                var args = Args.Parse(rest, s_Switches);
                args.RejectExtraPositionals(s_TwoPositionals.Contains(verb) ? 2 : 1);
                var refresh = args.Flag("force");
                using var capture = CaptureStore.Open(args.Positional0("capture (a .data or .raw file, a folder, or part of a name in ProfilerCaptures/)"), refresh);
                return command(capture, args, output);
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
                    var force = args.Flag("force");
                    using var a = CaptureStore.Open(args.PositionalAt(0, "first capture"), force);
                    using var b = CaptureStore.Open(args.PositionalAt(1, "second capture"), force);
                    return Markers.Diff(a, b, args, output);
                }
                case "extract":
                    return EditorCommands.Extract(Args.Parse(rest, s_Switches), output);
                case "convert":
                    return EditorCommands.Convert(Args.Parse(rest, s_Switches), output);
                case "record":
                    return EditorCommands.Record(Args.Parse(rest, s_Switches), output, error);
                case "cache":
                    return EditorCommands.Cache(Args.Parse(rest, s_Switches), output);
                case "selftest":
                    return Testing.SelfTest.Run(output);
                default:
                    error.WriteLine($"unknown command '{verb}'");
                    Help.Index(error);
                    return 2;
            }
        }
    }
}
