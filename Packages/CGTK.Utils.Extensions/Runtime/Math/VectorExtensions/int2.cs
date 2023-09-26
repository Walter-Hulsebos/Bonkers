using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace CGTK.Utils.Extensions.Math.VectorExtensions
{
	using I32 = Int32;

	namespace Math
	{
		public static partial class VectorExtensions
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static I32[] ToArray(in this int2 self)
				=> new I32[2]
				{
					self.x,
					self.y,
				};

		}
	}
}