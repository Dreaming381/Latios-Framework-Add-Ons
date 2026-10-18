using System;
using System.Globalization;
using System.IO;

namespace PrefabWaitress.Model
{
    /// <summary>
    /// Imported model hierarchies the Editor exported (`models`), stored as Unity YAML under
    /// UserSettings/Waitress/PrefabWaitress/Models/. A cache is used only while the model file and its .meta are unchanged.
    /// </summary>
    public sealed class ModelCache
    {
        readonly Project m_Project;

        public ModelCache(Project project)
        {
            m_Project = project;
        }

        public string Folder => Path.Combine(m_Project.Root, "UserSettings", "Waitress", "PrefabWaitress", "Models");

        public string FileFor(string guid) => Path.Combine(Folder, guid + ".yaml");

        /// <summary>The stamp a cache must carry to be current: the model's and its .meta's write times.</summary>
        public string Stamp(string assetPath)
        {
            var full = m_Project.Index.FullPath(assetPath);
            if (!File.Exists(full))
                return null;
            var meta = full + ".meta";
            var a = File.GetLastWriteTimeUtc(full).Ticks;
            var b = File.Exists(meta) ? File.GetLastWriteTimeUtc(meta).Ticks : 0;
            return a.ToString(CultureInfo.InvariantCulture) + " " + b.ToString(CultureInfo.InvariantCulture);
        }

        public string Find(Asset asset)
        {
            if (asset.Guid == null)
                return null;
            var file = FileFor(asset.Guid);
            if (!File.Exists(file))
                return null;
            var text = File.ReadAllText(file);
            var firstLine = text.Substring(0, Math.Max(0, text.IndexOf('\n')));
            var stamp = Stamp(asset.Path);
            if (stamp == null || firstLine != "# stamp " + stamp)
                return null;
            return text;
        }

        public bool IsCurrent(Asset asset) => Find(asset) != null;
    }
}
