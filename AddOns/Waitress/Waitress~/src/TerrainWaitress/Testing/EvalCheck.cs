using System;
using System.Globalization;
using System.IO;
using System.Linq;
using TerrainWaitress.Commands;

namespace TerrainWaitress.Testing
{
    /// <summary>
    /// Compares offline evaluation with Unity's own answers, written by AgentScripts/TerrainWaitress/EvalOracle.cs as
    /// tab-separated lines: asset path, u, v, GetInterpolatedHeight, GetInterpolatedNormal x y z, GetSteepness.
    /// </summary>
    public static class EvalCheck
    {
        public static int Run(Session s, string tsv, TextWriter w)
        {
            var inv = CultureInfo.InvariantCulture;
            int n = 0, badH = 0, badN = 0, badS = 0;
            double worstH = 0, worstN = 0, worstS = 0;
            var cache = new System.Collections.Generic.Dictionary<string, Model.TerrainRecord>();
            foreach (var line in File.ReadLines(tsv))
            {
                var f = line.Split('\t');
                if (f.Length < 8)
                    continue;
                if (!cache.TryGetValue(f[0], out var r))
                    cache[f[0]] = r = s.Record(s.Catalog.TryAsset(f[0]) ?? throw new WaitressException("no " + f[0]));
                var u = float.Parse(f[1], inv);
                var v = float.Parse(f[2], inv);
                var h = r.HeightAt(u, v);
                var nn = r.NormalAt(u, v);
                var st = r.SlopeAt(u, v);
                var eh = Math.Abs(h - double.Parse(f[3], inv));
                var en = Math.Sqrt(Math.Pow(nn.x - double.Parse(f[4], inv), 2) + Math.Pow(nn.y - double.Parse(f[5], inv), 2) + Math.Pow(nn.z - double.Parse(f[6], inv), 2));
                var es = Math.Abs(st - double.Parse(f[7], inv));
                n++;
                if (eh > 1e-4) badH++;
                if (en > 1e-5) badN++;
                if (es > 0.05) badS++;
                worstH = Math.Max(worstH, eh);
                worstN = Math.Max(worstN, en);
                worstS = Math.Max(worstS, es);
            }
            w.WriteLine($"{n} samples: height {n - badH}/{n} within 0.1 mm (worst {worstH:G3} m), normal {n - badN}/{n} within 1e-5 (worst {worstN:G3}), steepness {n - badS}/{n} within 0.05° (worst {worstS:G3}°)");
            return badH + badN + badS == 0 ? 0 : 1;
        }
    }
}
