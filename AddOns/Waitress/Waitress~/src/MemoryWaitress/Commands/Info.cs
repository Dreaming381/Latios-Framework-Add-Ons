using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    public static class Info
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var top = Describe.Top(args, 8);
            args.RejectUnknown();
            var s = m.S;
            var c = m.C;
            w.WriteLine($"snapshot  {Path.GetFileName(s.Path)}  {Format.Bytes(new FileInfo(s.Path).Length)}, taken {s.Date:yyyy-MM-dd HH:mm:ss}, format {s.Version}");
            w.WriteLine($"target    {s.ProductName}, {s.Platform}, Unity {s.UnityVersion}, {s.GraphicsApi}, {Format.Seconds(s.TimeSinceStartup)} after start");
            if (s.IsEditor)
                w.WriteLine("          Editor capture: Editor tooling, assets it has loaded and the Editor's own heap are mixed in; a development player gives game-only numbers");
            if (s.Stats.Valid)
            {
                var st = s.Stats;
                w.WriteLine($"unity     used {Format.Bytes(st.TotalUsed)} of {Format.Bytes(st.TotalReserved)} reserved: managed heap {Format.Bytes(st.GcHeapUsed)} used / {Format.Bytes(st.GcHeapReserved)}, " +
                            $"graphics {Format.Bytes(st.Graphics)}, audio {Format.Bytes(st.Audio)}, profiler {Format.Bytes(st.ProfilerUsed + st.MemoryProfilerUsed)}");
            }
            if (s.SystemSize.Length > 0)
            {
                ulong committed = 0, resident = 0;
                for (var i = 0; i < s.SystemSize.Length; i++)
                {
                    committed += s.SystemSize[i];
                    resident += s.SystemResident[i];
                }
                w.WriteLine($"process   {Format.Bytes(resident)} resident in {s.SystemSize.Length} regions ({Format.Bytes(committed)} mapped)");
            }

            w.WriteLine($"native    {Format.Bytes(m.NativeTotal)} in {s.AllocSize.Length} allocations; {Format.Bytes(m.NativeInObjects)} of it under {s.ObjName.Length} Unity objects");
            var allocated = m.AllocatedByRoot();
            var areas = s.RootArea.Select((a, i) => (a, size: allocated.GetValueOrDefault(s.RootId[i]))).GroupBy(x => x.a)
                .Select(g => (area: g.Key, size: g.Aggregate(0UL, (acc, x) => acc + x.size))).Append((area: "(unrooted)", size: m.UnrootedNative))
                .OrderByDescending(x => x.size).Take(top).ToList();
            if (areas.Count > 0)
                w.WriteLine("          by area: " + string.Join(", ", areas.Select(a => $"{a.area} {Format.Bytes(a.size)}")));
            var mapped = Enumerable.Range(0, s.RootId.Length).Where(r => s.RootArea[r] == "System" && s.RootObject[r] == "ExecutableAndDlls").Aggregate(0UL, (acc, r) => acc + s.RootSize[r]);
            if (mapped > 0)
                w.WriteLine($"          plus {Format.Bytes(mapped)} of executables and DLLs mapped into the process (not allocations)");
            w.WriteLine($"graphics  {Format.Bytes(m.GfxTotal)} in {s.GfxSize.Length} resources; {Format.Bytes(m.GfxInObjects)} of it owned by Unity objects");
            if (c != null)
            {
                w.WriteLine($"managed   {Format.Bytes(c.TotalSize)} in {c.Count} live objects; heap {Format.Bytes(m.HeapReserved)} in {s.Heap.Count(h => !h.IsVm)} sections " +
                            $"({Format.Pct(m.HeapReserved == 0 ? 0 : 1 - (double)c.TotalSize / m.HeapReserved)} free space or garbage not yet collected), runtime metadata {Format.Bytes(m.VmReserved)}");
                if (c.Unresolved > 0)
                    w.WriteLine($"          {c.Unresolved} references point outside the captured heap or at data with no known type; they are skipped");
            }

            var byType = Enumerable.Range(0, s.ObjName.Length).GroupBy(s.NativeTypeOf)
                .Select(g => (type: g.Key, n: g.Count(), native: Sum(g, i => m.ObjNative[i]), gfx: Sum(g, i => m.ObjGfx[i])))
                .OrderByDescending(x => x.native + x.gfx).Take(top).ToList();
            w.WriteLine("unity objects by type:");
            var t = new Table("native", "graphics", "count", "type");
            foreach (var x in byType)
                t.Add(Format.Bytes(x.native), Format.Bytes(x.gfx), x.n.ToString(), x.type);
            t.Write(w, "  ");

            if (c != null)
            {
                var managed = Enumerable.Range(0, c.Count).GroupBy(o => c.Type[o])
                    .Select(g => (type: g.Key, n: g.Count(), size: g.Sum(o => c.Size[o]))).OrderByDescending(x => x.size).Take(top).ToList();
                w.WriteLine("managed objects by type:");
                t = new Table("size", "count", "type");
                foreach (var x in managed)
                    t.Add(Format.Bytes(x.size), x.n.ToString(), Describe.ShortType(m.TypeName(x.type)));
                t.Write(w, "  ");
                var dead = m.DestroyedShells();
                if (dead.Count > 0)
                {
                    var kinds = dead.GroupBy(o => c.Type[o]).OrderByDescending(g => g.Count()).Take(4).Select(g => $"{m.TypeName(g.Key)} {g.Count()}");
                    w.WriteLine($"leaks     {dead.Count} destroyed Unity objects are still referenced from managed code ({string.Join(", ", kinds)}); see `leaks`");
                }
            }
            w.WriteLine($"# read in {s.LoadSeconds:0.0} s, crawled in {s.CrawlSeconds:0.0} s");
            return 0;
        }

        static ulong Sum(System.Collections.Generic.IEnumerable<int> items, Func<int, ulong> f)
        {
            ulong total = 0;
            foreach (var i in items)
                total += f(i);
            return total;
        }
    }
}
