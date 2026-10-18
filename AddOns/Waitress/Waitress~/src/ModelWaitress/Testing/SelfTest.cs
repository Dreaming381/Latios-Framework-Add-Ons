using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ModelWaitress.Commands;
using ModelWaitress.Model;

namespace ModelWaitress.Testing
{
    /// <summary>
    /// Offline checks on a throwaway project: record parsing, model discovery and naming, staleness, find conditions and
    /// the rules the summaries state. Nothing here needs the Editor.
    /// </summary>
    public static class SelfTest
    {
        public static int Run(TextWriter w)
        {
            var root = Path.Combine(Path.GetTempPath(), "modelwaitress-selftest-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            var failed = 0;
            var passed = 0;
            void Check(string name, Action test)
            {
                try
                {
                    test();
                    passed++;
                }
                catch (Exception e)
                {
                    failed++;
                    w.WriteLine($"FAIL {name}: {e.Message}");
                }
            }

            try
            {
                Build(root);
                var catalog = new Catalog(root);

                Check("discovery finds models by their .meta importer, skipping ~ folders", () =>
                {
                    var paths = catalog.All.Select(f => f.AssetPath).ToList();
                    Equal("Assets/Art/Hero.fbx|Assets/Art/Props/Crate.obj|Assets/Other/Hero.fbx", string.Join("|", paths));
                });

                Check("a unique file name resolves; a shared one lists the candidates", () =>
                {
                    Equal("Assets/Art/Props/Crate.obj", catalog.Resolve("Crate.obj").AssetPath);
                    Equal("Assets/Art/Props/Crate.obj", catalog.Resolve("Crate").AssetPath);
                    var e = Throws(() => catalog.Resolve("Hero.fbx"));
                    True(e.Contains("2 models") && e.Contains("Assets/Other/Hero.fbx"), e);
                    Equal("Assets/Other/Hero.fbx", catalog.Resolve("Other/Hero.fbx").AssetPath);
                });

                Check("a folder expands to the models under it", () =>
                {
                    True(catalog.TryFolder("Assets/Art", out var under), "not a folder");
                    Equal(2, under.Count);
                });

                Check("a record matching the stamp is current and read without the Editor", () =>
                {
                    var hero = catalog.Resolve("Assets/Art/Hero.fbx");
                    WriteRecord(catalog, hero, Catalog.Stamp(hero.FullPath));
                    var r = catalog.Get(hero, TextWriter.Null);
                    False(r.Stale, "marked stale");
                    Equal("Human", r.Get("animationType"));
                });

                Check("a touched .meta makes the record stale; without an Editor the last export is shown with a note", () =>
                {
                    var hero = catalog.Resolve("Assets/Art/Hero.fbx");
                    WriteRecord(catalog, hero, Catalog.Stamp(hero.FullPath));
                    File.SetLastWriteTimeUtc(hero.FullPath + ".meta", DateTime.UtcNow.AddMinutes(1));
                    var notes = new StringWriter();
                    var r = catalog.Get(hero, notes);
                    True(r.Stale, "not marked stale");
                    True(notes.ToString().Contains("changed since export"), notes.ToString());
                });

                Check("a model never exported fails without an Editor", () =>
                {
                    var crate = catalog.Resolve("Crate.obj");
                    var e = Throws(() => catalog.Get(crate, TextWriter.Null));
                    True(e.Contains("hasn't been exported"), e);
                });

                Check("pruning removes records of models gone from the project and keeps the rest", () =>
                {
                    var hero = catalog.Resolve("Assets/Art/Hero.fbx");
                    WriteRecord(catalog, hero, Catalog.Stamp(hero.FullPath));
                    var orphan = catalog.FileFor("99999999999999999999999999999999");
                    File.WriteAllText(orphan, "{}");
                    Equal(1, catalog.Prune());
                    False(File.Exists(orphan), "orphan kept");
                    True(File.Exists(catalog.FileFor(hero.Guid)), "live record removed");
                });

                Check("record parsing keeps settings, defaults, nodes, meshes and clips", () =>
                {
                    var r = ModelRecord.Parse(RecordJson("Assets/Art/Hero.fbx", "g", "s"));
                    True(r.Settings.Single(s => s.Name == "globalScale").Changed, "globalScale not changed");
                    False(r.Settings.Single(s => s.Name == "isReadable").Changed, "isReadable changed");
                    Equal("Root/Body", "Root/" + r.PathOf(1));
                    Equal(0, r.Nodes[1].Parent);
                    Equal(1, r.Nodes[0].Children.Count);
                    Equal(1200L, r.Meshes[0].Verts);
                    Equal("Run", r.Anims[0].Name);
                    Equal("1", r.Clips[0].Get("loopTime") == "true" ? "1" : "0");
                    Equal("Hips", r.HumanMap[0].human);
                });

                Check("effective scale is globalScale times the fileScale the importer applied", () =>
                {
                    var r = ModelRecord.Parse(RecordJson("a", "g", "s"));
                    Equal("0.01", Query.Value(r, "scale"));
                    r.Settings.Single(s => s.Name == "useFileScale").Value = "true";
                    r.FileScale = 0.01;
                    Equal("0.0001", Query.Value(r, "scale"));
                });

                Check("optimizeGameObjects with no avatar is reported as doing nothing", () =>
                {
                    var r = ModelRecord.Parse(RecordJson("a", "g", "s"));
                    r.Settings.Single(s => s.Name == "optimizeGameObjects").Value = "true";
                    True(Read.OptimizeIdle(r)?.Contains("needs an avatar") == true, Read.OptimizeIdle(r) ?? "null");
                    r.Avatars.Add(("HeroAvatar", true, true));
                    r.Nodes.RemoveAt(1);
                    Equal(null, Read.OptimizeIdle(r));
                });

                Check("size places each mesh by its node's rotation (Unity's Z, X, Y euler order)", () =>
                {
                    var r = ModelRecord.Parse(RecordJson("a", "g", "s"));
                    r.Meshes[0].Center = new double[] { 0, 0, 1 };
                    r.Meshes[0].Extents = new double[] { 1, 2, 3 };
                    r.Nodes[1].R = new double[] { 270, 0, 0 };
                    Equal("2 × 6 × 4 m (x × y × z), y from -2 to 4", Read.SizeLine(r));
                    r.Nodes[1].R = new double[] { 0, 90, 90 };
                    Equal("6 × 2 × 4 m (x × y × z), y from -1 to 1", Read.SizeLine(r));
                });

                Check("conditions parse the earliest operator, two-character ones first", () =>
                {
                    Equal(("verts", ">=", "10"), Query.ParseCondition("verts>=10"));
                    Equal(("a", "!=", "b=c"), Query.ParseCondition("a!=b=c"));
                    Equal(("changed", "~", "scale"), Query.ParseCondition("changed~scale"));
                    Equal((null, null, null), Query.ParseCondition("Assets/Art"));
                });

                Check("comparisons: numbers numerically, text ignoring case, lists by length", () =>
                {
                    True(Query.Test("65536", ">", "65535"), ">");
                    True(Query.Test("Human", "=", "human"), "=");
                    True(Query.Test("a,b,c", "=", "3"), "list length");
                    True(Query.Test("isReadable,globalScale", "~", "global"), "~");
                    True(Query.Test("", "=", "0"), "empty list");
                });

                Check("a binary .fbx header gives the application, axes and unit, skipping other nodes", () =>
                {
                    var file = Path.Combine(root, "header.fbx");
                    File.WriteAllBytes(file, BinaryFbx());
                    var h = FbxHeader.Read(file) ?? throw new Exception("not read");
                    Equal("Blender (stable FBX IO)", h.Application);
                    Equal("Z-up, -Y front", h.Axes);
                    Equal("FBX 7400 binary, from Blender (stable FBX IO), Z-up, -Y front, unit m", h.Describe());
                });

                Check("an ASCII .fbx header is read too", () =>
                {
                    var file = Path.Combine(root, "ascii.fbx");
                    File.WriteAllText(file, "; FBX 7.4.0 project file\nCreator: \"Some Exporter\"\nGlobalSettings:  {\n\tProperties70:  {\n" +
                                            "\t\tP: \"UpAxis\", \"int\", \"Integer\", \"\",1\n\t\tP: \"UnitScaleFactor\", \"double\", \"Number\", \"\",2.54\n\t}\n}\n");
                    var h = FbxHeader.Read(file) ?? throw new Exception("not read");
                    Equal("FBX 7400 ASCII, by Some Exporter, Y-up, +Z front, unit inch", h.Describe());
                });

                Check("every command has help", () =>
                {
                    foreach (var c in Cli.CommandRouter.Commands)
                    {
                        var sw = new StringWriter();
                        Cli.Help.Topic(sw, c);
                        True(sw.ToString().Trim().Length > 0, c);
                    }
                });
            }
            finally
            {
                try
                {
                    Directory.Delete(root, true);
                }
                catch (IOException)
                {
                }
            }
            w.WriteLine(failed == 0 ? $"selftest: {passed} passed" : $"selftest: {failed} failed, {passed} passed");
            return failed == 0 ? 0 : 1;
        }

        static void Build(string root)
        {
            Directory.CreateDirectory(Path.Combine(root, "ProjectSettings"));
            void Model(string rel, string guid, string importer = "ModelImporter")
            {
                var full = Path.Combine(root, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(full));
                File.WriteAllText(full, "model");
                File.WriteAllText(full + ".meta", $"fileFormatVersion: 2\nguid: {guid}\n{importer}:\n  serializedVersion: 22200\n");
            }
            Model("Assets/Art/Hero.fbx", "11111111111111111111111111111111");
            Model("Assets/Art/Props/Crate.obj", "22222222222222222222222222222222");
            Model("Assets/Other/Hero.fbx", "33333333333333333333333333333333");
            Model("Assets/Art/Samples~/Ignored.fbx", "44444444444444444444444444444444");
            Model("Assets/Art/Scripted.fbx", "55555555555555555555555555555555", "ScriptedImporter");
        }

        static void WriteRecord(Catalog catalog, ModelFile f, string stamp)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(catalog.FileFor(f.Guid)));
            File.WriteAllText(catalog.FileFor(f.Guid), RecordJson(f.AssetPath, f.Guid, stamp));
        }

