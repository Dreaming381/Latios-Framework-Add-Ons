using System;
using System.Collections.Generic;
using System.Linq;
using ProfilerWaitress.Commands;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Analysis
{
    public enum BubbleCause
    {
        OneJob,
        ParallelTail,
        Sync,
        LateSchedule,
        Unstarted,
        NoWork,
        Unknown,
    }

    /// <summary>A span of one frame where at most a few Job workers ran anything.</summary>
    public struct Bubble
    {
        public int Frame;
        public long Start;
        public long End;
        /// <summary>Workers running anything, averaged over the span.</summary>
        public double AvgBusy;
        public BubbleCause Cause;
        /// <summary>The job execution the work after the bubble waited for (OneJob, ParallelTail), or -1.</summary>
        public int Gate;
        /// <summary>The first job execution that ended the bubble, or -1.</summary>
        public int Release;
        public long ScheduledAt;
        /// <summary>The job wait that drained the workers (Sync), or -1.</summary>
        public long SyncWait;
        /// <summary>The job execution that wait waited on last, or -1.</summary>
        public int SyncGate;
        /// <summary>What the thread that scheduled the release (or the main thread) mostly did during the bubble, or -1.</summary>
        public long Activity;
        /// <summary>The child of Activity that covered the most of the rest, or -1.</summary>
        public long ActivityPart;
        /// <summary>Main-thread time spent waiting on jobs because of this bubble.</summary>
        public long Cost;
        /// <summary>For OneJob: how many single-threaded jobs ran back to back, and the first of them.</summary>
        public int Chain;
        /// <summary>For Unstarted: the scheduling thread ran the work itself (inline in a wait) while the workers stayed idle.</summary>
        public bool RanInline;
        /// <summary>For Sync: no End flow named the job the wait waited on, so SyncGate is the job that finished last by timing.</summary>
        public bool SyncGateGuessed;
        /// <summary>The main-thread job wait that overlapped the span the most, or -1.</summary>
        public long MainWaitSample;
        public int ChainFirst;
        /// <summary>Which bubble of its frame this cause belongs to.</summary>
        public int Id;
    }

    /// <summary>
    /// Finds worker bubbles frame by frame and names their causes. A bubble ends when work starts again; when that
    /// work was scheduled, and what was still running just before it started, separate the causes:
    /// a dependency still running (one job, or the slowest run of a parallel job), scheduled only during the bubble
    /// (late schedule, or a sync when a job wait on the scheduling thread drained the workers first), scheduled
    /// earlier with nothing running (unstarted: never kicked), or nothing started at all (no work).
    /// </summary>
    public sealed unsafe class BubbleFinder
    {
        /// <summary>Clock disagreement allowed between threads.</summary>
        const long k_Slack = 5_000;
        /// <summary>A wait shorter than this did not really block.</summary>
        const long k_MinBlock = 20_000;
        /// <summary>How soon after its dependency (or its scheduling) a job is expected to start.</summary>
        public const long WakeLatency = 150_000;

        readonly CaptureFile m_C;
        public readonly JobIndex Jobs;
        readonly List<int>[] m_InstancesByFrame;
        readonly bool[] m_NotWork;
        readonly bool[] m_JobWait;
        readonly bool[] m_AnyWait;
        readonly bool[] m_Cleanup;
        readonly bool[] m_Elsewhere;

        /// <summary>Main-thread time the last frame spent waiting on the render thread, the GPU or a frame cap.</summary>
        public long ElsewhereNs;
        readonly List<(long start, long end, long sample)> m_ElsewhereSpans = new List<(long, long, long)>();
        readonly double m_BusyFraction;
        readonly long m_MinNs;
        readonly int m_Main;
        readonly Dictionary<int, List<int>> m_ThreadBlocks = new Dictionary<int, List<int>>();
        readonly Dictionary<int, List<(long start, long end, long sample)>> m_Waits = new Dictionary<int, List<(long, long, long)>>();
        readonly Dictionary<int, List<int>> m_Groups = new Dictionary<int, List<int>>();

        public BubbleFinder(CaptureFile c, double busyFraction, long minNs)
        {
            m_C = c;
            m_BusyFraction = busyFraction;
            m_MinNs = minNs;
            Jobs = new JobIndex(c);
            m_InstancesByFrame = new List<int>[c.FrameCount];
            for (var f = 0; f < c.FrameCount; f++)
                m_InstancesByFrame[f] = new List<int>();
            for (var i = 0; i < Jobs.Instances.Count; i++)
                m_InstancesByFrame[Jobs.Instances[i].Frame].Add(i);
            m_NotWork = c.Markers.Select(m => m.Name is "Idle" or "BeginJob" or "EndJob" || Commands.Jobs.IsWait(m.Name)).ToArray();
            m_Cleanup = c.Markers.Select(m => m.Name.EndsWith(" (Cleanup)", StringComparison.Ordinal)).ToArray();
            m_AnyWait = c.Markers.Select(m => Commands.Jobs.IsWait(m.Name)).ToArray();
            m_JobWait = c.Markers.Select(m => Commands.Jobs.IsWait(m.Name) && Commands.Jobs.IsJobWait(m.Name)).ToArray();
            m_Main = Array.FindIndex(c.Threads, t => t.Kind == ThreadKind.Main);
            m_Elsewhere = c.Markers.Select(m => m.Name is "Gfx.WaitForRenderThread" or "WaitForTargetFPS" or "Application.Idle" || m.Name.Contains("WaitForPresent", StringComparison.Ordinal) ||
                                                    m.Name.Contains("WaitForLastPresentation", StringComparison.Ordinal) || m.Name.Contains("WaitOnSwapChain", StringComparison.Ordinal)).ToArray();
        }

        public List<int> Group(int instance)
        {
            if (!m_Groups.TryGetValue(instance, out var g))
            {
                g = Jobs.Group(instance);
                foreach (var i in g)
                    m_Groups[i] = g;
            }
            return g;
        }

        public List<Bubble> Frame(int f, out int workers, out long busyNs, out int threshold)
        {
            var frame = m_C.Frames[f];
            var f0 = (long)frame.StartNs;
            var f1 = f0 + (long)frame.TimeNs;
            m_ThreadBlocks.Clear();
            m_Waits.Clear();
            for (var g = Math.Max(0, f - 1); g <= Math.Min(m_C.FrameCount - 1, f + 1); g++)
            {
                var fr = m_C.Frames[g];
                for (var b = fr.BlockFirst; b < fr.BlockFirst + fr.BlockCount; b++)
                {
                    var t = m_C.Blocks[b].Thread;
                    if (!m_ThreadBlocks.TryGetValue(t, out var list))
                        m_ThreadBlocks[t] = list = new List<int>();
                    list.Add(b);
                }
            }

            ElsewhereNs = 0;
            m_ElsewhereSpans.Clear();
            for (var b = frame.BlockFirst; b < frame.BlockFirst + frame.BlockCount; b++)
            {
                var block = m_C.Blocks[b];
                if (block.Thread != m_Main)
                    continue;
                var end = block.SampleBase + block.SampleCount;
                for (var s = block.SampleBase; s < end; s++)
                {
                    if (!m_Elsewhere[m_C.SampleMarker[s]])
                        continue;
                    ElsewhereNs += m_C.DurNs(block, s);
                    var es = Abs(block, s);
                    m_ElsewhereSpans.Add((es, es + m_C.DurNs(block, s), s));
                    int d = m_C.SampleDepth[s];
                    while (s + 1 < end && m_C.SampleDepth[s + 1] > d)
                        s++;
                }
            }

            // Each worker's busy time, merged so nested samples count once.
            var events = new List<(long time, int delta)>();
            workers = 0;
            busyNs = 0;
            var spans = new List<(long s, long e)>();
            for (var b = frame.BlockFirst; b < frame.BlockFirst + frame.BlockCount; b++)
            {
                var thread = m_C.Blocks[b].Thread;
                if (m_C.Threads[thread].Kind != ThreadKind.Worker)
                    continue;
                workers++;
                spans.Clear();
                foreach (var nb in m_ThreadBlocks[thread])
                {
                    var block = m_C.Blocks[nb];
                    var end = block.SampleBase + block.SampleCount;
                    for (var s = block.SampleBase; s < end; s++)
                    {
                        if (m_C.SampleDepth[s] != 1 || m_NotWork[m_C.SampleMarker[s]])
                            continue;
                        var st = Abs(block, s);
                        var en = Math.Min(f1, st + m_C.DurNs(block, s));
                        st = Math.Max(f0, st);
                        if (en > st)
                            spans.Add((st, en));
                    }
                }
                spans.Sort();
                long cs = -1, ce = -1;
                foreach (var (s, e) in spans)
                {
                    if (s > ce)
                    {
                        if (ce > cs)
                        {
                            events.Add((cs, 1));
                            events.Add((ce, -1));
                            busyNs += ce - cs;
                        }
                        cs = s;
                        ce = e;
                    }
                    else
                        ce = Math.Max(ce, e);
                }
                if (ce > cs)
                {
                    events.Add((cs, 1));
                    events.Add((ce, -1));
                    busyNs += ce - cs;
                }
            }
            threshold = (int)Math.Floor(workers * m_BusyFraction);
            var bubbles = new List<Bubble>();
            if (workers == 0)
                return bubbles;
            // Starts sort before ends at the same instant, so back-to-back work leaves no zero-length gap.
            events.Sort((x, y) => x.time != y.time ? x.time.CompareTo(y.time) : y.delta.CompareTo(x.delta));

            var busy = 0;
            var open = f0;
            var isOpen = true;
            var stepTime = new List<long> { f0 };
            var stepBusy = new List<int> { 0 };
            foreach (var (time, delta) in events.Append((f1, 0)))
            {
                busy += delta;
                if (stepTime[^1] == time)
                    stepBusy[^1] = busy;
                else
                {
                    stepTime.Add(time);
                    stepBusy.Add(busy);
                }
                if (isOpen && (busy > threshold || time == f1))
                {
                    if (time - open >= m_MinNs)
                        bubbles.Add(new Bubble { Frame = f, Start = open, End = time });
                    isOpen = false;
                }
                if (!isOpen && busy <= threshold && time < f1)
                {
                    isOpen = true;
                    open = time;
                }
            }

            var near = new List<int>();
            for (var g = Math.Max(0, f - 1); g <= Math.Min(m_C.FrameCount - 1, f + 1); g++)
                near.AddRange(m_InstancesByFrame[g]);
            // Main-thread waiting inside a bubble is charged to it directly; waiting later is charged at most once.
            m_Claimed.Clear();
            foreach (var b in bubbles)
                m_Claimed.Add((b.Start, b.End));
            var segments = new List<Bubble>();
            for (var i = 0; i < bubbles.Count; i++)
            {
                var b = bubbles[i];
                b.Id = i;
                Split(b, near, segments);
            }
            // Average busy workers over each segment, from the step function of busy counts.
            var k = 0;
            for (var i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                while (k + 1 < stepTime.Count && stepTime[k + 1] <= seg.Start)
                    k++;
                double area = 0;
                for (var j = k; j < stepTime.Count && stepTime[j] < seg.End; j++)
                {
                    var s = Math.Max(seg.Start, stepTime[j]);
                    var e = j + 1 < stepTime.Count ? Math.Min(seg.End, stepTime[j + 1]) : seg.End;
                    area += stepBusy[j] * (double)Math.Max(0, e - s);
                }
                seg.AvgBusy = seg.End > seg.Start ? area / (seg.End - seg.Start) : 0;
                segments[i] = seg;
            }
            return segments;
        }

        readonly List<(long start, long end)> m_Claimed = new List<(long, long)>();

        /// <summary>
        /// Splits a bubble into causes, walking back from its end: the work that ended it, then whatever held that work
        /// up (a dependency still running, a late or unkicked schedule), then what held that up, until the bubble's start.
        /// </summary>
        void Split(Bubble bubble, List<int> near, List<Bubble> segments)
        {
            var first = segments.Count;
            var a = bubble.Start;
            var end = bubble.End;
            var releaseEnd = end;
            var release = -1;
            var starts = near.Where(i => m_C.Threads[Jobs.Instances[i].Thread].Kind == ThreadKind.Worker &&
                                         Jobs.Instances[i].StartAbs >= end - k_Slack && Jobs.Instances[i].StartAbs <= end + 50_000).ToList();
            if (starts.Count > 0)
            {
                release = starts.OrderByDescending(i => Group(i).Count(j => starts.Contains(j))).ThenBy(i => Jobs.Instances[i].StartAbs).First();
                releaseEnd = Group(release).Max(i => Jobs.Instances[i].EndAbs);
            }
            while (true)
            {
                var seg = bubble;
                seg.Start = a;
                seg.End = end;
                seg.Release = release;
                seg.Gate = seg.SyncGate = -1;
                seg.SyncWait = seg.Activity = seg.ActivityPart = seg.ScheduledAt = -1;
                if (release < 0)
                {
                    seg.Cause = BubbleCause.NoWork;
                    (seg.Activity, seg.ActivityPart) = Activity(m_Main, a, end);
                    segments.Add(seg);
                    break;
                }
                var group = Group(release);
                var (_, scheduled, schedSample) = Jobs.Schedule(release);
                seg.ScheduledAt = scheduled;
                var schedThread = schedSample >= 0 ? m_C.Blocks[m_C.BlockOf(schedSample)].Thread : m_Main;

                // A dependency still running: the execution that finished last, just before the work started.
                // A zero-length "(Cleanup)" execution only stands in when no real one qualifies.
                var gate = -1;
                var gateIsCleanup = true;
                foreach (var i in near)
                {
                    var inst = Jobs.Instances[i];
                    if (inst.StartAbs >= end || inst.EndAbs <= a || inst.EndAbs > end + k_Slack || inst.EndAbs < end - WakeLatency)
                        continue;
                    if ((scheduled >= 0 && inst.EndAbs < scheduled - k_Slack) || group.Contains(i))
                        continue;
                    var cleanup = m_Cleanup[inst.Marker];
                    if (gate < 0 || (gateIsCleanup && !cleanup) || (cleanup == gateIsCleanup && inst.EndAbs > Jobs.Instances[gate].EndAbs))
                    {
                        gate = i;
                        gateIsCleanup = cleanup;
                    }
                }
                if (gate >= 0)
                {
                    seg.Gate = gate;
                    seg.Chain = 1;
                    seg.ChainFirst = gate;
                    seg.Cause = Group(gate).Count == 1 ? BubbleCause.OneJob : BubbleCause.ParallelTail;
                    seg.Start = Math.Max(a, Jobs.Instances[gate].StartAbs);
                    if (seg.Start - a < m_MinNs)
                        seg.Start = a;
                    segments.Add(seg);
                    if (seg.Start == a)
                        break;
                    end = seg.Start;
                    release = gate;
                    continue;
                }
                if (scheduled < 0)
                {
                    seg.Cause = BubbleCause.Unknown;
                    (seg.Activity, seg.ActivityPart) = Activity(m_Main, a, end);
                    segments.Add(seg);
                    break;
                }
                if (end - scheduled > WakeLatency)
                {
                    seg.Cause = BubbleCause.Unstarted;
                    seg.Start = Math.Max(a, scheduled);
                    if (seg.Start - a < m_MinNs)
                        seg.Start = a;
                    var segStart = seg.Start;
                    var segEnd = end;
                    seg.RanInline = group.Any(i => Jobs.Instances[i].Thread == schedThread &&
                                                         (Math.Min(Jobs.Instances[i].EndAbs, segEnd) - Math.Max(Jobs.Instances[i].StartAbs, segStart)) * 2 > segEnd - segStart);
                    (seg.Activity, seg.ActivityPart) = Activity(schedThread, seg.Start, end);
                    segments.Add(seg);
                    if (seg.Start == a)
                        break;
                    end = seg.Start;
                    seg.Activity = -1;
                    seg.RanInline = false;
                }

                // Scheduled only during the bubble. If the scheduling thread blocked on jobs in it first, that wait is a
                // sync point: from it on the thread waited on little work, or ran alone after draining the workers.
                seg.Start = a;
                seg.End = end;
                long syncStart = a, syncEnd = a;
                foreach (var (ws, we, sample) in Waits(schedThread))
                {
                    if (we < a - 50_000 || we > scheduled || we - ws < k_MinBlock)
                        continue;
                    if (seg.SyncWait < 0 || we > syncEnd)
                    {
                        seg.SyncWait = sample;
                        syncStart = Math.Max(a, ws);
                        syncEnd = Math.Max(a, we);
                    }
                }
                if (seg.SyncWait < 0)
                {
                    seg.Cause = BubbleCause.LateSchedule;
                    (seg.Activity, seg.ActivityPart) = Activity(schedThread, a, Math.Max(a, Math.Min(end, scheduled)));
                    segments.Add(seg);
                    break;
                }
                seg.Cause = BubbleCause.Sync;
                seg.Start = syncStart - a < m_MinNs ? a : syncStart;
                (seg.Activity, seg.ActivityPart) = Activity(schedThread, syncEnd, Math.Max(syncEnd, Math.Min(end, scheduled)));
                var waitBlock = m_C.Blocks[m_C.BlockOf(seg.SyncWait)];
                seg.SyncGate = Jobs.WaitedOn(seg.SyncWait);
                if (seg.SyncGate < 0)
                {
                    seg.SyncGate = LastJobBefore(Abs(waitBlock, seg.SyncWait) + m_C.DurNs(waitBlock, seg.SyncWait), near);
                    seg.SyncGateGuessed = seg.SyncGate >= 0;
                }
                segments.Add(seg);
                if (seg.Start > a)
                {
                    var before = seg;
                    before.Start = a;
                    before.End = seg.Start;
                    before.Cause = BubbleCause.LateSchedule;
                    before.SyncWait = before.Activity = before.ActivityPart = -1;
                    before.SyncGate = -1;
                    (before.Activity, before.ActivityPart) = Activity(schedThread, a, seg.Start);
                    segments.Add(before);
                }
                break;
            }

            segments.Reverse(first, segments.Count - first);
            // A chain of single-threaded jobs reads as one cause; slivers under --min join the cause they fed into.
            var merged = new List<Bubble>();
            for (var i = first; i < segments.Count; i++)
            {
                var seg = segments[i];
                if (merged.Count > 0)
                {
                    var prev = merged[^1];
                    if (prev.Cause == BubbleCause.OneJob && seg.Cause == BubbleCause.OneJob && SchedulerOf(prev.Gate) == SchedulerOf(seg.Gate))
                    {
                        seg.Start = prev.Start;
                        seg.Chain += prev.Chain;
                        seg.ChainFirst = prev.ChainFirst;
                        merged[^1] = seg;
                        continue;
                    }
                    if (prev.End - prev.Start < m_MinNs)
                    {
                        seg.Start = prev.Start;
                        merged[^1] = seg;
                        continue;
                    }
                }
                merged.Add(seg);
            }
            if (merged.Count > 1 && merged[^1].End - merged[^1].Start < m_MinNs)
            {
                var last = merged[^1];
                merged.RemoveAt(merged.Count - 1);
                var prev = merged[^1];
                prev.End = last.End;
                merged[^1] = prev;
            }
            segments.RemoveRange(first, segments.Count - first);
            segments.AddRange(merged);
            for (var i = first; i < segments.Count; i++)
            {
                var seg = segments[i];
                // Waiting on a frame cap or on present is idleness by choice or bound elsewhere, not a scheduling gap.
                if (seg.Cause is BubbleCause.LateSchedule or BubbleCause.Unknown or BubbleCause.NoWork)
                {
                    foreach (var (es, ee, sample) in m_ElsewhereSpans)
                    {
                        if ((Math.Min(ee, seg.End) - Math.Max(es, seg.Start)) * 2 > seg.End - seg.Start)
                        {
                            seg.Cause = BubbleCause.NoWork;
                            seg.Activity = sample;
                            seg.ActivityPart = -1;
                        }
                    }
                }
                seg.Cost = MainWait(seg.Start, seg.End);
                seg.MainWaitSample = -1;
                long most = 0;
                foreach (var (ws, we, sample) in Waits(m_Main))
                {
                    var overlap = Math.Min(we, seg.End) - Math.Max(ws, seg.Start);
                    if (overlap > most)
                    {
                        most = overlap;
                        seg.MainWaitSample = sample;
                    }
                }
                // Work scheduled late or left unkicked makes the main thread wait for it later, by up to the delay.
                if (seg.Cause is BubbleCause.LateSchedule or BubbleCause.Sync or BubbleCause.Unstarted)
                    seg.Cost += Claim(bubble.End, releaseEnd + k_Slack, seg.End - seg.Start - seg.Cost);
                segments[i] = seg;
            }
        }

        /// <summary>Main-thread job waiting in [from, to] outside bubbles and earlier claims, up to cap; marks it claimed.</summary>
        long Claim(long from, long to, long cap)
        {
            long total = 0;
            foreach (var (ws, we, _) in Waits(m_Main))
            {
                var s = Math.Max(ws, from);
                var e = Math.Min(we, to);
                while (s < e && total < cap)
                {
                    var next = e;
                    var covered = false;
                    foreach (var (cs, ce) in m_Claimed)
                    {
                        if (cs <= s && ce > s)
                        {
                            s = ce;
                            covered = true;
                            break;
                        }
                        if (cs > s && cs < next)
                            next = cs;
                    }
                    if (covered)
                        continue;
                    var take = Math.Min(next - s, cap - total);
                    m_Claimed.Add((s, s + take));
                    total += take;
                    s += take;
                }
            }
            return total;
        }


        string SchedulerOf(int instance)
        {
            var sample = Jobs.Schedule(instance).sample;
            return sample < 0 ? null : Jobs.SchedulerName(sample);
        }

        int LastJobBefore(long time, List<int> near)
        {
            var best = -1;
            foreach (var i in near)
            {
                var inst = Jobs.Instances[i];
                if (inst.EndAbs > time + k_Slack || inst.EndAbs < time - WakeLatency)
                    continue;
                if (best < 0 || inst.EndAbs > Jobs.Instances[best].EndAbs)
                    best = i;
            }
            return best;
        }

        long Abs(in BlockInfo block, long s) => (long)(block.BaseNs + ((ulong)m_C.SampleStart[s] << block.Shift));

        /// <summary>Outermost job waits on a thread around the current frame.</summary>
        List<(long start, long end, long sample)> Waits(int thread)
        {
            if (m_Waits.TryGetValue(thread, out var list))
                return list;
            m_Waits[thread] = list = new List<(long, long, long)>();
            if (thread < 0 || !m_ThreadBlocks.TryGetValue(thread, out var blocks))
                return list;
            foreach (var b in blocks)
            {
                var block = m_C.Blocks[b];
                var end = block.SampleBase + block.SampleCount;
                for (var s = block.SampleBase; s < end; s++)
                {
                    int d = m_C.SampleDepth[s];
                    if (d == 0 || !m_JobWait[m_C.SampleMarker[s]])
                        continue;
                    var st = Abs(block, s);
                    list.Add((st, st + m_C.DurNs(block, s), s));
                    while (s + 1 < end && m_C.SampleDepth[s + 1] > d)
                        s++;
                }
            }
            return list;
        }

        /// <summary>Main-thread time spent in job waits within [from, to].</summary>
        long MainWait(long from, long to)
        {
            long total = 0;
            if (to <= from)
                return 0;
            foreach (var (s, e, _) in Waits(m_Main))
                total += Math.Max(0, Math.Min(e, to) - Math.Max(s, from));
            return total;
        }

        /// <summary>The deepest visible sample on a thread covering more than half of [from, to], or -1.</summary>
        (long sample, long part) Activity(int thread, long from, long to)
        {
            var best = Dominant(thread, from, to, out var part);
            return (best, part);
        }

        long Dominant(int thread, long from, long to) => Dominant(thread, from, to, out _);

        /// <summary>
        /// The deepest visible sample on a thread covering more than half of [from, to] (or the job it runs inside), or -1;
        /// and its child covering the most of the rest, when that is at least a quarter.
        /// </summary>
        long Dominant(int thread, long from, long to, out long part)
        {
            part = -1;
            if (thread < 0 || to <= from || !m_ThreadBlocks.TryGetValue(thread, out var blocks))
                return -1;
            long best = -1;
            var bestBlock = -1;
            var candidates = new List<(long sample, long parent, long overlap)>();
            var bestDepth = 0;
            var half = (to - from) / 2;
            var chain = new List<long>();
            foreach (var b in blocks)
            {
                var block = m_C.Blocks[b];
                if (block.SampleCount == 0)
                    continue;
                var end = block.SampleBase + block.SampleCount;
                // Starts are non-decreasing in depth-first order: find the first sample starting at or after `from`.
                long lo = block.SampleBase, hi = end;
                while (lo < hi)
                {
                    var mid = (lo + hi) >> 1;
                    if (Abs(block, mid) < from)
                        lo = mid + 1;
                    else
                        hi = mid;
                }
                var parents = Parents(b);
                chain.Clear();
                for (var p = lo - 1; p >= block.SampleBase; p = parents[p - block.SampleBase])
                    chain.Add(p);
                chain.Reverse();
                foreach (var s in chain)
                {
                    if (m_C.SampleDepth[s] > 0 && m_C.Markers[m_C.SampleMarker[s]].IsInternal)
                        break;
                    Consider(block, s);
                }
                for (var s = lo; s < end; s++)
                {
                    int d = m_C.SampleDepth[s];
                    if (Abs(block, s) >= to)
                        break;
                    if (d > 0 && m_C.Markers[m_C.SampleMarker[s]].IsInternal)
                    {
                        while (s + 1 < end && m_C.SampleDepth[s + 1] > d)
                            s++;
                        continue;
                    }
                    Consider(block, s);
                }
            }
            if (best < 0)
                return -1;
            var parentsOfBest = Parents(bestBlock);
            var baseOfBest = m_C.Blocks[bestBlock].SampleBase;
            for (var p = parentsOfBest[best - baseOfBest]; p >= 0; p = parentsOfBest[p - baseOfBest])
                if (Jobs.InstanceOf.ContainsKey(p))
                    return p;
            if (Jobs.InstanceOf.ContainsKey(best))
                return best;
            long partOverlap = (to - from) / 4;
            foreach (var (s, parent, overlap) in candidates)
                if (parent == best && overlap >= partOverlap)
                {
                    part = s;
                    partOverlap = overlap;
                }
            return best;

            void Consider(in BlockInfo block, long s)
            {
                int d = m_C.SampleDepth[s];
                var st = Abs(block, s);
                var en = st + m_C.DurNs(block, s);
                var overlap = Math.Min(en, to) - Math.Max(st, from);
                if (overlap <= 0)
                    return;
                var b = m_C.BlockOf(s);
                candidates.Add((s, Parents(b)[s - m_C.Blocks[b].SampleBase], overlap));
                if (d > bestDepth && overlap > half)
                {
                    best = s;
                    bestDepth = d;
                    bestBlock = b;
                }
            }
        }

        readonly Dictionary<int, long[]> m_Parents = new Dictionary<int, long[]>();

        /// <summary>Each sample's parent in a block (absolute sample index), or -1 for roots.</summary>
        long[] Parents(int b)
        {
            if (m_Parents.TryGetValue(b, out var parents))
                return parents;
            if (m_Parents.Count > 64)
                m_Parents.Clear();
            var block = m_C.Blocks[b];
            parents = new long[block.SampleCount];
            var stack = new long[64];
            for (var i = 0; i < block.SampleCount; i++)
            {
                var s = block.SampleBase + i;
                int d = m_C.SampleDepth[s];
                if (d >= stack.Length)
                    Array.Resize(ref stack, d * 2);
                parents[i] = d == 0 ? -1 : stack[d - 1];
                stack[d] = s;
            }
            m_Parents[b] = parents;
            return parents;
        }
    }
}
