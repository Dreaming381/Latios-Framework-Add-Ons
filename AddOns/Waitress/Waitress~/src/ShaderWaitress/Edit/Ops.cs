using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ShaderWaitress.Catalog;
using ShaderWaitress.Cli;
using ShaderWaitress.Model;
using ShaderWaitress.Query;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Edit
{
    /// <summary>
    /// The mutation verbs, in one place. The CLI wraps each of these with load/save; the batch
    /// interpreter calls them repeatedly against a single in-memory document so a whole edit
    /// session is one file write.
    /// </summary>
    public static class Ops
    {
        /// <summary>
        /// Adds one output block by descriptor, e.g. "SurfaceDescription.Alpha". The catalog
        /// carries the same template the Editor writes, including the slot, and the block is
        /// listed in the vertex or fragment context according to its stage.
        /// </summary>
        public static SgNode BlockAdd(ShaderGraphDocument doc, GraphEditor editor, string descriptor)
        {
            var entry = NodeCatalog.Shared.Blocks.FirstOrDefault(b =>
                            string.Equals(b.Descriptor, descriptor, StringComparison.OrdinalIgnoreCase)) ??
                        NodeCatalog.Shared.Blocks.FirstOrDefault(b =>
                            b.Descriptor != null &&
                            b.Descriptor.EndsWith("." + descriptor, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                throw new ShaderWaitressException($"no block descriptor '{descriptor}'. Known: " +
                    string.Join(", ", NodeCatalog.Shared.Blocks.Select(b => b.Descriptor).OrderBy(d => d)));
            }

            var existing = doc.Nodes.FirstOrDefault(n => n.IsBlock &&
                string.Equals(n.BlockDescriptor, entry.Descriptor, StringComparison.Ordinal));
            if (existing != null)
            {
                editor.Log.Add($"{entry.Descriptor} is already there as {existing.ShortId}");
                return existing;
            }

            return editor.AddNode(entry);
        }

        public static void BlockRemove(ShaderGraphDocument doc, GraphEditor editor, IReadOnlyList<string> rest)
        {
            foreach (var node in Select(doc, rest, "block rm"))
            {
                if (!node.IsBlock)
                    throw new ShaderWaitressException($"{node.ShortId} is a {node.TypeLabel}, not a block. Use 'node rm'.");
                editor.RemoveNode(node, reconnect: false);
            }
        }

        public static SgNode NodeAdd(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            if (rest.Count < 1)
                throw new ShaderWaitressException("node add needs a type, e.g. 'node add Multiply'");
            var entry = NodeCatalog.Shared.Require(rest[0]);
            var group = args.Has("group") ? doc.ResolveGroup(args.Get("group")) : null;
            var node = BindReferences(doc, editor, args, editor.AddNode(entry, args.Get("name"), group));
            if (args.Get("preview") is { } previewText)
                SetPreview(editor, node, previewText);
            ApplySets(editor, node, args);
            ApplyWires(doc, editor, node, args);
            return node;
        }

        /// <summary>
        /// Applies --property, --keyword and --subgraph. A node of those types has no ports at
        /// all until it is bound, so anything that goes on to resolve a port by name has to do
        /// this first.
        /// </summary>
        public static SgNode BindReferences(ShaderGraphDocument doc, GraphEditor editor, Args args, SgNode node)
        {
            var propertyHandle = args.Get("property");
            if (propertyHandle != null)
            {
                BindProperty(doc, editor, node, doc.ResolveProperty(propertyHandle));
                node = doc.NodeById(node.ObjectId);
            }

            var keywordHandle = args.Get("keyword");
            if (keywordHandle != null)
            {
                BindKeyword(doc, editor, node, doc.ResolveKeyword(keywordHandle));
                node = doc.NodeById(node.ObjectId);
            }

            var subGraphPath = args.Get("subgraph");
            if (subGraphPath != null)
            {
                BindSubGraph(doc, editor, node, subGraphPath);
                node = doc.NodeById(node.ObjectId);
            }

            return node;
        }

        /// <summary>
        /// Points a Property node at a blackboard property and writes the output slot Shader
        /// Graph would build from that property's type.
        /// </summary>
        public static void BindProperty(ShaderGraphDocument doc, GraphEditor editor, SgNode node, SgProperty property)
        {
            if (!node.IsProperty)
                throw new ShaderWaitressException($"{node.ShortId} is a {node.TypeLabel}, not a Property node");

            foreach (var port in node.Ports.ToList())
            {
                if (port.Entry != null)
                    doc.Raw.Remove(port.Entry.ObjectId);
            }

            var slotType = SlotTypes.SlotTypeForKind(PropertySlotKind(property)) ?? "UnityEditor.ShaderGraph.Vector1MaterialSlot";
            var slot = doc.Raw.AddObject(slotType);
            var slotJson = slot.Edit();
            slotJson["m_Id"] = 0;
            slotJson["m_DisplayName"] = property.Name;
            slotJson["m_SlotType"] = 1;
            slotJson["m_Hidden"] = false;
            slotJson["m_ShaderOutputName"] = "Out";
            slotJson["m_StageCapability"] = 3;

            var json = node.Entry.Edit();
            json["m_Property"] = UnityJsonWriter.Ref(property.ObjectId);
            json["m_Slots"] = new JsonArray { UnityJsonWriter.Ref(slot.ObjectId) };
            node.InvalidatePorts();
            doc.Rebuild();
            editor.Log.Add($"bound {node.ShortId} to property \"{property.Name}\"");
        }

        static SlotKind PropertySlotKind(SgProperty property) => property.TypeLabel switch
        {
            "Float" => SlotKind.Float,
            "Vector2" => SlotKind.Vector2,
            "Vector3" => SlotKind.Vector3,
            "Vector4" or "Color" => SlotKind.Vector4,
            "Boolean" => SlotKind.Boolean,
            "Texture2D" => SlotKind.Texture2D,
            "Texture2DArray" => SlotKind.Texture2DArray,
            "Texture3D" => SlotKind.Texture3D,
            "Cubemap" => SlotKind.Cubemap,
            "SamplerState" => SlotKind.SamplerState,
            "Gradient" => SlotKind.Gradient,
            "Matrix2" => SlotKind.Matrix2,
            "Matrix3" => SlotKind.Matrix3,
            "Matrix4" => SlotKind.Matrix4,
            _ => SlotKind.Float,
        };

        /// <summary>
        /// Points a Keyword node at a keyword and writes the slots Shader Graph would build
        /// from it: Out plus On/Off for a boolean, or one input per entry for an enum.
        /// </summary>
        public static void BindKeyword(ShaderGraphDocument doc, GraphEditor editor, SgNode node, SgKeyword keyword)
        {
            if (!node.IsKeyword)
                throw new ShaderWaitressException($"{node.ShortId} is a {node.TypeLabel}, not a Keyword node");

            foreach (var port in node.Ports.ToList())
            {
                if (port.Entry != null)
                    doc.Raw.Remove(port.Entry.ObjectId);
            }

            var slots = new JsonArray();
            slots.Add(UnityJsonWriter.Ref(KeywordSlot(doc, 0, "Out", "Out", input: false)));
            if (keyword.KeywordType == 0)
            {
                slots.Add(UnityJsonWriter.Ref(KeywordSlot(doc, 1, "On", "On", input: true)));
                slots.Add(UnityJsonWriter.Ref(KeywordSlot(doc, 2, "Off", "Off", input: true)));
            }
            else
            {
                foreach (var (id, display, reference) in keyword.EntryDetails)
                    slots.Add(UnityJsonWriter.Ref(KeywordSlot(doc, id, display, reference, input: true)));
            }

            var json = node.Entry.Edit();
            json["m_Keyword"] = UnityJsonWriter.Ref(keyword.ObjectId);
            json["m_Slots"] = slots;
            json["m_Name"] = keyword.Name;
            node.InvalidatePorts();
            doc.Rebuild();
            editor.Log.Add($"bound {node.ShortId} to keyword \"{keyword.Name}\"");
        }

        static string KeywordSlot(ShaderGraphDocument doc, int id, string display, string reference, bool input)
        {
            var slot = doc.Raw.AddObject("UnityEditor.ShaderGraph.DynamicVectorMaterialSlot");
            var json = slot.Edit();
            json["m_Id"] = id;
            json["m_DisplayName"] = display;
            json["m_SlotType"] = input ? 0 : 1;
            json["m_Hidden"] = false;
            json["m_ShaderOutputName"] = reference;
            json["m_StageCapability"] = 3;
            json["m_Value"] = new JsonObject { ["x"] = 0.0, ["y"] = 0.0, ["z"] = 0.0, ["w"] = 0.0 };
            json["m_DefaultValue"] = new JsonObject { ["x"] = 0.0, ["y"] = 0.0, ["z"] = 0.0, ["w"] = 0.0 };
            json["m_Labels"] = new JsonArray();
            return slot.ObjectId;
        }

        /// <summary>
        /// Points a SubGraph node at a .shadersubgraph file. Unity identifies the asset by the
        /// guid in its .meta, and the node's ports mirror the subgraph's own properties and
        /// output slots, so those are read straight out of the referenced file.
        /// </summary>
        public static void BindSubGraph(ShaderGraphDocument doc, GraphEditor editor, SgNode node, string subGraphPath)
        {
            if (node.TypeName != "UnityEditor.ShaderGraph.SubGraphNode")
                throw new ShaderWaitressException($"{node.ShortId} is a {node.TypeLabel}, not a SubGraph node");
            if (!System.IO.File.Exists(subGraphPath))
                throw new ShaderWaitressException($"no such subgraph: {subGraphPath}");

            var guid = ReadAssetGuid(subGraphPath);
            var sub = ShaderGraphDocument.Load(subGraphPath, doc.Catalog);
            if (!sub.IsSubGraph)
                throw new ShaderWaitressException($"{subGraphPath} is a shader graph, not a subgraph");

            foreach (var port in node.Ports.ToList())
            {
                if (port.Entry != null)
                    doc.Raw.Remove(port.Entry.ObjectId);
            }

            var slots = new JsonArray();
            var outputNode = sub.NodeById(UnityJsonWriter.RefId(sub.RootNode["m_OutputNode"]));
            if (outputNode != null)
            {
                foreach (var port in outputNode.Inputs)
                {
                    slots.Add(UnityJsonWriter.Ref(CopySlot(doc, port, id: port.Id, input: false,
                        display: port.Name, reference: port.ShaderOutputName ?? port.Name)));
                }
            }

            // Subgraph inputs are its exposed properties, in blackboard order, after the outputs.
            var nextId = 1000;
            foreach (var property in sub.Properties)
            {
                // A promoted property is declared on the parent shader instead of arriving
                // through the node, so it has no port here.
                if (property.Promoted)
                    continue;
                var kind = PropertySlotKind(property);
                var slotType = SlotTypes.SlotTypeForKind(kind) ?? "UnityEditor.ShaderGraph.Vector1MaterialSlot";
                var slot = doc.Raw.AddObject(slotType);
                var json = slot.Edit();
                json["m_Id"] = nextId++;
                json["m_DisplayName"] = property.Name;
                json["m_SlotType"] = 0;
                json["m_Hidden"] = false;
                json["m_ShaderOutputName"] = property.ReferenceName;
                json["m_StageCapability"] = 3;
                slots.Add(UnityJsonWriter.Ref(slot.ObjectId));
            }

            var nodeJson = node.Entry.Edit();
            nodeJson["m_SerializedSubGraph"] =
                "{\"subGraph\":{\"fileID\":11400000,\"guid\":\"" + guid + "\",\"type\":3}}";
            nodeJson["m_Slots"] = slots;
            nodeJson["m_Name"] = System.IO.Path.GetFileNameWithoutExtension(subGraphPath);
            node.InvalidatePorts();
            doc.Rebuild();
            editor.Log.Add($"bound {node.ShortId} to subgraph \"{System.IO.Path.GetFileName(subGraphPath)}\"");
        }

        static string CopySlot(ShaderGraphDocument doc, SgPort source, int id, bool input, string display, string reference)
        {
            var slot = doc.Raw.AddObject(source.SlotTypeName ?? "UnityEditor.ShaderGraph.Vector4MaterialSlot");
            var json = slot.Edit();
            json["m_Id"] = id;
            json["m_DisplayName"] = display;
            json["m_SlotType"] = input ? 0 : 1;
            json["m_Hidden"] = false;
            json["m_ShaderOutputName"] = reference;
            json["m_StageCapability"] = 3;
            return slot.ObjectId;
        }

        /// <summary>Unity's asset guid, which lives in the sidecar .meta file.</summary>
        static string ReadAssetGuid(string assetPath)
        {
            var meta = assetPath + ".meta";
            if (!System.IO.File.Exists(meta))
                throw new ShaderWaitressException(
                    $"{assetPath} has no .meta file, so Unity has not imported it yet and it has no guid to reference");
            foreach (var line in System.IO.File.ReadLines(meta))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("guid:", StringComparison.Ordinal))
                    return trimmed.Substring(5).Trim();
            }
            throw new ShaderWaitressException($"{meta} has no guid");
        }

        public static SgKeyword KeywordAdd(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            var kind = rest.Count > 0 ? rest[0] : "boolean";
            var template = NodeCatalog.Shared.KeywordTemplates
                .FirstOrDefault(k => string.Equals(k.Title, kind, StringComparison.OrdinalIgnoreCase));
            if (template == null)
                throw new ShaderWaitressException($"unknown keyword kind '{kind}'. Known: " +
                    string.Join(", ", NodeCatalog.Shared.KeywordTemplates.Select(k => k.Title.ToLowerInvariant())));

            var name = args.Get("name") ?? rest.ElementAtOrDefault(1)
                       ?? throw new ShaderWaitressException("keyword add needs --name");
            var entries = args.Has("entries")
                ? args.Get("entries").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : null;
            var keyword = editor.AddKeyword(template, name, args.Get("ref"), entries,
                args.Get("definition"), args.Get("scope"), args.Get("default"));
            if (args.Has("allow-definition-override"))
                SetAllowDefinitionOverride(editor, keyword, args.Get("allow-definition-override"));
            if (args.Has("promote"))
                SetPromoted(doc, editor, keyword, args.Get("promote"));
            return keyword;
        }

        /// <summary>
        /// While "Allow Definition Override" is on, Shader Graph compiles the keyword to a
        /// runtime ternary rather than an #if. It's on by default, so a compile-time branch has
        /// to turn it off.
        /// </summary>
        public static void SetAllowDefinitionOverride(GraphEditor editor, SgKeyword keyword, string text)
        {
            var on = OnOff(text, "allow-definition-override");
            keyword.AllowDefinitionOverride = on;
            editor.Log.Add($"keyword \"{keyword.Name}\" allow-definition-override = {(on ? "on" : "off")}" +
                           (on ? " (compiles to a runtime branch)" : " (compiles to an #if)"));
            editor.MarkChanged();
        }

        public static void KeywordSet(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            if (rest.Count < 1)
                throw new ShaderWaitressException("keyword set needs a keyword");
            var keyword = doc.ResolveKeyword(rest[0]);
            var json = keyword.Entry.Edit();
            if (args.Has("name"))
                json["m_Name"] = args.Get("name");
            if (args.Has("ref"))
                json["m_OverrideReferenceName"] = args.Get("ref");
            if (args.Has("definition"))
                json["m_KeywordDefinition"] = ParseChoice(args.Get("definition"), "definition", "shaderfeature", "multicompile", "predefined");
            if (args.Has("scope"))
                json["m_KeywordScope"] = ParseChoice(args.Get("scope"), "scope", "local", "global");
            if (args.Has("allow-definition-override"))
                SetAllowDefinitionOverride(editor, keyword, args.Get("allow-definition-override"));
            if (args.Has("promote"))
                SetPromoted(doc, editor, keyword, args.Get("promote"));
            if (args.Has("default"))
            {
                var entries = keyword.Entries;
                var index = entries.ToList().FindIndex(e => string.Equals(e, args.Get("default"), StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                    json["m_Value"] = index;
                else if (int.TryParse(args.Get("default"), out var raw))
                    json["m_Value"] = raw;
                else
                    json["m_Value"] = args.Get("default") is "1" or "on" or "true" ? 1 : 0;
            }
            editor.Log.Add($"updated keyword \"{keyword.Name}\"");
            editor.MarkChanged();
        }

        static int ParseChoice(string text, string what, params string[] options)
        {
            var index = Array.FindIndex(options, o => string.Equals(o, text, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                throw new ShaderWaitressException($"'{text}' is not a {what}. Options: {string.Join(", ", options)}");
            return index;
        }

        /// <summary>
        /// Splices a new node into what an existing node already feeds. Every consumer moves onto
        /// the new node's output, and the existing output becomes the new node's input. The
        /// inverse of `node rm --reconnect`.
        /// </summary>
        public static List<SgNode> NodeInsert(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            if (rest.Count < 1)
                throw new ShaderWaitressException("node insert needs a type, e.g. 'node insert Multiply --after n2433'");
            var entry = NodeCatalog.Shared.Require(rest[0]);
            var after = args.Get("after")
                        ?? throw new ShaderWaitressException("node insert needs --after <selector>: the node whose output is being intercepted");

            var targets = Select(doc, new[] { after }, "node insert --after");
            var group = args.Has("group") ? doc.ResolveGroup(args.Get("group")) : null;
            var fromName = args.Get("from");
            var intoName = args.Get("port");
            var outName = args.Get("out");

            // The ports an explicit --wire will fill must not be chosen as the splice input.
            var wired = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var assignment in args.GetAll("wire"))
            {
                var at = assignment.IndexOf('=');
                if (at > 0)
                    wired.Add(assignment.Substring(0, at));
            }

            var created = new List<SgNode>();
            foreach (var target in targets)
            {
                var live = doc.NodeById(target.ObjectId);
                if (live == null)
                    continue;

                var source = fromName != null
                    ? live.RequirePort(fromName, input: false)
                    : SingleOutput(live);
                var consumers = doc.EdgesOut(live).Where(e => e.FromSlot == source.Id)
                    .Select(e => (node: e.To, slot: e.ToSlot)).ToList();

                // Bind before resolving ports: a SubGraph, Property or Keyword node has none
                // until it knows what it points at.
                var node = BindReferences(doc, editor, args, editor.AddNode(entry, args.Get("name"), group));
                var into = intoName != null
                    ? node.RequirePort(intoName, input: true)
                    : node.Inputs.FirstOrDefault(p => !p.Hidden && !wired.Contains(p.Name))
                      ?? throw new ShaderWaitressException($"{node.ShortId} ({node.TypeLabel}) has no free input to splice into");
                var outPort = outName != null ? node.RequirePort(outName, input: false) : SingleOutput(node);

                foreach (var consumer in consumers)
                    editor.Connect(node, outPort.Id, consumer.node, consumer.slot);
                editor.Connect(live, source.Id, node, into.Id);
                editor.Log.Add($"spliced {node.ShortId} after {live.ShortId}.{source.Name}, moving {consumers.Count} consumer(s)");

                ApplySets(editor, doc.NodeById(node.ObjectId), args);
                ApplyWires(doc, editor, doc.NodeById(node.ObjectId), args);
                created.Add(doc.NodeById(node.ObjectId));
            }
            return created;
        }

        static SgPort SingleOutput(SgNode node)
        {
            var outputs = node.Outputs.Where(p => !p.Hidden).ToList();
            if (outputs.Count == 1)
                return outputs[0];
            if (outputs.Count == 0)
                throw new ShaderWaitressException($"{node.ShortId} ({node.TypeLabel}) has no output");
            throw new ShaderWaitressException(
                $"{node.ShortId} ({node.TypeLabel}) has {outputs.Count} outputs; name one with --from or --out: " +
                string.Join(", ", outputs.Select(DenseText.PortToken)));
        }

        /// <summary>
        /// Adds a port to a node. Most node types build their own ports in code, but a Custom
        /// Function node has none until the author declares them.
        /// </summary>
        public static void PortAdd(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            var targets = Select(doc, rest, "node port add");
            var name = args.Get("name") ?? throw new ShaderWaitressException("node port add needs --name");
            var direction = (args.Get("direction") ?? "in").ToLowerInvariant();
            var input = direction switch
            {
                "in" or "input" => true,
                "out" or "output" => false,
                _ => throw new ShaderWaitressException($"--direction is in or out, not '{direction}'"),
            };
            var kind = SlotTypes.ParseKind(args.Get("type") ?? "Vector4");
            if (kind == SlotKind.Unknown)
                throw new ShaderWaitressException(
                    $"'{args.Get("type")}' is not a port type. Try Float, Vector2, Vector3, Vector4, Color, Bool, Texture2D, SamplerState, Matrix4 or Gradient.");
            var slotType = SlotTypes.SlotTypeForKind(kind)
                           ?? throw new ShaderWaitressException($"a {SlotTypes.Label(kind)} port cannot be created directly");

            foreach (var target in targets)
            {
                var node = doc.NodeById(target.ObjectId);
                if (node == null)
                    continue;
                if (node.FindPort(name, input) != null)
                    throw new ShaderWaitressException($"{node.ShortId} already has a{(input ? "n input" : "n output")} called '{name}'");
                editor.AddPort(node, name, args.Get("shader-name"), input, kind, slotType,
                    args.Has("id") ? args.GetInt("id", 0) : (int?)null, args.Get("value"));
            }
        }

        public static void PortRemove(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            var targets = Select(doc, rest, "node port rm");
            var name = args.Get("name") ?? throw new ShaderWaitressException("node port rm needs --name");
            foreach (var target in targets)
            {
                var node = doc.NodeById(target.ObjectId);
                if (node == null)
                    continue;
                var port = node.FindPort(name) ?? throw new ShaderWaitressException(
                    $"{node.ShortId} ({node.TypeLabel}) has no port '{name}'. Ports: " +
                    string.Join(", ", node.Ports.Select(DenseText.PortToken)));
                editor.RemovePort(node, port);
            }
        }

        public static void NodeRemove(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            var targets = Select(doc, rest, "node rm");
            var reconnect = args.Flag("reconnect");
            foreach (var node in targets)
            {
                var live = doc.NodeById(node.ObjectId);
                if (live != null)
                    editor.RemoveNode(live, reconnect);
            }
        }

        /// <summary>Shared spelling for the on/off flags, so they all refuse the same way.</summary>
        public static bool OnOff(string text, string flag)
        {
            if (text is "1" or "on" or "true" or "yes")
                return true;
            if (text is "0" or "off" or "false" or "no")
                return false;
            throw new ShaderWaitressException($"--{flag} is on or off, not '{text}'");
        }

        /// <summary>Collapsing a preview takes about 184 px off the node's height.</summary>
        public static void SetPreview(GraphEditor editor, SgNode node, string text)
        {
            var on = OnOff(text, "preview");
            if (node.PreviewExpanded == on)
                return;
            node.PreviewExpanded = on;
            editor.Log.Add($"{node.ShortId} preview {(on ? "expanded" : "collapsed")}");
            editor.MarkChanged();
        }

        public static void NodeSet(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            var targets = Select(doc, rest, "node set");
            var newName = args.Get("name");
            var groupText = args.Get("group");
            var previewText = args.Get("preview");
            foreach (var node in targets)
            {
                if (newName != null)
                {
                    node.Name = newName;
                    editor.Log.Add($"renamed {node.ShortId} to \"{newName}\"");
                }
                if (groupText != null)
                    editor.Assign(node, groupText is "none" or "-" ? null : doc.ResolveGroup(groupText));
                if (previewText != null)
                    SetPreview(editor, node, previewText);
                ApplySets(editor, node, args);
                ApplyWires(doc, editor, node, args);
            }
        }

        public static void NodeReplace(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            var entry = NodeCatalog.Shared.Require(args.Require("with"));
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in args.GetAll("map"))
            {
                var eq = pair.IndexOf('=');
                if (eq <= 0)
                    throw new ShaderWaitressException($"--map expects Old=New, got '{pair}'");
                map[pair.Substring(0, eq)] = pair.Substring(eq + 1);
            }
            foreach (var node in Select(doc, rest, "node replace"))
            {
                var live = doc.NodeById(node.ObjectId);
                if (live == null)
                    continue;
                var replacement = editor.ReplaceNode(live, entry, map, out var dropped);
                editor.Log.Add($"replaced with {replacement.ShortId} {replacement.TypeLabel}");
                foreach (var drop in dropped)
                    editor.Log.Add("  dropped wire " + drop);
            }
        }

        public static void Wire(ShaderGraphDocument doc, GraphEditor editor, IReadOnlyList<string> rest)
        {
            var pieces = Normalize(rest);
            if (pieces.Count != 2)
                throw new ShaderWaitressException("wire takes a source and a destination, e.g. 'wire n42 nb17.UV'");
            editor.Connect(PortRef.ResolveOutput(doc, pieces[0]), PortRef.ResolveInput(doc, pieces[1]));
        }

        public static int Unwire(ShaderGraphDocument doc, GraphEditor editor, IReadOnlyList<string> rest)
        {
            var pieces = Normalize(rest);
            if (pieces.Count == 1)
            {
                var target = PortRef.ResolveInput(doc, pieces[0]);
                var removed = editor.Disconnect(e => e.To.Is(target.Node) && e.ToSlot == target.Id);
                editor.Log.Add($"removed {removed} wire(s) into {target.Node.ShortId}.{target.Name}");
                return removed;
            }
            if (pieces.Count == 2)
            {
                var source = PortRef.ResolveOutput(doc, pieces[0]);
                var target = PortRef.ResolveInput(doc, pieces[1]);
                var removed = editor.Disconnect(e => e.From.Is(source.Node) && e.FromSlot == source.Id &&
                                                     e.To.Is(target.Node) && e.ToSlot == target.Id);
                editor.Log.Add($"removed {removed} wire(s)");
                return removed;
            }
            throw new ShaderWaitressException("unwire takes a destination, or a source and a destination");
        }

        /// <summary>Accepts "a -> b" as well as separate arguments.</summary>
        static List<string> Normalize(IReadOnlyList<string> rest)
        {
            var joined = string.Join(" ", rest);
            if (joined.Contains("->", StringComparison.Ordinal))
            {
                return joined.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return rest.ToList();
        }

        public static SgGroup GroupNew(ShaderGraphDocument doc, GraphEditor editor, IReadOnlyList<string> rest)
        {
            if (rest.Count < 1)
                throw new ShaderWaitressException("group new needs a title");
            var group = editor.AddGroup(rest[0]);
            if (rest.Count > 1)
            {
                foreach (var node in Selector.Evaluate(doc, string.Join(" ", rest.Skip(1))))
                    editor.Assign(node, group);
            }
            return group;
        }

        public static SgProperty PropertyAdd(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            if (rest.Count < 1)
                throw new ShaderWaitressException("prop add needs a type, e.g. 'prop add Float --name \"Speed\"'");
            var entry = ResolvePropertyType(rest[0]);
            var name = args.Get("name") ?? rest.ElementAtOrDefault(1) ?? entry.Title;
            var property = editor.AddProperty(entry, name, args.Get("ref"), args.Get("value"));
            if (args.Flag("hidden"))
                property.Exposed = false;
            if (args.Has("declaration"))
                SetDeclaration(editor, entry, property, args.Get("declaration"));
            if (args.Has("promote"))
                SetPromoted(doc, editor, property, args.Get("promote"));
            if (args.Has("default"))
                SetTextureDefault(editor, property, args.Get("default"));
            return property;
        }

        /// <summary>What a texture property samples as while nothing is assigned. See <see cref="TextureDefaults"/>.</summary>
        public static void SetTextureDefault(GraphEditor editor, SgProperty property, string text)
        {
            if (!property.HasTextureDefault)
                throw new ShaderWaitressException(
                    $"--default is a texture property's fallback; a {property.TypeLabel} property has none. " +
                    "Use --value to set a value.");
            var value = TextureDefaults.Parse(text);
            property.TextureDefault = value;
            editor.Log.Add($"property \"{property.Name}\" default = {TextureDefaults.Name(value)}");
            editor.MarkChanged();
        }

        /// <summary>
        /// A promoted input stores the sub-graph's own guid, read from its .meta. A graph Unity
        /// has never imported doesn't have one yet.
        /// </summary>
        static string OwnAssetGuid(ShaderGraphDocument doc)
        {
            if (!doc.IsSubGraph)
                throw new ShaderWaitressException(
                    "only a sub-graph can promote an input: promotion is what carries it out to the " +
                    "parent shader, and a shader graph has no parent");
            if (string.IsNullOrEmpty(doc.Path))
                throw new ShaderWaitressException("the sub-graph has no path, so its own guid cannot be read");
            return ReadAssetGuid(doc.Path);
        }

        /// <summary>
        /// A promoted sub-graph input becomes a material property on the parent shader instead
        /// of an input port on the Sub Graph node.
        /// </summary>
        public static void SetPromoted(ShaderGraphDocument doc, GraphEditor editor, SgProperty property, string text)
        {
            var on = OnOff(text, "promote");
            if (on && !property.CanPromote)
                throw new ShaderWaitressException(
                    $"a {property.TypeLabel} property cannot be promoted; Shader Graph allows it for " +
                    "every property type except Gradient, Sampler State and Virtual Texture");
            property.SetPromoted(on, on ? OwnAssetGuid(doc) : null);
            editor.Log.Add($"property \"{property.Name}\" promote = {(on ? "on (declared on the parent shader)" : "off (an input port on the Sub Graph node)")}");
            editor.MarkChanged();
        }

        public static void SetPromoted(ShaderGraphDocument doc, GraphEditor editor, SgKeyword keyword, string text)
        {
            var on = OnOff(text, "promote");
            keyword.SetPromoted(on, on ? OwnAssetGuid(doc) : null);
            editor.Log.Add($"keyword \"{keyword.Name}\" promote = {(on ? "on (declared on the parent shader)" : "off (an input port on the Sub Graph node)")}");
            editor.MarkChanged();
        }

        /// <summary>
        /// A property that receives an Entities Graphics override needs HybridPerInstance. With
        /// the default UnityPerMaterial, the override silently does nothing. The choice is
        /// checked against what the property type accepts.
        /// </summary>
        public static void SetDeclaration(GraphEditor editor, CatalogEntry entry, SgProperty property, string text)
        {
            var declaration = HlslDeclarations.Parse(text);
            entry ??= NodeCatalog.Shared.Properties.FirstOrDefault(p =>
                string.Equals(p.TypeName, property.TypeName, StringComparison.Ordinal));
            if (!HlslDeclarations.IsAllowed(entry?.Declarations, declaration))
            {
                var allowed = entry.Declarations.Select(d => HlslDeclarations.Name(HlslDeclarations.Parse(d)));
                throw new ShaderWaitressException(
                    $"a {property.TypeLabel} property cannot be {HlslDeclarations.Name(declaration)}. " +
                    (entry.Declarations.Length == 0
                        ? "Shader Graph does not let this type choose a declaration at all."
                        : $"Options: {string.Join(", ", allowed)}."));
            }
            property.SetDeclaration(declaration);
            editor.Log.Add($"set property \"{property.Name}\" declaration = {HlslDeclarations.Name(declaration)}");
            editor.MarkChanged();
        }

        public static CatalogEntry ResolvePropertyType(string text)
        {
            var catalog = NodeCatalog.Shared;
            var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["float"] = "Vector1",
                ["bool"] = "Boolean",
                ["texture"] = "Texture2D",
                ["tex2d"] = "Texture2D",
            };
            if (aliases.TryGetValue(text, out var alias))
                text = alias;
            var match = catalog.Properties.FirstOrDefault(p =>
                string.Equals(p.Title, text, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(SlotTypes.ShortName(p.TypeName), text, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p.TypeName, text, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;
            throw new ShaderWaitressException($"unknown property type '{text}'. Known: " +
                string.Join(", ", catalog.Properties.Select(p => p.Title)));
        }

        public static void PropertySet(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            if (rest.Count < 1)
                throw new ShaderWaitressException("prop set needs a property");
            var property = doc.ResolveProperty(rest[0]);
            if (args.Has("value"))
                editor.SetPropertyValue(property, args.Get("value"));
            if (args.Has("name"))
                property.Name = args.Get("name");
            if (args.Has("ref"))
                property.SetReferenceName(args.Get("ref"));
            if (args.Flag("exposed"))
                property.Exposed = true;
            if (args.Flag("hidden"))
                property.Exposed = false;
            if (args.Has("declaration"))
            {
                var text = args.Get("declaration");
                if (string.Equals(text, "default", StringComparison.OrdinalIgnoreCase))
                {
                    property.ClearDeclarationOverride();
                    editor.Log.Add($"property \"{property.Name}\" declaration back to the default");
                    editor.MarkChanged();
                }
                else
                {
                    SetDeclaration(editor, null, property, text);
                }
            }
            if (args.Has("promote"))
                SetPromoted(doc, editor, property, args.Get("promote"));
            if (args.Has("default"))
                SetTextureDefault(editor, property, args.Get("default"));
            editor.Log.Add($"updated property \"{property.Name}\"");
        }

        public static void TargetSet(ShaderGraphDocument doc, GraphEditor editor, Args args, IReadOnlyList<string> rest)
        {
            if (doc.Targets.Count == 0)
                throw new ShaderWaitressException("this graph has no targets");
            var chosen = args.Has("id")
                ? doc.Targets.FirstOrDefault(t => t.ShortId == args.Get("id")) ?? throw new ShaderWaitressException($"no target '{args.Get("id")}'")
                : doc.Targets[0];
            if (rest.Count == 0)
                throw new ShaderWaitressException("nothing to set. Known keys: " + string.Join(", ", TargetSettings.KnownKeys));
            foreach (var assignment in rest)
            {
                var eq = assignment.IndexOf('=');
                if (eq <= 0)
                    throw new ShaderWaitressException($"expected key=value, got '{assignment}'");
                editor.Log.Add("target " + TargetSettings.Apply(chosen, assignment.Substring(0, eq), assignment.Substring(eq + 1)));
            }

            // Switching to transparent leaves the opaque block list behind, so there is nowhere
            // to send alpha. Only ever additive: deciding a block is surplus needs the active
            // list, which only the Editor can produce.
            if (args.Flag("sync-blocks"))
            {
                var added = 0;
                foreach (var descriptor in BlockAdvice.MissingDescriptors(doc).ToList())
                {
                    BlockAdd(doc, editor, descriptor);
                    added++;
                }
                if (added == 0)
                    editor.Log.Add("blocks already match the target settings");
            }
            else
            {
                foreach (var problem in BlockAdvice.Missing(doc))
                    editor.Log.Add("note: " + problem + " (--sync-blocks adds it)");
            }
        }

        public static readonly Dictionary<string, string> GraphSettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["precision"] = "m_GraphPrecision",
            ["previewmode"] = "m_PreviewMode",
            ["path"] = "m_Path",
        };

        public static void SettingsSet(ShaderGraphDocument doc, GraphEditor editor, IReadOnlyList<string> rest)
        {
            foreach (var assignment in rest)
            {
                var eq = assignment.IndexOf('=');
                if (eq <= 0)
                    throw new ShaderWaitressException($"expected key=value, got '{assignment}'");
                var key = assignment.Substring(0, eq);
                var value = assignment.Substring(eq + 1);
                if (!GraphSettings.TryGetValue(key, out var field))
                    throw new ShaderWaitressException($"unknown setting '{key}'. Known: " + string.Join(", ", GraphSettings.Keys));
                JsonNode parsed = key.ToLowerInvariant() switch
                {
                    "precision" => value.ToLowerInvariant() switch
                    {
                        "inherit" => 0,
                        "single" or "float" => 1,
                        "half" => 2,
                        _ => throw new ShaderWaitressException("precision must be inherit, single or half"),
                    },
                    "previewmode" => int.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
                    _ => value,
                };
                doc.Raw.Root.Edit()[field] = parsed;
                editor.Log.Add($"{key} = {value}");
            }
        }

        // ---- shared helpers ----

        public static IReadOnlyList<SgNode> Select(ShaderGraphDocument doc, IReadOnlyList<string> rest, string what)
        {
            if (rest.Count == 0)
                throw new ShaderWaitressException($"{what} needs a selector");
            var matches = Selector.Evaluate(doc, string.Join(" ", rest));
            if (matches.Count == 0)
                throw new ShaderWaitressException($"{what}: selector matched nothing");
            return matches;
        }

        public static void ApplySets(GraphEditor editor, SgNode node, Args args)
        {
            foreach (var assignment in args.GetAll("setting"))
            {
                var eq = assignment.IndexOfAny(new[] { (char)61, (char)58 });
                if (eq <= 0)
                    throw new ShaderWaitressException($"--setting expects Name=value, got '{assignment}'");
                editor.SetNodeSetting(node, assignment.Substring(0, eq), assignment.Substring(eq + 1));
            }

            foreach (var assignment in args.GetAll("set"))
            {
                var eq = assignment.IndexOf((char)61);
                if (eq <= 0)
                    throw new ShaderWaitressException($"--set expects Port=value, got '{assignment}'");
                var name = assignment.Substring(0, eq);
                var value = assignment.Substring(eq + 1);
                // A caller who does not know whether a control is a port or a dropdown should
                // not have to; --set finds either, and only complains when neither exists.
                var port = node.FindPort(name, input: true);
                if (port != null)
                {
                    editor.SetPortValue(port, value);
                    continue;
                }
                if (NodeSettings.Find(node, name) != null)
                {
                    editor.SetNodeSetting(node, name, value);
                    continue;
                }
                var settings = node.Settings.Select(s => s.Name).ToList();
                throw new ShaderWaitressException(
                    $"{node.ShortId} ({node.TypeLabel}) has no input '{name}'. " +
                    $"Ports: {string.Join(", ", node.Inputs.Select(DenseText.PortToken))}." +
                    (settings.Count == 0 ? string.Empty : $" Settings: {string.Join(", ", settings)}."));
            }
        }

        public static void ApplyWires(ShaderGraphDocument doc, GraphEditor editor, SgNode node, Args args)
        {
            foreach (var assignment in args.GetAll("wire"))
            {
                var eq = assignment.IndexOf('=');
                if (eq <= 0)
                    throw new ShaderWaitressException($"--wire expects Port=source, got '{assignment}'");
                var port = node.RequirePort(assignment.Substring(0, eq), input: true);
                editor.Connect(PortRef.ResolveOutput(doc, assignment.Substring(eq + 1)), port);
            }
            foreach (var assignment in args.GetAll("feed"))
            {
                var eq = assignment.IndexOf('=');
                if (eq <= 0)
                    throw new ShaderWaitressException($"--feed expects Port=destination, got '{assignment}'");
                var port = node.RequirePort(assignment.Substring(0, eq), input: false);
                editor.Connect(port, PortRef.ResolveInput(doc, assignment.Substring(eq + 1)));
            }
        }
    }
}
