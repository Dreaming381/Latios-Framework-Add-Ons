# BuildWaitress

BuildWaitress is a command-line tool for Unity player builds, and the ninth
Waitress tool. It's built for AI agents. It answers:

-   what a build contains, how big each part is, and why each asset ships;
-   what changed between two builds, and why a build failed;
-   what is wrong with a build folder;
-   what is inside the build's Entities subscenes.

Questions about a build are usually these:

-   Why is my build 300 MB, and what is the biggest part of it?
-   Why is this texture in the build at all?
-   What grew since the last build?
-   Why is this subscene 3 MB when it only has 150 entities?
-   Why did the build fail? The report only says "Failed".
-   Is this folder safe to ship, or does it still hold files from older builds?

An agent can't read any of this directly:

-   **The build report** is a binary file, `Library/LastBuild.buildreport`, and
    Unity overwrites it on every build.
-   **Subscenes** in a DOTS build are written as `.entityheader` and `.entities`
    files and content archives under `StreamingAssets`. Unity's report lists
    them, but says nothing about what they hold.

BuildWaitress reads all of it offline. It archives every report so that builds
can be compared, and it can also run a build and report on it in one call.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/BuildWaitress/release/buildwaitress[.exe]`.

Only `build` and `types` need the Editor open with the Pipeline package (0.7 or
newer). The tool carries its Editor script inside itself, writes it to
`UserSettings/Waitress/BuildWaitress/`, and runs it through `run_script`, so
nothing is imported into the project.

Install the agent skill with
`buildwaitress skill --install <project>/.claude/skills/buildwaitress`.

## Naming a build

Commands take `last` (the default), `prev` or `~N` for older builds, a
`.buildreport` file, or a build folder, either as an argument or with
`--build`. `--folder <dir>` pairs a report with a copy of its build that was
moved. Sizes are binary: 1 MB is 2^20 bytes.

The tool copies each new `LastBuild.buildreport` to
`UserSettings/Waitress/BuildWaitress/Reports/` the first time it runs after a
build. At the same time it records a snapshot of the subscene sizes, while the
build folder still matches the report. Reports from other projects and older
Unity versions (2021.3 and later) read the same way.

## Commands

| Command | What it gives |
|---|---|
| `summary` | Result, options, time and total size, split into native code, managed code, scenes and assets, subscenes, subscene content, streaming assets and debug symbols. Also the largest assets, subscene totals and the problem count. |
| `files [filter] [--by folder]` | Output files by size. |
| `assets [filter] [--by asset\|type\|folder\|package\|file]` | Assets totalled per source asset, with texture and mesh data in `.resS` included, plus the objects in subscene content archives. |
| `why <asset>` | Every place an asset is stored, its size, and why it ships: Resources, the scenes that use it (needs a `--detailed` build), or the subscene sections that reference it. |
| `subscenes [name] [--archetypes]` | Per section: chunks, blobs and buffers by size; entities; archetypes; chunk capacity and fill; blobs by owning component; chunk bytes by component; referenced meshes, materials and textures. |
| `steps [--all]` | The slowest build steps, and every error and warning with its step. |
| `check` | Problems, each with a fix (list below). Exits 1 on errors or warnings. |
| `diff [old] [new]` | Size changes: total, by category, files, assets and subscene sections. |
| `history` | Archived reports. |
| `build [scene...]` | Builds the player and waits for it. Then it prints the summary, the errors logged before Unity's build steps (the report never records these), and the project files the build changed. `--target`, `--out`, `--dev`, `--detailed`, `--clean`. |
| `types` | Exports Entities component names and sizes. This runs on its own when the export is older than the scripts. |

`check` reports:

-   a failed build, with its first errors;
-   files in the build folder that this build didn't write, which still ship;
-   content archives the catalog doesn't list, and missing archives it does;
-   subscene files older than the build;
-   assets stored twice, in scene data and in subscene content;
-   subscene sections where at least 128 KB, and a quarter of the file, is
    empty chunk space;
-   section files that no header lists;
-   debug symbols, including `*_DoNotShip` folders, in a release folder;
-   plugins for another CPU (as a note);
-   test assemblies, deep profiling and script debugging.

## Examples

```
buildwaitress build --target win64 --clean       # build, then the summary and anything the build changed
buildwaitress why Assets/UI/Atlas.png             # 2 MB in resources.assets.resS; it is in a Resources folder
buildwaitress subscenes Mission5 --archetypes     # 26 archetypes in 26 chunks for 153 entities, 2.5 MB of collider blobs
buildwaitress diff                                # prev → last: +1.2 MB, Texture2D Assets/Art/Sky.png +1 MB
buildwaitress check ../OtherProject/Library/LastBuild.buildreport
```

## Limits

Read the "Known unsupported" section of
[BuildWaitress-DESIGN.md](BuildWaitress-DESIGN.md). In short:

-   component values inside subscenes aren't decoded;
-   asset bundle and Addressables builds are read only as far as their report
    goes;
-   shader variant counts aren't reported;
-   IL2CPP and code stripping detail only goes as far as the report records it;
-   reports keep no record of where they came from, so a moved build folder
    needs to be named directly.
