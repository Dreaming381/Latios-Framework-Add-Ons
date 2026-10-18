using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ModelWaitress.Model;

namespace ModelWaitress.Commands
{
    /// <summary>Commands that describe one model from its exported record.</summary>
    public static class Read
    {
        static readonly string[] s_RequiredHuman =
        {
            "Hips", "LeftUpperLeg", "RightUpperLeg", "LeftLowerLeg", "RightLowerLeg", "LeftFoot", "RightFoot", "Spine", "Head",
            "LeftUpperArm", "RightUpperArm", "LeftLowerArm", "RightLowerArm", "LeftHand", "RightHand",
        };

        // ------------------------------------------------------------ info

        public static int Info(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            w.WriteLine(m.Path + (m.Stale ? "  (last export; the file changed since)" : ""));
            if (m.Header != null)
                Line(w, "source", m.Header.Describe());
            Line(w, "scale", ScaleLine(m));
            if (SizeLine(m) is string size)
                Line(w, "size", size);
            var tilted = TiltedNodes(m).ToList();
            if (tilted.Count > 0)
                Line(w, "rotated", string.Join("; ", tilted));
            Line(w, "rig", RigLine(m));
            Line(w, "meshes", MeshLine(m));
            Line(w, "materials", MaterialLine(m));
            Line(w, "clips", ClipLine(m));
            var changed = m.Settings.Where(s => s.Changed).Select(s => $"{s.Name}={Show(s.Value)} ({Show(s.Default)})")
                           .Concat(HumanChanges(m).Select(p => $"human.{p.Key}={p.Value} ({HumanDefaults[p.Key]})")).ToList();
            Line(w, "changed", changed.Count == 0 ? "nothing (every setting is Unity's default)"
                : Fmt.Plural(changed.Count, "setting") + ": " + string.Join(", ", changed));
            Line(w, "messages", m.Messages.Count == 0 ? "none" : string.Join("\n            ", MessageLines(m)));
            return 0;
        }

        public static IEnumerable<string> MessageLines(ModelRecord m) =>
            m.Messages.Select(x => x.level + ": " + string.Join("\n              ", x.text.Replace(" in " + m.Path, "")
                                                                          .Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0)) + Hint(x.text))
             .GroupBy(s => s).Select(g => g.Count() > 1 ? $"{g.Key} ({g.Count()} times)" : g.Key);

        // Fixes for import messages whose cause was measured.
        static string Hint(string text)
        {
            if (text.Contains("Avatar Rig Configuration mis-match", StringComparison.Ordinal))
                return "\n              -> the avatar's stored skeleton pose no longer matches the file (typically after a scale change);" +
                       "\n                 `set <model> human.skeleton=rebuild` takes a fresh pose and keeps the bone mapping";
            return "";
        }

        public static readonly Dictionary<string, string> HumanDefaults = new Dictionary<string, string>
        {
            ["upperArmTwist"] = "0.5", ["lowerArmTwist"] = "0.5", ["upperLegTwist"] = "0.5", ["lowerLegTwist"] = "0.5",
            ["armStretch"] = "0.05", ["legStretch"] = "0.05", ["feetSpacing"] = "0", ["hasTranslationDoF"] = "false",
        };

        static IEnumerable<KeyValuePair<string, string>> HumanChanges(ModelRecord m) =>
            m.Get("animationType") != "Human" ? Enumerable.Empty<KeyValuePair<string, string>>()
                : m.HumanParams.Where(p => HumanDefaults.TryGetValue(p.Key, out var d) && d != p.Value);

        static bool BonesFolded(ModelRecord m, Component c) =>
            m.Is("optimizeGameObjects") && c.Data.Length > 3 && c.Data[3].ValueKind == JsonValueKind.Array && c.Data[3].GetArrayLength() == 0;

