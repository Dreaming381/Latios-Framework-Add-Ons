using Unity.Mathematics;

namespace Latios.Mecanim
{
    public class MathUtil
    {
        /**
         * Scales a quaternion by a float using angle-axis approach
         */
        public static quaternion ScaleQuaternion(quaternion transformQvvsRotation, float scale)
        {
            transformQvvsRotation = math.normalize(transformQvvsRotation);

            float halfAngle = math.acos(transformQvvsRotation.value.w);
            float angle = halfAngle * 2f;
            
            float sinHalf = math.sin(halfAngle);
            float3 axis = (sinHalf > 1e-6f) 
                ? transformQvvsRotation.value.xyz / sinHalf
                : new float3(0f,0f,1f); // fallback axis
            
            quaternion scaledQuaternion = quaternion.AxisAngle(axis, angle * scale);
            return scaledQuaternion;
        }
    }
}