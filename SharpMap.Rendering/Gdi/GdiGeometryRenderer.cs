using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Gdi
{
    public class GdiGeometryRasterizer : GdiRasterizer, IGeometryRasterizer<Bitmap, Graphics>
    {
        readonly Dictionary<IGeometry, GraphicsPath> _pathCache = new Dictionary<IGeometry, GraphicsPath>();
        readonly Dictionary<Symbol2D, Image> _symbolCache = new Dictionary<Symbol2D, Image>();

        public GdiGeometryRasterizer(Bitmap surface, Graphics context)
            : base(surface, context)
        {
        }

        #region IGeometryRasterizer<Bitmap,Graphics> Members

        public void Rasterize(IGeometry geometry, GeometryStyle style, Matrix2D transform)
        {
            GraphicsPath path = GetGraphicsPath(geometry, transform);
            if (style.Fill != null)
                Context.FillPath(ViewConverter.Convert(style.Fill), path);
            if (style.Line != null)
                Context.DrawPath(ViewConverter.Convert(style.Line), path);
            //if (style.Symbol != null)
            //{
            //    RectangleF rect = path.GetBounds();
            //    Context.DrawImage(ConvertSymbol(style.Symbol), rect.Left - rect.Width / 2,
            //                      rect.Top - rect.Height / 2);
            //}
        }

        private Image ConvertSymbol(Symbol2D symbol2D)
        {
            Image img;
            if (_symbolCache.TryGetValue(symbol2D, out img))
                return img;

            throw new NotImplementedException();
        }

        private GraphicsPath GetGraphicsPath(IGeometry geometry, Matrix2D transform)
        {
            GraphicsPath path;
            if (!_pathCache.TryGetValue(geometry, out path))
            {
                path = CreateGraphicsPath(geometry, transform);
                _pathCache.Add(geometry, path);
            }
            return path;
        }

        private GraphicsPath CreateGraphicsPath(IGeometry geometry, Matrix2D transform)
        {
            GraphicsPath path = new GraphicsPath(FillMode.Winding);
            //TODO : do properly - quick test for now.
            ICoordinateSequence seq = geometry.Coordinates;
            Point2D prev = default(Point2D);
            for (int i = 0; i < seq.Count; i++)
            {
                ICoordinate coord = seq[i];
                Point2D curr = transform.TransformVector(coord[Ordinates.X], coord[Ordinates.Y]);
                if (i > 0)
                {
                    path.AddLine((float)prev.X, (float)prev.Y, (float)curr.X, (float)curr.Y);
                }

                prev = curr;
            }
            return path;
        }

        #endregion
    }
}