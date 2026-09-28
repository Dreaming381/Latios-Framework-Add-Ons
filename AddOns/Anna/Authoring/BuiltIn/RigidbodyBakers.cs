using Latios.Psyshock;
using Latios.Psyshock.Authoring;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Anna.Authoring
{
    [DisableAutoCreation]
    internal class RigidbodyBaker : Baker<Rigidbody>
    {
        public override void Bake(Rigidbody authoring)
        {
            if (GetComponent<AnnaRigidBodyAuthoring>() != null)
                return;

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            if (authoring.isKinematic)
                return;

            // Unity's defaults when a collider has no material
            float friction    = 0.6f;
            float restitution = 0f;
            var   collider    = GetComponent<UnityEngine.Collider>();
            var   material    = collider != null ? collider.sharedMaterial : null;
            if (material != null)
            {
                DependsOn(material);
                friction    = material.dynamicFriction;
                restitution = material.bounciness;
            }

            AddComponent(entity, new RigidBody
            {
                inverseMass              = math.rcp(math.max(authoring.mass, math.EPSILON)),
                coefficientOfFriction    = (half)friction,
                coefficientOfRestitution = (half)restitution,
            });
            AddComponent<CollisionWorldAabb>( entity);
            AddComponent<CollisionWorldIndex>(entity);
            AddBuffer<AddImpulse>(entity);

            var constraints = authoring.constraints;
            if (constraints != RigidbodyConstraints.None)
            {
                var flags       = new LockWorldAxesFlags();
                flags.positionX = (constraints & RigidbodyConstraints.FreezePositionX) != 0;
                flags.positionY = (constraints & RigidbodyConstraints.FreezePositionY) != 0;
                flags.positionZ = (constraints & RigidbodyConstraints.FreezePositionZ) != 0;
                flags.rotationX = (constraints & RigidbodyConstraints.FreezeRotationX) != 0;
                flags.rotationY = (constraints & RigidbodyConstraints.FreezeRotationY) != 0;
                flags.rotationZ = (constraints & RigidbodyConstraints.FreezeRotationZ) != 0;
                AddComponent(entity, flags);
            }

            if (!authoring.useGravity)
                AddComponent(entity, new GravityOverride { gravity = float3.zero });
            if (!authoring.automaticCenterOfMass)
                AddComponent(entity, new LocalCenterOfMassOverride { centerOfMass = authoring.centerOfMass });
            if (!authoring.automaticInertiaTensor)
            {
                AddComponent(entity, new LocalInertiaOverride
                {
                    inertiaDiagonal = new UnitySim.LocalInertiaTensorDiagonal
                    {
                        inertiaDiagonal   = (float3)authoring.inertiaTensor / math.max(authoring.mass, math.EPSILON),
                        tensorOrientation = authoring.inertiaTensorRotation
                    }
                });
            }
        }
    }
}

