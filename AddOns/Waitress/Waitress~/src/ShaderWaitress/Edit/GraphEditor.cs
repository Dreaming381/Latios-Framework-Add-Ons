using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using ShaderWaitress.Catalog;
using ShaderWaitress.Model;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Edit
{
    /// <summary>
    /// All structural mutation. Nothing here takes a coordinate; layout is applied separately
    /// once the caller is done editing.
    /// </summary>
    public sealed class GraphEditor
    {
        static readonly Regex s_ObjectId = new Regex("[0-9a-f]{32}", RegexOptions.Compiled);

        public ShaderGraphDocument Doc { get; }
        public List<string> Log { get; } = new List<string>();
        public bool StructureChanged { get; private set; }

        public GraphEditor(ShaderGraphDocument doc)
        {
            Doc = doc;
        }

        void Touch() => StructureChanged = true;

        /// <summary>Records a change that does not move any node.</summary>
        public void MarkChanged() => StructureChanged = true;

        // ---------- nodes ----------

        public SgNode AddNode(CatalogEntry entry, string name = null, SgGroup group = null)
        {
            if (entry.Template.Count == 0)
                throw new ShaderWaitressException($"the catalog has no creation template for '{entry.Title ?? entry.TypeName}'");

            var created = Instantiate(entry.Template);
            var nodeEntry = created[0];

            var nodes = RootArray("m_Nodes");
            nodes.Add(UnityJsonWriter.Ref(nodeEntry.ObjectId));

            // The template carries a zero draw state. Left in, it reads as "the Editor placed
            // this at the origin", and the layout treats the node as already positioned. Blocks
            // keep it: their context positions them, and Shader Graph always writes the zero rect.
            if (entry.Descriptor == null && nodeEntry.Node["m_DrawState"] is JsonObject draw)
                draw.Remove("m_Position");

            if (name != null)
                nodeEntry.Edit()["m_Name"] = name;
            if (group != null)
                nodeEntry.Edit()["m_Group"] = UnityJsonWriter.Ref(group.ObjectId);

            if (entry.Descriptor != null)
                AttachBlock(nodeEntry, entry);

            Touch();
            Doc.Rebuild();
            var node = Doc.NodeById(nodeEntry.ObjectId);
            Log.Add($"added {node.ShortId} {(node.IsBlock ? node.BlockDescriptor : node.TypeLabel)}");
            return node;
        }

        /// <summary>Block nodes also have to be listed in the vertex or fragment context.</summary>
        void AttachBlock(MultiJsonEntry nodeEntry, CatalogEntry entry)
        {
            var vertex = string.Equals(entry.Stage, "vertex", StringComparison.OrdinalIgnoreCase);
            var field = vertex ? "m_VertexContext" : "m_FragmentContext";
            if (Doc.RootNode[field] is not JsonObject context)
            {
                context = new JsonObject
                {
                    ["m_Position"] = new JsonObject { ["x"] = 0.0, ["y"] = vertex ? 0.0 : 200.0 },
                    ["m_Blocks"] = new JsonArray(),
                };
                Doc.Raw.Root.Edit()[field] = context;
            }
            if (context["m_Blocks"] is not JsonArray blocks)
            {
                blocks = new JsonArray();
                context["m_Blocks"] = blocks;
            }
            Doc.Raw.Root.Edit();
            blocks.Add(UnityJsonWriter.Ref(nodeEntry.ObjectId));
        }

        /// <summary>
        /// Copies a catalog template into the document with fresh object ids. Ids are rewritten
        /// textually because they are 32-hex tokens that cannot collide with anything else.
        /// </summary>
        List<MultiJsonEntry> Instantiate(IReadOnlyList<string> template)
        {
            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var text in template)
            {
                foreach (Match m in s_ObjectId.Matches(text))
                {
                    if (!map.ContainsKey(m.Value))
                        map[m.Value] = Doc.Raw.NewObjectId();
                }
            }

            var created = new List<MultiJsonEntry>();
            foreach (var text in template)
            {
                var rewritten = s_ObjectId.Replace(text, m => map.TryGetValue(m.Value, out var id) ? id : m.Value);
                var node = JsonNode.Parse(rewritten) as JsonObject
                           ?? throw new ShaderWaitressException("catalog template is not a JSON object");
                var entry = MultiJsonEntry.FromNode(node);
                Doc.Raw.Add(entry);
                created.Add(entry);
            }
            return created;
        }

        public void RemoveNode(SgNode node, bool reconnect)
        {
            if (node.IsBlock)
                DetachBlock(node);

            if (reconnect)
                Reconnect(node);

            Doc.RemoveEdges(e => e.From.Is(node) || e.To.Is(node));

            var nodes = RootArray("m_Nodes");
            RemoveRef(nodes, node.ObjectId);

            foreach (var port in node.Ports)
            {
                if (port.Entry != null)
                    Doc.Raw.Remove(port.Entry.ObjectId);
            }
            Doc.Raw.Remove(node.ObjectId);

            Log.Add($"removed {node.ShortId} {(node.IsBlock ? node.BlockDescriptor : node.TypeLabel)}");
            Touch();
            Doc.Rebuild();
        }

        /// <summary>Splices this node's first input through to each of its consumers.</summary>
        void Reconnect(SgNode node)
        {
            var incoming = Doc.EdgesIn(node).ToList();
            var outgoing = Doc.EdgesOut(node).ToList();
            if (incoming.Count == 0 || outgoing.Count == 0)
                return;
            var source = incoming[0];
            foreach (var e in outgoing)
            {
                Doc.RemoveEdges(x => x.To.Is(e.To) && x.ToSlot == e.ToSlot);
                Doc.AddEdge(new SgEdge { From = source.From, FromSlot = source.FromSlot, To = e.To, ToSlot = e.ToSlot });
                Log.Add($"reconnected {source.From.ShortId} -> {e.To.ShortId}");
            }
        }

        void DetachBlock(SgNode node)
        {
            foreach (var field in new[] { "m_VertexContext", "m_FragmentContext" })
            {
                if (Doc.RootNode[field] is JsonObject context && context["m_Blocks"] is JsonArray blocks)
                {
                    if (RemoveRef(blocks, node.ObjectId))
                        Doc.Raw.Root.Edit();
                }
            }
        }

        public SgNode ReplaceNode(SgNode node, CatalogEntry entry, IReadOnlyDictionary<string, string> portMap, out List<string> dropped)
        {
            dropped = new List<string>();
            var incoming = Doc.EdgesIn(node).ToList();
            var outgoing = Doc.EdgesOut(node).ToList();
            var group = Doc.GroupById(node.GroupId);
            var oldPorts = node.Ports.ToList();
            var oldId = node.ShortId;

            var replacement = AddNode(entry, null, group);

            foreach (var e in incoming)
            {
                var oldPort = oldPorts.FirstOrDefault(p => p.Id == e.ToSlot && p.IsInput);
                var target = MatchPort(replacement, oldPort, portMap, input: true);
                if (target == null)
                {
                    dropped.Add($"{e.From.ShortId} -> {oldId}.{oldPort?.Name ?? e.ToSlot.ToString(CultureInfo.InvariantCulture)}");
                    continue;
                }
                Connect(e.From, e.FromSlot, replacement, target.Id);
            }

            foreach (var e in outgoing)
            {
                var oldPort = oldPorts.FirstOrDefault(p => p.Id == e.FromSlot && !p.IsInput);
                var source = MatchPort(replacement, oldPort, portMap, input: false);
                if (source == null)
                {
                    dropped.Add($"{oldId}.{oldPort?.Name ?? e.FromSlot.ToString(CultureInfo.InvariantCulture)} -> {e.To.ShortId}");
                    continue;
                }
                Connect(replacement, source.Id, e.To, e.ToSlot);
            }

            RemoveNode(node, reconnect: false);
            Doc.Rebuild();
            return Doc.NodeById(replacement.ObjectId);
        }

        static SgPort MatchPort(SgNode node, SgPort oldPort, IReadOnlyDictionary<string, string> portMap, bool input)
        {
            if (oldPort == null)
                return null;
            if (portMap != null && portMap.TryGetValue(oldPort.Name, out var mapped))
                return node.FindPort(mapped, input);
            return node.FindPort(oldPort.Name, input) ??
                   node.Ports.FirstOrDefault(p => p.IsInput == input && p.Id == oldPort.Id);
        }

        // ---------- wiring ----------

        public void Connect(SgNode from, int fromSlot, SgNode to, int toSlot)
        {
            // Shader Graph allows exactly one edge into an input port.
            Doc.RemoveEdges(e => e.To.Is(to) && e.ToSlot == toSlot);
            if (Doc.Edges.Any(e => e.From.Is(from) && e.FromSlot == fromSlot && e.To.Is(to) && e.ToSlot == toSlot))
                return;
            Doc.AddEdge(new SgEdge { From = from, FromSlot = fromSlot, To = to, ToSlot = toSlot });
            Touch();
        }

        public void Connect(SgPort source, SgPort target)
        {
            if (source.IsInput)
                throw new ShaderWaitressException($"{source} is an input; the source of a wire must be an output");
            if (!target.IsInput)
                throw new ShaderWaitressException($"{target} is an output; the destination of a wire must be an input");
            if (source.Node == target.Node)
                throw new ShaderWaitressException("a node cannot be wired to itself");
            Connect(source.Node, source.Id, target.Node, target.Id);
            Log.Add($"wired {source.Node.ShortId}.{source.Name} -> {target.Node.ShortId}.{target.Name}");
        }

        public int Disconnect(Func<SgEdge, bool> predicate)
        {
            var count = Doc.RemoveEdges(predicate);
            if (count > 0)
                Touch();
            return count;
        }

        // ---------- values ----------

        public void SetPortValue(SgPort port, string text)
        {
            if (!port.IsInput)
                throw new ShaderWaitressException($"{port} is an output and has no editable value");
            var entry = port.Entry ?? MaterializeSlot(port);
            var kind = port.Kind;
            var field = SlotTypes.ValueFieldOf(port.SlotTypeName)
                        ?? throw new ShaderWaitressException($"port '{port.Name}' holds a {SlotTypes.Label(kind)} which cannot be set from the command line");
            entry.Edit()[field] = SlotTypes.ParseValue(kind, text);
            Log.Add($"set {port.Node.ShortId}.{port.Name} = {text}");
            Touch();
        }

        /// <summary>
        /// Sets one of a node's body controls — a dropdown or toggle that no wire reveals and
        /// that changes what the shader computes.
        /// </summary>
        public void SetNodeSetting(SgNode node, string name, string value)
        {
            var setting = NodeSettings.Require(node, name);
            setting.Set(value);
            Log.Add($"set {node.ShortId}.{setting.Name} = {setting.Display}");
            RenameCustomFunction(node, setting);
            Touch();
        }

        /// <summary>
        /// A Custom Function node titles itself after its function. Shader Graph does this on
        /// load anyway; doing it here means the file, and everything the tool prints, already
        /// says what the Editor will show.
        /// </summary>
        static void RenameCustomFunction(SgNode node, SgSetting setting)
        {
            if (node.TypeName != "UnityEditor.ShaderGraph.CustomFunctionNode" || setting.Field != "m_FunctionName")
                return;
            var function = Json.Text(setting.Raw);
            node.Name = string.IsNullOrEmpty(function) || function == "Custom Function"
                ? "Custom Function"
                : function + " (Custom Function)";
        }

        /// <summary>
        /// Writes out a slot the node was relying on Shader Graph to rebuild, so a value can be
        /// stored on it.
        /// </summary>
        MultiJsonEntry MaterializeSlot(SgPort port)
        {
            var catalogEntry = Doc.Catalog?.Find(port.Node.TypeName);
            var template = catalogEntry?.Template;
            if (template != null)
            {
                foreach (var text in template.Skip(1))
                {
                    var probe = JsonNode.Parse(text) as JsonObject;
                    if (probe == null)
                        continue;
                    var id = ShaderWaitress.Serialization.Json.Int(probe["m_Id"], -1);
                    var isInput = ShaderWaitress.Serialization.Json.Int(probe["m_SlotType"], 0) == 0;
                    if (id != port.Id || isInput != port.IsInput)
                        continue;
                    var objectId = Doc.Raw.NewObjectId();
                    probe["m_ObjectId"] = objectId;
                    var entry = MultiJsonEntry.FromNode(probe);
                    Doc.Raw.Add(entry);
                    AppendSlot(port.Node, objectId);
                    port.Entry = entry;
                    port.Node.InvalidatePorts();
                    return entry;
                }
            }
            throw new ShaderWaitressException($"port '{port.Name}' on {port.Node.ShortId} has no serialized slot and the catalog has no template for it");
        }

        void AppendSlot(SgNode node, string slotObjectId)
        {
            var json = node.Entry.Edit();
            if (json["m_Slots"] is not JsonArray slots)
            {
                slots = new JsonArray();
                json["m_Slots"] = slots;
            }
            slots.Add(UnityJsonWriter.Ref(slotObjectId));
        }

        // ---------- groups ----------

        public SgGroup AddGroup(string title)
        {
            var entry = Doc.Raw.AddObject("UnityEditor.ShaderGraph.GroupData");
            var json = entry.Edit();
            json["m_Title"] = title;
            json["m_Position"] = new JsonObject { ["x"] = 0.0, ["y"] = 0.0 };
            RootArray("m_GroupDatas").Add(UnityJsonWriter.Ref(entry.ObjectId));
            Touch();
            Doc.Rebuild();
            var group = Doc.GroupById(entry.ObjectId);
            Log.Add($"added group {group.ShortId} \"{title}\"");
            return group;
        }

        public void RemoveGroup(SgGroup group, bool removeMembers)
        {
            var members = Doc.Nodes.Where(n => n.GroupId == group.ObjectId).ToList();
            if (removeMembers)
            {
                foreach (var node in members)
                    RemoveNode(node, reconnect: false);
            }
            else
            {
                foreach (var node in members)
                    node.GroupId = string.Empty;
            }
            RemoveRef(RootArray("m_GroupDatas"), group.ObjectId);
            Doc.Raw.Remove(group.ObjectId);
            Log.Add($"removed group \"{group.Title}\"" + (removeMembers ? $" and {members.Count} node(s)" : string.Empty));
            Touch();
            Doc.Rebuild();
        }

        public void Assign(SgNode node, SgGroup group)
        {
            node.GroupId = group?.ObjectId ?? string.Empty;
            Log.Add(group == null ? $"ungrouped {node.ShortId}" : $"moved {node.ShortId} into \"{group.Title}\"");
            Touch();
        }

        // ---------- properties ----------

        public SgProperty AddProperty(CatalogEntry entry, string name, string referenceName, string value)
        {
            if (entry.Template.Count == 0)
                throw new ShaderWaitressException($"the catalog has no template for property type '{entry.Title ?? entry.TypeName}'");
            var created = Instantiate(entry.Template);
            var propEntry = created[0];
            var json = propEntry.Edit();
            json["m_Name"] = name;
            if (json.ContainsKey("m_DefaultReferenceName"))
                json["m_DefaultReferenceName"] = string.Empty;
            json["m_OverrideReferenceName"] = referenceName ?? DefaultReferenceName(name);
            if (json.ContainsKey("m_Guid"))
                json["m_Guid"] = new JsonObject { ["m_GuidSerialized"] = Guid.NewGuid().ToString("D") };

            RootArray("m_Properties").Add(UnityJsonWriter.Ref(propEntry.ObjectId));
            AddToFirstCategory(propEntry.ObjectId);

            Touch();
            Doc.Rebuild();
            var property = Doc.PropertyById(propEntry.ObjectId);
            if (value != null)
                SetPropertyValue(property, value);
            Log.Add($"added property {property.ShortId} {property.TypeLabel} \"{name}\"");
            return property;
        }

        public static string DefaultReferenceName(string displayName)
        {
            var sb = new StringBuilder("_");
            foreach (var c in displayName ?? string.Empty)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c is ' ' or '_' or '-')
                    sb.Append('_');
            }
            return sb.Length == 1 ? "_Property" : sb.ToString();
        }

        void AddToFirstCategory(string propertyObjectId)
        {
            var categories = Doc.RefList(Doc.RootNode, "m_CategoryData").ToList();
            if (categories.Count == 0)
            {
                var category = Doc.Raw.AddObject("UnityEditor.ShaderGraph.CategoryData");
                var json = category.Edit();
                json["m_Name"] = string.Empty;
                json["m_ChildObjectList"] = new JsonArray { UnityJsonWriter.Ref(propertyObjectId) };
                RootArray("m_CategoryData").Add(UnityJsonWriter.Ref(category.ObjectId));
                return;
            }
            var first = Doc.Raw.Find(categories[0]);
            if (first == null)
                return;
            var list = first.Edit()["m_ChildObjectList"] as JsonArray;
            if (list == null)
            {
                list = new JsonArray();
                first.Edit()["m_ChildObjectList"] = list;
            }
            list.Add(UnityJsonWriter.Ref(propertyObjectId));
        }

        public void RemoveProperty(SgProperty property, bool removeNodes)
        {
            var readers = Doc.Nodes.Where(n => n.IsProperty && n.PropertyId == property.ObjectId).ToList();
            if (readers.Count > 0 && !removeNodes)
                throw new ShaderWaitressException($"{property.ShortId} is read by {readers.Count} node(s); pass --with-nodes to remove them too");
            foreach (var node in readers)
                RemoveNode(node, reconnect: false);

            RemoveRef(RootArray("m_Properties"), property.ObjectId);
            foreach (var categoryId in Doc.RefList(Doc.RootNode, "m_CategoryData").ToList())
            {
                var category = Doc.Raw.Find(categoryId);
                if (category?.Node["m_ChildObjectList"] is JsonArray list && RemoveRef(list, property.ObjectId))
                    category.Edit();
            }
            Doc.Raw.Remove(property.ObjectId);
            Log.Add($"removed property \"{property.Name}\"");
            Touch();
            Doc.Rebuild();
        }

        public void SetPropertyValue(SgProperty property, string text)
        {
            var json = property.Entry.Edit();
            var current = json["m_Value"];
            switch (property.TypeLabel)
            {
                case "Float":
                    json["m_Value"] = SlotTypes.ParseDouble(text);
                    break;
                case "Boolean":
                    json["m_Value"] = text is "1" or "true" or "True" or "on";
                    break;
                case "Vector2":
                case "Vector3":
                case "Vector4":
                    json["m_Value"] = SlotTypes.ParseValue(SlotKind.Vector4, text);
                    break;
                case "Color":
                    json["m_Value"] = ParseColor(text);
                    break;
                default:
                    throw new ShaderWaitressException($"cannot set a value on a {property.TypeLabel} property from the command line");
            }
            Log.Add($"set property \"{property.Name}\" = {text}");
            Touch();
        }

        static JsonNode ParseColor(string text)
        {
            var pieces = text.Trim().TrimStart('(', '[').TrimEnd(')', ']')
                .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var names = new[] { "r", "g", "b", "a" };
            var obj = new JsonObject();
            for (var i = 0; i < 4; i++)
                obj[names[i]] = i < pieces.Length ? SlotTypes.ParseDouble(pieces[i]) : (i == 3 ? 1.0 : 0.0);
            return obj;
        }

        // ---------- ports ----------

        /// <summary>
        /// Writes a new slot object and lists it on the node. Only the fields that differ from
        /// the slot class's own defaults are written: Shader Graph deserializes with
        /// FromJsonOverwrite onto a default-constructed slot, so anything absent keeps its
        /// code default rather than becoming zero.
        /// </summary>
        public SgPort AddPort(SgNode node, string name, string shaderName, bool input, SlotKind kind,
            string slotTypeName, int? explicitId, string value)
        {
            var id = explicitId ?? NextSlotId(node);
            if (node.Ports.Any(p => p.Id == id))
                throw new ShaderWaitressException($"{node.ShortId} already has a port with id {id}");

            var slot = Doc.Raw.AddObject(slotTypeName);
            var json = slot.Edit();
            json["m_Id"] = id;
            json["m_DisplayName"] = name;
            json["m_SlotType"] = input ? 0 : 1;
            json["m_Hidden"] = false;
            json["m_ShaderOutputName"] = shaderName ?? name.Replace(" ", string.Empty);
            json["m_StageCapability"] = 3;

            var valueField = SlotTypes.ValueFieldOf(slotTypeName);
            if (value != null && valueField != null)
                json[valueField] = SlotTypes.ParseValue(kind, value);

            var nodeJson = node.Entry.Edit();
            if (nodeJson["m_Slots"] is not JsonArray slots)
            {
                slots = new JsonArray();
                nodeJson["m_Slots"] = slots;
            }
            slots.Add(UnityJsonWriter.Ref(slot.ObjectId));

            node.InvalidatePorts();
            Touch();
            Doc.Rebuild();
            Log.Add($"added {(input ? "input" : "output")} {name} ({SlotTypes.Label(kind)}) to {node.ShortId}");
            return Doc.NodeById(node.ObjectId)?.Ports.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>Slot ids are unique per node across both directions.</summary>
        static int NextSlotId(SgNode node)
        {
            var used = node.Ports.Select(p => p.Id).ToList();
            return used.Count == 0 ? 0 : used.Max() + 1;
        }

        public void RemovePort(SgNode node, SgPort port)
        {
            if (port.Entry == null)
                throw new ShaderWaitressException(
                    $"{node.ShortId}.{port.Name} is built by the node type in code, so it cannot be removed");

            Doc.RemoveEdges(e => (e.To.Is(node) && e.ToSlot == port.Id) || (e.From.Is(node) && e.FromSlot == port.Id));

            if (node.Entry.Edit()["m_Slots"] is JsonArray slots)
                RemoveRef(slots, port.Entry.ObjectId);
            Doc.Raw.Remove(port.Entry.ObjectId);

            node.InvalidatePorts();
            Log.Add($"removed port {port.Name} from {node.ShortId}");
            Touch();
            Doc.Rebuild();
        }

        // ---------- keywords ----------

        public SgKeyword AddKeyword(CatalogEntry entry, string name, string referenceName,
            IReadOnlyList<string> entries, string definition, string scope, string defaultValue)
        {
            if (entry.Template.Count == 0)
                throw new ShaderWaitressException($"the catalog has no template for a {entry.Title} keyword");
            var created = Instantiate(entry.Template);
            var json = created[0].Edit();

            json["m_Name"] = name;
            json["m_DefaultReferenceName"] = string.Empty;
            json["m_OverrideReferenceName"] = referenceName ?? KeywordReferenceName(name);
            if (json.ContainsKey("m_Guid"))
                json["m_Guid"] = new JsonObject { ["m_GuidSerialized"] = Guid.NewGuid().ToString("D") };

            var isEnum = string.Equals(entry.Title, "Enum", StringComparison.OrdinalIgnoreCase);
            if (isEnum)
            {
                if (entries == null || entries.Count < 2)
                    throw new ShaderWaitressException("an enum keyword needs at least two entries, e.g. --entries Low,High");
                var array = new JsonArray();
                for (var i = 0; i < entries.Count; i++)
                {
                    array.Add(new JsonObject
                    {
                        ["id"] = i + 1,
                        ["displayName"] = entries[i],
                        ["referenceName"] = KeywordEntryReference(entries[i]),
                    });
                }
                json["m_Entries"] = array;
            }
            else if (entries != null && entries.Count > 0)
            {
                throw new ShaderWaitressException("only an enum keyword has entries");
            }

            if (definition != null)
                json["m_KeywordDefinition"] = ParseChoice(definition, "definition", "shaderfeature", "multicompile", "predefined");
            if (scope != null)
                json["m_KeywordScope"] = ParseChoice(scope, "scope", "local", "global");
            if (defaultValue != null)
                json["m_Value"] = ParseKeywordDefault(defaultValue, isEnum, entries);

            RootArray("m_Keywords").Add(UnityJsonWriter.Ref(created[0].ObjectId));
            AddToFirstCategory(created[0].ObjectId);

            Touch();
            Doc.Rebuild();
            var keyword = Doc.Keywords.FirstOrDefault(k => k.ObjectId == created[0].ObjectId);
            Log.Add($"added keyword {keyword?.ShortId} \"{name}\"");
            return keyword;
        }

        static int ParseChoice(string text, string what, params string[] options)
        {
            var index = Array.FindIndex(options, o => string.Equals(o, text, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                throw new ShaderWaitressException($"'{text}' is not a {what}. Options: {string.Join(", ", options)}");
            return index;
        }

        static int ParseKeywordDefault(string text, bool isEnum, IReadOnlyList<string> entries)
        {
            if (!isEnum)
                return text is "1" or "on" or "true" or "yes" ? 1 : 0;
            if (entries != null)
            {
                var index = entries.ToList().FindIndex(e => string.Equals(e, text, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                    return index;
            }
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var raw))
                return raw;
            throw new ShaderWaitressException($"'{text}' is not one of the keyword's entries");
        }

        public static string KeywordReferenceName(string displayName)
        {
            var sb = new StringBuilder("_");
            foreach (var c in displayName ?? string.Empty)
                sb.Append(char.IsLetterOrDigit(c) ? char.ToUpperInvariant(c) : '_');
            return sb.Length == 1 ? "_KEYWORD" : sb.ToString();
        }

        static string KeywordEntryReference(string displayName)
        {
            var sb = new StringBuilder();
            foreach (var c in displayName ?? string.Empty)
                sb.Append(char.IsLetterOrDigit(c) ? char.ToUpperInvariant(c) : '_');
            return sb.Length == 0 ? "ENTRY" : sb.ToString();
        }

        public void RemoveKeyword(SgKeyword keyword, bool removeNodes)
        {
            var readers = Doc.Nodes.Where(n => n.IsKeyword && n.KeywordId == keyword.ObjectId).ToList();
            if (readers.Count > 0 && !removeNodes)
                throw new ShaderWaitressException($"{keyword.ShortId} is read by {readers.Count} node(s); pass --with-nodes to remove them too");
            foreach (var node in readers)
                RemoveNode(node, reconnect: false);

            RemoveRef(RootArray("m_Keywords"), keyword.ObjectId);
            foreach (var categoryId in Doc.RefList(Doc.RootNode, "m_CategoryData").ToList())
            {
                var category = Doc.Raw.Find(categoryId);
                if (category?.Node["m_ChildObjectList"] is JsonArray list && RemoveRef(list, keyword.ObjectId))
                    category.Edit();
            }
            Doc.Raw.Remove(keyword.ObjectId);
            Log.Add($"removed keyword \"{keyword.Name}\"");
            Touch();
            Doc.Rebuild();
        }

        // ---------- helpers ----------

        JsonArray RootArray(string field)
        {
            var root = Doc.Raw.Root.Edit();
            if (root[field] is not JsonArray arr)
            {
                arr = new JsonArray();
                root[field] = arr;
            }
            return arr;
        }

        static bool RemoveRef(JsonArray array, string objectId)
        {
            for (var i = array.Count - 1; i >= 0; i--)
            {
                if (UnityJsonWriter.RefId(array[i]) == objectId)
                {
                    array.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
