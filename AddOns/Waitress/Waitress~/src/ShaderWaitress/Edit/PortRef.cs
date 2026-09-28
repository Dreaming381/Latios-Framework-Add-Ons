using System;
using System.Linq;
using ShaderWaitress.Model;

namespace ShaderWaitress.Edit
{
    /// <summary>
    /// Parses the "node.Port" references that appear all over the command line. The port half
    /// may be omitted when the node has exactly one port in the required direction, which is
    /// the common case and keeps commands short.
    /// </summary>
    public static class PortRef
    {
        public static SgPort ResolveOutput(ShaderGraphDocument doc, string text) => Resolve(doc, text, input: false);

        public static SgPort ResolveInput(ShaderGraphDocument doc, string text) => Resolve(doc, text, input: true);

        public static SgPort Resolve(ShaderGraphDocument doc, string text, bool input)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ShaderWaitressException("empty port reference");

            // A block is addressed by its descriptor ("SurfaceDescription.BaseColor"); a
            // subgraph has one output node instead, so its port names work the same way.
            var onOutput = OutputNodePort(doc, text, input);
            if (onOutput != null)
                return onOutput;

            var (nodeText, portText) = Split(doc, text);
            var node = doc.ResolveNode(nodeText);
            if (portText != null)
                return node.RequirePort(portText, input);

            var candidates = node.Ports.Where(p => p.IsInput == input && !p.Hidden).ToList();
            if (candidates.Count == 1)
                return candidates[0];
            if (candidates.Count == 0)
                throw new ShaderWaitressException($"{node.ShortId} ({node.TypeLabel}) has no {(input ? "inputs" : "outputs")}");
            throw new ShaderWaitressException(
                $"{node.ShortId} ({node.TypeLabel}) has {candidates.Count} {(input ? "inputs" : "outputs")}; " +
                $"name one of: {string.Join(", ", candidates.Select(p => DenseText.PortToken(p)))}");
        }

        /// <summary>
        /// Splits on the last dot, but only when the left half actually names a node — block
        /// descriptors such as "SurfaceDescription.BaseColor" contain a dot themselves.
        /// </summary>
        static (string node, string port) Split(ShaderGraphDocument doc, string text)
        {
            var dot = text.LastIndexOf('.');
            if (dot <= 0 || dot == text.Length - 1)
                return (text, null);

            var left = text.Substring(0, dot);
            var right = text.Substring(dot + 1);
            if (TryResolve(doc, left) != null)
                return (left, right);
            // Neither half names a node, so report the whole reference rather than blaming the
            // part before the dot: "SurfaceDescription.Alpha" is one name, not node plus port.
            return (text, null);
        }

        /// <summary>
        /// Resolves a bare port name against the graph's output node, so a subgraph can be
        /// wired with "wire $x OutVector4" the way a shader graph is wired with
        /// "wire $x SurfaceDescription.BaseColor". Only used when nothing else names that node.
        /// </summary>
        static SgPort OutputNodePort(ShaderGraphDocument doc, string text, bool input)
        {
            if (text.IndexOf('.') >= 0)
                return null;
            var output = doc.OutputNode;
            if (output == null || TryResolve(doc, text) != null)
                return null;
            return output.FindPort(text, input);
        }

        static SgNode TryResolve(ShaderGraphDocument doc, string handle)
        {
            try
            {
                return doc.ResolveNode(handle);
            }
            catch (ShaderWaitressException)
            {
                return null;
            }
        }
    }
}
