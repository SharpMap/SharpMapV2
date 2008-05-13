using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Expressions;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Tests.Rendering
{
    #region BasicGeometryRenderer2D

    [TestFixture]
    public class BasicGeometryRenderer2DTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);
        }

        #region Test stub types

        private struct RenderObject
        {
            public Double[][] RenderedPaths;
        }

        private class TestVectorRenderer : VectorRenderer2D<RenderObject>
        {
            public override IEnumerable<RenderObject> RenderPaths(IEnumerable<Path2D> paths,
                                                                  StylePen outline, StylePen highlightOutline,
                                                                  StylePen selectOutline, RenderState renderState)
            {
                foreach (Path2D path in paths)
                {
                    RenderObject ro = new RenderObject();

                    ro.RenderedPaths = new Double[path.Figures.Count][];

                    for (Int32 figureIndex = 0; figureIndex < path.Figures.Count; figureIndex++)
                    {
                        ro.RenderedPaths[figureIndex] = new Double[path.Figures[figureIndex].Points.Count*2];

                        for (Int32 pointIndex = 0; pointIndex < path.Figures[figureIndex].Points.Count; pointIndex++)
                        {
                            Point2D point = path.Figures[figureIndex].Points[pointIndex];
                            ro.RenderedPaths[figureIndex][pointIndex*2] = point.X;
                            ro.RenderedPaths[figureIndex][pointIndex*2 + 1] = point.Y;
                        }
                    }

                    yield return ro;
                }
            }

            public override IEnumerable<RenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                                  StylePen highlightLine, StylePen selectLine,
                                                                  StylePen outline, StylePen highlightOutline,
                                                                  StylePen selectOutline, RenderState renderState)
            {
                return renderPath(paths);
            }

            public override IEnumerable<RenderObject> RenderPaths(IEnumerable<Path2D> paths,
                                                                  StyleBrush fill, StyleBrush highlightFill,
                                                                  StyleBrush selectFill, StylePen outline,
                                                                  StylePen highlightOutline, StylePen selectOutline,
                                                                  RenderState renderState)
            {
                return renderPath(paths);
            }

            private static IEnumerable<RenderObject> renderPath(IEnumerable<Path2D> paths)
            {
                foreach (Path2D path in paths)
                {
                    RenderObject ro = new RenderObject();

                    ro.RenderedPaths = new Double[path.Figures.Count][];

                    for (Int32 figureIndex = 0; figureIndex < path.Figures.Count; figureIndex++)
                    {
                        ro.RenderedPaths[figureIndex] = new Double[path.Figures[figureIndex].Points.Count*2];

                        for (Int32 pointIndex = 0; pointIndex < path.Figures[figureIndex].Points.Count; pointIndex++)
                        {
                            Point2D point = path.Figures[figureIndex].Points[pointIndex];
                            ro.RenderedPaths[figureIndex][pointIndex*2] = point.X;
                            ro.RenderedPaths[figureIndex][pointIndex*2 + 1] = point.Y;
                        }
                    }

                    yield return ro;
                }
            }

            public override IEnumerable<RenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    RenderState renderState)
            {
                foreach (Point2D point in locations)
                {
                    RenderObject ro = new RenderObject();

                    ro.RenderedPaths = new Double[][] {new Double[] {point.X, point.Y}};

                    yield return ro;
                }
            }

            public override IEnumerable<RenderObject> RenderSymbols(IEnumerable<Point2D> locations,
                                                                    Symbol2D symbolData, ColorMatrix highlight,
                                                                    ColorMatrix select, RenderState renderState)
            {
                foreach (Point2D point in locations)
                {
                    RenderObject ro = new RenderObject();

                    ro.RenderedPaths = new Double[][] {new Double[] {point.X, point.Y}};

                    yield return ro;
                }
            }

            public override IEnumerable<RenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    Symbol2D highlightSymbolData,
                                                                    Symbol2D selectSymbolData, RenderState renderState)
            {
                foreach (Point2D point in locations)
                {
                    RenderObject ro = new RenderObject();

                    ro.RenderedPaths = new Double[][] {new Double[] {point.X, point.Y}};

                    yield return ro;
                }
            }
        }

        #endregion

        [Test]
        [Ignore("Test not yet implemented")]
        public void RenderFeatureTest()
        {
            IFeatureProvider provider = DataSourceHelper.CreateGeometryDatasource(_geoFactory);
            TestVectorRenderer vectorRenderer = new TestVectorRenderer();
            BasicGeometryRenderer2D<RenderObject> geometryRenderer =
                new BasicGeometryRenderer2D<RenderObject>(vectorRenderer);

            FeatureDataTable features = new FeatureDataTable(_geoFactory);
            IExtents extents = provider.GetExtents();
            features.Merge(provider.ExecuteIntersectionQuery(extents) as IEnumerable<IFeatureDataRecord>,
                           provider.GeometryFactory);

            foreach (FeatureDataRow feature in features)
            {
                IGeometry g = feature.Geometry;
                List<RenderObject> renderedObjects = new List<RenderObject>(geometryRenderer.RenderFeature(feature));

                for (Int32 i = 0; i < renderedObjects.Count; i++)
                {
                    RenderObject ro = renderedObjects[i];
                }
            }
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawMultiLineStringTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawLineStringTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawMultiPolygonTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawPolygonTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawPointTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawMultiPointTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof (NotSupportedException))]
        public void UnsupportedGeometryTest()
        {
        }
    }

    #endregion
}