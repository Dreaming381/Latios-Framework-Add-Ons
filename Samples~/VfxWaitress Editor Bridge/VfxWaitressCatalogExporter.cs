// Exports the VfxWaitress catalog from a running Unity Editor.
//
// The .asmref beside this file compiles it into Unity.VisualEffectGraph.Editor. VFX Graph's
// authoring model is entirely internal, and this needs far too much of it to reach by reflection.
//
// Run `unity command vfxwaitress_export_catalog`, use Tools > VfxWaitress > Export Catalog, or run
// it headless:
//
//   Unity -batchmode -quit -projectPath <project> \
//         -executeMethod VfxWaitress.CatalogExporter.ExportToDefaultPath
//
// The output goes to UserSettings/Waitress/catalog.json, which is where the tool looks. Override
// it with the -vfxWaitressOutput <path> command line argument.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.VFX;
using UnityEditorInternal;
using UnityEngine;

namespace VfxWaitress
{
    public static class CatalogExporter
    {
        [MenuItem("Tools/VfxWaitress/Export Catalog")]
        public static void ExportFromMenu()
        {
            var path = DefaultOutputPath();
            Export(path);
            EditorUtility.RevealInFinder(path);
        }

        public static void ExportToDefaultPath()
        {
            var args = Environment.GetCommandLineArgs();
            var path = DefaultOutputPath();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-vfxWaitressOutput")
                    path = args[i + 1];
            }
            Export(path);
        }

