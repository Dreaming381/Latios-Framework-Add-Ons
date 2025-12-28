#if LATIOS_ADDON_TERRAINY_DEBUG
using Latios.Psyshock;
using Unity.Burst;
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
	[WorldSystemFilter(WorldSystemFilterFlags.Editor)]
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[RequireMatchingQueriesForUpdate]
	public partial class TerrainColliderDebugSystem : SystemBase
	{
		private Camera _camera;
		private Mouse _current;
		
		[BurstCompile]
		public partial struct RayJob : IJobEntity
		{
			public Ray Ray;

			private void Execute(in Collider collider, in WorldTransform transform)
			{
				if (collider.type != ColliderType.Terrain) return;
				//var rigid = new RigidTransform(transform.ValueRO.worldTransform.ToMatrix4x4());
				//PhysicsDebug.DrawCollider(in collider.ValueRO, rigid, Color.red);
				if (Physics.Raycast(in Ray, in collider, in transform.worldTransform, out var hit))
				{
					Debug.Log("Hit Ray");
				}
			}
		}

		protected override void OnStartRunning()
		{
			base.OnStartRunning();
			_camera = Camera.main;
			_current = Mouse.current;
		}
		
		protected override void OnUpdate()
		{
#if !LATIOS_TRANSFORMS_UNITY
			if (!_current.leftButton.wasPressedThisFrame) return;
			if (_camera == null) {
				_camera = Camera.main;
			}
			var unityRay = _camera.ScreenPointToRay(_current.position.ReadValue());
			Ray ray = new Ray(unityRay.origin, unityRay.direction, 100f);
			Debug.DrawLine(ray.start, ray.end, Color.blue, 1f);
			var job = new RayJob()
			{
				Ray = ray
			};
			job.ScheduleParallel();
#else
			foreach (var (collider, localToWorld) in SystemAPI.Query<Collider, LocalToWorld>())
			{
				if (collider.type != ColliderType.Terrain) continue;
				var rigid = new RigidTransform(localToWorld.Value);
				PhysicsDebug.DrawCollider(in collider, in rigid, Color.red);
			}
#endif
		}
	}
}
#endif