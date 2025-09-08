using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal struct OutputFloat : IComponentData
    {
        public float blended;
    }

    internal struct OutputBinding : IComponentData
    {
        public ComponentBinding binding;
    }

    internal struct OutputEntity : IComponentData
    {
        public Entity entity;
    }
}

