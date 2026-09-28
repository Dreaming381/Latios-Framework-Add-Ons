using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using VfxWaitress.Catalog;
using VfxWaitress.Model;
using VfxWaitress.Query;
using VfxWaitress.Serialization;

namespace VfxWaitress.Edit
{
    /// <summary>
    /// Mutations on a loaded asset. Every change is made in memory and written once, so a failed
    /// command leaves the file exactly as it was.
    /// </summary>
    public sealed class GraphEditor
    {
        public readonly VfxAsset Asset;
        readonly List<string> m_Log = new List<string>();

        public IReadOnlyList<string> Log => m_Log;

        public GraphEditor(VfxAsset asset)
        {
            Asset = asset;
        }

        public void Note(string line)
        {
            if (!Silent)
                m_Log.Add(line);
        }

        /// <summary>Suppresses notes while a pass makes many small edits the caller didn't ask about.</summary>
        public bool Silent;

        readonly List<VfxNode> m_Added = new List<VfxNode>();

        /// <summary>
        /// Moves every top-level node added through this editor next to what it ended up wired to.
        /// Call once all the wiring is done. Nothing else moves.
        /// </summary>
        public void PlaceAdded()
        {
            var live = m_Added.Where(n => Asset.TopLevel.Contains(n)).ToList();
            if (live.Count == 0)
                return;
            foreach (var node in Layout.LayoutEngine.PlaceNear(Asset, live))
                Note($"placed {node.ShortId} beside what it's wired to");
            m_Added.Clear();
        }

        // --- node add ----------------------------------------------------------------

        /// <summary>
        /// Writes a node by cloning the catalog's template of a fresh instance, including its whole
        /// slot tree, and remapping the local file ids onto ids this file doesn't use. Building a
        /// slot tree by hand isn't practical, since every slot carries its own assembly-qualified
        /// type, master reference, and owner.
        /// </summary>
        public VfxNode AddNode(CatalogModel model, VfxNode context, IReadOnlyDictionary<string, string> settings)
        {
            if (model == null)
                throw new VfxWaitressException("no such model");
            model = PickVariant(model, settings);
            if (string.IsNullOrEmpty(model.Template))
                throw new VfxWaitressException($"the catalog has no template for {model.Type}; re-export it against this Editor");

            var template = UnityYaml.Parse(model.Template);
            var map = AssignIds(template);
            var root = template.Documents.First();

            foreach (var doc in template.Documents)
            {
                Remap(doc, map);
                // The standalone serializer records the class name here; a .vfx leaves it blank.
                if (doc.Body.Find("m_EditorClassIdentifier") != null)
                    doc.Body.SetScalar("m_EditorClassIdentifier", "");
                Asset.Yaml.Documents.Add(doc);
                Asset.ById[doc.FileId] = doc;
            }

            var owner = context ?? NodeFor(Asset.GraphDoc);
            root.Body.SetScalar("m_Parent", "{fileID: " + owner.FileId + "}");
            AppendChild(owner.Doc, root.FileId);

            var node = new VfxNode { FileId = root.FileId, Doc = root, Context = context, Kind = model.Kind };
            if (model.Kind == "context")
                AttachData(node, model);
            ApplySettings(node, model, settings);
            Asset.Reindex(node);
            node.Kind = model.Kind;
            // A template carries no position, so park the node clear of everything until
            // PlaceAdded knows what it's wired to.
            if (context == null)
            {
                Layout.LayoutEngine.PlaceNew(Asset, node);
                m_Added.Add(node);
            }
            // Report the short id, since that's what the next command will take.
            node.ShortId = Model.VfxAsset.ShortIdFor(model.Kind, root.FileId);
            Note($"added {model.ShortType} as {node.ShortId}" + (context != null ? $" in {context.ShortId}" : ""));
            return node;
        }

