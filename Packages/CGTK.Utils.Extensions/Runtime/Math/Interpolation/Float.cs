using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Interpolation
{
	namespace Math
	{
		public static partial class Interpolation
		{
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Single Lerp(ref this Single from, in Single to, in Single t)
				=> Mathf.Lerp(a: from, b: to, t: t);
	
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Single LerpUnclamped(ref this Single from, in Single to, in Single t)
				=> Mathf.LerpUnclamped(a: from, b: to, t: t);
	
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Single InverseLerp(ref this Single value, in Single from, in Single to)
				=> Mathf.InverseLerp(a: from, b: to, value: value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Single LerpAngle(ref this Single from, in Single to, in Single t)
				=> Mathf.LerpAngle(a: from, b: to, t: t);


			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Single MoveTowards(ref this Single current, in Single target, in Single maxDelta)
				=> Mathf.MoveTowards(current: current, target: target, maxDelta: maxDelta);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Single MoveTowardsAngle(ref this Single current, in Single target, in Single maxDelta)
				=> Mathf.MoveTowardsAngle(current: current, target: target, maxDelta: maxDelta);
			
		}
	}
}