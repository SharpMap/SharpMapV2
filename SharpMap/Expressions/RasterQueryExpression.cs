using System;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Expressions
{
    [Serializable]
    public class RasterQueryExpression : QueryExpression, IEquatable<RasterQueryExpression>
    {
        public static RasterQueryExpression Intersects(IExtents extents)
        {
            return new RasterQueryExpression(extents, SpatialOperation.Intersects);
        }

        public static RasterQueryExpression Intersects(IGeometry geometry)
        {
            return new RasterQueryExpression(geometry, SpatialOperation.Intersects);
        }

        public RasterQueryExpression(IExtents extents, SpatialOperation op)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new ExtentsExpression(extents),
                                                                         op,
                                                                         new ThisExpression()))
        {
            checkOp(op);
        }

        public RasterQueryExpression(IGeometry geometry, SpatialOperation op)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                         op,
                                                                         new ThisExpression()))
        {
            checkOp(op);
        }

        public RasterQueryExpression(IGeometry geometry, SpatialOperation op, ILayer layer)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                         op,
                                                                         new LayerExpression(layer)))
        {
            checkOp(op);
        }

        public RasterQueryExpression(IGeometry geometry, SpatialOperation op, IRasterProvider provider)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                         op,
                                                                         new ProviderExpression(provider)))
        {
            checkOp(op);
        }

        public RasterQueryExpression(SpatialBinaryExpression spatialFilter)
            : base(new AllBandsExpression(), spatialFilter) { }

        protected internal RasterQueryExpression(ProjectionExpression projection,
                                                 PredicateExpression predicate)
            : base(projection, predicate) { }

        public SpatialBinaryExpression SpatialPredicate
        {
            get
            {
                SpatialBinaryExpression spatialBinaryExpression =
                    Predicate as SpatialBinaryExpression;

                if (spatialBinaryExpression != null)
                {
                    return spatialBinaryExpression;
                }

                BinaryExpression binaryExpression = Predicate as BinaryExpression;

                if (binaryExpression == null)
                {
                    return null;
                }

                spatialBinaryExpression = binaryExpression.Left as SpatialBinaryExpression;

                if (spatialBinaryExpression != null)
                {
                    return spatialBinaryExpression;
                }

                spatialBinaryExpression = binaryExpression.Right as SpatialBinaryExpression;

                return spatialBinaryExpression;
            }
        }

        public Boolean Equals(RasterQueryExpression other)
        {
            return !ReferenceEquals(other, null) && base.Equals(other);
        }

        public override Boolean Equals(Object obj)
        {
            return ReferenceEquals(this, obj) || Equals(obj as RasterQueryExpression);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return base.GetHashCode() ^ 131;
            }
        }
    
        private void checkOp(SpatialOperation op)
        {
            switch (op)
            {
                case SpatialOperation.Contains:
                case SpatialOperation.Disjoint:
                case SpatialOperation.Intersects:
                    return;
                default:
                    throw new NotSupportedException("Operation not supported for raster query.");
            }
        }
    }
}