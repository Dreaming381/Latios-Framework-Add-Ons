using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TerrainWaitress.Model;

namespace TerrainWaitress.Commands
{
    public static class Edit
    {
        // ------------------------------------------------------------ check

        public static int Check(Session s, Args a, TextWriter w)
        {
            var targets = new List<Target>();
            if (a.Positional.Count == 0)
            {
                foreach (var file in s.Catalog.Placements.Select(p => p.File).Distinct())
                    targets.AddRange(s.Targets(file));
                var placed = new HashSet<string>(targets.Select(t => t.Data.Guid));
                var loose = s.Catalog.All.Where(f => f.AssetPath.StartsWith("Assets/", StringComparison.Ordinal) && !placed.Contains(f.Guid)).ToList();
                targets.AddRange(s.Catalog.Get(loose, s.Notes).Select(r => new Target { Data = r }));
                if (targets.Count == 0)
                {
                    w.WriteLine("no terrains in the project");
                    return 2;
                }
            }
            else
                foreach (var what in a.Positional)
                    targets.AddRange(s.Targets(what));

            var pipeline = Analysis.RenderPipeline(s.Root);
            var issues = new List<Issue>();
            foreach (var group in targets.Where(t => t.World).GroupBy(t => t.Placement.File))
            {
                var list = group.ToList();
                issues.AddRange(Analysis.Placement(list, s.Catalog.Placements, pipeline, g => s.Catalog.AssetPathOf(g), s.Catalog.ShaderOf));
                issues.AddRange(Analysis.SeamIssues(Analysis.Seams(list), group.Key));
            }
            foreach (var t in targets.GroupBy(t => t.Data.Guid).Select(g => g.First()))
                issues.AddRange(Analysis.Data(t.Data, pipeline, t.World && s.Catalog.PlacementsOf(t.Data.Guid).Count == 1 ? t.Placement.Ref : t.Data.Name));

            var shown = issues;
            var notes = 0;
            var checkedText = $"checked {Fmt.Plural(targets.Count, "terrain")}";
            if (shown.Count == 0)
                w.WriteLine($"{checkedText}: no problems" + (notes > 0 ? $" ({Fmt.Plural(notes, "note")}; --all shows them)" : ""));
            else
            {
                foreach (var g in shown.GroupBy(i => i.Where))
                {
                    w.WriteLine(g.Key);
                    foreach (var i in g.OrderBy(i => i.Level == "error" ? 0 : i.Level == "warning" ? 1 : 2))
                    {
                        w.WriteLine($"  {i.Level,-8} {i.Text}");
                        if (i.Fix != null)
                            w.WriteLine($"           fix: {i.Fix}");
                    }
                }
                w.WriteLine($"{checkedText}: {Fmt.Plural(shown.Count(i => i.Level == "error"), "error")}, {Fmt.Plural(shown.Count(i => i.Level == "warning"), "warning")}, " +
                            Fmt.Plural(shown.Count(i => i.Level == "note"), "note"));
            }
            return shown.Any(i => i.Level != "note") ? 1 : 0;
        }

        // ------------------------------------------------------------ set

