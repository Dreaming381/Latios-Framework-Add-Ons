using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ShaderWaitress.Model;

namespace ShaderWaitress.Layout
{
    public sealed class LayoutSettings
    {
        public double ColumnGutter = 96;
        public double RowGap = 44;
        public double ComponentGap = 180;
        public double GroupPadding = 40;
        public double GroupHeader = 48;
        public int RefinementPasses = 3;
        public bool Refine = true;
        /// <summary>
        /// Skip incremental placement and lay out from scratch. A layout that scores worse than the
        /// existing one is still never written.
        /// </summary>
        public bool Force;
        /// <summary>Estimate every node's size instead of trusting sizes the Editor wrote.</summary>
        public bool RemeasureAll;
    }

    /// <summary>
    /// Owns every node, group and context position in the file.
    ///
    /// The graph is reduced to layout units: one per ordinary node, one per labeled group, and
    /// one per vertex or fragment context, since Shader Graph stacks a context's blocks itself.
    /// Units are split into weakly connected components, each component gets a layered
    /// left-to-right layout, groups and contexts are expanded back out, components are stacked,
    /// and a hill-climb on <see cref="LayoutMetrics"/> cleans up what's left.
    /// See docs/ShaderWaitress-DESIGN.md for the full pipeline.
    /// </summary>
    public sealed class LayoutEngine
    {
        const string k_VertexContext = "ctx:vertex";
        const string k_FragmentContext = "ctx:fragment";
        // Measured off a live Shader Graph window.
        const double k_ContextWidth = 224;
        const double k_ContextHeader = 2;
        const double k_BlockRow = NodeSizer.BlockHeight;
        const double k_ContextFooter = 46;
        /// <summary>
        /// A full relayout only replaces incremental placement or a repair when it scores under
        /// half of (their score minus this).
        /// </summary>
        const double k_IncrementalAllowance = 500;
        /// <summary>Above this many nodes in a column, only adjacent pairs are tried.</summary>
        const int k_SwapColumnLimit = 12;
        /// <summary>How many columns either side a node may be relocated into.</summary>
        const int k_ColumnMoveReach = 3;
        /// <summary>How many times the pipeline may re-seed itself from its own result.</summary>
        const int k_SeedRounds = 3;
        /// <summary>Above this many nodes, the searching passes stop trying every option.</summary>
        const int k_ExhaustiveNodeLimit = 60;
        /// <summary>Most nodes a single pass will try to relocate.</summary>
        const int k_ColumnMoveBudget = 12;
        /// <summary>How large a cascade a lane-opening shove may set off before it is abandoned.</summary>
        const int k_ShoveLimit = 8;

        readonly ShaderGraphDocument m_Doc;
        readonly LayoutSettings m_Settings;
        readonly Dictionary<string, Rect> m_Boxes = new Dictionary<string, Rect>(StringComparer.Ordinal);
        readonly Dictionary<string, Rect> m_UnitBoxes = new Dictionary<string, Rect>(StringComparer.Ordinal);
        readonly Dictionary<string, string> m_UnitOfNode = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> m_ContextBlocks = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        public LayoutMetrics Before { get; private set; }
        public LayoutMetrics After { get; private set; }

        /// <summary>True when the file was left alone because its layout was already better.</summary>
        public bool Kept { get; private set; }

        /// <summary>Positions from a previous round, used to seed the ordering of the next.</summary>
        Dictionary<string, Rect> m_Seed;

        /// <summary>
        /// Every refinement move re-measures the whole graph, which costs roughly the square of
        /// its size. Above <see cref="k_ExhaustiveNodeLimit"/> the searching passes still run but
        /// stop trying every option.
        /// </summary>
        bool m_ExhaustiveSearch = true;

        bool m_PortAwareOrdering = true;
        bool m_Incremental;
        bool m_Repaired;

        /// <summary>True when only the new nodes were placed and the rest was left alone.</summary>
        public bool Incremental => m_Incremental;

        /// <summary>True when the existing arrangement was kept and only nudged clear of a wire.</summary>
        public bool Repaired => m_Repaired;

        /// <summary>Things the caller should know about, such as groups that feed each other.</summary>
        public List<string> Notes { get; } = new List<string>();

        // The pipeline runs several times, so the same note can arrive more than once.
        void AddNote(string note)
        {
            if (!Notes.Contains(note))
                Notes.Add(note);
        }

        public LayoutEngine(ShaderGraphDocument doc, LayoutSettings settings = null)
        {
            m_Doc = doc;
            m_Settings = settings ?? new LayoutSettings();
        }

        public void Run()
        {
            if (m_Doc.Nodes.Count == 0)
                return;

            Before = LayoutMetrics.Measure(m_Doc);
            m_ExhaustiveSearch = m_Doc.Nodes.Count <= k_ExhaustiveNodeLimit;
            BuildUnits();

            LayoutMetrics incrementalMetrics = null;
            Dictionary<string, Rect> incrementalBoxes = null;
            if (!m_Settings.Force && TryIncremental())
            {
                incrementalMetrics = LayoutMetrics.Measure(m_Doc, m_Boxes);
                incrementalBoxes = new Dictionary<string, Rect>(m_Boxes, StringComparer.Ordinal);
            }

            // Neither ordering heuristic wins on every graph, so both run in full. Ordering is
            // seeded from the current vertical order, so the hill-climb can get stuck in a local
            // minimum. While something still crosses, the best result seeds another round.
            LayoutMetrics best = null;
            Dictionary<string, Rect> bestBoxes = null;
            var rounds = m_ExhaustiveSearch ? k_SeedRounds : 1;
            for (var round = 0; round < rounds; round++)
            {
                foreach (var portAware in new[] { true, false })
                {
                    m_PortAwareOrdering = portAware;
                    BuildUnits();

                    var placements = new List<Dictionary<string, Rect>>();
                    foreach (var component in Components())
                        placements.Add(LayoutComponent(component));
                    StackComponents(placements);

                    PlaceContexts();
                    ExpandContexts();
                    if (m_Settings.Refine)
                        Refine();

                    var candidate = LayoutMetrics.Measure(m_Doc, m_Boxes);
                    if (best == null || candidate.Score < best.Score)
                    {
                        best = candidate;
                        bestBoxes = new Dictionary<string, Rect>(m_Boxes, StringComparer.Ordinal);
                    }
                }

                if (best.Crossings == 0 && best.WiresOverNodes == 0)
                    break;
                m_Seed = bestBoxes;
            }
            m_Seed = null;

            // Rearranging a graph someone has already read costs more than the score can see, so
            // incremental placement and repair both win unless a full relayout is dramatically
            // better. The allowance is additive as well as proportional because a tidy graph's
            // full layout scores near zero, and a purely proportional test would rearrange
            // everything over one node's worth of extra wire.
            LayoutMetrics repairMetrics = null;
            Dictionary<string, Rect> repairBoxes = null;
            if (CanKeepExisting())
                TryRepairExisting(out repairMetrics, out repairBoxes);

            if (incrementalMetrics != null && incrementalMetrics.Score <= best.Score * 2 + k_IncrementalAllowance)
            {
                m_Incremental = true;
                best = incrementalMetrics;
                bestBoxes = incrementalBoxes;
            }
            else if (repairBoxes != null && repairMetrics.Score <= best.Score * 2 + k_IncrementalAllowance)
            {
                m_Repaired = true;
                best = repairMetrics;
                bestBoxes = repairBoxes;
            }
            // This applies even with --force, which only skips incremental placement.
            else if (CanKeepExisting() && best.Score >= Before.Score)
            {
                Kept = true;
                After = Before;
                return;
            }

            m_Boxes.Clear();
            foreach (var pair in bestBoxes)
                m_Boxes[pair.Key] = pair.Value;
            Commit();
            After = best;
        }

