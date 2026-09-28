using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VfxWaitress.Model;

namespace VfxWaitress.Layout
{
    /// <summary>
    /// Positions a graph the way VFX Graph is read: each system is a vertical column of contexts
    /// in flow order, systems sit side by side, and the operators feeding them are ranked
    /// leftward from their consumers. Unlike Shader Graph, contexts run top to bottom.
    ///
    /// Blocks aren't positioned. A context stacks its own blocks and ignores anything stored.
    /// See docs/VfxWaitress-DESIGN.md for the full approach.
    /// </summary>
    public static class LayoutEngine
    {
        const float k_ColumnGutter = 300f;
        const float k_RowGutter = 80f;
        const float k_OperatorRowGutter = 40f;  // tight, so each operator stays near what it feeds
        const float k_OperatorGutter = 150f;    // wide enough that a wire runs across, not up
        const float k_ContextWidth = 424f;      // measured: every context is this wide

        public sealed class Box
        {
            public VfxNode Node;
            public float X, Y, W, H;
            public float Right => X + W;
            public float Bottom => Y + H;
        }

        public static List<string> Apply(VfxAsset asset) => Apply(asset, out _);

        /// <summary>
        /// Lays the graph out and reports how readable the result is, so a caller can compare two
        /// arrangements of the same graph.
        /// </summary>
        public static List<string> Apply(VfxAsset asset, out (int crossings, int steep, int cost) quality)
        {
            var log = new List<string>();
            var boxes = new Dictionary<VfxNode, Box>();
            foreach (var node in asset.TopLevel)
                boxes[node] = new Box { Node = node, X = node.X, Y = node.Y, W = Width(node), H = Height(node) };

            var systems = OrderedSystems(asset);
            var feeders = asset.TopLevel.Where(n => n.Kind != "context").ToList();
            var ranks = RankFeeders(asset, feeders);
            var maxRank = ranks.Count == 0 ? 0 : ranks.Values.Max();

            // Each operator goes in the left margin of the system it feeds. Pooling them all left
            // of the first column would send every wire into later systems through the columns
            // before them.
            var owner = AssignToSystems(asset, systems, feeders, ranks);

            var columns = new Dictionary<VfxNode, int>(owner);
            for (var i = 0; i < systems.Count; i++)
            {
                foreach (var context in systems[i])
                    columns[context] = i;
            }

            var spanning = feeders.Where(f => ReaderColumns(asset, f, columns).Count > 1).ToList();

            // A margin node that feeds another column sends its wire over its own column's
            // contexts. Lifted into a lane above the canvas, the wire crosses empty space instead.
            var lane = Lift ? new HashSet<VfxNode>(spanning) : new HashSet<VfxNode>();

            // The arrangement the graph arrived in is a candidate too, so a layout a person drew
            // better is never replaced with a worse one.
            var bestSeen = boxes.ToDictionary(kvp => kvp.Key, kvp => (x: kvp.Value.X, y: kvp.Value.Y));
            var bestScore = Score(asset, boxes);
            var keptIncoming = true;

            // Context heights that operators aim at. A column's operators are placed before its
            // contexts, so without these every operator would aim at zero and huddle at the top of
            // the canvas. Seeded from the incoming positions and carried between passes.
            var columnTop = new Dictionary<VfxNode, float>();
            foreach (var system in systems)
            {
                foreach (var context in system)
                    columnTop[context] = boxes[context].Y;
            }

            // The passes read the positions they're given, so a badly drawn graph can get stuck.
            // The second attempt starts from neutral: contexts stacked in flow order and operators
            // at zero. Within an attempt, each pass reads the positions the last one produced,
            // which lets operators aim at consumers placed after them.
            for (var attempt = 0; attempt < 2; attempt++)
            {
                if (attempt == 1)
                {
                    foreach (var system in systems)
                    {
                        var stacked = 0f;
                        foreach (var context in system)
                        {
                            boxes[context].Y = stacked;
                            columnTop[context] = stacked;
                            stacked += boxes[context].H + k_RowGutter;
                        }
                    }
                    foreach (var feeder in feeders)
                        boxes[feeder].Y = 0f;
                }

                for (var pass = 0; pass < k_Passes; pass++)
                {
                    var columnX = 0f;
                    var marginX = new float[systems.Count];
                    var highest = 0f;
                    for (var i = 0; i < systems.Count; i++)
                    {
                        marginX[i] = columnX;

                        // The operator band for this system, highest rank furthest from the contexts.
                        var band = feeders.Where(f => owner[f] == i && !lane.Contains(f))
                                          .GroupBy(f => ranks.TryGetValue(f, out var r) ? r : 0)
                                          .OrderByDescending(g => g.Key)
                                          .ToList();
                        foreach (var group in band)
                        {
                            var widest = group.Max(n => boxes[n].W);
                            // Start each node level with what it feeds, then pack down from there.
                            var pull = group.ToDictionary(n => n, n => PreferredY(asset, n, boxes, columnTop, columns));

                            // Order by the height of the ports fed, not where each node wants its top
                            // edge. Those disagree when node heights differ, and the second one crosses
                            // wires. A node feeding nothing placed yet keeps its incoming order.
                            var order = group
                                .OrderBy(n => pull[n].count == 0 ? boxes[n].Y : pull[n].target)
                                .ThenBy(n => n.Y)
                                .ToList();
                            var packed = float.MinValue;
                            var pushes = new List<float>();
                            foreach (var node in order)
                            {
                                var box = boxes[node];
                                box.X = columnX;
                                var wants = pull[node].count == 0 ? box.Y : pull[node].top;
                                box.Y = Math.Max(wants, packed);
                                packed = box.Y + box.H + k_OperatorRowGutter;
                                pushes.Add(box.Y - wants);
                            }

                            // Packing only pushes down, so a crowded group drifts below what it feeds.
                            // Slide the whole group back up by the shift with the least total error.
                            // Not the average, because one node shoved far would drag the rest off.
                            var slide = BestShift(pushes);
                            foreach (var node in order)
                            {
                                var box = boxes[node];
                                box.Y -= slide;
                                highest = Math.Min(highest, box.Y);
                            }
                            columnX += widest + k_OperatorGutter;
                        }

                        // A GPU event column starts level with the block that triggers it.
                        var y = StartOf(asset, systems[i][0], boxes, columnTop);
                        foreach (var context in systems[i])
                        {
                            var box = boxes[context];
                            box.X = columnX;
                            box.Y = y;
                            columnTop[context] = y;
                            y += box.H + k_RowGutter;
                            highest = Math.Min(highest, box.Y);
                        }
                        columnX += k_ContextWidth + k_ColumnGutter;
                    }

                    // The lane sits above everything. Each node goes over the column it serves,
                    // ordered by rank so chains still read left to right.
                    if (lane.Count > 0)
                    {
                        var laneBottom = highest - k_RowGutter;
                        foreach (var group in lane.GroupBy(n => owner.TryGetValue(n, out var c) ? c : 0))
                        {
                            var x = marginX[Math.Clamp(group.Key, 0, systems.Count - 1)];
                            var stackY = laneBottom;
                            foreach (var node in group.OrderByDescending(n => ranks.TryGetValue(n, out var r) ? r : 0))
                            {
                                var box = boxes[node];
                                box.X = x;
                                stackY -= box.H;
                                box.Y = stackY;
                                stackY -= k_OperatorRowGutter;
                            }
                        }
                    }

                    var score = Score(asset, boxes);
                    if (score.cost < bestScore.cost)
                    {
                        bestScore = score;
                        bestSeen = boxes.ToDictionary(kvp => kvp.Key, kvp => (x: kvp.Value.X, y: kvp.Value.Y));
                        keptIncoming = false;
                    }
                }
            }

            foreach (var kvp in bestSeen)
            {
                boxes[kvp.Key].X = kvp.Value.x;
                boxes[kvp.Key].Y = kvp.Value.y;
            }

            foreach (var box in boxes.Values)
                Write(box);
            WriteBounds(asset, boxes.Values);

            log.Add(keptIncoming
                ? "kept the positions the graph came in with; nothing this pass tried read better"
                : $"laid out {systems.Count} system(s), {feeders.Count} feeder node(s) over {maxRank + 1} rank(s)");
            if (lane.Count > 0)
                log.Add($"lifted {lane.Count} node(s) that wire into more than one column into a lane above them");
            quality = Readability(asset, boxes, log);

            // Sizes may be estimates, so warn about any overlap. An overlap hides a node completely.
            var all = boxes.Values.ToList();
            for (var i = 0; i < all.Count; i++)
            {
                for (var j = i + 1; j < all.Count; j++)
                {
                    if (!Intersects(all[i], all[j]))
                        continue;
                    log.Add($"warning: {all[i].Node.ShortId} and {all[j].Node.ShortId} overlap by the tool's own size estimate");
                }
            }
            return log;
        }

