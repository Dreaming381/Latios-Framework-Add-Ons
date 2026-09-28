# Peekaboo Implementation Notes

These notes are for anyone working on Peekaboo's code. If you just want to use
the add-on, the [package README](../../README.md) is what you want.

Occlusion culling can make two kinds of mistakes, and they aren't equal. Culling
something that's hidden is the whole point. Culling something that's *visible* is
a bug the player sees right away, and no amount of performance makes up for it.
Most of the design choices in here give up a little culling to make the second
mistake impossible. These notes explain where those choices are and why each one
is safe.

## The Rules

-   [The Conservative Rule](The%20Conservative%20Rule.md) – The one thing that
    must always hold, and the four places that enforce it
-   [Plate Validity](Plate%20Validity.md) – The rules a plate has to follow, and
    how a plate knows which viewpoints it's safe to use from
-   [Validation](Validation.md) – How the add-on gets tested, and what each test
    can and can't catch

## Baking

-   [Plates](Plates.md) – What a plate is, why it isn't a triangle, and the two
    ways plates get made
-   [Surface Plates](Surface%20Plates.md) – Plates cut from the flat faces of a
    mesh
-   [Interior Plates](Interior%20Plates.md) – Plates cut through the middle of a
    mesh, which is how a sphere gets an occluder
-   [Baking Pipeline](Baking%20Pipeline.md) – How baking is organized, and which
    meshes get skipped

## Runtime

-   [Views](Views.md) – How Peekaboo builds its own projection from a culling
    pass's planes
-   [Rasterizing](Rasterizing.md) – How plates are drawn into the depth buffer
    without ever claiming too much
-   [Culling](Culling.md) – Picking occluders, the depth pyramid, and shadow
    cascades
-   [Performance](Performance.md) – What was measured and what came of it

## Future Work

-   [Deferred Work](Deferred%20Work.md) – Things that were left for later, and
    what each would take
