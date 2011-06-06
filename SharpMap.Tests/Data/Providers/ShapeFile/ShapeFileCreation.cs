using System;
using System.Data;
using GeoAPI.Coordinates;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Expressions;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;
using Xunit;

namespace SharpMap.Tests.Data.Providers.ShapeFile
{
    public class ShapeFileCreation
    {
        static ShapeFileCreation()
        {
            SridMap.DefaultInstance = new SridMap(new [] { new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory)});
        }

        private readonly IGeometryFactory _geometryFactory = new GeometryServices()["EPSG:4326"];

        [Fact]
        public void CreatePointShapeFile()
        {
            var fdt = CreatePointFeatureDataTable(_geometryFactory);
            using (var sfp = ShapeFileProvider.Create(".", "Point", ShapeType.Point, fdt, _geometryFactory, new GeometryServices().CoordinateSystemFactory))
            {
                sfp.Open(WriteAccess.ReadWrite);
                foreach (FeatureDataRow row in fdt.Rows)
                    sfp.Insert(row);
            }


            int number = 0;
            var gs = new GeometryServices();
            using (var sfp = new ShapeFileProvider("Point.shp", gs.DefaultGeometryFactory, gs.CoordinateSystemFactory))
            {
                sfp.IsSpatiallyIndexed = false;
                sfp.Open(WriteAccess.ReadOnly);
                using(var p = sfp.ExecuteFeatureQuery(FeatureQueryExpression.Intersects(sfp.GetExtents())))
                {
                    while (p.Read())
                    {
                        number++;
                        Console.WriteLine(string.Format("{0}; {1}; {2}", p.GetOid(), p.Geometry.GeometryTypeName, p.Geometry));
                    }
                }
            }
            Assert.True(number==100);
        }

        [Fact]
        public void CreateMultiPointShapeFile()
        {
            var fdt = CreateMultiPointFeatureDataTable(_geometryFactory);
            using (var sfp = ShapeFileProvider.Create(".", "MultiPoint", ShapeType.MultiPoint, fdt, _geometryFactory, new GeometryServices().CoordinateSystemFactory))
            {
                sfp.Open(WriteAccess.ReadWrite);
                foreach (FeatureDataRow row in fdt.Rows)
                    sfp.Insert(row);
            }


            int number = 0;
            var gs = new GeometryServices();
            using (var sfp = new ShapeFileProvider("MultiPoint.shp", gs.DefaultGeometryFactory, gs.CoordinateSystemFactory))
            {
                sfp.IsSpatiallyIndexed = false;
                sfp.Open(WriteAccess.ReadOnly);
                using (var p = sfp.ExecuteFeatureQuery(FeatureQueryExpression.Intersects(sfp.GetExtents())))
                {
                    while (p.Read())
                    {
                        number++;
                        Console.WriteLine(string.Format("{0}; {1}; {2}", p.GetOid(), p.Geometry.GeometryTypeName, p.Geometry));
                    }
                }
            }
            Assert.True(number == 100);
        }

        [Fact]
        public void CreateLinealShapeFile()
        {
            var fdt = CreateLinealFeatureDataTable(_geometryFactory);
            using (var sfp = ShapeFileProvider.Create(".", "Lineal", ShapeType.PolyLine, fdt, _geometryFactory, new GeometryServices().CoordinateSystemFactory))
            {
                sfp.Open(WriteAccess.ReadWrite);
                foreach (FeatureDataRow row in fdt.Rows)
                    sfp.Insert(row);
            }


            int number = 0;
            var gs = new GeometryServices();
            using (var sfp = new ShapeFileProvider("Lineal.shp", gs.DefaultGeometryFactory, gs.CoordinateSystemFactory))
            {
                sfp.IsSpatiallyIndexed = false;
                sfp.Open(WriteAccess.ReadOnly);
                using (var p = sfp.ExecuteFeatureQuery(FeatureQueryExpression.Intersects(sfp.GetExtents())))
                {
                    while (p.Read())
                    {
                        number++;
                        Console.WriteLine(string.Format("{0}; {1}; {2}", p.GetOid(), p.Geometry.GeometryTypeName, p.Geometry));
                    }
                }
            }
            Assert.True(number == 100);
        }

