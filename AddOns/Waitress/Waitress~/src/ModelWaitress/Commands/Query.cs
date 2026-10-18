using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ModelWaitress.Model;

namespace ModelWaitress.Commands
{
    /// <summary>Commands across several models: find by settings and results, and diff two models.</summary>
    public static class Query
    {
        static readonly string[] s_Ops = { ">=", "<=", "!=", "=", ">", "<", "~" };

        static readonly (string key, string meaning)[] s_Stats =
        {
            ("verts", "vertices over all meshes"), ("maxverts", "vertices of the largest mesh"), ("tris", "triangles over all meshes"), ("meshes", "mesh count"),
            ("skinned", "skinned mesh count"), ("bones", "most bindposes on one mesh"), ("shapes", "blend shapes"),
            ("clips", "clips produced"), ("takes", "takes in the file"), ("transforms", "GameObjects in the result"),
            ("slots", "renderer material slots"), ("empty", "empty material slots"), ("warnings", "import warnings"),
            ("errors", "import errors"), ("avatar", "none, valid or invalid"), ("unit", "the file's unit (cm, m, ...)"),
            ("scale", "effective import scale"), ("changed", "settings changed from default (count, or ~name)"),
            ("importer", "importer class"), ("path", "asset path"), ("app", "application that wrote the .fbx"), ("up", "the .fbx file's up axis (Y, Z, -Z...)"),
        };

        public static string StatHelp => string.Join(", ", s_Stats.Select(s => s.key));

        public static int Find(Session session, Args args, TextWriter w)
        {
            var all = args.Flag("all");
            var list = args.Flag("list") ? int.MaxValue : 25;
            args.RejectUnknown();
            var conditions = new List<(string key, string op, string value)>();
            var targets = new List<string>();
            foreach (var p in args.Positional)
            {
                var c = ParseCondition(p);
                if (c.key != null)
                    conditions.Add(c);
                else
                    targets.Add(p);
            }
            var catalog = session.Catalog;
            var files = targets.Count == 0 ? catalog.All.Where(f => all || f.AssetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)).ToList() : catalog.Expand(targets);
            if (files.Count == 0)
                throw new WaitressException("no models there");
            var records = catalog.Get(files, session.Notes);
            if (records.Count == 0)
                return 1;
            var clipSettings = new HashSet<string>(records.SelectMany(r => r.Clips.SelectMany(c => c.Settings.Select(s => s.Name))), StringComparer.OrdinalIgnoreCase);
            foreach (var c in conditions)
            {
                if (IsClipKey(c.key))
                {
                    if (!clipSettings.Contains(c.key.Substring(5)))
                        throw new WaitressException($"unknown clip setting '{c.key.Substring(5)}'; {Read.Suggest(c.key.Substring(5), clipSettings)}");
                    continue;
                }
                if (Value(records[0], c.key) == null && !records[0].Settings.Any(s => s.Name.Equals(c.key, StringComparison.OrdinalIgnoreCase)))
                    throw new WaitressException($"unknown key '{c.key}'; keys are importer settings (`settings <model> --all`), clip.<setting>, or {StatHelp}. {Read.Suggest(c.key, records[0].Settings.Select(s => s.Name).Concat(s_Stats.Select(s => s.key)))}");
            }

