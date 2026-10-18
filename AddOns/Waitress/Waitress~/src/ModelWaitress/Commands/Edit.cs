using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ModelWaitress.Model;

namespace ModelWaitress.Commands
{
    /// <summary>Commands that go through the Editor: changing import settings, re-exporting, and listing postprocessors.</summary>
    public static class Edit
    {
        public static int Set(Session session, Args args, TextWriter w)
        {
            var dryRun = args.Flag("dry-run");
            args.RejectUnknown();
            var targets = new List<string>();
            var ops = new List<(string key, string op, string value)>();
            foreach (var p in args.Positional)
            {
                if (SplitBracketKey(p) is var (bk, bop, bvalue) && bk != null)
                {
                    ops.Add((bk, bop, bvalue));
                    continue;
                }
                var eq = p.IndexOf('=');
                if (eq <= 0)
                {
                    targets.Add(p);
                    continue;
                }
                var op = p[eq - 1] is '+' or '-' ? p[eq - 1] + "=" : "=";
                var key = p.Substring(0, eq - (op.Length - 1)).Trim();
                if (key.Length == 0)
                    throw new WaitressException($"'{p}': expected key=value");
                ops.Add((key, op, p.Substring(eq + 1)));
            }
            if (targets.Count == 0)
                throw new WaitressException("missing a model (or folder) to change");
            if (ops.Count == 0)
                throw new WaitressException("missing key=value; see `help set`");
            var catalog = session.Catalog;
            var files = catalog.Expand(targets);
            var cache = Path.Combine(catalog.Root, "Library", "PackageCache") + Path.DirectorySeparatorChar;
            var immutable = files.Where(f => Path.GetFullPath(f.FullPath).StartsWith(cache, StringComparison.OrdinalIgnoreCase)).ToList();
            if (immutable.Count > 0)
                throw new WaitressException($"{immutable[0].AssetPath} is in a registry package, which Unity keeps read-only; copy the model into Assets/ to change its settings");
            var before = catalog.Get(files, session.Notes).ToDictionary(r => r.Path);
            var request = JsonSerializer.Serialize(new
            {
                @out = catalog.Folder.Replace('\\', '/'),
                dryRun,
                ops = files.SelectMany(f => ops.Select(o => new { path = f.AssetPath, o.key, o.op, o.value })).ToArray(),
            });
            var result = EditorScript.Call(session.Root, "Edit", request);
            if (dryRun)
            {
                w.WriteLine($"valid: {Fmt.Plural(ops.Count, "change")} to {Fmt.Plural(files.Count, "model")}; nothing was changed (--dry-run)");
                return 0;
            }
            var done = result.GetProperty("done").EnumerateArray().Select(e => e.GetString()).ToHashSet();
            var after = catalog.Get(files.Where(f => done.Contains(f.AssetPath)).ToList(), session.Notes).ToDictionary(r => r.Path);
            // A clip renamed in this call is found under its new name afterwards.
            var renames = ops.Where(o => o.op == "=" && o.key.StartsWith("clips[", StringComparison.Ordinal) && o.key.EndsWith("].name", StringComparison.Ordinal))
                             .ToDictionary(o => o.key.Substring(6, o.key.Length - 12), o => o.value);
            var broken = 0;
            foreach (var f in files)
            {
                if (!after.TryGetValue(f.AssetPath, out var now))
                    continue;
                before.TryGetValue(f.AssetPath, out var was);
                w.WriteLine(f.AssetPath + " (reimported)");
                foreach (var o in ops)
                {
                    var old = was != null ? Describe(was, o.key) : "?";
                    var newKey = o.key;
                    foreach (var r in renames)
                        newKey = newKey.Replace("clips[" + r.Key + "]", "clips[" + r.Value + "]");
                    var neu = Describe(now, newKey);
                    if (o.key == "human.skeleton")
                        w.WriteLine($"  human.skeleton: rebuilt from the file ({neu})");
                    else
                        w.WriteLine($"  {o.key}: {old} -> {neu}" + (old == neu ? "  (unchanged)" : ""));
                }
                if (was != null)
                    foreach (var line in Delta(was, now))
                        w.WriteLine("  " + line);
                var heads = new HashSet<string>((was?.Messages ?? new List<(string, string)>()).Select(x => FirstLine(x.Item2)));
                if (now.Messages.Any(x => IsError(x) && !heads.Contains(FirstLine(x.text))))
                    broken++;
            }
            if (broken > 0)
                w.WriteLine($"the import now reports an error on {Fmt.Plural(broken, "model")}; the settings were saved anyway (exit code 1)");
            return broken > 0 ? 1 : 0;
        }

        // The import log flags rig failures as warnings, so their text decides too.
        static string FirstLine(string text) => text.Split('\n')[0].Trim();

