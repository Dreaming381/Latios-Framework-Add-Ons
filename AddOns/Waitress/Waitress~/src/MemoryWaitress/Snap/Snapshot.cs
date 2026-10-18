using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryWaitress.Snap
{
    [Flags]
    public enum TypeFlags
    {
        ValueType = 1 << 0,
        Array = 1 << 1,
    }

    /// <summary>Everything a .snap file holds, as parallel arrays, plus the managed objects found by crawling it.</summary>
    public sealed class Snapshot
    {
        public const uint MinVersion = 11;

        public string Path;
        public uint Version;
        public DateTime Date;
        public uint CaptureFlags;
        public bool IsEditor;
        public string Platform;
        public string UnityVersion;
        public string ProductName;
        public string Description;
        public string GraphicsApi;
        public uint SessionGuid;
        public ulong TotalPhysical;
        public ulong TotalGraphics;
        public double TimeSinceStartup;
        public Stats Stats;

        public VmInfo Vm;

        public string[] NativeTypeName;
        public int[] NativeTypeBase;

        public int[] ObjType;
        public uint[] ObjHideFlags;
        public int[] ObjFlags;
        public long[] ObjInstanceId;
        public string[] ObjName;
        public ulong[] ObjAddress;
        public ulong[] ObjSize;
        public long[] ObjRootId;
        public int[] ObjGcHandle;

        public ulong[] GcHandleTarget;
        public long[] ConnFrom;
        public long[] ConnTo;

        public HeapSection[] Heap;

        public int[] TypeFlagBits;
        public string[] TypeName;
        public string[] TypeAssembly;
        public int[][] TypeFields;
        public byte[][] TypeStatics;
        public int[] TypeBaseOrElement;
        public int[] TypeSize;
        public ulong[] TypeInfoAddress;

        public int[] FieldOffset;
        public int[] FieldType;
        public string[] FieldName;
        public bool[] FieldStatic;

        public long[] RootId;
        public string[] RootArea;
        public string[] RootObject;
        public ulong[] RootSize;
        public Dictionary<long, int> RootIndexById;

        public long[] AllocRootId;
        public ulong[] AllocSize;
        public int[] AllocRegion;

        public string[] RegionName;
        public int[] RegionParent;
        public ulong[] RegionSize;

        public string[] LabelName;
        public ulong[] LabelSize;

        public ulong[] GfxSize;
        public long[] GfxRootId;

        public string[] AllocatorName;
        public ulong[] AllocatorUsed;
        public ulong[] AllocatorReserved;

        public ulong[] SystemSize;
        public ulong[] SystemResident;
        public ushort[] SystemType;
        public string[] SystemName;

        public string[] SceneName;
        public string[] ScenePath;

        public Crawl Managed;
        public double LoadSeconds;
        public double CrawlSeconds;

        public bool IsValueType(int t) => (TypeFlagBits[t] & (int)TypeFlags.ValueType) != 0;
        public bool IsArray(int t) => (TypeFlagBits[t] & (int)TypeFlags.Array) != 0;
        public int ArrayRank(int t) => (int)((uint)TypeFlagBits[t] >> 16);

        public static Snapshot Load(string path, bool crawl = true)
        {
            var clock = Stopwatch.StartNew();
            using var f = new SnapFile(path);
            if (f.Version < MinVersion)
                throw new WaitressException($"{path} is snapshot format {f.Version}; MemoryWaitress reads {MinVersion} and newer (Unity 2021.2+)");
            var s = new Snapshot { Path = path, Version = f.Version };
            s.Date = new DateTime(f.Single<long>(Chapter.RecordDate), DateTimeKind.Utc).ToLocalTime();
            s.CaptureFlags = f.Single<uint>(Chapter.CaptureFlags);
            s.ReadMetadata(f);

            var vm = f.SingleBytes(Chapter.VirtualMachineInformation);
            uint U(int i) => i * 4 + 4 <= vm.Length ? BitConverter.ToUInt32(vm, i * 4) : 0;
            s.Vm = new VmInfo { PointerSize = U(0), ObjectHeaderSize = U(1), ArrayHeaderSize = U(2), ArrayBoundsOffset = U(3), ArraySizeOffset = U(4), AllocationGranularity = U(5) };
            if (s.Vm.PointerSize != 8 && s.Vm.PointerSize != 4)
                throw new WaitressException($"{path}: unexpected pointer size {s.Vm.PointerSize}");

            s.NativeTypeName = f.Strings(Chapter.NativeTypes_Name);
            s.NativeTypeBase = f.Array<int>(Chapter.NativeTypes_BaseType);

            s.ObjType = f.Array<int>(Chapter.NativeObjects_Type);
            s.ObjHideFlags = f.Array<uint>(Chapter.NativeObjects_HideFlags);
            s.ObjFlags = f.Array<int>(Chapter.NativeObjects_Flags);
            s.ObjInstanceId = InstanceIds(f, Chapter.NativeObjects_InstanceId);
            s.ObjName = f.Strings(Chapter.NativeObjects_Name);
            s.ObjAddress = f.Array<ulong>(Chapter.NativeObjects_Address);
            s.ObjSize = f.Array<ulong>(Chapter.NativeObjects_Size);
            s.ObjRootId = f.Array<long>(Chapter.NativeObjects_RootId);
            s.ObjGcHandle = f.Array<int>(Chapter.NativeObjects_GCHandleIndex);

            s.GcHandleTarget = f.Array<ulong>(Chapter.GCHandles_Target);
            s.ConnFrom = InstanceIds(f, Chapter.Connections_From);
            s.ConnTo = InstanceIds(f, Chapter.Connections_To);

            var starts = f.Array<ulong>(Chapter.ManagedHeap_Start);
            var bytes = f.Blobs(Chapter.ManagedHeap_Bytes);
            const ulong vmBit = 1UL << 63;
            s.Heap = starts.Select((a, i) => new HeapSection { Start = a & ~vmBit, IsVm = (a & vmBit) != 0, Bytes = bytes[i] })
                .Where(h => h.Bytes.Length > 0).OrderBy(h => h.Start).ToArray();

            s.TypeFlagBits = f.Array<int>(Chapter.Types_Flags);
            s.TypeName = f.Strings(Chapter.Types_Name);
            s.TypeAssembly = f.Strings(Chapter.Types_Assembly);
            s.TypeFields = f.Arrays<int>(Chapter.Types_FieldIndices);
            s.TypeStatics = f.Arrays<byte>(Chapter.Types_StaticFieldBytes);
            s.TypeBaseOrElement = f.Array<int>(Chapter.Types_BaseOrElement);
            s.TypeSize = f.Array<int>(Chapter.Types_Size);
            s.TypeInfoAddress = f.Array<ulong>(Chapter.Types_TypeInfoAddress);
            for (var t = 0; t < s.TypeSize.Length; t++)
            {
                // Capture code subtracts the header from every type, including ones that never had one.
                if (s.TypeSize[t] < 0 && s.TypeBaseOrElement[t] < 0)
                    s.TypeSize[t] += (int)s.Vm.ObjectHeaderSize;
            }

            s.FieldOffset = f.Array<int>(Chapter.Fields_Offset);
            s.FieldType = f.Array<int>(Chapter.Fields_Type);
            s.FieldName = f.Strings(Chapter.Fields_Name);
            s.FieldStatic = f.Array<byte>(Chapter.Fields_IsStatic).Select(b => b != 0).ToArray();

            s.RootId = f.Array<long>(Chapter.Roots_Id);
            s.RootArea = f.Strings(Chapter.Roots_AreaName);
            s.RootObject = f.Strings(Chapter.Roots_ObjectName);
            s.RootSize = f.Array<ulong>(Chapter.Roots_AccumulatedSize);
            s.RootIndexById = new Dictionary<long, int>(s.RootId.Length);
            for (var i = 0; i < s.RootId.Length; i++)
                s.RootIndexById[s.RootId[i]] = i;

            s.AllocRootId = f.Array<long>(Chapter.Allocations_RootId);
            s.AllocSize = f.Array<ulong>(Chapter.Allocations_Size);
            s.AllocRegion = f.Array<int>(Chapter.Allocations_RegionIndex);
            s.RegionName = f.Strings(Chapter.Regions_Name);
            s.RegionParent = f.Array<int>(Chapter.Regions_Parent);
            s.RegionSize = f.Array<ulong>(Chapter.Regions_Size);
            s.LabelName = f.Strings(Chapter.Labels_Name);
            s.LabelSize = f.Array<ulong>(Chapter.Labels_Size);

            s.GfxSize = f.Array<ulong>(Chapter.Gfx_Size);
            s.GfxRootId = f.Array<long>(Chapter.Gfx_RootId);

            s.AllocatorName = f.Strings(Chapter.Allocators_Name);
            s.AllocatorUsed = f.Array<ulong>(Chapter.Allocators_Used);
            s.AllocatorReserved = f.Array<ulong>(Chapter.Allocators_Reserved);

            s.SystemSize = f.Array<ulong>(Chapter.System_Size);
            s.SystemResident = f.Array<ulong>(Chapter.System_Resident);
            s.SystemType = f.Array<ushort>(Chapter.System_Type);
            s.SystemName = f.Strings(Chapter.System_Name);

            s.SceneName = f.Strings(Chapter.Scenes_Name);
            s.ScenePath = f.Strings(Chapter.Scenes_Path);
            s.LoadSeconds = clock.Elapsed.TotalSeconds;

            if (crawl)
            {
                clock.Restart();
                s.Managed = Crawl.Run(s);
                s.CrawlSeconds = clock.Elapsed.TotalSeconds;
            }
            return s;
        }

        static long[] InstanceIds(SnapFile f, Chapter c)
        {
            if (f.ElementSize(c) == 4)
                return f.Array<int>(c).Select(i => (long)i).ToArray();
            return f.Array<long>(c);
        }

        void ReadMetadata(SnapFile f)
        {
            var user = f.SingleBytes(Chapter.UserMetadata);
            if (user.Length >= 4)
            {
                var length = BitConverter.ToInt32(user, 0);
                if (length > 0 && 4 + length * 2 <= user.Length)
                    Description = Encoding.Unicode.GetString(user, 4, length * 2);
            }
            var info = f.SingleBytes(Chapter.TargetInfo);
            if (info.Length >= 328)
            {
                SessionGuid = BitConverter.ToUInt32(info, 0);
                Platform = PlatformName(BitConverter.ToInt32(info, 4));
                GraphicsApi = GraphicsName(BitConverter.ToInt32(info, 8));
                TotalPhysical = BitConverter.ToUInt64(info, 16);
                TotalGraphics = BitConverter.ToUInt64(info, 24);
                TimeSinceStartup = BitConverter.ToDouble(info, 40);
                UnityVersion = Encoding.UTF8.GetString(info, 52, (int)Math.Min(16, BitConverter.ToUInt32(info, 48)));
                ProductName = Encoding.UTF8.GetString(info, 72, (int)Math.Min(256, BitConverter.ToUInt32(info, 68)));
                IsEditor = Platform.Contains("Editor");
            }
            var stats = f.SingleBytes(Chapter.MemoryStats);
            if (stats.Length >= 96)
            {
                ulong S(int i) => BitConverter.ToUInt64(stats, i * 8);
                Stats = new Stats
                {
                    Valid = true,
                    TotalVirtual = S(0), TotalUsed = S(1), TotalReserved = S(2), TempAllocator = S(3), Graphics = S(4), Audio = S(5),
                    GcHeapUsed = S(6), GcHeapReserved = S(7), ProfilerUsed = S(8), ProfilerReserved = S(9),
                    MemoryProfilerUsed = S(10), MemoryProfilerReserved = S(11),
                };
            }
        }

        static string PlatformName(int p) => p switch
        {
            0 => "OSXEditor", 1 => "OSXPlayer", 2 => "WindowsPlayer", 7 => "WindowsEditor", 8 => "IPhonePlayer", 11 => "Android",
            13 => "LinuxPlayer", 16 => "LinuxEditor", 17 => "WebGLPlayer", 18 => "WSAPlayerX86", 19 => "WSAPlayerX64", 20 => "WSAPlayerARM",
            25 => "PS4", 27 => "XboxOne", 31 => "tvOS", 32 => "Switch", 36 => "GameCoreXboxSeries", 37 => "GameCoreXboxOne", 38 => "PS5",
            44 => "LinuxServer", 45 => "WindowsServer", 46 => "OSXServer",
            _ => "platform " + p,
        };

        static string GraphicsName(int g) => g switch
        {
            2 => "Direct3D11", 4 => "Null", 8 => "OpenGLES2", 11 => "OpenGLES3", 13 => "PlayStation4", 14 => "XboxOne", 16 => "Metal", 17 => "OpenGLCore",
            18 => "Direct3D12", 21 => "Vulkan", 22 => "Switch", 23 => "XboxOneD3D12", 24 => "GameCoreXboxOne", 25 => "GameCoreXboxSeries", 26 => "PlayStation5",
            27 => "PlayStation5NGGC", 29 => "WebGPU",
            _ => "device " + g,
        };

        /// <summary>The native object that owns a root id, or -1.</summary>
        public Dictionary<long, int> ObjectByRootId()
        {
            var map = new Dictionary<long, int>(ObjRootId.Length);
            for (var i = 0; i < ObjRootId.Length; i++)
                map.TryAdd(ObjRootId[i], i);
            return map;
        }

        /// <summary>Graphics memory per root id.</summary>
        public Dictionary<long, ulong> GfxByRootId()
        {
            var map = new Dictionary<long, ulong>();
            for (var i = 0; i < GfxSize.Length; i++)
            {
                map.TryGetValue(GfxRootId[i], out var v);
                map[GfxRootId[i]] = v + GfxSize[i];
            }
            return map;
        }

        public string NativeTypeOf(int obj) => ObjType[obj] >= 0 && ObjType[obj] < NativeTypeName.Length ? NativeTypeName[ObjType[obj]] : "?";
    }

    public struct VmInfo
    {
        public uint PointerSize;
        public uint ObjectHeaderSize;
        public uint ArrayHeaderSize;
        public uint ArrayBoundsOffset;
        public uint ArraySizeOffset;
        public uint AllocationGranularity;
    }

    public struct Stats
    {
        public bool Valid;
        public ulong TotalVirtual, TotalUsed, TotalReserved, TempAllocator, Graphics, Audio, GcHeapUsed, GcHeapReserved;
        public ulong ProfilerUsed, ProfilerReserved, MemoryProfilerUsed, MemoryProfilerReserved;
    }

    public sealed class HeapSection
    {
        public ulong Start;
        public bool IsVm;
        public byte[] Bytes;
        public ulong End => Start + (ulong)Bytes.Length;
    }
}
