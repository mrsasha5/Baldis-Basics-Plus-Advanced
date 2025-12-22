using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Extensions
{
    public static class PrimitiveTypeExtensions
    {
        public static T GetRandomElement<T>(this List<T> list)
        {
            int index = Random.Range(0, list.Count);
            T element = list[index];

            return element;
        }

        public static T GetRandomElementAndRemove<T>(this List<T> list)
        {
            int index = Random.Range(0, list.Count);
            T element = list[index];
            list.Remove(element);

            return element;
        }

        public static bool ToBool(this int value)
        {
            return value > 0;
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

    }
}
