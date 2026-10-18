# Waitress

Has your agent ever complained to you that trying to edit an asset would be very
difficult or expensive? Mine has. So I had AI build cli tools to solve this
problem.

## Features

-   ShaderWaitress – Queries and edits `.shadergraph` and `.shadersubgraph`
    assets
-   VfxWaitress – Queries and edits `.vfx`, `.vfxoperator`, and `.vfxblock`
    assets
-   ProfilerWaitress – Records and analyzes Profiler captures (`.data` and
    `.raw`)
-   MemoryWaitress – Takes and analyzes Memory Profiler snapshots (`.snap`)
-   PrefabWaitress – Resolves and edits prefab overrides through variants and
    nested prefabs
-   ModelWaitress – Inspects model imports and changes their settings
-   ClipWaitress – Inspects, validates, and edits AnimationClips
-   TerrainWaitress – Inspects and edits terrain heights, paint, trees, details,
    and settings
-   BuildWaitress – Analyzes and compares player builds, including the contents
    of Entities subscenes
-   BurstWaitress – Reports the code Burst generates and whether Burst runs at
    all

## Getting Started

**Scripting Define:** None. Waitress has no runtime code to enable.

**Requirements:**

-   The .NET 10 SDK or newer to build the tools
-   The Unity CLI with the Pipeline package
-   VfxWaitress needs the Visual Effect Graph package
-   Validated against Unity 6000.3 with Shader Graph and VFX Graph 17.3

**Main Author(s):** Dreaming I'm Latios

**Support:** Feel free to reach me through any of the same channels you would
use for the Latios Framework!

**Disclaimer:** This add-on was developed by AI with conversational assistance.
If it works for you, great! If not, ¯\\*(ツ)*/¯

### Building

Each tool is a command-line program for an AI agent to run. Build them all by
running this from your project's root folder:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

`<addons package>` is wherever Unity put the add-ons package, usually
`Packages/com.latios.latiosframework.addons` or
`Library/PackageCache/com.latios.latiosframework.addons@<hash>`. The
`--artifacts-path` keeps every build file in your project's `UserSettings/`, so
nothing gets written into the package itself. Each tool ends up at
`UserSettings/Waitress/build/bin/<Tool>/release/<tool>`, such as
`UserSettings/Waitress/build/bin/ShaderWaitress/release/shaderwaitress`. Rebuild
after updating the add-ons package.

Each tool teaches itself to an agent through `help`. `skill` prints a one-page
guide written for agents, and `skill --install <dir>` saves it as a `SKILL.md`
for agent harnesses that support installable skills.

Waitress ships no runtime code and no assemblies. The tools live in the
`Waitress~` folder, which Unity ignores because of the trailing `~`. They read
offline wherever they can, and reach the Editor through the Pipeline package
when they can’t. Except for the graph tools, each tool carries its Editor script
inside itself and runs it with `run_script` (Pipeline 0.7 or newer), so nothing
gets imported into your project.

### Graph Tools

ShaderWaitress and VfxWaitress print a graph in a few dozen lines instead of
thousands of lines of JSON or YAML, and edit it without the agent ever touching
an object ID. A whole editing session can be sent as one script.

**Layout belongs to the tool.** Agents never reason about coordinates. A new
node gets placed somewhere readable without moving anything else, and `layout`
only rewrites positions when the result scores as more readable.

**Editor Bridge samples.** Shader Graph and VFX Graph keep their authoring
models internal, so the graph tools’ Editor code ships as package samples that
compile into those packages’ editor assemblies. In the Package Manager, select
*Latios Framework Addons*, open the *Samples* tab, and import what you need:

-   **ShaderWaitress Editor Bridge**
-   **VfxWaitress Editor Bridge** (only if you have VFX Graph installed)

**Node catalogs.** Neither graph tool knows any node types until you export a
catalog, because render pipelines and custom nodes differ per project. With the
samples imported and the Editor open, run the following, and re-run it whenever
you upgrade Unity, switch render pipelines, or add custom nodes:

```
unity command shaderwaitress_export_catalog
unity command vfxwaitress_export_catalog --timeout 120
```

**Open graph windows are safe.** Every write saves what an open window hasn’t,
then has the window reload the result quietly, so no reload dialog ever blocks
the Editor. Pass `--offline` to skip the Editor.

### Profiling Tools

ProfilerWaitress and MemoryWaitress read captures that only Unity’s own windows
can open, and answer questions like “why is this frame slow” or “what keeps this
texture alive” in one call. Both can record new captures through the Editor,
and both `diff` two captures to show whether a change helped.

ProfilerWaitress has the Editor extract each capture once, then works offline.
MemoryWaitress reads snapshots directly, without the Editor or the Memory
Profiler package.

### Asset Tools

PrefabWaitress, ModelWaitress, ClipWaitress, and TerrainWaitress report the
values Unity actually uses, which the asset files alone don’t give: overrides
resolved through prefab chains, import results, evaluated curves, and terrain
heights between samples.

-   ModelWaitress, ClipWaitress, and TerrainWaitress have the Editor export each
    asset once per change, then read the export offline. PrefabWaitress reads
    scenes and prefabs directly.
-   Edits run through Unity’s own APIs in the Editor, and `--dry-run` shows what
    would change first.

### Build Tools

BuildWaitress reads Unity’s build report and the build folder offline, and
decodes Entities subscene sections and content archives directly. It archives
every report, so builds can be compared after Unity overwrites the report.
`check` finds problems such as stale files that still ship, and `build` runs a
build and reports the errors Unity’s report leaves out.

### Code Tools

BurstWaitress compiles Burst entry points through the Editor, the same way the
Burst Inspector does, and reports the result as facts: whether each loop
vectorized and how wide, alias checks and minimum trip counts, math calls that
stayed scalar, spills, and which C# lines the hot loop comes from. `sweep` and
`diff` compare methods, float modes and targets, including ARM and WASM without
a device. `status` says whether the Editor runs Burst code for a method right
now. The tool only reads Burst settings and never changes them.

### More Details

Each tool has a README and a design document in `Waitress~/docs/`. `Design
Rules.md` in the same folder lists the rules the graph tools are built around.
