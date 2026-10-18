using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BuildWaitress.Serialized;

namespace BuildWaitress.Model
{
    public sealed class SectionMeta
    {
        public int Index;
        public long FileSize;
        public long RawSize;
        public int ObjectRefs;
        public int Codec;
        public float[] Bounds;
    }

    /// <summary>A built subscene: its .entityheader and the .entities file of each section.</summary>
    public sealed class EntityScene
    {
        public string Guid;
        public string Name;
        public string HeaderFile;
        public readonly List<SectionMeta> Sections = new List<SectionMeta>();

        public static EntityScene ReadHeader(string path)
        {
            var b = File.ReadAllBytes(path);
            var version = BitConverter.ToInt32(b, 0);
            if (version != 3)
                throw new WaitressException($"{path}: entity scene header version {version} isn't supported (expected 3)");
            const int data = 4 + 32;
            var s = new EntityScene { HeaderFile = path, Guid = Path.GetFileNameWithoutExtension(path) };
            var (sectionsAt, sectionCount) = Blob.Array(b, data);
            for (var i = 0; i < sectionCount; i++)
            {
                var at = sectionsAt + i * 72;
                s.Sections.Add(new SectionMeta
                {
                    Index = BitConverter.ToInt32(b, at + 16),
                    FileSize = BitConverter.ToInt32(b, at + 20),
                    ObjectRefs = BitConverter.ToInt32(b, at + 24),
                    Bounds = Enumerable.Range(0, 6).Select(k => BitConverter.ToSingle(b, at + 28 + k * 4)).ToArray(),
                    Codec = BitConverter.ToInt32(b, at + 52),
                    RawSize = BitConverter.ToInt32(b, at + 56),
                });
            }
            s.Name = Blob.String(b, data + 8);
            return s;
        }

        public string SectionFile(int index) => Path.Combine(Path.GetDirectoryName(HeaderFile), $"{Guid}.{index}.entities");
    }

    public static class Blob
    {
        /// <summary>A BlobArray field: (absolute offset of element 0, length).</summary>
        public static (int at, int length) Array(byte[] b, int field)
        {
            var offset = BitConverter.ToInt32(b, field);
            var length = BitConverter.ToInt32(b, field + 4);
            return (field + offset, length);
        }

        public static string String(byte[] b, int field)
        {
            var (at, length) = Array(b, field);
            if (length <= 0)
                return "";
            return Encoding.UTF8.GetString(b, at, Math.Max(0, length - 1));
        }
    }

    public sealed class Archetype
    {
        public int Index;
        public int Entities;
        /// <summary>Indices into the section's type hashes; the chunk-component flag is split out.</summary>
        public int[] Types;
        public bool[] ChunkComponent;
        public int Chunks;
        public int Capacity = -1;
        public int[] Offsets;
        public int[] Sizes;
    }

    public sealed class BlobInfo
    {
        public int Offset;
        public int Length;
        public ulong Hash;
        public string Owner;
        public int References;
    }

    /// <summary>One section's .entities file (Entities' DOTSBIN container), read offline.</summary>
    public sealed class EntitySection
    {
        public const int ChunkSize = 16 * 1024;
        const int ChunkBuffer = ChunkSize - 64;

        public string File;
        public long Size;
        public int FileVersion;
        public bool Compressed;
        public readonly Dictionary<string, (long at, long size)> Nodes = new Dictionary<string, (long, long)>();
        public readonly List<ulong> TypeHashes = new List<ulong>();
        public readonly List<Archetype> Archetypes = new List<Archetype>();
        public readonly List<(int archetype, int count, long at)> Chunks = new List<(int, int, long)>();
        public readonly List<BlobInfo> Blobs = new List<BlobInfo>();
        public readonly List<(int chunk, int chunkOffset, int size, long at)> BufferRecords = new List<(int, int, int, long)>();
        public readonly Dictionary<ulong, string> DebugNames = new Dictionary<ulong, string>();
        public int SharedComponents;
        public long ManagedBytes;
        byte[] m_B;

        public static readonly Dictionary<string, string> NodeNames = new Dictionary<string, string>
        {
            ["e9fdfdeb4775443abb236d4858232894"] = "world",
            ["582bafe96ec44a72acd9ccbc850fdbc4"] = "debug",
            ["4847374145504072849aac7b1921e7fd"] = "type name strings",
            ["828510fad38f4ea78cd908d4d780388b"] = "type names",
            ["f5364e1ccb62466a9883f6f9554d4f0c"] = "archetypes",
            ["d565355c5cf34c0dbbd4a06adda948b1"] = "shared and managed components",
            ["5846e500ea614c1da94afb85afd8f4f4"] = "enabled bits",
            ["9a26954ff1ed4cc5a64e8aad4f64773a"] = "blob assets",
            ["2ea7ce3325f04d7a84ceab46790c628a"] = "chunks",
            ["e33124bfac2649d792de36e4611ddd70"] = "buffer data",
            ["2a84a183583a4fad8cb7105ae3c47598"] = "prefab",
        };

