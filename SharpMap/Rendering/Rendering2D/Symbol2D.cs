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
using System.IO;
using System.Runtime.Serialization;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IAffineMatrixD = NPack.Interfaces.IAffineTransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a 2 dimensional graphical symbol used for point data on a map.
    /// </summary>
    public class Symbol2D : Symbol<Point2D, Size2D>
    {

        #region Object construction / disposal

        /// <summary>
        /// Creates a new, blank symbol with the given <paramref name="size"/>.
        /// </summary>
        /// <param name="size">The base size of the symbol.</param>
        public Symbol2D(Size2D size)
            : base(size) { }

        /// <summary>
        /// Creates a new symbol with the given <paramref name="symbolData">data</paramref>
        /// and <paramref name="size"/>.
        /// </summary>
        /// <param name="symbolData">The data which encodes this symbol.</param>
        /// <param name="size">The base size of the symbol.</param>
        public Symbol2D(Stream symbolData, Size2D size)
            : base(symbolData, size)
        {
        }

        /// <summary>
        /// Creates a new symbol with the given <paramref name="symbolData">data</paramref>
        /// and <paramref name="size"/>.
        /// </summary>
        /// <param name="symbolData">The data which encodes this symbol.</param>
        /// <param name="size">The base size of the symbol.</param>
        public Symbol2D(Byte[] symbolData, Size2D size)
            : this(new MemoryStream(symbolData), size)
        {
        }

        protected Symbol2D(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Returns a String description of this symbol.
        /// </summary>
        /// <returns>
        /// A String representing the value of this <see cref="Symbol2D"/>.
        /// </returns>
        public override String ToString()
        {
            CheckDisposed();
            return String.Format(
                    "[{0}] Size: {1}; Data Hash: {2}; Affine Transform: {3}; Color Transform: {4}; Offset: {5}; Rotation: {6:N}; ScaleX: {7:N}; ScaleY: {8:N}",
                    GetType(), Size, SymbolDataHash, AffineTransform, ColorTransform, Offset, Rotation, ScaleX, ScaleY);
        }

        /// <summary>
        /// Gets or sets a <see cref="Matrix2D"/> object used 
        /// to transform this <see cref="Symbol2D"/>.
        /// </summary>
        public new Matrix2D AffineTransform
        {
            get
            {
                return base.AffineTransform as Matrix2D;
            }
            // TODO: need to compute a decomposition to get _rotationTransform, _scaleTransform and _translationTransform
            set
            {
                if (value != null && value != Matrix2D.Identity)
                {
                    throw new NotSupportedException(
                        "Setting affine transform directly with a value other than " +
                        "null or Matrix2D.Identity is not supported. " +
                        "Use ScaleX, ScaleY, SetOffset and Rotation.");
                }

                base.AffineTransform = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of rotation to render this symbol with in radians.
        /// </summary>
        public new Double Rotation
        {
            get
            {
                return Math.Asin((Double)base.Rotation[1, 0])
                    + (Double)base.Rotation[0, 0] < 0 ? Math.PI : 0;
            }
            set
            {
                Matrix2D matrix = new Matrix2D();
                matrix.Rotate(value);
                base.Rotation = matrix;
            }
        }

        /// <summary>
        /// Scales this symbol's width and height.
        /// </summary>
        public new void Scale(Double amount)
        {
            Matrix2D scaleMatrix = new Matrix2D();
            scaleMatrix.M11 = scaleMatrix.M22 = amount;
            base.Scale = (IAffineMatrixD)base.Scale.Multiply(scaleMatrix);
        }

        /// <summary>
        /// Gets or sets a value by which to scale this symbol's width.
        /// </summary>
        public Double ScaleX
        {
            get
            {
                return (Double)base.Scale[0, 0];
            }
            set
            {
                base.Scale[0, 0] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value by which to scale this symbol's height.
        /// </summary>
        public Double ScaleY
        {
            get
            {
                return (Double)base.Scale[1, 1];
            }
            set
            {
                base.Scale[1, 1] = value;
            }
        }

        #region Symbol`2 implementation

        protected override Symbol<Point2D, Size2D> CreateNew(Size2D size)
        {
            return new Symbol2D(size);
        }

        protected override IAffineMatrixD CreateIdentityMatrix()
        {
            return new Matrix2D();
        }

        protected override IAffineMatrixD CreateMatrix(IMatrixD matrix)
        {
            return new Matrix2D(matrix);
        }

        protected override Point2D GetOffset(IAffineMatrixD translationMatrix)
        {
            if(!(translationMatrix is Matrix2D))
            {
                throw new ArgumentException("Parameter 'translationMatrix' must be a Matrix2D.");
            }

            Matrix2D matrix = translationMatrix as Matrix2D;
            return new Point2D(matrix.OffsetX, matrix.OffsetY);
        }

        protected override void SetOffset(Point2D offset)
        {

        }
        #endregion
    }
}