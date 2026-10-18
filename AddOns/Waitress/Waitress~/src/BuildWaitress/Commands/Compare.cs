using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BuildWaitress.Model;

namespace BuildWaitress.Commands
{
    public static class Compare
    {
        public static int Diff(Session session, Args a, TextWriter w)
        {
            var specs = a.Positional.ToList();
            var top = a.GetInt("top", 12);
            a.RejectUnknown();
            if (specs.Count > 2)
                throw new WaitressException("diff takes at most two builds: diff [old] [new] (default: prev last)");
            var oldSpec = specs.Count == 2 ? specs[0] : specs.Count == 1 ? specs[0] : "prev";
            var newSpec = specs.Count == 2 ? specs[1] : "last";
            var before = session.Open(oldSpec);
            var after = session.Open(newSpec);
            if (before.Report == null || after.Report == null)
                throw new WaitressException("diff compares two build reports; a folder without a report can't be compared");
            var ra = before.Report;
            var rb = after.Report;
            if (string.Equals(Path.GetFullPath(ra.Source), Path.GetFullPath(rb.Source), StringComparison.OrdinalIgnoreCase))
                throw new WaitressException($"'{oldSpec}' and '{newSpec}' are the same build");
            var fa = Analysis.Files(before);
            var fb = Analysis.Files(after);
            w.WriteLine($"{Label(oldSpec, ra)} → {Label(newSpec, rb)}: total {Fmt.Signed(fb.Sum(f => f.Size) - fa.Sum(f => f.Size))} ({Fmt.Bytes(fa.Sum(f => f.Size))} → {Fmt.Bytes(fb.Sum(f => f.Size))})");
            if (!string.Equals(ra.OutputDir, rb.OutputDir, StringComparison.OrdinalIgnoreCase))
                w.WriteLine($"note: different output folders ({ra.OutputPath} → {rb.OutputPath})");
            static int Scenes(Report r) => r.Files.Count(f => f.Role == "Scene" || System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(f.Rel), "^level\\d+$"));
            var scenesA = Scenes(ra);
            var scenesB = Scenes(rb);
            if (scenesA != scenesB)
                w.WriteLine($"note: {Fmt.Plural(scenesA, "scene")} → {Fmt.Plural(scenesB, "scene")}");
            if (ra.Platform != rb.Platform)
                w.WriteLine($"note: different platforms ({ra.Platform}, {rb.Platform})");
            if (ra.Options != rb.Options)
                w.WriteLine($"options: {ra.OptionText()} → {rb.OptionText()}");
            var cats = Analysis.Categories.Select(c => (c, fa.Where(f => Analysis.Category(f) == c).Sum(f => f.Size), fb.Where(f => Analysis.Category(f) == c).Sum(f => f.Size)))
                .Where(x => x.Item2 != 0 || x.Item3 != 0).ToList();
            w.WriteLine("by category: " + string.Join(", ", cats.Select(x => x.Item2 == x.Item3 ? $"{x.c} unchanged" : $"{x.c} {Fmt.Signed(x.Item3 - x.Item2)}")));
            // Files line up by path with the product name taken out, and print with the newer build's real path.
            var real = fb.GroupBy(f => Neutral(f.Rel, rb)).ToDictionary(g => g.Key, g => g.First().Rel);
            foreach (var f in fa)
                real.TryAdd(Neutral(f.Rel, ra), f.Rel);
            Changes(w, "files", fa.GroupBy(f => Neutral(f.Rel, ra)).ToDictionary(g => g.Key, g => g.Sum(f => f.Size)), fb.GroupBy(f => Neutral(f.Rel, rb)).ToDictionary(g => g.Key, g => g.Sum(f => f.Size)), top, real);
            var scriptsA = ra.Assets.Where(x => x.ClassId == 115).Select(x => x.Path).Distinct().Count();
            var scriptsB = rb.Assets.Where(x => x.ClassId == 115).Select(x => x.Path).Distinct().Count();
            if (scriptsA != scriptsB)
                w.WriteLine($"scripts (MonoScript entries): {scriptsA} → {scriptsB}");
            Changes(w, "assets", ra.Assets.Where(x => x.ClassId != 115).GroupBy(x => x.Path.Length > 0 ? x.Path : $"({ClassIds.Name(x.ClassId)} in {x.File})").ToDictionary(g => g.Key, g => g.Sum(x => x.Size)),
                rb.Assets.Where(x => x.ClassId != 115).GroupBy(x => x.Path.Length > 0 ? x.Path : $"({ClassIds.Name(x.ClassId)} in {x.File})").ToDictionary(g => g.Key, g => g.Sum(x => x.Size)), top);
            var sa = LoadSnapshot(session, before);
            var sb = LoadSnapshot(session, after);
            if (sa != null && sb != null)
                Changes(w, "subscene sections", sa, sb, top);
            else if ((sa != null) != (sb != null) || after.Subscenes.Count > 0)
                w.WriteLine("subscenes: no snapshot for " + (sa == null ? "the older" : "the newer") + " build (snapshots are taken when buildwaitress first sees a report while its folder is intact)");
            var ta = ra.Duration;
            var tb = rb.Duration;
            w.WriteLine($"build time {Fmt.Time(ta)} → {Fmt.Time(tb)}");
            return 0;
        }

