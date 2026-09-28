using System.Collections.Generic;
using System.Linq;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// Output blocks the target settings require. A transparent surface needs an Alpha block and
    /// alpha clipping needs a threshold, on every target that has those settings. A graph
    /// switched to transparent keeps the block list it was created with, so these are the
    /// blocks that go missing.
    ///
    /// This only reports blocks that should be present. Deciding a block is extra needs the
    /// target's full active block list, and only the Editor can produce that.
    /// </summary>
    public static class BlockAdvice
    {
        public static IEnumerable<string> Required(ShaderGraphDocument doc)
        {
            if (doc.Targets.Count == 0)
                yield break;
            var settings = doc.Targets.SelectMany(TargetSettings.Describe).ToList();
            if (settings.Contains("surface=transparent"))
                yield return "SurfaceDescription.Alpha";
            if (settings.Contains("alphaclip=on"))
                yield return "SurfaceDescription.AlphaClipThreshold";
        }

        /// <summary>The required blocks the graph does not have, as sentences.</summary>
        public static IEnumerable<string> Missing(ShaderGraphDocument doc)
        {
            var present = doc.Nodes.Where(n => n.IsBlock).Select(n => n.BlockDescriptor).ToHashSet();
            foreach (var descriptor in Required(doc))
            {
                if (present.Contains(descriptor))
                    continue;
                yield return descriptor == "SurfaceDescription.Alpha"
                    ? "the target is transparent but there is no SurfaceDescription.Alpha block to send alpha to"
                    : "alpha clipping is on but there is no SurfaceDescription.AlphaClipThreshold block";
            }
        }

        public static IEnumerable<string> MissingDescriptors(ShaderGraphDocument doc)
        {
            var present = doc.Nodes.Where(n => n.IsBlock).Select(n => n.BlockDescriptor).ToHashSet();
            return Required(doc).Where(d => !present.Contains(d));
        }
    }
}
