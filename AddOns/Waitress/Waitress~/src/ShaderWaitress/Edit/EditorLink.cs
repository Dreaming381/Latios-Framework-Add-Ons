namespace ShaderWaitress.Edit
{
    /// <summary>
    /// The Editor round trips a write makes. An open Shader Graph window keeps its own copy of the
    /// graph, so its unsaved edits are invisible in the file, and a file written behind its back
    /// makes the window ask whether to reload, a dialog that can't be answered while the Editor
    /// runs with -automated. So a write saves the window's unsaved edits first, and afterwards has
    /// the window reload the new file without asking.
    ///
    /// The Editor side is the shaderwaitress_* Pipeline commands the ShaderWaitress Editor Bridge
    /// sample registers. Without an Editor or the sample, everything here becomes a no-op.
    /// </summary>
    public static class EditorLink
    {
        public const string SampleName = "ShaderWaitress Editor Bridge";

        public static readonly EditorBridge Bridge = new EditorBridge("shaderwaitress", SampleName);

        /// <summary>Saves unsaved edits in any window showing the graph. Call before reading a file to edit it.</summary>
        public static string Prepare(string path) => Bridge.Call(path, "Prepare");

        /// <summary>Imports what was written and reloads any window showing it.</summary>
        public static string Refresh(string path) => Bridge.Call(path, "Refresh");
    }
}
