using System.Collections.Generic;

namespace wyn.core.Utils
{
    internal static class DictionaryExtension
    {
        internal static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            if (map.ContainsKey(key))
            {
                map[key] = value;
            }
            else
            {
                map.Add(key, value);
            }
        }
    }
}
