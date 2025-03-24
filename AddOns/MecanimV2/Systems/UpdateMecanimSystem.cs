using Latios.Kinemation;
using Unity.Burst;
using Unity.Entities;

namespace Latios.MecanimV2
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct UpdateMecanimSystem : ISystem
    {
        private EntityQuery _mecanimQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _mecanimQuery = state.Fluent().WithAspect<MecanimAspect>().WithAspect<OptimizedSkeletonAspect>().Build();
            state.RequireForUpdate(_mecanimQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new UpdateMecanimJob
            {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(_mecanimQuery, state.Dependency);
        }

        [BurstCompile]
        public partial struct UpdateMecanimJob : IJobEntity
        {
            public double ElapsedTime;
            public float DeltaTime;

            public void Execute(MecanimAspect mecanimAspect, OptimizedSkeletonAspect optimizedSkeletonAspect)
            {
                mecanimAspect.Update(optimizedSkeletonAspect, ElapsedTime, DeltaTime);
            }
        }
    }
}
