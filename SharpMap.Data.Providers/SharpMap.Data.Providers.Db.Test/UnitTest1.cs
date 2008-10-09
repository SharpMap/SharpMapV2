using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Expressions;
using SharpMap.Utilities;

namespace SharpMap.Data.Providers.Db.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestSqlServer2008()
        {
            var services = new GeometryServices();

            var search = new MsSqlServer2008Provider<long>(services.DefaultGeometryFactory,
                                                           ConfigurationManager.ConnectionStrings["sql2008"].ConnectionString,
                                                           "dbo",
                                                           "vw_iMARS_BRANCH", "ACSId", "Geom")
                             {
                                 DefaultProviderProperties
                                     = new ProviderPropertiesExpression(
                                     new ProviderPropertyExpression[]
                                         {
                                             new WithNoLockExpression(true),
                                             new ForceIndexExpression(true)
                                         })
                             };

            var binaryExpression =
                new CollectionBinaryExpression(new PropertyNameExpression("PostCode"), CollectionOperator.In, new CollectionExpression(new[] { 3, 4, 5, 6 }));

            var providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[]
                        {
                            new WithNoLockExpression(true),
                            new OrderByExpression(new[] {"PostCode"}),
                            new ForceIndexExpression(true),
                            new IndexNamesExpression(new[] {"Index1", "Index2"})
                        });


            var prov = new ProviderQueryExpression(providerProps, new AllAttributesExpression(), binaryExpression);

            object obj = search.ExecuteQuery(prov);

            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestSqLite()
        {
            var services = new GeometryServices();

            var search = new SpatiaLite2_Provider(services.DefaultGeometryFactory,
                                                  ConfigurationManager.ConnectionStrings["sqLite"].ConnectionString, "main",
                                                  "regions", "OID", "XGeometryX");
            search.SpatiaLiteIndexType = SpatiaLite2_IndexType.MBRCache;

            var binaryExpression =
                new BinaryExpression(new PropertyNameExpression("VHG5"),
                                     BinaryOperator.GreaterThan, new LiteralExpression<int>(6));

            var providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[] { });


            var prov = new ProviderQueryExpression(providerProps, new AllAttributesExpression(), binaryExpression);

            object obj = search.ExecuteQuery(prov);

            Assert.IsNotNull(obj);
        }
    }
}