using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace SFH {
    public static class EnumerableExtensions {

        #region Getting items

        public static T RandomItem<T>(this IEnumerable<T> enumerabe) {
            if (enumerabe.IsNullOrEmpty())
                throw new ArgumentOutOfRangeException(nameof(enumerabe), $"{nameof(enumerabe)} cannot be null or empty");
            return enumerabe.ElementAt(Random.Range(0, enumerabe.Count()));
        }

        public static T GetLast<T>(this IEnumerable<T> enumerabe) {
            return enumerabe.Last();
        }

        #endregion

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerabe) {
            return enumerabe == null || !enumerabe.Any();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerabe) {
            return !enumerabe.Any();
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) {
            T[] array = enumerable.ToArray();
            for (int i = array.Length - 1; i >= 0; i--) {
                int swapIndex = Random.Range(0,array.Length);
                yield return array[swapIndex];
                array[swapIndex] = array[i];
            }
        }

        #region List

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Random.Range(0,n+1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #endregion

        #region Array

        public static void Shuffle<T>(this T[] array) {
            int n = array.Length;
            while (n > 1) {
                int k = Random.Range(0,n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        #endregion

        #region Output

        public static string ToString<T>(this IEnumerable<T> enumerable, string separator = ", ", string encapsulate = "\"") {
            bool useEncapsulation = encapsulate.Length > 0;
            StringBuilder stringBuilder = new StringBuilder();

            foreach (T element in enumerable) {
                if (stringBuilder.Length > 0) {
                    stringBuilder.Append(separator);
                }
                if (useEncapsulation) {
                    stringBuilder.Append(encapsulate);
                }
                stringBuilder.Append(element);
                if (useEncapsulation) {
                    stringBuilder.Append(encapsulate);
                }
            }

            return stringBuilder.ToString();
        }

        public static void Print<T>(this IEnumerable<T> enumerable, string separator = ", ", string encapsulate = "\"") {
            UnityEngine.Debug.Log(enumerable.ToString(separator, encapsulate));
        }

        #endregion

    }
}