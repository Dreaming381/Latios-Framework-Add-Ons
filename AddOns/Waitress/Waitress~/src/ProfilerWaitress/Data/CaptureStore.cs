using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ProfilerWaitress.Data
{
    /// <summary>
    /// Maps a capture (.data or .raw) to its extracted cache in UserSettings/Waitress/ProfilerCache/, extracting it
    /// through the Editor when the cache is missing or older than the capture.
    /// </summary>
    public static class CaptureStore
    {
        public static string ProjectOverride;
        public static TextWriter Log = Console.Error;

        public static readonly string[] CaptureExtensions = { ".data", ".raw" };

        /// <summary>The project whose Editor extracts, and whose UserSettings/ holds the cache.</summary>
        public static string ProjectRoot(string capture = null)
        {
            if (ProjectOverride != null)
                return UnityProject.FindRoot(ProjectOverride) ?? throw new WaitressException($"{ProjectOverride} is not inside a Unity project");
            return UnityProject.FindRoot(Environment.CurrentDirectory)
                   ?? (capture != null ? UnityProject.FindRoot(capture) : null)
                   ?? throw new WaitressException("run this from inside a Unity project, or pass --project <path>");
        }

        public static string CacheDirectory(string root) => Path.Combine(root, "UserSettings", "Waitress", "ProfilerCache");

        /// <summary>
        /// A capture path, a folder (its newest capture), or a cache file. Relative paths that don't exist are
        /// also tried under the project's ProfilerCaptures/ folder.
        /// </summary>
        public static string ResolveCapture(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new WaitressException("missing capture path");
            if (!File.Exists(path) && !Directory.Exists(path) && !Path.IsPathRooted(path))
            {
                var root = ProjectOverride != null ? UnityProject.FindRoot(ProjectOverride) : UnityProject.FindRoot(Environment.CurrentDirectory);
                if (root == null)
                    throw new WaitressException($"no such file: {path}; capture names are looked up in <project>/ProfilerCaptures/, and this isn't inside a Unity project (pass --project <dir>)");
                if (root != null)
                {
                    foreach (var candidate in new[] { Path.Combine(root, "ProfilerCaptures", path), Path.Combine(root, path) })
                        if (File.Exists(candidate) || Directory.Exists(candidate))
                            return ResolveCapture(candidate);
                    var stem = Directory.Exists(Path.Combine(root, "ProfilerCaptures"))
                        ? Directory.EnumerateFiles(Path.Combine(root, "ProfilerCaptures"))
                            .Where(f => CaptureExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                            .Where(f => Path.GetFileNameWithoutExtension(f).Contains(path, StringComparison.OrdinalIgnoreCase)).ToList()
                        : new List<string>();
                    var exact = stem.Where(f => string.Equals(Path.GetFileNameWithoutExtension(f), path, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (exact.Count == 1)
                        return Path.GetFullPath(exact[0]);
                    if (stem.Count == 1)
                        return Path.GetFullPath(stem[0]);
                    if (stem.Count > 1)
                        throw new WaitressException($"'{path}' matches {stem.Count} captures in ProfilerCaptures/: " +
                                                    string.Join(", ", stem.Take(8).Select(Path.GetFileName)));
                }
                throw new WaitressException($"no such capture: {path}");
            }
            if (Directory.Exists(path))
            {
                var newest = Directory.EnumerateFiles(path)
                    .Where(f => CaptureExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
                return newest != null ? Path.GetFullPath(newest) : throw new WaitressException($"no .data or .raw capture in {path}");
            }
            var extension = Path.GetExtension(path).ToLowerInvariant();
            if (!CaptureExtensions.Contains(extension) && extension != ".pwx")
                throw new WaitressException($"{path} is not a Profiler capture; profilerwaitress reads .data (saved from the Profiler window) and .raw (player binary logs)");
            return Path.GetFullPath(path);
        }

        public static string CachePathFor(string capture)
        {
            var full = Path.GetFullPath(capture).Replace('\\', '/');
            var hash = 14695981039346656037UL;
            foreach (var c in full.ToLowerInvariant())
            {
                hash ^= c;
                hash *= 1099511628211UL;
            }
            var stem = Path.GetFileNameWithoutExtension(full);
            if (stem.Length > 60)
                stem = stem.Substring(0, 60);
            return Path.Combine(CacheDirectory(ProjectRoot(capture)), $"{stem}-{hash & 0xffffffffUL:x8}.pwx");
        }

        public static bool IsFresh(string capture, string cache)
        {
            if (!File.Exists(cache))
                return false;
            var stamp = CaptureFile.ReadStamp(cache);
            if (stamp == null)
                return false;
            var info = new FileInfo(capture);
            return stamp.Value.size == info.Length && stamp.Value.ticks == info.LastWriteTimeUtc.Ticks;
        }

        public static CaptureFile Open(string path, bool forceExtract = false)
        {
            path = ResolveCapture(path);
            if (path.EndsWith(".pwx", StringComparison.OrdinalIgnoreCase))
                return CaptureFile.Open(path);
            var cache = CachePathFor(path);
            if (forceExtract || !IsFresh(path, cache))
                Extract(path, cache);
            try
            {
                File.SetLastAccessTimeUtc(cache, DateTime.UtcNow);
            }
            catch (IOException)
            {
            }
            return CaptureFile.Open(cache);
        }

        public static void Extract(string capture, string cache)
        {
            var root = ProjectRoot(capture);
            Log.WriteLine($"extracting {Path.GetFileName(capture)} through the Editor (once per capture)...");
            var reply = EditorScript.Call(root, "Extract", capture.Replace('\\', '/'), Path.GetFullPath(cache).Replace('\\', '/'));
            Log.WriteLine($"  {reply.GetProperty("frames").GetInt32()} frames, {reply.GetProperty("samples").GetInt64():N0} samples, " +
                          $"{reply.GetProperty("totalMs").GetInt64() / 1000.0:F1}s");
            Prune(CacheDirectory(root), cache);
        }

        /// <summary>Keeps the cache under PROFILERWAITRESS_CACHE_MB (default 4096) by dropping the least recently used files.</summary>
        static void Prune(string directory, string keep)
        {
            var limit = 4096L;
            if (long.TryParse(Environment.GetEnvironmentVariable("PROFILERWAITRESS_CACHE_MB"), out var configured) && configured > 0)
                limit = configured;
            limit <<= 20;
            var files = new DirectoryInfo(directory).GetFiles("*.pwx").OrderByDescending(f => f.LastAccessTimeUtc).ToList();
            long total = 0;
            foreach (var f in files)
            {
                total += f.Length;
                if (total > limit && !string.Equals(f.FullName, Path.GetFullPath(keep), StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        f.Delete();
                        Log.WriteLine($"  pruned {f.Name} from the cache");
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }
    }

    /// <summary>
    /// Runs Editor/ProfilerWaitressEditor.cs through `unity command run_script`. The script is written into the
    /// project's UserSettings/, outside Assets/, so writing it never triggers an import.
    /// </summary>
    public static class EditorScript
    {
        public static string Source()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ProfilerWaitressEditor.cs");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static string Install(string root)
        {
            var path = Path.Combine(root, "UserSettings", "Waitress", "ProfilerWaitress", "ProfilerWaitressEditor.cs");
            var source = Source();
            if (!File.Exists(path) || File.ReadAllText(path) != source)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, source, new UTF8Encoding(false));
            }
            return path;
        }

        public static JsonElement Call(string root, string entry, params object[] args)
        {
            var script = Install(root);
            var call = new Dictionary<string, string>
            {
                ["file"] = script,
                ["entry"] = "ProfilerWaitressEditor." + entry,
                ["args"] = JsonSerializer.Serialize(args),
            };
            var reply = UnityCli.Call(root, "run_script", call, 3600);
            if (!reply.Success)
            {
                var why = reply.Reached ? reply.Error : "no Editor has this project open (" + reply.Error + ")";
                throw new WaitressException($"{entry} needs the Unity Editor: {why}. Open the project with `unity open \"{root}\"` and retry; " +
                                            "queries on captures that were already extracted work without it");
            }
            JsonElement run;
            try
            {
                run = JsonDocument.Parse(reply.Result).RootElement;
            }
            catch (JsonException)
            {
                throw new WaitressException("unexpected run_script reply: " + reply.Result);
            }
            if (!run.TryGetProperty("success", out var ok) || ok.ValueKind != JsonValueKind.True)
            {
                var sb = new StringBuilder("the Editor script failed: ");
                if (run.TryGetProperty("errorDetails", out var details) && details.ValueKind == JsonValueKind.String)
                    sb.Append(details.GetString());
                else if (run.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.String)
                    sb.Append(error.GetString());
                if (run.TryGetProperty("diagnostics", out var diagnostics) && diagnostics.ValueKind == JsonValueKind.Array)
                    foreach (var d in diagnostics.EnumerateArray())
                        sb.Append("\n  line ").Append(d.GetProperty("line")).Append(": ").Append(d.GetProperty("message").GetString());
                throw new WaitressException(sb.ToString());
            }
            var text = run.GetProperty("result").GetString();
            var result = JsonDocument.Parse(text).RootElement;
            if (result.TryGetProperty("ok", out var fine) && fine.ValueKind == JsonValueKind.False)
                throw new WaitressException(result.GetProperty("error").GetString());
            return result;
        }
    }
}
