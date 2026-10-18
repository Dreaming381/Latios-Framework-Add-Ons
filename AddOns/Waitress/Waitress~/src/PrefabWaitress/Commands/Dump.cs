using System.Globalization;
using System.IO;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>Every object's effective values in a fixed format, for checking against the Editor's own view (hidden command).</summary>
    public static class Dump
    {
        public static int Run(Session session, Args args, TextWriter w)
        {
            foreach (var file in args.Positional)
            {
                var asset = session.Asset(file);
                Write(asset, w);
            }
            return 0;
        }

        public static void Write(Asset asset, TextWriter w)
        {
            w.WriteLine("# " + asset.Path);
            if (asset.Problem != null)
            {
                w.WriteLine("! " + asset.Problem);
                return;
            }
            foreach (var o in asset.Objects.Values.OrderBy(o => o.Id))
            {
                if (o.ClassId == 1001 || o.Body == null)
                    continue;
                var go = o.GameObject != null ? Hierarchy.PathOf(o.GameObject) : "";
                w.WriteLine($"&{o.Id} {o.TypeName} {go}{(o.Opaque ? " (opaque)" : "")}");
                foreach (var (path, value) in PropPath.Leaves(o.Body))
                    w.WriteLine("  " + path + " = " + Value(value));
            }
        }

        public static string Value(YNode node)
        {
            if (node.IsRef())
            {
                var r = node.AsRef();
                if (r.IsNull)
                    return "null";
                return r.IsLocal ? "&" + r.FileId.ToString(CultureInfo.InvariantCulture) : r.Guid + ":" + r.FileId.ToString(CultureInfo.InvariantCulture);
            }
            if (node is YScalar s)
                return Escape(s.Value);
            return node.Flat();
        }

        static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r");
    }
}
