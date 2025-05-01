using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna
{
    internal partial struct CapturedRigidBodies : ICollectionComponent
    {
        public NativeArray<CapturedRigidBodyState> states;  // src indices
        public NativeParallelHashMap<Entity, int>  entityToSrcIndexMap;

        public JobHandle TryDispose(JobHandle inputDeps) => inputDeps;  // WorldUpdateAllocator
    }

    internal partial struct CapturedKinematics : ICollectionComponent
    {
        public NativeArray<CapturedKinematic>     kinematics;  // src indices
        public NativeParallelHashMap<Entity, int> entityToSrcIndexMap;

        public JobHandle TryDispose(JobHandle inputDeps) => inputDeps;  // WorldUpdateAllocator
    }

    // Enabled during constraint writing phase
    internal struct ConstraintWritingConstants : IComponentData
    {
        public float deltaTime;
        public float inverseDeltaTime;
        public int   numIterations;
        public int   numSubSteps;
        public float stiffDamping;
        public float stiffTau;
        public uint  constraintStartGlobalVersion;
        public bool  isInConstraintWritingPhase;
    }

    internal partial struct BroadphaseCollisionWorld : ICollectionComponent
    {
        public CollisionWorld collisionWorld;

        public JobHandle TryDispose(JobHandle inputDeps) => inputDeps;  // Allocated with WorldUpdateAllocator
    }
}

