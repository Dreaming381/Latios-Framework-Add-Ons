# ClipWaitress Design

## What It Answers

The tool is built around the questions that come up when an animation
misbehaves. Each one is answered in a single call.

| Question | Command | Where the answer comes from |
|---|---|---|
| Why doesn't part of the model move? | `check` | Each curve is resolved against the object that plays the clip, with `AnimationUtility.GetAnimatedObject` and `GetFloatValue`. |
| What was it renamed to? | `check` | The first path segment that doesn't resolve, plus objects elsewhere with the same name, or a child of the parent with a similar name. |
| Why does the loop pop? | `info` | Each curve group's value at the start and end of the clip's range, plus Unity's internal loop-match values. |
| Why does it run in place, or slide? | `info`, `check` | Average velocity and bake-into-pose settings, and whether Apply Root Motion is on for the Animator. |
| Why "AnimationEvent has no receiver"? | `check` | The methods of the scripts on the Animator's GameObject, and whether they take parameters an event can pass. |
| What value at time t? | `sample` | The curves evaluated offline, the way `AnimationCurve.Evaluate` does it. |
| What plays it? | `users` | Controller and override-controller YAML, then the prefabs and scenes whose Animators use those controllers. |
| Which clips match X? | `find` | Every clip's export. |

## Why an Editor Export

The CLI never parses `.anim` YAML, for four reasons:

-   A clip imported from a model has no YAML of its own.
-   A `.anim` stores its runtime curves next to separate editor curves.
-   Euler and quaternion rotation curves are stored differently.
-   What Unity derives from the curves (loop match, average velocity, curve
    stats) exists only in memory.

Instead, the Editor writes one JSON record per file to
`UserSettings/Waitress/ClipWaitress/Clips/<guid>.json`. A record holds:

-   each clip's `AnimationClipSettings`, with Unity's defaults: a new clip's
    settings, with `stopTime` set to the clip's length;
-   events;
-   every curve's keys, tangents and weights;
-   object-reference keys, with keys whose object no longer loads marked
    `missing`;
-   the internal `AnimationUtility.GetAnimationClipStats` and
    `MuscleClipUtility.GetMuscleClipQualityInfo`;
-   the rotation order of each Euler runtime curve.

How records stay current, and what happens without an Editor:

-   **Staleness.** A record is current while the file's and its `.meta`'s write
    times match its stamp. Any command re-exports stale records first, which
    takes about 1 s for one file and about 6 s for all 100 files in this
    project.
-   **No Editor open.** The last export is shown with a note.
-   **Pruning.** Records of files no longer in the project are deleted on every
    export and refresh.
-   **Which models are read.** Only models whose importer imports animation.

Reading is then offline, and `find` over 50 clips takes about 0.3 s.

`check` runs in the Editor at query time, because whether a curve binds depends
on an object's hierarchy and components. It finds its targets offline first:

1.  It scans controllers, override controllers and timelines that reference the
    clip's GUID (and its file ID, for model clips).
2.  It scans for prefabs and scenes whose Animators reference those controllers.
3.  In the Editor, it keeps only the Animators that actually play the clip,
    using `runtimeAnimatorController.animationClips`.

## Edits

Edits go through the `AnimationUtility` API in one `run_script` call:

-   `SetEditorCurves`, `SetObjectReferenceCurve`, `SetAnimationClipSettings`
    and `SetAnimationEvents`;
-   `AssetDatabase.RenameAsset` for `name=`.

How a call runs:

1.  Every op in the call is validated first, and every mistake is reported at
    once.
2.  Nothing changes if any op fails, including across several clips.
3.  Each clip gets `Undo.RegisterCompleteObjectUndo`, then `SaveAssetIfDirty`.
4.  The CLI re-exports and prints a delta between the two records.

`--dry-run` applies the planned ops to an `Object.Instantiate` copy, exports
the copy the same way, and prints the same delta. So a preview and a real run
can't disagree.

Choices made along the way:

-   **Paths are resolved by the CLI.** A unique tail of a path the clip
    animates is accepted. When `path[Old]` was given as a tail, the new name
    keeps Old's parent. An agent fixing a renamed bone writes
    `path[Arm]=LeftArm`, and without this rule round 2 found the curves moving
    to the root.
-   **Euler input for quaternion curves.** Quaternion curves take Euler
    degrees, including the humanoid RootQ and IK goal Q curves. Each new key
    takes the sign that faces its neighbour. Round 2 found that without this,
    an Euler key added between quaternion keys landed on the other hemisphere
    and the bone spun the long way.
