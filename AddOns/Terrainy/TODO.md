﻿# Open Doings 

Thanks to Dreaming I'm Latios for providing this list

-   [x] Set up the add-on from the template and assembly definitions.
-   [x] Define two components, one to store a UnityObjectRef and the other as a
    cleanup component to store a UnityObjectRef.
-   [x] Bake the former by instantiating TerrainData and clearing all vegetation
    and detail properties so that it is just the textured heightmap (for builds
    only).
-   [x] Write a reactive system in the LatiosWorldSyncGroup for terrain that
    constructs a new GameObject with a Terrain component and assigns it the
    TerrainData, or destroys the Terrain GameObject if the TerrainData component
    is missing (teardown).
-   [ ] Find all the source prefabs associated with the tree and detail
    prototypes in the terrain and bake entity prefabs for those to put on the
    terrain.
-   [ ] Bake all tree and detail instance data into dynamic buffers, at least
    for closed subscenes.
-   [ ] In the reactive system, spawn the trees and details as entities, and
    maybe gather and respawn all entities during live baking.
-   [ ] Figure out some mechanism so that we have terrain tree/detail shaders at
    authoring and DOTS shaders at runtime.
-   [ ] Bake and iterate on terrain colliders in parallel somewhere between
    steps 5 and 8, without worrying about details and trees, since those will be
    their own entities at runtime. Might need to rebuild colliders at runtime
    for live-baked entities.
-   [ ] Validate builds have all unnecessary TerrainData stripped.
