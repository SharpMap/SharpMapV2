using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Expressions
{
    public class BrutileRasterQueryExpression : RasterQueryExpression
    {
        private LiteralExpression<double> _resolution;

        public BrutileRasterQueryExpression(IExtents extents, SpatialOperation op, LiteralExpression<double> resolution) : base(extents, op)
        {
            _resolution = resolution;
        }

        public BrutileRasterQueryExpression(IGeometry geometry, SpatialOperation op, LiteralExpression<double> resolution)
            : base(geometry, op)
        {
            _resolution = resolution;
        }

        public BrutileRasterQueryExpression(IGeometry geometry, SpatialOperation op, ILayer layer, LiteralExpression<double> resolution)
            : base(geometry, op, layer)
        {
            _resolution = resolution;
        }

        public BrutileRasterQueryExpression(IGeometry geometry, SpatialOperation op, IRasterProvider provider, LiteralExpression<double> resolution)
            : base(geometry, op, provider)
        {
            _resolution = resolution;
        }

        public BrutileRasterQueryExpression(SpatialBinaryExpression spatialFilter, LiteralExpression<double> resolution)
            : base(spatialFilter)
        {
            _resolution = resolution;
        }

        protected internal BrutileRasterQueryExpression(ProjectionExpression projection, PredicateExpression predicate, LiteralExpression<double> resolution)
            : base(projection, predicate)
        {
            _resolution = resolution;
        }

        public LiteralExpression<double> Resolution
        {
            get { return _resolution; }
        }
    }
}