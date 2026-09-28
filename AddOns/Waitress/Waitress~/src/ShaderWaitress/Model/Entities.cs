using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    public struct Rect
    {
        public double X, Y, Width, Height;

        public Rect(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public double Right => X + Width;
        public double Bottom => Y + Height;
        public double CenterY => Y + Height * 0.5;

        public bool Overlaps(Rect other, double margin = 0)
        {
            return X - margin < other.Right && other.X - margin < Right &&
                   Y - margin < other.Bottom && other.Y - margin < Bottom;
        }

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "({0:0.#}, {1:0.#}, {2:0.#}x{3:0.#})", X, Y, Width, Height);
    }

    /// <summary>One port on one node. Backed by a serialized slot object when it has one.</summary>
    public sealed class SgPort
    {
        public SgNode Node;
        public MultiJsonEntry Entry;      // null when the port is only known from the catalog
        public int Id;
        public string Name;               // display name, what the caller types
        public string ShaderOutputName;
        public bool IsInput;
        public SlotKind Kind;
        public string SlotTypeName;
        public bool Hidden;

        /// <summary>
        /// The value the node type's constructor supplies. Present even when the port has no
        /// serialized slot, because Shader Graph rebuilds the slot with this value on load —
        /// so it is what the shader computes either way.
        /// </summary>
        public JsonNode CatalogDefault;

        public JsonNode Value
        {
            get
            {
                var valueField = SlotTypes.ValueFieldOf(SlotTypeName);
                if (valueField == null)
                    return null;
                if (Entry == null)
                    return CatalogDefault;
                return Entry.Node.TryGetPropertyValue(valueField, out var v) ? v : null;
            }
        }

        public JsonNode DefaultValue
        {
            get
            {
                if (Entry == null)
                    return null;
                return Entry.Node.TryGetPropertyValue("m_DefaultValue", out var v) ? v : null;
            }
        }

        public bool HasNonDefaultValue
        {
            get
            {
                if (Entry == null)
                    return false;       // nothing serialized means nothing was overridden
                var v = Value;
                if (v == null)
                    return false;
                var d = DefaultValue;
                if (d == null)
                    return SlotTypes.FormatValue(Kind, v) != null;
                return !SlotTypes.ValueEquals(v, d);
            }
        }

        public string FormattedValue => SlotTypes.FormatValue(Kind, Value);

        public void SetValue(JsonNode value)
        {
            var field = SlotTypes.ValueFieldOf(SlotTypeName);
            if (Entry == null || field == null)
                throw new ShaderWaitressException($"port '{Name}' on {Node.ShortId} has no editable value");
            Entry.Edit()[field] = value;
        }

        public override string ToString() => $"{Node?.ShortId}.{Name}";
    }

    public sealed class SgNode
    {
        public ShaderGraphDocument Doc;
        public MultiJsonEntry Entry;
        public string ShortId;

        List<SgPort> m_Ports;

        public string ObjectId => Entry.ObjectId;
        public string TypeName => Entry.TypeName;

        /// <summary>"UnityEditor.ShaderGraph.MultiplyNode" becomes "Multiply".</summary>
        public string TypeLabel
        {
            get
            {
                var s = SlotTypes.ShortName(TypeName) ?? TypeName;
                if (s.EndsWith("Node", StringComparison.Ordinal) && s.Length > 4)
                    s = s.Substring(0, s.Length - 4);
                return s;
            }
        }

        public bool IsBlock => TypeName == "UnityEditor.ShaderGraph.BlockNode";
        public bool IsProperty => TypeName == "UnityEditor.ShaderGraph.PropertyNode";
        public bool IsKeyword => TypeName == "UnityEditor.ShaderGraph.KeywordNode";
        public bool IsDropdown => TypeName == "UnityEditor.ShaderGraph.DropdownNode";
        public bool IsRedirect => TypeName == "UnityEditor.ShaderGraph.RedirectNodeData";

        public string Name
        {
            get => (string)Entry.Node["m_Name"] ?? TypeLabel;
            set => Entry.Edit()["m_Name"] = value;
        }

        public string BlockDescriptor => (string)Entry.Node["m_SerializedDescriptor"];

        public string PropertyId => UnityJsonWriter.RefId(Entry.Node["m_Property"]);
        public string KeywordId => UnityJsonWriter.RefId(Entry.Node["m_Keyword"]);
        public string DropdownId => UnityJsonWriter.RefId(Entry.Node["m_Dropdown"]);

        public string GroupId
        {
            get => UnityJsonWriter.RefId(Entry.Node["m_Group"]);
            set => Entry.Edit()["m_Group"] = UnityJsonWriter.Ref(value);
        }

        /// <summary>
        /// Whether the Editor has ever placed this node. Property tokens are stored with a
        /// position but no measured size until they are first drawn, so a zero size is not
        /// evidence that a node is unplaced.
        /// </summary>
        public bool HasStoredPosition => Entry.Node["m_DrawState"]?["m_Position"] != null;

        public Rect Position
        {
            get
            {
                var p = Entry.Node["m_DrawState"]?["m_Position"];
                if (p == null)
                    return default;
                return new Rect(Num(p["x"]), Num(p["y"]), Num(p["width"]), Num(p["height"]));
            }
            set
            {
                var node = Entry.Edit();
                if (node["m_DrawState"] is not JsonObject draw)
                {
                    draw = new JsonObject { ["m_Expanded"] = true };
                    node["m_DrawState"] = draw;
                }
                draw["m_Position"] = new JsonObject
                {
                    ["serializedVersion"] = "2",
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["width"] = value.Width,
                    ["height"] = value.Height,
                };
            }
        }

        /// <summary>
        /// Whether the node draws its preview swatch. Collapsing it takes ~184 px off the
        /// node's height, which is most of what makes a graph of preview nodes sprawl.
        /// </summary>
        public bool PreviewExpanded
        {
            get
            {
                var v = Entry.Node["m_PreviewExpanded"];
                return ShaderWaitress.Serialization.Json.Bool(v, true);
            }
            set
            {
                var node = Entry.Edit();
                node["m_PreviewExpanded"] = value;
                // The stored size was measured with the preview in its old state, so it is no
                // longer what the node draws. Drop it and let the sizer estimate.
                if (node["m_DrawState"] is JsonObject draw && draw["m_Position"] is JsonObject p)
                {
                    p["width"] = 0.0;
                    p["height"] = 0.0;
                }
            }
        }

        static double Num(JsonNode n) => Json.Number(n, 0);

        public IReadOnlyList<SgPort> Ports
        {
            get
            {
                if (m_Ports == null)
                    m_Ports = BuildPorts();
                return m_Ports;
            }
        }

        public IEnumerable<SgPort> Inputs => Ports.Where(p => p.IsInput);
        public IEnumerable<SgPort> Outputs => Ports.Where(p => !p.IsInput);

        public void InvalidatePorts() => m_Ports = null;

        /// <summary>The node body controls: dropdowns and toggles that no wire reveals.</summary>
        public IReadOnlyList<SgSetting> Settings => NodeSettings.For(this);

        List<SgPort> BuildPorts()
        {
            var ports = new List<SgPort>();
            var seen = new HashSet<int>();
            if (Entry.Node["m_Slots"] is JsonArray slots)
            {
                foreach (var slotRef in slots)
                {
                    var id = UnityJsonWriter.RefId(slotRef);
                    var slotEntry = Doc.Raw.Find(id);
                    if (slotEntry == null)
                        continue;
                    var port = FromSlotEntry(slotEntry);
                    port.Node = this;
                    ports.Add(port);
                    seen.Add(port.Id);
                }
            }

            // Slots a node did not serialize are rebuilt by Shader Graph on load, so the
            // catalog is the only place they exist until then.
            var catalogNode = Doc.Catalog?.Find(TypeName);
            if (catalogNode != null)
            {
                foreach (var cp in catalogNode.Ports)
                {
                    if (seen.Contains(cp.Id))
                        continue;
                    ports.Add(new SgPort
                    {
                        Node = this,
                        Entry = null,
                        Id = cp.Id,
                        Name = cp.Name,
                        ShaderOutputName = cp.ShaderOutputName,
                        IsInput = cp.IsInput,
                        Kind = cp.Kind,
                        SlotTypeName = cp.SlotTypeName,
                        CatalogDefault = cp.DefaultValue,
                    });
                }
            }

            return ports;
        }

        internal static SgPort FromSlotEntry(MultiJsonEntry slotEntry)
        {
            var node = slotEntry.Node;
            var kind = SlotTypes.KindOf(slotEntry.TypeName);
            return new SgPort
            {
                Entry = slotEntry,
                Id = ShaderWaitress.Serialization.Json.Int(node["m_Id"], 0),
                Name = (string)node["m_DisplayName"] ?? (string)node["m_ShaderOutputName"] ?? "?",
                ShaderOutputName = (string)node["m_ShaderOutputName"],
                IsInput = ShaderWaitress.Serialization.Json.Int(node["m_SlotType"], 0) == 0,
                Kind = kind,
                SlotTypeName = slotEntry.TypeName,
                Hidden = ShaderWaitress.Serialization.Json.Bool(node["m_Hidden"], false),
            };
        }

        public SgPort FindPort(string nameOrId, bool? input = null)
        {
            var candidates = Ports.Where(p => input == null || p.IsInput == input.Value).ToList();
            if (int.TryParse(nameOrId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                var byId = candidates.FirstOrDefault(p => p.Id == id);
                if (byId != null)
                    return byId;
            }
            var exact = candidates.Where(p => string.Equals(p.Name, nameOrId, StringComparison.OrdinalIgnoreCase)).ToList();
            if (exact.Count == 1)
                return exact[0];
            if (exact.Count > 1)
                throw new ShaderWaitressException($"port '{nameOrId}' is ambiguous on {ShortId} ({TypeLabel})");
            var byOutputName = candidates.Where(p => string.Equals(p.ShaderOutputName, nameOrId, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byOutputName.Count == 1)
                return byOutputName[0];
            var squashed = candidates.Where(p => string.Equals(Squash(p.Name), Squash(nameOrId), StringComparison.OrdinalIgnoreCase)).ToList();
            if (squashed.Count == 1)
                return squashed[0];
            return null;
        }

        static string Squash(string s) => s == null ? null : s.Replace(" ", string.Empty).Replace("_", string.Empty);

        public SgPort RequirePort(string nameOrId, bool? input = null)
        {
            var port = FindPort(nameOrId, input);
            if (port != null)
                return port;
            var direction = input == null ? "port" : input.Value ? "input" : "output";
            var available = string.Join(", ", Ports.Where(p => input == null || p.IsInput == input.Value).Select(p => p.Name));
            throw new ShaderWaitressException($"{ShortId} ({TypeLabel}) has no {direction} '{nameOrId}'. Available: {available}");
        }

        /// <summary>
        /// Identity by object id. Rebuilding the semantic view replaces every SgNode
        /// instance, so an edge predicate written against a reference silently stops matching.
        /// </summary>
        public bool Is(SgNode other) => other != null && string.Equals(ObjectId, other.ObjectId, StringComparison.Ordinal);

        public override string ToString() => $"{ShortId} {TypeLabel}";
    }

    public sealed class SgEdge
    {
        public SgNode From;
        public int FromSlot;
        public SgNode To;
        public int ToSlot;

        public SgPort FromPort => From?.Ports.FirstOrDefault(p => p.Id == FromSlot && !p.IsInput)
                                  ?? From?.Ports.FirstOrDefault(p => p.Id == FromSlot);

        public SgPort ToPort => To?.Ports.FirstOrDefault(p => p.Id == ToSlot && p.IsInput)
                                ?? To?.Ports.FirstOrDefault(p => p.Id == ToSlot);

        public override string ToString() => $"{From?.ShortId}.{FromSlot} -> {To?.ShortId}.{ToSlot}";
    }

    public sealed class SgProperty
    {
        public ShaderGraphDocument Doc;
        public MultiJsonEntry Entry;
        public string ShortId;

        public string ObjectId => Entry.ObjectId;
        public string TypeName => Entry.TypeName;

        /// <summary>"Internal.Vector1ShaderProperty" becomes "Float".</summary>
        public string TypeLabel
        {
            get
            {
                var s = SlotTypes.ShortName(TypeName) ?? TypeName;
                if (s.EndsWith("ShaderProperty", StringComparison.Ordinal))
                    s = s.Substring(0, s.Length - "ShaderProperty".Length);
                return s switch
                {
                    "Vector1" => "Float",
                    "Vector2" => "Vector2",
                    "Vector3" => "Vector3",
                    "Vector4" => "Vector4",
                    _ => s,
                };
            }
        }

        public string Name
        {
            get => (string)Entry.Node["m_Name"] ?? string.Empty;
            set => Entry.Edit()["m_Name"] = value;
        }

        public string ReferenceName
        {
            get
            {
                var over = (string)Entry.Node["m_OverrideReferenceName"];
                return !string.IsNullOrEmpty(over) ? over : (string)Entry.Node["m_DefaultReferenceName"] ?? string.Empty;
            }
        }

        public void SetReferenceName(string value) => Entry.Edit()["m_OverrideReferenceName"] = value ?? string.Empty;

        public bool Exposed
        {
            get => ShaderWaitress.Serialization.Json.Bool(Entry.Node["m_GeneratePropertyBlock"], true);
            set => Entry.Edit()["m_GeneratePropertyBlock"] = value;
        }

        /// <summary>
        /// Where the property is declared in the generated HLSL. Left alone, Shader Graph picks
        /// UnityPerMaterial for an exposed property and Global otherwise — so a property that
        /// needs to receive a per-instance override (Entities Graphics) has to say so, and
        /// nothing about the graph reveals that it did not.
        /// </summary>
        public bool DeclarationOverridden =>
            ShaderWaitress.Serialization.Json.Bool(Entry.Node["overrideHLSLDeclaration"], false);

        public HlslDeclaration Declaration
        {
            get
            {
                if (DeclarationOverridden)
                    return (HlslDeclaration)ShaderWaitress.Serialization.Json.Int(Entry.Node["hlslDeclarationOverride"], 2);
                return Exposed ? HlslDeclaration.UnityPerMaterial : HlslDeclaration.Global;
            }
        }

        public void SetDeclaration(HlslDeclaration declaration)
        {
            var json = Entry.Edit();
            json["overrideHLSLDeclaration"] = true;
            json["hlslDeclarationOverride"] = (int)declaration;
            // Shader Graph forces the exposed flag on for a per-instance property, so the file
            // stays consistent with what the Editor would have written.
            if (declaration == HlslDeclaration.HybridPerInstance)
                json["m_GeneratePropertyBlock"] = true;
        }

        public void ClearDeclarationOverride()
        {
            var json = Entry.Edit();
            json["overrideHLSLDeclaration"] = false;
            json["hlslDeclarationOverride"] = 0;
        }

        public bool Promoted => Promotion.IsPromoted(Entry);
        public void SetPromoted(bool on, string assetGuid) => Promotion.Set(Entry, on, assetGuid);

        /// <summary>Types Shader Graph refuses to promote, by <see cref="TypeLabel"/>.</summary>
        public bool CanPromote => Promotion.CanPromote(TypeLabel);

        /// <summary>Only Texture2D carries a default type; no other property type has the field.</summary>
        public bool HasTextureDefault => string.Equals(TypeLabel, "Texture2D", StringComparison.OrdinalIgnoreCase);

        public int TextureDefault
        {
            get => ShaderWaitress.Serialization.Json.Int(Entry.Node["m_DefaultType"], 0);
            set => Entry.Edit()["m_DefaultType"] = value;
        }

        public JsonNode Value => Entry.Node["m_Value"];

        public string FormattedValue
        {
            get
            {
                var v = Value;
                if (v == null)
                    return null;
                var kind = TypeLabel switch
                {
                    "Float" => SlotKind.Float,
                    "Vector2" => SlotKind.Vector2,
                    "Vector3" => SlotKind.Vector3,
                    "Vector4" => SlotKind.Vector4,
                    "Color" => SlotKind.Color4,
                    "Boolean" => SlotKind.Boolean,
                    "Texture2D" or "Texture2DArray" or "Texture3D" or "Cubemap" => SlotKind.Texture2D,
                    "Matrix2" or "Matrix3" or "Matrix4" => SlotKind.Matrix4,
                    "Gradient" => SlotKind.Gradient,
                    _ => SlotKind.Unknown,
                };
                if (kind == SlotKind.Color4 && v is JsonObject co && co.ContainsKey("r"))
                    return SlotTypes.FormatValue(SlotKind.Color4, v);
                return SlotTypes.FormatValue(kind, v);
            }
        }

        public override string ToString() => $"{ShortId} {TypeLabel} \"{Name}\"";
    }

    public sealed class SgGroup
    {
        public MultiJsonEntry Entry;
        public string ShortId;

        public string ObjectId => Entry.ObjectId;

        public string Title
        {
            get => (string)Entry.Node["m_Title"] ?? string.Empty;
            set => Entry.Edit()["m_Title"] = value;
        }

        public double X
        {
            get => ShaderWaitress.Serialization.Json.Number(Entry.Node["m_Position"]?["x"], 0);
        }

        public double Y
        {
            get => ShaderWaitress.Serialization.Json.Number(Entry.Node["m_Position"]?["y"], 0);
        }

        public void SetPosition(double x, double y)
        {
            Entry.Edit()["m_Position"] = new JsonObject { ["x"] = x, ["y"] = y };
        }

        public override string ToString() => $"{ShortId} \"{Title}\"";
    }

    public sealed class SgKeyword
    {
        public MultiJsonEntry Entry;
        public string ShortId;

        public string ObjectId => Entry.ObjectId;
        public string Name => (string)Entry.Node["m_Name"] ?? string.Empty;
        public string ReferenceName
        {
            get
            {
                var over = (string)Entry.Node["m_OverrideReferenceName"];
                return !string.IsNullOrEmpty(over) ? over : (string)Entry.Node["m_DefaultReferenceName"] ?? string.Empty;
            }
        }
        public int KeywordType => ShaderWaitress.Serialization.Json.Int(Entry.Node["m_KeywordType"], 0);
        public int KeywordDefinition => ShaderWaitress.Serialization.Json.Int(Entry.Node["m_KeywordDefinition"], 0);
        public int KeywordScope => ShaderWaitress.Serialization.Json.Int(Entry.Node["m_KeywordScope"], 0);
        public int Value => ShaderWaitress.Serialization.Json.Int(Entry.Node["m_Value"], 0);

        public bool Promoted => Promotion.IsPromoted(Entry);
        public void SetPromoted(bool on, string assetGuid) => Promotion.Set(Entry, on, assetGuid);

        /// <summary>
        /// The Editor's "Allow Definition Override". While it is on -- and it is on by
        /// default -- Shader Graph compiles the keyword to a runtime ternary rather than an
        /// #if with keyword permutations, so a branch that looks compile-time is not one.
        /// </summary>
        public bool AllowDefinitionOverride
        {
            get => ShaderWaitress.Serialization.Json.Bool(Entry.Node["m_IsShaderBuildSettingsCompatible"], true);
            set => Entry.Edit()["m_IsShaderBuildSettingsCompatible"] = value;
        }

        public IReadOnlyList<string> Entries => EntryDetails.Select(e => e.display).ToList();

        /// <summary>Entry id, label and reference name; the ids become the node's slot ids.</summary>
        public IReadOnlyList<(int id, string display, string reference)> EntryDetails
        {
            get
            {
                var list = new List<(int, string, string)>();
                if (Entry.Node["m_Entries"] is JsonArray arr)
                {
                    var index = 0;
                    foreach (var e in arr)
                    {
                        index++;
                        list.Add((ShaderWaitress.Serialization.Json.Int(e?["id"], index),
                            (string)e?["displayName"] ?? "?",
                            (string)e?["referenceName"] ?? "?"));
                    }
                }
                return list;
            }
        }
    }

    public sealed class SgTarget
    {
        public MultiJsonEntry Entry;
        public MultiJsonEntry SubTarget;
        public string ShortId;

        public string ObjectId => Entry.ObjectId;

        public string TypeLabel
        {
            get
            {
                var s = SlotTypes.ShortName(Entry.TypeName) ?? Entry.TypeName;
                return s.EndsWith("Target", StringComparison.Ordinal) && s.Length > 6 ? s.Substring(0, s.Length - 6) : s;
            }
        }

        public string SubTargetLabel
        {
            get
            {
                if (SubTarget == null)
                    return null;
                var s = SlotTypes.ShortName(SubTarget.TypeName) ?? SubTarget.TypeName;
                if (s.StartsWith("Universal", StringComparison.Ordinal))
                    s = s.Substring("Universal".Length);
                if (s.EndsWith("SubTarget", StringComparison.Ordinal))
                    s = s.Substring(0, s.Length - "SubTarget".Length);
                return s;
            }
        }
    }
}