        /// <summary>
        /// Some settings decide what slots a node has. SetAttribute has five slot shapes depending
        /// on its attribute and random mode, and each is a separate catalog variant with its own
        /// template. Patching the setting onto the wrong variant gives a valid-looking node with
        /// the wrong slots, which Unity doesn't repair and no validator catches.
        ///
        /// So a setting any variant is keyed on selects a variant instead of being patched on.
        /// If no variant has the requested value, this refuses.
        /// </summary>
        CatalogModel PickVariant(CatalogModel model, IReadOnlyDictionary<string, string> settings)
        {
            var family = Asset.Catalog.Models.Where(m => m.Type == model.Type && m.Kind == model.Kind).ToList();
            if (family.Count <= 1 || settings == null || settings.Count == 0)
                return model;

            var shapeDefining = new HashSet<string>(family.SelectMany(m => m.VariantSettings.Keys), StringComparer.OrdinalIgnoreCase);
            var requested = settings.Where(kv => shapeDefining.Contains(kv.Key)).ToList();
            if (requested.Count == 0)
                return model;

            var matches = family.Where(m => requested.All(kv =>
                m.VariantSettings.TryGetValue(FamilyKey(shapeDefining, kv.Key), out var v) &&
                string.Equals(v, kv.Value, StringComparison.OrdinalIgnoreCase))).ToList();

            if (matches.Count > 0)
            {
                // Among equals prefer the one that pins the fewest other settings.
                var best = matches.OrderBy(m => m.VariantSettings.Count).First();
                if (best != model)
                    Note($"using the '{best.Name}' variant, whose slots match those settings");
                return best;
            }

            var offender = requested[0];
            var available = family
                .Select(m => m.VariantSettings.TryGetValue(FamilyKey(shapeDefining, offender.Key), out var v) ? v : null)
                .Where(v => v != null).Distinct().OrderBy(v => v, StringComparer.Ordinal).ToList();
            throw new VfxWaitressException(
                $"'{offender.Key}={offender.Value}' has no variant of {model.ShortType}, and that setting decides the node's slots, " +
                $"so writing it onto another variant's template would produce the wrong slots. Values with a variant: {string.Join(", ", available)}. " +
                "Add this one through the Editor, or pick a value from that list.");
        }

        static string FamilyKey(HashSet<string> keys, string requested) =>
            keys.FirstOrDefault(k => string.Equals(k, requested, StringComparison.OrdinalIgnoreCase)) ?? requested;

        /// <summary>
        /// Gives a new context its VFXData, which holds capacity and bounds mode. A context
        /// without one is incomplete.
        /// </summary>
        void AttachData(VfxNode node, CatalogModel model)
        {
            // A template usually brings its own data object, which just has to be registered.
            var existing = UnityYaml.FileIdIn(node.Doc.Body.Scalar("m_Data"));
            if (existing.HasValue && existing.Value != 0 && Asset.ById.TryGetValue(existing.Value, out var existingDoc))
            {
                RegisterData(node, existingDoc);
                return;
            }
            // Only particle contexts need one. A spawner has its own kind of data.
            var dataTypeName = model.Settings.Any(s => s.Name == "capacity")
                ? "UnityEditor.VFX.VFXDataParticle"
                : null;
            if (dataTypeName == null)
                return;
            var dataModel = Asset.Catalog.ByType(dataTypeName);
            if (dataModel?.Template == null)
                return;

            var template = UnityYaml.Parse(dataModel.Template);
            var map = AssignIds(template);
            YamlDocument dataDoc = null;
            foreach (var doc in template.Documents)
            {
                Remap(doc, map);
                if (doc.Body.Find("m_EditorClassIdentifier") != null)
                    doc.Body.SetScalar("m_EditorClassIdentifier", "");
                Asset.Yaml.Documents.Add(doc);
                Asset.ById[doc.FileId] = doc;
                dataDoc ??= doc;
            }
            if (dataDoc == null)
                return;

            node.Doc.Body.SetScalar("m_Data", "{fileID: " + dataDoc.FileId.ToString(CultureInfo.InvariantCulture) + "}");
            RegisterData(node, dataDoc);
            Note($"created system {node.Data.ShortId} for the new context");
        }

        void RegisterData(VfxNode node, YamlDocument dataDoc)
        {
            if (!Asset.Datas.TryGetValue(dataDoc.FileId, out var data))
            {
                data = new VfxData
                {
                    FileId = dataDoc.FileId,
                    Doc = dataDoc,
                    Model = Asset.Catalog.ByScript(VfxAsset.ScriptOf(dataDoc)),
                    Title = dataDoc.Body.Scalar("title"),
                    ShortId = VfxAsset.ShortIdFor("data", dataDoc.FileId),
                };
                Asset.Datas[dataDoc.FileId] = data;
            }
            if (!data.Owners.Contains(node))
                data.Owners.Add(node);
            node.Data = data;
            SetOwners(data);
        }

