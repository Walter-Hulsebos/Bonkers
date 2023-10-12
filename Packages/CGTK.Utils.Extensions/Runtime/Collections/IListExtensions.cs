using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

using JetBrains.Annotations;

using Unity.Mathematics;

using Random = System.Random;

namespace CGTK.Utils.Extensions.Collections
{
    using F32 = Single;
    using F64 = Double;
    
    [PublicAPI]
    public static class IListExtensions
    {
        private static readonly Random DefaultRNG = new Random();
        
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static T RandomElement<T>(this IList<T> list, in Random RNG = null)
            => (list.Count == 0) ? default : list.ElementAt(index: (RNG ?? DefaultRNG).Next(maxValue: list.Count));
        
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static float2 ToFloat2(this IList<Single> value)
        {
            return new float2
            {
                x = value[index: 0],
                y = value[index: 1],
            };
        }
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static float3 ToFloat3(this IList<Single> value)
        {
            return new float3
            {
                x = value[index: 0],
                y = value[index: 1],
                z = value[index: 2],
            };
        }
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static float4 ToFloat4(this IList<Single> value)
        {
            return new float4
            {
                x = value[index: 0],
                y = value[index: 1],
                z = value[index: 2],
                w = value[index: 3],
            };
        }
        
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static int2 ToInt2(this IList<Int32> value)
        {
            return new int2
            {
                x = value[index: 0],
                y = value[index: 1],
            };
        }
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static int3 ToInt3(this IList<Int32> value)
        {
            return new int3
            {
                x = value[index: 0],
                y = value[index: 1],
                z = value[index: 2],
            };
        }
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static int4 ToInt4(this IList<Int32> value)
        {
            return new int4
            {
                x = value[index: 0],
                y = value[index: 1],
                z = value[index: 2],
                w = value[index: 3],
            };
        }
        
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static quaternion ToQuaternion(this IList<Single> value)
        {
            return new quaternion
            {
                value = value.ToFloat4()
            };
        }
        
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static IList<T> AddRangeUnique<T>(this IList<T> list, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                if (!list.Contains(item: item))
                {
                    list.Add(item: item);
                }
            }
            
            return list;
        }
        
    }
}
