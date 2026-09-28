using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ShaderWaitress.Model;

namespace ShaderWaitress.Query
{
    /// <summary>
    /// The chainable selector language. A pipeline of steps separated by '|' seeds a node set
    /// and then filters or walks it; whole pipelines combine with '+', '-' and '&amp;'.
    /// </summary>
    public static class Selector
    {
        public static IReadOnlyList<SgNode> Evaluate(ShaderGraphDocument doc, string expression, GraphAnalysis analysis = null)
        {
            analysis ??= new GraphAnalysis(doc);
            var tokens = Tokenize(expression);
            var index = 0;
            var result = ParseUnion(doc, analysis, tokens, ref index);
            if (index < tokens.Count)
                throw new ShaderWaitressException($"unexpected '{tokens[index]}' in selector");
            return Order(doc, analysis, result);
        }

        static IReadOnlyList<SgNode> Order(ShaderGraphDocument doc, GraphAnalysis analysis, HashSet<string> ids)
        {
            return analysis.ReadingOrder().Where(n => ids.Contains(n.ObjectId)).ToList();
        }

        // ---- parsing ----

        static List<string> Tokenize(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ShaderWaitressException("empty selector");
            var tokens = new List<string>();
            var sb = new StringBuilder();
            var quote = '\0';
            foreach (var c in expression)
            {
                if (quote != '\0')
                {
                    if (c == quote)
                        quote = '\0';
                    else
                        sb.Append(c);
                    continue;
                }
                if (c is '"' or '\'')
                {
                    quote = c;
                    continue;
                }
                if (c is '|' or '+' or '&')
                {
                    Flush(tokens, sb);
                    tokens.Add(c.ToString());
                    continue;
                }
                if (c == '-' && sb.Length == 0)
                {
                    // Only a separator between steps, never inside a name.
                    tokens.Add("-");
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    Flush(tokens, sb);
                    continue;
                }
                sb.Append(c);
            }
            if (quote != '\0')
                throw new ShaderWaitressException("unterminated quote in selector");
            Flush(tokens, sb);
            return tokens;
        }

        static void Flush(List<string> tokens, StringBuilder sb)
        {
            if (sb.Length == 0)
                return;
            tokens.Add(sb.ToString());
            sb.Clear();
        }

        static HashSet<string> ParseUnion(ShaderGraphDocument doc, GraphAnalysis analysis, List<string> tokens, ref int index)
        {
            var left = ParsePipeline(doc, analysis, tokens, ref index);
            while (index < tokens.Count && tokens[index] is "+" or "-" or "&")
            {
                var op = tokens[index++];
                var right = ParsePipeline(doc, analysis, tokens, ref index);
                switch (op)
                {
                    case "+":
                        left.UnionWith(right);
                        break;
                    case "-":
                        left.ExceptWith(right);
                        break;
                    default:
                        left.IntersectWith(right);
                        break;
                }
            }
            return left;
        }

        static HashSet<string> ParsePipeline(ShaderGraphDocument doc, GraphAnalysis analysis, List<string> tokens, ref int index)
        {
            var current = new HashSet<string>(doc.Nodes.Select(n => n.ObjectId), StringComparer.Ordinal);
            var applied = false;
            while (index < tokens.Count)
            {
                var token = tokens[index];
                if (token is "+" or "-" or "&")
                    break;
                index++;
                if (token == "|")
                    continue;
                current = ApplyStep(doc, analysis, current, token);
                applied = true;
            }
            if (!applied)
                throw new ShaderWaitressException("selector ends where a step was expected");
            return current;
        }

        // ---- steps ----

