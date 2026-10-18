# PrefabWaitress Design

## What It Answers

Agents edit prefabs and scenes all the time, and prefab overrides are where those
edits go silently wrong. An agent changes a default on a prefab and believes it
reached every copy, but thirty instances override that field. Or it reads a
scene's YAML, sees no value for a property, and reports the base prefab's value,
missing the variant in between. Nothing errors in either case. The tool exists so
those answers come from the same resolution the Editor performs, in one call.

## Reading Offline

Reading never needs the Editor. That keeps it fast, makes it work while the
Editor is busy or closed, and lets one call cover hundreds of files.

**YAML.** Unity's dialect is parsed into a tree: block mappings and sequences,
flow mappings and sequences (including ones Unity wraps across lines), plain,
single-quoted and double-quoted scalars with folding and escapes. Each document
keeps its class id, file id, the `stripped` flag and its line.

**The asset index.** Every `.meta` under `Assets/`, `Packages/`, the package
cache and local packages named in `manifest.json` gives the GUID-to-path map, in
about 0.5 s for this project. A MonoBehaviour's type is the class named by its
script file. Built-in meshes and the default material get their names.

**Resolving an instance.** For each `PrefabInstance` in a file:

1.  The source asset is loaded and resolved first, recursively, so a variant of
    a variant, or a prefab nested in a prefab nested in a scene, are all the
    same case. A cycle is reported, not followed.
2.  Every object of the source becomes an object of this file. Its id is the one
    a stripped object in this file gives it. When the file doesn't mention it,
    the id is `(instance id XOR source id) & 0x7FFFFFFFFFFFFFFF`. Files written
    before Unity 2018.3 don't follow the XOR rule for the objects they mention,
    so the stripped objects come first.
3.  Removed components and removed GameObjects (with their subtrees) are dropped.
4.  The source's values are copied only when an object is first read. References
    between source objects are pointed at their copies, and the instance root's
    parent becomes the instance's `m_TransformParent`. Instanced objects start
    with hide flags 0, as in the Editor.
