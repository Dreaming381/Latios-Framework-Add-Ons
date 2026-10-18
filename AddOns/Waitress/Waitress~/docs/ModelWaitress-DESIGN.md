# ModelWaitress Design

## What It Answers

Troubleshooting a model tends to come down to the same handful of questions:

-   Why is it this size?
-   Why is it rotated?
-   Why is the avatar invalid, or the character in a T-pose?
-   Where did the bones go?
-   Why is a material slot empty?
-   Why does the clip not loop, or why does it slide?
-   Can this mesh be read at runtime?
-   Which models have this setting?
-   What is different between the model that works and the one that doesn't?
-   What uses this model?

Each answer needs the importer's settings, the result of the import, or a rule
for combining the two. An agent can't easily get at any of these:

-   **Settings** sit in the `.meta` file under serialized names. Some differ from
    the API names an agent knows (`materialImportMode` used to be
    `importMaterials`). Enums are stored as numbers, and nothing marks a value
    as non-default.
-   **The result** lives in binary artifacts in `Library/` that only the Editor
    reads.
-   **The rules** aren't documented. Examples are how `globalScale` and
    `useFileScale` combine, and when `optimizeGameObjects` does anything.

## Why an Editor Export

The alternative was to read `.meta` files offline. That was rejected for three
reasons:

-   The import result isn't in them, and the result is half of every answer.
-   Unity upgrades old settings when it loads them. One of the workspace models
    still has a `serializedVersion: 22` `.meta` file with keys that no longer
    exist. Matching the Editor would mean carrying Unity's upgrade rules.
-   Defaults aren't in the file, and the defaults change between Unity versions.

So the Editor writes one JSON record per model to
`UserSettings/Waitress/ModelWaitress/Models/<guid>.json`. Every read command
works from those records, offline and fast. The record holds:

-   every writable, non-obsolete `ModelImporter` property, with Unity's default
    for it;
-   the file's unit, its takes and the transform paths in the file;
-   each clip's settings, with the defaults of the take it comes from;
-   the humanoid description;
-   the remaps, and the materials in the file;
-   the import log, and the legacy animation warnings the importer keeps;
-   the imported result: hierarchy, components, meshes (layout, submeshes,
    bounds, blend shapes, bones per vertex), avatars, clips (length, looping,
    root motion, animated paths) and sub-asset ids.

A record is current while the model's and its `.meta`'s write times match the
stamp the Editor wrote. Any command first re-exports missing or stale models,
in one `run_script` call for each batch of 50. Before reading a model, the
export calls `ImportAsset`, which re-imports only a file that changed while the
Editor wasn't looking. With no Editor reachable, the last record is shown with
a note, and a model that was never exported fails with the command to open the
project.

Records are keyed by GUID, so a moved or renamed model overwrites its own
record. Every export, and every `refresh`, deletes records whose GUID no model in
the project has any more: deleted models, and models re-added without their
`.meta`.

Records are keyed by GUID, so a moved or renamed model overwrites its own
record. Every export, and every `refresh`, deletes records whose GUID no model
in the project has any more: deleted models, and models re-added without their
`.meta`.

Exporting the 90 models in this workspace takes 8.8 s. A stale model refreshes
in about 2 s.

### Defaults

Unity's default for a setting comes from a clone of the model's importer with
`Unsupported.SmartReset` applied to it. That reset was checked against a real
fresh import: a model was copied without its `.meta` into a temporary folder,
imported, and compared. All 76 settings matched. The default is Unity's own,
not a Preset Manager preset. `postprocessors` lists any default presets
separately.

### Names

Settings use their `ModelImporter` API names, and clip settings use
`ModelImporterClipAnimation` names. Those are what an agent finds in Unity's
documentation and writes in scripts. `set` uses the same names, so a value read
can be written back with the same key.

## Edits

`set` sends every change in one request. The Editor works in four steps:

1.  It checks every key and value for every model before changing anything.
    That covers enum names, asset paths, clip names, humanoid bone names,
    transform paths (a unique tail is enough), and the material names in the
    file. Misspellings get suggestions.
2.  It applies the changes through the API. Clips and the humanoid description
    are edited as copies and assigned once.
3.  It calls `SaveAndReimport`.
4.  It writes a fresh record.

The tool then prints each key's old and new value, and what changed in the
result: transforms, meshes, clips, avatar validity, material slots, and new or
cleared import messages. That printout is the confirmation, so no human has to
look at the Inspector.

Edits are refused in Play Mode. They are also refused while an importer
Inspector holds unapplied changes to that model, because those changes would
later prompt the person or overwrite the tool's edit. When an Inspector is open
on the model with no unapplied changes, it simply shows the new values. Both
cases were checked by driving the Inspector from a script.

## Measured Rules

These came from experiments on throwaway copies of workspace models. The
summaries state them.

