using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Interpolation
{
	using F32 = Single;

	namespace Math
	{
		public static partial class Interpolation
		{
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Color Lerp(in this Color from, in Color to, in F32 t)
				=> Color.Lerp(a: from, b: to, t: t);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Color LerpUnclamped(in this Color from, in Color to, in F32 t)
				=> Color.LerpUnclamped(a: from, b: to, t: t);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Vector3 LerpInverse(in this Color value, in Color from, in Color to)
			{
				F32 r = Mathf.InverseLerp(a: from.r, b: to.r, value: value.r);
				F32 g = Mathf.InverseLerp(a: from.g, b: to.g, value: value.g);
				F32 b = Mathf.InverseLerp(a: from.b, b: to.b, value: value.b);

				return new Vector3(x: r, y: g, z: b);
			}
		}
	}
}