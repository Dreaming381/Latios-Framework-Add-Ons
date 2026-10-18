using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Analysis
{
    /// <summary>One marker's time in one frame, over the selected threads.</summary>
    public struct MarkerFrame
    {
        public int Marker;
        public long Incl;
        public long Self;
        public int Calls;
    }

    /// <summary>Per-frame marker totals for a scope. Most commands are a fold over this table.</summary>
    public sealed class FrameTable
    {
        public CaptureFile Capture;
        public Scope Scope;
        /// <summary>Parallel to Scope.Frames; each sorted by marker.</summary>
        public MarkerFrame[][] Rows;

        public static FrameTable Build(CaptureFile capture, Scope scope)
        {
            var rows = new MarkerFrame[scope.Frames.Length][];
            Parallel.For(0, scope.Frames.Length, () => new Walker(capture), (i, _, walker) =>
            {
                rows[i] = walker.Frame(scope.Frames[i], scope.Threads, scope.ShowInternal);
                return walker;
            }, _ => { });
            return new FrameTable { Capture = capture, Scope = scope, Rows = rows };
        }

        public struct Total
        {
            public int Marker;
            public long Incl;
            public long Self;
            public long Calls;
            public long MaxIncl;
            public int MaxFrame;
            public int Frames;
        }

        public Total[] Totals()
        {
            var totals = new Total[Capture.Markers.Length];
            for (var m = 0; m < totals.Length; m++)
                totals[m].Marker = m;
            for (var i = 0; i < Rows.Length; i++)
            {
                foreach (var r in Rows[i])
                {
                    ref var t = ref totals[r.Marker];
                    t.Incl += r.Incl;
                    t.Self += r.Self;
                    t.Calls += r.Calls;
                    t.Frames++;
                    if (r.Incl > t.MaxIncl)
                    {
                        t.MaxIncl = r.Incl;
                        t.MaxFrame = Scope.Frames[i];
                    }
                }
            }
            return totals;
        }

        public long Get(int row, int marker, bool self = false)
        {
            var list = Rows[row];
            int lo = 0, hi = list.Length - 1;
            while (lo <= hi)
            {
                var mid = (lo + hi) >> 1;
                if (list[mid].Marker == marker)
                    return self ? list[mid].Self : list[mid].Incl;
                if (list[mid].Marker < marker)
                    lo = mid + 1;
                else
                    hi = mid - 1;
            }
            return 0;
        }
    }

    /// <summary>
    /// Walks blocks depth-first with an explicit stack. Inclusive time counts a marker once per outermost
    /// occurrence, so recursion is not double counted; self time is a sample's time minus its children's.
    /// Thread roots (depth 0) are not markers anyone wrote and are skipped.
    /// </summary>
    public sealed unsafe class Walker
    {
        readonly CaptureFile m_Capture;
        readonly long[] m_Incl;
        readonly long[] m_Self;
        readonly int[] m_Calls;
        readonly int[] m_Open;
        readonly List<int> m_Touched = new List<int>();
        int[] m_StackMarker = new int[64];
        long[] m_StackChildren = new long[64];
        long[] m_StackDur = new long[64];
        int[] m_StackDepth = new int[64];
        bool m_ShowInternal;

        public Walker(CaptureFile capture)
        {
            m_Capture = capture;
            var n = capture.Markers.Length;
            m_Incl = new long[n];
            m_Self = new long[n];
            m_Calls = new int[n];
            m_Open = new int[n];
        }

        public MarkerFrame[] Frame(int frame, bool[] threads, bool showInternal)
        {
            m_ShowInternal = showInternal;
            var f = m_Capture.Frames[frame];
            for (var b = f.BlockFirst; b < f.BlockFirst + f.BlockCount; b++)
                if (threads[m_Capture.Blocks[b].Thread])
                    Block(b);
            m_Touched.Sort();
            var rows = new MarkerFrame[m_Touched.Count];
            for (var i = 0; i < rows.Length; i++)
            {
                var m = m_Touched[i];
                rows[i] = new MarkerFrame { Marker = m, Incl = m_Incl[m], Self = m_Self[m], Calls = m_Calls[m] };
                m_Incl[m] = m_Self[m] = 0;
                m_Calls[m] = 0;
            }
            m_Touched.Clear();
            return rows;
        }

        void Block(int blockIndex)
        {
            var block = m_Capture.Blocks[blockIndex];
            var markers = m_Capture.SampleMarker + block.SampleBase;
            var depths = m_Capture.SampleDepth + block.SampleBase;
            var durs = m_Capture.SampleDur + block.SampleBase;
            var shift = block.Shift;
            var sp = 0;
            for (var s = 0; s < block.SampleCount; s++)
            {
                int d = depths[s];
                while (sp > 0 && m_StackDepth[sp - 1] >= d)
                    Pop(ref sp);
                var dur = (long)((ulong)durs[s] << shift);
                if (d == 0)
                {
                    Push(ref sp, -1, dur, 0);
                    continue;
                }
                var m = markers[s];
                // Hidden with its subtree, as the Profiler window does; its time still leaves the parent's self.
                if (!m_ShowInternal && m_Capture.Markers[m].IsInternal)
                {
                    if (sp > 0)
                        m_StackChildren[sp - 1] += dur;
                    while (s + 1 < block.SampleCount && depths[s + 1] > d)
                        s++;
                    continue;
                }
                if (sp > 0)
                    m_StackChildren[sp - 1] += dur;
                if (m_Calls[m] == 0)
                    m_Touched.Add(m);
                if (m_Open[m] == 0)
                    m_Incl[m] += dur;
                m_Calls[m]++;
                m_Open[m]++;
                Push(ref sp, m, dur, d);
            }
            while (sp > 0)
                Pop(ref sp);
        }

        void Push(ref int sp, int marker, long dur, int depth)
        {
            if (sp == m_StackMarker.Length)
            {
                Array.Resize(ref m_StackDepth, sp * 2);
                Array.Resize(ref m_StackMarker, sp * 2);
                Array.Resize(ref m_StackChildren, sp * 2);
                Array.Resize(ref m_StackDur, sp * 2);
            }
            m_StackMarker[sp] = marker;
            m_StackDur[sp] = dur;
            m_StackChildren[sp] = 0;
            m_StackDepth[sp] = depth;
            sp++;
        }

        void Pop(ref int sp)
        {
            sp--;
            var m = m_StackMarker[sp];
            if (m < 0)
                return;
            m_Self[m] += Math.Max(0, m_StackDur[sp] - m_StackChildren[sp]);
            m_Open[m]--;
        }
    }

    public static class Stats
    {
        public static double Percentile(double[] sorted, double p)
        {
            if (sorted.Length == 0)
                return 0;
            var rank = p * (sorted.Length - 1);
            var lo = (int)Math.Floor(rank);
            var hi = Math.Min(sorted.Length - 1, lo + 1);
            return sorted[lo] + (sorted[hi] - sorted[lo]) * (rank - lo);
        }

        public static double Median(IEnumerable<double> values)
        {
            var list = new List<double>(values);
            list.Sort();
            return Percentile(list.ToArray(), 0.5);
        }
    }
}