        /// <summary>
        /// Tries to lift wires off nodes in the layout the file already has, leaving every other
        /// position alone. A relayout from scratch can't do this, because the arrangement it would
        /// build is the one already there. Returns false unless a wire was actually lifted.
        /// </summary>
        bool TryRepairExisting(out LayoutMetrics metrics, out Dictionary<string, Rect> boxes)
        {
            metrics = Before;
            boxes = null;
            if (Before.WiresOverNodes == 0)
                return false;
            // Only a layout that's otherwise sound is worth preserving.
            if (Before.NodeOverlaps > 0 || Before.GroupOverlaps > 0 || Before.NodesOutsideGroup > 0)
                return false;

            var saved = new Dictionary<string, Rect>(m_Boxes, StringComparer.Ordinal);
            m_Boxes.Clear();
            foreach (var pair in LayoutMetrics.Rects(m_Doc))
                m_Boxes[pair.Key] = pair.Value;

            var best = LayoutMetrics.Measure(m_Doc, m_Boxes);
            var moved = false;
            for (var pass = 0; pass < m_Settings.RefinementPasses && best.WiresOverNodes > 0; pass++)
            {
                // A wire over a node is often just too long, like a texture property stranded
                // columns away from its sampler. Sliding the source up to its consumer is a
                // smaller move than shoving the node it crosses, so it goes first.
                var sources = best.Obstructions
                    .Select(o => m_Doc.NodeById(o.from))
                    .Where(n => n != null && m_Boxes.ContainsKey(n.ObjectId))
                    .Distinct()
                    .ToList();
                var progress = TryPullTogether(sources, ref best);
                progress |= TryOpenLanes(ref best);
                if (!progress)
                    break;
                moved = true;
            }

            if (moved && best.Score < Before.Score)
            {
                metrics = best;
                boxes = new Dictionary<string, Rect>(m_Boxes, StringComparer.Ordinal);
            }

            m_Boxes.Clear();
            foreach (var pair in saved)
                m_Boxes[pair.Key] = pair.Value;
            return boxes != null;
        }

