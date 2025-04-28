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

        internal static void BakeCollider(Component authoring, IBaker baker)
        {
            var  search        = authoring.gameObject;
            bool isEnvironment = false;
            bool isKinematic   = false;
            while (search != null)
            {
                var tag = baker.GetComponentInParent<CollisionTagAuthoring>(search);
                if (tag == null)
                    break;

                if (tag.mode == Mode.IncludeEnvironmentSelfOnly)
                {
                    if (search == authoring.gameObject)
                    {
                        isEnvironment = true;
                        break;
                    }
                }
                else if (tag.mode == Mode.IncludeKinematicSelfOnly)
                {
                    if (search == authoring.gameObject)
                    {
                        isKinematic = true;
                        break;
                    }
                }
                else if (tag.mode == Mode.ExcludeSelfOnly)
                {
                    if (search == authoring.gameObject)
                    {
                        break;
                    }
                }
                else if (tag.mode == Mode.IncludeEnvironmentRecursively)
                {
                    isEnvironment = true;
                    break;
                }
                else if (tag.mode == Mode.IncludeKinematicRecursively)
                {
                    isKinematic = true;
                    break;
                }

                search = baker.GetParent(search);
            }

            bool   isRigidBody = baker.GetComponent<AnnaRigidBodyAuthoring>() != null;
            Entity entity;
            if (isEnvironment)
            {
                entity = baker.GetEntity(TransformUsageFlags.Renderable);
                baker.AddComponent<EnvironmentCollisionTag>(entity);
            }
            else if (isKinematic)
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
                entity = baker.GetEntity(TransformUsageFlags.Renderable);

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

            CollisionTagAuthoring.BakeCollider(authoring, this);
        }
    }

    public class CollisionTagAuthoringCompoundBaker : Baker<ColliderAuthoring>
    {
        public override void Bake(ColliderAuthoring authoring)
        {
            CollisionTagAuthoring.BakeCollider(authoring, this);
        }
    }
}

