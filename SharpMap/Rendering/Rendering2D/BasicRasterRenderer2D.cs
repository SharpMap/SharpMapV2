using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NPack;
using NPack.Interfaces;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rendering2D
{
    public class BasicRasterRenderer2D<TRenderObject> : IRasterRenderer2D

    {
        private readonly RasterRenderer2D<TRenderObject> _rasterRenderer2D;

        public BasicRasterRenderer2D(RasterRenderer2D<TRenderObject> rasterRenderer2D)
        {
            _rasterRenderer2D = rasterRenderer2D;
        }

        #region Events

        /// <summary>
        /// Event fired when a feature is about to render to the render stream.
        /// </summary>
        public event CancelEventHandler RasterRendering;

        /// <summary>
        /// Event fired when a feature has been rendered.
        /// </summary>
        public event EventHandler RasterRendered;

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _rasterRenderer2D.Dispose();
        }

        #endregion

        #region Implementation of IRenderer

        public IMatrix<DoubleComponent> RenderTransform
        {
            get { return _rasterRenderer2D.RenderTransform; }
            set { _rasterRenderer2D.RenderTransform = new Matrix2D(value); }
        }

        public StyleRenderingMode StyleRenderingMode
        {
            get { return _rasterRenderer2D.StyleRenderingMode; }
            set { _rasterRenderer2D.StyleRenderingMode = value; }
        }

        #endregion

        #region Implementation of IRasterRenderer<Rectangle2D>

        public IEnumerable RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds)
        {
            return _rasterRenderer2D.RenderRaster(rasterData, viewBounds,rasterBounds);
        }

        public IEnumerable RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds, IMatrix<DoubleComponent> rasterTransform)
        {
            return _rasterRenderer2D.RenderRaster(rasterData, viewBounds,rasterBounds,rasterTransform);
        }

        #endregion
    }
}