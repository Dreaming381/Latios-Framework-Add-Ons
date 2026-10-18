using System;
using System.Collections.Generic;
using System.Linq;

namespace ClipWaitress.Model
{
    /// <summary>Problems a clip's data shows on its own, without knowing what plays it.</summary>
    public static class Analysis
    {
        public static string Lamp(double v) => v < 0.33 ? "red" : v < 0.66 ? "orange" : "green";

        /// <summary>
        /// Curve groups whose value at the end of the clip's range differs from the start, which pops when the clip loops.
        /// Root motion is left to Unity's loop-match values, and Loop Pose (loopBlend) hides gaps, so none are reported then.
        /// Differences under 0.001 (or 0.5°, or 2% of the curve's range) don't count.
        /// </summary>
        public static List<(string name, string start, string end, double size)> LoopGaps(ClipRecord c)
        {
            var gaps = new List<(string, string, string, double)>();
            if (c.On("loopBlend") || c.Legacy)
                return gaps;
            double t0 = c.StartTime, t1 = c.StopTime;
            foreach (var (path, group, curves) in Bindings.Groups(c.Curves))
            {
                var cls = curves[0].Class;
                if (cls is "root" or "motion" || path.Length == 0 && cls is "pos" or "euler" or "quat" && c.GenericRoot)
                    continue;
                if (curves.All(x => x.Constant))
                    continue;
                var a = curves.Select(x => x.Evaluate(t0)).ToArray();
                var b = curves.Select(x => x.Evaluate(t1)).ToArray();
                double size;
                if (Bindings.IsQuaternion(curves))
                    size = Bindings.Angle(a, b) >= 0.5 ? Bindings.Angle(a, b) : 0;
                else if (cls == "euler")
                    size = a.Zip(b, (p, q) => Math.Abs(Bindings.Wrap(q - p))).Max() >= 0.5 ? a.Zip(b, (p, q) => Math.Abs(Bindings.Wrap(q - p))).Max() : 0;
                else
                {
                    size = 0;
                    for (var i = 0; i < curves.Count; i++)
                    {
                        var range = curves[i].Max - curves[i].Min;
                        var d = Math.Abs(b[i] - a[i]);
                        if (d > 1e-3 && d > 0.02 * range)
                            size = Math.Max(size, range > 0 ? d / range * 100 : 100);
                    }
                }
                if (size <= 0)
                    continue;
                var shownA = Bindings.Value(curves, t0);
                var shownB = Bindings.Value(curves, t1);
                gaps.Add((path.Length == 0 ? ":" + group : path + ":" + group, Show(shownA), Show(shownB), size));
            }
            return gaps.OrderByDescending(g => g.Item4).ToList();
        }

        static string Show(double[] v) => v.Length == 1 ? Fmt.Num(v[0]) : Fmt.Vec(v);

