using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ClipWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("clipwaitress — AnimationClips in a Unity project: what they animate, their values, settings and events, what plays");
            w.WriteLine("them, and why a curve doesn't bind");
            w.WriteLine();
            w.WriteLine("Answers come from a per-file export the Editor writes (UserSettings/Waitress/ClipWaitress/). A file that changed");
            w.WriteLine("is re-exported automatically when an Editor has the project open; reading needs no other step.");
            w.WriteLine("<clip> is an .anim path, Model.fbx#ClipName, or a unique clip or file name. Commands that take several also take");
            w.WriteLine("a folder or a model (every clip in it).");
            w.WriteLine("Times are seconds (0.5), frames (12f), percent of the clip (50%), start or end.");
            w.WriteLine();
            w.WriteLine("read");
            w.WriteLine("  info <clip>... [--all]       length, kind, what it animates, loop settings and Unity's loop match, root motion,");
            w.WriteLine("                               settings changed from default, events, warnings (loop pops, events past the end,");
            w.WriteLine("                               NaN keys, euler flips, missing object keys)");
            w.WriteLine("  curves <clip> [filter] [--keys] [--tangents] [--all]");
            w.WriteLine("                               what each path animates; with a filter (path:property, a unique path tail, an");
            w.WriteLine("                               object name, or text), each curve's start, end and range;");
            w.WriteLine("                               --keys: every key (time:value); --tangents: slopes and weights too");
            w.WriteLine("  sample <clip> [filter] [--at t,t,...] [--all]");
            w.WriteLine("                               values at times (default start, middle, end), as Unity evaluates the curves;");
            w.WriteLine("                               rotations in degrees");
            w.WriteLine("  find [folder...] [cond...] [--all] [--list]");
            w.WriteLine("                               clips matching every cond: key=v key!=v 'key>n' 'key<n' key~text (`help find`)");
            w.WriteLine("  diff <clipA> <clipB>         settings, events, curves only in one, and the shared curves that differ most");
            w.WriteLine("  users <clip> [--all]         controllers (states, blend trees), override controllers, Animation components and");
            w.WriteLine("                               timelines that reference it, and the prefabs and scenes whose Animators play those");
            w.WriteLine();
            w.WriteLine("check and change (need the Editor)");
            w.WriteLine("  check <clip|folder>... [--on <prefab|model|scene>]... [--all]");
            w.WriteLine("                               do the curves bind on what plays the clip: missing paths (with the rename that");
            w.WriteLine("                               fixes them), missing components and properties, humanoid clip without a humanoid");
            w.WriteLine("                               avatar, Legacy mismatch, parameters the controller lacks, events with no receiver.");
            w.WriteLine("                               Without --on: every prefab and open scene whose Animator plays it (see users).");
            w.WriteLine("                               Exit 1: something doesn't bind; 2: nothing could be checked");
            w.WriteLine("  set <clip|folder>... key=value ... [--dry-run]");
            w.WriteLine("                               settings, events, path[Old]=New, curves-=..., keys[path:property]=... (`help set`)");
            w.WriteLine("  copy <clip> <Assets/.../New.anim>   an editable .anim copy (for clips imported from models)");
            w.WriteLine("  refresh [clip|folder...] [--force]   export now (normally automatic)");
            w.WriteLine();
            w.WriteLine("  --project <dir> picks the Unity project");
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("find", @"Finding clips

  find [folder...] cond... [--all] [--list]   prints each match with the values its conditions name (values an =
  condition pins aren't repeated). Without a folder it searches Assets/ (--all adds packages). Without conditions it
  lists the clips with a one-line summary. More than 25 matches are grouped by folder unless --list.
  Quote conditions with > or < so the shell doesn't treat them as redirection: find 'length>2'.
  cond       key=value  key!=value  key>n  key<n  key>=n  key<=n  key~text (contains). Text compares ignore case.
             On list keys (event, animates, params, changed), = means one item equals, and ~ shows the matching items.
  keys       any clip setting (loopTime, loopBlend, mirror, cycleOffset, legacy, wrapMode...), or:
             name, path, kind (anim|model), length (s), frames, fps, curves, keys, paths, animates (path:group list),
             events (count), event (function names), human, loop (= loopTime), speed (m/s, any direction), velocity.x,
             velocity.y, velocity.z (average root velocity; +z is forward), turning (°/s), size (bytes), changed (settings
             off their default), missing (object keys to deleted assets), warnings, gaps (curve groups that pop when the
             clip loops: loopTime on, Loop Pose off, a difference of at least 0.001, 0.5° or 2% of the curve's range),
             muscles, params (Animator parameters the clip drives)
  examples   find loopTime=true 'gaps>0'          find animates~Hips/Spine        find event~Footstep
             find human=true loopTime=false      find 'warnings>0' Assets/Characters"),
            ("set", @"Changing clips (.anim files; imported clips are read-only: change them with modelwaitress, or `copy`)

  set <clip|folder>... key=value ... [--dry-run]   every change is checked before any is made, and every mistake is
  reported at once; --dry-run prints what would change and changes nothing. Each clip is saved once and the tool
  prints what changed (settings old -> new, events, curves added, removed or moved, keys added, removed or changed,
  new warnings). Changes apply in the order given (events= then events+=... replaces the list). Unity's Undo can
  revert it. A path may be a unique tail of one the clip animates (Ellen_Left_Arm for Root/.../Ellen_Left_Arm).
  key=value            a clip setting: loopTime, loopBlend (or loopPose), cycleOffset, mirror, startTime, stopTime,
                       loopBlendOrientation, loopBlendPositionY, loopBlendPositionXZ, keepOriginalOrientation,
                       keepOriginalPositionY, keepOriginalPositionXZ, orientationOffsetY, level, heightFromFeet,
                       hasAdditiveReferencePose, additiveReferencePoseClip, additiveReferencePoseTime,
                       frameRate, wrapMode, legacy, name (renames the .anim file)
  events+=0.5:Footstep[:param]   add an event (time as seconds, 12f, 50%, start or end; param: int, float, text, or an
                       asset path; ""5"" in double quotes is text); several: events+=0.2:StepL;0.6:StepR
  events-=Footstep     remove every Footstep event; events-=Footstep@0.5 the ones at that time; events= clears all
  path[Old/Path]=New/Path   re-point every curve on Old/Path and below to New/Path (the fix for a renamed or moved
                       object; `check` prints the one to use). Old may be a unique tail (path[Arm]=LeftArm renames
                       just that object, keeping its parent). path[]=Wrapper moves everything under a new parent
  curves-=Path         remove every curve on Path (exactly that path); curves-=Path:position one group;
                       curves-=Path:position.y one curve (quaternion rotations only go as a whole)
  keys[Path:property]=t:v;t:v   replace a curve's keys (creates the curve if missing); += adds keys, replacing one at
                       the same time; -=t;t removes the keys within half a frame of those times. position, rotation
                       (degrees, also for quaternion curves and RootQ/IK goal Q) and scale take x,y,z:
                       keys[Hips:position]+=0.5:0,1.1,0. New keys get Clamped Auto tangents (active and enabled curves
                       step); a new quaternion key takes the sign that faces its neighbour, so it turns the short way.
                       t:+(dx,dy,dz) offsets the value the curve has at t (for rotation, the Transform's euler
                       angles): keys[Head:rotation]+=50%:+(-15,0,0). A key a frame from a very different one warns.
  property names       position, rotation, scale (or .x/.y/.z), active, Type.property
                       (SkinnedMeshRenderer.blendShape.Smile, MeshRenderer.material._Color.r, Light.m_Intensity,
                       MyScript.speed), or on the root (:Name) an Animator parameter or muscle"),
            ("export", @"Where answers come from

  The Editor writes one JSON file per .anim or model to UserSettings/Waitress/ClipWaitress/Clips/<guid>.json: each
  clip's settings with Unity's defaults, events, every curve's keys and tangents, object-reference keys, and what
  Unity derives from them (average velocity, loop match, curve stats). An export is current while the file's and its
  .meta's write times match; any command re-exports stale files through the Editor first. With no Editor open, the
  last export is shown with a note. Records of files no longer in the project are deleted on every export and refresh.
  Values are the Editor's curves evaluated the way AnimationCurve.Evaluate does (Hermite, weighted Bezier, stepped);
  a player build's compressed clip can differ slightly. Models are read only when they import animation."),
        };

        public static void Topic(TextWriter w, string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                Index(w);
                return;
            }
            var hit = s_Topics.FirstOrDefault(t => string.Equals(t.name, topic, StringComparison.OrdinalIgnoreCase));
            if (hit.name != null)
            {
                w.WriteLine(hit.text);
                return;
            }
            if (CommandRouter.Commands.Contains(topic, StringComparer.OrdinalIgnoreCase))
            {
                var index = new StringWriter();
                Index(index);
                var mine = false;
                foreach (var line in index.ToString().Split('\n'))
                {
                    var t = line.TrimEnd('\r');
                    if (t.StartsWith("  ", StringComparison.Ordinal) && !t.StartsWith("   ", StringComparison.Ordinal))
                        mine = t.Substring(2).StartsWith(topic + " ", StringComparison.Ordinal) || t.Substring(2) == topic;
                    else if (!t.StartsWith("   ", StringComparison.Ordinal))
                        mine = false;
                    if (mine)
                        w.WriteLine(t);
                }
                if (topic is "find" or "set")
                    w.WriteLine($"see `help {topic}`");
                return;
            }
            throw new WaitressException($"no help topic '{topic}'; topics: {string.Join(", ", s_Topics.Select(t => t.name))}, or a command name");
        }

        public static int Skill(string install, TextWriter w)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClipWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream);
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
