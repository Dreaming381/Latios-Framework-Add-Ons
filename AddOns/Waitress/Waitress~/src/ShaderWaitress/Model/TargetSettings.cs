using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Friendly names for the render-pipeline target's serialized fields. Unknown fields are
    /// still readable and writable through their raw json name, so a target from a pipeline
    /// this table has never heard of is not a dead end.
    /// </summary>
    public static class TargetSettings
    {
        sealed class Setting
        {
            public string Cli;
            public string Field;
            public string[] Values;      // enum labels indexed by value, null for bool/other
            public int[] Codes;          // explicit codes when they are not 0..n-1
            public string[] Aliases;    // alternative spellings, index-matched to Values
            public bool IsBool;
        }

        static readonly Setting[] s_Settings =
        {
            new Setting { Cli = "surface", Field = "m_SurfaceType", Values = new[] { "opaque", "transparent" } },
            new Setting { Cli = "blend", Field = "m_AlphaMode", Values = new[] { "alpha", "premultiply", "additive", "multiply" } },
            // Unity's field says which faces to KEEP, so "cull=front" would read as "cull the
            // front" while meaning "render the front". The key is named after the field, and
            // "cull" is only accepted where it's unambiguous.
            new Setting
            {
                Cli = "renderface", Field = "m_RenderFace",
                Values = new[] { "both", "back", "front" }, Codes = new[] { 0, 1, 2 },
                Aliases = new[] { "off", null, null },
            },
            new Setting { Cli = "zwrite", Field = "m_ZWriteControl", Values = new[] { "auto", "on", "off" } },
            new Setting
            {
                Cli = "ztest", Field = "m_ZTestMode",
                Values = new[] { "disabled", "never", "less", "equal", "lequal", "greater", "notequal", "gequal", "always" },
            },
            new Setting { Cli = "alphaclip", Field = "m_AlphaClip", IsBool = true },
            new Setting { Cli = "castshadows", Field = "m_CastShadows", IsBool = true },
            new Setting { Cli = "receiveshadows", Field = "m_ReceiveShadows", IsBool = true },
            new Setting { Cli = "materialoverride", Field = "m_AllowMaterialOverride", IsBool = true },
            new Setting { Cli = "lodcrossfade", Field = "m_SupportsLODCrossFade", IsBool = true },
            new Setting { Cli = "vfx", Field = "m_SupportVFX", IsBool = true },
            new Setting { Cli = "customeditor", Field = "m_CustomEditorGUI" },
        };

        public static IEnumerable<string> Describe(SgTarget target)
        {
            var node = target.Entry.Node;
            foreach (var setting in s_Settings)
            {
                if (!node.TryGetPropertyValue(setting.Field, out var value) || value == null)
                    continue;
                var text = Format(setting, value);
                if (text == null)
                    continue;
                yield return setting.Cli + "=" + text;
            }
        }

        static string Format(Setting setting, JsonNode value)
        {
            if (setting.IsBool)
                return ShaderWaitress.Serialization.Json.Bool(value, false) ? "on" : "off";
            if (setting.Values == null)
            {
                var s = value.GetValue<string>();
                return string.IsNullOrEmpty(s) ? null : "\"" + s + "\"";
            }
            var code = ShaderWaitress.Serialization.Json.Int(value, 0);
            var index = setting.Codes == null ? code : Array.IndexOf(setting.Codes, code);
            if (index >= 0 && index < setting.Values.Length)
                return setting.Values[index];
            return code.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Applies "surface=transparent" style assignments. Returns what changed.</summary>
        public static string Apply(SgTarget target, string key, string value)
        {
            if (string.Equals(key, "cull", StringComparison.OrdinalIgnoreCase))
                (key, value) = TranslateCull(value);

            var setting = s_Settings.FirstOrDefault(s => string.Equals(s.Cli, key, StringComparison.OrdinalIgnoreCase))
                          ?? s_Settings.FirstOrDefault(s => string.Equals(s.Field, key, StringComparison.OrdinalIgnoreCase));
            var node = target.Entry.Edit();

            if (setting == null)
            {
                if (!node.ContainsKey(key))
                    throw new ShaderWaitressException($"unknown target setting '{key}'. Known: " + string.Join(", ", s_Settings.Select(s => s.Cli)));
                node[key] = ParseRaw(node[key], value);
                return $"{key}={value}";
            }

            if (setting.IsBool)
            {
                var on = value is "on" or "true" or "1" or "yes";
                node[setting.Field] = on;
                return $"{setting.Cli}={(on ? "on" : "off")}";
            }
            if (setting.Values == null)
            {
                node[setting.Field] = value;
                return $"{setting.Cli}=\"{value}\"";
            }

            var index = Array.FindIndex(setting.Values, v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
            if (index < 0 && setting.Aliases != null)
                index = Array.FindIndex(setting.Aliases, v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var raw))
                {
                    node[setting.Field] = raw;
                    return $"{setting.Cli}={raw}";
                }
                throw new ShaderWaitressException($"'{value}' is not valid for {setting.Cli}. Options: " + string.Join(", ", setting.Values));
            }
            var code = setting.Codes == null ? index : setting.Codes[index];
            node[setting.Field] = code;
            return $"{setting.Cli}={setting.Values[index]}";
        }

        /// <summary>
        /// "cull=off" is unambiguous and means render both faces. "cull=front" is not: it could
        /// mean cull the front or keep the front, and Unity's field means the second. Rather
        /// than pick for the caller, it is refused and pointed at the unambiguous key.
        /// </summary>
        static (string key, string value) TranslateCull(string value)
        {
            switch (value?.ToLowerInvariant())
            {
                case "off":
                case "none":
                case "both":
                    return ("renderface", "both");
                default:
                    throw new ShaderWaitressException(
                        $"'cull={value}' is ambiguous. Unity stores which faces to render, not which to cull, " +
                        "so use renderface=front (cull back), renderface=back (cull front) or renderface=both (cull off).");
            }
        }

        static JsonNode ParseRaw(JsonNode existing, string value)
        {
            if (existing is JsonValue v)
            {
                if (v.TryGetValue<bool>(out _))
                    return value is "on" or "true" or "1" or "yes";
                if (v.TryGetValue<double>(out _) && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                    return d;
            }
            return value;
        }

        public static IEnumerable<string> KnownKeys => s_Settings.Select(s => s.Cli);
    }
}
