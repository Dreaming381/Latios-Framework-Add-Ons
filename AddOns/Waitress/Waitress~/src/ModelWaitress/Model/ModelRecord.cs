using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ModelWaitress.Model
{
    public sealed class Setting
    {
        public string Name;
        public string Type;
        public string Value;
        public string Default;
        public bool Changed => Value != Default;
    }

    public sealed class Take
    {
        public string Name, Clip;
        public double Start, Stop, Fps;
    }

    public sealed class ClipSettings
    {
        public string Name, Take, Mask;
        public List<Setting> Settings = new List<Setting>();
        public List<(double time, string function, string text)> Events = new List<(double, string, string)>();
        public List<string> Curves = new List<string>();

        public string Get(string name) => Settings.FirstOrDefault(s => s.Name == name)?.Value;
    }

    public sealed class Node
    {
        public string Name;
        public int Parent;
        public bool Inactive;
        public double[] P, R, S;
        public List<Component> Components = new List<Component>();
        public List<int> Children = new List<int>();
    }

    public sealed class Component
    {
        public string Type;
        public JsonElement[] Data;
    }

    public sealed class SubMesh
    {
        public string Topology;
        public long IndexCount, FirstVertex, VertexCount, BaseVertex;
    }

    public sealed class MeshInfo
    {
        public string Name, Id, Index;
        public long Verts, Tris, Bindposes;
        public bool Readable;
        public int? MaxBonesPerVertex, Lods;
        public List<SubMesh> Sub = new List<SubMesh>();
        public List<(string attr, string format, int dim, int stream)> Attrs = new List<(string, string, int, int)>();
        public double[] Center, Extents;
        public List<(string name, int frames)> Shapes = new List<(string, int)>();
    }

    public sealed class AnimInfo
    {
        public string Name, Id, Wrap;
        public double Length, Fps, AngularSpeed;
        public double[] AverageSpeed;
        public bool Loop, Legacy, HumanMotion, RootCurves, MotionCurves, GenericRoot;
        public int Events, Curves;
        public List<(string path, List<string> props)> Paths = new List<(string, List<string>)>();
    }

    /// <summary>One model as the Editor exported it: importer settings with their defaults, and what the import produced.</summary>
    public sealed class ModelRecord
    {
        public string Path, Guid, Stamp, Unity, Importer, Artifact;
        public double ImportMs;
        public Dictionary<string, string> PostprocessorHash = new Dictionary<string, string>();
        public List<Setting> Settings = new List<Setting>();
        public double FileScale, UnitFactor;
        public string Unit;
        public bool HasExtraRoot, ContainsAnimation, HasEmbeddedTextures;
        public List<string> TransformPaths = new List<string>();
        public List<string> ReferencedClips = new List<string>();
        public List<Take> Takes = new List<Take>();
        public bool ClipsCustom;
        public List<ClipSettings> Clips = new List<ClipSettings>();
        public List<(string human, string bone)> HumanMap = new List<(string, string)>();
        public int HumanSkeleton;
        public Dictionary<string, string> HumanParams = new Dictionary<string, string>();
        public string RootMotionBone;
        public List<(string type, string name, string target)> Remaps = new List<(string, string, string)>();
        public List<string> SourceMaterials = new List<string>();
        public List<(string level, string text)> Messages = new List<(string, string)>();
        public List<Node> Nodes = new List<Node>();
        public List<MeshInfo> Meshes = new List<MeshInfo>();
        public List<(string name, bool valid, bool human)> Avatars = new List<(string, bool, bool)>();
        public List<AnimInfo> Anims = new List<AnimInfo>();
        public List<(string type, string name, string id)> SubAssets = new List<(string, string, string)>();
        public List<(string path, string stamp)> Deps = new List<(string, string)>();

        /// <summary>Set when the record is older than the model or its .meta and couldn't be refreshed.</summary>
        public bool Stale;

        /// <summary>The model file on disk, set when the record is loaded through the catalog.</summary>
        public string FullPath;

        FbxHeader m_Header;
        bool m_HeaderRead;

        /// <summary>The .fbx file's own header, or null for other formats and unreadable files.</summary>
        public FbxHeader Header
        {
            get
            {
                if (!m_HeaderRead)
                {
                    m_HeaderRead = true;
                    if (FullPath != null && FullPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                        m_Header = FbxHeader.Read(FullPath);
                }
                return m_Header;
            }
        }

        /// <summary>Usable as the rig asks: a Humanoid rig needs a human avatar, and Unity marks a failed mapping valid but not human.</summary>
        public bool AvatarOk((string name, bool valid, bool human) a) => a.valid && (Get("animationType") != "Human" || a.human);

        public bool IsFbx => Path?.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) == true;

        public string Get(string setting) => Settings.FirstOrDefault(s => s.Name == setting)?.Value;
        public bool Is(string setting) => Get(setting) == "true";

        public static ModelRecord Parse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var r = doc.RootElement;
            var m = new ModelRecord
            {
                Path = Str(r, "path"),
                Guid = Str(r, "guid"),
                Stamp = Str(r, "stamp"),
                Unity = Str(r, "unity"),
                Importer = Str(r, "importer"),
                Artifact = Str(r, "artifact"),
                ImportMs = Num(r, "importMs"),
            };
            if (r.TryGetProperty("postprocessorHash", out var pp))
                foreach (var p in pp.EnumerateObject())
                    m.PostprocessorHash[p.Name] = p.Value.GetString();
            foreach (var s in Arr(r, "settings"))
                m.Settings.Add(ParseSetting(s));
            if (r.TryGetProperty("file", out var f))
            {
                m.FileScale = Num(f, "fileScale");
                m.Unit = Str(f, "unit");
                m.UnitFactor = Num(f, "unitFactor");
                m.HasExtraRoot = Bool(f, "hasExtraRoot");
                m.HasEmbeddedTextures = Bool(f, "hasEmbeddedTextures");
                m.ContainsAnimation = Bool(f, "containsAnimation");
                m.TransformPaths = Strings(f, "transformPaths");
                m.ReferencedClips = Strings(f, "referencedClips");
                foreach (var t in Arr(f, "takes"))
                    m.Takes.Add(new Take { Name = Str(t, "name"), Clip = Str(t, "clip"), Start = Num(t, "start"), Stop = Num(t, "stop"), Fps = Num(t, "fps") });
            }
            m.ClipsCustom = Bool(r, "clipsCustom");
            foreach (var c in Arr(r, "clips"))
            {
                var clip = new ClipSettings { Name = Str(c, "name"), Take = Str(c, "take"), Mask = Str(c, "mask"), Curves = Strings(c, "curves") };
                foreach (var s in Arr(c, "settings"))
                    clip.Settings.Add(ParseSetting(s));
                foreach (var e in Arr(c, "events"))
                {
                    var a = e.EnumerateArray().ToArray();
                    clip.Events.Add((a[0].GetDouble(), a[1].GetString(), a[2].ValueKind == JsonValueKind.String ? a[2].GetString() : ""));
                }
                m.Clips.Add(clip);
            }
            if (r.TryGetProperty("human", out var h))
            {
                foreach (var pair in Arr(h, "map"))
                {
                    var a = pair.EnumerateArray().ToArray();
                    m.HumanMap.Add((a[0].GetString(), a[1].GetString()));
                }
                m.HumanSkeleton = (int)Num(h, "skeleton");
                foreach (var p in h.EnumerateObject())
                    if (p.Value.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False && p.Name != "skeleton")
                        m.HumanParams[p.Name] = p.Value.ValueKind == JsonValueKind.Number ? Fmt.Num(p.Value.GetDouble()) : p.Value.ValueKind == JsonValueKind.True ? "true" : "false";
                m.RootMotionBone = Str(h, "rootMotionBone");
            }
            foreach (var x in Arr(r, "remaps"))
            {
                var a = x.EnumerateArray().ToArray();
                m.Remaps.Add((S(a[0]), S(a[1]), S(a[2])));
            }
            m.SourceMaterials = Strings(r, "sourceMaterials");
            foreach (var x in Arr(r, "messages"))
            {
                var a = x.EnumerateArray().ToArray();
                m.Messages.Add((S(a[0]), S(a[1])));
            }
            foreach (var n in Arr(r, "nodes"))
            {
                var node = new Node
                {
                    Name = Str(n, "name"),
                    Parent = (int)Num(n, "parent"),
                    Inactive = Bool(n, "inactive"),
                    P = Vec(n, "p"),
                    R = Vec(n, "r"),
                    S = Vec(n, "s"),
                };
                foreach (var c in Arr(n, "c"))
                {
                    var a = c.EnumerateArray().ToArray();
                    node.Components.Add(new Component { Type = a[0].GetString(), Data = a.Skip(1).Select(e => e.Clone()).ToArray() });
                }
                m.Nodes.Add(node);
            }
            for (var i = 0; i < m.Nodes.Count; i++)
                if (m.Nodes[i].Parent >= 0)
                    m.Nodes[m.Nodes[i].Parent].Children.Add(i);
            foreach (var x in Arr(r, "meshes"))
            {
                var mesh = new MeshInfo
                {
                    Name = Str(x, "name"),
                    Id = Str(x, "id"),
                    Verts = (long)Num(x, "verts"),
                    Readable = Bool(x, "readable"),
                    Index = Str(x, "index"),
                    Tris = (long)Num(x, "tris"),
                    Bindposes = (long)Num(x, "bindposes"),
                    MaxBonesPerVertex = x.TryGetProperty("maxBonesPerVertex", out var bpv) ? bpv.GetInt32() : null,
                    Lods = x.TryGetProperty("lods", out var lods) ? lods.GetInt32() : null,
                };
                foreach (var s in Arr(x, "sub"))
                {
                    var a = s.EnumerateArray().ToArray();
                    mesh.Sub.Add(new SubMesh { Topology = a[0].GetString(), IndexCount = (long)a[1].GetDouble(), FirstVertex = (long)a[2].GetDouble(), VertexCount = (long)a[3].GetDouble(), BaseVertex = (long)a[4].GetDouble() });
                }
                foreach (var s in Arr(x, "attrs"))
                {
                    var a = s.EnumerateArray().ToArray();
                    mesh.Attrs.Add((a[0].GetString(), a[1].GetString(), (int)a[2].GetDouble(), (int)a[3].GetDouble()));
                }
                var bounds = Arr(x, "bounds").ToArray();
                if (bounds.Length == 2)
                {
                    mesh.Center = bounds[0].EnumerateArray().Select(NumOf).ToArray();
                    mesh.Extents = bounds[1].EnumerateArray().Select(NumOf).ToArray();
                }
                foreach (var s in Arr(x, "shapes"))
                {
                    var a = s.EnumerateArray().ToArray();
                    mesh.Shapes.Add((a[0].GetString(), (int)a[1].GetDouble()));
                }
                m.Meshes.Add(mesh);
            }
            foreach (var x in Arr(r, "avatars"))
                m.Avatars.Add((Str(x, "name"), Bool(x, "valid"), Bool(x, "human")));
            foreach (var x in Arr(r, "anims"))
            {
                var anim = new AnimInfo
                {
                    Name = Str(x, "name"),
                    Id = Str(x, "id"),
                    Length = Num(x, "length"),
                    Fps = Num(x, "fps"),
                    Loop = Bool(x, "loop"),
                    Legacy = Bool(x, "legacy"),
                    Wrap = Str(x, "wrap"),
                    HumanMotion = Bool(x, "humanMotion"),
                    RootCurves = Bool(x, "rootCurves"),
                    MotionCurves = Bool(x, "motionCurves"),
                    GenericRoot = Bool(x, "genericRoot"),
                    AverageSpeed = Vec(x, "averageSpeed"),
                    AngularSpeed = Num(x, "averageAngularSpeed"),
                    Events = (int)Num(x, "events"),
                    Curves = (int)Num(x, "curves"),
                };
                foreach (var p in Arr(x, "paths"))
                {
                    var a = p.EnumerateArray().Select(S).ToList();
                    anim.Paths.Add((a[0], a.Skip(1).ToList()));
                }
                m.Anims.Add(anim);
            }
            foreach (var x in Arr(r, "deps"))
            {
                var a = x.EnumerateArray().ToArray();
                m.Deps.Add((S(a[0]), S(a[1])));
            }
            foreach (var x in Arr(r, "subassets"))
            {
                var a = x.EnumerateArray().ToArray();
                m.SubAssets.Add((S(a[0]), S(a[1]), S(a[2])));
            }
            return m;
        }

        static Setting ParseSetting(JsonElement s)
        {
            var a = s.EnumerateArray().ToArray();
            return new Setting { Name = a[0].GetString(), Type = a[1].GetString(), Value = S(a[2]), Default = S(a[3]) };
        }

        static string S(JsonElement e) => e.ValueKind == JsonValueKind.String ? e.GetString() : e.ValueKind == JsonValueKind.Null ? null : e.ToString();
        static double NumOf(JsonElement e) => e.ValueKind == JsonValueKind.Number ? e.GetDouble() : 0;
        static string Str(JsonElement e, string key) => e.TryGetProperty(key, out var v) ? S(v) : null;
        static double Num(JsonElement e, string key) => e.TryGetProperty(key, out var v) ? NumOf(v) : 0;
        static bool Bool(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.True;
        static double[] Vec(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray().Select(NumOf).ToArray() : new double[3];
        static IEnumerable<JsonElement> Arr(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray() : Enumerable.Empty<JsonElement>();
        static List<string> Strings(JsonElement e, string key) => Arr(e, key).Select(S).ToList();

        public string PathOf(int node)
        {
            var parts = new List<string>();
            for (var i = node; i > 0 && i < Nodes.Count; i = Nodes[i].Parent)
                parts.Add(Nodes[i].Name);
            parts.Reverse();
            return string.Join("/", parts);
        }
    }

    /// <summary>The project-wide part of an export: model postprocessors, default presets and enum names.</summary>
    public sealed class ProjectRecord
    {
        public string Unity;
        public List<(string type, string assembly, List<string> callbacks, int? order, int? version, string script)> Postprocessors = new List<(string, string, List<string>, int?, int?, string)>();
        public List<(string filter, string preset, bool enabled)> Presets = new List<(string, string, bool)>();
        public Dictionary<string, List<string>> Enums = new Dictionary<string, List<string>>();

        public static ProjectRecord Parse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var r = doc.RootElement;
            var p = new ProjectRecord { Unity = r.TryGetProperty("unity", out var u) ? u.GetString() : null };
            if (r.TryGetProperty("postprocessors", out var pps))
                foreach (var x in pps.EnumerateArray())
                    p.Postprocessors.Add((x.GetProperty("type").GetString(), x.GetProperty("assembly").GetString(),
                                          x.GetProperty("callbacks").EnumerateArray().Select(c => c.GetString()).ToList(),
                                          x.TryGetProperty("order", out var o) ? o.GetInt32() : null,
                                          x.TryGetProperty("version", out var v) ? (int)v.GetDouble() : null,
                                          x.TryGetProperty("script", out var s) ? s.GetString() : null));
            if (r.TryGetProperty("presets", out var presets))
                foreach (var x in presets.EnumerateArray())
                {
                    var a = x.EnumerateArray().ToArray();
                    p.Presets.Add((a[0].GetString(), a[1].ValueKind == JsonValueKind.String ? a[1].GetString() : null, a[2].ValueKind == JsonValueKind.True));
                }
            if (r.TryGetProperty("enums", out var enums))
                foreach (var e in enums.EnumerateObject())
                    p.Enums[e.Name] = e.Value.EnumerateArray().Select(n => n.GetString()).ToList();
            return p;
        }
    }
}
