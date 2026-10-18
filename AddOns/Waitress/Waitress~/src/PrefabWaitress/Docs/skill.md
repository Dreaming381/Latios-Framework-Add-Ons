---
name: prefabwaitress
description: Read and edit Unity prefab override chains — prefab variants, nested prefabs, and the overrides scenes and prefabs put on prefab instances. Use whenever a task involves a prefab instance's values, what a prefab variant changes, why an object in a scene has some value, where a prefab is used, whether a change to a prefab will reach its instances, unused or redundant overrides, or setting, reverting or applying overrides, because scene and prefab YAML only stores the differences and can't be read correctly by hand.
---

# PrefabWaitress

A CLI over Unity scenes (`.unity`) and prefabs (`.prefab`). It reads them
offline and resolves every prefab instance through its variants and nested
prefabs, so each value is the one Unity uses and carries the file that set it.
Edits go through the open Editor, which writes the files; the tool reads them
back and prints the result.

Scene and prefab YAML can't be read directly for this: an instance stores only
its overrides, keyed by object ids in other files, and objects nested from other
prefabs aren't written at all. Use this tool instead of reading or editing that
YAML.

## First commands

```
prefabwaitress tree Assets/Scenes/Level.unity                 # hierarchy; instances marked and collapsed
prefabwaitress tree Assets/Scenes/Level.unity Door            # expand one instance
prefabwaitress show Assets/Scenes/Level.unity Door/Panel      # overridden values, each with the file that set it
prefabwaitress why Assets/Scenes/Level.unity Door/Panel MeshRenderer.m_Materials[0]
prefabwaitress instances Assets/Prefabs/Door.prefab --prop DoorController.speed
prefabwaitress help                                           # every command and option
```

## Answering the usual questions

| Question | Command |
|---|---|
| What's in this scene/prefab, which objects are prefab instances? | `tree <file>` (`--components` lists components, `--instances` lists every instance, nested ones too) |
| What does this instance (or variant) change? | `overrides <file> [object]` |
| What values does this object have, and where do they come from? | `show <file> <object> [Component]` (`--all` for unchanged values too) |
| Why does this property have this value? | `why <file> <object> Component.property` |
| Where is this prefab used, including through variants and nesting? | `instances <prefab>` |
| If I change this prefab's value, which instances won't follow? | `instances <prefab> [object] --prop Component.property` |
| Leftover overrides that do nothing, or that pin the prefab's value? | `overrides <file\|folder> --unused` / `--redundant` |
| Change a value on an instance (create an override) | `set <file> <object> Component.property=value ...` |
| Undo an override / remove an added component or object | `revert <file> <object> [Component[.property]]` |
| Bring back a removed component or child object | `revert <file> <object> --removed <Type or name>` |
| Push an override into the prefab (prints where it now reaches) | `apply <file> <object> Component.property [--to <prefab>]` |
| Remove unused (and redundant) overrides | `clean <file> [--redundant] [--dry-run]` |
| Objects from .fbx models show as unchecked or missing | `models` once (needs the Editor), then read again |

## Naming things

- `<object>`: a hierarchy path `Root/Child/Leaf`, or a name when only one object
  has it. `Name[1]` picks the second of several same-named siblings, as `tree`
  prints them.
- `Component`: the type (`MeshRenderer`) or the script class (`DoorController`);
  `Type[n]` when there are several; `GameObject` for the object itself.
- `property`: the serialized name (`m_LocalPosition`, `speed`, `hinge.limits`).
  `[i]` indexes a list and `.size` is its length: `m_Materials[0]`, `points.size`.

## Reading the output

- `Name [Door.prefab < DoorBase.prefab, 5 overrides]`: an instance of the
  variant Door.prefab, whose base is DoorBase.prefab.
- `value   <- Level.unity (prefab: old)`: the override in Level.unity set it;
  without that override it would be `old`.
- `root: ...`: the instance root's name and placement (Unity writes these on
  every instance).
- `+Type on X` / `+Name under X`: added here. `-Type on X`, `-X`: removed here.
- `unused`: Unity ignores it (the target or property no longer exists).
  `redundant`: equal to the prefab's value, so later prefab changes won't reach
  this instance. `unchecked`: the source can't be read yet (run `models`).
- `instances --prop` lists places that keep their own value with the file that
  sets it; `inherited by` lists places that get that value through it.

## Editing

- Needs the Editor open with the project (`unity open .`); reading never does.
- One `set` call takes several `Component.property=value` pairs. Values:
  numbers, `true`/`false`, enum names, strings, `0,1,0` for vectors and
  colors, `None`, an asset path (`Assets/Mats/Red.mat`), or `@Path/To/Object`
  (`@Path:Component`) for an object in the same file.
- `apply` changes the prefab, so it reaches every instance that doesn't
  override that value. Check with `instances --prop` first.
- A scene open in the Editor with unsaved changes is refused unless
  `--save-open`; a prefab open in Prefab Mode with unsaved changes is refused.
  Nothing changes when a request fails.

## Notes

- Files saved by older Unity versions may lack fields that Unity adds or
  renames when it loads them; the tool shows what the files hold.
- `help paths`, `help overrides`, `help edits`, `help models`, `help format`
  for details.
