using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildWaitress.Model;

namespace BuildWaitress.Commands
{
    public static class Read
    {
        // ------------------------------------------------------------------ summary

        /// <summary>The build a command names, as its one positional argument or --build.</summary>
        static Build OpenOne(Session session, Args a)
        {
            var b = session.Open(a.Get("build") ?? a.Positional.FirstOrDefault());
            a.RejectExtraPositionals(a.Has("build") ? 0 : 1);
            return b;
        }

        public static int Top(Args a, string name, int fallback)
        {
            var n = a.GetInt(name, fallback);
            if (n < 1)
                throw new WaitressException($"--{name} must be 1 or more");
            return n;
        }

        public static int Summary(Session session, Args a, TextWriter w)
        {
            var top = Top(a, "top", 6);
            var b = OpenOne(session, a);
            a.RejectUnknown();
            Header(b, w);
            var files = Analysis.Files(b);
            if (files.Count > 0)
            {
                var total = files.Sum(f => f.Size);
                w.WriteLine($"total {Fmt.Bytes(total)} in {Fmt.Plural(files.Count, "file")}" + (Analysis.StreamingFromDisk(b) && b.Report != null ? " (StreamingAssets counted from disk; the report doesn't list them)" : ""));
                foreach (var g in files.GroupBy(Analysis.Category).OrderByDescending(g => g.Sum(f => f.Size)))
                    w.WriteLine($"  {Fmt.Bytes(g.Sum(f => f.Size)),9}  {g.Key,-18} {CategoryDetail(b, g.Key, g.ToList())}");
            }
            if (b.Report != null && b.Report.Assets.Count > 0)
            {
                var assets = Analysis.Assets(b, false);
                w.WriteLine($"largest assets (of {assets.Count}; `assets` for more):");
                foreach (var t in assets.Take(top))
                    w.WriteLine($"  {Fmt.Bytes(t.Size),9}  {string.Join("+", t.Classes)}  {t.Key}");
            }
            var subscenes = b.Subscenes;
            if (subscenes.Count > 0)
            {
                var sections = subscenes.SelectMany(s => s.Sections).ToList();
                w.WriteLine($"subscenes: {subscenes.Count} with {Fmt.Plural(sections.Count, "section")} of {Fmt.Bytes(sections.Sum(s => s.meta.FileSize))}, " +
                            $"{Fmt.Count(sections.Where(s => s.data != null).Sum(s => Analysis.Entities(s.data)))} entities; largest: " +
                            string.Join(", ", subscenes.OrderByDescending(s => s.Size).Take(3).Select(s => $"{s.Name} {Fmt.Bytes(s.Size)}")));
            }
            var issues = Analysis.Check(b);
            var serious = issues.Count(i => i.Severity != "note");
            w.WriteLine(serious == 0 ? "no problems found" : $"{Fmt.Plural(serious, "problem")} (`check`): " + string.Join("; ", issues.Where(i => i.Severity != "note").Take(3).Select(i => Analysis.Shorten(i.Text, 110))));
            return 0;
        }

        static void Header(Build b, TextWriter w)
        {
            var r = b.Report;
            if (r == null)
            {
                w.WriteLine($"build folder {b.Dir} (no report found for it; reading what is on disk)");
                return;
            }
            var kind = r.BuildType == 2 ? "asset bundles" : "player";
            w.WriteLine($"{r.Platform} {kind}, {r.Result} in {Fmt.Time(r.Duration)}, {r.Started:yyyy-MM-dd HH:mm} (Unity {r.UnityVersion}); options: {r.OptionText()}" +
                        (r.Errors + r.Warnings > 0 ? $"; {Fmt.Plural(r.Errors, "error")}, {Fmt.Plural(r.Warnings, "warning")} (`steps`)" : ""));
            w.WriteLine($"output {r.OutputPath}" + (b.OnDisk ? b.Changed().Count > 0 ? " (the folder now holds a different build)" : "" : " (folder gone)"));
        }

