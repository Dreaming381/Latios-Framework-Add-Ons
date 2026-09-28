using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VfxWaitress.Catalog;
using VfxWaitress.Serialization;

namespace VfxWaitress.Model
{
    public sealed class VfxSlot
    {
        public long FileId;
        public YamlDocument Doc;
        public VfxNode Owner;
        public VfxSlot Parent;
        public readonly List<VfxSlot> Children = new List<VfxSlot>();
        public string Name;
        public string TypeName;
        public bool IsInput;
        public string Value;
        public readonly List<long> LinkedSlotIds = new List<long>();

        /// <summary>
        /// Dotted path from the node, e.g. <c>Position</c> or <c>Position.x</c>. A node with a
        /// single anonymous port names it <c>in</c> or <c>out</c> rather than nothing at all.
        /// </summary>
        public string Path
        {
            get
            {
                var segments = new List<string>();
                for (var s = this; s != null; s = s.Parent)
                {
                    if (!string.IsNullOrEmpty(s.Name))
                        segments.Insert(0, s.Name);
                }
                return segments.Count == 0 ? (IsInput ? "in" : "out") : string.Join(".", segments);
            }
        }

        public IEnumerable<VfxSlot> SelfAndDescendants()
        {
            yield return this;
            foreach (var c in Children)
            {
                foreach (var d in c.SelfAndDescendants())
                    yield return d;
            }
        }
    }

    public sealed class VfxNode
    {
        public long FileId;
        public YamlDocument Doc;
        public string ShortId;
        public CatalogModel Model;
        public string Kind = "unknown";
        public string TypeName;
        public string ScriptGuid;
        public string Label;
        public float X, Y;
        public bool Collapsed;
        public bool SuperCollapsed;
        public readonly List<VfxSlot> Inputs = new List<VfxSlot>();
        public readonly List<VfxSlot> Outputs = new List<VfxSlot>();
        public readonly List<VfxNode> Blocks = new List<VfxNode>();
        public VfxNode Context;
        /// <summary>Flow links: (outputSlotIndex, target context, target input index).</summary>
        public readonly List<(int from, long toContext, int toIndex)> FlowOut = new List<(int, long, int)>();
        /// <summary>Parameter nodes only: the on-canvas instances of an exposed parameter.</summary>
        public readonly List<VfxParameterNode> ParameterNodes = new List<VfxParameterNode>();
        public string ExposedName;
        public bool Exposed;
        /// <summary>Subgraph outputs are parameters whose data flows into them.</summary>
        public bool IsOutput;
        public VfxData Data;
        public string ParameterValue;

        /// <summary>
        /// The node's type as the graph editor names it. A script the catalog does not cover —
        /// a render pipeline extension that is not installed, say — still prints its guid so the
        /// reader can identify it.
        /// </summary>
        public string DisplayType
        {
            get
            {
                // Every parameter is the same class; what distinguishes one is the type it carries.
                if (ExposedName != null)
                    return (IsOutput ? Inputs : Outputs).FirstOrDefault()?.TypeName ?? "Parameter";
                return Model?.ShortType ?? TypeName ??
                    ("Unknown<" + (ScriptGuid == null ? "?" : ScriptGuid.Substring(0, 8)) + ">");
            }
        }

        public IEnumerable<VfxSlot> AllInputSlots => Inputs.SelectMany(s => s.SelfAndDescendants());
        public IEnumerable<VfxSlot> AllOutputSlots => Outputs.SelectMany(s => s.SelfAndDescendants());
    }

    /// <summary>
    /// The per-system state a group of contexts shares. Capacity and bounds mode live here, not
    /// on any context, so a graph read context by context cannot see them at all.
    /// </summary>
    public sealed class VfxData
    {
        public long FileId;
        public YamlDocument Doc;
        public Catalog.CatalogModel Model;
        public string ShortId;
        public string Title;
        public readonly List<VfxNode> Owners = new List<VfxNode>();
    }

    /// <summary>One placement of an exposed parameter on the canvas.</summary>
    public sealed class VfxParameterNode
    {
        public int Id;
        public float X, Y;
        public readonly List<(long outputSlot, long inputSlot)> Links = new List<(long, long)>();
    }

