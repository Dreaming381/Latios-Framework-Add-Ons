using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderWaitress.Layout
{
    public sealed class LayoutItem
    {
        public string Key;
        public double Width;
        public double Height;
        public double X;
        public double Y;
        public bool IsSink;         // pinned to the rightmost column
        public double Seed;         // initial vertical ordering hint, usually the existing y
        public bool IsDummy;
    }

    public sealed class LayoutLink
    {
        public int From;
        public int To;
        public double FromAnchor;   // y offset of the source port inside the source item
        public double ToAnchor;
    }

    public sealed class LayoutOptions
    {
        public double ColumnGutter = 96;
        public double RowGap = 44;
        public double DummyWidth = 1;
        public double DummyHeight = 1;
        public int OrderingSweeps = 16;
        public int TransposePasses = 6;
        /// <summary>Order by the port a wire attaches to rather than by node alone.</summary>
        public bool PortAwareOrdering = true;
    }

    /// <summary>
    /// Sugiyama-style layered layout, rotated so rank 0 is the rightmost column and every
    /// edge points right-to-left in rank space (an output therefore never travels left on
    /// screen). Long edges are routed through dummy items so they occupy an empty lane rather
    /// than passing over node bodies.
    /// </summary>
    public sealed class LayeredLayout
    {
        readonly List<LayoutItem> m_Items;
        readonly List<LayoutLink> m_Links;
        readonly LayoutOptions m_Options;

        int[] m_Rank;
        List<List<int>> m_Layers;
        List<LayoutLink> m_Routed;          // links after dummy insertion
        readonly List<LayoutItem> m_All = new List<LayoutItem>();

        public IReadOnlyList<LayoutItem> Items => m_All;
        public int Crossings { get; private set; }

        public LayeredLayout(List<LayoutItem> items, List<LayoutLink> links, LayoutOptions options = null)
        {
            m_Items = items;
            m_Links = links;
            m_Options = options ?? new LayoutOptions();
        }

        public void Run()
        {
            m_All.Clear();
            m_All.AddRange(m_Items);
            if (m_All.Count == 0)
                return;

            AssignRanks();
            InsertDummies();
            BuildLayers();
            OrderLayers();
            AssignCoordinates();
            Crossings = CountCrossings();
        }

        // ---- ranking ----

        void AssignRanks()
        {
            var n = m_Items.Count;
            m_Rank = new int[n];

            // Ranking needs a DAG, but two groups that feed each other form a cycle between their
            // units. Left in, the cycle's members would be ranked before their successors and
            // land in the rightmost column. Drop the edges that close cycles. Those wires run
            // backward either way.
            var cyclic = FeedbackArcs(n);
            var ranking = cyclic.Count == 0 ? m_Links : m_Links.Where(l => !cyclic.Contains(l)).ToList();
            var outgoing = BuildAdjacency(ranking, forward: true, n);

            // Kahn from sources, so the reverse pass sees final successor ranks first.
            var indegree = new int[n];
            foreach (var link in ranking)
            {
                if (link.From != link.To)
                    indegree[link.To]++;
            }
            var order = new List<int>();
            var ready = new Stack<int>(Enumerable.Range(0, n).Where(i => indegree[i] == 0));
            var placed = new bool[n];
            while (ready.Count > 0)
            {
                var i = ready.Pop();
                if (placed[i])
                    continue;
                placed[i] = true;
                order.Add(i);
                foreach (var j in outgoing[i])
                {
                    if (--indegree[j] == 0)
                        ready.Push(j);
                }
            }
            for (var i = 0; i < n; i++)
            {
                if (!placed[i])
                    order.Add(i);           // cyclic remainder, appended in index order
            }

            for (var k = order.Count - 1; k >= 0; k--)
            {
                var i = order[k];
                var rank = 0;
                foreach (var j in outgoing[i])
                {
                    if (j != i)
                        rank = Math.Max(rank, m_Rank[j] + 1);
                }
                m_Rank[i] = rank;
            }

            var sinkRank = 0;
            for (var i = 0; i < n; i++)
            {
                if (m_Items[i].IsSink)
                    sinkRank = Math.Max(sinkRank, m_Rank[i]);
            }
            for (var i = 0; i < n; i++)
            {
                if (m_Items[i].IsSink)
                    m_Rank[i] = sinkRank;
            }

        }

        /// <summary>
        /// Edges that close a cycle, found by depth-first search: an edge into a node still on the
        /// stack is a back edge. Removing them leaves a DAG. It's not the minimum feedback arc set,
        /// which is NP-hard, but the graphs are small and one of the wires has to run backward
        /// anyway.
        /// </summary>
        HashSet<LayoutLink> FeedbackArcs(int n)
        {
            var back = new HashSet<LayoutLink>();
            var edges = BuildLinkAdjacency(n);
            var state = new byte[n];        // 0 unvisited, 1 on the stack, 2 done
            var stack = new Stack<(int node, int next)>();
            for (var root = 0; root < n; root++)
            {
                if (state[root] != 0)
                    continue;
                state[root] = 1;
                stack.Push((root, 0));
                while (stack.Count > 0)
                {
                    var (node, next) = stack.Pop();
                    if (next >= edges[node].Count)
                    {
                        state[node] = 2;
                        continue;
                    }
                    stack.Push((node, next + 1));
                    var link = edges[node][next];
                    if (link.From == link.To)
                    {
                        back.Add(link);
                        continue;
                    }
                    if (state[link.To] == 1)
                        back.Add(link);
                    else if (state[link.To] == 0)
                    {
                        state[link.To] = 1;
                        stack.Push((link.To, 0));
                    }
                }
            }
            return back;
        }

        List<List<LayoutLink>> BuildLinkAdjacency(int n)
        {
            var adjacency = new List<List<LayoutLink>>(n);
            for (var i = 0; i < n; i++)
                adjacency.Add(new List<LayoutLink>());
            foreach (var link in m_Links)
                adjacency[link.From].Add(link);
            return adjacency;
        }

        static List<List<int>> BuildAdjacency(List<LayoutLink> links, bool forward, int n)
        {
            var adjacency = new List<List<int>>(n);
            for (var i = 0; i < n; i++)
                adjacency.Add(new List<int>());
            foreach (var link in links)
            {
                if (forward)
                    adjacency[link.From].Add(link.To);
                else
                    adjacency[link.To].Add(link.From);
            }
            return adjacency;
        }

        // ---- dummies ----

        void InsertDummies()
        {
            m_Routed = new List<LayoutLink>();
            var ranks = new List<int>(m_Rank);
            foreach (var link in m_Links)
            {
                var fromRank = ranks[link.From];
                var toRank = ranks[link.To];
                var span = fromRank - toRank;
                if (span <= 1)
                {
                    m_Routed.Add(link);
                    continue;
                }

                var previous = link.From;
                var previousAnchor = link.FromAnchor;
                for (var r = fromRank - 1; r > toRank; r--)
                {
                    var dummy = new LayoutItem
                    {
                        Key = null,
                        Width = m_Options.DummyWidth,
                        Height = m_Options.DummyHeight,
                        IsDummy = true,
                        Seed = m_All[link.From].Seed,
                    };
                    m_All.Add(dummy);
                    ranks.Add(r);
                    var index = m_All.Count - 1;
                    m_Routed.Add(new LayoutLink { From = previous, To = index, FromAnchor = previousAnchor, ToAnchor = 0 });
                    previous = index;
                    previousAnchor = 0;
                }
                m_Routed.Add(new LayoutLink { From = previous, To = link.To, FromAnchor = previousAnchor, ToAnchor = link.ToAnchor });
            }
            m_Rank = ranks.ToArray();
        }

        void BuildLayers()
        {
            var maxRank = m_Rank.Length == 0 ? 0 : m_Rank.Max();
            m_Layers = new List<List<int>>();
            for (var r = 0; r <= maxRank; r++)
                m_Layers.Add(new List<int>());
            for (var i = 0; i < m_All.Count; i++)
                m_Layers[m_Rank[i]].Add(i);

            // Seeding from the current positions keeps a human's layout close to where it was.
            foreach (var layer in m_Layers)
                layer.Sort((a, b) => m_All[a].Seed.CompareTo(m_All[b].Seed));

            // Crossing counting runs thousands of times during transposition, so bucket the
            // links by the rank pair they span once instead of rescanning every link.
            m_LinksByRankPair = new List<(int, int)>[m_Layers.Count];
            foreach (var link in m_Routed)
            {
                var upper = m_Rank[link.From];
                var lower = m_Rank[link.To];
                if (upper != lower + 1)
                    continue;
                m_LinksByRankPair[upper] ??= new List<(int, int)>();
                m_LinksByRankPair[upper].Add((link.From, link.To));
            }
        }

        List<(int from, int to)>[] m_LinksByRankPair;

        // ---- ordering ----

        void OrderLayers()
        {
            var successors = BuildAdjacency(m_Routed, forward: true, m_All.Count);
            var predecessors = BuildAdjacency(m_Routed, forward: false, m_All.Count);

            var best = Snapshot();
            var bestCrossings = CountCrossings();

            // Two starting points: the order seeded from the file, and plain insertion order.
            // Neither wins consistently, so both are optimized and the better one is kept.
            Optimize(successors, predecessors, ref best, ref bestCrossings);
            foreach (var layer in m_Layers)
                layer.Sort();
            Optimize(successors, predecessors, ref best, ref bestCrossings);
            Restore(best);
        }

        void Optimize(List<List<int>> successors, List<List<int>> predecessors, ref List<List<int>> best, ref int bestCrossings)
        {
            var current = CountCrossings();
            if (current < bestCrossings)
            {
                bestCrossings = current;
                best = Snapshot();
            }

            for (var sweep = 0; sweep < m_Options.OrderingSweeps; sweep++)
            {
                // Layers are indexed by rank and edges run from high rank to low rank, so an
                // item's predecessors sit in layer r+1 and its successors in layer r-1. Each
                // sweep orders one layer against the neighbouring layer already fixed.
                if (sweep % 2 == 0)
                {
                    for (var r = m_Layers.Count - 2; r >= 0; r--)
                        SortByMedian(m_Layers[r], m_Layers[r + 1], downward: false);
                }
                else
                {
                    for (var r = 1; r < m_Layers.Count; r++)
                        SortByMedian(m_Layers[r], m_Layers[r - 1], downward: true);
                }

                for (var pass = 0; pass < m_Options.TransposePasses; pass++)
                {
                    if (!Transpose())
                        break;
                }

                var crossings = CountCrossings();
                if (crossings < bestCrossings)
                {
                    bestCrossings = crossings;
                    best = Snapshot();
                }
            }
            Restore(best);
        }

        /// <summary>
        /// Median barycenter over the neighboring layer. A neighbor's position is its index plus
        /// how far down it the wire attaches, so two wires between the same pair of nodes get
        /// ordered by port. That keeps multi-port nodes from crossing their own wires.
        /// </summary>
        void SortByMedian(List<int> layer, List<int> reference, bool downward)
        {
            var position = new Dictionary<int, int>();
            for (var i = 0; i < reference.Count; i++)
                position[reference[i]] = i;

            var samples = new Dictionary<int, List<double>>();
            foreach (var item in layer)
                samples[item] = new List<double>();

            foreach (var link in m_Routed)
            {
                var self = downward ? link.From : link.To;
                var other = downward ? link.To : link.From;
                if (!samples.TryGetValue(self, out var bucket) || !position.TryGetValue(other, out var index))
                    continue;
                var otherHeight = Math.Max(1, m_All[other].Height);
                var otherAnchor = downward ? link.ToAnchor : link.FromAnchor;
                var selfHeight = Math.Max(1, m_All[self].Height);
                var selfAnchor = downward ? link.FromAnchor : link.ToAnchor;
                // Subtracting the port's own offset keeps a wire that leaves low on this node
                // from dragging the whole node down.
                bucket.Add(m_Options.PortAwareOrdering
                    ? index + Math.Clamp(otherAnchor / otherHeight, 0, 1) - Math.Clamp(selfAnchor / selfHeight, 0, 1)
                    : index);
            }

            var medians = new Dictionary<int, double>();
            for (var i = 0; i < layer.Count; i++)
            {
                var values = samples[layer[i]];
                values.Sort();
                medians[layer[i]] = values.Count == 0 ? i : Median(values);
            }

            var original = new Dictionary<int, int>();
            for (var i = 0; i < layer.Count; i++)
                original[layer[i]] = i;
            layer.Sort((a, b) =>
            {
                var c = medians[a].CompareTo(medians[b]);
                return c != 0 ? c : original[a].CompareTo(original[b]);
            });
        }

        static double Median(List<double> values)
        {
            var n = values.Count;
            if (n % 2 == 1)
                return values[n / 2];
            return (values[n / 2 - 1] + values[n / 2]) * 0.5;
        }

        bool Transpose()
        {
            var improved = false;
            for (var r = 0; r < m_Layers.Count; r++)
            {
                var layer = m_Layers[r];
                for (var i = 0; i + 1 < layer.Count; i++)
                {
                    var before = LocalCrossings(r);
                    (layer[i], layer[i + 1]) = (layer[i + 1], layer[i]);
                    var after = LocalCrossings(r);
                    if (after < before)
                        improved = true;
                    else
                        (layer[i], layer[i + 1]) = (layer[i + 1], layer[i]);
                }
            }
            return improved;
        }

        int LocalCrossings(int rank)
        {
            var total = 0;
            if (rank + 1 < m_Layers.Count)
                total += CrossingsBetween(rank + 1, rank);
            if (rank - 1 >= 0)
                total += CrossingsBetween(rank, rank - 1);
            return total;
        }

        int CountCrossings()
        {
            var total = 0;
            for (var r = m_Layers.Count - 1; r > 0; r--)
                total += CrossingsBetween(r, r - 1);
            return total;
        }

        /// <summary>Counts inversions between the two layers an edge set spans.</summary>
        int CrossingsBetween(int upperRank, int lowerRank)
        {
            var links = upperRank < m_LinksByRankPair.Length ? m_LinksByRankPair[upperRank] : null;
            if (links == null || links.Count == 0)
                return 0;
            var upperIndex = IndexMap(m_Layers[upperRank]);
            var lowerIndex = IndexMap(m_Layers[lowerRank]);
            var pairs = new List<(int a, int b)>(links.Count);
            foreach (var (from, to) in links)
            {
                if (upperIndex.TryGetValue(from, out var a) && lowerIndex.TryGetValue(to, out var b))
                    pairs.Add((a, b));
            }
            pairs.Sort((x, y) => x.a != y.a ? x.a.CompareTo(y.a) : x.b.CompareTo(y.b));
            var crossings = 0;
            for (var i = 0; i < pairs.Count; i++)
            {
                for (var j = i + 1; j < pairs.Count; j++)
                {
                    if (pairs[i].b > pairs[j].b)
                        crossings++;
                }
            }
            return crossings;
        }

        static Dictionary<int, int> IndexMap(List<int> layer)
        {
            var map = new Dictionary<int, int>(layer.Count);
            for (var i = 0; i < layer.Count; i++)
                map[layer[i]] = i;
            return map;
        }

        List<List<int>> Snapshot() => m_Layers.Select(l => new List<int>(l)).ToList();

        /// <summary>Copies, so a later in-place sort cannot corrupt the snapshot it came from.</summary>
        void Restore(List<List<int>> snapshot)
        {
            m_Layers = snapshot.Select(l => new List<int>(l)).ToList();
        }

        // ---- coordinates ----

        void AssignCoordinates()
        {
            // X: rank 0 is rightmost, so walk left as rank increases.
            var columnWidth = new double[m_Layers.Count];
            for (var r = 0; r < m_Layers.Count; r++)
                columnWidth[r] = m_Layers[r].Count == 0 ? 0 : m_Layers[r].Max(i => m_All[i].Width);

            var x = 0.0;
            for (var r = 0; r < m_Layers.Count; r++)
            {
                foreach (var i in m_Layers[r])
                    m_All[i].X = x - m_All[i].Width;
                x -= columnWidth[r] + m_Options.ColumnGutter;
            }

            // Y: pack each layer, then pull items toward the average of their neighbours.
            foreach (var layer in m_Layers)
                PackLayer(layer);

            var outgoing = new List<LayoutLink>[m_All.Count];
            var incoming = new List<LayoutLink>[m_All.Count];
            foreach (var link in m_Routed)
            {
                (outgoing[link.From] ??= new List<LayoutLink>()).Add(link);
                (incoming[link.To] ??= new List<LayoutLink>()).Add(link);
            }

            for (var iteration = 0; iteration < 8; iteration++)
            {
                var downward = iteration % 2 == 0;
                for (var r = 0; r < m_Layers.Count; r++)
                {
                    var index = downward ? r : m_Layers.Count - 1 - r;
                    Align(m_Layers[index], downward ? outgoing : incoming, downward);
                    PackLayer(m_Layers[index]);
                }
            }

            Normalize();
        }

        void PackLayer(List<int> layer)
        {
            var y = double.NegativeInfinity;
            foreach (var i in layer)
            {
                var item = m_All[i];
                if (double.IsNegativeInfinity(y))
                    y = item.Y;
                else if (item.Y < y)
                    item.Y = y;
                y = item.Y + item.Height + m_Options.RowGap;
            }
        }

        /// <summary>
        /// Pulls each item toward the position that makes its wires horizontal. The target is
        /// computed on PORT anchors, not node centres, because a wire leaves and arrives at a
        /// port; aligning centres is what leaves near-parallel wires crossing each other.
        /// </summary>
        void Align(List<int> layer, List<LayoutLink>[] links, bool downward)
        {
            foreach (var i in layer)
            {
                var item = m_All[i];
                var bucket = links[i];
                if (bucket == null || bucket.Count == 0)
                    continue;
                var sum = 0.0;
                foreach (var link in bucket)
                {
                    sum += downward
                        ? m_All[link.To].Y + link.ToAnchor - link.FromAnchor
                        : m_All[link.From].Y + link.FromAnchor - link.ToAnchor;
                }
                var desired = sum / bucket.Count;
                item.Y = item.Y * 0.3 + desired * 0.7;
            }
        }

        void Normalize()
        {
            if (m_All.Count == 0)
                return;
            var minX = m_All.Min(i => i.X);
            var minY = m_All.Min(i => i.Y);
            foreach (var item in m_All)
            {
                item.X -= minX;
                item.Y -= minY;
            }
        }
    }
}
