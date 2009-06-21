using System;
using System.Collections.Generic;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Index.Strtree;
using GisSharpBlog.NetTopologySuite.Operation.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Coordinates;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Data.Providers.PostGis.Tests
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für BufferTests
    /// </summary>
    [TestClass]
    public class BufferTests
    {
        private static IGeometryFactory<BufferedCoordinate> _geometryFactory;

        private TestContext testContextInstance;

        /// <summary>
        ///Ruft den Textkontext mit Informationen über
        ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Zusätzliche Testattribute

        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            SridMap.DefaultInstance =
                new SridMap(new[] { new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory) });
            _geometryFactory = (IGeometryFactory<BufferedCoordinate>)new GeometryServices().DefaultGeometryFactory;
        }

        //
        // Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
        // [ClassCleanup()]
        public static void MyClassCleanup()
        {
            _geometryFactory = null;
        }

        // Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void Test_T97Buffer()
        {
            ILineString ls = _geometryFactory.WktReader.Read("LINESTRING(0 0, 100 100)") as ILineString;
            Assert.IsNotNull(ls);

            BufferBuilder<BufferedCoordinate> bb =
                new BufferBuilder<BufferedCoordinate>(_geometryFactory);
            IGeometry<BufferedCoordinate> res = bb.Buffer((IGeometry<BufferedCoordinate>)ls, 10);
            Assert.IsNotNull(res);
        }

        readonly Random _rnd = new Random(DateTime.Now.Millisecond);


        public IEnumerable<IGeometry<BufferedCoordinate>> CreateTestGeometries(int count, double minx, double miny, double maxx, double maxy)
        {
            double xrange = Math.Abs(maxx - minx);
            double yrange = Math.Abs(maxy - miny);
            
            for (int i = 0; i < count; i++)
            {
                double x1 = _rnd.NextDouble() * xrange + minx;
                double x2 = _rnd.NextDouble() * xrange + minx;
                double y1 = _rnd.NextDouble() * yrange + miny;
                double y2 = _rnd.NextDouble() * yrange + miny;

                yield return (IGeometry<BufferedCoordinate>)_geometryFactory.CreateExtents2D(Math.Min(x1, x2), Math.Min(y1, y2), Math.Max(x1, x2),
                                                        Math.Max(y1, y2)).ToGeometry();
            }
        }

        [TestMethod]
        public void TestStrIndex()
        {
            StrTree<BufferedCoordinate, IGeometry<BufferedCoordinate>>
                index = new StrTree<BufferedCoordinate, IGeometry<BufferedCoordinate>>(_geometryFactory);

            index.BulkLoad(
                CreateTestGeometries(1000, 0.0, 0.0, 3000.0, 3000.0));
            index.Build();

            IExtents<BufferedCoordinate> queryExtents =
                (IExtents<BufferedCoordinate>)_geometryFactory.CreateExtents2D(100.0, 100.0, 120.0, 120.0);

            IList<IGeometry<BufferedCoordinate>> matches = new List<IGeometry<BufferedCoordinate>>(
                index.Query(queryExtents));


            foreach (IGeometry<BufferedCoordinate> list in matches)
            {
                Assert.IsTrue(list.Bounds.Intersects(queryExtents), "a result from the index does not intersect the query bounds");
            }

        }

    }
}