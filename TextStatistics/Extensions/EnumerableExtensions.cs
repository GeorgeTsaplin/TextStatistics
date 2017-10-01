using System;
using System.Collections.Generic;

namespace TextStatistics
{
    public static class EnumerableExtensions
    {
        public static void ForEachEx<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var item in @this)
            {
                action(item);
            }
        }
    }
}
