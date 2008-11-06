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
using GeoAPI.Algorithms;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IAffineMatrixD = NPack.Interfaces.IAffineTransformMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Represents a graphical symbol used for point data on a map.
    /// </summary>
    public abstract class Symbol<TPoint, TSize> : ICloneable, IDisposable, ISerializable
        where TPoint : IVectorD
        where TSize : IVectorD
    {
        private ColorMatrix _colorTransform = ColorMatrix.Identity;
        private Stream _symbolData;
        private String _symbolDataHash;
        private Boolean _disposed;
        private IAffineMatrixD _rotationTransform;
        private IAffineMatrixD _scalingTransform;
        private IAffineMatrixD _translationTransform;
        private TSize _size;

        #region Object construction / disposal

        protected Symbol()
        {
            _symbolData = new MemoryStream(new Byte[] { 0x0, 0x0, 0x0, 0x0 });
            initMatrixes();
        }

        protected Symbol(TSize size)
            : this()
        {
            _size = size;
        }

        protected Symbol(Stream symbolData, TSize size)
        {
            _size = size;

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
                    copy.Write(reader.ReadBytes((Int32)symbolData.Length), 0, (Int32)symbolData.Length);
                }

                symbolData = copy;
            }

            setSymbolData(symbolData);
            initMatrixes();
        }

        #region Dispose Pattern

        ~Symbol()
        {
            Dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Gets a value indicating if the <see cref="Symbol{TPoint,TSize}"/>
        /// is disposed.
        /// </summary>
        public Boolean IsDisposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }

        protected void Dispose(Boolean disposing)
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

        #region Public properties

        /// <summary>
        /// Gets or sets an <see cref="IAffineMatrixD"/> used 
        /// to transform this <see cref="IAffineMatrixD"/>.
        /// </summary>
        public IAffineMatrixD AffineTransform
        {
            get
            {
                CheckDisposed();

                IAffineMatrixD concatenated = CreateMatrix(
                    _rotationTransform
                    .Multiply(_scalingTransform)
                    .Multiply(_translationTransform));

                return CreateMatrix(concatenated);
            }
            // TODO: need to compute a decomposition to get _rotationTransform, _scaleTransform and _translationTransform
            set
            {
                if (value != null && value != value.One)
                {
                    throw new NotSupportedException(
                        "Setting affine transform directly with a value other than " +
                        "null or an identity matrix is not supported. " +
                        "Use ScaleX, ScaleY, SetOffset and Rotation.");
                }

                _rotationTransform = _scalingTransform = _translationTransform = CreateIdentityMatrix();
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="ColorMatrix"/> used to change the color 
        /// of this symbol.
        /// </summary>
        public ColorMatrix ColorTransform
        {
            get { return _colorTransform; }
            set { _colorTransform = value; }
        }

        /// <summary>
        /// Gets or sets a vector by which to offset the symbol.
        /// </summary>
        public TPoint Offset
        {
            get
            {
                CheckDisposed();
                return GetOffset(_translationTransform);
            }
            set
            {
                CheckDisposed();
                SetOffset(value);
            }
        }

        /// <summary>
        /// Gets or sets the size of this symbol.
        /// </summary>
        public TSize Size
        {
            get
            {
                CheckDisposed();
                return _size;
            }
            set
            {
                CheckDisposed();
                _size = value;
            }
        }

        /// <summary>
        /// Gets a stream containing the <see cref="Symbol{TPoint,TSize}"/> 
        /// data.
        /// </summary>
        /// <remarks>
        /// This is often a bitmap or a vector-based image.
        /// </remarks>
        public Stream SymbolData
        {
            get
            {
                CheckDisposed();
                return _symbolData;
            }
            private set
            {
                CheckDisposed();
                if (value == null) throw new ArgumentNullException("value");
                setSymbolData(value);
            }
        }
        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or sets a rotation matrix by which to rotate this symbol.
        /// </summary>
        protected IAffineMatrixD Rotation
        {
            get
            {
                CheckDisposed();
                return _rotationTransform;
            }
            set
            {
                CheckDisposed();

                if (value == null)
                {
                    _rotationTransform = CreateIdentityMatrix();
                }
                else
                {
                    _rotationTransform = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a scale matrix by which to scale this symbol.
        /// </summary>
        protected IAffineMatrixD Scale
        {
            get
            {

                CheckDisposed();
                return _scalingTransform;
            }
            set
            {
                CheckDisposed();

                if (value == null)
                {
                    _scalingTransform = CreateIdentityMatrix();
                }
                else
                {
                    _scalingTransform = value;
                }
            }
        }

        protected String SymbolDataHash
        {
            get { return _symbolDataHash; }
        }

        #endregion

        #region Protected methods
        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        protected abstract Symbol<TPoint, TSize> CreateNew(TSize size);
        protected abstract IAffineMatrixD CreateIdentityMatrix();
        protected abstract IAffineMatrixD CreateMatrix(IMatrixD matrix);
        protected abstract TPoint GetOffset(IAffineMatrixD translationMatrix);
        protected abstract void SetOffset(TPoint offset);
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones this symbol.
        /// </summary>
        /// <returns>
        /// A duplicate of this <see cref="Symbol{TPoint,TSize}"/>.
        /// </returns>
        public Symbol<TPoint, TSize> Clone()
        {
            Symbol<TPoint, TSize> clone = CreateNew(Size);
            MemoryStream copy;
            lock (_symbolData)
            {

                // Record the original position
                Int64 streamPos = _symbolData.Position;
                _symbolData.Seek(0, SeekOrigin.Begin);

                Byte[] buffer = new Byte[_symbolData.Length];
                _symbolData.Read(buffer, 0, buffer.Length);
                copy = new MemoryStream(buffer);

                // Restore the original position
                _symbolData.Position = streamPos;
            }
            clone.SymbolData = copy;
            clone._symbolDataHash = _symbolDataHash;
            clone.ColorTransform = ColorTransform.Clone();
            clone._rotationTransform = CreateMatrix(_rotationTransform.Clone());
            clone._translationTransform = CreateMatrix(_translationTransform.Clone());
            clone._scalingTransform = CreateMatrix(_scalingTransform.Clone());
            return clone;
        }

        /// <summary>
        /// Clones this symbol.
        /// </summary>
        /// <returns>
        /// A duplicate of this <see cref="Symbol{TPoint,TSize}"/> 
        /// as an object reference.
        /// </returns>
        Object ICloneable.Clone()
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

        #region Private helper methods

        private void initMatrixes()
        {
            _rotationTransform = CreateIdentityMatrix();
            _translationTransform = CreateIdentityMatrix();
            _scalingTransform = CreateIdentityMatrix();
        }


        private void setSymbolData(Stream symbolData)
        {
            _symbolData = symbolData;
            _symbolDataHash = Hash.AsString(_symbolData);
        }
        #endregion
    }
}