        void ApplySettings(VfxNode node, CatalogModel model, IReadOnlyDictionary<string, string> settings)
        {
            foreach (var kvp in model.VariantSettings)
                SetSetting(node, model, kvp.Key, kvp.Value);
            if (settings == null)
                return;
            foreach (var kvp in settings)
                SetSetting(node, model, kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Writes a setting by name. Enums serialize as ordinals and bools as 0/1, so this
        /// translates <c>repeat=Periodic</c> into what the file stores.
        /// </summary>
        public void SetSetting(VfxNode node, CatalogModel model, string name, string value)
        {
            var setting = model?.Settings.FirstOrDefault(s => Loose.Equal(s.Name, name));
            var key = setting?.Name ?? (node.Doc.Body.Find(name) != null ? name : "m_" + name);

            // Capacity, bounds mode, and space live on the shared VFXData. Written to the context,
            // they'd be silently ignored.
            var target = node.Doc;
            if (target.Body.Find(key) == null && node.Data?.Doc.Body.Find(key) != null)
            {
                target = node.Data.Doc;
                if (node.Data.Owners.Count > 1)
                    Note($"note: {key} is shared by every context in system {node.Data.ShortId}");
            }

            if (target.Body.Find(key) == null)
            {
                var available = new List<string>();
                if (model != null)
                    available.AddRange(model.Settings.Where(s => node.Doc.Body.Find(s.Name) != null).Select(s => s.Name));
                if (node.Data != null)
                    available.AddRange(node.Data.Doc.Body.Entries.Select(e => e.Key).Where(k => !k.StartsWith("m_", StringComparison.Ordinal)));
                throw new VfxWaitressException(
                    $"no setting '{name}' on {model?.ShortType ?? node.DisplayType}" +
                    (available.Count > 0 ? " (has: " + string.Join(", ", available.Distinct()) + ")" : ""));
            }

            // A setting that isn't a bare scalar can't be written as one. Unity silently loads the
            // mismatch as unset, and the graph compiles as something other than what was asked.
            if (setting != null && setting.Shape == "serializableType")
            {
                var aqn = Asset.Catalog.AssemblyQualified(value);
                if (aqn == null)
                    throw new VfxWaitressException(
                        $"'{value}' is not a type this VFX Graph knows; {setting.Name} takes one of the graph's own types " +
                        "(see `vfxwaitress catalog --kind slot-type`)");
                target.Body.SetBlock(key, "    m_SerializableType: " + Quote(aqn) + "\n");
                Note($"set {key} = {value}");
                return;
            }
            if (setting != null && setting.Shape == "objectRef")
            {
                target.Body.SetScalar(key, AssetReference(value, setting.Name));
                Note($"set {key} = {value}");
                return;
            }
            // A choice-of-strings setting stores only the chosen name. The node re-derives the
            // list of choices itself.
            if (setting != null && setting.Type != null && setting.Type.Contains("MultipleValuesChoice"))
            {
                target.Body.SetBlock(key, "    selection: " + value + "\n    selectedIndex: 0\n");
                Note($"set {key} = {value}");
                Note("  run `vfxwaitress resync` so the Editor re-derives this node's slots from it");
                return;
            }
            if (setting != null && setting.Shape == "nested")
            {
                throw new VfxWaitressException(
                    $"{setting.Name} serializes as a nested object of a shape the tool does not know ({setting.Type}); set it in the Editor.");
            }

            var text = value;
            if (setting != null && setting.Values.Count > 0)
            {
                var index = setting.Values.FindIndex(v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
                if (index < 0 && !int.TryParse(value, out _))
                    throw new VfxWaitressException($"'{value}' is not a value of {setting.Name}; one of: {string.Join(", ", setting.Values)}");
                if (index >= 0)
                    text = index.ToString(CultureInfo.InvariantCulture);
            }
            else if (setting?.Type == "System.Boolean")
            {
                text = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1" ? "1" : "0";
            }
            target.Body.SetScalar(key, text);
            Note($"set {key} = {text}");
        }

        /// <summary>
        /// Writes a per-system setting, shared by every context in the system.
        /// </summary>
        public void SetDataSetting(VfxData data, string name, string value)
        {
            var setting = data.Model?.Settings.FirstOrDefault(s => Loose.Equal(s.Name, name));
            var key = setting?.Name ?? name;
            if (data.Doc.Body.Find(key) == null)
            {
                var available = data.Doc.Body.Entries.Select(e => e.Key).Where(k => !k.StartsWith("m_", StringComparison.Ordinal));
                throw new VfxWaitressException($"no setting '{name}' on this system (has: {string.Join(", ", available)})");
            }
            var text = value;
            if (setting != null && setting.Values.Count > 0)
            {
                var index = setting.Values.FindIndex(v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
                if (index < 0 && !int.TryParse(value, out _))
                    throw new VfxWaitressException($"'{value}' is not a value of {setting.Name}; one of: {string.Join(", ", setting.Values)}");
                if (index >= 0)
                    text = index.ToString(CultureInfo.InvariantCulture);
            }
            data.Doc.Body.SetScalar(key, text);
            Note($"system {data.ShortId}: {key} = {text}" +
                 (data.Owners.Count > 1 ? $" (shared by {data.Owners.Count} contexts)" : ""));
        }

        /// <summary>
        /// Resolves an asset by path or name to the reference a .vfx stores. The main object's
        /// local file id is a per-importer constant, and a wrong guess silently resolves to
        /// nothing. Known kinds come from a table, and anything else is asked of the Editor.
        /// </summary>
        string AssetReference(string value, string settingName)
        {
            var index = GuidIndex.For(Asset.Path);
            var guid = index.GuidOf(value, out var ambiguous);
            if (ambiguous != null)
                throw new VfxWaitressException($"'{value}' matches {ambiguous.Count} assets: {string.Join(", ", ambiguous)}");
            if (guid == null)
                throw new VfxWaitressException($"no asset named '{value}' in this project; give a path relative to Assets/ or Packages/");
            var path = index.PathOf(guid);
            var fileId = GuidIndex.MainFileIdFor(path);
            if (fileId != 0)
                return "{fileID: " + fileId.ToString(CultureInfo.InvariantCulture) + ", guid: " + guid + ", type: 3}";

            var resolved = EditorLink.Bridge.Require(path, "ResolveReference");
            if (!resolved.StartsWith("error", StringComparison.Ordinal))
            {
                var parts = resolved.Trim().Split(',');
                if (parts.Length == 3)
                {
                    Note($"resolved {Path.GetFileName(path)} through the Editor");
                    return "{fileID: " + parts[0] + ", guid: " + parts[1] + ", type: " + parts[2] + "}";
                }
            }
            throw new VfxWaitressException(
                $"{settingName} points at {path}, whose main file id the tool doesn't know, so it has to ask the Editor. {resolved}");
        }

        /// <summary>Unity folds a long plain scalar, so an assembly-qualified name is quoted.</summary>
        static string Quote(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        // --- wiring ------------------------------------------------------------------

        /// <summary>
        /// Links an output slot to an input slot by writing the reference on both slots. VFX
        /// stores no edge list, and a link on only one end is a broken graph.
        /// </summary>
        public void Wire(VfxSlot from, VfxSlot to)
        {
            if (!from.IsInput && to.IsInput)
            {
                // already the right way round
            }
            else if (from.IsInput && !to.IsInput)
            {
                (from, to) = (to, from);
            }
            else
            {
                throw new VfxWaitressException($"cannot wire {from.Owner.ShortId}.{from.Path} to {to.Owner.ShortId}.{to.Path}: need one output and one input");
            }

            Unwire(to);
            AppendLink(from, to.FileId);
            AppendLink(to, from.FileId);
            Note($"wired {from.Owner.ShortId}.{from.Path} ({from.TypeName}) -> {to.Owner.ShortId}.{to.Path} ({to.TypeName})");
            if (from.TypeName != to.TypeName)
                Note($"  note: {from.TypeName} -> {to.TypeName}; VFX Graph will unify these types when it loads the graph");
        }

        /// <summary>Clears whatever feeds an input slot, on both ends.</summary>
        public void Unwire(VfxSlot input)
        {
            foreach (var sourceId in input.LinkedSlotIds.ToList())
            {
                var source = Asset.SlotById(sourceId);
                if (source != null)
                    RemoveLink(source, input.FileId);
                Note($"unwired {input.Owner.ShortId}.{input.Path}");
            }
            if (input.LinkedSlotIds.Count > 0)
            {
                input.LinkedSlotIds.Clear();
                SetLinks(input, Array.Empty<long>());
            }
        }

        void AppendLink(VfxSlot slot, long other)
        {
            if (slot.LinkedSlotIds.Contains(other))
                return;
            slot.LinkedSlotIds.Add(other);
            SetLinks(slot, slot.LinkedSlotIds);
        }

        void RemoveLink(VfxSlot slot, long other)
        {
            if (!slot.LinkedSlotIds.Remove(other))
                return;
            SetLinks(slot, slot.LinkedSlotIds);
        }

        static void SetLinks(VfxSlot slot, IEnumerable<long> ids)
        {
            var list = ids.ToList();
            if (list.Count == 0)
            {
                slot.Doc.Body.SetScalar("m_LinkedSlots", "[]");
                return;
            }
            var sb = new StringBuilder();
            foreach (var id in list)
                sb.Append("  - {fileID: ").Append(id.ToString(CultureInfo.InvariantCulture)).Append("}\n");
            slot.Doc.Body.SetBlock("m_LinkedSlots", sb.ToString());
        }

        // --- context flow ------------------------------------------------------------

        public void LinkContexts(VfxNode from, int fromIndex, VfxNode to, int toIndex)
        {
            if (from.Kind != "context" || to.Kind != "context")
                throw new VfxWaitressException("flow links join two contexts");
            SetFlowEntry(from.Doc, "m_OutputFlowSlot", fromIndex, to.FileId, toIndex);
            SetFlowEntry(to.Doc, "m_InputFlowSlot", toIndex, from.FileId, fromIndex);
            if (!from.FlowOut.Contains((fromIndex, to.FileId, toIndex)))
                from.FlowOut.Add((fromIndex, to.FileId, toIndex));
            Note($"flow {from.ShortId}[{fromIndex}] -> {to.ShortId}[{toIndex}]");
            MergeData(from, to);
        }

        /// <summary>
        /// A flow link between two particle contexts also joins them into one system, so the target
        /// adopts the source's VFXData. Writing only the flow edge leaves each context in its own
        /// system, which the Editor never produces and no validator notices.
        /// </summary>
        void MergeData(VfxNode from, VfxNode to)
        {
            if (from.Data == null || to.Data == null || from.Data == to.Data)
                return;
            // A spawner's data is a different kind and never merges with the system it feeds.
            if (from.Data.Doc.Body.Find("capacity") == null || to.Data.Doc.Body.Find("capacity") == null)
                return;

            var target = from.Data;
            var orphan = to.Data;
            foreach (var owner in orphan.Owners.ToList())
            {
                owner.Doc.Body.SetScalar("m_Data", "{fileID: " + target.FileId.ToString(CultureInfo.InvariantCulture) + "}");
                owner.Data = target;
                target.Owners.Add(owner);
            }
            SetOwners(target);
            orphan.Owners.Clear();
            RemoveDocument(orphan.FileId);
            Asset.Datas.Remove(orphan.FileId);
            Note($"merged {to.ShortId} into system {target.ShortId} (\"{target.Title}\"), which is what a flow link between particle contexts means");
        }

        void SetOwners(VfxData data)
        {
            var sb = new StringBuilder();
            foreach (var owner in data.Owners.Select(o => o.FileId).Distinct())
                sb.Append("  - {fileID: ").Append(owner.ToString(CultureInfo.InvariantCulture)).Append("}\n");
            if (sb.Length == 0)
                data.Doc.Body.SetScalar("m_Owners", "[]");
            else
                data.Doc.Body.SetBlock("m_Owners", sb.ToString());
        }

        void RemoveDocument(long fileId)
        {
            var doc = Asset.Yaml.Documents.FirstOrDefault(d => d.FileId == fileId);
            if (doc == null)
                return;
            Asset.Yaml.Documents.Remove(doc);
            Asset.ById.Remove(fileId);
        }

        static void SetFlowEntry(YamlDocument doc, string key, int index, long otherContext, int otherIndex)
        {
            var existing = ReadFlowSlots(doc, key);
            while (existing.Count <= index)
                existing.Add(new List<(long, int)>());
            existing[index].Add((otherContext, otherIndex));
            WriteFlowSlots(doc, key, existing);
        }

        static List<List<(long context, int slotIndex)>> ReadFlowSlots(YamlDocument doc, string key)
        {
            var result = new List<List<(long, int)>>();
            var seq = doc.Body.Seq(key);
            if (seq == null)
                return result;
            foreach (var item in seq.Items)
            {
                var links = new List<(long, int)>();
                var lines = item.Split('\n');
                for (var i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Contains("context:"))
                        continue;
                    var target = UnityYaml.FileIdIn(lines[i]);
                    if (!target.HasValue || target.Value == 0)
                        continue;
                    var slotIndex = 0;
                    for (var j = i + 1; j < lines.Length && !lines[j].Contains("context:"); j++)
                    {
                        var t = lines[j].Trim();
                        if (t.StartsWith("slotIndex:", StringComparison.Ordinal))
                        {
                            int.TryParse(t.Substring("slotIndex:".Length).Trim(), out slotIndex);
                            break;
                        }
                    }
                    links.Add((target.Value, slotIndex));
                }
                result.Add(links);
            }
            return result;
        }

        static void WriteFlowSlots(YamlDocument doc, string key, List<List<(long context, int slotIndex)>> slots)
        {
            var sb = new StringBuilder();
            foreach (var slot in slots)
            {
                if (slot.Count == 0)
                {
                    sb.Append("  - link: []\n");
                    continue;
                }
                sb.Append("  - link:\n");
                foreach (var (context, slotIndex) in slot)
                {
                    sb.Append("    - context: {fileID: ").Append(context.ToString(CultureInfo.InvariantCulture)).Append("}\n");
                    sb.Append("      slotIndex: ").Append(slotIndex.ToString(CultureInfo.InvariantCulture)).Append('\n');
                }
            }
            doc.Body.SetBlock(key, sb.ToString());
        }

        // --- values ------------------------------------------------------------------

        /// <summary>
        /// Writes a literal onto a slot. A composite value is stored as a JSON blob on the master
        /// slot, so setting a child slot is refused, since VFX Graph would ignore it.
        /// </summary>
        public void SetValue(VfxSlot slot, string value)
        {
            if (slot.Parent != null)
                throw new VfxWaitressException(
                    $"{slot.Owner.ShortId}.{slot.Path} is a child slot; VFX Graph stores the value on its master slot, so set {Root(slot).Path} instead");
            // A parameter carries its value on an output slot, so that one's allowed.
            if (!slot.IsInput && slot.Owner?.ExposedName == null)
                throw new VfxWaitressException($"{slot.Owner?.ShortId}.{slot.Path} is an output; values are set on inputs");
            // A wired input's value is overridden by the wire. A parameter's links are consumers,
            // so its value still matters.
            if (slot.IsInput && slot.LinkedSlotIds.Count > 0)
                throw new VfxWaitressException($"{slot.Owner.ShortId}.{slot.Path} is wired; unwire it before setting a value");

            var master = slot.Doc.Body.Map("m_MasterData")?.Map("m_Value");
            if (master == null)
                throw new VfxWaitressException($"{slot.Owner.ShortId}.{slot.Path} has no value storage");
            master.SetScalar("m_SerializableObject", Encode(slot, value));
            Note($"set {slot.Owner.ShortId}.{slot.Path} = {value}");
        }

        static VfxSlot Root(VfxSlot slot)
        {
            while (slot.Parent != null)
                slot = slot.Parent;
            return slot;
        }

        /// <summary>
        /// Scalars serialize bare, and composites serialize as JSON quoted the way Unity quotes it.
        /// Field names come from the slot's own children, so any type's shape works.
        /// </summary>
        static string Encode(VfxSlot slot, string value)
        {
            if (slot.Children.Count == 0)
                return value;
            var parts = value.Split(',').Select(p => p.Trim()).ToList();
            var supplied = parts.Count;
            var json = EncodeComposite(slot, parts, out var consumed);
            if (parts.Count > 0)
                throw new VfxWaitressException($"{slot.Path} takes {consumed} value(s), got {supplied}");
            return "'" + json.Replace("'", "''") + "'";
        }

        static string EncodeComposite(VfxSlot slot, List<string> parts, out int consumed)
        {
            if (slot.Children.Count == 0)
            {
                consumed = 1;
                if (parts.Count == 0)
                    throw new VfxWaitressException("not enough values");
                var scalar = parts[0];
                parts.RemoveAt(0);
                return Number(scalar);
            }
            var sb = new StringBuilder("{");
            consumed = 0;
            for (var i = 0; i < slot.Children.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append('"').Append(slot.Children[i].Name).Append("\":");
                sb.Append(EncodeComposite(slot.Children[i], parts, out var used));
                consumed += used;
            }
            sb.Append('}');
            return sb.ToString();
        }

        static string Number(string s)
        {
            if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase))
                return "true";
            if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase))
                return "false";
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                throw new VfxWaitressException($"'{s}' is not a number");
            return d == Math.Floor(d) && Math.Abs(d) < 1e15
                ? d.ToString("0.0", CultureInfo.InvariantCulture)
                : d.ToString("R", CultureInfo.InvariantCulture);
        }