    public sealed class VfxAsset
    {
        public const string GraphScriptGuid = "7d4c867f6b72b714dbb5fd1780afe208";
        public const string UiScriptGuid = "d01270efd3285ea4a9d6c555cb0a8027";

        public string Path;
        public YamlFile Yaml;
        public YamlDocument ResourceDoc;
        public YamlDocument GraphDoc;
        public YamlDocument UiDoc;
        public NodeCatalog Catalog;

        public readonly Dictionary<long, YamlDocument> ById = new Dictionary<long, YamlDocument>();
        public readonly Dictionary<long, VfxNode> Nodes = new Dictionary<long, VfxNode>();
        public readonly Dictionary<long, VfxSlot> Slots = new Dictionary<long, VfxSlot>();
        public readonly List<VfxNode> TopLevel = new List<VfxNode>();
        public readonly Dictionary<long, VfxData> Datas = new Dictionary<long, VfxData>();
        public readonly List<string> Warnings = new List<string>();

        public string Name
        {
            get
            {
                var stored = GraphDoc?.Body.Scalar("m_Name");
                return string.IsNullOrWhiteSpace(stored) ? System.IO.Path.GetFileNameWithoutExtension(Path) : stored;
            }
        }

        public string Kind => System.IO.Path.GetExtension(Path).TrimStart('.').ToLowerInvariant();

        public static VfxAsset Load(string path, NodeCatalog catalog = null)
        {
            if (!File.Exists(path))
                throw new VfxWaitressException("no such file: " + path);
            var asset = new VfxAsset
            {
                Path = path,
                Yaml = UnityYaml.Parse(File.ReadAllText(path)),
                Catalog = catalog ?? NodeCatalog.Default,
            };
            asset.Build();
            return asset;
        }

        void Build()
        {
            foreach (var doc in Yaml.Documents)
                ById[doc.FileId] = doc;

            ResourceDoc = Yaml.Documents.FirstOrDefault(d => d.TypeName == "VisualEffectResource");
            if (ResourceDoc != null)
            {
                var graphId = UnityYaml.FileIdIn(ResourceDoc.Body.Scalar("m_Graph"));
                if (graphId.HasValue && ById.TryGetValue(graphId.Value, out var g))
                    GraphDoc = g;
            }
            GraphDoc ??= Yaml.Documents.FirstOrDefault(d => ScriptOf(d) == GraphScriptGuid);
            if (GraphDoc == null)
                throw new VfxWaitressException("no VFXGraph object found in " + Path);

            var uiId = UnityYaml.FileIdIn(GraphDoc.Body.Scalar("m_UIInfos"));
            if (uiId.HasValue && ById.TryGetValue(uiId.Value, out var ui))
                UiDoc = ui;

            foreach (var childId in ChildIds(GraphDoc))
            {
                var node = BuildNode(childId, null);
                if (node != null)
                    TopLevel.Add(node);
            }

            ResolveShortIds();
        }

        VfxNode BuildNode(long fileId, VfxNode context)
        {
            if (!ById.TryGetValue(fileId, out var doc))
            {
                Warnings.Add($"dangling child reference {fileId}");
                return null;
            }
            var node = new VfxNode
            {
                FileId = fileId,
                Doc = doc,
                ScriptGuid = ScriptOf(doc),
                Context = context,
            };
            node.Model = Catalog.ResolveVariant(node.ScriptGuid, s => SettingText(doc, s));
            node.TypeName = node.Model?.Type;
            // Anything parented to a context is a block, whether or not the catalog knows it.
            node.Kind = node.Model?.Kind ?? (context != null ? "block" : "unknown");
            // With no catalog at all, every node is unknown; one message covers that.
            if (node.Model == null && !Catalog.IsEmpty)
                Warnings.Add($"unknown node script {node.ScriptGuid} (file id {fileId})");
            Nodes[fileId] = node;

            var label = doc.Body.Scalar("m_Label");
            if (!string.IsNullOrWhiteSpace(label))
                node.Label = label;

            ReadPosition(node, doc.Body.Scalar("m_UIPosition"));
            node.Collapsed = doc.Body.Scalar("m_UICollapsed") == "1";
            node.SuperCollapsed = doc.Body.Scalar("m_UISuperCollapsed") == "1";

            foreach (var slotId in FileIdList(doc, "m_InputSlots"))
                AddSlot(node, slotId, null, true);
            foreach (var slotId in FileIdList(doc, "m_OutputSlots"))
                AddSlot(node, slotId, null, false);

            if (node.Kind == "context")
            {
                foreach (var blockId in ChildIds(doc))
                {
                    var block = BuildNode(blockId, node);
                    if (block != null)
                        node.Blocks.Add(block);
                }
                ReadFlow(node, doc);
                node.Data = ResolveData(node, doc);
            }

            if (node.Kind == "parameter" || IsParameter(node))
            {
                node.ExposedName = doc.Body.Scalar("m_ExposedName");
                node.Exposed = doc.Body.Scalar("m_Exposed") == "1";
                node.IsOutput = doc.Body.Scalar("m_IsOutput") == "1";
                ReadParameterNodes(node, doc);
            }

            return node;
        }

