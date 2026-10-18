using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BurstWaitress.Asm;
using BurstWaitress.Model;

namespace BurstWaitress.Commands
{
    public static class Commands
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // -----------------------------------------------------------------------------------------------------------
        // status

        public static int Status(Args args, TextWriter w)
        {
            var json = args.Flag("json");
            var quick = args.Flag("quick");
            TimeoutArg(args);
            args.RejectUnknown();
            var index = TargetIndex.Load();
            var names = new List<string>();
            var notes = new List<string>();
            foreach (var spec in args.Positional)
            {
                var found = index.Resolve(spec);
                var member = MemberOfEntry(index, spec);
                if (member.entry != null)
                {
                    // A job's Execute is Burst code exactly when the job is.
                    names.Add(member.entry.Name);
                    notes.Add($"{spec}: reported as its job, {TargetIndex.ShortNames(new[] { member.entry.Name })[member.entry.Name]}, which Burst compiles with it");
                }
                else if (found.Count == 0)
                    names.Add(spec);
                else
                {
                    if (found.Count > 60)
                        notes.Add($"{spec}: {found.Count} entry points, the first 60 checked");
                    names.AddRange(found.Take(60).Select(e => e.Name));
                }
            }
            var file = Path.Combine(Project.Dir, "Requests", "status-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".txt");
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllLines(file, names.Distinct());
            var outDir = Path.Combine(Project.Dir, "StatusDumps");
            var r = EditorScript.Call(Project.Root, "Status", file, outDir, 20000, quick);
            // Whole-assembly checks run in the Editor's background; wait for their files, then ask again.
            var pending = r.GetProperty("items").EnumerateArray().Where(i => i.TryGetProperty("assemblyCheckFile", out _)).Select(i => i.GetProperty("assemblyCheckFile").GetString()).Distinct().ToList();
            if (pending.Count > 0)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var progress = r.GetProperty("items").EnumerateArray().Where(i => i.TryGetProperty("assemblyCheck", out _)).Select(i => i.GetProperty("assemblyCheck").GetString()).Distinct();
                Console.Error.WriteLine("status: " + string.Join("; ", progress) + " — once per build of each assembly; --quick skips it");
                var lastNote = 0.0;
                while (pending.Any(p => !File.Exists(p)))
                {
                    if (sw.Elapsed.TotalSeconds - lastNote > 20)
                    {
                        lastNote = sw.Elapsed.TotalSeconds;
                        var counts = pending.Select(p => p.Substring(0, p.Length - ".done".Length)).Select(p => File.Exists(p) ? File.ReadAllLines(p).Length : 0);
                        Console.Error.WriteLine($"status: {(int)sw.Elapsed.TotalSeconds} s, {string.Join(", ", counts)} entry points checked");
                    }
                    if (sw.Elapsed.TotalSeconds > DumpStore.TimeoutSeconds)
                        throw new WaitressException($"the whole-assembly compile checks are still running after {DumpStore.TimeoutSeconds} s (they run once per build of each assembly); re-run status to continue, or raise --timeout");
                    System.Threading.Thread.Sleep(250);
                }
                r = EditorScript.Call(Project.Root, "Status", file, outDir, 20000, quick);
            }
            File.Delete(file);
            if (json)
            {
                w.WriteLine(r.GetRawText());
                return 0;
            }
            WriteState(r, w);
            if (quick)
                w.WriteLine("--quick: the other entry points of each assembly weren't compiled; one that fails makes its whole assembly run managed");
            if (index.Stale)
                w.WriteLine("note: " + index.Note);
            foreach (var n in notes)
                w.WriteLine("note: " + n);
            if (names.Count == 0)
                return 0;
            var shortNames = TargetIndex.ShortNames(names);
            var bad = 0;
            foreach (var item in r.GetProperty("items").EnumerateArray())
            {
                var name = item.TryGetProperty("resolved", out var resolved) ? resolved.GetString() : item.GetProperty("name").GetString();
                var state = item.GetProperty("state").GetString();
                var reason = item.TryGetProperty("reason", out var rs) ? rs.GetString() : null;
                var extra = "";
                if (item.TryGetProperty("directCall", out var dc))
                    extra = dc.GetString() switch
                    {
                        "set" => " (called in this session: direct-call pointer set)",
                        "unrequested" => " (it compiles; nothing called it this session, so no native code is loaded yet, and the first call requests it)",
                        _ => "",
                    };
                if (state != "compiled")
                    bad++;
                w.WriteLine($"{state,-13} {shortNames.GetValueOrDefault(name, TargetIndex.ShortNames(new[] { name })[name])}{extra}");
                if (reason != null)
                    foreach (var line in reason.Split('\n').Take(4))
                        w.WriteLine("              " + line);
                if (item.TryGetProperty("assemblyOk", out var aok))
                    w.WriteLine("              " + aok.GetString());
                if (item.TryGetProperty("key", out var key) && state != "failed")
                {
                    var note = Fingerprint(key.GetString(), name);
                    if (note != null)
                        w.WriteLine("              " + note);
                }
            }
            return bad == 0 ? 0 : 1;
        }

