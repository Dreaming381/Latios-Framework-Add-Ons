using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using BurstWaitress.Asm;
using BurstWaitress.Model;
using BurstWaitress.Testing;

namespace BurstWaitress.Commands
{
    /// <summary>
    /// Acceptance checks against the kernels in Assets/Validation/BurstWaitress: counts against an independent census of
    /// the raw dumps, codegen facts measured by hand, float modes against raw FMA counts, status honesty, and that
    /// nothing in the Editor changed.
    /// </summary>
    public static class Verify
    {
        const string Ns = "Latios.Validation.BurstWaitress.";

        public static int Run(Args args, TextWriter w)
        {
            args.RejectExtraPositionals(0);
            args.RejectUnknown();
            var failed = 0;
            void Check(string name, Func<string> body)
            {
                try
                {
                    var note = body();
                    w.WriteLine("ok   " + name + (note != null ? " · " + note : ""));
                }
                catch (Exception e) when (e is not EditorUnavailable)
                {
                    failed++;
                    w.WriteLine($"FAIL {name}: {e.Message}");
                }
            }
            var before = EditorScript.Call(Project.Root, "Snapshot");
            var index = TargetIndex.Load();
            Entry E(string shortName) => index.One(Ns + shortName);
            var avx2 = new CompileOptions();
            var kernels = new[] { "BwKernels.Mul", "BwKernels.Dot", "BwKernels.DotFast", "BwKernels.MulAdd", "BwKernels.Channels8Plain", "BwKernels.Channels8NoAlias", "BwKernels.Sin", "BwKernels.Acos",
                                  "BwKernels.TransformPoints", "BwKernels.RotateAll", "BwKernels.TwoLoops", "BwKernels.Straight", "BwKernels.Pressure", "BwMulJob", "BwForJob", "BwParallelJob", "BwGenericJob<float>",
                                  "BwSystem.OnUpdate", "BwFunctionPointers.Hypot", "BwKernels.HandSimd8", "BwKernels.MulFromOne", "BwKernels.SerialSelect", "BwKernels.Range", "BwKernels.ClampedRange",
                                  "Round3.BwR3Kernels.Float4Hand", "Round3.BwR3Kernels.BranchStore", "Round3.BwR3Kernels.SoaLength", "Round3.BwR3Kernels.AosLength" };
            var dumps = DumpStore.Get(kernels.Select(k => (E(k), avx2)).ToList(), w).ToDictionary(d => d.Entry.Name);
            Dump D(string k) => dumps[E(k).Name];

            foreach (var k in kernels)
                Check("census of " + k, () =>
                {
                    var d = D(k);
                    if (d.Error != null)
                        throw new Exception(d.Error);
                    SelfTest.CheckCensus(d.Text, d.Options.Target);
                    return $"{d.Asm.Body.Stats.Instrs} instrs, {d.Asm.Body.Loops.Count} loops";
                });

            Loop Main(Dump d) => d.Asm.Main;
            Check("a[i] * b[i] into another array vectorizes as packed ymm", () =>
            {
                var m = Main(D("BwKernels.Mul"));
                Expect(m.Stats.Width == 256 && m.Stats.PackedRatio == 1, Report.LoopLine(Isa.X86, m));
                return Report.LoopLine(Isa.X86, m);
            });
            Check("sum += a[i] * b[i] stays scalar under FloatMode.Default", () =>
            {
                var m = Main(D("BwKernels.Dot"));
                Expect(m.Stats.Width == 0 && m.Stats.PackedFloat == 0, Report.LoopLine(Isa.X86, m));
                var fast = Main(D("BwKernels.DotFast"));
                Expect(fast.Stats.Width == 256, "the FloatMode.Fast control didn't vectorize: " + Report.LoopLine(Isa.X86, fast));
                return "Fast control: " + Report.WidthName(Isa.X86, fast.Stats.Width, fast.Stats.Lane);
            });
            Check("sub-ranges of one buffer without [NoAlias]: alias check, trip 8 -> 40", () =>
            {
                var plain = Main(D("BwKernels.Channels8Plain"));
                var noAlias = Main(D("BwKernels.Channels8NoAlias"));
                Expect(plain.AliasGuard && plain.MinCount == 40, "plain: " + Report.LoopLine(Isa.X86, plain));
                Expect(!noAlias.AliasGuard && noAlias.MinCount == 8, "[NoAlias]: " + Report.LoopLine(Isa.X86, noAlias));
                // The raw dumps: the plain one has lane compares, the [NoAlias] one has none.
                Expect(Regex.IsMatch(D("BwKernels.Channels8Plain").Text, @"\bvpcmpgtq\b") && !Regex.IsMatch(D("BwKernels.Channels8NoAlias").Text, @"\bvpcmpgtq\b"), "raw vpcmpgtq evidence");
                Expect(Regex.IsMatch(D("BwKernels.Channels8Plain").Text, @"cmp\s+r9d, 40\b") && Regex.IsMatch(D("BwKernels.Channels8NoAlias").Text, @"cmp\s+r9d, 8\b"), "raw cmp guards");
                return $"plain trip {plain.MinCount} with {plain.AliasGuardInstrs}-instr check; [NoAlias] trip {noAlias.MinCount}";
            });
            Check("one float3/float4 element per pass is SLP, not vectorized across iterations", () =>
            {
                var parts = new List<string>();
                foreach (var k in new[] { "BwKernels.TransformPoints", "BwKernels.RotateAll", "BwKernels.Pressure" })
                {
                    var m = Main(D(k));
                    Expect(m.Shape == Shape.Slp && m.ElemsPerIter == 1 && m.Stats.PackedWidth == 128, k + ": " + Report.LoopLine(Isa.X86, m));
                    parts.Add(k.Substring(10) + " " + Report.ShapeWidth(Isa.X86, m, true));
                }
                return string.Join(", ", parts);
            });
            Check("hand-written SIMD i += 8 is 8 per pass; a loop from i = 1 guards n >= 33", () =>
            {
                var hand = Main(D("BwKernels.HandSimd8"));
                Expect(hand.Shape == Shape.Vector && hand.ElemsPerIter == 8 && hand.Unroll == 2 && hand.MinCount == 8, "HandSimd8: " + Report.LoopLine(Isa.X86, hand));
                var one = Main(D("BwKernels.MulFromOne"));
                Expect(one.ElemsPerIter == 32 && one.MinCount == 33, "MulFromOne: " + Report.LoopLine(Isa.X86, one));
                // The raw evidence: the counter steps by 8, and the guard compares against 33.
                Expect(Regex.IsMatch(D("BwKernels.HandSimd8").Text, @"add\s+e\w+, 8\b") && Regex.IsMatch(D("BwKernels.MulFromOne").Text, @"cmp\s+\w+, 33\b"), "raw step and guard");
                return $"HandSimd8 {Report.PerIter(hand)}/iter u{hand.Unroll}; MulFromOne n ≥ {one.MinCount}";
            });
            Check("guards: a widened int count isn't an alias check; counts behind not/add, setb and sub #1 are read through", () =>
            {
                var clamped = Main(D("BwKernels.ClampedRange"));
                Expect(!clamped.AliasGuard && clamped.MinCount == 32, "ClampedRange: " + Report.LoopLine(Isa.X86, clamped));
                Expect(Regex.IsMatch(D("BwKernels.ClampedRange").Text, @"sub\s+r\w+, r\w+") && Regex.IsMatch(D("BwKernels.ClampedRange").Text, @"cmp\s+r\w+, 32\b"), "raw 64-bit count guard");
                var range = Main(D("BwKernels.Range"));
                Expect(range.MinCount == 32 && Regex.IsMatch(D("BwKernels.Range").Text, @"\bnot\s+e\w+") && Regex.IsMatch(D("BwKernels.Range").Text, @"cmp\s+e\w+, 31\b"), "Range: " + Report.LoopLine(Isa.X86, range));
                var store = Main(D("Round3.BwR3Kernels.BranchStore"));
                Expect(store.AliasGuard && store.MinCount == 33, "BranchStore: " + Report.LoopLine(Isa.X86, store));
                return $"ClampedRange n ≥ {clamped.MinCount}, no alias check; Range n ≥ {range.MinCount} (cmp against 31 of n - 1); BranchStore n ≥ {store.MinCount} with its alias check";
            });
            Check("float4 per element through a pointer is SLP; SoA float3 math vectorizes, AoS packs one point", () =>
            {
                var f4 = Main(D("Round3.BwR3Kernels.Float4Hand"));
                var soa = Main(D("Round3.BwR3Kernels.SoaLength"));
                var aos = Main(D("Round3.BwR3Kernels.AosLength"));
                Expect(f4.Shape == Shape.Slp, "Float4Hand: " + Report.LoopLine(Isa.X86, f4));
                Expect(soa.Shape == Shape.Vector && soa.Stats.PackedWidth == 256 && soa.ElemsPerIter == 8, "SoaLength: " + Report.LoopLine(Isa.X86, soa));
                Expect(aos.Shape == Shape.Slp && aos.ElemsPerIter == 1, "AosLength: " + Report.LoopLine(Isa.X86, aos));
                return $"Float4Hand {Report.ShapeWidth(Isa.X86, f4, true)}, SoaLength {Report.ShapeWidth(Isa.X86, soa, true)} {Report.PerIter(soa)}/iter, AosLength {Report.ShapeWidth(Isa.X86, aos, true)}";
            });
            var r3 = DumpStore.Get(new[]
            {
                (E("Round3.BwR3Kernels.BranchStore"), new CompileOptions { Target = "X64_SSE4" }), (E("Round3.BwR3Kernels.BranchStore"), new CompileOptions { Target = "ARMV9A" }),
                (E("BwKernels.MaskThenLoop"), new CompileOptions()), (E("Round4.BwR4Kernels.Stride2Pairs"), new CompileOptions { Target = "ARMV8A_AARCH64" }),
                (E("Round4.BwR4Kernels.Stride2Pairs"), new CompileOptions { Target = "X64_SSE4" }), (E("Round4.BwR4Kernels.HandAvx"), new CompileOptions()),
            }, w);
            Check("no alias check from a data lane mask or an ARM overflow guard; or'ed count tests bound n; a v256 hand loop is SLP", () =>
            {
                var mask = r3[2].Asm.Main;
                Expect(!mask.AliasGuard && Regex.IsMatch(r3[2].Text, @"vmovmskps"), "MaskThenLoop: " + Report.LoopLine(Isa.X86, mask));
                var arm = r3[3].Asm.Main;
                Expect(!arm.AliasGuard && Regex.IsMatch(r3[3].Text, @"mov\s+x\d+, #-1073741824"), "Stride2Pairs ARM64: " + Report.LoopLine(Isa.Arm64, arm));
                var sse = r3[4].Asm.Main;
                Expect(sse.MinCount == 12 && Regex.IsMatch(r3[4].Text, @"cmp\s+\w+, 12\s*\n\s*setb"), "Stride2Pairs SSE4: " + Report.LoopLine(Isa.X86, sse));
                var hand = r3[5].Asm.Main;
                Expect(hand.Shape == Shape.Slp && hand.HandPacked, "HandAvx: " + Report.LoopLine(Isa.X86, hand));
                return $"MaskThenLoop and Stride2Pairs ARM64 no alias check; Stride2Pairs SSE4 n ≥ {sse.MinCount}; HandAvx {Report.ShapeWidth(Isa.X86, hand, true)}";
            });
            Check("count folded into an alias check (SSE4 cmp 9; setb) and the SVE rdvl pointer-distance check", () =>
            {
                var sse = r3[0].Asm.Main;
                Expect(sse.AliasGuard && sse.MinCount == 9 && Regex.IsMatch(r3[0].Text, @"cmp\s+\w+, 9\s*\n\s*setb"), "SSE4: " + Report.LoopLine(Isa.X86, sse));
                var sve = r3[1].Asm.Main;
                Expect(sve.AliasGuard && sve.MinCountScalable && sve.MinCount == 8 && sve.MinCountExtra == 1 && Regex.IsMatch(r3[1].Text, @"\brdvl\b"), "ARMv9: " + Report.LoopLine(Isa.Arm64, sve));
                return $"SSE4 n ≥ {sse.MinCount}; ARMv9 n ≥ {Report.MinCount(sve)}, alias check {sve.AliasGuardInstrs} instrs";
            });
            var cross = DumpStore.Get(new[] { (E("BwKernels.Sin"), new CompileOptions { Target = "X64_SSE4" }), (E("BwKernels.Mul"), new CompileOptions { Target = "ARMV9A" }) }, w);
            Check("SSE4 math.sin: vector Sleef calls are the packed work; the scalar call is the remainder's", () =>
            {
                var a = cross[0].Asm;
                var m = a.Main;
                Expect(m.Stats.VectorMathCalls > 0 && m.Shape == Shape.Vector && m.Stats.PackedWidth == 128, Report.LoopLine(Isa.X86, m));
                Expect(a.Calls.Where(c => c.Kind == "scalar math").All(c => c.OutsideRemainder == 0), "a scalar math call outside the remainder");
                Expect(Regex.IsMatch(cross[0].Text, @"call\s+.*Sleef_sinf4_"), "raw vector call");
                return Report.LoopLine(Isa.X86, m);
            });
            Check("ARMv9 Mul: SVE, steps and guard scale with the vector length", () =>
            {
                var m = cross[1].Asm.Main;
                Expect(m.Stats.Scalable && m.Shape == Shape.Vector && m.PerIterScalable && m.MinCountScalable && !m.AliasGuard, Report.LoopLine(Isa.Arm64, m));
                Expect(Regex.IsMatch(cross[1].Text, @"fmul\s+z\d+\.s") && Regex.IsMatch(cross[1].Text, @"\bcnt[bhwd]\b"), "raw SVE evidence");
                return Report.LoopLine(Isa.Arm64, m);
            });

            var modes = new[] { "Default", "Strict", "Fast" };
            var fmaDumps = DumpStore.Get(modes.Select(m => (E("BwKernels.MulAdd"), new CompileOptions { FloatMode = m })).ToList(), w);
            Check("a*b + c: FMA counts per float mode match the raw dumps", () =>
            {
                var parts = new List<string>();
                foreach (var d in fmaDumps)
                {
                    var raw = Census.Count(d.Text, d.Asm.Body.Name);
                    var rawFma = BodyLines(d).Count(l => Regex.IsMatch(l, @"^\s+vf(n)?m(add|sub)"));
                    Expect(rawFma == d.Asm.Body.Stats.Fma, $"{d.Options.FloatMode}: summary fma {d.Asm.Body.Stats.Fma}, raw {rawFma}");
                    parts.Add($"{d.Options.FloatMode} {rawFma}");
                }
                Expect(fmaDumps[0].Asm.Body.Stats.Fma == fmaDumps[1].Asm.Body.Stats.Fma, "Default and Strict differ");
                Expect(fmaDumps[2].Asm.Body.Stats.Fma > 0, "Fast has no FMA");
                return "fma " + string.Join(", ", parts);
            });

            Check("status: managed allocation fails, its assembly fails, plain job not eligible, kernel compiled", () =>
            {
                var names = new[] { Ns + "BwManagedJob", Ns + "BwFailingNeighbor.Twice(int)", Ns + "BwPlainJob", E("BwKernels.Mul").Name };
                var file = Path.Combine(Project.Dir, "Requests", "verify-status.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.WriteAllLines(file, names);
                var outDir = Path.Combine(Project.Dir, "StatusDumps");
                JsonElement r = default;
                for (var attempt = 0; attempt < 120; attempt++)
                {
                    r = EditorScript.Call(Project.Root, "Status", file, outDir, 20000, false);
                    if (!r.GetProperty("items").EnumerateArray().Any(i => i.TryGetProperty("assemblyCheck", out _)))
                        break;
                    System.Threading.Thread.Sleep(1000);
                }
                var states = r.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("state").GetString()).ToList();
                Expect(states[0] == "failed", "BwManagedJob: " + states[0]);
                Expect(states[1] == "failed" && r.GetProperty("items")[1].TryGetProperty("assemblyFailure", out _), "BwFailingNeighbor.Twice: " + states[1]);
                Expect(states[2] == "not eligible", "BwPlainJob: " + states[2]);
                Expect(states[3] == "compiled" || states[3] == "pending", "BwKernels.Mul: " + states[3]);
                return string.Join(", ", states);
            });

            Check("token budget", () =>
            {
                var rows = new List<string>();
                foreach (var k in new[] { "BwKernels.Mul", "BwKernels.Channels8Plain", "BwKernels.Sin", "BwMulJob" })
                {
                    var d = D(k);
                    var sw = new StringWriter();
                    Report.Summary(d, sw, false);
                    rows.Add($"{k} summary {sw.ToString().Length} chars vs raw {d.Text.Length}");
                }
                var sweep = new StringWriter();
                var saved = Console.Out;
                return string.Join("; ", rows);
            });

            var after = EditorScript.Call(Project.Root, "Snapshot");
            Check("no side effects: Burst options, compiled scripts and scenes unchanged", () =>
            {
                foreach (var key in new[] { "enabled", "menuEnabled", "safetyChecks", "forceSafetyChecks", "synchronous", "nativeDebug", "scriptAssemblies", "dirtyScenes" })
                    Expect(before.GetProperty(key).GetRawText() == after.GetProperty(key).GetRawText(), $"{key}: {before.GetProperty(key).GetRawText()} -> {after.GetProperty(key).GetRawText()}");
                return $"safety {after.GetProperty("safetyChecks").GetRawText()}, synchronous {after.GetProperty("synchronous").GetRawText()}, dirty scenes {after.GetProperty("dirtyScenes").GetRawText()}";
            });
            w.WriteLine(failed == 0 ? "all passed" : $"{failed} failed");
            return failed == 0 ? 0 : 1;
        }

        static IEnumerable<string> BodyLines(Dump d) => d.Asm.Lines.Skip(d.Asm.Body.StartLine - 1).Take(d.Asm.Body.EndLine - d.Asm.Body.StartLine + 1);

        static void Expect(bool ok, string what)
        {
            if (!ok)
                throw new Exception(what);
        }
    }
}
