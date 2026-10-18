# PrefabWaitress

PrefabWaitress is a command-line tool for reading and editing prefab override
chains in Unity scenes and prefabs. It's built for AI agents, and it's the fifth
Waitress tool next to ShaderWaitress, VfxWaitress, ProfilerWaitress and
MemoryWaitress.

A prefab instance in a scene or prefab file stores only its differences: a list
of overrides keyed by object ids that live in other files, plus the components
and objects it added or removed. Objects that come from nested prefabs aren't
written at all, and a variant of a variant adds one more file to follow. Reading
that YAML by hand gives the wrong value whenever any layer is missed, and an
agent that edits a prefab's default can't tell which instances override it.
PrefabWaitress resolves every instance the way the Editor does, so each value is
the one Unity uses and knows which file set it. It answers questions like "why
does this door have this material", "where is this prefab used" or "if I change
this default, which instances won't get it" in one call.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/PrefabWaitress/release/prefabwaitress[.exe]`.

Reading needs nothing else: no Editor, no package. Editing needs the Editor open
with the Pipeline package (0.7 or newer). The tool carries its Editor script
inside itself, writes it to `UserSettings/Waitress/PrefabWaitress/`, and runs it
through `run_script`, so nothing is imported into the project.

## Start Here

```
prefabwaitress tree Assets/Scenes/Level.unity              # hierarchy; prefab instances marked and collapsed
prefabwaitress show Assets/Scenes/Level.unity Door/Panel   # overridden values and the file that set each
prefabwaitress why Assets/Scenes/Level.unity Door/Panel MeshRenderer.m_Materials[0]
prefabwaitress overrides Assets/Scenes/Level.unity         # what every instance changes, adds and removes
prefabwaitress instances Assets/Prefabs/Door.prefab --prop DoorController.speed
prefabwaitress help                                        # command index
prefabwaitress skill                                       # the whole tool on one page, for an agent
```

## What It Looks Like

From the validation scenario (`Assets/Validation/PrefabWaitress/`): DoorRed is a
variant of DoorBase, DoorRedLocked a variant of DoorRed, House nests a DoorRed
and a DoorBase, and the scene instances all of them.

```
$ prefabwaitress tree PrefabScenario.unity
Assets/Validation/PrefabWaitress/PrefabScenario.unity: 29 GameObjects, 5 prefab instances, 67 overrides (1 unused)
House [House.prefab, 13 overrides] +11 below
DoorA [DoorBase.prefab, 11 overrides] +3 below
DoorBase [DoorBase.prefab, 19 overrides, 1 unused] +3 below
DoorC [DoorBase.prefab, 12 overrides] +2 below
DoorLocked [DoorRedLocked.prefab < DoorRed.prefab < DoorBase.prefab, 12 overrides] +4 below
Ground
```

```
$ prefabwaitress why PrefabScenario.unity House/FrontDoor/Panel MeshRenderer.m_Materials[0]
Assets/Validation/PrefabWaitress/PrefabScenario.unity House/FrontDoor/Panel MeshRenderer.m_Materials[0] = Assets/Validation/PrefabWaitress/Green.mat
  DoorBase.prefab       DoorBase/Panel         Assets/Validation/PrefabWaitress/Grey.mat   written there
  DoorRed.prefab        DoorRed/Panel          Assets/Validation/PrefabWaitress/Red.mat    override
  House.prefab          House/FrontDoor/Panel  Assets/Validation/PrefabWaitress/Red.mat    inherited
  PrefabScenario.unity  House/FrontDoor/Panel  Assets/Validation/PrefabWaitress/Green.mat  override
```

