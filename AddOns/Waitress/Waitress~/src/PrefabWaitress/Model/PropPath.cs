using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Model
{
    /// <summary>A place in a serialized object that a property path names: a mapping entry, a sequence item, or a sequence's size.</summary>
    public struct Slot
    {
        public YMap Map;
        public string Key;
        public YSeq Seq;
        public int Index;
        public bool IsSize;

        public YNode Get()
        {
            if (IsSize)
                return new YScalar(Seq.Items.Count.ToString(CultureInfo.InvariantCulture));
            if (Map != null)
                return Map[Key];
            return Seq.Items[Index];
        }

        public void Put(YNode value)
        {
            if (Map != null)
                Map[Key] = value;
            else
                Seq.Items[Index] = value;
        }
    }

    /// <summary>Unity property paths ("m_Materials.Array.data[0]") and the short form printed for them ("m_Materials[0]").</summary>
    public static class PropPath
    {
        static readonly Regex s_Short = new Regex(@"\[(\d+)\]", RegexOptions.Compiled);

        /// <summary>The Unity form of a path given in either form.</summary>
        public static string ToUnity(string path)
        {
            if (path.IndexOf('[') < 0 && !path.EndsWith(".size", StringComparison.Ordinal) && path.IndexOf(".size.", StringComparison.Ordinal) < 0)
                return path;
            var sb = new StringBuilder();
            var parts = Split(path);
            for (var i = 0; i < parts.Count; i++)
            {
                var p = parts[i];
                if (sb.Length > 0)
                    sb.Append('.');
                if (p == "Array")
                {
                    sb.Append(p).Append('.').Append(parts[++i]);
                    continue;
                }
                if (p == "size" && i > 0 && parts[i - 1] != "Array")
                {
                    sb.Append("Array.size");
                    continue;
                }
                var bracket = p.IndexOf('[');
                if (bracket > 0)
                {
                    sb.Append(p, 0, bracket);
                    foreach (Match m in s_Short.Matches(p.Substring(bracket)))
                        sb.Append(".Array.data[").Append(m.Groups[1].Value).Append(']');
                    continue;
                }
                sb.Append(p);
            }
            return sb.ToString();
        }

        /// <summary>The short form: ".Array.data[i]" becomes "[i]" and ".Array.size" becomes ".size".</summary>
        public static string ToShort(string path) => path.Replace(".Array.data[", "[").Replace(".Array.size", ".size");

        static List<string> Split(string path)
        {
            var parts = new List<string>();
            var start = 0;
            var depth = 0;
            for (var i = 0; i < path.Length; i++)
            {
                if (path[i] == '[')
                    depth++;
                else if (path[i] == ']')
                    depth--;
                else if (path[i] == '.' && depth == 0)
                {
                    parts.Add(path.Substring(start, i - start));
                    start = i + 1;
                }
            }
            parts.Add(path.Substring(start));
            return parts;
        }

        /// <summary>Finds the slot a Unity path names, or explains why it can't.</summary>
        public static bool TryResolve(YMap body, string unityPath, out Slot slot, out string missing)
        {
            slot = default;
            missing = null;
            YNode current = body;
            var parts = Split(unityPath);
            for (var i = 0; i < parts.Count; i++)
            {
                var p = parts[i];
                if (p == "Array" && i + 1 < parts.Count)
                {
                    var next = parts[++i];
                    if (current is YScalar hex && i == parts.Count - 1 && IsHexBlob(hex))
                    {
                        missing = "packed data";
                        return false;
                    }
                    if (current is not YSeq seq)
                    {
                        missing = Join(parts, i - 1) + " is not a list";
                        return false;
                    }
                    if (next == "size")
                    {
                        slot = new Slot { Seq = seq, IsSize = true };
                        if (i != parts.Count - 1)
                        {
                            missing = unityPath;
                            return false;
                        }
                        return true;
                    }
                    if (!next.StartsWith("data[", StringComparison.Ordinal) || !int.TryParse(next.AsSpan(5, next.Length - 6), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                    {
                        missing = unityPath;
                        return false;
                    }
                    if (index < 0 || index >= seq.Items.Count)
                    {
                        missing = $"{Join(parts, i + 1)} (list has {seq.Items.Count})";
                        return false;
                    }
                    slot = new Slot { Seq = seq, Index = index };
                    current = seq.Items[index];
                    continue;
                }
                if (current is not YMap map || map.IndexOf(p) < 0)
                {
                    missing = Join(parts, i + 1);
                    return false;
                }
                slot = new Slot { Map = map, Key = p };
                current = map[p];
            }
            return true;
        }

        static bool IsHexBlob(YScalar s)
        {
            if (s.Value.Length == 0 || s.Value.Length % 2 != 0)
                return false;
            foreach (var c in s.Value)
                if (!Uri.IsHexDigit(c))
                    return false;
            return true;
        }

        static string Join(List<string> parts, int count) => string.Join(".", parts.GetRange(0, Math.Min(count, parts.Count)));

        /// <summary>Every leaf of a serialized object with its Unity path, in file order.</summary>
        public static IEnumerable<(string path, YNode value)> Leaves(YMap body, string prefix = "")
        {
            for (var i = 0; i < body.Count; i++)
                foreach (var leaf in Leaves(body.Values[i], prefix.Length == 0 ? body.Keys[i] : prefix + "." + body.Keys[i]))
                    yield return leaf;
        }

        static IEnumerable<(string, YNode)> Leaves(YNode node, string path)
        {
            switch (node)
            {
                case YMap m when m.IsRef():
                    yield return (path, m);
                    break;
                case YMap m:
                    foreach (var leaf in Leaves(m, path))
                        yield return leaf;
                    break;
                case YSeq s:
                    yield return (path + ".Array.size", new YScalar(s.Items.Count.ToString(CultureInfo.InvariantCulture)));
                    for (var i = 0; i < s.Items.Count; i++)
                        foreach (var leaf in Leaves(s.Items[i], $"{path}.Array.data[{i}]"))
                            yield return leaf;
                    break;
                default:
                    yield return (path, node);
                    break;
            }
        }

        /// <summary>True when two values are the same as Unity would compare them: numbers by value, references by target.</summary>
        public static bool SameValue(YNode a, YNode b)
        {
            if (a is YScalar sa && b is YScalar sb)
                return SameScalar(sa.Value, sb.Value);
            if (a.IsRef() && b.IsRef())
                return a.AsRef().Equals(b.AsRef());
            if (a is YMap ma && b is YMap mb)
            {
                if (ma.Count != mb.Count)
                    return false;
                for (var i = 0; i < ma.Count; i++)
                    if (ma.Keys[i] != mb.Keys[i] || !SameValue(ma.Values[i], mb.Values[i]))
                        return false;
                return true;
            }
            if (a is YSeq qa && b is YSeq qb)
            {
                if (qa.Items.Count != qb.Items.Count)
                    return false;
                for (var i = 0; i < qa.Items.Count; i++)
                    if (!SameValue(qa.Items[i], qb.Items[i]))
                        return false;
                return true;
            }
            return false;
        }

        public static bool SameScalar(string a, string b)
        {
            if (a == b)
                return true;
            if (double.TryParse(a, NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                double.TryParse(b, NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
                return x == y || (float)x == (float)y;
            return false;
        }
    }
}
