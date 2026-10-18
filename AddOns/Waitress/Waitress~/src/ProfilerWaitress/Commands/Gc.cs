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
    public static class Gc
    {
        /// <summary>
        /// Managed allocations grouped by where they happened: the callstack when the capture recorded one,
        /// otherwise the marker the GC.Alloc sample sits in. Also GC.Collect time.
        /// </summary>
        public static unsafe int Run(CaptureFile c, Args args, TextWriter w)
        {
            var scope = Scope.Parse(c, args, "all");
            var top = Scope.Top(args, 15);
            var depth = args.GetInt("stack", 4);
            var pattern = args.Get("marker") ?? "GC.Alloc";
            var by = args.Get("by", c.HasCallstacks ? "stack" : "marker");
            var fromText = args.Get("from");
            args.RejectUnknown();
            if (by != "stack" && by != "marker")
                throw new WaitressException("--by takes stack or marker");
            var from = fromText != null ? Glob.Compile(fromText.Contains('*') ? fromText : "*" + fromText + "*") : null;
            long skipped = 0;
            var allocs = MarkerMatch.Resolve(c, pattern, false);
            var collect = MarkerMatch.Resolve(c, "GC.Collect", false);

            var sites = new Dictionary<string, (long bytes, long count, long max, HashSet<int> frames)>();
            long totalBytes = 0, totalCount = 0, collectNs = 0, collects = 0, unsized = 0;
            var framesWithAlloc = new HashSet<int>();
            var collectFrames = new List<(int frame, long ns)>();
            var anc = new Ancestry(c);
            foreach (var b in Blocks.Of(c, scope.Frames, scope.Threads))
            {
                var block = c.Blocks[b];
                anc.Walk(b, (s, d) =>
                {
                    var m = c.SampleMarker[s];
                    if (collect[m])
                    {
                        var ns = c.DurNs(block, s);
                        collectNs += ns;
                        collects++;
                        collectFrames.Add((block.Frame, ns));
                        return;
                    }
                    if (!allocs[m])
                        return;
                    var size = Size(c, s);
                    if (size < 0)
                    {
                        unsized++;
                        size = 0;
                    }
                    var stack = by == "stack" ? c.Callstack(s) : null;
                    string site;
                    if (stack != null)
                    {
                        var frames = stack.Select(f => c.Strings[f]).Where(f => f.Length > 0).Select(Method);
                        if (from != null)
                        {
                            frames = frames.SkipWhile(f => !from(f));
                            if (!frames.Any())
                            {
                                skipped++;
                                return;
                            }
                        }
                        site = string.Join(" < ", frames.Take(depth));
                    }
                    else
                    {
                        site = anc.Path(3);
                        if (c.Threads[block.Thread].Kind != ThreadKind.Main)
                            site = c.Threads[block.Thread].Display + ": " + site;
                    }
                    sites.TryGetValue(site, out var v);
                    v.frames ??= new HashSet<int>();
                    v.bytes += size;
                    v.count++;
                    v.max = Math.Max(v.max, size);
                    v.frames.Add(block.Frame);
                    sites[site] = v;
                    totalBytes += size;
                    totalCount++;
                    framesWithAlloc.Add(block.Frame);
                });
            }

            var n = (double)scope.Frames.Length;
            w.WriteLine($"# {scope.Describe(c)}: {Format.Bytes(totalBytes / n)}/frame in {Format.PerFrame(totalCount / n)} allocations/frame, " +
                        $"{framesWithAlloc.Count}/{scope.Frames.Length} frames allocate" + (c.HasCallstacks ? "" : "; no callstacks recorded (record --callstacks gc), sites are parent markers"));
            if (collects > 0)
            {
                var worst = collectFrames.OrderByDescending(x => x.ns).Take(4).Select(x => $"{c.FrameNumber(x.frame)} ({Format.Ms(x.ns)})");
                w.WriteLine($"# GC.Collect: {collects} times, {Format.Ms(collectNs / n)} ms/frame; worst frames {string.Join(", ", worst)}");
            }
            if (skipped > 0)
                w.WriteLine($"# {skipped} allocations have no frame matching --from {fromText} and are left out");
            if (unsized > 0)
                w.WriteLine($"# {unsized} allocations carried no size");
            if (sites.Count == 0)
                return 0;
            var t = new Table("bytes/f", "allocs/f", "largest", "frames", "site");
            foreach (var kv in sites.OrderByDescending(kv => kv.Value.bytes).ThenByDescending(kv => kv.Value.count).Take(top > 0 ? top : int.MaxValue))
                t.Add(Format.Bytes(kv.Value.bytes / n), Format.PerFrame(kv.Value.count / n), Format.Bytes(kv.Value.max), kv.Value.frames.Count.ToString(), kv.Key);
            t.Write(w);
            if (top > 0 && sites.Count > top)
                w.WriteLine($"# {sites.Count - top} more sites (--top 0)");
            return 0;
        }

        /// <summary>"Assembly.dll!Namespace::Type.Method()" without the assembly.</summary>
        public static string Method(string frame)
        {
            var bang = frame.IndexOf('!');
            return bang >= 0 ? frame.Substring(bang + 1) : frame;
        }

        /// <summary>The allocation size a GC.Alloc sample carries as metadata, or -1.</summary>
        static long Size(CaptureFile c, long sample)
        {
            var i = c.FirstMeta(sample);
            if (i < 0)
                return -1;
            var meta = c.Metadata;
            for (; i < meta.Length && meta[i].Sample == sample; i++)
                if (meta[i].Kind == 0)
                    return meta[i].Bits;
            return -1;
        }
    }
}
