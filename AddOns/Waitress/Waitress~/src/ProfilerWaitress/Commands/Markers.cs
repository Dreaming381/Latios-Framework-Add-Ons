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
    public static class Markers
    {
        static readonly HashSet<string> s_Noise = new HashSet<string>(StringComparer.Ordinal) { "Idle", "BeginJob", "EndJob" };

        public static int Top(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "main");
            var top = Scope.Top(args, 25);
            var sort = args.Get("sort", "incl");
            var filter = args.Get("marker") ?? args.Get("m");
            var category = args.Get("category");
            var minMs = args.GetDouble("min", 0);
            var noise = args.Flag("idle");
            args.RejectUnknown();

            var table = FrameTable.Build(c, scope);
            var totals = table.Totals();
            var hits = filter != null ? MarkerMatch.Resolve(c, filter) : null;
            var categoryMatch = category != null ? Glob.Compile(category.Contains('*') ? category : "*" + category + "*") : null;
            if (categoryMatch != null && !c.Markers.Any(m => categoryMatch(m.Category)))
                throw new WaitressException($"--category {category} matches no category; this capture has: " + string.Join(", ", c.Markers.Select(m => m.Category).Distinct().OrderBy(x => x)));
            var n = (double)scope.Frames.Length;
            var rows = totals.Where(t => t.Calls > 0)
                .Where(t => hits == null || hits[t.Marker])
                .Where(t => categoryMatch == null || categoryMatch(c.Markers[t.Marker].Category))
                .Where(t => noise || hits != null || !s_Noise.Contains(c.Markers[t.Marker].Name))
                .Where(t => t.Incl / n / 1e6 >= minMs);
            rows = sort.ToLowerInvariant() switch
            {
                "incl" or "total" => rows.OrderByDescending(t => t.Incl),
                "self" => rows.OrderByDescending(t => t.Self),
                "calls" => rows.OrderByDescending(t => t.Calls),
                "max" => rows.OrderByDescending(t => t.MaxIncl),
                "name" => rows.OrderBy(t => c.Markers[t.Marker].Name, StringComparer.OrdinalIgnoreCase),
                _ => throw new WaitressException($"--sort {sort}: use incl, self, calls, max or name"),
            };
            var list = rows.ToList();
            var shown = top > 0 ? list.Take(top).ToList() : list;
            w.WriteLine($"# {scope.Describe(c)}; ms and calls per frame (avg over the frames in scope); max = worst frame");
            var t = new Table("incl", "self", "calls", "max", "@frame", "in%", "marker");
            foreach (var r in shown)
                t.Add(Format.Ms(r.Incl / n), Format.Ms(r.Self / n), Format.PerFrame(r.Calls / n), Format.Ms(r.MaxIncl),
                      c.FrameNumber(r.MaxFrame).ToString(), Format.Pct(r.Frames / n), c.Markers[r.Marker].Name);
            t.Write(w);
            if (list.Count > shown.Count)
                w.WriteLine($"# {list.Count - shown.Count} more (--top 0 for all)");
            return 0;
        }

        public static int Diff(CaptureFile a, CaptureFile b, Args args, TextWriter w)
        {
            var threads = args.Get("thread") ?? args.Get("t") ?? "main";
            var framesA = args.Get("frames-a") ?? args.Get("frames");
            var framesB = args.Get("frames-b") ?? args.Get("frames");
            var top = Scope.Top(args, 25);
            var self = args.Flag("self");
            var filter = args.Get("marker") ?? args.Get("m");
            var counterRows = args.GetInt("counters", 8);
            args.RejectUnknown();

            var scopeA = new Scope { Frames = Scope.ParseFrames(a, framesA), Threads = Scope.ParseThreads(a, threads), FramesText = framesA, ThreadsText = threads, ShowInternal = args.Flag("internal") };
            var scopeB = new Scope { Frames = Scope.ParseFrames(b, framesB), Threads = Scope.ParseThreads(b, threads), FramesText = framesB, ThreadsText = threads, ShowInternal = args.Flag("internal") };
            FrameTable.Total[] ta = null, tb = null;
            Parallel.Invoke(() => ta = FrameTable.Build(a, scopeA).Totals(), () => tb = FrameTable.Build(b, scopeB).Totals());

            var fa = scopeA.Frames.Select(f => (double)a.Frames[f].TimeNs).OrderBy(x => x).ToArray();
            var fb = scopeB.Frames.Select(f => (double)b.Frames[f].TimeNs).OrderBy(x => x).ToArray();
            w.WriteLine($"A {Path.GetFileName(a.SourcePath)}: {scopeA.Describe(a)}");
            w.WriteLine($"B {Path.GetFileName(b.SourcePath)}: {scopeB.Describe(b)}");
            var ft = new Table("main frame ms", "avg", "p50", "p95", "p99", "max");
            ft.Add("A", Format.Ms(fa.Average()), Format.Ms(Stats.Percentile(fa, .5)), Format.Ms(Stats.Percentile(fa, .95)), Format.Ms(Stats.Percentile(fa, .99)), Format.Ms(fa[^1]));
            ft.Add("B", Format.Ms(fb.Average()), Format.Ms(Stats.Percentile(fb, .5)), Format.Ms(Stats.Percentile(fb, .95)), Format.Ms(Stats.Percentile(fb, .99)), Format.Ms(fb[^1]));
            ft.Add("B-A", Format.SignedMs(fb.Average() - fa.Average()), Format.SignedMs(Stats.Percentile(fb, .5) - Stats.Percentile(fa, .5)),
                   Format.SignedMs(Stats.Percentile(fb, .95) - Stats.Percentile(fa, .95)), Format.SignedMs(Stats.Percentile(fb, .99) - Stats.Percentile(fa, .99)),
                   Format.SignedMs(fb[^1] - fa[^1]));
            ft.Write(w);
            double dAvg = fb.Average() - fa.Average(), dMedian = Stats.Percentile(fb, .5) - Stats.Percentile(fa, .5);
            if (Math.Abs(dAvg - dMedian) > 0.2e6 && (Math.Sign(dAvg) != Math.Sign(dMedian) || Math.Abs(dAvg - dMedian) > 0.5 * Math.Max(Math.Abs(dAvg), Math.Abs(dMedian))))
                w.WriteLine("# note: the average and the median moved differently, so the captures likely hold different workloads or phases; " +
                            "compare matching windows with --frames-a/--frames-b (find them with `frames --sort frame --top 0` or info's histogram)");
            // How much a capture differs from itself bounds what a difference between captures can mean.
            static double HalfDrift(CaptureFile c, int[] frames)
            {
                if (frames.Length < 20)
                    return double.NaN;
                var half = frames.Length / 2;
                return Math.Abs(frames.Take(half).Average(f => (double)c.Frames[f].TimeNs) - frames.Skip(half).Average(f => (double)c.Frames[f].TimeNs));
            }
            var driftA = HalfDrift(a, scopeA.Frames);
            var driftB = HalfDrift(b, scopeB.Frames);
            if (!double.IsNaN(driftA) && !double.IsNaN(driftB))
                w.WriteLine($"# noise: the two halves of A differ by {Format.Ms(driftA)} ms on average, of B by {Format.Ms(driftB)}; " +
                            (Math.Abs(dAvg) <= Math.Max(driftA, driftB) ? "the A-B change is within that, so it may be run-to-run variation" : "the A-B change is larger than that"));
            if (a.HasCallstacks != b.HasCallstacks)
                w.WriteLine($"# note: only {(a.HasCallstacks ? "A" : "B")} recorded allocation callstacks, which costs time on every allocation");
            if (counterRows > 0)
                DiffCounters(a, b, scopeA.Frames, scopeB.Frames, counterRows, w);

            // Marker ids differ between captures, so they are matched by name.
            var byName = new Dictionary<string, (double a, double b, double ca, double cb)>(StringComparer.Ordinal);
            double na = scopeA.Frames.Length, nb = scopeB.Frames.Length;
            void Fold(CaptureFile c, FrameTable.Total[] totals, double frames, bool isB)
            {
                foreach (var t in totals)
                {
                    if (t.Calls == 0)
                        continue;
                    var name = c.Markers[t.Marker].Name;
                    if (s_Noise.Contains(name))
                        continue;
                    byName.TryGetValue(name, out var v);
                    var value = (self ? t.Self : t.Incl) / frames;
                    if (isB)
                        byName[name] = (v.a, v.b + value, v.ca, v.cb + t.Calls / frames);
                    else
                        byName[name] = (v.a + value, v.b, v.ca + t.Calls / frames, v.cb);
                }
            }
            Fold(a, ta, na, false);
            Fold(b, tb, nb, true);
            var match = filter != null ? Glob.Compile(filter.Contains('*') ? filter : "*" + filter + "*") : null;
            var rows = byName.Where(kv => match == null || match(kv.Key)).OrderByDescending(kv => Math.Abs(kv.Value.b - kv.Value.a)).ToList();
            var shown = top > 0 ? rows.Take(top) : rows;
            w.WriteLine($"# {(self ? "self" : "inclusive")} ms per frame by marker name, largest change first");
            var t2 = new Table("A", "B", "B-A", "change", "calls A>B", "marker");
            foreach (var kv in shown)
            {
                var (va, vb, ca, cb) = kv.Value;
                var change = va == 0 ? "new" : vb == 0 ? "gone" : Format.Pct((vb - va) / va).Replace("%", "%").Insert(0, vb >= va ? "+" : "");
                t2.Add(Format.Ms(va), Format.Ms(vb), Format.SignedMs(vb - va), change, $"{Format.PerFrame(ca)}>{Format.PerFrame(cb)}", kv.Key);
            }
            t2.Write(w);
            return 0;
        }

        /// <summary>Counters both captures have, by largest relative change of their average.</summary>
        static void DiffCounters(CaptureFile a, CaptureFile b, int[] framesA, int[] framesB, int rows, TextWriter w)
        {
            static Dictionary<string, (double avg, byte unit)> Averages(CaptureFile c, int[] frames)
            {
                var result = new Dictionary<string, (double, byte)>(StringComparer.Ordinal);
                foreach (var counter in c.Counters)
                {
                    var values = frames.Select(f => counter.Values[f]).Where(v => !double.IsNaN(v)).ToArray();
                    if (values.Length > 0)
                        result[c.Markers[counter.Marker].Name] = (values.Average(), counter.Unit);
                }
                return result;
            }
            var ca = Averages(a, framesA);
            var cb = Averages(b, framesB);
            var changes = ca.Where(kv => cb.ContainsKey(kv.Key))
                .Select(kv => (name: kv.Key, a: kv.Value.avg, b: cb[kv.Key].avg, unit: kv.Value.unit))
                .Where(x => Math.Max(Math.Abs(x.a), Math.Abs(x.b)) > 0)
                .Select(x => (x.name, x.a, x.b, x.unit, rel: (x.b - x.a) / Math.Max(Math.Abs(x.a), Math.Abs(x.b))))
                .Where(x => Math.Abs(x.rel) >= 0.02)
                .OrderByDescending(x => Math.Abs(x.rel)).ToList();
            if (changes.Count == 0)
                return;
            w.WriteLine($"# counters, average per frame: the {Math.Min(rows, changes.Count)} that changed most of {changes.Count} that changed by 2% or more (--counters N; 0 hides)");
            var t = new Table("A", "B", "change", "counter");
            foreach (var x in changes.Take(rows))
                t.Add(Format.Unit(x.a, x.unit), Format.Unit(x.b, x.unit), x.a == 0 ? "new" : (x.b >= x.a ? "+" : "") + Format.Pct((x.b - x.a) / Math.Abs(x.a)), x.name);
            t.Write(w);
        }

        public static unsafe int Find(CaptureFile c, Args args, TextWriter w)
        {
            var pattern = args.PositionalAt(1, "marker pattern");
            var scope = Scope.Parse(c, args, "all");
            var top = Scope.Top(args, 20);
            var sort = args.Get("sort", "dur");
            if (sort != "dur" && sort != "start")
                throw new WaitressException("--sort takes dur or start");
            var minMs = args.GetDouble("min", 0);
            var under = args.Get("under");
            args.RejectUnknown();
            var hits = MarkerMatch.Resolve(c, pattern);
            var underHits = under != null ? MarkerMatch.Resolve(c, under) : null;
            var minNs = (long)(minMs * 1e6);

            var found = new List<(long dur, long start, int frame, int thread, string path, long sample)>();
            var gate = new object();
            Parallel.ForEach(Partition(scope.Frames), frames =>
            {
                var anc = new Ancestry(c);
                var local = new List<(long, long, int, int, string, long)>();
                foreach (var b in Blocks.Of(c, frames, scope.Threads))
                {
                    var block = c.Blocks[b];
                    anc.Walk(b, (s, d) =>
                    {
                        if (d == 0 || !hits[c.SampleMarker[s]])
                            return;
                        var dur = c.DurNs(block, s);
                        if (dur < minNs || (underHits != null && !anc.HasAncestor(underHits)))
                            return;
                        local.Add((dur, c.StartNs(block, s), block.Frame, block.Thread, anc.Path(3), s));
                    });
                }
                lock (gate)
                    found.AddRange(local);
            });
            var ordered = sort == "start" ? found.OrderBy(x => x.frame).ThenBy(x => x.start) : found.OrderByDescending(x => x.dur);
            var shown = (top > 0 ? ordered.Take(top) : ordered).ToList();
            w.WriteLine($"# {found.Count} instances in {scope.Describe(c)}; sorted by {sort}; start = ms from frame start");
            var t = new Table("ms", "frame", "start", "thread", "marker", "parents");
            foreach (var x in shown)
            {
                var meta = MetaSummary(c, x.sample);
                t.Add(Format.Ms(x.dur), c.FrameNumber(x.frame).ToString(), Format.Ms(x.start), c.Threads[x.thread].Display,
                      c.MarkerName(x.sample) + meta, x.path);
            }
            t.Write(w);
            if (found.Count > 0)
            {
                var durs = found.Select(x => (double)x.dur).OrderBy(x => x).ToArray();
                w.WriteLine($"# instance ms: {Overview.Distribution(durs)}; total {Format.Ms(durs.Sum() / scope.Frames.Length)} ms/frame");
            }
            return 0;
        }

        public static string MetaSummary(CaptureFile c, long sample)
        {
            var i = c.FirstMeta(sample);
            if (i < 0)
                return "";
            var meta = c.Metadata;
            var parts = new List<string>();
            for (; i < meta.Length && meta[i].Sample == sample; i++)
            {
                var text = meta[i].Unit == 2 && meta[i].Kind == 0 ? Format.Bytes(meta[i].Bits) : c.MetaText(meta[i]);
                parts.Add(meta[i].Name.Length > 0 ? meta[i].Name + "=" + text : text);
            }
            return " {" + string.Join(" ", parts) + "}";
        }

        public static IEnumerable<int[]> Partition(int[] frames)
        {
            var size = Math.Max(1, frames.Length / (Environment.ProcessorCount * 2));
            for (var i = 0; i < frames.Length; i += size)
                yield return frames.Skip(i).Take(size).ToArray();
        }

        public static unsafe int Marker(CaptureFile c, Args args, TextWriter w)
        {
            var pattern = args.PositionalAt(1, "marker name or pattern");
            var scope = Scope.Parse(c, args, "all");
            var top = Scope.Top(args, 8);
            args.RejectUnknown();
            var hits = MarkerMatch.List(MarkerMatch.Resolve(c, pattern));
            if (hits.Count > 6)
            {
                var table = FrameTable.Build(c, scope).Totals();
                w.WriteLine($"'{pattern}' matches {hits.Count} markers; the busiest (ms/frame, {scope.Describe(c)}):");
                foreach (var m in hits.OrderByDescending(m => table[m].Incl).Take(15))
                    w.WriteLine($"  {Format.Ms(table[m].Incl / (double)scope.Frames.Length),8}  {c.Markers[m].Name}");
                w.WriteLine("narrow it down, or quote the exact name");
                return 2;
            }
            foreach (var m in hits)
                Focus(c, scope, m, top, w);
            return 0;
        }

        static unsafe void Focus(CaptureFile c, Scope scope, int marker, int top, TextWriter w)
        {
            var info = c.Markers[marker];
            var n = scope.Frames.Length;
            var perFrame = new long[n];
            var perThread = new long[c.Threads.Length];
            var parents = new Dictionary<string, long>();
            var children = new Dictionary<int, long>();
            long self = 0, calls = 0, maxInstance = 0, instanceSample = -1;
            var instances = new List<double>();
            var gate = new object();
            Parallel.For(0, n, i =>
            {
                var anc = new Ancestry(c);
                var lParents = new Dictionary<string, long>();
                var lChildren = new Dictionary<int, long>();
                long lSelf = 0, lCalls = 0, lMax = 0, lMaxSample = -1, frameTotal = 0;
                var lThread = new Dictionary<int, long>();
                var lInst = new List<double>();
                var frame = c.Frames[scope.Frames[i]];
                for (var b = frame.BlockFirst; b < frame.BlockFirst + frame.BlockCount; b++)
                {
                    var block = c.Blocks[b];
                    if (!scope.HasThread(block.Thread))
                        continue;
                    long openUntil = -1, current = -1, childSum = 0, currentDur = 0;
                    var outerDepth = -1;
                    void Close()
                    {
                        if (current < 0)
                            return;
                        lSelf += Math.Max(0, currentDur - childSum);
                        current = -1;
                    }
                    anc.Walk(b, (s, d) =>
                    {
                        if (current >= 0 && s >= openUntil)
                            Close();
                        if (outerDepth >= 0 && d <= outerDepth)
                            outerDepth = -1;
                        if (current >= 0 && d == c.SampleDepth[current] + 1)
                        {
                            var dur = c.DurNs(block, s);
                            childSum += dur;
                            lChildren.TryGetValue(c.SampleMarker[s], out var v);
                            lChildren[c.SampleMarker[s]] = v + dur;
                        }
                        if (d == 0 || c.SampleMarker[s] != marker)
                            return;
                        var length = c.DurNs(block, s);
                        lCalls++;
                        lInst.Add(length);
                        if (length > lMax)
                        {
                            lMax = length;
                            lMaxSample = s;
                        }
                        if (outerDepth < 0)
                        {
                            outerDepth = d;
                            frameTotal += length;
                            lThread.TryGetValue(block.Thread, out var tv);
                            lThread[block.Thread] = tv + length;
                            var thread = c.Threads[block.Thread];
                            var parent = d > 1 ? c.MarkerName(anc.Stack[d - 1])
                                : "(thread root: " + (thread.Kind is ThreadKind.Worker or ThreadKind.Background ? thread.Group + "/*" : thread.Display) + ")";
                            if (d > 1 && c.Threads[block.Thread].Kind == ThreadKind.Worker)
                                parent += " (on workers)";
                            lParents.TryGetValue(parent, out var pv);
                            lParents[parent] = pv + length;
                        }
                        Close();
                        current = s;
                        currentDur = length;
                        childSum = 0;
                        openUntil = NextSibling(c, block, s);
                    });
                    Close();
                }
                perFrame[i] = frameTotal;
                lock (gate)
                {
                    self += lSelf;
                    calls += lCalls;
                    instances.AddRange(lInst);
                    if (lMax > maxInstance)
                    {
                        maxInstance = lMax;
                        instanceSample = lMaxSample;
                    }
                    foreach (var kv in lThread)
                        perThread[kv.Key] += kv.Value;
                    foreach (var kv in lParents)
                    {
                        parents.TryGetValue(kv.Key, out var v);
                        parents[kv.Key] = v + kv.Value;
                    }
                    foreach (var kv in lChildren)
                    {
                        children.TryGetValue(kv.Key, out var v);
                        children[kv.Key] = v + kv.Value;
                    }
                }
            });

            var total = perFrame.Sum();
            w.WriteLine($"marker \"{info.Name}\"  [{info.Category}]  {scope.Describe(c)}");
            if (calls == 0)
            {
                w.WriteLine("  no samples in scope");
                return;
            }
            var present = perFrame.Count(x => x > 0);
            var sorted = perFrame.Select(x => (double)x).OrderBy(x => x).ToArray();
            w.WriteLine($"  per frame  {Overview.Distribution(sorted)}  (ms; in {present}/{n} frames)");
            w.WriteLine($"  totals     incl {Format.Ms(total / (double)n)}  self {Format.Ms(self / (double)n)}  calls {Format.PerFrame(calls / (double)n)}  per frame");
            var inst = instances.OrderBy(x => x).ToArray();
            var where = instanceSample >= 0 ? Locate(c, instanceSample) : "";
            w.WriteLine($"  instance   avg {Format.Ms(inst.Average())}  p50 {Format.Ms(Stats.Percentile(inst, .5))}  p95 {Format.Ms(Stats.Percentile(inst, .95))}  max {Format.Ms(inst[^1])} {where}");

            var byKind = Enumerable.Range(0, c.Threads.Length).Where(t => perThread[t] > 0)
                .GroupBy(t => c.Threads[t].Kind == ThreadKind.Worker || c.Threads[t].Kind == ThreadKind.Background ? c.Threads[t].Group + "/*" : c.Threads[t].Display)
                .Select(g => (g.Key, ms: g.Sum(t => perThread[t]) / (double)n, count: g.Count())).OrderByDescending(x => x.ms).ToList();
            w.WriteLine("  threads    " + string.Join(", ", byKind.Take(top).Select(x => $"{x.Key}{(x.count > 1 ? $" ({x.count})" : "")} {Format.Ms(x.ms)}")));
            if (parents.Count > 0)
            {
                w.WriteLine("  called from (incl ms/frame):");
                foreach (var kv in parents.OrderByDescending(kv => kv.Value).Take(top))
                    w.WriteLine($"    {Format.Ms(kv.Value / (double)n),8}  {kv.Key}");
            }
            if (children.Count > 0)
            {
                w.WriteLine($"  children (incl ms/frame; self is {Format.Pct(total > 0 ? (double)self / total : 0)} of incl):");
                foreach (var kv in children.OrderByDescending(kv => kv.Value).Take(top))
                    w.WriteLine($"    {Format.Ms(kv.Value / (double)n),8}  {c.Markers[kv.Key].Name}");
                if (children.Count > top)
                    w.WriteLine($"    ... {children.Count - top} more");
            }
            var worst = Enumerable.Range(0, n).OrderByDescending(i => perFrame[i]).Take(Math.Min(top, 6))
                .Select(i => $"{c.FrameNumber(scope.Frames[i])} ({Format.Ms(perFrame[i])})");
            w.WriteLine("  worst frames  " + string.Join(", ", worst));
        }

        /// <summary>Index one past a sample's subtree.</summary>
        public static unsafe long NextSibling(CaptureFile c, in BlockInfo block, long s)
        {
            var d = c.SampleDepth[s];
            var end = block.SampleBase + block.SampleCount;
            var i = s + 1;
            while (i < end && c.SampleDepth[i] > d)
                i++;
            return i;
        }

        public static string Locate(CaptureFile c, long sample)
        {
            var b = c.BlockOf(sample);
            var block = c.Blocks[b];
            return $"(frame {c.FrameNumber(block.Frame)}, {c.Threads[block.Thread].Display} @{Format.Ms(c.StartNs(block, sample))})";
        }
    }
}
