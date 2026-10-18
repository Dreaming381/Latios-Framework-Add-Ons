using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurstWaitress.Model;

namespace BurstWaitress.Cli
{
    public static class CommandRouter
    {
        static readonly string[] s_Switches = { "raw", "source", "json", "all", "quick" };

        delegate int Command(Args args, TextWriter w);

        static readonly Dictionary<string, Command> s_Commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase)
        {
            ["status"] = BurstWaitress.Commands.Commands.Status,
            ["dump"] = BurstWaitress.Commands.Commands.DumpCmd,
            ["diff"] = BurstWaitress.Commands.Commands.Diff,
            ["sweep"] = BurstWaitress.Commands.Commands.Sweep,
            ["list"] = BurstWaitress.Commands.Commands.List,
            ["targets"] = BurstWaitress.Commands.Commands.Targets,
            ["verify"] = BurstWaitress.Commands.Verify.Run,
        };

        public static IEnumerable<string> Commands => s_Commands.Keys;

        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            var words = argv.ToList();
            for (var i = 0; i < words.Count - 1; i++)
            {
                if (words[i] != "--project")
                    continue;
                Project.Override = words[i + 1];
                words.RemoveRange(i, 2);
                i--;
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
                output.WriteLine("burstwaitress " + typeof(CommandRouter).Assembly.GetName().Version);
                return 0;
            }
            if (verb != "help" && (rest.Contains("--help") || rest.Contains("-h")))
            {
                Help.Topic(output, verb);
                return 0;
            }
            if (s_Commands.TryGetValue(verb, out var command))
            {
                var result = command(Args.Parse(rest, s_Switches), output);
                DumpStore.Prune();
                return result;
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
                case "selftest":
                    return Testing.SelfTest.Run(output);
                default:
                    var near = s_Commands.Keys.Where(k => k.StartsWith(verb.Substring(0, Math.Min(2, verb.Length)), StringComparison.OrdinalIgnoreCase)).ToList();
                    error.WriteLine($"unknown command '{verb}'" + (near.Count > 0 ? $"; did you mean {string.Join(" or ", near)}?" : ""));
                    Help.Index(error);
                    return 2;
            }
        }
    }
}
