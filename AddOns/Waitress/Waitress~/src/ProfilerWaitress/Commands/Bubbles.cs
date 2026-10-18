using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ProfilerWaitress.Analysis;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;
using ProfilerWaitress.Text;

namespace ProfilerWaitress.Commands
{
    /// <summary>
    /// Worker bubbles: spans where most Job workers sit idle, what caused each, and what it cost the main thread.
    /// </summary>
    public static class Bubbles
    {
        static readonly string[] s_CauseNames = { "one job", "parallel tail", "sync", "late schedule", "unstarted", "no work", "unknown" };

        static readonly string[] s_Advice =
        {
            "one job: a single-threaded job held up the work after it; make it parallel, split it, or schedule independent work beside it",
            "parallel tail: one run of a parallel job finished long after the rest; use smaller batches or split the expensive items",
            "sync: a job wait blocked the thread, then it ran alone; remove the sync (structural change, main-thread access) or move it later",
            "late schedule: the thread ran serial work before scheduling the next jobs; schedule earlier or move that work into jobs",
            "unstarted: jobs sat scheduled but not running on workers; call JobHandle.ScheduleBatchedJobs() after scheduling (when they ran inline, they were waited on right after scheduling: schedule earlier)",
            "no work: nothing was scheduled; spare capacity, not latency",
            "unknown: the capture has no flow events linking jobs to their scheduling",
        };

