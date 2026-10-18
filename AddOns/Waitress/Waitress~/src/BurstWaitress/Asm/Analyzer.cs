using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BurstWaitress.Asm
{
    /// <summary>
    /// Control flow per function: basic blocks, dominators, natural loops (back edges to a dominating header), and for
    /// each loop the compare guards on every path to it, which give the alias check and the minimum trip count.
    /// </summary>
    public static class Analyzer
    {
        /// <param name="owner">The entry point's name; functions from its namespace rank first for the main loop.</param>
        public static AsmFile Analyze(string text, string target, string owner = null)
        {
            var file = AsmParser.Parse(text, AsmParser.IsaForTarget(target));
            Classifier.Classify(file);
            foreach (var f in file.Functions)
            {
                if (file.Isa == Isa.Wasm)
                    WasmLoops(f);
                else
                {
                    BuildBlocks(f);
                    FindLoops(f, file.Isa);
                }
                f.Stats = Stats(f, f.Instrs.Where(x => x.Mnemonic != ":"));
                foreach (var l in f.Loops)
                {
                    l.Owner = f;
                    l.Stats = Stats(f, f.Instrs.Where(x => x.Mnemonic != ":" && InLoop(f, l, x)), true);
                    // Runtime alias checks only version vectorized loops.
                    if (l.Stats.PackedFloat + l.Stats.PackedInt + l.Stats.VectorMathCalls == 0 && !l.Stats.Copy)
                    {
                        l.AliasGuard = false;
                        l.AliasEvidence = null;
                        l.AliasGuardInstrs = 0;
                        l.AliasFirstLine = l.AliasLastLine = 0;
                    }
                    l.OwnPacked = f.Instrs.Count(x => (x.Class == Cls.PackedFloat || x.Class == Cls.PackedInt) && InLoop(f, l, x) &&
                                                      !f.Loops.Any(c => c != l && c.Parent == l && InLoop(f, c, x)));
                    if (file.Isa != Isa.Wasm)
                        Iteration(f, l, file.Isa);
                    else
                        Shape(l);
                }
                foreach (var l in f.Loops)
                {
                    ByteStepEvidence(f, l);
                    // No loop vectorizer marks, and one vector or less per pass: a float4 (or similar) per iteration.
                    if (l.Shape == Asm.Shape.Packed && l.PerIterBytes && l.ElemsPerIter > 0 && l.ElemsPerIter <= l.Stats.PackedWidth / 8)
                        l.Shape = Asm.Shape.Slp;
                }
                foreach (var l in f.Loops)
                    VectorizerEvidence(f, l);
                foreach (var l in f.Loops)
                    l.Remainder = l.Shape == Asm.Shape.Scalar && l.Innermost &&
                                  f.Loops.Any(v => v.Shape == Asm.Shape.Vector && v.Depth == l.Depth &&
                                                   (v.Stats.FirstLine < l.Stats.FirstLine || v.AliasGuard || ExitsInto(f, v, l)));
            }
            PickBody(file);
            PickFocus(file, owner);
            CollectCalls(file);
            return file;
        }

        public static bool InLoop(Function f, Loop l, Instr x) =>
            f.Blocks.Count > 0 ? x.Block >= 0 && l.Blocks.Contains(x.Block) : l.Stats != null ? x.Line >= l.Stats.FirstLine && x.Line <= l.Stats.LastLine : false;

        // -----------------------------------------------------------------------------------------------------------
        // Blocks

        static void BuildBlocks(Function f)
        {
            var list = f.Instrs;
            Block current = null;
            var byLabel = new Dictionary<string, int>(StringComparer.Ordinal);
            var startNew = true;
            for (var i = 0; i < list.Count; i++)
            {
                var x = list[i];
                if (x.Mnemonic == ":")
                {
                    if (current == null || current.First <= current.Last || current.Label != null)
                    {
                        current = new Block { Index = f.Blocks.Count, Label = x.Text, First = i + 1, Last = i };
                        f.Blocks.Add(current);
                    }
                    else
                        current.Label ??= x.Text;
                    byLabel[x.Text] = current.Index;
                    startNew = false;
                    continue;
                }
                if (startNew || current == null)
                {
                    current = new Block { Index = f.Blocks.Count, First = i, Last = i - 1 };
                    f.Blocks.Add(current);
                    startNew = false;
                }
                current.Last = i;
                x.Block = current.Index;
                if (x.Class == Cls.Branch || x.Class == Cls.Ret || x.TailCall)
                    startNew = true;
            }
            // Empty label-only blocks fall through to the next.
            for (var b = 0; b < f.Blocks.Count; b++)
            {
                var block = f.Blocks[b];
                var last = LastReal(f, block);
                if (last == null)
                {
                    if (b + 1 < f.Blocks.Count)
                        Link(f, b, b + 1);
                    continue;
                }
                if (last.Class == Cls.Ret || last.TailCall)
                    continue;
                if (last.Class == Cls.Branch)
                {
                    if (last.Target != null && byLabel.TryGetValue(last.Target, out var t))
                        Link(f, b, t);
                    if (last.Conditional && b + 1 < f.Blocks.Count)
                        Link(f, b, b + 1);
                    continue;
                }
                if (b + 1 < f.Blocks.Count)
                    Link(f, b, b + 1);
            }
        }

        static void Link(Function f, int from, int to)
        {
            if (!f.Blocks[from].Succ.Contains(to))
            {
                f.Blocks[from].Succ.Add(to);
                f.Blocks[to].Pred.Add(from);
            }
        }

        public static Instr LastReal(Function f, Block b)
        {
            for (var i = b.Last; i >= b.First; i--)
                if (f.Instrs[i].Mnemonic != ":")
                    return f.Instrs[i];
            return null;
        }

        static readonly HashSet<string> s_ArmFlagSetters = new HashSet<string>
        {
            "cmp", "cmn", "tst", "adds", "subs", "ands", "bics", "ccmp", "ccmn", "fcmp", "fcmpe", "negs", "whilelo", "whilelt", "whilele", "whilels", "whilehi", "whilehs",
            "whilegt", "whilege", "whilewr", "whilerw", "ptest",
        };

        /// <summary>The instruction whose flags a conditional branch tests, skipping instructions the compiler scheduled
        /// between them that leave the flags alone (moves, address arithmetic, vector work).</summary>
        static Instr FlagSetter(Function f, Block b, Instr br, Isa isa)
        {
            var at = f.Instrs.IndexOf(br);
            for (var i = at - 1; i >= b.First; i--)
            {
                var x = f.Instrs[i];
                var m = x.Mnemonic;
                if (m == ":")
                    continue;
                if (isa == Isa.X86)
                {
                    if (m.StartsWith("mov", StringComparison.Ordinal) || m == "lea" || m == "push" || m == "pop" || m == "nop" ||
                        m[0] == 'v' && !m.StartsWith("vucomis", StringComparison.Ordinal) && !m.StartsWith("vcomis", StringComparison.Ordinal) && !m.StartsWith("vptest", StringComparison.Ordinal))
                        continue;
                    return x;
                }
                if (s_ArmFlagSetters.Contains(m))
                    return x;
            }
            return null;
        }

        // -----------------------------------------------------------------------------------------------------------
        // Dominators and loops

        static BitArray[] Dominators(Function f)
        {
            var n = f.Blocks.Count;
            var dom = new BitArray[n];
            var reachable = new bool[n];
            var stack = new Stack<int>();
            if (n == 0)
                return dom;
            stack.Push(0);
            while (stack.Count > 0)
            {
                var b = stack.Pop();
                if (reachable[b])
                    continue;
                reachable[b] = true;
                foreach (var s in f.Blocks[b].Succ)
                    stack.Push(s);
            }
            for (var i = 0; i < n; i++)
            {
                dom[i] = new BitArray(n, i != 0);
                if (i == 0)
                    dom[i][0] = true;
            }
            var changed = true;
            while (changed)
            {
                changed = false;
                for (var b = 1; b < n; b++)
                {
                    if (!reachable[b])
                        continue;
                    var next = new BitArray(n, true);
                    var any = false;
                    foreach (var p in f.Blocks[b].Pred)
                    {
                        if (!reachable[p])
                            continue;
                        next.And(dom[p]);
                        any = true;
                    }
                    if (!any)
                        next.SetAll(false);
                    next[b] = true;
                    if (!Same(next, dom[b]))
                    {
                        dom[b] = next;
                        changed = true;
                    }
                }
            }
            for (var b = 0; b < n; b++)
                if (!reachable[b])
                {
                    dom[b].SetAll(false);
                    dom[b][b] = true;
                }
            return dom;
        }

        static bool Same(BitArray a, BitArray b)
        {
            for (var i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        static void FindLoops(Function f, Isa isa)
        {
            var dom = Dominators(f);
            f.Dom = dom;
            var byHeader = new Dictionary<int, Loop>();
            for (var u = 0; u < f.Blocks.Count; u++)
                foreach (var h in f.Blocks[u].Succ)
                {
                    if (!dom[u][h])
                        continue;
                    if (!byHeader.TryGetValue(h, out var loop))
                    {
                        loop = new Loop { Header = h };
                        loop.Blocks.Add(h);
                        byHeader[h] = loop;
                    }
                    var work = new Stack<int>();
                    if (loop.Blocks.Add(u))
                        work.Push(u);
                    while (work.Count > 0)
                    {
                        var b = work.Pop();
                        foreach (var p in f.Blocks[b].Pred)
                            if (loop.Blocks.Add(p))
                                work.Push(p);
                    }
                }
            var loops = byHeader.Values.OrderBy(l => l.Header).ToList();
            foreach (var l in loops)
            {
                l.Parent = loops.Where(o => o != l && o.Blocks.Count > l.Blocks.Count && o.Blocks.IsSupersetOf(l.Blocks)).OrderBy(o => o.Blocks.Count).FirstOrDefault();
            }
            foreach (var l in loops)
            {
                var d = 1;
                for (var p = l.Parent; p != null; p = p.Parent)
                    d++;
                l.Depth = d;
                l.Innermost = loops.All(o => o.Parent != l);
            }
            for (var i = 0; i < loops.Count; i++)
                loops[i].Id = i + 1;
            f.Loops.AddRange(loops);
            foreach (var l in loops)
            {
                Guards(f, l, dom, isa);
            }
        }

        static void WasmLoops(Function f)
        {
            var open = new Stack<int>();
            for (var i = 0; i < f.Instrs.Count; i++)
            {
                var m = f.Instrs[i].Mnemonic;
                if (m == "loop")
                    open.Push(i);
                else if (m == "end_loop" && open.Count > 0)
                {
                    var start = open.Pop();
                    var l = new Loop { Header = start, Depth = open.Count + 1 };
                    l.Stats = new LoopStats { FirstLine = f.Instrs[start].Line, LastLine = f.Instrs[i].Line };
                    f.Loops.Add(l);
                }
            }
            f.Loops.Sort((a, b) => a.Header.CompareTo(b.Header));
            for (var i = 0; i < f.Loops.Count; i++)
            {
                var l = f.Loops[i];
                l.Id = i + 1;
                l.Innermost = !f.Loops.Any(o => o != l && o.Stats.FirstLine > l.Stats.FirstLine && o.Stats.LastLine < l.Stats.LastLine);
            }
        }

        // -----------------------------------------------------------------------------------------------------------
        // Guards: every conditional branch on all paths to a loop whose other edge can't reach it. A compare of the trip
        // count against a constant bounds the trip count; pointer arithmetic and lane compares are a runtime alias check.

        static readonly Dictionary<string, string> s_X86Cc = new Dictionary<string, string>
        {
            ["jae"] = ">=", ["jnb"] = ">=", ["jnc"] = ">=", ["ja"] = ">", ["jnbe"] = ">", ["jb"] = "<", ["jc"] = "<", ["jnae"] = "<", ["jbe"] = "<=", ["jna"] = "<=",
            ["jge"] = ">=", ["jnl"] = ">=", ["jg"] = ">", ["jnle"] = ">", ["jl"] = "<", ["jnge"] = "<", ["jle"] = "<=", ["jng"] = "<=", ["je"] = "==", ["jz"] = "==",
            ["jne"] = "!=", ["jnz"] = "!=",
        };

        static readonly Dictionary<string, string> s_ArmCc = new Dictionary<string, string>
        {
            ["hs"] = ">=", ["cs"] = ">=", ["hi"] = ">", ["lo"] = "<", ["cc"] = "<", ["ls"] = "<=", ["ge"] = ">=", ["gt"] = ">", ["lt"] = "<", ["le"] = "<=", ["eq"] = "==",
            ["ne"] = "!=",
        };

        static readonly HashSet<string> s_Gpr64 = new HashSet<string> { "rax", "rbx", "rcx", "rdx", "rsi", "rdi", "rbp", "rsp", "r8", "r9", "r10", "r11", "r12", "r13", "r14", "r15" };

        static void Guards(Function f, Loop l, BitArray[] dom, Isa isa)
        {
            var chain = new List<int>();
            for (var d = 0; d < f.Blocks.Count; d++)
                if (d != l.Header && dom[l.Header][d] && !l.Blocks.Contains(d))
                    chain.Add(d);
            chain.Sort((a, b) => Count(dom[b]).CompareTo(Count(dom[a])));
            var aliasFallbacks = new List<(int block, int fallback)>();
            long best = 0, bestExtra = 0;
            l.MinCountFloor = 0;
            var bestScalable = false;
            var excluded = new HashSet<long>();

            void Offer(long n, long extra, bool scalable)
            {
                // Compared at a 128-bit vector length; a scalable bound beats a constant one only when it is larger there.
                if (n + extra > Math.Max(best, l.MinCountFloor) + bestExtra)
                {
                    best = n;
                    bestExtra = extra;
                    bestScalable = scalable;
                    l.MinCountFloor = 0;
                }
            }

            // Walk from the entry toward the loop, so an alias check is known before the checks chained after it.
            foreach (var d in Enumerable.Reverse(chain))
            {
                var block = f.Blocks[d];
                var br = LastReal(f, block);
                if (br == null || br.Class != Cls.Branch || !br.Conditional || block.Succ.Count != 2)
                    continue;
                var taken = block.Succ.FirstOrDefault(s => f.Blocks[s].Label != null && f.Blocks[s].Label == br.Target);
                var fall = block.Succ.FirstOrDefault(s => s != taken);
                if (taken == fall)
                    continue;
                var reachTaken = Reaches(f, taken, l.Header, d);
                var reachFall = Reaches(f, fall, l.Header, d);
                if (reachTaken == reachFall)
                {
                    // Non-overlap is a disjunction (a_end <= b, or b_end <= a): each compare's edges both reach the loop.
                    if (reachTaken && AliasEvidence(f, block, isa) is string disjunct)
                        MarkAlias(l, f, block, disjunct);
                    continue;
                }
                var other = reachTaken ? fall : taken;
                var flag = isa == Isa.Arm64 && (br.Mnemonic.StartsWith("cb", StringComparison.Ordinal) || br.Mnemonic.StartsWith("tb", StringComparison.Ordinal)) ? br : FlagSetter(f, block, br, isa);
                // SVE: a count compared against cnth's vector length is a trip-count guard, not a pointer distance.
                long vl = 0;
                var vlFirst = false;
                Floor = 0;
                if (isa == Isa.Arm64 && flag != null && flag.Mnemonic == "cmp" && flag.Operands.Length == 2)
                {
                    if (VectorLengthConst(f, flag, flag.Operands[1], out vl))
                        vlFirst = false;
                    else if (VectorLengthConst(f, flag, flag.Operands[0], out vl))
                        vlFirst = true;
                }
                var why = vl == 0 ? AliasEvidence(f, block, isa) : null;
                var alias = vl == 0 && (why != null || aliasFallbacks.Any(a => a.fallback == other && dom[d][a.block]));
                if (alias)
                {
                    aliasFallbacks.Add((d, other));
                    MarkAlias(l, f, block, why);
                    // A count test folded into the check (cmp r8d, 9; setb; or ...; jne fallback) still bounds the count.
                    foreach (var (n, _) in FoldedCountTests(f, block, isa))
                        Offer(n, 0, false);
                    continue;
                }
                var bound = Bound(f, br, flag, reachTaken, isa, vl, vlFirst);
                if (bound == null)
                {
                    // Count tests set into flags and or'ed (cmp r8d, 12; setb; or; je vector): the loop needs them false.
                    var toLoopWhenClear = (br.Mnemonic == "je" || br.Mnemonic == "jz") == reachTaken;
                    if (flag != null && (flag.Mnemonic == "or" || flag.Mnemonic == "test" || flag.Mnemonic == "and") && toLoopWhenClear)
                        foreach (var (n, _) in FoldedCountTests(f, block, isa))
                            Offer(n, 0, false);
                    continue;
                }
                var (rel, value, offset) = bound.Value;
                // The guarded register is the count plus offset (sub x9, x8, #1): bound the count itself.
                if (vl != 0 && Floor != 0)
                {
                    if (rel == ">=" || rel == ">")
                    {
                        // Ranked at a 128-bit VL, where the floor is what binds.
                        if (Math.Max(value, Floor) - offset > best + bestExtra)
                        {
                            best = value;
                            bestExtra = -offset + (rel == ">" ? 1 : 0);
                            bestScalable = true;
                            l.MinCountFloor = (int)Floor;
                        }
                    }
                    continue;
                }
                if (vl != 0)
                {
                    if (rel == ">=")
                        Offer(value, -offset, true);
                    else if (rel == ">")
                        Offer(value, 1 - offset, true);
                    continue;
                }
                value -= offset;
                switch (rel)
                {
                    case ">=":
                    case "==":
                        Offer(value, 0, false);
                        break;
                    case ">":
                        Offer(value + 1, 0, false);
                        break;
                    case "!=":
                        excluded.Add(value);
                        break;
                }
            }
            // n >= 1 and n != 1 together mean n >= 2.
            if (!bestScalable)
            {
                if (best == 0 && excluded.Contains(0))
                    best = 1;
                while (best > 0 && excluded.Contains(best))
                    best++;
            }
            l.MinCount = (int)Math.Min(best, int.MaxValue);
            l.MinCountScalable = bestScalable;
            l.MinCountExtra = (int)bestExtra;
        }

        static void MarkAlias(Loop l, Function f, Block block, string why)
        {
            l.AliasGuard = true;
            l.AliasEvidence ??= why;
            l.AliasGuardInstrs += block.Last - block.First + 1 - Enumerable.Range(block.First, block.Last - block.First + 1).Count(i => f.Instrs[i].Mnemonic == ":");
            var lines = Enumerable.Range(block.First, block.Last - block.First + 1).Select(i => f.Instrs[i].Line).ToList();
            l.AliasFirstLine = l.AliasFirstLine == 0 ? lines.Min() : Math.Min(l.AliasFirstLine, lines.Min());
            l.AliasLastLine = Math.Max(l.AliasLastLine, lines.Max());
        }

        /// <summary>Whether the register holds an SVE element count (cnth x9: 8 at a 128-bit vector length), set in the
        /// instructions before the compare.</summary>
        [ThreadStatic] static long Floor;

        static bool VectorLengthConst(Function f, Instr at, string reg, out long value)
        {
            value = 0;
            var r = Gpr(reg, Isa.Arm64);
            if (r == null)
                return false;
            for (var i = f.Instrs.IndexOf(at) - 1; i >= 0; i--)
            {
                var x = f.Instrs[i];
                if (x.Mnemonic == ":" || x.Operands.Length == 0 || Gpr(x.Operands[0], Isa.Arm64) != r || x.Store || x.Class == Cls.Branch ||
                    x.Mnemonic == "cmp" || x.Mnemonic == "cmn" || x.Mnemonic == "tst" || x.Mnemonic == "ccmp")
                    continue;
                if (x.Mnemonic == "csel" && x.Operands.Length == 4)
                {
                    // csel x8, x11, x8, hi after cmp x11, #16: max(cnth, 16). Reported as its value at a 128-bit VL.
                    if (VectorLengthConst(f, x, x.Operands[1], out var v) && ConstantIn(f, x, x.Operands[2], Isa.Arm64, out var floor))
                    {
                        value = v;
                        Floor = floor;
                        return true;
                    }
                    return false;
                }
                var m = Regex.Match(x.Mnemonic, "^cnt([bhwd])$");
                if (!m.Success)
                    return false;
                value = SveCount(m.Groups[1].Value[0], x.Operands);
                return true;
            }
            return false;
        }

        static int Count(BitArray b)
        {
            var c = 0;
            for (var i = 0; i < b.Length; i++)
                if (b[i])
                    c++;
            return c;
        }

        static bool Reaches(Function f, int from, int target, int excluded)
        {
            var seen = new HashSet<int> { excluded };
            var work = new Stack<int>();
            work.Push(from);
            while (work.Count > 0)
            {
                var b = work.Pop();
                if (b == target)
                    return true;
                if (!seen.Add(b))
                    continue;
                foreach (var s in f.Blocks[b].Succ)
                    work.Push(s);
            }
            return false;
        }

        /// <summary>The instruction that last wrote the register before the given one, scanning back through the
        /// function's layout (at most 64 instructions); null when none is found.</summary>
        static Instr Definition(Function f, Instr at, string reg, Isa isa)
        {
            var r = Gpr(reg, isa);
            if (r == null)
                return null;
            var start = f.Instrs.IndexOf(at);
            for (var i = start - 1; i >= 0 && i >= start - 64; i--)
            {
                var x = f.Instrs[i];
                // A call returns in rax / x0 and clobbers it.
                if (x.Class == Cls.Call && !x.TailCall && (r == "rax" || r == "x0"))
                    return x;
                if (x.Mnemonic == ":" || x.Operands.Length == 0 || x.Class == Cls.Branch || x.Store)
                    continue;
                var m = x.Mnemonic;
                if (isa == Isa.X86 ? s_X86NoWrite.Contains(m) : m.StartsWith("cmp", StringComparison.Ordinal) || m == "cmn" || m == "tst" || m == "ccmp" || m == "ccmn")
                    continue;
                if (Gpr(x.Operands[0], isa) == r)
                    return x;
            }
            return null;
        }

        /// <summary>Whether the register holds a difference of two registers (a pointer distance), through copies.</summary>
        static bool IsDistance(Function f, Instr at, string reg, Isa isa, int depth = 0)
        {
            var def = Definition(f, at, reg, isa);
            if (def == null || depth > 2)
                return false;
            var ops = def.Operands;
            if (def.Mnemonic == "sub" || def.Mnemonic == "subs")
            {
                if (isa == Isa.X86)
                    return ops.Length == 2 && Gpr(ops[1], isa) != null && IsPointerValue(f, def, ops[0], isa) && IsPointerValue(f, def, ops[1], isa);
                return ops.Length >= 3 && Gpr(ops[2], isa) != null && IsPointerValue(f, def, ops[1], isa) && IsPointerValue(f, def, ops[2], isa);
            }
            if ((def.Mnemonic == "mov" || def.Mnemonic == "lsr" || def.Mnemonic == "asr" || def.Mnemonic == "sar" || def.Mnemonic == "shr") && ops.Length >= 2 &&
                Gpr(ops[isa == Isa.X86 && def.Mnemonic != "mov" ? 0 : 1], isa) != null)
                return IsDistance(f, def, ops[isa == Isa.X86 && def.Mnemonic != "mov" ? 0 : 1], isa, depth + 1);
            return false;
        }

        /// <summary>Whether the register can hold a pointer: not last written as a 32-bit value (an int count Burst widened,
        /// mov r11d / cmovl r11d), through copies. Unknown origins (arguments) can.</summary>
        static bool IsPointerValue(Function f, Instr at, string reg, Isa isa, int depth = 0)
        {
            var def = Definition(f, at, reg, isa);
            if (def == null || depth > 3 || def.Class == Cls.Call)
                return true;
            var dst = def.Operands[0];
            if (isa == Isa.X86 ? !s_Gpr64.Contains(dst) : dst.StartsWith("w", StringComparison.Ordinal))
                return false;
            if ((def.Mnemonic == "mov" || def.Mnemonic == "movsxd") && def.Operands.Length == 2 && Gpr(def.Operands[1], isa) != null)
                return def.Mnemonic == "mov" && IsPointerValue(f, def, def.Operands[1], isa, depth + 1);
            return true;
        }

        /// <summary>Whether the register holds a constant set by a move (mov x11, #-1073741824; movabs r9, imm).</summary>
        static bool ConstantIn(Function f, Instr at, string reg, Isa isa, out long value)
        {
            value = 0;
            var def = Definition(f, at, reg, isa);
            return def != null && (def.Mnemonic == "mov" || def.Mnemonic == "movabs" || def.Mnemonic == "movz") && def.Operands.Length == 2 &&
                   TryImmediate(def.Operands[1], isa, out value);
        }

        /// <summary>An end pointer: lea with an index, or an add of two registers.</summary>
        static bool IsAddress(Function f, Instr at, string reg, Isa isa)
        {
            var def = Definition(f, at, reg, isa);
            if (def == null)
                return false;
            if (def.Mnemonic == "lea" && def.Operands.Length == 2)
                return Regex.Matches(def.Operands[1], @"\b(r\w+|e\w\w)\b").Count >= 2;
            if (def.Mnemonic != "add")
                return false;
            var ops = def.Operands;
            if (isa == Isa.X86)
                return ops.Length == 2 && Gpr(ops[1], isa) != null && !ConstantIn(f, def, ops[1], isa, out _);
            return ops.Length >= 3 && Gpr(ops[2], isa) != null && !ConstantIn(f, def, ops[1], isa, out _) && !ConstantIn(f, def, ops[2], isa, out _);
        }

        /// <summary>Whether the path to the loop requires the two compared values strictly ordered (start &lt; end).</summary>
        static bool StrictToLoop(string rel, bool whenTaken) =>
            whenTaken ? rel == ">" || rel == "<" : rel == ">=" || rel == "<=";

        /// <summary>A compare of two 32-bit registers (start against end).</summary>
        static bool Narrow(Instr flag, Isa isa)
        {
            var ops = flag.Operands.Skip(flag.Mnemonic == "subs" ? 1 : 0).Take(2).ToArray();
            return ops.Length == 2 && ops.All(o => Gpr(o, isa) != null && !IsWide(o, isa));
        }

        static bool IsWide(string op, Isa isa) => isa == Isa.X86 ? s_Gpr64.Contains(op) : isa == Isa.Arm64 && Regex.IsMatch(op, @"^x\d+$");

        /// <summary>
        /// A runtime pointer-overlap test: 64-bit lane compares of pointer vectors, SVE whilewr/whilerw, or a compare whose
        /// operand is a pointer distance, or that compares two 64-bit registers next to an end pointer or a setcc. A
        /// 64-bit count compared with a constant is a count guard (Burst widens int counts).
        /// </summary>
        static bool IsAliasCheck(Function f, Block block, Isa isa) => AliasEvidence(f, block, isa) != null;

        /// <summary>The instruction that makes the block an alias check, with why; null when it isn't one.</summary>
        static string AliasEvidence(Function f, Block block, Isa isa)
        {
            var hasSet = false;
            for (var i = block.First; i <= block.Last; i++)
            {
                var x = f.Instrs[i];
                var m = x.Mnemonic;
                if (isa == Isa.X86 && (m == "vpcmpgtq" || m == "pcmpgtq"))
                    return Quote(x) + ": pointer lanes compared";
                if (m == "whilewr" || m == "whilerw")
                    return Quote(x) + ": SVE overlap test";
                if (m.StartsWith("set", StringComparison.Ordinal) || m == "cset")
                    hasSet = true;
            }
            for (var i = block.First; i <= block.Last; i++)
            {
                var x = f.Instrs[i];
                var m = x.Mnemonic;
                if (!(m == "cmp" || m == "ccmp" || m == "ccmn" || m == "cmn") || x.Operands.Length < 2 || !OrderedUse(f, block, i, isa))
                    continue;
                var a = x.Operands[0];
                var b = x.Operands[1];
                if (IsWide(a, isa) && IsDistance(f, x, a, isa) || IsWide(b, isa) && IsDistance(f, x, b, isa))
                    return Quote(x) + ": pointer distance against the span";
                if (IsWide(a, isa) && IsWide(b, isa) && !ConstantIn(f, x, a, isa, out _) && !ConstantIn(f, x, b, isa, out _) &&
                    (hasSet || IsAddress(f, x, a, isa) || IsAddress(f, x, b, isa)))
                    return Quote(x) + ": pointer against pointer";
            }
            return null;
        }

        static readonly Regex s_EqualityCc = new Regex(@"^(j|set|cmov)(e|ne|z|nz)$|^b\.(eq|ne)$", RegexOptions.Compiled);

        /// <summary>Whether the flags of the compare at index i are read with an ordering condition (jae, setb, b.hs,
        /// cset .., lo): an overlap test orders pointers, while eq/ne compares a cached value.</summary>
        static bool OrderedUse(Function f, Block block, int i, Isa isa)
        {
            for (var j = i + 1; j <= block.Last + 1 && j < f.Instrs.Count; j++)
            {
                var x = f.Instrs[j];
                var m = x.Mnemonic;
                if (m == ":")
                    continue;
                if (isa == Isa.X86)
                {
                    if (m[0] == 'j' && m != "jmp" || m.StartsWith("set", StringComparison.Ordinal) || m.StartsWith("cmov", StringComparison.Ordinal))
                        return !s_EqualityCc.IsMatch(m);
                    if (m == "cmp" || m == "test" || s_IntAlu.Contains(m) && m != "lea" && m != "mov")
                        return false;
                    continue;
                }
                if (m.StartsWith("b.", StringComparison.Ordinal))
                    return !s_EqualityCc.IsMatch(m);
                if (m == "cset" || m == "csel" || m == "csinc" || m == "ccmp" || m == "ccmn" || m == "cinc" || m == "csetm")
                {
                    var cc = x.Operands.Length > 0 ? x.Operands[x.Operands.Length - 1].Trim() : "";
                    return cc != "eq" && cc != "ne";
                }
                if (m == "cmp" || m == "cmn" || m == "tst" || m == "subs" || m == "adds" || m == "ands")
                    return false;
            }
            return false;
        }

        static readonly HashSet<string> s_IntAlu = new HashSet<string> { "add", "sub", "and", "or", "xor", "inc", "dec", "neg", "shl", "shr", "sar", "imul", "adc", "sbb" };

        static string Quote(Instr x) => $"{x.Line} {x.Mnemonic} {string.Join(", ", x.Operands)}";

        /// <summary>Count tests set into flags inside an alias check: cmp r8d, 9; setb means n &lt; 9 goes to the fallback,
        /// so the vector loop needs n &gt;= 9.</summary>
        static IEnumerable<(long n, Instr at)> FoldedCountTests(Function f, Block block, Isa isa)
        {
            if (isa != Isa.X86)
                yield break;
            for (var i = block.First; i < block.Last; i++)
            {
                var x = f.Instrs[i];
                if (x.Mnemonic != "cmp" || x.Operands.Length != 2 || IsWide(x.Operands[0], isa) || Gpr(x.Operands[0], isa) == null || !TryImmediate(x.Operands[1], isa, out var n))
                    continue;
                for (var j = i + 1; j <= block.Last && j <= i + 3; j++)
                {
                    var m = f.Instrs[j].Mnemonic;
                    var offset = CountOffset(f, x, x.Operands[0], isa);
                    if (m == "setb" || m == "setl" || m == "setnae" || m == "setc")
                    {
                        yield return (n - offset, x);
                        break;
                    }
                    if (m == "setbe" || m == "setle" || m == "setna")
                    {
                        yield return (n + 1 - offset, x);
                        break;
                    }
                    if (m == "cmp" || m == "test")
                        break;
                }
            }
        }

        /// <summary>The count register's offset from the count (sub x9, x8, #1 is -1; lea ecx, [rdx + 8] is +8).</summary>
        static long CountOffset(Function f, Instr flag, string reg, Isa isa)
        {
            var def = Definition(f, flag, reg, isa);
            if (def == null)
                return 0;
            var ops = def.Operands;
            if (isa == Isa.X86)
            {
                if (def.Mnemonic == "lea" && ops.Length == 2)
                {
                    var m = Regex.Match(ops[1], @"^\[\s*\w+\s*([+-])\s*(\d+)\s*\]$");
                    if (m.Success)
                        return (m.Groups[1].Value == "-" ? -1 : 1) * long.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                }
                // not edx; add edx, r8d: end + ~start, the count minus one.
                if (def.Mnemonic == "add" && ops.Length == 2 && Gpr(ops[1], isa) != null && Definition(f, def, ops[0], isa) is Instr prev && prev.Mnemonic == "not")
                    return -1;
                return 0;
            }
            if ((def.Mnemonic == "sub" || def.Mnemonic == "add") && ops.Length == 3 && Gpr(ops[1], isa) != null && TryImmediate(ops[2], isa, out var v))
                return def.Mnemonic == "sub" ? -v : v;
            return 0;
        }

        /// <summary>
        /// The relation a guard requires on the way to the loop, as (relation, value, offset): the guarded register
        /// REL value, where the register is the count plus offset. Null when the guard isn't a count compare.
        /// </summary>
        /// <param name="vl">Nonzero when one compare operand is an SVE element count of this value; vlFirst when it is the
        /// first operand, which turns the relation around.</param>
        static (string rel, long value, long offset)? Bound(Function f, Instr br, Instr flag, bool whenTaken, Isa isa, long vl, bool vlFirst)
        {
            if (flag == null)
                return null;
            string rel;
            long n;
            string countReg = null;
            if (isa == Isa.X86)
            {
                if (!s_X86Cc.TryGetValue(br.Mnemonic, out rel))
                    return null;
                if (flag.Mnemonic == "cmp" && flag.Operands.Length == 2 && TryImmediate(flag.Operands[1], isa, out n))
                    countReg = flag.Operands[0];
                else if (flag.Mnemonic == "test" && flag.Operands.Length == 2 && flag.Operands[0] == flag.Operands[1])
                {
                    n = 0;
                    countReg = flag.Operands[0];
                }
                else if (flag.Mnemonic == "cmp" && Narrow(flag, isa) && StrictToLoop(rel, whenTaken))
                    return (">=", 1, 0);
                else
                    return null;
            }
            else if (isa == Isa.Arm64 || isa == Isa.Arm32)
            {
                if (br.Mnemonic == "cbz" || br.Mnemonic == "cbnz")
                {
                    rel = br.Mnemonic == "cbz" ? "==" : "!=";
                    n = 0;
                    countReg = br.Operands.FirstOrDefault();
                }
                else
                {
                    var cc = br.Mnemonic.StartsWith("b.", StringComparison.Ordinal) ? br.Mnemonic.Substring(2) : br.Mnemonic.Length == 3 ? br.Mnemonic.Substring(1) : "";
                    if (!s_ArmCc.TryGetValue(cc, out rel))
                        return null;
                    if (vl != 0)
                    {
                        n = vl;
                        countReg = flag.Operands[vlFirst ? 1 : 0];
                        if (vlFirst)
                            rel = rel switch { ">=" => "<=", "<=" => ">=", ">" => "<", "<" => ">", _ => rel };
                    }
                    else if (flag.Mnemonic == "cmp" && flag.Operands.Length == 2 && TryImmediate(flag.Operands[1], isa, out n))
                        countReg = flag.Operands[0];
                    else if (flag.Mnemonic == "cmp" && flag.Operands.Length == 2 && ConstantIn(f, flag, flag.Operands[1], isa, out n))
                        countReg = flag.Operands[0];
                    else if ((flag.Mnemonic == "cmp" || flag.Mnemonic == "subs") && Narrow(flag, isa) && StrictToLoop(rel, whenTaken))
                        return (">=", 1, 0);
                    else
                        return null;
                }
            }
            else
                return null;
            if (!whenTaken)
                rel = rel switch { ">=" => "<", "<" => ">=", ">" => "<=", "<=" => ">", "==" => "!=", _ => "==" };
            var offset = countReg != null ? CountOffset(f, flag, countReg, isa) : 0;
            return (rel, n, offset);
        }

        static bool IsImmediate(string s, Isa isa) => TryImmediate(s, isa, out _);

        static bool TryImmediate(string s, Isa isa, out long value)
        {
            if (isa != Isa.X86)
                s = s.TrimStart('#');
            if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return long.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            return long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        // -----------------------------------------------------------------------------------------------------------
        // Stats, spills and calls

        static LoopStats Stats(Function f, IEnumerable<Instr> instrs, bool loop = false)
        {
            var s = new LoopStats { FirstLine = int.MaxValue };
            var spillSlots = SpillSlots(f);
            // NEON code can use a few SVE2 bit operations on the same registers; SVE when most packed math is.
            var scalableVotes = 0;
            foreach (var x in instrs)
            {
                s.Instrs++;
                s.FirstLine = Math.Min(s.FirstLine, x.Line);
                s.LastLine = Math.Max(s.LastLine, x.Line);
                switch (x.Class)
                {
                    case Cls.PackedFloat:
                        s.PackedFloat++;
                        break;
                    case Cls.ScalarFloat:
                        s.ScalarFloat++;
                        break;
                    case Cls.PackedInt:
                        s.PackedInt++;
                        break;
                    case Cls.ScalarInt:
                        s.ScalarInt++;
                        break;
                    case Cls.Move:
                        s.Moves++;
                        break;
                    case Cls.Branch:
                    case Cls.Ret:
                        s.Branches++;
                        break;
                    case Cls.Call:
                        s.Calls++;
                        var kind = CallKind(x.Target);
                        if (kind == "vector math")
                        {
                            s.VectorMathCalls++;
                            var vw = VectorCallWidth(x.Target, out var vl);
                            if (vw > s.CallWidth)
                            {
                                s.CallWidth = vw;
                                s.CallLane = vl;
                            }
                        }
                        else if (kind == "scalar math")
                            s.ScalarMathCalls++;
                        break;
                }
                if (x.Class == Cls.PackedFloat || x.Class == Cls.PackedInt)
                    scalableVotes += x.Scalable ? 1 : -1;
                if (x.Load)
                    s.Loads++;
                if (x.Store)
                    s.Stores++;
                if (x.Fma)
                    s.Fma++;
                if (x.Mnemonic == "fadda")
                    s.OrderedReductions++;
                if (x.Class == Cls.Move && x.Width >= 128 && x.Load)
                    s.VectorLoads++;
                if (x.Class == Cls.Move && x.Width >= 128 && x.Store)
                    s.VectorStores++;
                if (x.Mnemonic == "lock" || x.Mnemonic.StartsWith("cmpxchg", StringComparison.Ordinal) || Regex.IsMatch(x.Mnemonic, "^(ldaxr|stlxr|ldxr|stxr|cas|casal|ldadd|swp)"))
                    s.Atomics++;
                // One element loaded into one lane (vinsertps xmm, xmm, [mem], ld1 {v0.s}[1]): a gather done lane by lane.
                if (x.Load && IsLaneLoad(x))
                    s.LaneLoads++;
                if (x.Mnemonic.StartsWith("vgather", StringComparison.Ordinal) || x.Mnemonic.StartsWith("vpgather", StringComparison.Ordinal) ||
                    x.Mnemonic.StartsWith("ld1", StringComparison.Ordinal) && x.Operands.Any(o => Regex.IsMatch(o, @"z\d+\.[sd]\s*,\s*lsl|\[\s*x\d+\s*,\s*z\d+")))
                    s.Gathers++;
                if ((x.Class == Cls.PackedFloat || x.Class == Cls.PackedInt) && x.Width > s.PackedWidth)
                    s.PackedWidth = x.Width;
                if (x.Class == Cls.Move && x.Width > s.MoveWidth)
                    s.MoveWidth = x.Width;
                if (x.StackSlot != null && !x.Prologue && !x.Epilogue && spillSlots.Contains(x.StackSlot))
                {
                    if (x.Store)
                        s.Spills++;
                    else if (x.Load)
                        s.Reloads++;
                }
            }
            if (s.Instrs == 0)
                s.FirstLine = 0;
            // With no packed math (SVE stores of a splat), any z register makes the moves scalable.
            s.Scalable = scalableVotes > 0 || scalableVotes == 0 && s.PackedFloat + s.PackedInt == 0 && instrs.Any(x => x.Scalable);
            s.Width = s.PackedWidth > 0 ? s.PackedWidth : s.MoveWidth;
            // Lane size of the widest packed math: float first, then int.
            foreach (var x in instrs)
                if (x.Class == Cls.PackedFloat && x.Width == s.PackedWidth && x.Lane > 0)
                {
                    s.Lane = x.Lane;
                    break;
                }
            if (s.Lane == 0)
                foreach (var x in instrs)
                    if (x.Class == Cls.PackedInt && x.Width == s.PackedWidth && x.Lane > 0)
                    {
                        s.Lane = x.Lane;
                        break;
                    }
            // A loop that only moves vectors (a vectorized copy or conversion) works at the move width.
            if (loop && s.PackedFloat + s.PackedInt + s.VectorMathCalls == 0 && s.ScalarFloat == 0 && s.VectorLoads > 0 && s.VectorStores > 0)
            {
                s.Copy = true;
                s.PackedWidth = s.MoveWidth;
            }
            // Packed work done out of line by vector math routines counts at their width.
            if (s.CallWidth > s.PackedWidth)
            {
                s.PackedWidth = s.CallWidth;
                s.Lane = s.CallLane;
                s.Width = Math.Max(s.Width, s.CallWidth);
            }
            return s;
        }

        static bool IsLaneLoad(Instr x)
        {
            var m = x.Mnemonic;
            if (m == "vinsertps" || m == "insertps" || m.StartsWith("vpinsr", StringComparison.Ordinal) || m.StartsWith("pinsr", StringComparison.Ordinal))
                return true;
            return m == "ld1" && x.Operands.Length > 0 && Regex.IsMatch(x.Operands[0], @"\}\s*\[\d+\]|\.[bhsd]\[\d+\]");
        }

        static readonly Regex s_SleefLanes = new Regex(@"(?i)sleef_[a-z0-9]*?([fd])(\d+)_", RegexOptions.Compiled);

        /// <summary>Sleef_sinf4_u35sse4 works on 4 floats: 128 bits of 32-bit lanes.</summary>
        static int VectorCallWidth(string name, out int lane)
        {
            lane = 0;
            var m = name == null ? Match.Empty : s_SleefLanes.Match(name);
            if (!m.Success)
                return 0;
            lane = m.Groups[1].Value.ToLowerInvariant() == "d" ? 64 : 32;
            return lane * int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
        }

        static readonly Dictionary<Function, HashSet<string>> s_SpillCache = new Dictionary<Function, HashSet<string>>();

        /// <summary>Stack slots the body both writes and reads: spills. Slots only written are outgoing call arguments;
        /// slots only read are incoming stack arguments.</summary>
        static HashSet<string> SpillSlots(Function f)
        {
            lock (s_SpillCache)
            {
                if (s_SpillCache.TryGetValue(f, out var cached))
                    return cached;
                var stored = new HashSet<string>();
                var loaded = new HashSet<string>();
                foreach (var x in f.Instrs)
                {
                    if (x.StackSlot == null || x.Prologue || x.Epilogue)
                        continue;
                    if (x.Store)
                        stored.Add(x.StackSlot);
                    else if (x.Load)
                        loaded.Add(x.StackSlot);
                }
                stored.IntersectWith(loaded);
                // A slot whose address is taken (lea rdx, [rbp - 20]; add x1, sp, #16) is a local passed by reference.
                foreach (var x in f.Instrs)
                {
                    if (x.Mnemonic == "lea" && x.Operands.Length == 2 && Classifier.StackSlotOf(x.Operands[1]) is string taken)
                        stored.Remove(taken);
                    else if ((x.Mnemonic == "add" || x.Mnemonic == "sub") && x.Operands.Length == 3 && (x.Operands[1] == "sp" || x.Operands[1] == "x29") &&
                             TryImmediate(x.Operands[2], Isa.Arm64, out var k))
                        stored.Remove(x.Operands[1] + (x.Mnemonic == "sub" ? -k : k).ToString(CultureInfo.InvariantCulture));
                }
                s_SpillCache[f] = stored;
                return stored;
            }
        }

        /// <summary>The body plus every non-library function it reaches, with loops renumbered across them, and the
        /// main loop picked among all of them: for jobs, the producer's dispatch loop in the entry rarely is it.</summary>
        static readonly Regex s_Infrastructure = new Regex(@"^Unity\.(Entities\.EntitiesJournaling|Burst\.|Collections\.FixedString|Collections\.LowLevel\.Unsafe\.UnsafeUtility|Entities\.FixedString)", RegexOptions.Compiled);

        /// <summary>0: the body or the entry point's own namespace; 1: other C# functions; 2: Entities journaling, Burst
        /// string formatting and similar infrastructure, which ride along in Editor compiles.</summary>
        static readonly Regex s_Producer = new Regex(@"JobChunkProducer|JobProducer|ParallelForJobStruct|JobStruct`?\d*<|IJobExtensions|JobsUtility", RegexOptions.Compiled);

        public static int Tier(AsmFile file, Function f, string owner)
        {
            if (f == file.Body && !s_Producer.IsMatch(f.Display))
                return 0;
            if (owner != null)
            {
                var parts = owner.Split('.');
                var prefix = parts.Length >= 2 ? parts[0] + "." + parts[1] : parts[0];
                if (f.Display.StartsWith(prefix, StringComparison.Ordinal))
                    return 0;
            }
            return s_Infrastructure.IsMatch(f.Display) ? 2 : 1;
        }

        static void PickFocus(AsmFile file, string owner)
        {
            if (file.Body == null)
                return;
            foreach (var f in Reached(file))
                if (f == file.Body || IsUserFunction(f))
                    file.Focus.Add(f);
            var id = 1;
            foreach (var f in file.Focus)
                foreach (var l in f.Loops.OrderBy(l => l.Stats.FirstLine))
                    l.Id = id++;
            foreach (var f in file.Focus)
                f.Tier = Tier(file, f, owner);
            var inner = file.Focus.SelectMany(f => f.Loops).Where(l => l.Innermost).ToList();
            if (inner.Count == 0)
                return;
            // When every innermost loop is an atomic retry loop, a loop around it with packed work of its own
            // is the better answer (an SLP loop containing a lock cmpxchg loop).
            if (inner.All(l => l.Stats.Atomics > 0))
                inner.AddRange(file.Focus.SelectMany(f => f.Loops).Where(l => !l.Innermost && l.OwnPacked > 0 && l.Shape != Asm.Shape.Scalar));
            var main = inner.OrderBy(l => l.Owner.Tier).ThenByDescending(l => ShapeRank(l.Shape) - (l.Shape == Asm.Shape.Packed && l.Stats.Calls > l.Stats.VectorMathCalls ? 1.5 : 0) -
                                                                              (l.Stats.Copy ? 0.5 : 0) - (l.Stats.Atomics > 0 ? 4 : 0))
                            .ThenByDescending(l => l.Stats.PackedWidth)
                            .ThenByDescending(l => l.Stats.PackedFloat + l.Stats.VectorMathCalls)
                            .ThenByDescending(l => l.Stats.PackedInt)
                            .ThenByDescending(l => l.Stats.Instrs)
                            .First();
            main.Main = true;
            file.Main = main;
        }

        /// <summary>Loops vectorized across iterations first, then packed loops whose span isn't known, then SLP.</summary>
        static int ShapeRank(Shape s) => s switch { Asm.Shape.Vector => 3, Asm.Shape.Packed => 2, Asm.Shape.Slp => 1, _ => 0 };

        /// <summary>A C# method Burst kept as its own function, as opposed to a library routine or Burst helper.</summary>
        public static bool IsUserFunction(Function f) => f.Name.Contains('(', StringComparison.Ordinal);

        // -----------------------------------------------------------------------------------------------------------
        // Iterations: what one pass of the loop advances. A symbolic walk of the body tracks each register as its value at
        // the header plus a constant; a register that comes back to itself plus a step is an induction variable, and the
        // one behind the back edge's compare is the loop's counter (the compare often tests a value derived from it,
        // such as the next pass's i + 8).

        readonly struct Sym
        {
            public readonly string Base;
            public readonly long Off;
            /// <summary>SVE: a vector-length-dependent part, at a 128-bit vector length.</summary>
            public readonly long VOff;

            public Sym(string b, long off, long voff)
            {
                Base = b;
                Off = off;
                VOff = voff;
            }

            public bool Known => Base != null;
            public Sym Add(long off, long voff) => Known ? new Sym(Base, Off + off, VOff + voff) : this;
            public static readonly Sym Unknown = new Sym(null, 0, 0);
        }

        static readonly Dictionary<string, string> s_X86Reg = BuildX86Regs();

        static Dictionary<string, string> BuildX86Regs()
        {
            var d = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var (r, alts) in new[]
                     {
                         ("rax", "eax ax al ah"), ("rbx", "ebx bx bl bh"), ("rcx", "ecx cx cl ch"), ("rdx", "edx dx dl dh"), ("rsi", "esi si sil"),
                         ("rdi", "edi di dil"), ("rbp", "ebp bp bpl"), ("rsp", "esp sp spl"),
                     })
            {
                d[r] = r;
                foreach (var a in alts.Split(' '))
                    d[a] = r;
            }
            for (var i = 8; i <= 15; i++)
                foreach (var s in new[] { "", "d", "w", "b" })
                    d["r" + i + s] = "r" + i;
            return d;
        }

        static readonly Regex s_ArmGpr = new Regex(@"^[xw](\d+)$", RegexOptions.Compiled);

        /// <summary>The 64-bit register a general-purpose register name is part of, or null.</summary>
        static string Gpr(string op, Isa isa)
        {
            if (isa == Isa.X86)
                return s_X86Reg.TryGetValue(op, out var r) ? r : null;
            var m = s_ArmGpr.Match(op);
            if (m.Success)
                return "x" + m.Groups[1].Value;
            return isa == Isa.Arm32 && Regex.IsMatch(op, @"^r\d+$") ? op : null;
        }

        static readonly HashSet<string> s_X86NoWrite = new HashSet<string> { "cmp", "test", "bt", "push", "call", "nop", "prefetcht0", "prefetchnta" };
        static readonly HashSet<string> s_X86CallerSaved = new HashSet<string> { "rax", "rcx", "rdx", "r8", "r9", "r10", "r11" };
        static readonly Regex s_SveCountOp = new Regex(@"^(inc|dec)([bhwd])$", RegexOptions.Compiled);

        /// <summary>A register already stepped this pass plus one not yet changed (an end pointer, a bound) keeps the
        /// stepped one's base: the sum steps with it. Otherwise unknown.</summary>
        static Sym SteppedPlusInvariant(Sym a, Sym b)
        {
            bool Stepped(Sym s) => s.Known && s.Base != "#" && (s.Off != 0 || s.VOff != 0);
            bool Fresh(Sym s) => s.Known && s.Off == 0 && s.VOff == 0;
            if (Stepped(a) && Fresh(b))
                return a;
            if (Stepped(b) && Fresh(a))
                return b;
            return Sym.Unknown;
        }

        static Sym Value(Dictionary<string, Sym> state, string op, Isa isa)
        {
            var r = Gpr(op, isa);
            if (r != null)
                return state.TryGetValue(r, out var s) ? s : new Sym(r, 0, 0);
            if (op == "xzr" || op == "wzr")
                return new Sym("#", 0, 0);
            return TryImmediate(op, isa, out var v) ? new Sym("#", v, 0) : Sym.Unknown;
        }

        /// <summary>The 128-bit-vector-length value of an SVE element count multiple ("all, mul #2" on incb is 2 × 16).</summary>
        static long SveCount(char size, string[] ops)
        {
            long mul = 1;
            foreach (var o in ops)
            {
                var m = Regex.Match(o, @"^mul\s+#(\d+)$");
                if (m.Success)
                    mul = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            var per = size switch { 'b' => 16, 'h' => 8, 'w' => 4, _ => 2 };
            return per * mul;
        }

        static void Step(Instr x, Isa isa, Dictionary<string, Sym> state)
        {
            var m = x.Mnemonic;
            var ops = x.Operands;
            if (ops.Length == 0)
                return;
            if (isa == Isa.X86)
            {
                if (x.Class == Cls.Call)
                {
                    foreach (var r in s_X86CallerSaved)
                        state[r] = Sym.Unknown;
                    return;
                }
                var dst = Gpr(ops[0], isa);
                if (dst == null || s_X86NoWrite.Contains(m) || m[0] == 'j')
                    return;
                if ((m == "mov" || m == "movsxd" || m == "movsx" || m == "movzx") && ops.Length == 2)
                {
                    state[dst] = ops[1].Contains('[', StringComparison.Ordinal) ? Sym.Unknown : Value(state, ops[1], isa);
                    return;
                }
                if (m == "lea" && ops.Length == 2)
                {
                    var lm = Regex.Match(ops[1], @"^\[\s*(\w+)\s*(?:([+-])\s*(\d+|0x[0-9a-fA-F]+))?\s*\]$");
                    if (lm.Success && Gpr(lm.Groups[1].Value, isa) != null)
                    {
                        long d = 0;
                        if (lm.Groups[2].Success)
                        {
                            TryImmediate(lm.Groups[3].Value, isa, out d);
                            if (lm.Groups[2].Value == "-")
                                d = -d;
                        }
                        state[dst] = Value(state, lm.Groups[1].Value, isa).Add(d, 0);
                    }
                    else
                        state[dst] = Sym.Unknown;
                    return;
                }
                if ((m == "add" || m == "sub") && ops.Length == 2 && TryImmediate(ops[1], isa, out var v))
                {
                    state[dst] = Value(state, ops[0], isa).Add(m == "add" ? v : -v, 0);
                    return;
                }
                if (m == "add" && ops.Length == 2 && Gpr(ops[1], isa) != null)
                {
                    state[dst] = SteppedPlusInvariant(Value(state, ops[0], isa), Value(state, ops[1], isa));
                    return;
                }
                if (m == "inc" || m == "dec")
                {
                    state[dst] = Value(state, ops[0], isa).Add(m == "inc" ? 1 : -1, 0);
                    return;
                }
                if (ops.All(o => o == ops[0]) && (m == "xor" || m == "sub"))
                {
                    state[dst] = new Sym("#", 0, 0);
                    return;
                }
                state[dst] = Sym.Unknown;
                return;
            }

            // ARM
            if (x.Class == Cls.Call)
            {
                for (var i = 0; i <= 18; i++)
                    state["x" + i] = Sym.Unknown;
                return;
            }
            if (x.Class == Cls.Branch || x.Class == Cls.Ret)
                return;
            // Writeback addressing: [xN], #imm (post-index) and [xN, #imm]! (pre-index) move the base.
            for (var i = 0; i < ops.Length; i++)
            {
                var post = Regex.Match(ops[i], @"^\[\s*(\w+)\s*\]$");
                if (post.Success && i + 1 < ops.Length && TryImmediate(ops[i + 1], isa, out var pv) && Gpr(post.Groups[1].Value, isa) is string pb)
                    state[pb] = Value(state, pb, isa).Add(pv, 0);
                var pre = Regex.Match(ops[i], @"^\[\s*(\w+)\s*,\s*#(-?\d+)\s*\]!$");
                if (pre.Success && Gpr(pre.Groups[1].Value, isa) is string qb)
                    state[qb] = Value(state, qb, isa).Add(long.Parse(pre.Groups[2].Value, CultureInfo.InvariantCulture), 0);
            }
            if (x.Store || m.StartsWith("cmp", StringComparison.Ordinal) || m == "cmn" || m == "tst" || m == "ccmp" || m == "ccmn" || m.StartsWith("prfm", StringComparison.Ordinal))
                return;
            var to = Gpr(ops[0], isa);
            if (to == null)
                return;
            var cnt = s_SveCountOp.Match(m);
            if (cnt.Success)
            {
                var n = SveCount(cnt.Groups[2].Value[0], ops);
                state[to] = Value(state, to, isa).Add(0, cnt.Groups[1].Value == "inc" ? n : -n);
                return;
            }
            if ((m == "mov" || m == "sxtw" || m == "uxtw") && ops.Length == 2)
            {
                state[to] = Value(state, ops[1], isa);
                return;
            }
            if ((m == "add" || m == "sub" || m == "adds" || m == "subs") && ops.Length == 3 && TryImmediate(ops[2], isa, out var iv))
            {
                state[to] = Value(state, ops[1], isa).Add(m.StartsWith("add", StringComparison.Ordinal) ? iv : -iv, 0);
                return;
            }
            if ((m == "add" || m == "adds") && ops.Length == 3 && Gpr(ops[2], isa) != null)
            {
                state[to] = SteppedPlusInvariant(Value(state, ops[1], isa), Value(state, ops[2], isa));
                return;
            }
            if (x.Load && ops.Length > 1 && Gpr(ops[1], isa) is string second && !ops[1].Contains('[', StringComparison.Ordinal))
                state[second] = Sym.Unknown;
            state[to] = Sym.Unknown;
        }

        static readonly Regex s_AddrScaled = new Regex(@"(\d+)\s*\*\s*(\w+)|(\w+)\s*\*\s*(\d+)", RegexOptions.Compiled);
        static readonly Regex s_AddrWord = new Regex(@"[a-z]\w*", RegexOptions.Compiled);
        static readonly Regex s_ArmShift = new Regex(@"^\[\s*(\w+)\s*,\s*(\w+)\s*(?:,\s*(?:lsl|sxtw|uxtw)\s*#(\d+))?\s*\]!?$", RegexOptions.Compiled);

        /// <summary>The scale each register addresses memory with in this operand: an index's scale, 1 for a base or an
        /// unscaled index.</summary>
        static IEnumerable<(string reg, long scale)> Addressing(string mem, Isa isa)
        {
            if (isa == Isa.X86)
            {
                var inside = mem.Substring(mem.IndexOf('[') + 1).TrimEnd(']', ' ');
                var scaled = new HashSet<string>();
                foreach (Match s in s_AddrScaled.Matches(inside))
                {
                    var reg = s.Groups[2].Success ? s.Groups[2].Value : s.Groups[3].Value;
                    var k = s.Groups[1].Success ? s.Groups[1].Value : s.Groups[4].Value;
                    if (Gpr(reg, isa) is string r)
                    {
                        scaled.Add(reg);
                        yield return (r, long.Parse(k, CultureInfo.InvariantCulture));
                    }
                }
                foreach (Match w in s_AddrWord.Matches(inside))
                    if (!scaled.Contains(w.Value) && Gpr(w.Value, isa) is string r)
                        yield return (r, 1);
                yield break;
            }
            var am = s_ArmShift.Match(mem);
            if (am.Success)
            {
                if (Gpr(am.Groups[1].Value, isa) is string b)
                    yield return (b, 1);
                if (Gpr(am.Groups[2].Value, isa) is string i)
                    yield return (i, am.Groups[3].Success ? 1L << int.Parse(am.Groups[3].Value, CultureInfo.InvariantCulture) : 1);
                yield break;
            }
            var bm = Regex.Match(mem, @"^\[\s*(\w+)");
            if (bm.Success && Gpr(bm.Groups[1].Value, isa) is string only)
                yield return (only, 1);
        }

        static void Iteration(Function f, Loop l, Isa isa)
        {
            var dom = f.Dom;
            foreach (var latch in l.Blocks.OrderBy(b => b))
            {
                if (!f.Blocks[latch].Succ.Contains(l.Header))
                    continue;
                var br = LastReal(f, f.Blocks[latch]);
                if (br == null || br.Class != Cls.Branch || !br.Conditional)
                    continue;
                if (IterationAlong(f, l, isa, Path(f, l, latch, dom), latch, br))
                    return;
            }
            Shape(l);
        }

        /// <summary>One path through the loop from its header to the latch: conditional updates on other paths aren't
        /// counted twice. At a fork it takes the successor that dominates the latch, else the first that reaches it.</summary>
        static List<int> Path(Function f, Loop l, int latch, BitArray[] dom)
        {
            var path = new List<int> { l.Header };
            var at = l.Header;
            var seen = new HashSet<int> { l.Header };
            while (at != latch)
            {
                var next = f.Blocks[at].Succ.Where(s => s != l.Header && l.Blocks.Contains(s) && !seen.Contains(s) && (s == latch || ReachesWithin(f, l, s, latch)))
                            .OrderByDescending(s => dom != null && dom[latch][s]).ThenBy(s => s).ToList();
                if (next.Count == 0)
                    break;
                at = next[0];
                seen.Add(at);
                path.Add(at);
            }
            return path;
        }

        static bool ReachesWithin(Function f, Loop l, int from, int to)
        {
            var seen = new HashSet<int> { l.Header };
            var work = new Stack<int>();
            work.Push(from);
            while (work.Count > 0)
            {
                var b = work.Pop();
                if (b == to)
                    return true;
                if (!seen.Add(b) || !l.Blocks.Contains(b))
                    continue;
                foreach (var s in f.Blocks[b].Succ)
                    work.Push(s);
            }
            return false;
        }

        static bool IterationAlong(Function f, Loop l, Isa isa, List<int> path, int latch, Instr br)
        {
            var state = new Dictionary<string, Sym>(StringComparer.Ordinal);
            var before = new Dictionary<Instr, Dictionary<string, Sym>>();
            var walk = new List<Instr>();
            foreach (var b in path)
                for (var i = f.Blocks[b].First; i <= f.Blocks[b].Last; i++)
                {
                    var x = f.Instrs[i];
                    if (x.Mnemonic == ":")
                        continue;
                    before[x] = new Dictionary<string, Sym>(state, StringComparer.Ordinal);
                    walk.Add(x);
                    Step(x, isa, state);
                }
            if (!before.ContainsKey(br))
                return false;

            Sym At(Instr x, string reg, bool after)
            {
                var s = new Dictionary<string, Sym>(before[x], StringComparer.Ordinal);
                if (after)
                    Step(x, isa, s);
                return Value(s, reg, isa);
            }

            // Induction variables: registers that come back to themselves plus a step.
            var steps = new Dictionary<string, (long step, bool scalable)>(StringComparer.Ordinal);
            foreach (var (reg, sym) in state)
                if (sym.Base == reg && (sym.Off != 0 || sym.VOff != 0))
                    steps[reg] = (Math.Abs(sym.VOff != 0 ? sym.VOff : sym.Off), sym.VOff != 0);

            var candidates = new List<(Instr at, string reg, bool after)>();
            if (isa != Isa.X86 && (br.Mnemonic.StartsWith("cb", StringComparison.Ordinal) || br.Mnemonic.StartsWith("tb", StringComparison.Ordinal)) && br.Operands.Length > 0)
                candidates.Add((br, br.Operands[0], false));
            else
            {
                var flag = FlagSetter(f, f.Blocks[latch], br, isa);
                if (flag == null || !before.ContainsKey(flag))
                    return false;
                var compare = flag.Mnemonic == "cmp" || flag.Mnemonic == "test" || flag.Mnemonic == "cmn";
                var ops = flag.Mnemonic.StartsWith("while", StringComparison.Ordinal) ? flag.Operands.Skip(1) : compare ? flag.Operands : flag.Operands.Take(1);
                foreach (var o in ops)
                    candidates.Add((flag, o, !compare));
            }
            // The counter test can be an exit branch rather than the back edge (cmp r9, r10; jge out in the header).
            foreach (var b in path)
            {
                var ex = LastReal(f, f.Blocks[b]);
                if (ex == null || ex == br || ex.Class != Cls.Branch || !ex.Conditional || f.Blocks[b].Succ.All(s => l.Blocks.Contains(s)))
                    continue;
                var flag = FlagSetter(f, f.Blocks[b], ex, isa);
                if (flag == null || !before.ContainsKey(flag) || !(flag.Mnemonic == "cmp" || flag.Mnemonic == "test"))
                    continue;
                foreach (var o in flag.Operands)
                    candidates.Add((flag, o, false));
            }
            string counter = null;
            long step = 0;
            var scalable = false;
            foreach (var (at, reg, after) in candidates)
            {
                if (Gpr(reg, isa) == null)
                    continue;
                var sym = At(at, reg, after);
                if (!sym.Known || !steps.TryGetValue(sym.Base, out var s))
                    continue;
                counter = sym.Base;
                (step, scalable) = s;
                break;
            }
            if (counter == null || step == 0)
                return false;

            // Every stream an induction variable addresses: the bytes it touches per pass, against its own stride.
            long scale = 0;
            var counterScales = new HashSet<long>();
            var streams = new Dictionary<string, (long stride, List<(long from, long to)> spans, HashSet<long> vectorOffsets)>(StringComparer.Ordinal);
            var vectorBytes = l.Stats.PackedWidth / 8;
            foreach (var x in walk)
            {
                if (!(x.Load || x.Store))
                    continue;
                var mem = x.Operands.FirstOrDefault(o => o.Contains('[', StringComparison.Ordinal));
                if (mem == null)
                    continue;
                var regs = Addressing(mem, isa).ToList();
                foreach (var hit in regs)
                {
                    var v = Value(before[x], hit.reg, isa);
                    if (!v.Known || !steps.TryGetValue(v.Base, out var iv))
                        continue;
                    if (v.Base == counter)
                    {
                        if (scale == 0)
                            scale = hit.scale;
                        counterScales.Add(hit.scale);
                    }
                    var size = AccessBytes(x, isa);
                    if (size <= 0)
                        break;
                    var at = Displacement(mem, isa) + v.Off * hit.scale;
                    var key = v.Base + "|" + string.Join(",", regs.Where(r => r.reg != hit.reg).Select(r => r.reg + "*" + r.scale)) + "|" + hit.scale;
                    if (!streams.TryGetValue(key, out var st))
                        streams[key] = st = (iv.step * hit.scale, new List<(long, long)>(), new HashSet<long>());
                    st.spans.Add((at, at + size));
                    if (vectorBytes > 0 && size == vectorBytes)
                        st.vectorOffsets.Add(at);
                    break;
                }
            }
            // A pure counter counts source iterations, and an index scaled by its element size counts elements; a
            // pointer, or an unscaled index, counts bytes.
            if (counterScales.Count > 1)
                scale = counterScales.Min();
            long elems;
            if (scale != 1)
                elems = step;
            else
            {
                elems = step;
                l.PerIterBytes = true;
            }
            l.PerIterScalable = scalable;
            l.ElemsPerIter = (int)Math.Min(int.MaxValue, elems);
            // A stream that touches well under its stride each pass walks a struct, one element per pass: whatever packs
            // is within that element. Under half, since accesses through pointers derived from the counter aren't seen.
            if (streams.Values.Any(s => !scalable && 2 * Covered(s.spans) < s.stride))
            {
                l.Strided = true;
                if (scale > 0)
                {
                    // The struct's size: the stride of the stream that reads only part of it.
                    var structStride = streams.Values.Where(st => 2 * Covered(st.spans) < st.stride).Max(st => st.stride);
                    l.ElemsPerIter = (int)Math.Min(int.MaxValue, structStride);
                    l.PerIterBytes = true;
                }
                else
                    l.ElemsPerIter = (int)Math.Min(int.MaxValue, step);
            }
            // Full-vector accesses at distinct offsets in one stream are the unroll (four ymm loads of a byte mask: 4).
            l.StreamVectors = streams.Values.Select(s => s.vectorOffsets.Count).DefaultIfEmpty(0).Max();
            Shape(l);
            return true;
        }

        /// <summary>Bytes covered by the union of the spans.</summary>
        static long Covered(List<(long from, long to)> spans)
        {
            long total = 0, end = long.MinValue;
            foreach (var (from, to) in spans.OrderBy(s => s.from))
            {
                var start = Math.Max(from, end);
                if (to > start)
                    total += to - start;
                end = Math.Max(end, to);
            }
            return total;
        }

        static long Displacement(string mem, Isa isa)
        {
            long d = 0;
            var inside = mem.Substring(mem.IndexOf('[') + 1);
            inside = inside.Substring(0, Math.Max(0, inside.IndexOf(']')));
            if (isa == Isa.X86)
            {
                foreach (Match m in Regex.Matches(inside, @"([+-])\s*(0x[0-9a-fA-F]+|\d+)(?!\s*\*)"))
                {
                    TryImmediate(m.Groups[2].Value, isa, out var v);
                    if (Regex.IsMatch(inside.Substring(m.Index + m.Length), @"^\s*\*"))
                        continue;
                    d += m.Groups[1].Value == "-" ? -v : v;
                }
                return d;
            }
            var arm = Regex.Match(inside, @",\s*#(-?\d+)(\s*,\s*mul\s+vl)?\s*$");
            if (arm.Success)
                d = long.Parse(arm.Groups[1].Value, CultureInfo.InvariantCulture) * (arm.Groups[2].Success ? 16 : 1);
            return d;
        }

        static readonly Regex s_PtrSize = new Regex(@"^(byte|word|dword|qword|xmmword|ymmword|zmmword)\s+ptr", RegexOptions.Compiled);

        /// <summary>The bytes a load or store moves (SVE at a 128-bit vector length), 0 when not known.</summary>
        static long AccessBytes(Instr x, Isa isa)
        {
            if (isa == Isa.X86)
            {
                var mem = x.Operands.FirstOrDefault(o => o.Contains('[', StringComparison.Ordinal));
                var m = mem == null ? Match.Empty : s_PtrSize.Match(mem.Trim());
                if (!m.Success)
                    return 0;
                return m.Groups[1].Value switch { "byte" => 1, "word" => 2, "dword" => 4, "qword" => 8, "xmmword" => 16, "ymmword" => 32, _ => 64 };
            }
            long per = 0;
            var count = 0;
            foreach (var o in x.Operands)
            {
                if (o.Contains('[', StringComparison.Ordinal))
                    break;
                var r = o.Trim('{', '}', ' ');
                long b = r[0] switch
                {
                    'q' or 'z' => 16,
                    'd' or 'x' => 8,
                    's' or 'w' => 4,
                    'h' => 2,
                    'b' => 1,
                    'v' => Regex.IsMatch(r, @"\.(16b|8h|4s|2d)") ? 16 : 8,
                    _ => 0,
                };
                if (b == 0 || r.Length < 2 || !char.IsDigit(r[1]))
                    continue;
                per = b;
                count++;
            }
            return per * count;
        }

        static int? LaneBits(int width, int lanesPerRegister) =>
            lanesPerRegister > 0 && width % lanesPerRegister == 0 && width / lanesPerRegister is 8 or 16 or 32 or 64 ? width / lanesPerRegister : null;

        static int Lanes(LoopStats s) => s.PackedWidth > 0 && s.Lane > 0 ? s.PackedWidth / s.Lane : 0;

        /// <summary>
        /// A loop stepped by a pointer's bytes is loop-vectorized, with one lane per source iteration, when the loop
        /// vectorizer's other marks are there: a scalar loop after it in the same function for the leftovers, or a guard
        /// requiring exactly the lanes one pass covers.
        /// </summary>
        static void ByteStepEvidence(Function f, Loop l)
        {
            var s = l.Stats;
            if (l.Shape != Asm.Shape.Packed || !l.PerIterBytes)
                return;
            // The loop vectorizer's guard usually asks for exactly the elements one pass covers (n >= 32 for 32 per
            // pass), which also settles mixed element sizes (int32 in, ushort out); else bytes over the lane size.
            long lanes;
            if (l.MinCount > 1 && l.ElemsPerIter % l.MinCount == 0 && l.MinCountScalable == l.PerIterScalable && l.MinCountExtra == 0 &&
                (s.Lane == 0 || l.MinCount >= l.ElemsPerIter / (s.Lane / 8) / 4))
                lanes = l.MinCount;
            else if (s.Lane > 0 && l.ElemsPerIter % (s.Lane / 8) == 0)
                lanes = l.ElemsPerIter / (s.Lane / 8);
            else
                return;
            // The scalar loop can sit before the vector loop when it also serves as the alias check's fallback.
            var remainder = f.Loops.Any(o => o != l && o.Innermost && o.Depth == l.Depth &&
                                             o.Stats.PackedFloat + o.Stats.PackedInt + o.Stats.VectorMathCalls == 0);
            if (!remainder && !(l.MinCount == lanes && l.MinCountScalable == l.PerIterScalable))
                return;
            if (l.StreamVectors > 1 && lanes % l.StreamVectors == 0 && LaneBits(s.PackedWidth, (int)(lanes / l.StreamVectors)) is int bits)
            {
                l.Shape = Asm.Shape.Vector;
                l.PerIterBytes = false;
                l.ElemsPerIter = (int)lanes;
                l.Unroll = l.StreamVectors;
                s.Lane = bits;
                return;
            }
            var perVector = Lanes(s);
            if (perVector == 0 || lanes % perVector != 0 || lanes / perVector > 16)
                return;
            l.Shape = Asm.Shape.Vector;
            l.PerIterBytes = false;
            l.ElemsPerIter = (int)lanes;
            l.Unroll = (int)(lanes / perVector);
        }

        /// <summary>
        /// The loop vectorizer leaves marks: a scalar loop beside the vector loop for the leftovers (or the alias
        /// fallback), a guard requiring the lanes one pass covers, or a predicated SVE loop. A vector-shaped loop with
        /// none is packed by hand, one source iteration per pass (a v256 or float4 loop): SLP.
        /// </summary>
        static void VectorizerEvidence(Function f, Loop l)
        {
            if (l.Shape != Asm.Shape.Vector || !l.Innermost)
                return;
            var scalarBeside = f.Loops.Any(o => o != l && o.Innermost && o.Depth == l.Depth && o.Stats.PackedFloat + o.Stats.PackedInt + o.Stats.VectorMathCalls == 0);
            var guard = l.MinCount >= l.ElemsPerIter && l.MinCount > 1;
            var predicated = f.Instrs.Any(x => InLoop(f, l, x) && x.Mnemonic.StartsWith("while", StringComparison.Ordinal));
            var vectorBeside = f.Loops.Any(o => o != l && o.Innermost && o.Depth == l.Depth && o.Shape == Asm.Shape.Vector && o.ElemsPerIter != l.ElemsPerIter);
            if (scalarBeside || guard || predicated || vectorBeside || l.Stats.VectorMathCalls > 0)
                return;
            l.Shape = Asm.Shape.Slp;
            l.HandPacked = true;
        }

        /// <summary>Whether leaving the vector loop leads into the scalar loop laid out before it (its remainder).</summary>
        static bool ExitsInto(Function f, Loop v, Loop s)
        {
            if (s.Stats.FirstLine > v.Stats.FirstLine)
                return false;
            foreach (var b in v.Blocks)
                foreach (var succ in f.Blocks[b].Succ)
                    if (!v.Blocks.Contains(succ) && Reaches(f, succ, s.Header, v.Header))
                        return true;
            return false;
        }

        /// <summary>Vector when one pass covers a whole number of vectors of elements (at most 16); SLP when it covers one
        /// element (or a pointer moves less than a vector per pass); Packed when the step doesn't say.</summary>
        static void Shape(Loop l)
        {
            var s = l.Stats;
            var lanes = Lanes(s);
            l.Unroll = 1;
            // An outer loop is judged by its own instructions, not its inner loops'.
            if (s.PackedFloat + s.PackedInt + s.VectorMathCalls == 0 && !s.Copy || !l.Innermost && l.OwnPacked == 0)
                l.Shape = Asm.Shape.Scalar;
            else if (l.ElemsPerIter == 0)
                l.Shape = Asm.Shape.Packed;
            else if (l.PerIterBytes)
                l.Shape = l.Strided || l.ElemsPerIter < s.PackedWidth / 8 ? Asm.Shape.Slp : Asm.Shape.Packed;
            else if (l.ElemsPerIter == 1)
                l.Shape = Asm.Shape.Slp;
            else if (!l.PerIterBytes && l.StreamVectors > 1 && l.ElemsPerIter % l.StreamVectors == 0 && LaneBits(s.PackedWidth, l.ElemsPerIter / l.StreamVectors) is int bits && (s.Lane == 0 || bits < s.Lane))
            {
                // More elements per register than the math's lane size says: byte or short data handled with 32-bit
                // ops (vorps on a byte mask). The stream's vector count is the unroll.
                l.Shape = Asm.Shape.Vector;
                l.Unroll = l.StreamVectors;
                s.Lane = bits;
            }
            else if (lanes > 0 && l.ElemsPerIter % lanes == 0 && l.ElemsPerIter / lanes <= 16)
            {
                l.Shape = Asm.Shape.Vector;
                l.Unroll = l.ElemsPerIter / lanes;
            }
            else
                l.Shape = Asm.Shape.Packed;
        }

        static void PickBody(AsmFile file)
        {
            var real = file.Functions.Where(f => !f.Runtime).ToList();
            file.Entry = real.FirstOrDefault(f => f.Exported) ??
                         real.FirstOrDefault(f => Regex.IsMatch(f.Name, "^[0-9a-f]{32}$")) ??
                         real.FirstOrDefault();
            file.Body = file.Entry;
            if (file.Entry == null)
                return;
            var calls = file.Entry.Instrs.Where(x => x.Class == Cls.Call).ToList();
            if (file.Entry.Stats.Instrs <= 12 && calls.Count == 1 && calls[0].Target != null)
            {
                var callee = file.Functions.FirstOrDefault(f => f.Name == calls[0].Target);
                if (callee != null)
                    file.Body = callee;
            }
        }

        static readonly Regex s_Math = new Regex(@"(?i)sleef_|(^|[._])(sin|cos|tan|asin|acos|atan|atan2|exp|exp2|exp10|expm1|log|log2|log10|log1p|pow|sinh|cosh|tanh|asinh|acosh|atanh|cbrt|fmod|hypot|erf|erfc|sincos|sqrt|rsqrt)f?($|[_@])", RegexOptions.Compiled);
        static readonly Regex s_SleefVector = new Regex(@"(?i)sleef_[a-z0-9]*?[fd](\d+)_", RegexOptions.Compiled);

        public static string CallKind(string name)
        {
            if (name == null || name.Contains("ptr [", StringComparison.Ordinal) || name.StartsWith("[", StringComparison.Ordinal) || s_Gpr64.Contains(name) ||
                Regex.IsMatch(name, "^[xw]\\d+$"))
                return "indirect";
            if (name.Contains('(', StringComparison.Ordinal))
                return "method";
            if (s_Math.IsMatch(name))
                return s_SleefVector.IsMatch(name) ? "vector math" : "scalar math";
            if (Regex.IsMatch(name, "(?i)abort|throw|exception|panic"))
                return "throw";
            return "runtime";
        }

        static void CollectCalls(AsmFile file)
        {
            if (file.Body == null)
                return;
            var seen = new HashSet<Function>();
            var work = new Queue<Function>();
            work.Enqueue(file.Body);
            var byName = new Dictionary<string, Call>(StringComparer.Ordinal);
            while (work.Count > 0)
            {
                var f = work.Dequeue();
                if (!seen.Add(f))
                    continue;
                foreach (var x in f.Instrs.Where(i => i.Class == Cls.Call))
                {
                    var name = x.Target ?? x.Operands.FirstOrDefault() ?? "?";
                    var callee = file.Functions.FirstOrDefault(g => g.Name == name);
                    if (callee != null && !seen.Contains(callee))
                        work.Enqueue(callee);
                    // Calls made inside library routines (Sleef's own helpers) aren't the method's own calls.
                    if (f != file.Body && !IsUserFunction(f))
                        continue;
                    if (!byName.TryGetValue(name, out var c))
                    {
                        c = new Call { Name = name, Display = Names.Pretty(name), Kind = CallKind(name) };
                        byName[name] = c;
                        file.Calls.Add(c);
                    }
                    c.Count++;
                    c.From.Add(f == file.Body && !IsUserFunction(f) ? "entry" : Names.Short(f.Display));
                    var loop = f.Loops.Where(l => InLoop(f, l, x)).OrderByDescending(l => l.Depth).FirstOrDefault();
                    if (loop != null)
                    {
                        c.InLoop = true;
                        c.Loops.Add(loop.Id);
                    }
                    if (loop == null || !loop.Remainder)
                        c.OutsideRemainder++;
                }
            }
        }

        /// <summary>The body and every function it reaches through calls defined in the dump.</summary>
        public static List<Function> Reached(AsmFile file)
        {
            var list = new List<Function>();
            if (file.Body == null)
                return list;
            var work = new Queue<Function>();
            work.Enqueue(file.Body);
            while (work.Count > 0)
            {
                var f = work.Dequeue();
                if (list.Contains(f))
                    continue;
                list.Add(f);
                foreach (var x in f.Instrs.Where(i => i.Class == Cls.Call && i.Target != null))
                {
                    var callee = file.Functions.FirstOrDefault(g => g.Name == x.Target);
                    if (callee != null)
                        work.Enqueue(callee);
                }
            }
            return list;
        }
    }
}