        /// <summary>
        /// Places one new node without moving anything else. It goes left of the whole graph, level
        /// with its top, and slides down until it overlaps nothing.
        /// </summary>
        public static void PlaceNew(VfxAsset asset, VfxNode node)
        {
            var others = asset.TopLevel.Where(n => n != node).ToList();
            if (others.Count == 0)
            {
                node.X = 0;
                node.Y = 0;
                Write(new Box { Node = node, X = 0, Y = 0, W = Width(node), H = Height(node) });
                return;
            }

            var left = others.Min(n => n.X);
            var w = Width(node);
            var h = Height(node);
            var x = left - w - k_OperatorGutter;
            var y = others.Min(n => n.Y);

            var box = new Box { Node = node, X = x, Y = y, W = w, H = h };
            while (others.Any(o => Overlaps(box, o)))
                box.Y += k_RowGutter;
            Write(box);
        }

        /// <summary>
        /// Moves each of <paramref name="added"/> beside what it's wired to, without moving anything
        /// else: left of what it feeds and level with the port it feeds, or else right of what feeds
        /// it. A chain of new nodes resolves from the consumer end outward. Nodes wired to nothing
        /// placed stay where <see cref="PlaceNew"/> parked them. Returns the nodes it moved.
        /// </summary>
        public static List<VfxNode> PlaceNear(VfxAsset asset, IReadOnlyList<VfxNode> added)
        {
            var boxes = asset.TopLevel.ToDictionary(n => n, n => new Box { Node = n, X = n.X, Y = n.Y, W = Width(n), H = Height(n) });
            var unplaced = new HashSet<VfxNode>(added.Where(boxes.ContainsKey));
            var moved = new List<VfxNode>();

            // A new context nothing flows into starts a new column where PlaceNew parked it, and
            // the rest of its chain stacks below it.
            foreach (var root in unplaced.Where(n => n.Kind == "context" && !asset.AllNodes().Any(o => o.FlowOut.Any(f => f.toContext == n.FileId))).ToList())
                unplaced.Remove(root);
            var progress = true;
            while (unplaced.Count > 0 && progress)
            {
                progress = false;
                foreach (var node in unplaced.ToList())
                {
                    if (!Anchor(asset, node, boxes, unplaced, out var x, out var y))
                        continue;
                    var box = boxes[node];
                    box.X = x;
                    box.Y = y;
                    var obstacles = boxes.Values.Where(o => o != box && !unplaced.Contains(o.Node)).ToList();
                    for (var guard = 0; guard < 200; guard++)
                    {
                        var hit = obstacles.FirstOrDefault(o => Intersects(Grow(box, k_OperatorRowGutter * 0.5f), o));
                        if (hit == null)
                            break;
                        box.Y = hit.Bottom + k_OperatorRowGutter;
                    }
                    Write(box);
                    unplaced.Remove(node);
                    moved.Add(node);
                    progress = true;
                }
            }
            return moved;
        }

