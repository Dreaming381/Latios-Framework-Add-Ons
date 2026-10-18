using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>One-line names for objects, roots and reference steps, and target lookup.</summary>
    public static class Describe
    {
        public static string Native(Model m, int i)
        {
            var s = m.S;
            var name = s.ObjName[i].Length == 0 ? "(no name)" : "\"" + Clip(s.ObjName[i], 60) + "\"";
            return $"{s.NativeTypeOf(i)} {name} #{s.ObjInstanceId[i]}";
        }

        public static string Managed(Model m, int o)
        {
            var c = m.C;
            var t = c.Type[o];
            var text = $"{ShortType(m.TypeName(t))} @{c.Address[o]:x}";
            if (m.S.IsArray(t))
                text += $" [{c.ArrayLength(c.Address[o], t)}]";
            else if (m.TypeName(t) == "System.String")
                text += " " + Quote(c.ReadString(o, 40));
            var native = c.NativeObject[o];
            if (native >= 0)
                text += $" (shell of {Native(m, native)})";
            return text;
        }

        public static string Root(Model m, Root r)
        {
            var s = m.S;
            if (r.Kind == RootKind.Static)
                return $"static {ShortType(s.TypeName[r.Index])}.{s.FieldName[r.Field]}";
            if (r.NativeObject >= 0)
                return $"GC handle of {Native(m, r.NativeObject)}";
            var target = m.C != null && r.Index < s.GcHandleTarget.Length && m.C.ByAddress.TryGetValue(s.GcHandleTarget[r.Index], out var o) ? ShortType(m.TypeName(m.C.Type[o])) : "?";
            return $"GC handle to a {target} (held by native code)";
        }

        /// <summary>How the parent reaches a child: ".field" or "[index]".</summary>
        public static string Via(Model m, int via)
        {
            if (via <= -2)
                return $"[{-via - 2}]";
            if (via >= 0)
                return "." + m.S.FieldName[via];
            return "";
        }

        /// <summary>The chain from a root down to a managed object, root first.</summary>
        public static List<string> PathToRoot(Model m, int o, int maxSteps = 40)
        {
            var c = m.C;
            var steps = new List<string>();
            var current = o;
            var guard = 0;
            while (current >= 0 && guard++ < maxSteps)
            {
                var parent = c.Parent[current];
                var via = parent < 0 && parent != Crawl.NoParent && c.Roots[-parent - 1].Kind == RootKind.Static ? "" : Via(m, c.ParentVia[current]);
                steps.Add($"{via} -> {Managed(m, current)}".TrimStart());
                if (parent < 0 && parent != Crawl.NoParent)
                {
                    steps.Add(Root(m, c.Roots[-parent - 1]));
                    break;
                }
                current = parent;
            }
            steps.Reverse();
            return steps;
        }

        /// <summary>The root a managed object hangs from and how deep, in one line.</summary>
        public static string RootSummary(Model m, int o)
        {
            var c = m.C;
            var depth = 0;
            var current = o;
            while (current >= 0 && depth < 100000)
            {
                var parent = c.Parent[current];
                if (parent < 0)
                    return parent == Crawl.NoParent ? "?" : Root(m, c.Roots[-parent - 1]) + (depth > 0 ? $" ({depth + 1} steps)" : "");
                current = parent;
                depth++;
            }
            return "?";
        }

        public static string ShortType(string name)
        {
            // Engine and BCL namespaces are dropped inside generic arguments; project namespaces stay, so filters on them stay visible.
            var lt = name.IndexOf('<');
            if (lt < 0)
                return name;
            return name.Substring(0, lt) + s_EngineNamespace.Replace(name.Substring(lt), "");
        }

        static readonly Regex s_EngineNamespace =
            new Regex(@"(?<![A-Za-z0-9_.])(?:System|UnityEngine|UnityEditor|Unity|Microsoft|UnityPipeline)\.(?:[A-Za-z_][A-Za-z0-9_]*\.)*(?=[A-Za-z_])", RegexOptions.Compiled);

        /// <summary>
        /// A type filter: a glob when it has * or ?; otherwise the types named exactly (full name, or name without namespace,
        /// either as printed) when any exist, else every type whose full name contains it.
        /// </summary>
        public static Func<string, bool> TypeMatcher(IEnumerable<string> names, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return _ => true;
            if (pattern.IndexOfAny(new[] { '*', '?' }) >= 0)
                return Glob.Compile(pattern);
            var exact = new HashSet<string>(names.Where(n => NamedBy(n, pattern)), StringComparer.Ordinal);
            if (exact.Count > 0)
                return exact.Contains;
            return n => n.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Managed types for a filter, or null for no filter: a glob matches names; a name matches that type and its subclasses;
        /// anything else matches every type whose full name contains it.
        /// </summary>
        public static HashSet<int> ManagedTypeSet(Model m, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return null;
            var s = m.S;
            var all = Enumerable.Range(0, s.TypeName.Length);
            if (pattern.IndexOfAny(new[] { '*', '?' }) >= 0)
                return new HashSet<int>(all.Where(t => Matches(s.TypeName[t], pattern)));
            var named = new HashSet<int>(all.Where(t => NamedBy(s.TypeName[t], pattern)));
            if (named.Count == 0)
                return new HashSet<int>(all.Where(t => s.TypeName[t].Contains(pattern, StringComparison.OrdinalIgnoreCase)));
            var result = new HashSet<int>(named);
            foreach (var t in all)
                for (var x = t; x >= 0 && !s.IsArray(x) && x < s.TypeBaseOrElement.Length; x = s.TypeBaseOrElement[x])
                    if (named.Contains(x))
                    {
                        result.Add(t);
                        break;
                    }
            return result;
        }

        /// <summary>True when a pattern names a type exactly: its full name or its name without namespace, raw or as printed.</summary>
        public static bool NamedBy(string name, string pattern)
        {
            if (string.Equals(name, pattern, StringComparison.OrdinalIgnoreCase))
                return true;
            var shortName = ShortType(name);
            if (string.Equals(shortName, pattern, StringComparison.OrdinalIgnoreCase))
                return true;
            var bare = Bare(name);
            return string.Equals(WithoutNamespace(name), pattern, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(WithoutNamespace(shortName), pattern, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(bare, pattern, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(WithoutNamespace(bare), pattern, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>A type name with every namespace removed from its generic arguments: List&lt;LeakNode&gt;.</summary>
        static string Bare(string name)
        {
            var lt = name.IndexOf('<');
            return lt < 0 ? name : name.Substring(0, lt) + s_AnyNamespace.Replace(name.Substring(lt), "");
        }

        static readonly Regex s_AnyNamespace = new Regex(@"[A-Za-z_][A-Za-z0-9_]*\.", RegexOptions.Compiled);

        static string WithoutNamespace(string name)
        {
            var lt = name.IndexOf('<');
            var dot = name.LastIndexOf('.', lt < 0 ? name.Length - 1 : lt);
            return dot >= 0 ? name.Substring(dot + 1) : name;
        }

        /// <summary>The --top option: rows to print, 0 for all.</summary>
        public static int Top(Args args, int fallback)
        {
            var top = args.GetInt("top", fallback);
            if (top < 0)
                throw new WaitressException("--top takes a row count, or 0 for all");
            return top;
        }

        public static string Clip(string s, int max) => s == null ? "" : s.Length <= max ? s : s.Substring(0, max - 3) + "...";

        static string Quote(string s) => s == null ? "\"?\"" : "\"" + s.Replace("\n", "\\n").Replace("\r", "") + "\"";

        public static bool Matches(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;
            if (pattern.IndexOfAny(new[] { '*', '?' }) >= 0)
                return Glob.Compile(pattern)(text);
            return text.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }

        public static IEnumerable<int> TypesMatching(Model m, string pattern) =>
            Enumerable.Range(0, m.S.TypeName.Length).Where(t => Matches(m.S.TypeName[t], pattern));

        /// <summary>
        /// A target named on the command line: #instanceId or a bare integer (Unity object), 0x... (managed object address),
        /// or text matched against Unity object names.
        /// </summary>
        public static (List<int> native, List<int> managed) Find(Model m, string what)
        {
            var native = new List<int>();
            var managed = new List<int>();
            var s = m.S;
            var idText = what.StartsWith("#") ? what.Substring(1) : what;
            if (long.TryParse(idText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                for (var i = 0; i < s.ObjInstanceId.Length; i++)
                    if (s.ObjInstanceId[i] == id)
                        native.Add(i);
                if (native.Count > 0 || what.StartsWith("#"))
                    return (native, managed);
            }
            if (what.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || what.StartsWith("@"))
            {
                var hex = what.TrimStart('@');
                if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    hex = hex.Substring(2);
                if (ulong.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var address) && m.C != null && m.C.ByAddress.TryGetValue(address, out var o))
                    managed.Add(o);
                return (native, managed);
            }
            for (var i = 0; i < s.ObjName.Length; i++)
                if (Matches(s.ObjName[i], what) && (what.IndexOfAny(new[] { '*', '?' }) >= 0 || s.ObjName[i].Equals(what, StringComparison.OrdinalIgnoreCase)))
                    native.Add(i);
            if (native.Count == 0)
                for (var i = 0; i < s.ObjName.Length; i++)
                    if (Matches(s.ObjName[i], what))
                        native.Add(i);
            return (native, managed);
        }

        public static string Flags(Model m, int i)
        {
            var f = m.S.ObjFlags[i];
            var parts = new List<string>();
            parts.Add((f & 2) != 0 ? "asset" : "runtime");
            if ((f & 1) != 0)
                parts.Add("dontdestroy");
            if ((f & 4) != 0)
                parts.Add("manager");
            var hide = m.S.ObjHideFlags[i];
            if ((hide & 52) == 52)
                parts.Add("dontsave");
            return string.Join(",", parts);
        }
    }
}
