using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ShaderWaitress.Model;

namespace ShaderWaitress.Layout
{
    /// <summary>
    /// Readability score for a laid-out graph. Shader Graph draws its own wires and the tool
    /// can't route them, so readability is a penalty minimized by moving nodes. Crossings use
    /// the straight line between ports, which is close to what Shader Graph draws, and wires
    /// over nodes use the real curve. The weights are in <see cref="Score"/> and in
    /// docs/ShaderWaitress-DESIGN.md.
    /// </summary>
    public sealed class LayoutMetrics
    {
        public const double ShallowAngleDegrees = 25;
        public const double CrossingClusterRadius = 24;

        /// <summary>
        /// Clear space two node boxes must leave between them. A stored size can be older than
        /// a dropdown or preview turned on since, so boxes packed to touching can overlap on
        /// screen.
        /// </summary>
        public const double Clearance = 24;

        /// <summary>
        /// Horizontal reach of a wire's control points, and how finely the curve is sampled.
        /// Shader Graph's EdgeControl uses a fixed 28 px tangent at each end however long the
        /// wire is, so a wire is nearly the straight line between its ports.
        /// </summary>
        const double k_WireTangent = 28;
        const int k_WireSamples = 12;
        /// <summary>How far inside a node edge its connectors sit.</summary>
        const double k_PortInset = 16;

        public int NodeOverlaps;
        /// <summary>Pairs closer than <see cref="Clearance"/> without actually overlapping.</summary>
        public int Crowded;
        public int Crossings;
        public int ShallowCrossings;
        public int CrossingClusters;
        public int WiresOverNodes;
        /// <summary>Value editors of unconnected inputs drawn on top of a neighbouring node.</summary>
        public int EditorOverlaps;
        /// <summary>Set when the vertex and fragment stacks do not share a column.</summary>
        public int ContextsMisaligned;
        public int BackwardEdges;
        public double WireLength;
        public int GroupOverlaps;
        public int NodesOutsideGroup;
        /// <summary>
        /// Pixels of empty band inside a group box, beyond normal spacing. A group laid out as a
        /// chain is wide but has no holes. A group whose members drifted apart has a visible gap.
        /// </summary>
        public double GroupGaps;

        /// <summary>One line per defect, naming the wires and nodes involved.</summary>
        public List<string> Detail { get; } = new List<string>();

        /// <summary>
        /// Nodes involved in a crossing or with a wire drawn across them. Refinement only tries
        /// moving these, since trying every node is too slow on a large graph.
        /// </summary>
        public HashSet<string> Implicated { get; } = new HashSet<string>(StringComparer.Ordinal);

        /// <summary>
        /// Each wire drawn across a node, with the vertical band it covers within that node's
        /// horizontal span. That's enough to work out which way and how far to move the node.
        /// </summary>
        public List<(string node, string from, double bandTop, double bandBottom)> Obstructions { get; } =
            new List<(string, string, double, double)>();

        static string Label(ShaderGraphDocument doc, string objectId)
        {
            var node = doc.NodeById(objectId);
            if (node == null)
                return objectId.Substring(0, 4);
            return node.IsBlock ? node.BlockDescriptor ?? node.ShortId : $"{node.ShortId} {node.TypeLabel}";
        }

        public double Score =>
            NodeOverlaps * 5000.0 +
            BackwardEdges * 1000.0 +
            GroupOverlaps * 2000.0 +
            NodesOutsideGroup * 800.0 +
            CrossingClusters * 400.0 +
            ShallowCrossings * 250.0 +
            Crossings * 100.0 +
            // A wire over a node hides part of it, which reads worse than a crossing. At 200,
            // layout will trade one crossing to lift a wire off a node, but not two.
            WiresOverNodes * 200.0 +
            // A value editor is an opaque box, so it hides what it lands on outright.
            EditorOverlaps * 500.0 +
            ContextsMisaligned * 300.0 +
            WireLength * 0.01 +
            // Unity draws a group box around whatever its members span, so a stray member
            // leaves a big empty band. No other term notices that.
            GroupGaps * 0.5;

        struct Segment
        {
            public double X1, Y1, X2, Y2;
            public string From, To;
            public int FromSlot, ToSlot;
        }

