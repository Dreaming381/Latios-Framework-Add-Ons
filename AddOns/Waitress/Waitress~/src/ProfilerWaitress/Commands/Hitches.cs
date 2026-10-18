using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProfilerWaitress.Analysis;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;
using ProfilerWaitress.Text;

namespace ProfilerWaitress.Commands
{
    public static class Hitches
    {
        /// <summary>
        /// Slow frames, each explained by the markers whose self time grew the most against their own median in
        /// scope, with the path each sits in. A context line puts GPU time and render-thread work next to the frame
        /// time, because a main thread that only waited longer is bound somewhere else.
        /// </summary>
        public static int Run(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "main");
            var top = Scope.Top(args, 8);
            var over = args.GetDouble("over", -1);
            var factor = args.GetDouble("factor", 1.5);
            var lines = args.GetInt("lines", 4);
            if (args.Has("over") && over < 0)
                throw new WaitressException("--over takes a frame time in ms");
            if (factor <= 1)
                throw new WaitressException("--factor must be above 1 (a multiple of the median frame time)");
            args.RejectUnknown();

            var table = FrameTable.Build(c, scope);
            var n = scope.Frames.Length;
            var frameNs = scope.Frames.Select(f => (double)c.Frames[f].TimeNs).ToArray();
            var median = Stats.Median(frameNs);
            var threshold = over >= 0 ? over * 1e6 : median * factor;
            var slow = Enumerable.Range(0, n).Where(i => frameNs[i] > threshold).OrderByDescending(i => frameNs[i]).ToList();
            w.WriteLine($"# {slow.Count} of {n} frames over {Format.Ms(threshold)} ms (median {Format.Ms(median)}); {scope.Describe(c)}");
            if (slow.Count == 0)
            {
                slow = Enumerable.Range(0, n).OrderByDescending(i => frameNs[i]).Take(Math.Min(top, 5)).ToList();
                w.WriteLine($"# no frame is a hitch by that measure; explaining the {slow.Count} slowest instead (--over MS or --factor F to change the threshold)");
            }
            w.WriteLine("# each line: self-time growth over the marker's median, the marker, and where its longest sample sat");

            // Medians per marker over the frames in scope, absent frames counting as zero.
            var mCount = c.Markers.Length;
            var medSelf = new double[mCount];
            var columns = new List<long>[mCount];
            for (var i = 0; i < n; i++)
                foreach (var r in table.Rows[i])
                    (columns[r.Marker] ??= new List<long>()).Add(r.Self);
            for (var m = 0; m < mCount; m++)
                medSelf[m] = MedianWithZeros(columns[m], n);

            var gpu = scope.Frames.Select(f => (double)c.Frames[f].GpuNs).ToArray();
            var gpuMedian = Stats.Median(gpu);
            var render = RenderWork(c, scope.Frames);
            var renderMedian = render != null ? Stats.Median(render) : 0;
            var waits = c.Markers.Select(m => Jobs.IsWait(m.Name) || m.Name == "Idle").ToArray();

            foreach (var i in slow.Take(top))
            {
                var f = scope.Frames[i];
                var context = $"frame {c.FrameNumber(f)}  {Format.Ms(frameNs[i])} ms ({Format.SignedMs(frameNs[i] - median)})";
                if (gpu[i] > 0)
                    context += $"  gpu {Format.Ms(gpu[i])} ({Format.SignedMs(gpu[i] - gpuMedian)})";
                if (render != null)
                    context += $"  render-thread work {Format.Ms(render[i])} ({Format.SignedMs(render[i] - renderMedian)})";
                w.WriteLine(context);
                var what = table.Rows[i].Select(r => (r.Marker, delta: r.Self - medSelf[r.Marker]))
                    .Where(x => x.delta > 0.05e6).OrderByDescending(x => x.delta).Take(lines).ToList();
                if (what.Count == 0)
                {
                    w.WriteLine("  no marker's self time grew by 0.05 ms on these threads; try --thread render, or `timeline`");
                    continue;
                }
                var paths = PathsIn(c, f, scope.Threads, what.Select(x => x.Marker).ToHashSet());
                foreach (var x in what)
                    w.WriteLine($"  {Format.SignedMs(x.delta)}  {c.Markers[x.Marker].Name}{(waits[x.Marker] ? " (waiting)" : "")}" +
                                (paths.TryGetValue(x.Marker, out var p) ? $"  in {p}" : ""));
            }
            if (slow.Count > top)
                w.WriteLine($"# {slow.Count - top} more slow frames (--top N); for recurring causes compare `top --frames <slow frames>` with the rest, or `diff`");
            return 0;
        }

        static double MedianWithZeros(List<long> values, int n)
        {
            if (values == null || values.Count * 2 < n)
                return 0;
            var all = new double[n];
            for (var i = 0; i < values.Count; i++)
                all[i] = values[i];
            Array.Sort(all);
            return Stats.Percentile(all, 0.5);
        }

        /// <summary>Render-thread time in top-level samples that are neither waits nor Idle, per frame; null without a render thread.</summary>
        static unsafe double[] RenderWork(CaptureFile c, int[] frames)
        {
            var renderThreads = c.Threads.Select(t => t.Kind == ThreadKind.Render).ToArray();
            if (!renderThreads.Any(x => x))
                return null;
            var skip = c.Markers.Select(m => Jobs.IsWait(m.Name) || m.Name == "Idle").ToArray();
            var work = new double[frames.Length];
            for (var i = 0; i < frames.Length; i++)
                foreach (var b in Blocks.Of(c, new[] { frames[i] }, renderThreads))
                {
                    var block = c.Blocks[b];
                    for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                        if (c.SampleDepth[s] == 1 && !skip[c.SampleMarker[s]])
                            work[i] += c.DurNs(block, s);
                }
            return work;
        }

        /// <summary>For each marker, the parent path of its longest sample in a frame.</summary>
        static Dictionary<int, string> PathsIn(CaptureFile c, int frame, bool[] threads, HashSet<int> markers)
        {
            var best = new Dictionary<int, (long dur, string path)>();
            var anc = new Ancestry(c);
            var several = threads.Count(x => x) > 1;
            foreach (var b in Blocks.Of(c, new[] { frame }, threads))
            {
                var block = c.Blocks[b];
                anc.Walk(b, (s, d) =>
                {
                    if (d == 0)
                        return;
                    unsafe
                    {
                        var m = c.SampleMarker[s];
                        if (!markers.Contains(m))
                            return;
                        var dur = c.DurNs(block, s);
                        if (best.TryGetValue(m, out var prev) && prev.dur >= dur)
                            return;
                        var path = anc.Path(3);
                        if (several)
                            path = c.Threads[block.Thread].Display + ": " + path;
                        best[m] = (dur, path);
                    }
                });
            }
            return best.ToDictionary(kv => kv.Key, kv => kv.Value.path);
        }
    }
}
