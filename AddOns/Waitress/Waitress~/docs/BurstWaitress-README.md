# BurstWaitress

BurstWaitress is a command-line tool for the code Burst generates, and the tenth
Waitress tool. It's built for AI agents doing Burst performance and codegen
work. It answers, one call each:

-   Is this method or job Burst-compiled in this Editor right now, and if not,
    why? Pending, failed with the error, Burst disabled, or not eligible.
-   Did its hot loop vectorize across iterations, or only pack the math of one
    iteration (SLP)? How wide? Packed or scalar math?
-   Which calls survive into the generated code, especially math routines that
    stayed scalar library calls?
-   Is the loop versioned on a runtime alias check, and how large must the
    loop count be before the vector path runs?
-   How much does it spill to the stack?
-   What changed between two variants: two methods, two float modes, two
    targets?
-   Which C# lines do the instructions in a hot loop come from?

Without the tool, an agent reads raw disassembly, roughly 5k to 30k tokens for
one small job, and writes one-off reflection scripts. BurstWaitress compiles
through the same call the Burst Inspector uses, then reads the result itself
and prints a summary of 10 to 40 lines.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/BurstWaitress/release/burstwaitress[.exe]`.

Every command needs the Editor open with the Pipeline package (0.7 or newer)
the first time it sees an entry point, since only the Editor's Burst can
compile. The tool carries its Editor script inside itself, writes it to
`UserSettings/Waitress/BurstWaitress/`, and runs it through `run_script`, so
nothing is imported into the project. Dumps are cached in
`UserSettings/Waitress/BurstWaitress/Dumps/` per compile of the scripts, so
reading one again, or narrowing it differently, doesn't need the Editor.

Install the agent skill with
`burstwaitress skill --install <project>/.claude/skills/burstwaitress`.

## Burst settings are never changed

The tool reads `BurstCompiler.Options` and reports them. It never assigns
them. Assigning any of them, even to the value they already hold, invalidates
every Burst-compiled method in the project and starts a full recompile, and
synchronous compilation can wedge the Editor. The options a dump asks for, such
as target and float mode, go into that one compile's option string and nowhere
else.

## Naming entry points

-   A `[BurstCompile]` static method: `BwKernels.Mul`, or
    `BwKernels.Mul(float*, float*, float*, int)` for one overload.
-   A job struct: IJob, IJobFor, IJobParallelFor, IJobChunk, and whatever the
    generators emit for IJobEach and IJobEntity.
-   A closed generic job: `BwGenericJob<float>`, or the generic FindPairs jobs,
    which `list FindPairs` shows.
-   An ISystem method: `MySystem.OnUpdate`.

Full or unambiguous short names work. An ambiguous name lists its candidates,
and `sweep` and `list` take globs such as `"BwKernels.*"`. The names come from
the same scan the Burst Inspector makes, exported once per compile of the
scripts.

## Commands

| Command | What it gives |
|---|---|
| `status <name...>` | Per entry point: compiled, pending, failed with the error, burst disabled, or not eligible and why. Also the Editor's Burst settings, whether background compilation is running, and whether a benchmark here would time Burst code. |
| `dump <name>` | The summary: instruction classes, widest register, FMA count, spills, every loop (width and lanes, shape, unroll, elements per pass, packed %, alias check, the count it needs), calls with scalar math flagged, and the functions Burst didn't inline. |
| `dump <name> --raw [selector]` | Raw assembly for one part only: the main loop (the default), `loop:N`, `body`, `entry`, `fn:<name, number or glob>`, `lines:A-B`, or `src:A-B` (`src:File.cs:A-B` for one file). |
| `dump <name> --source` | Instructions and their classes per C# line, hot loops first, with the source text. |
| `diff <a> <b>` | Two dumps side by side with deltas, loops matched, calls added and removed. Two methods, or one method with one option given two values. |
| `sweep <names...>` | One row per method and option set: main loop width and shape, elements per pass, packed %, alias check, the count it needs, loops, calls, spills, FMA and size. Progress goes to stderr. |
| `list [filter]` | Entry points, with their `[BurstCompile]` settings. |
| `targets` | The targets this Editor's Burst compiles, each one tried. |
| `verify` | Acceptance checks against the validation kernels. |

`dump`, `diff` and `sweep` take `--target` (`avx2` by default, `avx`, `sse4`,
`sse2`, `auto`, `arm64`, `armv9`, `armv7`, `wasm`), `--float-mode` (the
method's own by default, `Default`, `Strict`, `Deterministic`, `Fast`),
`--float-precision` and `--safety` (`off` by default, as players run, or `on`
or `force`). `diff` and `sweep` take comma lists. Cross targets compile without
a device. Burst 1.8 has no AVX-512 target.

## Examples

```
burstwaitress status BwKernels.Mul BwManagedJob      # compiled · failed: BC1028 Creating a managed array at BwFailing.cs(17,13)
burstwaitress dump BwKernels.Channels8Plain          # main loop ymm×8 (8/iter), runs if n ≥ 40, alias check (59 instrs)
burstwaitress diff BwKernels.Channels8Plain BwKernels.Channels8NoAlias   # -77 instrs, n ≥ 40 → 8, no alias check
burstwaitress dump BwKernels.TransformPoints         # xmm×4 SLP (1/iter): one float3 per pass, not vectorized
burstwaitress dump BwKernels.Mul --target armv9      # SVE×VL/32 unroll 2 (VL/16/iter)
burstwaitress sweep "BwKernels.*" --target avx2,sse4 --float-mode Default,Fast
burstwaitress dump BwKernels.Sin --source            # which C# lines the 791-instruction main loop comes from
burstwaitress dump BwKernels.Sin --raw loop:3        # the remainder loop that calls Sleef_sinf_u35
```

## How to read a summary

`help reading` explains every field. The least obvious:

-   **Shape.** A packed register width doesn't mean the loop vectorized.
    `ymm×8 unroll 4 (32/iter)` covers 32 source iterations per pass. `xmm×4 SLP
    (1/iter)` packs the math of one iteration, such as a float4. `(span
    unknown)` means the per-pass count doesn't say which. The count comes from
    the loop's induction variable, and a struct that a pass reads less than half
    of is SLP.
-   **runs if n ≥ N.** The smallest value of the guarded count, usually the C#
    loop bound, for which that loop runs, read from the compare guards on every
    path to it. Below N the code takes a narrower or scalar path. For a loop
    from `i = 1` it is one more than the iterations. With the alias check in
    place, it is usually higher.
-   **alias check (K instrs).** The loop is versioned on a runtime test of
    whether its pointers overlap, and K instructions run before the loop on
    every call. `[NoAlias]` on a static method's pointer parameters removes it,
    and it survives inlining. It is reported only on pointer evidence; Burst
    already treats job containers as not aliasing, so jobs rarely have one.
-   **gather emulated: N lane loads.** An indexed load done one lane at a
    time.

## Limits

Read the "Known unsupported" section of
[BurstWaitress-DESIGN.md](BurstWaitress-DESIGN.md). In short:

-   it reports facts and doesn't time anything;
-   it can't say whether the Editor's Burst cache serves old native code, only
    that a dump is always the current code (on Burst 1.8.30 it never did);
-   guard counts and alias checks are read from compare guards, so unusual
    guard shapes, such as `long` loop counters, can be misread;
-   the shape is read from the induction variable and the bytes each stream
    touches, so a loop whose counter steps through a derived pointer can read
    `(span unknown)`;
-   WASM gets counts and loops only, without guards;
-   the first `status` on an assembly compiles all of its entry points once,
    which takes minutes for the largest assemblies.
