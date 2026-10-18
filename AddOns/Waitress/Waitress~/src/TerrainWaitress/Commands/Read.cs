using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TerrainWaitress.Model;

namespace TerrainWaitress.Commands
{
    public static class Read
    {
        // ------------------------------------------------------------ info

        public static int Info(Session s, Args a, TextWriter w)
        {
            var all = a.Flag("all");
            if (a.Positional.Count == 0)
                return Overview(s, w);
            var first = true;
            foreach (var what in a.Positional)
            {
                var targets = s.Targets(what);
                foreach (var t in targets)
                {
                    if (!first)
                        w.WriteLine();
                    first = false;
                    Describe(s, t, all, w);
                }
                if (targets.Count > 1 && targets.All(t => t.World))
                    Neighbours(targets, w);
            }
            return 0;
        }

        static int Overview(Session s, TextWriter w)
        {
            var files = s.Catalog.All.Where(f => f.AssetPath.StartsWith("Assets/", StringComparison.Ordinal)).ToList();
            if (files.Count == 0)
            {
                w.WriteLine("no TerrainData assets in Assets/ (create one with `terrainwaitress new Assets/.../Name.asset`)");
                return 0;
            }
            var records = s.Catalog.Get(files, s.Notes);
            foreach (var r in records)
            {
                var placed = s.Catalog.PlacementsOf(r.Guid);
                w.WriteLine($"{r.Path}  {Fmt.M(r.Size[0])} x {Fmt.M(r.Size[2])} m, {r.HeightRes} heightmap, {Fmt.Plural(r.Layers.Count, "layer")}, {Fmt.Plural(r.Trees.Length, "tree")}" +
                            (placed.Count == 0 ? "; not placed" : "; in " + string.Join(", ", placed.Select(p => p.Ref))));
            }
            foreach (var n in s.Catalog.PlacementNotes)
                w.WriteLine("note: " + n);
            return 0;
        }

