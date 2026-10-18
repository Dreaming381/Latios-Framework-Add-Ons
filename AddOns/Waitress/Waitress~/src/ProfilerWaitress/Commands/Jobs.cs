using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProfilerWaitress.Analysis;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;
using ProfilerWaitress.Text;

namespace ProfilerWaitress.Commands
{
    public static class Jobs
    {
        public static int List(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "all");
            var top = Scope.Top(args, 25);
            var filter = args.Get("marker") ?? args.Get("m");
            var fromText = args.Get("from");
            args.RejectUnknown();
            var index = new JobIndex(c);
            var from = fromText != null ? Glob.Compile(fromText.Contains('*') ? fromText : "*" + fromText + "*") : null;
            var inFrame = new bool[c.FrameCount];
            foreach (var f in scope.Frames)
                inFrame[f] = true;
            var hits = filter != null ? MarkerMatch.Resolve(c, filter) : null;


            var stats = new Dictionary<int, (long total, long count, long max, long onMain, Dictionary<string, int> sched, HashSet<int> threads)>();
            for (var i = 0; i < index.Instances.Count; i++)
            {
                var inst = index.Instances[i];
                if (!inFrame[inst.Frame] || !scope.HasThread(inst.Thread) || (hits != null && !hits[inst.Marker]))
                    continue;
                if (from != null && !inst.Flows.Any(f => index.Scheduler(f, inst.StartAbs) is { } who && from(who)))
                    continue;
                if (!stats.TryGetValue(inst.Marker, out var s))
                    s = (0, 0, 0, 0, new Dictionary<string, int>(), new HashSet<int>());
                s.total += inst.Dur;
                s.count++;
                s.max = Math.Max(s.max, inst.Dur);
                if (c.Threads[inst.Thread].Kind == ThreadKind.Main)
                    s.onMain += inst.Dur;
                s.threads.Add(inst.Thread);
                foreach (var f in inst.Flows)
                {
                    var who = index.Scheduler(f, inst.StartAbs);
                    if (who != null)
                    {
                        s.sched.TryGetValue(who, out var k);
                        s.sched[who] = k + 1;
                    }
                }
                stats[inst.Marker] = s;
            }
            var n = (double)scope.Frames.Length;
            w.WriteLine($"# job executions in {scope.Describe(c)}; ms and runs per frame; main = run on the main thread while it waited");
            var t = new Table("ms", "runs", "avg", "longest", "main", "threads", "job", "scheduled by");
            var rows = stats.OrderByDescending(kv => kv.Value.total).ToList();
            foreach (var kv in top > 0 ? rows.Take(top) : rows)
            {
                var s = kv.Value;
                var sched = s.sched.Count == 0 ? "-" : string.Join(", ", s.sched.OrderByDescending(x => x.Value).Take(2).Select(x => x.Key));
                t.Add(Format.Ms(s.total / n), Format.PerFrame(s.count / n), Format.Ms(s.total / (double)s.count), Format.Ms(s.max),
                      Format.Ms(s.onMain / n), s.threads.Count.ToString(), c.Markers[kv.Key].Name, sched);
            }
            t.Write(w);
            if (rows.Count > top && top > 0)
                w.WriteLine($"# {rows.Count - top} more jobs (--top 0)");
            return 0;
        }

        public static bool IsJobWait(string name) => name.Contains("Job", StringComparison.Ordinal) || name.Contains("Complete", StringComparison.Ordinal);