        // --- file ids and plumbing ---------------------------------------------------

        VfxNode NodeFor(YamlDocument doc) => new VfxNode { FileId = doc.FileId, Doc = doc };

        /// <summary>
        /// Fresh file ids for a template's documents. Each one's last four digits differ from every
        /// id already in the file, so a new node gets a four-digit short id and no existing node's
        /// id has to grow to make room for it.
        /// </summary>
        Dictionary<long, long> AssignIds(YamlFile template)
        {
            var used = new HashSet<long>(Asset.Yaml.Documents.Select(d => d.FileId));
            var tails = new HashSet<long>(used.Select(id => Math.Abs(id % 10000)));
            var next = used.Count == 0 ? 1 : used.Max() + 1;
            var map = new Dictionary<long, long>();
            foreach (var doc in template.Documents)
            {
                // Past 9000 taken tails, a free one is hard to find and collisions are unavoidable.
                while (used.Contains(next) || (tails.Count < 9000 && tails.Contains(Math.Abs(next % 10000))))
                    next++;
                map[doc.FileId] = next;
                used.Add(next);
                tails.Add(Math.Abs(next % 10000));
                next++;
            }
            return map;
        }

        static void Remap(YamlDocument doc, Dictionary<long, long> map)
        {
            if (map.TryGetValue(doc.FileId, out var mine))
            {
                doc.AnchorLine = doc.AnchorLine.Substring(0, doc.AnchorLine.IndexOf('&') + 1) + mine.ToString(CultureInfo.InvariantCulture);
                doc.FileId = mine;
            }
            RemapNode(doc.Body, map);
        }

