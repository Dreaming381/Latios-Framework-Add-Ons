using System;
using System.Runtime.CompilerServices;
using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[assembly: InternalsVisibleTo("Latios.Anna")]

namespace Latios.Shockwave
{
    internal partial struct ShockwaveWorldCollision : ICollectionComponent
    {
        public CollisionWorld collisionWorld;

        public JobHandle TryDispose(JobHandle inputDeps) => inputDeps;  // Uses WorldUpdateAllocator
    }

    public partial struct WorldCollisionAspect
    {
        private CollisionWorld collisionWorld;

        public partial struct Mask
        {
            internal CollisionWorld.Mask mask;
            private bool isCreatedPrivate => mask.isCreated;
        }

        public partial struct FindObjectsEnumerator
        {
            internal Psyshock.FindObjectsEnumerator layerEnumerator;
            internal FindObjectsWorldEnumerator     worldEnumerator;
            internal bool                           usesMask;
            private FindObjectsResult CurrentPrivate => usesMask ? worldEnumerator.Current : layerEnumerator.Current;
            private bool MoveNextPrivate() => usesMask? worldEnumerator.MoveNext() : layerEnumerator.MoveNext();
        }

        private partial FluentQuery AppendToQueryPrivate(FluentQuery query) => query.With<ShockwaveWorldCollision.ExistComponent>(true);
        private partial WorldCollisionAspect CreateCollectionAspectPrivate(LatiosWorldUnmanaged latiosWorld, EntityManager entityManager,
                                                                           Entity entity)
        {
            return new WorldCollisionAspect
            {
                collisionWorld = latiosWorld.GetCollectionComponent<ShockwaveWorldCollision>(entity, true).collisionWorld
            };
        }

        private partial Mask CreateMaskPrivate(EntityQueryMask entityQueryMask) => new Mask {
            mask = collisionWorld.CreateMask(entityQueryMask)
        };
        private partial Mask CreateMaskPrivate(in TempQuery tempQuery) => new Mask {
            mask = collisionWorld.CreateMask(in tempQuery)
        };

        private partial FindObjectsEnumerator FindObjectsPrivate(in Aabb searchAabb)
        {
            return new FindObjectsEnumerator
            {
                layerEnumerator = Physics.FindObjects(in searchAabb, collisionWorld.collisionLayer),
                usesMask        = false
            };
        }
        private partial FindObjectsEnumerator FindObjectsPrivate(in Aabb searchAabb, in Mask mask)
        {
            return new FindObjectsEnumerator
            {
                worldEnumerator = Physics.FindObjects(in searchAabb, in collisionWorld, in mask.mask),
                usesMask        = true
            };
        }
    }
}

