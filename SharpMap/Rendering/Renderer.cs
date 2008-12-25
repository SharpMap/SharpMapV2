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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    public abstract class Renderer : IRenderer
    {
        #region Instance fields

        private IMatrixD _renderTransform;
        private RenderingMode _renderMode = RenderingMode.Default;
        private Boolean _isDisposed;

        #endregion

        #region Object construction and disposal

        #region Dispose Pattern

        ~Renderer()
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

        protected virtual void Dispose(Boolean disposing) {}

        protected Boolean IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        #endregion
        #endregion

        #region IRenderer Members

        /// <summary>
        /// Gets or sets a matrix used to transform 
        /// coordinate values during rendering.
        /// </summary>
        public IMatrixD RenderTransform
        {
            get { return _renderTransform; }
            set { _renderTransform = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="StyleRenderingMode"/> 
        /// value used to render objects.
        /// </summary>
        public RenderingMode StyleRenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        #endregion
    }
}
