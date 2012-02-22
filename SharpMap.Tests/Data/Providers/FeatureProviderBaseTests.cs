using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using NPack;
using NPack.Interfaces;
using NPack.Matrix;
using NetTopologySuite.CoordinateSystems.Transformations;
using Rhino.Mocks;
using SharpMap.Data.Providers;
using Xunit;

#if BUFFERED
using NetTopologySuite.Coordinates;
#else
using BufferedCoordinate = NetTopologySuite.Coordinates.Simple.Coordinate;
using BufferedCoordinateFactory = NetTopologySuite.Coordinates.Simple.CoordinateFactory;
using BufferedCoordinateSequence = NetTopologySuite.Coordinates.Simple.CoordinateSequence;
using BufferedCoordinateSequenceFactory = NetTopologySuite.Coordinates.Simple.CoordinateSequenceFactory;
using NetTopologySuite.Geometries;
#endif

namespace SharpMap.Tests.Data.Providers
{
    public class FeatureProviderBaseTests
    {
        [Fact]
        public void CoordinateTransformationTransformsQueryHavingDifferentSpatialReferenceAsTransformationTarget()
        {
            var repository = new MockRepository();
            var provider = repository.StrictMock<FeatureProviderBase>();
            var coordFactory = new BufferedCoordinateFactory();
            var coordSeqFactory = new BufferedCoordinateSequenceFactory(coordFactory);
            var geoFactory = new GeometryFactory<BufferedCoordinate>(coordSeqFactory);
            var matrixFactory = new LinearFactory<DoubleComponent>();
            ICoordinateTransformationFactory transformFactory =
                new CoordinateTransformationFactory<BufferedCoordinate>(coordFactory, geoFactory, matrixFactory);
            //SetupResult.For(provider.CoordinateTransformation).Return(transformFactory);
        }

        [Fact]
        public void CoordinateTransformationDoesNotTransformsQueryHavingSameSpatialReferenceAsTransformationTarget()
        {
            
        }
    }
}
