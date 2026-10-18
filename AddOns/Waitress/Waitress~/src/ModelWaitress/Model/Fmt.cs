using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ModelWaitress.Model
{
    public static class Fmt
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public static string Num(double d)
        {
            if (Math.Abs(d) < 1e-6)
                return "0";
            var r = Math.Round(d, Math.Abs(d) >= 100 ? 1 : 4);
            return r == 0 ? "0" : r.ToString("0.####", Inv);
        }

        public static string Vec(double[] v) => v == null ? "?" : "(" + string.Join(", ", v.Select(Num)) + ")";

        public static string Count(long n) => n.ToString("N0", Inv);

        public static string Plural(long n, string word) => Count(n) + " " + (n == 1 ? word : word.EndsWith("s", StringComparison.Ordinal) || word.EndsWith("sh", StringComparison.Ordinal) ? word + "es" : word + "s");

        public static bool IsIdentityRotation(double[] r) => r.All(x => Math.Abs(Math.IEEERemainder(x, 360)) < 0.01);
        public static bool IsUnitScale(double[] s) => s.All(x => Math.Abs(x - 1) < 1e-4);

        public static string Seconds(double s) => Num(s) + "s";

        public static string Join(IEnumerable<string> parts, string sep = ", ") => string.Join(sep, parts.Where(p => !string.IsNullOrEmpty(p)));
    }

    /// <summary>The Inspector tab each importer setting appears on, used to group them.</summary>
    public static class Groups
    {
        public static readonly string[] Order = { "Model", "Rig", "Animation", "Materials", "Other" };

        static readonly Dictionary<string, string> s_Group = new Dictionary<string, string>(StringComparer.Ordinal);

        static Groups()
        {
            Add("Model", "globalScale", "useFileScale", "useFileUnits", "bakeAxisConversion", "importBlendShapes", "importBlendShapeDeformPercent",
                "importVisibility", "importCameras", "importLights", "preserveHierarchy", "sortHierarchyByName", "meshCompression", "isReadable",
                "meshOptimizationFlags", "optimizeMeshPolygons", "optimizeMeshVertices", "addCollider", "keepQuads", "weldVertices", "indexFormat",
                "importNormals", "importBlendShapeNormals", "normalCalculationMode", "normalSmoothingSource", "normalSmoothingAngle", "importTangents",
                "swapUVChannels", "generateSecondaryUV", "secondaryUVAngleDistortion", "secondaryUVAreaDistortion", "secondaryUVHardAngle",
                "secondaryUVMarginMethod", "secondaryUVMinLightmapResolution", "secondaryUVMinObjectScale", "secondaryUVPackMargin",
                "strictVertexDataChecks", "generateMeshLods", "meshLodGenerationFlags", "maximumMeshLod");
            Add("Rig", "animationType", "avatarSetup", "sourceAvatar", "autoGenerateAvatarMappingIfUnspecified", "motionNodeName", "skinWeights",
                "maxBonesPerVertex", "minBoneWeight", "optimizeBones", "optimizeGameObjects", "extraExposedTransformPaths", "extraUserProperties");
            Add("Animation", "importAnimation", "importConstraints", "importAnimatedCustomProperties", "bakeIK", "resampleCurves", "animationCompression",
                "animationRotationError", "animationPositionError", "animationScaleError", "removeConstantScaleCurves", "animationWrapMode",
                "generateAnimations", "humanoidOversampling");
            Add("Materials", "materialImportMode", "materialLocation", "materialName", "materialSearch", "searchTexturesGlobally", "useSRGBMaterialColor");
        }

        static void Add(string group, params string[] names)
        {
            foreach (var n in names)
                s_Group[n] = group;
        }

        public static string Of(string setting) => s_Group.TryGetValue(setting, out var g) ? g : "Other";
    }
}

namespace ModelWaitress.Model
{
    /// <summary>Row-major 4x4 transforms built the way Unity composes a Transform (euler applied Z, then X, then Y).</summary>
    public static class Mat
    {
        public static double[] Trs(double[] p, double[] eulerDegrees, double[] s)
        {
            var q = Euler(eulerDegrees);
            double x = q[0], y = q[1], z = q[2], w = q[3];
            var r = new[]
            {
                1 - 2 * (y * y + z * z), 2 * (x * y - z * w), 2 * (x * z + y * w),
                2 * (x * y + z * w), 1 - 2 * (x * x + z * z), 2 * (y * z - x * w),
                2 * (x * z - y * w), 2 * (y * z + x * w), 1 - 2 * (x * x + y * y),
            };
            return new[]
            {
                r[0] * s[0], r[1] * s[1], r[2] * s[2], p[0],
                r[3] * s[0], r[4] * s[1], r[5] * s[2], p[1],
                r[6] * s[0], r[7] * s[1], r[8] * s[2], p[2],
                0, 0, 0, 1,
            };
        }

        static double[] Euler(double[] e)
        {
            double[] Axis(int axis, double degrees)
            {
                var half = degrees * System.Math.PI / 360;
                var q = new double[] { 0, 0, 0, System.Math.Cos(half) };
                q[axis] = System.Math.Sin(half);
                return q;
            }
            return QMul(Axis(1, e[1]), QMul(Axis(0, e[0]), Axis(2, e[2])));
        }

        static double[] QMul(double[] a, double[] b) => new[]
        {
            a[3] * b[0] + a[0] * b[3] + a[1] * b[2] - a[2] * b[1],
            a[3] * b[1] - a[0] * b[2] + a[1] * b[3] + a[2] * b[0],
            a[3] * b[2] + a[0] * b[1] - a[1] * b[0] + a[2] * b[3],
            a[3] * b[3] - a[0] * b[0] - a[1] * b[1] - a[2] * b[2],
        };

        public static double[] Mul(double[] a, double[] b)
        {
            var r = new double[16];
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    for (var k = 0; k < 4; k++)
                        r[i * 4 + j] += a[i * 4 + k] * b[k * 4 + j];
            return r;
        }

        public static double[] Point(double[] m, double[] p) => new[]
        {
            m[0] * p[0] + m[1] * p[1] + m[2] * p[2] + m[3],
            m[4] * p[0] + m[5] * p[1] + m[6] * p[2] + m[7],
            m[8] * p[0] + m[9] * p[1] + m[10] * p[2] + m[11],
        };
    }
}
