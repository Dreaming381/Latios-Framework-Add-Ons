using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildWaitress.Serialized;

namespace BuildWaitress.Testing
{
    /// <summary>Raw view of a serialized file, for working out formats.</summary>
    public static class Dump
    {
        public static int Run(Args a, TextWriter w)
        {
            var path = a.Positional0("file");
            if (path.EndsWith(".entityheader", StringComparison.OrdinalIgnoreCase))
            {
                var h = Model.EntityScene.ReadHeader(path);
                w.WriteLine($"scene {h.Guid} '{h.Name}' sections {h.Sections.Count}");
                foreach (var s in h.Sections)
                    w.WriteLine($"  section {s.Index}: {s.FileSize} bytes (raw {s.RawSize}, codec {s.Codec}), {s.ObjectRefs} object refs, bounds {string.Join(",", s.Bounds)}");
                return 0;
            }
            if (path.EndsWith(".entities", StringComparison.OrdinalIgnoreCase))
            {
                var es = Model.EntitySection.Read(path, null);
                var types = Model.TypeTable.Load(a.Get("types"), a.Get("log"));
                var entity = types.ByHash.Values.FirstOrDefault(t => t.Name == "Unity.Entities.Entity") ?? new Model.EcsType { SizeInChunk = 8, Name = "Entity" };
                es.Layout(types, entity);
                es.AttributeBlobs(types);
                w.WriteLine($"v{es.FileVersion} {es.Size} bytes; types {es.TypeHashes.Count}, archetypes {es.Archetypes.Count}, chunks {es.Chunks.Count}, blobs {es.Blobs.Count}, buffer records {es.BufferRecords.Count}, shared {es.SharedComponents}, debug names {es.DebugNames.Count}");
                foreach (var n in es.Nodes)
                    w.WriteLine($"  node {n.Key}: {n.Value.size} bytes at {n.Value.at}");
                foreach (var ar in es.Archetypes)
                    w.WriteLine($"  archetype {ar.Index}: {ar.Entities} entities in {ar.Chunks} chunks, capacity {ar.Capacity}: " + string.Join(", ", ar.Types.Select((t, i) => types.Get(es.TypeHashes[t]).Short + (ar.ChunkComponent[i] ? "(chunk)" : "") + (ar.Sizes != null ? "/" + ar.Sizes[i + 1] : ""))));
                foreach (var g in es.Blobs.GroupBy(b => b.Owner ?? "?"))
                    w.WriteLine($"  blobs of {g.Key}: {g.Count()} totalling {g.Sum(b => (long)b.Length)} bytes, {g.Sum(b => b.References)} refs");
                return 0;
            }
            if (Path.GetFileName(path) == "archive_dependencies.bin")
            {
                var c = Model.ContentCatalog.Read(path);
                w.WriteLine($"archives {c.Archives.Count} files {c.Files.Count} objects {c.Objects.Count} scenes {c.Scenes.Count} deps {c.Dependencies.Count}");
                foreach (var x in c.Archives.Take(5))
                    w.WriteLine("  archive " + x);
                foreach (var x in c.Files.Take(5))
                    w.WriteLine($"  file {x.id} archive {x.archive} deps {x.dependencies}");
                foreach (var x in c.Objects.Where(o => a.Flag("all") || o.GenerationType == 6).Take(a.GetInt("items", 8)))
                    w.WriteLine($"  object {x.AssetGuid}:{x.LocalId} {Model.ContentCatalog.GenerationNames[Math.Clamp(x.GenerationType, 0, 6)]} file {x.FileIndex} path {x.PathId}");
                foreach (var x in c.Scenes.Take(5))
                    w.WriteLine($"  scene {x.id} file {x.file} {x.name}");
                foreach (var x in c.Dependencies.Take(5))
                    w.WriteLine("  deps " + string.Join(",", x));
                return 0;
            }
            var head = new byte[64];
            using (var s = File.OpenRead(path))
                s.Read(head, 0, head.Length);
            if (UnityArchive.Looks(head))
            {
                var arc = UnityArchive.Read(path);
                w.WriteLine($"UnityFS {arc.UnityVersion} {arc.Compression}");
                foreach (var n in arc.Nodes)
                {
                    w.WriteLine($"  node {n.Path} {n.Size} bytes flags {n.Flags}");
                    if (!n.IsSerializedFile)
                        continue;
                    var sf = arc.Open(n);
                    w.WriteLine($"    v{sf.Version} typetrees {sf.HasTypeTrees} objects {sf.Objects.Count} externals {sf.Externals.Count}");
                    foreach (var e in sf.Externals)
                        w.WriteLine($"    external {e.Guid} {e.Type} {e.Path}");
                    foreach (var o in sf.Objects.Take(a.GetInt("items", 4) * 4))
                        w.WriteLine($"    {o.PathId} class {o.ClassId} {o.Size} bytes");
                }
                return 0;
            }
            var f = SerializedFile.Read(path);
            var trees = a.Flag("trees");
            var limit = a.GetInt("items", 4);
            var only = a.Get("class");
            w.WriteLine($"v{f.Version} unity {f.UnityVersion} platform {f.Platform} typetrees {f.HasTypeTrees} objects {f.Objects.Count} externals {f.Externals.Count}");
            foreach (var e in f.Externals)
                w.WriteLine($"  external {e.Guid} {e.Type} {e.Path}");
            foreach (var g in f.Objects.GroupBy(o => o.ClassId))
                w.WriteLine($"  class {g.Key}: {g.Count()} objects, {g.Sum(o => (long)o.Size)} bytes ({f.Types[g.First().TypeIndex].Tree?.Type})");
            foreach (var o in f.Objects)
            {
                if (only != null && o.ClassId.ToString() != only && f.TypeOf(o)?.Tree?.Type != only)
                    continue;
                var t = f.TypeOf(o);
                w.WriteLine($"--- {o.PathId} class {o.ClassId} {t?.Tree?.Type} {o.Size} bytes");
                if (trees && t?.Tree != null)
                    Tree(w, t.Tree, "  ");
                if (t?.Tree != null)
                    Value(w, f.Decode(o), "  ", limit);
            }
            return 0;
        }

