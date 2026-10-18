using System.IO;
using System.Linq;
using MemoryWaitress.Snap;

namespace MemoryWaitress.Commands
{
    /// <summary>Per-type and per-object numbers in the layout of the package oracle's dump, for diffing against it.</summary>
    public static class Dump
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var all = args.Flag("all");
            args.RejectUnknown();
            var c = m.C;
            w.WriteLine($"managedObjects\t{c.Count}\t{c.TotalSize}");
            foreach (var g in Enumerable.Range(0, c.Count).GroupBy(o => c.Type[o]).OrderByDescending(g => g.Sum(o => c.Size[o])))
                w.WriteLine($"type\t{m.S.TypeName[g.Key]}\t{g.Count()}\t{g.Sum(o => c.Size[o])}");
            for (var i = 0; i < m.S.ObjName.Length; i++)
                w.WriteLine($"obj\t{m.S.ObjInstanceId[i]}\t{m.S.ObjName[i]}\t{m.ObjNative[i]}\t{m.ObjManaged[i]}\t{m.ObjGfx[i]}");
            if (all)
                for (var o = 0; o < c.Count; o++)
                    w.WriteLine($"addr	{c.Address[o]:x}	{m.S.TypeName[c.Type[o]]}	{c.Size[o]}	{Describe.RootSummary(m, o)}	{(c.Parent[o] >= 0 ? m.S.TypeName[c.Type[c.Parent[o]]] : "root")}	{Describe.Via(m, c.ParentVia[o])}");
            return 0;
        }
    }
}
