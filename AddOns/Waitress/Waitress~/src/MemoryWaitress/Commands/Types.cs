using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>Totals per type: Unity object types (native + graphics + shells) and managed types.</summary>
    public static class Types
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var filter = args.Get("filter");
            var top = Describe.Top(args, 25);
            var onlyNative = args.Flag("native");
            var onlyManaged = args.Flag("managed");
            args.RejectUnknown();
            var s = m.S;
            if (!onlyManaged)
            {
                var groups = Enumerable.Range(0, s.ObjName.Length).Where(i => Describe.Matches(s.NativeTypeOf(i), filter)).GroupBy(s.NativeTypeOf)
                    .Select(g => (type: g.Key, n: g.Count(), native: g.Aggregate(0UL, (a, i) => a + m.ObjNative[i]), gfx: g.Aggregate(0UL, (a, i) => a + m.ObjGfx[i]),
                        managed: g.Sum(i => m.ObjManaged[i])))
                    .OrderByDescending(x => x.native + x.gfx + (ulong)x.managed).ToList();
                w.WriteLine("unity object types:");
                var t = new Table("total", "native", "gfx", "managed", "count", "type");
                foreach (var x in top > 0 ? groups.Take(top) : groups)
                    t.Add(Format.Bytes(x.native + x.gfx + (ulong)x.managed), Format.Bytes(x.native), Format.Bytes(x.gfx), Format.Bytes(x.managed), x.n.ToString(), x.type);
                t.Write(w, "  ");
                if (top > 0 && groups.Count > top)
                    w.WriteLine($"  # {groups.Count - top} more types (--top 0)");
            }
            if (!onlyNative && m.C != null)
            {
                var c = m.C;
                var groups = Enumerable.Range(0, c.Count).GroupBy(o => c.Type[o]).Where(g => Describe.Matches(m.TypeName(g.Key), filter))
                    .Select(g => (type: g.Key, n: g.Count(), size: g.Sum(o => c.Size[o]), biggest: g.Max(o => c.Size[o])))
                    .OrderByDescending(x => x.size).ToList();
                w.WriteLine("managed types:");
                var t = new Table("size", "count", "largest", "type");
                foreach (var x in top > 0 ? groups.Take(top) : groups)
                    t.Add(Format.Bytes(x.size), x.n.ToString(), Format.Bytes(x.biggest), Describe.ShortType(m.TypeName(x.type)));
                t.Write(w, "  ");
                if (top > 0 && groups.Count > top)
                    w.WriteLine($"  # {groups.Count - top} more types (--top 0)");
            }
            return 0;
        }
    }
}
