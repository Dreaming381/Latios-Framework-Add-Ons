using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BurstWaitress.Testing
{
    /// <summary>
    /// An independent reading of raw x86 disassembly, written apart from the parser and classifier with plain regular
    /// expressions over lines, so the summary's counts can be checked against it.
    /// </summary>
    public static class Census
    {
        public sealed class Counts
        {
            public int Total, PackedFloat, ScalarFloat, Loads, Stores, Branches, Calls, Ymm;
            public readonly List<(int from, int to)> BackwardRanges = new List<(int, int)>();

            public override string ToString() =>
                $"total {Total} pf {PackedFloat} sf {ScalarFloat} ld {Loads} st {Stores} br {Branches} call {Calls} ymm {Ymm}";
        }

        static readonly Regex s_Instr = new Regex(@"^\s+([a-z][a-z0-9_]*)(?:\s+(.*))?$");
        static readonly Regex s_TopLabel = new Regex(@"^(""[^""]+""|[^\s.""][^\s]*):\s*$");
        static readonly Regex s_LocalLabel = new Regex(@"^(\.L[\w$.]+):\s*$");

        /// <summary>Counts over the function whose label is <paramref name="function"/> (dump line numbers, 1-based).</summary>
        public static Counts Count(string text, string function)
        {
            var lines = text.Replace("\r", "").Split('\n');
            var start = Array.FindIndex(lines, l => l.TrimEnd() == function + ":" || l.TrimEnd() == "\"" + function + "\":");
            if (start < 0)
                throw new Exception("no function " + function);
            var c = new Counts();
            var labels = new Dictionary<string, int>();
            var branches = new List<(int line, string target)>();
            for (var i = start + 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (s_TopLabel.IsMatch(line) && !line.StartsWith("__", StringComparison.Ordinal) || line.Contains(".section", StringComparison.Ordinal) && !line.Contains(".text", StringComparison.Ordinal))
                    break;
                var local = s_LocalLabel.Match(line);
                if (local.Success)
                {
                    labels[local.Groups[1].Value] = i + 1;
                    continue;
                }
                var m = s_Instr.Match(line.Split('#')[0].TrimEnd());
                if (!m.Success)
                    continue;
                var op = m.Groups[1].Value;
                var args = m.Groups[2].Success ? m.Groups[2].Value : "";
                c.Total++;
                if (args.Contains("ymm", StringComparison.Ordinal))
                    c.Ymm++;
                var parts = SplitTop(args);
                var zero = parts.Count >= 2 && parts.All(p => p == parts[0]) && (op.Contains("xor", StringComparison.Ordinal) || op == "sub" || op == "vpsubd" || op == "vpsubq");
                var mov = (op.StartsWith("mov", StringComparison.Ordinal) || op.StartsWith("vmov", StringComparison.Ordinal) || op.StartsWith("vbroadcast", StringComparison.Ordinal)) && !Regex.IsMatch(op, "mov(lh|hl)ps$");
                // Packed float: ps/pd math and lane shuffles, packed conversions; not moves, not integer (vp*) ops.
                var floatShuffle = Regex.IsMatch(op, "^v?(permil|permp[sd]$|perm2f|insertf|extractf|shuff)");
                var packedCvt = Regex.IsMatch(op, "^v?cvt") && !Regex.IsMatch(op, "s[sd]|si2");
                if (!zero && (floatShuffle || packedCvt || !op.StartsWith("vp", StringComparison.Ordinal) && !mov && Regex.IsMatch(op, "p[sd]$") || op.Contains("movmskp", StringComparison.Ordinal)))
                    c.PackedFloat++;
                else if (!op.StartsWith("vp", StringComparison.Ordinal) && !mov && Regex.IsMatch(op, "s[sd]$") && !op.StartsWith("cvt", StringComparison.Ordinal) && !op.StartsWith("vcvt", StringComparison.Ordinal))
                    c.ScalarFloat++;
                else if ((op.StartsWith("cvt", StringComparison.Ordinal) || op.StartsWith("vcvt", StringComparison.Ordinal)) && Regex.IsMatch(op, "s[sd]|si2"))
                    c.ScalarFloat++;
                if (op[0] == 'j' && op != "jmp" || op == "jmp" && args.StartsWith(".L", StringComparison.Ordinal) || op == "ret")
                {
                    c.Branches++;
                    if (op != "ret")
                        branches.Add((i + 1, args.Trim()));
                }
                if (op == "call" || op == "jmp" && !args.StartsWith(".L", StringComparison.Ordinal))
                    c.Calls++;
                if (op == "lea" || op.StartsWith("nop", StringComparison.Ordinal))
                    continue;
                if (op == "push")
                    c.Stores++;
                else if (op == "pop")
                    c.Loads++;
                else if (parts.Count > 0)
                {
                    var memFirst = parts[0].Contains('[', StringComparison.Ordinal);
                    var compare = op.StartsWith("cmp", StringComparison.Ordinal) || op == "test" || op.Contains("comis", StringComparison.Ordinal) || op.StartsWith("vcmp", StringComparison.Ordinal) || op == "call" || op[0] == 'j' || op == "bt";
                    if (memFirst && !compare && parts.Count > 1)
                    {
                        c.Stores++;
                        if (Regex.IsMatch(op, "^(add|sub|and|or|xor|inc|dec|neg|not|shl|shr|sar|sal|adc|sbb|xadd|rol|ror|bts|btr|btc|cmpxchg)$"))
                            c.Loads++;
                    }
                    else if (parts.Any(p => p.Contains('[', StringComparison.Ordinal)))
                        c.Loads++;
                }
            }
            foreach (var (line, target) in branches)
                if (labels.TryGetValue(target, out var at) && at < line)
                    c.BackwardRanges.Add((at, line));
            return c;
        }

        static List<string> SplitTop(string s)
        {
            var list = new List<string>();
            if (s.Trim().Length == 0)
                return list;
            var depth = 0;
            var start = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '[')
                    depth++;
                else if (s[i] == ']')
                    depth--;
                else if (s[i] == ',' && depth == 0)
                {
                    list.Add(s.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }
            list.Add(s.Substring(start).Trim());
            return list;
        }
    }
}