        static void Describe(Session s, Target t, bool all, TextWriter w)
        {
            var r = t.Data;
            var range = Analysis.HeightRange(r);
            if (t.World)
            {
                var p = t.Placement;
                w.WriteLine($"{p.HierarchyPath} in {p.File}: {r.Path}{(r.Stale ? " (stale export)" : "")}");
                w.WriteLine($"  placed    at {Fmt.Vec(p.Position)}; covers x {Fmt.M(t.OX)}..{Fmt.M(t.OX + t.SX)}, z {Fmt.M(t.OZ)}..{Fmt.M(t.OZ + t.SZ)}; ground y {Fmt.M(t.OY + range.min)}..{Fmt.M(t.OY + range.max)}" +
                            (p.Active && p.Enabled ? "" : p.Active ? " (Terrain disabled)" : " (inactive)"));
                var mat = p.MaterialGuid == null ? "pipeline default" : s.Catalog.AssetPathOf(p.MaterialGuid) ?? (p.MaterialGuid == "0000000000000000f000000000000000" ? "a built-in material" : p.MaterialGuid);
                w.WriteLine($"  terrain   material {mat}, pixel error {p.Fields.GetValueOrDefault("HeightmapPixelError")}, basemap distance {p.Fields.GetValueOrDefault("SplatMapDistance")}, " +
                            $"instanced {(p.Fields.GetValueOrDefault("DrawInstanced") == "1" ? "on" : "off")}, group {p.GroupingId}{(p.AutoConnect ? "" : " (auto connect off)")}, " +
                            $"collider {(!p.HasCollider ? "none" : p.ColliderDataGuid != p.DataGuid ? "DIFFERENT DATA" : p.ColliderEnabled ? "yes" : "disabled")}");
                if (all)
                    w.WriteLine("            " + string.Join(", ", p.Fields.Where(f => f.Key != "TerrainData").Select(f => f.Key + " " + f.Value)));
            }
            else
                w.WriteLine($"{r.Name}  {r.Path}{(r.Stale ? " (stale export)" : "")}");
            w.WriteLine($"  size      {Fmt.M(r.Size[0])} x {Fmt.M(r.Size[2])} m, height limit {Fmt.M(r.Size[1])} m; heights {Fmt.M(range.min)}..{Fmt.M(range.max)} m above the base (mean {Fmt.M(range.mean)})" +
                        (Analysis.AtTop(r) > 0 ? $", {Fmt.Count(Analysis.AtTop(r))} at the limit" : ""));
            w.WriteLine($"  grids     heightmap {r.HeightRes} ({Fmt.M(r.Size[0] / (r.HeightRes - 1))} m), splat {r.AlphaRes} ({Fmt.M(r.Size[0] / (r.AlphaRes - 1))} m), " +
                        $"details {r.DetailRes} per {r.DetailPatch} ({Fmt.M(r.Size[0] / r.DetailRes)} m), holes {r.HolesRes}, basemap {r.SettingNumber("baseMapResolution")}");
            var coverage = Analysis.Coverage(r);
            w.WriteLine("  layers    " + (r.Layers.Count == 0 ? "none" : string.Join(", ", r.Layers.Select((l, i) => $"{l.Name} {Fmt.Pct(i < coverage.Length ? coverage[i] : 0)}"))));
            if (all)
                foreach (var l in r.Layers.Where(l => !l.Missing))
                    w.WriteLine($"            {l.Name}: {l.Asset}; {Props(l)}");
            var byProto = r.Trees.GroupBy(x => x.Prototype).ToDictionary(g => g.Key, g => g.Count());
            w.WriteLine($"  trees     {Fmt.Count(r.Trees.Length)}" + (r.TreePrototypes.Count == 0 ? " (no prototypes)" :
                        ": " + string.Join(", ", r.TreePrototypes.Select((p, i) => $"{p.Name} {byProto.GetValueOrDefault(i)}{(p.Valid ? "" : " (INVALID)")}"))));
            if (all)
                foreach (var p in r.TreePrototypes)
                    w.WriteLine($"            {p.Name}: {Props(p)}");
            w.WriteLine("  details   " + (r.DetailPrototypes.Count == 0 ? "none" : string.Join(", ", r.DetailPrototypes.Select((p, i) =>
                $"{p.Name} ({(p.Props.TryGetValue("usePrototypeMesh", out var m) && m.ValueKind == JsonValueKind.True ? "mesh" : "texture")}) painted in {Fmt.Count(Analysis.DetailCells(r, i))} cells, density sum {Fmt.Count(Analysis.DetailTotal(r, i))}" +
                (p.Instances > 0 ? $" ({Fmt.Count(p.Instances)} instances)" : "")))) +
                        (r.DetailPrototypes.Count > 0 ? $"; {r.DetailScatter}" : ""));
            if (all)
                foreach (var p in r.DetailPrototypes)
                    w.WriteLine($"            {p.Name}: {Props(p)}");
            var holes = r.HoleCount;
            if (holes > 0)
                w.WriteLine($"  holes     {Fmt.Count(holes)} cells ({Fmt.Pct(holes / (double)r.Holes.Length)})");
            var changed = r.Settings.Where(x => !r.Defaults.TryGetValue(x.Key, out var d) || d.GetRawText() != x.Value.GetRawText()).ToList();
            if (all || changed.Count > 0)
                w.WriteLine("  settings  " + string.Join(", ", (all ? r.Settings.ToList() : changed).Select(x => x.Key + " " + Show(x.Value))));
            if (!t.World)
            {
                var placed = s.Catalog.PlacementsOf(r.Guid);
                w.WriteLine("  placed    " + (placed.Count == 0 ? "nowhere (no scene or prefab has a Terrain using it)" : string.Join("; ", placed.Select(p => $"{p.Ref} at {Fmt.Vec(p.Position)}"))));
            }
            var issues = Analysis.Data(r, Analysis.RenderPipeline(s.Root), t.World ? t.Placement.Ref : r.Name);
            var errors = issues.Count(i => i.Level == "error");
            var warnings = issues.Count(i => i.Level == "warning");
            if (errors + warnings > 0)
                w.WriteLine($"  problems  {(errors > 0 ? Fmt.Plural(errors, "error") + (warnings > 0 ? ", " : "") : "")}{(warnings > 0 ? Fmt.Plural(warnings, "warning") : "")}: " +
                            $"{string.Join("; ", issues.Where(i => i.Level != "note").Take(3).Select(i => i.Text))}{(errors + warnings > 3 ? "; ..." : "")} (check shows the fixes)");
        }

