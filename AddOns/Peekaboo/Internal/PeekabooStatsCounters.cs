#if (UNITY_EDITOR || DEVELOPMENT_BUILD || PEEKABOO_ENABLE_STATS) && !PEEKABOO_DISABLE_STATS
#define PEEKABOO_STATS
#endif

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Per-worker counters that feed <see cref="PeekabooStats"/>. Stats are on in the editor and in
    /// development builds, and PEEKABOO_ENABLE_STATS turns them on in release builds too.
    /// PEEKABOO_DISABLE_STATS turns them off everywhere. When they're off, every method is empty and
    /// nothing gets allocated, so the counting compiles out of the jobs that use it.
    /// </summary>
    /// <remarks>
    /// This is the only place stats code gets switched on and off, so the jobs never need an #if in
    /// the middle of their algorithms. Each worker gets its own cache line, so counting never makes
    /// two workers fight over one.
    /// </remarks>
    internal unsafe struct PeekabooStatsCounters
    {
        const int kStride     = 16;
        const int kConsidered = 0;
        const int kCulled     = 1;
        const int kSurviving  = 2;

        [NativeDisableUnsafePtrRestriction] int* m_counts;
        int                                      m_threadCount;

        /// <summary>
        /// Whether stats are compiled in. When they aren't, the pass skips the job that writes them.
        /// </summary>
        public static bool isEnabled
        {
            get
            {
#if PEEKABOO_STATS
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Allocates zeroed counters for every worker, from an allocator that frees itself, like the
        /// world update allocator.
        /// </summary>
        public static PeekabooStatsCounters Create(int threadCount, AllocatorManager.AllocatorHandle allocator)
        {
            var counters = new PeekabooStatsCounters();
#if PEEKABOO_STATS
            counters.m_threadCount = threadCount;
            counters.m_counts      = AllocatorManager.Allocate<int>(allocator, threadCount * kStride);
            UnsafeUtility.MemClear(counters.m_counts, threadCount * kStride * sizeof(int));
#endif
            return counters;
        }

        /// <summary>
        /// Zeroes every worker's counts.
        /// </summary>
        public void Clear()
        {
#if PEEKABOO_STATS
            UnsafeUtility.MemClear(m_counts, m_threadCount * kStride * sizeof(int));
#endif
        }

        /// <summary>
        /// Frees counters made with an allocator that doesn't free itself.
        /// </summary>
        public void Dispose(AllocatorManager.AllocatorHandle allocator)
        {
#if PEEKABOO_STATS
            AllocatorManager.Free(allocator, m_counts, m_threadCount * kStride);
            m_counts = null;
#endif
        }

        /// <summary>
        /// Counts one entity and view pair that competed for an occluder slot.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CountConsidered(int threadIndex)
        {
#if PEEKABOO_STATS
            m_counts[threadIndex * kStride + kConsidered]++;
#endif
        }

        /// <summary>
        /// Adds a worker's culled and surviving entities.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CountCulled(int threadIndex, int culled, int surviving)
        {
#if PEEKABOO_STATS
            m_counts[threadIndex * kStride + kCulled]    += culled;
            m_counts[threadIndex * kStride + kSurviving] += surviving;
#endif
        }

        /// <summary>
        /// Adds a <see cref="CullTally"/> to a worker's counts.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CountCulled(int threadIndex, in CullTally tally)
        {
#if PEEKABOO_STATS
            CountCulled(threadIndex, tally.culled, tally.surviving);
#endif
        }

        /// <summary>
        /// Adds every worker's counts to the stats.
        /// </summary>
        public void AddTo(ref PeekabooStats stats)
        {
#if PEEKABOO_STATS
            for (int t = 0; t < m_threadCount; t++)
            {
                stats.consideredCount += m_counts[t * kStride + kConsidered];
                stats.culledCount     += m_counts[t * kStride + kCulled];
                stats.survivingCount  += m_counts[t * kStride + kSurviving];
            }
#endif
        }
    }

    /// <summary>
    /// A running count of culled and surviving entities inside one cull job work item. Empty when
    /// stats are compiled out, so the counting disappears from the inner loops.
    /// </summary>
    internal struct CullTally
    {
#if PEEKABOO_STATS
        int m_culled;
        int m_surviving;
#endif

        public int culled
        {
            get
            {
#if PEEKABOO_STATS
                return m_culled;
#else
                return 0;
#endif
            }
        }

        public int surviving
        {
            get
            {
#if PEEKABOO_STATS
                return m_surviving;
#else
                return 0;
#endif
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Culled(int count = 1)
        {
#if PEEKABOO_STATS
            m_culled += count;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Survived(int count = 1)
        {
#if PEEKABOO_STATS
            m_surviving += count;
#endif
        }
    }
}
