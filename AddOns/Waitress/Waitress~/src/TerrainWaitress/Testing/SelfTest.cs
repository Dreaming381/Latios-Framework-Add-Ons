using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrainWaitress.Model;

namespace TerrainWaitress.Testing
{
    /// <summary>Offline checks of parsing and editing on a synthetic terrain; `terrainwaitress selftest`.</summary>
    public static class SelfTest
    {
        static TerrainRecord Flat(int res = 5, float size = 40, float height = 10)
        {
            var r = new TerrainRecord
            {
                Path = "Assets/T.asset", Guid = "0", Size = new double[] { size, height, size }, HeightRes = res, HolesRes = res - 1, AlphaRes = 4, DetailRes = 4,
                DetailPatch = 8, DetailScatter = "CoverageMode", AlphaLayers = 2,
            };
            r.Heights = new ushort[res * res];
            r.Holes = Enumerable.Repeat((byte)1, (res - 1) * (res - 1)).ToArray();
            r.Alpha = new byte[16 * 2];
            for (var i = 0; i < 16; i++)
                r.Alpha[i] = 255;
            r.Layers.Add(new Prototype { Name = "A", Asset = "Assets/A.terrainlayer" });
            r.Layers.Add(new Prototype { Name = "B", Asset = "Assets/B.terrainlayer" });
            r.TreePrototypes.Add(new Prototype { Name = "Tree" });
            r.Details = new ushort[0];
            r.Trees = new[] { new Tree { X = 0.5f, Z = 0.5f, Y = 0, Prototype = 0 } };
            return r;
        }

        public static int Run(TextWriter w)
        {
            var failures = new List<string>();
            void Test(string name, Action body)
            {
                try
                {
                    body();
                }
                catch (Exception e)
                {
                    failures.Add($"{name}: {e.Message}");
                }
            }
            void Eq(object want, object got, string what)
            {
                if (!Equals(want, got))
                    throw new Exception($"{what}: want {want}, got {got}");
            }

            Test("op parse", () =>
            {
                var op = Op.Parse("layer[Grass][0,0..10,10+5]+=0.5");
                Eq("layer", op.Name, "name");
                Eq(2, op.Args.Count, "args");
                Eq("0,0..10,10+5", op.Args[1], "area");
                Eq("+=", op.Kind, "kind");
                Eq("0.5", op.Value, "value");
                var p = Op.Parse("layer[Grass].tileSize=8,8");
                Eq("tileSize", p.Prop, "prop");
                Eq("-=", Op.Parse("trees-=all").Kind, "minus");
                Eq("=", Op.Parse("height[-5,-5..5,5]=-1").Kind, "negative value");
            });
            Test("region", () =>
            {
                var t = new Target { Data = Flat() };
                var r = Region.Parse("10,10..20,20+5", t);
                Eq(5.0, r.Falloff, "falloff");
                Eq(1.0, r.Weight(15, 15), "inside");
                Eq(0.0, r.Weight(30, 15), "beyond");
                Eq(true, r.Weight(22.5, 15) is > 0 and < 1, "eased");
                var c = Region.Parse("50%,50%~5", t);
                Eq(20.0, c.CX, "percent");
            });
            Test("flatten with snapped tree", () =>
            {
                var t = new Target { Data = Flat() };
                var e = new Editing(t, _ => true);
                e.Apply(Op.Parse("height[all]=5"));
                Eq(16383, (int)e.After.Heights[12], "half of 32766");
                Eq(5.0, Math.Round(e.After.HeightAt(0.3f, 0.7f), 3), "interpolated");
                Eq(0.5f, (float)Math.Round(e.After.Trees[0].Y, 4), "tree snapped");
                Eq(0, (int)e.Before.Heights[12], "before untouched");
            });
            Test("paint renormalizes", () =>
            {
                var t = new Target { Data = Flat() };
                var e = new Editing(t, _ => true);
                e.Apply(Op.Parse("layer[B][all]=0.25"));
                Eq(255, e.After.Weight(0, 1, 1) + e.After.Weight(1, 1, 1), "sum");
                Eq(64, (int)e.After.Weight(1, 1, 1), "B");
            });
            Test("layer removal keeps sums", () =>
            {
                var t = new Target { Data = Flat() };
                var e = new Editing(t, _ => true);
                e.Apply(Op.Parse("layer[B][all]=0.25"));
                e.Apply(Op.Parse("layers-=A"));
                Eq(1, e.After.AlphaLayers, "layers");
                Eq(255, (int)e.After.Weight(0, 2, 2), "B takes all");
            });
            Test("resample keeps a ramp", () =>
            {
                var r = Flat(33);
                for (var z = 0; z < 33; z++)
                for (var x = 0; x < 33; x++)
                    r.Heights[z * 33 + x] = (ushort)(x * 1000);
                var e = new Editing(new Target { Data = r }, _ => true);
                e.Apply(Op.Parse("heightmapResolution=65"));
                Eq(65, e.After.HeightRes, "res");
                Eq(500, (int)e.After.Heights[1], "midpoint");
                Eq(64, e.After.HolesRes, "holes follow");
            });
            Test("errors name the setting", () =>
            {
                var e = new Editing(new Target { Data = Flat() }, _ => true);
                try
                {
                    e.Apply(Op.Parse("heightmapResolution=100"));
                    throw new Exception("accepted 100");
                }
                catch (WaitressException x) when (x.Message.Contains("power of two plus one"))
                {
                }
            });
            Test("trees scatter avoids holes", () =>
            {
                var r = Flat();
                for (var i = 0; i < 8; i++)
                    r.Holes[i] = 0;
                var e = new Editing(new Target { Data = r }, _ => true);
                e.Apply(Op.Parse("trees[Tree]+=all:50:3"));
                Eq(51, e.After.Trees.Length, "count");
                Eq(false, e.After.Trees.Any(x => e.After.HoleAt(x.X, x.Z)), "none in holes");
            });

            foreach (var f in failures)
                w.WriteLine("FAIL " + f);
            w.WriteLine($"{8 - failures.Count}/8 self-tests passed");
            return failures.Count == 0 ? 0 : 1;
        }
    }
}
