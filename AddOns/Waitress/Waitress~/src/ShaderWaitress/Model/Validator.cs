using System;
using System.Collections.Generic;
using System.Linq;
using ShaderWaitress.Layout;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Integrity and readability checks. Everything reported here is something Shader Graph
    /// would either refuse to load or a human would complain about; nothing here is a matter
    /// of taste.
    /// </summary>
    public static class Validator
    {
        public static List<string> Run(ShaderGraphDocument doc, bool includeLayout = true)
        {
            var problems = new List<string>();

            foreach (var warning in doc.Warnings)
                problems.Add("structure: " + warning);

            var referenced = new HashSet<string>(StringComparer.Ordinal);
            Walk(doc, doc.Raw.Root, referenced, new HashSet<string>(StringComparer.Ordinal));
            foreach (var entry in doc.Raw.Entries)
            {
                if (entry == doc.Raw.Root || referenced.Contains(entry.ObjectId))
                    continue;
                problems.Add($"orphan object: {entry.ObjectId} ({SlotTypes.ShortName(entry.TypeName)}) is not referenced by anything");
            }

            foreach (var edge in doc.Edges)
            {
                if (edge.FromPort == null)
                    problems.Add($"edge: {edge.From.ShortId} has no output slot {edge.FromSlot}");
                if (edge.ToPort == null)
                    problems.Add($"edge: {edge.To.ShortId} has no input slot {edge.ToSlot}");
            }

            var byInput = new Dictionary<(string, int), int>();
            foreach (var edge in doc.Edges)
            {
                var key = (edge.To.ObjectId, edge.ToSlot);
                byInput[key] = byInput.GetValueOrDefault(key) + 1;
            }
            foreach (var pair in byInput.Where(p => p.Value > 1))
            {
                var node = doc.NodeById(pair.Key.Item1);
                problems.Add($"edge: {node?.ShortId} input slot {pair.Key.Item2} has {pair.Value} incoming wires (Shader Graph allows one)");
            }

            foreach (var node in doc.Nodes)
            {
                if (node.IsProperty && doc.PropertyById(node.PropertyId) == null)
                    problems.Add($"node: {node.ShortId} is a Property node with no property");
                if (!string.IsNullOrEmpty(node.GroupId) && doc.GroupById(node.GroupId) == null)
                    problems.Add($"node: {node.ShortId} points at a group that does not exist");
            }

            var contextIds = doc.ContextBlocks(true).Concat(doc.ContextBlocks(false)).ToHashSet(StringComparer.Ordinal);
            foreach (var node in doc.Nodes.Where(n => n.IsBlock))
            {
                if (!contextIds.Contains(node.ObjectId))
                    problems.Add($"block: {node.ShortId} ({node.BlockDescriptor}) is not listed in a vertex or fragment context");
            }
            foreach (var id in contextIds)
            {
                if (doc.NodeById(id) == null)
                    problems.Add($"context: block {id} is listed in a context but is not in the node list");
            }

            var analysis = new GraphAnalysis(doc);
            if (analysis.BackEdges.Count > 0)
                problems.Add($"cycle: {analysis.BackEdges.Count} edge(s) form a loop, which Shader Graph does not allow");

            if (includeLayout)
            {
                var metrics = LayoutMetrics.Measure(doc);
                if (metrics.NodeOverlaps > 0)
                    problems.Add($"layout: {metrics.NodeOverlaps} pair(s) of nodes overlap; run 'shaderwaitress layout'");
                if (metrics.BackwardEdges > 0)
                {
                    // Groups that feed each other cannot both be left of the other, so telling
                    // the caller to run layout would send them round in a circle.
                    var stuck = Layout.LayoutEngine.MutuallyFeedingGroups(doc).ToList();
                    problems.Add(stuck.Count > 1
                        ? $"layout: {metrics.BackwardEdges} wire(s) travel right to left, because these groups feed " +
                          $"each other: {string.Join(", ", stuck)}. Splitting or merging them is the only fix."
                        : $"layout: {metrics.BackwardEdges} wire(s) travel right to left; run 'shaderwaitress layout'");
                }
                if (metrics.GroupOverlaps > 0)
                    problems.Add($"layout: {metrics.GroupOverlaps} pair(s) of groups overlap; run 'shaderwaitress layout'");
            }

            // The block list does not follow the target when the target changes, so a graph can
            // be structurally perfect and still have no output for the thing it exists to do.
            foreach (var missing in BlockAdvice.Missing(doc))
                problems.Add($"blocks: {missing}; 'block add' or 'target set --sync-blocks' adds it");

            return problems;
        }

        static void Walk(ShaderGraphDocument doc, MultiJsonEntry entry, HashSet<string> referenced, HashSet<string> visited)
        {
            if (entry == null || !visited.Add(entry.ObjectId))
                return;
            foreach (var id in CollectRefs(entry.Node))
            {
                referenced.Add(id);
                Walk(doc, doc.Raw.Find(id), referenced, visited);
            }
        }

        static IEnumerable<string> CollectRefs(System.Text.Json.Nodes.JsonNode node)
        {
            switch (node)
            {
                case System.Text.Json.Nodes.JsonObject obj:
                {
                    if (obj.Count == 1 && obj.TryGetPropertyValue("m_Id", out var value) && value is System.Text.Json.Nodes.JsonValue)
                    {
                        var id = (string)value;
                        if (!string.IsNullOrEmpty(id))
                            yield return id;
                        yield break;
                    }
                    foreach (var pair in obj)
                    {
                        foreach (var id in CollectRefs(pair.Value))
                            yield return id;
                    }
                    yield break;
                }
                case System.Text.Json.Nodes.JsonArray arr:
                {
                    foreach (var item in arr)
                    {
                        foreach (var id in CollectRefs(item))
                            yield return id;
                    }
                    yield break;
                }
            }
        }
    }
}