        /// <summary>
        /// Whether every node has a stored position, which is what makes the existing layout
        /// worth keeping. This checks positions rather than sizes because a property token has a
        /// position from the moment it exists but no size until the Editor first draws it.
        /// </summary>
        bool CanKeepExisting()
        {
            foreach (var node in m_Doc.Nodes)
            {
                if (m_UnitOfNode.TryGetValue(node.ObjectId, out var unit) && IsContext(unit))
                    continue;
                if (!node.HasStoredPosition)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Places only the nodes that have no position yet and leaves everything else alone. Only
        /// applies when the existing layout is sound, and fails unless every new node lands without
        /// an overlap or a backward wire.
        /// </summary>
        bool TryIncremental()
        {
            var placed = new List<SgNode>();
            var missing = new List<SgNode>();
            foreach (var node in m_Doc.Nodes)
            {
                if (m_UnitOfNode.TryGetValue(node.ObjectId, out var unit) && IsContext(unit))
                    continue;
                if (node.HasStoredPosition)
                    placed.Add(node);
                else
                    missing.Add(node);
            }
            if (missing.Count == 0 || placed.Count == 0 || missing.Count > placed.Count)
                return false;

            var boxes = LayoutMetrics.Rects(m_Doc);
            foreach (var node in placed)
                boxes[node.ObjectId] = NodeSizer.Measure(node);

            // Soundness has to be judged on the part of the graph that already has positions;
            // the unplaced nodes are all sitting on top of each other at the origin.
            var unplaced = missing.Select(n => n.ObjectId).ToHashSet(StringComparer.Ordinal);
            var baseline = LayoutMetrics.Measure(m_Doc, boxes, unplaced);
            if (baseline.NodeOverlaps > 0 || baseline.BackwardEdges > 0 || baseline.GroupOverlaps > 0)
                return false;

            // Nearest the consumers first, so a chain of new nodes resolves outward-in.
            foreach (var node in missing.OrderBy(n => m_Doc.EdgesOut(n).Count == 0 ? 1 : 0))
            {
                var (w, h) = NodeSizer.Estimate(node);
                var neighbours = new List<(double x, double y, bool consumer)>();
                foreach (var edge in m_Doc.EdgesOut(node))
                {
                    if (!boxes.TryGetValue(edge.To.ObjectId, out var box) || missing.Contains(edge.To))
                        continue;
                    neighbours.Add((box.X, box.Y + AnchorOf(edge.To, edge.ToPort), true));
                }
                foreach (var edge in m_Doc.EdgesIn(node))
                {
                    if (!boxes.TryGetValue(edge.From.ObjectId, out var box) || missing.Contains(edge.From))
                        continue;
                    neighbours.Add((box.Right, box.Y + AnchorOf(edge.From, edge.FromPort), false));
                }

                double x, y;
                if (neighbours.Any(n => n.consumer))
                {
                    x = neighbours.Where(n => n.consumer).Min(n => n.x) - w - m_Settings.ColumnGutter;
                    y = neighbours.Average(n => n.y) - h * 0.5;
                }
                else if (neighbours.Count > 0)
                {
                    x = neighbours.Max(n => n.x) + m_Settings.ColumnGutter;
                    y = neighbours.Average(n => n.y) - h * 0.5;
                }
                else
                {
                    x = boxes.Values.Min(r => r.X);
                    y = boxes.Values.Max(r => r.Bottom) + m_Settings.RowGap;
                }

                // A node joining a group lands beside the group's other members, not beside its
                // consumer. Unity draws the group box around the whole span, so one stray member
                // leaves a big empty rectangle on screen.
                var home = OwnGroupBox(boxes, node);
                if (home.HasValue)
                {
                    x = Math.Clamp(x, home.Value.X - w - m_Settings.ColumnGutter, home.Value.Right + m_Settings.ColumnGutter);
                    y = Math.Clamp(y, home.Value.Y - h - m_Settings.RowGap, home.Value.Bottom + m_Settings.RowGap);
                }

                // The first free spot often leaves a wire across a node, so try several heights.
                var obstacles = GroupObstacles(boxes, node);
                Rect chosen = default;
                double bestScore = double.MaxValue;
                foreach (var dy in new[] { 0.0, -80, 80, -180, 180, -300, 300 })
                {
                    var candidate = Settle(new Rect(x, y + dy, w, h), boxes, node.ObjectId, obstacles);
                    boxes[node.ObjectId] = candidate;
                    var score = LayoutMetrics.Measure(m_Doc, boxes, unplaced.Where(id => id != node.ObjectId).ToList()).Score;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        chosen = candidate;
                    }
                }
                boxes[node.ObjectId] = chosen;
                unplaced.Remove(node.ObjectId);
            }

            m_Boxes.Clear();
            foreach (var pair in boxes)
                m_Boxes[pair.Key] = pair.Value;

            var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
            return trial.NodeOverlaps == 0 && trial.GroupOverlaps == 0 &&
                   trial.BackwardEdges <= baseline.BackwardEdges &&
                   trial.NodesOutsideGroup <= baseline.NodesOutsideGroup;
        }

        /// <summary>The box the node's own group already occupies, if it has placed members.</summary>
        Rect? OwnGroupBox(Dictionary<string, Rect> boxes, SgNode node)
        {
            if (string.IsNullOrEmpty(node.GroupId))
                return null;
            var members = m_Doc.Nodes
                .Where(n => n.GroupId == node.GroupId && n.ObjectId != node.ObjectId && boxes.ContainsKey(n.ObjectId))
                .Select(n => boxes[n.ObjectId])
                .ToList();
            if (members.Count == 0)
                return null;
            var minX = members.Min(r => r.X);
            var minY = members.Min(r => r.Y);
            return new Rect(minX, minY, members.Max(r => r.Right) - minX, members.Max(r => r.Bottom) - minY);
        }

        /// <summary>The bounding boxes of every group this node does not belong to.</summary>
        List<Rect> GroupObstacles(Dictionary<string, Rect> boxes, SgNode node)
        {
            var obstacles = new List<Rect>();
            foreach (var group in m_Doc.Groups)
            {
                if (group.ObjectId == node.GroupId)
                    continue;
                var members = m_Doc.Nodes
                    .Where(n => n.GroupId == group.ObjectId && boxes.ContainsKey(n.ObjectId))
                    .Select(n => boxes[n.ObjectId])
                    .ToList();
                if (members.Count == 0)
                    continue;
                var minX = members.Min(r => r.X);
                var minY = members.Min(r => r.Y);
                obstacles.Add(new Rect(minX, minY, members.Max(r => r.Right) - minX, members.Max(r => r.Bottom) - minY));
            }
            return obstacles;
        }

        /// <summary>
        /// Slides a box down until it stops overlapping anything already placed, including the
        /// bounding box of a group it is not a member of.
        /// </summary>
        Rect Settle(Rect box, Dictionary<string, Rect> boxes, string self, List<Rect> obstacles)
        {
            for (var attempt = 0; attempt < 400; attempt++)
            {
                var clash = false;
                foreach (var pair in boxes)
                {
                    if (pair.Key == self || !box.Overlaps(pair.Value, LayoutMetrics.Clearance))
                        continue;
                    box = new Rect(box.X, pair.Value.Bottom + m_Settings.RowGap, box.Width, box.Height);
                    clash = true;
                    break;
                }
                if (!clash)
                {
                    foreach (var obstacle in obstacles)
                    {
                        if (!box.Overlaps(obstacle, LayoutMetrics.Clearance))
                            continue;
                        box = new Rect(box.X, obstacle.Bottom + m_Settings.RowGap, box.Width, box.Height);
                        clash = true;
                        break;
                    }
                }
                if (!clash)
                    return box;
            }
            return box;
        }

        // ---------- units ----------

        void BuildUnits()
        {
            m_Boxes.Clear();
            m_UnitBoxes.Clear();
            m_UnitOfNode.Clear();
            m_ContextBlocks.Clear();

            foreach (var vertex in new[] { true, false })
            {
                var key = vertex ? k_VertexContext : k_FragmentContext;
                var blocks = m_Doc.ContextBlocks(vertex).Where(id => m_Doc.NodeById(id) != null).ToList();
                if (blocks.Count == 0)
                    continue;
                m_ContextBlocks[key] = blocks;
                foreach (var id in blocks)
                    m_UnitOfNode[id] = key;
                m_UnitBoxes[key] = new Rect(0, 0, k_ContextWidth + ContextLiteral(key),
                    k_ContextHeader + blocks.Count * k_BlockRow + k_ContextFooter);
            }

            foreach (var node in m_Doc.Nodes)
            {
                if (m_UnitOfNode.ContainsKey(node.ObjectId))
                    continue;
                // A block that is not listed in any context still has to go somewhere.
                // A size the Editor measured is exact; only fall back to the estimate without one.
                var stored = node.Position;
                double w, h;
                if (!m_Settings.RemeasureAll && stored.Width > 1 && stored.Height > 1)
                    (w, h) = (stored.Width, stored.Height);
                else
                    (w, h) = NodeSizer.Estimate(node);
                m_UnitOfNode[node.ObjectId] = node.ObjectId;
                m_UnitBoxes[node.ObjectId] = new Rect(0, 0, w, h);
                m_Boxes[node.ObjectId] = new Rect(0, 0, w, h);
            }
        }

        string UnitOf(SgNode node) => m_UnitOfNode.TryGetValue(node.ObjectId, out var unit) ? unit : node.ObjectId;

        bool IsContext(string unit) => m_ContextBlocks.ContainsKey(unit);

        /// <summary>A subgraph has no block contexts; its output node is the sink instead.</summary>
        bool IsGraphOutput(string unit)
        {
            var node = m_Doc.NodeById(unit);
            if (node == null)
                return false;
            if (node.TypeName == "UnityEditor.ShaderGraph.SubGraphOutputNode")
                return true;
            return unit == ShaderWaitress.Serialization.UnityJsonWriter.RefId(m_Doc.RootNode["m_OutputNode"]);
        }

        /// <summary>Existing vertical position, used only to seed the initial ordering.</summary>
        double SeedOf(string unit)
        {
            if (!IsContext(unit))
            {
                if (m_Seed != null && m_Seed.TryGetValue(unit, out var seeded))
                    return seeded.Y;
                return m_Doc.NodeById(unit)?.Position.Y ?? 0;
            }
            var field = unit == k_VertexContext ? "m_VertexContext" : "m_FragmentContext";
            return ShaderWaitress.Serialization.Json.Number(m_Doc.RootNode[field]?["m_Position"]?["y"], 0);
        }

        /// <summary>Vertical offset of the port an edge attaches to, inside its unit.</summary>
        double AnchorOf(SgNode node, SgPort port)
        {
            var unit = UnitOf(node);
            if (IsContext(unit))
            {
                var index = m_ContextBlocks[unit].IndexOf(node.ObjectId);
                return k_ContextHeader + Math.Max(0, index) * k_BlockRow + NodeSizer.BlockHeight * 0.5;
            }
            var box = m_UnitBoxes.TryGetValue(unit, out var r) ? r : NodeSizer.Measure(node);
            return port == null ? box.Height * 0.5 : NodeSizer.PortAnchor(node, port, box.Height);
        }

        IEnumerable<(string from, string to, double fromAnchor, double toAnchor)> UnitEdges()
        {
            foreach (var edge in m_Doc.Edges)
            {
                var from = UnitOf(edge.From);
                var to = UnitOf(edge.To);
                if (from == to)
                    continue;
                yield return (from, to, AnchorOf(edge.From, edge.FromPort), AnchorOf(edge.To, edge.ToPort));
            }
        }

        List<List<string>> Components()
        {
            var adjacency = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var unit in m_UnitBoxes.Keys)
                adjacency[unit] = new List<string>();
            foreach (var (from, to, _, _) in UnitEdges())
            {
                adjacency[from].Add(to);
                adjacency[to].Add(from);
            }

            // Group members belong together even when no wire connects them. Otherwise a group
            // split across two components gets laid out twice, and its box spans the gap and
            // swallows whatever sits between.
            var byGroup = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var node in m_Doc.Nodes)
            {
                var groupId = node.GroupId;
                if (string.IsNullOrEmpty(groupId) || m_Doc.GroupById(groupId) == null)
                    continue;
                var unit = UnitOf(node);
                if (IsContext(unit) || !adjacency.ContainsKey(unit))
                    continue;
                if (!byGroup.TryGetValue(groupId, out var members))
                {
                    members = new List<string>();
                    byGroup[groupId] = members;
                }
                members.Add(unit);
            }
            foreach (var members in byGroup.Values)
            {
                for (var i = 1; i < members.Count; i++)
                {
                    adjacency[members[0]].Add(members[i]);
                    adjacency[members[i]].Add(members[0]);
                }
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);
            var components = new List<List<string>>();
            foreach (var unit in m_UnitBoxes.Keys)
            {
                if (!seen.Add(unit))
                    continue;
                var bucket = new List<string>();
                var stack = new Stack<string>();
                stack.Push(unit);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    bucket.Add(current);
                    foreach (var next in adjacency[current])
                    {
                        if (seen.Add(next))
                            stack.Push(next);
                    }
                }
                components.Add(bucket);
            }

