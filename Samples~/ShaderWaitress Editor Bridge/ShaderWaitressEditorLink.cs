// The Editor side of shaderwaitress writes: keeping an open Shader Graph window and the file on
// disk in agreement, and compiling a graph for `shaderwaitress validate --editor`.
//
// Compiled into Unity.ShaderGraph.Editor via the .asmref next to this file, which is what gives
// it access to the Shader Graph window's internals.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

namespace ShaderWaitress
{
    public static class EditorLink
    {
        /// <summary>
        /// Saves unsaved edits in any Shader Graph window showing this graph, so the tool edits what
        /// the person is looking at and their work isn't lost when the file is rewritten.
        /// </summary>
        public static string Prepare(string assetPath)
        {
            var saved = 0;
            foreach (var window in WindowsFor(assetPath))
            {
                if (!HasUnsavedEdits(window))
                    continue;
                if (!window.SaveAsset())
                    return "error: the open Shader Graph window couldn't save its unsaved changes to " + assetPath;
                saved++;
            }
            return saved > 0 ? "saved the open window's unsaved changes" : "clean";
        }

        /// <summary>
        /// Imports a file the tool just wrote and reloads any window showing it. Dropping the
        /// window's graph makes its next update reload from disk before it checks whether the file
        /// changed, so the "Graph has changed on disk" dialog never comes up.
        /// </summary>
        public static string Refresh(string assetPath)
        {
            // Check for edits before importing. The import makes the window recompare its graph to
            // the new file, which marks even an untouched window as changed.
            var windows = WindowsFor(assetPath).Where(w => w.graphObject?.graph != null).ToList();
            var edited = windows.Where(HasUnsavedEdits).ToList();
            Canonicalize(assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            var reloaded = 0;
            var kept = 0;
            foreach (var window in windows)
            {
                // Someone edited the graph after Prepare. Leave it to the window to ask.
                if (edited.Contains(window))
                {
                    kept++;
                    continue;
                }
                window.graphObject = null;
                window.Repaint();
                reloaded++;
            }
            if (kept > 0)
                return "imported " + assetPath + ", but an open window has unsaved changes made since the tool read the file, so it wasn't reloaded";
            return reloaded > 0 ? "imported " + assetPath + " and reloaded the open window" : "imported " + assetPath;
        }

        /// <summary>
        /// Imports and compiles a graph, then reports the shader's errors and warnings. One line per
        /// message, prefixed error/warning/ok, then a summary line.
        /// </summary>
        public static string Compile(string assetPath)
        {
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            var sb = new StringBuilder();
            if (assetPath.EndsWith(".shadersubgraph", System.StringComparison.OrdinalIgnoreCase))
            {
                var subGraph = AssetDatabase.LoadAssetAtPath<SubGraphAsset>(assetPath);
                if (subGraph == null)
                    return "error: no sub-graph at " + assetPath;
                if (!subGraph.isValid)
                    return sb.AppendLine("error: Shader Graph marked the sub-graph invalid").Append("1 error(s), 0 warning(s)").ToString();
                return sb.AppendLine("ok: imported; a sub-graph compiles as part of the graphs that use it")
                         .Append("0 error(s), 0 warning(s)").ToString();
            }

            var shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
            if (shader == null)
                return "error: no shader was imported from " + assetPath;
            var errors = 0;
            var warnings = 0;
            foreach (var message in ShaderUtil.GetShaderMessages(shader))
            {
                var isError = message.severity == UnityEditor.Rendering.ShaderCompilerMessageSeverity.Error;
                sb.AppendLine($"{(isError ? "error" : "warning")}: {message.message.Trim()}" +
                              (string.IsNullOrEmpty(message.file) ? "" : $" ({message.file}:{message.line})"));
                if (isError)
                    errors++;
                else
                    warnings++;
            }
            if (ShaderUtil.ShaderHasError(shader) && errors == 0)
            {
                sb.AppendLine("error: the shader has errors, but Unity reported no messages for them");
                errors++;
            }
            if (errors == 0)
                sb.AppendLine($"ok: compiled \"{shader.name}\"");
            return sb.Append($"{errors} error(s), {warnings} warning(s)").ToString();
        }

        /// <summary>
        /// Rewrites the file as Shader Graph itself would save it, if that differs. A window compares
        /// its graph against the file it loaded, so a file in any other form shows an asterisk after
        /// the reload, and closing the window then asks to save. The steps mirror how the window
        /// loads a graph.
        /// </summary>
        static void Canonicalize(string assetPath)
        {
            var text = File.ReadAllText(assetPath, Encoding.UTF8);
            var graph = new GraphData
            {
                assetGuid = AssetDatabase.AssetPathToGUID(assetPath),
                isSubGraph = assetPath.EndsWith(".shadersubgraph", StringComparison.OrdinalIgnoreCase),
                messageManager = new MessageManager(),
            };
            MultiJson.Deserialize(graph, text);
            graph.OnEnable();
            graph.ValidateGraph();
            if (MultiJson.Serialize(graph) != text)
                FileUtilities.WriteShaderGraphToDisk(assetPath, graph);
        }

        /// <summary>
        /// Whether the person has edits the window would ask to save. This is the window's own
        /// asterisk, not GraphHasChangedSinceLastSerialization, which is also true for a graph Shader
        /// Graph merely serializes differently than the file on disk. An edit marks the graph object
        /// dirty before the window's next update turns it into hasUnsavedChanges, so both count.
        /// </summary>
        static bool HasUnsavedEdits(MaterialGraphEditWindow window) =>
            window.graphObject?.graph != null && (window.hasUnsavedChanges || window.graphObject.isDirty);

        static IEnumerable<MaterialGraphEditWindow> WindowsFor(string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return Resources.FindObjectsOfTypeAll<MaterialGraphEditWindow>().Where(w => w.selectedGuid == guid);
        }
    }
}
