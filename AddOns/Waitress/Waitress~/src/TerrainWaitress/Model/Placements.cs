using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TerrainWaitress.Model
{
    /// <summary>A Terrain component in a scene or prefab file, read from its YAML.</summary>
    public sealed class Placement
    {
        public string File;          // asset path of the scene or prefab
        public string Object;        // GameObject name
        public string HierarchyPath; // Parent/Child
        public double[] Position;    // world, from the transform chain (terrains ignore rotation and scale)
        public string DataGuid;
        public bool Enabled, Active;
        public bool HasCollider, ColliderEnabled;
        public string ColliderDataGuid;
        public string MaterialGuid;
        public long MaterialFileId;
        public int GroupingId;
        public bool AutoConnect;
        public Dictionary<string, string> Fields = new Dictionary<string, string>();
        public bool Rotated, Scaled;

        public string Ref => File + ":" + HierarchyPath;
        public bool IsScene => File.EndsWith(".unity", StringComparison.OrdinalIgnoreCase);
    }

    public static class Placements
    {
        sealed class Doc
        {
            public int ClassId;
            public long Id;
            public bool Stripped;
            public string Type;
            public Dictionary<string, string> Fields = new Dictionary<string, string>();
        }

        /// <summary>Every Terrain in a scene or prefab file; files with no terrain return nothing quickly.</summary>
        public static List<Placement> Read(string fullPath, string assetPath, List<string> notes)
        {
            var result = new List<Placement>();
            string text;
            try
            {
                text = File.ReadAllText(fullPath);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                return result;
            }
            if (!text.StartsWith("%YAML", StringComparison.Ordinal) || text.IndexOf("\nTerrain:", StringComparison.Ordinal) < 0 && text.IndexOf("!u!218 ", StringComparison.Ordinal) < 0)
                return result;
            var docs = Parse(text);
            var byId = docs.GroupBy(d => d.Id).ToDictionary(g => g.Key, g => g.First());
            var transforms = docs.Where(d => d.ClassId == 4 || d.ClassId == 224).ToDictionary(d => Ref(d.Fields.GetValueOrDefault("m_GameObject")), d => d);
            foreach (var t in docs.Where(d => d.ClassId == 218))
            {
                if (t.Stripped)
                {
                    notes?.Add($"{assetPath} has a Terrain inside a prefab instance; read the prefab itself");
                    continue;
                }
                var go = Ref(t.Fields.GetValueOrDefault("m_GameObject"));
                byId.TryGetValue(go, out var goDoc);
                var p = new Placement
                {
                    File = assetPath,
                    Object = goDoc?.Fields.GetValueOrDefault("m_Name") ?? "?",
                    Enabled = t.Fields.GetValueOrDefault("m_Enabled") != "0",
                    Active = goDoc?.Fields.GetValueOrDefault("m_IsActive") != "0",
                    DataGuid = Guid(t.Fields.GetValueOrDefault("m_TerrainData")),
                    MaterialGuid = Guid(t.Fields.GetValueOrDefault("m_MaterialTemplate")),
                    MaterialFileId = Ref(t.Fields.GetValueOrDefault("m_MaterialTemplate")),
                    GroupingId = int.TryParse(t.Fields.GetValueOrDefault("m_GroupingID"), out var g) ? g : 0,
                    AutoConnect = t.Fields.GetValueOrDefault("m_AllowAutoConnect") == "1",
                };
                foreach (var f in t.Fields)
                    if (!f.Value.StartsWith("{", StringComparison.Ordinal) && f.Key.StartsWith("m_", StringComparison.Ordinal) && f.Key != "m_Enabled" && f.Key != "m_ObjectHideFlags")
                        p.Fields[f.Key.Substring(2)] = f.Value;
                var collider = docs.FirstOrDefault(d => d.ClassId == 154 && Ref(d.Fields.GetValueOrDefault("m_GameObject")) == go);
                if (collider != null)
                {
                    p.HasCollider = true;
                    p.ColliderEnabled = collider.Fields.GetValueOrDefault("m_Enabled") != "0";
                    p.ColliderDataGuid = Guid(collider.Fields.GetValueOrDefault("m_TerrainData"));
                }
                // World position through the parents; the terrain's own rotation and scale are ignored by Unity.
                var names = new List<string> { p.Object };
                double[] pos = { 0, 0, 0 };
                var first = true;
                for (var tr = transforms.GetValueOrDefault(go); tr != null;)
                {
                    var lp = Vec(tr.Fields.GetValueOrDefault("m_LocalPosition"), 0);
                    if (first)
                    {
                        pos = lp;
                        var rot = Vec(tr.Fields.GetValueOrDefault("m_LocalRotation"), 0, true);
                        p.Rotated = Math.Abs(rot[3]) < 0.99999;
                        var sc = Vec(tr.Fields.GetValueOrDefault("m_LocalScale"), 1);
                        p.Scaled = sc.Any(s => Math.Abs(s - 1) > 1e-6);
                        first = false;
                    }
                    var father = Ref(tr.Fields.GetValueOrDefault("m_Father"));
                    if (father == 0 || !byId.TryGetValue(father, out var parent))
                        break;
                    var q = Vec(parent.Fields.GetValueOrDefault("m_LocalRotation"), 0, true);
                    var s = Vec(parent.Fields.GetValueOrDefault("m_LocalScale"), 1);
                    var scaled = new[] { pos[0] * s[0], pos[1] * s[1], pos[2] * s[2] };
                    var rotated = Rotate(q, scaled);
                    var ppos = Vec(parent.Fields.GetValueOrDefault("m_LocalPosition"), 0);
                    pos = new[] { rotated[0] + ppos[0], rotated[1] + ppos[1], rotated[2] + ppos[2] };
                    var parentGo = Ref(parent.Fields.GetValueOrDefault("m_GameObject"));
                    names.Insert(0, byId.TryGetValue(parentGo, out var pg) ? pg.Fields.GetValueOrDefault("m_Name") ?? "?" : "?");
                    if (pg != null && pg.Fields.GetValueOrDefault("m_IsActive") == "0")
                        p.Active = false;
                    tr = parent;
                }
                p.Position = pos;
                p.HierarchyPath = string.Join("/", names);
                result.Add(p);
            }
            return result;
        }

        static double[] Rotate(double[] q, double[] v)
        {
            double x = q[0], y = q[1], z = q[2], w = q[3];
            double tx = 2 * (y * v[2] - z * v[1]), ty = 2 * (z * v[0] - x * v[2]), tz = 2 * (x * v[1] - y * v[0]);
            return new[] { v[0] + w * tx + (y * tz - z * ty), v[1] + w * ty + (z * tx - x * tz), v[2] + w * tz + (x * ty - y * tx) };
        }

        static List<Doc> Parse(string text)
        {
            var docs = new List<Doc>();
            Doc cur = null;
            foreach (var raw in text.Split('\n'))
            {
                var line = raw.TrimEnd('\r');
                if (line.StartsWith("--- !u!", StringComparison.Ordinal))
                {
                    var amp = line.IndexOf(" &", StringComparison.Ordinal);
                    cur = new Doc { ClassId = int.Parse(line.Substring(7, amp - 7), CultureInfo.InvariantCulture) };
                    var rest = line.Substring(amp + 2);
                    var sp = rest.IndexOf(' ');
                    cur.Id = long.Parse(sp < 0 ? rest : rest.Substring(0, sp), CultureInfo.InvariantCulture);
                    cur.Stripped = rest.Contains("stripped");
                    docs.Add(cur);
                    continue;
                }
                if (cur == null)
                    continue;
                if (cur.Type == null && line.Length > 0 && line[0] != ' ')
                {
                    cur.Type = line.TrimEnd(':');
                    continue;
                }
                if (line.Length > 2 && line[0] == ' ' && line[1] == ' ' && line[2] != ' ' && line[2] != '-')
                {
                    var colon = line.IndexOf(": ", StringComparison.Ordinal);
                    if (colon > 0)
                        cur.Fields[line.Substring(2, colon - 2)] = line.Substring(colon + 2).Trim();
                }
            }
            return docs;
        }

        static long Ref(string v)
        {
            if (v == null)
                return 0;
            var at = v.IndexOf("fileID:", StringComparison.Ordinal);
            if (at < 0)
                return 0;
            var s = at + 7;
            while (s < v.Length && v[s] == ' ')
                s++;
            var e = s;
            while (e < v.Length && (char.IsDigit(v[e]) || v[e] == '-'))
                e++;
            return long.TryParse(v.Substring(s, e - s), NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : 0;
        }

        static string Guid(string v)
        {
            if (v == null)
                return null;
            var at = v.IndexOf("guid:", StringComparison.Ordinal);
            if (at < 0)
                return null;
            return v.Substring(at + 5).Trim().Split(',', '}')[0].Trim();
        }

        static double[] Vec(string v, double fallback, bool quaternion = false)
        {
            var r = quaternion ? new double[] { 0, 0, 0, 1 } : new[] { fallback, fallback, fallback };
            if (v == null)
                return r;
            foreach (var part in v.Trim('{', '}').Split(','))
            {
                var kv = part.Split(':');
                if (kv.Length != 2 || !double.TryParse(kv[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                    continue;
                switch (kv[0].Trim())
                {
                    case "x": r[0] = d; break;
                    case "y": r[1] = d; break;
                    case "z": r[2] = d; break;
                    case "w" when quaternion: r[3] = d; break;
                }
            }
            return r;
        }
    }
}
