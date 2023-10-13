using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace CGTK.Utils.Extensions.Math.VectorExtensions
{
	using F32 = Single;

	namespace Math
	{
		public static partial class VectorExtensions
		{
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32[] ToArray(in this float2 self)
				=> new F32[2]
				{
					self.x,
					self.y,
				};

		}
	}
}