        /// <summary>A file's path with the product name taken out, so builds of differently named players line up.</summary>
        static string Neutral(string rel, Report r)
        {
            var product = Path.GetFileNameWithoutExtension(r.OutputPath);
            if (product.Length == 0)
                return rel;
            if (rel.StartsWith(product + "_", StringComparison.OrdinalIgnoreCase))
                return "<product>" + rel.Substring(product.Length);
            if (rel.StartsWith(product + ".", StringComparison.OrdinalIgnoreCase))
                return "<product>" + rel.Substring(product.Length);
            return rel;
        }

        static string Label(string spec, Report r) => $"{spec} ({r.Started:MM-dd HH:mm}{(r.Result == "Succeeded" ? "" : ", " + r.Result)})";

        static void Changes(TextWriter w, string what, Dictionary<string, long> a, Dictionary<string, long> b, int top, Dictionary<string, string> display = null)
        {
            var rows = a.Keys.Union(b.Keys).Select(k => (k, old: a.TryGetValue(k, out var x) ? x : -1, now: b.TryGetValue(k, out var y) ? y : -1))
                .Where(r => r.old != r.now).OrderByDescending(r => Math.Abs(Math.Max(r.now, 0) - Math.Max(r.old, 0))).ToList();
            if (rows.Count == 0)
            {
                w.WriteLine($"{what}: unchanged");
                return;
            }
            var added = rows.Count(r => r.old < 0);
            var removed = rows.Count(r => r.now < 0);
            w.WriteLine($"{what}: {rows.Count} changed ({added} new, {removed} gone)");
            foreach (var r in rows.Take(top))
            {
                var change = r.old < 0 ? $"new {Fmt.Bytes(r.now)}" : r.now < 0 ? $"gone {Fmt.Bytes(r.old)}" : $"{Fmt.Signed(r.now - r.old)} → {Fmt.Bytes(r.now)}";
                w.WriteLine($"  {change,-22} {(display != null && display.TryGetValue(r.k, out var shown) ? shown : r.k)}");
            }
            if (rows.Count > top)
                w.WriteLine($"  … {rows.Count - top} more; --top N");
        }

        /// <summary>Per-section sizes saved next to an archived report, so subscenes stay comparable after the folder is rebuilt.</summary>
        public static void WriteSnapshot(string reportPath, Build b)
        {
            if (!b.OnDisk || b.Changed().Count > 0)
                return;
            var lines = new List<string>();
            foreach (var s in b.Subscenes)
                foreach (var (meta, data, _) in s.Sections)
                    lines.Add(string.Join("\t", $"{s.Name} section {meta.Index}", meta.FileSize.ToString(CultureInfo.InvariantCulture),
                        data == null ? "" : Analysis.Entities(data).ToString(CultureInfo.InvariantCulture), data?.Chunks.Count.ToString(CultureInfo.InvariantCulture) ?? ""));
            File.WriteAllLines(Path.ChangeExtension(reportPath, ".subscenes.tsv"), lines);
        }

        static Dictionary<string, long> LoadSnapshot(Session session, Build b)
        {
            var path = Path.ChangeExtension(b.Report.Source, ".subscenes.tsv");
            if (!File.Exists(path))
                return null;
            return File.ReadAllLines(path).Select(l => l.Split('\t')).Where(c => c.Length >= 2).ToDictionary(c => c[0], c => long.Parse(c[1], CultureInfo.InvariantCulture));
        }
    }
}
