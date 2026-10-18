using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClipWaitress.Model;

namespace ClipWaitress.Commands
{
    /// <summary>Commands that read one clip: the summary, its curves, and their values at given times.</summary>
    public static class Read
    {
        static readonly Dictionary<string, string> s_Quality = new Dictionary<string, string>
        {
            ["loop"] = "pose", ["loopOrientation"] = "rotation", ["loopPositionY"] = "height (Y)", ["loopPositionXZ"] = "position (XZ)",
        };

        public static int Info(Session session, Args args, TextWriter w)
        {
            var all = args.Flag("all");
            args.RejectUnknown();
            var clips = args.Positional.Count == 0 ? throw new WaitressException("missing a clip (an .anim path, Model.fbx#Clip, or a unique clip name)")
                : args.Positional.SelectMany(session.Clips).ToList();
            foreach (var c in clips)
            {
                if (clips.Count > 1 && c != clips[0])
                    w.WriteLine();
                Info(c, all, w);
            }
            return 0;
        }

        static void Info(ClipRecord c, bool all, TextWriter w)
        {
            w.WriteLine(c.Ref + (c.Asset.Stale ? "  (stale: changed since export)" : ""));
            var kind = c.Legacy ? "legacy (Animation component)" : c.Human ? "humanoid" : "generic";
            var size = c.Stats.TryGetValue("size", out var bytes) ? ", " + Fmt.Bytes(bytes) : "";
            w.WriteLine($"  {Fmt.Num(c.Length)}s, {Fmt.Plural((long)Math.Round(c.Frames), "frame")}{(Math.Abs(c.Frames - Math.Round(c.Frames)) > 1e-3 ? $" ({Fmt.Num(c.Frames)})" : "")} at {Fmt.Num(c.Fps)} fps; {kind}; {Fmt.Plural(c.Curves.Count + c.Objects.Count, "curve")} ({Summary(c)}), {Fmt.Plural(c.Keys, "key")}{size}" +
                        (c.Asset.Kind == "model" ? "; imported from the model (read-only here; `modelwaitress set` changes its import settings)" : ""));
            var loop = $"loopTime {OnOff(c, "loopTime")}, loopBlend (Loop Pose) {OnOff(c, "loopBlend")}";
            if (c.On("cycleOffset") || c.Get("cycleOffset") is string co && co != "0")
                loop += $", cycleOffset {c.Get("cycleOffset")}";
            if (c.Quality != null)
            {
                var keys = c.Human ? new[] { "loop", "loopOrientation", "loopPositionY", "loopPositionXZ" } : new[] { "loopOrientation", "loopPositionY", "loopPositionXZ" };
                loop += "; Unity's loop match: " + string.Join(", ", keys.Where(c.Quality.ContainsKey).Select(k => $"{s_Quality[k]} {Fmt.Num(c.Quality[k])} {Analysis.Lamp(c.Quality[k])}"));
                if (c.Quality.TryGetValue("loopPositionXZ", out var xz) && xz < 0.33 && c.Speed > 0.05)
                    loop += " (red XZ means the clip travels, which is right for a clip that moves the character)";
            }
            w.WriteLine("  loop      " + loop);
            if (c.Human || c.GenericRoot || c.MotionCurves)
            {
                var bake = $"bake into pose: rotation {OnOff(c, "loopBlendOrientation")}, Y {OnOff(c, "loopBlendPositionY")}, XZ {OnOff(c, "loopBlendPositionXZ")}";
                w.WriteLine($"  root      average velocity {Fmt.Vec(c.AverageSpeed)} m/s ({Fmt.Num(c.Speed)} m/s), turning {Fmt.Num(c.AverageAngularSpeed)}°/s; {bake}" +
                            (c.Human ? "" : c.GenericRoot ? " (root motion node set)" : " (motion curves)"));
                // Baked axes hide their motion from the velocity, so show where the root curve itself goes.
                var rootT = Bindings.Ordered(c.Curves.Where(x => x.Class is "root" or "motion" && Bindings.Group(x) is "RootT" or "MotionT"));
                if (rootT.Count == 3 && (c.On("loopBlendPositionXZ") || c.On("loopBlendPositionY")))
                {
                    var a = rootT.Select(x => x.Evaluate(c.StartTime)).ToArray();
                    var b = rootT.Select(x => x.Evaluate(c.StopTime)).ToArray();
                    var xz = Math.Sqrt(Math.Pow(b[0] - a[0], 2) + Math.Pow(b[2] - a[2], 2));
                    w.WriteLine($"            before baking, {Bindings.Group(rootT[0])} goes {Fmt.Vec(a)} -> {Fmt.Vec(b)} ({Fmt.Num(xz)} in XZ over the clip" +
                                (xz < 0.05 ? "; it stays in place, so no setting makes it travel)" : $"; un-baking XZ would make it travel about {Fmt.Num(xz / Math.Max(1e-6, c.StopTime - c.StartTime))}/s)"));
                }
            }
            var changed = c.Settings.Where(s => s.Changed && s.Name != "stopTime" || s.Name == "stopTime" && Math.Abs(c.StopTime - c.Length) > 1e-4).ToList();
            if (changed.Count > 0)
                w.WriteLine("  changed   " + string.Join(", ", changed.Select(s => $"{s.Name}={Show(s.Value)}")) + "  (from Unity's defaults)");
            if (c.Events.Count > 0)
            {
                var shown = all ? c.Events : c.Events.Take(8).ToList();
                w.WriteLine("  events    " + string.Join("; ", shown.Select(e => $"{Fmt.Time(e.Time, c.Fps)} {e.Function}({e.Param}){(e.Optional ? " optional" : "")}")) +
                            (shown.Count < c.Events.Count ? $"; +{c.Events.Count - shown.Count} more (--all)" : ""));
            }
            if (c.EulerOrder.Count > 0)
                w.WriteLine($"  note      euler rotation curves on {Fmt.Plural(c.EulerOrder.Count, "path")} play in {string.Join("/", c.EulerOrder.Values.Distinct().Select(o => ClipRecord.OrderNames[o]))} order, not Unity's ZXY, so their values aren't the Transform's rotation (`sample` converts)");
            var warnings = Analysis.Warnings(c);
            if (all && c.On("loopTime"))
            {
                var gaps = Analysis.LoopGaps(c);
                if (gaps.Count > 3)
                {
                    warnings.RemoveAll(x => x.StartsWith("loopTime is on but", StringComparison.Ordinal));
                    warnings.Add($"loopTime is on but {Fmt.Plural(gaps.Count, "curve group")} end away from where they start:");
                    warnings.AddRange(gaps.Select(g => $"  {g.name} {g.start} -> {g.end}"));
                }
            }
            foreach (var x in warnings)
                w.WriteLine("  warning   " + x);
        }

