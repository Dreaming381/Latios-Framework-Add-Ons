using System;
using System.Collections.Generic;
using System.Linq;
using VfxWaitress.Model;
using VfxWaitress.Query;

namespace VfxWaitress.Edit
{
    /// <summary>
    /// Resolves <c>node.Port</c> and <c>node.Port.child</c> against a graph.
    ///
    /// Slot names rarely match how the node was configured. A SetAttribute block set to
    /// <c>position</c> names its slot <c>_Position</c>, and with random mode on its slots become
    /// <c>A</c> and <c>B</c>. So matching ignores underscores, spaces, and case, and a miss lists
    /// the slots the node does have.
    /// </summary>
    public static class SlotRef
    {
        public static VfxSlot Resolve(VfxAsset asset, string reference, bool? wantInput = null)
        {
            var dot = reference.IndexOf('.');
            var nodeRef = dot < 0 ? reference : reference.Substring(0, dot);
            var slotPath = dot < 0 ? null : reference.Substring(dot + 1);

            var nodes = Selector.Resolve(asset, nodeRef).ToList();
            if (nodes.Count == 0)
                throw new VfxWaitressException($"no node matches '{nodeRef}'");
            if (nodes.Count > 1)
                throw new VfxWaitressException($"'{nodeRef}' matches {nodes.Count} nodes: {string.Join(" ", nodes.Select(n => n.ShortId))}");
            var node = nodes[0];

            // A bare parameter name resolves to the slot carrying its value: an output for an
            // exposed input, or an input for a subgraph output.
            if (node.ExposedName != null && slotPath == null)
            {
                var carrier = (node.IsOutput ? node.Inputs : node.Outputs).FirstOrDefault();
                if (carrier != null)
                    return carrier;
            }

            var candidates = Candidates(node, wantInput).ToList();
            if (slotPath == null)
            {
                var roots = candidates.Where(s => s.Parent == null).ToList();
                if (roots.Count == 1)
                    return roots[0];
                throw new VfxWaitressException(
                    $"{node.ShortId} ({node.DisplayType}) has {roots.Count} {Side(wantInput)} slots; name one: {Listing(roots)}");
            }

            var exact = candidates.FirstOrDefault(s => string.Equals(s.Path, slotPath, StringComparison.Ordinal));
            if (exact != null)
                return exact;
            var loose = candidates.Where(s => Loose.Equal(s.Path, slotPath)).ToList();
            if (loose.Count == 1)
                return loose[0];
            if (loose.Count > 1)
                throw new VfxWaitressException($"'{slotPath}' is ambiguous on {node.ShortId}: {Listing(loose)}");

            throw new VfxWaitressException(
                $"no {Side(wantInput)} slot '{slotPath}' on {node.ShortId} ({node.DisplayType}); has: {Listing(candidates)}");
        }

        static IEnumerable<VfxSlot> Candidates(VfxNode node, bool? wantInput)
        {
            if (wantInput != false)
            {
                foreach (var s in node.AllInputSlots)
                    yield return s;
            }
            if (wantInput != true)
            {
                foreach (var s in node.AllOutputSlots)
                    yield return s;
            }
        }

        static string Side(bool? wantInput) => wantInput == true ? "input" : wantInput == false ? "output" : "";

        static string Listing(IEnumerable<VfxSlot> slots) =>
            string.Join(", ", slots.Select(s => s.Path + (s.Children.Count > 0 ? "/" : "")));
    }
}