        [Fact]
        public void CreatePolygonalShapeFile()
        {
            var fdt = CreatePolygonalFeatureDataTable(_geometryFactory);
            using (var sfp = ShapeFileProvider.Create(".", "Polygonal", ShapeType.Polygon, fdt, _geometryFactory, new GeometryServices().CoordinateSystemFactory))
            {
                sfp.Open(WriteAccess.ReadWrite);
                foreach (FeatureDataRow row in fdt.Rows)
                    sfp.Insert(row);
            }


            int number = 0;
            var gs = new GeometryServices();
            using (var sfp = new ShapeFileProvider("Polygonal.shp", gs.DefaultGeometryFactory, gs.CoordinateSystemFactory))
            {
                sfp.IsSpatiallyIndexed = false;
                sfp.Open(WriteAccess.ReadOnly);
                using (var p = sfp.ExecuteFeatureQuery(FeatureQueryExpression.Intersects(sfp.GetExtents())))
                {
                    while (p.Read())
                    {
                        number++;
                        Console.WriteLine(string.Format("{0}; {1}; {2}", p.GetOid(), p.Geometry.GeometryTypeName, p.Geometry));
                    }
                }
            }
            Assert.True(number == 100);
        }


        private static FeatureDataTable CreateLinealFeatureDataTable(IGeometryFactory geometryFactory)
        {
            var fdt = new FeatureDataTable<uint>("OID", geometryFactory);
            fdt.Columns.AddRange(new[] { new DataColumn("Length", typeof(double)), new DataColumn("Y", typeof(double)), });
            for (uint i = 1; i <= 100; i++)
            {
                var row = fdt.NewRow(i);
                var geom = RandomLineal(geometryFactory);
                row.Geometry = geom;
                if (geom is ILineString)
                    row["Length"] = ((ILineString) geom).Length;
                else
                    row["Length"] = ((IMultiLineString)geom).Length;
                fdt.Rows.Add(row);
            }
            Assert.True(fdt.Rows.Count == 100);
            return fdt;
        }

        private static ILineal RandomLineal(IGeometryFactory geometryFactory)
        {
            switch (Random.Next(0,2))
            {
                case 0:
                    return geometryFactory.CreateLineString(RandomCoordinates(geometryFactory, 2, 10));
                case 1:
                    var ls = new ILineString[Random.Next(2, 5)];
                    for (int i = 0; i < ls.Length; i++)
                        ls[i] = geometryFactory.CreateLineString(RandomCoordinates(geometryFactory, 2, 15));
                    return geometryFactory.CreateMultiLineString(ls);
            }
            return geometryFactory.CreateLineString(RandomCoordinates(geometryFactory, 2, 10));
        }

        private static FeatureDataTable CreatePointFeatureDataTable(IGeometryFactory geometryFactory)
        {
            var fdt = new FeatureDataTable<uint>("OID", geometryFactory);
            fdt.Columns.AddRange( new[] { new DataColumn("X", typeof(double)), new DataColumn("Y", typeof(double)),  });
            for (uint i = 1; i <= 100; i++)
            {
                var row = fdt.NewRow(i);
                var geom = row.Geometry = RandomPoint(geometryFactory);
                row["X"] = geom.Centroid[Ordinates.X];
                row["Y"] = geom.Centroid[Ordinates.Y];
                fdt.Rows.Add(row);
            }
            Assert.True(fdt.Rows.Count == 100);
            return fdt;
        }

        private static FeatureDataTable CreateMultiPointFeatureDataTable(IGeometryFactory geometryFactory)
        {
            var fdt = new FeatureDataTable<uint>("OID", geometryFactory);
            fdt.Columns.AddRange(new[] { new DataColumn("X", typeof(double)), new DataColumn("Y", typeof(double)), });
            for (uint i = 1; i <= 100; i++)
            {
                var row = fdt.NewRow(i);
                var geom = row.Geometry = RandomMultiPoint(geometryFactory);
                row["X"] = geom.Centroid[Ordinates.X];
                row["Y"] = geom.Centroid[Ordinates.Y];
                fdt.Rows.Add(row);
            }
            Assert.True(fdt.Rows.Count == 100);
            return fdt;
        }


