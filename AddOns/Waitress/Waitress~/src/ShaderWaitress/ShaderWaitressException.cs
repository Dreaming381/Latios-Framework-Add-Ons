using System;

namespace ShaderWaitress
{
    /// <summary>Expected, user-facing failure. Reported as a message, never a stack trace.</summary>
    public sealed class ShaderWaitressException : WaitressException
    {
        /// <summary>
        /// The file is a valid Shader Graph, just in the pre-10.0 format that Unity upgrades on
        /// import. Corpus runs report those separately from real parse failures.
        /// </summary>
        public bool LegacyGraphFormat { get; init; }

        public ShaderWaitressException(string message) : base(message) { }
        public ShaderWaitressException(string message, Exception inner) : base(message, inner) { }
    }
}
