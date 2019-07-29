using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Helpers
{
    static class CollectionExtentions
    {
        public static V PutIfAbsent<K, V>(this IDictionary<K, V> self, K key, V value) where V : class
        {
            V oldValue;
            if (!self.TryGetValue(key, out oldValue))
            {
                self.Add(key, value);
                return value;
            }
            return oldValue;
        }

        public static V PutIfAbsent<K, V>(this IDictionary<K, V> self, K key, Func<V> ifAbsent) where V : class
        {
            V value;
            if (!self.TryGetValue(key, out value))
            {
                self.Add(key, value = ifAbsent());
                return value;
            }
            return value;
        }

        public static T[] AddToArray<T>(this T[] array, T value)
        {
            var len = array.Length;
            var result = new T[len + 1];
            Array.Copy(array, result, len);
            result[len] = value;
            return result;
        }

        public static T[] AddToArray<T>(this T[] array, params T[] values)
        {
            var len = array.Length;
            var valueLen = values.Length;
            var result = new T[len + valueLen];
            Array.Copy(array, result, len);
            Array.Copy(values, 0, result, len, valueLen);
            return result;
        }


        public static T[] AddToArray<T>(this T[] array, IEnumerable<T> values) => AddToArray(array, values.ToArray());

        public static T[] RemoveFromArray<T>(this T[] array, T value)
        {
            var list = array.ToList();
            return list.Remove(value) ? list.ToArray() : array;
        }

        public static T[] RemoveAllFromArray<T>(this T[] array, Predicate<T> match)
        {
            var list = array.ToList();
            return list.RemoveAll(match) > 0 ? list.ToArray() : array;
        }

        public static bool ContainsElementOfType<T>(this T[] array, Type type)
        {
            return array.ToList().Any(e => e.GetType() == type);
        }
    }
}
