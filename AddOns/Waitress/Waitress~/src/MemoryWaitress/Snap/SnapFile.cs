using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace MemoryWaitress.Snap
{
    /// <summary>Chapter ids of the Memory Profiler snapshot format. The values are fixed by the file format.</summary>
    public enum Chapter : ushort
    {
        Version = 0,
        RecordDate,
        UserMetadata,
        CaptureFlags,
        VirtualMachineInformation,
        NativeTypes_Name,
        NativeTypes_BaseType,
        NativeObjects_Type,
        NativeObjects_HideFlags,
        NativeObjects_Flags,
        NativeObjects_InstanceId,
        NativeObjects_Name,
        NativeObjects_Address,
        NativeObjects_Size,
        NativeObjects_RootId,
        GCHandles_Target,
        Connections_From,
        Connections_To,
        ManagedHeap_Start,
        ManagedHeap_Bytes,
        ManagedStacks_Start,
        ManagedStacks_Bytes,
        Types_Flags,
        Types_Name,
        Types_Assembly,
        Types_FieldIndices,
        Types_StaticFieldBytes,
        Types_BaseOrElement,
        Types_Size,
        Types_TypeInfoAddress,
        Types_TypeIndex,
        Fields_Offset,
        Fields_Type,
        Fields_Name,
        Fields_IsStatic,
        Roots_Id,
        Roots_AreaName,
        Roots_ObjectName,
        Roots_AccumulatedSize,
        Allocations_RegionIndex,
        Allocations_RootId,
        Allocations_SiteId,
        Allocations_Address,
        Allocations_Size,
        Allocations_Overhead,
        Allocations_Padding,
        Regions_Name,
        Regions_Parent,
        Regions_Address,
        Regions_Size,
        Regions_FirstAllocation,
        Regions_AllocationCount,
        Labels_Name,
        Sites_Id,
        Sites_Label,
        Sites_Callstack,
        Symbols_Symbol,
        Symbols_Text,
        NativeObjects_GCHandleIndex,
        TargetInfo,
        MemoryStats,
        Labels_Size,
        Scenes_Name,
        Scenes_Path,
        Scenes_AssetPath,
        Scenes_BuildIndex,
        Scenes_RootCounts,
        Scenes_RootOffsets,
        Scenes_RootIds,
        Labels_AllocatorId,
        Gfx_Id,
        Gfx_Size,
        Gfx_RootId,
        Allocators_Name,
        Allocators_Id,
        Allocators_Used,
        Allocators_Reserved,
        Allocators_Overhead,
        Allocators_Peak,
        Allocators_Count,
        Allocators_Flags,
        MetaData_Indices,
        MetaData_Buffer,
        System_Address,
        System_Size,
        System_Resident,
        System_Type,
        System_Name,
        Count,
    }

    /// <summary>
    /// Reads chapters out of a .snap file. A chapter is a single value, a fixed-size array or a
    /// variable-size array, stored in a block whose bytes are split into chunks scattered through the file.
    /// </summary>
    public sealed class SnapFile : IDisposable
    {
        const uint k_HeaderSignature = 0xAEABCDCD;
        const uint k_DirectorySignature = 0xCDCDAEAB;
        const uint k_FooterSignature = 0xABCDCDAE;
        const uint k_SectionVersion = 0x20170724;

        enum Format : ushort { Undefined = 0, Single, Fixed, Dynamic }

        struct Block
        {
            public long ChunkSize;
            public long[] Chunks;
        }

        struct Entry
        {
            public Format Format;
            public int Block;
            public uint EntriesMeta;
            public ulong HeaderMeta;
            /// <summary>Dynamic arrays: Count + 1 block offsets, the last one being the end.</summary>
            public long[] Offsets;

            public long Count => Format switch
            {
                Format.Single => 1,
                Format.Fixed => (long)HeaderMeta,
                Format.Dynamic => EntriesMeta,
                _ => 0,
            };
        }

        readonly SafeFileHandle m_File;
        readonly Block[] m_Blocks;
        readonly Entry[] m_Entries = new Entry[(int)Chapter.Count];

        public string Path { get; }
        public long Length { get; }
        public uint Version { get; }

        public SnapFile(string path)
        {
            Path = path;
            m_File = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.RandomAccess);
            try
            {
                Length = RandomAccess.GetLength(m_File);
                if (Length < 32 || U32(0) != k_HeaderSignature)
                    throw new WaitressException($"{path} is not a Memory Profiler snapshot (.snap)");
                if (U32(Length - 4) != k_FooterSignature)
                    throw new WaitressException($"{path} is an incomplete snapshot: it starts like one but has no directory at the end (cut short, or still being written)");
                var directory = I64(Length - 12);
                if (directory <= 0 || directory >= Length || U32(directory) != k_DirectorySignature || U32(directory + 4) != k_SectionVersion)
                    throw new WaitressException($"{path}: snapshot directory is damaged");
                var blockSection = I64(directory + 8);
                if (U32(blockSection) != k_SectionVersion)
                    throw new WaitressException($"{path}: snapshot block section is damaged");

                var blockCount = (int)U32(blockSection + 4);
                var blockOffsets = Read<long>(blockSection + 8, blockCount);
                m_Blocks = new Block[blockCount];
                for (var i = 0; i < blockCount; i++)
                {
                    var chunkSize = I64(blockOffsets[i]);
                    var total = I64(blockOffsets[i] + 8);
                    var chunks = chunkSize == 0 ? 0 : (int)((total + chunkSize - 1) / chunkSize);
                    m_Blocks[i] = new Block { ChunkSize = chunkSize, Chunks = Read<long>(blockOffsets[i] + 16, chunks) };
                }

                var entryCount = Math.Min((int)U32(directory + 16), (int)Chapter.Count);
                var entryOffsets = Read<long>(directory + 20, entryCount);
                Span<byte> header = stackalloc byte[18];
                for (var i = 0; i < entryCount; i++)
                {
                    if (entryOffsets[i] == 0)
                        continue;
                    RandomAccess.Read(m_File, header, entryOffsets[i]);
                    var e = new Entry
                    {
                        Format = (Format)BitConverter.ToUInt16(header),
                        Block = (int)BitConverter.ToUInt32(header.Slice(2)),
                        EntriesMeta = BitConverter.ToUInt32(header.Slice(6)),
                        HeaderMeta = BitConverter.ToUInt64(header.Slice(10)),
                    };
                    if (e.Format == Format.Dynamic && e.EntriesMeta > 0)
                    {
                        // On disk the header holds the first offset and the array holds the rest plus the end.
                        var rest = Read<long>(entryOffsets[i] + 18, (int)e.EntriesMeta);
                        e.Offsets = new long[e.EntriesMeta + 1];
                        e.Offsets[0] = (long)e.HeaderMeta;
                        System.Array.Copy(rest, 0, e.Offsets, 1, rest.Length);
                    }
                    m_Entries[i] = e;
                }
                Version = Has(Chapter.Version) ? Single<uint>(Chapter.Version) : 0;
            }
            catch
            {
                m_File.Dispose();
                throw;
            }
        }

        public void Dispose() => m_File.Dispose();

        public bool Has(Chapter c) => m_Entries[(int)c].Format != Format.Undefined && m_Entries[(int)c].Count > 0;

        public long Count(Chapter c) => m_Entries[(int)c].Count;

        /// <summary>Byte size of one element of a fixed-size array chapter.</summary>
        public int ElementSize(Chapter c) => m_Entries[(int)c].Format == Format.Fixed ? (int)m_Entries[(int)c].EntriesMeta : 0;

        public T Single<T>(Chapter c) where T : unmanaged
        {
            var bytes = SingleBytes(c);
            if (bytes.Length < Marshal.SizeOf<T>())
                System.Array.Resize(ref bytes, Marshal.SizeOf<T>());
            return MemoryMarshal.Read<T>(bytes);
        }

        public byte[] SingleBytes(Chapter c)
        {
            var e = m_Entries[(int)c];
            if (e.Format == Format.Undefined)
                return System.Array.Empty<byte>();
            if (e.Format != Format.Single)
                throw new WaitressException($"snapshot chapter {c} is not a single value");
            var bytes = new byte[e.EntriesMeta];
            ReadBlock(e.Block, (long)e.HeaderMeta, bytes);
            return bytes;
        }

        /// <summary>A fixed-size array chapter. Elements narrower than <typeparamref name="T"/> are widened (instance ids grew from 4 to 8 bytes).</summary>
        public T[] Array<T>(Chapter c) where T : unmanaged
        {
            var e = m_Entries[(int)c];
            if (e.Format == Format.Undefined || e.Count == 0)
                return System.Array.Empty<T>();
            if (e.Format != Format.Fixed)
                throw new WaitressException($"snapshot chapter {c} is not a fixed-size array");
            var size = (int)e.EntriesMeta;
            var count = (int)e.Count;
            var target = Marshal.SizeOf<T>();
            var bytes = new byte[(long)size * count];
            ReadBlock(e.Block, 0, bytes);
            if (size == target)
                return MemoryMarshal.Cast<byte, T>(bytes).ToArray();
            if (size > target)
                throw new WaitressException($"snapshot chapter {c} has {size}-byte elements, more than expected ({target})");
            var result = new T[count];
            var scratch = new byte[target];
            for (var i = 0; i < count; i++)
            {
                System.Array.Clear(scratch);
                System.Array.Copy(bytes, (long)i * size, scratch, 0, size);
                result[i] = MemoryMarshal.Read<T>(scratch);
            }
            return result;
        }

        public string[] Strings(Chapter c)
        {
            var e = m_Entries[(int)c];
            if (e.Format == Format.Undefined || e.Count == 0)
                return System.Array.Empty<string>();
            var all = DynamicBytes(e);
            var result = new string[e.Count];
            for (var i = 0; i < result.Length; i++)
                result[i] = System.Text.Encoding.UTF8.GetString(all, (int)(e.Offsets[i] - e.Offsets[0]), (int)(e.Offsets[i + 1] - e.Offsets[i]));
            return result;
        }

        /// <summary>A variable-size array chapter whose elements are arrays of <typeparamref name="T"/>.</summary>
        public T[][] Arrays<T>(Chapter c) where T : unmanaged
        {
            var e = m_Entries[(int)c];
            if (e.Format == Format.Undefined || e.Count == 0)
                return System.Array.Empty<T[]>();
            var all = DynamicBytes(e);
            var result = new T[e.Count][];
            for (var i = 0; i < result.Length; i++)
            {
                var start = (int)(e.Offsets[i] - e.Offsets[0]);
                var length = (int)(e.Offsets[i + 1] - e.Offsets[i]);
                result[i] = MemoryMarshal.Cast<byte, T>(all.AsSpan(start, length)).ToArray();
            }
            return result;
        }

        /// <summary>Each element of a variable-size byte chapter read separately, for chapters too big for one array.</summary>
        public byte[][] Blobs(Chapter c)
        {
            var e = m_Entries[(int)c];
            if (e.Format == Format.Undefined || e.Count == 0)
                return System.Array.Empty<byte[]>();
            var result = new byte[e.Count][];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new byte[e.Offsets[i + 1] - e.Offsets[i]];
                ReadBlock(e.Block, e.Offsets[i], result[i]);
            }
            return result;
        }

        byte[] DynamicBytes(in Entry e)
        {
            var total = e.Offsets[e.Count] - e.Offsets[0];
            if (total > int.MaxValue)
                throw new WaitressException("snapshot chapter too large to read at once");
            var bytes = new byte[total];
            ReadBlock(e.Block, e.Offsets[0], bytes);
            return bytes;
        }

        void ReadBlock(int blockIndex, long offset, Span<byte> destination)
        {
            var block = m_Blocks[blockIndex];
            while (destination.Length > 0)
            {
                var chunk = (int)(offset / block.ChunkSize);
                var within = offset % block.ChunkSize;
                var n = (int)Math.Min(destination.Length, block.ChunkSize - within);
                var read = 0;
                while (read < n)
                {
                    var got = RandomAccess.Read(m_File, destination.Slice(read, n - read), block.Chunks[chunk] + within + read);
                    if (got <= 0)
                        throw new WaitressException($"{Path} is truncated");
                    read += got;
                }
                destination = destination.Slice(n);
                offset += n;
            }
        }

        T[] Read<T>(long position, int count) where T : unmanaged
        {
            var bytes = new byte[count * Marshal.SizeOf<T>()];
            RandomAccess.Read(m_File, bytes, position);
            return MemoryMarshal.Cast<byte, T>(bytes).ToArray();
        }

        uint U32(long position)
        {
            Span<byte> b = stackalloc byte[4];
            RandomAccess.Read(m_File, b, position);
            return BitConverter.ToUInt32(b);
        }

        long I64(long position)
        {
            Span<byte> b = stackalloc byte[8];
            RandomAccess.Read(m_File, b, position);
            return BitConverter.ToInt64(b);
        }
    }
}
