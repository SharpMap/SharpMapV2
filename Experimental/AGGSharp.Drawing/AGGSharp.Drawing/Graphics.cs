using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;
using Microsoft.Practices.Unity;
using AGGSharp.Drawing.Interface;
using AGG.VertexSource;
namespace AGGSharp.Drawing
{
    public class Graphics
        : IGraphics
    {
        private readonly Stack<IGraphicsContext> _contexts
            = new Stack<IGraphicsContext>();


        private IGraphicsContext GraphicsContext
        {
            get
            {
                return _contexts.Peek();
            }
        }

        private IInternalPixMap _pixMapInternals;
        [Dependency]
        internal IInternalPixMap PixMapInternal
        {
            set
            {
                _pixMapInternals = value;
            }
        }

        private IPixMap _pixMap;

        [InjectionConstructor]
        public Graphics(IPixMap pixMap)
            : this()
        {
            _pixMap = pixMap;
        }

        private Graphics()
        {
            BeginGraphicsContext();
        }


        public static Graphics FromPixMap(IPixMap map)
        {
            return new Graphics(map);
        }

        #region IGraphics Members


        public AGG.Transform.ITransform Transform
        {
            get
            {
                return GraphicsContext.Transform;
            }
            set
            {
                GraphicsContext.Transform = value;
            }
        }

        public IAlphaMask AlphaMask
        {
            get
            {
                return GraphicsContext.Mask;
            }
            set
            {
                GraphicsContext.Mask = value;
            }
        }

        public GeoAPI.Geometries.IExtents2D ClipBounds
        {
            get
            {
                return GraphicsContext.ClipBounds;
            }
            set
            {
                GraphicsContext.ClipBounds = value;
            }
        }

        public void Clear(IColorType color)
        {
            throw new NotImplementedException();
        }

        public void BeginGraphicsContext()
        {
            _contexts.Push(
                IoC.Instance.Resolve<IGraphicsContext>()
                );
        }

        public uint Stride
        {
            get
            {
                return _pixMap.Stride;
            }
        }

        public void BeginGraphicsContext(AGG.Transform.ITransform transform, IAlphaMask mask)
        {
            throw new NotImplementedException();
        }

        public void EndGraphicsContext()
        {
            _contexts.Pop();
        }

        public void DrawArc(IPen pen, GeoAPI.Geometries.IExtents2D rect, double startAngle, double sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(IPen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(IPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawBezier(IPen pen, GeoAPI.Geometries.IPoint2D pt1, GeoAPI.Geometries.IPoint2D pt2, GeoAPI.Geometries.IPoint2D pt3, GeoAPI.Geometries.IPoint2D pt4)
        {
            throw new NotImplementedException();
        }

        public void DrawBezier(IPen pen, double pt1x, double pt1y, double pt2x, double pt2y, double pt3x, double pt3y, double pt4x, double pt4y)
        {
            throw new NotImplementedException();
        }

        public void DrawBeziers<TPoint>(IPen pen, TPoint[] controlPoints) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawBeziers<TPoint>(IPen pen, IEnumerable<TPoint> controlPoints) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve<TPoint>(IPen pen, TPoint[] points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve<TPoint>(IPen pen, TPoint[] points, double tension, FillMode fillmode) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve<TPoint>(IPen pen, IEnumerable<TPoint> points, double tension, FillMode fillmode) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawCurve<TPoint>(IPen pen, TPoint[] points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawCurve<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawCurve<TPoint>(IPen pen, TPoint[] points, double tension) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawCurve<TPoint>(IPen pen, IEnumerable<TPoint> points, double tension) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawCurve<TPoint>(IPen pen, TPoint[] points, uint offset, uint numberOfSegments) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawCurve<TPoint>(IPen pen, IEnumerable<TPoint> points, uint offset, uint numberOfSegments) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(IPen pen, GeoAPI.Geometries.IExtents boundingRect)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse<TPoint>(IPen pen, TPoint lowerBoundingBoxCoord, TPoint upperBoundingBoxCoord) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(IPen pen, double x1, double y1, double x2, double y2)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(PixMap srcImage, GeoAPI.Geometries.IPoint2D point)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(PixMap srcImage, GeoAPI.Geometries.IPoint2D referencePoint, Position position)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(PixMap srcImage, double x, double y)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(PixMap srcImage, double x, double y, Position position)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(PixMap srcImage, GeoAPI.Geometries.IExtents2D srcRect, GeoAPI.Geometries.IExtents2D destRect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(PixMap srcImage, GeoAPI.Geometries.IExtents2D destRect)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(PixMap srcImage, GeoAPI.Geometries.IPoint2D point)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(PixMap srcImage, GeoAPI.Geometries.IPoint2D referencePoint, Position position)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(PixMap srcImage, double x, double y)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(PixMap srcImage, double x, double y, Position position)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaledAndClipped(PixMap srcImage, GeoAPI.Geometries.IExtents2D destRect)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(IPen pen, GeoAPI.Geometries.IPoint2D p1, GeoAPI.Geometries.IPoint2D p2)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(IPen pen, double x1, double y1, double x2, double y2)
        {
            PathStorage p = new PathStorage();
            p.move_to(x1, y1);
            p.line_to(x2, y2);
            this._pixMapInternals.Renderer.Rasterizer.add_path(p);

        }

        public void DrawLines<TPoint>(IPen pen, TPoint[] points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawLines<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawLines(IPen pen, double[] xcoords, double[] ycoords)
        {
            throw new NotImplementedException();
        }

        public void DrawLines(IPen pen, double[] coords)
        {
            throw new NotImplementedException();
        }

        public void DrawPath(IPen pen, IGraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public void DrawVertices(IPen pen, AGG.VertexSource.IVertexSource vertexSource)
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon<TPoint>(IPen pen, TPoint[] points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(IPen pen, GeoAPI.Geometries.IExtents2D rect)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(IPen pen, double x1, double y1, double x2, double y2)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangles<TExtents>(IPen pen, TExtents[] rectangles) where TExtents : GeoAPI.Geometries.IExtents2D
        {
            throw new NotImplementedException();
        }

        public void DrawRectangles<TExtents>(IPen pen, IEnumerable<TExtents> rectangles) where TExtents : GeoAPI.Geometries.IExtents2D
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve<TPoint>(IBrush brush, TPoint[] points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve<TPoint>(IBrush brush, IEnumerable<TPoint> points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve<TPoint>(IBrush brush, TPoint[] points, double tension, FillMode fillmode) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve<TPoint>(IBrush brush, IEnumerable<TPoint> points, double tension, FillMode fillmode) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(IBrush brush, GeoAPI.Geometries.IExtents boundingRect)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse<TPoint>(IBrush brush, TPoint lowerBoundingBoxCoord, TPoint upperBoundingBoxCoord) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(IBrush brush, double x1, double y1, double x2, double y2)
        {
            throw new NotImplementedException();
        }

        public void FillPath(IPen pen, IGraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public void FillVertices(IPen pen, AGG.VertexSource.IVertexSource vertexSource)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon<TPoint>(IPen pen, TPoint[] points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillPolygon<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : GeoAPI.Geometries.IPoint2D
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(IPen pen, GeoAPI.Geometries.IExtents2D rect)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(IPen pen, double x1, double y1, double x2, double y2)
        {
            throw new NotImplementedException();
        }

        public void FillRectangles<TExtents>(IPen pen, TExtents[] rectangles) where TExtents : GeoAPI.Geometries.IExtents2D
        {
            throw new NotImplementedException();
        }

        public void FillRectangles<TExtents>(IPen pen, IEnumerable<TExtents> rectangles) where TExtents : GeoAPI.Geometries.IExtents2D
        {
            throw new NotImplementedException();
        }

        public void FillGeometry<TGeometry>(IBrush brush, TGeometry geometry) where TGeometry : GeoAPI.Geometries.IGeometry
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
