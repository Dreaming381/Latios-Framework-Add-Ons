using Latios;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    [BurstCompile]
    public struct Flow : INativeDisposable
    {
        public bool IsCreated {get; private set;}

        internal NativeReference<TransformQvvs> Transform;
        internal NativeHashSet<int2> GoalCells;
        internal NativeArray<float> Costs;
        internal NativeArray<float2> DirectionMap;
        
        

        internal FlowSettings Settings;

        public Flow(in Field field, FlowSettings settings, AllocatorManager.AllocatorHandle allocator)
        {
            this.Settings = settings;
            Transform = field.Transform;
            var length = field.Width * field.Height;
            Costs = CollectionHelper.CreateNativeArray<float>(length, allocator);
            DirectionMap = CollectionHelper.CreateNativeArray<float2>(length, allocator);
            GoalCells = new NativeHashSet<int2>(FlowSettings.MaxFootprintSize, allocator);
            IsCreated = true;
        }

        public void Dispose()
        {
            IsCreated = false;
            Settings = default;
            if (Costs.IsCreated)
                Costs.Dispose();
            if (GoalCells.IsCreated)
                GoalCells.Dispose();
            if (DirectionMap.IsCreated)
                DirectionMap.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            IsCreated = false;
            Settings = default;
            return CollectionsExtensions.CombineDependencies(stackalloc JobHandle[]
            {
                Costs.Dispose(inputDeps),
                GoalCells.Dispose(inputDeps),
                DirectionMap.Dispose(inputDeps),
            });
        }
    }
}