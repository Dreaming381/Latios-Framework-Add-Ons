using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Testing
{
    /// <summary>
    /// Parses and re-serializes a corpus of graphs and requires byte-identical output. This
    /// is the fidelity guarantee the rest of the tool is built on.
    /// </summary>
    public static class RoundTripCheck
    {
        public static int Run(IReadOnlyList<string> roots, TextWriter output, bool verbose)
        {
            var files = new List<string>();
            foreach (var root in roots)
            {
                if (File.Exists(root))
                {
                    files.Add(root);
                    continue;
                }
                if (!Directory.Exists(root))
                {
                    output.WriteLine($"skip (not found): {root}");
                    continue;
                }
                foreach (var pattern in new[] { "*.shadergraph", "*.shadersubgraph" })
                    files.AddRange(Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories));
            }
            files.Sort(StringComparer.OrdinalIgnoreCase);

            var failures = 0;
            var legacy = 0;
            var nonCanonical = 0;
            var totalObjects = 0;
            foreach (var file in files)
            {
                string original;
                try
                {
                    original = File.ReadAllText(file);
                }
                catch (Exception e)
                {
                    output.WriteLine($"FAIL read  {file}: {e.Message}");
                    failures++;
                    continue;
                }

                try
                {
                    var doc = MultiJsonDocument.Parse(original);
                    totalObjects += doc.Entries.Count;
                    var rewritten = doc.Serialize();
                    if (!string.Equals(original, rewritten, StringComparison.Ordinal))
                    {
                        failures++;
                        output.WriteLine($"FAIL diff  {file}");
                        output.WriteLine(Describe(original, rewritten));
                    }
                    else if (verbose)
                    {
                        output.WriteLine($"ok    {doc.Entries.Count,5} objects  {file}");
                    }

                    // Force every object through the parse/write path too, which is what an
                    // edited object takes. A mismatch here does not break the guarantee above —
                    // untouched objects are still emitted verbatim — it means the file is not
                    // in the formatting Unity itself writes, which a hand-edited file often is
                    // not. The tool would normalise those objects if it ever edited them.
                    var forced = ForceRewrite(original);
                    if (!string.Equals(original, forced, StringComparison.Ordinal))
                    {
                        nonCanonical++;
                        output.WriteLine($"not in Unity's formatting: {file}");
                        if (verbose)
                            output.WriteLine(Describe(original, forced));
                    }
                }
                catch (ShaderWaitressException e) when (e.LegacyGraphFormat)
                {
                    legacy++;
                    output.WriteLine($"skip (pre-10.0 format): {file}");
                }
                catch (Exception e)
                {
                    failures++;
                    output.WriteLine($"FAIL parse {file}: {e.Message}");
                }
            }

            output.WriteLine($"round trip: {files.Count} files, {totalObjects} objects, {failures} failures" +
                             (legacy == 0 ? string.Empty : $", {legacy} in the pre-10.0 format") +
                             (nonCanonical == 0 ? string.Empty : $", {nonCanonical} not in Unity's formatting"));
            return failures == 0 ? 0 : 1;
        }

        static string ForceRewrite(string original)
        {
            var doc = MultiJsonDocument.Parse(original);
            foreach (var entry in doc.Entries)
                entry.Edit();
            return doc.Serialize();
        }

        static string Describe(string a, string b)
        {
            var i = 0;
            var max = Math.Min(a.Length, b.Length);
            while (i < max && a[i] == b[i])
                i++;
            var line = 1;
            for (var k = 0; k < i; k++)
            {
                if (a[k] == '\n')
                    line++;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"       first difference at offset {i} (line {line})");
            sb.AppendLine($"       expected: {Snippet(a, i)}");
            sb.Append($"       actual:   {Snippet(b, i)}");
            return sb.ToString();
        }

        static string Snippet(string s, int at)
        {
            var start = Math.Max(0, at - 40);
            var end = Math.Min(s.Length, at + 60);
            return s.Substring(start, end - start).Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}
