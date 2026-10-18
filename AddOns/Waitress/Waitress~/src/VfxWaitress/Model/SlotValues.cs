using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace VfxWaitress.Model
{
    /// <summary>
    /// What VFX Graph can load from a slot with no child slots. VFXSerializer.Load parses the stored
    /// text by the slot's type, and a failure only logs. The slot's value becomes null, and a null
    /// curve or gradient reaching the expression sheet crashes the Editor natively during import.
    /// </summary>
    public static class SlotValues
    {
        const string k_Curve = "VFX Graph's curve JSON {\"frames\":[{\"time\":..,\"value\":..,\"inTangent\":..,\"outTangent\":..,\"tangentMode\":0," +
                               "\"leftTangentMode\":..,\"rightTangentMode\":..,\"broken\":false},...],\"preWrapMode\":8,\"postWrapMode\":8,\"version\":1}";
        const string k_Gradient = "VFX Graph's gradient JSON {\"colorKeys\":[{\"color\":{\"r\":..,\"g\":..,\"b\":..,\"a\":..},\"time\":..},...]," +
                                  "\"alphaKeys\":[{\"alpha\":..,\"time\":..},...],\"gradientMode\":0}";
        const string k_Reference = "an asset reference stored as {\"obj\":{\"fileID\":..,\"guid\":\"..\",\"type\":3}}";

        static readonly HashSet<string> s_References = new HashSet<string>(StringComparer.Ordinal)
        {
            "Texture2D", "Texture2DArray", "Texture3D", "Cubemap", "CubemapArray", "Mesh", "SkinnedMeshRenderer",
        };

        static readonly HashSet<string> s_Known = new HashSet<string>(s_References, StringComparer.Ordinal)
        {
            "Single", "Int32", "UInt32", "Boolean", "AnimationCurve", "Gradient", "Matrix4x4",
        };

        public static bool IsReference(string typeName) => typeName != null && s_References.Contains(typeName);

        /// <summary>A curve or gradient that loads as null crashes the import; other types just lose the value.</summary>
        public static bool NullCrashes(string typeName) => typeName is "AnimationCurve" or "Gradient";

        /// <summary>
        /// The text to store for <paramref name="value"/> on a leaf slot of <paramref name="typeName"/>,
        /// before YAML quoting. Throws when VFX Graph couldn't load it.
        /// </summary>
        public static string Encode(string typeName, string value, string where)
        {
            if (typeName == null || !s_Known.Contains(typeName))
                throw new VfxWaitressException($"{where} is a {typeName ?? "slot of unknown type"}, and the tool doesn't know how VFX Graph stores one; set it in the Editor");
            var v = value.Trim();
            if (typeName == "Boolean")
            {
                if (string.Equals(v, "true", StringComparison.OrdinalIgnoreCase) || v == "1")
                    return "True";
                if (string.Equals(v, "false", StringComparison.OrdinalIgnoreCase) || v == "0")
                    return "False";
            }
            if (typeName == "AnimationCurve" && v.Length > 0 && v[0] != '{')
                v = CurveJson(v, where);
            else if (typeName == "Gradient" && v.Length > 0 && v[0] != '{')
                v = GradientJson(v, where);
            var problem = v.Length == 0 ? "needs a value" : Problem(typeName, v);
            if (problem != null)
                throw new VfxWaitressException($"{where}: {problem}");
            return v;
        }

        /// <summary>
        /// Why VFX Graph can't load <paramref name="stored"/> (the scalar as the file holds it, YAML
        /// quotes included) into a leaf slot of <paramref name="typeName"/>. Null when it can, or
        /// when the type is one this can't judge.
        /// </summary>
        public static string Problem(string typeName, string stored)
        {
            if (typeName == null || !s_Known.Contains(typeName))
                return null;
            var v = Unquote(stored ?? "").Trim();
            if (s_References.Contains(typeName))
                return v.Length == 0 ? null : Json(v, "obj", 0, k_Reference);
            // An empty primitive loads as its default.
            switch (typeName)
            {
                case "Single":
                    return v.Length == 0 || float.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var f) && !float.IsInfinity(f)
                        ? null : $"'{Short(v)}' is not a number";
                case "Int32":
                    return v.Length == 0 || int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                        ? null : $"'{Short(v)}' is not a whole number";
                case "UInt32":
                    return v.Length == 0 || uint.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                        ? null : $"'{Short(v)}' is not a non-negative whole number";
                case "Boolean":
                    return v.Length == 0 || bool.TryParse(v, out _) ? null : $"'{Short(v)}' is not True or False";
                case "AnimationCurve":
                    return Json(v, "frames", 0, k_Curve);
                case "Gradient":
                    return Json(v, "colorKeys", 8, k_Gradient) ?? Json(v, "alphaKeys", 8, k_Gradient);
                default:
                    return Json(v, null, 0, "a Matrix4x4 stored as JSON {\"e00\":..,...,\"e33\":..}");
            }
        }

        /// <summary>
        /// <c>time:value,...</c> as a curve of straight segments, the way the curve editor's Linear
        /// tangents draw it. Past the ends the curve holds the end values.
        /// </summary>
        static string CurveJson(string text, string where)
        {
            var keys = Pairs(text, where, "time:value,... such as 0:1,1:0", s =>
                double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) && double.IsFinite(d) ? d : (double?)null);
            var sb = new System.Text.StringBuilder("{\"frames\":[");
            for (var i = 0; i < keys.Count; i++)
            {
                var inSlope = i > 0 ? Slope(keys[i - 1], keys[i]) : keys.Count > 1 ? Slope(keys[0], keys[1]) : 0.0;
                var outSlope = i < keys.Count - 1 ? Slope(keys[i], keys[i + 1]) : inSlope;
                if (i > 0)
                    sb.Append(',');
                sb.Append("{\"time\":").Append(Number(keys[i].time)).Append(",\"value\":").Append(Number(keys[i].value))
                  .Append(",\"inTangent\":").Append(Number(inSlope)).Append(",\"outTangent\":").Append(Number(outSlope))
                  .Append(",\"tangentMode\":0,\"leftTangentMode\":2,\"rightTangentMode\":2,\"broken\":true}");
            }
            return sb.Append("],\"preWrapMode\":8,\"postWrapMode\":8,\"version\":1}").ToString();
        }

        static double Slope((double time, double value) a, (double time, double value) b) => (b.value - a.value) / (b.time - a.time);

        /// <summary><c>time:#RRGGBB[AA],...</c> as a blended gradient, with an alpha key at every color key.</summary>
        static string GradientJson(string text, string where)
        {
            var keys = Pairs(text, where, "time:#RRGGBB[AA],... such as 0:#ff8000,1:#2040ff00", Hex);
            if (keys.Count > 8)
                throw new VfxWaitressException($"{where}: a gradient has at most 8 keys, got {keys.Count}");
            foreach (var key in keys)
            {
                if (key.time < 0 || key.time > 1)
                    throw new VfxWaitressException($"{where}: gradient key times run from 0 to 1, got {Number(key.time)}");
            }
            var colors = string.Join(",", keys.Select(k =>
                $"{{\"color\":{{\"r\":{Number(k.value.r)},\"g\":{Number(k.value.g)},\"b\":{Number(k.value.b)},\"a\":1.0}},\"time\":{Number(k.time)}}}"));
            var alphas = string.Join(",", keys.Select(k => $"{{\"alpha\":{Number(k.value.a)},\"time\":{Number(k.time)}}}"));
            return "{\"colorKeys\":[" + colors + "],\"alphaKeys\":[" + alphas + "],\"gradientMode\":0}";
        }

        static (double r, double g, double b, double a)? Hex(string s)
        {
            if (!s.StartsWith("#", StringComparison.Ordinal) || (s.Length != 7 && s.Length != 9) ||
                !uint.TryParse(s.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var bits))
                return null;
            if (s.Length == 7)
                bits = (bits << 8) | 0xff;
            return ((bits >> 24) / 255.0, ((bits >> 16) & 0xff) / 255.0, ((bits >> 8) & 0xff) / 255.0, (bits & 0xff) / 255.0);
        }

        /// <summary>Comma-separated <c>time:value</c> pairs, sorted by time. Two keys may not share a time.</summary>
        static List<(double time, T value)> Pairs<T>(string text, string where, string form, Func<string, T?> parse) where T : struct
        {
            var keys = new List<(double time, T value)>();
            foreach (var part in text.Split(','))
            {
                var colon = part.IndexOf(':');
                T? value = colon > 0 ? parse(part.Substring(colon + 1).Trim()) : null;
                if (colon <= 0 || value == null ||
                    !double.TryParse(part.Substring(0, colon).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var time) || !double.IsFinite(time))
                    throw new VfxWaitressException($"{where}: '{Short(part.Trim())}' is not a key; give {form}, or VFX Graph's own JSON");
                keys.Add((time, value.Value));
            }
            keys.Sort((a, b) => a.time.CompareTo(b.time));
            for (var i = 1; i < keys.Count; i++)
            {
                if (keys[i].time == keys[i - 1].time)
                    throw new VfxWaitressException($"{where}: two keys at time {Number(keys[i].time)}");
            }
            return keys;
        }

        /// <summary>Whole numbers keep a ".0", the way JsonUtility writes a float.</summary>
        static string Number(double d) =>
            d == Math.Floor(d) && Math.Abs(d) < 1e15 ? d.ToString("0.0", CultureInfo.InvariantCulture) : d.ToString("R", CultureInfo.InvariantCulture);

        static string Json(string v, string key, int maxItems, string what)
        {
            if (v.Length == 0)
                return $"is empty; it needs {what}";
            try
            {
                // JsonUtility writes a constant tangent as a bare Infinity, which strict JSON lacks.
                var strict = Regex.Replace(v, @"(?<=[:\[,]\s*)-?(?:Infinity|NaN)(?=\s*[,\]}])", "0");
                using var doc = JsonDocument.Parse(strict);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                    return $"'{Short(v)}' is not a JSON object; it needs {what}";
                if (key == null)
                    return null;
                if (!root.TryGetProperty(key, out var member))
                    return $"has no \"{key}\"; it needs {what}";
                if (key == "obj")
                    return member.ValueKind == JsonValueKind.Object ? null : $"\"obj\" is not an object; it needs {what}";
                if (member.ValueKind != JsonValueKind.Array)
                    return $"\"{key}\" is not a list; it needs {what}";
                if (maxItems > 0 && member.GetArrayLength() > maxItems)
                    return $"\"{key}\" has {member.GetArrayLength()} keys; Unity allows at most {maxItems}";
                return null;
            }
            catch (JsonException)
            {
                return $"'{Short(v)}' is not JSON; it needs {what}";
            }
        }

        static string Unquote(string s)
        {
            var t = s.Trim();
            if (t.Length >= 2 && t[0] == '\'' && t[t.Length - 1] == '\'')
                return t.Substring(1, t.Length - 2).Replace("''", "'");
            return t;
        }

        static string Short(string s) => s.Length > 40 ? s.Substring(0, 37) + "..." : s;
    }
}
