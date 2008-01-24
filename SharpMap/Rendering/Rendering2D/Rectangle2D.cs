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
	/// Represents an axis-aligned 2D rectangle in screen coordinates.
	/// </summary>
	/// <remarks>
	/// Since the Rectangle2D is in screen coordinates, the <see cref="Top"/> 
	/// property is less than the <see cref="Bottom"/> property when the <see cref="Height"/>
	/// is positive, and Bottom is less than Top when the Height is negative.
	/// </remarks>
    [Serializable]
    public struct Rectangle2D 
        : IViewRectangle<Point2D>, IEquatable<Rectangle2D>, IComparable<Rectangle2D>, IHasEmpty, IVertexStream<Point2D, DoubleComponent>
    {
		/// <summary>
		/// An empty Rectangle2D, having no value.
		/// </summary>
        public static readonly Rectangle2D Empty = new Rectangle2D();

		/// <summary>
		/// A Rectangle2D with zero height and zero width centered at (0, 0).
		/// </summary>
        public static readonly Rectangle2D Zero = new Rectangle2D(0, 0, 0, 0);

        private DoubleComponent _bottom;
        private DoubleComponent _left;
        private DoubleComponent _right;
        private DoubleComponent _top;
        private Boolean _hasValue;

        #region Constructors
		/// <summary>
		/// Creates a new Rectangle2D with the given values for the sides.
		/// </summary>
		/// <param name="left">The X value of the left side.</param>
		/// <param name="top">The Y value of the top side.</param>
		/// <param name="right">The X value of the right side.</param>
		/// <param name="bottom">The Y value of the bottom side.</param>
        public Rectangle2D(Double left, Double top, Double right, Double bottom)
        {
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
            _hasValue = true;
        }

		/// <summary>
		/// Creates a new Rectangle2D with the upper-left at
		/// <paramref name="location"/>, and the given <paramref name="size"/>.
		/// </summary>
		/// <param name="location">The upper-left point of the Rectangle2D.</param>
		/// <param name="size">The size of the Rectangle2D.</param>
        public Rectangle2D(Point2D location, Size2D size)
        {
            _left = location.X;
            _top = location.Y;
            _right = _left + size.Width;
            _bottom = _top + size.Height;

            _hasValue = true;
        }

        #endregion

        #region ToString
        public override String ToString()
        {
            return
                String.Format(
                    "[Rectangle2D] Left: {0:N3}; Top: {1:N3}; Right: {2:N3}; Bottom: {3:N3}; IsEmpty: {4}", 
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
            if (other is Rectangle2D)
            {
                return Equals((Rectangle2D)other);
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

        public static Boolean operator ==(Rectangle2D lhs, Rectangle2D rhs)
        {
            return (lhs.Equals(rhs));
        }

        public static Boolean operator !=(Rectangle2D lhs, Rectangle2D rhs)
        {
            return !(lhs.Equals(rhs));
        }

        public override Boolean Equals(object obj)
        {
            if (obj is Rectangle2D)
            {
                return Equals((Rectangle2D)obj);
            }

            if (obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
            }

            return false;
        }

        public Boolean Equals(Rectangle2D rectangle)
        {
            return IsEmpty == rectangle.IsEmpty &&
                   Left == rectangle.Left &&
                   Right == rectangle.Right &&
                   Top == rectangle.Top &&
                   Bottom == rectangle.Bottom;
        }

        #endregion

        #region IComparable<Rectangle2D> Members

        /// <summary>
        /// Compares this <see cref="Rectangle2D"/> instance with another instance.
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
        /// <param name="other">Rectangle2D to perform comparison with.</param>
        /// <returns>
        /// Returns 0 if the <see cref="Rectangle2D"/> instances intersect each other,
        /// 1 if this rectangle is located to the right or downward from the <paramref name="other"/>
        /// rectangle, and -1 if this rectangle is located to the left or upward from the other.
        /// </returns>
        public Int32 CompareTo(Rectangle2D other)
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

        internal static Rectangle2D FromLTRB(Double left, Double top, Double right, Double bottom)
        {
            return new Rectangle2D(left, top, right, bottom);
        }

        #region Intersects
        /// <summary>
        /// Determines whether this <see cref="Rectangle2D"/> intersects another.
        /// </summary>
        /// <param name="rectangle"><see cref="Rectangle2D"/> to check intersection with.</param>
        /// <returns>True if there is intersection, false if not.</returns>
        public Boolean Intersects(Rectangle2D rectangle)
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
        /// corner of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        public Double X
        {
            get { return (Double)_left; }
        }

        /// <summary>
        /// Gets the Y value of the coordinate of the upper left 
        /// corner of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        public Double Y
        {
            get { return (Double)_top; }
        }

        /// <summary>
        /// Gets the X value of the left edge of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Top"/>
        public Double Left
        {
            get { return (Double)_left; }
        }

        /// <summary>
        /// Gets the Y value of the top edge of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Left"/>
        public Double Top
        {
            get { return (Double)_top; }
        }

        /// <summary>
        /// Gets the X value of the right edge of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Bottom"/>
        /// <seealso cref="Top"/>
        /// <seealso cref="Left"/>
        public Double Right
        {
            get { return (Double)_right; }
        }

        /// <summary>
        /// Gets the Y value of the bottom edge of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Right"/>
        /// <seealso cref="Top"/>
        /// <seealso cref="Left"/>
        public Double Bottom
        {
            get { return (Double)_bottom; }
        }

        /// <summary>
        /// Gets the coordinates of the center of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Location"/>
        /// <seealso cref="X"/>
        /// <seealso cref="Y"/>
        public Point2D Center
        {
            get { return new Point2D(X + Width / 2, Y + Height / 2); }
        }

        /// <summary>
        /// Gets the coordinates of the upper left corner of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Center"/>
        public Point2D Location
        {
            get { return new Point2D((Double)_left, (Double)_top); }
        }

        /// <summary>
        /// Gets the size of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Width"/>
        /// <seealso cref="Height"/>
        public Size2D Size
        {
            get { return new Size2D(Width, Height); }
        }

        /// <summary>
        /// Gets the width of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public Double Width
        {
            get { return (Double)_right.Subtract(_left); }
        }

        /// <summary>
        /// Gets the height of the <see cref="Rectangle2D"/>.
        /// </summary>
        /// <seealso cref="Size"/>
        public Double Height
        {
            get { return (Double)_bottom.Subtract(_top); }
        }

		/// <summary>
		/// Gets true if the Rectangle2D has no set value.
		/// </summary>
        public Boolean IsEmpty
        {
            get { return !_hasValue; }
        }
        #endregion

        #region Clone
        public Rectangle2D Clone()
        {
            return new Rectangle2D(Location, Size);
        }
        #endregion

		public Point2D UpperLeft
		{
			get { return Location; }
		}

		public Point2D UpperRight
		{
			get { return new Point2D(X + Width, Y); }
		}

		public Point2D LowerLeft
		{
			get { return new Point2D(X, Y + Height); }
		}

		public Point2D LowerRight
		{
			get { return new Point2D(X + Width, Y + Height); }
		}

        #region IViewRectangle<Point2D> Members

		Point2D IViewRectangle<Point2D>.GetLowerBound(IVector<DoubleComponent> axis)
        {
			return UpperLeft;
        }

		Point2D IViewRectangle<Point2D>.GetUpperBound(IVector<DoubleComponent> axis)
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
        /// <returns></returns>
        DoubleComponent IMatrixD.this[Int32 row, Int32 column]
        {
            get
            {
                checkIndexes(row, column);

                if (row == 0)
                {
                    if (column == 0)
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

        #region IAffineTransformMatrix<DoubleComponent> Members

        /// <summary>
        /// Scales the matrix by the given <paramref name="amount"/> in all orthoganal columns.
        /// </summary>
        /// <param name="amount">Amount to scale by.</param>
        public void Scale(DoubleComponent amount)
        {
            if (IsEmpty)
            {
                return;
            }

            _right = _right.Multiply(amount);
            _bottom = _bottom.Multiply(amount);
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
            if (scaleVector.ComponentCount != 2)
            {
                throw new ArgumentException("Number of components in scale vector must be 2.", "scaleVector");
            }

            if (IsEmpty)
            {
                return;
            }

            _right = _right.Multiply(scaleVector[0]);
            _bottom = _bottom.Multiply(scaleVector[1]);
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

            _left = _left.Add(amount);
            _top = _top.Add(amount);
            _right = _right.Add(amount);
            _bottom = _bottom.Add(amount);
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
            if (translateVector.ComponentCount != 2)
            {
                throw new ArgumentException("Number of components in scale vector must be 2.", "translateVector");
            }

            if (IsEmpty)
            {
                return;
            }

            _left = _left.Add(translateVector[0]);
            _top = _top.Add(translateVector[1]);
            _right = _right.Add(translateVector[0]);
            _bottom = _bottom.Add(translateVector[1]);
        }
        #endregion

        #region IVertexStream<Point2D,DoubleComponent> Members

        public IEnumerable<Point2D> GetVertexes()
        {
            yield return LowerLeft;
            yield return UpperLeft;
            yield return UpperRight;
            yield return LowerRight;
        }

        public IEnumerable<Point2D> GetVertexes(ITransformMatrix<DoubleComponent> transform)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helper Methods

        private static void checkIndexes(Int32 row, Int32 column)
        {
            if (row < 0 || row > 1)
            {
                throw new ArgumentOutOfRangeException("row", row, "A Rectangle2D has only 2 rows.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeException("column", row, "A Rectangle2D has only 2 columns.");
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

        public int CompareTo(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Abs()
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Set(double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrix<DoubleComponent>> Members

        public bool GreaterThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public bool GreaterThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public bool LessThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public bool LessThanOrEqualTo(IMatrix<DoubleComponent> value)
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

        public IMatrix<DoubleComponent> Log(double newBase)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Power(double exponent)
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