5.  Overrides apply in file order. Each one either sets its value, recording
    itself as the setter of that property path, or gets a status: `NoTarget` (the
    object is gone from the source), `NoProperty` (the source has no such path),
    `Redundant` (applied, but equal to the value before), or `Unchecked` (the
    source can't be read). Array sizes resize the list.
6.  The hierarchy is linked. Components and children come from the source in
    order. Added ones go at their recorded insert index, or in the order of the
    instance's added-object records, or by their `m_RootOrder` override in files
    from before Unity 2022. A scene's root order comes from `SceneRoots`, which
    lists instance roots by their `PrefabInstance` id.
7.  A prefab's root is named after its file, as the Editor does on load.

Each object keeps a link to the object it came from, so the chain from a scene
down to the base prefab is always available. Its record of which override set
each property spans every layer below it.

**Usage.** `instances` finds files that use a prefab by scanning every scene and
prefab for `m_SourcePrefab` GUIDs without parsing them, then following those
links backwards transitively. Only the files that actually depend on the prefab
are parsed.

## Why Edits Use the Editor

Writing overrides into YAML directly is possible, but every rule above would
have to be inverted correctly. That means new stripped objects, XOR ids, added
object records with insert indices, and default overrides. A mistake there
corrupts a scene quietly, and an agent can't check it without the Editor anyway.
`SerializedObject` and `PrefabUtility` already do all of it, record overrides as
the Inspector would, and save the file in the Editor's own form. So an edit is
one `run_script` call carrying every operation. The Editor validates every
lookup and value before it changes anything. Objects are found by child index
and the name at each level is checked, so a file that changed since the tool
read it fails instead of editing the wrong object. After the save, the tool
reads the file back and prints what is now there, which is the confirmation an
agent needs without human review.

Unity's `JsonUtility` can't fill arrays of types compiled by `run_script`, so the
Editor script reads its request with a small JSON reader of its own.

## Models

Model importers create their objects at import time, so a scene instancing an
`.fbx` has nothing to read for them. `models` asks the Editor to write each
instanced model's objects as Unity YAML, with their local file ids, built from
`SerializedObject`. Those files sit in `UserSettings/`, stamped with the write
times of the model and its `.meta`, and are ignored once either changes. Until a
model is exported, overrides on its objects are `Unchecked`. Objects from it that
no file mentions don't appear.

## Verification

**Against the Editor.** `AgentScripts/PrefabWaitress/PrefabOracle.cs` opens each
scene, or loads each prefab, and writes every object's `SerializedObject`
properties in the format of the hidden `dump` command. Objects that come from an
instance are written as `instance:source` ids, so files that predate the XOR
rule can be matched. `prefabwaitress oraclecheck <dump>` compares values,
references and hierarchy and groups the differences.

| Set | Files | Values | Differences |
|---|---|---|---|
| Validation scenario (saved by Unity 6000.3) | 5 | 4175 (172 overridden) | 0 |
| Every prefab in the project | 807 | 1.73 M | 603, all on-load changes |
| Every scene with prefab instances | 23 | 1.18 M (14 K overridden) | 4552, all on-load changes |

Every difference left is something Unity changes when it loads a file, not
something the file says:

-   fields that newer Unity versions add or rename, which files from older
    versions lack (`m_LinearDamping`, include/exclude layers, raycast padding);
-   data scripts upgrade on load (TextMeshPro alignment, URP light data version,
    old `UnityEngine.UI` DLL script references, MeshCollider cooking flags);
-   RectTransforms a Canvas drives, and lightmap indices supplied by lighting
    data;
-   legacy prefab files that store hide flags 1 on their own objects;
-   references to assets that no longer exist (the tool says `missing asset`),
    and stale source links on objects disconnected from their prefab;
-   one ParticleSystem whose shape arc mode Unity normalizes in a scene.

Every hierarchy matches, including component order, child order and the root
order of scenes. Those differences shaped four rules above: the stripped-id map,
`SceneRoots` naming instances, the `m_RootOrder` and record-order rules for
added children, and the prefab root's name.

**Self-test.** `prefabwaitress selftest` builds a small project of hand-written
YAML: a base, a variant with every kind of override, a scene whose stripped ids
break the XOR rule, and a prefab nesting the base with a removed component. It
checks each rule. Breaking the stripped-id map, the reference remapping or the
`SceneRoots` rule each fails its test.

**Edits.** Every edit path was run against the scenario: set (numbers, vectors,
strings, same-file and asset references), revert (property, component, added
object, added component, removed component), apply to a base prefab with `--to`,
and clean (unused and redundant). The file read back after each edit showed the
expected values.

**Subagents.** Two rounds, one at a time. The first did productivity tasks and
checked them in the Editor; the second built situations meant to break the tool:
variant chains four deep, prefabs nested three deep with overrides at every
level, twin nested prefabs with identical names, odd names, several components
of one type, list and reference cases, models, SerializeReference, and edits
against dirty scenes and Prefab Mode. Their findings shaped three safety rules:

-   `clean` has the Editor confirm each candidate on the instance's own object,
    and never removes overrides in SerializeReference data.
-   New list elements start zeroed, as Unity creates them.
-   `apply` defaults to the prefab the file instances. Unity's
    `GetPrefabAssetPathOfNearestInstanceRoot` returns the innermost nested
    prefab, which would change every user of that inner prefab. When `--to`
    reaches past prefabs that override the property, the tool says that Unity
    clears those overrides.

## Known Unsupported

-   Binary-serialized scenes and prefabs (Asset Serialization Mode set to
    binary) can't be read.
-   Fields a file lacks because it predates them are not filled in. The tool
    doesn't know script defaults or Unity's upgrade rules, so it reports what the
    files hold, and `why` says when no layer writes a property.
-   `[SerializeReference]` data is read as plain YAML. Overrides inside managed
    references (`managedReferences[id].field`) aren't applied; they are listed as
    `unchecked`, and `clean` never removes them.
-   Lists Unity stores as packed bytes (`int[]` and other primitive arrays print
    as hex) aren't decoded. Overrides on their elements are listed as
    `unchecked`. Overrides inside list elements another override added are
    `unchecked` too.
-   Enums print as numbers. `set` accepts a single enum name, but not flag
    combinations.
-   Objects from a missing prefab can't be shown in `tree` or addressed;
    `overrides` lists what the instance sets on them.
-   Output names a component `Type`, even when the object has several of that
    type (`Type[n]` is accepted as input).
-   Models must be exported with `models` (once, and again after a reimport)
    before their objects can be read.
-   Edits need the Editor and can't run in Play Mode. There is no offline write
    path, by design.
