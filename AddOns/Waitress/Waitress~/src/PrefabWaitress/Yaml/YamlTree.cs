using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PrefabWaitress.Yaml
{
    /// <summary>A parsed value from Unity's YAML dialect: a mapping, a sequence, or a scalar.</summary>
    public abstract class YNode
    {
        public abstract YNode Clone();
    }

    public sealed class YScalar : YNode
    {
        public string Value;
        public bool Quoted;

        public YScalar(string value, bool quoted = false)
        {
            Value = value;
            Quoted = quoted;
        }

        public override YNode Clone() => new YScalar(Value, Quoted);

        public override string ToString() => Value;
    }

    public sealed class YMap : YNode
    {
        public readonly List<string> Keys = new List<string>();
        public readonly List<YNode> Values = new List<YNode>();
        public bool Flow;

        public int Count => Keys.Count;

        public int IndexOf(string key)
        {
            for (var i = 0; i < Keys.Count; i++)
                if (Keys[i] == key)
                    return i;
            return -1;
        }

        public YNode this[string key]
        {
            get
            {
                var i = IndexOf(key);
                return i < 0 ? null : Values[i];
            }
            set
            {
                var i = IndexOf(key);
                if (i < 0)
                {
                    Keys.Add(key);
                    Values.Add(value);
                }
                else
                {
                    Values[i] = value;
                }
            }
        }

        public string Scalar(string key) => (this[key] as YScalar)?.Value;

        public YMap Map(string key) => this[key] as YMap;

        public YSeq Seq(string key) => this[key] as YSeq;

        public long FileId(string key) => (this[key] as YMap).AsRef().FileId;

        public override YNode Clone()
        {
            var m = new YMap { Flow = Flow };
            m.Keys.AddRange(Keys);
            foreach (var v in Values)
                m.Values.Add(v?.Clone());
            return m;
        }
    }

    public sealed class YSeq : YNode
    {
        public readonly List<YNode> Items = new List<YNode>();

        public override YNode Clone()
        {
            var s = new YSeq();
            foreach (var v in Items)
                s.Items.Add(v?.Clone());
            return s;
        }
    }

    /// <summary>An object reference: {fileID: n} for this file, plus a guid for another asset.</summary>
    public readonly struct Ref : IEquatable<Ref>
    {
        public readonly long FileId;
        public readonly string Guid;

        public Ref(long fileId, string guid)
        {
            FileId = fileId;
            Guid = string.IsNullOrEmpty(guid) ? null : guid;
        }

        public bool IsNull => FileId == 0;
        public bool IsLocal => Guid == null;

        public bool Equals(Ref other) => FileId == other.FileId && Guid == other.Guid;
        public override bool Equals(object obj) => obj is Ref r && Equals(r);
        public override int GetHashCode() => HashCode.Combine(FileId, Guid);
        public override string ToString() => Guid == null ? $"{{fileID: {FileId}}}" : $"{{fileID: {FileId}, guid: {Guid}}}";
    }

    public static class YNodeExtensions
    {
        public static bool IsRef(this YNode node) => node is YMap m && m.Flow && m.IndexOf("fileID") == 0;

        public static Ref AsRef(this YNode node)
        {
            if (node is not YMap m)
                return default;
            var id = m.Scalar("fileID");
            long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fileId);
            return new Ref(fileId, m.Scalar("guid"));
        }

        public static YMap MakeRef(Ref r, string type = null)
        {
            var m = new YMap { Flow = true };
            m["fileID"] = new YScalar(r.FileId.ToString(CultureInfo.InvariantCulture));
            if (r.Guid != null)
            {
                m["guid"] = new YScalar(r.Guid);
                m["type"] = new YScalar(type ?? "3");
            }
            return m;
        }

        /// <summary>Compact one-line text for a value, as YAML would print it in flow style.</summary>
        public static string Flat(this YNode node, int max = 200)
        {
            var sb = new StringBuilder();
            Flat(node, sb, max);
            if (sb.Length > max)
            {
                sb.Length = max - 3;
                sb.Append("...");
            }
            return sb.ToString();
        }

        static void Flat(YNode node, StringBuilder sb, int max)
        {
            if (sb.Length > max)
                return;
            switch (node)
            {
                case null:
                    sb.Append("~");
                    break;
                case YScalar s:
                    sb.Append(Quote(s.Value));
                    break;
                case YMap m:
                    sb.Append('{');
                    for (var i = 0; i < m.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(", ");
                        sb.Append(m.Keys[i]).Append(": ");
                        Flat(m.Values[i], sb, max);
                    }
                    sb.Append('}');
                    break;
                case YSeq q:
                    sb.Append('[');
                    for (var i = 0; i < q.Items.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(", ");
                        Flat(q.Items[i], sb, max);
                    }
                    sb.Append(']');
                    break;
            }
        }

        public static string Quote(string value)
        {
            if (value == null)
                return "~";
            if (value.Length == 0)
                return "\"\"";
            foreach (var c in value)
                if (c < ' ' || c == ',' || c == '{' || c == '}' || c == '[' || c == ']' || c == '"' || c == '#')
                    return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t") + "\"";
            if (value.StartsWith(" ") || value.EndsWith(" ") || value.Contains(": "))
                return "\"" + value + "\"";
            return value;
        }
    }
}
