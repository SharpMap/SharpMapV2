using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Tests;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;

namespace SharpMap.Rendering.Gdi.Tests
{
    [TestFixture]
    public class BasicGeometryRenderer2DWithGdiVectorRenderer
    {
        private static readonly Single _e = 0.0001f;
        private IGeometryFactory _geoFactory;

        [SetUp]
        public void Setup()
        {
            ICoordinateSequenceFactory<BufferedCoordinate2D> sequenceFactory 
                = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);
        }

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

                FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);

                foreach (IFeatureDataRecord record in provider.ExecuteIntersectionQuery(provider.GetExtents()))
                {
                    IEnumerable<GdiRenderObject> renderedObjects = geometryRenderer.RenderFeature(record);

                    foreach (GdiRenderObject ro in renderedObjects)
                    {
                        IGeometry g = record.Geometry;

                        if (g is IPolygon)
                        {
                            IPolygon p = g as IPolygon;
                            using (GraphicsPathIterator iter = new GraphicsPathIterator(ro.GdiPath))
                            {
                                Int32 start, end;
                                Boolean isClosed;
                                iter.NextSubpath(out start, out end, out isClosed);

                                Assert.IsTrue(isClosed);
                                Assert.AreEqual(p.ExteriorRing.Coordinates.Count, end - start + 1);

                                for (Int32 vertexIndex = 0; vertexIndex < p.ExteriorRing.Coordinates.Count; vertexIndex++)
                                {
                                    ICoordinate v = (ICoordinate)p.ExteriorRing.Coordinates[vertexIndex];
                                    PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                    Assert.AreEqual(v[Ordinates.X], gdiPoint.X);
                                    Assert.AreEqual(v[Ordinates.Y], gdiPoint.Y);
                                }

                                foreach (ILineString interiorRing in p.InteriorRings)
                                {
                                    iter.NextSubpath(out start, out end, out isClosed);
                                    Assert.IsTrue(isClosed);
                                    Assert.AreEqual(interiorRing.PointCount, end - start + 1);

                                    for (Int32 vertexIndex = 0;
                                         vertexIndex < interiorRing.PointCount;
                                         vertexIndex++)
                                    {
                                        ICoordinate v = interiorRing.Coordinates[vertexIndex];
                                        PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                        Assert.AreEqual(v[Ordinates.X], gdiPoint.X);
                                        Assert.AreEqual(v[Ordinates.Y], gdiPoint.Y);
                                    }
                                }
                            }
                        }
                        else if (g is IMultiPolygon)
                        {
                            IMultiPolygon mp = g as IMultiPolygon;

                            using (GraphicsPathIterator iter = new GraphicsPathIterator(ro.GdiPath))
                            {
                                foreach (IPolygon p in (IEnumerable<IPolygon>)mp)
                                {
                                    Int32 start, end;
                                    Boolean isClosed;
                                    iter.NextSubpath(out start, out end, out isClosed);

                                    Assert.IsTrue(isClosed);
                                    Int32 exteriorPointCount = p.ExteriorRing.PointCount;
                                    Assert.AreEqual(exteriorPointCount, end - start + 1);

                                    for (Int32 vertexIndex = 0; vertexIndex < exteriorPointCount; vertexIndex++)
                                    {
                                        ICoordinate v = p.ExteriorRing.Coordinates[vertexIndex];
                                        PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                        Assert.AreEqual(v[Ordinates.X], gdiPoint.X);
                                        Assert.AreEqual(v[Ordinates.Y], gdiPoint.Y);
                                    }

                                    foreach (ILineString interiorRing in p.InteriorRings)
                                    {
                                        iter.NextSubpath(out start, out end, out isClosed);
                                        Assert.IsTrue(isClosed);
                                        Assert.AreEqual(interiorRing.PointCount, end - start + 1);

                                        for (Int32 vertexIndex = 0;
                                             vertexIndex < interiorRing.PointCount;
                                             vertexIndex++)
                                        {
                                            ICoordinate v = interiorRing.Coordinates[vertexIndex];
                                            PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                            Assert.AreEqual(v[Ordinates.X], gdiPoint.X);
                                            Assert.AreEqual(v[Ordinates.Y], gdiPoint.Y);
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

                FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);

                foreach (IFeatureDataRecord record in provider.ExecuteIntersectionQuery(provider.GetExtents()))
                {
                    IEnumerable<GdiRenderObject> renderedObjects = geometryRenderer.RenderFeature(record);

                    Int32 geoIndex = 0;
                    IGeometry g = record.Geometry;

                    foreach (GdiRenderObject ro in renderedObjects)
                    {
                        using (GraphicsPathIterator iter = new GraphicsPathIterator(ro.GdiPath))
                        {
                            Int32 start, end;
                            Boolean isClosed;
                            iter.NextSubpath(out start, out end, out isClosed);

                            if (g is ILineString)
                            {
                                Assert.IsFalse(isClosed);
                                ILineString ls = g as ILineString;
                                Assert.AreEqual(1, iter.SubpathCount);
                                for (Int32 vertexIndex = 0; vertexIndex < ls.Coordinates.Count; vertexIndex++)
                                {
                                    ICoordinate v = (ICoordinate)ls.Coordinates[vertexIndex];
                                    PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                    Assert.AreEqual(v[Ordinates.X], gdiPoint.X);
                                    Assert.AreEqual(v[Ordinates.Y], gdiPoint.Y);
                                }
                            }
                            else if (g is IMultiLineString)
                            {
                                Assert.IsFalse(isClosed);
                                IMultiLineString mls = g as IMultiLineString;
                                Assert.AreEqual(mls.Count, iter.SubpathCount);
                                foreach (ILineString lineString in (IEnumerable<ILineString>)mls)
                                {
                                    for (Int32 vertexIndex = 0; vertexIndex < lineString.Coordinates.Count; vertexIndex++)
                                    {
                                        ICoordinate v = (ICoordinate)lineString.Coordinates[vertexIndex];
                                        PointF gdiPoint = ro.GdiPath.PathPoints[vertexIndex + start];
                                        Assert.AreEqual(v[Ordinates.X], gdiPoint.X);
                                        Assert.AreEqual(v[Ordinates.Y], gdiPoint.Y);
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

                FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);

                foreach (IFeatureDataRecord record in provider.ExecuteIntersectionQuery(provider.GetExtents()))
                {
                    IEnumerable<GdiRenderObject> renderedObjects = geometryRenderer.RenderFeature(record);

                    Int32 geoIndex = 0;
                    foreach (GdiRenderObject ro in renderedObjects)
                    {
                        IGeometry g = record.Geometry;

                        if (g is IMultiPoint)
                        {
                            IMultiPoint mp = g as IMultiPoint;
                            Assert.AreEqual(mp[geoIndex][Ordinates.X], ro.Bounds.Location.X, _e);
                            Assert.AreEqual(mp[geoIndex][Ordinates.Y], ro.Bounds.Location.Y, _e);
                            geoIndex++;
                        }
                        else if (g is IPoint)
                        {
                            IPoint p = g as IPoint;
                            Assert.AreEqual(p[Ordinates.X], ro.Bounds.Location.X, _e);
                            Assert.AreEqual(p[Ordinates.Y], ro.Bounds.Location.Y, _e);
                        }
                    }
                }
            }
        }
    }
}