            // A clip.<setting> condition holds when any clip of the model satisfies it.
            bool Holds(ModelRecord m, (string key, string op, string value) c) =>
                IsClipKey(c.key) ? m.Clips.Any(clip => Test(ClipValue(clip, c.key), c.op, c.value)) : Test(Value(m, c.key), c.op, c.value);
            var matches = records.Where(m => conditions.All(c => Holds(m, c))).ToList();
            // Values an equality condition already pins aren't repeated on every line.
            var shown = conditions.Where(c => c.op != "=" || c.value.Length == 0).Select(c => c.key).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (matches.Count > list && !args.Flag("list"))
            {
                foreach (var g in matches.GroupBy(m => m.Path.Substring(0, m.Path.LastIndexOf('/') + 1)).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
                    w.WriteLine(g.Count() == 1 ? Line(g.First()) : $"{g.Key}  {Fmt.Plural(g.Count(), "model")}" + Range(g.ToList()));
                w.WriteLine($"(grouped by folder; --list prints every model)");
            }
            else
                foreach (var m in matches)
                    w.WriteLine(Line(m));
            var scope = targets.Count == 0 ? (all ? "the project" : "Assets/ (--all adds packages)") : string.Join(", ", targets);
            w.WriteLine(conditions.Count == 0 ? $"{Fmt.Plural(records.Count, "model")} in {scope}" : $"{matches.Count} of {Fmt.Plural(records.Count, "model")} in {scope} match");
            return 0;

            string Line(ModelRecord m) => conditions.Count == 0 ? $"{m.Path}  {Brief(m)}"
                : shown.Count == 0 ? m.Path : $"{m.Path}  {string.Join(" ", shown.Select(k => Shown(m, k)))}";

            // changed~name shows the matching settings' values rather than the whole list of changed names.
            string Shown(ModelRecord m, string key)
            {
                var filters = conditions.Where(c => c.key.Equals("changed", StringComparison.OrdinalIgnoreCase) && c.op == "~").Select(c => c.value).ToList();
                if (!key.Equals("changed", StringComparison.OrdinalIgnoreCase) || filters.Count == 0)
                    return IsClipKey(key) ? string.Join(" ", m.Clips.Select(clip => $"{clip.Name}:{key.Substring(5)}={Read.Show(ClipValue(clip, key))}")) : key + "=" + Read.Show(Value(m, key));
                return string.Join(" ", m.Settings.Where(s => s.Changed && filters.Any(f => s.Name.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0))
                                         .Select(s => $"{s.Name}={Read.Show(s.Value)}"));
            }

            string Range(List<ModelRecord> group)
            {
                if (shown.Count == 0)
                    return "";
                return "  " + string.Join(" ", shown.Select(k =>
                {
                    var values = group.Select(m => Value(m, k)).Distinct().ToList();
                    var numbers = values.Select(v => double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : double.NaN).ToList();
                    if (values.Count == 1)
                        return k + "=" + Read.Show(values[0]);
                    if (numbers.All(d => !double.IsNaN(d)))
                        return $"{k}={Fmt.Num(numbers.Min())}..{Fmt.Num(numbers.Max())}";
                    return $"{k}={string.Join("|", values.Take(4))}{(values.Count > 4 ? "|..." : "")}";
                }));
            }
        }

        public static (string key, string op, string value) ParseCondition(string text)
        {
            // The earliest operator wins, preferring two-character ones at the same place.
            var first = s_Ops.Select(o => (o, i: text.IndexOf(o, StringComparison.Ordinal))).Where(x => x.i > 0)
                             .OrderBy(x => x.i).ThenByDescending(x => x.o.Length).FirstOrDefault();
            if (first.o == null)
                return (null, null, null);
            return (text.Substring(0, first.i).Trim(), first.o, text.Substring(first.i + first.o.Length).Trim());
        }

        static string Brief(ModelRecord m)
        {
            var parts = new List<string> { $"{Fmt.Count(m.Meshes.Sum(x => x.Verts))}v" };
            var rig = m.Get("animationType");
            if (rig != "None")
                parts.Add(rig == "Human" ? "humanoid" : rig.ToLowerInvariant());
            if (m.Anims.Count > 0)
                parts.Add(Fmt.Plural(m.Anims.Count, "clip"));
            var changed = m.Settings.Count(s => s.Changed);
            if (changed > 0)
                parts.Add($"{changed} changed");
            var issues = m.Messages.Count;
            if (issues > 0)
                parts.Add(Fmt.Plural(issues, "message"));
            if (m.Stale)
                parts.Add("stale");
            return string.Join(", ", parts);
        }

        static bool IsClipKey(string key) => key.StartsWith("clip.", StringComparison.OrdinalIgnoreCase);

        static string ClipValue(ClipSettings clip, string key) =>
            clip.Settings.FirstOrDefault(s => s.Name.Equals(key.Substring(5), StringComparison.OrdinalIgnoreCase))?.Value;

