using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuildWaitress.Model
{
    public sealed class Issue
    {
        public string Severity;
        public string Text;
        public string Fix;
    }

    public sealed class AssetTotal
    {
        public string Key;
        public string Path;
        public string Name;
        public long Size;
        public readonly HashSet<string> Classes = new HashSet<string>();
        public readonly HashSet<string> Files = new HashSet<string>();
        public bool InSubscenes;
        public bool InPlayerData;
    }

    public static class Analysis
    {
        public static readonly string[] Categories = { "managed code", "native code", "scenes and assets", "subscenes", "subscene content", "streaming assets", "debug symbols", "other" };

        public static string Category(OutputFile f)
        {
            var rel = "/" + f.Rel.Replace('\\', '/');
            var lower = rel.ToLowerInvariant();
            if (lower.Contains("/streamingassets/"))
                return lower.Contains("/entityscenes/") ? "subscenes" : lower.Contains("/contentarchives/") ? "subscene content" : "streaming assets";
            if (lower.EndsWith(".pdb") || lower.EndsWith(".mdb") || lower.Contains("donotship") || lower.EndsWith(".dbg") || lower.EndsWith(".sym"))
                return "debug symbols";
            switch (f.Role)
            {
                case "ManagedLibrary":
                case "DependentManagedLibrary":
                case "ManagedEngineAPI":
                    return "managed code";
                case "Scene":
                case "SharedAssets":
                case "ResourcesFile":
                case "StreamingResourceFile":
                case "GlobalGameManagers":
                case "BuiltInResources":
                case "BuiltInShaders":
                    return "scenes and assets";
            }
            var name = System.IO.Path.GetFileName(lower);
            if (lower.Contains("/managed/") && (name.EndsWith(".dll") || name.EndsWith(".exe")))
                return "managed code";
            if (name.EndsWith(".dll") || name.EndsWith(".so") || name.EndsWith(".dylib") || name.EndsWith(".exe") || name.EndsWith(".bundle") || name.EndsWith(".x86_64") || name.EndsWith(".wasm"))
                return "native code";
            if (name.EndsWith(".assets") || name.EndsWith(".ress") || name.EndsWith(".resource") || name.StartsWith("level") || name.StartsWith("sharedassets") ||
                name == "globalgamemanagers" || name == "unity default resources" || name == "unity_builtin_extra" || name == "data.unity3d")
                return "scenes and assets";
            return "other";
        }

        /// <summary>The build's files: the report's list, plus StreamingAssets files on disk when the report leaves them out.</summary>
        public static List<OutputFile> Files(Build b)
        {
            var files = b.Report?.Files.ToList() ?? new List<OutputFile>();
            var reportHasStreaming = files.Any(f => Category(f) is "subscenes" or "subscene content" or "streaming assets");
            if (b.OnDisk && (b.Report == null || !reportHasStreaming))
            {
                var root = b.Report == null ? b.Dir : b.StreamingDir;
                if (root != null && Directory.Exists(root))
                    foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                    {
                        var rel = System.IO.Path.GetRelativePath(b.Dir, path).Replace('\\', '/');
                        if (b.Report == null || !files.Any(f => f.Rel == rel))
                            files.Add(new OutputFile { Path = path.Replace('\\', '/'), Rel = rel, Size = new FileInfo(path).Length, Role = b.Report == null ? "" : "(on disk)" });
                    }
            }
            if (b.Report != null)
                foreach (var f in DoNotShip(b))
                    if (!files.Any(x => x.Rel == f.Rel))
                        files.Add(f);
            return files;
        }

        /// <summary>Files Unity writes next to the build for debugging but never lists in the report (*_DoNotShip).</summary>
        public static IEnumerable<OutputFile> DoNotShip(Build b)
        {
            if (!b.OnDisk)
                yield break;
            foreach (var dir in Directory.GetDirectories(b.Dir, "*DoNotShip*"))
                foreach (var path in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
                    yield return new OutputFile { Path = path.Replace('\\', '/'), Rel = System.IO.Path.GetRelativePath(b.Dir, path).Replace('\\', '/'), Size = new FileInfo(path).Length, Role = "(on disk)" };
        }

        public static bool StreamingFromDisk(Build b) => b.Report == null || Files(b).Any(f => f.Role == "(on disk)" && !f.Rel.Contains("DoNotShip"));

        public static bool IsResources(string path) => path != null && (path.Contains("/Resources/", StringComparison.OrdinalIgnoreCase) || path.StartsWith("Resources/", StringComparison.OrdinalIgnoreCase));

        /// <summary>Packed assets totalled per source asset (an asset's .resS data included), plus content archive objects.</summary>
        public static List<AssetTotal> Assets(Build b, bool withSubscenes = true)
        {
            var map = new Dictionary<string, AssetTotal>(StringComparer.OrdinalIgnoreCase);
            if (b.Report != null)
                foreach (var a in b.Report.Assets)
                {
                    var key = a.Path.Length > 0 ? a.Path : $"({ClassIds.Name(a.ClassId)} in {a.File})";
                    if (!map.TryGetValue(key, out var t))
                        map[key] = t = new AssetTotal { Key = key, Path = a.Path };
                    t.Size += a.Size;
                    t.Classes.Add(ClassIds.Name(a.ClassId));
                    t.Files.Add(a.File);
                    t.InPlayerData = true;
                }
            if (withSubscenes)
                foreach (var o in b.ArchiveObjects.Where(o => !o.Leftover))
                {
                    var key = $"{ClassIds.Name(o.ClassId)} '{o.Name}' (subscene content)";
                    if (!map.TryGetValue(key, out var t))
                        map[key] = t = new AssetTotal { Key = key, Name = o.Name };
                    t.Size += o.Size;
                    t.Classes.Add(ClassIds.Name(o.ClassId));
                    t.Files.Add("ContentArchives/" + o.Archive);
                    t.InSubscenes = true;
                }
            return map.Values.OrderByDescending(t => t.Size).ToList();
        }

        /// <summary>Objects stored both in the player's data and in subscene content archives: the build carries them twice.</summary>
        public static List<(int classId, string name, long playerSize, long contentSize, List<string> playerPaths)> Duplicates(Build b)
        {
            var result = new List<(int, string, long, long, List<string>)>();
            if (b.Report == null || b.ArchiveObjects.Count == 0)
                return result;
            var player = new Dictionary<(int, string), (long size, List<string> paths)>();
            foreach (var a in b.Report.Assets)
            {
                var name = b.PlayerName(a) ?? (a.Path.Length > 0 ? System.IO.Path.GetFileNameWithoutExtension(a.Path) : null);
                if (string.IsNullOrEmpty(name))
                    continue;
                var key = (a.ClassId, name);
                if (!player.TryGetValue(key, out var v))
                    player[key] = v = (0, new List<string>());
                v.size += a.Size;
                if (a.Path.Length > 0 && !v.paths.Contains(a.Path))
                    v.paths.Add(a.Path);
                player[key] = v;
            }
            // MonoScripts are tiny and expected on both sides.
            foreach (var g in b.ArchiveObjects.Where(o => !string.IsNullOrEmpty(o.Name) && o.ClassId != 115).GroupBy(o => (o.ClassId, o.Name)))
                if (player.TryGetValue(g.Key, out var p))
                    result.Add((g.Key.ClassId, g.Key.Name, p.size, g.Sum(o => o.Size), p.paths));
            return result.OrderByDescending(r => Math.Min(r.Item3, r.Item4)).ToList();
        }

        public static long ChunkBytes(EntitySection s) => s.NodeSize("chunks");

        /// <summary>Chunk bytes no entity uses: exact with component sizes, else estimated from 1-2 entity chunks.</summary>
        public static long EmptyChunkBytes(EntitySection s)
        {
            if (s.Archetypes.All(a => a.Sizes != null))
                return Math.Max(0, s.Chunks.Count * (long)(EntitySection.ChunkSize - 64) - UsedChunkBytes(s));
            return s.Archetypes.Where(a => a.Entities > 0 && a.Entities <= 2 && a.Chunks == 1).Count() * (long)(EntitySection.ChunkSize - 512);
        }
        public static long BlobBytes(EntitySection s) => s.NodeSize("blob assets");
        public static int Entities(EntitySection s) => s.Archetypes.Sum(a => a.Entities);

        /// <summary>Bytes the entities' components actually use in their chunks, when the layout is known.</summary>
        public static long UsedChunkBytes(EntitySection s) => s.Archetypes.Where(a => a.Sizes != null).Sum(a => (long)a.Entities * a.Sizes.Sum());

        public static List<Issue> Check(Build b)
        {
            var issues = new List<Issue>();
            var r = b.Report;
            void Add(string severity, string text, string fix = null) => issues.Add(new Issue { Severity = severity, Text = text, Fix = fix });
            if (r != null)
            {
                if (r.Result != "Succeeded")
                {
                    var errors = r.Steps.SelectMany(s => s.Messages.Where(m => m.type == 0 || m.type == 4).Select(m => m.text)).Distinct().Take(3).ToList();
                    Add("error", $"the build {r.Result.ToLowerInvariant()}" + (errors.Count > 0 ? ": " + string.Join(" | ", errors.Select(e => Shorten(e, 300))) : " before Unity's build steps ran (a build processor or script compilation failed; the report has no messages)"),
                        errors.Count > 0 ? "`buildwaitress steps` shows every message with its step" : "rebuild with `buildwaitress build`, which captures the errors the report misses");
                }
                else if (r.Errors > 0)
                    Add("warning", $"the build succeeded with {Fmt.Plural(r.Errors, "error")} logged", "`buildwaitress steps` lists them");
                if (r.IsDevelopment)
                    Add("note", "development build (larger, slower, includes the profiler connection)");
                foreach (var flag in new[] { (1L << 28, "deep profiling support"), (1L << 9, "script debugging"), (1L << 22, "test assemblies"), (1L << 17, "forced assertions") })
                    if ((r.Options & flag.Item1) != 0)
                        Add("warning", $"{flag.Item2} is enabled; not for a release build");
                var tests = r.Files.Where(f => f.Rel.Contains("/Managed/") && (f.Rel.EndsWith("nunit.framework.dll", StringComparison.OrdinalIgnoreCase) || f.Rel.Contains("TestRunner", StringComparison.OrdinalIgnoreCase))).ToList();
                if (tests.Count > 0)
                    Add("warning", $"test framework assemblies ship in the build: {string.Join(", ", tests.Select(t => System.IO.Path.GetFileName(t.Rel)))} ({Fmt.Bytes(tests.Sum(t => t.Size))})",
                        "an assembly referencing them isn't Editor- or test-only; check `buildwaitress files Managed/`");
                if (b.OnDisk)
                {
                    var missing = b.Missing();
                    if (missing.Count > 0)
                        Add("note", $"{Fmt.Plural(missing.Count, "file")} of the report are gone from the folder (e.g. {missing[0].Rel}); it has been cleaned or moved since");
                    var changed = b.Changed();
                    if (changed.Count > 0)
                        Add("note", $"{Fmt.Plural(changed.Count, "file")} changed size since this build (e.g. {changed[0].Rel}); the folder holds a later build", "name the newer report, or rebuild");
                    if (!Analysis.StreamingFromDisk(b))
                    {
                        var stale = b.Stale();
                        if (stale.Count > 0)
                            Add("warning", $"{Fmt.Plural(stale.Count, "file")} in the build folder weren't written by this build ({Fmt.Bytes(stale.Sum(s => s.size))}), e.g. {string.Join(", ", stale.OrderByDescending(s => s.size).Take(3).Select(s => s.rel))}; they ship if the folder is shipped",
                                "delete the build folder and build again (Unity doesn't remove files an earlier build left)");
                    }
                }
            }
            var catalog = b.Catalog;
            if (catalog != null)
            {
                var listed = new HashSet<string>(catalog.Files.Select(f => f.id), StringComparer.OrdinalIgnoreCase);
                var orphan = b.Archives.Keys.Where(k => !listed.Contains(k)).ToList();
                if (orphan.Count > 0)
                {
                    var bytes = orphan.Sum(o => new FileInfo(System.IO.Path.Combine(b.StreamingDir, "ContentArchives", o)).Length);
                    Add("warning", $"{Fmt.Plural(orphan.Count, "content archive")} ({Fmt.Bytes(bytes)}) {(orphan.Count == 1 ? "isn't" : "aren't")} in the content catalog: leftovers of earlier builds, never loaded but shipped",
                        "delete StreamingAssets/ContentArchives in the build folder (or the whole folder) and build again");
                }
                var missingArchives = catalog.Files.Select(f => f.id).Where(id => id.Trim('0').Length > 0 && !b.Archives.ContainsKey(id)).ToList();
                if (missingArchives.Count > 0)
                    Add("error", $"{Fmt.Plural(missingArchives.Count, "content archive")} the catalog lists {(missingArchives.Count == 1 ? "is" : "are")} missing from the folder ({missingArchives[0]}); subscenes using them won't load");
            }
            foreach (var s in b.Subscenes)
            {
                foreach (var (meta, data, error) in s.Sections)
                {
                    var where = $"subscene {s.Name} section {meta.Index}";
                    if (error != null)
                    {
                        Add("error", $"{where}: {error}");
                        continue;
                    }
                    if (data.Size != meta.FileSize)
                        Add("error", $"{where}: the file is {Fmt.Bytes(data.Size)} but its header expects {Fmt.Bytes(meta.FileSize)}; the header and section come from different builds");
                    var single = data.Archetypes.Count(a => a.Entities > 0 && a.Entities <= 2 && a.Chunks == 1);
                    var empty = Analysis.EmptyChunkBytes(data);
                    if (single >= 4 && empty >= 128 * 1024 && empty >= data.Size / 4)
                        Add("warning", $"{where}: about {Fmt.Bytes(empty)} of its {Fmt.Bytes(data.Size)} is empty chunk space; {single} archetypes hold only 1-2 entities, each in its own 16 KB chunk",
                            $"`buildwaitress subscenes {s.Name} --archetypes` shows which component sets differ; giving those entities the same components (or tags) packs them together");
                }
            }
            if (b.StreamingDir != null && Directory.Exists(System.IO.Path.Combine(b.StreamingDir, "EntityScenes")))
            {
                var expected = new HashSet<string>(b.Subscenes.SelectMany(s => s.Sections.Select(x => $"{s.Guid}.{x.meta.Index}.entities")), StringComparer.OrdinalIgnoreCase);
                var orphans = Directory.GetFiles(System.IO.Path.Combine(b.StreamingDir, "EntityScenes"), "*.entities").Select(System.IO.Path.GetFileName).Where(f => !expected.Contains(f)).ToList();
                if (orphans.Count > 0)
                    Add("warning", $"{Fmt.Plural(orphans.Count, "section file")} in StreamingAssets/EntityScenes no subscene header lists ({string.Join(", ", orphans.Take(3))}): leftovers that ship but never load",
                        "delete StreamingAssets/EntityScenes in the build folder and build again");
            }
            var stalePairs = StaleSubscenes(b);
            if (stalePairs.Count > 0)
                Add("warning", $"{Fmt.Plural(stalePairs.Count, "subscene")} in StreamingAssets/EntityScenes predate this build by more than a minute ({string.Join(", ", stalePairs.Take(3))}): leftovers of an earlier build, or subscenes this build no longer includes",
                    "delete StreamingAssets/EntityScenes in the build folder and build again");
            var dups = Duplicates(b);
            if (dups.Count > 0)
            {
                var total = dups.Sum(d => Math.Min(d.playerSize, d.contentSize));
                Add("warning", $"{Fmt.Plural(dups.Count, "asset")} {(dups.Count == 1 ? "ships" : "ship")} twice, in the scenes' data and in subscene content archives ({Fmt.Bytes(total)} duplicated): " +
                               string.Join(", ", dups.Take(4).Select(d => $"{ClassIds.Name(d.classId)} '{d.name}' {Fmt.Bytes(Math.Min(d.playerSize, d.contentSize))}")),
                    "an asset referenced both from a regular scene (or Resources) and from a subscene is built into both; reference it from one side only, or load it through the subscene");
            }
            if (r != null)
            {
                var res = r.Assets.Where(a => IsResources(a.Path)).ToList();
                if (res.Count > 0)
                {
                    var tests = res.Where(a => a.Path.Contains("PerformanceTestRun", StringComparison.OrdinalIgnoreCase)).ToList();
                    if (tests.Count > 0)
                        Add("note", $"the Performance Testing package puts its run settings in Resources for every build ({Fmt.Bytes(tests.Sum(t => t.Size))}); harmless, but it means the package ships with the player");
                }
            }
            var files = Files(b);
            if (r != null && r.Platform is "Win64" or "Linux64" or "OSXUniversal")
            {
                var foreign = files.Where(f => System.Text.RegularExpressions.Regex.IsMatch(f.Rel, "/Plugins/(ARM64|arm64|x86|Win32|ARMv7|armv7)/", System.Text.RegularExpressions.RegexOptions.None)).ToList();
                if (r.Platform == "Win64" && foreign.Count > 0)
                    Add("note", $"{Fmt.Plural(foreign.Count, "plugin")} for another CPU {(foreign.Count == 1 ? "is" : "are")} in this Win64 build ({string.Join(", ", foreign.Take(3).Select(f => f.Rel))}; {Fmt.Bytes(foreign.Sum(f => f.Size))}); Unity copies them even when their import settings name only that CPU, so they're harmless but can be left out of an x64-only release");
            }
            var symbols = files.Where(f => Category(f) == "debug symbols").ToList();

            if (symbols.Count > 0)
            {
                var doNotShip = symbols.Where(f => f.Rel.Contains("DoNotShip", StringComparison.OrdinalIgnoreCase)).Sum(f => f.Size);
                var dev = r?.IsDevelopment ?? false;
                Add(dev ? "note" : "warning", $"debug symbols in the build folder: {Fmt.Bytes(symbols.Sum(f => f.Size))} in {Fmt.Plural(symbols.Count, "file")}" +
                    (doNotShip > 0 ? $", {Fmt.Bytes(doNotShip)} of it in *_DoNotShip folders" : "") + (dev ? " (expected in a development build)" : ""),
                    "leave *_DoNotShip folders and .pdb files out of what you ship; a release build without script debugging writes fewer of them");
            }
            if (r == null)
                Add("note", "no build report for this folder: failed-build, stale-file, duplicate and option checks need one (build again, or name the .buildreport)");
            foreach (var note in b.Notes)
                Add("note", note);
            return issues;
        }

        /// <summary>Subscene headers older than the build: with no file list for StreamingAssets, age is the remaining evidence.</summary>
        public static List<string> StaleSubscenes(Build b)
        {
            var result = new List<string>();
            if (b.Report == null || b.StreamingDir == null || !StreamingFromDisk(b) || b.Report.Started.Year < 2000)
                return result;
            var start = b.Report.Started.AddMinutes(-1);
            foreach (var s in b.Subscenes)
                if (File.GetLastWriteTime(s.Header.HeaderFile) < start)
                    result.Add(s.Name);
            return result;
        }

        public static string Shorten(string s, int max)
        {
            s = s.Replace("\r", "").Replace('\n', ' ');
            return s.Length <= max ? s : s.Substring(0, max) + "…";
        }
    }
}
