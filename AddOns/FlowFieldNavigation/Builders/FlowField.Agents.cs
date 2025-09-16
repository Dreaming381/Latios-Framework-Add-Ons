using Latios;
using Latios.Transforms.Abstract;
using Unity.Entities;
using Unity.Jobs;

namespace Latios.FlowFieldNavigation
{
    /// <summary>
    /// Configuration structure for controlling agent movement directions within a flow field.
    /// </summary>
    public struct AgentDirectionsConfig
    {
        internal EntityQuery AgentsQuery;
        internal FlowFieldAgentsTypeHandles AgentTypeHandles;
        internal float DeltaTime;
    }
    
    public static partial class FlowField
    {
        /// <summary>
        /// Configures an EntityQuery to include required components for flow field agents.
        /// </summary>
        /// <param name="fluent">The query builder to modify</param>
        /// <returns>Modified query with required agent components</returns>
        /// <remarks>
        /// Adds these required components:
        /// - WorldTransform (read-only)
        /// - AgentDirection (required for movement direction)
        /// - AgentFootprint (required for density calculations)
        /// - PrevPosition (required for movement history)
        /// - Velocity (required for current movement state)
        /// </remarks>
        public static FluentQuery PatchQueryForFlowFieldAgents(this FluentQuery fluent)
        {
            return fluent.WithWorldTransformReadOnly().With<AgentDirection>().With<PrevPosition>().With<Velocity>().With<AgentFootprint>().With<AgentDensity>();
        }

        /// <summary>
        /// Creates configuration for updating agent directions based on flow field navigation.
        /// </summary>
        /// <param name="agentsQuery">Query to find agents to update</param>
        /// <param name="handles">Type handles for agent components</param>
        /// <param name="deltaTime">Time step for movement calculations</param>
        /// <returns>New agent directions configuration</returns>
        public static AgentDirectionsConfig AgentsDirections(EntityQuery agentsQuery, in FlowFieldAgentsTypeHandles handles, float deltaTime) =>
            new() { AgentsQuery = agentsQuery, AgentTypeHandles = handles, DeltaTime = deltaTime };
        
        
        #region Schedulers
        
        /// <summary>
        /// Schedules parallel jobs to update agent movement directions based on flow field.
        /// </summary>
        /// <param name="config">Direction configuration</param>
        /// <param name="field">Current navigation field</param>
        /// <param name="flow">Flow field data</param>
        /// <param name="inputDeps">Optional input job dependencies</param>
        /// <returns>JobHandle representing the scheduled jobs</returns>
        /// <remarks>
        /// This job:
        /// 1. Calculates desired movement direction for each agent based on flow field.
        /// 2. Updates agent direction component.
        /// </remarks>
        public static JobHandle ScheduleParallel(this AgentDirectionsConfig config, in Field field, in Flow flow, JobHandle inputDeps = default)
        {
            var dependency = inputDeps;

            dependency = new FlowFieldInternal.CalculateAgentsDirectionsJob
            {
                Flow = flow,
                Field = field,
                TypeHandles = config.AgentTypeHandles, DeltaTime = config.DeltaTime
            }.ScheduleParallel(config.AgentsQuery, dependency);
            return dependency;
        }
        
        /// <summary>
        /// Schedules single-threaded jobs to update agent movement directions based on flow field.
        /// </summary>
        /// <param name="config">Direction configuration</param>
        /// <param name="field">Current navigation field</param>
        /// <param name="flow">Current navigation flow</param>
        /// <param name="inputDeps">Optional input job dependencies</param>
        /// <returns>JobHandle representing the scheduled jobs</returns>
        /// <remarks>
        /// This job:
        /// 1. Calculates desired movement direction for each agent based on flow field.
        /// 2. Updates agent direction component.
        /// </remarks>
        public static JobHandle Schedule(this AgentDirectionsConfig config, in Field field, in Flow flow, JobHandle inputDeps = default)
        {
            var dependency = inputDeps;

            dependency = new FlowFieldInternal.CalculateAgentsDirectionsJob
            {
                Flow = flow,
                Field = field,
                TypeHandles = config.AgentTypeHandles, DeltaTime = config.DeltaTime
            }.Schedule(config.AgentsQuery, dependency);
            return dependency;
        }
        
        #endregion
    }
}