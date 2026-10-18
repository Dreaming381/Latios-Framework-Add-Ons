using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProfilerWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("profilerwaitress — read Unity Profiler captures (.data, .raw) from the command line");
            w.WriteLine();
            w.WriteLine("Every command takes a capture first: a path, a folder (its newest capture), or part of a file");
            w.WriteLine("name in <project>/ProfilerCaptures/. The first read of a capture extracts it through the Editor");
            w.WriteLine("(a few seconds); later reads are offline and take well under a second.");
            w.WriteLine();
            w.WriteLine("overview");
            w.WriteLine("  info <cap>                       frame time distribution, threads, slowest frames");
            w.WriteLine("  frames <cap> [--sort ms|gpu|gc|frame|<col>] [--asc] [--over MS] [--under MS] [--cols A,B@render]");
            w.WriteLine("                                   per-frame table; columns are counters or markers (name@threads)");
            w.WriteLine("  threads <cap> [--each]           work, wait and utilization per thread (same-named threads summed)");
            w.WriteLine("  counters <cap> [pattern] [--all] Profiler counters (memory, rendering, physics...): avg/min/max");
            w.WriteLine();
            w.WriteLine("where the time goes");
            w.WriteLine("  top <cap> [--sort incl|self|calls|max] [--marker P] [--category C]   markers by ms/frame");
            w.WriteLine("  tree <cap> [frame|slowest|median] [--root P] [--min MS] [--depth N] [--each]");
            w.WriteLine("                                   call hierarchy of one frame, or averaged over --frames");
            w.WriteLine("  marker <cap> <P>                 one marker: distribution, threads, callers, children, worst frames");
            w.WriteLine("  find <cap> <P> [--under P2] [--min MS] [--sort dur|start]   individual samples, longest first");
            w.WriteLine("  timeline <cap> <frame|slowest> [--from MS --to MS] [--meta] [--flows]  samples in time order");
            w.WriteLine();
            w.WriteLine("problems");
            w.WriteLine("  hitches <cap> [--over MS | --factor 1.5] [--lines N]   slow frames and what grew in them");
            w.WriteLine("  waits <cap> [--sort total|idle|count]   waits: idle time, jobs run inline, the job waited on last");
            w.WriteLine("  bubbles <cap> [frame] [--busy 25%] [--min MS] [--sort cost|ms]");
            w.WriteLine("                                   gaps where most workers idle: why (one job, parallel tail, sync,");
            w.WriteLine("                                   late schedule, unstarted, no work) and what each cost the main thread");
            w.WriteLine("  jobs <cap> [--marker P] [--from S] job executions: ms, runs, threads, the system that scheduled them");
            w.WriteLine("  gc <cap> [--stack N]             managed allocations by site; GC.Collect time");
            w.WriteLine("  diff <capA> <capB> [--self] [--frames-a R --frames-b R]   frame times and markers, A vs B");
            w.WriteLine();
            w.WriteLine("captures (need the Editor)");
            w.WriteLine("  record [--frames N] [--play | --editor] [--warmup N] [--callstacks gc,...] [--out x.data]");
            w.WriteLine("                                   record N frames after N warm-up frames (help captures)");
            w.WriteLine("  extract <cap> [--force]          (re)build the cache; other commands do this on demand");
            w.WriteLine("  convert <cap.raw> <out.data>     to the Profiler window's format, for a human");
            w.WriteLine("  cache [list|clear]               cached extracts in UserSettings/Waitress/ProfilerCache/");
            w.WriteLine("  verify <cap> [--frames R]        check the cache against the Profiler's own Hierarchy view");
            w.WriteLine("  selftest                         offline checks of the reader and the numbers");
            w.WriteLine();
            w.WriteLine("shared options");
            w.WriteLine("  --frames R   e.g. 100-200, 512, -50, 300-, skip:30, last:100, first:60 (default: all)");
            w.WriteLine("  --thread T   main, render, workers, background, jobs, all, a glob like 'Job/Worker 1*',");
            w.WriteLine("               comma lists, !excludes (default: main; find/marker/jobs/gc/threads: all)");
            w.WriteLine("  --top N      rows to print (0 = all); --project <dir> picks the Unity project");
            w.WriteLine("  P            a marker: exact name, else every name containing it; globs with * and ?");
            w.WriteLine();
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("numbers", @"How the numbers are computed

  ms/frame    totals divided by the number of frames in scope, so a marker that runs in half the frames
              shows half its per-run cost. `in%` (top) and `in N/M frames` (marker) say how often it ran.
  incl        a sample's whole time. Recursion counts once: a marker nested in itself adds only the
              outermost sample. Summed over the threads in scope.
  self        incl minus the time of direct children.
  frame time  the main thread's frame time as the Profiler reports it (start of one frame to the next).
  gpu         the Profiler's GPU frame time when the capture has one; info falls back to the
              'GPU Frame Time' counter.
  thread root depth-0 samples (""Main Thread"", the unnamed worker roots) are never counted as markers.
  internal    the job system's bookkeeping markers (BeginJob, EndJob, ScheduleJob, KickJobs...) are hidden
              everywhere, as in the Profiler window; their children count toward their parent. --internal
              shows them. top also hides Idle unless --idle.
  counters    values exactly as Unity recorded them. The FrameTimingManager counters (CPU Main Thread Frame
              Time and friends) describe a frame a few frames earlier, so they don't line up with frame time.
  frame       numbers are what the Profiler window's frame counter shows for the capture (one-based),
              so a human can open the capture and go straight to a frame the tool names."),
            ("frames", @"Selecting frames (--frames)

  all          every frame (default)
  512          one frame
  100-200      a range; 300- is 300 to the end; -50 is the start to 50
  first:60     the first 60 frames; last:100 the last 100; skip:30 all but the first 30
  a,b,c        any comma list of the above

  tree, timeline: the second argument names the frame: a number, or slowest, median, fastest (within
  --frames when given). Without one, tree averages over --frames."),
            ("threads", @"Selecting threads (--thread)

  main, render                 the main thread and the render thread
  workers                      the job system's workers (group 'Job')
  background                   background job workers (group 'Background Job')
  jobs                         workers + background
  all                          every thread
  'Job/Worker 1*', Loading/*   a glob over 'Group/Name' or just the name; a plain word matches any
                               thread containing it
  main,render  !background     comma lists; a leading ! removes (alone, it removes from all)

  `threads <cap> --each` lists every thread with its group and busy time."),
            ("captures", @"Getting a capture

  From the Editor: profilerwaitress record --play --frames 300
    enters Play Mode (and leaves it afterwards), skips --warmup frames (default 60 with --play, else 10),
    records exactly --frames frames, saves to <project>/ProfilerCaptures/, and prints `info`. The Editor
    runs the recording itself, so nothing the tool does lands in the capture.
    Without --play it records whatever is already playing; --editor profiles the Editor itself
    (edit mode). The Profiler keeps only its last 'Frame Count' frames (Preferences > Analysis > Profiler,
    up to 2000); record longer runs from a player. --callstacks gc records managed allocation callstacks
    (for `gc`); also malloc, complete, native. --deep needs deep profiling already on (it reloads the domain).
    An unfocused Editor ticks Play Mode only with Pipeline's set_autotick (on by default).

  From a development player build, either run it with
      -profiler-enable -profiler-log-file <path>.raw -profiler-capture-frame-count <N>
  or have the game write one itself:
      Profiler.logFile = path; Profiler.enableBinaryLog = true; Profiler.enabled = true;
      ... N frames later: Profiler.enabled = false; Profiler.logFile = """";
  The .raw file reads directly; it is not limited to the window's frame count.

  .data files saved from the Profiler window read the same way. `convert` turns a .raw into a .data
  for a human to open in the window."),
            ("cache", @"The cache

  The first command on a capture loads it into the Editor's Profiler (Unity's own reader; the formats
  are undocumented), writes every sample, counter, flow event, metadata value and callstack to
  <project>/UserSettings/Waitress/ProfilerCache/<name>-<hash>.pwx, and then answers from that file.
  It is re-extracted when the capture's size or timestamp changes, or with --force. The folder is kept
  under 4 GB (PROFILERWAITRESS_CACHE_MB), least recently used first.

  Extracting replaces what the Profiler window shows. Frames you recorded yourself are saved first and
  loaded back afterwards. A cache from another project can be read directly: pass the .pwx path."),
            ("jobs", @"Jobs, waits and flows

  Unity brackets every job execution with BeginJob/EndJob samples and links the pieces with flow
  events: Begin on the innermost visible sample open when the job was scheduled (a system update,
  ScheduleRenderJobs...), Next/ParallelNext on each execution, End on the wait that completed it
  (WaitForJobGroupID). The schedule time is that sample's matching hidden ScheduleJob child.
  timeline --internal --flows --min 0 shows every flow event.

  jobs    the sample between BeginJob and EndJob is the job; 'scheduled by' follows its flow to the
          Begin sample (or that sample's parent, when it is a bare ScheduleJob).
  waits   main-thread samples named *WaitFor*, Semaphore.Wait*, *.Complete, Wait*, grouped by the
          marker they sit in. 'idle' is the wait minus jobs the main thread ran inline during it.
          'last job' is the job, among those whose flows ended at the wait, that finished last before
          the wait returned: the one the main thread was actually waiting for."),
            ("bubbles", @"Worker bubbles

  bubbles <cap>          every frame: bubble time and cost per cause, the costliest causes first
  bubbles <cap> 412      one frame (or slowest, median): each bubble in time order, start ms from frame
                         start like timeline, so `timeline <cap> 412 --thread main,workers --from S --to E`
                         shows it

  A bubble is a span (at least --min, default 0.02 ms) where at most --busy of the Job workers
  (default 25%; a count like 2 also works) run anything. Each is split into causes by walking back
  from its end: the work that ended it ('next:'), what held that work up, what held that up:
    one job        a single-threaded job (or a chain from one system) ran while the work after it
                   waited; 'the next job started X ms after it ended' is hand-off latency, part of the span
    parallel tail  one run of a parallel job finished long after the others; 'slowest vs median'
    sync           a job wait on the scheduling thread blocked ('last on' = the job it waited for, from
                   flow events, as in `waits`), then the thread ran alone until it scheduled more
    late schedule  the next work was scheduled only during the bubble; 'X ran' before it
    unstarted      work was scheduled but no worker started it for a while: not kicked
                   (JobHandle.ScheduleBatchedJobs) until something waited on it; or 'ran only inline':
                   the scheduling thread waited on it at once and ran it itself before workers woke
    no work        nothing started after it within the frame, or the main thread spent most of it
                   waiting on a frame cap or present (WaitForTargetFPS, Gfx.WaitForPresent*)
    unknown        the capture has no flow events, so the scheduling is unknown
  'X, mostly Y': X covered most of the span and its child Y the largest part of it.
  'main waited in W in S': the main-thread wait that blocked during a gated span; removing or
  delaying that wait is often the fix, rather than the job. '(timing guess)': no flow event named
  the job a sync waited on, so the last job to finish is shown.

  cost   main-thread time blocked on jobs (idle, or running them inline) because of the cause:
         inside the span, plus for sync, late schedule and unstarted the later wait on the work they
         delayed; never more than the span itself, and each wait counts once. Bubbles without cost
         are spare worker capacity: more parallel work fits there for free; nothing needs fixing.
         Cost is main-thread time; it shortens the frame only when the main thread bounds it. The
         verdict says when the main thread also waits on the render thread, GPU or a frame cap.
  busy   workers running anything, averaged over the span (a count, not a percentage).
  n      spans per frame; longest: the longest single span, which 'what' describes, in the frame
         named at the end.

  Workers busy during a wait are not a bubble: a long JobHandle.Complete that runs jobs while the
  workers are full shows in `waits`, not here. Dependencies are not in the capture; a gate is the job
  that ended last, within 0.15 ms, before the work started, so a job that merely finished then can be
  named instead. EngineJob is Unity's native work. Profiler.* jobs belong to the Editor's Profiler and
  are absent from player captures."),
            ("timeline", @"timeline <cap> <frame|slowest|median> [--thread T] [--from MS --to MS] [--min MS] [--depth N]

  Samples of one frame in time order, per thread: start ms from frame start, duration, indented name.
  --from/--to keep samples overlapping the window (with their ancestors, for context) and default
  --depth to 40. --min hides short samples (default 0.05; 0 shows zero-length markers such as
  ScheduleJob). --internal shows markers the Profiler window hides (BeginJob, EndJob, ScheduleJob,
  KickJobs). --meta appends metadata. --flows appends flow events: [begin N] where job N was
  scheduled, [parallel N]/[next N] on its executions, [end N] on the wait that completed it."),
        };

        public static void Topic(TextWriter w, string topic)
        {
            if (topic == null)
            {
                Index(w);
                return;
            }
            if (topic is "record" or "convert")
                topic = "captures";
            var hit = s_Topics.FirstOrDefault(t => string.Equals(t.name, topic, StringComparison.OrdinalIgnoreCase));
            if (hit.name != null)
            {
                w.WriteLine(hit.text.Replace("\r\n", "\n"));
                return;
            }
            // A command name prints the index lines about it.
            using var sw = new StringWriter();
            Index(sw);
            var lines = sw.ToString().Split('\n').Where(l => l.TrimStart().StartsWith(topic + " ", StringComparison.OrdinalIgnoreCase)).ToList();
            if (lines.Count == 0)
                throw new WaitressException($"no help topic '{topic}'; topics: {string.Join(", ", s_Topics.Select(t => t.name))}");
            foreach (var l in lines)
                w.WriteLine(l.TrimEnd());
            w.WriteLine("shared options: help (no topic); details: help numbers|frames|threads|jobs|bubbles");
        }

        public static int Skill(string install, TextWriter output) => Waitress.Skill.Write(SkillText(), install, output);

        static string SkillText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".skill.md", StringComparison.OrdinalIgnoreCase));
            if (resource == null)
                return "(skill.md is missing from this build)\n";
            using var stream = assembly.GetManifestResourceStream(resource);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
