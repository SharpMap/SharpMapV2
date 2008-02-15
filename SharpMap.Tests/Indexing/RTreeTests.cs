using System;
using System.Collections.Generic;
using System.IO;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using SharpMap.Indexing.RTree;
using SharpMap.SimpleGeometries;

namespace SharpMap.Tests.Indexing
{
    [TestFixture]
    public class RTreeTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DFactory coordFactory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory(coordFactory, sequenceFactory);
        }

        [Test]
        public void CreateRTreeTest()
        {
            Func<Int32, IExtents> bounder = null;
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<Int32>(_geoFactory, bounder),
                                                            new DynamicRTreeBalanceHeuristic(), bounder);

            rTree.Dispose();
        }

        [Test]
        public void InsertTest()
        {
            Func<Int32, IExtents> bounder = null;
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<Int32>(_geoFactory, bounder),
                                                            new DynamicRTreeBalanceHeuristic(), bounder);

            addEntries(rTree);

            IExtents expected = _geoFactory.CreateExtents2D(
                -100, -100, 5928.57523425, 3252.50803582);

            Assert.AreEqual(expected, rTree.Root.Bounds);

            rTree.Dispose();
        }

        [Test]
        public void SearchTest()
        {
            Func<Int32, IExtents> bounder = null;
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<Int32>(_geoFactory, bounder),
                                                            new DynamicRTreeBalanceHeuristic(), bounder);

            addEntries(rTree);

            List<Int32> resultsList = new List<Int32>();

            IExtents searchExtents = _geoFactory.CreateExtents2D(-100, -100, 5928.57523425, 3252.50803582);
            resultsList.AddRange(rTree.Query(searchExtents));
            Assert.AreEqual(8, resultsList.Count);
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(0, 0, 100, 100);
            resultsList.AddRange(rTree.Query(searchExtents));
            Assert.AreEqual(6, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 3; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 6; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(1500, 1500, 1500, 1500);
            resultsList.AddRange(rTree.Query(searchExtents));
            Assert.AreEqual(2, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 4; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 5; }));
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(100, 100, 100, 100);
            resultsList.AddRange(rTree.Query(searchExtents));
            Assert.AreEqual(4, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

            addRandomEntries(rTree);
            resultsList.AddRange(rTree.Query(searchExtents));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

            rTree.Dispose();
        }

        [Test]
        [Ignore("Saving index broken")]
        public void SaveIndexTest()
        {
            Func<Int32, IExtents> bounder = null;
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<Int32>(_geoFactory, bounder),
                                                            new DynamicRTreeBalanceHeuristic(), bounder);

            addRandomEntries(rTree);
            MemoryStream s = new MemoryStream();
            rTree.SaveIndex(s);
            rTree.Dispose();

            s.Position = 0;
            DynamicRTree<Int32> rTree2 = DynamicRTree<Int32>.FromStream(s, bounder, _geoFactory);
            List<Int32> results = new List<Int32>();
            results.AddRange(rTree2.Query(rTree2.Root.Bounds));
            Assert.AreEqual(99990, results.Count);
        }

        private void addEntries(ISpatialIndex<IExtents, int> rTree)
        {
            rTree.Insert(_geoFactory.CreateExtents2D(0, 0, 100, 100), 1);
            rTree.Insert(_geoFactory.CreateExtents2D(50, 50, 150, 150), 2);
            rTree.Insert(_geoFactory.CreateExtents2D(-100, -100, 0, 0), 3);
            rTree.Insert(_geoFactory.CreateExtents2D(1000, 1000, 2000, 2000), 4);
            rTree.Insert(_geoFactory.CreateExtents2D(346.23975, 424.5720832, 5928.57523425, 3252.50803582), 5);
            rTree.Insert(_geoFactory.CreateExtents2D(0, 0, 0, 0), 6);
            rTree.Insert(_geoFactory.CreateExtents2D(100, 100, 100, 100), 7);
            rTree.Insert(_geoFactory.CreateExtents2D(0, 0, 100, 100), 8);
        }

        private void addRandomEntries(ISpatialIndex<IExtents, int> rTree)
        {
            Random rnd = new Random();

            for (Int32 i = 10; i < 100000; i++)
            {
                Double xMin = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();
                Double xMax = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();
                Double yMin = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();
                Double yMax = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();

                IExtents bounds = _geoFactory.CreateExtents2D(xMin, yMin, xMax, yMax);
                rTree.Insert(bounds, i);
            }
        }
    }
}