using System.Text.RegularExpressions;

namespace BurstWaitress.Asm
{
    /// <summary>Short forms of the symbol names Burst emits for managed methods.</summary>
    public static class Names
    {
        static readonly Regex s_Hash = new Regex(@"_[0-9a-f]{32}( from .*)?$", RegexOptions.Compiled);
        static readonly Regex s_Decor = new Regex(@"@@\d+$", RegexOptions.Compiled);
        static readonly Regex s_AsmQual = new Regex(@", [\w.]+, Version=[^,]+, Culture=[^,]+, PublicKeyToken=\w+", RegexOptions.Compiled);
        static readonly Regex s_Generic = new Regex(@"(?:`\d+)?\[\[((?:(?!\[\[|\]\]).)*)\]\]", RegexOptions.Compiled);
        static readonly Regex s_Tick = new Regex(@"`\d+", RegexOptions.Compiled);
        static readonly Regex s_Return = new Regex(@"\) -> .*$", RegexOptions.Compiled);

        static readonly (string, string)[] s_Primitives =
        {
            ("System.Single", "float"), ("System.Double", "double"), ("System.Int32", "int"), ("System.UInt32", "uint"),
            ("System.Int64", "long"), ("System.UInt64", "ulong"), ("System.Int16", "short"), ("System.UInt16", "ushort"),
            ("System.Byte", "byte"), ("System.SByte", "sbyte"), ("System.Boolean", "bool"), ("System.Void", "void"),
            ("System.IntPtr", "IntPtr"), ("Unity.Mathematics.", ""),
        };

        /// <summary>"Ns.Type.Method(float* a, int count)" from Burst's decorated symbol; other symbols minus decoration.</summary>
        public static string Pretty(string symbol)
        {
            var s = s_Decor.Replace(symbol, "");
            s = s_Hash.Replace(s, "");
            if (!s.Contains("("))
                return s;
            s = s_AsmQual.Replace(s, "");
            s = s_Return.Replace(s, ")");
            // Reflection generic syntax: Type`1[[Arg]] -> Type<Arg>, nested '+' -> '.'.
            for (var guard = 0; guard < 8 && s.Contains("[[", System.StringComparison.Ordinal); guard++)
                s = s_Generic.Replace(s, m => "<" + m.Groups[1].Value.Replace("],[", ",") + ">");
            s = s_Tick.Replace(s, "").Replace('+', '.');
            return Primitives(s);
        }

        /// <summary>System.Single to float and so on, at word boundaries only (FrustumCullSystem.SingleSplitJob stays).</summary>
        public static string Primitives(string s)
        {
            foreach (var (from, to) in s_Primitives)
                s = Regex.Replace(s, @"(?<![\w.])" + Regex.Escape(from) + (from.EndsWith(".") ? "" : @"(?!\w)"), to);
            return s;
        }

        /// <summary>The method's own name with its declaring type, without namespace or parameters: "BwKernels.Opaque".</summary>
        public static string Short(string pretty)
        {
            var paren = pretty.IndexOf('(');
            var head = paren >= 0 ? pretty.Substring(0, paren) : pretty;
            var depth = 0;
            var cut = -1;
            var seen = 0;
            for (var i = head.Length - 1; i >= 0; i--)
            {
                var c = head[i];
                if (c == '>')
                    depth++;
                else if (c == '<')
                    depth--;
                else if (c == '.' && depth == 0 && ++seen == 2)
                {
                    cut = i;
                    break;
                }
            }
            return cut >= 0 ? head.Substring(cut + 1) : head;
        }
    }
}