        static HashSet<string> ApplyStep(ShaderGraphDocument doc, GraphAnalysis analysis, HashSet<string> current, string step)
        {
            var colon = step.IndexOf(':');
            var head = colon < 0 ? step : step.Substring(0, colon);
            var arg = colon < 0 ? null : step.Substring(colon + 1);

            switch (head.ToLowerInvariant())
            {
                case "all":
                    return new HashSet<string>(doc.Nodes.Select(n => n.ObjectId), StringComparer.Ordinal);
                case "none":
                    return new HashSet<string>(StringComparer.Ordinal);

                case "in":
                case "upstream":
                    return Traverse(doc, current, arg, upstream: true, transitive: false);
                case "in*":
                case "upstream*":
                    return Traverse(doc, current, arg, upstream: true, transitive: true);
                case "out":
                case "downstream":
                    return Traverse(doc, current, arg, upstream: false, transitive: false);
                case "out*":
                case "downstream*":
                    return Traverse(doc, current, arg, upstream: false, transitive: true);
                case "both":
                {
                    var set = Traverse(doc, current, arg, upstream: true, transitive: false);
                    set.UnionWith(Traverse(doc, current, arg, upstream: false, transitive: false));
                    return set;
                }

                case "type":
                    return Filter(doc, current, n => AnyGlob(arg, g => g(n.TypeLabel) || g(SlotTypes.ShortName(n.TypeName)) || g(n.TypeName)));
                case "name":
                    return Filter(doc, current, n => AnyGlob(arg, g => g(n.Name)));
                case "id":
                    return FilterIds(doc, current, arg);
                case "group":
                    return Filter(doc, current, n =>
                    {
                        var group = doc.GroupById(n.GroupId);
                        return group != null && AnyGlob(arg, g => g(group.Title) || g(group.ShortId));
                    });
                case "prop":
                case "property":
                    return Filter(doc, current, n =>
                    {
                        if (!n.IsProperty)
                            return false;
                        var p = doc.PropertyById(n.PropertyId);
                        return p != null && AnyGlob(arg, g => g(p.Name) || g(p.ReferenceName) || g(p.ShortId));
                    });
                case "block":
                    return Filter(doc, current, n => n.IsBlock && AnyGlob(arg, g => g(n.BlockDescriptor) ||
                        (n.BlockDescriptor != null && g(n.BlockDescriptor.Substring(n.BlockDescriptor.IndexOf('.') + 1)))));
                case "port":
                    return Filter(doc, current, n => n.Ports.Any(p => AnyGlob(arg, g => g(p.Name) || g(p.ShaderOutputName))));
                case "rank":
                    return Filter(doc, current, n => MatchNumber(arg, analysis.RankOf(n)));
                case "is":
                    return FilterTags(doc, analysis, current, arg);
                default:
                    // A bare word is shorthand for name, type or id. The full object id counts too,
                    // because that is what a script's $label expands to.
                    if (colon < 0)
                        return Filter(doc, current, n => AnyGlob(step,
                            g => g(n.Name) || g(n.TypeLabel) || g(n.ShortId) || g(n.ObjectId)));
                    throw new ShaderWaitressException($"unknown selector step '{step}'. See 'shaderwaitress help select'.");
            }
        }

        static bool MatchNumber(string arg, int value)
        {
            if (string.IsNullOrEmpty(arg))
                return false;
            if (arg.StartsWith(">=", StringComparison.Ordinal))
                return value >= int.Parse(arg.Substring(2), CultureInfo.InvariantCulture);
            if (arg.StartsWith("<=", StringComparison.Ordinal))
                return value <= int.Parse(arg.Substring(2), CultureInfo.InvariantCulture);
            if (arg[0] == '>')
                return value > int.Parse(arg.Substring(1), CultureInfo.InvariantCulture);
            if (arg[0] == '<')
                return value < int.Parse(arg.Substring(1), CultureInfo.InvariantCulture);
            return value == int.Parse(arg, CultureInfo.InvariantCulture);
        }

        static bool AnyGlob(string arg, Func<Func<string, bool>, bool> test)
        {
            if (arg == null)
                throw new ShaderWaitressException("this selector step needs an argument, e.g. type:Multiply");
            foreach (var piece in arg.Split(','))
            {
                if (piece.Length == 0)
                    continue;
                if (test(Glob.Compile(piece)))
                    return true;
            }
            return false;
        }

