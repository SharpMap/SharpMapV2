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
        private readonly static Matrix2D _identity
            = new Matrix2D(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1);

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
            : this(1, 0, 0, 0, 1, 0) { }

        /// <summary>
        /// Creates a new Matrix2D with the given values.
        /// </summary>
        /// <param name="x1">The first row, first column component.</param>
        /// <param name="x2">The second row, first column component.</param>
        /// <param name="offsetX">The third row, first column component.</param>
        /// <param name="y1">The second row, first column component.</param>
        /// <param name="y2">The second row, second column component.</param>
        /// <param name="offsetY">The second row, third column component.</param>
        public Matrix2D(double x1, double x2, double offsetX,
            double y1, double y2, double offsetY)
            : this(x1, x2, offsetX, y1, y2, offsetY, 0, 0, 1)
        {
        }

        protected Matrix2D(double x1, double x2, double offsetX,
            double y1, double y2, double offsetY,
            double w1, double w2, double w3)
            :base(MatrixFormat.RowMajor, 3)
        {
            X1 = x1; X2 = x2; OffsetX = offsetX;
            Y1 = y1; Y2 = y2; OffsetY = offsetY;
            W1 = w1; W2 = w2; W3 = w3;
        }

        public Matrix2D(IMatrixD matrixToCopy)
            : base(MatrixFormat.RowMajor, 3)
        {
            if (matrixToCopy == null) throw new ArgumentNullException("matrixToCopy");

            for (int i = 0; i < RowCount; i++)
            {
                Array.Copy(matrixToCopy.Elements, Elements, matrixToCopy.Elements.Length);
            }
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return String.Format("[ViewMatrix2D] [ [{0:N3}, {1:N3}, {2:N3}], [{3:N3}, {4:N3}, {5:N3}], [{6:N3}, {7:N3}, {8:N3}] ]",
                X1, Y1, W1, X2, Y2, W2, OffsetX, OffsetY, W3);
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            return unchecked(this[0, 0].GetHashCode() + 24243 ^ this[0, 1].GetHashCode() + 7318674
                ^ this[0, 2].GetHashCode() + 282 ^ this[1, 0].GetHashCode() + 54645
                ^ this[1, 1].GetHashCode() + 42 ^ this[1, 2].GetHashCode() + 244892
                ^ this[2, 0].GetHashCode() + 8464 ^ this[2, 1].GetHashCode() + 36565 ^ this[2, 2].GetHashCode() + 3210186);
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
            get
            {
                return new Matrix2D(base.Inverse);
            }
		}

        /// <summary>
        /// Appends a scale factor to this matrix.
        /// </summary>
        /// <param name="x">Scale to apply to the X dimension.</param>
        /// <param name="y">Scale to apply to the Y dimension.</param>
        public void Scale(double x, double y)
        {
            base.Scale(new Point2D(x, y));
		}

        /// <summary>
        /// Prepends a scale factor to this matrix.
        /// </summary>
        /// <param name="x">Scale to apply to the X dimension.</param>
        /// <param name="y">Scale to apply to the Y dimension.</param>
		public void ScalePrepend(double x, double y)
		{
			base.Scale(new Point2D(x, y), MatrixOperationOrder.Prepend);
		}

        /// <summary>
        /// Appends a translation vector to this matrix.
        /// </summary>
        /// <param name="x">X component of the translation vector.</param>
        /// <param name="y">Y component of the translation vector.</param>
        public void Translate(double x, double y)
        {
            base.Translate(new Point2D(x, y));
        }

        /// <summary>
        /// Prepends a translation vector to this matrix.
        /// </summary>
        /// <param name="x">X component of the translation vector.</param>
        /// <param name="y">Y component of the translation vector.</param>
		public void TranslatePrepend(double x, double y)
		{
			base.Translate(new Point2D(x, y), MatrixOperationOrder.Prepend);
		}

        /// <summary>
        /// Appends a rotation in radians onto this matrix.
        /// </summary>
        /// <param name="theta">Amount to rotate, in radians.</param>
        public void Rotate(double theta)
        {
            RotateAlong(null, theta);
        }

        private readonly DoubleComponent[] _transferPoints = new DoubleComponent[3];

        /// <summary>
        /// Transforms a point by multiplying it with this matrix.
        /// </summary>
        /// <param name="x">X component of the point.</param>
        /// <param name="y">Y component of the point.</param>
        /// <returns>A Point2D describing the transformed input.</returns>
        public Point2D TransformVector(double x, double y)
        {
            _transferPoints[0] = x;
            _transferPoints[1] = y;
            _transferPoints[2] = 1;
            MatrixProcessor<DoubleComponent>.Instance.Operations.Multiply(_transferPoints, this);
            return new Point2D((double)_transferPoints[0], (double)_transferPoints[1]);
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

        #region IEquatable<ViewMatrix2D> Members

        public bool Equals(Matrix2D other)
        {
            return X1 == other.X1 &&
                X2 == other.X2 &&
                OffsetX == other.OffsetX && 
                Y1 == other.Y1 &&
                Y2 == other.Y2 &&
                OffsetY == other.OffsetY &&
                W1 == other.W1 &&
                W2 == other.W2 &&
                W3 == other.W3;
        }

        #endregion
        #endregion

        #region Properties
        public double X1
        {
            get { return (double)this[0, 0]; }
            set { this[0, 0] = value; }
        }

        public double X2
        {
            get { return (double)this[1, 0]; }
            set { this[1, 0] = value; }
        }

        public double OffsetX
        {
            get { return (double)this[2, 0]; }
            set { this[2, 0] = value; }
        }

        public double Y1
        {
            get { return (double)this[0, 1]; }
            set { this[0, 1] = value; }
        }

        public double Y2
        {
            get { return (double)this[1, 1]; }
            set { this[1, 1] = value; }
        }

        public double OffsetY
        {
            get { return (double)this[2, 1]; }
            set { this[2, 1] = value; }
        }

        public double W1
        {
            get { return (double)this[0, 2]; }
            set { this[0, 2] = value; }
        }

        public double W2
        {
            get { return (double)this[1, 2]; }
            set { this[1, 2] = value; }
        }

        public double W3
        {
            get { return (double)this[2, 2]; }
            set { this[2, 2] = value; }
        }
        #endregion
    }
}
