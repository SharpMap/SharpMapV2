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
using System.Collections.Generic;

using NPack;
using NPack.Interfaces;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// A point in 2 dimensional Cartesian space.
    /// </summary>
    [Serializable]
    public struct Point2D : IVectorD, IHasEmpty
    {
        public static readonly Point2D Empty = new Point2D();
        public static readonly Point2D Zero = new Point2D(0, 0);
        public static readonly Point2D One = new Point2D(1, 1);

        private DoubleComponent _x, _y;
        private bool _hasValue;

        #region Constructors
        public Point2D(double x, double y)
        {
            _x = x;
            _y = y;
            _hasValue = true;
        }

        public Point2D(double[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");

            if (elements.Length != 2)
            {
                throw new ArgumentException("Elements array must have only 2 components.");
            }

            _x = elements[0];
            _y = elements[1];
            _hasValue = true;
        }

        public Point2D(IVectorD vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            if (vector.ComponentCount != 2)
            {
                throw new ArgumentException("Elements array must have only 2 components.");
            }

            _x = vector[0];
            _y = vector[1];
            _hasValue = true;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return String.Format("[Point2D] ({0:N3}, {1:N3})", _x, _y);
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            return unchecked((int)_x ^ (int)_y);
        }
        #endregion

        #region Equality Testing

        public static bool operator ==(Point2D lhs, Point2D rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Point2D lhs, Point2D rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static bool operator ==(Point2D lhs, IVectorD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Point2D lhs, IVectorD rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point2D)
            {
                return Equals((Point2D)obj);
            }

            if (obj is IVectorD)
            {
                return Equals(obj as IVectorD);
            }

            return false;
        }

        public bool Equals(Point2D point)
        {
            return _x.Equals(point._x) &&
                _y.Equals(point._y) &&
                IsEmpty == point.IsEmpty;
        }

        #region IEquatable<IViewVector> Members

        public bool Equals(IVectorD other)
        {
            if (other == null)
            {
                return false;
            }

            if ((this as IVectorD).ComponentCount != other.ComponentCount)
            {
                return false;
            }

            if (!_x.Equals(other[0]) || !_y.Equals(other[1]))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region IEquatable<IMatrix<DoubleComponent>> Members

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="other">An object to compare with this object.</param>
        public bool Equals(IMatrixD other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.RowCount != 1 || other.ColumnCount != 2)
            {
                return false;
            }

            if (!other[0, 0].Equals(_x) || !other[0, 1].Equals(_y))
            {
                return false;
            }

            return true;
        }

        #endregion
        #endregion

        #region Properties
        public double X
        {
            get { return (double)_x; }
        }

        public double Y
        {
            get { return (double)_y; }
        }

        public double this[int element]
        {
            get
            {
                checkIndex(element);

                return element == 0 ? (double)_x : (double)_y;
            }
        }

        public bool IsEmpty
        {
            get { return !_hasValue; }
        }
        #endregion

        #region Clone
        public Point2D Clone()
        {
            return new Point2D((double)_x, (double)_y);
        }
        #endregion

        #region Negative
        public Point2D Negative()
        {
            return new Point2D((double)_x.Negative(), (double)_y.Negative());
        }
        #endregion

        #region IEnumerable<double> Members

        public IEnumerator<double> GetEnumerator()
        {
            yield return (double)_x;
            yield return (double)_y;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IVector<DoubleComponent> Members

        MatrixFormat IMatrix<DoubleComponent>.Format
        {
            get { return MatrixFormat.Unspecified; }
        }

        IVectorD IVectorD.Clone()
        {
            return Clone();
        }

        IVectorD IVectorD.Negative()
        {
            return Negative();
        }

        int IVectorD.ComponentCount
        {
            get { return IsEmpty ? 0 : 2; }
        }

        /// <summary>
        /// Gets or sets the vector component array.
        /// </summary>
        DoubleComponent[] IVectorD.Components
        {
            get { return new DoubleComponent[] { _x, _y }; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                checkIndex(value.Length);

                _x = value[0];
                _y = value[1];
            }
        }

        DoubleComponent IVectorD.this[int index]
        {
            get
            {
                checkIndex(index);
                return this[index];
            }
            set
            {
                checkIndex(index);

                if (index == 0)
                {
                    _x = value;
                }
                else
                {
                    _y = value;
                }

                _hasValue = true;
            }
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
            return MatrixProcessor<DoubleComponent>.Instance.Operations.Add(this, b);
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
            return MatrixProcessor<DoubleComponent>.Instance.Operations.Subtract(this, b);
        }

        #endregion

        #region IHasZero<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the additive identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasZero<IMatrixD>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region INegatable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the negative of the object. Must not modify the object itself.
        /// </summary>
        /// <returns>The negative.</returns>
        IMatrixD INegatable<IMatrixD>.Negative()
        {
            return Negative();
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the product of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The product.</returns>
        public IMatrixD Multiply(IMatrixD b)
        {
            return MatrixProcessor<DoubleComponent>.Instance.Operations.Multiply(this, b);
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
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasOne<IMatrixD>.One
        {
            get { return One; }
        }

        #endregion

        #region IEnumerable<DoubleComponent> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<DoubleComponent> IEnumerable<DoubleComponent>.GetEnumerator()
        {
            yield return _x;
            yield return _y;
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
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        bool IMatrixD.IsSingular
        {
            get { return true; }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singluar).
        /// </summary>
        bool IMatrixD.IsInvertible
        {
            get { return false; }
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
            get { return false; }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        bool IMatrixD.IsSymmetrical
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        int IMatrixD.RowCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the elements in the matrix as an array of arrays (jagged array).
        /// </summary>
        DoubleComponent[][] IMatrixD.Elements
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
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
        /// <returns>The value of the element at the given index.</returns>
        DoubleComponent IMatrixD.this[int row, int column]
        {
            get
            {
                checkIndexes(row, column);

                return this[column];
            }
            set
            {
                checkIndexes(row, column);

                (this as IVectorD)[column] = value;
            }
        }

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>The matrix with the rows as columns and columns as rows.</returns>
        IMatrixD IMatrixD.Transpose()
        {
            return
                new Matrix<DoubleComponent>((this as IMatrixD).Format,
                    new DoubleComponent[][] { new DoubleComponent[] { _x }, new DoubleComponent[] { _y } });
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
                throw new ArgumentOutOfRangeException("row", row, "A Point2D has only 1 row.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeException("column", row, "A Point2D has only 2 columns.");
            }
        }
        #endregion

        public static Point2D operator +(Point2D lhs, Point2D rhs)
        {
            return new Point2D(lhs.Add(rhs));
        }

        public static Point2D operator -(Point2D lhs, Point2D rhs)
        {
            return new Point2D(lhs.Subtract(rhs));
        }

        #region INegatable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> INegatable<IVector<DoubleComponent>>.Negative()
        {
            return new Point2D(-X, -Y);
        }

        #endregion

        #region ISubtractable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Subtract(IVector<DoubleComponent> b)
        {
            if (b == null) throw new ArgumentNullException("b");

            if (b.ComponentCount != 2)
            {
                throw new ArgumentException("Vector must have only 2 components.");
            }

            return new Point2D(X - (double)b[0], Y - (double)b[1]);
        }

        #endregion

        #region IHasZero<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IHasZero<IVector<DoubleComponent>>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Add(IVector<DoubleComponent> b)
        {
            if (b == null) throw new ArgumentNullException("b");

            if (b.ComponentCount != 2)
            {
                throw new ArgumentException("Vector must have only 2 components.");
            }

            return new Point2D(X - (double)b[0], Y - (double)b[1]);
        }

        #endregion

        #region IDivisible<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Divide(IVector<DoubleComponent> b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IHasOne<IVector<DoubleComponent>>.One
        {
            get { return new Point2D(1, 1); }
        }

        #endregion

        #region IMultipliable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Multiply(IVector<DoubleComponent> b)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
