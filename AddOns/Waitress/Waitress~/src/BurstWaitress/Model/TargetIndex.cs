using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BurstWaitress.Model
{
    public sealed class Entry
    {
        public string Name;
        public string Kind;
        public string Interface;
        public bool HasBurst;
        public string Assembly;
        public string FloatMode;
        public string FloatPrecision;
        public string OptimizeFor;
        public bool DisableSafety;
        public bool Synchronous;

        public bool IsStatic => Kind == "static";

        /// <summary>The name without the parameter list (static methods) or interface suffix (jobs).</summary>
        public string Bare
        {
            get
            {
                var paren = IsStatic ? Name.IndexOf('(') : Name.IndexOf(" (", StringComparison.Ordinal);
                return paren > 0 ? Name.Substring(0, paren) : Name;
            }
        }
    }

    /// <summary>
    /// Every Burst entry point the Burst Inspector lists, exported by the Editor once per code epoch to
    /// UserSettings/Waitress/BurstWaitress/index.tsv, so names resolve without the Editor.
    /// </summary>
    public sealed class TargetIndex
    {
        public readonly List<Entry> Entries = new List<Entry>();
        public bool Stale;
        public string Note;

        static TargetIndex s_Loaded;

        public static TargetIndex Load()
        {
            if (s_Loaded != null)
                return s_Loaded;
            var path = Path.Combine(Project.Dir, "index.tsv");
            var stamp = Path.Combine(Project.Dir, "index.epoch");
            var epoch = Project.Epoch();
            var current = File.Exists(path) && File.Exists(stamp) && File.ReadAllText(stamp).Trim() == epoch;
            var index = new TargetIndex();
            if (!current)
            {
                try
                {
                    Directory.CreateDirectory(Project.Dir);
                    EditorScript.Call(Project.Root, "Index", path);
                    File.WriteAllText(stamp, epoch);
                }
                catch (EditorUnavailable) when (File.Exists(path))
                {
                    index.Stale = true;
                    index.Note = "the scripts changed since the target index was made and no Editor is open to refresh it; names are as of the last export";
                }
            }
            foreach (var line in File.ReadAllLines(path))
            {
                var p = line.Split('\t');
                if (p.Length < 10)
                    continue;
                index.Entries.Add(new Entry
                {
                    Name = p[0], Kind = p[1], Interface = p[2], HasBurst = p[3] == "1", Assembly = p[4], FloatMode = p[5], FloatPrecision = p[6],
                    OptimizeFor = p[7], DisableSafety = p[8] == "1", Synchronous = p[9] == "1",
                });
            }
            s_Loaded = index;
            return index;
        }

        static readonly (string, string)[] s_Primitives =
        {
            ("System.Single", "float"), ("System.Double", "double"), ("System.Int32", "int"), ("System.UInt32", "uint"), ("System.Int64", "long"),
            ("System.UInt64", "ulong"), ("System.Int16", "short"), ("System.UInt16", "ushort"), ("System.Byte", "byte"), ("System.SByte", "sbyte"),
            ("System.Boolean", "bool"), ("System.Char", "char"),
        };

        static readonly string[] s_SystemMethods = { "OnCreate", "OnUpdate", "OnDestroy", "OnStartRunning", "OnStopRunning", "OnCreateForCompiler" };

        static string Normalize(string s)
        {
            s = s.Replace('+', '.').Replace(" ", "").Replace("`1", "").Replace("`2", "").Replace("`3", "");
            foreach (var (from, to) in s_Primitives)
                s = Regex.Replace(s, @"(?<![\w.])" + Regex.Escape(from) + @"(?!\w)", to);
            return s;
        }

        /// <summary>The names an entry answers to: its full name, without parameters, ISystem methods by their plain name,
        /// and a static class by itself (which then matches all of its methods).</summary>
        static IEnumerable<string> Forms(Entry e)
        {
            yield return Normalize(e.Name);
            var bare = Normalize(e.Bare);
            yield return bare;
            if (!e.IsStatic && e.Name.Contains(" (", StringComparison.Ordinal))
                yield return Normalize(e.Bare + "-(" + e.Interface + ")");
            if (e.IsStatic)
            {
                var dot = LastDot(bare);
                var method = bare.Substring(dot + 1);
                if (method.StartsWith("__codegen__", StringComparison.Ordinal))
                    yield return bare.Substring(0, dot + 1) + method.Substring("__codegen__".Length);
            }
        }

        static IEnumerable<string> TypeForms(Entry e)
        {
            if (!e.IsStatic)
                yield break;
            var bare = Normalize(e.Bare);
            yield return bare.Substring(0, LastDot(bare));
        }

        static int LastDot(string s)
        {
            var depth = 0;
            for (var i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == '>' || s[i] == ')')
                    depth++;
                else if (s[i] == '<' || s[i] == '(')
                    depth--;
                else if (s[i] == '.' && depth == 0)
                    return i;
            }
            return -1;
        }

        static bool Matches(string form, string spec, StringComparison cmp) =>
            form.Equals(spec, cmp) || form.EndsWith("." + spec, cmp);

        /// <summary>Entry points a name stands for: a full or unambiguous short name (Type, Type.Method, Method(params)),
        /// an Inspector display name ("X - (IJobFor)"), or a glob with *.</summary>
        public List<Entry> Resolve(string spec)
        {
            var s = Normalize(spec.Trim());
            if (s.Contains('*') || s.Contains('?'))
            {
                var rx = new Regex("(^|\\.)" + Regex.Escape(s).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
                return Entries.Where(e => Forms(e).Any(f => rx.IsMatch(f)) || TypeForms(e).Any(f => rx.IsMatch(f))).ToList();
            }
            foreach (var cmp in new[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase })
            {
                var exact = Entries.Where(e => Forms(e).Any(f => f.Equals(s, cmp))).ToList();
                if (exact.Count > 0)
                    return exact;
                var suffix = Entries.Where(e => Forms(e).Any(f => Matches(f, s, cmp))).ToList();
                if (suffix.Count > 0)
                    return suffix;
                var type = Entries.Where(e => TypeForms(e).Any(f => Matches(f, s, cmp))).ToList();
                if (type.Count > 0)
                    return type;
            }
            return new List<Entry>();
        }

        /// <summary>Exactly one entry point, or a failure naming the candidates.</summary>
        public Entry One(string spec)
        {
            var list = Resolve(spec);
            if (list.Count == 1)
                return list[0];
            if (list.Count == 0)
                throw new WaitressException(NotFound(spec));
            throw new WaitressException($"'{spec}' matches {list.Count} entry points; name one:\n" + string.Join("\n", list.Take(12).Select(e => "  " + e.Name)) +
                                        (list.Count > 12 ? $"\n  ... {list.Count - 12} more (burstwaitress list \"{spec}\")" : ""));
        }

        public string NotFound(string spec)
        {
            var key = Normalize(spec);
            var dot = LastDot(key);
            var last = dot >= 0 ? key.Substring(dot + 1) : key;
            var paren = last.IndexOf('(');
            if (paren > 0)
                last = last.Substring(0, paren);
            var near = Entries.Where(e => e.Name.Contains(last, StringComparison.OrdinalIgnoreCase)).Take(6).Select(e => "  " + e.Name).ToList();
            return $"no Burst entry point matches '{spec}'" + (near.Count > 0 ? "; similar:\n" + string.Join("\n", near) : "") +
                   "\n`burstwaitress status \"" + spec + "\"` says why a type or method isn't one" + (Stale ? "\nnote: " + Note : "");
        }

        /// <summary>The shortest dotted suffix of each name that no other listed name shares, for compact tables.</summary>
        public static Dictionary<string, string> ShortNames(IEnumerable<string> names)
        {
            var list = names.Distinct().ToList();
            var result = new Dictionary<string, string>();
            foreach (var n in list)
            {
                var bare = n;
                var cut = bare.IndexOf('(');
                var tail = "";
                if (cut > 0)
                {
                    tail = bare.Substring(cut);
                    bare = bare.Substring(0, cut);
                }
                var parts = SplitDots(bare);
                var chosen = bare;
                for (var k = Math.Min(2, parts.Count); k <= parts.Count; k++)
                {
                    var candidate = string.Join(".", parts.Skip(parts.Count - k));
                    if (list.Count(o => o != n && (StripParams(o) == candidate || StripParams(o).EndsWith("." + candidate, StringComparison.Ordinal))) == 0)
                    {
                        chosen = candidate;
                        break;
                    }
                }
                var sameBare = list.Count(o => StripParams(o) == bare) > 1;
                result[n] = chosen + (sameBare ? tail : "");
            }
            return result;
        }

        static string StripParams(string n)
        {
            var cut = n.IndexOf('(');
            return cut > 0 ? n.Substring(0, cut) : n;
        }

        static List<string> SplitDots(string s)
        {
            var parts = new List<string>();
            var depth = 0;
            var start = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '<')
                    depth++;
                else if (s[i] == '>')
                    depth--;
                else if (s[i] == '.' && depth == 0)
                {
                    parts.Add(s.Substring(start, i - start));
                    start = i + 1;
                }
            }
            parts.Add(s.Substring(start));
            return parts;
        }
    }
}
