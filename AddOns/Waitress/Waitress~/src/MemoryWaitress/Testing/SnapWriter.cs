using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MemoryWaitress.Snap;

namespace MemoryWaitress.Testing
{
    /// <summary>
    /// Writes synthetic .snap files in the Memory Profiler format. Every chapter gets its own block, chunks are small and
    /// interleaved across blocks, so reads that span chunks and scattered chunks are both exercised.
    /// </summary>
    public sealed class SnapWriter
    {
        const uint k_HeaderSignature = 0xAEABCDCD;
        const uint k_DirectorySignature = 0xCDCDAEAB;
        const uint k_FooterSignature = 0xABCDCDAE;
        const uint k_SectionVersion = 0x20170724;

        readonly Dictionary<Chapter, (ushort format, uint entriesMeta, ulong headerMeta, byte[] data, long[] offsets)> m_Chapters =
            new Dictionary<Chapter, (ushort, uint, ulong, byte[], long[])>();

        public int ChunkSize = 48;

        public void Single(Chapter c, byte[] value) => m_Chapters[c] = (1, (uint)value.Length, 0, value, null);

        public void Single<T>(Chapter c, T value) where T : unmanaged => Single(c, MemoryMarshal.AsBytes(new[] { value }.AsSpan()).ToArray());

        public void Fixed<T>(Chapter c, params T[] values) where T : unmanaged =>
            m_Chapters[c] = (2, (uint)Marshal.SizeOf<T>(), (ulong)values.Length, MemoryMarshal.AsBytes(values.AsSpan()).ToArray(), null);

        public void Dynamic(Chapter c, params byte[][] elements)
        {
            var offsets = new long[elements.Length + 1];
            for (var i = 0; i < elements.Length; i++)
                offsets[i + 1] = offsets[i] + elements[i].Length;
            m_Chapters[c] = (3, (uint)elements.Length, 0, elements.SelectMany(e => e).ToArray(), offsets);
        }

        public void Strings(Chapter c, params string[] values) => Dynamic(c, values.Select(Encoding.UTF8.GetBytes).ToArray());

        public void Arrays<T>(Chapter c, params T[][] values) where T : unmanaged =>
            Dynamic(c, values.Select(v => MemoryMarshal.AsBytes(v.AsSpan()).ToArray()).ToArray());

        public void Write(string path)
        {
            using var f = new BinaryWriter(File.Create(path));
            f.Write(k_HeaderSignature);
            var chapters = m_Chapters.OrderBy(kv => kv.Key).ToList();
            var chunkOffsets = chapters.Select(kv => new List<long>()).ToList();
            var chunkCounts = chapters.Select(kv => Math.Max(1, (kv.Value.data.Length + ChunkSize - 1) / ChunkSize)).ToList();
            // Round-robin over blocks so no block's chunks are contiguous.
            for (var round = 0; round < chunkCounts.Max(); round++)
            {
                for (var b = 0; b < chapters.Count; b++)
                {
                    if (round >= chunkCounts[b])
                        continue;
                    chunkOffsets[b].Add(f.BaseStream.Position);
                    var chunk = new byte[ChunkSize];
                    var data = chapters[b].Value.data;
                    var start = round * ChunkSize;
                    if (start < data.Length)
                        Array.Copy(data, start, chunk, 0, Math.Min(ChunkSize, data.Length - start));
                    f.Write(chunk);
                }
            }
            var blockAddresses = new long[chapters.Count];
            for (var b = 0; b < chapters.Count; b++)
            {
                blockAddresses[b] = f.BaseStream.Position;
                f.Write((ulong)ChunkSize);
                f.Write((ulong)chapters[b].Value.data.Length);
                var needed = (chapters[b].Value.data.Length + ChunkSize - 1) / ChunkSize;
                foreach (var o in chunkOffsets[b].Take(needed))
                    f.Write(o);
            }
            var blockSection = f.BaseStream.Position;
            f.Write(k_SectionVersion);
            f.Write((uint)chapters.Count);
            foreach (var a in blockAddresses)
                f.Write(a);
            var chapterAddresses = new long[(int)Chapter.Count];
            for (var b = 0; b < chapters.Count; b++)
            {
                var (format, entriesMeta, headerMeta, _, offsets) = chapters[b].Value;
                chapterAddresses[(int)chapters[b].Key] = f.BaseStream.Position;
                f.Write(format);
                f.Write((uint)b);
                f.Write(entriesMeta);
                if (format == 3)
                {
                    // The header holds the first offset; the array holds the rest and then the total.
                    f.Write((ulong)offsets[0]);
                    for (var i = 1; i < offsets.Length; i++)
                        f.Write(offsets[i]);
                }
                else
                    f.Write(headerMeta);
            }
            var directory = f.BaseStream.Position;
            f.Write(k_DirectorySignature);
            f.Write(k_SectionVersion);
            f.Write(blockSection);
            f.Write((uint)chapterAddresses.Length);
            foreach (var a in chapterAddresses)
                f.Write(a);
            f.Write(directory);
            f.Write(k_FooterSignature);
        }
    }
}
