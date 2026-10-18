using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BurstWaitress.Asm;
using BurstWaitress.Model;

namespace BurstWaitress.Commands
{
    /// <summary>Renders one analyzed dump: the summary, a narrowed raw listing, and per-C#-line attribution.</summary>
    public static class Report
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public static string WidthName(Isa isa, int bits, int lane, bool scalable = false)
        {
            if (bits == 0)
                return "scalar";
            if (scalable)
                return lane > 0 ? $"SVE×VL/{lane}" : "SVE";
            var reg = isa == Isa.X86 ? bits switch { 512 => "zmm", 256 => "ymm", 128 => "xmm", _ => bits + "b" } : bits + "-bit";
            var lanes = lane > 0 ? $"×{bits / lane}" : "";
            return reg + lanes;
        }

        /// <summary>An SVE count given at a 128-bit vector length, as a fraction of the vector length in bits.</summary>
        public static string Scalable(int at128) => at128 > 0 && 128 % at128 == 0 ? $"VL/{128 / at128}" : $"{at128} at VL128";

        public static string PerIter(Loop l)
        {
            if (l.ElemsPerIter <= 0)
                return "?";
            // A scalar loop does one source iteration per pass, whatever its counter steps by (i += 2).
            if (l.Shape == Shape.Scalar && !l.PerIterBytes && l.ElemsPerIter > 1)
                return $"1 s{l.ElemsPerIter}";
            if (l.PerIterBytes)
                return l.PerIterScalable ? (l.ElemsPerIter % 16 == 0 ? $"{l.ElemsPerIter / 16} VL B" : l.ElemsPerIter + " B at VL128") : l.ElemsPerIter + " B";
            return l.PerIterScalable ? Scalable(l.ElemsPerIter) : l.ElemsPerIter.ToString(Inv);
        }

        public static string MinCount(Loop l)
        {
            if (l.MinCount <= 0)
                return null;
            if (!l.MinCountScalable)
                return l.MinCount.ToString(Inv);
            var vl = Scalable(l.MinCount) + (l.MinCountExtra != 0 ? l.MinCountExtra.ToString("+0;-0", Inv) : "");
            return l.MinCountFloor > 0 ? $"max({vl}, {l.MinCountFloor})" : vl;
        }

        /// <summary>Packed width with its shape: SLP when the packing is within one source iteration.</summary>
        public static string ShapeWidth(Isa isa, Loop l, bool shortUnroll)
        {
            var s = l.Stats;
            if (!l.Innermost && l.Shape == Shape.Scalar)
                return "outer loop" + (s.PackedWidth > 0 ? " (packed math is in its inner loops)" : "");
            if (s.PackedWidth == 0)
                return s.MoveWidth > 0 ? WidthName(isa, s.MoveWidth, 0, s.Scalable) + " moves only" : "scalar";
            var text = WidthName(isa, s.PackedWidth, s.Lane, s.Scalable) + (s.Copy ? " copy" : "");
            if (l.Shape == Shape.Slp)
                text += " SLP";
            else if (l.Shape == Shape.Packed)
                text += shortUnroll ? " ?" : l.ElemsPerIter > 0 && !l.PerIterBytes ? " (count doesn't fill whole vectors)" : " (span unknown)";
            if (l.Unroll > 1)
                text += (shortUnroll ? " u" : " unroll ") + l.Unroll;
            if (s.OrderedReductions > 0)
                text += shortUnroll ? " ord" : $" (ordered reduction: {s.OrderedReductions} fadda, serial over the lanes)";
            return text;
        }

