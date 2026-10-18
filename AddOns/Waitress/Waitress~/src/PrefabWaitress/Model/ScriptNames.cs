using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace PrefabWaitress.Model
{
    /// <summary>
    /// Class names for scripts referenced inside a DLL. Unity names such a class by a file id: the first four bytes of
    /// MD4("s\0\0\0" + namespace + name). DLLs in the project are read for their types; references to the UnityEngine.UI
    /// DLL that preceded the uGUI package are matched against the package's sources.
    /// </summary>
    public sealed class ScriptNames
    {
        public const string LegacyUiGuid = "f5f67c52d1564df4a8936ccd202a3bd8";
        readonly AssetIndex m_Index;
        readonly Dictionary<string, Dictionary<int, string>> m_ByGuid = new Dictionary<string, Dictionary<int, string>>(StringComparer.OrdinalIgnoreCase);

        public ScriptNames(AssetIndex index)
        {
            m_Index = index;
        }

        public string Find(string guid, long fileId)
        {
            if (!m_ByGuid.TryGetValue(guid, out var map))
                m_ByGuid[guid] = map = Build(guid);
            return map.TryGetValue((int)fileId, out var name) ? name : null;
        }

        Dictionary<int, string> Build(string guid)
        {
            var map = new Dictionary<int, string>();
            var path = m_Index.PathOf(guid);
            if (path != null && path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using var stream = File.OpenRead(m_Index.FullPath(path));
                    using var pe = new PEReader(stream);
                    var md = pe.GetMetadataReader();
                    foreach (var h in md.TypeDefinitions)
                    {
                        var t = md.GetTypeDefinition(h);
                        if (t.GetDeclaringType().IsNil)
                            map[FileId(md.GetString(t.Namespace), md.GetString(t.Name))] = md.GetString(t.Name);
                    }
                }
                catch (Exception e) when (e is IOException || e is BadImageFormatException || e is InvalidOperationException)
                {
                }
            }
            else if (string.Equals(guid, LegacyUiGuid, StringComparison.OrdinalIgnoreCase))
            {
                var ns = new Regex(@"^\s*namespace\s+([\w.]+)", RegexOptions.Multiline);
                foreach (var file in m_Index.AllPaths.Where(p => p.StartsWith("Packages/com.unity.ugui/Runtime/", StringComparison.OrdinalIgnoreCase) && p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
                {
                    var full = m_Index.FullPath(file);
                    if (!File.Exists(full))
                        continue;
                    var m = ns.Match(File.ReadAllText(full));
                    var name = Path.GetFileNameWithoutExtension(file);
                    map[FileId(m.Success ? m.Groups[1].Value : "", name)] = name;
                }
            }
            return map;
        }

        public static int FileId(string ns, string name)
        {
            var hash = Md4(Encoding.UTF8.GetBytes("s\0\0\0" + ns + name));
            return hash[0] | hash[1] << 8 | hash[2] << 16 | hash[3] << 24;
        }

        static byte[] Md4(byte[] input)
        {
            uint a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;
            var length = input.Length;
            var padded = new byte[(length + 8) / 64 * 64 + 64];
            Array.Copy(input, padded, length);
            padded[length] = 0x80;
            var bits = (ulong)length * 8;
            for (var i = 0; i < 8; i++)
                padded[padded.Length - 8 + i] = (byte)(bits >> (8 * i));
            var x = new uint[16];
            for (var block = 0; block < padded.Length; block += 64)
            {
                for (var i = 0; i < 16; i++)
                    x[i] = BitConverter.ToUInt32(padded, block + i * 4);
                uint aa = a, bb = b, cc = c, dd = d;
                static uint F(uint p, uint q, uint r) => (p & q) | (~p & r);
                static uint G(uint p, uint q, uint r) => (p & q) | (p & r) | (q & r);
                static uint H(uint p, uint q, uint r) => p ^ q ^ r;
                static uint Rot(uint v, int s) => (v << s) | (v >> (32 - s));
                int[] r1 = { 3, 7, 11, 19 }, r2 = { 3, 5, 9, 13 }, r3 = { 3, 9, 11, 15 };
                for (var i = 0; i < 16; i++)
                {
                    var t = Rot(a + F(b, c, d) + x[i], r1[i % 4]);
                    (a, b, c, d) = (d, t, b, c);
                }
                for (var i = 0; i < 16; i++)
                {
                    var k = (i % 4) * 4 + i / 4;
                    var t = Rot(a + G(b, c, d) + x[k] + 0x5a827999, r2[i % 4]);
                    (a, b, c, d) = (d, t, b, c);
                }
                int[] order = { 0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15 };
                for (var i = 0; i < 16; i++)
                {
                    var t = Rot(a + H(b, c, d) + x[order[i]] + 0x6ed9eba1, r3[i % 4]);
                    (a, b, c, d) = (d, t, b, c);
                }
                a += aa;
                b += bb;
                c += cc;
                d += dd;
            }
            var result = new byte[16];
            BitConverter.GetBytes(a).CopyTo(result, 0);
            BitConverter.GetBytes(b).CopyTo(result, 4);
            BitConverter.GetBytes(c).CopyTo(result, 8);
            BitConverter.GetBytes(d).CopyTo(result, 12);
            return result;
        }
    }
}
