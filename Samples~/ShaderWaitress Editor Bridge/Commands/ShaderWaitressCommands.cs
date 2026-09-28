// Registers the ShaderWaitress Editor operations as Pipeline commands, which is how the
// shaderwaitress CLI reaches them through `unity command`. This assembly only compiles when the
// Pipeline package is installed.

using Unity.Pipeline.Commands;

namespace ShaderWaitress
{
    static class ShaderWaitressCommands
    {
        const string k_Tag = "waitress";

        [CliCommand("shaderwaitress_prepare", "Save unsaved edits in any Shader Graph window showing a graph, before shaderwaitress edits the file.", Tags = new[] { k_Tag })]
        static string Prepare([CliArg("path", "Asset path of the graph", Required = true)] string path) => EditorLink.Prepare(path);

        [CliCommand("shaderwaitress_refresh", "Import a graph shaderwaitress wrote and reload any Shader Graph window showing it, without the changed-on-disk dialog.", Tags = new[] { k_Tag })]
        static string Refresh([CliArg("path", "Asset path of the graph", Required = true)] string path) => EditorLink.Refresh(path);

        [CliCommand("shaderwaitress_compile", "Import and compile a Shader Graph asset and report its errors and warnings.", Tags = new[] { k_Tag })]
        static string Compile([CliArg("path", "Asset path of the graph", Required = true)] string path) => EditorLink.Compile(path);

        [CliCommand("shaderwaitress_export_catalog", "Export this project's Shader Graph node catalog for shaderwaitress, to UserSettings/Waitress/nodes.json unless an output path is given.", Tags = new[] { k_Tag })]
        static string ExportCatalog([CliArg("output", "Where to write the catalog")] string output = null)
        {
            var path = string.IsNullOrEmpty(output) ? CatalogExporter.DefaultOutputPath() : output;
            CatalogExporter.Export(path);
            return "wrote " + path;
        }
    }
}
