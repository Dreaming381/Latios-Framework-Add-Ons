using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BuildWaitress.Serialized;

namespace BuildWaitress.Model
{
    public sealed class ArchiveObject
    {
        public string Archive;
        public long PathId;
        public int ClassId;
        public string Name;
        /// <summary>The archive isn't in the content catalog: a leftover of an earlier build.</summary>
        public bool Leftover;
        public readonly List<ArchiveObject> Uses = new List<ArchiveObject>();
        /// <summary>The archive's size on disk shared out by raw size, so object sizes add up to the archive files.</summary>
        public long Size;
        /// <summary>Serialized size plus streamed texture/mesh data, uncompressed.</summary>
        public long Raw;
        public readonly List<ArchiveObject> UsedBy = new List<ArchiveObject>();
        public string Label => $"{(ClassId < 0 ? "object" : ClassIds.Name(ClassId))} '{Name ?? "?"}'";
    }

    public sealed class Subscene
    {
        public EntityScene Header;
        public string Path;
        public readonly List<(SectionMeta meta, EntitySection data, string error)> Sections = new List<(SectionMeta, EntitySection, string)>();
        public readonly Dictionary<int, List<ArchiveObject>> Objects = new Dictionary<int, List<ArchiveObject>>();
        /// <summary>The scene's path without Assets/ and .unity when known (subscene names often repeat), else its baked name.</summary>
        public string Name => Path != null
            ? (Path.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase) ? Path.Substring(7) : Path).Replace(".unity", "")
            : (Header.Name.Length > 0 ? Header.Name + " [" + Header.Guid.Substring(0, 8) + "]" : Header.Guid);

