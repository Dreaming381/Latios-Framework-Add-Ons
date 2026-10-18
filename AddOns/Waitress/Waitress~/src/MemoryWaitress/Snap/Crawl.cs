using System;
using System.Collections.Generic;

namespace MemoryWaitress.Snap
{
    public enum RootKind : byte
    {
        Static,
        GcHandle,
    }

    public struct Root
    {
        public RootKind Kind;
        /// <summary>The type owning the static field, or the GC handle index.</summary>
        public int Index;
        /// <summary>The static field, or -1.</summary>
        public int Field;
        /// <summary>The native object holding the GC handle, or -1.</summary>
        public int NativeObject;
    }

    /// <summary>
    /// Managed objects found by following references from GC handles and static fields, the same roots the
    /// Memory Profiler uses. The walk is breadth-first, so each object's parent is on a shortest path to a root.
    /// </summary>
    public sealed class Crawl
    {
        /// <summary>Edge sources at or above zero are objects; below zero they are roots, encoded as -(root + 1).</summary>
        public const int NoParent = int.MinValue;

        public int Count;
        public ulong[] Address = new ulong[1024];
        public int[] Type = new int[1024];
        public long[] Size = new long[1024];
        public int[] Parent = new int[1024];
        /// <summary>The field the parent reached this object through; an array element is -(index + 2); -1 is unknown.</summary>
        public int[] ParentVia = new int[1024];
        public int[] NativeObject;

        public readonly List<Root> Roots = new List<Root>();
        public readonly Dictionary<ulong, int> ByAddress = new Dictionary<ulong, int>();

        public int EdgeCount;
        public int[] EdgeFrom = new int[4096];
        public int[] EdgeTo = new int[4096];
        public int[] EdgeVia = new int[4096];

        public int Unresolved;
        public long TotalSize;

        readonly Snapshot m_S;
        readonly Dictionary<int, (int offset, int field)[]> m_Layouts = new Dictionary<int, (int, int)[]>();
        readonly Dictionary<ulong, int> m_TypeByInfo = new Dictionary<ulong, int>();
        readonly ulong[] m_HeapStarts;
        readonly int m_StringType;
        readonly uint m_Ptr;
        readonly uint m_Header;
        int m_Head;

        Crawl(Snapshot s)
        {
            m_S = s;
            m_Ptr = s.Vm.PointerSize;
            m_Header = s.Vm.ObjectHeaderSize;
            for (var t = 0; t < s.TypeInfoAddress.Length; t++)
                m_TypeByInfo.TryAdd(s.TypeInfoAddress[t], t);
            m_HeapStarts = Array.ConvertAll(s.Heap, h => h.Start);
            m_StringType = Array.IndexOf(s.TypeName, "System.String");
        }

        public static Crawl Run(Snapshot s)
        {
            var c = new Crawl(s);
            c.Walk();
            return c;
        }

        void Walk()
        {
            var s = m_S;
            var nativeByHandle = new Dictionary<int, int>();
            for (var i = 0; i < s.ObjGcHandle.Length; i++)
                if (s.ObjGcHandle[i] >= 0)
                    nativeByHandle.TryAdd(s.ObjGcHandle[i], i);

            // Statics first: when a static and a GC handle reach an object at the same depth, the static explains more.
            for (var t = 0; t < s.TypeFields.Length; t++)
            {
                var statics = t < s.TypeStatics.Length ? s.TypeStatics[t] : null;
                if (statics == null || statics.Length == 0)
                    continue;
                foreach (var f in s.TypeFields[t])
                {
                    if (!s.FieldStatic[f])
                        continue;
                    var ft = s.FieldType[f];
                    if (ft < 0)
                        continue;
                    var offset = s.FieldOffset[f];
                    if (s.IsValueType(ft))
                    {
                        foreach (var (o, _) in Layout(ft))
                            StaticRef(statics, offset + o, t, f);
                    }
                    else
                        StaticRef(statics, offset, t, f);
                }
            }
            for (var h = 0; h < s.GcHandleTarget.Length; h++)
            {
                if (s.GcHandleTarget[h] == 0)
                    continue;
                Roots.Add(new Root { Kind = RootKind.GcHandle, Index = h, Field = -1, NativeObject = nativeByHandle.TryGetValue(h, out var n) ? n : -1 });
                Visit(s.GcHandleTarget[h], -Roots.Count, -1);
            }

            while (m_Head < Count)
                Scan(m_Head++);

            NativeObject = new int[Count];
            Array.Fill(NativeObject, -1);
            for (var i = 0; i < s.ObjGcHandle.Length; i++)
            {
                var h = s.ObjGcHandle[i];
                if (h >= 0 && h < s.GcHandleTarget.Length && ByAddress.TryGetValue(s.GcHandleTarget[h], out var o))
                    NativeObject[o] = i;
            }
        }