        static void WriteState(JsonElement r, TextWriter w)
        {
            bool B(string k) => r.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.True;
            string S(string k) => r.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : "";
            var enabled = B("enabled");
            var why = enabled ? "" : B("forceDisabled") ? " (forced off by command line or environment)" : !B("menuEnabled") ? " (Jobs > Burst > Enable Compilation is off)" : "";
            w.WriteLine($"Burst {S("burstVersion")}: {(enabled ? "enabled" : "DISABLED" + why)} · safety checks {(B("forceSafetyChecks") ? "forced on" : B("safetyChecks") ? "on" : "off")} · synchronous compilation {(B("synchronous") ? "ON" : "off")} · native debug {(B("nativeDebug") ? "on" : "off")} · host {S("host")}");
            var eager = S("eager");
            var progress = S("progress");
            var idle = B("jitIdle") && progress.Length == 0;
            w.WriteLine($"background compilation: {(idle ? "idle" : "running" + (progress.Length > 0 ? " (" + progress + ")" : ""))}{(B("compiling") ? " · scripts compiling" : "")}{(B("playing") ? " · play mode" : "")}  (settings are read, never changed)");
            if (r.TryGetProperty("disabledAssemblies", out var da) && da.GetArrayLength() > 0)
                w.WriteLine("Burst off for assemblies: " + string.Join(", ", da.EnumerateArray().Select(x => x.GetString())));
            var bench = new List<string>();
            if (!enabled)
                bench.Add("Burst is off, so they time managed code");
            if (B("safetyChecks") || B("forceSafetyChecks"))
                bench.Add("safety checks are on, so they include the checks");
            if (!idle)
                bench.Add("compilation is still running, so early samples run managed");
            if (B("nativeDebug"))
                bench.Add("native debug mode disables optimizations");
            w.WriteLine(bench.Count == 0 ? "benchmarks here would time optimized Burst code" : "benchmarks here: " + string.Join("; ", bench));
        }

