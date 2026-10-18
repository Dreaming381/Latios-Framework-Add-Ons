using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>What each prefab instance in a file changes: overridden values, added and removed components and objects, and unused overrides.</summary>
    public static class Overrides
    {
        static readonly HashSet<string> s_RootTransformDefaults = new HashSet<string>
        {
            "m_LocalPosition", "m_LocalRotation", "m_LocalEulerAnglesHint", "m_RootOrder",
            "m_AnchorMin", "m_AnchorMax", "m_AnchoredPosition", "m_SizeDelta", "m_Pivot",
        };

        /// <summary>Overrides Unity writes on every instance and its Overrides window hides: the root's name and placement.</summary>
        public static bool IsDefault(Mod m)
        {
            var o = m.TargetObj;
            if (o == null || o.Layer?.Root == null)
                return false;
            var field = m.Path.Split('.')[0];
            if (o == o.Layer.Root)
                return field == "m_Name";
            return o.IsTransform && o.GameObject == o.Layer.Root && s_RootTransformDefaults.Contains(field);
        }

        public static bool IsUnused(Mod m) => m.Status is ModStatus.NoTarget or ModStatus.NoProperty;

        public static int Run(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            var target = args.Positional0("scene, prefab or folder");
            var unusedOnly = args.Flag("unused");
            var redundantOnly = args.Flag("redundant");
            var full = Path.Combine(session.Root, target);
            if (Directory.Exists(target) || Directory.Exists(full))
            {
                args.RejectUnknown();
                return Folder(session, Directory.Exists(target) ? target : full, unusedOnly, redundantOnly, w);
            }
            var asset = session.Asset(target);
            var project = session.Project;
            List<Layer> layers = asset.Layers;
            HashSet<Obj> below = null;
            if (args.Positional.Count > 1)
            {
                var go = Describe.FindOne(asset, args.Positional[1]);
                below = new HashSet<Obj>(Hierarchy.Subtree(go));
                // The instance holding the object, and any instance below it.
                layers = layers.Where(l => l.Root != null && (below.Contains(l.Root) || Hierarchy.Subtree(l.Root).Contains(go)) || l.Mods.Any(m => m.TargetObj != null && below.Contains(m.TargetObj))).ToList();
                if (layers.Any(l => l.Root == go))
                    below = null;
            }
            args.RejectUnknown();
            w.WriteLine(Summary(asset, asset.Layers));
            foreach (var layer in layers)
                PrintLayer(project, asset, layer, unusedOnly, redundantOnly, below, w);
            return 0;
        }

        public static string Summary(Asset a, List<Layer> layers)
        {
            var mods = layers.Sum(l => l.Mods.Count);
            var unused = layers.Sum(l => l.Mods.Count(IsUnused));
            var redundant = layers.Sum(l => l.Mods.Count(m => m.Status == ModStatus.Redundant && !IsDefault(m)));
            var unchecked_ = layers.Sum(l => l.Mods.Count(m => m.Status == ModStatus.Unchecked));
            var text = $"{a.Path}: {Describe.Plural(layers.Count, "prefab instance")}, {Describe.Plural(mods, "override")}";
            var notes = new List<string>();
            if (unused > 0)
                notes.Add($"{unused} unused");
            if (redundant > 0)
                notes.Add($"{redundant} redundant");
            if (unchecked_ > 0)
                notes.Add($"{unchecked_} unchecked");
            return notes.Count > 0 ? text + " (" + string.Join(", ", notes) + ")" : text;
        }

        static void PrintLayer(Project project, Asset asset, Layer layer, bool unusedOnly, bool redundantOnly, HashSet<Obj> below, TextWriter w)
        {
            var root = layer.Root;
            var title = root != null ? Hierarchy.PathOf(root) : layer.Mods.FirstOrDefault(m => m.Path == "m_Name")?.Value ?? $"&{layer.Id}";
            var source = layer.Source.Readable ? Tree.Chain(root?.Source ?? layer.Source.Roots.FirstOrDefault()) : $"{layer.Source.FileName}, can't be read: {layer.Source.Problem}";
            var lines = new List<string>();
            bool Within(Obj o) => below == null || o != null && (below.Contains(o) || o.GameObject != null && below.Contains(o.GameObject));

            if (!unusedOnly)
            {
                var defaults = new List<string>();
                var byTarget = layer.Mods.Where(m => m.TargetObj != null && !IsUnused(m) && Within(m.TargetObj)).GroupBy(m => m.TargetObj);
                foreach (var group in byTarget)
                {
                    var obj = group.Key;
                    var mine = new HashSet<Mod>(group);
                    var label = Label(project, layer, obj);
                    foreach (var line in Fields.Lines(obj))
                    {
                        var setters = line.Setters.Where(mine.Contains).ToList();
                        if (setters.Count == 0)
                            continue;
                        var redundant = setters.Where(m => m.Status == ModStatus.Redundant).ToList();
                        var isDefault = setters.All(IsDefault);
                        if (redundantOnly && (redundant.Count == 0 || isDefault))
                            continue;
                        var text = $"{Describe.Short(line.Path)} = {Describe.Value(project, asset, line.Node)}";
                        var before = Fields.SourceValue(obj, line.Path);
                        var differs = before != null && !PropPath.SameValue(before, line.Node);
                        if (isDefault)
                        {
                            defaults.Add(differs && !line.Path.StartsWith("m_LocalPosition", StringComparison.Ordinal) && !line.Path.StartsWith("m_AnchoredPosition", StringComparison.Ordinal)
                                ? $"{text} (prefab: {Describe.Value(project, layer.Source, before, 60)})" : text);
                            continue;
                        }
                        if (redundant.Count == setters.Count)
                            text += " (redundant: same as the prefab)";
                        else
                        {
                            if (differs)
                                text += $" (prefab: {Describe.Value(project, layer.Source, before, 60)})";
                            if (redundant.Count > 0)
                                text += " (redundant part: " + string.Join(", ", redundant.Select(m => Describe.Short(m.Path))) + ")";
                        }
                        if (setters.Any(m => m.Status == ModStatus.Unchecked))
                            text += " (unchecked)";
                        lines.Add($"  {label}{text}");
                    }
                }
                if (defaults.Count > 0 && !redundantOnly)
                    lines.Insert(0, "  root: " + string.Join("; ", defaults));

                if (!redundantOnly)
                {
                    foreach (var o in asset.Objects.Values.Where(o => o.Source == null && !o.IsGameObject && o.GameObject != null && o.GameObject.Source != null && o.GameObject.Layer == layer && Within(o)))
                        lines.Add($"  +{Describe.TypeOf(project, o)} on {RelOrRoot(layer, o.GameObject)} (added component)");
                    foreach (var go in asset.GameObjects.Where(g => g.Source == null && g.Parent != null && g.Parent.Layer == layer && g.Parent.Source != null && Within(g)))
                        lines.Add($"  +{go.Name} under {RelOrRoot(layer, go.Parent)} (added object)");
                    foreach (var r in layer.RemovedComponents)
                    {
                        var c = layer.Source.Get(r.FileId);
                        if (below != null && (c?.GameObject == null || !Within(asset.Get(layer.Map(c.GameObject.Id)))))
                            continue;
                        lines.Add(c == null ? $"  -&{r.FileId} (removed component, no longer in {layer.Source.FileName})" : $"  -{Describe.TypeOf(project, c)} on {SourceRel(layer, c)} (removed component)");
                    }
                    foreach (var r in layer.RemovedGameObjects)
                    {
                        var g = layer.Source.Get(r.FileId);
                        if (below != null)
                            continue;
                        lines.Add(g == null ? $"  -&{r.FileId} (removed object, no longer in {layer.Source.FileName})" : $"  -{SourceRel(layer, g)} (removed object)");
                    }
                }
            }
            if (!redundantOnly)
                foreach (var m in layer.Mods.Where(m => IsUnused(m) && (below == null || Within(m.TargetObj))))
                {
                    var label = m.TargetObj != null ? Label(project, layer, m.TargetObj) : $"&{m.Target.FileId} ";
                    var value = m.SetsReference ? Describe.Ref(project, layer.Source, m.ObjectReference.AsRef()) : m.Value;
                    lines.Add($"  unused: {label}{Describe.Short(m.Path)} = {value} ({m.Detail})");
                }

            if (lines.Count == 0 && (unusedOnly || redundantOnly || below != null))
                return;
            var counts = $"{Describe.Plural(layer.Mods.Count, "override")}";
            w.WriteLine($"{title} [{source}] {counts}");
            foreach (var l in lines)
                w.WriteLine(l);
        }

        static string RelOrRoot(Layer layer, Obj go) => Rel(layer, go) is var rel && rel.Length > 0 ? rel : go.Name;

        /// <summary>A source object's place in the instance: its path below the source root, or the instance root's name.</summary>
        static string SourceRel(Layer layer, Obj o)
        {
            var go = o.GameObject ?? o;
            var path = Hierarchy.PathOf(go);
            var slash = path.IndexOf('/');
            return slash < 0 ? layer.Root?.Name ?? path : path.Substring(slash + 1);
        }


        /// <summary>"Child/Leaf MeshRenderer." for an override's target, relative to the instance root.</summary>
        public static string Label(Project project, Layer layer, Obj obj)
        {
            if (obj.Opaque && (obj.GameObject == null || obj.GameObject.Name.Length == 0))
            {
                var source = obj.Body?["m_CorrespondingSourceObject"].AsRef().FileId ?? 0;
                return $"{layer.Source.FileName}#{source}{(obj.TypeName != "?" ? " " + obj.TypeName : "")}: ";
            }
            var go = obj.GameObject ?? obj;
            var rel = Rel(layer, go);
            var type = obj.IsGameObject ? "" : Describe.TypeOf(project, obj) + ".";
            if (obj.GameObject == null && !obj.IsGameObject)
                return $"&{obj.Id} ";
            return (rel.Length > 0 ? rel + " " : "") + type;
        }

        static string Rel(Layer layer, Obj go)
        {
            var path = Hierarchy.PathOf(go);
            var root = layer.Root != null ? Hierarchy.PathOf(layer.Root) : null;
            if (root != null && path == root)
                return "";
            if (root != null && path.StartsWith(root + "/", StringComparison.Ordinal))
                return path.Substring(root.Length + 1);
            return path;
        }

        static int Folder(Session session, string folder, bool unusedOnly, bool redundantOnly, TextWriter w)
        {
            var project = session.Project;
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
            int withInstances = 0, mods = 0, unused = 0, redundant = 0;
            foreach (var f in files)
            {
                var a = project.Load(f);
                if (a.Layers.Count == 0)
                    continue;
                withInstances++;
                var u = a.Layers.Sum(l => l.Mods.Count(IsUnused));
                var r = a.Layers.Sum(l => l.Mods.Count(m => m.Status == ModStatus.Redundant && !IsDefault(m)));
                mods += a.Layers.Sum(l => l.Mods.Count);
                unused += u;
                redundant += r;
                if (unusedOnly && u == 0 || redundantOnly && r == 0)
                    continue;
                w.WriteLine(Summary(a, a.Layers));
                if (unusedOnly || redundantOnly)
                    foreach (var layer in a.Layers)
                        PrintLayer(project, a, layer, unusedOnly, redundantOnly, null, w);
            }
            w.WriteLine($"total: {Describe.Plural(withInstances, "file")} with prefab instances, {Describe.Plural(mods, "override")}, {unused} unused, {redundant} redundant (of {files.Count} scenes and prefabs)");
            return 0;
        }
    }
}
