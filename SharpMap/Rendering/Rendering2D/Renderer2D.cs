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
using SharpMap.Rendering.Rasterize;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    public abstract class Renderer2D : IRenderer
    {

        #region Instance fields

        private Matrix2D _renderTransform = new Matrix2D();
        private StyleRenderingMode _renderMode = StyleRenderingMode.Default;
        private Boolean _isDisposed = false;

        #endregion

        #region Object construction and disposal

        #region Dispose Pattern

        ~Renderer2D()
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
        public Matrix2D RenderTransform
        {
            get { return _renderTransform; }
            set { _renderTransform = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="StyleRenderingMode"/> 
        /// value used to render objects.
        /// </summary>
        public StyleRenderingMode StyleRenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        #endregion


        #region Explicit Interface Implementation

        #region IRenderer Members

        IMatrixD IRenderer.RenderTransform
        {
            get { return RenderTransform; }
            set
            {
                if (!(value is Matrix2D))
                {
                    throw new ArgumentException("Only a Matrix2D is supported on a Renderer2D instance.", "value");
                }

                RenderTransform = value as Matrix2D;
            }
        }

        #endregion

        #endregion
    }
}