        static string OnOff(ClipRecord c, string setting) => c.On(setting) ? "on" : "off";

        public static string Show(string v) => v == null ? "?" : v.Length == 0 ? "\"\"" : double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) && v.Length > 6 ? Fmt.Num(d) : v;

        /// <summary>What the curves animate, by kind.</summary>
        public static string Summary(ClipRecord c)
        {
            var parts = new List<string>();
            var transforms = c.Curves.Where(x => x.Class is "pos" or "euler" or "quat" or "scale").Select(x => x.Path).Distinct().Count();
            if (transforms > 0)
            {
                var rotation = c.Curves.Any(x => x.Class == "euler") ? c.Curves.Any(x => x.Class == "quat") ? "euler and quaternion rotation" : "euler rotation" : c.Curves.Any(x => x.Class == "quat") ? "quaternion rotation" : null;
                parts.Add(Fmt.Plural(transforms, "transform") + (rotation != null ? ", " + rotation : ""));
            }
            var muscles = c.Curves.Count(x => x.Class == "muscle");
            if (muscles > 0)
                parts.Add(Fmt.Plural(muscles, "muscle"));
            if (c.Curves.Any(x => x.Class == "root"))
                parts.Add("body root");
            if (c.Curves.Any(x => x.Class == "motion"))
                parts.Add("motion");
            var ik = c.Curves.Where(x => x.Class == "ik").Select(Bindings.Group).Distinct().Count();
            if (ik > 0)
                parts.Add(Fmt.Plural(ik, "IK goal curve"));
            var parameters = c.Curves.Where(x => x.Class == "param").Select(x => x.Prop).ToList();
            if (parameters.Count > 0)
                parts.Add("Animator parameters " + string.Join(", ", parameters.Take(4)) + (parameters.Count > 4 ? $" +{parameters.Count - 4}" : ""));
            var active = c.Curves.Count(x => x.Class == "active");
            if (active > 0)
                parts.Add(Fmt.Plural(active, "active toggle"));
            var props = c.Curves.Where(x => x.Class == "prop").Select(Bindings.Group).Distinct().ToList();
            if (props.Count > 0)
                parts.Add(string.Join(", ", props.Select(p => p.Split('.')[0]).GroupBy(t => t).Select(g => g.Count() == 1 ? g.Key + " property" : $"{g.Count()} {g.Key} properties")));
            if (c.Objects.Count > 0)
                parts.Add(string.Join(", ", c.Objects.Select(o => o.Name).Distinct()) + " (object keys)");
            return parts.Count == 0 ? "nothing" : string.Join(", ", parts);
        }

