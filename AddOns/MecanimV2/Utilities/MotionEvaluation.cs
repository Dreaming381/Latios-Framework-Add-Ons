using System;
using Latios.Kinemation;
using Unity.Mathematics;

namespace Latios.MecanimV2
{
    public static class MotionEvaluation
    {
        #region API
        public static float GetBlendedMotionDuration(ref MecanimControllerBlob controller,
                                                     ref SkeletonClipSetBlob clips,
                                                     ReadOnlySpan<MecanimParameter>    parameters,
                                                     MecanimControllerBlob.MotionIndex motion)
        {
            if (motion.invalid)
                return 0f;
            if (!motion.isBlendTree)
            {
                return clips.clips[motion.index].duration;
            }

            ref var tree = ref controller.blendTrees[motion.index];
            switch (tree.blendTreeType)
            {
                case MecanimControllerBlob.BlendTree.BlendTreeType.Simple1D:
                    return GetBlendedMotionDurationSimple1D(ref controller, ref clips, parameters, ref tree);
                case MecanimControllerBlob.BlendTree.BlendTreeType.SimpleDirectional2D:
                    return GetBlendedMotionDurationSimpleDirectional2D(ref controller, ref clips, parameters, ref tree);
                case MecanimControllerBlob.BlendTree.BlendTreeType.FreeformDirectional2D:
                case MecanimControllerBlob.BlendTree.BlendTreeType.FreeformCartesian2D:
                    return GetBlendedMotionDurationFreeform(ref controller, ref clips, parameters, ref tree);
                case MecanimControllerBlob.BlendTree.BlendTreeType.Direct:
                    return GetBlendedMotionDurationDirect(ref controller, ref clips, parameters, ref tree);
                default: return 0f;
            }
        }

        public struct ClipResult
        {
            public float  currentLoopTime;
            public float  previousLoopTime;
            public float  weight;
            public ushort clipIndex;
            public bool   mirror;
        }

        public interface IProcessor
        {
            void Execute(in ClipResult result);
        }

        public static void Evaluate<T>(float motionNormalizedStartTimeWithLoops,
                                       float motionNormalizedEndTimeWithLoops,
                                       ref MecanimControllerBlob controller,
                                       ref SkeletonClipSetBlob clips,
                                       ReadOnlySpan<MecanimParameter>    parameters,
                                       MecanimControllerBlob.MotionIndex motion,
                                       ref T processor)
            where T : unmanaged, IProcessor
        {
            // Todo:
            throw new NotImplementedException();
        }
        #endregion

        #region Utility
        private static float Modulo(float n, float m)
        {
            return ((n % m) + m) % m;
        }

        private static float3 GetBarycentricWeights(float2 clip1Pos, float2 clip2Pos, float2 targetPoint)
        {
            // Determinant of the triangle area
            float denominator = (clip1Pos.x * clip2Pos.y - clip1Pos.y * clip2Pos.x);

            float w1 = (targetPoint.x * clip2Pos.y - targetPoint.y * clip2Pos.x) / denominator;
            float w2 = (targetPoint.y * clip1Pos.x - targetPoint.x * clip1Pos.y) / denominator;
            float w3 = 1.0f - w1 - w2;

            return new float3(w1, w2, w3);
        }
        #endregion

        #region Durations
        private static float GetBlendedMotionDurationSimple1D(ref MecanimControllerBlob controller,
                                                              ref SkeletonClipSetBlob clips,
                                                              ReadOnlySpan<MecanimParameter>      parameters,
                                                              ref MecanimControllerBlob.BlendTree tree)
        {
            // Find the last child before or at our parameter
            int beforeIndex = -1;
            var parameter   = parameters[tree.parameterIndices[0]].floatParam;
            for (int i = 0; i < tree.children.Length; i++)
            {
                if (tree.children[i].position.x <= parameter)
                    beforeIndex = i;
                else
                    break;
            }

            // Find the first child at or after our parameter
            int afterIndex;
            if (beforeIndex >= 0 && tree.children[beforeIndex].position.x == parameter)
                afterIndex = beforeIndex;
            else
                afterIndex = beforeIndex + 1;

            // Try to get the child before's duration.
            float beforeDuration = 0f;
            if (beforeIndex >= 0)
            {
                var timeScale  = math.abs(tree.children[beforeIndex].timeScale);
                beforeDuration = timeScale * GetBlendedMotionDuration(ref controller, ref clips, parameters, tree.children[beforeIndex].motionIndex);
            }

            // Try to get the child after's duration. If invalid, walk backwards.
            float afterDuration = 0f;
            if (afterIndex < tree.children.Length)
            {
                var timeScale = math.abs(tree.children[afterIndex].timeScale);
                afterDuration = timeScale * GetBlendedMotionDuration(ref controller, ref clips, parameters, tree.children[afterIndex].motionIndex);
            }

            // Process results
            if (beforeIndex < 0 && afterIndex == tree.children.Length)
                return 0f; // Tree is totally invalid
            if (beforeIndex < 0)
                return afterDuration;
            if (afterIndex == tree.children.Length)
                return beforeDuration;
            return math.remap(tree.children[beforeIndex].position.x, tree.children[afterIndex].position.x, beforeDuration, afterDuration, parameter);
        }

