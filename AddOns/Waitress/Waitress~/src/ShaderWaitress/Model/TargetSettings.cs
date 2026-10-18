using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Friendly names for a render-pipeline target's serialized fields. A setting can live on the
    /// target, on its sub-target (URP's normal space), or on one of the data objects HDRP keeps
    /// most of its settings in, so each key lists the object types that hold it. Unknown fields
    /// are still readable and writable through their raw json name, so a target from a pipeline
    /// this table has never heard of is not a dead end.
    /// </summary>
    public static class TargetSettings
    {
        sealed class Setting
        {
            public string Cli;
            public string[] Fields;     // "OwnerType.m_Field", OwnerType being the short type name
            public string[] Values;     // enum labels indexed by value, null for bool/other
            public int[] Codes;         // explicit codes when they are not 0..n-1
            public string[] Aliases;    // alternative spellings, index-matched to Values
            public bool IsBool;
        }

        static readonly Setting[] s_Settings =
        {
            new Setting
            {
                Cli = "surface", Values = new[] { "opaque", "transparent" },
                Fields = new[] { "UniversalTarget.m_SurfaceType", "BuiltInTarget.m_SurfaceType", "SystemData.m_SurfaceType" },
            },
            new Setting
            {
                Cli = "blend", Values = new[] { "alpha", "premultiply", "additive", "multiply" },
                Fields = new[] { "UniversalTarget.m_AlphaMode", "BuiltInTarget.m_AlphaMode" },
            },
            // Unity's field says which faces to KEEP, so "cull=front" would read as "cull the
            // front" while meaning "render the front". The key is named after the field, and
            // "cull" is only accepted where it's unambiguous.
            new Setting
            {
                Cli = "renderface", Values = new[] { "both", "back", "front" }, Codes = new[] { 0, 1, 2 },
                Aliases = new[] { "off", null, null },
                Fields = new[] { "UniversalTarget.m_RenderFace", "BuiltInTarget.m_RenderFace" },
            },
            new Setting
            {
                Cli = "zwrite", Values = new[] { "auto", "on", "off" },
                Fields = new[] { "UniversalTarget.m_ZWriteControl", "BuiltInTarget.m_ZWriteControl" },
            },
            new Setting
            {
                Cli = "ztest",
                Values = new[] { "disabled", "never", "less", "equal", "lequal", "greater", "notequal", "gequal", "always" },
                Fields = new[] { "UniversalTarget.m_ZTestMode", "BuiltInTarget.m_ZTestMode", "SystemData.m_ZTest" },
            },
            new Setting
            {
                Cli = "alphaclip", IsBool = true,
                Fields = new[] { "UniversalTarget.m_AlphaClip", "BuiltInTarget.m_AlphaClip", "SystemData.m_AlphaTest" },
            },
            new Setting { Cli = "castshadows", IsBool = true, Fields = new[] { "UniversalTarget.m_CastShadows" } },
            new Setting { Cli = "receiveshadows", IsBool = true, Fields = new[] { "UniversalTarget.m_ReceiveShadows" } },
            new Setting
            {
                Cli = "materialoverride", IsBool = true,
                Fields = new[] { "UniversalTarget.m_AllowMaterialOverride", "BuiltInTarget.m_AllowMaterialOverride" },
            },
            // HDRP also has SystemData.m_SupportLodCrossFade, but only as a migration source.
            new Setting
            {
                Cli = "lodcrossfade", IsBool = true,
                Fields = new[] { "UniversalTarget.m_SupportsLODCrossFade", "BuiltinData.m_SupportLodCrossFade" },
            },
            new Setting { Cli = "vfx", IsBool = true, Fields = new[] { "UniversalTarget.m_SupportVFX", "HDTarget.m_SupportVFX" } },
            new Setting
            {
                Cli = "normalspace", Values = new[] { "tangent", "object", "world" },
                Fields = new[] { "UniversalLitSubTarget.m_NormalDropOffSpace", "BuiltInLitSubTarget.m_NormalDropOffSpace", "LightingData.m_NormalDropOffSpace" },
            },
            new Setting
            {
                Cli = "customeditor",
                Fields = new[] { "UniversalTarget.m_CustomEditorGUI", "BuiltInTarget.m_CustomEditorGUI", "HDTarget.m_CustomEditorGUI" },
            },
        };

        static readonly HashSet<string> s_Bookkeeping = new HashSet<string>(StringComparer.Ordinal)
        {
            MultiJsonDocument.ObjectIdKey, MultiJsonDocument.TypeKey, MultiJsonDocument.VersionKey,
        };

        static string ShortType(MultiJsonEntry entry) => SlotTypes.ShortName(entry.TypeName) ?? entry.TypeName;

        /// <summary>
        /// Where a setting lives on this target. A named owner wins even when the field is absent,
        /// since graphs saved by older versions omit fields added since. Otherwise any owned
        /// object that already has the field, which is what lets a custom target use the keys.
        /// </summary>
        static (MultiJsonEntry owner, string field) Locate(SgTarget target, Setting setting)
        {
            foreach (var spec in setting.Fields)
            {
                var dot = spec.IndexOf('.');
                var ownerType = spec.Substring(0, dot);
                var owner = target.Owned.FirstOrDefault(e => ShortType(e) == ownerType);
                if (owner != null)
                    return (owner, spec.Substring(dot + 1));
            }
            foreach (var field in setting.Fields.Select(s => s.Substring(s.IndexOf('.') + 1)).Distinct())
            {
                var owner = target.Owned.FirstOrDefault(e => e.Node.ContainsKey(field));
                if (owner != null)
                    return (owner, field);
            }
            return (null, null);
        }

        static Setting Find(string key) =>
            s_Settings.FirstOrDefault(s => string.Equals(s.Cli, key, StringComparison.OrdinalIgnoreCase));

        public static bool Has(SgTarget target, string key)
        {
            if (string.Equals(key, "cull", StringComparison.OrdinalIgnoreCase))
                key = "renderface";
            var setting = Find(key);
            if (setting != null)
                return Locate(target, setting).owner != null;
            return RawOwners(target, key).Count > 0;
        }

        public static IEnumerable<string> Describe(SgTarget target)
        {
            foreach (var setting in s_Settings)
            {
                var (owner, field) = Locate(target, setting);
                if (owner == null || !owner.Node.TryGetPropertyValue(field, out var value) || value == null)
                    continue;
                var text = Format(setting, value);
                if (text == null)
                    continue;
                yield return setting.Cli + "=" + text;
            }
        }

        /// <summary>Every scalar field of every object the target owns, keyed the way 'target set' takes them.</summary>
        public static IEnumerable<(string owner, IReadOnlyList<string> fields)> DescribeRaw(SgTarget target)
        {
            foreach (var entry in target.Owned)
            {
                var fields = new List<string>();
                foreach (var pair in entry.Node)
                {
                    if (s_Bookkeeping.Contains(pair.Key) || pair.Value is JsonObject || pair.Value is JsonArray)
                        continue;
                    fields.Add(pair.Key + "=" + (pair.Value == null ? "null" : pair.Value.ToJsonString()));
                }
                yield return (ShortType(entry), fields);
            }
        }

        static string Format(Setting setting, JsonNode value)
        {
            if (setting.IsBool)
                return Json.Bool(value, false) ? "on" : "off";
            if (setting.Values == null)
            {
                var s = value.GetValue<string>();
                return string.IsNullOrEmpty(s) ? null : "\"" + s + "\"";
            }
            var code = Json.Int(value, 0);
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

            var setting = Find(key);
            if (setting == null)
                return ApplyRaw(target, key, value);

            var (owner, field) = Locate(target, setting);
            if (owner == null)
                throw new ShaderWaitressException(
                    $"the {target.Label} target has no '{setting.Cli}' setting. 'target show --raw' lists the fields it does have, " +
                    "and any of them can be set by name");
            var node = owner.Edit();

            if (setting.IsBool)
            {
                var on = value is "on" or "true" or "1" or "yes";
                node[field] = on;
                return $"{setting.Cli}={(on ? "on" : "off")}";
            }
            if (setting.Values == null)
            {
                node[field] = value;
                return $"{setting.Cli}=\"{value}\"";
            }

            var index = Array.FindIndex(setting.Values, v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
            if (index < 0 && setting.Aliases != null)
                index = Array.FindIndex(setting.Aliases, v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var raw))
                {
                    node[field] = raw;
                    return $"{setting.Cli}={raw}";
                }
                throw new ShaderWaitressException($"'{value}' is not valid for {setting.Cli}. Options: " + string.Join(", ", setting.Values));
            }
            var code = setting.Codes == null ? index : setting.Codes[index];
            node[field] = code;
            return $"{setting.Cli}={setting.Values[index]}";
        }

        /// <summary>
        /// The objects holding a raw field. "m_Field" searches everything the target owns;
        /// "SystemData.m_Field" names the owner, for a field two objects both carry.
        /// </summary>
        static List<(MultiJsonEntry owner, string field)> RawOwners(SgTarget target, string key)
        {
            var dot = key.IndexOf('.');
            var ownerType = dot > 0 ? key.Substring(0, dot) : null;
            var field = dot > 0 ? key.Substring(dot + 1) : key;
            return target.Owned
                .Where(e => (ownerType == null || string.Equals(ShortType(e), ownerType, StringComparison.OrdinalIgnoreCase)) &&
                            e.Node.ContainsKey(field) && !s_Bookkeeping.Contains(field))
                .Select(e => (e, field))
                .ToList();
        }

        static string ApplyRaw(SgTarget target, string key, string value)
        {
            var owners = RawOwners(target, key);
            if (owners.Count == 0)
                throw new ShaderWaitressException(
                    $"unknown target setting '{key}'. Known: {string.Join(", ", KnownKeys)}. " +
                    $"'target show --raw' lists every field the {target.Label} target and its sub-target and data objects have");
            if (owners.Count > 1)
                throw new ShaderWaitressException(
                    $"'{key}' is a field of more than one object on the {target.Label} target; name the one you mean: " +
                    string.Join(", ", owners.Select(o => ShortType(o.owner) + "." + o.field)));
            var (owner, field) = owners[0];
            var node = owner.Edit();
            if (node[field] is JsonObject || node[field] is JsonArray)
                throw new ShaderWaitressException($"{ShortType(owner)}.{field} is not a plain value and cannot be set from the command line");
            node[field] = ParseRaw(node[field], value);
            return $"{ShortType(owner)}.{field}={value}";
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

    /// <summary>
    /// How targets are named on the command line: the catalog's starter names, which are the
    /// type name without its Target suffix ("universal/lit", "hd/lit"), plus the pipeline
    /// nicknames people actually type.
    /// </summary>
    public static class TargetNames
    {
        static readonly Dictionary<string, string> s_Nicknames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["hdrp"] = "hd",
            ["urp"] = "universal",
            ["built-in"] = "builtin",
            ["birp"] = "builtin",
        };

        public static string Normalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;
            var parts = name.Trim().ToLowerInvariant().Replace('\\', '/').Split('/');
            if (s_Nicknames.TryGetValue(parts[0], out var canonical))
                parts[0] = canonical;
            return string.Join("/", parts);
        }

        public static bool Matches(SgTarget target, string normalized)
        {
            var type = target.TypeLabel.ToLowerInvariant();
            return normalized == type || normalized == target.Label.ToLowerInvariant();
        }
    }
}