        static HashSet<string> Filter(ShaderGraphDocument doc, HashSet<string> current, Func<SgNode, bool> predicate)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in current)
            {
                var node = doc.NodeById(id);
                if (node != null && predicate(node))
                    result.Add(id);
            }
            return result;
        }

        static HashSet<string> FilterIds(ShaderGraphDocument doc, HashSet<string> current, string arg)
        {
            var wanted = new HashSet<string>(StringComparer.Ordinal);
            foreach (var piece in (arg ?? string.Empty).Split(','))
            {
                if (piece.Length == 0)
                    continue;
                var node = doc.ResolveNode(piece);
                wanted.Add(node.ObjectId);
            }
            wanted.IntersectWith(current);
            return wanted;
        }

        static HashSet<string> FilterTags(ShaderGraphDocument doc, GraphAnalysis analysis, HashSet<string> current, string arg)
        {
            if (arg == null)
                throw new ShaderWaitressException("is: needs a tag, e.g. is:orphan");
            var tags = arg.Split(',').Where(t => t.Length > 0).Select(t => t.ToLowerInvariant()).ToList();
            return Filter(doc, current, n => tags.Any(tag => MatchTag(doc, analysis, n, tag)));
        }

        static bool MatchTag(ShaderGraphDocument doc, GraphAnalysis analysis, SgNode n, string tag)
        {
            switch (tag)
            {
                case "orphan":
                case "unconnected":
                    return doc.EdgesIn(n).Count == 0 && doc.EdgesOut(n).Count == 0;
                case "root":
                case "source":
                    return doc.EdgesIn(n).Count == 0;
                case "leaf":
                case "sink":
                    return doc.EdgesOut(n).Count == 0;
                case "block":
                    return n.IsBlock;
                case "property":
                    return n.IsProperty;
                case "keyword":
                    return n.IsKeyword;
                case "grouped":
                    return !string.IsNullOrEmpty(n.GroupId);
                case "ungrouped":
                    return string.IsNullOrEmpty(n.GroupId);
                case "unreachable":
                    return !analysis.IsReachableFromBlocks(n);
                case "reachable":
                    return analysis.IsReachableFromBlocks(n);
                case "node":
                    return !n.IsBlock;
                default:
                    throw new ShaderWaitressException($"unknown tag 'is:{tag}'. Known: orphan, root, leaf, block, property, keyword, grouped, ungrouped, reachable, unreachable, node");
            }
        }

        static HashSet<string> Traverse(ShaderGraphDocument doc, HashSet<string> current, string portFilter, bool upstream, bool transitive)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            var frontier = new Queue<string>(current);
            var seen = new HashSet<string>(current, StringComparer.Ordinal);
            var firstHop = true;

            while (frontier.Count > 0)
            {
                var level = frontier.Count;
                for (var i = 0; i < level; i++)
                {
                    var id = frontier.Dequeue();
                    var node = doc.NodeById(id);
                    if (node == null)
                        continue;
                    var edges = upstream ? doc.EdgesIn(node) : doc.EdgesOut(node);
                    foreach (var e in edges)
                    {
                        if (firstHop && portFilter != null && !PortMatches(e, portFilter, upstream))
                            continue;
                        var next = upstream ? e.From : e.To;
                        if (!result.Add(next.ObjectId))
                            continue;
                        if (transitive && seen.Add(next.ObjectId))
                            frontier.Enqueue(next.ObjectId);
                    }
                }
                firstHop = false;
                if (!transitive)
                    break;
            }
            return result;
        }

        static bool PortMatches(SgEdge edge, string pattern, bool upstream)
        {
            var glob = Glob.Compile(pattern);
            var port = upstream ? edge.ToPort : edge.FromPort;
            if (port == null)
                return false;
            return glob(port.Name) || glob(port.ShaderOutputName ?? string.Empty) ||
                   glob(port.Name?.Replace(" ", string.Empty) ?? string.Empty);
        }
    }
}
