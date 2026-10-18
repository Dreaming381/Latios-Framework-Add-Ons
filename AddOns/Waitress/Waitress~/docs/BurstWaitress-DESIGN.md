# BurstWaitress design

BurstWaitress answers questions about Burst's generated code in one call each,
from compact facts instead of raw disassembly. This document covers how it gets
the code, how it reads it, what was measured along the way, and how each claim
is checked.

## Sources

-   **`Unity.Burst.LowLevel.BurstCompilerService.GetDisassembly(MethodInfo,
    string options)`.** This is the engine call the Burst Inspector uses. It's
    internal, so the Editor script reaches it by reflection. It compiles the
    method from the assemblies on disk with the option string given and returns
    LLVM assembler text: Intel syntax on x86, plus every function the module
    emits (the entry stub, Burst's initializers, callees it didn't inline, and
    library routines such as Sleef).
-   **The Inspector's own target scan.** `BurstReflection.FindExecuteMethods`
    over `EditorAssembliesThatCanPossiblyContainJobs` returns the Inspector's
    list: static `[BurstCompile]` methods, and job types mapped to their
    producer's `Execute` (`IJobExtensions.JobStruct<T>.Execute`, and so on),
    including the closed generic jobs found by scanning generic instances. That
    covers IJobEach, IJobEntity and the generic FindPairs jobs, since those all
    end up as IJobChunk, IJobFor or IJob producers. ISystem methods appear as the
    generated `__codegen__OnUpdate(IntPtr, IntPtr)` statics.
    1423 targets in this workspace, in 1.3 s.
