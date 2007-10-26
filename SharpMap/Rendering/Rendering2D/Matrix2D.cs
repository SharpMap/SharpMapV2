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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a 2 dimensional affine transform matrix (a 3x3 matrix).
    /// </summary>
    /// <remarks>
    /// Matrix2D is arranged in row-major format, like GDI+, WPF and DirectX matrixes, where
    /// the translate components are in the 3rd row.
    /// </remarks>
    [Serializable]
    public class Matrix2D : AffineMatrix<DoubleComponent>
    {
        private static readonly Matrix2D _identity
            = new Matrix2D(1, 0, 0, 1, 0, 0);

        /// <summary>
        /// Gets an identity 2D matrix.
        /// </summary>
        /// <remarks>
        /// A 3x3 affine transform matrix, with 1s in the diagonal, and 0s everywhere else:
        /// 
        ///     | 1  0  0 |
        ///     | 0  1  0 |
        ///     | 0  0  1 |
        /// </remarks>
        public new static Matrix2D Identity
        {
            get { return _identity.Clone(); }
        }

        #region Constructors

        /// <summary>
        /// Creates a new identity Matrix2D.
        /// </summary>
        public Matrix2D()
            : this(1, 0, 0, 1, 0, 0)
        {
        }

        /// <summary>
        /// Creates a new Matrix2D with the given values.
        /// </summary>
        /// <param name="m11">The first row, first column component.</param>
        /// <param name="m21">The second row, first column component.</param>
        /// <param name="m12">The second row, first column component.</param>
        /// <param name="m22">The second row, second column component.</param>
        /// <param name="offsetX">The third row, first column component.</param>
        /// <param name="offsetY">The second row, third column component.</param>
        public Matrix2D(Double m11, Double m21, Double m12, Double m22, Double offsetX, Double offsetY)
            : base(MatrixFormat.RowMajor, 3)
        {
            M11 = m11;
            M21 = m21;
            OffsetX = offsetX;
            M12 = m12;
            M22 = m22;
            OffsetY = offsetY;
        }

        /// <summary>
        /// Creates a new matrix, copying the elements in <paramref name="matrixToCopy"/>.
        /// </summary>
        /// <param name="matrixToCopy">The matrix to copy the elements from.</param>
        public Matrix2D(IMatrixD matrixToCopy)
            : base(MatrixFormat.RowMajor, 3)
        {
            if (matrixToCopy == null) throw new ArgumentNullException("matrixToCopy");

            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    this[i, j] = matrixToCopy[i, j];
                }
            }
        }

        #endregion

        #region ToString

        public override String ToString()
        {
            return String.Format("[Matrix2D] [ [{0:N3}, {1:N3}, 0], [{2:N3}, {3:N3}, 0], [{4:N3}, {5:N3}, 1] ]",
                                 M11, M12, M21, M22, OffsetX, OffsetY);
        }

        #endregion

        #region GetHashCode

        public override Int32 GetHashCode()
        {
            return unchecked(this[0, 0].GetHashCode() + 24243 ^ this[0, 1].GetHashCode() + 7318674
                             ^ this[0, 2].GetHashCode() + 282 ^ this[1, 0].GetHashCode() + 54645
                             ^ this[1, 1].GetHashCode() + 42 ^ this[1, 2].GetHashCode() + 244892
                             ^ this[2, 0].GetHashCode() + 8464 ^ this[2, 1].GetHashCode() + 36565 ^
                             this[2, 2].GetHashCode() + 3210186);
        }

        #endregion

        /// <summary>
        /// Creates an element-for-element copy of the 2D matrix.
        /// </summary>
        /// <returns>An identical 2D matrix.</returns>
        public new Matrix2D Clone()
        {
            return new Matrix2D(this);
        }

        /// <summary>
        /// Gets the inverse of the 2D matrix.
        /// </summary>
        public new Matrix2D Inverse
        {
            get { return new Matrix2D(base.Inverse); }
        }

        /// <summary>
        /// Appends a scale factor to this matrix.
        /// </summary>
        /// <param name="x">Scale to apply to the X dimension.</param>
        /// <param name="y">Scale to apply to the Y dimension.</param>
        public void Scale(Double x, Double y)
        {
            Scale(new Point2D(x, y));
        }

        /// <summary>
        /// Prepends a scale factor to this matrix.
        /// </summary>
        /// <param name="x">Scale to apply to the X dimension.</param>
        /// <param name="y">Scale to apply to the Y dimension.</param>
        public void ScalePrepend(Double x, Double y)
        {
            base.Scale(new Point2D(x, y), MatrixOperationOrder.Prepend);
        }

        /// <summary>
        /// Appends a translation vector to this matrix.
        /// </summary>
        /// <param name="x">X component of the translation vector.</param>
        /// <param name="y">Y component of the translation vector.</param>
        public void Translate(Double x, Double y)
        {
            Translate(new Point2D(x, y));
        }

        /// <summary>
        /// Prepends a translation vector to this matrix.
        /// </summary>
        /// <param name="x">X component of the translation vector.</param>
        /// <param name="y">Y component of the translation vector.</param>
        public void TranslatePrepend(Double x, Double y)
        {
            Translate(new Point2D(x, y), MatrixOperationOrder.Prepend);
        }

        /// <summary>
        /// Appends a rotation in radians onto this matrix.
        /// </summary>
        /// <param name="theta">Amount to rotate, in radians.</param>
        public void Rotate(Double theta)
        {
            RotateAlong(null, theta);
        }

        /// <summary>
        /// Transforms a point by multiplying it with this matrix.
        /// </summary>
        /// <param name="x">X component of the point.</param>
        /// <param name="y">Y component of the point.</param>
        /// <returns>A Point2D describing the transformed input.</returns>
        public Point2D TransformVector(Double x, Double y)
        {
            DoubleComponent[] transferPoints = new DoubleComponent[] { x, y, 1 };

            MatrixProcessor<DoubleComponent>.Instance.Operations.Multiply(transferPoints, this);
            return new Point2D((Double)transferPoints[0], (Double)transferPoints[1]);
        }

        #region Equality Computation

        public override bool Equals(object obj)
        {
            if (obj is Matrix2D)
            {
                return Equals(obj as Matrix2D);
            }

            if (obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
            }

            return false;
        }

        #region IEquatable<Matrix2D> Members

        public bool Equals(Matrix2D other)
        {
            return M11 == other.M11 &&
                   M21 == other.M21 &&
                   OffsetX == other.OffsetX &&
                   M12 == other.M12 &&
                   M22 == other.M22 &&
                   OffsetY == other.OffsetY;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// The first row, first column component.
        /// </summary>
        public Double M11
        {
            get { return (Double)this[0, 0]; }
            set { this[0, 0] = value; }
        }

        /// <summary>
        /// The second row, first column component.
        /// </summary>
        public Double M21
        {
            get { return (Double)this[1, 0]; }
            set { this[1, 0] = value; }
        }

        /// <summary>
        /// The third row, first column component.
        /// </summary>
        public Double OffsetX
        {
            get { return (Double)this[2, 0]; }
            set { this[2, 0] = value; }
        }

        /// <summary>
        /// The second row, first column component.
        /// </summary>
        public Double M12
        {
            get { return (Double)this[0, 1]; }
            set { this[0, 1] = value; }
        }

        /// <summary>
        /// The second row, second column component.
        /// </summary>
        public Double M22
        {
            get { return (Double)this[1, 1]; }
            set { this[1, 1] = value; }
        }

        /// <summary>
        /// The second row, third column component.
        /// </summary>
        public Double OffsetY
        {
            get { return (Double)this[2, 1]; }
            set { this[2, 1] = value; }
        }

        #endregion
    }
}