        public static int Set(Session s, Args a, TextWriter w)
        {
            var words = a.Positional.ToList();
            var ops = new List<Op>();
            var names = new List<string>();
            foreach (var word in words)
            {
                if (word.Contains('=') || word == "stitch")
                    ops.Add(Op.Parse(word));
                else if (ops.Count == 0)
                    names.Add(word);
                else
                    throw new WaitressException($"'{word}' comes after the changes; name terrains first, then key=value changes");
            }
            if (names.Count == 0 || ops.Count == 0)
                throw new WaitressException("set <terrain>... key=value ... [--dry-run] (`help set`)");
            var dryRun = a.Flag("dry-run");
            var targets = names.SelectMany(s.Targets).ToList();
            // Stitching only touches tiles that meet, so a scene's unconnected terrains don't count.
            if (ops.Any(o => o.Name == "stitch") && targets.Count > 2 && targets.All(t => t.World))
            {
                var seams = Analysis.Seams(targets).Where(x => Math.Abs(x.Offset) <= 0.01).ToList();
                targets = targets.Where(t => seams.Any(x => x.A == t || x.B == t)).ToList();
            }
            var shared = targets.GroupBy(t => t.Data.Guid).FirstOrDefault(g => g.Count() > 1);
            if (shared != null)
                throw new WaitressException($"{string.Join(" and ", shared.Select(t => t.Label))} share {shared.First().Data.Name}; name one of them");

            Func<string, bool> exists = p => File.Exists(Path.Combine(s.Root, p));
            var edits = targets.Select(t => new Editing(t, exists)).ToList();
            var placementOps = new List<(Target t, Op op)>();
            var errors = new List<string>();
            foreach (var op in ops)
            {
                if (op.Name == "stitch")
                {
                    try
                    {
                        Stitch(edits, w);
                    }
                    catch (WaitressException e)
                    {
                        errors.Add(e.Message);
                    }
                    continue;
                }
                if (op.Name is "terrain" or "collider")
                {
                    if (op.Prop == null)
                        errors.Add($"'{op.Text}': terrain.Setting=value or collider.TerrainData=Assets/... (settings as `info --all` lists them)");
                    else
                        foreach (var t in targets)
                        {
                            if (!t.World)
                            {
                                errors.Add($"'{op.Text}': {t.Label} is a TerrainData asset; name the Terrain object (Scene.unity:Name) to change its component");
                                continue;
                            }
                            var problem = CheckField(s, t.Placement, op);
                            if (problem != null)
                                errors.Add($"'{op.Text}': {problem}");
                            else
                                placementOps.Add((t, op));
                        }
                    continue;
                }
                var applied = false;
                foreach (var e in edits)
                {
                    if (edits.Count > 1 && op.Args.Count > 0 && Region.LooksLike(op.Args.Last()) && !Region.Parse(op.Args.Last(), null).Overlaps(e.T))
                        continue;
                    try
                    {
                        e.Apply(op);
                        applied = true;
                    }
                    catch (WaitressException x)
                    {
                        errors.Add(edits.Count > 1 ? $"{e.T.Label}: {x.Message}" : x.Message);
                    }
                }
                if (!applied && errors.Count == 0)
                    errors.Add($"'{op.Text}': the area misses every terrain named");
            }
            if (errors.Count > 0)
                throw new WaitressException((errors.Count > 1 ? $"{errors.Count} problems; nothing changed:\n  " : "") + string.Join("\n  ", errors.Distinct()));

            var any = false;
            foreach (var e in edits)
            {
                var lines = Delta(e.Before, e.After, e.T, e);
                lines.AddRange(e.Notes);
                if (lines.Count == 0)
                    continue;
                any = true;
                w.WriteLine((dryRun ? "would change " : "") + e.T.Label + (e.T.World ? $" ({e.T.Data.Path})" : ""));
                foreach (var l in lines)
                    w.WriteLine("  " + l);
            }
            foreach (var (t, op) in placementOps)
            {
                any = true;
                w.WriteLine($"{(dryRun ? "would set" : "setting")} {t.Placement.Ref} {op.Name}.{op.Prop}: {OldField(s, t.Placement, op) ?? "?"} -> {op.Value}");
            }
            if (!any)
            {
                w.WriteLine("nothing to change");
                return 0;
            }
            if (dryRun)
                return 0;

            foreach (var e in edits)
            {
                if (Delta(e.Before, e.After, e.T, e).Count == 0)
                    continue;
                EditorScript.Call(s.Root, "Apply", Request(s, e));
                var file = s.Catalog.All.First(f => f.Guid == e.Before.Guid);
                var now = s.Catalog.Get(file, s.Notes, force: true);
                var off = Mismatch(e.After, now);
                if (off != null)
                    w.WriteLine($"note: {e.T.Label}: Unity's result differs from the preview: {off}");
            }
            if (placementOps.Count > 0)
            {
                var request = JsonSerializer.Serialize(new
                {
                    changes = placementOps.Select(x => new { file = x.t.Placement.File, path = x.t.Placement.HierarchyPath, component = x.op.Name, field = x.op.Prop, value = x.op.Value }).ToArray(),
                });
                var result = EditorScript.Call(s.Root, "Place", request);
                foreach (var line in result.GetProperty("done").EnumerateArray())
                    w.WriteLine("  " + line.GetString());
            }
            return 0;
        }

        static readonly Dictionary<string, string> s_FieldAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["pixelError"] = "HeightmapPixelError", ["basemapDistance"] = "SplatMapDistance", ["material"] = "MaterialTemplate", ["materialTemplate"] = "MaterialTemplate",
            ["data"] = "TerrainData", ["terrainData"] = "TerrainData", ["groupingID"] = "GroupingID", ["autoConnect"] = "AllowAutoConnect", ["drawInstanced"] = "DrawInstanced",
            ["enabled"] = "Enabled",
        };

        static string Field(Op op) => s_FieldAliases.TryGetValue(op.Prop, out var f) ? f : op.Prop;