        static bool Anchor(VfxAsset asset, VfxNode node, Dictionary<VfxNode, Box> boxes, HashSet<VfxNode> unplaced, out float x, out float y)
        {
            x = y = 0f;
            var box = boxes[node];
            if (node.Kind == "context")
            {
                // Contexts stack by flow: below whatever flows in, or above whatever it flows into.
                foreach (var other in asset.TopLevel)
                {
                    if (other == node || unplaced.Contains(other) || !boxes.TryGetValue(other, out var otherBox))
                        continue;
                    if (other.FlowOut.Any(f => f.toContext == node.FileId))
                    {
                        x = otherBox.X;
                        y = otherBox.Bottom + k_RowGutter;
                        return true;
                    }
                    if (node.FlowOut.Any(f => f.toContext == other.FileId))
                    {
                        x = otherBox.X;
                        y = otherBox.Y - box.H - k_RowGutter;
                        return true;
                    }
                }
                return false;
            }

            var mine = new HashSet<long>(node.AllOutputSlots.Select(s => s.FileId));
            var left = float.MaxValue;
            var total = 0f;
            var count = 0;
            foreach (var other in asset.AllNodes())
            {
                var target = other.Context ?? other;
                if (target == node || unplaced.Contains(target) || !boxes.ContainsKey(target))
                    continue;
                foreach (var slot in other.AllInputSlots)
                {
                    if (!slot.LinkedSlotIds.Any(mine.Contains))
                        continue;
                    left = Math.Min(left, boxes[target].X);
                    total += AnchorY(slot, boxes) - OutPortOffset(node, slot, mine);
                    count++;
                }
            }
            if (count > 0)
            {
                x = left - box.W - k_OperatorGutter;
                y = total / count;
                return true;
            }

            var right = float.MinValue;
            foreach (var input in node.AllInputSlots)
            {
                foreach (var id in input.LinkedSlotIds)
                {
                    var from = asset.SlotById(id);
                    var source = from?.Owner;
                    if (source == null)
                        continue;
                    source = source.Context ?? source;
                    if (source == node || unplaced.Contains(source) || !boxes.TryGetValue(source, out var sourceBox))
                        continue;
                    right = Math.Max(right, sourceBox.Right);
                    total += sourceBox.Y + PortHeight(source, from, false) - PortHeight(node, input, true);
                    count++;
                }
            }
            if (count == 0)
                return false;
            x = right + k_OperatorGutter;
            y = total / count;
            return true;
        }

        static Box Grow(Box box, float margin) =>
            new Box { X = box.X - margin, Y = box.Y - margin, W = box.W + margin * 2f, H = box.H + margin * 2f };

        /// <summary>
        /// The single shift that leaves a packed group closest to where its wires want it. Ties go
        /// to the shift that moves least.
        /// </summary>
        static float BestShift(List<float> pushes)
        {
            if (pushes.Count == 0)
                return 0f;
            var best = 0f;
            var bestError = pushes.Sum(Math.Abs);
            foreach (var candidate in pushes)
            {
                var error = pushes.Sum(p => Math.Abs(p - candidate));
                if (error < bestError - 0.5f ||
                    (error < bestError + 0.5f && Math.Abs(candidate) < Math.Abs(best)))
                {
                    best = candidate;
                    bestError = error;
                }
            }
            return best;
        }

        /// <summary>Which columns the wires leaving a node land in.</summary>
        static HashSet<int> ReaderColumns(VfxAsset asset, VfxNode node, Dictionary<VfxNode, int> columns)
        {
            var mine = new HashSet<long>(node.AllOutputSlots.Select(s => s.FileId));
            var reached = new HashSet<int>();
            foreach (var other in asset.AllNodes())
            {
                if (!other.AllInputSlots.Any(s => s.LinkedSlotIds.Any(mine.Contains)))
                    continue;
                var target = other.Context ?? other;
                if (columns.TryGetValue(target, out var column) && column >= 0 && column < int.MaxValue)
                    reached.Add(column);
            }
            return reached;
        }

