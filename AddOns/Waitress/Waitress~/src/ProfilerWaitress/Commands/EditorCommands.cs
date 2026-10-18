using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using ProfilerWaitress.Cli;
using ProfilerWaitress.Data;
using ProfilerWaitress.Text;

namespace ProfilerWaitress.Commands
{
    public static class EditorCommands
    {
        public static int Extract(Args args, TextWriter w)
        {
            var path = CaptureStore.ResolveCapture(args.Positional0("capture"));
            var force = args.Flag("force");
            args.RejectUnknown();
            var cache = CaptureStore.CachePathFor(path);
            if (!force && CaptureStore.IsFresh(path, cache))
            {
                w.WriteLine($"up to date: {cache}");
                return 0;
            }
            CaptureStore.Extract(path, cache);
            w.WriteLine($"wrote {cache} ({Format.Bytes(new FileInfo(cache).Length)})");
            return 0;
        }

        public static int Convert(Args args, TextWriter w)
        {
            var input = CaptureStore.ResolveCapture(args.Positional0("capture"));
            var output = args.PositionalAt(1, "output .data path");
            args.RejectUnknown();
            if (!output.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException("the output must be a .data file (the Profiler window's format)");
            var root = CaptureStore.ProjectRoot(input);
            var reply = EditorScript.Call(root, "Convert", input.Replace('\\', '/'), Path.GetFullPath(output).Replace('\\', '/'));
            w.WriteLine($"wrote {Path.GetFullPath(output)} ({reply.GetProperty("frames").GetInt32()} frames)");
            return 0;
        }

        static readonly (string name, int bit)[] s_Stacks = { ("gc", 1), ("malloc", 2), ("complete", 4), ("native", 8) };

        public static int Record(Args args, TextWriter w, TextWriter log)
        {
            var frames = args.GetInt("frames", 300);
            var output = args.Get("out");
            var editor = args.Flag("editor");
            var deep = args.Flag("deep");
            var play = args.Flag("play");
            var warmup = args.GetInt("warmup", play ? 60 : 10);
            var timeout = args.GetDouble("timeout", 120);
            var stacksText = args.Get("callstacks");
            var noExtract = args.Flag("no-extract");
            args.RejectUnknown();
            args.RejectExtraPositionals(0);
            if (frames < 1)
                throw new WaitressException("--frames must be at least 1");
            if (warmup < 0)
                throw new WaitressException("--warmup can't be negative");
            if (play && editor)
                throw new WaitressException("--play records the game in Play Mode, --editor records the Editor in edit mode; pick one");
            var stacks = 0;
            if (stacksText != null)
                foreach (var s in stacksText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var hit = s_Stacks.FirstOrDefault(x => x.name == s.ToLowerInvariant());
                    if (hit.name == null)
                        throw new WaitressException($"--callstacks takes gc, malloc, complete, native (comma-separated), not '{s}'");
                    stacks |= hit.bit;
                }

            var root = CaptureStore.ProjectRoot();
            output ??= Path.Combine(root, "ProfilerCaptures", $"{Path.GetFileName(root)}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.data");
            if (!output.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException("--out must be a .data file");
            output = Path.GetFullPath(output);
            var statusFile = output + ".status";

            var enteredPlay = false;
            if (play && !Status(root).GetProperty("playing").GetBoolean())
            {
                log.WriteLine("entering Play Mode...");
                EditorScript.Call(root, "SetPlayMode", true);
                enteredPlay = true;
                WaitFor(root, s => s.GetProperty("playing").GetBoolean(), 120, "Play Mode to start");
            }
            try
            {
                EditorScript.Call(root, "RecordStart", editor, deep, stacks, warmup, frames, output.Replace('\\', '/'));
                log.WriteLine($"recording {frames} frames{(editor ? " of the Editor" : "")} after {warmup} warm-up frames...");
                // Only the file system is polled while recording: every Editor call compiles C# inside the capture.
                var clock = Stopwatch.StartNew();
                while (!File.Exists(statusFile) && clock.Elapsed.TotalSeconds < timeout)
                    Thread.Sleep(200);
                JsonElement result;
                if (File.Exists(statusFile))
                {
                    Thread.Sleep(50);
                    result = JsonDocument.Parse(File.ReadAllText(statusFile)).RootElement;
                    File.Delete(statusFile);
                }
                else
                {
                    var status = Status(root);
                    log.WriteLine($"timed out after {timeout} s with {status.GetProperty("frames").GetInt32()} frames recorded " +
                                  "(is the Editor ticking? Play Mode in an unfocused Editor needs Pipeline's set_autotick); saving what there is");
                    result = EditorScript.Call(root, "RecordAbort", output.Replace('\\', '/'));
                }
                if (result.TryGetProperty("ok", out var ok) && ok.ValueKind == JsonValueKind.False)
                    throw new WaitressException(result.GetProperty("error").GetString());
                var note = result.TryGetProperty("note", out var n) ? $"; {n.GetString()}" : "";
                var got = result.GetProperty("frames").GetInt32();
                if (note.Length == 0 && got > frames)
                    note = $"; {got - frames} more than asked: the Profiler delivered {(got - frames == 1 ? "a frame" : "frames")} while stopping";
                w.WriteLine($"wrote {output} ({got} frames{note})");
            }
            finally
            {
                if (enteredPlay)
                {
                    try
                    {
                        EditorScript.Call(root, "SetPlayMode", false);
                    }
                    catch (WaitressException e)
                    {
                        log.WriteLine("could not leave Play Mode: " + e.Message);
                    }
                }
            }
            if (!noExtract)
            {
                using var capture = CaptureStore.Open(output);
                Overview.Info(capture, Args.Parse(Array.Empty<string>()), w);
            }
            return 0;
        }

        static JsonElement Status(string root) => EditorScript.Call(root, "RecordStatus");

        static void WaitFor(string root, Func<JsonElement, bool> done, double seconds, string what)
        {
            var clock = Stopwatch.StartNew();
            while (clock.Elapsed.TotalSeconds < seconds)
            {
                try
                {
                    if (done(Status(root)))
                        return;
                }
                catch (WaitressException)
                {
                    // Entering Play Mode reloads the domain, which drops the connection for a moment.
                }
                Thread.Sleep(500);
            }
            throw new WaitressException($"gave up waiting for {what}");
        }

        public static int Cache(Args args, TextWriter w)
        {
            var action = args.Positional.Count > 0 ? args.Positional[0] : "list";
            args.RejectUnknown();
            var dir = CaptureStore.CacheDirectory(CaptureStore.ProjectRoot());
            if (!Directory.Exists(dir))
            {
                w.WriteLine("cache is empty");
                return 0;
            }
            var files = new DirectoryInfo(dir).GetFiles("*.pwx").OrderByDescending(f => f.LastAccessTimeUtc).ToList();
            switch (action)
            {
                case "list":
                    var t = new Table("size", "used", "fresh", "source");
                    foreach (var f in files)
                    {
                        var stamp = CaptureFile.ReadStamp(f.FullName);
                        var source = stamp?.source ?? "(unreadable)";
                        var fresh = stamp != null && File.Exists(source) && CaptureStore.IsFresh(source, f.FullName) ? "yes" : File.Exists(source) ? "stale" : "orphan";
                        t.Add(Format.Bytes(f.Length), f.LastAccessTime.ToString("MM-dd HH:mm"), fresh, source);
                    }
                    t.Write(w);
                    w.WriteLine($"# {files.Count} files, {Format.Bytes(files.Sum(f => f.Length))} in {dir}");
                    return 0;
                case "clear":
                    foreach (var f in files)
                        f.Delete();
                    w.WriteLine($"deleted {files.Count} files");
                    return 0;
                default:
                    throw new WaitressException("cache takes list or clear");
            }
        }
    }
}
