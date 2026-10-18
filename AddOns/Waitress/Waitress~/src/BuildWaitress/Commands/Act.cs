using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using BuildWaitress.Model;

namespace BuildWaitress.Commands
{
    public static class Act
    {
        static readonly Dictionary<string, string> s_Targets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["win64"] = "StandaloneWindows64", ["windows"] = "StandaloneWindows64", ["win"] = "StandaloneWindows64",
            ["linux"] = "StandaloneLinux64", ["linux64"] = "StandaloneLinux64", ["mac"] = "StandaloneOSX", ["osx"] = "StandaloneOSX",
            ["android"] = "Android", ["ios"] = "iOS", ["webgl"] = "WebGL",
        };

        public static int Build(Session session, Args a, TextWriter w)
        {
            var root = session.RequireRoot;
            var target = a.Get("target");
            if (target != null && s_Targets.TryGetValue(target, out var full))
                target = full;
            var outPath = a.Get("out");
            var options = new List<string>(a.GetAll("option"));
            if (a.Flag("dev"))
                options.Add("Development");
            if (a.Flag("detailed"))
                options.Add("DetailedBuildReport");
            var clean = a.Flag("clean");
            var noWait = a.Flag("no-wait");
            var timeout = Read.Top(a, "timeout", 120);
            var scenes = a.Positional.Select(s => SceneAsset(root, s)).ToList();
            a.RejectUnknown();
            var effective = target ?? ActiveTarget(root) ?? "StandaloneWindows64";
            outPath ??= DefaultOut(root, effective);
            var outFull = Path.GetFullPath(Path.IsPathRooted(outPath) ? outPath : Path.Combine(root, outPath));
            var outDir = effective.StartsWith("Standalone", StringComparison.Ordinal) || effective == "Android" ? Path.GetDirectoryName(outFull) : outFull;
            if (clean && Directory.Exists(outDir))
            {
                if (!Directory.EnumerateDirectories(outDir, "*_Data").Any() && !File.Exists(Path.Combine(outDir, "UnityPlayer.dll")) && Directory.EnumerateFileSystemEntries(outDir).Any())
                    throw new WaitressException($"--clean: {outDir} doesn't look like a Unity player build folder; not deleting it");
                Directory.Delete(outDir, true);
                w.WriteLine($"deleted {outDir}");
            }
            var status = Path.Combine(session.Dir, "build-status.json");
            var args = new Dictionary<string, object> { ["out"] = outFull.Replace('\\', '/'), ["status"] = status.Replace('\\', '/'), ["options"] = options, ["scenes"] = scenes };
            if (target != null)
                args["target"] = target;
            var started = DateTime.Now;
            var reply = EditorScript.Call(root, "Build", JsonSerializer.Serialize(args));
            w.WriteLine($"building {reply.GetProperty("target").GetString()} player ({Fmt.Plural(reply.GetProperty("scenes").GetInt32(), "scene")}) to {outFull.Replace('\\', '/')}");
            if (noWait)
            {
                w.WriteLine($"not waiting; the result goes to {status} and `buildwaitress summary` reads the report when it is done");
                return 0;
            }
            JsonElement done = default;
            var deadline = DateTime.Now.AddMinutes(timeout);
            while (true)
            {
                Thread.Sleep(2000);
                try
                {
                    var text = File.ReadAllText(status);
                    var doc = JsonDocument.Parse(text).RootElement;
                    if (doc.GetProperty("state").GetString() == "done")
                    {
                        done = doc.Clone();
                        break;
                    }
                }
                catch (Exception e) when (e is IOException || e is JsonException)
                {
                }
                if (DateTime.Now > deadline)
                    throw new WaitressException($"the build is still running after {timeout} minutes; it continues in the Editor and `buildwaitress summary` reads it when done");
                if (!Process.GetProcessesByName("Unity").Any())
                    throw new WaitressException("the Editor exited during the build");
            }
            var result = done.GetProperty("result").GetString();
            var errors = done.GetProperty("errors").EnumerateArray().Select(e => e.GetString()).ToList();
            var changed = done.GetProperty("changed").EnumerateArray().Select(e => e.GetString()).Where(p => !p.EndsWith(".meta (deleted)", StringComparison.Ordinal)).ToList();
            w.WriteLine($"{result} after {Fmt.Time(DateTime.Now - started)}");
            if (errors.Count > 0)
            {
                w.WriteLine("errors logged during the build:");
                foreach (var g in errors.GroupBy(Similar).Take(8))
                    w.WriteLine($"  {(g.Count() > 1 ? $"(×{g.Count()}) " : "")}{Analysis.Shorten(g.First(), 500)}");
            }
            if (changed.Count > 0)
            {
                var cache = changed.Count(c => c.Contains("/SceneDependencyCache/"));
                var rest = changed.Where(c => !c.Contains("/SceneDependencyCache/")).ToList();
                w.WriteLine("the build changed project files: " + string.Join(", ", rest.Take(12)) + (rest.Count > 12 ? $" and {rest.Count - 12} more" : "") +
                            (cache > 0 ? $"{(rest.Count > 0 ? "; " : "")}{cache} Entities SceneDependencyCache files" : ""));
                if (rest.Count > 0)
                    w.WriteLine("  (Unity, render pipelines and packages rewrite settings during builds; under version control, `git checkout -- <file>` undoes one you don't want)");
            }
            session.ArchiveLatest();
            var history = session.History();
            if (history.Count > 0 && Report.Load(history[0].Path).Started >= started.AddMinutes(-1))
            {
                w.WriteLine("this build is now `last`; the one before it is `~1`");
                w.WriteLine();
                return Read.Summary(session, Args.Parse(new List<string>()), w) == 0 && result == "Succeeded" ? 0 : 1;
            }
            w.WriteLine("Unity wrote no new build report (the build stopped before it started)");
            return 1;
        }

