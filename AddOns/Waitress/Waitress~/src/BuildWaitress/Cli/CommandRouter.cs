using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildWaitress.Commands;

namespace BuildWaitress.Cli
{
    public static class CommandRouter
    {
        static readonly string[] s_Switches = { "all", "trees", "archetypes", "dev", "detailed", "no-wait", "force", "clean" };

        delegate int Command(Session session, Args args, TextWriter w);

        static readonly Dictionary<string, Command> s_Commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase)
        {
            ["summary"] = Read.Summary,
            ["files"] = Read.Files,
            ["assets"] = Read.Assets,
            ["why"] = Read.Why,
            ["subscenes"] = Read.Subscenes,
            ["steps"] = Read.Steps,
            ["check"] = Read.Check,
            ["history"] = Read.History,
            ["diff"] = Compare.Diff,
            ["build"] = Act.Build,
            ["types"] = Act.Types,
        };

        public static IEnumerable<string> Commands => s_Commands.Keys;

        public static int Run(IReadOnlyList<string> argv, TextWriter output, TextWriter error)
        {
            var words = argv.ToList();
            for (var i = 0; i < words.Count - 1; i++)
            {
                if (words[i] == "--project")
                    Session.ProjectOverride = words[i + 1];
                else if (words[i] == "--folder")
                    Session.FolderOverride = words[i + 1];
                else
                    continue;
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
                output.WriteLine("buildwaitress " + typeof(CommandRouter).Assembly.GetName().Version);
                return 0;
            }
            if (verb != "help" && (rest.Contains("--help") || rest.Contains("-h")))
            {
                Help.Topic(output, verb);
                return 0;
            }
            if (s_Commands.TryGetValue(verb, out var command))
                return command(new Session(null, output), Args.Parse(rest, s_Switches), output);
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
                case "dump":
                    return Testing.Dump.Run(Args.Parse(rest, s_Switches), output);
                case "layouts":
                    return Testing.Layouts.Run(new Session(null, output), Args.Parse(rest, s_Switches), output);
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
