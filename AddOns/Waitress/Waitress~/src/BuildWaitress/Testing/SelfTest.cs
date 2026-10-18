using System;
using System.IO;

namespace BuildWaitress.Testing
{
    public static class SelfTest
    {
        public static int Run(TextWriter w)
        {
            var failed = 0;
            void Test(string name, Action body)
            {
                try
                {
                    body();
                    w.WriteLine("ok   " + name);
                }
                catch (Exception e)
                {
                    failed++;
                    w.WriteLine($"FAIL {name}: {e.Message}");
                }
            }
            void Eq<T>(T expected, T actual, string what)
            {
                if (!Equals(expected, actual))
                    throw new Exception($"{what}: expected {expected}, got {actual}");
            }

            Test("lz4 literals and overlapping match", () =>
            {
                // "abc" literal, then a 9-byte match at offset 3, then literal "!".
                var block = new byte[] { 0x35, (byte)'a', (byte)'b', (byte)'c', 3, 0, 0x10, (byte)'!' };
                var raw = Serialized.Lz4.Decode(block, 0, block.Length, 13);
                Eq("abcabcabcabc!", System.Text.Encoding.ASCII.GetString(raw), "decoded");
            });
            Test("common strings", () =>
            {
                Eq("int", Serialized.CommonStrings.At(222), "offset 222");
                Eq("m_Name", Serialized.CommonStrings.At(427), "offset 427");
            });
            Test("class ids", () =>
            {
                Eq("Texture2D", Model.ClassIds.Name(28), "28");
                Eq("Mesh", Model.ClassIds.Name(43), "43");
                Eq("BuildReport", Model.ClassIds.Name(1125), "1125");
            });
            Test("output file categories", () =>
            {
                Eq("subscenes", Model.Analysis.Category(new Model.OutputFile { Rel = "X_Data/StreamingAssets/EntityScenes/a.0.entities", Role = "entities" }), "entities");
                Eq("subscene content", Model.Analysis.Category(new Model.OutputFile { Rel = "X_Data/StreamingAssets/ContentArchives/0123", Role = "" }), "archive");
                Eq("managed code", Model.Analysis.Category(new Model.OutputFile { Rel = "X_Data/Managed/Foo.dll", Role = "ManagedLibrary" }), "dll");
                Eq("scenes and assets", Model.Analysis.Category(new Model.OutputFile { Rel = "X_Data/sharedassets0.assets.resS", Role = "resS" }), "resS");
                Eq("debug symbols", Model.Analysis.Category(new Model.OutputFile { Rel = "X_BurstDebugInformation_DoNotShip/lib.pdb", Role = "pdb" }), "pdb");
            });
            w.WriteLine(failed == 0 ? "all passed" : $"{failed} failed");
            return failed == 0 ? 0 : 1;
        }
    }
}