        /// <summary>The extent of every mesh placed by its node, in the model's space (metres after import scale).</summary>
        public static string SizeLine(ModelRecord m)
        {
            var world = new double[m.Nodes.Count][];
            var folded = false;
            double[] min = null, max = null;
            for (var i = 0; i < m.Nodes.Count; i++)
            {
                var n = m.Nodes[i];
                var local = Mat.Trs(n.P, n.R, n.S);
                world[i] = n.Parent >= 0 && n.Parent < i ? Mat.Mul(world[n.Parent], local) : local;
                foreach (var c in n.Components)
                {
                    if (c.Type is not ("MeshFilter" or "SkinnedMeshRenderer") || c.Data.Length == 0 || c.Data[0].ValueKind != JsonValueKind.Number)
                        continue;
                    if (c.Type == "SkinnedMeshRenderer" && BonesFolded(m, c))
                    {
                        folded = true;
                        continue;
                    }
                    var index = c.Data[0].GetInt32();
                    if (index < 0 || index >= m.Meshes.Count || m.Meshes[index].Center == null)
                        continue;
                    var mesh = m.Meshes[index];
                    for (var corner = 0; corner < 8; corner++)
                    {
                        var p = new double[3];
                        for (var k = 0; k < 3; k++)
                            p[k] = mesh.Center[k] + ((corner >> k & 1) == 0 ? -1 : 1) * mesh.Extents[k];
                        var q = Mat.Point(world[i], p);
                        min ??= (double[])q.Clone();
                        max ??= (double[])q.Clone();
                        for (var k = 0; k < 3; k++)
                        {
                            min[k] = Math.Min(min[k], q[k]);
                            max[k] = Math.Max(max[k], q[k]);
                        }
                    }
                }
            }
            const string skinNote = "skinned meshes follow bones inside the Animator (optimizeGameObjects), so their extent isn't known here";
            if (min == null)
                return folded ? "not computed: " + skinNote : null;
            return (folded ? "without skinned meshes (" + skinNote + "): " : "") + $"{Fmt.Num(max[0] - min[0])} × {Fmt.Num(max[1] - min[1])} × {Fmt.Num(max[2] - min[2])} m (x × y × z), y from {Fmt.Num(min[1])} to {Fmt.Num(max[1])}";
        }

        static void Line(TextWriter w, string label, string text) => w.WriteLine("  " + label.PadRight(10) + text);

        public static string Show(string v) => v == null ? "None" : v.Length == 0 ? "\"\"" : v;

        public static string ScaleLine(ModelRecord m)
        {
            var global = m.Get("globalScale") ?? "1";
            var g = double.Parse(global, System.Globalization.CultureInfo.InvariantCulture);
            // ModelImporter.fileScale is the unit conversion actually applied: 1 when useFileScale is off or the format has none.
            var total = g * m.FileScale;
            var why = !m.IsFbx || string.IsNullOrEmpty(m.Unit) ? (m.Is("useFileScale") && m.FileScale == 1 ? " (this format carries no unit conversion)" : "")
                : m.Is("useFileScale") ? $" (useFileScale on: file unit {m.Unit} = {Fmt.Num(m.UnitFactor)} m)" : $" (useFileScale off: the file's unit, {m.Unit}, is ignored)";
            return $"{Fmt.Num(total)} = globalScale {global} × fileScale {Fmt.Num(m.FileScale)}{why}; baked into vertices and positions, the root keeps scale 1";
        }

        public static IEnumerable<string> TiltedNodes(ModelRecord m)
        {
            for (var i = 0; i < m.Nodes.Count; i++)
            {
                var n = m.Nodes[i];
                if (i != 0 && n.Parent != 0)
                    continue;
                // An optimized skinned mesh keeps a placeholder transform; the skin follows the bones.
                if (n.Components.Any(c => c.Type == "SkinnedMeshRenderer" && BonesFolded(m, c)))
                    continue;
                var parts = new List<string>();
                if (!Fmt.IsIdentityRotation(n.R))
                    parts.Add("rotation " + Fmt.Vec(n.R));
                if (!Fmt.IsUnitScale(n.S))
                    parts.Add("scale " + Fmt.Vec(n.S));
                if (parts.Count > 0)
                    yield return (i == 0 ? n.Name + " (root)" : n.Name) + " " + string.Join(", ", parts);
            }
        }

        static string RigLine(ModelRecord m)
        {
            var type = m.Get("animationType");
            var parts = new List<string> { type == "Human" ? "Humanoid" : type };
            if (type != "None")
            {
                var setup = m.Get("avatarSetup");
                parts.Add(setup == "CopyFromOther" ? "avatar copied from " + Show(m.Get("sourceAvatar")) : setup == "NoAvatar" ? "no avatar" : "avatar created from this model");
                foreach (var a in m.Avatars)
                    parts.Add(m.AvatarOk(a) ? $"{a.name} valid{(a.human ? ", human" : "")}"
                        : a.valid ? $"{a.name} NOT HUMAN: the humanoid mapping failed, so it animates as nothing (see messages)" : $"{a.name} INVALID");
                if (type == "Human")
                {
                    var missing = s_RequiredHuman.Where(h => !m.HumanMap.Any(x => x.human == h)).ToList();
                    parts.Add($"{m.HumanMap.Count} bones mapped" + (missing.Count > 0 && m.Get("avatarSetup") != "CopyFromOther" ? ", required unmapped: " + string.Join(" ", missing) : ""));
                }
            }
            var skinned = m.Meshes.Where(x => x.Bindposes > 0).ToList();
            if (skinned.Count > 0)
                parts.Add("skin weights " + (m.Get("skinWeights") == "Custom" ? "up to " + m.Get("maxBonesPerVertex") : "up to 4") + " per vertex");
            var transforms = m.Nodes.Count;
            if (m.Is("optimizeGameObjects"))
                parts.Add(OptimizeIdle(m) is string why ? $"{Fmt.Plural(transforms, "transform")}; optimizeGameObjects is on but {why}"
                              : $"optimizeGameObjects: {transforms} of {m.TransformPaths.Count} transforms kept");
            else
                parts.Add(Fmt.Plural(transforms, "transform"));
            return string.Join(", ", parts);
        }

