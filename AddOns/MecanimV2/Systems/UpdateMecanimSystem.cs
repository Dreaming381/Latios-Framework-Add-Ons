using System;
using Latios.MecanimV2;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Latios.Mecanim.AddOns.MecanimV2.Systems
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
            _mecanimQuery = state.Fluent()
                .With<MecanimController>(false)
                .With<MecanimStateMachineActiveStates>(false)
                .With<LayerWeights>(true)
                .With<MecanimParameter>(false).Build();
            
            state.RequireForUpdate(_mecanimQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new UpdateStateMachinesJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(_mecanimQuery, state.Dependency);
        }

        [BurstCompile]
        public partial struct UpdateStateMachinesJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref MecanimController mecanimController, DynamicBuffer<MecanimStateMachineActiveStates> stateMachineActiveStates, DynamicBuffer<LayerWeights> layerWeights, DynamicBuffer<MecanimParameter> parameters)
            {
                Span<BitField64> localTriggersToReset = stackalloc BitField64[parameters.Length / 64 + 1];
                Span<StateMachineEvaluation.StatePassage> statePassages = stackalloc StateMachineEvaluation.StatePassage[8];
                
                for (var i = 0; i < stateMachineActiveStates.Length; i++)
                {
                    var stateMachineActiveState = stateMachineActiveStates[i];
                    
                    StateMachineEvaluation.Evaluate(
                        ref stateMachineActiveState,
                        ref mecanimController.controllerBlob.Value,
                        ref mecanimController.skeletonClipsBlob.Value,
                        i,
                        DeltaTime,
                        layerWeights.AsNativeArray().AsReadOnlySpan(),
                        parameters.AsNativeArray().AsReadOnlySpan(),
                        localTriggersToReset,
                        statePassages,
                        out int passagesCount,
                        out float newInertialBlendProgressRealtime,
                        out float newInertialBlendDurationRealtime);

                    stateMachineActiveStates[i] = stateMachineActiveState;
                }
            }
        }
    }
}
