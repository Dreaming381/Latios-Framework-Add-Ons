using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShaderWaitress.Cli
{
    /// <summary>
    /// Embedded documentation. An agent pointed at the binary with no other context can run
    /// `help` and be productive; `skill` is the single-page version.
    /// </summary>
    public static class Help
    {
        public const string Version = "shaderwaitress 1.0.0";

        static readonly string[] s_Topics = { "format", "select", "edit", "layout", "catalog", "recipes", "ids" };

        public static void WriteIndex(TextWriter output) => output.Write(Read("index"));

        public static void WriteTopic(string topic, TextWriter output)
        {
            if (string.IsNullOrEmpty(topic))
            {
                WriteIndex(output);
                return;
            }
            var key = topic.Trim().ToLowerInvariant();
            if (key is "skill" or "agent")
            {
                output.Write(SkillText());
                return;
            }
            if (!s_Topics.Contains(key))
            {
                output.WriteLine($"no help topic '{topic}'. Topics: {string.Join(", ", s_Topics)}");
                return;
            }
            output.Write(Read(key));
        }

        public static string SkillText() => Read("skill");

        static string Read(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("." + name + ".md", StringComparison.OrdinalIgnoreCase));
            if (resource == null)
                return $"(documentation resource '{name}.md' is missing from this build)\n";
            using var stream = assembly.GetManifestResourceStream(resource);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
