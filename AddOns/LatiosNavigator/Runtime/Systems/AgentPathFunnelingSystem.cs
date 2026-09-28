using Latios.Navigator.Components;
using Latios.Navigator.Utils;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Navigator.Systems
{
    [RequireMatchingQueriesForUpdate]
    internal partial struct AgentPathFunnelingSystem : ISystem, ILatiosApi
    {
        EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);
            m_query = state.Fluent()
                      .With<WorldTransform>(true)
                      .With<NavmeshAgentTag>()
                      .WithEnabled<NavMeshAgent>()
                      .With<AgentDestination>()
                      .With<AgentPath>()
                      .With<AgentPathEdge>()
                      .With<AgentPathPoint>()
                      .WithEnabled<AgentHasEdgePathTag>()
                      .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new FunnelJob
            {
                AgentHasEdgePathTagLookup = SystemAPI.GetComponentLookup<AgentHasEdgePathTag>(),
            }.ScheduleParallel(this.GetApi(ref state), m_query);
        }

        /// <summary>
        ///     This job implements the funnel algorithm to create a path from the agent's current position to its destination
        ///     <see href="https://digestingduck.blogspot.com/2010/03/simple-stupid-funnel-algorithm.html" />
        /// </summary>
        [BurstCompile]
        partial struct FunnelJob : IJobEach
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<AgentHasEdgePathTag> AgentHasEdgePathTagLookup;

            public void Execute(Entity entity, in WorldTransform worldTransform,
                                ref AgentPath agentPath, in AgentDestination destination, in DynamicBuffer<AgentPathEdge> portals,
                                ref DynamicBuffer<AgentPathPoint> pathPoints)
            {
                pathPoints.Clear();
                var start = worldTransform.position;
                var end   = destination.Position;

                // No portals, just a direct path
                if (portals.IsEmpty)
                {
                    pathPoints.Add(new AgentPathPoint
                    {
                        Position = start
                    });

                    pathPoints.Add(new AgentPathPoint
                    {
                        Position = end
                    });

                    agentPath.PathLength = 2;
                    agentPath.PathIndex  = 0;
                    AgentHasEdgePathTagLookup.SetComponentEnabled(entity, false);
                    return;
                }

                var portalCount = portals.Length;
                var portalApex  =
                    portals[0].PortalVertex1;  // Start with the first portal's left vertex as the apex

                var portalLeft  = portals[0].PortalVertex1;
                var portalRight = portals[0].PortalVertex2;
                int leftIndex   = 0, rightIndex = 0, apexIndex = 0;

                for (var i = 1; i < portalCount; i++)
                {
                    var left  = portals[i].PortalVertex1;
                    var right = portals[i].PortalVertex2;

                    // Right leg
                    if (TriMath.SignedArea2D(portalApex, portalRight, right) <= 0f)
                    {
                        if (Vequals(portalApex, portalRight) ||
                            TriMath.SignedArea2D(portalApex, portalLeft, right) > 0f)
                        {
                            // Tighten the funnel
                            portalRight = right;
                            rightIndex  = i;
                        }
                        else
                        {
                            // Right over left
                            pathPoints.Add(new AgentPathPoint
                            {
                                Position = portalLeft
                            });

                            portalApex = portalLeft;
                            apexIndex  = leftIndex;

                            // Reset
                            portalLeft  = portalApex;
                            portalRight = portalApex;

                            leftIndex  = apexIndex;
                            rightIndex = apexIndex;
                            i          = apexIndex;
                            continue;
                        }
                    }

                    // Left leg
                    if (TriMath.SignedArea2D(portalApex, portalLeft, left) >= 0f)
                    {
                        if (Vequals(portalApex, portalLeft) || TriMath.SignedArea2D(portalApex, portalRight, left) < 0f)
                        {
                            // Tighten the funnel
                            portalLeft = left;
                            leftIndex  = i;
                        }
                        else
                        {
                            // Left over right
                            pathPoints.Add(new AgentPathPoint
                            {
                                Position = portalRight
                            });

                            portalApex = portalRight;
                            apexIndex  = rightIndex;

                            // Reset
                            portalLeft  = portalApex;
                            portalRight = portalApex;
                            leftIndex   = apexIndex;
                            rightIndex  = apexIndex;
                            i           = apexIndex;
                        }
                    }
                }

                pathPoints.Add(new AgentPathPoint
                {
                    Position = end
                });

                agentPath.PathLength = pathPoints.Length;
                agentPath.PathIndex  = 0;
                AgentHasEdgePathTagLookup.SetComponentEnabled(entity, false);
            }
        }

        static bool Vequals(float3 a, float3 b) => math.distancesq(a, b) < .001f * .001f;
    }
}