        static string CategoryDetail(Build b, string category, List<OutputFile> files)
        {
            switch (category)
            {
                case "managed code":
                    return $"{Fmt.Plural(files.Count, "assembly", "assemblies")}; largest " + string.Join(", ", files.OrderByDescending(f => f.Size).Take(3).Select(f => $"{Path.GetFileName(f.Rel)} {Fmt.Bytes(f.Size)}"));
                case "scenes and assets":
                {
                    var res = files.Where(f => f.Rel.EndsWith(".resS", StringComparison.OrdinalIgnoreCase)).Sum(f => f.Size);
                    return $"{Fmt.Plural(files.Count(f => f.Role == "Scene" || Path.GetFileName(f.Rel).StartsWith("level")), "scene")}" + (res > 0 ? $", {Fmt.Bytes(res)} of it streamed texture/mesh data (.resS)" : "");
                }
                case "subscenes":
                    return $"{Fmt.Plural(files.Count(f => f.Rel.EndsWith(".entityheader")), "subscene")}, {Fmt.Plural(files.Count(f => f.Rel.EndsWith(".entities")), "section")} (`subscenes`)";
                case "subscene content":
                    return $"{Fmt.Plural(files.Count(f => !f.Rel.Contains('.')), "content archive")}: Unity objects subscenes reference";
                case "streaming assets":
                    return "largest " + string.Join(", ", files.GroupBy(f => TopFolder(f.Rel)).OrderByDescending(g => g.Sum(f => f.Size)).Take(3).Select(g => $"{g.Key} {Fmt.Bytes(g.Sum(f => f.Size))}"));
                default:
                    return "largest " + string.Join(", ", files.OrderByDescending(f => f.Size).Take(3).Select(f => $"{f.Rel} {Fmt.Bytes(f.Size)}"));
            }
        }

        static string TopFolder(string rel)
        {
            var i = rel.IndexOf("StreamingAssets/", StringComparison.OrdinalIgnoreCase);
            var rest = i >= 0 ? rel.Substring(i + 16) : rel;
            var slash = rest.IndexOf('/');
            return slash < 0 ? rest : rest.Substring(0, slash) + "/";
        }

        // ------------------------------------------------------------------ files

