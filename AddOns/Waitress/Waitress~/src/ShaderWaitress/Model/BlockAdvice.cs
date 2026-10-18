using System.Collections.Generic;
using System.Linq;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Output blocks the target settings require. A transparent surface needs an Alpha block and
    /// alpha clipping needs a threshold, on every target that has those settings. A lit target
    /// reads its normal from the block for its normal space, so a graph that writes a normal into
    /// another space's block gives that target nothing. A graph switched to transparent, or to
    /// another normal space, or given a second target, keeps the block list it already had, so
    /// these are the blocks that go missing.
    ///
    /// This only reports blocks that should be present. Deciding a block is extra needs the
    /// target's full active block list, and only the Editor can produce that.
    /// </summary>
    public static class BlockAdvice
    {
        static readonly string[] s_NormalSpaces = { "tangent", "object", "world" };
        static readonly string[] s_NormalBlocks =
        {
            "SurfaceDescription.NormalTS", "SurfaceDescription.NormalOS", "SurfaceDescription.NormalWS",
        };

        static IEnumerable<(string descriptor, string reason)> Requirements(ShaderGraphDocument doc)
        {
            var present = doc.Nodes.Where(n => n.IsBlock).Select(n => n.BlockDescriptor).ToHashSet();
            foreach (var target in doc.Targets)
            {
                var settings = TargetSettings.Describe(target).ToList();
                if (settings.Contains("surface=transparent"))
                    yield return ("SurfaceDescription.Alpha",
                                  $"the {target.Label} target is transparent but there is no SurfaceDescription.Alpha block to send alpha to");
                if (settings.Contains("alphaclip=on"))
                    yield return ("SurfaceDescription.AlphaClipThreshold",
                                  $"the {target.Label} target clips alpha but there is no SurfaceDescription.AlphaClipThreshold block");

                var space = NormalSpaceIndex(settings);
                if (space < 0)
                    continue;
                var wanted = s_NormalBlocks[space];
                var others = s_NormalBlocks.Where(b => b != wanted && present.Contains(b)).ToList();
                if (others.Count > 0)
                    yield return (wanted,
                                  $"the {target.Label} target has normalspace={s_NormalSpaces[space]} and reads {wanted}, but the graph's " +
                                  $"normal block is {string.Join(" and ", others)}. Set normalspace to match, or add {wanted} and wire it");
            }
        }

        static int NormalSpaceIndex(List<string> settings)
        {
            const string prefix = "normalspace=";
            var setting = settings.FirstOrDefault(s => s.StartsWith(prefix, System.StringComparison.Ordinal));
            return setting == null ? -1 : System.Array.IndexOf(s_NormalSpaces, setting.Substring(prefix.Length));
        }

        public static IEnumerable<string> Required(ShaderGraphDocument doc) =>
            Requirements(doc).Select(r => r.descriptor).Distinct();

        /// <summary>The required blocks the graph does not have, as sentences.</summary>
        public static IEnumerable<string> Missing(ShaderGraphDocument doc)
        {
            var present = doc.Nodes.Where(n => n.IsBlock).Select(n => n.BlockDescriptor).ToHashSet();
            return Requirements(doc).Where(r => !present.Contains(r.descriptor)).Select(r => r.reason);
        }

        public static IEnumerable<string> MissingDescriptors(ShaderGraphDocument doc)
        {
            var present = doc.Nodes.Where(n => n.IsBlock).Select(n => n.BlockDescriptor).ToHashSet();
            return Required(doc).Where(d => !present.Contains(d));
        }
    }
}
