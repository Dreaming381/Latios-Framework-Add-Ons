using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BuildWaitress.Model
{
    public sealed class EcsType
    {
        public ulong Hash;
        public ulong MemoryOrdering;
        public int Flags;
        public string Category;
        public int SizeInChunk = -1;
        public int TypeSize;
        public int ElementSize;
        public int BufferCapacity = -1;
        public int MaxChunkCapacity = 128;
        public int EntityOffsets;
        public int[] BlobOffsets = Array.Empty<int>();
        public bool BlobOffsetsUnknown;
        public string Name;

        public bool Known => SizeInChunk >= 0;
        public bool IsBuffer => BufferCapacity >= 0;
        public bool IsShared => Category != null && Category.Contains("Shared");
        public bool IsManaged => Category is "UnityEngineObject" || (Category != null && Category.Contains("Class"));
        public string Short => Name == null ? $"0x{Hash:x16}" : Name.Substring(Name.LastIndexOfAny(new[] { '.', '+' }) + 1);
    }

    /// <summary>
    /// Entities type information keyed by stable type hash: the Editor's TypeManager export (names, sizes, layout), or
    /// names only from the Logs/ExportedTypes.log a DOTS build writes.
    /// </summary>
    public sealed class TypeTable
    {
        public readonly Dictionary<ulong, EcsType> ByHash = new Dictionary<ulong, EcsType>();
        public string Source = "none";
        public bool HasLayout;

        public EcsType Get(ulong hash)
        {
            if (!ByHash.TryGetValue(hash, out var t))
                ByHash[hash] = t = new EcsType { Hash = hash };
            return t;
        }

        /// <summary>Adds the types of another export that this table doesn't have (or has only by name).</summary>
        public void Merge(string export)
        {
            if (!File.Exists(export))
                return;
            var other = Load(export, null);
            foreach (var kv in other.ByHash)
                if (!ByHash.TryGetValue(kv.Key, out var mine) || !mine.Known)
                    ByHash[kv.Key] = kv.Value;
            HasLayout |= other.HasLayout;
            if (Source == "none")
                Source = export;
        }

        public static TypeTable Load(string export, string exportedTypesLog)
        {
            var table = new TypeTable();
            if (export != null && File.Exists(export))
            {
                foreach (var line in File.ReadLines(export))
                {
                    if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
                        continue;
                    var c = line.Split('\t');
                    if (c.Length < 14)
                        continue;
                    var t = new EcsType
                    {
                        Hash = ulong.Parse(c[0], CultureInfo.InvariantCulture),
                        MemoryOrdering = ulong.Parse(c[1], CultureInfo.InvariantCulture),
                        Flags = int.Parse(c[2], CultureInfo.InvariantCulture),
                        Category = c[3],
                        SizeInChunk = int.Parse(c[4], CultureInfo.InvariantCulture),
                        TypeSize = int.Parse(c[5], CultureInfo.InvariantCulture),
                        ElementSize = int.Parse(c[6], CultureInfo.InvariantCulture),
                        BufferCapacity = int.Parse(c[7], CultureInfo.InvariantCulture),
                        MaxChunkCapacity = int.Parse(c[8], CultureInfo.InvariantCulture),
                        EntityOffsets = int.Parse(c[9], CultureInfo.InvariantCulture),
                        Name = c[13],
                    };
                    if (c[10].StartsWith("?", StringComparison.Ordinal))
                        t.BlobOffsetsUnknown = true;
                    else if (c[10].Length > 0)
                        t.BlobOffsets = c[10].Split(',').Select(x => int.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                    table.ByHash[t.Hash] = t;
                }
                table.Source = export;
                table.HasLayout = true;
            }
            if (exportedTypesLog != null && File.Exists(exportedTypesLog))
            {
                var any = false;
                foreach (var line in File.ReadLines(exportedTypesLog))
                {
                    if (!line.StartsWith("0x", StringComparison.Ordinal))
                        continue;
                    var parts = line.Split(" - ", 3);
                    if (parts.Length < 3 || !ulong.TryParse(parts[0].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hash))
                        continue;
                    var t = table.Get(hash);
                    t.Name ??= parts[2].Trim();
                    any = true;
                }
                if (any && !table.HasLayout)
                    table.Source = exportedTypesLog;
            }
            return table;
        }
    }
}
