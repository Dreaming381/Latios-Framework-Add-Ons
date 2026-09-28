using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Structural facts shared by the renderer, the selector language and layout: ranks,
    /// connected components, reachability, and cycle detection. Everything here tolerates
    /// hand-authored graphs, including cyclic ones, rather than assuming a clean DAG.
    /// </summary>
    public sealed class GraphAnalysis
    {
        public ShaderGraphDocument Doc { get; }

        readonly Dictionary<string, int> m_Rank = new Dictionary<string, int>(StringComparer.Ordinal);
        readonly Dictionary<string, int> m_Component = new Dictionary<string, int>(StringComparer.Ordinal);
        readonly HashSet<string> m_ReachableFromBlocks = new HashSet<string>(StringComparer.Ordinal);
        readonly List<SgEdge> m_BackEdges = new List<SgEdge>();
        readonly HashSet<SgEdge> m_BackEdgeSet = new HashSet<SgEdge>();
        List<List<SgNode>> m_Components;
        List<SgNode> m_TopoOrder;

        public int ComponentCount => m_Components.Count;
        public IReadOnlyList<List<SgNode>> Components => m_Components;
        public IReadOnlyList<SgEdge> BackEdges => m_BackEdges;
        public int MaxRank { get; private set; }

        public GraphAnalysis(ShaderGraphDocument doc)
        {
            Doc = doc;
            BuildComponents();
            BuildTopoOrder();
            BuildRanks();
            BuildReachability();
        }

        public int RankOf(SgNode node) => m_Rank.TryGetValue(node.ObjectId, out var r) ? r : 0;
        public int ComponentOf(SgNode node) => m_Component.TryGetValue(node.ObjectId, out var c) ? c : 0;
        public bool IsReachableFromBlocks(SgNode node) => m_ReachableFromBlocks.Contains(node.ObjectId);
        public IReadOnlyList<SgNode> TopoOrder => m_TopoOrder;

        void BuildComponents()
        {
            m_Components = new List<List<SgNode>>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var node in Doc.Nodes)
            {
                if (!seen.Add(node.ObjectId))
                    continue;
                var index = m_Components.Count;
                var bucket = new List<SgNode>();
                var stack = new Stack<SgNode>();
                stack.Push(node);
                m_Component[node.ObjectId] = index;
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    bucket.Add(current);
                    foreach (var e in Doc.EdgesOut(current))
                    {
                        if (seen.Add(e.To.ObjectId))
                        {
                            m_Component[e.To.ObjectId] = index;
                            stack.Push(e.To);
                        }
                    }
                    foreach (var e in Doc.EdgesIn(current))
                    {
                        if (seen.Add(e.From.ObjectId))
                        {
                            m_Component[e.From.ObjectId] = index;
                            stack.Push(e.From);
                        }
                    }
                }
                m_Components.Add(bucket);
            }
        }

        /// <summary>
        /// Kahn's algorithm. Any node left over is in a cycle; its remaining incoming edges
        /// are recorded as back edges and ignored so the rest of the pipeline still works.
        /// </summary>
        void BuildTopoOrder()
        {
            var indegree = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var node in Doc.Nodes)
                indegree[node.ObjectId] = 0;
            foreach (var e in Doc.Edges)
            {
                if (e.From == e.To)
                {
                    m_BackEdges.Add(e);
                    m_BackEdgeSet.Add(e);
                    continue;
                }
                indegree[e.To.ObjectId] = indegree.GetValueOrDefault(e.To.ObjectId) + 1;
            }

            var ready = new List<SgNode>(Doc.Nodes.Where(n => indegree[n.ObjectId] == 0));
            m_TopoOrder = new List<SgNode>();
            var placed = new HashSet<string>(StringComparer.Ordinal);
            while (ready.Count > 0)
            {
                var node = ready[ready.Count - 1];
                ready.RemoveAt(ready.Count - 1);
                if (!placed.Add(node.ObjectId))
                    continue;
                m_TopoOrder.Add(node);
                foreach (var e in Doc.EdgesOut(node))
                {
                    if (e.From == e.To)
                        continue;
                    var left = --indegree[e.To.ObjectId];
                    if (left == 0)
                        ready.Add(e.To);
                }
            }

            if (m_TopoOrder.Count == Doc.Nodes.Count)
                return;

            // Cyclic remainder: break edges into the lowest-indegree node until it drains.
            var remaining = Doc.Nodes.Where(n => !placed.Contains(n.ObjectId)).ToList();
            foreach (var node in remaining.OrderBy(n => indegree[n.ObjectId]))
            {
                if (placed.Contains(node.ObjectId))
                    continue;
                foreach (var e in Doc.EdgesIn(node))
                {
                    if (!placed.Contains(e.From.ObjectId) && m_BackEdgeSet.Add(e))
                        m_BackEdges.Add(e);
                }
                var stack = new Stack<SgNode>();
                stack.Push(node);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (!placed.Add(current.ObjectId))
                        continue;
                    m_TopoOrder.Add(current);
                    foreach (var e in Doc.EdgesOut(current))
                    {
                        if (!placed.Contains(e.To.ObjectId))
                            stack.Push(e.To);
                    }
                }
            }
        }

        bool IsBackEdge(SgEdge e) => m_BackEdgeSet.Contains(e);

        /// <summary>
        /// Rank 0 is the rightmost column. A node's rank is one past the deepest consumer it
        /// feeds, which puts producers immediately left of what they feed and pins block
        /// nodes to the right edge.
        /// </summary>
        void BuildRanks()
        {
            foreach (var node in Doc.Nodes)
                m_Rank[node.ObjectId] = 0;

            for (var i = m_TopoOrder.Count - 1; i >= 0; i--)
            {
                var node = m_TopoOrder[i];
                var rank = 0;
                foreach (var e in Doc.EdgesOut(node))
                {
                    if (e.From == e.To || IsBackEdge(e))
                        continue;
                    rank = Math.Max(rank, m_Rank[e.To.ObjectId] + 1);
                }
                m_Rank[node.ObjectId] = rank;
            }

            // Blocks belong in the last column together even when one is fed and another is not.
            var blockRank = 0;
            foreach (var node in Doc.Nodes)
            {
                if (node.IsBlock)
                    blockRank = Math.Max(blockRank, m_Rank[node.ObjectId]);
            }
            foreach (var node in Doc.Nodes)
            {
                if (node.IsBlock)
                    m_Rank[node.ObjectId] = blockRank;
            }

            MaxRank = m_Rank.Count == 0 ? 0 : m_Rank.Values.Max();
        }

        void BuildReachability()
        {
            var stack = new Stack<SgNode>();
            foreach (var node in Doc.Nodes)
            {
                if (node.IsBlock)
                {
                    stack.Push(node);
                    m_ReachableFromBlocks.Add(node.ObjectId);
                }
            }
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                foreach (var e in Doc.EdgesIn(node))
                {
                    if (m_ReachableFromBlocks.Add(e.From.ObjectId))
                        stack.Push(e.From);
                }
            }
        }

        /// <summary>Reading order: left to right, then by existing vertical position.</summary>
        public IEnumerable<SgNode> ReadingOrder()
        {
            return Doc.Nodes
                .OrderBy(n => ComponentOf(n))
                .ThenByDescending(n => RankOf(n))
                .ThenBy(n => n.Position.Y)
                .ThenBy(n => n.ObjectId, StringComparer.Ordinal);
        }
    }
}
