using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using ShaderWaitress.Catalog;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Semantic view over a MultiJson file. Everything is a live projection of the underlying
    /// JSON: reads never dirty an object, and writes go straight through so untouched objects
    /// still serialize from their original text.
    /// </summary>
    public sealed class ShaderGraphDocument
    {
        public MultiJsonDocument Raw { get; private set; }
        public NodeCatalog Catalog { get; set; }
        public string Path { get; private set; }
        public bool IsSubGraph { get; private set; }

        readonly Dictionary<string, SgNode> m_Nodes = new Dictionary<string, SgNode>(StringComparer.Ordinal);
        readonly List<SgNode> m_NodeOrder = new List<SgNode>();
        readonly List<SgEdge> m_Edges = new List<SgEdge>();
        readonly Dictionary<string, SgProperty> m_Properties = new Dictionary<string, SgProperty>(StringComparer.Ordinal);
        readonly List<SgProperty> m_PropertyOrder = new List<SgProperty>();
        readonly Dictionary<string, SgGroup> m_Groups = new Dictionary<string, SgGroup>(StringComparer.Ordinal);
        readonly List<SgGroup> m_GroupOrder = new List<SgGroup>();
        readonly List<SgKeyword> m_Keywords = new List<SgKeyword>();
        readonly List<SgTarget> m_Targets = new List<SgTarget>();

        ShortIdTable m_ShortIds = new ShortIdTable();
        bool m_EdgesDirty;

        public IReadOnlyList<SgNode> Nodes => m_NodeOrder;
        public IReadOnlyList<SgEdge> Edges => m_Edges;
        public IReadOnlyList<SgProperty> Properties => m_PropertyOrder;
        public IReadOnlyList<SgGroup> Groups => m_GroupOrder;
        public IReadOnlyList<SgKeyword> Keywords => m_Keywords;
        public IReadOnlyList<SgTarget> Targets => m_Targets;
        public ShortIdTable ShortIds => m_ShortIds;
        public JsonObject RootNode => Raw.Root.Node;
        public List<string> Warnings { get; } = new List<string>();

        public static ShaderGraphDocument Load(string path, NodeCatalog catalog = null)
        {
            if (!File.Exists(path))
                throw new ShaderWaitressException($"no such graph: {path}");
            var doc = new ShaderGraphDocument
            {
                Raw = MultiJsonDocument.Load(path),
                Path = path,
                Catalog = catalog ?? NodeCatalog.Shared,
            };
            doc.Rebuild();
            return doc;
        }

        public static ShaderGraphDocument FromRaw(MultiJsonDocument raw, string path, NodeCatalog catalog = null)
        {
            var doc = new ShaderGraphDocument { Raw = raw, Path = path, Catalog = catalog ?? NodeCatalog.Shared };
            doc.Rebuild();
            return doc;
        }

        /// <summary>
        /// Re-derives the semantic view. Edges live in memory until they are flushed, so any
        /// pending edge changes have to reach the JSON before it is read back.
        /// </summary>
        public void Rebuild()
        {
            FlushEdges();
            m_Nodes.Clear();
            m_NodeOrder.Clear();
            m_Edges.Clear();
            m_Properties.Clear();
            m_PropertyOrder.Clear();
            m_Groups.Clear();
            m_GroupOrder.Clear();
            m_Keywords.Clear();
            m_Targets.Clear();
            m_ShortIds = new ShortIdTable();
            Warnings.Clear();

            var root = Raw.Root.Node;
            // A subgraph has the same GraphData root as a shader graph; what tells them apart is
            // that it names an output node instead of carrying render targets.
            IsSubGraph = !string.IsNullOrEmpty(UnityJsonWriter.RefId(root["m_OutputNode"])) ||
                         (Path != null && Path.EndsWith(".shadersubgraph", StringComparison.OrdinalIgnoreCase));

            foreach (var id in RefList(root, "m_Nodes"))
            {
                var entry = Raw.Find(id);
                if (entry == null)
                {
                    Warnings.Add($"node reference {id} has no object");
                    continue;
                }
                var node = new SgNode { Doc = this, Entry = entry };
                node.ShortId = m_ShortIds.Assign(entry.TypeName == "UnityEditor.ShaderGraph.BlockNode" ? 'b' : 'n', id);
                m_Nodes[id] = node;
                m_NodeOrder.Add(node);
            }

            foreach (var id in RefList(root, "m_Properties"))
            {
                var entry = Raw.Find(id);
                if (entry == null)
                    continue;
                var prop = new SgProperty { Doc = this, Entry = entry, ShortId = m_ShortIds.Assign('p', id) };
                m_Properties[id] = prop;
                m_PropertyOrder.Add(prop);
            }

            foreach (var id in RefList(root, "m_GroupDatas"))
            {
                var entry = Raw.Find(id);
                if (entry == null)
                    continue;
                var group = new SgGroup { Entry = entry, ShortId = m_ShortIds.Assign('g', id) };
                m_Groups[id] = group;
                m_GroupOrder.Add(group);
            }

            foreach (var id in RefList(root, "m_Keywords"))
            {
                var entry = Raw.Find(id);
                if (entry == null)
                    continue;
                m_Keywords.Add(new SgKeyword { Entry = entry, ShortId = m_ShortIds.Assign('k', id) });
            }

            foreach (var id in RefList(root, "m_ActiveTargets"))
            {
                var entry = Raw.Find(id);
                if (entry == null)
                    continue;
                var subId = UnityJsonWriter.RefId(entry.Node["m_ActiveSubTarget"]);
                m_Targets.Add(new SgTarget
                {
                    Entry = entry,
                    SubTarget = Raw.Find(subId),
                    ShortId = m_ShortIds.Assign('t', id),
                });
            }

            if (root["m_Edges"] is JsonArray edges)
            {
                foreach (var e in edges)
                {
                    var outNode = UnityJsonWriter.RefId(e?["m_OutputSlot"]?["m_Node"]);
                    var inNode = UnityJsonWriter.RefId(e?["m_InputSlot"]?["m_Node"]);
                    if (!m_Nodes.TryGetValue(outNode, out var from) || !m_Nodes.TryGetValue(inNode, out var to))
                    {
                        Warnings.Add("edge references a node that is not in the graph");
                        continue;
                    }
                    m_Edges.Add(new SgEdge
                    {
                        From = from,
                        FromSlot = ShaderWaitress.Serialization.Json.Int(e["m_OutputSlot"]["m_SlotId"], 0),
                        To = to,
                        ToSlot = ShaderWaitress.Serialization.Json.Int(e["m_InputSlot"]["m_SlotId"], 0),
                    });
                }
            }
            m_EdgesDirty = false;
        }

        public IEnumerable<string> RefList(JsonObject obj, string field)
        {
            if (obj?[field] is not JsonArray arr)
                yield break;
            foreach (var item in arr)
            {
                var id = UnityJsonWriter.RefId(item);
                if (!string.IsNullOrEmpty(id))
                    yield return id;
            }
        }

        public IReadOnlyList<string> ContextBlocks(bool vertex)
        {
            var ctx = RootNode[vertex ? "m_VertexContext" : "m_FragmentContext"] as JsonObject;
            return ctx == null ? Array.Empty<string>() : RefList(ctx, "m_Blocks").ToList();
        }

        /// <summary>A subgraph's Sub Graph Output node, which plays the part blocks play.</summary>
        public SgNode OutputNode
        {
            get
            {
                var named = NodeById(UnityJsonWriter.RefId(RootNode["m_OutputNode"]));
                if (named != null)
                    return named;
                return m_NodeOrder.FirstOrDefault(n => n.TypeName == "UnityEditor.ShaderGraph.SubGraphOutputNode");
            }
        }

        public SgNode NodeById(string objectId) =>
            objectId != null && m_Nodes.TryGetValue(objectId, out var n) ? n : null;

        public SgProperty PropertyById(string objectId) =>
            objectId != null && m_Properties.TryGetValue(objectId, out var p) ? p : null;

        public SgGroup GroupById(string objectId) =>
            objectId != null && m_Groups.TryGetValue(objectId, out var g) ? g : null;

        // ---- lookup by user-typed handle ----

        public SgNode ResolveNode(string handle)
        {
            if (string.IsNullOrEmpty(handle))
                throw new ShaderWaitressException("empty node reference");
            var objectId = m_ShortIds.ObjectFor(handle) ?? handle;
            var node = NodeById(objectId);
            if (node != null)
                return node;

            var byName = m_NodeOrder.Where(n => string.Equals(n.Name, handle, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byName.Count == 1)
                return byName[0];
            if (byName.Count > 1)
                throw new ShaderWaitressException($"'{handle}' matches {byName.Count} nodes by name; use a short id");

            var byBlock = m_NodeOrder.Where(n => n.IsBlock && n.BlockDescriptor != null &&
                                                 n.BlockDescriptor.EndsWith(handle, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byBlock.Count == 1)
                return byBlock[0];

            // A property handle in a wiring expression means "the node that reads it".
            var prop = ResolvePropertyOrNull(handle);
            if (prop != null)
            {
                var readers = m_NodeOrder.Where(n => n.IsProperty && n.PropertyId == prop.ObjectId).ToList();
                if (readers.Count == 1)
                    return readers[0];
                if (readers.Count > 1)
                    throw new ShaderWaitressException($"property '{handle}' is read by {readers.Count} nodes; use a node short id");
                throw new ShaderWaitressException($"property '{handle}' has no node in the graph; add one with 'node add Property --property {handle}'");
            }

            throw new ShaderWaitressException($"no node matches '{handle}'");
        }

        public SgProperty ResolvePropertyOrNull(string handle)
        {
            if (string.IsNullOrEmpty(handle))
                return null;
            var objectId = m_ShortIds.ObjectFor(handle) ?? handle;
            if (m_Properties.TryGetValue(objectId, out var byId))
                return byId;
            var matches = m_PropertyOrder.Where(p =>
                string.Equals(p.Name, handle, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p.ReferenceName, handle, StringComparison.OrdinalIgnoreCase)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public SgProperty ResolveProperty(string handle)
        {
            var p = ResolvePropertyOrNull(handle);
            if (p == null)
                throw new ShaderWaitressException($"no property matches '{handle}'");
            return p;
        }

        public SgKeyword ResolveKeyword(string handle)
        {
            if (string.IsNullOrEmpty(handle))
                throw new ShaderWaitressException("empty keyword reference");
            var objectId = m_ShortIds.ObjectFor(handle) ?? handle;
            var match = m_Keywords.FirstOrDefault(k => k.ObjectId == objectId)
                        ?? m_Keywords.FirstOrDefault(k => string.Equals(k.Name, handle, StringComparison.OrdinalIgnoreCase))
                        ?? m_Keywords.FirstOrDefault(k => string.Equals(k.ReferenceName, handle, StringComparison.OrdinalIgnoreCase));
            if (match == null)
                throw new ShaderWaitressException($"no keyword matches '{handle}'");
            return match;
        }

        public SgGroup ResolveGroupOrNull(string handle)
        {
            if (string.IsNullOrEmpty(handle))
                return null;
            var objectId = m_ShortIds.ObjectFor(handle) ?? handle;
            if (m_Groups.TryGetValue(objectId, out var byId))
                return byId;
            var matches = m_GroupOrder.Where(g => string.Equals(g.Title, handle, StringComparison.OrdinalIgnoreCase)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public SgGroup ResolveGroup(string handle)
        {
            var g = ResolveGroupOrNull(handle);
            if (g == null)
                throw new ShaderWaitressException($"no group matches '{handle}'");
            return g;
        }

        // ---- adjacency ----

        Dictionary<string, List<SgEdge>> m_OutEdges;
        Dictionary<string, List<SgEdge>> m_InEdges;

        void EnsureAdjacency()
        {
            if (m_OutEdges != null)
                return;
            m_OutEdges = new Dictionary<string, List<SgEdge>>(StringComparer.Ordinal);
            m_InEdges = new Dictionary<string, List<SgEdge>>(StringComparer.Ordinal);
            foreach (var e in m_Edges)
            {
                Bucket(m_OutEdges, e.From.ObjectId).Add(e);
                Bucket(m_InEdges, e.To.ObjectId).Add(e);
            }
        }

        static List<SgEdge> Bucket(Dictionary<string, List<SgEdge>> map, string key)
        {
            if (!map.TryGetValue(key, out var list))
            {
                list = new List<SgEdge>();
                map[key] = list;
            }
            return list;
        }

        public void InvalidateAdjacency()
        {
            m_OutEdges = null;
            m_InEdges = null;
        }

        public IReadOnlyList<SgEdge> EdgesOut(SgNode node)
        {
            EnsureAdjacency();
            return m_OutEdges.TryGetValue(node.ObjectId, out var list) ? (IReadOnlyList<SgEdge>)list : Array.Empty<SgEdge>();
        }

        public IReadOnlyList<SgEdge> EdgesIn(SgNode node)
        {
            EnsureAdjacency();
            return m_InEdges.TryGetValue(node.ObjectId, out var list) ? (IReadOnlyList<SgEdge>)list : Array.Empty<SgEdge>();
        }

        // ---- edge mutation ----

        public void AddEdge(SgEdge edge)
        {
            m_Edges.Add(edge);
            m_EdgesDirty = true;
            InvalidateAdjacency();
        }

        public int RemoveEdges(Func<SgEdge, bool> predicate)
        {
            var removed = m_Edges.RemoveAll(e => predicate(e));
            if (removed > 0)
            {
                m_EdgesDirty = true;
                InvalidateAdjacency();
            }
            return removed;
        }

        public void MarkEdgesDirty()
        {
            m_EdgesDirty = true;
            InvalidateAdjacency();
        }

        void FlushEdges()
        {
            if (!m_EdgesDirty)
                return;
            var arr = new JsonArray();
            foreach (var e in m_Edges)
            {
                arr.Add(new JsonObject
                {
                    ["m_OutputSlot"] = new JsonObject
                    {
                        ["m_Node"] = UnityJsonWriter.Ref(e.From.ObjectId),
                        ["m_SlotId"] = e.FromSlot,
                    },
                    ["m_InputSlot"] = new JsonObject
                    {
                        ["m_Node"] = UnityJsonWriter.Ref(e.To.ObjectId),
                        ["m_SlotId"] = e.ToSlot,
                    },
                });
            }
            Raw.Root.Edit()["m_Edges"] = arr;
            m_EdgesDirty = false;
        }

        public string Serialize()
        {
            FlushEdges();
            return Raw.Serialize();
        }

        public void Save(string path = null)
        {
            FlushEdges();
            Raw.Save(path ?? Path ?? throw new ShaderWaitressException("no output path"));
        }

        /// <summary>Title used in headers: the file name without extension.</summary>
        public string DisplayName =>
            Path == null ? "graph" : System.IO.Path.GetFileNameWithoutExtension(Path);
    }
}
