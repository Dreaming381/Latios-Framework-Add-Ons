using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TerrainWaitress.Model
{
    /// <summary>
    /// A terrain as a command sees it: its data, and the Terrain that places it when one was named. Coordinates are world
    /// metres for a placed terrain and metres from the asset's corner otherwise.
    /// </summary>
    public sealed class Target
    {
        public TerrainRecord Data;
        public Placement Placement;

        public bool World => Placement != null;
        public double OX => Placement?.Position[0] ?? 0;
        public double OY => Placement?.Position[1] ?? 0;
        public double OZ => Placement?.Position[2] ?? 0;
        public double SX => Data.Size[0];
        public double SY => Data.Size[1];
        public double SZ => Data.Size[2];

        public string Label => Placement != null ? Placement.HierarchyPath : Data.Name;
        public string Frame => World ? "world" : "local";

        public (float u, float v) UV(double x, double z) => ((float)((x - OX) / SX), (float)((z - OZ) / SZ));
        public double X(double u) => OX + u * SX;
        public double Z(double v) => OZ + v * SZ;

        public bool Contains(double x, double z, double eps = 1e-6) =>
            x >= OX - eps && x <= OX + SX + eps && z >= OZ - eps && z <= OZ + SZ + eps;

        /// <summary>Ground height in this target's frame at (x, z).</summary>
        public double Height(double x, double z)
        {
            var (u, v) = UV(x, z);
            return OY + Data.HeightAt(u, v);
        }

        public override string ToString() => Label;
    }

    /// <summary>
    /// An area on a terrain: all, a rectangle a,b..c,d, or a circle x,z~r, each with an optional +f metres of falloff;
    /// several joined with ';'; and conditions after '|' (slope>30, y<12) that keep only the ground matching them.
    /// </summary>
    public sealed class Region
    {
        public bool All, Circle;
        public double X0, Z0, X1, Z1, CX, CZ, R, Falloff;
        public double AX, AZ, BX, BZ; // a rectangle's corners as given
        public bool HasConditions => m_Conditions != null;
        public string Text;
        List<Region> m_Parts;
        List<Region> m_Excluded;
        List<(string what, char op, double value)> m_Conditions;
        Target m_Target;

        public static bool LooksLike(string s)
        {
            var shapes = s.Split('|')[0];
            return shapes.Split(';').All(p => p.TrimStart('!') is var q && (q == "all" || q.Contains("..") || q.Contains('~')));
        }

        public static Region Parse(string text, Target t)
        {
            var bar = text.IndexOf('|');
            var shapes = (bar < 0 ? text : text.Substring(0, bar)).Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (shapes.Length == 0)
                throw new WaitressException($"'{text}' names no area");
            var excluded = shapes.Where(x => x.StartsWith("!", StringComparison.Ordinal)).Select(x => ParseOne(x.Substring(1), t)).ToList();
            var parts = shapes.Where(x => !x.StartsWith("!", StringComparison.Ordinal)).Select(x => ParseOne(x, t)).ToList();
            if (parts.Count == 0)
                parts.Add(ParseOne("all", t));
            var r = parts.Count == 1 && excluded.Count == 0 ? parts[0] : new Region
            {
                m_Parts = parts, All = parts.Any(x => x.All), Falloff = parts.Max(x => x.Falloff),
                X0 = parts.Min(x => x.X0), Z0 = parts.Min(x => x.Z0), X1 = parts.Max(x => x.X1), Z1 = parts.Max(x => x.Z1),
            };
            r.Text = text;
            r.m_Target = t;
            if (excluded.Count > 0)
                r.m_Excluded = excluded;
            if (bar >= 0)
            {
                r.m_Conditions = new List<(string, char, double)>();
                foreach (var c in text.Substring(bar + 1).Split('|', StringSplitOptions.RemoveEmptyEntries))
                {
                    var at = c.IndexOfAny(new[] { '<', '>' });
                    var what = at < 0 ? null : c.Substring(0, at).Trim();
                    var layer = what != null && what.StartsWith("layer[", StringComparison.Ordinal) && what.EndsWith("]", StringComparison.Ordinal);
                    if (at < 0 || !(what is "slope" or "y" || layer) || !double.TryParse(c.Substring(at + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                        throw new WaitressException($"'{c}' in '{text}' is not a condition; use slope>30, slope<5 (degrees), y>40 or y<12 (height{(t != null && t.World ? ", world" : "")}), layer[Road]<0.5 (weight)");
                    if (layer && t != null)
                    {
                        var name = what.Substring(6, what.Length - 7);
                        var index = t.Data.Layers.FindIndex(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase));
                        if (index < 0)
                            throw new WaitressException($"'{c}' in '{text}': {t.Data.Name} has no layer {name}; it has {string.Join(", ", t.Data.Layers.Select(l => l.Name))}");
                        what = "layer" + index;
                    }
                    r.m_Conditions.Add((what, c[at], v));
                }
            }
            return r;
        }

        static Region ParseOne(string text, Target t)
        {
            var r = new Region { Text = text };
            var body = text;
            var plus = text.LastIndexOf('+');
            if (plus > 0 && text[plus - 1] != ',' && text[plus - 1] != 'e' && text[plus - 1] != 'E')
            {
                r.Falloff = Num(text.Substring(plus + 1), "falloff", null, 0);
                if (r.Falloff < 0)
                    throw new WaitressException($"falloff in '{text}' must be positive");
                body = text.Substring(0, plus);
            }
            if (body == "all")
            {
                r.All = true;
                if (t != null)
                {
                    r.X0 = t.OX;
                    r.Z0 = t.OZ;
                    r.X1 = t.OX + t.SX;
                    r.Z1 = t.OZ + t.SZ;
                }
                return r;
            }
            var tilde = body.IndexOf('~');
            if (tilde > 0)
            {
                r.Circle = true;
                (r.CX, r.CZ) = Point(body.Substring(0, tilde), t);
                r.R = Num(body.Substring(tilde + 1), "radius", null, 0);
                if (r.R <= 0)
                    throw new WaitressException($"'{text}': the radius must be above 0");
                r.X0 = r.CX - r.R;
                r.X1 = r.CX + r.R;
                r.Z0 = r.CZ - r.R;
                r.Z1 = r.CZ + r.R;
                return r;
            }
            var dots = body.IndexOf("..", StringComparison.Ordinal);
            if (dots < 0)
                throw new WaitressException($"'{text}' is not an area: use all, x0,z0..x1,z1 (a rectangle) or x,z~r (a circle), with +f for f metres of falloff");
            var (ax, az) = Point(body.Substring(0, dots), t);
            var (bx, bz) = Point(body.Substring(dots + 2), t);
            (r.AX, r.AZ, r.BX, r.BZ) = (ax, az, bx, bz);
            r.X0 = Math.Min(ax, bx);
            r.X1 = Math.Max(ax, bx);
            r.Z0 = Math.Min(az, bz);
            r.Z1 = Math.Max(az, bz);
            return r;
        }

        /// <summary>x,z in metres; a component ending in % is that fraction of the terrain's size from its corner.</summary>
        public static (double x, double z) Point(string text, Target t)
        {
            var parts = text.Split(',');
            if (parts.Length != 2)
                throw new WaitressException($"'{text}' is not a point; use x,z (metres{(t == null ? "" : t.World ? ", world" : ", from the terrain's corner")}) or 50%,50%");
            return (Num(parts[0], "x", t, 0), Num(parts[1], "z", t, 2));
        }

        static double Num(string s, string what, Target t, int axis)
        {
            s = s.Trim();
            if (s.EndsWith("%", StringComparison.Ordinal))
            {
                if (t == null)
                    throw new WaitressException($"'{s}': percentages need one terrain (name a terrain, not a scene)");
                if (!double.TryParse(s.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var p))
                    throw new WaitressException($"'{s}' is not a number for {what}");
                return (axis == 0 ? t.OX : t.OZ) + p / 100 * (axis == 0 ? t.SX : t.SZ);
            }
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                throw new WaitressException($"'{s}' is not a number for {what}");
            return d;
        }

        /// <summary>1 inside, easing to 0 across the falloff, 0 beyond or where a condition fails.</summary>
        public double Weight(double x, double z)
        {
            var w = m_Parts == null ? Shape(x, z) : m_Parts.Max(p => p.Shape(x, z));
            if (m_Excluded != null && w > 0)
                w *= 1 - m_Excluded.Max(p => p.Shape(x, z));
            if (w <= 0 || m_Conditions == null || m_Target == null)
                return w;
            var (u, v) = m_Target.UV(x, z);
            foreach (var (what, op, value) in m_Conditions)
            {
                var have = what == "slope" ? m_Target.Data.SlopeAt(u, v) : what == "y" ? m_Target.OY + m_Target.Data.HeightAt(u, v)
                    : m_Target.Data.WeightsAt(u, v)[int.Parse(what.Substring(5), CultureInfo.InvariantCulture)];
                if (op == '>' ? have <= value : have >= value)
                    return 0;
            }
            return w;
        }

        double Shape(double x, double z)
        {
            if (All)
                return 1;
            double d;
            if (Circle)
                d = Math.Sqrt((x - CX) * (x - CX) + (z - CZ) * (z - CZ)) - R;
            else
            {
                var dx = Math.Max(Math.Max(X0 - x, x - X1), 0);
                var dz = Math.Max(Math.Max(Z0 - z, z - Z1), 0);
                d = Math.Sqrt(dx * dx + dz * dz);
            }
            if (d <= 1e-9)
                return 1;
            if (Falloff <= 0 || d >= Falloff)
                return 0;
            var s = 1 - d / Falloff;
            return s * s * (3 - 2 * s);
        }

        public bool Inside(double x, double z) => Weight(x, z) >= 1;

        public double MinX => X0 - Falloff;
        public double MaxX => X1 + Falloff;
        public double MinZ => Z0 - Falloff;
        public double MaxZ => Z1 + Falloff;

        public bool Overlaps(Target t) => All || MaxX >= t.OX && MinX <= t.OX + t.SX && MaxZ >= t.OZ && MinZ <= t.OZ + t.SZ;

        /// <summary>Grid indices i whose position origin + i*step falls in [lo, hi], clamped to [0, n-1].</summary>
        public static (int from, int to) Span(double origin, double step, int n, double lo, double hi)
        {
            var a = (int)Math.Ceiling((lo - origin) / step - 1e-9);
            var b = (int)Math.Floor((hi - origin) / step + 1e-9);
            return (Math.Max(a, 0), Math.Min(b, n - 1));
        }

        public override string ToString() => Text;
    }
}
