using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrefabWaitress.Model
{
    /// <summary>Which scenes and prefabs instance which prefabs, found by scanning files for m_SourcePrefab without parsing them.</summary>
    public sealed class Usage
    {
        /// <summary>Asset path to the GUIDs of the prefabs (and models) it instances directly.</summary>
        public readonly Dictionary<string, HashSet<string>> Sources = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        public int Scanned;

        public Usage(Project project, IEnumerable<string> folders)
        {
            var files = folders.Where(Directory.Exists)
                .SelectMany(f => Directory.EnumerateFiles(f, "*.*", SearchOption.AllDirectories))
                .Where(f => f.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                .ToList();
            Scanned = files.Count;
            var found = new ConcurrentBag<(string path, HashSet<string> guids)>();
            Parallel.ForEach(files, f =>
            {
                var guids = Scan(f);
                if (guids.Count > 0)
                    found.Add((project.Index.ToAssetPath(f), guids));
            });
            foreach (var (path, guids) in found)
                Sources[path] = guids;
        }

        static HashSet<string> Scan(string file)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var text = File.ReadAllText(file, Encoding.UTF8);
            var at = 0;
            while ((at = text.IndexOf("m_SourcePrefab:", at, StringComparison.Ordinal)) >= 0)
            {
                var g = text.IndexOf("guid: ", at, StringComparison.Ordinal);
                if (g < 0 || g > at + 200)
                {
                    at += 15;
                    continue;
                }
                result.Add(text.Substring(g + 6, 32));
                at = g;
            }
            // Pre-2018.3 files name the source m_ParentPrefab.
            at = 0;
            while ((at = text.IndexOf("m_ParentPrefab:", at, StringComparison.Ordinal)) >= 0)
            {
                var g = text.IndexOf("guid: ", at, StringComparison.Ordinal);
                if (g > 0 && g < at + 200)
                    result.Add(text.Substring(g + 6, 32));
                at += 15;
            }
            return result;
        }

        /// <summary>Every file that instances the asset, directly or through other prefabs, nearest first.</summary>
        public List<string> Dependents(Project project, string guid)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { guid };
            var queue = new Queue<string>();
            queue.Enqueue(guid);
            while (queue.Count > 0)
            {
                var g = queue.Dequeue();
                foreach (var (path, guids) in Sources)
                {
                    if (!guids.Contains(g) || result.Contains(path, StringComparer.OrdinalIgnoreCase))
                        continue;
                    result.Add(path);
                    var own = project.Index.GuidOf(path);
                    if (own != null && seen.Add(own))
                        queue.Enqueue(own);
                }
            }
            return result;
        }
    }
}