-   **Scale.** The effective scale is globalScale × (useFileScale ? the file's
    unit factor : 1). The scale is baked into vertices and positions, and the
    root's scale stays 1. This was measured on a centimetre file with
    globalScale 1, 0.01 and 2, with Convert Units both on and off. The API's
    `fileScale` returns the unit factor only while useFileScale is on.
-   **optimizeGameObjects** does nothing without an avatar. With
    `avatarSetup NoAvatar` all 109 transforms of the Ellen model stayed. With
    `CreateFromThisModel` four remained: the root, the skinned mesh, and the two
    exposed transforms. When the hierarchy is optimized, a skinned mesh keeps an
    empty bone list, and exposed transforms move up to sit under the root.
-   **A remap to a deleted asset** loads as no object, so the slot is empty, and
    Unity logs nothing. The export reads the GUID the remap still holds from the
    `.meta` file, and `materials` names it.
-   **Humanoid auto-mapping can be wrong.** On a fresh copy of a Mixamo
    character, Unity mapped Head to `Alpha:Neck1` and RightEye to `Alpha:Head`.
    The original asset's hand-made mapping is different. `avatar` and `diff`
    show this directly.
-   **bakeAxisConversion** has no single rule. On the one Z-up file in the
    workspace (UnityMaterialBall, 3ds Max), turning it on put a 270° X
    rotation on the root, where there had been none. On a 3ds Max file that
    declares Y-up but carries a 270° node rotation, turning it on changed the
    root to 90° and mirrored the mesh centre. So the tool reports facts and
    makes no prediction: `info` shows the file's own header (application, up
    axis, unit, read straight from the .fbx), and `set` reports changes in
    root rotation.
-   **A stale humanoid skeleton.** After a scale change, the import warns
    "Avatar Rig Configuration mis-match", because the avatar's stored skeleton
    pose still has the old scale. Emptying `humanDescription.skeleton` makes the
    importer take a fresh pose, but it also re-runs the automatic bone mapping,
    which threw away a hand-fixed Head mapping. `human.skeleton=rebuild` does it
    in two imports and puts the mapping back. Checked at globalScale 0.5: no
    warning, avatar valid, Head mapping kept.
-   `ModelImporter.transformPaths` is empty until the model has an avatar. The
    tool takes the paths from the imported hierarchy then. Before an avatar
    exists, optimizeGameObjects does nothing, so the full hierarchy is there.
-   A few defaults depend on the importer's age. A reset importer kept
    `secondaryUVMarginMethod` at Manual for a model first imported long ago, and
    Calculate for a fresh copy. So "default" means Unity's default for that
    importer, which is what its Reset button gives.
-   An artifact's dependencies include a hash of the model postprocessors'
    versions. The record keeps it, but the tool can't read it offline, so a
    changed postprocessor doesn't make a record stale.

## Verification

-   **Self-test.** `modelwaitress selftest` runs 15 offline checks on a
    throwaway project. They cover record parsing, discovery (including skipping
    `~` folders and non-model importers), name and folder resolution, stamp
    staleness without an Editor, condition parsing and comparison, the scale
    rule, the optimizeGameObjects rule, mesh placement for the size line, binary
    and ASCII FBX headers, and help for every command. Three
    deliberate mutations were each caught: stamps ignored, `~` folders walked,
    and scale ignoring useFileScale.
-   **Export.** All 90 models in this workspace exported without failures:
    81 in `Assets/`, plus the package ones.
-   **Edits** run against `Assets/Validation/ModelWaitress/`. Each was read
    back:
    -   importer settings, including enums, floats and lists added to by unique
        path tails;
    -   clip settings, an added clip, an added event;
    -   humanoid remapping and unmapping, and a humanoid parameter;
    -   a material remap;
    -   rejected keys and values, with nothing applied.
-   **Staleness.** Three cases refreshed on the next read:
    -   a setting changed outside the tool;
    -   a model file replaced on disk while the Editor wasn't looking;
    -   a model file restored afterwards.
-   **No Editor.** Run against a closed project with no records, the tool fails
    in 0.45 s and names the command that opens it.
-   **Size.** `info` places every mesh's bounds by its node's transform, offline.
    An Editor script did the same with Unity's own `localToWorldMatrix` for all
    85 models in `Assets/`: every line matched after one fix (a `-0`). The
    self-test pins Unity's Z, X, Y euler order.
-   **FBX header.** It was read from all 94 `.fbx` files in the project, with no failures. The
    applications were 3ds Max, Maya, Blender and Houdini, and one file is Z-up.
    The self-test builds a binary and an ASCII file.

## Subagent Rounds

**Round 1 (tasks).** A subagent fixed four broken models without being told
the causes, and checked every fix with its own Editor script:

-   a character that was too small and had a wrong humanoid Head mapping;
-   optimizeGameObjects doing nothing;
-   a clip to loop, rename, and give an event;
-   Read/Write turned off.

