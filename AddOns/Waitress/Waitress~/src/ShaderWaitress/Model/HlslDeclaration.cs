using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Where a blackboard property is declared in the generated shader. The numbers are Shader
    /// Graph's own, stored in <c>hlslDeclarationOverride</c>.
    /// </summary>
    public enum HlslDeclaration
    {
        DoNotDeclare = 0,
        Global = 1,
        UnityPerMaterial = 2,
        HybridPerInstance = 3,
    }

    public static class HlslDeclarations
    {
        static readonly (HlslDeclaration value, string cli, string[] aliases)[] s_Names =
        {
            (HlslDeclaration.DoNotDeclare, "do-not-declare", new[] { "donotdeclare", "none" }),
            (HlslDeclaration.Global, "global", Array.Empty<string>()),
            (HlslDeclaration.UnityPerMaterial, "per-material", new[] { "unitypermaterial", "permaterial", "material" }),
            (HlslDeclaration.HybridPerInstance, "hybrid-per-instance", new[] { "hybridperinstance", "perinstance", "instance", "hybrid", "dots" }),
        };

        public static string Name(HlslDeclaration value) =>
            s_Names.FirstOrDefault(n => n.value == value).cli ?? value.ToString();

        /// <summary>The Shader Graph enum member name, which is what the catalog records.</summary>
        public static string EnumName(HlslDeclaration value) => value.ToString();

        public static IEnumerable<string> AllNames => s_Names.Select(n => n.cli);

        public static HlslDeclaration Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ShaderWaitressException("missing declaration");
            var key = text.Trim().ToLowerInvariant().Replace("_", "-");
            foreach (var entry in s_Names)
            {
                if (key == entry.cli || key == entry.value.ToString().ToLowerInvariant() ||
                    entry.aliases.Contains(key.Replace("-", string.Empty)))
                    return entry.value;
            }
            throw new ShaderWaitressException(
                $"'{text}' is not an HLSL declaration. Options: {string.Join(", ", AllNames)}");
        }

        /// <summary>Matches a catalog entry's allowed list, which stores Shader Graph's names.</summary>
        public static bool IsAllowed(IReadOnlyList<string> allowed, HlslDeclaration value)
        {
            if (allowed == null || allowed.Count == 0)
                return true;    // no catalog information; do not invent a restriction
            return allowed.Any(a => string.Equals(a, value.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
