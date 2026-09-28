using System;
using System.Collections.Generic;
using System.Linq;
using VfxWaitress.Model;

namespace VfxWaitress.Query
{
    /// <summary>
    /// A pipeline of steps separated by <c>|</c>. The first step seeds a set of nodes; later steps
    /// filter it or walk the wires. Whole pipelines combine with <c>+</c>, <c>-</c> and <c>&amp;</c>.
    /// </summary>
    public static class Selector
    {
        public static IEnumerable<VfxNode> Resolve(VfxAsset asset, string expression)
        {
            var result = Evaluate(asset, expression);
            return result.OrderBy(n => n.X).ThenBy(n => n.Y);
        }

        static HashSet<VfxNode> Evaluate(VfxAsset asset, string expression)
        {
            var tokens = SplitTop(expression);
            var current = Pipeline(asset, tokens.first);
            foreach (var (op, operand) in tokens.rest)
            {
                var other = Pipeline(asset, operand);
                switch (op)
                {
                    case '+': current.UnionWith(other); break;
                    case '-': current.ExceptWith(other); break;
                    case '&': current.IntersectWith(other); break;
                }
            }
            return current;
        }

        static (string first, List<(char, string)> rest) SplitTop(string expression)
        {
            var parts = new List<(char, string)>();
            var start = 0;
            var first = expression;
            var inQuote = false;
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                if (c == '"')
                    inQuote = !inQuote;
                if (inQuote || (c != '+' && c != '-' && c != '&'))
                    continue;
                // A '-' inside a word is part of it, not a set difference.
                if (c == '-' && (i == 0 || expression[i - 1] != ' '))
                    continue;
                if (parts.Count == 0 && start == 0)
                    first = expression.Substring(0, i);
                else
                    parts[parts.Count - 1] = (parts[parts.Count - 1].Item1, expression.Substring(start, i - start));
                parts.Add((c, ""));
                start = i + 1;
            }
            if (parts.Count > 0)
                parts[parts.Count - 1] = (parts[parts.Count - 1].Item1, expression.Substring(start));
            return (first, parts);
        }

        static HashSet<VfxNode> Pipeline(VfxAsset asset, string expression)
        {
            HashSet<VfxNode> set = null;
            foreach (var rawStep in expression.Split('|'))
            {
                var step = rawStep.Trim();
                if (step.Length == 0)
                    continue;
                set = set == null ? Seed(asset, step) : Step(asset, set, step);
            }
            return set ?? new HashSet<VfxNode>();
        }

        static HashSet<VfxNode> Seed(VfxAsset asset, string step)
        {
            var all = asset.AllNodes().ToList();
            if (step == "all")
                return new HashSet<VfxNode>(all);
            return new HashSet<VfxNode>(all.Where(n => Matches(asset, n, step)));
        }

        static HashSet<VfxNode> Step(VfxAsset asset, HashSet<VfxNode> set, string step)
        {
            switch (step)
            {
                case "in":
                    return new HashSet<VfxNode>(set.SelectMany(n => Upstream(asset, n)));
                case "out":
                    return new HashSet<VfxNode>(set.SelectMany(n => Downstream(asset, n)));
                case "both":
                    return new HashSet<VfxNode>(set.SelectMany(n => Upstream(asset, n).Concat(Downstream(asset, n))));
                case "in*":
                    return Closure(asset, set, true);
                case "out*":
                    return Closure(asset, set, false);
                case "blocks":
                    return new HashSet<VfxNode>(set.SelectMany(n => n.Blocks));
                case "context":
                    return new HashSet<VfxNode>(set.Select(n => n.Context).Where(n => n != null));
            }
            return new HashSet<VfxNode>(set.Where(n => Matches(asset, n, step)));
        }