It also answered find, users and diff questions. Every answer matched the
Editor. It found seven problems, all fixed:

-   `set` rejected transform paths on a model with no avatar.
-   A clip renamed in the same call was reported as missing.
-   A warning whose numbers changed was reported as new.
-   `diff` left out blend shapes and material slot results.
-   `tree <node> --all` ignored the node.
-   `-0` in sizes.
-   Weak suggestions for misspelled transforms.

From its usability notes, the tool gained:

-   a size line in `info`;
-   a "root motion: none, and why" line for Generic clips;
-   event times as a percentage and in seconds;
-   humanoid parameter changes in `info`;
-   every validation error at once, and `--dry-run`;
-   `clip.<setting>` keys in `find`;
-   per-command help for `--all`;
-   `human.skeleton=rebuild`, with a hint on the warning that needs it (it had
    spent four calls on that).

**Round 2 (exploratory).** A second subagent built its own cases in
`Assets/Validation/ModelWaitress/Explore/`. It used copied FBX files and
hand-written `.obj`, `.mtl` and `.dae` files covering 90k-vertex meshes,
duplicate and unicode material names, names containing brackets, `=` and `#`,
corrupt files and paths of 300+ characters. It checked every claim against the
Editor. These were fixed:

-   **`sourceAvatar`.** Assigning it left the importer's human description
    empty, so the copied rig had no bones and every clip vanished. The edit now
    copies the avatar's description too (measured: the Avatar asset carries it).
-   **Staleness from other files.** Records now list the files they depend on:
    remap targets, the copied avatar's model, clip masks, and an `.obj`'s
    `mtllib` files. A change to or deletion of any of them re-exports the
    record. Before, deleting a remapped material or editing a `.mtl` left
    answers stale.
-   **Broken humanoid mappings.** Unity marks a failed humanoid avatar valid but
    not human. The tool now reports it as NOT HUMAN, and
    `find avatar=invalid` finds it.
    -   `set` refuses mapping one transform to two bones, and unmapping a
        required bone.
    -   It refuses mapping edits in the same call that changes the rig, because
        that import auto-maps over them.
    -   It exits 1 when its import reports a new error.
-   **Optimized hierarchies.** An optimized skinned mesh keeps a placeholder
    transform, so its node no longer feeds the size line or the "rotated" line.
-   **Rejected input.** Out-of-range enum numbers, NaN, a non-positive
    globalScale, inverted or negative clip ranges, duplicate clip names,
    conflicting duplicate keys, a remap to the model's own materials, and a
    remap to a file holding several materials without `#name`.
-   **Name and path parsing.** `motionNodeName` resolves a unique tail to the
    full path Unity needs (a bare name extracts no motion). Clip and material
    names containing `]` or `=`, and asset paths containing `#`, can now be
    addressed.
-   **`human.skeleton=rebuild`** keeps every humanoid parameter, not just the
    mapping. `hasTranslationDoF` was being reset.
-   **Scale and unit.** Scale uses the importer's `fileScale`, which is the
    conversion actually applied. For `.dae` the unit factor wasn't one, and
    `.obj` has no unit at all. Units are claimed only for `.fbx`.
-   **Smaller fixes.**
    -   `users` reads other models' `.meta` files (remaps, copied avatars,
        masks).
    -   `diff` compares clip events.
    -   `find` has `maxverts` (the largest single mesh; `verts` is the total).
    -   `meshes` says "no UVs".
    -   `clips` explains a missing take.
    -   Import messages keep their file paths.
    -   Deprecated enum aliases are hidden.
    -   Errors name their model when several are edited.

## Known Unsupported

-   Only ModelImporter assets are read (and SketchUp, which derives from it).
    Texture, audio and ScriptedImporter models aren't.
-   The file header is read for .fbx only, not for .obj, .blend or other
    formats.
-   A changed AssetPostprocessor, a changed default preset, or a Unity upgrade
    doesn't make a record stale. Run `refresh --force`.
-   Setting `humanDescription` limits (muscle ranges), clip curves, and
    `extraUserProperties` values beyond the list itself isn't supported. An
    event's float, int and object parameters aren't settable either. They are
    shown where the export has them.
-   `users` reads text-serialized assets only, and reports references, not
    whether they're live (a reference inside an unused override counts).
-   Reading needs at least one export, so the first use of a model needs the
    Editor.
-   Model paths over 260 characters can't be exported (the Editor's file API
    fails on them).
-   Which AssetPostprocessor changed a given model isn't known; `postprocessors`
    lists the ones that run on every model import.
-   A failed import of a model whose file isn't a readable model at all
    exports whatever Unity produced (often nothing).
-   Loop-match quality, clip masks naming transforms the model lacks, and
    legacy clips dropping events aren't reported.
