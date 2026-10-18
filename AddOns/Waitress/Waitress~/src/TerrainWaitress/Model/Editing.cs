using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TerrainWaitress.Model
{
    /// <summary>One key=value, key+=value or key-=value argument of `set`.</summary>
    public sealed class Op
    {
        public string Text, Name, Prop, Kind, Value;
        public List<string> Args = new List<string>();

        public static Op Parse(string text)
        {
            var depth = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '[')
                    depth++;
                else if (c == ']')
                    depth--;
                else if (depth == 0 && c == '=')
                {
                    var kind = i > 0 && (text[i - 1] == '+' || text[i - 1] == '-') ? text[i - 1] + "=" : "=";
                    var key = text.Substring(0, i - (kind.Length - 1));
                    var op = new Op { Text = text, Kind = kind, Value = text.Substring(i + 1) };
                    var at = 0;
                    while (at < key.Length && key[at] != '[' && key[at] != '.')
                        at++;
                    op.Name = key.Substring(0, at);
                    while (at < key.Length && key[at] == '[')
                    {
                        var close = Close(key, at);
                        op.Args.Add(key.Substring(at + 1, close - at - 1));
                        at = close + 1;
                    }
                    if (at < key.Length && key[at] == '.')
                        op.Prop = key.Substring(at + 1);
                    else if (at < key.Length)
                        throw new WaitressException($"can't read '{text}': unexpected '{key.Substring(at)}'");
                    return op;
                }
            }
            if (text is "stitch")
                return new Op { Text = text, Name = text, Kind = "=", Value = "" };
            throw new WaitressException($"'{text}' is not a change; use key=value, key+=value or key-=value (`help set`)");
        }

        static int Close(string s, int open)
        {
            var depth = 0;
            for (var i = open; i < s.Length; i++)
            {
                if (s[i] == '[')
                    depth++;
                else if (s[i] == ']' && --depth == 0)
                    return i;
            }
            throw new WaitressException($"unclosed [ in '{s}'");
        }
    }

    /// <summary>
    /// Applies `set` operations to a copy of a terrain's record, so the change can be shown (and checked) before anything is
    /// sent to the Editor. Everything Unity would compute itself (re-snapping trees under new heights, resampling at a new
    /// resolution) is done here the same way.
    /// </summary>
    public sealed class Editing
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        public readonly Target T;
        public readonly TerrainRecord Before;
        public readonly TerrainRecord After;
        public readonly List<string> Notes = new List<string>();
        public readonly Dictionary<string, Dictionary<string, JsonElement>> LayerProps = new Dictionary<string, Dictionary<string, JsonElement>>();
        public readonly Dictionary<string, JsonElement> SettingChanges = new Dictionary<string, JsonElement>();
        public bool? LimitKept;
        public bool LayersChanged, TreeProtosChanged, DetailProtosChanged, ResolutionChanged, SizeChanged, TreesChanged;
        readonly Func<string, bool> m_AssetExists;
        readonly Random m_Random = new Random(1);

        public Editing(Target t, Func<string, bool> assetExists)
        {
            Before = t.Data;
            After = t.Data.Clone();
            // Later changes in one set see the earlier ones: sizes, and the ground that conditions read.
            T = new Target { Data = After, Placement = t.Placement };
            m_AssetExists = assetExists;
        }

        static readonly string[] s_Changes =
        {
            "height", "heights", "layer", "layers", "holes", "details", "detail", "trees", "tree", "treePrototypes", "detailPrototypes", "size", "heightLimit",
            "heightmapResolution", "alphamapResolution", "detailResolution", "detailPatch", "terrain", "collider", "stitch",
        };

        static int Distance(string a, string b)
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

        public void Apply(Op op)
        {
            var name = op.Name;
            switch (name)
            {
                case "size":
                    Size(op);
                    return;
                case "heightLimit":
                    HeightLimit(op);
                    return;
                case "heightmapResolution":
                case "alphamapResolution":
                case "detailResolution":
                case "detailPatch":
                    Resolution(op);
                    return;
                case "height":
                case "heights":
                    if (op.Value.StartsWith("@", StringComparison.Ordinal))
                        ImportHeights(op);
                    else
                        Height(op);
                    return;
                case "layer" when op.Prop != null:
                    LayerProp(op);
                    return;
                case "layer":
                    Paint(op);
                    return;
                case "layers":
                    Layers(op);
                    return;
                case "holes":
                case "hole":
                    Holes(op);
                    return;
                case "details":
                case "detail" when op.Prop == null:
                    Details(op);
                    return;
                case "detail":
                case "tree" when op.Prop != null:
                    PrototypeProp(op);
                    return;
                case "trees":
                case "tree":
                    Trees(op);
                    return;
                case "treePrototypes":
                case "detailPrototypes":
                    Prototypes(op);
                    return;
            }
            if (After.Settings.ContainsKey(name) && op.Args.Count == 0 && op.Kind == "=")
            {
                var v = Value(op.Value, After.Settings[name], name);
                After.Settings[name] = v;
                SettingChanges[name] = v;
                if (name == "baseMapResolution")
                    CheckPower(op, (int)v.GetDouble(), 16, 2048, false);
                return;
            }
            var known = s_Changes.Concat(After.Settings.Keys).ToList();
            var near = known.OrderBy(k => Distance(k, name)).First();
            throw new WaitressException($"'{op.Text}': no change '{name}'" + (Distance(near, name) <= 3 ? $"; did you mean {near}?" : "") +
                                        $" (changes: {string.Join(", ", s_Changes)}; settings: {string.Join(", ", After.Settings.Keys)}; `help set`)");
        }

        // ------------------------------------------------------------ values

        /// <summary>Parses text into the JSON shape of an existing value (number, bool, vector, asset path or enum name).</summary>
        public static JsonElement Value(string text, JsonElement like, string what)
        {
            string json;
            switch (like.ValueKind)
            {
                case JsonValueKind.Number:
                    if (!double.TryParse(text, NumberStyles.Float, Inv, out var d))
                        throw new WaitressException($"{what} takes a number, not '{text}'");
                    json = d.ToString("R", Inv);
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    if (text is not ("true" or "false" or "1" or "0" or "on" or "off"))
                        throw new WaitressException($"{what} takes true or false, not '{text}'");
                    json = text is "true" or "1" or "on" ? "true" : "false";
                    break;
                case JsonValueKind.Array:
                    var parts = text.Trim('(', ')').Split(',');
                    var n = like.GetArrayLength();
                    if (parts.Length != n && !(n == 4 && parts.Length == 3))
                        throw new WaitressException($"{what} takes {n} numbers separated by commas, not '{text}'");
                    var nums = new List<string>();
                    foreach (var p in parts)
                    {
                        if (!double.TryParse(p, NumberStyles.Float, Inv, out var x))
                            throw new WaitressException($"{what}: '{p}' is not a number");
                        nums.Add(x.ToString("R", Inv));
                    }
                    if (nums.Count < n)
                        nums.Add("1");
                    json = "[" + string.Join(",", nums) + "]";
                    break;
                default:
                    json = text is "none" or "null" or "" ? "null" : JsonSerializer.Serialize(text);
                    break;
            }
            return JsonDocument.Parse(json).RootElement.Clone();
        }

        double Number(Op op) => Number(op, op.Value);

        static double Number(Op op, string text)
        {
            if (!double.TryParse(text, NumberStyles.Float, Inv, out var d) || !double.IsFinite(d))
                throw new WaitressException($"'{op.Text}': '{text}' is not a number");
            return d;
        }

        static void CheckPower(Op op, int v, int lo, int hi, bool plusOne)
        {
            var b = plusOne ? v - 1 : v;
            if (v < lo || v > hi || (b & (b - 1)) != 0)
                throw new WaitressException($"'{op.Text}': must be {(plusOne ? "a power of two plus one" : "a power of two")} from {lo} to {hi}");
        }

        int Index(List<Prototype> list, string name, string what)
        {
            if (int.TryParse(name, NumberStyles.Integer, Inv, out var i))
            {
                if (i < 0 || i >= list.Count)
                    throw new WaitressException($"no {what} {i}; there are {list.Count}");
                return i;
            }
            var hit = list.FindIndex(p => p.Name == name);
            if (hit < 0)
                hit = list.FindIndex(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase) || p.Asset == name || p.Prop("prefab") == name);
            if (hit < 0)
                throw new WaitressException($"no {what} '{name}'; {T.Data.Name} has {(list.Count == 0 ? "none" : string.Join(", ", list.Select(p => p.Name)))}");
            return hit;
        }

        // Local height (metres above the base) for a value given in the target's frame.
        double Local(double y) => y - T.OY;

        // ------------------------------------------------------------ size and resolution

        void Size(Op op)
        {
            var size = (double[])After.Size.Clone();
            if (op.Prop == null)
            {
                var parts = op.Value.Split(',');
                if (parts.Length != 3)
                    throw new WaitressException($"'{op.Text}': size takes x,y,z (width, height limit, length) in metres");
                for (var i = 0; i < 3; i++)
                    size[i] = double.Parse(parts[i], NumberStyles.Float, Inv);
            }
            else
            {
                var axis = "xyz".IndexOf(op.Prop, StringComparison.Ordinal);
                if (op.Prop.Length != 1 || axis < 0)
                    throw new WaitressException($"'{op.Text}': size.x, size.y or size.z");
                size[axis] = Number(op);
            }
            if (size.Any(v => v <= 0 || v > 100000))
                throw new WaitressException($"'{op.Text}': sizes must be above 0");
            if (size[1] != After.Size[1])
                LimitKept = false;
            After.Size = size;
            SizeChanged = true;
        }

        // A new size.y with the stored fractions rescaled, so the ground stays at the same heights.
        void HeightLimit(Op op)
        {
            var limit = Number(op);
            if (limit <= 0 || limit > 100000)
                throw new WaitressException($"'{op.Text}': the height limit is in metres above the terrain's base");
            var old = After.Size[1];
            var clipped = 0;
            for (var i = 0; i < After.Heights.Length; i++)
            {
                var h = After.Heights[i] / TerrainRecord.HeightSteps * old;
                if (h > limit + 1e-6)
                    clipped++;
                After.Heights[i] = Steps(h / limit);
            }
            if (clipped > 0)
                Notes.Add($"warning: {Fmt.Plural(clipped, "sample")} stand above {Fmt.M(limit)} m and are cut to it");
            After.Size = new[] { After.Size[0], limit, After.Size[2] };
            LimitKept ??= true;
            for (var i = 0; i < After.Trees.Length; i++)
                After.Trees[i].Y = Math.Min(1f, (float)(After.Trees[i].Y * old / limit));
            SizeChanged = true;
            TreesChanged = true;
        }

        void Resolution(Op op)
        {
            var v = (int)Number(op);
            switch (op.Name)
            {
                case "heightmapResolution":
                    CheckPower(op, v, 33, 4097, true);
                    if (v == After.HeightRes)
                        return;
                    var heights = new ushort[v * v];
                    for (var z = 0; z < v; z++)
                    for (var x = 0; x < v; x++)
                        heights[z * v + x] = Steps(After.HeightAt(x / (v - 1f), z / (v - 1f)) / After.Size[1]);
                    var holes = new byte[(v - 1) * (v - 1)];
                    for (var z = 0; z < v - 1; z++)
                    for (var x = 0; x < v - 1; x++)
                        holes[z * (v - 1) + x] = After.HoleAt((x + 0.5f) / (v - 1), (z + 0.5f) / (v - 1)) ? (byte)0 : (byte)1;
                    After.Heights = heights;
                    After.HeightRes = v;
                    After.Holes = holes;
                    After.HolesRes = v - 1;
                    SnapTrees(0, v - 1, 0, v - 1);
                    break;
                case "alphamapResolution":
                    CheckPower(op, v, 16, 2048, false);
                    if (v == After.AlphaRes)
                        return;
                    var alpha = new byte[v * v * After.AlphaLayers];
                    for (var z = 0; z < v; z++)
                    for (var x = 0; x < v; x++)
                        SetTexel(alpha, v, x, z, After.WeightsAt(x / (v - 1f), z / (v - 1f)));
                    After.Alpha = alpha;
                    After.AlphaRes = v;
                    break;
                case "detailResolution":
                case "detailPatch":
                    var res = op.Name == "detailResolution" ? v : After.DetailRes;
                    var patch = op.Name == "detailPatch" ? v : After.DetailPatch;
                    if (res < 0 || res > 4048 || patch < 8 || patch > 128)
                        throw new WaitressException($"'{op.Text}': detail resolution 0..4048, patch 8..128");
                    if (res != After.DetailRes)
                    {
                        var details = new ushort[res * res * After.DetailPrototypes.Count];
                        for (var l = 0; l < After.DetailPrototypes.Count; l++)
                        for (var z = 0; z < res; z++)
                        for (var x = 0; x < res; x++)
                            details[(l * res + z) * res + x] = After.Detail(l, After.DetailCell((x + 0.5f) / res), After.DetailCell((z + 0.5f) / res));
                        After.Details = details;
                        if (After.DetailScatter == "InstanceCountMode" && res != After.DetailRes)
                            Notes.Add("detail cells keep their instance counts when resampled, so the number of instances per square metre changes with the cell size");
                    }
                    After.DetailRes = res;
                    After.DetailPatch = patch;
                    break;
            }
            ResolutionChanged = true;
        }

        static ushort Steps(double fraction) => (ushort)Math.Clamp(Math.Round(fraction * TerrainRecord.HeightSteps), 0, 32766);

        // Writes weights as bytes adding to 255 (largest remainders get the rounding).
        static void SetTexel(byte[] alpha, int res, int x, int z, double[] w)
        {
            var layers = w.Length;
            if (layers == 0)
                return;
            var sum = w.Sum();
            if (sum <= 0)
            {
                w = (double[])w.Clone();
                w[0] = 1;
                sum = 1;
            }
            var scaled = w.Select(v => v / sum * 255).ToArray();
            var floor = scaled.Select(v => (int)Math.Floor(v)).ToArray();
            var left = 255 - floor.Sum();
            foreach (var i in Enumerable.Range(0, layers).OrderByDescending(i => scaled[i] - floor[i]).Take(left))
                floor[i]++;
            for (var l = 0; l < layers; l++)
                alpha[(l * res + z) * res + x] = (byte)floor[l];
        }

        // ------------------------------------------------------------ heights

        Region RegionArg(Op op, int index)
        {
            if (op.Args.Count <= index)
                throw new WaitressException($"'{op.Text}': name the area in brackets, e.g. {op.Name}[all] or {op.Name}[10,10..50,50]");
            return Region.Parse(op.Args[index], T);
        }

        void Height(Op op)
        {
            var region = RegionArg(op, 0);
            var r = After;
            var res = r.HeightRes;
            var step = T.SX / (res - 1);
            var stepZ = T.SZ / (res - 1);
            var (x0, x1) = Region.Span(T.OX, step, res, region.MinX, region.MaxX);
            var (z0, z1) = Region.Span(T.OZ, stepZ, res, region.MinZ, region.MaxZ);
            if (x0 > x1 || z0 > z1)
                throw new WaitressException($"'{op.Text}': the area is off the terrain (it covers x {Fmt.M(T.OX)}..{Fmt.M(T.OX + T.SX)}, z {Fmt.M(T.OZ)}..{Fmt.M(T.OZ + T.SZ)}, {T.Frame})");
            var smooth = op.Value.StartsWith("smooth", StringComparison.Ordinal);
            var passes = 1;
            if (smooth && op.Value.Length > 6 && (op.Value[6] != ':' || !int.TryParse(op.Value.Substring(7), out passes) || passes < 1 || passes > 100))
                throw new WaitressException($"'{op.Text}': smooth or smooth:N (N passes, 1..100)");
            var ramp = !smooth && op.Value.Contains("..");
            double value = 0, rampTo = 0;
            if (ramp)
            {
                var ends = op.Value.Split("..");
                if (ends.Length != 2 || op.Kind != "=" || region.Circle || region.All)
                    throw new WaitressException($"'{op.Text}': a ramp is height[x0,z0..x1,z1]=y0..y1 on a rectangle: y0 at the first corner's end, y1 at the second's, along the longer side");
                value = Number(op, ends[0]);
                rampTo = Number(op, ends[1]);
            }
            else if (!smooth)
                value = Number(op);
            if (smooth && op.Kind != "=")
                throw new WaitressException($"'{op.Text}': use height[area]=smooth");
            var clippedTop = 0;
            var highest = 0.0;
            var clippedBottom = 0;
            var source = r.Heights;
            for (var pass = 0; pass < passes; pass++)
            {
                var next = (ushort[])source.Clone();
                for (var z = z0; z <= z1; z++)
                for (var x = x0; x <= x1; x++)
                {
                    var wgt = region.Weight(T.OX + x * step, T.OZ + z * stepZ);
                    if (wgt <= 0)
                        continue;
                    var cur = source[z * res + x] / TerrainRecord.HeightSteps * T.SY;
                    double target;
                    if (smooth)
                    {
                        double sum = 0;
                        var n = 0;
                        for (var dz = -1; dz <= 1; dz++)
                        for (var dx = -1; dx <= 1; dx++)
                        {
                            int sx = x + dx, sz = z + dz;
                            if (sx < 0 || sz < 0 || sx >= res || sz >= res)
                                continue;
                            sum += source[sz * res + sx];
                            n++;
                        }
                        target = sum / n / TerrainRecord.HeightSteps * T.SY;
                    }
                    else if (ramp)
                    {
                        var alongX = Math.Abs(region.BX - region.AX) >= Math.Abs(region.BZ - region.AZ);
                        var from = alongX ? region.AX : region.AZ;
                        var to = alongX ? region.BX : region.BZ;
                        var at = alongX ? T.OX + x * step : T.OZ + z * stepZ;
                        var f = Math.Clamp((at - from) / (to - from), 0, 1);
                        target = Local(value + (rampTo - value) * f);
                    }
                    else
                        target = op.Kind == "=" ? Local(value) : op.Kind == "+=" ? cur + value : cur - value;
                    var h = cur + (target - cur) * wgt;
                    if (h > T.SY + 1e-6)
                    {
                        clippedTop++;
                        highest = Math.Max(highest, h);
                    }
                    if (h < -1e-6)
                        clippedBottom++;
                    next[z * res + x] = Steps(h / T.SY);
                }
                source = next;
            }
            r.Heights = source;
            if (clippedTop > 0)
                Notes.Add($"warning: {Fmt.Plural(clippedTop, "sample")} would go above the height limit ({Fmt.M(T.OY + T.SY)}{(T.World ? " world" : "")}) and stop there; put heightLimit={Fmt.M(Math.Ceiling(highest * 1.1))} before this change for room");
            if (clippedBottom > 0)
                Notes.Add($"warning: {Fmt.Plural(clippedBottom, "sample")} would go below the terrain's base ({Fmt.M(T.OY)}) and stop there; a terrain can't go lower than its position");
            SnapTrees(x0, x1, z0, z1);
        }

        // SetHeights moves the trees standing in the changed rectangle onto the new ground.
        void SnapTrees(int x0, int x1, int z0, int z1)
        {
            var res = After.HeightRes;
            for (var i = 0; i < After.Trees.Length; i++)
            {
                var t = After.Trees[i];
                var fx = t.X * (res - 1);
                var fz = t.Z * (res - 1);
                if (fx < x0 - 1 || fx > x1 + 1 || fz < z0 - 1 || fz > z1 + 1)
                    continue;
                t.Y = After.HeightAt(t.X, t.Z) / (float)After.Size[1];
                After.Trees[i] = t;
            }
        }

        void ImportHeights(Op op)
        {
            var path = op.Value.Substring(1);
            if (!File.Exists(path))
                throw new WaitressException($"'{op.Text}': no file {path}");
            var bytes = File.ReadAllBytes(path);
            var side = (int)Math.Round(Math.Sqrt(bytes.Length / 2.0));
            if (side < 2)
                throw new WaitressException($"'{op.Text}': {path} is {bytes.Length} bytes, too small for a heightmap");
            if (side * side * 2 != bytes.Length)
                throw new WaitressException($"'{op.Text}': {path} is {bytes.Length} bytes, not a square of 16-bit samples (a .raw/.r16 heightmap, little-endian)");
            var res = After.HeightRes;
            var heights = new ushort[res * res];
            double At(int x, int z) => BitConverter.ToUInt16(bytes, (z * side + x) * 2) / 65535.0;
            for (var z = 0; z < res; z++)
            for (var x = 0; x < res; x++)
            {
                if (side == res)
                {
                    heights[z * res + x] = Steps(At(x, z));
                    continue;
                }
                var fx = x / (res - 1.0) * (side - 1);
                var fz = z / (res - 1.0) * (side - 1);
                var ix = Math.Min((int)fx, side - 2);
                var iz = Math.Min((int)fz, side - 2);
                var tx = fx - ix;
                var tz = fz - iz;
                var v = (At(ix, iz) * (1 - tx) + At(ix + 1, iz) * tx) * (1 - tz) + (At(ix, iz + 1) * (1 - tx) + At(ix + 1, iz + 1) * tx) * tz;
                heights[z * res + x] = Steps(v);
            }
            if (side != res)
                Notes.Add($"{path} is {side}x{side}; resampled to the heightmap's {res}x{res}" +
                          (side >= 33 && side <= 4097 && ((side - 1) & (side - 2)) == 0 ? $" (put heightmapResolution={side} before it to keep every sample)" : ""));
            After.Heights = heights;
            SnapTrees(0, res - 1, 0, res - 1);
        }

        // ------------------------------------------------------------ layers and painting

        void Paint(Op op)
        {
            if (op.Args.Count < 2)
                throw new WaitressException($"'{op.Text}': layer[Name][area]=weight (0..1), e.g. layer[Grass][all]=1");
            var l = Index(After.Layers, op.Args[0], "layer");
            var region = RegionArg(op, 1);
            var v = Number(op);
            if (op.Kind == "=" && (v < 0 || v > 1))
                throw new WaitressException($"'{op.Text}': a layer weight is 0..1 (0.5 = 50%)");
            var r = After;
            var a = r.AlphaRes;
            var step = T.SX / (a - 1);
            var stepZ = T.SZ / (a - 1);
            var (x0, x1) = Region.Span(T.OX, step, a, region.MinX, region.MaxX);
            var (z0, z1) = Region.Span(T.OZ, stepZ, a, region.MinZ, region.MaxZ);
            var only = 0;
            for (var z = z0; z <= z1; z++)
            for (var x = x0; x <= x1; x++)
            {
                var wgt = region.Weight(T.OX + x * step, T.OZ + z * stepZ);
                if (wgt <= 0)
                    continue;
                var w = Enumerable.Range(0, r.AlphaLayers).Select(i => r.Weight(i, x, z) / 255.0).ToArray();
                var target = Math.Clamp(op.Kind == "=" ? v : op.Kind == "+=" ? w[l] + v : w[l] - v, 0, 1);
                var nl = w[l] + (target - w[l]) * wgt;
                var others = w.Sum() - w[l];
                for (var i = 0; i < w.Length; i++)
                {
                    if (i == l)
                        continue;
                    if (others > 1e-9)
                        w[i] = w[i] / others * (1 - nl);
                }
                if (others <= 1e-9 && nl < 1 - 1e-9)
                {
                    if (w.Length == 1)
                    {
                        only++;
                        nl = 1;
                    }
                    else
                        w[l == 0 ? 1 : 0] = 1 - nl;
                }
                w[l] = nl;
                SetTexel(r.Alpha, a, x, z, w);
            }
            if (only > 0)
                Notes.Add($"{r.Layers[l].Name} is the only layer, so its weight stays 100%");
        }

        void Layers(Op op)
        {
            var r = After;
            var a = r.AlphaRes * r.AlphaRes;
            if (op.Kind == "=" && op.Args.Count == 0)
            {
                if (op.Value != "normalize")
                    throw new WaitressException($"'{op.Text}': layers+=Assets/.../X.terrainlayer, layers-=Name, layers[i]=Assets/.../X.terrainlayer, or layers=normalize");
                var res = r.AlphaRes;
                for (var z = 0; z < res; z++)
                for (var x = 0; x < res; x++)
                    SetTexel(r.Alpha, res, x, z, Enumerable.Range(0, r.AlphaLayers).Select(i => (double)r.Weight(i, x, z)).ToArray());
                return;
            }
            if (op.Kind == "+=" || op.Kind == "=" && op.Args.Count == 1)
            {
                var asset = op.Value.Replace('\\', '/');
                if (!asset.EndsWith(".terrainlayer", StringComparison.OrdinalIgnoreCase) || !m_AssetExists(asset))
                    throw new WaitressException($"'{op.Text}': {asset} is not a .terrainlayer file in the project (make one in the Editor, or copy an existing one)");
                var layer = new Prototype { Name = Path.GetFileNameWithoutExtension(asset), Asset = asset };
                if (op.Kind == "=")
                {
                    var i = Index(r.Layers, op.Args[0], "layer");
                    r.Layers[i] = layer;
                }
                else
                {
                    if (r.Layers.Any(x => x.Asset == asset))
                        throw new WaitressException($"'{op.Text}': {T.Data.Name} already has {layer.Name}");
                    r.Layers.Add(layer);
                    var alpha = new byte[a * (r.AlphaLayers + 1)];
                    Buffer.BlockCopy(r.Alpha, 0, alpha, 0, r.Alpha.Length);
                    if (r.AlphaLayers == 0)
                        Array.Fill(alpha, (byte)255);
                    r.Alpha = alpha;
                    r.AlphaLayers++;
                }
                LayersChanged = true;
                return;
            }
            if (op.Kind == "-=")
            {
                var i = Index(r.Layers, op.Value, "layer");
                r.Layers.RemoveAt(i);
                var res = r.AlphaRes;
                var old = r.Alpha;
                var layers = r.AlphaLayers;
                r.AlphaLayers--;
                r.Alpha = new byte[a * r.AlphaLayers];
                for (var z = 0; z < res; z++)
                for (var x = 0; x < res; x++)
                {
                    var w = Enumerable.Range(0, layers).Where(k => k != i).Select(k => (double)old[(k * res + z) * res + x]).ToArray();
                    SetTexel(r.Alpha, res, x, z, w);
                }
                LayersChanged = true;
                return;
            }
            throw new WaitressException($"'{op.Text}': layers+=..., layers-=..., layers[i]=... or layers=normalize");
        }

        void LayerProp(Op op)
        {
            var i = Index(After.Layers, op.Args[0], "layer");
            var layer = After.Layers[i];
            if (layer.Missing || layer.Asset == null)
                throw new WaitressException($"'{op.Text}': layer {i} is missing");
            if (!Before.Layers.Any(x => x.Asset == layer.Asset) || !layer.Props.TryGetValue(op.Prop, out var like))
                throw new WaitressException(layer.Props.Count == 0
                    ? $"'{op.Text}': {layer.Name} was just added; change its settings in a second set"
                    : $"'{op.Text}': a TerrainLayer has no '{op.Prop}'; it has {string.Join(", ", layer.Props.Keys)}");
            var v = Value(op.Value, like, op.Prop);
            if (v.ValueKind == JsonValueKind.String && op.Prop.EndsWith("Texture", StringComparison.Ordinal))
            {
                if (!m_AssetExists(v.GetString()))
                    throw new WaitressException($"'{op.Text}': no file {v.GetString()}");
                if (Path.GetExtension(v.GetString()).ToLowerInvariant() is not (".png" or ".jpg" or ".jpeg" or ".tga" or ".psd" or ".tif" or ".tiff" or ".exr" or ".bmp" or ".gif" or ".hdr"))
                    throw new WaitressException($"'{op.Text}': {v.GetString()} is not a texture");
            }
            layer.Props[op.Prop] = v;
            if (!LayerProps.TryGetValue(layer.Asset, out var props))
                LayerProps[layer.Asset] = props = new Dictionary<string, JsonElement>();
            props[op.Prop] = v;
        }

        // ------------------------------------------------------------ holes and details

        void Holes(Op op)
        {
            var region = RegionArg(op, 0);
            if (region.Falloff > 0)
                throw new WaitressException($"'{op.Text}': holes have no falloff");
            if (op.Kind != "=" || op.Value is not ("true" or "false"))
                throw new WaitressException($"'{op.Text}': holes[area]=true cuts a hole, holes[area]=false fills it");
            var r = After;
            var cell = T.SX / r.HolesRes;
            var cellZ = T.SZ / r.HolesRes;
            var changed = 0;
            for (var z = 0; z < r.HolesRes; z++)
            for (var x = 0; x < r.HolesRes; x++)
                if (region.Inside(T.OX + (x + 0.5) * cell, T.OZ + (z + 0.5) * cellZ))
                {
                    r.Holes[z * r.HolesRes + x] = op.Value == "true" ? (byte)0 : (byte)1;
                    changed++;
                }
            if (changed == 0)
                throw new WaitressException($"'{op.Text}': the area covers no hole cell centre on {T.Label} (cells are {Fmt.M(cell)} m; the terrain covers x {Fmt.M(T.OX)}..{Fmt.M(T.OX + T.SX)}, z {Fmt.M(T.OZ)}..{Fmt.M(T.OZ + T.SZ)}, {T.Frame})");
        }

        void Details(Op op)
        {
            if (op.Args.Count < 2)
                throw new WaitressException($"'{op.Text}': details[Name][area]=n (density per cell), e.g. details[Grass][0,0..20,20]=4");
            var l = Index(After.DetailPrototypes, op.Args[0], "detail prototype");
            var region = RegionArg(op, 1);
            var v = (int)Number(op);
            if (v < 0)
                throw new WaitressException($"'{op.Text}': a detail density is 0 or more");
            var r = After;
            var res = r.DetailRes;
            var max = r.DetailScatter == "CoverageMode" ? 255 : 16;
            var cell = T.SX / res;
            var cellZ = T.SZ / res;
            for (var z = 0; z < res; z++)
            for (var x = 0; x < res; x++)
            {
                if (!region.Inside(T.OX + (x + 0.5) * cell, T.OZ + (z + 0.5) * cellZ))
                    continue;
                var i = (l * res + z) * res + x;
                var cur = r.Details[i];
                var n = op.Kind == "=" ? v : op.Kind == "+=" ? cur + v : cur - v;
                r.Details[i] = (ushort)Math.Clamp(n, 0, max);
            }
            if (v > max)
                Notes.Add($"detail density is at most {max} per cell in {r.DetailScatter}; clamped");
        }

        // ------------------------------------------------------------ trees

        void Trees(Op op)
        {
            var r = After;
            if (op.Kind == "=" && op.Args.Count == 0)
            {
                if (op.Value == "snap")
                {
                    for (var i = 0; i < r.Trees.Length; i++)
                        r.Trees[i].Y = r.HeightAt(r.Trees[i].X, r.Trees[i].Z) / (float)r.Size[1];
                }
                else if (op.Value == "prune")
                    r.Trees = r.Trees.Where(t => t.Prototype >= 0 && t.Prototype < r.TreePrototypes.Count).ToArray();
                else
                    throw new WaitressException($"'{op.Text}': trees=snap (onto the ground) or trees=prune (drop trees with a missing prototype)");
                TreesChanged = true;
                return;
            }
            var proto = op.Args.Count > 0 ? Index(r.TreePrototypes, op.Args[0], "tree prototype") : -1;
            if (op.Kind == "-=")
            {
                Func<Tree, bool> hit;
                if (op.Value == "all")
                    hit = _ => true;
                else if (op.Value == "holes")
                    hit = t => r.HoleAt(t.X, t.Z);
                else
                {
                    var region = Region.Parse(op.Value, T);
                    hit = t => region.Inside(T.X(t.X), T.Z(t.Z));
                }
                var before = r.Trees.Length;
                r.Trees = r.Trees.Where(t => !(hit(t) && (proto < 0 || t.Prototype == proto))).ToArray();
                if (r.Trees.Length == before)
                    Notes.Add($"{op.Text} matched no trees");
                TreesChanged = true;
                return;
            }
            if (op.Kind != "+=" || proto < 0)
                throw new WaitressException($"'{op.Text}': trees[Prototype]+=x,z;x,z (place), trees[Prototype]+=area:count[:seed] (scatter), trees-=area, trees[Prototype]-=area|all, trees=snap");
            if (!r.TreePrototypes[proto].Valid)
                Notes.Add($"warning: {r.TreePrototypes[proto].Name} is not a valid tree prototype ({r.TreePrototypes[proto].Message}); Unity won't draw these trees");
            var added = new List<Tree>();
            var colon = op.Value.IndexOf(':');
            if (colon > 0 && Region.LooksLike(op.Value.Substring(0, colon)))
            {
                var region = Region.Parse(op.Value.Substring(0, colon), T);
                var rest = op.Value.Substring(colon + 1).Split(':');
                if (!int.TryParse(rest[0], out var count) || count < 1 || count > 1000000)
                    throw new WaitressException($"'{op.Text}': area:count[:seed], count 1..1000000");
                var seed = 0;
                if (rest.Length > 1 && !int.TryParse(rest[1], out seed))
                    throw new WaitressException($"'{op.Text}': the seed '{rest[1]}' is not a whole number");
                var rng = rest.Length > 1 ? new Random(seed) : m_Random;
                var tries = 0;
                while (added.Count < count && tries++ < count * 50)
                {
                    var x = region.X0 + rng.NextDouble() * (region.X1 - region.X0);
                    var z = region.Z0 + rng.NextDouble() * (region.Z1 - region.Z0);
                    var (u, v) = T.UV(x, z);
                    if (!region.Inside(x, z) || u < 0 || u > 1 || v < 0 || v > 1 || r.HoleAt(u, v))
                        continue;
                    added.Add(NewTree(u, v, proto, (float)(rng.NextDouble() * Math.PI * 2)));
                }
                if (added.Count < count)
                    Notes.Add($"warning: placed {added.Count} of {count} trees: too little of the area is on the terrain, off holes{(region.HasConditions ? " and matching its conditions" : "")}");
            }
            else
            {
                if (Region.LooksLike(op.Value))
                    throw new WaitressException($"'{op.Text}': to scatter in an area give a count: {op.Name}[{op.Args[0]}]+={op.Value}:50 (area:count[:seed]); to place, give points x,z;x,z");
                foreach (var text in op.Value.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var (x, z) = Region.Point(text, T);
                    var (u, v) = T.UV(x, z);
                    if (u < 0 || u > 1 || v < 0 || v > 1)
                        throw new WaitressException($"'{op.Text}': {text} is off the terrain (x {Fmt.M(T.OX)}..{Fmt.M(T.OX + T.SX)}, z {Fmt.M(T.OZ)}..{Fmt.M(T.OZ + T.SZ)}, {T.Frame})");
                    added.Add(NewTree(u, v, proto, 0));
                }
            }
            r.Trees = r.Trees.Concat(added).ToArray();
            TreesChanged = true;
        }

        Tree NewTree(float u, float v, int proto, float rotation) => new Tree
        {
            X = u, Z = v, Y = After.HeightAt(u, v) / (float)After.Size[1], Width = 1, Height = 1, Rotation = rotation, Prototype = proto,
            Color = 0xFFFFFFFF, LightmapColor = 0xFFFFFFFF,
        };

        // ------------------------------------------------------------ prototypes

        void Prototypes(Op op)
        {
            var trees = op.Name == "treePrototypes";
            var list = trees ? After.TreePrototypes : After.DetailPrototypes;
            if (op.Kind == "+=")
            {
                var asset = op.Value.Replace('\\', '/');
                if (!m_AssetExists(asset))
                    throw new WaitressException($"'{op.Text}': no file {asset}");
                var ext = Path.GetExtension(asset).ToLowerInvariant();
                var prefab = ext is ".prefab" or ".fbx" or ".obj" or ".blend";
                var texture = ext is ".png" or ".jpg" or ".jpeg" or ".tga" or ".psd" or ".tif" or ".tiff" or ".exr" or ".bmp" or ".gif" or ".hdr";
                if (trees && !prefab)
                    throw new WaitressException($"'{op.Text}': a tree prototype is a prefab or model (.prefab, .fbx, .obj, .blend)");
                if (!trees && !prefab && !texture)
                    throw new WaitressException($"'{op.Text}': a detail prototype is a texture (grass billboards) or a prefab or model (meshes)");
                if (list.Any(x => x.Prop("prefab") == asset || x.Prop("prototype") == asset || x.Prop("prototypeTexture") == asset))
                    throw new WaitressException($"'{op.Text}': {T.Data.Name} already has {Path.GetFileNameWithoutExtension(asset)}");
                var p = new Prototype { Name = Path.GetFileNameWithoutExtension(asset) };
                if (trees)
                    p.Props["prefab"] = Value(asset, JsonDocument.Parse("\"\"").RootElement, "prefab");
                else if (prefab)
                {
                    p.Props["prototype"] = Value(asset, JsonDocument.Parse("\"\"").RootElement, "prototype");
                    p.Props["usePrototypeMesh"] = JsonDocument.Parse("true").RootElement.Clone();
                    p.Props["renderMode"] = JsonDocument.Parse("\"VertexLit\"").RootElement.Clone();
                    p.Props["useInstancing"] = JsonDocument.Parse("true").RootElement.Clone();
                }
                else
                {
                    p.Props["prototypeTexture"] = Value(asset, JsonDocument.Parse("\"\"").RootElement, "prototypeTexture");
                    p.Props["usePrototypeMesh"] = JsonDocument.Parse("false").RootElement.Clone();
                    p.Props["renderMode"] = JsonDocument.Parse("\"GrassBillboard\"").RootElement.Clone();
                }
                list.Add(p);
                if (!trees)
                {
                    var n = After.DetailRes * After.DetailRes;
                    var details = new ushort[After.Details.Length + n];
                    Buffer.BlockCopy(After.Details, 0, details, 0, After.Details.Length * 2);
                    After.Details = details;
                }
            }
            else if (op.Kind == "-=")
            {
                var i = Index(list, op.Value, trees ? "tree prototype" : "detail prototype");
                list.RemoveAt(i);
                if (trees)
                {
                    After.Trees = After.Trees.Where(t => t.Prototype != i).Select(t =>
                    {
                        if (t.Prototype > i)
                            t.Prototype--;
                        return t;
                    }).ToArray();
                    TreesChanged = true;
                }
                else
                {
                    var n = After.DetailRes * After.DetailRes;
                    After.Details = After.Details.Take(i * n).Concat(After.Details.Skip((i + 1) * n)).ToArray();
                }
            }
            else
                throw new WaitressException($"'{op.Text}': {op.Name}+=Assets/... or {op.Name}-=Name");
            if (trees)
                TreeProtosChanged = true;
            else
                DetailProtosChanged = true;
        }

        void PrototypeProp(Op op)
        {
            var trees = op.Name == "tree";
            var list = trees ? After.TreePrototypes : After.DetailPrototypes;
            if (op.Args.Count != 1 || op.Prop == null)
                throw new WaitressException($"'{op.Text}': {op.Name}[Name].setting=value");
            var p = list[Index(list, op.Args[0], trees ? "tree prototype" : "detail prototype")];
            if (!p.Props.TryGetValue(op.Prop, out var like))
            {
                var known = (trees ? Before.TreePrototypes : Before.DetailPrototypes).FirstOrDefault()?.Props.Keys;
                throw new WaitressException($"'{op.Text}': no setting '{op.Prop}'" + (known == null ? "" : "; settings: " + string.Join(", ", known)));
            }
            p.Props[op.Prop] = Value(op.Value, like, op.Prop);
            if (trees)
                TreeProtosChanged = true;
            else
                DetailProtosChanged = true;
        }
    }
}
