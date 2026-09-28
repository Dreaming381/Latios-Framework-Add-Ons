using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna.Systems
{
    [RequireMatchingQueriesForUpdate]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct WriteJointConstraintsSystem : ISystem, ILatiosApi
    {
        LatiosWorldUnmanaged latiosWorld;
        BlackboardEntity     systemBlackboard;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.OnCreateForLatios(ref state);
            latiosWorld      = state.GetLatiosWorldUnmanaged();
            systemBlackboard = new BlackboardEntity(state.SystemHandle, latiosWorld);
            systemBlackboard.AddOrSetCollectionComponentAndDisposeOld(new ConstraintWriter());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var infoLookup       = latiosWorld.sceneBlackboardEntity.GetCollectionAspect<ConstraintEntityInfoLookup>();
            var constraintWriter = new ConstraintWriter(ref state, latiosWorld);
            new Job
            {
                infoLookup       = infoLookup,
                constraintWriter = constraintWriter,
                transformLookup  = SystemAPI.GetComponentLookup<WorldTransform>(true),
                deltaTime        = SystemAPI.Time.DeltaTime
            }.Schedule(this.GetApi(ref state));

            systemBlackboard.SetCollectionComponentAndDisposeOld(constraintWriter);
        }

        [BurstCompile]
        partial struct Job : IJobEach
        {
            [ReadOnly] public ConstraintEntityInfoLookup      infoLookup;
            [ReadOnly] public ComponentLookup<WorldTransform> transformLookup;
            public ConstraintWriter                           constraintWriter;
            public float                                      deltaTime;

            public void Execute(in DynamicBuffer<JointConstraint> joints)
            {
                foreach (var joint in joints)
                {
                    if (!infoLookup.TryGetRigidBodyHandle(joint.entityA, out var handle))
                        continue;
                    if (!transformLookup.TryGetComponent(joint.entityA, out var transformA))
                        continue;

                    var frameA = ToWorld(in transformA.worldTransform, joint.weldInLocalA);
                    var spring = infoLookup.CreateSpring(joint.springFrequency, joint.dampingRatio);

                    if (joint.entityB == Entity.Null)
                    {
                        WriteToWorld(handle, frameA, joint.weldInLocalB, in joint, spring);
                        continue;
                    }
                    if (!transformLookup.TryGetComponent(joint.entityB, out var transformB))
                        continue;

                    var frameB = ToWorld(in transformB.worldTransform, joint.weldInLocalB);
                    if (infoLookup.TryGetRigidBodyHandle(joint.entityB, out var rigidBodyB))
                        WriteToRigidBody(handle, rigidBodyB, frameA, frameB, in joint, spring);
                    else if (infoLookup.TryGetKinematicHandle(joint.entityB, out var kinematicB))
                        WriteToKinematic(handle, kinematicB, frameA, frameB, in joint, spring);
                    else
                        WriteToWorld(handle, frameA, frameB, in joint, spring);
                }
            }

            static RigidTransform ToWorld(in TransformQvvs transform, RigidTransform localFrame)
            {
                return new RigidTransform(math.mul(transform.rotation, localFrame.rot), qvvs.TransformPoint(in transform, localFrame.pos));
            }

            static int AxisIndex(bool3 axes) => math.tzcnt(math.bitmask(new bool4(axes, false)));

            void WriteToRigidBody(ConstraintEntityInfoLookup.RigidBodyHandle handle,
                                  ConstraintEntityInfoLookup.RigidBodyHandle rigidBodyB,
                                  RigidTransform frameA,
                                  RigidTransform frameB,
                                  in JointConstraint joint,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring)
            {
                var axis       = AxisIndex(joint.constrainedAxes);
                var maxImpulse = joint.maxForce * deltaTime;
                switch (joint.type)
                {
                    case JointConstraintType.Position:
                        constraintWriter.ConstrainPositions(ref infoLookup, handle, rigidBodyB, frameA.pos, frameB, joint.min, joint.max, spring, joint.constrainedAxes);
                        break;
                    case JointConstraintType.Rotation:
                        constraintWriter.ConstrainRotations(ref infoLookup, handle, rigidBodyB, frameA.rot, frameB.rot, joint.min, joint.max, spring, joint.constrainedAxes);
                        break;
                    case JointConstraintType.RotationMotor:
                        constraintWriter.DriveRotation(ref infoLookup, handle, rigidBodyB, frameA.rot, frameB.rot, joint.target, joint.min, joint.max, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.AngularVelocityMotor:
                        constraintWriter.DriveAngularVelocity(ref infoLookup, handle, rigidBodyB, frameA.rot, joint.target, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.PositionMotor:
                        constraintWriter.DrivePosition(ref infoLookup, handle, rigidBodyB, frameA.pos, frameB, joint.target, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.LinearVelocityMotor:
                        constraintWriter.DriveLinearVelocity(ref infoLookup, handle, rigidBodyB, frameA.pos, frameB, joint.target, maxImpulse, spring, axis);
                        break;
                }
            }

            void WriteToKinematic(ConstraintEntityInfoLookup.RigidBodyHandle handle,
                                  ConstraintEntityInfoLookup.KinematicHandle kinematicB,
                                  RigidTransform frameA,
                                  RigidTransform frameB,
                                  in JointConstraint joint,
                                  ConstraintEntityInfoLookup.ConstraintSpring spring)
            {
                var axis       = AxisIndex(joint.constrainedAxes);
                var maxImpulse = joint.maxForce * deltaTime;
                switch (joint.type)
                {
                    case JointConstraintType.Position:
                        constraintWriter.ConstrainPositions(ref infoLookup, handle, kinematicB, frameA.pos, frameB, joint.min, joint.max, spring, joint.constrainedAxes);
                        break;
                    case JointConstraintType.Rotation:
                        constraintWriter.ConstrainRotations(ref infoLookup, handle, kinematicB, frameA.rot, frameB.rot, joint.min, joint.max, spring, joint.constrainedAxes);
                        break;
                    case JointConstraintType.RotationMotor:
                        constraintWriter.DriveRotation(ref infoLookup, handle, kinematicB, frameA.rot, frameB.rot, joint.target, joint.min, joint.max, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.AngularVelocityMotor:
                        constraintWriter.DriveAngularVelocity(ref infoLookup, handle, kinematicB, frameA.rot, joint.target, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.PositionMotor:
                        constraintWriter.DrivePosition(ref infoLookup, handle, kinematicB, frameA.pos, frameB, joint.target, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.LinearVelocityMotor:
                        constraintWriter.DriveLinearVelocity(ref infoLookup, handle, kinematicB, frameA.pos, frameB, joint.target, maxImpulse, spring, axis);
                        break;
                }
            }

            void WriteToWorld(ConstraintEntityInfoLookup.RigidBodyHandle handle,
                              RigidTransform frameA,
                              RigidTransform worldFrame,
                              in JointConstraint joint,
                              ConstraintEntityInfoLookup.ConstraintSpring spring)
            {
                var axis       = AxisIndex(joint.constrainedAxes);
                var maxImpulse = joint.maxForce * deltaTime;
                switch (joint.type)
                {
                    case JointConstraintType.Position:
                        constraintWriter.ConstrainPositions(ref infoLookup, handle, frameA.pos, worldFrame, joint.min, joint.max, spring, joint.constrainedAxes);
                        break;
                    case JointConstraintType.Rotation:
                        constraintWriter.ConstrainRotations(ref infoLookup, handle, frameA.rot, worldFrame.rot, joint.min, joint.max, spring, joint.constrainedAxes);
                        break;
                    case JointConstraintType.RotationMotor:
                        constraintWriter.DriveRotation(ref infoLookup, handle, frameA.rot, worldFrame.rot, joint.target, joint.min, joint.max, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.AngularVelocityMotor:
                        constraintWriter.DriveAngularVelocity(ref infoLookup, handle, frameA.rot, joint.target, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.PositionMotor:
                        constraintWriter.DrivePosition(ref infoLookup, handle, frameA.pos, worldFrame, joint.target, maxImpulse, spring, axis);
                        break;
                    case JointConstraintType.LinearVelocityMotor:
                        constraintWriter.DriveLinearVelocity(ref infoLookup, handle, frameA.pos, worldFrame, joint.target, maxImpulse, spring, axis);
                        break;
                }
            }
        }
    }
}