        /// <summary>
        /// Reads a node that was just written into the model, so the same run can wire it without
        /// saving and loading the file again. Everything a fresh load would derive from the
        /// document — its type, its slot tree, its place in the graph — is derived here too.
        /// </summary>
        public void Reindex(VfxNode node)
        {
            node.ScriptGuid = ScriptOf(node.Doc);
            node.Model = Catalog.ResolveVariant(node.ScriptGuid, s => SettingText(node.Doc, s));
            node.TypeName = node.Model?.Type;
            node.SuperCollapsed = node.Doc.Body.Scalar("m_UISuperCollapsed") == "1";
            node.Inputs.Clear();
            node.Outputs.Clear();
            Nodes[node.FileId] = node;
            foreach (var slotId in FileIdList(node.Doc, "m_InputSlots"))
                AddSlot(node, slotId, null, true);
            foreach (var slotId in FileIdList(node.Doc, "m_OutputSlots"))
                AddSlot(node, slotId, null, false);
            if (node.Context == null && !TopLevel.Contains(node))
                TopLevel.Add(node);
            else if (node.Context != null && !node.Context.Blocks.Contains(node))
                node.Context.Blocks.Add(node);
        }

        /// <summary>
        /// Drops a node that was just deleted from the model, so the rest of the run stops seeing
        /// it. Without this a pass that removes a node and then reads the graph again is reading
        /// links into slots the file no longer has.
        /// </summary>
        public void Forget(VfxNode node)
        {
            foreach (var block in node.Blocks.ToList())
                Forget(block);
            foreach (var slot in node.AllInputSlots.Concat(node.AllOutputSlots).ToList())
                Slots.Remove(slot.FileId);
            Nodes.Remove(node.FileId);
            TopLevel.Remove(node);
            node.Context?.Blocks.Remove(node);
        }

        static bool IsParameter(VfxNode node) => node.TypeName != null && node.TypeName.EndsWith(".VFXParameter", StringComparison.Ordinal);

        void AddSlot(VfxNode node, long fileId, VfxSlot parent, bool isInput)
        {
            if (!ById.TryGetValue(fileId, out var doc))
            {
                Warnings.Add($"dangling slot reference {fileId}");
                return;
            }
            var slot = new VfxSlot
            {
                FileId = fileId,
                Doc = doc,
                Owner = node,
                Parent = parent,
                IsInput = isInput,
                Name = doc.Body.Map("m_Property")?.Scalar("name") ?? "",
                TypeName = SerializedTypeName(doc.Body.Map("m_Property")?.Map("m_serializedType")?.Scalar("m_SerializableType")),
            };
            var masterData = doc.Body.Map("m_MasterData");
            var value = masterData?.Map("m_Value")?.Scalar("m_SerializableObject");
            if (!string.IsNullOrWhiteSpace(value))
                slot.Value = value;
            foreach (var linked in FileIdList(doc, "m_LinkedSlots"))
                slot.LinkedSlotIds.Add(linked);

            Slots[fileId] = slot;
            if (parent == null)
                (isInput ? node.Inputs : node.Outputs).Add(slot);
            else
                parent.Children.Add(slot);

            foreach (var childId in ChildIds(doc))
                AddSlot(node, childId, slot, isInput);
        }

