using System;
using System.Collections.Generic;
using System.IO;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NPack;
using NUnit.Framework;
using SharpMap.Indexing.RTree;

namespace SharpMap.Tests.Indexing
{
    [TestFixture]
    public class RTreeTests
    {
        class BoundedInt32 : IBoundable<IExtents>
        {
            public readonly Int32 Value;
            public readonly IExtents Bounds;

            public BoundedInt32(Int32 value, IExtents bounds)
            {
                Value = value;
                Bounds = bounds;
            }

            #region IBoundable<IExtents> Members

            IExtents IBoundable<IExtents>.Bounds
            {
                get { return Bounds; }
            }

            public Boolean Intersects(IExtents bounds)
            {
                return Bounds.Intersects(bounds);
            }

            #endregion
        }

        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);
        }

        [Test]
        public void DynamicRTreeCreationAndDisposalSucceeds()
        {
            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            new DynamicRTreeBalanceHeuristic());

            rTree.Dispose();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DynamicRTreeCreationWithNullInsertionStrategyFails()
        {
            new DynamicRTree<BoundedInt32>(
                            _geoFactory,
                            null,
                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                            new DynamicRTreeBalanceHeuristic());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DynamicRTreeCreationWithNullSplitStrategyFails()
        {
            new DynamicRTree<BoundedInt32>(
                            _geoFactory,
                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                            null,
                            new DynamicRTreeBalanceHeuristic());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DynamicRTreeCreationWithNullBalanceHeuristicFails()
        {
            new DynamicRTree<BoundedInt32>(
                            _geoFactory,
                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                            null);
        }

        [Test]
        public void InsertingItemsUnderMaxNodeItemsSucceeds()
        {
            DynamicRTreeBalanceHeuristic balanceHeuristic = new DynamicRTreeBalanceHeuristic();

            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            balanceHeuristic);

            addEntries(rTree, balanceHeuristic.NodeItemMaximumCount - 2);

            IExtents expected = _geoFactory.CreateExtents2D(
                -100, -100, 5928.57523425, 3252.50803582);

            Assert.AreEqual(expected, rTree.Root.Bounds);

            rTree.Dispose();
        }

        [Test]
        public void InsertingItemsOverMaxNodeItemsSucceeds()
        {
            DynamicRTreeBalanceHeuristic balanceHeuristic = new DynamicRTreeBalanceHeuristic();

            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            balanceHeuristic);

            addEntries(rTree, balanceHeuristic.NodeItemMaximumCount + 1);

            IExtents expected = _geoFactory.CreateExtents2D(
                -100, -100, 5928.57523425, 3252.50803582);

            Assert.AreEqual(expected, rTree.Root.Bounds);

            rTree.Dispose();
        }

        [Test]
        public void SearchingInIndexWithLessThanMaxNodeItemsSucceeds()
        {
            DynamicRTreeBalanceHeuristic balanceHeuristic = new DynamicRTreeBalanceHeuristic();

            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            balanceHeuristic);

            addEntries(rTree, balanceHeuristic.NodeItemMaximumCount - 2);

            List<Int32> resultsList = new List<Int32>();

            IExtents searchExtents = _geoFactory.CreateExtents2D(-100, -100, 5928.57523425, 3252.50803582);
#if DOTNET35
            // Man, I can't wait.
            resultsList.AddRange(rTree.Query(searchExtents, x => x.Value));
#else
            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item)
                                                            {
                                                                return item.Value;
                                                            }));
#endif
            Assert.AreEqual(8, resultsList.Count);

            resultsList.Clear();    

            searchExtents = _geoFactory.CreateExtents2D(0, 0, 100, 100);

            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item)
            {
                return item.Value;
            }));

            Assert.AreEqual(6, resultsList.Count);
#if DOTNET35
            Assert.IsTrue(resultsList.Exists(x => x == 1));
            Assert.IsTrue(resultsList.Exists(x => x == 2));
            Assert.IsTrue(resultsList.Exists(x => x == 3));
            Assert.IsTrue(resultsList.Exists(x => x == 6));
            Assert.IsTrue(resultsList.Exists(x => x == 7));
            Assert.IsTrue(resultsList.Exists(x => x == 8));
