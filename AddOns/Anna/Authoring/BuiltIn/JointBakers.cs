using System.Collections.Generic;
using Latios.Psyshock;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Anna.Authoring
{
    [DisableAutoCreation]
    internal class RigidbodyJointsBaker : Baker<Rigidbody>
    {
        static readonly FixedJointConverter        sFixed        = new FixedJointConverter();
        static readonly HingeJointConverter        sHinge        = new HingeJointConverter();
        static readonly CharacterJointConverter    sCharacter    = new CharacterJointConverter();
        static readonly ConfigurableJointConverter sConfigurable = new ConfigurableJointConverter();
        static readonly SpringJointConverter       sSpring       = new SpringJointConverter();

        List<UnityEngine.Joint> m_joints      = new List<UnityEngine.Joint>();
        List<JointConstraint>   m_constraints = new List<JointConstraint>();

        public override void Bake(Rigidbody authoring)
        {
            m_joints.Clear();
            m_constraints.Clear();
            GetComponents(m_joints);
            foreach (var joint in m_joints)
            {
                switch (joint)
                {
                    case FixedJoint fixedJoint: sFixed.Convert(this, fixedJoint, m_constraints); break;
                    case HingeJoint hingeJoint: sHinge.Convert(this, hingeJoint, m_constraints); break;
                    case CharacterJoint characterJoint: sCharacter.Convert(this, characterJoint, m_constraints); break;
                    case ConfigurableJoint configurableJoint: sConfigurable.Convert(this, configurableJoint, m_constraints); break;
                    case SpringJoint springJoint: sSpring.Convert(this, springJoint, m_constraints); break;
                }
            }
            if (m_constraints.Count == 0)
                return;

            var buffer = AddBuffer<JointConstraint>(GetEntity(TransformUsageFlags.Dynamic));
            foreach (var constraint in m_constraints)
                buffer.Add(constraint);
        }
    }

    internal abstract class AnnaJointConverter<T> where T : UnityEngine.Joint
    {
        IBaker                m_baker;
        List<JointConstraint> m_constraints;
        JointConstraint       m_template;
        bool                  m_swapped;
        float                 m_constrainedMass;
        float                 m_constrainedInertia;

        protected virtual quaternion GetLocalFrameOrientation(T joint) => quaternion.identity;
        protected virtual bool IsFrameInWorldSpace(T joint) => false;
        protected abstract void AddConstraints(T joint);

        public void Convert(IBaker baker, T authoring, List<JointConstraint> constraints)
        {
            m_baker        = baker;
            m_constraints  = constraints;
            var bodyA      = baker.GetComponent<Rigidbody>();
            var bodyB      = authoring.connectedBody;
            var aIsDynamic = IsDynamic(authoring.gameObject, bodyA);
            var bIsDynamic = bodyB != null && IsDynamic(bodyB.gameObject, bodyB);
            if (!aIsDynamic && !bIsDynamic)
                return;

            // Anchors stay in each body's local space so that they follow scale at runtime. Only the frame rotation is unscaled.
            var        transformA    = baker.GetComponent<Transform>();
            quaternion rotationA     = transformA.rotation;
            var        frameRotation = GetLocalFrameOrientation(authoring);
            if (!IsFrameInWorldSpace(authoring))
                frameRotation = math.mul(rotationA, frameRotation);
            var anchorWorld   = transformA.TransformPoint(authoring.anchor);

            var frameInB   = new RigidTransform(frameRotation, authoring.autoConfigureConnectedAnchor ? anchorWorld : authoring.connectedAnchor);
            var transformB = bodyB != null? baker.GetComponent<Transform>(bodyB) : null;
            if (transformB != null)
            {
                quaternion rotationB = transformB.rotation;
                frameInB.rot         = math.mul(math.inverse(rotationB), frameRotation);
                if (authoring.autoConfigureConnectedAnchor)
                    frameInB.pos = transformB.InverseTransformPoint(anchorWorld);
            }

            var entity = baker.GetEntity(TransformUsageFlags.Dynamic);
            m_template = new JointConstraint
            {
                entityA                                                                   = entity,
                entityB                                                                   = bodyB != null? baker.GetEntity(bodyB, TransformUsageFlags.Dynamic) : Entity.Null,
                                                                          weldInLocalA    = new RigidTransform(math.mul(math.inverse(rotationA), frameRotation), authoring.anchor),
                                                                          weldInLocalB    = frameInB,
                                                                          enableCollision = authoring.enableCollision,
            };
            m_swapped = !aIsDynamic;
            if (m_swapped)
            {
                (m_template.entityA, m_template.entityB)           = (m_template.entityB, m_template.entityA);
                (m_template.weldInLocalA, m_template.weldInLocalB) = (m_template.weldInLocalB, m_template.weldInLocalA);
            }

            m_constrainedMass    = 0f;
            m_constrainedInertia = 0f;
            AccumulateMass(bodyA, aIsDynamic);
            AccumulateMass(bodyB, bIsDynamic);
            m_constrainedMass    = m_constrainedMass > 0f ? m_constrainedMass : 1f;
            m_constrainedInertia = m_constrainedInertia > 0f ? m_constrainedInertia : 1f;

            AddConstraints(authoring);
            m_baker       = null;
            m_constraints = null;
        }

        bool IsDynamic(GameObject gameObject, Rigidbody rigidbody)
        {
            if (m_baker.GetComponent<AnnaRigidBodyAuthoring>(gameObject) != null)
                return true;
            return rigidbody != null && !rigidbody.isKinematic;
        }

        void AccumulateMass(Rigidbody rigidbody, bool isDynamic)
        {
            if (!isDynamic || rigidbody == null)
                return;
            m_constrainedMass += rigidbody.mass;
            var inertia        = (float3)rigidbody.inertiaTensor;
            if (math.all(math.isfinite(inertia)))
                m_constrainedInertia += math.csum(inertia) / 3f;
        }

        protected static quaternion FrameOrientationFrom(Vector3 axis, Vector3 secondaryAxis)
        {
            if (axis.sqrMagnitude < 1e-12f)
                axis = Vector3.right;
            if (secondaryAxis.sqrMagnitude < 1e-12f || Vector3.Cross(axis, secondaryAxis).sqrMagnitude < 1e-12f)
                secondaryAxis = Mathf.Abs(axis.normalized.y) < 0.9f ? Vector3.up : Vector3.forward;
            Vector3.OrthoNormalize(ref axis, ref secondaryAxis);
            float3 a = axis;
            float3 b = secondaryAxis;
            return new quaternion(new float3x3(a, b, math.cross(a, b)));
        }

        protected void AddStiff(JointConstraintType type, bool3 axes, float min, float max)
        {
            Add(type, axes, min, max, UnitySim.kStiffSpringFrequency, UnitySim.kStiffDampingRatio);
        }

        protected void AddSoft(JointConstraintType type, bool3 axes, float min, float max, float springConstant, float damperConstant,
                               float target = 0f, float maxForce = 0f)
        {
            bool isLinear = type == JointConstraintType.Position || type == JointConstraintType.PositionMotor || type == JointConstraintType.LinearVelocityMotor;
            var  mass     = isLinear ? m_constrainedMass : m_constrainedInertia;
            if (springConstant <= 0.001f)
            {
                Add(type, axes, min, max, UnitySim.kStiffSpringFrequency, UnitySim.kStiffDampingRatio, target, maxForce);
                return;
            }
            var frequency    = UnitySim.SpringFrequencyFrom(springConstant, 1f / mass);
            var dampingRatio = damperConstant <= 0.001f ? 0f : UnitySim.DampingRatioFrom(springConstant, damperConstant, mass);
            Add(type, axes, min, max, frequency, dampingRatio, target, maxForce);
        }

        protected void AddVelocityMotor(JointConstraintType type, int axis, float target, float maxForce)
        {
            var axes = new bool3(axis == 0, axis == 1, axis == 2);
            Add(type, axes, 0f, 0f, UnitySim.kStiffSpringFrequency, UnitySim.kStiffDampingRatio, target, maxForce);
        }

        void Add(JointConstraintType type, bool3 axes, float min, float max, float frequency, float dampingRatio, float target = 0f, float maxForce = 0f)
        {
            // A single axis measures a signed value from A to B, which flips when the bodies swap roles.
            if (m_swapped && math.countbits(math.bitmask(new bool4(axes, false))) == 1)
            {
                (min, max) = (-max, -min);
                target     = -target;
            }

            var constraint             = m_template;
            constraint.type            = type;
            constraint.constrainedAxes = axes;
            constraint.min             = math.min(min, max);
            constraint.max             = math.max(min, max);
            constraint.springFrequency = frequency;
            constraint.dampingRatio    = dampingRatio;
            constraint.target          = target;
            constraint.maxForce        = maxForce;
            m_constraints.Add(constraint);
        }

        protected static float WrapDegrees(float degrees) => Mathf.DeltaAngle(0f, degrees);
    }

    internal class FixedJointConverter : AnnaJointConverter<FixedJoint>
    {
        protected override void AddConstraints(FixedJoint joint)
        {
            AddStiff(JointConstraintType.Position, true, 0f, 0f);
            AddStiff(JointConstraintType.Rotation, true, 0f, 0f);
        }
    }

    internal class HingeJointConverter : AnnaJointConverter<HingeJoint>
    {
        protected override quaternion GetLocalFrameOrientation(HingeJoint joint) => FrameOrientationFrom(joint.axis, Vector3.zero);

        protected override void AddConstraints(HingeJoint joint)
        {
            AddStiff(JointConstraintType.Position, true,                         0f, 0f);
            AddStiff(JointConstraintType.Rotation, new bool3(false, true, true), 0f, 0f);
            if (joint.useLimits)
                AddStiff(JointConstraintType.Rotation, new bool3(true, false, false), math.radians(joint.limits.min), math.radians(joint.limits.max));
            // Like Game Object physics, the motor takes over from the spring.
            if (joint.useMotor)
            {
                var motor = joint.motor;
                AddVelocityMotor(JointConstraintType.AngularVelocityMotor, 0, math.radians(motor.targetVelocity), motor.force);
            }
            else if (joint.useSpring)
            {
                var spring = joint.spring;
                var target = math.radians(spring.targetPosition);
                AddSoft(JointConstraintType.Rotation, new bool3(true, false, false), target, target, spring.spring, spring.damper);
            }
        }
    }

    internal class CharacterJointConverter : AnnaJointConverter<CharacterJoint>
    {
        protected override quaternion GetLocalFrameOrientation(CharacterJoint joint) => FrameOrientationFrom(joint.axis, joint.swingAxis);

        protected override void AddConstraints(CharacterJoint joint)
        {
            AddStiff(JointConstraintType.Position, true, 0f, 0f);

            var twistSpring = joint.twistLimitSpring;
            var swingSpring = joint.swingLimitSpring;
            AddSoft(JointConstraintType.Rotation,
                    new bool3(true, false, false),
                    -math.radians(joint.highTwistLimit.limit),
                    -math.radians(joint.lowTwistLimit.limit),
                    twistSpring.spring,
                    twistSpring.damper);
            var swing1 = math.radians(joint.swing1Limit.limit);
            AddSoft(JointConstraintType.Rotation, new bool3(false, true, false), -swing1, swing1, swingSpring.spring, swingSpring.damper);
            var swing2 = math.radians(joint.swing2Limit.limit);
            AddSoft(JointConstraintType.Rotation, new bool3(false, false, true), -swing2, swing2, swingSpring.spring, swingSpring.damper);
        }
    }

    internal class ConfigurableJointConverter : AnnaJointConverter<ConfigurableJoint>
    {
        protected override quaternion GetLocalFrameOrientation(ConfigurableJoint joint) => FrameOrientationFrom(joint.axis, joint.secondaryAxis);
        protected override bool IsFrameInWorldSpace(ConfigurableJoint joint) => joint.configuredInWorldSpace;

        protected override void AddConstraints(ConfigurableJoint joint)
        {
            var linearLocked = new bool3(joint.xMotion == ConfigurableJointMotion.Locked, joint.yMotion == ConfigurableJointMotion.Locked,
                                         joint.zMotion == ConfigurableJointMotion.Locked);
            var linearLimited = new bool3(joint.xMotion == ConfigurableJointMotion.Limited, joint.yMotion == ConfigurableJointMotion.Limited,
                                          joint.zMotion == ConfigurableJointMotion.Limited);
            if (math.any(linearLocked))
                AddStiff(JointConstraintType.Position, linearLocked, 0f, 0f);
            if (math.any(linearLimited))
            {
                var limit  = joint.linearLimit.limit;
                var spring = joint.linearLimitSpring;
                // One axis measures a signed offset along the axis, while more axes measure an unsigned distance.
                var min = math.countbits(math.bitmask(new bool4(linearLimited, false))) == 1 ? -limit : 0f;
                AddSoft(JointConstraintType.Position, linearLimited, min, limit, spring.spring, spring.damper);
            }

            var angularLocked = new bool3(joint.angularXMotion == ConfigurableJointMotion.Locked, joint.angularYMotion == ConfigurableJointMotion.Locked,
                                          joint.angularZMotion == ConfigurableJointMotion.Locked);
            if (math.any(angularLocked))
                AddStiff(JointConstraintType.Rotation, angularLocked, 0f, 0f);
            if (joint.angularXMotion == ConfigurableJointMotion.Limited)
            {
                var spring = joint.angularXLimitSpring;
                AddSoft(JointConstraintType.Rotation,
                        new bool3(true, false, false),
                        -math.radians(joint.highAngularXLimit.limit),
                        -math.radians(joint.lowAngularXLimit.limit),
                        spring.spring,
                        spring.damper);
            }
            var yzSpring = joint.angularYZLimitSpring;
            if (joint.angularYMotion == ConfigurableJointMotion.Limited)
            {
                var limit = math.radians(joint.angularYLimit.limit);
                AddSoft(JointConstraintType.Rotation, new bool3(false, true, false), -limit, limit, yzSpring.spring, yzSpring.damper);
            }
            if (joint.angularZMotion == ConfigurableJointMotion.Limited)
            {
                var limit = math.radians(joint.angularZLimit.limit);
                AddSoft(JointConstraintType.Rotation, new bool3(false, false, true), -limit, limit, yzSpring.spring, yzSpring.damper);
            }

            AddDrives(joint, linearLocked, angularLocked);
        }

        // Each drive becomes a single-axis motor. A position spring drives toward the target position or rotation,
        // otherwise a damper drives toward the target velocity.
        void AddDrives(ConfigurableJoint joint, bool3 linearLocked, bool3 angularLocked)
        {
            var linearDrives = new[] { joint.xDrive, joint.yDrive, joint.zDrive };
            for (int axis = 0; axis < 3; axis++)
            {
                var drive = linearDrives[axis];
                if (linearLocked[axis] || drive.maximumForce <= 0f)
                    continue;
                var axes = new bool3(axis == 0, axis == 1, axis == 2);
                if (drive.positionSpring > 0f)
                    AddSoft(JointConstraintType.PositionMotor, axes, 0f, 0f, drive.positionSpring, drive.positionDamper, -joint.targetPosition[axis], drive.maximumForce);
                else if (drive.positionDamper > 0f)
                    AddVelocityMotor(JointConstraintType.LinearVelocityMotor, axis, -joint.targetVelocity[axis], drive.maximumForce);
            }

            if (joint.rotationDriveMode != RotationDriveMode.XYAndZ)
                return;
            var angularDrives = new[] { joint.angularXDrive, joint.angularYZDrive, joint.angularYZDrive };
            var targetAngles  = joint.targetRotation.eulerAngles;
            for (int axis = 0; axis < 3; axis++)
            {
                var drive = angularDrives[axis];
                if (angularLocked[axis] || drive.maximumForce <= 0f)
                    continue;
                var axes = new bool3(axis == 0, axis == 1, axis == 2);
                if (drive.positionSpring > 0f)
                {
                    var target = -math.radians(WrapDegrees(targetAngles[axis]));
                    AddSoft(JointConstraintType.RotationMotor, axes, -2f * math.PI, 2f * math.PI, drive.positionSpring, drive.positionDamper, target, drive.maximumForce);
                }
                else if (drive.positionDamper > 0f)
                    AddVelocityMotor(JointConstraintType.AngularVelocityMotor, axis, joint.targetAngularVelocity[axis], drive.maximumForce);
            }
        }
    }

    internal class SpringJointConverter : AnnaJointConverter<SpringJoint>
    {
        protected override void AddConstraints(SpringJoint joint)
        {
            // A zero spring constant applies no force, so there is nothing to solve.
            if (joint.spring <= 0.001f)
                return;
            AddSoft(JointConstraintType.Position, true, joint.minDistance, joint.maxDistance, joint.spring, joint.damper);
        }
    }
}