        static string OldField(Session s, Placement p, Op op)
        {
            var f = Field(op);
            if (op.Name == "collider")
                return f == "TerrainData" ? s.Catalog.AssetPathOf(p.ColliderDataGuid) ?? "none" : f == "Enabled" ? (p.ColliderEnabled ? "true" : "false") : null;
            if (f == "TerrainData")
                return s.Catalog.AssetPathOf(p.DataGuid);
            if (f == "MaterialTemplate")
                return p.MaterialGuid == null ? "none" : s.Catalog.AssetPathOf(p.MaterialGuid) ?? p.MaterialGuid;
            if (f == "Enabled")
                return p.Enabled ? "true" : "false";
            var key = p.Fields.Keys.FirstOrDefault(k => string.Equals(k, f, StringComparison.OrdinalIgnoreCase));
            return key == null ? null : p.Fields[key];
        }

        // The checks the Editor would make, so a dry run catches them and a real run changes nothing when one fails.
        static string CheckField(Session s, Placement p, Op op)
        {
            var f = Field(op);
            var v = op.Value;
            if (op.Name == "collider")
            {
                if (!p.HasCollider)
                    return $"{p.Object} has no TerrainCollider";
                if (f is not ("TerrainData" or "Enabled"))
                    return "collider takes TerrainData or Enabled";
            }
            if (f == "TerrainData")
            {
                var file = s.Catalog.TryAsset(v);
                return file == null ? $"{v} is not a TerrainData asset" : null;
            }
            if (f == "MaterialTemplate")
                return v is "none" or "" || v.EndsWith(".mat", StringComparison.OrdinalIgnoreCase) && File.Exists(Path.Combine(s.Root, v)) ? null : $"{v} is not a material (.mat) in the project, or none";
            if (f == "Enabled")
                return v is "true" or "false" or "1" or "0" ? null : "Enabled takes true or false";
            var key = p.Fields.Keys.FirstOrDefault(k => string.Equals(k, f, StringComparison.OrdinalIgnoreCase));
            if (key == null)
            {
                var near = p.Fields.Keys.OrderBy(k => Levenshtein(k, f)).First();
                return $"a Terrain has no field '{op.Prop}'" + (Levenshtein(near, f) <= 4 ? $"; did you mean {near}?" : "") + $" (fields: pixelError, basemapDistance, material, data, {string.Join(", ", p.Fields.Keys.Take(30))})";
            }
            var old = p.Fields[key];
            if (old is "0" or "1" && v is "true" or "false")
                return null;
            if (!double.TryParse(v, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d) || !double.IsFinite(d))
                return $"{key} takes a number{(old is "0" or "1" ? " or true/false" : "")}, not '{v}'";
            if (!old.Contains('.') && !old.Contains('e') && d != Math.Floor(d))
                return $"{key} takes a whole number, not '{v}'";
            return null;
        }

        static int Levenshtein(string a, string b)
        {
            a = a.ToLowerInvariant();
            b = b.ToLowerInvariant();
            var d = new int[a.Length + 1, b.Length + 1];
            for (var i = 0; i <= a.Length; i++) d[i, 0] = i;
            for (var j = 0; j <= b.Length; j++) d[0, j] = j;
            for (var i = 1; i <= a.Length; i++)
            for (var j = 1; j <= b.Length; j++)
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
            return d[a.Length, b.Length];
        }

        static string Mismatch(TerrainRecord expect, TerrainRecord got)
        {
            if (expect.HeightRes != got.HeightRes)
                return $"heightmap resolution {got.HeightRes}";
            var h = expect.Heights.Zip(got.Heights).Count(x => x.First != x.Second);
            if (h > 0)
                return $"{Fmt.Plural(h, "height sample")}";
            if (expect.AlphaLayers == got.AlphaLayers && expect.Alpha.Length == got.Alpha.Length)
            {
                var al = expect.Alpha.Zip(got.Alpha).Count(x => Math.Abs(x.First - x.Second) > 1);
                if (al > 0)
                    return $"{Fmt.Plural(al, "splat weight")}";
            }
            if (expect.Trees.Length != got.Trees.Length)
                return $"{got.Trees.Length} trees, not {expect.Trees.Length}";
            var moved = expect.Trees.Zip(got.Trees).Count(x => Math.Abs(x.First.Y - x.Second.Y) * got.Size[1] > 0.01);
            if (moved > 0)
                return $"{Fmt.Plural(moved, "tree")} at another height";
            return null;
        }

