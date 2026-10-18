using System;
using System.Collections.Generic;
using System.Linq;
using PrefabWaitress.Model;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Commands
{
    /// <summary>One printed property: a field, or a whole small struct or list, with the overrides that set any part of it.</summary>
    public sealed class FieldLine
    {
        public string Path;
        public YNode Node;
        public List<Mod> Setters = new List<Mod>();
    }

    public static class Fields
    {
        /// <summary>The printable lines of an object's values, noise fields left out.</summary>
        public static List<FieldLine> Lines(Obj o)
        {
            var lines = new List<FieldLine>();
            if (o.Body == null)
                return lines;
            for (var i = 0; i < o.Body.Count; i++)
            {
                if (Describe.Noise.Contains(o.Body.Keys[i]))
                    continue;
                Collect(o.Body.Keys[i], o.Body.Values[i], lines);
            }
            if (o.SetBy.Count > 0)
            {
                foreach (var line in lines)
                    foreach (var (path, mod) in o.SetBy)
                        if (Covers(line.Path, path))
                            line.Setters.Add(mod);
                // Overrides on properties the values don't have (e.g. on an object from a model) still get a line.
                foreach (var (path, mod) in o.SetBy)
                    if (!lines.Any(l => Covers(l.Path, path)) && !Describe.Noise.Contains(path.Split('.')[0]))
                        lines.Add(new FieldLine { Path = path, Node = mod.NewValue(null), Setters = { mod } });
            }
            return lines;
        }

        public static bool Covers(string line, string path) =>
            path == line || path.StartsWith(line + ".", StringComparison.Ordinal);

        static void Collect(string path, YNode v, List<FieldLine> lines)
        {
            switch (v)
            {
                case YMap m when !m.Flow && m.Count > 0:
                    for (var i = 0; i < m.Count; i++)
                        if (m.Keys[i] != "serializedVersion")
                            Collect(path + "." + m.Keys[i], m.Values[i], lines);
                    return;
                case YSeq s when s.Items.Any(i => i is YMap im && !im.Flow || i is YSeq):
                    lines.Add(new FieldLine { Path = path + ".Array.size", Node = new YScalar(s.Items.Count.ToString()) });
                    for (var i = 0; i < s.Items.Count; i++)
                        Collect($"{path}.Array.data[{i}]", s.Items[i], lines);
                    return;
                default:
                    lines.Add(new FieldLine { Path = path, Node = v });
                    return;
            }
        }

        /// <summary>The files that set a line, nearest first.</summary>
        public static string SetterNames(FieldLine line) =>
            string.Join(", ", line.Setters.Select(m => m.Layer.Owner.FileName).Distinct().Reverse());

        /// <summary>A value as it stands in the source an instanced object comes from, or null.</summary>
        public static YNode SourceValue(Obj o, string path)
        {
            if (o.Source?.Body == null)
                return null;
            return PropPath.TryResolve(o.Source.Body, path, out var slot, out _) ? slot.Get() : null;
        }
    }
}
