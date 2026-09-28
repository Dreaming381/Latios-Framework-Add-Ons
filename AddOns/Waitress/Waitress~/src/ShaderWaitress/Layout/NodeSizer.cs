using System;
using System.Linq;
using System.Collections.Generic;
using ShaderWaitress.Model;

namespace ShaderWaitress.Layout
{
    /// <summary>
    /// Node sizes. Shader Graph writes the size it measured into the file, so a stored size
    /// always wins, and the estimate only covers nodes the Editor hasn't drawn yet. The constants
    /// were fitted to sizes the Editor wrote in hundreds of real graphs, plus measurements off a
    /// live Shader Graph window.
    /// </summary>
    public static class NodeSizer
    {
        public const double RowHeight = 24;
        public const double HeaderHeight = 46;
        public const double PreviewHeight = 184;
        public const double PreviewMinWidth = 208;
        public const double DefaultWidth = 160;
        public const double BlockWidth = 200;
        public const double BlockHeight = 41;
        public const double PropertyHeight = 34;
        /// <summary>Token width: a base plus a per-glyph share, fitted under the measurements.</summary>
        public const double TokenBaseWidth = 55;
        public const double TokenGlyphWidth = 5.3;
        /// <summary>Shortest type suffix a token can carry, e.g. "(1)".</summary>
        const int k_TypeSuffix = 3;
        /// <summary>The strip of dropdowns and toggles on a node body: padding once, plus a row each.</summary>
        public const double ControlsPadding = 8;
        public const double ControlHeight = 26;
        /// <summary>Padding below the last row, and the preview's collapse strip.</summary>
        public const double BottomPadding = 7;
        public const double PreviewStrip = 17;
        /// <summary>A redirect dot: no title, no rows.</summary>
        public const double RedirectWidth = 56;
        public const double RedirectHeight = 24;

        /// <summary>
        /// A block with an unconnected input draws that input's value editor to the left of the
        /// context, outside the block. Nothing else can sit there.
        /// </summary>
        public const double BlockLiteralWidth = 220;

        /// <summary>How far left of the context this block's own value editor reaches.</summary>
        public static double LiteralWidth(Model.ShaderGraphDocument doc, SgNode block)
        {
            if (doc == null || !block.IsBlock || doc.EdgesIn(block).Count > 0)
                return 0;
            return block.Inputs.Any(p => p.FormattedValue != null) ? BlockLiteralWidth : 0;
        }

        /// <summary>Height of one value editor: a single port row's worth.</summary>
        public const double PortEditorHeight = 22;

        /// <summary>
        /// Where an ordinary node's unconnected inputs draw their value editors. It's the same
        /// widget blocks use, measured at x=-224 w=232 h=22 on every slot type, and it vanishes
        /// once the port is wired. Only the part left of the node is returned, since that's the
        /// part that can land on another node.
        /// </summary>
        public static IEnumerable<Rect> PortEditors(Model.ShaderGraphDocument doc, SgNode node, Rect box)
        {
            // A block's editor is reserved by its context, which owns where the stack sits.
            if (doc == null || node.IsBlock || node.IsProperty || node.IsRedirect)
                yield break;

            var wired = new HashSet<int>(doc.EdgesIn(node).Select(e => e.ToSlot));
            foreach (var port in node.Inputs)
            {
                if (port.Hidden || wired.Contains(port.Id))
                    continue;
                var anchor = PortAnchor(node, port, box.Height);
                yield return new Rect(box.X - BlockLiteralWidth, box.Y + anchor - PortEditorHeight * 0.5,
                                      BlockLiteralWidth, PortEditorHeight);
            }
        }

        public static Rect Measure(SgNode node, bool trustStored = true)
        {
            var stored = node.Position;
            if (trustStored && stored.Width > 1 && stored.Height > 1)
                return new Rect(stored.X, stored.Y, stored.Width, stored.Height);
            var (w, h) = Estimate(node);
            return new Rect(stored.X, stored.Y, w, h);
        }

        public static (double width, double height) Estimate(SgNode node)
        {
            if (node.IsBlock)
                return (BlockWidth, BlockHeight);

            if (node.IsProperty)
            {
                // A token's width follows its label plus a type suffix like "(1)". Measured:
                // "Metallic(1)" is 116, "Alpha Clip Threshold(1)" is 183, and "Soft Particles
                // Near Fade Distance(1)" is 251.
                //
                // The estimate deliberately comes in under all of those. The token's right edge
                // is where its wire starts, and guessing too far right makes the tool expect a
                // steeper wire than Shader Graph draws, missing corners the real wire clips.
                var label = node.Doc?.PropertyById(node.PropertyId)?.Name ?? node.Name ?? string.Empty;
                var glyphs = label.Length + k_TypeSuffix;
                return (Math.Max(TokenBaseWidth + TokenGlyphWidth * glyphs, 110), PropertyHeight);
            }

            // The dot you get by double-clicking a wire.
            if (node.IsRedirect)
                return (RedirectWidth, RedirectHeight);

            // Inputs and outputs share rows, so the row count is the longer list, not the total.
            // Fitted to what the Editor writes: Saturate (1 in, 1 out) is 94, Multiply (2, 1) is
            // 118, Lerp (3, 1) is 142, Split (1, 4) is 149, and Time (0, 5) is 173.
            var inputs = node.Ports.Count(p => p.IsInput && !p.Hidden);
            var outputs = node.Ports.Count(p => !p.IsInput && !p.Hidden);
            var rows = Math.Max(1, Math.Max(inputs, outputs));
            var catalogEntry = node.Doc?.Catalog?.Find(node.TypeName);
            var hasPreview = catalogEntry?.HasPreview ?? false;

            // Only settings that draw a control add height. Without that count from the
            // catalog, the number of settings is the best guess.
            var controls = catalogEntry != null && catalogEntry.ControlRows >= 0
                ? catalogEntry.ControlRows
                : node.Settings.Count;
            var height = HeaderHeight + RowHeight * rows + BottomPadding +
                         (controls > 0 ? ControlsPadding + ControlHeight * controls : 0) +
                         (hasPreview ? PreviewStrip : 0);
            var width = Math.Max(DefaultWidth, 34 + 7.0 * (node.Name ?? node.TypeLabel ?? string.Empty).Length);

            if (hasPreview && node.PreviewExpanded)
            {
                height += PreviewHeight;
                width = Math.Max(width, PreviewMinWidth);
            }
            return (width, height);
        }

        /// <summary>Vertical offset of a port's anchor inside the node body.</summary>
        public static double PortAnchor(SgNode node, SgPort port, double height)
        {
            if (node.IsBlock || node.IsProperty || node.IsRedirect)
                return height * 0.5;
            var siblings = node.Ports.Where(p => p.IsInput == port.IsInput && !p.Hidden).ToList();
            var index = siblings.FindIndex(p => p.Id == port.Id);
            if (index < 0)
                return height * 0.5;
            // Rows start below the header, so the first port is centered at header + half a row.
            var anchor = HeaderHeight + RowHeight * (index + 0.5);
            return Math.Min(anchor, height - RowHeight * 0.5);
        }
    }
}
