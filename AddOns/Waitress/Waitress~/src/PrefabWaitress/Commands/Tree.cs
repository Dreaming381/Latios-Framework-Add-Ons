using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;

namespace PrefabWaitress.Commands
{
    /// <summary>The hierarchy of a scene or prefab, with prefab instances marked and collapsed.</summary>
    public static class Tree
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            var asset = session.Asset(args.Positional0("scene or prefab"));
            var project = session.Project;
            var depth = args.GetInt("depth", int.MaxValue);
            var components = args.Flag("components");
            var deep = args.Flag("deep");
            var instances = args.Flag("instances");
            var start = args.Positional.Count > 1 ? Describe.FindObjects(asset, args.Positional[1]) : null;
            if (start != null && start.Count == 0)
                Describe.FindOne(asset, args.Positional[1]);
            args.RejectUnknown();

            w.WriteLine(Header(asset));
            if (instances)
            {
                foreach (var root in start ?? asset.Roots)
                    foreach (var go in Hierarchy.Subtree(root).Where(o => o.IsGameObject))
                        for (var o = go; o != null; o = o.Source)
                            if (o.Layer?.Root == o)
                            {
                                var where = o.Owner == asset ? Describe.Plural(o.Layer.Mods.Count, "override") : "nested in " + o.Owner.FileName;
                                w.WriteLine($"{Hierarchy.PathOf(go)} [{Chain(o.Source ?? o)}, {where}]");
                                break;
                            }
                return 0;
            }
            var printer = new Printer(project, asset, w, components, deep, depth);
            foreach (var root in start ?? asset.Roots)
                printer.Print(root, 0, start != null);
            if (printer.Collapsed > 0)
                w.WriteLine($"({Describe.Plural(printer.Collapsed, "prefab instance")} collapsed; `tree {Path.GetFileName(asset.Path)} <instance>` or --deep expands)");
            return 0;
        }

        public static string Header(Asset a)
        {
            var gos = a.GameObjects.Count();
            var mods = a.Layers.Sum(l => l.Mods.Count);
            var unused = a.Layers.Sum(l => l.Mods.Count(m => m.Status is ModStatus.NoTarget or ModStatus.NoProperty));
            var text = $"{a.Path}: {Describe.Plural(gos, "GameObject")}, {Describe.Plural(a.Layers.Count, "prefab instance")}, {Describe.Plural(mods, "override")}";
            if (unused > 0)
                text += $" ({unused} unused)";
            var variant = Variant(a);
            if (variant != null)
                text += $"; a variant of {variant}";
            if (a.HasOpaque)
                text += "; some objects come from models this tool can't read (see `help models`)";
            return text;
        }

        /// <summary>For a prefab variant, its base chain: "Door.prefab", or "DoorRed.prefab &lt; Door.prefab".</summary>
        public static string Variant(Asset a)
        {
            if (a.Kind != AssetKind.Prefab || a.Roots.Count != 1 || a.Roots[0].Source == null)
                return null;
            return Chain(a.Roots[0].Source);
        }

        /// <summary>"A.prefab &lt; B.prefab": the asset an instanced object comes from and the bases below it.</summary>
        public static string Chain(Obj source)
        {
            var parts = new List<string>();
            for (var o = source; o != null; o = o.Source)
            {
                parts.Add(o.Owner.FileName);
                if (o.Owner.Roots.Count != 1 || o.Owner.Roots[0] != o)
                    break;
            }
            return string.Join(" < ", parts);
        }

        sealed class Printer
        {
            readonly Project m_Project;
            readonly Asset m_Asset;
            readonly TextWriter m_W;
            readonly bool m_Components, m_Deep;
            readonly int m_Depth;
            public int Collapsed;

            public Printer(Project project, Asset asset, TextWriter w, bool components, bool deep, int depth)
            {
                m_Project = project;
                m_Asset = asset;
                m_W = w;
                m_Components = components;
                m_Deep = deep;
                m_Depth = depth;
            }

            public void Print(Obj go, int level, bool expand)
            {
                var line = new string(' ', level * 2) + Label(go);
                var isRoot = go.Layer != null && go.Layer.Root == go;
                var collapse = isRoot && !expand && !m_Deep && go.Children.Count > 0 && !(go.Parent == null && m_Asset.Kind == AssetKind.Prefab);
                if (m_Components)
                    line += "  {" + string.Join(", ", ComponentLabels(go)) + "}";
                if (collapse)
                {
                    var count = Hierarchy.Subtree(go).Count(o => o.IsGameObject) - 1;
                    line += $" +{count} below";
                    Collapsed++;
                }
                m_W.WriteLine(line);
                if (collapse || level + 1 > m_Depth)
                    return;
                foreach (var child in go.Children)
                    Print(child, level + 1, false);
            }

            string Label(Obj go)
            {
                var name = Hierarchy.Segment(go);
                if (go.Layer != null && go.Layer.Root == go)
                {
                    var layer = go.Layer;
                    var unused = layer.Mods.Count(m => m.Status is ModStatus.NoTarget or ModStatus.NoProperty);
                    var info = $"{Chain(go.Source)}, {Describe.Plural(layer.Mods.Count, "override")}";
                    if (unused > 0)
                        info += $", {unused} unused";
                    if (!go.Layer.Source.Readable)
                        info = $"{layer.Source.FileName}, can't be read: {layer.Source.Problem}";
                    return $"{name} [{info}]";
                }
                if (go.Source == null && go.Parent != null && go.Parent.Source != null && go.Parent.Owner == m_Asset)
                    return "+" + name;
                if (go.Body?.Scalar("m_IsActive") == "0")
                    return name + " (inactive)";
                return name;
            }

            IEnumerable<string> ComponentLabels(Obj go)
            {
                foreach (var (name, comp) in Describe.ComponentNames(m_Project, go))
                    yield return comp.Source == null && go.Source != null ? "+" + name : name;
                if (go.Layer != null && go.Source != null)
                    foreach (var r in go.Layer.RemovedComponents)
                    {
                        var removed = go.Layer.Source.Get(r.FileId);
                        if (removed?.GameObject == go.Source)
                            yield return "-" + Describe.TypeOf(m_Project, removed);
                    }
            }
        }
    }
}
