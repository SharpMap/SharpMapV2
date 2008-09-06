using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPack.Interfaces;

namespace NPack
{
    public static class VectorUtilities<T>
        where T : IAddable<T>, IMultipliable<T>, IComputable<T>, IEquatable<T>, IComparable<T>, IConvertible, IFormattable
    {
        public static void CopyInto(IVector<T> src, IVector<T> dest)
        {
            if (src.ComponentCount != dest.ComponentCount)
                throw new ArgumentException("ComponentCount  mismatch");

            for (int i = 0; i < src.ComponentCount; i++)
                dest.Components[i] = src.Components[i];
        }

        public static IVector<T> Cross(IVector<T> a, IVector<T> b)
        {
            return a.Cross(b);
        }
    }

    public static class IVectorExtensions
    {
        public static T GetMagnitude<T>(this IVector<T> vector)
              where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {

            T p = M.Zero<T>();

            for (int i = 0; i < vector.ComponentCount; i++)
            {
                p.AddEquals(vector[i].Multiply(vector[i]));
            }

            return p.Sqrt();
        }

        public static IVector<T> Normalize<T>(this IVector<T> vector)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            T length = vector.GetMagnitude();

            if (length.NotEqual(0))
                return MatrixFactory<T>.CreateVector3D(vector[0].Divide(length), vector[1].Divide(length), vector[2].Divide(length));

            return MatrixFactory<T>.CreateZeroVector(VectorDimension.Three);
        }

        public static IVector<T> GetPerpendicular<T>(this IVector<T> v)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            throw new NotImplementedException();
        }

        public static T GetMagnitudeSquared<T>(this IVector<T> v)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            if (v.RowCount == 2)
                return M.LengthSquared(v[0], v[1]);
            else if (v.RowCount == 3)
                return M.LengthSquared(v[0], v[1], v[2]);

            throw new NotImplementedException();
        }


        public static void Set<T>(this IVector<T> v, T x, T y)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            v[0] = x;
            v[1] = y;
        }

        public static void Set<T>(this IVector<T> v, T x, T y, T z)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            v[0] = x;
            v[1] = y;
            v[2] = z;
        }

        public static T Dot<T>(this IVector<T> v1, IVector<T> v2)
            where T : IAddable<T>, IMultipliable<T>, IComputable<T>, IEquatable<T>, IComparable<T>, IConvertible
        {
            if (v1.ComponentCount != v2.ComponentCount)
                throw new ArgumentException("Dimension Mismatch");

            T retVal = M.New<T>(0);

            for (int i = 0; i < v1.ComponentCount; i++)
                retVal.AddEquals(v1[i].Multiply(v2[i]));

            return retVal;
        }


        public static IVector<T> Cross<T>(this IVector<T> v1, IVector<T> v2)
            where T : IAddable<T>, IMultipliable<T>, IComputable<T>, IEquatable<T>, IComparable<T>, IConvertible, IFormattable
        {
            if (v1.ComponentCount != v2.ComponentCount)
                throw new ArgumentException("Dimension Mismatch");

            if (v1.ComponentCount != 3)
                throw new NotImplementedException();

            T a1 = v1[0], a2 = v1[1], a3 = v1[2], b1 = v2[0], b2 = v2[1], b3 = v2[2];

            return MatrixFactory<T>.CreateVector3D(
                a2.Multiply(b3).Subtract(a3.Multiply(b3)),
                a3.Multiply(b1).Subtract(a1.Multiply(b3)),
                a1.Multiply(b2).Subtract(a2.Subtract(b1)));

        }
    }
}
