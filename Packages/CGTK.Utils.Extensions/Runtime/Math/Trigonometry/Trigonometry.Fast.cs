using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Trigonometry
{
	using F32 = Single;
	using I32 = Int32;

	namespace Math
	{
		public static partial class Trigonometry
		{
			private const I32 _INDEX_MASK   = ~(-1 << 12);
			private const F32 _INDEX_FACTOR = (_CACHE_SIZE / Constants.Constants.TAU);
			private const I32 _CACHE_SIZE   = (_INDEX_MASK + 1);

			private static readonly F32[] SinCache, CosCache, TanCache, AsinCache, AcosCache, AtanCache;

			static Trigonometry()
			{
				SinCache  = new F32[_CACHE_SIZE];
				CosCache  = new F32[_CACHE_SIZE];
				TanCache  = new F32[_CACHE_SIZE];

				AsinCache = new F32[_CACHE_SIZE];
				AcosCache = new F32[_CACHE_SIZE];
				AtanCache = new F32[_CACHE_SIZE];

				I32 __index;
				for (__index = 0; __index < _CACHE_SIZE; __index++)
				{
					F32 __value = ((__index + 0.5f) / _CACHE_SIZE * Constants.Constants.TAU);
					SinCache[__index] = Mathf.Sin(f: __value);
					CosCache[__index] = Mathf.Cos(f: __value);
					TanCache[__index] = Mathf.Tan(f: __value);

					AsinCache[__index] = Mathf.Asin(f: __value);
					AcosCache[__index] = Mathf.Acos(f: __value);
					AtanCache[__index] = Mathf.Atan(f: __value);
				}

				const F32 __FACTOR = (_CACHE_SIZE / 360f);
				for (__index = 0; __index < 360; __index += 90)
				{
					I32 __element = (I32) (__index * __FACTOR) & _INDEX_MASK;

					F32 __angleRadian = (__index * Constants.Constants.DEG_TO_RAD);

					SinCache[__element] = Mathf.Sin(f: __angleRadian);
					CosCache[__element] = Mathf.Cos(f: __angleRadian);
					TanCache[__index]   = Mathf.Tan(f: __angleRadian);

					//needed??
					AsinCache[__element] = Mathf.Asin(f: __angleRadian);
					AcosCache[__element] = Mathf.Acos(f: __angleRadian);
					AtanCache[__element] = Mathf.Atan(f: __angleRadian);
				}
			}

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SinFast(in this F32 value)
				=> SinCache[(I32) (value * _INDEX_FACTOR) & _INDEX_MASK];

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 CosFast(in this F32 value)
				=> CosCache[(I32) (value * _INDEX_FACTOR) & _INDEX_MASK];

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 TanFast(in this F32 value)
				=> TanCache[(I32) (value * _INDEX_FACTOR) & _INDEX_MASK];

			//TODO: Maybe add clamps to Asin/Acos, so it doesn't excede -1 and 1

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 AsinFast(in this F32 value)
				=> AsinCache[(I32) (value * _INDEX_FACTOR) & _INDEX_MASK];

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 AcosFast(in this F32 value)
				=> AcosCache[(I32) (value * _INDEX_FACTOR) & _INDEX_MASK];

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 AtanFast(in this F32 value)
				=> AtanCache[(I32) (value * _INDEX_FACTOR) & _INDEX_MASK];

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 SqrtFast(in this I32 value)
			{
				if (value == 0) return 0; // Avoid zero divide

				I32 n = (value / 2) + 1; // Initial estimate, never low
				I32 n1 = (n + (value / n)) / 2;

				while (n1 < n)
				{
					n = n1;
					n1 = (n + (value / n)) / 2;
				}

				return n;
			}
			
			/*
			[StructLayout(LayoutKind.Explicit)]
			private struct IntFloat
			{
				[FieldOffset(0)]
				public I32 Int;
				
				[FieldOffset(0)]
				public F32 Float;
			}
			
			//Based on https://www.gamedev.net/forums/topic/704525-3-quick-ways-to-calculate-the-square-root-in-c/5417778/?page=1
			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static unsafe F32 SqrtFast(this F32 value)
			{
				IntFloat __union;
				
				__union.Float = 0;
				__union.Int = 0x2035AD0C + (*(I32*)(&value) >> 1);
				
				return value / __union.Float + __union.Float * 0.25f;
			}

			[BurstCompile]
			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static F32 SqrtSlow(in this F32 value)
			{
				v128 __reg = set_ps1(value);

				return cvtss_f32(rcp_ss(rsqrt_ss(__reg)));
			}
			*/
		}
	}
}