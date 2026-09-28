using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Latios.Mash.Systems
{
    /// <summary>
    /// Sets up new Mash entities, then every frame writes action values into the generated state
    /// components and moves buffered events into the event buffers. One instance handles every
    /// generated asset in the project.
    /// </summary>
    /// <remarks>
    /// Runs first in SimulationSystemGroup, so the Input System has already updated for the frame
    /// and gameplay systems see fresh values. When Core's local ticking is installed, this runs
    /// before the ticking loop.
    /// </remarks>
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    [UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(VariableRateSimulationSystemGroup))]
    // TickedLocalSuperSystem updates after FixedStepSimulationSystemGroup, so this already runs before
    // ticking. Don't order against it directly. That logs a warning when ticking isn't installed.
    public partial class MashReceiverAndPollSystem : SystemBase
    {
        // One closed generic per generated asset, so the per-frame path never boxes a receiver or
        // uses reflection.
        abstract class ReceiverDispatch
        {
            public EntityQuery query;

            public abstract void PollAndDrainAll(EntityManager entityManager, in NativeArray<Entity> entities);
        }

        class ReceiverDispatch<TReceiver, TAssetRef, TFailed> : ReceiverDispatch
            where TReceiver : struct, IMashReceiver, IManagedStructComponent, InternalSourceGen.StaticAPI.IManagedStructComponentSourceGenerated
            where TAssetRef : unmanaged, IComponentData, IMashAssetRef
            where TFailed : unmanaged, IComponentData
        {
            public override void PollAndDrainAll(EntityManager entityManager, in NativeArray<Entity> entities)
            {
                foreach (var entity in entities)
                {
                    // Ask the receiver, not an entity component. Instantiate copies components, and
                    // the copy must not think it owns the original's asset.
                    var receiver = entityManager.GetManagedStructComponent<TReceiver>(entity);
                    if (!receiver.isInitialized)
                    {
                        if (!TryInitialize(entityManager, entity, ref receiver))
                            continue;
                    }
                    else
                    {
                        // A blackboard merge can overwrite the AssetRef of an initialized receiver.
                        var                              assetRef         = entityManager.GetComponentData<TAssetRef>(entity);
                        UnityObjectRef<InputActionAsset> liveRuntimeAsset = receiver.runtimeAsset;
                        if (assetRef.runtimeAsset != liveRuntimeAsset)
                        {
                            assetRef.runtimeAsset = liveRuntimeAsset;
                            entityManager.SetComponentData(entity, assetRef);
                        }
                    }
                    receiver.PollAndDrain(entityManager, entity);
                }
            }

            static bool TryInitialize(EntityManager entityManager, Entity entity, ref TReceiver receiver)
            {
                var assetRef    = entityManager.GetComponentData<TAssetRef>(entity);
                var sourceAsset = assetRef.sourceAsset.Value;
                if (sourceAsset == null)
                {
                    Debug.LogError($"Mash: Entity {entity} has no source InputActionAsset in {typeof(TAssetRef).Name} and will be ignored by Mash.");
                    entityManager.AddComponent<TFailed>(entity);
                    return false;
                }

                var runtimeAsset = MashRuntimeAsset.Create(sourceAsset);
                try
                {
                    receiver.Initialize(runtimeAsset);
                }
                catch (Exception e)
                {
                    // The receiver may not have taken ownership of the copy yet, so destroy it here too.
                    receiver.Uninitialize();
                    MashRuntimeAsset.Destroy(runtimeAsset);
                    Debug.LogError($"Mash: Entity {entity} failed to initialize {typeof(TReceiver).Name} and will be ignored by Mash.\n{e}");
                    entityManager.AddComponent<TFailed>(entity);
                    return false;
                }

                entityManager.SetManagedStructComponent(entity, receiver);
                assetRef.runtimeAsset = runtimeAsset;
                entityManager.SetComponentData(entity, assetRef);
                return true;
            }
        }

        List<ReceiverDispatch> m_dispatches;

        protected override void OnCreate()
        {
            m_dispatches = new List<ReceiverDispatch>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }
                if (types == null)
                    continue;

                foreach (var type in types)
                {
                    if (type == null || type.IsAbstract || type.IsInterface || !typeof(IMashGeneratedAsset).IsAssignableFrom(type))
                        continue;
                    if (!(Activator.CreateInstance(type) is IMashGeneratedAsset descriptor))
                        continue;
                    if (TryCreateDispatch(descriptor, out var dispatch))
                        m_dispatches.Add(dispatch);
                }
            }
        }

        bool TryCreateDispatch(IMashGeneratedAsset descriptor, out ReceiverDispatch dispatch)
        {
            dispatch = null;
            var name = descriptor.GetType().FullName;

            var receiverType = descriptor.receiverType;
            if (receiverType == null || !receiverType.IsValueType || !typeof(IMashReceiver).IsAssignableFrom(receiverType) ||
                !typeof(InternalSourceGen.StaticAPI.IManagedStructComponentSourceGenerated).IsAssignableFrom(receiverType))
            {
                Debug.LogError($"Mash: {name} has an invalid receiver type. It must be a partial struct implementing IMashReceiver and IManagedStructComponent. Skipping.");
                return false;
            }
            var assetRefType = descriptor.assetRefType;
            if (assetRefType == null || !assetRefType.IsValueType || !typeof(IMashAssetRef).IsAssignableFrom(assetRefType) ||
                !typeof(IComponentData).IsAssignableFrom(assetRefType))
            {
                Debug.LogError($"Mash: {name} has an invalid asset ref type. It must be an IComponentData struct implementing IMashAssetRef. Skipping.");
                return false;
            }
            var failedType = descriptor.initializationFailedType;
            if (failedType == null || !failedType.IsValueType || !typeof(IComponentData).IsAssignableFrom(failedType))
            {
                Debug.LogError($"Mash: {name} has an invalid initialization failed type. It must be an IComponentData struct. Skipping.");
                return false;
            }

            var existType   = receiverType.GetNestedType("ExistComponent", BindingFlags.Public);
            var cleanupType = receiverType.GetNestedType("CleanupComponent", BindingFlags.Public);
            if (existType == null || cleanupType == null)
            {
                Debug.LogError($"Mash: {receiverType.FullName} is missing its generated ExistComponent or CleanupComponent. Skipping.");
                return false;
            }

            // Requiring the CleanupComponent waits for Core to take ownership of the receiver.
            // A receiver initialized before that is never disposed if the entity is destroyed
            // or merged into a blackboard entity first, which leaks its asset copy.
            FixedList64Bytes<ComponentType> requiredList = default;
            requiredList.Add(ComponentType.FromTypeIndex(TypeManager.GetTypeIndex(assetRefType)));
            requiredList.Add(ComponentType.FromTypeIndex(TypeManager.GetTypeIndex(existType)));
            requiredList.Add(ComponentType.FromTypeIndex(TypeManager.GetTypeIndex(cleanupType)));
            FixedList32Bytes<ComponentType> excludedList = default;
            excludedList.Add(ComponentType.FromTypeIndex(TypeManager.GetTypeIndex(failedType)));

            var builder = new EntityQueryBuilder(Allocator.Temp);
            var query   = builder.WithAll(ref requiredList).WithNone(ref excludedList).Build(this);
            builder.Dispose();

            var dispatchType = typeof(ReceiverDispatch<, , >).MakeGenericType(receiverType, assetRefType, failedType);
            dispatch         = (ReceiverDispatch)Activator.CreateInstance(dispatchType);
            dispatch.query   = query;
            return true;
        }

        protected override void OnUpdate()
        {
            foreach (var dispatch in m_dispatches)
            {
                if (dispatch.query.IsEmptyIgnoreFilter)
                    continue;

                var entities = dispatch.query.ToEntityArray(Allocator.Temp);
                dispatch.PollAndDrainAll(EntityManager, in entities);
                entities.Dispose();
            }
        }
    }
}
