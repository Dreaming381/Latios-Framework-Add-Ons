using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClipWaitress.Model;

namespace ClipWaitress.Commands
{
    /// <summary>Commands across clips: find by settings and contents, diff two clips, and what uses a clip.</summary>
    public static class Query
    {
        static readonly string[] s_Ops = { ">=", "<=", "!=", "=", ">", "<", "~" };

        static readonly string[] s_Stats =
        {
            "name", "path", "kind", "length", "frames", "fps", "curves", "keys", "paths", "animates", "events", "event", "human", "legacy",
            "loop", "speed", "velocity.x", "velocity.y", "velocity.z", "turning", "size", "changed", "missing", "warnings", "gaps", "muscles", "params",
        };

        public static string StatHelp => string.Join(", ", s_Stats);

        public static int Find(Session session, Args args, TextWriter w)
        {
            var all = args.Flag("all");
            var list = args.Flag("list");
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
            List<ClipFile> files;
            if (targets.Count == 0)
                files = catalog.All.Where(f => all || f.AssetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)).ToList();
            else
            {
                files = new List<ClipFile>();
                foreach (var t in targets)
                {
                    if (catalog.TryFolder(t, out var under))
                        files.AddRange(under);
                    else
                        files.Add(catalog.TryAsset(t) ?? throw new WaitressException($"no folder or clip file '{t}'"));
                }
                files = files.Distinct().ToList();
            }
            if (files.Count == 0)
                throw new WaitressException("no clip files there");
            var clips = catalog.Get(files, session.Notes).SelectMany(r => r.Clips).ToList();
            if (clips.Count == 0)
            {
                w.WriteLine("no clips there");
                return 1;
            }
            var settingNames = clips[0].Settings.Select(s => s.Name).ToList();
            foreach (var c in conditions)
                if (!settingNames.Contains(c.key, StringComparer.OrdinalIgnoreCase) && !s_Stats.Contains(c.key, StringComparer.OrdinalIgnoreCase))
                    throw new WaitressException($"unknown key '{c.key}'; keys are clip settings ({string.Join(", ", settingNames)}) or {StatHelp}. {Read.Suggest(c.key, settingNames.Concat(s_Stats))}");
            var matches = clips.Where(c => conditions.All(k => Test(Value(c, k.key), k.op, k.value, k.key))).ToList();
            var shown = conditions.Where(c => c.op != "=").Select(c => c.key).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (matches.Count > 25 && !list)
            {
                foreach (var g in matches.GroupBy(c => c.Asset.Path.Substring(0, c.Asset.Path.LastIndexOf('/') + 1)).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
                    w.WriteLine(g.Count() == 1 ? Line(g.First()) : $"{g.Key}  {Fmt.Plural(g.Count(), "clip")}");
                w.WriteLine("(grouped by folder; --list prints every clip)");
            }
            else
                foreach (var c in matches)
                    w.WriteLine(Line(c));
            var scope = targets.Count == 0 ? (all ? "the project" : "Assets/ (--all adds packages)") : string.Join(", ", targets);
            w.WriteLine(conditions.Count == 0 ? $"{Fmt.Plural(clips.Count, "clip")} in {scope}" : $"{matches.Count} of {Fmt.Plural(clips.Count, "clip")} in {scope} match");
            return 0;

            string Line(ClipRecord c) => conditions.Count == 0 ? $"{c.Ref}  {Brief(c)}" : shown.Count == 0 ? c.Ref : $"{c.Ref}  {string.Join(" ", shown.Select(k => k + "=" + Read.Show(Shown(c, k))))}";

            // A contains-condition on a list shows only the entries that matched.
            string Shown(ClipRecord c, string key)
            {
                var value = Value(c, key);
                var texts = conditions.Where(x => x.op == "~" && x.key.Equals(key, StringComparison.OrdinalIgnoreCase)).Select(x => x.value).ToList();
                if (texts.Count == 0 || value == null || !value.Contains(','))
                    return value;
                return string.Join(",", value.Split(',').Where(v => texts.Any(t => v.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0)));
            }
        }

