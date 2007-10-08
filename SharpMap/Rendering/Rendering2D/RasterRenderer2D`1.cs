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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    public abstract class RasterRenderer2D<TRenderObject> : IRasterRenderer2D<TRenderObject>
    {
        #region IRasterLayerRenderer<Rectangle2D, TRenderObject> Members

        public abstract IEnumerable<TRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                Rectangle2D rasterBounds);

        public abstract IEnumerable<TRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                Rectangle2D rasterBounds, IMatrixD rasterTransform);

        #endregion

        #region IRenderer Members

        public IMatrixD RenderTransform
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public StyleRenderingMode StyleRenderingMode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRasterRenderer<Rectangle2D> Members

        IEnumerable IRasterRenderer<Rectangle2D>.RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds)
        {
            throw new NotImplementedException();
        }

        IEnumerable IRasterRenderer<Rectangle2D>.RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds, IMatrixD rasterTransform)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRasterRenderer2D Members

        IEnumerable IRasterRenderer2D.RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IEnumerable IRasterRenderer2D.RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds, NPack.Interfaces.IMatrix<NPack.DoubleComponent> rasterTransform)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}