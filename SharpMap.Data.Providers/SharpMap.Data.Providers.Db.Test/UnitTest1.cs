using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
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
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        public void TestMethod1()
        {
            var services = new GeometryServices();

            var search = new MsSqlServer2008Provider<long>(services.DefaultGeometryFactory,
                                                 ConfigurationManager.ConnectionStrings["db"].ConnectionString, "dbo",
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
            //DataTable dt = search.TableSchema;

            var binaryExpression =
                new BinaryExpression(new PropertyNameExpression("PostCode"),
                                     BinaryOperator.GreaterThan,
                                     new PropertyNameExpression("Market Sector"));

            var providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[]
                        {
                            new WithNoLockExpression(true), 
                            new OrderByExpression(new []{"PostCode"} ),
                            new ForceIndexExpression( true), 
                            new IndexNamesExpression(new[]{"Index1","Index2"} )
                            
                        });


            var prov = new ProviderQueryExpression(providerProps, new AllAttributesExpression(), binaryExpression);

            object obj = search.ExecuteQuery(prov);

            Assert.IsNotNull(obj);
        }
    }
}
