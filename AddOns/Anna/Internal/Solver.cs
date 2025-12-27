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
        }
    }
}