        static HashSet<VfxNode> Closure(VfxAsset asset, HashSet<VfxNode> seed, bool upstream)
        {
            var result = new HashSet<VfxNode>();
            var queue = new Queue<VfxNode>(seed);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                foreach (var next in upstream ? Upstream(asset, node) : Downstream(asset, node))
                {
                    if (result.Add(next))
                        queue.Enqueue(next);
                }
            }
            return result;
        }

        static IEnumerable<VfxNode> Upstream(VfxAsset asset, VfxNode node)
        {
            foreach (var slot in node.AllInputSlots)
            {
                foreach (var id in slot.LinkedSlotIds)
                {
                    var source = asset.SlotById(id);
                    if (source?.Owner != null)
                        yield return source.Owner;
                }
            }
        }

        static IEnumerable<VfxNode> Downstream(VfxAsset asset, VfxNode node)
        {
            var mine = new HashSet<long>(node.AllOutputSlots.Select(s => s.FileId));
            foreach (var other in asset.AllNodes())
            {
                foreach (var slot in other.AllInputSlots)
                {
                    if (slot.LinkedSlotIds.Any(mine.Contains))
                    {
                        yield return other;
                        break;
                    }
                }
            }
        }

        static bool Matches(VfxAsset asset, VfxNode node, string step)
        {
            var colon = step.IndexOf(':');
            var key = colon < 0 ? null : step.Substring(0, colon);
            var value = colon < 0 ? step : step.Substring(colon + 1).Trim('"');

            switch (key)
            {
                case null:
                    // A bare word is a short id, a raw file id, or a type.
                    return Any(value, v => string.Equals(node.ShortId, v, StringComparison.OrdinalIgnoreCase) ||
                                           node.FileId.ToString(System.Globalization.CultureInfo.InvariantCulture) == v ||
                                           Glob.CompileLoose(v)(node.DisplayType));
                case "id":
                    return Any(value, v => string.Equals(node.ShortId, v, StringComparison.OrdinalIgnoreCase) ||
                                           node.FileId.ToString() == v);
                case "type":
                    return Any(value, v => Glob.CompileLoose(v)(node.DisplayType) || Glob.CompileLoose(v)(node.TypeName));
                case "name":
                    return Any(value, v => Glob.Compile(v)(node.Model?.Name) || Glob.Compile(v)(node.Label) ||
                                           Glob.Compile(v)(node.ExposedName));
                case "label":
                    return Any(value, v => Glob.Compile(v)(node.Label));
                case "kind":
                    return Any(value, v => string.Equals(node.Kind, v, StringComparison.OrdinalIgnoreCase));
                case "context":
                    return node.Context != null && Any(value, v =>
                        string.Equals(node.Context.ShortId, v, StringComparison.OrdinalIgnoreCase) ||
                        Glob.CompileLoose(v)(node.Context.DisplayType));
                case "system":
                    return node.Data != null && Any(value, v =>
                        string.Equals(node.Data.ShortId, v, StringComparison.OrdinalIgnoreCase) || Glob.Compile(v)(node.Data.Title));
                case "param":
                    return node.ExposedName != null && Any(value, v => Glob.Compile(v)(node.ExposedName));
                case "setting":
                {
                    var eq = value.IndexOf('=');
                    if (eq < 0)
                        return node.Doc.Body.Find(value) != null || node.Doc.Body.Find("m_" + value) != null;
                    var settingName = value.Substring(0, eq);
                    var settingValue = value.Substring(eq + 1);
                    var raw = node.Doc.Body.Scalar(settingName) ?? node.Doc.Body.Scalar("m_" + settingName);
                    return raw != null && Glob.Compile(settingValue)(raw.Trim());
                }
                case "port":
                case "slot":
                    return Any(value, v => node.AllInputSlots.Concat(node.AllOutputSlots).Any(s => Loose.Equal(s.Path, v)));
                case "is":
                    return Is(asset, node, value);
            }
            throw new VfxWaitressException($"unknown selector step '{step}'");
        }

        static bool Is(VfxAsset asset, VfxNode node, string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "context": return node.Kind == "context";
                case "block": return node.Kind == "block";
                case "operator": return node.Kind == "operator";
                case "param":
                case "parameter": return node.ExposedName != null;
                case "input": return node.ExposedName != null && !node.IsOutput;
                case "output": return node.ExposedName != null && node.IsOutput;
                case "unknown": return node.Model == null;
                case "orphan":
                    // A context is joined by flow rather than by wires, and a block belongs to
                    // its context; neither is orphaned just because no data reaches it.
                    if (node.Kind == "block")
                        return false;
                    if (node.Kind == "context")
                        return node.FlowOut.Count == 0 && !asset.AllNodes().Any(n => n.FlowOut.Any(f => f.toContext == node.FileId));
                    return !node.AllInputSlots.Any(s => s.LinkedSlotIds.Count > 0) && !Downstream(asset, node).Any();
                case "root": return !node.AllInputSlots.Any(s => s.LinkedSlotIds.Count > 0);
                case "leaf": return !Downstream(asset, node).Any();
            }
            throw new VfxWaitressException($"unknown 'is:' test '{value}'");
        }

        static bool Any(string commaList, Func<string, bool> predicate) =>
            commaList.Split(',').Select(s => s.Trim()).Any(predicate);
    }

    /// <summary>
    /// Slot names are addressed by many spellings of the same thing: a SetAttribute block set to
    /// <c>position</c> names its slot <c>_Position</c>, and the display name of a port has spaces
    /// the serialized name does not. Comparison ignores both.
    /// </summary>
    public static class Loose
    {
        public static string Normalize(string s) =>
            s == null ? null : new string(s.Where(c => c != '_' && c != ' ' && c != '.').ToArray()).ToLowerInvariant();

        public static bool Equal(string a, string b) =>
            string.Equals(Normalize(a), Normalize(b), StringComparison.Ordinal);
    }
}
