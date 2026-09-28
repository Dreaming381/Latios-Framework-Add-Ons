using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using ShaderWaitress.Catalog;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// A control drawn on a node's body rather than on a port: Scene Depth's sampling mode,
    /// Screen Position's space, Sample Texture 2D's type. These change what the shader computes
    /// but never appear as a wire, so a graph that reads correctly can still be wrong.
    /// </summary>
    public sealed class SgSetting
    {
        public SgNode Node;
        public string[] Path;           // where the value lives, e.g. ["m_Color", "color"]
        public string Name;             // what the caller types, e.g. DepthSamplingMode
        public string Kind;             // enum | bool | int | float | string | color | vector
        public string[] Values;         // enum member names
        public long[] Codes;            // the numbers those names serialize as

        public string Field => Path.Length > 0 ? Path[0] : null;

        public JsonNode Raw
        {
            get
            {
                JsonNode node = Node.Entry.Node;
                foreach (var key in Path)
                {
                    if (node is not JsonObject obj || !obj.TryGetPropertyValue(key, out node))
                        return null;
                }
                return node;
            }
        }

        JsonObject Container(JsonObject root)
        {
            var node = root;
            for (var i = 0; i < Path.Length - 1; i++)
            {
                if (node[Path[i]] is not JsonObject child)
                    throw new ShaderWaitressException($"{Name} is not where it was expected on {Node.ShortId}");
                node = child;
            }
            return node;
        }

        /// <summary>The value as the caller should see it: an enum name where one is known.</summary>
        public string Display
        {
            get
            {
                var raw = Raw;
                if (raw == null)
                    return null;
                switch (Kind)
                {
                    case "enum":
                    {
                        var code = Json.Number(raw);
                        if (code == null)
                            return Json.Text(raw);
                        var index = Codes == null ? -1 : Array.IndexOf(Codes, (long)code.Value);
                        if (index >= 0 && Values != null && index < Values.Length)
                            return Values[index];
                        return ((long)code.Value).ToString(CultureInfo.InvariantCulture);
                    }
                    case "bool":
                        return Json.Bool(raw, false) ? "true" : "false";
                    case "color":
                        return SlotTypes.FormatValue(SlotKind.Color4, raw);
                    case "vector":
                        return SlotTypes.FormatValue(SlotKind.Vector4, raw);
                    case "string":
                        return Json.Text(raw) is { Length: > 0 } s ? '"' + s + '"' : "\"\"";
                    default:
                    {
                        var number = Json.Number(raw);
                        return number == null ? raw.ToJsonString() : SlotTypes.FormatValue(SlotKind.Float, raw);
                    }
                }
            }
        }

        public void Set(string text)
        {
            var json = Container(Node.Entry.Edit());
            var key = Path[Path.Length - 1];
            switch (Kind)
            {
                case "enum":
                {
                    if (Values != null)
                    {
                        var index = Array.FindIndex(Values, v => string.Equals(v, text, StringComparison.OrdinalIgnoreCase));
                        if (index < 0)
                            index = Array.FindIndex(Values, v => v.Replace(" ", string.Empty)
                                .Equals(text.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                        if (index >= 0)
                        {
                            json[key] = Codes != null && index < Codes.Length ? Codes[index] : index;
                            return;
                        }
                    }
                    if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var raw))
                    {
                        json[key] = raw;
                        return;
                    }
                    throw new ShaderWaitressException(
                        $"'{text}' is not a value for {Node.ShortId}.{Name}. Options: {string.Join(", ", Values ?? Array.Empty<string>())}");
                }
                case "bool":
                    json[key] = text is "1" or "true" or "True" or "on" or "yes";
                    return;
                case "int":
                    if (!long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                        throw new ShaderWaitressException($"{Name} expects a whole number, got '{text}'");
                    json[key] = i;
                    return;
                case "float":
                    json[key] = SlotTypes.ParseDouble(text);
                    return;
                case "color":
                    json[key] = ParseComponents(text, "r", "g", "b", "a");
                    return;
                case "vector":
                    json[key] = ParseComponents(text, "x", "y", "z", "w");
                    return;
                default:
                    json[key] = text;
                    return;
            }
        }

        static JsonNode ParseComponents(string text, params string[] names)
        {
            var pieces = text.Trim().TrimStart((char)40, (char)91).TrimEnd((char)41, (char)93)
                .Split(new[] { (char)44, (char)32 }, StringSplitOptions.RemoveEmptyEntries);
            var obj = new JsonObject();
            for (var i = 0; i < names.Length; i++)
            {
                obj[names[i]] = i < pieces.Length
                    ? SlotTypes.ParseDouble(pieces[i])
                    : (pieces.Length == 1 ? SlotTypes.ParseDouble(pieces[0]) : (names[i] == "a" ? 1.0 : 0.0));
            }
            return obj;
        }

        public override string ToString() => Name + ":" + Display;
    }

    public static class NodeSettings
    {
        /// <summary>
        /// Fields every node carries for the framework's own bookkeeping. Anything else on a
        /// node object is one of its settings, which is how settings are still found for a node
        /// type the catalog has never heard of.
        /// </summary>
        static readonly HashSet<string> s_Bookkeeping = new HashSet<string>(StringComparer.Ordinal)
        {
            "m_SGVersion", "m_Type", "m_ObjectId", "m_Group", "m_Name", "m_DrawState", "m_Slots",
            "synonyms", "m_Precision", "m_PreviewExpanded", "m_DismissedVersion", "m_PreviewMode",
            "m_CustomColors", "m_SerializedDescriptor", "m_Property", "m_Keyword", "m_Dropdown",
            // References to other assets, shown by the node line itself rather than as a control.
            "m_SerializedSubGraph", "m_SerializedTexture", "m_SerializedCubemap", "IsFirstSlotValid",
        };

        public static List<SgSetting> For(SgNode node)
        {
            var result = new List<SgSetting>();
            var described = new HashSet<string>(StringComparer.Ordinal);

            var entry = node.Doc?.Catalog?.Find(node.TypeName);
            if (entry != null)
            {
                foreach (var s in entry.Settings)
                {
                    if (s_Bookkeeping.Contains(s.Field) || !described.Add(s.Field))
                        continue;
                    result.Add(new SgSetting
                    {
                        Node = node,
                        Path = new[] { s.Field },
                        Name = s.Name,
                        Kind = s.Kind,
                        Values = s.Values,
                        Codes = s.Codes,
                    });
                }
            }

            // A node type the catalog does not cover still shows its settings, just without
            // names for the enum values. That is what keeps the tool usable against a project
            // whose custom nodes were never exported.
            foreach (var pair in node.Entry.Node)
            {
                if (s_Bookkeeping.Contains(pair.Key) || described.Contains(pair.Key))
                    continue;
                if (pair.Value is JsonValue)
                {
                    described.Add(pair.Key);
                    result.Add(Scalar(node, new[] { pair.Key }, Strip(pair.Key), pair.Value));
                    continue;
                }
                if (pair.Value is not JsonObject nested)
                    continue;

                // A struct-valued control such as Color's {color, mode} is flattened one level
                // so each part is reachable; the colour itself keeps the field's own name.
                described.Add(pair.Key);
                var vectorChildren = nested.Count(c => Shape(c.Value) != null);
                foreach (var child in nested)
                {
                    var shape = Shape(child.Value);
                    var path = new[] { pair.Key, child.Key };
                    if (shape != null)
                    {
                        var name = vectorChildren == 1 ? Strip(pair.Key) : Strip(pair.Key) + "." + child.Key;
                        result.Add(new SgSetting { Node = node, Path = path, Name = name, Kind = shape });
                    }
                    else if (child.Value is JsonValue)
                    {
                        result.Add(Scalar(node, path, Strip(pair.Key) + "." + child.Key, child.Value));
                    }
                }
            }

            return result.Where(s => s.Raw != null).ToList();
        }

        static SgSetting Scalar(SgNode node, string[] path, string name, JsonNode value) => new SgSetting
        {
            Node = node,
            Path = path,
            Name = name,
            Kind = Json.Text(value) != null ? "string" : Json.Number(value) != null ? "int" : "bool",
        };

        /// <summary>"color" for an {r,g,b,a} object, "vector" for {x,y,z,w}, otherwise null.</summary>
        static string Shape(JsonNode node)
        {
            if (node is not JsonObject obj)
                return null;
            if (obj.ContainsKey("r") && obj.ContainsKey("g") && obj.ContainsKey("b"))
                return "color";
            if (obj.ContainsKey("x") && obj.ContainsKey("y"))
                return "vector";
            return null;
        }

        public static SgSetting Find(SgNode node, string name)
        {
            var settings = For(node);
            var match = settings.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase))
                        ?? settings.FirstOrDefault(s => string.Equals(s.Field, name, StringComparison.OrdinalIgnoreCase))
                        ?? settings.FirstOrDefault(s => Squash(s.Name).Equals(Squash(name), StringComparison.OrdinalIgnoreCase));
            return match;
        }

        public static SgSetting Require(SgNode node, string name)
        {
            var match = Find(node, name);
            if (match != null)
                return match;
            var available = For(node).Select(s => s.Name).ToList();
            throw new ShaderWaitressException(available.Count == 0
                ? $"{node.ShortId} ({node.TypeLabel}) has no settings"
                : $"{node.ShortId} ({node.TypeLabel}) has no setting '{name}'. Available: {string.Join(", ", available)}");
        }

        static string Strip(string field) =>
            field.StartsWith("m_", StringComparison.Ordinal) && field.Length > 2 ? field.Substring(2) : field;

        static string Squash(string s) => s?.Replace(" ", string.Empty).Replace("_", string.Empty) ?? string.Empty;
    }
}
