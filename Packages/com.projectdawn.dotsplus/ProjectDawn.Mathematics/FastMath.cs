using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace ProjectDawn.Mathematics
{
    /// <summary>
    /// A static class to contain various fast math functions that has lower precision.
    /// </summary>
    public static partial class fastmath
    {
        /// <summary>
        /// <see cref="int"/> and <see cref="float"/> shares same memory.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct IntFloatUnion
        {
            [FieldOffset(0)]
            public float f;
            [FieldOffset(0)]
            public int i;
        }
        
        /// <summary>
        /// <see cref="int"/> and <see cref="double"/> shares same memory.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct IntDoubleUnion
        {
            //named f because a double is also a floating-point type.
            [FieldOffset(0)]
            public double f;
            [FieldOffset(0)]
            public int i;
        }
        
        
        /// <summary>
        /// Returns 1/sqrt(value).
        /// Based on https://en.wikipedia.org/wiki/Fast_inverse_square_root.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float rsqrt(float value)
        {
            IntFloatUnion u = new IntFloatUnion();
            u.f = value;
            u.i = 1597463174 - (u.i >> 1);
            return u.f * (1.5f - (0.5f * value * u.f * u.f));
        }
        
        /// <summary>
        /// Returns 1/sqrt(value).
        /// Based on https://en.wikipedia.org/wiki/Fast_inverse_square_root.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double rsqrt(double value)
        {
            // IntDoubleUnion u = new IntDoubleUnion();
            // u.f = value;
            // u.i = 1597463174 - (u.i >> 1);
            // return u.f * (1.5d - (0.5d * value * u.f * u.f));
            IntDoubleUnion u = new IntDoubleUnion();
            u.i = 0x5F1FFFF9 - (u.i >> 1);
            u.f *= 0.703952253d * (2.38924456d - value * u.f * u.f);
            return u.f;
        }

        const float RFAC2 = 1f / 2f;
        const float RFAC3 = 1f / 6f;
        const float RFAC4 = 1f / 24f;
        const float RFAC5 = 1f / 120f;
        const float RFAC6 = 1f / 720f;
        const float RFAC7 = 1f / 5040f;
        
        const double RFAC2_D = 1d / 2d;
        const double RFAC3_D = 1d / 6d;
        const double RFAC4_D = 1d / 24d;
        const double RFAC5_D = 1d / 120d;
        const double RFAC6_D = 1d / 720d;
        const double RFAC7_D = 1d / 5040d;

        /// <summary>
        /// Returns cosine of value.
        /// Based on Maclaurin Series 4 iterations https://blogs.ubc.ca/infiniteseriesmodule/units/unit-3-power-series/taylor-series/the-maclaurin-expansion-of-cosx/.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float cos(float x)
        {
            float x2 = x * x;
            float x4 = x2 * x2;
            float x6 = x4 * x2;
            // Maclaurin Series
            return 1 - (x2 * RFAC2) + (x4 * RFAC4) - (x6 * RFAC6);
        }
        
        /// <summary>
        /// Returns cosine of value.
        /// Based on Maclaurin Series 4 iterations https://blogs.ubc.ca/infiniteseriesmodule/units/unit-3-power-series/taylor-series/the-maclaurin-expansion-of-cosx/.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double cos(double x)
        {
            double x2 = x * x;
            double x4 = x2 * x2;
            double x6 = x4 * x2;
            // Maclaurin Series
            return 1 - (x2 * RFAC2_D) + (x4 * RFAC4_D) - (x6 * RFAC6_D);
        }

        /// <summary>
        /// Returns cosine of value.
        /// Based on Maclaurin Series 4 iterations https://blogs.ubc.ca/infiniteseriesmodule/units/unit-3-power-series/taylor-series/the-maclaurin-expansion-of-cosx/.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float sin(float x)
        {
            float x2 = x * x;
            float x4 = x2 * x2;
            float x6 = x4 * x2;
            // Maclaurin Series
            return x * (1 - (x2 * RFAC3) + (x4 * RFAC5) - (x6 * RFAC7));
        }
        
        /// <summary>
        /// Returns cosine of value.
        /// Based on Maclaurin Series 4 iterations https://blogs.ubc.ca/infiniteseriesmodule/units/unit-3-power-series/taylor-series/the-maclaurin-expansion-of-cosx/.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sin(double x)
        {
            double x2 = x * x;
            double x4 = x2 * x2;
            double x6 = x4 * x2;
            // Maclaurin Series
            return x * (1 - (x2 * RFAC3_D) + (x4 * RFAC5_D) - (x6 * RFAC7_D));
        }
    }
}
