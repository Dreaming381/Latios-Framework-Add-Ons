using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BuildWaitress.Model;

namespace BuildWaitress.Commands
{
    /// <summary>
    /// Finds reports: Unity overwrites Library/LastBuild.buildreport on every build, so each new one is copied to
    /// UserSettings/Waitress/BuildWaitress/Reports/ the first time the tool sees it, which is what makes diff and history work.
    /// </summary>
    public sealed class Session
    {
        public static string ProjectOverride;
        /// <summary>--folder: the build folder to read with a report, for a build that was moved or copied.</summary>
        public static string FolderOverride;
        readonly string m_Hint;

        public Session(string hint, TextWriter notes)
        {
            m_Hint = hint;
            Notes = notes;
        }

        public TextWriter Notes { get; }

        /// <summary>The Unity project, or null when the tool runs outside one (reports and folders can still be named by path).</summary>
        public string Root
        {
            get
            {
                if (ProjectOverride != null)
                    return UnityProject.FindRoot(ProjectOverride) ?? throw new WaitressException($"--project {ProjectOverride} is not inside a Unity project (no Assets/ and ProjectSettings/ above it)");
                // Only the working directory picks the project: naming another project's report must never write into it.
                return UnityProject.FindRoot(Environment.CurrentDirectory);
            }
        }

        public string RequireRoot => Root ?? throw new WaitressException("not inside a Unity project; run from the project folder or pass --project <path>");

        public string Dir => Path.Combine(RequireRoot, "UserSettings", "Waitress", "BuildWaitress");
        string ReportsDir => Path.Combine(RequireRoot, "UserSettings", "Waitress", "BuildWaitress", "Reports");

        public sealed class Entry
        {
            public string Path;
            public DateTime Started;
            public string Guid;
        }

        /// <summary>Copies Library/LastBuild.buildreport into the archive when it holds a build not seen before.</summary>
        public void ArchiveLatest()
        {
            if (Root == null)
                return;
            var last = Path.Combine(Root, "Library", "LastBuild.buildreport");
            if (!File.Exists(last))
                return;
            Report r;
            try
            {
                r = Report.Load(last);
            }
            catch (WaitressException e)
            {
                Notes.WriteLine($"note: Library/LastBuild.buildreport is unreadable ({e.Message})");
                return;
            }
            var id = r.Guid.Trim('0').Length == 0 ? new FileInfo(last).LastWriteTimeUtc.Ticks.ToString("x") : r.Guid;
            Directory.CreateDirectory(ReportsDir);
            // Unity can give consecutive builds to one folder the same GUID, so the start time is part of the identity.
            var stamp = (r.Started.Year > 2000 ? r.Started : File.GetLastWriteTime(last)).ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            if (File.Exists(Path.Combine(ReportsDir, $"{stamp}_{id}.buildreport")))
                return;
            var copy = Path.Combine(ReportsDir, $"{stamp}_{id}.buildreport");
            File.Copy(last, copy, true);
            try
            {
                Compare.WriteSnapshot(copy, new Build(r, Directory.Exists(r.OutputDir) ? r.OutputDir : null, Root));
            }
            catch (Exception e) when (e is WaitressException || e is IOException)
            {
                Notes.WriteLine($"note: no subscene snapshot for this build ({e.Message})");
            }
        }

