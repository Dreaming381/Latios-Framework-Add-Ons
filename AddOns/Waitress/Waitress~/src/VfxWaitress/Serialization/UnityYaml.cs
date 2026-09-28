using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VfxWaitress.Serialization
{
    /// <summary>
    /// Reader and writer for the restricted YAML dialect Unity uses for serialized assets.
    ///
    /// Every node keeps the exact source text it was parsed from, and a node that was never
    /// written is emitted from that text verbatim. Only edited nodes are regenerated, so the
    /// tool can never reformat a field it did not touch — which matters because Unity folds
    /// long plain scalars at a column width that is not worth reproducing.
    /// </summary>
    public abstract class YNode
    {
        string m_Raw;
        bool m_Dirty;

        internal YNode Parent;

        protected YNode(string raw)
        {
            m_Raw = raw;
        }

        /// <summary>The exact text of this node, including its trailing newline.</summary>
        public string Raw
        {
            get
            {
                if (m_Dirty)
                {
                    m_Raw = Regenerate();
                    m_Dirty = false;
                }
                return m_Raw;
            }
        }

        public bool Dirty => m_Dirty;

        protected abstract string Regenerate();

        protected void MarkDirty()
        {
            for (var n = this; n != null; n = n.Parent)
                n.m_Dirty = true;
        }

        internal void SetRaw(string raw)
        {
            m_Raw = raw;
            m_Dirty = false;
            for (var n = Parent; n != null; n = n.Parent)
                n.m_Dirty = true;
        }
    }

    /// <summary>A leaf value: everything after <c>key: </c>, plus any folded continuation lines.</summary>
    public sealed class YScalar : YNode
    {
        public YScalar(string raw) : base(raw) { }

        protected override string Regenerate() => Raw;

        /// <summary>The value with folding undone and surrounding whitespace removed.</summary>
        public string Text
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var line in Raw.Split('\n'))
                {
                    var t = line.Trim();
                    if (t.Length == 0)
                        continue;
                    if (sb.Length > 0)
                        sb.Append(' ');
                    sb.Append(t);
                }
                return sb.ToString();
            }
        }
    }

    public sealed class YMapEntry
    {
        public string Key;
        public string Indent;
        public YNode Value;
        /// <summary>Text between the key and the value on the key line: ": " or ":" then newline.</summary>
        public bool ValueOnKeyLine;
    }

    public sealed class YMap : YNode
    {
        public readonly List<YMapEntry> Entries = new List<YMapEntry>();

        public YMap(string raw) : base(raw) { }

        protected override string Regenerate()
        {
            var sb = new StringBuilder();
            foreach (var e in Entries)
            {
                sb.Append(e.Indent).Append(e.Key).Append(':');
                if (e.ValueOnKeyLine)
                    sb.Append(e.Value.Raw);
                else
                    sb.Append('\n').Append(e.Value.Raw);
            }
            return sb.ToString();
        }

        public YMapEntry Find(string key) => Entries.FirstOrDefault(e => e.Key == key);

        public YNode this[string key] => Find(key)?.Value;

        public string Scalar(string key) => (Find(key)?.Value as YScalar)?.Text;

        public YSeq Seq(string key) => Find(key)?.Value as YSeq;

        public YMap Map(string key) => Find(key)?.Value as YMap;

        /// <summary>Replaces a scalar value, or adds the key at the end when it is absent.</summary>
        public void SetScalar(string key, string value)
        {
            var entry = Find(key);
            if (entry == null)
            {
                var indent = Entries.Count > 0 ? Entries[0].Indent : "  ";
                Entries.Add(new YMapEntry
                {
                    Key = key,
                    Indent = indent,
                    ValueOnKeyLine = true,
                    Value = Attach(new YScalar(" " + value + "\n")),
                });
                MarkDirty();
                return;
            }
            entry.ValueOnKeyLine = true;
            entry.Value = Attach(new YScalar(" " + value + "\n"));
            MarkDirty();
        }

        /// <summary>Replaces a key's whole block with pre-rendered lines (each newline-terminated).</summary>
        public void SetBlock(string key, string blockText)
        {
            var entry = Find(key);
            if (entry == null)
            {
                var indent = Entries.Count > 0 ? Entries[0].Indent : "  ";
                entry = new YMapEntry { Key = key, Indent = indent };
                Entries.Add(entry);
            }
            entry.ValueOnKeyLine = false;
            entry.Value = Attach(BlockNode(blockText));
            MarkDirty();
        }

        /// <summary>
        /// A block of "- " lines has to be stored as a sequence. As opaque text, the next read of
        /// that key would find nothing, and a second edit in the same run would wipe the list.
        /// </summary>
        static YNode BlockNode(string blockText)
        {
            var items = new List<string>();
            var indent = "  ";
            foreach (var line in blockText.Split('\n'))
            {
                if (line.Length == 0)
                    continue;
                var trimmed = line.TrimStart();
                if (!trimmed.StartsWith("- ", StringComparison.Ordinal))
                    return new YSeqOrMapText(blockText);
                if (items.Count == 0)
                    indent = line.Substring(0, line.Length - trimmed.Length);
                items.Add(line + "\n");
            }
            if (items.Count == 0)
                return new YSeqOrMapText(blockText);
            var seq = new YSeq(blockText) { Indent = indent };
            seq.Items.AddRange(items);
            return seq;
        }

        public void Remove(string key)
        {
            var entry = Find(key);
            if (entry == null)
                return;
            Entries.Remove(entry);
            MarkDirty();
        }

        internal T Attach<T>(T node) where T : YNode
        {
            node.Parent = this;
            return node;
        }

        internal void Touch() => MarkDirty();
    }

    /// <summary>A block whose inner structure the tool renders itself rather than parsing.</summary>
    public sealed class YSeqOrMapText : YNode
    {
        public YSeqOrMapText(string raw) : base(raw) { }
        protected override string Regenerate() => Raw;
    }

    public sealed class YSeq : YNode
    {
        /// <summary>Each item's exact text, starting at its <c>- </c> and newline-terminated.</summary>
        public readonly List<string> Items = new List<string>();
        public string Indent = "  ";
        public bool Empty;

        public YSeq(string raw) : base(raw) { }

        protected override string Regenerate()
        {
            if (Items.Count == 0)
                return " []\n";
            var sb = new StringBuilder();
            foreach (var item in Items)
                sb.Append(item);
            return sb.ToString();
        }

        /// <summary>For the common <c>- {fileID: n}</c> shape, the ids in order.</summary>
        public List<long> FileIds()
        {
            var result = new List<long>();
            foreach (var item in Items)
            {
                var id = UnityYaml.FileIdIn(item);
                if (id.HasValue)
                    result.Add(id.Value);
            }
            return result;
        }

        public void SetFileIds(IEnumerable<long> ids)
        {
            Items.Clear();
            foreach (var id in ids)
                Items.Add(Indent + "- {fileID: " + id.ToString(CultureInfo.InvariantCulture) + "}\n");
            Empty = Items.Count == 0;
            MarkDirty();
        }

        public void AddFileId(long id)
        {
            var list = FileIds();
            list.Add(id);
            SetFileIds(list);
        }

        public void RemoveFileId(long id)
        {
            var list = FileIds();
            list.Remove(id);
            SetFileIds(list);
        }

        public void Touch() => MarkDirty();

        public void AddRawItem(string text)
        {
            Items.Add(text);
            MarkDirty();
        }
    }

    public sealed class YamlDocument
    {
        /// <summary>The <c>--- !u!114 &amp;58</c> line, without its newline.</summary>
        public string AnchorLine;
        public int ClassId;
        public long FileId;
        /// <summary>The <c>MonoBehaviour:</c> line, without its newline.</summary>
        public string TypeLine;
        public string TypeName;
        public YMap Body;

        public string Write(string nl)
        {
            var sb = new StringBuilder();
            sb.Append(AnchorLine).Append(nl == "\n" ? "\n" : nl);
            sb.Append(TypeLine).Append(nl == "\n" ? "\n" : nl);
            sb.Append(Body.Raw);
            return sb.ToString();
        }
    }

    public sealed class YamlFile
    {
        public string Header;
        public readonly List<YamlDocument> Documents = new List<YamlDocument>();
        public bool Crlf;

        public YamlDocument ById(long fileId) => Documents.FirstOrDefault(d => d.FileId == fileId);

        public string Write()
        {
            var sb = new StringBuilder();
            sb.Append(Header);
            foreach (var d in Documents)
            {
                sb.Append(d.AnchorLine).Append('\n');
                sb.Append(d.TypeLine).Append('\n');
                sb.Append(d.Body.Raw);
            }
            var text = sb.ToString();
            return Crlf ? text.Replace("\n", "\r\n") : text;
        }
    }

    public static class UnityYaml
    {
        public static YamlFile Parse(string text)
        {
            var file = new YamlFile();
            if (text.Contains("\r\n"))
            {
                file.Crlf = true;
                text = text.Replace("\r\n", "\n");
            }

            var lines = text.Split('\n');
            var i = 0;
            var header = new StringBuilder();
            while (i < lines.Length && !lines[i].StartsWith("--- ", StringComparison.Ordinal))
            {
                header.Append(lines[i]).Append('\n');
                i++;
            }
            file.Header = header.ToString();

            while (i < lines.Length)
            {
                if (!lines[i].StartsWith("--- ", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }
                var doc = new YamlDocument { AnchorLine = lines[i] };
                ParseAnchor(doc);
                i++;
                if (i >= lines.Length)
                    throw new VfxWaitressException("truncated document at " + doc.AnchorLine);
                doc.TypeLine = lines[i];
                doc.TypeName = doc.TypeLine.TrimEnd().TrimEnd(':');
                i++;

                var start = i;
                while (i < lines.Length && !lines[i].StartsWith("--- ", StringComparison.Ordinal))
                    i++;
                var end = i;
                // A file ending in a newline produces one trailing empty element; it is not a line.
                if (end == lines.Length && end > start && lines[end - 1].Length == 0)
                    end--;

                doc.Body = ParseMap(lines, start, end, IndentOf(lines, start, end));
                file.Documents.Add(doc);
            }
            return file;
        }

        static void ParseAnchor(YamlDocument doc)
        {
            var line = doc.AnchorLine;
            var bang = line.IndexOf("!u!", StringComparison.Ordinal);
            var amp = line.IndexOf('&');
            if (bang < 0 || amp < 0)
                throw new VfxWaitressException("unrecognised document header: " + line);
            var classPart = line.Substring(bang + 3, amp - bang - 3).Trim();
            if (!int.TryParse(classPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var classId))
                throw new VfxWaitressException("unrecognised class id in: " + line);
            doc.ClassId = classId;
            var idPart = line.Substring(amp + 1).Trim();
            var space = idPart.IndexOf(' ');
            if (space >= 0)
                idPart = idPart.Substring(0, space);
            if (!long.TryParse(idPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fileId))
                throw new VfxWaitressException("unrecognised file id in: " + line);
            doc.FileId = fileId;
        }

        static char OpeningQuote(string rest)
        {
            var t = rest.TrimStart(' ');
            return t.Length > 0 && (t[0] == '\'' || t[0] == '"') ? t[0] : '\0';
        }

        static bool QuoteStillOpen(string text, char quote)
        {
            var start = text.IndexOf(quote);
            if (start < 0)
                return false;
            var i = start + 1;
            while (i < text.Length)
            {
                if (quote == '"' && text[i] == '\\')
                {
                    i += 2;
                    continue;
                }
                if (text[i] == quote)
                {
                    if (quote == '\'' && i + 1 < text.Length && text[i + 1] == '\'')
                    {
                        i += 2;
                        continue;
                    }
                    return false;
                }
                i++;
            }
            return true;
        }

        static bool IsSeqItemAt(string line, int indent) =>
            LeadingSpaces(line) == indent && (line.TrimStart().StartsWith("- ", StringComparison.Ordinal) || line.Trim() == "-");

        static bool NextIsSiblingSeq(string[] lines, int start, int end, int indent)
        {
            for (var i = start; i < end; i++)
            {
                if (lines[i].Trim().Length == 0)
                    continue;
                return IsSeqItemAt(lines[i], indent);
            }
            return false;
        }

        static bool NextIsDeeper(string[] lines, int start, int end, int indent)
        {
            for (var i = start; i < end; i++)
            {
                if (lines[i].Trim().Length == 0)
                    continue;
                return LeadingSpaces(lines[i]) > indent;
            }
            return false;
        }

        static int IndentOf(string[] lines, int start, int end)
        {
            for (var i = start; i < end; i++)
            {
                if (lines[i].Trim().Length == 0)
                    continue;
                return LeadingSpaces(lines[i]);
            }
            return 2;
        }

        static int LeadingSpaces(string line)
        {
            var n = 0;
            while (n < line.Length && line[n] == ' ')
                n++;
            return n;
        }

        static YMap ParseMap(string[] lines, int start, int end, int indent)
        {
            var map = new YMap(Join(lines, start, end));
            var i = start;
            while (i < end)
            {
                var line = lines[i];
                if (line.Trim().Length == 0)
                {
                    if (map.Entries.Count > 0)
                    {
                        var prev = map.Entries[map.Entries.Count - 1];
                        prev.Value.SetRaw(prev.Value.Raw + line + "\n");
                    }
                    i++;
                    continue;
                }
                var colon = KeyColon(line, indent);
                if (colon < 0)
                {
                    // Not a key line at this level; fold it into the previous entry's raw text.
                    if (map.Entries.Count == 0)
                        throw new VfxWaitressException("unexpected line: " + line);
                    var last = map.Entries[map.Entries.Count - 1];
                    last.Value.SetRaw(last.Value.Raw + line + "\n");
                    i++;
                    continue;
                }

                var key = line.Substring(indent, colon - indent);
                var rest = line.Substring(colon + 1);
                var entry = new YMapEntry { Key = key, Indent = new string(' ', indent) };

                // A key introduces a block when the next non-blank line is indented past it, or
                // when it is a sequence item at the key's own indent, which is how Unity writes
                // the sequences that hang off a document root.
                var isBlock = rest.Trim().Length == 0 &&
                    (NextIsDeeper(lines, i + 1, end, indent) || NextIsSiblingSeq(lines, i + 1, end, indent));
                if (!isBlock)
                {
                    // Scalar or flow value, possibly folded over following deeper-indented lines.
                    // The separator after the colon is part of the value, so an empty scalar keeps
                    // the trailing space Unity writes for it.
                    var sb = new StringBuilder(rest).Append('\n');
                    i++;
                    // A quoted scalar runs until its closing quote and may contain blank lines;
                    // an unquoted one is folded and ends at the first line that is not deeper.
                    var quote = OpeningQuote(rest);
                    if (quote != '\0')
                    {
                        while (i < end && QuoteStillOpen(sb.ToString(), quote))
                        {
                            sb.Append(lines[i]).Append('\n');
                            i++;
                        }
                    }
                    else
                    {
                        while (i < end && lines[i].Trim().Length > 0 && LeadingSpaces(lines[i]) > indent && KeyColon(lines[i], LeadingSpaces(lines[i])) < 0 && !lines[i].TrimStart().StartsWith("- ", StringComparison.Ordinal))
                        {
                            sb.Append(lines[i]).Append('\n');
                            i++;
                        }
                    }
                    entry.ValueOnKeyLine = true;
                    entry.Value = map.Attach(new YScalar(sb.ToString()));
                    map.Entries.Add(entry);
                    continue;
                }

                // Block value: gather every following line indented past the key, plus sequence
                // items at the key's own indent.
                var blockStart = i + 1;
                var j = blockStart;
                while (j < end && (lines[j].Trim().Length == 0 || LeadingSpaces(lines[j]) > indent || IsSeqItemAt(lines[j], indent)))
                    j++;
                // Blank lines that trail the block belong to whatever follows, not to it.
                while (j > blockStart && lines[j - 1].Trim().Length == 0)
                    j--;
                entry.ValueOnKeyLine = false;
                entry.Value = map.Attach(ParseBlock(lines, blockStart, j, indent));
                map.Entries.Add(entry);
                i = j;
            }
            return map;
        }

        static YNode ParseBlock(string[] lines, int start, int end, int parentIndent)
        {
            var first = -1;
            for (var i = start; i < end; i++)
            {
                if (lines[i].Trim().Length > 0)
                {
                    first = i;
                    break;
                }
            }
            if (first < 0)
                return new YSeqOrMapText(Join(lines, start, end));

            var indent = LeadingSpaces(lines[first]);
            if (lines[first].TrimStart().StartsWith("- ", StringComparison.Ordinal) || lines[first].Trim() == "-")
                return ParseSeq(lines, start, end, indent);
            if (KeyColon(lines[first], indent) >= 0)
                return ParseMap(lines, start, end, indent);
            return new YSeqOrMapText(Join(lines, start, end));
        }

        static YSeq ParseSeq(string[] lines, int start, int end, int indent)
        {
            var seq = new YSeq(Join(lines, start, end)) { Indent = new string(' ', indent) };
            var i = start;
            string current = null;
            while (i < end)
            {
                var line = lines[i];
                var isItem = LeadingSpaces(line) == indent && (line.TrimStart().StartsWith("- ", StringComparison.Ordinal) || line.Trim() == "-");
                if (isItem)
                {
                    if (current != null)
                        seq.Items.Add(current);
                    current = line + "\n";
                }
                else if (current != null)
                {
                    current += line + "\n";
                }
                i++;
            }
            if (current != null)
                seq.Items.Add(current);
            return seq;
        }

        /// <summary>
        /// Index of the colon that ends a key at <paramref name="indent"/>, or -1 when the line is
        /// not a key line. A colon inside a flow mapping or a quoted string does not count.
        /// </summary>
        static int KeyColon(string line, int indent)
        {
            if (LeadingSpaces(line) != indent)
                return -1;
            if (indent >= line.Length)
                return -1;
            if (line[indent] == '-' && (indent + 1 >= line.Length || line[indent + 1] == ' '))
                return -1;
            var inQuote = false;
            for (var i = indent; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '\'' || c == '"')
                    inQuote = !inQuote;
                else if (c == '{' || c == '[')
                    return -1;
                else if (!inQuote && c == ':')
                    return (i + 1 >= line.Length || line[i + 1] == ' ') ? i : -1;
            }
            return -1;
        }

        static string Join(string[] lines, int start, int end)
        {
            var sb = new StringBuilder();
            for (var i = start; i < end; i++)
                sb.Append(lines[i]).Append('\n');
            return sb.ToString();
        }

        static readonly System.Text.RegularExpressions.Regex s_FileId =
            new System.Text.RegularExpressions.Regex(@"fileID:\s*(-?\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);

        public static long? FileIdIn(string text)
        {
            var m = s_FileId.Match(text ?? "");
            return m.Success ? long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) : (long?)null;
        }

        public static IEnumerable<long> AllFileIdsIn(string text)
        {
            foreach (System.Text.RegularExpressions.Match m in s_FileId.Matches(text ?? ""))
                yield return long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        public static string GuidIn(string text)
        {
            var m = System.Text.RegularExpressions.Regex.Match(text ?? "", @"guid:\s*([0-9a-fA-F]{32})");
            return m.Success ? m.Groups[1].Value : null;
        }
    }
}
