using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>Unity objects one by one, and objects that look like duplicates of each other.</summary>
    public static class Objects
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var type = args.Get("type");
            var name = args.Get("name");
            var top = Describe.Top(args, 25);
            var sort = args.Get("sort", "total");
            var kind = args.Get("kind");
            args.RejectUnknown();
            var s = m.S;
            Func<int, double> key = sort switch
            {
                "total" => i => m.ObjNative[i] + m.ObjGfx[i] + (ulong)m.ObjManaged[i],
                "native" => i => m.ObjNative[i],
                "gfx" or "graphics" => i => m.ObjGfx[i],
                "managed" => i => m.ObjManaged[i],
                "name" => i => 0,
                _ => throw new WaitressException("--sort takes total, native, gfx, managed or name"),
            };
            if (kind != null && kind is not ("asset" or "runtime" or "dontdestroy" or "manager" or "dontsave"))
                throw new WaitressException("--kind takes asset, runtime, dontdestroy, manager or dontsave");
            var isType = Describe.TypeMatcher(s.NativeTypeName, type);
            var hits = Enumerable.Range(0, s.ObjName.Length)
                .Where(i => isType(s.NativeTypeOf(i)) && Describe.Matches(s.ObjName[i], name) && (kind == null || Describe.Flags(m, i).Split(',').Contains(kind)))
                .ToList();
            var ordered = sort == "name" ? hits.OrderBy(i => s.ObjName[i], StringComparer.OrdinalIgnoreCase) : hits.OrderByDescending(key);
            var shown = (top > 0 ? ordered.Take(top) : ordered).ToList();
            var t = new Table("total", "native", "gfx", "managed", "id", "kind", "type", "name");
            foreach (var i in shown)
                t.Add(Format.Bytes(m.ObjNative[i] + m.ObjGfx[i] + (ulong)m.ObjManaged[i]), Format.Bytes(m.ObjNative[i]), Format.Bytes(m.ObjGfx[i]),
                    Format.Bytes(m.ObjManaged[i]), "#" + s.ObjInstanceId[i], Describe.Flags(m, i), s.NativeTypeOf(i), Describe.Clip(s.ObjName[i], 70));
            t.Write(w);
            ulong native = 0, gfx = 0;
            long managed = 0;
            foreach (var i in hits)
            {
                native += m.ObjNative[i];
                gfx += m.ObjGfx[i];
                managed += m.ObjManaged[i];
            }
            w.WriteLine($"# {hits.Count} objects: native {Format.Bytes(native)}, graphics {Format.Bytes(gfx)}, managed shells {Format.Bytes(managed)}" +
                        (shown.Count < hits.Count ? $"; showing {shown.Count} (--top 0 for all)" : ""));
            return 0;
        }

        public static int Dupes(Model m, Args args, TextWriter w)
        {
            var type = args.Get("type");
            var top = Describe.Top(args, 20);
            args.RejectUnknown();
            var s = m.S;
            var isType = Describe.TypeMatcher(s.NativeTypeName, type);
            var groups = Enumerable.Range(0, s.ObjName.Length)
                .Where(i => s.ObjName[i].Length > 0 && isType(s.NativeTypeOf(i)) && m.ObjNative[i] + m.ObjGfx[i] > 0)
                .GroupBy(i => (s.ObjType[i], s.ObjName[i], m.ObjNative[i], m.ObjGfx[i]))
                .Where(g => g.Count() > 1)
                .Select(g => (g.Key, ids: g.ToList(), wasted: (g.Key.Item3 + g.Key.Item4) * (ulong)(g.Count() - 1)))
                .OrderByDescending(g => g.wasted).ToList();
            var t = new Table("extra", "copies", "each", "type", "name", "ids");
            foreach (var g in top > 0 ? groups.Take(top) : groups)
                t.Add(Format.Bytes(g.wasted), g.ids.Count.ToString(), Format.Bytes(g.Key.Item3 + g.Key.Item4), s.NativeTypeName[g.Key.Item1], Describe.Clip(g.Key.Item2, 50),
                    string.Join(" ", g.ids.Take(4).Select(i => "#" + s.ObjInstanceId[i])) + (g.ids.Count > 4 ? " ..." : ""));
            t.Write(w);
            var total = groups.Aggregate(0UL, (a, g) => a + g.wasted);
            w.WriteLine($"# {groups.Count} groups of objects with the same type, name and size; {Format.Bytes(total)} beyond one copy each");
            w.WriteLine("# same name and size is a hint, not proof: `why <snap> #id` shows what holds each copy");
            return 0;
        }
    }
}
