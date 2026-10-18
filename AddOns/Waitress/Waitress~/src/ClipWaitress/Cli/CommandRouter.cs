using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClipWaitress.Commands;

namespace ClipWaitress.Cli
{
    public static class CommandRouter
    {
        static readonly string[] s_Switches = { "all", "keys", "tangents", "force", "list" };

        delegate int Command(Session session, Args args, TextWriter w);

        static readonly Dictionary<string, Command> s_Commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase)
        {
            ["info"] = Read.Info,
            ["curves"] = Read.Curves,
            ["sample"] = Read.Sample,
            ["find"] = Query.Find,
            ["diff"] = Query.Diff,
            ["users"] = Query.Users,
            ["check"] = Edit.Check,
            ["set"] = Edit.Set,
            ["copy"] = Edit.Copy,
            ["refresh"] = Edit.Refresh,
        };

        public static IEnumerable<string> Commands => s_Commands.Keys;

        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            var words = argv.ToList();
            for (var i = 0; i < words.Count - 1; i++)
            {
                if (words[i] != "--project")
                    continue;
                Session.ProjectOverride = words[i + 1];
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
                output.WriteLine("clipwaitress " + typeof(CommandRouter).Assembly.GetName().Version);
                return 0;
            }
            if (verb != "help" && (rest.Contains("--help") || rest.Contains("-h")))
            {
                Help.Topic(output, verb);
                return 0;
            }
            UnityProject.HintFromArguments(rest);
            if (s_Commands.TryGetValue(verb, out var command))
            {
                var session = new Session(rest.FirstOrDefault(r => !r.StartsWith("-", StringComparison.Ordinal)), output);
                return command(session, Args.Parse(rest, s_Switches), output);
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
                case "evalcheck":
                    return Testing.EvalCheck.Run(new Session(null, output), rest.FirstOrDefault() ?? throw new WaitressException("evalcheck <oracle.tsv>"), output);
                default:
                    error.WriteLine($"unknown command '{verb}'");
                    Help.Index(error);
                    return 2;
            }
        }
    }
}
