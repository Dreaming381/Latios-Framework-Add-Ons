# FlowField Navigation

This is a lightweight, performance-focused flow-field navigation toolkit that I use in my personal projects.
There are no agent locomotion systems here, only the construction of FlowField. 
[Here you can see a demo project using this addon](https://github.com/Webheart/latios_flowfield_showcase)

## Features

Building a flowfield with obstacles and crowd density calculations. Obstacles are calculated using Psyshock.

## Getting Started

**Scripting Define:** LATIOS_ADDON_FLOWFIELD

**Requirements:**

-   Requires Latios Framework 0.13.0 or newer

**Main Author(s):** Webheart

**Additional Contributors:**

**Support:** Please make feature requests for features you would like to see
added! You can use any of the Latios Framework support channels to make
requests.

## Basic Usage

### Field & Flow

`Field` and `Flow` are structures containing multiple native containers and properties.
The `Field` struct represents the navigation grid and contains all spatial data required for pathfinding and agent navigation.
The `Flow` struct represents generated navigation vectors that guide agents from any grid location toward nearest goal.
These structures can be stored in either a singleton or a `ICollectionComponents`.

### Building

#### Step 1: FlowField.BuildField() - Start

You start the fluent chain by calling `FlowField.BuildField`, which returns a `BuildFieldConfig`. 
The config does not internally allocate any native containers, so it is safe to discard.

##### WithXXX - Choose your settings

**WithTransform(TransformQvvs transform)** - Provide transforms for the navigation grid.

**WithSettings(FieldSettings settings)** - Provide settings such as grid size, cell size, and grid pivot relative to grid transform.

**WithObstacles(CollisionLayer obstaclesLayer)** - Provide a `CollisionLayer` for the obstacles.

**WithAgents(EntityQuery agentsQuery, FlowFieldAgentsTypeHandles agentsHandles)** - Provide an `EntityQuery` for agents to calculate crowd density.
If using a `FluentQuery`, you can ensure correctness by calling  `PatchQueryForFlowFieldAgents()` in the `FluentQuery` chain. 
Otherwise, the query must contain the following components: `FlowField.AgentDirection`, `FlowField.PrevPosition`, `FlowField.Velocity` and `WorldTransform`.

##### Schedule/ScheduleParallel(out Field field, AllocatorManager.AllocatorHandle allocator, JobHandle inputDeps = default)

Constructs the `Field` using jobs. It returns a JobHandle that must be completed or used as a dependency for any operation that wishes to use the output `field`.

Example:

```csharp
state.Dependency = FlowField.BuildField()
    .WithTransform(TransformQvvs.identity)
    .WithSettings(settings.FieldSettings)
    .WithObstacles(obstacleLayer.Layer)
    .WithAgents(agentsQuery, in agentsHandles)
    .ScheduleParallel(out var field, state.WorldUpdateAllocator, state.Dependency);
```


#### Step 2: FlowField.BuildFlow(Field field, EntityQuery goalsQuery, FlowGoalTypeHandles requiredTypeHandles)

Once the `Field` has been built, one or more `Flow` can be built on its basis.
Here you need to provide a `EntityQuery` and type handles for the entities that represent the goals.
If using a `FluentQuery`, you can ensure correctness by calling  `PatchQueryForBuildingFlowGoals()` in the `FluentQuery` chain.
Otherwise, the query must contain the following components: `FlowField.Goal` and `WorldTransform`.

##### WithSettings(FlowSettings settings) - Provide settings such as the influence of crowd density on agent movement directions.

##### Schedule/ScheduleParallel(out Flow flow, AllocatorManager.AllocatorHandle allocator, JobHandle inputDeps = default)

Constructs the `Flow` using jobs. It returns a JobHandle that must be completed or used as a dependency for any operation that wishes to use the output `flow`.

Example:

```csharp
state.Dependency = FlowField.BuildFlow(field, goalsQuery, in flowGoalHandles)
    .WithSettings(settings.FlowSettings)
    .ScheduleParallel(out var flow, state.WorldUpdateAllocator, state.Dependency);
```

#### Step 3: FlowField.AgentsDirections(EntityQuery agentsQuery, in FlowFieldAgentsTypeHandles handles, float deltaTime)

Once the `Field` and `Flow` are ready, directions can be calculated for each agent.
Here you need to provide `deltaTime`, `EntityQuery` and type handles for the agents, similar to those used in constructing the `Field`. 
After that, you can move agents in your system, reading the direction of movement from the `FlowField.AgentDirection` component.

```csharp
state.Dependency = FlowField.AgentsDirections(agentsQuery, handles, SystemAPI.Time.DeltaTime)
    .ScheduleParallel(field, flow, state.Dependency);
state.Dependency = new YourAgentsMovementJob().ScheduleParallel(state.Dependency);
```

##### For convenience there is a `FlowFieldAgentAuthoring`, which will bake all the necessary components on the agent.
`FlowFieldAgentAuthoring` has the following settings: 
- `FootprintSize` - defines the area in which the agent will influence the density calculation.
- `MaxDensity` - how much the agent will influence the density in the cells closest to it.
- `MinDensity` - how much the agent will influence the density in the cells farthest from it.

##### There is also a `FlowFieldGoalAuthoring` that will bake the necessary components for the target entity.

### Updating

If the obstacles have been changed, or if the field transform needs to be changed, the `Field` must be rebuilt.
If the `Field` has been rebuilt, all `Flows` related to it must also be rebuilt.
A `Flow` must also be rebuilt if its goals have changed.
If the spatial state of the `Field` and the `Flow` goals have remained unchanged, but the agents have moved, you can simply recalculate their influence on the flow and field.
Example:

```csharp
state.Dependency = FlowField.UpdateAgentsInfluence(agentsQuery, agentsHandles).ScheduleParallel(field, state.Dependency);
state.Dependency = FlowField.UpdateFlowDirections().ScheduleParallel(field, flow, state.Dependency);
```

## Debug

You can display direction vectors using `FlowFieldDebug.DrawCells(Field field, Flow flow, JobHandle inputDeps)`.
Keep in mind that you will most likely need to run the project with the command line argument `-debug-line-buffer-size X`, where X can be as big as 100k+, depending on the size of your grid.

## Limitations

At the moment, agent directions and FlowField calculations only work for xz world axes.

