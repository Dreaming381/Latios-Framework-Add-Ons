using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PrefabWaitress.Yaml
{
    public sealed class YamlDoc
    {
        public int ClassId;
        public long FileId;
        public bool Stripped;
        public string TypeName;
        public YMap Body;
        public int Line;
    }

    /// <summary>Parses the YAML subset Unity writes for scenes, prefabs and assets into a tree.</summary>
    public static class YamlReader
    {
        public static List<YamlDoc> Parse(string text, string where)
        {
            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
                if (lines[i].Length > 0 && lines[i][lines[i].Length - 1] == '\r')
                    lines[i] = lines[i].Substring(0, lines[i].Length - 1);
            var docs = new List<YamlDoc>();
            var n = 0;
            while (n < lines.Length && !lines[n].StartsWith("--- ", StringComparison.Ordinal))
                n++;
            while (n < lines.Length)
            {
                var doc = new YamlDoc { Line = n + 1 };
                ParseAnchor(lines[n], doc, where);
                n++;
                if (n >= lines.Length)
                    break;
                doc.TypeName = lines[n].TrimEnd().TrimEnd(':');
                n++;
                var end = n;
                while (end < lines.Length && !lines[end].StartsWith("--- ", StringComparison.Ordinal))
                    end++;
                try
                {
                    var p = new Parser(lines, n, end);
                    doc.Body = p.Map(Indent(lines, n, end));
                }
                catch (FormatException e)
                {
                    throw new WaitressException($"{where}:{doc.Line}: cannot read YAML ({e.Message})");
                }
                docs.Add(doc);
                n = end;
            }
            return docs;
        }

        static int Indent(string[] lines, int start, int end)
        {
            for (var i = start; i < end; i++)
                if (!IsBlank(lines[i]))
                    return Lead(lines[i]);
            return 2;
        }

        static void ParseAnchor(string line, YamlDoc doc, string where)
        {
            var bang = line.IndexOf("!u!", StringComparison.Ordinal);
            var amp = line.IndexOf('&');
            if (bang < 0 || amp < 0)
                throw new WaitressException($"{where}: not a Unity YAML document header: {line}");
            doc.ClassId = int.Parse(line.AsSpan(bang + 3, amp - bang - 3).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);
            var rest = line.Substring(amp + 1);
            var space = rest.IndexOf(' ');
            doc.FileId = long.Parse(space < 0 ? rest : rest.Substring(0, space), NumberStyles.Integer, CultureInfo.InvariantCulture);
            doc.Stripped = space >= 0 && rest.IndexOf("stripped", space, StringComparison.Ordinal) >= 0;
        }

        static bool IsBlank(string line)
        {
            foreach (var c in line)
                if (c != ' ')
                    return false;
            return true;
        }

        static int Lead(string line)
        {
            var i = 0;
            while (i < line.Length && line[i] == ' ')
                i++;
            return i;
        }

        sealed class Parser
        {
            readonly string[] m_Lines;
            readonly int m_End;
            int m_Pos;

            public Parser(string[] lines, int start, int end)
            {
                m_Lines = lines;
                m_Pos = start;
                m_End = end;
            }

            void SkipBlank()
            {
                while (m_Pos < m_End && IsBlank(m_Lines[m_Pos]))
                    m_Pos++;
            }

            bool IsItem(int line, int indent)
            {
                var s = m_Lines[line];
                return Lead(s) == indent && s.Length > indent && s[indent] == '-' && (s.Length == indent + 1 || s[indent + 1] == ' ');
            }

            public YMap Map(int indent)
            {
                var map = new YMap();
                while (true)
                {
                    SkipBlank();
                    if (m_Pos >= m_End)
                        break;
                    var line = m_Lines[m_Pos];
                    var lead = Lead(line);
                    if (lead != indent || IsItem(m_Pos, indent))
                        break;
                    var colon = KeyColon(line, indent);
                    if (colon < 0)
                        throw new FormatException("expected a key at line " + (m_Pos + 1) + ": " + line.Trim());
                    var key = line.Substring(indent, colon - indent);
                    var rest = colon + 1 < line.Length ? line.Substring(colon + 1).TrimStart(' ') : "";
                    m_Pos++;
                    if (rest.Length == 0)
                    {
                        SkipBlank();
                        if (m_Pos < m_End && IsItem(m_Pos, indent))
                            map[key] = Seq(indent);
                        else if (m_Pos < m_End && Lead(m_Lines[m_Pos]) > indent)
                        {
                            var inner = Lead(m_Lines[m_Pos]);
                            map[key] = IsItem(m_Pos, inner) ? Seq(inner) : Map(inner);
                        }
                        else
                            map[key] = new YScalar("");
                    }
                    else
                    {
                        map[key] = Inline(rest, indent);
                    }
                }
                return map;
            }

            YSeq Seq(int indent)
            {
                var seq = new YSeq();
                while (true)
                {
                    SkipBlank();
                    if (m_Pos >= m_End || !IsItem(m_Pos, indent))
                        break;
                    var line = m_Lines[m_Pos];
                    var content = line.Length > indent + 2 ? line.Substring(indent + 2) : "";
                    if (content.Length == 0)
                    {
                        m_Pos++;
                        SkipBlank();
                        if (m_Pos < m_End && Lead(m_Lines[m_Pos]) > indent)
                        {
                            var inner = Lead(m_Lines[m_Pos]);
                            seq.Items.Add(IsItem(m_Pos, inner) ? Seq(inner) : Map(inner));
                        }
                        else
                            seq.Items.Add(new YScalar(""));
                        continue;
                    }
                    var itemIndent = indent + 2;
                    if (KeyColon(new string(' ', itemIndent) + content, itemIndent) >= 0)
                    {
                        m_Lines[m_Pos] = new string(' ', itemIndent) + content;
                        seq.Items.Add(Map(itemIndent));
                    }
                    else
                    {
                        m_Pos++;
                        seq.Items.Add(Inline(content, indent));
                    }
                }
                return seq;
            }

            /// <summary>A value that starts on the current line; continuation lines are those indented past <paramref name="indent"/>.</summary>
            YNode Inline(string rest, int indent)
            {
                var first = rest[0];
                if (first == '{' || first == '[')
                {
                    var text = rest;
                    while (!Balanced(text) && m_Pos < m_End && Lead(m_Lines[m_Pos]) > indent)
                        text += " " + m_Lines[m_Pos++].Trim();
                    var i = 0;
                    return Flow(text, ref i);
                }
                if (first == '"' || first == '\'')
                {
                    var sb = new StringBuilder(rest);
                    while (!Closed(sb.ToString(), first) && m_Pos < m_End)
                        sb.Append('\n').Append(m_Lines[m_Pos++]);
                    return new YScalar(first == '"' ? DoubleQuoted(sb.ToString()) : SingleQuoted(sb.ToString()), true);
                }
                var plain = new StringBuilder(rest.TrimEnd());
                var blank = 0;
                while (m_Pos < m_End)
                {
                    var line = m_Lines[m_Pos];
                    if (IsBlank(line))
                    {
                        if (NextNonBlankDeeper(indent))
                        {
                            blank++;
                            m_Pos++;
                            continue;
                        }
                        break;
                    }
                    if (Lead(line) <= indent)
                        break;
                    if (blank > 0)
                        plain.Append('\n', blank);
                    else
                        plain.Append(' ');
                    blank = 0;
                    plain.Append(line.Trim());
                    m_Pos++;
                }
                return new YScalar(plain.ToString());
            }

            bool NextNonBlankDeeper(int indent)
            {
                for (var i = m_Pos; i < m_End; i++)
                    if (!IsBlank(m_Lines[i]))
                        return Lead(m_Lines[i]) > indent;
                return false;
            }
        }

        static bool Balanced(string text)
        {
            var depth = 0;
            var quote = '\0';
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (quote != '\0')
                {
                    if (quote == '"' && c == '\\')
                        i++;
                    else if (c == quote)
                        quote = '\0';
                    continue;
                }
                if (c == '"' || c == '\'')
                    quote = c;
                else if (c == '{' || c == '[')
                    depth++;
                else if (c == '}' || c == ']')
                    depth--;
            }
            return depth <= 0;
        }

        static bool Closed(string text, char quote)
        {
            for (var i = 1; i < text.Length; i++)
            {
                var c = text[i];
                if (quote == '"' && c == '\\')
                {
                    i++;
                    continue;
                }
                if (c == quote)
                {
                    if (quote == '\'' && i + 1 < text.Length && text[i + 1] == '\'')
                    {
                        i++;
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>Index of the colon ending a key at <paramref name="indent"/>, or -1.</summary>
        static int KeyColon(string line, int indent)
        {
            if (indent >= line.Length)
                return -1;
            var c0 = line[indent];
            if (c0 == '{' || c0 == '[' || c0 == '"' || c0 == '\'')
                return -1;
            for (var i = indent; i < line.Length; i++)
            {
                if (line[i] == ':' && (i + 1 == line.Length || line[i + 1] == ' '))
                    return i;
            }
            return -1;
        }

        static YNode Flow(string text, ref int i)
        {
            SkipSpace(text, ref i);
            if (i < text.Length && text[i] == '{')
            {
                i++;
                var map = new YMap { Flow = true };
                while (true)
                {
                    SkipSpace(text, ref i);
                    if (i >= text.Length)
                        break;
                    if (text[i] == '}')
                    {
                        i++;
                        break;
                    }
                    var keyStart = i;
                    while (i < text.Length && text[i] != ':' && text[i] != '}' && text[i] != ',')
                        i++;
                    var key = text.Substring(keyStart, i - keyStart).Trim();
                    YNode value = new YScalar("");
                    if (i < text.Length && text[i] == ':')
                    {
                        i++;
                        value = FlowValue(text, ref i);
                    }
                    map[key] = value;
                    SkipSpace(text, ref i);
                    if (i < text.Length && text[i] == ',')
                        i++;
                }
                return map;
            }
            if (i < text.Length && text[i] == '[')
            {
                i++;
                var seq = new YSeq();
                while (true)
                {
                    SkipSpace(text, ref i);
                    if (i >= text.Length)
                        break;
                    if (text[i] == ']')
                    {
                        i++;
                        break;
                    }
                    seq.Items.Add(FlowValue(text, ref i));
                    SkipSpace(text, ref i);
                    if (i < text.Length && text[i] == ',')
                        i++;
                }
                return seq;
            }
            return FlowValue(text, ref i);
        }

        static YNode FlowValue(string text, ref int i)
        {
            SkipSpace(text, ref i);
            if (i >= text.Length)
                return new YScalar("");
            var c = text[i];
            if (c == '{' || c == '[')
                return Flow(text, ref i);
            if (c == '"' || c == '\'')
            {
                var start = i;
                i++;
                while (i < text.Length)
                {
                    if (c == '"' && text[i] == '\\')
                    {
                        i += 2;
                        continue;
                    }
                    if (text[i] == c)
                    {
                        if (c == '\'' && i + 1 < text.Length && text[i + 1] == '\'')
                        {
                            i += 2;
                            continue;
                        }
                        i++;
                        break;
                    }
                    i++;
                }
                var quoted = text.Substring(start, i - start);
                return new YScalar(c == '"' ? DoubleQuoted(quoted) : SingleQuoted(quoted), true);
            }
            var s = i;
            while (i < text.Length && text[i] != ',' && text[i] != '}' && text[i] != ']')
                i++;
            return new YScalar(text.Substring(s, i - s).Trim());
        }

        static void SkipSpace(string text, ref int i)
        {
            while (i < text.Length && (text[i] == ' ' || text[i] == '\t'))
                i++;
        }

        /// <summary>Line folding for quoted scalars: a line break becomes a space, an empty line a newline.</summary>
        static string Fold(string body, bool escapes)
        {
            if (body.IndexOf('\n') < 0)
                return body;
            var lines = body.Split('\n');
            var sb = new StringBuilder();
            for (var k = 0; k < lines.Length; k++)
            {
                var line = k == 0 ? lines[k].TrimEnd(' ', '\t') : k == lines.Length - 1 ? lines[k].TrimStart(' ', '\t') : lines[k].Trim(' ', '\t');
                if (k > 0)
                {
                    if (line.Length == 0 && k < lines.Length - 1)
                    {
                        sb.Append('\n');
                        continue;
                    }
                    var prevEscaped = escapes && sb.Length > 0 && EndsWithEscape(sb);
                    if (prevEscaped)
                        sb.Length--;
                    else if (sb.Length == 0 || sb[sb.Length - 1] != '\n')
                        sb.Append(' ');
                }
                sb.Append(line);
            }
            return sb.ToString();
        }

        static bool EndsWithEscape(StringBuilder sb)
        {
            var n = 0;
            for (var i = sb.Length - 1; i >= 0 && sb[i] == '\\'; i--)
                n++;
            return n % 2 == 1;
        }

        static string SingleQuoted(string quoted)
        {
            var body = quoted.Length >= 2 ? quoted.Substring(1, quoted.Length - 2) : "";
            return Fold(body, false).Replace("''", "'");
        }

        static string DoubleQuoted(string quoted)
        {
            var body = Fold(quoted.Length >= 2 ? quoted.Substring(1, quoted.Length - 2) : "", true);
            if (body.IndexOf('\\') < 0)
                return body;
            var sb = new StringBuilder();
            for (var i = 0; i < body.Length; i++)
            {
                var c = body[i];
                if (c != '\\' || i + 1 >= body.Length)
                {
                    sb.Append(c);
                    continue;
                }
                var e = body[++i];
                switch (e)
                {
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'r': sb.Append('\r'); break;
                    case '0': sb.Append('\0'); break;
                    case 'a': sb.Append('\a'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'e': sb.Append('\x1b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'v': sb.Append('\v'); break;
                    case ' ': sb.Append(' '); break;
                    case '/': sb.Append('/'); break;
                    case 'N': sb.Append((char)0x85); break;
                    case '_': sb.Append((char)0xA0); break;
                    case 'L': sb.Append((char)0x2028); break;
                    case 'P': sb.Append((char)0x2029); break;
                    case 'x':
                        sb.Append((char)int.Parse(body.Substring(i + 1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                        i += 2;
                        break;
                    case 'u':
                        sb.Append((char)int.Parse(body.Substring(i + 1, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                        i += 4;
                        break;
                    case 'U':
                        sb.Append(char.ConvertFromUtf32(int.Parse(body.Substring(i + 1, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture)));
                        i += 8;
                        break;
                    default: sb.Append(e); break;
                }
            }
            return sb.ToString();
        }
    }
}
