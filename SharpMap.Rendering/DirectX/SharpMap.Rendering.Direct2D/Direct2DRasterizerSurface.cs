using System;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    public class Direct2DRasterizerSurface : IRasterizeSurface<RenderTarget, RenderTarget>
    {
        #region IRasterizeSurface<RenderTarget,RenderTarget> Members

        public RenderTarget BackSurface
        {
            get { throw new NotImplementedException(); }
        }

        public RenderTarget FrontSurface
        {
            get { throw new NotImplementedException(); }
        }

        public IRasterizers<RenderTarget, RenderTarget> RetrieveSurface()
        {
            throw new NotImplementedException();
        }

        public IRasterizers<RenderTarget, RenderTarget> CreateSurface()
        {
            throw new NotImplementedException();
        }

        public IMapView2D MapView
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler RenderComplete;

        public void RenderCompleted()
        {
            throw new NotImplementedException();
        }

        object IRasterizeSurface.BackSurface
        {
            get { return BackSurface; }
        }

        object IRasterizeSurface.FrontSurface
        {
            get { return FrontSurface; }
        }

        IRasterizers IRasterizeSurface.RetrieveSurface()
        {
            return RetrieveSurface();
        }

        IRasterizers IRasterizeSurface.CreateSurface()
        {
            return CreateSurface();
        }

        #endregion
    }
}