        /// <summary>
        /// Where the tool looks: UserSettings/Waitress/ in this project. That folder stays writable
        /// however the add-on was installed, and Unity's default .gitignore excludes it.
        /// </summary>
        public static string DefaultOutputPath()
        {
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectRoot, "UserSettings", "Waitress", "catalog.json");
        }

        public static string Export(string outputPath)
        {
            var w = new Json();
            w.Obj();
            w.Key("schema").Str("1");
            w.Key("generatedBy").Str("VfxWaitress.CatalogExporter");
            w.Key("generatedAt").Str(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            w.Key("unityVersion").Str(Application.unityVersion);
            w.Key("vfxGraphVersion").Str(PackageVersion());

            var warnings = new List<string>();

            w.Key("models").Arr();
            var seenVariants = new HashSet<string>(StringComparer.Ordinal);
            foreach (var d in Descriptors())
            {
                WriteModel(w, d.Item1, d.Item2, warnings, null);
                // Sub-variants are the entries the node menu actually lists (Set Size, Add
                // Velocity, ...). They share a type with their parent but differ by settings.
                IVFXModelDescriptor[] subs;
                try
                {
                    subs = d.Item2.subVariantDescriptors ?? Array.Empty<IVFXModelDescriptor>();
                }
                catch (Exception e)
                {
                    warnings.Add(d.Item2.modelType?.FullName + ": sub-variants failed: " + e.Message);
                    continue;
                }
                foreach (var sub in subs)
                {
                    var key = sub.modelType?.FullName + "/" + sub.category + "/" + sub.name;
                    if (!seenVariants.Add(key))
                        continue;
                    // Names arrive in the node menu encoding, so make the parent's readable.
                    WriteModel(w, d.Item1, sub, warnings, d.Item2.name.ToHumanReadable());
                }
            }
            // Subgraph nodes, deprecated models, and anything else the node menu never offers are
            // missing from the library but show up in graphs, so cover every concrete model type.
            var covered = new HashSet<Type>(Descriptors().Select(d => d.Item2).Select(d => d.unTypedModel?.GetType()).Where(t => t != null));
            foreach (var type in VFXLibrary.FindConcreteSubclasses(typeof(VFXModel)).OrderBy(t => t.FullName, StringComparer.Ordinal))
            {
                if (covered.Contains(type) || typeof(VFXSlot).IsAssignableFrom(type))
                    continue;
                WriteUnlistedModel(w, type, warnings);
            }
            w.End();

            w.Key("slotTypes").Arr();
            foreach (var t in VFXLibrary.GetSlotsType().OrderBy(o => o.FullName, StringComparer.Ordinal))
                WriteSlotType(w, t, warnings);
            w.End();

            w.Key("attributes").Arr();
            foreach (var a in VFXAttributesManager.GetBuiltInAttributesOrCombination(true, true, true, true).OrderBy(o => o.name, StringComparer.Ordinal))
                w.Obj().Key("name").Str(a.name).Key("valueType").Str(a.type.ToString()).Key("variadic").Str(a.variadic.ToString()).End();
            w.End();

            w.Key("warnings").Arr();
            foreach (var warning in warnings)
                w.Str(warning);
            w.End();
            w.End();

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, w.ToString(), new UTF8Encoding(false));
            Debug.Log($"VfxWaitress: catalog written to {outputPath} ({new FileInfo(outputPath).Length / 1024} KB, {warnings.Count} warnings)");
            return outputPath;
        }

        // The public accessors hide experimental models unless a user preference is on, but
        // experimental blocks show up in real graphs. The backing lists are unfiltered.
        static IEnumerable<System.Tuple<string, IVFXModelDescriptor> > Descriptors()
        {
            VFXLibrary.GetOperators().FirstOrDefault(); // forces the lazy load
            foreach (var pair in new[]
                     {
                         System.Tuple.Create("operator", "m_OperatorDescs"),
                         System.Tuple.Create("context", "m_ContextDescs"),
                         System.Tuple.Create("block", "m_BlockDescs"),
                         System.Tuple.Create("parameter", "m_ParametersDescs"),
                     })
            {
                var field = typeof(VFXLibrary).GetField(pair.Item2, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (field?.GetValue(null) is System.Collections.IEnumerable list)
                {
                    foreach (IVFXModelDescriptor d in list)
                        yield return System.Tuple.Create(pair.Item1, d);
                }
            }
        }

        static void WriteModel(Json w, string kind, IVFXModelDescriptor d, List<string> warnings, string variantOf)
        {
            VFXModel model = null;
            try
            {
                model = d.unTypedModel;
            }
            catch (Exception e)
            {
                warnings.Add(d.modelType?.FullName + ": instantiation failed: " + e.Message);
            }

            w.Obj();
            w.Key("kind").Str(kind);
            w.Key("type").Str(d.modelType?.FullName ?? "");
            w.Key("name").Str((d.name ?? "").ToHumanReadable());
            w.Key("category").Str(d.category ?? "");
            if (variantOf != null)
                w.Key("variantOf").Str(variantOf);
            if (d.modelType != null && VFXInfoAttribute.Get(d.modelType)?.experimental == true)
                w.Key("experimental").Bool(true);
            if (d.synonyms != null && d.synonyms.Length > 0)
            {
                w.Key("synonyms").Arr();
                foreach (var s in d.synonyms)
                    w.Str(s);
                w.End();
            }

            var variantSettings = d.variant?.settings;
            if (variantSettings != null && variantSettings.Length > 0)
            {
                w.Key("variantSettings").Obj();
                foreach (var kvp in variantSettings)
                    w.Key(kvp.Key).Str(SettingValueToString(kvp.Value));
                w.End();
            }

            // A parameter descriptor's modelType is the value type it exposes, not the model
            // class, so the script has to come from the instance.
            var scriptType = model?.GetType() ?? d.modelType;
            var script = ScriptRef(scriptType);
            if (script != null)
                w.Key("script").Str(script);
            else if (scriptType != null)
                warnings.Add(scriptType.FullName + ": no MonoScript found");
            if (kind == "parameter" && d.modelType != null)
                w.Key("valueType").Str(d.modelType.FullName);

            if (model != null)
            {
                WriteSettings(w, model);
                WriteSlots(w, "inputs", model as IVFXSlotContainer, true);
                WriteSlots(w, "outputs", model as IVFXSlotContainer, false);
                // Every variant needs its own template, since settings can reshape the slot list
                // and Unity doesn't repair a node built from the wrong one.
                WriteTemplate(w, model, warnings);
            }
            w.End();
        }

        static void WriteUnlistedModel(Json w, Type type, List<string> warnings)
        {
            var script = ScriptRef(type);
            if (script == null)
                return;
            w.Obj();
            w.Key("kind").Str(KindOf(type));
            w.Key("type").Str(type.FullName);
            w.Key("name").Str(ObjectNames.NicifyVariableName(type.Name));
            w.Key("category").Str("");
            w.Key("listed").Bool(false);
            w.Key("script").Str(script);
            VFXModel model = null;
            try
            {
                model = (VFXModel)ScriptableObject.CreateInstance(type);
                WriteSettings(w, model);
                WriteSlots(w, "inputs", model as IVFXSlotContainer, true);
                WriteSlots(w, "outputs", model as IVFXSlotContainer, false);
                // Unlisted models get templates too. A context needs a VFXData beside it, and
                // nothing in the node menu creates one.
                WriteTemplate(w, model, warnings);
            }
            catch (Exception e)
            {
                warnings.Add(type.FullName + ": unlisted model inspection failed: " + e.Message);
            }
            finally
            {
                if (model != null)
                    ScriptableObject.DestroyImmediate(model);
            }
            w.End();
        }

        static string KindOf(Type type)
        {
            if (typeof(VFXContext).IsAssignableFrom(type))
                return "context";
            if (typeof(VFXBlock).IsAssignableFrom(type))
                return "block";
            if (typeof(VFXOperator).IsAssignableFrom(type))
                return "operator";
            if (typeof(VFXParameter).IsAssignableFrom(type))
                return "parameter";
            return "model";
        }

        static void WriteSettings(Json w, VFXModel model)
        {
            var settings = model.GetSettings(true).Where(o => o.valid).ToList();
            if (settings.Count == 0)
                return;
            w.Key("settings").Arr();
            foreach (var s in settings)
            {
                w.Obj();
                w.Key("name").Str(s.name);
                w.Key("type").Str(s.field.FieldType.FullName);
                // How the value is serialized. Unity silently ignores a bare scalar where it
                // expects a nested object, so the tool needs to know.
                w.Key("shape").Str(ShapeOf(s.field.FieldType));
                if (s.field.FieldType.IsEnum)
                {
                    w.Key("values").Arr();
                    foreach (var v in Enum.GetNames(s.field.FieldType))
                        w.Str(v);
                    w.End();
                }
                object value = null;
                try
                {
                    value = s.value;
                }
                catch
                {
                }
                if (value != null)
                    w.Key("default").Str(SettingValueToString(value));
                // What the class initializes the field to, before any variant is applied. A
                // descriptor's own "default" is just its variant's value.
                var pristine = ClassDefault(model.GetType(), s.name);
                if (pristine != null)
                    w.Key("classDefault").Str(pristine);
                w.End();
            }
            w.End();
        }

        static readonly Dictionary<Type, Dictionary<string, string>> s_ClassDefaults = new Dictionary<Type, Dictionary<string, string>>();

        static string ClassDefault(Type type, string settingName)
        {
            if (!s_ClassDefaults.TryGetValue(type, out var map))
            {
                map = new Dictionary<string, string>(StringComparer.Ordinal);
                VFXModel pristine = null;
                try
                {
                    pristine = (VFXModel)ScriptableObject.CreateInstance(type);
                    foreach (var s in pristine.GetSettings(true).Where(o => o.valid))
                    {
                        try
                        {
                            if (s.value != null)
                                map[s.name] = SettingValueToString(s.value);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    if (pristine != null)
                        ScriptableObject.DestroyImmediate(pristine);
                }
                s_ClassDefaults[type] = map;
            }
            return map.TryGetValue(settingName, out var value) ? value : null;
        }

        static string ShapeOf(Type type)
        {
            if (type == typeof(SerializableType))
                return "serializableType";
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return "objectRef";
            if (type.IsEnum || type.IsPrimitive || type == typeof(string))
                return "scalar";
            return "nested";
        }

        static void WriteSlots(Json w, string key, IVFXSlotContainer container, bool input)
        {
            if (container == null)
                return;
            var slots = input ? container.inputSlots : container.outputSlots;
            if (slots.Count == 0)
                return;
            w.Key(key).Arr();
            foreach (var slot in slots)
                WriteSlot(w, slot);
            w.End();
        }

        static void WriteSlot(Json w, VFXSlot slot)
        {
            w.Obj();
            w.Key("name").Str(slot.property.name ?? "");
            w.Key("type").Str(slot.property.type?.FullName ?? "");
            if (slot.property.type != null && slot.property.type != typeof(void))
                w.Key("valueType").Str(VFXExpression.GetVFXValueTypeFromType(slot.property.type).ToString());
            if (slot.GetNbChildren() > 0)
            {
                w.Key("children").Arr();
                foreach (var child in slot.children)
                    WriteSlot(w, child);
                w.End();
            }
            w.End();
        }

        // A freshly created model plus its slot tree, serialized exactly as a .vfx stores it.
        // Writing a node then means cloning this and remapping its local file ids.
        static void WriteTemplate(Json w, VFXModel model, List<string> warnings)
        {
            var objs = new HashSet<ScriptableObject>();
            try
            {
                model.CollectDependencies(objs);
            }
            catch (Exception e)
            {
                warnings.Add(model.GetType().FullName + ": CollectDependencies failed: " + e.Message);
                return;
            }
            objs.Add(model);

            var temp = Path.Combine(Path.GetTempPath(), "vfxwaitress_template.asset");
            try
            {
                var ordered = objs.OrderBy(o => ReferenceEquals(o, model) ? 0 : 1).Cast<UnityEngine.Object>().ToArray();
                InternalEditorUtility.SaveToSerializedFileAndForget(ordered, temp, true);
                w.Key("template").Str(File.ReadAllText(temp));
            }
            catch (Exception e)
            {
                warnings.Add(model.GetType().FullName + ": template serialization failed: " + e.Message);
            }
            finally
            {
                if (File.Exists(temp))
                    File.Delete(temp);
            }
        }

        static void WriteSlotType(Json w, Type type, List<string> warnings)
        {
            w.Obj();
            w.Key("type").Str(type.FullName);
            // The spelling a SerializableType setting stores, which is what a caller naming a
            // buffer type has to end up writing.
            w.Key("assemblyQualified").Str(type.AssemblyQualifiedName);
            var desc = VFXLibrary.GetSlot(type);
            if (desc != null)
            {
                var script = ScriptRef(desc.modelType);
                if (script != null)
                    w.Key("slotClass").Str(desc.modelType.FullName).Key("script").Str(script);
            }
            w.Key("spaceable").Bool(VFXLibrary.IsSpaceableSlotType(type));
            var fields = VFXLibrary.GetFieldFromType(type).ToList();
            if (fields.Count > 0)
            {
                w.Key("fields").Arr();
                foreach (var f in fields)
                    w.Obj().Key("name").Str(f.name).Key("type").Str(f.type?.FullName ?? "").End();
                w.End();
            }
            w.End();
        }

        static readonly Dictionary<Type, string> s_ScriptRefs = new Dictionary<Type, string>();

        // The guid of the MonoScript that declares the type, which is what a .vfx stores in m_Script.
        static string ScriptRef(Type type)
        {
            if (type == null)
                return null;
            if (s_ScriptRefs.TryGetValue(type, out var cached))
                return cached;
            string result = null;
            ScriptableObject instance = null;
            try
            {
                instance = ScriptableObject.CreateInstance(type);
                var script = MonoScript.FromScriptableObject(instance);
                if (script != null)
                {
                    var path = AssetDatabase.GetAssetPath(script);
                    if (!string.IsNullOrEmpty(path))
                        result = AssetDatabase.AssetPathToGUID(path);
                }
            }
            catch
            {
            }
            finally
            {
                if (instance != null)
                    ScriptableObject.DestroyImmediate(instance);
            }
            s_ScriptRefs[type] = result;
            return result;
        }

        static string SettingValueToString(object value)
        {
            if (value == null)
                return "";
            if (value is UnityEngine.Object o)
                return o == null ? "" : o.name;
            if (value is Type t)
                return t.FullName;
            // A SerializableType prints its own class name, not the type it stands for, and it is
            // the value that tells a variant of Inline apart from every other one.
            if (value is SerializableType serializable)
                return ((Type)serializable)?.FullName ?? "";
            if (value is bool b)
                return b ? "true" : "false";
            if (value is IFormattable f)
                return f.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            return value.ToString();
        }

        static string PackageVersion()
        {
            var info = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(VFXGraph).Assembly);
            return info?.version ?? "unknown";
        }

        // Minimal streaming JSON writer. Output is one line; the catalog is machine-read.
        class Json
        {
            readonly StringBuilder m_Sb = new StringBuilder();
            readonly Stack<char> m_Open = new Stack<char>();
            readonly Stack<bool> m_First = new Stack<bool>();
            bool m_PendingKey;

            public Json Obj()
            {
                Sep();
                m_Sb.Append('{');
                m_Open.Push('{');
                m_First.Push(true);
                return this;
            }

            public Json Arr()
            {
                Sep();
                m_Sb.Append('[');
                m_Open.Push('[');
                m_First.Push(true);
                return this;
            }

            public Json End()
            {
                m_First.Pop();
                m_Sb.Append(m_Open.Pop() == '{' ? '}' : ']');
                return this;
            }

            public Json Key(string name)
            {
                Sep();
                Escape(name);
                m_Sb.Append(": ");
                m_PendingKey = true;
                return this;
            }

            public Json Str(string value)
            {
                Sep();
                Escape(value);
                return this;
            }

            public Json Bool(bool value)
            {
                Sep();
                m_Sb.Append(value ? "true" : "false");
                return this;
            }

            void Sep()
            {
                if (m_PendingKey)
                {
                    m_PendingKey = false;
                    return;
                }
                if (m_First.Count == 0)
                    return;
                if (m_First.Peek())
                {
                    m_First.Pop();
                    m_First.Push(false);
                }
                else
                {
                    m_Sb.Append(", ");
                }
            }

            void Escape(string s)
            {
                m_Sb.Append('"');
                foreach (var c in s ?? "")
                {
                    switch (c)
                    {
                        case '"': m_Sb.Append("\\\""); break;
                        case '\\': m_Sb.Append("\\\\"); break;
                        case '\n': m_Sb.Append("\\n"); break;
                        case '\r': m_Sb.Append("\\r"); break;
                        case '\t': m_Sb.Append("\\t"); break;
                        default:
                            if (c < 0x20)
                                m_Sb.Append("\\u").Append(((int)c).ToString("x4"));
                            else
                                m_Sb.Append(c);
                            break;
                    }
                }
                m_Sb.Append('"');
            }

            public override string ToString() => m_Sb.ToString();
        }
    }
}
