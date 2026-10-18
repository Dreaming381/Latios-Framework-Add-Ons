using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClipWaitress.Model;

namespace ClipWaitress.Testing
{
    /// <summary>Checks that need no Unity project: curve evaluation, euler conversion, names and filters.</summary>
    public static class SelfTest
    {
        public static int Run(TextWriter w)
        {
            var tests = new List<(string name, Action body)>
            {
                ("hermite matches a hand-computed midpoint", () =>
                {
                    var c = Curve(new[] { 0.0, 1 }, new[] { 0.0, 1 }, new[] { 0.0, 0 }, new[] { 0.0, 0 });
                    Near(c.Evaluate(0.5), 0.5);
                    Near(c.Evaluate(0.25), 0.15625);
                }),
                ("stepped tangents hold the left key", () =>
                {
                    var c = Curve(new[] { 0.0, 1 }, new[] { 2.0, 5 }, new[] { 0.0, 0 }, new[] { double.PositiveInfinity, 0 });
                    Near(c.Evaluate(0.99), 2);
                }),
                ("weighted tangents of 1/3 equal hermite", () =>
                {
                    var plain = Curve(new[] { 0.0, 2 }, new[] { 1.0, -3 }, new[] { 0.0, 4 }, new[] { 2.0, 0 });
                    var weighted = Curve(new[] { 0.0, 2 }, new[] { 1.0, -3 }, new[] { 0.0, 4 }, new[] { 2.0, 0 });
                    weighted.Mode = new[] { 3, 3 };
                    weighted.InW = new[] { 1.0 / 3, 1.0 / 3 };
                    weighted.OutW = new[] { 1.0 / 3, 1.0 / 3 };
                    foreach (var t in new[] { 0.1, 0.7, 1.3, 1.9 })
                        Near(weighted.Evaluate(t), plain.Evaluate(t), 1e-6);
                }),
                ("evaluation clamps outside the keys", () =>
                {
                    var c = Curve(new[] { 0.5, 1 }, new[] { 3.0, 4 }, new[] { 0.0, 0 }, new[] { 0.0, 0 });
                    Near(c.Evaluate(0), 3);
                    Near(c.Evaluate(9), 4);
                }),
                ("quaternion to euler follows Unity's ZXY order", () =>
                {
                    // Quaternion.Euler(30, 45, 60) in Unity.
                    var e = Bindings.Euler(0.3919038, 0.2005621, 0.3604234, 0.8223632);
                    Near(e[0], 30, 1e-3);
                    Near(e[1], 45, 1e-3);
                    Near(e[2], 60, 1e-3);
                }),
                ("names group transform curves", () =>
                {
                    var c = new Curve { Path = "Hips", Type = "Transform", Prop = "m_LocalPosition.y", Class = "pos" };
                    Same(Bindings.Group(c), "position");
                    Same(Bindings.Name(c), "position.y");
                    var shape = new Curve { Path = "Body", Type = "SkinnedMeshRenderer", Prop = "blendShape.Smile", Class = "prop" };
                    Same(Bindings.Name(shape), "SkinnedMeshRenderer.blendShape.Smile");
                    var color = new Curve { Path = "Body", Type = "MeshRenderer", Prop = "material._Color.r", Class = "prop" };
                    Same(Bindings.Group(color), "MeshRenderer.material._Color");
                }),
                ("filters take path:property or text", () =>
                {
                    var c = new Curve { Path = "Hips/Spine", Type = "Transform", Prop = "localEulerAnglesRaw.x", Class = "euler" };
                    True(Bindings.Matches("Hips/Spine:rotation", c));
                    True(Bindings.Matches("Hips/Spine:rotation.x", c));
                    True(!Bindings.Matches("Hips:rotation", c));
                    True(Bindings.Matches("spine", c));
                }),
                ("filters resolve a unique path tail and a bare object name", () =>
                {
                    var paths = new[] { "Root/Hips", "Root/Hips/Spine", "Root/Hips/Spine/Chest", "Other/Spine2" };
                    Same(Bindings.Resolve("Spine:rotation", paths), "Root/Hips/Spine:rotation");
                    Same(Bindings.Resolve("Chest", paths), "Root/Hips/Spine/Chest:");
                    Same(Bindings.Resolve("pine", paths), "pine");
                    var threw = false;
                    try
                    {
                        Bindings.Resolve("Spine:rotation", paths.Append("Other/Spine").ToArray());
                    }
                    catch (WaitressException)
                    {
                        threw = true;
                    }
                    True(threw);
                }),
                ("= on a list key tests membership", () =>
                {
                    True(Commands.Query.Test("Footstep,Jump", "=", "footstep", "event"));
                    True(!Commands.Query.Test("Footstep,Jump", "=", "Foot", "event"));
                    True(Commands.Query.Test("Footstep,Jump", "!=", "Land", "event"));
                    True(Commands.Query.Test("Footstep,Jump", "=", "2", "event"));
                }),
                ("rotation orders compose with the first letter turning first", () =>
                {
                    var zxy = Bindings.Quaternion(30, 45, 60, 4);
                    Near(zxy[0], 0.3919038, 1e-6);
                    Near(zxy[3], 0.8223632, 1e-6);
                    var x = Bindings.Quaternion(30, 0, 0, 0);
                    var e = Bindings.Euler(x[0], x[1], x[2], x[3]);
                    Near(e[0], 30, 1e-6);
                }),
            };
            var failed = 0;
            foreach (var (name, body) in tests)
            {
                try
                {
                    body();
                    w.WriteLine("pass  " + name);
                }
                catch (Exception e)
                {
                    failed++;
                    w.WriteLine("FAIL  " + name + ": " + e.Message);
                }
            }
            w.WriteLine($"{tests.Count - failed} of {tests.Count} passed");
            return failed == 0 ? 0 : 1;
        }

        static Curve Curve(double[] t, double[] v, double[] i, double[] o) => new Curve { Path = "", Type = "T", Prop = "p", Class = "prop", T = t, V = v, In = i, Out = o };

        static void Near(double a, double b, double tolerance = 1e-9)
        {
            if (Math.Abs(a - b) > tolerance)
                throw new Exception($"{a} != {b}");
        }

        static void Same(string a, string b)
        {
            if (a != b)
                throw new Exception($"'{a}' != '{b}'");
        }

        static void True(bool b)
        {
            if (!b)
                throw new Exception("expected true");
        }
    }
}
