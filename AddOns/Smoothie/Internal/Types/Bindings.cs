using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal struct ComponentBinding
    {
        public TypeIndex            typeIndex;
        public short                offset;
        public BindingPrimitiveType primitiveType;

        public static unsafe ComponentBinding Create<TBlendType, TComponentType>(ref TComponentType component,
                                                                                 ref TBlendType field) where TBlendType : unmanaged where TComponentType : unmanaged,
        IComponentData
        {
            var componentPtr = (byte*)UnsafeUtility.AddressOf(ref component);
            var fieldPtr     = (byte*)UnsafeUtility.AddressOf(ref field);
            CheckFieldValid<TBlendType, TComponentType>(componentPtr, fieldPtr);

            BindingPrimitiveType primitiveType = default;
            if (BurstRuntime.GetHashCode64<TBlendType>() == BurstRuntime.GetHashCode64<float>())
                primitiveType = BindingPrimitiveType.Float;
            else
            {
                ThrowUnrecognizedType<TBlendType>();
            }

            return new ComponentBinding
            {
                typeIndex     = TypeManager.GetTypeIndex<TComponentType>(),
                offset        = (short)(fieldPtr - componentPtr),
                primitiveType = primitiveType,
            };
        }

        public static unsafe T ConvertBindingTo<T>(void* srcPtr, BindingPrimitiveType srcType, BindingPrimitiveType dstType) where T : unmanaged
        {
            switch ((dstType, srcType))
            {
                case (BindingPrimitiveType.Float, BindingPrimitiveType.Float):
                    return *(T*)srcPtr;
            }
            return default;
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

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void ThrowUnrecognizedType<TBlendType>() where TBlendType : unmanaged
        {
            throw new System.ArgumentException("The binding field is not a primitive type Smoothie can blend. Please use UnsafeUtility.As<T, U>() to convert to a primitive type.");
        }
    }

    internal struct EntityComponentBinding
    {
        public Entity           entity;
        public ComponentBinding binding;
    }

    internal enum BindingPrimitiveType : byte
    {
        Float = 0,
    }
}

