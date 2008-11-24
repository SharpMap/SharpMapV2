using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NPack;
using NPack.Interfaces;
using NPack.Matrix;
using ProjNet.CoordinateSystems.Transformations;
using Rhino.Mocks;
using SharpMap.Data.Providers;
using Xunit;

namespace SharpMap.Tests.Data.Providers
{
    public class FeatureProviderBaseTests
    {
        [Fact]
        public void CoordinateTransformationTransformsQueryHavingDifferentSpatialReferenceAsTransformationTarget()
        {
            MockRepository repository = new MockRepository();
            FeatureProviderBase provider = repository.CreateMock<FeatureProviderBase>();
            BufferedCoordinateFactory coordFactory = new BufferedCoordinateFactory();
            BufferedCoordinateSequenceFactory coordSeqFactory = new BufferedCoordinateSequenceFactory(coordFactory);
            IGeometryFactory<BufferedCoordinate> geoFactory = new GeometryFactory<BufferedCoordinate>(coordSeqFactory);
            IMatrixFactory<DoubleComponent> matrixFactory = new LinearFactory<DoubleComponent>();
            ICoordinateTransformationFactory transformFactory =
                new CoordinateTransformationFactory<BufferedCoordinate>(coordFactory, geoFactory, matrixFactory);
            SetupResult.For(provider.CoordinateTransformation).Return(transformFactory);
        }

        [Fact]
        public void CoordinateTransformationDoesNotTransformsQueryHavingSameSpatialReferenceAsTransformationTarget()
        {
            
        }
    }
}
