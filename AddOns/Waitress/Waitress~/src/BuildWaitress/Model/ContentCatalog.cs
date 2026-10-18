using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildWaitress.Model
{
    public sealed class CatalogObject
    {
        public string AssetGuid;
        public long LocalId;
        public int GenerationType;
        public int FileIndex;
        public long PathId;
    }

    /// <summary>
    /// Entities content management's catalog (StreamingAssets/ContentArchives/archive_dependencies.bin): which archive
    /// holds which content file, and which object (asset GUID + local id) lives in which file.
    /// </summary>
    public sealed class ContentCatalog
    {
        public string Path;
        public readonly List<string> Archives = new List<string>();
        public readonly List<(string id, int archive, int dependencies)> Files = new List<(string, int, int)>();
        public readonly List<CatalogObject> Objects = new List<CatalogObject>();
        public readonly List<(string id, int file, string name)> Scenes = new List<(string, int, string)>();
        public readonly List<int[]> Dependencies = new List<int[]>();

        public static readonly string[] GenerationNames = { "Unknown", "UnityObject", "Texture", "GameObjectScene", "EntityScene", "EntityPrefab", "SubSceneObjectReferences" };

        public static ContentCatalog Read(string path)
        {
            var b = File.ReadAllBytes(path);
            var c = new ContentCatalog { Path = path };
            const int data = 4 + 32;
            var (archivesAt, archives) = Blob.Array(b, data);
            for (var i = 0; i < archives; i++)
                c.Archives.Add(Hash(b, archivesAt + i * 16));
            var (filesAt, files) = Blob.Array(b, data + 8);
            for (var i = 0; i < files; i++)
            {
                var at = filesAt + i * 24;
                c.Files.Add((Hash(b, at), BitConverter.ToInt32(b, at + 16), BitConverter.ToInt32(b, at + 20)));
            }
            var (objectsAt, objects) = Blob.Array(b, data + 16);
            for (var i = 0; i < objects; i++)
            {
                var at = objectsAt + i * 64;
                c.Objects.Add(new CatalogObject
                {
                    LocalId = BitConverter.ToInt64(b, at),
                    AssetGuid = Hash(b, at + 16),
                    GenerationType = BitConverter.ToInt16(b, at + 40),
                    FileIndex = BitConverter.ToInt32(b, at + 48),
                    PathId = BitConverter.ToInt64(b, at + 56),
                });
            }
            var (scenesAt, scenes) = Blob.Array(b, data + 24);
            for (var i = 0; i < scenes; i++)
            {
                var at = scenesAt + i * 184;
                var nameLength = BitConverter.ToUInt16(b, at + 52);
                c.Scenes.Add((Hash(b, at + 16), BitConverter.ToInt32(b, at + 48), Encoding.UTF8.GetString(b, at + 54, Math.Min((int)nameLength, 125))));
            }
            var (depsAt, deps) = Blob.Array(b, data + 32);
            for (var i = 0; i < deps; i++)
            {
                var (at, n) = Blob.Array(b, depsAt + i * 8);
                c.Dependencies.Add(Enumerable.Range(0, n).Select(k => BitConverter.ToInt32(b, at + k * 4)).ToArray());
            }
            return c;
        }

        /// <summary>Unity.Entities.Hash128 as its ToString prints it (and as content files are named).</summary>
        public static string Hash(byte[] b, int at)
        {
            var sb = new StringBuilder(32);
            for (var k = 0; k < 16; k++)
            {
                var v = b[at + k];
                sb.Append("0123456789abcdef"[v & 15]).Append("0123456789abcdef"[v >> 4]);
            }
            return sb.ToString();
        }
    }
}
