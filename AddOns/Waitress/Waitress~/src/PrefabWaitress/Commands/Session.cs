using System;
using System.IO;
using PrefabWaitress.Model;

namespace PrefabWaitress.Commands
{
    /// <summary>The project a command works in, created on first use so commands that don't read assets stay instant.</summary>
    public sealed class Session
    {
        public static string ProjectOverride;
        Project m_Project;
        readonly string m_Hint;

        public Session(string hint)
        {
            m_Hint = hint;
        }

        public string Root
        {
            get
            {
                if (ProjectOverride != null)
                    return UnityProject.FindRoot(ProjectOverride) ?? throw new WaitressException($"--project {ProjectOverride} is not inside a Unity project (no Assets/ and ProjectSettings/ above it)");
                return UnityProject.FindRoot(m_Hint) ?? UnityProject.Current ?? throw new WaitressException("not inside a Unity project; run from the project folder or pass --project <path>");
            }
        }

        public Project Project => m_Project ??= new Project(Root);

        /// <summary>A scene or prefab named by a path (relative to the working directory or the project) or an asset path.</summary>
        public Asset Asset(string what)
        {
            var path = FindFile(what);
            var asset = Project.Load(path);
            if (asset.Kind == AssetKind.Missing)
                throw new WaitressException($"{what}: {asset.Problem}");
            if (asset.Kind == AssetKind.Other)
                throw new WaitressException($"{what} is not a scene (.unity) or prefab (.prefab)");
            return asset;
        }

        public string FindFile(string what)
        {
            if (File.Exists(what))
                return Path.GetFullPath(what);
            var inProject = Path.Combine(Root, what);
            if (File.Exists(inProject))
                return Path.GetFullPath(inProject);
            var full = Project.Index.FullPath(what.Replace('\\', '/'));
            if (File.Exists(full))
                return full;
            var guidPath = Project.Index.PathOf(what);
            if (guidPath != null)
                return Project.Index.FullPath(guidPath);
            throw new WaitressException($"no file '{what}' (a path, or an asset path such as Assets/Prefabs/Door.prefab)");
        }
    }
}