        /// <summary>
        /// Where the wires leaving this node pull it, as two separate numbers.
        ///
        /// <c>target</c> is the height of the ports it feeds. That decides stacking order, since
        /// two nodes feeding the same consumer must be stacked in the order of the ports they
        /// feed, or their wires cross.
        ///
        /// <c>top</c> is where this node's top edge would sit for its wires to run level. It
        /// differs from <c>target</c> by the node's own port offset, which varies by node type, so
        /// ordering by it crosses wires.
        /// </summary>
        static (float target, float top, int count) PreferredY(
            VfxAsset asset, VfxNode node, Dictionary<VfxNode, Box> boxes, Dictionary<VfxNode, float> placed,
            Dictionary<VfxNode, int> columns)
        {
            var mine = new HashSet<long>(node.AllOutputSlots.Select(s => s.FileId));
            var home = columns != null && columns.TryGetValue(node, out var c) ? c : int.MinValue;
            var all = (target: 0f, top: 0f, count: 0);
            var near = (target: 0f, top: 0f, count: 0);

            foreach (var other in asset.AllNodes())
            {
                foreach (var slot in other.AllInputSlots)
                {
                    if (!slot.LinkedSlotIds.Any(mine.Contains))
                        continue;
                    var consumer = other.Context ?? other;
                    if (consumer.Kind == "context" && !placed.ContainsKey(consumer))
                        continue;
                    if (!boxes.ContainsKey(consumer))
                        continue;
                    var anchor = AnchorY(slot, boxes);
                    var wants = anchor - OutPortOffset(node, slot, mine);
                    all = (all.target + anchor, all.top + wants, all.count + 1);
                    if (columns != null && columns.TryGetValue(consumer, out var where) && where == home)
                        near = (near.target + anchor, near.top + wants, near.count + 1);
                }
            }

            // For a node feeding several columns, the average would be far from all of them. Line
            // it up with the consumers in its own column. The far ones get a long wire either way.
            var chosen = near.count > 0 ? near : all;
            return chosen.count == 0
                ? (0f, 0f, 0)
                : (chosen.target / chosen.count, chosen.top / chosen.count, chosen.count);
        }

        /// <summary>
        /// How far below this node's top edge the output port feeding <paramref name="consumer"/>
        /// is drawn. Subtracting it lets a wire leave and arrive at the same height.
        /// </summary>
        static float OutPortOffset(VfxNode node, VfxSlot consumer, HashSet<long> mine)
        {
            foreach (var id in consumer.LinkedSlotIds)
            {
                if (!mine.Contains(id))
                    continue;
                var slot = node.AllOutputSlots.FirstOrDefault(s => s.FileId == id);
                if (slot != null)
                    return PortHeight(node, slot, false);
            }
            return node.SuperCollapsed ? k_CollapsedPort : k_PortStart;
        }

        /// <summary>Which system columns this node ultimately feeds, following operator chains.</summary>
        static HashSet<int> SystemsFed(VfxAsset asset, List<List<VfxNode>> systems, VfxNode node)
        {
            var systemOf = new Dictionary<VfxNode, int>();
            for (var i = 0; i < systems.Count; i++)
            {
                foreach (var context in systems[i])
                    systemOf[context] = i;
            }

            var reached = new HashSet<int>();
            var seen = new HashSet<VfxNode>();
            var queue = new Queue<VfxNode>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var mine = new HashSet<long>(current.AllOutputSlots.Select(s => s.FileId));
                foreach (var other in asset.AllNodes())
                {
                    if (!other.AllInputSlots.Any(s => s.LinkedSlotIds.Any(mine.Contains)))
                        continue;
                    var target = other.Context ?? other;
                    if (systemOf.TryGetValue(target, out var index))
                        reached.Add(index);
                    else if (seen.Add(target))
                        queue.Enqueue(target);
                }
            }
            return reached;
        }

        /// <summary>
        /// Which system's margin each operator belongs in: the leftmost system it feeds, followed
        /// through other operators so a chain ends up beside the column it ultimately serves.
        /// </summary>
        static Dictionary<VfxNode, int> AssignToSystems(
            VfxAsset asset, List<List<VfxNode>> systems, List<VfxNode> feeders, Dictionary<VfxNode, int> ranks)
        {
            var systemOf = new Dictionary<VfxNode, int>();
            for (var i = 0; i < systems.Count; i++)
            {
                foreach (var context in systems[i])
                    systemOf[context] = i;
            }

            var owner = feeders.ToDictionary(f => f, _ => int.MaxValue);
            // Lowest rank first. A rank-0 operator gets its system from a context, and everything
            // feeding it gets its system from that operator.
            foreach (var feeder in feeders.OrderBy(f => ranks.TryGetValue(f, out var r) ? r : 0))
            {
                var mine = new HashSet<long>(feeder.AllOutputSlots.Select(s => s.FileId));
                foreach (var other in asset.AllNodes())
                {
                    if (!other.AllInputSlots.Any(s => s.LinkedSlotIds.Any(mine.Contains)))
                        continue;
                    var target = other.Context ?? other;
                    var index = systemOf.TryGetValue(target, out var direct) ? direct
                        : owner.TryGetValue(target, out var viaFeeder) ? viaFeeder
                        : int.MaxValue;
                    if (index < owner[feeder])
                        owner[feeder] = index;
                }
                if (owner[feeder] == int.MaxValue)
                    owner[feeder] = 0;
            }
            return owner;
        }


