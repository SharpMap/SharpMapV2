using GeoAPI.Geometries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Coordinates;
using NetTopologySuite.Operation.Buffer;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Data.Providers.PostGis.Tests
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für UnrelatedTests
    /// </summary>
    [TestClass]
    public class UnrelatedTests
    {
        private static IGeometryFactory _geometryFactory;

        /// <summary>
        ///Ruft den Textkontext mit Informationen über
        ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Zusätzliche Testattribute

        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            SridMap.DefaultInstance =
                new SridMap(new[] {new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory)});
            _geometryFactory = new GeometryServices().DefaultGeometryFactory;
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
                new BufferBuilder<BufferedCoordinate>((IGeometryFactory<BufferedCoordinate>) _geometryFactory);
            IGeometry<BufferedCoordinate> res = bb.Buffer((IGeometry<BufferedCoordinate>) ls, 10);
            Assert.IsNotNull(res);
        }
    }
}