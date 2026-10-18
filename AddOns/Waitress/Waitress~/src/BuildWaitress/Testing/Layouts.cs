using System.IO;
using System.Linq;
using BuildWaitress.Commands;

namespace BuildWaitress.Testing
{
    /// <summary>Every archetype's computed capacity and type hashes, for checking against the Editor's EntityArchetype.ChunkCapacity.</summary>
    public static class Layouts
    {
        public static int Run(Session session, Args a, TextWriter w)
        {
            var b = session.Open(a.Get("build"));
            a.RejectUnknown();
            foreach (var s in b.Subscenes)
                foreach (var (meta, data, _) in s.Sections)
                    if (data != null)
                        foreach (var ar in data.Archetypes)
                            w.WriteLine($"{s.Name}#{meta.Index}#{ar.Index}\t{ar.Capacity}\t{ar.Entities}\t{ar.Chunks}\t" +
                                        string.Join(",", ar.Types.Select((t, i) => data.TypeHashes[t] + (ar.ChunkComponent[i] ? "c" : ""))));
            return 0;
        }
    }
}
