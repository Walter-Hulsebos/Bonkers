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
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static I32[] ToArray(in this int4 self)
				=> new I32[4]
				{
					self.x,
					self.y,
					self.z,
					self.w,
				};

		}
	}
}