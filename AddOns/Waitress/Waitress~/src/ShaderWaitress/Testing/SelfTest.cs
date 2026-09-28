using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ShaderWaitress.Catalog;
using ShaderWaitress.Cli;
using ShaderWaitress.Edit;
using ShaderWaitress.Layout;
using ShaderWaitress.Model;
using ShaderWaitress.Query;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Testing
{
    /// <summary>
    /// In-process suite covering the parser, the selector language, every mutation, and the
    /// layout invariants. Runs with no external dependencies so it works anywhere the binary
    /// does.
    /// </summary>
    public static class SelfTest
    {
        static int s_Passed;
        static readonly List<string> s_Failures = new List<string>();
        static TextWriter s_Out;
        static string s_Scratch;

        public static int Run(Args args, TextWriter output)
        {
            s_Out = output;
            s_Passed = 0;
            s_Failures.Clear();
            var verbose = args.Flag("verbose") || args.Flag("v");
            var filter = args.Get("filter");
            s_Scratch = Path.Combine(Path.GetTempPath(), "shaderwaitress-selftest-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(s_Scratch);
            args.RejectUnknown();

            try
            {
                foreach (var (name, test) in Tests())
                {
                    if (filter != null && name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    try
                    {
                        test();
                        s_Passed++;
                        if (verbose)
                            output.WriteLine($"pass  {name}");
                    }
                    catch (Exception e)
                    {
                        s_Failures.Add($"{name}: {e.Message}");
                        output.WriteLine($"FAIL  {name}: {e.Message}");
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(s_Scratch, true);
                }
                catch (Exception)
                {
                    // Leaving a temp directory behind is not worth failing the run over.
                }
            }

            output.WriteLine($"selftest: {s_Passed} passed, {s_Failures.Count} failed");
            return s_Failures.Count == 0 ? 0 : 1;
        }

        static IEnumerable<(string, Action)> Tests()
        {
            yield return ("multijson/roundtrip", MultiJsonRoundTrip);
            yield return ("multijson/order", MultiJsonOrder);
            yield return ("multijson/crlf", MultiJsonCrLf);
            yield return ("writer/floats", WriterKeepsFloatText);
            yield return ("writer/empty-collections", WriterEmptyCollections);
            yield return ("shortids/stable", ShortIdsAreStable);
            yield return ("shortids/collision", ShortIdsExtendOnCollision);
            yield return ("catalog/present", CatalogPresent);
            yield return ("catalog/ports", CatalogPortsDerived);
            yield return ("new/creates", NewCreatesLoadableGraph);
            yield return ("edit/add-wire", AddAndWire);
            yield return ("edit/one-wire-per-input", OneWirePerInput);
            yield return ("edit/set-value", SetPortValue);
            yield return ("edit/remove-reconnect", RemoveWithReconnect);
            yield return ("edit/replace", ReplaceNode);
            yield return ("edit/property", PropertyLifecycle);
            yield return ("edit/group", GroupLifecycle);
            yield return ("edit/target", TargetSettingsRoundTrip);
            yield return ("select/pipeline", SelectorPipeline);
            yield return ("select/setops", SelectorSetOps);
            yield return ("select/tags", SelectorTags);
            yield return ("select/traversal-ports", SelectorPortTraversal);
            yield return ("script/labels", ScriptLabels);
            yield return ("script/atomic", ScriptFailureLeavesFileUntouched);
            yield return ("layout/invariants", LayoutInvariants);
            yield return ("layout/groups", LayoutKeepsGroupsSeparate);
            yield return ("layout/disconnected", LayoutHandlesDisconnected);
            yield return ("layout/incremental", LayoutLeavesPlacedNodesAlone);
            yield return ("validate/clean", ValidateCleanGraph);
            yield return ("validate/catches", ValidateCatchesBrokenEdge);
            yield return ("dense/parses-back", DenseTextMentionsEveryEdge);
            yield return ("settings/read-write", NodeSettingsAreReadableAndSettable);
            yield return ("settings/reject-bad-value", NodeSettingRejectsUnknownValue);
            yield return ("settings/unknown-type-fallback", NodeSettingsSurviveWithoutCatalog);
            yield return ("values/type-default-visible", TypeDefaultLiteralIsVisible);
            yield return ("values/dynamic-slot", DynamicSlotValueRoundTrips);
            yield return ("keyword/create-and-bind", KeywordLifecycle);
            yield return ("subgraph/create", SubGraphCreation);
            yield return ("cli/format-validated", UnknownFormatIsRejected);
            yield return ("edit/insert", InsertSplicesIntoExistingWires);
            yield return ("edit/insert-selector", InsertPatchesEveryMatch);
            yield return ("edit/rewire-after-rebuild", RewiringSurvivesARebuild);
            yield return ("script/all-verbs", ScriptAcceptsEveryEditingVerb);
            yield return ("subgraph/output-port", SubGraphOutputIsNameable);
            yield return ("target/renderface", RenderFaceIsNotCalledCull);
            yield return ("prop/declaration", DeclarationIsVisibleAndSettable);
            yield return ("prop/declaration-allowed", DeclarationChecksWhatTheTypeAccepts);
            yield return ("port/add-remove", PortsCanBeAuthored);
            yield return ("script/continuation", ScriptJoinsContinuedLines);
            yield return ("edit/insert-binds", InsertBindsBeforeResolvingPorts);
            yield return ("keyword/allow-override", KeywordDefinitionOverrideIsVisibleAndSettable);
            yield return ("layout/group-gaps", LayoutClosesGapsInsideGroups);
            yield return ("layout/group-join", IncrementalPlacementStaysInsideItsGroup);
            yield return ("multijson/legacy-format", LegacyFormatIsNamed);
            yield return ("layout/clearance", LayoutLeavesDaylightBetweenNodes);
            yield return ("layout/block-order", ProducersFollowTheBlockOrder);
            yield return ("layout/crossings-at-one-node", CrossingsAtOneNodeAreCounted);
            yield return ("layout/sources-sit-close", SourcesSitBesideWhatTheyFeed);
            yield return ("layout/node-geometry", NodeGeometryMatchesTheEditor);
            yield return ("layout/group-cycle", GroupCycleDoesNotWreckRanking);
            yield return ("layout/open-lanes", LaneOpeningClearsAWireInPlace);
            yield return ("layout/keyword-node", KeywordNodesAreNodesNotTokens);
            yield return ("layout/value-editors", ValueEditorsDoNotCoverNeighbours);
            yield return ("layout/stacks-align", ContextStacksShareAColumn);
            yield return ("layout/no-guessed-sizes", LayoutWritesPositionsNotSizes);
            yield return ("node/preview-toggle", PreviewCanBeCollapsed);
            yield return ("prop/promote", PromotionReachesTheParentShader);
            yield return ("block/add-rm", BlocksCanBeAddedAndRemoved);
            yield return ("block/follows-target", BlocksFollowTheTargetSettings);
            yield return ("prop/texture-default", TextureDefaultIsVisibleAndSettable);
            yield return ("ids/new-prefixes-are-free", NewIdsAvoidTakenPrefixes);
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

        static string NewGraph(string name, string starter = "universal/unlit")
        {
            var path = Path.Combine(s_Scratch, name + ".shadergraph");
            var code = CommandRouter.Run(new[] { "new", path, "--target", starter, "--force" }, TextWriter.Null, TextWriter.Null);
            Check(code == 0, "new returned " + code);
            return path;
        }

        static ShaderGraphDocument Load(string path) => ShaderGraphDocument.Load(path);

        static string Sh(params string[] argv)
        {
            var sb = new StringWriter();
            var code = CommandRouter.Run(argv, sb, sb);
            if (code != 0)
                throw new Exception($"'{string.Join(" ", argv)}' returned {code}: {sb}");
            return sb.ToString();
        }

        // ---- container ----

        const string k_Sample =
            "{\n    \"m_SGVersion\": 3,\n    \"m_Type\": \"UnityEditor.ShaderGraph.GraphData\",\n" +
            "    \"m_ObjectId\": \"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\",\n    \"m_Nodes\": [],\n" +
            "    \"m_Edges\": [],\n    \"m_Value\": 33.99993896484375\n}\n\n" +
            "{\n    \"m_SGVersion\": 0,\n    \"m_Type\": \"UnityEditor.ShaderGraph.Vector1MaterialSlot\",\n" +
            "    \"m_ObjectId\": \"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb\",\n    \"m_Labels\": [],\n" +
            "    \"m_Empty\": {}\n}\n\n";

        static void MultiJsonRoundTrip()
        {
            var doc = MultiJsonDocument.Parse(k_Sample);
            Equal(2, doc.Entries.Count, "entry count");
            Equal(k_Sample, doc.Serialize(), "round trip");
        }

        static void MultiJsonOrder()
        {
            var reversed =
                k_Sample.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)[1] + "\n\n" +
                k_Sample.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)[0] + "\n\n";
            var doc = MultiJsonDocument.Parse(reversed);
            // Whatever came first is the root; the rest sort by object id.
            Check(doc.Serialize().StartsWith("{\n    \"m_SGVersion\": 0", StringComparison.Ordinal),
                "the first object stays first");
        }

        static void MultiJsonCrLf()
        {
            var crlf = k_Sample.Replace("\n", "\r\n");
            var doc = MultiJsonDocument.Parse(crlf);
            Check(doc.UseCrLf, "CRLF detected");
            Equal(crlf, doc.Serialize(), "CRLF round trip");
        }

        static void WriterKeepsFloatText()
        {
            var doc = MultiJsonDocument.Parse(k_Sample);
            doc.Root.Edit();
            Check(doc.Serialize().Contains("33.99993896484375", StringComparison.Ordinal),
                "float text survived a forced rewrite");
        }

        static void WriterEmptyCollections()
        {
            var doc = MultiJsonDocument.Parse(k_Sample);
            foreach (var entry in doc.Entries)
                entry.Edit();
            var text = doc.Serialize();
            Check(text.Contains("\"m_Labels\": []", StringComparison.Ordinal), "empty array stays on one line");
            Check(text.Contains("\"m_Empty\": {}", StringComparison.Ordinal), "empty object stays on one line");
        }

        static void ShortIdsAreStable()
        {
            var path = NewGraph("stable");
            var before = Load(path).Nodes.ToDictionary(n => n.ObjectId, n => n.ShortId);
            Sh("node", "add", path, "Multiply");
            var after = Load(path).Nodes.ToDictionary(n => n.ObjectId, n => n.ShortId);
            foreach (var pair in before)
            {
                Check(after.TryGetValue(pair.Key, out var now) && now == pair.Value,
                    $"short id for {pair.Key} changed from {pair.Value}");
            }
        }

        static void ShortIdsExtendOnCollision()
        {
            var table = new ShortIdTable();
            var a = table.Assign('n', "abcd11111111111111111111111111111");
            var b = table.Assign('n', "abcd22222222222222222222222222222");
            Check(a != b, "colliding ids were separated");
            Check(table.ObjectFor(b) == "abcd22222222222222222222222222222", "lookup works after extension");
        }

        static void CatalogPresent()
        {
            var catalog = NodeCatalog.Shared;
            Check(!catalog.IsEmpty, "catalog loaded");
            Check(catalog.Nodes.Count > 100, $"catalog has {catalog.Nodes.Count} nodes");
            Check(catalog.Blocks.Count > 5, "catalog has block descriptors");
            Check(catalog.Starters.Count > 0, "catalog has starter graphs");
        }

        static void CatalogPortsDerived()
        {
            var multiply = NodeCatalog.Shared.Require("Multiply");
            Equal(3, multiply.Ports.Count, "Multiply port count");
            Check(multiply.Ports.Any(p => p.Name == "A" && p.IsInput), "Multiply has input A");
            Check(multiply.Ports.Any(p => p.Name == "Out" && !p.IsInput), "Multiply has output Out");
        }

        static void NewCreatesLoadableGraph()
        {
            var path = NewGraph("fresh");
            var doc = Load(path);
            Check(doc.Nodes.Any(n => n.IsBlock), "starter has blocks");
            Check(doc.Targets.Count > 0, "starter has a target");
            Equal(0, Validator.Run(doc).Count, "starter validates clean");

            var other = NewGraph("fresh2");
            var ids = Load(path).Raw.Entries.Select(e => e.ObjectId).ToHashSet();
            Check(!Load(other).Raw.Entries.Any(e => ids.Contains(e.ObjectId)),
                "two graphs from one starter do not share object ids");
        }

        static void AddAndWire()
        {
            var path = NewGraph("wire");
            var id = Sh("node", "add", path, "Multiply").Split('\n')[0].Trim();
            Sh("wire", path, id, "BaseColor");
            var doc = Load(path);
            var node = doc.ResolveNode(id);
            Check(doc.EdgesOut(node).Count == 1, "wire was created");
            Check(doc.EdgesOut(node)[0].To.BlockDescriptor.EndsWith("BaseColor", StringComparison.Ordinal),
                "wire lands on Base Color");
        }

        static void OneWirePerInput()
        {
            var path = NewGraph("single");
            var a = Sh("node", "add", path, "Multiply").Split('\n')[0].Trim();
            var b = Sh("node", "add", path, "Add").Split('\n')[0].Trim();
            Sh("wire", path, a, "BaseColor");
            Sh("wire", path, b, "BaseColor");
            var doc = Load(path);
            var block = doc.Nodes.First(n => n.IsBlock && n.BlockDescriptor.EndsWith("BaseColor", StringComparison.Ordinal));
            Equal(1, doc.EdgesIn(block).Count, "second wire replaced the first");
        }

        static void SetPortValue()
        {
            var path = NewGraph("value");
            var id = Sh("node", "add", path, "Multiply", "--set", "B=3").Split('\n')[0].Trim();
            var doc = Load(path);
            var port = doc.ResolveNode(id).RequirePort("B", input: true);
            Check(port.FormattedValue != null && port.FormattedValue.Contains('3'), $"B is {port.FormattedValue}");
        }

        static void RemoveWithReconnect()
        {
            var path = NewGraph("reconnect");
            var time = Sh("node", "add", path, "Time").Split('\n')[0].Trim();
            var mul = Sh("node", "add", path, "Multiply", "--wire", "A=" + time + ".Time").Split('\n')[0].Trim();
            Sh("wire", path, mul, "BaseColor");
            Sh("node", "rm", path, "id:" + mul, "--reconnect");

            var doc = Load(path);
            var block = doc.Nodes.First(n => n.IsBlock && n.BlockDescriptor.EndsWith("BaseColor", StringComparison.Ordinal));
            Equal(1, doc.EdgesIn(block).Count, "Base Color is still fed");
            Check(doc.EdgesIn(block)[0].From.TypeLabel == "Time", "the upstream node was spliced through");
        }

        static void ReplaceNode()
        {
            var path = NewGraph("replace");
            var time = Sh("node", "add", path, "Time").Split('\n')[0].Trim();
            var mul = Sh("node", "add", path, "Multiply", "--wire", "A=" + time + ".Time").Split('\n')[0].Trim();
            Sh("wire", path, mul, "BaseColor");
            Sh("node", "replace", path, "id:" + mul, "--with", "Add");

            var doc = Load(path);
            Check(doc.Nodes.Any(n => n.TypeLabel == "Add"), "the replacement exists");
            Check(!doc.Nodes.Any(n => n.TypeLabel == "Multiply"), "the original is gone");
            var add = doc.Nodes.First(n => n.TypeLabel == "Add");
            Equal(1, doc.EdgesIn(add).Count, "the input wire survived");
            Equal(1, doc.EdgesOut(add).Count, "the output wire survived");
        }

        static void PropertyLifecycle()
        {
            var path = NewGraph("prop");
            var pid = Sh("prop", "add", path, "Float", "--name", "Speed", "--ref", "_Speed", "--value", "2").Split('\n')[0].Trim();
            var doc = Load(path);
            var property = doc.ResolveProperty(pid);
            Equal("Speed", property.Name, "property name");
            Equal("_Speed", property.ReferenceName, "reference name");
            Check(property.FormattedValue == "2", $"value is {property.FormattedValue}");

            var nid = Sh("node", "add", path, "Property", "--property", pid).Split('\n')[0].Trim();
            doc = Load(path);
            var node = doc.ResolveNode(nid);
            Check(node.IsProperty && node.PropertyId == property.ObjectId, "node is bound to the property");
            Equal(1, node.Outputs.Count(), "property node has one output");

            Sh("prop", "rm", path, pid, "--with-nodes");
            doc = Load(path);
            Equal(0, doc.Properties.Count, "property removed");
            Check(!doc.Nodes.Any(n => n.IsProperty), "its node went with it");
            Equal(0, Validator.Run(doc).Count(p => p.StartsWith("orphan", StringComparison.Ordinal)), "no orphan objects left");
        }

        static void GroupLifecycle()
        {
            var path = NewGraph("group");
            var a = Sh("node", "add", path, "Multiply").Split('\n')[0].Trim();
            var b = Sh("node", "add", path, "Add").Split('\n')[0].Trim();
            var gid = Sh("group", "new", path, "Math", "id:" + a + "," + b).Split('\n')[0].Trim();

            var doc = Load(path);
            var group = doc.ResolveGroup(gid);
            Equal(2, doc.Nodes.Count(n => n.GroupId == group.ObjectId), "both nodes joined");

            Sh("group", "rename", path, gid, "Arithmetic");
            Equal("Arithmetic", Load(path).ResolveGroup(gid).Title, "group renamed");

            Sh("group", "rm", path, gid);
            doc = Load(path);
            Equal(0, doc.Groups.Count, "group removed");
            Equal(2, doc.Nodes.Count(n => n.TypeLabel is "Multiply" or "Add"), "members survived");
        }

        static void TargetSettingsRoundTrip()
        {
            var path = NewGraph("target");
            Sh("target", "set", path, "surface=transparent", "blend=additive", "cull=off", "zwrite=on");
            var doc = Load(path);
            var described = string.Join(" ", TargetSettings.Describe(doc.Targets[0]));
            Check(described.Contains("surface=transparent"), described);
            Check(described.Contains("blend=additive"), described);
            Check(described.Contains("zwrite=on"), described);
        }

        static string BuildSampleGraph(string name)
        {
            var path = NewGraph(name);
            var script = string.Join("\n",
                "prop add Float --name \"Speed\" --ref _Speed --value 1 --as speed",
                "node add Property --property $speed --as speedNode",
                "node add Time --as time",
                "node add Multiply --as scroll --wire A=$speedNode --wire B=$time.Time",
                "node add TilingAndOffset --as uv --wire Offset=$scroll",
                "node add SampleTexture2D --as sample --wire UV=$uv",
                "wire $sample.RGBA SurfaceDescription.BaseColor",
                "node add Add --as orphanAdd");
            var scriptPath = Path.Combine(s_Scratch, name + ".sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            return path;
        }

        static void SelectorPipeline()
        {
            var path = BuildSampleGraph("selector");
            var doc = Load(path);
            var upstreamOfBase = Selector.Evaluate(doc, "block:BaseColor | in*");
            Check(upstreamOfBase.Any(n => n.TypeLabel == "SampleTexture2D"), "reached the sampler");
            Check(upstreamOfBase.Any(n => n.TypeLabel == "Time"), "reached Time transitively");
            Check(!upstreamOfBase.Any(n => n.TypeLabel == "Add"), "did not reach the disconnected Add");

            var direct = Selector.Evaluate(doc, "type:Multiply | in | type:Property");
            Equal(1, direct.Count, "one property feeds the Multiply");
        }

        static void SelectorSetOps()
        {
            var path = BuildSampleGraph("setops");
            var doc = Load(path);
            var union = Selector.Evaluate(doc, "type:Time + type:Add");
            Equal(2, union.Count, "union");
            var difference = Selector.Evaluate(doc, "all - is:block");
            Check(difference.All(n => !n.IsBlock), "difference removed blocks");
            var intersection = Selector.Evaluate(doc, "all & type:Time");
            Equal(1, intersection.Count, "intersection");
        }

        static void SelectorTags()
        {
            var path = BuildSampleGraph("tags");
            var doc = Load(path);
            var orphans = Selector.Evaluate(doc, "is:orphan");
            Check(orphans.Any(n => n.TypeLabel == "Add"), "the disconnected Add is an orphan");
            var unreachable = Selector.Evaluate(doc, "is:unreachable | type:Add");
            Equal(1, unreachable.Count, "the Add does not feed a block");
        }

        static void SelectorPortTraversal()
        {
            var path = BuildSampleGraph("ports");
            var doc = Load(path);
            var throughA = Selector.Evaluate(doc, "type:Multiply | in:A");
            Equal(1, throughA.Count, "one node feeds A");
            Check(throughA[0].IsProperty, "and it is the property node");
        }

        static void ScriptLabels()
        {
            var path = BuildSampleGraph("labels");
            var doc = Load(path);
            Check(doc.Nodes.Any(n => n.TypeLabel == "TilingAndOffset"), "the labelled chain was built");
            var sampler = doc.Nodes.First(n => n.TypeLabel == "SampleTexture2D");
            Equal(1, doc.EdgesIn(sampler).Count(e => e.ToPort != null && e.ToPort.Name == "UV"), "UV was wired by label");
        }

        static void ScriptFailureLeavesFileUntouched()
        {
            var path = NewGraph("atomic");
            var before = File.ReadAllText(path);
            var scriptPath = Path.Combine(s_Scratch, "bad.sw");
            File.WriteAllText(scriptPath, "node add Multiply\nnode add ThisTypeDoesNotExist\n");
            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "apply", path, scriptPath }, TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException)
            {
                failed = true;
            }
            Check(failed, "a bad script fails");
            Equal(before, File.ReadAllText(path), "the file was not written");
        }

        static void LayoutInvariants()
        {
            var path = BuildSampleGraph("layout");
            Sh("layout", path);
            var doc = Load(path);
            var metrics = LayoutMetrics.Measure(doc);
            Equal(0, metrics.NodeOverlaps, "no node overlaps");
            Equal(0, metrics.BackwardEdges, "no wire travels right to left");
        }

        static void LayoutKeepsGroupsSeparate()
        {
            var path = BuildSampleGraph("groups");
            var doc = Load(path);
            var time = doc.Nodes.First(n => n.TypeLabel == "Time");
            var multiply = doc.Nodes.First(n => n.TypeLabel == "Multiply");
            var sampler = doc.Nodes.First(n => n.TypeLabel == "SampleTexture2D");
            Sh("group", "new", path, "Scroll", "id:" + time.ShortId + "," + multiply.ShortId);
            Sh("group", "new", path, "Sampling", "id:" + sampler.ShortId);
            Sh("layout", path);

            var metrics = LayoutMetrics.Measure(Load(path));
            Equal(0, metrics.GroupOverlaps, "group boxes are disjoint");
            Equal(0, metrics.NodesOutsideGroup, "no foreign node sits inside a group box");
        }

        static void LayoutHandlesDisconnected()
        {
            var path = NewGraph("islands");
            for (var i = 0; i < 5; i++)
                Sh("node", "add", path, "Multiply");
            Sh("layout", path);
            var metrics = LayoutMetrics.Measure(Load(path));
            Equal(0, metrics.NodeOverlaps, "disconnected nodes do not overlap");
        }

        /// <summary>
        /// Adding to a graph that is already arranged must not move anything that was arranged.
        /// </summary>
        static void LayoutLeavesPlacedNodesAlone()
        {
            var path = BuildSampleGraph("incremental");
            Sh("layout", path, "--force");
            var before = Load(path).Nodes
                .Where(n => !n.IsBlock)
                .ToDictionary(n => n.ObjectId, n => n.Position);
            Check(Load(path).Nodes.Where(n => !n.IsBlock).All(n => n.HasStoredPosition),
                "the starting layout placed every node");

            var added = Sh("node", "add", path, "Multiply").Split('\n')[0].Trim();
            var after = Load(path);
            foreach (var pair in before)
            {
                var node = after.NodeById(pair.Key);
                Check(node != null, "node survived");
                var moved = Math.Abs(node.Position.X - pair.Value.X) + Math.Abs(node.Position.Y - pair.Value.Y);
                Check(moved < 1.5, $"{node.ShortId} moved by {moved:0.#}");
            }
            var newNode = after.ResolveNode(added);
            // A size is the Editor's to measure, so the layout writes a position and nothing else.
            Check(newNode.HasStoredPosition, "the new node was placed");
            Equal(0.0, newNode.Position.Width, "and left unsized for the Editor to measure");
            var metrics = LayoutMetrics.Measure(after);
            Equal(0, metrics.NodeOverlaps, "no overlap after the insertion");
            Equal(0, metrics.Crowded, "and nothing packed tight enough to look like one");
        }

        static void ValidateCleanGraph()
        {
            var path = BuildSampleGraph("valid");
            var problems = Validator.Run(Load(path));
            Check(problems.Count == 0, "clean graph reports: " + string.Join("; ", problems));
        }

        static void ValidateCatchesBrokenEdge()
        {
            var path = BuildSampleGraph("broken");
            var text = File.ReadAllText(path);
            // Point an edge at a slot id that does not exist on the node.
            text = text.Replace("\"m_SlotId\": 0", "\"m_SlotId\": 991", StringComparison.Ordinal);
            File.WriteAllText(path, text);
            var problems = Validator.Run(Load(path));
            Check(problems.Any(p => p.StartsWith("edge:", StringComparison.Ordinal)), "a dangling slot reference is reported");
        }

        static void DenseTextMentionsEveryEdge()
        {
            var path = BuildSampleGraph("dense");
            var doc = Load(path);
            var text = DenseText.Render(doc);
            foreach (var node in doc.Nodes)
                Check(text.Contains(node.ShortId, StringComparison.Ordinal), $"{node.ShortId} appears");
            var arrows = text.Split('\n').Sum(line => CountOccurrences(line, "<-"));
            Equal(doc.Edges.Count, arrows, "one arrow per edge");
        }

        /// <summary>
        /// Scene Depth's sampling mode is a dropdown, not a port. A graph that needs Eye but
        /// silently gets Linear01 compiles and validates, so the tool has to surface it.
        /// </summary>
        static void NodeSettingsAreReadableAndSettable()
        {
            var path = NewGraph("settings");
            var id = Sh("node", "add", path, "SceneDepth").Split('\n')[0].Trim();

            var doc = Load(path);
            var setting = NodeSettings.Require(doc.ResolveNode(id), "DepthSamplingMode");
            Equal("Linear01", setting.Display, "the default is visible before anyone changes it");
            Check(DenseText.Render(doc).Contains("DepthSamplingMode:Linear01", StringComparison.Ordinal),
                "the dense form shows it too");

            Sh("node", "set", path, "id:" + id, "--set", "DepthSamplingMode=Eye");
            Equal("Eye", NodeSettings.Require(Load(path).ResolveNode(id), "DepthSamplingMode").Display, "after --set");

            Sh("node", "set", path, "id:" + id, "--setting", "DepthSamplingMode=Raw");
            Equal("Raw", NodeSettings.Require(Load(path).ResolveNode(id), "DepthSamplingMode").Display, "after --setting");
        }

        static void NodeSettingRejectsUnknownValue()
        {
            var path = NewGraph("badsetting");
            var id = Sh("node", "add", path, "ScreenPosition").Split('\n')[0].Trim();
            var before = File.ReadAllText(path);
            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "node", "set", path, "id:" + id, "--set", "ScreenSpaceType=Sideways" },
                    TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException e)
            {
                failed = true;
                Check(e.Message.Contains("Raw", StringComparison.Ordinal), "the message lists the real options: " + e.Message);
            }
            Check(failed, "an invalid setting value is rejected");
            Equal(before, File.ReadAllText(path), "and the file is untouched");
        }

        /// <summary>
        /// The catalog cannot know about every project's custom nodes, so settings are also
        /// discovered from the serialized object itself.
        /// </summary>
        static void NodeSettingsSurviveWithoutCatalog()
        {
            var path = NewGraph("nocatalog");
            var id = Sh("node", "add", path, "SceneDepth").Split('\n')[0].Trim();
            var doc = ShaderGraphDocument.Load(path, new NodeCatalog());
            var setting = NodeSettings.Require(doc.ResolveNode(id), "DepthSamplingMode");
            Check(setting.Display != null, "the value is still readable with an empty catalog");
        }

        /// <summary>
        /// Multiply's B defaults to 2, so a Multiply nobody touched still doubles. Hiding a
        /// literal because it matches the type default hides that.
        /// </summary>
        static void TypeDefaultLiteralIsVisible()
        {
            var path = NewGraph("defaults");
            var id = Sh("node", "add", path, "Multiply").Split('\n')[0].Trim();
            var doc = Load(path);
            var line = DenseText.NodeLine(doc, doc.ResolveNode(id), new DenseTextOptions());
            // Multiply constructs A as zero and B as 2, so a fresh Multiply doubles whatever
            // reaches A. Both numbers have to be on the line for that to be readable.
            Check(line.Contains("A=0", StringComparison.Ordinal) && line.Contains("B=2", StringComparison.Ordinal),
                "both inputs show their value: " + line);
        }

        /// <summary>A dynamic slot stores a Matrix4x4 but reads as row 0.</summary>
        static void DynamicSlotValueRoundTrips()
        {
            var path = NewGraph("dynamic");
            var id = Sh("node", "add", path, "Multiply", "--set", "B=3").Split('\n')[0].Trim();
            var port = Load(path).ResolveNode(id).RequirePort("B", input: true);
            Equal("3", port.FormattedValue, "a scalar written to a dynamic slot reads back as a scalar");

            Sh("node", "set", path, "id:" + id, "--set", "A=1,2,3,4");
            Equal("(1, 2, 3, 4)", Load(path).ResolveNode(id).RequirePort("A", input: true).FormattedValue, "a vector too");
        }

        static void KeywordLifecycle()
        {
            var path = NewGraph("keywords");
            Sh("keyword", "add", path, "boolean", "--name", "Use Detail", "--ref", "_USE_DETAIL", "--definition", "multicompile");
            var doc = Load(path);
            Equal(1, doc.Keywords.Count, "keyword created");
            var keyword = doc.Keywords[0];
            Equal("_USE_DETAIL", keyword.ReferenceName, "reference name");
            Equal(1, keyword.KeywordDefinition, "multi_compile");

            var nodeId = Sh("node", "add", path, "Keyword", "--keyword", "Use Detail").Split('\n')[0].Trim();
            doc = Load(path);
            var node = doc.ResolveNode(nodeId);
            Check(node.IsKeyword && node.KeywordId == doc.Keywords[0].ObjectId, "the node is bound");
            Check(node.Inputs.Any(p => p.Name == "On") && node.Inputs.Any(p => p.Name == "Off"), "boolean gets On and Off");
            Equal(1, node.Outputs.Count(), "and one output");

            Sh("keyword", "add", path, "enum", "--name", "Quality", "--entries", "Low,Medium,High", "--default", "Medium");
            var enumKeyword = Load(path).ResolveKeyword("Quality");
            Equal(3, enumKeyword.Entries.Count, "enum entries");
            Equal(1, enumKeyword.Value, "the named default resolved to its index");

            var enumNodeId = Sh("node", "add", path, "Keyword", "--keyword", "Quality").Split('\n')[0].Trim();
            var enumNode = Load(path).ResolveNode(enumNodeId);
            Equal(3, enumNode.Inputs.Count(), "one input per entry");

            Sh("keyword", "rm", path, "Quality", "--with-nodes");
            doc = Load(path);
            Equal(1, doc.Keywords.Count, "keyword removed");
            Equal(0, Validator.Run(doc).Count(p => p.StartsWith("orphan", StringComparison.Ordinal)), "no orphan objects");
        }

        static void SubGraphCreation()
        {
            var path = Path.Combine(s_Scratch, "made.shadersubgraph");
            Sh("new", path, "--force");
            var doc = Load(path);
            Check(doc.IsSubGraph, "reported as a subgraph");
            Check(doc.Nodes.Any(n => n.TypeName.EndsWith("SubGraphOutputNode", StringComparison.Ordinal)), "has an output node");
            Equal(0, doc.Targets.Count, "and no render target");
            Equal(0, Validator.Run(doc).Count, "validates clean");

            // The extension and the kind have to agree, or Unity imports nothing usable.
            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "new", Path.Combine(s_Scratch, "wrong.shadergraph"), "--target", "subgraph", "--force" },
                    TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException)
            {
                failed = true;
            }
            Check(failed, "a subgraph cannot be written to a .shadergraph file");
        }

        static void UnknownFormatIsRejected()
        {
            var path = BuildSampleGraph("format");
            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "q", path, "all", "--format", "yaml" }, TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException e)
            {
                failed = true;
                Check(e.Message.Contains("text", StringComparison.Ordinal), "the message lists valid formats");
            }
            Check(failed, "an unknown --format is rejected rather than ignored");

            // --starters is a switch, so it must work without a value.
            var sb = new StringWriter();
            Equal(0, CommandRouter.Run(new[] { "nodes", "--starters" }, sb, sb), "nodes --starters runs");
            Check(sb.ToString().Contains("subgraph", StringComparison.Ordinal), "and lists the subgraph starter");
        }

        /// <summary>
        /// The inverse of `node rm --reconnect`: every consumer moves onto the new node, and
        /// the intercepted output becomes its input. Getting this wrong leaves the old wires in
        /// place and every consumer with two inputs.
        /// </summary>
        static void InsertSplicesIntoExistingWires()
        {
            var path = NewGraph("insert");
            var source = Sh("node", "add", path, "VertexColor").Split('\n')[0].Trim();
            Sh("wire", path, source, "BaseColor");
            Sh("node", "add", path, "Split", "--wire", "In=" + source);

            Sh("node", "insert", path, "Multiply", "--after", "id:" + source, "--set", "B=2");

            var doc = Load(path);
            var inserted = doc.Nodes.First(n => n.TypeLabel == "Multiply");
            var original = doc.ResolveNode(source);

            Equal(1, doc.EdgesOut(original).Count, "the intercepted node now feeds only the new one");
            Check(doc.EdgesOut(original)[0].To.Is(inserted), "and it feeds the new node");
            Equal(2, doc.EdgesOut(inserted).Count, "both consumers moved across");
            foreach (var node in doc.Nodes)
            {
                foreach (var port in node.Inputs)
                {
                    Check(doc.EdgesIn(node).Count(e => e.ToSlot == port.Id) <= 1,
                        $"{node.ShortId}.{port.Name} kept a single incoming wire");
                }
            }
            Equal(0, Validator.Run(doc).Count, "the result validates clean");
        }

        /// <summary>One script has to be able to patch every graph that matches a selector.</summary>
        static void InsertPatchesEveryMatch()
        {
            var path = NewGraph("insert-many");
            for (var i = 0; i < 3; i++)
            {
                var id = Sh("node", "add", path, "VertexColor").Split('\n')[0].Trim();
                Sh("node", "add", path, "Split", "--wire", "In=" + id);
            }
            Sh("node", "insert", path, "Multiply", "--after", "type:VertexColor", "--set", "B=2");

            var doc = Load(path);
            Equal(3, doc.Nodes.Count(n => n.TypeLabel == "Multiply"), "one per match");
            foreach (var split in doc.Nodes.Where(n => n.TypeLabel == "Split"))
            {
                Equal(1, doc.EdgesIn(split).Count, "each Split still has one input");
                Check(doc.EdgesIn(split)[0].From.TypeLabel == "Multiply", "and it comes from a Multiply now");
            }
        }

        /// <summary>
        /// Rebuilding the semantic view replaces every SgNode, so an edge predicate compared by
        /// reference stops matching and a replaced wire is left behind as a duplicate.
        /// </summary>
        static void RewiringSurvivesARebuild()
        {
            var path = NewGraph("rewire");
            var first = Sh("node", "add", path, "VertexColor").Split('\n')[0].Trim();
            Sh("wire", path, first, "BaseColor");

            // Adding a node rebuilds the view; the wire below must still replace, not duplicate.
            var second = Sh("node", "add", path, "Color").Split('\n')[0].Trim();
            Sh("wire", path, second, "BaseColor");

            var doc = Load(path);
            var block = doc.Nodes.First(n => n.IsBlock && n.BlockDescriptor.EndsWith("BaseColor", StringComparison.Ordinal));
            Equal(1, doc.EdgesIn(block).Count, "Base Color has exactly one input");
            Check(doc.EdgesIn(block)[0].From.TypeLabel == "Color", "and it is the later wire");
        }

        static void ScriptAcceptsEveryEditingVerb()
        {
            var path = NewGraph("verbs");
            var script = string.Join("\n",
                "target set surface=transparent blend=additive renderface=both",
                "settings set precision=half",
                "keyword add boolean --name \"Soft\" --ref _SOFT --as soft",
                "prop add Float --name \"Amount\" --ref _Amount --value 1 --as amount",
                "node add Property --property $amount --as amountNode",
                "node add Keyword --keyword $soft --as branch",
                "node add VertexColor --as vc",
                "wire $vc SurfaceDescription.BaseColor",
                "node insert Multiply --after $vc --wire B=$amountNode --as scaled",
                "group new \"Patched\" \"id:$scaled\"",
                "unwire SurfaceDescription.Alpha",
                "echo done");
            var scriptPath = Path.Combine(s_Scratch, "verbs.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);

            var doc = Load(path);
            Equal(1, doc.Keywords.Count, "keyword add works in a script");
            Equal(1, doc.Properties.Count, "prop add works in a script");
            Check(doc.Nodes.Any(n => n.TypeLabel == "Multiply"), "node insert works in a script");
            Equal(1, doc.Groups.Count, "group new works in a script");
            var described = string.Join(" ", TargetSettings.Describe(doc.Targets[0]));
            Check(described.Contains("surface=transparent"), "target set works in a script: " + described);
            Equal(0, Validator.Run(doc).Count, "the result validates clean");
        }

        static void SubGraphOutputIsNameable()
        {
            var path = Path.Combine(s_Scratch, "outport.shadersubgraph");
            Sh("new", path, "--force");
            var script = string.Join("\n",
                "node add Multiply --as product --set A=2 --set B=3",
                "wire $product OutVector4");
            var scriptPath = Path.Combine(s_Scratch, "outport.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);

            var doc = Load(path);
            var output = doc.OutputNode;
            Check(output != null, "the subgraph has an output node");
            Equal(1, doc.EdgesIn(output).Count, "a bare port name wired to it, the way a block does");
        }

        /// <summary>
        /// Unity stores which faces to RENDER. Calling that "cull" inverts the reading, so the
        /// key is named after the field and the ambiguous spelling is refused.
        /// </summary>
        static void RenderFaceIsNotCalledCull()
        {
            var path = NewGraph("renderface");
            var described = string.Join(" ", TargetSettings.Describe(Load(path).Targets[0]));
            Check(described.Contains("renderface="), "the key is renderface: " + described);
            Check(!described.Contains("cull="), "and not cull");

            Sh("target", "set", path, "cull=off");
            Check(string.Join(" ", TargetSettings.Describe(Load(path).Targets[0])).Contains("renderface=both"),
                "cull=off is kept as an unambiguous alias for both");

            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "target", "set", path, "cull=front" }, TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException e)
            {
                failed = true;
                Check(e.Message.Contains("renderface=front", StringComparison.Ordinal), "the message says what to use: " + e.Message);
            }
            Check(failed, "the ambiguous spelling is refused rather than guessed at");
        }

        /// <summary>
        /// A property only receives an Entities Graphics override when it is declared
        /// HybridPerInstance. The default is UnityPerMaterial, the override then does nothing,
        /// and nothing warns — so the value has to be both settable and visible.
        /// </summary>
        static void DeclarationIsVisibleAndSettable()
        {
            var path = NewGraph("declaration");
            var id = Sh("prop", "add", path, "Color", "--name", "Mesh Color", "--ref", "_MeshColor").Split('\n')[0].Trim();

            var doc = Load(path);
            var property = doc.ResolveProperty(id);
            Equal(HlslDeclaration.UnityPerMaterial, property.Declaration, "an exposed property defaults to per-material");
            Check(!property.DeclarationOverridden, "and that default is not an override");
            Check(!DenseText.Render(doc).Contains("decl=", StringComparison.Ordinal), "so nothing is printed for it");

            Sh("prop", "set", path, id, "--declaration", "hybrid-per-instance");
            doc = Load(path);
            property = doc.ResolveProperty(id);
            Equal(HlslDeclaration.HybridPerInstance, property.Declaration, "after being set");
            Check(property.DeclarationOverridden, "recorded as an override");
            Check(property.Exposed, "and forced exposed, as the Editor does");
            Check(DenseText.Render(doc).Contains("decl=hybrid-per-instance", StringComparison.Ordinal), "now printed");

            // The raw fields have to be what Shader Graph reads, not just what the tool prints.
            Check(ShaderWaitress.Serialization.Json.Bool(property.Entry.Node["overrideHLSLDeclaration"], false),
                "overrideHLSLDeclaration is true");
            Equal(3, ShaderWaitress.Serialization.Json.Int(property.Entry.Node["hlslDeclarationOverride"], -1),
                "hlslDeclarationOverride is HybridPerInstance");

            Sh("prop", "set", path, id, "--declaration", "default");
            Check(!Load(path).ResolveProperty(id).DeclarationOverridden, "and the override can be cleared");
        }

        static void DeclarationChecksWhatTheTypeAccepts()
        {
            var path = NewGraph("declaration-allowed");
            var texture = Sh("prop", "add", path, "Texture2D", "--name", "Tex", "--ref", "_Tex").Split('\n')[0].Trim();
            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "prop", "set", path, texture, "--declaration", "hybrid-per-instance" },
                    TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException e)
            {
                failed = true;
                Check(e.Message.Contains("per-material", StringComparison.Ordinal), "the message lists what is allowed: " + e.Message);
            }
            Check(failed, "a texture cannot be per-instance, and Shader Graph is the one saying so");
        }

        /// <summary>
        /// Custom Function nodes carry no ports of their own, so without this the node type
        /// cannot be authored at all — and it is the only route to hand-written HLSL.
        /// </summary>
        static void PortsCanBeAuthored()
        {
            var path = NewGraph("ports");
            var id = Sh("node", "add", path, "CustomFunction",
                "--set", "SourceType=String", "--set", "FunctionName=Fade",
                "--set", "FunctionBody=Out = In;").Split('\n')[0].Trim();

            Equal(0, Load(path).ResolveNode(id).Ports.Count, "a fresh Custom Function has no ports");

            Sh("node", "port", "add", path, "id:" + id, "--name", "In", "--type", "Vector4", "--direction", "in");
            Sh("node", "port", "add", path, "id:" + id, "--name", "Out", "--type", "Vector4", "--direction", "out");

            var doc = Load(path);
            var node = doc.ResolveNode(id);
            Equal(2, node.Ports.Count, "both ports exist");
            Check(node.Inputs.Any(p => p.Name == "In"), "the input is there");
            Check(node.Outputs.Any(p => p.Name == "Out"), "and the output");
            Check(node.Ports.Select(p => p.Id).Distinct().Count() == 2, "with distinct slot ids");
            Equal("Fade (Custom Function)", node.Name, "the node titles itself after the function, as the Editor does");

            Sh("wire", path, id + ".Out", "BaseColor");
            Equal(1, Load(path).EdgesOut(Load(path).ResolveNode(id)).Count, "an authored port can be wired");

            // Removing a port takes its wires with it.
            Sh("node", "port", "rm", path, "id:" + id, "--name", "Out");
            doc = Load(path);
            node = doc.ResolveNode(id);
            Equal(1, node.Ports.Count, "the port is gone");
            Equal(0, doc.EdgesOut(node).Count, "and so is the wire that used it");
            Equal(0, Validator.Run(doc).Count(p => p.StartsWith("orphan", StringComparison.Ordinal)), "no orphan slot left behind");
        }

        static void ScriptJoinsContinuedLines()
        {
            var path = NewGraph("continuation");
            var script = string.Join("\n",
                "prop add Color --name \"Tint\" --ref _Tint \\",
                "    --value 1,0,0,1 \\",
                "    --declaration hybrid-per-instance --as tint",
                "node add Property --property $tint --as tintNode",
                "wire $tintNode SurfaceDescription.BaseColor");
            var scriptPath = Path.Combine(s_Scratch, "continuation.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);

            var doc = Load(path);
            Equal(1, doc.Properties.Count, "the continued line ran as one command");
            Equal(HlslDeclaration.HybridPerInstance, doc.Properties[0].Declaration, "with every option applied");

            // A line that starts with an option is a continuation the author forgot to mark.
            File.WriteAllText(scriptPath, "prop add Float --name \"A\"\n  --ref _A\n");
            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "apply", path, scriptPath }, TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException e)
            {
                failed = true;
                Check(e.Message.Contains("backslash", StringComparison.Ordinal), "and the error says why: " + e.Message);
            }
            Check(failed, "an unmarked continuation is refused");
        }

        /// <summary>
        /// A SubGraph, Property or Keyword node has no ports until it's bound to what it reads,
        /// so `insert` has to bind before it resolves the port it splices into.
        /// </summary>
        static void InsertBindsBeforeResolvingPorts()
        {
            var subPath = Path.Combine(s_Scratch, "spliced.shadersubgraph");
            Sh("new", subPath, "--force");
            Sh("prop", "add", subPath, "float", "--name", "In", "--ref", "_In");
            // The reference is by guid, which outside a test comes from Unity's import.
            File.WriteAllText(subPath + ".meta", "fileFormatVersion: 2\nguid: " + Guid.NewGuid().ToString("N") + "\n");

            var path = NewGraph("insertbind");
            var source = Sh("node", "add", path, "VertexColor").Split('\n')[0].Trim();
            Sh("wire", path, source, "BaseColor");
            Sh("node", "insert", path, "SubGraph", "--subgraph", subPath, "--after", "id:" + source, "--port", "_In");

            var doc = Load(path);
            var inserted = doc.Nodes.First(n => n.TypeName == "UnityEditor.ShaderGraph.SubGraphNode");
            // Unbound, the node has no ports at all and --port cannot resolve anything.
            Equal(1, inserted.Inputs.Count(), "the bound subgraph contributed its input");
            Equal(1, doc.EdgesIn(inserted).Count, "the intercepted output feeds it");
            Equal(inserted.Inputs.First().Id, doc.EdgesIn(inserted)[0].ToSlot, "on the port --port named");
            Check(doc.EdgesOut(inserted).Count > 0, "and the consumer moved across");
            Equal(0, Validator.Run(doc).Count, "the result validates clean");
        }

        /// <summary>
        /// "Allow Definition Override" is on by default, and while it's on, a Keyword node
        /// compiles to a runtime ternary instead of an #if. It has to be settable and visible.
        /// </summary>
        static void KeywordDefinitionOverrideIsVisibleAndSettable()
        {
            var path = NewGraph("allowoverride");
            Sh("keyword", "add", path, "boolean", "--name", "Use Detail", "--ref", "_USE_DETAIL");
            Check(Load(path).ResolveKeyword("Use Detail").AllowDefinitionOverride, "on by default, the way Unity writes it");
            Check(Sh("keyword", "list", path).Contains("allow-override=on", StringComparison.Ordinal), "listed either way");

            Sh("keyword", "set", path, "Use Detail", "--allow-definition-override", "off");
            var doc = Load(path);
            Check(!doc.ResolveKeyword("Use Detail").AllowDefinitionOverride, "turned off");
            Check(DenseText.Render(doc).Contains("allow-override=off", StringComparison.Ordinal),
                "and the dense text says so, since off is not the default");

            Sh("keyword", "add", path, "boolean", "--name", "Fast", "--ref", "_FAST", "--allow-definition-override", "off");
            Check(!Load(path).ResolveKeyword("Fast").AllowDefinitionOverride, "settable at creation too");

            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "keyword", "set", path, "Fast", "--allow-definition-override", "maybe" },
                    TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException)
            {
                failed = true;
            }
            Check(failed, "a value that is neither on nor off is refused");
        }

        /// <summary>
        /// A group member placed away from the rest leaves an empty band in the group box, which
        /// the score has to notice. A group laid out as a chain has no band, however wide it is.
        /// </summary>
        static void LayoutClosesGapsInsideGroups()
        {
            var path = BuildSampleGraph("groupgap");
            Sh("layout", path, "--force");
            var members = Load(path).Nodes.Where(n => !n.IsBlock).Take(3).Select(n => n.ShortId).ToList();
            Sh("group", "new", path, "Chain", "id:" + string.Join(",", members));
            // Not exactly zero. Value editors hanging left of a node add a few px of spacing,
            // which shows up as a small hole.
            Check(LayoutMetrics.Measure(Load(path)).GroupGaps < 100, "a chain of neighbours has no real gap");

            // Fling one member far away.
            var doc = Load(path);
            var stray = doc.Nodes.First(n => n.ShortId == members[0]);
            stray.Position = new Rect(4000, 2000, stray.Position.Width, stray.Position.Height);
            doc.Save();
            Check(LayoutMetrics.Measure(Load(path)).GroupGaps > 1000, "a stray member registers as a gap");

            Sh("layout", path, "--force");
            Check(LayoutMetrics.Measure(Load(path)).GroupGaps < 100, "and a relayout closes it");
        }

        /// <summary>
        /// Placing a new node incrementally must put it beside the members its group already
        /// has, not beside whatever consumes it, or the group box stretches out to reach it.
        /// </summary>
        static void IncrementalPlacementStaysInsideItsGroup()
        {
            var path = BuildSampleGraph("groupjoin");
            Sh("layout", path, "--force");
            var anchor = Load(path).Nodes.First(n => n.TypeLabel == "Time");
            Sh("group", "new", path, "Scroll", "id:" + anchor.ShortId);

            Sh("node", "add", path, "Multiply", "--group", "Scroll", "--wire", "A=" + anchor.ObjectId + ".Time");
            Equal(0.0, LayoutMetrics.Measure(Load(path)).GroupGaps, "the newcomer landed beside its group");
        }

        /// <summary>
        /// Some graphs shipping inside URP still use the pre-10.0 format, which has no object ids.
        /// Loading one has to say what the file is, not throw an internal error.
        /// </summary>
        static void LegacyFormatIsNamed()
        {
            const string legacy =
                "{\n    \"m_SerializedProperties\": [],\n" +
                "    \"m_SerializableNodes\": [\n        {\n            \"JSONnodeData\": \"{}\"\n        }\n    ]\n}\n";
            var path = Path.Combine(s_Scratch, "old.shadergraph");
            File.WriteAllText(path, legacy);

            var caught = false;
            try
            {
                ShaderGraphDocument.Load(path);
            }
            catch (ShaderWaitressException e)
            {
                caught = true;
                Check(e.LegacyGraphFormat, "recognised as the old format rather than a corrupt file");
                Check(e.Message.Contains("pre-10.0", StringComparison.Ordinal), "and the message says so: " + e.Message);
            }
            Check(caught, "a pre-10.0 graph fails with a message, not an internal error");
        }

        /// <summary>
        /// Boxes that merely touch pass the overlap test but can overlap on screen, because a
        /// stored size can be older than what the node draws now. Refinement must not slide nodes
        /// flush to save a few px of wire.
        /// </summary>
        static void LayoutLeavesDaylightBetweenNodes()
        {
            var path = BuildSampleGraph("clearance");
            Sh("layout", path, "--force");

            var doc = Load(path);
            var metrics = LayoutMetrics.Measure(doc);
            Equal(0, metrics.NodeOverlaps, "nothing overlaps");
            Equal(0, metrics.Crowded, "and nothing is packed closer than the clearance");

            var rects = LayoutMetrics.Rects(doc);
            foreach (var a in doc.Nodes.Where(n => !n.IsBlock))
            {
                foreach (var b in doc.Nodes.Where(n => !n.IsBlock))
                {
                    if (ReferenceEquals(a, b))
                        continue;
                    Check(!rects[a.ObjectId].Overlaps(rects[b.ObjectId], LayoutMetrics.Clearance),
                        $"{a.ShortId} and {b.ShortId} are closer than {LayoutMetrics.Clearance} px");
                }
            }
        }

        /// <summary>
        /// Only the Editor can measure a node. A guessed size written back would be trusted by
        /// the next run as if it were measured.
        /// </summary>
        static void LayoutWritesPositionsNotSizes()
        {
            var path = BuildSampleGraph("sizes");
            Sh("layout", path, "--force");
            foreach (var node in Load(path).Nodes.Where(n => !n.IsBlock))
            {
                Check(node.HasStoredPosition, $"{node.ShortId} was placed");
                Equal(0.0, node.Position.Width, $"{node.ShortId} was left unsized");
                Equal(0.0, node.Position.Height, $"{node.ShortId} was left unsized");
            }

            // A size the Editor wrote has to survive.
            var doc = Load(path);
            var target = doc.Nodes.First(n => !n.IsBlock);
            target.Position = new Rect(target.Position.X, target.Position.Y, 217, 311);
            doc.Save();
            Sh("layout", path, "--force");
            var after = Load(path).ResolveNode(target.ObjectId);
            Equal(217.0, after.Position.Width, "a measured width survives a relayout");
            Equal(311.0, after.Position.Height, "so does the height");
        }

        static void PreviewCanBeCollapsed()
        {
            var path = BuildSampleGraph("preview");
            var sampler = Load(path).Nodes.First(n => n.TypeLabel == "SampleTexture2D");
            Check(sampler.PreviewExpanded, "previews start expanded, the way Unity writes them");
            var tall = NodeSizer.Estimate(sampler).height;

            Sh("node", "set", path, "id:" + sampler.ShortId, "--preview", "off");
            var collapsed = Load(path).ResolveNode(sampler.ObjectId);
            Check(!collapsed.PreviewExpanded, "collapsed");
            Check(NodeSizer.Estimate(collapsed).height < tall - 100, "and the node is measured shorter for it");

            Sh("node", "set", path, "id:" + sampler.ShortId, "--preview", "on");
            Check(Load(path).ResolveNode(sampler.ObjectId).PreviewExpanded, "and back on again");

            var failed = false;
            try
            {
                CommandRouter.Run(new[] { "node", "set", path, "id:" + sampler.ShortId, "--preview", "maybe" },
                    TextWriter.Null, TextWriter.Null);
            }
            catch (ShaderWaitressException)
            {
                failed = true;
            }
            Check(failed, "a value that is neither on nor off is refused");
        }

        /// <summary>
        /// A promoted sub-graph input is declared on the parent shader instead of arriving through
        /// the Sub Graph node. Shader Graph stores this as the sub-graph's own asset guid, and the
        /// node loses the input port for it.
        /// </summary>
        static void PromotionReachesTheParentShader()
        {
            var subPath = Path.Combine(s_Scratch, "promote.shadersubgraph");
            Sh("new", subPath, "--force");
            const string guid = "0123456789abcdef0123456789abcdef";
            File.WriteAllText(subPath + ".meta", "fileFormatVersion: 2\nguid: " + guid + "\n");
            Sh("prop", "add", subPath, "Color", "--name", "Tint", "--ref", "_Tint", "--promote", "on");
            Sh("prop", "add", subPath, "float", "--name", "Amount", "--ref", "_Amount");

            var sub = Load(subPath);
            var promoted = sub.ResolveProperty("_Tint");
            Check(promoted.Promoted, "promoted");
            Equal(guid, (string)promoted.Entry.Node["promotedFromAssetID"], "stores the sub-graph's own guid");
            Equal(-1, ShaderWaitress.Serialization.Json.Int(promoted.Entry.Node["promotedOrdering"], 0),
                "and the ordering Shader Graph writes with it");
            Check(!sub.ResolveProperty("_Amount").Promoted, "the other one is untouched");
            Check(Sh("prop", "list", subPath).Contains("promote=on", StringComparison.Ordinal), "listed either way");

            // A promoted input doesn't arrive through the node, so it has no port.
            var host = NewGraph("promotehost");
            Sh("node", "add", host, "SubGraph", "--subgraph", subPath);
            var node = Load(host).Nodes.First(n => n.TypeName == "UnityEditor.ShaderGraph.SubGraphNode");
            Check(node.Inputs.All(p => p.Name != "_Tint" && p.Name != "Tint"),
                "the promoted property is not a port on the node");
            Equal(1, node.Inputs.Count(), "and the ordinary one is the only port left");

            Sh("prop", "set", subPath, "_Tint", "--promote", "off");
            Check(!Load(subPath).ResolveProperty("_Tint").Promoted, "and it can be turned back off");

            // Only a sub-graph has a parent to promote into.
            var plain = BuildSampleGraph("promoteplain");
            Expect<ShaderWaitressException>(
                () => CommandRouter.Run(new[] { "prop", "set", plain, "_Speed", "--promote", "on" }, TextWriter.Null, TextWriter.Null),
                e => e.Message.Contains("sub-graph", StringComparison.Ordinal),
                "a shader graph cannot promote");

            Sh("prop", "add", subPath, "Gradient", "--name", "Ramp", "--ref", "_Ramp");
            Expect<ShaderWaitressException>(
                () => CommandRouter.Run(new[] { "prop", "set", subPath, "_Ramp", "--promote", "on" }, TextWriter.Null, TextWriter.Null),
                e => e.Message.Contains("Gradient", StringComparison.Ordinal),
                "a Gradient cannot be promoted");
        }

        static void Expect<T>(Action action, Func<T, bool> check, string what) where T : Exception
        {
            try
            {
                action();
            }
            catch (T e)
            {
                Check(check(e), $"{what}, but the message was: {e.Message}");
                return;
            }
            throw new Exception(what + ", but nothing was thrown");
        }

        /// <summary>
        /// A well-arranged graph can still have one node sitting under a wire, and a relayout
        /// from scratch can't fix that, since it would build the same arrangement. The node has
        /// to be moved out of the wire's way in place.
        /// </summary>
        static void LaneOpeningClearsAWireInPlace()
        {
            var path = NewGraph("lanes");
            // A four-Multiply chain, and the Time node that starts it also feeding an output
            // directly. That direct wire has to cross every column of the chain.
            var script = string.Join("\n",
                "node add Time --as t",
                "node add Multiply --as a1 --wire A=$t.Time",
                "node add Multiply --as a2 --wire A=$a1",
                "node add Multiply --as a3 --wire A=$a2",
                "node add Multiply --as a4 --wire A=$a3",
                "wire $a4 SurfaceDescription.BaseColor",
                "wire $t.Time SurfaceDescription.Alpha");
            var scriptPath = Path.Combine(s_Scratch, "lanes.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            Sh("layout", path, "--force");

            // Drop the second-to-last node of the chain into the Time node's long wire, and leave
            // everything else where layout put it.
            var doc = Load(path);
            var stray = doc.Nodes.Where(n => !n.IsBlock).OrderByDescending(n => n.Position.X).Skip(1).First();
            var home = stray.Position;

            // Search for a spot under the wire instead of hardcoding one, since where the wire
            // runs depends on every node size in the graph.
            LayoutMetrics before = null;
            for (var step = -40; step <= 40 && before == null; step++)
            {
                stray.Position = new Rect(home.X, home.Y + step * 20, home.Width, home.Height);
                doc.Save();
                var trial = LayoutMetrics.Measure(Load(path));
                if (trial.WiresOverNodes >= 1 && trial.NodeOverlaps == 0)
                    before = trial;
            }
            Check(before != null, "no position put the node under the long wire and nothing else");

            var engine = new LayoutEngine(Load(path));
            engine.Run();
            Check(engine.Repaired,
                "the wire should be cleared by nudging what is there, not by a relayout or by keeping it");
            Equal(0, engine.After.WiresOverNodes, "and no wire should be left lying over a node");
        }

        /// <summary>
        /// The vertex and fragment stacks start in the same column. Each reserves a lane on its
        /// left for its blocks' value editors, and the lanes differ whenever one stack is wired
        /// and the other isn't, which is the usual case for an unlit graph.
        /// </summary>
        static void ContextStacksShareAColumn()
        {
            var path = NewGraph("stacks");
            // Every fragment block has to be wired. One unconnected block would make both stacks
            // reserve the same lane, and the test would pass by accident.
            var script = string.Join("\n",
                "node add Time --as t",
                "node add Multiply --as m --wire A=$t.Time",
                "wire $m SurfaceDescription.BaseColor",
                "wire $t.Time SurfaceDescription.Alpha");
            var scriptPath = Path.Combine(s_Scratch, "stacks.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            Sh("layout", path, "--force");

            var doc = Load(path);
            // Precondition: the vertex stack reserves a lane and the fragment stack doesn't.
            Check(doc.ContextBlocks(true).All(b => doc.EdgesIn(doc.NodeById(b)).Count == 0),
                "nothing wires the vertex blocks here");
            Check(doc.ContextBlocks(false).All(b => doc.EdgesIn(doc.NodeById(b)).Count > 0),
                "and everything wires the fragment ones");

            var rects = LayoutMetrics.Rects(doc);
            var vertexX = rects[doc.ContextBlocks(true).First()].Right - NodeSizer.BlockWidth;
            var fragmentX = rects[doc.ContextBlocks(false).First()].Right - NodeSizer.BlockWidth;
            Equal(vertexX, fragmentX, "the two stacks draw in the same column");
            Equal(0, LayoutMetrics.Measure(doc).ContextsMisaligned, "and the score agrees");
        }

        /// <summary>
        /// A keyword dropped into a graph draws as a full node with a port per entry and a
        /// preview, not as a property-style token.
        /// </summary>
        static void KeywordNodesAreNodesNotTokens()
        {
            var path = NewGraph("keywordnode");
            Sh("keyword", "add", path, "boolean", "--name", "Fancy", "--ref", "_FANCY");
            var script = string.Join("\n",
                "node add Time --as t",
                "node add Multiply --as m1 --wire A=$t.Time",
                "node add Multiply --as m2 --wire A=$t.Time",
                "node add Keyword --keyword _FANCY --as k --wire On=$m1 --wire Off=$m2",
                "wire $k SurfaceDescription.BaseColor");
            var scriptPath = Path.Combine(s_Scratch, "keywordnode.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);

            var doc = Load(path);
            var keyword = doc.Nodes.First(n => n.IsKeyword);
            var (width, height) = NodeSizer.Estimate(keyword);
            // Measured off a live Shader Graph window: two entry rows and a preview.
            Equal(208.0, width, "a keyword node is as wide as a previewing node");
            Equal(302.0, height, "and as tall as two rows plus a preview");
        }

        /// <summary>
        /// An unconnected input draws an opaque value editor 220 px to the left of its node.
        /// Nothing should be placed under it.
        /// </summary>
        static void ValueEditorsDoNotCoverNeighbours()
        {
            var path = NewGraph("editors");
            var script = string.Join("\n",
                "node add Time --as t",
                "node add Multiply --as m1 --wire A=$t.Time",
                "node add Multiply --as m2 --wire A=$m1",
                "wire $m2 SurfaceDescription.BaseColor");
            var scriptPath = Path.Combine(s_Scratch, "editors.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            Sh("layout", path, "--force");

            // Park a node under one of the value editors. Ask for the editor's rect rather than
            // hardcoding it, so the test follows the measurement.
            var doc = Load(path);
            var host = doc.Nodes.FirstOrDefault(n => NodeSizer.PortEditors(doc, n, NodeSizer.Measure(n)).Any());
            Check(host != null, "every Multiply here has a spare input, so one of them draws an editor");
            var lane = NodeSizer.PortEditors(doc, host, NodeSizer.Measure(host)).First();
            var victim = doc.Nodes.First(n => !n.IsBlock && !n.Is(host));
            var size = NodeSizer.Measure(victim);
            victim.Position = new Rect(lane.X, lane.Y - 40, victim.Position.Width, victim.Position.Height);
            doc.Save();

            var before = LayoutMetrics.Measure(Load(path));
            Check(before.EditorOverlaps >= 1,
                $"{victim.ShortId} sits under {host.ShortId}'s value editor and should count as covered");
            Equal(0, before.NodeOverlaps, "the two node bodies themselves do not overlap");

            Sh("layout", path, "--force");
            Equal(0, LayoutMetrics.Measure(Load(path)).EditorOverlaps, "and a relayout clears the lane");
        }

        /// <summary>
        /// Two groups that feed each other form a cycle between their layout units. Ranking has
        /// to break it without dumping the whole cycle in the rightmost column.
        /// </summary>
        static void GroupCycleDoesNotWreckRanking()
        {
            var path = NewGraph("groupcycle");
            // The nodes themselves are acyclic, as Shader Graph requires. The cycle is between
            // layout units: A -> m1 -> m2 -> B -> m3 -> A.
            var script = string.Join("\n",
                "node add Time --as time",
                "node add UV --as uv",
                "node add Multiply --as a1 --wire A=$time.Time",
                "node add Add      --as m1 --wire A=$a1",
                "node add Multiply --as m2 --wire A=$m1",
                "node add Add      --as b1 --wire A=$m2",
                "node add Multiply --as m3 --wire A=$b1",
                "node add Add      --as a2 --wire A=$m3 --wire B=$uv",
                "node add Multiply --as b2 --wire A=$a2",
                "wire $b2 SurfaceDescription.BaseColor",
                "group new \"A\" \"id:$a1,$a2\"",
                "group new \"B\" \"id:$b1,$b2\"");
            var scriptPath = Path.Combine(s_Scratch, "groupcycle.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            Sh("layout", path, "--force");

            var doc = Load(path);
            var metrics = LayoutMetrics.Measure(doc);
            Equal(0, metrics.NodeOverlaps, "nothing overlaps");
            // Exactly one wire has to close the loop. More means the cycle broke the ranking.
            Check(metrics.BackwardEdges <= 1,
                $"only the wire that closes the cycle should run backwards, not {metrics.BackwardEdges}");

            // The visible symptom would be sources dumped into the rightmost column.
            var rects = LayoutMetrics.Rects(doc);
            var rightmost = doc.Nodes.Where(n => !n.IsBlock).Max(n => rects[n.ObjectId].X);
            foreach (var node in doc.Nodes.Where(n => !n.IsBlock && !n.IsProperty))
            {
                if (doc.EdgesIn(node).Count == 0 && doc.EdgesOut(node).Count > 0)
                {
                    Check(rects[node.ObjectId].X < rightmost,
                        $"{node.ShortId} ({node.TypeLabel}) feeds something but sits in the last column");
                }
            }
        }

        /// <summary>
        /// Blocks are the graph's outputs. Adding one has to list it in the right context, or
        /// Shader Graph does not see it, and there was no verb that named the operation at all.
        /// </summary>
        static void BlocksCanBeAddedAndRemoved()
        {
            var path = NewGraph("blocks", "universal/lit");
            Check(Load(path).Nodes.All(n => n.BlockDescriptor != "SurfaceDescription.Alpha"),
                "the opaque starter has no Alpha block");

            Sh("block", "add", path, "SurfaceDescription.Alpha");
            var doc = Load(path);
            var alpha = doc.Nodes.FirstOrDefault(n => n.BlockDescriptor == "SurfaceDescription.Alpha");
            Check(alpha != null, "the block exists");
            Check(alpha.IsBlock, "and is a block");
            Check(doc.ContextBlocks(vertex: false).Contains(alpha.ObjectId),
                "listed in the fragment context, which is what makes Shader Graph see it");
            Equal(0, Validator.Run(doc).Count, "validates clean");

            // The short name is what a caller reaches for.
            Sh("block", "add", path, "AlphaClipThreshold");
            Check(Load(path).Nodes.Any(n => n.BlockDescriptor == "SurfaceDescription.AlphaClipThreshold"),
                "resolved by the half after the dot too");

            // Adding one that is already there is not an error, and does not duplicate it.
            Sh("block", "add", path, "SurfaceDescription.Alpha");
            Equal(1, Load(path).Nodes.Count(n => n.BlockDescriptor == "SurfaceDescription.Alpha"), "still one");

            Sh("block", "rm", path, "block:AlphaClipThreshold");
            doc = Load(path);
            Check(doc.Nodes.All(n => n.BlockDescriptor != "SurfaceDescription.AlphaClipThreshold"), "removed");
            Check(!doc.ContextBlocks(vertex: false).Contains(
                      doc.Nodes.FirstOrDefault(n => n.BlockDescriptor == "SurfaceDescription.AlphaClipThreshold")?.ObjectId ?? "-"),
                "and delisted from the context");
            Equal(0, Validator.Run(doc).Count, "still validates clean");

            Expect<ShaderWaitressException>(
                () => CommandRouter.Run(new[] { "block", "add", path, "Nonsense" }, TextWriter.Null, TextWriter.Null),
                e => e.Message.Contains("SurfaceDescription.Alpha", StringComparison.Ordinal),
                "an unknown descriptor is refused with the real ones");
        }

        /// <summary>
        /// The block list doesn't follow the target when the target changes, so a graph turned
        /// transparent has nowhere to send alpha. Validate has to say so.
        /// </summary>
        static void BlocksFollowTheTargetSettings()
        {
            var path = NewGraph("blocktarget", "universal/lit");
            Sh("target", "set", path, "surface=transparent");
            var problems = Validator.Run(Load(path));
            Check(problems.Any(p => p.StartsWith("blocks:", StringComparison.Ordinal) &&
                                    p.Contains("Alpha", StringComparison.Ordinal)),
                "validate says the transparent graph has no Alpha block: " + string.Join("; ", problems));

            Sh("target", "set", path, "surface=transparent", "--sync-blocks");
            var doc = Load(path);
            Check(doc.Nodes.Any(n => n.BlockDescriptor == "SurfaceDescription.Alpha"), "--sync-blocks added it");
            Equal(0, Validator.Run(doc).Count, "and the graph validates clean");

            // Only ever adds, since deciding a block is extra needs the Editor's active list.
            var before = doc.Nodes.Count(n => n.IsBlock);
            Sh("target", "set", path, "surface=opaque", "--sync-blocks");
            Equal(before, Load(path).Nodes.Count(n => n.IsBlock), "going back to opaque removes nothing");
        }

        /// <summary>
        /// A texture property samples as White while nothing is assigned, and White read as a
        /// normal map unpacks to a tilted normal. The default has to be settable and visible.
        /// </summary>
        static void TextureDefaultIsVisibleAndSettable()
        {
            var path = NewGraph("texdefault");
            Sh("prop", "add", path, "Texture2D", "--name", "Normal Map", "--ref", "_BumpMap", "--default", "normal-map");
            Sh("prop", "add", path, "Texture2D", "--name", "Mask", "--ref", "_Mask");

            var doc = Load(path);
            Equal(3, doc.ResolveProperty("_BumpMap").TextureDefault, "NormalMap is 3, the way Shader Graph orders them");
            Equal(0, doc.ResolveProperty("_Mask").TextureDefault, "White is what it stores by default");

            var listed = Sh("prop", "list", path);
            Check(listed.Contains("default=normal-map", StringComparison.Ordinal), "listed for the one that was set");
            Check(listed.Contains("default=white", StringComparison.Ordinal), "and for the one that was not, which is the point");
            Check(DenseText.Render(doc).Contains("default=normal-map", StringComparison.Ordinal),
                "dense text carries it when it is not the default");

            Sh("prop", "set", path, "_Mask", "--default", "black");
            Equal(1, Load(path).ResolveProperty("_Mask").TextureDefault, "settable after the fact");

            Sh("prop", "add", path, "Float", "--name", "Speed", "--ref", "_Speed");
            Expect<ShaderWaitressException>(
                () => CommandRouter.Run(new[] { "prop", "set", path, "_Speed", "--default", "black" }, TextWriter.Null, TextWriter.Null),
                e => e.Message.Contains("--value", StringComparison.Ordinal),
                "a non-texture property says what to use instead");
            Expect<ShaderWaitressException>(
                () => CommandRouter.Run(new[] { "prop", "set", path, "_Mask", "--default", "fuchsia" }, TextWriter.Null, TextWriter.Null),
                e => e.Message.Contains("normal-map", StringComparison.Ordinal),
                "an unknown value is refused with the real ones");
        }

        /// <summary>
        /// Whatever feeds the blocks has to be stacked in the block order, or the wires into the
        /// context cross at shallow angles. The column passes have to find columns by right edge,
        /// since layers are right-aligned and nodes of very different widths share a column.
        /// </summary>
        static void ProducersFollowTheBlockOrder()
        {
            var path = NewGraph("blockorder", "universal/lit");
            // Each of these feeds exactly one block, their widths differ a lot, and they're added
            // in the opposite order to the block stack.
            var script = string.Join("\n",
                "prop add Float --name \"Smoothness Amount\" --ref _SmoothnessAmount --as sm",
                "prop add Float --name \"M\" --ref _M --as me",
                "node add Property --property $sm --as smNode",
                "node add Property --property $me --as meNode",
                "node add Multiply --as emission --set A=1 --set B=1",
                "wire $emission SurfaceDescription.Emission",
                "wire $smNode SurfaceDescription.Smoothness",
                "wire $meNode SurfaceDescription.Metallic");
            var scriptPath = Path.Combine(s_Scratch, "blockorder.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            Sh("layout", path, "--force");

            var doc = Load(path);
            var rects = LayoutMetrics.Rects(doc);
            var blockIndex = doc.ContextBlocks(vertex: false)
                .Select((id, i) => (id, i))
                .ToDictionary(p => p.id, p => p.i);

            // For every pair of single-block producers, the one feeding the higher block has to
            // sit higher.
            var producers = doc.Nodes
                .Where(n => !n.IsBlock && doc.EdgesOut(n).Count == 1 && doc.EdgesOut(n)[0].To.IsBlock)
                .Select(n => (node: n, block: blockIndex.GetValueOrDefault(doc.EdgesOut(n)[0].To.ObjectId, -1)))
                .Where(p => p.block >= 0)
                .OrderBy(p => p.block)
                .ToList();
            Check(producers.Count >= 3, $"the graph has {producers.Count} single-block producers to order");

            // Precondition: these share a right edge while their left edges are far apart.
            var rights = producers.Select(p => rects[p.node.ObjectId].Right).Distinct().ToList();
            var lefts = producers.Select(p => rects[p.node.ObjectId].X).ToList();
            Equal(1, rights.Count, "the producers share one right edge");
            Check(lefts.Max() - lefts.Min() > 50, "while their left edges are nowhere near each other");

            for (var i = 0; i + 1 < producers.Count; i++)
            {
                var above = producers[i];
                var below = producers[i + 1];
                Check(rects[above.node.ObjectId].Y < rects[below.node.ObjectId].Y,
                    $"{above.node.ShortId} feeds a block above {below.node.ShortId}'s, so it should sit above it");
            }

            Equal(0, LayoutMetrics.Measure(doc).ShallowCrossings, "and nothing crosses at a shallow angle");
        }

        /// <summary>
        /// Two wires into one node cross when their sources are in the opposite order to the
        /// inputs they feed. Only wires sharing a port may be skipped, not wires sharing a node.
        /// </summary>
        static void CrossingsAtOneNodeAreCounted()
        {
            var path = NewGraph("crossatnode");
            var script = string.Join("\n",
                "node add Time --as top",
                "node add UV --as bottom",
                // top feeds B (the lower input), bottom feeds A (the upper one): they must cross.
                "node add Multiply --as target --wire B=$top.Time --wire A=$bottom",
                "wire $target SurfaceDescription.BaseColor");
            var scriptPath = Path.Combine(s_Scratch, "crossatnode.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);

            var doc = Load(path);
            var top = doc.Nodes.First(n => n.TypeLabel == "Time");
            var bottom = doc.Nodes.First(n => n.TypeLabel == "UV");
            var target = doc.Nodes.First(n => n.TypeLabel == "Multiply");

            // Place them so the two wires must intersect, and far enough apart that nothing else
            // in the graph reaches the region between them.
            target.Position = new Rect(900, 400, 208, 302);
            top.Position = new Rect(300, 200, 208, 254);
            bottom.Position = new Rect(300, 800, 208, 254);
            doc.Save();

            var metrics = LayoutMetrics.Measure(Load(path));
            Check(metrics.Crossings >= 1,
                "the two wires into one node's different inputs count as a crossing");
            Check(metrics.Detail.Any(d => d.StartsWith("crossing", StringComparison.Ordinal)),
                "and --verbose names it");

            // One output feeding two places leaves from a single port and only diverges.
            var fanPath = NewGraph("crossfan");
            var fan = string.Join("\n",
                "node add Time --as t",
                "node add Multiply --as a --wire A=$t.Time",
                "node add Multiply --as b --wire A=$t.Time",
                "node add Add --as sum --wire A=$a --wire B=$b",
                "wire $sum SurfaceDescription.BaseColor");
            var fanScript = Path.Combine(s_Scratch, "crossfan.sw");
            File.WriteAllText(fanScript, fan);
            Sh("apply", fanPath, fanScript);
            Sh("layout", fanPath, "--force");
            Equal(0, LayoutMetrics.Measure(Load(fanPath)).Crossings,
                "one output feeding two nodes is not a crossing with itself");
        }

        /// <summary>
        /// Ranking puts a node one column left of the deepest thing it feeds, which can strand a
        /// property far from its only consumer. Sources have to end up beside what they feed.
        /// </summary>
        static void SourcesSitBesideWhatTheyFeed()
        {
            var path = NewGraph("pullclose", "universal/lit");
            // A long chain, plus a texture property feeding only the sampler near its start.
            var script = string.Join("\n",
                "prop add Texture2D --name \"Tex\" --ref _Tex --as tex",
                "node add Property --property $tex --as texNode",
                "node add UV --as uv",
                "node add TilingAndOffset --as uvs --wire UV=$uv",
                "node add SampleTexture2D --as sample --wire Texture=$texNode --wire UV=$uvs",
                "node add Multiply --as m1 --wire A=$sample.RGBA",
                "node add Multiply --as m2 --wire A=$m1",
                "node add Multiply --as m3 --wire A=$m2",
                "wire $m3 SurfaceDescription.BaseColor");
            var scriptPath = Path.Combine(s_Scratch, "pullclose.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);
            Sh("layout", path, "--force");

            var doc = Load(path);
            var rects = LayoutMetrics.Rects(doc);
            foreach (var node in doc.Nodes.Where(n => !n.IsBlock && doc.EdgesIn(n).Count == 0))
            {
                var consumers = doc.EdgesOut(node)
                    .Where(e => rects.ContainsKey(e.To.ObjectId))
                    .Select(e => rects[e.To.ObjectId].X)
                    .ToList();
                if (consumers.Count == 0)
                    continue;
                var gap = consumers.Min() - rects[node.ObjectId].Right;
                Check(gap >= 0, $"{node.ShortId} is not to the right of what it feeds");
                Check(gap < 300,
                    $"{node.ShortId} ({node.TypeLabel}) sits {gap:0} px from the nearest thing it feeds");
            }
        }

        /// <summary>
        /// Node heights and port positions, pinned to what the Editor draws. Every wire endpoint
        /// is a node box plus a port offset, so an error here shows up on every wire. The heights
        /// are what Shader Graph itself wrote into real graphs.
        /// </summary>
        static void NodeGeometryMatchesTheEditor()
        {
            var path = NewGraph("geometry");
            var script = string.Join("\n",
                "node add Saturate --as sat",
                "node add Multiply --as mul",
                "node add Lerp --as lerp");
            var scriptPath = Path.Combine(s_Scratch, "geometry.sw");
            File.WriteAllText(scriptPath, script);
            Sh("apply", path, scriptPath);

            var doc = Load(path);
            var expected = new Dictionary<string, double>
            {
                ["Saturate"] = 94,      // 1 in, 1 out
                ["Multiply"] = 118,     // 2 in, 1 out
                ["Lerp"] = 142,         // 3 in, 1 out
            };
            foreach (var pair in expected)
            {
                var node = doc.Nodes.First(n => n.TypeLabel == pair.Key);
                Sh("node", "set", path, "id:" + node.ShortId, "--preview", "off");
                var collapsed = Load(path).ResolveNode(node.ObjectId);
                Equal(pair.Value, NodeSizer.Estimate(collapsed).height, $"{pair.Key} with its preview collapsed");

                Sh("node", "set", path, "id:" + node.ShortId, "--preview", "on");
                var expanded = Load(path).ResolveNode(node.ObjectId);
                Equal(pair.Value + NodeSizer.PreviewHeight, NodeSizer.Estimate(expanded).height,
                    $"{pair.Key} with its preview showing");
            }

            // A dropdown adds a row. Only settings that draw a control count, and Sample
            // Texture 2D has more settings than controls.
            var withControl = new Dictionary<string, double>
            {
                ["ScreenPosition"] = 128,   // 1 out, 1 dropdown; the Editor writes 126-129
                ["UV"] = 128,               // same shape
            };
            foreach (var pair in withControl)
            {
                Sh("node", "add", path, pair.Key);
                var node = Load(path).Nodes.Last(n => n.TypeLabel == pair.Key);
                Sh("node", "set", path, "id:" + node.ShortId, "--preview", "off");
                var collapsed = Load(path).ResolveNode(node.ObjectId);
                Equal(pair.Value, NodeSizer.Estimate(collapsed).height, $"{pair.Key} with its dropdown");
            }

            var sampler = Sh("node", "add", path, "SampleTexture2D").Split('\n')[0].Trim();
            Sh("node", "set", path, "id:" + Load(path).ResolveNode(sampler).ShortId, "--preview", "off");
            var texture = Load(path).ResolveNode(sampler);
            Check(texture.Settings.Count > 2, "Sample Texture 2D serializes more settings than it draws");
            var textureHeight = NodeSizer.Estimate(texture).height;
            Check(textureHeight > 240 && textureHeight < 275,
                $"Sample Texture 2D is {textureHeight:0}, and the Editor writes 246-253");

            // Property tokens, against widths measured off a live Shader Graph window. The
            // estimate has to come in under each one. See NodeSizer.Estimate for why.
            var tokens = new (string name, double unity)[]
            {
                ("Metallic", 116),
                ("Alpha Clip Threshold", 183),
                ("Soft Particles Near Fade Distance", 251),
                ("Base Map Tiling and Offset", 218),
            };
            foreach (var (name, unity) in tokens)
            {
                var reference = "_" + name.Replace(" ", string.Empty);
                Sh("prop", "add", path, "Float", "--name", name, "--ref", reference);
                Sh("node", "add", path, "Property", "--property", reference);
                var token = Load(path).Nodes.Last(n => n.IsProperty);
                var width = NodeSizer.Estimate(token).width;
                Check(width <= unity, $"\"{name}\" estimates {width:0}, wider than the {unity:0} the Editor draws");
                Check(width > unity - 30, $"\"{name}\" estimates {width:0}, far under the {unity:0} the Editor draws");
                Equal(NodeSizer.PropertyHeight, NodeSizer.Estimate(token).height, $"\"{name}\" is one row tall");
            }

            // Rows begin below the header, and an input shares its row with the output beside it.
            var multiply = Load(path).Nodes.First(n => n.TypeLabel == "Multiply");
            var height = NodeSizer.Estimate(multiply).height;
            var a = multiply.Inputs.First();
            var b = multiply.Inputs.Skip(1).First();
            var outPort = multiply.Outputs.First();
            Equal(58.0, NodeSizer.PortAnchor(multiply, a, height), "the first input clears the header");
            Equal(82.0, NodeSizer.PortAnchor(multiply, b, height), "the second is a row below it");
            Equal(58.0, NodeSizer.PortAnchor(multiply, outPort, height), "the output is level with the first input");
        }


        /// <summary>
        /// A short id is its object id's first four characters. New objects get ids whose prefix
        /// nothing else has, so the id a command reports never grows, and no existing id changes.
        /// </summary>
        static void NewIdsAvoidTakenPrefixes()
        {
            var raw = Load(BuildSampleGraph("prefixes")).Raw;
            var taken = new HashSet<string>(raw.Entries.Select(e => e.ObjectId.Substring(0, 4)), StringComparer.Ordinal);
            for (var i = 0; i < 2000; i++)
            {
                var prefix = raw.NewObjectId().Substring(0, 4);
                Check(taken.Add(prefix), $"new id #{i} reused the prefix {prefix}");
            }
        }
        static int CountOccurrences(string haystack, string needle)
        {
            var count = 0;
            var index = 0;
            while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }
            return count;
        }
    }
}
