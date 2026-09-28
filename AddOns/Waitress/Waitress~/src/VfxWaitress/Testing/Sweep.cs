using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxWaitress.Model;

namespace VfxWaitress.Testing
{
    /// <summary>
    /// Runs the read path over every VFX asset under the given roots without writing anything,
    /// and reports what the tool could not name. Unknown scripts are the signal that the catalog
    /// is stale or that the corpus uses a package the catalog was not exported against.
    /// </summary>
    public static class Sweep
    {
        public static int Run(IReadOnlyList<string> roots, TextWriter output, bool verbose)
        {
            var files = RoundTrip.Find(roots).OrderBy(o => o, StringComparer.OrdinalIgnoreCase).ToList();
            var failures = 0;
            var rawBytes = 0L;
            var denseBytes = 0L;
            var unknownScripts = new Dictionary<string, int>(StringComparer.Ordinal);
            var nodeCount = 0;

            foreach (var path in files)
            {
                try
                {
                    var asset = VfxAsset.Load(path);

                    // Two entities sharing a short id would make commands silently hit the wrong one.
                    var byShortId = new Dictionary<string, VfxNode>(StringComparer.OrdinalIgnoreCase);
                    foreach (var node in asset.AllNodes())
                    {
                        if (byShortId.TryGetValue(node.ShortId, out var other))
                        {
                            output.WriteLine($"DUPLICATE ID {path}: {node.ShortId} is both file id {other.FileId} and {node.FileId}");
                            failures++;
                        }
                        else
                        {
                            byShortId[node.ShortId] = node;
                        }
                    }

                    var dense = DenseText.Render(asset);
                    rawBytes += new FileInfo(path).Length;
                    denseBytes += dense.Length;
                    nodeCount += asset.AllNodes().Count();
                    foreach (var w in asset.Warnings)
                    {
                        if (!w.StartsWith("unknown node script ", StringComparison.Ordinal))
                            continue;
                        var guid = w.Substring("unknown node script ".Length).Split(' ')[0];
                        unknownScripts.TryGetValue(guid, out var n);
                        unknownScripts[guid] = n + 1;
                    }
                    if (verbose)
                        output.WriteLine($"ok   {path} ({asset.AllNodes().Count()} nodes, {dense.Length} B)");
                }
                catch (Exception e)
                {
                    failures++;
                    output.WriteLine($"FAIL {path}: {e.Message}");
                }
            }

            if (unknownScripts.Count > 0)
            {
                output.WriteLine($"{unknownScripts.Count} script guid(s) not in the catalog:");
                foreach (var kvp in unknownScripts.OrderByDescending(k => k.Value))
                    output.WriteLine($"  {kvp.Key}  {kvp.Value} node(s)");
            }

            var ratio = denseBytes == 0 ? 0 : rawBytes / denseBytes;
            output.WriteLine($"{files.Count - failures}/{files.Count} graphs rendered, {nodeCount} nodes, {rawBytes / 1024} KB raw -> {denseBytes / 1024} KB dense ({ratio}x)");
            return failures == 0 && unknownScripts.Count == 0 ? 0 : 1;
        }
    }
}
