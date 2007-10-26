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

namespace SharpMap.Rendering.Rendering3D
{
    [Serializable]
    public struct ViewRectangle3D : IMatrixD, IComparable<ViewRectangle3D>
    {
        private DoubleComponent _xMin;
        private DoubleComponent _yMin;
        private DoubleComponent _xMax;
        private DoubleComponent _yMax;
        private DoubleComponent _zMin;
        private DoubleComponent _zMax;
        private Boolean _hasValue;

        public static readonly ViewRectangle3D Empty = new ViewRectangle3D();
        public static readonly ViewRectangle3D Zero = new ViewRectangle3D(0, 0, 0, 0, 0, 0);

        public ViewRectangle3D(Double xMin, Double xMax, Double yMin, Double yMax, Double zMin, Double zMax)
        {
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _zMin = zMin;
            _zMax = zMax;
            _hasValue = true;
        }

        public ViewRectangle3D(ViewPoint3D location, ViewSize3D size)
        {
            _xMin = location.X;
            _yMin = location.Y;
            _zMin = location.Z;
            _xMax = _xMin.Add(size.Width);
            _yMax = _yMin.Add(size.Height);
            _zMax = _zMin.Add(size.Depth);
            _hasValue = true;
        }

        public DoubleComponent X
        {
            get { return _xMin; }
        }

        public DoubleComponent Y
        {
            get { return _yMin; }
        }

        public DoubleComponent Z
        {
            get { return _zMin; }
        }

        public DoubleComponent Left
        {
            get { return _xMin; }
        }

        public DoubleComponent Top
        {
            get { return _yMin; }
        }

        public DoubleComponent Right
        {
            get { return _xMax; }
        }

        public DoubleComponent Bottom
        {
            get { return _yMax; }
        }

        public DoubleComponent Back
        {
            get { return _zMax; }
        }

        public DoubleComponent Front
        {
            get { return _zMin; }
        }

        public ViewPoint3D Center
        {
            get { return new ViewPoint3D(X + Width / 2, Y + Height / 2, Z + Depth / 2); }
        }

        public ViewPoint3D Location
        {
            get { return new ViewPoint3D(_xMin, _yMin, _zMin); }
        }

        public ViewPoint3D Size
        {
            get { return new ViewPoint3D(Width, Height, Depth); }
        }

        public Double Width
        {
            get { return Math.Abs((Double)_xMax.Subtract(_xMin)); }
        }

        public Double Height
        {
            get { return Math.Abs((Double)_yMax.Subtract(_yMin)); }
        }

        public Double Depth
        {
            get { return Math.Abs((Double)_zMax.Subtract(_zMin)); }
        }

        public static Boolean operator ==(ViewRectangle3D rect1, ViewRectangle3D rect2)
        {
            return rect1.Left.Equals(rect2.Left) &&
                rect1.Right.Equals(rect2.Right) &&
                rect1.Top.Equals(rect2.Top) &&
                rect1.Bottom.Equals(rect2.Bottom) &&
                rect1.Back.Equals(rect2.Back) &&
                rect1.Front.Equals(rect2.Front);
        }

        public static Boolean operator !=(ViewRectangle3D rect1, ViewRectangle3D rect2)
        {
            return !(rect1 == rect2);
        }

        public override Boolean Equals(object obj)
        {
            if (!(obj is ViewRectangle3D))
            {
                return false;
            }

            ViewRectangle3D other = (ViewRectangle3D)obj;

            return other == this;
        }

        public override Int32 GetHashCode()
        {
            return unchecked((Int32)Left ^ (Int32)Right ^ (Int32)Top ^ (Int32)Bottom ^ (Int32)Back ^ (Int32)Front);
        }

        public override String ToString()
        {
            return String.Format("ViewRectangle - Left: {0:N3}; Top: {1:N3}; Right: {2:N3}; Bottom: {3:N3}; Back: {4:N3}; Front: {5:N3}",
                Left, Top, Right, Top, Bottom, Back, Front);
        }

        /// <summary>
        /// Determines whether this <see cref="ViewRectangle3D"/> intersects another.
        /// </summary>
        /// <param name="rectangle"><see cref="ViewRectangle3D"/> to check intersection with.</param>
        /// <returns>True if there is intersection, false if not.</returns>
        public Boolean Intersects(ViewRectangle3D rectangle)
        {
            return !(rectangle.Left.GreaterThan(Right) ||
                     rectangle.Right.LessThan(Left) ||
                     rectangle.Bottom.GreaterThan(Top) ||
                     rectangle.Top.LessThan(Bottom) ||
                     rectangle.Front.GreaterThan(Back) ||
                     rectangle.Back.LessThan(Front));
        }

        #region IComparable<Rectangle> Members

        /// <summary>
        /// Compares this <see cref="ViewRectangle3D"/> instance with another instance.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="other">Rectangle to perform intersection test with.</param>
        /// <returns>
        /// Returns 0 if the <see cref="ViewRectangle3D"/> instances intersect each other,
        /// 1 if this Rectangle is located to the right or down from the <paramref name="other"/>
        /// Rectange, and -1 if this Rectangle is located to the left or up from the other.
        /// </returns>
        public Int32 CompareTo(ViewRectangle3D other)
        {
            if (Intersects(other))
            {
                return 0;
            }

            if (other.Left.GreaterThan(Right) || other.Top.GreaterThan(Bottom) || other.Front.GreaterThan(Back))
            {
                return -1;
            }

            return 1;
        }

        #endregion

        #region IViewMatrix Members

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Invert()
        {
            throw new NotSupportedException();
        }

        public Boolean IsInvertible
        {
            get { return false; }
        }

        public Boolean IsEmpty
        {
            get { return _hasValue; }
        }

        public Double[,] Elements
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

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
        /// Gets the elements in the matrix as an array of arrays (jagged array).
        /// </summary>
        DoubleComponent[][] IMatrixD.Elements
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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

        public void Rotate(Double degreesTheta)
        {
            throw new NotSupportedException();
        }

        public void RotateAt(Double degreesTheta, IVectorD center)
        {
            throw new NotSupportedException();
        }

        public Double GetOffset(Int32 dimension)
        {
            throw new NotSupportedException();
        }

        public void Offset(IVectorD offsetVector)
        {
            Translate(offsetVector);
        }

        public void Multiply(IMatrixD matrix)
        {
            throw new NotSupportedException();
        }

        public void Scale(Double scaleAmount)
        {
            throw new NotImplementedException();
        }

        public void Scale(IVectorD scaleVector)
        {
            throw new NotImplementedException();
        }

        public void Translate(Double translationAmount)
        {
            throw new NotImplementedException();
        }

        public void Translate(IVectorD translationVector)
        {
            throw new NotImplementedException();
        }

        public IVectorD Transform(IVectorD vector)
        {
            throw new NotImplementedException();
        }

        public Double[] Transform(params Double[] vector)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IViewMatrix> Members

        public Boolean Equals(IMatrixD other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
