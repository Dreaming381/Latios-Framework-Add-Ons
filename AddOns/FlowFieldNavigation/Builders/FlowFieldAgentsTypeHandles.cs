using Latios.Transforms.Abstract;
using Unity.Collections;
using Unity.Entities;

namespace Latios.FlowFieldNavigation
{
    public struct FlowFieldAgentsTypeHandles
    {
        [ReadOnly] internal EntityTypeHandle Entity;
        [ReadOnly] internal WorldTransformReadOnlyAspect.TypeHandle WorldTransform;
        internal ComponentTypeHandle<FlowField.AgentDirection> AgentDirection;
        internal ComponentTypeHandle<FlowField.AgentFootprint> AgentFootprint;
        internal ComponentTypeHandle<FlowField.AgentDensity> AgentDensity;
        internal ComponentTypeHandle<FlowField.PrevPosition> PrevPosition;
        internal ComponentTypeHandle<FlowField.Velocity> Velocity;

        /// <summary>
        /// Constructs the FlowFieldAgents type handles using a managed system
        /// </summary>
        public FlowFieldAgentsTypeHandles(SystemBase system)
        {
            WorldTransform = new WorldTransformReadOnlyAspect.TypeHandle(ref system.CheckedStateRef);
            AgentDirection = system.GetComponentTypeHandle<FlowField.AgentDirection>();
            AgentFootprint = system.GetComponentTypeHandle<FlowField.AgentFootprint>();
            AgentDensity = system.GetComponentTypeHandle<FlowField.AgentDensity>();
            PrevPosition = system.GetComponentTypeHandle<FlowField.PrevPosition>();
            Velocity = system.GetComponentTypeHandle<FlowField.Velocity>();
            Entity = system.GetEntityTypeHandle();
        }

        /// <summary>
        /// Constructs the FlowFieldAgents type handles using a SystemState
        /// </summary>
        public FlowFieldAgentsTypeHandles(ref SystemState system)
        {
            WorldTransform = new WorldTransformReadOnlyAspect.TypeHandle(ref system);
            AgentDirection = system.GetComponentTypeHandle<FlowField.AgentDirection>();
            AgentFootprint = system.GetComponentTypeHandle<FlowField.AgentFootprint>();
            AgentDensity = system.GetComponentTypeHandle<FlowField.AgentDensity>();
            PrevPosition = system.GetComponentTypeHandle<FlowField.PrevPosition>();
            Velocity = system.GetComponentTypeHandle<FlowField.Velocity>();
            Entity = system.GetEntityTypeHandle();
        }

        /// <summary>
        /// Updates the type handles using a managed system
        /// </summary>
        public void Update(SystemBase system)
        {
            WorldTransform.Update(ref system.CheckedStateRef);
            AgentDirection.Update(system);
            AgentFootprint.Update(system);
            AgentDensity.Update(system);
            PrevPosition.Update(system);
            Velocity.Update(system);
            Entity.Update(system);
        }

        /// <summary>
        /// Updates the type handles using a SystemState
        /// </summary>
        public void Update(ref SystemState system)
        {
            WorldTransform.Update(ref system);
            AgentDirection.Update(ref system);
            AgentFootprint.Update(ref system);
            AgentDensity.Update(ref system);
            PrevPosition.Update(ref system);
            Velocity.Update(ref system);
            Entity.Update(ref system);
        }
    }
}