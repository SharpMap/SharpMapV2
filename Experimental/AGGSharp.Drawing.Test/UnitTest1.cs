using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AGG.PixelFormat;
using System.Diagnostics;
using AGGSharp.Drawing.Interface;

namespace AGGSharp.Drawing.Test
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
        public void TestDependencyInjectionOnPixMap()
        {

            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 1001; i++)
            {
                if (i == 1)
                    sw.Start();

                PixMap m = new PixMap(PixelFormat.RGBA, 100, 100);
                IGraphics g = m.CreateGraphics();
                Debug.Assert(g != null);

                g.BeginGraphicsContext();
                
                g.EndGraphicsContext();
                g.AlphaMask = null;
            }

            sw.Stop();
            Debug.WriteLine(string.Format("{0}ms to create 1000 PixMaps", sw.ElapsedMilliseconds));

        }

    }
}
