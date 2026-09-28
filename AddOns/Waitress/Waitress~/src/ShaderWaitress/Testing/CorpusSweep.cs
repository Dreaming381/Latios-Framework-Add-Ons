using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using ShaderWaitress.Cli;
using ShaderWaitress.Layout;
using ShaderWaitress.Model;

namespace ShaderWaitress.Testing
{
    /// <summary>
    /// Runs the whole pipeline over every graph under the given roots without writing
    /// anything: load, render, lay out, and assert the layout invariants. This is the check
    /// that the tool copes with graphs real people made rather than only the ones it built.
    /// </summary>
    public static class CorpusSweep
    {
        public static int Run(Args args, TextWriter output)
        {
            var roots = args.Positional.Count > 0 ? args.Positional.ToList() : new List<string> { Directory.GetCurrentDirectory() };
            var verbose = args.Flag("verbose") || args.Flag("v");
            var limit = args.GetInt("limit", int.MaxValue);
            args.RejectUnknown();

            var files = new List<string>();
            foreach (var root in roots)
            {
                if (File.Exists(root))
                {
                    files.Add(root);
                    continue;
                }
                if (!Directory.Exists(root))
                    continue;
                foreach (var pattern in new[] { "*.shadergraph", "*.shadersubgraph" })
                    files.AddRange(Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories));
            }
            files.Sort(StringComparer.OrdinalIgnoreCase);
            if (files.Count > limit)
                files = files.Take(limit).ToList();

            var failures = 0;
            var legacy = 0;
            var improved = 0;
            var worsened = 0;
            var kept = 0;
            var totalNodes = 0;
            double beforeTotal = 0, afterTotal = 0;
            var stopwatch = Stopwatch.StartNew();

            foreach (var file in files)
            {
                try
                {
                    var doc = ShaderGraphDocument.Load(file);
                    totalNodes += doc.Nodes.Count;

                    // Rendering must never throw, whatever is in the file.
                    var dense = DenseText.Render(doc);
                    if (string.IsNullOrEmpty(dense))
                        throw new Exception("empty dense render");

                    var engine = new LayoutEngine(doc);
                    engine.Run();
                    var after = engine.After;
                    if (after == null)
                        continue;

                    beforeTotal += engine.Before.Score;
                    afterTotal += after.Score;
                    if (after.Score < engine.Before.Score)
                        improved++;
                    else if (after.Score > engine.Before.Score)
                        worsened++;

                    if (engine.Kept)
                    {
                        kept++;
                        if (verbose)
                            output.WriteLine($"keep {Path.GetFileName(file),-52} existing layout already scores {engine.Before.Score:0}");
                        continue;
                    }

                    // Invariants only bind on a layout the tool actually wrote. A backward wire
                    // is only a defect if the source graph did not already have one: a group
                    // whose members straddle a consumer makes one unavoidable.
                    var problems = new List<string>();
                    if (after.NodeOverlaps > 0)
                        problems.Add($"{after.NodeOverlaps} node overlap(s)");
                    // Mutually dependent groups make a backward wire unavoidable; the engine says so.
                    if (after.BackwardEdges > engine.Before.BackwardEdges && engine.Notes.Count == 0)
                        problems.Add($"{after.BackwardEdges} backward wire(s), was {engine.Before.BackwardEdges}");
                    if (after.GroupOverlaps > 0)
                        problems.Add($"{after.GroupOverlaps} group overlap(s)");
                    if (after.NodesOutsideGroup > 0)
                        problems.Add($"{after.NodesOutsideGroup} foreign node(s) inside a group");
                    // Trading crossings for the removal of an overlap or a backward wire is the
                    // intended behaviour, so a rise in crossings is only a defect when the
                    // layout came out worse overall as well.
                    var structurallyWorse =
                        after.Crossings > engine.Before.Crossings ||
                        after.ShallowCrossings > engine.Before.ShallowCrossings ||
                        after.CrossingClusters > engine.Before.CrossingClusters ||
                        after.WiresOverNodes > engine.Before.WiresOverNodes;
                    if (after.Score > engine.Before.Score && structurallyWorse)
                        problems.Add($"scored worse overall ({engine.Before.Score:0} -> {after.Score:0}) and reads worse");

                    if (problems.Count > 0)
                    {
                        failures++;
                        output.WriteLine($"FAIL {Path.GetFileName(file)}: {string.Join(", ", problems)}");
                        output.WriteLine($"     before: {engine.Before.Describe()}");
                        output.WriteLine($"     after:  {after.Describe()}");
                    }
                    else if (verbose)
                    {
                        output.WriteLine($"ok   {Path.GetFileName(file),-52} {engine.Before.Score,10:0} -> {after.Score,8:0}");
                    }
                }
                catch (ShaderWaitressException e) when (e.LegacyGraphFormat)
                {
                    legacy++;
                    if (verbose)
                        output.WriteLine($"skip (pre-10.0 format): {file}");
                }
                catch (Exception e)
                {
                    failures++;
                    output.WriteLine($"FAIL {file}: {e.Message}");
                }
            }

            stopwatch.Stop();
            output.WriteLine($"sweep: {files.Count} graphs, {totalNodes} nodes, {failures} failure(s)" +
                             (legacy == 0 ? string.Empty : $", {legacy} skipped in the pre-10.0 format") +
                             $" in {stopwatch.ElapsedMilliseconds} ms");
            output.WriteLine($"score: {beforeTotal.ToString("0", CultureInfo.InvariantCulture)} -> {afterTotal.ToString("0", CultureInfo.InvariantCulture)}" +
                             $"  ({improved} improved, {worsened} worsened, {kept} kept as-is)");
            return failures == 0 ? 0 : 1;
        }
    }
}
