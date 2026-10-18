using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ProfilerWaitress.Text
{
    public static class Format
    {
        static readonly CultureInfo s_Inv = CultureInfo.InvariantCulture;

        /// <summary>Milliseconds with precision that shrinks as values grow, so columns stay narrow.</summary>
        public static string Ms(double ns)
        {
            var ms = ns / 1e6;
            var abs = Math.Abs(ms);
            if (abs == 0)
                return "0";
            if (abs < 0.0005)
                return ms < 0 ? "-<.001" : "<.001";
            if (abs < 10)
                return ms.ToString("0.000", s_Inv);
            if (abs < 100)
                return ms.ToString("0.00", s_Inv);
            return ms.ToString("0.0", s_Inv);
        }

        /// <summary>A per-frame count: two decimals below 10, one below 100, none above, so columns stay consistent.</summary>
        public static string PerFrame(double v) =>
            v.ToString(Math.Abs(v) < 10 ? "0.00" : Math.Abs(v) < 100 ? "0.0" : "0", s_Inv);

        public static string SignedMs(double ns) => (ns > 0 ? "+" : "") + Ms(ns);

        public static string Num(double v, int decimals = 1)
        {
            if (v == Math.Floor(v) && Math.Abs(v) < 1e15)
                return ((long)v).ToString(s_Inv);
            return v.ToString("F" + decimals, s_Inv);
        }

        public static string Bytes(double b)
        {
            var abs = Math.Abs(b);
            if (abs < 1024)
                return Num(b, 0) + "B";
            if (abs < 1024 * 1024)
                return (b / 1024).ToString(abs < 10240 ? "0.0" : "0", s_Inv) + "KB";
            if (abs < 1024.0 * 1024 * 1024)
                return (b / 1048576).ToString(abs < 10485760 ? "0.0" : "0", s_Inv) + "MB";
            return (b / 1073741824).ToString("0.00", s_Inv) + "GB";
        }

        public static string Pct(double fraction) => (fraction * 100).ToString(Math.Abs(fraction) < 0.1 ? "0.0" : "0", s_Inv) + "%";

        /// <summary>A counter value in its own unit (Unity's ProfilerMarkerDataUnit).</summary>
        public static string Unit(double value, byte unit)
        {
            if (double.IsNaN(value))
                return "-";
            return unit switch
            {
                1 => Ms(value) + "ms",
                2 => Bytes(value),
                4 => value.ToString("0.#", s_Inv) + "%",
                5 => value.ToString("0.#", s_Inv) + "Hz",
                _ => Math.Abs(value) >= 1e6 ? value.ToString("0.###e0", s_Inv) : Num(value),
            };
        }
    }

    /// <summary>Aligned plain-text columns. Numeric-looking cells are right-aligned; the last column is never padded.</summary>
    public sealed class Table
    {
        readonly List<string[]> m_Rows = new List<string[]>();
        readonly string[] m_Header;

        public Table(params string[] header)
        {
            m_Header = header;
        }

        public int Count => m_Rows.Count;

        public void Add(params string[] cells) => m_Rows.Add(cells);

        public void Write(TextWriter w, string indent = "")
        {
            var all = new List<string[]>();
            if (m_Header != null && m_Header.Length > 0)
                all.Add(m_Header);
            all.AddRange(m_Rows);
            var columns = all.Max(r => r.Length);
            var widths = new int[columns];
            foreach (var row in all)
                for (var c = 0; c < row.Length - 1; c++)
                    widths[c] = Math.Max(widths[c], row[c]?.Length ?? 0);
            var sb = new StringBuilder();
            foreach (var row in all)
            {
                sb.Clear().Append(indent);
                for (var c = 0; c < row.Length; c++)
                {
                    var cell = row[c] ?? "";
                    if (c == row.Length - 1)
                    {
                        sb.Append(cell);
                        break;
                    }
                    if (row != m_Header && LooksNumeric(cell))
                        sb.Append(' ', widths[c] - cell.Length).Append(cell);
                    else
                        sb.Append(cell).Append(' ', widths[c] - cell.Length);
                    sb.Append("  ");
                }
                w.WriteLine(sb.ToString().TrimEnd());
            }
        }

        static bool LooksNumeric(string s)
        {
            if (s.Length == 0)
                return false;
            var c = s[0];
            return char.IsDigit(c) || c == '-' || c == '+' || c == '<' || c == '.';
        }
    }
}
