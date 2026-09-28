# Baking Pipeline

`BakeOccluderPlatesSystem` runs in `PostBakingSystemGroup`, after Kinemation has
baked the `RenderMeshArray` the plates are keyed to.

## One Blob per RenderMeshArray

The plates for every mesh in a `RenderMeshArray` go into one blob, and every
entity sharing that array gets a reference to it. At runtime, Kinemation already
tells Peekaboo which mesh an entity is drawing, so nothing extra gets stored per
entity.

Baking per array instead of per renderer also means a mesh used by a thousand
entities only gets processed once.

```
PeekabooOccluderBlob
    meshes[]           lines up with RenderMeshArray.MeshReferences
    plates[]           all plates, referenced by OccluderMesh ranges
    vertices[]         all plate vertices, referenced by OccluderPlate ranges
    opaqueMaterials[]  lines up with RenderMeshArray.MaterialReferences
    handBuilt          true for blobs from PeekabooOccluderBuilder
```

Nothing about `MaterialMeshInfo` ranges gets baked. Which meshes, materials, and
submeshes an entity draws right now, including LOD ranges and crossfades, comes
from Kinemation's `OcclusionCullingContextAspect` at runtime. That's the same data
Kinemation uses to generate draw commands, so only what's actually being drawn is
allowed to occlude.

Meshes are read with `UnityEditor.MeshUtility.AcquireReadOnlyMeshData`, which
works even without Read/Write enabled. Psyshock's collider bakers read meshes the
same way. Plates get built in a parallel job over the meshes, cached per mesh,
then assembled into a blob. See Incremental Baking below.

**There's no authoring.** A renderer occludes because it's a renderer. The only
authoring component is for opting out.

## Materials and Submeshes

Baking doesn't look at materials (Amendment B in
[Plate Validity](Plate%20Validity.md)). Every submesh gets plates, and each plate
records the one submesh it depends on:

-   A surface plate depends on the submesh its patch came from. Coplanar
    triangles only get merged into a patch within one submesh.
-   An interior plate depends on the whole shell, written as submesh -1. The
    shell is the set of submeshes that together form a closed surface, stored per
    mesh as a 64-bit mask.

To find the shell, the baker starts with every submesh and keeps dropping any
submesh with a triangle on an edge that isn't shared by exactly two triangles. A
decal quad or a detail strip gets dropped, and the solid body is left. Without
this, one open submesh would stop a whole solid from getting interior plates.

The blob also records which of the array's materials are opaque. A material
counts if it's in the opaque render queue range, writes depth, has no alpha
keywords, has `_AlphaClip` and URP's `_Surface` off, and doesn't cull front faces.
The culling pass then only uses a plate while its submesh is being drawn with one
of those. See [Culling](Culling.md).

Submeshes past 64 get no plates, since the dependency is checked against a 64-bit
mask.

A mesh with more than one submesh, or with no shell, also gets narrowed plates
that depend on a single submesh through a cone of view directions. That's what
keeps a car occluding while its glass is drawn transparent, and what lets an open
curved mesh occlude at all. See Narrowing With a Cone in
[Plate Validity](Plate%20Validity.md).

## Mesh LODs

Only mesh LOD 0 is baked. With mesh LODs, a submesh's index range covers every
LOD, but reading the indices only returns LOD 0's. So the baker sizes its read
from LOD 0's range, and takes the mesh's bounds from LOD 0's vertices, since the
vertex buffer can hold other LODs' vertices too. At runtime, a renderer drawing any
other mesh LOD doesn't occlude, since LOD 0's plates can poke out of a coarser
surface.

## Which Meshes Get Skipped

**A mesh nothing uses.** An array can hold meshes no entity uses, and baking those
is wasted work.

**A renderer that can't occlude.** Some renderers never occlude, because what
gets drawn isn't the shape the plates were cut from: depth-sorted, skinned,
blend-shaped, dynamic-meshed, unique-meshed, or shader-displaced renderers, plus
anything with `PeekabooDisableOccluderTag`. The culling pass skips them with the
query in `PeekabooOcclusionCullingSystem.OnCreate()`. Baking skips the same list,
so they don't get an occluder, and a mesh only they draw doesn't get baked.

Some of those components only show up at runtime, so the runtime query is still
the one that decides.

## Incremental Baking

`BakeOccluderPlatesSystem` is a baking system, not a baker. Unity reverts what
bakers add when they rerun, but anything a baking system adds stays on the entity
through every later incremental bake. So the system tracks its own occluders and
cleans up after them.

**It marks what it adds.** Next to each `PeekabooOccluder` it adds, it puts
`PeekabooBakedOccluder`, a `[BakingType]` component that never reaches the
output. The marker also holds the blob the system set. Every bake, before
anything else, the system:

-   Gives up any entity whose occluder now holds a different blob. Something else,
    like a user's baker using `PeekabooOccluderBuilder`, replaced it.
-   Drops the marker from any entity whose occluder was removed.
-   Removes both components from any entity that lost its `MaterialMeshInfo` or
    `RenderMeshArray`, or gained one of the components in the list above.

Then each array's entities either get the array's blob, or lose the system's
occluders if the array has no plates. An occluder without the marker is never
touched, so hand-built occluders survive every bake.

**It only rebuilds what changed.** The system runs over the whole subscene on
every live update. Rebuilding every mesh each time took 59 to 68 ms in the shapes
test scene. Now there are two caches:

-   **Plates per mesh**, in `OccluderMeshCache`. A mesh in the asset database is
    keyed by its asset's dependency hash, its local file ID, and its dirty count,
    so nothing gets read to find the key. Reimporting the mesh, changing its
    import settings, or editing it in memory changes the key. Any other mesh is
    keyed by a hash of exactly what the plate baker reads: vertex positions, and
    the LOD 0 indices of the first 64 triangle submeshes. Changing normals or UVs
    doesn't cost a rebuild.
-   **The blob per array**, in the `BlobAssetStore`, under a key made from the
    keys of the meshes the array uses and which of its materials are opaque.
    Arrays that bake the same share a blob. The store drops blobs nothing
    references after each live update, so this needs no cleanup.

The mesh cache is static, so it carries over between subscenes. It holds up to
64 MB, dropping the least recently used meshes past that, and a domain reload
clears it. That also covers changes to the plate baker itself.

In the shapes test scene, a live update that changes no meshes went from 59 to
68 ms in this system to 0.05 to 0.2 ms. One that adds a mesh only builds that
mesh.

A mesh outside the asset database gets its contents hashed on every bake. That's
far cheaper than building its plates, but it still reads the whole mesh. An asset
mesh edited in memory without being marked dirty keeps its old key, and its old
plates, until something dirties it.

`PeekabooIncrementalBakeProbe` in the validation folder drives these cases
through live baking.

## Determinism

Baking has to be deterministic. Otherwise, reimporting a subscene produces a
different blob and invalidates content that didn't change. Grouping triangles
walks them in index order rather than hash order, and choosing which plates to
keep uses a stable sort followed by a fixed round-robin.

## Cost

The expensive part is the interior pass. It voxelizes at up to 48 cubed, sweeps
three times, and slices five times along each of thirteen axes. It runs on every
closed mesh. For a box, the slices get thrown away afterward, since deciding
whether a box needs a slice means knowing how big the slice would be.

That's the first thing to revisit if bake times become a problem. A cheaper
version could check how well the surface plates cover each axis first, and skip
voxelizing entirely when every axis is covered. The risk is a mesh with big but
badly spread-out surface plates.
