using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ModelWaitress.Model
{
    /// <summary>
    /// What an .fbx file says about itself: format version, the application that wrote it, its axis system and unit.
    /// Read straight from the file (binary or ASCII), stopping once GlobalSettings has been read.
    /// </summary>
    public sealed class FbxHeader
    {
        public int Version;
        public bool Binary;
        public string Creator;
        public readonly Dictionary<string, string> Props = new Dictionary<string, string>(StringComparer.Ordinal);

        public string Application => Get("LastSaved|ApplicationName") ?? Get("Original|ApplicationName");
        public string ApplicationVersion => Get("LastSaved|ApplicationVersion") ?? Get("Original|ApplicationVersion");

        public string Get(string key) => Props.TryGetValue(key, out var v) && v.Length > 0 ? v : null;

        int Int(string key, int fallback) => int.TryParse(Get(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;

        static string Axis(int axis, int sign) => (sign < 0 ? "-" : "+") + (axis switch { 0 => "X", 1 => "Y", 2 => "Z", _ => "?" });

        /// <summary>"Y-up, +Z front" style text, or null when the file has no axis settings.</summary>
        public string Axes
        {
            get
            {
                if (Get("UpAxis") == null)
                    return null;
                var up = Axis(Int("UpAxis", 1), Int("UpAxisSign", 1));
                var front = Axis(Int("FrontAxis", 2), Int("FrontAxisSign", 1));
                return $"{up.TrimStart('+')}-up, {front} front";
            }
        }

        public bool YUp => Get("UpAxis") == null || (Int("UpAxis", 1) == 1 && Int("UpAxisSign", 1) > 0);

        /// <summary>The file's centimetres per unit, as FBX stores it (1 = cm, 100 = m).</summary>
        public double? UnitScaleFactor => double.TryParse(Get("UnitScaleFactor"), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;

        public static FbxHeader Read(string path)
        {
            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var magic = new byte[27];
                if (stream.Read(magic, 0, 27) < 27)
                    return null;
                if (Encoding.ASCII.GetString(magic, 0, 18) == "Kaydara FBX Binary")
                {
                    var h = new FbxHeader { Binary = true, Version = BitConverter.ToInt32(magic, 23) };
                    using var reader = new BinaryReader(stream, Encoding.UTF8, true);
                    h.ReadNodes(reader, stream.Length, "");
                    return h;
                }
                stream.Position = 0;
                return ReadAscii(stream);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or EndOfStreamException or ArgumentException)
            {
                return null;
            }
        }

        // Walks node records, descending only into the nodes that hold the header and global settings.
        void ReadNodes(BinaryReader r, long end, string parent)
        {
            var wide = Version >= 7500;
            while (r.BaseStream.Position < end)
            {
                long endOffset = wide ? (long)r.ReadUInt64() : r.ReadUInt32();
                long count = wide ? (long)r.ReadUInt64() : r.ReadUInt32();
                long listLength = wide ? (long)r.ReadUInt64() : r.ReadUInt32();
                int nameLength = r.ReadByte();
                if (endOffset == 0)
                    return;
                var name = Encoding.ASCII.GetString(r.ReadBytes(nameLength));
                var propsStart = r.BaseStream.Position;
                var interesting = name is "FBXHeaderExtension" or "SceneInfo" or "Properties70" or "GlobalSettings" or "P" or "Creator";
                if (!interesting)
                {
                    r.BaseStream.Position = endOffset;
                    continue;
                }
                var values = new List<string>();
                for (var i = 0; i < count; i++)
                    values.Add(ReadProperty(r));
                r.BaseStream.Position = propsStart + listLength;
                if (name == "P" && values.Count >= 5)
                    Props[values[0]] = values[4];
                else if (name == "Creator" && values.Count > 0 && Creator == null)
                    Creator = values[0];
                if (r.BaseStream.Position < endOffset)
                    ReadNodes(r, endOffset, name);
                r.BaseStream.Position = endOffset;
                if (parent == "" && name == "GlobalSettings")
                    return;
            }
        }

        static string ReadProperty(BinaryReader r)
        {
            var type = (char)r.ReadByte();
            switch (type)
            {
                case 'Y': return r.ReadInt16().ToString(CultureInfo.InvariantCulture);
                case 'C': return r.ReadByte() != 0 ? "1" : "0";
                case 'I': return r.ReadInt32().ToString(CultureInfo.InvariantCulture);
                case 'F': return r.ReadSingle().ToString("R", CultureInfo.InvariantCulture);
                case 'D': return r.ReadDouble().ToString("R", CultureInfo.InvariantCulture);
                case 'L': return r.ReadInt64().ToString(CultureInfo.InvariantCulture);
                case 'S':
                case 'R':
                {
                    var length = r.ReadInt32();
                    var bytes = r.ReadBytes(length);
                    return type == 'S' ? Encoding.UTF8.GetString(bytes).Replace("\0\u0001", "::") : "";
                }
                case 'f':
                case 'd':
                case 'l':
                case 'i':
                case 'b':
                {
                    r.ReadInt32();
                    r.ReadInt32();
                    var compressed = r.ReadInt32();
                    r.BaseStream.Position += compressed;
                    return "";
                }
            }
            throw new EndOfStreamException("unknown FBX property type " + (int)type);
        }

        static readonly Regex s_P = new Regex("^\\s*P:\\s*\"([^\"]*)\",\\s*\"[^\"]*\",\\s*\"[^\"]*\",\\s*\"[^\"]*\"\\s*,\\s*(.*?)\\s*$", RegexOptions.Compiled);

        static FbxHeader ReadAscii(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 1 << 16, true);
            var first = reader.ReadLine();
            if (first == null || !first.StartsWith("; FBX", StringComparison.Ordinal))
                return null;
            var h = new FbxHeader();
            var version = Regex.Match(first, "FBX (\\d+)\\.(\\d+)\\.(\\d+)");
            if (version.Success)
                h.Version = int.Parse(version.Groups[1].Value) * 1000 + int.Parse(version.Groups[2].Value) * 100 + int.Parse(version.Groups[3].Value);
            string line;
            var inGlobal = false;
            for (var n = 0; (line = reader.ReadLine()) != null && n < 20000; n++)
            {
                var t = line.Trim();
                if (t.StartsWith("Creator:", StringComparison.Ordinal) && h.Creator == null)
                    h.Creator = t.Substring(8).Trim().Trim('"');
                if (t.StartsWith("GlobalSettings:", StringComparison.Ordinal))
                    inGlobal = true;
                var m = s_P.Match(line);
                if (m.Success)
                    h.Props[m.Groups[1].Value] = m.Groups[2].Value.Trim('"');
                if (inGlobal && t == "}" && line.Length - line.TrimStart().Length == 0)
                    break;
            }
            return h;
        }

        /// <summary>One line for `info`: format, author application, axes and unit.</summary>
        public string Describe()
        {
            var parts = new List<string> { $"FBX {Version} {(Binary ? "binary" : "ASCII")}" };
            var app = Application;
            if (app != null)
                parts.Add("from " + app + (ApplicationVersion != null ? " " + ApplicationVersion : ""));
            else if (Creator != null)
                parts.Add("by " + Creator);
            if (Axes != null)
                parts.Add(Axes);
            if (UnitScaleFactor is double u)
                parts.Add("unit " + (Math.Abs(u - 1) < 1e-6 ? "cm" : Math.Abs(u - 100) < 1e-4 ? "m" : Math.Abs(u - 0.1) < 1e-6 ? "mm" : Math.Abs(u - 2.54) < 1e-4 ? "inch" : Fmt.Num(u) + " cm"));
            return string.Join(", ", parts);
        }
    }
}
