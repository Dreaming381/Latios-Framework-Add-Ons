using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Ribbons
{
    /// <summary>
    /// Which way a ribbon faces. Mirrors UnityEngine.LineAlignment.
    /// </summary>
    public enum RibbonAlignment : byte
    {
        /// <summary>
        /// The ribbon faces along the entity's local Z axis.
        /// </summary>
        TransformZ = 0,
        /// <summary>
        /// The ribbon faces Camera.main. Other cameras see the same mesh.
        /// </summary>
        View = 1,
    }

    /// <summary>
    /// How the U texture coordinate runs along the ribbon. Mirrors part of UnityEngine.LineTextureMode.
    /// </summary>
    public enum RibbonTextureMode : byte
    {
        /// <summary>
        /// U goes from 0 to 1 across the whole ribbon.
        /// </summary>
        Stretch = 0,
        /// <summary>
        /// U is the distance along the ribbon, so the texture repeats once per unit of length.
        /// </summary>
        Tile = 1,
        /// <summary>
        /// U goes from 0 to 1 across the whole ribbon, but every segment gets an equal share regardless of its length.
        /// </summary>
        DistributeEvenly = 2,
    }

    #region Line
    /// <summary>
    /// Settings for an entity baked from a LineRenderer. Enable this after changing the settings, the RibbonPoint buffer,
    /// the width curve, or the gradient, and Ribbons will rebuild the mesh and disable this again.
    /// </summary>
    public struct RibbonLineConfig : IComponentData, IEnableableComponent
    {
        public RibbonAlignment   alignment;
        public RibbonTextureMode textureMode;
        /// <summary>
        /// Multiplies the width curve.
        /// </summary>
        public float widthMultiplier;
        /// <summary>
        /// If true, the last point connects back to the first.
        /// </summary>
        public bool loop;
        /// <summary>
        /// Vertices added to round the outside of each corner. 0 leaves corners sharp.
        /// </summary>
        public int numCornerVertices;
        /// <summary>
        /// Vertices added to round each end. 0 leaves ends flat. Ignored for loops.
        /// </summary>
        public int numCapVertices;
    }

    /// <summary>
    /// The points of a line in the entity's local space. A line needs at least 2 points to render.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct RibbonPoint : IBufferElementData
    {
        public float3 position;
    }

    /// <summary>
    /// Added to lines using RibbonAlignment.View. These lines rebuild whenever they or the camera move.
    /// </summary>
    public struct RibbonViewAlignedTag : IComponentData { }
    #endregion

    #region Trail
    /// <summary>
    /// Settings for an entity baked from a TrailRenderer. The trail emits points as the entity moves and removes them as they expire.
    /// </summary>
    public struct RibbonTrailConfig : IComponentData
    {
        public RibbonAlignment   alignment;
        public RibbonTextureMode textureMode;
        /// <summary>
        /// Multiplies the width curve.
        /// </summary>
        public float widthMultiplier;
        /// <summary>
        /// How many seconds a point lasts.
        /// </summary>
        public float time;
        /// <summary>
        /// How far the entity must move in world space before a new point is emitted.
        /// </summary>
        public float minVertexDistance;
        /// <summary>
        /// If false, new points have zero width, which leaves a gap in the trail. Existing points still expire.
        /// </summary>
        public bool emitting;
        /// <summary>
        /// Vertices added to round the outside of each corner. 0 leaves corners sharp.
        /// </summary>
        public int numCornerVertices;
        /// <summary>
        /// Vertices added to round each end. 0 leaves ends flat.
        /// </summary>
        public int numCapVertices;
    }

    /// <summary>
    /// The points of a trail in world space, from oldest to newest. The trail also draws from the newest point to the
    /// entity's current position.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct RibbonTrailPoint : IBufferElementData
    {
        public float3 worldPosition;
        public float  creationTime;
        /// <summary>
        /// If true, this point has zero width. Points added while RibbonTrailConfig.emitting is false are gaps.
        /// </summary>
        public bool isGap;
    }

    /// <summary>
    /// Where the trail last emitted a point.
    /// </summary>
    public struct RibbonTrailEmitterState : IComponentData
    {
        public float3 lastEmitWorldPosition;
        public bool   hasEmitted;
    }
    #endregion

    #region Width and color
    /// <summary>
    /// One keyframe of a ribbon's width curve. The curve runs from 0 to 1 by distance along the ribbon. For lines, it goes from
    /// the first point to the last. For trails, it goes from the entity's current position to the oldest point.
    /// </summary>
    /// <remarks>
    /// Keys must be sorted by time. Before the first key and after the last, the curve holds that key's value. An infinite
    /// tangent makes a step, like it does in an AnimationCurve. With no keys, the width is 1.
    /// </remarks>
    [InternalBufferCapacity(2)]
    public struct RibbonWidthKeyframe : IBufferElementData
    {
        public Calci.Keyframe keyframe;
    }

    /// <summary>
    /// One key of a ribbon's color gradient. The gradient runs along the ribbon the same way as the width curve.
    /// </summary>
    /// <remarks>
    /// Keys must be sorted by time. Before the first key and after the last, the gradient holds that key's color. With no
    /// keys, the color is white.
    /// </remarks>
    [InternalBufferCapacity(2)]
    public struct RibbonColorKey : IBufferElementData
    {
        /// <summary>
        /// The time of this key, in the range 0 to 1.
        /// </summary>
        public float time;

        /// <summary>
        /// The color at this key in gamma space, in the half precision it is stored at. Use <see cref="color"/> to work with
        /// UnityEngine.Color instead.
        /// </summary>
        public half4 colorRaw;

        internal int m_isFixed;

        /// <summary>
        /// The color at this key in gamma space, like the colors of a UnityEngine.Gradient.
        /// </summary>
        public UnityEngine.Color color
        {
            get
            {
                var value = (float4)colorRaw;
                return new UnityEngine.Color(value.x, value.y, value.z, value.w);
            }
            set => colorRaw = (half4) new float4(value.r, value.g, value.b, value.a);
        }

        /// <summary>
        /// If true, the gradient jumps to this key's color right after the previous key instead of blending into it, like
        /// GradientMode.Fixed.
        /// </summary>
        public bool isFixed
        {
            get => m_isFixed != 0;
            set => m_isFixed = value ? 1 : 0;
        }
    }

    public static class RibbonCurves
    {
        /// <summary>
        /// Replaces the contents of the buffer with the keys of the AnimationCurve, the same way baking does.
        /// </summary>
        /// <param name="widthCurve">The width curve, or null for a constant width of 1</param>
        /// <param name="keyframes">The buffer to fill</param>
        public static void SetWidthCurve(UnityEngine.AnimationCurve widthCurve, DynamicBuffer<RibbonWidthKeyframe> keyframes)
        {
            keyframes.Clear();
            if (widthCurve == null)
                return;
            foreach (var key in widthCurve.keys)
                keyframes.Add(new RibbonWidthKeyframe { keyframe = key });
        }

        /// <summary>
        /// Replaces the contents of the buffer with the Gradient, the same way baking does. Color and alpha keys are merged
        /// into one key at each of their times. Perceptual blending and linear-space gradients get extra keys in between,
        /// since the color between two keys is not a straight blend for those.
        /// </summary>
        /// <param name="gradient">The gradient, or null for white</param>
        /// <param name="colorKeys">The buffer to fill</param>
        public static void SetGradient(UnityEngine.Gradient gradient, DynamicBuffer<RibbonColorKey> colorKeys)
        {
            colorKeys.Clear();
            if (gradient == null)
                return;

            var times = new System.Collections.Generic.List<float>();
            foreach (var key in gradient.colorKeys)
                times.Add(key.time);
            foreach (var key in gradient.alphaKeys)
                times.Add(key.time);
            times.Add(0f);
            times.Add(1f);
            times.Sort();

            bool isFixed      = gradient.mode == UnityEngine.GradientMode.Fixed;
            bool needsSubkeys = gradient.mode == UnityEngine.GradientMode.PerceptualBlend || gradient.colorSpace == UnityEngine.ColorSpace.Linear;
            float previous    = -1f;
            foreach (var time in times)
            {
                if (time - previous < 1e-6f)
                    continue;
                if (needsSubkeys && previous >= 0f)
                {
                    for (int i = 1; i < kSubkeysPerSpan; i++)
                        AddKey(colorKeys, gradient, math.lerp(previous, time, (float)i / kSubkeysPerSpan), false);
                }
                AddKey(colorKeys, gradient, time, isFixed);
                previous = time;
            }
        }

        const int kSubkeysPerSpan = 8;

        static void AddKey(DynamicBuffer<RibbonColorKey> colorKeys, UnityEngine.Gradient gradient, float time, bool isFixed)
        {
            colorKeys.Add(new RibbonColorKey { time = time, color = gradient.Evaluate(time), isFixed = isFixed });
        }
    }
    #endregion
}

