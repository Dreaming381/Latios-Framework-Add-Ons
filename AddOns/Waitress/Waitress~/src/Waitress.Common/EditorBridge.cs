using System.Collections.Generic;
using System.Text;

namespace Waitress
{
    /// <summary>
    /// A tool's connection to the Editor-side commands its package sample registers, named
    /// <c>&lt;prefix&gt;_&lt;operation&gt;</c>. Everything here becomes a no-op when no Editor answers or the
    /// sample isn't imported. The first such failure disables the rest of the run, so it costs one
    /// attempt rather than one per call.
    /// </summary>
    public sealed class EditorBridge
    {
        readonly string m_Prefix;
        readonly string m_Sample;
        bool m_Unavailable;

        public EditorBridge(string prefix, string sampleName)
        {
            m_Prefix = prefix;
            m_Sample = sampleName;
        }

        /// <summary>Set by --offline.</summary>
        public bool Offline;

        /// <summary>Why the last call couldn't reach the Editor, for a caller that wants to say so.</summary>
        public string LastProblem { get; private set; }

        public bool Available => !Offline && !m_Unavailable;

        /// <summary>
        /// Runs <paramref name="operation"/> against the asset at <paramref name="path"/>. Returns the
        /// command's result, "error: ..." when the command itself failed, or null when no Editor
        /// could run it.
        /// </summary>
        public string Call(string path, string operation, string argument = null)
        {
            if (!Available)
                return null;
            var reply = Run(path, operation, argument);
            if (reply.Success)
                return reply.Result;
            if (reply.Reached && !reply.MissingCommand)
                return "error: " + reply.Error;
            m_Unavailable = true;
            LastProblem = reply.MissingCommand
                ? $"the Editor has no {m_Prefix}_{operation} command; import the \"{m_Sample}\" sample from the Latios Framework Addons package"
                : "could not reach the Editor through the unity CLI: " + reply.Error;
            return null;
        }

        /// <summary>
        /// Like <see cref="Call"/>, but for an explicit request such as <c>validate --editor</c>, where
        /// failing to reach the Editor is itself the answer. Ignores --offline.
        /// </summary>
        public string Require(string path, string operation, string argument = null)
        {
            var reply = Run(path, operation, argument);
            if (reply.Success)
                return reply.Result;
            if (reply.MissingCommand)
                return $"error: the Editor has no {m_Prefix}_{operation} command; import the \"{m_Sample}\" sample from the Latios Framework Addons package";
            return reply.Reached ? "error: " + reply.Error : "error: could not reach the Editor through the unity CLI: " + reply.Error;
        }

        UnityCli.Reply Run(string path, string operation, string argument)
        {
            var args = new Dictionary<string, string>();
            if (path != null)
            {
                var assetPath = UnityProject.AssetPath(path);
                if (assetPath == null)
                    return new UnityCli.Reply { Reached = true, Error = $"{path} is not under the Assets/ or Packages/ folder of a Unity project" };
                args["path"] = assetPath;
            }
            if (argument != null)
                args["arg"] = argument;
            var root = path != null ? UnityProject.FindRoot(path) : UnityProject.Current;
            return UnityCli.Call(root, m_Prefix + "_" + Snake(operation), args);
        }

        static string Snake(string name)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && i > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(name[i]));
            }
            return sb.ToString();
        }
    }
}
