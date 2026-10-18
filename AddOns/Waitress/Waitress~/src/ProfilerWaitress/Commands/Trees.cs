using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProfilerWaitress.Analysis;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;
using ProfilerWaitress.Text;

namespace ProfilerWaitress.Commands
{
    public static class Trees
    {
        /// <summary>
        /// The frame named by the second positional argument: a number, or slowest, median, fastest.
        /// Falls back to --frames, then to every frame.
        /// </summary>
        public static Scope FrameScope(CaptureFile c, Args args, string defaultThreads, bool requireSingle)
        {
            var scope = Scope.Parse(c, args, defaultThreads);
            if (args.Positional.Count < 2)
            {
                if (requireSingle && scope.Frames.Length != 1)
                    throw new WaitressException("name a frame: a number, slowest, median or fastest");
                return scope;
            }
            var which = args.Positional[1].ToLowerInvariant();
            var pool = scope.Frames.OrderBy(f => c.Frames[f].TimeNs).ToArray();
            int frame = which switch
            {
                "slowest" or "worst" or "max" => pool[^1],
                "fastest" or "min" => pool[0],
                "median" or "p50" => pool[pool.Length / 2],
                _ => Scope.ParseFrames(c, which)[0],
            };
            scope.Frames = new[] { frame };
            return scope;
        }

        sealed class Node
        {
            public int Marker;
            public long Incl;
            public long Self;
            public long Calls;
            public Dictionary<int, Node> Children;
        }

        public static unsafe int Tree(CaptureFile c, Args args, TextWriter w)
        {
            var scope = FrameScope(c, args, "main", false);
            var minMs = args.GetDouble("min", 0.05);
            var maxDepth = args.GetInt("depth", 12);
            var rootPattern = args.Get("root");
            var each = args.Flag("each");
            var expand = args.Flag("no-collapse");
            args.RejectUnknown();
            if (maxDepth < 1 || minMs < 0)
                throw new WaitressException("--depth must be at least 1 and --min can't be negative");
            var rootHits = rootPattern != null ? MarkerMatch.Resolve(c, rootPattern) : null;
            var n = (double)scope.Frames.Length;

            var threadGroups = each
                ? Enumerable.Range(0, c.Threads.Length).Where(scope.HasThread).Select(t => new[] { t }).ToList()
                : new List<int[]> { Enumerable.Range(0, c.Threads.Length).Where(scope.HasThread).ToArray() };
            var title = scope.Frames.Length == 1
                ? $"frame {c.FrameNumber(scope.Frames[0])} ({Format.Ms(c.Frames[scope.Frames[0]].TimeNs)} ms)"
                : $"avg per frame over {scope.Describe(c).Split(',')[0]}";
            w.WriteLine($"# {title}; incl self calls name (on an 'A > B' line, self is B's); children under {minMs} ms hidden, depth <= {maxDepth}" +
                        (rootPattern != null ? $"; rooted at '{rootPattern}'" : ""));

            foreach (var threads in threadGroups)
            {
                var root = new Node { Marker = -1 };
                var selected = new bool[c.Threads.Length];
                foreach (var t in threads)
                    selected[t] = true;
                var stack = new Node[64];
                foreach (var b in Blocks.Of(c, scope.Frames, selected))
                {
                    var block = c.Blocks[b];
                    var rootDepth = -1;
                    for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                    {
                        int d = c.SampleDepth[s];
                        if (d + 1 >= stack.Length)
                            Array.Resize(ref stack, stack.Length * 2);
                        var dur = c.DurNs(block, s);
                        if (d == 0)
                        {
                            stack[0] = rootHits == null ? root : null;
                            continue;
                        }
                        if (rootDepth >= 0 && d <= rootDepth)
                            rootDepth = -1;
                        var m = c.SampleMarker[s];
                        if (!scope.ShowInternal && c.Markers[m].IsInternal)
                        {
                            // Hidden with its subtree, as the Profiler window does; its time still leaves the parent's self.
                            var owner = rootHits == null || rootDepth >= 0 ? stack[d - 1] : null;
                            if (owner != null && owner != root)
                                owner.Self -= dur;
                            while (s + 1 < block.SampleBase + block.SampleCount && c.SampleDepth[s + 1] > d)
                                s++;
                            continue;
                        }
                        Node parent;
                        if (rootHits == null)
                            parent = stack[d - 1];
                        else if (rootDepth < 0)
                        {
                            if (!rootHits[m])
                                continue;
                            rootDepth = d;
                            parent = root;
                        }
                        else
                            parent = stack[d - 1];
                        if (parent == null)
                            continue;
                        parent.Children ??= new Dictionary<int, Node>();
                        if (!parent.Children.TryGetValue(m, out var node))
                            parent.Children[m] = node = new Node { Marker = m };
                        node.Incl += dur;
                        node.Self += dur;
                        node.Calls++;
                        if (parent != root)
                            parent.Self -= dur;
                        stack[d] = node;
                    }
                }
                if (threadGroups.Count > 1 || threads.Length > 1)
                    w.WriteLine(threads.Length == 1 ? $"[{c.Threads[threads[0]].Display}]" : $"[{threads.Length} threads merged: {DescribeThreads(c, threads)}]");
                else
                    w.WriteLine($"[{c.Threads[threads[0]].Display}]");
                if (root.Children == null)
                {
                    w.WriteLine("  (nothing)");
                    continue;
                }
                var sb = new StringBuilder();
                Print(c, root, 0, maxDepth, (long)(minMs * 1e6 * n), n, !expand, sb);
                w.Write(sb.ToString());
            }
            return 0;
        }

