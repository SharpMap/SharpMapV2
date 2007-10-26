using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SharpMap.Geometries;
using SharpMap.Indexing.RTree;

namespace SharpMap.Tests.Indexing
{
    [TestFixture]
    public class RTreeTests
    {
        [Test]
        public void CreateRTreeTest()
        {
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(),
                                                            new GuttmanQuadraticSplit<Int32>(),
                                                            new DynamicRTreeBalanceHeuristic());

            rTree.Dispose();
        }

        [Test]
        public void InsertTest()
        {
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(),
                                                            new GuttmanQuadraticSplit<Int32>(),
                                                            new DynamicRTreeBalanceHeuristic());

            addEntries(rTree);

            Assert.AreEqual(new BoundingBox(-100, -100, 5928.57523425, 3252.50803582), rTree.Root.BoundingBox);

            rTree.Dispose();
        }

        [Test]
        public void SearchTest()
        {
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(),
                                                            new GuttmanQuadraticSplit<Int32>(),
                                                            new DynamicRTreeBalanceHeuristic());

            addEntries(rTree);

            List<RTreeIndexEntry<Int32>> resultsList = new List<RTreeIndexEntry<Int32>>();

            resultsList.AddRange(rTree.Search(new BoundingBox(-100, -100, 5928.57523425, 3252.50803582)));
            Assert.AreEqual(8, resultsList.Count);
            resultsList.Clear();

            resultsList.AddRange(rTree.Search(new BoundingBox(0, 0, 100, 100)));
            Assert.AreEqual(6, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 3; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 6; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 8; }));
            resultsList.Clear();

            resultsList.AddRange(rTree.Search(new BoundingBox(1500, 1500, 1500, 1500)));
            Assert.AreEqual(2, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 4; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 5; }));
            resultsList.Clear();

            resultsList.AddRange(rTree.Search(new BoundingBox(100, 100, 100, 100)));
            Assert.AreEqual(4, resultsList.Count);
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 8; }));
            resultsList.Clear();

            addRandomEntries(rTree);
            resultsList.AddRange(rTree.Search(new BoundingBox(100, 100, 100, 100)));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 1; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 2; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 7; }));
            Assert.IsTrue(resultsList.Exists(delegate(RTreeIndexEntry<Int32> match) { return match.Value == 8; }));
            resultsList.Clear();

            rTree.Dispose();
        }

        [Test]
        [Ignore("Saving index broken")]
        public void SaveIndexTest()
        {
            DynamicRTree<Int32> rTree = new DynamicRTree<Int32>(new GuttmanQuadraticInsert<Int32>(),
                                                            new GuttmanQuadraticSplit<Int32>(),
                                                            new DynamicRTreeBalanceHeuristic());

            addRandomEntries(rTree);
            MemoryStream s = new MemoryStream();
            rTree.SaveIndex(s);
            rTree.Dispose();

            s.Position = 0;
            DynamicRTree<Int32> rTree2 = DynamicRTree<Int32>.FromStream(s);
            List<RTreeIndexEntry<Int32>> results = new List<RTreeIndexEntry<Int32>>();
            results.AddRange(rTree2.Search(rTree2.Root.BoundingBox));
            Assert.AreEqual(99990, results.Count);
        }

        private static void addEntries(DynamicRTree<Int32> rTree)
        {
            rTree.Insert(new RTreeIndexEntry<Int32>(1, new BoundingBox(0, 0, 100, 100)));
            rTree.Insert(new RTreeIndexEntry<Int32>(2, new BoundingBox(50, 50, 150, 150)));
            rTree.Insert(new RTreeIndexEntry<Int32>(3, new BoundingBox(-100, -100, 0, 0)));
            rTree.Insert(new RTreeIndexEntry<Int32>(4, new BoundingBox(1000, 1000, 2000, 2000)));
            rTree.Insert(
                new RTreeIndexEntry<Int32>(5, new BoundingBox(346.23975, 424.5720832, 5928.57523425, 3252.50803582)));
            rTree.Insert(new RTreeIndexEntry<Int32>(6, new BoundingBox(0, 0, 0, 0)));
            rTree.Insert(new RTreeIndexEntry<Int32>(7, new BoundingBox(100, 100, 100, 100)));
            rTree.Insert(new RTreeIndexEntry<Int32>(8, new BoundingBox(0, 0, 100, 100)));
        }

        private static void addRandomEntries(DynamicRTree<Int32> rTree)
        {
            Random rnd = new Random();

            for (Int32 i = 10; i < 100000; i++)
            {
                Double xMin = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();
                Double xMax = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();
                Double yMin = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();
                Double yMax = rnd.NextDouble()*(rnd.Next(0, 1) == 1 ? -1 : 1)*rnd.Next();

                BoundingBox bounds = new BoundingBox(xMin, yMin, xMax, yMax);
                rTree.Insert(new RTreeIndexEntry<Int32>(i, bounds));
            }
        }
    }
}