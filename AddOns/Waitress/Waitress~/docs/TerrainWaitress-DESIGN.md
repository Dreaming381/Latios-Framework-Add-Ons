# TerrainWaitress design

This document covers what TerrainWaitress answers, how it gets its data, the
Unity rules it reproduces (and how each was measured), how the tool was
validated, and what it doesn't support.

## What it answers

An agent working on a level asks about the ground in metres and degrees:

-   the height at a point;
-   whether a spot is walkable;
-   where there is a flat site;
-   whether two tiles meet;
-   why something floats or falls through.

It changes terrains with numbers too: flatten this rectangle to 12 m, ramp
this road from 10 to 18 m, paint rock above 30°, scatter 50 trees but not on
the road.

Every answer and every edit is numeric, and either Unity or `sample` can check
it. None of them needs a human to look at a render. Brushes, noise and erosion
are judged by eye, so the tool leaves them out.

## Why the Editor exports records

TerrainData is always serialized in binary form, even in Force Text projects.
It is the main object (file id 15600000) of a `.asset` file, and its
alphamaps are separate Texture2D sub-assets.

Reading the file offline would mean reimplementing Unity's binary serializer.
Instead, the Editor exports one record per asset:

-   **`Terrains/<guid>.json`** holds:
    -   size and resolutions;
    -   settings, with the defaults of a new TerrainData;
    -   layers, with every TerrainLayer setting;
    -   tree and detail prototypes, with every public setting and with Unity's
        own internal `Validate(out string)` verdict;
    -   detail instance counts from `ComputeDetailInstanceTransforms`.
-   **`Terrains/<guid>.bin`** holds the arrays in TerrainData's own row-major
    layout, with rows along z:
    -   heights as Unity's 0..32766 steps;
    -   holes as 1 for surface and 0 for a hole;
    -   splat weights as bytes, one plane per layer;
    -   detail densities as ushort;
    -   trees as 36 bytes each.

A record is current while the asset's and its `.meta`'s write times match the
stamp in it. Any command re-exports stale records through the Editor first.
With no Editor, the last export is used with a note. Records of deleted assets
are pruned on every export.

Terrain components are read from scene and prefab YAML on disk. The tool takes
class 218 (Terrain) and 154 (TerrainCollider) and works out the world position
through the parent transforms. That makes reading a scene's terrains instant,
and means no scene has to be opened.

## Measured rules

The probes are in `AgentScripts/TerrainWaitress/`. They were run on Unity
6000.3.24.

-   **Height storage.** Heights are stored as `round(h × 32766)` and read back as
    `k / 32766f`; 1.0 is stored as 32766. A record keeps those steps, so the tool
    rebuilds every height exactly.
-   **GetInterpolatedHeight.** Each heightmap cell is split along its (0,0)-(1,1)
    diagonal, and the height is linear on each triangle. This matched Unity to
    6e-6 m. Bilinear interpolation was off by 0.20 m, and the other diagonal by
    0.40 m.