        public static int Run(CaptureFile c, Args args, TextWriter w)
        {
            if (args.Has("thread") || args.Has("t"))
                throw new WaitressException("bubbles always looks at the Job workers (and the threads that schedule for them); --thread does not apply");
            var scope = Trees.FrameScope(c, args, "all", false);
            var busyText = args.Get("busy", "25%");
            var minMs = args.GetDouble("min", 0.02);
            var top = Scope.Top(args, 12);
            var sort = args.Get("sort", "cost");
            args.RejectUnknown();
            if (sort != "cost" && sort != "ms")
                throw new WaitressException("--sort takes cost or ms");
            if (minMs < 0)
                throw new WaitressException("--min can't be negative");
            var fraction = ParseBusy(busyText, c);
            if (!c.Threads.Any(t => t.Kind == ThreadKind.Worker))
                throw new WaitressException("the capture has no Job worker threads");
            var finder = new BubbleFinder(c, fraction, (long)(minMs * 1e6));

            var all = new List<Bubble>();
            long busyNs = 0, frameNs = 0, elsewhere = 0;
            int workers = 0, threshold = 0;
            foreach (var f in scope.Frames)
            {
                all.AddRange(finder.Frame(f, out var n, out var busy, out var thr));
                busyNs += busy;
                elsewhere += finder.ElsewhereNs;
                frameNs += (long)c.Frames[f].TimeNs * n;
                workers = Math.Max(workers, n);
                threshold = Math.Max(threshold, thr);
            }
            var frames = (double)scope.Frames.Length;
            var wall = all.Sum(b => b.End - b.Start);
            var cost = all.Sum(b => b.Cost);
            var frameTime = scope.Frames.Sum(f => (double)c.Frames[f].TimeNs);
            var single = scope.Frames.Length == 1;
            var per = single ? "" : "/frame";
            w.WriteLine($"# worker bubbles in {(single ? $"frame {c.FrameNumber(scope.Frames[0])} ({Format.Ms(c.Frames[scope.Frames[0]].TimeNs)} ms)" : scope.Describe(c).Split(',')[0])}: " +
                        $"spans of at least {minMs} ms where at most {threshold} of {workers} Job workers ran anything{(single ? "" : "; ms and counts per frame")}");
            w.WriteLine($"workers busy {Format.Pct(frameNs > 0 ? (double)busyNs / frameNs : 0)} of the frame; bubbles {Format.Ms(wall / frames)} ms{per} ({Format.Pct(wall / frameTime)} of the frame), " +
                        $"{Format.PerFrame(all.Select(b => (b.Frame, b.Id)).Distinct().Count() / frames)} bubbles{per}; cost {Format.Ms(cost / frames)} ms{per}");
            var causes = all.GroupBy(b => b.Cause).OrderByDescending(g => g.Sum(b => b.Cost)).ThenByDescending(g => g.Sum(b => b.End - b.Start)).ToList();
            w.WriteLine("by cause (ms, cost): " + string.Join(" | ", causes.Select(g =>
                $"{s_CauseNames[(int)g.Key]} {Format.Ms(g.Sum(b => b.End - b.Start) / frames)}, {Format.Ms(g.Sum(b => b.Cost) / frames)}")));
            w.WriteLine("# cost: main-thread time blocked on jobs (idle or running them inline) because of the span, at most its length; 0 = spare capacity");

            if (single)
            {
                w.WriteLine("# start: ms from frame start, as in timeline; busy: workers running anything, on average; next: the work that ended the span");
                var f0 = (long)c.Frames[scope.Frames[0]].StartNs;
                var t = new Table("start", "ms", "busy", "cost", "cause", "what");
                var shown = top > 0 && args.Has("top") ? all.OrderByDescending(b => b.Cost).ThenByDescending(b => b.End - b.Start).Take(top).OrderBy(b => b.Start).ToList() : all;
                foreach (var b in shown)
                    t.Add(Format.Ms(b.Start - f0), Format.Ms(b.End - b.Start), Format.Num(b.AvgBusy), Format.Ms(b.Cost),
                          s_CauseNames[(int)b.Cause], Describe(c, finder, b, true));
                t.Write(w);
                if (shown.Count < all.Count)
                    w.WriteLine($"# {all.Count - shown.Count} more with less cost (--top 0)");
            }
            else
            {
                w.WriteLine("# n: spans per frame; longest: the longest single span, which 'what' describes (its frame at the end); next: the work that ended it");
                var groups = all.GroupBy(b => (b.Cause, Describe(c, finder, b, false)))
                                .Select(g => (g.Key, ms: g.Sum(b => b.End - b.Start), cost: g.Sum(b => b.Cost), count: g.Count(), longest: g.OrderByDescending(b => b.End - b.Start).First()))
                                .OrderByDescending(g => sort == "ms" ? g.ms : g.cost).ThenByDescending(g => g.ms).ToList();
                var t = new Table("ms", "cost", "n", "longest", "cause", "what");
                foreach (var g in top > 0 ? groups.Take(top) : groups)
                    t.Add(Format.Ms(g.ms / frames), Format.Ms(g.cost / frames), Format.PerFrame(g.count / frames), Format.Ms(g.longest.End - g.longest.Start),
                          s_CauseNames[(int)g.Key.Cause], Describe(c, finder, g.longest, true) + $" (frame {c.FrameNumber(g.longest.Frame)})");
                t.Write(w);
                if (groups.Count > top && top > 0)
                    w.WriteLine($"# {groups.Count - top} more (--top 0)");
            }

            var share = frameTime > 0 ? cost / frameTime : 0;
            var free = all.Where(b => b.Cost * 20 < b.End - b.Start).Sum(b => b.End - b.Start);
            if (cost / frames < 50_000 || share < 0.01)
                w.WriteLine("verdict: the bubbles cost the main thread almost nothing; they are spare worker capacity, so more parallel work fits for free");
            else
            {
                var worst = causes.First();
                w.WriteLine($"verdict: bubbles cost the main thread {Format.Ms(cost / frames)} ms{per} ({Format.Pct(share)} of the frame), mostly {s_CauseNames[(int)worst.Key]}; " +
                            $"{Format.Ms(free / frames)} ms{per} of bubbles cost nothing (spare capacity, ignore those)");
                // Main-thread time saved only shortens the frame when nothing else bounds it.
                if (elsewhere * 2 >= cost)
                    w.WriteLine($"  but the main thread also waits {Format.Ms(elsewhere / frames)} ms{per} on the render thread, GPU or a frame cap " +
                                "(Gfx.WaitForPresent*, WaitForTargetFPS): saving main-thread time turns largely into more of that waiting, not a shorter frame (see info's bound line)");
            }
            var minAdvice = Math.Max(cost * 0.1, 20_000 * frames);
            foreach (var g in causes.Where(g => g.Sum(b => b.Cost) >= minAdvice).Take(4))
                w.WriteLine("  " + s_Advice[(int)g.Key]);
            return 0;
        }

        static double ParseBusy(string text, CaptureFile c)
        {
            var pct = text.EndsWith("%", StringComparison.Ordinal);
            if (!double.TryParse(pct ? text[..^1] : text, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) || v < 0)
                throw new WaitressException("--busy takes a worker count or a percentage, e.g. 2 or 25%");
            var workers = c.Threads.Count(t => t.Kind == ThreadKind.Worker);
            if ((pct && v >= 100) || (!pct && v >= workers))
                throw new WaitressException($"--busy must leave some workers idle: under 100%, or fewer than the {workers} workers");
            if (pct)
                return v / 100;
            return workers == 0 ? 0 : (v + 0.5) / workers;
        }