        static string RecordJson(string path, string guid, string stamp) => JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["v"] = 1, ["path"] = path, ["guid"] = guid, ["stamp"] = stamp, ["unity"] = "6000.3", ["importer"] = "ModelImporter",
            ["settings"] = new[]
            {
                new[] { "animationType", "ModelImporterAnimationType", "Human", "Generic" },
                new[] { "globalScale", "float", "0.01", "1" },
                new[] { "isReadable", "bool", "false", "false" },
                new[] { "useFileScale", "bool", "false", "true" },
                new[] { "optimizeGameObjects", "bool", "false", "false" },
                new[] { "avatarSetup", "ModelImporterAvatarSetup", "NoAvatar", "NoAvatar" },
            },
            ["file"] = new Dictionary<string, object>
            {
                ["fileScale"] = 1, ["unit"] = "cm", ["unitFactor"] = 0.01, ["transformPaths"] = new[] { "", "Body" }, ["referencedClips"] = Array.Empty<string>(),
                ["takes"] = new[] { new Dictionary<string, object> { ["name"] = "Take 001", ["clip"] = "Take 001", ["start"] = 0, ["stop"] = 1, ["fps"] = 30 } },
            },
            ["clipsCustom"] = true,
            ["clips"] = new[]
            {
                new Dictionary<string, object>
                {
                    ["name"] = "Run", ["take"] = "Take 001", ["mask"] = null, ["events"] = Array.Empty<object>(), ["curves"] = Array.Empty<string>(),
                    ["settings"] = new[] { new[] { "loopTime", "bool", "true", "false" } },
                },
            },
            ["human"] = new Dictionary<string, object> { ["map"] = new[] { new[] { "Hips", "Body" } }, ["skeleton"] = 2, ["feetSpacing"] = 0 },
            ["remaps"] = Array.Empty<object>(), ["sourceMaterials"] = Array.Empty<string>(), ["messages"] = Array.Empty<object>(),
            ["nodes"] = new[]
            {
                new Dictionary<string, object> { ["name"] = "Root", ["parent"] = -1, ["p"] = new[] { 0, 0, 0 }, ["r"] = new[] { 0, 0, 0 }, ["s"] = new[] { 1, 1, 1 }, ["c"] = Array.Empty<object>() },
                new Dictionary<string, object> { ["name"] = "Body", ["parent"] = 0, ["p"] = new[] { 0, 0, 0 }, ["r"] = new[] { -90, 0, 0 }, ["s"] = new[] { 1, 1, 1 }, ["c"] = new object[] { new object[] { "MeshFilter", 0 } } },
            },
            ["meshes"] = new[]
            {
                new Dictionary<string, object>
                {
                    ["name"] = "Body", ["id"] = "4300000", ["verts"] = 1200, ["readable"] = false, ["index"] = "UInt16", ["tris"] = 2000, ["bindposes"] = 0,
                    ["sub"] = new[] { new object[] { "Triangles", 6000, 0, 1200, 0 } }, ["attrs"] = new[] { new object[] { "Position", "Float32", 3, 0 } },
                    ["bounds"] = new[] { new[] { 0, 0, 0 }, new[] { 1, 1, 1 } }, ["shapes"] = Array.Empty<object>(),
                },
            },
            ["avatars"] = Array.Empty<object>(),
            ["anims"] = new[]
            {
                new Dictionary<string, object> { ["name"] = "Run", ["id"] = "7400000", ["length"] = 1, ["fps"] = 30, ["loop"] = true, ["averageSpeed"] = new[] { 0, 0, 1 }, ["paths"] = Array.Empty<object>() },
            },
            ["subassets"] = Array.Empty<object>(),
        });

        // A minimal FBX 7400 binary: the header extension, a big node to skip, and GlobalSettings.
        static byte[] BinaryFbx()
        {
            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.Write(System.Text.Encoding.ASCII.GetBytes("Kaydara FBX Binary  \0"));
            w.Write((byte)0x1A);
            w.Write((byte)0);
            w.Write(7400);
            void P(params object[] values) => Node(w, "P", values, null);
            Node(w, "FBXHeaderExtension", new object[0], () =>
                Node(w, "SceneInfo", new object[] { "GlobalInfo\0\u0001SceneInfo", "UserData" }, () =>
                    Node(w, "Properties70", new object[0], () => P("Original|ApplicationName", "KString", "", "", "Blender (stable FBX IO)"))));
            Node(w, "Objects", new object[] { new double[] { 1, 2, 3 } }, () => Node(w, "P", new object[] { "Decoy", "", "", "", "wrong" }, null));
            Node(w, "GlobalSettings", new object[0], () => Node(w, "Properties70", new object[0], () =>
            {
                P("UpAxis", "int", "Integer", "", 2);
                P("UpAxisSign", "int", "Integer", "", 1);
                P("FrontAxis", "int", "Integer", "", 1);
                P("FrontAxisSign", "int", "Integer", "", -1);
                P("UnitScaleFactor", "double", "Number", "", 100.0);
            }));
            w.Write(new byte[13]);
            return ms.ToArray();
        }

        static void Node(BinaryWriter w, string name, object[] props, Action children)
        {
            var s = w.BaseStream;
            var start = s.Position;
            w.Write(0u);
            w.Write((uint)props.Length);
            w.Write(0u);
            w.Write((byte)name.Length);
            w.Write(System.Text.Encoding.ASCII.GetBytes(name));
            var propsStart = s.Position;
            foreach (var p in props)
            {
                switch (p)
                {
                    case string text:
                        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                        w.Write((byte)'S');
                        w.Write(bytes.Length);
                        w.Write(bytes);
                        break;
                    case int i:
                        w.Write((byte)'I');
                        w.Write(i);
                        break;
                    case double d:
                        w.Write((byte)'D');
                        w.Write(d);
                        break;
                    case double[] array:
                        w.Write((byte)'d');
                        w.Write(array.Length);
                        w.Write(0);
                        w.Write(array.Length * 8);
                        foreach (var d in array)
                            w.Write(d);
                        break;
                }
            }
            var propsEnd = s.Position;
            if (children != null)
            {
                children();
                w.Write(new byte[13]);
            }
            var end = s.Position;
            s.Position = start;
            w.Write((uint)end);
            w.Write((uint)props.Length);
            w.Write((uint)(propsEnd - propsStart));
            s.Position = end;
        }

        static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception($"expected {expected}, got {actual}");
        }

        static void True(bool b, string what)
        {
            if (!b)
                throw new Exception("expected true: " + what);
        }

        static void False(bool b, string what) => True(!b, what);

        static string Throws(Action a)
        {
            try
            {
                a();
            }
            catch (WaitressException e)
            {
                return e.Message;
            }
            throw new Exception("expected an error");
        }
    }
}
