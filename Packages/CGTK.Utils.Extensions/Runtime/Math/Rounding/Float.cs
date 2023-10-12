using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Rounding
{ 
	using F32 = Single;
	using I32 = Int32;
	using U8  = Byte;

	namespace Math
	{
		public static partial class Rounding
		{
			[PublicAPI]
			public enum Mode : U8
			{
				Nearest,
				Up,
				Down
			}

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Round(this F32 value, in Mode roundingMode)
			{
				return roundingMode switch
				{
					Mode.Nearest => Mathf.Round(f: value),
					Mode.Down    => Mathf.Floor(f: value),
					Mode.Up      => Mathf.Ceil(f: value),
					_            => throw new ArgumentOutOfRangeException(paramName: nameof(roundingMode), actualValue: roundingMode, message: null)
				};
			}
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 RoundToInt(this F32 value, Mode roundingMode)
			{
				return roundingMode switch
				{
					Mode.Nearest => Mathf.RoundToInt(f: value),
					Mode.Down    => Mathf.FloorToInt(f: value),
					Mode.Up      => Mathf.CeilToInt(f: value),
					_            => throw new ArgumentOutOfRangeException(paramName: nameof(roundingMode), actualValue: roundingMode, message: null)
				};
			}
			
			[PublicAPI]
				[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
				public static F32 Round(this F32 value)
				=> Mathf.Round(f: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Round2(this F32 value)
				=> (F32)System.Math.Round(a: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 RoundToInt(this F32 value)
				=> Mathf.RoundToInt(f: value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Floor(this F32 value)
				=> Mathf.Floor(f: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 FloorToInt(this F32 value)
				=> Mathf.FloorToInt(f: value);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Ceil(this F32 value)
				=> Mathf.Ceil(f: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 CeilToInt(this F32 value)
				=> Mathf.CeilToInt(f: value);

			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 ClosestPowerOfTwo(this F32 value)
				=> Mathf.ClosestPowerOfTwo(value: Mathf.RoundToInt(f: value));
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 NextPowerOfTwo(this F32 value)
				=> Mathf.NextPowerOfTwo(value: Mathf.RoundToInt(f: value));

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 PreviousPowerOfTwo(this F32 value)
			{
				I32 __valueRoundedToInt = Mathf.RoundToInt(f: value);
				
				if (__valueRoundedToInt == 0) 
				{
					return 0;
				}
				
				__valueRoundedToInt |= (__valueRoundedToInt >> 1);
				__valueRoundedToInt |= (__valueRoundedToInt >> 2);
				__valueRoundedToInt |= (__valueRoundedToInt >> 4);
				__valueRoundedToInt |= (__valueRoundedToInt >> 8);
				__valueRoundedToInt |= (__valueRoundedToInt >> 16);
				return __valueRoundedToInt - (__valueRoundedToInt >> 1);
			}

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 ToPowerOfTwo(this F32 value, Mode mode)
			{
				return mode switch
				{
					Mode.Nearest => ClosestPowerOfTwo(value: value),
					Mode.Down    => PreviousPowerOfTwo(value: value),
					Mode.Up      => NextPowerOfTwo(value: value),
					_            => throw new ArgumentOutOfRangeException(paramName: nameof(mode), actualValue: mode, message: null)
				};
			}

		}
	}
}