        static void Stitch(List<Editing> edits, TextWriter w)
        {
            var targets = edits.Select(e => e.T).ToList();
            if (targets.Count < 2 || !targets.All(t => t.World))
                throw new WaitressException("stitch: name a scene (or two or more Terrain objects) whose tiles touch");
            var seams = Analysis.Seams(targets).Where(x => Math.Abs(x.Offset) <= 0.01).ToList();
            if (seams.Count == 0)
                throw new WaitressException("stitch: none of these terrains share an edge (edges must touch exactly)");
            foreach (var seam in seams)
            {
                var ea = edits.First(e => e.T == seam.A);
                var eb = edits.First(e => e.T == seam.B);
                // Each tile's edge samples move to the average of both tiles' ground there.
                foreach (var (e, other, edge) in new[] { (ea, eb, 1), (eb, ea, 0) })
                {
                    var r = e.After;
                    var res = r.HeightRes;
                    var t = e.T;
                    for (var k = 0; k < res; k++)
                    {
                        int x = seam.AlongX ? edge * (res - 1) : k, z = seam.AlongX ? k : edge * (res - 1);
                        var wx = t.OX + x * t.SX / (res - 1);
                        var wz = t.OZ + z * t.SZ / (res - 1);
                        if ((seam.AlongX ? wz : wx) < seam.From - 1e-6 || (seam.AlongX ? wz : wx) > seam.To + 1e-6)
                            continue;
                        var ot = other.T;
                        var mine = t.OY + r.SampleY(x, z) * t.SY;
                        var (ou, ov) = ot.UV(wx, wz);
                        var theirs = ot.OY + other.Before.HeightAt(Math.Clamp(ou, 0, 1), Math.Clamp(ov, 0, 1));
                        var mean = (mine + theirs) / 2;
                        r.Heights[z * res + x] = (ushort)Math.Clamp(Math.Round((mean - t.OY) / t.SY * TerrainRecord.HeightSteps), 0, 32766);
                    }
                }
                if (seam.A.Data.HeightRes != seam.B.Data.HeightRes)
                    w.WriteLine($"note: {seam.A.Label} and {seam.B.Label} have different heightmap resolutions, so the seam can only match at shared samples");
            }
        }

        static string Request(Session s, Editing e)
        {
            var dir = Path.Combine(s.Catalog.Folder, "tmp");
            Directory.CreateDirectory(dir);
            var b = e.Before;
            var r = e.After;
            var req = new Dictionary<string, object> { ["path"] = b.Path };
            var full = e.ResolutionChanged || e.LayersChanged || e.DetailProtosChanged;
            if (e.SizeChanged || e.ResolutionChanged)
                req["size"] = r.Size;
            if (r.HeightRes != b.HeightRes)
                req["heightmapResolution"] = r.HeightRes;
            if (r.AlphaRes != b.AlphaRes)
                req["alphamapResolution"] = r.AlphaRes;
            if (r.DetailRes != b.DetailRes || r.DetailPatch != b.DetailPatch)
            {
                req["detailResolution"] = r.DetailRes;
                req["detailPatch"] = r.DetailPatch;
            }
            if (e.SettingChanges.Count > 0)
                req["settings"] = e.SettingChanges;
            if (e.LayerProps.Count > 0)
                req["layerProps"] = e.LayerProps;
            if (e.LayersChanged)
                req["layers"] = r.Layers.Select(l => l.Asset).ToArray();
            if (e.TreeProtosChanged)
                req["treePrototypes"] = r.TreePrototypes.Select(p => p.Props).ToArray();
            if (e.DetailProtosChanged)
                req["detailPrototypes"] = r.DetailPrototypes.Select(p => p.Props).ToArray();

            object Block<T>(string kind, T[] before, T[] after, int res, int layers, int bytes, Func<T, byte[]> encode) where T : IEquatable<T>
            {
                int x0 = res, x1 = -1, z0 = res, z1 = -1;
                if (full || before.Length != after.Length)
                {
                    x0 = z0 = 0;
                    x1 = z1 = res - 1;
                }
                else
                    for (var l = 0; l < layers; l++)
                    for (var z = 0; z < res; z++)
                    for (var x = 0; x < res; x++)
                    {
                        var i = (l * res + z) * res + x;
                        if (before[i].Equals(after[i]))
                            continue;
                        x0 = Math.Min(x0, x);
                        x1 = Math.Max(x1, x);
                        z0 = Math.Min(z0, z);
                        z1 = Math.Max(z1, z);
                    }
                if (x1 < 0 || res == 0 || layers == 0)
                    return null;
                int bw = x1 - x0 + 1, bh = z1 - z0 + 1;
                var data = new byte[bw * bh * layers * bytes];
                var o = 0;
                for (var l = 0; l < layers; l++)
                for (var z = z0; z <= z1; z++)
                for (var x = x0; x <= x1; x++)
                {
                    var enc = encode(after[(l * res + z) * res + x]);
                    Buffer.BlockCopy(enc, 0, data, o, enc.Length);
                    o += enc.Length;
                }
                var path = Path.Combine(dir, b.Guid + "." + kind + ".bin");
                File.WriteAllBytes(path, data);
                return new { file = path.Replace('\\', '/'), x = x0, y = z0, w = bw, h = bh };
            }

            var heights = Block("heights", b.Heights, r.Heights, r.HeightRes, 1, 2, v => BitConverter.GetBytes(v));
            if (heights != null)
                req["heights"] = heights;
            var holes = Block("holes", b.Holes, r.Holes, r.HolesRes, 1, 1, v => new[] { v });
            if (holes != null)
                req["holes"] = holes;
            var alpha = Block("alpha", b.Alpha, r.Alpha, r.AlphaRes, r.AlphaLayers, 1, v => new[] { v });
            if (alpha != null)
                req["alpha"] = alpha;
            var details = Block("details", b.Details, r.Details, r.DetailRes, r.DetailPrototypes.Count, 2, v => BitConverter.GetBytes(v));
            if (details != null)
                req["details"] = details;
            // Unity re-snaps trees whenever heights change, so the computed trees are always sent last.
            if (e.TreesChanged || heights != null || e.TreeProtosChanged || !b.Trees.SequenceEqual(r.Trees))
            {
                var path = Path.Combine(dir, b.Guid + ".trees.bin");
                using (var bw = new BinaryWriter(File.Create(path)))
                    foreach (var t in r.Trees)
                    {
                        bw.Write(t.X);
                        bw.Write(t.Y);
                        bw.Write(t.Z);
                        bw.Write(t.Width);
                        bw.Write(t.Height);
                        bw.Write(t.Rotation);
                        bw.Write(t.Prototype);
                        bw.Write(t.Color);
                        bw.Write(t.LightmapColor);
                    }
                req["trees"] = new { file = path.Replace('\\', '/') };
            }
            return JsonSerializer.Serialize(req);
        }

