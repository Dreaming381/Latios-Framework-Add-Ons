using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using MemoryWaitress.Snap;
using MemoryWaitress.Text;

namespace MemoryWaitress.Commands
{
    /// <summary>Takes a snapshot through the Pipeline's own snapshot command (edit mode or Play Mode) and lists the snapshots a project has.</summary>
    public static class Capture
    {
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
            var flags = "ManagedObjects,NativeObjects,NativeAllocations" + (callstacks ? ",NativeAllocationSites,NativeStackTraces" : "");

            var enteredPlay = false;
            if (play && !EditorCommands.Playing(root))
            {
                log.WriteLine("entering Play Mode...");
                EditorCommands.Call(root, "editor.playmode start");
                enteredPlay = true;
                WaitFor(root, () => EditorCommands.Playing(root), 120, "Play Mode to start");
                Thread.Sleep(TimeSpan.FromSeconds(wait));
            }
            string target;
            try
            {
                var started = EditorCommands.Call(root, "diagnostics memory_snapshot", new Dictionary<string, string>
                {
                    ["flags"] = flags,
                    ["output-path"] = output,
                    ["collect-garbage"] = "false",
                    ["timeout-seconds"] = ((int)Math.Ceiling(timeout)).ToString(CultureInfo.InvariantCulture),
                });
                var id = started.GetProperty("snapshotId").GetString();
                var clock = Stopwatch.StartNew();
                while (true)
                {
                    var status = EditorCommands.Call(root, "diagnostics memory_snapshot_status");
                    var state = status.GetProperty("status").GetString();
                    var mine = status.TryGetProperty("snapshotId", out var sid) && sid.GetString() == id;
                    if (mine && state == "completed")
                    {
                        target = status.TryGetProperty("target", out var t) ? t.GetString() : null;
                        break;
                    }
                    if (mine && (state == "failed" || state == "interrupted"))
                        throw new WaitressException($"the snapshot {state}: " + (status.TryGetProperty("error", out var e) ? e.ToString() : status.GetRawText()));
                    if (clock.Elapsed.TotalSeconds > timeout)
                        throw new WaitressException($"the snapshot did not finish within {timeout} s (is the Editor responsive?)");
                    Thread.Sleep(500);
                }
            }
            finally
            {
                if (enteredPlay)
                {
                    try
                    {
                        EditorCommands.Call(root, "editor.playmode stop");
                    }
                    catch (WaitressException e)
                    {
                        log.WriteLine("could not leave Play Mode: " + e.Message);
                    }
                }
            }
            w.WriteLine($"wrote {output} ({Format.Bytes(new FileInfo(output).Length)}){(target != null && target != "Editor" ? " from " + target : "")}");
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

        static void WaitFor(string root, Func<bool> done, double seconds, string what)
        {
            var clock = Stopwatch.StartNew();
            while (clock.Elapsed.TotalSeconds < seconds)
            {
                try
                {
                    if (done())
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
