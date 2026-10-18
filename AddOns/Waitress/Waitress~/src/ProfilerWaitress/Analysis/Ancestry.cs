using System;
using System.Collections.Generic;
using System.Text;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Analysis
{
    /// <summary>
    /// Depth-first walk of one block that knows every sample's ancestors. Stack[0] is the thread root,
    /// Stack[depth - 1] the parent.
    /// </summary>
    public sealed unsafe class Ancestry
    {
        public readonly CaptureFile Capture;
        public long[] Stack = new long[64];
        public int Depth;

        public Ancestry(CaptureFile capture)
        {
            Capture = capture;
        }

        public delegate void Visit(long sample, int depth);

        public void Walk(int blockIndex, Visit visit)
        {
            var block = Capture.Blocks[blockIndex];
            var end = block.SampleBase + block.SampleCount;
            for (var s = block.SampleBase; s < end; s++)
            {
                int d = Capture.SampleDepth[s];
                if (d >= Stack.Length)
                    Array.Resize(ref Stack, Math.Max(d + 1, Stack.Length * 2));
                Depth = d;
                visit(s, d);
                Stack[d] = s;
            }
        }

        public long Parent => Depth > 0 ? Stack[Depth - 1] : -1;

        /// <summary>Ancestor names from the outermost non-root down, e.g. "PlayerLoop/Update.ScriptRunBehaviourUpdate".</summary>
        public string Path(int maxSegments = 4, string separator = " > ")
        {
            var sb = new StringBuilder();
            var from = Math.Max(1, Depth - maxSegments);
            if (from > 1)
                sb.Append("..").Append(separator);
            for (var i = from; i < Depth; i++)
            {
                if (i > from)
                    sb.Append(separator);
                sb.Append(Capture.MarkerName(Stack[i]));
            }
            return sb.Length == 0 ? "(thread root)" : sb.ToString();
        }

        public bool HasAncestor(bool[] markers)
        {
            for (var i = 1; i < Depth; i++)
                if (markers[Capture.SampleMarker[Stack[i]]])
                    return true;
            return false;
        }
    }

    /// <summary>Selected blocks of selected frames, in frame order.</summary>
    public static class Blocks
    {
        public static IEnumerable<int> Of(CaptureFile c, int[] frames, bool[] threads)
        {
            foreach (var f in frames)
            {
                var frame = c.Frames[f];
                for (var b = frame.BlockFirst; b < frame.BlockFirst + frame.BlockCount; b++)
                    if (threads[c.Blocks[b].Thread])
                        yield return b;
            }
        }
    }
}