        static void RemapNode(YNode node, Dictionary<long, long> map)
        {
            switch (node)
            {
                case YMap m:
                    foreach (var e in m.Entries)
                        RemapNode(e.Value, map);
                    break;
                case YSeq seq:
                    for (var i = 0; i < seq.Items.Count; i++)
                        seq.Items[i] = RemapText(seq.Items[i], map);
                    seq.Touch();
                    break;
                case YScalar scalar:
                {
                    var replaced = RemapText(scalar.Raw, map);
                    if (replaced != scalar.Raw)
                        scalar.SetRaw(replaced);
                    break;
                }
                case YSeqOrMapText text:
                {
                    var replaced = RemapText(text.Raw, map);
                    if (replaced != text.Raw)
                        text.SetRaw(replaced);
                    break;
                }
            }
        }

        static string RemapText(string text, Dictionary<long, long> map)
        {
            if (text == null || text.IndexOf("fileID", StringComparison.Ordinal) < 0)
                return text;
            // A guid alongside the id means an external asset reference, which must not be touched.
            return System.Text.RegularExpressions.Regex.Replace(text, @"fileID:\s*(-?\d+)(?!\s*,\s*guid)", m =>
            {
                var id = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                return map.TryGetValue(id, out var mapped) ? "fileID: " + mapped.ToString(CultureInfo.InvariantCulture) : m.Value;
            });
        }

