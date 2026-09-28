using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Waitress;

namespace Waitress.Cli
{
    /// <summary>
    /// Minimal option parser. Options are <c>--name value</c>, <c>--name=value</c> or a bare
    /// <c>--name</c> switch. Switch names must be declared so that a value starting with '-'
    /// is never mistaken for the next option.
    /// </summary>
    public sealed class Args
    {
        static readonly HashSet<string> s_Switches = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "help", "h", "verbose", "v", "quiet", "q", "dry-run", "no-layout", "layout",
            "reconnect", "exposed", "hidden", "force", "report", "all", "verify",
            "recursive", "stdin", "keep-ids", "no-header", "counts", "long", "short",
            "include-defaults", "outputs", "positions", "raw", "check", "stats", "no-color",
            "brief", "starters", "notes", "no-refine", "remeasure", "with-nodes", "no-banner",
        };

        readonly List<string> m_Positional = new List<string>();
        readonly Dictionary<string, List<string>> m_Options = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> m_Used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<string> Positional => m_Positional;

        public static Args Parse(IReadOnlyList<string> argv, params string[] extraSwitches)
        {
            var switches = new HashSet<string>(s_Switches, StringComparer.OrdinalIgnoreCase);
            foreach (var s in extraSwitches)
                switches.Add(s);

            var args = new Args();
            for (var i = 0; i < argv.Count; i++)
            {
                var a = argv[i];
                if (a == "--")
                {
                    for (var k = i + 1; k < argv.Count; k++)
                        args.m_Positional.Add(argv[k]);
                    break;
                }
                // A negative number is a value, not an option. Without this, `set g n.A -1,2,-3`
                // reads as an option named "1,2,-3" and fails asking for its value.
                if (a.Length > 1 && a[0] == '-' && (char.IsDigit(a[1]) || a[1] == '.'))
                {
                    args.m_Positional.Add(a);
                    continue;
                }
                if (a.Length > 1 && a[0] == '-')
                {
                    var name = a.TrimStart('-');
                    string value = null;
                    var eq = name.IndexOf('=');
                    if (eq >= 0)
                    {
                        value = name.Substring(eq + 1);
                        name = name.Substring(0, eq);
                    }
                    else if (!switches.Contains(name))
                    {
                        if (i + 1 >= argv.Count)
                            throw new WaitressException($"option --{name} requires a value");
                        value = argv[++i];
                    }
                    args.AddOption(name, value ?? "true");
                    continue;
                }
                args.m_Positional.Add(a);
            }
            return args;
        }

        void AddOption(string name, string value)
        {
            if (!m_Options.TryGetValue(name, out var list))
            {
                list = new List<string>();
                m_Options[name] = list;
            }
            list.Add(value);
        }

        public bool Has(string name)
        {
            m_Used.Add(name);
            return m_Options.ContainsKey(name);
        }

        public bool Flag(string name)
        {
            m_Used.Add(name);
            if (!m_Options.TryGetValue(name, out var list))
                return false;
            var last = list[list.Count - 1];
            return !string.Equals(last, "false", StringComparison.OrdinalIgnoreCase) && last != "0";
        }

        public string Get(string name, string fallback = null)
        {
            m_Used.Add(name);
            return m_Options.TryGetValue(name, out var list) ? list[list.Count - 1] : fallback;
        }

        public string Require(string name)
        {
            var v = Get(name);
            if (v == null)
                throw new WaitressException($"missing required option --{name}");
            return v;
        }

        public IReadOnlyList<string> GetAll(string name)
        {
            m_Used.Add(name);
            return m_Options.TryGetValue(name, out var list) ? (IReadOnlyList<string>)list : Array.Empty<string>();
        }

        public int GetInt(string name, int fallback)
        {
            var v = Get(name);
            if (v == null)
                return fallback;
            if (!int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                throw new WaitressException($"option --{name} expects an integer, got '{v}'");
            return result;
        }

        public double GetDouble(string name, double fallback)
        {
            var v = Get(name);
            if (v == null)
                return fallback;
            if (!double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                throw new WaitressException($"option --{name} expects a number, got '{v}'");
            return result;
        }

        public string Positional0(string what)
        {
            if (m_Positional.Count < 1)
                throw new WaitressException($"missing {what}");
            return m_Positional[0];
        }

        public string PositionalAt(int index, string what)
        {
            if (m_Positional.Count <= index)
                throw new WaitressException($"missing {what}");
            return m_Positional[index];
        }

        /// <summary>Fails on options the command never looked at, so typos are not silent.</summary>
        public void RejectUnknown()
        {
            var unknown = m_Options.Keys.Where(k => !m_Used.Contains(k)).ToList();
            if (unknown.Count != 0)
                throw new WaitressException("unknown option(s): " + string.Join(", ", unknown.Select(u => "--" + u)));
        }
    }
}
