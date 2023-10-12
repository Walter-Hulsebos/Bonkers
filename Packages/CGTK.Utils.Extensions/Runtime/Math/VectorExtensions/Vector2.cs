using System;
using System.Runtime.CompilerServices;
using CGTK.Utils.Extensions.Math.Math;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.VectorExtensions
{
    namespace Math
    {
        public static partial class VectorExtensions
        {
            [PublicAPI]
            [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
            public static Boolean Approx(in this Vector2 self, in Vector2 compareTo) 
                => (self.x.Approx(compareTo: compareTo.x) && 
                    self.y.Approx(compareTo: compareTo.y));

        }
    }
}
