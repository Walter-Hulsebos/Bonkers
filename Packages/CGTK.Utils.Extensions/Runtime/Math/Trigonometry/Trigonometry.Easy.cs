using System;
using System.Runtime.CompilerServices;
using CGTK.Utils.Extensions.Math.Math;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions.Math.Trigonometry
{
	using F32 = Single;

	namespace Math
	{
		public static partial class Trigonometry
		{
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SqrDistance(in this Vector3 from, in Vector3 to)
				=> (to - from).sqrMagnitude;

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 SqrDistance(this Transform from, in Transform to)
				=> (to.position - from.position).sqrMagnitude;
			
			/// <summary>
			/// Faster than Vector3.Distance, exact same result.
			/// </summary>
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Distance(in this Vector3 from, in Vector3 to)
				=> (to - from).magnitude;

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 Distance(this Transform from, in Transform to)
				=> (to.position - from.position).magnitude;
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 AsDegrees(in this F32 angle)
				=> (angle * Constants.Constants.RAD_TO_DEG);

			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 AsRadians(in this F32 angle)
				=> (angle * Constants.Constants.DEG_TO_RAD);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ToDegrees(ref this F32 angle)
				=> (angle *= Constants.Constants.RAD_TO_DEG);
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 ToRadians(ref this F32 angle)
				=> (angle *= Constants.Constants.DEG_TO_RAD);
			
			
			//TODO: Try to get these to work with Tuples and then as extesnion method.
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 GetAngle(in F32? opposite = null, in F32? adjacent = null, in F32? hypotenuse = null)
			{
				Boolean __hasOpposite   = (opposite != null);
				Boolean __hasAdjacent   = (adjacent != null);
				Boolean __hasHypotenuse = (hypotenuse != null);

				if (__hasOpposite && __hasHypotenuse)
				{
					return Asin(value: opposite / hypotenuse).ClampNeg1To1();
				}

				if (__hasAdjacent && __hasHypotenuse)
				{
					return Acos(value: adjacent / hypotenuse).ClampNeg1To1();
				}

				if (__hasOpposite && __hasAdjacent)
				{
					return Atan(value: opposite / adjacent);
				}
				
				//TODO: Error
				return 0;
			}

			//done
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 GetHypotenuse(in F32? opposite = null, in F32? adjacent = null, in F32? angle = null)
			{
				Boolean __hasOpposite = (opposite != null);
				Boolean __hasAdjacent = (adjacent != null);
				Boolean __hasAngle    = (angle != null);

				if (__hasOpposite && __hasAdjacent)
				{
					return Sqrt(value: adjacent.Squared() + opposite.Squared());
				}
				
				if (__hasAngle && __hasAdjacent)
				{
					return (F32)(adjacent / Cos(value: angle));
				}

				if (__hasAngle && __hasOpposite)
				{
					return (F32)(opposite / Sin(value: angle));
				}


				//TODO: Error
				return 0;
			}
			
			//done
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 GetAdjacent(in F32? opposite = null, in F32? hypotenuse = null, in F32? angle = null)
			{
				Boolean __hasOpposite   = (opposite != null);
				Boolean __hasHypotenuse = (hypotenuse != null);
				Boolean __hasAngle      = (angle != null);

				if (__hasOpposite && __hasHypotenuse)
				{
					F32 __result = (opposite.Squared() - hypotenuse.Squared()).Sqrt();

					__result += 1;

					return __result;
				}
				
				if (__hasAngle && __hasOpposite)
				{
					return (F32)(opposite / Tan(value: angle));
				}

				if (__hasAngle && __hasHypotenuse)
				{
					return (F32)(Cos(value: angle) * hypotenuse);
				}

				//TODO: Error
				return 0;
			}
			
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static F32 GetOpposite(in F32? adjacent = null, in F32? hypotenuse = null, in F32? angle = null)
			{
				Boolean __hasAdjacent   = (adjacent != null);
				Boolean __hasHypotenuse = (hypotenuse != null);
				Boolean __hasAngle      = (angle != null);

				if (__hasAngle && __hasAdjacent)
				{
					return (F32)(Tan(value: angle) * adjacent);
				}

				if (__hasAngle && __hasHypotenuse)
				{
					return (F32)(Sin(value: angle) * hypotenuse);
				}

				if (__hasAdjacent && __hasHypotenuse)
				{
					return Sqrt(value: hypotenuse.Squared() - adjacent.Squared());
				}

				//TODO: Error
				return 0;
			}
		}
	}
}