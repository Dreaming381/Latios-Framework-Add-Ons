// Registers the VfxWaitress Editor operations as Pipeline commands, which is how the vfxwaitress
// CLI reaches them through `unity command`. This assembly only compiles when the Pipeline package
// is installed.

using Unity.Pipeline.Commands;

namespace VfxWaitress
{
    static class VfxWaitressCommands
    {
        const string k_Tag = "waitress";

        [CliCommand("vfxwaitress_validate", "Compile a VFX Graph asset and report what VFX Graph finds.", Tags = new[] { k_Tag })]
        static string Validate([CliArg("path", "Asset path of the .vfx", Required = true)] string path) => Validator.Validate(path);

        [CliCommand("vfxwaitress_prepare", "Flush unsaved edits in an open VFX Graph window to disk, before vfxwaitress edits the file.", Tags = new[] { k_Tag })]
        static string Prepare([CliArg("path", "Asset path of the .vfx", Required = true)] string path) => Validator.Prepare(path);

        [CliCommand("vfxwaitress_refresh", "Reimport a VFX Graph asset and rebuild any window showing it.", Tags = new[] { k_Tag })]
        static string Refresh([CliArg("path", "Asset path of the .vfx", Required = true)] string path) => Validator.Refresh(path);

        [CliCommand("vfxwaitress_resave", "Rewrite a VFX Graph asset through VFX Graph's own serializer.", Tags = new[] { k_Tag })]
        static string Resave([CliArg("path", "Asset path of the .vfx", Required = true)] string path) => Validator.Resave(path);

        [CliCommand("vfxwaitress_resync", "Re-derive every node's slots from its settings, then save.", Tags = new[] { k_Tag })]
        static string Resync([CliArg("path", "Asset path of the .vfx", Required = true)] string path) => Validator.Resync(path);

        [CliCommand("vfxwaitress_measure", "Report node sizes and port positions from a live VFX Graph view. Opens a window for the asset.", Tags = new[] { k_Tag })]
        static string Measure([CliArg("path", "Asset path of the .vfx", Required = true)] string path) => Validator.Measure(path);

        [CliCommand("vfxwaitress_resolve_reference", "Report the fileID,guid,type a .vfx stores to reference an asset.", Tags = new[] { k_Tag })]
        static string ResolveReference([CliArg("path", "Asset path of the referenced asset", Required = true)] string path) => Validator.ResolveReference(path);

        [CliCommand("vfxwaitress_add_attribute", "Declare a custom attribute on a VFX Graph, or correct its type.", Tags = new[] { k_Tag })]
        static string AddAttribute(
            [CliArg("path", "Asset path of the .vfx", Required = true)] string path,
            [CliArg("arg", "name,type[,description]", Required = true)] string arg) => Validator.AddAttribute(path, arg);

        [CliCommand("vfxwaitress_export_catalog", "Export this project's VFX Graph catalog for vfxwaitress, to UserSettings/Waitress/catalog.json unless an output path is given. Takes several seconds.", Tags = new[] { k_Tag })]
        static string ExportCatalog([CliArg("output", "Where to write the catalog")] string output = null) =>
            "wrote " + CatalogExporter.Export(string.IsNullOrEmpty(output) ? CatalogExporter.DefaultOutputPath() : output);
    }
}
