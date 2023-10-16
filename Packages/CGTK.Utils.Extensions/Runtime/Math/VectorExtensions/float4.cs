using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Mathematics;

namespace CGTK.Utils.Extensions.Math.VectorExtensions
{
	using F32 = Single;

	namespace Math
	{
		[PublicAPI]
		public static partial class VectorExtensions
		{
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32[] ToArray(in this float4 self)
				=> new F32[4]
				{
					self.x,
					self.y,
					self.z,
					self.w,
				};

		}
	}
}