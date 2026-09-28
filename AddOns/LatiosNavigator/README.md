# Navigator - A simple navigation system for Unity DOTS and Latios Framework

Navigator is a simple implementation of a navigation mesh system for Unity DOTS
and Latios Framework.

## Getting Started

**Scripting Define:** `LATIOS_ADDON_NAVIGATOR`

**Requirements:**

-   Requires Latios Framework 0.16.1 or newer

**Main Author(s):** [clandais](https://github.com/clandais)

### Installing

Add the following to `LatiosBootstrap`:

```csharp
// In LatiosbakingBootstrap:
Latios.Navigator.NavigatorBakingBootstrap.InstallNavigatorBakers(ref context);
// In LatiosBootstrap:
Latios.Navigator.NavigatorBootstrap.InstallNavigator(world);
```

## Usage

### Baking a Navigation Mesh

-   Layout your level geometry in the scene, then add a standard Unity
    `NavMeshSurface` component to a GameObject in the scene.
-   Add a `BlackboardEntityDataAuthoring` component to the same GameObject
    (World or Scene).
-   Configure it as desired, then bake the nav mesh.
-   The baked nav mesh will be automatically converted to a Latios navigation
    mesh.

>   Navigator does not currently support dynamic nav mesh updates.

>   Navigator only supports a single navigation mesh (no support for multiple
>   agent types)

### Agents

To use the navigation system, you need to create an agent. This can be done by
adding a standard Unity `NavMeshAgent` component to a GameObject in the scene.
The agent will automatically be converted to a `NavMeshAgent` component.

>   Currently, only the radius of the agent is used for navigation.

### Pathfinding

#### Setting the goal position and requesting a path

Given a `NavMeshAgent`, getting a path to a target position is done this way:

```csharp
// given a goal position
float3 goalPosition = ...;

// Set the agent's destination
foreach (var (destination, entity) in SystemAPI
             .Query<RefRW<AgentDestination>>()
             .WithEntityAccess())
{
    // Enable the AgenPathRequestedTag to indicate that a path request is being made
    // This will trigger the pathfinding system to calculate a path
    ecb.SetComponentEnabled<AgenPathRequestedTag>(entity, true);
    // Set the destination position
    destination.ValueRW.Position = goalPosition;
}
```

### Internal Pathfinding Systems

The pathfinding system will automatically compute a path (if any) in two steps:

1.  `AgentEdgePathSystem` will compute a path of "portals" between the agent's
    current position and the goal position.
2.  `AgentPathFunnelingSystem` will refine the path by removing unnecessary
    waypoints, creating a more direct path.

### Retrieving the path

Retrieving the path is done by querying the `DynamicBuffer<AgentPathPoint>` and
the optional `AgentPath`. Example IJobEach.Execute

```csharp
void Execute(
            TransformAspect transformAspect,
            ref AgentPath pathState,
            in DynamicBuffer<AgentPathPoint> pathPoints,
            in NavMeshAgent navMeshAgent)
{

    // AgentPath is a utility component that contains the current path's length and the current path's index.
    // AgentPath.Length  == DynamicBuffer<AgentPathPoint>.Length;
    // AgentPath.Index is reset to 0 when a new path is computed.
    var currentIndex = pathState.PathIndex;
    var pointCount = pathState.PathLength;
    if (currentIndex >= pointCount || pointCount == 0)
    {
        return;
    }
    
    // Get the current target position from the path
    var targetPosition = pathPoints[currentIndex].Position;
    
    // Move the agent towards the target position
    var direction = math.normalize(targetPosition - transformAspect.worldPosition);
    // ...
    // If the agent is close enough to the target position, increment the path index
    if (math.distancesq(transformAspect.worldPosition, targetPosition) < navMeshAgent.Radius * navMeshAgent.StoppingDistance)
    {
        // You have to increment the path index manually if you want to use it.
        pathState.PathIndex++;
    }
}
```

### Debugging

You can visualize the navigation mesh with `NavUtils.Debug(ref
NavMeshSurfaceBlob navMeshSurfaceBlob)`. This will draw the navigation mesh in
the scene view.

You can also visualize the `NavMeshSurfaceBlob`'s adjacency map with
`NavUtils.DebugAdjacency(ref NavMeshSurfaceBlob navMeshSurfaceBlob, Color
color)`.
