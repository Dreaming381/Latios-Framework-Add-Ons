using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ClipWaitress.Model;

namespace ClipWaitress.Commands
{
    /// <summary>Commands that go through the Editor: changing clips, copying one, re-exporting, and checking bindings.</summary>
    public static class Edit
    {
        static readonly Regex s_Bracket = new Regex(@"^(path|keys)\[(.*)\](\+=|-=|=)(.*)$", RegexOptions.Singleline);

        public static int Set(Session session, Args args, TextWriter w)
        {
            var dryRun = args.Flag("dry-run");
            args.RejectUnknown();
            var targets = new List<string>();
            var ops = new List<(string key, string op, string value)>();
            foreach (var p in args.Positional)
            {
                var m = s_Bracket.Match(p);
                if (m.Success)
                {
                    ops.Add((m.Groups[1].Value + "[" + m.Groups[2].Value + "]", m.Groups[3].Value, m.Groups[4].Value));
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
                throw new WaitressException("missing a clip (or folder) to change");
            if (ops.Count == 0)
                throw new WaitressException("missing key=value; see `help set`");
            var catalog = session.Catalog;
            var clips = targets.SelectMany(session.Clips).Distinct().ToList();
            var cache = Path.Combine(catalog.Root, "Library", "PackageCache") + Path.DirectorySeparatorChar;
            var immutable = clips.FirstOrDefault(c => Path.GetFullPath(c.Asset.FullPath).StartsWith(cache, StringComparison.OrdinalIgnoreCase));
            if (immutable != null)
                throw new WaitressException($"{immutable.Ref} is in a registry package, which Unity keeps read-only; `copy` it into Assets/ to change it");
            var request = JsonSerializer.Serialize(new
            {
                dryRun,
                ops = clips.SelectMany(c => ops.Select(o => Resolved(c, o)).Select(o => new { path = c.Asset.Path, id = c.Id, o.key, o.op, o.value })).ToArray(),
            });
            var result = EditorScript.Call(session.Root, "Edit", request);
            if (dryRun)
            {
                var previews = result.GetProperty("previews").EnumerateArray().Select(e => e.GetString()).ToList();
                foreach (var (c, json) in clips.Zip(previews))
                {
                    using var doc = JsonDocument.Parse(json);
                    w.WriteLine(c.Ref + " (--dry-run: nothing was changed)");
                    foreach (var line in Delta(c, ClipRecord.Parse(doc.RootElement, c.Asset)))
                        w.WriteLine("  " + line);
                }
                return 0;
            }
            foreach (var note in result.GetProperty("notes").EnumerateArray())
                w.WriteLine("note: " + note.GetString());
            var done = result.GetProperty("done").EnumerateArray().Select(e => e.GetString()).ToList();
            // A rename moves the file, so look the clips up again in a fresh catalog.
            var after = new Catalog(catalog.Root);
            // The Editor reports each clip's file, in the order the clips were given.
            foreach (var (c, path) in clips.Zip(done))
            {
                var file = after.TryAsset(path);
                var now = file == null ? null : after.Get(file, session.Notes).Clips.FirstOrDefault(x => x.Id == c.Id);
                if (now == null)
                {
                    w.WriteLine($"{c.Ref}: changed, but couldn't be re-read");
                    continue;
                }
                w.WriteLine(now.Ref + (now.Ref != c.Ref ? $" (was {c.Ref})" : ""));
                foreach (var line in Delta(c, now))
                    w.WriteLine("  " + line);
            }
            return 0;
        }

        // Paths in path[...], keys[...] and curves-= may be a unique tail of a path the clip animates.
        static (string key, string op, string value) Resolved(ClipRecord c, (string key, string op, string value) o)
        {
            var paths = c.Curves.Select(x => x.Path).Concat(c.Objects.Select(x => x.Path)).Distinct().ToList();
            string Path(string p)
            {
                p = p.Trim().Trim('/');
                if (p.Length == 0 || paths.Contains(p) || paths.Any(x => x.StartsWith(p + "/", StringComparison.Ordinal)))
                    return p;
                var hits = paths.Where(x => x.EndsWith("/" + p, StringComparison.Ordinal)).ToList();
                if (hits.Count > 1)
                    throw new WaitressException($"'{p}' ends {hits.Count} paths in {c.Ref}; use more of it:\n  " + string.Join("\n  ", hits.Take(8)));
                return hits.Count == 1 ? hits[0] : p;
            }
            string Binding(string b)
            {
                var colon = b.LastIndexOf(':');
                return colon < 0 ? Path(b) : Path(b.Substring(0, colon)) + b.Substring(colon);
            }
            if (o.key.StartsWith("path[", StringComparison.Ordinal))
            {
                // Old given as a tail renames just that tail: New keeps the parent path unless it is a full path itself.
                var tail = o.key.Substring(5, o.key.Length - 6).Trim().Trim('/');
                var resolved = Path(tail);
                var value = o.value.Trim().Trim('/');
                if (resolved != tail && resolved.EndsWith("/" + tail, StringComparison.Ordinal) && value.Length > 0 && !value.Contains('/'))
                    value = resolved.Substring(0, resolved.Length - tail.Length) + value;
                return ("path[" + resolved + "]", o.op, value);
            }
            if (o.key.StartsWith("keys[", StringComparison.Ordinal))
                return ("keys[" + Binding(o.key.Substring(5, o.key.Length - 6)) + "]", o.op, o.value);
            if (o.key == "curves" && o.op == "-=")
                return (o.key, o.op, Binding(o.value));
            return o;
        }

        /// <summary>What changed between two exports of a clip.</summary>
        public static List<string> Delta(ClipRecord was, ClipRecord now)
        {
            var lines = new List<string>();
            foreach (var s in now.Settings)
            {
                var old = was.Get(s.Name);
                if (old != s.Value && !(s.Name == "stopTime" && Math.Abs(now.StopTime - now.Length) < 1e-4 && Math.Abs(was.StopTime - was.Length) < 1e-4))
                    lines.Add($"{s.Name}: {Read.Show(old)} -> {Read.Show(s.Value)}");
            }
            if (was.Name != now.Name)
                lines.Add($"name: {was.Name} -> {now.Name}");
            if (Math.Abs(was.Fps - now.Fps) > 1e-6)
                lines.Add($"frameRate: {Fmt.Num(was.Fps)} -> {Fmt.Num(now.Fps)}");
            if (Math.Abs(was.Length - now.Length) > 1e-6)
                lines.Add($"length: {Fmt.Num(was.Length)}s -> {Fmt.Num(now.Length)}s");
            static string Event(ClipEvent e) => $"{Fmt.Num(e.Time)}s {e.Function}({e.Param})";
            var oldEvents = was.Events.Select(Event).ToList();
            var newEvents = now.Events.Select(Event).ToList();
            if (!oldEvents.SequenceEqual(newEvents))
            {
                var gone = oldEvents.Where(e => !newEvents.Contains(e)).ToList();
                var plus = newEvents.Where(e => !oldEvents.Contains(e)).ToList();
                var parts = new List<string>();
                if (plus.Count > 0)
                    parts.Add("added " + string.Join("; ", plus));
                if (gone.Count > 0)
                    parts.Add("removed " + string.Join("; ", gone));
                lines.Add($"events ({oldEvents.Count} -> {newEvents.Count}): {(parts.Count == 0 ? "reordered" : string.Join("; ", parts))}");
            }
            string Key(Curve c) => c.Path + "\n" + c.Type + "." + c.Prop;
            string Label(Curve c) => (c.Path.Length == 0 ? "" : c.Path) + ":" + Bindings.Group(c);
            var before = was.Curves.GroupBy(Key).ToDictionary(g => g.Key, g => g.First());
            var afterMap = now.Curves.GroupBy(Key).ToDictionary(g => g.Key, g => g.First());
            var removed = was.Curves.Where(c => !afterMap.ContainsKey(Key(c))).ToList();
            var added = now.Curves.Where(c => !before.ContainsKey(Key(c))).ToList();
            var changed = now.Curves.Where(c => before.TryGetValue(Key(c), out var b) && !Same(b, c)).ToList();
            // Curves that only changed path read as one move: every removed path maps onto an added one by a prefix swap.
            if (removed.Count > 0 && removed.Count == added.Count)
            {
                var from = removed.Select(c => c.Path).OrderBy(p => p.Length).First();
                var to = added.Select(c => c.Path).OrderBy(p => p.Length).First();
                string Moved(Curve c) => (to + c.Path.Substring(from.Length)).Trim('/') + "\n" + c.Type + "." + c.Prop;
                var addedKeys = new HashSet<string>(added.Select(Key));
                if (removed.All(c => (c.Path == from || c.Path.StartsWith(from + "/", StringComparison.Ordinal) || from.Length == 0) && addedKeys.Contains(Moved(c))))
                {
                    lines.Add($"moved {Fmt.Plural(removed.Count, "curve")} on {Fmt.Plural(removed.Select(c => c.Path).Distinct().Count(), "path")}: {Bindings.Label(from)} -> {Bindings.Label(to)} (and below)");
                    removed.Clear();
                    added.Clear();
                }
            }
            foreach (var g in removed.GroupBy(c => c.Path).Take(8))
                lines.Add($"removed {Fmt.Plural(g.Count(), "curve")} on {Bindings.Label(g.Key)} ({string.Join(" ", g.Select(Bindings.Group).Distinct())})");
            var oldPaths = new HashSet<string>(was.Curves.Select(c => c.Path).Concat(was.Objects.Select(o => o.Path)));
            foreach (var g in added.GroupBy(c => c.Path).Take(8))
                lines.Add($"added {Fmt.Plural(g.Count(), "curve")} on {Bindings.Label(g.Key)} ({string.Join(" ", g.Select(Bindings.Group).Distinct())})" +
                          (oldPaths.Contains(g.Key) ? "" : oldPaths.Any(p => p.StartsWith(g.Key + "/", StringComparison.Ordinal) || g.Key.StartsWith(p + "/", StringComparison.Ordinal)) ? " (new path)" : " (new path, not under any path the clip animated)"));
            // A re-keyed group shows the keys added, removed or changed, in the units `sample` uses.
            foreach (var label in changed.Select(Label).Distinct().Take(8))
            {
                var nowGroup = Bindings.Ordered(now.Curves.Where(c => Label(c) == label));
                var wasGroup = Bindings.Ordered(was.Curves.Where(c => Label(c) == label));
                var path = nowGroup[0].Path;
                var order = nowGroup[0].Class == "euler" && now.EulerOrder.TryGetValue(path, out var o) ? o : 4;
                string At(List<Curve> g, double t) => string.Join(",", Bindings.Value(g, t, order).Select(Fmt.Num));
                var oldTimes = wasGroup.SelectMany(c => c.T).Distinct().ToList();
                var newTimes = nowGroup.SelectMany(c => c.T).Distinct().ToList();
                bool Has(List<double> times, double t) => times.Any(x => Math.Abs(x - t) < 1e-5);
                var parts = new List<string>();
                var add = newTimes.Where(t => !Has(oldTimes, t)).ToList();
                var gone = oldTimes.Where(t => !Has(newTimes, t)).ToList();
                var moved = newTimes.Where(t => Has(oldTimes, t) && At(wasGroup, t) != At(nowGroup, t)).ToList();
                if (add.Count > 0)
                    parts.Add("added " + string.Join(" ", add.Take(6).Select(t => $"{Fmt.Num(t)}s:({At(nowGroup, t)})")) + (add.Count > 6 ? " ..." : ""));
                if (gone.Count > 0)
                    parts.Add("removed " + string.Join(" ", gone.Take(6).Select(t => Fmt.Num(t) + "s")) + (gone.Count > 6 ? " ..." : ""));
                if (moved.Count > 0)
                    parts.Add("changed " + string.Join(" ", moved.Take(6).Select(t => $"{Fmt.Num(t)}s:({At(wasGroup, t)})->({At(nowGroup, t)})")) + (moved.Count > 6 ? " ..." : ""));
                if (parts.Count == 0)
                    parts.Add("tangents changed");
                lines.Add($"{label}: {Fmt.Plural(oldTimes.Count, "key")} -> {Fmt.Plural(newTimes.Count, "key")}; {string.Join("; ", parts)}");
                // A new or changed key right next to another that differs a lot reads as a pop.
                var rotation = nowGroup[0].Class is "euler" or "quat" || Bindings.IsQuaternion(nowGroup);
                var range = nowGroup.Max(c => c.Max - c.Min);
                var sorted = newTimes.OrderBy(t => t).ToList();
                foreach (var t in add.Concat(moved))
                {
                    var i = sorted.FindIndex(x => Math.Abs(x - t) < 1e-5);
                    foreach (var j in new[] { i - 1, i + 1 }.Where(j => j >= 0 && j < sorted.Count && Math.Abs(sorted[j] - t) <= 1.5 / now.Fps))
                    {
                        var a = Bindings.Value(nowGroup, t, order);
                        var b = Bindings.Value(nowGroup, sorted[j], order);
                        var jump = a.Zip(b, (p, q) => rotation ? Math.Abs(Bindings.Wrap(q - p)) : Math.Abs(q - p)).Max();
                        if (rotation ? jump > 5 : jump > 0.1 * Math.Max(range, 1e-6))
                            lines.Add($"warning: the key at {Fmt.Num(t)}s is one frame from the key at {Fmt.Num(sorted[j])}s and differs by {Fmt.Num(jump)}{(rotation ? "°" : "")}, which plays as a pop");
                    }
                }
            }
            var newWarnings = Analysis.Warnings(now).Except(Analysis.Warnings(was)).ToList();
            foreach (var x in newWarnings)
                lines.Add("warning: " + x);
            if (lines.Count == 0)
                lines.Add("(unchanged)");
            return lines;
        }

        static bool Same(Curve a, Curve b) => a.Count == b.Count && Enumerable.Range(0, a.Count).All(i => a.T[i] == b.T[i] && a.V[i] == b.V[i] && Eq(a.In[i], b.In[i]) && Eq(a.Out[i], b.Out[i]));
        static bool Eq(double x, double y) => x == y || double.IsNaN(x) && double.IsNaN(y);

        // ------------------------------------------------------------ copy

        public static int Copy(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var clip = session.One(args.PositionalAt(0, "a clip and a destination .anim"));
            var dest = args.PositionalAt(1, "a clip and a destination .anim").Replace('\\', '/');
            var root = session.Root;
            // A relative path is taken from the working directory when that is inside the project, else from the project.
            var full = Path.GetFullPath(dest);
            if (!full.StartsWith(Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase))
                full = Path.GetFullPath(Path.Combine(root, dest));
            dest = Path.GetRelativePath(root, full).Replace('\\', '/');
            var name = Path.GetFileName(dest);
            if (!dest.StartsWith("Assets/", StringComparison.Ordinal) || name.Length == 0)
                throw new WaitressException($"the copy goes to a file under Assets/ (Assets/.../New.anim), not '{args.PositionalAt(1, "a destination")}'");
            if (Path.HasExtension(name) && !name.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException($"the copy is an .anim file, not {Path.GetExtension(name)}");
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.IndexOfAny(new[] { ':', '*', '?', '"', '<', '>', '|' }) >= 0 || name.TrimEnd('.').Length == 0)
                throw new WaitressException($"'{name}' isn't a valid file name");
            if (!dest.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                dest += ".anim";
            var result = EditorScript.Call(root, "Copy", JsonSerializer.Serialize(new { path = clip.Asset.Path, id = clip.Id, dest }));
            var made = result.GetProperty("path").GetString();
            var catalog = new Catalog(root);
            var copy = catalog.Get(catalog.TryAsset(made) ?? throw new WaitressException($"copied to {made}, but it couldn't be read back"), session.Notes).Clips.First();
            w.WriteLine($"copied {clip.Ref} to {made} ({Fmt.Plural(copy.Curves.Count + copy.Objects.Count, "curve")}, {Fmt.Plural(copy.Events.Count, "event")}); edit it with `set {made} ...`");
            return 0;
        }

        // ------------------------------------------------------------ refresh

        public static int Refresh(Session session, Args args, TextWriter w)
        {
            var force = args.Flag("force");
            args.RejectUnknown();
            var catalog = session.Catalog;
            var pruned = catalog.Prune();
            if (pruned > 0)
                w.WriteLine($"removed {Fmt.Plural(pruned, "record")} of clip files no longer in the project");
            var files = new List<ClipFile>();
            if (args.Positional.Count == 0)
                files.AddRange(catalog.All);
            foreach (var t in args.Positional)
            {
                if (catalog.TryFolder(t, out var under))
                    files.AddRange(under);
                else
                    files.Add(catalog.TryAsset(t.Split('#')[0]) ?? throw new WaitressException($"no clip file '{t}'"));
            }
            files = files.Distinct().ToList();
            var stale = files.Count(f => !catalog.IsCurrent(f));
            var records = catalog.Get(files, session.Notes, force);
            w.WriteLine($"{Fmt.Plural(records.Count, "clip file")} ({Fmt.Plural(records.Sum(r => r.Clips.Count), "clip")}) current; {(force ? "re-exported all" : $"exported {stale}")}");
            return 0;
        }

        // ------------------------------------------------------------ check

        public static int Check(Session session, Args args, TextWriter w)
        {
            var on = args.GetAll("on").ToList();
            var all = args.Flag("all");
            args.RejectUnknown();
            if (args.Positional.Count == 0)
                throw new WaitressException("missing a clip (or folder) to check");
            var catalog = session.Catalog;
            var clips = args.Positional.SelectMany(session.Clips).Distinct().ToList();
            var targets = new List<string>();
            var skipped = new List<string>();
            foreach (var t in on)
                targets.Add(AssetPath(catalog.Root, t));
            if (on.Count == 0)
            {
                foreach (var c in clips)
                {
                    var found = Users.Find(catalog, c, all);
                    var holders = found.Where(u => !u.IsController).Select(u => u.File)
                                       .Concat(found.SelectMany(Flatten).Where(u => !u.IsController).Select(u => u.File)).Distinct().ToList();
                    if (holders.Count == 0 && c.Asset.Kind == "model")
                        holders.Add(c.Asset.Path);
                    targets.AddRange(holders);
                    if (holders.Count == 0)
                        skipped.Add(c.Ref);
                }
                targets = targets.Distinct().ToList();
                if (targets.Count == 0)
                    throw new WaitressException($"nothing in {(all ? "the project" : "Assets/")} plays {(clips.Count == 1 ? clips[0].Ref : "these clips")} (no prefab, model or scene with an Animator or Animation using it); name the object with --on <prefab|model|open scene>");
            }
            var request = JsonSerializer.Serialize(new { clips = clips.Select(c => new { path = c.Asset.Path, id = c.Id }).ToArray(), targets, strict = on.Count == 0 });
            var result = EditorScript.Call(session.Root, "Check", request);
            var bad = 0;
            var checkedAny = false;
            foreach (var r in result.GetProperty("results").EnumerateArray())
            {
                var target = r.GetProperty("target").GetString();
                if (r.TryGetProperty("skipped", out var why))
                {
                    w.WriteLine($"{target}: skipped ({why.GetString()})");
                    continue;
                }
                checkedAny = true;
                bad += Report(r, clips, w);
            }
            foreach (var s in skipped)
                w.WriteLine($"{s}: nothing plays it here (check it against an object with --on)");
            if (!checkedAny)
            {
                w.WriteLine("nothing was checked: open the scene in the Editor, or name a prefab or model with --on");
                return 2;
            }
            return bad > 0 ? 1 : 0;
        }

        static int Distance(string a, string b)
        {
            var d = new int[a.Length + 1, b.Length + 1];
            for (var x = 0; x <= a.Length; x++)
                d[x, 0] = x;
            for (var y = 0; y <= b.Length; y++)
                d[0, y] = y;
            for (var x = 1; x <= a.Length; x++)
                for (var y = 1; y <= b.Length; y++)
                    d[x, y] = Math.Min(Math.Min(d[x - 1, y] + 1, d[x, y - 1] + 1), d[x - 1, y - 1] + (a[x - 1] == b[y - 1] ? 0 : 1));
            return d[a.Length, b.Length];
        }

        static IEnumerable<Use> Flatten(Use u) => u.Holders.Concat(u.Holders.SelectMany(Flatten));

        static string AssetPath(string root, string t)
        {
            var full = Path.GetFullPath(File.Exists(t) || Directory.Exists(t) ? t : Path.Combine(root, t));
            if (!File.Exists(full))
                throw new WaitressException($"--on {t}: no such file (a prefab, a model, or a scene open in the Editor)");
            return Path.GetRelativePath(root, full).Replace('\\', '/');
        }

        static int Report(JsonElement r, List<ClipRecord> clips, TextWriter w)
        {
            var holder = r.GetProperty("holder").GetString();
            var avatar = r.TryGetProperty("avatar", out var av) ? ", avatar " + av.GetString() : "";
            var clipName = r.GetProperty("clip").GetString();
            var clip = clips.FirstOrDefault(c => c.Asset.Path + "#" + c.Name == clipName);
            var shown = clip?.Ref ?? clipName;
            var controller = r.TryGetProperty("controller", out var ctl) ? $" with controller {ctl.GetString()}" : "";
            w.WriteLine($"{shown} on {r.GetProperty("target").GetString()}: {r.GetProperty("object").GetString()} ({(holder == "none" ? "no Animator or Animation; checked against the object itself" : holder + controller)}{avatar})");
            if (r.TryGetProperty("plays", out var plays) && plays.ValueKind == JsonValueKind.False)
                w.WriteLine($"  note      this {holder} doesn't play the clip{(holder == "Animator" ? " (no state of its controller uses it)" : "")}; checked as if it did");
            var bad = 0;
            var missing = r.GetProperty("missing").EnumerateArray().ToList();
            var lostPaths = missing.Where(m => m.GetProperty("why").GetString() == "path").ToList();
            foreach (var g in lostPaths.GroupBy(m => m.GetProperty("first").GetString()))
            {
                bad++;
                var first = g.Key;
                var exists = g.First().GetProperty("exists").GetString();
                var same = g.First().GetProperty("same").EnumerateArray().Select(e => e.GetString()).ToList();
                var curves = g.Sum(m => m.GetProperty("curves").GetInt32());
                var leaf = first.Substring(first.LastIndexOf('/') + 1);
                var paths = g.Count() == 1 && g.First().GetProperty("path").GetString() == first ? first : $"{first} and {Fmt.Plural(g.Count(f => f.GetProperty("path").GetString() != first), "path")} under it";
                var line = $"  missing   {paths} ({Fmt.Plural(curves, "curve")}): no '{leaf}' under {(exists.Length == 0 ? "the root" : exists)}";
                var children = g.First().TryGetProperty("children", out var ch) ? ch.EnumerateArray().Select(e => e.GetString()).ToList() : new List<string>();
                var similar = children.Select(n => (n, d: Distance(n.ToLowerInvariant(), leaf.ToLowerInvariant()))).Where(x => x.d <= Math.Max(2, leaf.Length / 3)).OrderBy(x => x.d).Select(x => x.n).FirstOrDefault();
                if (same.Count == 1)
                    line += $"; an object named '{leaf}' is at {same[0]}. Fix: set {shown} \"path[{first}]={same[0]}\"";
                else if (same.Count > 1)
                    line += $"; objects named '{leaf}' are at {string.Join(", ", same)}";
                else if (similar != null)
                {
                    var to = exists.Length == 0 ? similar : exists + "/" + similar;
                    line += $"; it has a child '{similar}'. If that's the renamed object: set {shown} \"path[{first}]={to}\"";
                }
                else if (children.Count > 0)
                    line += $"; its children are {string.Join(", ", children.Take(8))}{(children.Count > 8 ? ", ..." : "")}";
                w.WriteLine(line);
            }
            foreach (var m in missing.Where(m => m.GetProperty("why").GetString() != "path"))
            {
                bad++;
                w.WriteLine($"  missing   {Bindings.Label(m.GetProperty("path").GetString())} ({Fmt.Plural(m.GetProperty("curves").GetInt32(), "curve")}): {m.GetProperty("why").GetString()}");
            }
            foreach (var p in r.GetProperty("props").EnumerateArray())
            {
                bad++;
                var text = p.GetString();
                var bar = text.IndexOf('|');
                w.WriteLine($"  property  {(bar < 0 ? text : text.Substring(0, bar))}: the object has no such property{(bar < 0 ? "" : "; " + text.Substring(bar + 1))}");
            }
            foreach (var p in r.GetProperty("problems").EnumerateArray())
            {
                bad++;
                var text = p.GetString();
                w.WriteLine("  problem   " + text.Substring(text.IndexOf('|') + 1));
            }
            var receiverNote = false;
            foreach (var e in r.GetProperty("events").EnumerateArray())
            {
                bad++;
                var text = e.GetString();
                const string note = " (Unity logs \"AnimationEvent has no receiver\")";
                if (receiverNote)
                    text = text.Replace(note, "");
                receiverNote |= text.Contains(note);
                w.WriteLine("  event     " + text);
            }
            foreach (var n in r.GetProperty("notes").EnumerateArray())
                w.WriteLine("  note      " + n.GetString());
            if (bad == 0)
            {
                var count = clip == null ? 0 : clip.Curves.Count + clip.Objects.Count;
                w.WriteLine($"  ok: {(count > 0 ? $"all {Fmt.Plural(count, "curve")} bind" : "every curve binds")}{(clip != null && clip.Events.Count > 0 ? ", every event has a receiver" : "")}");
            }
            return bad;
        }
    }
}