        /// <summary>
        /// Readability as numbers: steep wires read as diagonals cutting across the graph, and a
        /// wire through an unrelated node is what a person notices first. Wires over nodes cost
        /// more than slope, because they hide something.
        /// </summary>
        static (int crossings, int steep, int cost) Score(VfxAsset asset, Dictionary<VfxNode, Box> boxes) =>
            Inspect(asset, boxes, null);

        static (int crossings, int steep, int cost) Readability(VfxAsset asset, Dictionary<VfxNode, Box> boxes, List<string> log) =>
            Inspect(asset, boxes, log);

        static (int crossings, int steep, int cost) Inspect(
            VfxAsset asset, Dictionary<VfxNode, Box> boxes, List<string> log)
        {
            var steep = 0;
            var crossings = new List<(string where, bool level)>();
            var wires = 0;
            // Slope counts by how far past the threshold a wire goes, not yes or no, so shaving
            // fifty pixels off every wire still scores better.
            var cost = 0f;
            var steepest = new List<(string where, float slope)>();

            foreach (var consumer in asset.AllNodes())
            {
                var target = consumer.Context ?? consumer;
                if (!boxes.TryGetValue(target, out var targetBox))
                    continue;
                foreach (var slot in consumer.AllInputSlots)
                {
                    foreach (var id in slot.LinkedSlotIds)
                    {
                        var sourceTop = asset.SlotById(id)?.Owner;
                        if (sourceTop == null)
                            continue;
                        sourceTop = sourceTop.Context ?? sourceTop;
                        if (!boxes.TryGetValue(sourceTop, out var sourceBox) || sourceBox == targetBox)
                            continue;

                        wires++;
                        var from = asset.SlotById(id);
                        var y0 = sourceBox.Y + (from == null ? k_PortStart : PortHeight(sourceTop, from, false));
                        var y1 = AnchorY(slot, boxes);
                        var slope = Math.Abs(y1 - y0);
                        cost += Math.Max(0f, slope - k_SteepWire);
                        if (slope > k_SteepWire)
                        {
                            steep++;
                            steepest.Add(($"{sourceTop.ShortId} -> {target.ShortId}.{slot.Path} drops {slope:0} px", slope));
                        }

                        var level = slope <= k_LevelWire;
                        foreach (var box in boxes.Values)
                        {
                            if (box == sourceBox || box == targetBox)
                                continue;
                            if (!CrossesBox(sourceBox.Right, y0, targetBox.X, y1, box))
                                continue;
                            // The eye can follow a dead-level wire across a node. A sloped one gets
                            // lost in whatever it passes over, so it costs more.
                            crossings.Add(($"{sourceTop.ShortId} -> {target.ShortId} passes " +
                                           (level ? "level across " : "through ") + box.Node.ShortId, level));
                        }
                    }
                }
            }

            // Without this, piling every node on one point would score best.
            var all = boxes.Values.ToList();
            var overlaps = 0;
            for (var i = 0; i < all.Count; i++)
            {
                for (var j = i + 1; j < all.Count; j++)
                {
                    if (Intersects(all[i], all[j]))
                        overlaps++;
                }
            }

            var distinct = crossings.Distinct().ToList();
            var sloped = distinct.Count(c => !c.level);
            var total = (int)(cost / 10f) + sloped * k_CrossingCost +
                        (distinct.Count - sloped) * k_LevelCrossingCost + overlaps * k_OverlapCost;
            if (log != null && wires > 0)
            {
                log.Add($"readability: {wires} wire(s), {steep} steeper than {k_SteepWire:0} px, " +
                        $"{sloped} crossing a node at a slope, {distinct.Count - sloped} crossing one dead level, " +
                        $"{overlaps} node pair(s) overlapping, cost {total}");
                foreach (var crossing in distinct.Take(6))
                    log.Add("  " + crossing.where);
                foreach (var wire in steepest.OrderByDescending(s => s.slope).Take(6))
                    log.Add("  " + wire.where);
            }
            return (distinct.Count, steep, total);
        }

        /// <summary>
        /// How many placement passes each attempt runs. The best-scoring pass is the one written.
        /// </summary>
        const int k_Passes = 6;

        const float k_SteepWire = 120f;

        /// <summary>What one sloped wire drawn over an unrelated node is worth in slope pixels.</summary>
        const int k_CrossingCost = 150;

        /// <summary>The same wire when it runs level.</summary>
        const int k_LevelCrossingCost = 25;

        /// <summary>How far off level a wire may be and still read as a straight line.</summary>
        const float k_LevelWire = 8f;

        /// <summary>One node drawn over another, which hides it completely.</summary>
        const int k_OverlapCost = 400;

