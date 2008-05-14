using System;
using GeoAPI.Geometries;

namespace SharpMap.Expressions
{
    public class SpatialExpression : Expression, IEquatable<SpatialExpression>
    {
        private readonly IGeometry _geometry;
        
        public SpatialExpression(IGeometry geometry)
        {
            _geometry = geometry;
        }

        public IGeometry Geometry
        {
            get { return _geometry; }
        }

        public override Boolean Matches(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Boolean Equals(Expression other)
        {
            throw new NotImplementedException();
        }

        public Boolean Equals(SpatialExpression other)
        {
            throw new NotImplementedException();
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (_geometry != null
                            ? _geometry.GetHashCode()
                            : 0x1fd3b) ^ 29;
            }
        }

        public override Expression Clone()
        {
            throw new NotImplementedException();
        }
    }
}
