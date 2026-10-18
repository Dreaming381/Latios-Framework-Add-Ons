using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BuildWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("buildwaitress — Unity player builds: what is in a build and why, what changed between builds, what is wrong with");
            w.WriteLine("one, and what is inside its Entities subscenes (entities, archetypes, chunk fill, blobs, referenced Unity objects)");
            w.WriteLine();
            w.WriteLine("Everything is read offline from the build report (Library/LastBuild.buildreport, archived to");
            w.WriteLine("UserSettings/Waitress/BuildWaitress/Reports/ the first time the tool sees it) and from the build folder.");
            w.WriteLine("<build> is last (default), prev or ~N (history), a .buildreport file, or a build folder; --folder <dir> pairs a");
            w.WriteLine("report with a moved copy of its build. Sizes are binary (1 MB = 2^20 bytes).");
            w.WriteLine();
            w.WriteLine("read");
            w.WriteLine("  summary [build]              result, options, time, size by category (code, scenes and assets, subscenes,");
            w.WriteLine("                               subscene content, streaming assets), largest assets, subscenes, problem count");
            w.WriteLine("  files [filter] [--by folder] [--top 25] [--build b]   output files by size (filter: text, glob or category)");
            w.WriteLine("  assets [filter] [--by asset|type|folder|package|file] [--top 25] [--build b]");
            w.WriteLine("                               packed assets totalled per source asset (texture data in .resS included), plus");
            w.WriteLine("                               the Unity objects in subscene content archives; filter by path, glob or type");
            w.WriteLine("  why <asset|name|archive> [--build b]   where an asset is in the build, how big, and why it is included");
            w.WriteLine("                               (Resources, the scenes using it with --detailed builds, the chain of objects");
            w.WriteLine("                               from a subscene section to it); a content archive name lists what it holds");
            w.WriteLine("  subscenes [name] [--archetypes] [--top 8] [--build b]");
            w.WriteLine("                               per subscene and section: size split into chunks, blobs and buffers, entities,");
            w.WriteLine("                               archetypes, chunk capacity and fill, blobs by owning component, Unity objects");
            w.WriteLine("  steps [build] [--all]        slowest build steps, and every error and warning with its step");
            w.WriteLine("  check [build]                problems with fixes: failed builds, stale files shipping from earlier builds,");
            w.WriteLine("                               orphan content archives, assets duplicated between scenes and subscenes, near-");
            w.WriteLine("                               empty chunks, test assemblies, debug options. Exit 1 on errors or warnings");
            w.WriteLine("  diff [old] [new] [--top 12]  size changes (default prev → last): total, categories, files, assets, subscenes");
            w.WriteLine("  history                      archived reports");
            w.WriteLine();
            w.WriteLine("act (need the Editor)");
            w.WriteLine("  build [scene...] [--target win64|linux|mac|android|webgl|<BuildTarget>] [--out path] [--dev] [--detailed]");
            w.WriteLine("        [--option <BuildOptions>] [--clean] [--no-wait] [--timeout 120]");
            w.WriteLine("                               builds the player (scenes default to the build profile's list), waits, prints");
            w.WriteLine("                               the summary, the errors logged and any project files the build changed");
            w.WriteLine("  types                        exports Entities type names and sizes for subscene analysis (automatic when stale)");
            w.WriteLine();
            w.WriteLine("  --project <dir> picks the Unity project");
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("subscenes", @"Subscenes in a build

  An Entities build writes each subscene as StreamingAssets/EntityScenes/<guid>.entityheader plus one
  <guid>.<section>.entities file per section, and the Unity objects subscenes reference (meshes, materials,
  textures, shaders) into StreamingAssets/ContentArchives/, indexed by archive_dependencies.bin. Unity's build
  report says nothing about what is inside them; buildwaitress reads them directly.

  A section file holds: archetypes (component sets), every chunk as a full 16 KB block, blob assets, buffers that
  outgrew their chunk, shared and managed components. So a section's size is mostly
    chunks × 16 KB  — many small archetypes (entities that differ by one component) each cost a whole chunk
    blobs           — attributed to the first component type referencing each blob (colliders, skeletons, curves)
  Capacity and fill need component sizes, which come from the Editor (`buildwaitress types`, run automatically
  when the Editor is open); without it, names come from Logs/ExportedTypes.log, written by every DOTS build.

  Content archives on disk that the catalog doesn't list, and subscene files older than the build, are leftovers
  that still ship (`check`). An asset referenced both from a scene and from a subscene ships twice (`check`).
  Unity's ImHex patterns (UNITY_DOTS_IMHEX, Library/GeneratedImHex) describe the same section format for manual
  hex inspection; buildwaitress needs neither the define nor ImHex."),
            ("build", @"Building

  build [scene...] [--target T] [--out path] [--dev] [--detailed] [--option X] [--clean] [--no-wait]
  Scenes are asset paths or names; none means the scenes enabled in the build profile. The default output is
  Builds/<target>/<productName>.exe (or .x86_64/.app/.apk, or a folder). --detailed records which scenes use each
  asset (`why`); --clean deletes the output folder first (only one that looks like a player build), so no files from
  an earlier build ship. The build runs in the Editor on its next update; the tool waits and reads the result,
  including errors that happen before Unity's build steps start (script compilation, build processors, Burst),
  which the report itself doesn't keep. It refuses while a scene has unsaved changes, since the build would stop
  at a save dialog. Unity, render pipelines and packages rewrite some settings files during builds; the tool lists
  every project file the build changed."),
            ("check", @"Problems check reports

  error    the build failed (with its first errors); a content archive the catalog needs is missing; a subscene
           section doesn't match its header
  warning  files in the build folder this build didn't write (they still ship); content archives the catalog
           doesn't list; subscene files older than the build; assets stored twice, in scene data and in subscene
           content; sections with 128 KB+ of empty chunk space; section files no header lists; test assemblies; deep profiling, script
           debugging or forced assertions; debug symbols (.pdb, *_DoNotShip) in a release build folder
  note     development build (and its symbols); the folder has been rebuilt or cleaned since the report; the
           Performance Testing package's files in Resources; checks skipped for a folder without its report"),
        };

        public static void Topic(TextWriter w, string topic)
        {
            if (topic == null)
            {
                Index(w);
                return;
            }
            var t = s_Topics.FirstOrDefault(x => x.name.Equals(topic, StringComparison.OrdinalIgnoreCase));
            if (t.text != null)
            {
                w.WriteLine(t.text);
                return;
            }
            // A command's own lines from the index.
            var index = new StringWriter();
            Index(index);
            var lines = index.ToString().Replace("\r", "").Split('\n');
            var start = Array.FindIndex(lines, l => l.StartsWith("  " + topic + " ", StringComparison.OrdinalIgnoreCase) || l.Equals("  " + topic, StringComparison.OrdinalIgnoreCase));
            if (start < 0)
                throw new WaitressException($"no help topic or command '{topic}'; topics: {string.Join(", ", s_Topics.Select(x => x.name))}");
            w.WriteLine(lines[start].Trim());
            for (var i = start + 1; i < lines.Length && lines[i].StartsWith("        ", StringComparison.Ordinal); i++)
                w.WriteLine("  " + lines[i].Trim());
            w.WriteLine("  build: last (default), prev, ~N, a .buildreport, or a build folder; --folder <dir> pairs a report with a moved folder");
        }

        public static int Skill(string install, TextWriter w)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BuildWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream ?? throw new WaitressException("skill text missing from the build"));
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
