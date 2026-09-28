using Latios.Psyshock;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    public partial struct ConstraintWriter
    {
        /// <summary>
        /// Drives the twist angle between the joint welds about a single axis toward a target angle, while keeping it within limits
        /// </summary>
        /// <param name="constraintEntityInfoLookup">Constraint-writing context from the sceneBlackboardEntity</param>
        /// <param name="rigidBodyHandleA">The rigid body that is driven</param>
        /// <param name="rigidBodyHandleB">The rigid body the motor pushes against</param>
        /// <param name="worldSpaceJointRotationForA">The world-space joint weld orientation attached to A</param>
        /// <param name="worldSpaceJointRotationForB">The world-space joint weld orientation attached to B</param>
        /// <param name="targetAngle">The angle in radians the motor drives toward</param>
        /// <param name="minAngle">The minimum angle in radians the joint is allowed to reach</param>
        /// <param name="maxAngle">The maximum angle in radians the joint is allowed to reach</param>
        /// <param name="maxImpulse">The largest impulse the motor may apply in this update</param>
        /// <param name="spring">How strongly the motor pulls toward the target</param>
        /// <param name="axisIndex">The joint weld axis to drive about (0 = x, 1 = y, 2 = z)</param>
        public void DriveRotation(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                  quaternion worldSpaceJointRotationForA,
                                  quaternion worldSpaceJointRotationForB,
                                  float targetAngle,
                                  float minAngle,
                                  float maxAngle,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            DriveRotation(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, rigidBodyHandleB),
                          worldSpaceJointRotationForA, worldSpaceJointRotationForB, targetAngle, minAngle, maxAngle, maxImpulse, spring, axisIndex);
        }

        /// <inheritdoc cref="DriveRotation(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, quaternion, quaternion, float, float, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DriveRotation(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                  ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                  quaternion worldSpaceJointRotationForA,
                                  quaternion worldSpaceJointRotationForB,
                                  float targetAngle,
                                  float minAngle,
                                  float maxAngle,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            DriveRotation(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, kinematicHandleB),
                          worldSpaceJointRotationForA, worldSpaceJointRotationForB, targetAngle, minAngle, maxAngle, maxImpulse, spring, axisIndex);
        }

        /// <summary>
        /// Drives the twist angle between the joint weld on A and a fixed world-space weld about a single axis toward a target angle,
        /// while keeping it within limits
        /// </summary>
        /// <param name="worldSpaceWorldJointRotation">The fixed world-space joint weld orientation A is driven relative to</param>
        /// <inheritdoc cref="DriveRotation(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, quaternion, quaternion, float, float, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DriveRotation(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                  quaternion worldSpaceJointRotationForA,
                                  quaternion worldSpaceWorldJointRotation,
                                  float targetAngle,
                                  float minAngle,
                                  float maxAngle,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            DriveRotation(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.World(ref constraintEntityInfoLookup, rigidBodyHandleA),
                          worldSpaceJointRotationForA, worldSpaceWorldJointRotation, targetAngle, minAngle, maxAngle, maxImpulse, spring, axisIndex);
        }

        /// <summary>
        /// Drives the relative angular velocity between the bodies about a single axis of the joint weld on A
        /// </summary>
        /// <param name="constraintEntityInfoLookup">Constraint-writing context from the sceneBlackboardEntity</param>
        /// <param name="rigidBodyHandleA">The rigid body that is driven</param>
        /// <param name="rigidBodyHandleB">The rigid body the motor pushes against</param>
        /// <param name="worldSpaceJointRotationForA">The world-space joint weld orientation attached to A</param>
        /// <param name="targetAngularVelocity">The angular velocity in radians per second the motor drives toward</param>
        /// <param name="maxImpulse">The largest impulse the motor may apply in this update</param>
        /// <param name="spring">How quickly the motor corrects the velocity. Only the damping part is used.</param>
        /// <param name="axisIndex">The joint weld axis to drive about (0 = x, 1 = y, 2 = z)</param>
        public void DriveAngularVelocity(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                         ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                         ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                         quaternion worldSpaceJointRotationForA,
                                         float targetAngularVelocity,
                                         float maxImpulse,
                                         ConstraintEntityInfoLookup.ConstraintSpring spring,
                                         int axisIndex)
        {
            DriveAngularVelocity(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, rigidBodyHandleB),
                                 worldSpaceJointRotationForA, targetAngularVelocity, maxImpulse, spring, axisIndex);
        }

        /// <inheritdoc cref="DriveAngularVelocity(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, quaternion, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DriveAngularVelocity(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                         ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                         ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                         quaternion worldSpaceJointRotationForA,
                                         float targetAngularVelocity,
                                         float maxImpulse,
                                         ConstraintEntityInfoLookup.ConstraintSpring spring,
                                         int axisIndex)
        {
            DriveAngularVelocity(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, kinematicHandleB),
                                 worldSpaceJointRotationForA, targetAngularVelocity, maxImpulse, spring, axisIndex);
        }

        /// <summary>
        /// Drives the angular velocity of A relative to the world about a single axis of the joint weld on A
        /// </summary>
        /// <inheritdoc cref="DriveAngularVelocity(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, quaternion, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DriveAngularVelocity(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                         ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                         quaternion worldSpaceJointRotationForA,
                                         float targetAngularVelocity,
                                         float maxImpulse,
                                         ConstraintEntityInfoLookup.ConstraintSpring spring,
                                         int axisIndex)
        {
            DriveAngularVelocity(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.World(ref constraintEntityInfoLookup, rigidBodyHandleA),
                                 worldSpaceJointRotationForA, targetAngularVelocity, maxImpulse, spring, axisIndex);
        }

        /// <summary>
        /// Drives the joint position on A toward a target offset along a single axis of the joint weld on B
        /// </summary>
        /// <param name="constraintEntityInfoLookup">Constraint-writing context from the sceneBlackboardEntity</param>
        /// <param name="rigidBodyHandleA">The rigid body that is driven</param>
        /// <param name="rigidBodyHandleB">The rigid body the motor pushes against</param>
        /// <param name="worldSpaceJointPositionOnA">The world-space joint position attached to A</param>
        /// <param name="worldSpaceJointPositionAndOrientationOnB">The world-space joint weld attached to B, whose axes the motor drives along</param>
        /// <param name="targetDistance">The signed offset along the axis the motor drives toward</param>
        /// <param name="maxImpulse">The largest impulse the motor may apply in this update</param>
        /// <param name="spring">How strongly the motor pulls toward the target</param>
        /// <param name="axisIndex">The joint weld axis to drive along (0 = x, 1 = y, 2 = z)</param>
        public void DrivePosition(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                  float3 worldSpaceJointPositionOnA,
                                  RigidTransform worldSpaceJointPositionAndOrientationOnB,
                                  float targetDistance,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            DrivePosition(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, rigidBodyHandleB),
                          worldSpaceJointPositionOnA, worldSpaceJointPositionAndOrientationOnB, targetDistance, maxImpulse, spring, axisIndex, false);
        }

        /// <inheritdoc cref="DrivePosition(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, float3, RigidTransform, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DrivePosition(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                  ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                  float3 worldSpaceJointPositionOnA,
                                  RigidTransform worldSpaceJointPositionAndOrientationOnB,
                                  float targetDistance,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            DrivePosition(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, kinematicHandleB),
                          worldSpaceJointPositionOnA, worldSpaceJointPositionAndOrientationOnB, targetDistance, maxImpulse, spring, axisIndex, false);
        }

        /// <summary>
        /// Drives the joint position on A toward a target offset along a single axis of a fixed world-space weld
        /// </summary>
        /// <param name="worldSpaceWorldJointPositionAndOrientation">The fixed world-space joint weld whose axes the motor drives along</param>
        /// <inheritdoc cref="DrivePosition(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, float3, RigidTransform, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DrivePosition(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                  float3 worldSpaceJointPositionOnA,
                                  RigidTransform worldSpaceWorldJointPositionAndOrientation,
                                  float targetDistance,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            DrivePosition(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.World(ref constraintEntityInfoLookup, rigidBodyHandleA),
                          worldSpaceJointPositionOnA, worldSpaceWorldJointPositionAndOrientation, targetDistance, maxImpulse, spring, axisIndex, false);
        }

        /// <summary>
        /// Drives the velocity of A along a single axis of the joint weld on B
        /// </summary>
        /// <param name="constraintEntityInfoLookup">Constraint-writing context from the sceneBlackboardEntity</param>
        /// <param name="rigidBodyHandleA">The rigid body that is driven</param>
        /// <param name="rigidBodyHandleB">The rigid body the motor pushes against</param>
        /// <param name="worldSpaceJointPositionOnA">The world-space joint position attached to A</param>
        /// <param name="worldSpaceJointPositionAndOrientationOnB">The world-space joint weld attached to B, whose axes the motor drives along</param>
        /// <param name="targetVelocity">The velocity in meters per second the motor drives toward</param>
        /// <param name="maxImpulse">The largest impulse the motor may apply in this update</param>
        /// <param name="spring">How quickly the motor corrects the velocity. Only the damping part is used.</param>
        /// <param name="axisIndex">The joint weld axis to drive along (0 = x, 1 = y, 2 = z)</param>
        public void DriveLinearVelocity(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                        ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                        ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleB,
                                        float3 worldSpaceJointPositionOnA,
                                        RigidTransform worldSpaceJointPositionAndOrientationOnB,
                                        float targetVelocity,
                                        float maxImpulse,
                                        ConstraintEntityInfoLookup.ConstraintSpring spring,
                                        int axisIndex)
        {
            DrivePosition(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, rigidBodyHandleB),
                          worldSpaceJointPositionOnA, worldSpaceJointPositionAndOrientationOnB, targetVelocity, maxImpulse, spring, axisIndex, true);
        }

        /// <inheritdoc cref="DriveLinearVelocity(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, float3, RigidTransform, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DriveLinearVelocity(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                        ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                        ConstraintEntityInfoLookup.KinematicHandle kinematicHandleB,
                                        float3 worldSpaceJointPositionOnA,
                                        RigidTransform worldSpaceJointPositionAndOrientationOnB,
                                        float targetVelocity,
                                        float maxImpulse,
                                        ConstraintEntityInfoLookup.ConstraintSpring spring,
                                        int axisIndex)
        {
            DrivePosition(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.From(ref constraintEntityInfoLookup, kinematicHandleB),
                          worldSpaceJointPositionOnA, worldSpaceJointPositionAndOrientationOnB, targetVelocity, maxImpulse, spring, axisIndex, true);
        }

        /// <summary>
        /// Drives the velocity of A along a single axis of a fixed world-space weld
        /// </summary>
        /// <param name="worldSpaceWorldJointPositionAndOrientation">The fixed world-space joint weld whose axes the motor drives along</param>
        /// <inheritdoc cref="DriveLinearVelocity(ref ConstraintEntityInfoLookup, ConstraintEntityInfoLookup.RigidBodyHandle, ConstraintEntityInfoLookup.RigidBodyHandle, float3, RigidTransform, float, float, ConstraintEntityInfoLookup.ConstraintSpring, int)"/>
        public void DriveLinearVelocity(ref ConstraintEntityInfoLookup constraintEntityInfoLookup,
                                        ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyHandleA,
                                        float3 worldSpaceJointPositionOnA,
                                        RigidTransform worldSpaceWorldJointPositionAndOrientation,
                                        float targetVelocity,
                                        float maxImpulse,
                                        ConstraintEntityInfoLookup.ConstraintSpring spring,
                                        int axisIndex)
        {
            DrivePosition(ref constraintEntityInfoLookup, rigidBodyHandleA, MotorBodyB.World(ref constraintEntityInfoLookup, rigidBodyHandleA),
                          worldSpaceJointPositionOnA, worldSpaceWorldJointPositionAndOrientation, targetVelocity, maxImpulse, spring, axisIndex, true);
        }

        struct MotorBodyB
        {
            public Entity         entity;
            public RigidTransform inertialPose;
            public int            bucketIndex;
            public int            index;
            public bool           isRigidBody;

            public static MotorBodyB From(ref ConstraintEntityInfoLookup lookup, ConstraintEntityInfoLookup.RigidBodyHandle handle)
            {
                var body = lookup.rigidBodies.states[handle.index];
                return new MotorBodyB
                {
                    entity       = handle.entity,
                    inertialPose = body.inertialPoseWorldTransform,
                    bucketIndex  = body.bucketIndex,
                    index        = handle.index,
                    isRigidBody  = true
                };
            }

            public static MotorBodyB From(ref ConstraintEntityInfoLookup lookup, ConstraintEntityInfoLookup.KinematicHandle handle)
            {
                var body = lookup.kinematics.kinematics[handle.index];
                return new MotorBodyB
                {
                    entity       = handle.entity,
                    inertialPose = body.inertialPoseWorldTransform,
                    bucketIndex  = body.bucketIndex,
                    index        = handle.index,
                    isRigidBody  = false
                };
            }

            public static MotorBodyB World(ref ConstraintEntityInfoLookup lookup, ConstraintEntityInfoLookup.RigidBodyHandle handleA)
            {
                return new MotorBodyB
                {
                    entity       = Entity.Null,
                    inertialPose = RigidTransform.identity,
                    bucketIndex  = lookup.rigidBodies.states[handleA.index].bucketIndex,
                    index        = -1,
                    isRigidBody  = false
                };
            }
        }

        void DriveRotation(ref ConstraintEntityInfoLookup lookup,
                           ConstraintEntityInfoLookup.RigidBodyHandle handleA,
                           MotorBodyB bodyB,
                           quaternion worldSpaceJointRotationForA,
                           quaternion worldSpaceJointRotationForB,
                           float targetAngle,
                           float minAngle,
                           float maxAngle,
                           float maxImpulse,
                           ConstraintEntityInfoLookup.ConstraintSpring spring,
                           int axisIndex)
        {
            var     bodyA      = lookup.rigidBodies.states[handleA.index];
            ref var streamData = ref pairStream.AddPairAndGetRef<RotationMotorData>(handleA.entity, bodyA.bucketIndex, true,
                                                                                    bodyB.entity, bodyB.bucketIndex, bodyB.isRigidBody, out var pair);
            pair.userByte                 = SolveByteCodes.rotationMotor;
            streamData.indexA             = handleA.index;
            streamData.indexB             = bodyB.index;
            streamData.accumulatedImpulse = 0f;
            UnitySim.BuildJacobian(out streamData.parameters,
                                   bodyA.inertialPoseWorldTransform.rot,
                                   math.InverseRotateFast(bodyA.inertialPoseWorldTransform.rot, worldSpaceJointRotationForA),
                                   bodyB.inertialPose.rot,
                                   math.InverseRotateFast(bodyB.inertialPose.rot, worldSpaceJointRotationForB),
                                   targetAngle,
                                   maxImpulse,
                                   minAngle,
                                   maxAngle,
                                   spring.tau,
                                   spring.damping,
                                   axisIndex);
        }

        void DriveAngularVelocity(ref ConstraintEntityInfoLookup lookup,
                                  ConstraintEntityInfoLookup.RigidBodyHandle handleA,
                                  MotorBodyB bodyB,
                                  quaternion worldSpaceJointRotationForA,
                                  float targetAngularVelocity,
                                  float maxImpulse,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring,
                                  int axisIndex)
        {
            var     bodyA      = lookup.rigidBodies.states[handleA.index];
            ref var streamData = ref pairStream.AddPairAndGetRef<AngularVelocityMotorData>(handleA.entity, bodyA.bucketIndex, true,
                                                                                           bodyB.entity, bodyB.bucketIndex, bodyB.isRigidBody, out var pair);
            pair.userByte                 = SolveByteCodes.angularVelocityMotor;
            streamData.indexA             = handleA.index;
            streamData.indexB             = bodyB.index;
            streamData.accumulatedImpulse = 0f;
            UnitySim.BuildJacobian(out streamData.parameters,
                                   bodyA.inertialPoseWorldTransform.rot,
                                   math.InverseRotateFast(bodyA.inertialPoseWorldTransform.rot, worldSpaceJointRotationForA),
                                   bodyB.inertialPose.rot,
                                   targetAngularVelocity,
                                   maxImpulse,
                                   spring.damping,
                                   axisIndex);
        }

        void DrivePosition(ref ConstraintEntityInfoLookup lookup,
                           ConstraintEntityInfoLookup.RigidBodyHandle handleA,
                           MotorBodyB bodyB,
                           float3 worldSpaceJointPositionOnA,
                           RigidTransform worldSpaceJointPositionAndOrientationOnB,
                           float target,
                           float maxImpulse,
                           ConstraintEntityInfoLookup.ConstraintSpring spring,
                           int axisIndex,
                           bool drivesVelocity)
        {
            var bodyA       = lookup.rigidBodies.states[handleA.index];
            var poseA       = bodyA.inertialPoseWorldTransform;
            var jointLocalA = math.InverseRotateFast(poseA.rot, worldSpaceJointPositionOnA - poseA.pos);
            var jointLocalB = math.mul(math.inverse(bodyB.inertialPose), worldSpaceJointPositionAndOrientationOnB);
            if (drivesVelocity)
            {
                ref var streamData = ref pairStream.AddPairAndGetRef<LinearVelocityMotorData>(handleA.entity, bodyA.bucketIndex, true,
                                                                                              bodyB.entity, bodyB.bucketIndex, bodyB.isRigidBody, out var pair);
                pair.userByte                 = SolveByteCodes.linearVelocityMotor;
                streamData.indexA             = handleA.index;
                streamData.indexB             = bodyB.index;
                streamData.accumulatedImpulse = 0f;
                UnitySim.BuildJacobian(out streamData.parameters, poseA, jointLocalA, bodyB.inertialPose, jointLocalB, target, maxImpulse, axisIndex,
                                       spring.tau, spring.damping);
            }
            else
            {
                ref var streamData = ref pairStream.AddPairAndGetRef<PositionMotorData>(handleA.entity, bodyA.bucketIndex, true,
                                                                                        bodyB.entity, bodyB.bucketIndex, bodyB.isRigidBody, out var pair);
                pair.userByte                 = SolveByteCodes.positionMotor;
                streamData.indexA             = handleA.index;
                streamData.indexB             = bodyB.index;
                streamData.accumulatedImpulse = 0f;
                UnitySim.BuildJacobian(out streamData.parameters, poseA, jointLocalA, bodyB.inertialPose, jointLocalB, target, maxImpulse, axisIndex,
                                       spring.tau, spring.damping);
            }
        }
    }
}

