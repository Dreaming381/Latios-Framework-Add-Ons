using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Model
{
    /// <summary>
    /// Loads scenes and prefabs and resolves every prefab instance in them: each object a source brings in gets its
    /// effective values (the source's, with the instance's overrides applied) and a record of which override set what.
    /// </summary>
    public sealed class Project
    {
        public readonly AssetIndex Index;
        readonly Dictionary<string, Asset> m_Assets = new Dictionary<string, Asset>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> m_Loading = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public ModelCache Models;
        public readonly ScriptNames Scripts;

        public Project(string root)
        {
            Index = new AssetIndex(root);
            Models = new ModelCache(this);
            Scripts = new ScriptNames(Index);
        }

        public string Root => Index.Root;

        /// <summary>Loads an asset by asset path ("Assets/...") or file path.</summary>
        public Asset Load(string path)
        {
            var assetPath = path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) || path.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase)
                ? path.Replace('\\', '/')
                : Index.ToAssetPath(path);
            if (m_Assets.TryGetValue(assetPath, out var cached))
                return cached;
            var asset = new Asset { Path = assetPath, Guid = Index.GuidOf(assetPath) };
            if (!m_Loading.Add(assetPath))
            {
                asset.Kind = AssetKind.Missing;
                asset.Problem = "prefab cycle: it instances itself through its sources";
                return asset;
            }
            try
            {
                Read(asset);
            }
            finally
            {
                m_Loading.Remove(assetPath);
            }
            m_Assets[assetPath] = asset;
            return asset;
        }

        public Asset LoadGuid(string guid)
        {
            var path = Index.PathOf(guid);
            if (path == null)
                return new Asset { Path = "missing asset " + guid, Guid = guid, Kind = AssetKind.Missing, Problem = "no asset in the project has this GUID" };
            return Load(path);
        }

        void Read(Asset asset)
        {
            var full = Index.FullPath(asset.Path);
            var ext = Path.GetExtension(asset.Path).ToLowerInvariant();
            asset.Kind = ext == ".unity" ? AssetKind.Scene : ext == ".prefab" ? AssetKind.Prefab : Asset.IsModelPath(asset.Path) ? AssetKind.Model : AssetKind.Other;
            if (asset.Kind == AssetKind.Model)
            {
                var cache = Models.Find(asset);
                if (cache == null)
                {
                    asset.Problem = "a model not exported yet; run `prefabwaitress models`";
                    return;
                }
                asset.HasModelCache = true;
                asset.Docs = YamlReader.Parse(cache, asset.Path);
            }
            else
            {
                if (!File.Exists(full))
                {
                    asset.Kind = AssetKind.Missing;
                    asset.Problem = "file not found";
                    return;
                }
                if (asset.Kind == AssetKind.Other)
                {
                    asset.Problem = "not a scene, prefab or model";
                    return;
                }
                var text = File.ReadAllText(full);
                if (!text.StartsWith("%YAML", StringComparison.Ordinal))
                {
                    asset.Problem = "not saved as text (Asset Serialization Mode is binary)";
                    return;
                }
                asset.Docs = YamlReader.Parse(text, asset.Path);
            }
            Resolve(asset);
        }

        void Resolve(Asset asset)
        {
            var stripped = new List<YamlDoc>();
            foreach (var doc in asset.Docs)
            {
                if (doc.ClassId == 1001)
                {
                    // Files from before Unity 2018.3 write a "Prefab" object instead, and copy instanced objects out in full.
                    if (doc.TypeName == "PrefabInstance")
                        asset.Layers.Add(ReadLayer(asset, doc));
                    continue;
                }
                if (doc.Stripped)
                {
                    stripped.Add(doc);
                    continue;
                }
                asset.Objects[doc.FileId] = new Obj { Owner = asset, Id = doc.FileId, ClassId = doc.ClassId, TypeName = doc.TypeName, Body = doc.Body, Line = doc.Line };
            }

            var layerById = asset.Layers.ToDictionary(l => l.Id);
            foreach (var doc in stripped)
                if (layerById.TryGetValue(doc.Body["m_PrefabInstance"].AsRef().FileId, out var layer))
                    layer.IdMap[doc.Body["m_CorrespondingSourceObject"].AsRef().FileId] = doc.FileId;

            var lines = new Dictionary<long, int>();
            foreach (var d in stripped)
                lines[d.FileId] = d.Line;
            foreach (var layer in asset.Layers)
                Instantiate(asset, layer, lines);

            foreach (var doc in stripped)
            {
                if (asset.Objects.ContainsKey(doc.FileId))
                    continue;
                // An object the source no longer has, or one from a source that can't be read.
                layerById.TryGetValue(doc.Body["m_PrefabInstance"].AsRef().FileId, out var layer);
                var obj = new Obj { Owner = asset, Id = doc.FileId, ClassId = doc.ClassId, TypeName = doc.TypeName, Body = new YMap(), Layer = layer, Opaque = true, Line = doc.Line };
                obj.Body["m_CorrespondingSourceObject"] = doc.Body["m_CorrespondingSourceObject"];
                asset.Objects[doc.FileId] = obj;
            }

            foreach (var layer in asset.Layers)
                ApplyMods(asset, layer);

            Hierarchy.Build(asset);

            // The Editor names a prefab's root after its file, whatever the file says.
            if (asset.Kind == AssetKind.Prefab && asset.Roots.Count == 1 && asset.Roots[0].Transform?.Father == null)
                asset.Roots[0].Body["m_Name"] = new YScalar(Path.GetFileNameWithoutExtension(asset.Path));
        }

        static Layer ReadLayer(Asset asset, YamlDoc doc)
        {
            var layer = new Layer { Owner = asset, Id = doc.FileId, Line = doc.Line };
            var body = doc.Body;
            layer.SourceGuid = (body["m_SourcePrefab"] as YMap).AsRef().Guid;
            var mod = body.Map("m_Modification") ?? new YMap();
            layer.TransformParent = mod["m_TransformParent"].AsRef().FileId;
            var index = 0;
            foreach (var item in mod.Seq("m_Modifications")?.Items ?? new List<YNode>())
            {
                if (item is not YMap m)
                    continue;
                layer.Mods.Add(new Mod
                {
                    Layer = layer,
                    Index = index++,
                    Target = m["target"].AsRef(),
                    Path = m.Scalar("propertyPath") ?? "",
                    Value = m.Scalar("value") ?? "",
                    ObjectReference = m["objectReference"],
                });
            }
            foreach (var item in mod.Seq("m_RemovedComponents")?.Items ?? new List<YNode>())
                layer.RemovedComponents.Add(item.AsRef());
            foreach (var item in mod.Seq("m_RemovedGameObjects")?.Items ?? new List<YNode>())
                layer.RemovedGameObjects.Add(item.AsRef());
            foreach (var item in mod.Seq("m_AddedGameObjects")?.Items ?? new List<YNode>())
                if (item is YMap m)
                    layer.AddedGameObjects.Add((m["targetCorrespondingSourceObject"].AsRef(), ParseInt(m.Scalar("insertIndex")), m["addedObject"].AsRef().FileId));
            foreach (var item in mod.Seq("m_AddedComponents")?.Items ?? new List<YNode>())
                if (item is YMap m)
                    layer.AddedComponents.Add((m["targetCorrespondingSourceObject"].AsRef(), ParseInt(m.Scalar("insertIndex")), m["addedObject"].AsRef().FileId));
            return layer;
        }

        static int ParseInt(string s) => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : -1;

        void Instantiate(Asset asset, Layer layer, Dictionary<long, int> lines)
        {
            layer.Source = LoadGuid(layer.SourceGuid);
            var source = layer.Source;
            if (!source.Readable || source.HasOpaque)
                asset.HasOpaque = true;
            if (!source.Readable)
                return;

            var removed = new HashSet<long>();
            foreach (var r in layer.RemovedComponents)
                removed.Add(r.FileId);
            foreach (var r in layer.RemovedGameObjects)
            {
                var go = source.Get(r.FileId);
                if (go != null)
                    foreach (var o in Hierarchy.Subtree(go))
                        removed.Add(o.Id);
            }

            foreach (var s in source.Objects.Values)
            {
                if (removed.Contains(s.Id))
                    continue;
                var id = layer.Map(s.Id);
                asset.Objects[id] = new Obj
                {
                    Owner = asset,
                    Id = id,
                    ClassId = s.ClassId,
                    TypeName = s.TypeName,
                    Source = s,
                    Layer = layer,
                    SetBy = s.SetBy,
                    Opaque = s.Opaque,
                    Line = lines.TryGetValue(id, out var line) ? line : layer.Line,
                };
            }
            foreach (var root in source.Roots)
                layer.Root ??= asset.Get(layer.Map(root.Id));
        }

        /// <summary>Gives an instanced object its own copy of its source's values, with references between source objects pointed at their copies.</summary>
        internal static void Materialize(Obj obj)
        {
            var layer = obj.Layer;
            var s = obj.Source;
            var body = (YMap)s.Body.Clone();
            Remap(body, layer);
            body["m_CorrespondingSourceObject"] = YNodeExtensions.MakeRef(new Ref(s.Id, layer.Source.Guid));
            body["m_PrefabInstance"] = YNodeExtensions.MakeRef(new Ref(layer.Id, null));
            body["m_PrefabAsset"] = YNodeExtensions.MakeRef(default);
            if (body.IndexOf("m_ObjectHideFlags") >= 0)
                body["m_ObjectHideFlags"] = new YScalar("0");
            obj.Body = body;
            obj.SetBy = new Dictionary<string, Mod>(s.SetBy, StringComparer.Ordinal);
            if (obj.Owner.HierarchyBuilt)
                Hierarchy.Fix(obj);
        }

        static void Remap(YNode node, Layer layer)
        {
            switch (node)
            {
                case YMap m when m.IsRef():
                    var r = m.AsRef();
                    if (r.IsLocal && !r.IsNull)
                        m["fileID"] = new YScalar(layer.Map(r.FileId).ToString(CultureInfo.InvariantCulture));
                    break;
                case YMap m:
                    foreach (var v in m.Values)
                        Remap(v, layer);
                    break;
                case YSeq s:
                    foreach (var v in s.Items)
                        Remap(v, layer);
                    break;
            }
        }

        static void ApplyMods(Asset asset, Layer layer)
        {
            var source = layer.Source;
            foreach (var mod in layer.Mods)
            {
                var obj = asset.Get(layer.Map(mod.Target.FileId));
                mod.TargetObj = obj;
                if (!source.Readable)
                {
                    mod.Status = ModStatus.Unchecked;
                    mod.Detail = source.Problem;
                    mod.TargetObj = obj ??= Placeholder(asset, layer, mod);
                    ApplyOpaque(obj, mod);
                    continue;
                }
                if ((obj == null || obj.Source == null) && source.HasOpaque)
                {
                    mod.Status = ModStatus.Unchecked;
                    mod.Detail = $"{source.FileName} has objects from a model or missing asset this tool can't read";
                    mod.TargetObj = obj ??= Placeholder(asset, layer, mod);
                    ApplyOpaque(obj, mod);
                    continue;
                }
                if (obj == null || obj.Layer != layer || obj.Source == null)
                {
                    mod.Status = ModStatus.NoTarget;
                    mod.Detail = layer.RemovedComponents.Any(r => r.FileId == mod.Target.FileId) || layer.RemovedGameObjects.Any(r => r.FileId == mod.Target.FileId)
                        ? "the target was removed from this instance"
                        : $"no object &{mod.Target.FileId} in {source.FileName}";
                    mod.TargetObj = null;
                    continue;
                }
                if (obj.Opaque)
                {
                    mod.Status = ModStatus.Unchecked;
                    mod.Detail = "the target's source can't be read";
                    ApplyOpaque(obj, mod);
                    continue;
                }
                var body = obj.Body;
                if (!PropPath.TryResolve(body, mod.Path, out var slot, out var missing))
                {
                    // Only a property the serialized data clearly lacks is unused; data this tool doesn't follow is unchecked.
                    string why = null;
                    if (missing == "packed data")
                        why = "inside a list Unity stores as packed bytes";
                    else if (mod.Path.StartsWith("managedReferences[", StringComparison.Ordinal))
                        why = "inside [SerializeReference] data, which this tool doesn't follow";
                    else if (InAddedElement(obj, mod.Path))
                        why = "inside a list element an override added";
                    mod.Status = why != null ? ModStatus.Unchecked : ModStatus.NoProperty;
                    mod.Detail = why ?? $"{obj.TypeName} has no {PropPath.ToShort(missing)}";
                    if (why != null)
                        obj.SetBy[mod.Path] = mod;
                    continue;
                }
                if (slot.IsSize)
                {
                    var count = ParseInt(mod.Value);
                    mod.Before = slot.Get();
                    var items = slot.Seq.Items;
                    if (count >= 0)
                    {
                        while (items.Count > count)
                            items.RemoveAt(items.Count - 1);
                        while (items.Count < count)
                            items.Add(items.Count > 0 ? Zeroed(items[items.Count - 1]) : new YScalar("0"));
                    }
                    mod.Status = PropPath.SameScalar(((YScalar)mod.Before).Value, mod.Value) ? ModStatus.Redundant : ModStatus.Applied;
                }
                else
                {
                    var previous = slot.Get();
                    mod.Before = previous;
                    var value = mod.NewValue(previous);
                    // A [SerializeReference] field holds {rid: n}; its override sets the id.
                    if (previous is YMap rid && rid.Count == 1 && rid.Keys[0] == "rid" && value is YScalar id)
                    {
                        var m = new YMap { Flow = true };
                        m["rid"] = id;
                        value = m;
                    }
                    if (previous is YMap pm && !pm.IsRef() && value is YScalar)
                    {
                        mod.Status = ModStatus.NoProperty;
                        mod.Detail = $"{PropPath.ToShort(mod.Path)} is a group of values, not one";
                        continue;
                    }
                    slot.Put(value);
                    mod.Status = previous != null && PropPath.SameValue(previous, value) ? ModStatus.Redundant : ModStatus.Applied;
                }
                obj.SetBy[mod.Path] = mod;
            }
        }

        /// <summary>True when a path runs through a list element past the end of that list in the source.</summary>
        static bool InAddedElement(Obj obj, string path)
        {
            var at = 0;
            while ((at = path.IndexOf(".Array.data[", at, StringComparison.Ordinal)) >= 0)
            {
                var close = path.IndexOf(']', at);
                if (close < 0 || !int.TryParse(path.AsSpan(at + 12, close - at - 12), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                    return false;
                var list = path.Substring(0, at);
                if (obj.Source?.Body != null && PropPath.TryResolve(obj.Source.Body, list, out var slot, out _) && slot.Get() is YSeq s && index >= s.Items.Count)
                    return true;
                at = close;
            }
            return false;
        }

        /// <summary>A new list element as Unity creates it: the shape of its neighbour with every value zeroed and every reference empty.</summary>
        static YNode Zeroed(YNode like)
        {
            switch (like)
            {
                case YMap m when m.IsRef():
                    return YNodeExtensions.MakeRef(default);
                case YMap m:
                    var copy = new YMap { Flow = m.Flow };
                    for (var i = 0; i < m.Count; i++)
                        copy[m.Keys[i]] = m.Keys[i] == "serializedVersion" ? m.Values[i].Clone() : Zeroed(m.Values[i]);
                    return copy;
                case YSeq:
                    return new YSeq();
                case YScalar s:
                    return new YScalar(s.Quoted || !double.TryParse(s.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ? "" : "0", s.Quoted);
                default:
                    return new YScalar("0");
            }
        }

        /// <summary>An object an override targets in a source that can't be read and that no stripped object names.</summary>
        static Obj Placeholder(Asset asset, Layer layer, Mod mod)
        {
            var obj = new Obj { Owner = asset, Id = layer.Map(mod.Target.FileId), ClassId = 0, TypeName = "?", Body = new YMap(), Layer = layer, Opaque = true, Line = layer.Line };
            obj.Body["m_CorrespondingSourceObject"] = YNodeExtensions.MakeRef(mod.Target);
            asset.Objects[obj.Id] = obj;
            return obj;
        }

        /// <summary>For a source that can't be read, the override's value is all that's known.</summary>
        static void ApplyOpaque(Obj obj, Mod mod)
        {
            if (PropPath.TryResolve(obj.Body, mod.Path, out var slot, out _) && !slot.IsSize)
                slot.Put(mod.NewValue(slot.Get()));
            else
                obj.Body[mod.Path] = mod.NewValue(null);
            obj.SetBy[mod.Path] = mod;
        }
    }
}