        /// <summary>
        /// Burst names an entry point's compiled module by a hash of the entry's own IL and options, not its callees'. If
        /// the Editor's JIT cache were keyed the same way, a callee-only change could leave old native code running (not
        /// reproduced on 1.8.30). The tool remembers each entry hash with a digest of its code, and reports when the code
        /// changed under the same hash.
        /// </summary>
        static string Fingerprint(string key, string name)
        {
            var path = Path.Combine(Project.Dir, "StatusDumps", key + ".asm");
            if (!File.Exists(path))
                return null;
            var text = File.ReadAllText(path);
            var hashes = Regex.Matches(text, "^([0-9a-f]{32}):", RegexOptions.Multiline).Select(m => m.Groups[1].Value).Distinct().ToList();
            if (hashes.Count == 0)
                return null;
            var normalized = text;
            foreach (var h in hashes)
                normalized = normalized.Replace(h, "H");
            var digest = Project.Hash(normalized);
            var entryHash = hashes[0];
            var history = Path.Combine(Project.Dir, "fingerprints.tsv");
            var records = File.Exists(history) ? File.ReadAllLines(history).Select(l => l.Split('\t')).Where(p => p.Length >= 4).ToList() : new List<string[]>();
            var last = records.LastOrDefault(p => p[0] == entryHash);
            string note = null;
            if (last != null && last[1] != digest)
            {
                var when = new DateTime(long.Parse(last[3], Inv), DateTimeKind.Utc).ToLocalTime();
                note = $"note: its compiled code differs from {when:yyyy-MM-dd HH:mm} under the same Burst hash ({entryHash.Substring(0, 8)}), which covers only the entry's own IL " +
                       "and options; a callee or an edit-and-revert changes code without it. Not seen to leave old code running on Burst 1.8.30; if it behaves like old code, " +
                       "make an IL change in the entry method itself";
            }
            if (last == null || last[1] != digest)
                File.AppendAllText(history, $"{entryHash}\t{digest}\t{name}\t{DateTime.UtcNow.Ticks}\n");
            return note;
        }

        // -----------------------------------------------------------------------------------------------------------
        // dump

        public static int DumpCmd(Args args, TextWriter w)
        {
            var spec = args.Positional0("an entry point (a [BurstCompile] static method, job or ISystem method)");
            var raw = args.Flag("raw");
            var source = args.Flag("source");
            var json = args.Flag("json");
            var all = args.Flag("all");
            var fn = args.Get("fn");
            var selector = raw && args.Positional.Count > 1 ? args.Positional[1] : null;
            if (selector != null && selector.StartsWith("src:", StringComparison.OrdinalIgnoreCase))
                source = true;
            var sets = CompileOptions.Sets(args, source);
            TimeoutArg(args);
            args.RejectExtraPositionals(raw ? 2 : 1);
            args.RejectUnknown();
            if (sets.Count != 1)
                throw new WaitressException("dump takes one value per option; use diff for two and sweep for more");
            if (json && (raw || source))
                throw new WaitressException("--json is the summary's data; --raw and --source are text, so use them without --json");
            var index = TargetIndex.Load();
            var member = MemberOfEntry(index, spec);
            Entry entry;
            if (member.entry != null && fn == null)
            {
                // Job.Execute and similar: the method lives inside an entry point's dump as its own function.
                entry = member.entry;
                fn = member.member;
            }
            else
                entry = index.One(spec);
            if (!entry.HasBurst)
                throw new WaitressException($"{entry.Name} has no [BurstCompile], so Burst doesn't compile it (status says the same); add the attribute to dump it");
            var d = DumpStore.Get(new[] { (entry, sets[0]) }, w)[0];
            if (fn != null && d.Error == null)
            {
                if (member.entry != null && !Analyzer.Reached(d.Asm).Any(f => f.Display.Contains(fn, StringComparison.OrdinalIgnoreCase)))
                {
                    w.WriteLine($"note: {fn} was inlined into the job's code, so this is the whole job");
                    fn = null;
                }
                else
                    Report.Pick(d.Asm, fn);
            }
            if (json)
            {
                w.WriteLine(JsonSerializer.Serialize(Report.Json(d, fn), Report.JsonOptions));
                return d.Error == null ? 0 : 1;
            }
            if (raw)
                return Report.Raw(d, selector ?? (fn != null ? "fn:" + fn : null), w);
            if (source)
                return Report.Source(d, w, all, fn);
            return Report.Summary(d, w, all, fn);
        }

        static void TimeoutArg(Args args)
        {
            var t = args.GetInt("timeout", DumpStore.TimeoutSeconds);
            if (t <= 0)
                throw new WaitressException("--timeout takes seconds, more than 0");
            DumpStore.TimeoutSeconds = t;
        }