        static string Brief(ClipRecord c)
        {
            var parts = new List<string> { Fmt.Num(c.Length) + "s", c.Legacy ? "legacy" : c.Human ? "humanoid" : "generic" };
            if (c.On("loopTime"))
                parts.Add("loop");
            parts.Add(Fmt.Plural(c.Curves.Count + c.Objects.Count, "curve"));
            if (c.Events.Count > 0)
                parts.Add(Fmt.Plural(c.Events.Count, "event"));
            var warnings = Analysis.Warnings(c).Count;
            if (warnings > 0)
                parts.Add(Fmt.Plural(warnings, "warning"));
            if (c.Asset.Stale)
                parts.Add("stale");
            return string.Join(", ", parts);
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

        public static string Value(ClipRecord c, string key)
        {
            var setting = c.Settings.FirstOrDefault(s => s.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (setting != null)
                return setting.Value;
            string N(double d) => Fmt.Num(d);
            switch (key.ToLowerInvariant())
            {
                case "name": return c.Name;
                case "path": return c.Ref;
                case "kind": return c.Asset.Kind;
                case "length": return N(c.Length);
                case "frames": return N(c.Frames);
                case "fps": return N(c.Fps);
                case "curves": return N(c.Curves.Count + c.Objects.Count);
                case "keys": return N(c.Keys);
                case "paths": return N(c.Curves.Select(x => x.Path).Concat(c.Objects.Select(o => o.Path)).Distinct().Count());
                case "animates":
                    return string.Join(",", c.Curves.Select(x => (x.Path.Length == 0 ? "" : x.Path) + ":" + Bindings.Group(x)).Concat(c.Objects.Select(o => o.Path + ":" + o.Name)).Distinct());
                case "events": return N(c.Events.Count);
                case "event": return string.Join(",", c.Events.Select(e => e.Function).Distinct());
                case "human": return c.Human ? "true" : "false";
                case "legacy": return c.Legacy ? "true" : "false";
                case "loop": return c.Get("loopTime");
                case "speed": return N(c.Speed);
                case "velocity.x": return N(c.AverageSpeed[0]);
                case "velocity.y": return N(c.AverageSpeed[1]);
                case "velocity.z": return N(c.AverageSpeed[2]);
                case "turning": return N(c.AverageAngularSpeed);
                case "size": return N(c.Stats.TryGetValue("size", out var s) ? s : 0);
                case "changed": return string.Join(",", c.Settings.Where(x => x.Changed && x.Name != "stopTime").Select(x => x.Name));
                case "missing": return N(c.Objects.Sum(o => o.Missing));
                case "warnings": return N(Analysis.Warnings(c).Count);
                case "gaps": return N(c.On("loopTime") ? Analysis.LoopGaps(c).Count : 0);
                case "muscles": return N(c.Curves.Count(x => x.Class == "muscle"));
                case "params": return string.Join(",", c.Curves.Where(x => x.Class == "param").Select(x => x.Prop));
            }
            return null;
        }

        static readonly string[] s_Lists = { "event", "animates", "params", "changed" };

        public static bool Test(string actual, string op, string expected, string key = null)
        {
            actual ??= "";
            // = and != on a list test membership.
            if (key != null && s_Lists.Contains(key, StringComparer.OrdinalIgnoreCase) && op is "=" or "!=" && !double.TryParse(expected, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                var member = actual.Split(',').Any(v => string.Equals(v, expected, StringComparison.OrdinalIgnoreCase));
                return op == "=" ? member : !member;
            }
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
                case "=": return numeric ? Math.Abs(a - e) < 1e-4 : string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
                case "!=": return numeric ? Math.Abs(a - e) >= 1e-4 : !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
            }
            if (!numeric)
                throw new WaitressException($"{op} compares numbers; '{expected}' is not one");
            return op switch { ">" => a > e, "<" => a < e, ">=" => a >= e, _ => a <= e };
        }

        // ------------------------------------------------------------ users

        public static int Users(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            var all = args.Flag("all");
            args.RejectUnknown();
            var clip = session.One(args.Positional0("a clip"));
            var uses = Model.Users.Find(session.Catalog, clip, all);
            var players = 0;
            foreach (var u in uses)
            {
                w.WriteLine($"{u.File}  {string.Join("; ", u.What.Take(8))}{(u.What.Count > 8 ? $"; +{u.What.Count - 8} more" : "")}");
                Holders(u, "  ");
            }
            w.WriteLine($"{Fmt.Plural(uses.Count, "file")} in {(all ? "Assets/ and Packages/" : "Assets/ (--all adds Packages/)")} {(uses.Count == 1 ? "references" : "reference")} {clip.Ref}" +
                        (players > 0 ? $"; {Fmt.Plural(players, "file")} {(players == 1 ? "holds" : "hold")} Animators that play it" : "") + " (text-serialized scenes, prefabs, controllers, timelines)");
            return 0;

            void Holders(Use u, string indent)
            {
                foreach (var h in u.Holders)
                {
                    players += h.IsController ? 0 : 1;
                    w.WriteLine($"{indent}{(h.IsController ? "through " : "played by ")}{h.File}  {string.Join("; ", h.What.Take(6))}{(h.What.Count > 6 ? $"; +{h.What.Count - 6} more" : "")}");
                    Holders(h, indent + "  ");
                }
            }
        }

        // ------------------------------------------------------------ diff

        public static int Diff(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var a = session.One(args.PositionalAt(0, "two clips"));
            var b = session.One(args.PositionalAt(1, "two clips"));
            w.WriteLine($"A {a.Ref}");
            w.WriteLine($"B {b.Ref}");
            var any = false;
            var rows = new List<(string what, string av, string bv)>
            {
                ("length", Fmt.Num(a.Length) + "s", Fmt.Num(b.Length) + "s"), ("fps", Fmt.Num(a.Fps), Fmt.Num(b.Fps)),
                ("kind", a.Human ? "humanoid" : "generic", b.Human ? "humanoid" : "generic"), ("legacy", a.Legacy.ToString(), b.Legacy.ToString()),
                ("curves", Fmt.Count(a.Curves.Count + a.Objects.Count), Fmt.Count(b.Curves.Count + b.Objects.Count)),
                ("average velocity", Fmt.Vec(a.AverageSpeed), Fmt.Vec(b.AverageSpeed)),
                ("events", string.Join(" ", a.Events.Select(e => $"{Fmt.Num(e.Time)}:{e.Function}({e.Param})")), string.Join(" ", b.Events.Select(e => $"{Fmt.Num(e.Time)}:{e.Function}({e.Param})"))),
            };
            rows.AddRange(a.Settings.Join(b.Settings, s => s.Name, s => s.Name, (x, y) => (x.Name, x.Value, y.Value)).Where(r => r.Item2 != r.Item3 && !(r.Name == "stopTime")));
            var differing = rows.Where(r => r.av != r.bv).ToList();
            if (differing.Count > 0)
            {
                any = true;
                var width = differing.Max(r => r.what.Length);
                foreach (var r in differing)
                    w.WriteLine($"  {r.what.PadRight(width)}  A {(string.IsNullOrEmpty(r.av) ? "none" : r.av)}  B {(string.IsNullOrEmpty(r.bv) ? "none" : r.bv)}");
            }
            string Key(Curve c) => c.Path + ":" + c.Type + "." + c.Prop;
            var mapB = b.Curves.GroupBy(Key).ToDictionary(g => g.Key, g => g.First());
            var keysA = new HashSet<string>(a.Curves.Select(Key));
            // Curve pairs to compare: same binding, or the same binding under a renamed path.
            var pairs = a.Curves.Where(c => mapB.ContainsKey(Key(c))).Select(c => (x: c, y: mapB[Key(c)])).ToList();
            var onlyA = a.Curves.Where(c => !mapB.ContainsKey(Key(c))).ToList();
            var onlyB = b.Curves.Where(c => !keysA.Contains(Key(c))).ToList();
            if (onlyA.Count > 0 && onlyA.Count == onlyB.Count)
            {
                var from = onlyA.Select(c => c.Path).OrderBy(p => p.Length).First();
                var to = onlyB.Select(c => c.Path).OrderBy(p => p.Length).First();
                var moved = onlyA.Select(c => (x: c, y: onlyB.FirstOrDefault(d => d.Type == c.Type && d.Prop == c.Prop &&
                                                                              (c.Path == from || c.Path.StartsWith(from + "/", StringComparison.Ordinal) || from.Length == 0) &&
                                                                              d.Path == (to + c.Path.Substring(from.Length)).Trim('/')))).ToList();
                if (moved.All(m => m.y != null))
                {
                    any = true;
                    w.WriteLine($"path renamed: {Bindings.Label(from)} (A) -> {Bindings.Label(to)} (B) and below, {Fmt.Plural(moved.Count, "curve")} on {Fmt.Plural(moved.Select(m => m.x.Path).Distinct().Count(), "path")}; their values are compared below");
                    pairs.AddRange(moved);
                    onlyA.Clear();
                    onlyB.Clear();
                }
            }
            string Groups(List<Curve> curves) => string.Join(", ", Bindings.Groups(curves).Select(g => (g.path.Length == 0 ? "" : g.path) + ":" + g.group).Take(10)) +
                                                 (Bindings.Groups(curves).Count > 10 ? ", ..." : "");
            if (onlyA.Count > 0)
            {
                any = true;
                w.WriteLine($"only in A: {Fmt.Plural(onlyA.Count, "curve")}: {Groups(onlyA)}");
            }
            if (onlyB.Count > 0)
            {
                any = true;
                w.WriteLine($"only in B: {Fmt.Plural(onlyB.Count, "curve")}: {Groups(onlyB)}");
            }
            // Every frame of the faster clip over the shorter one, and its end.
            var end = Math.Min(a.Length, b.Length);
            var fps = Math.Max(a.Fps, b.Fps);
            var times = Enumerable.Range(0, (int)Math.Min(2000, Math.Floor(end * fps + 1e-6)) + 1).Select(i => i / fps).Append(end).Distinct().ToList();
            var diffs = new List<(string name, double size, double at, double av, double bv)>();
            string Label(Curve c) => (c.Path.Length == 0 ? "" : c.Path) + ":" + Bindings.Name(c);
            foreach (var group in pairs.GroupBy(p => (p.x.Path, Bindings.Group(p.x))))
            {
                var qa = Bindings.Ordered(group.Select(p => p.x));
                if (Bindings.IsQuaternion(qa))
                {
                    var qb = Bindings.Ordered(group.Select(p => p.y));
                    (double size, double at) worst = (0, 0);
                    foreach (var t in times)
                    {
                        var d = Bindings.Angle(qa.Select(x => x.Evaluate(t)).ToArray(), qb.Select(x => x.Evaluate(t)).ToArray());
                        if (d > worst.size)
                            worst = (d, t);
                    }
                    if (worst.size > 0.01)
                        diffs.Add(((group.Key.Path.Length == 0 ? "" : group.Key.Path) + ":" + group.Key.Item2 + " (degrees)", worst.size, worst.at, double.NaN, double.NaN));
                    continue;
                }
                foreach (var (x, y) in group)
                {
                    (double size, double at, double av, double bv) worst = (0, 0, 0, 0);
                    foreach (var t in times)
                    {
                        var (p, q) = (x.Evaluate(t), y.Evaluate(t));
                        var d = x.Class == "euler" ? Math.Abs(Bindings.Wrap(q - p)) : Math.Abs(q - p);
                        if (d > worst.size)
                            worst = (d, t, p, q);
                    }
                    if (worst.size > 1e-4)
                        diffs.Add((Label(x), worst.size, worst.at, worst.av, worst.bv));
                }
            }
            if (diffs.Count > 0)
            {
                any = true;
                w.WriteLine($"{Fmt.Plural(diffs.Count, "shared curve")} differ{(diffs.Count == 1 ? "s" : "")} (largest first, sampled at every frame of the shorter clip):");
                foreach (var d in diffs.OrderByDescending(d => d.size).Take(12))
                    w.WriteLine($"  {d.name}  by {Fmt.Num(d.size)} at {Fmt.Num(d.at)}s" + (double.IsNaN(d.av) ? "" : $" (A {Fmt.Num(d.av)}, B {Fmt.Num(d.bv)})"));
                if (diffs.Count > 12)
                    w.WriteLine($"  +{diffs.Count - 12} more");
            }
            if (!any)
                w.WriteLine("same settings, events and curve values");
            return 0;
        }
    }
}
