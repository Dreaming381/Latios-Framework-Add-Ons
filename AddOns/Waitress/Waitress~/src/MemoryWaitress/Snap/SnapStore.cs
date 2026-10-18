using System;
using System.IO;
using System.Linq;

namespace MemoryWaitress.Snap
{
    /// <summary>Finds snapshot files: a path, a folder (its newest snapshot), or part of a name in the project's MemoryCaptures/.</summary>
    public static class SnapStore
    {
        public static string ProjectOverride;

        public static string ProjectRoot(string near = null)
        {
            if (ProjectOverride != null && UnityProject.FindRoot(ProjectOverride) == null)
                throw new WaitressException($"--project {ProjectOverride} is not inside a Unity project (no Assets/ and ProjectSettings/ above it)");
            var root = ProjectOverride != null ? UnityProject.FindRoot(ProjectOverride) : UnityProject.FindRoot(near) ?? UnityProject.Current;
            if (root == null)
                throw new WaitressException("not inside a Unity project; run from the project folder or pass --project <path>");
            return root;
        }

        public static string CaptureFolder(string root) => Path.Combine(root, "MemoryCaptures");

        public static string Resolve(string what)
        {
            if (File.Exists(what))
                return Path.GetFullPath(what);
            if (Directory.Exists(what))
                return Newest(what) ?? throw new WaitressException($"no .snap files in {what}");
            string folder = null;
            try
            {
                folder = CaptureFolder(ProjectRoot());
            }
            catch (WaitressException) when (ProjectOverride == null)
            {
            }
            if (folder != null && Directory.Exists(folder))
            {
                if (what is "latest" or "newest")
                    return Newest(folder) ?? throw new WaitressException($"no .snap files in {folder}");
                var hits = Directory.GetFiles(folder, "*.snap").Where(f => Path.GetFileName(f).Contains(what, StringComparison.OrdinalIgnoreCase)).ToList();
                if (hits.Count == 1)
                    return hits[0];
                if (hits.Count > 1)
                {
                    // A capture label (the part after the last _) or the whole name, matched exactly, wins over containment.
                    var labelled = hits.Where(h => { var n = Path.GetFileNameWithoutExtension(h); return string.Equals(n, what, StringComparison.OrdinalIgnoreCase) || n.EndsWith("_" + what, StringComparison.OrdinalIgnoreCase); }).ToList();
                    if (labelled.Count == 1)
                        return labelled[0];
                    var exact = hits.FirstOrDefault(h => string.Equals(Path.GetFileNameWithoutExtension(h), what, StringComparison.OrdinalIgnoreCase));
                    if (exact != null)
                        return exact;
                    throw new WaitressException($"'{what}' matches {hits.Count} snapshots: " + string.Join(", ", hits.Select(Path.GetFileName)));
                }
            }
            throw new WaitressException($"no snapshot '{what}' (a .snap path, a folder, 'latest', or part of a name in {folder ?? "<project>/MemoryCaptures"})");
        }

        static string Newest(string folder) =>
            Directory.GetFiles(folder, "*.snap").OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
    }
}
