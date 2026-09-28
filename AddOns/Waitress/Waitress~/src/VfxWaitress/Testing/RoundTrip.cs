using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VfxWaitress.Serialization;

namespace VfxWaitress.Testing
{
    /// <summary>
    /// Parses and re-serializes every VFX asset under the given roots and requires byte-identical
    /// output. Everything else the tool does rests on this, so point it at as many real graphs as
    /// possible.
    /// </summary>
    public static class RoundTrip
    {
        public static readonly string[] Extensions = { ".vfx", ".vfxoperator", ".vfxblock" };

        public static IEnumerable<string> Find(IEnumerable<string> roots)
        {
            foreach (var root in roots)
            {
                if (File.Exists(root))
                {
                    yield return root;
                    continue;
                }
                if (!Directory.Exists(root))
                    throw new VfxWaitressException("no such file or directory: " + root);
                foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                {
                    if (Extensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
                        yield return path;
                }
            }
        }

        public static int Run(IReadOnlyList<string> roots, TextWriter output, bool verbose)
        {
            var files = Find(roots).OrderBy(o => o, StringComparer.OrdinalIgnoreCase).ToList();
            var failures = 0;
            var totalBytes = 0L;
            var totalDocs = 0;

            foreach (var path in files)
            {
                string original;
                try
                {
                    original = File.ReadAllText(path);
                }
                catch (Exception e)
                {
                    output.WriteLine($"READ FAIL {path}: {e.Message}");
                    failures++;
                    continue;
                }

                try
                {
                    var file = UnityYaml.Parse(original);
                    var written = file.Write();
                    totalBytes += original.Length;
                    totalDocs += file.Documents.Count;
                    if (written != original)
                    {
                        failures++;
                        output.WriteLine($"DIFF {path}");
                        output.WriteLine(Diff(original, written));
                        continue;
                    }

                    // Again, forcing every node through the regeneration path rather than its
                    // captured text, which is what an edited document takes.
                    var forced = UnityYaml.Parse(original);
                    foreach (var doc in forced.Documents)
                        TouchAll(doc.Body);
                    var written2 = forced.Write();
                    if (written2 != original)
                    {
                        failures++;
                        output.WriteLine($"DIFF (regenerated) {path}");
                        output.WriteLine(Diff(original, written2));
                        continue;
                    }

                    if (verbose)
                        output.WriteLine($"ok   {path} ({file.Documents.Count} docs)");
                }
                catch (Exception e)
                {
                    failures++;
                    output.WriteLine($"PARSE FAIL {path}: {e.Message}");
                }
            }

            output.WriteLine($"{files.Count - failures}/{files.Count} files round-tripped ({totalDocs} documents, {totalBytes / 1024} KB)");
            return failures == 0 ? 0 : 1;
        }

        static void TouchAll(YNode node)
        {
            switch (node)
            {
                case YMap map:
                    map.Touch();
                    foreach (var e in map.Entries)
                        TouchAll(e.Value);
                    break;
            }
        }

        public static string Diff(string a, string b)
        {
            var la = a.Split('\n');
            var lb = b.Split('\n');
            var sb = new StringBuilder();
            var shown = 0;
            for (var i = 0; i < Math.Max(la.Length, lb.Length) && shown < 6; i++)
            {
                var x = i < la.Length ? la[i] : "<eof>";
                var y = i < lb.Length ? lb[i] : "<eof>";
                if (x == y)
                    continue;
                sb.AppendLine($"  line {i + 1}:");
                sb.AppendLine($"    - {Show(x)}");
                sb.AppendLine($"    + {Show(y)}");
                shown++;
            }
            if (shown == 0)
                sb.AppendLine($"  (identical line-wise; lengths {a.Length} vs {b.Length})");
            return sb.ToString().TrimEnd();
        }

        static string Show(string s) => s.Length > 140 ? s.Substring(0, 140) + "…" : s;
    }
}
