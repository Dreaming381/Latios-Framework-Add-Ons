using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>Takes a snapshot of the open Editor (edit mode or Play Mode) and lists the snapshots a project has.</summary>
    public static class Capture
    {
        const int k_Managed = 1, k_NativeObjects = 2, k_NativeAllocations = 4, k_AllocationSites = 8, k_Stacks = 16;

        public static int Run(Args args, TextWriter w, TextWriter log)
        {
            var play = args.Flag("play");
            var wait = args.GetDouble("wait", 2);
            var name = args.Get("name");
            var output = args.Get("out");
            var callstacks = args.Flag("callstacks");
            var noInfo = args.Flag("no-info");
            var timeout = args.GetDouble("timeout", 300);
            args.RejectUnknown();
            args.RejectExtraPositionals(0);
            if (wait < 0)
                throw new WaitressException("--wait can't be negative");

            var root = SnapStore.ProjectRoot();
            var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            output ??= Path.Combine(SnapStore.CaptureFolder(root), $"{Path.GetFileName(root)}_{stamp}{(name != null ? "_" + name : "")}.snap");
            if (!output.EndsWith(".snap", StringComparison.OrdinalIgnoreCase))
                throw new WaitressException("--out must be a .snap file");
            output = Path.GetFullPath(output);
            var flags = k_Managed | k_NativeObjects | k_NativeAllocations | (callstacks ? k_AllocationSites | k_Stacks : 0);

            var enteredPlay = false;
            if (play && !EditorScript.Call(root, "Status").GetProperty("playing").GetBoolean())
            {
                log.WriteLine("entering Play Mode...");
                EditorScript.Call(root, "SetPlayMode", true);
                enteredPlay = true;
                WaitFor(root, s => s.GetProperty("playing").GetBoolean(), 120, "Play Mode to start");
                Thread.Sleep(TimeSpan.FromSeconds(wait));
            }
            try
            {
                var status = output + ".status";
                EditorScript.Call(root, "Capture", output.Replace('\\', '/'), flags);
                var clock = Stopwatch.StartNew();
                while (!File.Exists(status) && clock.Elapsed.TotalSeconds < timeout)
                    Thread.Sleep(200);
                if (!File.Exists(status))
                    throw new WaitressException($"the snapshot did not finish within {timeout} s (is the Editor responsive?)");
                Thread.Sleep(50);
                var result = JsonDocument.Parse(File.ReadAllText(status)).RootElement;
                File.Delete(status);
                if (result.TryGetProperty("ok", out var ok) && ok.ValueKind == JsonValueKind.False)
                    throw new WaitressException(result.GetProperty("error").GetString());
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
            w.WriteLine($"wrote {output} ({Format.Bytes(new FileInfo(output).Length)})");
            if (!noInfo)
                Info.Run(new Model(Snapshot.Load(output)), Args.Parse(Array.Empty<string>()), w);
            return 0;
        }

        public static int List(Args args, TextWriter w)
        {
            args.RejectUnknown();
            var folder = args.Positional.Count > 0 ? args.Positional[0] : SnapStore.CaptureFolder(SnapStore.ProjectRoot());
            if (!Directory.Exists(folder))
            {
                w.WriteLine($"no snapshots: {folder} does not exist");
                return 0;
            }
            var files = new DirectoryInfo(folder).GetFiles("*.snap").OrderByDescending(f => f.LastWriteTimeUtc).ToList();
            var t = new Table("size", "taken", "name");
            foreach (var f in files)
                t.Add(Format.Bytes(f.Length), f.LastWriteTime.ToString("yyyy-MM-dd HH:mm"), f.Name);
            t.Write(w);
            w.WriteLine($"# {files.Count} snapshots in {folder}");
            return 0;
        }

        static void WaitFor(string root, Func<JsonElement, bool> done, double seconds, string what)
        {
            var clock = Stopwatch.StartNew();
            while (clock.Elapsed.TotalSeconds < seconds)
            {
                try
                {
                    if (done(EditorScript.Call(root, "Status")))
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
    }
}
