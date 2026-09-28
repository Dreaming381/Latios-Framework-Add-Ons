using Latios.Calci;
using Latios.Kinemation;
using Latios.Transforms.Abstract;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Latios.Ribbons.Systems
{
    /// <summary>
    /// Builds ribbon meshes the same way LineRenderer does. The ribbon is a strip of left/right vertex pairs, with left at
    /// V = 1 and right at V = 0. A sharp corner is one mitered pair. A rounded corner is a run of pairs that share the
    /// inside miter point and sweep the outside around it. Caps are fans on the ends.
    /// </summary>
    internal static class RibbonMeshBuilder
    {
        // Keeps sharp turns from spiking. 4 is SVG's default miter limit.
        const float kMaxMiterScale = 4f;

        /// <summary>
        /// One vertex of a ribbon mesh, laid out the way VertexLayout() describes.
        /// </summary>
        public struct Vertex
        {
            public float3 position;
            public uint   normal;  // SNorm8 x4
            public uint   color;  // UNorm8 x4, like the Color32 colors LineRenderer uses
            public float2 uv;
        }

        public static UniqueMeshVertexRawLayout VertexLayout()
        {
            var layout = new UniqueMeshVertexRawLayout();
            layout.Add(new UniqueMeshVertexRawLayout.Descriptor { attribute = VertexAttribute.Position, format = VertexAttributeFormat.Float32, dimension = 3 });
            layout.Add(new UniqueMeshVertexRawLayout.Descriptor { attribute = VertexAttribute.Normal, format = VertexAttributeFormat.SNorm8, dimension = 4 });
            layout.Add(new UniqueMeshVertexRawLayout.Descriptor { attribute = VertexAttribute.Color, format = VertexAttributeFormat.UNorm8, dimension = 4 });
            layout.Add(new UniqueMeshVertexRawLayout.Descriptor { attribute = VertexAttribute.TexCoord0, format = VertexAttributeFormat.Float32, dimension = 2 });
            return layout;
        }

        public struct MeshOutput
        {
            public DynamicBuffer<UniqueMeshVertexRawData> vertices;
            public DynamicBuffer<UniqueMeshIndex>         indices;

            public void Clear()
            {
                vertices.Clear();
                indices.Clear();
            }
        }

        struct Builder
        {
            public NativeList<Vertex> vertices;
            public NativeList<int>    indices;
            // Each strip pair is packed as (left, right) vertex indices.
            public NativeList<int2> pairs;

            public int AddVertex(float3 p, float3 normal, float2 uv, float4 color) => AddVertex(p, PackNormal(normal), uv, PackColor(color));

            public int AddVertex(float3 p, uint normal, float2 uv, uint color)
            {
                vertices.Add(new Vertex { position = p, normal = normal, color = color, uv = uv });
                return vertices.Length - 1;
            }

            static uint PackNormal(float3 normal)
            {
                var q = (int3)math.round(math.clamp(normal, -1f, 1f) * 127f);
                return (uint)(q.x & 0xff) | ((uint)(q.y & 0xff) << 8) | ((uint)(q.z & 0xff) << 16);
            }

            static uint PackColor(float4 color)
            {
                var q = (uint4)math.round(math.saturate(color) * 255f);
                return q.x | (q.y << 8) | (q.z << 16) | (q.w << 24);
            }

            public void AddTriangle(int a, int b, int c)
            {
                if (a == b || b == c || a == c)
                    return;
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
            }
        }

        struct PointSample
        {
            public float3 position;
            public float  width;
            public float4 color;
            public float  u;
        }

        /// <summary>
        /// Builds a ribbon mesh from local-space points.
        /// </summary>
        /// <param name="positions">Local-space points, at least 2. The width and color curves start at the first point.</param>
        /// <param name="widthScales">A width multiplier for each point, or an empty array to use 1 for all points</param>
        /// <param name="linearColorSpace">If true, gradient colors are converted to linear, like LineRenderer does in linear color space projects</param>
        /// <param name="loop">If true, the last point connects back to the first</param>
        /// <param name="numCornerVertices">How many steps round the outside of each corner. 0 leaves corners sharp.</param>
        /// <param name="numCapVertices">How many vertices round each end, ignored for loops</param>
        /// <param name="facing">Which way the ribbon faces</param>
        public static void Build(NativeArray<float3>                positions,
                                 NativeArray<float>                 widthScales,
                                 DynamicBuffer<RibbonWidthKeyframe> widthKeyframes,
                                 float widthMultiplier,
                                 DynamicBuffer<RibbonColorKey>      colorKeys,
                                 bool linearColorSpace,
                                 bool loop,
                                 RibbonTextureMode textureMode,
                                 int numCornerVertices,
                                 int numCapVertices,
                                 in Facing facing,
                                 ref MeshOutput output,
                                 out float3 boundsMin,
                                 out float3 boundsMax)
        {
            int n                 = positions.Length;
            int segmentCount      = loop ? n : n - 1;
            numCornerVertices     = math.max(numCornerVertices, 0);
            numCapVertices        = loop ? 0 : math.max(numCapVertices, 0);
            int pairCountEstimate = (n + 1) * (numCornerVertices + 1);
            int vertexEstimate    = pairCountEstimate * 2 + 2 * (numCapVertices + 3);

            var builder = new Builder
            {
                vertices = new NativeList<Vertex>(vertexEstimate, Allocator.Temp),
                indices  = new NativeList<int>(pairCountEstimate * 6 + 6 * (numCapVertices + 2), Allocator.Temp),
                pairs    = new NativeList<int2>(pairCountEstimate, Allocator.Temp),
            };

            var distances = new NativeArray<float>(n, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            distances[0]  = 0f;
            for (int i = 1; i < n; i++)
                distances[i] = distances[i - 1] + math.distance(positions[i], positions[i - 1]);
            float totalLength = distances[n - 1] + (loop ? math.distance(positions[n - 1], positions[0]) : 0f);
            float invLength   = totalLength > 0f ? 1f / totalLength : 0f;

            // Loops sample one extra time at t = 1 for the end of the closing segment.
            int sampleCount = loop ? n + 1 : n;
            var times       = new NativeArray<float>(sampleCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < n; i++)
                times[i] = distances[i] * invLength;
            if (loop)
                times[n] = 1f;
            var widths = new NativeArray<float>(sampleCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            EvaluateWidths(widthKeyframes, widthMultiplier, times, widths);

            PointSample SamplePoint(int sampleIndex, float u)
            {
                int   pointIndex = sampleIndex % n;
                float width      = widthScales.Length > 0 ? widths[sampleIndex] * widthScales[pointIndex] : widths[sampleIndex];
                var   color      = EvaluateGradient(colorKeys, times[sampleIndex], linearColorSpace);
                return new PointSample { position = positions[pointIndex], width = width, color = color, u = u };
            }

            float U(int i) => textureMode switch
            {
                RibbonTextureMode.Tile => distances[i],
                RibbonTextureMode.DistributeEvenly => (float)i / segmentCount,
                _ => distances[i] * invLength,
            };

            for (int i = 0; i < n; i++)
            {
                var    sample = SamplePoint(i, U(i));
                float3 dirIn  = i > 0 ? positions[i] - positions[i - 1] : (loop ? positions[0] - positions[n - 1] : float3.zero);
                float3 dirOut = i < n - 1 ? positions[i + 1] - positions[i] : (loop ? positions[0] - positions[n - 1] : float3.zero);
                // A loop's first point only gets the end of its rounded corner. The whole corner comes at the end of the loop.
                var part = loop && i == 0 ? CornerPart.OutgoingOnly : CornerPart.All;
                AddPoint(ref builder, in sample, dirIn, dirOut, in facing, numCornerVertices, part);
            }
            if (loop)
            {
                float u    = textureMode == RibbonTextureMode.Tile ? totalLength : 1f;
                var   wrap = SamplePoint(n, u);
                AddPoint(ref builder, in wrap, positions[0] - positions[n - 1], positions[1] - positions[0], in facing, numCornerVertices,
                         CornerPart.All);
            }

            AddStripTriangles(ref builder);

            if (numCapVertices > 0)
            {
                AddCap(ref builder, in facing, builder.pairs[0], positions[0], positions[0] - positions[1], numCapVertices, U(0));
                AddCap(ref builder, in facing, builder.pairs[builder.pairs.Length - 1], positions[n - 1], positions[n - 1] - positions[n - 2], numCapVertices, U(n - 1));
            }

            output.vertices.ResizeUninitialized(builder.vertices.Length * UnsafeUtility.SizeOf<Vertex>());
            output.indices.ResizeUninitialized(builder.indices.Length);
            output.vertices.Reinterpret<byte>().AsNativeArray().Reinterpret<Vertex>(1).CopyFrom(builder.vertices.AsArray());
            output.indices.Reinterpret<int>().AsNativeArray().CopyFrom(builder.indices.AsArray());

            boundsMin = builder.vertices[0].position;
            boundsMax = boundsMin;
            for (int i = 1; i < builder.vertices.Length; i++)
            {
                boundsMin = math.min(boundsMin, builder.vertices[i].position);
                boundsMax = math.max(boundsMax, builder.vertices[i].position);
            }
        }

        static void EvaluateWidths(DynamicBuffer<RibbonWidthKeyframe> keyframes, float multiplier, NativeArray<float> times, NativeArray<float> results)
        {
            if (keyframes.Length < 2)
            {
                float constant = (keyframes.Length == 0 ? 1f : keyframes[0].keyframe.value) * multiplier;
                for (int i = 0; i < results.Length; i++)
                    results[i] = constant;
                return;
            }

            var segments = new NativeList<KeyedCurve>(keyframes.Length - 1, Allocator.Temp);
            for (int i = 0; i + 1 < keyframes.Length; i++)
            {
                var left  = keyframes[i].keyframe;
                var right = keyframes[i + 1].keyframe;
                if (right.time <= left.time)
                    continue;
                // An infinite tangent is a step, which holds the left value until the right key.
                if (!math.isfinite(left.outTangentSlope) || !math.isfinite(right.inTangentSlope))
                {
                    right                 = left;
                    right.time            = keyframes[i + 1].keyframe.time;
                    left.outTangentSlope  = 0f;
                    right.inTangentSlope  = 0f;
                    left.outTangentWeight = Keyframe.kHermite;
                    right.inTangentWeight = Keyframe.kHermite;
                }
                segments.Add(KeyedCurve.FromKeyframes(left, right));
            }

            if (segments.IsEmpty)
            {
                for (int i = 0; i < results.Length; i++)
                    results[i] = keyframes[0].keyframe.value * multiplier;
                return;
            }

            BezierMath.Evaluate(segments.AsArray().AsReadOnlySpan(), times.AsReadOnlySpan(), results.AsSpan());
            for (int i = 0; i < results.Length; i++)
                results[i] *= multiplier;
        }

        static float4 EvaluateGradient(DynamicBuffer<RibbonColorKey> keys, float time, bool linearColorSpace)
        {
            float4 color;
            if (keys.Length == 0)
                color = 1f;
            else if (keys.Length == 1 || time <= keys[0].time)
                color = (float4)keys[0].colorRaw;
            else if (time >= keys[keys.Length - 1].time)
                color = (float4)keys[keys.Length - 1].colorRaw;
            else
            {
                int right = 1;
                while (keys[right].time < time)
                    right++;
                var rightKey = keys[right];
                var leftKey  = keys[right - 1];
                if (rightKey.isFixed)
                    color = (float4)rightKey.colorRaw;
                else
                {
                    float span = rightKey.time - leftKey.time;
                    color      = math.lerp((float4)leftKey.colorRaw, (float4)rightKey.colorRaw, span > 0f ? (time - leftKey.time) / span : 1f);
                }
            }

            if (linearColorSpace)
                color.xyz = math.select(math.pow((color.xyz + 0.055f) / 1.055f, 2.4f), color.xyz / 12.92f, color.xyz <= 0.04045f);
            return color;
        }

        public static bool IsLinearColorSpace() => UnityEngine.QualitySettings.activeColorSpace == UnityEngine.ColorSpace.Linear;

        public static void GetMainCamera(out float3 position, out float3 forward)
        {
            var camera = UnityEngine.Camera.main;
            if (camera == null)
            {
                position = float3.zero;
                forward  = new float3(0f, 0f, 1f);
                return;
            }
            position = camera.transform.position;
            forward  = camera.transform.forward;
        }

        /// <summary>
        /// The direction a ribbon faces. Directions along the ribbon are flattened into the plane the viewer sees, and the
        /// sides are perpendicular to those flattened directions within that plane.
        /// </summary>
        public struct Facing
        {
            // The front of the ribbon faces opposite this.
            public float3 reference;
            public float3 cameraPosition;
            public bool   perspective;

            public static Facing TransformZ => new Facing { reference = new float3(0f, 0f, 1f) };

            public static Facing View(in WorldTransformReadOnlyAspect worldTransform, float3 cameraPosition, float3 cameraForward)
            {
                return new Facing
                {
                    // Sides must stay perpendicular to the camera forward in world space, which is how normals transform.
                    reference      = math.normalizesafe(worldTransform.InverseTransformNormalUnnormalized(cameraForward), new float3(0f, 0f, 1f)),
                    cameraPosition = worldTransform.InverseTransformPoint(cameraPosition),
                    perspective    = true,
                };
            }

            // For View, this is the direction on screen, including perspective, written as a vector in the camera plane.
            public float3 Flatten(float3 dir, float3 p)
            {
                if (!perspective)
                    return dir - reference * math.dot(dir, reference);
                float3 q = p - cameraPosition;
                return dir * math.dot(q, reference) - q * math.dot(dir, reference);
            }

            public float3 Side(float3 flatDir)
            {
                float3 side = math.cross(reference, flatDir);
                if (math.lengthsq(side) < 1e-12f)
                {
                    side = math.cross(new float3(1f, 0f, 0f), flatDir);
                    if (math.lengthsq(side) < 1e-12f)
                        side = new float3(0f, 1f, 0f);
                }
                return math.normalize(side);
            }
        }

        enum CornerPart : byte
        {
            All,
            OutgoingOnly
        }

        static void AddPoint(ref Builder builder,
                             in PointSample sample,
                             float3 dirIn,
                             float3 dirOut,
                             in Facing facing,
                             int numCornerVertices,
                             CornerPart part)
        {
            // Corners are mitered as the viewer sees them, so everything below works with flattened directions.
            var p  = sample.position;
            dirIn  = math.normalizesafe(facing.Flatten(dirIn, p));
            dirOut = math.normalizesafe(facing.Flatten(dirOut, p));
            if (math.all(dirIn == 0f))
                dirIn = dirOut;
            if (math.all(dirOut == 0f))
                dirOut = dirIn;

            float3 dir    = math.normalizesafe(dirIn + dirOut, dirIn);
            float3 side   = facing.Side(dir);
            float3 normal = math.cross(side, dir);

            // Scaling by 1 / cos(halfAngle) keeps the segments at full width through the corner.
            float  cosHalfAngle = math.dot(dirIn, dir);
            float3 miterOffset  = side * (0.5f * sample.width / math.max(cosHalfAngle, 1f / kMaxMiterScale));

            bool isCorner = math.dot(dirIn, dirOut) < 0.99999f;
            if (numCornerVertices == 0 || !isCorner)
            {
                int left  = builder.AddVertex(p - miterOffset, normal, new float2(sample.u, 1f), sample.color);
                int right = builder.AddVertex(p + miterOffset, normal, new float2(sample.u, 0f), sample.color);
                builder.pairs.Add(new int2(left, right));
                return;
            }

            // The inside of a rounded corner is the miter point. The outside is an arc around it, tangent to the outside
            // edge of both segments.
            float3 sideIn        = facing.Side(dirIn);
            float3 sideOut       = facing.Side(dirOut);
            bool   insideIsRight = math.dot(dirOut, sideIn) > 0f;
            float  insideSign    = insideIsRight ? 1f : -1f;
            float3 pivot         = p + miterOffset * insideSign;
            float3 armIn         = -sideIn * (insideSign * sample.width);
            float3 armOut        = -sideOut * (insideSign * sample.width);
            float3 arcAxis       = math.normalizesafe(math.cross(armIn, armOut), normal);
            float  arcAngle      = math.atan2(math.length(math.cross(armIn, armOut)), math.dot(armIn, armOut));

            int pivotIdx = builder.AddVertex(pivot, normal, new float2(sample.u, insideIsRight ? 0f : 1f), sample.color);
            int first    = part == CornerPart.OutgoingOnly ? numCornerVertices : 0;
            for (int k = first; k <= numCornerVertices; k++)
            {
                math.sincos(arcAngle * k / numCornerVertices, out var s, out var c);
                float3 arm      = armIn * c + math.cross(arcAxis, armIn) * s;
                int    outerIdx = builder.AddVertex(pivot + arm, normal, new float2(sample.u, insideIsRight ? 1f : 0f), sample.color);
                builder.pairs.Add(insideIsRight ? new int2(outerIdx, pivotIdx) : new int2(pivotIdx, outerIdx));
            }
        }

        static void AddStripTriangles(ref Builder builder)
        {
            for (int k = 0; k + 1 < builder.pairs.Length; k++)
            {
                var a = builder.pairs[k];
                var b = builder.pairs[k + 1];
                builder.AddTriangle(a.x, a.y, b.x);
                builder.AddTriangle(a.y, b.y, b.x);
            }
        }

        // Fans around the back of an end, from its right vertex to its left vertex.
        static void AddCap(ref Builder builder, in Facing facing, int2 endPair, float3 center, float3 outward, int numCapVertices, float u)
        {
            float3 rightPos  = builder.vertices[endPair.y].position;
            float3 leftPos   = builder.vertices[endPair.x].position;
            uint   normal    = builder.vertices[endPair.x].normal;
            uint   color     = builder.vertices[endPair.x].color;
            float3 rightArm  = rightPos - center;
            float3 backArm   = math.normalizesafe(facing.Flatten(outward, center)) * math.length(rightArm);
            int    centerIdx = builder.AddVertex(center, normal, new float2(u, 0.5f), color);
            int    prevIdx   = builder.AddVertex(rightPos, normal, new float2(u, 0f), color);

            // Match the strip's winding.
            var   strip = builder.pairs[0];
            var   p0    = builder.vertices[strip.x].position;
            float flip  = math.dot(math.cross(builder.vertices[strip.y].position - p0, builder.vertices[builder.pairs[1].x].position - p0),
                                   math.cross(rightArm, backArm));
            for (int k = 1; k <= numCapVertices + 1; k++)
            {
                float t = (float)k / (numCapVertices + 1);
                math.sincos(math.PI * t, out var s, out var c);
                float3 pos = k == numCapVertices + 1 ? leftPos : center + rightArm * c + backArm * s;
                int    idx = builder.AddVertex(pos, normal, new float2(u, t), color);
                if (flip < 0f)
                    builder.AddTriangle(centerIdx, idx, prevIdx);
                else
                    builder.AddTriangle(centerIdx, prevIdx, idx);
                prevIdx = idx;
            }
        }
    }
}

