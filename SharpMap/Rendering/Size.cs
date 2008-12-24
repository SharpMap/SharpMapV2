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

namespace SharpMap.Rendering
{
    /// <summary>
    /// Represents the size of an object in scene coordinates.
    /// </summary>
    [Serializable]
    public struct Size<TCoordinate> : IVectorD, 
                                      //IHasEmpty, 
                                      IComparable<Size<TCoordinate>>, 
                                      IComputable<Double, Size<TCoordinate>>
    {
        private DoubleComponent _width, _height;
        private Boolean _hasValue;

        public static readonly Size<TCoordinate> Empty = new Size<TCoordinate>();
        private static readonly Size<TCoordinate> _zero = new Size<TCoordinate>(0, 0);
        public static readonly Size<TCoordinate> Unit = new Size<TCoordinate>(1, 1);

        #region Constructors
        public Size(Double width, Double height)
        {
            _width = width;
            _height = height;
            _hasValue = true;
        }
        #endregion

        #region ToString

        public override String ToString()
        {
            return IsEmpty
                ? "Empty"
                : String.Format("Width: {0}, Height: {1}", Width, Height);
        }
        #endregion

        #region GetHashCode
        public override Int32 GetHashCode()
        {
            return unchecked(Width.GetHashCode() ^ Height.GetHashCode());
        }
        #endregion

        #region Properties
        public Double Width
        {
            get { return (Double)_width; }
        }

        public Double Height
        {
            get { return (Double)_height; }
        }

        public Double this[Int32 element]
        {
            get
            {
                checkIndex(element);

                return element == 0 ? (Double)_width : (Double)_height;
            }
        }

        public Boolean IsEmpty
        {
            get { return !_hasValue; }
        }

        public Int32 ComponentCount
        {
            get { return 2; }
        }

        public DoubleComponent[] Components
        {
            get { return new DoubleComponent[] { _width, _height }; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                checkIndex(value.Length);

                _width = value[0];
                _height = value[1];
            }
        }

        #endregion

        public void GetComponents(out DoubleComponent a, out DoubleComponent b)
        {
            a = _width;
            b = _height;
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b, out DoubleComponent c)
        {
            GetComponents(out a, out b);
            c = Double.NaN;
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b, out DoubleComponent c, out DoubleComponent d)
        {
            GetComponents(out a, out b, out c);
            d = Double.NaN;
        }

        public Size<TCoordinate> Add(Size<TCoordinate> size)
        {
            if(IsEmpty)
            {
                return size;
            }

            if(size.IsEmpty)
            {
                return this;
            }

            return new Size<TCoordinate>(Width + size.Width, Height + size.Height);
        }

        public Size<TCoordinate> Subtract(Size<TCoordinate> size)
        {
            return Add(size.Negative());
        }

        public Int32 CompareTo(IMatrixD other)
        {
            throw new System.NotImplementedException();
        }

        public Int32 CompareTo(IVectorD other)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Divide(Size<TCoordinate> b)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Multiply(Size<TCoordinate> b)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Add(Double b)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Subtract(Double b)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Multiply(Double b)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Divide(Double b)
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Zero
        {
            get { return _zero; }
        }

        public Size<TCoordinate> One
        {
            get { throw new System.NotImplementedException(); }
        }

        public Boolean GreaterThan(Size<TCoordinate> value)
        {
            throw new System.NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(Size<TCoordinate> value)
        {
            throw new System.NotImplementedException();
        }

        public Boolean LessThan(Size<TCoordinate> value)
        {
            throw new System.NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(Size<TCoordinate> value)
        {
            throw new System.NotImplementedException();
        }

        #region Operators
        public static Size<TCoordinate> operator + (Size<TCoordinate> lhs, Size<TCoordinate> rhs)
        {
            return lhs.Add(rhs);
        }

        public static Size<TCoordinate> operator -(Size<TCoordinate> lhs, Size<TCoordinate> rhs)
        {
            return lhs.Subtract(rhs);
        }

        public static Size<TCoordinate> operator *(Size<TCoordinate> factor, Double multiplier)
        {
            if(factor.IsEmpty)
            {
                return factor;
            }

            return new Size<TCoordinate>(factor.Width * multiplier, factor.Height * multiplier);
        }
        #endregion

        #region Equality Testing
        public static Boolean operator ==(Size<TCoordinate> lhs, Size<TCoordinate> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Size<TCoordinate> lhs, Size<TCoordinate> rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Boolean operator ==(Size<TCoordinate> lhs, IVectorD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Size<TCoordinate> lhs, IVectorD rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Size<TCoordinate>)
            {
                return Equals((Size<TCoordinate>)obj);
            }

            if (obj is IVectorD)
            {
                return Equals(obj as IVectorD);
            }

            return false;
        }

        public Boolean Equals(Size<TCoordinate> size)
        {
            return _width.Equals(size._width) &&
                _height.Equals(size._height) &&
                IsEmpty == size.IsEmpty;
        }

        #region IEquatable<IViewVector> Members

        public Boolean Equals(IVectorD other)
        {
            if (other == null)
            {
                return false;
            }

            if (ComponentCount != other.ComponentCount)
            {
                return false;
            }

            if (!_width.Equals(other[0]) || !_height.Equals(other[1]))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region IEquatable<IMatrixD> Members

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
            if (other == null)
            {
                return false;
            }

            if (other.RowCount != 1 || other.ColumnCount != 2)
            {
                return false;
            }

            if (!other[0, 0].Equals(_width) || !other[0, 1].Equals(_height))
            {
                return false;
            }

            return true;
        }

        #endregion
        #endregion

        public Size<TCoordinate> Clone()
        {
            return new Size<TCoordinate>((Double)_width, (Double)_height);
        }

        public IMatrixD GetMatrix(Int32[] rowIndexes, Int32 startColumn, Int32 endColumn)
        {
            throw new System.NotImplementedException();
        }

        public IMatrixD Transpose()
        {
            throw new System.NotImplementedException();
        }

        public Size<TCoordinate> Negative()
        {
            return new Size<TCoordinate>(-((Double)_width), -((Double)_height));
        }

        #region IComparable<Size<TCoordinate>> Members

        public Int32 CompareTo(Size<TCoordinate> other)
        {
            if (other.Equals(this))
            {
                return 0;
            }

            if (other.Width < Width || other.Height < Height)
            {
                return -1;
            }

            return 1;
        }

        #endregion

        #region IEnumerable<Double> Members

        public IEnumerator<Double> GetEnumerator()
        {
            yield return (Double)_width;
            yield return (Double)_height;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IVectorD Members

        IVectorD IAddable<Double, IVectorD>.Add(Double b)
        {
            return Add(b);
        }

        IVectorD ISubtractable<Double, IVectorD>.Subtract(Double b)
        {
            return Subtract(b);
        }

        IVectorD IMultipliable<Double, IVectorD>.Multiply(Double b)
        {
            return Multiply(b);
        }

        MatrixFormat IMatrixD.Format
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

        DoubleComponent IVectorD.this[Int32 index]
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
                    _width = value;
                }
                else
                {
                    _height = value;
                }

                _hasValue = true;
            }
        }

        #endregion

        #region IHasZero<IMatrixD> Members

        /// <summary>
        /// Returns the additive identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasZero<IMatrixD>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region INegatable<IMatrixD> Members

        /// <summary>
        /// Returns the negative of the object. Must not modify the object itself.
        /// </summary>
        /// <returns>The negative.</returns>
        IMatrixD INegatable<IMatrixD>.Negative()
        {
            return Negative();
        }

        #endregion

        #region IHasOne<IMatrixD> Members

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasOne<IMatrixD>.One
        {
            get { return Unit; }
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
            yield return _width;
            yield return _height;
        }

        #endregion

        #region IMatrixD Members
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
        Double IMatrixD.Determinant
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        Int32 IMatrixD.ColumnCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        Boolean IMatrixD.IsSingular
        {
            get { return true; }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singular).
        /// </summary>
        Boolean IMatrixD.IsInvertible
        {
            get { return false; }
        }

        public IMatrixD Inverse
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is square (<c>RowCount == ColumnCount != 0</c>).
        /// </summary>
        Boolean IMatrixD.IsSquare
        {
            get { return false; }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        Boolean IMatrixD.IsSymmetrical
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        Int32 IMatrixD.RowCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets or sets an element in the matrix.
        /// </summary>
        /// <param name="row">The index of the row of the element.</param>
        /// <param name="column">The index of the column of the element.</param>
        /// <returns>The value of the element at the given index.</returns>
        DoubleComponent IMatrixD.this[Int32 row, Int32 column]
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

        #endregion

        #region INegatable<IVectorD> Members

        IVectorD INegatable<IVectorD>.Negative()
        {
            return new Size<TCoordinate>(-Width, -Height);
        }

        #endregion

        #region ISubtractable<IVectorD> Members

        IVectorD ISubtractable<IVectorD>.Subtract(IVectorD b)
        {
            if (b == null) throw new ArgumentNullException("b");

            if (b.ComponentCount != 2)
            {
                throw new ArgumentException("Vector must have only 2 components.");
            }

            return new Size<TCoordinate>(Width - (Double)b[0], Height - (Double)b[1]);
        }

        #endregion

        #region IHasZero<IVectorD> Members

        IVectorD IHasZero<IVectorD>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region IAddable<IVectorD> Members

        IVectorD IAddable<IVectorD>.Add(IVectorD b)
        {
            if (b == null) throw new ArgumentNullException("b");

            if (b.ComponentCount != 2)
            {
                throw new ArgumentException("Vector must have only 2 components.");
            }

            return new Size<TCoordinate>(Width + (Double)b[0], Height + (Double)b[1]);
        }

        #endregion

        #region IDivisible<IVectorD> Members

        IVectorD IDivisible<IVectorD>.Divide(IVectorD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDivisible<Double, IVectorD> Members

        IVectorD IDivisible<Double, IVectorD>.Divide(Double b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IVectorD> Members

        IVectorD IHasOne<IVectorD>.One
        {
            get { return new Size<TCoordinate>(1, 1); }
        }

        #endregion

        #region IMultipliable<IVectorD> Members

        IVectorD IMultipliable<IVectorD>.Multiply(IVectorD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        IMatrixD ISubtractable<IMatrixD>.Subtract(IMatrixD b)
        {
            throw new System.NotImplementedException();
        }

        IMatrixD IAddable<IMatrixD>.Add(IMatrixD b)
        {
            throw new System.NotImplementedException();
        }

        IMatrixD IDivisible<IMatrixD>.Divide(IMatrixD b)
        {
            throw new System.NotImplementedException();
        }

        IMatrixD IMultipliable<IMatrixD>.Multiply(IMatrixD b)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.GreaterThan(IMatrixD value)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.GreaterThanOrEqualTo(IMatrixD value)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.LessThan(IMatrixD value)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.LessThanOrEqualTo(IMatrixD value)
        {
            throw new System.NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Power(Double exponent)
        {
            return ((IExponential<Size<TCoordinate>>)this).Power(exponent);
        }

        Size<TCoordinate> IExponential<Size<TCoordinate>>.Sqrt()
        {
            throw new System.NotImplementedException();
        }

        Size<TCoordinate> IExponential<Size<TCoordinate>>.Log(Double newBase)
        {
            throw new System.NotImplementedException();
        }

        Size<TCoordinate> IExponential<Size<TCoordinate>>.Log()
        {
            throw new System.NotImplementedException();
        }

        Size<TCoordinate> IExponential<Size<TCoordinate>>.Exp()
        {
            throw new System.NotImplementedException();
        }

        Size<TCoordinate> IExponential<Size<TCoordinate>>.Power(Double exponent)
        {
            throw new System.NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Sqrt()
        {
            return ((IExponential<Size<TCoordinate>>)this).Sqrt();
        }

        IVectorD IExponential<IVectorD>.Log(Double newBase)
        {
            return ((IExponential<Size<TCoordinate>>)this).Log(newBase);
        }

        IVectorD IExponential<IVectorD>.Log()
        {
            return ((IExponential<Size<TCoordinate>>)this).Log();
        }

        IVectorD IExponential<IVectorD>.Exp()
        {
            return ((IExponential<Size<TCoordinate>>)this).Exp();
        }

        IVectorD IExponential<IVectorD>.Power(Double exponent)
        {
            return ((IExponential<Size<TCoordinate>>)this).Power(exponent);
        }

        IMatrixD IExponential<IMatrixD>.Sqrt()
        {
            return ((IExponential<Size<TCoordinate>>)this).Sqrt();
        }

        IMatrixD IExponential<IMatrixD>.Log(Double newBase)
        {
            return ((IExponential<Size<TCoordinate>>)this).Log(newBase);
        }

        IMatrixD IExponential<IMatrixD>.Log()
        {
            return ((IExponential<Size<TCoordinate>>)this).Log();
        }

        IMatrixD IExponential<IMatrixD>.Exp()
        {
            return ((IExponential<Size<TCoordinate>>)this).Exp();
        }

        IMatrixD IComputable<IMatrixD>.Abs()
        {
            return ((IComputable<Size<TCoordinate>>)this).Abs();
        }

        Size<TCoordinate> IComputable<Double, Size<TCoordinate>>.Set(Double value)
        {
            return ((IComputable<Size<TCoordinate>>)this).Set(value);
        }

        Size<TCoordinate> IComputable<Size<TCoordinate>>.Set(Double value)
        {
            return ((IComputable<Size<TCoordinate>>)this).Set(value);
        }

        Size<TCoordinate> IComputable<Size<TCoordinate>>.Abs()
        {
            throw new System.NotImplementedException();
        }

        IVectorD IComputable<Double, IVectorD>.Set(Double value)
        {
            return ((IComputable<Size<TCoordinate>>)this).Set(value);
        }

        IVectorD IComputable<IVectorD>.Set(Double value)
        {
            return ((IComputable<Size<TCoordinate>>)this).Set(value);
        }

        IVectorD IComputable<IVectorD>.Abs()
        {
            return ((IComputable<Size<TCoordinate>>)this).Abs();
        }

        IMatrixD IComputable<IMatrixD>.Set(Double value)
        {
            return ((IComputable<Size<TCoordinate>>)this).Set(value);
        }

        Boolean IBooleanComparable<IVectorD>.GreaterThan(IVectorD value)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.GreaterThanOrEqualTo(IVectorD value)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.LessThan(IVectorD value)
        {
            throw new System.NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.LessThanOrEqualTo(IVectorD value)
        {
            throw new System.NotImplementedException();
        }

        #region Private Helper Methods

        private static void checkIndex(Int32 index)
        {
            if (index != 0 && index != 1)
            {
                throw new ArgumentOutOfRangeException("index", index, "The element index must be either 0 or 1 for a 2D point.");
            }
        }

        private static void checkIndexes(Int32 row, Int32 column)
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
    }
}
