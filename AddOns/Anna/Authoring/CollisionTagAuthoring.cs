using Latios.Psyshock;
using Latios.Psyshock.Authoring;
using Latios.Transforms.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Anna.Authoring
{
    public class CollisionTagAuthoring : MonoBehaviour
    {
        public enum Mode
        {
            IncludeEnvironmentRecursively,
            IncludeKinematicRecursively,
            ExcludeRecursively,
            IncludeEnvironmentSelfOnly,
            IncludeKinematicSelfOnly,
            ExcludeSelfOnly,
        }

        public Mode mode;

        [BakingType]
        struct RequestPrevious : IRequestPreviousTransform { }

        enum Role
        {
            Undecided,
            None,
            Environment,
            Kinematic,
        }

        static Role FindTaggedRole(GameObject colliderObject, IBaker baker)
        {
            var search = colliderObject;
            while (search != null)
            {
                var tag = baker.GetComponentInParent<CollisionTagAuthoring>(search);
                if (tag == null)
                    return Role.Undecided;

                bool isSelf = tag.gameObject == colliderObject;
                switch (tag.mode)
                {
                    case Mode.IncludeEnvironmentRecursively:
                        return Role.Environment;
                    case Mode.IncludeKinematicRecursively:
                        return Role.Kinematic;
                    case Mode.ExcludeRecursively:
                        return Role.None;
                    case Mode.IncludeEnvironmentSelfOnly when isSelf:
                        return Role.Environment;
                    case Mode.IncludeKinematicSelfOnly when isSelf:
                        return Role.Kinematic;
                    case Mode.ExcludeSelfOnly when isSelf:
                        return Role.None;
                }

                search = baker.GetParent(tag.gameObject);
            }
            return Role.Undecided;
        }

        internal static void BakeCollider(Component authoring, IBaker baker, bool bakeUnityRigidbodies)
        {
            var  role        = FindTaggedRole(authoring.gameObject, baker);
            bool isRigidBody = baker.GetComponent<AnnaRigidBodyAuthoring>() != null;

            if (bakeUnityRigidbodies && role == Role.Undecided && !isRigidBody)
            {
                var rigidbody = baker.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    if (rigidbody.isKinematic)
                        role = Role.Kinematic;
                    else
                        isRigidBody = true;
                }
            }

            Entity entity;
            if (role == Role.Environment)
            {
                entity = baker.GetEntity(TransformUsageFlags.Renderable);
                baker.AddComponent<EnvironmentCollisionTag>(entity);
            }
            else if (role == Role.Kinematic)
            {
                entity = baker.GetEntity(TransformUsageFlags.Dynamic);
                baker.AddComponent<KinematicCollisionTag>(entity);
                baker.AddComponent<RequestPrevious>(      entity);
                if (!isRigidBody)
                {
                    baker.AddComponent<CollisionWorldAabb>(entity);
                }
            }
            else
                return;

            if (!isRigidBody)
                baker.AddComponent<CollisionWorldIndex>(entity);
        }
    }

    [BakeDerivedTypes]
    public class CollisionTagAuthoringBaker : Baker<UnityEngine.Collider>
    {
        public override void Bake(UnityEngine.Collider authoring)
        {
            if (this.GetMultiColliderBakeMode(authoring, out _) == MultiColliderBakeMode.Ignore)
                return;

            CollisionTagAuthoring.BakeCollider(authoring, this, sEnableUnityRigidBodyBaking);
        }

        internal static bool sEnableUnityRigidBodyBaking = false;
    }

    public class CollisionTagAuthoringCompoundBaker : Baker<ColliderAuthoring>
    {
        public override void Bake(ColliderAuthoring authoring)
        {
            CollisionTagAuthoring.BakeCollider(authoring, this, CollisionTagAuthoringBaker.sEnableUnityRigidBodyBaking);
        }
    }
}

