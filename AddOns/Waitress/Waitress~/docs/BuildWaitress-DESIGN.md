# BuildWaitress design

This document covers what BuildWaitress reads and how, the Unity and Entities
formats it decodes, the facts measured while building it, how it was
validated, and what it doesn't support.

## Sources

-   **The build report.** `Library/LastBuild.buildreport` is a Unity serialized
    file (format 22) with type trees, in 2021.3 as well as 6000.x. The tool has
    its own reader (`Serialized/SerializedFile.cs`). It decodes any object from
    the file's own type tree, so it needs neither the Editor nor a matching
    Unity version. The report holds:
    -   one BuildReport object (class 1125): the summary, the output files with
        their roles, and the build steps with their messages;
    -   one PackedAssets object (1126) per serialized output file, with every
        object in it: file id, class, size, source GUID and path;
    -   with `DetailedBuildReport`, one ScenesUsingAssets object listing the
        scenes that use each asset.
-   **The build folder.**
    -   Built files that are still the size the report gives are opened to name
        the objects the report lists by file id. Most asset classes serialize
        `m_Name` first, so that string names sub-assets such as sprites and
        meshes.
    -   Subscene files and content archives are read fully (below).
-   **Entities types.** The Editor exports TypeManager to
    `UserSettings/Waitress/BuildWaitress/types.tsv`. Each type gets:
    -   its stable hash, memory ordering and TypeIndex flags;
    -   its category, size in chunk, element size, buffer capacity and maximum
        chunk capacity;
    -   its blob reference offsets, its name, and a few other counts.

    The export runs again whenever it is older than `Library/ScriptAssemblies`
    and an Editor answers. Without an export, names come from
    `Logs/ExportedTypes.log`, which every DOTS build writes. That log gives
    names only, with no sizes.

## Subscene formats (Entities 1.4)

Unity's ImHex support (`UNITY_DOTS_IMHEX`, `ImHexPatternEntitySceneBinaryWriter`)
emits a pattern next to each write in `SerializeUtility`. That writer, and the
serialization code it annotates, are the specification the tool's readers
follow. The tool needs neither the define nor ImHex.

-   **`<guid>.entityheader`**
    -   It starts with an int version (3) and a 32-byte blob asset header.
    -   Then comes `SceneMetaData`:
        -   `BlobArray<SceneSectionData>`, 72 bytes each: scene GUID, section
            index, file size, object reference count, bounds, codec, raw size,
            and more;
        -   the scene name (`BlobString`);
        -   custom metadata;
        -   the header blob batch size.
    -   A `BlobArray` is an int offset, relative to the field itself, followed
        by a length.
-   **`<guid>.<section>.entities`** is a `DOTSBIN!` container.
    -   It starts with a 152-byte FileHeader. Its node section holds
        `NodeHeader`s of 72 bytes or more, identified by the Hash128 ids in
        `SerializeUtility`, written as nibble-swapped hex.
    -   The nodes are, in order:
        -   archetypes: a count, the type hashes, then each archetype's entity
            count and its type indices. Bit 29 marks a chunk component, and
            Entity itself is left out.
        -   blob assets: a 16-byte batch header, then each blob as a 32-byte
            header plus its data, aligned to 16;
        -   shared and managed components;
        -   enabled bits;
        -   chunks: every chunk is written as a full 16 KB block, whose header
            carries the archetype index and entity count;
        -   buffer data: per chunk, records of (offset in the chunk, size, data)
            for buffers that left their chunk;
        -   optionally, a prefab node and a debug section with type names.
    -   A section is LZ4-compressed only when its header's codec says so. The
        default codec is None.
-   **Chunk layout.** The tool reproduces `EntityComponentStore.CreateArchetype`:
    -   sizes are `SizeInChunk`, or 0 for tags, shared components and chunk
        components;
    -   capacity is the largest that fits `CalculateSpaceRequirement` within
        16,320 bytes, capped by every type's maximum chunk capacity;
    -   offsets follow the memory order, sorted by (TypeIndex flags, memory
        ordering), with each array aligned to 64 bytes.
-   **Blob owners.** Before saving, Entities replaces every
    `BlobAssetReference` in chunk data with the blob's offset in the batch, and
    -1 when null. The tool reads those offsets through the layout and the
    exported blob offsets, including buffer elements that live outside the
    chunk. Each blob goes to the first component type that references it.
-   **`ContentArchives/archive_dependencies.bin`** is a blob of
    `RuntimeContentCatalogData`. It holds:
    -   archives (Hash128);
    -   files: id, archive index and dependency index, 24 bytes each;
    -   objects: a 64-byte `UnsafeUntypedWeakReferenceId` with the file index
        and the local id;
    -   scenes (184 bytes);
    -   dependency lists.

    Hash128 file names are the bytes in nibble-swapped hex.
