using System.Runtime.CompilerServices;
using Unity.Mathematics;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

using Vector2Int = UnityEngine.Vector2Int;
using Vector3Int = UnityEngine.Vector3Int;


namespace ProjectDawn.Mathematics
{
    /// <summary>
    /// A static class to contain various math functions.
    /// </summary>
    public static partial class math2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 asvector3(this float2 value) => new Vector3(value.x, value.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 asvector4(this float2 value) => new Vector4(value.x, value.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 asvector4(this float3 value) => new Vector4(value.x, value.y, value.z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 asvector3(this double2 value) => new Vector3((float)value.x, (float)value.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 asvector4(this double2 value) => new Vector4((float)value.x, (float)value.y, 0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 asvector4(this double3 value) => new Vector4((float)value.x, (float)value.y, (float)value.z, 0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int asvector3(this int2 value) => new Vector3Int(value.x, value.y, 0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 asfloat3(this float2 value) => new float3(value.x, value.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 asfloat4(this float2 value) => new float4(value.x, value.y, 0, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 asfloat4(this float3 value) => new float4(value.x, value.y, value.z, 0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 asdouble3(this double2 value) => new double3(value.x, value.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 asdouble4(this double2 value) => new double4(value.x, value.y, 0, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 asdouble4(this double3 value) => new double4(value.x, value.y, value.z, 0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 asint3(this int2 value) => new int3(value.x, value.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 asint4(this int2 value) => new int4(value.x, value.y, 0, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 asint4(this int3 value) => new int4(value.x, value.y, value.z, 0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 asfloat(this Vector2 value) => value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 asfloat(this Vector3 value) => value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 asfloat(this Vector4 value) => value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 asint(this Vector2Int value) => new int2(x: value.x, y: value.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 asint(this Vector3Int value) => new int3(x: value.x, y: value.y, z: value.z);
        //There is no Vector4Int, so we can't do that.
    }
}
