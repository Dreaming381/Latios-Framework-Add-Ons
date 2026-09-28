using System;

namespace VfxWaitress
{
    /// <summary>Expected, user-facing failure. Reported as a message, never a stack trace.</summary>
    public sealed class VfxWaitressException : WaitressException
    {
        public VfxWaitressException(string message) : base(message) { }
        public VfxWaitressException(string message, Exception inner) : base(message, inner) { }
    }
}
