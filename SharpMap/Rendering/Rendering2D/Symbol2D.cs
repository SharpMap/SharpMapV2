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
using System.IO;
using System.Runtime.Serialization;
using NPack;
using NPack.Interfaces;
using SharpMap.Utilities;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a 2 dimensional graphic used for point data on a map.
    /// </summary>
    public sealed class Symbol2D : ICloneable, IDisposable, ISerializable
    {
        private ColorMatrix _colorTransform = ColorMatrix.Identity;
        private Matrix2D _rotationTransform = Matrix2D.Identity;
        private Matrix2D _scalingTransform = Matrix2D.Identity;
        private Matrix2D _translationTransform = Matrix2D.Identity;
        private Stream _symbolData;
        private Rectangle2D _symbolBox;
        private string _symbolDataHash;
        private bool _disposed;

        #region Object Construction/Destruction

        public Symbol2D(Size2D size)
        {
            _symbolData = new MemoryStream(new byte[] { 0x0, 0x0, 0x0, 0x0 });
            _symbolBox = new Rectangle2D(Point2D.Zero, size);
        }

        public Symbol2D(Stream symbolData, Size2D size)
        {
            _symbolBox = new Rectangle2D(Point2D.Zero, size);

            if (!symbolData.CanSeek)
            {
                if (symbolData.Position != 0)
                {
                    throw new InvalidOperationException(
                        "Symbol data stream isn't at the beginning, and it can't be repositioned");
                }

                MemoryStream copy = new MemoryStream();

                using (BinaryReader reader = new BinaryReader(symbolData))
                {
                    copy.Write(reader.ReadBytes((int)symbolData.Length), 0, (int)symbolData.Length);
                }

                symbolData = copy;
            }

            _symbolData = symbolData;
            _symbolDataHash = Hash.AsString(_symbolData);
        }

        public Symbol2D(byte[] symbolData, Size2D size)
            : this(new MemoryStream(symbolData), size)
        {
        }

        private Symbol2D(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #region Dispose Pattern

        ~Symbol2D()
        {
            dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        public bool IsDisposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }

        private void dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (_symbolData != null)
                {
                    _symbolData.Dispose();
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns a string description of this symbol.
        /// </summary>
        /// <returns>A string representing the value of this <see cref="Symbol2D"/>.</returns>
        public override string ToString()
        {
            checkDisposed();
            return
                String.Format(
                    "[{0}] Size: {1}; Data Hash: {2}; Affine Transform: {3}; Color Transform: {4}; Offset: {5}; Rotation: {6:N}; ScaleX: {7:N}; ScaleY: {8:N}",
                    GetType(), Size, _symbolDataHash, AffineTransform, ColorTransform, Offset, Rotation, ScaleX, ScaleY);
        }

        /// <summary>
        /// Gets or sets the size of this symbol.
        /// </summary>
        public Size2D Size
        {
            get { checkDisposed(); return _symbolBox.Size; }
            set { checkDisposed(); _symbolBox = new Rectangle2D(new Point2D(0, 0), value); }
        }

        /// <summary>
        /// Gets a stream containing the <see cref="Symbol2D"/> data.
        /// </summary>
        /// <remarks>
        /// This is often a bitmap or a vector-based image.
        /// </remarks>
        public Stream SymbolData
        {
            get { checkDisposed(); return _symbolData; }
            private set { checkDisposed(); _symbolData = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="Matrix2D"/> object used to transform this <see cref="Symbol2D"/>.
        /// </summary>
        public Matrix2D AffineTransform
        {
            get
            {
                checkDisposed();
                IMatrix<DoubleComponent> concatenated =
                    _rotationTransform.Multiply(_scalingTransform).Multiply(_translationTransform);
                return new Matrix2D(concatenated); 
            }
            // TODO: need to compute a decomposition to get _rotationTransform, _scaleTransform and _translationTransform
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a <see cref="ColorMatrix"/> used to change the color of this symbol.
        /// </summary>
        public ColorMatrix ColorTransform
        {
            get { return _colorTransform; }
            set { _colorTransform = value; }
        }

        /// <summary>
        /// Gets or sets a vector by which to offset the symbol.
        /// </summary>
        public Point2D Offset
        {
            get { checkDisposed(); return new Point2D(_translationTransform.OffsetX, _translationTransform.OffsetY); }
            set
            {
                checkDisposed();
                _translationTransform.OffsetX = value.X;
                _translationTransform.OffsetY = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets a value by which to rotate this symbol, in radians.
        /// </summary>
        public double Rotation
        {
            get
            {
                checkDisposed();
                return Math.Asin(_rotationTransform.M21) 
                    + _rotationTransform.M11 < 0 ? Math.PI : 0;
            }
            set { checkDisposed(); _rotationTransform.Rotate(value); }
        }

        /// <summary>
        /// Sets a value by which to scale this symbol's width and height.
        /// </summary>
        public double Scale
        {
            set { checkDisposed(); _scalingTransform.M11 = _scalingTransform.M22 = value; }
        }

        /// <summary>
        /// Gets or sets a value by which to scale this symbol's width.
        /// </summary>
        public double ScaleX
        {
            get { checkDisposed(); return _scalingTransform.M11; }
            set { checkDisposed(); _scalingTransform.M11 = value; }
        }

        /// <summary>
        /// Gets or sets a value by which to scale this symbol's height.
        /// </summary>
        public double ScaleY
        {
            get { checkDisposed(); return _scalingTransform.M22; }
            set { checkDisposed(); _scalingTransform.M22 = value; }
        }

        #region Private helper methods

        private void checkDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones this symbol.
        /// </summary>
        /// <returns>
        /// A duplicate of this <see cref="Symbol2D"/>.
        /// </returns>
        public Symbol2D Clone()
        {
            Symbol2D clone = new Symbol2D(_symbolBox.Size);

            // Record the original position
            long streamPos = _symbolData.Position;
            _symbolData.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[_symbolData.Length];
            _symbolData.Read(buffer, 0, buffer.Length);
            MemoryStream copy = new MemoryStream(buffer);

            // Restore the original position
            _symbolData.Position = streamPos;

            clone.SymbolData = copy;
            clone._symbolDataHash = _symbolDataHash;
            clone.ColorTransform = ColorTransform.Clone();
            clone._rotationTransform = _rotationTransform.Clone();
            clone._translationTransform = _translationTransform.Clone();
            clone._scalingTransform = _scalingTransform.Clone();
            return clone;
        }

        /// <summary>
        /// Clones this symbol.
        /// </summary>
        /// <returns>A duplicate of this <see cref="Symbol2D"/> as an object reference.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}