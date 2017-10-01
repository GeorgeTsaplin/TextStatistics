using System;
using System.Collections.Generic;
using System.Linq;

namespace TextStatistics
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(
            this Dictionary<TKey, TValue> @this,
            IDictionary<TKey, TValue> other,
            Func<TValue, TValue, TValue> valueSelector)
        {
            if (other?.Any() != true)
            {
                return;
            }

            var mustMerge = other.Any();
            foreach (var kvp in other)
            {
                TValue current;
                if (mustMerge && @this.TryGetValue(kvp.Key, out current))
                {
                    @this[kvp.Key] = valueSelector(current, kvp.Value);
                }
                else
                {
                    @this.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