        public static List<string> Warnings(ClipRecord c)
        {
            var list = new List<string>();
            if (c.Empty || c.Curves.Count + c.Objects.Count == 0)
                list.Add("the clip has no curves, so it changes nothing");
            foreach (var e in c.Events)
                if (e.Time < -1e-5 || e.Time > c.Length + 1e-4)
                    list.Add($"event {e.Function} at {Fmt.Num(e.Time)}s is outside the clip (0..{Fmt.Num(c.Length)}s), so it never fires");
            var nan = c.Curves.Where(x => x.V.Any(v => double.IsNaN(v) || double.IsInfinity(v))).ToList();
            if (nan.Count > 0)
                list.Add($"{Fmt.Plural(nan.Count, "curve")} have NaN or infinite values: {string.Join(", ", nan.Take(4).Select(Ref))}");
            var duplicate = c.Curves.Where(x => Enumerable.Range(1, Math.Max(0, x.Count - 1)).Any(i => x.T[i] - x.T[i - 1] < 1e-6)).ToList();
            if (duplicate.Count > 0)
                list.Add($"{Fmt.Plural(duplicate.Count, "curve")} have two keys at the same time: {string.Join(", ", duplicate.Take(4).Select(Ref))}");
            var flips = new List<string>();
            foreach (var x in c.Curves.Where(x => x.Class == "euler"))
                for (var i = 1; i < x.Count; i++)
                    if (Math.Abs(x.V[i] - x.V[i - 1]) > 180)
                    {
                        flips.Add($"{Ref(x)} {Fmt.Num(x.V[i - 1])}° -> {Fmt.Num(x.V[i])}° at {Fmt.Num(x.T[i])}s");
                        break;
                    }
            if (flips.Count > 0)
                list.Add($"{Fmt.Plural(flips.Count, "euler rotation curve")} {(flips.Count == 1 ? "jumps" : "jump")} more than 180° between keys, which spins the long way round: {string.Join("; ", flips.Take(3))}");
            var hemisphere = new List<string>();
            foreach (var (path, group, curves) in Bindings.Groups(c.Curves).Where(g => Bindings.IsQuaternion(g.curves)))
            {
                if (curves.Any(x => x.Count != curves[0].Count))
                    continue;
                for (var i = 1; i < curves[0].Count; i++)
                    if (curves.Sum(x => x.V[i] * x.V[i - 1]) < 0)
                    {
                        hemisphere.Add($"{(path.Length == 0 ? "" : path)}:{group} at {Fmt.Num(curves[0].T[i])}s");
                        break;
                    }
            }
            if (hemisphere.Count > 0)
                list.Add($"{Fmt.Plural(hemisphere.Count, "quaternion rotation")} {(hemisphere.Count == 1 ? "flips" : "flip")} sign between keys, which turns the long way round: {string.Join("; ", hemisphere.Take(3))}");
            var duplicates = c.Events.GroupBy(e => (Math.Round(e.Time, 4), e.Function, e.Param)).Where(g => g.Count() > 1).ToList();
            if (duplicates.Count > 0)
                list.Add($"events fire twice at the same time: {string.Join(", ", duplicates.Select(g => $"{g.Key.Function} at {Fmt.Num(g.Key.Item1)}s"))}");
            if (c.Legacy && c.Curves.Any(x => Bindings.IsHuman(x.Class)))
                list.Add("the clip is Legacy but has humanoid curves (muscles, root, IK), which only an Animator with a humanoid Avatar plays");
            var missing = c.Objects.Where(o => o.Missing > 0).ToList();
            if (missing.Count > 0)
                list.Add($"{Fmt.Plural(missing.Sum(o => o.Missing), "object key")} point at assets that no longer exist ({string.Join(", ", missing.Select(o => Bindings.Label(o.Path) + ":" + o.Name))})");
            if (!c.Human && c.On("mirror"))
                list.Add("mirror is on, but only humanoid clips can be mirrored, so it does nothing");
            if (c.Legacy)
            {
            }
            else if (!c.On("loopTime"))
            {
                if (c.Get("cycleOffset") is string offset && offset != "0")
                    list.Add("cycleOffset is set but loopTime is off; it only applies to looping clips (the Inspector disables it)");
                var loopOnly = new[] { "loopBlend", "loopBlendOrientation", "loopBlendPositionY", "loopBlendPositionXZ" }.Where(c.On).ToList();
                if (loopOnly.Contains("loopBlend"))
                    list.Add("loopBlend (Loop Pose) is on but loopTime is off, so it does nothing");
            }
            else
            {
                var gaps = LoopGaps(c);
                if (gaps.Count > 0)
                    list.Add($"loopTime is on but {Fmt.Plural(gaps.Count, "curve group")} {(gaps.Count == 1 ? "ends away from where it starts" : "end away from where they start")}, so the loop pops: " +
                             string.Join("; ", gaps.Take(3).Select(g => $"{g.name} {g.start} -> {g.end}")) + (gaps.Count > 3 ? $"; +{gaps.Count - 3} more (`info --all`)" : ""));
            }
            if (c.Settings.Any(s => s.Name == "stopTime") && c.StopTime < c.Length - 1e-4 && c.Asset.Kind == "anim")
                list.Add($"stopTime {Fmt.Num(c.StopTime)}s is before the clip's last key ({Fmt.Num(c.Length)}s); root motion and loop settings use that range");
            return list;
        }

        static string Ref(Curve x) => (x.Path.Length == 0 ? "" : x.Path) + ":" + Bindings.Name(x);
    }
}
