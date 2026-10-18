using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TerrainWaitress.Model
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
            var r = Math.Round(d, Math.Abs(d) >= 1000 ? 1 : Math.Abs(d) >= 1 ? 3 : 4);
            return r == 0 ? "0" : r.ToString("0.####", Inv);
        }

        /// <summary>Metres to the centimetre.</summary>
        public static string M(double d) => Math.Abs(d) < 0.005 ? "0" : Math.Round(d, 2).ToString("0.##", Inv);

        public static string Deg(double d) => Math.Round(d, 1).ToString("0.#", Inv) + "°";
        public static string Pct(double fraction) => fraction <= 0 ? "0%" : fraction < 0.001 ? "<0.1%" : (fraction * 100).ToString(fraction < 0.1 ? "0.#" : "0", Inv) + "%";
        public static string Vec(IEnumerable<double> v) => "(" + string.Join(", ", v.Select(M)) + ")";
        public static string XZ(double x, double z) => $"({M(x)}, {M(z)})";
        public static string Count(long n) => n.ToString("N0", Inv);
        public static string Plural(long n, string word) => Count(n) + " " + (n == 1 ? word : word.EndsWith("s", StringComparison.Ordinal) || word.EndsWith("sh", StringComparison.Ordinal) ? word + "es" : word + "s");
        public static string Bytes(double b) => b >= 1 << 20 ? (b / (1 << 20)).ToString("0.#", Inv) + " MB" : b >= 1024 ? (b / 1024).ToString("0", Inv) + " KB" : b.ToString("0", Inv) + " B";
    }
}
