using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public static class DictionaryExtensions
    {
        //public static Dictionary<string, T> TryAddOrUpdate<T>(this Dictionary<string, T> dict, string key, T val)
        public static IDictionary<TKey, TValue> TryAddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key,
            TValue val
        )
        {
            if (dict.ContainsKey(key))
                dict[key] = val;
            else
                dict.Add(key, val);

            return dict;
        }

        //public static IDictionary<TKey, TValue> GetDesiredLanguage<TKey, TValue>(this IDictionary<TKey, TValue> dict, string language)
        //{
        //    return (IDictionary<TKey, TValue>)dict.Where(kvp.Key.ToString() == language);
        //}
    }
}