        public static string Value(ModelRecord m, string key)
        {
            var setting = m.Settings.FirstOrDefault(s => s.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (setting != null)
                return setting.Value;
            string N(double d) => Fmt.Num(d);
            switch (key.ToLowerInvariant())
            {
                case "verts": return N(m.Meshes.Sum(x => x.Verts));
                case "tris": return N(m.Meshes.Sum(x => x.Tris));
                case "maxverts": return N(m.Meshes.Count == 0 ? 0 : m.Meshes.Max(x => x.Verts));
                case "meshes": return N(m.Meshes.Count);
                case "skinned": return N(m.Meshes.Count(x => x.Bindposes > 0));
                case "bones": return N(m.Meshes.Count == 0 ? 0 : m.Meshes.Max(x => x.Bindposes));
                case "shapes": return N(m.Meshes.Sum(x => x.Shapes.Count));
                case "clips": return N(m.Anims.Count);
                case "takes": return N(m.Takes.Count);
                case "transforms": return N(m.Nodes.Count);
                case "slots": return N(Read.Slots(m).Count());
                case "empty": return N(Read.Slots(m).Count(s => s.material == null));
                case "warnings": return N(m.Messages.Count(x => x.level != "error"));
                case "errors": return N(m.Messages.Count(x => x.level == "error"));
                case "avatar": return m.Avatars.Count == 0 ? "none" : m.Avatars.All(m.AvatarOk) ? "valid" : "invalid";
                case "unit": return m.IsFbx ? m.Unit ?? "" : "";
                case "scale":
                    return N(double.Parse(m.Get("globalScale") ?? "1", CultureInfo.InvariantCulture) * m.FileScale);
                case "changed": return string.Join(",", m.Settings.Where(s => s.Changed).Select(s => s.Name));
                case "importer": return m.Importer;
                case "app": return m.Header?.Application ?? m.Header?.Creator ?? "";
                case "up": return m.Header == null ? "" : m.Header.Axes?.Split('-')[0] ?? "Y";
                case "path": return m.Path;
            }
            return null;
        }

        public static bool Test(string actual, string op, string expected)
        {
            actual ??= "";
            expected = expected.ToLowerInvariant() switch { "yes" or "on" => "true", "no" or "off" => "false", _ => expected };
            if (op == "~")
                return actual.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
            var numeric = double.TryParse(expected, NumberStyles.Float, CultureInfo.InvariantCulture, out var e);
            double a = 0;
            // A list compared with a number is compared by its length.
            if (numeric && !double.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out a))
                a = actual.Length == 0 ? 0 : actual.Split(',').Length;
            switch (op)
            {
                case "=": return numeric ? Math.Abs(a - e) < 1e-6 : string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
                case "!=": return numeric ? Math.Abs(a - e) >= 1e-6 : !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
            }
            if (!numeric)
                throw new WaitressException($"{op} compares numbers; '{expected}' is not one");
            return op switch { ">" => a > e, "<" => a < e, ">=" => a >= e, _ => a <= e };
        }

        // ------------------------------------------------------------ users

        static readonly string[] s_TextAssets = { ".unity", ".prefab", ".asset", ".controller", ".overrideController", ".anim", ".mask", ".mat", ".playable", ".signal", ".preset" };

