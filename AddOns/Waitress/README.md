# Waitress

Has your agent ever complained to you that trying to edit an asset would be very
difficult or expensive? Mine has. So I had AI build cli tools to solve this
problem.

## Features

-   ShaderWaitress – Works with `.shadergraph` and `.shadersubgraph` assets
-   VfxWaitress – Works with `.vfx`, `.vfxoperator`, and `.vfxblock` assets
-   Provides agents succinct query and edit APIs for graph topology and settings
-   Automatically computes node positions for human readability without the
    agent having to care

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

### The Tools

Waitress contains a pair of command-line tools that let an AI agent read, query,
edit, and build Unity's graph assets without opening them in the Editor.

-   **ShaderWaitress** handles `.shadergraph` and `.shadersubgraph`
-   **VfxWaitress** handles `.vfx`, `.vfxoperator`, and `.vfxblock`

These assets are huge, and neither Shader Graph nor VFX Graph has a public
authoring API. A nine-node VFX effect is 2500 lines of YAML, and a single-node
`.vfxoperator` can pass 6900. Reading one raw eats more of an agent's context
than the task it was asked to do. Waitress prints the same graph in a few dozen
lines, answers questions about it in one call, and makes edits without the agent
ever touching a coordinate or an object ID.

Waitress ships no runtime code and no assemblies. The tools live in the
`Waitress~` folder, which Unity ignores because of the trailing `~`, and the
Editor scripts the tools talk to ship as package samples due to asmref
requirements.

### Building

Run this from your project's root folder:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

`<addons package>` is wherever Unity put the add-ons package, usually
`Packages/com.latios.latiosframework.addons` or
`Library/PackageCache/com.latios.latiosframework.addons@<hash>`. The
`--artifacts-path` keeps every build file in your project's `UserSettings/`, so
nothing gets written into the package itself. The tools end up at
`UserSettings/Waitress/build/bin/ShaderWaitress/release/shaderwaitress` and
`UserSettings/Waitress/build/bin/VfxWaitress/release/vfxwaitress`.

`UserSettings/` survives deleting `Library/`, and Unity's default `.gitignore`
already leaves it out of source control. Rebuild after updating the add-ons
package so the tools match the Editor Bridge samples.

Each tool teaches itself to an agent through `help`. `skill` prints a one-page
guide written for agents, and `skill --install <dir>` saves it as a `SKILL.md`
for agent harnesses that support installable skills.

### The Editor Bridge Samples

In the Package Manager, select *Latios Framework Addons*, open the *Samples*
tab, and import what you need:

-   **ShaderWaitress Editor Bridge**
-   **VfxWaitress Editor Bridge** (only if you have VFX Graph installed)

Each sample registers `shaderwaitress_*` or `vfxwaitress_*` commands with the
Pipeline package, and the tools call them through `unity command`. You can see
them with `unity command --query waitress`. Each sample also includes an
`.asmref` that compiles it into Shader Graph's or VFX Graph's editor assembly,
since both keep their authoring model internal.

### Exporting the Node Catalog

Neither tool knows any node types until you export a catalog from your project.
URP and HDRP register different nodes, and your project can add its own, so a
catalog baked into the tool would be wrong for almost everyone. With the samples
imported and the Editor open, run:

```
unity command shaderwaitress_export_catalog
unity command vfxwaitress_export_catalog --timeout 120
```

The menu items *Tools \> ShaderWaitress \> Export Node Catalog* and *Tools \>
VfxWaitress \> Export Catalog* do the same. The catalogs go in your project's
`UserSettings/Waitress/` folder, which Unity's default `.gitignore` already
excludes. Re-export whenever you upgrade Unity, switch render pipelines, or add
custom nodes.

### Editing Graphs That Are Open

An open Shader Graph or VFX Graph window keeps its own copy of the graph. A tool
that rewrote the file outside the editor would either lose the person's unsaved
changes or leave the window asking whether to reload, and that dialog can't be
answered when the Editor runs with `-automated`. So every write first saves what
the window hasn't, then hands the result back so the window reloads it quietly.
The Editor also rewrites the file in the exact form it would save itself, so the
reloaded window doesn't show unsaved changes.

This needs the Editor running with the matching sample imported. Pass
`--offline` to skip it.

### More Details

Each tool has a README and a design document in `Waitress~/docs/`. `Design
Rules.md` in the same folder lists the rules both tools are built around.
