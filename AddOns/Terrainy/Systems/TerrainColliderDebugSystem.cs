using Latios.Psyshock;
#if !LATIOS_TRANSFORMS_UNITY
using Latios.Transforms;
using Ray = Latios.Psyshock.Ray;
using Physics = Latios.Psyshock.Physics;
using UnityEngine.InputSystem;
#elif LATIOS_TRANSFORMS_UNITY
using Unity.Transforms;
#endif
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Collider = Latios.Psyshock.Collider;

namespace Latios.Terrainy.Systems
{
	[DisableAutoCreation]
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[WorldSystemFilter(WorldSystemFilterFlags.Editor)]
	public partial struct TerrainColliderDebugSystem : ISystem
	{
		public void OnCreate(ref SystemState state) { }

		public void OnUpdate(ref SystemState state)
		{

#if !LATIOS_TRANSFORMS_UNITY
			var camera = Camera.main;
			var unityRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
			Debug.DrawRay(unityRay.origin, unityRay.direction * 100f, Color.blue);
			Ray ray = new Ray(unityRay.origin, unityRay.direction, 100f);
			foreach (var (collider, transform, entity) in SystemAPI.Query<RefRO<Collider>, RefRO<WorldTransform>>().WithEntityAccess())
			{
				if (collider.ValueRO.type != ColliderType.Terrain) continue;
				var rigid = new RigidTransform(transform.ValueRO.worldTransform.ToMatrix4x4());
				PhysicsDebug.DrawCollider(in collider.ValueRO, rigid, Color.red);
				if (Physics.Raycast(ray, in collider.ValueRO, in transform.ValueRO.worldTransform, out var hit))
				{
					Debug.Log("Hit Ray");
				}
			}
#else
			foreach (var (collider, localToWorld) in SystemAPI.Query<Collider, LocalToWorld>())
			{
				if (collider.type != ColliderType.Terrain) continue;
				var rigid = new RigidTransform(localToWorld.Value);
				PhysicsDebug.DrawCollider(in collider, in rigid, Color.red);
			}
#endif
		}

		public void OnDestroy(ref SystemState state) { }

	}
}