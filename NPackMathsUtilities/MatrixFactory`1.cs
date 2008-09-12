using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPack.Interfaces;

namespace NPack
{
    public enum VectorDimension : int
    {
        Two = 2,
        Three = 3
    }

    public static class MatrixFactory<T>
                    where T : IComputable<T>, IEquatable<T>, IComparable<T>, IConvertible, IFormattable //IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {


        public static IAffineTransformMatrix<T> CreateAffine(IMatrix<T> from)
        {
            if (!from.IsSquare)
                throw new ArgumentException("from matrix should be square");

            if (from.ColumnCount < 3 || from.ColumnCount > 4)
                throw new ArgumentException("from matrix should be square and have a row count of 3 for two dimensions or 4 for three dimensions");

            return new AffineMatrix<T>(from);
        }

        public static IAffineTransformMatrix<T> NewIdentity(VectorDimension dimension)
        {
            return new AffineMatrix<T>(
                MatrixFormat.ColumnMajor,
                dimension == VectorDimension.Two ? 3 : 4);
                //3);
        }

        public static IAffineTransformMatrix<T> NewIdentity(int rows)
        {
            return new AffineMatrix<T>(
                MatrixFormat.ColumnMajor,
                rows);
        }

        public static IVector<T> CreateZeroVector(VectorDimension dimension)
        {
            return new Vector<T>((int)dimension);
        }


        public static IVector<T> CreateVector2D(T x, T y)
        {
            return new Vector<T>(x, y, M.One<T>());
        }

        public static IVector<T> CreateVector2D(double x, double y)
        {
            return new Vector<T>(M.New<T>(x), M.New<T>(y), M.One<T>());
        }

        public static IVector<T> CreateVector3D(T x, T y, T z)
        {
            return new Vector<T>(x, y, z, M.One<T>());
        }

        public static IVector<T> CreateVector3D(double x, double y, double z)
        {
            return new Vector<T>(M.New<T>(x), M.New<T>(y), M.New<T>(z), M.One<T>());
        }

        public static IAffineTransformMatrix<T> NewScaling2D(T dx, T dy)
        {
            return MatrixFactory<T>.NewScaling(MatrixFactory<T>.CreateVector2D(dx, dy));
        }

        public static IAffineTransformMatrix<T> NewScaling3D(T dx, T dy, T dz)
        {
            return MatrixFactory<T>.NewScaling(MatrixFactory<T>.CreateVector3D(dx, dy, dz));
        }

        public static IAffineTransformMatrix<T> NewScaling(IVector<T> scaleVector)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(scaleVector.RowCount);
            a.Scale(scaleVector);
            return a;
        }

        public static IAffineTransformMatrix<T> NewScaling(VectorDimension dimension, T s)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(dimension);
            a.Scale(s);
            return a;
        }

        public static IAffineTransformMatrix<T> NewRotation(VectorDimension dimension, double r)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(dimension);
            a.RotateAlong(MatrixFactory<T>.CreateZeroVector(dimension), r);
            return a;
        }


        public static IAffineTransformMatrix<T> NewRotation(IVector<T> axis, double r)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(axis.RowCount);
            a.RotateAlong(axis, r);
            return a;
        }

        public static IAffineTransformMatrix<T> NewTranslation(double dx, double dy)
        {
            return NewTranslation(M.New<T>(dx), M.New<T>(dy));
        }
        public static IAffineTransformMatrix<T> NewTranslation(T dx, T dy)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            a.Translate(MatrixFactory<T>.CreateVector2D(dx, dy));
            return a;
        }

        public static IAffineTransformMatrix<T> NewTranslation(double dx, double dy, double dz)
        {
            return NewTranslation(M.New<T>(dx), M.New<T>(dy), M.New<T>(dz));
        }

        public static IAffineTransformMatrix<T> NewTranslation(T dx, T dy, T dz)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(VectorDimension.Three);
            a.Translate(MatrixFactory<T>.CreateVector3D(dx, dy, dz));
            return a;
        }

        public static IAffineTransformMatrix<T> NewTranslation(IVector<T> translationVector)
        {
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(translationVector.RowCount);
            a.Translate(translationVector, MatrixOperationOrder.Append);
            return a;
        }

    }
}
