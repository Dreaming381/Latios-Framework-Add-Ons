using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PrefabWaitress.Model;

namespace PrefabWaitress.Commands
{
    /// <summary>
    /// Edits through the Editor: set values (an override when the object comes from a prefab), revert overrides, apply them
    /// to a prefab, and remove unused or redundant ones. Unity writes the files; the tool re-reads them to report the result.
    /// </summary>
    public static class Edit
    {
        public static int Set(Session session, Args args, TextWriter w)
        {
            var asset = session.Asset(args.Positional0("scene or prefab"));
            var project = session.Project;
            var go = Describe.FindOne(asset, args.PositionalAt(1, "object"));
            if (args.Positional.Count < 3)
                throw new WaitressException("missing Component.property=value (several may follow, e.g. Transform.m_LocalPosition=0,1,0 DoorScenario.speed=2)");
            var ops = new List<Dictionary<string, object>>();
            var touched = new List<(Obj comp, string path)>();
            foreach (var pair in args.Positional.Skip(2))
            {
                var eq = pair.IndexOf('=');
                if (eq <= 0)
                    throw new WaitressException($"'{pair}' is not Component.property=value");
                var (comp, path) = Describe.FindProperty(project, go, pair.Substring(0, eq));
                var value = pair.Substring(eq + 1);
                var op = Op("set", go, comp);
                op["property"] = path;
                if (value.StartsWith("@", StringComparison.Ordinal))
                {
                    var spec = value.Substring(1);
                    var colon = spec.LastIndexOf(':');
                    var target = Describe.FindOne(asset, colon > 0 ? spec.Substring(0, colon) : spec);
                    var targetComp = colon > 0 ? Describe.FindComponent(project, target, spec.Substring(colon + 1)) : target;
                    op["hasRef"] = true;
                    op["refPath"] = IndexPath(target);
                    op["refComponent"] = targetComp == target ? -1 : target.Components.IndexOf(targetComp);
                }
                else
                    op["value"] = value;
                ops.Add(op);
                touched.Add((comp, path));
            }
            var saveOpen = args.Flag("save-open");
            args.RejectUnknown();
            var done = Send(session, asset, ops, saveOpen);
            Report(session, asset, go, touched, done, w);
            return 0;
        }

        public static int Revert(Session session, Args args, TextWriter w) => RevertOrApply(session, args, w, "revert");

        public static int Apply(Session session, Args args, TextWriter w) => RevertOrApply(session, args, w, "apply");

