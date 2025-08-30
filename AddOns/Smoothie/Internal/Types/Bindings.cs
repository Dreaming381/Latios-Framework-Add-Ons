using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal struct ComponentBinding
    {
        public TypeIndex typeIndex;
        public int       offset;

        public static unsafe ComponentBinding Create<TBlendType, TComponentType>(ref TComponentType component,
                                                                                 ref TBlendType field) where TBlendType : unmanaged where TComponentType : unmanaged,
        IComponentData
        {
            var componentPtr = (byte*)UnsafeUtility.AddressOf(ref component);
            var fieldPtr     = (byte*)UnsafeUtility.AddressOf(ref field);
            CheckFieldValid<TBlendType, TComponentType>(componentPtr, fieldPtr);

            return new ComponentBinding
            {
                typeIndex = TypeManager.GetTypeIndex<TComponentType>(),
                offset    = (int)(fieldPtr - componentPtr),
            };
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static unsafe void CheckFieldValid<TBlendType, TComponentType>(byte* componentPtr, byte* fieldPtr) where TBlendType : unmanaged where TComponentType : unmanaged,
        IComponentData
        {
            var componentSize = UnsafeUtility.SizeOf<TComponentType>();
            var fieldSize     = UnsafeUtility.SizeOf<TBlendType>();
            if (fieldPtr < componentPtr || fieldPtr + fieldSize > componentPtr + componentSize)
            {
                throw new System.ArgumentOutOfRangeException("The field is not fully contained within the specified struct.");
            }
        }
    }

    internal struct EntityComponentBinding
    {
        public Entity           entity;
        public ComponentBinding binding;
    }
}

