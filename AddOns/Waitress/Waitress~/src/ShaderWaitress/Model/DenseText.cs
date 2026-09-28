using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    public sealed class DenseTextOptions
    {
        public bool ShowPositions;
        public bool ShowOutputs;
        /// <summary>Hide unconnected literals that match the node type default.</summary>
        public bool Brief;
        public bool ShowNotes;
        public bool ShowHeader = true;
        public bool ShowAnalysis = true;
        public IReadOnlyCollection<SgNode> Only;   // null = whole graph
    }

    /// <summary>
    /// Renders a graph as the dense line-oriented form documented in `help format`. The goal
    /// is that every functional fact survives and every pixel coordinate does not.
    /// </summary>
    public static class DenseText
    {
        public static string Render(ShaderGraphDocument doc, DenseTextOptions options = null)
        {
            options ??= new DenseTextOptions();
            var sb = new StringBuilder();
            var analysis = new GraphAnalysis(doc);
            var filter = options.Only == null ? null : new HashSet<string>(options.Only.Select(n => n.ObjectId), StringComparer.Ordinal);

            if (options.ShowHeader)
            {
                WriteHeader(sb, doc);
                WriteTargets(sb, doc);
                sb.AppendLine();
            }

            if (filter == null)
            {
                if (WriteProperties(sb, doc))
                    sb.AppendLine();
                if (WriteKeywords(sb, doc))
                    sb.AppendLine();
                if (WriteGroups(sb, doc))
                    sb.AppendLine();
            }

            var nodes = analysis.ReadingOrder()
                .Where(n => filter == null || filter.Contains(n.ObjectId))
                .ToList();

            var lastComponent = -1;
            foreach (var node in nodes.Where(n => !n.IsBlock))
            {
                var component = analysis.ComponentOf(node);
                if (lastComponent >= 0 && component != lastComponent)
                    sb.AppendLine();
                lastComponent = component;
                sb.AppendLine(NodeLine(doc, node, options));
            }

            var blocks = nodes.Where(n => n.IsBlock).ToList();
            if (blocks.Count > 0)
            {
                if (lastComponent >= 0)
                    sb.AppendLine();
                foreach (var block in OrderBlocks(doc, blocks))
                    sb.AppendLine(NodeLine(doc, block, options));
            }

            if (options.ShowAnalysis && filter == null)
                WriteAnalysis(sb, doc, analysis);

            if (options.ShowNotes)
                WriteStickyNotes(sb, doc);

            return sb.ToString();
        }

        static IEnumerable<SgNode> OrderBlocks(ShaderGraphDocument doc, List<SgNode> blocks)
        {
            var order = new List<string>();
            order.AddRange(doc.ContextBlocks(vertex: true));
            order.AddRange(doc.ContextBlocks(vertex: false));
            var rank = new Dictionary<string, int>(StringComparer.Ordinal);
            for (var i = 0; i < order.Count; i++)
                rank[order[i]] = i;
            return blocks.OrderBy(b => rank.TryGetValue(b.ObjectId, out var r) ? r : int.MaxValue)
                         .ThenBy(b => b.BlockDescriptor, StringComparer.Ordinal);
        }

        static void WriteHeader(StringBuilder sb, ShaderGraphDocument doc)
        {
            var root = doc.RootNode;
            sb.Append("graph \"").Append(doc.DisplayName).Append('"');
            sb.Append(doc.IsSubGraph ? " kind=subgraph" : " kind=shadergraph");
            sb.Append(" nodes=").Append(doc.Nodes.Count(n => !n.IsBlock));
            sb.Append(" blocks=").Append(doc.Nodes.Count(n => n.IsBlock));
            sb.Append(" edges=").Append(doc.Edges.Count);
            sb.Append(" props=").Append(doc.Properties.Count);
            if (doc.Groups.Count > 0)
                sb.Append(" groups=").Append(doc.Groups.Count);
            var precision = ShaderWaitress.Serialization.Json.Number(root["m_GraphPrecision"]);
            if (precision != null)
                sb.Append(" precision=").Append(PrecisionLabel((int)precision.Value));
            var path = (string)root["m_Path"];
            if (!string.IsNullOrEmpty(path))
                sb.Append(" path=\"").Append(path).Append('"');
            sb.AppendLine();
        }

        static string PrecisionLabel(int value) => value switch
        {
            0 => "inherit",
            1 => "single",
            2 => "half",
            _ => value.ToString(CultureInfo.InvariantCulture),
        };

        static void WriteTargets(StringBuilder sb, ShaderGraphDocument doc)
        {
            foreach (var target in doc.Targets)
            {
                sb.Append("target ").Append(target.ShortId).Append(' ').Append(target.TypeLabel);
                var sub = target.SubTargetLabel;
                if (!string.IsNullOrEmpty(sub))
                    sb.Append('/').Append(sub);
                foreach (var setting in TargetSettings.Describe(target))
                    sb.Append(' ').Append(setting);
                sb.AppendLine();
            }
        }

        static bool WriteProperties(StringBuilder sb, ShaderGraphDocument doc)
        {
            if (doc.Properties.Count == 0)
                return false;
            var typeWidth = doc.Properties.Max(p => p.TypeLabel.Length);
            var nameWidth = Math.Min(30, doc.Properties.Max(p => p.Name.Length + 2));
            foreach (var p in doc.Properties)
            {
                sb.Append("prop ").Append(p.ShortId).Append(' ');
                sb.Append(p.TypeLabel.PadRight(typeWidth)).Append(' ');
                sb.Append(('"' + p.Name + '"').PadRight(nameWidth));
                sb.Append(" ref=").Append(p.ReferenceName);
                var value = p.FormattedValue;
                if (value != null)
                    sb.Append(" =").Append(value);
                // Only when overridden: the default is derived from the exposed flag, so
                // printing it on every line would be noise. "prop list" always shows it.
                if (p.DeclarationOverridden)
                    sb.Append(" decl=").Append(HlslDeclarations.Name(p.Declaration));
                // Only when on, since off is the default; on is what puts it on the parent shader.
                if (p.Promoted)
                    sb.Append(" promote=on");
                // Only when it is not White, which is what Shader Graph stores by default.
                if (p.HasTextureDefault && p.TextureDefault != 0)
                    sb.Append(" default=").Append(TextureDefaults.Name(p.TextureDefault));
                if (!p.Exposed)
                    sb.Append(" hidden");
                sb.AppendLine();
            }
            return true;
        }

        static bool WriteKeywords(StringBuilder sb, ShaderGraphDocument doc)
        {
            if (doc.Keywords.Count == 0)
                return false;
            foreach (var k in doc.Keywords)
            {
                sb.Append("keyword ").Append(k.ShortId).Append(" \"").Append(k.Name).Append('"');
                sb.Append(" ref=").Append(k.ReferenceName);
                sb.Append(" type=").Append(k.KeywordType == 0 ? "boolean" : "enum");
                sb.Append(" def=").Append(k.KeywordDefinition switch
                {
                    0 => "shaderfeature",
                    1 => "multicompile",
                    2 => "predefined",
                    _ => k.KeywordDefinition.ToString(CultureInfo.InvariantCulture),
                });
                sb.Append(" scope=").Append(k.KeywordScope == 0 ? "local" : "global");
                var entries = k.Entries;
                if (entries.Count > 0)
                    sb.Append(" entries=[").Append(string.Join(", ", entries)).Append(']');
                sb.Append(" default=").Append(k.Value.ToString(CultureInfo.InvariantCulture));
                // Only when off, since on is the default; off is what makes it a real #if.
                if (!k.AllowDefinitionOverride)
                    sb.Append(" allow-override=off");
                if (k.Promoted)
                    sb.Append(" promote=on");
                sb.AppendLine();
            }
            return true;
        }

        static bool WriteGroups(StringBuilder sb, ShaderGraphDocument doc)
        {
            if (doc.Groups.Count == 0)
                return false;
            foreach (var g in doc.Groups)
            {
                var members = doc.Nodes.Count(n => n.GroupId == g.ObjectId);
                sb.Append("group ").Append(g.ShortId).Append(" \"").Append(g.Title).Append('"');
                sb.Append(" members=").Append(members);
                sb.AppendLine();
            }
            return true;
        }

        public static string NodeLine(ShaderGraphDocument doc, SgNode node, DenseTextOptions options)
        {
            var sb = new StringBuilder();
            sb.Append(node.IsBlock ? "block " : "node ");
            sb.Append(node.ShortId).Append(' ');

            if (node.IsBlock)
            {
                sb.Append(node.BlockDescriptor ?? node.Name);
            }
            else
            {
                sb.Append(node.TypeLabel);
                var extra = Descriptor(doc, node);
                if (extra != null)
                    sb.Append(' ').Append(extra);
                if (!string.IsNullOrEmpty(node.Name) &&
                    !string.Equals(Squash(node.Name), Squash(node.TypeLabel), StringComparison.OrdinalIgnoreCase) &&
                    !node.IsProperty && !node.IsKeyword)
                {
                    sb.Append(" \"").Append(node.Name).Append('"');
                }
            }

            foreach (var setting in node.Settings)
            {
                if (options.Brief && setting.Display == null)
                    continue;
                sb.Append("  ").Append(setting.Name).Append((char)58).Append(setting.Display);
            }

            foreach (var text in InputTexts(doc, node, options))
                sb.Append("  ").Append(text);

            if (options.ShowOutputs)
            {
                foreach (var text in OutputTexts(doc, node))
                    sb.Append("  ").Append(text);
            }

            var groupId = node.GroupId;
            if (!string.IsNullOrEmpty(groupId))
            {
                var group = doc.GroupById(groupId);
                sb.Append("  @").Append(group?.ShortId ?? groupId);
            }

            if (options.ShowPositions)
            {
                // The size the layout works with, not the raw field: a node the Editor has
                // never drawn stores 0x0, and printing that hides the geometry every placement
                // decision is actually made on. A "~" marks one the tool estimated.
                var stored = node.Position;
                var r = ShaderWaitress.Layout.NodeSizer.Measure(node);
                var estimated = stored.Width <= 1 || stored.Height <= 1;
                sb.Append(string.Format(CultureInfo.InvariantCulture, "  #({0:0}, {1:0} {2:0}x{3:0}{4})",
                    r.X, r.Y, r.Width, r.Height, estimated ? "~" : string.Empty));
            }

            return sb.ToString();
        }

        /// <summary>The blackboard item a Property/Keyword/Dropdown node reads.</summary>
        static string Descriptor(ShaderGraphDocument doc, SgNode node)
        {
            if (node.IsProperty)
            {
                var prop = doc.PropertyById(node.PropertyId);
                return prop != null ? prop.ShortId + " \"" + prop.Name + "\"" : "<missing>";
            }
            if (node.IsKeyword)
            {
                var keyword = doc.Keywords.FirstOrDefault(k => k.ObjectId == node.KeywordId);
                return keyword != null ? keyword.ShortId + " \"" + keyword.Name + "\"" : "<missing>";
            }
            var subGraph = (string)node.Entry.Node["m_SerializedSubGraph"];
            if (!string.IsNullOrEmpty(subGraph))
            {
                var g = subGraph.IndexOf("\"guid\":\"", StringComparison.Ordinal);
                if (g >= 0)
                {
                    var start = g + 8;
                    var end = subGraph.IndexOf('"', start);
                    if (end > start)
                        return "sub:" + subGraph.Substring(start, Math.Min(8, end - start));
                }
            }
            var function = (string)node.Entry.Node["m_FunctionName"];
            if (!string.IsNullOrEmpty(function))
                return "fn:" + function;
            return null;
        }

        static string Squash(string s) => s == null ? null : s.Replace(" ", string.Empty);

        /// <summary>
        /// Port names appear as single tokens, so the space-free shader output name wins over
        /// the display name ("BaseColor" rather than "Base Color"). Both are accepted on input.
        /// </summary>
        public static string PortToken(SgPort port)
        {
            if (!string.IsNullOrEmpty(port.ShaderOutputName) && port.ShaderOutputName.IndexOf(' ') < 0)
                return port.ShaderOutputName;
            return Squash(port.Name) ?? port.Id.ToString(CultureInfo.InvariantCulture);
        }

        static IEnumerable<string> InputTexts(ShaderGraphDocument doc, SgNode node, DenseTextOptions options)
        {
            var incoming = doc.EdgesIn(node);
            foreach (var port in node.Inputs)
            {
                var edges = incoming.Where(e => e.ToSlot == port.Id).ToList();
                if (edges.Count > 0)
                {
                    foreach (var e in edges)
                        yield return PortToken(port) + "<-" + SourceRef(e);
                    continue;
                }
                // An unconnected input feeds the shader whether or not anyone typed the number,
                // so the literal is shown even when it is the type's own default. Multiply's
                // B defaults to 2: hiding that would hide a doubling.
                if (options.Brief && !port.HasNonDefaultValue)
                    continue;
                var literal = port.FormattedValue;
                if (literal != null)
                    yield return PortToken(port) + "=" + literal;
            }
        }

        static IEnumerable<string> OutputTexts(ShaderGraphDocument doc, SgNode node)
        {
            foreach (var e in doc.EdgesOut(node))
            {
                var port = node.Ports.FirstOrDefault(p => p.Id == e.FromSlot && !p.IsInput);
                var toPort = e.ToPort;
                var target = e.To.ShortId + "." + (toPort != null ? PortToken(toPort) : e.ToSlot.ToString(CultureInfo.InvariantCulture));
                yield return (port != null ? PortToken(port) : e.FromSlot.ToString(CultureInfo.InvariantCulture)) + "->" + target;
            }
        }

        /// <summary>
        /// "n4200.Out", shortened to "n4200" when the source node has a single output, which
        /// is the common case and removes a lot of noise.
        /// </summary>
        static string SourceRef(SgEdge edge)
        {
            var from = edge.From;
            var outputs = from.Outputs.ToList();
            if (outputs.Count == 1 && outputs[0].Id == edge.FromSlot)
                return from.ShortId;
            var port = outputs.FirstOrDefault(p => p.Id == edge.FromSlot);
            return from.ShortId + "." + (port != null ? PortToken(port) : edge.FromSlot.ToString(CultureInfo.InvariantCulture));
        }

        static void WriteAnalysis(StringBuilder sb, ShaderGraphDocument doc, GraphAnalysis analysis)
        {
            var lines = new List<string>();

            var unreachable = doc.Nodes.Where(n => !analysis.IsReachableFromBlocks(n)).ToList();
            if (unreachable.Count > 0 && doc.Nodes.Any(n => n.IsBlock))
                lines.Add("not feeding any block: " + Join(unreachable));

            var orphans = doc.Nodes.Where(n => doc.EdgesIn(n).Count == 0 && doc.EdgesOut(n).Count == 0 && !n.IsBlock).ToList();
            if (orphans.Count > 0)
                lines.Add("unconnected: " + Join(orphans));

            var danglingInputs = new List<string>();
            foreach (var node in doc.Nodes)
            {
                if (!node.IsBlock)
                    continue;
                if (doc.EdgesIn(node).Count == 0)
                    danglingInputs.Add(node.ShortId);
            }
            if (danglingInputs.Count > 0)
                lines.Add("blocks with no input: " + string.Join(" ", danglingInputs));

            if (analysis.ComponentCount > 1)
                lines.Add($"clusters: {analysis.ComponentCount}");

            if (analysis.BackEdges.Count > 0)
                lines.Add($"cycles: {analysis.BackEdges.Count} edge(s) close a loop");

            foreach (var warning in doc.Warnings)
                lines.Add("warning: " + warning);

            if (lines.Count == 0)
                return;
            sb.AppendLine();
            foreach (var line in lines)
                sb.Append("# ").AppendLine(line);
        }

        static string Join(IEnumerable<SgNode> nodes)
        {
            var list = nodes.Select(n => n.ShortId).ToList();
            if (list.Count <= 12)
                return string.Join(" ", list);
            return string.Join(" ", list.Take(12)) + $" (+{list.Count - 12} more)";
        }

        static void WriteStickyNotes(StringBuilder sb, ShaderGraphDocument doc)
        {
            var ids = doc.RefList(doc.RootNode, "m_StickyNoteDatas").ToList();
            if (ids.Count == 0)
                return;
            sb.AppendLine();
            foreach (var id in ids)
            {
                var entry = doc.Raw.Find(id);
                if (entry == null)
                    continue;
                var title = (string)entry.Node["m_Title"] ?? string.Empty;
                var content = ((string)entry.Node["m_Content"] ?? string.Empty).Replace("\n", "\\n");
                sb.Append("note \"").Append(title).Append("\" ").AppendLine(content);
            }
        }
    }
}