        // Todo: This needs to be revisited at some point if we want this to be robust.
        private static float GetBlendedMotionDurationSimpleDirectional2D(ref MecanimControllerBlob controller,
                                                                         ref SkeletonClipSetBlob clips,
                                                                         ReadOnlySpan<MecanimParameter>      parameters,
                                                                         ref MecanimControllerBlob.BlendTree tree)
        {
            var childCount      = tree.children.Length;
            var blendParameters = new float2(parameters[tree.parameterIndices[0]].floatParam, parameters[tree.parameterIndices[1]].floatParam);

            int centerClipIndex       = math.asint(tree.pipjs[0].y);
            int counterClockwiseIndex = -1;
            int clockWiseIndex        = -1;

            float clipWeightCounterClockwise = 0f;
            float clipWeightClockwise        = 0f;
            float clipWeightCenter           = 0f;

            // if the parameters are in the center, no need to find anything else, put all the weight on the center
            if (blendParameters.Equals(float2.zero))
            {
                clipWeightCenter = 1f;
            }
            else
            {
                // Find the closest two clips to the blendParameters going clockwise and going counterclockwise (that are not the center clip)
                float targetAngle                   = math.atan2(blendParameters.y, blendParameters.x);
                float minCounterClockwiseDifference = float.MaxValue;
                float minClockwiseDifference        = float.MaxValue;

                for (int i = 0; i < tree.pipjs.Length; i++)
                {
                    if (i != centerClipIndex)
                    {
                        float angle = tree.pipjs[i].x;
                        float diff  = angle - targetAngle;

                        // Normalize the angle difference to be between [-π, π]
                        diff = Modulo(diff + math.PI, 2 * math.PI) - math.PI;

                        if (diff >= 0 && diff < minCounterClockwiseDifference)
                        {
                            minCounterClockwiseDifference = diff;
                            counterClockwiseIndex         = i;
                        }
                        else if (diff < 0 && -diff < minClockwiseDifference)
                        {
                            minClockwiseDifference = -diff;
                            clockWiseIndex         = i;
                        }
                    }
                }

                if (counterClockwiseIndex < 0 || clockWiseIndex < 0)
                {
                    // TODO: we should check blend tree child positions and give warnings about this while baking so it doesn't fail silently.
                    // Requirements for 2D Simple Directional blend trees: Clip positions must be at least 2 degrees and at most 180 degrees apart from each other radially.
                    // Only one optional clip can be at the center.
                    return 0f;
                }

                // Calculate the 3 barycentric weights for the blendParameters target position, using the triangle formed by the 2 clips positions and the center
                float3 weights             = GetBarycentricWeights(tree.children[counterClockwiseIndex].position, tree.children[clockWiseIndex].position, blendParameters);
                clipWeightCounterClockwise = weights.x;
                clipWeightClockwise        = weights.y;
                clipWeightCenter           = weights.z;

                // When the target point is beyond the segment that connects the two clips, the center weight will turn negative
                // To fix this, we need to make the weight center 0, and divide the weights of the remaining two clips by the sum of both, to make sure they add up to 1
                if (clipWeightCenter < 0f)
                {
                    clipWeightCenter            = 0f;
                    float sumClipWeights        = clipWeightClockwise + clipWeightCounterClockwise;
                    clipWeightCounterClockwise /= sumClipWeights;
                    clipWeightClockwise        /= sumClipWeights;
                }
            }

            // If there is no center clip, it's weight gets split evenly between all clips
            float extraWeightForEachChild = 0f;
            if (centerClipIndex < 0)
            {
                extraWeightForEachChild = clipWeightCenter / childCount;
            }

            // Sum all the durations, multiplied by their weights and time scales
            float duration = 0f;
            for (int i = 0; i < childCount; i++)
            {
                float weight = extraWeightForEachChild;

                if (i == centerClipIndex)
                    weight += clipWeightCenter;
                if (i == clockWiseIndex)
                    weight += clipWeightClockwise;
                if (i == counterClockwiseIndex)
                    weight += clipWeightCounterClockwise;

                if (weight > 0f)
                {
                    float clipDuration         = GetBlendedMotionDuration(ref controller, ref clips, parameters, tree.children[i].motionIndex);
                    float clipTimeScale        = math.abs(tree.children[i].timeScale);
                    float weightedClipDuration = weight * clipTimeScale * clipDuration;

                    duration += weightedClipDuration;
                }
            }

            return duration;
        }

