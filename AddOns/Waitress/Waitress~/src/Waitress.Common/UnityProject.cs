using System;
using System.Collections.Generic;
using System.IO;

namespace Waitress
{
    /// <summary>
    /// Finds the Unity project a command is working in, and where that project keeps Waitress's
    /// per-project files. The catalogs live in UserSettings/Waitress/, which Unity's default
    /// .gitignore already excludes, and which stays writable however the add-on was installed.
    /// </summary>
    public static class UnityProject
    {
        static string s_Hint;

        /// <summary>
        /// Remembers the first argument that names an existing file or folder, so later lookups
        /// start from the graph being edited rather than from wherever the shell happens to be.
        /// </summary>
        public static void HintFromArguments(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                if (string.IsNullOrEmpty(arg) || arg.StartsWith("-", StringComparison.Ordinal))
                    continue;
                try
                {
                    if (File.Exists(arg) || Directory.Exists(arg))
                    {
                        s_Hint = Path.GetFullPath(arg);
                        return;
                    }
                }
                catch (ArgumentException)
                {
                }
            }
        }

        /// <summary>The nearest folder at or above <paramref name="start"/> holding Assets/ and ProjectSettings/.</summary>
        public static string FindRoot(string start)
        {
            if (string.IsNullOrEmpty(start))
                return null;
            var dir = File.Exists(start) ? Path.GetDirectoryName(Path.GetFullPath(start)) : Path.GetFullPath(start);
            while (!string.IsNullOrEmpty(dir))
            {
                if (Directory.Exists(Path.Combine(dir, "Assets")) && Directory.Exists(Path.Combine(dir, "ProjectSettings")))
                    return dir;
                dir = Path.GetDirectoryName(dir);
            }
            return null;
        }

        /// <summary>The project found from the argument hint, falling back to the working directory.</summary>
        public static string Current => FindRoot(s_Hint) ?? FindRoot(Environment.CurrentDirectory);

        /// <summary>UserSettings/Waitress/<paramref name="fileName"/> in the current project, or null outside one.</summary>
        public static string SettingsFile(string fileName)
        {
            var root = Current;
            return root == null ? null : Path.Combine(root, "UserSettings", "Waitress", fileName);
        }

        /// <summary>
        /// The asset path Unity uses for a file ("Assets/..." or "Packages/..."), or null when the
        /// file isn't inside the project.
        /// </summary>
        public static string AssetPath(string path)
        {
            var full = Path.GetFullPath(path).Replace('\\', '/');
            var root = FindRoot(path);
            if (root != null)
            {
                var prefix = root.Replace('\\', '/').TrimEnd('/') + "/";
                if (full.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var relative = full.Substring(prefix.Length);
                    if (relative.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) ||
                        relative.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase))
                        return relative;
                }
            }
            return null;
        }
    }
}
