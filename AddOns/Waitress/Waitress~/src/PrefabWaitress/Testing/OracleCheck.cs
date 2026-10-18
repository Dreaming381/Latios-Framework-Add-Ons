using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PrefabWaitress.Commands;
using PrefabWaitress.Model;

namespace PrefabWaitress.Testing
{
    /// <summary>
    /// Compares the tool's effective values with a dump the Editor wrote from SerializedObject (AgentScripts/PrefabWaitress/PrefabOracle.cs).
    /// Hidden command: `oraclecheck oracle.txt [--verbose]`.
    /// </summary>
    public static class OracleCheck
    {
        static readonly Regex s_Index = new Regex(@"\[\d+\]", RegexOptions.Compiled);

        public static int Run(Session session, Args args, TextWriter w)
        {
            var verbose = args.Flag("verbose");
            var oracle = Read(args.Positional0("oracle dump"));
            int files = 0, objects = 0, missingObjects = 0, values = 0, mismatched = 0, missingPaths = 0, opaqueSkipped = 0, overridden = 0;
            var kinds = new Dictionary<string, (int count, string example)>();
            var missingKinds = new Dictionary<string, (int count, string example)>();
            foreach (var (path, objs) in oracle)
            {
                Asset asset;
                try
                {
                    asset = session.Asset(path);
                }
                catch (WaitressException e)
                {
                    w.WriteLine($"{path}: {e.Message}");
                    continue;
                }
                files++;
                foreach (var (key, (type, props)) in objs)
                {
                    objects++;
                    var id = Resolve(asset, key);
                    var obj = asset.Get(id);
                    if (obj == null && type == "Object")
                        continue;
                    if (obj == null)
                    {
                        missingObjects++;
                        Count(missingKinds, "object " + type, $"{path} &{id} (editor &{key})");
                        continue;
                    }
                    if (obj.Opaque)
                    {
                        opaqueSkipped++;
                        continue;
                    }
                    var mine = new Dictionary<string, string>();
                    foreach (var (p, v) in PropPath.Leaves(obj.Body))
                        mine[p] = Dump.Value(v);
                    foreach (var (p, v) in props)
                    {
                        if (v.StartsWith("<", StringComparison.Ordinal))
                            continue;
                        values++;
                        if (obj.SetBy.ContainsKey(p))
                            overridden++;
                        if (!mine.TryGetValue(p, out var t) && mine.TryGetValue(p + ".m_Bits", out var bits))
                            t = ((int)uint.Parse(bits, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        if (t == null)
                        {
                            missingPaths++;
                            Count(missingKinds, type + "." + s_Index.Replace(p, "[]"), $"{path} &{id} {p} = {v}");
                            continue;
                        }
                        if (!Same(t, v.StartsWith("&", StringComparison.Ordinal) ? "&" + Resolve(asset, v.Substring(1)).ToString(CultureInfo.InvariantCulture) : v))
                        {
                            mismatched++;
                            Count(kinds, type + "." + s_Index.Replace(p, "[]"), $"{path} &{id} {p}: tool {t} editor {v}{(obj.SetBy.TryGetValue(p, out var m) ? $" (override in {m.Layer.Owner.FileName})" : "")}");
                        }
                    }
                }
            }
            w.WriteLine($"{files} files, {objects} objects ({missingObjects} not found by the tool, {opaqueSkipped} from unreadable sources), {values} values ({overridden} overridden): {mismatched} differ, {missingPaths} the tool lacks");
            foreach (var (k, (c, e)) in kinds.OrderByDescending(k => k.Value.count).Take(verbose ? 200 : 25))
                w.WriteLine($"  differ {c,6} {k}   e.g. {e}");
            foreach (var (k, (c, e)) in missingKinds.OrderByDescending(k => k.Value.count).Take(verbose ? 200 : 25))
                w.WriteLine($"  lacks  {c,6} {k}   e.g. {e}");
            return mismatched == 0 && missingObjects == 0 ? 0 : 1;
        }

        /// <summary>The file-space id for the oracle's "id" or "instance:source".</summary>
        static long Resolve(Asset asset, string key)
        {
            var colon = key.IndexOf(':');
            if (colon < 0)
                return long.Parse(key, CultureInfo.InvariantCulture);
            var instance = long.Parse(key.AsSpan(0, colon), CultureInfo.InvariantCulture);
            var source = long.Parse(key.AsSpan(colon + 1), CultureInfo.InvariantCulture);
            var layer = asset.Layers.Find(l => l.Id == instance);
            return layer != null ? layer.Map(source) : Asset.InstancedId(instance, source);
        }

        static void Count(Dictionary<string, (int, string)> map, string key, string example)
        {
            map.TryGetValue(key, out var e);
            map[key] = (e.Item1 + 1, e.Item2 ?? example);
        }

        static bool Same(string tool, string editor)
        {
            if (tool == editor)
                return true;
            if (double.TryParse(tool, NumberStyles.Float, CultureInfo.InvariantCulture, out var a) && double.TryParse(editor, NumberStyles.Float, CultureInfo.InvariantCulture, out var b))
                return a == b || (float)a == (float)b || Math.Abs(a - b) <= 1e-6 * Math.Max(1, Math.Abs(b));
            if (editor == "missing" && tool != "null")
                return true;
            return false;
        }

        static List<(string path, Dictionary<string, (string type, List<(string, string)> props)>)> Read(string file)
        {
            var result = new List<(string, Dictionary<string, (string, List<(string, string)>)>)>();
            Dictionary<string, (string, List<(string, string)>)> current = null;
            List<(string, string)> props = null;
            foreach (var line in File.ReadLines(file))
            {
                if (line.StartsWith("# ", StringComparison.Ordinal))
                {
                    current = new Dictionary<string, (string, List<(string, string)>)>();
                    result.Add((line.Substring(2), current));
                    continue;
                }
                if (line.StartsWith("&", StringComparison.Ordinal) && current != null)
                {
                    var space = line.IndexOf(' ');
                    var id = line.Substring(1, space - 1);
                    var rest = line.Substring(space + 1);
                    var type = rest.Split(' ')[0];
                    props = new List<(string, string)>();
                    current[id] = (type, props);
                    continue;
                }
                if (line.StartsWith("  ", StringComparison.Ordinal) && props != null)
                {
                    var eq = line.IndexOf(" = ", StringComparison.Ordinal);
                    if (eq > 0)
                        props.Add((line.Substring(2, eq - 2), line.Substring(eq + 3)));
                }
            }
            return result;
        }
    }
}