        /// <param name="ignore">
        /// Nodes to leave out entirely, along with every wire touching them. Used to judge the
        /// part of a graph that is already placed while some nodes have no position yet.
        /// </param>
        public static LayoutMetrics Measure(ShaderGraphDocument doc, IReadOnlyDictionary<string, Rect> boxes = null,
            IReadOnlyCollection<string> ignore = null)
        {
            var metrics = new LayoutMetrics();
            var rects = Rects(doc, boxes);
            if (ignore != null)
            {
                foreach (var id in ignore)
                    rects.Remove(id);
            }

            var nodes = doc.Nodes.Where(n => rects.ContainsKey(n.ObjectId)).ToList();
            for (var i = 0; i < nodes.Count; i++)
            {
                for (var j = i + 1; j < nodes.Count; j++)
                {
                    var a = rects[nodes[i].ObjectId];
                    var b = rects[nodes[j].ObjectId];
                    if (a.Overlaps(b, -1))
                        metrics.NodeOverlaps++;
                    // Shader Graph stacks blocks tightly, and that isn't the layout's to decide.
                    else if (!nodes[i].IsBlock && !nodes[j].IsBlock && a.Overlaps(b, Clearance))
                        metrics.Crowded++;
                }
            }

            // The vertex and fragment stacks belong in one column. Compare where the blocks
            // draw, since each stack's box includes its own value-editor lane.
            var stacks = new List<double>();
            foreach (var isVertex in new[] { true, false })
            {
                var first = doc.ContextBlocks(isVertex).FirstOrDefault(b => rects.ContainsKey(b));
                if (first != null)
                    stacks.Add(rects[first].Right - NodeSizer.BlockWidth);
            }
            if (stacks.Count == 2 && Math.Abs(stacks[0] - stacks[1]) > 1)
            {
                metrics.ContextsMisaligned = 1;
                metrics.Detail.Add("the vertex and fragment stacks start in different columns");
            }

            // An unconnected input draws an opaque value editor outside its node, on the left,
            // where another node might be.
            foreach (var node in nodes)
            {
                foreach (var editor in NodeSizer.PortEditors(doc, node, rects[node.ObjectId]))
                {
                    foreach (var other in nodes)
                    {
                        if (other.Is(node) || other.IsBlock || !editor.Overlaps(rects[other.ObjectId], -1))
                            continue;
                        metrics.EditorOverlaps++;
                        metrics.Implicated.Add(node.ObjectId);
                        metrics.Implicated.Add(other.ObjectId);
                        metrics.Detail.Add(
                            $"value editor of {Label(doc, node.ObjectId)} covers {Label(doc, other.ObjectId)}");
                    }
                }
            }

            var segments = new List<Segment>();
            foreach (var edge in doc.Edges)
            {
                if (!rects.TryGetValue(edge.From.ObjectId, out var fromRect) ||
                    !rects.TryGetValue(edge.To.ObjectId, out var toRect))
                    continue;
                var fromPort = edge.FromPort;
                var toPort = edge.ToPort;
                var y1 = fromRect.Y + (fromPort != null ? NodeSizer.PortAnchor(edge.From, fromPort, fromRect.Height) : fromRect.Height * 0.5);
                var y2 = toRect.Y + (toPort != null ? NodeSizer.PortAnchor(edge.To, toPort, toRect.Height) : toRect.Height * 0.5);
                var segment = new Segment
                {
                    // Connectors sit 16 px inside the node edge, measured off a live EdgeControl.
                    X1 = fromRect.Right - k_PortInset,
                    Y1 = y1,
                    X2 = toRect.X + k_PortInset,
                    Y2 = y2,
                    From = edge.From.ObjectId,
                    To = edge.To.ObjectId,
                    FromSlot = edge.FromSlot,
                    ToSlot = edge.ToSlot,
                };
                segments.Add(segment);
                metrics.WireLength += Math.Sqrt((segment.X2 - segment.X1) * (segment.X2 - segment.X1) +
                                                (segment.Y2 - segment.Y1) * (segment.Y2 - segment.Y1));
                if (segment.X2 < segment.X1 - 1)
                    metrics.BackwardEdges++;
            }

            var crossingPoints = new List<(double x, double y)>();
            for (var i = 0; i < segments.Count; i++)
            {
                for (var j = i + 1; j < segments.Count; j++)
                {
                    var a = segments[i];
                    var b = segments[j];
                    // Wires sharing a port only diverge, so skip them. Wires that merely share a
                    // node still cross when their far ends are in the opposite order.
                    if (a.From == b.From && a.FromSlot == b.FromSlot)
                        continue;
                    if (a.To == b.To && a.ToSlot == b.ToSlot)
                        continue;
                    // Cheap rejection first: most pairs in a wide graph are nowhere near.
                    if (Math.Max(a.X1, a.X2) < Math.Min(b.X1, b.X2) || Math.Max(b.X1, b.X2) < Math.Min(a.X1, a.X2) ||
                        Math.Max(a.Y1, a.Y2) < Math.Min(b.Y1, b.Y2) || Math.Max(b.Y1, b.Y2) < Math.Min(a.Y1, a.Y2))
                        continue;
                    if (!Intersect(a, b, out var px, out var py))
                        continue;
                    metrics.Crossings++;
                    crossingPoints.Add((px, py));
                    var angle = AngleBetween(a, b);
                    if (angle < ShallowAngleDegrees)
                        metrics.ShallowCrossings++;
                    metrics.Implicated.Add(a.From);
                    metrics.Implicated.Add(a.To);
                    metrics.Implicated.Add(b.From);
                    metrics.Implicated.Add(b.To);
                    metrics.Detail.Add($"crossing at ({px:0}, {py:0}) {angle:0}deg: " +
                                       $"{Label(doc, a.From)}->{Label(doc, a.To)} x {Label(doc, b.From)}->{Label(doc, b.To)}");
                }
            }

            metrics.CrossingClusters = CountClusters(crossingPoints);

            foreach (var segment in segments)
            {
                foreach (var node in nodes)
                {
                    if (node.ObjectId == segment.From || node.ObjectId == segment.To)
                        continue;
                    // A wire into a block runs past the other blocks in the stack. That's normal.
                    if (node.IsBlock)
                        continue;
                    if (rects.TryGetValue(node.ObjectId, out var body) && WireHitsRect(segment, body))
                    {
                        metrics.WiresOverNodes++;
                        // Where the wire runs across this node, so a repair knows the gap to open.
                        var t1 = (body.X - segment.X1) / Math.Max(1e-6, segment.X2 - segment.X1);
                        var t2 = (body.Right - segment.X1) / Math.Max(1e-6, segment.X2 - segment.X1);
                        var ya = segment.Y1 + Math.Clamp(t1, 0, 1) * (segment.Y2 - segment.Y1);
                        var yb = segment.Y1 + Math.Clamp(t2, 0, 1) * (segment.Y2 - segment.Y1);
                        metrics.Obstructions.Add((node.ObjectId, segment.From, Math.Min(ya, yb), Math.Max(ya, yb)));
                        metrics.Implicated.Add(node.ObjectId);
                        metrics.Implicated.Add(segment.From);
                        metrics.Implicated.Add(segment.To);
                        metrics.Detail.Add($"wire {Label(doc, segment.From)}->{Label(doc, segment.To)} " +
                                           $"passes over {Label(doc, node.ObjectId)}");
                    }
                }
            }

            MeasureGroups(doc, rects, metrics);
            return metrics;
        }