        public List<Entry> History()
        {
            ArchiveLatest();
            var list = new List<Entry>();
            if (Root == null || !Directory.Exists(ReportsDir))
                return list;
            foreach (var f in Directory.GetFiles(ReportsDir, "*.buildreport"))
            {
                var name = Path.GetFileNameWithoutExtension(f);
                var cut = name.IndexOf('_');
                if (cut < 0 || !DateTime.TryParseExact(name.Substring(0, cut), "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var when))
                    continue;
                list.Add(new Entry { Path = f, Started = when, Guid = name.Substring(cut + 1) });
            }
            return list.OrderByDescending(e => e.Started).ToList();
        }

        /// <summary>
        /// A build by name: nothing or "last" for the newest report, "prev" or "~N" for earlier ones, a .buildreport file,
        /// a build folder (its report when one is archived, else the folder alone), or a date/guid prefix from history.
        /// </summary>
        public Build Open(string spec)
        {
            var history = History();
            if (spec == null || spec is "last" or "latest" or "~0")
            {
                if (history.Count == 0)
                    throw new WaitressException(Root == null
                        ? "not inside a Unity project; name a .buildreport file or a build folder, or pass --project <path>"
                        : "no build report yet: build the player (buildwaitress build, or File > Build) or name a .buildreport file or build folder");
                return Load(history[0].Path);
            }
            if (spec == "prev" || spec.StartsWith("~", StringComparison.Ordinal))
            {
                var n = spec == "prev" ? 1 : int.TryParse(spec.Substring(1), out var k) ? k : -1;
                if (n < 0)
                    throw new WaitressException($"'{spec}': use ~1 for the build before the last, ~2 before that");
                if (n >= history.Count)
                    throw new WaitressException($"only {Fmt.Plural(history.Count, "archived report")}; `buildwaitress history` lists them");
                return Load(history[n].Path);
            }
            if (File.Exists(spec))
            {
                if (spec.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    return Open(Path.GetDirectoryName(Path.GetFullPath(spec)));
                return Load(spec);
            }
            if (Directory.Exists(spec))
            {
                var full = Path.GetFullPath(spec).Replace('\\', '/').TrimEnd('/');
                foreach (var e in history)
                {
                    var r = Report.Load(e.Path);
                    if (string.Equals(r.OutputDir, full, StringComparison.OrdinalIgnoreCase))
                        return Prepare(new Build(r, full, Root), Root);
                }
                if (!Directory.EnumerateDirectories(full, "*_Data").Any() && !Directory.Exists(Path.Combine(full, "StreamingAssets")) && !File.Exists(Path.Combine(full, "UnityPlayer.dll")))
                    throw new WaitressException($"{full} isn't a Unity player build folder (no *_Data folder, StreamingAssets or UnityPlayer.dll)");
                var under = Root != null && full.StartsWith(Root.Replace('\\', '/').TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase);
                return Prepare(new Build(null, full, under ? Root : null), under ? Root : null);
            }
            var match = history.Where(e => Path.GetFileNameWithoutExtension(e.Path).Contains(spec, StringComparison.OrdinalIgnoreCase)).ToList();
            if (match.Count == 1)
                return Load(match[0].Path);
            if (match.Count > 1)
                throw new WaitressException($"'{spec}' matches {match.Count} reports: {string.Join(", ", match.Take(5).Select(m => Path.GetFileNameWithoutExtension(m.Path)))}");
            throw new WaitressException($"no report or build folder '{spec}' (a .buildreport file, a build folder, last, prev, ~N, or a date/guid from `history`)");
        }

        Build Prepare(Build b, string owner)
        {
            if (owner != null && owner == Root)
                b.RefreshTypes = () => RefreshTypes(owner, b);
            if (Root != null)
                b.FallbackTypes = Path.Combine(Root, "UserSettings", "Waitress", "BuildWaitress", "types.tsv");
            if (Root != null && File.Exists(Path.Combine(Dir, "build-status.json")) && File.ReadAllText(Path.Combine(Dir, "build-status.json")).Contains("\"running\""))
                Notes.WriteLine("note: a build started by `buildwaitress build --no-wait` is still running; this is the build before it");
            return b;
        }

        /// <summary>Re-exports Entities types when the export is older than the compiled scripts and an Editor answers.</summary>
        static void RefreshTypes(string root, Build b)
        {
            var export = Path.Combine(root, "UserSettings", "Waitress", "BuildWaitress", "types.tsv");
            var scripts = Path.Combine(root, "Library", "ScriptAssemblies");
            if (!Directory.Exists(scripts) || b.StreamingDir == null || !Directory.Exists(Path.Combine(b.StreamingDir, "EntityScenes")))
                return;
            var newest = Directory.EnumerateFiles(scripts, "*.dll").Select(File.GetLastWriteTimeUtc).DefaultIfEmpty().Max();
            if (File.Exists(export) && File.GetLastWriteTimeUtc(export) >= newest)
                return;
            try
            {
                Act.ExportTypes(root);
            }
            catch (WaitressException e)
            {
                b.Notes.Add(File.Exists(export)
                    ? $"the Entities type export is older than the project's scripts and no Editor could refresh it; types changed since may show without names or sizes"
                    : "no Entities type export and no Editor to make one: open the project and run `buildwaitress types` for component sizes, chunk fill and blob owners");
            }
        }

        Build Load(string reportPath)
        {
            var r = Report.Load(reportPath);
            var dir = FolderOverride != null ? Path.GetFullPath(FolderOverride).Replace('\\', '/').TrimEnd('/') : Directory.Exists(r.OutputDir) ? r.OutputDir : null;
            if (FolderOverride != null && !Directory.Exists(dir))
                throw new WaitressException($"--folder {FolderOverride} doesn't exist");
            // A report inside some project's Library/ belongs to that project, and so do its subscene types.
            var root = Root;
            var lib = Path.GetDirectoryName(Path.GetFullPath(reportPath));
            if (lib != null && Path.GetFileName(lib).Equals("Library", StringComparison.OrdinalIgnoreCase))
                root = UnityProject.FindRoot(Path.GetDirectoryName(lib)) ?? root;
            var b = Prepare(new Build(r, dir, root), root);
            if (dir == null)
                b.Notes.Add($"the build folder {r.OutputDir} no longer exists; sizes come from the report alone (--folder names a moved copy)");
            return b;
        }
    }
}