        public static void AppendChild(YamlDocument doc, long childId)
        {
            var seq = doc.Body.Seq("m_Children");
            if (seq != null)
            {
                seq.AddFileId(childId);
                return;
            }
            // An empty list is a flow "[]" on the key line, so it has to be rewritten as a block.
            doc.Body.SetBlock("m_Children", "  - {fileID: " + childId.ToString(CultureInfo.InvariantCulture) + "}\n");
        }

        /// <summary>
        /// Removes a node and everything it owns: its slot tree, its blocks, and the links other
        /// nodes hold to it. VFX Graph doesn't report a slot left pointing at a deleted one.
        /// </summary>
        public void RemoveNode(VfxNode node)
        {
            var doomed = new HashSet<long> { node.FileId };
            foreach (var block in node.Blocks)
            {
                doomed.Add(block.FileId);
                foreach (var slot in block.AllInputSlots.Concat(block.AllOutputSlots))
                    doomed.Add(slot.FileId);
            }
            foreach (var slot in node.AllInputSlots.Concat(node.AllOutputSlots))
                doomed.Add(slot.FileId);
            if (node.Data != null && node.Data.Owners.All(o => o == node))
                doomed.Add(node.Data.FileId);

            // Break every link into this node before the slots go, on the far end too.
            foreach (var other in Asset.AllNodes())
            {
                if (doomed.Contains(other.FileId))
                    continue;
                foreach (var slot in other.AllInputSlots.Concat(other.AllOutputSlots))
                {
                    var survivors = slot.LinkedSlotIds.Where(id => !doomed.Contains(id)).ToList();
                    if (survivors.Count == slot.LinkedSlotIds.Count)
                        continue;
                    slot.LinkedSlotIds.Clear();
                    slot.LinkedSlotIds.AddRange(survivors);
                    SetLinks(slot, survivors);
                }
                // A context flowing into this one keeps a dangling flow entry otherwise.
                if (other.FlowOut.RemoveAll(f => doomed.Contains(f.toContext)) > 0)
                    RewriteFlow(other, doomed);
            }

            var parentDoc = node.Context?.Doc ?? Asset.GraphDoc;
            var children = VfxAsset.ChildIds(parentDoc).Where(id => id != node.FileId).ToList();
            if (children.Count == 0)
                parentDoc.Body.SetScalar("m_Children", "[]");
            else
                parentDoc.Body.SetBlock("m_Children", string.Concat(children.Select(id => "  - {fileID: " + id.ToString(CultureInfo.InvariantCulture) + "}\n")));

            foreach (var id in doomed)
                RemoveDocument(id);
            Asset.Forget(node);
            Note($"removed {node.ShortId} ({node.DisplayType}) and {doomed.Count - 1} object(s) it owned");
        }

