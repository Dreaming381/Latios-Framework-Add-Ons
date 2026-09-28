using System;
using System.Collections.Generic;
using System.Linq;

namespace Waitress
{
    /// <summary>
    /// Maps long serialized ids to short handles: a kind letter plus the shortest unique slice of
    /// the id, starting at four characters. Both sides of a collision get one character longer.
    /// Since a handle comes only from the id, adding or removing an entity leaves the others alone,
    /// except for an entity whose slice the newcomer collides with.
    ///
    /// Which end of the id to slice depends on the format. Shader Graph's 32-hex object ids differ
    /// from the first character, so a prefix works. Unity's local file ids within one file share a
    /// long leading run, so only the tail tells them apart.
    /// </summary>
    public sealed class ShortIdTable
    {
        public enum Slice
        {
            Prefix,
            Tail,
        }

        const int k_MinLength = 4;

        readonly Slice m_Slice;
        readonly Dictionary<string, string> m_ByObjectId = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, string> m_ByShortId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ShortIdTable(Slice slice = Slice.Prefix)
        {
            m_Slice = slice;
        }

        public string Assign(char kind, string objectId)
        {
            if (m_ByObjectId.TryGetValue(objectId, out var existing))
                return existing;
            var length = k_MinLength;
            string candidate;
            while (true)
            {
                candidate = kind + Take(objectId, length);
                if (!m_ByShortId.TryGetValue(candidate, out var owner))
                    break;
                if (owner == objectId)
                    break;
                // Push the incumbent out one character too, so neither keeps an ambiguous handle —
                // but only onto a handle that is actually free. Overwriting here would hand two
                // entities the same id, which every id-addressed command would then resolve wrongly.
                if (length < objectId.Length)
                {
                    var bumped = kind + Take(owner, length + 1);
                    if (!m_ByShortId.ContainsKey(bumped))
                    {
                        m_ByShortId.Remove(candidate);
                        m_ByObjectId.Remove(owner);
                        m_ByShortId[bumped] = owner;
                        m_ByObjectId[owner] = bumped;
                    }
                }
                length++;
                if (length > objectId.Length)
                {
                    candidate = kind + objectId + "#" + m_ByShortId.Count.ToString();
                    break;
                }
            }
            m_ByShortId[candidate] = objectId;
            m_ByObjectId[objectId] = candidate;
            return candidate;
        }

        string Take(string objectId, int length)
        {
            if (objectId.Length <= length)
                return objectId;
            return m_Slice == Slice.Prefix
                ? objectId.Substring(0, length)
                : objectId.Substring(objectId.Length - length);
        }

        public string ShortFor(string objectId) =>
            objectId != null && m_ByObjectId.TryGetValue(objectId, out var s) ? s : null;

        public string ObjectFor(string shortId) =>
            shortId != null && m_ByShortId.TryGetValue(shortId, out var s) ? s : null;

        public IEnumerable<KeyValuePair<string, string>> All => m_ByShortId;

        /// <summary>Sequential handles (n1, n2, …) for callers that want maximum density.</summary>
        public static ShortIdTable Sequential(IEnumerable<(char kind, string objectId)> ordered)
        {
            var table = new ShortIdTable();
            var counters = new Dictionary<char, int>();
            foreach (var (kind, objectId) in ordered)
            {
                counters.TryGetValue(kind, out var n);
                counters[kind] = ++n;
                var candidate = kind.ToString() + n.ToString();
                table.m_ByShortId[candidate] = objectId;
                table.m_ByObjectId[objectId] = candidate;
            }
            return table;
        }
    }
}
