using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProfilerWaitress.Analysis;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Testing
{
    /// <summary>
    /// Checks the reader and the analysis semantics on small synthetic captures written in the cache format.
    /// The Editor-side check is `verify`, which compares a real capture against Unity's Hierarchy view.
    /// </summary>
    public static class SelfTest
    {
        public static int Run(TextWriter w)
        {
            var tests = new (string name, Action test)[]
            {
                ("reader round trip", ReaderRoundTrip),
                ("recursion counts once in incl", Recursion),
                ("self excludes children", SelfTime),
                ("internal markers are hidden with their subtrees", InternalHidden),
                ("frame selectors", FrameSelectors),
                ("thread selectors", ThreadSelectors),
                ("large times use a shifted block", ShiftedBlock),
                ("marker patterns", MarkerPatterns),
                ("worker bubbles and their causes", BubbleCauses),
            };
            var failed = 0;
            foreach (var (name, test) in tests)
            {
                try
                {
                    test();
                    w.WriteLine("ok    " + name);
                }
                catch (Exception e)
                {
                    failed++;
                    w.WriteLine("FAIL  " + name + ": " + e.Message);
                }
            }
            foreach (var file in Directory.EnumerateFiles(Path.GetTempPath(), "profilerwaitress-selftest-*.pwx"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException)
                {
                }
            }
            w.WriteLine(failed == 0 ? $"all {tests.Length} passed" : $"{failed} of {tests.Length} failed");
            return failed == 0 ? 0 : 1;
        }

        static void Check(bool condition, string message)
        {
            if (!condition)
                throw new Exception(message);
        }

        static void Near(double actual, double expected, string what) =>
            Check(Math.Abs(actual - expected) < 1e-6, $"{what}: expected {expected}, got {actual}");

        // Each frame: Main Thread root, PlayerLoop 10ms { A 4ms { A 2ms { B 1ms } }, Wait 3ms { BeginJob, Job 2ms, EndJob } }
        // and a worker with Job 5ms.
        static Builder Standard()
        {
            var b = new Builder();
            for (var f = 0; f < 3; f++)
            {
                b.Frame(16 + f);
                b.Thread(0, "", "Main Thread");
                b.Sample("Main Thread", 0, 0, 16);
                b.Sample("PlayerLoop", 1, 0, 10);
                b.Sample("A", 2, 0, 4);
                b.Sample("A", 3, 0, 2);
                b.Sample("B", 4, 0, 1);
                b.Sample("WaitForJobGroupID", 2, 5, 3);
                b.Sample("BeginJob", 3, 5, 0, internalMarker: true);
                b.Sample("Job", 3, 5, 2);
                b.Sample("EndJob", 3, 7, 0, internalMarker: true);
                b.Thread(1, "Job", "Worker 0");
                b.Sample("", 0, 0, 16);
                b.Sample("Job", 1, 1, 5);
            }
            return b;
        }

        static void ReaderRoundTrip()
        {
            using var c = Standard().Open();
            Check(c.FrameCount == 3, "frame count");
            Check(c.Threads.Length == 2 && c.Threads[1].Kind == ThreadKind.Worker && c.Threads[0].Kind == ThreadKind.Main, "threads");
            Check(c.FrameNumber(0) == 1, "frame numbers are one-based");
            Near(c.Frames[2].TimeNs / 1e6, 18, "frame time");
            var block = c.Blocks[c.Frames[1].BlockFirst];
            Near(c.DurNs(block, block.SampleBase + 1) / 1e6, 10, "sample duration");
            Near(c.StartNs(block, block.SampleBase + 5) / 1e6, 5, "sample start");
        }

        static Dictionary<string, FrameTable.Total> Totals(CaptureFile c, string threads, bool showInternal = false)
        {
            var scope = new Scope { Frames = Scope.ParseFrames(c, null), Threads = Scope.ParseThreads(c, threads), ThreadsText = threads, ShowInternal = showInternal };
            return FrameTable.Build(c, scope).Totals().Where(t => t.Calls > 0).ToDictionary(t => c.Markers[t.Marker].Name);
        }

        static void Recursion()
        {
            using var c = Standard().Open();
            var t = Totals(c, "main");
            Near(t["A"].Incl / 3e6, 4, "A incl per frame");
            Check(t["A"].Calls == 6, "A calls");
            Near(t["PlayerLoop"].Incl / 3e6, 10, "PlayerLoop incl");
            Check(!t.ContainsKey("Main Thread"), "thread root is not a marker");
        }

        static void SelfTime()
        {
            using var c = Standard().Open();
            var t = Totals(c, "main");
            Near(t["A"].Self / 3e6, 3, "A self (2 + 1)");
            Near(t["PlayerLoop"].Self / 3e6, 3, "PlayerLoop self");
            Near(t["WaitForJobGroupID"].Self / 3e6, 1, "wait self");
            var all = Totals(c, "all");
            Near(all["Job"].Incl / 3e6, 7, "Job over all threads");
        }

        static void InternalHidden()
        {
            using var c = Standard().Open();
            var hidden = Totals(c, "main");
            Check(!hidden.ContainsKey("BeginJob") && !hidden.ContainsKey("EndJob"), "internal markers hidden by default");
            var shown = Totals(c, "main", true);
            Check(shown.ContainsKey("BeginJob") && shown["BeginJob"].Calls == 3, "--internal shows them");

            // An internal sample with children: Unity hides the whole subtree, and the parent's self time still
            // excludes it (measured on HierarchyFrameDataView with markers of every verbosity).
            var b = new Builder();
            b.Frame(10);
            b.Thread(0, "", "Main Thread");
            b.Sample("Main Thread", 0, 0, 10);
            b.Sample("P", 1, 0, 8);
            b.Sample("I", 2, 0, 6, internalMarker: true);
            b.Sample("C", 3, 0, 2);
            b.Sample("C", 3, 3, 2);
            b.Sample("D", 2, 7, 1);
            using var c2 = b.Open();
            var t = Totals(c2, "main");
            Check(!t.ContainsKey("C") && !t.ContainsKey("I"), "the internal sample's subtree is hidden");
            Near(t["P"].Self / 1e6, 8 - 6 - 1, "P self excludes the hidden subtree");
            var withInternal = Totals(c2, "main", true);
            Check(withInternal["C"].Calls == 2 && withInternal.ContainsKey("I"), "--internal shows the subtree");

            // VerbosityExternal (0xC00) carries the internal bit and is hidden too; Advanced (0x1000) and Debug (0x400) are not.
            Check(new MarkerInfo { Flags = 0xC00 }.IsInternal && !new MarkerInfo { Flags = 0x1000 }.IsInternal && !new MarkerInfo { Flags = 0x400 }.IsInternal,
                  "verbosity flags");
        }

        static void FrameSelectors()
        {
            using var c = Standard().Open();
            Check(Scope.ParseFrames(c, "2-3").SequenceEqual(new[] { 1, 2 }), "range");
            Check(Scope.ParseFrames(c, "2-").SequenceEqual(new[] { 1, 2 }), "open range");
            Check(Scope.ParseFrames(c, "-2").SequenceEqual(new[] { 0, 1 }), "leading range");
            Check(Scope.ParseFrames(c, "skip:1").SequenceEqual(new[] { 1, 2 }), "skip");
            Check(Scope.ParseFrames(c, "last:1").SequenceEqual(new[] { 2 }), "last");
            Check(Scope.ParseFrames(c, "first:2").SequenceEqual(new[] { 0, 1 }), "first");
            Check(Scope.ParseFrames(c, "1,3").SequenceEqual(new[] { 0, 2 }), "list");
            var threw = false;
            try
            {
                Scope.ParseFrames(c, "40-50");
            }
            catch (WaitressException)
            {
                threw = true;
            }
            Check(threw, "out of range selects nothing and says so");
        }

        static void ThreadSelectors()
        {
            using var c = Standard().Open();
            Check(Scope.ParseThreads(c, "main").SequenceEqual(new[] { true, false }), "main");
            Check(Scope.ParseThreads(c, "workers").SequenceEqual(new[] { false, true }), "workers");
            Check(Scope.ParseThreads(c, "!main").SequenceEqual(new[] { false, true }), "exclusion alone");
            Check(Scope.ParseThreads(c, "all,!workers").SequenceEqual(new[] { true, false }), "all then exclusion");
            Check(Scope.ParseThreads(c, "Job/Worker*").SequenceEqual(new[] { false, true }), "glob");
        }

        static void ShiftedBlock()
        {
            var b = new Builder();
            b.Frame(9000);
            b.Thread(0, "", "Main Thread");
            b.Sample("Main Thread", 0, 0, 9000);
            b.Sample("Loading", 1, 0, 8000);
            using var c = b.Open();
            var block = c.Blocks[0];
            Check(block.Shift > 0, "a 9 s block needs a shift");
            Check(Math.Abs(c.DurNs(block, 1) / 1e6 - 8000) < 0.01, "duration survives the shift");
        }

        static void MarkerPatterns()
        {
            using var c = Standard().Open();
            Check(MarkerMatch.List(MarkerMatch.Resolve(c, "A")).Count == 1, "exact name wins over substring");
            Check(MarkerMatch.List(MarkerMatch.Resolve(c, "Loop")).Count == 1, "substring");
            Check(MarkerMatch.List(MarkerMatch.Resolve(c, "*Job*")).Count == 4, "glob");
        }

        // One 10 ms frame, four workers. Main schedules J (single, 0.1-1.1) and P (parallel, depends on J) from "A",
        // waits for P, schedules Q from "B", syncs on Q, runs "Serial" alone, schedules R late, and leaves the rest of the frame idle.
        static void BubbleCauses()
        {
            var b = new Builder();
            b.Frame(10);
            b.Thread(0, "", "Main Thread");
            b.Sample("Main Thread", 0, 0, 10);
            b.Sample("A", 1, 0, 0.1);
            b.Flow(1, 0);
            b.Flow(2, 0);
            b.Sample("ScheduleJob", 2, 0.05, 0, internalMarker: true);
            b.Sample("ScheduleJob", 2, 0.06, 0, internalMarker: true);
            b.Sample("WaitForJobGroupID", 1, 0.2, 1.3);
            b.Flow(2, 2);
            b.Sample("B", 1, 1.5, 1.7);
            b.Flow(3, 0);
            b.Flow(4, 0);
            b.Sample("ScheduleJob", 2, 1.6, 0, internalMarker: true);
            b.Sample("WaitForJobGroupID", 2, 1.62, 0.38);
            b.Flow(3, 2);
            b.Sample("Serial", 2, 2.0, 0.8);
            b.Sample("ScheduleJob", 2, 2.8, 0, internalMarker: true);
            b.Sample("WaitForJobGroupID", 2, 2.82, 0.38);
            b.Flow(4, 2);
            for (var w = 0; w < 4; w++)
            {
                b.Thread(1 + w, "Job", "Worker " + w);
                b.Sample("", 0, 0, 10);
                if (w == 0)
                    Job(b, "J", 0.1, 1.0, 1, 3);
                Job(b, "P", 1.12, 0.38, 2, 1);
                Job(b, "Q", 1.62, 0.38, 3, 1);
                Job(b, "R", 2.82, 0.38, 4, 1);
            }
            using var c = b.Open();
            var finder = new BubbleFinder(c, 0.25, 20_000);
            var found = finder.Frame(0, out var workers, out _, out var threshold);
            Check(workers == 4 && threshold == 1, $"4 workers, at most 1 busy (got {workers}, {threshold})");
            string Show(Bubble x) => $"{x.Cause} {(x.Start - 1_000_000_000) / 1e6:0.###}-{(x.End - 1_000_000_000) / 1e6:0.###} cost {x.Cost / 1e6:0.###}";
            var text = string.Join("; ", found.Select(Show));
            Check(found.Count == 5, "five causes: " + text);
            Check(Show(found[0]) == "LateSchedule 0-0.1 cost 0.1", "J was scheduled only at 0.05, which delayed P and main's wait on it: " + text);
            Check(Show(found[1]) == "OneJob 0.1-1.12 cost 0.92", "J alone held up P while main waited from 0.2: " + text);
            Check(c.MarkerName(finder.Jobs.Instances[found[1].Gate].Sample) == "J", "J is the gate");
            Check(Show(found[2]) == "Sync 1.5-1.62 cost 0.12", "P's wait drained the workers until B scheduled Q: " + text);
            Check(Show(found[3]) == "Sync 2-2.82 cost 0.38", "Q's wait drained the workers, Serial delayed R, main waited 0.38 on R: " + text);
            Check(c.MarkerName(found[3].Activity) == "Serial", "the sync names what ran alone");
            Check(Show(found[4]) == "NoWork 3.2-10 cost 0", "nothing after R: " + text);
        }

        static void Job(Builder b, string name, double start, double ms, uint flow, byte type)
        {
            b.Sample("BeginJob", 1, start, 0, internalMarker: true);
            b.Sample(name, 1, start, ms);
            b.Flow(flow, type);
            b.Sample("EndJob", 1, start + ms, 0, internalMarker: true);
        }

        /// <summary>Writes synthetic captures in the format Editor/ProfilerWaitressEditor.cs produces.</summary>
        sealed class Builder
        {
            readonly List<string> m_Strings = new List<string>();
            readonly List<(string name, bool internalMarker)> m_Markers = new List<(string, bool)>();
            readonly List<(int id, string group, string name)> m_Threads = new List<(int, string, string)>();
            readonly List<(double ms, int blockFirst)> m_Frames = new List<(double, int)>();
            readonly List<(int thread, int frame, List<(int marker, int depth, double start, double dur)> samples)> m_Blocks =
                new List<(int, int, List<(int, int, double, double)>)>();

            int Str(string s)
            {
                var i = m_Strings.IndexOf(s);
                if (i >= 0)
                    return i;
                m_Strings.Add(s);
                return m_Strings.Count - 1;
            }

            public void Frame(double ms) => m_Frames.Add((ms, m_Blocks.Count));

            public void Thread(int id, string group, string name)
            {
                if (!m_Threads.Any(t => t.id == id))
                    m_Threads.Add((id, group, name));
                m_Blocks.Add((m_Threads.FindIndex(t => t.id == id), m_Frames.Count - 1, new List<(int, int, double, double)>()));
            }

            public void Sample(string name, int depth, double startMs, double durMs, bool internalMarker = false)
            {
                var marker = m_Markers.FindIndex(m => m.name == name);
                if (marker < 0)
                {
                    m_Markers.Add((name, internalMarker));
                    marker = m_Markers.Count - 1;
                }
                m_Blocks[^1].samples.Add((marker, depth, startMs, durMs));
            }

            readonly List<(int block, int sample, uint id, byte type)> m_Flows = new List<(int, int, uint, byte)>();

            /// <summary>A flow event on the last sample: type 0 begin, 1 parallel, 2 end, 3 next.</summary>
            public void Flow(uint id, byte type) => m_Flows.Add((m_Blocks.Count - 1, m_Blocks[^1].samples.Count - 1, id, type));

            public CaptureFile Open()
            {
                var path = Path.Combine(Path.GetTempPath(), "profilerwaitress-selftest-" + Guid.NewGuid().ToString("N") + ".pwx");
                File.WriteAllBytes(path, Bytes());
                var file = CaptureFile.Open(path);
                return file;
            }

            byte[] Bytes()
            {
                const ulong origin = 1_000_000_000;
                var frameStart = new ulong[m_Frames.Count];
                ulong t = origin;
                for (var f = 0; f < m_Frames.Count; f++)
                {
                    frameStart[f] = t;
                    t += (ulong)(m_Frames[f].ms * 1e6);
                }

                var markers = new MemoryStream();
                var mw = new BinaryWriter(markers);
                mw.Write(m_Markers.Count);
                foreach (var (name, internalMarker) in m_Markers)
                {
                    mw.Write(Str(name));
                    mw.Write(Str("Scripts"));
                    mw.Write((ushort)(internalMarker ? 0x800 : 0));
                }
                var threads = new MemoryStream();
                var tw = new BinaryWriter(threads);
                tw.Write(m_Threads.Count);
                foreach (var (id, group, name) in m_Threads)
                {
                    tw.Write((ulong)id);
                    tw.Write(Str(group));
                    tw.Write(Str(name));
                }
                var frames = new MemoryStream();
                var fw = new BinaryWriter(frames);
                for (var f = 0; f < m_Frames.Count; f++)
                {
                    var next = f + 1 < m_Frames.Count ? m_Frames[f + 1].blockFirst : m_Blocks.Count;
                    fw.Write(frameStart[f]);
                    fw.Write((ulong)(m_Frames[f].ms * 1e6));
                    fw.Write(0UL);
                    fw.Write(0f);
                    fw.Write(m_Frames[f].blockFirst);
                    fw.Write(next - m_Frames[f].blockFirst);
                }
                var blocks = new MemoryStream();
                var bw = new BinaryWriter(blocks);
                var sm = new MemoryStream();
                var sd = new MemoryStream();
                var ss = new MemoryStream();
                var su = new MemoryStream();
                var smw = new BinaryWriter(sm);
                var sdw = new BinaryWriter(sd);
                var ssw = new BinaryWriter(ss);
                var suw = new BinaryWriter(su);
                long sampleBase = 0;
                foreach (var (thread, frame, samples) in m_Blocks)
                {
                    var max = samples.Count == 0 ? 0 : samples.Max(s => Math.Max(s.start, s.dur)) * 1e6;
                    byte shift = 0;
                    while ((ulong)max >> shift > uint.MaxValue)
                        shift++;
                    bw.Write(thread);
                    bw.Write(frame);
                    bw.Write(sampleBase);
                    bw.Write(samples.Count);
                    bw.Write(frameStart[frame]);
                    bw.Write(shift);
                    foreach (var (marker, depth, start, dur) in samples)
                    {
                        smw.Write(marker);
                        sdw.Write((ushort)depth);
                        ssw.Write((uint)((ulong)(start * 1e6) >> shift));
                        suw.Write((uint)((ulong)(dur * 1e6) >> shift));
                    }
                    sampleBase += samples.Count;
                }
                var flows = new MemoryStream();
                var flw = new BinaryWriter(flows);
                var bases = new long[m_Blocks.Count];
                for (var i = 1; i < m_Blocks.Count; i++)
                    bases[i] = bases[i - 1] + m_Blocks[i - 1].samples.Count;
                foreach (var (block, sample, id, type) in m_Flows)
                {
                    flw.Write(bases[block] + sample);
                    flw.Write(id);
                    flw.Write(type);
                }
                var head = new MemoryStream();
                var hw = new BinaryWriter(head);
                hw.Write(CaptureFile.FormatVersion);
                hw.Write(Str("synthetic.data"));
                hw.Write(0L);
                hw.Write(0L);
                hw.Write(0L);
                hw.Write(Str("test"));
                hw.Write(0);
                hw.Write(m_Frames.Count);
                hw.Write(300);
                var counters = new MemoryStream();
                new BinaryWriter(counters).Write(0);
                var stacks = new MemoryStream();
                new BinaryWriter(stacks).Write(0);
                var strings = new MemoryStream();
                var stw = new BinaryWriter(strings, new UTF8Encoding(false));
                stw.Write(m_Strings.Count);
                foreach (var s in m_Strings)
                    stw.Write(s);

                var sections = new List<(uint tag, byte[] data)>
                {
                    (CaptureFile.Head, head.ToArray()), (CaptureFile.Strs, strings.ToArray()), (CaptureFile.Mrkr, markers.ToArray()),
                    (CaptureFile.Thrd, threads.ToArray()), (CaptureFile.Fram, frames.ToArray()), (CaptureFile.Blck, blocks.ToArray()),
                    (CaptureFile.SMrk, sm.ToArray()), (CaptureFile.SDep, sd.ToArray()), (CaptureFile.SSta, ss.ToArray()),
                    (CaptureFile.SDur, su.ToArray()), (CaptureFile.Meta, Array.Empty<byte>()), (CaptureFile.Flow, flows.ToArray()),
                    (CaptureFile.Cntr, counters.ToArray()), (CaptureFile.CVal, Array.Empty<byte>()), (CaptureFile.Stak, stacks.ToArray()),
                    (CaptureFile.SStk, Array.Empty<byte>()),
                };
                var file = new MemoryStream();
                var w = new BinaryWriter(file);
                w.Write(CaptureFile.Magic);
                w.Write(sections.Count);
                long offset = 8 + sections.Count * 20L;
                foreach (var s in sections)
                {
                    w.Write(s.tag);
                    w.Write(offset);
                    w.Write((long)s.data.Length);
                    offset += s.data.Length;
                }
                foreach (var s in sections)
                    w.Write(s.data);
                return file.ToArray();
            }
        }
    }
}