        void RewriteFlow(VfxNode node, HashSet<long> doomed)
        {
            var slots = ReadFlowSlots(node.Doc, "m_OutputFlowSlot");
            foreach (var slot in slots)
                slot.RemoveAll(l => doomed.Contains(l.context));
            WriteFlowSlots(node.Doc, "m_OutputFlowSlot", slots);
        }

        /// <summary>
        /// Fixes the shapes VFX Graph silently repairs the first time an asset is opened. Each one
        /// marks the asset dirty on open and produces a diff nobody asked for.
        /// </summary>
        public int Repair()
        {
            var fixes = 0;

            var name = System.IO.Path.GetFileNameWithoutExtension(Asset.Path);
            foreach (var doc in new[] { Asset.GraphDoc, Asset.ResourceDoc })
            {
                if (doc == null || !string.IsNullOrWhiteSpace(doc.Body.Scalar("m_Name")))
                    continue;
                doc.Body.SetScalar("m_Name", name);
                Note($"named {doc.TypeName} \"{name}\", which the Editor would have filled in on open");
                fixes++;
            }

            // Blackboard order is a per-parameter index; duplicates get renumbered on open.
            var parameters = Asset.TopLevel.Where(n => n.ExposedName != null).ToList();
            var orders = parameters.Select(p => p.Doc.Body.Scalar("m_Order")).ToList();
            if (orders.Count > 1 && orders.Distinct().Count() < orders.Count)
            {
                for (var i = 0; i < parameters.Count; i++)
                    parameters[i].Doc.Body.SetScalar("m_Order", i.ToString(CultureInfo.InvariantCulture));
                Note($"renumbered m_Order across {parameters.Count} parameters, which shared an order");
                fixes++;
            }

            // A wired parameter with no placement is drawn nowhere.
            var unplaced = parameters
                .Where(p => p.ParameterNodes.Count == 0 && p.AllOutputSlots.Any(s => s.LinkedSlotIds.Count > 0))
                .ToList();
            foreach (var parameter in unplaced)
            {
                // Only place the parameter. A repair never moves positions a person chose.
                Layout.LayoutEngine.PlaceNew(Asset, parameter);
                Note($"placed \"{parameter.ExposedName}\", which was wired but drawn nowhere");
                fixes++;
            }

            return fixes;
        }

