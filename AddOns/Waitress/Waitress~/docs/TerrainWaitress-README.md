# TerrainWaitress

TerrainWaitress is a command-line tool for Unity terrains. It answers what the
ground is like at a point or over an area, what is wrong with a terrain or with
how its tiles meet, and where there is room for something flat. It also edits
heights, paint, holes, trees, details, layers, prototypes, resolutions and
Terrain component fields. It's built for AI agents, and it's the eighth Waitress
tool.

Questions about a terrain are usually these:

-   What is the ground height at this spot, and can a character walk there?
-   Where is there a flat 20 by 30 m area for a building?
-   Why is there a crack between these two tiles?
-   Why do some trees float, or fall through the world?
-   Why does the player fall through the terrain here? (Usually because the
    collider uses different data.)
-   Paint rock on everything steeper than 30°, and keep the forest off the road.

An agent can't read any of this directly. TerrainData is always saved in binary
form. Where a terrain sits, and which collider and material it uses, is stored
in scene YAML. Ground height between samples, normals and slope are things Unity
computes. TerrainWaitress brings them together and answers most questions in
one call.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/TerrainWaitress/release/terrainwaitress[.exe]`.

It needs the Editor open with the Pipeline package (0.7 or newer):

-   once per change to a TerrainData, to export it;
-   for edits.

Everything else works offline. The tool carries its Editor script inside
itself, writes it to `UserSettings/Waitress/TerrainWaitress/`, and runs it
through `run_script`, so nothing is imported into the project. Install the agent
skill with
`terrainwaitress skill --install <project>/.claude/skills/terrainwaitress`.

## Naming terrains, points and areas

`<terrain>` can be any of these:

-   **A TerrainData asset** (`TW_West`, `Assets/.../X.asset`): coordinates are
    metres from the asset's corner.
-   **A Terrain object**, as `Scene.unity:Name` or as a name unique in the
    project: world coordinates.
-   **A scene or prefab**: all of its terrains, and a point finds the tile it
    lies on.

Points are `x,z`, or a percentage of the terrain such as `50%,50%`.

Areas are `all`, a rectangle `x0,z0..x1,z1` or a circle `x,z~r`. Each can
take extras:

-   `+f` gives f metres of eased falloff.
-   `;` joins several areas into one.
-   Conditions after `|` keep only matching ground: `slope>30`, `y<12`, or
    `layer[Road]<0.5`.

## Commands

| Command | What it gives |
|---|---|
| `info [terrain...] [--all]` | Size and height range, with samples clipped at the limit. Also: grid spacings, layer coverage, trees and details per prototype, holes, placement, material, pixel error, collider. For a scene, how its tiles meet. |
| `sample <terrain> <point\|area>...` | **Point:** y, slope, downhill direction, splat weights, holes, detail density, nearby trees. **Area:** height range and where its extremes are, slope spread, coverage, holes, trees (`--all` lists them), details. |
| `flat <terrain> WxD [--max-slope 5] [--in area]` | Places of that size where every slope is under the limit, flattest first, with the command that levels the best one. |
| `check [terrain...]` | Problems, each with a `fix:` command (list below). |
| `set <terrain>... change... [--dry-run]` | Edits (`help set`); details below. |
| `new`, `copy`, `export`, `refresh` | Create a TerrainData; copy one, and point a Terrain and its collider at the copy; write the heightmap as 16-bit raw; export now. |

`check` reports:

-   floating trees, and trees standing in holes;
-   invalid tree or detail prototypes;
-   missing, untextured or unpainted layers;
-   splat weights that don't add up to 100%;
-   peaks clipped at the height limit;
-   two Terrains sharing one TerrainData;
-   a collider that uses different data from its Terrain;
-   seams, gaps and resolution mismatches between tiles;
-   a built-in-pipeline material under URP or HDRP.

`set` is atomic: the tool reports every mistake at once and changes nothing
until all of them are fixed. It computes each change offline, so `--dry-run`
prints exactly what the real run will do. It then writes the changed
rectangles back through the TerrainData API with an Undo entry, and re-exports
the asset. If Unity's result differed from the preview, it says so.

## Examples

```
terrainwaitress sample Main.unity:Island 120,80          # y 21.32, slope 47.9°, downhill +x-z, Grass 100%
terrainwaitress flat Island 20x30                        # flattest 20 x 30 m spots + the command to level one
terrainwaitress set Island "layer[Rock][all|slope>30]=1" "trees-=all|slope>30"
terrainwaitress set Island "height[20,100..180,110+4]=10..18" "trees-=20,100..180,110+2"   # a ramped road
terrainwaitress set Main.unity stitch                    # make every shared tile edge match
terrainwaitress check Main.unity
```

## Limits

Read the "Known unsupported" section of
[TerrainWaitress-DESIGN.md](TerrainWaitress-DESIGN.md). In short, the tool
leaves out:

-   artistic sculpting and painting;
-   terrains inside prefab instances;
-   unsaved scene changes;
-   Terrain Tools brushes and stamps;
-   detail instance positions;
-   HDRP- and URP-specific material checks beyond "built-in shader".
