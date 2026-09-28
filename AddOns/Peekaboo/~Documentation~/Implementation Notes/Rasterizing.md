# Rasterizing

The rasterizer works one row at a time and one plate at a time, and a pixel only
ever claims what covers all of it.

## A Plate's Span in a Row

For a convex polygon, a thin vertical strip at `x` that spans one pixel row is
inside the polygon exactly when both the top and the bottom of the strip are. So
four numbers describe a plate's part of a row: where its left and right edges
cross the top of the row, and where they cross the bottom.

```csharp
struct PlateSpan
{
    float  leftAtTop,    rightAtTop;
    float  leftAtBottom, rightAtBottom;
    float2 gradient;  float constant;   // Closeness as a line in screen space
}
```

From those, `innerMin = max(leftAtTop, leftAtBottom)` and
`innerMax = min(rightAtTop, rightAtBottom)` give the x range the plate covers
across the full height of the row on its own. Pixels `ceil(innerMin)` through
`floor(innerMax) - 1` are fully covered by this plate alone.

Keeping all four numbers, not just the range they share, is what makes welding
work. More on that below.

## A Pixel's Depth

Closeness changes linearly across a plate in screen space (see
[Views](Views.md)). The value a pixel can safely claim is the *minimum* closeness
over the pixel, which is always at one of its corners. The sign of the gradient
tells you which corner, so for a whole row the safe value becomes a single line:

```csharp
rampBase = constant + gradient.y * (gradient.y < 0 ? row + 1 : row)
                    + (gradient.x < 0 ? gradient.x : 0)
value(x) = rampBase + gradient.x * x
```

That's one multiply-add per pixel with no branches, which is both cheap and easy
for Burst to vectorize.

The buffer keeps the **max** across plates. Each plate's value is a threshold
(anything farther away is hidden), and the strongest threshold wins.

## Two Passes

Almost every covered pixel is fully covered by a single plate, and those only cost
the ramp. Only the pixels no single plate covers need welding, and there are just
a few of those along each seam. So the rasterizer runs in two passes:

1.  Every plate writes its own fully covered range.
2.  The welding pass only looks at pixels the first pass left empty. That's a
    quick compare against negative infinity, since that's what an untouched pixel
    still holds.

Skipping pixels the first pass already wrote isn't just faster, it's correct. A
single plate's claim is always at least as strong as a welded one, because a
plate that fully covers the pixel is also one of the plates the welded value
takes the minimum over.

## Welding

When two plates share an edge, like two faces of a box, the pixels along that
seam aren't fully covered by either one alone. They are covered by the pair
together, and it's safe to say so, but only if there's truly no gap between them.

Here's the test. Plate B follows plate A with no gap when B's left edge is at or
left of A's right edge at *both* the top and the bottom of the row. Checking just
those two points is enough for the whole row. For a convex polygon, the left
boundary curves one way and the right boundary curves the other, so the gap
between them is largest at the top or bottom of the row.

Comparing only the fully covered ranges wouldn't work. A seam that drifts sideways
across the row would look like a gap as wide as the drift. That's why all four
crossings are kept.

Spans get sorted and walked in order. A chain of plates, each following the one
before it, covers everything from the first plate's range to the farthest any of
them reaches. A pixel in that run that no single plate covers takes the
*farthest* depth among the plates touching it, since every part of the pixel is
in front of at least one of them.

**Welding visits each plate's edges, not each pixel's plates.** A pixel still
empty after the first pass isn't fully covered by any plate, so every plate that
touches it touches it with an edge. So each plate only visits the pixels between
its outer and inner crossings, and keeps a running minimum per pixel. That finds
the same plates as checking every plate in the run for every empty pixel, but it
stays linear when a row near the horizon chains hundreds of plates together. A bit
per pixel marks which pixels an edge reached, so writing the results back only
visits those, not the whole run.

**The one tolerance.** Two plates cut from the same mesh share an edge exactly,
but each reaches it through its own math, so they disagree in the last few bits.
The comparison allows a thousandth of a pixel of slack. Without it, every shared
edge would look like a gap. With it, the worst possible over-claim is a sliver a
thousandth of a pixel wide, which is far smaller than anything a real render
target can show.

## Clipping

Under a perspective projection, a plate gets clipped against the near plane in
view space. Then it gets projected and clipped to the screen rectangle. Clipping
to the screen costs nothing correctness-wise, since the parts of a plate outside
the view can't hide anything the view renders, and it keeps coordinates bounded.
Both clips are Sutherland-Hodgman, so a convex polygon stays convex.

Clipping a polygon against one edge can add a vertex, and a plate can be clipped
against five edges: the near plane and the four sides of the screen. So buffers
holding a clipped plate are sized to `kMaxClippedVertices`, which is five more
than the most vertices a plate can have.

A plate whose plane passes through the eye (under perspective), or lies parallel
to the view direction (under orthographic), has no linear depth and gets dropped.
It's exactly edge-on, so it wouldn't cover any pixels anyway.
