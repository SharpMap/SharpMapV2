using System;
using System.Drawing;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Gdi
{
    public class GdiTextRasterizer : GdiRasterizer, ITextRasterizer<Bitmap, Graphics>
    {
        public GdiTextRasterizer(Bitmap surface, Graphics context)
            : base(surface, context)
        {
        }

        #region ITextRasterizer<Bitmap,Graphics> Members

        public void Rasterize(IGeometry geometry, string text, LabelStyle style, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}