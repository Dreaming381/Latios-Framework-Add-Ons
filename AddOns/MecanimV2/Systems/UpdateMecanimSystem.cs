using Latios.Kinemation;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Latios.MecanimV2
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct UpdateMecanimSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new UpdateMecanimJob
            {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                DeltaTime   = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new ApplyMecanimRootMotionsJob().ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct UpdateMecanimJob : IJobEntity
        {
            public double ElapsedTime;
            public float  DeltaTime;

            public void Execute(MecanimAspect mecanimAspect, OptimizedSkeletonAspect optimizedSkeletonAspect)
            {
                mecanimAspect.Update(optimizedSkeletonAspect, ElapsedTime, DeltaTime);
            }
        }

        [BurstCompile]
        public partial struct ApplyMecanimRootMotionsJob : IJobEntity
        {
            public void Execute(MecanimAspect mecanimAspect, OptimizedRootDeltaROAspect optimizedRootAspect, LocalTransformQvvsReadWriteAspect localTransform)
            {
                if (!mecanimAspect.applyRootMotion)
                    return;

                var rootBone                  = optimizedRootAspect.rootDelta;
                localTransform.localTransform = RootMotionTools.ConcatenateDeltas(in rootBone, localTransform.localTransform);
            }
        }
    }
}

