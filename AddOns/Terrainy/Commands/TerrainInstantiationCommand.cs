using AOT;
using Latios.Terrainy.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Latios.Terrainy.Commands
{
	/// <summary>
	/// Struct which gets a callback for every Entity that is added to a terrain.
	/// It will then create the decoration group for the entities
	/// </summary>
	[BurstCompile]
	internal struct TerrainInstantiationCommand : IInstantiateCommand
	{

		internal Entity terrainEntity; 
		
		public FunctionPointer<IInstantiateCommand.OnPlayback> GetFunctionPointer()
		{
			return BurstCompiler.CompileFunctionPointer<IInstantiateCommand.OnPlayback>(CallbackFunction);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(IInstantiateCommand.OnPlayback))]
		internal static void CallbackFunction(ref IInstantiateCommand.Context context)
		{
			//Debug.Log($"Test with entities: {context.entities.Length}");
			var terrainToCreatedEntities = new NativeHashMap<Entity, NativeList<Entity>>(32, Allocator.Temp);
			for (var index = 0; index < context.entities.Length; index++)
			{
				Entity entity = context.entities[index];
				var command = context.ReadCommand<TerrainInstantiationCommand>(index);
				if (terrainToCreatedEntities.ContainsKey(command.terrainEntity))
				{
					terrainToCreatedEntities[command.terrainEntity].Add(entity);
				}
				else
				{
					terrainToCreatedEntities.Add(command.terrainEntity, new NativeList<Entity>(Allocator.Temp));
					terrainToCreatedEntities[command.terrainEntity].Add(entity);
				}
			}

			foreach (var kvPair in terrainToCreatedEntities)
			{
				Entity decorationsGroupEntity = context.entityManager.CreateEntity();
				DynamicBuffer<Entity> leg = context.entityManager.AddBuffer<LinkedEntityGroup>(decorationsGroupEntity).Reinterpret<Entity>();
				leg.AddRange(kvPair.Value.AsArray());
				var terrainComp = context.entityManager.GetComponentData<TerrainComponent>(kvPair.Key);
				terrainComp.DecorationsGroupEntity = decorationsGroupEntity;
				context.entityManager.SetComponentData(kvPair.Key, terrainComp);
			}
			
		}
	}
}