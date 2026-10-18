using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TerrainWaitress.Model
{
    public sealed class TerrainFile
    {
        public string AssetPath;
        public string FullPath;
        public string Guid;
        public override string ToString() => AssetPath;
    }

    /// <summary>
    /// The project's TerrainData assets and their exported records, and the Terrain components that place them. Records live
    /// in UserSettings/Waitress/TerrainWaitress/Terrains/<guid>.json (+ .bin) and are current while the asset's and its
    /// .meta's write times match the stamp the Editor wrote. Missing or stale records are re-exported through the Editor
    /// when one has the project open.
    /// </summary>
    public sealed class Catalog
    {
        public readonly string Root;
        List<TerrainFile> m_All;

        public Catalog(string root)
        {
            Root = Path.GetFullPath(root);
        }

        public string Folder => Path.Combine(Root, "UserSettings", "Waitress", "TerrainWaitress");
        public string FileFor(string guid) => Path.Combine(Folder, "Terrains", guid + ".json");

        public IReadOnlyList<TerrainFile> All => m_All ??= Discover();

        List<TerrainFile> Discover()
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

            var found = new ConcurrentBag<TerrainFile>();
            var scenes = new ConcurrentBag<(string full, string asset)>();
            Parallel.ForEach(roots.Where(r => Directory.Exists(r.dir)).GroupBy(r => r.asset).Select(g => g.First()), root =>
            {
                foreach (var file in Walk(root.dir))
                {
                    var rel = Path.GetRelativePath(root.dir, file).Replace('\\', '/');
                    if (!file.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!root.dir.Contains("PackageCache"))
                            scenes.Add((file, root.asset + "/" + rel));
                        continue;
                    }
                    var guid = Guid(file + ".meta");
                    if (guid == null)
                        continue;
                    found.Add(new TerrainFile { AssetPath = root.asset + "/" + rel, FullPath = file, Guid = guid });
                }
            });
            m_SceneFiles = scenes.OrderBy(f => f.asset, StringComparer.OrdinalIgnoreCase).ToList();
            return found.OrderBy(f => f.AssetPath, StringComparer.OrdinalIgnoreCase).ToList();
        }

        List<(string full, string asset)> m_SceneFiles;
        List<Placement> m_Placements;
        public readonly List<string> PlacementNotes = new List<string>();

        /// <summary>Every Terrain component in the project's scenes and prefabs, as saved on disk.</summary>
        public IReadOnlyList<Placement> Placements
        {
            get
            {
                if (m_Placements != null)
                    return m_Placements;
                _ = All;
                var found = new ConcurrentBag<(int order, List<Placement> list)>();
                var notes = new ConcurrentBag<string>();
                Parallel.For(0, m_SceneFiles.Count, i =>
                {
                    var local = new List<string>();
                    var list = Model.Placements.Read(m_SceneFiles[i].full, m_SceneFiles[i].asset, local);
                    if (list.Count > 0)
                        found.Add((i, list));
                    foreach (var n in local)
                        notes.Add(n);
                });
                PlacementNotes.AddRange(notes.Distinct().OrderBy(n => n));
                m_Placements = found.OrderBy(f => f.order).SelectMany(f => f.list).ToList();
                return m_Placements;
            }
        }

        public List<Placement> PlacementsOf(string guid) => Placements.Where(p => p.DataGuid == guid).ToList();

        Dictionary<string, string> m_Guids;

        /// <summary>The asset path of a GUID in Assets/ or an embedded package, from the .meta files; null when none.</summary>
        public string AssetPathOf(string guid)
        {
            if (guid == null)
                return null;
            if (m_Guids == null)
            {
                var map = new ConcurrentDictionary<string, string>();
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
                Parallel.ForEach(roots, root =>
                {
                    foreach (var meta in Directory.EnumerateFiles(root.dir, "*.meta", SearchOption.AllDirectories))
                    {
                        try
                        {
                            using var stream = new FileStream(meta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 256);
                            var buffer = new byte[128];
                            var n = stream.Read(buffer, 0, buffer.Length);
                            var head = Encoding.ASCII.GetString(buffer, 0, n);
                            var at = head.IndexOf("guid: ", StringComparison.Ordinal);
                            if (at >= 0 && at + 38 <= head.Length)
                                map[head.Substring(at + 6, 32)] = root.asset + "/" + Path.GetRelativePath(root.dir, meta.Substring(0, meta.Length - 5)).Replace('\\', '/');
                        }
                        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                        {
                        }
                    }
                });
                m_Guids = new Dictionary<string, string>(map);
            }
            return m_Guids.GetValueOrDefault(guid);
        }

        /// <summary>"builtin" when a material uses a shader built into Unity, else the shader's asset path (null if unknown).</summary>
        public string ShaderOf(string materialGuid)
        {
            var path = AssetPathOf(materialGuid);
            if (path == null || !path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
                return null;
            var full = Path.Combine(Root, path);
            if (!File.Exists(full))
                return null;
            foreach (var line in File.ReadLines(full))
            {
                var t = line.Trim();
                if (!t.StartsWith("m_Shader:", StringComparison.Ordinal))
                    continue;
                var at = t.IndexOf("guid: ", StringComparison.Ordinal);
                if (at < 0)
                    return null;
                var guid = t.Substring(at + 6, 32);
                return guid == "0000000000000000f000000000000000" ? "builtin" : AssetPathOf(guid);
            }
            return null;
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
            {
                var ext = Path.GetExtension(f).ToLowerInvariant();
                if (ext is ".asset" or ".unity" or ".prefab")
                    yield return f;
            }
            foreach (var d in dirs)
            {
                var name = Path.GetFileName(d);
                if (name.EndsWith("~", StringComparison.Ordinal) || name.StartsWith(".", StringComparison.Ordinal))
                    continue;
                foreach (var f in Walk(d))
                    yield return f;
            }
        }

        // TerrainData is the main object of its .asset (class 156, file id 15600000); Unity always writes it in binary.
        static string Guid(string meta)
        {
            try
            {
                if (!File.Exists(meta))
                    return null;
                var text = File.ReadAllText(meta);
                if (text.IndexOf("mainObjectFileID: 15600000", StringComparison.Ordinal) < 0)
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

        /// <summary>An asset named by a path (relative to the working directory or project), an asset path, a GUID, or a unique file name; null when none.</summary>
        public TerrainFile TryAsset(string what)
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
                throw new WaitressException($"{what} is not a TerrainData asset");
            }
            var byPath = All.FirstOrDefault(f => string.Equals(f.AssetPath, norm, StringComparison.OrdinalIgnoreCase) || f.Guid == norm);
            if (byPath != null)
                return byPath;
            var byName = All.Where(f => f.AssetPath.EndsWith("/" + norm, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byName.Count == 0 && !Path.HasExtension(norm))
                byName = All.Where(f => f.AssetPath.EndsWith("/" + norm + Path.GetExtension(f.AssetPath), StringComparison.OrdinalIgnoreCase)).ToList();
            if (byName.Count == 1)
                return byName[0];
            if (byName.Count > 1)
                throw new WaitressException($"'{what}' names {byName.Count} files; use more of the path:\n  " + string.Join("\n  ", byName.Take(10).Select(f => f.AssetPath)));
            return null;
        }

        /// <summary>True when <paramref name="what"/> names a folder; <paramref name="files"/> gets the TerrainData assets under it.</summary>
        public bool TryFolder(string what, out List<TerrainFile> files)
        {
            files = null;
            string full = null;
            foreach (var candidate in new[] { what, Path.Combine(Root, what) })
                if (Directory.Exists(candidate))
                {
                    full = Path.GetFullPath(candidate).TrimEnd('\\', '/');
                    break;
                }
            var norm = what.Replace('\\', '/').TrimEnd('/');
            if (full != null)
                files = All.Where(f => Path.GetFullPath(f.FullPath).StartsWith(full + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)).ToList();
            else if (All.Any(f => f.AssetPath.StartsWith(norm + "/", StringComparison.OrdinalIgnoreCase)))
                files = All.Where(f => f.AssetPath.StartsWith(norm + "/", StringComparison.OrdinalIgnoreCase)).ToList();
            if (files != null && full != null && files.Count == 0)
                files = All.Where(f => f.AssetPath.StartsWith(norm + "/", StringComparison.OrdinalIgnoreCase)).ToList();
            return files != null;
        }

        public static string Stamp(string fullPath)
        {
            var meta = fullPath + ".meta";
            return File.GetLastWriteTimeUtc(fullPath).Ticks.ToString(CultureInfo.InvariantCulture) + " " +
                   (File.Exists(meta) ? File.GetLastWriteTimeUtc(meta).Ticks : 0).ToString(CultureInfo.InvariantCulture);
        }

        public bool IsCurrent(TerrainFile f)
        {
            Cached(f, out var current);
            return current;
        }

        TerrainRecord Cached(TerrainFile f, out bool current)
        {
            current = false;
            var file = FileFor(f.Guid);
            if (!File.Exists(file))
                return null;
            TerrainRecord r;
            try
            {
                r = TerrainRecord.Load(File.ReadAllText(file), Path.ChangeExtension(file, ".bin"));
            }
            catch (Exception e) when (e is JsonException or KeyNotFoundException or InvalidOperationException or IOException or ArgumentException)
            {
                return null;
            }
            current = r.Stamp == Stamp(f.FullPath) && r.Path == f.AssetPath;
            return r;
        }

        /// <summary>
        /// Records for the assets, exporting the missing and stale ones first when an Editor is reachable. Without one, a
        /// stale record is returned marked Stale, and an asset with no record at all fails.
        /// </summary>
        public List<TerrainRecord> Get(IReadOnlyList<TerrainFile> files, TextWriter notes, bool force = false)
        {
            var records = new Dictionary<TerrainFile, TerrainRecord>();
            var needed = new List<TerrainFile>();
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
                        throw new WaitressException((files.Count == 1 ? files[0].AssetPath + " hasn't" : $"{missing.Count} TerrainData assets haven't") +
                                                    $" been exported yet, and no Editor has the project open. Open it with `unity open \"{Root}\"` (exporting needs it once per change)");
                    foreach (var f in needed.Where(records.ContainsKey))
                        records[f].Stale = true;
                    var stale = needed.Count(records.ContainsKey);
                    if (stale > 0)
                        notes.WriteLine($"note: {(stale == 1 ? needed.First(records.ContainsKey).AssetPath + " changed" : stale + " TerrainData assets changed")} since export and no Editor is open to refresh; showing the last export");
                    if (missing.Count > 0)
                        notes.WriteLine($"note: skipped {missing.Count} never-exported file(s); open the Editor to include them");
                }
            }
            foreach (var pair in records)
                pair.Value.FullPath = pair.Key.FullPath;
            return files.Where(records.ContainsKey).Select(f => records[f]).ToList();
        }

        public TerrainRecord Get(TerrainFile file, TextWriter notes, bool force = false) =>
            Get(new[] { file }, notes, force).FirstOrDefault() ?? throw new WaitressException($"{file.AssetPath} couldn't be exported");

        /// <summary>
        /// Deletes records whose GUID no TerrainData in the project has any more (deleted, or re-added without its .meta, or a
        /// file that is no longer a TerrainData). Discovery covers the whole project, so this is safe whatever a command
        /// was asked about.
        /// </summary>
        public int Prune()
        {
            var folder = Path.Combine(Folder, "Terrains");
            if (!Directory.Exists(folder))
                return 0;
            var live = new HashSet<string>(All.Select(f => f.Guid), StringComparer.OrdinalIgnoreCase);
            var removed = 0;
            foreach (var file in Directory.EnumerateFiles(folder).Where(f => f.EndsWith(".json", StringComparison.Ordinal) || f.EndsWith(".bin", StringComparison.Ordinal)))
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

        /// <summary>Has the Editor export the assets; returns the ones it couldn't, with why.</summary>
        public Dictionary<string, string> Export(IReadOnlyList<TerrainFile> files)
        {
            Prune();
            var failed = new Dictionary<string, string>();
            foreach (var batch in files.Chunk(20))
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
            return failed;
        }
    }
}
