using System;

namespace Waitress
{
    /// <summary>
    /// Expected, user-facing failure. Reported as a message, never a stack trace. Each tool
    /// derives its own.
    /// </summary>
    public class WaitressException : Exception
    {
        public WaitressException(string message) : base(message) { }
        public WaitressException(string message, Exception inner) : base(message, inner) { }
    }
}
