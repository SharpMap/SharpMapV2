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
using GeoAPI.Coordinates;
using NPack;
using NPack.Interfaces;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    /// <summary>
    /// A point in scene coordinate space.
    /// </summary>
    [Serializable]
    public struct Point<TCoordinate> : IVectorD, IComputable<Double, Point<TCoordinate>>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        public static readonly Point<TCoordinate> Empty = new Point<TCoordinate>();
        //public static readonly Point<TCoordinate> Zero = new Point<TCoordinate>(0, 0);
        //public static readonly Point<TCoordinate> One = new Point<TCoordinate>(1, 1);

        public static Point<TCoordinate> operator +(Point<TCoordinate> lhs, Point<TCoordinate> rhs)
        {
            return new Point<TCoordinate>(lhs._coordinate.Factory as ICoordinateFactory<TCoordinate>, lhs.Add(rhs));
        }

        public static Point<TCoordinate> operator -(Point<TCoordinate> lhs, Point<TCoordinate> rhs)
        {
            return new Point<TCoordinate>(lhs._coordinate.Factory as ICoordinateFactory<TCoordinate>, lhs.Subtract(rhs));
        }

        private readonly TCoordinate _coordinate;
        private Boolean _hasValue;

        #region Constructors
        public Point(TCoordinate coordinate)
        {
            _coordinate = coordinate;
            _hasValue = true;
        }

        public Point(ICoordinateFactory<TCoordinate> coordinateFactory, Double x, Double y)
        {
            _coordinate = coordinateFactory.Create(x, y);
            _hasValue = true;
        }

        public Point(ICoordinateFactory<TCoordinate> coordinateFactory, Double[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");

            if (elements.Length != 2 || elements.Length != 3)
            {
                throw new ArgumentException("Elements array must have only 2 or 3 components.");
            }

            _coordinate = coordinateFactory.Create(elements);
            _hasValue = true;
        }

        public Point(ICoordinateFactory<TCoordinate> coordinateFactory, IVectorD vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            _coordinate = coordinateFactory.Create(vector);
            _hasValue = true;
        }
        #endregion

        #region ToString
        public override String ToString()
        {
            return _coordinate.ToString();
        }
        #endregion

        #region GetHashCode
        public override Int32 GetHashCode()
        {
            return _coordinate.GetHashCode() ^ 3;
        }
        #endregion

        #region Equality Testing

        public static Boolean operator ==(Point<TCoordinate> lhs, Point<TCoordinate> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Point<TCoordinate> lhs, Point<TCoordinate> rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Boolean operator ==(Point<TCoordinate> lhs, IVectorD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Point<TCoordinate> lhs, IVectorD rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Point<TCoordinate>)
            {
                return Equals((Point<TCoordinate>)obj);
            }

            if (obj is IVectorD)
            {
                return Equals(obj as IVectorD);
            }

            return false;
        }

        public Boolean Equals(Point<TCoordinate> point)
        {
            return _coordinate.Equals(point._coordinate) &&
                    IsEmpty == point.IsEmpty;
        }

        #region IEquatable<IViewVector> Members

        public Boolean Equals(IVectorD other)
        {
            return _coordinate.Equals(other);
        }

        #endregion

        #region IEquatable<IMatrixD> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type. 
        /// </summary>
        ///
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the other parameter; otherwise, <see langword="false"/>.
        /// </returns>
        ///
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals(IMatrixD other)
        {
            return _coordinate.Equals(other);
        }

        #endregion
        #endregion

        #region Properties

        public TCoordinate Coordinate
        {
            get { return _coordinate; }
        }

        public Double X
        {
            get { return _coordinate[Ordinates.X]; }
        }

        public Double Y
        {
            get { return _coordinate[Ordinates.Y]; }
        }

        public Double Z
        {
            get { return _coordinate[Ordinates.Z]; }
        }

        public Double this[Int32 element]
        {
            get
            {
                checkIndex(element);

                return (Double)_coordinate[element];
            }
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b)
        {
            a = X;
            b = Y;
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b, out DoubleComponent c)
        {
            a = X;
            b = Y;
            c = Z;
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b, out DoubleComponent c, out DoubleComponent d)
        {
            GetComponents(out a, out b, out c);
            d = 1;
        }

        public Boolean IsEmpty
        {
            get { return !_hasValue; }
        }
        #endregion

        #region Clone
        public Point<TCoordinate> Clone()
        {
            return new Point<TCoordinate>(_coordinate.Clone());
        }
        #endregion

        #region Negative
        public Point<TCoordinate> Negative()
        {
            return new Point<TCoordinate>(((IVector<DoubleComponent, TCoordinate>)_coordinate).Negative());
        }
        #endregion

        #region IComputable<Double,Point<TCoordinate>> Members

        public Point<TCoordinate> Abs()
        {
            throw new NotImplementedException();
        }

        Point<TCoordinate> IComputable<Double, Point<TCoordinate>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Point<TCoordinate>> Members

        public Point<TCoordinate> Subtract(Point<TCoordinate> b)
        {
            return new Point<TCoordinate>(_coordinate.Subtract(b._coordinate));
        }

        #endregion

        #region IHasZero<Point<TCoordinate>> Members

        Point<TCoordinate> IHasZero<Point<TCoordinate>>.Zero
        {
            get { return new Point<TCoordinate>(_coordinate.Zero); }
        }

        #endregion

        #region IAddable<Point<TCoordinate>> Members

        public Point<TCoordinate> Add(Point<TCoordinate> b)
        {
            return new Point<TCoordinate>(_coordinate.Add(b._coordinate));
        }

        #endregion

        #region IDivisible<Point<TCoordinate>> Members

        public Point<TCoordinate> Divide(Point<TCoordinate> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<Point<TCoordinate>> Members

        Point<TCoordinate> IHasOne<Point<TCoordinate>>.One
        {
            get { return new Point<TCoordinate>(((IHasOne<TCoordinate>)_coordinate).One); }
        }

        #endregion

        #region IMultipliable<Point<TCoordinate>> Members

        public Point<TCoordinate> Multiply(Point<TCoordinate> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,Point<TCoordinate>> Members

        public Point<TCoordinate> Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<Point<TCoordinate>> Members

        public Boolean GreaterThan(Point<TCoordinate> value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(Point<TCoordinate> value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(Point<TCoordinate> value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(Point<TCoordinate> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<Point<TCoordinate>> Members

        Point<TCoordinate> IExponential<Point<TCoordinate>>.Exp()
        {
            throw new NotImplementedException();
        }

        Point<TCoordinate> IExponential<Point<TCoordinate>>.Log()
        {
            throw new NotImplementedException();
        }

        Point<TCoordinate> IExponential<Point<TCoordinate>>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        public Point<TCoordinate> Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        public Point<TCoordinate> Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<Double> Members

        public IEnumerator<Double> GetEnumerator()
        {
            foreach (DoubleComponent component in _coordinate)
            {
                yield return (Double)component;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IVectorD Members

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

        Int32 IVectorD.ComponentCount
        {
            get { return IsEmpty ? 0 : _coordinate.ComponentCount; }
        }

        /// <summary>
        /// Gets or sets the vector component array.
        /// </summary>
        DoubleComponent[] IVectorD.Components
        {
            get { return ((IVectorD)_coordinate).Components; }
            set
            {
                //if (value == null)
                //{
                //    throw new ArgumentNullException("value");
                //}

                //checkIndex(value.Length - 1);

                //_x = value[0];
                //_y = value[1];
                throw new NotSupportedException();
            }
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
                //checkIndex(index);

                //if (index == 0)
                //{
                //    _x = value;
                //}
                //else
                //{
                //    _y = value;
                //}

                //_hasValue = true;
                throw new NotSupportedException();
            }
        }

        #endregion

        #region IAddable<IMatrixD> Members

        /// <summary>
        /// Returns the sum of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The sum.</returns>
        IMatrixD IAddable<IMatrixD>.Add(IMatrixD b)
        {
            //return MatrixProcessor.Add(this, b);
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IMatrixD> Members

        /// <summary>
        /// Returns the difference of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The difference.</returns>
        IMatrixD ISubtractable<IMatrixD>.Subtract(IMatrixD b)
        {
            //return MatrixProcessor.Subtract(this, b);
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IMatrixD> Members

        /// <summary>
        /// Returns the additive identity.
        /// </summary>
        IMatrixD IHasZero<IMatrixD>.Zero
        {
            get { return new Point<TCoordinate>(_coordinate.Zero); }
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

        #region IMultipliable<IMatrixD> Members

        /// <summary>
        /// Returns the product of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The product.</returns>
        IMatrixD IMultipliable<IMatrixD>.Multiply(IMatrixD b)
        {
            //return MatrixProcessor.Multiply(this, b);
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrixD> Members

        /// <summary>
        /// Returns the quotient of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The quotient.</returns>
        IMatrixD IDivisible<IMatrixD>.Divide(IMatrixD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IMatrixD> Members

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        IMatrixD IHasOne<IMatrixD>.One
        {
            get { return ((IHasOne<Point<TCoordinate>>)this).One; }
        }

        #endregion

        #region IEnumerable<DoubleComponent> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<DoubleComponent> IEnumerable<DoubleComponent>.GetEnumerator()
        {
            return _coordinate.GetEnumerator();
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
        /// Gets true if the matrix is invertable (non-singluar).
        /// </summary>
        Boolean IMatrixD.IsInvertible
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
        /// Gets a submatrix.
        /// </summary>
        /// <param name="rowIndexes">The indexes of the rows to include.</param>
        /// <param name="j0">The starting column to include.</param>
        /// <param name="j1">The ending column to include.</param>
        /// <returns></returns>
        IMatrixD IMatrixD.GetMatrix(Int32[] rowIndexes, Int32 j0, Int32 j1)
        {
            throw new NotSupportedException();
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

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>The matrix with the rows as columns and columns as rows.</returns>
        IMatrixD IMatrixD.Transpose()
        {
            //return new Matrix<DoubleComponent>((this as IMatrixD).Format,
            //        new DoubleComponent[][] { new DoubleComponent[] { _x }, new DoubleComponent[] { _y } });

            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IVectorD> Members

        IVectorD INegatable<IVectorD>.Negative()
        {
            return ((IVectorD)_coordinate).Negative();
        }

        #endregion

        #region ISubtractable<IVectorD> Members

        IVectorD ISubtractable<IVectorD>.Subtract(IVectorD b)
        {
            if (b == null) throw new ArgumentNullException("b");

            ICoordinateFactory<TCoordinate> factory = _coordinate.Factory as ICoordinateFactory<TCoordinate>;
            return new Point<TCoordinate>(factory.Create(_coordinate.Subtract(b)));
        }

        #endregion

        #region IHasZero<IVectorD> Members

        IVectorD IHasZero<IVectorD>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IVectorD> Members

        IVectorD IAddable<IVectorD>.Add(IVectorD b)
        {
            if (b == null) throw new ArgumentNullException("b");

            ICoordinateFactory<TCoordinate> factory = _coordinate.Factory as ICoordinateFactory<TCoordinate>;
            return new Point<TCoordinate>(factory.Create(_coordinate.Add(b)));
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
            get { return ((IHasOne<IVectorD>)_coordinate).One; }
        }

        #endregion

        #region IMultipliable<IVectorD> Members

        IVectorD IMultipliable<IVectorD>.Multiply(IVectorD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IComparable<IMatrixD> Members

        Int32 IComparable<IMatrixD>.CompareTo(IMatrixD other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrixD> Members

        IMatrixD IComputable<IMatrixD>.Abs()
        {
            throw new NotImplementedException();
        }

        IMatrixD IComputable<IMatrixD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrixD> Members

        Boolean IBooleanComparable<IMatrixD>.GreaterThan(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.GreaterThanOrEqualTo(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.LessThan(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.LessThanOrEqualTo(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IMatrixD> Members

        IMatrixD IExponential<IMatrixD>.Exp()
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Log()
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IVectorD> Members

        IVectorD IComputable<IVectorD>.Abs()
        {
            throw new NotImplementedException();
        }

        IVectorD IComputable<Double, IVectorD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IVectorD> Members

        Boolean IBooleanComparable<IVectorD>.GreaterThan(IVectorD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.GreaterThanOrEqualTo(IVectorD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.LessThan(IVectorD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.LessThanOrEqualTo(IVectorD value)
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

        Int32 IComparable<IVectorD>.CompareTo(IVectorD other)
        {
            throw new NotImplementedException();
        }

        #endregion

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
                throw new ArgumentOutOfRangeException("row", row, "A Point<TCoordinate> has only 1 row.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeException("column", row, "A Point<TCoordinate> has only 2 columns.");
            }
        }
        #endregion

        #region IComputable<IVectorD> Members

        IVectorD IComputable<IVectorD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IVectorD> Members

        IVectorD IMultipliable<Double, IVectorD>.Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Point<TCoordinate>> Members
        Point<TCoordinate> IComputable<Point<TCoordinate>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,Point<TCoordinate>> Members

        public Point<TCoordinate> Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<Double,IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Add(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Double,IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Subtract(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<Double,Point<TCoordinate>> Members

        Point<TCoordinate> IAddable<Double, Point<TCoordinate>>.Add(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Double,Point<TCoordinate>> Members

        Point<TCoordinate> ISubtractable<Double, Point<TCoordinate>>.Subtract(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
