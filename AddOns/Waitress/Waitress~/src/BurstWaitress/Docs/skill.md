---
name: burstwaitress
description: Use for questions about the code Burst generates and whether Burst runs at all - did a loop vectorize and how wide (xmm/ymm), packed vs scalar math, which math calls stayed scalar library calls, SLP (packing within one iteration) vs vectorized across iterations, runtime alias checks and the count a vector loop needs, stack spills, what a float mode / target / code change costs, which C# lines the hot loop's instructions come from, and whether a job or [BurstCompile] method is Burst-compiled in this Editor right now (pending, failed with the error, disabled) - instead of reading Burst Inspector disassembly or writing reflection scripts.
---

# burstwaitress

`burstwaitress` compiles Burst entry points through the open Editor's Burst compiler (the same call the Burst
Inspector makes) and reports the generated code as compact facts. A dump costs the Editor once per compile of the
scripts and is cached; re-reading it (`--raw`, other selectors) is offline. It never changes Burst settings: per-dump
options go only into that one compile. Binary:
`UserSettings/Waitress/build/bin/BurstWaitress/release/burstwaitress.exe` (build it with `dotnet build
<addons>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build`). Run it from
the project folder.

## Naming entry points

A [BurstCompile] static method (`BwKernels.Mul`, or `BwKernels.Mul(float*, float*, float*, int)` for an overload), a
job struct (IJob, IJobFor, IJobParallelFor, IJobChunk, IJobEach, IJobEntity, closed generics such as
`BwGenericJob<float>`), or an ISystem method (`MySystem.OnUpdate`). Full or unambiguous short names; globs with `*`
for `sweep` and `list`. An ambiguous name lists the candidates. `list <filter>` shows what exists, e.g.
`list FindPairs` for Psyshock's generic FindPairs jobs.

## Commands

- `status <name...>` - is it Burst code in this Editor right now: `compiled`, `pending` (background compilation still
  running: it runs managed meanwhile), `failed` (with the error and line), `burst disabled` (globally or for its
  assembly), `not eligible` (and why: missing [BurstCompile], instance method, no such entry point). Also prints the
  Editor's Burst settings and whether a benchmark here would time Burst code. Burst compiles each assembly as one
  library, so one failing entry point makes the whole assembly run managed; status checks every entry point of the
  assembly once per build of it (a few minutes for Kinemation's 259; `--quick` skips that).
- `dump <name>` - the summary (about 15-40 lines), over the method's own code: the entry and every C# function Burst
  kept apart (for a job that is usually its `Execute`; Unity's producer dispatch loop is the entry). Loops are grouped by
  function, and the numbered function list lets `--fn N` (or `--fn Execute`, or naming `Job.Execute`) narrow the
  summary, `--source` or `--raw` to one function. The summary has:
  - instruction classes (packed/scalar float, packed/scalar int, moves, loads, stores, branches, calls), widest
    register, FMA count, spills;
  - each loop: instructions, width and lanes of its packed math (`ymm×8`, `SVE×VL/32`, or `moves only`), its shape,
    unroll and `(N/iter)` (what one pass advances, from the loop's induction variable: source iterations or lane
    elements, `B` for a pointer's bytes, `VL/16` on SVE), packed % (vector math calls count as packed), `runs if n ≥ N`
    (the smallest guarded count, usually the C# loop bound, for which that loop runs; one more than the iterations for a
    loop from 1), runtime `alias check` with its dump lines, `remainder`. Shapes: no tag = vectorized across iterations;
    `SLP` = packed within one source iteration (a float4, paired accumulators, a struct read one element per pass), not
    across iterations, including loops packed by hand (one `v256`/float4 per iteration, no remainder loop);
    `(span unknown)` = packed, but the per-pass count doesn't say. `main` is the innermost loop
    vectorized across iterations, else the widest;
  - calls, with `[scalar math]` flagged (a `math.*` call that didn't vectorize; `(remainder)` when it is only in the
    leftover loop, which is normal), the loop each sits in, and the functions they come from;
  - functions Burst didn't inline, with sizes.
- `dump <name> --raw [main|loop:N|body|entry|fn:<name|N|glob>|lines:A-B|src:[File.cs:]A-B]` - raw assembly, narrowed
  (default: the main loop). `lines:` takes the dump line numbers the summary prints. Entities journaling and string
  formatting functions (`infra`) are listed but not counted in the totals.
- `dump <name> --source` - per C# line: instruction counts and classes, hot loops first, with the source text.
- `diff <a> <b>` or `diff <a> --float-mode Default,Strict` - side by side with deltas, loops matched, calls added and
  removed.
- `sweep <names...> --target avx2,sse4 --float-mode Default,Strict,Fast` - one row per method × option set: main loop
  width and shape (`SLP`, `?` span unknown, `u4` unroll, `ord` ordered reduction), `/iter`, packed %, alias check, `n≥`,
  loops, calls (`sm` = scalar math outside remainder loops), spills, FMA, size. Use globs: `"BwKernels.*"`. Prints
  progress on stderr; Ctrl-C (or a timeout) stops the Editor's queue, and finished dumps stay cached.
- `targets` - the targets this Editor compiles. `verify` - acceptance checks against the BurstWaitress validation
  kernels. `--json` on status, dump, diff and sweep.

## Options (dump, diff, sweep)

`--target avx2` (default) `|avx|sse4|sse2|auto|arm64|armv9|armv7|wasm` (cross targets need no device; Burst 1.8 has
no AVX-512). `--float-mode attr` (default: the method's own) `|Default|Strict|Deterministic|Fast`.
`--float-precision attr|Standard|High|Medium|Low`. `--safety off` (default, as in players) `|on|force`. Comma lists
in `diff` and `sweep`.

## Notes

- Measured on Burst 1.8.30: `Default` float mode does not contract `a*b+c` into FMA (only `Fast` does); a
  `sum += a[i]*b[i]` reduction stays scalar unless the float mode is `Fast`; `math.sin` vectorizes inline on AVX2 and
  through vector Sleef calls (`Sleef_sinf4_u35sse4`) on SSE4, with scalar `sinf` only in the remainder loop; ARMv7 and
  WASM float loops stay scalar.
- A packed width alone doesn't mean vectorized: check the shape. `xmm×4 SLP (1/iter)` is one float4 per pass.
  `(span unknown)` loops need `--raw` to decide. On ARMv9 many loops stay NEON (128-bit, as on ARM64); SVE shows up for
  simple maps, masked stores and ordered reductions (`fadda` keeps a Default-mode float reduction in lane order).
- `alias check` needs pointer evidence (pointer compares or distances, `whilewr`), and quotes the instruction that decided
  it, so it can be checked in one `--raw lines:` call; job containers are already treated as
  not aliasing, so it shows up on static methods with raw pointers. A `gather emulated: N lane loads` note means an
  indexed load done one lane at a time.
- `[NoAlias]` on a static method's pointer parameters removes the alias check even after inlining.
- Dumps compile the current code. `status` notes when an entry point's code changed under the same Burst hash (callees
  or an edit-and-revert); on Burst 1.8.30 that never left old code running, so treat it as a lead, not a verdict.
- `Job.Execute` works as a name for dump and status; when Burst inlined Execute, dump shows the whole job and says so.
- Debug line info (`--source`) can shift code slightly; the summary without it is the reference.
- Never set `BurstCompiler.Options` (any assignment recompiles everything; synchronous compilation can wedge the
  Editor). The tool only reads them.