        /// <summary>"Job.Execute": a name that isn't an entry point, but whose prefix is exactly one.</summary>
        static (Entry entry, string member) MemberOfEntry(TargetIndex index, string spec)
        {
            var dot = spec.LastIndexOf('.');
            if (dot <= 0 || spec.Contains('*') || index.Resolve(spec).Count != 0)
                return (null, null);
            var owner = index.Resolve(spec.Substring(0, dot));
            return owner.Count == 1 && !owner[0].IsStatic ? (owner[0], spec.Substring(dot + 1)) : (null, null);
        }

        // -----------------------------------------------------------------------------------------------------------
        // diff

        public static int Diff(Args args, TextWriter w)
        {
            var json = args.Flag("json");
            TimeoutArg(args);
            var sets = CompileOptions.Sets(args);
            args.RejectExtraPositionals(2);
            args.RejectUnknown();
            var index = TargetIndex.Load();
            Entry ea, eb;
            CompileOptions oa, ob;
            if (args.Positional.Count == 2)
            {
                ea = index.One(args.Positional[0]);
                eb = index.One(args.Positional[1]);
                if (sets.Count == 1)
                {
                    oa = sets[0];
                    ob = sets[0];
                }
                else if (sets.Count == 2)
                {
                    oa = sets[0];
                    ob = sets[1];
                }
                else
                    throw new WaitressException("with two methods, give each option one value (both use it) or one option two values (A gets the first)");
            }
            else if (args.Positional.Count == 1)
            {
                ea = eb = index.One(args.Positional[0]);
                if (sets.Count != 2)
                    throw new WaitressException("diff of one method needs one option with two values, e.g. --float-mode Default,Strict or --target avx2,sse4");
                oa = sets[0];
                ob = sets[1];
            }
            else
                throw new WaitressException("diff takes two methods, or one method and an option with two values (--target avx2,sse4)");
            var dumps = DumpStore.Get(new[] { (ea, oa), (eb, ob) }, w);
            var a = dumps[0];
            var b = dumps[1];
            if (json)
            {
                object delta = null;
                object loops = null;
                if (a.Error == null && b.Error == null)
                {
                    var ja = Report.Totals(Report.Counted(a.Asm));
                    var jb = Report.Totals(Report.Counted(b.Asm));
                    delta = new
                    {
                        instrs = jb.Instrs - ja.Instrs, packedFloat = jb.PackedFloat - ja.PackedFloat, scalarFloat = jb.ScalarFloat - ja.ScalarFloat,
                        packedInt = jb.PackedInt - ja.PackedInt, scalarInt = jb.ScalarInt - ja.ScalarInt, loads = jb.Loads - ja.Loads, stores = jb.Stores - ja.Stores,
                        branches = jb.Branches - ja.Branches, calls = jb.Calls - ja.Calls, fma = jb.Fma - ja.Fma, spills = jb.Spills - ja.Spills, reloads = jb.Reloads - ja.Reloads,
                    };
                    loops = MatchLoops(a.Asm, b.Asm).Select(p => new { a = p.a?.Id, b = p.b?.Id, same = p.a != null && p.b != null && Report.LoopLine(a.Asm.Isa, p.a) == Report.LoopLine(b.Asm.Isa, p.b) }).ToList();
                }
                w.WriteLine(JsonSerializer.Serialize(new { a = Report.Json(a), b = Report.Json(b), delta, loops, identical = a.Text == b.Text }, Report.JsonOptions));
                return a.Error == null && b.Error == null ? 0 : 1;
            }
            var names = TargetIndex.ShortNames(new[] { ea.Name, eb.Name });
            w.WriteLine($"A: {names[ea.Name]} · {oa.Describe(ea)}");
            w.WriteLine($"B: {names[eb.Name]} · {ob.Describe(eb)}");
            if (a.Error != null || b.Error != null)
            {
                if (a.Error != null)
                    w.WriteLine("A failed: " + a.Error.Split('\n')[0]);
                if (b.Error != null)
                    w.WriteLine("B failed: " + b.Error.Split('\n')[0]);
                return 1;
            }
            var ta = Report.Totals(Report.Counted(a.Asm));
            var tb = Report.Totals(Report.Counted(b.Asm));
            w.WriteLine($"{"",-14}{"A",9}{"B",9}{"Δ",9}");
            void Row(string label, int x, int y)
            {
                if (x == 0 && y == 0)
                    return;
                var delta = y - x;
                w.WriteLine($"{label,-14}{x,9}{y,9}{(delta == 0 ? "" : delta.ToString("+0;-0", Inv)),9}");
            }
            Row("instrs", ta.Instrs, tb.Instrs);
            Row("packed float", ta.PackedFloat, tb.PackedFloat);
            Row("scalar float", ta.ScalarFloat, tb.ScalarFloat);
            Row("packed int", ta.PackedInt, tb.PackedInt);
            Row("scalar int", ta.ScalarInt, tb.ScalarInt);
            Row("loads", ta.Loads, tb.Loads);
            Row("stores", ta.Stores, tb.Stores);
            Row("branches", ta.Branches, tb.Branches);
            Row("calls", ta.Calls, tb.Calls);
            Row("fma", ta.Fma, tb.Fma);
            Row("spills", ta.Spills, tb.Spills);
            Row("reloads", ta.Reloads, tb.Reloads);
            Row("functions", a.Asm.Focus.Count, b.Asm.Focus.Count);
            Row("loops", a.Asm.Focus.Sum(f => f.Loops.Count), b.Asm.Focus.Sum(f => f.Loops.Count));
            var wa = a.Asm.Focus.SelectMany(x => x.Instrs).Select(x => x.Width).DefaultIfEmpty(0).Max();
            var wb = b.Asm.Focus.SelectMany(x => x.Instrs).Select(x => x.Width).DefaultIfEmpty(0).Max();
            if (wa != wb || a.Asm.Focus.Any(f => f.Stats.Scalable) != b.Asm.Focus.Any(f => f.Stats.Scalable))
                w.WriteLine($"widest register: {Report.WidthName(a.Asm.Isa, wa, 0, a.Asm.Focus.Any(f => f.Stats.Scalable))} → {Report.WidthName(b.Asm.Isa, wb, 0, b.Asm.Focus.Any(f => f.Stats.Scalable))}");
            var shownLoops = 0;
            foreach (var (la, lb) in MatchLoops(a.Asm, b.Asm))
            {
                if (++shownLoops > 12)
                {
                    w.WriteLine("... more loops (diff --json, or dump each side)");
                    break;
                }
                if (la == null)
                    w.WriteLine($"loop B#{lb.Id} (only in B): {Report.LoopLine(b.Asm.Isa, lb)}");
                else if (lb == null)
                    w.WriteLine($"loop A#{la.Id} (only in A): {Report.LoopLine(a.Asm.Isa, la)}");
                else
                {
                    var sa = Report.LoopLine(a.Asm.Isa, la);
                    var sb = Report.LoopLine(b.Asm.Isa, lb);
                    var tag = la.Main || lb.Main ? " main" : "";
                    if (sa == sb)
                        w.WriteLine($"loop #{la.Id}{tag} same: {sa}");
                    else
                    {
                        w.WriteLine($"loop A#{la.Id}{tag}: {sa}");
                        w.WriteLine($"     B#{lb.Id}{tag}: {sb}");
                    }
                }
            }
            var callsA = a.Asm.Calls.GroupBy(c => CallKey(c.Display)).ToDictionary(g => g.Key, g => (call: g.First(), count: g.Sum(c => c.Count)));
            var callsB = b.Asm.Calls.GroupBy(c => CallKey(c.Display)).ToDictionary(g => g.Key, g => (call: g.First(), count: g.Sum(c => c.Count)));
            foreach (var k in callsB.Keys.Where(k => !callsA.ContainsKey(k)))
                w.WriteLine($"call added: {callsB[k].count}× {Report.Shorten(callsB[k].call.Display, 90)} [{callsB[k].call.Kind}]");
            foreach (var k in callsA.Keys.Where(k => !callsB.ContainsKey(k)))
                w.WriteLine($"call removed: {callsA[k].count}× {Report.Shorten(callsA[k].call.Display, 90)} [{callsA[k].call.Kind}]");
            foreach (var k in callsA.Keys.Where(k => callsB.ContainsKey(k) && callsA[k].count != callsB[k].count))
                w.WriteLine($"call sites: {Report.Shorten(callsB[k].call.Display, 90)} {callsA[k].count} → {callsB[k].count}");
            if (a.Text == b.Text)
                w.WriteLine("the two dumps are identical");
            return 0;
        }

