using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BurstWaitress.Asm
{
    /// <summary>Sorts each instruction into a census class and notes its memory access, vector width and branch target.</summary>
    public static class Classifier
    {
        public static void Classify(AsmFile file)
        {
            foreach (var f in file.Functions)
                foreach (var x in f.Instrs)
                {
                    if (x.Mnemonic == ":")
                    {
                        x.Class = Cls.Other;
                        continue;
                    }
                    switch (file.Isa)
                    {
                        case Isa.X86:
                            X86(x);
                            break;
                        case Isa.Arm64:
                        case Isa.Arm32:
                            Arm(x, file.Isa);
                            break;
                        default:
                            Wasm(x);
                            break;
                    }
                }
        }

        // ---------------------------------------------------------------------------------------------------------------
        // x86 (Intel syntax)

        static readonly HashSet<string> s_IntAlu = new HashSet<string>
        {
            "add", "sub", "imul", "mul", "div", "idiv", "inc", "dec", "neg", "not", "and", "or", "xor", "shl", "shr", "sar", "sal", "rol", "ror",
            "rcl", "rcr", "adc", "sbb", "lea", "cmp", "test", "bt", "bts", "btr", "btc", "andn", "bextr", "blsi", "blsr", "blsmsk", "bzhi", "lzcnt",
            "tzcnt", "popcnt", "shlx", "shrx", "sarx", "rorx", "mulx", "pdep", "pext", "shld", "shrd", "cdq", "cqo", "cdqe", "cwde", "cwd", "bswap",
            "xadd", "cmpxchg", "bsf", "bsr",
        };

        static readonly HashSet<string> s_ReadModifyWrite = new HashSet<string>
        {
            "add", "sub", "and", "or", "xor", "inc", "dec", "neg", "not", "shl", "shr", "sar", "sal", "adc", "sbb", "xadd", "rol", "ror", "bts", "btr", "btc", "cmpxchg",
        };

        static readonly HashSet<string> s_ReadOnlyMem = new HashSet<string> { "cmp", "test", "bt", "push", "call", "jmp" };

        static readonly HashSet<string> s_VectorMoves = new HashSet<string>
        {
            "movaps", "movups", "movapd", "movupd", "movntps", "movntpd", "movdqa", "movdqu", "movntdq", "movntdqa", "lddqu", "movhps", "movlps",
            "movhpd", "movlpd", "movddup", "movsldup", "movshdup", "broadcastss", "broadcastsd", "broadcastf128", "broadcasti128", "movdqa32",
            "movdqa64", "movdqu32", "movdqu64", "movdqu8", "movdqu16",
        };

        static readonly HashSet<string> s_ScalarMoves = new HashSet<string> { "movss", "movsd", "movd", "movq" };

        /// <summary>The stack slot key a memory operand names ("rbp-20"), or null.</summary>
        public static string StackSlotOf(string mem)
        {
            var slot = s_StackSlot.Match(mem);
            return slot.Success ? slot.Groups[1].Value + (slot.Groups[2].Success ? slot.Groups[2].Value + slot.Groups[3].Value : "+0") : null;
        }

        static readonly Regex s_StackSlot = new Regex(@"\[\s*(rsp|rbp)\s*(?:([+-])\s*(\d+))?\s*\]", RegexOptions.Compiled);

        static void X86(Instr x)
        {
            var m = x.Mnemonic;
            var ops = x.Operands;
            if (m == "lock" || m == "rep" || m == "repe" || m == "repne")
            {
                x.Class = Cls.Other;
                return;
            }
            if (m == "ret")
            {
                x.Class = Cls.Ret;
                return;
            }
            if (m == "call")
            {
                x.Class = Cls.Call;
                x.Target = ops.Length > 0 ? AsmParser.Unquote(ops[0]) : null;
                Memory(x);
                return;
            }
            if (m[0] == 'j')
            {
                x.Conditional = m != "jmp";
                var t = ops.Length > 0 ? AsmParser.Unquote(ops[0]) : null;
                x.Target = t;
                if (!x.Conditional && t != null && !t.StartsWith(".L", StringComparison.Ordinal))
                {
                    x.Class = Cls.Call;
                    x.TailCall = true;
                }
                else
                    x.Class = Cls.Branch;
                Memory(x);
                return;
            }
            // xor x, x and friends only zero a register.
            if (ops.Length >= 2 && ops.All(o => o == ops[0]) && (m.Contains("xor", StringComparison.Ordinal) || m == "sub" || m == "vpsubd" || m == "vpsubq"))
            {
                x.Class = Cls.Other;
                x.ZeroIdiom = true;
                return;
            }
            var b = m.Length > 1 && m[0] == 'v' && m != "vzeroupper" && m != "vzeroall" ? m.Substring(1) : m;
            if (b.StartsWith("fmadd", StringComparison.Ordinal) || b.StartsWith("fmsub", StringComparison.Ordinal) ||
                b.StartsWith("fnmadd", StringComparison.Ordinal) || b.StartsWith("fnmsub", StringComparison.Ordinal))
                x.Fma = true;

            if (m.StartsWith("vpermil", StringComparison.Ordinal) || m == "vpermps" || m == "vpermpd" || m.StartsWith("vperm2f", StringComparison.Ordinal) ||
                b == "insertf128" || b == "extractf128" || b.StartsWith("insertf", StringComparison.Ordinal) || b.StartsWith("extractf", StringComparison.Ordinal) ||
                b.StartsWith("shuff", StringComparison.Ordinal))
                x.Class = Cls.PackedFloat;
            else if (s_VectorMoves.Contains(b) || b.StartsWith("pbroadcast", StringComparison.Ordinal))
                x.Class = Cls.Move;
            else if (b.StartsWith("pinsr", StringComparison.Ordinal) || b.StartsWith("pextr", StringComparison.Ordinal))
                x.Class = Cls.Move;
            else if (b == "inserti128" || b == "extracti128" || b.StartsWith("inserti", StringComparison.Ordinal) || b.StartsWith("extracti", StringComparison.Ordinal) ||
                     m.StartsWith("vp", StringComparison.Ordinal) || IsSsePackedInt(m))
                x.Class = Cls.PackedInt;
            else if (s_ScalarMoves.Contains(b))
                x.Class = Cls.Move;
            else if (b.StartsWith("cvt", StringComparison.Ordinal) && (b.Contains("ss", StringComparison.Ordinal) || b.Contains("sd", StringComparison.Ordinal) || b.Contains("si2", StringComparison.Ordinal)))
                x.Class = Cls.ScalarFloat;
            else if (b.StartsWith("cvt", StringComparison.Ordinal))
                x.Class = Cls.PackedFloat;
            else if (b.EndsWith("ps", StringComparison.Ordinal) || b.EndsWith("pd", StringComparison.Ordinal) || b.Contains("movmskp", StringComparison.Ordinal))
                x.Class = Cls.PackedFloat;
            else if (b.EndsWith("ss", StringComparison.Ordinal) || b.EndsWith("sd", StringComparison.Ordinal))
                x.Class = Cls.ScalarFloat;
            else if (s_IntAlu.Contains(m) || m.StartsWith("set", StringComparison.Ordinal) || m.StartsWith("cmov", StringComparison.Ordinal))
                x.Class = Cls.ScalarInt;
            else if (m.StartsWith("mov", StringComparison.Ordinal) || m.StartsWith("vmov", StringComparison.Ordinal) || m == "push" || m == "pop" || m == "xchg")
                x.Class = Cls.Move;
            else
                x.Class = Cls.Other;

            if (x.Class == Cls.PackedFloat || x.Class == Cls.PackedInt || x.Class == Cls.Move && s_VectorMoves.Contains(b) || b.StartsWith("pbroadcast", StringComparison.Ordinal))
            {
                x.Width = ops.Select(RegWidth).DefaultIfEmpty(0).Max();
                if (x.Class == Cls.PackedFloat)
                    x.Lane = b.EndsWith("pd", StringComparison.Ordinal) ? 64 : 32;
                else if (x.Class == Cls.PackedInt)
                    x.Lane = IntLane(b);
            }
            Memory(x);
        }

        static bool IsSsePackedInt(string m) =>
            m.Length > 2 && m[0] == 'p' && m != "push" && m != "pop" && m != "popcnt" && m != "pause" && m != "pdep" && m != "pext" &&
            !m.StartsWith("prefetch", StringComparison.Ordinal) && !m.StartsWith("pushf", StringComparison.Ordinal) && !m.StartsWith("popf", StringComparison.Ordinal);

        static int IntLane(string b)
        {
            if (b == "pand" || b == "pandn" || b == "por" || b == "pxor" || b == "ptest" || b.StartsWith("pternlog", StringComparison.Ordinal))
                return 0;
            var c = b[b.Length - 1];
            if (b.EndsWith("dq", StringComparison.Ordinal) && !b.StartsWith("punpck", StringComparison.Ordinal))
                return 64;
            return c switch
            {
                'b' => 8,
                'w' => 16,
                'd' => 32,
                'q' => 64,
                _ => 0,
            };
        }

        static int RegWidth(string op)
        {
            if (op.Contains("zmm", StringComparison.Ordinal))
                return 512;
            if (op.Contains("ymm", StringComparison.Ordinal))
                return 256;
            if (op.Contains("xmm", StringComparison.Ordinal))
                return 128;
            return 0;
        }

        static void Memory(Instr x)
        {
            var m = x.Mnemonic;
            var ops = x.Operands;
            if (m == "lea" || m == "nop" || m.StartsWith("prefetch", StringComparison.Ordinal))
                return;
            if (m == "push")
            {
                x.Store = true;
                return;
            }
            if (m == "pop")
            {
                x.Load = true;
                return;
            }
            var memIndex = Array.FindIndex(ops, o => o.Contains('[', StringComparison.Ordinal));
            if (memIndex < 0)
                return;
            if (memIndex == 0 && !s_ReadOnlyMem.Contains(m) && !m.StartsWith("cmp", StringComparison.Ordinal) && !m.StartsWith("vcmp", StringComparison.Ordinal) &&
                !m.StartsWith("ucomis", StringComparison.Ordinal) && !m.StartsWith("vucomis", StringComparison.Ordinal) && !m.StartsWith("comis", StringComparison.Ordinal) &&
                !m.StartsWith("vcomis", StringComparison.Ordinal) && ops.Length > 1)
            {
                x.Store = true;
                x.Load = s_ReadModifyWrite.Contains(m);
            }
            else
                x.Load = true;
            var slot = s_StackSlot.Match(ops[memIndex]);
            if (slot.Success)
                x.StackSlot = slot.Groups[1].Value + (slot.Groups[2].Success ? slot.Groups[2].Value + slot.Groups[3].Value : "+0");
        }

        // ---------------------------------------------------------------------------------------------------------------
        // ARM (AArch64 and ARMv7/Thumb2 NEON)

        static readonly Regex s_ArmVector = new Regex(@"^v\d+\.(\d+)([bhsd])$|^v\d+\.([bhsd])\[|^q\d+$", RegexOptions.Compiled);
        static readonly Regex s_ArmFloatReg = new Regex(@"^[hsd]\d+$", RegexOptions.Compiled);
        static readonly Regex s_ArmStack = new Regex(@"\[\s*(sp|x29|fp)\s*(?:,\s*#(-?\d+))?\s*\]", RegexOptions.Compiled);

        static readonly HashSet<string> s_ArmIntAlu = new HashSet<string>
        {
            "add", "adds", "sub", "subs", "mul", "madd", "msub", "smull", "umull", "smulh", "umulh", "sdiv", "udiv", "and", "ands", "orr", "orn", "eor",
            "eon", "bic", "bics", "lsl", "lsr", "asr", "ror", "cmp", "cmn", "tst", "csel", "csinc", "csinv", "csneg", "cset", "csetm", "cinc", "cneg",
            "neg", "negs", "mvn", "ubfx", "sbfx", "bfi", "bfxil", "ubfiz", "sbfiz", "sxtw", "sxth", "sxtb", "uxtw", "uxth", "uxtb", "clz", "rbit",
            "rev", "adr", "adrp", "ccmp", "ccmn", "extr", "mla", "mls", "mneg", "smaddl", "umaddl",
        };

        static void Arm(Instr x, Isa isa)
        {
            var m = x.Mnemonic;
            var ops = x.Operands;
            var dot = m.IndexOf('.');
            var stem = dot > 0 && isa == Isa.Arm32 ? m.Substring(0, dot) : m;
            if (m == "ret" || isa == Isa.Arm32 && (m == "bx" && ops.Length == 1 && ops[0] == "lr" || m.StartsWith("pop", StringComparison.Ordinal) && ops.Any(o => o.Contains("pc", StringComparison.Ordinal))))
            {
                x.Class = Cls.Ret;
                x.Load = m.StartsWith("pop", StringComparison.Ordinal);
                return;
            }
            if (m == "bl" || m == "blr" || m == "blx")
            {
                x.Class = Cls.Call;
                x.Target = m == "bl" && ops.Length > 0 ? AsmParser.Unquote(ops[0]) : null;
                return;
            }
            if (m == "b" || m.StartsWith("b.", StringComparison.Ordinal) || m == "cbz" || m == "cbnz" || m == "tbz" || m == "tbnz" || m == "br" ||
                isa == Isa.Arm32 && IsArm32CondBranch(m))
            {
                x.Conditional = m != "b" && m != "br";
                x.Target = m == "br" ? null : AsmParser.Unquote(ops.Length > 0 ? ops[ops.Length - 1] : "");
                if (m == "b" && x.Target != null && !x.Target.StartsWith(".L", StringComparison.Ordinal))
                {
                    x.Class = Cls.Call;
                    x.TailCall = true;
                }
                else
                    x.Class = Cls.Branch;
                return;
            }
            if (m == "fmla" || m == "fmls" || m == "fmadd" || m == "fmsub" || m == "fnmadd" || m == "fnmsub" || m == "fmad" || m == "fmsb" || m == "fnmla" || m == "fnmls" ||
                stem == "vfma" || stem == "vfms" || stem == "vmla" && m.Contains("f32", StringComparison.Ordinal))
                x.Fma = true;
            if (isa == Isa.Arm64 && Sve(x))
                return;
            var vector = ops.Select(o => s_ArmVector.Match(o.Trim('{', '}', ' '))).FirstOrDefault(r => r.Success);
            var arm32Vector = isa == Isa.Arm32 && ops.Any(o => o.StartsWith("q", StringComparison.Ordinal) && o.Length > 1 && char.IsDigit(o[1]) ||
                                                                   o.StartsWith("d", StringComparison.Ordinal) && o.Length > 1 && char.IsDigit(o[1]) && m.Contains(".f32", StringComparison.Ordinal) && !m.Contains(".f64", StringComparison.Ordinal));
            var isLoad = stem.StartsWith("ld", StringComparison.Ordinal) || stem.StartsWith("vld", StringComparison.Ordinal) || stem == "pop" || stem == "vpop";
            var isStore = stem.StartsWith("st", StringComparison.Ordinal) && stem != "sturb" || stem.StartsWith("vst", StringComparison.Ordinal) || stem == "push" || stem == "vpush";
            if (stem.StartsWith("sturb", StringComparison.Ordinal))
                isStore = true;
            if (isLoad || isStore)
            {
                x.Class = Cls.Move;
                x.Load = isLoad;
                x.Store = isStore;
                if (vector != null || arm32Vector)
                    x.Width = 128;
                var mem = ops.FirstOrDefault(o => o.Contains('[', StringComparison.Ordinal));
                if (mem != null)
                {
                    var slot = s_ArmStack.Match(mem);
                    if (slot.Success && !mem.Contains("]!", StringComparison.Ordinal))
                        x.StackSlot = slot.Groups[1].Value + (slot.Groups[2].Success ? slot.Groups[2].Value : "0");
                }
                return;
            }
            if (vector != null || arm32Vector)
            {
                x.Width = 128;
                if (vector != null && vector.Groups[1].Success)
                {
                    var lanes = int.Parse(vector.Groups[1].Value);
                    var bits = vector.Groups[2].Value switch { "b" => 8, "h" => 16, "s" => 32, _ => 64 };
                    x.Width = lanes * bits;
                    x.Lane = bits;
                }
                var floaty = m[0] == 'f' || m.StartsWith("scvtf", StringComparison.Ordinal) || m.StartsWith("ucvtf", StringComparison.Ordinal) || m.Contains(".f32", StringComparison.Ordinal) || m.Contains(".f64", StringComparison.Ordinal);
                if (m == "mov" || m == "dup" || m == "ins" || m == "umov" || m == "smov" || m == "movi" || stem == "vmov" || stem == "vdup")
                    x.Class = Cls.Move;
                else if (floaty)
                {
                    x.Class = Cls.PackedFloat;
                    if (x.Lane == 0)
                        x.Lane = m.Contains("f64", StringComparison.Ordinal) ? 64 : 32;
                }
                else
                    x.Class = Cls.PackedInt;
                return;
            }
            var floatRegs = ops.Any(o => s_ArmFloatReg.IsMatch(o));
            if (floatRegs && (m[0] == 'f' || m.StartsWith("scvtf", StringComparison.Ordinal) || m.StartsWith("ucvtf", StringComparison.Ordinal) || stem.StartsWith("v", StringComparison.Ordinal) && m.Contains(".f", StringComparison.Ordinal)))
            {
                x.Class = m == "fmov" || stem == "vmov" ? Cls.Move : Cls.ScalarFloat;
                return;
            }
            if (m == "mov" || m == "movz" || m == "movk" || m == "movn" || m == "movw" || m == "movt" || m == "fmov" || stem == "vmov")
                x.Class = Cls.Move;
            else if (s_ArmIntAlu.Contains(stem) || s_ArmIntAlu.Contains(StripArm32Cond(stem)))
                x.Class = Cls.ScalarInt;
            else
                x.Class = Cls.Other;
        }

        static readonly Regex s_SveVector = new Regex(@"^z\d+(?:\.([bhsdq]))?(?:\[|$)", RegexOptions.Compiled);
        static readonly Regex s_SvePredicate = new Regex(@"^p\d+(?:\.[bhsdq]|/[zm])?$", RegexOptions.Compiled);
        static readonly Regex s_SveStack = new Regex(@"\[\s*(?:sp|x29)\s*(?:,\s*#(-?\d+)\s*,\s*mul\s+vl)?\s*\]", RegexOptions.Compiled);
        static readonly Regex s_SveCount =new Regex(@"^(cnt|inc|dec|sqinc|sqdec|uqinc|uqdec)[bhwd]$", RegexOptions.Compiled);

        /// <summary>SVE: z registers are vectors of a length known only at run time (128 bits at least), p registers
        /// predicates. Returns false for instructions that touch neither.</summary>
        static bool Sve(Instr x)
        {
            var m = x.Mnemonic;
            var ops = x.Operands.Select(o => o.Trim('{', '}', ' ')).ToArray();
            if (s_SveCount.IsMatch(m) && ops.Length > 0 && !ops[0].StartsWith("z", StringComparison.Ordinal))
            {
                x.Class = Cls.ScalarInt;
                return true;
            }
            var z = ops.Select(o => s_SveVector.Match(o)).FirstOrDefault(r => r.Success);
            if (z == null)
            {
                if (ops.Length > 0 && s_SvePredicate.IsMatch(ops[0]) && !m.StartsWith("st", StringComparison.Ordinal) && !m.StartsWith("ld", StringComparison.Ordinal))
                {
                    // ptrue, whilelo, ptest and other predicate work.
                    x.Class = Cls.Other;
                    return true;
                }
                return false;
            }
            x.Scalable = true;
            x.Width = 128;
            var lane = ops.Select(o => s_SveVector.Match(o)).Where(r => r.Success && r.Groups[1].Success).Select(r => r.Groups[1].Value).FirstOrDefault();
            var isLoad = m.StartsWith("ld", StringComparison.Ordinal);
            var isStore = m.StartsWith("st", StringComparison.Ordinal);
            if (isLoad || isStore)
            {
                x.Class = Cls.Move;
                x.Load = isLoad;
                x.Store = isStore;
                var slot = x.Operands.Select(o => s_SveStack.Match(o)).FirstOrDefault(r => r.Success);
                if (slot != null)
                    x.StackSlot = "sp" + (slot.Groups[1].Success ? slot.Groups[1].Value : "0") + "vl";
                return true;
            }
            x.Lane = lane switch { "b" => 8, "h" => 16, "s" => 32, "d" => 64, "q" => 128, _ => 0 };
            if (m == "mov" || m == "dup" || m == "sel" || m == "movprfx" || m == "fmov" || m == "fdup" || m == "fcpy" || m == "cpy")
                x.Class = Cls.Move;
            else if (m[0] == 'f' || m == "scvtf" || m == "ucvtf")
            {
                x.Class = Cls.PackedFloat;
                if (x.Lane == 0)
                    x.Lane = 32;
            }
            else
                x.Class = Cls.PackedInt;
            return true;
        }

        static readonly string[] s_Conds ={ "eq", "ne", "cs", "hs", "cc", "lo", "mi", "pl", "vs", "vc", "hi", "ls", "ge", "lt", "gt", "le", "al" };

        static bool IsArm32CondBranch(string m) => m.Length == 3 && m[0] == 'b' && s_Conds.Contains(m.Substring(1)) || m.StartsWith("b.", StringComparison.Ordinal) || m == "cbz" || m == "cbnz";

        static string StripArm32Cond(string m)
        {
            if (m.EndsWith("s", StringComparison.Ordinal) && m.Length > 3)
                m = m.Substring(0, m.Length - 1);
            foreach (var c in s_Conds)
                if (m.Length > c.Length && m.EndsWith(c, StringComparison.Ordinal))
                    return m.Substring(0, m.Length - c.Length);
            return m;
        }

        // ---------------------------------------------------------------------------------------------------------------
        // WebAssembly (structured control flow; loops are loop ... end_loop)

        static void Wasm(Instr x)
        {
            var m = x.Mnemonic;
            if (m == "call" || m == "call_indirect")
            {
                x.Class = Cls.Call;
                x.Target = m == "call" && x.Operands.Length > 0 ? x.Operands[0] : null;
                return;
            }
            if (m == "br" || m == "br_if" || m == "br_table")
            {
                x.Class = Cls.Branch;
                x.Conditional = m != "br";
                return;
            }
            if (m == "return")
            {
                x.Class = Cls.Ret;
                return;
            }
            var dot = m.IndexOf('.');
            var type = dot > 0 ? m.Substring(0, dot) : "";
            var op = dot > 0 ? m.Substring(dot + 1) : m;
            if (op.Contains("load", StringComparison.Ordinal))
            {
                x.Class = Cls.Move;
                x.Load = true;
                x.Width = type == "v128" || type.Contains('x') ? 128 : 0;
                return;
            }
            if (op.Contains("store", StringComparison.Ordinal))
            {
                x.Class = Cls.Move;
                x.Store = true;
                x.Width = type == "v128" || type.Contains('x') ? 128 : 0;
                return;
            }
            if (type == "local" || type == "global" || op == "const" && !type.Contains('x'))
            {
                x.Class = Cls.Move;
                return;
            }
            if (type.Contains('x') || type == "v128")
            {
                x.Width = 128;
                x.Class = type.StartsWith("f", StringComparison.Ordinal) ? Cls.PackedFloat : Cls.PackedInt;
                x.Lane = type.StartsWith("f64", StringComparison.Ordinal) || type.StartsWith("i64", StringComparison.Ordinal) ? 64 : type.StartsWith("i16", StringComparison.Ordinal) ? 16 : type.StartsWith("i8", StringComparison.Ordinal) ? 8 : 32;
                x.Fma = op.Contains("madd", StringComparison.Ordinal);
                return;
            }
            if (type == "f32" || type == "f64")
                x.Class = Cls.ScalarFloat;
            else if (type == "i32" || type == "i64")
                x.Class = Cls.ScalarInt;
            else
                x.Class = Cls.Other;
        }
    }
}
