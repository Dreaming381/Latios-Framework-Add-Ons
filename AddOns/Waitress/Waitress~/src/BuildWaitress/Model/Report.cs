using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BuildWaitress.Serialized;

namespace BuildWaitress.Model
{
    public sealed class OutputFile
    {
        public string Path;
        public string Role;
        public long Size;
        /// <summary>Path below the build's output folder, with '/'.</summary>
        public string Rel;
    }

    public sealed class Step
    {
        public string Name;
        public int Depth;
        public TimeSpan Duration;
        public readonly List<(int type, string text)> Messages = new List<(int, string)>();
        public Step Parent;
    }

    public sealed class PackedAsset
    {
        public string File;
        public long FileId;
        public int ClassId;
        public long Size;
        public string Guid;
        public string Path;
    }

    /// <summary>A build report (Library/LastBuild.buildreport or a copy), read offline.</summary>
    public sealed class Report
    {
        public string Source;
        public string UnityVersion;
        public string Guid;
        public DateTime Started;
        public TimeSpan Duration;
        public string Platform;
        public string PlatformGroup;
        public long Options;
        public string OutputPath;
        public long TotalSize;
        public int Errors;
        public int Warnings;
        public string Result;
        public int BuildType;
        public readonly List<OutputFile> Files = new List<OutputFile>();
        public readonly List<Step> Steps = new List<Step>();
        public readonly List<(string file, long overhead)> Packed = new List<(string, long)>();
        public readonly List<PackedAsset> Assets = new List<PackedAsset>();
        public readonly Dictionary<string, List<string>> ScenesUsing = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        public bool HasSceneUsage;
        public readonly List<string> Modules = new List<string>();
        public readonly Dictionary<string, List<string>> ModuleReasons = new Dictionary<string, List<string>>();

        /// <summary>The folder files are listed under: the player's folder for a standalone exe, else the output path.</summary>
        public string OutputDir;

        public static Report Load(string path)
        {
            var f = SerializedFile.Read(path);
            var r = new Report { Source = path, UnityVersion = f.UnityVersion };
            var main = f.Objects.FirstOrDefault(o => o.ClassId == 1125) ?? throw new WaitressException($"{path} has no BuildReport object");
            var root = (Dictionary<string, object>)f.Decode(main);
            var s = D(root["m_Summary"]);
            r.Started = new DateTime(L(D(s["buildStartTime"])["ticks"]), DateTimeKind.Utc).ToLocalTime();
            r.Guid = GuidOf(s["buildGUID"]);
            r.Platform = (string)s["platformName"];
            r.PlatformGroup = (string)s["platformGroupName"];
            r.Options = L(s["options"]);
            r.OutputPath = ((string)s["outputPath"]).Replace('\\', '/');
            r.TotalSize = L(s["totalSize"]);
            r.Duration = new TimeSpan(L(s["totalTimeTicks"]));
            r.Errors = (int)L(s["totalErrors"]);
            r.Warnings = (int)L(s["totalWarnings"]);
            r.BuildType = s.TryGetValue("buildType", out var bt) ? (int)L(bt) : 1;
            r.Result = L(s["buildResult"]) switch { 1 => "Succeeded", 2 => "Failed", 3 => "Cancelled", _ => "Unknown" };
            foreach (var x in (IList)root["m_Files"])
            {
                var d = D(x);
                r.Files.Add(new OutputFile { Path = ((string)d["path"]).Replace('\\', '/'), Role = (string)d["role"], Size = L(d["totalSize"]) });
            }
            r.OutputDir = OutputDirOf(r.OutputPath, r.Files);
            foreach (var file in r.Files)
                file.Rel = file.Path.StartsWith(r.OutputDir + "/", StringComparison.OrdinalIgnoreCase) ? file.Path.Substring(r.OutputDir.Length + 1) : file.Path;
            var stack = new List<Step>();
            foreach (var x in (IList)root["m_BuildSteps"])
            {
                var d = D(x);
                var step = new Step { Name = (string)d["stepName"], Depth = (int)L(d["depth"]), Duration = new TimeSpan(L(d["durationTicks"])) };
                foreach (var m in (IList)d["messages"])
                {
                    var md = D(m);
                    step.Messages.Add(((int)L(md["type"]), ((string)md["content"]).TrimEnd()));
                }
                while (stack.Count > step.Depth)
                    stack.RemoveAt(stack.Count - 1);
                step.Parent = stack.Count > 0 ? stack[^1] : null;
                stack.Add(step);
                r.Steps.Add(step);
            }
            foreach (var o in f.Objects)
            {
                var type = f.TypeOf(o)?.Tree?.Type;
                if (o.ClassId == 1126)
                {
                    var d = (Dictionary<string, object>)f.Decode(o);
                    var file = (string)d["m_ShortPath"];
                    r.Packed.Add((file, L(d["m_Overhead"])));
                    foreach (var x in (IList)d["m_Contents"])
                    {
                        var c = D(x);
                        r.Assets.Add(new PackedAsset
                        {
                            File = file,
                            FileId = L(c["fileID"]),
                            ClassId = (int)L(c["classID"]),
                            Size = L(c["packedSize"]),
                            Guid = GuidOf(c["sourceAssetGUID"]),
                            Path = (string)c["buildTimeAssetPath"],
                        });
                    }
                }
                else if (type == "ScenesUsingAssets")
                {
                    r.HasSceneUsage = true;
                    var d = (Dictionary<string, object>)f.Decode(o);
                    var list = d.Values.OfType<IList>().FirstOrDefault();
                    if (list == null)
                        continue;
                    foreach (var x in list)
                    {
                        var e = D(x);
                        var asset = (string)e.Values.OfType<string>().FirstOrDefault();
                        var scenes = e.Values.OfType<IList>().FirstOrDefault()?.Cast<object>().Select(v => (string)v).ToList() ?? new List<string>();
                        if (asset != null)
                            r.ScenesUsing[asset] = scenes;
                    }
                }
                else if (o.ClassId == 114 && f.TypeOf(o)?.Tree != null)
                {
                    var d = (Dictionary<string, object>)f.Decode(o);
                    if (!d.TryGetValue("modules", out var modules))
                        continue;
                    foreach (var m in (IList)modules)
                        r.Modules.Add((string)m);
                    if (d.TryGetValue("serializedDependencies", out var deps))
                        foreach (var x in (IList)deps)
                        {
                            var e = D(x);
                            r.ModuleReasons[(string)e["key"]] = ((IList)e["value"]).Cast<object>().Select(v => (string)v).ToList();
                        }
                }
            }
            return r;
        }