        /// <summary>Whether the straight run between two ports passes over a node's box.</summary>
        static bool CrossesBox(float x0, float y0, float x1, float y1, Box box)
        {
            if (x1 <= x0)
                return false;
            var enter = Math.Max(x0, box.X);
            var exit = Math.Min(x1, box.Right);
            if (enter >= exit)
                return false;
            var yAt = new Func<float, float>(x => y0 + (y1 - y0) * (x - x0) / (x1 - x0));
            var top = Math.Min(yAt(enter), yAt(exit));
            var bottom = Math.Max(yAt(enter), yAt(exit));
            return bottom > box.Y && top < box.Bottom;
        }
        static bool Intersects(Box a, Box b) =>
            a.X < b.Right && a.Right > b.X && a.Y < b.Bottom && a.Bottom > b.Y;

        static bool Overlaps(Box box, VfxNode other)
        {
            var ow = Width(other);
            var oh = Height(other);
            return box.X < other.X + ow && box.Right > other.X &&
                   box.Y < other.Y + oh && box.Bottom > other.Y;
        }

        /// <summary>
        /// Where a column begins. A GPU event context is triggered by a block in another column,
        /// so it starts beside that block instead of at the top of the canvas.
        /// </summary>
        static float StartOf(VfxAsset asset, VfxNode root, Dictionary<VfxNode, Box> boxes, Dictionary<VfxNode, float> placed)
        {
            var below = 0f;
            var found = false;

            // Flow from a context already placed, usually a spawner shared by several systems.
            // Start below it so flow keeps running downward.
            foreach (var source in asset.AllNodes())
            {
                if (!source.FlowOut.Any(f => f.toContext == root.FileId) || !placed.ContainsKey(source))
                    continue;
                below = Math.Max(below, boxes[source].Bottom + k_RowGutter);
                found = true;
            }

            // A GPU event is triggered by a block, so it starts below the context holding it.
            foreach (var slot in root.AllInputSlots)
            {
                foreach (var id in slot.LinkedSlotIds)
                {
                    var owner = asset.SlotById(id)?.Owner?.Context;
                    if (owner == null || !placed.ContainsKey(owner))
                        continue;
                    below = Math.Max(below, boxes[owner].Bottom + k_RowGutter);
                    found = true;
                }
            }

            return found ? below : 0f;
        }

        public static List<List<VfxNode>> Systems(VfxAsset asset) => OrderedSystems(asset);

        /// <summary>
        /// Which column every node is drawn in — contexts and their blocks from the system they
        /// belong to, operators from the leftmost system they end up feeding.
        /// </summary>
        public static Dictionary<VfxNode, int> Columns(VfxAsset asset, List<List<VfxNode>> systems)
        {
            var feeders = asset.TopLevel.Where(n => n.Kind != "context").ToList();
            var columns = AssignToSystems(asset, systems, feeders, RankFeeders(asset, feeders));
            for (var i = 0; i < systems.Count; i++)
            {
                foreach (var context in systems[i])
                {
                    columns[context] = i;
                    foreach (var block in context.Blocks)
                        columns[block] = i;
                }
            }
            return columns;
        }

        static List<List<VfxNode>> OrderedSystems(VfxAsset asset)
        {
            var contexts = asset.TopLevel.Where(n => n.Kind == "context").ToList();
            var groups = new List<List<VfxNode>>();
            var placed = new HashSet<VfxNode>();

            // Start from every context nothing flows into, then walk the flow forwards.
            var roots = contexts.Where(c => !asset.AllNodes().Any(n => n.FlowOut.Any(f => f.toContext == c.FileId))).ToList();
            foreach (var root in roots.Concat(contexts))
            {
                if (!placed.Add(root))
                    continue;
                var chain = new List<VfxNode> { root };
                var current = root;
                while (true)
                {
                    var next = current.FlowOut
                        .Select(f => asset.Nodes.TryGetValue(f.toContext, out var n) ? n : null)
                        .FirstOrDefault(n => n != null && !placed.Contains(n));
                    if (next == null)
                        break;
                    placed.Add(next);
                    chain.Add(next);
                    current = next;
                }
                groups.Add(chain);
            }
            return groups;
        }

        static Dictionary<VfxNode, int> RankFeeders(VfxAsset asset, List<VfxNode> feeders)
        {
            var consumers = new Dictionary<VfxNode, List<VfxNode>>();
            foreach (var node in asset.AllNodes())
            {
                foreach (var slot in node.AllInputSlots)
                {
                    foreach (var id in slot.LinkedSlotIds)
                    {
                        var source = asset.SlotById(id)?.Owner;
                        if (source == null)
                            continue;
                        // A block's consumer is its context, which is what carries a position.
                        var consumer = node.Context ?? node;
                        if (!consumers.TryGetValue(source, out var list))
                            consumers[source] = list = new List<VfxNode>();
                        if (!list.Contains(consumer))
                            list.Add(consumer);
                    }
                }
            }

            var ranks = new Dictionary<VfxNode, int>();
            foreach (var feeder in feeders)
                ranks[feeder] = Rank(feeder, consumers, ranks, new HashSet<VfxNode>());
            return ranks;
        }