        public static string LoopLine(Isa isa, Loop l)
        {
            var s = l.Stats;
            var sb = new StringBuilder();
            sb.Append(s.Instrs).Append(" instrs · ").Append(ShapeWidth(isa, l, false));
            if (l.Shape == Shape.Scalar && !l.PerIterBytes && l.ElemsPerIter > 1)
                sb.Append(" (1/iter, counter step ").Append(l.ElemsPerIter).Append(')');
            else if (l.ElemsPerIter > 0)
                sb.Append(" (").Append(PerIter(l)).Append("/iter)");
            if (l.Remainder)
                sb.Append(" · remainder");
            sb.Append(" · packed ").Append(s.PackedRatio < 0 ? (s.PackedInt > 0 ? $"int {s.PackedInt}" : "-") : Pct(s.PackedRatio));
            if (s.VectorMathCalls > 0)
                sb.Append(" · vector math calls ").Append(s.VectorMathCalls);
            if (s.Gathers > 0)
                sb.Append(" · gathers ").Append(s.Gathers);
            else if (s.LaneLoads >= 4 && s.PackedWidth > 0)
                sb.Append(" · gather emulated: ").Append(s.LaneLoads).Append(" lane loads");
            if (s.Fma > 0)
                sb.Append(" · fma ").Append(s.Fma);
            if (MinCount(l) is string n)
                sb.Append(" · runs if n ≥ ").Append(n);
            if (isa != Isa.Wasm && s.PackedWidth > 0)
                sb.Append(l.AliasGuard ? $" · alias check ({l.AliasGuardInstrs} instrs, asm {l.AliasFirstLine}-{l.AliasLastLine}{(l.AliasEvidence != null ? "; " + l.AliasEvidence : "")})" : " · no alias check");
            if (s.Calls > 0)
                sb.Append(" · calls ").Append(s.Calls);
            if (s.Spills + s.Reloads > 0)
                sb.Append(" · spills ").Append(s.Spills).Append('/').Append(s.Reloads);
            return sb.ToString();
        }

        public static string Pct(double r) => r < 0 ? "-" : Math.Round(r * 100).ToString(Inv) + "%";

        public static void Header(TextWriter w, Dump d)
        {
            var e = d.Entry;
            w.WriteLine($"{e.Name} · {(e.IsStatic ? "static" : e.Interface)} · {d.Options.Describe(e)}{(d.Options.Debug ? " · debug lines" : "")}");
        }

        /// <summary>The focus functions the totals count: not Entities journaling or Burst string formatting, which Editor
        /// compiles carry and players don't run in the method's work.</summary>
        public static List<Function> Counted(AsmFile a) => a.Focus.Where(f => f.Tier != 2).ToList();

        /// <summary>Counts over the focus functions, or over the one function --fn picked.</summary>
        public static LoopStats Totals(IEnumerable<Function> functions)
        {
            var t = new LoopStats();
            foreach (var f in functions)
            {
                var s = f.Stats;
                t.Instrs += s.Instrs;
                t.PackedFloat += s.PackedFloat;
                t.ScalarFloat += s.ScalarFloat;
                t.PackedInt += s.PackedInt;
                t.ScalarInt += s.ScalarInt;
                t.Moves += s.Moves;
                t.Loads += s.Loads;
                t.Stores += s.Stores;
                t.Branches += s.Branches;
                t.Calls += s.Calls;
                t.Fma += s.Fma;
                t.Spills += s.Spills;
                t.Reloads += s.Reloads;
                t.Width = Math.Max(t.Width, s.Width);
                t.PackedWidth = Math.Max(t.PackedWidth, s.PackedWidth);
            }
            return t;
        }

        public static string FunctionName(AsmFile a, Function f) =>
            f == a.Entry && !Analyzer.IsUserFunction(f) ? (f == a.Body ? "entry" : "entry stub") : Names.Short(f.Display);

