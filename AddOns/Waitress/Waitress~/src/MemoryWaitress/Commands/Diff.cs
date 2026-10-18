using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>What changed between two snapshots: totals, types that grew or shrank, and Unity objects that appeared or went away.</summary>
    public static class Diff
    {
        public static int Run(Model a, Model b, Args args, TextWriter w)
        {
            var top = Describe.Top(args, 15);
            var filter = args.Get("filter");
            args.RejectUnknown();
            if (string.Equals(Path.GetFullPath(a.S.Path), Path.GetFullPath(b.S.Path), StringComparison.OrdinalIgnoreCase))
                w.WriteLine("# a and b are the same file");
            var sameSession = a.S.SessionGuid != 0 && a.S.SessionGuid == b.S.SessionGuid;
            w.WriteLine($"a  {Path.GetFileName(a.S.Path)}  {a.S.Date:yyyy-MM-dd HH:mm:ss}");
            w.WriteLine($"b  {Path.GetFileName(b.S.Path)}  {b.S.Date:yyyy-MM-dd HH:mm:ss}  ({(sameSession ? "same run: objects matched by instance id" : "different runs: objects matched by type and name")})");
            var t = new Table("", "a", "b", "change");
            void Row(string what, double x, double y) => t.Add(what, Format.Bytes(x), Format.Bytes(y), Signed(y - x));
            if (a.S.Stats.Valid && b.S.Stats.Valid)
                Row("unity used", a.S.Stats.TotalUsed, b.S.Stats.TotalUsed);
            Row("native", a.NativeTotal, b.NativeTotal);
            Row("  unrooted", a.UnrootedNative, b.UnrootedNative);
            Row("graphics", a.GfxTotal, b.GfxTotal);
            if (a.C != null && b.C != null)
            {
                Row("managed objects", a.C.TotalSize, b.C.TotalSize);
                Row("managed heap", a.HeapReserved, b.HeapReserved);
            }
            t.Add("unity objects", a.S.ObjName.Length.ToString(), b.S.ObjName.Length.ToString(), SignedCount(b.S.ObjName.Length - a.S.ObjName.Length));
            if (a.C != null && b.C != null)
                t.Add("managed count", a.C.Count.ToString(), b.C.Count.ToString(), SignedCount(b.C.Count - a.C.Count));
            t.Write(w);

            var ua = UnityTypes(a);
            var ub = UnityTypes(b);
            Changes(w, "unity object types", ua, ub, filter, top);
            if (a.C != null && b.C != null)
                Changes(w, "managed types", ManagedTypes(a), ManagedTypes(b), filter, top);
            if (a.C != null && b.C != null)
                Changes(w, "managed memory by root", ManagedRoots(a), ManagedRoots(b), filter, top);
            Changes(w, "native areas (allocated)", Areas(a), Areas(b), filter, top);
            Changes(w, "native allocators (used)", Allocators(a), Allocators(b), filter, top);

            var added = Added(a, b, sameSession).Where(i => Describe.Matches(b.S.NativeTypeOf(i), filter) || Describe.Matches(b.S.ObjName[i], filter)).ToList();
            var removed = Added(b, a, sameSession).Where(i => Describe.Matches(a.S.NativeTypeOf(i), filter) || Describe.Matches(a.S.ObjName[i], filter)).ToList();
            ObjectList(w, "new unity objects in b", b, added, top);
            ObjectList(w, "unity objects gone in b", a, removed, top);
            return 0;
        }

        static void ObjectList(TextWriter w, string title, Model m, List<int> items, int top)
        {
            if (items.Count == 0)
                return;
            var size = items.Aggregate(0UL, (acc, i) => acc + m.ObjNative[i] + m.ObjGfx[i]);
            w.WriteLine($"{title}: {items.Count}, {Format.Bytes(size)}" + (items.Count > top ? $" (largest {top}; by type: " +
                         string.Join(", ", items.GroupBy(m.S.NativeTypeOf).OrderByDescending(g => g.Count()).Take(5).Select(g => $"{g.Key} {g.Count()}")) + ")" : ""));
            var t = new Table("size", "object");
            foreach (var i in items.OrderByDescending(i => m.ObjNative[i] + m.ObjGfx[i]).Take(top))
                t.Add(Format.Bytes(m.ObjNative[i] + m.ObjGfx[i]), Describe.Native(m, i));
            t.Write(w, "  ");
        }

        static void Changes(TextWriter w, string title, Dictionary<string, (long n, double size)> a, Dictionary<string, (long n, double size)> b, string filter, int top)
        {
            var rows = a.Keys.Union(b.Keys).Where(k => Describe.Matches(k, filter)).Select(k =>
                {
                    a.TryGetValue(k, out var x);
                    b.TryGetValue(k, out var y);
                    return (k, dn: y.n - x.n, ds: y.size - x.size, y);
                })
                .Where(r => r.dn != 0 || Math.Abs(r.ds) >= 1).OrderByDescending(r => Math.Abs(r.ds)).ThenByDescending(r => Math.Abs(r.dn)).ToList();
            if (rows.Count == 0)
                return;
            w.WriteLine($"{title} that changed: {rows.Count}");
            var t = new Table("size", "count", "now", "name");
            foreach (var r in rows.Take(top))
                t.Add(Signed(r.ds), SignedCount(r.dn), Format.Bytes(r.y.size), Describe.ShortType(r.k));
            t.Write(w, "  ");
        }

        static Dictionary<string, (long, double)> UnityTypes(Model m) =>
            Enumerable.Range(0, m.S.ObjName.Length).GroupBy(m.S.NativeTypeOf)
                .ToDictionary(g => g.Key, g => ((long)g.Count(), (double)g.Aggregate(0UL, (acc, i) => acc + m.ObjNative[i] + m.ObjGfx[i])));

        static Dictionary<string, (long, double)> ManagedTypes(Model m) =>
            Enumerable.Range(0, m.C.Count).GroupBy(o => m.TypeName(m.C.Type[o])).ToDictionary(g => g.Key, g => ((long)g.Count(), (double)g.Sum(o => m.C.Size[o])));

        static Dictionary<string, (long, double)> Areas(Model m) =>
            m.S.RootArea.Select((area, i) => (area, i)).GroupBy(x => x.area)
                .ToDictionary(g => g.Key, g => ((long)g.Count(), (double)g.Aggregate(0UL, (acc, x) => acc + m.AllocatedByRoot().GetValueOrDefault(m.S.RootId[x.i]))));

        static Dictionary<string, (long, double)> Allocators(Model m)
        {
            var d = new Dictionary<string, (long, double)>();
            for (var i = 0; i < m.S.AllocatorName.Length; i++)
            {
                d.TryGetValue(m.S.AllocatorName[i], out var v);
                d[m.S.AllocatorName[i]] = (v.Item1 + 1, v.Item2 + m.S.AllocatorUsed[i]);
            }
            return d;
        }

        /// <summary>Managed bytes per root, keyed by the root's description so they line up across snapshots.</summary>
        static Dictionary<string, (long, double)> ManagedRoots(Model m)
        {
            var roots = Managed.RootOf(m.C);
            var names = new Dictionary<int, string>();
            var d = new Dictionary<string, (long, double)>();
            for (var o = 0; o < m.C.Count; o++)
            {
                var r = roots[o];
                if (!names.TryGetValue(r, out var name))
                    names[r] = name = r < 0 ? "?" : Describe.Root(m, m.C.Roots[r]);
                d.TryGetValue(name, out var v);
                d[name] = (v.Item1 + 1, v.Item2 + m.C.Size[o]);
            }
            return d;
        }

        /// <summary>Objects in <paramref name="b"/> with no counterpart in <paramref name="a"/>.</summary>
        static IEnumerable<int> Added(Model a, Model b, bool sameSession)
        {
            if (sameSession)
            {
                var ids = new HashSet<long>(a.S.ObjInstanceId);
                return Enumerable.Range(0, b.S.ObjName.Length).Where(i => !ids.Contains(b.S.ObjInstanceId[i]));
            }
            var counts = new Dictionary<(string, string), int>();
            for (var i = 0; i < a.S.ObjName.Length; i++)
            {
                var key = (a.S.NativeTypeOf(i), a.S.ObjName[i]);
                counts[key] = counts.TryGetValue(key, out var n) ? n + 1 : 1;
            }
            var result = new List<int>();
            for (var i = 0; i < b.S.ObjName.Length; i++)
            {
                var key = (b.S.NativeTypeOf(i), b.S.ObjName[i]);
                if (counts.TryGetValue(key, out var n) && n > 0)
                    counts[key] = n - 1;
                else
                    result.Add(i);
            }
            return result;
        }

        static string Signed(double bytes) => bytes == 0 ? "0" : (bytes > 0 ? "+" : "-") + Format.Bytes(Math.Abs(bytes));

        static string SignedCount(long n) => n > 0 ? "+" + n : n.ToString();
    }
}
