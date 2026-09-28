using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VfxWaitress.Model
{
    /// <summary>
    /// The dense textual form of a graph: one line per node, inputs only, literals inline, no
    /// coordinates and no slot objects. A .vfx that costs 100k tokens to read raw fits in a
    /// couple of hundred lines here.
    /// </summary>
    public static class DenseText
    {
        public static string Render(VfxAsset asset, bool showPositions = false)
        {
            t_Guids = GuidIndex.For(asset.Path);
            t_Catalog = asset.Catalog;
            var sb = new StringBuilder();
            var contexts = asset.TopLevel.Where(n => n.Kind == "context").ToList();
            var parameters = asset.TopLevel.Where(IsParam).ToList();
            var operators = asset.TopLevel.Where(n => !IsParam(n) && n.Kind != "context").ToList();
            var blocks = contexts.Sum(c => c.Blocks.Count);

            sb.Append("graph \"").Append(asset.Name).Append("\" kind=").Append(asset.Kind);
            sb.Append(" contexts=").Append(contexts.Count);
            sb.Append(" blocks=").Append(blocks);
            sb.Append(" operators=").Append(operators.Count);
            sb.Append(" params=").Append(parameters.Count);
            sb.Append(" edges=").Append(CountEdges(asset));
            sb.AppendLine();

            RenderResource(sb, asset);
            RenderCustomAttributes(sb, asset);
            sb.AppendLine();

            if (parameters.Count > 0)
            {
                foreach (var p in parameters)
                    RenderParameter(sb, asset, p);
                sb.AppendLine();
            }

            if (operators.Count > 0)
            {
                foreach (var n in operators.OrderBy(o => o.X).ThenBy(o => o.Y))
                    RenderNode(sb, asset, n, "", showPositions);
                sb.AppendLine();
            }

            // Contexts are grouped by the VFXData they share, because capacity, bounds mode and
            // space live there and are invisible from any context.
            var ordered = contexts.OrderBy(o => o.X).ThenBy(o => o.Y).ToList();
            var emitted = new HashSet<long>();
            foreach (var c in ordered)
            {
                if (!emitted.Add(c.FileId))
                    continue;
                var group = new List<VfxNode> { c };
                if (c.Data != null)
                {
                    RenderData(sb, c.Data);
                    foreach (var owner in ordered.Where(o => o.Data == c.Data && emitted.Add(o.FileId)))
                        group.Add(owner);
                }
                var indent = c.Data != null ? "  " : "";
                foreach (var member in group)
                {
                    RenderNode(sb, asset, member, indent, showPositions);
                    foreach (var b in member.Blocks)
                        RenderNode(sb, asset, b, indent + "  ", showPositions);
                }
            }

            RenderTrailer(sb, asset, contexts, operators);
            return sb.ToString();
        }

        static bool IsParam(VfxNode n) => n.Kind == "parameter" || n.ExposedName != null;

        static void RenderResource(StringBuilder sb, VfxAsset asset)
        {
            var infos = asset.ResourceDoc?.Body.Map("m_Infos");
            if (infos == null)
                return;
            sb.Append("resource");
            Append(sb, "initialEvent", infos.Scalar("m_InitialEventName"));
            Append(sb, "updateMode", infos.Scalar("m_UpdateMode"));
            Append(sb, "culling", infos.Scalar("m_CullingFlags"));
            Append(sb, "instancing", infos.Scalar("m_InstancingCapacity"));
            var prewarm = infos.Scalar("m_PreWarmStepCount");
            if (prewarm != null && prewarm != "0")
                Append(sb, "prewarmSteps", prewarm);
            sb.AppendLine();
        }

        /// <summary>
        /// Attributes the graph declares itself. Without these, a custom attribute looks the same
        /// as a built-in one.
        /// </summary>
        static void RenderCustomAttributes(StringBuilder sb, VfxAsset asset)
        {
            foreach (var line in CustomAttributeLines(asset))
                sb.AppendLine(line);
        }

        public static IEnumerable<string> CustomAttributeLines(VfxAsset asset)
        {
            var seq = asset.GraphDoc?.Body.Seq("m_CustomAttributes");
            if (seq == null || seq.Items.Count == 0)
                yield break;
            // Each entry is a reference to its own descriptor document.
            foreach (var id in seq.FileIds())
            {
                if (!asset.ById.TryGetValue(id, out var doc))
                    continue;
                var name = doc.Body.Scalar("m_AttributeName");
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                var line = new StringBuilder("attr ").Append(name).Append(' ').Append(AttributeType(doc.Body.Scalar("m_Type")));
                var description = doc.Body.Scalar("m_Description");
                if (!string.IsNullOrWhiteSpace(description))
                    line.Append(" \"").Append(description).Append('"');
                yield return line.ToString();
            }
        }

        /// <summary>CustomAttributeUtility.Signature, in declaration order.</summary>
        static readonly string[] k_AttributeTypes = { "float", "Vector2", "Vector3", "Vector4", "bool", "uint", "int" };

        static string AttributeType(string serialized) =>
            int.TryParse(serialized, out var i) && i >= 0 && i < k_AttributeTypes.Length ? k_AttributeTypes[i] : serialized ?? "?";

        static void Append(StringBuilder sb, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            sb.Append(' ').Append(key).Append('=').Append(value);
        }

        static void RenderParameter(StringBuilder sb, VfxAsset asset, VfxNode p)
        {
            var carrier = (p.IsOutput ? p.Inputs : p.Outputs).FirstOrDefault();
            sb.Append("param ").Append(p.ShortId).Append(p.IsOutput ? " out " : " in  ");
            sb.Append(carrier?.TypeName ?? "?");
            sb.Append(" \"").Append(p.ExposedName ?? "").Append('"');
            // An output parameter's stored value is whatever was last computed; it says nothing.
            if (carrier?.Value != null && !p.IsOutput)
                sb.Append(" =").Append(CompactValue(carrier.Value));
            if (p.Exposed && !p.IsOutput)
                sb.Append(" exposed");
            foreach (var slot in p.Inputs)
                RenderSlotInputs(sb, asset, slot);
            var placements = p.ParameterNodes.Count;
            if (placements > 1)
                sb.Append("  placements=").Append(placements);
            sb.AppendLine();
        }

        static void RenderData(StringBuilder sb, VfxData data)
        {
            sb.Append("system ").Append(data.ShortId);
            if (!string.IsNullOrWhiteSpace(data.Title))
                sb.Append(" \"").Append(data.Title).Append('"');
            foreach (var setting in SettingsOf(data.Model, data.Doc))
                sb.Append("  ").Append(setting);
            sb.AppendLine();
        }

        static void RenderNode(StringBuilder sb, VfxAsset asset, VfxNode node, string indent, bool showPositions)
        {
            var tag = node.Kind switch
            {
                "context" => "ctx",
                "block" => "blk",
                _ => "node",
            };
            sb.Append(indent).Append(tag).Append(' ').Append(node.ShortId).Append(' ').Append(node.DisplayType);
            if (node.Label != null)
                sb.Append(" \"").Append(node.Label).Append('"');

            foreach (var setting in InterestingSettings(node))
                sb.Append("  ").Append(setting);

            foreach (var slot in node.Inputs)
                RenderSlotInputs(sb, asset, slot);

            foreach (var flow in node.FlowOut)
            {
                var target = asset.Nodes.TryGetValue(flow.toContext, out var t) ? t.ShortId : flow.toContext.ToString();
                sb.Append("  flow>").Append(target);
                if (flow.from != 0 || flow.toIndex != 0)
                    sb.Append('[').Append(flow.from).Append("->").Append(flow.toIndex).Append(']');
            }

            if (showPositions)
                sb.Append("  @(").Append(Num(node.X)).Append(',').Append(Num(node.Y)).Append(')');

            sb.AppendLine();
        }

        /// <summary>
        /// Prints a slot as one field. A composite slot prints as a whole when it is wired or
        /// valued as a whole, and descends into its children only where they differ.
        /// </summary>
        static void RenderSlotInputs(StringBuilder sb, VfxAsset asset, VfxSlot slot)
        {
            foreach (var s in slot.SelfAndDescendants())
            {
                if (s.LinkedSlotIds.Count > 0)
                {
                    foreach (var sourceId in s.LinkedSlotIds)
                        sb.Append("  ").Append(Compact(s.Path)).Append("<-").Append(SourceRef(asset, sourceId));
                    continue;
                }
                // Values live on the master slot only, so children of a valued slot say nothing,
                // and a master whose children are wired keeps a stale value that says nothing either.
                if (s.Parent == null && s.Value != null && !s.SelfAndDescendants().Any(d => d.LinkedSlotIds.Count > 0))
                    sb.Append("  ").Append(Compact(s.Path)).Append('=').Append(CompactValue(s.Value));
            }
        }

        static string SourceRef(VfxAsset asset, long sourceSlotId)
        {
            var source = asset.SlotById(sourceSlotId);
            if (source == null)
            {
                // Parameters wire through their placement records rather than a slot the node owns.
                foreach (var p in asset.TopLevel)
                {
                    foreach (var pn in p.ParameterNodes)
                    {
                        if (pn.Links.Any(l => l.outputSlot == sourceSlotId))
                            return p.ShortId;
                    }
                }
                return "?" + sourceSlotId;
            }
            var owner = source.Owner;
            var name = Compact(source.Path);
            if (owner != null && IsParam(owner))
            {
                // A parameter's carrier slot is named "o"; the reader wants the field under it.
                if (source.Parent == null)
                    return owner.ShortId;
                var root = source;
                while (root.Parent != null)
                    root = root.Parent;
                var relative = name;
                if (root.Name.Length > 0 && relative.StartsWith(root.Name + ".", StringComparison.Ordinal))
                    relative = relative.Substring(root.Name.Length + 1);
                return owner.ShortId + "." + relative;
            }
            var only = owner != null && owner.Outputs.Count == 1 && owner.Outputs[0].Children.Count == 0;
            return only ? owner.ShortId : owner?.ShortId + "." + name;
        }

        static IEnumerable<string> InterestingSettings(VfxNode node) => SettingsOf(node.Model, node.Doc);

        static IEnumerable<string> SettingsOf(Catalog.CatalogModel model, Serialization.YamlDocument doc)
        {
            if (model == null)
                yield break;
            foreach (var setting in model.Settings)
            {
                var raw = VfxAsset.SettingDisplay(doc, setting.Name);
                if (raw == null)
                    continue;
                var text = raw.Trim();
                if (text.Length == 0)
                    continue;
                // Enums serialize as an ordinal; the catalog reports the name.
                if (setting.Values.Count > 0 && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ordinal) &&
                    ordinal >= 0 && ordinal < setting.Values.Count)
                    text = setting.Values[ordinal];
                // Skip settings equal to the class default, which is the value before any variant
                // is applied. A catalog entry's own default is just its variant's value, so
                // comparing against it would hide the setting that names the node, like
                // attribute=position. When the class leaves a setting unset and a variant is keyed
                // on it, there's no baseline and it always prints.
                var baseline = setting.ClassDefault;
                if (baseline == null && Discriminates(model, setting.Name))
                    baseline = null;
                else
                    baseline ??= setting.Default;
                if (baseline != null && SameSetting(text, baseline))
                    continue;
                if (text.Contains("fileID"))
                {
                    var label = ReferenceLabel(text);
                    // An unassigned asset reference is the absence of a setting, not a setting.
                    if (label == "none")
                        continue;
                    yield return SettingLabel(setting.Name) + "=" + label;
                    continue;
                }
                yield return SettingLabel(setting.Name) + "=" + Compact(text);
            }
        }

        /// <summary>Undoes the double-quoted YAML scalar Unity writes for multi-line strings.</summary>
        static string Unquote(string s)
        {
            if (s.Length < 2 || s[0] != '"' || s[s.Length - 1] != '"')
                return s;
            return s.Substring(1, s.Length - 2)
                .Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        [ThreadStatic] static Catalog.NodeCatalog t_Catalog;

        static readonly Dictionary<string, HashSet<string>> s_Discriminators = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        /// <summary>True when some variant of this type is defined by this setting's value.</summary>
        static bool Discriminates(Catalog.CatalogModel model, string settingName)
        {
            if (t_Catalog == null)
                return false;
            if (!s_Discriminators.TryGetValue(model.Type, out var keys))
            {
                keys = new HashSet<string>(
                    t_Catalog.Models.Where(m => m.Type == model.Type).SelectMany(m => m.VariantSettings.Keys),
                    StringComparer.OrdinalIgnoreCase);
                s_Discriminators[model.Type] = keys;
            }
            return keys.Contains(settingName);
        }

        static string SettingLabel(string name) =>
            name.StartsWith("m_", StringComparison.Ordinal) ? name.Substring(2) : name;

        static bool SameSetting(string a, string b)
        {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(Unquote(a), b, StringComparison.Ordinal))
                return true;
            // Booleans serialize as 0/1 but the catalog reports true/false.
            if (a == "0" && string.Equals(b, "false", StringComparison.OrdinalIgnoreCase))
                return true;
            if (a == "1" && string.Equals(b, "true", StringComparison.OrdinalIgnoreCase))
                return true;
            // An unassigned object reference renders as "none" and reports an empty default.
            if (a == "none" && b.Length == 0)
                return true;
            return false;
        }

        [ThreadStatic] static GuidIndex t_Guids;

        static string ReferenceLabel(string flow)
        {
            var guid = Serialization.UnityYaml.GuidIn(flow);
            if (guid != null)
            {
                var name = t_Guids?.NameOf(guid);
                return name != null ? "\"" + name + "\"" : "asset:" + guid.Substring(0, 8);
            }
            var id = Serialization.UnityYaml.FileIdIn(flow);
            return id.GetValueOrDefault() == 0 ? "none" : flow;
        }

        static int CountEdges(VfxAsset asset)
        {
            var n = 0;
            foreach (var node in asset.AllNodes())
            {
                foreach (var slot in node.AllInputSlots)
                    n += slot.LinkedSlotIds.Count;
            }
            return n;
        }

        static void RenderTrailer(StringBuilder sb, VfxAsset asset, List<VfxNode> contexts, List<VfxNode> operators)
        {
            var lines = new List<string>();

            var orphans = operators.Where(o => o.AllInputSlots.All(s => s.LinkedSlotIds.Count == 0) &&
                                               !IsConsumed(asset, o)).Select(o => o.ShortId).ToList();
            if (orphans.Count > 0)
                lines.Add("# unconnected operators: " + string.Join(" ", orphans));

            var roots = contexts.Where(c => !asset.AllNodes().Any(n => n.FlowOut.Any(f => f.toContext == c.FileId))).Select(c => c.ShortId).ToList();
            if (roots.Count > 0)
                lines.Add("# flow roots: " + string.Join(" ", roots));

            if (asset.Catalog.IsEmpty)
                lines.Add("# warning: " + Catalog.NodeCatalog.MissingMessage);
            foreach (var w in asset.Warnings.Distinct())
                lines.Add("# warning: " + w);

            if (lines.Count == 0)
                return;
            sb.AppendLine();
            foreach (var line in lines)
                sb.AppendLine(line);
        }

        static bool IsConsumed(VfxAsset asset, VfxNode node)
        {
            var outputs = new HashSet<long>(node.AllOutputSlots.Select(s => s.FileId));
            foreach (var other in asset.AllNodes())
            {
                foreach (var slot in other.AllInputSlots)
                {
                    if (slot.LinkedSlotIds.Any(outputs.Contains))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Port names print without spaces so every field is one token.</summary>
        public static string Compact(string s) => s?.Replace(" ", "");

        static string CompactValue(string value)
        {
            var v = value.Trim();
            if (v.Length >= 2 && v[0] == '\'' && v[v.Length - 1] == '\'')
                v = v.Substring(1, v.Length - 2).Replace("''", "'");
            if (v.Contains("fileID"))
                return ReferenceLabel(v);
            // Curves and gradients are key lists no one wants inline; say how many keys.
            if (v.Contains("\"frames\""))
                return "curve(" + CountOf(v, "\"time\"") + ")";
            if (v.Contains("\"colorKeys\""))
                return "gradient(" + CountOf(v, "\"color\"") + ")";
            // Other JSON blobs: drop keys and quotes so a vector reads as (0,1.5,0).
            if (v.StartsWith("{", StringComparison.Ordinal))
            {
                // Field names can contain digits (m00, m01, ...), so drop the keys first.
                var valuesOnly = System.Text.RegularExpressions.Regex.Replace(v, "\"[^\"]*\"\\s*:", "");
                var numbers = System.Text.RegularExpressions.Regex.Matches(valuesOnly, @"-?\d+(?:\.\d+)?(?:[eE][-+]?\d+)?")
                    .Select(m => Trim(m.Value)).ToList();
                if (numbers.Count > 0 && numbers.Count <= 16)
                    return "(" + string.Join(",", numbers) + ")";
                return v.Length > 60 ? v.Substring(0, 57) + "..." : v;
            }
            return Trim(v);
        }

        static int CountOf(string haystack, string needle)
        {
            var n = 0;
            for (var i = haystack.IndexOf(needle, StringComparison.Ordinal); i >= 0; i = haystack.IndexOf(needle, i + needle.Length, StringComparison.Ordinal))
                n++;
            return n;
        }

        static string Trim(string number)
        {
            if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                return number;
            var rounded = Math.Round(d, 4);
            return rounded.ToString("0.####", CultureInfo.InvariantCulture);
        }

        static string Num(float f) => f.ToString("0.#", CultureInfo.InvariantCulture);
    }
}