        static int RevertOrApply(Session session, Args args, TextWriter w, string verb)
        {
            args.RejectExtraPositionals(3);
            var asset = session.Asset(args.Positional0("scene or prefab"));
            var project = session.Project;
            var go = Describe.FindOne(asset, args.PositionalAt(1, "object"));
            var op = Op(verb, go, go);
            var spec = args.Positional.Count > 2 ? args.Positional[2] : null;
            Obj comp = go;
            string path = null;
            var removedType = args.Get("removed");
            if (removedType != null)
            {
                if (spec != null)
                    throw new WaitressException("--removed names the component; leave out the component argument");
                op["removedType"] = removedType;
            }
            else if (spec != null && (spec.Contains('.') || spec.Contains('[')))
            {
                (comp, path) = Describe.FindProperty(project, go, spec);
                op = Op(verb, go, comp);
                op["property"] = path;
            }
            else if (spec != null)
            {
                comp = Describe.FindComponent(project, go, spec);
                op = Op(verb, go, comp);
            }
            if (go.Source == null && go.Opaque == false && !(go.Parent?.Source != null) && path == null && spec == null)
                throw new WaitressException($"{Hierarchy.PathOf(go)} is not part of a prefab instance, so it has no overrides");
            op["all"] = args.Flag("all");
            var to = args.Get("to");
            if (to != null && verb != "apply")
                throw new WaitressException("--to is for apply");
            if (verb == "apply")
            {
                // The prefabs that can receive it: the object's own sources, or for something this file added, its parent's.
                var first = comp.Source ?? go.Source ?? go.Parent?.Source;
                if (first == null)
                    throw new WaitressException($"{Hierarchy.PathOf(go)} isn't part of a prefab instance, so there is no prefab to apply to");
                var chain = new List<Asset>();
                for (var s = first; s != null; s = s.Source)
                    if (!chain.Contains(s.Owner))
                        chain.Add(s.Owner);
                // Unity's own default is the innermost prefab; the nearest one is the safe default. A bare file name is enough.
                var target = to == null ? chain[0] : chain.FirstOrDefault(c => string.Equals(c.FileName, to, StringComparison.OrdinalIgnoreCase) || string.Equals(c.Path, to.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase));
                if (target == null)
                    throw new WaitressException($"{to} isn't one of this object's prefabs: " + string.Join(", ", chain.Select(c => c.Path)));
                op["to"] = target.Path;
                if (path != null && comp.Source != null)
                {
                    // Unity clears this property's overrides in the prefabs between this file and the one receiving it.
                    var between = new List<string>();
                    for (var s = comp.Source; s != null && s.Owner != target; s = s.Source)
                        if (s.SetBy.Any(e => (Fields.Covers(path, e.Key) || Fields.Covers(e.Key, path)) && e.Value.Layer.Owner == s.Owner))
                            between.Add(s.Owner.FileName);
                    if (between.Count > 0)
                        w.WriteLine($"note: applying to {target.FileName} also clears this property's overrides in {string.Join(", ", between)}");
                }
            }
            var saveOpen = args.Flag("save-open");
            args.RejectUnknown();
            var done = Send(session, asset, new List<Dictionary<string, object>> { op }, saveOpen);
            if (path != null)
            {
                Report(session, asset, go, new List<(Obj, string)> { (comp, path) }, done, w);
                if (verb == "apply" && comp.Source != null)
                {
                    // Where the applied value now goes: the receiving prefab's instances, as `instances --prop` reports them.
                    var receiver = op.TryGetValue("to", out var t) ? (string)t : comp.Source.Owner.Path;
                    var o = comp.Source;
                    while (o != null && o.Owner.Path != receiver)
                        o = o.Source;
                    if (o?.GameObject != null)
                    {
                        var name = o.IsGameObject ? "GameObject" : Describe.ComponentNames(session.Project, o.GameObject).First(n => n.comp == o).name;
                        var argv = new List<string> { receiver, Hierarchy.PathOf(o.GameObject), "--prop", name + "." + Describe.Short(path) };
                        Instances.Run(new Session(receiver), Args.Parse(argv), w);
                    }
                }
            }
            else
            {
                foreach (var d in done)
                    w.WriteLine(d);
                var fresh = new Session(asset.Path).Asset(asset.Path);
                w.WriteLine(Overrides.Summary(fresh, fresh.Layers));
            }
            return 0;
        }

