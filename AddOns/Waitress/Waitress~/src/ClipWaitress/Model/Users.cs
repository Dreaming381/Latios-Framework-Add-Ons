using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClipWaitress.Model
{
    /// <summary>A text-serialized asset that references a clip or a controller, and how.</summary>
    public sealed class Use
    {
        public string File;
        public List<string> What = new List<string>();
        public List<Use> Holders = new List<Use>();
        public bool IsController => File.EndsWith(".controller", StringComparison.OrdinalIgnoreCase) || File.EndsWith(".overrideController", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Finds what references a clip in scenes, prefabs, controllers and timelines by scanning their YAML.</summary>
    public static class Users
    {
        static readonly string[] s_TextAssets = { ".unity", ".prefab", ".asset", ".controller", ".overrideController", ".playable" };
        static readonly Regex s_Header = new Regex(@"^--- !u!(\d+) &(-?\d+)", RegexOptions.Multiline);

        public static List<Use> Find(Catalog catalog, ClipRecord clip, bool all)
        {
            var files = Files(catalog, all);
            var direct = Scan(files, catalog.Root, clip.Asset.Guid, clip.Asset.Kind == "model" ? clip.Id : null);
            // Controllers are played by Animators, and override controllers stand in for their base controller.
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<Use>(direct.Where(u => u.IsController));
            while (queue.Count > 0)
            {
                var controller = queue.Dequeue();
                if (!seen.Add(controller.File))
                    continue;
                var guid = MetaGuid(Path.Combine(catalog.Root, controller.File));
                if (guid == null)
                    continue;
                controller.Holders = Scan(files, catalog.Root, guid, null).Where(u => u.File != controller.File).ToList();
                foreach (var h in controller.Holders.Where(h => h.IsController))
                    queue.Enqueue(h);
            }
            return direct;
        }

        static List<string> Files(Catalog catalog, bool all)
        {
            var folders = new List<string> { Path.Combine(catalog.Root, "Assets") };
            if (all)
                folders.Add(Path.Combine(catalog.Root, "Packages"));
            return folders.Where(Directory.Exists)
                          .SelectMany(f => Directory.EnumerateFiles(f, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true }))
                          .Where(f => s_TextAssets.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)).ToList();
        }

        static string MetaGuid(string file)
        {
            var meta = file + ".meta";
            if (!File.Exists(meta))
                return null;
            foreach (var line in File.ReadLines(meta))
                if (line.StartsWith("guid: ", StringComparison.Ordinal))
                    return line.Substring(6).Trim();
            return null;
        }

        static List<Use> Scan(List<string> files, string root, string guid, string fileId)
        {
            var needle = "guid: " + guid;
            var hits = new ConcurrentBag<Use>();
            Parallel.ForEach(files, f =>
            {
                string text;
                try
                {
                    text = File.ReadAllText(f);
                }
                catch (IOException)
                {
                    return;
                }
                if (!text.Contains(needle, StringComparison.Ordinal))
                    return;
                var use = new Use { File = Path.GetRelativePath(root, f).Replace('\\', '/') };
                var docs = Docs(text);
                foreach (var doc in docs.Values)
                {
                    foreach (var line in doc.Lines)
                    {
                        if (line.IndexOf(needle, StringComparison.Ordinal) < 0)
                            continue;
                        if (fileId != null && line.IndexOf("fileID: " + fileId + ",", StringComparison.Ordinal) < 0)
                            continue;
                        var key = line.TrimStart().TrimStart('-', ' ').Split(':')[0];
                        // Override targets and source links inside a prefab instance are the instance's own objects.
                        if (key is "target" or "m_CorrespondingSourceObject" or "m_SourcePrefab" or "objectReference")
                            continue;
                        use.What.Add(Describe(doc, key, docs));
                    }
                }
                if (use.What.Count > 0)
                {
                    use.What = use.What.Distinct().ToList();
                    hits.Add(use);
                }
            });
            return hits.OrderBy(h => h.File, StringComparer.OrdinalIgnoreCase).ToList();
        }

        sealed class Doc
        {
            public string Type, Name, GameObject;
            public List<string> Lines = new List<string>();
        }

        static Dictionary<string, Doc> Docs(string text)
        {
            var docs = new Dictionary<string, Doc>();
            var matches = s_Header.Matches(text);
            for (var i = 0; i < matches.Count; i++)
            {
                var start = matches[i].Index;
                var end = i + 1 < matches.Count ? matches[i + 1].Index : text.Length;
                var doc = new Doc();
                foreach (var raw in text.Substring(start, end - start).Split('\n').Skip(1))
                {
                    var line = raw.TrimEnd('\r');
                    if (doc.Type == null && line.Length > 0 && !line.StartsWith(" ", StringComparison.Ordinal))
                        doc.Type = line.TrimEnd(':');
                    else if (doc.Name == null && line.StartsWith("  m_Name: ", StringComparison.Ordinal))
                        doc.Name = line.Substring(10).Trim();
                    else if (doc.GameObject == null && line.StartsWith("  m_GameObject: {fileID: ", StringComparison.Ordinal))
                        doc.GameObject = line.Substring(25).TrimEnd('}').Trim();
                    doc.Lines.Add(line);
                }
                docs[matches[i].Groups[2].Value] = doc;
            }
            return docs;
        }

        // The state a blend tree sits in, through any blend trees nested above it.
        static string StateOf(Doc tree, Dictionary<string, Doc> docs, int depth)
        {
            var id = docs.FirstOrDefault(p => p.Value == tree).Key;
            var needle = "m_Motion: {fileID: " + id + "}";
            foreach (var d in docs.Values)
            {
                if (d == tree || !d.Lines.Any(l => l.Contains(needle, StringComparison.Ordinal)))
                    continue;
                if (d.Type == "AnimatorState")
                    return $" in state '{d.Name}'";
                if (d.Type == "BlendTree" && depth < 8)
                    return $" in blend tree '{d.Name}'" + StateOf(d, docs, depth + 1);
            }
            return "";
        }

        static string Describe(Doc doc, string key, Dictionary<string, Doc> docs)
        {
            var owner = doc.GameObject != null && docs.TryGetValue(doc.GameObject, out var go) && go.Name != null ? go.Name : null;
            switch (doc.Type)
            {
                case "AnimatorState": return $"state '{doc.Name}'";
                case "BlendTree": return $"blend tree '{doc.Name}'" + StateOf(doc, docs, 0);
                case "AnimatorOverrideController":
                    return key == "m_OverrideClip" ? "override clip (replaces another clip)" : key == "m_OriginalClip" ? "original clip (replaced by an override)" : "base controller";
                case "Animator": return owner != null ? $"Animator on '{owner}'" : "Animator (on a prefab instance's object)";
                case "Animation": return (owner != null ? $"Animation component on '{owner}'" : "Animation component") + (key == "m_Animation" ? " (default clip)" : "");
                case "MonoBehaviour": return owner != null ? $"script on '{owner}' ({key})" : doc.Name is { Length: > 0 } ? $"'{doc.Name}' ({key})" : key;
            }
            return $"{doc.Type}{(doc.Name is { Length: > 0 } ? " '" + doc.Name + "'" : "")} ({key})";
        }
    }
}
