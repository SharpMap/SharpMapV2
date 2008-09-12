using NPack.Interfaces;

namespace System
{
    /// <summary>
    /// Utility class to save typing..
    /// </summary>
    public static class M
    {
        public static T New<T>(double val)
          where T : IComputable<T>//, IEquatable<T>, IComparable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return default(T).Set(val);
        }

        public static T Zero<T>()
            where T : IHasZero<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return default(T).Zero;
        }

        public static T One<T>()
            where T : IHasOne<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return default(T).One;
        }

        public static T PI<T>()
            where T : IComputable<T> //IEquatable<T>, IComparable<T>, , IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return default(T).Set(Math.PI);
        }

        /// <summary>
        /// returns (dx * dx) + (dy * dy)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static T LengthSquared<T>(T dx, T dy, T dz)
             where T : IAddable<T>, IMultipliable<T>, IComputable<T>//   IEquatable<T>, IComparable<T>,  IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return dx.Multiply(dx).Add(dy.Multiply(dy)).Add(dz.Multiply(dz));
        }
        public static T LengthSquared<T>(T dx, T dy)
             where T : IAddable<T>, IMultipliable<T>, IComputable<T> //IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return dx.Multiply(dx).Add(dy.Multiply(dy));
        }

        /// <summary>
        /// returns Math.Sqrt((dx * dx) + (dy * dy))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static T Length<T>(T dx, T dy, T dz)
              where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return LengthSquared<T>(dx, dy, dz).Sqrt();
        }

        public static T Length<T>(T dx, T dy)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return dx.Multiply(dx).Add(dy.Multiply(dy)).Sqrt();
        }

        public static T Atan2<T>(T y, T x)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return M.New<T>(Math.Atan2(y.ToDouble(), x.ToDouble()));
            //throw new NotImplementedException();
        }

        public static T Min<T>(T val1, T val2)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val1.LessThan(val2) ? val1 : val2;
        }

        public static T Max<T>(T val1, T val2)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val1.GreaterThan(val2) ? val1 : val2;
        }
    }

    public static class MathExtensions
    {

        public static string ToString<T>(this T val, string format)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.ToDouble().ToString(format);
        }
        public static string ToString<T>(this T val)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.ToDouble().ToString();
        }

        public static T AddEquals<T>(this T val, T other)
            where T : IAddable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Add(other));
            return val;
        }

        public static T SubtractEquals<T>(this T val, T other)
            where T : ISubtractable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Subtract(other));
            return val;
        }

        public static T MultiplyEquals<T>(this T val, T other)
                where T : IMultipliable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Multiply(other));
            return val;
        }

        public static T DivideEquals<T>(this T val, T other)
            where T : IDivisible<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Divide(other));
            return val;
        }


        public static T AddEquals<T>(this T val, double other)
             where T : IAddable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Add(other));
            return val;
        }

        public static T SubtractEquals<T>(this T val, double other)
            where T : ISubtractable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Subtract(other));
            return val;
        }

        public static T MultiplyEquals<T>(this T val, double other)
                where T : IMultipliable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Multiply(other));
            return val;
        }

        public static T DivideEquals<T>(this T val, double other)
            where T : IDivisible<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            val.SetFrom(val.Divide(other));
            return val;
        }

        public static bool LessThan<T>(this T val, double other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, ICommonNumericalOperations<T>
        {
            return val.LessThan(M.New<T>(other));
        }

        public static bool LessThanOrEqualTo<T>(this T val, double other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.LessThanOrEqualTo(M.New<T>(other));
        }

        public static bool GreaterThan<T>(this T val, double other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.GreaterThan(M.New<T>(other));
        }

        public static bool GreaterThanOrEqualTo<T>(this T val, double other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.GreaterThanOrEqualTo(M.New<T>(other));
        }

        public static bool Equals<T>(this T val, double other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.Equals(M.New<T>(other));
        }

        public static bool NotEqual<T>(this T val, double other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return !val.Equals(M.New<T>(other));
        }

        public static bool Equals<T>(this T val, T other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.Equals(other);
        }

        public static bool NotEqual<T>(this T val, T other)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return !val.Equals(other);
        }

        //public static V Cast<T, V>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
        //    where V : IConvertible
        //{
        //    return (V)val;
        //}

        //public static int ToInt<T>(this T val)
        //     where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T> , ITrigonometryOperations<T>  
        //{
        //    return val.ToInt(); 
        //}

        //public static double ToDouble<T>(this T val)
        //     where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    return val.ToDouble();
        //}

        //public static T Ceiling<T>(this T val) where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    return M.New<T>(Math.Ceiling(val.ToDouble()));
        //}

        //public static T Floor<T>(this T val) where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    return M.New<T>(Math.Floor(val.ToDouble()));
        //}

        //public static V Round<T, V>(this T val) where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    throw new NotImplementedException();
        //    //return (V)M.New<T>(Math.Floor(val.ToDouble()));
        //}

        //public static T Cos<T>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    //todo
        //    throw new NotImplementedException();
        //    //return default(T).Set((double)Math.Cos(val));
        //}

        //public static T Sin<T>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    //todo
        //    throw new NotImplementedException();
        //}

        //public static T Tan<T>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    //todo
        //    throw new NotImplementedException();
        //}
        //public static T Atan<T>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    //todo
        //    throw new NotImplementedException();
        //}

        //public static T Asin<T>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    throw new NotImplementedException();
        //}

        //public static T Acos<T>(this T val)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonOperations<T>, ITrigonometryOperations<T>
        //{
        //    throw new NotImplementedException();
        //}


        public static T Multiply<T>(this T val, double dbl)
            where T : IMultipliable<T>, IComputable<T> // IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.Multiply(M.New<T>(dbl));
        }

        public static T Divide<T>(this T val, double dbl)
            where T : IDivisible<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.Divide(M.New<T>(dbl));
        }

        public static T Add<T>(this T val, double dbl)
            where T : IAddable<T>, IComputable<T>//   IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.Add(M.New<T>(dbl));
        }

        public static T Subtract<T>(this T val, double dbl)
            where T : ISubtractable<T>, IComputable<T>//  IEquatable<T>, IComparable<T>, , IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return val.Subtract(M.New<T>(dbl));
        }
    }
}