        static string MeshLine(ModelRecord m)
        {
            if (m.Meshes.Count == 0)
                return "none";
            var skinned = m.Meshes.Count(x => x.Bindposes > 0);
            var shapes = m.Meshes.Sum(x => x.Shapes.Count);
            var parts = new List<string>
            {
                $"{m.Meshes.Count} ({(skinned > 0 ? skinned + " skinned, " : "")}{Fmt.Count(m.Meshes.Sum(x => x.Verts))} verts, {Fmt.Count(m.Meshes.Sum(x => x.Tris))} tris)",
            };
            if (shapes > 0)
                parts.Add(Fmt.Plural(shapes, "blend shape"));
            if (m.Meshes.Any(x => x.Index == "UInt32"))
                parts.Add("32-bit indices on " + string.Join(", ", m.Meshes.Where(x => x.Index == "UInt32").Select(x => x.Name)));
            parts.Add("isReadable " + (m.Is("isReadable") ? "on (CPU copy kept)" : "off"));
            return string.Join(", ", parts);
        }

        static string MaterialLine(ModelRecord m)
        {
            var slots = Slots(m).ToList();
            var parts = new List<string> { "materialImportMode " + m.Get("materialImportMode") };
            if (slots.Count > 0)
            {
                var embedded = slots.Count(s => s.material != null && s.material.StartsWith(m.Path + "#", StringComparison.Ordinal));
                var none = slots.Count(s => s.material == null);
                parts.Add($"{Fmt.Plural(slots.Count, "renderer slot")}: {embedded} from the model, {slots.Count - embedded - none} project materials" + (none > 0 ? $", {none} EMPTY" : ""));
            }
            if (m.Remaps.Count > 0)
            {
                var missing = m.Remaps.Count(r => r.target != null && r.target.StartsWith("missing:", StringComparison.Ordinal));
                parts.Add(Fmt.Plural(m.Remaps.Count, "remap") + (missing > 0 ? $" ({missing} to MISSING assets; see `materials`)" : ""));
            }
            return string.Join(", ", parts);
        }

        static string ClipLine(ModelRecord m)
        {
            if (m.Anims.Count == 0)
                return m.Takes.Count > 0 ? $"none produced ({Fmt.Plural(m.Takes.Count, "take")} in the file; importAnimation={m.Get("importAnimation")}, animationType={m.Get("animationType")})" : "none";
            return string.Join("; ", m.Anims.Select(a => $"{a.Name} {Fmt.Seconds(a.Length)} @{Fmt.Num(a.Fps)}fps{(a.Loop ? " loop" : "")}{(a.Legacy ? " legacy" : "")}")) +
                   (m.ClipsCustom ? "" : " (one clip per take, not customized)");
        }

        /// <summary>Every renderer material slot: node path, renderer type, slot index, material ("path" or "model#name", null when empty).</summary>
        public static IEnumerable<(string node, string renderer, int slot, string material)> Slots(ModelRecord m)
        {
            for (var i = 0; i < m.Nodes.Count; i++)
                foreach (var c in m.Nodes[i].Components)
                {
                    var index = c.Type == "SkinnedMeshRenderer" ? 1 : c.Type.EndsWith("Renderer", StringComparison.Ordinal) ? 0 : -1;
                    if (index < 0 || c.Data.Length <= index || c.Data[index].ValueKind != JsonValueKind.Array)
                        continue;
                    var slot = 0;
                    foreach (var mat in c.Data[index].EnumerateArray())
                        yield return (m.PathOf(i), c.Type, slot++, mat.ValueKind == JsonValueKind.String ? mat.GetString() : null);
                }
        }

        // ------------------------------------------------------------ settings

        public static int Settings(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            var all = args.Flag("all");
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            var filter = args.Positional.Count > 1 ? args.Positional[1] : null;
            var rows = m.Settings.Where(s => (all || filter != null || s.Changed) && Matches(s.Name, filter)).ToList();
            if (filter != null && rows.Count == 0)
                throw new WaitressException($"no setting matches '{filter}'; {Suggest(filter, m.Settings.Select(s => s.Name))}");
            if (rows.Count == 0)
            {
                w.WriteLine("every setting is Unity's default (--all lists them)");
                return 0;
            }
            var width = rows.Max(s => s.Name.Length);
            foreach (var g in rows.GroupBy(s => Groups.Of(s.Name)).OrderBy(g => Array.IndexOf(Groups.Order, g.Key)))
            {
                w.WriteLine(g.Key);
                foreach (var s in g)
                    w.WriteLine($"  {s.Name.PadRight(width)}  {Show(s.Value)}{(s.Changed ? "  (default " + Show(s.Default) + ")" : "")}{EnumHint(session, s, all || filter != null)}");
            }
            if (!all && filter == null)
                w.WriteLine($"({m.Settings.Count - rows.Count} more at their defaults; --all lists them)");
            if (filter == null || Matches("human", filter))
                HumanSettings(m, w, all);
            return 0;
        }