        static readonly Dictionary<string, string> s_Meaning = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Gfx.WaitForPresentOnGfxThread"] = "main thread waits for the render thread to finish the previous frame: render thread or GPU bound",
            ["Gfx.WaitForPresent"] = "waiting for the GPU to present: GPU bound or VSync",
            ["WaitForTargetFPS"] = "frame rate cap or VSync: idle by choice",
            ["Gfx.WaitForGfxCommandsFromMainThread"] = "render thread waits for the main thread: main thread bound",
            ["Gfx.WaitForRenderThread"] = "main thread waits for the render thread",
            ["Semaphore.WaitForSignal"] = "a thread blocked on a semaphore; see its parent",
        };

        public static bool IsWait(string name) =>
            name.Contains("WaitFor", StringComparison.Ordinal) || name.StartsWith("Semaphore.Wait", StringComparison.Ordinal) ||
            name == "JobHandle.Complete" || name.EndsWith(".Complete", StringComparison.Ordinal) || name.StartsWith("Wait", StringComparison.Ordinal);

        public static unsafe int Waits(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "main");
            var top = Scope.Top(args, 15);
            var filter = args.Get("marker") ?? args.Get("m");
            var jobsPerSite = args.GetInt("jobs", 3);
            var sort = args.Get("sort", "total");
            if (sort != "total" && sort != "idle" && sort != "count")
                throw new WaitressException("--sort takes total, idle or count");
            args.RejectUnknown();
            var waitMarkers = filter != null ? MarkerMatch.Resolve(c, filter) : c.Markers.Select(m => IsWait(m.Name)).ToArray();
            var index = new JobIndex(c);

            var sites = new Dictionary<string, (long total, long idle, long count, long max, long maxSample, Dictionary<string, long> critical, Dictionary<string, long> inline)>();
            var anc = new Ancestry(c);
            foreach (var b in Blocks.Of(c, scope.Frames, scope.Threads))
            {
                var block = c.Blocks[b];
                anc.Walk(b, (s, d) =>
                {
                    if (d == 0 || !waitMarkers[c.SampleMarker[s]])
                        return;
                    // A wait nested in another wait is part of the outer one.
                    for (var i = 1; i < d; i++)
                        if (waitMarkers[c.SampleMarker[anc.Stack[i]]])
                            return;
                    var dur = c.DurNs(block, s);
                    var parent = d > 1 ? c.MarkerName(anc.Stack[d - 1]) : "(root)";
                    var key = c.MarkerName(s) + " in " + parent;
                    if (c.Threads[block.Thread].Kind != ThreadKind.Main)
                        key = c.Threads[block.Thread].Display + ": " + key;
                    sites.TryGetValue(key, out var site);
                    site.critical ??= new Dictionary<string, long>();
                    site.inline ??= new Dictionary<string, long>();
                    site.total += dur;
                    site.count++;
                    if (dur > site.max)
                    {
                        site.max = dur;
                        site.maxSample = s;
                    }
                    // Jobs run inline during the wait are work, not waiting.
                    long stolen = 0;
                    var end = Markers.NextSibling(c, block, s);
                    for (var i = s + 1; i < end; i++)
                    {
                        if (!index.InstanceOf.TryGetValue(i, out var inst))
                            continue;
                        var job = index.Instances[inst];
                        stolen += job.Dur;
                        var sched = job.Flows.Select(f => index.Scheduler(f, job.StartAbs)).FirstOrDefault(x => x != null);
                        var jobName = c.Markers[job.Marker].Name + (sched != null ? " (from " + sched + ")" : "");
                        site.inline.TryGetValue(jobName, out var ran);
                        site.inline[jobName] = ran + job.Dur;
                    }
                    site.idle += Math.Max(0, dur - stolen);
                    var waitEnd = (long)(block.BaseNs + ((ulong)c.SampleStart[s] << block.Shift)) + dur;
                    var lastEnd = long.MinValue;
                    string lastJob = null;
                    // The End flows sit on the WaitForJobGroupID inside a JobHandle.Complete, so the whole subtree counts.
                    for (var w2 = s; w2 < end; w2++)
                    {
                        if (!index.EndsAt.TryGetValue(w2, out var flows))
                            continue;
                        foreach (var f in flows)
                        {
                            foreach (var i in index.EndedBy(f, waitEnd))
                            {
                                var job = index.Instances[i];
                                if (job.EndAbs > lastEnd)
                                {
                                    lastEnd = job.EndAbs;
                                    var sched = index.Scheduler(f, job.StartAbs);
                                    lastJob = c.Markers[job.Marker].Name + (sched != null ? " (from " + sched + ")" : "");
                                }
                            }
                        }
                    }
                    if (lastJob == null && stolen == 0 && IsJobWait(c.MarkerName(s)))
                    {
                        // No flow ended here. A wait returns within microseconds of its last dependency finishing, so
                        // a job that finished just before the wait returned is the likely one.
                        var guess = index.LastFinishedWithin(Math.Max(waitEnd - dur, waitEnd - 50_000), waitEnd + 1000);
                        if (guess >= 0)
                        {
                            var job = index.Instances[guess];
                            var sched = job.Flows.Select(f => index.Scheduler(f, job.StartAbs)).FirstOrDefault(x => x != null);
                            lastJob = c.Markers[job.Marker].Name + (sched != null ? " (from " + sched + ")" : "") + " (timing guess)";
                        }
                    }
                    if (lastJob != null)
                    {
                        site.critical.TryGetValue(lastJob, out var v);
                        site.critical[lastJob] = v + dur;
                    }
                    sites[key] = site;
                });
            }

            var n = (double)scope.Frames.Length;
            var all = sites.Values.Sum(x => x.total);
            var allIdle = sites.Values.Sum(x => x.idle);
            w.WriteLine($"# waits in {scope.Describe(c)}: {Format.Ms(all / n)} ms/frame, of which idle {Format.Ms(allIdle / n)} (the rest ran jobs inline)");
            w.WriteLine($"# by {sort}; ms and count per frame. inline = jobs the waiting thread ran itself; last job = the job that finished last before the wait returned (share of wait time)");
            var ordered = sort == "idle" ? sites.OrderByDescending(kv => kv.Value.idle) : sort == "count" ? sites.OrderByDescending(kv => kv.Value.count) : sites.OrderByDescending(kv => kv.Value.total);
            foreach (var kv in ordered.Take(top > 0 ? top : int.MaxValue))
            {
                var s = kv.Value;
                w.WriteLine($"{Format.Ms(s.total / n),8} ms  idle {Format.Ms(s.idle / n)}  x{Format.PerFrame(s.count / n)}  longest {Format.Ms(s.max)}  {kv.Key}");
                var waitName = kv.Key.Contains(" in ") ? kv.Key.Substring(0, kv.Key.IndexOf(" in ", StringComparison.Ordinal)) : kv.Key;
                if (s_Meaning.TryGetValue(waitName.Substring(waitName.LastIndexOf(": ", StringComparison.Ordinal) + 1).Trim(), out var meaning))
                    w.WriteLine($"           = {meaning}");
                foreach (var job in s.inline.OrderByDescending(x => x.Value).Take(jobsPerSite))
                    w.WriteLine($"           inline {Format.Ms(job.Value / n)}  {job.Key}");
                foreach (var job in s.critical.Where(x => x.Value * 100 >= s.total).OrderByDescending(x => x.Value).Take(jobsPerSite))
                    w.WriteLine($"           last job {Format.Pct(s.total > 0 ? (double)job.Value / s.total : 0)}  {job.Key}");
            }
            if (sites.Count > top && top > 0)
                w.WriteLine($"# {sites.Count - top} more wait sites (--top 0)");
            return 0;
        }
    }
}
