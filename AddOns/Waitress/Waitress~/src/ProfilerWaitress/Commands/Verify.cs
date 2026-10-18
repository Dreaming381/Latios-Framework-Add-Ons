using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;
using ProfilerWaitress.Text;

namespace ProfilerWaitress.Commands
{
    /// <summary>
    /// Compares the cache against Unity's own Hierarchy view (HierarchyFrameDataView) for a few frames on every
    /// thread: each path's total, self, calls and GC bytes. Guards against the cache drifting from what the
    /// Profiler window shows, e.g. after a Unity upgrade.
    /// </summary>
    public static class Verify
    {
        struct Item
        {
            public double Total;
            public double Self;
            public double Calls;
            public double Gc;
        }

        public static unsafe int Run(CaptureFile c, Args args, TextWriter w)
        {
            var framesText = args.Get("frames");
            var tolerance = args.GetDouble("tolerance", 0.0005);
            var show = Scope.Top(args, 10);
            args.RejectUnknown();
            int[] frames;
            if (framesText != null)
                frames = Scope.ParseFrames(c, framesText);
            else
            {
                var byTime = Enumerable.Range(0, c.FrameCount).OrderBy(f => c.Frames[f].TimeNs).ToArray();
                frames = new[] { 0, byTime[byTime.Length / 2], byTime[^1], c.FrameCount - 1 }.Distinct().OrderBy(x => x).ToArray();
            }
            if (frames.Length > 20)
                throw new WaitressException("verify compares at most 20 frames per run");

            var root = CaptureStore.ProjectRoot(c.SourcePath);
            var tsv = Path.Combine(root, "UserSettings", "Waitress", "ProfilerWaitress", "verify.tsv");
            var reply = EditorScript.Call(root, "Hierarchy", c.SourcePath, tsv.Replace('\\', '/'), string.Join(",", frames.Select(f => c.ProfilerIndex(f))));
            var unity = new Dictionary<string, Item>(StringComparer.Ordinal);
            foreach (var line in File.ReadLines(tsv))
            {
                var p = line.Split('\t');
                // Distinct markers can share a name, so Unity can list one name path more than once.
                var key = p[0] + "\t" + p[1] + "\t" + p[3];
                unity.TryGetValue(key, out var item);
                item.Total += double.Parse(p[4], CultureInfo.InvariantCulture);
                item.Self += double.Parse(p[5], CultureInfo.InvariantCulture);
                item.Calls += double.Parse(p[6], CultureInfo.InvariantCulture);
                item.Gc += double.Parse(p[7], CultureInfo.InvariantCulture);
                unity[key] = item;
            }

            var gcMarker = MarkerMatch.Resolve(c, "GC.Alloc", false);
            var ours = new Dictionary<string, Item>(StringComparer.Ordinal);
            foreach (var f in frames)
            {
                var frame = c.Frames[f];
                for (var b = frame.BlockFirst; b < frame.BlockFirst + frame.BlockCount; b++)
                {
                    var block = c.Blocks[b];
                    var prefix = c.ProfilerIndex(f) + "\t" + c.Threads[block.Thread].Id + "\t";
                    var paths = new string[64];
                    var keys = new string[64];
                    for (var s = block.SampleBase; s < block.SampleBase + block.SampleCount; s++)
                    {
                        int d = c.SampleDepth[s];
                        if (d == 0)
                            continue;
                        if (d >= paths.Length)
                        {
                            Array.Resize(ref paths, d * 2);
                            Array.Resize(ref keys, d * 2);
                        }
                        // The Hierarchy view leaves out internal markers with their subtrees, but still takes their
                        // time out of the parent's self time.
                        if (c.Markers[c.SampleMarker[s]].IsInternal)
                        {
                            if (keys[d - 1] != null)
                            {
                                var owner = ours[keys[d - 1]];
                                owner.Self -= c.DurNs(block, s) / 1e6;
                                ours[keys[d - 1]] = owner;
                            }
                            // Allocations inside the hidden subtree still count toward the visible ancestors' GC.
                            long hiddenGc = 0;
                            for (var h = s; h == s || (h < block.SampleBase + block.SampleCount && c.SampleDepth[h] > d); h++)
                            {
                                if (!gcMarker[c.SampleMarker[h]])
                                    continue;
                                var i = c.FirstMeta(h);
                                hiddenGc += i >= 0 && c.Metadata[i].Kind == 0 ? c.Metadata[i].Bits : 0;
                            }
                            for (var a = 1; a < d && hiddenGc > 0; a++)
                            {
                                if (keys[a] == null || (a > 1 && keys[a] == keys[a - 1]))
                                    continue;
                                var it = ours[keys[a]];
                                it.Gc += hiddenGc;
                                ours[keys[a]] = it;
                            }
                            while (s + 1 < block.SampleBase + block.SampleCount && c.SampleDepth[s + 1] > d)
                                s++;
                            continue;
                        }
                        var name = c.MarkerName(s);
                        paths[d] = paths[d - 1] == null ? name : paths[d - 1] + "\u001f" + name;
                        keys[d] = prefix + paths[d];
                        var ms = c.DurNs(block, s) / 1e6;
                        ours.TryGetValue(keys[d], out var item);
                        item.Total += ms;
                        item.Self += ms;
                        item.Calls++;
                        ours[keys[d]] = item;
                        if (keys[d - 1] != null)
                        {
                            var parent = ours[keys[d - 1]];
                            parent.Self -= ms;
                            ours[keys[d - 1]] = parent;
                        }
                        if (gcMarker[c.SampleMarker[s]])
                        {
                            var i = c.FirstMeta(s);
                            var size = i >= 0 && c.Metadata[i].Kind == 0 ? c.Metadata[i].Bits : 0;
                            for (var a = 1; a <= d; a++)
                            {
                                if (keys[a] == null || (a > 1 && keys[a] == keys[a - 1]))
                                    continue;
                                var it = ours[keys[a]];
                                it.Gc += size;
                                ours[keys[a]] = it;
                            }
                        }
                    }
                }
            }

            var mismatches = new List<string>();
            var compared = 0;
            double worst = 0;
            foreach (var kv in unity)
            {
                compared++;
                if (!ours.TryGetValue(kv.Key, out var mine))
                {
                    mismatches.Add($"missing  {Describe(c, kv.Key)}  (Unity: total {kv.Value.Total:0.####} ms, {kv.Value.Calls} calls)");
                    continue;
                }
                var u = kv.Value;
                var dt = Math.Abs(u.Total - mine.Total);
                var ds = Math.Abs(Math.Max(0, u.Self) - Math.Max(0, mine.Self));
                worst = Math.Max(worst, Math.Max(dt, ds));
                var limit = tolerance + 1e-5 * u.Total;
                if (dt > limit || ds > limit || u.Calls != mine.Calls || Math.Abs(u.Gc - mine.Gc) > 0.5)
                    mismatches.Add($"differs  {Describe(c, kv.Key)}  total {mine.Total:0.####}/{u.Total:0.####}  self {mine.Self:0.####}/{u.Self:0.####}  " +
                                   $"calls {mine.Calls}/{u.Calls}  gc {mine.Gc}/{u.Gc}  (ours/Unity)");
            }
            foreach (var key in ours.Keys)
                if (!unity.ContainsKey(key))
                    mismatches.Add($"extra    {Describe(c, key)}");

            w.WriteLine($"compared {compared} hierarchy items over frames {string.Join(",", frames.Select(f => c.FrameNumber(f)))} on every thread " +
                        $"({reply.GetProperty("items").GetInt32()} rows from Unity, which lists same-named markers separately); largest time difference {worst * 1000:0.###} us");
            foreach (var m in mismatches.Take(show))
                w.WriteLine("  " + m);
            if (mismatches.Count > show)
                w.WriteLine($"  ... {mismatches.Count - show} more");
            w.WriteLine(mismatches.Count == 0 ? "OK: the cache matches the Profiler's Hierarchy view" : $"FAILED: {mismatches.Count} differences");
            return mismatches.Count == 0 ? 0 : 1;
        }

        static string Describe(CaptureFile c, string key)
        {
            var p = key.Split('\t');
            var frame = int.Parse(p[0], CultureInfo.InvariantCulture) - c.FirstFrame + 1;
            var thread = c.Threads.FirstOrDefault(t => t.Id.ToString(CultureInfo.InvariantCulture) == p[1]).Display ?? "thread " + p[1];
            return $"frame {frame} {thread}: {p[2].Replace('\u001f', '>')}";
        }
    }
}
