using System;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Constants
{
    using F32 = Single;

    public static class Constants
    {
        /// <summary>
        /// Circle constant relating the circumference of a circle to its linear dimension.
        /// (circumference / radius)
        /// </summary>
        public const F32 TAU = 6.28318530717959f;
        
        public const F32 PI = Mathf.PI;
        
        public const F32 PI_HALF = (PI * 0.5f);
        
        /// <summary>
        /// Use this to convert from Degrees to Radians.
        /// (TAU / 360) or (PI / 180)
        /// </summary>
        public const F32 DEG_TO_RAD = (TAU / 360f);
        
        /// <summary>
        /// Use this to convert from Radians to Degrees.
        /// (360 / TAU) or (180 / PI)
        /// </summary>
        public const F32 RAD_TO_DEG = (360f / TAU);
        
        public const F32 INFINITY = Mathf.Infinity;
        
        public const F32 NEGATIVE_INFINITY = Mathf.NegativeInfinity;
        
        public static F32 EPSILON = Mathf.Epsilon;
    }
}