        static void Tree(TextWriter w, TypeNode n, string indent)
        {
            w.WriteLine($"{indent}{n.Type} {n.Name} [{n.ByteSize}]{(n.AlignAfter ? " align" : "")}{(n.Flags != 0 ? " f" + n.Flags : "")}");
            foreach (var c in n.Children)
                Tree(w, c, indent + "  ");
        }

        static void Value(TextWriter w, object v, string indent, int limit)
        {
            switch (v)
            {
                case Dictionary<string, object> d:
                    foreach (var kv in d)
                    {
                        if (kv.Value is Dictionary<string, object> || kv.Value is IList l && !(kv.Value is byte[]) && l.Count > 0)
                        {
                            w.WriteLine($"{indent}{kv.Key}:{(kv.Value is IList list ? $" ({list.Count})" : "")}");
                            Value(w, kv.Value, indent + "  ", limit);
                        }
                        else
                            w.WriteLine($"{indent}{kv.Key} = {Short(kv.Value)}");
                    }
                    break;
                case IList list when !(v is byte[]):
                    for (var i = 0; i < Math.Min(limit, list.Count); i++)
                    {
                        w.WriteLine($"{indent}[{i}]");
                        Value(w, list[i], indent + "  ", limit);
                    }
                    if (list.Count > limit)
                        w.WriteLine($"{indent}... {list.Count - limit} more");
                    break;
                default:
                    w.WriteLine(indent + Short(v));
                    break;
            }
        }

        static string Short(object v) => v switch
        {
            byte[] b => b.Length <= 32 ? "bytes " + Convert.ToHexString(b) : $"bytes[{b.Length}]",
            IList l => $"[{l.Count}]",
            null => "null",
            _ => v.ToString()
        };
    }
}