        static readonly Regex s_IsaSuffix = new Regex(@"(_(x64_|x86_)?(sse2|sse4|avx2|avx|avx512|advsimd|neon|armv7a|armv8a|armv9a|aarch64|wasm32|wasm64|thumb2))+$|(?<=u\d+)(sse2|sse4|avx2|avx|advsimd|sve)", RegexOptions.Compiled);

        /// <summary>A math routine's name without the target Burst built it for: Sleef_sinf_u35_avx2 and
        /// Sleef_sinf_u35_x64_sse4 are the same call.</summary>
        static string CallKey(string display) => s_IsaSuffix.Replace(display, "");

        /// <summary>Loops paired by role: the main loops, then within each function vector loops by width and scalar loops,
        /// each in layout order.</summary>
        static IEnumerable<(Loop a, Loop b)> MatchLoops(AsmFile fa, AsmFile fb)
        {
            var la = fa.Focus.SelectMany(f => f.Loops).ToList();
            var lb = fb.Focus.SelectMany(f => f.Loops).ToList();
            if (fa.Main != null && fb.Main != null)
            {
                yield return (fa.Main, fb.Main);
                la.Remove(fa.Main);
                lb.Remove(fb.Main);
            }
            string Role(Loop l) => Names.Short(l.Owner.Display) + "|" + (l.Stats.PackedFloat + l.Stats.VectorMathCalls > 0 ? "f" : l.Stats.PackedInt > 0 ? "i" : "s") + "|" + l.Depth;
            foreach (var role in la.Select(Role).Concat(lb.Select(Role)).Distinct().ToList())
            {
                var xa = la.Where(l => Role(l) == role).OrderByDescending(l => l.Stats.PackedWidth).ThenBy(l => l.Stats.FirstLine).ToList();
                var xb = lb.Where(l => Role(l) == role).OrderByDescending(l => l.Stats.PackedWidth).ThenBy(l => l.Stats.FirstLine).ToList();
                for (var i = 0; i < Math.Max(xa.Count, xb.Count); i++)
                    yield return (i < xa.Count ? xa[i] : null, i < xb.Count ? xb[i] : null);
            }
        }

