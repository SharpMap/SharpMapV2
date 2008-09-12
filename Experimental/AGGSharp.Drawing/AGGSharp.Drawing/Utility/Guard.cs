using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AGGSharp.Utility
{
    public static class Guard
    {
        public static void LessThan<T>(T obj, T compare) where T : IComparable<T>
        {
            if (obj.CompareTo(compare) >= 0)
                throw new ArgumentException();
        }

        public static void LessThanOrEqual<T>(T obj, T compare) where T : IComparable<T>
        {
            if (obj.CompareTo(compare) > 0)
                throw new ArgumentException();
        }

        public static void GreaterThan<T>(T obj, T compare) where T : IComparable<T>
        {
            if (obj.CompareTo(compare) <= 0)
                throw new ArgumentException();
        }

        public static void GreaterThanOrEqual<T>(T obj, T compare) where T : IComparable<T>
        {
            if (obj.CompareTo(compare) < 0)
                throw new ArgumentException();
        }

        public static void EqualTo<T>(T obj, T compare) where T : IComparable<T>
        {
            if (obj.CompareTo(compare) != 0)
                throw new ArgumentException();
        }

        public static void NotEqualTo<T>(T obj, T compare) where T : IComparable<T>
        {
            if (obj.CompareTo(compare) == 0)
                throw new ArgumentException();
        }

        public static void ReferenceEqual<T>(T obj, T compare) where T : IComparable<T>
        {
            if (!object.ReferenceEquals(obj, compare))
                throw new ArgumentException();
        }
    }
}
