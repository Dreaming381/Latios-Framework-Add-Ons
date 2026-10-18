using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BurstWaitress.Asm;

namespace BurstWaitress.Model
{
    public sealed class Dump
    {
        public Entry Entry;
        public CompileOptions Options;
        public string Key;
        public string AsmPath;
        public string Error;
        public string OptionString;
        public long Ms;
        public bool Cached;
        AsmFile m_Parsed;
        string m_Text;

        public string Text => m_Text ??= File.Exists(AsmPath) ? File.ReadAllText(AsmPath) : "";

        public AsmFile Asm => m_Parsed ??= Analyzer.Analyze(Text, Options.Target, Entry.Name);
    }

    /// <summary>
    /// Disassembly cache in UserSettings/Waitress/BurstWaitress/Dumps/, keyed by code epoch, entry point and options.
    /// Missing dumps are compiled by the Editor script on a background thread; the script returns before the
    /// Pipeline's 30 s main-thread limit and the tool waits for the files, so a long sweep never gets the command killed.
    /// </summary>
    public static class DumpStore
    {
        public static int TimeoutSeconds = 180;
        const int BudgetMs = 20000;

        static string s_ScriptHash;

        // The Editor script's hash is part of the key, so a tool update never serves dumps made the old way.
        public static string KeyFor(string epoch, Entry e, CompileOptions o) =>
            Project.Hash(epoch + "\n" + (s_ScriptHash ??= Project.Hash(EditorScript.Source())) + "\n" + e.Name + "\n" + o.Request());

        public static List<Dump> Get(IReadOnlyList<(Entry entry, CompileOptions options)> wanted, TextWriter notes)
        {
            var epoch = Project.Epoch();
            var dir = Project.DumpDir;
            Directory.CreateDirectory(dir);
            var dumps = wanted.Select(w =>
            {
                var key = KeyFor(epoch, w.entry, w.options);
                return new Dump { Entry = w.entry, Options = w.options, Key = key, AsmPath = Path.Combine(dir, key + ".asm") };
            }).ToList();
            var missing = dumps.Where(d => !File.Exists(Path.Combine(dir, d.Key + ".meta"))).GroupBy(d => d.Key).Select(g => g.First()).ToList();
            foreach (var d in dumps.Where(d => !missing.Contains(d)))
                d.Cached = true;
            if (missing.Count > 0)
                Request(missing, dumps.Count - missing.Count, dir, notes);
            foreach (var d in dumps)
                ReadMeta(d, dir);
            return dumps;
        }

        static void Request(List<Dump> missing, int cached, string dir, TextWriter notes)
        {
            var requestDir = Path.Combine(Project.Dir, "Requests");
            Directory.CreateDirectory(requestDir);
            foreach (var old in Directory.GetFiles(requestDir, "*.tsv").Where(f => File.GetLastWriteTimeUtc(f) < DateTime.UtcNow.AddDays(-1)))
                File.Delete(old);
            var file = Path.Combine(requestDir, Guid.NewGuid().ToString("N").Substring(0, 12) + ".tsv");
            var sb = new StringBuilder();
            foreach (var d in missing)
                sb.Append(d.Key).Append('\t').Append(d.Entry.Name).Append('\t').Append(d.Options.Request()).Append('\n');
            File.WriteAllText(file, sb.ToString(), new UTF8Encoding(false));
            ConsoleCancelEventHandler cancel = (_, _) => TryDelete(file);
            Console.CancelKeyPress += cancel;
            // The Editor works through the request only while this process touches it, so a killed run stops it.
            using var heartbeat = new Timer(_ =>
            {
                try
                {
                    if (File.Exists(file))
                        File.SetLastWriteTimeUtc(file, DateTime.UtcNow);
                }
                catch (IOException)
                {
                }
            }, null, 2000, 2000);
            try
            {
                var sw = Stopwatch.StartNew();
                var reply = EditorScript.Call(Project.Root, "Dump", file, dir, BudgetMs);
                if (reply.TryGetProperty("finished", out var finished) && finished.ValueKind == System.Text.Json.JsonValueKind.True)
                    return;
                if (reply.TryGetProperty("queued", out var queued) && queued.ValueKind == System.Text.Json.JsonValueKind.True)
                    Console.Error.WriteLine("burstwaitress: waiting for an earlier request in the Editor's Burst queue");
                // The Editor keeps compiling in the background; wait for its files.
                var lastNote = 0.0;
                var pending = missing.Select(d => Path.Combine(dir, d.Key + ".meta")).ToList();
                while (true)
                {
                    pending.RemoveAll(File.Exists);
                    var left = pending.Count;
                    if (left == 0)
                        break;
                    var t = sw.Elapsed.TotalSeconds;
                    if (missing.Count >= 10 && t - lastNote > 15)
                    {
                        lastNote = t;
                        Console.Error.WriteLine($"burstwaitress: {missing.Count - left} of {missing.Count} dumps compiled{(cached > 0 ? $" ({cached} more were cached)" : "")}, {(int)t} s");
                    }
                    if (t > TimeoutSeconds)
                        throw new WaitressException($"{left} of {missing.Count} dumps still compiling after {TimeoutSeconds} s; finished ones are cached, so re-running continues where this stopped (or raise --timeout)");
                    Thread.Sleep(250);
                }
            }
            finally
            {
                Console.CancelKeyPress -= cancel;
                TryDelete(file);
            }
        }

        static void TryDelete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException)
            {
            }
        }

        static void ReadMeta(Dump d, string dir)
        {
            var meta = Path.Combine(dir, d.Key + ".meta");
            if (!File.Exists(meta))
            {
                d.Error = "no dump was produced";
                return;
            }
            foreach (var line in File.ReadAllLines(meta))
            {
                var tab = line.IndexOf('\t');
                if (tab < 0)
                    continue;
                var k = line.Substring(0, tab);
                var v = line.Substring(tab + 1);
                switch (k)
                {
                    case "error":
                        d.Error = v.Replace("\\n", "\n");
                        break;
                    case "options":
                        d.OptionString = v.Replace("|", "\n");
                        break;
                    case "ms":
                        long.TryParse(v, out d.Ms);
                        break;
                }
            }
        }

        /// <summary>Deletes dumps from earlier code epochs that nothing can use any more, beyond the newest few hundred.</summary>
        public static void Prune()
        {
            var dir = Project.DumpDir;
            if (!Directory.Exists(dir))
                return;
            var files = new DirectoryInfo(dir).GetFiles("*.meta").OrderByDescending(f => f.LastWriteTimeUtc).Skip(2000).ToList();
            foreach (var f in files)
            {
                f.Delete();
                var asm = Path.ChangeExtension(f.FullName, ".asm");
                if (File.Exists(asm))
                    File.Delete(asm);
            }
        }
    }
}
