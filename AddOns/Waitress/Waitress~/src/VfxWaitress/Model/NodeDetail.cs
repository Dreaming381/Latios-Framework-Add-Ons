using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VfxWaitress.Catalog;

namespace VfxWaitress.Model
{
    /// <summary>
    /// One node in full: settings with their enum values, and the whole slot tree with types,
    /// values, and links. This shows the child slots the one-line form hides.
    /// </summary>
    public static class NodeDetail
    {
        public static string Render(VfxAsset asset, VfxNode node)
        {
            var sb = new StringBuilder();
            sb.Append(node.Kind).Append(' ').Append(node.ShortId).Append(' ').Append(node.DisplayType);
            if (node.Label != null)
                sb.Append(" \"").Append(node.Label).Append('"');
            sb.AppendLine();
            // Every parameter is the same class, so the catalog entry matched by its script is an
            // arbitrary one of dozens. Don't name it.
            if (node.ExposedName != null)
            {
                sb.Append("  type UnityEditor.VFX.VFXParameter carrying ").Append(node.DisplayType)
                  .Append(node.IsOutput ? " (subgraph output)" : " (exposed input)").AppendLine();
            }
            else
            {
                sb.Append("  type ").Append(node.TypeName ?? "?").AppendLine();
                if (node.Model?.Category is { Length: > 0 })
                    sb.Append("  menu ").Append(node.Model.Category).Append('/').Append(node.Model.Name).AppendLine();
            }
            if (node.Context != null)
                sb.Append("  in context ").Append(node.Context.ShortId).AppendLine();
            if (node.Data != null)
                sb.Append("  system ").Append(node.Data.ShortId).Append(' ').Append(node.Data.Title ?? "").AppendLine();

            RenderSettings(sb, node);

            if (node.Inputs.Count > 0)
            {
                sb.AppendLine("  inputs");
                foreach (var slot in node.Inputs)
                    RenderSlot(sb, asset, slot, "    ");
            }
            if (node.Outputs.Count > 0)
            {
                sb.AppendLine("  outputs");
                foreach (var slot in node.Outputs)
                    RenderSlot(sb, asset, slot, "    ");
            }
            return sb.ToString();
        }

        static void RenderSettings(StringBuilder sb, VfxNode node)
        {
            if (node.Model == null || node.Model.Settings.Count == 0)
                return;
            sb.AppendLine("  settings");
            foreach (var setting in node.Model.Settings)
            {
                var raw = VfxAsset.SettingDisplay(node.Doc, setting.Name)?.Trim();
                var shown = raw;
                if (setting.Values.Count > 0 && int.TryParse(raw, out var ordinal) && ordinal >= 0 && ordinal < setting.Values.Count)
                    shown = setting.Values[ordinal];
                sb.Append("    ").Append(setting.Name).Append(" = ").Append(Shorten(shown ?? "<unset>"));
                if (setting.Values.Count > 0)
                    sb.Append("   one of: ").Append(string.Join(", ", setting.Values));
                else if (setting.Default != null)
                    sb.Append("   default: ").Append(Shorten(setting.Default));
                sb.AppendLine();
            }
        }

        static void RenderSlot(StringBuilder sb, VfxAsset asset, VfxSlot slot, string indent)
        {
            sb.Append(indent).Append(string.IsNullOrEmpty(slot.Name) ? "<anon>" : slot.Name);
            sb.Append(" : ").Append(slot.TypeName ?? "?");
            if (slot.Value != null)
                sb.Append(" = ").Append(Shorten(slot.Value));
            foreach (var linked in slot.LinkedSlotIds)
            {
                var other = asset.SlotById(linked);
                sb.Append(slot.IsInput ? "  <- " : "  -> ");
                sb.Append(other == null ? "?" + linked : other.Owner.ShortId + "." + other.Path);
            }
            if (slot.Children.Count > 0)
                sb.Append("   (composite: ").Append(string.Join(", ", slot.Children.Select(c => c.Name))).Append(')');
            sb.AppendLine();
            foreach (var child in slot.Children)
                RenderSlot(sb, asset, child, indent + "  ");
        }

        /// <summary>The whole of one catalog entry, for planning a node before writing it.</summary>
        public static string RenderModel(CatalogModel model)
        {
            var sb = new StringBuilder();
            sb.Append(model.Kind).Append(' ').Append(model.Type).AppendLine();
            sb.Append("  menu ").Append(model.Category).Append('/').Append(model.Name).AppendLine();
            if (model.Synonyms.Count > 0)
                sb.Append("  synonyms ").Append(string.Join(", ", model.Synonyms)).AppendLine();
            if (model.VariantSettings.Count > 0)
                sb.Append("  variant ").Append(string.Join("  ", model.VariantSettings.Select(kv => kv.Key + "=" + kv.Value))).AppendLine();
            if (model.Settings.Count > 0)
            {
                sb.AppendLine("  settings");
                foreach (var s in model.Settings)
                {
                    sb.Append("    ").Append(s.Name).Append(" : ").Append(ShortType(s.Type));
                    if (s.Default != null)
                        sb.Append(" = ").Append(Shorten(s.Default));
                    if (s.Values.Count > 0)
                        sb.Append("   one of: ").Append(string.Join(", ", s.Values));
                    sb.AppendLine();
                }
                sb.AppendLine("  # a setting can reshape the slot list: check the slots again after changing one");
            }
            RenderCatalogSlots(sb, "inputs", model.Inputs);
            RenderCatalogSlots(sb, "outputs", model.Outputs);
            return sb.ToString();
        }

        static void RenderCatalogSlots(StringBuilder sb, string label, List<CatalogSlot> slots)
        {
            if (slots.Count == 0)
                return;
            sb.Append("  ").AppendLine(label);
            foreach (var s in slots)
                RenderCatalogSlot(sb, s, "    ");
        }

        static void RenderCatalogSlot(StringBuilder sb, CatalogSlot slot, string indent)
        {
            sb.Append(indent).Append(string.IsNullOrEmpty(slot.Name) ? "<anon>" : slot.Name)
              .Append(" : ").Append(ShortType(slot.Type)).AppendLine();
            foreach (var c in slot.Children)
                RenderCatalogSlot(sb, c, indent + "  ");
        }

        static string ShortType(string type)
        {
            if (string.IsNullOrEmpty(type))
                return "?";
            var dot = type.LastIndexOf('.');
            return dot >= 0 ? type.Substring(dot + 1) : type;
        }

        static string Shorten(string s)
        {
            s = s.Replace("\n", "\\n").Replace("\r", "");
            return s.Length > 90 ? s.Substring(0, 87) + "..." : s;
        }
    }
}