        static string OutputDirOf(string output, List<OutputFile> files)
        {
            var dir = output.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || output.EndsWith(".x86_64", StringComparison.OrdinalIgnoreCase) ||
                      output.EndsWith(".apk", StringComparison.OrdinalIgnoreCase) || output.EndsWith(".aab", StringComparison.OrdinalIgnoreCase)
                ? System.IO.Path.GetDirectoryName(output)?.Replace('\\', '/') ?? output
                : output.TrimEnd('/');
            if (files.Count > 0 && !files.Any(f => f.Path.StartsWith(dir + "/", StringComparison.OrdinalIgnoreCase)))
            {
                var common = files[0].Path;
                foreach (var f in files)
                    while (!f.Path.StartsWith(common, StringComparison.OrdinalIgnoreCase))
                        common = common.Substring(0, Math.Max(0, common.LastIndexOf('/')));
                dir = common.TrimEnd('/');
            }
            return dir;
        }

        public static Dictionary<string, object> D(object o) => (Dictionary<string, object>)o;

        public static long L(object o) => o switch
        {
            int i => i,
            uint u => u,
            long l => l,
            ulong ul => (long)ul,
            short s => s,
            ushort us => us,
            byte b => b,
            sbyte sb => sb,
            bool z => z ? 1 : 0,
            _ => throw new WaitressException($"expected a number, got {o?.GetType().Name ?? "null"}")
        };

        /// <summary>A GUID stored as four uints, as Unity prints it.</summary>
        public static string GuidOf(object o)
        {
            var d = D(o);
            var sb = new StringBuilder(32);
            for (var i = 0; i < 4; i++)
            {
                var v = (uint)L(d[$"data[{i}]"]);
                for (var k = 0; k < 8; k++)
                    sb.Append("0123456789abcdef"[(int)(v >> (k * 4)) & 15]);
            }
            return sb.ToString();
        }

        public bool IsDevelopment => (Options & 1) != 0;

        public static readonly (long flag, string name)[] OptionNames =
        {
            (1, "Development"), (1 << 2, "AutoRunPlayer"), (1 << 3, "ShowBuiltPlayer"), (1 << 4, "BuildAdditionalStreamedScenes"),
            (1 << 5, "AcceptExternalModificationsToPlayer"), (1 << 6, "InstallInBuildFolder"), (1 << 7, "CleanBuildCache"), (1 << 8, "ConnectWithProfiler"),
            (1 << 9, "AllowDebugging"), (1 << 10, "SymlinkSources"), (1 << 11, "UncompressedAssetBundle"), (1 << 12, "ConnectToHost"),
            (1 << 13, "CustomConnectionID"), (1 << 14, "EnableHeadlessMode"), (1 << 15, "BuildScriptsOnly"), (1 << 16, "PatchPackage"),
            (1 << 17, "ForceEnableAssertions"), (1 << 18, "CompressWithLz4"), (1 << 19, "CompressWithLz4HC"), (1 << 20, "ComputeCRC"),
            (1 << 21, "StrictMode"), (1 << 22, "IncludeTestAssemblies"), (1 << 23, "NoUniqueIdentifier"), (1 << 25, "WaitForPlayerConnection"),
            (1 << 26, "EnableCodeCoverage"), (1 << 28, "EnableDeepProfilingSupport"), (1 << 29, "DetailedBuildReport"),
        };

        public string OptionText()
        {
            var names = OptionNames.Where(o => (Options & o.flag) != 0).Select(o => o.name).ToList();
            return names.Count == 0 ? "none" : string.Join(", ", names);
        }
    }
}