        VfxData ResolveData(VfxNode node, YamlDocument doc)
        {
            var id = UnityYaml.FileIdIn(doc.Body.Scalar("m_Data"));
            if (!id.HasValue || id.Value == 0 || !ById.TryGetValue(id.Value, out var dataDoc))
                return null;
            if (!Datas.TryGetValue(id.Value, out var data))
            {
                data = new VfxData
                {
                    FileId = id.Value,
                    Doc = dataDoc,
                    Model = Catalog.ByScript(ScriptOf(dataDoc)),
                    Title = dataDoc.Body.Scalar("title"),
                };
                Datas[id.Value] = data;
            }
            data.Owners.Add(node);
            return data;
        }

        void ReadFlow(VfxNode node, YamlDocument doc)
        {
            var seq = doc.Body.Seq("m_OutputFlowSlot");
            if (seq == null)
                return;
            for (var i = 0; i < seq.Items.Count; i++)
            {
                var lines = seq.Items[i].Split('\n');
                for (var k = 0; k < lines.Length; k++)
                {
                    if (!lines[k].Contains("context:"))
                        continue;
                    var target = UnityYaml.FileIdIn(lines[k]);
                    if (!target.HasValue || target.Value == 0)
                        continue;
                    var slotIndex = 0;
                    for (var j = k + 1; j < lines.Length && !lines[j].Contains("context:"); j++)
                    {
                        var t = lines[j].Trim();
                        if (t.StartsWith("slotIndex:", StringComparison.Ordinal))
                        {
                            int.TryParse(t.Substring("slotIndex:".Length).Trim(), out slotIndex);
                            break;
                        }
                    }
                    node.FlowOut.Add((i, target.Value, slotIndex));
                }
            }
        }

        void ReadParameterNodes(VfxNode node, YamlDocument doc)
        {
            var seq = doc.Body.Seq("m_Nodes");
            if (seq == null)
                return;
            foreach (var item in seq.Items)
            {
                var pn = new VfxParameterNode();
                long? pendingOut = null;
                foreach (var raw in item.Split('\n'))
                {
                    var t = raw.Trim();
                    if (t.StartsWith("- m_Id:", StringComparison.Ordinal) || t.StartsWith("m_Id:", StringComparison.Ordinal))
                    {
                        var v = t.Substring(t.IndexOf("m_Id:", StringComparison.Ordinal) + 5).Trim();
                        if (int.TryParse(v, out var id))
                            pn.Id = id;
                    }
                    else if (t.StartsWith("- outputSlot:", StringComparison.Ordinal) || t.StartsWith("outputSlot:", StringComparison.Ordinal))
                    {
                        pendingOut = UnityYaml.FileIdIn(t);
                    }
                    else if (t.StartsWith("inputSlot:", StringComparison.Ordinal))
                    {
                        var input = UnityYaml.FileIdIn(t);
                        if (pendingOut.HasValue && input.HasValue)
                            pn.Links.Add((pendingOut.Value, input.Value));
                        pendingOut = null;
                    }
                    else if (t.StartsWith("position:", StringComparison.Ordinal))
                    {
                        var (x, y) = ParseVector2(t.Substring("position:".Length));
                        pn.X = x;
                        pn.Y = y;
                    }
                }
                node.ParameterNodes.Add(pn);
            }

            // A parameter's m_UIPosition is always zero. It's drawn once per placement record, so
            // take the first placement's position, or every parameter looks like it's at the origin.
            if (node.ParameterNodes.Count > 0)
            {
                node.X = node.ParameterNodes[0].X;
                node.Y = node.ParameterNodes[0].Y;
            }
        }