        public static EntitySection Read(string path, SectionMeta meta)
        {
            var raw = System.IO.File.ReadAllBytes(path);
            if (meta != null && raw.Length != meta.FileSize)
                throw new WaitressException($"the file is {raw.Length} bytes but its header expects {meta.FileSize}: the header and the section come from different builds, or the file is damaged");
            var s = new EntitySection { File = path, Size = raw.Length };
            if (meta != null && meta.Codec == 1)
            {
                raw = Lz4.Decode(raw, 0, raw.Length, (int)meta.RawSize);
                s.Compressed = true;
            }
            s.m_B = raw;
            s.Parse();
            return s;
        }

        int I32(long at) => BitConverter.ToInt32(m_B, (int)at);
        long I64(long at) => BitConverter.ToInt64(m_B, (int)at);

        void Parse()
        {
            if (m_B.Length < 152 || Encoding.ASCII.GetString(m_B, 0, 8) != "DOTSBIN!")
                throw new WaitressException($"{File} is not an Entities binary file");
            FileVersion = I32(8);
            var nodesAt = I64(0x68);
            var nodesSize = I32(0x70);
            for (var p = nodesAt; p < nodesAt + nodesSize;)
            {
                var id = new StringBuilder(32);
                for (var k = 0; k < 16; k++)
                {
                    var v = m_B[p + 8 + k];
                    id.Append("0123456789abcdef"[v & 15]).Append("0123456789abcdef"[v >> 4]);
                }
                var size = I32(p + 24);
                var name = NodeNames.TryGetValue(id.ToString(), out var n) ? n : id.ToString();
                Nodes[name] = (I64(p + 56), I64(p + 64));
                if (size <= 0)
                    break;
                p += size;
            }
            if (Nodes.TryGetValue("archetypes", out var arch))
                ReadArchetypes(arch.at);
            if (Nodes.TryGetValue("blob assets", out var blobs))
                ReadBlobs(blobs.at, blobs.size);
            if (Nodes.TryGetValue("chunks", out var chunks))
                for (var at = chunks.at; at + ChunkSize <= chunks.at + chunks.size; at += ChunkSize)
                {
                    var a = I32(at);
                    var count = I32(at + 4);
                    Chunks.Add((a, count, at));
                    if (a >= 0 && a < Archetypes.Count)
                        Archetypes[a].Chunks++;
                }
            if (Nodes.TryGetValue("buffer data", out var buffers))
                ReadBuffers(buffers.at);
            if (Nodes.TryGetValue("shared and managed components", out var shared))
            {
                SharedComponents = I32(shared.at);
                ManagedBytes = shared.size;
            }
            if (Nodes.TryGetValue("type names", out var names) && Nodes.TryGetValue("type name strings", out var strings))
                ReadDebugNames(names, strings);
        }

        void ReadArchetypes(long at)
        {
            var hashes = I32(at);
            at += 4;
            for (var i = 0; i < hashes; i++, at += 8)
                TypeHashes.Add((ulong)I64(at));
            var count = I32(at);
            at += 4;
            for (var i = 0; i < count; i++)
            {
                var a = new Archetype { Index = i, Entities = I32(at) };
                var types = I32(at + 4);
                at += 8;
                a.Types = new int[types];
                a.ChunkComponent = new bool[types];
                for (var t = 0; t < types; t++, at += 4)
                {
                    var v = I32(at);
                    a.ChunkComponent[t] = (v & (1 << 29)) != 0;
                    a.Types[t] = v & 0x00ffffff;
                }
                Archetypes.Add(a);
            }
        }

        void ReadBlobs(long at, long size)
        {
            var count = I32(at + 4);
            var p = at + 16;
            for (var i = 0; i < count && p + 32 <= at + size; i++)
            {
                var length = I32(p + 8);
                Blobs.Add(new BlobInfo { Offset = (int)(p + 32 - at), Length = length, Hash = (ulong)I64(p + 16) });
                p += 32 + length;
            }
        }

        void ReadBuffers(long at)
        {
            var chunks = I32(at);
            at += 4;
            for (var c = 0; c < chunks; c++)
            {
                var records = I32(at);
                at += 4;
                for (var r = 0; r < records; r++)
                {
                    var offset = I32(at);
                    var size = I32(at + 4);
                    BufferRecords.Add((c, offset, size, at + 8));
                    at += 8 + size;
                }
            }
        }

