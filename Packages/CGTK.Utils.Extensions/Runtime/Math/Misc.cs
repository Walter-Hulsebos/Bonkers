using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math
{
	using F32 = Single;
	using I32 = Int32;

	namespace Math
	{
		public static partial class Misc
		{
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Abs(this F32 value) => Mathf.Abs(f: value);
		
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ToAbs(ref this F32 value) => value = Mathf.Abs(f: value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Sign(this F32 value) => Mathf.Sign(f: value); //((value > 0) - (value < 0));
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SignA(this F32 value) => Mathf.Sign(f: value); //((value > 0) - (value < 0));
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SignB(this F32 value) => math.sign(x: value); //((value > 0) - (value < 0));

			[StructLayout(layoutKind: LayoutKind.Explicit)]
			private struct BoolByte
			{
				[FieldOffset(offset: 0)]
				public Boolean Bool;
				[FieldOffset(offset: 0)]
				public Byte Byte;
			}

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SignC(this F32 value)
			{
				BoolByte __a, __b;
				__a.Byte = 0;
				__b.Byte = 0;
			
				__a.Bool = (value > 0);
				__b.Bool = (value < 0);
			
				return (__a.Byte - __b.Byte);
			}
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static unsafe F32 SignD(in this F32 value)
			{
				Boolean __larger = (value > 0);
				Boolean __smaller = (value < 0);

				return (*(I32*)&__larger - *(I32*)&__smaller);
			}
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SignE(this F32 value) => (value > 0.0f ? 1.0f : 0.0f) - (value < 0.0f ? 1.0f : 0.0f);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SignF(this F32 value) => (value >= 0.0 ? 1f : -1f);

			#region Min

				#region Float

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static F32 Min(this F32 a, F32 b) => (a < b) ? a : b;

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static F32 Min(this (F32 a, F32 b) compare) => (compare.a < compare.b) ? compare.a : compare.b;
				
				#endregion

				#region Int

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static I32 Min(this I32 a, I32 b) => (a < b) ? a : b;

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static I32 Min(this (I32 a, I32 b) compare) => (compare.a < compare.b) ? compare.a : compare.b;
				
				#endregion
			
			#endregion
			
			#region Max

				#region Float

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static F32 Max(this F32 a, F32 b) => (a > b) ? a : b;

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static F32 Max(this (F32 a, F32 b) compare) => (compare.a > compare.b) ? compare.a : compare.b;

				#endregion

				#region Int

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static I32 Max(this I32 a, I32 b) => (a > b) ? a : b;

				[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static I32 Max(this (I32 a, I32 b) compare) => (compare.a > compare.b) ? compare.a : compare.b;

				#endregion
			
			#endregion
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Max(in this F32 a, in F32 b) => Mathf.Max(a: a, b: b);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 Max(in this I32 a, in I32 b) => (a > b ? a : b);


			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 AtLeastZero(in this I32 value)
				=> Max(a: 0, b: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 AtLeastZero(in this F32 value)
				=> Max(a: 0, b: value);
			
			//return x < y ? x : y;
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Clamp(in this F32 value, in F32 min, in F32 max)
				=> Mathf.Clamp(value: value, min: min, max: max);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Clamp1(this F32 value, F32 min, F32 max)
				=> Mathf.Clamp(value: value, min: min, max: max);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ClampA(this F32 value, F32 min, F32 max)
				=> math.clamp(valueToClamp: value, lowerBound: min, upperBound: max);

			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ClampB(this F32 value, F32 min, F32 max)
			{
				if (value < min)
				{
					value = min;
				}
				else if (value > max)
				{
					value = max;
				}
				return value;
			}

			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ClampC(this F32 value, F32 min, F32 max) 
				=> Max(a: Min(a: value, b: max), b: min);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Clamp01(this F32 value)
				=> Mathf.Clamp01(value: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ClampNeg1To1(this F32 value)
			{
				if( value < -1f ) value = -1f;
				if( value > 1f ) value = 1f;
				return value;
			}

			//TODO: Vector2/float2 Clamping/Rounding.
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Log(this F32 value)
				=> Mathf.Log(f: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 LogA(this F32 value)
				=> (F32)System.Math.Log(d: value); //math.log(value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Log10(this F32 value)
				=> Mathf.Log10(f: value);
			
			
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Boolean Approx(this F32 value, in F32 compareTo)
				=> Mathf.Approximately(a: value, b: compareTo);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Boolean IsZero(this F32 value)
				=> Mathf.Approximately(a: value, b: 0);

		}
	}
}