        void ResolveShortIds()
        {
            // Local file ids in one file share a long leading run; only the tail tells them apart.
            var table = new ShortIdTable(ShortIdTable.Slice.Tail);
            foreach (var node in AllNodes())
            {
                var kind = node.Kind switch
                {
                    "context" => 'c',
                    "block" => 'b',
                    "parameter" => 'p',
                    _ => 'n',
                };
                if (IsParameter(node))
                    kind = 'p';
                node.ShortId = table.Assign(kind, node.FileId.ToString(CultureInfo.InvariantCulture));
            }
            foreach (var data in Datas.Values)
                data.ShortId = table.Assign('d', data.FileId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The short id a newly added node will most likely have after a reload, without rebuilding
        /// the table. It's wrong when the four-digit tail collides with another node's, since the
        /// table then lengthens both.
        /// </summary>
        public static string ShortIdFor(string kind, long fileId)
        {
            var letter = kind switch
            {
                "context" => 'c',
                "block" => 'b',
                "parameter" => 'p',
                "data" => 'd',
                _ => 'n',
            };
            var text = fileId.ToString(CultureInfo.InvariantCulture);
            return letter + (text.Length <= 4 ? text : text.Substring(text.Length - 4));
        }

        public IEnumerable<VfxNode> AllNodes()
        {
            foreach (var n in TopLevel)
            {
                yield return n;
                foreach (var b in n.Blocks)
                    yield return b;
            }
        }

        public VfxNode NodeByShortId(string shortId) =>
            AllNodes().FirstOrDefault(n => string.Equals(n.ShortId, shortId, StringComparison.OrdinalIgnoreCase));

        public VfxSlot SlotById(long fileId) => Slots.TryGetValue(fileId, out var s) ? s : null;

        // --- YAML helpers -------------------------------------------------------------

        public static string ScriptOf(YamlDocument doc) => UnityYaml.GuidIn(doc.Body.Scalar("m_Script"));

        public static string SettingText(YamlDocument doc, string name) =>
            (doc.Body.Scalar(name) ?? doc.Body.Scalar("m_" + name))?.Trim();

        /// <summary>
        /// A setting's value as text, including shapes that aren't bare scalars. A SerializableType
        /// nests its value one level down, like the type that sets a Sample Graphics Buffer's
        /// stride.
        /// </summary>
        public static string SettingDisplay(YamlDocument doc, string name)
        {
            var scalar = SettingText(doc, name);
            if (!string.IsNullOrEmpty(scalar))
                return scalar;
            var map = doc.Body.Map(name) ?? doc.Body.Map("m_" + name);
            if (map == null)
                return scalar;
            var serializable = map.Scalar("m_SerializableType");
            if (!string.IsNullOrWhiteSpace(serializable))
                return SerializedTypeName(serializable.Trim('"'));
            // A choice-of-strings setting keeps the chosen name here. For a Custom HLSL node, that's
            // the function it calls.
            var selection = map.Scalar("selection");
            return string.IsNullOrWhiteSpace(selection) ? scalar : selection;
        }

        public static List<long> ChildIds(YamlDocument doc) => FileIdList(doc, "m_Children");

        public static List<long> FileIdList(YamlDocument doc, string key)
        {
            var seq = doc.Body.Seq(key);
            if (seq != null)
                return seq.FileIds();
            // An empty list is written as a flow "[]" on the key line, so it parses as a scalar.
            return new List<long>();
        }

        static void ReadPosition(VfxNode node, string raw)
        {
            var (x, y) = ParseVector2(raw);
            node.X = x;
            node.Y = y;
        }

        static (float, float) ParseVector2(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return (0, 0);
            float x = 0, y = 0;
            var mx = System.Text.RegularExpressions.Regex.Match(raw, @"x:\s*(-?[\d.eE+]+)");
            var my = System.Text.RegularExpressions.Regex.Match(raw, @"y:\s*(-?[\d.eE+]+)");
            if (mx.Success)
                float.TryParse(mx.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x);
            if (my.Success)
                float.TryParse(my.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out y);
            return (x, y);
        }

        /// <summary>Strips the assembly qualification Unity writes after the type name.</summary>
        public static string SerializedTypeName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;
            var comma = raw.IndexOf(',');
            var full = (comma >= 0 ? raw.Substring(0, comma) : raw).Trim();
            var dot = full.LastIndexOf('.');
            return dot >= 0 ? full.Substring(dot + 1) : full;
        }
    }
}
