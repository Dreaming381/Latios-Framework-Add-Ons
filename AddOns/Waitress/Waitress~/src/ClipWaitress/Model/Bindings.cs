using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClipWaitress.Model
{
    public static class Fmt
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public static string Num(double d)
        {
            if (double.IsNaN(d))
                return "NaN";
            if (double.IsInfinity(d))
                return d > 0 ? "inf" : "-inf";
            if (Math.Abs(d) < 1e-6)
                return "0";
            var r = Math.Round(d, Math.Abs(d) >= 100 ? 1 : 4);
            return r == 0 ? "0" : r.ToString("0.####", Inv);
        }

        public static string Vec(IEnumerable<double> v) => "(" + string.Join(", ", v.Select(Num)) + ")";
        public static string Count(long n) => n.ToString("N0", Inv);
        public static string Plural(long n, string word) => Count(n) + " " + (n == 1 ? word : word.EndsWith("s", StringComparison.Ordinal) || word.EndsWith("sh", StringComparison.Ordinal) ? word + "es" : word + "s");

        /// <summary>A clip time in seconds with its frame: 0.5s (f15).</summary>
        public static string Time(double t, double fps)
        {
            var f = t * fps;
            var frame = Math.Abs(f - Math.Round(f)) < 1e-3 ? Math.Round(f).ToString(Inv) : f.ToString("0.##", Inv);
            return $"{Num(t)}s (f{frame})";
        }

        public static string Bytes(double b) => b >= 1 << 20 ? (b / (1 << 20)).ToString("0.#", Inv) + " MB" : b >= 1024 ? (b / 1024).ToString("0", Inv) + " KB" : b.ToString("0", Inv) + " B";
    }

    /// <summary>How curves are named on screen and in set keys: path:property, with Transform curves grouped.</summary>
    public static class Bindings
    {
        public static bool IsHuman(string cls) => cls is "muscle" or "root" or "ik" or "motion";

        public static string Label(string path) => path.Length == 0 ? "(root)" : path;

        static string Component(string prop)
        {
            var dot = prop.LastIndexOf('.');
            return dot > 0 && prop.Length - dot == 2 ? prop.Substring(dot + 1) : null;
        }

        static string StripComponent(string prop)
        {
            var dot = prop.LastIndexOf('.');
            return dot > 0 && prop.Length - dot == 2 && "xyzwrgba".IndexOf(prop[dot + 1]) >= 0 ? prop.Substring(0, dot) : prop;
        }

        /// <summary>The name of the group a curve belongs to: position, rotation, scale, active, Type.property, or the muscle/parameter.</summary>
        public static string Group(string cls, string type, string prop)
        {
            switch (cls)
            {
                case "pos": return "position";
                case "euler": case "quat": return "rotation";
                case "scale": return "scale";
                case "active": return "active";
                case "muscle": case "param": return prop;
                case "root": case "motion": case "ik": return StripComponent(prop);
            }
            if (prop == "m_Enabled")
                return type + ".enabled";
            return type + "." + StripComponent(prop);
        }

        public static string Group(Curve c) => Group(c.Class, c.Type, c.Prop);

        /// <summary>One curve's name: position.x, rotation.w (quaternion), SkinnedMeshRenderer.blendShape.Smile...</summary>
        public static string Name(Curve c)
        {
            var g = Group(c);
            var comp = Component(c.Prop);
            if (c.Class is "muscle" or "param" || comp == null || g == c.Prop || g.EndsWith(".enabled", StringComparison.Ordinal))
                return g;
            return g + "." + comp;
        }

        /// <summary>Components of a group in order (x y z w, r g b a).</summary>
        public static List<Curve> Ordered(IEnumerable<Curve> group) =>
            group.OrderBy(c => "xyzwrgba".IndexOf(Component(c.Prop)?[0] ?? '?')).ThenBy(c => c.Prop, StringComparer.Ordinal).ToList();

        /// <summary>Curves grouped by path and group, in path order with the root first.</summary>
        public static List<(string path, string group, List<Curve> curves)> Groups(IEnumerable<Curve> curves) =>
            curves.GroupBy(c => (c.Path, Group(c), c.Class == "quat"))
                  .Select(g => (g.Key.Path, g.Key.Item2, Ordered(g)))
                  .OrderBy(g => g.Path, StringComparer.Ordinal).ThenBy(g => Rank(g.Item3[0].Class)).ThenBy(g => g.Item2, StringComparer.Ordinal).ToList();

        static int Rank(string cls) => cls switch { "pos" => 0, "euler" or "quat" => 1, "scale" => 2, "root" => 3, "motion" => 4, "ik" => 5, "muscle" => 6, "param" => 7, "active" => 8, _ => 9 };

        /// <summary>
        /// True when the filter selects the curve: path:property (exact path; property is a group, one component, or a Unity
        /// property name), or otherwise text found in the path or the curve's name.
        /// </summary>
        public static bool Matches(string filter, string path, string name, string group, string prop)
        {
            if (string.IsNullOrEmpty(filter))
                return true;
            var colon = filter.LastIndexOf(':');
            if (colon >= 0)
            {
                var p = filter.Substring(0, colon).Trim().Trim('/');
                var q = filter.Substring(colon + 1).Trim();
                if (!string.Equals(p, path, StringComparison.Ordinal))
                    return false;
                return q.Length == 0 || Eq(q, name) || Eq(q, group) || Eq(q, prop);
            }
            return path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Resolves a filter's path against the clip's paths so it needn't be typed in full: path:property takes an exact
        /// path or a unique path ending in it, and a bare object name selects that object alone rather than everything
        /// whose path contains it.
        /// </summary>
        public static string Resolve(string filter, IEnumerable<string> paths)
        {
            if (string.IsNullOrEmpty(filter))
                return filter;
            var all = paths.Distinct().ToList();
            var colon = filter.LastIndexOf(':');
            if (colon < 0)
            {
                var named = all.Where(p => p.Length > 0 && p.Substring(p.LastIndexOf('/') + 1).Equals(filter, StringComparison.OrdinalIgnoreCase)).ToList();
                return named.Count == 1 ? named[0] + ":" : filter;
            }
            var path = filter.Substring(0, colon).Trim().Trim('/');
            if (all.Contains(path))
                return filter;
            var hits = all.Where(p => p.EndsWith("/" + path, StringComparison.Ordinal)).ToList();
            if (hits.Count == 1)
                return hits[0] + filter.Substring(colon);
            if (hits.Count > 1)
                throw new WaitressException($"'{path}' ends {hits.Count} paths; use more of it:\n  " + string.Join("\n  ", hits.Take(8)));
            return filter;
        }

        /// <summary>Why a filter matched nothing: what the path animates instead, or the paths there are.</summary>
        public static string NoMatch(ClipRecord c, string filter)
        {
            var colon = filter.LastIndexOf(':');
            var paths = c.Curves.Select(x => x.Path).Concat(c.Objects.Select(o => o.Path)).Distinct().ToList();
            if (colon >= 0)
            {
                var path = filter.Substring(0, colon).Trim().Trim('/');
                if (paths.Contains(path))
                {
                    var groups = c.Curves.Where(x => x.Path == path).Select(Group).Concat(c.Objects.Where(o => o.Path == path).Select(o => o.Name)).Distinct();
                    return $"{Label(path)} animates {string.Join(", ", groups)}; '{filter.Substring(colon + 1)}' isn't animated, so the object keeps its own value there";
                }
            }
            var shown = paths.Select(Label).Take(12).ToList();
            return $"no curve matches '{filter}'; paths: {string.Join(", ", shown)}{(paths.Count > shown.Count ? $", +{paths.Count - shown.Count} more (`curves <clip>`)" : "")}";
        }

        public static bool Matches(string filter, Curve c) => Matches(filter, c.Path, Name(c), Group(c), c.Prop);
        public static bool Matches(string filter, ObjectCurve o) => Matches(filter, o.Path, o.Name, o.Name, o.Prop);

        static bool Eq(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A group's value at time t, as shown: rotation in degrees (quaternions, and euler curves in another rotation order,
        /// converted to the Transform's euler angles), others as they are.
        /// </summary>
        public static double[] Value(List<Curve> group, double t, int order = 4)
        {
            var values = group.Select(c => c.Evaluate(t)).ToArray();
            if (IsQuaternion(group) && values.Length == 4)
                return Euler(values[0], values[1], values[2], values[3]);
            if (group[0].Class == "euler" && order != 4 && values.Length == 3)
            {
                var q = Quaternion(values[0], values[1], values[2], order);
                return Euler(q[0], q[1], q[2], q[3]);
            }
            return values;
        }

        /// <summary>Transform rotation quaternions, and the humanoid root, motion and IK goal rotations (RootQ...).</summary>
        public static bool IsQuaternion(List<Curve> group) =>
            group.Count == 4 && (group[0].Class == "quat" || group[0].Class is "root" or "motion" or "ik" && Group(group[0]).EndsWith("Q", StringComparison.Ordinal));

        /// <summary>Unity's euler angles for a quaternion (applied Z, then X, then Y), each in (-180, 180].</summary>
        public static double[] Euler(double x, double y, double z, double w)
        {
            var n = Math.Sqrt(x * x + y * y + z * z + w * w);
            if (n > 0)
            {
                x /= n;
                y /= n;
                z /= n;
                w /= n;
            }
            var sx = 2 * (w * x - y * z);
            double ex, ey, ez;
            if (Math.Abs(sx) >= 0.99999)
            {
                ex = Math.Sign(sx) * Math.PI / 2;
                ey = Math.Atan2(2 * (w * y - x * z), 1 - 2 * (y * y + z * z));
                ez = 0;
            }
            else
            {
                ex = Math.Asin(sx);
                ey = Math.Atan2(2 * (x * z + w * y), 1 - 2 * (x * x + y * y));
                ez = Math.Atan2(2 * (x * y + w * z), 1 - 2 * (x * x + z * z));
            }
            const double deg = 180 / Math.PI;
            return new[] { Wrap(ex * deg), Wrap(ey * deg), Wrap(ez * deg) };
        }

        public static double Wrap(double degrees)
        {
            var d = Math.IEEERemainder(degrees, 360);
            return d <= -180 ? d + 360 : d;
        }

        /// <summary>
        /// The quaternion for euler degrees applied in a RotationOrder (0 XYZ ... 5 ZYX; the first letter turns first).
        /// Unity's default, 4 (ZXY), is Quaternion.Euler.
        /// </summary>
        public static double[] Quaternion(double x, double y, double z, int order)
        {
            const double r = Math.PI / 360;
            var axis = new Dictionary<char, double[]>
            {
                ['X'] = new[] { Math.Sin(x * r), 0, 0, Math.Cos(x * r) },
                ['Y'] = new[] { 0, Math.Sin(y * r), 0, Math.Cos(y * r) },
                ['Z'] = new[] { 0, 0, Math.Sin(z * r), Math.Cos(z * r) },
            };
            var name = ClipRecord.OrderNames[order is >= 0 and <= 5 ? order : 4];
            return Mul(axis[name[2]], Mul(axis[name[1]], axis[name[0]]));
        }

        static double[] Mul(double[] a, double[] b) => new[]
        {
            a[3] * b[0] + a[0] * b[3] + a[1] * b[2] - a[2] * b[1],
            a[3] * b[1] - a[0] * b[2] + a[1] * b[3] + a[2] * b[0],
            a[3] * b[2] + a[0] * b[1] - a[1] * b[0] + a[2] * b[3],
            a[3] * b[3] - a[0] * b[0] - a[1] * b[1] - a[2] * b[2],
        };

        /// <summary>The angle in degrees between two quaternions.</summary>
        public static double Angle(double[] a, double[] b)
        {
            var dot = Math.Abs(a.Zip(b, (p, q) => p * q).Sum()) / Math.Sqrt(a.Sum(v => v * v) * b.Sum(v => v * v));
            return 2 * Math.Acos(Math.Min(1, dot)) * 180 / Math.PI;
        }
    }
}
