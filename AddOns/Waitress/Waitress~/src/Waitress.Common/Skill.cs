using System;
using System.IO;
using System.Text;

namespace Waitress
{
    /// <summary>Prints a tool's agent guide, or installs it as a SKILL.md for agent harnesses.</summary>
    public static class Skill
    {
        public static int Write(string text, string install, TextWriter output)
        {
            if (install == null)
            {
                output.Write(text);
                return 0;
            }

            // An installed copy is read far from this binary, so it records where the binary is.
            text += Environment.NewLine + "Binary on this machine: `" +
                    System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName + "`" + Environment.NewLine;
            // A path that isn't an .md file names the skill's folder, whether or not it exists yet.
            var file = install.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ? install : Path.Combine(install, "SKILL.md");
            var directory = Path.GetDirectoryName(Path.GetFullPath(file));
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(file, text, new UTF8Encoding(false));
            output.WriteLine($"wrote {file}");
            return 0;
        }
    }
}
