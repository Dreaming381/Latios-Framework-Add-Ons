// The Editor half of terrainwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/TerrainWaitress/, and
// runs it through `unity command run_script`, so nothing is imported into the project. Export writes each TerrainData's
// settings and prototypes as JSON and its heights, holes, splat weights, detail densities and trees as a binary sidecar.
// Apply writes back what the CLI computed, with Undo.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class TerrainWaitressEditor
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    const float HeightSteps = 32766f;

    static readonly string[] s_DataSettings =
    {
        "wavingGrassStrength", "wavingGrassAmount", "wavingGrassSpeed", "wavingGrassTint", "baseMapResolution", "enableHolesTextureCompression",
    };

    // ---------------------------------------------------------------- export

    // {"paths":[...],"out":"<dir>"}: writes <dir>/Terrains/<guid>.json and .bin per asset.
    public static string Export(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var outDir = Path.Combine((string)root["out"], "Terrains");
        var paths = ((List<object>)root["paths"]).Cast<string>().ToList();
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return Error("the Editor is compiling or importing; try again when it finishes");
        Directory.CreateDirectory(outDir);
        var done = new List<string>();
        var failed = new List<string>();
        foreach (var path in paths)
        {
            try
            {
                if (!File.Exists(FileUtil.GetPhysicalPath(path)))
                    throw new Exception("no such file");
                AssetDatabase.ImportAsset(path);
                var data = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
                if (data == null)
                    throw new Exception("not a TerrainData asset");
                var guid = AssetDatabase.AssetPathToGUID(path);
                var bin = Path.Combine(outDir, guid + ".bin");
                var text = ExportData(path, data, bin);
                File.WriteAllText(Path.Combine(outDir, guid + ".json"), text, new UTF8Encoding(false));
                done.Add(path);
            }
            catch (Exception e)
            {
                failed.Add(path + ": " + e.Message);
            }
        }
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        w.Key("done").Strings(done);
        w.Key("failed").Strings(failed);
        w.End();
        return w.ToString();
    }

    public static string Stamp(string path)
    {
        path = Path.GetFullPath(FileUtil.GetPhysicalPath(path));
        var meta = path + ".meta";
        return File.GetLastWriteTimeUtc(path).Ticks.ToString(Inv) + " " + (File.Exists(meta) ? File.GetLastWriteTimeUtc(meta).Ticks : 0).ToString(Inv);
    }

    static string ExportData(string path, TerrainData data, string binPath)
    {
        var w = new W();
        w.Begin();
        w.Key("v").Num(1);
        w.Key("path").Str(path);
        w.Key("guid").Str(AssetDatabase.AssetPathToGUID(path));
        w.Key("stamp").Str(Stamp(path));
        w.Key("unity").Str(Application.unityVersion);
        w.Key("size").Vec(data.size);
        w.Key("heightmapResolution").Num(data.heightmapResolution);
        w.Key("holesResolution").Num(data.holesResolution);
        w.Key("alphamapResolution").Num(data.alphamapResolution);
        w.Key("detailResolution").Num(data.detailResolution);
        w.Key("detailPatch").Num(data.detailResolutionPerPatch);
        w.Key("detailScatter").Str(data.detailScatterMode.ToString());
        w.Key("settings").Begin();
        foreach (var name in s_DataSettings)
        {
            var p = typeof(TerrainData).GetProperty(name);
            w.Key(name);
            Value(w, p.GetValue(data));
        }
        w.End();
        using (var defaults = new Disposer(new TerrainData()))
        {
            var fresh = (TerrainData)defaults.Object;
            w.Key("defaults").Begin();
            foreach (var name in s_DataSettings)
            {
                w.Key(name);
                Value(w, typeof(TerrainData).GetProperty(name).GetValue(fresh));
            }
            w.End();
        }

        w.Key("layers").Arr();
        foreach (var layer in data.terrainLayers)
        {
            w.Begin();
            if (layer == null)
            {
                w.Key("missing").Bool(true);
                w.End();
                continue;
            }
            w.Key("name").Str(layer.name).Key("asset").Str(AssetDatabase.GetAssetPath(layer));
            w.Key("props").Begin();
            Props(w, layer);
            w.End();
            w.End();
        }
        w.EndArr();

        var validateTree = typeof(TreePrototype).GetMethod("Validate", Any, null, new[] { typeof(string).MakeByRefType() }, null);
        w.Key("treePrototypes").Arr();
        foreach (var p in data.treePrototypes)
        {
            w.Begin();
            w.Key("name").Str(p.prefab == null ? null : p.prefab.name);
            w.Key("props").Begin();
            Props(w, p);
            w.End();
            if (validateTree != null)
            {
                object[] args = { null };
                var ok = (bool)validateTree.Invoke(p, args);
                w.Key("valid").Bool(ok);
                if (!string.IsNullOrEmpty((string)args[0]))
                    w.Key("message").Str((string)args[0]);
            }
            w.End();
        }
        w.EndArr();

        var validateDetail = typeof(DetailPrototype).GetMethod("Validate", Any, null, new[] { typeof(string).MakeByRefType() }, null);
        w.Key("detailPrototypes").Arr();
        var protos = data.detailPrototypes;
        for (var d = 0; d < protos.Length; d++)
        {
            var p = protos[d];
            w.Begin();
            var source = p.usePrototypeMesh ? (UnityEngine.Object)p.prototype : p.prototypeTexture;
            w.Key("name").Str(source == null ? null : source.name);
            w.Key("props").Begin();
            Props(w, p);
            w.End();
            if (validateDetail != null)
            {
                object[] args = { null };
                var ok = (bool)validateDetail.Invoke(p, args);
                w.Key("valid").Bool(ok);
                if (!string.IsNullOrEmpty((string)args[0]))
                    w.Key("message").Str((string)args[0]);
            }
            // What Unity scatters from the densities, as the renderer and Terrainy see it.
            long count = 0;
            var res = data.detailResolution;
            var patch = data.detailResolutionPerPatch;
            if (res > 0 && patch > 0 && (long)res * res <= 4096L * 4096L)
                for (var y = 0; y < res / patch; y++)
                for (var x = 0; x < res / patch; x++)
                    count += data.ComputeDetailInstanceTransforms(x, y, d, p.density, out _).Length;
            w.Key("instances").Num(count);
            w.End();
        }
        w.EndArr();

        var r = data.heightmapResolution;
        var hr = data.holesResolution;
        var a = data.alphamapResolution;
        var layers = data.alphamapLayers;
        var dr = data.detailResolution;
        var trees = data.treeInstances;
        long offset = 0;
        w.Key("bin").Begin();
        w.Key("heights").Num(offset);
        offset += (long)r * r * 2;
        w.Key("holes").Num(offset);
        offset += (long)hr * hr;
        w.Key("alpha").Num(offset);
        w.Key("alphaLayers").Num(layers);
        offset += (long)a * a * layers;
        w.Key("details").Num(offset);
        offset += (long)dr * dr * protos.Length * 2;
        w.Key("trees").Num(offset);
        w.Key("treeCount").Num(trees.Length);
        offset += (long)trees.Length * 36;
        w.Key("length").Num(offset);
        w.End();
        w.End();

        using (var bin = new BinaryWriter(File.Create(binPath)))
        {
            var heights = data.GetHeights(0, 0, r, r);
            for (var y = 0; y < r; y++)
            for (var x = 0; x < r; x++)
                bin.Write((ushort)Mathf.RoundToInt(heights[y, x] * HeightSteps));
            var holes = data.GetHoles(0, 0, hr, hr);
            for (var y = 0; y < hr; y++)
            for (var x = 0; x < hr; x++)
                bin.Write((byte)(holes[y, x] ? 1 : 0));
            if (layers > 0)
            {
                var maps = data.GetAlphamaps(0, 0, a, a);
                for (var l = 0; l < layers; l++)
                for (var y = 0; y < a; y++)
                for (var x = 0; x < a; x++)
                    bin.Write((byte)Mathf.RoundToInt(maps[y, x, l] * 255f));
            }
            for (var l = 0; l < protos.Length; l++)
            {
                var layer = data.GetDetailLayer(0, 0, dr, dr, l);
                for (var y = 0; y < dr; y++)
                for (var x = 0; x < dr; x++)
                    bin.Write((ushort)Mathf.Clamp(layer[y, x], 0, 65535));
            }
            foreach (var t in trees)
            {
                bin.Write(t.position.x);
                bin.Write(t.position.y);
                bin.Write(t.position.z);
                bin.Write(t.widthScale);
                bin.Write(t.heightScale);
                bin.Write(t.rotation);
                bin.Write(t.prototypeIndex);
                bin.Write(Pack(t.color));
                bin.Write(Pack(t.lightmapColor));
            }
        }
        return w.ToString();
    }

    static uint Pack(Color32 c) => (uint)(c.r | c.g << 8 | c.b << 16 | c.a << 24);

    static Color32 Unpack(uint v) => new Color32((byte)v, (byte)(v >> 8), (byte)(v >> 16), (byte)(v >> 24));

    sealed class Disposer : IDisposable
    {
        public readonly UnityEngine.Object Object;
        public Disposer(UnityEngine.Object o) => Object = o;
        public void Dispose() => UnityEngine.Object.DestroyImmediate(Object);
    }

    static IEnumerable<PropertyInfo> Settable(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && p.GetCustomAttribute<ObsoleteAttribute>() == null)
            .Where(p => p.PropertyType.IsPrimitive || p.PropertyType.IsEnum || p.PropertyType == typeof(Color) || p.PropertyType == typeof(Vector2) ||
                        p.PropertyType == typeof(Vector3) || p.PropertyType == typeof(Vector4) || typeof(UnityEngine.Object).IsAssignableFrom(p.PropertyType))
            .Where(p => p.Name != "name" && p.Name != "hideFlags");

    static void Props(W w, object o)
    {
        foreach (var p in Settable(o.GetType()))
        {
            w.Key(p.Name);
            Value(w, p.GetValue(o));
        }
    }

    static void Value(W w, object v)
    {
        switch (v)
        {
            case null:
                w.Str(null);
                break;
            case bool b:
                w.Bool(b);
                break;
            case Enum e:
                w.Str(e.ToString());
                break;
            case Color c:
                w.Arr().Num(c.r).Num(c.g).Num(c.b).Num(c.a).EndArr();
                break;
            case Vector2 v2:
                w.Arr().Num(v2.x).Num(v2.y).EndArr();
                break;
            case Vector3 v3:
                w.Vec(v3);
                break;
            case Vector4 v4:
                w.Arr().Num(v4.x).Num(v4.y).Num(v4.z).Num(v4.w).EndArr();
                break;
            case UnityEngine.Object o:
                w.Str(o == null ? null : AssetDatabase.GetAssetPath(o) is var p && p.Length > 0 ? p : "(" + o.name + ")");
                break;
            default:
                w.Num(Convert.ToDouble(v, Inv));
                break;
        }
    }

    static object Parse(Type type, object json)
    {
        if (type == typeof(bool))
            return (bool)json;
        if (type.IsEnum)
            return Enum.Parse(type, (string)json, true);
        if (type.IsPrimitive)
            return Convert.ChangeType((double)json, type, Inv);
        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            if (json == null || (string)json == "")
                return null;
            var asset = AssetDatabase.LoadAssetAtPath((string)json, type);
            if (asset == null)
                throw new Exception($"{json} is not a {type.Name}");
            return asset;
        }
        var f = ((List<object>)json).Select(x => (float)(double)x).ToArray();
        if (type == typeof(Color))
            return new Color(f[0], f[1], f[2], f.Length > 3 ? f[3] : 1);
        if (type == typeof(Vector2))
            return new Vector2(f[0], f[1]);
        if (type == typeof(Vector3))
            return new Vector3(f[0], f[1], f[2]);
        return new Vector4(f[0], f[1], f[2], f[3]);
    }

    static void SetProps(object target, Dictionary<string, object> props)
    {
        var settable = Settable(target.GetType()).ToDictionary(p => p.Name);
        foreach (var pair in props)
        {
            if (!settable.TryGetValue(pair.Key, out var p))
                throw new Exception($"{target.GetType().Name} has no setting '{pair.Key}'");
            p.SetValue(target, Parse(p.PropertyType, pair.Value));
        }
    }

    // ---------------------------------------------------------------- apply

    // {"path", "size"?, "heightmapResolution"?, "alphamapResolution"?, "detailResolution"?, "detailPatch"?, "settings"?,
    //  "layers"?: [asset paths], "layerProps"?: {asset: {prop: v}}, "treePrototypes"?: [{props}], "detailPrototypes"?: [{props}],
    //  "heights"?, "holes"?, "alpha"?, "details"?: {"file", "x", "y", "w", "h"} rectangles in a binary file, "details" one per
    //  layer, "trees"?: {"file"}}. Sizes and resolutions are set before data, and trees last because SetHeights re-snaps them.
    public static string Apply(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var path = (string)root["path"];
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return Error("the Editor is compiling or importing; try again when it finishes");
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return Error("the Editor is in Play Mode; edits made now would be lost or fight the running game. Exit Play Mode first");
        var data = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
        if (data == null)
            return Error(path + " is not a TerrainData asset");
        // Everything that can fail on a bad asset is built before anything changes. The edit gets an undo group of its
        // own, so a failure reverts only this edit (PerformUndo would revert the whole current group, which in a
        // scripted session spans earlier edits too).
        var layerAssets = new List<TerrainLayer>();
        TreePrototype[] treeProtos = null;
        DetailPrototype[] detailProtos = null;
        TerrainLayer[] layerList = null;
        try
        {
            if (root.TryGetValue("layerProps", out var lp0))
                foreach (var pair in (Dictionary<string, object>)lp0)
                    layerAssets.Add(AssetDatabase.LoadAssetAtPath<TerrainLayer>(pair.Key) ?? throw new Exception(pair.Key + " is not a TerrainLayer"));
            if (root.TryGetValue("treePrototypes", out var tp0))
                treeProtos = ((List<object>)tp0).Select(o =>
                {
                    var p = new TreePrototype();
                    SetProps(p, (Dictionary<string, object>)o);
                    return p;
                }).ToArray();
            if (root.TryGetValue("detailPrototypes", out var dp0))
                detailProtos = ((List<object>)dp0).Select(o =>
                {
                    var p = new DetailPrototype();
                    SetProps(p, (Dictionary<string, object>)o);
                    return p;
                }).ToArray();
            if (root.TryGetValue("layers", out var ls0))
                layerList = ((List<object>)ls0).Select(o => AssetDatabase.LoadAssetAtPath<TerrainLayer>((string)o) ?? throw new Exception(o + " is not a TerrainLayer")).ToArray();
        }
        catch (Exception e)
        {
            return Error((e.InnerException?.Message ?? e.Message) + "; nothing changed");
        }
        Undo.IncrementCurrentGroup();
        var group = Undo.GetCurrentGroup();
        var registered = false;
        try
        {
            var undo = new List<UnityEngine.Object> { data };
            undo.AddRange(data.alphamapTextures);
            undo.AddRange(layerAssets);
            Undo.RegisterCompleteObjectUndo(undo.ToArray(), "terrainwaitress set");
            registered = true;

            if (root.TryGetValue("layerProps", out var lp))
                foreach (var pair in (Dictionary<string, object>)lp)
                {
                    var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(pair.Key);
                    SetProps(layer, (Dictionary<string, object>)pair.Value);
                    EditorUtility.SetDirty(layer);
                }
            if (treeProtos != null)
                data.treePrototypes = treeProtos;
            if (detailProtos != null)
                data.detailPrototypes = detailProtos;
            if (layerList != null)
                data.terrainLayers = layerList;

            // heightmapResolution keeps heightmapScale, so it changes size; size is set again after it.
            var size = data.size;
            if (root.TryGetValue("heightmapResolution", out var hr))
                data.heightmapResolution = (int)(double)hr;
            if (root.TryGetValue("size", out var sz))
                size = (Vector3)Parse(typeof(Vector3), sz);
            data.size = size;
            if (root.TryGetValue("alphamapResolution", out var ar))
                data.alphamapResolution = (int)(double)ar;
            if (root.TryGetValue("detailResolution", out var dr))
                data.SetDetailResolution((int)(double)dr, (int)(double)root["detailPatch"]);
            if (root.TryGetValue("settings", out var st))
                foreach (var pair in (Dictionary<string, object>)st)
                {
                    if (!s_DataSettings.Contains(pair.Key))
                        throw new Exception("TerrainData has no setting " + pair.Key);
                    var p = typeof(TerrainData).GetProperty(pair.Key);
                    p.SetValue(data, Parse(p.PropertyType, pair.Value));
                }

            if (root.TryGetValue("heights", out var h))
            {
                var rect = Rect(h, out var bytes);
                var block = new float[rect.h, rect.w];
                for (var y = 0; y < rect.h; y++)
                for (var x = 0; x < rect.w; x++)
                    block[y, x] = BitConverter.ToUInt16(bytes, (y * rect.w + x) * 2) / HeightSteps;
                data.SetHeights(rect.x, rect.y, block);
            }
            if (root.TryGetValue("holes", out var ho))
            {
                var rect = Rect(ho, out var bytes);
                var block = new bool[rect.h, rect.w];
                for (var y = 0; y < rect.h; y++)
                for (var x = 0; x < rect.w; x++)
                    block[y, x] = bytes[y * rect.w + x] != 0;
                data.SetHoles(rect.x, rect.y, block);
            }
            if (root.TryGetValue("alpha", out var al))
            {
                var rect = Rect(al, out var bytes);
                var layers = data.alphamapLayers;
                var block = new float[rect.h, rect.w, layers];
                for (var l = 0; l < layers; l++)
                for (var y = 0; y < rect.h; y++)
                for (var x = 0; x < rect.w; x++)
                    block[y, x, l] = bytes[(l * rect.h + y) * rect.w + x] / 255f;
                data.SetAlphamaps(rect.x, rect.y, block);
            }
            if (root.TryGetValue("details", out var de))
            {
                var rect = Rect(de, out var bytes);
                var count = data.detailPrototypes.Length;
                for (var l = 0; l < count; l++)
                {
                    var block = new int[rect.h, rect.w];
                    for (var y = 0; y < rect.h; y++)
                    for (var x = 0; x < rect.w; x++)
                        block[y, x] = BitConverter.ToUInt16(bytes, ((l * rect.h + y) * rect.w + x) * 2);
                    data.SetDetailLayer(rect.x, rect.y, l, block);
                }
            }
            if (root.TryGetValue("trees", out var tr))
            {
                var bytes = File.ReadAllBytes((string)((Dictionary<string, object>)tr)["file"]);
                var trees = new TreeInstance[bytes.Length / 36];
                for (var t = 0; t < trees.Length; t++)
                {
                    var o = t * 36;
                    trees[t] = new TreeInstance
                    {
                        position = new Vector3(BitConverter.ToSingle(bytes, o), BitConverter.ToSingle(bytes, o + 4), BitConverter.ToSingle(bytes, o + 8)),
                        widthScale = BitConverter.ToSingle(bytes, o + 12),
                        heightScale = BitConverter.ToSingle(bytes, o + 16),
                        rotation = BitConverter.ToSingle(bytes, o + 20),
                        prototypeIndex = BitConverter.ToInt32(bytes, o + 24),
                        color = Unpack(BitConverter.ToUInt32(bytes, o + 28)),
                        lightmapColor = Unpack(BitConverter.ToUInt32(bytes, o + 32)),
                    };
                }
                data.SetTreeInstances(trees, false);
            }
            EditorUtility.SetDirty(data);
            foreach (var t in data.alphamapTextures)
                EditorUtility.SetDirty(t);
            AssetDatabase.SaveAssetIfDirty(data);
            foreach (var layer in layerAssets)
                AssetDatabase.SaveAssetIfDirty(layer);
            foreach (var terrain in Terrain.activeTerrains.Where(t => t.terrainData == data))
                terrain.Flush();
            Undo.CollapseUndoOperations(group);
            Undo.IncrementCurrentGroup();
        }
        catch (Exception e)
        {
            if (registered)
                Undo.RevertAllDownToGroup(group);
            return Error(e.InnerException?.Message ?? e.Message);
        }
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        w.End();
        return w.ToString();
    }

    static (int x, int y, int w, int h) Rect(object json, out byte[] bytes)
    {
        var m = (Dictionary<string, object>)json;
        bytes = File.ReadAllBytes((string)m["file"]);
        return ((int)(double)m["x"], (int)(double)m["y"], (int)(double)m["w"], (int)(double)m["h"]);
    }

    // ---------------------------------------------------------------- new

    // {"path", "size", "heightmapResolution", "alphamapResolution", "detailResolution", "detailPatch"}: an empty TerrainData.
    public static string Create(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var path = (string)root["path"];
        if (File.Exists(FileUtil.GetPhysicalPath(path)))
            return Error(path + " already exists");
        var folder = Path.GetDirectoryName(path).Replace('\\', '/');
        if (!AssetDatabase.IsValidFolder(folder))
        {
            if (Directory.Exists(FileUtil.GetPhysicalPath(folder)))
                AssetDatabase.ImportAsset(folder);
            else
                Directory.CreateDirectory(FileUtil.GetPhysicalPath(folder));
            AssetDatabase.Refresh();
        }
        var data = new TerrainData();
        data.heightmapResolution = (int)(double)root["heightmapResolution"];
        data.size = (Vector3)Parse(typeof(Vector3), root["size"]);
        data.alphamapResolution = (int)(double)root["alphamapResolution"];
        data.SetDetailResolution((int)(double)root["detailResolution"], (int)(double)root["detailPatch"]);
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        w.End();
        return w.ToString();
    }

    // ---------------------------------------------------------------- copy

    // {"from", "to", "file"?, "path"?}: copies a TerrainData; with a Terrain named, points it and its collider at the copy.
    public static string Copy(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var from = (string)root["from"];
        var to = (string)root["to"];
        var folder = Path.GetDirectoryName(to).Replace('\\', '/');
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Directory.CreateDirectory(FileUtil.GetPhysicalPath(folder));
            AssetDatabase.Refresh();
        }
        Undo.IncrementCurrentGroup();
        if (!AssetDatabase.CopyAsset(from, to))
            return Error($"Unity couldn't copy {from} to {to}");
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        if (root.TryGetValue("file", out var file) && file != null)
        {
            var done = new List<string>();
            var error = Edit((string)file, (string)root["path"], done, go =>
            {
                var data = AssetDatabase.LoadAssetAtPath<TerrainData>(to);
                var terrain = go.GetComponent<Terrain>();
                Undo.RecordObject(terrain, "terrainwaitress copy");
                terrain.terrainData = data;
                var collider = go.GetComponent<TerrainCollider>();
                if (collider != null)
                {
                    Undo.RecordObject(collider, "terrainwaitress copy");
                    collider.terrainData = data;
                }
                done.Add($"{root["path"]} {(collider != null ? "and its collider " : "")}now use {to}");
            });
            if (error != null)
                return Error($"copied to {to}, but {error}");
            w.Key("repointed").Str(string.Join("; ", done));
        }
        w.End();
        return w.ToString();
    }

    // ---------------------------------------------------------------- Terrain components

    static readonly Dictionary<string, string> s_Aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["pixelError"] = "HeightmapPixelError", ["basemapDistance"] = "SplatMapDistance", ["material"] = "MaterialTemplate", ["materialTemplate"] = "MaterialTemplate",
        ["data"] = "TerrainData", ["terrainData"] = "TerrainData", ["groupingID"] = "GroupingID", ["autoConnect"] = "AllowAutoConnect", ["drawInstanced"] = "DrawInstanced",
        ["enabled"] = "Enabled",
    };

    // {"changes":[{"file","path","component":"terrain"|"collider","field","value"}]}: sets serialized fields of Terrain or
    // TerrainCollider components through Undo and saves the scene or prefab. A scene that isn't open is opened additively and
    // closed again; one with unsaved changes is refused.
    public static string Place(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var done = new List<string>();
        Undo.IncrementCurrentGroup();
        foreach (var group in ((List<object>)root["changes"]).Cast<Dictionary<string, object>>().GroupBy(c => (string)c["file"] + "|" + (string)c["path"]))
        {
            var first = group.First();
            var error = Edit((string)first["file"], (string)first["path"], done, go =>
            {
                foreach (var c in group)
                {
                    var field = (string)c["field"];
                    if (s_Aliases.TryGetValue(field, out var alias))
                        field = alias;
                    var component = (string)c["component"] == "collider" ? (Component)go.GetComponent<TerrainCollider>() : go.GetComponent<Terrain>();
                    if (component == null)
                        throw new Exception($"{go.name} has no {(string)c["component"]}");
                    var so = new SerializedObject(component);
                    var p = so.FindProperty("m_" + field);
                    if (p == null)
                    {
                        var it = so.GetIterator();
                        it.Next(true);
                        do
                            if (string.Equals(it.name, "m_" + field, StringComparison.OrdinalIgnoreCase))
                                p = it.Copy();
                        while (p == null && it.Next(false));
                    }
                    if (p == null)
                        throw new Exception($"{component.GetType().Name} has no field '{c["field"]}'");
                    var text = (string)c["value"];
                    switch (p.propertyType)
                    {
                        case SerializedPropertyType.Float:
                            p.floatValue = float.Parse(text, Inv);
                            break;
                        case SerializedPropertyType.Integer:
                        case SerializedPropertyType.LayerMask:
                            p.intValue = int.Parse(text, Inv);
                            break;
                        case SerializedPropertyType.Boolean:
                            p.boolValue = text is "true" or "1" or "on";
                            break;
                        case SerializedPropertyType.Enum:
                            var index = Array.FindIndex(p.enumNames, n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase));
                            p.enumValueIndex = index >= 0 ? index : int.Parse(text, Inv);
                            break;
                        case SerializedPropertyType.ObjectReference:
                            var asset = text is "none" or "" ? null : AssetDatabase.LoadMainAssetAtPath(text) ?? throw new Exception("no asset " + text);
                            p.objectReferenceValue = asset;
                            if (asset != null && p.objectReferenceValue == null)
                                throw new Exception($"{text} is the wrong kind of asset for {field}");
                            break;
                        default:
                            throw new Exception($"{field} is a {p.propertyType}, which set can't change");
                    }
                    so.ApplyModifiedProperties();
                    if (field == "TerrainData" && component is Terrain && go.GetComponent<TerrainCollider>() is TerrainCollider tc)
                    {
                        Undo.RecordObject(tc, "terrainwaitress set");
                        tc.terrainData = ((Terrain)component).terrainData;
                        done.Add($"{group.First()["path"]}: TerrainCollider follows ({text})");
                    }
                    done.Add($"{c["path"]}: {component.GetType().Name}.{field} = {text}");
                }
            });
            if (error != null)
                return Error(error);
        }
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        w.Key("done").Strings(done);
        w.End();
        return w.ToString();
    }

    // Runs an edit on a GameObject (by hierarchy path) of a scene or prefab and saves the file; returns an error or null.
    static string Edit(string file, string path, List<string> done, Action<GameObject> edit)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return "the Editor is in Play Mode; exit it first";
        if (file.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
        {
            var contents = PrefabUtility.LoadPrefabContents(file);
            try
            {
                var go = Find(new[] { contents }, path, true);
                if (go == null)
                    return $"{file} has no {path}";
                edit(go);
                PrefabUtility.SaveAsPrefabAsset(contents, file);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
            return null;
        }
        var scene = EditorSceneManager.GetSceneByPath(file);
        var opened = false;
        // Unity marks a scene dirty a frame after a Terrain's data is reassigned without changing it, so dirty alone
        // doesn't mean unsaved work: compare a saved copy with the file.
        if (scene.IsValid() && scene.isLoaded && scene.isDirty)
        {
            const string probe = "Temp/terrainwaitress-dirty.unity";
            if (!EditorSceneManager.SaveScene(scene, probe, true) || !File.ReadAllBytes(probe).SequenceEqual(File.ReadAllBytes(FileUtil.GetPhysicalPath(file))))
                return $"{file} is open with unsaved changes; save or discard them first";
        }
        if (!scene.IsValid() || !scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene(file, OpenSceneMode.Additive);
            opened = true;
        }
        try
        {
            var go = Find(scene.GetRootGameObjects(), path, false);
            if (go == null)
                return $"{file} has no {path}";
            edit(go);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        catch (Exception e)
        {
            return e.Message;
        }
        finally
        {
            if (opened)
                EditorSceneManager.CloseScene(scene, true);
        }
        return null;
    }

    static GameObject Find(GameObject[] roots, string path, bool prefab)
    {
        var parts = path.Split('/');
        foreach (var r in roots)
        {
            if (r.name != parts[0])
                continue;
            var t = r.transform;
            for (var k = 1; k < parts.Length && t != null; k++)
                t = t.Cast<Transform>().FirstOrDefault(c => c.name == parts[k]);
            if (t != null)
                return t.gameObject;
        }
        return null;
    }

    // ---------------------------------------------------------------- JSON

    sealed class W
    {
        readonly StringBuilder m_Sb = new StringBuilder();
        readonly Stack<bool> m_First = new Stack<bool>();
        bool m_AfterKey;

        void Sep()
        {
            if (m_AfterKey)
            {
                m_AfterKey = false;
                return;
            }
            if (m_First.Count > 0)
            {
                if (!m_First.Peek())
                    m_Sb.Append(',');
                m_First.Pop();
                m_First.Push(false);
            }
        }

        public W Begin()
        {
            Sep();
            m_Sb.Append('{');
            m_First.Push(true);
            return this;
        }

        public W End()
        {
            m_Sb.Append('}');
            m_First.Pop();
            return this;
        }

        public W Arr()
        {
            Sep();
            m_Sb.Append('[');
            m_First.Push(true);
            return this;
        }

        public W EndArr()
        {
            m_Sb.Append(']');
            m_First.Pop();
            return this;
        }

        public W Key(string k)
        {
            Sep();
            Quote(k);
            m_Sb.Append(':');
            m_AfterKey = true;
            return this;
        }

        public W Str(string s)
        {
            Sep();
            if (s == null)
                m_Sb.Append("null");
            else
                Quote(s);
            return this;
        }

        public W Num(double d)
        {
            Sep();
            if (double.IsNaN(d) || double.IsInfinity(d))
                m_Sb.Append("null");
            else
                m_Sb.Append((float)d == d ? ((float)d).ToString("R", Inv) : d.ToString("R", Inv));
            return this;
        }

        // Keyframe numbers keep infinities (stepped tangents) and NaN as strings.
        public W NumX(float f)
        {
            if (float.IsNaN(f))
                return Str("nan");
            if (float.IsInfinity(f))
                return Str(f > 0 ? "inf" : "-inf");
            Sep();
            m_Sb.Append(f.ToString("R", Inv));
            return this;
        }

        public W Bool(bool b)
        {
            Sep();
            m_Sb.Append(b ? "true" : "false");
            return this;
        }

        public W Vec(Vector3 v)
        {
            Arr();
            Num(v.x).Num(v.y).Num(v.z);
            return EndArr();
        }

        public W Strings(IEnumerable<string> items)
        {
            Arr();
            foreach (var s in items)
                Str(s);
            return EndArr();
        }

        void Quote(string s)
        {
            m_Sb.Append('"');
            foreach (var c in s)
            {
                if (c == '"' || c == '\\')
                    m_Sb.Append('\\').Append(c);
                else if (c < ' ')
                    m_Sb.Append("\\u").Append(((int)c).ToString("x4", Inv));
                else
                    m_Sb.Append(c);
            }
            m_Sb.Append('"');
        }

        public override string ToString() => m_Sb.ToString();
    }

    static object Json(string s, ref int i)
    {
        while (char.IsWhiteSpace(s[i]))
            i++;
        var c = s[i];
        if (c == '{')
        {
            var map = new Dictionary<string, object>();
            i++;
            while (true)
            {
                while (char.IsWhiteSpace(s[i]) || s[i] == ',')
                    i++;
                if (s[i] == '}')
                {
                    i++;
                    return map;
                }
                var key = (string)Json(s, ref i);
                while (char.IsWhiteSpace(s[i]) || s[i] == ':')
                    i++;
                map[key] = Json(s, ref i);
            }
        }
        if (c == '[')
        {
            var list = new List<object>();
            i++;
            while (true)
            {
                while (char.IsWhiteSpace(s[i]) || s[i] == ',')
                    i++;
                if (s[i] == ']')
                {
                    i++;
                    return list;
                }
                list.Add(Json(s, ref i));
            }
        }
        if (c == '"')
        {
            var sb = new StringBuilder();
            i++;
            while (s[i] != '"')
            {
                if (s[i] == '\\')
                {
                    i++;
                    var e = s[i];
                    if (e == 'u')
                    {
                        sb.Append((char)Convert.ToInt32(s.Substring(i + 1, 4), 16));
                        i += 4;
                    }
                    else
                        sb.Append(e == 'n' ? '\n' : e == 't' ? '\t' : e == 'r' ? '\r' : e == 'b' ? '\b' : e == 'f' ? '\f' : e);
                }
                else
                    sb.Append(s[i]);
                i++;
            }
            i++;
            return sb.ToString();
        }
        var start = i;
        while (i < s.Length && ",}] \t\r\n".IndexOf(s[i]) < 0)
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

    static string Error(string message)
    {
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(false).Key("error").Str(message);
        w.End();
        return w.ToString();
    }
}
