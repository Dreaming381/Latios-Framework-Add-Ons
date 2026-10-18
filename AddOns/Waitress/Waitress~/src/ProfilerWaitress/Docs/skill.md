---
name: profilerwaitress
description: Read Unity Profiler captures (.data from the Profiler window, .raw binary logs from players) from the command line, and record new ones from the Editor. Use whenever a task involves frame times, hitches, what a frame spends its time on, job/worker utilization, worker idle gaps (bubbles) and what causes them, main-thread waits, GC allocations, or comparing performance before and after a change, because the capture formats are undocumented binaries that only the Profiler window can open.
---

# ProfilerWaitress

A CLI over Unity Profiler captures. The first command on a capture extracts it
through the running Editor (a few seconds, once); every later command reads
the extracted cache offline in well under a second. Output is dense text: times
are milliseconds, per frame, averaged over the frames in scope.

## First commands

```
profilerwaitress info <cap>                  # frame times, what bounds the frame, slowest frames
profilerwaitress hitches <cap>               # each slow frame and what grew in it
profilerwaitress top <cap> [--thread workers] [--sort self]
profilerwaitress tree <cap> slowest          # call hierarchy of one frame
profilerwaitress bubbles <cap>               # worker idle gaps: their causes and what they cost
profilerwaitress help                        # every command and option
```

`<cap>` is a path, a folder (its newest capture), or part of a file name in
`<project>/ProfilerCaptures/`. Run from inside the Unity project.

## Recording a capture

```
profilerwaitress record --play --frames 300            # enters Play Mode, records, leaves it, prints info
profilerwaitress record --play --frames 300 --callstacks gc   # also managed allocation callstacks
profilerwaitress record --editor --frames 60           # the Editor itself, in edit mode
```

The Editor runs the recording itself, so the tool adds nothing to the capture.
`--warmup N` frames are skipped first (60 with `--play`: entering Play Mode JITs
and loads). Recording stops at exactly `--frames`.

The Profiler keeps only its last *Frame Count* frames (Preferences > Analysis >
Profiler, default 300, max 2000). For longer or more realistic runs, profile a
development player: `-profiler-enable -profiler-log-file <path>.raw
-profiler-capture-frame-count <N>`. A `.raw` reads the same way. See
`help captures`.

## Answering the usual questions

| Question | Command |
|---|---|
| How fast is it, how stable? What bounds it? | `info` (its `bound` line), `frames --sort ms` |
| Why is frame N slow? | `hitches`, `tree <cap> N`, `timeline <cap> N` |
| What costs the most on the main thread / workers? | `top`, `top --thread workers`, `top --sort self` |
| Where is a marker called from, what does it call? | `marker <cap> <name>` |
| Which instances of X were longest? | `find <cap> X [--under Y]` |
| Is the main thread waiting on jobs? Which ones? | `waits` |
| Why are the workers idle in gaps? Does it matter? | `bubbles` (then `bubbles <cap> <frame>`) |
| Which jobs run, who scheduled them? | `jobs` |
| Are the workers busy? | `threads` |
| Who allocates managed memory? | `gc` (needs `--callstacks gc` for call sites) |
| Did my change help? | `diff <before> <after>` |
| Memory, draw calls, other counters? | `counters <cap> [pattern]`, `frames --cols "Batches Count"` |
| A marker or counter per frame? | `frames --sort frame --top 0 --cols "Name,Other@render"` |

## Scope options (every analysis command)

- `--frames 100-200` (also `512`, `300-`, `-50`, `skip:30`, `last:100`,
  `first:60`, comma lists). Frame numbers are the ones the Profiler window's
  frame counter shows. Warm-up frames are usually slow: `--frames skip:30`.
- `--thread main|render|workers|background|jobs|all`, a glob such as
  `'Job/Worker 1*'`, comma lists, `!` to exclude. Most commands default to
  `main`. `find`, `marker`, `jobs`, `gc` and `threads` default to `all`.
- `--top N` rows (0 = all), `--min MS` to hide small samples (`tree`,
  `timeline`, `find`, `top`).
- Marker patterns: an exact name, else every name containing the text; `*` and
  `?` are globs. `marker` lists candidates when a pattern is ambiguous.

## Reading the output

```
incl   self   calls  max    @frame  in%   marker
1.433  0.482     21  2.417     752  100%  WaitForJobGroupID
```

`incl` counts a marker once even when it recurses. `self` is incl minus direct
children. `calls` and both times are per frame, averaged over the frames in
scope. `max` is the worst single frame and `@frame` says which. `in%` is how
many frames the marker ran in.

`tree` prints `incl self calls name` indented by depth. A wrapper whose only
child does all its work prints on one line as `A > B`. `... N under --min` and
`... N children below --depth` say what was left out.