        // Measured off a live Shader Graph window: a context draws its first block 2 px in and
        // stacks the rest with no gap.
        const double k_ContextHeader = 2;
        const double k_BlockGap = 0;
        const double k_ColumnGutter = 96;
        const double k_RowGap = 44;

        /// <summary>
        /// Where each node actually draws. Block nodes store a zero rect, so their boxes are
        /// rebuilt from the context position and their order in the stack.
        /// </summary>
        public static Dictionary<string, Rect> Rects(ShaderGraphDocument doc, IReadOnlyDictionary<string, Rect> boxes = null)
        {
            var rects = new Dictionary<string, Rect>(StringComparer.Ordinal);
            foreach (var node in doc.Nodes)
            {
                if (boxes != null && boxes.TryGetValue(node.ObjectId, out var given))
                {
                    rects[node.ObjectId] = given;
                    continue;
                }
                if (!node.IsBlock)
                {
                    rects[node.ObjectId] = NodeSizer.Measure(node);
                    continue;
                }
                rects[node.ObjectId] = default;
            }

            foreach (var vertex in new[] { true, false })
            {
                var context = doc.RootNode[vertex ? "m_VertexContext" : "m_FragmentContext"];
                var x = ShaderWaitress.Serialization.Json.Number(context?["m_Position"]?["x"], 0);
                var y = ShaderWaitress.Serialization.Json.Number(context?["m_Position"]?["y"], 0);
                var index = 0;
                foreach (var id in doc.ContextBlocks(vertex))
                {
                    var node = doc.NodeById(id);
                    if (node == null)
                        continue;
                    if (boxes == null || !boxes.ContainsKey(id))
                    {
                        // The stored position is where Unity draws the block; an unconnected
                        // input's value editor extends the box to the left of it.
                        var literal = NodeSizer.LiteralWidth(doc, node);
                        rects[id] = new Rect(x - literal, y + k_ContextHeader + index * (NodeSizer.BlockHeight + k_BlockGap),
                            NodeSizer.BlockWidth + literal, NodeSizer.BlockHeight);
                    }
                    index++;
                }
            }
            return rects;
        }

