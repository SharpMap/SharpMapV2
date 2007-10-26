using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Geometries;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Tests;
using Point=SharpMap.Geometries.Point;

namespace SharpMap.Rendering.Gdi.Tests
{
    [TestFixture]
    public class BasicGeometryRenderer2DWithGdiVectorRenderer
    {
        private static readonly Single _e = 0.0001f;

        [Test]
        public void CreatingBasicGeometryRenderer2DWithGdiVectorRendererSucceeds()
        {
            using (GdiVectorRenderer vectorRenderer = new GdiVectorRenderer())
            {
                BasicGeometryRenderer2D<GdiRenderObject> geometryRenderer
                    = new BasicGeometryRenderer2D<GdiRenderObject>(vectorRenderer);
            }
        }

        [Test]
        public void RenderingPolygonsWithGdiVectorRendererProducesCorrectGdiPaths()
        {
            using (GdiVectorRenderer vectorRenderer = new GdiVectorRenderer())
            {
                BasicGeometryRenderer2D<GdiRenderObject> geometryRenderer
                    = new BasicGeometryRenderer2D<GdiRenderObject>(vectorRenderer);

                FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();

                foreach (IFeatureDataRecord record in provider.ExecuteIntersectionQuery(provider.GetExtents()))
                {
                    IEnumerable<GdiRenderObject> renderedObjects = geometryRenderer.RenderFeature(record);

                    foreach (GdiRenderObject ro in renderedObjects)
                    {
                        Geometry g = record.Geometry;

                        if (g is Polygon)
                        {
                            Polygon p = g as Polygon;
                            using (GraphicsPathIterator iter = new GraphicsPathIterator(ro.GdiPath))
                            {
                                Int32 start, end;
                                Boolean isClosed;
                                iter.NextSubpath(out start, out end, out isClosed);

                                Assert.IsTrue(isClosed);
                                Assert.AreEqual(p.ExteriorRing.Vertices.Count, end - start + 1);

                                for (Int32 vertexIndex = 0; vertexIndex < p.ExteriorRing.Vertices.Count; vertexIndex++)
                                {
                                    Point v = p.ExteriorRing.Vertices[vertexIndex];
                                    PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                    Assert.AreEqual(v.X, gdiPoint.X);
                                    Assert.AreEqual(v.Y, gdiPoint.Y);
                                }

                                for (Int32 interiorIndex = 0; interiorIndex < p.InteriorRings.Count; interiorIndex++)
                                {
                                    iter.NextSubpath(out start, out end, out isClosed);
                                    Assert.IsTrue(isClosed);
                                    Assert.AreEqual(p.InteriorRings[interiorIndex].Vertices.Count, end - start + 1);

                                    for (Int32 vertexIndex = 0;
                                         vertexIndex < p.InteriorRings[interiorIndex].Vertices.Count;
                                         vertexIndex++)
                                    {
                                        Point v = p.InteriorRings[interiorIndex].Vertices[vertexIndex];
                                        PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                        Assert.AreEqual(v.X, gdiPoint.X);
                                        Assert.AreEqual(v.Y, gdiPoint.Y);
                                    }
                                }
                            }
                        }
                        else if (g is MultiPolygon)
                        {
                            MultiPolygon mp = g as MultiPolygon;

                            using (GraphicsPathIterator iter = new GraphicsPathIterator(ro.GdiPath))
                            {
                                foreach (Polygon p in mp)
                                {
                                    Int32 start, end;
                                    Boolean isClosed;
                                    iter.NextSubpath(out start, out end, out isClosed);

                                    Assert.IsTrue(isClosed);
                                    Assert.AreEqual(p.ExteriorRing.Vertices.Count, end - start + 1);

                                    for (Int32 vertexIndex = 0;
                                         vertexIndex < p.ExteriorRing.Vertices.Count;
                                         vertexIndex++)
                                    {
                                        Point v = p.ExteriorRing.Vertices[vertexIndex];
                                        PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                        Assert.AreEqual(v.X, gdiPoint.X);
                                        Assert.AreEqual(v.Y, gdiPoint.Y);
                                    }

                                    for (Int32 interiorIndex = 0; interiorIndex < p.InteriorRings.Count; interiorIndex++)
                                    {
                                        iter.NextSubpath(out start, out end, out isClosed);
                                        Assert.IsTrue(isClosed);
                                        Assert.AreEqual(p.InteriorRings[interiorIndex].Vertices.Count, end - start + 1);

                                        for (Int32 vertexIndex = 0;
                                             vertexIndex < p.InteriorRings[interiorIndex].Vertices.Count;
                                             vertexIndex++)
                                        {
                                            Point v = p.InteriorRings[interiorIndex].Vertices[vertexIndex];
                                            PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                            Assert.AreEqual(v.X, gdiPoint.X);
                                            Assert.AreEqual(v.Y, gdiPoint.Y);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void RenderingLinesWithGdiVectorRendererProducesCorrectGdiPaths()
        {
            using (GdiVectorRenderer vectorRenderer = new GdiVectorRenderer())
            {
                BasicGeometryRenderer2D<GdiRenderObject> geometryRenderer
                    = new BasicGeometryRenderer2D<GdiRenderObject>(vectorRenderer);

                FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();

                foreach (IFeatureDataRecord record in provider.ExecuteIntersectionQuery(provider.GetExtents()))
                {
                    IEnumerable<GdiRenderObject> renderedObjects = geometryRenderer.RenderFeature(record);

                    Int32 geoIndex = 0;
                    Geometry g = record.Geometry;

                    foreach (GdiRenderObject ro in renderedObjects)
                    {
                        using (GraphicsPathIterator iter = new GraphicsPathIterator(ro.GdiPath))
                        {
                            Int32 start, end;
                            Boolean isClosed;
                            iter.NextSubpath(out start, out end, out isClosed);

                            if (g is LineString)
                            {
                                Assert.IsFalse(isClosed);
                                LineString ls = g as LineString;
                                Assert.AreEqual(1, iter.SubpathCount);
                                for (Int32 vertexIndex = 0; vertexIndex < ls.Vertices.Count; vertexIndex++)
                                {
                                    Point v = ls.Vertices[vertexIndex];
                                    PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                    Assert.AreEqual(v.X, gdiPoint.X);
                                    Assert.AreEqual(v.Y, gdiPoint.Y);
                                }
                            }
                            else if (g is MultiLineString)
                            {
                                Assert.IsFalse(isClosed);
                                MultiLineString mls = g as MultiLineString;
                                Assert.AreEqual(mls.Collection.Count, iter.SubpathCount);
                                foreach (LineString lineString in mls)
                                {
                                    for (Int32 vertexIndex = 0; vertexIndex < lineString.Vertices.Count; vertexIndex++)
                                    {
                                        Point v = lineString.Vertices[vertexIndex];
                                        PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                        Assert.AreEqual(v.X, gdiPoint.X);
                                        Assert.AreEqual(v.Y, gdiPoint.Y);
                                    }

                                    iter.NextSubpath(out start, out end, out isClosed);
                                }
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void RenderingPointsGeometriesWithGdiVectorRendererProducesCorrectGdiBitmaps()
        {
            using (GdiVectorRenderer vectorRenderer = new GdiVectorRenderer())
            {
                BasicGeometryRenderer2D<GdiRenderObject> geometryRenderer
                    = new BasicGeometryRenderer2D<GdiRenderObject>(vectorRenderer);

                FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();

                foreach (IFeatureDataRecord record in provider.ExecuteIntersectionQuery(provider.GetExtents()))
                {
                    IEnumerable<GdiRenderObject> renderedObjects = geometryRenderer.RenderFeature(record);

                    Int32 geoIndex = 0;
                    foreach (GdiRenderObject ro in renderedObjects)
                    {
                        Geometry g = record.Geometry;

                        if (g is MultiPoint)
                        {
                            MultiPoint mp = g as MultiPoint;
                            Assert.AreEqual(mp[geoIndex].X, ro.Bounds.Location.X, _e);
                            Assert.AreEqual(mp[geoIndex].Y, ro.Bounds.Location.Y, _e);
                            geoIndex++;
                        }
                        else if (g is Point)
                        {
                            Point p = g as Point;
                            Assert.AreEqual(p.X, ro.Bounds.Location.X, _e);
                            Assert.AreEqual(p.Y, ro.Bounds.Location.Y, _e);
                        }
                    }
                }
            }
        }
    }
}