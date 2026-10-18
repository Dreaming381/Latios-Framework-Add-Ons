---
name: buildwaitress
description: Use for Unity player builds and build reports - what a build contains and why, how big each part is, what changed between two builds, why a build failed, stale files shipping from earlier builds, and what is inside Entities subscenes (entities, archetypes, chunk fill, blobs, referenced meshes/materials) - instead of opening the Build Report inspector or reading Library/LastBuild.buildreport or StreamingAssets binaries.
---

# buildwaitress

`buildwaitress` reads Unity build reports and build folders offline, including the Entities subscene files
(`StreamingAssets/EntityScenes/*.entities`, `ContentArchives/`) that Unity's own report doesn't look inside. Only
`build` and `types` need the Editor. Binary: `UserSettings/Waitress/build/bin/BuildWaitress/release/buildwaitress.exe`
(build it with `dotnet build <addons>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build`).

## Naming a build

Commands take `[build]` (or `--build b`): `last` (default), `prev` / `~N` (earlier builds), a `.buildreport` file,
or a build folder. `--folder <dir>` pairs a report with a moved copy of its build. Sizes are binary (MB = 2^20 bytes). Unity overwrites `Library/LastBuild.buildreport` on every build; the tool archives each new one
the first time it runs, so `diff` and `history` work across builds. Run any command right after a build.

## Commands

- `summary` - result, options, time, total size by category (native code, managed code, scenes and assets, subscenes,
  subscene content, streaming assets, debug symbols), largest assets, subscene totals, and the problem count.
- `files [filter] [--by folder]` - output files by size. Filter by text, glob, or category name.
- `assets [filter] [--by asset|type|folder|package|file]` - packed assets totalled per source asset (texture/mesh
  data in `.resS` included), plus objects in subscene content archives. `assets Texture2D`, `assets --by package`,
  `assets subscenes`.
- `why <asset path|file name|object name|content archive>` - each place it's stored, its size, and why it's
  included:
  - in a Resources folder;
  - used by which scenes (needs a `--detailed` build);
  - the chain from a subscene section to it, e.g. `Material 'X' ← Sub section 0`.

  For a content archive file name, it lists the objects inside, both compressed and uncompressed.
- `subscenes [name] [--archetypes]` - for each subscene section:
  - the size split into chunks (16 KB each), blobs, and buffers outside chunks;
  - entities, archetypes, chunk capacity and fill;
  - blobs grouped by the component that references them;
  - the Unity objects it references.

  Name one subscene, or pass `--archetypes`, to list archetypes with their components.
- `steps [--all]` - slowest build steps, plus every error and warning with the step it came from.
- `check` - problems, each with a fix. Exits 1 on errors or warnings. It reports:
  - failed builds;
  - files left by earlier builds, which ship anyway;
  - content archives that aren't in the catalog;
  - assets stored twice, in scene data and in subscene content;
  - sections whose chunks are mostly near-empty;
  - test assemblies and debug options.
- `diff [old] [new]` - size changes, by default `prev` → `last`: total, by category, files, assets, subscene
  sections.
- `history` - archived reports.
- `build [scene...] [--target win64|linux|mac|android|webgl] [--out path] [--dev] [--detailed] [--clean]` - builds
  in the open Editor and waits. Then it prints:
  - the summary;
  - errors logged before Unity's build steps, which the report never records (script compilation, Burst, build
    processors);
  - project files the build changed.

  It refuses while a scene has unsaved changes. `--clean` deletes the old output folder first.
- `types` - exports Entities component names and sizes. Usually automatic, whenever the export is older than the
  scripts and an Editor is open. Without it, names come from `Logs/ExportedTypes.log`, and there are no sizes,
  chunk fill or blob owners.

## Notes

- A subscene section is mostly chunks × 16 KB plus blobs. Many archetypes with 1-2 entities each mean many
  near-empty chunks.
- Unity doesn't clean old builds out of the output folder, and especially not out of StreamingAssets. `check` finds
  files the report doesn't list, and content archives the catalog doesn't list.
- `diff` compares subscenes from snapshots. A snapshot is taken when the tool first sees a report while its folder is
  intact.
- Reports from other projects or Unity versions (2021.3 and later) read fine. Subscene type names then come from
  that project's `Logs/ExportedTypes.log`.
