using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabWaitress.Commands;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Testing
{
    /// <summary>
    /// Offline checks of the resolver on a hand-written project: a base prefab, a variant, a scene instancing the variant,
    /// and a prefab nesting the base, each exercising one of the rules the Editor follows.
    /// </summary>
    public static class SelfTest
    {
        const string BaseGuid = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1";
        const string VariantGuid = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa2";
        const string SceneGuid = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa3";
        const string NestGuid = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa4";
        const long V = 100, S = 300, N = 400;

        public static int Run(TextWriter w)
        {
            var dir = Path.Combine(Path.GetTempPath(), "prefabwaitress-selftest-" + Guid.NewGuid().ToString("N"));
            var failures = 0;
            try
            {
                Write(dir);
                var project = new Project(dir);
                var tests = new (string name, Action<Project> run)[]
                {
                    ("YAML: flow maps, quoted and folded scalars, sequences", _ => Yaml()),
                    ("variant overrides apply over the base", Variant),
                    ("scene overrides apply over the variant; each layer is recorded", SceneLayers),
                    ("list size and element overrides", Lists),
                    ("unused overrides: missing property, missing target", Unused),
                    ("redundant override is applied and flagged", Redundant),
                    ("removed component drops out of the object", Removed),
                    ("added object and stripped ids that break the XOR rule", Added),
                    ("prefab roots take the file name; instance names override it", Names),
                    ("SceneRoots lists instance roots by PrefabInstance", SceneOrder),
                    ("references inside a source point at the instance's copies", References),
                    ("nested prefab: overrides reach through two layers", Nested),
                    ("property paths: short and Unity forms", _ => Paths()),
                    ("DLL script file ids (MD4 of namespace and class)", _ => ScriptIds()),
                };
                foreach (var (name, run) in tests)
                {
                    try
                    {
                        run(project);
                        w.WriteLine("ok    " + name);
                    }
                    catch (Exception e)
                    {
                        failures++;
                        w.WriteLine("FAIL  " + name + ": " + e.Message);
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
            w.WriteLine(failures == 0 ? "all passed" : $"{failures} failed");
            return failures == 0 ? 0 : 1;
        }

        static void Check(bool ok, string what)
        {
            if (!ok)
                throw new Exception(what);
        }

        static string Value(Obj o, string path) =>
            PropPath.TryResolve(o.Body, path, out var slot, out _) ? Dump.Value(slot.Get()) : "(missing)";

        static Obj At(Asset a, string path, string type)
        {
            var go = Describe.FindOne(a, path);
            return type == "GameObject" ? go : go.Components.First(c => c.TypeName == type);
        }

        static void Yaml()
        {
            var docs = YamlReader.Parse("%YAML 1.1\n--- !u!114 &5\nMonoBehaviour:\n  a: {x: 1, y: -2.5}\n  b: 'it''s'\n  c: \"x\\ny\"\n  d: long text\n    continues\n  e:\n  - k: 1\n    l: [1, 2]\n  - k: 2\n    l: []\n  f: {fileID: 7, guid: 0123456789abcdef0123456789abcdef,\n    type: 3}\n", "t");
            var b = docs[0].Body;
            Check(b.Map("a").Scalar("y") == "-2.5", "flow map");
            Check(b.Scalar("b") == "it's", "single quoted");
            Check(b.Scalar("c") == "x\ny", "double quoted escape");
            Check(b.Scalar("d") == "long text continues", "folded plain scalar");
            Check(b.Seq("e").Items.Count == 2 && ((YMap)b.Seq("e").Items[1]).Scalar("k") == "2", "sequence of maps");
            Check(b["f"].AsRef().Guid == "0123456789abcdef0123456789abcdef" && b["f"].AsRef().FileId == 7, "wrapped reference");
        }

        static void Variant(Project p)
        {
            var a = p.Load("Assets/Variant.prefab");
            var box = At(a, "Variant", "BoxCollider");
            Check(Value(box, "m_Size.x") == "5", "m_Size.x is " + Value(box, "m_Size.x"));
            Check(Value(box, "m_Size.z") == "1", "m_Size.z keeps the base value");
            Check(box.SetBy.TryGetValue("m_Size.x", out var m) && m.Layer.Owner == a, "the variant set m_Size.x");
        }

        static void SceneLayers(Project p)
        {
            var a = p.Load("Assets/Scene.unity");
            var box = At(a, "Variant", "BoxCollider");
            Check(Value(box, "m_Size.x") == "7", "scene value is " + Value(box, "m_Size.x"));
            Check(box.SetBy["m_Size.x"].Layer.Owner == a, "scene set it");
            Check(Value(box.Source, "m_Size.x") == "5" && Value(box.Source.Source, "m_Size.x") == "1", "layers below keep their values");
        }

        static void Lists(Project p)
        {
            var box = At(p.Load("Assets/Scene.unity"), "Variant", "BoxCollider");
            Check(Value(box, "m_Items.Array.size") == "3", "size " + Value(box, "m_Items.Array.size"));
            Check(Value(box, "m_Items.Array.data[2]") == "9", "element " + Value(box, "m_Items.Array.data[2]"));
            Check(Value(box, "m_Items.Array.data[0]") == "1", "kept element");
        }

        static void Unused(Project p)
        {
            var mods = p.Load("Assets/Variant.prefab").Layers[0].Mods;
            Check(mods.Single(m => m.Path == "m_Gone").Status == ModStatus.NoProperty, "m_Gone");
            Check(mods.Single(m => m.Target.FileId == 999).Status == ModStatus.NoTarget, "target 999");
        }

        static void Redundant(Project p)
        {
            var mods = p.Load("Assets/Variant.prefab").Layers[0].Mods;
            Check(mods.Single(m => m.Path == "m_Size.y").Status == ModStatus.Redundant, "m_Size.y");
            Check(mods.Single(m => m.Path == "m_Size.x").Status == ModStatus.Applied, "m_Size.x");
        }

        static void Removed(Project p)
        {
            var a = p.Load("Assets/Nest.prefab");
            var door = Describe.FindOne(a, "Nest/Inner");
            Check(door.Components.All(c => c.TypeName != "BoxCollider"), "the collider is gone");
            Check(door.Components.Count == 1, "only the transform remains");
            Check(door.Body.Seq("m_Component").Items.Count == 1, "m_Component lists only the transform");
        }

        static void Added(Project p)
        {
            var a = p.Load("Assets/Scene.unity");
            var root = Describe.FindOne(a, "Variant");
            var names = root.Children.Select(c => c.Name).ToList();
            Check(names.SequenceEqual(new[] { "Child", "Extra", "Kid" }), "children " + string.Join(",", names));
            var t = root.Transform;
            Check(t.Id == 12345, "the stripped id names the instance's transform");
            Check(Describe.FindOne(a, "Variant/Kid").Source == null, "Kid is the scene's own");
        }

        static void Names(Project p)
        {
            Check(p.Load("Assets/Base.prefab").Roots[0].Name == "Base", "base root takes the file name");
            Check(p.Load("Assets/Variant.prefab").Roots[0].Name == "Variant", "variant root takes the file name");
            Check(Describe.FindOne(p.Load("Assets/Nest.prefab"), "Nest/Inner") != null, "nested instance takes its overridden name");
        }

        static void SceneOrder(Project p)
        {
            var roots = p.Load("Assets/Scene.unity").Roots.Select(r => r.Name).ToList();
            Check(roots.SequenceEqual(new[] { "Variant", "Ground" }), "roots " + string.Join(",", roots));
        }

        static void References(Project p)
        {
            var a = p.Load("Assets/Scene.unity");
            var box = At(a, "Variant", "BoxCollider");
            var go = Describe.FindOne(a, "Variant");
            Check(box.Body["m_GameObject"].AsRef().FileId == go.Id, "m_GameObject points at the scene's copy");
            Check(go.Transform.Body["m_Father"].AsRef().IsNull, "the instance root has no parent");
        }

        static void Nested(Project p)
        {
            var a = p.Load("Assets/Nest.prefab");
            var inner = Describe.FindOne(a, "Nest/Inner");
            Check(Value(inner.Transform, "m_LocalPosition.y") == "3", "nested override");
            Check(inner.Source.Owner.Path == "Assets/Base.prefab", "source chain");
        }

        static void ScriptIds()
        {
            Check(ScriptNames.FileId("UnityEngine.UI", "Image") == -765806418, "Image");
            Check(ScriptNames.FileId("UnityEngine.UI", "Text") == 708705254, "Text");
            Check(ScriptNames.FileId("UnityEngine.UI", "Button") == 1392445389, "Button");
        }

        static void Paths()
        {
            Check(PropPath.ToUnity("m_Materials[0]") == "m_Materials.Array.data[0]", "short to Unity");
            Check(PropPath.ToUnity("a.b[1].c") == "a.b.Array.data[1].c", "nested element");
            Check(PropPath.ToShort("m_Items.Array.data[2]") == "m_Items[2]" && PropPath.ToShort("m_Items.Array.size") == "m_Items.size", "Unity to short");
        }

        static string Id(long instance, long source) => Asset.InstancedId(instance, source).ToString();

        static void Write(string dir)
        {
            Directory.CreateDirectory(Path.Combine(dir, "ProjectSettings"));
            var assets = Path.Combine(dir, "Assets");
            Directory.CreateDirectory(assets);
            void File(string name, string guid, string text)
            {
                System.IO.File.WriteAllText(Path.Combine(assets, name), "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n" + text);
                System.IO.File.WriteAllText(Path.Combine(assets, name + ".meta"), $"fileFormatVersion: 2\nguid: {guid}\n");
            }

            File("Base.prefab", BaseGuid,
                "--- !u!1 &1\nGameObject:\n  m_Component:\n  - component: {fileID: 2}\n  - component: {fileID: 3}\n  m_Name: Door\n  m_IsActive: 1\n" +
                "--- !u!4 &2\nTransform:\n  m_GameObject: {fileID: 1}\n  m_LocalPosition: {x: 0, y: 0, z: 0}\n  m_Children:\n  - {fileID: 5}\n  m_Father: {fileID: 0}\n" +
                "--- !u!65 &3\nBoxCollider:\n  m_GameObject: {fileID: 1}\n  m_Size: {x: 1, y: 1, z: 1}\n  m_Items:\n  - 1\n  - 2\n  m_Owner: {fileID: 1}\n" +
                "--- !u!1 &4\nGameObject:\n  m_Component:\n  - component: {fileID: 5}\n  m_Name: Child\n  m_IsActive: 1\n" +
                "--- !u!4 &5\nTransform:\n  m_GameObject: {fileID: 4}\n  m_LocalPosition: {x: 0, y: 1, z: 0}\n  m_Children: []\n  m_Father: {fileID: 2}\n");

            string Mod(long target, string guid, string path, string value) =>
                $"    - target: {{fileID: {target}, guid: {guid}, type: 3}}\n      propertyPath: {path}\n      value: {value}\n      objectReference: {{fileID: 0}}\n";

            File("Variant.prefab", VariantGuid,
                $"--- !u!1001 &{V}\nPrefabInstance:\n  m_Modification:\n    m_TransformParent: {{fileID: 0}}\n    m_Modifications:\n" +
                Mod(3, BaseGuid, "m_Size.x", "5") + Mod(3, BaseGuid, "m_Size.y", "1") + Mod(3, BaseGuid, "m_Items.Array.size", "3") +
                Mod(3, BaseGuid, "m_Items.Array.data[2]", "9") + Mod(3, BaseGuid, "m_Gone", "1") + Mod(999, BaseGuid, "m_Name", "x") +
                $"    m_RemovedComponents: []\n  m_SourcePrefab: {{fileID: 100100000, guid: {BaseGuid}, type: 3}}\n" +
                $"--- !u!4 &{Id(V, 2)} stripped\nTransform:\n  m_CorrespondingSourceObject: {{fileID: 2, guid: {BaseGuid}, type: 3}}\n  m_PrefabInstance: {{fileID: {V}}}\n" +
                "--- !u!1 &200\nGameObject:\n  m_Component:\n  - component: {fileID: 201}\n  m_Name: Extra\n  m_IsActive: 1\n" +
                $"--- !u!4 &201\nTransform:\n  m_GameObject: {{fileID: 200}}\n  m_LocalPosition: {{x: 0, y: 0, z: 0}}\n  m_Children: []\n  m_Father: {{fileID: {Id(V, 2)}}}\n");

            // The scene's stripped transform has an id that doesn't follow the XOR rule, as in files from before 2018.3.
            var variantBox = Asset.InstancedId(V, 3);
            File("Scene.unity", SceneGuid,
                "--- !u!1 &10\nGameObject:\n  m_Component:\n  - component: {fileID: 11}\n  m_Name: Ground\n  m_IsActive: 1\n" +
                "--- !u!4 &11\nTransform:\n  m_GameObject: {fileID: 10}\n  m_Children: []\n  m_Father: {fileID: 0}\n" +
                $"--- !u!1001 &{S}\nPrefabInstance:\n  m_Modification:\n    m_TransformParent: {{fileID: 0}}\n    m_Modifications:\n" +
                Mod(variantBox, VariantGuid, "m_Size.x", "7") +
                $"    m_RemovedComponents: []\n  m_SourcePrefab: {{fileID: 100100000, guid: {VariantGuid}, type: 3}}\n" +
                $"--- !u!4 &12345 stripped\nTransform:\n  m_CorrespondingSourceObject: {{fileID: {Id(V, 2)}, guid: {VariantGuid}, type: 3}}\n  m_PrefabInstance: {{fileID: {S}}}\n" +
                "--- !u!1 &20\nGameObject:\n  m_Component:\n  - component: {fileID: 21}\n  m_Name: Kid\n  m_IsActive: 1\n" +
                "--- !u!4 &21\nTransform:\n  m_GameObject: {fileID: 20}\n  m_Children: []\n  m_Father: {fileID: 12345}\n" +
                $"--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_Roots:\n  - {{fileID: {S}}}\n  - {{fileID: 11}}\n");

            File("Nest.prefab", NestGuid,
                "--- !u!1 &1\nGameObject:\n  m_Component:\n  - component: {fileID: 2}\n  m_Name: Whatever\n  m_IsActive: 1\n" +
                $"--- !u!4 &2\nTransform:\n  m_GameObject: {{fileID: 1}}\n  m_Children:\n  - {{fileID: {Id(N, 2)}}}\n  m_Father: {{fileID: 0}}\n" +
                $"--- !u!1001 &{N}\nPrefabInstance:\n  m_Modification:\n    m_TransformParent: {{fileID: 2}}\n    m_Modifications:\n" +
                Mod(1, BaseGuid, "m_Name", "Inner") + Mod(2, BaseGuid, "m_LocalPosition.y", "3") +
                $"    m_RemovedComponents:\n    - {{fileID: 3, guid: {BaseGuid}, type: 3}}\n  m_SourcePrefab: {{fileID: 100100000, guid: {BaseGuid}, type: 3}}\n" +
                $"--- !u!4 &{Id(N, 2)} stripped\nTransform:\n  m_CorrespondingSourceObject: {{fileID: 2, guid: {BaseGuid}, type: 3}}\n  m_PrefabInstance: {{fileID: {N}}}\n");
        }
    }
}
