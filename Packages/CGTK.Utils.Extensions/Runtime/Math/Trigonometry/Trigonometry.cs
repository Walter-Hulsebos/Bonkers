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

			#region Power

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Pow(in this F32 value, in F32 power) 
				=> (F32)System.Math.Pow(x: value, y: power);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Pow(in this I32 value, in F32 power) 
				=> (F32)System.Math.Pow(x: value, y: power);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Sqrt(in this F32 value)
				=> (F32)System.Math.Sqrt(d: value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Squared(in this F32 value)
				=> (value * value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Cubed(in this F32 value)
				=> (value * value * value);

			#endregion

			#region Trigonometry

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Sin(in this F32 value)
				=> (F32)System.Math.Sin(a: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Cos(in this F32 value)
				=> (F32)System.Math.Cos(d: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Tan(in this F32 value)
				=> (F32)System.Math.Tan(a: value);
			
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Asin(in this F32 value)
				=> (F32)System.Math.Asin(d: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Acos(in this F32 value)
				=> (F32)System.Math.Acos(d: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Atan(in this F32 value)
				=> (F32)System.Math.Atan(d: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Atan2(in this F32 y, in F32 x)
				=> (F32)System.Math.Atan2(y: y, x: x);
			
			#endregion


			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Exp(in this F32 power)
				=> (F32)System.Math.Exp(d: power);
			
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Pow(in this F32? value, in F32 power) 
				=> Pow(value: (F32)value, power: power);
			
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Squared(in this F32? value)
				//=> (float)(value * value);
			{
				F32 __value = (F32) value;
				
				return (__value * __value);
			}
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Cubed(in this F32? value)
				=> (F32)(value * value * value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Sqrt(in this F32? value) 
				=> Sqrt(value: (F32)value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Sin(in this F32? value)
				=> Sin(value: (F32)value);
			
						
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Cos(in this F32? value) 
				=> Cos(value: (F32)value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Tan(in this F32? value)
				=> Tan(value: (F32)value);
			
			
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Asin(in this F32? value)
				=> Asin(value: (F32)value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Acos(in this F32? value)
				=> Acos(value: (F32)value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Atan(in this F32? value)
				=> Atan(value: (F32)value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Atan2(in this F32? y, in F32? x)
				=> Atan2(y: (F32)y, x: (F32)x);
			
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Exp(in this F32? power)
				=> Exp(power: (F32)power);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ToAngle(in this Vector2 value) //custom
				=> Atan2(x: value.x, y: value.y);
		}
	}
}