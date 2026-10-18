using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClipWaitress.Model
{
    public sealed class ClipFile
    {
        public string AssetPath;
        public string FullPath;
        public string Guid;
        public bool IsModel;
        public override string ToString() => AssetPath;
    }

    /// <summary>
    /// The project's clip assets (.anim files and models that import animation) and their exported records. Records live in
    /// UserSettings/Waitress/ClipWaitress/Clips/<guid>.json and are current while the asset's and its .meta's write times
    /// match the stamp the Editor wrote. Missing or stale records are re-exported through the Editor when one has the
    /// project open.
    /// </summary>
    public sealed class Catalog
    {
        public static readonly string[] ModelExtensions = { ".fbx", ".obj", ".dae", ".3ds", ".dxf", ".blend", ".max", ".ma", ".mb", ".c4d", ".lxo", ".jas" };

        public readonly string Root;
        List<ClipFile> m_All;

        public Catalog(string root)
        {
            Root = Path.GetFullPath(root);
        }

        public string Folder => Path.Combine(Root, "UserSettings", "Waitress", "ClipWaitress");
        public string FileFor(string guid) => Path.Combine(Folder, "Clips", guid + ".json");

        public IReadOnlyList<ClipFile> All => m_All ??= Discover();

        List<ClipFile> Discover()
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

            var found = new ConcurrentBag<ClipFile>();
            Parallel.ForEach(roots.Where(r => Directory.Exists(r.dir)).GroupBy(r => r.asset).Select(g => g.First()), root =>
            {
                foreach (var file in Walk(root.dir))
                {
                    var model = !file.EndsWith(".anim", StringComparison.OrdinalIgnoreCase);
                    var guid = Guid(file + ".meta", model);
                    if (guid == null)
                        continue;
                    var rel = Path.GetRelativePath(root.dir, file).Replace('\\', '/');
                    found.Add(new ClipFile { AssetPath = root.asset + "/" + rel, FullPath = file, Guid = guid, IsModel = model });
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
            {
                var ext = Path.GetExtension(f).ToLowerInvariant();
                if (ext == ".anim" || ModelExtensions.Contains(ext))
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

        // A model counts only while its importer imports animation.
        static string Guid(string meta, bool model)
        {
            try
            {
                if (!File.Exists(meta))
                    return null;
                var text = model ? File.ReadAllText(meta) : ReadHead(meta);
                if (model && (text.IndexOf("\nModelImporter:", StringComparison.Ordinal) < 0 || text.IndexOf("importAnimation: 0", StringComparison.Ordinal) >= 0))
                    return null;
                var at = text.IndexOf("guid: ", StringComparison.Ordinal);
                return at < 0 || at + 38 > text.Length ? null : text.Substring(at + 6, 32);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                return null;
            }
        }

        static string ReadHead(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024);
            var buffer = new byte[256];
            var n = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, n);
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
        public ClipFile TryAsset(string what)
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
                throw new WaitressException($"{what} holds no clips the tool reads (an .anim file, or a model that imports animation)");
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

        /// <summary>True when <paramref name="what"/> names a folder; <paramref name="files"/> gets the clip assets under it.</summary>
        public bool TryFolder(string what, out List<ClipFile> files)
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

        /// <summary>
        /// The clips a command argument names: an .anim, Model.fbx#Clip, a model (all its clips), a folder (every clip under
        /// it), or a clip name unique in the project.
        /// </summary>
        public List<ClipRecord> Clips(string what, TextWriter notes)
        {
            if (TryFolder(what, out var under))
            {
                if (under.Count == 0)
                    throw new WaitressException($"no clips under {what}");
                return Get(under, notes).SelectMany(r => r.Clips).ToList();
            }
            var hash = what.LastIndexOf('#');
            var file = TryAsset(what);
            string clipName = null;
            if (file == null && hash > 0)
            {
                file = TryAsset(what.Substring(0, hash));
                clipName = what.Substring(hash + 1);
            }
            if (file != null)
            {
                var record = Get(file, notes);
                if (clipName == null)
                {
                    if (record.Clips.Count == 0)
                        throw new WaitressException($"{file.AssetPath} has no clips");
                    return record.Clips;
                }
                var hit = record.Clips.Where(c => c.Name == clipName).ToList();
                if (hit.Count == 0)
                    hit = record.Clips.Where(c => c.Name.Equals(clipName, StringComparison.OrdinalIgnoreCase)).ToList();
                if (hit.Count == 0)
                    throw new WaitressException($"{file.AssetPath} has no clip '{clipName}'; it has {(record.Clips.Count == 0 ? "none" : string.Join(", ", record.Clips.Select(c => c.Name)))}");
                return hit;
            }
            var all = Get(All.ToList(), notes).SelectMany(r => r.Clips).ToList();
            var named = all.Where(c => c.Name == what).ToList();
            if (named.Count == 0)
                named = all.Where(c => c.Name.Equals(what, StringComparison.OrdinalIgnoreCase)).ToList();
            if (named.Count == 1)
                return named;
            if (named.Count > 1)
                throw new WaitressException($"'{what}' names {named.Count} clips; use the path:\n  " + string.Join("\n  ", named.Take(10).Select(c => c.Ref)));
            throw new WaitressException($"no clip '{what}' (an .anim path, Model.fbx#Clip, a model, a folder, or a unique clip name)");
        }

        /// <summary>Exactly one clip.</summary>
        public ClipRecord One(string what, TextWriter notes)
        {
            var clips = Clips(what, notes);
            if (clips.Count == 1)
                return clips[0];
            throw new WaitressException($"'{what}' holds {clips.Count} clips; name one:\n  " + string.Join("\n  ", clips.Take(12).Select(c => c.Ref)));
        }

        public static string Stamp(string fullPath)
        {
            var meta = fullPath + ".meta";
            return File.GetLastWriteTimeUtc(fullPath).Ticks.ToString(CultureInfo.InvariantCulture) + " " +
                   (File.Exists(meta) ? File.GetLastWriteTimeUtc(meta).Ticks : 0).ToString(CultureInfo.InvariantCulture);
        }

        public bool IsCurrent(ClipFile f)
        {
            Cached(f, out var current);
            return current;
        }

        AssetRecord Cached(ClipFile f, out bool current)
        {
            current = false;
            var file = FileFor(f.Guid);
            if (!File.Exists(file))
                return null;
            AssetRecord r;
            try
            {
                r = AssetRecord.Parse(File.ReadAllText(file));
            }
            catch (Exception e) when (e is JsonException or KeyNotFoundException or InvalidOperationException)
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
        public List<AssetRecord> Get(IReadOnlyList<ClipFile> files, TextWriter notes, bool force = false)
        {
            var records = new Dictionary<ClipFile, AssetRecord>();
            var needed = new List<ClipFile>();
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
                        throw new WaitressException((files.Count == 1 ? files[0].AssetPath + " hasn't" : $"{missing.Count} clip files haven't") +
                                                    $" been exported yet, and no Editor has the project open. Open it with `unity open \"{Root}\"` (exporting needs it once per change)");
                    foreach (var f in needed.Where(records.ContainsKey))
                        records[f].Stale = true;
                    var stale = needed.Count(records.ContainsKey);
                    if (stale > 0)
                        notes.WriteLine($"note: {(stale == 1 ? needed.First(records.ContainsKey).AssetPath + " changed" : stale + " clip files changed")} since export and no Editor is open to refresh; showing the last export");
                    if (missing.Count > 0)
                        notes.WriteLine($"note: skipped {missing.Count} never-exported file(s); open the Editor to include them");
                }
            }
            foreach (var pair in records)
                pair.Value.FullPath = pair.Key.FullPath;
            return files.Where(records.ContainsKey).Select(f => records[f]).ToList();
        }

        public AssetRecord Get(ClipFile file, TextWriter notes, bool force = false) =>
            Get(new[] { file }, notes, force).FirstOrDefault() ?? throw new WaitressException($"{file.AssetPath} couldn't be exported");

        /// <summary>
        /// Deletes records whose GUID no clip file in the project has any more (deleted, or re-added without its .meta, or a
        /// model that stopped importing animation). Discovery covers the whole project, so this is safe whatever a command
        /// was asked about.
        /// </summary>
        public int Prune()
        {
            var folder = Path.Combine(Folder, "Clips");
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

        /// <summary>Has the Editor export the assets; returns the ones it couldn't, with why.</summary>
        public Dictionary<string, string> Export(IReadOnlyList<ClipFile> files)
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
            return failed;
        }
    }
}
