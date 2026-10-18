using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TerrainWaitress.Model
{
    public sealed class Prototype
    {
        public string Name;
        public Dictionary<string, JsonElement> Props = new Dictionary<string, JsonElement>();
        public bool Valid = true;
        public string Message;
        public long Instances;
        public bool Missing;
        public string Asset;

        public string Prop(string key) => Props.TryGetValue(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        public Prototype Clone() => new Prototype
        {
            Name = Name, Props = new Dictionary<string, JsonElement>(Props), Valid = Valid, Message = Message, Instances = Instances, Missing = Missing, Asset = Asset,
        };
    }

    public struct Tree
    {
        public float X, Y, Z, Width, Height, Rotation;
        public int Prototype;
        public uint Color, LightmapColor;
    }

    /// <summary>
    /// One TerrainData asset as the Editor exported it: settings and prototypes from the JSON record, and its heights
    /// (Unity's 0..32766 steps), holes (1 = surface), splat weights (0..255 per layer), detail densities and trees from
    /// the binary sidecar. Arrays are row-major with z as the row, as TerrainData's own accessors are.
    /// </summary>
    public sealed class TerrainRecord
    {
        public const float HeightSteps = 32766f;

        public string Path, Guid, Stamp, FullPath;
        public bool Stale;
        public double[] Size;
        public int HeightRes, HolesRes, AlphaRes, DetailRes, DetailPatch;
        public string DetailScatter;
        public Dictionary<string, JsonElement> Settings = new Dictionary<string, JsonElement>();
        public Dictionary<string, JsonElement> Defaults = new Dictionary<string, JsonElement>();
        public List<Prototype> Layers = new List<Prototype>();
        public List<Prototype> TreePrototypes = new List<Prototype>();
        public List<Prototype> DetailPrototypes = new List<Prototype>();
        public ushort[] Heights;
        public byte[] Holes;
        public byte[] Alpha;
        public int AlphaLayers;
        public ushort[] Details;
        public Tree[] Trees;

        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        public static TerrainRecord Load(string json, string bin)
        {
            using var doc = JsonDocument.Parse(json);
            var e = doc.RootElement;
            var r = new TerrainRecord
            {
                Path = e.GetProperty("path").GetString(),
                Guid = e.GetProperty("guid").GetString(),
                Stamp = e.GetProperty("stamp").GetString(),
                Size = e.GetProperty("size").EnumerateArray().Select(x => x.GetDouble()).ToArray(),
                HeightRes = e.GetProperty("heightmapResolution").GetInt32(),
                HolesRes = e.GetProperty("holesResolution").GetInt32(),
                AlphaRes = e.GetProperty("alphamapResolution").GetInt32(),
                DetailRes = e.GetProperty("detailResolution").GetInt32(),
                DetailPatch = e.GetProperty("detailPatch").GetInt32(),
                DetailScatter = e.GetProperty("detailScatter").GetString(),
            };
            foreach (var p in e.GetProperty("settings").EnumerateObject())
                r.Settings[p.Name] = p.Value.Clone();
            foreach (var p in e.GetProperty("defaults").EnumerateObject())
                r.Defaults[p.Name] = p.Value.Clone();
            foreach (var l in e.GetProperty("layers").EnumerateArray())
                r.Layers.Add(l.TryGetProperty("missing", out _) ? new Prototype { Missing = true, Name = "(missing)" } : ReadProto(l));
            foreach (var t in e.GetProperty("treePrototypes").EnumerateArray())
                r.TreePrototypes.Add(ReadProto(t));
            foreach (var d in e.GetProperty("detailPrototypes").EnumerateArray())
                r.DetailPrototypes.Add(ReadProto(d));

            var b = e.GetProperty("bin");
            var bytes = File.ReadAllBytes(bin);
            if (bytes.Length != b.GetProperty("length").GetInt64())
                throw new InvalidOperationException("binary sidecar has the wrong length");
            var n = r.HeightRes * r.HeightRes;
            r.Heights = new ushort[n];
            Buffer.BlockCopy(bytes, (int)b.GetProperty("heights").GetInt64(), r.Heights, 0, n * 2);
            r.Holes = new byte[r.HolesRes * r.HolesRes];
            Buffer.BlockCopy(bytes, (int)b.GetProperty("holes").GetInt64(), r.Holes, 0, r.Holes.Length);
            r.AlphaLayers = b.GetProperty("alphaLayers").GetInt32();
            r.Alpha = new byte[r.AlphaRes * r.AlphaRes * r.AlphaLayers];
            Buffer.BlockCopy(bytes, (int)b.GetProperty("alpha").GetInt64(), r.Alpha, 0, r.Alpha.Length);
            r.Details = new ushort[r.DetailRes * r.DetailRes * r.DetailPrototypes.Count];
            Buffer.BlockCopy(bytes, (int)b.GetProperty("details").GetInt64(), r.Details, 0, r.Details.Length * 2);
            var count = b.GetProperty("treeCount").GetInt32();
            var at = (int)b.GetProperty("trees").GetInt64();
            r.Trees = new Tree[count];
            for (var i = 0; i < count; i++, at += 36)
                r.Trees[i] = new Tree
                {
                    X = BitConverter.ToSingle(bytes, at), Y = BitConverter.ToSingle(bytes, at + 4), Z = BitConverter.ToSingle(bytes, at + 8),
                    Width = BitConverter.ToSingle(bytes, at + 12), Height = BitConverter.ToSingle(bytes, at + 16), Rotation = BitConverter.ToSingle(bytes, at + 20),
                    Prototype = BitConverter.ToInt32(bytes, at + 24), Color = BitConverter.ToUInt32(bytes, at + 28), LightmapColor = BitConverter.ToUInt32(bytes, at + 32),
                };
            return r;
        }

        static Prototype ReadProto(JsonElement e)
        {
            var p = new Prototype { Name = e.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String ? n.GetString() : "(none)" };
            if (e.TryGetProperty("asset", out var a))
                p.Asset = a.GetString();
            foreach (var x in e.GetProperty("props").EnumerateObject())
                p.Props[x.Name] = x.Value.Clone();
            if (e.TryGetProperty("valid", out var v))
                p.Valid = v.GetBoolean();
            if (e.TryGetProperty("message", out var m))
                p.Message = m.GetString();
            if (e.TryGetProperty("instances", out var c))
                p.Instances = c.GetInt64();
            return p;
        }

        public TerrainRecord Clone()
        {
            var c = (TerrainRecord)MemberwiseClone();
            c.Size = (double[])Size.Clone();
            c.Settings = new Dictionary<string, JsonElement>(Settings);
            c.Layers = Layers.Select(l => l.Clone()).ToList();
            c.TreePrototypes = TreePrototypes.Select(l => l.Clone()).ToList();
            c.DetailPrototypes = DetailPrototypes.Select(l => l.Clone()).ToList();
            c.Heights = (ushort[])Heights.Clone();
            c.Holes = (byte[])Holes.Clone();
            c.Alpha = (byte[])Alpha.Clone();
            c.Details = (ushort[])Details.Clone();
            c.Trees = (Tree[])Trees.Clone();
            return c;
        }

        // ------------------------------------------------------------ evaluation (matches TerrainData's accessors)

        public float SampleY(int x, int z) => Heights[z * HeightRes + x] / HeightSteps;

        /// <summary>Metres above the terrain's base at normalized (u, v), as GetInterpolatedHeight: the cell splits along its 00-11 diagonal.</summary>
        public float HeightAt(float u, float v)
        {
            var r = HeightRes;
            var fx = Math.Clamp(u, 0f, 1f) * (r - 1);
            var fy = Math.Clamp(v, 0f, 1f) * (r - 1);
            var x = Math.Min((int)fx, r - 2);
            var y = Math.Min((int)fy, r - 2);
            var tx = fx - x;
            var ty = fy - y;
            float h00 = SampleY(x, y), h10 = SampleY(x + 1, y), h01 = SampleY(x, y + 1), h11 = SampleY(x + 1, y + 1);
            var h = tx > ty ? h00 + (h10 - h00) * tx + (h11 - h10) * ty : h00 + (h11 - h01) * tx + (h01 - h00) * ty;
            return h * (float)Size[1];
        }

        // Outside the grid Unity extrapolates linearly from the edge; at corners, along the diagonal.
        float Extended(int x, int z)
        {
            var r = HeightRes;
            int cx = Math.Clamp(x, 0, r - 1), cz = Math.Clamp(z, 0, r - 1);
            bool ox = cx != x, oz = cz != z;
            float G(int a, int b) => SampleY(a, b) * (float)Size[1];
            if (!ox && !oz)
                return G(cx, cz);
            int mx = 2 * cx - x, mz = 2 * cz - z;
            if (ox && !oz)
                return 2 * G(cx, cz) - G(mx, cz);
            if (!ox)
                return 2 * G(cx, cz) - G(cx, mz);
            return 2 * G(cx, cz) - G(mx, mz);
        }

        /// <summary>The Sobel normal Unity computes at a height sample.</summary>
        public (float x, float y, float z) VertexNormal(int x, int z)
        {
            float H(int a, int b) => Extended(a, b);
            var dX = -H(x - 1, z - 1) - 2 * H(x - 1, z) - H(x - 1, z + 1) + H(x + 1, z - 1) + 2 * H(x + 1, z) + H(x + 1, z + 1);
            var dZ = -H(x - 1, z - 1) - 2 * H(x, z - 1) - H(x + 1, z - 1) + H(x - 1, z + 1) + 2 * H(x, z + 1) + H(x + 1, z + 1);
            dX /= (float)Size[0] / (HeightRes - 1);
            dZ /= (float)Size[2] / (HeightRes - 1);
            return Normalize(-dX, 8, -dZ);
        }

        static (float x, float y, float z) Normalize(float x, float y, float z)
        {
            var l = MathF.Sqrt(x * x + y * y + z * z);
            return l > 1e-12f ? (x / l, y / l, z / l) : (0, 1, 0);
        }

        /// <summary>As GetInterpolatedNormal: the four corner normals blended bilinearly.</summary>
        public (float x, float y, float z) NormalAt(float u, float v)
        {
            var r = HeightRes;
            var fx = Math.Clamp(u, 0f, 1f) * (r - 1);
            var fy = Math.Clamp(v, 0f, 1f) * (r - 1);
            var x = Math.Min((int)fx, r - 2);
            var y = Math.Min((int)fy, r - 2);
            var tu = fx - x;
            var tv = fy - y;
            var n00 = VertexNormal(x, y);
            var n10 = VertexNormal(x + 1, y);
            var n01 = VertexNormal(x, y + 1);
            var n11 = VertexNormal(x + 1, y + 1);
            float L(float a, float b, float t) => a + (b - a) * t;
            var sx = L(n00.x, n10.x, tu); var sy = L(n00.y, n10.y, tu); var sz = L(n00.z, n10.z, tu);
            var tx = L(n01.x, n11.x, tu); var ty = L(n01.y, n11.y, tu); var tz = L(n01.z, n11.z, tu);
            return Normalize(L(sx, tx, tv), L(sy, ty, tv), L(sz, tz, tv));
        }

        /// <summary>Degrees from horizontal, as GetSteepness.</summary>
        public float SlopeAt(float u, float v) => MathF.Acos(Math.Clamp(NormalAt(u, v).y, -1f, 1f)) * (180f / MathF.PI);

        public float SlopeAtSample(int x, int z) => MathF.Acos(Math.Clamp(VertexNormal(x, z).y, -1f, 1f)) * (180f / MathF.PI);

        public byte Weight(int layer, int x, int z) => Alpha[(layer * AlphaRes + z) * AlphaRes + x];

        /// <summary>Splat weights at (u, v), bilinear between texel centres at i/(res-1), as the terrain shaders sample them.</summary>
        public double[] WeightsAt(float u, float v)
        {
            var a = AlphaRes;
            var w = new double[AlphaLayers];
            if (AlphaLayers == 0)
                return w;
            var fx = Math.Clamp(u, 0f, 1f) * (a - 1);
            var fy = Math.Clamp(v, 0f, 1f) * (a - 1);
            var x = Math.Min((int)fx, a - 2);
            var y = Math.Min((int)fy, a - 2);
            var tx = fx - x;
            var ty = fy - y;
            for (var l = 0; l < AlphaLayers; l++)
                w[l] = ((Weight(l, x, y) * (1 - tx) + Weight(l, x + 1, y) * tx) * (1 - ty) + (Weight(l, x, y + 1) * (1 - tx) + Weight(l, x + 1, y + 1) * tx) * ty) / 255.0;
            return w;
        }

        /// <summary>The hole cell (heightmap quad) under (u, v); true when it is cut out.</summary>
        public bool HoleAt(float u, float v)
        {
            var x = Math.Clamp((int)(u * HolesRes), 0, HolesRes - 1);
            var y = Math.Clamp((int)(v * HolesRes), 0, HolesRes - 1);
            return Holes[y * HolesRes + x] == 0;
        }

        public int DetailCell(float u) => Math.Clamp((int)(u * DetailRes), 0, DetailRes - 1);

        public ushort Detail(int layer, int x, int z) => Details[(layer * DetailRes + z) * DetailRes + x];

        public int HoleCount => Holes.Count(b => b == 0);

        public double SettingNumber(string key) => Settings.TryGetValue(key, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : double.NaN;
    }
}