        static void Neighbours(List<Target> targets, TextWriter w)
        {
            var seams = Analysis.Seams(targets);
            if (seams.Count == 0)
                return;
            w.WriteLine();
            w.WriteLine("neighbours");
            foreach (var x in seams)
                w.WriteLine($"  {x.A.Label} -> {x.B.Label} ({(x.AlongX ? "east" : "north")}): {(x.MaxGap > 0.01 ? $"heights differ by up to {Fmt.M(x.MaxGap)} m" : "seam matches")}" +
                            (Math.Abs(x.Offset) > 0.01 ? $", edges {Fmt.M(Math.Abs(x.Offset))} m apart" : ""));
        }

        internal static string Show(JsonElement v) => v.ValueKind switch
        {
            JsonValueKind.Array => "(" + string.Join(", ", v.EnumerateArray().Select(Show)) + ")",
            JsonValueKind.Number => Fmt.Num(v.GetDouble()),
            JsonValueKind.Null => "none",
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => v.GetString(),
        };

        static string Props(Prototype p) => string.Join(", ", p.Props.Select(x => x.Key + " " + Show(x.Value)));

        // ------------------------------------------------------------ flat

        public static int Flat(Session s, Args a, TextWriter w)
        {
            var what = a.Positional0("terrain");
            var sizeText = a.PositionalAt(1, "WxD (metres)");
            var dims = sizeText.Split('x', 'X');
            if (dims.Length != 2 || !double.TryParse(dims[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var sw) ||
                !double.TryParse(dims[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var sd) || sw <= 0 || sd <= 0)
                throw new WaitressException($"'{sizeText}' is not a size; use WxD in metres, e.g. 20x30");
            var maxSlope = a.GetDouble("max-slope", 5);
            var count = a.GetInt("count", 5);
            var targets = s.Targets(what);
            var within = a.Get("in");
            var found = new List<(Target t, double x0, double z0, double x1, double z1, double lo, double hi, double mean, double slope, int trees, int holes)>();
            foreach (var t in targets)
            {
                var area = within == null ? null : Region.Parse(within, targets.Count == 1 ? t : null);
                var r = t.Data;
                var res = r.HeightRes;
                var step = t.SX / (res - 1);
                var stepZ = t.SZ / (res - 1);
                int nx = (int)Math.Ceiling(sw / step) + 1, nz = (int)Math.Ceiling(sd / stepZ) + 1;
                if (nx > res || nz > res)
                {
                    s.Notes.WriteLine($"note: {t.Label} is only {Fmt.M(t.SX)} x {Fmt.M(t.SZ)} m");
                    continue;
                }
                var slopes = new float[res * res];
                for (var z = 0; z < res; z++)
                for (var x = 0; x < res; x++)
                    slopes[z * res + x] = r.SlopeAtSample(x, z);
                var stride = (double)res * res * nx * nz < 4e8 ? 1 : Math.Max(1, Math.Min(nx, nz) / 4);
                var candidates = new List<(int x, int z, double lo, double hi, double mean, double slope)>();
                for (var z0 = 0; z0 + nz <= res; z0 += stride)
                for (var x0 = 0; x0 + nx <= res; x0 += stride)
                {
                    if (area != null && !(area.Inside(t.OX + x0 * step, t.OZ + z0 * stepZ) && area.Inside(t.OX + (x0 + nx - 1) * step, t.OZ + (z0 + nz - 1) * stepZ)))
                        continue;
                    double worst = 0, lo = double.MaxValue, hi = double.MinValue, sum = 0;
                    for (var z = z0; z < z0 + nz && worst <= maxSlope; z++)
                    for (var x = x0; x < x0 + nx; x++)
                    {
                        worst = Math.Max(worst, slopes[z * res + x]);
                        var h = r.SampleY(x, z) * t.SY;
                        lo = Math.Min(lo, h);
                        hi = Math.Max(hi, h);
                        sum += h;
                    }
                    // Spots touching the terrain's edge are left out: leveling them spills past it.
                    var edge = x0 == 0 || z0 == 0 || x0 + nx == res || z0 + nz == res;
                    if (worst <= maxSlope && !edge)
                        candidates.Add((x0, z0, lo, hi, sum / (nx * nz), worst));
                }
                foreach (var c in candidates.OrderBy(c => Math.Round(c.hi - c.lo, 2)).ThenBy(c => c.slope))
                {
                    double x0 = t.OX + c.x * step, z0 = t.OZ + c.z * stepZ, x1 = x0 + (nx - 1) * step, z1 = z0 + (nz - 1) * stepZ;
                    if (found.Any(f => f.t == t && f.x0 < x1 && x0 < f.x1 && f.z0 < z1 && z0 < f.z1))
                        continue;
                    var region = Region.Parse($"{x0.ToString(System.Globalization.CultureInfo.InvariantCulture)},{z0.ToString(System.Globalization.CultureInfo.InvariantCulture)}..{x1.ToString(System.Globalization.CultureInfo.InvariantCulture)},{z1.ToString(System.Globalization.CultureInfo.InvariantCulture)}", t);
                    var trees = r.Trees.Count(tr => region.Inside(t.X(tr.X), t.Z(tr.Z)));
                    var holes = 0;
                    for (var hz = 0; hz < r.HolesRes; hz++)
                    for (var hx = 0; hx < r.HolesRes; hx++)
                        if (r.Holes[hz * r.HolesRes + hx] == 0 && region.Inside(t.OX + (hx + 0.5) * t.SX / r.HolesRes, t.OZ + (hz + 0.5) * t.SZ / r.HolesRes))
                            holes++;
                    if (holes > 0)
                        continue;
                    found.Add((t, x0, z0, x1, z1, t.OY + c.lo, t.OY + c.hi, t.OY + c.mean, c.slope, trees, holes));
                    if (found.Count(f => f.t == t) >= count)
                        break;
                }
            }
            if (found.Count == 0)
            {
                w.WriteLine($"no {Fmt.M(sw)} x {Fmt.M(sd)} m area{(within != null ? " inside " + within : "")} has every slope at or under {Fmt.Deg(maxSlope)}; try --max-slope {Fmt.M(Math.Max(maxSlope * 2, 5))} or a smaller size");
                return 1;
            }
            w.WriteLine($"{Fmt.M(sw)} x {Fmt.M(sd)} m areas with every slope at or under {Fmt.Deg(maxSlope)}, flattest first (height range inside):");
            foreach (var f in found.OrderBy(f => f.hi - f.lo).Take(count * Math.Max(1, targets.Count)))
                w.WriteLine($"  {Fmt.M(f.x0)},{Fmt.M(f.z0)}..{Fmt.M(f.x1)},{Fmt.M(f.z1)}{(targets.Count > 1 || f.t.World ? " on " + f.t.Label : "")}: y {Fmt.M(f.lo)}..{Fmt.M(f.hi)} (mean {Fmt.M(f.mean)}), max slope {Fmt.Deg(f.slope)}" +
                            (f.trees > 0 ? $", {Fmt.Plural(f.trees, "tree")}" : ""));
            var best = found.OrderBy(f => f.hi - f.lo).First();
            var areaText = $"{Fmt.M(best.x0)},{Fmt.M(best.z0)}..{Fmt.M(best.x1)},{Fmt.M(best.z1)}";
            if (best.hi - best.lo < 0.01)
                w.WriteLine($"the first is already level at y {Fmt.M(best.mean)}");
            else
                w.WriteLine($"level the first: terrainwaitress set {(best.t.World ? best.t.Placement.Ref : best.t.Data.Name)} \"height[{areaText}+5]={Fmt.M(best.mean)}\"" + (best.trees > 0 ? $" trees-={areaText}" : ""));
            return 0;
        }

        // ------------------------------------------------------------ sample

        public static int Sample(Session s, Args a, TextWriter w)
        {
            var what = a.Positional0("terrain");
            var targets = s.Targets(what);
            var points = a.Positional.Skip(1).SelectMany(p => p.Split(';', StringSplitOptions.RemoveEmptyEntries)).ToList();
            if (points.Count == 0)
                throw new WaitressException("sample <terrain> <x,z | area>...: give a point (x,z) or an area (x0,z0..x1,z1, x,z~r, all)");
            var single = targets.Count == 1 ? targets[0] : null;
            foreach (var text in points)
            {
                if (Region.LooksLike(text))
                    Area(targets, Region.Parse(text, single), a.Flag("all"), w);
                else
                    Point(targets, Region.Point(text, single), text, w);
            }
            return 0;
        }

        static void Point(List<Target> targets, (double x, double z) p, string text, TextWriter w)
        {
            var on = targets.Where(t => t.Contains(p.x, p.z)).ToList();
            if (on.Count == 0)
            {
                var t0 = targets[0];
                w.WriteLine($"{Fmt.XZ(p.x, p.z)}: off the terrain" + (targets.Count == 1
                    ? $" ({t0.Label} covers x {Fmt.M(t0.OX)}..{Fmt.M(t0.OX + t0.SX)}, z {Fmt.M(t0.OZ)}..{Fmt.M(t0.OZ + t0.SZ)}, {t0.Frame} coordinates)"
                    : " (no terrain here covers it)"));
                return;
            }
            var t = on[0];
            var r = t.Data;
            var (u, v) = t.UV(p.x, p.z);
            var h = t.OY + r.HeightAt(u, v);
            var n = r.NormalAt(u, v);
            var parts = new List<string> { $"y {Fmt.M(h)}", $"slope {Fmt.Deg(r.SlopeAt(u, v))}" };
            if (r.SlopeAt(u, v) > 0.5)
            {
                var downhill = Math.Atan2(n.x, n.z) * 180 / Math.PI;
                parts.Add($"downhill {Compass(downhill)}");
            }
            var weights = r.WeightsAt(u, v);
            var layers = weights.Select((x, i) => (x, i)).Where(x => x.x >= 0.005).OrderByDescending(x => x.x).Select(x => $"{(x.i < r.Layers.Count ? r.Layers[x.i].Name : "#" + x.i)} {Fmt.Pct(x.x)}").ToList();
            if (layers.Count > 0)
                parts.Add(string.Join(" ", layers));
            if (r.HoleAt(u, v))
                parts.Add("HOLE");
            var dx = r.DetailCell(u);
            var dz = r.DetailCell(v);
            var details = r.DetailPrototypes.Select((d, i) => (d, n: r.Detail(i, dx, dz))).Where(x => x.n > 0).Select(x => $"{x.d.Name} {x.n}").ToList();
            if (details.Count > 0)
                parts.Add("details " + string.Join(" ", details));
            var near = r.Trees.Select(tr => (tr, d: Math.Sqrt(Math.Pow((tr.X - u) * t.SX, 2) + Math.Pow((tr.Z - v) * t.SZ, 2)))).Where(x => x.d <= 3).OrderBy(x => x.d).ToList();
            if (near.Count > 0)
                parts.Add($"{Fmt.Plural(near.Count, "tree")} within 3 m (nearest {Proto(r, near[0].tr.Prototype)} at {Fmt.M(near[0].d)} m)");
            w.WriteLine($"{Fmt.XZ(p.x, p.z)}{(targets.Count > 1 || t.World ? " on " + t.Label : "")}: {string.Join(", ", parts)}");
        }

        static string Proto(TerrainRecord r, int i) => i >= 0 && i < r.TreePrototypes.Count ? r.TreePrototypes[i].Name : "#" + i;

        static string Compass(double deg)
        {
            var names = new[] { "+z", "+x+z", "+x", "+x-z", "-z", "-x-z", "-x", "-x+z" };
            return names[(int)Math.Round(((deg % 360) + 360) % 360 / 45) % 8];
        }

        static void Area(List<Target> targets, Region region, bool all, TextWriter w)
        {
            var on = targets.Where(region.Overlaps).ToList();
            if (on.Count == 0)
            {
                w.WriteLine($"{region}: off the terrain");
                return;
            }
            long n = 0, flat5 = 0, flat15 = 0, flat30 = 0, holes = 0, holeCells = 0;
            double lo = double.MaxValue, hi = double.MinValue, sum = 0, slopeSum = 0, slopeMax = 0;
            (double x, double z) loAt = default, hiAt = default, steepAt = default;
            var steepSet = false;
            var treeList = new List<string>();
            var layerSum = new Dictionary<string, double>();
            long layerTexels = 0;
            var trees = new Dictionary<string, int>();
            var details = new Dictionary<string, (long sum, long cells)>();
            foreach (var t in on)
            {
                var r = t.Data;
                var step = t.SX / (r.HeightRes - 1);
                var stepZ = t.SZ / (r.HeightRes - 1);
                var (x0, x1) = region.All ? (0, r.HeightRes - 1) : Region.Span(t.OX, step, r.HeightRes, region.X0, region.X1);
                var (z0, z1) = region.All ? (0, r.HeightRes - 1) : Region.Span(t.OZ, stepZ, r.HeightRes, region.Z0, region.Z1);
                for (var z = z0; z <= z1; z++)
                for (var x = x0; x <= x1; x++)
                {
                    var wx = t.OX + x * step;
                    var wz = t.OZ + z * stepZ;
                    if (!region.Inside(wx, wz))
                        continue;
                    var h = t.OY + r.SampleY(x, z) * t.SY;
                    var slope = r.SlopeAtSample(x, z);
                    n++;
                    sum += h;
                    slopeSum += slope;
                    if (h < lo) { lo = h; loAt = (wx, wz); }
                    if (h > hi) { hi = h; hiAt = (wx, wz); }
                    if (slope > slopeMax || !steepSet) { slopeMax = slope; steepAt = (wx, wz); steepSet = true; }
                    if (slope <= 5) flat5++;
                    if (slope <= 15) flat15++;
                    if (slope <= 30) flat30++;
                }
                var ast = t.SX / (r.AlphaRes - 1);
                var astZ = t.SZ / (r.AlphaRes - 1);
                var (ax0, ax1) = region.All ? (0, r.AlphaRes - 1) : Region.Span(t.OX, ast, r.AlphaRes, region.X0, region.X1);
                var (az0, az1) = region.All ? (0, r.AlphaRes - 1) : Region.Span(t.OZ, astZ, r.AlphaRes, region.Z0, region.Z1);
                for (var z = az0; z <= az1; z++)
                for (var x = ax0; x <= ax1; x++)
                {
                    if (!region.Inside(t.OX + x * ast, t.OZ + z * astZ))
                        continue;
                    layerTexels++;
                    for (var l = 0; l < r.AlphaLayers; l++)
                    {
                        var name = l < r.Layers.Count ? r.Layers[l].Name : "#" + l;
                        layerSum[name] = layerSum.GetValueOrDefault(name) + r.Weight(l, x, z) / 255.0;
                    }
                }
                var hc = t.SX / r.HolesRes;
                var hcZ = t.SZ / r.HolesRes;
                for (var z = 0; z < r.HolesRes; z++)
                for (var x = 0; x < r.HolesRes; x++)
                {
                    if (!region.Inside(t.OX + (x + 0.5) * hc, t.OZ + (z + 0.5) * hcZ))
                        continue;
                    holeCells++;
                    if (r.Holes[z * r.HolesRes + x] == 0)
                        holes++;
                }
                foreach (var tr in r.Trees)
                    if (region.Inside(t.X(tr.X), t.Z(tr.Z)))
                    {
                        trees[Proto(r, tr.Prototype)] = trees.GetValueOrDefault(Proto(r, tr.Prototype)) + 1;
                        if (all)
                            treeList.Add($"{Proto(r, tr.Prototype)} {Fmt.XZ(t.X(tr.X), t.Z(tr.Z))}");
                    }
                var dc = t.SX / r.DetailRes;
                var dcZ = t.SZ / r.DetailRes;
                for (var l = 0; l < r.DetailPrototypes.Count; l++)
                for (var z = 0; z < r.DetailRes; z++)
                for (var x = 0; x < r.DetailRes; x++)
                {
                    var d = r.Detail(l, x, z);
                    if (d == 0 || !region.Inside(t.OX + (x + 0.5) * dc, t.OZ + (z + 0.5) * dcZ))
                        continue;
                    var name = r.DetailPrototypes[l].Name;
                    var cur = details.GetValueOrDefault(name);
                    details[name] = (cur.sum + d, cur.cells + 1);
                }
            }
            var label = on.Count == 1 && (targets.Count > 1 || on[0].World) ? " on " + on[0].Label : on.Count > 1 ? $" on {string.Join(", ", on.Select(t => t.Label))}" : "";
            if (n == 0)
            {
                w.WriteLine($"{region}{label}: no height samples inside (the heightmap spacing is {Fmt.M(on[0].SX / (on[0].Data.HeightRes - 1))} m); sample a point instead");
                return;
            }
            w.WriteLine($"{region}{label}: {Fmt.Plural(n, "height sample")}");
            w.WriteLine($"  y         {Fmt.M(lo)}..{Fmt.M(hi)} (mean {Fmt.M(sum / n)}); lowest at {Fmt.XZ(loAt.x, loAt.z)}, highest at {Fmt.XZ(hiAt.x, hiAt.z)}");
            w.WriteLine($"  slope     mean {Fmt.Deg(slopeSum / n)}, max {Fmt.Deg(slopeMax)} at {Fmt.XZ(steepAt.x, steepAt.z)}; up to 5°: {Fmt.Pct(flat5 / (double)n)}, 15°: {Fmt.Pct(flat15 / (double)n)}, 30°: {Fmt.Pct(flat30 / (double)n)}");
            if (layerTexels > 0)
                w.WriteLine("  layers    " + string.Join(", ", layerSum.OrderByDescending(x => x.Value).Where(x => all || x.Value > 0).Select(x => $"{x.Key} {Fmt.Pct(x.Value / layerTexels)}")));
            if (holes > 0)
                w.WriteLine($"  holes     {Fmt.Count(holes)} of {Fmt.Count(holeCells)} cells");
            w.WriteLine("  trees     " + (trees.Count == 0 ? "none" : $"{trees.Values.Sum()}: " + string.Join(", ", trees.Select(x => $"{x.Key} {x.Value}"))));
            if (treeList.Count > 0)
                w.WriteLine("            " + string.Join("; ", treeList.Take(100)) + (treeList.Count > 100 ? $"; ... {treeList.Count - 100} more" : ""));
            if (details.Count > 0)
                w.WriteLine("  details   " + string.Join(", ", details.Select(x => $"{x.Key} in {Fmt.Count(x.Value.cells)} cells (density sum {Fmt.Count(x.Value.sum)})")));
        }
    }
}
