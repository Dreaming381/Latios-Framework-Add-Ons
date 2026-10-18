using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildWaitress.Serialized
{
    public sealed class TypeNode
    {
        public string Type;
        public string Name;
        public int ByteSize;
        public int Level;
        public byte Flags;
        public int MetaFlags;
        public readonly List<TypeNode> Children = new List<TypeNode>();

        public bool AlignAfter => (MetaFlags & 0x4000) != 0;
        public bool IsArray => (Flags & 1) != 0 || Type == "Array";
    }

    public sealed class SerializedType
    {
        public int ClassId;
        public bool Stripped;
        public short ScriptTypeIndex;
        public TypeNode Tree;
    }

    public sealed class ObjectInfo
    {
        public long PathId;
        public long Offset;
        public uint Size;
        public int TypeIndex;
        public int ClassId;
    }

    public sealed class ExternalFile
    {
        public string Guid;
        public int Type;
        public string Path;
    }

    /// <summary>
    /// A Unity serialized file (scenes, assets, build reports, player data). Objects decode generically through the
    /// file's type trees; files without type trees (player builds) still give the object table.
    /// </summary>
    public sealed class SerializedFile
    {
        public string Path;
        public int Version;
        public string UnityVersion;
        public int Platform;
        public bool HasTypeTrees;
        public bool BigEndian;
        public long DataOffset;
        public long FileSize;
        public readonly List<SerializedType> Types = new List<SerializedType>();
        public readonly List<ObjectInfo> Objects = new List<ObjectInfo>();
        public readonly List<ExternalFile> Externals = new List<ExternalFile>();
        public readonly List<(int file, long pathId)> ScriptTypes = new List<(int, long)>();

        byte[] m_Bytes;
        long m_Base;

        /// <summary>Whether a file starts like a serialized file, without throwing on other formats.</summary>
        public static bool Looks(byte[] head, long length)
        {
            if (head.Length < 20)
                return false;
            var version = (int)BE32(head, 8);
            if (version < 9 || version > 40)
                return false;
            if (version >= 22)
                return head.Length >= 48 && BE64(head, 24) == length;
            return BE32(head, 4) == length;
        }

        public static SerializedFile Read(string path) => Read(File.ReadAllBytes(path), path, 0);

        public static SerializedFile Read(byte[] bytes, string path, long start)
        {
            var head = new byte[Math.Min(64, bytes.Length - start)];
            Array.Copy(bytes, start, head, 0, head.Length);
            if (!Looks(head, bytes.Length - start))
            {
                var v = head.Length >= 12 ? (int)BE32(head, 8) : 0;
                if (v >= 9 && v <= 40)
                    throw new WaitressException($"{path} is truncated or damaged: its header says {(v >= 22 && head.Length >= 32 ? BE64(head, 24) : BE32(head, 4))} bytes, the file has {bytes.Length - start}");
                throw new WaitressException($"{path} is not a Unity serialized file");
            }
            var version = (int)BE32(head, 8);
            if (version > 22)
                throw new WaitressException($"{path} uses serialized file version {version} (Unity 6000.6 or later), which isn't supported yet; versions 9 to 22 (Unity 5 to 6000.5) are");
            var f = new SerializedFile { Path = path, m_Bytes = bytes, m_Base = start };
            f.Parse();
            return f;
        }

        static uint BE32(byte[] b, long o) => (uint)(b[o] << 24 | b[o + 1] << 16 | b[o + 2] << 8 | b[o + 3]);
        static long BE64(byte[] b, long o) => (long)((ulong)BE32(b, o) << 32 | BE32(b, o + 4));

        void Parse()
        {
            var r = new Reader(m_Bytes, m_Base, true);
            var metadataSize = r.U32();
            FileSize = r.U32();
            Version = (int)r.U32();
            DataOffset = r.U32();
            if (Version < 9)
                throw new WaitressException($"{Path}: serialized file version {Version} is too old");
            BigEndian = r.U8() != 0;
            r.Skip(3);
            if (Version >= 22)
            {
                metadataSize = r.U32();
                FileSize = r.I64();
                DataOffset = r.I64();
                r.I64();
            }
            r.BigEndian = BigEndian;
            UnityVersion = r.CString();
            Platform = r.I32();
            HasTypeTrees = Version < 13 || r.U8() != 0;
            var typeCount = r.I32();
            for (var i = 0; i < typeCount; i++)
                Types.Add(ReadType(r, false));
            var objectCount = r.I32();
            for (var i = 0; i < objectCount; i++)
            {
                if (Version >= 14)
                    r.Align4();
                var o = new ObjectInfo { PathId = Version >= 14 ? r.I64() : r.I32() };
                o.Offset = DataOffset + (Version >= 22 ? r.I64() : r.U32());
                o.Size = r.U32();
                o.TypeIndex = r.I32();
                if (Version < 16)
                {
                    o.ClassId = r.U16();
                    o.TypeIndex = Types.FindIndex(t => t.ClassId == o.TypeIndex);
                }
                else
                    o.ClassId = o.TypeIndex >= 0 && o.TypeIndex < Types.Count ? Types[o.TypeIndex].ClassId : -1;
                if (Version < 11)
                    r.U16();
                if (Version >= 11 && Version < 17)
                    r.I16();
                if (Version == 15 || Version == 16)
                    r.U8();
                Objects.Add(o);
            }
            if (Version >= 11)
            {
                var scripts = r.I32();
                for (var i = 0; i < scripts; i++)
                {
                    var file = r.I32();
                    if (Version >= 14)
                        r.Align4();
                    ScriptTypes.Add((file, Version >= 14 ? r.I64() : r.I32()));
                }
            }
            var externals = r.I32();
            for (var i = 0; i < externals; i++)
            {
                r.CString();
                var guid = r.Bytes(16);
                var type = r.I32();
                Externals.Add(new ExternalFile { Guid = GuidText(guid), Type = type, Path = r.CString() });
            }
        }

        /// <summary>Unity's GUID text: each byte's nibbles swapped.</summary>
        static string GuidText(byte[] g)
        {
            var sb = new StringBuilder(32);
            foreach (var b in g)
                sb.Append("0123456789abcdef"[b & 15]).Append("0123456789abcdef"[b >> 4]);
            return sb.ToString();
        }

        SerializedType ReadType(Reader r, bool refType)
        {
            var t = new SerializedType { ClassId = r.I32() };
            if (Version >= 16)
                t.Stripped = r.U8() != 0;
            if (Version >= 17)
                t.ScriptTypeIndex = r.I16();
            if (Version >= 13)
            {
                if ((Version < 16 && t.ClassId < 0) || (Version >= 16 && t.ClassId == 114) || refType)
                    r.Skip(16);
                r.Skip(16);
            }
            if (!HasTypeTrees)
                return t;
            if (Version < 12 && Version != 10)
                throw new WaitressException($"{Path}: type trees of serialized file version {Version} aren't supported");
            var nodeCount = r.I32();
            var stringSize = r.I32();
            var raw = new (ushort version, byte level, byte flags, uint type, uint name, int size, int index, int meta)[nodeCount];
            for (var i = 0; i < nodeCount; i++)
            {
                raw[i] = (r.U16(), r.U8(), r.U8(), r.U32(), r.U32(), r.I32(), r.I32(), r.I32());
                if (Version >= 19)
                    r.Skip(8);
            }
            var strings = r.Bytes(stringSize);
            string Str(uint offset)
            {
                if ((offset & 0x80000000) != 0)
                    return CommonStrings.At(offset & 0x7fffffff);
                var end = (int)offset;
                while (end < strings.Length && strings[end] != 0)
                    end++;
                return Encoding.UTF8.GetString(strings, (int)offset, end - (int)offset);
            }
            var stack = new List<TypeNode>();
            foreach (var n in raw)
            {
                var node = new TypeNode { Type = Str(n.type), Name = Str(n.name), ByteSize = n.size, Level = n.level, Flags = n.flags, MetaFlags = n.meta };
                while (stack.Count > n.level)
                    stack.RemoveAt(stack.Count - 1);
                if (stack.Count > 0)
                    stack[^1].Children.Add(node);
                else
                    t.Tree = node;
                stack.Add(node);
            }
            if (Version >= 21)
            {
                if (refType)
                {
                    r.CString();
                    r.CString();
                    r.CString();
                }
                else
                {
                    var deps = r.I32();
                    r.Skip(4L * deps);
                }
            }
            return t;
        }

        /// <summary>The string an object starts with (m_Name for most asset classes), without a type tree; null when implausible.</summary>
        public string LeadingString(ObjectInfo o)
        {
            var at = m_Base + o.Offset;
            if (at + 4 > m_Bytes.Length || o.Size < 4)
                return null;
            var r = new Reader(m_Bytes, at, BigEndian);
            var length = r.I32();
            if (length < 0 || length > 1024 || length + 4 > o.Size)
                return null;
            var bytes = r.Bytes(length);
            if (bytes.Any(b => b < 32 && b != 9))
                return null;
            return Encoding.UTF8.GetString(bytes);
        }

        public SerializedType TypeOf(ObjectInfo o) => o.TypeIndex >= 0 && o.TypeIndex < Types.Count ? Types[o.TypeIndex] : null;

        /// <summary>The object decoded through its type tree: a Dictionary for structs, a List or byte[] for arrays.</summary>
        public object Decode(ObjectInfo o)
        {
            var type = TypeOf(o);
            if (type?.Tree == null)
                throw new WaitressException($"{Path}: object {o.PathId} has no type tree");
            var r = new Reader(m_Bytes, m_Base + o.Offset, BigEndian);
            return ReadValue(r, type.Tree);
        }

        static object ReadValue(Reader r, TypeNode node)
        {
            object value;
            if (node.Children.Count == 0)
                value = ReadPrimitive(r, node);
            else if (node.Type == "string")
            {
                var length = r.I32();
                value = Encoding.UTF8.GetString(r.Bytes(length));
                r.Align4();
                return value;
            }
            else if (node.Children.Count == 1 && node.Children[0].IsArray)
            {
                value = ReadArray(r, node.Children[0]);
                if (node.Children[0].AlignAfter)
                    r.Align4();
            }
            else if (node.IsArray)
                value = ReadArray(r, node);
            else
            {
                var fields = new Dictionary<string, object>(node.Children.Count);
                foreach (var c in node.Children)
                    fields[c.Name] = ReadValue(r, c);
                value = fields;
            }
            if (node.AlignAfter)
                r.Align4();
            return value;
        }

        static object ReadArray(Reader r, TypeNode array)
        {
            var count = r.I32();
            if (count < 0 || count > 100_000_000)
                throw new WaitressException($"array '{array.Name}' has an invalid length {count}");
            var element = array.Children[1];
            if (element.Children.Count == 0 && element.ByteSize == 1)
                return r.Bytes(count);
            var list = new List<object>(count);
            for (var i = 0; i < count; i++)
                list.Add(ReadValue(r, element));
            return list;
        }

        static object ReadPrimitive(Reader r, TypeNode node)
        {
            switch (node.Type)
            {
                case "bool": return r.U8() != 0;
                case "SInt8": return (sbyte)r.U8();
                case "UInt8":
                case "char": return r.U8();
                case "SInt16":
                case "short": return r.I16();
                case "UInt16":
                case "unsigned short": return r.U16();
                case "SInt32":
                case "int": return r.I32();
                case "UInt32":
                case "unsigned int":
                case "Type*": return r.U32();
                case "SInt64":
                case "long long": return r.I64();
                case "UInt64":
                case "unsigned long long":
                case "FileSize": return (ulong)r.I64();
                case "float": return r.F32();
                case "double": return r.F64();
                case "TypelessData":
                    return r.Bytes(r.I32());
            }
            switch (node.ByteSize)
            {
                case 1: return r.U8();
                case 2: return r.U16();
                case 4: return r.U32();
                case 8: return (ulong)r.I64();
            }
            throw new WaitressException($"unknown primitive type '{node.Type}' ({node.ByteSize} bytes) in field '{node.Name}'");
        }

        sealed class Reader
        {
            readonly byte[] m_B;
            readonly long m_Start;
            long m_At;
            public bool BigEndian;

            public Reader(byte[] b, long at, bool bigEndian)
            {
                m_B = b;
                m_Start = at;
                m_At = at;
                BigEndian = bigEndian;
            }

            void Need(long n)
            {
                if (m_At + n > m_B.Length || n < 0)
                    throw new WaitressException($"unexpected end of data at offset {m_At} (wanted {n} bytes)");
            }

            public void Skip(long n)
            {
                Need(n);
                m_At += n;
            }

            public void Align4() => m_At = m_Start + ((m_At - m_Start + 3) & ~3L);
            public byte U8()
            {
                Need(1);
                return m_B[m_At++];
            }

            public byte[] Bytes(int n)
            {
                Need(n);
                var a = new byte[n];
                Array.Copy(m_B, m_At, a, 0, n);
                m_At += n;
                return a;
            }

            ulong Int(int size)
            {
                Need(size);
                ulong v = 0;
                if (BigEndian)
                    for (var i = 0; i < size; i++)
                        v = v << 8 | m_B[m_At + i];
                else
                    for (var i = size - 1; i >= 0; i--)
                        v = v << 8 | m_B[m_At + i];
                m_At += size;
                return v;
            }

            public ushort U16() => (ushort)Int(2);
            public short I16() => (short)Int(2);
            public uint U32() => (uint)Int(4);
            public int I32() => (int)Int(4);
            public long I64() => (long)Int(8);
            public float F32() => BitConverter.Int32BitsToSingle(I32());
            public double F64() => BitConverter.Int64BitsToDouble(I64());

            public string CString()
            {
                var end = m_At;
                while (end < m_B.Length && m_B[end] != 0)
                    end++;
                var s = Encoding.UTF8.GetString(m_B, (int)m_At, (int)(end - m_At));
                m_At = end + 1;
                return s;
            }
        }
    }
}
