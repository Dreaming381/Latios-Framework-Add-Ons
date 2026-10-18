using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>Managed objects one by one or grouped by the root that holds them, and destroyed Unity objects still referenced.</summary>
    public static class Managed
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var type = args.Get("type");
            var top = Describe.Top(args, 25);
            var by = args.Get("by", "object");
            var rootPattern = args.Get("root");
            args.RejectUnknown();
            var c = Need(m);
            var types = Describe.ManagedTypeSet(m, type);
            var hits = Enumerable.Range(0, c.Count).Where(o => types == null || types.Contains(c.Type[o])).ToList();
            if (rootPattern != null)
                hits = HeldBy(m, hits, rootPattern);
            if (by == "root")
            {
                var roots = RootOf(c);
                var groups = hits.GroupBy(o => roots[o]).Select(g => (root: g.Key, n: g.Count(), size: g.Sum(o => c.Size[o]))).OrderByDescending(x => x.size).ToList();
                var t = new Table("size", "objects", "root");
                foreach (var x in top > 0 ? groups.Take(top) : groups)
                    t.Add(Format.Bytes(x.size), x.n.ToString(), x.root < 0 ? "?" : Describe.Root(m, c.Roots[x.root]));
                t.Write(w);
                w.WriteLine($"# {groups.Count} roots hold {hits.Count} objects, {Format.Bytes(hits.Sum(o => c.Size[o]))}; each object counts under the first root that reaches it " +
                            "(shortest path), so shared objects appear once");
                return 0;
            }
            if (by != "object")
                throw new WaitressException("--by takes object or root");
            var shown = hits.OrderByDescending(o => c.Size[o]).Take(top > 0 ? top : int.MaxValue).ToList();
            var table = new Table("size", "object", "held by");
            foreach (var o in shown)
                table.Add(Format.Bytes(c.Size[o]), Describe.Managed(m, o), Describe.RootSummary(m, o));
            table.Write(w);
            w.WriteLine($"# {hits.Count} objects, {Format.Bytes(hits.Sum(o => c.Size[o]))}" + (shown.Count < hits.Count ? $"; showing {shown.Count} (--top 0 for all)" : "") +
                        "; `why <snap> @address` prints the full path");
            return 0;
        }

        public static int Leaks(Model m, Args args, TextWriter w)
        {
            var top = Describe.Top(args, 25);
            var type = args.Get("type");
            var rootPattern = args.Get("root");
            args.RejectUnknown();
            Need(m);
            var types = Describe.ManagedTypeSet(m, type);
            var dead = m.DestroyedShells();
            var all = dead.Count;
            dead = dead.Where(o => types == null || types.Contains(m.C.Type[o])).ToList();
            if (rootPattern != null)
                dead = HeldBy(m, dead, rootPattern);
            if (dead.Count == 0)
            {
                w.WriteLine(all == 0 ? "no destroyed Unity objects are referenced from managed code" : $"none of the {all} destroyed Unity objects referenced from managed code match");
                return 0;
            }
            var t = new Table("object", "held by");
            foreach (var o in dead.OrderBy(o => Describe.RootSummary(m, o)).Take(top > 0 ? top : int.MaxValue))
                t.Add(Describe.Managed(m, o), Describe.RootSummary(m, o));
            t.Write(w);
            if (all != dead.Count)
                w.WriteLine($"# {dead.Count} of {all} match the filters");
            w.WriteLine($"# {dead.Count} managed objects of UnityEngine.Object types whose native object was destroyed (m_CachedPtr is null), kept alive by managed references");
            w.WriteLine("# each costs only its shell, but the references usually mean a missed cleanup; `why <snap> @address` for the full path");
            return 0;
        }

        /// <summary>The objects whose root matches a pattern (substring, or * and ? wildcards) in its description.</summary>
        public static System.Collections.Generic.List<int> HeldBy(Model m, System.Collections.Generic.List<int> objects, string pattern)
        {
            var roots = RootOf(m.C);
            var text = new System.Collections.Generic.Dictionary<int, bool>();
            return objects.Where(o =>
            {
                var r = roots[o];
                if (!text.TryGetValue(r, out var hit))
                    text[r] = hit = r >= 0 && Describe.Matches(Describe.Root(m, m.C.Roots[r]), pattern);
                return hit;
            }).ToList();
        }

        /// <summary>The root each object hangs from; parents always precede children in walk order.</summary>
        public static int[] RootOf(Crawl c)
        {
            var roots = new int[c.Count];
            for (var o = 0; o < c.Count; o++)
            {
                var p = c.Parent[o];
                roots[o] = p == Crawl.NoParent ? -1 : p < 0 ? -p - 1 : roots[p];
            }
            return roots;
        }

        static Crawl Need(Model m) => m.C ?? throw new WaitressException("this snapshot has no managed data (captured without managed objects)");
    }
}
