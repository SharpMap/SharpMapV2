using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoAPI.Coordinates;
#if DOTNET35
using sl = System.Linq;
#else
using sl = GeoAPI.DataStructures;
#endif
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Gdi
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
        public GraphicsPath GraphicsPath { get; set; }
    }


    public class GdiGeometryRasterizer : GdiRasterizer, IGeometryRasterizer<Bitmap, Graphics>
    {
        private const FillMode PathFillMode = FillMode.Alternate;
        private readonly Dictionary<IGeometry, RenderObject> _pathCache = new Dictionary<IGeometry, RenderObject>();
        private readonly Dictionary<Symbol2D, Image> _symbolCache = new Dictionary<Symbol2D, Image>();

        public GdiGeometryRasterizer(Bitmap surface, Graphics context)
            : base(surface, context)
        {
        }

        #region IGeometryRasterizer<Bitmap,Graphics> Members

        public void Rasterize(IFeatureDataRecord feature, GeometryStyle style, Matrix2D transform)
        {
            IGeometry geometry = feature.Geometry;
            Context.SmoothingMode = ConvertSmoothingMode(style.RenderingMode);
            RenderObject path = GetGraphicsPath(geometry, transform);
            if (path.HasPaths)
            {
                if (style.Fill != null)
                {
                    foreach (Path p in path.Paths)
                        if (p.PathType == PathType.Polygon)
                            Context.FillPath(ViewConverter.Convert(style.Fill), p.GraphicsPath);
                }
                if (style.Line != null)
                {
                    foreach (Path p in path.Paths)
                        Context.DrawPath(ViewConverter.Convert(style.Line), p.GraphicsPath);
                }
            }
            if (path.HasPoints && style.Symbol != null)
            {
                foreach (PointF p in path.Points)
                    Context.DrawImage(ConvertSymbol(style.Symbol), (float) (p.X - style.Symbol.Size.Width/2),
                                      (float) (p.Y - style.Symbol.Size.Height/2));
            }
        }

        #endregion

        private static SmoothingMode ConvertSmoothingMode(StyleRenderingMode styleRenderingMode)
        {
            switch (styleRenderingMode)
            {
                case StyleRenderingMode.AntiAlias:
                    return SmoothingMode.AntiAlias;
                case StyleRenderingMode.HighQuality:
                    return SmoothingMode.HighQuality;
                case StyleRenderingMode.HighSpeed:
                    return SmoothingMode.HighSpeed;
                case StyleRenderingMode.Default:
                default:
                    return SmoothingMode.Default;
            }
        }

        private Image ConvertSymbol(Symbol2D symbol2D)
        {
            Image img;
            if (_symbolCache.TryGetValue(symbol2D, out img))
                return img;

            Bitmap bmp = new Bitmap(symbol2D.SymbolData);
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
                foreach (Path p in ConvertToPaths(new[] {geometry as IPolygon}, transform))
                    storage.Paths.Add(p);
            }
            else if (geometry is ILineString)
            {
                foreach (Path p in ConvertToPaths(new[] {geometry as ILineString}, transform))
                    storage.Paths.Add(p);
            }
            else if (geometry is IPoint)
                storage.Points.Add(TransformPoint((geometry as IPoint).Coordinate, transform));
        }

        private IEnumerable<Path> ConvertToPaths(IEnumerable<ILineString> lines, Matrix2D transform)
        {
            GraphicsPath gp = new GraphicsPath(PathFillMode);

            foreach (ILineString line in lines)
            {
                if (line.IsEmpty || line.PointCount <= 1)
                {
                    continue;
                }
                gp.StartFigure();
                gp.AddLines(sl.Enumerable.ToArray(TransformCoordinates(line.Coordinates, transform)));
                if (line is ILinearRing)
                    gp.CloseFigure();
            }

            yield return new Path
                             {
                                 GraphicsPath = gp,
                                 PathType = PathType.Line
                             };
        }

        private IEnumerable<Path> ConvertToPaths(IEnumerable<IPolygon> polygons, Matrix2D transform)
        {
            GraphicsPath gp = new GraphicsPath(PathFillMode);

            foreach (IPolygon polygon in polygons)
            {
                if (polygon.IsEmpty)
                {
                    continue;
                }

                gp.StartFigure();
                gp.AddLines(sl.Enumerable.ToArray(TransformCoordinates(polygon.ExteriorRing.Coordinates, transform)));
                gp.CloseFigure();

                foreach (ILinearRing ring in polygon.InteriorRings)
                {
                    gp.StartFigure();
                    gp.AddLines(sl.Enumerable.ToArray(TransformCoordinates(ring.Coordinates, transform)));
                    gp.CloseFigure();
                }
            }

            yield return new Path
                             {
                                 GraphicsPath = gp,
                                 PathType = PathType.Polygon
                             };
        }

        private IEnumerable<PointF> TransformCoordinates(ICoordinateSequence sequence, Matrix2D transform)
        {
            foreach (ICoordinate c in sequence)
                yield return TransformPoint(c, transform);
        }

        private PointF TransformPoint(ICoordinate point, Matrix2D transform)
        {
            Point2D p = transform.TransformVector(point[Ordinates.X], point[Ordinates.Y]);
            return new PointF((float) p.X, (float) p.Y);
        }
    }
}