-   **GetInterpolatedNormal.**
    -   Each sample's normal is a Sobel normal, `normalize(-dX/cellX, 8,
        -dZ/cellZ)`, blended bilinearly between a cell's four corners and then
        normalized.
    -   Past an edge, heights are extrapolated linearly from the edge
        (`2·h(edge) − h(inside)`); at corners, along the diagonal.
    -   Clamping the coordinates instead was 0.088 off at the border. The corner
        rule took a separate probe.
-   **GetSteepness** is `acos(normal.y)` in degrees.
-   **Splat sampling.** The URP terrain shader puts alphamap texel i at
    `i/(res−1)` (`TerrainLitPasses.hlsl`:
    `splatUV = (uv * (size − 1) + 0.5) / size`). So weights at a point are
    bilinear between texel centres on that grid. Holes cover one heightmap
    cell each, and a detail cell covers `size/detailResolution`.
-   **SetHeights re-snaps trees.** After `SetHeights`, a tree's y equals the
    ground: `GetInterpolatedHeight / size.y`. The tool predicts this for the
    changed rectangle. A resample re-snaps every tree.
-   **Unity's resolution setters destroy data.** Setting `heightmapResolution`
    clears the heights and changes `size`, because `heightmapScale` is kept.
    The alphamap and detail resolution setters clear their data too. So the
    tool resamples everything itself:
    -   heights with Unity's own interpolation;
    -   weights bilinearly, then renormalized to add up to 255;
    -   holes and details from the nearest cell.

    It then sends the full arrays and sets `size` again.
-   **Tree prototypes.** Unity rejects a tree prefab unless its *root*
    object has a MeshRenderer or LODGroup ("contains no valid mesh renderer").
    A prefab with renderers only on its children is invalid. The "must use the
    Nature/Soft Occlusion shader" message comes back with a *valid* verdict.
    It matters only to the built-in pipeline, so the tool shows it there only.
-   **Alphamaps can be lost when a TerrainData is created.** If a new TerrainData
    is painted (`SetAlphamaps`) before `AssetDatabase.CreateAsset`, the weights
    don't survive. Paint after the asset exists. The scenario builder hit this.
-   **Scenes go dirty for no reason.** Reassigning a Terrain's TerrainData marks
    its scene dirty a frame after the save, but the content doesn't change: a
    re-save is byte-identical. Before refusing a dirty scene, the Editor script
    saves a copy to `Temp/` and compares it with the file on disk.
-   **Undo groups.** `Undo.PerformUndo` reverts the whole current group. In a
    scripted session that group can include every earlier edit, so one failed
    edit reverted all the previous ones. Each apply now opens its own group,
    reverts only that group with `RevertAllDownToGroup`, and closes it. Bad
    assets are also rejected before anything changes.

## Verification

-   **Evaluation.** The oracle (`AgentScripts/TerrainWaitress/EvalOracle.cs`)
    writes Unity's `GetInterpolatedHeight`, `GetInterpolatedNormal` and
    `GetSteepness` for 6,000 points per TerrainData: random points, every edge,
    the corners, and exact sample points. `terrainwaitress evalcheck` compares
    them across 12 terrains, including ones edited by the tool and by the
    subagents:
    -   heights: 72,000 of 72,000 within 0.1 mm (worst 0.013 mm);
    -   normals: 72,000 of 72,000 within 1e-5 (worst 2.5e-6);
    -   slope: 71,983 within 0.01°, and the rest within 0.02°. The rest sit near
        flat ground, where acos magnifies a 1e-6 difference in the normal.
-   **Edits.** After every real `set`, the tool re-exports the asset and
    compares Unity's arrays with its own prediction: heights, weights, tree
    counts and tree heights. It prints a note on any difference. No difference
    appeared in the validation runs, which covered:
    -   flatten, ramp, raise and smooth;
    -   paint and layer changes;
    -   holes and details;
    -   tree placement, scatter and removal;
    -   resolution changes and `heightLimit`;
    -   stitching.
-   **Self-tests.** `terrainwaitress selftest` runs 8 offline tests of parsing,
    areas, flattening, painting, layer removal, resampling and scatter.

## Subagent rounds

Each round ran one subagent at a time.

**Round 1 (productivity).** All 6 tasks were answered correctly. Fixed
afterwards:

-   painting by slope: area conditions `|slope>30` and `|y<12`;
-   `;` unions inside area brackets;
-   the wrong fix suggested for clipped heights: `heightLimit=`, which keeps
    the ground in place;
-   missing `fix:` lines;
-   the error/warning count in `info`;
-   notes hidden from `check`;
-   tree positions in area samples (`--all`);
-   the new `flat` command;
-   repaint bounds in the output;
-   the location of the maximum slope on flat areas.

**Round 2 (exploratory, 62 calls).** Fixed afterwards:

-   the undo-group revert above (critical);
-   height edits after `size.y=` or `heightLimit=` in the same `set`, which
    used the old scale;
-   Terrain component fields: now checked in dry-run, with old values and
    did-you-mean;
-   prototype file types, checked before the Editor is called;
-   trees left floating after a resample;
-   `flat`, which missed areas and returned windows that were too large;
-   crashes on tiny raw files and on export to a missing folder;
-   `new` accepting zero sizes and invalid resolutions;
-   NaN and Infinity heights, negative radii, and bad seeds;
-   scatter messages;
-   holes outside the terrain, now an error;
-   `export` to a non-raw file name.

Added after round 2: ramps (`height[a..b]=y0..y1`), layer-weight conditions,
and did-you-mean for change names.

**Round 3 (verification).** All 5 tasks were done in 18 calls: a new terrain
with a mound, a ramped road with rock painted off the road, 200 trees kept off
the road and steep ground, and a levelled pad. Fixed afterwards:

-   the `heightLimit` suggestion, now based on the height actually needed;
-   `!area` to leave an area out (`all;!road+2`);
-   `flat` now skips spots touching the terrain's edge, and says when a spot is
    already level;
-   "did you mean" for a wrong terrain path;
-   `heightLimit=` accepted by `new`;
-   a duplicate warning.

Left open: a height edit doesn't warn when it breaks earlier rules, such as new
steep ground left unpainted or a tree moved onto a slope. Re-run `check` or the
same conditional changes after leveling.

## Known unsupported

-   **Artistic sculpting and painting**, such as brushes, noise, erosion and
    Terrain Tools stamps. Their results need a human to judge them.
-   **Terrains inside prefab instances in a scene.** The tool reports them,
    but reads only the prefab itself.
-   **Unsaved scene changes.** Scenes are read from disk. The Editor script
    refuses to change a scene that has real unsaved changes.
-   **Detail instance positions.** The tool reports densities, and Unity's
    instance count where `ComputeDetailInstanceTransforms` gives one. In
    CoverageMode that API returned 0 for painted billboard grass in this
    project, so densities are shown first.
-   **Material checks** go only as far as "built-in shader under an SRP".
    Whether a custom terrain shader suits the pipeline is not checked.
-   **Neighbours.** Seams are compared between tiles whose edges touch, in the
    same file. Auto-connect is reported from Grouping ID and Auto Connect
    flags; Unity's runtime neighbour assignment isn't simulated. `stitch` makes
    edges match only at shared samples when the resolutions differ.
-   **Detail resampling** keeps per-cell values. In InstanceCountMode, that
    changes the instances per square metre.
-   **Area conditions** read the ground as earlier changes in the same `set`
    left it, not as the final result of that `set`.
-   **Other Unity versions** were not measured. The interpolation and Sobel
    rules come from 6000.3.24.
-   **Undo isn't seen by the records.** `Undo.PerformUndo` after a `set`
    reverts exactly that set and leaves the edit before it intact (tested).
    But an Undo changes only the Editor's memory and not the file on disk, so
    run `terrainwaitress refresh --force` after undoing in the Editor.
