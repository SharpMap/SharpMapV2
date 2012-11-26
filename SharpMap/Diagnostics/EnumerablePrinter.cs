using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GeoAPI.SystemCoreReplacement;

namespace SharpMap.Diagnostics
{
    /// <summary>
    /// Utility class to print a series of items
    /// </summary>
    public static class EnumerablePrinter
    {
        /// <summary>
        /// Prints a series of <paramref name="enumerable">items</paramref> using the <see cref="object.ToString()"/> method
        /// </summary>
        /// <param name="enumerable">The items</param>
        /// <returns>A <see cref="string"/></returns>
        public static String Print(IEnumerable enumerable)
        {
            return Print(enumerable, o => o.ToString());
        }

        /// <summary>
        /// Prints a series of <paramref name="enumerable">items</paramref> using the <paramref name="decorator"/> method
        /// </summary>
        /// <param name="enumerable">The items</param>
        /// <param name="decorator">The function to convert from <see cref="object"/> to <see cref="string"/></param>
        /// <returns>A <see cref="string"/></returns>
        public static String Print(IEnumerable enumerable, Func<object, string> decorator)
        {
            var buffer = new StringBuilder();

            foreach (var value in enumerable)
            {
                buffer.Append(decorator(value));
                buffer.Append(", ");
            }

            if (buffer.Length >= 2)
            {
                buffer.Length -= 2;
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Prints a series of <paramref name="enumerable"><typeparamref name="T"/> items</paramref> using the <see cref="object.ToString()"/> method
        /// </summary>
        /// <param name="enumerable">The items</param>
        /// <returns>A string</returns>
        public static String Print<T>(IEnumerable<T> enumerable)
        {
            return Print(enumerable, o => o.ToString());
        }

        /// <summary>
        /// Prints a series of <paramref name="enumerable">items</paramref> using the <paramref name="decorator"/> method
        /// </summary>
        /// <param name="enumerable">The items</param>
        /// <param name="decorator">The function to convert from <typeparamref name="T"/> to <see cref="string"/></param>
        /// <returns>A <see cref="string"/></returns>
        public static String Print<T>(IEnumerable<T> enumerable, Func<T, string> decorator)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (decorator == null)
                throw new ArgumentNullException("decorator");

            var buffer = new StringBuilder();

            foreach (var value in enumerable)
            {
                buffer.Append(decorator(value));
                buffer.Append(", ");
            }

            if (buffer.Length >= 2)
            {
                buffer.Length -= 2;
            }

            return buffer.ToString();
        }
    }
}