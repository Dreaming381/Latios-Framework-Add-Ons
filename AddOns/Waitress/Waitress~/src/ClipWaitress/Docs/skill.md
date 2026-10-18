---
name: clipwaitress
description: Answer questions about Unity AnimationClips (.anim files and clips imported from models) and change .anim clips, with the clipwaitress CLI. Use when an animation doesn't play or plays wrong — curves that don't bind (renamed or moved objects, missing components, blend shapes or properties), humanoid clip on a generic avatar, Legacy mismatch, Animator parameter curves, events with no receiver — or for loop pops, root motion, a curve's value at a time, what a clip animates, which controllers and prefabs play a clip, finding clips by setting or content, or editing clip settings, events, keys and paths.
---

# clipwaitress

Reads every clip's settings next to Unity's defaults, its events, and every curve's keys from a per-file export the
Editor writes, and evaluates curves exactly as Unity does. Stale exports refresh themselves through the open Editor,
so there is no separate step. `check` and edits go through the Editor.

`<clip>` is an .anim path, `Model.fbx#ClipName`, or a unique clip or file name (`Run`). Times are seconds (`0.5`),
frames (`12f`), percent of the clip (`50%`), `start` or `end`.

## Start here

    clipwaitress info <clip>        what the clip is and what's wrong with it on its own
    clipwaitress check <clip>       does it bind on the objects that play it

`info` gives length and frame rate; humanoid, generic or legacy; what the curves animate; loopTime and Loop Pose with
Unity's own loop-match values (green/orange/red, as the Inspector shows for imported clips); average velocity and
bake-into-pose for root motion; settings changed from default; events; and warnings: curve groups that pop when the clip
loops, events past the end, NaN keys, euler curves that jump over 180°, object keys pointing at deleted assets, settings
that do nothing.

`check` finds what plays the clip (controller → Animator in a prefab, or open scene; Animation components) and resolves
every curve there: paths that don't exist (with the `path[...]` rename that fixes them, found by name), components or
properties (blend shapes, material properties, fields) that don't exist, a humanoid clip on a generic or missing avatar,
Legacy on the wrong component, curves that drive Animator parameters the controller lacks, and events no script on
the object receives. `--on <prefab|model|scene>` checks against a specific object instead (a scene must be open in
the Editor). Exit code 1 means something doesn't bind; 2 means nothing could be checked.

## Read

| Command | Answers |
|---|---|
| `curves <clip> [filter] [--keys] [--tangents] [--all]` | what each path animates; with a filter, each curve's start, end, range; `--keys` every key |
| `sample <clip> [filter] [--at 0.5,12f,50%]` | values at times (default start, middle, end), rotations in degrees |
| `find [folder...] cond... [--all] [--list]` | every clip matching all conditions |
| `diff <a> <b>` | settings, events, curves only in one, and the shared curves that differ most and when |
| `users <clip> [--all]` | controllers (state, blend tree), override controllers, Animation components, timelines, and the prefabs and scenes whose Animators play it |

A filter is `path:property` (the path in full or a unique tail of it: `Hips/Spine:rotation`, `Spine:rotation`, `Body:SkinnedMeshRenderer.blendShape.Smile`,
`:GlowIntensity` for the root) an object name (`Ellen_Left_Arm`: that object only), or text found in the path or curve name (`Spine`). Property names: `position`,
`rotation`, `scale` (or `.x`), `active`, `Type.property`, or a muscle or Animator parameter name on the root.

`find` conditions: `key=v`, `key!=v`, `'key>n'`, `'key<n'`, `key~text` (quote `>`/`<`). Keys: any clip setting
(`loopTime`, `loopBlend`, `mirror`, `legacy`, `wrapMode`...), or `name`, `path`, `kind` (anim/model), `length`, `frames`,
`fps`, `curves`, `keys`, `paths`, `animates` (path:group list), `events`, `event` (function names), `human`, `loop`,
`speed` (any direction), `velocity.x`/`.y`/`.z` (+z forward), `turning`, `size`, `changed`, `missing`, `warnings`,
`gaps` (groups that pop when looping with Loop Pose off), `muscles`, `params`.

    clipwaitress find animates~Hips/Spine
    clipwaitress find event~Footstep
    clipwaitress find loopTime=true 'gaps>0'

## Change (.anim only)

    clipwaitress set <clip|folder>... key=value ... [--dry-run]

Every change is validated before any is made; every mistake is reported at once. `--dry-run` prints the changes it
would make. Each clip is saved once and the tool prints what changed (keys added, removed or changed, in degrees for
rotation). That printout is the confirmation. Changes apply in the order given. Paths may be a unique tail.
Unity's Undo can revert it.

- Settings: `loopTime=true`, `loopBlend=true` (Loop Pose), `cycleOffset=0.5`, `mirror`, `startTime`, `stopTime`,
  `loopBlendOrientation`/`loopBlendPositionY`/`loopBlendPositionXZ` (bake into pose), `keepOriginal...`,
  `orientationOffsetY`, `level`, `heightFromFeet`, `frameRate=30`, `wrapMode=Loop`, `legacy=true`, `name=New` (renames the file).
- Events: `events+=0.5:Footstep:L` (time, function, optional int/float/text/asset param, `"5"` quoted for text;
  several with `;`), `events-=Footstep`, `events-=Footstep@0.5`, `events=` clears.
- Paths: `path[Old/Path]=New/Path` re-points every curve on that path and below — the fix `check` prints for a renamed
  or moved object. `path[Arm]=LeftArm` (a unique tail) renames just that object and keeps its parent.
- Curves: `curves-=Path` (all on that path), `curves-=Path:position`, `curves-=Path:position.y`.
- Keys: `keys[Path:property]=t:v;t:v` replaces (creates the curve if missing), `+=` adds keys (replacing one at the
  same time), `-=t;t` removes keys. position/rotation/scale take `x,y,z` (`keys[Hips:position]+=0.5:0,1.1,0`);
  rotation is in degrees, for quaternion curves and humanoid RootQ/IK goals too. `t:+(dx,dy,dz)` offsets the current
  value (`keys[Head:rotation]+=50%:+(-15,0,0)`, added to the Transform's euler angles). A key one frame from a very
  different key warns that it pops.

Clips imported from models are read-only: change their import settings with `modelwaitress set Model.fbx clips[Name]...`,
or `clipwaitress copy <clip> Assets/.../New.anim` to get an editable copy.

## Facts the output relies on (measured)

- Curve values match `AnimationCurve.Evaluate` (250k samples, every curve in the test project), and poses match
  `AnimationClip.SampleAnimation` (3k transform samples).
- Euler curves play in the rotation order stored on the clip's runtime curve, which can differ from Unity's ZXY
  (an imported .anim used XYZ). `info` notes it; `sample` shows the resulting Transform rotation.
- Loop-match values are Unity's internal ones; under 0.33 is red, under 0.66 orange. A red XZ on a clip that travels is right.

## Not covered

- Editing clips imported from models (use modelwaitress or `copy`), object-reference (sprite) keys, or tangent modes.
- Scenes that aren't open in the Editor, for `check`.
- AnimatorControllers themselves (states, transitions) beyond which states use a clip; Timeline clips' own settings.
- A player build's compressed clip can differ slightly from the Editor curves this reads.

Help: `clipwaitress help`, `help find`, `help set`, `help export`, `<command> --help`.
