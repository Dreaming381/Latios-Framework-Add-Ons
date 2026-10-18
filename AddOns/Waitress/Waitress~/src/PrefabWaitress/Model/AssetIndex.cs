using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrefabWaitress.Model
{
    /// <summary>The project's GUID to path map, read from every .meta file under Assets/, Packages/ and the package cache.</summary>
    public sealed class AssetIndex
    {
        public const string BuiltinExtra = "0000000000000000e000000000000000";
        public const string BuiltinDefault = "0000000000000000f000000000000000";

        public readonly string Root;
        readonly Dictionary<string, string> m_PathByGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, string> m_GuidByPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public AssetIndex(string root)
        {
            Root = Path.GetFullPath(root);
            var found = new ConcurrentBag<(string guid, string path)>();
            var folders = new List<string> { Path.Combine(Root, "Assets"), Path.Combine(Root, "Packages"), Path.Combine(Root, "Library", "PackageCache") };
            foreach (var folder in PackageFolders())
                folders.Add(folder);
            var metas = folders.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase)
                .SelectMany(f => Directory.EnumerateFiles(f, "*.meta", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = 0 }))
                .ToList();
            Parallel.ForEach(metas, meta =>
            {
                var guid = ReadGuid(meta);
                if (guid != null)
                    found.Add((guid, meta.Substring(0, meta.Length - 5)));
            });
            foreach (var (guid, path) in found)
            {
                var asset = ToAssetPath(path);
                m_PathByGuid[guid] = asset;
                m_GuidByPath[asset] = guid;
            }
        }

        /// <summary>Local packages referenced by path from Packages/manifest.json ("file:../x").</summary>
        IEnumerable<string> PackageFolders()
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
                var rel = line.Substring(at + 6, end - at - 6);
                var full = Path.GetFullPath(Path.Combine(Root, "Packages", rel));
                if (Directory.Exists(full) && !full.StartsWith(Root, StringComparison.OrdinalIgnoreCase))
                    yield return full;
            }
        }

        static string ReadGuid(string meta)
        {
            try
            {
                using var stream = new FileStream(meta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 512);
                var buffer = new byte[256];
                var n = stream.Read(buffer, 0, buffer.Length);
                var text = Encoding.ASCII.GetString(buffer, 0, n);
                var at = text.IndexOf("guid: ", StringComparison.Ordinal);
                if (at < 0 || at + 38 > text.Length)
                    return null;
                return text.Substring(at + 6, 32);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>"Assets/..." or "Packages/name/..." for a file on disk.</summary>
        public string ToAssetPath(string full)
        {
            full = Path.GetFullPath(full).Replace('\\', '/');
            var root = Root.Replace('\\', '/').TrimEnd('/') + "/";
            if (full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                var rel = full.Substring(root.Length);
                const string cache = "Library/PackageCache/";
                if (rel.StartsWith(cache, StringComparison.OrdinalIgnoreCase))
                {
                    var inner = rel.Substring(cache.Length);
                    var slash = inner.IndexOf('/');
                    var package = slash < 0 ? inner : inner.Substring(0, slash);
                    var at = package.IndexOf('@');
                    if (at >= 0)
                        package = package.Substring(0, at);
                    return "Packages/" + package + (slash < 0 ? "" : inner.Substring(slash));
                }
                return rel;
            }
            var packageJsonDir = FindPackageRoot(full);
            if (packageJsonDir != null)
            {
                var name = PackageName(packageJsonDir);
                if (name != null)
                    return "Packages/" + name + full.Substring(packageJsonDir.Length);
            }
            return full;
        }

        static string FindPackageRoot(string full)
        {
            var dir = Path.GetDirectoryName(full);
            while (!string.IsNullOrEmpty(dir))
            {
                if (File.Exists(Path.Combine(dir, "package.json")))
                    return dir.Replace('\\', '/');
                dir = Path.GetDirectoryName(dir);
            }
            return null;
        }

        static string PackageName(string dir)
        {
            foreach (var line in File.ReadLines(Path.Combine(dir, "package.json")))
            {
                var at = line.IndexOf("\"name\"", StringComparison.Ordinal);
                if (at < 0)
                    continue;
                var q1 = line.IndexOf('"', line.IndexOf(':', at) + 1);
                var q2 = line.IndexOf('"', q1 + 1);
                return line.Substring(q1 + 1, q2 - q1 - 1);
            }
            return null;
        }

        public string PathOf(string guid) => guid != null && m_PathByGuid.TryGetValue(guid, out var p) ? p : null;

        public string GuidOf(string assetPath) => m_GuidByPath.TryGetValue(assetPath.Replace('\\', '/'), out var g) ? g : null;

        public IEnumerable<string> AllPaths => m_GuidByPath.Keys;

        /// <summary>The file on disk for an asset path, following the package cache and local packages.</summary>
        public string FullPath(string assetPath)
        {
            var direct = Path.Combine(Root, assetPath);
            if (File.Exists(direct) || Directory.Exists(direct))
                return direct;
            if (assetPath.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase))
            {
                var inner = assetPath.Substring("Packages/".Length);
                var slash = inner.IndexOf('/');
                var package = slash < 0 ? inner : inner.Substring(0, slash);
                var rest = slash < 0 ? "" : inner.Substring(slash + 1);
                var cache = Path.Combine(Root, "Library", "PackageCache");
                if (Directory.Exists(cache))
                    foreach (var dir in Directory.EnumerateDirectories(cache, package + "@*"))
                    {
                        var candidate = Path.Combine(dir, rest);
                        if (File.Exists(candidate))
                            return candidate;
                    }
                foreach (var folder in PackageFolders())
                    if (string.Equals(PackageName(folder.Replace('\\', '/')), package, StringComparison.OrdinalIgnoreCase))
                    {
                        var candidate = Path.Combine(folder, rest);
                        if (File.Exists(candidate))
                            return candidate;
                    }
            }
            return direct;
        }

        /// <summary>A short display name for a reference to another asset.</summary>
        public string Describe(string guid, long fileId)
        {
            if (string.Equals(guid, BuiltinExtra, StringComparison.OrdinalIgnoreCase) || string.Equals(guid, BuiltinDefault, StringComparison.OrdinalIgnoreCase))
                return "builtin:" + (s_Builtins.TryGetValue(fileId, out var b) ? b : fileId.ToString());
            var path = PathOf(guid);
            if (path == null)
                return $"missing asset {guid}";
            return path;
        }

        static readonly Dictionary<long, string> s_Builtins = new Dictionary<long, string>
        {
            [10202] = "Cube",
            [10206] = "Cylinder",
            [10207] = "Sphere",
            [10208] = "Capsule",
            [10209] = "Plane",
            [10210] = "Quad",
            [10303] = "Default-Material",
        };
    }
}
