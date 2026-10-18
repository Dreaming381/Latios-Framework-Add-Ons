using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxWaitress.Cli;
using VfxWaitress.Model;

namespace VfxWaitress.Testing
{
    /// <summary>
    /// In-process suite covering the YAML container, project and guid lookup, every edit verb,
    /// batch atomicity, id stability, and placement of new nodes. Graphs are built in a throwaway
    /// project folder with the Editor link off. Tests that create nodes need the catalog of the
    /// project the suite is run from, and are skipped without one.
    /// </summary>
    public static class SelfTest
    {
        static int s_Passed;
        static int s_Skipped;
        static readonly List<string> s_Failures = new List<string>();
        static string s_Project;

        public static int Run(Args args, TextWriter output)
        {
            s_Passed = 0;
            s_Skipped = 0;
            s_Failures.Clear();
            var verbose = args.Flag("verbose") || args.Flag("v");
            var filter = args.Get("filter");
            args.RejectUnknown();

            var catalog = Catalog.NodeCatalog.Default;
            if (!catalog.IsEmpty)
                Catalog.NodeCatalog.OverridePath = catalog.Source;
            Edit.EditorLink.Offline = true;

            s_Project = Path.Combine(Path.GetTempPath(), "vfxwaitress-selftest-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(Path.Combine(s_Project, "Assets"));
            Directory.CreateDirectory(Path.Combine(s_Project, "ProjectSettings"));
            try
            {
                foreach (var (name, needsCatalog, test) in Tests())
                {
                    if (filter != null && name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    if (needsCatalog && catalog.IsEmpty)
                    {
                        s_Skipped++;
                        continue;
                    }
                    try
                    {
                        test();
                        s_Passed++;
                        if (verbose)
                            output.WriteLine($"pass  {name}");
                    }
                    catch (Exception e)
                    {
                        s_Failures.Add(name);
                        output.WriteLine($"FAIL  {name}: {e.Message}");
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(s_Project, true);
                }
                catch (Exception)
                {
                }
            }

            if (s_Skipped > 0)
                output.WriteLine($"skipped {s_Skipped} test(s) that need a catalog; run this from a project with UserSettings/Waitress/catalog.json");
            output.WriteLine($"selftest: {s_Passed} passed, {s_Failures.Count} failed");
            return s_Failures.Count == 0 ? 0 : 1;
        }

        static IEnumerable<(string, bool, Action)> Tests()
        {
            yield return ("yaml/new-asset-roundtrips", false, NewAssetRoundTrips);
            yield return ("project/root-and-asset-path", false, ProjectRootIsFound);
            yield return ("guids/package-cache", false, PackageCacheResolvesAsPackages);
            yield return ("bridge/outside-project", false, BridgeRefusesPathsOutsideAProject);
            yield return ("apply/builds-a-system", true, ApplyBuildsASystem);
            yield return ("apply/atomic", true, FailedScriptLeavesFileUntouched);
            yield return ("apply/unbound-label", true, UnboundLabelIsNamed);
            yield return ("apply/editor-verbs-refused", true, EditorVerbsAreRefusedInScripts);
            yield return ("link/merges-systems", true, LinkMergesSystems);
            yield return ("link/fan-out", true, FlowLinksFanOut);
            yield return ("wire/both-ends", true, WiringWritesBothEnds);
            yield return ("set/composite", true, CompositeValuesFillTheTree);
            yield return ("set/leaf-values-checked", true, LeafValuesAreChecked);
            yield return ("set/curve-forms", true, CurveValuesAreEncodedOrRefused);
            yield return ("set/gradient-forms", true, GradientValuesAreEncodedOrRefused);
            yield return ("write/unloadable-value-refused", true, UnloadableValuesAreNeverWritten);
            yield return ("variant/selects-template", true, SettingsSelectTheVariant);
            yield return ("variant/keeps-menu-name", true, MenuNameSurvivesASetting);
            yield return ("rm/cleans-links", true, RemovingANodeCleansItsLinks);
            yield return ("ids/new-ids-dont-collide", true, NewIdsDoNotCollide);
            yield return ("place/beside-consumer", true, NewNodesLandBesideWhatTheyFeed);
            yield return ("place/flow-column", true, NewContextsStackByFlow);
            yield return ("system/set", true, SystemSettingsLandOnTheData);
            yield return ("resource/set", false, ResourceSettingsAreStored);
            yield return ("resource/automatic-bounds", true, AutomaticBoundsKeepCullingOff);
        }

        // ---- helpers ----

        static void Check(bool condition, string message)
        {
            if (!condition)
                throw new Exception(message);
        }

        static void Equal<T>(T expected, T actual, string what)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception($"{what}: expected {expected}, got {actual}");
        }

        static string Sh(params string[] argv)
        {
            var sb = new StringWriter();
            var code = CommandRouter.Run(argv, sb, sb);
            if (code != 0)
                throw new Exception($"'{string.Join(" ", argv)}' returned {code}: {sb}");
            return sb.ToString();
        }

        static string Fails(params string[] argv)
        {
            var sb = new StringWriter();
            try
            {
                var code = CommandRouter.Run(argv, sb, sb);
                if (code == 0)
                    throw new Exception($"'{string.Join(" ", argv)}' should have failed: {sb}");
                return sb.ToString();
            }
            catch (VfxWaitressException e)
            {
                return e.Message;
            }
        }

        static string NewGraph(string name)
        {
            var path = Path.Combine(s_Project, "Assets", name + ".vfx");
            if (File.Exists(path))
                File.Delete(path);
            Sh("new", path);
            return path;
        }

        static string Script(string name, params string[] lines)
        {
            var path = Path.Combine(s_Project, name + ".vfxw");
            File.WriteAllText(path, string.Join("\n", lines));
            return path;
        }

        const string k_System = "node add Spawn --as spawn\n" +
                                "block add $spawn \"Constant Spawn Rate\" --as rate\n" +
                                "node add \"Initialize Particle\" --as init\n" +
                                "node add \"Update Particle\" --as update\n" +
                                "node add \"Output Particle Unlit Quad\" --as output\n" +
                                "link $spawn $init\n" +
                                "link $init $update\n" +
                                "link $update $output\n" +
                                "block add $init \"Set Color\" --as color\n" +
                                "node add \"Random Color\" --as tint --feed $color._Color";

        /// <summary>A spawner feeding one particle system, with a random tint wired into Set Color.</summary>
        static string BuildSystem(string name)
        {
            var path = NewGraph(name);
            Sh("apply", path, Script(name, k_System));
            return path;
        }

        static VfxNode Only(VfxAsset asset, Func<VfxNode, bool> predicate, string what)
        {
            var hits = asset.AllNodes().Where(predicate).ToList();
            Equal(1, hits.Count, "nodes matching " + what);
            return hits[0];
        }

        // ---- container and plumbing ----

        static void NewAssetRoundTrips()
        {
            var path = NewGraph("roundtrip");
            var before = File.ReadAllText(path);
            var asset = VfxAsset.Load(path);
            new Edit.GraphEditor(asset).Save();
            Equal(before, File.ReadAllText(path), "an untouched asset re-serializes byte for byte");
        }

        static void ProjectRootIsFound()
        {
            var file = Path.Combine(s_Project, "Assets", "Deep", "Graph.vfx");
            Equal(Path.GetFullPath(s_Project), UnityProject.FindRoot(file), "the project root above a file that doesn't exist yet");
            Equal("Assets/Deep/Graph.vfx", UnityProject.AssetPath(file), "its asset path");
            Equal(null, UnityProject.AssetPath(Path.Combine(Path.GetTempPath(), "Loose.vfx")), "a file outside any project");
        }

        /// <summary>
        /// Installed packages live in Library/PackageCache/&lt;name&gt;@&lt;hash&gt;, but Unity addresses them as
        /// Packages/&lt;name&gt;. Folders ending in ~ are never imported, and a Samples~ copy shares its
        /// guid with the imported sample, so it must not shadow it.
        /// </summary>
        static void PackageCacheResolvesAsPackages()
        {
            var root = Path.Combine(s_Project, "guids");
            Directory.CreateDirectory(Path.Combine(root, "Assets"));
            Directory.CreateDirectory(Path.Combine(root, "ProjectSettings"));
            var cached = Path.Combine(root, "Library", "PackageCache", "com.example.fx@1a2b3c", "Runtime");
            Directory.CreateDirectory(cached);
            File.WriteAllText(Path.Combine(cached, "Burst.vfxoperator.meta"), "fileFormatVersion: 2\nguid: 11111111111111111111111111111111\n");
            var samples = Path.Combine(root, "Packages", "com.example.local", "Samples~");
            Directory.CreateDirectory(samples);
            File.WriteAllText(Path.Combine(samples, "Copy.vfxoperator.meta"), "fileFormatVersion: 2\nguid: 22222222222222222222222222222222\n");
            File.WriteAllText(Path.Combine(root, "Assets", "Imported.vfxoperator.meta"), "fileFormatVersion: 2\nguid: 22222222222222222222222222222222\n");

            var index = GuidIndex.For(Path.Combine(root, "Assets", "Any.vfx"));
            Equal(Path.Combine(root, "Packages", "com.example.fx", "Runtime", "Burst.vfxoperator"),
                index.PathOf("11111111111111111111111111111111"), "a cached package's asset");
            Equal("Packages/com.example.fx/Runtime/Burst.vfxoperator",
                UnityProject.AssetPath(index.PathOf("11111111111111111111111111111111")), "and the path Unity knows it by");
            Equal("Imported", index.NameOf("22222222222222222222222222222222"), "the imported copy, not the Samples~ one");
        }

        static void BridgeRefusesPathsOutsideAProject()
        {
            var bridge = new EditorBridge("vfxwaitress", "test");
            var reply = bridge.Require(Path.Combine(Path.GetTempPath(), "Loose.vfx"), "Validate");
            Check(reply.StartsWith("error:", StringComparison.Ordinal) && reply.Contains("not under", StringComparison.Ordinal),
                "a path outside any project is refused before the CLI runs: " + reply);
        }

        // ---- editing ----

        static void ApplyBuildsASystem()
        {
            var path = BuildSystem("system");
            var asset = VfxAsset.Load(path);
            Equal(4, asset.TopLevel.Count(n => n.Kind == "context"), "contexts");
            Equal(2, asset.AllNodes().Count(n => n.Kind == "block"), "blocks");
            var findings = Validator.Check(asset);
            Equal(0, findings.Count(f => f.Severity == "error"), "structural errors: " + string.Join("; ", findings));
        }

        static void FailedScriptLeavesFileUntouched()
        {
            var path = BuildSystem("atomic");
            var before = File.ReadAllText(path);
            var message = Fails("apply", path, Script("atomic-bad",
                "node add Add --as sum",
                "system set d0000 capacity=12",
                "wire $sum nothing.here"));
            Check(message.Contains("line 2", StringComparison.Ordinal), "the error names the line: " + message);
            Equal(before, File.ReadAllText(path), "the file after a failed script");
        }

        static void UnboundLabelIsNamed()
        {
            var path = NewGraph("unbound");
            var message = Fails("apply", path, Script("unbound", "node add Add --wire a=$missing"));
            Check(message.Contains("'$missing' has not been bound", StringComparison.Ordinal), "names the label: " + message);
        }

        static void EditorVerbsAreRefusedInScripts()
        {
            var path = NewGraph("editorverbs");
            var message = Fails("apply", path, Script("editorverbs", "resync"));
            Check(message.Contains("can't run inside a script", StringComparison.Ordinal), "explains why: " + message);
        }

        /// <summary>A flow link between particle contexts also puts them in one system.</summary>
        static void LinkMergesSystems()
        {
            var asset = VfxAsset.Load(BuildSystem("merge"));
            var particle = asset.TopLevel.Where(n => n.Kind == "context" && n.DisplayType != "VFXBasicSpawner").Select(n => n.Data).Distinct().ToList();
            Equal(1, particle.Count, "systems shared by initialize, update and output");
        }

        /// <summary>A second link from one flow output adds to the first rather than replacing it.</summary>
        static void FlowLinksFanOut()
        {
            var path = NewGraph("fanout");
            Sh("apply", path, Script("fanout",
                                     "node add Spawn --as spawn",
                                     "node add \"Initialize Particle\" --as init",
                                     "node add \"Output Event\" --as tick",
                                     "link $spawn $init",
                                     "link $spawn $tick"));
            var asset = VfxAsset.Load(path);
            var spawn = Only(asset, n => n.DisplayType == "VFXBasicSpawner", "spawner");
            Equal(2, spawn.FlowOut.Count, "flow links out of the spawner");
            foreach (var flow in spawn.FlowOut)
                Check(asset.Nodes[flow.toContext].FlowIn.Contains((flow.toIndex, spawn.FileId, flow.from)), "the target records the link too");
            Check(Model.Validator.Check(asset).All(f => f.Severity != "error"), "validate finds no error");

            var init = Only(asset, n => n.DisplayType == "VFXBasicInitialize", "initialize");
            init.Doc.Body.SetBlock("m_InputFlowSlot", "  - link: []\n");
            new Edit.GraphEditor(asset).Save();
            Check(Model.Validator.Check(VfxAsset.Load(path)).Any(f => f.Severity == "error" && f.Where == spawn.ShortId), "validate flags a one-sided flow link");
        }

        /// <summary>VFX Graph stores each link on both slots. One end alone is a broken graph.</summary>
        static void WiringWritesBothEnds()
        {
            var asset = VfxAsset.Load(BuildSystem("wire"));
            var tint = Only(asset, n => n.DisplayType == "Random", "the random operator");
            var output = tint.AllOutputSlots.First(s => s.LinkedSlotIds.Count > 0);
            var input = asset.SlotById(output.LinkedSlotIds[0]);
            Check(input != null && input.LinkedSlotIds.Contains(output.FileId), "the input links back to the output");

            var path = asset.Path;
            Sh("unwire", path, input.Owner.ShortId + "." + input.Path);
            var after = VfxAsset.Load(path);
            Check(after.AllNodes().SelectMany(n => n.AllInputSlots.Concat(n.AllOutputSlots)).All(s => s.LinkedSlotIds.Count == 0),
                "unwire clears both ends");
        }

        static void CompositeValuesFillTheTree()
        {
            var path = BuildSystem("composite");
            var block = Only(VfxAsset.Load(path), n => n.Kind == "block" && n.DisplayType == "SetAttribute", "Set Color");
            Sh("unwire", path, block.ShortId + "._Color");
            Sh("set", path, block.ShortId + "._Color", "1,0.5,0");
            Check(Sh("show", path).Contains("_Color=(1,0.5,0)", StringComparison.Ordinal), "the value shows on the master slot");
            var message = Fails("set", path, block.ShortId + "._Color.x", "1");
            Check(message.Contains("child slot", StringComparison.Ordinal), "setting a child is refused: " + message);
        }

        static string SlotValue(string path, string shortId, string slotPath)
        {
            var slot = VfxAsset.Load(path).AllNodes().First(n => n.ShortId == shortId).AllInputSlots.First(s => s.Path == slotPath);
            return slot.Doc.Body.Map("m_MasterData").Map("m_Value").Scalar("m_SerializableObject");
        }

        /// <summary>VFX Graph parses a leaf value by the slot's type, and anything it can't parse loads as null.</summary>
        static void LeafValuesAreChecked()
        {
            var path = BuildSystem("leaf");
            var rate = Only(VfxAsset.Load(path), n => n.Kind == "block" && n.AllInputSlots.Any(s => s.Path == "Rate"), "Constant Spawn Rate");
            var before = File.ReadAllText(path);
            var message = Fails("set", path, rate.ShortId + ".Rate", "fast");
            Check(message.Contains("takes a number", StringComparison.Ordinal) || message.Contains("is not a number", StringComparison.Ordinal),
                "a word on a float is refused: " + message);
            Equal(before, File.ReadAllText(path), "the file after a refused value");
            Sh("set", path, rate.ShortId + ".Rate", "32");
            Equal("32", SlotValue(path, rate.ShortId, "Rate"), "a number is stored bare");

            var flag = Only(VfxAsset.Load(path), n => n.Kind == "block" && n.DisplayType == "SetAttribute", "Set Color");
            var activation = flag.AllInputSlots.FirstOrDefault(s => s.TypeName == "Boolean" && s.Parent == null && s.LinkedSlotIds.Count == 0);
            if (activation != null)
            {
                Sh("set", path, flag.ShortId + "." + activation.Path, "1");
                Equal("True", SlotValue(path, flag.ShortId, activation.Path), "a bool is stored the way Convert.ToBoolean reads it");
            }
        }

        const string k_Curve = "{\"frames\":[{\"time\":0.0,\"value\":1.0,\"inTangent\":0.0,\"outTangent\":-1.0,\"tangentMode\":0,\"leftTangentMode\":0,\"rightTangentMode\":0,\"broken\":false}," +
                               "{\"time\":1.0,\"value\":0.0,\"inTangent\":-1.0,\"outTangent\":0.0,\"tangentMode\":0,\"leftTangentMode\":0,\"rightTangentMode\":0,\"broken\":false}]," +
                               "\"preWrapMode\":8,\"postWrapMode\":8,\"version\":1}";

        static string AddSizeOverLife(string path)
        {
            var update = Only(VfxAsset.Load(path), n => n.DisplayType == "VFXBasicUpdate", "update");
            return AddedId(Sh("block", "add", path, update.ShortId, "Multiply Size Over Life"));
        }

        static string AddedId(string log)
        {
            var line = log.Split('\n').First(l => l.StartsWith("added ", StringComparison.Ordinal));
            var rest = line.Substring(line.IndexOf(" as ", StringComparison.Ordinal) + 4).Trim();
            return rest.Split(' ')[0];
        }

        /// <summary>
        /// A curve slot has no child slots, so it looks like a scalar. Text that isn't VFX Graph's
        /// curve JSON loads as a null curve, and importing that crashed the Editor. So the short
        /// form is encoded, and anything else is refused.
        /// </summary>
        static void CurveValuesAreEncodedOrRefused()
        {
            var path = BuildSystem("curve");
            var sol = AddSizeOverLife(path);
            var slot = VfxAsset.Load(path).AllNodes().First(n => n.ShortId == sol).AllInputSlots.First(s => s.TypeName == "AnimationCurve");
            var before = File.ReadAllText(path);
            var message = Fails("set", path, sol + "." + slot.Path, "fast");
            Check(message.Contains("is not a key", StringComparison.Ordinal), "a word is refused: " + message);
            message = Fails("apply", path, Script("curve", $"set {sol}.{slot.Path} 0:1,1:x"), "--dry-run");
            Check(message.Contains("is not a key", StringComparison.Ordinal), "and refused by a dry run too: " + message);
            message = Fails("set", path, sol + "." + slot.Path, "0:1,0:2");
            Check(message.Contains("two keys at time", StringComparison.Ordinal), "keys sharing a time are refused: " + message);
            message = Fails("set", path, sol + "." + slot.Path, "{\"keys\":[]}");
            Check(message.Contains("\"frames\"", StringComparison.Ordinal), "JSON of the wrong shape is refused: " + message);
            Equal(before, File.ReadAllText(path), "the file after refused curves");

            Sh("set", path, sol + "." + slot.Path, k_Curve);
            Equal("'" + k_Curve + "'", SlotValue(path, sol, slot.Path), "curve JSON is stored quoted, like VFX Graph writes it");
            Check(Sh("show", path).Contains("curve(2)", StringComparison.Ordinal), "and shows as a two-key curve");
            Equal(0, Validator.Check(VfxAsset.Load(path)).Count(f => f.Severity == "error"), "errors after a valid curve");

            Sh("set", path, sol + "." + slot.Path, "1:0, 0:1, 0.5:2");
            Equal("'{\"frames\":[" +
                  "{\"time\":0.0,\"value\":1.0,\"inTangent\":2.0,\"outTangent\":2.0,\"tangentMode\":0,\"leftTangentMode\":2,\"rightTangentMode\":2,\"broken\":true}," +
                  "{\"time\":0.5,\"value\":2.0,\"inTangent\":2.0,\"outTangent\":-4.0,\"tangentMode\":0,\"leftTangentMode\":2,\"rightTangentMode\":2,\"broken\":true}," +
                  "{\"time\":1.0,\"value\":0.0,\"inTangent\":-4.0,\"outTangent\":-4.0,\"tangentMode\":0,\"leftTangentMode\":2,\"rightTangentMode\":2,\"broken\":true}]," +
                  "\"preWrapMode\":8,\"postWrapMode\":8,\"version\":1}'",
                SlotValue(path, sol, slot.Path), "time:value keys sorted, with Linear tangents");
            Equal(0, Validator.Check(VfxAsset.Load(path)).Count(f => f.Severity == "error"), "errors after a short-form curve");
        }

        static void GradientValuesAreEncodedOrRefused()
        {
            var path = BuildSystem("gradient");
            var update = Only(VfxAsset.Load(path), n => n.DisplayType == "VFXBasicUpdate", "update");
            var col = AddedId(Sh("block", "add", path, update.ShortId, "Set Color Over Life"));
            var slot = VfxAsset.Load(path).AllNodes().First(n => n.ShortId == col).AllInputSlots.First(s => s.TypeName == "Gradient");
            var target = col + "." + slot.Path;
            var before = File.ReadAllText(path);
            var message = Fails("set", path, target, "0:#ff80,1:#000000");
            Check(message.Contains("is not a key", StringComparison.Ordinal), "a short hex color is refused: " + message);
            message = Fails("set", path, target, "0:#ffffff,1.5:#000000");
            Check(message.Contains("from 0 to 1", StringComparison.Ordinal), "a time past 1 is refused: " + message);
            message = Fails("set", path, target, string.Join(",", Enumerable.Range(0, 9).Select(i => $"{i / 8.0}:#ffffff")));
            Check(message.Contains("at most 8", StringComparison.Ordinal), "a ninth key is refused: " + message);
            Equal(before, File.ReadAllText(path), "the file after refused gradients");

            Sh("set", path, target, "0:#ff8000,1:#0000ff00");
            Equal("'{\"colorKeys\":[" +
                  "{\"color\":{\"r\":1.0,\"g\":" + (128 / 255.0).ToString("R", System.Globalization.CultureInfo.InvariantCulture) + ",\"b\":0.0,\"a\":1.0},\"time\":0.0}," +
                  "{\"color\":{\"r\":0.0,\"g\":0.0,\"b\":1.0,\"a\":1.0},\"time\":1.0}]," +
                  "\"alphaKeys\":[{\"alpha\":1.0,\"time\":0.0},{\"alpha\":0.0,\"time\":1.0}],\"gradientMode\":0}'",
                SlotValue(path, col, slot.Path), "hex keys with their alpha");
            Check(Sh("show", path).Contains("gradient(2)", StringComparison.Ordinal), "and shows as a two-key gradient");
            Equal(0, Validator.Check(VfxAsset.Load(path)).Count(f => f.Severity == "error"), "errors after a short-form gradient");
        }

        /// <summary>A file holding an unloadable value is reported, and no command writes one.</summary>
        static void UnloadableValuesAreNeverWritten()
        {
            var path = BuildSystem("unloadable");
            var sol = AddSizeOverLife(path);
            var slot = VfxAsset.Load(path).AllNodes().First(n => n.ShortId == sol).AllInputSlots.First(s => s.TypeName == "AnimationCurve");
            var good = SlotValue(path, sol, slot.Path);
            File.WriteAllText(path, File.ReadAllText(path).Replace("m_SerializableObject: " + good, "m_SerializableObject: 0:1,1:0"));
            var corrupt = File.ReadAllText(path);
            Check(corrupt.Contains("m_SerializableObject: 0:1,1:0", StringComparison.Ordinal), "the corruption was planted");

            var findings = Validator.Check(VfxAsset.Load(path)).Where(f => f.Severity == "error").ToList();
            Check(findings.Any(f => f.Where == sol + "." + slot.Path), "validate names the slot: " + string.Join("; ", findings));
            var message = Fails("node", "add", path, "Add");
            Check(message.Contains("can't load", StringComparison.Ordinal), "an unrelated edit refuses to write it: " + message);
            Equal(corrupt, File.ReadAllText(path), "the file after the refused write");
            Sh("set", path, sol + "." + slot.Path, k_Curve);
            Equal(0, Validator.Check(VfxAsset.Load(path)).Count(f => f.Severity == "error"), "errors once the curve is set properly");
        }

        /// <summary>A menu name implies settings; adding another setting keeps them.</summary>
        static void MenuNameSurvivesASetting()
        {
            var path = BuildSystem("menuname");
            var init = Only(VfxAsset.Load(path), n => n.DisplayType == "VFXBasicInitialize", "initialize");
            var log = Sh("block", "add", path, init.ShortId, "Set Lifetime", "Random=Uniform");
            var id = AddedId(log);
            var block = VfxAsset.Load(path).AllNodes().First(n => n.ShortId == id);
            Equal("lifetime", block.Doc.Body.Scalar("attribute"), "the attribute named by 'Set Lifetime'");
            Check(block.AllInputSlots.Any(s => s.Path == "A") && block.AllInputSlots.Any(s => s.Path == "B"), "the uniform-random A/B slots");
            Check(block.AllInputSlots.Where(s => s.Parent == null).All(s => s.TypeName != "Vector3"), "no Vector3 slot from the old position variant");
        }

        /// <summary>A setting that decides the slots picks the variant with those slots, or is refused.</summary>
        static void SettingsSelectTheVariant()
        {
            var path = BuildSystem("variant");
            var init = Only(VfxAsset.Load(path), n => n.DisplayType == "VFXBasicInitialize", "initialize");
            Sh("block", "add", path, init.ShortId, "SetAttribute", "attribute=size");
            var size = Only(VfxAsset.Load(path), n => n.Kind == "block" && n.AllInputSlots.Any(s => s.Path == "_Size"), "a block with a _Size slot");
            Check(Sh("show", path).Contains("attribute=size", StringComparison.Ordinal), "the setting shows");
            var message = Fails("block", "add", path, init.ShortId, "SetAttribute", "attribute=nonsense");
            Check(message.Contains("Values with a variant", StringComparison.Ordinal), "a value with no variant lists the real ones: " + message);
            Check(size != null, "size block exists");
        }

        static void RemovingANodeCleansItsLinks()
        {
            var path = BuildSystem("remove");
            var tint = Only(VfxAsset.Load(path), n => n.DisplayType == "Random", "the random operator");
            Sh("rm", path, tint.ShortId);
            var asset = VfxAsset.Load(path);
            Check(asset.AllNodes().All(n => n.DisplayType != "Random"), "the operator is gone");
            Equal(0, Validator.Check(asset).Count(f => f.Severity == "error"), "no link points at a deleted slot");
        }

        /// <summary>
        /// New nodes get file ids whose last four digits are free, so the id a command reports is
        /// the one a reload assigns, and no existing node's id changes.
        /// </summary>
        static void NewIdsDoNotCollide()
        {
            var path = BuildSystem("ids");

            // With nothing in the way, a dry run shows the id a new Add node would get. Give the
            // tint an id with the same last four digits, so the next add has to avoid it.
            var dryRun = Sh("node", "add", path, "Add", "--dry-run");
            var planned = dryRun.Split('\n').First(l => l.StartsWith("added Add as n", StringComparison.Ordinal)).Trim();
            var tail = long.Parse(planned.Substring("added Add as n".Length), System.Globalization.CultureInfo.InvariantCulture);
            var built = VfxAsset.Load(path);
            var tint = Only(built, n => n.DisplayType == "Random", "the random operator");
            var used = built.Yaml.Documents.Select(d => d.FileId).ToHashSet();
            var clash = used.Max() / 10000 * 10000 - 10000 + tail;
            while (used.Contains(clash))
                clash -= 10000;
            File.WriteAllText(path, File.ReadAllText(path).Replace(
                tint.FileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                clash.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            var before = VfxAsset.Load(path).AllNodes().ToDictionary(n => n.FileId, n => n.ShortId);
            var lines = Enumerable.Range(0, 40).Select(i => $"node add Add --as a{i}").ToArray();
            var log = Sh("apply", path, Script("ids", lines));
            var after = VfxAsset.Load(path);
            foreach (var pair in before)
                Equal(pair.Value, after.Nodes[pair.Key].ShortId, "an existing node's short id");
            foreach (var line in log.Split('\n').Where(l => l.StartsWith("added Add as ", StringComparison.Ordinal)))
            {
                var reported = line.Substring("added Add as ".Length).Trim();
                Check(after.AllNodes().Any(n => n.ShortId == reported && n.DisplayType == "Add"),
                    $"reported id {reported} is the one a reload gives the new node");
            }
            foreach (var node in after.AllNodes().Where(n => n.DisplayType == "Add"))
                Equal(5, node.ShortId.Length, $"{node.ShortId}'s length");
        }

        static void NewNodesLandBesideWhatTheyFeed()
        {
            var asset = VfxAsset.Load(BuildSystem("place"));
            var tint = Only(asset, n => n.DisplayType == "Random", "the random operator");
            var init = Only(asset, n => n.DisplayType == "VFXBasicInitialize", "initialize");
            Check(tint.X < init.X, $"the operator ({tint.X}) is left of the context it feeds ({init.X})");
            Check(init.X - tint.X < 600, $"and close to it, not parked far away ({init.X - tint.X} px)");
            Check(tint.Y >= init.Y, "and level with the block it feeds, not above the context");
        }

        static void NewContextsStackByFlow()
        {
            var asset = VfxAsset.Load(BuildSystem("column"));
            var order = new[] { "VFXBasicSpawner", "VFXBasicInitialize", "VFXBasicUpdate", "VFXPlanarPrimitiveOutput" }
                .Select(t => Only(asset, n => n.DisplayType == t, t)).ToList();
            for (var i = 1; i < order.Count; i++)
            {
                Equal(order[0].X, order[i].X, order[i].DisplayType + " shares the spawner's column");
                Check(order[i].Y > order[i - 1].Y, $"{order[i].DisplayType} sits below {order[i - 1].DisplayType}");
            }
        }

        static void SystemSettingsLandOnTheData()
        {
            var path = BuildSystem("capacity");
            var init = Only(VfxAsset.Load(path), n => n.DisplayType == "VFXBasicInitialize", "initialize");
            Sh("system", "set", path, init.Data.ShortId, "capacity=4096");
            Check(Sh("show", path).Contains("capacity=4096", StringComparison.Ordinal), "the system shows its new capacity");
        }

        static string Infos(string path, string key) => VfxAsset.Load(path).ResourceDoc.Body.Map("m_Infos").Scalar(key);

        static void ResourceSettingsAreStored()
        {
            var path = NewGraph("resource");
            Check(Sh("show", path).Contains("resource initialEvent=OnPlay updateMode=FixedDeltaTime culling=CullDefault instancing=Auto" + Environment.NewLine, StringComparison.Ordinal),
                  "a new graph shows Unity's template defaults");

            Sh("resource", "set", path, "updateMode=DeltaTime", "ignoreTimeScale=true");
            Equal("3", Infos(path, "m_UpdateMode"), "DeltaTime | IgnoreTimeScale");
            Sh("resource", "set", path, "updateMode=ExactFixedTime");
            Equal("6", Infos(path, "m_UpdateMode"), "a new time step keeps IgnoreTimeScale");
            Sh("resource", "set", path, "ignoreTimeScale=false", "updateMode=FixedDeltaTime");
            Equal("0", Infos(path, "m_UpdateMode"), "back to Unity's default");

            Sh("resource", "set", path, "culling=CullNone", "initialEvent=Begin", "instancing=Custom", "instancingCapacity=16");
            Equal("0", Infos(path, "m_CullingFlags"), "culling");
            Equal("Begin", Infos(path, "m_InitialEventName"), "initial event");
            Equal("1", Infos(path, "m_InstancingMode"), "instancing mode");
            Equal("16", Infos(path, "m_InstancingCapacity"), "instancing capacity");
            Sh("resource", "set", path, "instancing=Disabled");
            Equal("-1", Infos(path, "m_InstancingMode"), "instancing off");
            Check(Fails("resource", "set", path, "instancingCapacity=8").Contains("Custom", StringComparison.Ordinal),
                  "a capacity without Custom instancing is refused");

            Sh("resource", "set", path, "prewarmTime=2");
            Equal("40", Infos(path, "m_PreWarmStepCount"), "2s at the default 0.05s step");
            Equal("0.05", Infos(path, "m_PreWarmDeltaTime"), "the step length is kept");
            Sh("resource", "set", path, "prewarmTime=1", "prewarmSteps=10");
            Equal("0.1", Infos(path, "m_PreWarmDeltaTime"), "a given step count derives the step length");
            Sh("resource", "set", path, "prewarmTime=0");
            Equal("0", Infos(path, "m_PreWarmStepCount"), "no prewarm");
            Check(Fails("resource", "set", path, "prewarmDeltaTime=0.0001").Contains("1/800", StringComparison.Ordinal),
                  "a step under the inspector's minimum is refused");

            var before = File.ReadAllText(path);
            Check(Fails("resource", "set", path, "updateMode=DeltaTime", "culling=Sometimes").Contains("CullDefault", StringComparison.Ordinal),
                  "an unknown value lists the choices");
            Check(Fails("resource", "set", path, "updatemode=Sometimes").Contains("FixedDeltaTime", StringComparison.Ordinal),
                  "an unknown update mode lists the choices");
            Check(Fails("resource", "set", path, "speed=2").Contains("updateMode", StringComparison.Ordinal),
                  "an unknown key lists the keys");
            Equal(before, File.ReadAllText(path), "a refused command writes nothing");

            Sh("apply", path, Script("resource", "resource set updateMode=DeltaTime"));
            Equal("1", Infos(path, "m_UpdateMode"), "resource set runs inside a script");
        }

        static void AutomaticBoundsKeepCullingOff()
        {
            var path = BuildSystem("autobounds");
            var init = Only(VfxAsset.Load(path), n => n.DisplayType == "VFXBasicInitialize", "initialize");
            Sh("system", "set", path, init.Data.ShortId, "boundsMode=Automatic");
            Check(Fails("resource", "set", path, "culling=CullDefault").Contains("automatically", StringComparison.Ordinal),
                  "culling is refused while a system computes its bounds");
            Sh("resource", "set", path, "culling=CullNone");
        }
    }
}
