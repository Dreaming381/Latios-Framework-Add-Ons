using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Commands;

namespace PrefabWaitress.Cli
{
    public static class CommandRouter
    {
        static readonly string[] s_Switches = { "all", "unused", "redundant", "components", "deep", "dry-run", "save-open", "verbose", "instances" };

        delegate int Command(Session session, Args args, TextWriter w);

        static readonly Dictionary<string, Command> s_Commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase)
        {
            ["tree"] = Tree.Run,
            ["show"] = Show.Run,
            ["why"] = Why.Run,
            ["overrides"] = Overrides.Run,
            ["instances"] = Instances.Run,
            ["set"] = Edit.Set,
            ["revert"] = Edit.Revert,
            ["apply"] = Edit.Apply,
            ["clean"] = Edit.Clean,
            ["models"] = Models.Run,
            ["dump"] = Dump.Run,
            ["stats"] = Stats.Run,
            ["oraclecheck"] = Testing.OracleCheck.Run,
        };

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
                output.WriteLine("prefabwaitress " + typeof(CommandRouter).Assembly.GetName().Version);
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
                var session = new Session(rest.FirstOrDefault(r => !r.StartsWith("-", StringComparison.Ordinal)));
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
                default:
                    error.WriteLine($"unknown command '{verb}'");
                    Help.Index(error);
                    return 2;
            }
        }
    }
}
