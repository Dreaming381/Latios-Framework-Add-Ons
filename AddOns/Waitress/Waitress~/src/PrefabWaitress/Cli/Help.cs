using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PrefabWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("prefabwaitress — follow prefab override chains in Unity scenes and prefabs, and edit overrides");
            w.WriteLine();
            w.WriteLine("Reading needs no Editor. Every prefab instance is resolved through its variants and nested prefabs, so each");
            w.WriteLine("value is the one Unity uses and knows which file set it. <file> is a .unity or .prefab path (or Assets/... path).");
            w.WriteLine();
            w.WriteLine("read");
            w.WriteLine("  tree <file> [object] [--components] [--depth N] [--deep] [--instances]");
            w.WriteLine("                     hierarchy; instances shown as Name [Source.prefab < Base.prefab, N overrides] and collapsed;");
            w.WriteLine("                     --instances lists every instance, nested ones too");
            w.WriteLine("  show <file> <object> [Component] [--all] [--filter P]");
            w.WriteLine("                     an object's values; overridden ones end in `<- File` (the file that set them)");
            w.WriteLine("  why <file> <object> <Component.property>");
            w.WriteLine("                     that value at every layer, from the base prefab up to this file");
            w.WriteLine("  overrides <file|folder> [object] [--unused] [--redundant]");
            w.WriteLine("                     what each instance overrides, adds and removes; unused and redundant overrides");
            w.WriteLine("  instances <prefab> [object] [--prop Component.property] [--in folder]");
            w.WriteLine("                     every place the prefab is used, through variants and nesting too; with --prop, which");
            w.WriteLine("                     places keep their own value, so a change to the prefab won't reach them");
            w.WriteLine();
            w.WriteLine("edit (needs the Editor open; Unity's PrefabUtility writes the files, then the result is read back)");
            w.WriteLine("  set <file> <object> Component.property=value [...]   on an instance, each becomes an override");
            w.WriteLine("  revert <file> <object> [Component[.property]] [--removed Name] [--all]");
            w.WriteLine("  apply <file> <object> [Component[.property]] [--removed Name] [--to <prefab>] [--all]");
            w.WriteLine("  clean <file> [--redundant] [--dry-run]   remove unused overrides (and ones equal to the prefab)");
            w.WriteLine("  models [file|folder] [--all]             export imported models, so objects from .fbx files can be read");
            w.WriteLine();
            w.WriteLine("  --project <dir> picks the Unity project; P is a substring or a glob with * and ?");
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("paths", @"Naming objects, components and properties

  object      a hierarchy path from a root, Root/Child/Leaf, or just a name when only one object has it. Siblings that
              share a name are told apart by [n], counting from 0 (Door[1]), exactly as `tree` prints them. &<fileID>
              names an object by its id in the file. * and ? glob a segment for `tree`.
  Component   a type (Transform, MeshRenderer) or a script's class name (DoorController); Type[n] when an object has
              several. GameObject names the object itself (m_Name, m_IsActive, m_Layer...).
  property    Unity's serialized name, as in the YAML and in SerializedProperty: m_LocalPosition, speed, hinge.limits.
              Lists take [i] for an element and .size for the length: m_Materials[0], waypoints.size (Unity's own
              .Array.data[i] and .Array.size are accepted too). A whole struct works for reading (m_LocalPosition);
              `set` takes its parts or a comma list (Transform.m_LocalPosition=0,1,0).
  Component.property   `why`, `set` and `instances --prop` take both at once: MeshRenderer.m_Materials[0]. A property
              only one component has can be given alone."),
            ("overrides", @"How overrides are reported

  `<- File.prefab` after a value: the file whose override set it. In a scene, a value can come from the scene, from a
  variant, or from a prefab that nests this one; `why` shows every layer.
  (prefab: X)   the value the instance would have without this file's override.
  root: ...     the instance root's name and placement. Unity writes these on every instance and its Overrides window
                hides them; they are listed on one line.
  +Type on X    a component this file added to an object of the prefab;  +Name under X  an added GameObject.
  -Type on X    a component this file removed;  -X (removed object)  a removed GameObject.
  unused        an override Unity ignores: its target object no longer exists in the prefab, or the property doesn't
                exist (a renamed or deleted field, m_RootOrder from before Unity 2022). `clean` removes them.
  redundant     an override that sets the value the prefab already has. Harmless today, but it pins the value: a later
                change to the prefab won't reach this instance. `clean --redundant` removes them.
  unchecked     the source can't be read (an .fbx not yet exported with `models`, or a missing prefab), so the
                override is shown but can't be classified.
  A field missing from a file saved by an older Unity version is filled in by Unity on load; the tool reports only
  what files contain (`why` says so when no layer writes a property)."),
            ("edits", @"Editing

  Edits go through the open Editor: the tool sends one request with every change, the Editor makes them with
  SerializedObject and PrefabUtility, saves, and the tool reads the file back and prints the result.
  set       values: numbers, true/false, enum names or numbers, strings, comma lists for vectors and colors, None for
            no reference, an asset path for an asset (Assets/Mats/Red.mat, or Assets/x.fbx#4300000 for a sub-asset), or
            @Path/To/Object (@Path:Component for one of its components) for an object in the same file. Setting a
            value on a prefab instance creates the override; several Component.property=value pairs go in one call.
  revert    a property, a whole component, an object and its components, an added component or object (removes it),
            --removed Name (restores a removed component by type, or a removed child GameObject by name), or --all
            on an instance root (everything).
  apply     the same targets, written into the prefab instead; --to picks which prefab in the chain receives it (a
            file name is enough). By default it is the prefab this file instances, not the innermost one Unity would pick. Applying changes the prefab, so it reaches every instance
            that doesn't override it; after applying a property, the tool prints which instances follow it.
  clean     removes unused overrides, and with --redundant also those equal to the prefab. --dry-run lists them.
            The Editor confirms each one first: an override on a field the source file merely predates is kept.
  An open scene with unsaved changes is refused unless --save-open (which saves those changes too); a prefab open in
  Prefab Mode with unsaved changes is refused. Everything is checked before anything changes, and the Editor checks
  that each object's name still matches, so a file changed since it was read fails cleanly. Edits can't be made
  in Play Mode."),
            ("models", @"Objects from models

  Scenes and prefabs often instance .fbx (and other model) files. Their objects come from the importer, not from a
  file, so until they are exported the tool sees only what overrides say about them, marks those overrides
  `unchecked`, and `tree` shows the instance without its children.
  models              exports every model the project's scenes and prefabs instance (needs the Editor).
  models <file>       just the models a scene or prefab uses; models <folder> those used under a folder.
  Exports go to UserSettings/Waitress/PrefabWaitress/Models/ and are used only while the model and its .meta are
  unchanged; re-run `models` after reimporting. --all re-exports everything."),
            ("format", @"What is read

  Text-serialized scenes and prefabs (the Unity default). Prefab instances are resolved the way the Editor resolves
  them: source objects take the ids the instance's stripped objects give them (or the XOR rule for objects the file
  doesn't mention), overrides apply in file order, removed components and objects drop out, and added ones take
  their recorded insert index (or m_RootOrder in files from before Unity 2022). A prefab's root is named after its
  file, as in the Editor. Files saved before Unity 2018.3 write instanced objects out in full; those values are read
  as written, without saying which prefab they came from. Scripts inside DLLs (including the UnityEngine.UI DLL older
  projects used) are named by Unity's file id for each class, an MD4 hash of its namespace and name.

  Checked against the Editor (SerializedObject on every object of 830 scenes and prefabs, 2.9 million values): every
  value the files hold matched, including every overridden one, once Unity's own on-load changes are set aside
  (fields newer Unity versions add or rename, scripts that upgrade old data, layout a Canvas drives, lightmap
  data applied from the lighting asset)."),
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
            var commands = new[] { "tree", "show", "why", "overrides", "instances", "set", "revert", "apply", "clean", "models" };
            if (commands.Contains(topic))
            {
                // The command's own lines from the index, with the topic that explains its arguments.
                var index = new StringWriter();
                Index(index);
                var mine = false;
                foreach (var line in index.ToString().Split('\n'))
                {
                    var t = line.TrimEnd('\r');
                    if (t.StartsWith("  ", StringComparison.Ordinal) && !t.StartsWith("   ", StringComparison.Ordinal))
                        mine = t.Substring(2).StartsWith(topic + " ", StringComparison.Ordinal);
                    else if (!t.StartsWith("   ", StringComparison.Ordinal))
                        mine = false;
                    if (mine)
                        w.WriteLine(t);
                }
                w.WriteLine(topic is "set" or "revert" or "apply" or "clean" ? "see `help edits` and `help paths`" : topic == "models" ? "see `help models`" : "see `help paths` and `help overrides`");
                return;
            }
            throw new WaitressException($"no help topic '{topic}'; topics: {string.Join(", ", s_Topics.Select(t => t.name))}");
        }

        public static int Skill(string install, TextWriter w)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PrefabWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream);
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
