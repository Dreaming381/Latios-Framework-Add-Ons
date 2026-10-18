---
name: modelwaitress
description: Answer questions about how Unity imports a model (.fbx, .obj, .blend...) and what the import produced, and change import settings, with the modelwaitress CLI. Use when troubleshooting a model's scale or rotation, rig or avatar, humanoid mapping, animation clips (loop, root motion, events), materials and remaps, mesh data (vertex layout, index format, blend shapes, Read/Write), optimizeGameObjects, import warnings, or when finding every model with a given setting.
---

# modelwaitress

Reads every ModelImporter setting next to Unity's default for it, and the imported result (hierarchy, meshes, skins,
avatar, clips, material slots, import warnings), from a per-model export the Editor writes. Stale exports refresh
themselves through the open Editor, so there is no separate step. Changes go through the Editor and are reimported.

`<model>` is a path, an asset path, or a unique file name (`Hero.fbx`, or `Hero`).

## Start here

    modelwaitress info <model>

One screen covering:
- what an .fbx file says about itself: the application that wrote it, its up axis and its unit;
- the effective scale, and why it is that;
- the overall size in metres;
- root-level transforms that are rotated or scaled;
- the rig and avatar validity, including required humanoid bones left unmapped and an optimizeGameObjects that does nothing;
- meshes, material slots (empty ones, remaps to deleted assets) and clips;
- every setting changed from Unity's default;
- import warnings.

Start every model question with it: it often answers outright, and otherwise points at the command that does.

## Read

| Command | Answers |
|---|---|
| `settings <model> [filter] [--all]` | settings off their default (`--all`: every one, with enum options) |
| `tree <model> [node] [--depth N] [--xforms] [--all]` | the imported hierarchy with meshes, skins, materials, Animator; `--all` lists transforms optimizeGameObjects folded away |
| `meshes <model> [mesh]` | sizes; one mesh's vertex layout, submeshes, bounds, blend shapes, bones per vertex, users |
| `clips <model> [clip] [--paths] [--all]` | takes, clips and their settings against the take's defaults, results (length, loop, curves, root motion); `--paths` lists animated paths |
| `avatar <model> [--all]` | humanoid mapping, unmapped required bones, mappings to transforms that don't exist |
| `materials <model>` | materials in the file, remaps, and what each renderer slot got |
| `find [folder...] cond... [--all] [--list]` | every model matching all conditions |
| `diff <a> <b>` | settings and results that differ between two models |
| `users <model> [--all]` | scenes, prefabs, controllers, masks... that reference the model, and which sub-asset (mesh, clip, avatar) each uses |
| `postprocessors` | AssetPostprocessors that run on model imports, and default presets |

How `find` works:
- Conditions take the forms `key=v`, `key!=v`, `'key>n'`, `'key<n'`, `'key>=n'` and `key~text`. Quote any condition with `>` or `<`, or the shell treats it as redirection.
- A key is any importer setting, or one of:
  - mesh data: `verts`, `maxverts` (largest mesh), `tris`, `meshes`, `skinned`, `bones`, `shapes`
  - animation: `clips`, `takes`
  - hierarchy and materials: `transforms`, `slots`, `empty`
  - import messages: `warnings`, `errors`
  - `avatar` (`none`, `valid` or `invalid`)
  - `unit`, `scale`, `importer`, `path`
  - `app` (the application that wrote an .fbx), `up` (its up axis)
  - `changed` (a count, or `changed~name`)
  - `clip.<setting>` (true when any clip matches, e.g. `clip.loopTime=false`)
- With no folder it searches `Assets/`. Add `--all` to include packages.
- Results group by folder past 25 matches.

Examples:

    modelwaitress find isReadable=true
    modelwaitress find Assets/Characters animationType=Human avatar=invalid
    modelwaitress find 'maxverts>65535' indexFormat=UInt16
    modelwaitress find 'empty>0'

## Change

    modelwaitress set <model|folder>... key=value [key+=v key-=v ...]

Every change is validated before any is made. Each model then reimports once, and the tool prints old -> new for each key, plus what changed in the result. That printout is the confirmation; nothing else needs checking.

- `isReadable=true`, `globalScale=0.01`, `useFileScale=false`, `animationType=Human` (Humanoid works too), `avatarSetup=CreateFromThisModel`, `sourceAvatar=Assets/A.fbx`, `materialImportMode=None`. Settings use their ModelImporter API names, enums take names, and assets take a path or `path#name`.
- `extraExposedTransformPaths+=Hips/Spine`: a unique tail of a path (or a bare name) is enough, for `=`, `+=` and `-=`. `-=` removes and `=` replaces.
- `clips[Run].loopTime=true`, `clips[Run].firstFrame=10`, `clips[Run].events+=0.5:Footstep[:text]` (time 0..1 of the clip), `clips[Run].events-=Footstep`, `clips[Run].mask=Assets/M.mask`.
- `clips+=Wave` adds a clip over the first take, `clips-=Wave` removes one, and `clips=default` goes back to one clip per take.
- `human[Head]=Alpha:Head` maps a humanoid bone, `human[RightEye]=` unmaps it, and `human.feetSpacing=0.1` sets a humanoid parameter.
- `human.skeleton=rebuild` takes a fresh skeleton pose from the file and keeps the mapping. It fixes "Avatar Rig Configuration mis-match" warnings after a scale change; put it in the same call as the change.
- `materials[Body_MAT]=Assets/Mats/Body.mat` remaps a material from the file; `=None` removes the remap.

Every mistake in a call is reported at once and nothing is changed; `--dry-run` only checks. Edits are refused in Play Mode, and while an Inspector holds unapplied changes to that model.

## Facts the summaries rely on (measured)

- Effective scale = globalScale × (useFileScale ? the file's unit factor : 1). It is baked into vertices and positions, and the root's scale stays 1.
- optimizeGameObjects needs an avatar, so with `avatarSetup NoAvatar` it does nothing. When it does work, skinned meshes keep no bone transforms, and `extraExposedTransformPaths` brings named ones back under the root.
- A remap to a deleted asset leaves the slot empty without any warning. `materials` shows it as MISSING, with the GUID.
- Defaults are Unity's own, from an importer reset to defaults (this matches a fresh import). They are not a preset's.

## Not covered

- What bakeAxisConversion will do to a given file: it varies by file (measured), so try it; `set` prints root rotation changes.
- Texture and other importers.
- Changes to AssetPostprocessor code or default presets aren't detected as staleness; run `refresh --force`.

Help: `modelwaitress help`, `help find`, `help set`, `help export`, `<command> --help`.
