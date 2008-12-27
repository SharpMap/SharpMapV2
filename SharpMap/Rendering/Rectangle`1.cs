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
	/// Represents an axis-aligned extent in scene coordinates.
	/// </summary>
    [Serializable]
    public struct Rectangle<TCoordinate> : ISceneExtent<TCoordinate>, 
                                           IEquatable<Rectangle<TCoordinate>>, 
                                           IComparable<Rectangle<TCoordinate>>, 
                                           //IHasEmpty, 
                                           IVertexStream<TCoordinate, DoubleComponent>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {

        internal static Rectangle<TCoordinate> FromLTRB(ICoordinateFactory<TCoordinate> coordinateFactory, Double left, Double top, Double right, Double bottom)
        {
            return new Rectangle<TCoordinate>(coordinateFactory, left, top, right, bottom);
        }

		/// <summary>
		/// An empty <see cref="Rectangle{TCoordinate}" />, having no value.
		/// </summary>
        public static readonly Rectangle<TCoordinate> Empty = new Rectangle<TCoordinate>();

        ///// <summary>
        ///// A <see cref="Rectangle{TCoordinate}" /> with zero height and zero width centered at (0, 0).
        ///// </summary>
        //public static readonly Rectangle<TCoordinate> Zero = new Rectangle<TCoordinate>(0, 0, 0, 0);

        private TCoordinate _min;
        private TCoordinate _max;
        private Boolean _hasValue;

        #region Constructors
		/// <summary>
		/// Creates a new <see cref="Rectangle{TCoordinate}" /> with the given values for the sides.
		/// </summary>
		/// <param name="left">The X value of the left side.</param>
		/// <param name="top">The Y value of the top side.</param>
		/// <param name="right">The X value of the right side.</param>
		/// <param name="bottom">The Y value of the bottom side.</param>
        public Rectangle(ICoordinateFactory<TCoordinate> coordinateFactory, Double left, Double top, Double right, Double bottom)
        {
            _min = coordinateFactory.Create(left, top);
            _max = coordinateFactory.Create(right, bottom);
            _hasValue = true;
        }

		/// <summary>
        /// Creates a new <see cref="Rectangle{TCoordinate}"/> with the upper-left at
		/// <paramref name="location"/>, and the given <paramref name="size"/>.
		/// </summary>
        /// <param name="location">The upper-left point of the <see cref="Rectangle{TCoordinate}"/>.</param>
        /// <param name="size">The size of the <see cref="Rectangle{TCoordinate}"/>.</param>
        public Rectangle(Point<TCoordinate> location, Size<TCoordinate> size)
            : this(location.Coordinate, size) { }

        /// <summary>
        /// Creates a new <see cref="Rectangle{TCoordinate}"/> with the upper-left at
        /// <paramref name="location"/>, and the given <paramref name="size"/>.
        /// </summary>
        /// <param name="location">The upper-left point of the <see cref="Rectangle{TCoordinate}"/>.</param>
        /// <param name="size">The size of the <see cref="Rectangle{TCoordinate}"/>.</param>
        public Rectangle(TCoordinate location, Size<TCoordinate> size)
        {
            _min = location;
            _max = (TCoordinate)_min.Add(size);

            _hasValue = true;
        }

        internal Rectangle(TCoordinate min, TCoordinate max)
        {
            _min = min;
            _max = max;

            _hasValue = true;
        }

        #endregion

        #region ToString
        public override String ToString()
        {
            return
                String.Format(
                    "[Rectangle<TCoordinate>] Left: {0:N3}; Top: {1:N3}; Right: {2:N3}; Bottom: {3:N3}; IsEmpty: {4}", 
                    Left, Top, Right, Bottom, IsEmpty);
        }
        #endregion

        #region GetHashCode
        public override Int32 GetHashCode()
        {
            return unchecked((Int32)Left ^ (Int32)Right ^ (Int32)Top ^ (Int32)Bottom);
        }
        #endregion

        #region Equality Testing

        public Boolean Equals(IMatrixD other)
        {
            if (other is Rectangle<TCoordinate>)
            {
                return Equals((Rectangle<TCoordinate>)other);
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
            //DoubleComponent[][] lhs = thisMatrix.Elements, rhs = other.Elements;

            if (thisMatrix.ColumnCount != other.ColumnCount)
            {
                return false;
            }

            if (thisMatrix.RowCount != thisMatrix.RowCount)
            {
                return false;
            }

            if (thisMatrix.RowCount != other.RowCount || thisMatrix.ColumnCount != other.ColumnCount)
            {
                return false;
            }

            unchecked
            {
                for (Int32 row = 0; row < thisMatrix.RowCount; row++)
                {
                    for (Int32 col = 0; col < thisMatrix.ColumnCount; col++)
                    {
                        if (!thisMatrix[row, col].Equals(other[row, col]))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static Boolean operator ==(Rectangle<TCoordinate> lhs, Rectangle<TCoordinate> rhs)
        {
            return (lhs.Equals(rhs));
        }

        public static Boolean operator !=(Rectangle<TCoordinate> lhs, Rectangle<TCoordinate> rhs)
        {
            return !(lhs.Equals(rhs));
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Rectangle<TCoordinate>)
            {
                return Equals((Rectangle<TCoordinate>)obj);
            }

            if (obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
            }

            return false;
        }

        public Boolean Equals(Rectangle<TCoordinate> rectangle)
        {
            return IsEmpty == rectangle.IsEmpty &&
                   Left == rectangle.Left &&
                   Right == rectangle.Right &&
                   Top == rectangle.Top &&
                   Bottom == rectangle.Bottom;
        }

        #endregion

        #region IComparable<Rectangle<TCoordinate>> Members

        /// <summary>
        /// Compares this <see cref="Rectangle{TCoordinate}"/> instance with another instance.
        /// </summary>
        /// <remarks>
        /// The comparison for non-intersecting rectangles is computed by determining where 
        /// the top-left corner of the <paramref name="other"/> rectangle is in relation 
        /// to the one CompareTo is called on. The following diagram shows which value is 
        /// returned depending on the location of the top-left corner of the other rectangle:
        /// <pre>
        /// 
        ///                  +1
        /// 
        ///                            +--------------------------------------------
        ///                            |              |
        ///                            |              |
        ///                            |              |
        ///                            |              |
        ///                            |              |
        ///                            |--------------+
        ///                            |
        ///                            |
        ///                            |                      -1
        ///                            |
        ///                            |
        /// </pre>
        /// Any intersecting rectangle returns a 0, regardless of the position of the top-left corner.
        /// </remarks>
        /// <param name="other">Rectangle<TCoordinate> to perform comparison with.</param>
        /// <returns>
        /// Returns 0 if the <see cref="Rectangle{TCoordinate}"/> instances intersect each other,
        /// 1 if this rectangle is located to the right or downward from the <paramref name="other"/>
        /// rectangle, and -1 if this rectangle is located to the left or upward from the other.
        /// </returns>
        public Int32 CompareTo(Rectangle<TCoordinate> other)
        {
            if (other.IsEmpty && IsEmpty)
            {
                return 0;
            }

            if (Intersects(other))
            {
                return 0;
            }

            if (IsEmpty || other.Left > Left || other.Top > Top)
            {
                return -1;
            }

            return 1;
        }

        #endregion

        #region Intersects
        /// <summary>
        /// Determines whether this <see cref="Rectangle{TCoordinate}"/> intersects another.
        /// </summary>
        /// <param name="rectangle"><see cref="Rectangle{TCoordinate}"/> to check intersection with.</param>
        /// <returns>True if there is intersection, false if not.</returns>
        public Boolean Intersects(Rectangle<TCoordinate> rectangle)
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
        #endregion

        #region Properties

        /// <summary>
        /// Gets the X value of the coordinate of the upper left 
        /// corner of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        public Double X
        {
            get { return _min[Ordinates.X]; }
        }

        /// <summary>
        /// Gets the Y value of the coordinate of the upper left 
        /// corner of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        public Double Y
        {
            get { return _min[Ordinates.Y]; }
        }

        /// <summary>
        /// Gets the X value of the left edge of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Top"/>
        public Double Left
        {
            get { return X; }
        }

        /// <summary>
        /// Gets the Y value of the top edge of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Left"/>
        public Double Top
        {
            get { return Y; }
        }

        /// <summary>
        /// Gets the X value of the right edge of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Top"/>
        /// <seealso cref="Left"/>
        public Double Right
        {
            get { return _max[Ordinates.X]; }
        }

        /// <summary>
        /// Gets the Y value of the bottom edge of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Right"/>
        /// <seealso cref="Top"/>
        /// <seealso cref="Left"/>
        public Double Bottom
        {
            get { return _max[Ordinates.Y]; }
        }

        /// <summary>
        /// Gets the coordinates of the center of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        /// <seealso cref="X"/>
        /// <seealso cref="Y"/>
        public TCoordinate Center
        {
            get { return ((IDivisible<Double, TCoordinate>)_min.Add(_max)).Divide(2); }
        }

        /// <summary>
        /// Gets the coordinates of the upper left corner of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Center"/>
        public TCoordinate Location
        {
            get { return _min; }
        }

        /// <summary>
        /// Gets the size of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Width"/>
        /// <seealso cref="Height"/>
        public Size<TCoordinate> Size
        {
            get { return new Size<TCoordinate>(Width, Height); }
        }

        /// <summary>
        /// Gets the width of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public Double Width
        {
            get { return _max.Subtract(_min)[Ordinates.X]; }
        }

        /// <summary>
        /// Gets the height of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public Double Height
        {
            get { return _max.Subtract(_min)[Ordinates.Y]; }
        }

        /// <summary>
        /// Gets the depth of the <see cref="Rectangle{TCoordinate}"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public Double Depth
        {
            get { return _max.Subtract(_min)[Ordinates.Z]; }
        }

		/// <summary>
		/// Gets <see langword="true"/> if the <see cref="Rectangle{TCoordinate}"/> has no set value.
		/// </summary>
        public Boolean IsEmpty
        {
            get { return !_hasValue; }
        }
        #endregion

        #region Clone
        public Rectangle<TCoordinate> Clone()
        {
            return new Rectangle<TCoordinate>(_min.Clone(), _max.Clone());
        }
        #endregion

		public TCoordinate UpperLeft
		{
			get { return _min; }
		}

        //public TCoordinate UpperRight
        //{
        //    get { return new Point<TCoordinate>(X + Width, Y); }
        //}

        //public TCoordinate LowerLeft
        //{
        //    get { return new Point<TCoordinate>(X, Y + Height); }
        //}

		public TCoordinate LowerRight
		{
			get { return _max; }
        }

        #region ISceneExtent<Point<TCoordinate>> Members

        TCoordinate ISceneExtent<TCoordinate>.GetLowerBound(IVector<DoubleComponent> axis)
        {
			return UpperLeft;
        }

        TCoordinate ISceneExtent<TCoordinate>.GetUpperBound(IVector<DoubleComponent> axis)
        {
			return LowerRight;
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

        MatrixFormat IMatrixD.Format
        {
            get { return MatrixFormat.Unspecified; }
        }

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

        ///// <summary>
        ///// Gets the elements in the matrix as an array of arrays (jagged array).
        ///// </summary>
        //DoubleComponent[][] IMatrixD.Elements
        //{
        //    get
        //    {
        //        if (IsEmpty)
        //        {
        //            return new DoubleComponent[0][];
        //        }

        //        return new DoubleComponent[][] { new DoubleComponent[] { _left, _top }, new DoubleComponent[] { _right, _bottom } };
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw new ArgumentNullException("value");
        //        }

        //        if (value.Length != 2)
        //        {
        //            throw new ArgumentException("Elements can be set only to a 2x2 array");
        //        }

        //        if (value[0] == null || value[1] == null)
        //        {
        //            throw new ArgumentException("The array elements of value cannot be null.");
        //        }

        //        if (value[0].Length != 2 || value[1].Length != 2)
        //        {
        //            throw new ArgumentException("Elements can be set only to a 2x2 array");
        //        }

        //        _left = value[0][0].LessThan(value[1][0]) ? value[0][0] : value[1][0];
        //        _top = value[0][1].LessThan(value[1][1]) ? value[0][1] : value[1][1];
        //        _right = value[0][0].LessThan(value[1][0]) ? value[1][0] : value[0][0];
        //        _bottom = value[0][1].LessThan(value[1][1]) ? value[1][1] : value[0][1];
        //        _hasValue = true;
        //    }
        //}

        /// <summary>
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        Boolean IMatrixD.IsSingular
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singular).
        /// </summary>
        Boolean IMatrixD.IsInvertible
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
        Boolean IMatrixD.IsSquare
        {
            get { return true; }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        Boolean IMatrixD.IsSymmetrical
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        Int32 IMatrixD.RowCount
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
        IMatrixD IMatrixD.GetMatrix(Int32[] rowIndexes, Int32 j0, Int32 j1)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets an element in the matrix.
        /// </summary>
        /// <param name="row">The index of the row of the element.</param>
        /// <param name="column">The index of the column of the element.</param>
        DoubleComponent IMatrixD.this[Int32 row, Int32 column]
        {
            get
            {
                checkIndexes(row, column);

                return row == 0 ? _min[column] : _max[column];
            }
            set
            {
                throw new NotSupportedException();

                //checkIndexes(row, column);

                //if (row == 0)
                //{
                //    if (column == 0)
                //    {
                //        _left = value;
                //    }
                //    else
                //    {
                //        _top = value;
                //    }
                //}
                //else
                //{
                //    if (column == 0)
                //    {
                //        _right = value;
                //    }
                //    else
                //    {
                //        _bottom = value;
                //    }
                //}

                //_hasValue = true;
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

        #region IAffineTransformMatrix<DoubleComponent> Members

        /// <summary>
        /// Scales the matrix by the given <paramref name="amount"/> in all orthogonal columns.
        /// </summary>
        /// <param name="amount">Amount to scale by.</param>
        public void Scale(DoubleComponent amount)
        {
            if (IsEmpty)
            {
                return;
            }

            _max = _max.Multiply((Double)amount);
        }

        /// <summary>
        /// Scales the matrix by the given vector <paramref name="scaleVector"/>.
        /// </summary>
        /// <param name="scaleVector">
        /// A vector with scaling components which 
        /// correspond to the affine transform dimensions.
        /// </param>
        public void Scale(IVectorD scaleVector)
        {
            if (scaleVector == null) throw new ArgumentNullException("scaleVector");

            if (IsEmpty)
            {
                return;
            }

            _max = (TCoordinate)_max.Multiply(scaleVector);
        }

        /// <summary>
        /// Translates the affine transform by the given amount in each dimension.
        /// </summary>
        /// <param name="amount">Amount to translate by.</param>
        public void Translate(DoubleComponent amount)
        {
            if(IsEmpty)
            {
                return;
            }

            _min = ((IAddable<Double, TCoordinate>)_min).Add((Double)amount);
            _max = ((IAddable<Double, TCoordinate>)_max).Add((Double)amount);
        }

        /// <summary>
        /// Translates the affine transform by the given translation vector.
        /// </summary>
        /// <param name="translateVector">
        /// A vector whose components will translate the transform 
        /// in the corresponding dimension.
        /// </param>
        public void Translate(IVectorD translateVector)
        {
            if (translateVector == null) throw new ArgumentNullException("translateVector");

            if (IsEmpty)
            {
                return;
            }

            _min = (TCoordinate)_min.Add(translateVector);
            _max = (TCoordinate)_max.Add(translateVector);
        }
        #endregion

        #region IVertexStream<TCoordinate> Members

        public IEnumerable<TCoordinate> GetVertexes()
        {
            throw new NotImplementedException();
            //yield return LowerLeft;
            //yield return UpperLeft;
            //yield return UpperRight;
            //yield return LowerRight;
        }

        public IEnumerable<TCoordinate> GetVertexes(ITransformMatrix<DoubleComponent> transform)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helper Methods

        private static void checkIndexes(Int32 row, Int32 column)
        {
            if (row < 0 || row > 1)
            {
                throw new ArgumentOutOfRangeException("row", row, "A Rectangle<TCoordinate> has only 2 rows.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeException("column", row, "A Rectangle<TCoordinate> has only 2 columns.");
            }
        }
        #endregion

        #region Non-supported IAffineTransformMatrix and IMatrix memebers
        /// <summary>
        /// Gets the inverse of the affine transform.
        /// </summary>
        IAffineTransformMatrix<DoubleComponent> IAffineTransformMatrix<DoubleComponent>.Inverse
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Resets the affine transform to the identity matrix (a diagonal of one).
        /// </summary>
        void IAffineTransformMatrix<DoubleComponent>.Reset()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Applies a shear to the transform.
        /// </summary>
        /// <param name="shearVector">The vector used to compute the shear.</param>
        void ITransformMatrix<DoubleComponent>.Shear(IVectorD shearVector)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Applies a shear to the transform.
        /// </summary>
        /// <param name="shearVector">The vector used to compute the shear.</param>
        /// <param name="order">Order in which to apply the operation.</param>
        void ITransformMatrix<DoubleComponent>.Shear(IVectorD shearVector, MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/>.
        /// </summary>
        /// <param name="axis">
        /// The axis to rotate around. May be an addition of the basis vectors.
        /// </param>
        /// <param name="radians">Angle to rotate through.</param>
        void ITransformMatrix<DoubleComponent>.RotateAlong(IVectorD axis, Double radians)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/>.
        /// </summary>
        /// <param name="axis">
        /// The axis to rotate around. May be an addition of the basis vectors.
        /// </param>
        /// <param name="radians">Angle to rotate through.</param>
        /// <param name="order">Order in which to apply the operation.</param>
        void ITransformMatrix<DoubleComponent>.RotateAlong(IVectorD axis, Double radians,
                                                                MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/> 
        /// at the given <paramref name="point"/>.
        /// </summary>
        /// <param name="point">Point at which to compute the rotation.</param>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="radians">Angle to rotate through.</param>
        void IAffineTransformMatrix<DoubleComponent>.RotateAt(IVectorD point, IVectorD axis, Double radians)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/> 
        /// at the given <paramref name="point"/>.
        /// </summary>
        /// <param name="point">Point at which to compute the rotation.</param>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="radians">Angle to rotate through.</param>
        /// <param name="order">Order in which to apply the operation.</param>
        void IAffineTransformMatrix<DoubleComponent>.RotateAt(IVectorD point, IVectorD axis, Double radians,
                                                             MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Scales the matrix by the given <paramref name="amount"/> in all orthoganal columns.
        /// </summary>
        /// <param name="amount">Amount to scale by.</param>
        /// <param name="order">Order in which to apply the operation.</param>
        void ITransformMatrix<DoubleComponent>.Scale(DoubleComponent amount, MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Scales the matrix by the given vector <paramref name="scaleVector"/>.
        /// </summary>
        /// <param name="scaleVector">
        /// A vector with scaling components which 
        /// correspond to the affine transform dimensions.
        /// </param>
        /// <param name="order">Order in which to apply the operation.</param>
        void ITransformMatrix<DoubleComponent>.Scale(IVectorD scaleVector, MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Translates the affine transform by the given translation vector.
        /// </summary>
        /// <param name="amount">Amount to translate by.</param>
        /// <param name="order">Order in which to apply the operation.</param>
        void IAffineTransformMatrix<DoubleComponent>.Translate(DoubleComponent amount, MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Translates the affine transform by the given translation vector.
        /// </summary>
        /// <param name="translateVector">
        /// A vector whose components will translate the transform 
        /// in the corresponding dimension.
        /// </param>
        /// <param name="order">Order in which to apply the operation.</param>
        void IAffineTransformMatrix<DoubleComponent>.Translate(IVectorD translateVector, MatrixOperationOrder order)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> matrix.
        /// </summary>
        /// <param name="input">Matrix to transform.</param>
        /// <returns>
        /// The multiplication of this transform matrix with the input matrix.
        /// </returns>
        IMatrixD ITransformMatrix<DoubleComponent>.TransformMatrix(IMatrixD input)
        {
            throw new NotSupportedException();
        }

        ///// <summary>
        ///// Applies this transform to the given <paramref name="input"/> matrix in place.
        ///// </summary>
        ///// <param name="input">Matrix to transform.</param>
        ///// <returns>
        ///// The multiplication of this transform matrix with the input matrix.
        ///// </returns>
        //void ITransformMatrix<DoubleComponent>.TransformMatrix(DoubleComponent[][] input)
        //{
        //    throw new NotSupportedException();
        //}

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Vector to transform.</param>
        /// <returns>
        /// The multiplication of this transform matrix with the input vector.
        /// </returns>
        IVectorD ITransformMatrix<DoubleComponent>.TransformVector(IVectorD input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vector in place.
        /// </summary>
        /// <param name="input">Vector to transform.</param>
        void ITransformMatrix<DoubleComponent>.TransformVector(DoubleComponent[] input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vectors.
        /// </summary>
        /// <param name="input">Set of vectors to transform.</param>
        /// <returns>
        /// The multiplication of this transform matrix with each of the input vectors.
        /// </returns>
        IEnumerable<IVectorD> ITransformMatrix<DoubleComponent>.TransformVectors(IEnumerable<IVectorD> input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vectors in place.
        /// </summary>
        /// <param name="input">Set of vectors to transform.</param>
        void ITransformMatrix<DoubleComponent>.TransformVectors(IEnumerable<DoubleComponent[]> input)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region IComparable<IMatrix<DoubleComponent>> Members

        public Int32 CompareTo(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Abs()
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Set(Double value)
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
    }
}