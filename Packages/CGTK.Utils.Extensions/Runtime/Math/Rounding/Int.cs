using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Rounding
{
	using I32 = Int32;

	namespace Math
	{
		public static partial class Rounding
		{
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 ClosestPowerOfTwo(this I32 value)
				=> Mathf.ClosestPowerOfTwo(value: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 NextPowerOfTwo(this I32 value)
				=> Mathf.NextPowerOfTwo(value: value);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 PreviousPowerOfTwo(this I32 value)
			{
				if (value == 0) return 0;

				value |= (value >> 1);
				value |= (value >> 2);
				value |= (value >> 4);
				value |= (value >> 8);
				value |= (value >> 16);
				return value - (value >> 1);
			}
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32 ToPowerOfTwo(this I32 value, Mode roundingMode)
			{
				return roundingMode switch
				{
					Mode.Nearest => ClosestPowerOfTwo(value: value),
					Mode.Down    => PreviousPowerOfTwo(value: value),
					Mode.Up      => NextPowerOfTwo(value: value),
					_            => throw new ArgumentOutOfRangeException(paramName: nameof(roundingMode), actualValue: roundingMode, message: null)
				};
			}

		}
	}
}