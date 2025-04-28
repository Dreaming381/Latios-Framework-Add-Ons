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
    }

    struct SolveByteCodes
    {
        public const byte contactEnvironment  = 0;
        public const byte contactKinematic    = 1;
        public const byte contactBody         = 2;
        public const byte positionConstraint  = 3;
        public const byte rotationConstraint1 = 4;
        public const byte rotationConstraint2 = 5;
        public const byte rotationConstraint3 = 6;
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
}