        // -----------------------------------------------------------------------------------------------------------
        // sweep

        public static int Sweep(Args args, TextWriter w)
        {
            var json = args.Flag("json");
            TimeoutArg(args);
            var sets = CompileOptions.Sets(args);
            args.RejectUnknown();
            if (args.Positional.Count == 0)
                throw new WaitressException("sweep takes entry points (names or globs such as \"BwKernels.*\")");
            var index = TargetIndex.Load();
            var entries = new List<Entry>();
            foreach (var spec in args.Positional)
            {
                var found = index.Resolve(spec);
                if (found.Count == 0)
                    throw new WaitressException(index.NotFound(spec));
                entries.AddRange(found.Where(e => !entries.Contains(e)));
            }
            var notBurst = entries.Where(e => !e.HasBurst).ToList();
            entries.RemoveAll(e => !e.HasBurst);
            if (entries.Count * sets.Count > 600)
                throw new WaitressException($"{entries.Count} entry points × {sets.Count} option sets is {entries.Count * sets.Count} dumps; narrow it to 600 or fewer");
            if (entries.Count == 0)
                throw new WaitressException($"none of the {notBurst.Count} matching entry points has [BurstCompile]");
            if (notBurst.Count > 0 && !json)
                w.WriteLine($"skipping {notBurst.Count} without [BurstCompile]: " + string.Join(", ", notBurst.Take(5).Select(e => TargetIndex.ShortNames(notBurst.Select(x => x.Name))[e.Name])) +
                            (notBurst.Count > 5 ? ", …" : ""));
            var wanted = entries.SelectMany(e => sets.Select(o => (e, o))).ToList();
            var dumps = DumpStore.Get(wanted, w);
            if (json)
            {
                w.WriteLine(JsonSerializer.Serialize(dumps.Select(d => Report.Json(d)), Report.JsonOptions));
                return 0;
            }
            var names = TargetIndex.ShortNames(entries.Select(e => e.Name));
            var nameWidth = Math.Min(60, Math.Max(6, names.Values.Max(n => n.Length)));
            var showTarget = sets.Select(s => s.Target).Distinct().Count() > 1 || sets[0].Target != "AVX2";
            var showSafety = sets.Select(s => s.Safety).Distinct().Count() > 1;
            var sb = new StringBuilder();
            sb.Append("method".PadRight(nameWidth)).Append(showTarget ? " target  " : " ").Append("float     ").Append(showSafety ? "safety " : "")
              .Append("main loop          /iter  packed alias n≥    loops calls      spills    fma  instrs");
            w.WriteLine(sb.ToString());
            if (!showTarget)
                w.WriteLine($"(target {sets[0].Target}, safety {sets[0].Safety})");
            foreach (var d in dumps)
            {
                var line = new StringBuilder();
                line.Append(ShortenLeft(names[d.Entry.Name], nameWidth).PadRight(nameWidth)).Append(' ');
                if (showTarget)
                    line.Append(Short(d.Options.Target).PadRight(8));
                var fm = d.Options.EffectiveFloatMode(d.Entry) + (d.Options.FloatPrecision != null && d.Options.FloatPrecision != "Standard" ? "/" + d.Options.FloatPrecision[0] : "");
                line.Append(fm.PadRight(10));
                if (showSafety)
                    line.Append(d.Options.Safety.PadRight(7));
                if (d.Error != null)
                {
                    line.Append("FAILED: ").Append(Report.Shorten(d.Error.Split('\n')[0], 90));
                    w.WriteLine(line.ToString());
                    continue;
                }
                var a = d.Asm;
                var t = Report.Totals(Report.Counted(a));
                var main = a.Main;
                if (main == null)
                    line.Append("-".PadRight(19)).Append("-".PadRight(7)).Append("-".PadRight(7)).Append("-".PadRight(6)).Append("-".PadRight(6));
                else
                {
                    var s = main.Stats;
                    var width = s.PackedWidth > 0 ? Report.ShapeWidth(a.Isa, main, true) : "scalar";
                    var packed = s.PackedRatio >= 0 ? Report.Pct(s.PackedRatio) : s.PackedInt > 0 ? "int" : "-";
                    line.Append(width.PadRight(18)).Append(' ').Append(Report.PerIter(main).PadRight(6)).Append(' ').Append(packed.PadRight(6)).Append(' ')
                        .Append((main.AliasGuard ? "yes" : "-").PadRight(5)).Append(' ').Append((Report.MinCount(main) ?? "-").PadRight(5)).Append(' ');
                }
                var scalarMath = a.Calls.Where(c => c.Kind == "scalar math").Sum(c => c.OutsideRemainder);
                var calls = a.Calls.Sum(c => c.Count);
                line.Append(a.Focus.Sum(f => f.Loops.Count).ToString(Inv).PadRight(5)).Append(' ').Append((calls + (scalarMath > 0 ? $" ({scalarMath}sm)" : "")).PadRight(10)).Append(' ')
                    .Append($"{t.Spills}/{t.Reloads}".PadRight(9)).Append(' ').Append(t.Fma.ToString(Inv).PadRight(4)).Append(' ').Append(t.Instrs);
                w.WriteLine(line.ToString());
            }
            w.WriteLine("main loop = the innermost loop in the method's own functions (the job's Execute, not the producer's dispatch) vectorized across iterations, else the widest");
            w.WriteLine("/iter = source iterations one pass covers (B: a pointer's bytes); SLP = packed within one iteration; ? = packed, span unknown; n≥ = smallest guarded count it runs at");
            if (dumps.Any(d => d.Error == null && d.Asm.Calls.Any(c => c.Kind == "scalar math" && c.OutsideRemainder > 0)))
                w.WriteLine("calls (N sm) = N scalar math library calls outside remainder loops, the usual sign a math.* call didn't vectorize");
            return dumps.Any(d => d.Error != null) ? 1 : 0;
        }

