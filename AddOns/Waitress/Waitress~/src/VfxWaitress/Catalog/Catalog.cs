using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace VfxWaitress.Catalog
{
    public sealed class CatalogSlot
    {
        public string Name;
        public string Type;
        public string ValueType;
        public List<CatalogSlot> Children = new List<CatalogSlot>();
    }

    public sealed class CatalogSetting
    {
        public string Name;
        public string Type;
        public string Default;
        public string Shape;
        /// <summary>What the class initializes the field to, with no variant applied.</summary>
        public string ClassDefault;
        public List<string> Values = new List<string>();
    }

    public sealed class CatalogModel
    {
        public string Kind;
        public string Type;
        public string Name;
        public string Category;
        public string Script;
        public string Template;
        public string VariantOf;
        public bool Experimental;
        public List<string> Synonyms = new List<string>();
        public Dictionary<string, string> VariantSettings = new Dictionary<string, string>(StringComparer.Ordinal);
        public List<CatalogSetting> Settings = new List<CatalogSetting>();
        public List<CatalogSlot> Inputs = new List<CatalogSlot>();
        public List<CatalogSlot> Outputs = new List<CatalogSlot>();

        /// <summary>The last segment of the type name, which is how the graph editor names it.</summary>
        public string ShortType
        {
            get
            {
                var dot = Type.LastIndexOf('.');
                return dot >= 0 ? Type.Substring(dot + 1) : Type;
            }
        }
    }

    public sealed class NodeCatalog
    {
        public string UnityVersion = "";
        public string VfxGraphVersion = "";
        public readonly List<CatalogModel> Models = new List<CatalogModel>();
        public readonly List<string> Attributes = new List<string>();
        /// <summary>Slot type full name -> the assembly-qualified spelling a setting stores.</summary>
        public readonly Dictionary<string, string> SlotTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Resolves a type by full or short name to what a SerializableType setting needs.</summary>
        public string AssemblyQualified(string name)
        {
            if (name == null)
                return null;
            if (SlotTypes.TryGetValue(name, out var exact))
                return exact;
            foreach (var kvp in SlotTypes)
            {
                var dot = kvp.Key.LastIndexOf('.');
                if (string.Equals(dot >= 0 ? kvp.Key.Substring(dot + 1) : kvp.Key, name, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }
            return null;
        }

        readonly Dictionary<string, CatalogModel> m_ByScript = new Dictionary<string, CatalogModel>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, CatalogModel> m_ByType = new Dictionary<string, CatalogModel>(StringComparer.Ordinal);
        readonly Dictionary<string, List<CatalogModel>> m_ByShortType = new Dictionary<string, List<CatalogModel>>(StringComparer.OrdinalIgnoreCase);

        public static NodeCatalog Empty => new NodeCatalog();

        /// <summary>Where the catalog came from, or why there isn't one.</summary>
        public string Source = "none";

        public bool IsEmpty => Models.Count == 0;

        /// <summary>Set by the caller before the first use of <see cref="Default"/>.</summary>
        public static string OverridePath;

        static NodeCatalog s_Default;

        /// <summary>
        /// The catalog in the project's UserSettings/Waitress/, or an empty one. It describes one
        /// project's installed packages, so it is never built in.
        /// </summary>
        public static NodeCatalog Default
        {
            get
            {
                if (s_Default != null)
                    return s_Default;
                var path = Locate();
                if (path == null)
                    return s_Default = Empty;
                try
                {
                    var catalog = Load(File.ReadAllText(path));
                    catalog.Source = path;
                    return s_Default = catalog;
                }
                catch (Exception e)
                {
                    throw new VfxWaitressException($"could not read the catalog at {path}: {e.Message}");
                }
            }
        }

        static string Locate()
        {
            if (!string.IsNullOrEmpty(OverridePath))
            {
                if (!File.Exists(OverridePath))
                    throw new VfxWaitressException("no catalog at " + OverridePath);
                return OverridePath;
            }
            var fromEnv = Environment.GetEnvironmentVariable("VFXWAITRESS_CATALOG");
            if (!string.IsNullOrEmpty(fromEnv) && File.Exists(fromEnv))
                return fromEnv;

            var inProject = UnityProject.SettingsFile("catalog.json");
            return inProject != null && File.Exists(inProject) ? inProject : null;
        }

        public const string MissingMessage =
            "no node catalog found in this project's UserSettings/Waitress/catalog.json. The catalog describes the " +
            "packages installed in one project, so it's exported from that project's Editor: import the " +
            "\"VfxWaitress Editor Bridge\" sample from the Latios Framework Addons package, then run " +
            "`unity command vfxwaitress_export_catalog` or Tools > VfxWaitress > Export Catalog. " +
            "Point at an existing one with --catalog <path> or VFXWAITRESS_CATALOG.";

        /// <summary>For commands that cannot do anything useful without one.</summary>
        public void Require()
        {
            if (IsEmpty)
                throw new VfxWaitressException(MissingMessage);
        }

        public static NodeCatalog Load(string json)
        {
            var catalog = new NodeCatalog();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("unityVersion", out var uv))
                catalog.UnityVersion = uv.GetString();
            if (root.TryGetProperty("vfxGraphVersion", out var vv))
                catalog.VfxGraphVersion = vv.GetString();
            if (root.TryGetProperty("attributes", out var attrs))
            {
                foreach (var a in attrs.EnumerateArray())
                    catalog.Attributes.Add(a.ValueKind == JsonValueKind.String ? a.GetString() : a.GetProperty("name").GetString());
            }
            if (root.TryGetProperty("slotTypes", out var slotTypes))
            {
                foreach (var t in slotTypes.EnumerateArray())
                {
                    var full = Str(t, "type");
                    var aqn = Str(t, "assemblyQualified");
                    if (full != null && aqn != null)
                        catalog.SlotTypes[full] = aqn;
                }
            }
            if (root.TryGetProperty("models", out var models))
            {
                foreach (var m in models.EnumerateArray())
                    catalog.Add(ReadModel(m));
            }
            return catalog;
        }

        static CatalogModel ReadModel(JsonElement e)
        {
            var m = new CatalogModel
            {
                Kind = Str(e, "kind"),
                Type = Str(e, "type"),
                Name = Str(e, "name"),
                Category = Clean(Str(e, "category")),
                Script = Str(e, "script"),
                Template = Str(e, "template"),
                VariantOf = Str(e, "variantOf"),
            };
            if (e.TryGetProperty("synonyms", out var syn))
            {
                foreach (var s in syn.EnumerateArray())
                    m.Synonyms.Add(s.GetString());
            }
            if (e.TryGetProperty("variantSettings", out var vs))
            {
                foreach (var p in vs.EnumerateObject())
                    m.VariantSettings[p.Name] = p.Value.GetString();
            }
            if (e.TryGetProperty("settings", out var settings))
            {
                foreach (var s in settings.EnumerateArray())
                {
                    var setting = new CatalogSetting { Name = Str(s, "name"), Type = Str(s, "type"), Default = Str(s, "default"), Shape = Str(s, "shape"), ClassDefault = Str(s, "classDefault") };
                    if (s.TryGetProperty("values", out var values))
                    {
                        foreach (var v in values.EnumerateArray())
                            setting.Values.Add(v.GetString());
                    }
                    m.Settings.Add(setting);
                }
            }
            ReadSlots(e, "inputs", m.Inputs);
            ReadSlots(e, "outputs", m.Outputs);
            return m;
        }

        static void ReadSlots(JsonElement e, string key, List<CatalogSlot> into)
        {
            if (!e.TryGetProperty(key, out var arr))
                return;
            foreach (var s in arr.EnumerateArray())
                into.Add(ReadSlot(s));
        }

        static CatalogSlot ReadSlot(JsonElement e)
        {
            var slot = new CatalogSlot { Name = Str(e, "name"), Type = Str(e, "type"), ValueType = Str(e, "valueType") };
            if (e.TryGetProperty("children", out var children))
            {
                foreach (var c in children.EnumerateArray())
                    slot.Children.Add(ReadSlot(c));
            }
            return slot;
        }

        /// <summary>Category paths carry a "#<index>" sort prefix the node menu strips.</summary>
        static string Clean(string s) => s == null ? null : System.Text.RegularExpressions.Regex.Replace(s, "#[0-9]+", "");

        static string Str(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        void Add(CatalogModel m)
        {
            Models.Add(m);
            if (!string.IsNullOrEmpty(m.Script))
            {
                if (!m_ByScript.ContainsKey(m.Script))
                    m_ByScript[m.Script] = m;
                if (!m_VariantsByScript.TryGetValue(m.Script, out var variants))
                    m_VariantsByScript[m.Script] = variants = new List<CatalogModel>();
                variants.Add(m);
            }
            if (!string.IsNullOrEmpty(m.Type) && !m_ByType.ContainsKey(m.Type))
                m_ByType[m.Type] = m;
            if (!string.IsNullOrEmpty(m.Type))
            {
                if (!m_ByShortType.TryGetValue(m.ShortType, out var list))
                    m_ByShortType[m.ShortType] = list = new List<CatalogModel>();
                list.Add(m);
            }
        }

        readonly Dictionary<string, List<CatalogModel>> m_VariantsByScript = new Dictionary<string, List<CatalogModel>>(StringComparer.OrdinalIgnoreCase);

        public CatalogModel ByScript(string guid) =>
            guid != null && m_ByScript.TryGetValue(guid, out var m) ? m : null;

        /// <summary>
        /// Many models share a script: every SetAttribute variant is the same class with different
        /// settings. Naming a node by the first of them is wrong, so pick the variant whose
        /// settings the node actually carries.
        /// </summary>
        public CatalogModel ResolveVariant(string guid, Func<string, string> settingValue)
        {
            if (guid == null || !m_VariantsByScript.TryGetValue(guid, out var candidates))
                return null;
            CatalogModel best = null;
            var bestScore = -1;
            foreach (var candidate in candidates)
            {
                var score = 0;
                var contradicted = false;
                foreach (var kvp in candidate.VariantSettings)
                {
                    var actual = Interpret(candidates[0], kvp.Key, settingValue(kvp.Key));
                    if (actual == null)
                        continue;
                    if (string.Equals(actual, kvp.Value, StringComparison.OrdinalIgnoreCase))
                        score++;
                    else
                        contradicted = true;
                }
                if (contradicted)
                    continue;
                // With nothing to go on, the base entry beats an arbitrary variant.
                if (candidate.VariantOf == null)
                    score++;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }
            return best ?? candidates[0];
        }

        /// <summary>Serialized settings are ordinals and 0/1; variant settings are names.</summary>
        static string Interpret(CatalogModel shape, string settingName, string serialized)
        {
            if (serialized == null)
                return null;
            var setting = shape.Settings.FirstOrDefault(s => s.Name == settingName);
            if (setting == null)
                return serialized;
            if (setting.Values.Count > 0 && int.TryParse(serialized, out var ordinal) && ordinal >= 0 && ordinal < setting.Values.Count)
                return setting.Values[ordinal];
            if (setting.Type == "System.Boolean")
                return serialized == "1" ? "True" : serialized == "0" ? "False" : serialized;
            return serialized;
        }

        public CatalogModel ByType(string type)
        {
            if (type == null)
                return null;
            if (m_ByType.TryGetValue(type, out var m))
                return m;
            if (m_ByShortType.TryGetValue(type, out var list))
                return list[0];
            return null;
        }

        public IEnumerable<CatalogModel> Search(string pattern)
        {
            var match = Glob.CompileLoose(pattern ?? "*");
            foreach (var m in Models)
            {
                if (match(m.Name) || match(m.ShortType) || match(m.Type) || m.Synonyms.Any(s => match(s)))
                    yield return m;
            }
        }
    }
}
