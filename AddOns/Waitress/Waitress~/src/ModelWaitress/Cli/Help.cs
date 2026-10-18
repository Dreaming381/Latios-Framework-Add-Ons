using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ModelWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("modelwaitress — model import settings and what they produced, for .fbx/.obj/.blend/... files in a Unity project");
            w.WriteLine();
            w.WriteLine("Answers come from a per-model export the Editor writes (UserSettings/Waitress/ModelWaitress/). A model whose file");
            w.WriteLine("or .meta changed is re-exported automatically when an Editor has the project open; reading needs no other step.");
            w.WriteLine("<model> is a path, an asset path, or a unique file name (Hero.fbx).");
            w.WriteLine();
            w.WriteLine("read");
            w.WriteLine("  info <model>                 one screen: the .fbx header, effective scale and size, rig and avatar, meshes,");
            w.WriteLine("                               materials, clips, every setting changed from default, import warnings with fixes");
            w.WriteLine("  settings <model> [filter] [--all]");
            w.WriteLine("                               settings changed from default; --all: every setting, with enum options");
            w.WriteLine("  tree <model> [node] [--depth N] [--xforms] [--all]");
            w.WriteLine("                               the imported hierarchy with meshes, skins, materials; --all: also the");
            w.WriteLine("                               transforms optimizeGameObjects folded away (below node, if given)");
            w.WriteLine("  meshes <model> [mesh]        mesh sizes; one mesh's vertex layout, submeshes, bounds, blend shapes");
            w.WriteLine("  clips <model> [clip] [--paths] [--all]");
            w.WriteLine("                               takes and clips, changed settings, results, root motion; --all: every clip setting;");
            w.WriteLine("                               --paths: animated paths");
            w.WriteLine("  avatar <model> [--all]       rig, avatar validity, humanoid mapping and gaps; --all: fingers too");
            w.WriteLine("  materials <model>            materials in the file, remaps, and what every renderer slot got");
            w.WriteLine("  find [folder...] [cond...] [--all] [--list]");
            w.WriteLine("                               models matching every cond: key=v key!=v 'key>n' 'key<n' key~text; keys are");
            w.WriteLine("                               settings, clip.<setting>, verts, tris, avatar, warnings... (`help find`);");
            w.WriteLine("                               --all: packages too; --list: no grouping by folder");
            w.WriteLine("  diff <modelA> <modelB>       settings and results that differ");
            w.WriteLine("  users <model> [--all]        scenes, prefabs, controllers, masks... that reference the model, and which of its");
            w.WriteLine("                               meshes, clips, avatar or materials each one uses; --all: Packages/ too");
            w.WriteLine("  postprocessors [--force]     AssetPostprocessors that run on model imports, and default presets");
            w.WriteLine();
            w.WriteLine("change (needs the Editor; reimports, then prints old -> new and what changed in the result)");
            w.WriteLine("  set <model|folder>... key=value [key+=v key-=v ...] [--dry-run]");
            w.WriteLine("                               importer settings, clips[Name].x, human[Bone], human.x, materials[Name] (`help set`)");
            w.WriteLine("  refresh [model|folder...] [--force]   export now (normally automatic)");
            w.WriteLine();
            w.WriteLine("  --project <dir> picks the Unity project");
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("find", @"Finding models

  find [folder...] cond... [--all] [--list]   prints each match with the values its conditions name (values an =
  condition pins aren't repeated). Without a folder it searches Assets/ (--all adds packages). Without conditions it
  lists the models with a one-line summary. More than 25 matches are grouped by folder unless --list.
  Quote conditions with > or < so the shell doesn't treat them as redirection: find 'verts>65535'.
  cond       key=value  key!=value  key>n  key<n  key>=n  key<=n  key~text (contains). Text compares ignore case.
  keys       any importer setting (isReadable, animationType, globalScale...; `settings <model> --all` lists them), or:
             verts (all meshes), maxverts (largest mesh), tris, meshes, skinned, bones (most bindposes on a mesh), shapes, clips, takes, transforms,
             slots (renderer material slots), empty (empty slots), warnings, errors, avatar (none|valid|invalid),
             unit (the file's unit), scale (effective import scale), changed (settings off their default: a count, or
             clip.<setting> (true when any clip matches: clip.loopTime=true),              changed~name), importer, path
  examples   find isReadable=true                find Assets/Characters animationType=Human avatar=invalid
             find 'maxverts>65535' indexFormat=UInt16  find changed~globalScale  find 'warnings>0'
             find clip.loopTime=false animationType=Human"),
            ("set", @"Changing import settings

  set <model|folder>... key=value ... [--dry-run]   every change is checked before any is made, and every mistake is
  reported at once; --dry-run stops there. Then each model is reimported once, and the tool prints each key's old -> new value and what changed in the result (transforms, meshes, clips,
  avatar validity, material slots, new import warnings).
  key=value            an importer setting by its ModelImporter API name: isReadable=true, globalScale=0.01,
                       animationType=Human, materialImportMode=None, meshCompression=High, sourceAvatar=Assets/A.fbx
                       Enums take a name (or number; flags take A|B). Asset settings take a path, or path#name.
  list+=a,b / list-=a  extraExposedTransformPaths and extraUserProperties (`=` replaces the list)
  clips[Name].x=v      a clip setting: loopTime, loopPose, firstFrame, lastFrame, lockRootRotation, keepOriginalPositionY,
                       heightFromFeet, mirror, cycleOffset, name, takeName... (`clips <model> <clip> --all` lists them)
  clips[Name].events+=0.5:Footstep[:text]  adds an event at half the clip (time is 0..1); events-=Footstep removes; events= clears
  clips[Name].mask=Assets/M.mask (or None)
  clips+=Name / clips-=Name / clips=default   add a clip over the first take, remove one, back to one per take.
                       Editing a clip of an uncustomized model first copies Unity's one-per-take defaults.
  human[Bone]=Transform  map a humanoid bone (Hips, LeftUpperArm, Left Thumb Proximal...); human[Bone]= unmaps it
  human.x=v            upperArmTwist, lowerArmTwist, upperLegTwist, lowerLegTwist, armStretch, legStretch,
                       feetSpacing, hasTranslationDoF
  human.skeleton=rebuild  a fresh skeleton pose from the file, keeping the bone mapping; the fix for ""Avatar Rig
                       Configuration mis-match"" warnings after a scale change (put it in the same call as the change)
  materials[Name]=Assets/M.mat   remap a material from the file to a project material; =None removes the remap
  Refused in Play Mode and while an Inspector holds unapplied changes to that model."),
            ("export", @"Where answers come from

  The Editor writes one JSON file per model to UserSettings/Waitress/ModelWaitress/Models/<guid>.json: every
  ModelImporter API setting with Unity's default for it, the file's unit and takes, clip settings with each take's
  defaults, the humanoid mapping, remaps, import warnings (the Editor's import log), and the imported result:
  hierarchy, components, meshes, avatars and clips. An export is current while the model's and its .meta's write
  times match; any command re-exports stale models through the Editor first. With no Editor open, the last export is
  shown with a note. Records of models no longer in the project are deleted on every export and refresh. Not detected: a changed AssetPostprocessor or default preset (run `refresh --force`).
  Defaults are Unity's own (an importer reset to its defaults), not a preset's."),
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
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ModelWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream);
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