        /// <summary>The smallest legal asset: a resource, an empty graph, and its UI object.</summary>
        public static void CreateEmpty(string path, bool subgraphOperator)
        {
            var graphId = 114350483966674976L;
            var uiId = 114340500867371532L;
            var resourceId = 8926484042661614527L;
            var sb = new StringBuilder();
            sb.Append("%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n");
            sb.Append("--- !u!114 &").Append(graphId).Append('\n');
            sb.Append("MonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n");
            sb.Append("  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 0}\n");
            sb.Append("  m_Enabled: 1\n  m_EditorHideFlags: 0\n");
            sb.Append("  m_Script: {fileID: 11500000, guid: ").Append(VfxAsset.GraphScriptGuid).Append(", type: 3}\n");
            sb.Append("  m_Name: ").Append(System.IO.Path.GetFileNameWithoutExtension(path)).Append('\n');
            sb.Append("  m_EditorClassIdentifier: \n  m_UIIgnoredErrors: []\n  m_Parent: {fileID: 0}\n");
            sb.Append("  m_Children: []\n  m_UIPosition: {x: 0, y: 0}\n  m_UICollapsed: 1\n  m_UISuperCollapsed: 0\n");
            sb.Append("  m_UIInfos: {fileID: ").Append(uiId).Append("}\n");
            sb.Append("  m_CustomAttributes: []\n  m_ParameterInfo: []\n  m_ImportDependencies: []\n");
            sb.Append("  m_GraphVersion: 19\n  m_ResourceVersion: 1\n  m_SubgraphDependencies: []\n  m_CategoryPath: \n");
            sb.Append("--- !u!114 &").Append(uiId).Append('\n');
            sb.Append("MonoBehaviour:\n  m_ObjectHideFlags: 1\n  m_CorrespondingSourceObject: {fileID: 0}\n");
            sb.Append("  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 0}\n");
            sb.Append("  m_Enabled: 1\n  m_EditorHideFlags: 0\n");
            sb.Append("  m_Script: {fileID: 11500000, guid: ").Append(VfxAsset.UiScriptGuid).Append(", type: 3}\n");
            sb.Append("  m_Name: VFXUI\n  m_EditorClassIdentifier: \n");
            sb.Append("  groupInfos: []\n  stickyNoteInfos: []\n  categories: []\n");
            sb.Append("  uiBounds:\n    serializedVersion: 2\n    x: 0\n    y: 0\n    width: 0\n    height: 0\n");
            sb.Append("--- !u!2058629511 &").Append(resourceId).Append('\n');
            sb.Append("VisualEffectResource:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n");
            sb.Append("  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n");
            sb.Append("  m_Name: ").Append(System.IO.Path.GetFileNameWithoutExtension(path)).Append('\n');
            sb.Append("  m_Graph: {fileID: ").Append(graphId).Append("}\n");
            if (!subgraphOperator)
            {
                sb.Append("  m_Infos:\n    m_RendererSettings:\n      motionVectorGenerationMode: 0\n      shadowCastingMode: 0\n");
                sb.Append("      rayTracingMode: 0\n      receiveShadows: 0\n      reflectionProbeUsage: 0\n      lightProbeUsage: 0\n");
                sb.Append("    m_CullingFlags: 3\n    m_UpdateMode: 0\n    m_PreWarmDeltaTime: 0.05\n    m_PreWarmStepCount: 0\n");
                sb.Append("    m_InitialEventName: OnPlay\n    m_InstancingMode: -1\n    m_InstancingCapacity: 64\n");
            }
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        }

        public void Save(string path = null)
        {
            File.WriteAllText(path ?? Asset.Path, Asset.Yaml.Write(), new UTF8Encoding(false));
        }
    }
}
