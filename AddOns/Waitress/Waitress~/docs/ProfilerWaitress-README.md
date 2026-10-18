# ProfilerWaitress

ProfilerWaitress is a command-line tool for reading Unity Profiler captures
(`.data` files saved from the Profiler window, and `.raw` binary logs written
by players) and for recording new ones from the Editor. It's built for AI
agents, and it's the third Waitress tool next to ShaderWaitress and VfxWaitress.

Both capture formats are undocumented binary streams that change between Unity
versions, and the only thing that opens them is the Profiler window, which an
agent can't look at. A busy 900-frame capture holds 8 million samples on 90
threads. ProfilerWaitress has the Editor extract a capture once, then answers
questions like "why is this frame slow", "what is the main thread waiting on",
"why are the workers idle" or "did my change help" offline, in one call and a
few dozen lines.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

All you need is the .NET SDK. There are no NuGet packages. The binary is
`UserSettings/Waitress/build/bin/ProfilerWaitress/release/profilerwaitress[.exe]`,
and the same build makes the other two tools.

ProfilerWaitress needs no package sample. The Profiler API it uses is public,
so the tool carries its Editor script inside itself, writes it to
`UserSettings/Waitress/ProfilerWaitress/`, and runs it through the Pipeline
package's `run_script` command (Pipeline 0.7 or newer). Nothing gets imported
into the project, and the Editor code always matches the tool.

## Start Here

```
profilerwaitress info <cap>               # frame times, what bounds the frame, slowest frames
profilerwaitress hitches <cap>            # each slow frame and what grew in it
profilerwaitress top <cap> --sort self    # where the main thread's time goes
profilerwaitress bubbles <cap>            # why the workers sit idle, and whether it matters
profilerwaitress diff <before> <after>    # did a change help
profilerwaitress help                     # command index
profilerwaitress skill                    # the whole tool on one page, for an agent
```

`<cap>` is a path, a folder (its newest capture), or part of a file name in
`<project>/ProfilerCaptures/`. Frame numbers are the ones the Profiler window's
frame counter shows, so a person can jump straight to a frame the tool names.
`profilerwaitress help <topic>` covers `numbers`, `frames`, `threads`,
`captures`, `cache`, `jobs`, and `bubbles`.

## What It Looks Like

```
$ profilerwaitress info 2026-10-08_Level3_5000_final_dev.raw          # 353 MB player capture
frames   901 (1-901), 4.83 s, 186.6 fps
main ms  avg 5.360  p50 5.842  p95 6.977  p99 7.542  max 8.390  (max at frame 290)
gpu ms   avg 3.248  p50 3.632  p95 4.446  p99 4.758  max 5.422  (858 frames with GPU time)
bound    render thread (heuristic; ms/frame: main waits 0.715 on render/present, 0.631 idle on jobs,
         render waits 1.460 on main, gpu 3.248, workers 35% busy)
threads  90: main, render, 31 workers, 16 background, 41 other
slowest  290 (8.390), 542 (8.101), 879 (7.860), 113 (7.631), 887 (7.628), 180 (7.620)
```

The first command on a capture takes a few seconds while the Editor extracts it.
After that, commands answer in 0.2 to 2.5 seconds with no Editor involved.

## Worker Bubbles

In the Profiler window's timeline, a human can see thin vertical gaps where
most of the worker threads sit idle at once. These bubbles are often the
biggest latency cost in a DOTS frame, but they're invisible in any table, so an
agent has no way to find them. `bubbles` finds every one and walks back from
its end to name what held up the work after it:

-   **one job**: a single-threaded job, or a chain of them, ran while
    everything else waited
-   **parallel tail**: one run of a parallel job took far longer than the others
-   **sync**: a job wait drained the workers, then the thread ran alone
-   **late schedule**: the next jobs were scheduled only after some serial work
-   **unstarted**: jobs sat scheduled but were never kicked
-   **no work**: nothing else was scheduled

Each cause also gets a **cost**: the main-thread time spent waiting because of
it. That's the difference between a bubble that adds latency and one that's
just spare capacity, where the right move is to stop worrying and fill it with
more features.

