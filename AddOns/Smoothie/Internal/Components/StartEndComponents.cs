using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal struct ComponentBindingStart : IComponentData
    {
        public EntityComponentBinding binding;
    }

    internal struct ComponentBindingEnd : IComponentData
    {
        public EntityComponentBinding binding;
    }

    internal struct ConstantStartFloat : IComponentData
    {
        public float start;
    }

    internal struct ConstantEndFloat : IComponentData
    {
        public float end;
    }
}