        /// <summary>Text-serialized assets that reference the model, and which of its sub-assets each one uses.</summary>
        public static int Users(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            var all = args.Flag("all");
            args.RejectUnknown();
            var catalog = session.Catalog;
            var file = catalog.Resolve(args.Positional0("a model"));
            var m = catalog.Get(file, session.Notes);
            var names = m.SubAssets.GroupBy(s => s.id).ToDictionary(g => g.Key, g => g.First().type + " " + g.First().name);
            var folders = new List<string> { Path.Combine(catalog.Root, "Assets") };
            if (all)
                folders.Add(Path.Combine(catalog.Root, "Packages"));
            var needle = "guid: " + m.Guid;
            var hits = new System.Collections.Concurrent.ConcurrentBag<(string path, List<string> uses)>();
            var files = folders.Where(Directory.Exists)
                .SelectMany(f => Directory.EnumerateFiles(f, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true }))
                .Where(f => s_TextAssets.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)).ToList();
            // Other models point at this one from their .meta: material remaps, copied avatars, clip masks.
            files.AddRange(catalog.All.Where(f => f.Guid != m.Guid && (all || f.AssetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))).Select(f => f.FullPath + ".meta"));
            System.Threading.Tasks.Parallel.ForEach(files, f =>
            {
                string text;
                try
                {
                    text = File.ReadAllText(f);
                }
                catch (IOException)
                {
                    return;
                }
                if (!text.Contains(needle, StringComparison.Ordinal))
                    return;
                var uses = new SortedSet<string>(StringComparer.Ordinal);
                var instances = 0;
                foreach (var line in text.Split('\n'))
                {
                    var at = line.IndexOf(needle, StringComparison.Ordinal);
                    if (at < 0)
                        continue;
                    var key = line.TrimStart().TrimStart('-', ' ').Split(':')[0];
                    if (key == "m_SourcePrefab")
                    {
                        instances++;
                        continue;
                    }
                    // Override targets and source links inside a prefab instance point at the model's own objects.
                    if (key is "target" or "m_CorrespondingSourceObject" or "targetCorrespondingSourceObject" || key.StartsWith("{", StringComparison.Ordinal))
                        continue;
                    var id = Between(line, "fileID: ", ",");
                    var what = id == null ? "?" : names.TryGetValue(id, out var n) ? n : "object " + id;
                    var how = key switch { "second" => "material remap", "lastHumanDescriptionAvatarSource" or "sourceAvatar" => "copies this avatar", "maskSource" => "clip mask", _ => key };
                    uses.Add($"{what} ({how})");
                }
                var list = uses.ToList();
                if (instances > 0)
                    list.Insert(0, instances == 1 ? "a prefab instance of the model" : $"{instances} prefab instances of the model");
                hits.Add((Path.GetRelativePath(catalog.Root, f).Replace('\\', '/'), list));
            });
            foreach (var (path, uses) in hits.OrderBy(h => h.path, StringComparer.OrdinalIgnoreCase))
                w.WriteLine($"{path}  {string.Join("; ", uses.Take(8))}{(uses.Count > 8 ? $"; +{uses.Count - 8} more" : "")}");
            w.WriteLine($"{Fmt.Plural(hits.Count, "file")} in {(all ? "Assets/ and Packages/" : "Assets/ (--all adds Packages/)")} reference {m.Path}" +
                        " (text-serialized scenes, prefabs, controllers, masks, materials and other assets)");
            return 0;
        }

        static string Between(string s, string start, string end)
        {
            var a = s.IndexOf(start, StringComparison.Ordinal);
            if (a < 0)
                return null;
            a += start.Length;
            var b = s.IndexOf(end, a, StringComparison.Ordinal);
            return b < 0 ? null : s.Substring(a, b - a).Trim();
        }

        // ------------------------------------------------------------ diff

        public static int Diff(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var catalog = session.Catalog;
            var files = new[] { catalog.Resolve(args.PositionalAt(0, "two models")), catalog.Resolve(args.PositionalAt(1, "two models")) };
            var records = catalog.Get(files, session.Notes);
            if (records.Count < 2)
                return 1;
            var (a, b) = (records[0], records[1]);
            w.WriteLine($"A {a.Path}");
            w.WriteLine($"B {b.Path}");
            var any = false;
            var settings = a.Settings.Join(b.Settings, s => s.Name, s => s.Name, (x, y) => (x, y)).Where(p => p.x.Value != p.y.Value).ToList();
            if (settings.Count > 0)
            {
                any = true;
                var width = settings.Max(p => p.x.Name.Length);
                w.WriteLine("settings");
                foreach (var (x, y) in settings)
                    w.WriteLine($"  {x.Name.PadRight(width)}  A {Read.Show(x.Value)}  B {Read.Show(y.Value)}");
            }
            var rows = new List<(string what, string av, string bv)>
            {
                ("scale", Value(a, "scale"), Value(b, "scale")),
                ("file unit", a.Unit, b.Unit),
                ("transforms", Value(a, "transforms"), Value(b, "transforms")),
                ("meshes", $"{a.Meshes.Count} ({Value(a, "verts")} verts, {Value(a, "tris")} tris)", $"{b.Meshes.Count} ({Value(b, "verts")} verts, {Value(b, "tris")} tris)"),
                ("blend shapes", string.Join(", ", a.Meshes.SelectMany(x => x.Shapes).Select(s => s.name)), string.Join(", ", b.Meshes.SelectMany(x => x.Shapes).Select(s => s.name))),
                ("index format", string.Join(", ", a.Meshes.Select(x => x.Index).Distinct()), string.Join(", ", b.Meshes.Select(x => x.Index).Distinct())),
                ("avatar", string.Join(", ", a.Avatars.Select(x => $"{x.name} {(a.AvatarOk(x) ? "valid" : x.valid ? "not human" : "invalid")}")), string.Join(", ", b.Avatars.Select(x => $"{x.name} {(b.AvatarOk(x) ? "valid" : x.valid ? "not human" : "invalid")}"))),
                ("humanoid map", Fmt.Plural(a.HumanMap.Count, "bone"), Fmt.Plural(b.HumanMap.Count, "bone")),
                ("material slots", string.Join(", ", Read.Slots(a).Select(s => Read.MaterialName(a, s.material))), string.Join(", ", Read.Slots(b).Select(s => Read.MaterialName(b, s.material)))),
                ("vertex layout", string.Join("; ", a.Meshes.Select(x => string.Join(" ", x.Attrs.Select(t => t.attr)))).Trim(), string.Join("; ", b.Meshes.Select(x => string.Join(" ", x.Attrs.Select(t => t.attr)))).Trim()),
                ("clips", string.Join(", ", a.Anims.Select(x => x.Name)), string.Join(", ", b.Anims.Select(x => x.Name))),
                ("remaps", string.Join(", ", a.Remaps.Select(r => r.name + "->" + r.target)), string.Join(", ", b.Remaps.Select(r => r.name + "->" + r.target))),
                ("messages", Fmt.Plural(a.Messages.Count, "message"), Fmt.Plural(b.Messages.Count, "message")),
            };
            var differing = rows.Where(r => r.av != r.bv).ToList();
            if (differing.Count > 0)
            {
                any = true;
                w.WriteLine("result");
                var width = differing.Max(r => r.what.Length);
                foreach (var r in differing)
                    w.WriteLine($"  {r.what.PadRight(width)}  A {(string.IsNullOrEmpty(r.av) ? "none" : r.av)}  B {(string.IsNullOrEmpty(r.bv) ? "none" : r.bv)}");
            }
            var mapA = a.HumanMap.ToDictionary(x => x.human, x => x.bone);
            var mapB = b.HumanMap.ToDictionary(x => x.human, x => x.bone);
            var human = mapA.Keys.Union(mapB.Keys).Where(k => !mapA.TryGetValue(k, out var x) || !mapB.TryGetValue(k, out var y) || x != y).ToList();
            if (human.Count > 0 && mapA.Count > 0 && mapB.Count > 0)
            {
                any = true;
                w.WriteLine("humanoid mapping");
                foreach (var h in human)
                    w.WriteLine($"  {h}  A {(mapA.TryGetValue(h, out var x) ? x : "-")}  B {(mapB.TryGetValue(h, out var y) ? y : "-")}");
            }
            foreach (var clip in a.Clips.Join(b.Clips, c => c.Name, c => c.Name, (x, y) => (x, y)))
            {
                var d = clip.x.Settings.Join(clip.y.Settings, s => s.Name, s => s.Name, (x, y) => (x, y)).Where(p => p.x.Value != p.y.Value).ToList();
                string Events(ClipSettings c) => string.Join(" ", c.Events.Select(e => $"{Fmt.Num(e.time)}:{e.function}"));
                if (Events(clip.x) != Events(clip.y))
                    d.Add((new Setting { Name = "events", Value = Events(clip.x) }, new Setting { Name = "events", Value = Events(clip.y) }));
                if (d.Count == 0)
                    continue;
                any = true;
                w.WriteLine($"clip {clip.x.Name}: " + string.Join(", ", d.Select(p => $"{p.x.Name} A {p.x.Value} B {p.y.Value}")));
            }
            if (!any)
                w.WriteLine("same settings and the same result summary");
            return 0;
        }
    }
}
