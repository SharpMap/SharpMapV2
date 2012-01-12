using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Wpf
{

    using WpfPoint = System.Windows.Point;
    using WpfSize = System.Windows.Size;
    using WpfRectangle = System.Windows.Rect;
    using WpfPen = System.Windows.Media.Pen;
    using WpfBrush = System.Windows.Media.Brush;
    using WpfBrushes = System.Windows.Media.Brushes;
    using WpfFont = System.Windows.Media.Typeface;
    using WpfFontFamily = System.Windows.Media.FontFamily;
    using WpfFontStyle = System.Windows.FontStyle;
    using WpfColor = System.Windows.Media.Color;
    using WpfStreamPath = System.Windows.Media.StreamGeometry;
    using WpfPath = System.Windows.Media.PathGeometry;
    using WpfPathFigure = System.Windows.Media.PathFigure;
    using WpfMatrix = System.Windows.Media.Matrix;
    using WpfDrawing = System.Windows.Media.Drawing;
    using WpfLineSegment = System.Windows.Media.LineSegment;
    using StyleColorMatrix = SharpMap.Rendering.ColorMatrix;
    using WpfImageSource = System.Windows.Media.ImageSource;
    using WpfGeometryDrawing = System.Windows.Media.GeometryDrawing;

    public class WpfGeometryRasterizer : WpfRasterizer, IGeometryRasterizer<DrawingVisual, DrawingContext>
    {
        Dictionary<Path2D, WpfStreamPath> _pathCache = new Dictionary<Path2D, WpfStreamPath>();
        private static readonly ReaderWriterLockSlim _pathCacheLock = new ReaderWriterLockSlim();


        private readonly Dictionary<IGeometry, RenderObject> _graphicsPathCache = new Dictionary<IGeometry, RenderObject>();
        private readonly Dictionary<Symbol2D, ImageSource> _symbolCache = new Dictionary<Symbol2D, ImageSource>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfGeometryRasterizer"/> class.
        /// </summary>
        /// <param name="surface">The <see cref="DrawingVisual"/> that is being drawn on.</param>
        /// <param name="context">The <see cref="DrawingContext"/>.</param>
        public WpfGeometryRasterizer(DrawingVisual surface, DrawingContext context) 
            : base(surface, context)
        {
        }

        public void Rasterize(IFeatureDataRecord feature, GeometryStyle style, Matrix2D transform)
        {
            IGeometry geometry = feature.Geometry;

            var filled = style.Fill != null;
            var stroked = style.Line != null;
            RenderObject path = CreateGraphicsPath(geometry, transform, filled, stroked);
            if (path.HasPaths)
            {

                if (filled && stroked)
                {
                    var fill = ViewConverter.Convert(style.Fill);
                    var pen = ViewConverter.Convert(style.Line);
                    foreach (var p in path.Paths.Where(p => p.PathType == PathType.Polygon))
                    {
                        Context.DrawGeometry(fill, pen, p.GraphicsPath);
                    }
                }

                if (filled && style.Line == null)
                {
                    foreach (var p in path.Paths)
                        if (p.PathType == PathType.Polygon)
                        {
                            var fill = ViewConverter.Convert(style.Fill);
                            Context.DrawGeometry(fill, null, p.GraphicsPath);
                        }
                }

                if (stroked && style.Fill == null)
                {
                    foreach (var p in path.Paths)
                    {
                        Context.DrawGeometry(null, ViewConverter.Convert(style.Line), p.GraphicsPath);
                    }
                }
            }
            if (path.HasPoints && style.Symbol != null)
            {
                foreach (var p in path.Points)
                {
                    this.RasterizePoint(style, p);
                }
            }
        }

        private RenderObject CreateGraphicsPath(IGeometry geometry, Matrix2D transform, bool filled, bool stroked)
        {
            var renderObject = new RenderObject();
            
            switch (geometry.GeometryType)
            {
                case OgcGeometryType.Point:
                    renderObject.Points.AddRange(CreateWpfGraphicsPoints(ConvertPoints(new[] {(geometry as IPoint)}), transform));
                    break;
                case OgcGeometryType.MultiPoint:
                    break;
                case OgcGeometryType.LineString:
                    renderObject.Paths.AddRange(CreateWpfGraphicsPaths(ConvertToPaths(new[] { geometry as ILineString }), PathType.Line, transform, filled, stroked));
                    break;
                case OgcGeometryType.MultiLineString:
                    renderObject.Paths.AddRange(CreateWpfGraphicsPaths(ConvertToPaths(geometry as IMultiLineString), PathType.Line, transform, filled, stroked));
                    break;
                case OgcGeometryType.Polygon:
                    renderObject.Paths.AddRange(CreateWpfGraphicsPaths(ConvertToPaths(new[] { geometry as IPolygon }), PathType.Polygon, transform, filled, stroked));
                    break;
                case OgcGeometryType.MultiPolygon:
                    renderObject.Paths.AddRange(CreateWpfGraphicsPaths(ConvertToPaths(geometry as IMultiPolygon), PathType.Polygon, transform, filled, stroked));
                    break;
                case OgcGeometryType.Surface:
                case OgcGeometryType.MultiSurface:
                case OgcGeometryType.Curve:
                case OgcGeometryType.MultiCurve:
                case OgcGeometryType.Geometry:
                case OgcGeometryType.GeometryCollection:
                case OgcGeometryType.Unknown:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return renderObject;
        }

        private void RasterizePoint(GeometryStyle style, Point p)
        {
            var translateTransform = new TranslateTransform();
            var rotateTransform = new RotateTransform();

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(rotateTransform);


            if (Math.Abs(style.Symbol.Rotation - 0) > 0.0001)
            {
                //rotate
                rotateTransform.Angle = style.Symbol.Rotation / Math.PI * 360.0;
                rotateTransform.CenterX += p.X;
                rotateTransform.CenterY += p.Y;
            }

            // Center the symbol
            translateTransform.X -= (style.Symbol.Size.Width / 2.0);
            translateTransform.Y -= (style.Symbol.Size.Height / 2.0);

            // Apply the symbol offsets.
            translateTransform.X += style.Symbol.Offset.X;
            translateTransform.Y += style.Symbol.Offset.Y;

            // Render the image at the point.

            this.Context.PushTransform(transformGroup);
            this.Context.DrawImage(ViewConverter.Convert(style.Symbol),
                                   new Rect(new Point(p.X, p.Y), ViewConverter.Convert(style.Symbol.Size)));

            // Reset the transform.
            this.Context.Pop();
        }

        private IEnumerable<Path> CreateWpfGraphicsPaths(IEnumerable<Path2D> paths, PathType pathType, Matrix2D transform, bool filled, bool stroked)
        {
            foreach (Path2D path in paths)
            {
                WpfStreamPath pathCache;

                if (!TryGetCache(path, out pathCache))
                {
                    pathCache = new WpfStreamPath();
                    using (var ctx = pathCache.Open())
                    {
                        foreach (Figure2D figure in path.Figures)
                        {
                            ctx.BeginFigure(TransformPoint(figure.Points[0], transform), filled, figure.IsClosed);
                            for (int i = 1; i < figure.Points.Count; ++i)
                            {
                                ctx.LineTo(TransformPoint(figure.Points[i], transform), stroked, false);
                            }
                        }
                        pathCache.Freeze();
                    }
                    AddCache(path, pathCache);

                }
                var graphicsPath = new Path{GraphicsPath = pathCache, PathType = pathType};
                

                yield return graphicsPath;
            }
        }

        private static IEnumerable<WpfPoint> CreateWpfGraphicsPoints(IEnumerable<Point2D> points, Matrix2D transform)
        {
            return points.Select(point => TransformPoint(point, transform));
        }


        private Boolean TryGetCache(Path2D path, out WpfStreamPath pathCache)
        {
            Boolean retVal;
            _pathCacheLock.EnterReadLock();
            try
            {
                retVal = _pathCache.TryGetValue(path, out pathCache);
            }

            finally
            {
                _pathCacheLock.ExitReadLock();
            }
            return retVal;

        }

        private void AddCache(Path2D path, WpfStreamPath pathCache)
        {
            _pathCacheLock.EnterWriteLock();
            try
            {
                _pathCache.Add(path, pathCache);
            }
            finally
            {
                _pathCacheLock.ExitWriteLock();
            }
        }

        private static IEnumerable<Path2D> ConvertToPaths(IEnumerable<ILineString> lines)
        {
            Path2D gp = new Path2D();

            foreach (ILineString line in lines)
            {
                if (line.IsEmpty || line.Coordinates.Count <= 1)
                {
                    continue;
                }

                gp.NewFigure(ConvertPoints(line.Coordinates), false);
            }

            yield return gp;
        }

        private static IEnumerable<Path2D> ConvertToPaths(IEnumerable<IPolygon> polygons)
        {
            Path2D gp = new Path2D();

            foreach (IPolygon polygon in polygons)
            {
                if (polygon.IsEmpty)
                {
                    continue;
                }

                // Add the exterior polygon
                gp.NewFigure(ConvertPoints(polygon.ExteriorRing.Coordinates), true);

                // Add the interior polygons (holes)
                foreach (var ring in polygon.InteriorRings)
                {
                    gp.NewFigure(ConvertPoints(ring.Coordinates), true);
                }
            }

            yield return gp;
        }


        private static IEnumerable<Point2D> ConvertPoints(IEnumerable<IPoint> points)
        {
            return points.Select(geoPoint => new Point2D(geoPoint[Ordinates.X], geoPoint[Ordinates.Y]));
        }

        private static IEnumerable<Point2D> ConvertPoints(ICoordinateSequence sequence)
        {
            return from ICoordinate c in sequence select new Point2D(c[Ordinates.X], c[Ordinates.Y]);
        }

        private static WpfPoint TransformPoint(Point2D point, Matrix2D transform)
        {
            var p = transform.TransformVector(point.X, point.Y);
            return new WpfPoint(p.X, p.Y);
        }
    }

    internal class RenderObject
    {
        private List<Path> _paths;
        private List<WpfPoint> _points;

        public List<Path> Paths
        {
            get
            {
                if (_paths == null)
                    _paths = new List<Path>();

                return _paths;
            }
        }

        public List<WpfPoint> Points
        {
            get
            {
                if (_points == null)
                    _points = new List<WpfPoint>();
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
        public WpfStreamPath GraphicsPath { get; set; }
    }
}
