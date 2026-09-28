using System.Runtime.CompilerServices;
using Unity.Profiling;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Profiler markers for the phases inside the rasterizer. They only compile in with the scripting
    /// define PEEKABOO_PROFILE_RASTER, since the rasterizer runs them several times per row and they
    /// cost real time in the editor even when the profiler isn't recording.
    /// </summary>
    internal static class PeekabooProfiling
    {
        /// <summary>
        /// Begins a marker that ends when the scope is disposed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Scope Auto(in ProfilerMarker marker)
        {
#if PEEKABOO_PROFILE_RASTER
            return new Scope(marker);
#else
            return default;
#endif
        }

        public struct Scope : System.IDisposable
        {
#if PEEKABOO_PROFILE_RASTER
            ProfilerMarker m_marker;

            public Scope(ProfilerMarker marker)
            {
                m_marker = marker;
                m_marker.Begin();
            }
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
#if PEEKABOO_PROFILE_RASTER
                m_marker.End();
#endif
            }
        }
    }
}