        void StaticRef(byte[] statics, int offset, int type, int field)
        {
            if (offset < 0 || offset + m_Ptr > statics.Length)
                return;
            var p = m_Ptr == 8 ? BitConverter.ToUInt64(statics, offset) : BitConverter.ToUInt32(statics, offset);
            if (p == 0)
                return;
            if (!ByAddress.ContainsKey(p))
            {
                Roots.Add(new Root { Kind = RootKind.Static, Index = type, Field = field, NativeObject = -1 });
                Visit(p, -Roots.Count, field);
                return;
            }
            Roots.Add(new Root { Kind = RootKind.Static, Index = type, Field = field, NativeObject = -1 });
            AddEdge(-Roots.Count, ByAddress[p], field);
        }

        void Scan(int o)
        {
            var s = m_S;
            var t = Type[o];
            var a = Address[o];
            if (s.IsArray(t))
            {
                var e = s.TypeBaseOrElement[t];
                if (e < 0)
                    return;
                var length = ArrayLength(a, t);
                var data = a + s.Vm.ArrayHeaderSize;
                if (!s.IsValueType(e))
                {
                    for (long i = 0; i < length; i++)
                        if (TryPtr(data + (ulong)i * m_Ptr, out var p))
                            Visit(p, o, Element(i));
                    return;
                }
                var layout = Layout(e);
                if (layout.Length == 0)
                    return;
                var stride = (ulong)Math.Max(1, s.TypeSize[e]);
                for (long i = 0; i < length; i++)
                    foreach (var (off, _) in layout)
                        if (TryPtr(data + (ulong)i * stride + (ulong)off, out var p))
                            Visit(p, o, Element(i));
                return;
            }
            if (t == m_StringType)
                return;
            // Instance field offsets include the object header; a boxed value type starts its fields after one.
            var start = a + m_Header;
            foreach (var (off, field) in Layout(t))
                if (TryPtr(start + (ulong)off, out var p))
                    Visit(p, o, field);
        }

        static int Element(long i) => i > int.MaxValue - 2 ? -1 : -(int)(i + 2);

        void Visit(ulong address, int from, int via)
        {
            if (address == 0)
                return;
            if (ByAddress.TryGetValue(address, out var existing))
            {
                AddEdge(from, existing, via);
                return;
            }
            if (!TryParse(address, out var type, out var size))
            {
                Unresolved++;
                return;
            }
            var o = Count++;
            if (o == Address.Length)
            {
                var n = o * 2;
                Array.Resize(ref Address, n);
                Array.Resize(ref Type, n);
                Array.Resize(ref Size, n);
                Array.Resize(ref Parent, n);
                Array.Resize(ref ParentVia, n);
            }
            Address[o] = address;
            Type[o] = type;
            Size[o] = size;
            Parent[o] = from;
            ParentVia[o] = via;
            TotalSize += size;
            ByAddress.Add(address, o);
            AddEdge(from, o, via);
        }

        void AddEdge(int from, int to, int via)
        {
            if (EdgeCount == EdgeFrom.Length)
            {
                var n = EdgeCount * 2;
                Array.Resize(ref EdgeFrom, n);
                Array.Resize(ref EdgeTo, n);
                Array.Resize(ref EdgeVia, n);
            }
            EdgeFrom[EdgeCount] = from;
            EdgeTo[EdgeCount] = to;
            EdgeVia[EdgeCount] = via;
            EdgeCount++;
        }

