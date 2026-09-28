using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waitress
{
    /// <summary>
    /// The text layer of the batch language both tools share: one command per line, # or //
    /// comments, a trailing backslash to continue a line, quoting, and <c>$label</c> bindings so a
    /// node created on one line can be used on the next. What each command means is up to the tool.
    /// </summary>
    public sealed class ScriptText
    {
        readonly Func<string, Exception> m_Error;
        readonly Dictionary<string, string> m_Labels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <param name="error">Builds the tool's own exception type, so errors report like any other.</param>
        public ScriptText(Func<string, Exception> error)
        {
            m_Error = error;
        }

        public struct Statement
        {
            public string Text;
            public int FirstLine;
            public int LastLine;
            public string Where => FirstLine == LastLine ? $"line {FirstLine}" : $"lines {FirstLine}-{LastLine}";
        }

        public IEnumerable<Statement> Statements(string text)
        {
            var lineNumber = 0;
            var pending = new StringBuilder();
            var startedAt = 0;
            foreach (var raw in text.Replace("\r\n", "\n").Split('\n'))
            {
                lineNumber++;
                var line = raw.Trim();
                if (pending.Length == 0)
                {
                    if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal) || line.StartsWith("//", StringComparison.Ordinal))
                        continue;
                    startedAt = lineNumber;
                }
                if (line.EndsWith("\\", StringComparison.Ordinal))
                {
                    pending.Append(line, 0, line.Length - 1).Append(' ');
                    continue;
                }
                pending.Append(line);
                var statement = pending.ToString().Trim();
                pending.Clear();
                yield return new Statement { Text = statement, FirstLine = startedAt, LastLine = lineNumber };
            }
            if (pending.Length > 0)
                throw m_Error($"line {startedAt}: the script ends with a trailing '\\'");
        }

        /// <summary>The statement split into arguments, with every <c>$label</c> replaced.</summary>
        public List<string> Arguments(string statement) => Tokenize(statement, m_Error).Select(Substitute).ToList();

        public void Bind(string label, string value)
        {
            if (string.IsNullOrEmpty(label))
                return;
            m_Labels[label.TrimStart('$')] = value;
        }

        string Substitute(string token)
        {
            if (token.IndexOf('$') < 0)
                return token;
            var sb = new StringBuilder();
            for (var i = 0; i < token.Length; i++)
            {
                if (token[i] != '$')
                {
                    sb.Append(token[i]);
                    continue;
                }
                var start = ++i;
                while (i < token.Length && (char.IsLetterOrDigit(token[i]) || token[i] == '_'))
                    i++;
                var name = token.Substring(start, i - start);
                i--;
                if (!m_Labels.TryGetValue(name, out var value))
                    throw m_Error($"'${name}' has not been bound; add '--as {name}' to the line that creates it");
                sb.Append(value);
            }
            return sb.ToString();
        }

        public static List<string> Tokenize(string line, Func<string, Exception> error)
        {
            var tokens = new List<string>();
            var sb = new StringBuilder();
            var quote = '\0';
            var pending = false;
            foreach (var c in line)
            {
                if (quote != '\0')
                {
                    if (c == quote)
                        quote = '\0';
                    else
                        sb.Append(c);
                    continue;
                }
                if (c is '"' or '\'')
                {
                    quote = c;
                    pending = true;
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0 || pending)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        pending = false;
                    }
                    continue;
                }
                sb.Append(c);
            }
            if (quote != '\0')
                throw error("unterminated quote");
            if (sb.Length > 0 || pending)
                tokens.Add(sb.ToString());
            return tokens;
        }
    }
}
