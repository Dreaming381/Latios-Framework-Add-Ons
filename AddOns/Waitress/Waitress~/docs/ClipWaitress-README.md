# ClipWaitress

ClipWaitress is a command-line tool for Unity AnimationClips: `.anim` files and
the clips models import. It answers what a clip animates, what value a curve
has at a given time, whether its curves bind on the objects that play it, and
what plays it. It also edits `.anim` clips. It's built for AI agents, and it's
the seventh Waitress tool.

Questions about an animation that misbehaves are usually these:

-   Why doesn't the arm move?
-   Why does the loop pop?
-   Why does the character run in place, or slide?
-   Why does Unity log "AnimationEvent has no receiver"?
-   What rotation does this bone have at frame 20?
-   Which clips fire Footstep, and what plays them?

The answers are spread across places an agent can't easily read. Curves sit in
YAML as thousands of keys, and a clip imported from a model has no file of its
own. Whether a curve binds depends on the hierarchy of whatever plays the clip,
and that is found through a controller and then a prefab. Unity derives loop
match and average velocity, and doesn't store them anywhere readable.
ClipWaitress gathers all of it and answers most questions in one call.

## Building

From your Unity project's root:

```
dotnet build <addons package>/AddOns/Waitress/Waitress~/Waitress.sln -c Release --artifacts-path UserSettings/Waitress/build
```

The binary is
`UserSettings/Waitress/build/bin/ClipWaitress/release/clipwaitress[.exe]`.

It needs the Editor open with the Pipeline package (0.7 or newer) at these
times:

-   once per change to a clip file, to export it;
-   for `check`;
-   for edits.

Everything else works offline from the export. The tool carries its Editor
script inside itself, writes it to `UserSettings/Waitress/ClipWaitress/`, and
runs it through `run_script`, so nothing is imported into the project.

## Start Here

```
clipwaitress info Run                         # length, kind, loop match, root motion, changed settings, events, warnings
clipwaitress check Run                        # do its curves bind on the prefabs whose Animators play it
clipwaitress sample Run Hips:rotation --at 12f,50%
clipwaitress find event~Footstep
clipwaitress set Run.anim loopTime=true "events+=0.5:Footstep:L"
clipwaitress help                             # command index
clipwaitress skill                            # the whole tool on one page, for an agent
```

## What It Looks Like

`info` on a third-party humanoid clip:

```
$ clipwaitress info "Backward Left"
Assets/Hovl Studio/HSFiles/Animations/Backward Left.anim
  0.8333s, 25 frames at 30 fps; humanoid; 130 curves (95 muscles, body root, 8 IK goal curves), 3,144 keys, 67 KB
  loop      loopTime on, loopBlend (Loop Pose) on; Unity's loop match: pose 1 green, rotation 1 green, height (Y) 1 green, position (XZ) -18.7569 red (red XZ means the clip travels, which is right for a clip that moves the character)
  root      average velocity (-1.6605, 0.1848, -1.6821) m/s (2.3708 m/s), turning 0°/s; bake into pose: rotation on, Y off, XZ off
  changed   loopTime=true, loopBlend=true, loopBlendOrientation=true, keepOriginalOrientation=true, keepOriginalPositionXZ=true  (from Unity's defaults)
```

`check` follows the clip through its controller to the prefab that plays it,
and resolves every curve there:

```
$ clipwaitress check CW_Wave
Assets/Validation/ClipWaitress/CW_Wave.anim on Assets/Validation/ClipWaitress/CW_Ellen.prefab: CW_Ellen (Animator, avatar EllenAvatar (generic))
  property  Ellen_Body:SkinnedMeshRenderer.blendShape.Stretch: the object has no such property; its mesh Ellen_Body has no blend shapes
  problem   curve GlowOn drives an Animator parameter the controller CW_Controller doesn't have, so it drives nothing (add a Float parameter named GlowOn)
  event     Footstep at 0.333s: no script on CW_Ellen has this method (Unity logs "AnimationEvent has no receiver")
  event     Footstep at 1.125s: no script on CW_Ellen has this method
```

When a bone was renamed, `check` prints the fix:

```
  missing   Ellen_Skeleton/.../Ellen_Left_UpperArm and 22 paths under it (146 curves): no 'Ellen_Left_UpperArm' under Ellen_Skeleton/.../Ellen_Left_Shoulder; it has a child 'Ellen_L_UpperArm'. If that's the renamed object: set Assets/Validation/ClipWaitress/CW_Idle.anim "path[Ellen_Skeleton/.../Ellen_Left_UpperArm]=Ellen_Skeleton/.../Ellen_L_UpperArm"
```

## Commands

| Command | What it does |
|---|---|
| `info <clip>... [--all]` | Summary and warnings. |
| `curves <clip> [filter] [--keys] [--tangents]` | What each path animates, or each matching curve's start, end, range and keys. |
| `sample <clip> [filter] [--at t,...]` | Values at times, evaluated as Unity does, with rotations in degrees. |
| `find [folder...] cond...` | Clips matching settings and contents. |
| `diff <a> <b>` | Settings, events, curves only in one, and the largest differences in shared curves. |
| `users <clip>` | Controller states, blend trees, override controllers, Animation components and timelines, then the prefabs and scenes whose Animators play those. |
| `check <clip>... [--on target]` | Binding, avatar, Legacy, parameter and event-receiver problems on what plays the clip. |
| `set <clip>... key=value ... [--dry-run]` | Settings, events, path renames, curve removal, and keys. |
| `copy <clip> <Assets/.../New.anim>` | An editable copy of a clip, for clips imported from models. |
| `refresh [--force]` | Export now. This normally happens automatically. |

A `<clip>` can be named in several ways:

-   an `.anim` path;
-   `Model.fbx#ClipName`;
-   a unique clip name or file name.

Times can be given in several forms:

-   seconds: `0.5`;
-   frames: `12f`;
-   a percentage of the clip: `50%`;
-   `start` or `end`.

## Scope

-   Reads `.anim` files and the clips of models that import animation, in
    `Assets/`, local packages and the package cache.
-   Edits only `.anim` files. Imported clips are read-only here: change their
    import settings with ModelWaitress, or `copy` the clip.
-   `check` takes prefabs, models, and scenes that are open in the Editor.

See [ClipWaitress-DESIGN.md](ClipWaitress-DESIGN.md) for how it works, the
Unity behavior it was measured against, and what it doesn't cover.
