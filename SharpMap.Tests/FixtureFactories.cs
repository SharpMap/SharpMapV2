using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;

namespace SharpMap.Tests
{
    public class FixtureFactories
    {
        private IGeometryFactory _geoFactory;

        public FixtureFactories()
        {
            BufferedCoordinateSequenceFactory sequenceFactory = new BufferedCoordinateSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate>(sequenceFactory);
        }

        public IGeometryFactory GeoFactory
        {
            get { return _geoFactory; }
        }
    }
}
