using System.IO;
using ModelWaitress.Model;

namespace ModelWaitress.Commands
{
    /// <summary>The project a command works in, created on first use so commands that don't read models stay instant.</summary>
    public sealed class Session
    {
        public static string ProjectOverride;
        readonly string m_Hint;
        Catalog m_Catalog;

        public Session(string hint, TextWriter notes)
        {
            m_Hint = hint;
            Notes = notes;
        }

        /// <summary>Where notes about stale or refreshed records go, ahead of the command's own output.</summary>
        public TextWriter Notes { get; }

        public string Root
        {
            get
            {
                if (ProjectOverride != null)
                    return UnityProject.FindRoot(ProjectOverride) ?? throw new WaitressException($"--project {ProjectOverride} is not inside a Unity project (no Assets/ and ProjectSettings/ above it)");
                return UnityProject.FindRoot(m_Hint) ?? UnityProject.Current ?? throw new WaitressException("not inside a Unity project; run from the project folder or pass --project <path>");
            }
        }

        public Catalog Catalog => m_Catalog ??= new Catalog(Root);

        public ModelRecord One(string what) => Catalog.Get(Catalog.Resolve(what), Notes);
    }
}
