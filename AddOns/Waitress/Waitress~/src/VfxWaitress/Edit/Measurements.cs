using System.Collections.Generic;

namespace VfxWaitress.Edit
{
    /// <summary>
    /// Node sizes and port positions measured off a live graph view. The .vfx file stores
    /// neither, and a context's ports can be spread over hundreds of pixels, so guessing aims
    /// wires at the wrong row.
    /// </summary>
    public sealed class Measurements
    {
        /// <summary>Node box, keyed by the file id of the model the layout places.</summary>
        public readonly Dictionary<long, (float w, float h)> Boxes = new Dictionary<long, (float w, float h)>();

        /// <summary>
        /// Where an input port is drawn, measured down from the top of the element the layout
        /// moves — so a block's ports are given relative to its context, not to the block.
        /// </summary>
        public readonly Dictionary<long, float> PortY = new Dictionary<long, float>();

        /// <summary>Where an output port is drawn, measured the same way.</summary>
        public readonly Dictionary<long, float> OutPortY = new Dictionary<long, float>();

        public bool IsEmpty => Boxes.Count == 0;
    }
}