-   **Quaternions are removed whole.** `curves-=` refuses to remove part of a
    quaternion.
-   **No mixed rotation representations.** A path rename refuses to put
    quaternion rotation onto a path that has Euler rotation, or the reverse.
-   **Tangents.** New keys get Clamped Auto tangents. `active` and `enabled`
    curves get Constant tangents, because those values are on/off.
-   **Replacing keys.** `keys[...]+=` replaces only a key at the same time,
    within 1e-4 s, so adding a key between frames no longer deletes a
    neighbour. `-=` removes keys within half a frame.
-   **Copies are rebuilt.** `copy` re-sets the copy's curves after
    `CreateAsset`. Without that, a fresh copy reported no stats, no loop match
    and zero velocity until its first edit (round 2).

Imported clips are refused with the `modelwaitress set ... clips[Name]...`
command to use instead, or `copy`. Registry packages are refused because
they're read-only.

## Measured Rules

-   **Curve values.** The values are the editor curves evaluated the way
    `AnimationCurve.Evaluate` does it:
    -   Hermite between keys;
    -   a 2D Bezier when either side of the segment is weighted, with an
        unweighted side using 1/3;
    -   the left value when either tangent is infinite (stepped);
    -   clamped outside the first and last keys.
-   **Rotation order.** Euler curves play in the rotation order stored on the
    clip's runtime Euler curve (`m_EulerCurves[].curve.m_RotationOrder`). The
    editor curves don't carry it.
    -   A third-party `.anim` (DragonFall) uses XYZ. Read with Unity's ZXY,
        its pose came out up to 4° off `SampleAnimation`.
    -   `info` notes the order, and `sample` converts to the Transform's
        rotation.
-   **Loop match.** The values come from
    `MuscleClipUtility.GetMuscleClipQualityInfo(clip, startTime, stopTime)`.
    -   The Inspector shows the lamp red under 0.33, orange under 0.66, and
        green otherwise.
    -   It draws the lamps only for imported clips, but the values exist for
        any clip with root curves.
    -   XZ goes negative on a clip that travels. That is expected, and `info`
        says so.
-   **Cycle Offset.** The Inspector disables it while `loopTime` is off.
-   **Mirror.** Only humanoid clips can be mirrored.
-   **Event receivers.** Unity calls the method on any script on the Animator's
    GameObject. A receiver may take no parameter, or one float, int, string,
    Object or AnimationEvent.

## Verification

-   **Evaluation against Unity.** `AgentScripts/ClipWaitress/EvalOracle.cs`
    writes Unity's own answers, and the hidden `clipwaitress evalcheck <file>`
    compares them:
    -   `AnimationCurve.Evaluate` at every key, midpoint and a random time per
        segment, on every curve of every clip in `Assets/`: 321,258 of 321,258
        match;
    -   `AnimationClip.SampleAnimation` on a built hierarchy, for every
        generic transform clip at five times: 5,250 of 5,250 match.

    This included clips the tool had edited. Before the rotation-order fix, 130
    poses in DragonFall failed; that failure is how the rule was found.
-   **Self-tests.** `clipwaitress selftest` runs 10 checks: Hermite, stepped,
    weighted against Hermite, clamping, Euler from a quaternion, rotation
    orders, names, filters, tail resolution, and list membership.
-   **Scenario.** `Assets/Validation/ClipWaitress/` is built by
    `AgentScripts/ClipWaitress/MakeScenario.cs`. `CW_Ellen.prefab` has a
    renamed left upper arm, and `CW_Controller` lacks the `GlowOn` parameter.
    `check` found both, plus the missing blend shape, and the printed
    `path[...]` fix made every curve bind.

## Subagent Rounds

### Round 1: productivity

Seven tasks: fix a non-binding arm, list everything wrong with a clip on a
prefab, read values at a frame, find popping loops, read a rotation in a clip
with a different Euler order, edit events, and find fast root-motion clips. All
were answered correctly, mostly in 2 to 5 calls.

Fixed:

-   **`gaps` disagreed with `info`.** Gaps now ignore clips with Loop Pose on,
    and differences under 0.001, 0.5° or 2% of a curve's range.
-   **Path filters had to be complete.** `path:property` now takes a unique
    path tail, and a bare object name selects only that object.
-   **A filter that matched nothing said nothing useful.** It now says what the
    path does animate.
-   **Missing properties had no alternatives.** `check` now lists the mesh's
    blend shapes or the closest animatable properties.
-   **`--dry-run` didn't show changes.** It now prints the changes it would
    make.
-   **No per-axis speed.** `velocity.x`, `velocity.y` and `velocity.z` are now
    `find` keys.
