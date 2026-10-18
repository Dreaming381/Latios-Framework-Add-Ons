using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BuildWaitress.Serialized
{
    /// <summary>A UnityFS archive (asset bundles, Entities content archives): its files, decompressed on demand.</summary>
    public sealed class UnityArchive
    {
        public sealed class Node
        {
            public string Path;
            public long Offset;
            public long Size;
            public uint Flags;
            public bool IsSerializedFile => (Flags & 4) != 0;
        }

        public string FilePath;
        public string UnityVersion;
        public string Compression;
        public readonly List<Node> Nodes = new List<Node>();
        readonly List<(uint raw, uint packed, ushort flags)> m_Blocks = new List<(uint, uint, ushort)>();
        long m_DataStart;
        byte[] m_File;
        byte[] m_Data;

        public static bool Looks(byte[] head) => head.Length >= 8 && Encoding.ASCII.GetString(head, 0, 7) == "UnityFS" && head[7] == 0;

        public static UnityArchive Read(string path)
        {
            var a = new UnityArchive { FilePath = path, m_File = File.ReadAllBytes(path) };
            a.Parse();
            return a;
        }

        void Parse()
        {
            var b = m_File;
            var at = 8L;
            var version = BE32(b, at);
            at += 4;
            CString(b, ref at);
            UnityVersion = CString(b, ref at);
            var size = BE64(b, at);
            var infoPacked = BE32(b, at + 8);
            var infoRaw = BE32(b, at + 12);
            var flags = BE32(b, at + 16);
            at += 20;
            if (version < 6)
                throw new WaitressException($"{FilePath}: UnityFS version {version} isn't supported");
            if (version >= 7)
                at = (at + 15) & ~15L;
            long infoAt;
            if ((flags & 0x80) != 0)
                infoAt = size - infoPacked;
            else
            {
                infoAt = at;
                at += infoPacked;
            }
            var info = Unpack(b, infoAt, (int)infoPacked, (int)infoRaw, (int)(flags & 0x3f));
            if ((flags & 0x200) != 0)
                at = (at + 15) & ~15L;
            m_DataStart = at;
            var p = 16L;
            var blockCount = (int)BE32(info, p);
            p += 4;
            for (var i = 0; i < blockCount; i++)
            {
                m_Blocks.Add((BE32(info, p), BE32(info, p + 4), (ushort)(info[p + 8] << 8 | info[p + 9])));
                p += 10;
            }
            var nodeCount = (int)BE32(info, p);
            p += 4;
            for (var i = 0; i < nodeCount; i++)
            {
                var n = new Node { Offset = BE64(info, p), Size = BE64(info, p + 8), Flags = BE32(info, p + 16) };
                p += 20;
                n.Path = CString(info, ref p);
                Nodes.Add(n);
            }
            var kinds = new HashSet<string>();
            foreach (var blk in m_Blocks)
                kinds.Add(CodecName(blk.flags & 0x3f));
            Compression = string.Join("+", kinds);
        }

        static string CodecName(int c) => c switch { 0 => "none", 1 => "LZMA", 2 => "LZ4", 3 => "LZ4HC", _ => "codec " + c };

        static byte[] Unpack(byte[] src, long at, int packed, int raw, int codec)
        {
            switch (codec)
            {
                case 0:
                    var copy = new byte[packed];
                    Array.Copy(src, at, copy, 0, packed);
                    return copy;
                case 2:
                case 3:
                    return Lz4.Decode(src, (int)at, packed, raw);
                default:
                    throw new WaitressException($"{CodecName(codec)} compressed archives aren't supported (rebuild with LZ4 or no compression)");
            }
        }

        /// <summary>The archive's decompressed data stream; nodes index into it.</summary>
        public byte[] Data
        {
            get
            {
                if (m_Data != null)
                    return m_Data;
                long total = 0;
                foreach (var blk in m_Blocks)
                    total += blk.raw;
                var data = new byte[total];
                long src = m_DataStart, dst = 0;
                foreach (var blk in m_Blocks)
                {
                    var part = Unpack(m_File, src, (int)blk.packed, (int)blk.raw, blk.flags & 0x3f);
                    Array.Copy(part, 0, data, dst, part.Length);
                    src += blk.packed;
                    dst += blk.raw;
                }
                return m_Data = data;
            }
        }

        public SerializedFile Open(Node n) => SerializedFile.Read(Slice(n), FilePath + "/" + n.Path, 0);

        public byte[] Slice(Node n)
        {
            var bytes = new byte[n.Size];
            Array.Copy(Data, n.Offset, bytes, 0, n.Size);
            return bytes;
        }

        static uint BE32(byte[] b, long o) => (uint)(b[o] << 24 | b[o + 1] << 16 | b[o + 2] << 8 | b[o + 3]);
        static long BE64(byte[] b, long o) => (long)((ulong)BE32(b, o) << 32 | BE32(b, o + 4));

        static string CString(byte[] b, ref long at)
        {
            var end = at;
            while (end < b.Length && b[end] != 0)
                end++;
            var s = Encoding.UTF8.GetString(b, (int)at, (int)(end - at));
            at = end + 1;
            return s;
        }
    }

    public static class Lz4
    {
        /// <summary>Decodes one LZ4 block.</summary>
        public static byte[] Decode(byte[] src, int at, int length, int rawLength)
        {
            var dst = new byte[rawLength];
            int s = at, end = at + length, d = 0;
            while (s < end)
            {
                var token = src[s++];
                var literals = token >> 4;
                if (literals == 15)
                {
                    byte more;
                    do
                    {
                        more = src[s++];
                        literals += more;
                    } while (more == 255);
                }
                Array.Copy(src, s, dst, d, literals);
                s += literals;
                d += literals;
                if (s >= end)
                    break;
                var offset = src[s] | src[s + 1] << 8;
                s += 2;
                var match = token & 15;
                if (match == 15)
                {
                    byte more;
                    do
                    {
                        more = src[s++];
                        match += more;
                    } while (more == 255);
                }
                match += 4;
                if (offset == 0 || offset > d)
                    throw new WaitressException("corrupt LZ4 data");
                for (var i = 0; i < match; i++, d++)
                    dst[d] = dst[d - offset];
            }
            if (d != rawLength)
                throw new WaitressException($"LZ4 data decoded to {d} bytes, expected {rawLength}");
            return dst;
        }
    }
}