        /// <summary>A sample's name, prefixed with its thread unless that is the main thread.</summary>
        static string Name(CaptureFile c, long sample, BubbleFinder finder = null, int release = -1)
        {
            if (sample < 0)
            {
                var begin = release >= 0 ? finder.Jobs.Schedule(release).sample : -1;
                return begin < 0 ? "?" : c.Threads[c.Blocks[c.BlockOf(begin)].Thread].Display + ": ?";
            }
            var thread = c.Threads[c.Blocks[c.BlockOf(sample)].Thread];
            return (thread.Kind == ThreadKind.Main ? "" : thread.Display + ": ") + c.MarkerName(sample);
        }

        /// <summary>What a thread mostly ran in a span, and the child that took the most of it.</summary>
        static string Activity(CaptureFile c, BubbleFinder finder, in Bubble b, bool detail = true) =>
            Name(c, b.Activity, finder, b.Release) + (detail && b.ActivityPart >= 0 ? ", mostly " + c.MarkerName(b.ActivityPart) : "");

        /// <summary>Where the main thread blocked during a span gated by a job, so the wait to remove is named too.</summary>
        static string MainWaited(CaptureFile c, JobIndex jobs, in Bubble b) =>
            b.Cost > 0 && b.MainWaitSample >= 0 ? $"; main waited in {c.MarkerName(b.MainWaitSample)} in {jobs.ParentName(b.MainWaitSample) ?? "?"}" : "";

        static string Describe(CaptureFile c, BubbleFinder finder, in Bubble b, bool detail)
        {
            var jobs = finder.Jobs;
            var next = detail && b.Release >= 0 ? "; next: " + c.Markers[jobs.Instances[b.Release].Marker].Name : "";
            switch (b.Cause)
            {
                case BubbleCause.OneJob:
                {
                    var name = b.Chain > 1
                        ? (detail ? $"chain of {b.Chain} single-threaded jobs, " : "chain of single-threaded jobs, ") + $"{c.Markers[jobs.Instances[b.ChainFirst].Marker].Name} .. {jobs.Describe(b.Gate)}"
                        : jobs.Describe(b.Gate);
                    if (!detail)
                        return name;
                    var handoff = b.End - jobs.Instances[b.Gate].EndAbs;
                    return name + (b.Chain > 1 ? "" : $" ran {Format.Ms(jobs.Instances[b.Gate].Dur)} ms") +
                           (handoff >= 10_000 ? $"; the next job started {Format.Ms(handoff)} ms after it ended" : "") + MainWaited(c, jobs, b) + next;
                }
                case BubbleCause.ParallelTail:
                {
                    if (!detail)
                        return jobs.Describe(b.Gate);
                    var runs = finder.Group(b.Gate).Select(i => jobs.Instances[i].Dur).OrderBy(x => x).ToArray();
                    return $"{jobs.Describe(b.Gate)}: {runs.Length} runs, slowest {Format.Ms(runs[^1])} ms, median {Format.Ms(runs[runs.Length / 2])}" + MainWaited(c, jobs, b) + next;
                }
                case BubbleCause.Sync:
                {
                    var key = $"{Name(c, b.SyncWait)} in {jobs.ParentName(b.SyncWait) ?? "?"}, then {Activity(c, finder, b, detail)} ran alone";
                    if (!detail)
                        return key;
                    var wait = c.Blocks[c.BlockOf(b.SyncWait)];
                    return key + $"; the wait blocked {Format.Ms(c.DurNs(wait, b.SyncWait))} ms" + (b.SyncGate >= 0 ? $", last on {jobs.Describe(b.SyncGate)}" + (b.SyncGateGuessed ? " (timing guess)" : "") : "") + next;
                }
                case BubbleCause.LateSchedule:
                    return $"{Activity(c, finder, b, detail)} ran before {jobs.Describe(b.Release)} was scheduled";
                case BubbleCause.Unstarted:
                    if (b.RanInline)
                        return jobs.Describe(b.Release) + (detail ? $" ran only inline on {(b.Activity >= 0 ? c.Threads[c.Blocks[c.BlockOf(b.Activity)].Thread].Display : "its scheduling thread")} for {Format.Ms(b.End - b.Start)} ms before workers joined" : " ran inline before workers joined");
                    return jobs.Describe(b.Release) + (detail ? $" scheduled {Format.Ms(b.End - b.ScheduledAt)} ms before it started, while {Activity(c, finder, b)} ran" : " waited to start");
                case BubbleCause.NoWork:
                    return "main thread: " + Activity(c, finder, b, detail);
                default:
                    return "main thread: " + Activity(c, finder, b) + next;
            }
        }
    }
}