        // ------------------------------------------------------------ curves

        public static int Curves(Session session, Args args, TextWriter w)
        {
            var keys = args.Flag("keys");
            var tangents = args.Flag("tangents");
            keys |= tangents;
            var all = args.Flag("all");
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var c = session.One(args.Positional0("a clip"));
            var filter = args.Positional.Count > 1 ? Bindings.Resolve(args.Positional[1], c.Curves.Select(x => x.Path).Concat(c.Objects.Select(o => o.Path))) : null;
            var groups = Bindings.Groups(c.Curves).Where(g => g.curves.Any(x => Bindings.Matches(filter, x))).ToList();
            var objects = c.Objects.Where(o => Bindings.Matches(filter, o)).ToList();
            w.WriteLine($"{c.Ref}: {Fmt.Plural(c.Curves.Count + c.Objects.Count, "curve")} on {Fmt.Plural(c.Curves.Select(x => x.Path).Concat(c.Objects.Select(o => o.Path)).Distinct().Count(), "path")}, " +
                        $"{Fmt.Plural(c.Keys, "key")}, {Fmt.Num(c.Length)}s at {Fmt.Num(c.Fps)} fps");
            if (groups.Count + objects.Count == 0)
            {
                w.WriteLine(Bindings.NoMatch(c, filter));
                return 1;
            }
            if (filter == null && !keys)
            {
                // One line per path, humanoid curves collapsed into counts.
                var byPath = groups.GroupBy(g => g.path).ToList();
                foreach (var path in byPath.Take(all ? int.MaxValue : 60))
                {
                    var names = new List<string>();
                    var human = path.Where(g => Bindings.IsHuman(g.curves[0].Class)).ToList();
                    if (human.Count > 0 && !all)
                    {
                        var muscles = human.Count(g => g.curves[0].Class == "muscle");
                        if (muscles > 0)
                            names.Add(Fmt.Plural(muscles, "muscle"));
                        names.AddRange(human.Where(g => g.curves[0].Class != "muscle").Select(g => g.group));
                    }
                    names.AddRange(path.Where(g => all || !Bindings.IsHuman(g.curves[0].Class)).Select(g => g.group + (g.curves[0].Class == "quat" ? "(quaternion)" : "")));
                    w.WriteLine($"  {Bindings.Label(path.Key)}  {string.Join("  ", names)}");
                }
                if (byPath.Count > 60 && !all)
                    w.WriteLine($"  +{byPath.Count - 60} more paths (--all, or a filter)");
                foreach (var o in objects)
                    w.WriteLine($"  {Bindings.Label(o.Path)}  {o.Name} ({Fmt.Plural(o.Times.Length, "object key")})");
                if (c.Curves.Any(x => x.Class == "muscle") && !all)
                    w.WriteLine("  (--all lists the muscles; `curves <clip> <filter>` shows values)");
                return 0;
            }
            var limit = all ? int.MaxValue : 60;
            foreach (var (path, group, curves) in groups.Take(limit))
            {
                var shown = curves.Where(x => Bindings.Matches(filter, x)).ToList();
                if (shown.Count == 0)
                    shown = curves;
                if (curves[0].Class == "quat")
                    shown = curves;
                var name = $"{(path.Length == 0 ? "" : path)}:{group}";
                var start = Bindings.Value(shown, c.StartTime);
                var end = Bindings.Value(shown, c.StopTime);
                var ranges = shown[0].Class == "quat" ? "" : "  range " + string.Join(" ", shown.Select(x => (shown.Count > 1 ? Bindings.Name(x).Split('.').Last() + " " : "") + $"{Fmt.Num(x.Min)}..{Fmt.Num(x.Max)}"));
                var count = shown.Max(x => x.Count);
                var note = curves[0].Class == "quat" ? " (quaternion, shown as euler degrees)" : curves[0].Class == "euler" && c.EulerOrder.TryGetValue(path, out var ord) ? $" (euler, plays in {ClipRecord.OrderNames[ord]} order)" : "";
                w.WriteLine($"  {name}{note}  {Fmt.Plural(count, "key")}  start {V(start)}  end {V(end)}{ranges}" +
                            (shown.All(x => x.Constant) ? "  constant" : ""));
                if (!keys)
                    continue;
                foreach (var x in shown)
                {
                    var parts = new List<string>();
                    for (var i = 0; i < x.Count; i++)
                    {
                        var k = $"{Fmt.Num(x.T[i])}:{Fmt.Num(x.V[i])}";
                        if (tangents)
                            k += $"[{Fmt.Num(x.In[i])},{Fmt.Num(x.Out[i])}{(x.Mode != null && x.Mode[i] != 0 ? $" w{Fmt.Num(x.InW[i])},{Fmt.Num(x.OutW[i])}" : "")}]";
                        else if (x.IsStepped(i))
                            k += "|step";
                        parts.Add(k);
                    }
                    w.WriteLine($"    {(x.Class == "quat" ? "quaternion " + x.Prop.Substring(x.Prop.Length - 1) : Bindings.Name(x))}  {string.Join(" ", parts)}");
                }
            }
            if (groups.Count > limit)
                w.WriteLine($"  +{groups.Count - limit} more (--all, or narrow the filter)");
            foreach (var o in objects)
            {
                w.WriteLine($"  {(o.Path.Length == 0 ? "" : o.Path)}:{o.Name}  {Fmt.Plural(o.Times.Length, "object key")}" + (o.Missing > 0 ? $", {o.Missing} missing" : ""));
                if (keys || o.Times.Length <= 8)
                    w.WriteLine("    " + string.Join(" ", o.Times.Select((t, i) => $"{Fmt.Num(t)}:{o.Values[i]}")));
            }
            return 0;
        }