        private static FeatureDataTable CreatePolygonalFeatureDataTable(IGeometryFactory geometryFactory)
        {
            var fdt = new FeatureDataTable<uint>("OID", geometryFactory);
            fdt.Columns.AddRange(new[] { new DataColumn("Area", typeof(double)), });
            for (uint i = 1; i <= 100; i++)
            {
                var row = fdt.NewRow(i);
                var geom = RandomPolygonal(geometryFactory);
                Assert.True(((ILinearRing)geom.ExteriorRing).IsCcw);
                Assert.False(((ILinearRing)(Enumerable.First(geom.InteriorRings))).IsCcw);
                row.Geometry = geom;
                row["Area"] = geom.Area;
                fdt.Rows.Add(row);
            }
            Assert.True(fdt.Rows.Count == 100);
            return fdt;
        }

        private static IPolygon RandomPolygonal(IGeometryFactory geometryFactory)
        {
            switch (Random.Next(0, 1))
            {
                case 0:
                    var center = RandomCoordinate(geometryFactory.CoordinateFactory);
                    double a = Random.Next(2, 5);
                    double b = Random.Next(2, 5);
                    var shell = CreateEllipse(geometryFactory, center, a, b, false);
                    a *= 0.5d;
                    b *= 0.35d;
                    var holes = new[] {CreateEllipse(geometryFactory, center, a, b, true)};
                    return geometryFactory.CreatePolygon(shell, holes);
                default:
                    break;

            }
            return geometryFactory.CreatePolygon();
        }

        private static ILinearRing CreateEllipse(IGeometryFactory geometryFactory, ICoordinate center, double a, double b, bool reverse)
        {
            const double piHalf = Math.PI * 0.5d;
            const int segmentsPerQuadrant = 12;

            double step = piHalf / segmentsPerQuadrant;
            if (reverse) step *= -1;

            var coordFac = geometryFactory.CoordinateFactory;
            var coords = new ICoordinate[4*segmentsPerQuadrant + 1];
            var angle = 0d;
            for (var i = 0; i < 4 * segmentsPerQuadrant; i++)
            {
                coords[i] = coordFac.Create(center[Ordinates.X] + Math.Cos(angle) * a,
                                            center[Ordinates.Y] + Math.Sin(angle) * b);
                angle += step;
            }
            coords[coords.Length - 1] = (ICoordinate)coords[0].Clone();
            
            return geometryFactory.CreateLinearRing(coords);
        }

        private static readonly Random Random = new Random();
        
        private static IPoint RandomPoint(IGeometryFactory geometryFactory)
        {
            return geometryFactory.CreatePoint(RandomCoordinate(geometryFactory.CoordinateFactory));
        }

        private static ICoordinate RandomCoordinate(ICoordinateFactory coordinateFactory)
        {
            return coordinateFactory.Create(Random.Next(-180, 180), Random.Next(-90, 90));
        }

        private static IMultiPoint RandomMultiPoint(IGeometryFactory geometryFactory)
        {
            return geometryFactory.CreateMultiPoint(RandomCoordinates(geometryFactory));
        }

        private static ICoordinateSequence RandomCoordinates(IGeometryFactory geometryFactory)
        {
            return RandomCoordinates(geometryFactory, 2, 10);
        }
        private static ICoordinateSequence RandomCoordinates(IGeometryFactory geometryFactory, int min, int max)
        {
            var csf = geometryFactory.CoordinateSequenceFactory;
            var cf = geometryFactory.CoordinateFactory;
            var number = Random.Next(min, max);
            var coordinates = new ICoordinate[number];
            for (int i = 0; i < number; i++)
                coordinates[i] = RandomCoordinate(cf);
            return csf.Create(coordinates);
        }
    }
}