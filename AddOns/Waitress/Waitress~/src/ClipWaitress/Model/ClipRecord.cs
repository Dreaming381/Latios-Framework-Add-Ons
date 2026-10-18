using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace ClipWaitress.Model
{
    /// <summary>One exported asset: an .anim file or a model, and the clips in it.</summary>
    public sealed class AssetRecord
    {
        public string Path, Guid, Stamp, Kind, Unity;
        public List<ClipRecord> Clips = new List<ClipRecord>();
        public bool Stale;
        public string FullPath;

        public static AssetRecord Parse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var r = new AssetRecord
            {
                Path = root.GetProperty("path").GetString(),
                Guid = root.GetProperty("guid").GetString(),
                Stamp = root.GetProperty("stamp").GetString(),
                Kind = root.GetProperty("kind").GetString(),
                Unity = Str(root, "unity"),
            };
            foreach (var c in root.GetProperty("clips").EnumerateArray())
                r.Clips.Add(ClipRecord.Parse(c, r));
            return r;
        }

        internal static string Str(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
        internal static double Num(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : 0;
        internal static bool Bool(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.True;
    }

    public sealed class Setting
    {
        public string Name, Type, Value, Default;
        public bool Changed => !Same(Value, Default);

        static bool Same(string a, string b) =>
            a == b || double.TryParse(a, NumberStyles.Float, CultureInfo.InvariantCulture, out var x) && double.TryParse(b, NumberStyles.Float, CultureInfo.InvariantCulture, out var y) && Math.Abs(x - y) < 1e-5;
    }

    public sealed class ClipEvent
    {
        public double Time;
        public string Function, Text, Object;
        public double Float;
        public int Int;
        public bool Optional;

        public string Param
        {
            get
            {
                var parts = new List<string>();
                if (Text != null)
                    parts.Add("\"" + Text + "\"");
                if (Float != 0)
                    parts.Add(Fmt.Num(Float) + "f");
                if (Int != 0)
                    parts.Add(Int.ToString(CultureInfo.InvariantCulture));
                if (Object != null)
                    parts.Add(Object);
                return string.Join(", ", parts);
            }
        }
    }

    public sealed class ClipRecord
    {
        public AssetRecord Asset;
        public string Name, Id, Wrap;
        public double Length, Fps, AverageAngularSpeed, ApparentSpeed, AverageDuration;
        public bool Legacy, Human, GenericRoot, MotionCurves, RootCurves, MotionFloat, Looping, Empty;
        public double[] AverageSpeed = new double[3];
        public Dictionary<string, double> Stats = new Dictionary<string, double>();
        public Dictionary<string, double> Quality;
        public List<Setting> Settings = new List<Setting>();
        public List<ClipEvent> Events = new List<ClipEvent>();
        public List<Curve> Curves = new List<Curve>();
        public List<ObjectCurve> Objects = new List<ObjectCurve>();

        /// <summary>Paths whose euler curves play in an order other than Unity's ZXY (RotationOrder: 0 XYZ ... 5 ZYX).</summary>
        public Dictionary<string, int> EulerOrder = new Dictionary<string, int>();

        public static readonly string[] OrderNames = { "XYZ", "XZY", "YZX", "YXZ", "ZXY", "ZYX" };

        /// <summary>How commands name this clip: the .anim path, or the model path with #name.</summary>
        public string Ref => Asset.Kind == "anim" ? Asset.Path : Asset.Path + "#" + Name;

        public string Get(string setting) => Settings.FirstOrDefault(s => s.Name.Equals(setting, StringComparison.OrdinalIgnoreCase))?.Value;
        public bool On(string setting) => Get(setting) == "true";
        public double Frames => Math.Round(Length * Fps, 3);
        public double Speed => Math.Sqrt(AverageSpeed.Sum(x => x * x));
        public double StartTime => double.TryParse(Get("startTime"), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0;
        public double StopTime => double.TryParse(Get("stopTime"), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : Length;
        public int Keys => Curves.Sum(c => c.Count) + Objects.Sum(o => o.Times.Length);

        public static ClipRecord Parse(JsonElement e, AssetRecord asset)
        {
            var c = new ClipRecord
            {
                Asset = asset,
                Name = e.GetProperty("name").GetString(),
                Id = e.GetProperty("id").GetString(),
                Length = AssetRecord.Num(e, "length"),
                Fps = AssetRecord.Num(e, "fps"),
                Legacy = AssetRecord.Bool(e, "legacy"),
                Wrap = AssetRecord.Str(e, "wrap"),
                Human = AssetRecord.Bool(e, "human"),
                GenericRoot = AssetRecord.Bool(e, "genericRoot"),
                MotionCurves = AssetRecord.Bool(e, "motionCurves"),
                RootCurves = AssetRecord.Bool(e, "rootCurves"),
                MotionFloat = AssetRecord.Bool(e, "motionFloat"),
                Looping = AssetRecord.Bool(e, "looping"),
                Empty = AssetRecord.Bool(e, "empty"),
                AverageAngularSpeed = AssetRecord.Num(e, "averageAngularSpeed"),
                ApparentSpeed = AssetRecord.Num(e, "apparentSpeed"),
                AverageDuration = AssetRecord.Num(e, "averageDuration"),
            };
            if (e.TryGetProperty("averageSpeed", out var speed))
                c.AverageSpeed = speed.EnumerateArray().Select(x => x.ValueKind == JsonValueKind.Number ? x.GetDouble() : 0).ToArray();
            if (e.TryGetProperty("stats", out var stats))
                foreach (var p in stats.EnumerateObject())
                    c.Stats[p.Name] = p.Value.GetDouble();
            if (e.TryGetProperty("quality", out var quality))
            {
                c.Quality = new Dictionary<string, double>();
                foreach (var p in quality.EnumerateObject())
                    c.Quality[p.Name] = p.Value.ValueKind == JsonValueKind.Number ? p.Value.GetDouble() : double.NaN;
            }
            foreach (var s in e.GetProperty("settings").EnumerateArray())
                c.Settings.Add(new Setting { Name = s[0].GetString(), Type = s[1].GetString(), Value = s[2].GetString(), Default = s[3].GetString() });
            foreach (var ev in e.GetProperty("events").EnumerateArray())
                c.Events.Add(new ClipEvent
                {
                    Time = AssetRecord.Num(ev, "t"), Function = AssetRecord.Str(ev, "f"), Text = AssetRecord.Str(ev, "s"), Float = AssetRecord.Num(ev, "n"),
                    Int = (int)AssetRecord.Num(ev, "i"), Object = AssetRecord.Str(ev, "o"), Optional = AssetRecord.Bool(ev, "optional"),
                });
            if (e.TryGetProperty("eulerOrder", out var orders))
                foreach (var p in orders.EnumerateObject())
                    c.EulerOrder[p.Name] = p.Value.GetInt32();
            foreach (var cv in e.GetProperty("curves").EnumerateArray())
                c.Curves.Add(Curve.Parse(cv));
            foreach (var o in e.GetProperty("objs").EnumerateArray())
            {
                var k = o.GetProperty("k").EnumerateArray().ToList();
                var oc = new ObjectCurve { Path = AssetRecord.Str(o, "p"), Type = AssetRecord.Str(o, "t"), Prop = AssetRecord.Str(o, "n") };
                oc.Times = Enumerable.Range(0, k.Count / 2).Select(i => k[2 * i].GetDouble()).ToArray();
                oc.Values = Enumerable.Range(0, k.Count / 2).Select(i => k[2 * i + 1].GetString()).ToArray();
                c.Objects.Add(oc);
            }
            return c;
        }
    }

    /// <summary>A curve over an object reference (sprites, materials): which object from each key's time on.</summary>
    public sealed class ObjectCurve
    {
        public string Path, Type, Prop;
        public double[] Times;
        public string[] Values;
        public string Name => Prop == "m_Sprite" ? Type + ".sprite" : Type + "." + Prop;
        public int Missing => Values.Count(v => v == "missing");

        public string At(double t)
        {
            var i = Array.FindLastIndex(Times, x => x <= t + 1e-6);
            return Values.Length == 0 ? "None" : Values[Math.Max(0, i)];
        }
    }

    /// <summary>A float curve and Unity's evaluation of it.</summary>
    public sealed class Curve
    {
        public string Path, Type, Prop, Class;
        public bool Discrete;
        public double[] T, V, In, Out;
        public int[] Mode;
        public double[] InW, OutW;

        public int Count => T.Length;

        public static Curve Parse(JsonElement e)
        {
            var c = new Curve
            {
                Path = AssetRecord.Str(e, "p"), Type = AssetRecord.Str(e, "t"), Prop = AssetRecord.Str(e, "n"), Class = AssetRecord.Str(e, "c"),
                Discrete = AssetRecord.Bool(e, "d"),
            };
            var k = e.GetProperty("k").EnumerateArray().Select(X).ToArray();
            var n = k.Length / 4;
            c.T = new double[n];
            c.V = new double[n];
            c.In = new double[n];
            c.Out = new double[n];
            for (var i = 0; i < n; i++)
            {
                c.T[i] = k[4 * i];
                c.V[i] = k[4 * i + 1];
                c.In[i] = k[4 * i + 2];
                c.Out[i] = k[4 * i + 3];
            }
            if (e.TryGetProperty("w", out var w))
            {
                var ws = w.EnumerateArray().Select(X).ToArray();
                c.Mode = new int[n];
                c.InW = new double[n];
                c.OutW = new double[n];
                for (var i = 0; i < n; i++)
                {
                    c.Mode[i] = (int)ws[3 * i];
                    c.InW[i] = ws[3 * i + 1];
                    c.OutW[i] = ws[3 * i + 2];
                }
            }
            return c;
        }

        static double X(JsonElement e)
        {
            if (e.ValueKind == JsonValueKind.Number)
                return e.GetDouble();
            return e.GetString() switch { "inf" => double.PositiveInfinity, "-inf" => double.NegativeInfinity, _ => double.NaN };
        }

        public double Min => V.Length == 0 ? 0 : V.Min();
        public double Max => V.Length == 0 ? 0 : V.Max();
        public bool Constant => V.Length == 0 || V.All(v => v == V[0]) && In.All(s => s == 0 || double.IsInfinity(s)) && Out.All(s => s == 0 || double.IsInfinity(s));
        public bool IsStepped(int i) => i < Count - 1 && (double.IsInfinity(Out[i]) || double.IsInfinity(In[i + 1]));

        /// <summary>The value at time t, clamped to the first and last key the way a clip samples its curves.</summary>
        public double Evaluate(double t)
        {
            var n = T.Length;
            if (n == 0)
                return 0;
            if (n == 1 || t <= T[0])
                return V[0];
            if (t >= T[n - 1])
                return V[n - 1];
            var hi = Array.BinarySearch(T, t);
            if (hi >= 0)
                return V[hi];
            hi = ~hi;
            var lo = hi - 1;
            if (Discrete)
                return V[lo];
            var dx = T[hi] - T[lo];
            if (dx <= 0)
                return V[lo];
            var u = (t - T[lo]) / dx;
            var m0 = Out[lo] * dx;
            var m1 = In[hi] * dx;
            if (double.IsInfinity(m0) || double.IsInfinity(m1) || double.IsNaN(m0) || double.IsNaN(m1))
                return V[lo];
            var outWeighted = Mode != null && (Mode[lo] & 2) != 0;
            var inWeighted = Mode != null && (Mode[hi] & 1) != 0;
            if (outWeighted || inWeighted)
                return Bezier(u, V[lo], m0, outWeighted ? OutW[lo] : 1.0 / 3, V[hi], m1, inWeighted ? InW[hi] : 1.0 / 3);
            var u2 = u * u;
            var u3 = u2 * u;
            return (2 * u3 - 3 * u2 + 1) * V[lo] + (u3 - 2 * u2 + u) * m0 + (u3 - u2) * m1 + (-2 * u3 + 3 * u2) * V[hi];
        }

        // Weighted tangents make the segment a 2D Bezier: find the parameter whose x is u, then return its y.
        static double Bezier(double u, double v0, double m0, double w0, double v1, double m1, double w1)
        {
            double X(double s) => 3 * w0 * s * (1 - s) * (1 - s) + 3 * (1 - w1) * s * s * (1 - s) + s * s * s;
            double lo = 0, hi = 1, s = u;
            for (var i = 0; i < 60; i++)
            {
                s = (lo + hi) / 2;
                if (X(s) < u)
                    lo = s;
                else
                    hi = s;
            }
            var p1 = v0 + w0 * m0;
            var p2 = v1 - w1 * m1;
            var r = 1 - s;
            return r * r * r * v0 + 3 * r * r * s * p1 + 3 * r * s * s * p2 + s * s * s * v1;
        }
    }
}
