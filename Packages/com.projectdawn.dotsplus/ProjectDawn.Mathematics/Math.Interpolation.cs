using System.Runtime.CompilerServices;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace ProjectDawn.Mathematics
{
    using UnityEngine;

    /// <summary>
    /// A static class to contain various math functions.
    /// </summary>
    public static partial class math2
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float3 slerp(float3 start, float3 end, float t)
        {
            // float __dot = dot(normalize(start), normalize(end));
            // float __omega = acos(saturate(__dot));
            // return (sin((1 - t) * __omega) * start + sin(t * __omega) * end) / sin(__omega);
            
            // Dot product - the cosine of the angle between 2 vectors.
            float __dot = dot(x: start, y: end);

            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            __dot = clamp(valueToClamp: __dot, lowerBound: -1.0f, upperBound: +1.0f);

            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float  __theta    = acos(x: __dot) * t;
            float3 __relative = end - start * __dot;
            
            __relative = normalize(x: __relative); 
            // Orthonormal basis
            
            // The final result.
            return ((start * cos(x: __theta)) + (__relative * sin(x: __theta)));
        }
        
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float3 slerpsafe(float3 start, float3 end, float t)
        {
            if(all(start == end)) return start;
            
            // float __dot = dot(normalize(start), normalize(end));
            // float __omega = acos(saturate(__dot));
            // return (sin((1 - t) * __omega) * start + sin(t * __omega) * end) / sin(__omega);
            
            // Dot product - the cosine of the angle between 2 vectors.
            float __dot = dot(x: start, y: end);

            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            __dot = clamp(valueToClamp: __dot, lowerBound: -1.0f, upperBound: +1.0f);

            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float  __theta    = acos(x: __dot) * t;
            float3 __relative = end - start * __dot;
            
            __relative = normalize(x: __relative); 
            // Orthonormal basis
            
            // The final result.
            return ((start * cos(x: __theta)) + (__relative * sin(x: __theta)));
        }
        
        

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static double3 slerp(double3 start, double3 end, double t)
        // {
        //     double __dot = dot(normalize(start), normalize(end));
        //     double __omega = acos(saturate(__dot));
        //     return (sin((1 - t) * __omega) * start + sin(t * __omega) * end) / sin(__omega);
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static float3 slerp(float3 start, float3 end, float3 t)
        // {
        //     float3 __dot = dot(normalize(start), normalize(end));
        //     float3 __omega = acos(saturate(__dot));
        //     return (sin((1 - t) * __omega) * start + sin(t * __omega) * end) / sin(__omega);
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static double3 slerp(double3 start, double3 end, double3 t)
        // {
        //     double3 __dot = dot(normalize(start), normalize(end));
        //     double3 __omega = acos(saturate(__dot));
        //     return (sin((1 - t) * __omega) * start + sin(t * __omega) * end) / sin(__omega);
        // }
        
        /// <summary>
        /// Inverse lerp returns a fraction, based on a value between start and end values.
        /// As example InvLerp(0.5, 1, 0.75) will result in 0.5, because it is middle of range 0.5 and 1.
        /// This is quite useful function if you want linear falloff that has start and end values.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The value between start and end.</param>
        /// <returns></returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float invlerp(float start, float end, float value) => saturate(x: (value - start) / (end - start));
        /// <summary>
        /// Inverse lerp returns a fraction, based on a value between start and end values.
        /// As example InvLerp(0.5, 1, 0.75) will result in 0.5, because it is middle of range 0.5 and 1.
        /// This is quite useful function if you want linear falloff that has start and end values.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The value between start and end.</param>
        /// <returns></returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double invlerp(double start, double end, double value) => saturate(x: (value - start) / (end - start));

        /// <summary>
        /// Inverse lerp returns a fraction, based on a value between start and end values.
        /// As example InvLerp(0.5, 1, 0.75) will result in 0.5, because it is middle of range 0.5 and 1.
        /// This is quite useful function if you want linear falloff that has start and end values.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The value between start and end.</param>
        /// <returns></returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float2 invlerp(float2 start, float2 end, float2 value) => saturate(x: (value - start) / (end - start));
        /// <summary>
        /// Inverse lerp returns a fraction, based on a value between start and end values.
        /// As example InvLerp(0.5, 1, 0.75) will result in 0.5, because it is middle of range 0.5 and 1.
        /// This is quite useful function if you want linear falloff that has start and end values.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The value between start and end.</param>
        /// <returns></returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double2 invlerp(double2 start, double2 end, double2 value) => saturate(x: (value - start) / (end - start));

        /// <summary>
        /// Inverse lerp returns a fraction, based on a value between start and end values.
        /// As example InvLerp(0.5, 1, 0.75) will result in 0.5, because it is middle of range 0.5 and 1.
        /// This is quite useful function if you want linear falloff that has start and end values.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The value between start and end.</param>
        /// <returns></returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float3 invlerp(float3 start, float3 end, float3 value) => saturate(x: (value - start) / (end - start));
        /// <summary>
        /// Inverse lerp returns a fraction, based on a value between start and end values.
        /// As example InvLerp(0.5, 1, 0.75) will result in 0.5, because it is middle of range 0.5 and 1.
        /// This is quite useful function if you want linear falloff that has start and end values.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The value between start and end.</param>
        /// <returns></returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double3 invlerp(double3 start, double3 end, double3 value) => saturate(x: (value - start) / (end - start));

        /// <summary>
        /// Returns true if value is barycentric coordinates.
        /// </summary>
        /// <param name="value">Value used for finding if its barycentric coordinates.</param>
        /// <returns>Returns true if value is barycentric coordinates.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static bool isbarycentric(float3 value) => (value.x + value.y + value.z) <= 1 - EPSILON;
        /// <summary>
        /// Returns true if value is barycentric coordinates.
        /// </summary>
        /// <param name="value">Value used for finding if its barycentric coordinates.</param>
        /// <returns>Returns true if value is barycentric coordinates.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static bool isbarycentric(double3 value) => (value.x + value.y + value.z) <= 1 - EPSILON_DBL;

        /// <summary>
        /// Returns barycentric coordinates of triangle point.
        /// Based on Christer Ericson's Real-Time Collision Detection.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="p">Point inside the triangle</param>
        /// <returns>Returns barycentric coordinates of triangle point.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float3 barycentric(float2 a, float2 b, float2 c, float2 p)
        {
            float2 v0 = b - a; 
            float2 v1 = c - a;
            float2 v2 = p - a;

            float d00 = dot(x: v0, y: v0);
            float d01 = dot(x: v0, y: v1);
            float d11 = dot(x: v1, y: v1);
            float d20 = dot(x: v2, y: v0);
            float d21 = dot(x: v2, y: v1);

            float denom = d00 * d11 - d01 * d01;

            float3 barycentric;
            barycentric.y = (d11 * d20 - d01 * d21) / denom;
            barycentric.z = (d00 * d21 - d01 * d20) / denom;
            barycentric.x = 1.0f - barycentric.z - barycentric.y;
            return barycentric;
        }
        /// <summary>
        /// Returns barycentric coordinates of triangle point.
        /// Based on Christer Ericson's Real-Time Collision Detection.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="p">Point inside the triangle</param>
        /// <returns>Returns barycentric coordinates of triangle point.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double3 barycentric(double2 a, double2 b, double2 c, double2 p)
        {
            double2 v0 = b - a; 
            double2 v1 = c - a;
            double2 v2 = p - a;

            double d00 = dot(x: v0, y: v0);
            double d01 = dot(x: v0, y: v1);
            double d11 = dot(x: v1, y: v1);
            double d20 = dot(x: v2, y: v0);
            double d21 = dot(x: v2, y: v1);

            double denom = d00 * d11 - d01 * d01;

            double3 barycentric;
            barycentric.y = (d11 * d20 - d01 * d21) / denom;
            barycentric.z = (d00 * d21 - d01 * d20) / denom;
            barycentric.x = 1.0f - barycentric.z - barycentric.y;
            return barycentric;
        }

        /// <summary>
        /// Returns barycentric coordinates of triangle point.
        /// Based on Christer Ericson's Real-Time Collision Detection.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="p">Point inside the triangle</param>
        /// <returns>Returns barycentric coordinates of triangle point.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float3 barycentric(float3 a, float3 b, float3 c, float3 p)
        {
            float3 v0 = b - a;
            float3 v1 = c - a;
            float3 v2 = p - a;

            float d00 = dot(x: v0, y: v0);
            float d01 = dot(x: v0, y: v1);
            float d11 = dot(x: v1, y: v1);
            float d20 = dot(x: v2, y: v0);
            float d21 = dot(x: v2, y: v1);

            float denom = d00 * d11 - d01 * d01;

            float3 barycentric;
            barycentric.x = (d11 * d20 - d01 * d21) / denom;
            barycentric.y = (d00 * d21 - d01 * d20) / denom;
            barycentric.z = 1.0f - barycentric.x - barycentric.y;
            return barycentric;
        }
        /// <summary>
        /// Returns barycentric coordinates of triangle point.
        /// Based on Christer Ericson's Real-Time Collision Detection.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="p">Point inside the triangle</param>
        /// <returns>Returns barycentric coordinates of triangle point.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double3 barycentric(double3 a, double3 b, double3 c, double3 p)
        {
            double3 v0 = b - a;
            double3 v1 = c - a;
            double3 v2 = p - a;

            double d00 = dot(x: v0, y: v0);
            double d01 = dot(x: v0, y: v1);
            double d11 = dot(x: v1, y: v1);
            double d20 = dot(x: v2, y: v0);
            double d21 = dot(x: v2, y: v1);

            double denom = d00 * d11 - d01 * d01;

            double3 barycentric;
            barycentric.x = (d11 * d20 - d01 * d21) / denom;
            barycentric.y = (d00 * d21 - d01 * d20) / denom;
            barycentric.z = 1.0f - barycentric.x - barycentric.y;
            return barycentric;
        }

        /// <summary>
        /// Returns blended point between triangle points using barycentric coordinates.
        /// It is basically lerp for three points.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="barycentric">Barycentric coordinates of triangle point.</param>
        /// <returns>Returns blended point between triangle points using barycentric coordinates.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float2 blend(float2 a, float2 b, float2 c, float3 barycentric) => a * barycentric.x + b * barycentric.y + c * barycentric.z;
        /// <summary>
        /// Returns blended point between triangle points using barycentric coordinates.
        /// It is basically lerp for three points.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="barycentric">Barycentric coordinates of triangle point.</param>
        /// <returns>Returns blended point between triangle points using barycentric coordinates.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double2 blend(double2 a, double2 b, double2 c, double3 barycentric) => a * barycentric.x + b * barycentric.y + c * barycentric.z;

        /// <summary>
        /// Returns blended point between triangle points using barycentric coordinates.
        /// It is basically lerp for three points.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="barycentric">Barycentric coordinates of triangle point.</param>
        /// <returns>Returns blended point between triangle points using barycentric coordinates.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static float3 blend(float3 a, float3 b, float3 c, float3 barycentric) => a * barycentric.x + b * barycentric.y + c * barycentric.z;
        /// <summary>
        /// Returns blended point between triangle points using barycentric coordinates.
        /// It is basically lerp for three points.
        /// </summary>
        /// <param name="a">Triangle point.</param>
        /// <param name="b">Triangle point.</param>
        /// <param name="c">Triangle point.</param>
        /// <param name="barycentric">Barycentric coordinates of triangle point.</param>
        /// <returns>Returns blended point between triangle points using barycentric coordinates.</returns>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static double3 blend(double3 a, double3 b, double3 c, double3 barycentric) => a * barycentric.x + b * barycentric.y + c * barycentric.z;
    }
}