        public static int Clean(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            var asset = session.Asset(args.Positional0("scene or prefab"));
            var redundant = args.Flag("redundant");
            var dry = args.Flag("dry-run");
            var ops = new List<Dictionary<string, object>>();
            var listed = new List<string>();
            foreach (var layer in asset.Layers)
            {
                var drop = layer.Mods.Where(m => Overrides.IsUnused(m) || redundant && m.Status == ModStatus.Redundant && !Overrides.IsDefault(m)).ToList();
                if (drop.Count == 0)
                    continue;
                if (layer.Root == null)
                    throw new WaitressException($"the instance at line {layer.Line} has no root object this tool can find; clean it in the Editor");
                var op = Op("dropmods", layer.Root, layer.Root);
                op["modTargets"] = drop.Select(m => (m.Status == ModStatus.NoTarget ? 0L : m.Target.FileId).ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray();
                op["modPaths"] = drop.Select(m => m.Path).ToArray();
                op["modKinds"] = drop.Select(m => Overrides.IsUnused(m) ? "unused" : "redundant").ToArray();
                ops.Add(op);
                foreach (var m in drop)
                    listed.Add($"{Hierarchy.PathOf(layer.Root)}: {(m.TargetObj != null ? Overrides.Label(session.Project, layer, m.TargetObj) : "")}{Describe.Short(m.Path)} ({(Overrides.IsUnused(m) ? m.Detail : "redundant")})");
            }
            var saveOpen = args.Flag("save-open");
            args.RejectUnknown();
            if (ops.Count == 0)
            {
                w.WriteLine($"{asset.Path}: nothing to clean ({(redundant ? "no unused or redundant overrides" : "no unused overrides; --redundant also removes overrides equal to the prefab")})");
                return 0;
            }
            foreach (var l in listed)
                w.WriteLine((dry ? "would remove " : "remove ") + l);
            if (dry)
                return 0;
            var done = Send(session, asset, ops, saveOpen);
            foreach (var d in done)
                w.WriteLine(d);
            var fresh = new Session(asset.Path).Asset(asset.Path);
            w.WriteLine(Overrides.Summary(fresh, fresh.Layers));
            return 0;
        }

        static Dictionary<string, object> Op(string verb, Obj go, Obj comp)
        {
            var op = new Dictionary<string, object>
            {
                ["op"] = verb,
                ["path"] = IndexPath(go),
                ["names"] = NamePath(go),
                ["component"] = comp == go ? -1 : go.Components.IndexOf(comp),
            };
            return op;
        }

        /// <summary>Child indices from the root: the first is the root's index among the file's roots.</summary>
        static int[] IndexPath(Obj go)
        {
            var path = new List<int>();
            for (var o = go; o != null; o = o.Parent)
            {
                if (o.Parent == null && o.Transform?.Father != null)
                    throw new WaitressException($"{Hierarchy.PathOf(go)} sits under an object from a model that hasn't been exported; run `prefabwaitress models {go.Owner.Path}` first");
                path.Add(o.Parent != null ? o.Parent.Children.IndexOf(o) : o.Owner.Roots.IndexOf(o));
            }
            path.Reverse();
            return path.ToArray();
        }

        static Obj ByIndex(Asset asset, int[] path)
        {
            var o = path[0] < asset.Roots.Count ? asset.Roots[path[0]] : null;
            for (var i = 1; o != null && i < path.Length; i++)
                o = path[i] < o.Children.Count ? o.Children[path[i]] : null;
            return o ?? throw new WaitressException("the edit was saved, but the object can't be found in the file afterwards; run `tree` to see it");
        }

        static string[] NamePath(Obj go)
        {
            var names = new List<string>();
            for (var o = go; o != null; o = o.Parent)
                names.Add(o.Name);
            names.Reverse();
            return names.ToArray();
        }

        static List<string> Send(Session session, Asset asset, List<Dictionary<string, object>> ops, bool saveOpen)
        {
            if (asset.Kind == AssetKind.Model)
                throw new WaitressException("models can't be edited; edit a prefab or scene that instances it");
            var request = new Dictionary<string, object>
            {
                ["file"] = asset.Path,
                ["saveOpen"] = saveOpen,
                ["ops"] = ops,
            };
            var json = JsonSerializer.Serialize(request);
            var result = EditorScript.Call(session.Root, "Edit", json);
            var done = new List<string>();
            if (result.TryGetProperty("done", out var list))
                foreach (var d in list.EnumerateArray())
                    done.Add(d.GetString());
            return done;
        }

        /// <summary>Re-reads the file and prints each touched property as it now stands.</summary>
        static void Report(Session session, Asset before, Obj go, List<(Obj comp, string path)> touched, List<string> done, TextWriter w)
        {
            var fresh = new Session(before.Path);
            var asset = fresh.Asset(before.Path);
            var project = fresh.Project;
            // By position, since a set may have renamed it.
            var now = ByIndex(asset, IndexPath(go));
            foreach (var (comp, path) in touched)
            {
                var index = comp == go ? -1 : go.Components.IndexOf(comp);
                var c = index < 0 ? now : now.Components[index];
                var line = Fields.Lines(c).FirstOrDefault(l => Fields.Covers(l.Path, path) || Fields.Covers(path, l.Path)) ?? new FieldLine { Path = path };
                if (line.Node == null)
                {
                    w.WriteLine($"{Hierarchy.PathOf(now)} {Describe.TypeOf(project, c)}.{Describe.Short(path)}: saved, but this tool can't read it back (SerializeReference or packed data)");
                    continue;
                }
                w.WriteLine($"{Hierarchy.PathOf(now)} {Describe.TypeOf(project, c)}.{Show.Line(project, asset, c, line)}");
            }
        }
    }
}