#else
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 3; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 6; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
#endif
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(1500, 1500, 1500, 1500);
            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item) { return item.Value; }));
            Assert.AreEqual(2, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 4; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 5; }));
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(100, 100, 100, 100);
            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item) { return item.Value; }));
            Assert.AreEqual(4, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

        }

        [Test]
        public void SearchingInIndexWithMoreThanMaxNodeItemsSucceeds()
        {
            DynamicRTreeBalanceHeuristic balanceHeuristic = new DynamicRTreeBalanceHeuristic();

            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            balanceHeuristic);

            addEntries(rTree, balanceHeuristic.NodeItemMaximumCount + 1);

            List<Int32> resultsList = new List<Int32>();

            IExtents searchExtents = _geoFactory.CreateExtents2D(-100, -100, 5928.57523425, 3252.50803582);

            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item)
            {
                return item.Value;
            }));

            Assert.AreEqual(11, resultsList.Count);

            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(0, 0, 100, 100);

            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item)
            {
                return item.Value;
            }));

            Assert.AreEqual(9, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 3; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 6; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(1500, 1500, 1500, 1500);
            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item) { return item.Value; }));
            Assert.AreEqual(2, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 4; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 5; }));
            resultsList.Clear();

            searchExtents = _geoFactory.CreateExtents2D(100, 100, 100, 100);
            resultsList.AddRange(rTree.Query(searchExtents, delegate(BoundedInt32 item) { return item.Value; }));
            Assert.AreEqual(7, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 9; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 10; }));
            Assert.IsTrue(resultsList.Exists(delegate(Int32 match) { return match == 11; }));

            rTree.Dispose();
        }

        [Test]
        [Ignore]
        public void LoadingWith100000ObjectsCompletesAndDoesntCrash()
        {
            DynamicRTreeBalanceHeuristic balanceHeuristic = new DynamicRTreeBalanceHeuristic();

            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            balanceHeuristic);

            List<Int32> resultsList = new List<Int32>();

            addRandomEntries(rTree);

            rTree.Dispose();
        }

        [Test]
        [Ignore("Saving index broken")]
        public void SaveIndexTest()
        {
            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _geoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_geoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_geoFactory),
                                                            new DynamicRTreeBalanceHeuristic());

            addRandomEntries(rTree);
            MemoryStream s = new MemoryStream();
            rTree.SaveIndex(s);
            rTree.Dispose();

            s.Position = 0;
            DynamicRTree<BoundedInt32> rTree2 = DynamicRTree<BoundedInt32>.FromStream(s, _geoFactory);
            List<Int32> results = new List<Int32>();
            results.AddRange(rTree2.Query(rTree2.Root.Bounds, delegate(BoundedInt32 item) { return item.Value; }));
            Assert.AreEqual(99990, results.Count);
        }

        private void addEntries(ISpatialIndex<IExtents, BoundedInt32> rTree, Int32 items)
        {
            if (items > 0)
            {
                rTree.Insert(new BoundedInt32(1, _geoFactory.CreateExtents2D(0, 0, 100, 100)));
            }

            if (items > 1)
            {
                rTree.Insert(new BoundedInt32(2, _geoFactory.CreateExtents2D(50, 50, 150, 150)));
            }

            if (items > 2)
            {
                rTree.Insert(new BoundedInt32(3, _geoFactory.CreateExtents2D(-100, -100, 0, 0)));
            }

            if (items > 3)
            {
                rTree.Insert(new BoundedInt32(4, _geoFactory.CreateExtents2D(1000, 1000, 2000, 2000)));
            }

            if (items > 4)
            {
                rTree.Insert(new BoundedInt32(5, _geoFactory.CreateExtents2D(346.23975, 424.5720832, 5928.57523425, 3252.50803582)));
            }

            if (items > 5)
            {
                rTree.Insert(new BoundedInt32(6, _geoFactory.CreateExtents2D(0, 0, 0, 0)));
            }

            if (items > 6)
            {
                rTree.Insert(new BoundedInt32(7, _geoFactory.CreateExtents2D(100, 100, 100, 100)));
            }

            if (items > 7)
            {
                rTree.Insert(new BoundedInt32(8, _geoFactory.CreateExtents2D(0, 0, 100, 100)));
            }

            if (items > 8)
            {
                rTree.Insert(new BoundedInt32(9, _geoFactory.CreateExtents2D(0, 0, 100, 100)));
            }

            if (items > 9)
            {
                rTree.Insert(new BoundedInt32(10, _geoFactory.CreateExtents2D(0, 0, 100, 100)));
            }

            if (items > 10)
            {
                rTree.Insert(new BoundedInt32(11, _geoFactory.CreateExtents2D(0, 0, 100, 100)));
            }
        }

        private void addRandomEntries(ISpatialIndex<IExtents, BoundedInt32> rTree)
        {
            Random rnd = new MersenneTwister();

            for (Int32 i = 0; i < 100000; i++)
            {
                Double xMin = rnd.NextDouble() * (rnd.Next(0, 1) == 1 ? -1 : 1) * rnd.Next();
                Double xMax = rnd.NextDouble() * (rnd.Next(0, 1) == 1 ? -1 : 1) * rnd.Next();
                Double yMin = rnd.NextDouble() * (rnd.Next(0, 1) == 1 ? -1 : 1) * rnd.Next();
                Double yMax = rnd.NextDouble() * (rnd.Next(0, 1) == 1 ? -1 : 1) * rnd.Next();

                IExtents bounds = _geoFactory.CreateExtents2D(xMin, yMin, xMax, yMax);
                rTree.Insert(new BoundedInt32(i, bounds));
            }
        }
    }
}