        /// <summary>What changed between two versions of a terrain, in the target's frame.</summary>
        public static List<string> Delta(TerrainRecord b, TerrainRecord a, Target t, Editing e = null)
        {
            var lines = new List<string>();
            if (!b.Size.SequenceEqual(a.Size))
            {
                lines.Add($"size {Fmt.M(b.Size[0])} x {Fmt.M(b.Size[1])} x {Fmt.M(b.Size[2])} -> {Fmt.M(a.Size[0])} x {Fmt.M(a.Size[1])} x {Fmt.M(a.Size[2])}");
                if (b.Size[1] != a.Size[1])
                {
                    var rb = Analysis.HeightRange(b);
                    var ra = Analysis.HeightRange(a);
                    var kept = e?.LimitKept ?? (Math.Abs(ra.max - rb.max) < 0.01 && Math.Abs(ra.min - rb.min) < 0.01);
                    lines.Add(kept
                        ? $"heightLimit keeps the ground in place; the limit is now y {Fmt.M(t.OY + a.Size[1])}"
                        : $"heights keep their fraction of size.y, so the ground now spans {Fmt.M(t.OY + ra.min)}..{Fmt.M(t.OY + ra.max)} (was {Fmt.M(t.OY + rb.min)}..{Fmt.M(t.OY + rb.max)}; heightLimit= keeps it in place)");
                }
            }
            foreach (var (name, x, y) in new[] { ("heightmapResolution", b.HeightRes, a.HeightRes), ("alphamapResolution", b.AlphaRes, a.AlphaRes), ("detailResolution", b.DetailRes, a.DetailRes), ("detailPatch", b.DetailPatch, a.DetailPatch) })
                if (x != y)
                    lines.Add($"{name} {x} -> {y}" + (name == "heightmapResolution" ? " (resampled; holes follow)" : name == "alphamapResolution" ? " (weights resampled)" : name == "detailResolution" ? " (densities resampled)" : ""));
            foreach (var kv in a.Settings)
                if (!b.Settings.TryGetValue(kv.Key, out var old) || old.GetRawText() != kv.Value.GetRawText())
                    lines.Add($"{kv.Key} {Read.Show(old)} -> {Read.Show(kv.Value)}");

            if (b.HeightRes == a.HeightRes)
            {
                int n = 0, x0 = int.MaxValue, x1 = -1, z0 = int.MaxValue, z1 = -1;
                double lo = double.MaxValue, hi = double.MinValue, dlo = double.MaxValue, dhi = double.MinValue;
                var res = a.HeightRes;
                for (var z = 0; z < res; z++)
                for (var x = 0; x < res; x++)
                {
                    var i = z * res + x;
                    if (b.Heights[i] == a.Heights[i])
                        continue;
                    var hb = b.Heights[i] / TerrainRecord.HeightSteps * b.Size[1];
                    var ha = a.Heights[i] / TerrainRecord.HeightSteps * a.Size[1];
                    if (Math.Abs(ha - hb) < 0.005)
                        continue;
                    n++;
                    x0 = Math.Min(x0, x); x1 = Math.Max(x1, x); z0 = Math.Min(z0, z); z1 = Math.Max(z1, z);
                    lo = Math.Min(lo, ha); hi = Math.Max(hi, ha);
                    dlo = Math.Min(dlo, ha - hb); dhi = Math.Max(dhi, ha - hb);
                }
                if (n > 0)
                {
                    var sx = t.SX / (res - 1);
                    var sz = t.SZ / (res - 1);
                    lines.Add($"heights: {Fmt.Plural(n, "sample")} in x {Fmt.M(t.OX + x0 * sx)}..{Fmt.M(t.OX + x1 * sx)}, z {Fmt.M(t.OZ + z0 * sz)}..{Fmt.M(t.OZ + z1 * sz)} " +
                              $"moved {Fmt.M(dlo)}..{Fmt.M(dhi)} m; they now span y {Fmt.M(t.OY + lo)}..{Fmt.M(t.OY + hi)}");
                }
            }
            if (b.HoleCount != a.HoleCount || b.HolesRes == a.HolesRes && !b.Holes.SequenceEqual(a.Holes))
                lines.Add($"holes {Fmt.Count(b.HoleCount)} -> {Fmt.Count(a.HoleCount)} cells");

            var nb = b.Layers.Select(l => l.Name).ToList();
            var na = a.Layers.Select(l => l.Name).ToList();
            if (!nb.SequenceEqual(na))
                lines.Add($"layers [{string.Join(", ", nb)}] -> [{string.Join(", ", na)}]");
            var cb = Analysis.Coverage(b);
            var ca = Analysis.Coverage(a);
            if (b.AlphaRes == a.AlphaRes && b.AlphaLayers == a.AlphaLayers && !b.Alpha.SequenceEqual(a.Alpha) || !nb.SequenceEqual(na))
            {
                var texels = 0;
                int tx0 = int.MaxValue, tx1 = -1, tz0 = int.MaxValue, tz1 = -1;
                if (b.AlphaRes == a.AlphaRes && b.Alpha.Length == a.Alpha.Length)
                {
                    var n = a.AlphaRes * a.AlphaRes;
                    for (var i = 0; i < n; i++)
                        for (var l = 0; l < a.AlphaLayers; l++)
                            if (a.Alpha[l * n + i] != b.Alpha[l * n + i])
                            {
                                texels++;
                                tx0 = Math.Min(tx0, i % a.AlphaRes); tx1 = Math.Max(tx1, i % a.AlphaRes);
                                tz0 = Math.Min(tz0, i / a.AlphaRes); tz1 = Math.Max(tz1, i / a.AlphaRes);
                                break;
                            }
                }
                var st = t.SX / (a.AlphaRes - 1);
                var stz = t.SZ / (a.AlphaRes - 1);
                var where = texels > 0 ? $" in x {Fmt.M(t.OX + tx0 * st)}..{Fmt.M(t.OX + tx1 * st)}, z {Fmt.M(t.OZ + tz0 * stz)}..{Fmt.M(t.OZ + tz1 * stz)}" : "";
                var parts = na.Select((name, i) =>
                {
                    var j = nb.IndexOf(name);
                    var before = j >= 0 && j < cb.Length ? cb[j] : 0;
                    var after = i < ca.Length ? ca[i] : 0;
                    return Math.Abs(before - after) < 1e-9 ? null : $"{name} {Fmt.Pct(before)} -> {Fmt.Pct(after)}";
                }).Where(x => x != null).ToList();
                if (parts.Count > 0 || texels > 0)
                    lines.Add("coverage " + (parts.Count > 0 ? string.Join(", ", parts) : "unchanged overall") + (texels > 0 ? $" ({Fmt.Plural(texels, "texel")} repainted{where})" : ""));
            }
            foreach (var x in e?.LayerProps ?? new Dictionary<string, Dictionary<string, JsonElement>>())
            {
                var old = b.Layers.First(l => l.Asset == x.Key);
                lines.Add($"{x.Key}: " + string.Join(", ", x.Value.Select(p => $"{p.Key} {Read.Show(old.Props[p.Key])} -> {Read.Show(p.Value)}")) + " (a shared asset: every terrain using it changes)");
            }

            ProtoDelta("tree prototypes", b.TreePrototypes, a.TreePrototypes, lines);
            ProtoDelta("detail prototypes", b.DetailPrototypes, a.DetailPrototypes, lines);
            for (var l = 0; l < a.DetailPrototypes.Count; l++)
            {
                var j = b.DetailPrototypes.FindIndex(p => p.Name == a.DetailPrototypes[l].Name);
                var before = j >= 0 && b.DetailRes == a.DetailRes ? Analysis.DetailTotal(b, j) : j >= 0 ? -1 : 0;
                var after = Analysis.DetailTotal(a, l);
                if (before >= 0 && before != after)
                    lines.Add($"details {a.DetailPrototypes[l].Name}: density total {Fmt.Count(before)} -> {Fmt.Count(after)} ({Fmt.Count(Analysis.DetailCells(a, l))} cells painted)");
            }

            var tb = b.Trees.GroupBy(x => x.Prototype < b.TreePrototypes.Count && x.Prototype >= 0 ? b.TreePrototypes[x.Prototype].Name : "#" + x.Prototype).ToDictionary(g => g.Key, g => g.Count());
            var ta = a.Trees.GroupBy(x => x.Prototype < a.TreePrototypes.Count && x.Prototype >= 0 ? a.TreePrototypes[x.Prototype].Name : "#" + x.Prototype).ToDictionary(g => g.Key, g => g.Count());
            var treeParts = tb.Keys.Union(ta.Keys).Where(k => tb.GetValueOrDefault(k) != ta.GetValueOrDefault(k)).Select(k => $"{k} {tb.GetValueOrDefault(k)} -> {ta.GetValueOrDefault(k)}").ToList();
            if (treeParts.Count > 0)
                lines.Add($"trees {Fmt.Count(b.Trees.Length)} -> {Fmt.Count(a.Trees.Length)}: {string.Join(", ", treeParts)}");
            else if (b.Trees.Length == a.Trees.Length)
            {
                var moved = b.Trees.Zip(a.Trees).Count(x => Math.Abs(x.First.Y * b.Size[1] - x.Second.Y * a.Size[1]) > 0.005 || x.First.X != x.Second.X || x.First.Z != x.Second.Z);
                if (moved > 0)
                    lines.Add($"trees: {Fmt.Plural(moved, "tree")} moved onto the {(e != null && e.TreesChanged ? "ground" : "new ground")}");
            }
            if (a.HeightRes == b.HeightRes)
            {
                var top = Analysis.AtTop(a) - Analysis.AtTop(b);
                if (top > 0 && e == null)
                    lines.Add($"warning: {Fmt.Plural(top, "more sample")} now sit at the height limit");
            }
            return lines;
        }