        static string DescribeThreads(CaptureFile c, int[] threads)
        {
            var groups = threads.GroupBy(t => c.Threads[t].Kind == ThreadKind.Worker || c.Threads[t].Kind == ThreadKind.Background
                ? c.Threads[t].Group + "/*" : c.Threads[t].Display);
            return string.Join(", ", groups.Select(g => g.Count() > 1 ? $"{g.Key} ({g.Count()})" : g.Key));
        }

        static void Print(CaptureFile c, Node node, int depth, int maxDepth, long minNs, double n, bool collapse, StringBuilder sb)
        {
            if (node.Children == null)
                return;
            var kids = node.Children.Values.OrderByDescending(k => k.Incl).ToList();
            long hiddenNs = 0;
            var hidden = 0;
            if (depth >= maxDepth)
            {
                sb.Append(' ', depth * 2 + 2).Append("... ").Append(kids.Count).Append(kids.Count == 1 ? " child" : " children").Append(" below --depth ").Append(maxDepth).Append('\n');
                return;
            }
            foreach (var k in kids)
            {
                if (k.Incl < minNs)
                {
                    hiddenNs += k.Incl;
                    hidden++;
                    continue;
                }
                var name = new StringBuilder(c.Markers[k.Marker].Name);
                var tail = k;
                // A wrapper whose one child does all its work prints on one line.
                while (collapse && tail.Children != null && tail.Children.Count == 1)
                {
                    var only = tail.Children.Values.First();
                    if (only.Calls != tail.Calls || only.Incl < tail.Incl * 0.98)
                        break;
                    name.Append(" > ").Append(c.Markers[only.Marker].Name);
                    tail = only;
                }
                sb.Append(' ', depth * 2 + 2)
                  .Append(Format.Ms(k.Incl / n)).Append(' ')
                  .Append(Format.Ms(tail.Self / n)).Append(' ')
                  .Append(n == 1 ? k.Calls.ToString() : Format.PerFrame(k.Calls / n)).Append(' ')
                  .Append(name).Append('\n');
                Print(c, tail, depth + 1, maxDepth, minNs, n, collapse, sb);
            }
            if (hidden > 0 && hiddenNs / n >= 1000)
                sb.Append(' ', depth * 2 + 2).Append(Format.Ms(hiddenNs / n)).Append(" ... ").Append(hidden).Append(" under --min\n");
        }

