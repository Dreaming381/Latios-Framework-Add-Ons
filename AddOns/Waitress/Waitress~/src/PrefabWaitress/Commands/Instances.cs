using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>
    /// Everywhere a prefab (or one object in it) is used, directly or through variants and nesting; with a property, which
    /// of those places keep their own value, so a change to the prefab won't reach them.
    /// </summary>
    public static class Instances
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(3);
            var prefab = session.Asset(args.Positional0("prefab"));
            var project = session.Project;
            if (prefab.Kind != AssetKind.Prefab)
                throw new WaitressException($"{prefab.Path} is not a prefab");
            var objectSpec = args.Positional.Count > 1 ? args.Positional[1] : null;
            var propSpec = args.Get("prop") ?? (args.Positional.Count > 2 ? args.Positional[2] : null);
            var folders = args.GetAll("in").Select(f => Path.Combine(session.Root, f)).ToList();
            var top = args.GetInt("top", 40);
            args.RejectUnknown();
            if (folders.Count == 0)
                folders = new List<string> { Path.Combine(session.Root, "Assets"), Path.Combine(session.Root, "Packages") };

            if (objectSpec == null && prefab.Roots.Count != 1)
                throw new WaitressException(prefab.HasOpaque
                    ? $"{prefab.Path}'s root comes from a model that hasn't been exported; run `prefabwaitress models {prefab.Path}` (needs the Editor), or name an object in it"
                    : $"{prefab.Path} has {prefab.Roots.Count} root objects; name one");
            var target = objectSpec != null ? Describe.FindOne(prefab, objectSpec) : prefab.Roots[0];
            Obj comp = null;
            string path = null;
            if (propSpec != null)
                (comp, path) = Describe.FindProperty(project, target, propSpec);
            if (comp != null && comp.Body != null && !PropPath.TryResolve(comp.Body, path, out _, out _))
                throw new WaitressException($"{prefab.FileName} {Hierarchy.PathOf(target)} {Describe.TypeOf(project, comp)} has no {Describe.Short(path)}" + Describe.Suggest(comp, path));
            var probe = comp ?? target;

            var usage = new Usage(project, folders);
            var files = usage.Dependents(project, prefab.Guid);
            var places = new List<(Asset file, Obj obj)>();
            foreach (var f in files)
            {
                var a = project.Load(f);
                foreach (var o in a.Objects.Values)
                    if (o.Source != null && Reaches(o, probe))
                        places.Add((a, o));
            }

            var what = Hierarchy.PathOf(target) + (comp != null ? $" {Describe.TypeOf(project, comp)}.{Describe.Short(path)}" : "");
            if (path == null)
            {
                w.WriteLine($"{prefab.FileName} {what}: {Describe.Plural(places.Count, "instance")} in {Describe.Plural(places.Select(p => p.file).Distinct().Count(), "file")} ({usage.Scanned} scenes and prefabs searched)");
                foreach (var g in places.GroupBy(p => p.file))
                    w.WriteLine($"  {g.Key.Path}: " + Join(g.Select(p => Place(p.obj)), top));
                return 0;
            }

            var mine = Value(comp, path);
            w.WriteLine($"{prefab.FileName} {what} = {(mine == null ? "(not in the file)" : Describe.Value(project, prefab, mine))}");
            var follow = new List<(Asset, Obj)>();
            var own = new Dictionary<Mod, List<(Asset file, Obj obj)>>();
            foreach (var (a, o) in places)
            {
                var setter = Setter(o, path, prefab);
                if (setter == null)
                    follow.Add((a, o));
                else
                {
                    if (!own.TryGetValue(setter, out var list))
                        own[setter] = list = new List<(Asset, Obj)>();
                    list.Add((a, o));
                }
            }
            var keep = own.Values.Sum(l => l.Count);
            w.WriteLine($"{Describe.Plural(places.Count, "instance")} in {Describe.Plural(places.Select(p => p.file).Distinct().Count(), "file")}: {follow.Count} follow the prefab, {keep} keep their own value");
            foreach (var (mod, list) in own.OrderByDescending(e => e.Value.Count))
            {
                var at = list.FirstOrDefault(l => l.file == mod.Layer.Owner);
                var holder = at.obj ?? list[0].obj;
                var value = Value(holder, path);
                var others = list.Where(l => l.file != mod.Layer.Owner).ToList();
                var text = $"  {mod.Layer.Owner.Path} {Place(holder)} = {(value == null ? "?" : Describe.Value(project, mod.Layer.Owner, value, 80))}";
                if (mod.Status == ModStatus.Redundant)
                    text += " (redundant: same value)";
                if (others.Count > 0)
                    text += $"; inherited by {Join(others.Select(l => $"{l.file.FileName} {Place(l.obj)}"), 6)}";
                w.WriteLine(text);
            }
            if (follow.Count > 0)
                w.WriteLine("  follow: " + Join(follow.Select(p => $"{p.Item1.FileName} {Place(p.Item2)}"), top));
            return 0;
        }

        static string Join(IEnumerable<string> items, int top)
        {
            var list = items.ToList();
            if (top > 0 && list.Count > top)
                return string.Join(", ", list.Take(top)) + $", ... {list.Count - top} more";
            return string.Join(", ", list);
        }

        static string Place(Obj o) => o.GameObject != null ? Hierarchy.PathOf(o.GameObject) : $"&{o.Id}";

        /// <summary>True when an object is an instance of the target, through any number of layers.</summary>
        static bool Reaches(Obj o, Obj target)
        {
            for (var s = o.Source; s != null; s = s.Source)
                if (s == target)
                    return true;
            return false;
        }

        /// <summary>The override above the prefab that decides the value, or null when the value is the prefab's.</summary>
        static Mod Setter(Obj o, string path, Asset prefab)
        {
            var above = new List<Asset>();
            for (var s = o; s != null && s.Owner != prefab; s = s.Source)
                above.Add(s.Owner);
            Mod nearest = null;
            var rank = int.MaxValue;
            foreach (var (p, m) in o.SetBy)
            {
                if (!Fields.Covers(path, p) && !Fields.Covers(p, path))
                    continue;
                var i = above.IndexOf(m.Layer.Owner);
                if (i >= 0 && i < rank)
                {
                    rank = i;
                    nearest = m;
                }
            }
            return nearest;
        }

        static YNode Value(Obj o, string path) =>
            o.Body != null && PropPath.TryResolve(o.Body, path, out var slot, out _) ? slot.Get() : null;
    }
}