        bool TryParse(ulong address, out int type, out long size)
        {
            type = -1;
            size = 0;
            var section = Section(address);
            if (section == null || !TryPtr(address, out var p))
                return false;
            if (!m_TypeByInfo.TryGetValue(p, out type))
            {
                // Mono objects point at a vtable, whose first word is the class.
                if (!TryPtr(p, out var klass) || !m_TypeByInfo.TryGetValue(klass, out type))
                    return false;
            }
            size = ObjectSize(address, type);
            return size > 0 && address + (ulong)size <= section.End;
        }

        long ObjectSize(ulong address, int t)
        {
            var s = m_S;
            if (s.IsArray(t))
            {
                var e = s.TypeBaseOrElement[t];
                var elementSize = e >= 0 && s.IsValueType(e) ? s.TypeSize[e] : (int)m_Ptr;
                return s.Vm.ArrayHeaderSize + Math.Max(0, elementSize) * ArrayLength(address, t);
            }
            if (t == m_StringType)
            {
                var length = ReadI32(address + m_Header);
                if (length < 0)
                    return 0;
                return m_Header + 4 + 2L * length + 2;
            }
            return s.TypeSize[t] + (s.IsValueType(t) ? m_Header : 0);
        }

        public long ArrayLength(ulong address, int t)
        {
            if (!TryPtr(address + m_S.Vm.ArrayBoundsOffset, out var bounds) || bounds == 0)
                return Math.Max(0, ReadI32(address + m_S.Vm.ArraySizeOffset));
            long length = 1;
            var rank = Math.Max(1, m_S.ArrayRank(t));
            for (var i = 0; i < rank; i++)
                length *= Math.Max(0, ReadI32(bounds + (ulong)i * 8));
            return length;
        }

        /// <summary>Reference fields of a type, flattened through embedded structs, as offsets from where its fields start.</summary>
        public (int offset, int field)[] Layout(int t)
        {
            if (m_Layouts.TryGetValue(t, out var cached))
                return cached;
            m_Layouts[t] = Array.Empty<(int, int)>();
            var s = m_S;
            var result = new List<(int, int)>();
            for (var x = t; x >= 0 && !s.IsArray(x); x = s.TypeBaseOrElement[x])
            {
                foreach (var f in s.TypeFields[x])
                {
                    if (s.FieldStatic[f])
                        continue;
                    var ft = s.FieldType[f];
                    if (ft < 0)
                        continue;
                    var off = s.FieldOffset[f] - (int)m_Header;
                    if (s.IsValueType(ft))
                    {
                        if (ft == x)
                            continue;
                        foreach (var (o, _) in Layout(ft))
                            result.Add((off + o, f));
                    }
                    else
                        result.Add((off, f));
                }
            }
            var array = result.ToArray();
            m_Layouts[t] = array;
            return array;
        }

        HeapSection Section(ulong address)
        {
            var i = Array.BinarySearch(m_HeapStarts, address);
            if (i < 0)
                i = ~i - 1;
            if (i < 0)
                return null;
            var h = m_S.Heap[i];
            return address < h.End ? h : null;
        }

        public bool TryPtr(ulong address, out ulong value)
        {
            value = 0;
            var h = Section(address);
            if (h == null)
                return false;
            var off = (long)(address - h.Start);
            if (off + m_Ptr > h.Bytes.Length)
                return false;
            value = m_Ptr == 8 ? BitConverter.ToUInt64(h.Bytes, (int)off) : BitConverter.ToUInt32(h.Bytes, (int)off);
            return value != 0;
        }

        public int ReadI32(ulong address)
        {
            var h = Section(address);
            if (h == null)
                return -1;
            var off = (long)(address - h.Start);
            return off + 4 > h.Bytes.Length ? -1 : BitConverter.ToInt32(h.Bytes, (int)off);
        }

        public string ReadString(int o, int max = 80)
        {
            var a = Address[o];
            var length = ReadI32(a + m_Header);
            var h = Section(a);
            if (h == null || length < 0)
                return null;
            var start = (long)(a - h.Start) + m_Header + 4;
            var n = (int)Math.Min(length, max);
            if (start + n * 2 > h.Bytes.Length)
                return null;
            var text = System.Text.Encoding.Unicode.GetString(h.Bytes, (int)start, n * 2);
            return length > max ? text + "..." : text;
        }
    }
}