-   **Content archives** are UnityFS bundles: a big-endian header, an
    LZ4-compressed block and directory list, and LZ4 data blocks. Each holds one
    serialized file, which keeps its type trees.
    -   The catalog keys a section's objects by scene GUID and section index,
        with generation type `SubSceneObjectReferences`. It does this in one of
        two forms:
        -   **One `ReferencedUnityObjects` object per section.** This is what
            6000.3.24 with Entities 1.4.8 writes. Its `Array` of PPtrs leads,
            through the file's externals, to the meshes and materials the
            section uses.
        -   **One catalog entry per referenced object.** This is what the lsss
            build (6000.3.18) has: 1,083 entries for 13 sections.

        The tool accepts both forms and merges them.
    -   Textures and shaders are reached through materials. The tool follows
        every PPtr in every archive object to build the reference graph, which
        is what makes the `why` chains and the "through them" totals possible.
    -   Object sizes are uncompressed. Each archive's size on disk is shared out
        among its objects by uncompressed size, so `assets` totals match the
        "subscene content" category.
    -   Texture and mesh sizes add the `m_StreamData` size.

## Measured facts

Measured on Unity 6000.3.24 with Entities 1.4.8, unless a line says otherwise.

-   **Main-thread time limit.** Pipeline cuts any command off after 30 seconds
    on the main thread, whatever `--timeout` says.
    -   A build can't run inside `run_script`. The Editor script checks the
        request and schedules the build with `EditorApplication.delayCall`.
        When the build finishes, it writes its result to
        `build-status.json`, and the CLI polls that file.
    -   The scheduled delegate lives in the `run_script` assembly, which stays
        loaded for the length of the build.
-   **Failures before the build steps aren't recorded.** These include an
    Entities build processor exception, player script compilation errors and
    Burst errors.
    -   The report then says Failed, with a zero GUID and no steps or messages,
        or with the messages missing.
    -   `build` subscribes to `Application.logMessageReceived` for the length of
        the build and returns those errors.
-   **A successful build lists everything it wrote**, including StreamingAssets.
    In a failed 6000.3.24 build the report lacked the exe, UnityPlayer.dll and
    StreamingAssets. For a report with no StreamingAssets entries, the tool
    counts those files from disk, and falls back to the files' ages to find
    stale subscenes.
-   **Unity never cleans the output folder.** In the real lsss build
    (6000.3.18, reported in place):
    -   38 content archives (26.8 MB) weren't in the catalog: leftovers of
        earlier builds, which shipped anyway;
    -   44 files in all (27.3 MB) weren't written by that build.
-   **Builds rewrite project files.**
    -   URP rewrites its shader-prefiltering fields in the pipeline asset, and
        its global settings, on every build.
    -   Input System adds its settings to `preloadedAssets` at the start of a
        build and removes them at the end. A failed build leaves them there.
    -   The Performance Testing package writes `Assets/Resources/
        PerformanceTestRun*.json` into every build and deletes the files after
        a successful one. A failed build leaves them, and an empty
        `Assets/Resources/` folder is left behind in either case.
    -   `build` lists every file under `Assets/` and `ProjectSettings/` that
        changed during the build.
-   **Unity can reuse a buildGUID.** Two consecutive builds to the same folder
    got the same GUID, so archived reports are identified by start time and
    GUID together.
-   **Plugins for other CPUs ship anyway.** Kinemation's ARM64 `AclUnity.dll`
    is marked for the ARM64 CPU only, yet a Win64 x64 build copies it to
    `Plugins/ARM64/`. The tool reports this as a note.
-   **The `*_DoNotShip` folder is never in the report.** It holds the Burst
    debug information: 1.7 MB in a release build, 88 MB in a development build.
    The tool adds it from disk under "debug symbols".
-   **Sizes are binary.** "MB" means 2^20 bytes throughout, as in Unity's own
    tools.
-   **Assets used by both scenes and subscenes ship twice.** BW_Shared.mat
    shipped in `sharedassets0.assets` and in a content archive, and so did
    URP's default `Lit` material.
-   **Player builds of this workspace failed on two add-on bugs, both fixed in
    this session:**
    -   Shuriken's ParticleSystemBaker read `Light.lightmapBakeType`, which
        exists only in the Editor. Now inside `#if UNITY_EDITOR`.
    -   Peekaboo's AVX projection compiled for the SSE2 player target (BC1200)
        even though every call to it was inside `IsAvxSupported`. Burst checks
        each compiled function on its own, and AggressiveInlining didn't change
        that. Each function that uses AVX now checks `IsAvxSupported` itself.
        Burst decides that check at compile time, so it costs nothing.

## Verification

