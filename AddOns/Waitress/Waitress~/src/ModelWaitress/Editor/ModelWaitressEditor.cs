// The Editor half of modelwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/ModelWaitress/, and
// runs it through `unity command run_script`, so nothing is imported into the project. Export writes what the importer
// is set to and what it produced; Edit changes importer settings through the ModelImporter API and reimports.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Rendering;

public static class ModelWaitressEditor
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    static readonly HashSet<string> s_SkipSettings = new HashSet<string>
    {
        "name", "hideFlags", "clipAnimations", "humanDescription", "userData", "assetBundleName", "assetBundleVariant",
    };

    static readonly string[] s_ModelCallbacks =
    {
        "OnPreprocessModel", "OnPostprocessModel", "OnPostprocessMeshHierarchy", "OnPreprocessAnimation", "OnPostprocessAnimation",
        "OnAssignMaterialModel", "OnPreprocessMaterialDescription", "OnPostprocessMaterial", "OnPostprocessGameObjectWithUserProperties",
        "OnPostprocessGameObjectWithAnimatedUserProperties", "OnPreprocessCameraDescription", "OnPreprocessLightDescription",
    };

    // ---------------------------------------------------------------- export

    // {"paths":[...],"out":"<dir>"}: writes <dir>/Models/<guid>.json per model and <dir>/project.json.
    public static string Export(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var outDir = (string)root["out"];
        var paths = ((List<object>)root["paths"]).Cast<string>().ToList();
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return Error("the Editor is compiling or importing; try again when it finishes");
        var models = Path.Combine(outDir, "Models");
        Directory.CreateDirectory(models);
        var done = new List<string>();
        var failed = new List<string>();
        foreach (var path in paths)
        {
            try
            {
                if (!File.Exists(FileUtil.GetPhysicalPath(path)))
                    throw new Exception("no such file");
                // Brings the import up to date when the file changed while the Editor wasn't looking; no-op otherwise.
                AssetDatabase.ImportAsset(path);
                var guid = AssetDatabase.AssetPathToGUID(path);
                File.WriteAllText(Path.Combine(models, guid + ".json"), ExportModel(path), new UTF8Encoding(false));
                done.Add(path);
            }
            catch (Exception e)
            {
                failed.Add(path + ": " + e.Message);
            }
        }
        File.WriteAllText(Path.Combine(outDir, "project.json"), ExportProject(paths.Select(p => AssetImporter.GetAtPath(p) as ModelImporter).FirstOrDefault(m => m != null)), new UTF8Encoding(false));
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

    static string ExportModel(string path)
    {
        var imp = AssetImporter.GetAtPath(path) as ModelImporter;
        if (imp == null)
            throw new Exception("not imported by a ModelImporter (" + (AssetImporter.GetAtPath(path)?.GetType().Name ?? "no importer") + ")");
        var w = new W();
        w.Begin();
        w.Key("v").Num(1);
        w.Key("path").Str(path);
        w.Key("guid").Str(AssetDatabase.AssetPathToGUID(path));
        w.Key("stamp").Str(Stamp(path));
        w.Key("unity").Str(Application.unityVersion);
        w.Key("importer").Str(imp.GetType().Name);
        WriteArtifact(w, path);
        WriteSettings(w, imp);
        WriteFileInfo(w, imp);
        WriteClipSettings(w, imp);
        WriteHuman(w, imp);
        WriteMaterials(w, imp);
        WriteMessages(w, imp, path);
        WriteResult(w, path);
        WriteDeps(w, imp, path);
        w.End();
        return w.ToString();
    }

    // Other files the record's answers depend on, stamped like the model, so the CLI notices when one changes or goes away.
    static void WriteDeps(W w, ModelImporter imp, string path)
    {
        var deps = new List<string>();
        foreach (var target in imp.GetExternalObjectMap().Values)
            if (target != null)
                deps.Add(AssetDatabase.GetAssetPath(target));
        if (imp.sourceAvatar != null)
            deps.Add(AssetDatabase.GetAssetPath(imp.sourceAvatar));
        foreach (var c in imp.clipAnimations)
            if (c.maskSource != null)
                deps.Add(AssetDatabase.GetAssetPath(c.maskSource));
        var physical = FileUtil.GetPhysicalPath(path);
        if (path.EndsWith(".obj", StringComparison.OrdinalIgnoreCase))
            foreach (var line in File.ReadLines(physical).Take(5000))
                if (line.StartsWith("mtllib ", StringComparison.Ordinal))
                    deps.Add(Path.Combine(Path.GetDirectoryName(path), line.Substring(7).Trim()).Replace(Path.DirectorySeparatorChar, '/'));
        w.Key("deps").Arr();
        foreach (var d in deps.Where(d => !string.IsNullOrEmpty(d) && d != path).Distinct())
        {
            var full = Path.GetFullPath(FileUtil.GetPhysicalPath(d));
            w.Arr();
            w.Str(full).Str(File.Exists(full) ? Stamp(d) : "missing");
            w.EndArr();
        }
        w.EndArr();
    }

    static void WriteArtifact(W w, string path)
    {
        var method = typeof(AssetDatabase).GetMethod("GetArtifactInfos", Any, null, new[] { typeof(GUID) }, null);
        if (method == null)
            return;
        var infos = (Array)method.Invoke(null, new object[] { AssetDatabase.GUIDFromAssetPath(path) });
        foreach (var info in infos)
        {
            var t = info.GetType();
            if (!(bool)t.GetProperty("isCurrentArtifact", Any).GetValue(info))
                continue;
            w.Key("artifact").Str((string)t.GetProperty("artifactID", Any).GetValue(info));
            var stats = t.GetProperty("importStats", Any).GetValue(info);
            if (stats != null)
            {
                var st = stats.GetType();
                w.Key("importMs").Num(Convert.ToDouble(st.GetProperty("importTimeMicroseconds", Any).GetValue(stats)) / 1000.0);
                w.Key("importedAt").Num(Convert.ToDouble(st.GetProperty("importedTimestamp", Any).GetValue(stats)));
            }
            var deps = (IDictionary)t.GetProperty("dependencies", Any).GetValue(info);
            w.Key("postprocessorHash").Begin();
            foreach (DictionaryEntry d in deps)
            {
                var key = (string)d.Key;
                const string prefix = "ImporterRegistry/PostProcessorVersionHash/";
                if (!key.StartsWith(prefix, StringComparison.Ordinal) || d.Value == null)
                    continue;
                var value = d.Value.GetType().GetField("m_Value", Any)?.GetValue(d.Value);
                w.Key(key.Substring(prefix.Length)).Str(Convert.ToString(value, Inv));
            }
            w.End();
            return;
        }
    }

    static IEnumerable<PropertyInfo> SettingProperties(Type type, HashSet<string> skip) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.SetMethod.IsPublic && p.GetIndexParameters().Length == 0
                        && p.GetCustomAttribute<ObsoleteAttribute>() == null && !skip.Contains(p.Name))
            .OrderBy(p => p.Name, StringComparer.Ordinal);

    static void WriteSettings(W w, ModelImporter imp)
    {
        var reset = UnityEngine.Object.Instantiate(imp);
        try
        {
            Unsupported.SmartReset(reset);
            w.Key("settings").Arr();
            foreach (var p in SettingProperties(typeof(ModelImporter), s_SkipSettings))
            {
                w.Arr();
                w.Str(p.Name).Str(TypeName(p.PropertyType)).Str(Format(p.GetValue(imp))).Str(Format(p.GetValue(reset)));
                w.EndArr();
            }
            w.EndArr();
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(reset);
        }
    }

    static void WriteFileInfo(W w, ModelImporter imp)
    {
        var so = new SerializedObject(imp);
        w.Key("file").Begin();
        w.Key("fileScale").Num(imp.fileScale);
        var unit = so.FindProperty("m_FileScaleUnit");
        if (unit != null)
            w.Key("unit").Str(unit.stringValue);
        var factor = so.FindProperty("m_FileScaleFactor");
        if (factor != null)
            w.Key("unitFactor").Num(factor.floatValue);
        foreach (var name in new[] { "m_HasExtraRoot", "m_HasEmbeddedTextures", "m_ContainsAnimation" })
        {
            var p = so.FindProperty(name);
            if (p != null)
                w.Key(char.ToLowerInvariant(name[2]) + name.Substring(3)).Bool(p.boolValue);
        }
        w.Key("transformPaths").Strings(TransformPaths(imp));
        w.Key("referencedClips").Strings(imp.referencedClips.Select(g => AssetDatabase.GUIDToAssetPath(g) is var p && p.Length > 0 ? p : g));
        w.Key("takes").Arr();
        foreach (var t in imp.importedTakeInfos)
        {
            w.Begin();
            w.Key("name").Str(t.name).Key("clip").Str(t.defaultClipName).Key("start").Num(t.startTime).Key("stop").Num(t.stopTime).Key("fps").Num(t.sampleRate);
            w.End();
        }
        w.EndArr();
        w.End();
    }

    static readonly HashSet<string> s_SkipClip = new HashSet<string> { "events", "curves", "maskSource", "takeName", "name" };

    static void WriteClipSettings(W w, ModelImporter imp)
    {
        var custom = imp.clipAnimations;
        var defaults = imp.defaultClipAnimations;
        w.Key("clipsCustom").Bool(custom.Length > 0);
        w.Key("clips").Arr();
        foreach (var c in custom.Length > 0 ? custom : defaults)
        {
            var d = defaults.FirstOrDefault(x => x.takeName == c.takeName) ?? defaults.FirstOrDefault() ?? new ModelImporterClipAnimation();
            w.Begin();
            w.Key("name").Str(c.name).Key("take").Str(c.takeName);
            w.Key("settings").Arr();
            foreach (var p in SettingProperties(typeof(ModelImporterClipAnimation), s_SkipClip))
            {
                w.Arr();
                w.Str(p.Name).Str(TypeName(p.PropertyType)).Str(Format(p.GetValue(c))).Str(Format(p.GetValue(d)));
                w.EndArr();
            }
            w.EndArr();
            w.Key("mask").Str(c.maskSource != null ? AssetDatabase.GetAssetPath(c.maskSource) : null);
            w.Key("events").Arr();
            foreach (var e in c.events ?? new AnimationEvent[0])
            {
                w.Arr();
                w.Num(e.time).Str(e.functionName).Str(e.stringParameter).Num(e.floatParameter).Num(e.intParameter).Str(e.objectReferenceParameter != null ? AssetDatabase.GetAssetPath(e.objectReferenceParameter) : null);
                w.EndArr();
            }
            w.EndArr();
            w.Key("curves").Strings((c.curves ?? new ClipAnimationInfoCurve[0]).Select(x => x.name));
            w.End();
        }
        w.EndArr();
    }

    static void WriteHuman(W w, ModelImporter imp)
    {
        var h = imp.humanDescription;
        w.Key("human").Begin();
        w.Key("map").Arr();
        foreach (var b in h.human ?? new HumanBone[0])
        {
            w.Arr();
            w.Str(b.humanName).Str(b.boneName);
            w.EndArr();
        }
        w.EndArr();
        w.Key("skeleton").Num(h.skeleton?.Length ?? 0);
        w.Key("upperArmTwist").Num(h.upperArmTwist).Key("lowerArmTwist").Num(h.lowerArmTwist).Key("upperLegTwist").Num(h.upperLegTwist)
         .Key("lowerLegTwist").Num(h.lowerLegTwist).Key("armStretch").Num(h.armStretch).Key("legStretch").Num(h.legStretch)
         .Key("feetSpacing").Num(h.feetSpacing).Key("hasTranslationDoF").Bool(h.hasTranslationDoF);
        var root = new SerializedObject(imp).FindProperty("m_HumanDescription.m_RootMotionBoneName");
        if (root != null)
            w.Key("rootMotionBone").Str(root.stringValue);
        w.End();
    }

    static void WriteMaterials(W w, ModelImporter imp)
    {
        w.Key("remaps").Arr();
        string meta = null;
        foreach (var pair in imp.GetExternalObjectMap())
        {
            w.Arr();
            var target = ObjectPath(pair.Value);
            // A remap to a deleted asset loads as null; the .meta still names the GUID it pointed at.
            if (pair.Value == null)
            {
                meta ??= File.ReadAllText(FileUtil.GetPhysicalPath(imp.assetPath) + ".meta");
                var guid = RemapGuid(meta, pair.Key.name);
                if (guid != null && guid != "0000000000000000" && !guid.All(c => c == '0'))
                    target = "missing:" + guid;
            }
            w.Str(pair.Key.type?.Name).Str(pair.Key.name).Str(target);
            w.EndArr();
        }
        w.EndArr();
        var sources = typeof(ModelImporter).GetProperty("sourceMaterials", Any)?.GetValue(imp) as AssetImporter.SourceAssetIdentifier[];
        w.Key("sourceMaterials").Strings((sources ?? new AssetImporter.SourceAssetIdentifier[0]).Select(s => s.name));
    }

    static string RemapGuid(string meta, string name)
    {
        var lines = meta.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim() != "name: " + name)
                continue;
            for (var j = i + 1; j < Math.Min(lines.Length, i + 4); j++)
            {
                var at = lines[j].IndexOf("guid: ", StringComparison.Ordinal);
                if (lines[j].TrimStart().StartsWith("second:", StringComparison.Ordinal) && at >= 0 && at + 38 <= lines[j].Length)
                    return lines[j].Substring(at + 6, 32);
            }
        }
        return null;
    }

    static void WriteMessages(W w, ModelImporter imp, string path)
    {
        w.Key("messages").Arr();
        var log = AssetImporter.GetImportLog(path);
        if (log != null)
        {
            foreach (var e in log.logEntries)
            {
                w.Arr();
                w.Str((e.flags & UnityEditor.AssetImporters.ImportLogFlags.Error) != 0 ? "error" : "warning").Str(e.message?.Trim());
                w.EndArr();
            }
        }
        var so = new SerializedObject(imp);
        foreach (var (name, level) in new[] { ("m_AnimationImportErrors", "error"), ("m_AnimationImportWarnings", "warning"), ("m_AnimationRetargetingWarnings", "retargeting") })
        {
            var p = so.FindProperty(name);
            if (p == null || string.IsNullOrWhiteSpace(p.stringValue))
                continue;
            foreach (var line in p.stringValue.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0))
            {
                w.Arr();
                w.Str(level).Str(line);
                w.EndArr();
            }
        }
        w.EndArr();
    }

    static void WriteResult(W w, string path)
    {
        var all = AssetDatabase.LoadAllAssetsAtPath(path);
        var meshes = all.OfType<Mesh>().ToList();
        var main = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
        var nodes = new List<Transform>();
        if (main != null)
            Collect(main.transform, nodes);
        var index = new Dictionary<Transform, int>();
        for (var n = 0; n < nodes.Count; n++)
            index[nodes[n]] = n;

        w.Key("nodes").Arr();
        foreach (var t in nodes)
        {
            w.Begin();
            w.Key("name").Str(t.name);
            w.Key("parent").Num(t.parent != null && index.TryGetValue(t.parent, out var pi) ? pi : -1);
            if (!t.gameObject.activeSelf)
                w.Key("inactive").Bool(true);
            w.Key("p").Vec(t.localPosition).Key("r").Vec(t.localEulerAngles).Key("s").Vec(t.localScale);
            w.Key("c").Arr();
            foreach (var c in t.GetComponents<Component>())
            {
                if (c is Transform)
                    continue;
                w.Arr();
                w.Str(c == null ? "Missing" : c.GetType().Name);
                Describe(w, c, meshes, index);
                w.EndArr();
            }
            w.EndArr();
            w.End();
        }
        w.EndArr();

        w.Key("meshes").Arr();
        foreach (var m in meshes)
            WriteMesh(w, m);
        w.EndArr();

        w.Key("avatars").Arr();
        foreach (var a in all.OfType<Avatar>())
        {
            w.Begin();
            w.Key("name").Str(a.name).Key("valid").Bool(a.isValid).Key("human").Bool(a.isHuman);
            w.End();
        }
        w.EndArr();

        w.Key("anims").Arr();
        foreach (var clip in all.OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview__", StringComparison.Ordinal)))
            WriteClip(w, clip);
        w.EndArr();

        w.Key("subassets").Arr();
        foreach (var o in all)
        {
            if (o == null || o is Transform || o is GameObject || o is Component)
                continue;
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o, out string _, out long id))
                id = 0;
            w.Arr();
            w.Str(o.GetType().Name).Str(o.name).Str(id.ToString(Inv));
            w.EndArr();
        }
        w.EndArr();
    }

    static void Collect(Transform t, List<Transform> nodes)
    {
        nodes.Add(t);
        foreach (Transform c in t)
            Collect(c, nodes);
    }

    static void Describe(W w, Component c, List<Mesh> meshes, Dictionary<Transform, int> index)
    {
        switch (c)
        {
            case MeshFilter mf:
                w.Num(meshes.IndexOf(mf.sharedMesh));
                break;
            case SkinnedMeshRenderer smr:
                w.Num(meshes.IndexOf(smr.sharedMesh));
                w.Strings(smr.sharedMaterials.Select(ObjectPath));
                w.Num(smr.rootBone != null && index.TryGetValue(smr.rootBone, out var r) ? r : -1);
                w.Arr();
                foreach (var b in smr.bones)
                    w.Num(b != null && index.TryGetValue(b, out var bi) ? bi : -1);
                w.EndArr();
                w.Str(smr.quality.ToString()).Bool(smr.updateWhenOffscreen);
                break;
            case Renderer mr:
                w.Strings(mr.sharedMaterials.Select(ObjectPath));
                break;
            case Animator an:
                w.Str(an.avatar != null ? an.avatar.name : null);
                break;
            case Animation la:
                w.Strings(AnimationUtility.GetAnimationClips(la.gameObject).Where(x => x != null).Select(x => x.name));
                break;
            case LODGroup lg:
                w.Arr();
                foreach (var lod in lg.GetLODs())
                {
                    w.Arr();
                    w.Num(lod.screenRelativeTransitionHeight);
                    foreach (var rr in lod.renderers)
                        w.Num(rr != null && index.TryGetValue(rr.transform, out var li) ? li : -1);
                    w.EndArr();
                }
                w.EndArr();
                break;
            case MeshCollider mc:
                w.Num(meshes.IndexOf(mc.sharedMesh));
                break;
            case Light l:
                w.Str(l.type.ToString());
                break;
            case Camera cam:
                w.Str(cam.orthographic ? "orthographic" : "perspective");
                break;
        }
    }

    static void WriteMesh(W w, Mesh m)
    {
        w.Begin();
        w.Key("name").Str(m.name);
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m, out string _, out long id);
        w.Key("id").Str(id.ToString(Inv));
        w.Key("verts").Num(m.vertexCount);
        w.Key("readable").Bool(m.isReadable);
        w.Key("index").Str(m.indexFormat.ToString());
        long tris = 0;
        w.Key("sub").Arr();
        for (var s = 0; s < m.subMeshCount; s++)
        {
            var d = m.GetSubMesh(s);
            if (d.topology == MeshTopology.Triangles)
                tris += d.indexCount / 3;
            w.Arr();
            w.Str(d.topology.ToString()).Num(d.indexCount).Num(d.firstVertex).Num(d.vertexCount).Num(d.baseVertex);
            w.EndArr();
        }
        w.EndArr();
        w.Key("tris").Num(tris);
        w.Key("attrs").Arr();
        foreach (var a in m.GetVertexAttributes())
        {
            w.Arr();
            w.Str(a.attribute.ToString()).Str(a.format.ToString()).Num(a.dimension).Num(a.stream);
            w.EndArr();
        }
        w.EndArr();
        w.Key("bounds").Arr().Vec(m.bounds.center).Vec(m.bounds.extents).EndArr();
        w.Key("shapes").Arr();
        for (var s = 0; s < m.blendShapeCount; s++)
        {
            w.Arr();
            w.Str(m.GetBlendShapeName(s)).Num(m.GetBlendShapeFrameCount(s));
            w.EndArr();
        }
        w.EndArr();
        w.Key("bindposes").Num(m.bindposeCount);
        try
        {
            var bpv = m.GetBonesPerVertex();
            var max = 0;
            for (var v = 0; v < bpv.Length; v++)
                max = Math.Max(max, bpv[v]);
            w.Key("maxBonesPerVertex").Num(max);
        }
        catch (Exception)
        {
        }
        var lods = typeof(Mesh).GetProperty("lodCount", BindingFlags.Public | BindingFlags.Instance);
        if (lods != null)
            w.Key("lods").Num(Convert.ToDouble(lods.GetValue(m)));
        w.End();
    }

    static void WriteClip(W w, AnimationClip clip)
    {
        w.Begin();
        w.Key("name").Str(clip.name);
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(clip, out string _, out long id);
        w.Key("id").Str(id.ToString(Inv));
        w.Key("length").Num(clip.length).Key("fps").Num(clip.frameRate).Key("loop").Bool(clip.isLooping).Key("legacy").Bool(clip.legacy)
         .Key("wrap").Str(clip.wrapMode.ToString()).Key("humanMotion").Bool(clip.isHumanMotion).Key("rootCurves").Bool(clip.hasRootCurves)
         .Key("motionCurves").Bool(clip.hasMotionCurves).Key("genericRoot").Bool(clip.hasGenericRootTransform);
        w.Key("averageSpeed").Vec(clip.averageSpeed).Key("averageAngularSpeed").Num(clip.averageAngularSpeed);
        w.Key("events").Num(clip.events.Length);
        var bindings = AnimationUtility.GetCurveBindings(clip);
        var objects = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        w.Key("curves").Num(bindings.Length + objects.Length);
        w.Key("paths").Arr();
        foreach (var g in bindings.Concat(objects).GroupBy(b => b.path).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            w.Arr();
            w.Str(g.Key);
            foreach (var prop in g.Select(b => (b.type == typeof(Transform) || b.type == typeof(Animator) ? "" : b.type.Name + ".") + Group(b.propertyName)).Distinct())
                w.Str(prop);
            w.EndArr();
        }
        w.EndArr();
        w.End();
    }

    static string Group(string property)
    {
        var dot = property.LastIndexOf('.');
        if (dot > 0 && property.Length - dot == 2 && "xyzwrgba".IndexOf(property[dot + 1]) >= 0)
            return property.Substring(0, dot);
        return property;
    }

    static string ExportProject(ModelImporter sample)
    {
        var w = new W();
        w.Begin();
        w.Key("unity").Str(Application.unityVersion);
        w.Key("postprocessors").Arr();
        foreach (var t in TypeCache.GetTypesDerivedFrom<AssetPostprocessor>().Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition).OrderBy(t => t.FullName, StringComparer.Ordinal))
        {
            var callbacks = s_ModelCallbacks.Where(n => t.GetMethods(Any).Any(m => m.Name == n && m.DeclaringType != typeof(AssetPostprocessor))).ToList();
            if (callbacks.Count == 0)
                continue;
            w.Begin();
            w.Key("type").Str(t.FullName).Key("assembly").Str(t.Assembly.GetName().Name);
            w.Key("callbacks").Strings(callbacks);
            try
            {
                var instance = (AssetPostprocessor)Activator.CreateInstance(t, true);
                w.Key("order").Num(instance.GetPostprocessOrder()).Key("version").Num(instance.GetVersion());
            }
            catch (Exception)
            {
            }
            var script = MonoScriptOf(t);
            if (script != null)
                w.Key("script").Str(script);
            w.End();
        }
        w.EndArr();
        w.Key("presets").Arr();
        foreach (var d in sample != null ? Preset.GetDefaultPresetsForType(new PresetType(sample)) : new DefaultPreset[0])
        {
            w.Arr();
            w.Str(d.filter).Str(d.preset != null ? AssetDatabase.GetAssetPath(d.preset) : null).Bool(d.enabled);
            w.EndArr();
        }
        w.EndArr();
        w.Key("enums").Begin();
        var types = SettingProperties(typeof(ModelImporter), s_SkipSettings).Concat(SettingProperties(typeof(ModelImporterClipAnimation), s_SkipClip))
                    .Select(p => p.PropertyType).Where(t => t.IsEnum).Distinct().OrderBy(t => t.Name, StringComparer.Ordinal);
        foreach (var t in types)
        {
            w.Key(t.Name).Arr();
            foreach (var n in EnumNames(t))
                w.Str(n);
            w.EndArr();
        }
        w.End();
        w.End();
        return w.ToString();
    }

    static Dictionary<Type, string> s_Scripts;

    static string MonoScriptOf(Type t)
    {
        if (s_Scripts == null)
        {
            s_Scripts = new Dictionary<Type, string>();
            foreach (var guid in AssetDatabase.FindAssets("t:MonoScript"))
            {
                var p = AssetDatabase.GUIDToAssetPath(guid);
                if (!p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;
                var text = File.Exists(p) ? File.ReadAllText(p) : "";
                if (!text.Contains("AssetPostprocessor"))
                    continue;
                var ms = AssetDatabase.LoadAssetAtPath<MonoScript>(p);
                var c = ms != null ? ms.GetClass() : null;
                if (c != null)
                    s_Scripts[c] = p;
            }
        }
        return s_Scripts.TryGetValue(t, out var path) ? path : null;
    }

    // ---------------------------------------------------------------- edit

    sealed class EditFailed : Exception
    {
        public EditFailed(string message) : base(message) { }
    }

    // {"out":"<dir>","ops":[{"path":..,"key":..,"op":"=|+=|-=","value":..}]}: validates every op, applies, reimports, exports.
    public static string Edit(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var outDir = (string)root["out"];
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return Error("the Editor is in Play Mode; leave it before changing import settings");
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return Error("the Editor is compiling or importing; try again when it finishes");
        var ops = ((List<object>)root["ops"]).Cast<Dictionary<string, object>>().ToList();
        var dryRun = root.TryGetValue("dryRun", out var dr) && dr is bool b && b;
        var byPath = new List<(string path, ModelImporter imp, Working work, List<string> notes)>();
        var errors = new List<string>();
        foreach (var group in ops.GroupBy(o => (string)o["path"]))
        {
            var path = group.Key;
            var imp = AssetImporter.GetAtPath(path) as ModelImporter;
            if (imp == null)
            {
                errors.Add(path + " is not imported by a ModelImporter");
                continue;
            }
            try
            {
                CheckInspectors(imp, path);
            }
            catch (EditFailed e)
            {
                errors.Add(e.Message);
                continue;
            }
            var work = new Working(imp);
            work.ChangesRig = group.Any(o => (string)o["key"] == "animationType" || (string)o["key"] == "avatarSetup" || (string)o["key"] == "sourceAvatar");
            var notes = new List<string>();
            // Every op is checked, so one reply lists every mistake.
            var prefix = ops.Select(o => (string)o["path"]).Distinct().Count() > 1 ? System.IO.Path.GetFileName(path) + ": " : "";
            foreach (var dup in group.Where(o => (string)o["op"] == "=").GroupBy(o => (string)o["key"]).Where(g => g.Select(o => (string)o["value"]).Distinct().Count() > 1))
                errors.Add($"{prefix}{dup.Key} is set to different values in one call");
            foreach (var o in group)
            {
                var key = (string)o["key"];
                try
                {
                    Plan(work, key, (string)o["op"], o.TryGetValue("value", out var v) ? (string)v : "", notes);
                }
                catch (EditFailed e)
                {
                    errors.Add(prefix + e.Message);
                }
                catch (Exception e)
                {
                    errors.Add($"{prefix}{key}: {e.GetType().Name}: {e.Message}");
                }
            }
            foreach (var problem in work.Check())
                errors.Add(prefix + problem);
            byPath.Add((path, imp, work, notes));
        }
        errors = errors.Distinct().ToList();
        if (errors.Count > 0)
            return Error((errors.Count > 1 ? $"{errors.Count} problems, nothing changed:\n  " : "") + string.Join("\n  ", errors));
        if (dryRun)
        {
            var dw = new W();
            dw.Begin();
            dw.Key("ok").Bool(true).Key("dryRun").Bool(true);
            dw.Key("done").Strings(new string[0]);
            dw.End();
            return dw.ToString();
        }

        var done = new List<string>();
        var failed = new List<string>();
        foreach (var (path, imp, work, notes) in byPath)
        {
            try
            {
                foreach (var a in work.Commit)
                    a();
                if (work.RebuildSkeleton)
                    RebuildSkeleton(path, imp);
                else
                    imp.SaveAndReimport();
                var guid = AssetDatabase.AssetPathToGUID(path);
                Directory.CreateDirectory(Path.Combine(outDir, "Models"));
                File.WriteAllText(Path.Combine(outDir, "Models", guid + ".json"), ExportModel(path), new UTF8Encoding(false));
                done.Add(path);
            }
            catch (Exception e)
            {
                failed.Add(path + ": " + e.Message);
            }
        }
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(failed.Count == 0);
        if (failed.Count > 0)
            w.Key("error").Str(string.Join("\n", failed));
        w.Key("done").Strings(done);
        w.End();
        return w.ToString();
    }

    // An empty skeleton makes the importer take a fresh one from the file, but it also re-runs the automatic mapping, so the
    // mapping is put back in a second import.
    static void RebuildSkeleton(string path, ModelImporter imp)
    {
        var keep = imp.humanDescription;
        var h = keep;
        h.skeleton = new SkeletonBone[0];
        imp.humanDescription = h;
        imp.SaveAndReimport();
        imp = (ModelImporter)AssetImporter.GetAtPath(path);
        keep.skeleton = imp.humanDescription.skeleton;
        imp.humanDescription = keep;
        imp.SaveAndReimport();
    }

    // An importer Inspector holding unapplied changes would ask about them later; refuse rather than race it.
    static void CheckInspectors(ModelImporter imp, string path)
    {
        var editorType = Type.GetType("UnityEditor.AssetImporters.AssetImporterEditor, UnityEditor");
        if (editorType == null)
            return;
        var hasModified = editorType.GetMethod("HasModified", Any, null, Type.EmptyTypes, null);
        foreach (var e in Resources.FindObjectsOfTypeAll(editorType).Cast<Editor>())
        {
            if (e.targets == null || !e.targets.Any(t => t == imp))
                continue;
            if (hasModified != null && (bool)hasModified.Invoke(e, null))
                throw new EditFailed(path + " has unapplied changes in an Inspector; apply or revert them in the Editor first");
        }
    }

    // Pending changes to one importer: plain properties are set in order at commit; clips and the human description
    // are edited as copies and assigned once.
    sealed class Working
    {
        public readonly ModelImporter Imp;
        public readonly List<Action> Commit = new List<Action>();
        public bool RebuildSkeleton;
        public bool ChangesRig;
        List<ModelImporterClipAnimation> m_Clips;
        HumanDescription? m_Human;

        /// <summary>Problems only visible once every op is planned, such as a clip whose range ends before it starts.</summary>
        public IEnumerable<string> Check()
        {
            if (m_Clips == null)
                yield break;
            foreach (var c in m_Clips)
            {
                if (c.firstFrame < 0)
                    yield return $"clip '{c.name}' starts at frame {c.firstFrame}; frames can't be negative";
                if (c.lastFrame < c.firstFrame)
                    yield return $"clip '{c.name}' ends (frame {c.lastFrame}) before it starts (frame {c.firstFrame})";
            }
            foreach (var name in m_Clips.GroupBy(c => c.name).Where(g => g.Count() > 1).Select(g => g.Key))
                yield return $"two clips would be named '{name}'";
        }

        public Working(ModelImporter imp)
        {
            Imp = imp;
        }

        public List<ModelImporterClipAnimation> Clips
        {
            get
            {
                if (m_Clips == null)
                {
                    var custom = Imp.clipAnimations;
                    m_Clips = (custom.Length > 0 ? custom : Imp.defaultClipAnimations).ToList();
                    Commit.Add(() => Imp.clipAnimations = m_Clips.ToArray());
                }
                return m_Clips;
            }
        }

        public HumanDescription Human
        {
            get
            {
                if (m_Human == null)
                {
                    m_Human = Imp.humanDescription;
                    Commit.Add(() => Imp.humanDescription = m_Human.Value);
                }
                return m_Human.Value;
            }
            set => m_Human = value;
        }
    }

    static void Plan(Working work, string key, string op, string value, List<string> notes)
    {
        var imp = work.Imp;
        if (key.StartsWith("clips[", StringComparison.Ordinal))
        {
            var close = key.LastIndexOf("].", StringComparison.Ordinal);
            if (close < 6)
                throw new EditFailed($"'{key}': expected clips[Name].property");
            var name = key.Substring(6, close - 6);
            var prop = key.Substring(close + 2);
            var clip = work.Clips.FirstOrDefault(c => c.name == name)
                       ?? throw new EditFailed($"no clip '{name}'; clips: {string.Join(", ", work.Clips.Select(c => c.name))}");
            PlanClip(work, clip, prop, op, value);
            return;
        }
        if (key == "clips")
        {
            if (op == "+=")
            {
                if (work.Clips.Any(c => c.name == value))
                    throw new EditFailed($"a clip named '{value}' already exists");
                var take = imp.defaultClipAnimations.FirstOrDefault() ?? throw new EditFailed("the model has no animation takes to make a clip from");
                take.name = value;
                work.Clips.Add(take);
            }
            else if (op == "-=")
            {
                if (work.Clips.RemoveAll(c => c.name == value) == 0)
                    throw new EditFailed($"no clip '{value}'; clips: {string.Join(", ", work.Clips.Select(c => c.name))}");
            }
            else if (op == "=" && (value == "" || value.Equals("default", StringComparison.OrdinalIgnoreCase)))
            {
                // An empty clip list is what makes Unity fall back to one clip per take.
                work.Clips.Clear();
            }
            else
                throw new EditFailed("clips takes +=Name, -=Name, or =default (back to one clip per take)");
            return;
        }
        if (key.StartsWith("human[", StringComparison.Ordinal))
        {
            RequireHuman(work);
            var close = key.LastIndexOf(']');
            var humanName = key.Substring(6, close < 0 ? key.Length - 6 : close - 6);
            var canonical = HumanTrait.BoneName.FirstOrDefault(n => Same(n, humanName))
                            ?? throw new EditFailed($"'{humanName}' is not a humanoid bone; {Suggest(humanName, HumanTrait.BoneName, "names are Unity's HumanBodyBones, e.g. Hips, LeftUpperArm, Left Thumb Proximal")}");
            var h = work.Human;
            var list = (h.human ?? new HumanBone[0]).Where(b => b.humanName != canonical).ToList();
            if (value.Length > 0)
            {
                var full = TransformPath(imp, value);
                var leaf = full.Substring(full.LastIndexOf('/') + 1);
                var taken = list.FirstOrDefault(b => b.boneName == leaf);
                if (taken.boneName != null)
                    throw new EditFailed($"{leaf} is already mapped to {taken.humanName}; unmap it first in the same call (\"human[{taken.humanName}]=\")");
                var bone = new HumanBone { humanName = canonical, boneName = leaf };
                bone.limit.useDefaultValues = true;
                list.Add(bone);
            }
            else if (HumanTrait.RequiredBone(Array.IndexOf(HumanTrait.BoneName, canonical)))
                throw new EditFailed($"{canonical} is required for a humanoid avatar; map it to another transform instead of unmapping it");
            h.human = list.ToArray();
            work.Human = h;
            return;
        }
        if (key.StartsWith("human.", StringComparison.Ordinal))
        {
            RequireHuman(work);
            var field = key.Substring(6);
            if (field == "skeleton")
            {
                if (!Same(value, "rebuild"))
                    throw new EditFailed("human.skeleton takes only =rebuild (a fresh skeleton pose from the file, keeping the bone mapping)");
                work.RebuildSkeleton = true;
                return;
            }
            // HumanDescription exposes some of these as fields and some as properties, depending on the Unity version.
            var f = typeof(HumanDescription).GetField(field, BindingFlags.Public | BindingFlags.Instance);
            var hp = f == null ? typeof(HumanDescription).GetProperty(field, BindingFlags.Public | BindingFlags.Instance) : null;
            var type = f?.FieldType ?? (hp != null && hp.CanWrite ? hp.PropertyType : null);
            if (type != typeof(float) && type != typeof(bool))
                throw new EditFailed($"'{field}' is not a humanoid setting; settings: upperArmTwist, lowerArmTwist, upperLegTwist, lowerLegTwist, armStretch, legStretch, feetSpacing, hasTranslationDoF");
            object boxed = work.Human;
            if (f != null)
                f.SetValue(boxed, Parse(type, value, key));
            else
                hp.SetValue(boxed, Parse(type, value, key));
            work.Human = (HumanDescription)boxed;
            return;
        }
        if (key.StartsWith("materials[", StringComparison.Ordinal))
        {
            var close = key.LastIndexOf(']');
            var name = key.Substring(10, close < 0 ? key.Length - 10 : close - 10);
            var sources = (typeof(ModelImporter).GetProperty("sourceMaterials", Any)?.GetValue(imp) as AssetImporter.SourceAssetIdentifier[]) ?? new AssetImporter.SourceAssetIdentifier[0];
            if (!sources.Any(s => s.name == name) && !imp.GetExternalObjectMap().Keys.Any(k => k.name == name))
                throw new EditFailed($"the model has no material '{name}'; materials: {string.Join(", ", sources.Select(s => s.name))}");
            var id = new AssetImporter.SourceAssetIdentifier(typeof(Material), name);
            if (value.Length == 0 || value == "None")
                work.Commit.Add(() => imp.RemoveRemap(id));
            else
            {
                var (matPath, matName) = SplitSubAsset(value);
                if (matPath == imp.assetPath)
                    throw new EditFailed($"materials[{name}] can't remap to the model's own materials; that leaves the slot empty");
                var candidates = AssetDatabase.LoadAllAssetsAtPath(matPath).OfType<Material>().Where(m => matName == null || m.name == matName).ToList();
                if (candidates.Count > 1)
                    throw new EditFailed($"{matPath} holds {candidates.Count} materials; name one with #: {string.Join(", ", candidates.Select(m => matPath + "#" + m.name))}");
                var mat = candidates.FirstOrDefault() ?? throw new EditFailed($"no material at '{value}'");
                work.Commit.Add(() => imp.AddRemap(id, mat));
            }
            return;
        }
        var p = typeof(ModelImporter).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
        if (p == null || !p.CanWrite || p.GetCustomAttribute<ObsoleteAttribute>() != null || s_SkipSettings.Contains(key))
        {
            var names = SettingProperties(typeof(ModelImporter), s_SkipSettings).Select(x => x.Name).ToArray();
            throw new EditFailed($"'{key}' is not an import setting; {Suggest(key, names)}");
        }
        if (key == "motionNodeName" && value.Length > 0)
            value = TransformPath(imp, value);
        var parsed = ParseOp(p.PropertyType, () => p.GetValue(imp), op, value, key, imp);
        if (parsed is float number && (float.IsNaN(number) || float.IsInfinity(number) || (key == "globalScale" && number <= 0)))
            throw new EditFailed($"{key} must be a finite number{(key == "globalScale" ? " above 0" : "")}, not '{value}'");
        if (parsed is Avatar source)
        {
            if (!source.isValid)
                throw new EditFailed($"{value} has no valid avatar to copy");
            // Assigning sourceAvatar alone leaves the importer's human description empty, so the copied rig finds no bones.
            work.Commit.Add(() =>
            {
                p.SetValue(imp, source);
                if (source.isHuman)
                    imp.humanDescription = source.humanDescription;
            });
            return;
        }
        work.Commit.Add(() => p.SetValue(imp, parsed));
    }

    // The import that turns Humanoid on maps bones itself and would overwrite mapping edits made in the same call.
    static void RequireHuman(Working work)
    {
        if (work.ChangesRig)
            throw new EditFailed("change animationType, avatarSetup or sourceAvatar in one call and the humanoid mapping in the next: the import that changes the rig maps bones itself");
        if (work.Imp.animationType != ModelImporterAnimationType.Human)
            throw new EditFailed($"human edits need animationType=Human (it is {work.Imp.animationType})");
    }

    static void PlanClip(Working work, ModelImporterClipAnimation clip, string prop, string op, string value)
    {
        if (prop == "events")
        {
            var list = (clip.events ?? new AnimationEvent[0]).ToList();
            if (op == "=" && value.Length == 0)
                list.Clear();
            else if (op == "+=")
            {
                var parts = value.Split(new[] { ':' }, 3);
                if (parts.Length < 2 || !float.TryParse(parts[0], NumberStyles.Float, Inv, out var t) || t < 0 || t > 1)
                    throw new EditFailed($"an event is time:Function[:text], with time 0..1 of the clip; got '{value}'");
                list.Add(new AnimationEvent { time = t, functionName = parts[1], stringParameter = parts.Length > 2 ? parts[2] : "" });
            }
            else if (op == "-=")
            {
                if (list.RemoveAll(e => e.functionName == value) == 0)
                    throw new EditFailed($"the clip has no event calling '{value}'");
            }
            else
                throw new EditFailed("events takes +=time:Function[:text], -=Function, or = (clear)");
            clip.events = list.OrderBy(e => e.time).ToArray();
            return;
        }
        if (prop == "mask")
        {
            if (value.Length == 0 || value == "None")
            {
                clip.maskSource = null;
                clip.maskType = ClipAnimationMaskType.None;
            }
            else
            {
                clip.maskSource = AssetDatabase.LoadAssetAtPath<AvatarMask>(value) ?? throw new EditFailed($"no AvatarMask at '{value}'");
                clip.maskType = ClipAnimationMaskType.CopyFromOther;
            }
            return;
        }
        var p = typeof(ModelImporterClipAnimation).GetProperty(prop, BindingFlags.Public | BindingFlags.Instance);
        if (p == null || !p.CanWrite || p.GetCustomAttribute<ObsoleteAttribute>() != null || (s_SkipClip.Contains(prop) && prop != "name" && prop != "takeName"))
        {
            var names = SettingProperties(typeof(ModelImporterClipAnimation), s_SkipClip).Select(x => x.Name).Concat(new[] { "name", "takeName", "events", "mask" }).ToArray();
            throw new EditFailed($"'{prop}' is not a clip setting; {Suggest(prop, names)}");
        }
        if (prop == "takeName" && !work.Imp.importedTakeInfos.Any(t => t.name == value))
            throw new EditFailed($"no take '{value}'; takes: {string.Join(", ", work.Imp.importedTakeInfos.Select(t => t.name))}");
        if (prop == "name" && work.Clips.Any(c => c != clip && c.name == value))
            throw new EditFailed($"a clip named '{value}' already exists");
        p.SetValue(clip, ParseOp(p.PropertyType, () => p.GetValue(clip), op, value, prop, null));
    }

    static object ParseOp(Type type, Func<object> current, string op, string value, string key, ModelImporter imp)
    {
        if (type == typeof(string[]))
        {
            var items = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            if (key == "extraExposedTransformPaths" && imp != null)
                items = items.Select(s => TransformPath(imp, s)).ToList();
            var now = ((string[])current() ?? new string[0]).ToList();
            if (op == "=")
                return items.ToArray();
            if (op == "+=")
                return now.Concat(items.Where(s => !now.Contains(s))).ToArray();
            foreach (var s in items.Where(s => !now.Contains(s)))
                throw new EditFailed($"'{s}' is not in {key}");
            return now.Where(s => !items.Contains(s)).ToArray();
        }
        if (op != "=")
            throw new EditFailed($"{key} takes = (+= and -= are for lists)");
        return Parse(type, value, key);
    }

    // ModelImporter.transformPaths is empty until the model has an avatar; the imported hierarchy has the same paths then.
    static string[] TransformPaths(ModelImporter imp)
    {
        var paths = imp.transformPaths;
        if (paths != null && paths.Length > 0)
            return paths;
        var main = AssetDatabase.LoadMainAssetAtPath(imp.assetPath) as GameObject;
        if (main == null)
            return new string[0];
        var list = new List<string>();
        void Walk(Transform t, string path)
        {
            list.Add(path);
            foreach (Transform c in t)
                Walk(c, path.Length == 0 ? c.name : path + "/" + c.name);
        }
        Walk(main.transform, "");
        return list.ToArray();
    }

    // "path" or "path#name"; a path that itself contains # is taken whole when an asset is there.
    static (string path, string sub) SplitSubAsset(string value)
    {
        if (AssetDatabase.LoadMainAssetAtPath(value) != null)
            return (value, null);
        var hash = value.LastIndexOf('#');
        return hash < 0 ? (value, null) : (value.Substring(0, hash), value.Substring(hash + 1));
    }

    static IEnumerable<string> EnumNames(Type type) => type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.GetCustomAttribute<ObsoleteAttribute>() == null).Select(f => f.Name);

    // A full transform path, or a tail of one (Hips/Spine, or a bare name) that only one path ends with.
    static string TransformPath(ModelImporter imp, string s)
    {
        var paths = TransformPaths(imp);
        if (paths.Contains(s))
            return s;
        var tails = paths.Where(p => p.EndsWith("/" + s, StringComparison.Ordinal)).ToList();
        if (tails.Count == 1)
            return tails[0];
        if (tails.Count > 1)
            throw new EditFailed($"'{s}' ends {tails.Count} transform paths: {string.Join(", ", tails.Take(4))}");
        throw new EditFailed($"'{s}' is not a transform path in the model; {Suggest(s, paths.Select(p => p.Substring(p.LastIndexOf('/') + 1)), "`modelwaitress tree <model>` lists the transforms")}");
    }

    static object Parse(Type type, string value, string key)
    {
        if (type == typeof(ModelImporterAnimationType) && Same(value, "Humanoid"))
            value = "Human";
        if (type == typeof(bool))
        {
            switch (value.ToLowerInvariant())
            {
                case "true": case "1": case "on": case "yes": return true;
                case "false": case "0": case "off": case "no": return false;
            }
            throw new EditFailed($"{key} is true or false, not '{value}'");
        }
        if (type == typeof(float))
        {
            if (float.TryParse(value, NumberStyles.Float, Inv, out var f) && !float.IsNaN(f) && !float.IsInfinity(f))
                return f;
            throw new EditFailed($"{key} is a number, not '{value}'");
        }
        if (type == typeof(int))
        {
            if (int.TryParse(value, NumberStyles.Integer, Inv, out var n))
                return n;
            throw new EditFailed($"{key} is a whole number, not '{value}'");
        }
        if (type == typeof(string))
            return value;
        if (type.IsEnum)
        {
            var names = EnumNames(type).ToArray();
            if (int.TryParse(value, NumberStyles.Integer, Inv, out var number))
            {
                if (type.GetCustomAttribute<FlagsAttribute>() == null && !Enum.IsDefined(type, Enum.ToObject(type, number)))
                    throw new EditFailed($"{key} has no value {number}; it takes {string.Join(", ", EnumNames(type))}");
                return Enum.ToObject(type, number);
            }
            var parts = value.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            if (parts.Count > 1 && type.GetCustomAttribute<FlagsAttribute>() == null)
                throw new EditFailed($"{key} takes one of: {string.Join(", ", names)}");
            long bits = 0;
            foreach (var part in parts)
            {
                var hit = names.FirstOrDefault(n => Same(n, part)) ?? throw new EditFailed($"{key} takes {string.Join(", ", names)}; not '{part}'");
                bits |= Convert.ToInt64(Enum.Parse(type, hit), Inv);
            }
            return Enum.ToObject(type, bits);
        }
        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            if (value.Length == 0 || value == "None")
                return null;
            var (assetPath, sub) = SplitSubAsset(value);
            return AssetDatabase.LoadAllAssetsAtPath(assetPath).FirstOrDefault(o => o != null && type.IsInstanceOfType(o) && (sub == null || o.name == sub))
                   ?? throw new EditFailed($"no {type.Name} at '{value}'");
        }
        throw new EditFailed($"{key} ({type.Name}) can't be set by modelwaitress");
    }

    static bool Same(string a, string b) => string.Equals(a.Replace(" ", ""), b.Replace(" ", ""), StringComparison.OrdinalIgnoreCase);

    static string Suggest(string key, IEnumerable<string> names, string otherwise = "see `modelwaitress settings <model> --all`")
    {
        var close = names.Select(n => (n, d: Distance(n.ToLowerInvariant(), key.ToLowerInvariant()))).Where(x => x.d <= Math.Max(2, key.Length / 3) || x.n.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                         .OrderBy(x => x.d).Take(4).Select(x => x.n).ToList();
        return close.Count > 0 ? "did you mean " + string.Join(", ", close) + "?" : otherwise;
    }

    static int Distance(string a, string b)
    {
        var d = new int[a.Length + 1, b.Length + 1];
        for (var x = 0; x <= a.Length; x++)
            d[x, 0] = x;
        for (var y = 0; y <= b.Length; y++)
            d[0, y] = y;
        for (var x = 1; x <= a.Length; x++)
            for (var y = 1; y <= b.Length; y++)
                d[x, y] = Math.Min(Math.Min(d[x - 1, y] + 1, d[x, y - 1] + 1), d[x - 1, y - 1] + (a[x - 1] == b[y - 1] ? 0 : 1));
        return d[a.Length, b.Length];
    }

    // ---------------------------------------------------------------- formatting

    static string TypeName(Type t) => t == typeof(bool) ? "bool" : t == typeof(float) ? "float" : t == typeof(int) ? "int" : t == typeof(string) ? "string"
        : t == typeof(string[]) ? "list" : t.IsEnum ? t.Name : typeof(UnityEngine.Object).IsAssignableFrom(t) ? "asset" : t.Name;

    static string Format(object v)
    {
        switch (v)
        {
            case null: return "None";
            case bool b: return b ? "true" : "false";
            case float f: return f.ToString("R", Inv);
            case double d: return d.ToString("R", Inv);
            case int n: return n.ToString(Inv);
            case string s: return s;
            case string[] list: return string.Join(",", list);
            case Enum e: return e.ToString().Replace(", ", "|");
            case UnityEngine.Object o: return o == null ? "None" : ObjectPath(o);
            case Vector3 vec: return vec.x.ToString("R", Inv) + "," + vec.y.ToString("R", Inv) + "," + vec.z.ToString("R", Inv);
        }
        return v.ToString();
    }

    static string ObjectPath(UnityEngine.Object o)
    {
        if (o == null)
            return null;
        var path = AssetDatabase.GetAssetPath(o);
        if (string.IsNullOrEmpty(path))
            return o.name;
        return AssetDatabase.IsSubAsset(o) ? path + "#" + o.name : path;
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
                m_Sb.Append(((float)d == d ? ((float)d).ToString("R", Inv) : d.ToString("R", Inv)));
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
