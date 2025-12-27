using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Smoothie
{
    internal static class InstructionSet
    {
        // Top level, 6 bits
        public enum Procedure : byte
        {
            Noop = 0,  // Do nothing, the instructions might have been defaulted
            ProgressCurveInterpolate,  // Apply a curve to the progress, then interpolate between start and end values
        }

        public static Procedure GetProcedure(BlendInstructions instructions) => (Procedure)Bits.GetBits(instructions.packed, 0, 6);
        public static void SetProcedure(ref BlendInstructions instructions, Procedure procedure) => Bits.SetBits(ref instructions.packed, 0, 6, (byte)procedure);

        // Lifecycle flags, 2 bits (only 1 used currently)
        public static bool GetUsesIncompleteFlag(BlendInstructions instructions) => Bits.GetBit(instructions.packed, 6);
        public static void SetUsesIncompleteFlag(ref BlendInstructions instructions, bool usesIncompleteFlag) => Bits.SetBit(ref instructions.packed, 6, usesIncompleteFlag);

        // Blend Payload for Progress Curve Interpolate procedures
        // Curve function, 8 bits
        public enum CurveFunction : byte
        {
            Passthrough = 0,  // y = x
            Smoothstep = 1,
        }

        public static CurveFunction GetCurveFunction(BlendInstructions instructions) => (CurveFunction)Bits.GetBits(instructions.packed, 8, 8);
        public static void SetCurveFunction(ref BlendInstructions instructions, CurveFunction curveFunction) => Bits.SetBits(ref instructions.packed, 8, 8, (byte)curveFunction);

        // Output type for interpolated fields, 8 bits
        public enum InterpolatedOutputType : byte
        {
            Float = 0,
        }

        public static InterpolatedOutputType GetInterpolatedOutputType(BlendInstructions instructions) => (InterpolatedOutputType)Bits.GetBits(instructions.packed, 16, 8);
        public static void SetInterpolatedOutputType(ref BlendInstructions instructions, InterpolatedOutputType outputType) => Bits.SetBits(ref instructions.packed,
                                                                                                                                            16,
                                                                                                                                            8,
                                                                                                                                            (byte)outputType);

        // Interpolation control binding flags, 8 bits
        public static bool GetInterpolationStartIsBinding(BlendInstructions instructions) => Bits.GetBit(instructions.packed, 24);
        public static void SetInterpolationStartIsBinding(ref BlendInstructions instructions, bool isBinding) => Bits.SetBit(ref instructions.packed, 24, isBinding);
        public static bool GetInterpolationEndIsBinding(BlendInstructions instructions) => Bits.GetBit(instructions.packed, 25);
        public static void SetInterpolationEndIsBinding(ref BlendInstructions instructions, bool isBinding) => Bits.SetBit(ref instructions.packed, 25, isBinding);

        // Single value output mode,
        //public enum SingleOutputMode : byte
    }
}

