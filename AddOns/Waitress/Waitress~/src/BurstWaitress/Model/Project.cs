using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BurstWaitress.Model
{
    /// <summary>
    /// The Unity project and where the tool keeps its files (UserSettings/Waitress/BurstWaitress/). The code epoch
    /// identifies the compiled scripts: a cached dump is valid only for the epoch it was made in.
    /// </summary>
    public static class Project
    {
        public static string Override;

        public static string Root
        {
            get
            {
                if (Override != null)
                    return UnityProject.FindRoot(Override) ?? throw new WaitressException($"--project {Override} is not inside a Unity project (no Assets/ and ProjectSettings/ above it)");
                return UnityProject.FindRoot(Environment.CurrentDirectory) ??
                       throw new WaitressException("not inside a Unity project; run from the project folder or pass --project <path>");
            }
        }

        public static string Dir => Path.Combine(Root, "UserSettings", "Waitress", "BurstWaitress");
        public static string DumpDir => Path.Combine(Dir, "Dumps");

        /// <summary>Hash of every compiled script assembly (name, size, write time), the Burst package and the Editor version.</summary>
        public static string Epoch()
        {
            var sb = new StringBuilder();
            var scripts = Path.Combine(Root, "Library", "ScriptAssemblies");
            if (Directory.Exists(scripts))
                foreach (var f in Directory.GetFiles(scripts, "*.dll").OrderBy(f => f, StringComparer.Ordinal))
                {
                    var info = new FileInfo(f);
                    sb.Append(info.Name).Append(':').Append(info.Length).Append(':').Append(info.LastWriteTimeUtc.Ticks).Append('\n');
                }
            var cache = Path.Combine(Root, "Library", "PackageCache");
            if (Directory.Exists(cache))
                foreach (var d in Directory.GetDirectories(cache, "com.unity.burst@*"))
                    sb.Append(Path.GetFileName(d)).Append('\n');
            var version = Path.Combine(Root, "ProjectSettings", "ProjectVersion.txt");
            if (File.Exists(version))
                sb.Append(File.ReadAllText(version));
            return Hash(sb.ToString());
        }

        /// <summary>When the scripts were last compiled, for messages.</summary>
        public static DateTime ScriptsTime()
        {
            var scripts = Path.Combine(Root, "Library", "ScriptAssemblies");
            return Directory.Exists(scripts) ? Directory.GetFiles(scripts, "*.dll").Select(File.GetLastWriteTime).DefaultIfEmpty().Max() : default;
        }

        public static string Hash(string s)
        {
            var bytes = SHA1.HashData(Encoding.UTF8.GetBytes(s));
            return Convert.ToHexString(bytes, 0, 8).ToLowerInvariant();
        }
    }
}
