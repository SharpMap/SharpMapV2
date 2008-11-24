using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;

using SharpMap.Data;
using SharpMap.Expressions;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using Xunit;

namespace SharpMap.Tests.Rendering
{
    #region BasicGeometryRenderer2D
    public class BasicGeometryRenderer2DTests : IUseFixture<FixtureFactories>
    {
        private FixtureFactories _factories;

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

        [Fact(Skip = "Incomplete")]
        public void RenderFeatureTest()
        {
            IFeatureProvider provider = DataSourceHelper.CreateGeometryDatasource(_factories.GeoFactory);
            TestVectorRenderer vectorRenderer = new TestVectorRenderer();
            BasicGeometryRenderer2D<RenderObject> geometryRenderer =
                new BasicGeometryRenderer2D<RenderObject>(vectorRenderer);

            FeatureDataTable features = new FeatureDataTable(_factories.GeoFactory);
            IExtents extents = provider.GetExtents();
            FeatureQueryExpression query = new FeatureQueryExpression(extents.ToGeometry(),
                                                                      SpatialOperation.Intersects,
                                                                      provider);
            features.Merge(provider.ExecuteFeatureQuery(query) as IEnumerable<IFeatureDataRecord>,
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

        [Fact(Skip = "Incomplete")]
        public void DrawMultiLineStringTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void DrawLineStringTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void DrawMultiPolygonTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void DrawPolygonTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void DrawPointTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void DrawMultiPointTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void UnsupportedGeometryTest()
        {
        }

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }
    }

    #endregion
}