        static bool IsError((string level, string text) m) => m.level == "error" || m.text.StartsWith("Rig Error", StringComparison.Ordinal);

        static readonly System.Text.RegularExpressions.Regex s_AfterBracket = new System.Text.RegularExpressions.Regex(@"^(\.[A-Za-z]+)?(\+=|-=|=)");

        // clips[Name].x=v, materials[Name]=v, human[Name]=v. A name may hold ] or =: a clip key ends at the first ] followed by
        // .property and an operator; the others at the last ] followed by an operator.
        static (string key, string op, string value) SplitBracketKey(string p)
        {
            var open = p.IndexOf('[');
            var clip = p.StartsWith("clips[", StringComparison.Ordinal);
            if (open <= 0 || !(clip || p.StartsWith("materials[", StringComparison.Ordinal) || p.StartsWith("human[", StringComparison.Ordinal)))
                return (null, null, null);
            var positions = Enumerable.Range(open + 1, p.Length - open - 1).Where(j => p[j] == ']');
            foreach (var j in clip ? positions : positions.Reverse())
            {
                var m = s_AfterBracket.Match(p.Substring(j + 1));
                if (!m.Success || m.Groups[1].Length > 0 != clip)
                    continue;
                var keyEnd = j + 1 + m.Groups[1].Length;
                return (p.Substring(0, keyEnd), m.Groups[2].Value, p.Substring(keyEnd + m.Groups[2].Length));
            }
            return (null, null, null);
        }

        /// <summary>The current value a set key refers to, as text.</summary>
        static string Describe(ModelRecord m, string key)
        {
            if (key.StartsWith("clips[", StringComparison.Ordinal))
            {
                var close = key.LastIndexOf("].", StringComparison.Ordinal);
                var name = key.Substring(6, close - 6);
                var prop = key.Substring(close + 2);
                var clip = m.Clips.FirstOrDefault(c => c.Name == name);
                if (clip == null)
                    return "(no clip " + name + ")";
                if (prop == "events")
                    return clip.Events.Count == 0 ? "none" : string.Join(", ", clip.Events.Select(e => $"{Fmt.Num(e.time)}:{e.function}{(string.IsNullOrEmpty(e.text) ? "" : ":" + e.text)}"));
                if (prop == "mask")
                    return clip.Mask ?? "None";
                if (prop == "name")
                    return clip.Name;
                if (prop == "takeName")
                    return clip.Take;
                return Read.Show(clip.Get(prop));
            }
            if (key == "clips")
                return string.Join(", ", m.Clips.Select(c => c.Name)) + (m.ClipsCustom ? "" : " (defaults)");
            if (key.StartsWith("human[", StringComparison.Ordinal))
            {
                var name = key.Substring(6, key.LastIndexOf(']') - 6).Replace(" ", "");
                var hit = m.HumanMap.FirstOrDefault(x => x.human.Replace(" ", "").Equals(name, StringComparison.OrdinalIgnoreCase));
                return hit.bone ?? "(unmapped)";
            }
            if (key == "human.skeleton")
                return $"{m.HumanSkeleton}-bone pose" + (m.Messages.Any(x => x.text.Contains("Avatar Rig Configuration mis-match", StringComparison.Ordinal)) ? " (mismatched)" : "");
            if (key.StartsWith("human.", StringComparison.Ordinal))
                return m.HumanParams.TryGetValue(key.Substring(6), out var v) ? v : "?";
            if (key.StartsWith("materials[", StringComparison.Ordinal))
            {
                var name = key.Substring(10, key.LastIndexOf(']') - 10);
                var remap = m.Remaps.FirstOrDefault(r => r.type == "Material" && r.name == name);
                return remap.name != null ? remap.target ?? "None" : "(not remapped)";
            }
            return Read.Show(m.Get(key));
        }

