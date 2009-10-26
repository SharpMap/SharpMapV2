using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Cairo;
using GeoAPI.Coordinates;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Cairo
{
    internal class RenderObject
    {
        private Collection<Path> _paths;
        private Collection<PointD> _points;

        public ICollection<Path> Paths
        {
            get
            {
                if (_paths == null)
                    _paths = new Collection<Path>();

                return _paths;
            }
        }

        public ICollection<PointD> Points
        {
            get
            {
                if (_points == null)
                    _points = new Collection<PointD>();
                return _points;
            }
        }

        public bool HasPaths
        {
            get { return _paths != null; }
        }

        public bool HasPoints
        {
            get { return _points != null; }
        }
    }

    internal enum PathType
    {
        Polygon,
        Line
    }

    internal struct Path
    {
        public PathType PathType { get; set; }
        public global::Cairo.Path GraphicsPath { get; set; }
    }

    public class CairoGeometryRasterizer : CairoRasterizer, IGeometryRasterizer<Surface, Context>
    {
        private readonly Dictionary<IGeometry, RenderObject> _pathCache = new Dictionary<IGeometry, RenderObject>();
        private readonly Dictionary<Symbol2D, Surface> _symbolCache = new Dictionary<Symbol2D, Surface>();


        public CairoGeometryRasterizer(Surface surface, Context context)
            : base(surface, context)
        {
        }

        #region IGeometryRasterizer<Surface,Context> Members

        public void Rasterize(IFeatureDataRecord record, GeometryStyle style, Matrix2D transform)
        {
            IGeometry geometry = record.Geometry;
            Context.Antialias = ConvertAntiAlias(style.RenderingMode);
            RenderObject path = GetGraphicsPath(geometry, transform);
            if (path.HasPaths)
            {
                if (style.Fill != null)
                {
                    foreach (Path p in path.Paths)
                        if (p.PathType == PathType.Polygon)
                            FillPath(Context, p.GraphicsPath, ViewConverter.Convert(style.Fill));
                }
                if (style.Line != null)
                {
                    foreach (Path p in path.Paths)
                        StrokePath(Context, p.GraphicsPath, ViewConverter.Convert(style.Line));
                }
            }
            if (path.HasPoints && style.Symbol != null)
            {
                foreach (PointD p in path.Points)
                    DrawSurface(Context, new PointD(p.X - style.Symbol.Size.Width / 2,
                                      p.Y - style.Symbol.Size.Height / 2), ConvertSymbol(style.Symbol));
            }

        }

        private static void DrawSurface(Context context, PointD pointD, Surface surface)
        {
            //context.SetSource(surface, pointD.X, pointD.Y);
            //context.Paint();
        }

        #endregion


        private static void FillPath(Context context, global::Cairo.Path path, StyleBrush brush)
        {
            context.FillRule = FillRule.EvenOdd;
            SetColour(context, brush);
            context.AppendPath(path);
            context.FillPreserve();
        }

        private static void StrokePath(Context context, global::Cairo.Path path, StylePen outline)
        {
            context.SetDash(ConvertDash(outline.DashPattern), outline.DashOffset);
            context.LineCap = ConvertCap(outline.EndCap);
            context.LineJoin = ConvertJoin(outline.LineJoin);
            context.LineWidth = outline.Width;
            context.Antialias = Antialias.Subpixel;
            context.MiterLimit = outline.MiterLimit;
            SetColour(context, outline.BackgroundBrush);
            context.AppendPath(path);
            context.StrokePreserve();
        }

        private static void SetColour(Context context, StyleColor colour)
        {
            context.SetSourceRGBA(colour.R, colour.G,
                                  colour.B, colour.A);
        }

        private static void SetColour(Context context, StyleBrush brush)
        {
            SolidStyleBrush solid = brush as SolidStyleBrush;
            if (solid != null)
                SetColour(context, brush.Color);

            LinearGradientStyleBrush grad = brush as LinearGradientStyleBrush;
            if (grad != null)
                throw new NotImplementedException();
        }

        private static LineJoin ConvertJoin(StyleLineJoin join)
        {
            switch (join)
            {
                case StyleLineJoin.Bevel:
                    return LineJoin.Bevel;
                case StyleLineJoin.MiterClipped:
                case StyleLineJoin.Miter:
                    return LineJoin.Miter;
                default:
                    return LineJoin.Round;
            }
        }

        private static LineCap ConvertCap(StyleLineCap lineDashCap)
        {
            switch (lineDashCap)
            {
                case StyleLineCap.Square:
                    return LineCap.Square;
                case StyleLineCap.Round:
                    return LineCap.Round;
                case StyleLineCap.Flat:
                default:
                    return LineCap.Butt;
            }
        }

        private static double[] ConvertDash(float[] pattern)
        {
            if (pattern == null)
                return new double[] { 1 };
            double[] p = new double[pattern.Length];
            for (int i = 0; i < pattern.Length; i++)
                p[i] = pattern[i];
            return p;
        }
        private static Antialias ConvertAntiAlias(StyleRenderingMode mode)
        {
            switch (mode)
            {
                case StyleRenderingMode.AntiAlias:
                    return Antialias.Subpixel;
                case StyleRenderingMode.HighSpeed:
                    return Antialias.Gray;
                case StyleRenderingMode.None:
                    return Antialias.None;
                default:
                    return Antialias.Default;
            }
        }

        private Surface ConvertSymbol(Symbol2D symbol2D)
        {
            Surface img;
            if (_symbolCache.TryGetValue(symbol2D, out img))
                return img;

            byte[] data = new byte[symbol2D.SymbolData.Length];
            lock (symbol2D.SymbolData)
            {
                using (BinaryReader r = new BinaryReader(symbol2D.SymbolData))
                {
                    r.BaseStream.Seek(0, SeekOrigin.Begin);
                    r.Read(data, 0, data.Length);
                }
            }
            Surface bmp = new ImageSurface(ref data, Format.Argb32, (int)symbol2D.Size.Width,
                                           (int)symbol2D.Size.Height,
                                           data.Length / ((int)symbol2D.Size.Width * (int)symbol2D.Size.Height));
            _symbolCache.Add(symbol2D, bmp);

            return bmp;
        }

        private RenderObject GetGraphicsPath(IGeometry geometry, Matrix2D transform)
        {
            RenderObject path;
            if (!_pathCache.TryGetValue(geometry, out path))
            {
                path = CreateGraphicsPath(geometry, transform);
                _pathCache.Add(geometry, path);
            }
            return path;
        }

        private RenderObject CreateGraphicsPath(IGeometry geometry, Matrix2D transform)
        {
            RenderObject renderObject = new RenderObject();
            ConvertToPath(renderObject, geometry, transform);
            return renderObject;
        }

        private void ConvertToPath(RenderObject storage, IGeometry geometry, Matrix2D transform)
        {
            if (geometry is IGeometryCollection)
            {
                foreach (IGeometry geom in geometry as IGeometryCollection)
                    ConvertToPath(storage, geom, transform);
            }
            else if (geometry is IPolygon)
            {
                foreach (Path p in ConvertToPaths(new[] { geometry as IPolygon }, transform))
                    storage.Paths.Add(p);
            }
            else if (geometry is ILineString)
            {
                foreach (Path p in ConvertToPaths(new[] { geometry as ILineString }, transform))
                    storage.Paths.Add(p);
            }
            else if (geometry is IPoint)
                storage.Points.Add(TransformPoint((geometry as IPoint).Coordinate, transform));
        }

        private IEnumerable<Path> ConvertToPaths(IEnumerable<ILineString> lines, Matrix2D transform)
        {
            Context.NewPath();
            foreach (ILineString line in lines)
            {
                if (line.IsEmpty || line.PointCount <= 1)
                {
                    continue;
                }
                Context.NewSubPath();
                IEnumerable<PointD> cairoPts = TransformCoordinates(line.Coordinates, transform);
                Context.MoveTo(Enumerable.First(cairoPts));
                foreach (PointD pointD in Enumerable.Skip(cairoPts, 1))
                {
                    Context.LineTo(pointD);
                }
                if (line is ILinearRing)
                    Context.ClosePath();
            }

            yield return new Path
            {
                GraphicsPath = Context.CopyPathFlat(),
                PathType = PathType.Line
            };
        }

        private IEnumerable<Path> ConvertToPaths(IEnumerable<IPolygon> polygons, Matrix2D transform)
        {
            Context.NewPath();

            foreach (IPolygon polygon in polygons)
            {
                if (polygon.IsEmpty)
                {
                    continue;
                }

                Context.NewSubPath();
                IEnumerable<PointD> cairoPts = TransformCoordinates(polygon.ExteriorRing.Coordinates, transform);
                Context.MoveTo(Enumerable.First(cairoPts));
                foreach (PointD pt in Enumerable.Skip(cairoPts, 1))
                {
                    Context.MoveTo(pt);
                }

                Context.ClosePath();

                foreach (ILinearRing ring in polygon.InteriorRings)
                {
                    Context.NewSubPath();
                    IEnumerable<PointD> holePts = TransformCoordinates(ring.Coordinates, transform);
                    Context.MoveTo(Enumerable.First(holePts));
                    foreach (PointD pt in Enumerable.Skip(holePts, 1))
                        Context.LineTo(pt);
                    Context.ClosePath();
                }
            }

            yield return new Path
            {
                GraphicsPath = Context.CopyPathFlat(),
                PathType = PathType.Polygon
            };
        }


        private IEnumerable<PointD> TransformCoordinates(ICoordinateSequence sequence, Matrix2D transform)
        {
            foreach (ICoordinate c in sequence)
                yield return TransformPoint(c, transform);
        }

        private PointD TransformPoint(ICoordinate point, Matrix2D transform)
        {
            Point2D p = transform.TransformVector(point[Ordinates.X], point[Ordinates.Y]);
            return new PointD(p.X, p.Y);
        }
    }
}