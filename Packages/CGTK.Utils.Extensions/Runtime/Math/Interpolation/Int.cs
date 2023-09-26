using System;
using System.Runtime.CompilerServices;
using CGTK.Utils.Extensions.Math.Rounding.Math;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Interpolation
{
	using F32 = Single;
	using I32 = Int32;

	namespace Math
	{
		public static partial class Interpolation
		{
			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static I32 Lerp(this I32 from, in I32 to, in F32 t)
				=> Mathf.Lerp(a: from, b: to, t: t).RoundToInt();
	
			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static I32 LerpUnclamped(this I32 from, in I32 to, in F32 t)
				=> Mathf.LerpUnclamped(a: from, b: to, t: t).RoundToInt();
	
			//[PublicAPI]
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			//public static float InverseLerp(this int value, in int from, in int to)
				//=> Mathf.InverseLerp(a: from, b: to, value: value);
			
			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static I32 LerpAngle(this I32 from, in I32 to, in F32 t)
				=> Mathf.LerpAngle(a: from, b: to, t: t).RoundToInt();
		}
	}
}