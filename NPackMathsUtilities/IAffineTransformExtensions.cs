using System;
//using AGG;
//using AGG.Transform;
using NPack.Interfaces;

namespace NPack
{


   

    public static class IAffineTransformExtensions
    {
        public static void Transform<T>(this IAffineTransformMatrix<T> m, ref T x, ref T y)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {

            IVector<T> v1 = m.TransformVector(MatrixFactory<T>.CreateVector2D(x, y));

            x = v1[0];
            y = v1[1];
        }



        public static void Transform<T>(this IAffineTransformMatrix<T> m, ref IVector<T> vector)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            vector = m.TransformVector(vector);
        }

        public static bool IsValid<T>(this IAffineTransformMatrix<T> m)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            //todo:
            throw new NotImplementedException();
        }

        public static T TranslationX<T>(this IAffineTransformMatrix<T> m)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            if (m.ColumnCount != 3)
                throw new InvalidOperationException();
            if (m.Format == MatrixFormat.RowMajor)
                return m[2, 0];
            else
                return m[0, 2];
        }

        public static T TranslationY<T>(this IAffineTransformMatrix<T> m)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            if (m.ColumnCount != 3)
                throw new InvalidOperationException();
            if (m.Format == MatrixFormat.RowMajor)
                return m[2, 1];
            else
                return m[1, 2];
        }

        //public static void Normalize<T>(this IVector<T> v)
        //    where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible
        //{
        //    throw new NotImplementedException();
        //}



    }
}