        public static unsafe int Timeline(CaptureFile c, Args args, TextWriter w)
        {
            var scope = FrameScope(c, args, "main", true);
            var minMs = args.GetDouble("min", 0.05);
            var from = args.GetDouble("from", double.NegativeInfinity);
            var to = args.GetDouble("to", double.PositiveInfinity);
            var window = args.Has("from") || args.Has("to");
            // A narrow window is for looking deep, so it shows deep samples by default.
            var maxDepth = args.GetInt("depth", window ? 40 : 6);
            var meta = args.Flag("meta");
            var flows = args.Flag("flows");
            args.RejectUnknown();
            if (from > to)
                throw new WaitressException("--from is after --to");
            if (maxDepth < 1 || minMs < 0)
                throw new WaitressException("--depth must be at least 1 and --min can't be negative");
            var frame = scope.Frames[0];
            var minNs = (long)(minMs * 1e6);
            var idle = MarkerMatch.Resolve(c, "Idle", false);
            w.WriteLine($"# frame {c.FrameNumber(frame)} ({Format.Ms(c.Frames[frame].TimeNs)} ms); start ms (from frame start), ms, name; " +
                        (window ? $"samples overlapping {(double.IsNegativeInfinity(from) ? "start" : from + " ms")} to {(double.IsPositiveInfinity(to) ? "end" : to + " ms")}; " : "") +
                        $"samples under {minMs} ms or deeper than {maxDepth} hidden");
            var f = c.Frames[frame];
            for (var b = f.BlockFirst; b < f.BlockFirst + f.BlockCount; b++)
            {
                var block = c.Blocks[b];
                if (!scope.HasThread(block.Thread))
                    continue;
                var t = new Table();
                for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                {
                    int d = c.SampleDepth[s];
                    if (d > 0 && !scope.ShowInternal && c.Markers[c.SampleMarker[s]].IsInternal)
                    {
                        while (s + 1 < block.SampleBase + block.SampleCount && c.SampleDepth[s + 1] > d)
                            s++;
                        continue;
                    }
                    if (d == 0 || idle[c.SampleMarker[s]])
                        continue;
                    var dur = c.DurNs(block, s);
                    var start = c.StartNs(block, s);
                    if (start / 1e6 + dur / 1e6 < from || start / 1e6 > to)
                        continue;
                    if (dur < minNs || d > maxDepth)
                        continue;
                    t.Add(Format.Ms(start), Format.Ms(dur), new string(' ', (d - 1) * 2) + c.MarkerName(s) + (meta ? Markers.MetaSummary(c, s) : "") + (flows ? FlowSummary(c, s) : ""));
                }
                if (t.Count == 0)
                    continue;
                w.WriteLine($"[{c.Threads[block.Thread].Display}]");
                t.Write(w, "  ");
            }
            return 0;
        }

        static readonly string[] s_FlowTypes = { "begin", "parallel", "end", "next" };

        /// <summary>A sample's flow events, e.g. " [begin 812, end 790]".</summary>
        static long[] s_FlowKeys;
        static int[] s_FlowOrder;

        static string FlowSummary(CaptureFile c, long s)
        {
            c.LoadFlows(out var sample, out var id, out var type);
            if (s_FlowKeys == null)
            {
                s_FlowKeys = (long[])sample.Clone();
                s_FlowOrder = Enumerable.Range(0, sample.Length).ToArray();
                Array.Sort(s_FlowKeys, s_FlowOrder);
            }
            int lo = 0, hi = s_FlowKeys.Length;
            while (lo < hi)
            {
                var mid = (lo + hi) >> 1;
                if (s_FlowKeys[mid] < s)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            var parts = new List<string>();
            for (var k = lo; k < s_FlowKeys.Length && s_FlowKeys[k] == s; k++)
            {
                var i = s_FlowOrder[k];
                parts.Add((type[i] < s_FlowTypes.Length ? s_FlowTypes[type[i]] : "type" + type[i]) + " " + id[i]);
            }
            return parts.Count == 0 ? "" : " [" + string.Join(", ", parts) + "]";
        }
    }
}