        void ReadDebugNames((long at, long size) names, (long at, long size) strings)
        {
            var count = (int)(names.size / 12);
            for (var i = 0; i < count; i++)
            {
                var hash = (ulong)I64(names.at + i * 12);
                var index = I32(names.at + i * 12 + 8);
                DebugNames[hash] = StringAt(strings, index);
            }
        }

        string StringAt((long at, long size) table, int index)
        {
            var p = table.at;
            var end = table.at + table.size;
            for (var i = 0; p < end; i++)
            {
                var length = I32(p);
                if (i == index)
                    return Encoding.UTF8.GetString(m_B, (int)p + 4, length);
                p += 4 + length;
            }
            return null;
        }

        public long NodeSize(string name) => Nodes.TryGetValue(name, out var n) ? n.size : 0;

        /// <summary>Unity's chunk layout for each archetype (capacity, per-type offsets), when every type's size is known.</summary>
        public void Layout(TypeTable types, EcsType entity)
        {
            foreach (var a in Archetypes)
            {
                var all = new List<(EcsType t, bool chunk)> { (entity, false) };
                all.AddRange(a.Types.Select((t, i) => (types.Get(TypeHashes[t]), a.ChunkComponent[i])));
                if (all.Any(x => !x.t.Known))
                    continue;
                var sizes = all.Select(x => x.chunk || (x.t.Flags & (1 << 30)) != 0 ? 0 : x.t.SizeInChunk).ToArray();
                var max = all.Min(x => x.t.MaxChunkCapacity);
                var total = sizes.Sum();
                var capacity = total == 0 ? max : ChunkBuffer / total;
                while (capacity > 0 && sizes.Sum(sz => Align64(sz * capacity)) > ChunkBuffer)
                    capacity--;
                capacity = Math.Min(max, capacity);
                var order = Enumerable.Range(0, all.Count).ToList();
                var flags = all.Select((x, i) => x.t.Flags | (x.chunk ? 1 << 29 : 0)).ToArray();
                bool Less(int l, int r) => flags[l] == flags[r] ? all[l].t.MemoryOrdering < all[r].t.MemoryOrdering : flags[l] < flags[r];
                var memory = new int[all.Count];
                for (var i = 0; i < all.Count; i++)
                {
                    var index = i;
                    while (index > 1 && Less(i, memory[index - 1]))
                    {
                        memory[index] = memory[index - 1];
                        index--;
                    }
                    memory[index] = i;
                }
                var offsets = new int[all.Count];
                var used = 0;
                foreach (var i in memory)
                {
                    offsets[i] = used;
                    used += Align64(sizes[i] * capacity);
                }
                a.Capacity = capacity;
                a.Offsets = offsets;
                a.Sizes = sizes;
            }
        }

        static int Align64(int x) => (x + 63) & ~63;

        /// <summary>Attributes each blob to the first component type that references it, through chunk and buffer data.</summary>
        public void AttributeBlobs(TypeTable types)
        {
            if (Blobs.Count == 0)
                return;
            var byOffset = Blobs.ToDictionary(b => b.Offset);
            void Hit(long value, string owner)
            {
                if (value < 0 || !byOffset.TryGetValue((int)value, out var blob))
                    return;
                blob.References++;
                blob.Owner ??= owner;
            }
            var bufferAt = BufferRecords.GroupBy(r => r.chunk).ToDictionary(g => g.Key, g => g.ToDictionary(r => r.chunkOffset, r => r.at));
            for (var c = 0; c < Chunks.Count; c++)
            {
                var (ai, count, at) = Chunks[c];
                if (ai < 0 || ai >= Archetypes.Count)
                    continue;
                var a = Archetypes[ai];
                if (a.Offsets == null)
                    continue;
                for (var ti = 0; ti < a.Types.Length; ti++)
                {
                    var t = types.Get(TypeHashes[a.Types[ti]]);
                    if (t.BlobOffsets.Length == 0 || a.Sizes[ti + 1] == 0)
                        continue;
                    var start = at + 64 + a.Offsets[ti + 1];
                    for (var e = 0; e < count; e++)
                    {
                        if (t.IsBuffer)
                        {
                            var header = start + (long)e * a.Sizes[ti + 1];
                            var length = I32(header + 8);
                            long elements = header + 16;
                            if (bufferAt.TryGetValue(c, out var recs) && recs.TryGetValue((int)(header - at - 64), out var heap))
                                elements = heap;
                            for (var k = 0; k < length; k++)
                                foreach (var o in t.BlobOffsets)
                                    Hit(I64(elements + (long)k * t.ElementSize + o), t.Short);
                        }
                        else
                            foreach (var o in t.BlobOffsets)
                                Hit(I64(start + (long)e * a.Sizes[ti + 1] + o), t.Short);
                    }
                }
            }
        }
    }
}
