using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClipWaitress.Commands;
using ClipWaitress.Model;

namespace ClipWaitress.Testing
{
    /// <summary>
    /// Compares the tool's curve evaluation with Unity's answers written by AgentScripts/ClipWaitress/EvalOracle.cs: C lines
    /// hold AnimationCurve.Evaluate, P lines the pose AnimationClip.SampleAnimation gives a built hierarchy.
    /// </summary>
    public static class EvalCheck
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public static int Run(Session session, string file, TextWriter w)
        {
            var records = new Dictionary<string, AssetRecord>();
            ClipRecord Clip(string path, string id)
            {
                if (!records.TryGetValue(path, out var r))
                    records[path] = r = session.Catalog.Get(session.Catalog.TryAsset(path), session.Notes);
                return r.Clips.First(c => c.Id == id);
            }
            int curves = 0, curveBad = 0, poses = 0, poseBad = 0;
            var worst = new List<(double err, string line)>();
            foreach (var line in File.ReadLines(file))
            {
                var f = line.Split('\t');
                if (f[0] == "C")
                {
                    var c = Clip(f[1], f[2]).Curves.FirstOrDefault(x => x.Path == f[3] && x.Type == f[4] && x.Prop == f[5]);
                    curves++;
                    if (c == null)
                    {
                        curveBad++;
                        worst.Add((double.PositiveInfinity, "no curve: " + line));
                        continue;
                    }
                    var t = double.Parse(f[6], Inv);
                    var expected = double.Parse(f[7], Inv);
                    var got = c.Evaluate(t);
                    var err = Math.Abs(got - expected) / Math.Max(1, Math.Abs(expected));
                    if (err > 1e-4 || double.IsNaN(got) != double.IsNaN(expected))
                    {
                        curveBad++;
                        worst.Add((err, $"{line} got {got.ToString("R", Inv)}"));
                    }
                    continue;
                }
                if (f[0] != "P")
                    continue;
                poses++;
                var clip = Clip(f[1], f[2]);
                var path = f[3];
                var time = double.Parse(f[4], Inv);
                var unity = f[5].Split(',').Select(s => double.Parse(s, Inv)).ToArray();
                var mine = Pose(clip, path, time);
                var pos = Math.Sqrt(Enumerable.Range(0, 3).Sum(i => Math.Pow(mine[i] - unity[i], 2)));
                var angle = Bindings.Angle(mine.Skip(3).Take(4).ToArray(), unity.Skip(3).Take(4).ToArray());
                var scale = Enumerable.Range(7, 3).Max(i => Math.Abs(mine[i] - unity[i]));
                if (pos > 1e-3 * Math.Max(1, Math.Sqrt(unity.Take(3).Sum(v => v * v))) || angle > 0.05 || scale > 1e-3)
                {
                    poseBad++;
                    worst.Add((Math.Max(pos, Math.Max(angle / 10, scale)), $"{line} got {string.Join(",", mine.Select(v => v.ToString("0.#####", Inv)))} (pos {pos:0.#####} angle {angle:0.###}° scale {scale:0.#####})"));
                }
            }
            w.WriteLine($"curves: {curves - curveBad} of {curves} samples match AnimationCurve.Evaluate");
            w.WriteLine($"poses: {poses - poseBad} of {poses} transform samples match AnimationClip.SampleAnimation");
            foreach (var (_, line) in worst.OrderByDescending(x => x.err).Take(12))
                w.WriteLine("  " + line);
            return curveBad + poseBad == 0 ? 0 : 1;
        }

        // Position, rotation quaternion and scale of one path at time t; components without a curve keep the defaults.
        static double[] Pose(ClipRecord c, string path, double t)
        {
            var on = c.Curves.Where(x => x.Path == path && x.Type == "Transform").ToList();
            double V(string prop, double fallback) => on.FirstOrDefault(x => x.Prop == prop)?.Evaluate(t) ?? fallback;
            var p = new[] { V("m_LocalPosition.x", 0), V("m_LocalPosition.y", 0), V("m_LocalPosition.z", 0) };
            var s = new[] { V("m_LocalScale.x", 1), V("m_LocalScale.y", 1), V("m_LocalScale.z", 1) };
            double[] q;
            if (on.Any(x => x.Class == "quat"))
            {
                q = new[] { V("m_LocalRotation.x", 0), V("m_LocalRotation.y", 0), V("m_LocalRotation.z", 0), V("m_LocalRotation.w", 1) };
            }
            else
            {
                var e = on.Where(x => x.Class == "euler").Select(x => x.Prop.Substring(0, x.Prop.Length - 2)).FirstOrDefault() ?? "localEulerAnglesRaw";
                q = Bindings.Quaternion(V(e + ".x", 0), V(e + ".y", 0), V(e + ".z", 0), c.EulerOrder.TryGetValue(path, out var order) ? order : 4);
            }
            return p.Concat(q).Concat(s).ToArray();
        }
    }
}
