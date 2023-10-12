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
			/// <summary> Use this to compare two <see cref="Vector3"/>s. </summary>
			/// <param name="self"></param>
			/// <param name="compareTo"></param>
			/// <returns></returns>
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Boolean Approx(in this Vector3 self, in Vector3 compareTo) 
				=> (self.x.Approx(compareTo: compareTo.x) && 
					self.y.Approx(compareTo: compareTo.y) && 
					self.z.Approx(compareTo: compareTo.z));
            
		}
	}
}