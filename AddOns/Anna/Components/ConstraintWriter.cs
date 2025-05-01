using System;
using System.Diagnostics;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Anna
{
    public partial struct ConstraintWriter : ICollectionComponent
    {
        internal PairStream pairStream;
        internal uint       systemScheduleOrder;

        public ConstraintWriter(ref SystemState systemState, LatiosWorldUnmanaged latiosWold)
        {
            var constants = latiosWold.sceneBlackboardEntity.GetComponentData<ConstraintWritingConstants>();
            CheckSafeToAccess(in constants);

            var settings        = latiosWold.GetPhysicsSettings();
            pairStream          = new PairStream(settings.collisionLayerSettings, systemState.WorldUpdateAllocator);
            systemScheduleOrder = systemState.GlobalSystemVersion - constants.constraintStartGlobalVersion;
        }

        public ParallelWriter AsParallelWriter() => new ParallelWriter
        {
            pairStream = pairStream.AsParallelWriter()
        };

        public void LockWorldAxes(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandle,
                                  LockWorldAxesFlags axesToLock)
        {
            var body                       = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandle.index];
            var bucketIndex                = body.bucketIndex;
            var inertialPoseWorldTransform = body.inertialPoseWorldTransform;
            var positions                  = axesToLock.packedFlags & 0x7;
            if (positions != 0)
            {
                var     bools      = positions == new int3(1, 2, 4);
                ref var streamData = ref pairStream.AddPairAndGetRef<PositionConstraintData>(rigidBodyHandle.entity,
                                                                                             bucketIndex,
                                                                                             true,
                                                                                             Entity.Null,
                                                                                             bucketIndex,
                                                                                             false,
                                                                                             out var pair);
                pair.userByte     = SolveByteCodes.positionConstraint;
                streamData.indexA = rigidBodyHandle.index;
                streamData.indexB = -1;
                UnitySim.BuildJacobian(out streamData.parameters,
                                       inertialPoseWorldTransform,
                                       float3.zero,
                                       RigidTransform.identity,
                                       new RigidTransform(quaternion.identity, inertialPoseWorldTransform.pos),
                                       0f,
                                       0f,
                                       constraintEntityInfoLookup.constants.stiffTau,
                                       constraintEntityInfoLookup.constants.stiffDamping,
                                       bools);
            }
            var rotations = axesToLock.packedFlags & 0x38;
            if (rotations != 0)
            {
                rotations           >>= 3;
                var constraintCount   = math.countbits(rotations);
                if (constraintCount == 3)
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation3ConstraintData>(rigidBodyHandle.entity,
                                                                                                  bucketIndex,
                                                                                                  true,
                                                                                                  Entity.Null,
                                                                                                  bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint3;
                    streamData.indexA = rigidBodyHandle.index;
                    streamData.indexB = -1;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseWorldTransform.rot,
                                           quaternion.identity,
                                           inertialPoseWorldTransform.rot,
                                           quaternion.identity,
                                           0f,
                                           0f,
                                           constraintEntityInfoLookup.constants.stiffTau,
                                           constraintEntityInfoLookup.constants.stiffDamping);
                }
                else if (constraintCount == 2)
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation2ConstraintData>(rigidBodyHandle.entity,
                                                                                                  bucketIndex,
                                                                                                  true,
                                                                                                  Entity.Null,
                                                                                                  bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint2;
                    streamData.indexA = rigidBodyHandle.index;
                    streamData.indexB = -1;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseWorldTransform.rot,
                                           quaternion.identity,
                                           inertialPoseWorldTransform.rot,
                                           quaternion.identity,
                                           0f,
                                           0f,
                                           constraintEntityInfoLookup.constants.stiffTau,
                                           constraintEntityInfoLookup.constants.stiffDamping,
                                           math.tzcnt(~rotations));
                }
                else
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation1ConstraintData>(rigidBodyHandle.entity,
                                                                                                  bucketIndex,
                                                                                                  true,
                                                                                                  Entity.Null,
                                                                                                  bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint1;
                    streamData.indexA = rigidBodyHandle.index;
                    streamData.indexB = -1;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseWorldTransform.rot,
                                           quaternion.identity,
                                           inertialPoseWorldTransform.rot,
                                           quaternion.identity,
                                           0f,
                                           0f,
                                           constraintEntityInfoLookup.constants.stiffTau,
                                           constraintEntityInfoLookup.constants.stiffDamping,
                                           math.tzcnt(rotations));
                }
            }
        }

        public void ConstrainPositions(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                       float3 worldSpaceJointPositionOnA,
                                       RigidTransform worldSpaceJointPositionAndOrientationOnB,
                                       float minDistance,
                                       float maxDistance,
                                       ConstraintEntityInfoLookup.ConstraintSpring spring,
                                       bool3 constrainedAxes)
        {
            if (!math.any(constrainedAxes))
                return;
            var     bodyA         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
            var     bodyB         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleB.index];
            var     inertialPoseA = bodyA.inertialPoseWorldTransform;
            var     inertialPoseB = bodyB.inertialPoseWorldTransform;
            var     jointLocalA   = math.InverseRotateFast(inertialPoseA.rot, worldSpaceJointPositionOnA - inertialPoseA.pos);
            var     jointLocalB   = math.mul(math.inverse(inertialPoseB), worldSpaceJointPositionAndOrientationOnB);
            ref var streamData    = ref pairStream.AddPairAndGetRef<PositionConstraintData>(rigidBodyHandleA.entity,
                                                                                            bodyA.bucketIndex,
                                                                                            true,
                                                                                            rigidBodyHandleB.entity,
                                                                                            bodyB.bucketIndex,
                                                                                            true,
                                                                                            out var pair);
            pair.userByte     = SolveByteCodes.positionConstraint;
            streamData.indexA = rigidBodyHandleA.index;
            streamData.indexB = rigidBodyHandleB.index;
            UnitySim.BuildJacobian(out streamData.parameters,
                                   inertialPoseA,
                                   jointLocalA,
                                   inertialPoseB,
                                   jointLocalB,
                                   minDistance,
                                   maxDistance,
                                   spring.tau,
                                   spring.damping,
                                   constrainedAxes);
        }

        public void ConstrainPositions(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                       ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                       float3 worldSpaceJointPositionOnA,
                                       RigidTransform worldSpaceJointPositionAndOrientationOnB,
                                       float minDistance,
                                       float maxDistance,
                                       ConstraintEntityInfoLookup.ConstraintSpring spring,
                                       bool3 constrainedAxes)
        {
            if (!math.any(constrainedAxes))
                return;
            var     bodyA         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
            var     bodyB         = constraintEntityInfoLookup.kinematics.kinematics[kinematicHandleB.index];
            var     inertialPoseA = bodyA.inertialPoseWorldTransform;
            var     inertialPoseB = bodyB.inertialPoseWorldTransform;
            var     jointLocalA   = math.InverseRotateFast(inertialPoseA.rot, worldSpaceJointPositionOnA - inertialPoseA.pos);
            var     jointLocalB   = math.mul(math.inverse(inertialPoseB), worldSpaceJointPositionAndOrientationOnB);
            ref var streamData    = ref pairStream.AddPairAndGetRef<PositionConstraintData>(rigidBodyHandleA.entity,
                                                                                            bodyA.bucketIndex,
                                                                                            true,
                                                                                            kinematicHandleB.entity,
                                                                                            bodyB.bucketIndex,
                                                                                            false,
                                                                                            out var pair);
            pair.userByte     = SolveByteCodes.positionConstraint;
            streamData.indexA = rigidBodyHandleA.index;
            streamData.indexB = kinematicHandleB.index;
            UnitySim.BuildJacobian(out streamData.parameters,
                                   inertialPoseA,
                                   jointLocalA,
                                   inertialPoseB,
                                   jointLocalB,
                                   minDistance,
                                   maxDistance,
                                   spring.tau,
                                   spring.damping,
                                   constrainedAxes);
        }

        public void ConstrainPositions(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                       float3 worldSpaceJointPositionOnA,
                                       RigidTransform worldSpaceWorldJointPositionAndOrientation,
                                       float minDistance,
                                       float maxDistance,
                                       ConstraintEntityInfoLookup.ConstraintSpring spring,
                                       bool3 constrainedAxes)
        {
            if (!math.any(constrainedAxes))
                return;
            var     bodyA         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
            var     inertialPoseA = bodyA.inertialPoseWorldTransform;
            var     jointLocalA   = math.InverseRotateFast(inertialPoseA.rot, worldSpaceJointPositionOnA - inertialPoseA.pos);
            ref var streamData    = ref pairStream.AddPairAndGetRef<PositionConstraintData>(rigidBodyHandleA.entity,
                                                                                            bodyA.bucketIndex,
                                                                                            true,
                                                                                            Entity.Null,
                                                                                            bodyA.bucketIndex,
                                                                                            false,
                                                                                            out var pair);
            pair.userByte     = SolveByteCodes.positionConstraint;
            streamData.indexA = rigidBodyHandleA.index;
            UnitySim.BuildJacobian(out streamData.parameters,
                                   inertialPoseA,
                                   jointLocalA,
                                   RigidTransform.identity,
                                   worldSpaceWorldJointPositionAndOrientation,
                                   minDistance,
                                   maxDistance,
                                   spring.tau,
                                   spring.damping,
                                   constrainedAxes);
        }

        public void ConstrainRotations(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                       quaternion worldSpaceJointRotationForA,
                                       quaternion worldSpaceJointRotationForB,
                                       float minAngle,
                                       float maxAngle,
                                       ConstraintEntityInfoLookup.ConstraintSpring spring,
                                       bool3 constrainedAxes)
        {
            var bits     = math.bitmask(new bool4(constrainedAxes, false));
            var bitCount = math.countbits(bits);
            if (bitCount == 0)
                return;
            var bodyA         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
            var bodyB         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleB.index];
            var inertialPoseA = bodyA.inertialPoseWorldTransform;
            var inertialPoseB = bodyB.inertialPoseWorldTransform;
            var jointLocalA   = math.InverseRotateFast(inertialPoseA.rot, worldSpaceJointRotationForA);
            var jointLocalB   = math.InverseRotateFast(inertialPoseB.rot, worldSpaceJointRotationForB);

            switch (bitCount)
            {
                case 1:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation1ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  rigidBodyHandleB.entity,
                                                                                                  bodyB.bucketIndex,
                                                                                                  true,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint1;
                    streamData.indexA = rigidBodyHandleA.index;
                    streamData.indexB = rigidBodyHandleB.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           inertialPoseB.rot,
                                           jointLocalB,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping,
                                           math.tzcnt(bits));
                    break;
                }
                case 2:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation2ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  rigidBodyHandleB.entity,
                                                                                                  bodyB.bucketIndex,
                                                                                                  true,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint2;
                    streamData.indexA = rigidBodyHandleA.index;
                    streamData.indexB = rigidBodyHandleB.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           inertialPoseB.rot,
                                           jointLocalB,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping,
                                           math.tzcnt(~bits));
                    break;
                }
                case 3:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation3ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  rigidBodyHandleB.entity,
                                                                                                  bodyB.bucketIndex,
                                                                                                  true,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint3;
                    streamData.indexA = rigidBodyHandleA.index;
                    streamData.indexB = rigidBodyHandleB.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           inertialPoseB.rot,
                                           jointLocalB,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping);
                    break;
                }
            }
        }

        public void ConstrainRotations(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                       ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                       quaternion worldSpaceJointRotationForA,
                                       quaternion worldSpaceJointRotationForB,
                                       float minAngle,
                                       float maxAngle,
                                       ConstraintEntityInfoLookup.ConstraintSpring spring,
                                       bool3 constrainedAxes)
        {
            var bits     = math.bitmask(new bool4(constrainedAxes, false));
            var bitCount = math.countbits(bits);
            if (bitCount == 0)
                return;
            var bodyA         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
            var bodyB         = constraintEntityInfoLookup.kinematics.kinematics[kinematicHandleB.index];
            var inertialPoseA = bodyA.inertialPoseWorldTransform;
            var inertialPoseB = bodyB.inertialPoseWorldTransform;
            var jointLocalA   = math.InverseRotateFast(inertialPoseA.rot, worldSpaceJointRotationForA);
            var jointLocalB   = math.InverseRotateFast(inertialPoseB.rot, worldSpaceJointRotationForB);

            switch (bitCount)
            {
                case 1:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation1ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  kinematicHandleB.entity,
                                                                                                  bodyB.bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint1;
                    streamData.indexA = rigidBodyHandleA.index;
                    streamData.indexB = kinematicHandleB.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           inertialPoseB.rot,
                                           jointLocalB,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping,
                                           math.tzcnt(bits));
                    break;
                }
                case 2:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation2ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  kinematicHandleB.entity,
                                                                                                  bodyB.bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint2;
                    streamData.indexA = rigidBodyHandleA.index;
                    streamData.indexB = kinematicHandleB.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           inertialPoseB.rot,
                                           jointLocalB,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping,
                                           math.tzcnt(~bits));
                    break;
                }
                case 3:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation3ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  kinematicHandleB.entity,
                                                                                                  bodyB.bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint3;
                    streamData.indexA = rigidBodyHandleA.index;
                    streamData.indexB = kinematicHandleB.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           inertialPoseB.rot,
                                           jointLocalB,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping);
                    break;
                }
            }
        }

        public void ConstrainRotations(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                       ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                       quaternion worldSpaceJointRotationForA,
                                       quaternion worldSpaceWorldJointRotation,
                                       float minAngle,
                                       float maxAngle,
                                       ConstraintEntityInfoLookup.ConstraintSpring spring,
                                       bool3 constrainedAxes)
        {
            var bits     = math.bitmask(new bool4(constrainedAxes, false));
            var bitCount = math.countbits(bits);
            if (bitCount == 0)
                return;
            var bodyA         = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
            var inertialPoseA = bodyA.inertialPoseWorldTransform;
            var jointLocalA   = math.InverseRotateFast(inertialPoseA.rot, worldSpaceJointRotationForA);

            switch (bitCount)
            {
                case 1:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation1ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  Entity.Null,
                                                                                                  bodyA.bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint1;
                    streamData.indexA = rigidBodyHandleA.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           worldSpaceWorldJointRotation,
                                           quaternion.identity,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping,
                                           math.tzcnt(bits));
                    break;
                }
                case 2:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation2ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  Entity.Null,
                                                                                                  bodyA.bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint2;
                    streamData.indexA = rigidBodyHandleA.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           worldSpaceWorldJointRotation,
                                           quaternion.identity,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping,
                                           math.tzcnt(~bits));
                    break;
                }
                case 3:
                {
                    ref var streamData = ref pairStream.AddPairAndGetRef<Rotation3ConstraintData>(rigidBodyHandleA.entity,
                                                                                                  bodyA.bucketIndex,
                                                                                                  true,
                                                                                                  Entity.Null,
                                                                                                  bodyA.bucketIndex,
                                                                                                  false,
                                                                                                  out var pair);
                    pair.userByte     = SolveByteCodes.rotationConstraint3;
                    streamData.indexA = rigidBodyHandleA.index;
                    UnitySim.BuildJacobian(out streamData.parameters,
                                           inertialPoseA.rot,
                                           jointLocalA,
                                           worldSpaceWorldJointRotation,
                                           quaternion.identity,
                                           minAngle,
                                           maxAngle,
                                           spring.tau,
                                           spring.damping);
                    break;
                }
            }
        }

        public JobHandle TryDispose(JobHandle inputDeps) => inputDeps;  // Always use WorldUpdateAllocator

        public struct ParallelWriter
        {
            internal PairStream.ParallelWriter pairStream;

            public void SpeculateContactsBetween(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                                 in FindPairsResult result,
                                                 in ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                                 in ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                                 float3 contactNormal,
                                                 ReadOnlySpan<UnitySim.ContactsBetweenResult.ContactOnB> contacts,
                                                 float coefficientOfRestitution,
                                                 float coefficientOfFriction,
                                                 float maxDepenetrationVelocity = UnitySim.kMaxDepenetrationVelocityDynamicDynamic)
            {
                CheckEntitiesEqual(rigidBodyHandleA.entity, result.entityA, false);
                CheckEntitiesEqual(rigidBodyHandleB.entity, result.entityB, true);

                var rigidBodyA = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
                var rigidBodyB = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleB.index];

                ref var streamData           = ref pairStream.AddPairAndGetRef<ContactStreamData>(result.pairStreamKey, true, true, out PairStream.Pair pair);
                streamData.contactParameters = pair.Allocate<UnitySim.ContactJacobianContactParameters>(contacts.Length, NativeArrayOptions.UninitializedMemory);
                streamData.contactImpulses   = pair.Allocate<float>(contacts.Length, NativeArrayOptions.ClearMemory);
                streamData.indexA            = rigidBodyHandleA.index;
                streamData.indexB            = rigidBodyHandleB.index;
                pair.userByte                = SolveByteCodes.contactBody;

                UnitySim.BuildJacobian(streamData.contactParameters.AsSpan(),
                                       out streamData.bodyParameters,
                                       rigidBodyA.inertialPoseWorldTransform,
                                       in rigidBodyA.velocity,
                                       in rigidBodyA.mass,
                                       rigidBodyB.inertialPoseWorldTransform,
                                       in rigidBodyB.velocity,
                                       in rigidBodyB.mass,
                                       contactNormal,
                                       contacts,
                                       coefficientOfRestitution,
                                       coefficientOfFriction,
                                       maxDepenetrationVelocity,
                                       math.max(0f, math.max(math.dot(rigidBodyA.gravity, -contactNormal), -math.dot(rigidBodyB.gravity, -contactNormal))),
                                       constraintEntityInfoLookup.constants.deltaTime,
                                       constraintEntityInfoLookup.constants.inverseDeltaTime,
                                       constraintEntityInfoLookup.constants.numSubSteps);
            }

            public void SpeculateContactsBetween(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                                 in FindPairsResult result,
                                                 in ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                                 in ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                                 float3 contactNormal,
                                                 ReadOnlySpan<UnitySim.ContactsBetweenResult.ContactOnB> contacts,
                                                 float coefficientOfRestitution,
                                                 float coefficientOfFriction,
                                                 float maxDepenetrationVelocity = UnitySim.kMaxDepenetrationVelocityDynamicStatic)
            {
                CheckEntitiesEqual(rigidBodyHandleA.entity, result.entityA, false);
                CheckEntitiesEqual(kinematicHandleB.entity, result.entityB, true);

                var rigidBodyA = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];
                var kinematicB = constraintEntityInfoLookup.kinematics.kinematics[kinematicHandleB.index];

                ref var streamData           = ref pairStream.AddPairAndGetRef<ContactStreamData>(result.pairStreamKey, true, false, out PairStream.Pair pair);
                streamData.contactParameters = pair.Allocate<UnitySim.ContactJacobianContactParameters>(contacts.Length, NativeArrayOptions.UninitializedMemory);
                streamData.contactImpulses   = pair.Allocate<float>(contacts.Length, NativeArrayOptions.ClearMemory);
                streamData.indexA            = rigidBodyHandleA.index;
                streamData.indexB            = kinematicHandleB.index;
                pair.userByte                = SolveByteCodes.contactKinematic;

                UnitySim.BuildJacobian(streamData.contactParameters.AsSpan(),
                                       out streamData.bodyParameters,
                                       rigidBodyA.inertialPoseWorldTransform,
                                       in rigidBodyA.velocity,
                                       in rigidBodyA.mass,
                                       kinematicB.inertialPoseWorldTransform,
                                       in kinematicB.velocity,
                                       default,
                                       contactNormal,
                                       contacts,
                                       coefficientOfRestitution,
                                       coefficientOfFriction,
                                       maxDepenetrationVelocity,
                                       math.max(0f, math.dot(rigidBodyA.gravity, -contactNormal)),
                                       constraintEntityInfoLookup.constants.deltaTime,
                                       constraintEntityInfoLookup.constants.inverseDeltaTime,
                                       constraintEntityInfoLookup.constants.numSubSteps);
            }

            public void SpeculateContactsBetween(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                                 in FindPairsResult result,
                                                 in ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                                 float3 contactNormal,
                                                 ReadOnlySpan<UnitySim.ContactsBetweenResult.ContactOnB> contacts,
                                                 float coefficientOfRestitution,
                                                 float coefficientOfFriction,
                                                 float maxDepenetrationVelocity = UnitySim.kMaxDepenetrationVelocityDynamicStatic)
            {
                CheckEntitiesEqual(rigidBodyHandleA.entity, result.entityA, false);

                var rigidBodyA = constraintEntityInfoLookup.rigidBodies.states[rigidBodyHandleA.index];

                ref var streamData           = ref pairStream.AddPairAndGetRef<ContactStreamData>(result.pairStreamKey, true, false, out PairStream.Pair pair);
                streamData.contactParameters = pair.Allocate<UnitySim.ContactJacobianContactParameters>(contacts.Length, NativeArrayOptions.UninitializedMemory);
                streamData.contactImpulses   = pair.Allocate<float>(contacts.Length, NativeArrayOptions.ClearMemory);
                streamData.indexA            = rigidBodyHandleA.index;
                streamData.indexB            = -1;
                pair.userByte                = SolveByteCodes.contactEnvironment;

                UnitySim.BuildJacobian(streamData.contactParameters.AsSpan(),
                                       out streamData.bodyParameters,
                                       rigidBodyA.inertialPoseWorldTransform,
                                       in rigidBodyA.velocity,
                                       in rigidBodyA.mass,
                                       RigidTransform.identity,
                                       default,
                                       default,
                                       contactNormal,
                                       contacts,
                                       coefficientOfRestitution,
                                       coefficientOfFriction,
                                       maxDepenetrationVelocity,
                                       math.max(0f, math.dot(rigidBodyA.gravity, -contactNormal)),
                                       constraintEntityInfoLookup.constants.deltaTime,
                                       constraintEntityInfoLookup.constants.inverseDeltaTime,
                                       constraintEntityInfoLookup.constants.numSubSteps);
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            static void CheckEntitiesEqual(Entity argumentEntity, Entity pairEntity, bool isB)
            {
                if (argumentEntity == pairEntity)
                    return;

                FixedString32Bytes letter = isB ? "B" : "A";
                throw new ArgumentException(
                    $"Passed in RigidBodyHandle{letter} does not correspond to FindPairsResult.entity{letter}. {argumentEntity.ToFixedString()} vs {pairEntity.ToFixedString()}");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckSafeToAccess(in ConstraintWritingConstants constants)
        {
            if (!constants.isInConstraintWritingPhase)
                throw new System.InvalidOperationException("ConstraintWriter must only be created in a system's update within ConstraintWritingSuperSystem");
        }
    }
}

