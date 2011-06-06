using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
#if BUFFERED
using NetTopologySuite.Coordinates;
#else
using BufferedCoordinate = NetTopologySuite.Coordinates.Simple.Coordinate;
using BufferedCoordinateFactory = NetTopologySuite.Coordinates.Simple.CoordinateFactory;
using BufferedCoordinateSequence = NetTopologySuite.Coordinates.Simple.CoordinateSequence;
using BufferedCoordinateSequenceFactory = NetTopologySuite.Coordinates.Simple.CoordinateSequenceFactory;
#endif

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
