using System.Diagnostics;
using Latios.Psyshock;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna
{
    public struct LockWorldAxesFlags : IComponentData
    {
        public byte packedFlags;

        public bool positionX
        {
            get => (packedFlags & 0x1) != 0;
            set => packedFlags = (byte)((packedFlags & ~0x1) | math.select(0, 0x1, value));
        }
        public bool positionY
        {
            get => (packedFlags & 0x2) != 0;
            set => packedFlags = (byte)((packedFlags & ~0x2) | math.select(0, 0x2, value));
        }
        public bool positionZ
        {
            get => (packedFlags & 0x4) != 0;
            set => packedFlags = (byte)((packedFlags & ~0x4) | math.select(0, 0x4, value));
        }
        public bool rotationX
        {
            get => (packedFlags & 0x8) != 0;
            set => packedFlags = (byte)((packedFlags & ~0x8) | math.select(0, 0x8, value));
        }
        public bool rotationY
        {
            get => (packedFlags & 0x10) != 0;
            set => packedFlags = (byte)((packedFlags & ~0x10) | math.select(0, 0x10, value));
        }
        public bool rotationZ
        {
            get => (packedFlags & 0x20) != 0;
            set => packedFlags = (byte)((packedFlags & ~0x20) | math.select(0, 0x20, value));
        }
    }

    public struct ConstraintEntityInfoLookup : ICollectionAspect<ConstraintEntityInfoLookup>
    {
        [ReadOnly] internal CapturedRigidBodies rigidBodies;
        [ReadOnly] internal CapturedKinematics  kinematics;
        internal CollisionLayerSettings         collisionLayerSettings;
        internal ConstraintWritingConstants     constants;

        public struct RigidBodyHandle
        {
            internal Entity entity;
            internal int    index;
        }

        public struct KinematicHandle
        {
            internal Entity entity;
            internal int    index;
        }

        public bool TryGetRigidBodyHandle(Entity entity, out RigidBodyHandle rigidBodyHandle)
        {
            rigidBodyHandle.index  = -1;
            rigidBodyHandle.entity = entity;
            return rigidBodies.entityToSrcIndexMap.TryGetValue(entity, out rigidBodyHandle.index);
        }

        public bool TryGetKinematicHandle(Entity entity, out KinematicHandle kinematicHandle)
        {
            kinematicHandle.index  = -1;
            kinematicHandle.entity = entity;
            return kinematics.entityToSrcIndexMap.TryGetValue(entity, out kinematicHandle.index);
        }

        public float GetCollisionMaxDistanceBetween(in RigidBodyHandle rigidBodyA)
        {
            var expansionA = rigidBodies.states[rigidBodyA.index].motionExpansion;
            return UnitySim.MotionExpansion.GetMaxDistance(in expansionA);
        }

        public float GetCollisionMaxDistanceBetween(in RigidBodyHandle rigidBodyA, in RigidBodyHandle rigidBodyB)
        {
            var expansionA = rigidBodies.states[rigidBodyA.index].motionExpansion;
            var expansionB = rigidBodies.states[rigidBodyB.index].motionExpansion;
            return UnitySim.MotionExpansion.GetMaxDistance(in expansionA, in expansionB);
        }

        public float GetCollisionMaxDistanceBetween(in RigidBodyHandle rigidBodyA, in KinematicHandle kinematicB)
        {
            var expansionA = rigidBodies.states[rigidBodyA.index].motionExpansion;
            var expansionB = kinematics.kinematics[kinematicB.index].motionExpansion;
            return UnitySim.MotionExpansion.GetMaxDistance(in expansionA, in expansionB);
        }

        public float GetCoefficientOfRestitution(in RigidBodyHandle rigidBodyHandle) => rigidBodies.states[rigidBodyHandle.index].coefficientOfRestitution;
        public float GetCoefficientOfFriction(in RigidBodyHandle rigidBodyHandle) => rigidBodies.states[rigidBodyHandle.index].coefficientOfFriction;

        public FluentQuery AppendToQuery(FluentQuery query) => query.With<CapturedRigidBodies.ExistComponent, CapturedKinematics.ExistComponent>(true);

        public ConstraintEntityInfoLookup CreateCollectionAspect(LatiosWorldUnmanaged latiosWorld, EntityManager entityManager, Entity entity)
        {
            var k = entityManager.GetComponentData<ConstraintWritingConstants>(entity);
            CheckSafeToAccess(in k);
            return new ConstraintEntityInfoLookup
            {
                collisionLayerSettings = latiosWorld.GetPhysicsSettings().collisionLayerSettings,
                rigidBodies            = latiosWorld.GetCollectionComponent<CapturedRigidBodies>(entity, true),
                kinematics             = latiosWorld.GetCollectionComponent<CapturedKinematics>(entity, true),
                constants              = k,
            };
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckSafeToAccess(in ConstraintWritingConstants constants)
        {
            if (!constants.isInConstraintWritingPhase)
                throw new System.InvalidOperationException("ConstraintEntityInfoLookup must only be accessed in a system within ConstraintWritingSuperSystem");
        }
    }

    public struct ConstraintCollisionWorld : ICollectionAspect<ConstraintCollisionWorld>
    {
        [ReadOnly] public CollisionWorld collisionWorld;

        public FluentQuery AppendToQuery(FluentQuery query) => query.With<BroadphaseCollisionWorld.ExistComponent>();

        public ConstraintCollisionWorld CreateCollectionAspect(LatiosWorldUnmanaged latiosWorld, EntityManager entityManager, Entity entity)
        {
            return new ConstraintCollisionWorld { collisionWorld = latiosWorld.GetCollectionComponent<BroadphaseCollisionWorld>(entity, true).collisionWorld };
        }
    }

    [InternalBufferCapacity(3)]
    public struct CollisionExclusionPair : IBufferElementData
    {
        // Note: Order does NOT matter here.
        public EntityQueryMask queryA;
        public EntityQueryMask queryB;
    }
}

