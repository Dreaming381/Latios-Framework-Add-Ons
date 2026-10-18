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
    public static class Overview
    {
        public static int Info(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "main");
            args.RejectUnknown();
            var frames = scope.Frames;
            var ms = frames.Select(f => (double)c.Frames[f].TimeNs).ToArray();
            var sorted = ms.OrderBy(x => x).ToArray();
            var source = new FileInfo(c.SourcePath);
            var span = c.Frames[frames[^1]].StartNs + c.Frames[frames[^1]].TimeNs - c.Frames[frames[0]].StartNs;

            w.WriteLine($"capture  {c.SourcePath}" + (source.Exists ? $" ({Format.Bytes(source.Length)})" : " (source gone; cache only)"));
            w.WriteLine($"frames   {frames.Length} ({c.FrameNumber(frames[0])}-{c.FrameNumber(frames[^1])}), {span / 1e9:0.00} s, " +
                        $"{frames.Length / (span / 1e9):0.#} fps" + (scope.FramesText == null ? "" : $" [--frames {scope.FramesText}]"));
            w.WriteLine("main ms  " + Distribution(sorted) + $"  (max at frame {c.FrameNumber(frames[Array.IndexOf(ms, sorted[^1])])})");
            var gpu = frames.Select(f => (double)c.Frames[f].GpuNs).Where(x => x > 0).OrderBy(x => x).ToArray();
            w.WriteLine("gpu ms   " + (gpu.Length > 0
                ? Distribution(gpu) + (gpu.Length < frames.Length ? $"  ({gpu.Length} frames with GPU time)" : "") + (c.GpuFromCounter ? (c.GpuCounterLag >= 0 ? $"  (GPU Frame Time counter, moved back {c.GpuCounterLag} frames to the frame it measured)" : "  (GPU Frame Time counter; its lag couldn't be measured, so per-frame values may be a few frames late)") : "")
                : "not recorded"));
            w.WriteLine("histo    " + Histogram(sorted));
            w.WriteLine("bound    " + Bound(c, frames, sorted.Average(), gpu.Length > 0 ? gpu.Average() : 0));

            var kinds = c.Threads.GroupBy(t => t.Kind).ToDictionary(g => g.Key, g => g.Count());
            int K(ThreadKind k) => kinds.TryGetValue(k, out var n) ? n : 0;
            w.WriteLine($"threads  {c.Threads.Length}: main, {(K(ThreadKind.Render) > 0 ? "render, " : "")}{K(ThreadKind.Worker)} workers, " +
                        $"{K(ThreadKind.Background)} background, {K(ThreadKind.Other)} other");

            var gcBytes = FindCounter(c, "GC Allocated In Frame");
            var gcText = "";
            if (gcBytes >= 0)
            {
                var values = frames.Select(f => c.Counters[gcBytes].Values[f]).Where(v => !double.IsNaN(v)).ToArray();
                var with = values.Count(v => v > 0);
                gcText = with == 0 ? ", GC alloc none" : $", GC alloc {Format.Bytes(values.Average())}/frame in {with} frames";
            }
            w.WriteLine($"data     {c.SampleCount:N0} samples, {c.Markers.Length} markers, {c.Counters.Length} counters, " +
                        $"callstacks {(c.HasCallstacks ? "yes" : "no")}{gcText}");
            var slow = Enumerable.Range(0, frames.Length).OrderByDescending(i => ms[i]).Take(6)
                .Select(i => $"{c.FrameNumber(frames[i])} ({Format.Ms(ms[i])})");
            w.WriteLine("slowest  " + string.Join(", ", slow));
            if (c.FrameCount == c.MaxFrameSetting && c.SourcePath.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
                w.WriteLine($"note     {c.FrameCount} frames equals the Profiler's frame-count preference; a .data file saved from the window keeps only that many");
            return 0;
        }

        /// <summary>
        /// Which part of the pipeline the frame waits on, from the main thread's waits, the render thread's work
        /// and waits, GPU time and worker load. A heuristic; the numbers it is based on are printed with it.
        /// </summary>
        static unsafe string Bound(CaptureFile c, int[] frames, double frameNs, double gpuNs)
        {
            var main = c.Threads.Select(t => t.Kind == ThreadKind.Main).ToArray();
            var render = c.Threads.Select(t => t.Kind == ThreadKind.Render).ToArray();
            var mainTable = FrameTable.Build(c, new Scope { Frames = frames, Threads = main, ThreadsText = "main" }).Totals();
            var renderTable = render.Any(x => x) ? FrameTable.Build(c, new Scope { Frames = frames, Threads = render, ThreadsText = "render" }).Totals() : null;
            var n = (double)frames.Length;
            double Sum(FrameTable.Total[] totals, bool self, params string[] names)
            {
                if (totals == null)
                    return 0;
                double sum = 0;
                foreach (var t in totals)
                    if (t.Calls > 0 && names.Contains(c.Markers[t.Marker].Name))
                        sum += self ? t.Self : t.Incl;
                return sum / n;
            }
            var present = Sum(mainTable, false, "Gfx.WaitForPresentOnGfxThread", "Gfx.WaitForPresent", "Gfx.WaitForRenderThread");
            var cap = Sum(mainTable, false, "WaitForTargetFPS", "Application.Idle");
            var jobs = Sum(mainTable, true, "WaitForJobGroupID") + Sum(mainTable, false, "Idle");
            var renderWaits = Sum(renderTable, false, "Gfx.WaitForGfxCommandsFromMainThread");

            double workerBusy = 0;
            var workers = 0;
            var idle = MarkerMatch.Resolve(c, "Idle", false);
            for (var t = 0; t < c.Threads.Length; t++)
                if (c.Threads[t].Kind == ThreadKind.Worker)
                    workers++;
            foreach (var b in Blocks.Of(c, frames, c.Threads.Select(t => t.Kind == ThreadKind.Worker).ToArray()))
            {
                var block = c.Blocks[b];
                for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                    if (c.SampleDepth[s] == 1 && !idle[c.SampleMarker[s]])
                        workerBusy += c.DurNs(block, s);
            }
            var workerUtil = workers > 0 ? workerBusy / n / frameNs / workers : 0;

            string verdict;
            if (cap > 0.15 * frameNs)
                verdict = "frame-rate cap, VSync or Editor throttling";
            else if (present > 0.1 * frameNs)
                verdict = gpuNs > 0.85 * frameNs ? "GPU" : renderTable != null ? "render thread" : "render thread or GPU";
            else if (jobs > 0.15 * frameNs)
                verdict = "main thread, waiting on jobs";
            else
                verdict = "main thread";
            var parts = new List<string> { $"main waits {Format.Ms(present)} on render/present, {Format.Ms(jobs)} idle on jobs" };
            if (cap > 0)
                parts.Add($"{Format.Ms(cap)} capped (WaitForTargetFPS, or Application.Idle in an Editor)");
            if (renderTable != null)
                parts.Add($"render waits {Format.Ms(renderWaits)} on main");
            if (gpuNs > 0)
                parts.Add($"gpu {Format.Ms(gpuNs)}");
            if (workers > 0)
                parts.Add($"workers {Format.Pct(workerUtil)} busy");
            return $"{verdict} (heuristic; ms/frame: {string.Join(", ", parts)})";
        }

        public static string Distribution(double[] sorted) =>
            $"avg {Format.Ms(sorted.Average())}  p50 {Format.Ms(Stats.Percentile(sorted, 0.5))}  p95 {Format.Ms(Stats.Percentile(sorted, 0.95))}  " +
            $"p99 {Format.Ms(Stats.Percentile(sorted, 0.99))}  max {Format.Ms(sorted[^1])}";

        static string Histogram(double[] sortedNs)
        {
            double[] edges = { 4.17, 8.33, 11.1, 16.7, 33.3, 66.7 };
            var counts = new int[edges.Length + 1];
            foreach (var ns in sortedNs)
            {
                var ms = ns / 1e6;
                var b = 0;
                while (b < edges.Length && ms >= edges[b])
                    b++;
                counts[b]++;
            }
            var parts = new List<string>();
            for (var b = 0; b <= edges.Length; b++)
            {
                if (counts[b] == 0)
                    continue;
                var label = b == 0 ? $"<{edges[0]}" : b == edges.Length ? $">{edges[^1]}" : $"{edges[b - 1]}-{edges[b]}";
                parts.Add($"{label}:{counts[b]}");
            }
            return string.Join("  ", parts) + "  (ms:frames)";
        }

        public static int FindCounter(CaptureFile c, string name)
        {
            for (var i = 0; i < c.Counters.Length; i++)
                if (string.Equals(c.Markers[c.Counters[i].Marker].Name, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        sealed class Column
        {
            public string Name;
            public Func<int, int, double> Value; // (row in scope, frame index)
            public byte Unit; // ProfilerMarkerDataUnit; marker columns are times
        }

        /// <summary>
        /// A --cols entry: a counter by exact name, or a marker (exact name, a glob, or text naming exactly one
        /// marker), optionally on its own threads with '@': "DrawBuffersBatchMode@render".
        /// </summary>
        static Column ResolveColumn(CaptureFile c, Scope scope, string spec, Dictionary<string, FrameTable> tables)
        {
            var at = spec.LastIndexOf('@');
            var name = at > 0 ? spec.Substring(0, at) : spec;
            var threads = at > 0 ? spec.Substring(at + 1) : null;
            if (name is "ms" or "gpu" or "gc" or "frame")
                throw new WaitressException($"--cols {name}: '{name}' is always a column; --cols takes counters and markers");
            var counter = FindCounter(c, name);
            if (counter >= 0 && threads == null)
            {
                var ci = c.Counters[counter];
                return new Column { Name = name, Value = (row, f) => ci.Values[f], Unit = ci.Unit };
            }
            var hits = MarkerMatch.Resolve(c, name, false);
            var names = MarkerMatch.List(hits).Select(m => c.Markers[m].Name).Distinct().ToList();
            if (names.Count == 0)
                throw new WaitressException($"--cols {name}: no counter or marker has that name; `counters <cap> --all` lists counters");
            if (names.Count > 1 && !name.Contains('*') && !name.Contains('?'))
                throw new WaitressException($"--cols {name} matches {names.Count} markers ({string.Join(", ", names.Take(6))}{(names.Count > 6 ? ", ..." : "")}); " +
                                            "use an exact name, or a glob to sum them");
            var key = threads ?? scope.ThreadsText;
            if (!tables.TryGetValue(key, out var table))
            {
                var columnScope = new Scope { Frames = scope.Frames, Threads = Scope.ParseThreads(c, key), ThreadsText = key, ShowInternal = scope.ShowInternal };
                tables[key] = table = FrameTable.Build(c, columnScope);
            }
            var markers = MarkerMatch.List(hits);
            return new Column { Name = threads != null ? spec : name, Value = (row, f) => markers.Sum(m => (double)table.Get(row, m)), Unit = 1 };
        }

        static string Cell(double value, byte unit) =>
            double.IsNaN(value) ? "-" : unit == 1 ? Format.Ms(value) : Format.Num(value, unit == 4 || unit == 5 ? 1 : 0);

        static string Header(string name, byte unit) => unit switch
        {
            1 => name + " ms",
            2 => name + " B",
            4 => name + " %",
            5 => name + " Hz",
            _ => name,
        };

        /// <summary>GC.Alloc bytes per frame on the scope's threads, or null when allocation samples carry no sizes.</summary>
        static unsafe double[] GcPerFrame(CaptureFile c, Scope scope)
        {
            var alloc = MarkerMatch.Resolve(c, "GC.Alloc", false);
            // Sizes are metadata on the GC.Alloc samples; without any, only the process-wide counter knows.
            var sized = false;
            foreach (var meta in c.Metadata)
                if (alloc[c.SampleMarker[meta.Sample]] && meta.Kind == 0)
                {
                    sized = true;
                    break;
                }
            if (!sized)
                return null;
            var bytes = new double[scope.Frames.Length];
            for (var i = 0; i < scope.Frames.Length; i++)
                foreach (var b in Blocks.Of(c, new[] { scope.Frames[i] }, scope.Threads))
                {
                    var block = c.Blocks[b];
                    for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                    {
                        if (!alloc[c.SampleMarker[s]])
                            continue;
                        var m = c.FirstMeta(s);
                        if (m >= 0 && c.Metadata[m].Kind == 0)
                            bytes[i] += c.Metadata[m].Bits;
                    }
                }
            return bytes;
        }

        public static int Frames(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "main");
            var sort = args.Get("sort", "ms");
            var top = Scope.Top(args, 20);
            var over = args.GetDouble("over", -1);
            var under = args.GetDouble("under", -1);
            var ascending = args.Flag("asc");
            var columns = args.Get("cols");
            args.RejectUnknown();

            var gcBytes = FindCounter(c, "GC Allocated In Frame");
            var gcSamples = GcPerFrame(c, scope);
            var gcHeader = gcSamples != null ? "gc B" : "gc B (all threads)";
            double Gc(int row, int frame) => gcSamples != null ? gcSamples[row] : gcBytes >= 0 ? c.Counters[gcBytes].Values[frame] : double.NaN;
            var extra = new List<Column>();
            var tables = new Dictionary<string, FrameTable>(StringComparer.OrdinalIgnoreCase);
            if (columns != null)
                foreach (var col in columns.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    extra.Add(ResolveColumn(c, scope, col, tables));

            var rows = Enumerable.Range(0, scope.Frames.Length).ToList();
            if (over >= 0)
                rows = rows.Where(r => c.Frames[scope.Frames[r]].TimeNs / 1e6 > over).ToList();
            if (under >= 0)
                rows = rows.Where(r => c.Frames[scope.Frames[r]].TimeNs / 1e6 < under).ToList();
            Func<int, double> key;
            switch (sort.ToLowerInvariant())
            {
                case "frame":
                    key = r => -r;
                    break;
                case "ms":
                    key = r => c.Frames[scope.Frames[r]].TimeNs;
                    break;
                case "gpu":
                    key = r => c.Frames[scope.Frames[r]].GpuNs;
                    break;
                case "gc":
                    key = r => Gc(r, scope.Frames[r]) is var g && double.IsNaN(g) ? double.MinValue : g;
                    break;
                default:
                    var column = extra.FirstOrDefault(x => string.Equals(x.Name, sort, StringComparison.OrdinalIgnoreCase))
                                 ?? throw new WaitressException($"--sort {sort}: use frame, ms, gpu, gc or a --cols name");
                    key = r => column.Value(r, scope.Frames[r]) is var v && double.IsNaN(v) ? double.MinValue : v;
                    break;
            }
            rows = (ascending ? rows.OrderBy(key) : rows.OrderByDescending(key)).ToList();
            var shown = top > 0 ? rows.Take(top).ToList() : rows;

            var hasGpu = Array.Exists(c.Frames, f => f.GpuNs > 0);
            var header = new List<string> { "frame", "ms" };
            if (hasGpu)
                header.Add("gpu");
            if (gcSamples != null || gcBytes >= 0)
                header.Add(gcHeader);
            header.AddRange(extra.Select(e => Header(e.Name, e.Unit)));
            var t = new Table(header.ToArray());
            foreach (var r in shown)
            {
                var f = scope.Frames[r];
                var cells = new List<string> { c.FrameNumber(f).ToString(), Format.Ms(c.Frames[f].TimeNs) };
                if (hasGpu)
                    cells.Add(c.Frames[f].GpuNs > 0 ? Format.Ms(c.Frames[f].GpuNs) : "-");
                if (gcSamples != null || gcBytes >= 0)
                    cells.Add(Cell(Gc(r, f), 2));
                foreach (var e in extra)
                    cells.Add(Cell(e.Value(r, f), e.Unit));
                t.Add(cells.ToArray());
            }
            var order = sort == "frame" ? (ascending ? "frame, descending" : "frame") : $"{sort}, {(ascending ? "smallest" : "largest")} first";
            w.WriteLine($"# {shown.Count} of {rows.Count} frames by {order}; {scope.Describe(c)} (marker columns sum these threads unless name@threads)");
            t.Write(w);
            return 0;
        }

        /// <summary>
        /// Per thread: work (top-level samples other than Idle, minus waits), wait (outermost wait samples), as ms per
        /// frame. A sample longer than its frame counts as one frame.
        /// </summary>
        public static unsafe int Threads(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "all");
            var each = args.Flag("each");
            args.RejectUnknown();

            var idle = MarkerMatch.Resolve(c, "Idle", false);
            var waits = c.Markers.Select(m => Jobs.IsWait(m.Name)).ToArray();
            var busy = new long[c.Threads.Length];
            var wait = new long[c.Threads.Length];
            var present = new int[c.Threads.Length];
            var samples = new long[c.Threads.Length];
            var top = new Dictionary<int, long>[c.Threads.Length];
            for (var i = 0; i < top.Length; i++)
                top[i] = new Dictionary<int, long>();
            var gate = new object();
            Parallel.For(0, scope.Frames.Length, i =>
            {
                var f = c.Frames[scope.Frames[i]];
                var frameNs = (long)f.TimeNs;
                var local = new List<(int thread, long busy, long wait, long samples)>();
                var localTop = new Dictionary<(int, int), long>();
                for (var b = f.BlockFirst; b < f.BlockFirst + f.BlockCount; b++)
                {
                    var block = c.Blocks[b];
                    if (!scope.HasThread(block.Thread))
                        continue;
                    long busyNs = 0, waitNs = 0;
                    var waitDepth = -1;
                    for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                    {
                        int d = c.SampleDepth[s];
                        if (d == 0)
                            continue;
                        if (waitDepth >= 0 && d <= waitDepth)
                            waitDepth = -1;
                        var m = c.SampleMarker[s];
                        if (d == 1 && idle[m])
                            continue;
                        // A sample can outlast its frame (a semaphore wait spanning seconds); count at most one frame of it.
                        // Not clipped to the frame's window: the render thread's work for a frame runs into the next one.
                        var clipped = Math.Min(c.DurNs(block, s), frameNs);
                        if (d == 1)
                        {
                            busyNs += clipped;
                            if (!c.Markers[m].IsInternal)
                            {
                                localTop.TryGetValue((block.Thread, m), out var v);
                                localTop[(block.Thread, m)] = v + clipped;
                            }
                        }
                        if (waitDepth < 0 && waits[m])
                        {
                            waitNs += clipped;
                            waitDepth = d;
                        }
                    }
                    local.Add((block.Thread, busyNs, waitNs, block.SampleCount));
                }
                lock (gate)
                {
                    foreach (var (thread, b, wt, n) in local)
                    {
                        busy[thread] += b;
                        wait[thread] += wt;
                        samples[thread] += n;
                        present[thread]++;
                    }
                    foreach (var kv in localTop)
                    {
                        top[kv.Key.Item1].TryGetValue(kv.Key.Item2, out var v);
                        top[kv.Key.Item1][kv.Key.Item2] = v + kv.Value;
                    }
                }
            });

            var frames = scope.Frames.Length;
            var frameAvg = scope.Frames.Average(f => (double)c.Frames[f].TimeNs);
            w.WriteLine($"# {scope.Describe(c)}; ms per frame: work = top-level samples other than Idle and waits, wait = time in wait samples " +
                        $"(*WaitFor*, Semaphore.Wait*...); util = work / avg frame time ({Format.Ms(frameAvg)} ms), per thread");
            var t = new Table("thread", "work", "wait", "util", "samples/f", "top samples (share of busy time)");
            string Top(IEnumerable<int> threads)
            {
                var merged = new Dictionary<int, long>();
                foreach (var th in threads)
                    foreach (var kv in top[th])
                    {
                        merged.TryGetValue(kv.Key, out var v);
                        merged[kv.Key] = v + kv.Value;
                    }
                var total = merged.Values.Sum();
                return string.Join(", ", merged.OrderByDescending(kv => kv.Value).Take(3)
                    .Select(kv => $"{c.Markers[kv.Key].Name} {Format.Pct(total > 0 ? (double)kv.Value / total : 0)}"));
            }
            var used = Enumerable.Range(0, c.Threads.Length).Where(i => scope.HasThread(i) && present[i] > 0).ToList();
            var groups = used
                .GroupBy(i => each ? i.ToString() : c.Threads[i].Kind is ThreadKind.Worker or ThreadKind.Background ? c.Threads[i].Group + "/*" : c.Threads[i].Display)
                .OrderBy(g => g.Min(i => (int)c.Threads[i].Kind)).ThenByDescending(g => g.Sum(i => busy[i]));
            var hidden = 0;
            foreach (var g in groups)
            {
                var ids = g.ToList();
                var name = each ? c.Threads[ids[0]].Display : g.Key;
                if (each && used.Count(i => c.Threads[i].Display == name) > 1)
                    name += " #" + (used.Where(i => c.Threads[i].Display == name).ToList().IndexOf(ids[0]) + 1);
                if (ids.Count > 1)
                    name += $" ({ids.Count} threads)";
                var b = ids.Sum(i => busy[i]) / (double)frames;
                var wt = ids.Sum(i => wait[i]) / (double)frames;
                if (b == 0 && !each && c.Threads[ids[0]].Kind == ThreadKind.Other)
                {
                    hidden++;
                    continue;
                }
                var work = Math.Max(0, b - wt);
                t.Add(name, Format.Ms(work), Format.Ms(wt), Format.Pct(work / frameAvg / ids.Count), Format.PerFrame(ids.Sum(i => samples[i]) / (double)frames), Top(ids));
            }
            t.Write(w);
            if (!each)
                w.WriteLine($"# threads sharing a name are summed (util is per thread); --each lists every thread{(hidden > 0 ? $"; {hidden} idle threads hidden" : "")}");
            return 0;
        }


        /// <summary>Counters by comma-separated terms: exact names, globs, or text the name contains.</summary>
        static bool[] CounterMatch(CaptureFile c, string pattern)
        {
            var hits = new bool[c.Markers.Length];
            var counters = c.Counters.Select(x => x.Marker).ToList();
            foreach (var term in pattern.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                Func<string, bool> match = term.Contains('*') || term.Contains('?')
                    ? Glob.Compile(term)
                    : counters.Any(m => string.Equals(c.Markers[m].Name, term, StringComparison.OrdinalIgnoreCase))
                        ? name => string.Equals(name, term, StringComparison.OrdinalIgnoreCase)
                        : name => name.Contains(term, StringComparison.OrdinalIgnoreCase);
                var any = false;
                foreach (var m in counters)
                    if (match(c.Markers[m].Name))
                        any = hits[m] = true;
                if (!any)
                    throw new WaitressException($"no counter matches '{term}' in this capture; `counters <cap> --all` lists every counter (separate several terms with commas)");
            }
            return hits;
        }

        public static int Counters(CaptureFile c, Args args, TextWriter w)
        {
            var pattern = args.Positional.Count > 1 ? args.Positional[1] : null;
            var scope = Scope.Parse(c, args, "main");
            var all = args.Flag("all");
            args.RejectUnknown();
            var hits = pattern == null ? null : CounterMatch(c, pattern);
            var t = new Table("counter", "category", "avg", "min", "max", "first", "last");
            var hidden = 0;
            foreach (var counter in c.Counters.OrderBy(x => c.Markers[x.Marker].Category).ThenBy(x => c.Markers[x.Marker].Name))
            {
                var m = c.Markers[counter.Marker];
                if (hits != null && !hits[counter.Marker])
                    continue;
                var values = scope.Frames.Select(f => counter.Values[f]).Where(v => !double.IsNaN(v)).ToArray();
                if (values.Length == 0 || (!all && hits == null && values.All(v => v == 0)))
                {
                    hidden++;
                    continue;
                }
                t.Add(m.Name, m.Category, Format.Unit(values.Average(), counter.Unit), Format.Unit(values.Min(), counter.Unit),
                      Format.Unit(values.Max(), counter.Unit), Format.Unit(values[0], counter.Unit), Format.Unit(values[^1], counter.Unit));
            }
            w.WriteLine($"# {scope.Frames.Length} frames; avg, min and max over frames with a value");
            t.Write(w);
            if (hidden > 0)
                w.WriteLine($"# {hidden} counters always zero or absent are hidden (--all shows them); per-frame values: frames --cols \"<counter>\"");
            return 0;
        }
    }
}