        static string V(double[] v) => v.Length == 1 ? Fmt.Num(v[0]) : Fmt.Vec(v);

        // ------------------------------------------------------------ sample

        public static int Sample(Session session, Args args, TextWriter w)
        {
            var at = args.Get("at");
            var all = args.Flag("all");
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var c = session.One(args.Positional0("a clip"));
            var filter = args.Positional.Count > 1 ? Bindings.Resolve(args.Positional[1], c.Curves.Select(x => x.Path).Concat(c.Objects.Select(o => o.Path))) : null;
            var times = at == null ? new List<double> { c.StartTime, (c.StartTime + c.StopTime) / 2, c.StopTime } : at.Split(',').Select(s => Time(c, s)).ToList();
            var groups = Bindings.Groups(c.Curves).Select(g => (g.path, g.group, curves: g.curves[0].Class == "quat" ? g.curves : g.curves.Where(x => Bindings.Matches(filter, x)).ToList()))
                                 .Where(g => g.curves.Count > 0 && g.curves.Any(x => Bindings.Matches(filter, x))).ToList();
            var objects = c.Objects.Where(o => Bindings.Matches(filter, o)).ToList();
            if (groups.Count + objects.Count == 0)
            {
                w.WriteLine(Bindings.NoMatch(c, filter));
                return 1;
            }
            w.WriteLine($"{c.Ref}  at {string.Join(", ", times.Select(t => Fmt.Time(t, c.Fps)))}" + (groups.Any(g => g.curves[0].Class is "quat" or "euler") ? "; rotations in degrees" : "") +
                        (groups.Any(g => g.curves[0].Class == "euler" && c.EulerOrder.ContainsKey(g.path)) ? $" (euler curves in {ClipRecord.OrderNames[c.EulerOrder.Values.First()]} order shown as the Transform's rotation)" : ""));
            var outside = times.Where(t => t < -1e-6 || t > c.Length + 1e-6).ToList();
            if (outside.Count > 0)
                w.WriteLine($"  note: {string.Join(", ", outside.Select(t => Fmt.Num(t) + "s"))} {(outside.Count == 1 ? "is" : "are")} outside the clip (0..{Fmt.Num(c.Length)}s); curves hold their first or last value there" +
                            (c.On("loopTime") ? $" (playing, a looping clip would be at {string.Join(", ", outside.Select(t => Fmt.Num(((t % c.Length) + c.Length) % c.Length) + "s"))})" : ""));
            if (c.Human && c.On("mirror"))
                w.WriteLine("  note: mirror is on, so Unity plays Left muscles on the right and Right on the left; these are the curves as stored");
            var rows = new List<(string name, List<string> values)>();
            foreach (var (path, group, curves) in groups)
            {
                var name = (path.Length == 0 ? ":" : path + ":") + (curves.Count == 1 && curves[0].Class != "quat" ? Bindings.Name(curves[0]) : group) + (Bindings.IsQuaternion(curves) && curves[0].Class != "quat" ? " (degrees)" : "");
                var order = curves[0].Class == "euler" && c.EulerOrder.TryGetValue(path, out var o) ? o : 4;
                rows.Add((name, times.Select(t => V(Bindings.Value(curves, t, order))).ToList()));
            }
            foreach (var o in objects)
                rows.Add(((o.Path.Length == 0 ? ":" : o.Path + ":") + o.Name, times.Select(o.At).ToList()));
            var limit = all ? int.MaxValue : 60;
            var width = Math.Min(48, rows.Take(limit).Max(r => r.name.Length));
            foreach (var (name, values) in rows.Take(limit))
                w.WriteLine($"  {name.PadRight(width)}  {string.Join("  ", values)}");
            if (rows.Count > limit)
                w.WriteLine($"  +{rows.Count - limit} more (--all, or a filter)");
            return 0;
        }

        /// <summary>A time in seconds (0.5 or 0.5s), frames (12f), or percent of the clip (50%), or start/end.</summary>
        public static double Time(ClipRecord c, string text)
        {
            text = text.Trim();
            if (text == "start")
                return c.StartTime;
            if (text == "end")
                return c.StopTime;
            if (text.EndsWith("f", StringComparison.OrdinalIgnoreCase) && double.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
                return f / c.Fps;
            if (text.EndsWith("%", StringComparison.Ordinal) && double.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out var p))
                return p / 100 * c.Length;
            if (text.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                text = text.Substring(0, text.Length - 1);
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var s))
                return s;
            throw new WaitressException($"'{text}' isn't a time: seconds (0.5), frames (12f), percent (50%), start or end");
        }

        public static string Suggest(string key, IEnumerable<string> names)
        {
            var best = names.OrderBy(n => Distance(key.ToLowerInvariant(), n.ToLowerInvariant())).FirstOrDefault();
            return best != null && Distance(key.ToLowerInvariant(), best.ToLowerInvariant()) <= Math.Max(2, key.Length / 3) ? $"did you mean '{best}'?" : "";
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
    }
}