        static void MeasureGroups(ShaderGraphDocument doc, Dictionary<string, Rect> rects, LayoutMetrics metrics)
        {
            var boxes = new List<(SgGroup group, Rect rect)>();
            foreach (var group in doc.Groups)
            {
                var members = doc.Nodes.Where(n => n.GroupId == group.ObjectId && rects.ContainsKey(n.ObjectId)).ToList();
                if (members.Count == 0)
                    continue;
                var minX = members.Min(n => rects[n.ObjectId].X);
                var minY = members.Min(n => rects[n.ObjectId].Y);
                var maxX = members.Max(n => rects[n.ObjectId].Right);
                var maxY = members.Max(n => rects[n.ObjectId].Bottom);
                var box = new Rect(minX, minY, maxX - minX, maxY - minY);
                boxes.Add((group, box));
                var rectangles = members.Select(n => rects[n.ObjectId]).ToList();
                metrics.GroupGaps += Holes(rectangles.Select(r => (r.X, r.Right)), k_ColumnGutter);
                metrics.GroupGaps += Holes(rectangles.Select(r => (r.Y, r.Bottom)), k_RowGap);
            }

            for (var i = 0; i < boxes.Count; i++)
            {
                for (var j = i + 1; j < boxes.Count; j++)
                {
                    if (boxes[i].rect.Overlaps(boxes[j].rect, -1))
                        metrics.GroupOverlaps++;
                }
            }

            foreach (var (group, rect) in boxes)
            {
                foreach (var node in doc.Nodes)
                {
                    if (node.GroupId == group.ObjectId || !rects.TryGetValue(node.ObjectId, out var box))
                        continue;
                    if (box.Overlaps(rect, -1))
                        metrics.NodesOutsideGroup++;
                }
            }
        }

        /// <summary>
        /// Total length of the bands one axis of a group's box covers that no member reaches,
        /// ignoring gaps no wider than the normal spacing between neighbours. Zero for a group
        /// laid out as an ordinary chain, however wide that chain is.
        /// </summary>
        static double Holes(IEnumerable<(double start, double end)> spans, double allowance)
        {
            var total = 0.0;
            var reached = double.NegativeInfinity;
            foreach (var (start, end) in spans.OrderBy(s => s.start))
            {
                if (reached > double.NegativeInfinity && start - reached > allowance)
                    total += start - reached - allowance;
                reached = Math.Max(reached, end);
            }
            return total;
        }

        static int CountClusters(List<(double x, double y)> points)
        {
            var used = new bool[points.Count];
            var clusters = 0;
            for (var i = 0; i < points.Count; i++)
            {
                if (used[i])
                    continue;
                var count = 1;
                for (var j = i + 1; j < points.Count; j++)
                {
                    if (used[j])
                        continue;
                    var dx = points[i].x - points[j].x;
                    var dy = points[i].y - points[j].y;
                    if (dx * dx + dy * dy <= CrossingClusterRadius * CrossingClusterRadius)
                    {
                        used[j] = true;
                        count++;
                    }
                }
                if (count >= 3)
                    clusters++;
            }
            return clusters;
        }

        static bool Intersect(Segment a, Segment b, out double px, out double py)
        {
            px = py = 0;
            var r1 = a.X2 - a.X1;
            var r2 = a.Y2 - a.Y1;
            var s1 = b.X2 - b.X1;
            var s2 = b.Y2 - b.Y1;
            var denominator = r1 * s2 - r2 * s1;
            if (Math.Abs(denominator) < 1e-9)
                return false;
            var t = ((b.X1 - a.X1) * s2 - (b.Y1 - a.Y1) * s1) / denominator;
            var u = ((b.X1 - a.X1) * r2 - (b.Y1 - a.Y1) * r1) / denominator;
            if (t <= 0 || t >= 1 || u <= 0 || u >= 1)
                return false;
            px = a.X1 + t * r1;
            py = a.Y1 + t * r2;
            return true;
        }

