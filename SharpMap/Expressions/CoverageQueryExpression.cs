using System;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Expressions
{
    public class CoverageQueryExpression : QueryExpression, IEquatable<CoverageQueryExpression>
    {
        public static CoverageQueryExpression Intersects(IExtents extents)
        {
            return new CoverageQueryExpression(extents, SpatialOperation.Intersects);
        }

        public static CoverageQueryExpression Intersects(IGeometry geometry)
        {
            return new CoverageQueryExpression(geometry, SpatialOperation.Intersects);
        }

        public CoverageQueryExpression(IExtents extents, SpatialOperation op)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new ExtentsExpression(extents),
                                                                         op,
                                                                         new ThisExpression()))
        {
            checkOp(op);
        }

        public CoverageQueryExpression(IGeometry geometry, SpatialOperation op)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                         op,
                                                                         new ThisExpression()))
        {
            checkOp(op);
        }

        public CoverageQueryExpression(IGeometry geometry, SpatialOperation op, ILayer layer)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                         op,
                                                                         new LayerExpression(layer)))
        {
            checkOp(op);
        }

        public CoverageQueryExpression(IGeometry geometry, SpatialOperation op, ICoverageProvider provider)
            : base(new AllBandsExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                         op,
                                                                         new ProviderExpression(provider)))
        {
            checkOp(op);
        }

        public CoverageQueryExpression(SpatialBinaryExpression spatialFilter)
            : base(new AllBandsExpression(), spatialFilter) { }

        protected internal CoverageQueryExpression(SelectExpression projection,
                                                 LogicExpression predicate)
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

                BinaryLogicExpression binaryExpression = Predicate as BinaryLogicExpression;

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

        public Boolean Equals(CoverageQueryExpression other)
        {
            return !ReferenceEquals(other, null) && base.Equals(other);
        }

        public override Boolean Equals(Object obj)
        {
            return ReferenceEquals(this, obj) || Equals(obj as CoverageQueryExpression);
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