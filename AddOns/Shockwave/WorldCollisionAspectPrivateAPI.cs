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
            //private bool isCreatedPrivate => throw new NotImplementedException();
        }

        public partial struct FindObjectsEnumerator
        {
            //private FindObjectsResult CurrentPrivate => throw new NotImplementedException();
            //private bool MoveNextPrivate() => throw new NotImplementedException();
        }

        private partial FluentQuery AppendToQueryPrivate(FluentQuery query);
        private partial WorldCollisionAspect CreateCollectionAspectPrivate(LatiosWorldUnmanaged latiosWorld, EntityManager entityManager,
                                                                           Entity entity);

        private partial Mask CreateMaskPrivate(EntityQueryMask entityQueryMask);
        private partial Mask CreateMaskPrivate(in TempQuery tempQuery);

        private partial FindObjectsEnumerator FindObjectsPrivate(in Aabb searchAabb);
        private partial FindObjectsEnumerator FindObjectsPrivate(in Aabb searchAabb, in Mask mask);
    }
}

