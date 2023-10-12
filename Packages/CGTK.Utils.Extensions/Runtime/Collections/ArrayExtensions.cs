using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace CGTK.Utils.Extensions.Collections
{
    public static class ArrayExtensions
    {
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static T[] AddRangeUnique<T>(this T[] array, IEnumerable<T> collection)
        {
            List<T> __list = array.ToList();
            
            foreach (T item in collection)
            {
                if (!__list.Contains(item: item))
                {
                    __list.Add(item: item);
                }
            }

            return __list.ToArray();
        }
        
        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static T[] ClearAllNulls<T>(this T[] array) where T : class
        {
            List<T> __list = array.ToList();
            
            __list.RemoveAll(match: item => item == null);

            return __list.ToArray();
        }
    }
}