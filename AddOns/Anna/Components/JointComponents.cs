using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Anna
{
    public enum JointConstraintType : byte
    {
        /// <summary>
        /// Limits the distance between the two joint weld positions. With one constrained axis, the distance is measured
        /// along that axis of the connected weld. With two, it is measured across their plane. With three, it is the full
        /// distance.
        /// </summary>
        Position,
        /// <summary>
        /// Limits the angle between the two joint weld orientations. With one constrained axis, this is the twist about that
        /// axis. With two, it is the angle between the free axes of each weld, which makes a cone. With three, it is the full
        /// angle between the welds.
        /// </summary>
        Rotation,
        /// <summary>
        /// Drives the twist angle about the single constrained axis toward a target value
        /// </summary>
        RotationMotor,
        /// <summary>
        /// Drives the relative angular velocity about the single constrained axis of the weld on A toward a target value
        /// per second.
        /// </summary>
        AngularVelocityMotor,
        /// <summary>
        /// Drives the offset between the joint positions along the single constrained axis of the weld on B toward a target value
        /// </summary>
        PositionMotor,
        /// <summary>
        /// Drives A's velocity along the single constrained axis of the weld on B toward a target value
        /// </summary>
        LinearVelocityMotor,
    }

    /// <summary>
    /// A constraint between a rigid body and another entity (or the world), which Anna feeds to its solver every update.
    /// The buffer can live on any entity.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct JointConstraint : IBufferElementData
    {
        /// <summary>
        /// The rigid body the constraint acts on. If this entity is not a rigid body, the constraint is skipped.
        /// </summary>
        public Entity entityA;
        /// <summary>
        /// The other entity in the joint. Entity.Null anchors the joint in world space. If the entity is neither a rigid body
        /// nor a kinematic collider, its current WorldTransform is treated as immovable.
        /// </summary>
        public Entity entityB;
        /// <summary>
        /// The position and orientation of axes of the joint in entityA's local space.
        /// </summary>
        public RigidTransform weldInLocalA;
        /// <summary>
        /// The position and orientation of axes of the joint in entityB's local space. The is the absolute world-space transform if entityB is Entity.Null.
        /// </summary>
        public RigidTransform weldInLocalB;
        /// <summary>
        /// The minimum distance for a position constraint, or the minimum angle in radians for a rotation constraint.
        /// </summary>
        public float min;
        /// <summary>
        /// The maximum distance for a position constraint, or the maximum angle in radians for a rotation constraint.
        /// </summary>
        public float max;
        /// <summary>
        /// How quickly the constraint pulls back within its limits. Use UnitySim.kStiffSpringFrequency for a rigid limit.
        /// </summary>
        public float springFrequency;
        /// <summary>
        /// How much the spring resists oscillating. Use UnitySim.kStiffDampingRatio for a rigid limit.
        /// </summary>
        public float dampingRatio;
        /// <summary>
        /// For motors, the angle (radians), angular velocity (rad/s), offset (meters), or velocity (m/s) to drive toward. Ignored for non-motors.
        /// </summary>
        public float target;
        /// <summary>
        /// For motors, the largest force (or torque) the motor may apply. Use float.PositiveInfinity for no limit. Ignored for non-motors.
        /// </summary>
        public float maxForce;
        /// <summary>
        /// The axes of the joint weld that this constraint acts on. Motors need exactly one axis.
        /// </summary>
        public bool3 constrainedAxes;
        /// <summary>
        /// The type of joint used by this constraint
        /// </summary>
        public JointConstraintType type;
        /// <summary>
        /// When false, entityA and entityB don't collide with each other.
        /// </summary>
        public bool enableCollision;
    }
}

