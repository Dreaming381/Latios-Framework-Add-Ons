---
name: terrainwaitress
description: Read and change Unity terrains from the command line - ground height, slope, splat weights, holes, grass and trees at any point or area; problems with a terrain or how tiles meet (seams, shared data, collider on other data, invalid prototypes, floating trees); and edits to heights, paint, holes, trees, details, layers, prototypes, resolutions, size and Terrain component fields. Use instead of reading TerrainData (binary) or scene YAML.
---

# terrainwaitress

`terrainwaitress <command> ...` answers questions about the Unity project's terrains. TerrainData is binary, so this is
the way to see it. Run `terrainwaitress help` or `help set` for everything.

## Naming terrains and places

- `<terrain>` is one of:
  - a TerrainData asset (`TW_West`, `Assets/.../X.asset`): coordinates are metres from the asset's corner;
  - a Terrain object, as `Scene.unity:Name` or a name unique in the project: world coordinates;
  - a scene or prefab file: all of its terrains, where a point finds the tile it lies on.
- Points are `x,z`, or a percentage of the terrain (`50%,50%`). Areas are `all`, `x0,z0..x1,z1` or `x,z~radius`.
  - Add `+f` for f metres of eased falloff.
  - Join several areas into one with `;`, and leave one out with `!`: `all;!0,120..256,136+2` (the `+2` keeps 2 m away).
  - Keep only matching ground with conditions after `|`: `all|slope>30`, `0,0..50,50|y<12`, `all|layer[Road]<0.5`.
    Conditions read the terrain as earlier changes in the same `set` left it.
- Heights you read and set are world y on a Terrain object, and metres above the base on an asset.

## Read

- `terrainwaitress info` lists every terrain and where it is placed.
- `info <terrain> [--all]` shows:
  - size, height range and how many samples are clipped at the height limit;
  - grid spacings, and the layers with their coverage;
  - trees and details per prototype, and holes;
  - placement, material, pixel error and collider.

  For a scene it also shows how the tiles meet. `--all` adds every layer, prototype and component field.
- `sample <terrain> 120,80 10,10..60,60` takes points and areas:
  - a point gives `y`, slope, downhill direction, splat weights, holes, detail density and nearby trees;
  - an area gives its height range (with where the lowest and highest points are), slope spread (the share under 5,
    15 and 30°), layer coverage, holes, trees and details.
- `flat <terrain> 20x30 [--max-slope 5] [--in area]` finds spots of that size where every slope is under the limit,
  flattest first. It also prints the command that levels the best one.
- `check [terrain...]` lists problems, each with a `fix:` command. Exit code 1 means warnings or errors. It catches:
  - floating trees, trees standing in holes, and invalid tree or detail prototypes;
  - missing, untextured or unused layers, and weights that don't add up to 100%;
  - peaks clipped at the height limit;
  - two Terrains sharing one TerrainData, and a collider using different data;
  - seams, gaps and resolution mismatches between tiles;
  - a built-in material under URP or HDRP (renders pink).

## Change (an Editor must have the project open)

`terrainwaitress set <terrain>... change... [--dry-run]`. The tool reports every mistake at once, and changes nothing
until all of them are fixed. `--dry-run` shows exactly what the change would do. Unity's Undo can revert it.

- `height[area]=y`, `+=d`, `-=d`, `=smooth[:n]`: `height[50,50~10+8]=20` makes a 10 m plateau with 8 m slopes.
  `height[x0,z0..x1,z1+4]=10..18` is a ramp (a road) from the first corner's end to the second's.
  `heights=@file.raw` imports a heightmap. Trees on changed ground move with it.
- `layer[Grass][area]=0.7` paints a layer's weight, and the other layers share the rest.
  - Add, remove or swap layers with `layers+=Assets/.../X.terrainlayer`, `layers-=Name` and `layers[Name]=...`.
  - `layers=normalize` makes every texel's weights add up to 100%.
  - `layer[Grass].tileSize=10,10` edits the layer asset itself, which every terrain using it shares.
- `holes[area]=true` cuts holes and `=false` fills them.
- `details[Grass][area]=4` (and `+=`, `-=`) sets detail density. Manage detail prototypes with
  `detailPrototypes+=X.png` or `X.prefab`, `detailPrototypes-=Name`, and `detail[Name].minHeight=..`.
- Trees:
  - `trees[Proto]+=x,z;x,z` places trees on the ground, and `trees[Proto]+=area:count[:seed]` scatters them;
  - `trees-=area`, `trees-=holes` and `trees[Proto]-=all` remove them;
  - `trees=snap` puts trees back on the ground, and `trees=prune` drops trees whose prototype is gone;
  - manage prototypes with `treePrototypes+=X.prefab` and `treePrototypes-=Name`.
- `size=x,y,z` or `size.y=..`: heights keep their fraction of size.y, so changing it scales the ground.
  `heightLimit=80` instead changes the limit and keeps the ground where it is.
- `heightmapResolution=`, `alphamapResolution=`, `detailResolution=` and `detailPatch=` resample the data. Unity's own
  setters would clear it and change the size.
- Settings: `wavingGrassStrength`, `wavingGrassAmount`, `wavingGrassSpeed`, `wavingGrassTint`, `baseMapResolution`,
  `enableHolesTextureCompression`.
- On a Terrain object:
  - `terrain.pixelError=3`, `terrain.material=X.mat`, and any other field from `info --all`;
  - `terrain.data=X.asset`, which the collider follows;
  - `collider.TerrainData=X.asset`.

  The tool saves the scene. It refuses a scene that has unsaved changes.
- `stitch` on a scene (or on several Terrain objects) moves every shared edge to the average height of the two tiles.

Other commands:

- `new Assets/.../T.asset [size=1000,600,1000] [heightmapResolution=513]` creates a TerrainData.
- `copy <terrain> Assets/.../New.asset` copies one. Naming a Terrain object also points it and its collider at the copy.
- `export <terrain> file.raw` writes the heightmap as 16-bit raw.

Records are exported automatically. Scenes are read from disk, so save a scene in the Editor before asking about it.
After an Undo in the Editor, run `terrainwaitress refresh --force`. Undo changes only the Editor's memory, not the file.
