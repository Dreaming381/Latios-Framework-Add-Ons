using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using ShaderWaitress.Model;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Catalog
{
    public sealed class CatalogPort
    {
        public int Id;
        public string Name;
        public string ShaderOutputName;
        public bool IsInput;
        public string SlotTypeName;
        /// <summary>The value the node type constructs this slot with.</summary>
        public System.Text.Json.Nodes.JsonNode DefaultValue;
        public SlotKind Kind => SlotTypes.KindOf(SlotTypeName);
    }

    public sealed class CatalogSetting
    {
        public string Field;
        public string Name;
        public string Kind;
        public string[] Values;
        public long[] Codes;
    }

    public sealed class StarterGraph
    {
        public string Name;
        public string Kind = "shadergraph";
        public string TargetType;
        public string SubTargetType;
        public string Text;
    }

    public sealed class CatalogEntry
    {
        public string TypeName;
        public string Title;
        public string[] Path = Array.Empty<string>();
        public string[] Synonyms = Array.Empty<string>();
        public bool HasPreview;
        public string Descriptor;               // block nodes only
        /// <summary>HLSL declarations a property type accepts; property entries only.</summary>
        public string[] Declarations = Array.Empty<string>();
        public string Stage;                    // block nodes only
        public List<CatalogPort> Ports = new List<CatalogPort>();

        /// <summary>Dropdowns and toggles drawn on the node body rather than on a port.</summary>
        public List<CatalogSetting> Settings = new List<CatalogSetting>();

        /// <summary>
        /// How many control rows the node draws, which can be fewer than its settings. Sample
        /// Texture 2D has four settings but draws two controls. -1 when the catalog does not
        /// record it.
        /// </summary>
        public int ControlRows = -1;

        /// <summary>
        /// Exactly what Unity serializes for a default-constructed instance: the object
        /// itself first, then everything it owns. Cloned and given fresh ids when a node is added.
        /// </summary>
        public List<string> Template = new List<string>();

        public string ShortTypeName
        {
            get
            {
                var s = SlotTypes.ShortName(TypeName) ?? TypeName;
                return s.EndsWith("Node", StringComparison.Ordinal) && s.Length > 4 ? s.Substring(0, s.Length - 4) : s;
            }
        }

        public string PathLabel => Path.Length == 0 ? string.Empty : string.Join("/", Path);
    }

    /// <summary>
    /// Ports and creation templates for every Shader Graph node type, exported from the
    /// Editor by the ShaderWaitress Catalog Exporter package sample. The tool works without
    /// it, but can't create nodes or check port names.
    /// </summary>
    public sealed class NodeCatalog
    {
        public string GeneratedBy = "(none)";
        public string UnityVersion = "";
        public string ShaderGraphVersion = "";

        readonly Dictionary<string, CatalogEntry> m_ByType = new Dictionary<string, CatalogEntry>(StringComparer.OrdinalIgnoreCase);
        readonly List<CatalogEntry> m_Nodes = new List<CatalogEntry>();
        readonly List<CatalogEntry> m_Blocks = new List<CatalogEntry>();
        readonly List<CatalogEntry> m_Properties = new List<CatalogEntry>();
        readonly List<CatalogEntry> m_Targets = new List<CatalogEntry>();
        readonly List<CatalogEntry> m_KeywordTemplates = new List<CatalogEntry>();
        readonly List<StarterGraph> m_Starters = new List<StarterGraph>();

        public IReadOnlyList<CatalogEntry> Nodes => m_Nodes;
        public IReadOnlyList<CatalogEntry> Blocks => m_Blocks;
        public IReadOnlyList<CatalogEntry> Properties => m_Properties;
        public IReadOnlyList<CatalogEntry> Targets => m_Targets;
        public IReadOnlyList<CatalogEntry> KeywordTemplates => m_KeywordTemplates;
        public IReadOnlyList<StarterGraph> Starters => m_Starters;
        public bool IsEmpty => m_Nodes.Count == 0;

        static NodeCatalog s_Shared;
        public static NodeCatalog Shared => s_Shared ??= LoadShared();

        /// <summary>Set by --catalog before the first use of <see cref="Shared"/>.</summary>
        public static string OverridePath;

        static NodeCatalog LoadShared()
        {
            if (!string.IsNullOrEmpty(OverridePath) && !File.Exists(OverridePath))
                throw new ShaderWaitressException("no catalog at " + OverridePath);
            foreach (var candidate in ProbePaths())
            {
                if (candidate != null && File.Exists(candidate))
                {
                    try
                    {
                        return Parse(File.ReadAllText(candidate), candidate);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"warning: could not read catalog '{candidate}': {e.Message}");
                    }
                }
            }

            Console.Error.WriteLine(
                "warning: no node catalog in this project's UserSettings/Waitress/nodes.json. Import the " +
                "\"ShaderWaitress Editor Bridge\" sample from the Latios Framework Addons package and run " +
                "`unity command shaderwaitress_export_catalog` or Tools > ShaderWaitress > Export Node Catalog, " +
                "or point at an existing catalog with --catalog or SHADERWAITRESS_CATALOG.");
            return new NodeCatalog();
        }

        static IEnumerable<string> ProbePaths()
        {
            yield return OverridePath;
            yield return Environment.GetEnvironmentVariable("SHADERWAITRESS_CATALOG");
            yield return UnityProject.SettingsFile("nodes.json");
        }

        public static NodeCatalog Load(string path) => Parse(File.ReadAllText(path), path);

        public static NodeCatalog Parse(string json, string origin)
        {
            var catalog = new NodeCatalog();
            var root = JsonNode.Parse(json) as JsonObject
                       ?? throw new ShaderWaitressException($"catalog '{origin}' is not a JSON object");
            catalog.GeneratedBy = (string)root["generatedBy"] ?? origin;
            catalog.UnityVersion = (string)root["unityVersion"] ?? "";
            catalog.ShaderGraphVersion = (string)root["shaderGraphVersion"] ?? "";

            ReadSection(root, "nodes", catalog.m_Nodes, catalog);
            ReadSection(root, "blocks", catalog.m_Blocks, catalog);
            ReadSection(root, "properties", catalog.m_Properties, catalog);
            ReadSection(root, "targets", catalog.m_Targets, catalog);
            ReadSection(root, "keywords", catalog.m_KeywordTemplates, catalog);

            if (root["starters"] is JsonArray starters)
            {
                foreach (var item in starters)
                {
                    if (item is not JsonObject o)
                        continue;
                    var name = (string)o["name"];
                    var text = (string)o["text"];
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(text))
                        continue;
                    // Internal targets that exist only to drive previews are not real choices.
                    if (name.IndexOf("unknown", StringComparison.OrdinalIgnoreCase) >= 0 || name == "preview")
                        continue;
                    catalog.m_Starters.Add(new StarterGraph
                    {
                        Name = name,
                        Kind = (string)o["kind"] ?? "shadergraph",
                        TargetType = (string)o["target"],
                        SubTargetType = (string)o["subTarget"],
                        Text = text,
                    });
                }
            }
            return catalog;
        }

        public StarterGraph FindStarter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            name = name.Trim().ToLowerInvariant().Replace('\\', '/');
            var exact = m_Starters.FirstOrDefault(s => s.Name == name);
            if (exact != null)
                return exact;
            var matches = m_Starters.Where(s => s.Name.EndsWith("/" + name, StringComparison.Ordinal) ||
                                                s.Name.StartsWith(name + "/", StringComparison.Ordinal)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        static void ReadSection(JsonObject root, string field, List<CatalogEntry> into, NodeCatalog catalog)
        {
            if (root[field] is not JsonArray arr)
                return;
            foreach (var item in arr)
            {
                if (item is not JsonObject o)
                    continue;
                var entry = new CatalogEntry
                {
                    TypeName = (string)o["type"],
                    Title = (string)o["title"],
                    Descriptor = (string)o["descriptor"],
                    Stage = (string)o["stage"],
                    HasPreview = ShaderWaitress.Serialization.Json.Bool(o["preview"], false),
                    ControlRows = o["controls"] != null ? ShaderWaitress.Serialization.Json.Int(o["controls"], 0) : -1,
                };
                entry.Path = ReadStrings(o["path"]);
                entry.Synonyms = ReadStrings(o["synonyms"]);
                entry.Declarations = ReadStrings(o["declarations"]);
                if (o["ports"] is JsonArray ports)
                {
                    foreach (var p in ports)
                    {
                        entry.Ports.Add(new CatalogPort
                        {
                            Id = ShaderWaitress.Serialization.Json.Int(p["id"], 0),
                            Name = (string)p["name"],
                            ShaderOutputName = (string)p["outName"] ?? (string)p["name"],
                            IsInput = ShaderWaitress.Serialization.Json.Bool(p["input"], true),
                            SlotTypeName = (string)p["slot"],
                        });
                    }
                }
                if (o["settings"] is JsonArray settings)
                {
                    foreach (var s in settings)
                    {
                        if (s is not JsonObject so)
                            continue;
                        entry.Settings.Add(new CatalogSetting
                        {
                            Field = (string)so["field"],
                            Name = (string)so["name"] ?? (string)so["field"],
                            Kind = (string)so["kind"] ?? "string",
                            Values = ReadStrings(so["values"]),
                            Codes = ReadStrings(so["codes"]).Select(c => long.TryParse(c, out var v) ? v : 0L).ToArray(),
                        });
                    }
                }
                if (o["template"] is JsonArray tpl)
                {
                    foreach (var t in tpl)
                        entry.Template.Add((string)t);
                }
                if (entry.TypeName == null)
                    continue;
                if (entry.Ports.Count == 0)
                    DerivePorts(entry);
                into.Add(entry);
                var key = entry.Descriptor ?? entry.TypeName;
                catalog.m_ByType[key] = entry;
                if (entry.Descriptor == null)
                    catalog.m_ByType[entry.TypeName] = entry;
            }
        }

        /// <summary>
        /// The template already contains every slot object the Editor would write, so port
        /// metadata is read back out of it rather than duplicated in the catalog file.
        /// </summary>
        static void DerivePorts(CatalogEntry entry)
        {
            if (entry.Template.Count == 0)
                return;
            var order = new List<string>();
            if (JsonNode.Parse(entry.Template[0]) is JsonObject node && node["m_Slots"] is JsonArray slots)
            {
                foreach (var slotRef in slots)
                    order.Add(UnityJsonWriter.RefId(slotRef));
            }

            var byId = new Dictionary<string, JsonObject>(StringComparer.Ordinal);
            for (var i = 1; i < entry.Template.Count; i++)
            {
                if (JsonNode.Parse(entry.Template[i]) is not JsonObject obj)
                    continue;
                var type = (string)obj["m_Type"];
                if (type == null || !type.EndsWith("MaterialSlot", StringComparison.Ordinal))
                    continue;
                byId[(string)obj["m_ObjectId"] ?? string.Empty] = obj;
            }

            foreach (var id in order)
            {
                if (!byId.TryGetValue(id, out var slot))
                    continue;
                var slotType = (string)slot["m_Type"];
                var valueField = ShaderWaitress.Model.SlotTypes.ValueFieldOf(slotType);
                entry.Ports.Add(new CatalogPort
                {
                    Id = ShaderWaitress.Serialization.Json.Int(slot["m_Id"], 0),
                    Name = (string)slot["m_DisplayName"] ?? (string)slot["m_ShaderOutputName"] ?? "?",
                    ShaderOutputName = (string)slot["m_ShaderOutputName"],
                    IsInput = ShaderWaitress.Serialization.Json.Int(slot["m_SlotType"], 0) == 0,
                    SlotTypeName = slotType,
                    DefaultValue = valueField == null ? null : slot[valueField]?.DeepClone(),
                });
            }
        }

        static string[] ReadStrings(JsonNode node)
        {
            if (node is not JsonArray arr)
                return Array.Empty<string>();
            return arr.Select(x => (string)x).Where(x => x != null).ToArray();
        }

        public CatalogEntry Find(string typeName) =>
            typeName != null && m_ByType.TryGetValue(typeName, out var e) ? e : null;

        /// <summary>
        /// Resolves what a caller typed. Accepts the full type name, the class name, the
        /// display title, a "Math/Basic/Multiply" path, or a synonym.
        /// </summary>
        public CatalogEntry Resolve(string text, out IReadOnlyList<CatalogEntry> ambiguous)
        {
            ambiguous = Array.Empty<CatalogEntry>();
            if (string.IsNullOrWhiteSpace(text))
                throw new ShaderWaitressException("missing node type");
            text = text.Trim();

            if (m_ByType.TryGetValue(text, out var direct))
                return direct;

            var all = m_Nodes.Concat(m_Blocks).ToList();

            var exactType = all.Where(n => string.Equals(SlotTypes.ShortName(n.TypeName), text, StringComparison.OrdinalIgnoreCase)).ToList();
            if (exactType.Count == 1)
                return exactType[0];

            var withSuffix = all.Where(n => string.Equals(SlotTypes.ShortName(n.TypeName), text + "Node", StringComparison.OrdinalIgnoreCase)).ToList();
            if (withSuffix.Count == 1)
                return withSuffix[0];

            var byDescriptor = m_Blocks.Where(b => b.Descriptor != null &&
                (string.Equals(b.Descriptor, text, StringComparison.OrdinalIgnoreCase) ||
                 b.Descriptor.EndsWith("." + text, StringComparison.OrdinalIgnoreCase))).ToList();
            if (byDescriptor.Count == 1)
                return byDescriptor[0];

            var byTitle = all.Where(n => string.Equals(n.Title, text, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byTitle.Count == 1)
                return byTitle[0];
            if (byTitle.Count > 1)
            {
                ambiguous = byTitle;
                return null;
            }

            var byPath = all.Where(n => string.Equals(n.PathLabel + "/" + n.Title, text, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byPath.Count == 1)
                return byPath[0];

            var bySynonym = all.Where(n => n.Synonyms.Any(s => string.Equals(s, text, StringComparison.OrdinalIgnoreCase))).ToList();
            if (bySynonym.Count == 1)
                return bySynonym[0];
            if (bySynonym.Count > 1)
            {
                ambiguous = bySynonym;
                return null;
            }

            var squashed = text.Replace(" ", string.Empty);
            var bySquash = all.Where(n => string.Equals(n.Title?.Replace(" ", string.Empty), squashed, StringComparison.OrdinalIgnoreCase)).ToList();
            if (bySquash.Count == 1)
                return bySquash[0];
            if (bySquash.Count > 1)
            {
                ambiguous = bySquash;
                return null;
            }

            return null;
        }

        public CatalogEntry Require(string text)
        {
            var entry = Resolve(text, out var ambiguous);
            if (entry != null)
                return entry;
            if (ambiguous.Count > 0)
                throw new ShaderWaitressException($"'{text}' is ambiguous: " + string.Join(", ", ambiguous.Select(a => a.PathLabel + "/" + a.Title)));
            if (IsEmpty)
                throw new ShaderWaitressException($"no node catalog is loaded, so '{text}' cannot be resolved. See 'shaderwaitress help catalog' for how to export one.");
            var near = Suggest(text);
            var hint = near.Count == 0 ? string.Empty : " Did you mean: " + string.Join(", ", near) + "?";
            throw new ShaderWaitressException($"unknown node type '{text}'.{hint}");
        }

        public IReadOnlyList<string> Suggest(string text)
        {
            var lower = text.ToLowerInvariant();
            return m_Nodes.Concat(m_Blocks)
                .Where(n => (n.Title ?? string.Empty).ToLowerInvariant().Contains(lower) ||
                            (SlotTypes.ShortName(n.TypeName) ?? string.Empty).ToLowerInvariant().Contains(lower))
                .Select(n => n.Title ?? SlotTypes.ShortName(n.TypeName))
                .Distinct()
                .Take(8)
                .ToList();
        }

        public IEnumerable<CatalogEntry> Search(string pattern)
        {
            var all = m_Nodes.Concat(m_Blocks);
            if (string.IsNullOrEmpty(pattern))
                return all;
            var glob = Glob.Compile(pattern);
            return all.Where(n => glob(n.Title ?? string.Empty) ||
                                  glob(SlotTypes.ShortName(n.TypeName) ?? string.Empty) ||
                                  glob(n.PathLabel) ||
                                  n.Synonyms.Any(s => glob(s)));
        }
    }
}
