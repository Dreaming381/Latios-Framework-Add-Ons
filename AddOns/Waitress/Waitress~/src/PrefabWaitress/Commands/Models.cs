using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PrefabWaitress.Model;

namespace PrefabWaitress.Commands
{
    /// <summary>
    /// Has the Editor export the imported hierarchy of every model that scenes and prefabs instance, so objects that come
    /// from .fbx (and other model) files can be read offline like prefab objects.
    /// </summary>
    public static class Models
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            args.RejectExtraPositionals(1);
            var project = session.Project;
            var all = args.Flag("all");
            args.RejectUnknown();

            var guids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var target = args.Positional.Count > 0 ? args.Positional[0] : null;
            var folder = target != null && Directory.Exists(Path.Combine(session.Root, target)) ? Path.Combine(session.Root, target) : target != null && Directory.Exists(target) ? target : null;
            if (target != null && folder == null)
                Collect(session.Asset(target), guids, new HashSet<Asset>());
            else
            {
                var usage = new Usage(project, folder != null ? new[] { folder } : new[] { Path.Combine(session.Root, "Assets"), Path.Combine(session.Root, "Packages") });
                foreach (var set in usage.Sources.Values)
                    foreach (var g in set)
                        guids.Add(g);
            }

            var models = guids.Select(g => project.Index.PathOf(g)).Where(p => p != null && Asset.IsModelPath(p)).Distinct().OrderBy(p => p).ToList();
            var stale = models.Where(p => all || !project.Models.IsCurrent(new Asset { Path = p, Guid = project.Index.GuidOf(p) })).ToList();
            if (stale.Count == 0)
            {
                w.WriteLine(models.Count == 0 ? "no models are instanced there" : $"all {Describe.Plural(models.Count, "model")} already exported (--all re-exports)");
                return 0;
            }
            Directory.CreateDirectory(project.Models.Folder);
            var written = 0;
            var failed = new List<string>();
            foreach (var batch in stale.Chunk(40))
            {
                var items = new List<string>();
                foreach (var p in batch)
                {
                    items.Add(p);
                    items.Add(project.Models.FileFor(project.Index.GuidOf(p)).Replace('\\', '/'));
                    items.Add("# stamp " + project.Models.Stamp(p));
                }
                var result = EditorScript.Call(session.Root, "ExportModels", JsonSerializer.Serialize(items));
                written += result.GetProperty("written").GetInt32();
                failed.AddRange(result.GetProperty("failed").EnumerateArray().Select(e => e.GetString()));
            }
            w.WriteLine($"exported {Describe.Plural(written, "model")} to {Path.GetRelativePath(session.Root, project.Models.Folder)} ({models.Count - stale.Count} already current)");
            foreach (var f in failed)
                w.WriteLine($"  {f}: the Editor found no objects in it");
            return 0;
        }

        static void Collect(Asset a, HashSet<string> guids, HashSet<Asset> seen)
        {
            if (!seen.Add(a))
                return;
            foreach (var layer in a.Layers)
            {
                if (layer.SourceGuid != null)
                    guids.Add(layer.SourceGuid);
                if (layer.Source != null && layer.Source.Readable)
                    Collect(layer.Source, guids, seen);
            }
        }
    }
}
