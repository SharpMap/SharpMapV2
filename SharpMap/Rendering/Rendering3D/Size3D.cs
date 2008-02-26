// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Rendering.Rendering3D
{
    /// <summary>
    /// A measurement of size in 3 dimensions.
    /// </summary>
	public struct Size3D : IVectorD, IComputable<Double, Size3D>
    {
        private readonly DoubleComponent _width, _height, _depth;
        private readonly Boolean _hasValue;

        public static readonly Size3D Empty = new Size3D();
        public static readonly Size3D Zero = new Size3D(0, 0, 0);

        /// <summary>
        /// Creates a new non-empty ViewSize3D with the given values.
        /// </summary>
        /// <param name="width">Width of the measurement.</param>
        /// <param name="height">Height of the measurement.</param>
        /// <param name="depth">Depth of the measurement.</param>
        public Size3D(DoubleComponent width, DoubleComponent height, DoubleComponent depth)
        {
            _width = width;
            _height = height;
            _depth = depth;
            _hasValue = true;
        }

        public override String ToString()
        {
            return String.Format("[ViewSize3D] Width: {0}, Height: {1}, Depth: {2}", Width, Height, Depth);
        }

        public override Int32 GetHashCode()
        {
            return unchecked(Width.GetHashCode() ^ Height.GetHashCode() ^ Depth.GetHashCode());
        }

        public DoubleComponent Width
        {
            get { return _width; }
        }

        public DoubleComponent Height
        {
            get { return _height; }
        }

        public DoubleComponent Depth
        {
            get { return _depth; }
        }

        public override Boolean Equals(Object obj)
        {
            if (!(obj is Size3D))
            {
                return false;
            }

            return Equals((Size3D)obj);
        }

        public Boolean Equals(Size3D size)
        {
            return _hasValue == size._hasValue
                && Width.Equals(size.Width)
                && Height.Equals(size.Height)
                && Depth.Equals(size.Depth);
        }

        #region IVectorD Members

        public DoubleComponent this[Int32 element]
        {
            get 
            {
                if (element == 0)
                {
                    return _width;
                }
                else if (element == 1)
                {
                    return _height;
                }
                else if (element == 2)
                {
                    return _depth;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("element", element, "Index must be 0, 1 or 2 for a 3D size.");
                }
            }
        }

        public Boolean IsEmpty
        {
            get { return _hasValue; }
        }

        #endregion

        #region ICloneable Members

        public Object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IViewVector> Members

        public Boolean Equals(IVectorD other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<Double> Members

        public IEnumerator<Double> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        public static Boolean operator !=(Size3D size1, Size3D size2)
        {
            return ! (size1 == size2);
        }

        public static Boolean operator ==(Size3D size1, Size3D size2)
        {
            return size1.Equals(size2);
        }

        /// <summary>
        /// Creates a component-by-component copy of the vector.
        /// </summary>
        /// <returns>A copy of the vector.</returns>
        IVectorD IVectorD.Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of components in the vector.
        /// </summary>
        Int32 IVectorD.ComponentCount
        {
            get { return IsEmpty ? 0 : 3; }
        }

        /// <summary>
        /// Gets or sets the vector component array.
        /// </summary>
        DoubleComponent[] IVectorD.Components
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the vector multiplied by -1.
        /// </summary>
        /// <returns>The vector when multiplied by -1.</returns>
        IVectorD IVectorD.Negative()
        {
            throw new NotImplementedException();
        }

        #region IVectorD Members

        /// <summary>
        /// Gets or sets a component in the vector.
        /// </summary>
        /// <param name="index">The index of the component.</param>
        /// <returns>The value of the component at the given <paramref name="index"/>.</returns>
        DoubleComponent IVectorD.this[Int32 index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        /// <summary>
        /// Gets the determinant for the matrix, if it exists.
        /// </summary>
        Double IMatrixD.Determinant
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        Int32 IMatrixD.ColumnCount
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the format of the matrix, either row-major or column-major.
        /// </summary>
        MatrixFormat IMatrixD.Format
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        Boolean IMatrixD.IsSingular
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singular).
        /// </summary>
        Boolean IMatrixD.IsInvertible
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the inverse of the matrix, if one exists.
        /// </summary>
        IMatrixD IMatrixD.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is square (<c>RowCount == ColumnCount != 0</c>).
        /// </summary>
        Boolean IMatrixD.IsSquare
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        Boolean IMatrixD.IsSymmetrical
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        Int32 IMatrixD.RowCount
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets an element in the matrix.
        /// </summary>
        /// <param name="row">The index of the row of the element.</param>
        /// <param name="column">The index of the column of the element.</param>
        /// <returns>The value of the element at the specified row and column.</returns>
        DoubleComponent IMatrixD.this[Int32 row, Int32 column]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Makes an element-by-element copy of the matrix.
        /// </summary>
        /// <returns>An exact copy of the matrix.</returns>
        IMatrixD IMatrixD.Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a submatrix.
        /// </summary>
        /// <param name="rowIndexes">The indexes of the rows to include.</param>
        /// <param name="j0">The starting column to include.</param>
        /// <param name="j1">The ending column to include.</param>
        /// <returns>A submatrix with rows given by <paramref name="rowIndexes"/> and columns <paramref name="j0"/> 
        /// through <paramref name="j1"/>.</returns>
        IMatrixD IMatrixD.GetMatrix(Int32[] rowIndexes, Int32 j0, Int32 j1)
        {
            throw new NotImplementedException();
        }

        #region IMatrix<DoubleComponent> Members

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>The matrix with the rows as columns and columns as rows.</returns>
        IMatrixD IMatrixD.Transpose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the sum of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The sum.</returns>
        IMatrixD IAddable<IMatrixD>.Add(IMatrixD b)
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
        IMatrixD ISubtractable<IMatrixD>.Subtract(IMatrixD b)
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
        IMatrixD INegatable<IMatrixD>.Negative()
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
        IMatrixD IDivisible<IMatrixD>.Divide(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasOne<IMatrixD>.One
        {
            get { throw new NotImplementedException(); }
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
        Boolean IEquatable<IMatrixD>.Equals(IMatrixD other)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IVectorD> Members

        IVectorD INegatable<IVectorD>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IVectorD> Members

        public IVectorD Subtract(IVectorD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IVectorD> Members

        IVectorD IHasZero<IVectorD>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IVectorD> Members

        public IVectorD Add(IVectorD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IVectorD> Members

        IVectorD IDivisible<IVectorD>.Divide(IVectorD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double, IVectorD> Members

        IVectorD IDivisible<Double, IVectorD>.Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IVectorD> Members

        IVectorD IHasOne<IVectorD>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<IVectorD> Members

        public IVectorD Multiply(IVectorD b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IMatrix<DoubleComponent>> Members

        public Int32 CompareTo(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Abs()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrix<DoubleComponent>> Members

        public Boolean GreaterThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Exp()
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Log()
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IVectorD> Members

        IVectorD IComputable<IVectorD>.Abs()
        {
            throw new NotImplementedException();
        }

        IVectorD IComputable<IVectorD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IVectorD> Members

        public Boolean GreaterThan(IVectorD value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(IVectorD value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(IVectorD value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(IVectorD value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IVectorD> Members

        IVectorD IExponential<IVectorD>.Exp()
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Log()
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IVectorD> Members

        public Int32 CompareTo(IVectorD other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Double,IVectorD> Members

        IVectorD IComputable<Double, IVectorD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IVectorD> Members

        public IVectorD Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Double,ViewSize3D> Members

        Size3D IComputable<Double, Size3D>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<ViewSize3D> Members

        Size3D IComputable<Size3D>.Abs()
        {
            throw new NotImplementedException();
        }

        Size3D IComputable<Size3D>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<ViewSize3D> Members

        Size3D INegatable<Size3D>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<ViewSize3D> Members

        public Size3D Subtract(Size3D b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<ViewSize3D> Members

        Size3D IHasZero<Size3D>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<ViewSize3D> Members

        public Size3D Add(Size3D b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<ViewSize3D> Members

        public Size3D Divide(Size3D b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<ViewSize3D> Members

        Size3D IHasOne<Size3D>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<ViewSize3D> Members

        public Size3D Multiply(Size3D b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<ViewSize3D> Members

        public Boolean GreaterThan(Size3D value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(Size3D value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(Size3D value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(Size3D value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<ViewSize3D> Members

        Size3D IExponential<Size3D>.Exp()
        {
            throw new NotImplementedException();
        }

        Size3D IExponential<Size3D>.Log()
        {
            throw new NotImplementedException();
        }

        Size3D IExponential<Size3D>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        Size3D IExponential<Size3D>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        Size3D IExponential<Size3D>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,ViewSize3D> Members

        Size3D IMultipliable<Double, Size3D>.Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,ViewSize3D> Members

        public Size3D Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