-   **Chunk capacity.** An oracle (`AgentScripts/BuildWaitress/CapacityOracle.cs`)
    creates every archetype found in the built sections from its type hashes,
    in a scratch World, and reads Unity's `EntityArchetype.ChunkCapacity`.
    39 of 39 matched, across two builds and 11 sections. These covered
    rendering, skinned mesh, Mecanim, Peekaboo, Anna physics, LifeFX, Mash and
    meta-chunk archetypes.
-   **Offsets and blob owners.** Blob reference counts equal the entity counts
    that hold them: 40 colliders share one TriMesh blob, 200 grids share one
    occluder blob, and 806 Cardboard references point at one blob. A wrong
    offset would read garbage and count nothing.
-   **Sizes.** The sizes recorded in section headers equal the file sizes.
    Node sizes add up to the file size.
-   **Content.** The catalog parse agrees with Entities' own
    `archive_dependencies.txt`. ReferencedUnityObjects led to the exact meshes
    and materials each scenario subscene uses (BW_Grid and BW_SubOnly in SubA;
    BW_Shared and the built-in cube in SubB).
-   **Reports.** Real reports were read:
    -   2021.3.5f1 (DOTS Subscene Bugs);
    -   6000.0.4f1 (Blasting-Blocks);
    -   6000.3.18f1 (lsss-wip, 13 subscenes);
    -   6000.3.24f1 (this workspace, three builds).
-   **Self-tests.** `buildwaitress selftest` checks LZ4, the common-string
    table, class ids and file categories.

## Subagent rounds

Each round ran one subagent at a time.

**Round 1 (7 productivity tasks).** Every task was answered, in 1 to 4 calls
each. Fixed afterwards:

-   `why` on a content archive's name, and `why` naming the chain from a section
    to an object (texture ← material ← section);
-   compressed against uncompressed sizes for content archive objects;
-   archives included in `assets --by file`;
-   the debug-symbol check;
-   `diff` warning about unrelated builds;
-   real paths in `diff`;
-   the BuildOptions names, which were wrong from bit 7 upward.

**Round 2 (exploratory, about 45 calls).** Fixed afterwards:

-   **A serious bug.** The project root came from command-line paths, so naming
    another project's report archived it into that project's `UserSettings`,
    and installed the Editor script there too. That happened for 13 projects,
    in my own tests and the subagent's. The folders were removed (each held
    only files the tool wrote that day), and the root now comes only from
    `--project` or the working directory.
-   Two crashes: a 6000.6 report (serialized file version 23, now a clear
    "unsupported" error), and a junk file among the content archives.
-   Damage handling:
    -   truncated and swapped section files: the size is checked against the
        header;
    -   orphan section files;
    -   truncated reports;
    -   empty folders.
-   `--clean` as a plain flag, `--build` on every command, and `--folder` to
    pair a report with a moved build.
-   Per-command help, argument checks, and the archive identity above.
-   `diff`: correct scene counts, and snapshots for builds without subscenes.

**Round 3 (5 verification tasks).** Four tasks were answered with high or
medium confidence, but the lsss one couldn't link objects to sections. That
led to finding the second catalog form. Fixed afterwards:

-   that catalog form;
-   leftover archives kept out of `assets`;
-   empty chunk bytes shown, and a check rule based on them;
-   `*_DoNotShip` contents counted;
-   the plugin note;
-   MonoScript churn summarized in `diff`.

Left open from round 3:

-   No command answers "what shouldn't ship" beyond debug symbols, stale files
    and foreign plugins. Test-looking StreamingAssets folders, for example,
    need a human's judgment.
-   Lsss archetypes show "capacity ?", because component sizes come only from
    an export of the project that built them.

## Known unsupported

-   **Component values.** Sections are read for structure, sizes and blob
    references, not field values. Decoding values needs field layouts, which
    the `UNITY_DOTS_IMHEX` patterns or an Editor export could supply. Planned
    for a later release.
-   **Asset bundles and Addressables.** Bundle build reports read like player
    reports. Addressables' own layout reports aren't read.
-   **Shader variant counts** aren't in the build report. They are only in the
    Editor log.
-   **Code stripping detail** (`StrippingInfo`) is decoded only where a report
    carries it, which is IL2CPP builds. It isn't summarized yet.
-   **Moved build folders.** A report records absolute paths. A folder that
    moved is read on its own, by naming it, without its report.
-   **Older Entities formats.** Section files from before 1.0 (pre-content
    archives) are counted by size but not decoded. Header version 3 and file
    format 78 are what was tested.
-   **Machine-readable output** (`--json`) isn't offered. Every command
    prints compact text, and `check` signals problems through its exit code.
-   **Content archive compression.** LZMA-compressed archives aren't decoded;
    LZ4 and uncompressed ones are.
