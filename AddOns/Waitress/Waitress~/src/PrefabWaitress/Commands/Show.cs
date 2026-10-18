using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;

namespace PrefabWaitress.Commands
{
    /// <summary>An object's components and values, marking every value an override set and which file set it.</summary>
    public static class Show
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(3);
            var asset = session.Asset(args.Positional0("scene or prefab"));
            var project = session.Project;
            var go = Describe.FindOne(asset, args.PositionalAt(1, "object (a hierarchy path such as Root/Child, or a name)"));
            var only = args.Positional.Count > 2 ? Describe.FindComponent(project, go, args.Positional[2]) : null;
            var all = args.Flag("all");
            var filter = args.Get("filter");
            args.RejectUnknown();

            w.WriteLine($"{asset.Path} {Hierarchy.PathOf(go)}{Origin(go)}");
            var hidden = 0;
            var quiet = new List<string>();
            var components = new List<(string name, Obj comp)> { ("GameObject", go) };
            components.AddRange(Describe.ComponentNames(project, go));
            foreach (var (name, comp) in components)
            {
                if (only != null && comp != only)
                    continue;
                var lines = Fields.Lines(comp);
                var instanced = comp.Source != null || comp.Opaque;
                var shown = lines.Where(l => (all || !instanced || l.Setters.Count > 0 || filter != null) && Matches(l.Path, filter)).ToList();
                hidden += lines.Count(l => Matches(l.Path, filter)) - shown.Count;
                var tag = comp.Source == null && go.Source != null && comp != go ? " (added here)" : "";
                if (shown.Count == 0)
                {
                    if (filter == null)
                        quiet.Add(name + tag);
                    continue;
                }
                w.WriteLine(name + tag);
                foreach (var line in shown)
                    w.WriteLine("  " + Line(project, asset, comp, line));
            }
            if (go.Layer != null && go.Source != null)
                foreach (var r in go.Layer.RemovedComponents)
                {
                    var removed = go.Layer.Source.Get(r.FileId);
                    if (removed?.GameObject == go.Source)
                        w.WriteLine($"-{Describe.TypeOf(project, removed)} (removed here)");
                }
            if (quiet.Count > 0)
                w.WriteLine((go.Source != null ? "unchanged: " : "no values: ") + string.Join(", ", quiet));
            if (hidden > 0)
                w.WriteLine($"({hidden} more values come unchanged from the prefab; --all shows them)");
            return 0;
        }

        static bool Matches(string path, string filter) =>
            filter == null || Describe.Short(path).Contains(filter, System.StringComparison.OrdinalIgnoreCase) || (filter.Contains('*') && Glob.Compile(filter)(Describe.Short(path)));

        public static string Line(Project project, Asset asset, Obj comp, FieldLine line)
        {
            var text = $"{Describe.Short(line.Path)} = {Describe.Value(project, asset, line.Node)}";
            if (line.Setters.Count == 0)
                return text;
            text += "   <- " + Fields.SetterNames(line);
            var mine = line.Setters.Any(m => m.Layer.Owner == asset);
            var before = mine ? Fields.SourceValue(comp, line.Path) : null;
            if (before != null && !PropPath.SameValue(before, line.Node))
                text += $" (prefab: {Describe.Value(project, comp.Source.Owner, before, 60)})";
            if (line.Setters.Any(m => m.Status == ModStatus.Redundant) && line.Setters.All(m => m.Status is ModStatus.Redundant or ModStatus.Applied) && before != null && PropPath.SameValue(before, line.Node))
                text += " (same as the prefab)";
            if (line.Setters.Any(m => m.Status == ModStatus.Unchecked))
                text += " (unchecked)";
            return text;
        }

        /// <summary>" [instance of Door.prefab Door/Panel &lt; DoorBase.prefab Base/Panel]" for an object a prefab brings in.</summary>
        public static string Origin(Obj go)
        {
            if (go.Source == null)
            {
                if (go.Opaque && go.Layer != null)
                    return $"  [from {go.Layer.Source.FileName}, which can't be read]";
                return go.Parent?.Source != null ? "  [added in this file]" : "";
            }
            var parts = new List<string>();
            for (var o = go.Source; o != null; o = o.Source)
                parts.Add($"{o.Owner.FileName} {(o.GameObject != null ? Hierarchy.PathOf(o.GameObject) : o.TypeName)}");
            return "  [from " + string.Join(" < ", parts) + "]";
        }
    }
}
