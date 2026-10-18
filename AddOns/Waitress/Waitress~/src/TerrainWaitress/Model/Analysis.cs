using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerrainWaitress.Model
{
    public sealed class Issue
    {
        public string Level; // error, warning, note
        public string Where;
        public string Text;
        public string Fix;
    }

    public sealed class Seam
    {
        public Target A, B;     // A's +x or +z edge meets B's -x or -z edge
        public bool AlongX;     // true: A's east edge meets B's west edge
        public double Offset;   // gap (+) or overlap (-) between the edges, metres
        public double MaxGap;   // largest height difference along the shared edge, metres
        public double At;       // where along the edge (world x or z)
        public int Samples;     // edge samples more than 1 cm apart
        public double From, To; // the shared stretch
    }

    public static class Analysis
    {
        public const double TreeTolerance = 0.05;

        public static double[] Coverage(TerrainRecord r)
        {
            var a = r.AlphaRes * r.AlphaRes;
            var c = new double[r.AlphaLayers];
            for (var l = 0; l < r.AlphaLayers; l++)
            {
                long sum = 0;
                for (var i = 0; i < a; i++)
                    sum += r.Alpha[l * a + i];
                c[l] = sum / (255.0 * a);
            }
            return c;
        }

        /// <summary>Texels whose weights don't add up to 1 (more than 2/255 off).</summary>
        public static int Unnormalized(TerrainRecord r)
        {
            if (r.AlphaLayers == 0)
                return 0;
            var a = r.AlphaRes * r.AlphaRes;
            var bad = 0;
            for (var i = 0; i < a; i++)
            {
                var sum = 0;
                for (var l = 0; l < r.AlphaLayers; l++)
                    sum += r.Alpha[l * a + i];
                if (Math.Abs(sum - 255) > 2)
                    bad++;
            }
            return bad;
        }

        public static (double min, double max, double mean) HeightRange(TerrainRecord r)
        {
            int lo = int.MaxValue, hi = int.MinValue;
            double sum = 0;
            foreach (var h in r.Heights)
            {
                lo = Math.Min(lo, h);
                hi = Math.Max(hi, h);
                sum += h;
            }
            var s = r.Size[1] / TerrainRecord.HeightSteps;
            return (lo * s, hi * s, sum / r.Heights.Length * s);
        }

        public static int AtTop(TerrainRecord r) => r.Heights.Count(h => h >= 32766);

        /// <summary>How far each tree sits above (+) or below (-) the ground, in metres.</summary>
        public static double TreeOffset(TerrainRecord r, Tree t) => (t.Y - r.HeightAt(t.X, t.Z) / r.Size[1]) * r.Size[1];

        public static long DetailTotal(TerrainRecord r, int layer)
        {
            long sum = 0;
            var n = r.DetailRes * r.DetailRes;
            for (var i = 0; i < n; i++)
                sum += r.Details[layer * n + i];
            return sum;
        }

        public static int DetailCells(TerrainRecord r, int layer)
        {
            var c = 0;
            var n = r.DetailRes * r.DetailRes;
            for (var i = 0; i < n; i++)
                if (r.Details[layer * n + i] != 0)
                    c++;
            return c;
        }

        public static string RenderPipeline(string root)
        {
            var path = Path.Combine(root, "ProjectSettings", "GraphicsSettings.asset");
            if (!File.Exists(path))
                return "built-in";
            foreach (var line in File.ReadLines(path))
            {
                var t = line.Trim();
                if (t.StartsWith("m_CustomRenderPipeline:", StringComparison.Ordinal) || t.StartsWith("m_RenderPipelineAsset:", StringComparison.Ordinal))
                    return t.Contains("guid:") ? "SRP" : "built-in";
            }
            return "built-in";
        }

        /// <summary>Problems in the data itself, whatever places it.</summary>
        public static List<Issue> Data(TerrainRecord r, string pipeline, string terrainRef)
        {
            var issues = new List<Issue>();
            void Add(string level, string text, string fix = null) => issues.Add(new Issue { Level = level, Where = r.Name, Text = text, Fix = fix });

            var top = AtTop(r);
            if (top > 0)
                Add(top >= 4 ? "warning" : "note", $"{Fmt.Plural(top, "height sample")} sit at the height limit ({Fmt.M(r.Size[1])} m above the base = size.y), so peaks there are cut flat. " +
                    "The cut-off shape is gone; raising the limit (the ground stays where it is) leaves room to sculpt them again",
                    $"terrainwaitress set {terrainRef} heightLimit={Fmt.M(Math.Ceiling(r.Size[1] * 1.25))}, then height[area]+=... where the peaks should rise");

            for (var l = 0; l < r.Layers.Count; l++)
            {
                var layer = r.Layers[l];
                if (layer.Missing)
                {
                    Add("error", $"layer {l} is a missing TerrainLayer asset (deleted, or its .meta changed); its weights paint nothing",
                        $"terrainwaitress set {terrainRef} layers-={l}  (or layers[{l}]=Assets/.../New.terrainlayer)");
                    continue;
                }
                if (layer.Prop("diffuseTexture") == null)
                    Add("warning", $"layer {layer.Name} has no diffuse texture, so it renders as flat grey", $"terrainwaitress set {terrainRef} layer[{layer.Name}].diffuseTexture=Assets/.../Texture.png");
            }
            var coverage = Coverage(r);
            for (var l = 0; l < coverage.Length && l < r.Layers.Count; l++)
                if (coverage[l] == 0 && !r.Layers[l].Missing && r.Layers.Count > 1)
                    Add("note", $"layer {r.Layers[l].Name} isn't painted anywhere (each layer costs a texture sample per pixel, and every 4 layers an extra pass)",
                        $"terrainwaitress set {terrainRef} layers-={r.Layers[l].Name}");
            var bad = Unnormalized(r);
            if (bad > 0)
                Add("warning", $"{Fmt.Plural(bad, "splat texel")} have weights that don't add up to 100% (renders darker or brighter there)",
                    $"terrainwaitress set {terrainRef} layers=normalize");

            for (var p = 0; p < r.TreePrototypes.Count; p++)
            {
                var proto = r.TreePrototypes[p];
                var uses = r.Trees.Count(t => t.Prototype == p);
                if (proto.Prop("prefab") == null)
                    Add("error", $"tree prototype {p} has no prefab ({Fmt.Plural(uses, "tree")} use it)", $"terrainwaitress set {terrainRef} treePrototypes-={p}");
                else if (!proto.Valid)
                    Add("error", $"tree prototype {proto.Name}: {proto.Message}" + (uses > 0 ? $" Unity skips its {Fmt.Plural(uses, "tree")}" : "") +
                                 (proto.Message != null && proto.Message.Contains("mesh renderer") ? " (it needs a MeshRenderer or LODGroup on the prefab's root object)" : ""),
                        $"fix the prefab, or terrainwaitress set {terrainRef} treePrototypes-={proto.Name} (removes its trees)");
                else if (proto.Message != null && pipeline == "built-in")
                    Add("note", $"tree prototype {proto.Name}: {proto.Message}");
                if (uses == 0 && proto.Valid)
                    Add("note", $"tree prototype {proto.Name} has no trees", $"terrainwaitress set {terrainRef} treePrototypes-={proto.Name}");
            }
            var badIndex = r.Trees.Count(t => t.Prototype < 0 || t.Prototype >= r.TreePrototypes.Count);
            if (badIndex > 0)
                Add("error", $"{Fmt.Plural(badIndex, "tree")} use a prototype index the terrain doesn't have; Unity skips them", $"terrainwaitress set {terrainRef} trees=prune");

            var floating = r.Trees.Select(t => TreeOffset(r, t)).Where(o => Math.Abs(o) > TreeTolerance).ToList();
            if (floating.Count > 0)
            {
                var up = floating.Count(o => o > 0);
                Add("warning", $"{Fmt.Plural(floating.Count, "tree")} {(up == floating.Count ? "float above" : up == 0 ? "sink below" : "float above or sink below")} the ground " +
                               $"(up to {Fmt.M(floating.Max(Math.Abs))} m; {up} above, {floating.Count - up} below)", $"terrainwaitress set {terrainRef} trees=snap");
            }
            var inHoles = r.Trees.Count(t => r.HoleAt(t.X, t.Z));
            if (inHoles > 0)
                Add("warning", $"{Fmt.Plural(inHoles, "tree")} stand over holes", $"terrainwaitress set {terrainRef} trees-=holes");

            for (var d = 0; d < r.DetailPrototypes.Count; d++)
            {
                var proto = r.DetailPrototypes[d];
                if (!proto.Valid)
                    Add("error", $"detail prototype {proto.Name}: {proto.Message}", $"terrainwaitress set {terrainRef} detailPrototypes-={proto.Name}");
                if (DetailTotal(r, d) == 0)
                    Add("note", $"detail prototype {proto.Name} isn't painted anywhere", $"terrainwaitress set {terrainRef} detailPrototypes-={proto.Name}");
            }
            return issues;
        }

        /// <summary>Problems in how a scene or prefab places its terrains.</summary>
        public static List<Issue> Placement(List<Target> placed, IReadOnlyList<Placement> everywhere, string pipeline, Func<string, string> assetPath, Func<string, string> shaderOf)
        {
            var issues = new List<Issue>();
            foreach (var t in placed)
            {
                var p = t.Placement;
                void Add(string level, string text, string fix = null) => issues.Add(new Issue { Level = level, Where = p.Ref, Text = text, Fix = fix });
                var sharing = everywhere.Where(o => o != p && o.DataGuid == p.DataGuid).ToList();
                if (sharing.Count > 0)
                    Add("warning", $"shares {t.Data.Name} with {string.Join(", ", sharing.Select(o => o.Ref))}: sculpting or painting one changes the other",
                        $"terrainwaitress copy {t.Data.Path} Assets/.../{p.Object}.asset, then point this Terrain and its collider at the copy");
                if (!p.HasCollider)
                    Add("note", "has no TerrainCollider, so nothing collides with it");
                else if (p.ColliderDataGuid != p.DataGuid)
                    Add("error", p.ColliderDataGuid == null
                            ? "its TerrainCollider has no TerrainData, so nothing collides with it"
                            : $"its TerrainCollider uses {assetPath(p.ColliderDataGuid) ?? p.ColliderDataGuid}, not the {t.Data.Name} it renders: physics and visuals disagree",
                        "set the TerrainCollider's Terrain Data to " + t.Data.Path);
                else if (!p.ColliderEnabled)
                    Add("note", "its TerrainCollider is disabled");
                if (!p.Enabled || !p.Active)
                    Add("note", !p.Active ? "its GameObject is inactive" : "the Terrain component is disabled");
                if (p.Rotated || p.Scaled)
                    Add("note", $"its transform is {(p.Rotated ? "rotated" : "")}{(p.Rotated && p.Scaled ? " and " : "")}{(p.Scaled ? "scaled" : "")}; Unity ignores a Terrain's own rotation and scale");
                if (pipeline == "SRP" && p.MaterialGuid != null)
                {
                    var shader = shaderOf(p.MaterialGuid);
                    if (p.MaterialGuid == "0000000000000000f000000000000000" || shader == "builtin")
                        Add("error", "its material template uses a built-in-pipeline shader, which this project's render pipeline can't draw (pink terrain)",
                            "set the Terrain's Material to one using Universal Render Pipeline/Terrain/Lit (or the pipeline's terrain shader)");
                }
            }
            return issues;
        }

        /// <summary>Pairs of terrains in one file whose edges meet (or nearly meet), with the height gap along the shared edge.</summary>
        public static List<Seam> Seams(List<Target> placed)
        {
            var seams = new List<Seam>();
            for (var i = 0; i < placed.Count; i++)
            for (var j = 0; j < placed.Count; j++)
            {
                if (i == j)
                    continue;
                var a = placed[i];
                var b = placed[j];
                foreach (var alongX in new[] { true, false })
                {
                    var edgeA = alongX ? a.OX + a.SX : a.OZ + a.SZ;
                    var edgeB = alongX ? b.OX : b.OZ;
                    var offset = edgeB - edgeA;
                    if (Math.Abs(offset) > 2)
                        continue;
                    var from = Math.Max(alongX ? a.OZ : a.OX, alongX ? b.OZ : b.OX);
                    var to = Math.Min(alongX ? a.OZ + a.SZ : a.OX + a.SX, alongX ? b.OZ + b.SZ : b.OX + b.SX);
                    if (to - from < 1e-3)
                        continue;
                    var s = new Seam { A = a, B = b, AlongX = alongX, Offset = offset, From = from, To = to };
                    var res = Math.Max(a.Data.HeightRes, b.Data.HeightRes);
                    var step = Math.Min((alongX ? a.SZ : a.SX) / (a.Data.HeightRes - 1), (alongX ? b.SZ : b.SX) / (b.Data.HeightRes - 1));
                    var n = Math.Max(2, (int)Math.Round((to - from) / step) + 1);
                    for (var k = 0; k < n; k++)
                    {
                        var t = from + (to - from) * k / (n - 1);
                        double ha, hb;
                        if (alongX)
                        {
                            ha = a.OY + a.Data.HeightAt(1, (float)((t - a.OZ) / a.SZ));
                            hb = b.OY + b.Data.HeightAt(0, (float)((t - b.OZ) / b.SZ));
                        }
                        else
                        {
                            ha = a.OY + a.Data.HeightAt((float)((t - a.OX) / a.SX), 1);
                            hb = b.OY + b.Data.HeightAt((float)((t - b.OX) / b.SX), 0);
                        }
                        var gap = Math.Abs(ha - hb);
                        if (gap > 0.01)
                            s.Samples++;
                        if (gap > s.MaxGap)
                        {
                            s.MaxGap = gap;
                            s.At = t;
                        }
                    }
                    seams.Add(s);
                }
            }
            return seams;
        }

        public static List<Issue> SeamIssues(List<Seam> seams, string fileRef)
        {
            var issues = new List<Issue>();
            foreach (var s in seams)
            {
                var where = $"{s.A.Label} | {s.B.Label}";
                var edge = s.AlongX ? $"x={Fmt.M(s.A.OX + s.A.SX)}" : $"z={Fmt.M(s.A.OZ + s.A.SZ)}";
                if (Math.Abs(s.Offset) > 0.01)
                    issues.Add(new Issue { Level = "warning", Where = where, Text = $"edges miss by {Fmt.M(Math.Abs(s.Offset))} m ({(s.Offset > 0 ? "gap" : "overlap")}) at {edge}; Unity only connects tiles that touch exactly" });
                if (s.A.Placement.GroupingId != s.B.Placement.GroupingId || !s.A.Placement.AutoConnect || !s.B.Placement.AutoConnect)
                    issues.Add(new Issue { Level = "note", Where = where, Text = "they touch but won't auto-connect (different Grouping IDs or Auto Connect off), so LOD can crack the seam" });
                if (s.A.Data.HeightRes != s.B.Data.HeightRes)
                    issues.Add(new Issue { Level = "warning", Where = where, Text = $"neighbours with different heightmap resolutions ({s.A.Data.HeightRes} and {s.B.Data.HeightRes}) can't match along the seam" });
                if (s.MaxGap > 0.01)
                    issues.Add(new Issue
                    {
                        Level = s.MaxGap > 0.1 ? "warning" : "note", Where = where,
                        Text = $"seam at {edge}: heights differ by up to {Fmt.M(s.MaxGap)} m at {(s.AlongX ? "z" : "x")}={Fmt.M(s.At)} ({Fmt.Plural(s.Samples, "edge sample")} over 1 cm)",
                        Fix = $"terrainwaitress set {fileRef} stitch",
                    });
            }
            return issues;
        }
    }
}