        static string Similar(string message)
        {
            var tick = message.IndexOf('`');
            return tick > 0 && message.Contains("BC") ? message.Substring(0, tick) : message;
        }

        static string SceneAsset(string root, string s)
        {
            if (s.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) && File.Exists(Path.Combine(root, s)))
                return s.Replace('\\', '/');
            var name = Path.GetFileNameWithoutExtension(s);
            var hits = Directory.EnumerateFiles(Path.Combine(root, "Assets"), name + ".unity", SearchOption.AllDirectories)
                .Select(p => Path.GetRelativePath(root, p).Replace('\\', '/')).ToList();
            if (hits.Count == 1)
                return hits[0];
            if (hits.Count > 1)
                throw new WaitressException($"'{s}' names {hits.Count} scenes: {string.Join(", ", hits.Take(5))}");
            throw new WaitressException($"no scene '{s}' under Assets/");
        }

        static string ActiveTarget(string root)
        {
            var path = Path.Combine(root, "Library", "EditorUserBuildSettings.asset");
            return null;
        }

        static string DefaultOut(string root, string target)
        {
            var product = "Player";
            var settings = Path.Combine(root, "ProjectSettings", "ProjectSettings.asset");
            if (File.Exists(settings))
                foreach (var line in File.ReadLines(settings))
                    if (line.StartsWith("  productName: ", StringComparison.Ordinal))
                    {
                        product = line.Substring(15).Trim();
                        break;
                    }
            foreach (var c in Path.GetInvalidFileNameChars())
                product = product.Replace(c, '_');
            var dir = $"Builds/{target}";
            return target switch
            {
                "StandaloneWindows64" or "StandaloneWindows" => $"{dir}/{product}.exe",
                "StandaloneLinux64" => $"{dir}/{product}.x86_64",
                "StandaloneOSX" => $"{dir}/{product}.app",
                "Android" => $"{dir}/{product}.apk",
                _ => dir,
            };
        }

        public static int Types(Session session, Args a, TextWriter w)
        {
            a.RejectUnknown();
            var root = session.RequireRoot;
            var count = ExportTypes(root);
            w.WriteLine($"exported {Fmt.Plural(count, "Entities type")} to UserSettings/Waitress/BuildWaitress/types.tsv");
            return 0;
        }

        public static int ExportTypes(string root)
        {
            var file = Path.Combine(root, "UserSettings", "Waitress", "BuildWaitress", "types.tsv").Replace('\\', '/');
            var reply = EditorScript.Call(root, "Types", JsonSerializer.Serialize(new Dictionary<string, object> { ["out"] = file }));
            return reply.GetProperty("types").GetInt32();
        }
    }
}
