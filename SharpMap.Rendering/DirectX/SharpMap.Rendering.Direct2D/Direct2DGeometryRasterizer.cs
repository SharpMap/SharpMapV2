using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using GeoAPI.Coordinates;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SlimDX.Direct2D;
using Bitmap = SlimDX.Direct2D.Bitmap;

namespace SharpMap.Rendering.Direct2D
{
    internal class RenderObject
    {
        private Collection<Path> _paths;
        private Collection<PointF> _points;

        public ICollection<Path> Paths
        {
            get
            {
                if (_paths == null)
                    _paths = new Collection<Path>();

                return _paths;
            }
        }

        public ICollection<PointF> Points
        {
            get
            {
                if (_points == null)
                    _points = new Collection<PointF>();
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
        public PathGeometry GraphicsPath { get; set; }
    }


    public class Direct2DGeometryRasterizer : Direct2DRasterizer, IGeometryRasterizer<RenderTarget, RenderTarget>
    {
        private readonly Dictionary<IGeometry, RenderObject> _pathCache = new Dictionary<IGeometry, RenderObject>();
        private readonly Dictionary<Symbol2D, Bitmap> _symbolCache = new Dictionary<Symbol2D, Bitmap>();

        public Direct2DGeometryRasterizer(RenderTarget surface, RenderTarget context)
            : base(surface, context)
        {
            Factory = new Factory();
        }

        private Factory Factory { get; set; }

        #region IGeometryRasterizer<RenderTarget,RenderTarget> Members

        public void Rasterize(IFeatureDataRecord record, GeometryStyle style, Matrix2D transform)
        {
            IGeometry geometry = record.Geometry;
            Context.AntialiasMode = ConvertAntiAliasMode(style.RenderingMode);

            RenderObject path = GetPathGeometry(geometry, transform);

            if (path.HasPaths)
            {
                if (style.Fill != null)
                {
                    foreach (Path p in path.Paths)
                        if (p.PathType == PathType.Polygon)
                            Context.FillGeometry(p.GraphicsPath, ViewConverter.Convert(Surface, style.Fill));
                }
                if (style.Line != null)
                {
                    foreach (Path p in path.Paths)
                        Context.DrawGeometry(p.GraphicsPath, ViewConverter.Convert(Surface, style.Line.BackgroundBrush),
                                             (float)style.Line.Width, ViewConverter.Convert(Surface, style.Line));
                }
            }
            //if (path.HasPoints && style.Symbol != null)
            //{
            //    foreach (PointF point in path.Points)
            //    {
            //        Context.DrawBitmap(ConvertSymbol(style.Symbol), GetRectangle(point, style.Symbol));
            //    }
            //}
        }

        #endregion

        private RectangleF GetRectangle(PointF pointF, Symbol2D symbol)
        {
            double width = symbol.Size.Height * symbol.ScaleY;
            double height = symbol.Size.Width * symbol.ScaleX;

            return new RectangleF(new PointF(pointF.X - (float)(width / 2), pointF.Y - (float)(height / 2)),
                                  new SizeF((float)width, (float)height));
        }

        private Bitmap ConvertSymbol(Symbol2D symbol)
        {
            Bitmap bmp;
            if (_symbolCache.TryGetValue(symbol, out bmp))
                return bmp;

            throw new NotImplementedException();
        }

        private RenderObject GetPathGeometry(IGeometry geometry, Matrix2D transform)
        {
            RenderObject path;
            if (!_pathCache.TryGetValue(geometry, out path))
            {
                path = CreatePathGeometry(geometry, transform);
                _pathCache.Add(geometry, path);
            }
            return path;
        }

        private RenderObject CreatePathGeometry(IGeometry geometry, Matrix2D transform)
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

        private PointF TransformPoint(ICoordinate coordinate, Matrix2D transform)
        {
            Point2D p = transform.TransformVector(coordinate[Ordinates.X], coordinate[Ordinates.Y]);
            return new PointF((float)p.X, (float)p.Y);
        }

        private IEnumerable<Path> ConvertToPaths(IEnumerable<ILineString> lineStrings, Matrix2D transform)
        {
            PathGeometry pathGeometry = new PathGeometry(Factory);
            foreach (ILineString lineString in lineStrings)
            {
                if (lineString.IsEmpty)
                    continue;

                GeometrySink snk = pathGeometry.Open();

                PointF[] pts = Enumerable.ToArray(TransformCoordinates(lineString.Coordinates, transform));
                snk.BeginFigure(pts[0], FigureBegin.Hollow);

                snk.AddLines(pts);

                snk.EndFigure(lineString.IsClosed ? FigureEnd.Closed : FigureEnd.Open);

                snk.Close();
            }

            yield return new Path
                             {
                                 GraphicsPath = pathGeometry,
                                 PathType = PathType.Line
                             };
        }

        private IEnumerable<Path> ConvertToPaths(IEnumerable<IPolygon> polygons, Matrix2D transform)
        {
            PathGeometry pathGeometry = new PathGeometry(Factory);
            foreach (IPolygon polygon in polygons)
            {
                if (polygon.IsEmpty)
                    continue;

                GeometrySink snk = pathGeometry.Open();
                PointF[] pts = Enumerable.ToArray(TransformCoordinates(polygon.ExteriorRing.Coordinates, transform));
                snk.BeginFigure(pts[0], FigureBegin.Filled);
                snk.AddLines(pts);
                snk.EndFigure(FigureEnd.Closed);

                foreach (ILinearRing linearRing in polygon.InteriorRings)
                {
                    pts = Enumerable.ToArray(TransformCoordinates(linearRing.Coordinates, transform));

                    snk.BeginFigure(pts[0], FigureBegin.Hollow);
                    snk.AddLines(pts);
                    snk.EndFigure(FigureEnd.Closed);

                }
                snk.Close();
            }

            yield return new Path
                             {
                                 GraphicsPath = pathGeometry,
                                 PathType = PathType.Polygon
                             };
        }

        private IEnumerable<PointF> TransformCoordinates(ICoordinateSequence sequence, Matrix2D transform)
        {
            foreach (ICoordinate c in sequence)
                yield return TransformPoint(c, transform);
        }

        private static AntialiasMode ConvertAntiAliasMode(StyleRenderingMode styleRenderingMode)
        {
            switch (styleRenderingMode)
            {
                case StyleRenderingMode.AntiAlias:
                case StyleRenderingMode.HighQuality:
                    return AntialiasMode.PerPrimitive;
                default:
                    return AntialiasMode.Aliased;
            }
        }
    }
}