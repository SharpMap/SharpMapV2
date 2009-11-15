using System;
using SharpMap.Rendering.Rasterize;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    public abstract class Direct2DRasterizer : IRasterizer<RenderTarget, RenderTarget>
    {
        protected Direct2DRasterizer(RenderTarget surface, RenderTarget context)
        {
            Surface = surface;
            Context = context;
        }

        #region IRasterizer<RenderTarget,RenderTarget> Members

        public RenderTarget Surface { get; protected set; }

        public RenderTarget Context { get; protected set; }

        object IRasterizer.Surface
        {
            get { return Surface; }
        }

        object IRasterizer.Context
        {
            get { return Context; }
        }

        public virtual void BeginPass()
        {
            Surface.BeginDraw();
        }

        public virtual void EndPass()
        {
            Surface.EndDraw();
        }

        #endregion
    }
}