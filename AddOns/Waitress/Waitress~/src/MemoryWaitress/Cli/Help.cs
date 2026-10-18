using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MemoryWaitress.Cli
{
    public static class Help
    {
        public static void Index(TextWriter w)
        {
            w.WriteLine("memorywaitress — read Unity Memory Profiler snapshots (.snap) from the command line");
            w.WriteLine();
            w.WriteLine("Every command takes a snapshot first: a path, a folder (its newest snapshot), 'latest', or part of a");
            w.WriteLine("file name in <project>/MemoryCaptures/. Snapshots are read directly; no Editor and no package needed.");
            w.WriteLine("A 1.6 GB Editor snapshot reads in about 4 s.");
            w.WriteLine();
            w.WriteLine("overview");
            w.WriteLine("  info <snap> [--top N]            totals (native, graphics, managed), top types, leak hints");
            w.WriteLine("  types <snap> [--native|--managed] [--filter P]   totals per Unity object type and managed type");
            w.WriteLine("  native <snap> [--by area|root|allocation|size|allocator|region|label] [--filter P]");
            w.WriteLine("                                   native memory grouped, or the largest single allocations");
            w.WriteLine();
            w.WriteLine("objects");
            w.WriteLine("  objects <snap> [--type P] [--name P] [--kind asset|runtime|dontdestroy|manager|dontsave]");
            w.WriteLine("          [--sort total|native|gfx|managed|name]   Unity objects: native, graphics, managed shell");
            w.WriteLine("  managed <snap> [--type P] [--root P] [--by object|root]   largest managed objects and what holds them");
            w.WriteLine("  why <snap> <#id | @address | name | Type | field> [--managed]");
            w.WriteLine("                                   what keeps it alive: root path, referrers, holders");
            w.WriteLine();
            w.WriteLine("problems");
            w.WriteLine("  diff <a> <b> [--filter P]        what grew: totals, types, roots, areas, allocators, new objects");
            w.WriteLine("  leaks <snap> [--type P] [--root P]   destroyed Unity objects still referenced from managed code");
            w.WriteLine("  dupes <snap> [--type P]          Unity objects with the same type, name and size (likely duplicates)");
            w.WriteLine();
            w.WriteLine("snapshots (need the Editor)");
            w.WriteLine("  capture [--play [--wait S]] [--name N] [--callstacks] [--out x.snap] [--no-info]");
            w.WriteLine("                                   snapshot the open Editor (edit mode, or Play Mode with --play)");
            w.WriteLine("  list [folder]                    snapshots in MemoryCaptures/, newest first");
            w.WriteLine("  selftest                         offline checks of the reader and crawler on synthetic snapshots");
            w.WriteLine();
            w.WriteLine("shared options");
            w.WriteLine("  --top N      rows to print (0 = all); --project <dir> picks the Unity project");
            w.WriteLine("  P            a pattern: substring (case-insensitive), or a whole-name glob with * and ?");
            w.WriteLine();
            w.WriteLine("  help <topic>   topics: " + string.Join(", ", s_Topics.Select(t => t.name)));
            w.WriteLine("  skill [--install <dir>]   the whole tool on one page, for an agent");
        }

        static readonly (string name, string text)[] s_Topics =
        {
            ("numbers", @"What the numbers mean

  native       bytes of native allocations. A Unity object's native size is the sum of the allocations Unity
               rooted to it (its root's accumulated size), exactly as the Memory Profiler window's Unity Objects
               table shows it. `native --by area` groups every root by area (Objects, UnsafeUtility, Managers...).
  graphics     graphics memory Unity tracked per resource (textures, meshes, buffers), owned by the object whose
               root it was reported under. It is an estimate of GPU memory, not a driver measurement.
  managed      a live managed object's size: header plus fields; arrays add length x element size; strings
               20 + 2 x length. ""Live"" means reachable from a GC handle or a static field. The managed heap
               itself is usually much bigger than its live objects: the rest is free space and garbage that
               has not been collected yet, so heap size is not a leak signal; live object size is.
  allocated    (native --by area|root) the allocations rooted there; these sum to the native total, with
               ""(unrooted)"" for the rest. reported is Unity's accumulated size for the root, which can include
               memory that is not an allocation (System / ExecutableAndDlls is the mapped executables).
  Entities     chunks live in 1 MB UnsafeUtility Malloc(Persistent) blocks of 64 chunks (megachunks); `native --by size`
               counts them. Blob assets and other Persistent allocations share that root and can't be told apart.
  vs runtime   native sizes are what the snapshot reports, as in the Memory Profiler window. Profiler.
               GetRuntimeMemorySizeLong agrees for textures and meshes but reports Materials about 25% higher.
  shell        the managed object of a Unity object (the C# Texture2D next to the native one).
  unity used   Unity's own memory counters at capture time (the Profiler's Memory module totals).
  kind         asset (loaded from disk), runtime (created at runtime or part of a loaded scene), dontdestroy,
               manager. Editor snapshots also contain everything the Editor itself has loaded."),
            ("snapshots", @"Taking snapshots

  capture                snapshots the Editor as it is (edit mode, or Play Mode if it is playing).
  capture --play         enters Play Mode if it isn't playing, waits --wait seconds (2), snapshots, and leaves
                         Play Mode again only if it entered it.
  capture --name before  appends a label to the file name, so `diff before after` finds both.
  capture --callstacks   also records native allocation callstacks (much bigger files; rarely needed).

  Files go to <project>/MemoryCaptures/ (Unity's default .gitignore excludes it). An Editor snapshot holds the
  whole Editor process, 1-2 GB for a DOTS project, and takes a few seconds. For game-only numbers, take one in a
  development player: call Unity.Profiling.Memory.MemoryProfiler.TakeSnapshot from game code, or use the Memory
  Profiler window connected to the player, then read the .snap here.

  For a leak hunt: capture A, repeat the suspect action several times, capture B, `diff A B`. Within one Editor
  run objects keep their instance ids, so `diff` lists exactly the Unity objects that are new."),
            ("why", @"What keeps an object alive

  why <snap> @1f8c1ffece0   a managed object: the shortest chain of references from a root (a static field or a
                            GC handle) down to it, then every other reference to it.
  why <snap> #-11022        a Unity object (by instance id): its native referrers (a Material using a Texture),
                            the managed objects referencing its shell, and a chain of native referrers upward.
  why <snap> Name           the largest Unity object with that name (others listed).
  why on a managed object also prints what it reaches first: the objects whose shortest path from a root runs
  through it, a rough retained size.

  Roots are GC handles and static fields, as in the Memory Profiler package. Objects reachable only from thread
  stacks, AppDomain data (AppDomain.SetData), or native pointers without a GC handle are not found at all, so
  `referrers: 0` can mean such a holder.

  why <snap> s_Leak         a static field (field, or Type.field): what it holds, by type.
  why <snap> X --managed    treat X as a managed type even when a Unity object has that name.
  why <snap> LeakNode       every live instance of a managed type (full name, name without namespace, or a glob),
                            grouped by the root holding them, with one example path per root.

  Roots: `static Type.field` is a static field; `GC handle of <Unity object>` is a Unity object's own handle to its
  shell; `GC handle to a T (held by native code)` is a GCHandle.Alloc or an engine-held object (ECS systems,
  for example). Paths are shortest, so a
  shared object shows one of several possible paths; `why` on it lists all of its referrers."),
            ("leaks", @"Finding leaks

  diff A B     after repeating an action: types whose count or size grew, and Unity objects new in B.
  leaks        managed objects of UnityEngine.Object types whose native object was destroyed but which managed code
               still references (m_CachedPtr is null). The shell is small, but the reference is usually a missed
               cleanup and can keep large managed data alive. --root narrows to your code's roots.
  dupes        Unity objects with identical type, name and size: often the same asset loaded or created twice.
  managed --by root   which static fields and handles hold the most managed memory.

  Editor snapshots contain the Editor's own leaks and duplicates; filter with --type, --root or --filter on
  names from your project."),
            ("format", @"Snapshot format support

  Reads .snap files of format 11 and newer (Unity 2021.2+). Field and type data come from the snapshot itself, so
  no package, project or Editor is needed to read one. Tested on format 17 (Unity 6000.3, Mono). IL2CPP objects
  point at their class directly instead of through a vtable; the crawler accepts both, but IL2CPP snapshots
  have not been tested.

  Checked against the Memory Profiler package (1.1.9, run in batch mode on the same files): every Unity object's
  native, graphics and managed size matched exactly, and so did every managed type's count and size, except
  that MemoryWaitress also follows references inside struct-typed static fields (such as ImmutableArray<T>.Empty).
  The package misses those, so MemoryWaitress can find a few hundred more live objects in an Editor snapshot."),
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
            var commands = new[] { "info", "types", "native", "objects", "managed", "why", "diff", "leaks", "dupes", "capture", "list" };
            if (commands.Contains(topic))
            {
                Index(w);
                return;
            }
            throw new WaitressException($"no help topic '{topic}'; topics: {string.Join(", ", s_Topics.Select(t => t.name))}");
        }

        public static int Skill(string install, TextWriter w)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MemoryWaitress.Docs.skill.md");
            using var reader = new StreamReader(stream);
            return Waitress.Skill.Write(reader.ReadToEnd(), install, w);
        }
    }
}
