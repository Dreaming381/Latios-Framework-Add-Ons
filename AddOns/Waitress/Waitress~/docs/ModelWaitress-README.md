# ModelWaitress

ModelWaitress is a command-line tool that answers questions about how Unity
imports a model (`.fbx`, `.obj`, `.blend` and the other formats the
ModelImporter reads) and about what that import produced. It can also change
import settings. It's built for AI agents, and it's the sixth Waitress tool.

Troubleshooting a model usually starts with a few questions:

-   Why is it a hundred times too big?
-   Why is it lying on its back?
-   Why does the character T-pose?
-   Why did the bones disappear?
-   Why is the slot empty?
-   Why does `Mesh.vertices` throw in a build?

The answers are spread over several places. Some are in the importer's
settings, which sit in a `.meta` file as numbers, with some names that differ
from the API's. Others are in the import result, which only exists inside the
Editor's Library. Others depend on Unity's rules for combining the two, and those
aren't written down. ModelWaitress puts all of it in one place and answers
most questions in one call.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/ModelWaitress/release/modelwaitress[.exe]`.

It needs the Editor open with the Pipeline package (0.7 or newer) whenever a
model needs to be exported, which is once per change to the model. After that,
reading works without the Editor. The tool carries its Editor script inside
itself, writes it to `UserSettings/Waitress/ModelWaitress/`, and runs it through
`run_script`, so nothing is imported into the project.

## Start Here

```
modelwaitress info Hero.fbx                      # scale, rig, meshes, materials, clips, changed settings, warnings
modelwaitress find isReadable=true               # every model with Read/Write on
modelwaitress find 'maxverts>65535' indexFormat=UInt16
modelwaitress set Hero.fbx animationType=Human "clips[Run].loopTime=true"
modelwaitress help                               # command index
modelwaitress skill                              # the whole tool on one page, for an agent
```

## What It Looks Like

The model below is a third-party asset in the workspace whose materials come
out empty:

```
$ modelwaitress info Character.fbx
Assets/Hovl Studio/Black mage spells/Demo scene/Demo scene files/Character.fbx
  source    FBX 7400 binary, from Maya 2012, Y-up, +Z front, unit cm
  scale     0.01 = globalScale 0.01 × 1 (useFileScale off; file unit cm = 0.01 m); baked into vertices and positions, the root keeps scale 1
  size      1.9393 × 1.8219 × 0.3253 m (x × y × z), y from -0.0013 to 1.8206
  rig       Humanoid, avatar created from this model, CharacterAvatar valid, human, 51 bones mapped, skin weights up to 4 per vertex, 71 transforms
  meshes    3 (3 skinned, 35,340 verts, 55,320 tris), isReadable on (CPU copy kept)
  materials materialImportMode LegacyImport, 3 renderer slots: 0 from the model, 0 project materials, 3 EMPTY, 2 remaps (2 to MISSING assets; see `materials`)
  clips     mixamo.com 0.0333s @30fps (one clip per take, not customized)
  changed   14 settings: animationType=Human (Generic), avatarSetup=CreateFromThisModel (NoAvatar), globalScale=0.01 (1), ...
  messages  none

$ modelwaitress materials Character.fbx
import LegacyImport, location InPrefab, naming BasedOnTextureName, search RecursiveUp
materials in the file:
  Alpha_Body_MAT  -> MISSING asset (guid 2a714df06e718d146bda41b2b3695d58, no such asset in the project; the slot gets no material)
  Alpha_Joints_MAT  -> MISSING asset (guid bb058fbc18c6ae64e9427db3f8279f16, no such asset in the project; the slot gets no material)
```

Unity gives no warning about either remap.

An edit prints what changed in the result as well as the setting itself:

```
$ modelwaitress set MW_Ellen.fbx avatarSetup=CreateFromThisModel
Assets/Validation/ModelWaitress/MW_Ellen.fbx (reimported)
  avatarSetup: NoAvatar -> CreateFromThisModel
  result transforms: 109 -> 4
  result avatar: none -> MW_EllenAvatar valid
```

That model already had `optimizeGameObjects` on. Before the change, `info`
reported "optimizeGameObjects is on but does nothing: it needs an avatar".

## Commands

| Command | What it answers |
|---|---|
| `info <model>` | the one-screen summary above: the .fbx header, scale and size, rig, meshes, materials, clips, changed settings, warnings with known fixes |
| `settings <model> [filter] [--all]` | settings off Unity's default, grouped by Inspector tab; `--all` lists every one with enum options |
| `tree <model> [node] [--depth N] [--xforms] [--all]` | the imported hierarchy with its meshes, skins, materials and Animator; transforms that optimizeGameObjects folded away |
| `meshes <model> [mesh]` | mesh sizes; vertex layout, submeshes, bounds, blend shapes, bones per vertex, and which renderers use a mesh |
| `clips <model> [clip] [--paths] [--all]` | takes and clips, settings against the take's defaults, length, looping, curves, root motion, animated paths |
| `avatar <model> [--all]` | humanoid mapping, required bones left unmapped, mappings to transforms that don't exist |
| `materials <model>` | materials in the file, remaps (including ones to deleted assets), and each renderer slot |
| `find [folder...] cond...` | models matching conditions on any setting, any clip setting (`clip.loopTime`), the .fbx header (`app`, `up`), or on `verts`, `maxverts`, `tris`, `clips`, `avatar`, `warnings`, `empty`, `scale`, `changed`... |
| `diff <a> <b>` | settings, humanoid mapping, clip settings and results that differ between two models |
| `users <model>` | scenes, prefabs, controllers and masks that reference the model, and which mesh, clip or avatar each one uses |
| `postprocessors` | AssetPostprocessors that run on model imports, and default presets for ModelImporter |
| `set <model\|folder>... key=value ... [--dry-run]` | importer settings, clip settings and events, humanoid mapping, parameters and skeleton rebuild, material remaps |
| `refresh [model\|folder...] [--force]` | export now (otherwise automatic) |

`help find`, `help set` and `help export` explain conditions, edit keys and the
export.

## Validation

`Assets/Validation/ModelWaitress/` holds copies of four workspace models with
fresh `.meta` files, so edits there don't touch third-party assets. Git stores
identical files once, so the copies don't add to the repository. `modelwaitress
selftest` runs the offline checks. `Waitress~/docs/ModelWaitress-DESIGN.md`
records what was measured and how.