        static void ProtoDelta(string what, List<Prototype> b, List<Prototype> a, List<string> lines)
        {
            var nb = b.Select(p => p.Name).ToList();
            var na = a.Select(p => p.Name).ToList();
            if (!nb.SequenceEqual(na))
                lines.Add($"{what} [{string.Join(", ", nb)}] -> [{string.Join(", ", na)}]");
            foreach (var p in a)
            {
                var old = b.FirstOrDefault(x => x.Name == p.Name);
                if (old == null)
                    continue;
                var changed = p.Props.Where(x => !old.Props.TryGetValue(x.Key, out var o) || o.GetRawText() != x.Value.GetRawText()).Select(x => $"{x.Key} {(old.Props.TryGetValue(x.Key, out var o) ? Read.Show(o) : "none")} -> {Read.Show(x.Value)}").ToList();
                if (changed.Count > 0)
                    lines.Add($"{p.Name}: {string.Join(", ", changed)}");
            }
        }

        // ------------------------------------------------------------ new, copy, export, refresh

        public static int New(Session s, Args a, TextWriter w)
        {
            var path = a.Positional0("Assets/.../Name.asset").Replace('\\', '/');
            if (!path.StartsWith("Assets/", StringComparison.Ordinal) || !path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException("new <Assets/.../Name.asset> [size=x,y,z] [heightmapResolution=513] [alphamapResolution=512] [detailResolution=1024]");
            var opts = new Dictionary<string, string> { ["size"] = "1000,600,1000", ["heightmapResolution"] = "513", ["alphamapResolution"] = "512", ["detailResolution"] = "1024", ["detailPatch"] = "32" };
            foreach (var raw in a.Positional.Skip(1))
            {
                var word = raw.StartsWith("heightLimit=", StringComparison.Ordinal) ? null : raw;
                if (word == null)
                {
                    var parts = opts["size"].Split(',');
                    opts["size"] = $"{parts[0]},{raw.Substring(12)},{parts[2]}";
                    continue;
                }
                var eq = word.IndexOf('=');
                if (eq < 0 || !opts.ContainsKey(word.Substring(0, eq)))
                    throw new WaitressException($"'{word}': new takes {string.Join(", ", opts.Keys.Select(k => k + "="))}, heightLimit= (size.y)");
                opts[word.Substring(0, eq)] = word.Substring(eq + 1);
            }
            var size = opts["size"].Split(',').Select(x => double.TryParse(x, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : double.NaN).ToArray();
            if (size.Length != 3 || size.Any(v => !(v > 0 && v <= 100000)))
                throw new WaitressException($"size={opts["size"]}: x,y,z in metres, each above 0");
            int Res(string key, int lo, int hi, bool plusOne)
            {
                if (!int.TryParse(opts[key], out var v) || v < lo || v > hi || ((plusOne ? v - 1 : v) & ((plusOne ? v - 1 : v) - 1)) != 0)
                    throw new WaitressException($"{key}={opts[key]}: must be {(plusOne ? "a power of two plus one" : "a power of two")} from {lo} to {hi}");
                return v;
            }
            Res("heightmapResolution", 33, 4097, true);
            Res("alphamapResolution", 16, 2048, false);
            if (!int.TryParse(opts["detailResolution"], out var dres) || dres < 0 || dres > 4048 || !int.TryParse(opts["detailPatch"], out var dp) || dp < 8 || dp > 128)
                throw new WaitressException("detailResolution 0..4048, detailPatch 8..128");
            var request = JsonSerializer.Serialize(new
            {
                path, size, heightmapResolution = int.Parse(opts["heightmapResolution"]), alphamapResolution = int.Parse(opts["alphamapResolution"]),
                detailResolution = int.Parse(opts["detailResolution"]), detailPatch = int.Parse(opts["detailPatch"]),
            });
            EditorScript.Call(s.Root, "Create", request);
            w.WriteLine($"created {path}: {Fmt.M(size[0])} x {Fmt.M(size[2])} m, height limit {Fmt.M(size[1])} m, flat at 0, no layers. Add a layer with " +
                        $"`terrainwaitress set {path} layers+=Assets/.../X.terrainlayer`, and place it in a scene with the Editor (GameObject > 3D Object > Terrain uses its own new data)");
            return 0;
        }

        public static int Copy(Session s, Args a, TextWriter w)
        {
            var from = s.One(a.PositionalAt(0, "terrain"));
            var to = a.PositionalAt(1, "Assets/.../New.asset").Replace('\\', '/');
            if (!to.StartsWith("Assets/", StringComparison.Ordinal) || !to.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException("copy <terrain> <Assets/.../New.asset>");
            if (File.Exists(Path.Combine(s.Root, to)))
                throw new WaitressException($"{to} already exists");
            var result = EditorScript.Call(s.Root, "Copy", JsonSerializer.Serialize(new { from = from.Data.Path, to, file = from.World ? from.Placement.File : null, path = from.World ? from.Placement.HierarchyPath : null }));
            w.WriteLine($"copied {from.Data.Path} to {to}" + (result.TryGetProperty("repointed", out var r) ? "; " + r.GetString() : ""));
            return 0;
        }

        public static int Export(Session s, Args a, TextWriter w)
        {
            var t = s.One(a.PositionalAt(0, "terrain"));
            var file = a.PositionalAt(1, "out.raw");
            if (!file.EndsWith(".raw", StringComparison.OrdinalIgnoreCase) && !file.EndsWith(".r16", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException($"{file}: export writes a .raw or .r16 file (16-bit little-endian)");
            var dir = Path.GetDirectoryName(Path.GetFullPath(file));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var r = t.Data;
            var bytes = new byte[r.Heights.Length * 2];
            for (var i = 0; i < r.Heights.Length; i++)
                BitConverter.GetBytes((ushort)Math.Round(r.Heights[i] / (double)TerrainRecord.HeightSteps * 65535)).CopyTo(bytes, i * 2);
            File.WriteAllBytes(file, bytes);
            w.WriteLine($"wrote {file}: {r.HeightRes}x{r.HeightRes} 16-bit little-endian, row 0 at z=0, 0..65535 = 0..{Fmt.M(r.Size[1])} m (re-import with set {a.Positional[0]} heights=@{file})");
            return 0;
        }

        public static int Refresh(Session s, Args a, TextWriter w)
        {
            var files = new List<TerrainFile>();
            if (a.Positional.Count == 0)
                files.AddRange(s.Catalog.All);
            foreach (var what in a.Positional)
            {
                var f = s.Catalog.TryAsset(what);
                if (f != null)
                    files.Add(f);
                else if (s.Catalog.TryFolder(what, out var under))
                    files.AddRange(under);
                else
                    throw new WaitressException($"no TerrainData asset or folder '{what}'");
            }
            var stale = a.Flag("force") ? files : files.Where(f => !s.Catalog.IsCurrent(f)).ToList();
            var removed = s.Catalog.Prune();
            if (stale.Count > 0)
            {
                var failed = s.Catalog.Export(stale);
                foreach (var f in failed)
                    w.WriteLine($"failed {f.Key}: {f.Value}");
            }
            w.WriteLine($"{Fmt.Plural(files.Count, "TerrainData asset")}: exported {stale.Count}, {files.Count - stale.Count} already current" + (removed > 0 ? $"; removed {Fmt.Plural(removed, "record")} of deleted assets" : ""));
            return 0;
        }
    }
}
