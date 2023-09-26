using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math
{
	using F32 = Single;

	namespace Math
	{
		using static Trigonometry.Math.Trigonometry;

		public static partial class Matrix4x4Extensions
		{

			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Vector3 Position(in this Matrix4x4 m)
				=> new Vector3(x: m[0, 3], y: m[1, 3], z: m[2, 3]);

			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Vector3 Scale(this Matrix4x4 m)
				=> new Vector3(x: m.GetColumn(0).magnitude, y: m.GetColumn(1).magnitude, z: m.GetColumn(2).magnitude);

			[PublicAPI]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Quaternion Rotation(in this Matrix4x4 m)
			{
				Vector3 __scale = Scale(m);

				// Normalize Scale from Matrix4x4
				F32 m00 = m[0, 0] / __scale.x;
				F32 m01 = m[0, 1] / __scale.y;
				F32 m02 = m[0, 2] / __scale.z;
				F32 m10 = m[1, 0] / __scale.x;
				F32 m11 = m[1, 1] / __scale.y;
				F32 m12 = m[1, 2] / __scale.z;
				F32 m20 = m[2, 0] / __scale.x;
				F32 m21 = m[2, 1] / __scale.y;
				F32 m22 = m[2, 2] / __scale.z;

				Quaternion q = new Quaternion
				{
					w = Mathf.Sqrt(f: (1 + m00 + m11 + m22).AtLeastZero()) / 2,
					x = Mathf.Sqrt(f: (1 + m00 - m11 - m22).AtLeastZero()) / 2,
					y = Mathf.Sqrt(f: (1 - m00 + m11 - m22).AtLeastZero()) / 2,
					z = Mathf.Sqrt(f: (1 - m00 - m11 + m22).AtLeastZero()) / 2
				};

				q.x *= Mathf.Sign(f: q.x * (m21 - m12));
				q.y *= Mathf.Sign(f: q.y * (m02 - m20));
				q.z *= Mathf.Sign(f: q.z * (m10 - m01));

				// q.Normalize()
				F32 __qMagnitude = (q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z).Sqrt();
				q.w /= __qMagnitude;
				q.x /= __qMagnitude;
				q.y /= __qMagnitude;
				q.z /= __qMagnitude;

				return q;
			}
		}
	}
}