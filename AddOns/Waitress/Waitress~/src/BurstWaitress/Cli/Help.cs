using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BurstWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("burstwaitress — the code Burst generates, without reading disassembly: did a loop vectorize and how wide, which calls");
            w.WriteLine("survive, alias checks and the counts a vector loop needs, spills, what an option or code change costs, and whether a method");
            w.WriteLine("is Burst-compiled in this Editor at all");
            w.WriteLine();
            w.WriteLine("<method> is a [BurstCompile] static method (Type.Method, or Type.Method(float*, int) for an overload), a job type");
            w.WriteLine("(IJob, IJobFor, IJobParallelFor, IJobChunk, IJobEach, IJobEntity, closed generic jobs), or an ISystem method");
            w.WriteLine("(MySystem.OnUpdate); full or unambiguous short names, globs with *. Dumps need the Editor once, then are cached");
            w.WriteLine("per compile of the scripts. Burst settings are only ever read: per-dump options go into that one compile.");
            w.WriteLine();
            w.WriteLine("compile options (dump, diff, sweep): --target avx2 (default)|avx|sse4|sse2|arm64|armv7|armv9|wasm|auto");
            w.WriteLine("  --float-mode attr (default: the method's [BurstCompile])|Default|Strict|Deterministic|Fast");
            w.WriteLine("  --float-precision attr|Standard|High|Medium|Low   --safety off (default, as in players)|on|force");
            w.WriteLine("  diff and sweep take comma lists: --target avx2,sse4 --float-mode Default,Strict");
            w.WriteLine();
            w.WriteLine("commands");
            w.WriteLine("  status <method...>           per method: compiled, pending, failed (with the error), burst disabled or not");
            w.WriteLine("                               eligible (and why); plus the Editor's Burst settings and background compilation;");
            w.WriteLine("                               notes code that changed without its Burst hash changing");
            w.WriteLine("  dump <method> [options]      summary over the method's own code (the entry and the C# functions Burst kept");
            w.WriteLine("                               apart, e.g. a job's Execute): instruction classes, widest register, each loop");
            w.WriteLine("                               (width, SLP or vector, unroll, per-pass count, packed %, alias check, n ≥ guard)");
            w.WriteLine("                               by function, calls with scalar math flagged and where they are, spills, the");
            w.WriteLine("                               numbered function list. --all lifts the line caps; --json");
            w.WriteLine("        --raw [main|loop:N|body|entry|fn:<name|N|glob>|lines:A-B|src:[File.cs:]A-B]  raw assembly (default main loop)");
            w.WriteLine("        --fn <name|N>          summary, --source or --raw for one function of the dump (numbers from its list);");
            w.WriteLine("                               Job.Execute as the name does the same");
            w.WriteLine("        --source               per C# line: instructions and classes, hot loops first (compiles with line info)");
            w.WriteLine("  diff <a> [<b>] [options]     two dumps side by side with deltas, loops matched, calls added and removed;");
            w.WriteLine("                               two methods, or one method and one option with two values (--float-mode Default,Strict)");
            w.WriteLine("  sweep <method...> [options]  one row per method × option set: main loop width and shape, per pass, packed %, alias, n ≥,");
            w.WriteLine("                               loops, calls, spills, fma, size. Globs: \"BwKernels.*\". Rows print when all dumps are in;");
            w.WriteLine("                               progress goes to stderr every 15 s, and finished dumps are cached, so a re-run resumes");
            w.WriteLine("  list [filter]                Burst entry points (what names resolve to), with their [BurstCompile] settings");
            w.WriteLine("  targets                      the targets this Editor's Burst can compile, each tried");
            w.WriteLine("  verify                       acceptance checks against the BurstWaitress validation kernels (needs them)");
            w.WriteLine();
            w.WriteLine("  --project <dir> picks the Unity project · --timeout <s> (default 180) bounds waiting on compiles; when it");
            w.WriteLine("  runs out, or the tool is stopped or killed, the Editor stops that request's queue within about 12 s");
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("reading", @"Reading a dump summary

  packed float / scalar float  SIMD float math (vmulps, vfmadd231ps, fmul v0.4s) vs one-lane math (vmulss, fmul s0).
                               Moves, loads and stores are counted apart; a packed multiply with a memory operand is
                               both packed float and a load.
  widest                       the widest vector register any instruction uses (xmm 128, ymm 256, zmm 512 bits).
  loop #N                      a natural loop (a back edge to a block that dominates it). Vectorized loops usually come in
                               a set: the main vector loop, a narrower or scalar remainder, sometimes a scalar fallback.
                               main = the innermost loop vectorized across iterations, else the widest packed one.
  ymm×8 unroll 4 (32/iter)     8 float lanes per register, 4 registers per pass, so 32 elements (source iterations of
                               a float loop) per pass. The count comes from the loop's induction variable: a counter
                               gives iterations, an index scaled by the lane size gives lane elements, a pointer gives
                               bytes (16 B/iter) unless a remainder loop or guard shows it is loop-vectorized.
  SLP                          packed math within one source iteration (a float4, a pair of accumulators, a struct walked
                               one element per pass): 1/iter, or a stream reading under half its stride, or a loop packed
                               by hand (one v256 per iteration, none of the loop vectorizer's remainder loop or lane
                               guard). Not vectorized across iterations, whatever the register width.
  (span unknown)               packed math whose per-pass count doesn't say (a pointer step, a batch stride).
  remainder                    a scalar loop beside a vector loop in the same function: the leftover iterations.
  runs if n ≥ N                the smallest value of the guarded count (usually the C# loop bound) for which that loop
                               runs, from the compare guards on every path to it. For a loop from i = 1 it is one more
                               than the iterations. Below N the code takes a narrower or scalar path.
  SVE×VL/32, VL/16             ARMv9 SVE: the vector length is set by the CPU (128 bits or more), so lanes, per-pass
                               counts and guards are fractions of VL in bits (n ≥ VL/16+1 for a loop from 1). ordered
                               reduction (fadda) adds the lanes one after another. Many ARMv9 loops stay NEON (128-bit).
                               In --json, scalable counts are given at a 128-bit VL with *Scalable: true.
  alias check (K instrs)       the loop is versioned on a runtime pointer-overlap test: lane compares of pointers, compares
                               of 64-bit pointer values or of their difference against the vector span, SVE whilewr or
                               rdvl distances; the deciding instruction is quoted. A widened int count against a
                               constant is a count guard, not one. K
                               instructions run before every call. [NoAlias] on the pointer parameters (it survives
                               inlining) removes it; Burst already treats job containers as not aliasing.
  gather emulated: N lane loads one element loaded into one lane at a time (vinsertps, ld1 {v0.s}[1]): an indexed load
                               the target has no gather instruction for, or that Burst didn't use one for.
  outer loop                   a loop whose own instructions (outside its inner loops) have no packed math.
  copy                         a loop that only moves vectors (a vectorized copy or conversion), at the move width.
  (1/iter, counter step 2)     a scalar loop whose counter steps by 2 (i += 2): still one source iteration per pass.
  calls [scalar math]          a math library routine called per element (Sleef_sinf_u35): the math.* call didn't
                               vectorize, unless it is only in a remainder loop (marked). [vector math] routines take a
                               whole register (Sleef_sinf4_u35sse4) and count as packed work at that width.
  spills N stores / M reloads  stack slots the code writes and reads back (callee-saved registers in the prologue and
                               outgoing call arguments aren't counted).
  functions in the dump        callees Burst didn't inline, and library routines, with their sizes. infra functions
                               (Entities journaling, Burst and FixedString formatting) ride along in Editor compiles and
                               aren't counted in the totals."),
            ("status", @"Is it Burst code?

  compiled       it compiles with the Editor's own settings, Burst is on, and background compilation has finished.
                 For static methods, also whether it was called this session (the direct-call pointer is set).
  pending        background compilation is still running (e.g. right after a script change); until it finishes,
                 jobs and calls run managed. Benchmarks taken now measure managed code.
  failed         Burst can't compile it: the error and the line. The Editor doesn't reliably log these; this is
                 checked by compiling, not read from the console.
  burst disabled Burst is off in the Editor, or for its assembly (Burst_DisableAssembliesForEditorCompilation.json).
  not eligible   no [BurstCompile] where Burst needs one, an instance method, an open generic, or no such entry point.

  Job.Execute: reported as its job, which Burst compiles with it.
  Code under the same hash: Burst names a compiled entry point by a hash of its own IL and options, not its callees'.
  status remembers each entry's hash with a digest of its code and notes when the code changed under the same hash.
  On Burst 1.8.30 that never left old native code running (a callee edit, a callee in another assembly, edits with
  Burst off then on were all fresh), so the note is a lead, not a verdict. If behavior says otherwise: an IL change in
  the entry method itself, then recompile. Dumps always compile the current code."),
            ("options", @"Compile options

  Each dump compiles with the method's [BurstCompile] settings (and the assembly's), with these replaced:
    --target         x64: avx2 (default), avx, sse4, sse2; auto = the Editor's host CPU. Cross targets compile
                     without a device: arm64/neon (ARMV8A_AARCH64), armv9, armv7 (NEON32), thumb2, wasm.
                     Burst 1.8 has no AVX-512 target. `targets` tries each in this Editor.
    --float-mode     attr (the attribute's, default), Default, Strict, Deterministic, Fast
    --float-precision attr, Standard, High, Medium, Low
    --safety         off (default, as players run), on (the Editor's checks, but [DisableSafetyChecks] still wins),
                     force (overrides DisableSafetyChecks)
  --source adds line info (it can shift code slightly; the summary of the same options without it is the reference).
  The options only go into the one compile's option string. The tool never assigns BurstCompiler.Options: doing so,
  even to the current value, recompiles every Burst method in the project, and synchronous compilation can wedge
  the Editor."),
        };

        public static void Topic(TextWriter w, string topic)
        {
            if (topic == null)
            {
                Index(w);
                return;
            }
            var t = s_Topics.FirstOrDefault(x => x.name.Equals(topic, StringComparison.OrdinalIgnoreCase));
            if (t.text != null)
            {
                w.WriteLine(t.text);
                return;
            }
            var index = new StringWriter();
            Index(index);
            var lines = index.ToString().Replace("\r", "").Split('\n');
            var start = Array.FindIndex(lines, l => l.StartsWith("  " + topic + " ", StringComparison.OrdinalIgnoreCase) || l.Equals("  " + topic, StringComparison.OrdinalIgnoreCase));
            if (start < 0)
                throw new WaitressException($"no help topic or command '{topic}'; topics: {string.Join(", ", s_Topics.Select(x => x.name))}");
            w.WriteLine(lines[start].Trim());
            for (var i = start + 1; i < lines.Length && lines[i].StartsWith("        ", StringComparison.Ordinal); i++)
                w.WriteLine("  " + lines[i].Trim());
            w.WriteLine("  options: --target, --float-mode, --float-precision, --safety (help options)");
        }

        public static int Skill(string install, TextWriter w)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BurstWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream ?? throw new WaitressException("skill text missing from the build"));
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
