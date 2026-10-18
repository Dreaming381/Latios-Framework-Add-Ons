# ProfilerWaitress Design

## The Problem

A Profiler capture is either a `.data` file saved from the Profiler window or a
`.raw` binary log written by a player. Both are undocumented binary streams
whose layout changes between Unity versions (the header begins with a date-like
format stamp). A 900-frame capture of a busy DOTS game is 350 MB holding about
8 million samples on 90 threads. The only reader is the Editor's Profiler, and
an agent can't look at the Profiler window.

The Editor does expose that reader: `ProfilerDriver.LoadProfile` plus
`RawFrameDataView`, which hands out every sample with its marker, start time,
duration, depth, metadata, flow events and callstack. It's fast: the capture
above loads in about 0.8 s and walks in about 0.5 s. The bottleneck for an
agent isn't speed. It's that every question costs a round trip through the
Editor and returns far more than it needs.

## Architecture

```
profilerwaitress <command> <capture>
   |
   |-- cache fresh? ---- yes --> memory-map UserSettings/Waitress/ProfilerCache/<name>-<hash>.pwx --> answer
   |
   `-- no --> unity command run_script --file UserSettings/Waitress/ProfilerWaitress/ProfilerWaitressEditor.cs
                 --entry ProfilerWaitressEditor.Extract  (LoadProfile, walk every frame and thread, write .pwx)