```
$ profilerwaitress bubbles 2026-10-08_Level3_5000_final_dev.raw --frames skip:30 --top 5
workers busy 35% of the frame; bubbles 3.037 ms/frame (57% of the frame), 21.5 bubbles/frame; cost 0.605 ms/frame
ms     cost   n     longest  cause          what
0.066  0.064  0.50    0.245  sync           WaitForJobGroupID in Shadows.ExecuteDrawShadows, then ... ran alone; the wait blocked ...
0.157  0.052  0.97    0.359  late schedule  Default World Overrun.Systems.BotSystem ran before EngineJob (from ...ZombieLayerSystem) was scheduled
0.046  0.046  0.99    0.101  one job        BuildCollisionLayerInternal:Part2Job (Burst) (from ...ZombieLayerSystem) ran 0.012 ms alone; ...
0.058  0.043  0.53    0.515  one job        EngineJob (from Inl_ScriptableRenderContext.Submit) ran 0.510 ms alone; next: EngineJob
0.035  0.035  0.98    0.214  parallel tail  BuildCollisionLayerInternal:Part4Job (Burst) (from ...): 29 runs, slowest 0.202 ms, median 0.004; ...
verdict: bubbles cost the main thread 0.605 ms/frame (11% of the frame), mostly sync; 1.805 ms/frame of bubbles cost nothing (spare capacity, ignore those)
  but the main thread also waits 0.709 ms/frame on the render thread, GPU or a frame cap (Gfx.WaitForPresent*, WaitForTargetFPS):
  saving main-thread time turns largely into more of that waiting, not a shorter frame (see info's bound line)
```

`bubbles <cap> <frame>` lists one frame's bubbles in time order, with start
times that `timeline` accepts.

## Recording

```
profilerwaitress record --play --frames 300                 # enters Play Mode, records, leaves, prints info
profilerwaitress record --play --frames 300 --callstacks gc # with managed allocation call sites
profilerwaitress record --editor --frames 60                # the Editor itself
```

The Editor runs the recording from its own update loop and writes a status
file, and the tool only watches that file. An earlier design polled the Editor
instead, and every poll compiled C# inside the capture. The Profiler window
keeps only its last *Frame Count* frames (up to 2000), so longer and more
realistic runs should come from a development player with
`-profiler-enable -profiler-log-file <path>.raw`. `convert` turns a `.raw` into
a `.data` for a person to open in the window.

## The Cache

Extraction loads the capture into the Editor's own Profiler, which is Unity's
reader, so nothing parses the binary format by guesswork. Every sample,
counter, flow event, metadata value, and callstack goes into a columnar,
memory-mapped file in `UserSettings/Waitress/ProfilerCache/`. The cache for a
353 MB capture is about 150 MB. It's rebuilt when the capture changes, and the
folder is kept under 4 GB, least recently used first. If the Profiler window
holds frames the user recorded, they're saved first and loaded back afterwards.

## Verifying Changes

-   `profilerwaitress verify <cap>` walks Unity's own Hierarchy view for a few
    frames on every thread and compares every path's total, self, calls, and GC
    bytes against the cache. It matched exactly (within 5 µs of float rounding)
    on five captures: a player capture, Editor captures with and without Play
    Mode or GC callstacks, one that ends in a scene load, and one built to
    exercise every marker verbosity.
-   `profilerwaitress selftest` checks the reader, the semantics (recursion,
    self time, hidden markers, selectors) and bubble classification on
    synthetic captures. The tests are mutation-checked.
-   `Assets/Validation/ProfilerWaitress/BubbleScenario.unity` produces one
    bubble of each cause per frame, each under its own marker, as ground truth
    for `bubbles`. Record it with `record --play --frames 120` and run
    `bubbles` on the result.

[ProfilerWaitress-DESIGN.md](ProfilerWaitress-DESIGN.md) covers the cache
format, the exact semantics of every number, the Unity behaviors that were
measured to get them right, and what isn't supported.
