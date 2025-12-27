using System;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Shockwave
{
    /// <summary>
    /// A unified API for performing queries on the spatial structures of the physics engine in use.
    /// </summary>
    public partial struct WorldCollisionAspect : ICollectionAspect<WorldCollisionAspect>
    {
        #region Types
        /// <summary>
        /// A temporary structure that represents an entity query that can filter the current WorldCollisionAspect.
        /// This is NOT a NativeContainer, but may reference memory allocated with Allocator.Temp.
        /// You can make it a field in a job, but you should not try to pass an instance created on the main thread into a job.
        /// </summary>
        public partial struct Mask
        {
            public bool isCreated => isCreatedPrivate;
        }

        /// <summary>
        /// This is an enumerator for iterating through a FindObjects query's results (AABB search).
        /// It can be used in a foreach expression.
        /// </summary>
        public partial struct FindObjectsEnumerator
        {
            public FindObjectsEnumerator GetEnumerator() => this;
            public FindObjectsResult Current => CurrentPrivate;
            public bool MoveNext() => MoveNextPrivate();
        }


        #endregion

        #region ICollectionAspect
        public FluentQuery AppendToQuery(FluentQuery query) => AppendToQueryPrivate(query);

        public WorldCollisionAspect CreateCollectionAspect(LatiosWorldUnmanaged latiosWorld, EntityManager entityManager, Entity entity) => CreateCollectionAspectPrivate(
            latiosWorld,
            entityManager,
            entity);
        #endregion

        #region Mask Creation
        /// <summary>
        /// Creates a mask that can be used when performing query operations on this WorldCollisionAspect
        /// to only include entities which match the specified EntityQueryMask without filtering.
        /// This mask should typically be created from inside the job and cached.
        /// </summary>
        /// <param name="entityQueryMask">An EntityQueryMask which will identify entities in the WorldCollisionAspect to be considered candidates in queries</param>
        /// <returns>A Mask which can be used as a parameter in various spatial query methods for this WorldCollisionAspect.</returns>
        public Mask CreateMask(EntityQueryMask entityQueryMask) => CreateMaskPrivate(entityQueryMask);

        /// <summary>
        /// Creates a mask that can be used when performing query operations on this WorldCollisionAspect
        /// to only include entities which match the specified entity query description without filtering.
        /// This mask should typically be created from inside the job and cached.
        /// </summary>
        /// <param name="entityStorageInfoLookup">The EntityStorageInfoLookup used for fetching entities and ensuring safety</param>
        /// <param name="with">The component types that should be present (enabled states are not considered)</param>
        /// <param name="withAny">The component types where at least one should be present (enabled states are not considered)</param>
        /// <param name="without">The component types that should be absent (disabled components don't count as absent)</param>
        /// <param name="options">EntityQuery options to use. FilterWriteGroup and IgnoreComponentEnabledState are not acknowledged.</param>
        /// <returns>A Mask which can be used as a parameter in various spatial query methods for this WorldCollisionAspect.</returns>
        public Mask CreateMask(EntityStorageInfoLookup entityStorageInfoLookup,
                               ComponentTypeSet with,
                               ComponentTypeSet withAny = default,
                               ComponentTypeSet without = default,
                               EntityQueryOptions options = EntityQueryOptions.Default)
        {
            return CreateMask(new TempQuery(default, entityStorageInfoLookup, with, withAny, without, options));
        }

        /// <summary>
        /// Creates a mask that can be used when performing query operations on this WorldCollisionAspect
        /// to only include entities which match the specified TempQuery.
        /// This mask should typically be created from inside the job and cached.
        /// </summary>
        /// <param name="entityQueryMask">A TempQuery which will identify entities in the WorldCollisionAspect to be considered candidates in queries.
        /// The TempQuery's internal array of EntityArchetype will be ignored, and constructing a TempQuery with that parameter defaulted is valid.</param>
        /// <returns>A Mask which can be used as a parameter in various spatial query methods for this WorldCollisionAspect.</returns>
        public Mask CreateMask(in TempQuery tempQuery) => CreateMaskPrivate(in tempQuery);
        #endregion

        #region FindObjects
        /// <summary>
        /// Finds all entities whose Aabbs overlap the searchAabb.
        /// </summary>
        /// <param name="searchAabb">The bounding box to look for colliders potentially intersecting</param>
        /// <returns>An iterator that can be used in a foreach expression</returns>
        public FindObjectsEnumerator FindObjects(in Aabb searchAabb) => FindObjectsPrivate(in searchAabb);

        /// <summary>
        /// Finds all entities matching the query represented by the mask whose Aabbs overlap the searchAabb.
        /// </summary>
        /// <param name="searchAabb">The bounding box to look for colliders potentially intersecting</param>
        /// <param name="mask">The mask representing the entity query that resulting entities should match</param>
        /// <returns>An iterator that can be used in a foreach expression</returns>
        public FindObjectsEnumerator FindObjects(in Aabb searchAabb, in Mask mask) => FindObjectsPrivate(in searchAabb, in mask);
        #endregion

        #region ColliderCast

        /// <summary>
        /// Sweeps a collider beginning at castStart throught to castEnd and checks if the collider hits
        /// any other collider in the CollisionLayer. If so, results of the hit in which the casted collider
        /// traveled the least is reported. It is assumed that rotation, scale, and stretch remain constant
        /// throughout the operation. Hits where the casted collider starts already overlapping a target in
        /// the CollisionLayer are ignored.
        /// </summary>
        /// /// <param name="colliderToCast">The casted collider that should be swept through space</param>
        /// <param name="castStart">The transform of the casted collider at the start of the cast</param>
        /// <param name="castEnd">The position of the casted collider at the end of the cast range</param>
        /// <param name="result">The resulting information about the least-swept hit if there is one. If there
        /// is no hit, the contents of the result are undefined.</param>
        /// <param name="layerBodyInfo">Additional info as to which collider in the CollisionLayer was hit</param>
        /// <returns>True if a hit was found, false otherwise</returns>
        public bool ColliderCast(in Collider colliderToCast, 
                                 in TransformQvvs castStart, 
                                 float3 castEnd, 
                                 out ColliderCastResult result, 
                                 out LayerBodyInfo layerBodyInfo) =>
                    ColliderCastPrivate(in colliderToCast, in castStart,  castEnd,  out result, out layerBodyInfo);


        /// <summary>
        /// Sweeps a collider beginning at castStart throught to castEnd and checks if the collider hits any other matching
        /// entity collider in the CollisionWorld. If so, results of the hit in which the casted collider traveled the least
        /// is reported. It is assumed that rotation, scale, and stretch remain constant throughout the operation. Hits where
        /// the casted collider starts already overlapping a target in the CollisionWorld are ignored.
        /// </summary>
        /// <param name="colliderToCast">The casted collider that should be swept through space</param>
        /// <param name="castStart">The transform of the casted collider at the start of the cast</param>
        /// <param name="castEnd">The position of the casted collider at the end of the cast range</param>
        /// <param name="mask">A mask containing the entity query used to prefilter the entities in the CollisionWorld</param>
        /// <param name="result">The resulting information about the least-swept hit if there is one. If there
        /// is no hit, the contents of the result are undefined.</param>
        /// <param name="worldBodyInfo">Additional info as to which collider in the CollisionWorld was hit</param>
        /// <returns>True if a hit was found, false otherwise</returns>
        public bool ColliderCast(in Collider colliderToCast,
                                 in TransformQvvs castStart,
                                 float3 castEnd,
                                 in Mask mask,
                                 out ColliderCastResult result,
                                 out LayerBodyInfo worldBodyInfo) =>
                    ColliderCastPrivate(in colliderToCast, castStart, castEnd, in mask, out result, out worldBodyInfo);

        #endregion

    }
}

