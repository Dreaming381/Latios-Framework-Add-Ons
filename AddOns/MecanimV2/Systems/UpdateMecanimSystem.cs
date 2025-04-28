using Latios.Kinemation;
using Latios.Transforms.Abstract;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Latios.Mecanim
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

            state.Dependency = new ApplyMecanimRootMotionsJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(state.Dependency);
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
            public float DeltaTime;
            
            public void Execute(MecanimAspect mecanimAspect, OptimizedRootDeltaROAspect optimizedRootAspect, LocalTransformQvvsReadWriteAspect localTransform)
            {
                if (!mecanimAspect.applyRootMotion)
                    return;

                var rootBone                  = optimizedRootAspect.rootDelta;
                
                // Reapply delta time scaling to root motion 
                rootBone.position *= DeltaTime;
                // We scale rotation by an additional 100f to revert our 0.01f scale that prevents angle overflow
                rootBone.rotation = MathUtil.ScaleQuaternion(rootBone.rotation, 100f * DeltaTime);
                
                var result                    = RootMotionTools.ConcatenateDeltas(localTransform.localTransform, in rootBone);
                result.rotation               = math.normalize(result.rotation);
                localTransform.localTransform = result;
            }
        }
    }
}