-   **Blend trees had no context.** `users` now names the state that holds a
    blend tree.
-   **Long list values in `find`.** Contains-conditions on lists show only the
    matching entries.
-   **Cycle Offset.** A Cycle Offset with `loopTime` off now gets a warning.
-   **Small fixes.** Fractional frames, and grammar.

### Round 2: exploratory

Fixed:

-   **Quaternion hemisphere flips.** These came from new Euler keys. There is
    now also a warning for existing flips.
-   **Mixed rotation representations.** A path move could put quaternion
    rotation onto an Euler path.
-   **Partial quaternion removal.**
-   **Copies had empty stats.**
-   **`check` exited 0 when it checked nothing.** It now exits 2.
-   **`copy` destinations weren't validated.** A trailing slash or a `:`
    leaked a stack trace and left stray folders, and other extensions were
    accepted.
-   **Rename onto an existing file.** It was reported as a success.
-   **`start` and `end` in `set`.**
-   **`find event=X`.** `=` now tests list membership.
-   **Several missing components on one object.** `check` named only one;
    it now names them all.
-   **Legacy clips.** They got loop warnings, and a legacy humanoid clip got
    none.
-   **`--tangents` alone printed no keys.**
-   **Event input:**
    -   only the first bad event was reported;
    -   quoted text parameters weren't possible;
    -   float parameters displayed like ints;
    -   Unicode method names were rejected.
-   **Animator `m_Enabled`.** It was read as a parameter curve.
-   **Unknown component types gave a generic message.**
-   **Tail renames.** A tail rename moved curves to the root.
-   **Adding a key could delete its neighbour.** `+=` between frames deleted
    the key next to it.
-   **On/off curves.** They got smooth tangents.
-   **New paths were created silently.** The output now marks them.
-   **Out-of-range sample times.** `sample` now says when a time is outside
    the clip, and where a looping clip would be.
-   **Overviews of big rigs.** `curves` now cuts them off at 60 paths.
-   **Humanoid Q curves showed as quaternions.** They are now in degrees.
-   **Mirrored humanoid clips.** `sample` now notes that left and right swap.
-   **Event output.** Event deltas list only what was added or removed, and
    the no-receiver note is printed once.
-   **`diff` on quaternions.** It now compares quaternions by angle.

### Round 3: verification

Six new tasks crossed the round-2 fixes:

-   copy an imported clip and make it bind on a prefab with a renamed bone;
-   tilt the head 15° at 50% of the clip;
-   edit events and find what would receive them;
-   turn an in-place walk into a travelling one;
-   find clips by event and by parameter;
-   diff the edited copy against the original.

All six were answered. For the walk, the right answer was that it walks in
place, so no setting can make it travel.

Fixed:

-   **`diff` showed a path rename as 146 + 146 unrelated curves.** It is now
    one "path renamed" line, and the renamed curves' values are compared.
-   **`diff` reported times between frames.** It now samples whole frames.
-   **`check --on` was silent when the object's Animator doesn't play the
    clip.** It now says so, and names the controller.
-   **Event parameters had no visible type.** They are now typed: `0.5f` is a
    float, a number without `f` is an int, and quoted values are text.
-   **`info` couldn't tell an in-place clip from one whose motion is baked
    away.** When XZ or Y is baked, it now prints where RootT goes before
    baking.
-   **No relative keys.** `keys[...]+=t:+(dx,dy,dz)` now offsets the current
    value.
-   **One-frame pops went unnoticed.** `set` now warns when a new or changed key
    sits a frame from a very different key.

## Known Unsupported

-   **Editing imported clips.** This is ModelWaitress's job, or `copy` the
    clip.
-   **Editing object-reference (sprite) keys and tangent modes.**
-   **`check` against scenes that aren't open.** Opening a scene would disturb
    the user's Editor.
-   **AnimatorControllers themselves.** States, transitions and layers are out
    of scope beyond which state uses a clip. Controller layout needs human
    review, so it is deferred.
-   **Timeline clips' own settings.** Clip in, ease and the like.
-   **Optimized hierarchies.** Under `optimizeGameObjects`, transform curves
    bind through the Avatar and are not checked.
-   **`users` takes one clip.** There is no `unused` query.
-   **Changing `frameRate` doesn't move keys onto the new frames.**
-   **Events before `startTime` or after `stopTime` aren't flagged.** Whether
    they fire wasn't measured.
-   **`additiveReferencePoseTime`.** Percent values are taken from the edited
    clip, not the reference clip.
-   **Player builds.** A build's compressed clip can differ slightly from the
    editor curves this reads.
