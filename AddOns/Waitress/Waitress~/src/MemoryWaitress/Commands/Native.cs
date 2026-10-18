using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>Native memory grouped by area, root, allocator, region or label, or the largest single allocations.</summary>
    public static class Native
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var by = args.Get("by", "area");
            var filter = args.Get("filter");
            var top = Describe.Top(args, 25);
            args.RejectUnknown();
            var s = m.S;
            switch (by)
            {
                case "area":
                case "root":
                {
                    var owner = s.ObjectByRootId();
                    var allocated = m.AllocatedByRoot();
                    string Name(int r) => by == "area" ? s.RootArea[r] : $"{s.RootArea[r]} / {(owner.TryGetValue(s.RootId[r], out var o) ? Describe.Native(m, o) : s.RootObject[r])}";
                    var rows = Enumerable.Range(0, s.RootId.Length).GroupBy(Name)
                        .Select(g => (name: g.Key, alloc: g.Aggregate(0UL, (a, r) => a + allocated.GetValueOrDefault(s.RootId[r])),
                            reported: g.Aggregate(0UL, (a, r) => a + s.RootSize[r]), roots: g.Count()))
                        .ToList();
                    rows.Add(("(unrooted: allocations with no root)", m.UnrootedNative, 0, 0));
                    var hits = rows.Where(r => Describe.Matches(r.name, filter)).OrderByDescending(r => System.Math.Max(r.alloc, r.reported)).ToList();
                    var t = by == "area" ? new Table("allocated", "reported", "roots", "area") : new Table("allocated", "reported", "root");
                    foreach (var r in top > 0 ? hits.Take(top) : hits)
                    {
                        if (by == "area")
                            t.Add(Format.Bytes(r.alloc), Format.Bytes(r.reported), r.roots.ToString(), r.name);
                        else
                            t.Add(Format.Bytes(r.alloc), Format.Bytes(r.reported), r.name);
                    }
                    t.Write(w);
                    w.WriteLine($"# {hits.Count} {by}s: allocated {Format.Bytes(hits.Aggregate(0UL, (a, r) => a + r.alloc))} of {Format.Bytes(m.NativeTotal)} native allocations" +
                                (top > 0 && hits.Count > top ? $"; showing {top} (--top 0 for all)" : ""));
                    w.WriteLine("# allocated: native allocations rooted there (these sum to the total). reported: Unity's accumulated size for the root, which is what");
                    w.WriteLine("# Unity objects show as native size; it can include memory that isn't an allocation, such as mapped executables (System / ExecutableAndDlls)");
                    return 0;
                }
                case "allocation":
                {
                    var owner = s.ObjectByRootId();
                    string Where(int a)
                    {
                        var id = s.AllocRootId[a];
                        if (owner.TryGetValue(id, out var o))
                            return Describe.Native(m, o);
                        return s.RootIndexById.TryGetValue(id, out var r) ? $"{s.RootArea[r]} / {s.RootObject[r]}" : "(unrooted)";
                    }
                    string Region(int a) => s.AllocRegion[a] >= 0 && s.AllocRegion[a] < s.RegionName.Length ? s.RegionName[s.AllocRegion[a]] : "?";
                    var hits = Enumerable.Range(0, s.AllocSize.Length).OrderByDescending(a => s.AllocSize[a])
                        .Where(a => filter == null || Describe.Matches(Where(a), filter) || Describe.Matches(Region(a), filter))
                        .Take(top > 0 ? top : int.MaxValue).ToList();
                    var t = new Table("size", "bytes", "root", "region");
                    foreach (var a in hits)
                        t.Add(Format.Bytes(s.AllocSize[a]), s.AllocSize[a].ToString(), Where(a), Region(a));
                    t.Write(w);
                    w.WriteLine("# single allocations, largest first. A NativeArray or UnsafeUtility.Malloc shows up under UnsafeUtility with its exact byte size");
                    return 0;
                }
                case "size":
                {
                    var owner = s.ObjectByRootId();
                    string Where(int a) => owner.TryGetValue(s.AllocRootId[a], out var o) ? s.NativeTypeOf(o) + " objects"
                        : s.RootIndexById.TryGetValue(s.AllocRootId[a], out var r) ? $"{s.RootArea[r]} / {s.RootObject[r]}" : "(unrooted)";
                    var groups = Enumerable.Range(0, s.AllocSize.Length).Where(a => filter == null || Describe.Matches(Where(a), filter))
                        .GroupBy(a => (s.AllocSize[a], Where(a))).Select(g => (g.Key.Item1, root: g.Key.Item2, n: g.Count()))
                        .OrderByDescending(g => g.Item1 * (ulong)g.n).ToList();
                    var t = new Table("total", "count", "each", "root");
                    foreach (var g in top > 0 ? groups.Take(top) : groups)
                        t.Add(Format.Bytes(g.Item1 * (ulong)g.n), g.n.ToString(), g.Item1.ToString(), g.root);
                    t.Write(w);
                    w.WriteLine("# allocations grouped by exact size and root, largest total first. Entities chunks come in 1048576-byte UnsafeUtility");
                    w.WriteLine("# Malloc(Persistent) blocks of 64 chunks each, so that row counts the megachunks of every World");
                    return 0;
                }
                case "allocator":
                case "region":
                case "label":
                {
                    List<(string name, ulong size, string extra)> rows;
                    Table t;
                    if (by == "allocator")
                    {
                        rows = s.AllocatorName.Select((a, i) => (a, s.AllocatorUsed[i], Format.Bytes(s.AllocatorReserved[i]))).ToList();
                        t = new Table("used", "reserved", "allocator");
                    }
                    else if (by == "region")
                    {
                        var sizes = new ulong[s.RegionName.Length];
                        for (var a = 0; a < s.AllocSize.Length; a++)
                            if (s.AllocRegion[a] >= 0 && s.AllocRegion[a] < sizes.Length)
                                sizes[s.AllocRegion[a]] += s.AllocSize[a];
                        rows = s.RegionName.Select((r, i) => (r, sizes[i], Format.Bytes(s.RegionSize[i]))).ToList();
                        t = new Table("allocated", "reserved", "region");
                    }
                    else
                    {
                        // Unity can report a label's size below zero (a wrapped unsigned value); those are left out of the sums.
                        rows = s.LabelName.Select((l, i) => (l, i < s.LabelSize.Length ? s.LabelSize[i] : 0, i < s.LabelSize.Length && s.LabelSize[i] > long.MaxValue ? "negative" : ""))
                            .Select(r => r.Item3 == "negative" ? (r.l, 0UL, r.Item3) : r).ToList();
                        t = new Table("size", "", "label");
                    }
                    var hits = rows.Where(r => Describe.Matches(r.name, filter)).OrderByDescending(r => r.size).ToList();
                    foreach (var r in top > 0 ? hits.Take(top) : hits)
                        t.Add(r.extra == "negative" ? "?" : Format.Bytes(r.size), r.extra == "negative" ? "(reported negative)" : r.extra, r.name);
                    t.Write(w);
                    w.WriteLine($"# {hits.Count} {by}s, {Format.Bytes(hits.Aggregate(0UL, (a, r) => a + r.size))}" + (top > 0 && hits.Count > top ? $"; showing {top} (--top 0 for all)" : ""));
                    return 0;
                }
                default:
                    throw new WaitressException("--by takes area, root, allocation, size, allocator, region or label");
            }
        }
    }
}
