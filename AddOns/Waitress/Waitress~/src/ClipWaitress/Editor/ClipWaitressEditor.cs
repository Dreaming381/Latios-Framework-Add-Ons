// The Editor half of clipwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/ClipWaitress/, and runs
// it through `unity command run_script`, so nothing is imported into the project. Export writes every clip of an asset
// with its settings, events and keyframes; Check resolves a clip's bindings against the objects that play it; Edit
// changes .anim clips through AnimationUtility.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ClipWaitressEditor
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    // ---------------------------------------------------------------- export

    // {"paths":[...],"out":"<dir>"}: writes <dir>/Clips/<guid>.json per asset.
    public static string Export(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var outDir = Path.Combine((string)root["out"], "Clips");
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
                // Brings the import up to date when the file changed while the Editor wasn't looking; no-op otherwise.
                AssetDatabase.ImportAsset(path);
                File.WriteAllText(Path.Combine(outDir, AssetDatabase.AssetPathToGUID(path) + ".json"), ExportAsset(path), new UTF8Encoding(false));
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

    static IEnumerable<AnimationClip> ClipsAt(string path) =>
        AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview__", StringComparison.Ordinal));

    static string ExportAsset(string path)
    {
        var w = new W();
        w.Begin();
        w.Key("v").Num(1);
        w.Key("path").Str(path);
        w.Key("guid").Str(AssetDatabase.AssetPathToGUID(path));
        w.Key("stamp").Str(Stamp(path));
        w.Key("unity").Str(Application.unityVersion);
        var importer = AssetImporter.GetAtPath(path);
        w.Key("kind").Str(importer is ModelImporter ? "model" : "anim");
        w.Key("clips").Arr();
        foreach (var clip in ClipsAt(path))
            WriteClip(w, clip, path);
        w.EndArr();
        w.End();
        return w.ToString();
    }

    static MethodInfo s_Quality, s_Stats;

    static void WriteClip(W w, AnimationClip clip, string path)
    {
        w.Begin();
        w.Key("name").Str(clip.name);
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(clip, out string _, out long id);
        w.Key("id").Str(id.ToString(Inv));
        w.Key("length").Num(clip.length).Key("fps").Num(clip.frameRate).Key("legacy").Bool(clip.legacy).Key("wrap").Str(clip.wrapMode.ToString())
         .Key("human").Bool(clip.isHumanMotion).Key("genericRoot").Bool(clip.hasGenericRootTransform).Key("motionCurves").Bool(clip.hasMotionCurves)
         .Key("rootCurves").Bool(clip.hasRootCurves).Key("motionFloat").Bool(clip.hasMotionFloatCurves).Key("looping").Bool(clip.isLooping).Key("empty").Bool(clip.empty);
        w.Key("averageSpeed").Vec(clip.averageSpeed).Key("averageAngularSpeed").Num(clip.averageAngularSpeed * Mathf.Rad2Deg)
         .Key("apparentSpeed").Num(clip.apparentSpeed).Key("averageDuration").Num(clip.averageDuration);
        w.Key("bounds").Arr().Vec(clip.localBounds.center).Vec(clip.localBounds.extents).EndArr();

        s_Stats ??= typeof(AnimationUtility).GetMethod("GetAnimationClipStats", Any);
        if (s_Stats != null)
        {
            var stats = s_Stats.Invoke(null, new object[] { clip });
            w.Key("stats").Begin();
            foreach (var f in stats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                w.Key(f.Name).Num(Convert.ToDouble(f.GetValue(stats), Inv));
            w.End();
        }
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        if (clip.hasRootCurves || clip.hasMotionCurves)
        {
            s_Quality ??= typeof(Editor).Assembly.GetType("UnityEditor.MuscleClipUtility")?.GetMethod("GetMuscleClipQualityInfo", Any);
            var q = s_Quality?.Invoke(null, new object[] { clip, settings.startTime, settings.stopTime });
            if (q != null)
            {
                w.Key("quality").Begin();
                foreach (var f in q.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                    w.Key(f.Name).Num(Convert.ToDouble(f.GetValue(q), Inv));
                w.End();
            }
        }
        WriteSettings(w, clip, settings);

        w.Key("events").Arr();
        foreach (var e in AnimationUtility.GetAnimationEvents(clip))
        {
            w.Begin();
            w.Key("t").Num(e.time).Key("f").Str(e.functionName);
            if (!string.IsNullOrEmpty(e.stringParameter))
                w.Key("s").Str(e.stringParameter);
            if (e.floatParameter != 0)
                w.Key("n").Num(e.floatParameter);
            if (e.intParameter != 0)
                w.Key("i").Num(e.intParameter);
            if (e.objectReferenceParameter != null)
                w.Key("o").Str(ObjectPath(e.objectReferenceParameter));
            if (e.messageOptions != SendMessageOptions.RequireReceiver)
                w.Key("optional").Bool(true);
            w.End();
        }
        w.EndArr();

        w.Key("curves").Arr();
        foreach (var b in AnimationUtility.GetCurveBindings(clip))
        {
            var curve = AnimationUtility.GetEditorCurve(clip, b);
            if (curve == null)
                continue;
            w.Begin();
            w.Key("p").Str(b.path).Key("t").Str(b.type?.Name ?? "?").Key("n").Str(b.propertyName).Key("c").Str(Class(b));
            if (b.isDiscreteCurve)
                w.Key("d").Bool(true);
            if (curve.preWrapMode != WrapMode.ClampForever && curve.preWrapMode != WrapMode.Default && curve.preWrapMode != WrapMode.Clamp)
                w.Key("pre").Str(curve.preWrapMode.ToString());
            if (curve.postWrapMode != WrapMode.ClampForever && curve.postWrapMode != WrapMode.Default && curve.postWrapMode != WrapMode.Clamp)
                w.Key("post").Str(curve.postWrapMode.ToString());
            var keys = curve.keys;
            w.Key("k").Arr();
            foreach (var k in keys)
                w.NumX(k.time).NumX(k.value).NumX(k.inTangent).NumX(k.outTangent);
            w.EndArr();
            if (keys.Any(k => k.weightedMode != WeightedMode.None))
            {
                w.Key("w").Arr();
                foreach (var k in keys)
                    w.Num((int)k.weightedMode).Num(k.inWeight).Num(k.outWeight);
                w.EndArr();
            }
            w.End();
        }
        w.EndArr();

        // Euler curves play in the rotation order stored on the clip's runtime curve, which the editor curves don't carry.
        var orders = new SerializedObject(clip).FindProperty("m_EulerCurves");
        w.Key("eulerOrder").Begin();
        for (var e = 0; orders != null && e < orders.arraySize; e++)
        {
            var item = orders.GetArrayElementAtIndex(e);
            var order = item.FindPropertyRelative("curve.m_RotationOrder");
            if (order != null && order.intValue != 4)
                w.Key(item.FindPropertyRelative("path").stringValue).Num(order.intValue);
        }
        w.End();

        var missing = MissingReferences(clip);
        w.Key("objs").Arr();
        foreach (var b in AnimationUtility.GetObjectReferenceCurveBindings(clip))
        {
            w.Begin();
            w.Key("p").Str(b.path).Key("t").Str(b.type?.Name ?? "?").Key("n").Str(b.propertyName);
            var keys = AnimationUtility.GetObjectReferenceCurve(clip, b);
            missing.TryGetValue(b.path + "\n" + b.propertyName, out var lost);
            w.Key("k").Arr();
            for (var k = 0; k < keys.Length; k++)
            {
                w.Num(keys[k].time);
                w.Str(keys[k].value != null ? ObjectPath(keys[k].value) : lost != null && lost.Contains(k) ? "missing" : "None");
            }
            w.EndArr();
            w.End();
        }
        w.EndArr();
        w.End();
    }

    // Keys of object curves whose reference points at an object that no longer loads, keyed by path and attribute.
    static Dictionary<string, HashSet<int>> MissingReferences(AnimationClip clip)
    {
        var found = new Dictionary<string, HashSet<int>>();
        var curves = new SerializedObject(clip).FindProperty("m_PPtrCurves");
        if (curves == null)
            return found;
        for (var c = 0; c < curves.arraySize; c++)
        {
            var curve = curves.GetArrayElementAtIndex(c);
            var keys = curve.FindPropertyRelative("curve");
            var set = new HashSet<int>();
            for (var k = 0; keys != null && k < keys.arraySize; k++)
            {
                var value = keys.GetArrayElementAtIndex(k).FindPropertyRelative("value");
                if (value != null && value.objectReferenceValue == null && value.objectReferenceInstanceIDValue != 0)
                    set.Add(k);
            }
            if (set.Count > 0)
                found[curve.FindPropertyRelative("path").stringValue + "\n" + curve.FindPropertyRelative("attribute").stringValue] = set;
        }
        return found;
    }

    static HashSet<string> s_Muscles;

    static string Class(EditorCurveBinding b)
    {
        if (b.type == typeof(Transform))
        {
            if (b.propertyName.StartsWith("m_LocalPosition", StringComparison.Ordinal))
                return "pos";
            if (b.propertyName.StartsWith("m_LocalRotation", StringComparison.Ordinal))
                return "quat";
            if (b.propertyName.StartsWith("m_LocalScale", StringComparison.Ordinal))
                return "scale";
            if (b.propertyName.StartsWith("localEuler", StringComparison.Ordinal))
                return "euler";
        }
        if (b.type == typeof(GameObject) && b.propertyName == "m_IsActive")
            return "active";
        if (b.type == typeof(Animator) && b.path.Length == 0 && b.propertyName != "m_Enabled")
        {
            var n = b.propertyName;
            if (n.StartsWith("RootT.", StringComparison.Ordinal) || n.StartsWith("RootQ.", StringComparison.Ordinal))
                return "root";
            if (n.StartsWith("MotionT.", StringComparison.Ordinal) || n.StartsWith("MotionQ.", StringComparison.Ordinal))
                return "motion";
            foreach (var goal in new[] { "LeftFoot", "RightFoot", "LeftHand", "RightHand" })
                if (n.StartsWith(goal + "T.", StringComparison.Ordinal) || n.StartsWith(goal + "Q.", StringComparison.Ordinal))
                    return "ik";
            s_Muscles ??= new HashSet<string>(HumanTrait.MuscleName.Concat(FingerMuscles()));
            if (s_Muscles.Contains(n))
                return "muscle";
            return "param";
        }
        return "prop";
    }

    // Clip curves name finger muscles "LeftHand.Thumb.1 Stretched" where HumanTrait says "Left Thumb 1 Stretched".
    static IEnumerable<string> FingerMuscles()
    {
        foreach (var side in new[] { "Left", "Right" })
            foreach (var finger in new[] { "Thumb", "Index", "Middle", "Ring", "Little" })
            {
                yield return $"{side}Hand.{finger}.Spread";
                for (var n = 1; n <= 3; n++)
                    yield return $"{side}Hand.{finger}.{n} Stretched";
            }
    }

    static readonly string[] s_SettingOrder =
    {
        "loopTime", "loopBlend", "cycleOffset", "mirror", "startTime", "stopTime", "loopBlendOrientation", "keepOriginalOrientation", "orientationOffsetY",
        "loopBlendPositionY", "keepOriginalPositionY", "heightFromFeet", "level", "loopBlendPositionXZ", "keepOriginalPositionXZ",
        "hasAdditiveReferencePose", "additiveReferencePoseClip", "additiveReferencePoseTime",
    };

    static void WriteSettings(W w, AnimationClip clip, AnimationClipSettings s)
    {
        var defaults = AnimationUtility.GetAnimationClipSettings(new AnimationClip());
        defaults.stopTime = clip.length;
        w.Key("settings").Arr();
        foreach (var name in s_SettingOrder)
        {
            var f = typeof(AnimationClipSettings).GetField(name);
            if (f == null)
                continue;
            w.Arr().Str(name).Str(TypeName(f.FieldType)).Str(Format(f.GetValue(s))).Str(Format(f.GetValue(defaults))).EndArr();
        }
        w.Arr().Str("wrapMode").Str("WrapMode").Str(clip.wrapMode.ToString()).Str("Default").EndArr();
        w.Arr().Str("legacy").Str("bool").Str(Format(clip.legacy)).Str("false").EndArr();
        w.EndArr();
    }

    // ---------------------------------------------------------------- check

    // {"clips":[{"path":..,"id":..}],"targets":[..],"strict":bool}: resolves each clip's bindings against the Animators
    // and Animation components in each target that play it (strict), or against all of them.
    public static string Check(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var clips = ((List<object>)root["clips"]).Cast<Dictionary<string, object>>().Select(c => FindClip((string)c["path"], (string)c["id"])).ToList();
        var targets = ((List<object>)root["targets"]).Cast<string>().ToList();
        var strict = root.TryGetValue("strict", out var s) && (bool)s;
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        w.Key("results").Arr();
        foreach (var target in targets)
        {
            var roots = TargetRoots(target, out var why);
            if (roots == null)
            {
                w.Begin().Key("target").Str(target).Key("skipped").Str(why).End();
                continue;
            }
            foreach (var clip in clips)
            {
                var holders = new List<Component>();
                foreach (var r in roots)
                    holders.AddRange(r.GetComponentsInChildren<Component>(true).Where(c => c is Animator || c is Animation));
                var playing = holders.Where(h => Plays(h, clip)).ToList();
                if (playing.Count == 0 && !strict)
                    playing = holders;
                if (playing.Count == 0)
                {
                    if (strict)
                        continue;
                    foreach (var r in roots)
                        CheckOne(w, clip, target, r, null);
                    continue;
                }
                foreach (var h in playing)
                    CheckOne(w, clip, target, h.gameObject, h);
            }
        }
        w.EndArr();
        w.End();
        return w.ToString();
    }

    static AnimationClip FindClip(string path, string id)
    {
        foreach (var c in ClipsAt(path))
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(c, out string _, out long local);
            if (local.ToString(Inv) == id)
                return c;
        }
        throw new Exception("no clip " + id + " in " + path);
    }

    static List<GameObject> TargetRoots(string target, out string why)
    {
        why = null;
        if (target.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
        {
            var scene = EditorSceneManager.GetSceneByPath(target);
            if (!scene.isLoaded)
            {
                why = "scene not open in the Editor";
                return null;
            }
            return scene.GetRootGameObjects().ToList();
        }
        var go = AssetDatabase.LoadMainAssetAtPath(target) as GameObject;
        if (go == null)
        {
            why = "not a prefab, model or open scene";
            return null;
        }
        return new List<GameObject> { go };
    }

    static bool Plays(Component holder, AnimationClip clip)
    {
        if (holder is Animator a)
            return a.runtimeAnimatorController != null && a.runtimeAnimatorController.animationClips.Contains(clip);
        if (holder is Animation anim)
            return AnimationUtility.GetAnimationClips(anim.gameObject).Contains(clip);
        return false;
    }

    static string HierarchyPath(Transform t)
    {
        var parts = new List<string>();
        for (; t != null; t = t.parent)
            parts.Add(t.name);
        parts.Reverse();
        return string.Join("/", parts);
    }

    static void CheckOne(W w, AnimationClip clip, string target, GameObject go, Component holder)
    {
        w.Begin();
        w.Key("target").Str(target);
        w.Key("object").Str(HierarchyPath(go.transform));
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(clip, out string guid, out long id);
        w.Key("clip").Str(AssetDatabase.GUIDToAssetPath(guid) + "#" + clip.name);
        var animator = holder as Animator;
        var animation = holder as Animation;
        w.Key("holder").Str(animator != null ? "Animator" : animation != null ? "Animation" : "none");
        if (animator != null)
            w.Key("controller").Str(animator.runtimeAnimatorController == null ? "none" : animator.runtimeAnimatorController.name);
        if (holder != null)
            w.Key("plays").Bool(Plays(holder, clip));
        var problems = new List<string>();
        var notes = new List<string>();
        if (animator != null && clip.legacy)
            problems.Add("legacy|the clip is marked Legacy, which an Animator can't play (set legacy=false)");
        if (animation != null && !clip.legacy)
            problems.Add("legacy|the Animation component only plays Legacy clips (set legacy=true, or play it from an Animator)");

        var bindings = AnimationUtility.GetCurveBindings(clip);
        var objects = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        var humanCurves = bindings.Count(b => Class(b) is "muscle" or "root" or "ik");
        if (animator != null)
        {
            var avatar = animator.avatar;
            w.Key("avatar").Str(avatar == null ? "none" : avatar.name + (avatar.isHuman ? " (humanoid)" : avatar.isValid ? " (generic)" : " (invalid)"));
            if (humanCurves > 0 && (avatar == null || !avatar.isHuman || !avatar.isValid))
                problems.Add($"humanoid|{humanCurves} humanoid curves (muscles, root, IK) need a valid humanoid Avatar on the Animator; it has {(avatar == null ? "none" : avatar.isHuman ? "an invalid one" : "a generic one")}, so the body won't move");
            if (!animator.applyRootMotion && clip.averageSpeed.magnitude > 0.05f)
                notes.Add($"the clip moves its root ({clip.averageSpeed.magnitude.ToString("0.##", Inv)} m/s) but Apply Root Motion is off, so the object stays in place");
            if (animator.applyRootMotion && clip.averageSpeed.magnitude <= 0.05f && clip.averageAngularSpeed == 0)
                notes.Add("Apply Root Motion is on; the clip has no root movement");
        }
        else if (humanCurves > 0)
            problems.Add($"humanoid|{humanCurves} humanoid curves need an Animator with a humanoid Avatar");

        var optimized = animator != null && !animator.hasTransformHierarchy;
        if (optimized)
            notes.Add("the hierarchy is optimized (Optimize Game Objects), so transform curves bind through the Avatar and weren't checked");

        // Parameters a clip curve drives must exist on the controller as floats.
        var controller = animator != null ? ControllerOf(animator.runtimeAnimatorController) : null;
        var parameters = controller?.parameters.ToDictionary(p => p.name, p => p.type) ?? new Dictionary<string, AnimatorControllerParameterType>();

        var lost = new Dictionary<string, (int count, string why)>();
        // A path that exists but lacks several component types names them all.
        void Lose(string path, string type)
        {
            lost.TryGetValue(path, out var had);
            string why;
            if (type == null || had.why == "path")
                why = "path";
            else if (had.why == null)
                why = "no " + type + " on it";
            else if (had.why.Contains(" " + type + " ") || had.why.Contains(" " + type + ","))
                why = had.why;
            else
                why = had.why.Substring(0, had.why.Length - 6) + ", " + type + " on it";
            lost[path] = (had.count + 1, why);
        }
        var props = new List<string>();
        foreach (var b in bindings.Concat(objects))
        {
            var cls = b.isPPtrCurve ? "pptr" : Class(b);
            if (cls is "muscle" or "root" or "ik" or "motion")
                continue;
            if (cls == "param")
            {
                if (animator == null)
                    continue;
                if (controller == null)
                    problems.Add($"param|curve {b.propertyName} drives an Animator parameter, but the Animator has no controller");
                else if (!parameters.TryGetValue(b.propertyName, out var type))
                    problems.Add($"param|curve {b.propertyName} drives an Animator parameter the controller {controller.name} doesn't have, so it drives nothing (add a Float parameter named {b.propertyName})");
                else if (type != AnimatorControllerParameterType.Float)
                    problems.Add($"param|curve {b.propertyName} drives an {type} parameter; clip curves only drive Float parameters");
                continue;
            }
            if (optimized && b.type == typeof(Transform))
                continue;
            var node = b.path.Length == 0 ? go.transform : go.transform.Find(b.path);
            if (node == null)
            {
                Lose(b.path, null);
                continue;
            }
            var animated = AnimationUtility.GetAnimatedObject(go, b);
            if (animated == null)
            {
                Lose(b.path, b.type?.Name);
                continue;
            }
            if (!b.isPPtrCurve && b.type != typeof(Transform) && !AnimationUtility.GetFloatValue(go, b, out _))
                props.Add($"{b.path}:{b.type?.Name}.{b.propertyName}|{Alternatives(go, node.gameObject, b)}");
        }
        var all = go.GetComponentsInChildren<Transform>(true).Select(t => (t, path: RelativePath(go.transform, t))).ToList();
        w.Key("missing").Arr();
        foreach (var pair in lost.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            w.Begin().Key("path").Str(pair.Key).Key("curves").Num(pair.Value.count).Key("why").Str(pair.Value.why);
            if (pair.Value.why == "path")
            {
                // The first segment that doesn't resolve, and objects elsewhere with its name (a rename or a move).
                var parts = pair.Key.Split('/');
                var depth = 0;
                while (depth < parts.Length - 1 && go.transform.Find(string.Join("/", parts.Take(depth + 1))) != null)
                    depth++;
                var first = string.Join("/", parts.Take(depth + 1));
                w.Key("exists").Str(string.Join("/", parts.Take(depth)));
                w.Key("first").Str(first);
                w.Key("same").Strings(all.Where(x => x.t.name == parts[depth] && x.path.Length > 0).Select(x => x.path).Take(4));
                var parent = depth == 0 ? go.transform : go.transform.Find(string.Join("/", parts.Take(depth)));
                w.Key("children").Strings(parent == null ? new string[0] : parent.Cast<Transform>().Select(t => t.name).Take(40));
            }
            w.End();
        }
        w.EndArr();
        w.Key("props").Strings(props);

        var receivers = go.GetComponents<MonoBehaviour>().Where(m => m != null).ToList();
        w.Key("events").Arr();
        foreach (var e in AnimationUtility.GetAnimationEvents(clip))
        {
            if (holder == null || e.messageOptions == SendMessageOptions.DontRequireReceiver)
                continue;
            var methods = receivers.SelectMany(r => r.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                                   .Where(m => m.Name == e.functionName).ToList();
            if (methods.Count == 0)
            {
                w.Str($"{e.functionName} at {e.time.ToString("0.###", Inv)}s: no script on {go.name} has this method (Unity logs \"AnimationEvent has no receiver\")");
                continue;
            }
            if (!methods.Any(m => m.GetParameters().Length == 0 || m.GetParameters().Length == 1 && Accepts(m.GetParameters()[0].ParameterType)))
                w.Str($"{e.functionName} at {e.time.ToString("0.###", Inv)}s: {methods[0].DeclaringType.Name}.{e.functionName} takes parameters an event can't pass (none, or one float, int, string, Object or AnimationEvent)");
        }
        w.EndArr();
        w.Key("receivers").Strings(receivers.Select(r => r.GetType().Name));
        w.Key("problems").Strings(problems);
        w.Key("notes").Strings(notes);
        w.End();
    }

    // What the object does have in place of a property that doesn't exist: its blend shapes, or the closest property names.
    static string Alternatives(GameObject root, GameObject target, EditorCurveBinding b)
    {
        if (b.propertyName.StartsWith("blendShape.", StringComparison.Ordinal))
        {
            var mesh = target.GetComponent<SkinnedMeshRenderer>()?.sharedMesh;
            if (mesh == null)
                return "the object has no SkinnedMeshRenderer mesh";
            if (mesh.blendShapeCount == 0)
                return $"its mesh {mesh.name} has no blend shapes";
            var names = Enumerable.Range(0, mesh.blendShapeCount).Select(mesh.GetBlendShapeName).ToList();
            return $"its mesh {mesh.name} has blend shapes {string.Join(", ", names.OrderBy(n => Distance(n, b.propertyName.Substring(11))).Take(8))}{(names.Count > 8 ? $" (+{names.Count - 8})" : "")}";
        }
        var have = AnimationUtility.GetAnimatableBindings(target, root).Where(x => x.type == b.type).Select(x => x.propertyName).Distinct().ToList();
        if (have.Count == 0)
            return $"no animatable {b.type?.Name} properties there";
        return "closest animatable: " + string.Join(", ", have.OrderBy(n => Distance(n.ToLowerInvariant(), b.propertyName.ToLowerInvariant())).Take(4));
    }

    static bool Accepts(Type t) => t == typeof(float) || t == typeof(int) || t == typeof(string) || t == typeof(AnimationEvent) || typeof(UnityEngine.Object).IsAssignableFrom(t);

    static AnimatorController ControllerOf(RuntimeAnimatorController c)
    {
        while (c is AnimatorOverrideController o)
            c = o.runtimeAnimatorController;
        return c as AnimatorController;
    }

    static string RelativePath(Transform root, Transform t)
    {
        var parts = new List<string>();
        for (; t != null && t != root; t = t.parent)
            parts.Add(t.name);
        parts.Reverse();
        return string.Join("/", parts);
    }

    // ---------------------------------------------------------------- edit

    sealed class EditFailed : Exception
    {
        public EditFailed(string message) : base(message) { }
    }

    // {"out":..,"dryRun":bool,"ops":[{"path","id","key","op","value"}]}: every op is checked first; nothing changes when
    // any is wrong. Then each clip is changed and saved once.
    public static string Edit(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var dryRun = root.TryGetValue("dryRun", out var d) && (bool)d;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return Error("the Editor is compiling or importing; try again when it finishes");
        var ops = ((List<object>)root["ops"]).Cast<Dictionary<string, object>>().ToList();
        var work = new Dictionary<AnimationClip, Working>();
        var errors = new List<string>();
        foreach (var op in ops)
        {
            var path = (string)op["path"];
            AnimationClip clip;
            try
            {
                clip = FindClip(path, (string)op["id"]);
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
                continue;
            }
            if (AssetImporter.GetAtPath(path) is ModelImporter)
            {
                errors.Add($"{path}#{clip.name} is imported from a model, so its curves are read-only; change its import settings with `modelwaitress set {path} clips[{clip.name}].<setting>=...`, or `clipwaitress copy` it to an .anim and edit that");
                continue;
            }
            if (!work.TryGetValue(clip, out var wk))
                work[clip] = wk = new Working(clip, path);
            try
            {
                Plan(wk, (string)op["key"], (string)op["op"], (string)op["value"]);
            }
            catch (EditFailed e)
            {
                errors.Add((work.Count > 1 || ops.Select(o => o["path"]).Distinct().Count() > 1 ? clip.name + ": " : "") + e.Message);
            }
        }
        foreach (var wk in work.Values)
        {
            try
            {
                wk.Check();
            }
            catch (EditFailed e)
            {
                errors.Add(e.Message);
            }
        }
        if (errors.Count > 0)
            return Error(string.Join("\n", errors.Distinct()));
        var done = new List<string>();
        var notes = new List<string>();
        var previews = new List<string>();
        if (dryRun)
        {
            // The planned edits applied to a throwaway copy, exported like the real clip, so the CLI can show the change.
            foreach (var wk in work.Values)
            {
                var copy = UnityEngine.Object.Instantiate(wk.Clip);
                copy.name = wk.NewName ?? wk.Clip.name;
                try
                {
                    wk.Apply(copy);
                    var pw = new W();
                    WriteClip(pw, copy, wk.Path);
                    previews.Add(pw.ToString());
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(copy);
                }
            }
        }
        else
        {
            foreach (var wk in work.Values)
            {
                if (EditorUtility.IsDirty(wk.Clip))
                    notes.Add($"{wk.Path} had unsaved changes in the Editor; they were saved with this edit");
                Undo.RegisterCompleteObjectUndo(wk.Clip, "clipwaitress set");
                wk.Apply(wk.Clip);
                EditorUtility.SetDirty(wk.Clip);
                AssetDatabase.SaveAssetIfDirty(wk.Clip);
                if (wk.NewName != null)
                {
                    var why = AssetDatabase.RenameAsset(wk.Path, wk.NewName);
                    if (!string.IsNullOrEmpty(why))
                        notes.Add("rename failed: " + why);
                    else
                        wk.Path = Path.GetDirectoryName(wk.Path).Replace('\\', '/') + "/" + wk.NewName + Path.GetExtension(wk.Path);
                }
                done.Add(wk.Path);
            }
        }
        var w = new W();
        w.Begin();
        w.Key("ok").Bool(true);
        w.Key("done").Strings(done);
        w.Key("notes").Strings(notes);
        w.Key("previews").Strings(previews);
        w.End();
        return w.ToString();
    }

    sealed class Working
    {
        public readonly AnimationClip Clip;
        public string Path;
        public AnimationClipSettings Settings;
        public bool SettingsChanged;
        public float? FrameRate;
        public WrapMode? Wrap;
        public bool? Legacy;
        public string NewName;
        public List<AnimationEvent> Events;
        // Float curves by binding; a null curve removes it.
        public readonly Dictionary<EditorCurveBinding, AnimationCurve> Curves = new Dictionary<EditorCurveBinding, AnimationCurve>();
        public readonly Dictionary<EditorCurveBinding, ObjectReferenceKeyframe[]> Objects = new Dictionary<EditorCurveBinding, ObjectReferenceKeyframe[]>();

        public Working(AnimationClip clip, string path)
        {
            Clip = clip;
            Path = path;
            Settings = AnimationUtility.GetAnimationClipSettings(clip);
        }

        // The clip's float bindings as they will be after the planned changes.
        public List<EditorCurveBinding> Bindings()
        {
            var list = AnimationUtility.GetCurveBindings(Clip).Where(b => !Curves.ContainsKey(b) || Curves[b] != null).ToList();
            list.AddRange(Curves.Where(p => p.Value != null && !list.Contains(p.Key)).Select(p => p.Key));
            return list;
        }

        public List<EditorCurveBinding> ObjectBindings()
        {
            var list = AnimationUtility.GetObjectReferenceCurveBindings(Clip).Where(b => !Objects.ContainsKey(b) || Objects[b] != null).ToList();
            list.AddRange(Objects.Where(p => p.Value != null && !list.Contains(p.Key)).Select(p => p.Key));
            return list;
        }

        public AnimationCurve Curve(EditorCurveBinding b) =>
            Curves.TryGetValue(b, out var c) ? c : AnimationUtility.GetEditorCurve(Clip, b);

        public void Check()
        {
            if (Settings.startTime > Settings.stopTime)
                throw new EditFailed($"startTime {Settings.startTime} is after stopTime {Settings.stopTime}");
            if (FrameRate is float f && (f <= 0 || float.IsNaN(f) || float.IsInfinity(f)))
                throw new EditFailed("frameRate must be a positive number");
        }

        public void Apply(AnimationClip target)
        {
            var changed = Curves.Keys.ToArray();
            if (changed.Length > 0)
                AnimationUtility.SetEditorCurves(target, changed, changed.Select(b => Curves[b]).ToArray());
            foreach (var pair in Objects)
                AnimationUtility.SetObjectReferenceCurve(target, pair.Key, pair.Value);
            if (FrameRate != null)
                target.frameRate = FrameRate.Value;
            if (Wrap != null)
                target.wrapMode = Wrap.Value;
            if (Legacy != null)
                target.legacy = Legacy.Value;
            if (SettingsChanged)
                AnimationUtility.SetAnimationClipSettings(target, Settings);
            if (Events != null)
                AnimationUtility.SetAnimationEvents(target, Events.OrderBy(e => e.time).ToArray());
        }
    }

    static void Plan(Working work, string key, string op, string value)
    {
        if (key == "events")
        {
            PlanEvents(work, op, value);
            return;
        }
        if (key.StartsWith("path[", StringComparison.Ordinal) && key.EndsWith("]", StringComparison.Ordinal))
        {
            if (op != "=")
                throw new EditFailed($"{key} takes = (path[Old]=New)");
            Repath(work, key.Substring(5, key.Length - 6).Trim('/'), value.Trim().Trim('/'));
            return;
        }
        if (key == "curves")
        {
            if (op != "-=")
                throw new EditFailed("curves takes -= (curves-=path or curves-=path:property); add curves with keys[path:property]=...");
            var hits = Match(work, value, false);
            if (hits.floats.Count + hits.objects.Count == 0)
                throw new EditFailed($"no curve matches '{value}'; {Known(work)}");
            foreach (var q in hits.floats.Where(IsQuaternionPart).GroupBy(b => b.path + "\n" + Group(b.propertyName)))
            {
                var first = q.First();
                var all = work.Bindings().Count(b => b.path == first.path && IsQuaternionPart(b) && Group(b.propertyName) == Group(first.propertyName));
                if (q.Count() < all)
                    throw new EditFailed($"'{value}' removes part of a quaternion ({Group(first.propertyName)} on '{first.path}'); remove all four components (curves-={first.path}:{(Class(first) == "quat" ? "rotation" : Group(first.propertyName))})");
            }
            foreach (var b in hits.floats)
                work.Curves[b] = null;
            foreach (var b in hits.objects)
                work.Objects[b] = null;
            return;
        }
        if (key.StartsWith("keys[", StringComparison.Ordinal) && key.EndsWith("]", StringComparison.Ordinal))
        {
            PlanKeys(work, key.Substring(5, key.Length - 6), op, value);
            return;
        }
        if (op != "=")
            throw new EditFailed($"{key} takes =");
        switch (key.ToLowerInvariant())
        {
            case "framerate":
                work.FrameRate = ParseFloat(value, key);
                return;
            case "wrapmode":
                if (!Enum.TryParse<WrapMode>(value, true, out var wrap) || !Enum.IsDefined(typeof(WrapMode), wrap))
                    throw new EditFailed($"wrapMode is one of {string.Join(", ", Enum.GetNames(typeof(WrapMode)).Distinct())}");
                work.Wrap = wrap;
                return;
            case "legacy":
                work.Legacy = ParseBool(value, key);
                return;
            case "name":
                if (!AssetDatabase.IsMainAsset(work.Clip))
                    throw new EditFailed("only a clip that is its own .anim file can be renamed");
                if (value.Length == 0 || value.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                    throw new EditFailed($"'{value}' isn't a valid file name");
                var target = System.IO.Path.GetDirectoryName(work.Path).Replace('\\', '/') + "/" + value + System.IO.Path.GetExtension(work.Path);
                if (!string.Equals(target, work.Path, StringComparison.OrdinalIgnoreCase) && File.Exists(FileUtil.GetPhysicalPath(target)))
                    throw new EditFailed($"can't rename to {value}: {target} already exists");
                work.NewName = value;
                return;
            case "looppose":
                key = "loopBlend";
                break;
        }
        var field = typeof(AnimationClipSettings).GetFields().FirstOrDefault(f => string.Equals(f.Name, key, StringComparison.OrdinalIgnoreCase));
        if (field == null)
            throw new EditFailed($"unknown key '{key}'; {Suggest(key, s_SettingOrder.Concat(new[] { "frameRate", "wrapMode", "legacy", "name", "events", "path[...]", "curves", "keys[...]" }))}");
        object boxed = work.Settings;
        object parsed;
        if (field.FieldType == typeof(bool))
            parsed = ParseBool(value, key);
        else if (field.FieldType == typeof(float))
        {
            parsed = field.Name is "startTime" or "stopTime" or "additiveReferencePoseTime" ? Time(work, value) : ParseFloat(value, key);
            if (field.Name is "startTime" or "stopTime" && ((float)parsed < 0))
                throw new EditFailed($"{key} can't be negative");
        }
        else if (field.FieldType == typeof(AnimationClip))
        {
            if (value.Length == 0 || value.Equals("None", StringComparison.OrdinalIgnoreCase))
                parsed = null;
            else
            {
                var (p, sub) = SplitSubAsset(value);
                var clips = ClipsAt(p).ToList();
                parsed = sub == null ? clips.FirstOrDefault() : clips.FirstOrDefault(c => c.name == sub);
                if (parsed == null)
                    throw new EditFailed($"no clip '{value}' (an .anim path, or Model.fbx#Clip)");
            }
        }
        else
            throw new EditFailed($"{key} can't be set");
        field.SetValue(boxed, parsed);
        work.Settings = (AnimationClipSettings)boxed;
        work.SettingsChanged = true;
    }

    static (string path, string sub) SplitSubAsset(string value)
    {
        var at = value.LastIndexOf('#');
        if (at > 0 && !File.Exists(FileUtil.GetPhysicalPath(value)))
            return (value.Substring(0, at), value.Substring(at + 1));
        return (value, null);
    }

    // A time in seconds (0.5), frames (12f) or percent of the clip (50%).
    static float Time(Working work, string text)
    {
        text = text.Trim();
        if (text == "start")
            return work.Settings.startTime;
        if (text == "end")
            return work.Settings.stopTime;
        if (text.EndsWith("f", StringComparison.OrdinalIgnoreCase) && float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, Inv, out var frame))
            return frame / (work.FrameRate ?? work.Clip.frameRate);
        if (text.EndsWith("%", StringComparison.Ordinal) && float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, Inv, out var percent))
            return percent / 100f * work.Clip.length;
        if (text.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            text = text.Substring(0, text.Length - 1);
        if (float.TryParse(text, NumberStyles.Float, Inv, out var seconds) && !float.IsNaN(seconds) && !float.IsInfinity(seconds))
            return seconds;
        throw new EditFailed($"'{text}' isn't a time (seconds 0.5, frames 12f, or 50% of the clip)");
    }

    static float ParseFloat(string value, string key)
    {
        if (float.TryParse(value, NumberStyles.Float, Inv, out var f) && !float.IsNaN(f) && !float.IsInfinity(f))
            return f;
        throw new EditFailed($"{key} needs a number, not '{value}'");
    }

    static bool ParseBool(string value, string key)
    {
        switch (value.ToLowerInvariant())
        {
            case "true": case "1": case "on": case "yes": return true;
            case "false": case "0": case "off": case "no": return false;
        }
        throw new EditFailed($"{key} needs true or false, not '{value}'");
    }

    // ------------------------------------------------ events

    static void PlanEvents(Working work, string op, string value)
    {
        work.Events ??= AnimationUtility.GetAnimationEvents(work.Clip).ToList();
        if (op == "=")
        {
            if (value.Trim().Length > 0)
                throw new EditFailed("events= only clears the list; add events with events+=time:Function[:param]");
            work.Events.Clear();
            return;
        }
        var failures = new List<string>();
        foreach (var item in value.Split(';').Select(s => s.Trim()).Where(s => s.Length > 0))
        {
            try
            {
                PlanEvent(work, op, item);
            }
            catch (EditFailed e)
            {
                failures.Add(e.Message);
            }
        }
        if (failures.Count > 0)
            throw new EditFailed(string.Join("\n", failures));
    }

    static void PlanEvent(Working work, string op, string item)
    {
        {
            if (op == "-=")
            {
                var at = item.IndexOf('@');
                var name = at < 0 ? item : item.Substring(0, at);
                float? time = at < 0 ? (float?)null : Time(work, item.Substring(at + 1));
                var tolerance = 0.5f / work.Clip.frameRate;
                var removed = work.Events.RemoveAll(e => (name.Length == 0 || e.functionName == name) && (time == null || Mathf.Abs(e.time - time.Value) <= tolerance));
                if (removed == 0)
                    throw new EditFailed($"no event matches '{item}'; the clip has {(work.Events.Count == 0 ? "none" : string.Join(", ", work.Events.Select(e => e.functionName + "@" + e.time.ToString("0.###", Inv))))}");
                return;
            }
            if (op != "+=")
                throw new EditFailed("events takes += (add), -= (remove) or = (clear)");
            var parts = item.Split(new[] { ':' }, 3);
            if (parts.Length < 2 || parts[1].Trim().Length == 0)
                throw new EditFailed($"'{item}': an event is time:Function[:param] (time in seconds, 12f frames, or 50%)");
            var e2 = new AnimationEvent { time = Time(work, parts[0]), functionName = parts[1].Trim() };
            if (e2.time < 0 || e2.time > work.Clip.length + 1e-4f)
                throw new EditFailed($"event time {e2.time.ToString("0.###", Inv)}s is outside the clip (0..{work.Clip.length.ToString("0.###", Inv)}s), so it would never fire");
            if (!System.Text.RegularExpressions.Regex.IsMatch(e2.functionName, @"^[\p{L}_][\p{L}\p{Nd}_]*$"))
                throw new EditFailed($"'{e2.functionName}' isn't a method name");
            if (parts.Length == 3)
            {
                var p = parts[2];
                if (p.Length >= 2 && p[0] == '"' && p[p.Length - 1] == '"')
                    e2.stringParameter = p.Substring(1, p.Length - 2);
                else if (int.TryParse(p, NumberStyles.Integer, Inv, out var n))
                    e2.intParameter = n;
                else if (float.TryParse(p, NumberStyles.Float, Inv, out var f))
                    e2.floatParameter = f;
                else if (p.StartsWith("Assets/", StringComparison.Ordinal) || p.StartsWith("Packages/", StringComparison.Ordinal))
                {
                    var (ap, sub) = SplitSubAsset(p);
                    var obj = sub == null ? AssetDatabase.LoadMainAssetAtPath(ap) : AssetDatabase.LoadAllAssetsAtPath(ap).FirstOrDefault(o => o.name == sub);
                    e2.objectReferenceParameter = obj != null ? obj : throw new EditFailed($"no asset '{p}' for the event's object parameter");
                }
                else
                    e2.stringParameter = p;
            }
            work.Events.Add(e2);
        }
    }

    // ------------------------------------------------ curves

    static void Repath(Working work, string from, string to)
    {
        bool Under(string p) => p == from || from.Length == 0 || p.StartsWith(from + "/", StringComparison.Ordinal);
        string Moved(string p) => from.Length == 0 ? (p.Length == 0 ? to : to.Length == 0 ? p : to + "/" + p) : to + p.Substring(from.Length);
        var floats = work.Bindings().Where(b => Under(b.path) && b.type != typeof(Animator)).ToList();
        var objects = work.ObjectBindings().Where(b => Under(b.path)).ToList();
        if (floats.Count + objects.Count == 0)
            throw new EditFailed($"no curve has path {(from.Length == 0 ? "'' (the root)" : "'" + from + "'")}; {Paths(work)}");
        var existing = new HashSet<string>(work.Bindings().Concat(work.ObjectBindings()).Where(b => !Under(b.path)).Select(b => b.path + ":" + b.type?.Name + "." + b.propertyName));
        var rotations = work.Bindings().Where(b => !Under(b.path) && Class(b) is "euler" or "quat").ToLookup(b => b.path, Class);
        foreach (var b in floats.Where(b => Class(b) is "euler" or "quat"))
        {
            var dest = Moved(b.path).Trim('/');
            if (rotations[dest].Any(c => c != Class(b)))
                throw new EditFailed($"path[{from}]={to} would put {(Class(b) == "quat" ? "quaternion" : "euler")} rotation curves on '{dest}', which already has {(Class(b) == "quat" ? "euler" : "quaternion")} rotation curves");
        }
        foreach (var b in floats)
        {
            var moved = b;
            moved.path = Moved(b.path).Trim('/');
            if (existing.Contains(moved.path + ":" + b.type?.Name + "." + b.propertyName))
                throw new EditFailed($"path[{from}]={to} would collide with an existing curve on '{moved.path}' ({b.propertyName})");
            var curve = work.Curve(b);
            work.Curves[b] = null;
            work.Curves[moved] = curve;
        }
        foreach (var b in objects)
        {
            var moved = b;
            moved.path = Moved(b.path).Trim('/');
            var keys = work.Objects.TryGetValue(b, out var k) ? k : AnimationUtility.GetObjectReferenceCurve(work.Clip, b);
            work.Objects[b] = null;
            work.Objects[moved] = keys;
        }
    }

    static string Paths(Working work)
    {
        var paths = work.Bindings().Concat(work.ObjectBindings()).Select(b => b.path).Where(p => p.Length > 0).Distinct().OrderBy(p => p, StringComparer.Ordinal).ToList();
        return paths.Count == 0 ? "the clip animates only its root" : "paths: " + string.Join(", ", paths.Take(12)) + (paths.Count > 12 ? $", +{paths.Count - 12} more" : "");
    }

    static string Known(Working work) => Paths(work);

    static readonly Dictionary<string, string> s_Short = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["position"] = "m_LocalPosition", ["rotation"] = "localEulerAnglesRaw", ["scale"] = "m_LocalScale", ["active"] = "m_IsActive",
    };

    // The bindings a "path:property" text names. property may be a group (position) or one component (position.x), a
    // Unity property name (m_LocalPosition.x), Type.property (SkinnedMeshRenderer.blendShape.Smile), or a bare name
    // matched against the clip's own curves (a muscle or parameter). With no ':', every curve on the path.
    static (List<EditorCurveBinding> floats, List<EditorCurveBinding> objects) Match(Working work, string text, bool exact)
    {
        var colon = text.LastIndexOf(':');
        var path = (colon < 0 ? text : text.Substring(0, colon)).Trim().Trim('/');
        var prop = colon < 0 ? null : text.Substring(colon + 1).Trim();
        bool Hit(EditorCurveBinding b)
        {
            if (b.path != path)
                return false;
            if (prop == null)
                return true;
            var names = Names(b);
            return names.Any(n => string.Equals(n, prop, StringComparison.OrdinalIgnoreCase) || !exact && string.Equals(Group(n), prop, StringComparison.OrdinalIgnoreCase));
        }
        return (work.Bindings().Where(Hit).ToList(), work.ObjectBindings().Where(Hit).ToList());
    }

    static IEnumerable<string> Names(EditorCurveBinding b)
    {
        yield return b.propertyName;
        yield return b.type?.Name + "." + b.propertyName;
        var cls = b.isPPtrCurve ? "pptr" : Class(b);
        var comp = Component(b.propertyName);
        switch (cls)
        {
            case "pos": yield return "position" + comp; break;
            case "euler": case "quat": yield return "rotation" + comp; break;
            case "scale": yield return "scale" + comp; break;
            case "active": yield return "active"; break;
        }
        if (b.propertyName == "m_Enabled")
            yield return b.type?.Name + ".enabled";
    }

    // A component of a quaternion: Transform rotation, or the humanoid root, motion and IK goal rotations.
    static bool IsQuaternionPart(EditorCurveBinding b)
    {
        var cls = Class(b);
        return cls == "quat" || cls is "root" or "motion" or "ik" && Group(b.propertyName).EndsWith("Q", StringComparison.Ordinal);
    }

    static string Component(string property)
    {
        var dot = property.LastIndexOf('.');
        return dot > 0 && property.Length - dot == 2 ? property.Substring(dot) : "";
    }

    static string Group(string name)
    {
        var dot = name.LastIndexOf('.');
        return dot > 0 && name.Length - dot == 2 && "xyzwrgba".IndexOf(name[dot + 1]) >= 0 ? name.Substring(0, dot) : name;
    }

    static void PlanKeys(Working work, string binding, string op, string value)
    {
        var (floats, objects) = Match(work, binding, false);
        if (objects.Count > 0)
            throw new EditFailed($"keys[{binding}] is an object curve; object keys can't be set yet");
        var colon = binding.LastIndexOf(':');
        if (colon < 0)
            throw new EditFailed($"keys[{binding}] needs path:property (Hips:position, Body:SkinnedMeshRenderer.blendShape.Smile, :GlowIntensity)");
        var path = binding.Substring(0, colon).Trim().Trim('/');
        var prop = binding.Substring(colon + 1).Trim();
        if (floats.Count == 0)
        {
            if (op == "-=")
                throw new EditFailed($"no curve '{binding}' to remove keys from; {Known(work)}");
            floats = NewBindings(work, path, prop);
        }
        floats = floats.OrderBy(b => b.propertyName, StringComparer.Ordinal).ToList();
        // Rotations stored as quaternions (Transform rotation, humanoid root/motion/IK goal Q) take euler degrees.
        var quaternion = floats.Count == 4 && floats.All(IsQuaternionPart);
        if (quaternion)
            floats = new[] { ".x", ".y", ".z", ".w" }.Select(c => floats.First(b => b.propertyName.EndsWith(c, StringComparison.Ordinal))).ToList();
        else if (floats.Count > 1)
            floats = floats.OrderBy(b => "xyzwrgba".IndexOf(b.propertyName[b.propertyName.Length - 1])).ToList();
        var width = quaternion ? 3 : floats.Count;
        var stepped = floats.All(b => b.isDiscreteCurve || b.propertyName == "m_IsActive" || b.propertyName == "m_Enabled");
        var curves = floats.Select(b => work.Curve(b) ?? new AnimationCurve()).Select(c => new AnimationCurve(c.keys) { preWrapMode = c.preWrapMode, postWrapMode = c.postWrapMode }).ToList();
        var nearby = 0.5f / work.Clip.frameRate;
        var added = new List<float>();
        if (op == "=")
            foreach (var c in curves)
                c.keys = new Keyframe[0];
        var failures = new List<string>();
        foreach (var item in value.Split(';').Select(s => s.Trim()).Where(s => s.Length > 0))
        {
            try
            {
                if (op == "-=")
                {
                    var t = Time(work, item);
                    var any = false;
                    foreach (var c in curves)
                    {
                        var index = Array.FindIndex(c.keys, k => Mathf.Abs(k.time - t) <= nearby);
                        if (index >= 0)
                        {
                            c.RemoveKey(index);
                            any = true;
                        }
                    }
                    if (!any)
                        throw new EditFailed($"keys[{binding}] has no key at {item}; keys are at {string.Join(", ", curves[0].keys.Select(k => k.time.ToString("0.###", Inv)).Take(12))}");
                    continue;
                }
                if (op != "+=" && op != "=")
                    throw new EditFailed($"keys[...] takes = (replace all), += (add keys, replacing one at the same time) or -= (remove keys at times)");
                var colonAt = item.IndexOf(':');
                if (colonAt <= 0)
                    throw new EditFailed($"'{item}': a key is time:value (0.5:1.2, or 0.5:0,1,0 for position/rotation/scale)");
                var time = Time(work, item.Substring(0, colonAt));
                if (time < 0)
                    throw new EditFailed($"'{item}': key time can't be negative");
                var text = item.Substring(colonAt + 1).Trim();
                // +(dx,dy,dz) offsets the value the curve has at that time; for rotations, the Transform's euler angles.
                var relative = text.StartsWith("+(", StringComparison.Ordinal) && text.EndsWith(")", StringComparison.Ordinal);
                if (relative)
                    text = text.Substring(2, text.Length - 3);
                var numbers = text.Split(',').Select(s => ParseFloat(s.Trim(), $"keys[{binding}]")).ToArray();
                if (numbers.Length != width)
                    throw new EditFailed($"keys[{binding}] animates {width} value{(width == 1 ? "" : "s")} ({(quaternion ? "rotation in degrees" : string.Join(", ", floats.Select(b => b.propertyName)))}), so each key needs {width}, not {numbers.Length}");
                if (relative && curves.Any(c => c.length == 0))
                    throw new EditFailed($"keys[{binding}]: +(...) offsets the current value, but the curve has no keys yet");
                if (relative && quaternion)
                {
                    var now = new Quaternion(curves[0].Evaluate(time), curves[1].Evaluate(time), curves[2].Evaluate(time), curves[3].Evaluate(time)).normalized.eulerAngles;
                    numbers = new[] { now.x + numbers[0], now.y + numbers[1], now.z + numbers[2] };
                }
                else if (relative)
                    numbers = numbers.Select((n, c) => curves[c].Evaluate(time) + n).ToArray();
                var values = numbers;
                if (quaternion)
                {
                    var q = Quaternion.Euler(numbers[0], numbers[1], numbers[2]);
                    values = new[] { q.x, q.y, q.z, q.w };
                }
                for (var c = 0; c < curves.Count; c++)
                {
                    var index = Array.FindIndex(curves[c].keys, k => Mathf.Abs(k.time - time) <= 1e-4f);
                    if (index >= 0)
                        curves[c].RemoveKey(index);
                    var at = curves[c].AddKey(new Keyframe(time, values[c]));
                    var mode = stepped ? AnimationUtility.TangentMode.Constant : AnimationUtility.TangentMode.ClampedAuto;
                    AnimationUtility.SetKeyLeftTangentMode(curves[c], at, mode);
                    AnimationUtility.SetKeyRightTangentMode(curves[c], at, mode);
                }
                added.Add(time);
            }
            catch (EditFailed e)
            {
                failures.Add(e.Message);
            }
        }
        if (failures.Count > 0)
            throw new EditFailed(string.Join("\n", failures));
        if (quaternion)
            KeepHemisphere(curves, added);
        var update = typeof(AnimationUtility).GetMethod("UpdateTangentsFromMode", Any, null, new[] { typeof(AnimationCurve) }, null);
        for (var c = 0; c < curves.Count; c++)
        {
            update?.Invoke(null, new object[] { curves[c] });
            work.Curves[floats[c]] = curves[c].length == 0 ? null : curves[c];
        }
    }

    // q and -q are the same rotation, but interpolating between keys on opposite hemispheres turns the long way round.
    // Each new key takes the sign that faces its previous key (its next one, when it is first).
    static void KeepHemisphere(List<AnimationCurve> curves, List<float> added)
    {
        var keys = curves.Select(c => c.keys).ToArray();
        var count = keys[0].Length;
        if (keys.Any(k => k.Length != count))
            return;
        for (var i = 0; i < count; i++)
        {
            if (!added.Any(t => Mathf.Abs(t - keys[0][i].time) <= 1e-4f))
                continue;
            var neighbour = i > 0 ? i - 1 : i + 1;
            if (neighbour >= count)
                continue;
            var dot = 0f;
            for (var c = 0; c < 4; c++)
                dot += keys[c][i].value * keys[c][neighbour].value;
            if (dot >= 0)
                continue;
            for (var c = 0; c < 4; c++)
            {
                var k = keys[c][i];
                k.value = -k.value;
                curves[c].MoveKey(i, k);
            }
        }
    }


    // Bindings for a curve the clip doesn't have yet.
    static List<EditorCurveBinding> NewBindings(Working work, string path, string prop)
    {
        var dot = prop.IndexOf('.');
        var head = dot < 0 ? prop : prop.Substring(0, dot);
        var comp = dot < 0 ? null : prop.Substring(dot + 1);
        if (s_Short.TryGetValue(head, out var unity))
        {
            var type = head.Equals("active", StringComparison.OrdinalIgnoreCase) ? typeof(GameObject) : typeof(Transform);
            if (type == typeof(GameObject))
                return new List<EditorCurveBinding> { EditorCurveBinding.FloatCurve(path, type, unity) };
            if (head.Equals("rotation", StringComparison.OrdinalIgnoreCase) && work.Bindings().Any(b => b.path == path && Class(b) == "quat"))
                throw new EditFailed($"'{path}' has quaternion rotation curves; set all of rotation at once (keys[{path}:rotation]=t:x,y,z in degrees)");
            var axes = comp == null ? new[] { "x", "y", "z" } : comp.Length == 1 && "xyz".Contains(comp) ? new[] { comp } : throw new EditFailed($"{head} has components x, y and z");
            return axes.Select(a => EditorCurveBinding.FloatCurve(path, type, unity + "." + a)).ToList();
        }
        if (comp != null)
        {
            var types = TypeCache.GetTypesDerivedFrom<Component>().Where(t => t.Name == head || t.FullName == head).ToList();
            if (types.Count == 1)
                return new List<EditorCurveBinding> { EditorCurveBinding.FloatCurve(path, types[0], comp == "enabled" ? "m_Enabled" : comp) };
            if (types.Count > 1)
                throw new EditFailed($"'{head}' names {types.Count} component types; use the full name ({string.Join(", ", types.Take(4).Select(t => t.FullName))})");
            if (char.IsUpper(head[0]) && path.Length > 0)
                throw new EditFailed($"no component type '{head}' in the project (for '{prop}')");
        }
        if (path.Length == 0)
        {
            s_Muscles ??= new HashSet<string>(HumanTrait.MuscleName.Concat(FingerMuscles()));
            // A bare name on the root is a muscle or an Animator parameter, as the Animation window records them.
            return new List<EditorCurveBinding> { EditorCurveBinding.FloatCurve("", typeof(Animator), prop) };
        }
        throw new EditFailed($"can't tell what '{prop}' animates; use position, rotation, scale, active, or Type.property (SkinnedMeshRenderer.blendShape.Smile, MeshRenderer.material._Color.r, Light.m_Intensity)");
    }

    // ---------------------------------------------------------------- copy

    // {"path","id","dest"}: writes an .anim copy of a clip (curves, settings, events).
    public static string Copy(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var clip = FindClip((string)root["path"], (string)root["id"]);
        var dest = (string)root["dest"];
        if (!dest.StartsWith("Assets/", StringComparison.Ordinal) || !dest.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
            return Error("the copy goes to Assets/.../Name.anim");
        if (File.Exists(FileUtil.GetPhysicalPath(dest)))
            return Error(dest + " already exists");
        var dir = Path.GetDirectoryName(dest).Replace('\\', '/');
        EnsureFolder(dir);
        var copy = UnityEngine.Object.Instantiate(clip);
        copy.name = Path.GetFileNameWithoutExtension(dest);
        try
        {
            AssetDatabase.CreateAsset(copy, dest);
        }
        catch (Exception e)
        {
            return Error("couldn't create " + dest + ": " + e.Message);
        }
        // Setting the curves again rebuilds what Unity derives from them (stats, loop match, average velocity).
        var bindings = AnimationUtility.GetCurveBindings(copy);
        if (bindings.Length > 0)
            AnimationUtility.SetEditorCurves(copy, bindings, bindings.Select(b => AnimationUtility.GetEditorCurve(copy, b)).ToArray());
        AnimationUtility.SetAnimationClipSettings(copy, AnimationUtility.GetAnimationClipSettings(clip));
        EditorUtility.SetDirty(copy);
        AssetDatabase.SaveAssets();
        var w = new W();
        w.Begin().Key("ok").Bool(true).Key("path").Str(dest).End();
        return w.ToString();
    }

    static void EnsureFolder(string dir)
    {
        if (AssetDatabase.IsValidFolder(dir))
            return;
        if (Directory.Exists(FileUtil.GetPhysicalPath(dir)))
        {
            AssetDatabase.ImportAsset(dir);
            if (AssetDatabase.IsValidFolder(dir))
                return;
        }
        var slash = dir.LastIndexOf('/');
        EnsureFolder(dir.Substring(0, slash));
        AssetDatabase.CreateFolder(dir.Substring(0, slash), dir.Substring(slash + 1));
    }

    // ---------------------------------------------------------------- formatting

    static string TypeName(Type t) => t == typeof(bool) ? "bool" : t == typeof(float) ? "float" : t == typeof(int) ? "int" : typeof(UnityEngine.Object).IsAssignableFrom(t) ? "asset" : t.Name;

    static string Format(object v)
    {
        switch (v)
        {
            case null: return "None";
            case bool b: return b ? "true" : "false";
            case float f: return f.ToString("R", Inv);
            case int n: return n.ToString(Inv);
            case string s: return s;
            case UnityEngine.Object o: return o == null ? "None" : ObjectPath(o);
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

    static string Suggest(string key, IEnumerable<string> names)
    {
        var best = names.OrderBy(n => Distance(key.ToLowerInvariant(), n.ToLowerInvariant())).First();
        return Distance(key.ToLowerInvariant(), best.ToLowerInvariant()) <= Math.Max(2, key.Length / 3) ? $"did you mean '{best}'?" : "see `clipwaitress help set`";
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
