using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    /// <summary>Value shape of a port, independent of which slot class implements it.</summary>
    public enum SlotKind
    {
        Unknown,
        Float,
        Vector2,
        Vector3,
        Vector4,
        Color3,
        Color4,
        Boolean,
        Matrix2,
        Matrix3,
        Matrix4,
        DynamicVector,
        DynamicValue,
        DynamicMatrix,
        Texture2D,
        Texture2DArray,
        Texture3D,
        Cubemap,
        VirtualTexture,
        SamplerState,
        Gradient,
        PropertyConnectionState,
    }

    public static class SlotTypes
    {
        const string k_Ns = "UnityEditor.ShaderGraph.";

        struct Info
        {
            public SlotKind Kind;
            public string ValueField;   // where the value lives; null when the slot has none
            public int Components;      // components in a vector value; 0 when not a vector
        }

        static readonly Dictionary<string, Info> s_ByType = new Dictionary<string, Info>(StringComparer.Ordinal)
        {
            ["Vector1MaterialSlot"] = new Info { Kind = SlotKind.Float, ValueField = "m_Value", Components = 1 },
            ["Vector2MaterialSlot"] = new Info { Kind = SlotKind.Vector2, ValueField = "m_Value", Components = 2 },
            ["Vector3MaterialSlot"] = new Info { Kind = SlotKind.Vector3, ValueField = "m_Value", Components = 3 },
            ["Vector4MaterialSlot"] = new Info { Kind = SlotKind.Vector4, ValueField = "m_Value", Components = 4 },
            ["DefaultVector2MaterialSlot"] = new Info { Kind = SlotKind.Vector2, ValueField = "m_Value", Components = 2 },
            ["DefaultVector4MaterialSlot"] = new Info { Kind = SlotKind.Vector4, ValueField = "m_Value", Components = 4 },
            ["ColorRGBMaterialSlot"] = new Info { Kind = SlotKind.Color3, ValueField = "m_Value", Components = 3 },
            ["ColorRGBAMaterialSlot"] = new Info { Kind = SlotKind.Color4, ValueField = "m_Value", Components = 4 },
            ["BooleanMaterialSlot"] = new Info { Kind = SlotKind.Boolean, ValueField = "m_Value", Components = 0 },
            ["Matrix2MaterialSlot"] = new Info { Kind = SlotKind.Matrix2, ValueField = "m_Value" },
            ["Matrix3MaterialSlot"] = new Info { Kind = SlotKind.Matrix3, ValueField = "m_Value" },
            ["Matrix4MaterialSlot"] = new Info { Kind = SlotKind.Matrix4, ValueField = "m_Value" },
            ["DynamicMatrixMaterialSlot"] = new Info { Kind = SlotKind.DynamicMatrix, ValueField = "m_Value" },
            ["DynamicVectorMaterialSlot"] = new Info { Kind = SlotKind.DynamicVector, ValueField = "m_Value", Components = 4 },
            ["DynamicValueMaterialSlot"] = new Info { Kind = SlotKind.DynamicValue, ValueField = "m_Value" },
            ["UVMaterialSlot"] = new Info { Kind = SlotKind.Vector2, ValueField = "m_Value", Components = 2 },
            ["ScreenPositionMaterialSlot"] = new Info { Kind = SlotKind.Vector4, ValueField = "m_Value", Components = 4 },
            ["PositionMaterialSlot"] = new Info { Kind = SlotKind.Vector3, ValueField = "m_Value", Components = 3 },
            ["NormalMaterialSlot"] = new Info { Kind = SlotKind.Vector3, ValueField = "m_Value", Components = 3 },
            ["TangentMaterialSlot"] = new Info { Kind = SlotKind.Vector3, ValueField = "m_Value", Components = 3 },
            ["BitangentMaterialSlot"] = new Info { Kind = SlotKind.Vector3, ValueField = "m_Value", Components = 3 },
            ["ViewDirectionMaterialSlot"] = new Info { Kind = SlotKind.Vector3, ValueField = "m_Value", Components = 3 },
            ["VertexColorMaterialSlot"] = new Info { Kind = SlotKind.Vector4, ValueField = "m_Value", Components = 4 },
            ["Texture2DMaterialSlot"] = new Info { Kind = SlotKind.Texture2D, ValueField = null },
            ["Texture2DInputMaterialSlot"] = new Info { Kind = SlotKind.Texture2D, ValueField = "m_Texture" },
            ["Texture2DArrayMaterialSlot"] = new Info { Kind = SlotKind.Texture2DArray, ValueField = null },
            ["Texture2DArrayInputMaterialSlot"] = new Info { Kind = SlotKind.Texture2DArray, ValueField = "m_TextureArray" },
            ["Texture3DMaterialSlot"] = new Info { Kind = SlotKind.Texture3D, ValueField = null },
            ["Texture3DInputMaterialSlot"] = new Info { Kind = SlotKind.Texture3D, ValueField = "m_Texture" },
            ["CubemapMaterialSlot"] = new Info { Kind = SlotKind.Cubemap, ValueField = null },
            ["CubemapInputMaterialSlot"] = new Info { Kind = SlotKind.Cubemap, ValueField = "m_Cubemap" },
            ["VirtualTextureMaterialSlot"] = new Info { Kind = SlotKind.VirtualTexture, ValueField = null },
            ["VirtualTextureInputMaterialSlot"] = new Info { Kind = SlotKind.VirtualTexture, ValueField = null },
            ["SamplerStateMaterialSlot"] = new Info { Kind = SlotKind.SamplerState, ValueField = null },
            ["GradientMaterialSlot"] = new Info { Kind = SlotKind.Gradient, ValueField = null },
            ["GradientInputMaterialSlot"] = new Info { Kind = SlotKind.Gradient, ValueField = "m_Value" },
            ["PropertyConnectionStateMaterialSlot"] = new Info { Kind = SlotKind.PropertyConnectionState, ValueField = "m_Value" },
        };

        static readonly Dictionary<SlotKind, string> s_DefaultType = new Dictionary<SlotKind, string>
        {
            [SlotKind.Float] = "Vector1MaterialSlot",
            [SlotKind.Vector2] = "Vector2MaterialSlot",
            [SlotKind.Vector3] = "Vector3MaterialSlot",
            [SlotKind.Vector4] = "Vector4MaterialSlot",
            [SlotKind.Color3] = "ColorRGBMaterialSlot",
            [SlotKind.Color4] = "ColorRGBAMaterialSlot",
            [SlotKind.Boolean] = "BooleanMaterialSlot",
            [SlotKind.Matrix2] = "Matrix2MaterialSlot",
            [SlotKind.Matrix3] = "Matrix3MaterialSlot",
            [SlotKind.Matrix4] = "Matrix4MaterialSlot",
            [SlotKind.DynamicVector] = "DynamicVectorMaterialSlot",
            [SlotKind.DynamicValue] = "DynamicValueMaterialSlot",
            [SlotKind.DynamicMatrix] = "DynamicMatrixMaterialSlot",
            [SlotKind.Texture2D] = "Texture2DInputMaterialSlot",
            [SlotKind.Texture2DArray] = "Texture2DArrayInputMaterialSlot",
            [SlotKind.Texture3D] = "Texture3DInputMaterialSlot",
            [SlotKind.Cubemap] = "CubemapInputMaterialSlot",
            [SlotKind.SamplerState] = "SamplerStateMaterialSlot",
            [SlotKind.Gradient] = "GradientInputMaterialSlot",
            [SlotKind.PropertyConnectionState] = "PropertyConnectionStateMaterialSlot",
        };

        public static string ShortName(string fullTypeName)
        {
            if (fullTypeName == null)
                return null;
            var i = fullTypeName.LastIndexOf('.');
            return i < 0 ? fullTypeName : fullTypeName.Substring(i + 1);
        }

        public static SlotKind KindOf(string fullTypeName)
        {
            var s = ShortName(fullTypeName);
            return s != null && s_ByType.TryGetValue(s, out var info) ? info.Kind : SlotKind.Unknown;
        }

        public static string ValueFieldOf(string fullTypeName)
        {
            var s = ShortName(fullTypeName);
            return s != null && s_ByType.TryGetValue(s, out var info) ? info.ValueField : null;
        }

        public static int ComponentsOf(string fullTypeName)
        {
            var s = ShortName(fullTypeName);
            return s != null && s_ByType.TryGetValue(s, out var info) ? info.Components : 0;
        }

        public static bool IsKnown(string fullTypeName) => s_ByType.ContainsKey(ShortName(fullTypeName) ?? string.Empty);

        public static string SlotTypeForKind(SlotKind kind)
        {
            return s_DefaultType.TryGetValue(kind, out var t) ? k_Ns + t : null;
        }

        public static SlotKind ParseKind(string text)
        {
            if (string.IsNullOrEmpty(text))
                return SlotKind.Unknown;
            switch (text.Trim().ToLowerInvariant())
            {
                case "float":
                case "vector1":
                case "1": return SlotKind.Float;
                case "vector2":
                case "float2":
                case "2": return SlotKind.Vector2;
                case "vector3":
                case "float3":
                case "3": return SlotKind.Vector3;
                case "vector4":
                case "float4":
                case "4": return SlotKind.Vector4;
                case "color": return SlotKind.Color4;
                case "color3": return SlotKind.Color3;
                case "bool":
                case "boolean": return SlotKind.Boolean;
                case "matrix2": return SlotKind.Matrix2;
                case "matrix3": return SlotKind.Matrix3;
                case "matrix4": return SlotKind.Matrix4;
                case "texture2d": return SlotKind.Texture2D;
                case "texture2darray": return SlotKind.Texture2DArray;
                case "texture3d": return SlotKind.Texture3D;
                case "cubemap": return SlotKind.Cubemap;
                case "samplerstate": return SlotKind.SamplerState;
                case "gradient": return SlotKind.Gradient;
                case "virtualtexture": return SlotKind.VirtualTexture;
                default: return SlotKind.Unknown;
            }
        }

        /// <summary>Human-facing type label used in the dense text form.</summary>
        public static string Label(SlotKind kind)
        {
            switch (kind)
            {
                case SlotKind.Float: return "Float";
                case SlotKind.Vector2: return "Vector2";
                case SlotKind.Vector3: return "Vector3";
                case SlotKind.Vector4: return "Vector4";
                case SlotKind.Color3: return "Color3";
                case SlotKind.Color4: return "Color";
                case SlotKind.Boolean: return "Bool";
                case SlotKind.DynamicVector: return "Dynamic";
                case SlotKind.DynamicValue: return "DynamicValue";
                case SlotKind.DynamicMatrix: return "DynamicMatrix";
                default: return kind.ToString();
            }
        }

        static readonly string[] k_VectorFields = { "x", "y", "z", "w" };

        /// <summary>
        /// Renders a slot value compactly: <c>0.5</c>, <c>(1, 0, 0)</c>, <c>true</c>,
        /// <c>tex:fd7e45…</c>. Returns null when there is nothing worth printing.
        /// </summary>
        public static string FormatValue(SlotKind kind, JsonNode value)
        {
            if (value == null)
                return null;
            switch (kind)
            {
                case SlotKind.Float:
                    return FormatNumber(value);
                case SlotKind.Boolean:
                    return Json.Bool(value, false) ? "true" : "false";
                case SlotKind.Vector2:
                case SlotKind.Vector3:
                case SlotKind.Vector4:
                case SlotKind.Color3:
                case SlotKind.Color4:
                case SlotKind.DynamicVector:
                    return FormatVector(value, ComponentsForKind(kind));
                case SlotKind.DynamicValue:
                    // Backed by a Matrix4x4, but Shader Graph reads and edits only row 0 unless
                    // the resolved type is an actual matrix, so that row is the literal.
                    return FormatMatrixRow0(value);
                case SlotKind.Matrix2:
                    return FormatMatrix(value, 2);
                case SlotKind.Matrix3:
                    return FormatMatrix(value, 3);
                case SlotKind.Matrix4:
                case SlotKind.DynamicMatrix:
                    return FormatMatrix(value, 4);
                case SlotKind.PropertyConnectionState:
                    return Json.Bool(value, false) ? "true" : "false";
                case SlotKind.Texture2D:
                case SlotKind.Texture2DArray:
                case SlotKind.Texture3D:
                case SlotKind.Cubemap:
                    return FormatTexture(value);
                case SlotKind.Gradient:
                    return "gradient";
                default:
                    return null;
            }
        }

        /// <summary>Number of components the caller should see for a value of this kind.</summary>
        public static int ComponentsForKind(SlotKind kind) => kind switch
        {
            SlotKind.Float => 1,
            SlotKind.Vector2 => 2,
            SlotKind.Vector3 or SlotKind.Color3 => 3,
            _ => 4,
        };

        static string FormatVector(JsonNode value, int components)
        {
            if (value is not JsonObject obj)
                return FormatNumber(value);
            var parts = new List<string>();
            foreach (var f in k_VectorFields)
            {
                if (parts.Count >= components)
                    break;
                if (obj.TryGetPropertyValue(f, out var v) && v != null)
                    parts.Add(FormatNumber(v));
            }
            if (parts.Count == 0)
            {
                foreach (var f in new[] { "r", "g", "b", "a" })
                {
                    if (parts.Count >= components)
                        break;
                    if (obj.TryGetPropertyValue(f, out var v) && v != null)
                        parts.Add(FormatNumber(v));
                }
            }
            return parts.Count == 0 ? null : "(" + string.Join(", ", parts) + ")";
        }

        static readonly string[] k_MatrixRows = { "e0", "e1", "e2", "e3" };

        /// <summary>
        /// Row 0 of a Matrix4x4-backed dynamic slot, collapsed to a single number when every
        /// component matches — which is how a scalar entered in the Editor is stored.
        /// </summary>
        static string FormatMatrixRow0(JsonNode value)
        {
            if (value is not JsonObject obj)
                return FormatNumber(value);
            var parts = new List<string>();
            for (var i = 0; i < 4; i++)
            {
                if (obj.TryGetPropertyValue("e0" + i, out var v) && v != null)
                    parts.Add(FormatNumber(v));
            }
            if (parts.Count == 0)
                return FormatVector(value, 4);
            if (parts.All(p => p == parts[0]))
                return parts[0];
            return "(" + string.Join(", ", parts) + ")";
        }

        static string FormatMatrix(JsonNode value, int size)
        {
            if (value is not JsonObject obj)
                return null;
            var rows = new List<string>();
            var identity = true;
            for (var r = 0; r < size; r++)
            {
                var cells = new List<string>();
                for (var c = 0; c < size; c++)
                {
                    obj.TryGetPropertyValue(k_MatrixRows[r] + c, out var v);
                    var text = FormatNumber(v);
                    cells.Add(text);
                    if (text != (r == c ? "1" : "0"))
                        identity = false;
                }
                rows.Add(string.Join(", ", cells));
            }
            return identity ? "identity" : "[" + string.Join("; ", rows) + "]";
        }

        static string FormatTexture(JsonNode value)
        {
            if (value is not JsonObject obj)
                return null;
            foreach (var key in new[] { "m_SerializedTexture", "m_SerializedCubemap", "m_SerializedTextureArray", "m_SerializedTexture3D" })
            {
                if (!obj.TryGetPropertyValue(key, out var v) || v == null)
                    continue;
                var s = v.GetValue<string>();
                if (string.IsNullOrEmpty(s))
                    return null;
                var g = s.IndexOf("\"guid\":\"", StringComparison.Ordinal);
                if (g < 0)
                    return null;
                var start = g + 8;
                var end = s.IndexOf('"', start);
                if (end < 0)
                    return null;
                var guid = s.Substring(start, end - start);
                if (guid.Length == 0 || guid.TrimStart(new[] { '0' }).Length == 0)
                    return null;
                return "tex:" + guid.Substring(0, Math.Min(8, guid.Length));
            }
            return null;
        }

        public static string FormatNumber(JsonNode value)
        {
            if (value == null)
                return "0";
            double d;
            try
            {
                d = Json.Number(value, 0);
            }
            catch (Exception)
            {
                return value.ToJsonString();
            }
            if (Math.Abs(d - Math.Round(d)) < 1e-9 && Math.Abs(d) < 1e15)
                return ((long)Math.Round(d)).ToString(CultureInfo.InvariantCulture);
            return d.ToString("0.######", CultureInfo.InvariantCulture);
        }

        /// <summary>Structural equality used to decide whether a value is still the default.</summary>
        public static bool ValueEquals(JsonNode a, JsonNode b)
        {
            if (a == null || b == null)
                return ReferenceEquals(a, b);
            return string.Equals(Canonical(a), Canonical(b), StringComparison.Ordinal);
        }

        static string Canonical(JsonNode node)
        {
            var sb = new StringBuilder();
            Canonical(node, sb);
            return sb.ToString();
        }

        static void Canonical(JsonNode node, StringBuilder sb)
        {
            switch (node)
            {
                case null:
                    sb.Append("null");
                    return;
                case JsonObject obj:
                    sb.Append('{');
                    foreach (var pair in obj)
                    {
                        sb.Append(pair.Key).Append(':');
                        Canonical(pair.Value, sb);
                        sb.Append(',');
                    }
                    sb.Append('}');
                    return;
                case JsonArray arr:
                    sb.Append('[');
                    foreach (var item in arr)
                    {
                        Canonical(item, sb);
                        sb.Append(',');
                    }
                    sb.Append(']');
                    return;
                default:
                    var v = (JsonValue)node;
                    if (v.TryGetValue<double>(out var d))
                        sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
                    else
                        sb.Append(v.ToJsonString());
                    return;
            }
        }

        /// <summary>
        /// Parses a literal from the command line into the JSON shape a slot of this kind
        /// expects. Accepts <c>0.5</c>, <c>1,0,0</c>, <c>(1, 0, 0, 1)</c>, <c>true</c>.
        /// </summary>
        public static JsonNode ParseValue(SlotKind kind, string text)
        {
            if (text == null)
                throw new ShaderWaitressException("missing value");
            text = text.Trim();
            switch (kind)
            {
                case SlotKind.Boolean:
                    return JsonValue.Create(text is "1" or "true" or "True" or "TRUE");
                case SlotKind.Float:
                    return JsonValue.Create(ParseDouble(text));
                case SlotKind.Vector2:
                    return Vector(text, 2, "x", "y", "z", "w");
                case SlotKind.Vector3:
                case SlotKind.Color3:
                    return Vector(text, 3, "x", "y", "z", "w");
                case SlotKind.Vector4:
                case SlotKind.Color4:
                case SlotKind.DynamicVector:
                    return Vector(text, 4, "x", "y", "z", "w");
                case SlotKind.DynamicValue:
                    return MatrixRow0(text);
                default:
                    throw new ShaderWaitressException($"cannot set a literal on a {Label(kind)} port");
            }
        }

        /// <summary>
        /// Builds the Matrix4x4 a dynamic slot stores. Row 0 carries the literal and the rest
        /// stays identity, which is what the Editor writes for a scalar or vector entry.
        /// </summary>
        static JsonNode MatrixRow0(string text)
        {
            var row = (JsonObject)Vector(text, 4, "x", "y", "z", "w");
            var obj = new JsonObject();
            var names = new[] { "x", "y", "z", "w" };
            for (var r = 0; r < 4; r++)
            {
                for (var c = 0; c < 4; c++)
                {
                    obj["e" + r + c] = r == 0
                        ? Number(row[names[c]])
                        : (r == c ? 1.0 : 0.0);
                }
            }
            return obj;
        }

        static double Number(JsonNode node) => Json.Number(node, 0);

        static JsonNode Vector(string text, int components, params string[] fields)
        {
            var trimmed = text.Trim().TrimStart('(', '[').TrimEnd(')', ']');
            var pieces = trimmed.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (pieces.Length == 1)
            {
                var all = ParseDouble(pieces[0]);
                pieces = new string[components];
                for (var i = 0; i < components; i++)
                    pieces[i] = all.ToString("R", CultureInfo.InvariantCulture);
            }
            if (pieces.Length > components)
                throw new ShaderWaitressException($"expected at most {components} components, got {pieces.Length}");
            var obj = new JsonObject();
            for (var i = 0; i < components; i++)
                obj[fields[i]] = JsonValue.Create(i < pieces.Length ? ParseDouble(pieces[i]) : 0.0);
            return obj;
        }

        public static double ParseDouble(string text)
        {
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                throw new ShaderWaitressException($"'{text}' is not a number");
            return d;
        }
    }
}