        static int Rank(VfxNode node, Dictionary<VfxNode, List<VfxNode>> consumers, Dictionary<VfxNode, int> memo, HashSet<VfxNode> visiting)
        {
            if (memo.TryGetValue(node, out var cached))
                return cached;
            if (!visiting.Add(node))
                return 0;
            var rank = 0;
            if (consumers.TryGetValue(node, out var list))
            {
                foreach (var consumer in list)
                {
                    if (consumer.Kind == "context")
                        continue;
                    rank = Math.Max(rank, Rank(consumer, consumers, memo, visiting) + 1);
                }
            }
            visiting.Remove(node);
            memo[node] = rank;
            return rank;
        }

        /// <summary>
        /// Node sizes read off a live graph view, keyed by file id. VFX Graph stores no sizes, so
        /// when this is empty the estimates below are used instead.
        /// </summary>
        public static Edit.Measurements Measured = new Edit.Measurements();

        /// <summary>
        /// Whether a node whose wires reach another column is lifted into a lane above the canvas.
        /// That keeps the wire off the contexts but makes it longer. Which is better depends on
        /// the graph, so the caller lays it out both ways and compares.
        /// </summary>
        public static bool Lift = true;

        static bool Size(VfxNode node, out float w, out float h)
        {
            if (node != null && Measured.Boxes.TryGetValue(node.FileId, out var m))
            {
                (w, h) = m;
                return true;
            }
            w = h = 0f;
            return false;
        }

        static float Width(VfxNode node)
        {
            if (Size(node, out var measuredWidth, out _))
                return measuredWidth;
            if (node.Kind == "context")
                return k_ContextWidth;
            if (node.Collapsed)
                return 140f;
            var longest = node.AllInputSlots.Concat(node.AllOutputSlots)
                .Select(s => s.Path?.Length ?? 0).DefaultIfEmpty(0).Max();
            // Measured operators run 143-210 wide. Use the widest, since a column that's too narrow
            // makes wires climb steeply.
            return Math.Max(210f, 90f + longest * 8f);
        }

        // Calibrated against sizes read off a live graph view (Validator.Measure). Estimating high
        // is the safe direction: extra whitespace is cosmetic, but an overlap hides a node.
        const float k_BlockHeader = 40f;
        const float k_BlockRow = 30f;
        const float k_OperatorBase = 68f;
        const float k_OperatorRow = 22f;
        const float k_OperatorSettingRow = 20f;
        const float k_PortStart = 44f;          // where the first port sits below a node's top edge
        const float k_CollapsedPort = 16f;      // a node collapsed to a dot draws its ports at its middle

        /// <summary>Measured: an empty context of each kind, before its blocks.</summary>
        static float ContextBase(VfxNode node)
        {
            // A measured context includes its blocks, so what is left over is the bare context.
            if (Size(node, out _, out var measured))
            {
                foreach (var block in node.Blocks)
                    measured -= BlockHeight(block);
                if (measured > 0f)
                    return measured;
            }
            var type = node.DisplayType ?? "";
            if (type.Contains("Initialize", StringComparison.Ordinal))
                return 260f;
            if (type.Contains("Output", StringComparison.Ordinal))
                return 297f;   // measured
            if (type.Contains("Spawner", StringComparison.Ordinal))
                return 200f;
            if (type.Contains("GPUEvent", StringComparison.Ordinal))
                return 160f;
            if (type.Contains("Update", StringComparison.Ordinal))
                return 170f;
            return 220f;
        }

        static float BlockHeight(VfxNode block) =>
            Size(block, out _, out var measured) ? measured : k_BlockHeader + k_BlockRow * Math.Max(1, block.Inputs.Count);

        /// <summary>
        /// Where on a node's left edge a given input slot is drawn. Wires aim at ports, not nodes.
        /// Otherwise every operator feeding a context aims at its top, and their wires fan out
        /// across each other.
        /// </summary>
        static float AnchorY(VfxSlot slot, Dictionary<VfxNode, Box> boxes)
        {
            var node = slot.Owner;
            if (node == null)
                return 0f;
            var top = node.Context ?? node;
            if (!boxes.TryGetValue(top, out var box))
                return 0f;
            var root = Root(slot);

            // Where the graph view draws this port, if an Editor answered. A collapsed sub-slot
            // draws on its root's row.
            if (Measured.PortY.TryGetValue(slot.FileId, out var exact))
                return box.Y + exact;
            if (Measured.PortY.TryGetValue(root.FileId, out exact))
                return box.Y + exact;

            if (node.Context != null)
            {
                var y = box.Y + ContextBase(node.Context);
                foreach (var block in node.Context.Blocks)
                {
                    if (block == node)
                        break;
                    y += BlockHeight(block);
                }
                return y + k_BlockHeader + k_BlockRow * Math.Max(0, node.Inputs.IndexOf(root));
            }
            return box.Y + PortHeight(node, slot, true);
        }

        /// <summary>
        /// How far below a node's top edge one of its ports is drawn: measured when an Editor
        /// answered, otherwise estimated from the row. Blocks are handled by <see cref="AnchorY"/>,
        /// since their ports are measured against their context.
        /// </summary>
        public static float PortHeight(VfxNode node, VfxSlot slot, bool input)
        {
            if (node.SuperCollapsed)
                return k_CollapsedPort;
            var table = input ? Measured.PortY : Measured.OutPortY;
            var root = Root(slot);
            if (table.TryGetValue(slot.FileId, out var exact))
                return exact;
            if (table.TryGetValue(root.FileId, out exact))
                return exact;
            var rows = input ? node.Inputs : node.Outputs;
            return k_PortStart + k_OperatorRow * Math.Max(0, rows.IndexOf(root));
        }

