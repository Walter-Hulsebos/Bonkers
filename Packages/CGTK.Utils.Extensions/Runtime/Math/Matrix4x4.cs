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
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Vector3 Position(in this Matrix4x4 m)
				=> new Vector3(x: m[row: 0, column: 3], y: m[row: 1, column: 3], z: m[row: 2, column: 3]);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Vector3 Scale(this Matrix4x4 m)
				=> new Vector3(x: m.GetColumn(index: 0).magnitude, y: m.GetColumn(index: 1).magnitude, z: m.GetColumn(index: 2).magnitude);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static Quaternion Rotation(in this Matrix4x4 m)
			{
				Vector3 __scale = Scale(m: m);

				// Normalize Scale from Matrix4x4
				F32 m00 = m[row: 0, column: 0] / __scale.x;
				F32 m01 = m[row: 0, column: 1] / __scale.y;
				F32 m02 = m[row: 0, column: 2] / __scale.z;
				F32 m10 = m[row: 1, column: 0] / __scale.x;
				F32 m11 = m[row: 1, column: 1] / __scale.y;
				F32 m12 = m[row: 1, column: 2] / __scale.z;
				F32 m20 = m[row: 2, column: 0] / __scale.x;
				F32 m21 = m[row: 2, column: 1] / __scale.y;
				F32 m22 = m[row: 2, column: 2] / __scale.z;

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