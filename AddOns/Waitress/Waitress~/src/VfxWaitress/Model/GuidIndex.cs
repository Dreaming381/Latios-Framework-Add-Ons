using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VfxWaitress.Model
{
    /// <summary>
    /// Resolves Unity asset guids to asset names by reading .meta files, so the dense form can say
    /// which subgraph a node points at instead of printing a guid.
    ///
    /// The index is built lazily, once per project root, from .meta files. Assets/ and Packages/
    /// are scanned on the first lookup. Library/PackageCache holds tens of thousands more, so it's
    /// only scanned when a lookup misses the first two.
    /// </summary>
    public sealed class GuidIndex
    {
        static readonly Dictionary<string, GuidIndex> s_ByRoot = new Dictionary<string, GuidIndex>(StringComparer.OrdinalIgnoreCase);

        readonly Dictionary<string, string> m_Paths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly string m_Root;
        bool m_ProjectScanned;
        bool m_CacheScanned;

        GuidIndex(string root)
        {
            m_Root = root;
        }

        public static GuidIndex For(string assetPath)
        {
            var root = ProjectRoot(assetPath);
            if (root == null)
                return new GuidIndex(null);
            lock (s_ByRoot)
            {
                if (!s_ByRoot.TryGetValue(root, out var index))
                    s_ByRoot[root] = index = new GuidIndex(root);
                return index;
            }
        }

        /// <summary>The asset's file name without extension, or null when it is not in this project.</summary>
        public string NameOf(string guid)
        {
            var path = PathOf(guid);
            return path == null ? null : Path.GetFileNameWithoutExtension(path);
        }

        public string PathOf(string guid)
        {
            if (guid == null || m_Root == null)
                return null;
            ScanProject();
            if (m_Paths.TryGetValue(guid, out var path))
                return path;
            // Unity's built-in resources have guids like 0000000000000000f000000000000000 and no
            // .meta file anywhere, so don't pay for a cache scan to find that out.
            if (guid.StartsWith("0000000000000000", StringComparison.Ordinal))
                return null;
            ScanCache();
            return m_Paths.TryGetValue(guid, out path) ? path : null;
        }

        /// <summary>
        /// Resolves an asset by path or by bare file name to its guid. Returns null when nothing
        /// matches, and fills in <paramref name="ambiguous"/> when several do rather than silently
        /// picking one.
        /// </summary>
        public string GuidOf(string nameOrPath, out List<string> ambiguous)
        {
            ambiguous = null;
            if (string.IsNullOrEmpty(nameOrPath) || m_Root == null)
                return null;
            ScanProject();
            ScanCache();
            var needle = nameOrPath.Replace('\\', '/');

            var exact = m_Paths.FirstOrDefault(kvp => kvp.Value.Replace('\\', '/').EndsWith("/" + needle, StringComparison.OrdinalIgnoreCase));
            if (exact.Key != null)
                return exact.Key;

            var byName = m_Paths
                .Where(kvp => string.Equals(Path.GetFileNameWithoutExtension(kvp.Value), needle, StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(Path.GetFileName(kvp.Value), needle, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (byName.Count == 1)
                return byName[0].Key;
            if (byName.Count > 1)
                ambiguous = byName.Select(kvp => kvp.Value).OrderBy(p => p, StringComparer.Ordinal).ToList();
            return null;
        }

        /// <summary>
        /// The local file id of the main object for known asset kinds, or 0 otherwise. It's a
        /// per-importer constant, and a wrong guess resolves to nothing.
        /// </summary>
        public static long MainFileIdFor(string path)
        {
            switch (Path.GetExtension(path).ToLowerInvariant())
            {
                case ".hlsl":
                case ".cginc":
                    return 10900000; // ShaderInclude
                case ".compute":
                    return 7200000;
                case ".shader":
                    return 4800000;
                default:
                    return 0;
            }
        }

        static string ProjectRoot(string assetPath) => UnityProject.FindRoot(assetPath);

        void ScanProject()
        {
            if (m_ProjectScanned)
                return;
            m_ProjectScanned = true;
            ScanFolder(Path.Combine(m_Root, "Assets"), null, null);
            ScanFolder(Path.Combine(m_Root, "Packages"), null, null);
        }

        /// <summary>
        /// Indexes installed packages in Library/PackageCache under the path Unity gives them,
        /// Packages/&lt;name&gt;/..., since that's the path the Editor and the tool both use.
        /// </summary>
        void ScanCache()
        {
            if (m_CacheScanned)
                return;
            m_CacheScanned = true;
            var root = m_Root;
            var cache = Path.Combine(root, "Library", "PackageCache");
            if (!Directory.Exists(cache))
                return;
            foreach (var package in Directory.GetDirectories(cache))
            {
                var folder = Path.GetFileName(package);
                var at = folder.IndexOf('@');
                var name = at > 0 ? folder.Substring(0, at) : folder;
                // An embedded copy of a package wins over the cached one, as it does in Unity.
                if (Directory.Exists(Path.Combine(root, "Packages", name)))
                    continue;
                ScanFolder(package, package, Path.Combine(root, "Packages", name));
            }
        }

        void ScanFolder(string folder, string physicalRoot, string logicalRoot)
        {
            if (!Directory.Exists(folder))
                return;
            foreach (var meta in SafeEnumerate(folder))
            {
                string guid = null;
                try
                {
                    foreach (var line in File.ReadLines(meta))
                    {
                        if (!line.StartsWith("guid:", StringComparison.Ordinal))
                            continue;
                        guid = line.Substring(5).Trim();
                        break;
                    }
                }
                catch
                {
                    continue;
                }
                if (guid == null || m_Paths.ContainsKey(guid))
                    continue;
                var path = meta.Substring(0, meta.Length - ".meta".Length);
                if (physicalRoot != null)
                    path = logicalRoot + path.Substring(physicalRoot.Length);
                m_Paths[guid] = path;
            }
        }

        /// <summary>Every .meta file below <paramref name="root"/>, skipping folders Unity never imports.</summary>
        static IEnumerable<string> SafeEnumerate(string root)
        {
            var stack = new Stack<string>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var dir = stack.Pop();
                string[] files;
                try
                {
                    files = Directory.GetFiles(dir, "*.meta");
                    foreach (var sub in Directory.GetDirectories(dir))
                    {
                        var name = Path.GetFileName(sub);
                        if (name.EndsWith("~", StringComparison.Ordinal) || name.StartsWith(".", StringComparison.Ordinal))
                            continue;
                        stack.Push(sub);
                    }
                }
                catch
                {
                    continue;
                }
                foreach (var file in files)
                    yield return file;
            }
        }
    }
}
