using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace ProfilerWaitress.Data
{
    public enum ThreadKind : byte
    {
        Main,
        Render,
        Worker,
        Background,
        Other,
    }

    public struct MarkerInfo
    {
        public string Name;
        public string Category;
        public ushort Flags;

        public bool IsCounter => (Flags & 128) != 0;

        /// <summary>
        /// Markers the Profiler window hides, with everything under them: VerbosityInternal (the job system's
        /// bookkeeping) and VerbosityExternal, both of which carry the 0x800 bit.
        /// </summary>
        public bool IsInternal => (Flags & 0x800) != 0;
    }

    public struct ThreadInfo
    {
        public ulong Id;
        public string Group;
        public string Name;
        public string Display;
        public ThreadKind Kind;
    }

    public struct FrameInfo
    {
        public ulong StartNs;
        public ulong TimeNs;
        public ulong GpuNs;
        public float Fps;
        public int BlockFirst;
        public int BlockCount;
    }

    /// <summary>One thread's samples in one frame, in depth-first order.</summary>
    public struct BlockInfo
    {
        public int Thread;
        public int Frame;
        public long SampleBase;
        public int SampleCount;
        public ulong BaseNs;
        public byte Shift;
    }

    public struct CounterInfo
    {
        public int Marker;
        public byte Type;
        public byte Unit;
        public double[] Values;
    }

    public struct MetaValue
    {
        public long Sample;
        public byte Index;
        public byte Kind; // 0 integer, 1 floating point, 2 string
        public string Name;
        public byte Unit;
        public long Bits;
    }

    /// <summary>
    /// A capture extracted by the Editor half of the tool (Editor/ProfilerWaitressEditor.cs writes the format). Sample
    /// columns stay memory-mapped; everything else is read eagerly.
    /// </summary>
    public sealed unsafe class CaptureFile : IDisposable
    {
        public const uint Magic = 0x31585750; // "PWX1"
        public const int FormatVersion = 2;

        public const uint Head = 0x44414548, Strs = 0x53525453, Mrkr = 0x524B524D, Thrd = 0x44524854, Fram = 0x4D415246,
                          Blck = 0x4B434C42, SMrk = 0x4B524D53, SDep = 0x50454453, SSta = 0x41545353, SDur = 0x52554453,
                          Meta = 0x4154454D, Flow = 0x574F4C46, Cntr = 0x52544E43, CVal = 0x4C415643, Stak = 0x4B415453,
                          SStk = 0x4B545353;

        readonly MemoryMappedFile m_File;
        readonly MemoryMappedViewAccessor m_View;
        readonly byte* m_Base;
        readonly Dictionary<uint, (long offset, long length)> m_Sections = new Dictionary<uint, (long, long)>();

        public readonly string CachePath;
        public string SourcePath;
        public long SourceSize;
        public long SourceTicks;
        public long ExtractedTicks;
        public string UnityVersion;
        public int FirstFrame;
        public int MaxFrameSetting;
        public string[] Strings;
        public MarkerInfo[] Markers;
        public ThreadInfo[] Threads;
        public FrameInfo[] Frames;
        public BlockInfo[] Blocks;
        public CounterInfo[] Counters;
        public long SampleCount;

        public int* SampleMarker;
        public ushort* SampleDepth;
        public uint* SampleStart;
        public uint* SampleDur;

        MetaValue[] m_Meta;
        long[] m_FlowSample;
        uint[] m_FlowId;
        byte[] m_FlowType;
        int[][] m_Stacks;
        Dictionary<long, int> m_SampleStacks;

        public int FrameCount => Frames.Length;

        CaptureFile(string path)
        {
            CachePath = path;
            m_File = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            m_View = m_File.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
            byte* p = null;
            m_View.SafeMemoryMappedViewHandle.AcquirePointer(ref p);
            m_Base = p + m_View.PointerOffset;
        }

        public static CaptureFile Open(string path)
        {
            var file = new CaptureFile(path);
            try
            {
                file.Load();
                return file;
            }
            catch
            {
                file.Dispose();
                throw;
            }
        }

        /// <summary>Reads just enough of a cache file to tell whether it still describes its source.</summary>
        public static (int version, string source, long size, long ticks)? ReadStamp(string path)
        {
            try
            {
                using var file = Open(path);
                return (FormatVersion, file.SourcePath, file.SourceSize, file.SourceTicks);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (m_View != null)
            {
                m_View.SafeMemoryMappedViewHandle.ReleasePointer();
                m_View.Dispose();
            }
            m_File?.Dispose();
        }

        BinaryReader Section(uint tag)
        {
            if (!m_Sections.TryGetValue(tag, out var s))
                throw new Waitress.WaitressException($"{CachePath} has no section {tag:x8}; delete it and extract again");
            return new BinaryReader(new UnmanagedMemoryStream(m_Base + s.offset, s.length), Encoding.UTF8);
        }

        void Load()
        {
            if (*(uint*)m_Base != Magic)
                throw new Waitress.WaitressException($"{CachePath} is not a profilerwaitress cache");
            var count = *(int*)(m_Base + 4);
            for (var i = 0; i < count; i++)
            {
                var entry = m_Base + 8 + i * 20;
                m_Sections[*(uint*)entry] = (*(long*)(entry + 4), *(long*)(entry + 12));
            }

            using (var r = Section(Strs))
            {
                Strings = new string[r.ReadInt32()];
                for (var i = 0; i < Strings.Length; i++)
                    Strings[i] = r.ReadString();
            }

            using (var r = Section(Head))
            {
                var version = r.ReadInt32();
                if (version != FormatVersion)
                    throw new Waitress.WaitressException($"{CachePath} has format {version}, this tool reads {FormatVersion}");
                SourcePath = Strings[r.ReadInt32()];
                SourceSize = r.ReadInt64();
                SourceTicks = r.ReadInt64();
                ExtractedTicks = r.ReadInt64();
                UnityVersion = Strings[r.ReadInt32()];
                FirstFrame = r.ReadInt32();
                r.ReadInt32();
                MaxFrameSetting = r.ReadInt32();
            }

            using (var r = Section(Mrkr))
            {
                Markers = new MarkerInfo[r.ReadInt32()];
                for (var i = 0; i < Markers.Length; i++)
                    Markers[i] = new MarkerInfo { Name = Strings[r.ReadInt32()], Category = Strings[r.ReadInt32()], Flags = r.ReadUInt16() };
            }

            using (var r = Section(Thrd))
            {
                Threads = new ThreadInfo[r.ReadInt32()];
                for (var i = 0; i < Threads.Length; i++)
                {
                    var t = new ThreadInfo { Id = r.ReadUInt64(), Group = Strings[r.ReadInt32()], Name = Strings[r.ReadInt32()] };
                    t.Display = t.Group.Length > 0 ? t.Group + "/" + t.Name : t.Name;
                    t.Kind = t.Name == "Main Thread" ? ThreadKind.Main
                        : t.Name == "Render Thread" ? ThreadKind.Render
                        : t.Group == "Job" ? ThreadKind.Worker
                        : t.Group == "Background Job" ? ThreadKind.Background
                        : ThreadKind.Other;
                    Threads[i] = t;
                }
            }

            using (var r = Section(Fram))
            {
                var n = (int)(m_Sections[Fram].length / 36);
                Frames = new FrameInfo[n];
                for (var i = 0; i < n; i++)
                    Frames[i] = new FrameInfo
                    {
                        StartNs = r.ReadUInt64(), TimeNs = r.ReadUInt64(), GpuNs = r.ReadUInt64(), Fps = r.ReadSingle(),
                        BlockFirst = r.ReadInt32(), BlockCount = r.ReadInt32(),
                    };
            }

            using (var r = Section(Blck))
            {
                var n = (int)(m_Sections[Blck].length / 29);
                Blocks = new BlockInfo[n];
                for (var i = 0; i < n; i++)
                    Blocks[i] = new BlockInfo
                    {
                        Thread = r.ReadInt32(), Frame = r.ReadInt32(), SampleBase = r.ReadInt64(), SampleCount = r.ReadInt32(),
                        BaseNs = r.ReadUInt64(), Shift = r.ReadByte(),
                    };
            }

            SampleCount = m_Sections[SDep].length / 2;
            SampleMarker = (int*)(m_Base + m_Sections[SMrk].offset);
            SampleDepth = (ushort*)(m_Base + m_Sections[SDep].offset);
            SampleStart = (uint*)(m_Base + m_Sections[SSta].offset);
            SampleDur = (uint*)(m_Base + m_Sections[SDur].offset);

            using (var r = Section(Cntr))
            {
                Counters = new CounterInfo[r.ReadInt32()];
                for (var i = 0; i < Counters.Length; i++)
                {
                    Counters[i] = new CounterInfo { Marker = r.ReadInt32(), Type = r.ReadByte(), Unit = r.ReadByte(), Values = new double[Frames.Length] };
                    // FrameTimingManager's "... Active Time" counters hold nanoseconds but declare no unit.
                    if (Counters[i].Unit == 0 && Markers[Counters[i].Marker].Name.EndsWith(" Time", StringComparison.Ordinal))
                        Counters[i].Unit = 1;
                }
            }
            using (var r = Section(CVal))
            {
                foreach (var c in Counters)
                    for (var f = 0; f < Frames.Length; f++)
                    {
                        // An unsigned counter holding -1 reads as about 1.8e19; it means "no value".
                        var v = r.ReadDouble();
                        c.Values[f] = v >= 1.8e19 ? double.NaN : v;
                    }
            }

            // Player captures leave the frame's GPU time empty and report it through a counter instead.
            // That counter comes from FrameTimingManager and describes a frame a few frames back. The same record's
            // 'CPU Total Frame Time' equals the real frame time once shifted, which measures the lag.
            if (Array.TrueForAll(Frames, f => f.GpuNs == 0))
            {
                var gpu = Array.FindIndex(Counters, x => Markers[x.Marker].Name == "GPU Frame Time");
                if (gpu >= 0)
                {
                    var cpu = Array.FindIndex(Counters, x => Markers[x.Marker].Name == "CPU Total Frame Time");
                    GpuCounterLag = cpu >= 0 ? CounterLag(Counters[cpu].Values) : -1;
                    var lag = Math.Max(0, GpuCounterLag);
                    for (var f = 0; f + lag < Frames.Length; f++)
                    {
                        var v = Counters[gpu].Values[f + lag];
                        if (!double.IsNaN(v) && v > 0)
                            Frames[f].GpuNs = (ulong)v;
                    }
                    GpuFromCounter = true;
                }
            }
        }

        /// <summary>Frame GPU times came from the 'GPU Frame Time' counter rather than the frame itself.</summary>
        public bool GpuFromCounter;

        /// <summary>How many frames late the FrameTimingManager counters report; -1 when it couldn't be measured.</summary>
        public int GpuCounterLag = -1;

        /// <summary>The shift that makes a frame-time counter match the frames' own times, if one matches closely.</summary>
        int CounterLag(double[] values)
        {
            int best = -1;
            var bestError = double.MaxValue;
            for (var lag = 0; lag <= 10; lag++)
            {
                var errors = new List<double>();
                for (var f = 0; f + lag < Frames.Length; f++)
                {
                    var v = values[f + lag];
                    if (!double.IsNaN(v) && v > 0 && Frames[f].TimeNs > 0)
                        errors.Add(Math.Abs(v - Frames[f].TimeNs) / Frames[f].TimeNs);
                }
                if (errors.Count < 10)
                    continue;
                errors.Sort();
                var median = errors[errors.Count / 2];
                if (median < bestError)
                {
                    bestError = median;
                    best = lag;
                }
            }
            return bestError < 0.02 ? best : -1;
        }

        /// <summary>Start of a sample, in nanoseconds from the start of its frame. Negative when it began in an earlier frame.</summary>
        public long StartNs(in BlockInfo block, long sample) =>
            (long)(block.BaseNs + ((ulong)SampleStart[sample] << block.Shift)) - (long)Frames[block.Frame].StartNs;

        public long DurNs(in BlockInfo block, long sample) => (long)((ulong)SampleDur[sample] << block.Shift);

        /// <summary>The number the Profiler window shows for a frame (its frame counter is one-based).</summary>
        public int FrameNumber(int frame) => FirstFrame + frame + 1;

        /// <summary>The frame index Unity's profiler API uses.</summary>
        public int ProfilerIndex(int frame) => FirstFrame + frame;

        public string MarkerName(long sample) => Markers[SampleMarker[sample]].Name;

        /// <summary>Metadata of every sample that has any, sorted by sample.</summary>
        public MetaValue[] Metadata
        {
            get
            {
                if (m_Meta != null)
                    return m_Meta;
                using var r = Section(Meta);
                var n = (int)(m_Sections[Meta].length / 23);
                var meta = new MetaValue[n];
                for (var i = 0; i < n; i++)
                    meta[i] = new MetaValue
                    {
                        Sample = r.ReadInt64(), Index = r.ReadByte(), Kind = r.ReadByte(), Name = Strings[r.ReadInt32()], Unit = r.ReadByte(),
                        Bits = r.ReadInt64(),
                    };
                return m_Meta = meta;
            }
        }

        /// <summary>Index of the first metadata value of a sample, or -1.</summary>
        public int FirstMeta(long sample)
        {
            var meta = Metadata;
            int lo = 0, hi = meta.Length - 1, found = -1;
            while (lo <= hi)
            {
                var mid = (lo + hi) >> 1;
                if (meta[mid].Sample < sample)
                    lo = mid + 1;
                else
                {
                    if (meta[mid].Sample == sample)
                        found = mid;
                    hi = mid - 1;
                }
            }
            return found;
        }

        public string MetaText(in MetaValue m) => m.Kind switch
        {
            1 => BitConverter.Int64BitsToDouble(m.Bits).ToString("G6", System.Globalization.CultureInfo.InvariantCulture),
            2 => Strings[(int)m.Bits],
            _ => m.Bits.ToString(System.Globalization.CultureInfo.InvariantCulture),
        };

        public void LoadFlows(out long[] sample, out uint[] id, out byte[] type)
        {
            if (m_FlowSample == null)
            {
                using var r = Section(Flow);
                var n = (int)(m_Sections[Flow].length / 13);
                m_FlowSample = new long[n];
                m_FlowId = new uint[n];
                m_FlowType = new byte[n];
                for (var i = 0; i < n; i++)
                {
                    m_FlowSample[i] = r.ReadInt64();
                    m_FlowId[i] = r.ReadUInt32();
                    m_FlowType[i] = r.ReadByte();
                }
            }
            sample = m_FlowSample;
            id = m_FlowId;
            type = m_FlowType;
        }

        public bool HasCallstacks => m_Sections.TryGetValue(SStk, out var s) && s.length > 0;

        /// <summary>The callstack of a sample, innermost frame first, or null.</summary>
        public int[] Callstack(long sample)
        {
            if (m_SampleStacks == null)
            {
                using (var r = Section(Stak))
                {
                    m_Stacks = new int[r.ReadInt32()][];
                    for (var i = 0; i < m_Stacks.Length; i++)
                    {
                        var frames = new int[r.ReadInt32()];
                        for (var k = 0; k < frames.Length; k++)
                            frames[k] = r.ReadInt32();
                        m_Stacks[i] = frames;
                    }
                }
                m_SampleStacks = new Dictionary<long, int>();
                using (var r = Section(SStk))
                {
                    var n = m_Sections[SStk].length / 12;
                    for (var i = 0; i < n; i++)
                        m_SampleStacks[r.ReadInt64()] = r.ReadInt32();
                }
            }
            return m_SampleStacks.TryGetValue(sample, out var id) ? m_Stacks[id] : null;
        }

        /// <summary>The block holding a global sample index.</summary>
        public int BlockOf(long sample)
        {
            int lo = 0, hi = Blocks.Length - 1;
            while (lo < hi)
            {
                var mid = (lo + hi + 1) >> 1;
                if (Blocks[mid].SampleBase <= sample)
                    lo = mid;
                else
                    hi = mid - 1;
            }
            // Empty blocks share their base with the block after the one holding the sample.
            while (lo > 0 && sample >= Blocks[lo].SampleBase + Blocks[lo].SampleCount)
                lo--;
            return lo;
        }
    }
}
