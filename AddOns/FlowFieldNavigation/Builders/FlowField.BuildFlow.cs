using System;
using System.Diagnostics;
using Latios.Transforms.Abstract;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Exposed;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.FlowFieldNavigation
{
    /// <summary>
    /// Configuration structure for building a navigation flow.
    /// </summary>
    public struct BuildFlowConfig
    {
        internal FlowSettings FlowSettings;
        internal Field Field;
        internal EntityQuery GoalsQuery;
        internal FlowGoalTypeHandles TypeHandles;
    }

    /// <summary>
    /// Configuration structure for updating an existing flow.
    /// </summary>
    public struct UpdateFlowConfig { }

    public static partial class FlowField
    {
        /// <summary>
        /// Configures an EntityQuery to include required components for flow goals.
        /// </summary>
        /// <param name="fluent">The query builder to modify</param>
        /// <returns>Modified query with required goal components</returns>
        public static FluentQuery PatchQueryForBuildingFlowGoals(this FluentQuery fluent)
        {
            return fluent.WithWorldTransformReadOnly().With<Goal>();
        }
        
        /// <summary>
        /// Creates a new default flow field configuration.
        /// </summary>
        /// <param name="field">Existing navigation field to build upon</param>
        /// <param name="goalsQuery">Query to find goals</param>
        /// <param name="requiredTypeHandles">Type handles for goal components</param>
        /// <returns>New flow configuration</returns>
        public static BuildFlowConfig BuildFlow(in Field field, EntityQuery goalsQuery, in FlowGoalTypeHandles requiredTypeHandles) =>
            new() { Field = field, FlowSettings = FlowSettings.Default, GoalsQuery = goalsQuery, TypeHandles = requiredTypeHandles };
       
        /// <summary>
        /// Creates configuration for updating an existing flow.
        /// Use this when field and goals haven't changed but agents have moved.
        /// </summary>
        /// <returns>New update configuration</returns>
        public static UpdateFlowConfig UpdateFlowDirections() => new();

        #region FluentChain
        
        /// <summary>
        /// Sets custom flow settings for the configuration.
        /// </summary>
        /// <param name="config">Configuration to modify</param>
        /// <param name="settings">New flow settings</param>
        /// <returns>Modified configuration</returns>
        public static BuildFlowConfig WithSettings(this BuildFlowConfig config, FlowSettings settings)
        {
            config.FlowSettings = settings;
            return config;
        }

        #endregion

        #region Schedulers

        /// <summary>
        /// Schedules parallel jobs to build a new navigation flow.
        /// </summary>
        /// <param name="config">Flow configuration</param>
        /// <param name="flow">Output flow that will be created</param>
        /// <param name="allocator">Memory allocator for flow data</param>
        /// <param name="inputDeps">Optional input job dependencies</param>
        /// <returns>JobHandle representing all scheduled jobs</returns>
        /// <remarks>
        /// This method schedules three phases of work:
        /// 1. Collects goal positions from entities.
        /// 2. Calculates costs (single-threaded).
        /// 3. Generates direction vectors from costs.
        /// The resulting flow can be used to guide agent movement.
        /// </remarks>
        public static JobHandle ScheduleParallel(this BuildFlowConfig config, out Flow flow, AllocatorManager.AllocatorHandle allocator, JobHandle inputDeps = default)
        {
            config.ValidateSettings();

            flow = new Flow(config.Field, config.FlowSettings, allocator);

            var dependency = inputDeps;

            dependency = new FlowFieldInternal.CollectGoalsJob
            {
                Field = config.Field,
                GoalCells = flow.GoalCells,
                TypeHandles = config.TypeHandles
            }.Schedule(config.GoalsQuery, dependency);
            
            dependency = new FlowFieldInternal.ResetJob
            {
                Costs = flow.Costs,
                GoalCells = flow.GoalCells,
                Width = config.Field.Width
            }.Schedule(dependency);
            
            dependency = new FlowFieldInternal.CalculateCostsWavefrontJob
            {
                PassabilityMap = config.Field.PassabilityMap,
                Width = config.Field.Width, Height = config.Field.Height,
                Costs = flow.Costs, 
                GoalCells = flow.GoalCells,
            }.Schedule(dependency);
            
            dependency = new FlowFieldInternal.CalculateDirectionJob
            {
                Settings = config.FlowSettings,
                DirectionMap = flow.DirectionMap,
                CostField = flow.Costs,
                DensityField = config.Field.DensityMap,
                Width = config.Field.Width,
                Height = config.Field.Height,
            }.ScheduleParallel(flow.DirectionMap.Length, 32, dependency);

            return dependency;
        }

        /// <summary>
        /// Schedules parallel jobs to update an existing flow's directions.
        /// </summary>
        /// <param name="config">Update configuration</param>
        /// <param name="field">Current navigation field state</param>
        /// <param name="flow">Existing flow to update</param>
        /// <param name="inputDeps">Optional input job dependencies</param>
        /// <returns>JobHandle representing the scheduled jobs</returns>
        /// <remarks>
        /// This efficiently updates just the direction vectors by:
        /// 1. Recalculating flow directions from existing costs.
        /// 2. Considers current density field for obstacle avoidance.
        /// </remarks>
        public static JobHandle ScheduleParallel(this UpdateFlowConfig config, in Field field, in Flow flow, JobHandle inputDeps = default)
        {
            var dependency = inputDeps;
            
            dependency = new FlowFieldInternal.CalculateDirectionJob
            {
                Settings = flow.Settings,
                DirectionMap = flow.DirectionMap,
                CostField = flow.Costs,
                DensityField = field.DensityMap,
                Width = field.Width,
                Height = field.Height,
            }.ScheduleParallel(flow.DirectionMap.Length, 32, dependency);

            return dependency;
        }
        
        /// <summary>
        /// Schedules single-threaded jobs to build a new navigation flow.
        /// </summary>
        /// <param name="config">Flow configuration</param>
        /// <param name="flow">Output flow that will be created</param>
        /// <param name="allocator">Memory allocator for flow data</param>
        /// <param name="inputDeps">Optional input job dependencies</param>
        /// <returns>JobHandle representing all scheduled jobs</returns>
        /// <remarks>
        /// This method schedules three phases of work:
        /// 1. Collects goal positions from entities.
        /// 2. Calculates costs.
        /// 3. Generates direction vectors from costs.
        /// The resulting flow can be used to guide agent movement.
        /// </remarks>
        public static JobHandle Schedule(this BuildFlowConfig config, out Flow flow, AllocatorManager.AllocatorHandle allocator, JobHandle inputDeps = default)
        {
            config.ValidateSettings();

            flow = new Flow(config.Field, config.FlowSettings, allocator);

            var dependency = inputDeps;

            // var count = (config.GoalsQuery.HasFilter() || config.GoalsQuery.UsesEnabledFiltering())
            //     ? config.GoalsQuery.CalculateEntityCount()
            //     : config.GoalsQuery.CalculateEntityCountWithoutFiltering();
            // flow.GoalCells.SetCapacity(count * FlowSettings.MaxFootprintSize * FlowSettings.MaxFootprintSize);

            dependency = new FlowFieldInternal.CollectGoalsJob
            {
                Field = config.Field,
                GoalCells = flow.GoalCells,
                TypeHandles = config.TypeHandles
            }.Schedule(config.GoalsQuery, dependency);

            dependency = new FlowFieldInternal.CalculateCostsWavefrontJob
            {
                PassabilityMap = config.Field.PassabilityMap,
                Width = config.Field.Width, Height = config.Field.Height,
                Costs = flow.Costs,
                GoalCells = flow.GoalCells
            }.Schedule(dependency);
            
            dependency = new FlowFieldInternal.CalculateDirectionJob
            {
                Settings = config.FlowSettings,
                DirectionMap = flow.DirectionMap,
                CostField = flow.Costs,
                Width = config.Field.Width,
                Height = config.Field.Height,
            }.Schedule(flow.DirectionMap.Length, dependency);

            return dependency;
        }
        
        /// <summary>
        /// Schedules single-threaded jobs to update an existing flow's directions.
        /// </summary>
        /// <param name="config">Update configuration</param>
        /// <param name="field">Current navigation field state</param>
        /// <param name="flow">Existing flow to update</param>
        /// <param name="inputDeps">Optional input job dependencies</param>
        /// <returns>JobHandle representing the scheduled jobs</returns>
        /// <remarks>
        /// This efficiently updates just the direction vectors by:
        /// 1. Recalculating flow directions from existing costs.
        /// 2. Considers current density field for obstacle avoidance.
        /// 
        /// Use this when field and goals haven't changed but agents have moved.
        /// </remarks>
        public static JobHandle Schedule(this UpdateFlowConfig config, in Field field, in Flow flow, JobHandle inputDeps = default)
        {
            var dependency = inputDeps;
            
            dependency = new FlowFieldInternal.CalculateDirectionJob
            {
                Settings = flow.Settings,
                DirectionMap = flow.DirectionMap,
                CostField = flow.Costs,
                Width = field.Width,
                Height = field.Height,
            }.Schedule(flow.DirectionMap.Length, dependency);

            return dependency;
        }

        #endregion

        #region Validators

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void ValidateSettings(this BuildFlowConfig config)
        {
            if (!config.Field.IsCreated)
                throw new InvalidOperationException("BuildFlow: Field is not created");
        }

        #endregion
    }
}