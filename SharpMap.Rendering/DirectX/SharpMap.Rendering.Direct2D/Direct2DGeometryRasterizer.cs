using System;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    public class Direct2DGeometryRasterizer : Direct2DRasterizer, IGeometryRasterizer<RenderTarget, RenderTarget>
    {
        public Direct2DGeometryRasterizer(RenderTarget surface, RenderTarget context) : base(surface, context)
        {
        }

        #region IGeometryRasterizer<RenderTarget,RenderTarget> Members

        public void Rasterize(IFeatureDataRecord record, GeometryStyle style, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}