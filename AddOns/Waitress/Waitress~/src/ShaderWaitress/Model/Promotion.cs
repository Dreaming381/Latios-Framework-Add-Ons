using System;
using ShaderWaitress.Serialization;

namespace ShaderWaitress.Model
{
    /// <summary>
    /// "Promote to final Shader" on a sub-graph input, which properties and keywords share.
    ///
    /// An ordinary sub-graph property becomes an input port on the Sub Graph node and never
    /// reaches the parent shader. A promoted one is declared on the parent instead, so the
    /// material really has it and <c>Material.HasProperty</c> is true — the node loses the port
    /// as a side effect.
    ///
    /// Shader Graph stores this as the sub-graph's own asset guid rather than a flag
    /// (<c>promoteToFinalShader =&gt; !string.IsNullOrEmpty(promotedFromAssetID)</c>), which is
    /// why only a sub-graph can promote: there is no guid to write otherwise.
    /// </summary>
    public static class Promotion
    {
        const string k_AssetIdKey = "promotedFromAssetID";
        const string k_CategoryKey = "promotedFromCategoryName";
        const string k_OrderingKey = "promotedOrdering";

        public static bool IsPromoted(MultiJsonEntry entry) =>
            !string.IsNullOrEmpty((string)entry.Node[k_AssetIdKey]);

        public static void Set(MultiJsonEntry entry, bool on, string assetGuid)
        {
            var json = entry.Edit();
            json[k_AssetIdKey] = on ? assetGuid : string.Empty;
            // Both travel with the flag and drive the material inspector's foldout. -1 is the
            // "no explicit place" ordering Shader Graph itself writes.
            json[k_CategoryKey] = string.Empty;
            json[k_OrderingKey] = -1;
        }

        /// <summary>
        /// Shader Graph declares promotion on AbstractShaderProperty and refuses it for the
        /// three types that have no material-property representation to promote into.
        /// </summary>
        public static bool CanPromote(string typeLabel) =>
            !string.Equals(typeLabel, "Gradient", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(typeLabel, "SamplerState", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(typeLabel, "VirtualTexture", StringComparison.OrdinalIgnoreCase);
    }
}