        // Freeform (directional or cartesian)
        private static float GetBlendedMotionDurationFreeform(ref MecanimControllerBlob controller,
                                                              ref SkeletonClipSetBlob clips,
                                                              ReadOnlySpan<MecanimParameter>      parameters,
                                                              ref MecanimControllerBlob.BlendTree tree)
        {
            // See https://runevision.com/thesis/rune_skovbo_johansen_thesis.pdf at 6.3 (p58) for details.
            // Freeform cartesian uses cartesian gradient bands, while freeform directional uses gradient
            // bands in polar space.
            var         childCount = tree.children.Length;
            Span<float> weights    = stackalloc float[childCount];
            Span<float> durations  = stackalloc float[childCount];
            durations.Fill(-1f);

            var blendParameters = new float2(parameters[tree.parameterIndices[0]].floatParam, parameters[tree.parameterIndices[1]].floatParam);

            // We try to only sample the child nodes if they have nonzero weights.
            float accumulatedWeight = 0f;
            // Reset the weights
            weights.Fill(float.MaxValue);
            accumulatedWeight = 0f;

            for (int i = 0; i < childCount; i++)
            {
                // Compute pip vector
                var    p  = blendParameters;
                var    pi = tree.children[i].position;
                float2 pip;
                if (tree.blendTreeType == MecanimControllerBlob.BlendTree.BlendTreeType.FreeformDirectional2D)
                {
                    var pmag      = math.length(p);
                    var pimag     = math.length(pi);
                    pip.x         = (pmag - pimag) / (0.5f * (pmag + pimag));
                    var direction = LatiosMath.ComplexMul(pi, new float2(p.x, -p.y));
                    pip.y         = MecanimControllerBlob.BlendTree.kFreeformDirectionalBias * math.atan2(direction.y, direction.x);
                }
                else
                {
                    pip = p - pi;
                }

                // Evaluate weights using dot(pip, pipj)
                for (int j = 0; j < childCount; j++)
                {
                    // TODO: skipping i==j fixes out of range exceptions for now (we don't store pipjs values for i==j in tree.pipjs),
                    // but might be completely wrong to just skip them.
                    if (i == j) continue; 
                    var pipj   = tree.pipjs[MecanimControllerBlob.BlendTree.PipjIndex(i, j, childCount)];
                    var h      = math.max(0, 1 - math.dot(pip, pipj.xy) * pipj.z);
                    weights[i] = math.min(weights[i], h);
                }

                accumulatedWeight += weights[i];

                // Populate child node duration for nonzero weights
                if (weights[i] > 0f)
                {
                    durations[i] = math.abs(tree.children[i].timeScale) * GetBlendedMotionDuration(ref controller, ref clips, parameters, tree.children[i].motionIndex);
                }
            }

            float inverseTotalWeight = math.select(1f / accumulatedWeight, 0f, accumulatedWeight <= 0f);
            float result             = 0f;
            for (int i = 0; i < childCount; i++)
            {
                result += inverseTotalWeight * weights[i] * durations[i];
            }
            return result;
        }

        private static float GetBlendedMotionDurationDirect(ref MecanimControllerBlob controller,
                                                            ref SkeletonClipSetBlob clips,
                                                            ReadOnlySpan<MecanimParameter>      parameters,
                                                            ref MecanimControllerBlob.BlendTree tree)
        {
            var totalWeight   = 0f;
            var totalDuration = 0f;
            for (int i = 0; i < tree.children.Length; i++)
            {
                var weight = parameters[tree.parameterIndices[i]].floatParam;
                if (weight > 0f)
                {
                    totalWeight   += weight;
                    totalDuration += weight * math.abs(tree.children[i].timeScale) * GetBlendedMotionDuration(ref controller,
                                                                                                              ref clips,
                                                                                                              parameters,
                                                                                                              tree.children[i].motionIndex);
                }
            }
            if (totalWeight <= 0f)
                return 0f;
            return totalDuration / totalWeight;
        }
        #endregion

        #region Clips
        // Todo: All blend tree types for evaluation
        #endregion
    }
}