        /// <summary>The functions a --fn value names: an index from the functions list, or text in the name.</summary>
        public static List<Function> Pick(AsmFile a, string fn)
        {
            if (fn == null)
                return a.Focus;
            var funcs = Analyzer.Reached(a);
            if (int.TryParse(fn, out var n))
            {
                if (n < 1 || n > funcs.Count)
                    throw new WaitressException($"function {n}: the dump has functions 1-{funcs.Count} (the summary lists them)");
                return new List<Function> { funcs[n - 1] };
            }
            if (fn.Contains('*'))
            {
                var glob = new System.Text.RegularExpressions.Regex("^" + System.Text.RegularExpressions.Regex.Escape(fn).Replace("\\*", ".*") + "$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var hits = funcs.Where(f => glob.IsMatch(Names.Short(f.Display)) || glob.IsMatch(f.Display)).ToList();
                if (hits.Count == 0)
                    throw new WaitressException($"no function matching '{fn}'; functions:\n" + string.Join("\n", funcs.Select((f, i) => $"  {i + 1} {FunctionName(a, f)}")));
                return hits;
            }
            var match = funcs.Where(f => f.Display.Contains(fn, StringComparison.OrdinalIgnoreCase)).ToList();
            var exact = match.Where(f => Names.Short(f.Display).Equals(fn, StringComparison.OrdinalIgnoreCase) || Names.Short(f.Display).EndsWith("." + fn, StringComparison.OrdinalIgnoreCase)).ToList();
            if (exact.Count == 1)
                return exact;
            if (match.Count == 1)
                return match;
            if (match.Count == 0)
                throw new WaitressException($"no function matching '{fn}'; functions:\n" + string.Join("\n", funcs.Select((f, i) => $"  {i + 1} {FunctionName(a, f)}")));
            throw new WaitressException($"'{fn}' matches {match.Count} functions; use its number:\n" + string.Join("\n", funcs.Select((f, i) => (f, i)).Where(p => match.Contains(p.f)).Select(p => $"  {p.i + 1} {Shorten(p.f.Display, 150)} ({p.f.Stats.Instrs} instrs)")));
        }

        public static int Summary(Dump d, TextWriter w, bool all, string fn = null)
        {
            Header(w, d);
            if (d.Error != null)
            {
                w.WriteLine("compile FAILED:");
                foreach (var line in d.Error.Split('\n').Take(8))
                    w.WriteLine("  " + line);
                return 1;
            }
            var a = d.Asm;
            if (a.Body == null)
            {
                w.WriteLine("no code in the dump");
                return 1;
            }
            var scope = Pick(a, fn);
            var counted = fn != null ? scope : Counted(a);
            var s = Totals(counted);
            var max = all ? int.MaxValue : 90;
            if (fn != null)
                w.WriteLine("function: " + Shorten(scope[0].Display, all ? int.MaxValue : 120));
            var where = fn != null ? "" : counted.Count > 1 ? $" in {counted.Count} functions" : a.Body != a.Entry ? " (the entry stub calls it)" : "";
            var infra = scope.Except(counted).ToList();
            if (infra.Count > 0)
                where += $" (not counting {infra.Count} infra functions, {infra.Sum(f => f.Stats.Instrs)} instrs: journaling and string formatting Editor compiles carry)";
            var widest = scope.SelectMany(x => x.Instrs).Select(x => x.Width).DefaultIfEmpty(0).Max();
            w.WriteLine($"code: {s.Instrs} instrs{where} · widest {(widest == 0 ? "scalar only" : WidthName(a.Isa, widest, 0, scope.Any(f => f.Stats.Scalable)))} · fma {s.Fma} · spills {s.Spills} stores / {s.Reloads} reloads");
            w.WriteLine($"  packed float {s.PackedFloat} · scalar float {s.ScalarFloat} · packed int {s.PackedInt} · scalar int {s.ScalarInt} · moves {s.Moves} · loads {s.Loads} · stores {s.Stores} · branches {s.Branches} · calls {s.Calls}");
            var loops = scope.SelectMany(f => f.Loops).ToList();
            w.WriteLine(loops.Count == 0 ? "loops: none" : $"loops: {loops.Count}" + (a.Main != null && fn == null ? $" (main #{a.Main.Id} in {FunctionName(a, a.Main.Owner)})" : ""));
            var shown = 0;
            var cap = all ? int.MaxValue : 12;
            // The main loop's function first, then by size; within a function, layout order.
            foreach (var f in scope.Where(f => f.Loops.Count > 0).OrderByDescending(f => f.Loops.Any(l => l.Main)).ThenBy(f => f.Tier).ThenByDescending(f => f.Stats.Instrs))
            {
                if (shown >= cap)
                    break;
                if (scope.Count > 1)
                    w.WriteLine($"  in {FunctionName(a, f)} ({f.Stats.Instrs} instrs):");
                foreach (var l in f.Loops.OrderByDescending(l => l.Main).ThenBy(l => l.Stats.FirstLine))
                {
                    if (shown++ >= cap)
                        break;
                    var indent = new string(' ', 2 + 2 * l.Depth);
                    w.WriteLine($"{indent}#{l.Id}{(l.Main ? " main" : "")}{(l.Depth > 1 ? $" (in #{l.Parent?.Id})" : "")}: {LoopLine(a.Isa, l)} · asm {l.Stats.FirstLine}-{l.Stats.LastLine}");
                }
            }
            if (shown > cap)
                w.WriteLine($"  ... {loops.Count - cap} more loops (--all, or --fn <function>)");
            if (loops.Any(l => l.Shape == Shape.Slp))
                w.WriteLine("  SLP: packed within one source iteration (a float4, paired accumulators), not across iterations");
            var calls = a.Calls.Where(c => fn == null || scope.Any(f => c.From.Contains(FunctionName(a, f)))).ToList();
            if (calls.Count == 0)
                w.WriteLine("calls: none");
            else
            {
                w.WriteLine($"calls: {calls.Sum(c => c.Count)} sites to {calls.Count} targets");
                foreach (var g in calls.OrderByDescending(c => c.Kind == "scalar math" && c.OutsideRemainder > 0).ThenByDescending(c => c.InLoop).ThenByDescending(c => c.Count).Take(all ? int.MaxValue : 10))
                {
                    var loopsText = g.InLoop ? ", in loop #" + string.Join(", #", g.Loops) + (g.OutsideRemainder == 0 ? " (remainder)" : "") : "";
                    var from = g.From.Count > 0 ? " from " + string.Join(", ", g.From.Take(all ? 100 : 3)) + (g.From.Count > 3 && !all ? $" +{g.From.Count - 3}" : "") : "";
                    w.WriteLine($"  {g.Count}× {Shorten(g.Display, all ? int.MaxValue : 80)} [{g.Kind}{loopsText}]{from}");
                }
                if (!all && calls.Count > 10)
                    w.WriteLine($"  ... {calls.Count - 10} more (--all)");
            }
            var funcs = Analyzer.Reached(a);
            if (funcs.Count > 1 && fn == null)
            {
                w.WriteLine("functions (fn:N or --fn N picks one):");
                var i = 0;
                foreach (var f in funcs)
                {
                    i++;
                    if (!all && i > 12)
                    {
                        w.WriteLine($"  ... {funcs.Count - 12} more (--all)");
                        break;
                    }
                    var tag = f == a.Body ? "body" : !Analyzer.IsUserFunction(f) ? "lib" : f.Tier == 2 ? "infra" : "";
                    w.WriteLine($"  {i,3} {tag,-5} {f.Stats.Instrs,6} instrs  {f.Loops.Count} loops  {Shorten(f.Display, all ? int.MaxValue : max)}");
                }
            }
            return 0;
        }

        public static string Shorten(string s, int max) => s.Length <= max ? s : s.Substring(0, max - 1) + "…";

        // -----------------------------------------------------------------------------------------------------------
        // Raw listing, narrowed by a selector.

        public static int Raw(Dump d, string selector, TextWriter w)
        {
            if (d.Error != null)
                return Summary(d, w, false);
            var a = d.Asm;
            var f = a.Body;
            int from, to;
            string what;
            selector ??= a.Main != null ? "main" : "body";
            var sel = selector.ToLowerInvariant();
            if (sel == "main" || sel.StartsWith("loop", StringComparison.Ordinal))
            {
                var all = a.Focus.SelectMany(x => x.Loops).ToList();
                var loop = sel == "main" ? a.Main : all.FirstOrDefault(l => "loop:" + l.Id == sel || "loop" + l.Id == sel || "loop:#" + l.Id == sel);
                if (loop == null)
                    throw new WaitressException(all.Count == 0 ? "the code has no loops" : $"no {selector}; loops are #{string.Join(", #", all.Select(l => l.Id))} (loop:N)");
                from = loop.Stats.FirstLine;
                to = loop.Stats.LastLine;
                what = $"loop #{loop.Id} in {FunctionName(a, loop.Owner)}";
            }
            else if (sel == "body" || sel == "entry")
            {
                var fn = sel == "entry" ? a.Entry : f;
                from = fn.StartLine;
                to = fn.EndLine;
                what = sel;
            }
            else if (sel.StartsWith("fn:", StringComparison.Ordinal))
            {
                var picked = Pick(a, selector.Substring(3));
                if (picked.Count > 1)
                {
                    Header(w, d);
                    foreach (var p in picked)
                    {
                        w.WriteLine($"raw: function {Names.Short(p.Display)} (dump lines {p.StartLine}-{p.EndLine})");
                        Lines(a, p.StartLine, p.EndLine, w);
                    }
                    return 0;
                }
                var fn = picked[0];
                from = fn.StartLine;
                to = fn.EndLine;
                what = "function " + Names.Short(fn.Display);
            }
            else if (sel.StartsWith("lines:", StringComparison.Ordinal) || sel.StartsWith("asm:", StringComparison.Ordinal))
            {
                var range = selector.Substring(selector.IndexOf(':') + 1).Split('-');
                if (range.Length != 2 || !int.TryParse(range[0], out from) || !int.TryParse(range[1], out to) || to < from)
                    throw new WaitressException("lines:A-B takes two dump line numbers, as the summary prints them (asm A-B)");
                if (from > a.Lines.Length)
                    throw new WaitressException($"the dump has {a.Lines.Length} lines");
                what = $"dump lines {from}-{to}";
            }
            else if (sel.StartsWith("src:", StringComparison.Ordinal))
            {
                // src:A-B, or src:File.cs:A-B for one file.
                var body = selector.Substring(4);
                string file = null;
                var colon = body.LastIndexOf(':');
                if (colon > 0)
                {
                    file = body.Substring(0, colon);
                    body = body.Substring(colon + 1);
                }
                var range = body.Split('-');
                if (!int.TryParse(range[0], out var lo) || !int.TryParse(range.Length > 1 ? range[1] : range[0], out var hi))
                    throw new WaitressException("src:A-B (or src:File.cs:A-B) takes C# line numbers");
                if (!d.Options.Debug)
                    throw new WaitressException("src: needs line info; add --source");
                return RawSource(d, lo, hi, file, w);
            }
            else
                throw new WaitressException($"unknown --raw selector '{selector}'; use main, loop:N, body, entry, fn:<name|N>, lines:A-B (dump lines) or src:A-B (C# lines, with --source)");
            Header(w, d);
            w.WriteLine($"raw: {what} (dump lines {from}-{to})");
            Lines(a, from, to, w);
            return 0;
        }

        static int RawSource(Dump d, int lo, int hi, string file, TextWriter w)
        {
            var a = d.Asm;
            bool InFile(int id) => file == null || a.Files.TryGetValue(id, out var p) && Path.GetFileName(p.Replace('\\', '/')).Equals(file, StringComparison.OrdinalIgnoreCase);
            if (file != null && !a.Files.Values.Any(p => Path.GetFileName(p.Replace('\\', '/')).Equals(file, StringComparison.OrdinalIgnoreCase)))
                throw new WaitressException($"no file {file} in the line info; files: " + string.Join(", ", a.Files.Values.Where(p => !p.Contains("unknown", StringComparison.Ordinal)).Select(p => Path.GetFileName(p.Replace('\\', '/'))).Distinct()));
            var picked = a.Focus.SelectMany(f => f.Instrs.Select(x => (f, x)))
                          .Where(p => p.x.Mnemonic != ":" && p.x.SrcLine >= lo && p.x.SrcLine <= hi && IsUserFile(a, p.x.SrcFile) && InFile(p.x.SrcFile)).ToList();
            if (picked.Count == 0)
                throw new WaitressException($"no instructions come from C# lines {lo}-{hi}{(file != null ? " of " + file : "")}; --source lists the lines that have code");
            var files = picked.Select(p => p.x.SrcFile).Distinct().Count();
            Header(w, d);
            w.WriteLine($"raw: instructions from C# lines {lo}-{hi}{(file != null ? " of " + file : "")} ({picked.Count} instrs{(files > 1 ? $", from {files} files: add the file, src:File.cs:{lo}-{hi}" : "")})");
            Function last = null;
            foreach (var (f, x) in picked)
            {
                if (f != last)
                    w.WriteLine($"== {FunctionName(a, f)}");
                last = f;
                w.WriteLine($"{x.Line,6}  L{x.SrcLine,-4} {Operand(x)}");
            }
            return 0;
        }

        static bool IsUserFile(AsmFile a, int file) => a.Files.TryGetValue(file, out var p) && !p.Contains("unknown", StringComparison.Ordinal);

        static void Lines(AsmFile a, int from, int to, TextWriter w)
        {
            var lastSrc = -1;
            var byLine = new Dictionary<int, Instr>();
            foreach (var x in a.Functions.SelectMany(f => f.Instrs))
                byLine.TryAdd(x.Line, x);
            var starts = a.Functions.GroupBy(f => f.StartLine).ToDictionary(g => g.Key, g => g.First());
            var inside = a.Functions.FirstOrDefault(f => f.StartLine < from && f.EndLine >= from);
            if (inside != null)
                w.WriteLine($"== in {FunctionName(a, inside)}");
            for (var i = Math.Max(1, from); i <= Math.Min(a.Lines.Length, to); i++)
            {
                if (starts.TryGetValue(i, out var fn))
                    w.WriteLine($"== {FunctionName(a, fn)} ({fn.Stats.Instrs} instrs)");
                if (!byLine.TryGetValue(i, out var x))
                    continue;
                if (x.Mnemonic == ":")
                {
                    w.WriteLine($"{i,6}  {x.Text}:");
                    continue;
                }
                var src = x.SrcLine > 0 && x.SrcLine != lastSrc ? $"  ; L{x.SrcLine}" : "";
                lastSrc = x.SrcLine;
                w.WriteLine($"{i,6}      {Operand(x)}{src}");
            }
        }

        static string Operand(Instr x)
        {
            if (x.Class == Cls.Call && x.Target != null)
                return x.Mnemonic + "\t" + Shorten(Names.Pretty(x.Target), 100);
            return x.Mnemonic + (x.Operands.Length > 0 ? "\t" + string.Join(", ", x.Operands) : "");
        }

        // -----------------------------------------------------------------------------------------------------------
        // Per-C#-line attribution

        public static int Source(Dump d, TextWriter w, bool all, string fn = null)
        {
            Header(w, d);
            if (d.Error != null)
                return Summary(d, w, false);
            var a = d.Asm;
            // Without --fn, journaling and string formatting (infra) stay out, as in the summary's totals.
            var scope = fn != null ? Pick(a, fn) : Counted(a);
            w.WriteLine("note: compiled with line info, which can change code shape slightly; counts below are this compile's");
            var texts = new Dictionary<string, string[]>();
            string SourceText(int file, int line)
            {
                if (!a.Files.TryGetValue(file, out var path) || line <= 0)
                    return "";
                if (!texts.TryGetValue(path, out var lines))
                {
                    try
                    {
                        lines = File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();
                    }
                    catch (IOException)
                    {
                        lines = Array.Empty<string>();
                    }
                    texts[path] = lines;
                }
                return line <= lines.Length ? lines[line - 1].Trim() : "";
            }
            string FileName(int file) => a.Files.TryGetValue(file, out var p) ? Path.GetFileName(p.Replace('\\', '/')) : "?";
            var mainFile = scope.SelectMany(f => f.Instrs).Where(x => x.SrcLine > 0 && IsUserFile(a, x.SrcFile)).GroupBy(x => x.SrcFile).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();

            void Section(string title, IEnumerable<Instr> instrs, int max)
            {
                var groups = instrs.Where(x => x.Mnemonic != ":").GroupBy(x => (x.SrcFile, x.SrcLine)).Select(g => (key: g.Key, list: g.ToList())).ToList();
                if (groups.Count == 0)
                    return;
                w.WriteLine(title);
                var ordered = groups.OrderByDescending(g => g.list.Count).Take(max).OrderBy(g => g.key.SrcFile != mainFile).ThenBy(g => g.key.SrcFile).ThenBy(g => g.key.SrcLine).ToList();
                foreach (var g in ordered)
                {
                    var c = g.list;
                    var cls = new List<string>();
                    void Add(int n, string label)
                    {
                        if (n > 0)
                            cls.Add(n + " " + label);
                    }
                    Add(c.Count(x => x.Class == Cls.PackedFloat), "pf");
                    Add(c.Count(x => x.Class == Cls.ScalarFloat), "sf");
                    Add(c.Count(x => x.Class == Cls.PackedInt), "pi");
                    Add(c.Count(x => x.Load), "ld");
                    Add(c.Count(x => x.Store), "st");
                    Add(c.Count(x => x.Class == Cls.Call), "call");
                    Add(c.Count(x => x.Fma), "fma");
                    var width = c.Where(x => x.Class == Cls.PackedFloat || x.Class == Cls.PackedInt).Select(x => x.Width).DefaultIfEmpty(0).Max();
                    var where = g.key.SrcLine <= 0 ? "(no line)" : (g.key.SrcFile == mainFile ? "" : FileName(g.key.SrcFile) + ":") + "L" + g.key.SrcLine;
                    var text = g.key.SrcLine > 0 ? Shorten(SourceText(g.key.SrcFile, g.key.SrcLine), 56) : "";
                    w.WriteLine($"  {where,-26} {c.Count,4}  {(width > 0 ? WidthName(a.Isa, width, 0) : ""),-4} {string.Join(" ", cls),-30} {text}");
                }
                if (groups.Count > max)
                    w.WriteLine($"  ... {groups.Count - max} more lines (--all)");
            }

            if (mainFile != 0 && a.Files.TryGetValue(mainFile, out var mainPath))
                w.WriteLine("file: " + mainPath);
            w.WriteLine("  line                      instrs width  classes (pf/sf packed/scalar float, pi packed int, ld/st)");
            // Hot loops first: the main loop, then the largest, each with only its own (not its inner loops') lines.
            var loops = scope.SelectMany(f => f.Loops).OrderByDescending(l => l.Main).ThenByDescending(l => l.Stats.Instrs).Take(all ? int.MaxValue : 4).ToList();
            foreach (var l in loops)
            {
                var f = l.Owner;
                var instrs = f.Instrs.Where(x => Analyzer.InLoop(f, l, x) && !f.Loops.Any(c => c.Parent == l && Analyzer.InLoop(f, c, x))).ToList();
                Section($"loop #{l.Id}{(l.Main ? " (main)" : "")} in {FunctionName(a, f)}: {LoopLine(a.Isa, l)}", instrs, all ? 1000 : 10);
            }
            foreach (var f in scope.OrderByDescending(f => f == a.Body).ThenByDescending(f => f.Stats.Instrs).Take(all ? int.MaxValue : 3))
                Section($"outside loops in {FunctionName(a, f)} ({f.Stats.Instrs} instrs):", f.Instrs.Where(x => !f.Loops.Any(l => Analyzer.InLoop(f, l, x))), all ? 1000 : 6);
            return 0;
        }

        // -----------------------------------------------------------------------------------------------------------
        // JSON

        public static object Json(Dump d, string fn = null)
        {
            if (d.Error != null)
                return new { name = d.Entry.Name, options = d.Options.Request(), error = d.Error };
            var a = d.Asm;
            // --fn narrows the counts and loops to the picked functions, as in the text summary.
            var scope = fn != null ? Pick(a, fn) : a.Focus;
            var counted = fn != null ? scope : Counted(a);
            return new
            {
                name = d.Entry.Name,
                function = fn != null ? scope.Select(f => f.Display).ToArray() : null,
                kind = d.Entry.IsStatic ? "static" : d.Entry.Interface,
                target = d.Options.Target,
                floatMode = d.Options.EffectiveFloatMode(d.Entry),
                floatPrecision = d.Options.FloatPrecision ?? (d.Entry.FloatPrecision == "" ? "Standard" : d.Entry.FloatPrecision),
                safety = d.Options.Safety,
                stats = StatsJson(Totals(counted)),
                excludedInfrastructure = a.Focus.Where(f => f.Tier == 2).Select(f => f.Display),
                widest = scope.SelectMany(x => x.Instrs).Select(x => x.Width).DefaultIfEmpty(0).Max(),
                widestScalable = scope.Any(f => f.Stats.Scalable),
                mainLoop = a.Main?.Id,
                loops = scope.SelectMany(f => f.Loops).Select(l => new
                {
                    id = l.Id,
                    function = FunctionName(a, l.Owner),
                    main = l.Main,
                    depth = l.Depth,
                    parent = l.Parent?.Id,
                    shape = !l.Innermost && l.Shape == Shape.Scalar ? "outer" : l.Shape.ToString().ToLowerInvariant(),
                    handPacked = l.HandPacked,
                    remainder = l.Remainder,
                    width = l.Stats.PackedWidth,
                    scalable = l.Stats.Scalable,
                    moveWidth = l.Stats.MoveWidth,
                    lane = l.Stats.Lane,
                    unroll = l.Unroll,
                    perIteration = l.ElemsPerIter > 0 ? l.ElemsPerIter : (int?)null,
                    perIterationUnit = l.ElemsPerIter <= 0 ? null : l.PerIterBytes ? "bytes" : "iterations",
                    perIterationScalable = l.PerIterScalable,
                    packedRatio = l.Stats.PackedRatio < 0 ? (double?)null : Math.Round(l.Stats.PackedRatio, 3),
                    vectorMathCalls = l.Stats.VectorMathCalls,
                    copy = l.Stats.Copy,
                    atomics = l.Stats.Atomics,
                    laneLoads = l.Stats.LaneLoads,
                    gathers = l.Stats.Gathers,
                    orderedReductions = l.Stats.OrderedReductions,
                    strided = l.Strided,
                    minCountExtra = l.MinCountExtra,
                    minCountFloor = l.MinCountFloor,
                    minCount = l.MinCount > 0 ? l.MinCount : (int?)null,
                    minCountScalable = l.MinCountScalable,
                    aliasCheck = l.AliasGuard,
                    aliasCheckInstrs = l.AliasGuardInstrs,
                    aliasCheckLines = l.AliasGuard ? new[] { l.AliasFirstLine, l.AliasLastLine } : null,
                    aliasEvidence = l.AliasEvidence,
                    asmLines = new[] { l.Stats.FirstLine, l.Stats.LastLine },
                    stats = StatsJson(l.Stats),
                }),
                calls = a.Calls.Select(c => new { name = c.Display, kind = c.Kind, count = c.Count, outsideRemainder = c.OutsideRemainder, inLoop = c.InLoop, loops = c.Loops, from = c.From }),
                functions = Analyzer.Reached(a).Select((x, i) => new { index = i + 1, name = x.Display, instrs = x.Stats.Instrs, loops = x.Loops.Count, body = x == a.Body, library = !Analyzer.IsUserFunction(x) && x != a.Body }),
            };
        }

        static object StatsJson(LoopStats s) => new
        {
            instrs = s.Instrs, packedFloat = s.PackedFloat, scalarFloat = s.ScalarFloat, packedInt = s.PackedInt, scalarInt = s.ScalarInt, moves = s.Moves,
            loads = s.Loads, stores = s.Stores, branches = s.Branches, calls = s.Calls, fma = s.Fma, spills = s.Spills, reloads = s.Reloads,
        };

        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = false };
    }
}
