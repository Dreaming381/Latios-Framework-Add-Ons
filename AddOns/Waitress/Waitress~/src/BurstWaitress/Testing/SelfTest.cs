using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BurstWaitress.Asm;
using BurstWaitress.Model;

namespace BurstWaitress.Testing
{
    /// <summary>Offline checks of the analysis on dumps recorded from the validation kernels (Testing/Data), with
    /// facts read off those dumps by hand and the independent census as the oracle.</summary>
    public static class SelfTest
    {
        static string Data(string name)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name + ".asm") ?? throw new Exception("missing test data " + name);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        static AsmFile Load(string name, string target) => Analyzer.Analyze(Data(name), target);

        public static int Run(TextWriter w)
        {
            var failed = 0;
            void Test(string name, Action body)
            {
                try
                {
                    body();
                    w.WriteLine("ok   " + name);
                }
                catch (Exception e)
                {
                    failed++;
                    w.WriteLine($"FAIL {name}: {e.Message}");
                }
            }
            void Eq<T>(T expected, T actual, string what)
            {
                if (!Equals(expected, actual))
                    throw new Exception($"{what}: expected {expected}, got {actual}");
            }
            Loop Main(AsmFile a) => a.Main;

            Test("operand split keeps brackets and quotes", () =>
            {
                var ops = AsmParser.SplitOperands("ymm0, ymmword ptr [rcx + r11 + 32], \"a, b\"");
                Eq(3, ops.Length, "count");
                Eq("ymmword ptr [rcx + r11 + 32]", ops[1], "memory operand");
            });
            Test("symbol names and call kinds", () =>
            {
                Eq("Ns.BwKernels.Opaque(float acc, float x)", Names.Pretty("Ns.BwKernels, Asm, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5250aef4a1bf354f590c69c4b3bed2ed from Asm, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"), "pretty");
                Eq("scalar math", Analyzer.CallKind("burst.Sleef_sinf_u35_avx2@@8"), "scalar sleef");
                Eq("vector math", Analyzer.CallKind("Sleef_sinf8_u35avx2"), "vector sleef");
                Eq("method", Analyzer.CallKind("Ns.T.M(float x)"), "managed");
                Eq("indirect", Analyzer.CallKind("rax"), "register");
            });
            Test("Mul avx2: ymm×8 unroll 4, trip 32; xmm trip 4; scalar remainder", () =>
            {
                var a = Load("Mul_avx2", "AVX2");
                Eq(3, a.Body.Loops.Count, "loops");
                var m = Main(a);
                Eq(256, m.Stats.Width, "width");
                Eq(32, m.Stats.Lane, "lane");
                Eq(4, m.Unroll, "unroll");
                Eq(32, m.MinCount, "min trip");
                Eq(false, m.AliasGuard, "alias");
                var xmm = a.Body.Loops.Single(l => l.Stats.Width == 128);
                Eq(4, xmm.MinCount, "xmm trip");
                Eq(1, xmm.Unroll, "xmm unroll");
                Eq(1, a.Body.Loops.Single(l => l.Stats.Width == 0).MinCount, "scalar trip");
            });
            Test("Channels8Plain avx2: alias check, trip 40", () =>
            {
                var m = Main(Load("Channels8Plain_avx2", "AVX2"));
                Eq(true, m.AliasGuard, "alias");
                Eq(40, m.MinCount, "min trip");
            });
            Test("Channels8NoAlias avx2: no alias check, trip 8", () =>
            {
                var m = Main(Load("Channels8NoAlias_avx2", "AVX2"));
                Eq(false, m.AliasGuard, "alias");
                Eq(8, m.MinCount, "min trip");
                Eq(256, m.Stats.Width, "width");
            });
            Test("Mul arm64: 128-bit ×4 unroll 2 (ldp pairs), trip 8 past a scheduled mov", () =>
            {
                var m = Main(Load("Mul_arm64", "ARMV8A_AARCH64"));
                Eq(128, m.Stats.Width, "width");
                Eq(2, m.Unroll, "unroll");
                Eq(8, m.MinCount, "min trip");
            });
            Test("Channels8Plain arm64: distance checks on x registers are the alias check, trip 24", () =>
            {
                var m = Main(Load("Channels8Plain_arm64", "ARMV8A_AARCH64"));
                Eq(true, m.AliasGuard, "alias");
                Eq(24, m.MinCount, "min trip");
            });
            Test("Pressure: spills, and two loads of different elements aren't an unroll", () =>
            {
                var a = Load("Pressure_avx2", "AVX2");
                Eq(14, a.Body.Stats.Spills, "spills");
                Eq(14, a.Body.Stats.Reloads, "reloads");
                Eq(1, Main(a).Unroll, "unroll");
            });
            Test("Dot: the reduction stays scalar", () =>
            {
                var m = Main(Load("Dot_avx2", "AVX2"));
                Eq(0, m.Stats.Width, "width");
                Eq(0.0, m.Stats.PackedRatio, "packed ratio");
            });
            Test("TwoLoops: non-inlined callee is a call in loop #4, defined in the dump", () =>
            {
                var a = Load("TwoLoops_avx2", "AVX2");
                var call = a.Calls.Single();
                Eq("method", call.Kind, "kind");
                Eq(true, call.InLoop, "in loop");
                Eq(2, Analyzer.Reached(a).Count, "functions");
            });
            Test("line info maps the main loop to the kernel's lines", () =>
            {
                var a = Load("Channels8Plain_avx2_lines", "AVX2");
                var m = Main(a);
                var lines = a.Body.Instrs.Where(x => Analyzer.InLoop(a.Body, m, x) && x.SrcLine > 0).Select(x => x.SrcLine).Distinct().OrderBy(x => x).ToList();
                Eq("65,67,68,69,70,71", string.Join(",", lines), "lines");
                Eq(true, a.Files.Values.Any(f => f.EndsWith("BwKernels.cs", StringComparison.Ordinal)), "file");
            });
            Test("Mul avx2: a byte-offset counter with a scalar remainder is 32 iterations per pass", () =>
            {
                var m = Main(Load("Mul_avx2", "AVX2"));
                Eq(Shape.Vector, m.Shape, "shape");
                Eq(32, m.ElemsPerIter, "per iteration");
                Eq(false, m.PerIterBytes, "unit");
            });
            Test("hand-written SIMD: the counter is i, not the next pass's i + 8 the compare tests", () =>
            {
                var a = Load("MinMaxSimd_avx2", "AVX2");
                var m = Main(a);
                Eq(8, m.ElemsPerIter, "per iteration");
                Eq(2, m.Unroll, "unroll");
                Eq(8, m.MinCount, "min count");
                Eq(Shape.Vector, m.Shape, "shape");
                Eq(Shape.Slp, a.Body.Loops.Single(l => !l.Main).Shape, "tail loop with one blend");
            });
            Test("float4 per iteration is SLP, not vectorized across iterations", () =>
            {
                var m = Main(Load("TransformPoints_avx2", "AVX2"));
                Eq(Shape.Slp, m.Shape, "shape");
                Eq(1, m.ElemsPerIter, "per iteration");
                Eq(1, m.Unroll, "unroll");
            });
            Test("a struct walked one element per pass (12 of 48 bytes read) is SLP, not 12 floats", () =>
            {
                var a = Analyzer.Analyze(Data("StridedStruct_avx2"), "AVX2", "Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem.GatherExposedSkeletonBoundsJob");
                var m = Main(a);
                Eq(true, m.Strided, "strided");
                Eq(Shape.Slp, m.Shape, "shape");
                Eq(48, m.ElemsPerIter, "bytes per pass");
                Eq(1, m.Unroll, "unroll");
            });
            Test("vector math calls are packed work; the scalar call in the remainder isn't a miss", () =>
            {
                var a = Load("Sin_sse4", "X64_SSE4");
                var m = Main(a);
                Eq(2, m.Stats.VectorMathCalls, "vector calls");
                Eq(128, m.Stats.PackedWidth, "width");
                Eq(8, m.ElemsPerIter, "per iteration");
                Eq(Shape.Vector, m.Shape, "shape");
                var scalar = a.Calls.Single(c => c.Kind == "scalar math");
                Eq(0, scalar.OutsideRemainder, "scalar sites outside the remainder");
            });
            Test("ARMv9 SVE: z registers are packed, dech/incb steps and the cnth guard scale with VL", () =>
            {
                var text = Data("Mul_armv9");
                var a = Analyzer.Analyze(text, "ARMV9A");
                var m = Main(a);
                Eq(true, m.Stats.Scalable, "scalable");
                Eq(Shape.Vector, m.Shape, "shape");
                Eq(8, m.ElemsPerIter, "per iteration at VL128");
                Eq(true, m.PerIterScalable, "per iteration scales");
                Eq(2, m.Unroll, "unroll");
                Eq(8, m.MinCount, "min count at VL128");
                Eq(true, m.MinCountScalable, "min count scales");
                Eq(false, m.AliasGuard, "the cnth compare isn't an alias check");
                // Independent count over the loop's lines: z-register float ops, z loads and stores.
                var lines = text.Replace("\r", "").Split('\n').Skip(m.Stats.FirstLine - 1).Take(m.Stats.LastLine - m.Stats.FirstLine + 1).Select(l => l.Trim()).ToList();
                Eq(lines.Count(l => System.Text.RegularExpressions.Regex.IsMatch(l, @"^f\w+\s+z\d+\.")), m.Stats.PackedFloat, "packed float");
                Eq(lines.Count(l => System.Text.RegularExpressions.Regex.IsMatch(l, @"^(ld1\w*|ldr)\s+\{?\s*z\d+")), m.Stats.Loads, "loads");
                Eq(lines.Count(l => System.Text.RegularExpressions.Regex.IsMatch(l, @"^(st1\w*|str)\s+\{?\s*z\d+")), m.Stats.Stores, "stores");
            });
            Test("a widened int count against a constant is a count guard, not an alias check", () =>
            {
                var a = Load("ClampedRange_avx2", "AVX2");
                var m = Main(a);
                Eq(false, m.AliasGuard, "alias");
                Eq(32, m.MinCount, "min count");
                Eq(Shape.Scalar, a.Body.Loops.Single(l => !l.Innermost).Shape, "outer loop judged by its own code");
            });
            Test("a count test folded into the alias check (cmp r8d, 9; setb; or) bounds the count", () =>
            {
                var m = Main(Load("BranchStore_sse4", "X64_SSE4"));
                Eq(true, m.AliasGuard, "alias");
                Eq(9, m.MinCount, "min count");
            });
            Test("SVE: pointer distance against rdvl is an alias check; sub x9, x8, #1 against cnth is n >= VL/16 + 1", () =>
            {
                var m = Main(Load("BranchStore_armv9", "ARMV9A"));
                Eq(true, m.AliasGuard, "alias");
                Eq(8, m.MinCount, "min count at VL128");
                Eq(1, m.MinCountExtra, "plus one");
                Eq(true, m.MinCountScalable, "scalable");
                Eq(Shape.Vector, m.Shape, "shape");
                Eq(2, m.Unroll, "unroll");
            });
            Test("n >= 1 then n != 1 on the way to the loop is n >= 2", () =>
            {
                var m = Main(Load("Gather_arm64", "ARMV8A_AARCH64"));
                Eq(2, m.MinCount, "min count");
            });
            Test("a float4 array, one per pass through a pointer, is SLP on x86 too", () =>
            {
                var m = Main(Load("Float4Hand_avx2", "AVX2"));
                Eq(Shape.Slp, m.Shape, "shape");
            });
            Test("increments on exclusive branch paths count once", () =>
            {
                var a = Load("BranchSelect_sse4", "X64_SSE4");
                var rem = a.Body.Loops.Single(l => l.Remainder);
                Eq(1, rem.ElemsPerIter, "per iteration");
            });
            Test("alias check written as a disjunction (a_end <= b, or b_end <= a) is found; its fallback before the loop is the remainder", () =>
            {
                var a = Analyzer.Analyze(Data("MyriSample_avx2"), "AVX2", "Latios.Myri.Sampling.SampleJob");
                var m = Main(a);
                Eq(true, m.AliasGuard, "alias");
                Eq(32, m.MinCount, "min count");
                Eq(true, m.Owner.Loops.Any(l => l.Remainder && l.Stats.FirstLine < m.Stats.FirstLine), "remainder before the vector loop");
            });
            Test("SVE count clamped from below (csel against mov #16) is max(VL/16, 16)", () =>
            {
                var m = Main(Analyzer.Analyze(Data("MyriSample_armv9"), "ARMV9A", "Latios.Myri.Sampling.SampleJob"));
                Eq(true, m.MinCountScalable, "scalable");
                Eq(8, m.MinCount, "VL part at VL128");
                Eq(16, m.MinCountFloor, "floor");
                Eq(true, m.AliasGuard, "alias");
            });
            Test("a data lane mask (vmovmskps) before the loop isn't an alias check", () =>
            {
                var m = Main(Load("MaskThenLoop_avx2", "AVX2"));
                Eq(false, m.AliasGuard, "alias");
                Eq(32, m.MinCount, "min count");
            });
            Test("ARM: an overflow guard against a constant in a register isn't an alias check", () =>
            {
                var m = Main(Load("Stride2Pairs_arm64", "ARMV8A_AARCH64"));
                Eq(false, m.AliasGuard, "alias");
                Eq(8, m.MinCount, "min count");
            });
            Test("count tests or'ed outside an alias check (cmp r8d, 12; setb; or; je) bound the count", () =>
            {
                var m = Main(Load("Stride2Pairs_sse4", "X64_SSE4"));
                Eq(false, m.AliasGuard, "alias");
                Eq(12, m.MinCount, "min count");
            });
            Test("one v256 per pass with no loop vectorizer marks is hand-packed SLP", () =>
            {
                var m = Main(Load("HandAvx_avx2", "AVX2"));
                Eq(Shape.Slp, m.Shape, "shape");
                Eq(true, m.HandPacked, "hand packed");
            });
            Test("a scalar loop entered from the vector path's exit is the remainder, even laid out first", () =>
            {
                var a = Load("Nested2D_avx2", "AVX2");
                var scalar = a.Body.Loops.Single(l => l.Innermost && l.Shape == Shape.Scalar);
                Eq(true, scalar.Remainder, "remainder");
            });
            Test("a vector copy loop is vector work with its alias check; the producer's mask loops don't win main", () =>
            {
                var a = Analyzer.Analyze(Data("BlendShapes_avx2"), "AVX2", "Latios.Mecanim.ApplyMecanimPropertiesSystem.ApplyBlendShapesJob");
                var m = Main(a);
                Eq(true, m.Stats.Copy, "copy");
                Eq(Shape.Vector, m.Shape, "shape");
                Eq(4, m.Unroll, "unroll");
                Eq(true, m.AliasGuard, "alias");
                Eq(true, m.AliasEvidence.Contains("cmp r11, 128", StringComparison.Ordinal), "evidence");
                Eq(true, m.Owner.Display.Contains("ApplyBlendShapesJob.Execute", StringComparison.Ordinal), "main in the job's Execute");
                Eq(3, m.Owner.Loops.Single(l => l.Stats.FirstLine == 686).MinCount, "lea eax, [rcx + 2]; cmp eax, 5; setb is n >= 3");
            });
            Test("ARM: an equality compare of a cached value (Entities lookup cache) isn't an alias check", () =>
            {
                var a = Analyzer.Analyze(Data("BlendShapes_armv9"), "ARMV9A", "Latios.Mecanim.ApplyMecanimPropertiesSystem.ApplyBlendShapesJob");
                Eq(false, a.Focus.SelectMany(f => f.Loops).Any(l => l.AliasEvidence != null && l.AliasEvidence.Contains("cmp x10, x9", StringComparison.Ordinal)), "lookup-cache compare");
            });
            Test("a lock cmpxchg retry loop isn't main; the SLP loop around it is", () =>
            {
                var m = Main(Load("UploadBlit_avx2", "AVX2"));
                Eq(false, m.Innermost, "the loop around the retry loop");
                Eq(Shape.Slp, m.Shape, "shape");
            });
            Test("ARMv9 SVE ordered reduction (fadda) is flagged", () =>
            {
                var m = Main(Load("Dot_armv9", "ARMV9A"));
                Eq(2, m.Stats.OrderedReductions, "fadda");
            });
            foreach (var (name, target) in new[]
                     {
                         ("Mul_avx2", "AVX2"), ("Channels8Plain_avx2", "AVX2"), ("Channels8NoAlias_avx2", "AVX2"), ("Pressure_avx2", "AVX2"), ("Dot_avx2", "AVX2"), ("TwoLoops_avx2", "AVX2"),
                         ("Channels8Plain_avx2_lines", "AVX2"), ("MinMaxSimd_avx2", "AVX2"), ("TransformPoints_avx2", "AVX2"), ("Sin_sse4", "X64_SSE4"),
                         ("ClampedRange_avx2", "AVX2"), ("BranchStore_sse4", "X64_SSE4"), ("Float4Hand_avx2", "AVX2"), ("BranchSelect_sse4", "X64_SSE4"),
                         ("Stride2Pairs_sse4", "X64_SSE4"), ("HandAvx_avx2", "AVX2"), ("Nested2D_avx2", "AVX2"), ("MaskThenLoop_avx2", "AVX2"),
                     })
                Test("census matches the independent count: " + name, () => CheckCensus(Data(name), target));
            Test("targets and option values", () =>
            {
                Eq("X64_SSE4", CompileOptions.ParseTarget("sse4"), "sse4");
                Eq("ARMV8A_AARCH64", CompileOptions.ParseTarget("neon"), "neon");
                Eq("Strict", CompileOptions.ParseFloatMode("strict"), "strict");
                Eq(null, CompileOptions.ParseFloatMode("attr"), "attr");
                try
                {
                    CompileOptions.ParseTarget("avx512");
                    throw new Exception("avx512 accepted");
                }
                catch (WaitressException e) when (e.Message.Contains("AVX-512", StringComparison.Ordinal))
                {
                }
            });
            Test("name resolution", () =>
            {
                var index = new TargetIndex();
                index.Entries.Add(new Entry { Name = "Ns.BwKernels.Mul(float*, float*, float*, int)", Kind = "static", HasBurst = true });
                index.Entries.Add(new Entry { Name = "Ns.BwKernels.Dot(float*, float*, int)", Kind = "static", HasBurst = true });
                index.Entries.Add(new Entry { Name = "Ns.BwSystem.__codegen__OnUpdate(IntPtr, IntPtr)", Kind = "static", HasBurst = true });
                index.Entries.Add(new Entry { Name = "Ns.BwGenericJob<float>", Kind = "job", Interface = "IJob", HasBurst = true });
                index.Entries.Add(new Entry { Name = "Ns.Twice (IJob)", Kind = "job", Interface = "IJob", HasBurst = true });
                index.Entries.Add(new Entry { Name = "Ns.Twice (IJobFor)", Kind = "job", Interface = "IJobFor", HasBurst = true });
                Eq("Ns.BwKernels.Mul(float*, float*, float*, int)", index.One("BwKernels.Mul").Name, "short");
                Eq("Ns.BwKernels.Mul(float*, float*, float*, int)", index.One("Mul(float*,float*,float*,int)").Name, "params");
                Eq("Ns.BwSystem.__codegen__OnUpdate(IntPtr, IntPtr)", index.One("BwSystem.OnUpdate").Name, "ISystem alias");
                Eq("Ns.BwGenericJob<float>", index.One("BwGenericJob<System.Single>").Name, "generic");
                Eq(2, index.Resolve("BwKernels").Count, "a class stands for its methods");
                Eq(2, index.Resolve("BwKernels.*").Count, "glob");
                Eq("Ns.Twice (IJobFor)", index.One("Twice - (IJobFor)").Name, "inspector display name");
                Eq(2, index.Resolve("Twice").Count, "two interfaces");
            });
            w.WriteLine(failed == 0 ? "all passed" : $"{failed} failed");
            return failed == 0 ? 0 : 1;
        }

