using System;
using System.Globalization;

namespace VfxWaitress.Edit
{
    /// <summary>
    /// The Editor round trips a write makes. Reading stays offline. But an open VFX Graph window
    /// keeps its own copy of the graph, so its unsaved edits are invisible in the file, and a file
    /// written behind its back gets overwritten when the window saves. So a write flushes the
    /// Editor first and hands the result back afterwards.
    ///
    /// The Editor side is the vfxwaitress_* Pipeline commands the VfxWaitress Editor Bridge sample
    /// registers. Without an Editor or the sample, everything here becomes a no-op.
    /// </summary>
    public static class EditorLink
    {
        public const string SampleName = "VfxWaitress Editor Bridge";

        public static readonly EditorBridge Bridge = new EditorBridge("vfxwaitress", SampleName);

        /// <summary>Set by --offline. Skips every round trip, at the cost of estimated node sizes.</summary>
        public static bool Offline
        {
            get => Bridge.Offline;
            set => Bridge.Offline = value;
        }

        public static bool Available => Bridge.Available;

        /// <summary>Flushes unsaved graph state to disk. Call before reading a file to edit it.</summary>
        public static string Prepare(string path) => Bridge.Call(path, "Prepare");

        /// <summary>Re-imports what was written and rebuilds any open window from it.</summary>
        public static string Refresh(string path) => Bridge.Call(path, "Refresh");

        /// <summary>
        /// Hands the file back to VFX Graph and has it write the asset itself. Several fields are
        /// derived from the graph view, so only the Editor's own bytes guarantee that opening the
        /// graph doesn't immediately mark it dirty.
        /// </summary>
        public static string Settle(string path)
        {
            // Twice on purpose. The first write settles the graph, and the second settles what the
            // graph view derives from it. A third changes nothing.
            var reply = Bridge.Call(path, "Resave");
            return reply == null ? null : Bridge.Call(path, "Resave") ?? reply;
        }

        /// <summary>
        /// Real node boxes and port positions from the graph view. The rects only exist after the
        /// panel runs a layout pass, which happens between calls, so an empty first answer is
        /// normal.
        /// </summary>
        public static Measurements Sizes(string path)
        {
            var measured = new Measurements();
            for (var attempt = 0; attempt < 3 && measured.IsEmpty; attempt++)
            {
                var reply = Bridge.Call(path, "Measure");
                if (reply == null || reply.StartsWith("error", StringComparison.Ordinal))
                    return measured;
                foreach (var line in reply.Split('\n'))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 4 || !Id(parts[1], out var id))
                        continue;
                    if (parts[0] == "n" && Number(parts[2], out var w) && Number(parts[3], out var h) && w >= 1f && h >= 1f)
                        measured.Boxes[id] = (w, h);
                    else if (parts[0] == "p" && Number(parts[3], out var y))
                        measured.PortY[id] = y;
                    else if (parts[0] == "q" && Number(parts[3], out y))
                        measured.OutPortY[id] = y;
                }
            }
            return measured;
        }

        static bool Id(string text, out long id) =>
            long.TryParse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out id);

        /// <summary>A rect the panel has not laid out yet parses fine and is NaN.</summary>
        static bool Number(string text, out float value) =>
            float.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value) && value >= 0f;
    }
}
