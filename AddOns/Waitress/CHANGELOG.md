# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic
Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.0] – 2026-10-17

### Added

-   Added *ProfilerWaitress* for recording and analyzing Profiler captures
-   Added *MemoryWaitress* for taking and analyzing Memory Profiler snapshots
-   Added *PrefabWaitress* for resolving and editing prefab override chains
-   Added *ModelWaitress* for inspecting model imports and changing their
    settings
-   Added *ClipWaitress* for inspecting, validating, and editing AnimationClips
-   Added *TerrainWaitress* for inspecting and editing terrains
-   Added *BuildWaitress* for analyzing and comparing player builds, including
    the contents of Entities subscenes
-   Added *BurstWaitress* for inspecting and comparing the code Burst generates,
    and checking whether Burst runs for a method
-   Added ShaderWaitress `target add` and `target rm` for graphs that target
    multiple render pipelines
-   Added ShaderWaitress `normalspace` target setting
-   Added VfxWaitress `resource set` for asset-level settings such as update
    mode, culling, instancing, and prewarm
-   Added VfxWaitress key syntax for curve and gradient slot values

### Changed

-   ShaderWaitress `target set` now applies to every target that has the
    setting unless `--id` is specified
-   VfxWaitress `show` now prints asset-level settings by name instead of by
    number

### Fixed

-   Fixed the Editor Bridge samples failing to compile with Pipeline 0.8.0 or
    newer
-   Fixed ShaderWaitress writing HDRP target settings where Unity ignores them
-   Fixed VfxWaitress accepting curve and gradient values that crash the Editor
    on import
-   Fixed VfxWaitress creating graphs with instancing disabled
-   Fixed VfxWaitress parsing of sequences whose items span multiple lines

### Improved

-   Improved ShaderWaitress validation to detect normal blocks that don’t match
    a target’s normal space
-   Improved VfxWaitress validation to detect flow links recorded on only one
    context
-   Improved errors for misspelled options and extra arguments

## [0.1.0] - 2026-9-27

### This is the first release of *Waitress*.