        /// <summary>The analyzer's body counts against the independent census, and its innermost loops against the
        /// backward branches read off the raw text.</summary>
        public static void CheckCensus(string text, string target)
        {
            var a = Analyzer.Analyze(text, target);
            var c = Census.Count(text, a.Body.Name);
            var s = a.Body.Stats;
            var mine = $"total {s.Instrs} pf {s.PackedFloat} sf {s.ScalarFloat} ld {s.Loads} st {s.Stores} br {s.Branches} call {s.Calls} ymm {a.Body.Instrs.Count(x => x.Mnemonic != ":" && x.Operands.Any(o => o.Contains("ymm", StringComparison.Ordinal)))}";
            if (mine != c.ToString())
                throw new Exception($"summary {mine} vs census {c}");
            // A loop laid out in order ends at a backward branch to its header label. A rotated loop, entered in the
            // middle, is the union of the backward branches inside it.
            var loops = a.Body.Loops.Where(l => l.Innermost).Select(l => (l.Stats.FirstLine, l.Stats.LastLine)).OrderBy(x => x).ToList();
            var edges = c.BackwardRanges.Select(r => (from: r.from + 1, to: r.to)).OrderBy(x => x).ToList();
            foreach (var l in loops)
            {
                var inside = edges.Where(e => e.from >= l.FirstLine - 1 && e.to <= l.LastLine).ToList();
                if (inside.Count == 0 || inside.Min(e => e.from) > l.FirstLine + 1 || inside.Max(e => e.to) != l.LastLine)
                    throw new Exception($"loop at {l.FirstLine}-{l.LastLine} doesn't match the backward branches (edges: {string.Join(" ", edges.Select(e => e.from + "-" + e.to))})");
            }
        }
    }
}
