using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModelWaitress.Model
{
    public sealed class ModelFile
    {
        public string AssetPath;
        public string FullPath;
        public string Guid;
        public override string ToString() => AssetPath;
    }

    /// <summary>
    /// The project's models and their exported records. Records live in UserSettings/Waitress/ModelWaitress/Models/<guid>.json
    /// and are current while the model's and its .meta's write times match the stamp the Editor wrote. Missing or stale
    /// records are re-exported through the Editor when one has the project open.
    /// </summary>
    public sealed class Catalog
    {
        public static readonly string[] Extensions = { ".fbx", ".obj", ".dae", ".3ds", ".dxf", ".blend", ".max", ".ma", ".mb", ".c4d", ".lxo", ".jas", ".skp" };

        public readonly string Root;
        List<ModelFile> m_All;
        ProjectRecord m_Project;

        public Catalog(string root)
        {
            Root = Path.GetFullPath(root);
        }

        public string Folder => Path.Combine(Root, "UserSettings", "Waitress", "ModelWaitress");
        public string FileFor(string guid) => Path.Combine(Folder, "Models", guid + ".json");

        public IReadOnlyList<ModelFile> All => m_All ??= Discover();

        List<ModelFile> Discover()
        {
            var roots = new List<(string dir, string asset)> { (Path.Combine(Root, "Assets"), "Assets") };
            var packages = Path.Combine(Root, "Packages");
            if (Directory.Exists(packages))
                foreach (var dir in Directory.EnumerateDirectories(packages))
                    roots.Add((dir, "Packages/" + (PackageName(dir) ?? Path.GetFileName(dir))));
            var cache = Path.Combine(Root, "Library", "PackageCache");
            if (Directory.Exists(cache))
                foreach (var dir in Directory.EnumerateDirectories(cache))
                {
                    var name = Path.GetFileName(dir);
                    var at = name.IndexOf('@');
                    roots.Add((dir, "Packages/" + (at >= 0 ? name.Substring(0, at) : name)));
                }
            foreach (var dir in LocalPackages())
                roots.Add((dir, "Packages/" + (PackageName(dir) ?? Path.GetFileName(dir))));

            var found = new ConcurrentBag<ModelFile>();
            Parallel.ForEach(roots.Where(r => Directory.Exists(r.dir)).GroupBy(r => r.asset).Select(g => g.First()), root =>
            {
                foreach (var file in Walk(root.dir))
                {
                    var guid = ModelGuid(file + ".meta");
                    if (guid == null)
                        continue;
                    var rel = Path.GetRelativePath(root.dir, file).Replace('\\', '/');
                    found.Add(new ModelFile { AssetPath = root.asset + "/" + rel, FullPath = file, Guid = guid });
                }
            });
            return found.OrderBy(f => f.AssetPath, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // Unity skips folders ending in ~ and hidden ones, so their files are never imported.
        static IEnumerable<string> Walk(string dir)
        {
            IEnumerable<string> files, dirs;
            try
            {
                files = Directory.EnumerateFiles(dir).ToList();
                dirs = Directory.EnumerateDirectories(dir).ToList();
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                yield break;
            }
            foreach (var f in files)
                if (Extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    yield return f;
            foreach (var d in dirs)
            {
                var name = Path.GetFileName(d);
                if (name.EndsWith("~", StringComparison.Ordinal) || name.StartsWith(".", StringComparison.Ordinal))
                    continue;
                foreach (var f in Walk(d))
                    yield return f;
            }
        }

        static string ModelGuid(string meta)
        {
            try
            {
                if (!File.Exists(meta))
                    return null;
                using var stream = new FileStream(meta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024);
                var buffer = new byte[512];
                var n = stream.Read(buffer, 0, buffer.Length);
                var text = Encoding.ASCII.GetString(buffer, 0, n);
                if (text.IndexOf("\nModelImporter:", StringComparison.Ordinal) < 0 && text.IndexOf("\nSketchUpImporter:", StringComparison.Ordinal) < 0)
                    return null;
                var at = text.IndexOf("guid: ", StringComparison.Ordinal);
                return at < 0 || at + 38 > text.Length ? null : text.Substring(at + 6, 32);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                return null;
            }
        }

        IEnumerable<string> LocalPackages()
        {
            var manifest = Path.Combine(Root, "Packages", "manifest.json");
            if (!File.Exists(manifest))
                yield break;
            foreach (var line in File.ReadLines(manifest))
            {
                var at = line.IndexOf("\"file:", StringComparison.Ordinal);
                if (at < 0)
                    continue;
                var end = line.IndexOf('"', at + 6);
                if (end < 0)
                    continue;
                var full = Path.GetFullPath(Path.Combine(Root, "Packages", line.Substring(at + 6, end - at - 6)));
                if (Directory.Exists(full) && !full.StartsWith(Root, StringComparison.OrdinalIgnoreCase))
                    yield return full;
            }
        }

        static string PackageName(string dir)
        {
            var json = Path.Combine(dir, "package.json");
            if (!File.Exists(json))
                return null;
            try
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(json));
                return doc.RootElement.TryGetProperty("name", out var n) ? n.GetString() : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>A model named by a path (relative to the working directory or project), an asset path, a GUID, or a unique file name.</summary>
        public ModelFile Resolve(string what)
        {
            var norm = what.Replace('\\', '/').TrimEnd('/');
            foreach (var candidate in new[] { what, Path.Combine(Root, what) })
            {
                if (!File.Exists(candidate))
                    continue;
                var full = Path.GetFullPath(candidate);
                var hit = All.FirstOrDefault(f => string.Equals(Path.GetFullPath(f.FullPath), full, StringComparison.OrdinalIgnoreCase));
                if (hit != null)
                    return hit;
                throw new WaitressException($"{what} is not a model the project imports (no ModelImporter in its .meta)");
            }
            var byPath = All.FirstOrDefault(f => string.Equals(f.AssetPath, norm, StringComparison.OrdinalIgnoreCase) || f.Guid == norm);
            if (byPath != null)
                return byPath;
            var byName = All.Where(f => f.AssetPath.EndsWith("/" + norm, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byName.Count == 0 && !Path.HasExtension(norm))
                byName = All.Where(f => string.Equals(Path.GetFileNameWithoutExtension(f.AssetPath), Path.GetFileName(norm), StringComparison.OrdinalIgnoreCase)
                                        && f.AssetPath.EndsWith(norm + Path.GetExtension(f.AssetPath), StringComparison.OrdinalIgnoreCase)).ToList();
            if (byName.Count == 1)
                return byName[0];
            if (byName.Count > 1)
                throw new WaitressException($"'{what}' names {byName.Count} models; use more of the path:\n  " + string.Join("\n  ", byName.Take(10).Select(f => f.AssetPath)));
            throw new WaitressException($"no model '{what}' (a model file path, an asset path such as Assets/Art/Hero.fbx, or a unique file name)");
        }

        /// <summary>True when <paramref name="what"/> names a folder; <paramref name="models"/> gets the models under it.</summary>
        public bool TryFolder(string what, out List<ModelFile> models)
        {
            models = null;
            string full = null;
            foreach (var candidate in new[] { what, Path.Combine(Root, what) })
                if (Directory.Exists(candidate))
                {
                    full = Path.GetFullPath(candidate).TrimEnd('\\', '/');
                    break;
                }
            var norm = what.Replace('\\', '/').TrimEnd('/');
            if (full != null)
                models = All.Where(f => Path.GetFullPath(f.FullPath).StartsWith(full + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)).ToList();
            else if (All.Any(f => f.AssetPath.StartsWith(norm + "/", StringComparison.OrdinalIgnoreCase)))
                models = All.Where(f => f.AssetPath.StartsWith(norm + "/", StringComparison.OrdinalIgnoreCase)).ToList();
            if (models != null && full != null && models.Count == 0)
                models = All.Where(f => f.AssetPath.StartsWith(norm + "/", StringComparison.OrdinalIgnoreCase)).ToList();
            return models != null;
        }

        /// <summary>Models named by each argument: a folder gives every model under it.</summary>
        public List<ModelFile> Expand(IEnumerable<string> targets)
        {
            var list = new List<ModelFile>();
            foreach (var t in targets)
            {
                if (TryFolder(t, out var under))
                {
                    if (under.Count == 0)
                        throw new WaitressException($"no models under {t}");
                    list.AddRange(under);
                }
                else
                    list.Add(Resolve(t));
            }
            return list.Distinct().ToList();
        }

        public static string Stamp(string fullPath)
        {
            var meta = fullPath + ".meta";
            return File.GetLastWriteTimeUtc(fullPath).Ticks.ToString(CultureInfo.InvariantCulture) + " " +
                   (File.Exists(meta) ? File.GetLastWriteTimeUtc(meta).Ticks : 0).ToString(CultureInfo.InvariantCulture);
        }

        public bool IsCurrent(ModelFile f)
        {
            Cached(f, out var current);
            return current;
        }

        ModelRecord Cached(ModelFile f, out bool current)
        {
            current = false;
            var file = FileFor(f.Guid);
            if (!File.Exists(file))
                return null;
            ModelRecord r;
            try
            {
                r = ModelRecord.Parse(File.ReadAllText(file));
            }
            catch (JsonException)
            {
                return null;
            }
            current = r.Stamp == Stamp(f.FullPath) && r.Path == f.AssetPath
                      && r.Deps.All(d => File.Exists(d.path) ? d.stamp == Stamp(d.path) : d.stamp == "missing");
            return r;
        }

        /// <summary>
        /// Records for the models, exporting the missing and stale ones first when an Editor is reachable. Without one, a
        /// stale record is returned marked Stale, and a model with no record at all fails.
        /// </summary>
        public List<ModelRecord> Get(IReadOnlyList<ModelFile> files, TextWriter notes, bool force = false)
        {
            var records = new Dictionary<ModelFile, ModelRecord>();
            var needed = new List<ModelFile>();
            foreach (var f in files)
            {
                var r = Cached(f, out var current);
                if (r != null)
                    records[f] = r;
                if (force || !current)
                    needed.Add(f);
            }
            if (needed.Count > 0)
            {
                try
                {
                    var failed = Export(needed);
                    foreach (var f in needed)
                    {
                        if (failed.TryGetValue(f.AssetPath, out var why))
                        {
                            notes.WriteLine($"note: the Editor couldn't export {f.AssetPath}: {why}");
                            records.Remove(f);
                            continue;
                        }
                        var r = Cached(f, out var current);
                        if (r != null)
                        {
                            r.Stale = !current;
                            records[f] = r;
                        }
                    }
                }
                catch (EditorUnavailable)
                {
                    var missing = needed.Where(f => !records.ContainsKey(f)).ToList();
                    if (missing.Count == files.Count)
                        throw new WaitressException((files.Count == 1 ? files[0].AssetPath + " hasn't" : $"{missing.Count} models haven't") +
                                                    $" been exported yet, and no Editor has the project open. Open it with `unity open \"{Root}\"` (exporting needs it once per model change)");
                    foreach (var f in needed.Where(records.ContainsKey))
                        records[f].Stale = true;
                    var stale = needed.Count(records.ContainsKey);
                    if (stale > 0)
                        notes.WriteLine($"note: {(stale == 1 ? needed.First(records.ContainsKey).AssetPath + " changed" : stale + " models changed")} since export and no Editor is open to refresh; showing the last export");
                    if (missing.Count > 0)
                        notes.WriteLine($"note: skipped {missing.Count} never-exported model(s); open the Editor to include them");
                }
            }
            foreach (var pair in records)
                pair.Value.FullPath = pair.Key.FullPath;
            return files.Where(records.ContainsKey).Select(f => records[f]).ToList();
        }

        public ModelRecord Get(ModelFile file, TextWriter notes, bool force = false) =>
            Get(new[] { file }, notes, force).FirstOrDefault() ?? throw new WaitressException($"{file.AssetPath} couldn't be exported");

        /// <summary>Has the Editor export the models; returns the ones it couldn't, with why.</summary>
        /// <summary>
        /// Deletes records whose GUID no model in the project has any more (the model was deleted, or re-added without
        /// its .meta). Discovery always covers the whole project, so this is safe whatever a command was asked about.
        /// </summary>
        public int Prune()
        {
            var folder = Path.Combine(Folder, "Models");
            if (!Directory.Exists(folder))
                return 0;
            var live = new HashSet<string>(All.Select(f => f.Guid), StringComparer.OrdinalIgnoreCase);
            var removed = 0;
            foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
            {
                if (live.Contains(Path.GetFileNameWithoutExtension(file)))
                    continue;
                try
                {
                    File.Delete(file);
                    removed++;
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                {
                }
            }
            return removed;
        }

        public Dictionary<string, string> Export(IReadOnlyList<ModelFile> files)
        {
            Prune();
            var failed = new Dictionary<string, string>();
            foreach (var batch in files.Chunk(50))
            {
                var request = JsonSerializer.Serialize(new { paths = batch.Select(f => f.AssetPath).ToArray(), @out = Folder.Replace('\\', '/') });
                var result = EditorScript.Call(Root, "Export", request);
                foreach (var e in result.GetProperty("failed").EnumerateArray())
                {
                    var text = e.GetString();
                    var colon = text.IndexOf(": ", StringComparison.Ordinal);
                    failed[colon < 0 ? text : text.Substring(0, colon)] = colon < 0 ? "" : text.Substring(colon + 2);
                }
            }
            m_Project = null;
            return failed;
        }

        public ProjectRecord Project
        {
            get
            {
                if (m_Project != null)
                    return m_Project;
                var file = Path.Combine(Folder, "project.json");
                return m_Project = File.Exists(file) ? ProjectRecord.Parse(File.ReadAllText(file)) : new ProjectRecord();
            }
        }
    }
}
