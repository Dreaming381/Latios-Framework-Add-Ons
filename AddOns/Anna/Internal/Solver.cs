using Latios.Psyshock;
using Latios.Transforms;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    internal struct SolveBodiesProcessor : IForEachPairProcessor
    {
        [NativeDisableParallelForRestriction] public NativeArray<CapturedRigidBodyState> states;
        [ReadOnly] public NativeArray<CapturedKinematic>                                 kinematics;
        public float                                                                     invNumSolverIterations;
        public float                                                                     deltaTime;
        public float                                                                     inverseDeltaTime;
        public bool                                                                      firstIteration;
        public bool                                                                      lastIteration;

        public InstantiateCommandBuffer<WorldTransform>.ParallelWriter icb;

        public void Execute(ref PairStream.Pair pair)
        {
            var statesSpan = states.AsSpan();

            if (pair.userByte == SolveByteCodes.contactEnvironment)
            {
                ref var           streamData          = ref pair.GetRef<ContactStreamData>();
                ref var           rigidBodyA          = ref statesSpan[streamData.indexA];
                UnitySim.Velocity environmentVelocity = default;
                var               previousVelocity    = rigidBodyA.velocity;
                UnitySim.SolveJacobian(ref rigidBodyA.velocity,
                                       in rigidBodyA.mass,
                                       in rigidBodyA.motionStabilizer,
                                       ref environmentVelocity,
                                       default,
                                       UnitySim.MotionStabilizer.kDefault,
                                       streamData.contactParameters.AsSpan(),
                                       streamData.contactImpulses.AsSpan(),
                                       in streamData.bodyParameters,
                                       true,
                                       invNumSolverIterations,
                                       out _);

                //if (math.distance(previousVelocity.linear.xz, rigidBodyA.velocity.linear.xz) > 0.25f)
                //{
                //    UnityEngine.Debug.Log($"Extreme impulse. Before: {previousVelocity.linear}, after: {rigidBodyA.velocity.linear}");
                //}

                if (firstIteration)
                {
                    if (UnitySim.IsStabilizerSignificantBody(rigidBodyA.mass.inverseMass, 0f))
                        rigidBodyA.numOtherSignificantBodiesInContact++;
                }
            }
            else if (pair.userByte == SolveByteCodes.contactKinematic)
            {
                ref var streamData          = ref pair.GetRef<ContactStreamData>();
                ref var rigidBodyA          = ref statesSpan[streamData.indexA];
                var     environmentVelocity = kinematics[streamData.indexB].velocity;
                UnitySim.SolveJacobian(ref rigidBodyA.velocity,
                                       in rigidBodyA.mass,
                                       in rigidBodyA.motionStabilizer,
                                       ref environmentVelocity,
                                       default,
                                       UnitySim.MotionStabilizer.kDefault,
                                       streamData.contactParameters.AsSpan(),
                                       streamData.contactImpulses.AsSpan(),
                                       in streamData.bodyParameters,
                                       true,
                                       invNumSolverIterations,
                                       out _);

                if (firstIteration)
                {
                    if (UnitySim.IsStabilizerSignificantBody(rigidBodyA.mass.inverseMass, 0f))
                        rigidBodyA.numOtherSignificantBodiesInContact++;
                }
            }
            else if (pair.userByte == SolveByteCodes.contactBody)
            {
                ref var streamData = ref pair.GetRef<ContactStreamData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];
                ref var rigidBodyB = ref statesSpan[streamData.indexB];
                UnitySim.SolveJacobian(ref rigidBodyA.velocity,
                                       in rigidBodyA.mass,
                                       in rigidBodyA.motionStabilizer,
                                       ref rigidBodyB.velocity,
                                       in rigidBodyB.mass,
                                       in rigidBodyB.motionStabilizer,
                                       streamData.contactParameters.AsSpan(),
                                       streamData.contactImpulses.AsSpan(),
                                       in streamData.bodyParameters,
                                       true,
                                       invNumSolverIterations,
                                       out _);
                if (firstIteration)
                {
                    if (UnitySim.IsStabilizerSignificantBody(rigidBodyA.mass.inverseMass, rigidBodyB.mass.inverseMass))
                        rigidBodyA.numOtherSignificantBodiesInContact++;
                    if (UnitySim.IsStabilizerSignificantBody(rigidBodyB.mass.inverseMass, rigidBodyA.mass.inverseMass))
                        rigidBodyB.numOtherSignificantBodiesInContact++;
                }
            }
            else if (pair.userByte == SolveByteCodes.positionConstraint)
            {
                ref var streamData = ref pair.GetRef<PositionConstraintData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];

                UnitySim.Velocity dummyVelocity = default;
                ref var           bVelocity     = ref dummyVelocity;
                var               bTransform    = RigidTransform.identity;
                UnitySim.Mass     bMass         = default;

                if (pair.bIsRW)
                {
                    ref var rigidBodyB = ref statesSpan[streamData.indexB];
                    bTransform         = rigidBodyB.inertialPoseWorldTransform;
                    bVelocity          = ref rigidBodyB.velocity;
                    bMass              = rigidBodyB.mass;
                }
                else if (streamData.indexB >= 0)
                {
                    var kinematic = kinematics[streamData.indexB];
                    bVelocity     = kinematic.velocity;
                    bTransform    = kinematic.inertialPoseWorldTransform;
                }
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.inertialPoseWorldTransform, in rigidBodyA.mass,
                                       ref bVelocity, in bTransform, bMass, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.rotationConstraint1)
            {
                ref var streamData = ref pair.GetRef<Rotation1ConstraintData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];

                UnitySim.Velocity dummyVelocity = default;
                ref var           bVelocity     = ref dummyVelocity;
                UnitySim.Mass     bMass         = default;

                if (pair.bIsRW)
                {
                    ref var rigidBodyB = ref statesSpan[streamData.indexB];
                    bVelocity          = ref rigidBodyB.velocity;
                    bMass              = rigidBodyB.mass;
                }
                else if (streamData.indexB >= 0)
                {
                    bVelocity = kinematics[streamData.indexB].velocity;
                }
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.mass,
                                       ref bVelocity, bMass, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.rotationConstraint2)
            {
                ref var streamData = ref pair.GetRef<Rotation2ConstraintData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];

                UnitySim.Velocity dummyVelocity = default;
                ref var           bVelocity     = ref dummyVelocity;
                UnitySim.Mass     bMass         = default;

                if (pair.bIsRW)
                {
                    ref var rigidBodyB = ref statesSpan[streamData.indexB];
                    bVelocity          = ref rigidBodyB.velocity;
                    bMass              = rigidBodyB.mass;
                }
                else if (streamData.indexB >= 0)
                {
                    bVelocity = kinematics[streamData.indexB].velocity;
                }
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.mass,
                                       ref bVelocity, bMass, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.rotationConstraint3)
            {
                ref var streamData = ref pair.GetRef<Rotation3ConstraintData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];

                UnitySim.Velocity dummyVelocity = default;
                ref var           bVelocity     = ref dummyVelocity;
                UnitySim.Mass     bMass         = default;

                if (pair.bIsRW)
                {
                    ref var rigidBodyB = ref statesSpan[streamData.indexB];
                    bVelocity          = ref rigidBodyB.velocity;
                    bMass              = rigidBodyB.mass;
                }
                else if (streamData.indexB >= 0)
                {
                    bVelocity = kinematics[streamData.indexB].velocity;
                }
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.mass,
                                       ref bVelocity, bMass, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.rotationMotor)
            {
                ref var streamData = ref pair.GetRef<RotationMotorData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];
                GetB(ref pair, streamData.indexB, out var dummyVelocity, out var bIndex, out _, out var bMass);
                ref var bVelocity = ref bIndex >= 0 ? ref statesSpan[bIndex].velocity : ref dummyVelocity;
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.mass, ref bVelocity, in bMass,
                                       ref streamData.accumulatedImpulse, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.angularVelocityMotor)
            {
                ref var streamData = ref pair.GetRef<AngularVelocityMotorData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];
                GetB(ref pair, streamData.indexB, out var dummyVelocity, out var bIndex, out _, out var bMass);
                ref var bVelocity = ref bIndex >= 0 ? ref statesSpan[bIndex].velocity : ref dummyVelocity;
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.mass, ref bVelocity, in bMass,
                                       ref streamData.accumulatedImpulse, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.positionMotor)
            {
                ref var streamData = ref pair.GetRef<PositionMotorData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];
                GetB(ref pair, streamData.indexB, out var dummyVelocity, out var bIndex, out var bTransform, out var bMass);
                ref var bVelocity = ref bIndex >= 0 ? ref statesSpan[bIndex].velocity : ref dummyVelocity;
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.inertialPoseWorldTransform, in rigidBodyA.mass,
                                       ref bVelocity, in bTransform, in bMass,
                                       ref streamData.accumulatedImpulse, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
            else if (pair.userByte == SolveByteCodes.linearVelocityMotor)
            {
                ref var streamData = ref pair.GetRef<LinearVelocityMotorData>();
                ref var rigidBodyA = ref statesSpan[streamData.indexA];
                GetB(ref pair, streamData.indexB, out var dummyVelocity, out var bIndex, out var bTransform, out var bMass);
                ref var bVelocity = ref bIndex >= 0 ? ref statesSpan[bIndex].velocity : ref dummyVelocity;
                UnitySim.SolveJacobian(ref rigidBodyA.velocity, in rigidBodyA.inertialPoseWorldTransform, in rigidBodyA.mass,
                                       ref bVelocity, in bTransform, in bMass,
                                       ref streamData.accumulatedImpulse, in streamData.parameters, deltaTime, inverseDeltaTime);
            }
        }

        // For B that isn't a rigid body, bIndex is -1 and velocity holds the kinematic's velocity, or zero for the world.
        void GetB(ref PairStream.Pair pair, int indexB, out UnitySim.Velocity velocity, out int bIndex, out RigidTransform transform, out UnitySim.Mass mass)
        {
            velocity  = default;
            transform = RigidTransform.identity;
            mass      = default;
            bIndex    = -1;
            if (pair.bIsRW)
            {
                ref var rigidBodyB = ref states.AsSpan()[indexB];
                transform          = rigidBodyB.inertialPoseWorldTransform;
                mass               = rigidBodyB.mass;
                bIndex             = indexB;
            }
            else if (indexB >= 0)
            {
                var kinematic = kinematics[indexB];
                velocity      = kinematic.velocity;
                transform     = kinematic.inertialPoseWorldTransform;
            }
        }
    }
}

