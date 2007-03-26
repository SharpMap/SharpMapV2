using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SharpMap.Layers;
using SharpMap.Styles;

namespace SharpMap.Rendering.Renderers
{
    public abstract class BaseRasterRenderer2D<TStyle, TRenderObject> : IRasterRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>
        where TStyle : class, IStyle
    {
        #region IRasterLayerRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D> Members

        public abstract void DrawRaster(Stream rasterData, ViewRectangle2D viewBounds, ViewRectangle2D rasterBounds);

        public abstract void DrawRaster(Stream rasterData, ViewRectangle2D viewBounds, ViewRectangle2D rasterBounds, IViewMatrix rasterTransform);

        #endregion

        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members

        public IStyle Style
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public SharpMap.Rendering.Thematics.ITheme Theme
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IViewTransformer<ViewPoint2D, ViewRectangle2D> ViewTransformer
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public StyleRenderingMode StyleRenderingMode
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IList<TRenderObject> RenderedObjects
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