        static double AngleBetween(Segment a, Segment b)
        {
            var ax = a.X2 - a.X1;
            var ay = a.Y2 - a.Y1;
            var bx = b.X2 - b.X1;
            var by = b.Y2 - b.Y1;
            var la = Math.Sqrt(ax * ax + ay * ay);
            var lb = Math.Sqrt(bx * bx + by * by);
            if (la < 1e-9 || lb < 1e-9)
                return 90;
            var cos = Math.Clamp((ax * bx + ay * by) / (la * lb), -1, 1);
            var degrees = Math.Acos(cos) * 180 / Math.PI;
            return Math.Min(degrees, 180 - degrees);
        }

        static bool Crosses(Segment s, double x1, double y1, double x2, double y2) =>
            Intersect(s, new Segment { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2 }, out _, out _);

        /// <summary>
        /// Whether the wire Shader Graph actually draws touches this box. The wire is a cubic with
        /// horizontal tangents at both ends, so a steep wire bulges sideways near its ports. The
        /// straight line would miss it clipping the corner of a node beside one of its ports.
        /// </summary>
        static bool WireHitsRect(Segment s, Rect r)
        {
            var bulge = k_WireTangent;
            var c1x = s.X1 + bulge;
            var c2x = s.X2 - bulge;

            // The curve stays inside the hull of its control points, so reject on that box first.
            if (Math.Max(Math.Max(s.X1, s.X2), Math.Max(c1x, c2x)) < r.X ||
                Math.Min(Math.Min(s.X1, s.X2), Math.Min(c1x, c2x)) > r.Right ||
                Math.Max(s.Y1, s.Y2) < r.Y || Math.Min(s.Y1, s.Y2) > r.Bottom)
                return false;

            var px = s.X1;
            var py = s.Y1;
            for (var i = 1; i <= k_WireSamples; i++)
            {
                var t = (double)i / k_WireSamples;
                var u = 1 - t;
                var x = u * u * u * s.X1 + 3 * u * u * t * c1x + 3 * u * t * t * c2x + t * t * t * s.X2;
                var y = u * u * u * s.Y1 + 3 * u * u * t * s.Y1 + 3 * u * t * t * s.Y2 + t * t * t * s.Y2;
                if (SegmentHitsRect(new Segment { X1 = px, Y1 = py, X2 = x, Y2 = y }, r))
                    return true;
                px = x;
                py = y;
            }
            return false;
        }

        static bool SegmentHitsRect(Segment s, Rect r)
        {
            if (Math.Max(s.X1, s.X2) < r.X || Math.Min(s.X1, s.X2) > r.Right ||
                Math.Max(s.Y1, s.Y2) < r.Y || Math.Min(s.Y1, s.Y2) > r.Bottom)
                return false;
            // A sample that lands inside counts even when no edge is crossed.
            if (s.X1 >= r.X && s.X1 <= r.Right && s.Y1 >= r.Y && s.Y1 <= r.Bottom)
                return true;
            // Measure runs thousands of times per layout, so this allocates nothing.
            return Crosses(s, r.X, r.Y, r.Right, r.Y) ||
                   Crosses(s, r.Right, r.Y, r.Right, r.Bottom) ||
                   Crosses(s, r.Right, r.Bottom, r.X, r.Bottom) ||
                   Crosses(s, r.X, r.Bottom, r.X, r.Y);
        }

        public string Describe()
        {
            var sb = new StringBuilder();
            sb.Append("score=").Append(Score.ToString("0", CultureInfo.InvariantCulture));
            sb.Append(" overlaps=").Append(NodeOverlaps);
            if (Crowded > 0)
                sb.Append(" crowded=").Append(Crowded);
            sb.Append(" backward=").Append(BackwardEdges);
            sb.Append(" crossings=").Append(Crossings);
            sb.Append(" shallow=").Append(ShallowCrossings);
            sb.Append(" clusters=").Append(CrossingClusters);
            sb.Append(" over-nodes=").Append(WiresOverNodes);
            if (EditorOverlaps > 0)
                sb.Append(" editor-overlaps=").Append(EditorOverlaps);
            if (ContextsMisaligned > 0)
                sb.Append(" stacks-apart");
            sb.Append(" group-overlaps=").Append(GroupOverlaps);
            sb.Append(" intruders=").Append(NodesOutsideGroup);
            if (GroupGaps > 0)
                sb.Append(" group-gap=").Append(GroupGaps.ToString("0", CultureInfo.InvariantCulture));
            sb.Append(" wire=").Append(WireLength.ToString("0", CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}
