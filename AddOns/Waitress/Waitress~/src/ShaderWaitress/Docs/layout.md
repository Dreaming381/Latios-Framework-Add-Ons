LAYOUT

The tool owns every node and group position. You never give it a coordinate.
Layout runs on its own after any structural edit, and you can also run it
directly:

  shaderwaitress layout <graph> [--report] [--verbose] [--force] [--no-refine]
                                [--remeasure] [--gutter 96] [--row-gap 44]

  --report     print the score before and after
  --verbose    the report, plus every crossing and every wire drawn over a
               node, so you can fix what layout can't (usually by regrouping
               or rewiring)
  --force      lay the whole graph out from scratch instead of only placing
               new nodes. It still never writes a worse layout.
  --no-refine  skip the final hill-climb, which is the slow part
  --remeasure  ignore stored node sizes and estimate everything, which helps
               after changing a node's ports

WHAT IT AIMS FOR

  1. Flow reads left to right. An output never travels left to its input.
  2. Nodes never overlap. Only wires may cross wires.
  3. Few crossings, and the ones left cross at clear angles, not nearly
     parallel.
  4. No three wires cross at the same spot.
  5. Groups stay apart from each other and wrap their members cleanly.
  6. Wires don't run across nodes they have nothing to do with.

WHAT TO EXPECT

  Adding to an arranged graph doesn't rearrange it. If the existing layout is
  sound, only the nodes without a position get placed, and everything else
  stays exactly where it was. So it's safe to edit a graph someone spent time
  arranging.

  A good layout gets repaired, not replaced. If one node sits under a wire in
  an otherwise good layout, the tool slides it (or the wire's source) out of
  the way and leaves the rest alone.

  A worse layout is never written. If what's already in the file scores better
  than anything the tool comes up with, the file is left alone and the tool
  says so.

  Groups that feed each other force one wire to run backward, however the graph
  is arranged. The tool tells you which groups, since regrouping is the only
  real fix.

  Big graphs get less effort. Above 60 nodes the slowest searches are cut
  short, so a large graph can end up short of its best layout, but never worse
  than it started.

  Messy human-made graphs load fine: backward wires, overlapping nodes, nodes
  far outside their group. Layout only happens when you ask for it or after an
  edit, and --no-layout on any edit keeps the current positions.

READING THE REPORT

  before: score=41200 overlaps=6 backward=2 crossings=31 shallow=9 ...
  after:  score=2100  overlaps=0 backward=0 crossings=12 shallow=1 ...

Lower is better. The score is a weighted sum where overlaps and backward wires
cost the most, then wires drawn over nodes and shallow crossings, then plain
crossings, with total wire length as a tie-breaker. `crowded=N` counts node
pairs closer than 24 px, which can look like an overlap on screen.

--verbose names each problem:

  crossing at (3268, 748) 79deg: n44b1 Property->Smoothness x nd8e4 SubGraph->BaseColor
  wire nd8e4 SubGraph->SurfaceDescription.BaseColor passes over n64e1 SampleTexture2D

MAKING A DENSE GRAPH READABLE

`node set <graph> "<selector>" --preview off` collapses preview swatches. Each
one takes about 184 px off its node, and it's the cheapest way to tidy a graph
full of preview nodes. Layout accounts for it right away.

NODE SIZES

Shader Graph measures nodes when it draws them and writes the size into the
file, so a stored size is always trusted. Nodes the Editor hasn't drawn yet get
an estimate. Layout only ever writes positions, never sizes, so a guess never
gets mistaken for a measurement. Unity fills in real sizes the next time it
opens the graph.
