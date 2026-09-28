using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    internal struct CapturedRigidBodyState
    {
        public UnitySim.Velocity         velocity;
        public UnitySim.MotionExpansion  motionExpansion;
        public RigidTransform            inertialPoseWorldTransform;
        public UnitySim.Mass             mass;
        public UnitySim.MotionStabilizer motionStabilizer;
        public float3                    gravity;
        public float                     angularExpansion;
        public int                       bucketIndex;
        public int                       numOtherSignificantBodiesInContact;
        public half                      coefficientOfFriction;
        public half                      coefficientOfRestitution;
        public half                      linearDamping;
        public half                      angularDamping;
    }

    internal struct CapturedKinematic
    {
        public UnitySim.Velocity        velocity;
        public RigidTransform           inertialPoseWorldTransform;
        public UnitySim.MotionExpansion motionExpansion;
        public int                      bucketIndex;
    }

    struct SolveByteCodes
    {
        public const byte contactEnvironment  = 0;
        public const byte contactKinematic    = 1;
        public const byte contactBody         = 2;
        public const byte positionConstraint  = 3;
        public const byte rotationConstraint1 = 4;
        public const byte rotationConstraint2 = 5;
        public const byte rotationConstraint3  = 6;
        public const byte rotationMotor        = 7;
        public const byte angularVelocityMotor = 8;
        public const byte positionMotor        = 9;
        public const byte linearVelocityMotor  = 10;
    }

    struct ContactStreamData
    {
        public int                                                   indexA;
        public int                                                   indexB;
        public UnitySim.ContactJacobianBodyParameters                bodyParameters;
        public StreamSpan<UnitySim.ContactJacobianContactParameters> contactParameters;
        public StreamSpan<float>                                     contactImpulses;
    }

    struct PositionConstraintData
    {
        public int                                           indexA;
        public int                                           indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.PositionConstraintJacobianParameters parameters;
    }

    struct Rotation1ConstraintData
    {
        public int                                             indexA;
        public int                                             indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.Rotation1DConstraintJacobianParameters parameters;
    }

    struct Rotation2ConstraintData
    {
        public int                                             indexA;
        public int                                             indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.Rotation2DConstraintJacobianParameters parameters;
    }

    struct Rotation3ConstraintData
    {
        public int                                             indexA;
        public int                                             indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.Rotation3DConstraintJacobianParameters parameters;
    }

    // Motors accumulate impulse across solver iterations so that the total stays under their cap.
    struct RotationMotorData
    {
        public int                                         indexA;
        public int                                         indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.Rotation1DMotorJacobianParameters parameters;
        public float                                       accumulatedImpulse;
    }

    struct AngularVelocityMotorData
    {
        public int                                               indexA;
        public int                                               indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.Angular1DVelocityMotorJacobianParameters parameters;
        public float                                             accumulatedImpulse;
    }

    struct PositionMotorData
    {
        public int                                         indexA;
        public int                                         indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.Position1DMotorJacobianParameters parameters;
        public float3                                      accumulatedImpulse;
    }

    struct LinearVelocityMotorData
    {
        public int                                               indexA;
        public int                                               indexB;  // negative and bIsRO => environment, otherwise bIsRO => kinematic
        public UnitySim.LinearVelocity1DMotorJacobianParameters parameters;
        public float                                             accumulatedImpulse;
    }
}

