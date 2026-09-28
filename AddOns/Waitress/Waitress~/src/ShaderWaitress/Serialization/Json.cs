using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ShaderWaitress.Serialization
{
    /// <summary>
    /// Tolerant readers for JsonNode values. A node parsed from a file is backed by a
    /// JsonElement and converts freely, but one the tool assigned is backed by the exact CLR
    /// type it was given, so <c>GetValue&lt;double&gt;()</c> throws on a value that was set as
    /// an int. Everything reads through here instead.
    /// </summary>
    public static class Json
    {
        public static double? Number(JsonNode node)
        {
            if (node is not JsonValue value)
                return null;
            if (value.TryGetValue<JsonElement>(out var element))
                return element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out var d) ? d : (double?)null;
            if (value.TryGetValue<double>(out var asDouble))
                return asDouble;
            if (value.TryGetValue<float>(out var asFloat))
                return asFloat;
            if (value.TryGetValue<int>(out var asInt))
                return asInt;
            if (value.TryGetValue<long>(out var asLong))
                return asLong;
            if (value.TryGetValue<decimal>(out var asDecimal))
                return (double)asDecimal;
            if (value.TryGetValue<string>(out var text) &&
                double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                return parsed;
            return null;
        }

        public static double Number(JsonNode node, double fallback) => Number(node) ?? fallback;

        public static int Int(JsonNode node, int fallback)
        {
            var d = Number(node);
            return d.HasValue ? (int)Math.Round(d.Value) : fallback;
        }

        public static bool? Bool(JsonNode node)
        {
            if (node is not JsonValue value)
                return null;
            if (value.TryGetValue<JsonElement>(out var element))
            {
                return element.ValueKind switch
                {
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Number => element.TryGetDouble(out var n) && n != 0,
                    _ => null,
                };
            }
            if (value.TryGetValue<bool>(out var asBool))
                return asBool;
            var number = Number(node);
            return number.HasValue ? number.Value != 0 : (bool?)null;
        }

        public static bool Bool(JsonNode node, bool fallback) => Bool(node) ?? fallback;

        public static string Text(JsonNode node)
        {
            if (node is not JsonValue value)
                return null;
            if (value.TryGetValue<JsonElement>(out var element))
                return element.ValueKind == JsonValueKind.String ? element.GetString() : null;
            return value.TryGetValue<string>(out var s) ? s : null;
        }
    }
}