        public static int Files(Session session, Args a, TextWriter w)
        {
            var b = session.Open(a.Get("build"));
            var filter = a.Positional.FirstOrDefault();
            var top = Top(a, "top", 25);
            var by = a.Get("by", "file");
            a.RejectExtraPositionals(1);
            a.RejectUnknown();
            if (by != "file" && by != "folder")
                throw new WaitressException($"--by {by}: use file or folder");
            var files = Analysis.Files(b).Where(f => filter == null || Match(f.Rel, filter) || Analysis.Category(f).Contains(filter, StringComparison.OrdinalIgnoreCase) || f.Role.Equals(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            if (files.Count == 0)
                throw new WaitressException(filter == null ? "the build has no files" : $"no files match '{filter}'");
            w.WriteLine($"{Fmt.Plural(files.Count, "file")}, {Fmt.Bytes(files.Sum(f => f.Size))}" + (filter != null ? $" matching '{filter}'" : ""));
            if (by == "folder")
            {
                foreach (var g in files.GroupBy(f => Path.GetDirectoryName(f.Rel)?.Replace('\\', '/') ?? "").OrderByDescending(g => g.Sum(f => f.Size)).Take(top))
                    w.WriteLine($"  {Fmt.Bytes(g.Sum(f => f.Size)),9}  {g.Count(),5}  {g.Key}/");
                return 0;
            }
            if (by != "file")
                throw new WaitressException($"--by {by}: use file or folder");
            foreach (var f in files.OrderByDescending(f => f.Size).Take(top))
                w.WriteLine($"  {Fmt.Bytes(f.Size),9}  {f.Rel}  [{(f.Role.Length > 0 && f.Role.Length < 24 ? f.Role : Analysis.Category(f))}]");
            if (files.Count > top)
                w.WriteLine($"  … {files.Count - top} more ({Fmt.Bytes(files.OrderByDescending(f => f.Size).Skip(top).Sum(f => f.Size))}); --top N");
            return 0;
        }

        static bool Match(string text, string pattern)
        {
            if (pattern.IndexOfAny(new[] { '*', '?' }) >= 0)
                return Glob.Compile(pattern)(text) || Glob.Compile("*" + pattern.TrimStart('*'))(text);
            return text.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }

        // ------------------------------------------------------------------ assets

        public static int Assets(Session session, Args a, TextWriter w)
        {
            var b = session.Open(a.Get("build"));
            var filter = a.Positional.FirstOrDefault();
            var by = a.Get("by", "asset");
            var top = Top(a, "top", 25);
            a.RejectExtraPositionals(1);
            a.RejectUnknown();
            if (by is not ("asset" or "type" or "folder" or "package" or "file"))
                throw new WaitressException($"--by {by}: use asset, type, folder, package or file");
            if (b.Report == null && b.ArchiveObjects.Count == 0)
                throw new WaitressException("no report for this folder and no subscene content: asset sizes come from the build report");
            var all = Analysis.Assets(b);
            var leftovers = b.ArchiveObjects.Count(o => o.Leftover);
            if (leftovers > 0)
                w.WriteLine($"(left out: {Fmt.Plural(leftovers, "object")} in content archives the catalog doesn't list, leftovers of earlier builds; `check`)");
            var list = all.Where(t => filter == null || Match(t.Key, filter) || t.Classes.Any(c => c.Equals(filter, StringComparison.OrdinalIgnoreCase)) ||
                                      (filter.Equals("subscenes", StringComparison.OrdinalIgnoreCase) && t.InSubscenes)).ToList();
            if (list.Count == 0)
                throw new WaitressException($"no assets match '{filter}'" + Suggest(all.Select(t => t.Key), filter));
            w.WriteLine($"{Fmt.Plural(list.Count, "asset")}, {Fmt.Bytes(list.Sum(t => t.Size))}" + (filter != null ? $" matching '{filter}'" : "") + (b.Report != null && b.Report.Assets.Count == 0 ? " (the report has no asset list)" : "") +
                        (list.Any(t => t.InSubscenes) ? "; subscene content sizes are each archive's compressed size shared out (`why <name>` gives uncompressed)" : ""));
            IEnumerable<(string key, long size, int count)> rows;
            switch (by)
            {
                case "asset":
                    foreach (var t in list.Take(top))
                        w.WriteLine($"  {Fmt.Bytes(t.Size),9}  {string.Join("+", t.Classes),-14} {t.Key}" + (t.Files.Count <= 2 ? $"  [{string.Join(", ", t.Files)}]" : $"  [{t.Files.Count} files]"));
                    if (list.Count > top)
                        w.WriteLine($"  … {list.Count - top} more ({Fmt.Bytes(list.Skip(top).Sum(t => t.Size))}); --top N");
                    return 0;
                case "type":
                    rows = list.GroupBy(t => string.Join("+", t.Classes)).Select(g => (g.Key, g.Sum(t => t.Size), g.Count()));
                    break;
                case "folder":
                    rows = list.GroupBy(t => t.Path == null ? "(subscene content)" : t.Path.Length == 0 ? "(generated)" : Path.GetDirectoryName(t.Path)?.Replace('\\', '/') + "/").Select(g => (g.Key, g.Sum(t => t.Size), g.Count()));
                    break;
                case "package":
                    rows = list.GroupBy(t => Package(t.Path)).Select(g => (g.Key, g.Sum(t => t.Size), g.Count()));
                    break;
                case "file":
                {
                    var paths = new HashSet<string>(list.Where(t => t.Path != null).Select(t => t.Path));
                    var names = new HashSet<string>(list.Where(t => t.InSubscenes).Select(t => t.Key));
                    var player = (b.Report?.Assets ?? new List<PackedAsset>()).Where(x => paths.Contains(x.Path)).GroupBy(x => x.File).Select(g => (g.Key, g.Sum(x => x.Size), g.Count()));
                    var content = b.ArchiveObjects.Where(o => names.Contains($"{ClassIds.Name(o.ClassId)} '{o.Name}' (subscene content)")).GroupBy(o => "ContentArchives/" + o.Archive).Select(g => (g.Key, g.Sum(o => o.Size), g.Count()));
                    rows = player.Concat(content);
                    break;
                }
                default:
                    throw new WaitressException($"--by {by}: use asset, type, folder, package or file");
            }
            var sorted = rows.OrderByDescending(r => r.size).ToList();
            foreach (var r in sorted.Take(top))
                w.WriteLine($"  {Fmt.Bytes(r.size),9}  {r.count,5}  {r.key}");
            if (sorted.Count > top)
                w.WriteLine($"  … {sorted.Count - top} more; --top N");
            return 0;
        }

        static string Package(string path)
        {
            if (path == null)
                return "(subscene content)";
            if (path.Length == 0)
                return "(generated)";
            if (path.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase))
            {
                var parts = path.Split('/');
                return parts.Length > 1 ? parts[1] : path;
            }
            if (path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                var parts = path.Split('/');
                return parts.Length > 2 ? "Assets/" + parts[1] + "/" : "Assets/";
            }
            return path.Contains("unity default resources", StringComparison.OrdinalIgnoreCase) || path.StartsWith("Resources/unity_builtin", StringComparison.OrdinalIgnoreCase) ? "(built-in)" : path;
        }

        static string Suggest(IEnumerable<string> keys, string filter)
        {
            if (filter == null)
                return "";
            var leaf = Path.GetFileNameWithoutExtension(filter);
            var near = keys.Where(k => k.Contains(leaf, StringComparison.OrdinalIgnoreCase)).Take(3).ToList();
            return near.Count > 0 ? "; similar: " + string.Join(", ", near) : "";
        }

        // ------------------------------------------------------------------ why

        public static int Why(Session session, Args a, TextWriter w)
        {
            var what = a.Positional0("asset path, object name, output file or content archive");
            var b = session.Open(a.Get("build"));
            a.RejectExtraPositionals(1);
            a.RejectUnknown();
            var r = b.Report;
            var found = false;
            var leaf = Path.GetFileName(what.Replace('\\', '/'));
            var outputs = Analysis.Files(b).Where(f => f.Rel.Equals(what.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase) || Path.GetFileName(f.Rel).Equals(leaf, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var f in outputs.Where(f => !b.Archives.ContainsKey(Path.GetFileName(f.Rel))))
            {
                found = true;
                w.WriteLine($"{f.Rel}: {Fmt.Bytes(f.Size)}, {Analysis.Category(f)}{(f.Role.Length > 0 && f.Role.Length < 24 ? $" (role {f.Role})" : "")}");
                var packed = r?.Assets.Where(x => x.File.Equals(Path.GetFileName(f.Rel), StringComparison.OrdinalIgnoreCase)).ToList() ?? new List<PackedAsset>();
                if (packed.Count > 0)
                    w.WriteLine("  holds " + string.Join(", ", packed.GroupBy(x => x.Path.Length > 0 ? x.Path : "(generated)").OrderByDescending(g => g.Sum(x => x.Size)).Take(6).Select(g => $"{g.Key} {Fmt.Bytes(g.Sum(x => x.Size))}")) +
                                (packed.Select(x => x.Path).Distinct().Count() > 6 ? $" and {packed.Select(x => x.Path).Distinct().Count() - 6} more assets (`assets --by file`)" : ""));
                var scene = b.Subscenes.FirstOrDefault(s => Path.GetFileName(f.Rel).StartsWith(s.Guid + ".", StringComparison.OrdinalIgnoreCase));
                if (scene != null)
                    w.WriteLine($"  subscene {scene.Name} (`subscenes {scene.Name}`)");
                if (Analysis.Category(f) == "debug symbols")
                    w.WriteLine("  debug symbols: not needed to run the player");
            }
            if (r != null)
            {
                var entries = r.Assets.Where(x => x.Path.Equals(what, StringComparison.OrdinalIgnoreCase)).ToList();
                if (entries.Count == 0)
                    entries = r.Assets.Where(x => x.Path.Length > 0 && (x.Path.EndsWith("/" + what, StringComparison.OrdinalIgnoreCase) ||
                                                                         Path.GetFileNameWithoutExtension(x.Path).Equals(what, StringComparison.OrdinalIgnoreCase))).ToList();
                if (entries.Count == 0)
                    entries = r.Assets.Where(x => what.Equals(b.PlayerName(x), StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var g in entries.GroupBy(x => x.Path))
                {
                    found = true;
                    var path = g.Key.Length > 0 ? g.Key : "(generated)";
                    w.WriteLine($"{path}: {Fmt.Bytes(g.Sum(x => x.Size))} in the player data");
                    foreach (var c in g.GroupBy(x => (x.ClassId, x.File)).OrderByDescending(c => c.Sum(x => x.Size)))
                    {
                        var names = c.Select(x => b.PlayerName(x)).Where(n => n != null).Distinct().ToList();
                        var label = c.Count() == 1 ? ClassIds.Name(c.Key.ClassId) + (names.Count == 1 ? $" '{names[0]}'" : "") : $"{c.Count()} {ClassIds.Name(c.Key.ClassId)}s" + (names.Count > 0 ? $" ({string.Join(", ", names.Take(4))}{(names.Count > 4 ? ", …" : "")})" : "");
                        w.WriteLine($"  {Fmt.Bytes(c.Sum(x => x.Size)),9}  {label} in {c.Key.File}" + (c.Key.File.EndsWith(".resS", StringComparison.OrdinalIgnoreCase) ? " (streamed data)" : ""));
                    }
                    w.WriteLine("  why: " + Reason(b, g.Key, g.First().File));
                }
            }
            var archiveName = Path.GetFileName(what.Replace('\\', '/'));
            if (b.Archives.ContainsKey(archiveName))
            {
                found = true;
                var inside = b.ArchiveObjects.Where(o => o.Archive.Equals(archiveName, StringComparison.OrdinalIgnoreCase)).OrderByDescending(o => o.Size).ToList();
                var disk = new FileInfo(Path.Combine(b.StreamingDir, "ContentArchives", archiveName)).Length;
                var listed = b.Catalog?.Files.Any(f => f.id.Equals(archiveName, StringComparison.OrdinalIgnoreCase)) ?? true;
                w.WriteLine($"content archive {archiveName}: {Fmt.Bytes(disk)} on disk ({Fmt.Bytes(inside.Sum(o => o.Raw))} uncompressed), {Fmt.Plural(inside.Count, "object")}" +
                            (listed ? "" : "; NOT in the content catalog: a leftover of an earlier build"));
                foreach (var o in inside.Take(12))
                    w.WriteLine($"  {Fmt.Bytes(o.Size),9}  {o.Label}" + (o.Raw > o.Size ? $" ({Fmt.Bytes(o.Raw)} uncompressed)" : ""));
                if (inside.Count > 12)
                    w.WriteLine($"  … {inside.Count - 12} more");
                var users = inside.SelectMany(o => Users(b, o)).Distinct().ToList();
                w.WriteLine("  why: " + (users.Count > 0 ? "used by " + string.Join(", ", users.Take(8)) : listed ? "no subscene section references it directly (weak references or other content)" : "nothing loads it"));
            }
            foreach (var g in b.ArchiveObjects.Where(o => what.Equals(o.Name, StringComparison.OrdinalIgnoreCase) || (o.Name != null && o.Name.EndsWith("/" + what, StringComparison.OrdinalIgnoreCase)) ||
                                                          Path.GetFileNameWithoutExtension(what).Equals(o.Name, StringComparison.OrdinalIgnoreCase)).GroupBy(o => (o.ClassId, o.Name)))
            {
                found = true;
                var objects = g.ToList();
                var live = objects.Where(o => !o.Leftover).ToList();
                var shown = live.Count > 0 ? live : objects;
                var raw = shown.Sum(o => o.Raw);
                var disk = shown.Sum(o => o.Size);
                w.WriteLine($"{objects[0].Label}: {Fmt.Bytes(disk)} in subscene content{(raw > disk ? $" ({Fmt.Bytes(raw)} uncompressed)" : "")}, ContentArchives/{string.Join(", ", shown.Select(o => o.Archive).Distinct().Take(4))}" +
                            (live.Count == 0 ? " — only in leftover archives the catalog doesn't list" : objects.Count > live.Count ? $"; also in {Fmt.Plural(objects.Select(o => o.Archive).Distinct().Count() - live.Select(o => o.Archive).Distinct().Count(), "leftover archive")}" : ""));
                objects = shown;
                foreach (var chain in objects.SelectMany(o => Chains(b, o)).Distinct().Take(8))
                    w.WriteLine("  why: " + chain);
            }
            var missing = b.Catalog?.Files.Count(f => f.id.Trim('0').Length > 0 && !b.Archives.ContainsKey(f.id)) ?? 0;
            if (!found && missing > 0)
                throw new WaitressException($"'{what}' isn't in what can be read of this build, but {Fmt.Plural(missing, "content archive")} the catalog lists {(missing == 1 ? "is" : "are")} missing from the folder, so subscene objects in them can't be listed (`check`)");
            if (!found)
                throw new WaitressException($"'{what}' is not in this build" + Suggest(Analysis.Assets(b).Select(t => t.Key), what));
            return 0;
        }

        /// <summary>Paths from an archive object up to the subscene sections that reference it: "Material 'Bark' ← Trees section 0".</summary>
        static List<string> Chains(Build b, ArchiveObject o)
        {
            var result = new List<string>();
            var queue = new Queue<(ArchiveObject obj, string path, int depth)>();
            queue.Enqueue((o, "", 0));
            var seen = new HashSet<ArchiveObject>();
            while (queue.Count > 0 && result.Count < 8)
            {
                var (obj, path, depth) = queue.Dequeue();
                if (!seen.Add(obj))
                    continue;
                foreach (var s in DirectUsers(b, obj))
                    result.Add($"referenced by {path}{s}");
                if (depth >= 4)
                    continue;
                foreach (var parent in obj.UsedBy.Where(p => !(p.ClassId == 114 && string.IsNullOrEmpty(p.Name))))
                    queue.Enqueue((parent, path + parent.Label + " ← ", depth + 1));
            }
            if (result.Count == 0)
                result.Add("no subscene section reaches it through references: it is loaded by weak reference, or a leftover");
            return result;
        }

        static IEnumerable<string> DirectUsers(Build b, ArchiveObject o) =>
            b.Subscenes.SelectMany(s => s.Objects.Where(kv => kv.Value.Contains(o)).Select(kv => $"{s.Name} section {kv.Key}"));

        static IEnumerable<string> Users(Build b, ArchiveObject o) =>
            Chains(b, o).Where(c => c.StartsWith("referenced by", StringComparison.Ordinal)).Select(c => c.Substring(c.LastIndexOf(' ', c.LastIndexOf(" section", StringComparison.Ordinal) - 1) + 1));

        static string Reason(Build b, string path, string file)
        {
            var r = b.Report;
            var reasons = new List<string>();
            if (Analysis.IsResources(path))
                reasons.Add("it is in a Resources folder, and everything there ships");
            if (r.ScenesUsing.TryGetValue(path, out var scenes) && scenes.Count > 0)
                reasons.Add("used by " + string.Join(", ", scenes.Take(5)) + (scenes.Count > 5 ? $" and {scenes.Count - 5} more scenes" : ""));
            if (file.StartsWith("level", StringComparison.OrdinalIgnoreCase))
                reasons.Add($"part of scene {file} itself");
            if (file.StartsWith("globalgamemanagers", StringComparison.OrdinalIgnoreCase))
                reasons.Add("referenced from project settings or preloaded assets (global game managers)");
            if (reasons.Count == 0)
                reasons.Add(r.HasSceneUsage
                    ? "not used by a scene directly: a dependency of another asset, or of the render pipeline/settings"
                    : "the report doesn't say which scenes use it; build with --detailed (BuildOptions.DetailedBuildReport) to record that");
            return string.Join("; ", reasons);
        }

        // ------------------------------------------------------------------ subscenes

        public static int Subscenes(Session session, Args a, TextWriter w)
        {
            var name = a.Positional.FirstOrDefault();
            var spec = a.Get("build");
            if (spec == null && name != null && (name is "last" or "prev" || name.StartsWith("~", StringComparison.Ordinal) || File.Exists(name) || Directory.Exists(name)))
            {
                spec = name;
                name = a.Positional.Skip(1).FirstOrDefault();
                a.RejectExtraPositionals(2);
            }
            else
                a.RejectExtraPositionals(1);
            var b = session.Open(spec);
            var archetypes = a.Flag("archetypes");
            var top = Top(a, "top", 8);
            a.RejectUnknown();
            var all = b.Subscenes;
            if (all.Count == 0)
                throw new WaitressException(b.StreamingDir == null
                    ? "the build folder isn't on disk (subscene files are read from StreamingAssets/EntityScenes)"
                    : "this build has no subscenes (StreamingAssets/EntityScenes is empty or missing)");
            var types = b.Types;
            var list = name == null ? all : all.Where(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || s.Header.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                                                            s.Header.Guid.StartsWith(name, StringComparison.OrdinalIgnoreCase) ||
                                                            (s.Path != null && s.Path.Contains(name, StringComparison.OrdinalIgnoreCase))).ToList();
            if (list.Count == 0)
                throw new WaitressException($"no subscene '{name}'; the build has {string.Join(", ", all.Select(s => s.Name))}");
            var detail = name != null || archetypes;
            var sections = list.SelectMany(s => s.Sections).Where(s => s.data != null).Select(s => s.data).ToList();
            w.WriteLine($"{Fmt.Plural(list.Count, "subscene")}, {Fmt.Plural(list.Sum(s => s.Sections.Count), "section")}, {Fmt.Bytes(list.Sum(s => s.Size))}: " +
                        $"chunks {Fmt.Bytes(sections.Sum(Analysis.ChunkBytes))}, blobs {Fmt.Bytes(sections.Sum(Analysis.BlobBytes))}, " +
                        $"buffers outside chunks {Fmt.Bytes(sections.Sum(s => s.NodeSize("buffer data")))}, other {Fmt.Bytes(sections.Sum(s => s.Size - Analysis.ChunkBytes(s) - Analysis.BlobBytes(s) - s.NodeSize("buffer data")))}");
            if (!types.HasLayout)
                w.WriteLine(types.Source == "none"
                    ? "component names and sizes unknown: run `buildwaitress types` with the Editor open (or build once, for names from Logs/ExportedTypes.log)"
                    : $"component sizes unknown (names from {Path.GetFileName(types.Source)}); `buildwaitress types` with the Editor open adds sizes and chunk fill");
            var shown = list.OrderByDescending(s => s.Size).ToList();
            if (name == null && shown.Count > top)
            {
                w.WriteLine($"(largest {top} of {shown.Count}; --top N, or name one)");
                shown = shown.Take(top).ToList();
            }
            foreach (var s in shown)
            {
                w.WriteLine($"{s.Name}: {Fmt.Bytes(s.Size)}");
                foreach (var (meta, data, error) in s.Sections)
                {
                    if (data == null)
                    {
                        w.WriteLine($"  section {meta.Index}: {Fmt.Bytes(meta.FileSize)}, unreadable: {error}");
                        continue;
                    }
                    var entities = Analysis.Entities(data);
                    var used = Analysis.UsedChunkBytes(data);
                    var fill = types.HasLayout && data.Archetypes.All(x => x.Sizes != null) && data.Chunks.Count > 0
                        ? $", {Fmt.Pct(used / (double)(data.Chunks.Count * (long)(EntitySection.ChunkSize - 64)))} filled, {Fmt.Bytes(Analysis.EmptyChunkBytes(data))} empty" : "";
                    w.WriteLine($"  section {meta.Index}: {Fmt.Bytes(meta.FileSize)}{(data.Compressed ? $" (LZ4, {Fmt.Bytes(meta.RawSize)} raw)" : "")}, {Fmt.Plural(entities, "entity", "entities")}, " +
                                $"{Fmt.Plural(data.Archetypes.Count, "archetype")} in {Fmt.Plural(data.Chunks.Count, "chunk")} ({Fmt.Bytes(Analysis.ChunkBytes(data))}{fill})" +
                                (data.Blobs.Count > 0 ? $", {Fmt.Plural(data.Blobs.Count, "blob")} {Fmt.Bytes(data.Blobs.Sum(x => (long)x.Length))}" : "") +
                                (data.NodeSize("buffer data") > 16 ? $", buffers outside chunks {Fmt.Bytes(data.NodeSize("buffer data"))}" : "") +
                                (meta.ObjectRefs > 0 ? $", {Fmt.Plural(meta.ObjectRefs, "Unity object")}" : ""));
                    if (data.Blobs.Count > 0)
                        w.WriteLine("    blobs by component: " + string.Join(", ", data.Blobs.GroupBy(x => x.Owner ?? (types.HasLayout ? "unreferenced" : "owner unknown without component sizes")).OrderByDescending(g => g.Sum(x => (long)x.Length))
                            .Take(top).Select(g => $"{g.Key} {Fmt.Bytes(g.Sum(x => (long)x.Length))} ({Fmt.Plural(g.Count(), "blob")}, {Fmt.Plural(g.Sum(x => x.References), "ref")})")));
                    if (s.Objects.TryGetValue(meta.Index, out var objects) && objects.Count > 0)
                    {
                        w.WriteLine("    Unity objects: " + string.Join(", ", objects.GroupBy(o => o.ClassId).OrderByDescending(g => g.Sum(o => o.Size)).Select(g => g.Key < 0
                            ? $"{g.Count()} built-in or outside the archives"
                            : $"{g.Count()} {ClassIds.Name(g.Key)} {Fmt.Bytes(g.Sum(o => o.Size))}" + (detail ? $" ({string.Join(", ", g.Take(top).Select(o => o.Name ?? "?"))})" : ""))));
                        var indirect = new HashSet<ArchiveObject>();
                        var stack = new Stack<ArchiveObject>(objects);
                        while (stack.Count > 0)
                            foreach (var u in stack.Pop().Uses)
                                if (!objects.Contains(u) && indirect.Add(u))
                                    stack.Push(u);
                        if (indirect.Count > 0)
                            w.WriteLine("    through them (textures, shaders...): " + string.Join(", ", indirect.GroupBy(o => o.ClassId).OrderByDescending(g => g.Sum(o => o.Size)).Select(g =>
                                $"{g.Count()} {ClassIds.Name(g.Key)} {Fmt.Bytes(g.Sum(o => o.Size))}" + (detail ? $" ({string.Join(", ", g.Take(top).Select(o => o.Name ?? "?"))})" : ""))));
                    }
                    if (!detail)
                        continue;
                    if (types.HasLayout)
                    {
                        var bytes = new Dictionary<string, long>();
                        foreach (var ar in data.Archetypes.Where(x => x.Sizes != null))
                            for (var i = 0; i < ar.Types.Length; i++)
                            {
                                var key = types.Get(data.TypeHashes[ar.Types[i]]).Short;
                                bytes[key] = (bytes.TryGetValue(key, out var v) ? v : 0) + (long)ar.Entities * ar.Sizes[i + 1];
                            }
                        if (bytes.Values.Any(v => v > 0))
                            w.WriteLine("    chunk bytes by component: " + string.Join(", ", bytes.Where(kv => kv.Value > 0).OrderByDescending(kv => kv.Value).Take(top).Select(kv => $"{kv.Key} {Fmt.Bytes(kv.Value)}")));
                    }
                    foreach (var ar in data.Archetypes.OrderByDescending(x => x.Chunks).ThenByDescending(x => x.Entities).Take(archetypes ? int.MaxValue : top))
                    {
                        var comps = ar.Types.Select((t, i) => types.Get(data.TypeHashes[t]).Short + (ar.ChunkComponent[i] ? "(chunk)" : "")).ToList();
                        var cap = ar.Capacity > 0 ? $"/{ar.Capacity}" : "";
                        var perEntity = ar.Sizes != null ? $", {ar.Sizes.Sum()} B/entity" : "";
                        w.WriteLine($"    {ar.Entities,6} {(ar.Entities == 1 ? "entity  " : "entities")} in {ar.Chunks} chunk{(ar.Chunks == 1 ? "" : "s")} (capacity {(ar.Capacity > 0 ? ar.Capacity.ToString() : "?")}{perEntity}): {string.Join(", ", comps)}");
                    }
                    if (!archetypes && data.Archetypes.Count > top)
                        w.WriteLine($"    … {data.Archetypes.Count - top} more archetypes (--archetypes)");
                }
            }
            return 0;
        }

        // ------------------------------------------------------------------ steps

        public static int Steps(Session session, Args a, TextWriter w)
        {
            var all = a.Flag("all");
            var top = Top(a, "top", 10);
            var b = OpenOne(session, a);
            a.RejectUnknown();
            var r = b.Report ?? throw new WaitressException("steps come from a build report; this folder has none");
            if (r.Steps.Count == 0)
            {
                w.WriteLine($"the report has no build steps: the build {r.Result.ToLowerInvariant()} before Unity's build steps ran (a build processor or script compilation failed). `buildwaitress build` captures those errors.");
                return 0;
            }
            if (all)
            {
                foreach (var s in r.Steps)
                    w.WriteLine($"{new string(' ', s.Depth * 2)}{s.Name} {Fmt.Time(s.Duration)}{(s.Messages.Count > 0 ? $" ({Fmt.Plural(s.Messages.Count, "message")})" : "")}");
            }
            else
            {
                var leaves = r.Steps.Where(s => !r.Steps.Any(c => c.Parent == s)).OrderByDescending(s => s.Duration).Take(top);
                w.WriteLine($"slowest steps of {Fmt.Time(r.Duration)}:");
                foreach (var s in leaves)
                    w.WriteLine($"  {Fmt.Time(s.Duration),11}  {Path_(s)}");
            }
            var messages = r.Steps.SelectMany(s => s.Messages.Select(m => (s, m))).Where(x => all || x.m.type is 0 or 1 or 2 or 4).ToList();
            if (messages.Count > 0)
            {
                w.WriteLine(all ? "messages:" : "errors and warnings:");
                foreach (var g in messages.GroupBy(x => x.m.text).Take(a.GetInt("messages", 30)))
                {
                    var (s, m) = g.First();
                    var kind = m.type switch { 0 => "error", 1 => "assert", 2 => "warning", 4 => "exception", _ => "log" };
                    w.WriteLine($"  {kind} in {s.Name}{(g.Count() > 1 ? $" (×{g.Count()})" : "")}: {Analysis.Shorten(m.text, 400)}");
                }
            }
            return 0;
        }

        static string Path_(Step s)
        {
            var names = new List<string>();
            for (var x = s; x != null && x.Depth > 0; x = x.Parent)
                names.Add(x.Name);
            names.Reverse();
            return string.Join(" > ", names);
        }

        // ------------------------------------------------------------------ check

        public static int Check(Session session, Args a, TextWriter w)
        {
            var b = OpenOne(session, a);
            a.RejectUnknown();
            var issues = Analysis.Check(b);
            if (issues.Count == 0)
            {
                w.WriteLine("no problems found");
                return 0;
            }
            foreach (var i in issues.OrderBy(i => i.Severity switch { "error" => 0, "warning" => 1, _ => 2 }))
            {
                w.WriteLine($"{i.Severity}: {i.Text}");
                if (i.Fix != null)
                    w.WriteLine($"  fix: {i.Fix}");
            }
            return issues.Any(i => i.Severity != "note") ? 1 : 0;
        }

        // ------------------------------------------------------------------ history

        public static int History(Session session, Args a, TextWriter w)
        {
            a.RejectExtraPositionals(0);
            a.RejectUnknown();
            var history = session.History();
            if (history.Count == 0)
            {
                w.WriteLine("no reports archived yet; each new Library/LastBuild.buildreport is archived the first time buildwaitress runs after a build");
                return 0;
            }
            for (var i = 0; i < history.Count; i++)
            {
                var r = Report.Load(history[i].Path);
                w.WriteLine($"{(i == 0 ? "last" : "~" + i),5}  {r.Started:yyyy-MM-dd HH:mm}  {r.Platform,-8} {r.Result,-9} {Fmt.Bytes(r.TotalSize),9}  {Fmt.Time(r.Duration),12}  {r.OutputPath}");
            }
            return 0;
        }
    }
}