        static IEnumerable<string> Delta(ModelRecord a, ModelRecord b)
        {
            var rows = new[]
            {
                ("scale", Query.Value(a, "scale"), Query.Value(b, "scale")),
                ("root rotation/scale", string.Join("; ", Read.TiltedNodes(a)), string.Join("; ", Read.TiltedNodes(b))),
                ("transforms", Query.Value(a, "transforms"), Query.Value(b, "transforms")),
                ("meshes", $"{a.Meshes.Count} ({Query.Value(a, "verts")} verts)", $"{b.Meshes.Count} ({Query.Value(b, "verts")} verts)"),
                ("blend shapes", Query.Value(a, "shapes"), Query.Value(b, "shapes")),
                ("clips", string.Join(", ", a.Anims.Select(x => x.Name + " " + Fmt.Seconds(x.Length) + (x.Loop ? " loop" : ""))), string.Join(", ", b.Anims.Select(x => x.Name + " " + Fmt.Seconds(x.Length) + (x.Loop ? " loop" : "")))),
                ("avatar", string.Join(", ", a.Avatars.Select(x => $"{x.name} {(a.AvatarOk(x) ? "valid" : x.valid ? "not human" : "invalid")}{(x.human ? " human" : "")}")), string.Join(", ", b.Avatars.Select(x => $"{x.name} {(b.AvatarOk(x) ? "valid" : x.valid ? "not human" : "invalid")}{(x.human ? " human" : "")}"))),
                ("material slots", string.Join(", ", Read.Slots(a).Select(s => Read.MaterialName(a, s.material))), string.Join(", ", Read.Slots(b).Select(s => Read.MaterialName(b, s.material)))),
            };
            foreach (var (what, x, y) in rows)
                if (x != y)
                    yield return $"result {what}: {(string.IsNullOrEmpty(x) ? "none" : x)} -> {(string.IsNullOrEmpty(y) ? "none" : y)}";
            // Messages are matched by their first line, so one whose numbers changed reads as changed, not new.
            static string Head(string text) => text.Split('\n')[0].Trim();
            var oldMessages = a.Messages.Select(m => m.level + ": " + m.text).ToList();
            var newMessages = b.Messages.Select(m => m.level + ": " + m.text).ToList();
            var oldHeads = new HashSet<string>(oldMessages.Select(Head));
            var newHeads = new HashSet<string>(newMessages.Select(Head));
            foreach (var m in newMessages.Where(m => !oldMessages.Contains(m)).Distinct())
                yield return (oldHeads.Contains(Head(m)) ? "still reported, details changed: " + Head(m) : "new " + string.Join("\n    ", m.Split('\n').Select(l => l.Trim())));
            var gone = oldMessages.Where(m => !newHeads.Contains(Head(m))).Distinct().ToList();
            foreach (var m in gone)
                yield return "no longer reported: " + Head(m);
        }

        public static int Refresh(Session session, Args args, TextWriter w)
        {
            var force = args.Flag("force");
            args.RejectUnknown();
            var catalog = session.Catalog;
            var files = args.Positional.Count == 0 ? catalog.All.ToList() : catalog.Expand(args.Positional);
            var pruned = catalog.Prune();
            if (pruned > 0)
                w.WriteLine($"removed {Fmt.Plural(pruned, "record")} of models no longer in the project");
            var current = files.Where(f => !force && catalog.IsCurrent(f)).ToList();
            var todo = files.Except(current).ToList();
            if (todo.Count == 0)
            {
                w.WriteLine($"all {Fmt.Plural(files.Count, "model")} already exported and current (--force re-exports)");
                return 0;
            }
            var failed = catalog.Export(todo);
            w.WriteLine($"exported {todo.Count - failed.Count} of {Fmt.Plural(todo.Count, "model")} ({current.Count} already current)");
            foreach (var f in failed)
                w.WriteLine($"  {f.Key}: {f.Value}");
            return 0;
        }

        public static int Postprocessors(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(0);
            var force = args.Flag("force");
            args.RejectUnknown();
            var catalog = session.Catalog;
            var file = Path.Combine(catalog.Folder, "project.json");
            if (force || !File.Exists(file))
            {
                var sample = catalog.All.FirstOrDefault() ?? throw new WaitressException("the project has no models");
                catalog.Export(new[] { sample });
            }
            var p = catalog.Project;
            if (p.Postprocessors.Count == 0)
                w.WriteLine("no AssetPostprocessor in the project touches model imports");
            else
            {
                w.WriteLine("AssetPostprocessors with model callbacks (they run on every model import, lowest order first):");
                foreach (var x in p.Postprocessors.OrderBy(x => x.order ?? 0).ThenBy(x => x.type, StringComparer.Ordinal))
                    w.WriteLine($"  {x.type}  order {x.order?.ToString() ?? "?"}, version {x.version?.ToString() ?? "?"}  {string.Join(", ", x.callbacks)}  [{x.script ?? x.assembly}]");
            }
            var presets = p.Presets.Where(x => x.preset != null).ToList();
            if (presets.Count == 0)
                w.WriteLine("no default presets for ModelImporter (Preset Manager); new models start from Unity's defaults");
            else
            {
                w.WriteLine("default presets for ModelImporter (applied when a model is first imported):");
                foreach (var x in presets)
                    w.WriteLine($"  {x.preset}  filter '{x.filter}'{(x.enabled ? "" : " (disabled)")}");
            }
            w.WriteLine($"(as of the last export{(force ? "" : "; --force re-reads")})");
            return 0;
        }
    }
}
