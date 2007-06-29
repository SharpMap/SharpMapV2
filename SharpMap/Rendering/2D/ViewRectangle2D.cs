// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using NPack;
using NPack.Interfaces;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    [Serializable]
    public struct ViewRectangle2D : IViewRectangle<ViewPoint2D>, IComparable<ViewRectangle2D>, IHasEmpty
    {
        public static readonly ViewRectangle2D Empty = new ViewRectangle2D();
        public static readonly ViewRectangle2D Zero = new ViewRectangle2D(0, 0, 0, 0);

        private DoubleComponent _bottom;
        private DoubleComponent _left;
        private DoubleComponent _right;
        private DoubleComponent _top;
        private bool _hasValue;

        #region Constructors

        public ViewRectangle2D(double left, double right, double top, double bottom)
        {
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
            _hasValue = true;
        }

        public ViewRectangle2D(ViewPoint2D location, ViewSize2D size)
        {
            _left = location.X;
            _top = location.Y;
            _right = _left + size.Width;
            _bottom = _top + size.Height;
            _hasValue = true;
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return
                String.Format(
                    "[ViewRectangle2D] Left: {0:N3}; Top: {1:N3}; Right: {2:N3}; Bottom: {3:N3}; IsEmpty: {4}", Left,
                    Top, Right, Bottom, IsEmpty);
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            return unchecked((int)Left ^ (int)Right ^ (int)Top ^ (int)Bottom);
        }
        #endregion

        #region IComparable<ViewRectangle2D> Members

        /// <summary>
        /// Compares this <see cref="ViewRectangle2D"/> instance with another instance.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="other">ViewRectangle2D to perform comparison with.</param>
        /// <returns>
        /// Returns 0 if the <see cref="ViewRectangle2D"/> instances intersect each other,
        /// 1 if this rectangle is located to the right or upward from the <paramref name="other"/>
        /// rectangle, and -1 if this rectangle is located to the left or downward from the other.
        /// </returns>
        public int CompareTo(ViewRectangle2D other)
        {
            if (other.IsEmpty && IsEmpty)
            {
                return 0;
            }

            if (Intersects(other))
            {
                return 0;
            }

            if (IsEmpty || other.Left > Right || other.Bottom > Top)
            {
                return -1;
            }

            return 1;
        }

        #endregion

        public bool IsEmpty
        {
            get { return !_hasValue; }
        }

        /// <summary>
        /// Determines whether this <see cref="ViewRectangle2D"/> intersects another.
        /// </summary>
        /// <param name="rectangle"><see cref="ViewRectangle2D"/> to check intersection with.</param>
        /// <returns>True if there is intersection, false if not.</returns>
        public bool Intersects(ViewRectangle2D rectangle)
        {
            if (IsEmpty || rectangle.IsEmpty)
            {
                return false;
            }

            return !(rectangle.Left > Right ||
                     rectangle.Right < Left ||
                     rectangle.Bottom < Top ||
                     rectangle.Top > Bottom);
        }

        internal static ViewRectangle2D FromLTRB(double left, double top, double right, double bottom)
        {
            return new ViewRectangle2D(left, right, top, bottom);
        }

        #region Equality Testing

        public bool Equals(IMatrixD other)
        {
            if(other is ViewRectangle2D)
            {
                return Equals((ViewRectangle2D) other);
            }

            if (IsEmpty && other == null)
            {
                return true;
            }

            if (IsEmpty || other == null)
            {
                return false;
            }

            IMatrixD thisMatrix = this;
            DoubleComponent[][] lhs = thisMatrix.Elements, rhs = other.Elements;

            if (lhs.Length != rhs.Length)
            {
                return false;
            }

            if (lhs.Rank != rhs.Rank)
            {
                return false;
            }

            if (thisMatrix.RowCount != other.RowCount || thisMatrix.ColumnCount != other.ColumnCount)
            {
                return false;
            }

            unchecked
            {
                for (int row = 0; row < thisMatrix.RowCount; row++)
                {
                    for (int col = 0; col < thisMatrix.ColumnCount; col++)
                    {
                        if (!lhs[row][col].Equals(rhs[row][col]))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool operator ==(ViewRectangle2D lhs, ViewRectangle2D rhs)
        {
            return (lhs.Equals(rhs));
        }

        public static bool operator !=(ViewRectangle2D lhs, ViewRectangle2D rhs)
        {
            return !(lhs.Equals(rhs));
        }

        public override bool Equals(object obj)
        {
            if (obj is ViewRectangle2D)
            {
                return Equals((ViewRectangle2D)obj);
            }

            if (obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
            }

            return false;
        }

        public bool Equals(ViewRectangle2D rectangle)
        {
            return IsEmpty == rectangle.IsEmpty &&
                   Left == rectangle.Left &&
                   Right == rectangle.Right &&
                   Top == rectangle.Top &&
                   Bottom == rectangle.Bottom;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the X value of the coordinate of the upper left corner of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        public double X
        {
            get { return (double)_left; }
        }

        /// <summary>
        /// Gets the Y value of the coordinate of the upper left corner of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        public double Y
        {
            get { return (double)_top; }
        }

        /// <summary>
        /// Gets the X value of the left edge of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Top"/>
        public double Left
        {
            get { return (double)_left; }
        }

        /// <summary>
        /// Gets the Y value of the top edge of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Left"/>
        public double Top
        {
            get { return (double)_top; }
        }

        /// <summary>
        /// Gets the X value of the right edge of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Top"/>
        /// <seealso cref="Left"/>
        public double Right
        {
            get { return (double)_right; }
        }

        /// <summary>
        /// Gets the Y value of the bottom edge of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Right"/>
        /// <seealso cref="Top"/>
        /// <seealso cref="Left"/>
        public double Bottom
        {
            get { return (double)_bottom; }
        }

        /// <summary>
        /// Gets the coordinates of the center of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        /// <seealso cref="X"/>
        /// <seealso cref="Y"/>
        public ViewPoint2D Center
        {
            get { return new ViewPoint2D(X + Width/2, Y + Height/2); }
        }

        /// <summary>
        /// Gets the coordinates of the upper left corner of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Center"/>
        public ViewPoint2D Location
        {
            get { return new ViewPoint2D((double)_left, (double)_top); }
        }

        /// <summary>
        /// Gets the size of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Width"/>
        /// <seealso cref="Height"/>
        public ViewSize2D Size
        {
            get { return new ViewSize2D(Width, Height); }
        }

        /// <summary>
        /// Gets the width of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public double Width
        {
            get { return (double)_right.Subtract(_left); }
        }

        /// <summary>
        /// Gets the height of the <see cref="ViewRectangle2D"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public double Height
        {
            get { return (double)_bottom.Subtract(_top); }
        }

        #endregion

        public ViewRectangle2D Clone()
        {
            return new ViewRectangle2D(Location, Size);
        }

        #region IViewRectangle<ViewPoint2D> Members

        public ViewPoint2D LowerBounds
        {
            get { return Location; }
        }

        public ViewPoint2D UpperBounds
        {
            get { return new ViewPoint2D(X + Width, Y + Height); }
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the sum of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The sum.</returns>
        public IMatrixD Add(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the difference of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The difference.</returns>
        public IMatrixD Subtract(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the additive identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasZero<IMatrixD>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region INegatable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the negative of the object. Must not modify the object itself.
        /// </summary>
        /// <returns>The negative.</returns>
        public IMatrixD Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the product of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The product.</returns>
        IMatrixD IMultipliable<IMatrixD>.Multiply(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the quotient of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The quotient.</returns>
        public IMatrixD Divide(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        /// <value>e</value>
        public IMatrixD One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMatrix<DoubleComponent> Members

        /// <summary>
        /// Makes an element-by-element copy of the matrix.
        /// </summary>
        /// <returns>An exact copy of the matrix.</returns>
        IMatrixD IMatrixD.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Gets the determinant for the matrix, if it exists.
        /// </summary>
        double IMatrixD.Determinant
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        int IMatrixD.ColumnCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the elements in the matrix as an array of arrays (jagged array).
        /// </summary>
        DoubleComponent[][] IMatrixD.Elements
        {
            get
            {
                if (IsEmpty)
                {
                    return new DoubleComponent[0][];
                }

                return new DoubleComponent[][] { new DoubleComponent[] { _left, _top }, new DoubleComponent[] { _right, _bottom } };
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length != 2)
                {
                    throw new ArgumentException("Elements can be set only to a 2x2 array");
                }

                if (value[0] == null || value[1] == null)
                {
                    throw new ArgumentException("The array elements of value cannot be null.");
                }

                if (value[0].Length != 2 || value[1].Length != 2)
                {
                    throw new ArgumentException("Elements can be set only to a 2x2 array");
                }

                _left = value[0][0].LessThan(value[1][0]) ? value[0][0] : value[1][0];
                _top = value[0][1].LessThan(value[1][1]) ? value[0][1] : value[1][1];
                _right = value[0][0].LessThan(value[1][0]) ? value[1][0] : value[0][0];
                _bottom = value[0][1].LessThan(value[1][1]) ? value[1][1] : value[0][1];
                _hasValue = true;
            }
        }

        /// <summary>
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        bool IMatrixD.IsSingular
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singular).
        /// </summary>
        bool IMatrixD.IsInvertable
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the inverse of the matrix, if one exists.
        /// </summary>
        IMatrixD IMatrixD.Inverse
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is square (<c>RowCount == ColumnCount != 0</c>).
        /// </summary>
        bool IMatrixD.IsSquare
        {
            get { return true; }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        bool IMatrixD.IsSymmetrical
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        int IMatrixD.RowCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets a submatrix.
        /// </summary>
        /// <param name="rowIndexes">The indexes of the rows to include.</param>
        /// <param name="j0">The starting column to include.</param>
        /// <param name="j1">The ending column to include.</param>
        /// <returns></returns>
        IMatrixD IMatrixD.GetMatrix(int[] rowIndexes, int j0, int j1)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets an element in the matrix.
        /// </summary>
        /// <param name="row">The index of the row of the element.</param>
        /// <param name="column">The index of the column of the element.</param>
        /// <returns></returns>
        DoubleComponent IMatrixD.this[int row, int column]
        {
            get 
            {
                checkIndexes(row, column);
                
                if(row == 0)
                {
                    if(column == 0)
                    {
                        return _left;
                    }
                    else
                    {
                        return _top;
                    }
                }
                else
                {
                    if (column == 0)
                    {
                        return _right;
                    }
                    else
                    {
                        return _bottom;
                    }
                }
            }
            set
            {
                checkIndexes(row, column);

                if (row == 0)
                {
                    if (column == 0)
                    {
                        _left = value;
                    }
                    else
                    {
                        _top = value;
                    }
                }
                else
                {
                    if (column == 0)
                    {
                        _right = value;
                    }
                    else
                    {
                        _bottom = value;
                    }
                }

                _hasValue = true;
            }
        }

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>The matrix with the rows as columns and columns as rows.</returns>
        IMatrixD IMatrixD.Transpose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helper Methods

        private static void checkIndex(int index)
        {
            if (index != 0 && index != 1)
            {
                throw new ArgumentOutOfRangeException("index", index, "The element index must be either 0 or 1 for a 2D point.");
            }
        }

        private static void checkIndexes(int row, int column)
        {
            if (row != 0)
            {
                throw new ArgumentOutOfRangeException("row", row, "A ViewPoint2D has only 1 row.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeException("column", row, "A ViewPoint2D has only 2 columns.");
            }
        }
        #endregion
    }
}