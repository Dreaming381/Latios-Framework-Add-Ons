// Exports the ShaderWaitress node catalog from a running Unity Editor.
//
// Run `unity command shaderwaitress_export_catalog`, use Tools > ShaderWaitress > Export Node
// Catalog, or run it headless:
//
//   Unity -batchmode -quit -projectPath <project> \
//         -executeMethod ShaderWaitress.CatalogExporter.ExportToDefaultPath
//
// The output goes to UserSettings/Waitress/nodes.json, which is where the tool looks. Override it
// with the -shaderWaitressOutput <path> command line argument.
//
// Everything here goes through reflection, so it works whichever assembly it compiles into.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ShaderWaitress
{
    public static class CatalogExporter
    {
        const string k_ShaderGraphAssembly = "Unity.ShaderGraph.Editor";

        [MenuItem("Tools/ShaderWaitress/Export Node Catalog")]
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
                if (args[i] == "-shaderWaitressOutput")
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
            return Path.Combine(projectRoot, "UserSettings", "Waitress", "nodes.json");
        }

        public static void Export(string outputPath)
        {
            var writer = new JsonWriter();
            var reflect = new Reflect();

            writer.BeginObject();
            writer.Field("schema", "2");
            writer.Field("generatedBy", "ShaderWaitress.CatalogExporter");
            writer.Field("generatedAt", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            writer.Field("unityVersion", Application.unityVersion);
            writer.Field("shaderGraphVersion", ShaderGraphVersion());

            var nodeCount = WriteNodes(writer, reflect);
            var blockCount = WriteBlocks(writer, reflect);
            var propertyCount = WriteProperties(writer, reflect);
            var keywordCount = WriteKeywords(writer, reflect);
            var starterCount = WriteStarters(writer, reflect);

            writer.EndObject();

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, writer.ToString(), new UTF8Encoding(false));
            Debug.Log($"ShaderWaitress: wrote {nodeCount} nodes, {blockCount} blocks, " +
                      $"{propertyCount} properties, {keywordCount} keywords, {starterCount} starter graphs to {outputPath}");
        }

        static string ShaderGraphVersion()
        {
            var info = UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == k_ShaderGraphAssembly));
            return info != null ? info.version : "unknown";
        }

        // ------------------------------------------------------------------ nodes

        static int WriteNodes(JsonWriter writer, Reflect reflect)
        {
            writer.BeginArray("nodes");
            var count = 0;
            foreach (var type in reflect.ConcreteSubclassesOf(reflect.AbstractMaterialNode).OrderBy(t => t.FullName, StringComparer.Ordinal))
            {
                if (type == reflect.BlockNode)
                    continue;
                if (type.Name.EndsWith("MasterNode1", StringComparison.Ordinal))
                    continue;   // legacy upgrade shims, never creatable

                var title = reflect.TitleOf(type);
                var creatable = title != null || reflect.AlwaysExport.Contains(type.Name);
                if (!creatable)
                    continue;

                object node;
                try
                {
                    node = Activator.CreateInstance(type, true);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"ShaderWaitress: cannot construct {type.FullName}: {e.GetBaseException().Message}");
                    continue;
                }

                var template = reflect.SerializeToTemplate(node);
                if (template == null)
                    continue;

                writer.BeginObject();
                writer.Field("type", type.FullName);
                writer.Field("title", title != null && title.Length > 0 ? title[title.Length - 1] : reflect.NameOf(node) ?? type.Name);
                if (title != null && title.Length > 1)
                    writer.StringArray("path", title.Take(title.Length - 1));
                var synonyms = reflect.SynonymsOf(node);
                if (synonyms != null && synonyms.Length > 0)
                    writer.StringArray("synonyms", synonyms);
                writer.Field("preview", reflect.HasPreview(node));
                writer.Field("controls", reflect.ControlCount(type));
                WriteSettings(writer, reflect, type, template);
                writer.StringArray("template", template);
                writer.EndObject();
                count++;
            }
            writer.EndArray();
            return count;
        }

        /// <summary>
        /// A node's settings: the dropdowns and toggles drawn on the node body rather than on a
        /// port. They're serialized fields declared below AbstractMaterialNode, found by walking
        /// the type's hierarchy and keeping the ones that appear in the serialized template.
        /// </summary>
        static void WriteSettings(JsonWriter writer, Reflect reflect, Type type, string[] template)
        {
            var nodeJson = template.Length > 0 ? template[0] : string.Empty;
            var settings = reflect.SettingsOf(type).Where(f => nodeJson.Contains("\"" + f.Name + "\"")).ToList();
            if (settings.Count == 0)
                return;

            writer.BeginArray("settings");
            foreach (var field in settings)
            {
                var fieldType = field.FieldType;
                writer.BeginObject();
                writer.Field("field", field.Name);
                writer.Field("name", TrimFieldPrefix(field.Name));
                if (fieldType.IsEnum)
                {
                    writer.Field("kind", "enum");
                    writer.StringArray("values", Enum.GetNames(fieldType));
                    writer.StringArray("codes", Enum.GetValues(fieldType).Cast<object>()
                        .Select(v => Convert.ToInt64(v).ToString(System.Globalization.CultureInfo.InvariantCulture)));
                }
                else if (fieldType == typeof(bool))
                {
                    writer.Field("kind", "bool");
                }
                else if (fieldType == typeof(int) || fieldType == typeof(uint) ||
                         fieldType == typeof(long) || fieldType == typeof(short))
                {
                    writer.Field("kind", "int");
                }
                else if (fieldType == typeof(float) || fieldType == typeof(double))
                {
                    writer.Field("kind", "float");
                }
                else
                {
                    writer.Field("kind", "string");
                }
                writer.EndObject();
            }
            writer.EndArray();
        }

        static string TrimFieldPrefix(string name)
        {
            if (name.StartsWith("m_", StringComparison.Ordinal) && name.Length > 2)
                return name.Substring(2);
            return name;
        }

        // ----------------------------------------------------------------- blocks

        static int WriteBlocks(JsonWriter writer, Reflect reflect)
        {
            writer.BeginArray("blocks");
            var count = 0;
            var initMethod = reflect.BlockNode?.GetMethod("Init", BindingFlags.Public | BindingFlags.Instance);
            if (initMethod == null)
            {
                writer.EndArray();
                return 0;
            }

            foreach (var (descriptor, tag, name, stage) in reflect.AllBlockFields())
            {
                object node;
                try
                {
                    node = Activator.CreateInstance(reflect.BlockNode, true);
                    initMethod.Invoke(node, new[] { descriptor });
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"ShaderWaitress: cannot build block {tag}.{name}: {e.GetBaseException().Message}");
                    continue;
                }

                var template = reflect.SerializeToTemplate(node);
                if (template == null)
                    continue;

                writer.BeginObject();
                writer.Field("type", reflect.BlockNode.FullName);
                writer.Field("descriptor", tag + "." + name);
                writer.Field("title", tag + "." + name);
                writer.Field("stage", stage);
                writer.StringArray("template", template);
                writer.EndObject();
                count++;
            }
            writer.EndArray();
            return count;
        }

        // ------------------------------------------------------------- properties

        static int WriteProperties(JsonWriter writer, Reflect reflect)
        {
            writer.BeginArray("properties");
            var count = 0;
            foreach (var type in reflect.ConcreteSubclassesOf(reflect.AbstractShaderProperty).OrderBy(t => t.FullName, StringComparer.Ordinal))
            {
                object property;
                try
                {
                    property = Activator.CreateInstance(type, true);
                }
                catch (Exception)
                {
                    continue;
                }
                var template = reflect.SerializeToTemplate(property);
                if (template == null)
                    continue;

                var title = type.Name.EndsWith("ShaderProperty", StringComparison.Ordinal)
                    ? type.Name.Substring(0, type.Name.Length - "ShaderProperty".Length)
                    : type.Name;

                writer.BeginObject();
                writer.Field("type", type.FullName);
                writer.Field("title", title);
                // Which HLSL declarations this property type will accept. A texture cannot be
                // HybridPerInstance, for instance, and the Editor greys those choices out.
                var allowed = reflect.AllowedDeclarations(property).ToList();
                if (allowed.Count > 0)
                    writer.StringArray("declarations", allowed);
                writer.StringArray("template", template);
                writer.EndObject();
                count++;
            }
            writer.EndArray();
            return count;
        }

        // --------------------------------------------------------------- keywords

        static int WriteKeywords(JsonWriter writer, Reflect reflect)
        {
            writer.BeginArray("keywords");
            var count = 0;
            var keywordType = reflect.Find("UnityEditor.ShaderGraph.ShaderKeyword");
            var keywordTypeEnum = reflect.Find("UnityEditor.ShaderGraph.KeywordType");
            if (keywordType != null && keywordTypeEnum != null)
            {
                foreach (var name in Enum.GetNames(keywordTypeEnum))
                {
                    object keyword;
                    try
                    {
                        keyword = Activator.CreateInstance(keywordType,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                            null, new[] { Enum.Parse(keywordTypeEnum, name) }, null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"ShaderWaitress: cannot build {name} keyword: {e.GetBaseException().Message}");
                        continue;
                    }
                    var template = reflect.SerializeToTemplate(keyword);
                    if (template == null)
                        continue;
                    writer.BeginObject();
                    writer.Field("type", keywordType.FullName);
                    writer.Field("title", name);
                    writer.StringArray("template", template);
                    writer.EndObject();
                    count++;
                }
            }
            writer.EndArray();
            return count;
        }

        // --------------------------------------------------------------- starters

        /// <summary>
        /// Whole-file templates for "shaderwaitress new", produced the same way the
        /// Assets/Create menu items do so they match what the Editor would have written.
        /// </summary>
        static int WriteStarters(JsonWriter writer, Reflect reflect)
        {
            writer.BeginArray("starters");
            var count = 0;

            var subGraph = reflect.BuildStarterSubGraph();
            if (subGraph != null)
            {
                writer.BeginObject();
                writer.Field("name", "subgraph");
                writer.Field("kind", "subgraph");
                writer.Field("text", subGraph);
                writer.EndObject();
                count++;
            }

            foreach (var recipe in reflect.StarterRecipes())
            {
                string text;
                try
                {
                    text = reflect.BuildStarterGraph(recipe.targetType, recipe.subTargetType, recipe.blocks);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"ShaderWaitress: cannot build starter '{recipe.name}': {e.GetBaseException().Message}");
                    continue;
                }
                if (text == null)
                    continue;

                writer.BeginObject();
                writer.Field("name", recipe.name);
                writer.Field("target", recipe.targetType.FullName);
                if (recipe.subTargetType != null)
                    writer.Field("subTarget", recipe.subTargetType.FullName);
                writer.Field("text", text);
                writer.EndObject();
                count++;
            }
            writer.EndArray();
            return count;
        }

        // ------------------------------------------------------------- reflection

        sealed class Reflect
        {
            /// <summary>
            /// How many controls Shader Graph draws on the node body, which decides the node's
            /// height. It can be fewer than the serialized settings: Sample Texture 2D has four
            /// settings and draws two. This runs the same query MaterialNodeView uses.
            /// </summary>
            public int ControlCount(Type nodeType)
            {
                if (ControlAttribute == null)
                    return -1;
                var count = 0;
                foreach (var property in nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (property.GetCustomAttributes(ControlAttribute, false).Length > 0)
                        count++;
                }
                return count;
            }

            public readonly Assembly ShaderGraph;
            public readonly Type ControlAttribute;
            public readonly Type AbstractMaterialNode;
            public readonly Type BlockNode;
            public readonly Type AbstractShaderProperty;
            public readonly Type TitleAttribute;
            public readonly Type BlockFieldDescriptor;
            public readonly Type GenerateBlocksAttribute;
            public readonly Type GraphData;
            public readonly Type CategoryData;
            public readonly Type Target;

            readonly MethodInfo m_Serialize;

            public readonly HashSet<string> AlwaysExport = new HashSet<string>
            {
                "PropertyNode", "KeywordNode", "DropdownNode", "SubGraphNode", "RedirectNodeData",
            };

            public Reflect()
            {
                ShaderGraph = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == k_ShaderGraphAssembly);
                if (ShaderGraph == null)
                    throw new InvalidOperationException($"assembly {k_ShaderGraphAssembly} is not loaded");

                AbstractMaterialNode = Need("UnityEditor.ShaderGraph.AbstractMaterialNode");
                ControlAttribute = Need("UnityEditor.ShaderGraph.Drawing.Controls.IControlAttribute")
                                   ?? NeedByName("IControlAttribute");
                BlockNode = Need("UnityEditor.ShaderGraph.BlockNode");
                AbstractShaderProperty = Need("UnityEditor.ShaderGraph.Internal.AbstractShaderProperty")
                                         ?? NeedByName("AbstractShaderProperty");
                TitleAttribute = Need("UnityEditor.ShaderGraph.TitleAttribute");
                BlockFieldDescriptor = Need("UnityEditor.ShaderGraph.BlockFieldDescriptor") ?? NeedByName("BlockFieldDescriptor");
                GenerateBlocksAttribute = Need("UnityEditor.ShaderGraph.GenerateBlocksAttribute") ?? NeedByName("GenerateBlocksAttribute");
                GraphData = Need("UnityEditor.ShaderGraph.GraphData");
                CategoryData = Need("UnityEditor.ShaderGraph.CategoryData");
                Target = Need("UnityEditor.ShaderGraph.Target") ?? NeedByName("Target");

                var multiJson = Need("UnityEditor.ShaderGraph.Serialization.MultiJson");
                m_Serialize = multiJson.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == "Serialize" && m.GetParameters().Length == 1);
                if (m_Serialize == null)
                    throw new InvalidOperationException("MultiJson.Serialize was not found");
            }

            Type Need(string fullName) => ShaderGraph.GetType(fullName, false);

            public Type Find(string fullName) => ShaderGraph.GetType(fullName, false) ?? NeedByName(fullName.Substring(fullName.LastIndexOf((char)46) + 1));

            Type NeedByName(string name) => ShaderGraph.GetTypes().FirstOrDefault(t => t.Name == name);

            public IEnumerable<Type> ConcreteSubclassesOf(Type baseType)
            {
                if (baseType == null)
                    yield break;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types;
                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        types = e.Types.Where(t => t != null).ToArray();
                    }
                    foreach (var type in types)
                    {
                        if (type.IsAbstract || type.IsGenericTypeDefinition || !baseType.IsAssignableFrom(type))
                            continue;
                        yield return type;
                    }
                }
            }

            public string[] TitleOf(Type type)
            {
                var attribute = type.GetCustomAttributes(TitleAttribute, false).FirstOrDefault();
                if (attribute == null)
                    return null;
                var member = TitleAttribute.GetProperty("title", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (member != null)
                    return member.GetValue(attribute) as string[];
                var field = TitleAttribute.GetField("title", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return field?.GetValue(attribute) as string[];
            }

            /// <summary>
            /// Serialized fields the node type adds on top of AbstractMaterialNode. Those are the
            /// node's own settings, and everything above is Shader Graph bookkeeping.
            /// </summary>
            public IEnumerable<FieldInfo> SettingsOf(Type type)
            {
                var seen = new HashSet<string>(StringComparer.Ordinal);
                for (var t = type; t != null && t != AbstractMaterialNode; t = t.BaseType)
                {
                    foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    {
                        if (field.IsNotSerialized || field.IsStatic || field.IsLiteral)
                            continue;
                        if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                            continue;
                        var ft = field.FieldType;
                        var usable = ft.IsEnum || ft == typeof(bool) || ft == typeof(int) || ft == typeof(uint) ||
                                     ft == typeof(long) || ft == typeof(short) || ft == typeof(float) ||
                                     ft == typeof(double) || ft == typeof(string);
                        if (!usable || !seen.Add(field.Name))
                            continue;
                        yield return field;
                    }
                }
            }

            /// <summary>
            /// The HLSL declarations a property type accepts, asked of the type itself so it
            /// matches whatever the installed Shader Graph allows.
            /// </summary>
            public IEnumerable<string> AllowedDeclarations(object property)
            {
                var declarationType = Find("UnityEditor.ShaderGraph.Internal.HLSLDeclaration")
                                      ?? Find("UnityEditor.ShaderGraph.HLSLDeclaration");
                var allow = property.GetType().GetMethod("AllowHLSLDeclaration",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (declarationType == null || allow == null)
                    yield break;
                foreach (var name in Enum.GetNames(declarationType))
                {
                    bool ok;
                    try
                    {
                        ok = (bool)allow.Invoke(property, new[] { Enum.Parse(declarationType, name) });
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (ok)
                        yield return name;
                }
            }

            public string NameOf(object node) => GetMember(node, "name") as string;

            public string[] SynonymsOf(object node) => GetMember(node, "synonyms") as string[];

            public bool HasPreview(object node) => GetMember(node, "hasPreview") is bool b && b;

            static object GetMember(object instance, string name)
            {
                var type = instance.GetType();
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
                var property = type.GetProperty(name, flags);
                if (property != null && property.CanRead)
                {
                    try
                    {
                        return property.GetValue(instance);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                var field = type.GetField(name, flags);
                try
                {
                    return field?.GetValue(instance);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>Serializes an object the way Shader Graph would, split into entries.</summary>
            public string[] SerializeToTemplate(object jsonObject)
            {
                string text;
                try
                {
                    text = (string)m_Serialize.Invoke(null, new[] { jsonObject });
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"ShaderWaitress: serialize failed for {jsonObject.GetType().FullName}: {e.GetBaseException().Message}");
                    return null;
                }
                if (string.IsNullOrEmpty(text))
                    return null;
                return text.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToArray();
            }

            public IEnumerable<(object descriptor, string tag, string name, string stage)> AllBlockFields()
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types;
                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        types = e.Types.Where(t => t != null).ToArray();
                    }
                    foreach (var type in types)
                    {
                        if (GenerateBlocksAttribute != null && !type.IsDefined(GenerateBlocksAttribute, false))
                            continue;
                        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (!BlockFieldDescriptor.IsAssignableFrom(field.FieldType))
                                continue;
                            object descriptor;
                            try
                            {
                                descriptor = field.GetValue(null);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            if (descriptor == null)
                                continue;
                            var tag = GetMember(descriptor, "tag") as string;
                            var name = GetMember(descriptor, "name") as string;
                            var stage = GetMember(descriptor, "shaderStage")?.ToString() ?? "Fragment";
                            if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(name))
                                continue;
                            yield return (descriptor, tag, name, stage.ToLowerInvariant());
                        }
                    }
                }
            }

            public struct StarterRecipe
            {
                public string name;
                public Type targetType;
                public Type subTargetType;
                public object[] blocks;
            }

            public IEnumerable<StarterRecipe> StarterRecipes()
            {
                var blocks = AllBlockFields().ToList();

                object Block(string tag, string name) =>
                    blocks.FirstOrDefault(b => b.tag == tag && b.name == name).descriptor;

                var vertex = new[]
                {
                    Block("VertexDescription", "Position"),
                    Block("VertexDescription", "Normal"),
                    Block("VertexDescription", "Tangent"),
                }.Where(b => b != null).ToArray();

                object[] Fragment(params string[] names) =>
                    names.Select(n => Block("SurfaceDescription", n)).Where(b => b != null).ToArray();

                foreach (var targetType in ConcreteSubclassesOf(Target))
                {
                    var subTargets = SubTargetsFor(targetType).ToList();
                    if (subTargets.Count == 0)
                    {
                        yield return new StarterRecipe
                        {
                            name = ShortTargetName(targetType),
                            targetType = targetType,
                            blocks = vertex.Concat(Fragment("BaseColor", "Alpha")).ToArray(),
                        };
                        continue;
                    }
                    foreach (var subTarget in subTargets)
                    {
                        var lit = subTarget.Name.IndexOf("Lit", StringComparison.OrdinalIgnoreCase) >= 0 &&
                                  subTarget.Name.IndexOf("Unlit", StringComparison.OrdinalIgnoreCase) < 0;
                        var fragment = lit
                            ? Fragment("BaseColor", "NormalTS", "Metallic", "Smoothness", "Emission", "Occlusion")
                            : Fragment("BaseColor", "Alpha");
                        yield return new StarterRecipe
                        {
                            name = ShortTargetName(targetType) + "/" + ShortSubTargetName(targetType, subTarget),
                            targetType = targetType,
                            subTargetType = subTarget,
                            blocks = vertex.Concat(fragment).ToArray(),
                        };
                    }
                }
            }

            IEnumerable<Type> SubTargetsFor(Type targetType)
            {
                var subTargetBase = Need("UnityEditor.ShaderGraph.SubTarget") ?? NeedByName("SubTarget");
                if (subTargetBase == null)
                    yield break;
                object instance;
                try
                {
                    instance = Activator.CreateInstance(targetType, true);
                }
                catch (Exception)
                {
                    yield break;
                }
                var method = targetType.GetMethod("TrySetActiveSubTarget",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                    yield break;

                foreach (var candidate in ConcreteSubclassesOf(subTargetBase).OrderBy(t => t.FullName, StringComparer.Ordinal))
                {
                    bool ok;
                    try
                    {
                        ok = (bool)method.Invoke(instance, new object[] { candidate });
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (ok)
                        yield return candidate;
                }
            }

            static string ShortTargetName(Type type)
            {
                var name = type.Name;
                if (name.EndsWith("Target", StringComparison.Ordinal) && name.Length > 6)
                    name = name.Substring(0, name.Length - 6);
                return name.ToLowerInvariant();
            }

            static string ShortSubTargetName(Type targetType, Type subTargetType)
            {
                var name = subTargetType.Name;
                if (name.EndsWith("SubTarget", StringComparison.Ordinal))
                    name = name.Substring(0, name.Length - "SubTarget".Length);
                var prefix = targetType.Name;
                if (prefix.EndsWith("Target", StringComparison.Ordinal))
                    prefix = prefix.Substring(0, prefix.Length - 6);
                if (name.StartsWith(prefix, StringComparison.Ordinal) && name.Length > prefix.Length)
                    name = name.Substring(prefix.Length);
                return name.ToLowerInvariant();
            }

            /// <summary>Argument array for a method whose later parameters are optional.</summary>
            static object[] FillDefaults(MethodInfo method, params object[] leading)
            {
                var parameters = method.GetParameters();
                var args = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (i < leading.Length)
                        args[i] = leading[i];
                    else if (parameters[i].HasDefaultValue)
                        args[i] = parameters[i].DefaultValue;
                    else if (parameters[i].ParameterType.IsValueType)
                        args[i] = Activator.CreateInstance(parameters[i].ParameterType);
                }
                return args;
            }

            /// <summary>Mirrors Assets/Create/Shader Graph/Sub Graph.</summary>
            public string BuildStarterSubGraph()
            {
                var outputNodeType = Find("UnityEditor.ShaderGraph.SubGraphOutputNode");
                var concreteValueType = Find("UnityEditor.ShaderGraph.ConcreteSlotValueType");
                if (outputNodeType == null || concreteValueType == null)
                    return null;
                try
                {
                    var graph = Activator.CreateInstance(GraphData, true);
                    GraphData.GetProperty("isSubGraph", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.SetValue(graph, true);

                    var outputNode = Activator.CreateInstance(outputNodeType, true);
                    var addNode = GraphData.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "AddNode" && m.GetParameters().Length >= 1 &&
                                             m.GetParameters()[0].ParameterType.IsInstanceOfType(outputNode));
                    // The signature has gained optional parameters across versions, so fill the
                    // trailing ones from their declared defaults.
                    addNode?.Invoke(graph, FillDefaults(addNode, outputNode));
                    GraphData.GetProperty("outputNode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.SetValue(graph, outputNode);

                    var addSlot = outputNodeType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "AddSlot" && m.GetParameters().Length == 1 &&
                                             m.GetParameters()[0].ParameterType == concreteValueType);
                    addSlot?.Invoke(outputNode, new[] { Enum.Parse(concreteValueType, "Vector4") });

                    GraphData.GetProperty("path", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.SetValue(graph, "Sub Graphs");
                    return (string)m_Serialize.Invoke(null, new[] { graph });
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"ShaderWaitress: cannot build the subgraph starter: {e.GetBaseException().Message}");
                    return null;
                }
            }

            public string BuildStarterGraph(Type targetType, Type subTargetType, object[] blocks)
            {
                var target = Activator.CreateInstance(targetType, true);
                if (subTargetType != null)
                {
                    var trySet = targetType.GetMethod("TrySetActiveSubTarget",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (trySet == null || !(bool)trySet.Invoke(target, new object[] { subTargetType }))
                        return null;
                }

                var graph = Activator.CreateInstance(GraphData, true);
                Invoke(graph, "AddContexts");

                var targetArray = Array.CreateInstance(Target, 1);
                targetArray.SetValue(target, 0);
                var blockArray = Array.CreateInstance(BlockFieldDescriptor, blocks.Length);
                for (var i = 0; i < blocks.Length; i++)
                    blockArray.SetValue(blocks[i], i);

                var initialize = GraphData.GetMethod("InitializeOutputs",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                initialize?.Invoke(graph, new object[] { targetArray, blockArray });

                var defaultCategory = CategoryData.GetMethod("DefaultCategory",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var addCategory = GraphData.GetMethod("AddCategory",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (defaultCategory != null && addCategory != null)
                {
                    var parameters = defaultCategory.GetParameters();
                    var category = defaultCategory.Invoke(null, parameters.Select(p => p.HasDefaultValue ? p.DefaultValue : null).ToArray());
                    addCategory.Invoke(graph, new[] { category });
                }

                var pathProperty = GraphData.GetProperty("path",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                pathProperty?.SetValue(graph, "Shader Graphs");

                return (string)m_Serialize.Invoke(null, new[] { graph });
            }

            static void Invoke(object instance, string method)
            {
                instance.GetType()
                    .GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(instance, null);
            }
        }

        // ------------------------------------------------------------ json writer

        sealed class JsonWriter
        {
            readonly StringBuilder m_Sb = new StringBuilder();
            readonly Stack<bool> m_First = new Stack<bool>();

            public void BeginObject(string name = null)
            {
                Separator();
                if (name != null)
                    m_Sb.Append('"').Append(name).Append("\": ");
                m_Sb.Append('{');
                m_First.Push(true);
            }

            public void EndObject()
            {
                m_First.Pop();
                m_Sb.Append('}');
            }

            public void BeginArray(string name)
            {
                Separator();
                m_Sb.Append('"').Append(name).Append("\": [");
                m_First.Push(true);
            }

            public void EndArray()
            {
                m_First.Pop();
                m_Sb.Append(']');
            }

            public void Field(string name, string value)
            {
                Separator();
                m_Sb.Append('"').Append(name).Append("\": ");
                Escape(value);
            }

            public void Field(string name, bool value)
            {
                Separator();
                m_Sb.Append('"').Append(name).Append("\": ").Append(value ? "true" : "false");
            }

            public void Field(string name, int value)
            {
                Separator();
                m_Sb.Append('"').Append(name).Append("\": ").Append(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            public void StringArray(string name, IEnumerable<string> values)
            {
                Separator();
                m_Sb.Append('"').Append(name).Append("\": [");
                var first = true;
                foreach (var value in values)
                {
                    if (!first)
                        m_Sb.Append(", ");
                    first = false;
                    Escape(value);
                }
                m_Sb.Append(']');
            }

            void Separator()
            {
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

            void Escape(string value)
            {
                if (value == null)
                {
                    m_Sb.Append("null");
                    return;
                }
                m_Sb.Append('"');
                foreach (var c in value)
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