        static VfxSlot Root(VfxSlot slot)
        {
            while (slot.Parent != null)
                slot = slot.Parent;
            return slot;
        }

        static float Height(VfxNode node)
        {
            if (node.Kind != "context" && Size(node, out _, out var measuredHeight))
                return measuredHeight;
            if (node.Kind == "context")
            {
                var height = ContextBase(node);
                foreach (var block in node.Blocks)
                    height += BlockHeight(block);
                return height;
            }
            if (node.Collapsed)
                return 40f;
            // A parameter's settings live in the blackboard, not on the node.
            if (node.ExposedName != null)
                return k_OperatorBase + k_OperatorRow * Math.Max(0, node.Outputs.Count - 1);
            var rows = node.Inputs.Count + node.Outputs.Count;
            var settings = Math.Min(3, node.Model?.Settings.Count ?? 0);
            return k_OperatorBase + k_OperatorRow * Math.Max(0, rows - 1) + k_OperatorSettingRow * settings;
        }

        static void Write(Box box)
        {
            var node = box.Node;
            node.X = box.X;
            node.Y = box.Y;
            // A parameter's m_UIPosition is always zero, since it's drawn once per placement
            // record in m_Nodes instead. Write the zero explicitly, because the Editor zeroes a
            // stale value on open and marks the graph dirty.
            node.Doc.Body.SetScalar("m_UIPosition",
                node.ExposedName != null ? Vector(0, 0) : Vector(box.X, box.Y));

            // A parameter with links but no placement gets repaired on open, which also marks
            // the graph dirty, so write a placement.
            if (node.ParameterNodes.Count == 0 && node.ExposedName == null)
                return;

            var seq = node.Doc.Body.Seq("m_Nodes");
            if (seq == null)
            {
                // An empty list is a flow "[]" on the key line, which parses as a scalar, so
                // writing through Seq() would silently do nothing.
                var links = node.Outputs.SelectMany(s => s.SelfAndDescendants())
                    .SelectMany(s => s.LinkedSlotIds.Select(id => (from: s.FileId, to: id)))
                    .ToList();
                node.Doc.Body.SetBlock("m_Nodes", PlacementRecord(box.X, box.Y, links));
                return;
            }

            var offset = 0f;
            for (var i = 0; i < seq.Items.Count; i++)
            {
                seq.Items[i] = System.Text.RegularExpressions.Regex.Replace(
                    seq.Items[i],
                    @"position: \{[^}]*\}",
                    "position: " + Vector(box.X, box.Y + offset));
                offset += box.H + k_OperatorRowGutter;
            }
            seq.Touch();
        }

        /// <summary>
        /// The graph's overall extent, which the view frames when it opens. The Editor recomputes
        /// it whenever the view builds, but left at zero a new graph opens framed on nothing.
        /// </summary>
        static void WriteBounds(VfxAsset asset, IEnumerable<Box> boxes)
        {
            if (asset.UiDoc == null)
                return;
            var all = boxes.ToList();
            if (all.Count == 0)
                return;
            // The Editor recomputes this from whole-pixel rects, so round to avoid a diff.
            var minX = MathF.Round(all.Min(b => b.X));
            var minY = MathF.Round(all.Min(b => b.Y));
            var maxX = MathF.Round(all.Max(b => b.Right));
            var maxY = MathF.Round(all.Max(b => b.Bottom));
            var sb = new System.Text.StringBuilder();
            sb.Append("    serializedVersion: 2\n");
            sb.Append("    x: ").Append(Num(minX)).Append('\n');
            sb.Append("    y: ").Append(Num(minY)).Append('\n');
            sb.Append("    width: ").Append(Num(maxX - minX)).Append('\n');
            sb.Append("    height: ").Append(Num(maxY - minY)).Append('\n');
            asset.UiDoc.Body.SetBlock("uiBounds", sb.ToString());
        }

        static string Num(float v) => v.ToString("0.####", CultureInfo.InvariantCulture);

        /// <summary>One on-canvas placement of a parameter, carrying the links it draws.</summary>
        static string PlacementRecord(float x, float y, List<(long from, long to)> links)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("  - m_Id: 0\n");
            if (links.Count == 0)
            {
                sb.Append("    linkedSlots: []\n");
            }
            else
            {
                sb.Append("    linkedSlots:\n");
                foreach (var (from, to) in links)
                {
                    sb.Append("    - outputSlot: {fileID: ").Append(from.ToString(CultureInfo.InvariantCulture)).Append("}\n");
                    sb.Append("      inputSlot: {fileID: ").Append(to.ToString(CultureInfo.InvariantCulture)).Append("}\n");
                }
            }
            sb.Append("    position: ").Append(Vector(x, y)).Append('\n');
            sb.Append("    expandedSlots: []\n");
            sb.Append("    expanded: 1\n");
            sb.Append("    supecollapsed: 0\n");
            return sb.ToString();
        }

        static string Vector(float x, float y) =>
            "{x: " + x.ToString("0.####", CultureInfo.InvariantCulture) + ", y: " + y.ToString("0.####", CultureInfo.InvariantCulture) + "}";
    }
}
