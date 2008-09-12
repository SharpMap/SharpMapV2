using NPack.Interfaces;
using System;

namespace NPack
{
    public static class MatrixUtilities<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        public static void CopyInto(IMatrix<T> src, IMatrix<T> dest)
        {
            if (src.Format != dest.Format)
                throw new ArgumentException("Matrix Format mismatch");

            if (src.RowCount != dest.RowCount || src.ColumnCount != dest.ColumnCount)
                throw new ArgumentException("Matrix dimension mismatch");

            for (int i = 0; i < src.RowCount; i++)
            {
                for (int j = 0; j < src.ColumnCount; j++)
                    dest[i, j] = src[i, j];
            }
        }
    }


}
