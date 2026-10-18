using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>Loads every scene and prefab under a folder and counts what resolved (hidden command, for testing).</summary>
    public static class Stats
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            var sw = Stopwatch.StartNew();
            var project = session.Project;
            var indexMs = sw.ElapsedMilliseconds;
            var folder = Path.Combine(session.Root, args.Positional.Count > 0 ? args.Positional[0] : "Assets");
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)).ToList();
            int objects = 0, layers = 0, errors = 0, strippedMismatch = 0;
            var byStatus = new int[Enum.GetValues(typeof(ModStatus)).Length];
            var verbose = args.Flag("verbose");
            foreach (var f in files)
            {
                Asset a;
                try
                {
                    a = project.Load(f);
                }
                catch (Exception e)
                {
                    errors++;
                    w.WriteLine($"error {f}: {e.Message}");
                    continue;
                }
                objects += a.Objects.Count;
                layers += a.Layers.Count;
                foreach (var l in a.Layers)
                foreach (var m in l.Mods)
                {
                    byStatus[(int)m.Status]++;
                    if (verbose && m.Status is ModStatus.NoProperty or ModStatus.NoTarget)
                        w.WriteLine($"  {a.Path}: {m.Status} {m.Path} {m.Detail}");
                }
                foreach (var d in a.Docs.Where(d => d.Stripped))
                {
                    var layerId = d.Body["m_PrefabInstance"].AsRef().FileId;
                    var layer = a.Layers.FirstOrDefault(l => l.Id == layerId);
                    if (layer?.Source == null || !layer.Source.Readable)
                        continue;
                    var src = d.Body["m_CorrespondingSourceObject"].AsRef().FileId;
                    if (a.Get(d.FileId)?.Source == null)
                    {
                        strippedMismatch++;
                        if (verbose)
                            w.WriteLine($"  {a.Path}: stripped &{d.FileId} != instanced id of {src}");
                    }
                }
            }
            w.WriteLine($"{files.Count} files, {objects} objects, {layers} prefab instances, index {indexMs} ms, total {sw.ElapsedMilliseconds} ms, {errors} errors, {strippedMismatch} stripped id mismatches");
            w.WriteLine("overrides: " + string.Join(", ", Enum.GetNames(typeof(ModStatus)).Select((n, i) => $"{n} {byStatus[i]}")));
            return 0;
        }
    }
}