            // The component that reaches the outputs goes first.
            return components
                .OrderByDescending(c => c.Any(IsContext))
                .ThenByDescending(c => c.Count)
                .ThenBy(c => c[0], StringComparer.Ordinal)
                .ToList();
        }

        // ---------- one component ----------

        Dictionary<string, Rect> LayoutComponent(List<string> component)
        {
            var inComponent = new HashSet<string>(component, StringComparer.Ordinal);

            var groupOf = new Dictionary<string, string>(StringComparer.Ordinal);      // unit -> group
            var groupMembers = new Dictionary<string, List<SgNode>>(StringComparer.Ordinal);
            foreach (var node in m_Doc.Nodes)
            {
                var unit = UnitOf(node);
                if (IsContext(unit) || !inComponent.Contains(unit))
                    continue;
                var groupId = node.GroupId;
                if (string.IsNullOrEmpty(groupId) || m_Doc.GroupById(groupId) == null)
                    continue;
                groupOf[unit] = groupId;
                if (!groupMembers.TryGetValue(groupId, out var list))
                {
                    list = new List<SgNode>();
                    groupMembers[groupId] = list;
                }
                list.Add(node);
            }

            var groupLayout = new Dictionary<string, Dictionary<string, Rect>>(StringComparer.Ordinal);
            foreach (var pair in groupMembers)
                groupLayout[pair.Key] = LayoutGroupInterior(pair.Value);

            var items = new List<LayoutItem>();
            var index = new Dictionary<string, int>(StringComparer.Ordinal);

            foreach (var unit in component)
            {
                if (groupOf.ContainsKey(unit))
                    continue;
                var box = m_UnitBoxes[unit];
                index[unit] = items.Count;
                items.Add(new LayoutItem
                {
                    Key = unit, Width = box.Width, Height = box.Height,
                    IsSink = IsContext(unit) || IsGraphOutput(unit), Seed = SeedOf(unit),
                });
            }
            foreach (var pair in groupLayout)
            {
                var w = pair.Value.Count == 0 ? 0 : pair.Value.Values.Max(r => r.Right);
                var h = pair.Value.Count == 0 ? 0 : pair.Value.Values.Max(r => r.Bottom);
                var key = "group:" + pair.Key;
                index[key] = items.Count;
                items.Add(new LayoutItem
                {
                    Key = key,
                    Width = w + m_Settings.GroupPadding * 2,
                    Height = h + m_Settings.GroupPadding + m_Settings.GroupHeader,
                    Seed = groupMembers[pair.Key].Min(n => SeedOf(n.ObjectId)),
                });
            }

            int Coarse(string unit) => index[groupOf.TryGetValue(unit, out var g) ? "group:" + g : unit];

            var links = new List<LayoutLink>();
            foreach (var (from, to, fromAnchor, toAnchor) in UnitEdges())
            {
                if (!inComponent.Contains(from) || !inComponent.Contains(to))
                    continue;
                var a = Coarse(from);
                var b = Coarse(to);
                if (a == b)
                    continue;
                links.Add(new LayoutLink { From = a, To = b, FromAnchor = fromAnchor, ToAnchor = toAnchor });
            }

            ReportGroupCycles(items, links);

            var layout = new LayeredLayout(items, links, new LayoutOptions
            {
                ColumnGutter = m_Settings.ColumnGutter,
                RowGap = m_Settings.RowGap,
                PortAwareOrdering = m_PortAwareOrdering,
            });
            layout.Run();

            var result = new Dictionary<string, Rect>(StringComparer.Ordinal);
            foreach (var item in layout.Items)
            {
                if (item.Key == null)
                    continue;
                if (item.Key.StartsWith("group:", StringComparison.Ordinal))
                {
                    var groupId = item.Key.Substring("group:".Length);
                    foreach (var pair in groupLayout[groupId])
                    {
                        result[pair.Key] = new Rect(
                            item.X + m_Settings.GroupPadding + pair.Value.X,
                            item.Y + m_Settings.GroupHeader + pair.Value.Y,
                            pair.Value.Width, pair.Value.Height);
                    }
                    continue;
                }
                result[item.Key] = new Rect(item.X, item.Y, item.Width, item.Height);
            }
            return result;
        }

        /// <summary>
        /// Titles of groups that sit in a cycle with each other. A group is laid out as one unit,
        /// so two groups that feed each other can't both be left of the other, and some wire has
        /// to run backward. It's reported rather than fixed, because regrouping is the caller's
        /// call.
        /// </summary>
        public static IEnumerable<string> MutuallyFeedingGroups(ShaderGraphDocument doc)
        {
            // One vertex per unit: a group, or a single ungrouped node.
            string UnitOf(SgNode node) =>
                string.IsNullOrEmpty(node.GroupId) ? "n:" + node.ObjectId : "g:" + node.GroupId;

            var successors = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            var indegree = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var node in doc.Nodes)
            {
                var unit = UnitOf(node);
                successors.TryAdd(unit, new HashSet<string>(StringComparer.Ordinal));
                indegree.TryAdd(unit, 0);
            }
            foreach (var edge in doc.Edges)
            {
                var from = UnitOf(edge.From);
                var to = UnitOf(edge.To);
                if (from == to || !successors.TryGetValue(from, out var set) || !set.Add(to))
                    continue;
                indegree[to] = indegree.GetValueOrDefault(to) + 1;
            }

            var ready = new Stack<string>(indegree.Where(p => p.Value == 0).Select(p => p.Key));
            var drained = new HashSet<string>(StringComparer.Ordinal);
            while (ready.Count > 0)
            {
                var unit = ready.Pop();
                if (!drained.Add(unit))
                    continue;
                foreach (var next in successors[unit])
                {
                    if (--indegree[next] == 0)
                        ready.Push(next);
                }
            }

            foreach (var unit in successors.Keys)
            {
                if (drained.Contains(unit) || !unit.StartsWith("g:", StringComparison.Ordinal))
                    continue;
                var group = doc.GroupById(unit.Substring(2));
                if (group != null)
                    yield return '"' + group.Title + '"';
            }
        }

        /// <summary>Adds a note naming groups that feed each other. See <see cref="MutuallyFeedingGroups"/>.</summary>
        void ReportGroupCycles(List<LayoutItem> items, List<LayoutLink> links)
        {
            var indegree = new int[items.Count];
            foreach (var link in links)
            {
                if (link.From != link.To)
                    indegree[link.To]++;
            }
            var ready = new Stack<int>(Enumerable.Range(0, items.Count).Where(i => indegree[i] == 0));
            var drained = new bool[items.Count];
            while (ready.Count > 0)
            {
                var i = ready.Pop();
                if (drained[i])
                    continue;
                drained[i] = true;
                foreach (var link in links)
                {
                    if (link.From != i || link.From == link.To)
                        continue;
                    if (--indegree[link.To] == 0)
                        ready.Push(link.To);
                }
            }

            var stuckGroups = new List<string>();
            for (var i = 0; i < items.Count; i++)
            {
                if (drained[i] || items[i].Key == null || !items[i].Key.StartsWith("group:", StringComparison.Ordinal))
                    continue;
                var group = m_Doc.GroupById(items[i].Key.Substring("group:".Length));
                if (group != null)
                    stuckGroups.Add('"' + group.Title + '"');
            }
            if (stuckGroups.Count > 1)
            {
                AddNote($"these groups feed each other, so some wires have to run backwards: {string.Join(", ", stuckGroups)}. " +
                          "Splitting or merging them is the only way to get a clean left-to-right flow.");
            }
        }

        Dictionary<string, Rect> LayoutGroupInterior(List<SgNode> members)
        {
            var items = new List<LayoutItem>();
            var index = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var node in members)
            {
                var box = m_UnitBoxes[node.ObjectId];
                index[node.ObjectId] = items.Count;
                items.Add(new LayoutItem { Key = node.ObjectId, Width = box.Width, Height = box.Height, Seed = SeedOf(node.ObjectId) });
            }

            var links = new List<LayoutLink>();
            foreach (var edge in m_Doc.Edges)
            {
                if (!index.TryGetValue(edge.From.ObjectId, out var from) || !index.TryGetValue(edge.To.ObjectId, out var to))
                    continue;
                if (from == to)
                    continue;
                links.Add(new LayoutLink
                {
                    From = from,
                    To = to,
                    FromAnchor = AnchorOf(edge.From, edge.FromPort),
                    ToAnchor = AnchorOf(edge.To, edge.ToPort),
                });
            }

            var layout = new LayeredLayout(items, links, new LayoutOptions
            {
                ColumnGutter = m_Settings.ColumnGutter,
                RowGap = m_Settings.RowGap,
                PortAwareOrdering = m_PortAwareOrdering,
            });
            layout.Run();

            var result = new Dictionary<string, Rect>(StringComparer.Ordinal);
            foreach (var item in layout.Items)
            {
                if (item.Key != null)
                    result[item.Key] = new Rect(item.X, item.Y, item.Width, item.Height);
            }
            return result;
        }

        void StackComponents(List<Dictionary<string, Rect>> placements)
        {
            var y = 0.0;
            // Keep the vertical order the components already have. Otherwise they stack in
            // object-id order, and a new node can land in the middle and push the rest down.
            foreach (var placement in placements.OrderBy(ExistingTop).ToList())
            {
                if (placement.Count == 0)
                    continue;
                var minX = placement.Values.Min(r => r.X);
                var minY = placement.Values.Min(r => r.Y);
                var height = placement.Values.Max(r => r.Bottom) - minY;
                foreach (var pair in placement)
                {
                    var r = pair.Value;
                    var box = new Rect(r.X - minX, r.Y - minY + y, r.Width, r.Height);
                    m_UnitBoxes[pair.Key] = box;
                    if (!IsContext(pair.Key))
                        m_Boxes[pair.Key] = box;
                }
                y += height + m_Settings.ComponentGap;
            }
        }

        /// <summary>
        /// Where a component's already-placed members sit today. A component of entirely new
        /// nodes has nothing to go on and sorts last.
        /// </summary>
        double ExistingTop(Dictionary<string, Rect> placement)
        {
            var top = double.MaxValue;
            foreach (var key in placement.Keys)
            {
                foreach (var id in NodesOfUnit(key))
                {
                    var node = m_Doc.NodeById(id);
                    if (node != null && node.HasStoredPosition)
                        top = Math.Min(top, node.Position.Y);
                }
            }
            return top;
        }

        IEnumerable<string> NodesOfUnit(string unit)
        {
            if (m_ContextBlocks.TryGetValue(unit, out var blocks))
                return blocks;
            return m_UnitOfNode.Where(p => p.Value == unit).Select(p => p.Key);
        }

        /// <summary>
        /// Moves the contexts to the right edge, with Vertex above Fragment, which is where Shader
        /// Graph puts them. Otherwise a context nothing feeds would be its own component, stacked
        /// far below the graph.
        /// </summary>
        void PlaceContexts()
        {
            if (m_ContextBlocks.Count == 0)
                return;
            var nodeBoxes = m_Boxes.Values.ToList();
            var anchorX = nodeBoxes.Count == 0 ? 0 : nodeBoxes.Max(r => r.Right) + m_Settings.ColumnGutter;

            // Line the contexts up by where they draw, not where their units start. A unit
            // reaches left of its context by the width of its blocks' value editors, which
            // differs between the two stacks whenever one is wired and the other isn't.
            var keys = m_ContextBlocks.Keys.ToList();
            var drawnX = Math.Max(anchorX + keys.Max(ContextLiteral),
                                  keys.Max(k => m_UnitBoxes[k].X + ContextLiteral(k)));
            foreach (var key in keys)
            {
                var box = m_UnitBoxes[key];
                m_UnitBoxes[key] = new Rect(drawnX - ContextLiteral(key), box.Y, box.Width, box.Height);
            }

            if (!m_UnitBoxes.TryGetValue(k_FragmentContext, out var fragment))
                return;
            if (!m_UnitBoxes.TryGetValue(k_VertexContext, out var vertex))
                return;
            m_UnitBoxes[k_VertexContext] = new Rect(vertex.X, fragment.Y - vertex.Height - k_ContextHeader, vertex.Width, vertex.Height);
        }

        /// <summary>
        /// Space to leave clear on the left of a context for its blocks' value editors, which
        /// Shader Graph draws outside the blocks themselves.
        /// </summary>
        double ContextLiteral(string key)
        {
            if (!m_ContextBlocks.TryGetValue(key, out var blocks))
                return 0;
            var reserved = 0.0;
            foreach (var id in blocks)
            {
                var block = m_Doc.NodeById(id);
                if (block != null)
                    reserved = Math.Max(reserved, NodeSizer.LiteralWidth(m_Doc, block));
            }
            return reserved;
        }

        /// <summary>Turns each context's box into the individual block rects it renders as.</summary>
        void ExpandContexts()
        {
            foreach (var pair in m_ContextBlocks)
            {
                if (!m_UnitBoxes.TryGetValue(pair.Key, out var box))
                    continue;
                var reserved = ContextLiteral(pair.Key);
                for (var i = 0; i < pair.Value.Count; i++)
                {
                    var block = m_Doc.NodeById(pair.Value[i]);
                    var literal = NodeSizer.LiteralWidth(m_Doc, block);
                    m_Boxes[pair.Value[i]] = new Rect(
                        box.X + reserved - literal, box.Y + k_ContextHeader + i * k_BlockRow,
                        NodeSizer.BlockWidth + literal, NodeSizer.BlockHeight);
                }
            }
        }

        // ---------- refinement ----------

        /// <summary>
        /// Hill-climbs on the real geometry: column re-sorts, swaps, column moves, lane opening,
        /// and small vertical nudges, keeping each move that lowers the score without breaking a
        /// hard rule. Crossings on the real geometry can only be measured once coordinates exist,
        /// which is why this runs last.
        /// </summary>
        void Refine()
        {
            var best = LayoutMetrics.Measure(m_Doc, m_Boxes);
            if (best.Score <= 0)
                return;

            var offsets = new[] { -m_Settings.RowGap, -m_Settings.RowGap * 0.5, m_Settings.RowGap * 0.5, m_Settings.RowGap };
            var movable = m_Doc.Nodes.Where(n => !m_ContextBlocks.Values.Any(b => b.Contains(n.ObjectId))).ToList();

            for (var pass = 0; pass < m_Settings.RefinementPasses; pass++)
            {
                var improved = TrySortColumns(movable, ref best);
                improved |= TrySwaps(movable, ref best);
                improved |= TryColumnMoves(movable, ref best);
                improved |= TryOpenLanes(ref best);
                foreach (var node in movable)
                {
                    if (!m_Boxes.TryGetValue(node.ObjectId, out var original))
                        continue;
                    var accepted = false;
                    // Small nudges can't uncross two wires into one node when their producers
                    // are whole rows apart. The shift that levels this node's wires can.
                    foreach (var dy in offsets.Concat(StraighteningShifts(node)))
                    {
                        m_Boxes[node.ObjectId] = new Rect(original.X, original.Y + dy, original.Width, original.Height);
                        var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
                        if (Accepts(trial, best))
                        {
                            best = trial;
                            improved = true;
                            accepted = true;
                            break;
                        }
                    }
                    if (!accepted)
                        m_Boxes[node.ObjectId] = original;
                }
                if (!improved)
                    break;
            }

            // Last, because it breaks the column alignment every pass above relies on.
            TryPullTogether(movable, ref best);
        }

        /// <summary>
        /// Re-sorts each column to match the real heights of what its nodes connect to.
        ///
        /// The ordering pass treats a whole context as one vertex, so it can't see that NormalTS
        /// sits above Metallic in the block stack. The producers of those blocks end up in an
        /// arbitrary order, and their wires cross all the way down the stack. Pairwise swaps
        /// can't fix that either, since the fix is a whole permutation.
        /// </summary>
        bool TrySortColumns(List<SgNode> movable, ref LayoutMetrics best)
        {
            var improved = false;
            foreach (var column in Columns(movable))
            {
                var ordered = column.OrderBy(n => m_Boxes[n.ObjectId].Y).ToList();
                if (ordered.Count < 2 || !SlotsOf(ordered, out var top, out var gaps))
                    continue;

                foreach (var downstream in new[] { true, false })
                {
                    var rects = LayoutMetrics.Rects(m_Doc, m_Boxes);
                    var key = ordered.ToDictionary(n => n.ObjectId, n => Barycenter(n, downstream, rects));
                    var candidate = ordered
                        .Select((n, i) => (node: n, i))
                        .OrderBy(p => key[p.node.ObjectId] ?? double.MaxValue)
                        .ThenBy(p => p.i)
                        .Select(p => p.node)
                        .ToList();
                    if (candidate.SequenceEqual(ordered))
                        continue;

                    Restack(candidate, top, gaps);
                    var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
                    if (Accepts(trial, best))
                    {
                        best = trial;
                        improved = true;
                        ordered = candidate;
                    }
                    else
                    {
                        Restack(ordered, top, gaps);
                    }
                }
            }
            return improved;
        }

        /// <summary>
        /// Mean y of the ports this node's wires attach to on the far end. Null when it has none
        /// in that direction, which sorts the node last rather than to the top.
        /// </summary>
        double? Barycenter(SgNode node, bool downstream, Dictionary<string, Rect> rects)
        {
            var total = 0.0;
            var count = 0;
            foreach (var edge in downstream ? m_Doc.EdgesOut(node) : m_Doc.EdgesIn(node))
            {
                var other = downstream ? edge.To : edge.From;
                var port = downstream ? edge.ToPort : edge.FromPort;
                if (!rects.TryGetValue(other.ObjectId, out var box))
                    continue;
                total += box.Y + (port != null ? NodeSizer.PortAnchor(other, port, box.Height) : box.Height * 0.5);
                count++;
            }
            return count == 0 ? null : total / count;
        }

        /// <summary>
        /// Groups nodes into columns by their right edge. Layers are right-aligned, so a narrow
        /// property and a wide preview node in one column can start hundreds of pixels apart.
        /// </summary>
        IEnumerable<IGrouping<double, SgNode>> Columns(List<SgNode> movable) =>
            movable.Where(n => m_Boxes.ContainsKey(n.ObjectId))
                   .GroupBy(n => Math.Round(m_Boxes[n.ObjectId].Right / 8));

        /// <summary>The column's top and the gap after each node, so a reorder keeps its extent.</summary>
        bool SlotsOf(List<SgNode> ordered, out double top, out double[] gaps)
        {
            top = m_Boxes[ordered[0].ObjectId].Y;
            gaps = new double[ordered.Count - 1];
            for (var k = 0; k + 1 < ordered.Count; k++)
                gaps[k] = m_Boxes[ordered[k + 1].ObjectId].Y - m_Boxes[ordered[k].ObjectId].Bottom;
            return gaps.All(g => g >= 0);
        }

        void Restack(List<SgNode> order, double top, double[] gaps)
        {
            var y = top;
            for (var k = 0; k < order.Count; k++)
            {
                var box = m_Boxes[order[k].ObjectId];
                m_Boxes[order[k].ObjectId] = new Rect(box.X, y, box.Width, box.Height);
                y += box.Height + (k < gaps.Length ? gaps[k] : 0);
            }
        }

        /// <summary>
        /// Opens a lane for a wire drawn across a node, by shoving the node clear and pushing
        /// whatever it runs into along with it.
        ///
        /// Dummy items normally reserve a lane for a long wire, but contexts move to the right
        /// edge after vertical placement, so the reserved lane can end up in the wrong place.
        /// This fixes it on the final geometry.
        /// </summary>
        bool TryOpenLanes(ref LayoutMetrics best)
        {
            var improved = false;
            foreach (var (id, _, bandTop, bandBottom) in best.Obstructions.ToList())
            {
                var node = m_Doc.NodeById(id);
                if (node == null || !m_Boxes.TryGetValue(id, out var box))
                    continue;
                if (m_ContextBlocks.Values.Any(b => b.Contains(id)))
                    continue;

                // Try the shorter side first, but either can win depending on what the shove drags along.
                var up = bandTop - LayoutMetrics.Clearance - box.Bottom;
                var down = bandBottom + LayoutMetrics.Clearance - box.Y;
                var order = box.CenterY <= (bandTop + bandBottom) * 0.5
                    ? new[] { up, down }
                    : new[] { down, up };

                foreach (var shift in order)
                {
                    if (Math.Abs(shift) < 1)
                        continue;

                    var before = new Dictionary<string, Rect>(m_Boxes, StringComparer.Ordinal);
                    if (Shove(node, shift))
                    {
                        var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
                        if (Accepts(trial, best))
                        {
                            best = trial;
                            improved = true;
                            break;
                        }
                    }

                    m_Boxes.Clear();
                    foreach (var pair in before)
                        m_Boxes[pair.Key] = pair.Value;
                }
            }
            return improved;
        }

        /// <summary>
        /// The node plus every source that exists only to feed it, like a sampler and its texture
        /// property. These move together. Moved alone, the sampler gets pinned in place by the
        /// very property it should take along.
        /// </summary>
        List<SgNode> ExclusiveCluster(SgNode node)
        {
            var cluster = new List<SgNode> { node };
            foreach (var edge in m_Doc.EdgesIn(node))
            {
                var source = edge.From;
                if (m_Doc.EdgesIn(source).Count == 0 &&
                    m_Doc.EdgesOut(source).All(e => e.To.Is(node)) &&
                    m_Boxes.ContainsKey(source.ObjectId) &&
                    !cluster.Any(n => n.Is(source)))
                {
                    cluster.Add(source);
                }
            }
            return cluster;
        }

        /// <summary>
        /// Moves a node by the given amount and carries along anything it would land on,
        /// repeatedly. Returns false if the cascade grows past what is worth trying.
        /// </summary>
        bool Shove(SgNode node, double shift)
        {
            var moved = new HashSet<string>(StringComparer.Ordinal);
            var queue = new Queue<SgNode>();
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!moved.Add(current.ObjectId) || !m_Boxes.TryGetValue(current.ObjectId, out var box))
                    continue;
                if (moved.Count > k_ShoveLimit)
                    return false;

                var placed = new Rect(box.X, box.Y + shift, box.Width, box.Height);
                m_Boxes[current.ObjectId] = placed;

                // Every node the shove touches takes its own exclusive feeders with it.
                foreach (var member in ExclusiveCluster(current))
                    queue.Enqueue(member);

                foreach (var other in m_Doc.Nodes)
                {
                    if (moved.Contains(other.ObjectId) || other.IsBlock)
                        continue;
                    if (m_ContextBlocks.Values.Any(b => b.Contains(other.ObjectId)))
                        continue;
                    if (m_Boxes.TryGetValue(other.ObjectId, out var neighbour) &&
                        placed.Overlaps(neighbour, LayoutMetrics.Clearance))
                        queue.Enqueue(other);
                }
            }
            return true;
        }

        /// <summary>
        /// Slides a node as far right as its consumers allow.
        ///
        /// Ranking puts a node one column left of the deepest thing it feeds. That's right for a
        /// chain, but it can strand a property columns away from its only consumer, with a long
        /// wire across everything between. Each node gets one candidate: up against its nearest
        /// consumer, minus the gutter.
        /// </summary>
        bool TryPullTogether(List<SgNode> movable, ref LayoutMetrics best)
        {
            var improved = false;
            foreach (var node in movable)
            {
                if (!m_Boxes.TryGetValue(node.ObjectId, out var original))
                    continue;

                // Block producers stay put. Their order has to match the block stack, and that
                // only holds while they share a column.
                if (m_Doc.EdgesOut(node).Any(e => e.To.IsBlock))
                    continue;

                var limit = double.PositiveInfinity;
                foreach (var edge in m_Doc.EdgesOut(node))
                {
                    if (m_Boxes.TryGetValue(edge.To.ObjectId, out var consumer))
                        limit = Math.Min(limit, consumer.X - m_Settings.ColumnGutter);
                }
                if (double.IsPositiveInfinity(limit))
                    continue;

                var x = limit - original.Width;
                if (x <= original.X + 1)
                    continue;

                var moved = Settle(new Rect(x, original.Y, original.Width, original.Height),
                                   m_Boxes, node.ObjectId, ForeignGroupBoxes(node));
                m_Boxes[node.ObjectId] = moved;
                var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
                if (Accepts(trial, best))
                {
                    best = trial;
                    improved = true;
                }
                else
                {
                    m_Boxes[node.ObjectId] = original;
                }
            }
            return improved;
        }

        /// <summary>
        /// Moves a node into a different column when that reads better. A node sitting in a long
        /// wire's lane gets that wire drawn across it, and moving it a column over clears the
        /// lane. A source like a property or a UV node can untangle a fan-in by moving to its
        /// own column.
        ///
        /// A move is only tried where the node stays right of everything feeding it and left of
        /// everything it feeds.
        /// </summary>
        bool TryColumnMoves(List<SgNode> movable, ref LayoutMetrics best)
        {
            var improved = false;
            var rights = Columns(movable).Select(c => c.Key * 8).OrderBy(x => x).ToList();
            if (rights.Count < 2)
                return false;

            // Only try nodes involved in a defect, and only nearby columns. Every try is a full
            // re-measure, and trying everything takes minutes on a large graph.
            var candidates = best.Implicated;
            var budget = k_ColumnMoveBudget;
            foreach (var node in movable)
            {
                if (budget <= 0)
                    break;
                if (!candidates.Contains(node.ObjectId))
                    continue;
                budget--;
                if (!m_Boxes.TryGetValue(node.ObjectId, out var original))
                    continue;
                var home = rights.FindIndex(r => Math.Abs(r - original.Right) < 1);

                var cluster = ExclusiveCluster(node);

                // The range the cluster can move in without turning a wire backward. Wires
                // between its own members move with it.
                var leftLimit = double.NegativeInfinity;
                var rightLimit = double.PositiveInfinity;
                foreach (var member in cluster)
                {
                    var offset = m_Boxes[member.ObjectId].X - original.X;
                    foreach (var edge in m_Doc.EdgesIn(member))
                    {
                        if (cluster.Any(n => n.Is(edge.From)))
                            continue;
                        if (m_Boxes.TryGetValue(edge.From.ObjectId, out var producer))
                            leftLimit = Math.Max(leftLimit, producer.Right + m_Settings.ColumnGutter - offset);
                    }
                    foreach (var edge in m_Doc.EdgesOut(member))
                    {
                        if (cluster.Any(n => n.Is(edge.To)))
                            continue;
                        if (m_Boxes.TryGetValue(edge.To.ObjectId, out var consumer))
                        {
                            var width = m_Boxes[member.ObjectId].Width;
                            rightLimit = Math.Min(rightLimit,
                                consumer.X - m_Settings.ColumnGutter - offset - width + original.Width);
                        }
                    }
                }

                var obstacles = ForeignGroupBoxes(node);
                for (var c = 0; c < rights.Count; c++)
                {
                    if (home >= 0 && Math.Abs(c - home) > k_ColumnMoveReach)
                        continue;
                    var right = rights[c];
                    var x = right - original.Width;
                    if (Math.Abs(x - original.X) < 1 || x < leftLimit || right > rightLimit)
                        continue;

                    var before = cluster.ToDictionary(n => n.ObjectId, n => m_Boxes[n.ObjectId]);
                    var shift = x - original.X;
                    foreach (var member in cluster)
                    {
                        var box = before[member.ObjectId];
                        var placed = new Rect(box.X + shift, box.Y, box.Width, box.Height);
                        m_Boxes[member.ObjectId] = Settle(placed, m_Boxes, member.ObjectId, obstacles);
                    }

                    var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
                    if (Accepts(trial, best))
                    {
                        best = trial;
                        improved = true;
                        original = m_Boxes[node.ObjectId];
                    }
                    else
                    {
                        foreach (var pair in before)
                            m_Boxes[pair.Key] = pair.Value;
                    }
                }
            }
            return improved;
        }

        /// <summary>
        /// How far this node would have to move for its wires to run level, averaged once over
        /// what it feeds and once over what feeds it. Shifts under a pixel are dropped.
        /// </summary>
        IEnumerable<double> StraighteningShifts(SgNode node)
        {
            var rects = LayoutMetrics.Rects(m_Doc, m_Boxes);
            if (!rects.TryGetValue(node.ObjectId, out var self))
                yield break;

            foreach (var downstream in new[] { true, false })
            {
                var total = 0.0;
                var count = 0;
                foreach (var edge in downstream ? m_Doc.EdgesOut(node) : m_Doc.EdgesIn(node))
                {
                    var other = downstream ? edge.To : edge.From;
                    var otherPort = downstream ? edge.ToPort : edge.FromPort;
                    var ownPort = downstream ? edge.FromPort : edge.ToPort;
                    if (!rects.TryGetValue(other.ObjectId, out var box))
                        continue;
                    var otherY = box.Y + (otherPort != null
                        ? NodeSizer.PortAnchor(other, otherPort, box.Height)
                        : box.Height * 0.5);
                    var ownY = self.Y + (ownPort != null
                        ? NodeSizer.PortAnchor(node, ownPort, self.Height)
                        : self.Height * 0.5);
                    total += otherY - ownY;
                    count++;
                }
                if (count == 0)
                    continue;
                var shift = total / count;
                if (Math.Abs(shift) > 1)
                    yield return shift;
            }
        }

        /// <summary>
        /// Whether a trial arrangement is worth keeping. A lower score isn't enough. Node overlaps,
        /// group overlaps, foreign nodes inside groups, and crowding are hard rules, and a move
        /// that breaks one is rejected however much the total improves.
        /// </summary>
        static bool Accepts(LayoutMetrics trial, LayoutMetrics best) =>
            trial.Score < best.Score - 1e-6 &&
            trial.NodeOverlaps <= best.NodeOverlaps &&
            trial.GroupOverlaps <= best.GroupOverlaps &&
            trial.NodesOutsideGroup <= best.NodesOutsideGroup &&
            trial.Crowded <= best.Crowded;

        /// <summary>Bounding boxes of the groups this node is not a member of.</summary>
        List<Rect> ForeignGroupBoxes(SgNode node)
        {
            var boxes = new List<Rect>();
            foreach (var group in m_Doc.Groups)
            {
                if (group.ObjectId == node.GroupId)
                    continue;
                var members = m_Doc.Nodes
                    .Where(n => n.GroupId == group.ObjectId && m_Boxes.ContainsKey(n.ObjectId))
                    .Select(n => m_Boxes[n.ObjectId])
                    .ToList();
                if (members.Count == 0)
                    continue;
                boxes.Add(new Rect(members.Min(r => r.X), members.Min(r => r.Y),
                                   members.Max(r => r.Right) - members.Min(r => r.X),
                                   members.Max(r => r.Bottom) - members.Min(r => r.Y)));
            }
            return boxes;
        }

        /// <summary>
        /// Swaps nodes within a column. The ordering pass only approximates the crossings a
        /// reader sees, so this checks the real geometry and keeps the swaps that help.
        /// </summary>
        bool TrySwaps(List<SgNode> movable, ref LayoutMetrics best)
        {
            var improved = false;
            var columns = Columns(movable).ToList();

            foreach (var column in columns)
            {
                var ordered = column.OrderBy(n => m_Boxes[n.ObjectId].Y).ToList();
                if (ordered.Count < 2)
                    continue;
                if (!SlotsOf(ordered, out var top, out var gaps))
                    continue;

                // Adjacent swaps can't move a node several places, since each step has to pay
                // for itself. So small columns try every pair.
                var exhaustive = m_ExhaustiveSearch && ordered.Count <= k_SwapColumnLimit;
                for (var i = 0; i < ordered.Count - 1; i++)
                {
                    var last = exhaustive ? ordered.Count - 1 : Math.Min(i + 1, ordered.Count - 1);
                    for (var j = i + 1; j <= last; j++)
                    {
                        var candidate = new List<SgNode>(ordered);
                        (candidate[i], candidate[j]) = (candidate[j], candidate[i]);
                        Restack(candidate, top, gaps);

                        var trial = LayoutMetrics.Measure(m_Doc, m_Boxes);
                        if (Accepts(trial, best))
                        {
                            best = trial;
                            improved = true;
                            ordered = candidate;
                        }
                        else
                        {
                            Restack(ordered, top, gaps);
                        }
                    }
                }
            }
            return improved;
        }

        // ---------- write back ----------

        void Commit()
        {
            foreach (var node in m_Doc.Nodes)
            {
                // Blocks are stacked by their context; Shader Graph ignores their own rect.
                if (m_UnitOfNode.TryGetValue(node.ObjectId, out var unit) && IsContext(unit))
                    continue;
                if (!m_Boxes.TryGetValue(node.ObjectId, out var box))
                    continue;
                // Only write the position. A size written here might be an estimate, and the
                // next run would trust it as if the Editor had measured it. Unity fills in the
                // real size when it next draws the node.
                var stored = node.Position;
                node.Position = new Rect(Math.Round(box.X), Math.Round(box.Y), stored.Width, stored.Height);
            }

            foreach (var group in m_Doc.Groups)
            {
                var members = m_Doc.Nodes.Where(n => n.GroupId == group.ObjectId && m_Boxes.ContainsKey(n.ObjectId)).ToList();
                if (members.Count == 0)
                    continue;
                var minX = members.Min(n => m_Boxes[n.ObjectId].X);
                var minY = members.Min(n => m_Boxes[n.ObjectId].Y);
                group.SetPosition(Math.Round(minX - m_Settings.GroupPadding), Math.Round(minY - m_Settings.GroupHeader));
            }

            if (m_Incremental || m_Repaired)
                return;     // the contexts were never moved

            foreach (var pair in m_ContextBlocks)
            {
                if (!m_UnitBoxes.TryGetValue(pair.Key, out var box))
                    continue;
                var field = pair.Key == k_VertexContext ? "m_VertexContext" : "m_FragmentContext";
                if (m_Doc.RootNode[field] is not JsonObject context)
                    continue;
                m_Doc.Raw.Root.Edit();
                context["m_Position"] = new JsonObject
                {
                    // The unit box includes room on its left for value editors. Unity's position
                    // is where the blocks start.
                    ["x"] = Math.Round(box.X + ContextLiteral(pair.Key)),
                    ["y"] = Math.Round(box.Y),
                };
            }
        }
    }
}
