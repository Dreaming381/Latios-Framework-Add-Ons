using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    public struct Field : INativeDisposable
    {
        public bool IsCreated => Width > 0 && Height > 0;
        public int Width => settings.FieldSize.x;
        public int Height => settings.FieldSize.y;
        public float2 CellSize => settings.CellSize;
        public float2 Pivot => settings.Pivot;

        internal NativeReference<TransformQvvs> Transform;
        internal NativeArray<int> PassabilityMap;
        internal NativeArray<float> DensityMap;
        internal NativeArray<float2> MeanVelocityMap;
        internal NativeArray<int> UnitsCountMap;
        internal NativeArray<TransformQvvs> TransformsMap;
        internal NativeArray<Aabb> AabbMap;

        FieldSettings settings;

        public Field(FieldSettings settings, TransformQvvs transform, AllocatorManager.AllocatorHandle allocator)
        {
            this.settings = settings;

            Transform = new NativeReference<TransformQvvs>(transform, allocator);
            var length = settings.FieldSize.x * settings.FieldSize.y;
            DensityMap = CollectionHelper.CreateNativeArray<float>(length, allocator);
            PassabilityMap = CollectionHelper.CreateNativeArray<int>(length, allocator);
            MeanVelocityMap = CollectionHelper.CreateNativeArray<float2>(length, allocator);
            TransformsMap = CollectionHelper.CreateNativeArray<TransformQvvs>(length, allocator, NativeArrayOptions.UninitializedMemory);
            AabbMap = CollectionHelper.CreateNativeArray<Aabb>(length, allocator, NativeArrayOptions.UninitializedMemory);
            UnitsCountMap = CollectionHelper.CreateNativeArray<int>(length, allocator);
        }

        public void Dispose()
        {
            settings = default;
            Transform.Dispose();
            DensityMap.Dispose();
            MeanVelocityMap.Dispose();
            PassabilityMap.Dispose();
            UnitsCountMap.Dispose();
            TransformsMap.Dispose();
            AabbMap.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            settings = default;
            return CollectionsExtensions.CombineDependencies(stackalloc JobHandle[]
            {
                Transform.Dispose(inputDeps),
                DensityMap.Dispose(inputDeps),
                MeanVelocityMap.Dispose(inputDeps),
                PassabilityMap.Dispose(inputDeps),
                UnitsCountMap.Dispose(inputDeps),
                TransformsMap.Dispose(inputDeps),
                AabbMap.Dispose(inputDeps),
            });
        }
    }
}