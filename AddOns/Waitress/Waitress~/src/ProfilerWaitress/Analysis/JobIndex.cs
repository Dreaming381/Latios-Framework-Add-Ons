using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProfilerWaitress.Commands;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Analysis
{
    /// <summary>
    /// Job executions and what they connect to. Unity brackets every job execution with BeginJob/EndJob samples and
    /// links them with flow events: Begin on the sample that scheduled the job, Next/ParallelNext on its executions,
    /// End on the wait that completed it. Flow ids are recycled about every 65536 flows, so events are matched to
    /// the nearest one in time, never by id alone.
    /// </summary>
    public sealed unsafe class JobIndex
    {
        /// <summary>How far apart in time two events with the same flow id may be and still belong together.</summary>
        public const long Window = 250_000_000;

        public struct Instance
        {
            public long Sample;
            public int Marker;
            public int Thread;
            public int Frame;
            public long StartAbs;
            public long EndAbs;
            public long Dur;
            public uint[] Flows;
        }

        public readonly List<Instance> Instances = new List<Instance>();
        public readonly Dictionary<uint, List<int>> FlowInstances = new Dictionary<uint, List<int>>();
        public readonly Dictionary<uint, List<(long time, long sample)>> Begins = new Dictionary<uint, List<(long, long)>>();
        public readonly Dictionary<long, List<uint>> EndsAt = new Dictionary<long, List<uint>>();
        public readonly Dictionary<long, int> InstanceOf = new Dictionary<long, int>();
        readonly CaptureFile m_Capture;
        readonly Dictionary<long, int> m_BeginsOn = new Dictionary<long, int>();
        readonly Dictionary<(long sample, uint flow), int> m_BeginRank = new Dictionary<(long, uint), int>();
        readonly Dictionary<long, List<long>> m_ScheduleCalls = new Dictionary<long, List<long>>();

        public JobIndex(CaptureFile c)
        {
            m_Capture = c;
            var begin = -1;
            var end = -1;
            var idle = -1;
            for (var m = 0; m < c.Markers.Length; m++)
            {
                if (c.Markers[m].Name == "BeginJob")
                    begin = m;
                else if (c.Markers[m].Name == "EndJob")
                    end = m;
                else if (c.Markers[m].Name == "Idle")
                    idle = m;
            }

            c.LoadFlows(out var flowSample, out var flowId, out var flowType);
            var order = Enumerable.Range(0, flowSample.Length).ToArray();
            var keys = (long[])flowSample.Clone();
            Array.Sort(keys, order);

            var perBlock = new List<(Instance inst, long rangeEnd)>[c.Blocks.Length];
            Parallel.For(0, c.Blocks.Length, b =>
            {
                var block = c.Blocks[b];
                List<(Instance, long)> found = null;
                var last = block.SampleBase + block.SampleCount;
                for (var s = block.SampleBase; s < last; s++)
                {
                    int d = c.SampleDepth[s];
                    var m = c.SampleMarker[s];
                    if (d == 0 || m == begin || m == end || m == idle)
                        continue;
                    var next = Markers.NextSibling(c, block, s);
                    var previous = PreviousSibling(c, block, s);
                    var afterBegin = previous >= 0 && c.SampleMarker[previous] == begin;
                    var beforeEnd = next < last && c.SampleDepth[next] == d && c.SampleMarker[next] == end;
                    if (!afterBegin && !beforeEnd)
                        continue;
                    var dur = c.DurNs(block, s);
                    var startAbs = (long)(block.BaseNs + ((ulong)c.SampleStart[s] << block.Shift));
                    (found ??= new List<(Instance, long)>()).Add((new Instance
                    {
                        Sample = s, Marker = m, Thread = block.Thread, Frame = block.Frame, StartAbs = startAbs, EndAbs = startAbs + dur, Dur = dur,
                    }, beforeEnd ? next + 1 : next));
                    s = next - 1;
                }
                perBlock[b] = found;
            });

            var flows = new List<uint>();
            foreach (var list in perBlock)
            {
                if (list == null)
                    continue;
                foreach (var (found, rangeEnd) in list)
                {
                    var inst = found;
                    var index = Instances.Count;
                    flows.Clear();
                    for (var i = LowerBound(keys, inst.Sample); i < keys.Length && keys[i] < rangeEnd; i++)
                    {
                        var e = order[i];
                        if (flowType[e] == 0 || flowType[e] == 2 || flows.Contains(flowId[e]))
                            continue;
                        flows.Add(flowId[e]);
                        if (!FlowInstances.TryGetValue(flowId[e], out var l))
                            FlowInstances[flowId[e]] = l = new List<int>(4);
                        l.Add(index);
                    }
                    inst.Flows = flows.Count == 0 ? Array.Empty<uint>() : flows.ToArray();
                    Instances.Add(inst);
                    InstanceOf[inst.Sample] = index;
                }
            }

            // Flows are stored block by block, so a forward-moving block cursor finds each event's block.
            var cursor = 0;
            for (var e = 0; e < flowSample.Length; e++)
            {
                var s = flowSample[e];
                while (cursor < c.Blocks.Length - 1 && s >= c.Blocks[cursor].SampleBase + c.Blocks[cursor].SampleCount)
                    cursor++;
                if (flowType[e] == 0)
                {
                    var b = c.Blocks[cursor];
                    var time = (long)(b.BaseNs + ((ulong)c.SampleStart[s] << b.Shift));
                    if (!Begins.TryGetValue(flowId[e], out var l))
                        Begins[flowId[e]] = l = new List<(long, long)>(4);
                    l.Add((time, s));
                    m_BeginsOn.TryGetValue(s, out var count);
                    m_BeginRank[(s, flowId[e])] = count;
                    m_BeginsOn[s] = count + 1;
                }
                else if (flowType[e] == 2)
                {
                    if (!EndsAt.TryGetValue(s, out var l))
                        EndsAt[s] = l = new List<uint>(2);
                    l.Add(flowId[e]);
                }
            }
            foreach (var l in Begins.Values)
                l.Sort((x, y) => x.time.CompareTo(y.time));
        }

        static long PreviousSibling(CaptureFile c, in BlockInfo block, long s)
        {
            var d = c.SampleDepth[s];
            for (var i = s - 1; i >= block.SampleBase; i--)
            {
                if (c.SampleDepth[i] == d)
                    return i;
                if (c.SampleDepth[i] < d)
                    return -1;
            }
            return -1;
        }

        static int LowerBound(long[] keys, long value)
        {
            int lo = 0, hi = keys.Length;
            while (lo < hi)
            {
                var mid = (lo + hi) >> 1;
                if (keys[mid] < value)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            return lo;
        }

        /// <summary>
        /// The marker that scheduled a job instance through a flow: the latest Begin of that flow id before the
        /// instance started. A bare ScheduleJob/KickJobs sample is replaced by its parent.
        /// </summary>
        public string Scheduler(uint flow, long instanceStart)
        {
            var (_, sample) = ScheduleOf(flow, instanceStart);
            return sample < 0 ? null : SchedulerName(sample);
        }

        public string SchedulerName(long beginSample)
        {
            var name = m_Capture.MarkerName(beginSample);
            return name is "ScheduleJob" or "ScheduleAllocJob" or "KickJobs" or "ScheduleBatchedJobs" ? ParentName(beginSample) ?? name : name;
        }

        /// <summary>The latest Begin of a flow before an instance started: (time, sample), or (-1, -1).</summary>
        public (long time, long sample) ScheduleOf(uint flow, long instanceStart)
        {
            if (!Begins.TryGetValue(flow, out var list))
                return (-1, -1);
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].time > instanceStart)
                    continue;
                return instanceStart - list[i].time <= Window ? list[i] : (-1, -1);
            }
            return (-1, -1);
        }

        /// <summary>
        /// When a job was scheduled. A Begin flow sits on the innermost visible sample open at the time (often a whole
        /// system), so its start can be long before the call. Its zero-length ScheduleJob children whose nearest visible
        /// ancestor is that sample come in the same order as its Begins; when the counts agree the matching one gives the
        /// time. Otherwise the latest of them before the job started is used.
        /// </summary>
        public long ScheduleTime(long beginSample, uint flow, long instanceStart)
        {
            var c = m_Capture;
            var block = c.Blocks[c.BlockOf(beginSample)];
            if (!m_ScheduleCalls.TryGetValue(beginSample, out var calls))
            {
                calls = new List<long>();
                int d = c.SampleDepth[beginSample];
                var end = block.SampleBase + block.SampleCount;
                for (var s = beginSample + 1; s < end && c.SampleDepth[s] > d; s++)
                {
                    var marker = c.Markers[c.SampleMarker[s]];
                    if (marker.Name == "ScheduleJob")
                        calls.Add((long)(block.BaseNs + ((ulong)c.SampleStart[s] << block.Shift)));
                    else if (!marker.IsInternal)
                    {
                        int sd = c.SampleDepth[s];
                        while (s + 1 < end && c.SampleDepth[s + 1] > sd)
                            s++;
                    }
                }
                m_ScheduleCalls[beginSample] = calls;
            }
            if (m_BeginsOn.TryGetValue(beginSample, out var count) && count == calls.Count && m_BeginRank.TryGetValue((beginSample, flow), out var rank))
                return calls[rank];
            var start = (long)(block.BaseNs + ((ulong)c.SampleStart[beginSample] << block.Shift));
            foreach (var t in calls)
                if (t <= instanceStart)
                    start = t;
            return start;
        }

        /// <summary>Where an instance was scheduled, through the first of its flows that has a Begin: (flow, time, sample).</summary>
        public (uint flow, long time, long sample) Schedule(int instance)
        {
            var inst = Instances[instance];
            foreach (var f in inst.Flows)
            {
                var (_, sample) = ScheduleOf(f, inst.StartAbs);
                if (sample >= 0)
                    return (f, ScheduleTime(sample, f, inst.StartAbs), sample);
            }
            return (0, -1, -1);
        }

        /// <summary>
        /// The executions of one scheduled job: the instances that share a flow and its Begin. A parallel job has one
        /// per thread that took batches; a single-threaded job has one.
        /// </summary>
        public List<int> Group(int instance)
        {
            var (flow, _, sample) = Schedule(instance);
            if (sample < 0)
                return new List<int> { instance };
            var group = new List<int>();
            foreach (var i in FlowInstances[flow])
                if (ScheduleOf(flow, Instances[i].StartAbs).sample == sample)
                    group.Add(i);
            return group;
        }

        public string Describe(int instance)
        {
            var (_, _, sample) = Schedule(instance);
            var name = m_Capture.Markers[Instances[instance].Marker].Name;
            return sample < 0 ? name : name + " (from " + SchedulerName(sample) + ")";
        }

        /// <summary>The instances a flow ending at <paramref name="endTime"/> completed.</summary>
        public IEnumerable<int> EndedBy(uint flow, long endTime)
        {
            if (!FlowInstances.TryGetValue(flow, out var list))
                yield break;
            foreach (var i in list)
            {
                var inst = Instances[i];
                if (inst.EndAbs <= endTime + 1000 && endTime - inst.EndAbs <= Window)
                    yield return i;
            }
        }

        /// <summary>
        /// The execution a wait waited on last: among those whose flows end in the wait's subtree, the one that finished
        /// last before it returned, or -1.
        /// </summary>
        public int WaitedOn(long waitSample)
        {
            var c = m_Capture;
            var block = c.Blocks[c.BlockOf(waitSample)];
            var waitEnd = (long)(block.BaseNs + ((ulong)c.SampleStart[waitSample] << block.Shift)) + c.DurNs(block, waitSample);
            var end = Markers.NextSibling(c, block, waitSample);
            var best = -1;
            for (var s = waitSample; s < end; s++)
            {
                if (!EndsAt.TryGetValue(s, out var flows))
                    continue;
                foreach (var f in flows)
                    foreach (var i in EndedBy(f, waitEnd))
                        if (best < 0 || Instances[i].EndAbs > Instances[best].EndAbs)
                            best = i;
            }
            return best;
        }

        int[] m_ByEnd;

        /// <summary>The instance that finished last within [start, end], or -1.</summary>
        public int LastFinishedWithin(long start, long end)
        {
            if (m_ByEnd == null)
            {
                m_ByEnd = Enumerable.Range(0, Instances.Count).ToArray();
                Array.Sort(m_ByEnd, (a, b) => Instances[a].EndAbs.CompareTo(Instances[b].EndAbs));
            }
            int lo = 0, hi = m_ByEnd.Length - 1, found = -1;
            while (lo <= hi)
            {
                var mid = (lo + hi) >> 1;
                if (Instances[m_ByEnd[mid]].EndAbs <= end)
                {
                    found = mid;
                    lo = mid + 1;
                }
                else
                    hi = mid - 1;
            }
            return found >= 0 && Instances[m_ByEnd[found]].EndAbs >= start ? m_ByEnd[found] : -1;
        }

        public string ParentName(long sample)
        {
            var b = m_Capture.BlockOf(sample);
            var block = m_Capture.Blocks[b];
            var d = m_Capture.SampleDepth[sample];
            for (var i = sample - 1; i >= block.SampleBase; i--)
                if (m_Capture.SampleDepth[i] < d)
                    return m_Capture.SampleDepth[i] == 0 ? null : m_Capture.MarkerName(i);
            return null;
        }
    }
}
