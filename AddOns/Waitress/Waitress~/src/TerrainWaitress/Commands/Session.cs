using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrainWaitress.Model;

namespace TerrainWaitress.Commands
{
    /// <summary>The project a command works in, created on first use so commands that don't read terrains stay instant.</summary>
    public sealed class Session
    {
        public static string ProjectOverride;
        readonly string m_Hint;
        Catalog m_Catalog;

        public Session(string hint, TextWriter notes)
        {
            m_Hint = hint;
            Notes = notes;
        }

        /// <summary>Where notes about stale or refreshed records go, ahead of the command's own output.</summary>
        public TextWriter Notes { get; }

        public string Root
        {
            get
            {
                if (ProjectOverride != null)
                    return UnityProject.FindRoot(ProjectOverride) ?? throw new WaitressException($"--project {ProjectOverride} is not inside a Unity project (no Assets/ and ProjectSettings/ above it)");
                return UnityProject.FindRoot(m_Hint) ?? UnityProject.Current ?? throw new WaitressException("not inside a Unity project; run from the project folder or pass --project <path>");
            }
        }

        public Catalog Catalog => m_Catalog ??= new Catalog(Root);

        public TerrainRecord Record(TerrainFile f, bool force = false) => Catalog.Get(f, Notes, force);

        TerrainFile FileOf(string guid) => Catalog.All.FirstOrDefault(f => f.Guid == guid);

        /// <summary>
        /// The terrains an argument names: a TerrainData asset (local coordinates), a Terrain object as Scene.unity:Name or a
        /// name unique in the project (world coordinates), a scene or prefab (all its terrains), or a folder (all assets).
        /// </summary>
        public List<Target> Targets(string what)
        {
            var colon = what.LastIndexOf(':');
            if (colon > 2 && (what.Substring(0, colon).EndsWith(".unity", StringComparison.OrdinalIgnoreCase) || what.Substring(0, colon).EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)))
            {
                var file = what.Substring(0, colon);
                var name = what.Substring(colon + 1);
                var inFile = PlacedIn(file);
                var hit = inFile.Where(p => p.HierarchyPath == name || p.Object == name || p.HierarchyPath.EndsWith("/" + name, StringComparison.Ordinal)).ToList();
                if (hit.Count == 0)
                    hit = inFile.Where(p => string.Equals(p.Object, name, StringComparison.OrdinalIgnoreCase)).ToList();
                if (hit.Count == 0)
                    throw new WaitressException($"{file} has no Terrain '{name}'; it has {(inFile.Count == 0 ? "none" : string.Join(", ", inFile.Select(p => p.HierarchyPath)))}");
                if (hit.Count > 1)
                    throw new WaitressException($"'{name}' names {hit.Count} terrains in {file}; use the hierarchy path: {string.Join(", ", hit.Select(p => p.HierarchyPath))}");
                return Placed(hit);
            }
            if (what.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) || what.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            {
                var inFile = PlacedIn(what);
                if (inFile.Count == 0)
                    throw new WaitressException($"{what} has no Terrain components (as saved on disk)");
                return Placed(inFile);
            }
            var asset = Catalog.TryAsset(what);
            if (asset != null)
                return new List<Target> { new Target { Data = Record(asset) } };
            if (Catalog.TryFolder(what, out var under))
            {
                if (under.Count == 0)
                    throw new WaitressException($"no TerrainData assets under {what}");
                return Catalog.Get(under, Notes).Select(r => new Target { Data = r }).ToList();
            }
            var named = Catalog.Placements.Where(p => p.Object == what || p.HierarchyPath == what).ToList();
            if (named.Count == 0)
                named = Catalog.Placements.Where(p => string.Equals(p.Object, what, StringComparison.OrdinalIgnoreCase)).ToList();
            if (named.Count == 1)
                return Placed(named);
            if (named.Count > 1)
                throw new WaitressException($"'{what}' names {named.Count} terrains; use Scene.unity:Name:\n  " + string.Join("\n  ", named.Take(10).Select(p => p.Ref)));
            var leaf = Path.GetFileName(what.Replace('\\', '/'));
            var alike = Catalog.All.Where(f => string.Equals(Path.GetFileName(f.AssetPath), leaf, StringComparison.OrdinalIgnoreCase) ||
                                               string.Equals(Path.GetFileNameWithoutExtension(f.AssetPath), leaf, StringComparison.OrdinalIgnoreCase)).ToList();
            if (alike.Count > 0)
                throw new WaitressException($"no terrain '{what}'; did you mean {string.Join(" or ", alike.Take(3).Select(f => f.AssetPath))}?");
            throw new WaitressException($"no terrain '{what}' (a TerrainData asset, Scene.unity:TerrainObject, a scene or prefab, a folder, or a unique Terrain object name)" +
                                        (Catalog.All.Count == 0 ? "; the project has no TerrainData assets" : ""));
        }

        public Target One(string what)
        {
            var all = Targets(what);
            if (all.Count == 1)
                return all[0];
            throw new WaitressException($"'{what}' holds {all.Count} terrains; name one:\n  " + string.Join("\n  ", all.Take(12).Select(t => t.World ? t.Placement.Ref : t.Data.Path)));
        }

        List<Placement> PlacedIn(string file)
        {
            var norm = file.Replace('\\', '/');
            var full = File.Exists(file) ? Path.GetFullPath(file) : File.Exists(Path.Combine(Root, file)) ? Path.GetFullPath(Path.Combine(Root, file)) : null;
            var rel = full != null ? Path.GetRelativePath(Root, full).Replace('\\', '/') : norm;
            var hits = Catalog.Placements.Where(p => string.Equals(p.File, rel, StringComparison.OrdinalIgnoreCase)).ToList();
            if (hits.Count > 0 || full != null)
                return hits;
            var byName = Catalog.Placements.Select(p => p.File).Distinct().Where(f => f.EndsWith("/" + norm, StringComparison.OrdinalIgnoreCase)).ToList();
            if (byName.Count > 1)
                throw new WaitressException($"'{file}' names {byName.Count} files: {string.Join(", ", byName)}");
            if (byName.Count == 0)
                throw new WaitressException($"no scene or prefab '{file}' with terrains");
            return Catalog.Placements.Where(p => p.File == byName[0]).ToList();
        }

        List<Target> Placed(List<Placement> placements)
        {
            var files = placements.Select(p => p.DataGuid).Distinct().Select(FileOf).Where(f => f != null).ToList();
            var records = Catalog.Get(files, Notes).ToDictionary(r => r.Guid);
            var result = new List<Target>();
            foreach (var p in placements)
            {
                if (p.DataGuid == null || !records.TryGetValue(p.DataGuid, out var r))
                {
                    Notes.WriteLine($"note: {p.Ref} has {(p.DataGuid == null ? "no TerrainData" : "a TerrainData that isn't in the project (" + p.DataGuid + ")")}; skipped");
                    continue;
                }
                result.Add(new Target { Data = r, Placement = p });
            }
            if (result.Count == 0)
                throw new WaitressException("none of those terrains has TerrainData");
            return result;
        }
    }
}
