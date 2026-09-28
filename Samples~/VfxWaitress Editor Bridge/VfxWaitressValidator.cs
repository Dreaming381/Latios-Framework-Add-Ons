// The Editor side of vfxwaitress. The tool calls these methods through the Unity CLI for anything
// it can't decide from the file: compiling a graph, re-deriving slots, resolving asset references,
// declaring custom attributes, measuring node sizes, and writing through VFX Graph's own
// serializer.
//
// Compiling matters most. VFX Graph's error reporters stay quiet on graphs that can't compile,
// and a failed compile leaves an asset that still loads, just with every exposed property missing.
//
// Compiled into Unity.VisualEffectGraph.Editor via the .asmref next to this file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.VFX;
using UnityEditor.VFX.Block;
using UnityEditor.VFX.UI;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace VfxWaitress
{
    public static class Validator
    {
        /// <summary>One line per finding, prefixed error/warning/ok, then a summary line.</summary>
        public static string Validate(string assetPath)
        {
            var sb = new StringBuilder();
            var errors = 0;
            var warnings = 0;

            // The tool may have written the file behind the Editor's back, so reload it.
            Reload(assetPath);

            var asset = AssetDatabase.LoadAssetAtPath<VisualEffectAsset>(assetPath);
            VisualEffectResource resource = null;
            if (asset != null)
                resource = asset.GetResource();
            else
                resource = VisualEffectResource.GetResourceAtPath(assetPath);

            if (resource == null)
            {
                sb.AppendLine($"error: no VisualEffectResource at {assetPath}");
                return sb.Append("1 error, 0 warnings").ToString();
            }

            VFXGraph graph;
            try
            {
                graph = resource.GetOrCreateGraph();
            }
            catch (Exception e)
            {
                sb.AppendLine($"error: graph would not load: {Flatten(e)}");
                return sb.Append("1 error, 0 warnings").ToString();
            }

            // The per-model error reporters, which are what the graph view shows as red nodes.
            try
            {
                graph.errorManager.GenerateErrors();
                foreach (var reporter in new[] { graph.errorManager.errorReporter, graph.errorManager.compileReporter })
                {
                    if (reporter == null)
                        continue;
                    foreach (var model in reporter.dirtyModels.ToList())
                    {
                        foreach (var error in reporter.GetDirtyModelErrors(model))
                        {
                            sb.AppendLine($"{(error.type == VFXErrorType.Error ? "error" : "warning")}: {Describe(model)}: {error.description}");
                            if (error.type == VFXErrorType.Error)
                                errors++;
                            else
                                warnings++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                sb.AppendLine($"warning: error reporters threw: {Flatten(e)}");
                warnings++;
            }

            // A Custom HLSL function with more than four inputs throws while its slots resolve,
            // which aborts the whole graph's compilation rather than flagging the one node.
            foreach (var op in graph.children.OfType<UnityEditor.VFX.Operator.CustomHLSL>())
            {
                var inputs = op.inputSlots.Count;
                if (inputs > 4)
                {
                    sb.AppendLine($"error: {op.name}: Custom HLSL function has {inputs} inputs; VFX Graph allows at most 4 (VFXExpression takes at most 4 parents)");
                    errors++;
                }
            }

            // The asset's dirty flag is useless here. The repairs that dirty a graph on open happen
            // when the graph view builds, so a headless load never sees them, and an open window
            // can mark an asset dirty for unrelated reasons. `vfxwaitress repair` checks for those
            // shapes directly.

            // The real test. Everything above can pass on a graph that can't compile.
            try
            {
                graph.SetExpressionGraphDirty();
                graph.RecompileIfNeeded(false, false);
                sb.AppendLine("ok: compiled");
            }
            catch (Exception e)
            {
                sb.AppendLine($"error: compile threw: {Flatten(e)}");
                errors++;
            }

            if (asset != null)
            {
                var exposed = new List<VFXExposedProperty>();
                asset.GetExposedProperties(exposed);
                sb.AppendLine($"ok: {graph.children.Count()} models, {exposed.Count} exposed properties");
                if (exposed.Count == 0 && graph.children.OfType<VFXParameter>().Any(p => p.exposed))
                {
                    sb.AppendLine("error: the graph declares exposed parameters but the compiled asset has none, which is what a failed compile looks like from the runtime side");
                    errors++;
                }
            }

            sb.Append($"{errors} error(s), {warnings} warning(s)");
            return sb.ToString();
        }

        /// <summary>
        /// Rewrites the asset through VFX Graph's own serializer, so what's on disk is exactly what
        /// the Editor would write. Comparing the dense form before and after also makes a good
        /// acceptance test for anything the tool wrote.
        /// </summary>
        public static string Resave(string assetPath)
        {
            // Reload first, or the Editor writes back whatever it still holds in memory and the
            // tool's file is silently lost. Validate and Resync reload for the same reason.
            Reload(assetPath);
            var resource = VisualEffectResource.GetResourceAtPath(assetPath);
            if (resource == null)
                return "error: no VisualEffectResource at " + assetPath;
            var graph = resource.GetOrCreateGraph();
            var before = File.GetLastWriteTimeUtc(assetPath);
            graph.SetExpressionGraphDirty();
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(resource);
            resource.WriteAssetWithSubAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            var after = File.GetLastWriteTimeUtc(assetPath);
            return after == before
                ? "unchanged (the serializer produced the same bytes) " + assetPath
                : "rewritten " + assetPath;
        }

        /// <summary>
        /// Asks VFX Graph to re-derive every node's slots from its settings, then writes the
        /// result. The rule for which slots a setting produces lives in each node's C#, so the
        /// offline tool can't do this itself.
        ///
        /// It's also how an edit to a Custom HLSL file reaches the graphs that use it. Unity caches
        /// the parsed function per node, so a reimport alone leaves the old slots in place.
        /// </summary>
        public static string Resync(string assetPath)
        {
            Reload(assetPath);
            var resource = VisualEffectResource.GetResourceAtPath(assetPath);
            if (resource == null)
                return "error: no VisualEffectResource at " + assetPath;
            var graph = resource.GetOrCreateGraph();

            var changed = new List<string>();
            foreach (var model in Models(graph).ToList())
            {
                try
                {
                    var before = Signature(model);
                    // Without kSettingChanged, the node answers from its cache.
                    model.Invalidate(VFXModel.InvalidationCause.kSettingChanged);
                    if (model is IVFXSlotContainer container)
                        container.ResyncSlots(true);
                    // ResyncSlots only reports its own changes, and the invalidate usually does the
                    // work first, so compare the slots instead.
                    if (Signature(model) != before)
                        changed.Add(Describe(model));
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"VfxWaitress: resync failed on {Describe(model)}: {Flatten(e)}");
                }
            }

            graph.SetExpressionGraphDirty();
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(resource);
            resource.WriteAssetWithSubAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (changed.Count == 0)
                return $"no slots changed in {assetPath}";
            return $"resynced {changed.Count} node(s) in {assetPath}: {string.Join(", ", changed.Distinct())}";
        }

        /// <summary>
        /// Flushes unsaved changes to disk before the tool reads the file. A VFX Graph window keeps
        /// its own copy of the graph, so a person's unsaved edits are invisible in the file, and a
        /// tool that then writes the file would destroy them.
        /// </summary>
        public static string Prepare(string assetPath)
        {
            var resource = VisualEffectResource.GetResourceAtPath(assetPath);
            if (resource == null)
                return "not open";
            VFXGraph graph;
            try
            {
                graph = resource.GetOrCreateGraph();
            }
            catch (Exception e)
            {
                return "error: graph would not load: " + Flatten(e);
            }
            if (!EditorUtility.IsDirty(graph) && !EditorUtility.IsDirty(resource))
                return "clean";
            resource.WriteAssetWithSubAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return "flushed unsaved editor changes to disk";
        }

        /// <summary>
        /// Hands a file the tool just wrote back to the Editor, so an open window shows the new
        /// file instead of its old copy, and nothing is left marked unsaved.
        /// </summary>
        public static string Refresh(string assetPath)
        {
            Reload(assetPath);
            var resource = VisualEffectResource.GetResourceAtPath(assetPath);
            if (resource == null)
                return "error: no VisualEffectResource at " + assetPath;
            var window = VFXViewWindow.GetWindow(resource, false, false);
            if (window?.graphView?.controller == null)
                return "reloaded " + assetPath;
            window.graphView.controller.ForceReload();
            window.Repaint();
            return "reloaded " + assetPath + " and refreshed the open window";
        }

        /// <summary>
        /// Re-reads the asset from disk. The Editor may still hold the graph it loaded earlier, and
        /// saving that would discard what the tool just wrote. Refresh alone isn't reliable, so the
        /// asset is also imported by name.
        /// </summary>
        static void Reload(string assetPath)
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }

        /// <summary>A node's slot shape, for telling whether a resync actually changed it.</summary>
        static string Signature(VFXModel model)
        {
            if (model is not IVFXSlotContainer container)
                return "";
            var sb = new StringBuilder();
            foreach (var slot in container.inputSlots.Concat(container.outputSlots))
                sb.Append(slot.property.name).Append(':').Append(slot.property.type?.Name).Append(';');
            return sb.ToString();
        }

        /// <summary>
        /// The reference a .vfx stores for an asset: its guid plus the local file id of the main
        /// object. That id is a per-importer constant the offline tool can't derive. Output is
        /// "fileID,guid,type", or an error line.
        /// </summary>
        public static string ResolveReference(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset == null)
                return "error: no asset at " + assetPath;
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long localId))
                return "error: could not resolve a reference to " + assetPath;
            // A .vfx uses type 3 for everything it points at.
            return $"{localId},{guid},3";
        }

        /// <summary>
        /// Declares a custom attribute, or corrects the type of one already declared. Changing a
        /// type reshapes the slots of every block that uses the attribute, so the Editor does it.
        ///
        /// <paramref name="spec"/> is "name,type[,description]"; type is a
        /// CustomAttributeUtility.Signature name (Float, Vector2, Vector3, Vector4, Bool, Uint, Int).
        /// </summary>
        public static string AddAttribute(string assetPath, string spec)
        {
            var parts = spec.Split(new[] { ',' }, 3);
            if (parts.Length < 2)
                return "error: expected name,type[,description]";
            var name = parts[0].Trim();
            var description = parts.Length > 2 ? parts[2] : "";
            if (!Enum.TryParse<CustomAttributeUtility.Signature>(parts[1].Trim(), true, out var signature))
                return $"error: '{parts[1].Trim()}' is not an attribute type; one of: {string.Join(", ", Enum.GetNames(typeof(CustomAttributeUtility.Signature)))}";

            Reload(assetPath);
            var resource = VisualEffectResource.GetResourceAtPath(assetPath);
            if (resource == null)
                return "error: no VisualEffectResource at " + assetPath;
            var graph = resource.GetOrCreateGraph();

            string outcome;
            if (graph.TryFindCustomAttributeDescriptor(name, out var existing))
            {
                if (existing.type == signature && existing.description == description)
                    return $"{name} is already declared as {signature}";
                var wasType = existing.type;
                if (!graph.TryUpdateCustomAttribute(name, signature, description))
                    return $"error: could not change {name} from {wasType} to {signature}";
                outcome = $"changed {name} from {wasType} to {signature}";
            }
            else
            {
                if (!graph.TryAddCustomAttribute(name, CustomAttributeUtility.GetValueType(signature), description, false, out _))
                    return $"error: could not declare {name}";
                outcome = $"declared {name} as {signature}";
            }

            graph.SyncCustomAttributes();
            graph.SetExpressionGraphDirty();
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(resource);
            resource.WriteAssetWithSubAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return outcome + " in " + assetPath;
        }

        /// <summary>
        /// The size VFX Graph draws each node at, and where it draws each port, read off a live
        /// graph view. The file stores neither. Opens a VFX Graph window for the asset if none is
        /// open. See <see cref="Node"/> and <see cref="Ports"/> for the line formats.
        /// </summary>
        public static string Measure(string assetPath)
        {
            Reload(assetPath);
            var resource = VisualEffectResource.GetResourceAtPath(assetPath);
            if (resource == null)
                return "error: no VisualEffectResource at " + assetPath;

            var window = VFXViewWindow.GetWindow(resource, true, true);
            if (window == null)
                return "error: could not open a VFX Graph window";
            // A new window starts empty, with no controller and no rects to read.
            if (window.displayedResource != resource)
                window.LoadResource(resource);
            window.Repaint();

            var view = window.graphView;
            if (view?.controller == null)
                return "opened the window; the view has no controller yet, call measure again";

            var sb = new StringBuilder();
            foreach (var node in view.GetAllNodes())
            {
                Node(sb, node);
                // Querying from the context also picks up its blocks' ports, in the context's space.
                Ports(sb, node, node);
            }
            // The rects only exist after the panel runs a layout pass, which happens between
            // calls. An empty first answer is normal.
            return sb.Length == 0
                ? "opened the window; no node has been laid out yet, call measure again"
                : sb.ToString();
        }

        /// <summary>"n,fileId,width,height,type" — the box the layout places.</summary>
        static void Node(StringBuilder sb, VFXNodeUI node)
        {
            var model = node.controller?.model;
            if (model == null)
                return;
            var size = node.layout;
            // A rect that hasn't been laid out yet is NaN, which fails every comparison, so this
            // has to be a positive test.
            if (!(size.width >= 1f) || !(size.height >= 1f))
                return;
            sb.Append("n,").Append(FileIdOf(model)).Append(',')
              .Append(size.width.ToString("0.#")).Append(',')
              .Append(size.height.ToString("0.#")).Append(',')
              .Append(model.GetType().Name).Append('\n');
        }

        /// <summary>
        /// "p,slotFileId,dx,dy" for an input port and "q,..." for an output: where the port is
        /// drawn, measured from the top-left of the element the layout moves. The layout aims
        /// wires at ports, and a tall context's inputs can be spread over hundreds of pixels.
        /// </summary>
        static void Ports(StringBuilder sb, VFXNodeUI node, VisualElement root)
        {
            foreach (var anchor in node.Query<VFXDataAnchor>().ToList())
            {
                var slot = anchor.controller?.model;
                if (slot == null)
                    continue;
                var center = anchor.ChangeCoordinatesTo(root, anchor.layout.size * 0.5f);
                if (!(center.y >= 0f))
                    continue;
                sb.Append(anchor.direction == Direction.Input ? "p," : "q,").Append(FileIdOf(slot)).Append(',')
                  .Append(center.x.ToString("0.#")).Append(',')
                  .Append(center.y.ToString("0.#")).Append('\n');
            }
        }

        /// <summary>The local file id the asset stores for a model, which is how the tool names it.</summary>
        static long FileIdOf(VFXModel model)
        {
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(model, out _, out long id) ? id : model.GetInstanceID();
        }

        static IEnumerable<VFXModel> Models(VFXGraph graph)
        {
            foreach (var child in graph.children)
            {
                yield return child;
                if (child is VFXContext context)
                {
                    foreach (var block in context.children)
                        yield return block;
                }
            }
        }

        static string Describe(VFXModel model)
        {
            if (model == null)
                return "<graph>";
            var label = model is VFXContext context && !string.IsNullOrEmpty(context.label) ? $" \"{context.label}\"" : "";
            return model.GetType().Name + label;
        }

        static string Flatten(Exception e)
        {
            while (e.InnerException != null)
                e = e.InnerException;
            var frame = e.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "";
            return $"{e.GetType().Name}: {e.Message}" + (frame.Length > 0 ? $" @ {frame}" : "");
        }
    }
}