        static string ShortenLeft(string s, int max) => s.Length <= max ? s : "…" + s.Substring(s.Length - max + 1);

        static string Short(string target) => target switch
        {
            "X64_SSE4" => "SSE4",
            "X64_SSE2" => "SSE2",
            "ARMV8A_AARCH64" => "ARM64",
            "ARMV8A_AARCH64_HALFFP" => "ARM64H",
            "ARMV7A_NEON32" => "ARMv7",
            "THUMB2_NEON32" => "Thumb2",
            "ARMV9A" => "ARMv9",
            _ => target,
        };

        // -----------------------------------------------------------------------------------------------------------
        // list and targets

        public static int List(Args args, TextWriter w)
        {
            var all = args.Flag("all");
            args.RejectExtraPositionals(1);
            args.RejectUnknown();
            var index = TargetIndex.Load();
            var filter = args.Positional.Count > 0 ? args.Positional[0] : null;
            List<Entry> list;
            if (filter == null)
                list = index.Entries;
            else
            {
                list = index.Resolve(filter);
                if (list.Count == 0 && !filter.Contains('*'))
                    list = index.Entries.Where(e => e.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var max = all ? int.MaxValue : 40;
            if (list.Count > max)
                w.WriteLine($"{list.Count} entry points, first {max} shown (--all, or narrow the filter):");
            foreach (var e in list.OrderBy(e => e.Name, StringComparer.Ordinal).Take(max))
            {
                var tags = new List<string> { e.IsStatic ? "static" : e.Interface };
                if (!e.HasBurst)
                    tags.Add("no [BurstCompile]");
                if (!string.IsNullOrEmpty(e.FloatMode) && e.FloatMode != "Default")
                    tags.Add("FloatMode." + e.FloatMode);
                if (!string.IsNullOrEmpty(e.FloatPrecision) && e.FloatPrecision != "Standard")
                    tags.Add("FloatPrecision." + e.FloatPrecision);
                if (!string.IsNullOrEmpty(e.OptimizeFor) && e.OptimizeFor != "Default")
                    tags.Add("OptimizeFor." + e.OptimizeFor);
                if (e.DisableSafety)
                    tags.Add("DisableSafetyChecks");
                w.WriteLine($"{e.Name}  [{string.Join(", ", tags)}]");
            }
            if (list.Count <= max)
                w.WriteLine($"{list.Count} entry point{(list.Count == 1 ? "" : "s")}");
            if (index.Stale)
                w.WriteLine("note: " + index.Note);
            return 0;
        }

        public static int Targets(Args args, TextWriter w)
        {
            args.RejectExtraPositionals(0);
            args.RejectUnknown();
            var r = EditorScript.Call(Project.Root, "TargetList", (string)null);
            w.WriteLine($"targets this Editor's Burst compiles (tried on {r.GetProperty("probe").GetString()}; host {r.GetProperty("host").GetString()}):");
            foreach (var t in r.GetProperty("targets").EnumerateArray())
            {
                var name = t.GetProperty("name").GetString();
                var ok = t.GetProperty("ok").GetBoolean();
                var inEnum = t.GetProperty("inEnum").GetBoolean();
                w.WriteLine($"  {name,-24} {(ok ? "ok" : "no: " + Report.Shorten(t.GetProperty("error").GetString().Split('\n')[0], 70))}{(inEnum ? "" : " (not in the Inspector's list)")}");
            }
            w.WriteLine("aliases: avx2, avx, sse4, sse2, arm64/neon, armv7, armv9, wasm, auto (the host)");
            return 0;
        }
    }
}
