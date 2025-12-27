using System;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.FlowFieldNavigation
{
    [Serializable]
    public struct FlowSettings
    {
        internal const float PassabilityLimit = 500000;
        internal const int MaxFootprintSize = 10;
        internal const int MaxDensity = 10;

        [Range(0, 10)]
        public float DensityInfluence;

        public static FlowSettings Default => new()
        {
            DensityInfluence = 1f,
        };
    }
}