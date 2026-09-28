using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Waitress
{
    /// <summary>Case-insensitive shell-style matching for the selector language.</summary>
    public static class Glob
    {
        static readonly Dictionary<string, Regex> s_Cache = new Dictionary<string, Regex>(StringComparer.Ordinal);

        public static Func<string, bool> Compile(string pattern)
        {
            if (pattern == null)
                return _ => false;
            if (pattern.IndexOf('*') < 0 && pattern.IndexOf('?') < 0)
                return s => string.Equals(s, pattern, StringComparison.OrdinalIgnoreCase);
            var regex = RegexFor(pattern);
            return s => s != null && regex.IsMatch(s);
        }

        /// <summary>Matches when the pattern matches the whole string or its trailing segment.</summary>
        public static Func<string, bool> CompileLoose(string pattern)
        {
            var strict = Compile(pattern);
            return s =>
            {
                if (s == null)
                    return false;
                if (strict(s))
                    return true;
                var dot = s.LastIndexOf('.');
                return dot >= 0 && strict(s.Substring(dot + 1));
            };
        }

        static Regex RegexFor(string pattern)
        {
            lock (s_Cache)
            {
                if (s_Cache.TryGetValue(pattern, out var cached))
                    return cached;
                var sb = new StringBuilder("^");
                foreach (var c in pattern)
                {
                    switch (c)
                    {
                        case '*':
                            sb.Append(".*");
                            break;
                        case '?':
                            sb.Append('.');
                            break;
                        default:
                            sb.Append(Regex.Escape(c.ToString()));
                            break;
                    }
                }
                sb.Append('$');
                var regex = new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                s_Cache[pattern] = regex;
                return regex;
            }
        }
    }
}
