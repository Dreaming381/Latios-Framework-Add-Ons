// The Editor half of profilerwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/ProfilerWaitress/,
// and runs its entry points through `unity command run_script`, so nothing is imported into the project.
// Every entry point returns one line of JSON.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Profiling.LowLevel;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEditorInternal;
using UnityEngine;

public static class ProfilerWaitressEditor
{
    const int k_FormatVersion = 2;
    const string k_LoadedKey = "ProfilerWaitress.Loaded";

    // Must match Data/CaptureFile.cs in the CLI.
    const uint k_Head = 0x44414548, k_Strs = 0x53525453, k_Mrkr = 0x524B524D, k_Thrd = 0x44524854, k_Fram = 0x4D415246,
               k_Blck = 0x4B434C42, k_SMrk = 0x4B524D53, k_SDep = 0x50454453, k_SSta = 0x41545353, k_SDur = 0x52554453,
               k_Meta = 0x4154454D, k_Flow = 0x574F4C46, k_Cntr = 0x52544E43, k_CVal = 0x4C415643, k_Stak = 0x4B415453,
               k_SStk = 0x4B545353;

    public static string Extract(string capture, string output)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        if (!File.Exists(capture))
            return Error("no such capture: " + capture);
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling; try again when it finishes");

        var session = SaveUserSession();
        var wasEnabled = ProfilerDriver.enabled;
        ProfilerDriver.enabled = false;
        try
        {
            if (!ProfilerDriver.LoadProfile(Path.GetFullPath(capture), false))
                return Error("the Profiler could not load " + capture + "; is it a complete capture? A player may still be writing it, or it was cut short");
            var loadMs = watch.ElapsedMilliseconds;
            var stats = new Writer().Write(capture, output);
            SessionState.SetString(k_LoadedKey, Range());
            return "{\"ok\":true,\"loadMs\":" + loadMs + ",\"totalMs\":" + watch.ElapsedMilliseconds + "," + stats + "}";
        }
        catch (Exception e)
        {
            return Error(e.GetType().Name + ": " + e.Message);
        }
        finally
        {
            RestoreUserSession(session);
            ProfilerDriver.enabled = wasEnabled;
        }
    }

    public static string Convert(string capture, string output)
    {
        if (!File.Exists(capture))
            return Error("no such capture: " + capture);
        var session = SaveUserSession();
        var wasEnabled = ProfilerDriver.enabled;
        ProfilerDriver.enabled = false;
        try
        {
            if (!ProfilerDriver.LoadProfile(Path.GetFullPath(capture), false))
                return Error("the Profiler could not load " + capture + "; is it a complete capture? A player may still be writing it, or it was cut short");
            var frames = ProfilerDriver.lastFrameIndex - ProfilerDriver.firstFrameIndex + 1;
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            if (!ProfilerDriver.SaveProfile(Path.GetFullPath(output)))
                return Error("the Profiler could not save " + output);
            SessionState.SetString(k_LoadedKey, Range());
            return "{\"ok\":true,\"frames\":" + frames + "}";
        }
        finally
        {
            RestoreUserSession(session);
            ProfilerDriver.enabled = wasEnabled;
        }
    }

    // Unity's own Hierarchy view of some frames, for `verify` to compare the cache against. One line per item:
    // frame, thread id, thread name, path (names joined by \u001f), total ms, self ms, calls, GC bytes.
    public static string Hierarchy(string capture, string output, string frames)
    {
        var session = SaveUserSession();
        var wasEnabled = ProfilerDriver.enabled;
        ProfilerDriver.enabled = false;
        try
        {
            if (!ProfilerDriver.LoadProfile(Path.GetFullPath(capture), false))
                return Error("the Profiler could not load " + capture + "; is it a complete capture? A player may still be writing it, or it was cut short");
            SessionState.SetString(k_LoadedKey, Range());
            var children = new List<int>();
            var items = 0;
            using (var writer = new StreamWriter(output, false, new UTF8Encoding(false)))
            {
                foreach (var text in frames.Split(','))
                {
                    var frame = int.Parse(text, CultureInfo.InvariantCulture);
                    for (var thread = 0; ; thread++)
                    {
                        using var raw = ProfilerDriver.GetRawFrameDataView(frame, thread);
                        if (raw == null || !raw.valid)
                            break;
                        using var view = ProfilerDriver.GetHierarchyFrameDataView(frame, thread, HierarchyFrameDataView.ViewModes.Default,
                                                                                   HierarchyFrameDataView.columnDontSort, false);
                        var prefix = frame + "\t" + raw.threadId + "\t" + raw.threadGroupName + "/" + raw.threadName + "\t";
                        var stack = new Stack<(int id, string path)>();
                        stack.Push((view.GetRootItemID(), null));
                        while (stack.Count > 0)
                        {
                            var (id, path) = stack.Pop();
                            if (path != null)
                            {
                                writer.Write(prefix);
                                writer.Write(path);
                                writer.Write('\t');
                                writer.Write(view.GetItemColumnDataAsDouble(id, HierarchyFrameDataView.columnTotalTime).ToString("R", CultureInfo.InvariantCulture));
                                writer.Write('\t');
                                writer.Write(view.GetItemColumnDataAsDouble(id, HierarchyFrameDataView.columnSelfTime).ToString("R", CultureInfo.InvariantCulture));
                                writer.Write('\t');
                                writer.Write(view.GetItemColumnDataAsDouble(id, HierarchyFrameDataView.columnCalls).ToString("R", CultureInfo.InvariantCulture));
                                writer.Write('\t');
                                writer.Write(view.GetItemColumnDataAsDouble(id, HierarchyFrameDataView.columnGcMemory).ToString("R", CultureInfo.InvariantCulture));
                                writer.Write('\n');
                                items++;
                            }
                            view.GetItemChildren(id, children);
                            foreach (var child in children)
                                stack.Push((child, path == null ? view.GetItemName(child) : path + "\u001f" + view.GetItemName(child)));
                        }
                    }
                }
            }
            return "{\"ok\":true,\"items\":" + items + "}";
        }
        finally
        {
            RestoreUserSession(session);
            ProfilerDriver.enabled = wasEnabled;
        }
    }


    // Recording runs inside the Editor from an update callback, so the CLI never calls into the Editor while frames
    // are being recorded (every run_script compiles C#, which would land in the capture). The callback skips the
    // warm-up frames, stops after the requested count, saves, and writes <output>.status for the CLI to read.
    // The callback lives in this script's assembly; state shared with later calls goes through SessionState.
    const string k_RecordingKey = "ProfilerWaitress.Recording";

    public static string RecordStart(bool editor, bool deep, int callstacks, int warmup, int frames, string output)
    {
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling; try again when it finishes");
        if (deep != ProfilerDriver.deepProfiling)
            return Error(deep
                ? "deep profiling is off; it needs a domain reload to turn on. Turn it on in the Profiler window (or ProfilerDriver.deepProfiling = true) before entering Play Mode, then record again"
                : "deep profiling is on; pass --deep, or turn it off in the Profiler window first");
        if (!editor && !EditorApplication.isPlaying)
            return Error("not in Play Mode; enter Play Mode first, or pass --editor to profile the Editor itself");
        if (frames > MaxFrames())
            return Error("the Profiler keeps only its last " + MaxFrames() + " frames (Preferences > Analysis > Profiler > Frame Count, up to 2000); record fewer, or profile a player");

        output = Path.GetFullPath(output);
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        File.Delete(output + ".status");
        var session = SaveUserSession();
        var id = Guid.NewGuid().ToString("N");
        SessionState.SetString(k_RecordingKey, id);
        SessionState.SetString("ProfilerWaitress.RecordRestore", session ?? "");
        SessionState.SetInt("ProfilerWaitress.RecordMemoryMode", (int)ProfilerDriver.memoryRecordMode);
        SessionState.SetBool("ProfilerWaitress.RecordProfileEditor", ProfilerDriver.profileEditor);
        ProfilerDriver.enabled = false;
        ProfilerDriver.ClearAllFrames();
        ProfilerDriver.profileEditor = editor;
        ProfilerDriver.memoryRecordMode = (ProfilerMemoryRecordMode)callstacks;
        ProfilerDriver.enabled = true;
        SessionState.SetString(k_LoadedKey, "recording");
        new Recorder { Id = id, Editor = editor, Warmup = warmup, Frames = frames, Output = output }.Attach();
        return "{\"ok\":true}";
    }

    sealed class Recorder
    {
        public string Id;
        public bool Editor;
        public int Warmup;
        public int Frames;
        public string Output;
        bool m_Counting;

        public void Attach() => EditorApplication.update += Tick;

        void Tick()
        {
            // A newer recording, or an abort, replaces this one.
            if (SessionState.GetString(k_RecordingKey, "") != Id)
            {
                EditorApplication.update -= Tick;
                return;
            }
            var first = ProfilerDriver.firstFrameIndex;
            var have = first < 0 ? 0 : ProfilerDriver.lastFrameIndex - first + 1;
            if (!m_Counting)
            {
                if (have >= Warmup)
                {
                    ProfilerDriver.ClearAllFrames();
                    m_Counting = true;
                }
                return;
            }
            var stopped = !Editor && !EditorApplication.isPlaying;
            if (have < Frames && !stopped)
                return;
            EditorApplication.update -= Tick;
            SessionState.EraseString(k_RecordingKey);
            File.WriteAllText(Output + ".status", Finish(Output, stopped ? "Play Mode ended early" : null));
        }
    }

    static string Finish(string output, string note)
    {
        ProfilerDriver.enabled = false;
        ProfilerDriver.memoryRecordMode = (ProfilerMemoryRecordMode)SessionState.GetInt("ProfilerWaitress.RecordMemoryMode", 0);
        ProfilerDriver.profileEditor = SessionState.GetBool("ProfilerWaitress.RecordProfileEditor", false);
        var first = ProfilerDriver.firstFrameIndex;
        string result;
        if (first < 0)
            result = Error("nothing was recorded");
        else if (!ProfilerDriver.SaveProfile(output))
            result = Error("the Profiler could not save " + output);
        else
        {
            SessionState.SetString(k_LoadedKey, Range());
            result = "{\"ok\":true,\"frames\":" + (ProfilerDriver.lastFrameIndex - first + 1) + (note != null ? ",\"note\":\"" + Escape(note) + "\"" : "") + "}";
        }
        var restore = SessionState.GetString("ProfilerWaitress.RecordRestore", "");
        SessionState.EraseString("ProfilerWaitress.RecordRestore");
        if (restore.Length > 0)
            RestoreUserSession(restore);
        return result;
    }

    public static string RecordStatus()
    {
        var first = ProfilerDriver.firstFrameIndex;
        var frames = first < 0 ? 0 : ProfilerDriver.lastFrameIndex - first + 1;
        return "{\"ok\":true,\"recording\":" + (SessionState.GetString(k_RecordingKey, "").Length > 0 ? "true" : "false") + ",\"frames\":" + frames +
               ",\"playing\":" + (EditorApplication.isPlaying ? "true" : "false") + "}";
    }

    // Stops a recording whose callback is gone (a domain reload) or that the CLI gave up on, keeping what it has.
    public static string RecordAbort(string output)
    {
        if (SessionState.GetString(k_RecordingKey, "").Length == 0 && !ProfilerDriver.enabled)
            return Error("no recording is running");
        SessionState.EraseString(k_RecordingKey);
        return Finish(Path.GetFullPath(output), "stopped early");
    }

    public static string SetPlayMode(bool playing)
    {
        EditorApplication.isPlaying = playing;
        return "{\"ok\":true}";
    }

    static int MaxFrames() => EditorPrefs.GetInt("Profiler.FrameCount", 300);

    static string Range() => ProfilerDriver.firstFrameIndex + ".." + ProfilerDriver.lastFrameIndex;

    // Frames the user recorded themselves are saved before a capture replaces them, and loaded back afterwards.
    // Frames this tool loaded last time are not worth keeping.
    static string SaveUserSession()
    {
        if (ProfilerDriver.firstFrameIndex < 0 || ProfilerDriver.lastFrameIndex < ProfilerDriver.firstFrameIndex)
            return null;
        if (SessionState.GetString(k_LoadedKey, "") == Range())
            return null;
        var path = Path.GetFullPath(Path.Combine("UserSettings", "Waitress", "ProfilerWaitress", "user-session.data"));
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        return ProfilerDriver.SaveProfile(path) ? path : null;
    }

    static void RestoreUserSession(string path)
    {
        if (path == null || !File.Exists(path))
            return;
        ProfilerDriver.LoadProfile(path, false);
        SessionState.EraseString(k_LoadedKey);
        File.Delete(path);
    }

    static string Error(string message) => "{\"ok\":false,\"error\":\"" + Escape(message) + "\"}";

    static string Escape(string s)
    {
        var sb = new StringBuilder(s.Length + 8);
        foreach (var c in s)
        {
            if (c == '"' || c == '\\')
                sb.Append('\\').Append(c);
            else if (c < ' ')
                sb.Append("\\u").Append(((int)c).ToString("x4"));
            else
                sb.Append(c);
        }
        return sb.ToString();
    }

    sealed class Growable<T>
    {
        public T[] Items = new T[1 << 16];
        public int Count;

        public void Add(T value)
        {
            if (Count == Items.Length)
                Array.Resize(ref Items, Items.Length * 2);
            Items[Count++] = value;
        }
    }

    sealed class Writer
    {
        readonly List<string> m_Strings = new List<string>();
        readonly Dictionary<string, int> m_StringIds = new Dictionary<string, int>();
        readonly Dictionary<int, int> m_MarkerLocal = new Dictionary<int, int>();
        readonly List<int> m_MarkerUnity = new List<int>();
        readonly Dictionary<string, int> m_ThreadLocal = new Dictionary<string, int>();
        readonly List<(ulong id, int group, int name)> m_Threads = new List<(ulong, int, int)>();
        readonly Dictionary<int, FrameDataView.MarkerMetadataInfo[]> m_MetaInfo = new Dictionary<int, FrameDataView.MarkerMetadataInfo[]>();

        readonly Growable<int> m_SMarker = new Growable<int>();
        readonly Growable<ushort> m_SDepth = new Growable<ushort>();
        readonly Growable<uint> m_SStart = new Growable<uint>();
        readonly Growable<uint> m_SDur = new Growable<uint>();
        readonly MemoryStream m_Blocks = new MemoryStream();
        readonly MemoryStream m_Frames = new MemoryStream();
        readonly MemoryStream m_Meta = new MemoryStream();
        readonly MemoryStream m_Flows = new MemoryStream();
        readonly MemoryStream m_SampleStacks = new MemoryStream();
        readonly List<int[]> m_Stacks = new List<int[]>();
        readonly Dictionary<string, int> m_StackIds = new Dictionary<string, int>();
        readonly Dictionary<ulong, int> m_MethodNames = new Dictionary<ulong, int>();
        int m_MetaCount, m_FlowCount, m_SampleStackCount, m_BlockCount;

        ulong[] m_Start = new ulong[4096];
        ulong[] m_Dur = new ulong[4096];

        int Str(string s)
        {
            s ??= "";
            if (!m_StringIds.TryGetValue(s, out var id))
            {
                id = m_Strings.Count;
                m_Strings.Add(s);
                m_StringIds.Add(s, id);
            }
            return id;
        }

        int Marker(int unityId)
        {
            if (!m_MarkerLocal.TryGetValue(unityId, out var local))
            {
                local = m_MarkerUnity.Count;
                m_MarkerUnity.Add(unityId);
                m_MarkerLocal.Add(unityId, local);
            }
            return local;
        }

        public string Write(string capture, string output)
        {
            int first = ProfilerDriver.firstFrameIndex, last = ProfilerDriver.lastFrameIndex;
            var frameWriter = new BinaryWriter(m_Frames);
            var blockWriter = new BinaryWriter(m_Blocks);
            var metaWriter = new BinaryWriter(m_Meta);
            var flowWriter = new BinaryWriter(m_Flows);
            var stackWriter = new BinaryWriter(m_SampleStacks);
            var flows = new List<RawFrameDataView.FlowEvent>();
            var callstack = new List<ulong>();
            var ends = new int[256];
            var counterIds = new List<int>();
            var counterTypes = new List<byte>();
            var counterUnits = new List<byte>();
            var counterValues = new List<double[]>();
            int frameCount = last - first + 1;
            FrameDataView anyView = null;

            for (var frame = first; frame <= last; frame++)
            {
                var blockFirst = m_BlockCount;
                ulong frameStart = 0, frameTime = 0, gpuTime = 0;
                float fps = 0;
                for (var thread = 0; ; thread++)
                {
                    var view = ProfilerDriver.GetRawFrameDataView(frame, thread);
                    if (view == null || !view.valid)
                    {
                        view?.Dispose();
                        break;
                    }
                    if (thread == 0)
                    {
                        frameStart = view.frameStartTimeNs;
                        frameTime = view.frameTimeNs;
                        gpuTime = view.frameGpuTimeNs;
                        fps = view.frameFps;
                        if (counterValues.Count == 0 && frame == first)
                            CollectCounters(view, counterIds, counterTypes, counterUnits, counterValues, frameCount);
                        for (var c = 0; c < counterIds.Count; c++)
                        {
                            var id = counterIds[c];
                            counterValues[c][frame - first] = view.HasCounterValue(id) ? view.GetCounterValueAsDouble(id) : double.NaN;
                        }
                    }
                    WriteBlock(view, frame - first, blockWriter, metaWriter, flowWriter, stackWriter, flows, callstack, ref ends);
                    if (anyView == null)
                        anyView = view;
                    else
                        view.Dispose();
                }
                frameWriter.Write(frameStart);
                frameWriter.Write(frameTime);
                frameWriter.Write(gpuTime);
                frameWriter.Write(fps);
                frameWriter.Write(blockFirst);
                frameWriter.Write(m_BlockCount - blockFirst);
            }

            // Marker names, categories and flags are session-wide, so the last valid view knows every one of them.
            var markerSection = new MemoryStream();
            var categoryNames = new Dictionary<ushort, int>();
            using (var view = ProfilerDriver.GetRawFrameDataView(last, 0))
            {
                var source = view != null && view.valid ? (FrameDataView)view : anyView;
                var categories = new List<ProfilerCategoryInfo>();
                source?.GetAllCategories(categories);
                foreach (var c in categories)
                    categoryNames[c.id] = Str(c.name);
                var w = new BinaryWriter(markerSection);
                w.Write(m_MarkerUnity.Count);
                foreach (var unityId in m_MarkerUnity)
                {
                    var name = source?.GetMarkerName(unityId) ?? "";
                    var category = source?.GetMarkerCategoryIndex(unityId) ?? 0;
                    w.Write(Str(name));
                    w.Write(categoryNames.TryGetValue(category, out var cn) ? cn : Str("category " + category));
                    w.Write((ushort)(source != null ? source.GetMarkerFlags(unityId) : 0));
                }
            }
            anyView?.Dispose();

            var counterSection = new MemoryStream();
            var counterValueSection = new MemoryStream();
            {
                var w = new BinaryWriter(counterSection);
                w.Write(counterIds.Count);
                for (var c = 0; c < counterIds.Count; c++)
                {
                    w.Write(Marker(counterIds[c]));
                    w.Write(counterTypes[c]);
                    w.Write(counterUnits[c]);
                }
                var v = new BinaryWriter(counterValueSection);
                foreach (var values in counterValues)
                    foreach (var value in values)
                        v.Write(value);
            }
            var head = new MemoryStream();
            {
                var w = new BinaryWriter(head);
                var info = new FileInfo(capture);
                w.Write(k_FormatVersion);
                w.Write(Str(Path.GetFullPath(capture).Replace('\\', '/')));
                w.Write(info.Length);
                w.Write(info.LastWriteTimeUtc.Ticks);
                w.Write(DateTime.UtcNow.Ticks);
                w.Write(Str(Application.unityVersion));
                w.Write(first);
                w.Write(frameCount);
                w.Write(EditorPrefs.GetInt("Profiler.FrameCount", 300));
            }

            var threadSection = new MemoryStream();
            {
                var w = new BinaryWriter(threadSection);
                w.Write(m_Threads.Count);
                foreach (var t in m_Threads)
                {
                    w.Write(t.id);
                    w.Write(t.group);
                    w.Write(t.name);
                }
            }

            var stackSection = new MemoryStream();
            {
                var w = new BinaryWriter(stackSection);
                w.Write(m_Stacks.Count);
                foreach (var stack in m_Stacks)
                {
                    w.Write(stack.Length);
                    foreach (var s in stack)
                        w.Write(s);
                }
            }

            // Strings last: every other section may still add to the pool.
            var stringSection = new MemoryStream();
            {
                var w = new BinaryWriter(stringSection, new UTF8Encoding(false));
                w.Write(m_Strings.Count);
                foreach (var s in m_Strings)
                    w.Write(s);
            }

            var sampleCount = m_SMarker.Count;
            var markerColumn = new byte[(long)sampleCount * 4];
            Buffer.BlockCopy(m_SMarker.Items, 0, markerColumn, 0, markerColumn.Length);
            var depthColumn = new byte[(long)sampleCount * 2];
            Buffer.BlockCopy(m_SDepth.Items, 0, depthColumn, 0, depthColumn.Length);
            var startColumn = new byte[(long)sampleCount * 4];
            Buffer.BlockCopy(m_SStart.Items, 0, startColumn, 0, startColumn.Length);
            var durColumn = new byte[(long)sampleCount * 4];
            Buffer.BlockCopy(m_SDur.Items, 0, durColumn, 0, durColumn.Length);

            var sections = new List<(uint tag, byte[] data)>
            {
                (k_Head, head.ToArray()),
                (k_Strs, stringSection.ToArray()),
                (k_Mrkr, markerSection.ToArray()),
                (k_Thrd, threadSection.ToArray()),
                (k_Fram, m_Frames.ToArray()),
                (k_Blck, m_Blocks.ToArray()),
                (k_SMrk, markerColumn),
                (k_SDep, depthColumn),
                (k_SSta, startColumn),
                (k_SDur, durColumn),
                (k_Meta, m_Meta.ToArray()),
                (k_Flow, m_Flows.ToArray()),
                (k_Cntr, counterSection.ToArray()),
                (k_CVal, counterValueSection.ToArray()),
                (k_Stak, stackSection.ToArray()),
                (k_SStk, m_SampleStacks.ToArray()),
            };

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            var temp = output + ".partial";
            using (var file = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20))
            {
                var w = new BinaryWriter(file);
                w.Write(0x31585750u); // "PWX1"
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
            }
            if (File.Exists(output))
                File.Delete(output);
            File.Move(temp, output);

            return "\"frames\":" + frameCount + ",\"threads\":" + m_Threads.Count + ",\"samples\":" + sampleCount +
                   ",\"markers\":" + m_MarkerUnity.Count + ",\"counters\":" + counterIds.Count + ",\"metadata\":" + m_MetaCount +
                   ",\"flows\":" + m_FlowCount + ",\"callstacks\":" + m_SampleStackCount + ",\"bytes\":" + new FileInfo(output).Length;
        }

        void CollectCounters(RawFrameDataView view, List<int> ids, List<byte> types, List<byte> units, List<double[]> values, int frameCount)
        {
            var markers = new List<FrameDataView.MarkerInfo>();
            view.GetMarkers(markers);
            foreach (var m in markers)
            {
                if ((m.flags & MarkerFlags.Counter) == 0)
                    continue;
                ids.Add(m.id);
                var info = view.GetMarkerMetadataInfo(m.id);
                types.Add(info != null && info.Length > 0 ? (byte)info[0].type : (byte)0);
                units.Add(info != null && info.Length > 0 ? (byte)info[0].unit : (byte)0);
                values.Add(new double[frameCount]);
                Marker(m.id);
            }
        }

        void WriteBlock(RawFrameDataView view, int frame, BinaryWriter blocks, BinaryWriter meta, BinaryWriter flowOut,
                        BinaryWriter stackOut, List<RawFrameDataView.FlowEvent> flows, List<ulong> callstack, ref int[] ends)
        {
            var key = view.threadId.ToString(CultureInfo.InvariantCulture) + "|" + view.threadGroupName + "|" + view.threadName;
            if (!m_ThreadLocal.TryGetValue(key, out var thread))
            {
                thread = m_Threads.Count;
                m_Threads.Add((view.threadId, Str(view.threadGroupName), Str(view.threadName)));
                m_ThreadLocal.Add(key, thread);
            }

            var count = view.sampleCount;
            if (m_Start.Length < count)
            {
                m_Start = new ulong[Math.Max(count, m_Start.Length * 2)];
                m_Dur = new ulong[m_Start.Length];
            }
            ulong min = ulong.MaxValue, max = 0;
            for (var s = 0; s < count; s++)
            {
                var start = view.GetSampleStartTimeNs(s);
                var dur = view.GetSampleTimeNs(s);
                m_Start[s] = start;
                m_Dur[s] = dur;
                if (start < min)
                    min = start;
            }
            if (count == 0)
                min = view.frameStartTimeNs;
            for (var s = 0; s < count; s++)
            {
                var span = Math.Max(m_Start[s] - min, m_Dur[s]);
                if (span > max)
                    max = span;
            }
            byte shift = 0;
            while ((max >> shift) > uint.MaxValue)
                shift++;

            var sampleBase = (long)m_SMarker.Count;
            blocks.Write(thread);
            blocks.Write(frame);
            blocks.Write(sampleBase);
            blocks.Write(count);
            blocks.Write(min);
            blocks.Write(shift);
            m_BlockCount++;

            var depth = 0;
            for (var s = 0; s < count; s++)
            {
                while (depth > 0 && s >= ends[depth - 1])
                    depth--;
                var unityMarker = view.GetSampleMarkerId(s);
                m_SMarker.Add(Marker(unityMarker));
                m_SDepth.Add((ushort)Math.Min(depth, ushort.MaxValue));
                m_SStart.Add((uint)((m_Start[s] - min) >> shift));
                m_SDur.Add((uint)(m_Dur[s] >> shift));
                var children = view.GetSampleChildrenCountRecursive(s);
                if (children > 0)
                {
                    if (depth == ends.Length)
                        Array.Resize(ref ends, ends.Length * 2);
                    ends[depth++] = s + 1 + children;
                }

                var metaCount = view.GetSampleMetadataCount(s);
                if (metaCount > 0)
                    WriteMetadata(view, s, unityMarker, metaCount, sampleBase + s, meta);

                view.GetSampleCallstack(s, callstack);
                if (callstack.Count > 0)
                {
                    stackOut.Write(sampleBase + s);
                    stackOut.Write(StackId(view, callstack));
                    m_SampleStackCount++;
                }
            }

            view.GetFlowEvents(flows);
            foreach (var f in flows)
            {
                if (f.ParentSampleIndex < 0 || f.ParentSampleIndex >= count)
                    continue;
                flowOut.Write(sampleBase + f.ParentSampleIndex);
                flowOut.Write(f.FlowId);
                flowOut.Write((byte)f.FlowEventType);
                m_FlowCount++;
            }
        }

        void WriteMetadata(RawFrameDataView view, int sample, int unityMarker, int metaCount, long globalSample, BinaryWriter meta)
        {
            if (!m_MetaInfo.TryGetValue(unityMarker, out var info))
            {
                info = view.GetMarkerMetadataInfo(unityMarker);
                m_MetaInfo.Add(unityMarker, info);
            }
            for (var i = 0; i < metaCount; i++)
            {
                var type = info != null && i < info.Length ? info[i].type : ProfilerMarkerDataType.Int64;
                byte kind;
                long bits;
                switch (type)
                {
                    case ProfilerMarkerDataType.Int32:
                    case ProfilerMarkerDataType.UInt32:
                    case ProfilerMarkerDataType.Int64:
                    case ProfilerMarkerDataType.UInt64:
                        kind = 0;
                        bits = view.GetSampleMetadataAsLong(sample, i);
                        break;
                    case ProfilerMarkerDataType.Float:
                    case ProfilerMarkerDataType.Double:
                        kind = 1;
                        bits = BitConverter.DoubleToInt64Bits(view.GetSampleMetadataAsDouble(sample, i));
                        break;
                    case ProfilerMarkerDataType.String16:
                        kind = 2;
                        bits = Str(view.GetSampleMetadataAsString(sample, i));
                        break;
                    case ProfilerMarkerDataType.InstanceId:
                        kind = 2;
                        bits = Str(ObjectName(view, view.GetSampleMetadataAsInt(sample, i)));
                        break;
                    default:
                        continue; // Binary blobs (the job system's) have no public layout.
                }
                meta.Write(globalSample);
                meta.Write((byte)i);
                meta.Write(kind);
                meta.Write(info != null && i < info.Length ? Str(info[i].name) : Str(""));
                meta.Write((byte)(info != null && i < info.Length ? info[i].unit : 0));
                meta.Write(bits);
                m_MetaCount++;
            }
        }

        static string ObjectName(RawFrameDataView view, int instanceId)
        {
#pragma warning disable CS0618
            if (!view.GetUnityObjectInfo(instanceId, out var info))
#pragma warning restore CS0618
                return "instance " + instanceId;
            return view.GetUnityObjectNativeTypeInfo(info.nativeTypeIndex, out var type) ? info.name + " (" + type.name + ")" : info.name;
        }

        int StackId(RawFrameDataView view, List<ulong> callstack)
        {
            var frames = new int[callstack.Count];
            for (var i = 0; i < callstack.Count; i++)
            {
                var address = callstack[i];
                if (!m_MethodNames.TryGetValue(address, out var name))
                {
                    var method = view.ResolveMethodInfo(address);
                    // Native frames do not resolve; they are stored empty and skipped by readers.
                    var text = method.methodName ?? "";
                    if (text.Length > 0 && !string.IsNullOrEmpty(method.sourceFileName))
                        text += " (" + Path.GetFileName(method.sourceFileName) + ":" + method.sourceFileLine + ")";
                    name = Str(text);
                    m_MethodNames.Add(address, name);
                }
                frames[i] = name;
            }
            var key = string.Join(",", frames);
            if (!m_StackIds.TryGetValue(key, out var id))
            {
                id = m_Stacks.Count;
                m_Stacks.Add(frames);
                m_StackIds.Add(key, id);
            }
            return id;
        }
    }
}
