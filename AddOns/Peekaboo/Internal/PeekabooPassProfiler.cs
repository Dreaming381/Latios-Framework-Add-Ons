using Unity.Burst;
using Unity.Jobs;
using Unity.Profiling.LowLevel.Unsafe;

namespace Latios.Peekaboo
{
    /// <summary>
    /// Adds up how long each stage of a culling pass takes. It's off unless a measurement tool turns
    /// it on, and costs one branch per stage when off.
    /// </summary>
    /// <remarks>
    /// Each stage is timed by completing it before the next one gets scheduled. That's the only way to
    /// tell stages apart, but it also stops them from overlapping. So each number is accurate for its
    /// stage, but the total is longer than what the pass costs when left alone.
    /// </remarks>
    internal static unsafe class PeekabooPassProfiler
    {
        public const int kGather      = 0;
        public const int kSelect      = 1;
        public const int kBuildPlates = 2;
        public const int kRasterize   = 3;
        public const int kCull        = 4;
        public const int kStats       = 5;
        public const int kUpstream    = 6;
        public const int kWholePass   = 7;
        public const int kStageCount  = 8;

        internal struct Data
        {
            public int        enabled;
            public int        passes;
            public int        lightPasses;
            public fixed long ticks[kStageCount];
        }

        // Burst keys a shared static by its two context types and its size, in native memory that
        // survives domain reloads. If Data changes size and this name doesn't, the type initializer
        // throws until the editor restarts. Rename this whenever Data changes size.
        struct StageTable8
        {
        }

        static readonly SharedStatic<Data> s_data = SharedStatic<Data>.GetOrCreate<Data, StageTable8>();

        /// <summary>
        /// Allocates the shared storage. Burst code doesn't run static constructors, so this has to be
        /// called from managed code before a Bursted pass reaches Sample.
        /// </summary>
        public static void Initialize() => s_data.Data.enabled = 0;

        /// <summary>
        /// 0 is off, 1 is per stage, and 2 is whole pass. Per stage completes each job before the next
        /// one is scheduled, so the total overstates the pass. Whole pass leaves the jobs alone and
        /// completes once at the end, which shows what the pass really costs.
        /// </summary>
        public static int mode
        {
            get => s_data.Data.enabled;
            set => s_data.Data.enabled = value;
        }

        public static bool enabled
        {
            get => s_data.Data.enabled != 0;
            set => s_data.Data.enabled = value ? 1 : 0;
        }

        public static int passes => s_data.Data.passes;
        public static int lightPasses => s_data.Data.lightPasses;

        /// <summary>
        /// Completes the stage and adds the wait to its time. Does nothing unless per stage mode is on,
        /// so the pass stays fully pipelined.
        /// </summary>
        public static void Sample(int stage, ref JobHandle handle)
        {
            ref var data = ref s_data.Data;
            if (data.enabled != 1)
                return;
            long start = ProfilerUnsafeUtility.Timestamp;
            handle.Complete();
            data.ticks[stage] += ProfilerUnsafeUtility.Timestamp - start;
        }

        /// <summary>
        /// The timestamp a whole pass measurement starts from, or zero if that mode is off.
        /// </summary>
        public static long BeginPass() => s_data.Data.enabled == 2 ? ProfilerUnsafeUtility.Timestamp : 0L;

        public static void EndPass(bool isLightView, ref JobHandle handle, long start)
        {
            ref var data = ref s_data.Data;
            if (data.enabled == 0)
                return;
            if (data.enabled == 2)
            {
                handle.Complete();
                data.ticks[kWholePass] += ProfilerUnsafeUtility.Timestamp - start;
            }
            data.passes++;
            if (isLightView)
                data.lightPasses++;
        }

        public static void Reset()
        {
            ref var data     = ref s_data.Data;
            data.passes      = 0;
            data.lightPasses = 0;
            for (int i = 0; i < kStageCount; i++)
                data.ticks[i] = 0;
        }

        public static double MillisecondsPerPass(int stage)
        {
            ref var data = ref s_data.Data;
            if (data.passes == 0)
                return 0.0;
            var ratio = ProfilerUnsafeUtility.TimestampToNanosecondsConversionRatio;
            return data.ticks[stage] * (double)ratio.Numerator / ratio.Denominator / 1e6 / data.passes;
        }

        public static string StageName(int stage)
        {
            switch (stage)
            {
                case kGather: return "gather";
                case kSelect: return "select";
                case kBuildPlates: return "build, bin";
                case kRasterize: return "raster, pyramid";
                case kCull: return "cull";
                case kStats: return "stats";
                case kUpstream: return "(upstream wait)";
                case kWholePass: return "WHOLE PASS";
                default: return "?";
            }
        }
    }
}
