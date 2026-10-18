using System;
using System.Collections.Generic;
using System.Linq;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Model
{
    /// <summary>
    /// Links GameObjects, components and transforms, including what instances add to or remove from their sources.
    /// Objects written in the file are linked from their own fields; instanced objects from their source's links.
    /// </summary>
    public static class Hierarchy
    {
        public const int SceneRootsClass = 1660057539;

        public static void Build(Asset asset)
        {
            var all = asset.Objects.Values.OrderBy(o => o.Line).ToList();
            foreach (var o in all)
            {
                if (o.IsGameObject)
                {
                    o.GameObject = o;
                    continue;
                }
                if (o.Source != null)
                {
                    var sgo = o.Source.GameObject;
                    o.GameObject = sgo == null ? null : asset.Get(o.Layer.Map(sgo.Id));
                }
                else if (o.Body?["m_GameObject"] is YMap r && r.IsRef())
                    o.GameObject = asset.Get(r.AsRef().FileId);
            }

            var owned = new Dictionary<Obj, List<Obj>>();
            foreach (var o in all)
                if (o.GameObject != null && o.GameObject != o)
                    Add(owned, o.GameObject, o);

            foreach (var go in all.Where(o => o.IsGameObject))
            {
                var order = new List<Obj>();
                if (go.Source != null)
                {
                    foreach (var c in go.Source.Components)
                        AddIfOwned(order, asset.Get(go.Layer.Map(c.Id)), go);
                }
                else if (go.Body.Seq("m_Component") is YSeq listed)
                {
                    foreach (var item in listed.Items)
                        AddIfOwned(order, asset.Get(ComponentRef(item)), go);
                }
                if (owned.TryGetValue(go, out var mine))
                {
                    var records = go.Layer?.AddedComponents;
                    foreach (var c in mine.OrderBy(c => records == null ? int.MaxValue : IndexOr(records.FindIndex(r => r.added == c.Id), int.MaxValue)).ThenBy(c => c.Line))
                        if (!order.Contains(c))
                            Insert(order, c, records, go);
                }
                go.Components.AddRange(order);
                go.Transform = order.FirstOrDefault(c => c.IsTransform);
            }

            var transforms = all.Where(o => o.IsTransform).ToList();
            var childrenOf = new Dictionary<Obj, List<Obj>>();
            foreach (var t in transforms)
            {
                if (t.Source != null)
                    t.Father = t.Source.Father != null ? asset.Get(t.Layer.Map(t.Source.Father.Id)) : asset.Get(t.Layer.TransformParent);
                else if (t.Body?["m_Father"] is YMap f && f.IsRef())
                    t.Father = asset.Get(f.AsRef().FileId);
                if (t.Father != null)
                    Add(childrenOf, t.Father, t);
            }
            foreach (var t in transforms)
            {
                childrenOf.TryGetValue(t, out var actual);
                actual ??= new List<Obj>();
                var order = new List<Obj>();
                if (t.Source != null)
                {
                    foreach (var c in t.Source.ChildTransforms)
                    {
                        var mine = asset.Get(t.Layer.Map(c.Id));
                        if (mine != null && actual.Contains(mine) && !order.Contains(mine))
                            order.Add(mine);
                    }
                }
                else if (t.Body.Seq("m_Children") is YSeq listed)
                {
                    foreach (var item in listed.Items)
                    {
                        var c = asset.Get(item.AsRef().FileId);
                        if (c != null && actual.Contains(c) && !order.Contains(c))
                            order.Add(c);
                    }
                }
                var records = t.Layer?.AddedGameObjects;
                var extras = actual.Where(c => !order.Contains(c))
                    .OrderBy(c => records == null ? int.MaxValue : IndexOr(records.FindIndex(r => r.added == c.Id), int.MaxValue))
                    .ThenBy(c => c.Line).ToList();
                if (t.Source != null)
                {
                    // Files saved before Unity 2022 order the children an instance adds by their m_RootOrder.
                    var ranked = extras.Select(c => (c, rank: RootOrder(c))).Where(e => e.rank >= 0).OrderBy(e => e.rank).ToList();
                    foreach (var (c, rank) in ranked)
                    {
                        order.Insert(Math.Min(rank, order.Count), c);
                        extras.Remove(c);
                    }
                }
                foreach (var c in extras)
                    Insert(order, c, t.Layer?.AddedGameObjects, t);
                t.ChildTransforms.AddRange(order);
                var go = t.GameObject;
                if (go == null || go.Transform != t)
                    continue;
                foreach (var c in order)
                    if (c.GameObject != null)
                    {
                        go.Children.Add(c.GameObject);
                        c.GameObject.Parent = go;
                    }
            }

            // An object under a transform whose GameObject can't be read (from an unexported model) is listed as a root too.
            var roots = all.Where(o => o.IsGameObject && o.Parent == null && o.Transform != null && (o.Transform.Father == null || o.Transform.Father.GameObject == null)).ToList();
            var sceneRoots = all.FirstOrDefault(o => o.ClassId == SceneRootsClass)?.Body.Seq("m_Roots");
            if (sceneRoots != null)
            {
                // Entries are root transforms, or the PrefabInstance for a root that is an instance.
                foreach (var item in sceneRoots.Items)
                {
                    var id = item.AsRef().FileId;
                    var go = asset.Get(id)?.GameObject ?? asset.Layers.Find(l => l.Id == id)?.Root;
                    if (go != null && roots.Remove(go))
                        asset.Roots.Add(go);
                }
            }
            asset.Roots.AddRange(roots);

            asset.HierarchyBuilt = true;
            foreach (var o in all)
                if (o.Source != null && o.HasBody)
                    Fix(o);
        }

        /// <summary>Writes the links an instance changed (added or removed components and children, the root's parent) into an instanced object's values.</summary>
        public static void Fix(Obj o)
        {
            var body = o.Body;
            if (o.IsGameObject)
            {
                var listed = body.Seq("m_Component");
                if (listed == null || listed.Items.Count != o.Components.Count || o.Components.Where((c, i) => ComponentRef(listed.Items[i]) != c.Id).Any())
                {
                    var seq = new YSeq();
                    foreach (var c in o.Components)
                    {
                        var m = new YMap();
                        m["component"] = YNodeExtensions.MakeRef(new Ref(c.Id, null));
                        seq.Items.Add(m);
                    }
                    body["m_Component"] = seq;
                }
            }
            else if (o.IsTransform)
            {
                body["m_Father"] = YNodeExtensions.MakeRef(new Ref(o.Father?.Id ?? 0, null));
                var listed = body.Seq("m_Children");
                if (listed == null || listed.Items.Count != o.ChildTransforms.Count || o.ChildTransforms.Where((c, i) => listed.Items[i].AsRef().FileId != c.Id).Any())
                {
                    var seq = new YSeq();
                    foreach (var c in o.ChildTransforms)
                        seq.Items.Add(YNodeExtensions.MakeRef(new Ref(c.Id, null)));
                    body["m_Children"] = seq;
                }
            }
        }

        static int IndexOr(int index, int fallback) => index < 0 ? fallback : index;

        static int RootOrder(Obj t)
        {
            string value = null;
            if (t.Source == null)
                value = t.Body?.Scalar("m_RootOrder");
            else if (t.Source.Father == null)
                value = t.Layer.Mods.LastOrDefault(m => m.Path == "m_RootOrder" && m.Target.FileId == t.Source.Id)?.Value;
            return int.TryParse(value, out var v) ? v : -1;
        }

        static void Add(Dictionary<Obj, List<Obj>> map, Obj key, Obj value)
        {
            if (!map.TryGetValue(key, out var list))
                map[key] = list = new List<Obj>();
            list.Add(value);
        }

        static void AddIfOwned(List<Obj> order, Obj c, Obj go)
        {
            if (c != null && c.GameObject == go && !order.Contains(c))
                order.Add(c);
        }

        static long ComponentRef(YNode item)
        {
            if (item is YMap m)
                foreach (var v in m.Values)
                    if (v.IsRef())
                        return v.AsRef().FileId;
            return 0;
        }

        /// <summary>Places an object an instance added among its source's objects, at the index the instance recorded.</summary>
        static void Insert(List<Obj> order, Obj added, List<(Ref target, int index, long added)> records, Obj parent)
        {
            if (records != null && parent.Source != null)
                foreach (var r in records)
                    if (r.added == added.Id && r.target.FileId == parent.Source.Id && r.index >= 0 && r.index <= order.Count)
                    {
                        order.Insert(r.index, added);
                        return;
                    }
            order.Add(added);
        }

        /// <summary>A GameObject, its components and everything below it.</summary>
        public static IEnumerable<Obj> Subtree(Obj go)
        {
            yield return go;
            foreach (var c in go.Components)
                if (c != go)
                    yield return c;
            foreach (var child in go.Children)
                foreach (var o in Subtree(child))
                    yield return o;
        }

        /// <summary>"Root/Child/Leaf", with [n] after a name that siblings share (n counts from 0).</summary>
        public static string PathOf(Obj go)
        {
            var parts = new List<string>();
            for (var o = go; o != null; o = o.Parent)
                parts.Add(Segment(o));
            parts.Reverse();
            return string.Join("/", parts);
        }

        public static string Segment(Obj go)
        {
            var name = go.Name;
            var siblings = go.Parent != null ? go.Parent.Children : go.Owner.Roots;
            var same = 0;
            var index = -1;
            foreach (var s in siblings)
            {
                if (s.Name != name)
                    continue;
                if (s == go)
                    index = same;
                same++;
            }
            return same > 1 && index >= 0 ? $"{name}[{index}]" : name;
        }
    }
}
