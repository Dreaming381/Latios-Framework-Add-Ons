using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>What keeps an object alive: the path from a root for managed objects, the referrers and a referrer chain for Unity objects.</summary>
    public static class Why
    {
        public static int Run(Model m, Args args, TextWriter w)
        {
            var target = args.PositionalAt(1, "object: #instanceId, @address (managed), a Unity object name, a managed type, or a static field");
            var max = Describe.Top(args, 8);
            var forceManaged = args.Flag("managed");
            args.RejectUnknown();
            var plain = !target.StartsWith("#") && !target.StartsWith("@") && !target.StartsWith("0x", System.StringComparison.OrdinalIgnoreCase);
            var types = m.C != null && plain ? ManagedTypes(m, target) : new HashSet<int>();
            if (forceManaged)
            {
                if (types.Count == 0)
                    throw new WaitressException($"no managed type matches '{target}'");
                TypeWhy(m, types, max, w);
                return 0;
            }
            var (native, managed) = Describe.Find(m, target);
            if (managed.Count > 0)
            {
                ManagedWhy(m, managed[0], max, w);
                return 0;
            }
            if (native.Count == 0 && types.Count > 0)
            {
                TypeWhy(m, types, max, w);
                return 0;
            }
            if (native.Count == 0 && m.C != null && plain && StaticWhy(m, target, max, w))
                return 0;
            if (native.Count == 0)
                throw new WaitressException($"nothing matches '{target}': use #instanceId or @address from another command's output, a Unity object name, " +
                                            "a managed type name, or a static field (Type.field or field)");
            var ordered = native.OrderByDescending(i => m.ObjNative[i] + m.ObjGfx[i]).ToList();
            if (ordered.Count > 1)
            {
                w.WriteLine($"# {ordered.Count} objects match '{target}'; showing the largest. Others: " +
                            string.Join(", ", ordered.Skip(1).Take(6).Select(i => $"{m.S.NativeTypeOf(i)} #{m.S.ObjInstanceId[i]}")) + (ordered.Count > 7 ? ", ..." : ""));
            }
            if (types.Count > 0)
                w.WriteLine($"# a managed type is also named '{target}': `why <snap> {target} --managed` for its instances");
            NativeWhy(m, ordered[0], max, w);
            return 0;
        }

        /// <summary>Managed types named by full name, by name without namespace (raw or as printed), or by a glob.</summary>
        static HashSet<int> ManagedTypes(Model m, string pattern)
        {
            if (pattern.IndexOfAny(new[] { '*', '?' }) < 0 && !m.S.TypeName.Any(n => Describe.NamedBy(n, pattern)))
                return new HashSet<int>();
            return Describe.ManagedTypeSet(m, pattern);
        }

        /// <summary>A static field named Type.field or field: what it points at and everything first reached through it.</summary>
        static bool StaticWhy(Model m, string target, int max, TextWriter w)
        {
            var c = m.C;
            var hits = Enumerable.Range(0, c.Roots.Count).Where(r => c.Roots[r].Kind == RootKind.Static)
                .Where(r =>
                {
                    var root = c.Roots[r];
                    var field = m.S.FieldName[root.Field];
                    var owner = m.S.TypeName[root.Index];
                    return field == target || (owner + "." + field).EndsWith("." + target, System.StringComparison.Ordinal) || owner + "." + field == target;
                }).ToList();
            if (hits.Count == 0)
                return false;
            var roots = Managed.RootOf(c);
            foreach (var r in hits.Take(max))
            {
                var held = Enumerable.Range(0, c.Count).Where(o => roots[o] == r).ToList();
                w.WriteLine($"{Describe.Root(m, c.Roots[r])}: {held.Count} objects, {Format.Bytes(held.Sum(o => c.Size[o]))} first reached through it");
                foreach (var g in held.GroupBy(o => c.Type[o]).OrderByDescending(g => g.Sum(o => c.Size[o])).Take(5))
                    w.WriteLine($"  {Format.Bytes(g.Sum(o => c.Size[o])),8}  {g.Count(),7}  {Describe.ShortType(m.TypeName(g.Key))}");
            }
            if (hits.Count > max)
                w.WriteLine($"... {hits.Count - max} more static fields match (--top)");
            return true;
        }

        /// <summary>Every live instance of some types, grouped by the root that holds them, with one full path per root.</summary>
        static void TypeWhy(Model m, HashSet<int> types, int max, TextWriter w)
        {
            var c = m.C;
            var instances = Enumerable.Range(0, c.Count).Where(o => types.Contains(c.Type[o])).ToList();
            var live = instances.Select(o => c.Type[o]).Distinct().ToList();
            var names = string.Join(", ", live.Select(t => Describe.ShortType(m.TypeName(t))).Take(4)) + (live.Count > 4 ? $", ... ({live.Count} types)" : "");
            if (instances.Count == 0)
            {
                w.WriteLine($"no live instances of {types.Count} matching type{(types.Count == 1 ? "" : "s")} ({string.Join(", ", types.Take(3).Select(t => Describe.ShortType(m.TypeName(t))))}{(types.Count > 3 ? ", ..." : "")})");
                return;
            }
            var roots = Managed.RootOf(c);
            var groups = instances.GroupBy(o => roots[o]).OrderByDescending(g => g.Count()).ToList();
            w.WriteLine($"{instances.Count} live instance{(instances.Count == 1 ? "" : "s")} of {names}, {Format.Bytes(instances.Sum(o => c.Size[o]))}, held through {groups.Count} root{(groups.Count == 1 ? "" : "s")}:");
            foreach (var g in groups.Take(max))
            {
                var example = g.First();
                w.WriteLine($"  {g.Count()} instance{(g.Count() == 1 ? "" : "s")}, {Format.Bytes(g.Sum(o => c.Size[o]))}: {(g.Key < 0 ? "?" : Describe.Root(m, c.Roots[g.Key]))}");
                w.WriteLine("    e.g. " + string.Join("  ", Describe.PathToRoot(m, example)));
            }
            if (groups.Count > max)
                w.WriteLine($"  ... {groups.Count - max} more roots (--top)");
        }

        static void ManagedWhy(Model m, int o, int max, TextWriter w)
        {
            var c = m.C;
            w.WriteLine($"{Describe.Managed(m, o)}  {Format.Bytes(c.Size[o])}");
            var below = Below(c, o);
            if (below.count > 1)
                w.WriteLine($"reaches first: {below.count - 1} more objects, {Format.Bytes(below.bytes - c.Size[o])} (objects whose shortest path from a root runs through it; some may also be reachable another way, so this is not exactly what freeing it would free)");
            w.WriteLine("path from root (shortest):");
            foreach (var step in Describe.PathToRoot(m, o))
                w.WriteLine("  " + step);
            var holder = HolderPath(m, o);
            if (holder != null)
            {
                w.WriteLine("shortest path through other code (its own GC handle aside):");
                foreach (var step in holder)
                    w.WriteLine("  " + step);
            }
            var referrers = Referrers(c, o);
            if (referrers.Count > 1)
            {
                w.WriteLine($"all {referrers.Count} references to it:");
                foreach (var (from, via) in referrers.Take(max))
                    w.WriteLine("  " + (from >= 0 ? $"{Describe.Managed(m, from)}{Describe.Via(m, via)}" : Describe.Root(m, c.Roots[-from - 1])));
                if (referrers.Count > max)
                    w.WriteLine($"  ... {referrers.Count - max} more (--top)");
            }
        }

        static void NativeWhy(Model m, int i, int max, TextWriter w)
        {
            var s = m.S;
            w.WriteLine($"{Describe.Native(m, i)}  native {Format.Bytes(m.ObjNative[i])}, graphics {Format.Bytes(m.ObjGfx[i])}, managed shell {Format.Bytes(m.ObjManaged[i])}  ({Describe.Flags(m, i)})");
            var byId = new Dictionary<long, int>(s.ObjInstanceId.Length);
            for (var k = 0; k < s.ObjInstanceId.Length; k++)
                byId.TryAdd(s.ObjInstanceId[k], k);
            var referrers = new Dictionary<int, List<int>>();
            for (var k = 0; k < s.ConnFrom.Length; k++)
            {
                if (!byId.TryGetValue(s.ConnTo[k], out var to) || !byId.TryGetValue(s.ConnFrom[k], out var from) || from == to)
                    continue;
                if (!referrers.TryGetValue(to, out var list))
                    referrers[to] = list = new List<int>();
                list.Add(from);
            }
            var direct = referrers.TryGetValue(i, out var d) ? d.Distinct().ToList() : new List<int>();
            w.WriteLine($"native referrers: {direct.Count}");
            foreach (var r in direct.Take(max))
                w.WriteLine("  " + Describe.Native(m, r) + $"  ({Describe.Flags(m, r)})");
            if (direct.Count > max)
                w.WriteLine($"  ... {direct.Count - max} more (--top)");

            var c = m.C;
            var shell = m.ObjShell[i];
            if (c != null && shell >= 0)
            {
                var managedRefs = Referrers(c, shell).Where(r => r.from >= 0 || c.Roots[-r.from - 1].Kind == RootKind.Static).ToList();
                w.WriteLine($"managed referrers of its shell @{c.Address[shell]:x}: {managedRefs.Count}");
                foreach (var (from, via) in managedRefs.Take(max))
                    w.WriteLine("  " + (from >= 0 ? $"{Describe.Managed(m, from)}{Describe.Via(m, via)}  <- {Describe.RootSummary(m, from)}" : Describe.Root(m, c.Roots[-from - 1])));
                if (managedRefs.Count > max)
                    w.WriteLine($"  ... {managedRefs.Count - max} more (--top)");
                var holder = HolderPath(m, shell);
                if (holder != null)
                {
                    w.WriteLine("shortest managed path to its shell:");
                    foreach (var step in holder)
                        w.WriteLine("  " + step);
                }
            }

            // Walk referrers upward breadth-first to the nearest object nothing else references.
            var parent = new Dictionary<int, int> { [i] = -1 };
            var queue = new Queue<int>();
            queue.Enqueue(i);
            var end = -1;
            while (queue.Count > 0 && end < 0)
            {
                var x = queue.Dequeue();
                if (!referrers.TryGetValue(x, out var ups) || ups.All(parent.ContainsKey))
                {
                    if (x != i)
                        end = x;
                    continue;
                }
                foreach (var u in ups)
                {
                    if (parent.ContainsKey(u))
                        continue;
                    parent[u] = x;
                    queue.Enqueue(u);
                }
            }
            if (end >= 0)
            {
                var chain = new List<string>();
                for (var x = end; x >= 0; x = parent[x])
                    chain.Add(Describe.Native(m, x));
                w.WriteLine("referrer chain (top first):");
                foreach (var link in chain)
                    w.WriteLine("  " + link);
                w.WriteLine("  # the top object has no further native referrers: it is a loaded asset or scene object, or held by managed code or the engine");
            }
            else if (direct.Count == 0)
                w.WriteLine("nothing native references it: it is alive because it is loaded (an asset or scene object), owned by the engine, or held by managed code");
        }

        /// <summary>
        /// The shortest path from a root to an object that doesn't go through the object's own GC handle (a Unity object's handle to its shell
        /// says nothing about who uses it), or null when there is no other path.
        /// </summary>
        static List<string> HolderPath(Model m, int o)
        {
            var c = m.C;
            var own = c.Parent[o];
            if (own >= 0 || own == Crawl.NoParent || c.Roots[-own - 1].Kind != RootKind.GcHandle)
                return null;
            var depth = new int[c.Count];
            for (var x = 0; x < c.Count; x++)
                depth[x] = c.Parent[x] >= 0 ? depth[c.Parent[x]] + 1 : 0;
            var best = (from: -1, via: -1);
            foreach (var (from, via) in Referrers(c, o))
                if (from >= 0 && from != o && (best.from < 0 || depth[from] < depth[best.from]))
                    best = (from, via);
            if (best.from < 0)
                return null;
            var path = Describe.PathToRoot(m, best.from);
            path.Add($"{Describe.Via(m, best.via)} -> {Describe.Managed(m, o)}".TrimStart());
            return path;
        }

        /// <summary>The objects whose shortest path from a root runs through an object, itself included.</summary>
        static (int count, long bytes) Below(Crawl c, int o)
        {
            // Parents precede children, so one forward pass marks every descendant in the shortest-path tree.
            var inside = new bool[c.Count];
            inside[o] = true;
            var count = 1;
            var bytes = c.Size[o];
            for (var x = o + 1; x < c.Count; x++)
            {
                var p = c.Parent[x];
                if (p >= 0 && inside[p])
                {
                    inside[x] = true;
                    count++;
                    bytes += c.Size[x];
                }
            }
            return (count, bytes);
        }

        static List<(int from, int via)> Referrers(Crawl c, int o)
        {
            var result = new List<(int, int)>();
            for (var e = 0; e < c.EdgeCount; e++)
                if (c.EdgeTo[e] == o)
                    result.Add((c.EdgeFrom[e], c.EdgeVia[e]));
            return result;
        }
    }
}