-   **The Inspector's option string.** `BurstCompilerOptions.TryGetOptions(member,
    isForCompilerClient: true)` on a clone of the global options. This is what
    the Inspector calls, with the attribute's settings and the assembly's merged
    in. The clone is constructed non-global, so its setters trigger nothing.
    The tool then replaces the lines it overrides.
-   **State, read only.** `BurstCompiler.IsEnabled`, `Options.*`,
    `ForceDisableBurstCompilation`, `BurstCompiler.IsCurrentCompilationDone()`
    (internal), the Editor's `Progress` item named "Burst", and
    `ProjectSettings/Burst_DisableAssembliesForEditorCompilation.json`.

The Editor half is one embedded C# file, run through `run_script`. It writes the
target index (`index.tsv`) once per code epoch, and writes dumps as
`<key>.asm` plus `<key>.meta` into `UserSettings/Waitress/BurstWaitress/Dumps/`.
The meta is written last, so a meta on disk means its dump is complete. The key
hashes the code epoch (every script assembly's name, size and write time, the
Burst package, the Editor version), the Editor script's own text, the entry
point and the options. Everything else, parsing, analysis and output, runs in
the CLI, offline.

## Never writing Burst options

Setting any `BurstCompiler.Options` property, even to its current value,
invalidates every Burst method and starts a project-wide recompile, and setting
`EnableBurstCompileSynchronously` has wedged this Editor for over an hour. The
Editor script contains no assignment to any of them. Per-dump settings exist
only as lines in the one `GetDisassembly` call's option string. `verify` reads
the options before and after a full run and requires them unchanged.

## Staying inside the Pipeline's 30 s limit

`run_script` work on the main thread is killed at about 30 s, and an aborted
script never runs its `finally`. Compiles therefore run on a background task in
the Editor (`GetDisassembly` works off the main thread, as the Inspector already
uses it). The script waits up to 20 s and returns either way. The CLI then
waits for the meta files on disk, without further Editor calls. A sweep that
outlasts `--timeout` keeps every dump that finished, so running it again picks
up where it stopped. One compile runs at a time: the lock lives in the
AppDomain's data, since every `run_script` is a new assembly.

## Reading a dump

1.  **Functions.** A top-level label followed by instructions is a function.
    `.LBB` and other local labels split blocks. `/EXPORT:` names the entry.
    When the entry is a stub of 12 instructions or fewer that calls one function
    defined in the dump (which happens with line info on), that function is the
    body.
2.  **Classes.** Each instruction gets one class: packed float, scalar float,
    packed int, scalar int, move, branch, call, ret, or other. Load and store
    are separate flags, so a packed multiply with a memory operand counts as
    both packed float and a load. Zeroing idioms (`vxorps x, x, x`) count as
    other. Float lane shuffles (`vshufps`, `vpermilps`, `vinsertf128`,
    `vmovlhps`) count as packed float, and `vp*` as packed int. ARM uses the
    vector arrangement (`v0.4s`) and `s`/`d` registers. SVE `z` registers are
    vectors of a length set by the CPU (128 bits at least): they count at 128
    bits and are marked scalable, and a loop is SVE when most of its packed math
    is (NEON loops can use a few SVE2 bit operations on the same registers).
    WASM uses its type prefixes.
3.  **Loops.** Natural loops: an edge to a block that dominates its source.
    Loops with the same header are merged, and nesting comes from block-set
    containment. Rotated loops, entered in the middle, are found the same way,
    because dominance doesn't depend on layout.
4.  **Guards.** For each loop, every conditional branch on all paths to its
    header whose other edge can't reach the header is a guard. Two kinds:
    -   an *alias check*, when the block has pointer evidence: 64-bit lane
        compares (`vpcmpgtq`; masks of data compares, `vmovmskps` of an
        `isfinite`, aren't), SVE `whilewr`/`whilerw`, a compare of a
        pointer distance (a register defined by subtracting two 64-bit values,
        through copies and shifts; SVE compares it against `rdvl`), or a
        compare of two 64-bit registers next to an end pointer (`lea` with an
        index) or a `setcc`, where neither holds a constant set by a move
        (ARM's `mov x11, #-1073741824` overflow guards). A call defines its
        return register, so a pointer an allocator returned stays a pointer.
        A compare counts only when its flags are read with an ordering
        condition (`jae`, `setb`, `b.hs`): an overlap test orders pointers,
        while `eq`/`ne` compares a cached value (Entities' component lookup
        cache: `cmp x11, x10; b.ne`). The summary quotes the instruction that
        decided it ("1787 cmp r8,
        r10: pointer against pointer"). Non-overlap is often a disjunction (`a_end <= b`, or `b_end <= a`),
        where both edges of each compare reach the loop: such a block is
        still an alias check. Guards after a known alias check that share its
        fallback target are part of it. A count test folded into the check
        (`cmp r8d, 9; setb; or; jne fallback`) still bounds the count. Burst
        widens int counts to 64 bits, so a 64-bit register against a constant
        is not evidence by itself, and neither is the difference of two values
        last written as 32-bit (`mov r11d`, `cmovl r11d`);
    -   otherwise, a compare of a register with a constant (`cmp r9d, 40`,
        `test r9d, r9d`, ARM `cmp w2, #24`, `cbz`) bounds the guarded count
        from below, according to the branch condition and which edge reaches
        the loop. `runs if n ≥ N` is the largest such bound. A count register
        derived as the count plus a constant (`sub x9, x8, #1`,
        `lea ecx, [rdx - 1]`, `not edx; add edx, r8d`) is corrected back to
        the count, and an `n ≠ v` guard on the way raises a bound of `v` by
        one. Count tests set into flags and or'ed (`cmp r8d, 12; setb; or;
        je vector`) bound the count whether or not an alias test shares the
        branch. An SVE count clamped from below (`csel x8, x11, x8, hi` after
        `mov w8, #16`) gives `n ≥ max(VL/16, 16)`. It is a bound on
        the count the code compares (usually the C# loop bound), not on
        iterations: a loop from `i = 1` guards `n ≥ 33` for 32 iterations. On
        SVE the constant is an element count (`cnth x9`, `cmp x9, x8`), so the
        bound scales with the vector length (`n ≥ VL/16`), and `whilewr` /
        `whilerw` are alias checks.

    The flags come from the last flag-setting instruction before the branch.
    ARM schedules a `mov` between `cmp` and `b.hs`.
5.  **Per pass, shape and unroll.** A symbolic walk of the loop body tracks
    each general register as its value at the header plus a constant (`mov`,
    `movsxd`, `lea r, [s + k]`, `add`/`sub`/`inc`/`dec` with constants, ARM
    writeback addressing, SVE `incb`/`dech` as vector-length multiples). A
    register that comes back to itself plus a step is an induction variable.
    The walk follows one path from the header to the latch (through the
    successor that dominates the latch at each fork), so increments on
    exclusive branches count once. A register stepped this pass plus one not
    yet changed (`add x16, x11, x13`) keeps the stepped one's base. The
    loop's counter is the induction variable behind the register its back
    edge compares, or else an exit branch on the path (`cmp r9, r10; jge
    out` in the header). That is often a derived value: hand-written
    `i += 8` loops compare the next pass's `i + 8`. What one pass covers then depends on how
    the counter addresses memory:
    -   never (a pure counter): the step is source iterations;
    -   as an index with a scale: the step counts elements of that size;
    -   as a pointer or an unscaled index: bytes (`16 B/iter`). Such a loop
        counts as vectorized, at bytes over lane size, only with the loop
        vectorizer's other marks: a scalar loop beside it in the same function,
        or a guard requiring exactly that many lanes.

    Every induction variable's memory accesses form streams, each with its
    own stride (the variable's step times that access's scale). If any
    stream touches under half its stride each pass, the loop walks a struct
    one element at a time. Full-vector accesses at distinct offsets in one
    stream give the unroll, and the lane size when it is smaller than the
    math's (`vorps` on a byte mask: `ymm×32 unroll 4`). A pointer-stepped
    packed loop with no vectorizer marks that moves one vector or less per
    pass is SLP: a float4 per iteration. A loop that only moves vectors (a
    vectorized copy, no math) is vector work at the move width, tagged
    `copy`. A counter that addresses streams of different scales (int32 in,
    ushort out) counts bytes of the smallest, and the vectorizer's guard
    gives the elements. So is a vector-shaped loop with none
    of the loop vectorizer's marks (a scalar loop beside it, a guard for the
    lanes it covers, `while*` predication, vector math calls): it is packed
    by hand, one `v256` or float4 per source iteration (`hand-packed` in
    JSON). An outer loop is judged by its own instructions, not its inner
    loops'. A scalar loop laid out before a vector loop is its remainder when
    the vector loop's exit leads into it. Shape then decides
    the label: **vector** when one pass covers a whole number of vectors
    (unroll = elements over lanes, at most 16), **SLP** when it covers one
    element or walks a struct, **span unknown** otherwise (a pointer step of a
    vector or more, a batch stride).
6.  **Spills.** Stack slots (`[rsp + n]`, `[rbp - n]`, `[sp, #n]`) that the body
    both writes and reads. Slots that are only written are outgoing call
    arguments. Slots that are only read are incoming stack arguments.
    Prologue and epilogue saves of callee-saved registers are excluded (SEH
    markers on Windows, otherwise the runs of saves at entry and restores
    before each `ret`).
7.  **Calls.** Grouped by target. `[scalar math]` means a Sleef or libm routine
    without a lane count (`Sleef_sinf_u35`), and `[vector math]` one with
    (`Sleef_sinf8_u35avx2`). A vector math call counts as packed work at its
    width (`sinf4` is 128 bits of 32-bit lanes). Non-inlined C# methods, runtime
    helpers and indirect calls are the other kinds. Each call records the
    innermost loop it is in, and whether every site is in a remainder loop (a
    scalar loop beside a vector loop): a scalar math call there is the
    leftovers, not a vectorization miss, and `sweep`'s `sm` count leaves it out.
8.  **The main loop** is the innermost loop in the method's own functions (a
    job's producer dispatch body ranks with other code, not first; an atomic
    retry loop ranks last, and when every innermost loop is one, the loop
    around it with packed work of its own competes),
    ranked by namespace tier, then shape (vector, span unknown, SLP, scalar;
    a span-unknown loop that makes calls, such as a dispatch or bit-walk
    loop, ranks below SLP),
    width, and packed instruction count. Entities journaling and Burst or
    FixedString formatting functions are listed as `infra` and left out of the
    totals.