        static string EnumHint(Session session, Setting s, bool show)
        {
            if (!show || !session.Catalog.Project.Enums.TryGetValue(s.Type, out var names))
                return "";
            return "  [" + string.Join("|", names) + "]";
        }

        static void HumanSettings(ModelRecord m, TextWriter w, bool all)
        {
            if (m.Get("animationType") != "Human")
                return;
            var defaults = HumanDefaults;
            var rows = m.HumanParams.Where(p => all || !defaults.TryGetValue(p.Key, out var d) || d != p.Value).ToList();
            if (rows.Count == 0)
                return;
            w.WriteLine("Rig (humanoid; set with human.<name>=value)");
            foreach (var p in rows)
                w.WriteLine($"  human.{p.Key} {p.Value}{(defaults.TryGetValue(p.Key, out var d) && d != p.Value ? "  (default " + d + ")" : "")}");
        }

        public static bool Matches(string name, string filter)
        {
            if (filter == null)
                return true;
            if (filter.IndexOfAny(new[] { '*', '?' }) >= 0)
                return Glob.Compile(filter)(name);
            return name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string Suggest(string key, IEnumerable<string> names)
        {
            var close = names.Distinct().Select(n => (n, d: Distance(n.ToLowerInvariant(), key.ToLowerInvariant()))).Where(x => x.d <= Math.Max(2, key.Length / 3))
                             .OrderBy(x => x.d).Take(4).Select(x => x.n).ToList();
            return close.Count > 0 ? "did you mean " + string.Join(", ", close) + "?" : "`settings <model> --all` lists every setting";
        }

        static int Distance(string a, string b)
        {
            var d = new int[a.Length + 1, b.Length + 1];
            for (var x = 0; x <= a.Length; x++)
                d[x, 0] = x;
            for (var y = 0; y <= b.Length; y++)
                d[0, y] = y;
            for (var x = 1; x <= a.Length; x++)
                for (var y = 1; y <= b.Length; y++)
                    d[x, y] = Math.Min(Math.Min(d[x - 1, y] + 1, d[x, y - 1] + 1), d[x - 1, y - 1] + (a[x - 1] == b[y - 1] ? 0 : 1));
            return d[a.Length, b.Length];
        }

        // ------------------------------------------------------------ tree

        public static int Tree(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            var depth = args.GetInt("depth", int.MaxValue);
            var xforms = args.Flag("xforms");
            var all = args.Flag("all");
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            var start = 0;
            if (args.Positional.Count > 1)
                start = FindNode(m, args.Positional[1]);
            Print(m, start, 0, depth, xforms, w);
            if (m.Is("optimizeGameObjects"))
            {
                if (OptimizeIdle(m) is string why)
                {
                    w.WriteLine("optimizeGameObjects is on but " + why);
                    return 0;
                }
                // Exposed transforms move up to sit under the root, so they are matched by name.
                var kept = new HashSet<string>(m.Nodes.Select(n => n.Name));
                var stripped = m.TransformPaths.Where(p => p.Length > 0 && !kept.Contains(p.Substring(p.LastIndexOf('/') + 1))).ToList();
                // With a node named, only what sits below it in the file.
                if (start > 0)
                {
                    var name = m.Nodes[start].Name;
                    stripped = stripped.Where(p => p.Contains("/" + name + "/", StringComparison.Ordinal) || p.StartsWith(name + "/", StringComparison.Ordinal)).ToList();
                }
                w.WriteLine($"optimizeGameObjects is on: {Fmt.Plural(stripped.Count, "transform")} from the file live only inside the Animator" +
                            (all ? ":" : "; --all lists them; extraExposedTransformPaths keeps one as a GameObject"));
                if (all)
                    foreach (var p in stripped)
                        w.WriteLine("  " + p);
            }
            return 0;
        }

        /// <summary>Why optimizeGameObjects removed nothing, or null when it took effect.</summary>
        public static string OptimizeIdle(ModelRecord m)
        {
            if (m.Nodes.Count < m.TransformPaths.Count)
                return null;
            if (m.Get("animationType") is "None" or "Legacy")
                return $"does nothing: it needs a Generic or Humanoid rig (animationType is {m.Get("animationType")})";
            if (m.Avatars.Count == 0)
                return $"does nothing: it needs an avatar (avatarSetup is {m.Get("avatarSetup")}; CreateFromThisModel makes one)";
            return "removed nothing";
        }

        public static int FindNode(ModelRecord m, string what)
        {
            var paths = Enumerable.Range(0, m.Nodes.Count).Select(i => (i, path: m.PathOf(i))).ToList();
            var hit = paths.Where(p => p.path == what).ToList();
            if (hit.Count == 0)
                hit = paths.Where(p => m.Nodes[p.i].Name == what).ToList();
            if (hit.Count == 1)
                return hit[0].i;
            if (hit.Count > 1)
                throw new WaitressException($"{hit.Count} transforms are named '{what}'; use a path: " + string.Join(", ", hit.Take(5).Select(h => h.path)));
            throw new WaitressException($"no transform '{what}'; {Suggest(what, m.Nodes.Select(n => n.Name))}");
        }

        static void Print(ModelRecord m, int i, int indent, int depth, bool xforms, TextWriter w)
        {
            var n = m.Nodes[i];
            var line = new string(' ', indent * 2) + n.Name;
            var notes = n.Components.Select(c => DescribeComponent(m, c)).ToList();
            if (n.Inactive)
                notes.Insert(0, "inactive");
            if (xforms)
                notes.Add($"p{Fmt.Vec(n.P)} r{Fmt.Vec(n.R)} s{Fmt.Vec(n.S)}");
            else
            {
                if (!Fmt.IsUnitScale(n.S))
                    notes.Add("scale " + Fmt.Vec(n.S));
            }
            w.WriteLine(notes.Count > 0 ? line + "  [" + string.Join("; ", notes) + "]" : line);
            if (indent >= depth)
            {
                if (n.Children.Count > 0)
                    w.WriteLine(new string(' ', indent * 2 + 2) + $"… {Fmt.Plural(Descendants(m, i), "transform")} below");
                return;
            }
            foreach (var c in n.Children)
                Print(m, c, indent + 1, depth, xforms, w);
        }

        static int Descendants(ModelRecord m, int i) => m.Nodes[i].Children.Sum(c => 1 + Descendants(m, c));

        static string MeshName(ModelRecord m, JsonElement e)
        {
            var index = e.ValueKind == JsonValueKind.Number ? e.GetInt32() : -1;
            return index >= 0 && index < m.Meshes.Count ? m.Meshes[index].Name : "None";
        }

        static string DescribeComponent(ModelRecord m, Component c)
        {
            var d = c.Data;
            switch (c.Type)
            {
                case "MeshFilter":
                {
                    var mesh = d.Length > 0 && d[0].ValueKind == JsonValueKind.Number && d[0].GetInt32() >= 0 ? m.Meshes[d[0].GetInt32()] : null;
                    return mesh == null ? "MeshFilter None" : $"mesh {mesh.Name} {Fmt.Count(mesh.Verts)}v";
                }
                case "MeshRenderer":
                    return "MeshRenderer " + MaterialList(m, d.Length > 0 ? d[0] : default);
                case "SkinnedMeshRenderer":
                {
                    var mesh = d[0].GetInt32() >= 0 ? m.Meshes[d[0].GetInt32()] : null;
                    var root = d[2].GetInt32();
                    var bones = d[3].GetArrayLength();
                    var boneText = bones == 0 && m.Is("optimizeGameObjects") && mesh != null && mesh.Bindposes > 0 ? "bones inside the Animator (optimizeGameObjects)" : Fmt.Plural(bones, "bone") + ", root " + (root >= 0 ? m.Nodes[root].Name : "None");
                    return $"Skinned {(mesh == null ? "None" : mesh.Name + " " + Fmt.Count(mesh.Verts) + "v")}, {boneText}, {MaterialList(m, d[1])}";
                }
                case "Animator":
                    return "Animator avatar " + (d.Length > 0 && d[0].ValueKind == JsonValueKind.String ? d[0].GetString() : "None");
                case "Animation":
                    return "Animation (legacy) " + string.Join(", ", d[0].EnumerateArray().Select(x => x.GetString()));
                case "LODGroup":
                    return "LODGroup " + string.Join(" | ", d[0].EnumerateArray().Select(l =>
                    {
                        var a = l.EnumerateArray().ToArray();
                        return Fmt.Num(a[0].GetDouble()) + ": " + string.Join(",", a.Skip(1).Select(x => x.GetInt32() >= 0 ? m.Nodes[x.GetInt32()].Name : "?"));
                    }));
                case "MeshCollider":
                    return "MeshCollider " + MeshName(m, d[0]);
                case "Light":
                    return "Light " + d[0].GetString();
                case "Camera":
                    return "Camera " + d[0].GetString();
            }
            return c.Type;
        }

        static string MaterialList(ModelRecord m, JsonElement list)
        {
            if (list.ValueKind != JsonValueKind.Array)
                return "";
            var names = list.EnumerateArray().Select(x => x.ValueKind == JsonValueKind.String ? MaterialName(m, x.GetString()) : "EMPTY").ToList();
            return (names.Count == 1 ? "material " : "materials ") + string.Join(", ", names);
        }

        public static string MaterialName(ModelRecord m, string material)
        {
            if (material == null)
                return "EMPTY";
            if (material.StartsWith(m.Path + "#", StringComparison.Ordinal))
                return material.Substring(m.Path.Length + 1) + " (embedded)";
            return material;
        }

        // ------------------------------------------------------------ meshes

        public static int Meshes(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            if (m.Meshes.Count == 0)
            {
                w.WriteLine("the model produced no meshes");
                return 0;
            }
            if (args.Positional.Count < 2)
            {
                foreach (var mesh in m.Meshes)
                {
                    var parts = new List<string> { Fmt.Count(mesh.Verts) + " verts", Fmt.Count(mesh.Tris) + " tris" };
                    if (mesh.Sub.Count != 1)
                        parts.Add(Fmt.Plural(mesh.Sub.Count, "submesh"));
                    if (mesh.Index == "UInt32")
                        parts.Add("32-bit indices");
                    if (mesh.Bindposes > 0)
                        parts.Add($"skinned ({mesh.Bindposes} bindposes{(mesh.MaxBonesPerVertex is int b ? ", up to " + b + " bones/vertex" : "")})");
                    if (mesh.Shapes.Count > 0)
                        parts.Add(Fmt.Plural(mesh.Shapes.Count, "blend shape"));
                    var uvs = mesh.Attrs.Where(a => a.attr.StartsWith("TexCoord", StringComparison.Ordinal)).Select(a => a.attr.Substring(8)).ToList();
                    parts.Add(uvs.Count == 0 ? "no UVs" : "uv" + string.Join("", uvs));
                    if (mesh.Lods is int lods && lods > 1)
                        parts.Add(lods + " mesh LODs");
                    w.WriteLine($"{mesh.Name}  {string.Join(", ", parts)}");
                }
                w.WriteLine("isReadable " + (m.Is("isReadable") ? "on" : "off") + "; `meshes <model> <name>` for attributes, submeshes and bounds");
                return 0;
            }
            var name = args.Positional[1];
            var hits = m.Meshes.Where(x => x.Name == name).ToList();
            if (hits.Count == 0)
                throw new WaitressException($"no mesh '{name}'; meshes: {string.Join(", ", m.Meshes.Select(x => x.Name))}");
            foreach (var mesh in hits)
            {
                w.WriteLine($"{mesh.Name}  (id {mesh.Id}{(hits.Count > 1 ? ", several share this name" : "")})");
                w.WriteLine($"  vertices  {Fmt.Count(mesh.Verts)}, {Fmt.Count(mesh.Tris)} triangles, {mesh.Index} indices, readable {(mesh.Readable ? "yes" : "no")}");
                w.WriteLine("  layout    " + string.Join(", ", mesh.Attrs.Select(a => $"{a.attr} {a.format}x{a.dim}{(a.stream > 0 ? " stream " + a.stream : "")}")));
                for (var s = 0; s < mesh.Sub.Count; s++)
                {
                    var sub = mesh.Sub[s];
                    w.WriteLine($"  submesh {s} {sub.Topology}, {Fmt.Count(sub.IndexCount)} indices, vertices {Fmt.Count(sub.FirstVertex)}..{Fmt.Count(sub.FirstVertex + sub.VertexCount - 1)}{(sub.BaseVertex != 0 ? ", base vertex " + sub.BaseVertex : "")}");
                }
                if (mesh.Center != null)
                    w.WriteLine($"  bounds    center {Fmt.Vec(mesh.Center)} size {Fmt.Vec(mesh.Extents.Select(e => e * 2).ToArray())}");
                if (mesh.Bindposes > 0)
                    w.WriteLine($"  skinning  {mesh.Bindposes} bindposes{(mesh.MaxBonesPerVertex is int b ? ", up to " + b + " bones on a vertex" : "")}");
                if (mesh.Shapes.Count > 0)
                    w.WriteLine("  shapes    " + string.Join(", ", mesh.Shapes.Select(x => x.frames == 1 ? x.name : $"{x.name} ({x.frames} frames)")));
                if (mesh.Lods is int lods && lods > 1)
                    w.WriteLine($"  LODs      {lods}");
                var users = new List<string>();
                var meshIndex = m.Meshes.IndexOf(mesh);
                for (var i = 0; i < m.Nodes.Count; i++)
                    foreach (var c in m.Nodes[i].Components)
                        if ((c.Type is "MeshFilter" or "SkinnedMeshRenderer" or "MeshCollider") && c.Data.Length > 0 && c.Data[0].ValueKind == JsonValueKind.Number && c.Data[0].GetInt32() == meshIndex)
                            users.Add(m.PathOf(i) + " (" + c.Type + ")");
                w.WriteLine("  used by   " + (users.Count == 0 ? "nothing in the model's hierarchy" : string.Join(", ", users)));
            }
            return 0;
        }

        // ------------------------------------------------------------ clips

        public static int Clips(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(2);
            var paths = args.Flag("paths");
            var all = args.Flag("all");
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            if (m.Takes.Count == 0 && m.Clips.Count == 0)
            {
                w.WriteLine(m.ContainsAnimation ? $"the file has animation, but no takes were imported (importAnimation={m.Get("importAnimation")}, animationType={m.Get("animationType")}; a rig error also stops them, see `info`)" : "the file has no animation takes");
                return 0;
            }
            if (args.Positional.Count < 2)
            {
                w.WriteLine($"takes in the file: {string.Join("; ", m.Takes.Select(t => $"{t.Name} {Fmt.Num(t.Start * t.Fps)}-{Fmt.Num(t.Stop * t.Fps)} @{Fmt.Num(t.Fps)}fps"))}");
                w.WriteLine(m.ClipsCustom ? "clips (customized):" : "clips (Unity's one-per-take defaults; nothing customized):");
                foreach (var c in m.Clips)
                {
                    var changed = c.Settings.Where(s => s.Changed && s.Name is not "firstFrame" and not "lastFrame").Select(s => $"{s.Name}={s.Value}").ToList();
                    if (c.Events.Count > 0)
                        changed.Add(Fmt.Plural(c.Events.Count, "event"));
                    if (c.Mask != null)
                        changed.Add("mask " + c.Mask);
                    var anim = m.Anims.FirstOrDefault(a => a.Name == c.Name);
                    w.WriteLine($"  {c.Name}  take {c.Take} frames {c.Get("firstFrame")}-{c.Get("lastFrame")}" + (changed.Count > 0 ? "  " + string.Join(", ", changed) : "") +
                                (anim != null ? $"  -> {Fmt.Seconds(anim.Length)}, {Fmt.Plural(anim.Curves, "curve")}{(anim.Loop ? ", loops" : "")}" : "  -> not produced"));
                }
                if (m.Get("importAnimation") != "true")
                    w.WriteLine("importAnimation is off, so no clips are produced");
                return 0;
            }
            var name = args.Positional[1];
            var clip = m.Clips.FirstOrDefault(c => c.Name == name);
            var result = m.Anims.FirstOrDefault(a => a.Name == name);
            if (clip == null && result == null)
                throw new WaitressException($"no clip '{name}'; clips: {string.Join(", ", m.Clips.Select(c => c.Name).Union(m.Anims.Select(a => a.Name)))}");
            if (clip != null)
            {
                w.WriteLine($"{clip.Name}  (take {clip.Take}; values in parentheses are the take's defaults)");
                var width = clip.Settings.Max(s => s.Name.Length);
                foreach (var s in clip.Settings.Where(s => all || s.Changed || s.Name is "firstFrame" or "lastFrame" or "loopTime"))
                    w.WriteLine($"  {s.Name.PadRight(width)}  {s.Value}{(s.Changed ? "  (" + s.Default + ")" : "")}");
                if (!all)
                    w.WriteLine($"  ({clip.Settings.Count(s => !(s.Changed || s.Name is "firstFrame" or "lastFrame" or "loopTime"))} more at the take's defaults; --all lists them)");
                if (clip.Mask != null)
                    w.WriteLine("  mask " + clip.Mask);
                foreach (var e in clip.Events)
                    w.WriteLine($"  event at {Fmt.Num(e.time * 100)}% ({(result != null ? Fmt.Seconds(e.time * result.Length) : "?")}) calls {e.function}({(string.IsNullOrEmpty(e.text) ? "" : "\"" + e.text + "\"")})");
                if (clip.Curves.Count > 0)
                    w.WriteLine("  curves " + string.Join(", ", clip.Curves));
            }
            if (result != null)
            {
                w.WriteLine($"result: {Fmt.Seconds(result.Length)} at {Fmt.Num(result.Fps)}fps, {(result.Loop ? "loops" : "doesn't loop")}, {(result.Legacy ? "legacy" : result.HumanMotion ? "humanoid" : "generic")}, " +
                            $"{Fmt.Plural(result.Curves, "curve")} on {Fmt.Plural(result.Paths.Count, "path")}, {Fmt.Plural(result.Events, "event")}");
                var root = new List<string>();
                if (result.RootCurves)
                    root.Add("root curves");
                if (result.MotionCurves)
                    root.Add("motion curves");
                if (result.HumanMotion || result.MotionCurves || result.RootCurves)
                    root.Add($"average speed {Fmt.Vec(result.AverageSpeed)} m/s, turn {Fmt.Num(result.AngularSpeed)} rad/s");
                if (root.Count > 0)
                    w.WriteLine("root motion: " + string.Join(", ", root));
                else if (!result.Legacy && !result.HumanMotion)
                    w.WriteLine("root motion: none" + (string.IsNullOrEmpty(m.Get("motionNodeName"))
                        ? " (a Generic rig extracts root motion only from the node named by motionNodeName, which is empty)"
                        : $" (motionNodeName {m.Get("motionNodeName")} has no motion in this clip)"));
                if (paths)
                    foreach (var p in result.Paths)
                        w.WriteLine($"  {(p.path.Length == 0 ? "(root)" : p.path)}: {string.Join(", ", p.props)}");
                else if (result.Paths.Count > 0)
                    w.WriteLine("(--paths lists every animated path and property)");
            }
            else
                w.WriteLine("result: not produced (importAnimation=" + m.Get("importAnimation") + ")");
            return 0;
        }

        // ------------------------------------------------------------ avatar

        public static int Avatar(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            var all = args.Flag("all");
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            w.WriteLine("rig      " + RigLine(m));
            if (m.Get("animationType") == "Human" && m.Get("avatarSetup") != "CopyFromOther")
            {
                var leaves = new HashSet<string>(m.TransformPaths.Select(p => p.Substring(p.LastIndexOf('/') + 1)));
                var missing = s_RequiredHuman.Where(h => !m.HumanMap.Any(x => x.human == h)).ToList();
                if (missing.Count > 0)
                    w.WriteLine("required bones not mapped: " + string.Join(", ", missing));
                var gone = m.HumanMap.Where(x => !leaves.Contains(x.bone)).ToList();
                if (gone.Count > 0)
                    w.WriteLine("mapped to transforms the model doesn't have: " + string.Join(", ", gone.Select(x => $"{x.human}={x.bone}")));
                if (m.RootMotionBone is { Length: > 0 })
                    w.WriteLine("root motion bone: " + m.RootMotionBone);
                var shown = all ? m.HumanMap : m.HumanMap.Where(x => !x.human.Contains(' ')).ToList();
                w.WriteLine($"mapping ({m.HumanMap.Count} bones{(all ? "" : ", fingers hidden; --all shows them")}):");
                foreach (var row in Columns(shown.Select(x => $"{x.human}={x.bone}").ToList(), 3))
                    w.WriteLine("  " + row);
                HumanSettings(m, w, false);
            }
            else if (m.Get("animationType") == "Generic")
                w.WriteLine("root node (motionNodeName): " + Show(m.Get("motionNodeName")));
            return 0;
        }

        static IEnumerable<string> Columns(List<string> items, int columns)
        {
            if (items.Count == 0)
                yield break;
            var width = items.Max(i => i.Length) + 2;
            for (var i = 0; i < items.Count; i += columns)
                yield return string.Concat(items.Skip(i).Take(columns).Select(x => x.PadRight(width))).TrimEnd();
        }

        public static string RemapTarget(string target) =>
            target == null ? "None (remapped to nothing)" : target.StartsWith("missing:", StringComparison.Ordinal) ? $"MISSING asset (guid {target.Substring(8)}, no such asset in the project; the slot gets no material)" : target + " (remap)";

        // ------------------------------------------------------------ materials

        public static int Materials(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            args.RejectUnknown();
            var m = session.One(args.Positional0("a model"));
            w.WriteLine($"import {m.Get("materialImportMode")}, location {m.Get("materialLocation")}" +
                        (m.Get("materialImportMode") == "LegacyImport" ? $", naming {m.Get("materialName")}, search {m.Get("materialSearch")}" : ""));
            if (m.SourceMaterials.Count > 0)
            {
                w.WriteLine("materials in the file:");
                foreach (var name in m.SourceMaterials)
                {
                    var remap = m.Remaps.FirstOrDefault(r => r.type == "Material" && r.name == name);
                    w.WriteLine($"  {name}" + (remap.name != null ? "  -> " + RemapTarget(remap.target) : ""));
                }
            }
            foreach (var r in m.Remaps.Where(r => r.type != "Material" || !m.SourceMaterials.Contains(r.name)))
                w.WriteLine($"  remap {r.type} {r.name} -> {RemapTarget(r.target)}{(r.type == "Material" ? " (the file has no material by that name)" : "")}");
            var slots = Slots(m).ToList();
            if (slots.Count > 0)
            {
                w.WriteLine("renderer slots:");
                foreach (var s in slots)
                    w.WriteLine($"  {s.node}[{s.slot}]  {MaterialName(m, s.material)}");
            }
            return 0;
        }
    }
}
