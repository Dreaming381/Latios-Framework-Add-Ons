using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>Where a value comes from: the property at every layer from the original prefab up to this file.</summary>
    public static class Why
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(3);
            var asset = session.Asset(args.Positional0("scene or prefab"));
            var project = session.Project;
            var go = Describe.FindOne(asset, args.PositionalAt(1, "object"));
            var (comp, path) = Describe.FindProperty(project, go, args.PositionalAt(2, "property, as Component.field (e.g. MeshRenderer.m_Materials[0])"));
            args.RejectUnknown();

            var chain = new List<Obj>();
            for (var o = comp; o != null; o = o.Source)
                chain.Add(o);
            chain.Reverse();

            var top = Value(comp, path);
            if (!comp.Opaque && chain.All(o => Value(o, path) == null) && !chain.Any(o => o.SetBy.Keys.Any(k => Fields.Covers(path, k) || Fields.Covers(k, path))))
                throw new WaitressException($"{Describe.TypeOf(project, comp)} has no {Describe.Short(path)} in any layer" + Describe.Suggest(comp, path));
            w.WriteLine($"{asset.Path} {Hierarchy.PathOf(go)} {Describe.TypeOf(project, comp)}.{Describe.Short(path)} = {(top == null ? "(not in the file)" : Describe.Value(project, asset, top))}");
            var rows = new List<string[]>();
            foreach (var o in chain)
            {
                var v = Value(o, path);
                var setters = o.SetBy.Where(e => Fields.Covers(path, e.Key) || Fields.Covers(e.Key, path)).Select(e => e.Value).Where(m => m.Layer.Owner == o.Owner).ToList();
                string how;
                if (o.Source == null)
                    how = o.Opaque ? "from a source this tool can't read" : "written there";
                else if (setters.Count > 0)
                    how = "override" + (setters.Any(m => m.Status == ModStatus.Redundant) && setters.All(m => m.Status is ModStatus.Redundant or ModStatus.Applied) && SameAsSource(o, path) ? " (same as below, redundant)" : "");
                else
                    how = "inherited";
                var where = o.GameObject != null ? Hierarchy.PathOf(o.GameObject) : "";
                rows.Add(new[] { o.Owner.FileName, where, v == null ? "(not in the file)" : Describe.Value(project, o.Owner, v, 80), how });
            }
            var w0 = rows.Max(r => r[0].Length);
            var w1 = rows.Max(r => r[1].Length);
            var w2 = rows.Max(r => r[2].Length);
            foreach (var r in rows)
                w.WriteLine($"  {r[0].PadRight(w0)}  {r[1].PadRight(w1)}  {r[2].PadRight(w2)}  {r[3]}");

            var unused = comp.Layer?.Mods.Where(m => m.TargetObj == comp && m.Status is ModStatus.NoProperty && (Fields.Covers(path, m.Path) || Fields.Covers(m.Path, path))).ToList();
            foreach (var m in unused ?? new List<Mod>())
                w.WriteLine($"  unused override in {m.Layer.Owner.FileName}: {Describe.Short(m.Path)} = {m.Value} ({m.Detail})");
            if (top == null && comp.Source != null && !comp.Opaque)
                w.WriteLine("  (no layer writes this property; the file may predate the field, so Unity uses the script's default)");
            return 0;
        }

        static YNode Value(Obj o, string path) =>
            o.Body != null && PropPath.TryResolve(o.Body, path, out var slot, out _) ? slot.Get() : null;

        static bool SameAsSource(Obj o, string path)
        {
            var a = Value(o, path);
            var b = Value(o.Source, path);
            return a != null && b != null && PropPath.SameValue(a, b);
        }
    }
}
