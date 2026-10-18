using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MemoryWaitress.Commands;
using MemoryWaitress.Snap;

namespace MemoryWaitress.Testing
{
    /// <summary>Offline checks of the reader, the crawler and the derived sizes on synthetic snapshots with known contents.</summary>
    public static class SelfTest
    {
        const ulong k_Gc = 0x10000, k_Vm = 0x90000;

        // Object addresses in the synthetic GC section.
        const ulong A = 0x10000, B = 0x10040, Arr1 = 0x10080, C = 0x100C0, Str = 0x10100, HolderArr = 0x10140, Arr2 = 0x10180,
                    NodeArr = 0x101A0, Uo = 0x101D0, Uo2 = 0x101E8, Boxed = 0x10200, Garbage = 0x10220, Arr3 = 0x10260;

        static ulong TypeInfo(int t) => 0x7000 + (ulong)t * 0x100;

        public static int Run(TextWriter w)
        {
            var failures = 0;
            var dir = Path.Combine(Path.GetTempPath(), "memorywaitress-selftest-" + Environment.ProcessId);
            Directory.CreateDirectory(dir);
            try
            {
                var path = Path.Combine(dir, "synthetic.snap");
                Build().Write(path);
                var m = new Model(Snapshot.Load(path));
                failures += Check(w, "reader", () => Reader(m));
                failures += Check(w, "crawl finds exactly the live objects", () => Live(m));
                failures += Check(w, "object sizes", () => Sizes(m));
                failures += Check(w, "mono vtable objects resolve", () => Vtable(m));
                failures += Check(w, "struct statics are followed", () => StructStatic(m));
                failures += Check(w, "shortest paths and referrers", () => Paths(m));
                failures += Check(w, "unity object sizes and shells", () => UnityObjects(m));
                failures += Check(w, "destroyed shells", () => Destroyed(m));
                failures += Check(w, "rejects non-snapshots", () => Rejects(dir));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
            w.WriteLine(failures == 0 ? "all tests passed" : $"{failures} test(s) failed");
            return failures == 0 ? 0 : 1;
        }

        static int Check(TextWriter w, string name, Func<string> test)
        {
            string problem;
            try
            {
                problem = test();
            }
            catch (Exception e)
            {
                problem = e.GetType().Name + ": " + e.Message;
            }
            w.WriteLine((problem == null ? "ok    " : "FAIL  ") + name + (problem == null ? "" : ": " + problem));
            return problem == null ? 0 : 1;
        }

        static string Expect<T>(T actual, T expected, string what) =>
            EqualityComparer<T>.Default.Equals(actual, expected) ? null : $"{what}: expected {expected}, got {actual}";

        static string First(params string[] problems) => problems.FirstOrDefault(p => p != null);

        static int Index(Model m, ulong address) => m.C.ByAddress.TryGetValue(address, out var o) ? o : -1;

        static string Reader(Model m) => First(
            Expect(m.S.Version, 17u, "format version"),
            Expect(m.S.ObjName.FirstOrDefault(), "Tëx", "UTF-8 object name across chunks"),
            Expect(m.S.TypeName[5], "Holder", "type name"),
            Expect(m.S.TypeFields[2].Length, 3, "Node field count"),
            Expect(m.S.TypeStatics[6].Length, 32, "static bytes"),
            Expect(m.S.ObjInstanceId.FirstOrDefault(), 77L, "4-byte instance ids widen"),
            Expect(m.S.Heap.Length, 2, "heap sections"),
            Expect(m.S.Heap.Count(h => h.IsVm), 1, "VM section bit"),
            Expect(m.S.ProductName, "SelfTest", "target info"));

        static string Live(Model m)
        {
            var expected = new[] { A, B, Arr1, C, Str, HolderArr, Arr2, NodeArr, Uo, Uo2, Boxed, Arr3 };
            var found = Enumerable.Range(0, m.C.Count).Select(o => m.C.Address[o]).OrderBy(a => a).ToArray();
            return First(
                Expect(m.C.Count, expected.Length, "live object count"),
                found.SequenceEqual(expected.OrderBy(a => a)) ? null : "found " + string.Join(",", found.Select(a => a.ToString("x"))),
                Index(m, Garbage) < 0 ? null : "unreferenced object was found");
        }

        static string Sizes(Model m)
        {
            long Size(ulong a) => m.C.Size[Index(m, a)];
            return First(
                Expect(Size(A), 40L, "class"),
                Expect(Size(Arr1), 44L, "int[3]"),
                Expect(Size(Arr2), 32L, "empty array"),
                Expect(Size(Str), 32L, "string of 5"),
                Expect(Size(HolderArr), 64L, "struct array"),
                Expect(Size(NodeArr), 48L, "reference array"),
                Expect(Size(Boxed), 20L, "boxed int"),
                Expect(m.C.TotalSize, 448L, "total"),
                Expect(m.C.ReadString(Index(m, Str)), "hello", "string text"));
        }

        static string Vtable(Model m)
        {
            var c = Index(m, C);
            return c < 0 ? "object behind a vtable not found" : Expect(m.S.TypeName[m.C.Type[c]], "Node", "type through vtable");
        }

        static string StructStatic(Model m)
        {
            var o = Index(m, Arr3);
            if (o < 0)
                return "array held by a struct static not found";
            var parent = m.C.Parent[o];
            return parent >= 0 ? "expected a root parent" : Expect(Describe.Root(m, m.C.Roots[-parent - 1]), "static Statics.s_holder", "root");
        }

        static string Paths(Model m)
        {
            var path = string.Join(" | ", Describe.PathToRoot(m, Index(m, Str)));
            var referrersOfA = 0;
            for (var e = 0; e < m.C.EdgeCount; e++)
                if (m.C.EdgeTo[e] == Index(m, A))
                    referrersOfA++;
            return First(
                Expect(path, $"static Statics.s_nodes | -> Node[] @{NodeArr:x} [2] | [1] -> Node @{C:x} | .tag -> System.String @{Str:x} \"hello\"", "path"),
                Expect(referrersOfA, 3, "references to A (GC handle, B.next, Node[0])"),
                Expect(Index(m, B) >= 0 ? m.C.Parent[Index(m, B)] < 0 : false, true, "B is first reached from its static"),
                Expect(Describe.RootSummary(m, Index(m, Boxed)), "static Statics.s_root (2 steps)", "root summary"));
        }

        static string UnityObjects(Model m) => First(
            Expect(m.ObjNative[0], 1000UL, "native size is the root's accumulated size"),
            Expect(m.ObjGfx[0], 4096UL, "graphics by root id"),
            Expect(m.ObjManaged[0], 24L, "managed shell through its GC handle"),
            Expect(m.NativeTotal, 1050UL, "all allocations"),
            Expect(m.C.NativeObject[Index(m, Uo2)], 0, "shell links to its native object"));

        static string Destroyed(Model m)
        {
            var dead = m.DestroyedShells().Select(o => m.C.Address[o]).ToList();
            return dead.Count == 1 && dead[0] == Uo ? null : "destroyed shells: " + string.Join(",", dead.Select(a => a.ToString("x")));
        }

        static string Rejects(string dir)
        {
            var bad = Path.Combine(dir, "bad.snap");
            File.WriteAllBytes(bad, new byte[64]);
            try
            {
                Snapshot.Load(bad);
                return "loaded a file of zeros";
            }
            catch (WaitressException)
            {
                return null;
            }
        }

        static SnapWriter Build()
        {
            var s = new SnapWriter();
            s.Single(Chapter.Version, 17u);
            s.Single(Chapter.RecordDate, new DateTime(2026, 1, 2).Ticks);
            s.Single(Chapter.CaptureFlags, 7u);
            s.Single(Chapter.VirtualMachineInformation, Bytes(w =>
            {
                foreach (var v in new uint[] { 8, 16, 32, 16, 24, 8 })
                    w.Write(v);
            }));
            s.Single(Chapter.TargetInfo, Bytes(w =>
            {
                w.Write(1234u);
                w.Write(7);
                w.Write(18);
                w.Write(0);
                w.Write(0UL);
                w.Write(0UL);
                w.Write(0);
                w.Write(0);
                w.Write(1.0);
                w.Write(4u);
                w.Write(Encoding.UTF8.GetBytes("6000".PadRight(16, '\0')));
                w.Write(8u);
                w.Write(Encoding.UTF8.GetBytes("SelfTest".PadRight(256, '\0')));
                w.Write(new byte[512 - 328]);
            }));

            // Types: 0 Object, 1 String, 2 Node, 3 Int32, 4 Int32[], 5 Holder, 6 Statics, 7 Node[], 8 Holder[], 9 UnityEngine.Object, 10 IntPtr.
            const int valueType = 1, array = 2 | (1 << 16);
            s.Fixed(Chapter.Types_Flags, 0, 0, 0, valueType, array, valueType, 0, array, array, 0, valueType);
            s.Strings(Chapter.Types_Name, "System.Object", "System.String", "Node", "System.Int32", "System.Int32[]", "Holder", "Statics", "Node[]", "Holder[]",
                "UnityEngine.Object", "System.IntPtr");
            s.Strings(Chapter.Types_Assembly, Enumerable.Repeat("Test", 11).ToArray());
            s.Arrays(Chapter.Types_FieldIndices, new int[0], new int[0], new[] { 0, 1, 2 }, new[] { 3 }, new int[0], new[] { 4, 5 }, new[] { 6, 7, 8 }, new int[0],
                new int[0], new[] { 9 }, new int[0]);
            var statics = new byte[32];
            BitConverter.GetBytes(B).CopyTo(statics, 0);
            BitConverter.GetBytes(Arr3).CopyTo(statics, 8);
            BitConverter.GetBytes(2).CopyTo(statics, 16);
            BitConverter.GetBytes(NodeArr).CopyTo(statics, 24);
            s.Arrays(Chapter.Types_StaticFieldBytes, new byte[0], new byte[0], new byte[0], new byte[0], new byte[0], new byte[0], statics, new byte[0], new byte[0],
                new byte[0], new byte[0]);
            s.Fixed(Chapter.Types_BaseOrElement, -1, 0, 0, -1, 3, -1, 0, 2, 5, 0, -1);
            s.Fixed(Chapter.Types_Size, 16, 16, 40, 4, 0, 16, 16, 0, 0, 24, 8);
            s.Fixed(Chapter.Types_TypeInfoAddress, Enumerable.Range(0, 11).Select(TypeInfo).ToArray());
            s.Fixed(Chapter.Types_TypeIndex, Enumerable.Range(0, 11).ToArray());

            // Fields: instance offsets include the 16-byte header; static offsets index the static bytes.
            s.Fixed(Chapter.Fields_Offset, 16, 24, 32, 16, 16, 24, 0, 8, 24, 16);
            s.Fixed(Chapter.Fields_Type, 2, 4, 0, 3, 4, 3, 2, 5, 7, 10);
            s.Strings(Chapter.Fields_Name, "next", "data", "tag", "m_value", "arr", "count", "s_root", "s_holder", "s_nodes", "m_CachedPtr");
            s.Fixed<byte>(Chapter.Fields_IsStatic, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0);

            var gc = new byte[0x300];
            void Ptr(ulong at, ulong value) => BitConverter.GetBytes(value).CopyTo(gc, (int)(at - k_Gc));
            void I32(ulong at, int value) => BitConverter.GetBytes(value).CopyTo(gc, (int)(at - k_Gc));
            Ptr(A, TypeInfo(2)); Ptr(A + 16, B); Ptr(A + 24, Arr1); Ptr(A + 32, HolderArr);
            Ptr(B, TypeInfo(2)); Ptr(B + 16, A); Ptr(B + 32, Boxed);
            Ptr(Arr1, TypeInfo(4)); I32(Arr1 + 24, 3); I32(Arr1 + 32, 1); I32(Arr1 + 36, 2); I32(Arr1 + 40, 3);
            Ptr(C, k_Vm); Ptr(C + 32, Str);
            Ptr(Str, TypeInfo(1)); I32(Str + 16, 5); Encoding.Unicode.GetBytes("hello").CopyTo(gc, (int)(Str + 20 - k_Gc));
            Ptr(HolderArr, TypeInfo(8)); I32(HolderArr + 24, 2); Ptr(HolderArr + 32, Arr2); I32(HolderArr + 40, 7); I32(HolderArr + 56, 9);
            Ptr(Arr2, TypeInfo(4));
            Ptr(NodeArr, TypeInfo(7)); I32(NodeArr + 24, 2); Ptr(NodeArr + 32, A); Ptr(NodeArr + 40, C);
            Ptr(Uo, TypeInfo(9));
            Ptr(Uo2, TypeInfo(9)); Ptr(Uo2 + 16, 0x5555);
            Ptr(Boxed, TypeInfo(3)); I32(Boxed + 16, 42);
            Ptr(Garbage, TypeInfo(2)); Ptr(Garbage + 16, A);
            Ptr(Arr3, TypeInfo(4)); I32(Arr3 + 24, 2);
            var vm = BitConverter.GetBytes(TypeInfo(2));
            s.Fixed(Chapter.ManagedHeap_Start, k_Gc, k_Vm | (1UL << 63));
            s.Dynamic(Chapter.ManagedHeap_Bytes, gc, vm);
            s.Fixed(Chapter.GCHandles_Target, A, Uo, Uo2);

            s.Strings(Chapter.NativeTypes_Name, "Texture2D");
            s.Fixed(Chapter.NativeTypes_BaseType, -1);
            s.Fixed(Chapter.NativeObjects_Type, 0);
            s.Fixed(Chapter.NativeObjects_HideFlags, 0u);
            s.Fixed(Chapter.NativeObjects_Flags, 2);
            s.Fixed(Chapter.NativeObjects_InstanceId, 77);
            s.Strings(Chapter.NativeObjects_Name, "Tëx");
            s.Fixed(Chapter.NativeObjects_Address, 0x5555UL);
            s.Fixed(Chapter.NativeObjects_Size, 100UL);
            s.Fixed(Chapter.NativeObjects_RootId, 5L);
            s.Fixed(Chapter.NativeObjects_GCHandleIndex, 2);
            s.Fixed(Chapter.Roots_Id, 5L, 6L);
            s.Strings(Chapter.Roots_AreaName, "Objects", "Other");
            s.Strings(Chapter.Roots_ObjectName, "Tëx", "thing");
            s.Fixed(Chapter.Roots_AccumulatedSize, 1000UL, 50UL);
            s.Fixed(Chapter.Allocations_RootId, 5L, 5L, 6L);
            s.Fixed(Chapter.Allocations_Size, 600UL, 400UL, 50UL);
            s.Fixed(Chapter.Allocations_RegionIndex, 0, 0, 0);
            s.Fixed(Chapter.Gfx_Size, 4096UL);
            s.Fixed(Chapter.Gfx_RootId, 5L);
            return s;
        }

        static byte[] Bytes(Action<BinaryWriter> write)
        {
            using var stream = new MemoryStream();
            using (var w = new BinaryWriter(stream))
                write(w);
            return stream.ToArray();
        }
    }
}
