// The Editor half of buildwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/BuildWaitress/, and
// runs it through `unity command run_script`, so nothing is imported into the project. Build makes a player build;
// Types exports the Entities TypeManager table that subscene analysis needs for names and chunk layouts.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildWaitressEditor
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    // {"scenes":[...],"out":"<path>","target":"StandaloneWindows64","options":["Development",...]}
    public static string Build(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return Error("the Editor is compiling or importing; try again when it finishes");
        if (EditorApplication.isPlaying)
            return Error("the Editor is in play mode");
        for (var s = 0; s < EditorSceneManager.sceneCount; s++)
        {
            var scene = EditorSceneManager.GetSceneAt(s);
            if (scene.isDirty)
                return Error($"scene '{(string.IsNullOrEmpty(scene.path) ? scene.name : scene.path)}' has unsaved changes; save or discard them first (a build would stop at a save dialog)");
        }
        var scenes = root.TryGetValue("scenes", out var list) && list is List<object> l && l.Count > 0
            ? l.Cast<string>().ToArray()
            : EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
        if (scenes.Length == 0)
            return Error("no scenes: pass scenes or enable some in the build profile's scene list");
        foreach (var path in scenes)
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
                return Error($"'{path}' is not a scene in this project");
        var target = EditorUserBuildSettings.activeBuildTarget;
        if (root.TryGetValue("target", out var t) && t is string ts && ts.Length > 0)
        {
            if (!Enum.TryParse(ts, true, out target))
                return Error($"unknown build target '{ts}'; for example {string.Join(", ", new[] { "StandaloneWindows64", "StandaloneLinux64", "StandaloneOSX", "Android", "WebGL" })}");
        }
        var group = BuildPipeline.GetBuildTargetGroup(target);
        if (!BuildPipeline.IsBuildTargetSupported(group, target))
            return Error($"build support for {target} isn't installed in this Editor");
        var options = BuildOptions.None;
        if (root.TryGetValue("options", out var o) && o is List<object> ol)
            foreach (string name in ol)
            {
                if (!Enum.TryParse(name, true, out BuildOptions flag))
                    return Error($"unknown build option '{name}'");
                options |= flag;
            }
        var output = (string)root["out"];
        var status = (string)root["status"];
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(status)));
        File.WriteAllText(status, "{\"state\":\"running\"}", new UTF8Encoding(false));
        // The build outlives the command's main-thread time limit, so it runs on the next Editor update and reports
        // through the status file.
        EditorApplication.delayCall += () => RunBuild(scenes, output, target, group, options, status);
        return "{\"ok\":true,\"scheduled\":true,\"target\":" + Q(target.ToString()) + ",\"scenes\":" + scenes.Length + "}";
    }

    static void RunBuild(string[] scenes, string output, BuildTarget target, BuildTargetGroup group, BuildOptions options, string status)
    {
        // Failures before the build starts (build processors, script compilation) never reach the report.
        var logged = new List<string>();
        Application.LogCallback capture = (text, stack, type) =>
        {
            if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) && logged.Count < 40 && !logged.Contains(text))
                logged.Add(text.Length > 2000 ? text.Substring(0, 2000) : text);
        };
        var before = Snapshot();
        var sb = new StringBuilder("{\"state\":\"done\"");
        Application.logMessageReceived += capture;
        try
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = target,
                targetGroup = group,
                options = options,
            });
            sb.Append(",\"result\":").Append(Q(report.summary.result.ToString()));
            sb.Append(",\"guid\":").Append(Q(report.summary.guid.ToString()));
            sb.Append(",\"seconds\":").Append(report.summary.totalTime.TotalSeconds.ToString("0.#", Inv));
        }
        catch (Exception e)
        {
            sb.Append(",\"result\":\"Failed\"");
            logged.Add(e.GetType().Name + ": " + e.Message);
        }
        finally
        {
            Application.logMessageReceived -= capture;
        }
        var after = Snapshot();
        var changed = after.Where(kv => !before.TryGetValue(kv.Key, out var old) || old != kv.Value).Select(kv => kv.Key)
            .Concat(before.Keys.Where(k => !after.ContainsKey(k)).Select(k => k + " (deleted)")).Take(60).ToList();
        sb.Append(",\"errors\":[").Append(string.Join(",", logged.Select(Q))).Append(']');
        sb.Append(",\"changed\":[").Append(string.Join(",", changed.Select(Q))).Append(']');
        File.WriteAllText(status, sb.Append('}').ToString(), new UTF8Encoding(false));
    }

    // Project files a build may rewrite as a side effect (settings, generated assets), by size and write time.
    static Dictionary<string, string> Snapshot()
    {
        var map = new Dictionary<string, string>();
        foreach (var top in new[] { "Assets", "ProjectSettings" })
            foreach (var f in Directory.EnumerateFiles(top, "*", SearchOption.AllDirectories))
            {
                var info = new FileInfo(f);
                map[f.Replace('\\', '/')] = info.Length + "@" + info.LastWriteTimeUtc.Ticks;
            }
        return map;
    }

    // {"out":"<file>"}: one line per TypeManager type.
    public static string Types(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var tm = Type.GetType("Unity.Entities.TypeManager, Unity.Entities");
        if (tm == null)
            return Error("the project doesn't use Entities");
        tm.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
        var all = (System.Collections.IEnumerable)tm.GetProperty("AllTypes").GetValue(null);
        var clearMask = (int)tm.GetField("ClearFlagsMask").GetValue(null);
        var sb = new StringBuilder();
        sb.Append("# hash\tmemoryOrdering\tflags\tcategory\tsizeInChunk\ttypeSize\telementSize\tbufferCapacity\tmaxChunkCapacity\tentityOffsets\tblobOffsets\tweakRefs\tunityObjectRefs\tname\n");
        var count = 0;
        foreach (var info in all)
        {
            var it = info.GetType();
            object F(string name) => it.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(info);
            var type = (Type)it.GetProperty("Type")?.GetValue(info);
            var typeIndex = F("TypeIndex");
            var value = (int)typeIndex.GetType().GetField("Value").GetValue(typeIndex);
            if (value == 0)
                continue;
            var blobCount = (int)F("BlobAssetRefOffsetCount");
            var blobs = new List<int>();
            if (blobCount > 0 && type != null)
                BlobOffsets(type, 0, blobs, 0);
            var blobText = blobCount == 0 ? "" : blobs.Count == blobCount ? string.Join(",", blobs) : "?" + blobCount;
            var name = type?.FullName ?? it.GetProperty("DebugTypeName")?.GetValue(info)?.ToString() ?? "?";
            sb.Append(F("StableTypeHash")).Append('\t').Append(F("MemoryOrdering")).Append('\t').Append(value & ~clearMask).Append('\t')
              .Append(F("Category")).Append('\t').Append(F("SizeInChunk")).Append('\t').Append(F("TypeSize")).Append('\t').Append(F("ElementSize")).Append('\t')
              .Append(F("BufferCapacity")).Append('\t').Append(F("MaximumChunkCapacity")).Append('\t').Append(F("EntityOffsetCount")).Append('\t')
              .Append(blobText).Append('\t').Append(F("WeakAssetRefOffsetCount")).Append('\t').Append(F("UnityObjectRefOffsetCount")).Append('\t')
              .Append(name.Replace('\t', ' ').Replace('\n', ' ')).Append('\n');
            count++;
        }
        var outFile = (string)root["out"];
        Directory.CreateDirectory(Path.GetDirectoryName(outFile));
        File.WriteAllText(outFile, sb.ToString(), new UTF8Encoding(false));
        return "{\"ok\":true,\"types\":" + count + "}";
    }

    // Offsets of BlobAssetReference fields, the way TypeManager finds them: in the component, or in one buffer element.
    static void BlobOffsets(Type type, int baseOffset, List<int> into, int depth)
    {
        if (depth > 16 || type.IsPrimitive || type.IsPointer || type.IsEnum)
            return;
        var buffer = type.GetInterfaces().Any(x => x.FullName == "Unity.Entities.IBufferElementData");
        foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var ft = f.FieldType;
            if (!ft.IsValueType || ft.IsPrimitive || ft.IsPointer || ft.IsEnum)
                continue;
            var at = baseOffset + Unity.Collections.LowLevel.Unsafe.UnsafeUtility.GetFieldOffset(f);
            if (ft.FullName == "Unity.Entities.BlobAssetReferenceData")
                into.Add(at);
            else if (f.GetCustomAttribute<System.Runtime.CompilerServices.FixedBufferAttribute>() == null)
                BlobOffsets(ft, at, into, depth + 1);
        }
    }

    static string Error(string message) => "{\"ok\":false,\"error\":" + Q(message) + "}";

    static string Q(string s)
    {
        var sb = new StringBuilder("\"");
        foreach (var c in s)
        {
            if (c == '"' || c == '\\')
                sb.Append('\\').Append(c);
            else if (c < 32)
                sb.Append("\\u").Append(((int)c).ToString("x4"));
            else
                sb.Append(c);
        }
        return sb.Append('"').ToString();
    }

    static object Json(string s, ref int i)
    {
        while (char.IsWhiteSpace(s[i]))
            i++;
        switch (s[i])
        {
            case '{':
            {
                var d = new Dictionary<string, object>();
                i++;
                while (true)
                {
                    while (char.IsWhiteSpace(s[i]) || s[i] == ',')
                        i++;
                    if (s[i] == '}')
                    {
                        i++;
                        return d;
                    }
                    var key = (string)Json(s, ref i);
                    while (char.IsWhiteSpace(s[i]) || s[i] == ':')
                        i++;
                    d[key] = Json(s, ref i);
                }
            }
            case '[':
            {
                var l = new List<object>();
                i++;
                while (true)
                {
                    while (char.IsWhiteSpace(s[i]) || s[i] == ',')
                        i++;
                    if (s[i] == ']')
                    {
                        i++;
                        return l;
                    }
                    l.Add(Json(s, ref i));
                }
            }
            case '"':
            {
                var sb = new StringBuilder();
                i++;
                while (s[i] != '"')
                {
                    if (s[i] == '\\')
                    {
                        i++;
                        if (s[i] == 'u')
                        {
                            sb.Append((char)int.Parse(s.Substring(i + 1, 4), NumberStyles.HexNumber));
                            i += 4;
                        }
                        else
                            sb.Append(s[i] == 'n' ? '\n' : s[i] == 't' ? '\t' : s[i]);
                    }
                    else
                        sb.Append(s[i]);
                    i++;
                }
                i++;
                return sb.ToString();
            }
            default:
            {
                var start = i;
                while (i < s.Length && ",}] \n\r\t".IndexOf(s[i]) < 0)
                    i++;
                var word = s.Substring(start, i - start);
                if (word == "true")
                    return true;
                if (word == "false")
                    return false;
                if (word == "null")
                    return null;
                return double.Parse(word, Inv);
            }
        }
    }
}