`hitches` lines read `+2.234  Semaphore.WaitForSignal (waiting)  in <path>`:
how much a marker's self time grew over its median, what it is, and where its
longest sample sat. When the top line is a wait, the frame was bound elsewhere.
The context line (GPU time and render-thread work against their medians) says
where.

`info`'s `bound` line is a heuristic verdict (main thread, waiting on jobs,
render thread, GPU, or a frame-rate cap) printed with the numbers it is based on.

`waits` splits each wait into idle time and jobs the main thread ran inline
while waiting. The `inline` lines name those jobs and the systems that scheduled them. `last job` is the job that finished last before the wait returned: the one
the main thread actually waited for. It follows Unity's job flow events.
`(timing guess)` marks a guess from timing alone, for waits with neither flow
events nor inline jobs.

Like the Profiler window, markers whose flags say internal or external (the
job system's BeginJob, EndJob, ScheduleJob, KickJobs, some loading markers) are
hidden with their subtrees. That is a marker flag; Unity's "Internal" category,
shown in `marker` headers, is unrelated and hides nothing; `--internal` shows them. `top` also hides `Idle`
unless you pass `--idle`.

`threads` separates work from waiting. Player captures carry GPU time only in
a counter that reports several frames late; the tool measures the lag and moves
GPU time back to the frame it describes (`info` says so).

## Worker bubbles

```
profilerwaitress bubbles <cap>               # all frames: cost and time per cause, costliest first
profilerwaitress bubbles <cap> slowest       # one frame: each bubble in time order
```

A bubble is a gap where most Job workers sit idle (at most `--busy`, default
25%, run anything) for at least `--min` (0.02 ms). Each is split into causes:

- `one job`: a single-threaded job (or a chain) held up the work after it. Make
  it parallel, split it, or schedule independent work beside it.
- `parallel tail`: one run of a parallel job took far longer than the rest
  (`slowest` vs `median`). Use smaller batches or split the heavy items.
- `sync`: a job wait blocked the thread (`last on` names the job it waited
  for), then the thread ran alone (`then X`). Remove the sync point (structural change, main-thread access) or move it.
- `late schedule`: X ran before the next jobs were scheduled. Schedule earlier
  or move X into jobs.
- `unstarted`: jobs sat scheduled but not running on workers. Call
  `JobHandle.ScheduleBatchedJobs()` after scheduling. `ran only inline` means
  they were waited on right after scheduling: schedule them earlier.
- `no work`: nothing was scheduled, or the main thread was waiting on a frame
  cap or present.

On `one job` and `parallel tail` rows, `main waited in W in S` names the wait
that blocked the main thread. Often the fix is to remove or delay that wait
(complete later, schedule the job earlier) rather than to change the job.

`cost` is main-thread time blocked on jobs (idle, or running them inline)
because of the cause, never more than the span itself. That is what a bubble
costs in latency. **A cause with no cost is spare capacity: do not fix it; more
parallel work fits there for free.** The verdict line says how much bubble time
is free, and warns when the main thread also waits on the render thread, GPU or
a frame cap: then saving main-thread time mostly becomes more of that waiting,
not a shorter frame. Start at the costliest row, then open its frame (named at
the end of the row) with `bubbles <cap> N` and `timeline --thread main,workers
--from S --to E`. A long `JobHandle.Complete` during which the workers are busy
is not a bubble; `waits` shows those. `help bubbles` explains every column.

## Comparing

```
profilerwaitress diff before.data after.data [--thread workers] [--self] [--frames skip:30]
```

Frame time distributions side by side, the counters that changed most, then
markers by largest change. Markers
are matched by name, so captures from different builds compare fine. Use
`--frames-a`/`--frames-b` when the runs need different windows. When the average and the median
moved differently, `diff` says so: the captures likely hold different
workloads (a camera that looks away, a quiet phase), so compare matching
windows. Find phases with `frames --sort frame --top 0` or `frames --under MS`.

## Trust

`verify <cap>` compares the cache against the Profiler's own Hierarchy view
(every path's total, self, calls and GC bytes, on every thread, for a few
frames) and prints OK or the differences. Run it if numbers look wrong or
after a Unity upgrade.

## Gotchas

- Extraction loads the capture into the Editor's Profiler. Frames the user
  recorded themselves are saved first and loaded back afterwards.
- A capture recorded in the Editor includes Editor overhead (`EditorLoop`,
  inspector repaints). Player captures are the ones to trust for shipping
  numbers.
- `jobs`/`waits` see job names Unity gave; `EngineJob` is Unity's own native work.
- Times in a `.data` saved from the window cover only the frames the window
  kept. `info` says so when the frame count equals the preference.
