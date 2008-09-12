using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;
using AGG.Transform;
using GeoAPI.Geometries;
using AGG.VertexSource;

namespace AGGSharp.Drawing.Interface
{
    /// <summary>
    /// Basen on System.Drawing.Graphics from .Net BCL
    /// </summary>
    public interface IGraphics
    {
        //uint DpiX { get; }
        //uint DpiY { get; }
        uint Stride { get; }
        ITransform Transform { get; set; }
        IAlphaMask AlphaMask { get; set; }
        IExtents2D ClipBounds { get; set; }
        void Clear(IColorType color);

        void BeginGraphicsContext();
        void BeginGraphicsContext(ITransform transform, IAlphaMask mask);
        void EndGraphicsContext();


        void DrawArc(IPen pen, IExtents2D rect, double startAngle, double sweepAngle);
        void DrawArc(IPen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);
        void DrawArc(IPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle);

        void DrawBezier(IPen pen, IPoint2D pt1, IPoint2D pt2, IPoint2D pt3, IPoint2D pt4);
        void DrawBezier(IPen pen, double pt1x, double pt1y, double pt2x, double pt2y, double pt3x, double pt3y, double pt4x, double pt4y);

        void DrawBeziers<TPoint>(IPen pen, TPoint[] controlPoints) where TPoint : IPoint2D;
        void DrawBeziers<TPoint>(IPen pen, IEnumerable<TPoint> controlPoints) where TPoint : IPoint2D;

        void DrawClosedCurve<TPoint>(IPen pen, TPoint[] points) where TPoint : IPoint2D;
        void DrawClosedCurve<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : IPoint2D;

        void DrawClosedCurve<TPoint>(IPen pen, TPoint[] points, double tension, FillMode fillmode) where TPoint : IPoint2D;
        void DrawClosedCurve<TPoint>(IPen pen, IEnumerable<TPoint> points, double tension, FillMode fillmode) where TPoint : IPoint2D;

        void DrawCurve<TPoint>(IPen pen, TPoint[] points) where TPoint : IPoint2D;
        void DrawCurve<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : IPoint2D;

        void DrawCurve<TPoint>(IPen pen, TPoint[] points, double tension) where TPoint : IPoint2D;
        void DrawCurve<TPoint>(IPen pen, IEnumerable<TPoint> points, double tension) where TPoint : IPoint2D;

        void DrawCurve<TPoint>(IPen pen, TPoint[] points, uint offset, uint numberOfSegments) where TPoint : IPoint2D;
        void DrawCurve<TPoint>(IPen pen, IEnumerable<TPoint> points, uint offset, uint numberOfSegments) where TPoint : IPoint2D;

        void DrawEllipse(IPen pen, IExtents boundingRect);
        void DrawEllipse<TPoint>(IPen pen, TPoint lowerBoundingBoxCoord, TPoint upperBoundingBoxCoord) where TPoint : IPoint2D;
        void DrawEllipse(IPen pen, double x1, double y1, double x2, double y2);

        void DrawImage(PixMap srcImage, IPoint2D point);
        void DrawImage(PixMap srcImage, IPoint2D referencePoint, Position position);
        void DrawImage(PixMap srcImage, double x, double y);
        void DrawImage(PixMap srcImage, double x, double y, Position position);

        void DrawImage(PixMap srcImage, IExtents2D srcRect, IExtents2D destRect);
        void DrawImage(PixMap srcImage, IExtents2D destRect);


        void DrawImageUnscaled(PixMap srcImage, IPoint2D point);
        void DrawImageUnscaled(PixMap srcImage, IPoint2D referencePoint, Position position);
        void DrawImageUnscaled(PixMap srcImage, double x, double y);
        void DrawImageUnscaled(PixMap srcImage, double x, double y, Position position);

        void DrawImageUnscaledAndClipped(PixMap srcImage, IExtents2D destRect);

        void DrawLine(IPen pen, IPoint2D p1, IPoint2D p2);
        void DrawLine(IPen pen, double x1, double y1, double x2, double y2);

        void DrawLines<TPoint>(IPen pen, TPoint[] points) where TPoint : IPoint2D;
        void DrawLines<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : IPoint2D;

        void DrawLines(IPen pen, double[] xcoords, double[] ycoords);
        void DrawLines(IPen pen, double[] coords);//coords = new double[]{x1, y1, x2, y2...

        void DrawPath(IPen pen, IGraphicsPath path);
        void DrawVertices(IPen pen, IVertexSource vertexSource);

        void DrawPolygon<TPoint>(IPen pen, TPoint[] points) where TPoint : IPoint2D;
        void DrawPolygon<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : IPoint2D;

        void DrawRectangle(IPen pen, IExtents2D rect);
        void DrawRectangle(IPen pen, double x1, double y1, double x2, double y2);

        void DrawRectangles<TExtents>(IPen pen, TExtents[] rectangles) where TExtents : IExtents2D;
        void DrawRectangles<TExtents>(IPen pen, IEnumerable<TExtents> rectangles) where TExtents : IExtents2D;

        //TODO : draw rounded rect

        //TODO : draw string



        void FillClosedCurve<TPoint>(IBrush brush, TPoint[] points) where TPoint : IPoint2D;
        void FillClosedCurve<TPoint>(IBrush brush, IEnumerable<TPoint> points) where TPoint : IPoint2D;

        void FillClosedCurve<TPoint>(IBrush brush, TPoint[] points, double tension, FillMode fillmode) where TPoint : IPoint2D;
        void FillClosedCurve<TPoint>(IBrush brush, IEnumerable<TPoint> points, double tension, FillMode fillmode) where TPoint : IPoint2D;


        void FillEllipse(IBrush brush, IExtents boundingRect);
        void FillEllipse<TPoint>(IBrush brush, TPoint lowerBoundingBoxCoord, TPoint upperBoundingBoxCoord) where TPoint : IPoint2D;
        void FillEllipse(IBrush brush, double x1, double y1, double x2, double y2);

        void FillPath(IPen pen, IGraphicsPath path);
        void FillVertices(IPen pen, IVertexSource vertexSource);

        void FillPolygon<TPoint>(IPen pen, TPoint[] points) where TPoint : IPoint2D;
        void FillPolygon<TPoint>(IPen pen, IEnumerable<TPoint> points) where TPoint : IPoint2D;

        void FillRectangle(IPen pen, IExtents2D rect);
        void FillRectangle(IPen pen, double x1, double y1, double x2, double y2);

        void FillRectangles<TExtents>(IPen pen, TExtents[] rectangles) where TExtents : IExtents2D;
        void FillRectangles<TExtents>(IPen pen, IEnumerable<TExtents> rectangles) where TExtents : IExtents2D;

        void FillGeometry<TGeometry>(IBrush brush, TGeometry geometry) where TGeometry : IGeometry;


    }
}
