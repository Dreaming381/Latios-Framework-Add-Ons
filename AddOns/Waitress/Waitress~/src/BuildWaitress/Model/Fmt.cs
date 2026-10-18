using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BuildWaitress.Model
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
        public static string Plural(long n, string word, string plural) => Count(n) + " " + (n == 1 ? word : plural);
        public static string Plural(long n, string word) => Count(n) + " " + (n == 1 ? word : word.EndsWith("s", StringComparison.Ordinal) || word.EndsWith("sh", StringComparison.Ordinal) ? word + "es" : word + "s");
        public static string Bytes(double b)
        {
            var a = Math.Abs(b);
            if (a >= 1L << 30)
                return (b / (1L << 30)).ToString("0.##", Inv) + " GB";
            if (a >= 1 << 20)
                return (b / (1 << 20)).ToString(a >= 100 << 20 ? "0" : "0.#", Inv) + " MB";
            if (a >= 1024)
                return (b / 1024).ToString(a >= 10 << 10 ? "0" : "0.#", Inv) + " KB";
            return b.ToString("0", Inv) + " B";
        }

        public static string Signed(double b) => (b > 0 ? "+" : b < 0 ? "-" : "±") + Bytes(Math.Abs(b));
        public static string Time(TimeSpan t) => t.TotalSeconds < 1 ? $"{t.TotalMilliseconds:0} ms" : t.TotalMinutes < 2 ? t.TotalSeconds.ToString("0.#", Inv) + " s" : $"{(int)t.TotalMinutes} min {t.Seconds} s";
    }
}
