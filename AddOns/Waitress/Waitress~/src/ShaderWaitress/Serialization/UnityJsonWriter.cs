using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ShaderWaitress.Serialization
{
    /// <summary>
    /// Reproduces UnityEngine.JsonUtility.ToJson(o, prettyPrint: true) formatting: 4-space
    /// indent, ": " between key and value, empty collections on one line. Numbers that came
    /// from the file are emitted from their original text so float formatting survives a
    /// read/modify/write cycle untouched.
    /// </summary>
    public static class UnityJsonWriter
    {
        public static string Write(JsonNode node)
        {
            var sb = new StringBuilder();
            WriteNode(sb, node, 0);
            return sb.ToString();
        }

        static void WriteNode(StringBuilder sb, JsonNode node, int indent)
        {
            switch (node)
            {
                case null:
                    sb.Append("null");
                    return;
                case JsonObject obj:
                    WriteObject(sb, obj, indent);
                    return;
                case JsonArray arr:
                    WriteArray(sb, arr, indent);
                    return;
                default:
                    WriteValue(sb, (JsonValue)node);
                    return;
            }
        }

        static void WriteObject(StringBuilder sb, JsonObject obj, int indent)
        {
            var count = 0;
            foreach (var _ in obj)
            {
                count++;
                break;
            }
            if (count == 0)
            {
                sb.Append("{}");
                return;
            }

            sb.Append("{\n");
            var inner = indent + 1;
            var first = true;
            foreach (var pair in obj)
            {
                if (!first)
                    sb.Append(",\n");
                first = false;
                Indent(sb, inner);
                sb.Append('"');
                EscapeString(sb, pair.Key);
                sb.Append("\": ");
                WriteNode(sb, pair.Value, inner);
            }
            sb.Append('\n');
            Indent(sb, indent);
            sb.Append('}');
        }

        static void WriteArray(StringBuilder sb, JsonArray arr, int indent)
        {
            if (arr.Count == 0)
            {
                sb.Append("[]");
                return;
            }

            sb.Append("[\n");
            var inner = indent + 1;
            for (var i = 0; i < arr.Count; i++)
            {
                if (i != 0)
                    sb.Append(",\n");
                Indent(sb, inner);
                WriteNode(sb, arr[i], inner);
            }
            sb.Append('\n');
            Indent(sb, indent);
            sb.Append(']');
        }

        static void WriteValue(StringBuilder sb, JsonValue value)
        {
            // Values that were parsed from a file carry their original text; reuse it so
            // float round-tripping is exact.
            if (value.TryGetValue<JsonElement>(out var element))
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Number:
                        sb.Append(element.GetRawText());
                        return;
                    case JsonValueKind.True:
                        sb.Append("true");
                        return;
                    case JsonValueKind.False:
                        sb.Append("false");
                        return;
                    case JsonValueKind.Null:
                        sb.Append("null");
                        return;
                    case JsonValueKind.String:
                        sb.Append('"');
                        EscapeString(sb, element.GetString());
                        sb.Append('"');
                        return;
                }
            }

            if (value.TryGetValue<string>(out var s))
            {
                sb.Append('"');
                EscapeString(sb, s);
                sb.Append('"');
                return;
            }
            if (value.TryGetValue<bool>(out var b))
            {
                sb.Append(b ? "true" : "false");
                return;
            }
            if (value.TryGetValue<int>(out var i))
            {
                sb.Append(i.ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (value.TryGetValue<long>(out var l))
            {
                sb.Append(l.ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (value.TryGetValue<double>(out var d))
            {
                sb.Append(FormatFloat(d));
                return;
            }
            sb.Append(value.ToJsonString());
        }

        /// <summary>Formats like Unity: integral values keep a ".0" so they stay floats.</summary>
        public static string FormatFloat(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d))
                return "0.0";
            var text = ((float)d).ToString("R", CultureInfo.InvariantCulture);
            if (text.IndexOf('.') < 0 && text.IndexOf('E') < 0 && text.IndexOf('e') < 0)
                text += ".0";
            return text;
        }

        static void Indent(StringBuilder sb, int levels)
        {
            sb.Append(' ', levels * 4);
        }

        static void EscapeString(StringBuilder sb, string s)
        {
            if (s == null)
                return;
            foreach (var c in s)
            {
                switch (c)
                {
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < 0x20)
                            sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else
                            sb.Append(c);
                        break;
                }
            }
        }

        /// <summary>Convenience helpers used throughout the model layer.</summary>
        public static JsonObject Ref(string objectId) => new JsonObject { ["m_Id"] = objectId ?? string.Empty };

        public static string RefId(JsonNode node)
        {
            if (node is JsonObject o && o.TryGetPropertyValue("m_Id", out var v))
                return (string)v ?? string.Empty;
            return string.Empty;
        }

        public static JsonArray RefArray(IEnumerable<string> ids)
        {
            var arr = new JsonArray();
            foreach (var id in ids)
                arr.Add(Ref(id));
            return arr;
        }
    }
}