```
$ prefabwaitress instances DoorBase.prefab --prop DoorScenario.openAngle
DoorBase.prefab DoorBase DoorScenario.openAngle = 90
10 instances in 4 files: 3 follow the prefab, 7 keep their own value
  Assets/Validation/PrefabWaitress/DoorRed.prefab DoorRed = 120; inherited by House.prefab House/FrontDoor, PrefabScenario.unity DoorLocked, ...
  Assets/Validation/PrefabWaitress/PrefabScenario.unity House/BackDoor = 45
  Assets/Validation/PrefabWaitress/PrefabScenario.unity DoorBase = 100
  follow: House.prefab House/BackDoor, PrefabScenario.unity DoorA, PrefabScenario.unity DoorC
```

```
$ prefabwaitress overrides PrefabScenario.unity DoorBase
DoorBase [DoorBase.prefab] 19 overrides
  root: m_Name = DoorBase; m_LocalPosition = {x: 0, y: 0, z: 0}; ...
  DoorScenario.openAngle = 100 (prefab: 90)
  DoorScenario.label = door (redundant: same as the prefab)
  DoorScenario.spawn = Assets/Validation/PrefabWaitress/House.prefab (prefab: None)
  unused: DoorScenario.m_RenamedAway = 5 (MonoBehaviour has no m_RenamedAway)
```

## Editing

```
prefabwaitress set Level.unity Door DoorController.speed=2 Transform.m_LocalPosition=0,1,0 DoorController.target=@Door/Handle
prefabwaitress revert Level.unity Door/Panel MeshRenderer.m_Materials[0]
prefabwaitress apply Level.unity Door DoorController.speed --to Assets/Prefabs/DoorBase.prefab
prefabwaitress clean Level.unity --redundant
```

Edits go through the open Editor. The tool sends one request with every change.
The Editor checks every object, property and value first, then makes the changes
with `SerializedObject` and `PrefabUtility` and saves. The tool then reads the
file back and prints each value as it now stands. On a prefab instance, `set`
creates overrides exactly as editing in the Inspector would. `revert` and
`apply` take a property, a component, an object, an added component or object,
or a removed component (`--removed Type`). `clean` removes overrides Unity
ignores, and with `--redundant` also the ones equal to the prefab's value, which
would otherwise stop later prefab changes from reaching that instance.

A scene open in the Editor with unsaved changes is refused unless you pass
`--save-open`, which saves those changes too. A prefab open in Prefab Mode with
unsaved changes is refused. The Editor also checks that each object's name still
matches what the tool read, so a file that changed in the meantime fails cleanly
instead of editing the wrong object.

## Models

Scenes and prefabs often instance `.fbx` files. The importer creates their
objects, so no file holds them. Until they're exported, the tool sees only what
overrides say about them, marks those overrides `unchecked`, and `tree` shows the
model instance without its children. `prefabwaitress models` has the Editor
write each instanced model's hierarchy to
`UserSettings/Waitress/PrefabWaitress/Models/`. Those exports are used while the
model and its `.meta` are unchanged.

## Verifying Changes

-   `AgentScripts/PrefabWaitress/PrefabOracle.cs` dumps every object of a list of
    scenes and prefabs as the Editor sees them, through `SerializedObject`, and
    the hidden `prefabwaitress oraclecheck` command compares that dump with the
    tool's values. All 807 prefabs and the 23 scenes with prefab instances in
    this project, about 2.9 million values, match except where Unity changes data
    on load. Unity adds or renames fields older files lack, scripts upgrade
    old data (TextMeshPro alignment, URP light data, UI script references), a
    Canvas drives RectTransform layout, and lighting data supplies lightmap
    indices. Every hierarchy (children, components and their order) matches.
    The validation scenario, saved by this Unity version, matches in all 4175
    values, 172 of them overridden.
-   `prefabwaitress selftest` builds a small project of hand-written YAML and
    checks each resolution rule. The tests are mutation-checked.
-   `AgentScripts/PrefabWaitress/MakePrefabScenario.cs` rebuilds the validation
    scenario through `PrefabUtility`, so the files are exactly what the Editor
    writes.

[PrefabWaitress-DESIGN.md](PrefabWaitress-DESIGN.md) covers how instances are
resolved, what each report means, and what isn't supported.
