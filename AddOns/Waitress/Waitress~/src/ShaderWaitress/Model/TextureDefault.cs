using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// What a Texture2D property samples as while nothing is assigned to it
    /// (<c>Texture2DShaderProperty.DefaultType</c>). The default is White, and White read as a
    /// normal map unpacks to a tilted normal, so a normal map slot left empty lights wrongly.
    /// </summary>
    public static class TextureDefaults
    {
        // Order is the serialized value; the names are Shader Graph's own.
        static readonly (string cli, string[] aliases)[] s_Names =
        {
            ("white", Array.Empty<string>()),
            ("black", Array.Empty<string>()),
            ("grey", new[] { "gray" }),
            ("normal-map", new[] { "normalmap", "normal", "bump" }),
            ("linear-grey", new[] { "lineargrey", "lineargray", "linear-gray" }),
            ("red", Array.Empty<string>()),
        };

        public static IEnumerable<string> AllNames => s_Names.Select(n => n.cli);

        public static string Name(int value) =>
            value >= 0 && value < s_Names.Length ? s_Names[value].cli : value.ToString();

        public static int Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ShaderWaitressException("missing texture default");
            var key = text.Trim().ToLowerInvariant().Replace("_", "-");
            for (var i = 0; i < s_Names.Length; i++)
            {
                if (key == s_Names[i].cli || s_Names[i].aliases.Contains(key))
                    return i;
            }
            throw new ShaderWaitressException(
                $"'{text}' is not a texture default. Options: {string.Join(", ", AllNames)}");
        }
    }
}
