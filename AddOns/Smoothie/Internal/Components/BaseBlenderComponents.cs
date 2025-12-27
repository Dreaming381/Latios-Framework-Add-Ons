using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal struct Progression : IComponentData
    {
        public float progression;
    }

    internal struct Duration : IComponentData
    {
        public float duration;
    }

    internal struct EnabledFlag : IComponentData, IEnableableComponent { }

    internal struct IncompleteFlag : IComponentData, IEnableableComponent, IAutoDestroyExpirable { }

    internal struct BlendInstructions : ISharedComponentData
    {
        public ulong packed;
    }
}

