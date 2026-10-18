// The Editor half of prefabwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/PrefabWaitress/,
// and runs it through `unity command run_script`, so nothing is imported into the project. Every edit goes through
// SerializedObject and PrefabUtility, so Unity records and writes the overrides itself.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrefabWaitressEditor
{
    public class Request
    {
        public string file;
        public bool saveOpen;
        public Op[] ops;
    }

    public class Op
    {
        public string op;
        public int[] path;
        public string[] names;
        public int component = -1;
        public string componentType;
        public string property;
        public string value;
        public string to;
        public int[] refPath;
        public int refComponent = -1;
        public bool hasRef;
        public bool all;
        public string removedType;
        public long[] modTargets;
        public string[] modPaths;
        public string[] modKinds;
    }

    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const InteractionMode Mode = InteractionMode.AutomatedAction;

    public static string Edit(string json)
    {
        var request = ParseRequest(json);
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return Error("the Editor is in Play Mode; edits to scenes and prefabs would be lost");
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling; try again when it finishes");
        var done = new List<string>();
        try
        {
            if (request.file.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                EditPrefab(request, done);
            else if (request.file.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                EditScene(request, done);
            else
                return Error(request.file + " is not a scene or prefab");
        }
        catch (EditFailed e)
        {
            return Error(e.Message, done);
        }
        catch (Exception e)
        {
            return Error(e.GetType().Name + ": " + e.Message, done);
        }
        return "{\"ok\":true,\"done\":[" + string.Join(",", done.Select(Quote)) + "]}";
    }

    sealed class EditFailed : Exception
    {
        public EditFailed(string message) : base(message) { }
    }

    static void EditPrefab(Request request, List<string> done)
    {
        var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (stage != null && stage.assetPath == request.file && stage.scene.isDirty)
            throw new EditFailed($"{request.file} is open in Prefab Mode with unsaved changes; save or close it first");
        var root = PrefabUtility.LoadPrefabContents(request.file);
        try
        {
            var roots = new List<GameObject> { root };
            foreach (var op in request.ops)
                Validate(op, roots);
            foreach (var op in request.ops)
                Apply(op, roots, done);
            PrefabUtility.SaveAsPrefabAsset(root, request.file, out var saved);
            if (!saved)
                throw new EditFailed("Unity did not save " + request.file);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void EditScene(Request request, List<string> done)
    {
        var scene = SceneManager.GetSceneByPath(request.file);
        var opened = false;
        if (scene.IsValid() && scene.isLoaded)
        {
            if (scene.isDirty && !request.saveOpen)
                throw new EditFailed($"{request.file} is open in the Editor with unsaved changes; save it first, or pass --save-open to save them along with this edit");
        }
        else
        {
            scene = EditorSceneManager.OpenScene(request.file, OpenSceneMode.Additive);
            opened = true;
        }
        try
        {
            var roots = scene.GetRootGameObjects().ToList();
            // Every lookup and value is checked before anything changes, so a bad request leaves an open scene untouched.
            foreach (var op in request.ops)
                Validate(op, roots);
            foreach (var op in request.ops)
                Apply(op, roots, done);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
                throw new EditFailed("Unity did not save " + request.file);
        }
        finally
        {
            if (opened)
                EditorSceneManager.CloseScene(scene, true);
        }
    }

    static GameObject Find(List<GameObject> roots, int[] path, string[] names)
    {
        if (path == null || path.Length == 0 || path[0] >= roots.Count)
            throw new EditFailed("object not found; the file may have changed since it was read");
        var go = roots[path[0]];
        Check(go, names, 0);
        for (var i = 1; i < path.Length; i++)
        {
            if (path[i] >= go.transform.childCount)
                throw new EditFailed($"{go.name} has no child #{path[i]}; the file may have changed since it was read");
            go = go.transform.GetChild(path[i]).gameObject;
            Check(go, names, i);
        }
        return go;
    }

    static void Check(GameObject go, string[] names, int i)
    {
        if (names != null && i < names.Length && go.name != names[i])
            throw new EditFailed($"expected {names[i]} but found {go.name}; the file may have changed since it was read, or the Editor holds unsaved changes");
    }

    static UnityEngine.Object Target(GameObject go, int component, string type)
    {
        if (component < 0)
            return go;
        var comps = go.GetComponents<Component>();
        if (component >= comps.Length || comps[component] == null)
            throw new EditFailed($"{go.name} has no component #{component}");
        return comps[component];
    }

    static void Validate(Op op, List<GameObject> roots)
    {
        var go = Find(roots, op.path, op.names);
        var target = Target(go, op.component, op.componentType);
        if (op.property == null)
            return;
        var so = new SerializedObject(target);
        var sp = so.FindProperty(op.property) ?? throw new EditFailed($"{target.GetType().Name} has no property {op.property}");
        if (op.op == "set")
            Assign(sp, op, roots);
    }

    static void Apply(Op op, List<GameObject> roots, List<string> done)
    {
        var go = Find(roots, op.path, op.names);
        var target = Target(go, op.component, op.componentType);
        switch (op.op)
        {
            case "set":
            {
                var so = new SerializedObject(target);
                var sp = so.FindProperty(op.property) ?? throw new EditFailed($"{target.GetType().Name} has no property {op.property}");
                Assign(sp, op, roots);
                so.ApplyModifiedPropertiesWithoutUndo();
                done.Add($"set {op.property}");
                break;
            }
            case "revert":
                Revert(op, go, target, done);
                break;
            case "apply":
                ApplyToPrefab(op, go, target, done);
                break;
            case "dropmods":
            {
                // Each candidate is confirmed here: an "unused" override is dropped only if its target or property is
                // really gone, and a "redundant" one only if its value equals the source's.
                var mods = PrefabUtility.GetPropertyModifications(go) ?? Array.Empty<PropertyModification>();
                var keep = new List<PropertyModification>();
                var dropped = 0;
                var kept = 0;
                foreach (var m in mods)
                {
                    var id = 0L;
                    if (m.target != null)
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m.target, out string _, out id);
                    var kind = -1;
                    for (var i = 0; i < op.modPaths.Length && kind < 0; i++)
                        if (m.propertyPath == op.modPaths[i] && (m.target == null ? op.modTargets[i] == 0 : id == op.modTargets[i]))
                            kind = i;
                    if (kind < 0)
                    {
                        keep.Add(m);
                        continue;
                    }
                    // The property is looked up on the instance's own object: list elements this instance added exist only
                    // there. SerializeReference paths can't be looked up by name, so they are always kept.
                    var mine = m.target == null ? null : Instanced(go, m.target);
                    var confirmed = m.propertyPath.Contains("managedReferences[")
                        ? false
                        : op.modKinds[kind] == "unused"
                            ? m.target == null || (mine == null ? new SerializedObject(m.target) : new SerializedObject(mine)).FindProperty(m.propertyPath) == null
                            : SameAsSource(m);
                    if (confirmed)
                        dropped++;
                    else
                    {
                        kept++;
                        keep.Add(m);
                    }
                }
                PrefabUtility.SetPropertyModifications(go, keep.ToArray());
                done.Add($"removed {dropped} overrides from {go.name}" + (kept > 0 ? $"; kept {kept} the Editor says still matter" : ""));
                break;
            }
            default:
                throw new EditFailed("unknown edit " + op.op);
        }
    }

    static void Revert(Op op, GameObject go, UnityEngine.Object target, List<string> done)
    {
        if (op.removedType != null)
        {
            var root = PrefabUtility.GetOutermostPrefabInstanceRoot(go) ?? throw new EditFailed(go.name + " is not in a prefab instance");
            var removed = PrefabUtility.GetRemovedComponents(root).FirstOrDefault(r => r.containingInstanceGameObject == go && Named(r.assetComponent, op.removedType));
            if (removed == null)
            {
                var gone = RemovedObject(root, go, op.removedType);
                gone.Revert(Mode);
                done.Add($"restored {op.removedType} under {go.name}");
                return;
            }
            removed.Revert(Mode);
            done.Add($"restored {op.removedType} on {go.name}");
            return;
        }
        if (op.property != null)
        {
            var so = new SerializedObject(target);
            var sp = so.FindProperty(op.property) ?? throw new EditFailed($"{target.GetType().Name} has no property {op.property}");
            if (!sp.prefabOverride && !sp.hasChildren)
                throw new EditFailed($"{op.property} is not overridden here");
            PrefabUtility.RevertPropertyOverride(sp, Mode);
            done.Add($"reverted {op.property}");
            return;
        }
        if (target is Component c)
        {
            if (PrefabUtility.IsAddedComponentOverride(c))
            {
                var type = c.GetType().Name;
                PrefabUtility.RevertAddedComponent(c, Mode);
                done.Add($"removed added {type}");
            }
            else
            {
                PrefabUtility.RevertObjectOverride(c, Mode);
                done.Add($"reverted {c.GetType().Name}");
            }
            return;
        }
        if (PrefabUtility.IsAddedGameObjectOverride(go))
        {
            var name = go.name;
            PrefabUtility.RevertAddedGameObject(go, Mode);
            done.Add($"removed added {name}");
            return;
        }
        if (op.all)
        {
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(go) && PrefabUtility.GetNearestPrefabInstanceRoot(go) != go)
                throw new EditFailed($"{go.name} is not a prefab instance root; --all reverts a whole instance");
            PrefabUtility.RevertPrefabInstance(go, Mode);
            done.Add($"reverted every override of {go.name}");
            return;
        }
        PrefabUtility.RevertObjectOverride(go, Mode);
        foreach (var comp in go.GetComponents<Component>())
            if (comp != null && !PrefabUtility.IsAddedComponentOverride(comp))
                PrefabUtility.RevertObjectOverride(comp, Mode);
        done.Add($"reverted {go.name} and its components");
    }

    static void ApplyToPrefab(Op op, GameObject go, UnityEngine.Object target, List<string> done)
    {
        var assetPath = string.IsNullOrEmpty(op.to) ? PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go) : op.to;
        if (string.IsNullOrEmpty(assetPath))
            throw new EditFailed(go.name + " is not in a prefab instance");
        if (op.removedType != null)
        {
            var root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
            var removed = PrefabUtility.GetRemovedComponents(root).FirstOrDefault(r => r.containingInstanceGameObject == go && Named(r.assetComponent, op.removedType));
            if (removed == null)
            {
                var gone = RemovedObject(root, go, op.removedType);
                gone.Apply(assetPath, Mode);
                done.Add($"removed {op.removedType} from {assetPath}");
                return;
            }
            removed.Apply(assetPath, Mode);
            done.Add($"removed {op.removedType} from {assetPath}");
            return;
        }
        if (op.property != null)
        {
            var so = new SerializedObject(target);
            var sp = so.FindProperty(op.property) ?? throw new EditFailed($"{target.GetType().Name} has no property {op.property}");
            PrefabUtility.ApplyPropertyOverride(sp, assetPath, Mode);
            done.Add($"applied {op.property} to {assetPath}");
            return;
        }
        if (target is Component c)
        {
            if (PrefabUtility.IsAddedComponentOverride(c))
                PrefabUtility.ApplyAddedComponent(c, assetPath, Mode);
            else
                PrefabUtility.ApplyObjectOverride(c, assetPath, Mode);
            done.Add($"applied {c.GetType().Name} to {assetPath}");
            return;
        }
        if (PrefabUtility.IsAddedGameObjectOverride(go))
        {
            PrefabUtility.ApplyAddedGameObject(go, assetPath, Mode);
            done.Add($"applied added {go.name} to {assetPath}");
            return;
        }
        if (op.all)
        {
            PrefabUtility.ApplyPrefabInstance(go, Mode);
            done.Add($"applied every override of {go.name}");
            return;
        }
        PrefabUtility.ApplyObjectOverride(go, assetPath, Mode);
        foreach (var comp in go.GetComponents<Component>())
            if (comp != null && !PrefabUtility.IsAddedComponentOverride(comp))
                PrefabUtility.ApplyObjectOverride(comp, assetPath, Mode);
        done.Add($"applied {go.name} and its components to {assetPath}");
    }

    // A GameObject the instance removed, by name, directly under go (or anywhere in the instance when unique).
    static RemovedGameObject RemovedObject(GameObject root, GameObject go, string name)
    {
        var all = PrefabUtility.GetRemovedGameObjects(root).Where(r => r.assetGameObject != null && r.assetGameObject.name == name).ToList();
        var here = all.Where(r => r.parentOfRemovedGameObjectInInstance == go).ToList();
        if (here.Count == 1)
            return here[0];
        if (all.Count == 1)
            return all[0];
        throw new EditFailed(all.Count == 0 ? $"no removed component or object named {name} on {go.name}" : $"{all.Count} removed objects are named {name}; name the object they were under");
    }

    // The object in this instance that comes from a source object.
    static UnityEngine.Object Instanced(GameObject root, UnityEngine.Object source)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) == source)
                return t.gameObject;
            foreach (var c in t.GetComponents<Component>())
                if (c != null && PrefabUtility.GetCorrespondingObjectFromSource(c) == source)
                    return c;
        }
        return null;
    }

    // True when an override sets the value its target (the source object) already has.
    static bool SameAsSource(PropertyModification m)
    {
        if (m.target == null)
            return false;
        var sp = new SerializedObject(m.target).FindProperty(m.propertyPath);
        if (sp == null)
            return false;
        switch (sp.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
                return sp.objectReferenceValue == m.objectReference;
            case SerializedPropertyType.Float:
                return double.TryParse(m.value, NumberStyles.Float, Inv, out var d) && (sp.numericType == SerializedPropertyNumericType.Double ? sp.doubleValue == d : sp.floatValue == (float)d);
            case SerializedPropertyType.Boolean:
                return (m.value == "1" || m.value == "true") == sp.boolValue;
            case SerializedPropertyType.Integer:
            case SerializedPropertyType.Enum:
            case SerializedPropertyType.LayerMask:
            case SerializedPropertyType.ArraySize:
            case SerializedPropertyType.Character:
                return long.TryParse(m.value, NumberStyles.Integer, Inv, out var l) && sp.longValue == l;
            case SerializedPropertyType.String:
                return m.value == sp.stringValue;
            default:
                return false;
        }
    }

    static bool Named(UnityEngine.Object o, string type)
    {
        for (var t = o.GetType(); t != null; t = t.BaseType)
            if (t.Name == type)
                return true;
        return false;
    }

    static void Assign(SerializedProperty sp, Op op, List<GameObject> roots)
    {
        try
        {
            AssignValue(sp, op, roots);
        }
        catch (Exception e) when (e is FormatException || e is OverflowException)
        {
            throw new EditFailed($"'{op.value}' is not a valid {sp.propertyType.ToString().ToLowerInvariant()} for {sp.propertyPath}" + (sp.propertyType == SerializedPropertyType.Integer && (op.value ?? "").Contains(".") ? " (it holds whole numbers)" : ""));
        }
    }

    static void AssignValue(SerializedProperty sp, Op op, List<GameObject> roots)
    {
        var v = op.value ?? "";
        switch (sp.propertyType)
        {
            case SerializedPropertyType.Integer:
            case SerializedPropertyType.LayerMask:
            case SerializedPropertyType.ArraySize:
            case SerializedPropertyType.Character:
                sp.longValue = long.Parse(v, Inv);
                break;
            case SerializedPropertyType.Enum:
                var names = sp.enumNames;
                var index = Array.FindIndex(names, n => string.Equals(n, v, StringComparison.OrdinalIgnoreCase) || string.Equals(n.Replace(" ", ""), v, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                    sp.enumValueIndex = index;
                else
                    sp.intValue = int.Parse(v, Inv);
                break;
            case SerializedPropertyType.Boolean:
                sp.boolValue = v == "1" || string.Equals(v, "true", StringComparison.OrdinalIgnoreCase);
                break;
            case SerializedPropertyType.Float:
                sp.doubleValue = double.Parse(v, Inv);
                break;
            case SerializedPropertyType.String:
                sp.stringValue = v;
                break;
            case SerializedPropertyType.Vector2:
                sp.vector2Value = new Vector2(N(v, 0), N(v, 1));
                break;
            case SerializedPropertyType.Vector3:
                sp.vector3Value = new Vector3(N(v, 0), N(v, 1), N(v, 2));
                break;
            case SerializedPropertyType.Vector4:
                sp.vector4Value = new Vector4(N(v, 0), N(v, 1), N(v, 2), N(v, 3));
                break;
            case SerializedPropertyType.Quaternion:
                sp.quaternionValue = new Quaternion(N(v, 0), N(v, 1), N(v, 2), N(v, 3));
                break;
            case SerializedPropertyType.Color:
                sp.colorValue = new Color(N(v, 0), N(v, 1), N(v, 2), Count(v) > 3 ? N(v, 3) : 1f);
                break;
            case SerializedPropertyType.Vector2Int:
                sp.vector2IntValue = new Vector2Int((int)N(v, 0), (int)N(v, 1));
                break;
            case SerializedPropertyType.Vector3Int:
                sp.vector3IntValue = new Vector3Int((int)N(v, 0), (int)N(v, 1), (int)N(v, 2));
                break;
            case SerializedPropertyType.ObjectReference:
                sp.objectReferenceValue = Reference(sp, op, roots);
                break;
            default:
                throw new EditFailed($"{sp.propertyPath} is a {sp.propertyType}; set its fields one at a time");
        }
    }

    static int Count(string v) => v.Split(',').Length;

    static float N(string v, int i)
    {
        var parts = v.Trim('(', ')', '{', '}', '[', ']').Split(',');
        if (i >= parts.Length)
            throw new EditFailed($"'{v}' needs one comma-separated number per component (x,y or x,y,z or x,y,z,w; r,g,b[,a] for colors)");
        var p = parts[i].Trim();
        var colon = p.IndexOf(':');
        if (colon >= 0)
            p = p.Substring(colon + 1).Trim();
        return float.Parse(p, Inv);
    }

    static UnityEngine.Object Reference(SerializedProperty sp, Op op, List<GameObject> roots)
    {
        var expected = sp.type.StartsWith("PPtr<", StringComparison.Ordinal) ? sp.type.Substring(5).TrimEnd('>').TrimStart('$') : "Object";
        UnityEngine.Object found;
        if (op.hasRef)
        {
            var go = Find(roots, op.refPath, null);
            found = op.refComponent >= 0 ? go.GetComponents<Component>()[op.refComponent] : go;
        }
        else if (string.IsNullOrEmpty(op.value) || op.value == "None" || op.value == "null")
            return null;
        else
        {
            var hash = op.value.LastIndexOf('#');
            var path = hash > 0 ? op.value.Substring(0, hash) : op.value;
            if (hash > 0)
            {
                var id = long.Parse(op.value.Substring(hash + 1), Inv);
                found = AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault(a => a != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(a, out string _, out long l) && l == id);
            }
            else
            {
                found = AssetDatabase.LoadMainAssetAtPath(path);
                if (found != null && !Named(found, expected) && expected != "Object")
                    found = AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault(a => a != null && Named(a, expected)) ?? found;
            }
            if (found == null)
                throw new EditFailed($"no asset {op.value}");
        }
        if (found is GameObject g && expected != "GameObject" && expected != "Object")
            found = g.GetComponents<Component>().FirstOrDefault(c => c != null && Named(c, expected)) ?? throw new EditFailed($"{g.name} has no {expected} for {sp.propertyPath}");
        if (expected != "Object" && found != null && !Named(found, expected))
            throw new EditFailed($"{sp.propertyPath} takes a {expected}, not a {found.GetType().Name}");
        return found;
    }


    // Writes each model's imported objects as Unity YAML, keyed by their local file ids, so the CLI can read objects that
    // scenes and prefabs instance from models. Arguments alternate: model path, output file, first line to write.
    public static string ExportModels(string json)
    {
        var i = 0;
        var items = ((List<object>)Json(json, ref i)).Select(o => (string)o).ToList();
        var written = 0;
        var failed = new List<string>();
        for (var k = 0; k + 2 < items.Count; k += 3)
        {
            var path = items[k];
            var guid = AssetDatabase.AssetPathToGUID(path);
            var objects = AssetDatabase.LoadAllAssetsAtPath(path).Where(o => o is GameObject || o is Component).ToList();
            if (objects.Count == 0)
            {
                failed.Add(path);
                continue;
            }
            var ids = new Dictionary<UnityEngine.Object, long>();
            foreach (var o in objects)
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o, out string _, out long id))
                    ids[o] = id;
            var sb = new StringBuilder();
            sb.Append(items[k + 2]).Append('\n');
            sb.Append("%YAML 1.1\n");
            foreach (var o in objects)
            {
                if (!ids.TryGetValue(o, out var id))
                    continue;
                var cls = o is GameObject ? 1 : o is RectTransform ? 224 : o is Transform ? 4 : o is MonoBehaviour ? 114 : 0;
                sb.Append("--- !u!").Append(cls).Append(" &").Append(id.ToString(Inv)).Append('\n');
                sb.Append(o.GetType().Name).Append(":\n");
                var it = new SerializedObject(o).GetIterator();
                if (it.Next(true))
                    WriteSiblings(it, 1, guid, ids, sb);
            }
            System.IO.File.WriteAllText(items[k + 1], sb.ToString());
            written++;
        }
        return "{\"ok\":true,\"written\":" + written + ",\"failed\":[" + string.Join(",", failed.Select(Quote)) + "]}";
    }

    // Writes a property and its following siblings at one depth, as YAML mapping entries.
    static void WriteSiblings(SerializedProperty it, int depth, string guid, Dictionary<UnityEngine.Object, long> ids, StringBuilder sb)
    {
        var level = it.depth;
        do
        {
            if (it.depth != level)
                break;
            WriteProperty(it.Copy(), depth, guid, ids, sb);
        }
        while (it.Next(false));
    }

    static void WriteProperty(SerializedProperty p, int depth, string guid, Dictionary<UnityEngine.Object, long> ids, StringBuilder sb)
    {
        var indent = new string(' ', depth * 2);
        var name = p.name;
        if (p.isArray && p.propertyType != SerializedPropertyType.String)
        {
            if (p.arraySize == 0)
            {
                sb.Append(indent).Append(name).Append(": []\n");
                return;
            }
            sb.Append(indent).Append(name).Append(":\n");
            for (var a = 0; a < Math.Min(p.arraySize, 10000); a++)
            {
                var e = p.GetArrayElementAtIndex(a);
                var leaf = Leaf(e, guid, ids);
                if (leaf != null)
                {
                    sb.Append(indent).Append("- ").Append(leaf).Append('\n');
                    continue;
                }
                var inner = new StringBuilder();
                var child = e.Copy();
                if (child.Next(true))
                    WriteSiblings(child, depth + 1, guid, ids, inner);
                var text = inner.ToString();
                // The first entry of a mapping item shares the dash's line.
                sb.Append(indent).Append("- ").Append(text.Length > depth * 2 + 2 ? text.Substring(depth * 2 + 2) : "{}\n");
            }
            return;
        }
        var value = Leaf(p, guid, ids);
        if (value != null)
        {
            sb.Append(indent).Append(name).Append(": ").Append(value).Append('\n');
            return;
        }
        var c = p.Copy();
        if (!c.Next(true) || c.depth <= p.depth)
        {
            sb.Append(indent).Append(name).Append(": {}\n");
            return;
        }
        sb.Append(indent).Append(name).Append(":\n");
        WriteSiblings(c, depth + 1, guid, ids, sb);
    }

    // A one-line YAML value, or null for a property with children.
    static string Leaf(SerializedProperty p, string guid, Dictionary<UnityEngine.Object, long> ids)
    {
        switch (p.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
            {
                var o = p.objectReferenceValue;
                if (o == null)
                    return "{fileID: 0}";
                if (ids.TryGetValue(o, out var local))
                    return "{fileID: " + local.ToString(Inv) + "}";
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o, out string g, out long id))
                    return "{fileID: " + id.ToString(Inv) + ", guid: " + g + ", type: 3}";
                return "{fileID: 0}";
            }
            case SerializedPropertyType.Integer:
            case SerializedPropertyType.LayerMask:
            case SerializedPropertyType.Character:
            case SerializedPropertyType.ArraySize:
                return p.numericType == SerializedPropertyNumericType.UInt64 ? p.ulongValue.ToString(Inv) : p.longValue.ToString(Inv);
            case SerializedPropertyType.Enum:
                return p.intValue.ToString(Inv);
            case SerializedPropertyType.Boolean:
                return p.boolValue ? "1" : "0";
            case SerializedPropertyType.Float:
                return p.numericType == SerializedPropertyNumericType.Double ? p.doubleValue.ToString("R", Inv) : p.floatValue.ToString("R", Inv);
            case SerializedPropertyType.String:
                return Quote(p.stringValue);
            case SerializedPropertyType.AnimationCurve:
            case SerializedPropertyType.Gradient:
            case SerializedPropertyType.ManagedReference:
            case SerializedPropertyType.ExposedReference:
                return "{}";
        }
        if (!p.hasChildren)
            return "{}";
        // Small all-number structs (vectors, colors, quaternions) are written in flow style, as Unity does.
        var c = p.Copy();
        var end = p.GetEndProperty();
        var parts = new List<string>();
        if (!c.Next(true))
            return null;
        while (!SerializedProperty.EqualContents(c, end))
        {
            if (c.depth != p.depth + 1 || c.hasChildren && c.propertyType != SerializedPropertyType.ObjectReference || c.propertyType == SerializedPropertyType.String || parts.Count > 6)
                return null;
            var v = Leaf(c, guid, ids);
            if (v == null)
                return null;
            parts.Add(c.name + ": " + v);
            if (!c.Next(false))
                break;
        }
        return "{" + string.Join(", ", parts) + "}";
    }

    // JsonUtility can't fill arrays of types compiled by run_script, so the request is read by hand.
    static Request ParseRequest(string json)
    {
        var i = 0;
        var root = (Dictionary<string, object>)Json(json, ref i);
        var request = new Request { file = (string)root["file"], saveOpen = root.TryGetValue("saveOpen", out var s) && (bool)s };
        var ops = new List<Op>();
        foreach (Dictionary<string, object> o in (List<object>)root["ops"])
        {
            var op = new Op();
            foreach (var f in typeof(Op).GetFields())
            {
                if (!o.TryGetValue(f.Name, out var v) || v == null)
                    continue;
                if (f.FieldType == typeof(string))
                    f.SetValue(op, (string)v);
                else if (f.FieldType == typeof(bool))
                    f.SetValue(op, (bool)v);
                else if (f.FieldType == typeof(int))
                    f.SetValue(op, (int)(double)v);
                else if (f.FieldType == typeof(int[]))
                    f.SetValue(op, ((List<object>)v).Select(x => (int)(double)x).ToArray());
                else if (f.FieldType == typeof(long[]))
                    f.SetValue(op, ((List<object>)v).Select(x => x is string t ? long.Parse(t, Inv) : (long)(double)x).ToArray());
                else if (f.FieldType == typeof(string[]))
                    f.SetValue(op, ((List<object>)v).Select(x => (string)x).ToArray());
            }
            ops.Add(op);
        }
        request.ops = ops.ToArray();
        return request;
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

    static string Quote(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";

    static string Error(string message, List<string> done = null) =>
        "{\"ok\":false,\"error\":" + Quote(message) + (done != null && done.Count > 0 ? ",\"done\":[" + string.Join(",", done.Select(Quote)) + "]" : "") + "}";
}
