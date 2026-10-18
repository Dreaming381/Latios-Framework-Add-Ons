using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TerrainWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("terrainwaitress — Unity terrains: ground height, slope and paint at any point or area, what's wrong with a terrain");
            w.WriteLine("or how tiles meet, and changes to heights, paint, holes, trees, grass, layers and settings");
            w.WriteLine();
            w.WriteLine("Answers come from a per-asset export the Editor writes (UserSettings/Waitress/TerrainWaitress/), refreshed");
            w.WriteLine("automatically when an Editor has the project open; scenes are read from disk.");
            w.WriteLine("<terrain> is a TerrainData asset (coordinates are metres from its corner), a Terrain object as Scene.unity:Name");
            w.WriteLine("or a unique object name (world coordinates), or a scene/prefab (all its terrains; points find their tile).");
            w.WriteLine("Points are x,z (or 25%,50% of the terrain); areas are all, x0,z0..x1,z1, or x,z~radius, with +f for f metres");
            w.WriteLine("of falloff, several joined with ';' (one area), !area to leave one out (all;!0,120..256,136+2), and conditions after");
            w.WriteLine("'|': all|slope>30, 0,0..50,50|y<12, all|layer[Road]<0.5.");
            w.WriteLine();
            w.WriteLine("read");
            w.WriteLine("  info [terrain...] [--all]    size, height range, grids, layers and their coverage, trees and details per");
            w.WriteLine("                               prototype, holes, where it is placed, Terrain settings, problem count.");
            w.WriteLine("                               A scene also shows how its tiles meet. No argument: every terrain in Assets/");
            w.WriteLine("  sample <terrain> <point|area>... [--all]");
            w.WriteLine("                               a point: ground y, slope, downhill direction, splat weights, hole, grass,");
            w.WriteLine("                               nearby trees; an area: height range with where, slope spread (share under");
            w.WriteLine("                               5/15/30°), layer coverage, holes, trees and details inside");
            w.WriteLine("  flat <terrain> <W>x<D> [--max-slope 5] [--in area] [--count 5]");
            w.WriteLine("                               places of that size where every slope is under the limit, flattest first,");
            w.WriteLine("                               with the command that levels the best one");
            w.WriteLine("  check [terrain...]           problems with the fix command: trees floating or in holes, invalid tree or");
            w.WriteLine("                               detail prototypes, missing or unpainted layers, unnormalized weights, peaks cut");
            w.WriteLine("                               at the height limit, shared TerrainData, collider on other data, seams and gaps");
            w.WriteLine("                               between tiles, pink material. Exit 1 when there are errors or warnings");
            w.WriteLine();
            w.WriteLine("change (need the Editor)");
            w.WriteLine("  set <terrain>... change... [--dry-run]");
            w.WriteLine("                               heights, paint, holes, trees, details, layers, prototypes, size, resolutions,");
            w.WriteLine("                               Terrain component fields, stitch seams (`help set`)");
            w.WriteLine("  new <Assets/.../Name.asset> [size=x,y,z] [heightmapResolution=513] [alphamapResolution=512] [detailResolution=1024]");
            w.WriteLine("  copy <terrain> <Assets/.../New.asset>   copies the data; naming a Terrain object points it (and its collider)");
            w.WriteLine("                               at the copy");
            w.WriteLine("  export <terrain> <file.raw>  the heightmap as 16-bit little-endian raw (re-import: set ... heights=@file.raw)");
            w.WriteLine("  refresh [terrain|folder...] [--force]   export now (normally automatic)");
            w.WriteLine();
            w.WriteLine("  --project <dir> picks the Unity project");
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("set", @"Changing terrains

  set <terrain>... change... [--dry-run]   every change is checked first and every mistake is reported at once;
  --dry-run prints what would change (computed exactly as the real change) and changes nothing. Changes apply in
  order. Heights, coordinates and areas are in the terrain's frame: world for a Terrain object or scene, metres from the
  corner for an asset. Naming a scene applies area changes to every tile the area touches. Unity's Undo reverts it.

  heights      height[area]=y        flatten to y (world y for a placed terrain)
               height[area]+=d / -=d raise or lower by d metres
               height[area]=smooth[:n]   average with neighbours, n passes
               heights=@file.raw     import a 16-bit little-endian raw heightmap (row 0 at z=0; resampled if needed)
               area+f eases the change out over f metres: height[50,50~10+8]=20 is a 10 m plateau with 8 m slopes.
               height[x0,z0..x1,z1+f]=y0..y1 ramps from y0 at the first corner's end to y1 at the second's, along the
               rectangle's longer side (a road).
               Conditions limit any area to matching ground as earlier changes in the same set left it: slope>30,
               y<12, layer[Road]<0.5. layer[Rock][all|slope>30]=1, height[all|y>55]=55, trees-=all|layer[Road]>0.3.
               Trees under changed heights move onto the new ground, as Unity does.
  paint        layer[Name][area]=w   set a layer's weight (0..1; += and -= too); other layers share the rest
  layers       layers+=Assets/.../X.terrainlayer   add (weight 0; the first layer gets 100%)
               layers-=Name          remove; the others share its weight.  layers[Name]=Assets/.../Y.terrainlayer swaps
               the asset, keeping the weights.  layers=normalize makes every texel add up to 100%
               layer[Name].tileSize=10,10 (any TerrainLayer setting; the .terrainlayer is shared by every terrain using it)
  holes        holes[area]=true cuts holes, =false fills them (cells whose centre is inside)
  details      details[Name][area]=n (+= -=)   density per detail cell (CoverageMode 0..255, InstanceCountMode 0..16)
               detailPrototypes+=Assets/.../grass.png (billboard) or Assets/.../Rock.prefab (mesh); detailPrototypes-=Name
               detail[Name].minHeight=0.5  (any DetailPrototype setting: min/maxWidth, min/maxHeight, density,
               healthyColor, dryColor, noiseSpread, renderMode, useInstancing, ...)
  trees        trees[Proto]+=x,z;x,z      place on the ground       trees[Proto]+=area:count[:seed]   scatter
               trees-=area / trees-=holes / trees[Proto]-=area|all   remove
               trees=snap  onto the ground      trees=prune  drop trees whose prototype is gone
               treePrototypes+=Assets/.../Tree.prefab / treePrototypes-=Name (removes its trees); tree[Name].bendFactor=..
  size         size=x,y,z or size.y=..   heights keep their fraction of size.y, so changing it scales the ground
               heightLimit=80   a new size.y with the ground kept where it is (more room above, same terrain)
  resolutions  heightmapResolution=257 (2^n+1), alphamapResolution=256, detailResolution=512, detailPatch=16: resampled,
               not cleared (Unity's own setters clear the data and change size)
  settings     wavingGrassStrength, wavingGrassAmount, wavingGrassSpeed, wavingGrassTint, baseMapResolution,
               enableHolesTextureCompression
  component    terrain.Field=value on a Terrain object: any field `info --all` lists (HeightmapPixelError,
               SplatMapDistance, DrawInstanced, GroupingID, AllowAutoConnect, ...), or terrain.material=Assets/.../X.mat,
               terrain.data=Assets/.../X.asset (the collider follows), collider.TerrainData=..., collider.Enabled=false.
               The scene is saved; one open with unsaved changes is refused
  stitch       on a scene (or several Terrain objects): moves every shared edge to the average of both tiles' heights"),
            ("export", @"Where answers come from

  The Editor writes UserSettings/Waitress/TerrainWaitress/Terrains/<guid>.json (settings, layers, prototypes with
  Unity's own validity checks, detail instance counts) and <guid>.bin (heights, holes, splat weights, detail densities,
  trees) per TerrainData. A record is current while the asset's and its .meta's write times match; any command
  re-exports stale ones through the Editor first, and with no Editor the last export is used with a note. Terrain
  components are read from the scene and prefab files on disk, so save a scene before asking about it.
  Values are evaluated as TerrainData does: GetInterpolatedHeight (each cell split along its 00-11 diagonal),
  GetInterpolatedNormal (Sobel normals, linearly extrapolated past the edges, blended bilinearly) and GetSteepness;
  splat weights bilinearly between texel centres, as the terrain shaders sample them."),
        };

        public static void Topic(TextWriter w, string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                Index(w);
                return;
            }
            var hit = s_Topics.FirstOrDefault(t => string.Equals(t.name, topic, StringComparison.OrdinalIgnoreCase));
            if (hit.name != null)
            {
                w.WriteLine(hit.text);
                return;
            }
            if (CommandRouter.Commands.Contains(topic, StringComparer.OrdinalIgnoreCase))
            {
                var index = new StringWriter();
                Index(index);
                var mine = false;
                foreach (var line in index.ToString().Split('\n'))
                {
                    var t = line.TrimEnd('\r');
                    if (t.StartsWith("  ", StringComparison.Ordinal) && !t.StartsWith("   ", StringComparison.Ordinal))
                        mine = t.Substring(2).StartsWith(topic + " ", StringComparison.Ordinal) || t.Substring(2) == topic;
                    else if (!t.StartsWith("   ", StringComparison.Ordinal))
                        mine = false;
                    if (mine)
                        w.WriteLine(t);
                }
                if (topic is "set")
                    w.WriteLine($"see `help {topic}`");
                return;
            }
            throw new WaitressException($"no help topic '{topic}'; topics: {string.Join(", ", s_Topics.Select(t => t.name))}, or a command name");
        }

        public static int Skill(string install, TextWriter w)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TerrainWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream);
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
