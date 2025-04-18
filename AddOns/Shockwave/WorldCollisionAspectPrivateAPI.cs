using System;
using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

// You may add any new methods to this file, but please try to minimize their use.
// Every method will be converted to a partial method and will need to be implemented
// by any physics engine that wishes to support Shockwave.

namespace Latios.Shockwave
{
    public partial struct WorldCollisionAspect
    {
        public partial struct Mask
        {
            private bool isCreatedPrivate => throw new NotImplementedException();
        }

        public partial struct FindObjectsEnumerator
        {
            private FindObjectsResult CurrentPrivate => throw new NotImplementedException();
            private bool MoveNextPrivate() => throw new NotImplementedException();
        }

        private FluentQuery AppendToQueryPrivate(FluentQuery query) => throw new NotImplementedException();
        private WorldCollisionAspect CreateCollectionAspectPrivate(LatiosWorldUnmanaged latiosWorld, EntityManager entityManager,
                                                                   Entity entity) => throw new NotImplementedException();

        private Mask CreateMaskPrivate(EntityQueryMask entityQueryMask) => throw new NotImplementedException();
        private Mask CreateMaskPrivate(in TempQuery tempQuery) => throw new NotImplementedException();

        private FindObjectsEnumerator FindObjectsPrivate(in Aabb searchAabb) => throw new NotImplementedException();
        private FindObjectsEnumerator FindObjectsPrivate(in Aabb searchAabb, in Mask mask) => throw new NotImplementedException();
    }
}

