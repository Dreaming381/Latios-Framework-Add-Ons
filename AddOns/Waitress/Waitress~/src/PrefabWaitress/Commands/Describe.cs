using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>Names for objects, components and values, and lookup of the objects a command names.</summary>
    public static class Describe
    {
        /// <summary>Fields every object has that say nothing about it, left out unless asked for.</summary>
        public static readonly HashSet<string> Noise = new HashSet<string>
        {
            "m_ObjectHideFlags", "m_CorrespondingSourceObject", "m_PrefabInstance", "m_PrefabAsset", "m_GameObject", "m_Script",
            "m_EditorHideFlags", "m_EditorClassIdentifier", "serializedVersion", "m_Component", "m_Children", "m_Father",
            "m_PrefabParentObject", "m_PrefabInternal",
        };

        /// <summary>A component's type as people name it: the script class for a MonoBehaviour.</summary>
        public static string TypeOf(Project p, Obj o)
        {
            if (o.ClassId != 114 || o.Body == null)
                return o.TypeName;
            var r = o.Body["m_Script"].AsRef();
            if (r.IsNull)
                return o.Source != null ? TypeOf(p, o.Source) : "MonoBehaviour";
            var path = p.Index.PathOf(r.Guid);
            if (path != null && path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                return Path.GetFileNameWithoutExtension(path);
            var named = p.Scripts.Find(r.Guid, r.FileId);
            if (named != null)
                return named;
            return path == null ? "MonoBehaviour (missing script)" : $"{Path.GetFileName(path)}#{r.FileId}";
        }

        /// <summary>The components of a GameObject named as `Type`, or `Type[n]` when it has several of a type.</summary>
        public static List<(string name, Obj comp)> ComponentNames(Project p, Obj go)
        {
            var result = new List<(string, Obj)>();
            var types = go.Components.Select(c => TypeOf(p, c)).ToList();
            for (var i = 0; i < types.Count; i++)
            {
                var same = types.Count(t => t == types[i]);
                var index = types.Take(i).Count(t => t == types[i]);
                result.Add((same > 1 ? $"{types[i]}[{index}]" : types[i], go.Components[i]));
            }
            return result;
        }

        public static string Where(Obj o)
        {
            if (o.GameObject != null)
                return Hierarchy.PathOf(o.GameObject) + (o.IsGameObject ? "" : " " + o.TypeName);
            return $"{o.TypeName} &{o.Id}";
        }

        /// <summary>A value as one line: references by path, everything else in YAML flow style.</summary>
        public static string Value(Project p, Asset a, YNode v, int max = 160)
        {
            if (v == null)
                return "~";
            if (v.IsRef())
                return Ref(p, a, v.AsRef());
            if (v is YSeq s && s.Items.Count > 0 && s.Items.All(i => i.IsRef()))
            {
                var parts = s.Items.Take(8).Select(i => Ref(p, a, i.AsRef())).ToList();
                if (s.Items.Count > 8)
                    parts.Add($"... {s.Items.Count} items");
                return "[" + string.Join(", ", parts) + "]";
            }
            return v.Flat(max);
        }

        public static string Ref(Project p, Asset a, Ref r)
        {
            if (r.IsNull)
                return "None";
            if (r.IsLocal)
            {
                var o = a?.Get(r.FileId);
                if (o == null)
                    return $"missing &{r.FileId}";
                if (o.IsGameObject)
                    return Hierarchy.PathOf(o);
                return o.GameObject != null ? $"{Hierarchy.PathOf(o.GameObject)} ({TypeOf(p, o)})" : $"{o.TypeName} &{o.Id}";
            }
            var path = p.Index.Describe(r.Guid, r.FileId);
            if (path.StartsWith("builtin:", StringComparison.Ordinal) || path.StartsWith("missing", StringComparison.Ordinal))
                return path;
            if (path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) || Asset.IsModelPath(path))
            {
                var other = p.Load(path);
                var o = other.Readable ? other.Get(r.FileId) : null;
                if (o != null)
                {
                    if (o.IsGameObject && other.Roots.Count == 1 && other.Roots[0] == o)
                        return path;
                    var go = o.GameObject ?? o;
                    return $"{path}:{Hierarchy.PathOf(go)}" + (o.IsGameObject ? "" : $" ({TypeOf(p, o)})");
                }
            }
            return r.FileId % 100000 == 0 && r.FileId < 100000L * 100000 ? path : $"{path}#{r.FileId}";
        }

        public static string Short(string unityPath) => PropPath.ToShort(unityPath);

        /// <summary>
        /// The GameObjects a spec names: a hierarchy path ("Root/Child", [n] picks among same-named siblings), a name
        /// found anywhere, or &amp;fileID.
        /// </summary>
        public static List<Obj> FindObjects(Asset a, string spec)
        {
            if (spec.StartsWith("&", StringComparison.Ordinal) && long.TryParse(spec.Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                var o = a.Get(id);
                return o == null ? new List<Obj>() : new List<Obj> { o.GameObject ?? o };
            }
            // Names may contain / or [n] themselves, so an exact path or name wins before the spec is split.
            var exact = a.GameObjects.Where(g => Hierarchy.PathOf(g) == spec).ToList();
            if (exact.Count == 0 && (spec.Contains('/') || spec.EndsWith("]", StringComparison.Ordinal)))
                exact = a.GameObjects.Where(g => g.Name == spec).ToList();
            if (exact.Count > 0)
                return exact;
            var parts = spec.Split('/');
            var byPath = new List<Obj>();
            Walk(a.Roots, parts, 0, byPath);
            if (byPath.Count > 0)
                return byPath;
            var tail = parts[parts.Length - 1];
            var found = a.GameObjects.Where(g => SegmentMatches(g, tail)).ToList();
            if (parts.Length > 1)
                found = found.Where(g => Hierarchy.PathOf(g).EndsWith("/" + spec, StringComparison.OrdinalIgnoreCase) || Hierarchy.PathOf(g).Equals(spec, StringComparison.OrdinalIgnoreCase)).ToList();
            return found.OrderBy(g => Hierarchy.PathOf(g), StringComparer.Ordinal).ToList();
        }

        static void Walk(List<Obj> level, string[] parts, int i, List<Obj> found)
        {
            foreach (var g in level)
            {
                if (!SegmentMatches(g, parts[i]))
                    continue;
                if (i == parts.Length - 1)
                    found.Add(g);
                else
                    Walk(g.Children, parts, i + 1, found);
            }
        }

        static bool SegmentMatches(Obj g, string part)
        {
            if (part.Contains('*') || part.Contains('?'))
                return Glob.Compile(part)(g.Name);
            if (string.Equals(Hierarchy.Segment(g), part, StringComparison.Ordinal))
                return true;
            return string.Equals(g.Name, part, StringComparison.Ordinal) && !part.EndsWith("]", StringComparison.Ordinal);
        }

        /// <summary>Exactly one GameObject, or an error naming the candidates.</summary>
        public static Obj FindOne(Asset a, string spec)
        {
            var found = FindObjects(a, spec);
            if (found.Count == 1)
                return found[0];
            if (found.Count == 0)
            {
                var close = a.GameObjects.Where(g => g.Name.Contains(spec.Split('/').Last(), StringComparison.OrdinalIgnoreCase)).Take(8).Select(g => $"{Hierarchy.PathOf(g)} (&{g.Id})").ToList();
                throw new WaitressException($"no object '{spec}' in {a.Path}" + (close.Count > 0 ? "; similar: " + string.Join(", ", close) : "; `tree` lists them"));
            }
            throw new WaitressException($"'{spec}' names {found.Count} objects in {a.Path}: " + string.Join(", ", found.Take(10).Select(g => $"{Hierarchy.PathOf(g)} (&{g.Id})")) + (found.Count > 10 ? ", ..." : "") + "; use one of these paths (with [n] for same-named siblings) or its &id");
        }

        /// <summary>A component of a GameObject by type or script name, `Type[n]`, or "GameObject" for the object itself.</summary>
        public static Obj FindComponent(Project p, Obj go, string spec)
        {
            if (string.Equals(spec, "GameObject", StringComparison.OrdinalIgnoreCase))
                return go;
            var names = ComponentNames(p, go);
            var hit = names.Where(n => string.Equals(n.name, spec, StringComparison.OrdinalIgnoreCase)).ToList();
            if (hit.Count == 0)
                hit = names.Where(n => string.Equals(n.name.Split('[')[0], spec, StringComparison.OrdinalIgnoreCase)).ToList();
            if (hit.Count == 1)
                return hit[0].comp;
            if (hit.Count > 1)
                throw new WaitressException($"{Hierarchy.PathOf(go)} has {hit.Count} {spec} components; use " + string.Join(" or ", hit.Select(h => h.name)));
            throw new WaitressException($"{Hierarchy.PathOf(go)} has no {spec}; it has GameObject, " + string.Join(", ", names.Select(n => n.name)));
        }

        /// <summary>Splits "Component.path.to.field" into the component and the Unity property path.</summary>
        public static (Obj comp, string path) FindProperty(Project p, Obj go, string spec)
        {
            var names = ComponentNames(p, go).Select(n => n.name).Append("GameObject").OrderByDescending(n => n.Length);
            foreach (var n in names)
                if (spec.StartsWith(n + ".", StringComparison.OrdinalIgnoreCase))
                {
                    var comp = FindComponent(p, go, n);
                    return (comp, ResolvePath(comp, spec.Substring(n.Length + 1)));
                }
            var dot = spec.IndexOf('.');
            if (dot > 0)
            {
                var comp = FindComponent(p, go, spec.Substring(0, dot));
                return (comp, ResolvePath(comp, spec.Substring(dot + 1)));
            }
            // A bare property: the one component that has it.
            var owners = go.Components.Prepend(go).Distinct().Where(c => c.Body != null && PropPath.TryResolve(c.Body, PropPath.ToUnity(spec), out _, out _)).ToList();
            if (owners.Count == 1)
                return (owners[0], PropPath.ToUnity(spec));
            if (owners.Count > 1)
                throw new WaitressException($"{spec} is on several components ({string.Join(", ", owners.Select(o => TypeOf(p, o)))}); write Component.{spec}");
            throw new WaitressException($"no component of {Hierarchy.PathOf(go)} has {spec}; write Component.property, e.g. Transform.m_LocalPosition.x");
        }

        /// <summary>The Unity form of a property path, accepting "[i]" for ".Array.data[i]" and ".size" for ".Array.size".</summary>
        public static string ResolvePath(Obj comp, string spec)
        {
            var unity = PropPath.ToUnity(spec);
            if (comp.Body == null || PropPath.TryResolve(comp.Body, unity, out _, out _))
                return unity;
            if (unity.EndsWith(".size", StringComparison.Ordinal))
            {
                var sized = unity.Substring(0, unity.Length - 5) + ".Array.size";
                if (PropPath.TryResolve(comp.Body, sized, out _, out _))
                    return sized;
            }
            return unity;
        }

        /// <summary>"; did you mean a, b" from an object's property paths that resemble the given one.</summary>
        public static string Suggest(Obj comp, string path)
        {
            if (comp.Body == null)
                return "";
            var want = PropPath.ToShort(path).ToLowerInvariant();
            var last = want.Split('.').Last();
            var all = PropPath.Leaves(comp.Body).Select(l => PropPath.ToShort(l.path)).Where(p => !p.Contains('[')).Distinct().ToList();
            var close = all.Select(p => (p, d: Distance(p.ToLowerInvariant(), want)))
                .Where(e => e.d <= Math.Max(2, want.Length / 3) || e.p.ToLowerInvariant().Contains(last.TrimStart('m', '_')))
                .OrderBy(e => e.d).Take(6).Select(e => e.p).ToList();
            return close.Count > 0 ? "; did you mean " + string.Join(", ", close) : "; `show <file> <object> <Component> --all` lists its properties";
        }

        static int Distance(string a, string b)
        {
            var d = new int[a.Length + 1, b.Length + 1];
            for (var i = 0; i <= a.Length; i++)
                d[i, 0] = i;
            for (var j = 0; j <= b.Length; j++)
                d[0, j] = j;
            for (var i = 1; i <= a.Length; i++)
                for (var j = 1; j <= b.Length; j++)
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
            return d[a.Length, b.Length];
        }

        public static string Plural(int n, string one, string many = null) => n == 1 ? $"1 {one}" : $"{n} {many ?? one + "s"}";
    }
}