```

**Extract once, query offline.** The Editor half dumps the whole capture into a
columnar cache in one pass (about 3 s for the capture above). Every command
after that is a memory-mapped read and an in-process computation: 0.15 to 1 s
cold, with no Editor involved. Two captures can be compared, the Editor can be
busy or closed, and nothing round-trips per question.

**No package sample.** ShaderWaitress and VfxWaitress need samples because they
reach into internal Shader Graph and VFX Graph types through `.asmref`s. The
Profiler API is public, so ProfilerWaitress embeds its Editor script as a
resource, writes it to `UserSettings/Waitress/ProfilerWaitress/` (outside
`Assets/`, so no import or domain reload), and runs it through Pipeline's
`run_script` (Pipeline 0.7+). Nothing gets imported into the project, nothing
needs re-syncing by hand, and the Editor code always matches the binary. The
add-on stays inert until a tool runs.

**Unity's reader, not ours.** Parsing the capture formats directly would mean
reverse-engineering a stream that Unity changes between versions, with no
oracle except Unity itself. Using `RawFrameDataView` makes the cache exact by
construction, and `verify` proves it (see below).

## The Cache Format

One file, little-endian. It starts with `"PWX1"` and a table of
`(fourcc, offset, length)` sections. Sample data is stored in columns so a pass
touches only what it reads.

| Section | Contents |
|---|---|
| `HEAD` | format version, source path, size, timestamp (the freshness check), extracting Unity version, first frame index, the Profiler's frame-count preference |
| `STRS` | string pool; everything else refers to strings by index |
| `MRKR` | per marker: name, category, `MarkerFlags` |
| `THRD` | per thread: id, group, name. Keyed by id + group + name, because thread indices are not stable across frames |
| `FRAM` | per frame: start ns, main-thread frame time, GPU time, fps, first block, block count |
| `BLCK` | per (frame, thread): first sample, count, base ns, time shift |
| `SMRK` `SDEP` `SSTA` `SDUR` | per sample, in depth-first order: marker (int32), depth (uint16), start and duration (uint32, relative to the block base, shifted right by the block's shift) |
| `META` | typed metadata values (integers, floats, strings, InstanceId resolved to "name (type)"); the job system's binary blobs are skipped |
| `FLOW` | flow events: sample, flow id, type |
| `CNTR` `CVAL` | counters (marker, value type, unit) and one double per counter per frame |
| `STAK` `SSTK` | interned callstacks (resolved method names, native frames empty) and sample-to-callstack refs |

A block's shift is 0 unless the block spans more than 4.29 s, so uint32 holds
nanoseconds exactly in practice and still fits a multi-second loading hitch.
The cache for the 350 MB capture is 150 MB. The format version is bumped
whenever the extracted meaning changes, and a cache with another version is
re-extracted.

The cache folder is kept under 4 GB (`PROFILERWAITRESS_CACHE_MB`), least
recently used first.

## Semantics

These match the Profiler window wherever the window has an answer.

- **Frame numbers** are what the window's frame counter shows (one-based), so a
  human can go straight to a frame the tool names.
- **Hidden markers.** A sample whose marker has the 0x800 bit is hidden along
  with its whole subtree, but its time still comes out of its parent's self time.
  This covers `VerbosityInternal` (BeginJob, EndJob, ScheduleJob, KickJobs...) and
  `VerbosityExternal`, which also carries the bit. `VerbosityDebug` and
  `VerbosityAdvanced` are shown. This was measured: an Editor update hook emitted
  markers of every verbosity, each with a child sample, and Unity's
  `HierarchyFrameDataView` showed exactly this. An earlier guess, that hidden
  samples hand their children to the parent, was wrong, and `verify` caught it on
  a scene-loading frame. `--internal` shows them.
- **Inclusive time** counts a marker once per outermost occurrence, so recursion
  isn't double counted. **Self time** is a sample's duration minus its direct
  children's.
- **Thread roots** (depth 0) are never counted as markers.
- **Per frame** means divided by the number of frames in scope, including frames
  where the marker didn't run.
- **GPU time per frame** is the frame's own GPU time when the capture has it.
  Player captures leave it empty, so it comes from the `GPU Frame Time` counter.
  That counter comes from FrameTimingManager and reports a frame several frames
  back (4 in every capture measured). The same record's `CPU Total Frame Time`
  equals the real frame time once shifted, so the shift is measured per capture
  and applied. `info` says when this happened. Other counters are shown exactly as
  Unity recorded them.
- **The `gc` column of `frames`** sums `GC.Alloc` sample sizes on the threads in
  scope. It falls back to the process-wide `GC Allocated In Frame` counter, and
  says so, only when the allocation samples carry no sizes.
- **`threads`** separates work from waiting and clips samples that span several
  frames to the frame they appear in.

### Jobs, Waits and Flow Events

Unity links a job's life with flow events: `Begin` on the sample that scheduled
it, `Next`/`ParallelNext` on each execution, `End` on the wait that completed
it. Measured on a real capture, **flow ids are recycled about every 65,536
flows**: 235,832 Begin events used only 65,574 distinct ids. So events are
matched to the nearest one in time (within 250 ms), never by id alone. Matching
by id put AnimateJob under `RenderLoop.ScheduleDraw` before this was found.

- A **job execution** is the sample between `BeginJob` and `EndJob` at the same
  depth.
- **scheduled by** is the latest `Begin` of the execution's flow before it
  started. A bare `ScheduleJob`/`KickJobs` sample is replaced by its parent,
  usually the system.
- A **wait** is a sample named `*WaitFor*`, `Semaphore.Wait*`, `*.Complete` or
  `Wait*`. A wait nested in a wait belongs to the outer one. **idle** is the wait
  minus the job executions run inline inside it.
- **last job** is the execution, among those whose flows end anywhere in the
  wait's subtree, that finished last before the wait returned. **inline** lists
  the jobs the waiting thread ran itself during the wait, with the systems that
  scheduled them; that is usually the chain it waited on. Some
  `JobHandle.Complete` waits carry no End flow and run nothing inline. For
  those, the job that finished within 50 µs of the wait returning is reported,
  marked `(timing guess)`. Waits on the render thread or GPU get a one-line
  explanation instead of a job.

**Where a Begin sits.** Measured on a capture made for it
(`Assets/Validation/ProfilerWaitress/BubbleScenario.cs`): a Begin flow sits on
the innermost *visible* sample open when the job was scheduled, often a whole
system update or a user marker, not on the zero-length `ScheduleJob` sample.
That sample's start can be milliseconds before the job was scheduled. Its
hidden `ScheduleJob` children (those whose nearest visible ancestor is that
sample) come in the same order as its Begins. So when the counts agree, the
k-th Begin's time is the k-th `ScheduleJob`. When they don't, the latest
`ScheduleJob` before the job started is used. Without this, every job scheduled
after serial work in the same marker looked scheduled early and unstarted.
`timeline --internal --flows --min 0` prints the raw events.

### Worker Bubbles

The Profiler window's timeline makes one thing obvious to a human that no
table shows: thin vertical gaps where most workers sit idle at once. These are
often the biggest latency cost in a DOTS frame. `bubbles` finds them and names
their causes.

A **bubble** is a span of at least `--min` (0.02 ms) where at most `--busy`
(25%) of the `Job` workers run anything. "Anything" is any depth-1 sample
except `Idle`, BeginJob/EndJob and waits. Each worker's spans are merged first
so nested samples count once. Bubbles are cut at the main thread's frame
boundaries.

A bubble ends when work starts again. The **release** is the job whose
executions started together in the largest number within 50 µs of the end.
Working back from the release, each step assigns the latest part of the bubble
to one cause and hands the rest on:

1. **A dependency still running.** An execution (any thread) that overlaps the
   bubble, ended within 0.15 ms before the release started, ended after the
   release was scheduled, and isn't part of the release itself. The latest
   such one is the **gate**; zero-length `(Cleanup)` executions only stand in
   when no real one qualifies. It gets the span from its own start: `one job`
   if its schedule produced one execution, else `parallel tail`. Then the gate
   becomes the release for what came before it. Consecutive `one job` steps
   merge into a chain.
2. **Scheduled long before it started** (more than 0.15 ms) with no gate:
   `unstarted`, from the schedule time on. In practice that is a job that was
   never kicked: Unity batches scheduled jobs until
   `JobHandle.ScheduleBatchedJobs()` or a wait flushes them. The scenario
   reproduces this exactly. When the scheduling thread itself ran the job
   inline during the span (it waited on the job right after scheduling it,
   before any worker woke), it says `ran only inline` instead.
3. **Scheduled during the bubble.** If a job wait on the scheduling thread
   ended in the bubble (or within 50 µs before it) after actually blocking for
   at least `--min`, it is `sync` from the wait's start. That covers both the
   classic sync point, where the wait drained the workers and the thread then
   ran alone, and a thread blocked on a few busy workers before scheduling the
   next work. The part before the wait, if any, is `late schedule`.
   Otherwise the whole rest is `late schedule`. Both name what the scheduling
   thread mostly ran: the deepest visible sample covering more than half of
   the span (or the job it runs inside, when the thread ran a job inline),
   plus its child covering the largest part when that is at least a quarter.
   A sync names the job its wait waited on last from the End flows in the
   wait, as `waits` does; timing is the fallback.
4. **No release** within the frame: `no work`. A `late schedule`, `unknown`
   or `no work` span that the main thread spent mostly waiting on a frame cap or
   present (WaitForTargetFPS, Gfx.WaitForPresent*, WaitForLastPresentation,
   DXGI.WaitOnSwapChain) is `no work` too: it is not a scheduling gap. Without
   this rule, a 120 fps-capped capture showed 3.8 ms/frame of `late schedule`.

`one job` and `parallel tail` rows also name the main-thread wait that blocked
during them. A capture where `WeaponSystem` completes a collision-layer build
right after scheduling it otherwise showed only Psyshock's internal jobs.
A sync wait counts as blocking when it lasted 20 µs, whatever `--min` is.

Slivers under `--min` join the neighbouring cause they fed into.

**Cost** answers "does this bubble matter?" It is main-thread time spent in job
waits inside the span, whether idle or running jobs inline. For `sync`,
`late schedule` and `unstarted`, it also includes the main thread's later wait
on the work the bubble delayed (from the bubble's end to the end of its
release). The total is capped at the span's length, because a delay of d can
cost at most d. Every wait nanosecond is charged at most once per frame, so
costs add up. Cost is main-thread time, and it shortens the frame only when the
main thread bounds it. When the main thread also waits on the render thread,
the GPU or a frame cap for at least half that long, the verdict says so. A bubble
with no cost is spare worker capacity. That is the "not enough work" case, and
the verdict line says how much of the bubble time it is.

Dependencies between jobs are in the job system's binary metadata, which has no
public layout. So a gate is inferred from timing. A job that merely happened to
finish within 0.15 ms of the release can be named instead of the real
dependency.

Validated on `BubbleScenario`, which produces one bubble of each cause per
frame under its own marker (single job gating a parallel job, an uneven
parallel job, a sync followed by serial work, serial work before a schedule,
an unkicked job). In a 120-frame Play Mode capture of it, the one job, parallel
tail and sync phases got their intended cause in all 120 frames. The late
schedule and unstarted phases got theirs in 119; in the odd frame the main
thread was running a job inline, or other work kept the workers busy. The costs
match the main-thread waits the scenario builds in (about 0.73 ms for the
single job, 1.8 ms for the uneven job). `selftest` checks the classification and
costs on a synthetic frame. Mutation-checked: without the ScheduleJob timing,
the late schedule turns into `unstarted` and the test fails.

On the Overrun player capture (901 frames, 31 workers), bubbles take 3.1 ms of
a 5.4 ms frame but cost the main thread 0.61 ms. Most of the bubble time is
spare capacity, and the frame is render-thread bound (the main thread waits
0.71 ms on present), which the verdict points out. The costly causes it names include Psyshock's
`BuildCollisionLayer` (a single-threaded `Part2Job`, and a `Part4Job` whose
slowest run is 0.2 ms against a 0.004 ms median), shadow-drawing waits on the
main thread (on an EngineJob from `Shadows.PrepareDrawShadows`), and the `AddCleanupForRootsOfSurvivingChildExpirablesSystem` sync.

### Hitches

A frame is a hitch when it's over `--over` ms or `--factor` times the median
(1.5). For each marker, the self-time median is taken over all frames in scope,
counting absent frames as zero. A hitch lists the markers whose self time grew
the most, each with the path of its longest sample in that frame. Self time
(not inclusive) is used because inclusive growth always names `PlayerLoop`. A
context line puts GPU time and render-thread work beside the frame time. Wait
markers are labelled `(waiting)`, since a frame that only waited longer was
bound elsewhere.

## Recording

`record` starts the Profiler through the Editor script, and the recording then
runs inside the Editor. An `EditorApplication.update` callback skips `--warmup`
frames, clears them, stops after exactly `--frames` frames, saves, and writes
`<capture>.status`. While it records, the CLI only polls the file system. The
first version polled the Editor with `run_script`, and each poll compiled C#
inside the capture: 70 ms frames and 4 MB of Roslyn allocations every few
hundred frames, which then topped `max`, `gc` and `hitches`. The callback lives
in a `run_script` assembly, so the state it shares with later calls goes through
`SessionState`. A newer recording, or `RecordAbort`, retires an older callback.

## Verification

- `verify <cap>` loads the capture in the Editor and walks Unity's own
  `HierarchyFrameDataView` for a few frames (first, median, slowest, last) on
  every thread. It compares every path's total time, self time, calls and GC
  bytes against what the cache gives. Unity lists two distinct markers with the
  same name as separate items, so items are summed by name path on both sides.
  Measured exact (time differences under 5 µs, from float rounding in Unity's
  columns) on the Overrun player capture, an Editor capture with GC callstacks,
  an Editor Play Mode capture, a 983-frame capture that ends in a scene load, and
  a capture made to exercise every marker verbosity. Unity's GC column credits
  visible ancestors with allocations made inside hidden subtrees, and `verify`
  does the same.
- `selftest` writes synthetic captures in the cache format and checks recursion,
  self time, hidden-marker subtrees and verbosity flags, frame and thread selectors,
  shifted blocks and marker patterns. Mutation-checked: disabling the recursion
  rule or the internal hiding each fails a test.

## Known Unsupported

- **GPU sample timings.** Only the frame's GPU time and the `GPU Frame Time`
  counter are read; the GPU module's per-pass hierarchy isn't extracted.
- **Attaching to a running player.** `record` profiles the Editor's Play Mode or
  the Editor itself. Players are captured with `-profiler-log-file`.
- **The job system's binary metadata** (job ids, dependencies) has no public
  layout and is skipped; flow events carry the relationships instead.
- **Module-specific data** such as UI event markers, audio DSP graphs and file
  I/O module tables isn't extracted. Their counters are.
- **Deep profiling** can be recorded only when it's already on, because
  switching it reloads the domain.
- **Memory Profiler snapshots** are a different format and a different tool.