        /// <summary>The scene GUID the section files are named after.</summary>
        public string Guid => Header.Guid;
        public long Size => Sections.Sum(s => s.meta.FileSize);
    }

    /// <summary>A player build: its report and, when the folder still exists, what is on disk, including subscenes.</summary>
    public sealed class Build
    {
        public Report Report;
        public string Dir;
        public string StreamingDir;
        public readonly List<string> Notes = new List<string>();
        readonly string m_ProjectRoot;
        TypeTable m_Types;
        List<Subscene> m_Subscenes;
        ContentCatalog m_Catalog;
        bool m_CatalogRead;
        Dictionary<string, (UnityArchive archive, SerializedFile file)> m_Archives;
        List<ArchiveObject> m_ArchiveObjects;
        Dictionary<string, string> m_PlayerNames;

        public Build(Report report, string dir, string projectRoot)
        {
            Report = report;
            Dir = dir;
            m_ProjectRoot = projectRoot;
            if (dir != null && Directory.Exists(dir))
            {
                var data = Directory.GetDirectories(dir, "*_Data").FirstOrDefault() ?? dir;
                var streaming = Path.Combine(data, "StreamingAssets");
                if (Directory.Exists(streaming))
                    StreamingDir = streaming;
            }
        }

        public bool OnDisk => Dir != null && Directory.Exists(Dir);

        /// <summary>Report files whose size on disk no longer matches: the folder has been rebuilt or edited since.</summary>
        public List<OutputFile> Changed()
        {
            if (Report == null || !OnDisk)
                return new List<OutputFile>();
            return Report.Files.Where(f => File.Exists(OnDiskPath(f)) && new FileInfo(OnDiskPath(f)).Length != f.Size).ToList();
        }

        public List<OutputFile> Missing()
        {
            if (Report == null || !OnDisk)
                return new List<OutputFile>();
            return Report.Files.Where(f => !File.Exists(OnDiskPath(f))).ToList();
        }

        /// <summary>Files in the output folder that the build didn't write: leftovers of earlier builds that would ship with this one.</summary>
        public List<(string rel, long size)> Stale()
        {
            var result = new List<(string, long)>();
            if (Report == null || !OnDisk)
                return result;
            var listed = new HashSet<string>(Report.Files.Select(f => Norm(OnDiskPath(f))), StringComparer.OrdinalIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(Dir, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(Dir, file).Replace('\\', '/');
                if (rel.Contains("_BurstDebugInformation_DoNotShip/") || rel.Contains("_BackUpThisFolder_ButDontShipItWithYourGame/"))
                    continue;
                if (!listed.Contains(Norm(file)))
                    result.Add((rel, new FileInfo(file).Length));
            }
            return result;
        }

        static string Norm(string p) => Path.GetFullPath(p).Replace('\\', '/');

        /// <summary>Where a reported file is now: under the folder in use, which differs from the report's when the build moved.</summary>
        public string OnDiskPath(OutputFile f) => Dir != null && !Path.IsPathRooted(f.Rel) ? Path.Combine(Dir, f.Rel) : f.Path;

        /// <summary>The running project's type export, used for hashes the build's own project doesn't explain.</summary>
        public string FallbackTypes;

        /// <summary>Set by the session for its own project: refreshes the Editor's type export when it is stale.</summary>
        public Action RefreshTypes;

        public TypeTable Types
        {
            get
            {
                if (m_Types != null)
                    return m_Types;
                RefreshTypes?.Invoke();
                var export = m_ProjectRoot == null ? null : Path.Combine(m_ProjectRoot, "UserSettings", "Waitress", "BuildWaitress", "types.tsv");
                var log = m_ProjectRoot == null ? null : Path.Combine(m_ProjectRoot, "Logs", "ExportedTypes.log");
                m_Types = TypeTable.Load(export, log);
                // A stable type hash pins the type's layout, so another project's export is valid wherever hashes match.
                if (FallbackTypes != null && !string.Equals(FallbackTypes, export, StringComparison.OrdinalIgnoreCase))
                    m_Types.Merge(FallbackTypes);
                return m_Types;
            }
            set => m_Types = value;
        }

        public ContentCatalog Catalog
        {
            get
            {
                if (m_CatalogRead)
                    return m_Catalog;
                m_CatalogRead = true;
                var path = StreamingDir == null ? null : Path.Combine(StreamingDir, "ContentArchives", "archive_dependencies.bin");
                if (path != null && File.Exists(path))
                {
                    try
                    {
                        m_Catalog = ContentCatalog.Read(path);
                    }
                    catch (Exception e) when (e is WaitressException || e is ArgumentException || e is IndexOutOfRangeException)
                    {
                        Notes.Add($"content catalog unreadable: {e.Message}");
                    }
                }
                return m_Catalog;
            }
        }

        public List<Subscene> Subscenes
        {
            get
            {
                if (m_Subscenes != null)
                    return m_Subscenes;
                m_Subscenes = new List<Subscene>();
                var dir = StreamingDir == null ? null : Path.Combine(StreamingDir, "EntityScenes");
                if (dir == null || !Directory.Exists(dir))
                    return m_Subscenes;
                var types = Types;
                var entity = types.ByHash.Values.FirstOrDefault(t => t.Name == "Unity.Entities.Entity") ?? new EcsType { SizeInChunk = 8, Name = "Entity" };
                var paths = ScenePaths();
                foreach (var header in Directory.GetFiles(dir, "*.entityheader").OrderBy(x => x, StringComparer.Ordinal))
                {
                    EntityScene h;
                    try
                    {
                        h = EntityScene.ReadHeader(header);
                    }
                    catch (Exception e) when (e is WaitressException || e is ArgumentException || e is IndexOutOfRangeException)
                    {
                        Notes.Add($"{Path.GetFileName(header)}: {e.Message}");
                        continue;
                    }
                    var s = new Subscene { Header = h, Path = paths.TryGetValue(h.Guid, out var p) ? p : null };
                    foreach (var meta in h.Sections)
                    {
                        var file = h.SectionFile(meta.Index);
                        try
                        {
                            var data = EntitySection.Read(file, meta);
                            data.Layout(types, entity);
                            data.AttributeBlobs(types);
                            foreach (var kv in data.DebugNames)
                                types.Get(kv.Key).Name ??= kv.Value;
                            s.Sections.Add((meta, data, null));
                        }
                        catch (Exception e) when (e is WaitressException || e is IOException || e is ArgumentException || e is IndexOutOfRangeException)
                        {
                            s.Sections.Add((meta, null, e.Message));
                        }
                    }
                    m_Subscenes.Add(s);
                }
                LinkObjects();
                return m_Subscenes;
            }
        }

        /// <summary>Scene GUID to asset path, from the project's scene .meta files.</summary>
        Dictionary<string, string> ScenePaths()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (m_ProjectRoot == null)
                return map;
            foreach (var top in new[] { "Assets", "Packages" })
            {
                var dir = Path.Combine(m_ProjectRoot, top);
                if (!Directory.Exists(dir))
                    continue;
                foreach (var meta in Directory.EnumerateFiles(dir, "*.unity.meta", SearchOption.AllDirectories))
                {
                    var guid = MetaGuid(meta);
                    if (guid != null)
                        map[guid] = Path.GetRelativePath(m_ProjectRoot, meta.Substring(0, meta.Length - 5)).Replace('\\', '/');
                }
            }
            return map;
        }

        static string MetaGuid(string meta)
        {
            foreach (var line in File.ReadLines(meta).Take(3))
                if (line.StartsWith("guid: ", StringComparison.Ordinal))
                    return line.Substring(6).Trim();
            return null;
        }

        /// <summary>Every object in the content archives, with its name and size.</summary>
        public List<ArchiveObject> ArchiveObjects
        {
            get
            {
                if (m_ArchiveObjects != null)
                    return m_ArchiveObjects;
                m_ArchiveObjects = new List<ArchiveObject>();
                foreach (var (name, (archive, file)) in Archives)
                {
                    if (file == null)
                        continue;
                    var objects = file.Objects.Select(o => new ArchiveObject { Archive = name, PathId = o.PathId, ClassId = o.ClassId, Raw = o.Size + StreamedSize(file, o), Name = NameOf(file, o) }).ToList();
                    var raw = Math.Max(1, objects.Sum(o => o.Raw));
                    var disk = new FileInfo(archive.FilePath).Length;
                    var listed = Catalog == null || Catalog.Files.Any(f => f.id.Equals(name, StringComparison.OrdinalIgnoreCase));
                    foreach (var o in objects)
                    {
                        o.Size = o.Raw * disk / raw;
                        o.Leftover = !listed;
                    }
                    m_ArchiveObjects.AddRange(objects);
                }
                LinkReferences();
                return m_ArchiveObjects;
            }
        }

        /// <summary>Which archive objects reference which (a material's textures and shader), through their PPtrs.</summary>
        void LinkReferences()
        {
            var byKey = m_ArchiveObjects.ToDictionary(o => (o.Archive.ToLowerInvariant(), o.PathId));
            foreach (var (name, (_, file)) in Archives)
            {
                if (file == null || !file.HasTypeTrees)
                    continue;
                foreach (var o in file.Objects)
                {
                    if (o.ClassId is 28 or 43 or 48 or 89 or 117 or 187 || o.Size > 1 << 20 || file.TypeOf(o)?.Tree == null)
                        continue;
                    if (!byKey.TryGetValue((name.ToLowerInvariant(), o.PathId), out var from))
                        continue;
                    object value;
                    try
                    {
                        value = file.Decode(o);
                    }
                    catch (WaitressException)
                    {
                        continue;
                    }
                    foreach (var (fileId, pathId) in PPtrs(value))
                    {
                        if (pathId == 0)
                            continue;
                        var target = fileId == 0 ? name : fileId - 1 < file.Externals.Count ? file.Externals[fileId - 1].Path : null;
                        if (target != null && byKey.TryGetValue((target.ToLowerInvariant(), pathId), out var to) && to != from && !to.UsedBy.Contains(from))
                        {
                            to.UsedBy.Add(from);
                            from.Uses.Add(to);
                        }
                    }
                }
            }
        }

        static IEnumerable<(int, long)> PPtrs(object value)
        {
            switch (value)
            {
                case Dictionary<string, object> d:
                    if (d.Count == 2 && d.TryGetValue("m_FileID", out var f) && d.TryGetValue("m_PathID", out var p))
                    {
                        yield return ((int)Report.L(f), Report.L(p));
                        yield break;
                    }
                    foreach (var v in d.Values)
                        foreach (var x in PPtrs(v))
                            yield return x;
                    break;
                case IList l when !(value is byte[]):
                    foreach (var v in l)
                        foreach (var x in PPtrs(v))
                            yield return x;
                    break;
            }
        }

        /// <summary>The content archives in the folder, keyed by file name; archives that fail to open have a null file.</summary>
        public Dictionary<string, (UnityArchive archive, SerializedFile file)> Archives
        {
            get
            {
                if (m_Archives != null)
                    return m_Archives;
                m_Archives = new Dictionary<string, (UnityArchive, SerializedFile)>(StringComparer.OrdinalIgnoreCase);
                var dir = StreamingDir == null ? null : Path.Combine(StreamingDir, "ContentArchives");
                if (dir == null || !Directory.Exists(dir))
                    return m_Archives;
                foreach (var path in Directory.GetFiles(dir))
                {
                    var name = Path.GetFileName(path);
                    if (name.Contains('.'))
                        continue;
                    try
                    {
                        var archive = UnityArchive.Read(path);
                        var node = archive.Nodes.FirstOrDefault(n => n.IsSerializedFile);
                        m_Archives[name] = (archive, node == null ? null : archive.Open(node));
                    }
                    catch (Exception e) when (e is WaitressException || e is IndexOutOfRangeException || e is ArgumentException || e is OverflowException)
                    {
                        Notes.Add($"content archive {name} is unreadable ({(e is WaitressException ? e.Message : "damaged or not an archive")})");
                        m_Archives[name] = (null, null);
                    }
                }
                return m_Archives;
            }
        }

        static readonly HashSet<int> s_NamedFirst = new HashSet<int>
        {
            21, 27, 28, 43, 48, 49, 74, 83, 84, 89, 90, 91, 93, 117, 128, 134, 150, 152, 156, 158, 187, 188, 213, 221, 238, 240, 258, 271, 272, 319, 329, 1102, 1107, 1109, 1111, 1113,
        };

        /// <summary>An object's name: m_Name through the type tree when there is one, else the leading string most asset classes start with.</summary>
        public static string NameOf(SerializedFile file, ObjectInfo o)
        {
            if (file.HasTypeTrees && file.TypeOf(o)?.Tree != null && o.Size < 4 << 20)
            {
                try
                {
                    var d = file.Decode(o) as Dictionary<string, object>;
                    if (d != null)
                    {
                        if (d.TryGetValue("m_ParsedForm", out var parsed) && parsed is Dictionary<string, object> pf && pf.TryGetValue("m_Name", out var sn) && sn is string shaderName && shaderName.Length > 0)
                            return shaderName;
                        if (d.TryGetValue("m_Name", out var n) && n is string name)
                            return name;
                    }
                }
                catch (WaitressException)
                {
                }
                return null;
            }
            return s_NamedFirst.Contains(o.ClassId) ? file.LeadingString(o) : null;
        }

        static long StreamedSize(SerializedFile file, ObjectInfo o)
        {
            if (!file.HasTypeTrees || (o.ClassId != 28 && o.ClassId != 43 && o.ClassId != 89 && o.ClassId != 187 && o.ClassId != 117) || o.Size >= 4 << 20)
                return 0;
            try
            {
                var d = file.Decode(o) as Dictionary<string, object>;
                if (d != null && d.TryGetValue("m_StreamData", out var sd) && sd is Dictionary<string, object> s && s.TryGetValue("size", out var size))
                    return Report.L(size);
            }
            catch (WaitressException)
            {
            }
            return 0;
        }

        /// <summary>The Unity objects each subscene section references, through the section's ReferencedUnityObjects list.</summary>
        void LinkObjects()
        {
            var catalog = Catalog;
            if (catalog == null)
                return;
            var byArchive = ArchiveObjects.GroupBy(o => o.Archive, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.ToDictionary(o => o.PathId), StringComparer.OrdinalIgnoreCase);
            foreach (var co in catalog.Objects.Where(o => o.GenerationType == 6))
            {
                var scene = m_Subscenes.FirstOrDefault(s => s.Header.Guid.Equals(co.AssetGuid, StringComparison.OrdinalIgnoreCase));
                if (scene == null || co.FileIndex < 0 || co.FileIndex >= catalog.Files.Count)
                    continue;
                var fileName = catalog.Files[co.FileIndex].id;
                if (!Archives.TryGetValue(fileName, out var arc) || arc.file == null)
                    continue;
                var info = arc.file.Objects.FirstOrDefault(o => o.PathId == co.PathId);
                if (info == null)
                    continue;
                // Entities lists either one ReferencedUnityObjects per section, or (older 1.x) every referenced object
                // under the section's id; both add up into the section's list.
                if (!scene.Objects.TryGetValue((int)co.LocalId, out var list))
                    scene.Objects[(int)co.LocalId] = list = new List<ArchiveObject>();
                if (info.ClassId != 114 || arc.file.TypeOf(info)?.Tree == null)
                {
                    if (byArchive.TryGetValue(fileName, out var own) && own.TryGetValue(co.PathId, out var direct) && !list.Contains(direct))
                        list.Add(direct);
                    continue;
                }
                try
                {
                    var d = (Dictionary<string, object>)arc.file.Decode(info);
                    if (!d.ContainsKey("Array") && byArchive.TryGetValue(fileName, out var self) && self.TryGetValue(co.PathId, out var behaviour) && !list.Contains(behaviour))
                        list.Add(behaviour);
                    if (d.TryGetValue("Array", out var refs) && refs is IList items)
                        foreach (var item in items)
                        {
                            var pptr = (Dictionary<string, object>)item;
                            var fileId = (int)Report.L(pptr["m_FileID"]);
                            var pathId = Report.L(pptr["m_PathID"]);
                            var target = fileId == 0 ? fileName : fileId - 1 < arc.file.Externals.Count ? arc.file.Externals[fileId - 1].Path : null;
                            if (target != null && byArchive.TryGetValue(target, out var objects) && objects.TryGetValue(pathId, out var obj))
                            {
                                if (!list.Contains(obj))
                                    list.Add(obj);
                            }
                            else if (target != null)
                                list.Add(new ArchiveObject { Archive = target, PathId = pathId, Name = target.Contains("unity default resources") ? "(built-in)" : null, ClassId = -1 });
                        }
                }
                catch (WaitressException e)
                {
                    Notes.Add($"{scene.Name} section {co.LocalId} references: {e.Message}");
                }
            }
        }

        /// <summary>Names of player-data objects (packed asset entries), read from the built files on disk when they are unchanged.</summary>
        public string PlayerName(PackedAsset a)
        {
            if (m_PlayerNames == null)
            {
                m_PlayerNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (Report != null && OnDisk)
                {
                    foreach (var file in Report.Files.Where(f => f.Role is "SharedAssets" or "Scene" or "ResourcesFile" or "GlobalGameManagers" || f.Path.EndsWith(".assets", StringComparison.OrdinalIgnoreCase)))
                    {
                        var onDisk = OnDiskPath(file);
                        if (!File.Exists(onDisk) || new FileInfo(onDisk).Length != file.Size)
                            continue;
                        try
                        {
                            var sf = SerializedFile.Read(onDisk);
                            var shortName = Path.GetFileName(file.Path);
                            foreach (var o in sf.Objects)
                            {
                                var name = NameOf(sf, o);
                                if (name != null)
                                    m_PlayerNames[shortName + ":" + o.PathId] = name;
                            }
                        }
                        catch (WaitressException)
                        {
                        }
                    }
                }
            }
            return m_PlayerNames.TryGetValue(a.File + ":" + a.FileId, out var n) ? n : null;
        }
    }
}