9.  **Source lines** come from `.cv_loc` (`.loc` off Windows) with `--debug=2`.
    That only happens with `--source` or `--raw src:`, because line info
    changes the code's shape (see below).

## Measured facts

All measured in this workspace on Burst 1.8.30, Unity 6000.3.24f1, x64 host.

-   **The Burst error that fails an assembly.** Burst compiles each assembly
    into one library. One entry point that fails to compile
    (`BwManagedJob`'s `new int[4]`, BC1028) made *every* Burst method in
    `Latios.Validation.BurstWaitress` run managed. A probe job reported
    `burst = False` for as long as the failing job shared its assembly, and
    `True` once the failing job moved to its own assembly. The Editor log shows
    the whole assembly's method list under "While compiling job:". For this
    reason `status` compiles every entry point of the assembly once per build
    of it (`AssemblyChecks/`). The failing job now lives in
    `Latios.Validation.BurstWaitress.Failing`, next to `BwFailingNeighbor`, an
    innocent method that `status` must report as failed.
-   **The cost of that check.** Kinemation has 259 entry points: about 0.75 s
    each with the real options, and 4 minutes for the whole assembly the first
    time. Unoptimized compiles (`--disable-opt`) are 2.3× faster and catch the
    same front-end errors, so the check uses them, and confirms any failure
    with the real options, since optimization can remove a managed allocation.
    Parallel `GetDisassembly` calls are safe and give identical output, but no
    speedup: 6.9 s sequential against 6.4 s with 4 threads, so the compiler
    serializes internally. Cached, a status call takes about 2 s.
-   **The console.** This time the eager-compilation error did reach the Editor
    log. Hazard 3 holds anyway, because the console can be cleared and the
    error names only the failing job, never the neighbors that also run
    managed. `status` compiles; it doesn't read the console.
-   **`EagerCompilationStatus` is unreliable.** After a domain reload that had
    nothing to compile, it stays `NotScheduled` indefinitely. `pending` uses
    `IsCurrentCompilationDone()` and the "Burst" progress item instead.
-   **When pending happens.** A script edit is usually not a pending window.
    Even 151 freshly changed heavy kernels were compiled before `unity
    recompile` returned: Burst compiles each assembly as it finishes compiling,
    before the domain reload. A probe job ran as Burst 2 ms after the reload.
    Changing a project-wide Burst setting is different. Switching safety checks
    off from the Jobs menu recompiled all 56 libraries in the background over
    about a minute:
    -   the probe job ran *managed* at first;
    -   `status` said `pending` ("Compiled 26 / 56 libraries");
    -   the job ran Burst again once its own library was done (about 31 of 56).

    Burst exposes no per-library state, so `status` stays `pending` until all
    of them finish. That is conservative but never wrong. Probe:
    `AgentScripts/BurstWaitress/PendingProbe.cs`. Safety checks were set back
    to On afterwards, also from the menu.
-   **Option strings.** Without a `--target` line, `GetDisassembly` returns an
    empty dump. The Inspector always passes one (`--target=Auto` for the host).
    The same option on two lines (two `--target`s, for example) also gives an
    empty dump, so overrides replace their line. `--target=AVX512` gives an
    empty dump too: `BurstTargetCpu` has no AVX-512 member. The cross targets
    all compile without a device (`targets`): SSE2, SSE4 (x86 and x64), AVX,
    AVX2, ARMv7 NEON32, Thumb2, ARMv8 AArch64 (and HalfFP), ARMv9 and
    WASM32/64.
-   **Dumps are always fresh.** After editing only a non-inlined callee, the
    next dump showed the new constant. The entry function's hash name
    (`7036149a…`) did not change, though: the hash covers the entry's own IL
    and options, not its callees.
-   **Hazard 4 not reproduced.** Three attempts to make the Editor run stale
    native code all ran the new code, checked with a job that writes a
    callee's result and a `[BurstDiscard]` flag (`BwStaleProbe`):
    -   a callee-only edit in the same assembly (16 → 40);
    -   a callee in another assembly (33);
    -   edits made while Burst compilation was off, then switched back on (40,
        51).

    `status` therefore doesn't warn about stale code. It notes when an entry's
    code changed under the same hash, which is the precondition, and says what
    to do if the Editor behaves like the old code.
-   **Codegen.**
    -   `c[i] = a[i] * b[i]` gives three loops: `ymm×8` unrolled 4 times (32
        elements per iteration, minimum trip 32), `xmm×4` (minimum 4), and a
        scalar remainder.
    -   `sum += a[i] * b[i]` stays scalar under `Default` and `Strict`, and
        vectorizes with FMA under `Fast`.
    -   Eight sub-ranges of one buffer, passed to a static method without
        `[NoAlias]`: the alias check is 59 instructions, and the minimum trip
        count is 40 against 8 with `[NoAlias]`. On ARM64 it is 24 against 4,
        with the check made of `cmp x, #16` distance compares.
    -   `Default` doesn't contract `a*b + c` into FMA on AVX2. Default and
        Strict produce identical code, and only `Fast` emits `vfmadd`.
    -   `math.sin` over an array vectorizes inline on AVX2: a 791-instruction
        ymm loop with 47 spill stores, plus a scalar `Sleef_sinf_u35` call
        only in the remainder loop. On SSE4 and ARM64 the loop calls the vector
        routine (`Sleef_sinf4_u35sse4`, `Sleef_sinf4_u35advsimd`) twice per
        pass, 8 elements, and only the remainder calls the scalar one.
    -   ARMv9 compiles some loops to SVE: simple maps (`ld1b {z0.b}` /
        `fmul z0.s`, steps of `dech`/`incb`, a `cnth` guard: vector-length
        agnostic), masked stores (`st1w {z}, p1`) and ordered reductions. Many
        others stay NEON, instruction for instruction the same as ARM64: a SoA
        float3 length, a select with `sin`, a gather, a float4 loop, the
        Calligraphics rasterizer. A `Default` float reduction there vectorizes the
        multiplies and keeps the adds in lane order with `fadda`. On ARM64 (v8)
        the same reduction packs the multiplies and adds the lanes one by one in
        scalar (20% packed).
    -   One `float3` or `float4` per pass (TransformPoints, RotateAll, the
        Pressure kernel) packs within the element (SLP) on every target: one
        pass, one source iteration.
    -   ARMv7 (NEON32) and WASM float loops stay scalar.
    -   Line info (`--debug=2`) makes the entry a stub that calls the body, and
        changes loop shape slightly (for example `add rdx, -32; jne` in place
        of `cmp; jne`).
-   **Calls from `run_script`.** A static `[BurstCompile]` method called through
    reflection from `run_script` reported `IsBurst() = False`. That was while
    its assembly's library was failing, so it is explained by the first fact
    above, and wasn't retested afterwards. Jobs scheduled from `run_script` do
    run Burst code.

## Verification

-   **Census against the raw dump.** `Testing/Census.cs` is a second reading
    of the raw text, written separately from the parser and classifier, using
    only line regexes. For every validation kernel (19 entry points across
    statics, all job kinds, a generic job, an ISystem and a function pointer
    target), `verify` requires the summary's totals to equal it: instructions,
    packed and scalar float, loads, stores, branches, calls and ymm lines. It
    also requires each innermost loop to match the backward branches in the
    text. The census found three bugs of its own, all fixed, and none in the
    analyzer:
    -   it rejected mnemonics with `_` (`vcmplt_oqps`);
    -   it treated `vaddps x, x, x` (that is, `x * 2`) as a zeroing idiom;
    -   it counted `vmovlhps` as a move.

    Loop boundaries were also read by hand from the branch targets for Mul,
    Channels8Plain and BwForJob. BwForJob's loop is rotated: it is entered at
    `.LBB0_5` from above, so its back edge isn't a single backward branch.
-   **Known codegen facts reproduced** (`verify`, and offline on recorded
    dumps in `selftest`):
    -   packed ymm for `a[i] * b[i]`;
    -   the scalar `Default` reduction, with a `Fast` control that vectorizes;
    -   the alias check and `n ≥ 40` guard without `[NoAlias]`, and 8 with it,
        cross-checked against `vpcmpgtq` and `cmp r9d, 40/8` in the raw text;
    -   SLP for one float3/float4 per pass (three kernels);
    -   8 per pass, unroll 2, for hand-written `i += 8` SIMD (checked against
        `add e.., 8` in the raw text), and `n ≥ 33` for a loop from `i = 1`
        (against `cmp .., 33`);
    -   SSE4 `math.sin`: vector Sleef calls as the packed work, the scalar call
        only in the remainder;
    -   ARMv9 Mul: SVE, with the step and the guard scaled by the vector
        length (against `fmul z..s` and `cnt*` in the raw text);
    -   guards: no alias check and n ≥ 32 for a clamped int range (against
        `sub r, r` / `cmp r, 32`), n ≥ 32 behind `not`/`add` (against
        `cmp e.., 31`), n ≥ 9 folded into an SSE4 alias check (against
        `cmp .., 9` / `setb`), and the SVE `rdvl` check with n ≥ VL/16+1;
    -   float4 through a pointer is SLP; SoA float3 length vectorizes 8/iter
        while AoS packs one point;
    -   no alias check for a data lane mask (`vmovmskps` in the raw text) or
        an ARM overflow guard (`mov x.., #-1073741824`), n ≥ 12 from or'ed
        count tests (`cmp .., 12` / `setb`), and a hand-packed `v256` loop.
-   **Float modes.** The FMA count for MulAdd under Default, Strict and Fast
    equals a raw `vf(n)m(add|sub)` count of the body: 0, 0 and 6.
-   **Status honesty.**
    -   `BwManagedJob` reports failed, with BC1028 and its line.
    -   `BwFailingNeighbor.Twice` reports failed, naming the assembly's
        failure.
    -   `BwPlainJob` (no `[BurstCompile]`) and `BwKernels.Opaque` (no attribute
        on the method) report not eligible, and say why.
    -   Pending was reported while a probe job really ran managed, during the
        background recompile that a safety-checks change starts (see Measured
        facts).
-   **No side effects.** `verify` snapshots the Burst options, a hash of every
    script assembly's write time, and the number of dirty scenes, before and
    after. All were unchanged.
-   **Mutation checks.** Breaking the trip-count bound (N-1) fails 5
    self-tests. Disabling the 64-bit distance rule fails the ARM64 alias test.
    Dropping `lea` from the induction walk fails the hand-written SIMD test.
    Calling 1-per-pass loops vector fails both SLP tests. Treating every
    value as a possible pointer fails the widened-count test, and dropping
    folded count tests fails the `setb` test. Dropping the disjunction rule
    fails both Myri tests; dropping both constant-register checks fails the
    ARM overflow-guard test (each check alone covers it). Accepting eq/ne
    compares as alias evidence fails the lookup-cache test.
-   **Self-tests.** `burstwaitress selftest` runs 54 offline checks on 31
    recorded dumps (`Testing/Data`, paths scrubbed): operand splitting,
    symbol names, every loop fact above on x86, ARM64 and ARMv9 SVE (with an
    independent regex count of the SVE loop), spills, per-pass counts and
    shapes (byte-offset counters, hand-written SIMD, float3 per pass, a
    48-byte struct read 12 bytes at a time from Kinemation, a float4 array
    through a pointer, exclusive-branch increments), guards (a widened int
    count, a count folded into the alias check, the SVE `rdvl` distance and
    `n ≥ VL/16+1`, `n ≠ 1`, Myri's disjunction alias check, `max(VL/16,
    16)`, a data lane mask, an ARM overflow guard, or'ed count tests), a
    hand-packed `v256` loop, a remainder laid out first, vector math calls,
    `fadda`, line mapping, the census on all x86 fixtures, option
    parsing and name resolution.

### Token budget

| Output | Size | Raw dump |
|---|---|---|
| `dump BwKernels.Mul` | 653 chars | 3,339 |
| `dump BwKernels.Channels8Plain` | 568 | 7,092 |
| `dump BwKernels.Sin` | 1,026 | 61,482 |
| `dump BwMulJob` | 611 | 3,416 |
| `sweep "BwKernels.*" --float-mode Default,Strict,Fast` (48 rows) | 6,098 | about 370,000 over the 48 dumps |

A summary takes about 150 to 250 tokens. A sweep row takes about 30. The
shape, `/iter` and `n ≥` fields added after round 2 cost about 40 characters
per summary.

## Subagent rounds

Each round ran one subagent at a time.

**Round 1 (4 productivity tasks on framework code, about 48 calls).** The tasks:
-   Psyshock FindPairs on AVX2, and what Strict costs (nothing: the code is
    byte-identical; Fast adds two FMAs);
-   a Kinemation culling job: compiled, and its hot loops are xmm×4 SoA plane
    tests;
-   Shuriken's simulate job: seven scalar Sleef call sites;
-   AVX2 against SSE4 for an upload-size reduction.

Fixed afterwards:

-   **The analysis covered only the entry.** For IJobChunk, IJobFor and
    IJobParallelForDefer jobs, the entry is Unity's producer dispatch, and the
    job's `Execute` is a separate function. About 70 Shuriken and Kinemation
    IJobChunk rows in one sweep were identical: they described
    `JobChunkProducer`. The summary now covers the method's own code: the
    entry plus every C# function it reaches. Loops are grouped by function and
    numbered across them, and the main loop is picked among all of them.
    Functions in the entry point's own namespace rank first, so Entities
    journaling and Burst string formatting, which ride along in Editor
    compiles, can't win. `--fn N` and `Job.Execute` narrow everything to one
    function.
-   **Width came from moves.** A loop whose only ymm instructions were struct
    copies read "ymm×8". Width now comes from packed math only, and "ymm
    moves only" is shown when there is none.
-   **Unroll and elements per iteration came from address patterns.** That
    gave 8/iter for a 32-per-iteration multi-accumulator reduction, and 64/iter
    for an 8-per-iteration one. They now come from the counter the loop's back
    edge tests: `add rsi, -32` means 32 per iteration. When the counter
    addresses memory, its byte step times its scale, over the lane size, is
    used instead. The displacement heuristic was removed.
-   **`diff` crashed** on two functions with the same display name, and paired
    loops by index. Calls are now grouped by name, and loops are paired by role
    (function, float/int/scalar, depth), widest first.
-   **A call was missing.** A caller whose symbol contained `[` (generic
    arguments) was taken for an indirect call, and its calls (`Sleef_powf`)
    were dropped. Calls now also say which functions make them.
-   **Names.** Reflection generics (`FindPairsInternal[[X]]`) are now `<X>`.
    Primitive names are replaced only at word boundaries
    (`FrustumCullSystem.SingleSplitCullingJob` had become
    `FrustumCullfloatSplitCullingJob`). Nothing is truncated with `--all`.
-   **Smaller things:**
    -   alias checks are only reported on vector loops;
    -   `fn:` with an ambiguous name lists the candidates with numbers;
    -   `lines:A-B` marks function boundaries;
    -   `status` prints the assembly check's result, and progress while it
        runs;
    -   sweep columns are separated, and names have at least two segments;
    -   `list` says up front when it caps the output.

**Round 2 (exploratory, about 48 calls; 18 for a sweep, diff and source
workflow shaped like the SPMD research).** It confirmed against the raw text:
-   Channels8Plain's trip 40 and 59-instruction alias check;
-   spills on Pressure Fast, counted line by line;
-   DotFast's 32/iter with four FMAs;
-   the loop placement of Sin's vector and scalar calls;
-   TwoLoops, Mul, and ARM64's 20% packed ordered reduction;
-   name resolution and option validation throughout.

The findings, most severe first, and what became of them (fixed in the
session after the usage window reset, unless marked open):

1.  **Packing within one iteration reads as vectorized.** Fixed: loop shapes
    (see "Per pass, shape and unroll"), an `/iter` column and `SLP` in the
    sweep, bytes for pointer steps. The original report: A loop that advances
    one source element per iteration but uses some packed opcodes (a
    `vblendvps` for NaN handling, SLP over a float4 or a lo/hi pair) shows
    `xmm×4` and a packed percentage in `sweep`. `dump` prints `(1/iter)`, but
    the sweep drops it. The fix: a per-iteration column in the sweep, and
    "SLP"/"scalar" for 1-per-iteration loops. That needs `ElemsPerIter` to mean
    source iterations. It does when the counter is a pure counter, or an index
    with a scale. It doesn't when it's a pointer stepped in bytes, which
    should then be reported as bytes.
2.  **ARMv9 SVE isn't classified.** Fixed: z registers, VL-scaled steps and
    guards, `fadda`. Predicated `whilelo` loops (no remainder) are parsed, but
    no recorded dump has one yet, so their per-pass reading is untested. `z` registers, `ld1b {z0.b}`, `fmul z0.s`
    and `incb` read as scalar, and SVE's vector-length-dependent trip guards
    (`cnth`) aren't understood.
3.  **Loops vectorized through vector math calls read "moves only".** Fixed,
    and the legend counts only scalar calls outside remainder loops.
    `Sleef_sinf4_u35sse4`, called twice per pass on SSE4 (8 per iteration),
    should count as packed work. The `(N sm)` legend also fires when the only
    scalar call is in the remainder loop.
4.  **Hand-written SIMD loops.** Fixed by the induction walk, and the main
    pick prefers vector loops over SLP ones. They (`i += 8` with an `i + 8 <= count` guard) got
    the next iteration's guard increment as their step (16/iter instead of 8).
    The main-loop pick can then prefer a scalar tail that contains one packed
    blend.
5.  **`min trip`.** Redefined and renamed `runs if n ≥ N` (JSON `minCount`).
    It was the guarded count value (33 for a loop that starts at
    i = 1, so 32 iterations), not always source iterations. Either compute
    iterations or redefine the field as "count ≥ N".
6.  **`Job.Execute` when Execute is inlined.** Fixed as proposed:
    -   `dump BwMulJob.Execute` fails ("no function matching"). It should fall
        back to the whole job, with a note that Execute was inlined.
    -   `status BwMulJob.Execute` says "instance method, not eligible". It
        should resolve to the job.
7.  **Long sweeps.** Fixed: progress on stderr every 15 s. The tool touches
    its request file while it waits and deletes it on exit or Ctrl-C, and the
    Editor's queue stops when the file is gone or untouched for 45 s. The
    report:
    -   no progress output for 90 s on 259 Kinemation entries;
    -   an abandoned request keeps the Editor's background queue busy, so the
        next call waits behind it. A request should be cancellable, or skipped
        when superseded.
8.  **Jobs whose Execute is inlined into the producer.** Partly fixed: the
    alias check's dump lines are printed, and vector loops outrank the scalar
    dispatch loops. Open: an inlined Execute with no packed loop can still have
    the dispatch loop as main. The report: the dispatch loops
    can win "main" again. Nested non-vector outer loops carry the same 5-instr
    "alias check". The alias region's lines should be exposed so this can be
    checked.
9.  **Editor-only code in Entities jobs.** Partly fixed: journaling and
    string-formatting functions are left out of the totals and named. Open:
    safety code inlined into the method's own functions is still counted. The
    report: Journaling, safety handles
    (`m_Safety1`, `burst_abort`) and FixedString formatting are counted, but
    players strip them, so "safety off (as in players)" overstates player
    parity. They should be tagged or excluded in the totals.
10. **Status.** Fixed: the note says what the hash covers and that staleness
    wasn't reproduced; "not called yet" is worded as "no native code loaded
    yet". The report:
    -   one probable false "code changed under the same hash" note, on
        `BwKernels.Straight`, which has no callees. It most likely came from my
        own edit-and-revert of that kernel during the pending test, which is
        exactly what the note describes, but worded wrongly ("only callees
        changed");
    -   "compiled (not called yet this session…)" reads as a contradiction.
11. **JSON.** Fixed: `--json` with `--raw`/`--source` is an error, `diff
    --json` has `delta`, `loops` and `identical`, and missing counts are null.
    The report:
    -   `--json` ignores `--raw` and `--source`;
    -   `diff --json` lacks the deltas and loop matches;
    -   a missing min trip is 0, not null.
12. **Small things.** Fixed except the overload suggestions and the loop
    cap; diff also matches a math routine across targets (`Sleef_sinf_u35_avx2`
    and `_x64_sse4`). The report:
    -   `src:` has no file qualifier;
    -   empty `lines:`/`src:` ranges exit 0 silently;
    -   `fn:` takes no globs;
    -   identical remainder loops in a diff aren't matched;
    -   `--timeout 0` is accepted;
    -   the "similar" suggestions for overload misses are poor;
    -   `dump` of a job without `[BurstCompile]` says "compile FAILED" where
        status says "not eligible";
    -   the loop cap hides loops that the calls list references;
    -   `list "*"` and `sweep "*"` count differently (sweep drops non-Burst
        jobs, but doesn't say so first);
    -   packed integer loops show `packed -` in the sweep.

It also asked for helpers that aren't entry points (Mathematics and Psyshock
math without a wrapper), and for a cost estimate in the style of llvm-mca.
Both are outside the requirements, and are noted for later.

**Round 3 (35 calls; SPMD-shaped: four framework jobs and seven new kernels in
`Assets/Validation/BurstWaitress/Round3/`, each predicted before dumping).** It
confirmed against the raw text: SoA vs AoS float3 length (ymm×8 8/iter vs SLP),
the masked-store kernel on AVX2 (`vmaskmovps`, n ≥ 33), SVE predicated stores
and `fadda`, Sleef vector calls with `(remainder)` tagging on SSE4 and ARM64,
the hot loops of Kinemation's culling jobs (SLP plane tests, never across
entities), the vectorized pixel loop of Calligraphics' rasterizer, and every
error message it tried. Fixed after it:

1.  **False alias checks** (high): widened int counts compared with a constant,
    a `math.csum` followed by `setne`, and a bit-scan `cmp rdx, 63` read as
    pointer tests. The sweep showed `alias yes` for 69 Kinemation jobs; after
    the evidence rule above, 0 (they are Entities chunk-mask loops; Burst
    treats job containers as not aliasing). The real checks (Channels8Plain,
    the masked-store kernel, the rasterizer's byte-output loop) remain.
2.  **Wrong `n ≥`** (high): counts folded into the alias check (`setb`), an
    equality branch (`n ≠ 1`), a guard among scalar code, and `n - 1`
    against `cnth`.
3.  **A float-to-byte loop read as SLP** (high): strides now come per stream.
4.  **Float4 hand loop `span unknown` on x86, SLP on ARM**; a per-entity loop
    read as 2/iter; a remainder with an increment on each of two exclusive
    branches read as 2/iter; `unroll 16` for a byte-mask loop that is 4 ymm
    registers of bytes; an SVE pointer-distance alias check missed; SVE
    `(span unknown)` when the counter was `x11 + x13` (all medium).
5.  **Abandoned requests** (medium): a killed sweep held the next dump for
    40.8 s with no message. The tool now touches its request every 2 s from
    the moment it writes it, the Editor drops a request untouched for 12 s,
    and a request behind another says so on stderr at once (measured: 12.6 s
    after a kill, with the message). Polling checks only the dumps still
    missing.
6.  **Docs**: ARMv9 is often NEON, not SVE; `--timeout` and progress are in
    `help`; JSON's scalable counts are explained.
7.  **Low**: outer loops showed their inner loops' packed math; SVE moves read
    `128-bit`; `diff` and JSON missed `SVE` as the widest register; emulated
    gathers weren't noted (now `gather emulated: N lane loads`); `--source`
    took the main file from infra code; `src:` printed a bare count.

Open from round 3: a moves-only main loop next to a scalar remainder isn't
tagged; sweep rows still print only at the end (progress goes to stderr, and
finished dumps are cached). (A scalar loop laid out before a vector loop with
an alias check is now tagged `remainder`: it is that check's fallback too.)

**Round 4 (41 calls; Psyshock, Myri and Shuriken jobs, and adversarial
kernels in `Assets/Validation/BurstWaitress/Round4/`).** It confirmed shapes,
widths, unrolls and per-pass counts everywhere it checked: a `long`
count-down loop, a stride-2 `vhaddps`/`ld2` loop, a nested 2D loop with a
runtime width, an early-exit search (scalar on every target: this Burst
doesn't vectorize early exits), a `ref NativeArray` loop (scalar, a pointer
reloaded per iteration), a `[SkipLocalsInit]` stack buffer (not counted as
spills), Myri's mixer (ymm×8 u4, SVE u2), and Psyshock's AVX sweep. Fixed
after it:

1.  **Alias checks missed** (high): Myri's `output[i + start] += input[i] * w`
    over two Spans (a disjunction of two range compares), and Shuriken's
    noise loop (its pointer came back from an allocator call, and the scan
    read an earlier `mov eax, eax` as its definition).
2.  **False alias checks** (high): data lane masks (`vpcmpgtw`/`vpmovmskb`
    of terrain heights, `vmovmskps` of `isfinite`) in a function's entry, and
    an ARM overflow guard against a constant held in a register.
3.  **`n ≥ 1` on vector loops** (medium): or'ed count tests outside alias
    checks, and SVE's `max(cnth, 16)`.
4.  **Main pick** (medium): a dispatch loop of struct copies with calls, and
    a bit-walk loop calling the processor, won over Psyshock's sweep loop.
5.  **`--json` ignored `--fn`**; JSON had no outer-loop marker (medium).
6.  **A remainder laid out before its vector loop** wasn't tagged when the
    vector loop had no alias check (medium/low).
7.  **A `v256` hand loop read as vectorized**, unlike the float4 one (low).
8.  **Wording**: sweep progress now says how many dumps were cached, and
    `status` words "not called yet" as "it compiles; no native code loaded
    yet".

**Round 5 (26 calls, read-only; Kinemation upload jobs, Calligraphics glyph
upload, Mecanim blend shapes, on AVX2 and ARM).** About 111 claims checked
against the raw text, 88 right: every instruction count and range, the shapes
of simple loops, x86 guards and the float-mode diff. Fixed after it:

1.  **False alias checks on ARM** (high): Entities' lookup-cache compare
    (`cmp x11, x10; b.ne`) read as pointer against pointer, on nearly every
    loop of IJobChunk jobs. Alias evidence now needs an ordered condition.
2.  **A missed alias check** (medium-high): a vectorized copy loop (four ymm
    loads and stores, no math) had its check cleared as a non-vector loop.
    Copy loops are now vector work.
3.  **Main pick** (medium): Entities' enabled-mask loops in the producer body
    (tier 0 as the body), and a `lock cmpxchg` retry loop.
4.  **A wrong count** (medium): int32 in, ushort out, read as 64/iter unroll
    2; now 32/iter unroll 4.
5.  **`n ≥ 5` for `n ≥ 3`** (medium): the `lea eax, [rcx + 2]` before a
    folded count test.
6.  **`2/iter` for `i += 2`** on a scalar loop (now `1/iter, counter step 2`);
    `vpinsrd` counted as packed math (now a move, as `ld1 {v0.s}[1]` is on
    ARM); bitwise ops given 32-bit lanes; a strict compare of two int
    bounds on ARM dropped as a guard; address-taken stack slots counted as
    spills (low).

Open from round 5: a quoted alias check names one compare of a two-compare
check; `(count doesn't fill whole vectors)` loops on byte data still need
`--raw`; long call lines with many `from` functions.

Open from round 4: `runs if n ≥` doesn't name which count (without line info
the dump has no variable names; `--source` shows the guard's line); `diff` headers use
Burst's target names (`AVX2`, `X64_SSE4`); a loop-invariant pointer reloaded
per iteration (the `ref NativeArray` case) isn't pointed out.

## Known unsupported

-   **Timing.** Benchmarks belong in tests, with Burst state checked. `status`
    says whether a benchmark here would time Burst code: Burst on, safety
    checks off, background compilation idle, native debug off.
-   **Whether the Editor runs stale native code.** A dump is always current.
    The Editor's JIT cache isn't inspectable, and the reported stale incidents
    couldn't be reproduced (see above), so the tool only notes when an entry's
    code changed under the same hash.
-   **Per-job runtime state.** Jobs have no queryable compile state. `compiled`
    means: it compiles with the Editor's options, so does its whole assembly,
    Burst is on, and background compilation is idle. Static methods also report
    whether their direct-call pointer was set this session.
-   **Loop shape** comes from the induction variable and the bytes each
    stream touches. A counter that only reaches memory through derived
    pointers (`add x13, x0, x10`) can't be scaled, and reads `span unknown`.
    An LLVM-unrolled scalar loop with a stray packed op could read vector.
-   **Guard shapes.** Guard counts and alias checks come from compare
    guards, so the rules assume C# `int` trip counts. A `long` loop counter
    compared with a constant reads as a distance check. Guards that don't
    dominate the loop (multi-exit structures) are ignored.
-   **WASM** gets classes and `loop`/`end_loop` loops only, with no guards or
    unroll.
-   **ARMv7** is classified more coarsely than AArch64.
-   **IR and IL dumps** (`--dump=IR`, `IROptimized`, `IRPassAnalysis`) aren't
    exposed. Their text exists, and could back a later "why didn't this
    vectorize" command from LLVM's remarks.
-   **Player builds.** The tool reads Editor-JIT compiles with explicit
    targets. Per-function AVX dispatch in players (`IsAvxSupported` per
    function) isn't modeled; dump each target instead.
