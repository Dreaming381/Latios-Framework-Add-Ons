using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace ShaderWaitress.Serialization
{
    /// <summary>
    /// One top-level object in a MultiJson file. Holds the original text until something
    /// asks for the parsed node, and reverts to emitting the original text whenever it was
    /// never modified.
    /// </summary>
    public sealed class MultiJsonEntry
    {
        string m_RawText;
        JsonObject m_Node;
        bool m_Dirty;

        public string ObjectId { get; private set; }
        public string TypeName { get; private set; }

        public MultiJsonEntry(string objectId, string typeName, string rawText)
        {
            ObjectId = objectId;
            TypeName = typeName;
            m_RawText = rawText;
        }

        public static MultiJsonEntry FromNode(JsonObject node)
        {
            var id = (string)node[MultiJsonDocument.ObjectIdKey];
            var type = (string)node[MultiJsonDocument.TypeKey];
            return new MultiJsonEntry(id, type, null) { m_Node = node, m_Dirty = true };
        }

        /// <summary>Parsed view. Reading it does not mark the entry dirty.</summary>
        public JsonObject Node
        {
            get
            {
                if (m_Node == null)
                    m_Node = (JsonObject)JsonNode.Parse(m_RawText);
                return m_Node;
            }
        }

        public bool IsDirty => m_Dirty;

        /// <summary>Parsed view, marked as modified so it is re-serialized on save.</summary>
        public JsonObject Edit()
        {
            var node = Node;
            m_Dirty = true;
            return node;
        }

        public void Rename(string newObjectId)
        {
            Edit()[MultiJsonDocument.ObjectIdKey] = newObjectId;
            ObjectId = newObjectId;
        }

        public string Serialize()
        {
            if (!m_Dirty && m_RawText != null)
                return m_RawText;
            return UnityJsonWriter.Write(Node);
        }
    }

    /// <summary>
    /// Reader/writer for Shader Graph's MultiJson container format: a sequence of
    /// pretty-printed JSON objects separated by "\n\n", root object first and the rest
    /// sorted by object id.
    /// </summary>
    public sealed class MultiJsonDocument
    {
        public const string ObjectIdKey = "m_ObjectId";
        public const string TypeKey = "m_Type";
        public const string VersionKey = "m_SGVersion";
        const string k_Separator = "\n\n";

        readonly List<MultiJsonEntry> m_Entries = new List<MultiJsonEntry>();
        readonly Dictionary<string, MultiJsonEntry> m_ById = new Dictionary<string, MultiJsonEntry>(StringComparer.Ordinal);

        public string SourcePath { get; set; }

        /// <summary>
        /// Unity always writes LF, but a file that has been through a normalizing checkout
        /// arrives as CRLF. Parsing works on LF text and the original convention is restored
        /// on save so untouched files stay byte-identical.
        /// </summary>
        public bool UseCrLf { get; set; }

        public MultiJsonEntry Root { get; private set; }
        public IReadOnlyList<MultiJsonEntry> Entries => m_Entries;

        public MultiJsonEntry Find(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
                return null;
            return m_ById.TryGetValue(objectId, out var e) ? e : null;
        }

        public bool Contains(string objectId) => !string.IsNullOrEmpty(objectId) && m_ById.ContainsKey(objectId);

        public void Add(MultiJsonEntry entry)
        {
            if (m_ById.ContainsKey(entry.ObjectId))
                throw new ShaderWaitressException($"duplicate object id '{entry.ObjectId}'");
            m_Entries.Add(entry);
            m_ById.Add(entry.ObjectId, entry);
        }

        public MultiJsonEntry AddObject(string typeName, int sgVersion = 0, string objectId = null)
        {
            objectId ??= NewObjectId();
            var node = new JsonObject
            {
                [VersionKey] = sgVersion,
                [TypeKey] = typeName,
                [ObjectIdKey] = objectId,
            };
            var entry = MultiJsonEntry.FromNode(node);
            Add(entry);
            return entry;
        }

        public bool Remove(string objectId)
        {
            if (!m_ById.TryGetValue(objectId, out var e))
                return false;
            m_ById.Remove(objectId);
            m_Entries.Remove(e);
            if (Root == e)
                Root = null;
            return true;
        }

        static readonly Random s_Random = new Random();
        HashSet<string> m_TakenPrefixes;

        /// <summary>
        /// A fresh object id whose first four characters no other object in the document starts
        /// with. Short ids are that prefix, so a new node gets a four-character id and no existing
        /// node's id has to grow to make room for it.
        /// </summary>
        public string NewObjectId()
        {
            m_TakenPrefixes ??= new HashSet<string>(m_Entries.Select(e => Prefix(e.ObjectId)), StringComparer.Ordinal);
            var buffer = new byte[16];
            string id;
            var attempts = 0;
            do
            {
                lock (s_Random)
                    s_Random.NextBytes(buffer);
                var sb = new StringBuilder(32);
                foreach (var b in buffer)
                    sb.Append(b.ToString("x2"));
                id = sb.ToString();
            }
            while (m_ById.ContainsKey(id) || (m_TakenPrefixes.Contains(Prefix(id)) && ++attempts < 10000));
            m_TakenPrefixes.Add(Prefix(id));
            return id;
        }

        static string Prefix(string objectId) => objectId.Length > 4 ? objectId.Substring(0, 4) : objectId;

        public static MultiJsonDocument Load(string path)
        {
            var text = File.ReadAllText(path);
            var doc = Parse(text);
            doc.SourcePath = path;
            return doc;
        }

        /// <summary>
        /// Mirrors UnityEditor.ShaderGraph.Serialization.MultiJsonInternal.Parse so entry
        /// boundaries agree with the Editor exactly, including its fallbacks.
        /// </summary>
        public static MultiJsonDocument Parse(string text)
        {
            var doc = new MultiJsonDocument();
            if (IsPureCrLf(text))
            {
                doc.UseCrLf = true;
                text = text.Replace("\r\n", "\n");
            }
            var startIndex = 0;
            while (startIndex < text.Length)
            {
                var jsonBegin = text.IndexOf('{', startIndex);
                if (jsonBegin == -1)
                    break;

                var jsonEnd = text.IndexOf(k_Separator, jsonBegin, StringComparison.Ordinal);
                if (jsonEnd == -1)
                {
                    jsonEnd = text.IndexOf("\n\r\n", jsonBegin, StringComparison.Ordinal);
                    if (jsonEnd == -1)
                        jsonEnd = text.LastIndexOf('}') + 1;
                }

                var json = text.Substring(jsonBegin, jsonEnd - jsonBegin);
                string id = null, type = null;
                ReadHeader(json, ref id, ref type);
                if (doc.m_Entries.Count != 0 && string.IsNullOrWhiteSpace(type))
                    throw new ShaderWaitressException("object with no m_Type in MultiJson stream");

                if (doc.m_Entries.Count == 0 && id == null)
                {
                    var legacy = IsLegacyFormat(json);
                    throw new ShaderWaitressException(legacy
                        ? "this is the pre-10.0 Shader Graph format, which has no object ids; " +
                          "open it in Unity once to upgrade it to MultiJson"
                        : "the first object has no m_ObjectId, so this is not a MultiJson stream")
                    {
                        LegacyGraphFormat = legacy,
                    };
                }

                var entry = new MultiJsonEntry(id, type, json);
                doc.Add(entry);
                if (doc.Root == null)
                    doc.Root = entry;

                startIndex = jsonEnd + k_Separator.Length;
            }

            if (doc.Root == null)
                throw new ShaderWaitressException("file contains no MultiJson objects");
            return doc;
        }

        /// <summary>
        /// Shader Graph before 10.0 wrote one JSON object holding every node as an escaped
        /// string, with no object ids anywhere. Unity upgrades such a file the first time it
        /// imports it; several still ship unupgraded inside URP. Say which it is rather than
        /// failing on the first missing id.
        /// </summary>
        static bool IsLegacyFormat(string json) =>
            json.Contains("\"m_SerializableNodes\"", StringComparison.Ordinal) ||
            json.Contains("\"m_SerializedProperties\"", StringComparison.Ordinal) ||
            json.Contains("\"JSONnodeData\"", StringComparison.Ordinal);

        /// <summary>Pulls m_ObjectId/m_Type without materializing the whole object.</summary>
        static void ReadHeader(string json, ref string id, ref string type)
        {
            id = ScanString(json, "\"" + ObjectIdKey + "\"");
            type = ScanString(json, "\"" + TypeKey + "\"");
        }

        static string ScanString(string json, string key)
        {
            var k = json.IndexOf(key, StringComparison.Ordinal);
            if (k < 0)
                return null;
            var colon = json.IndexOf(':', k + key.Length);
            if (colon < 0)
                return null;
            var q = json.IndexOf('"', colon + 1);
            if (q < 0)
                return null;
            var sb = new StringBuilder();
            for (var i = q + 1; i < json.Length; i++)
            {
                var c = json[i];
                if (c == '\\' && i + 1 < json.Length)
                {
                    i++;
                    sb.Append(json[i] switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        'r' => '\r',
                        'b' => '\b',
                        'f' => '\f',
                        var other => other,
                    });
                    continue;
                }
                if (c == '"')
                    break;
                sb.Append(c);
            }
            return sb.ToString();
        }

        public string Serialize()
        {
            var ordered = new List<MultiJsonEntry>(m_Entries);
            var rootId = Root?.ObjectId;
            ordered.Sort((x, y) =>
            {
                if (ReferenceEquals(x, y))
                    return 0;
                if (x.ObjectId == rootId)
                    return -1;
                if (y.ObjectId == rootId)
                    return 1;
                return string.CompareOrdinal(x.ObjectId, y.ObjectId);
            });

            var sb = new StringBuilder();
            foreach (var entry in ordered)
            {
                sb.Append(entry.Serialize());
                sb.Append(k_Separator);
            }
            var text = sb.ToString();
            return UseCrLf ? text.Replace("\n", "\r\n") : text;
        }

        /// <summary>True when every newline in the text is part of a CRLF pair.</summary>
        static bool IsPureCrLf(string text)
        {
            var sawCrLf = false;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] != '\n')
                    continue;
                if (i == 0 || text[i - 1] != '\r')
                    return false;
                sawCrLf = true;
            }
            return sawCrLf;
        }

        public void Save(string path)
        {
            var bytes = new UTF8Encoding(false).GetBytes(Serialize());
            File.WriteAllBytes(path, bytes);
        }
    }
}
