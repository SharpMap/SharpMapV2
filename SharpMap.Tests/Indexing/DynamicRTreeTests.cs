using System;
using System.Collections.Generic;
using System.IO;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NPack;

using SharpMap.Indexing.RTree;
using Xunit;

namespace SharpMap.Tests.Indexing
{

    public class DynamicRTreeTests : IUseFixture<FixtureFactories>
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

        private FixtureFactories _factories;

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }

        [Fact]
        public void DynamicRTreeCreationAndDisposalSucceeds()
        {
            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _factories.GeoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_factories.GeoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_factories.GeoFactory),
                                                            new DynamicRTreeBalanceHeuristic());

            rTree.Dispose();
        }

        [Fact]
        public void DynamicRTreeCreationWithNullInsertionStrategyFails()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         new DynamicRTree<BoundedInt32>(
                                                             _factories.GeoFactory,
                                                             null,
                                                             new GuttmanQuadraticSplit<BoundedInt32>(
                                                                 _factories.GeoFactory),
                                                             new DynamicRTreeBalanceHeuristic());
                                                     });
        }

        [Fact]
        public void DynamicRTreeCreationWithNullSplitStrategyFails()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         new DynamicRTree<BoundedInt32>(
                                                                         _factories.GeoFactory,
                                                                         new GuttmanQuadraticInsert<BoundedInt32>(_factories.GeoFactory),
                                                                         null,
                                                                         new DynamicRTreeBalanceHeuristic());
                                                     });
        }

        [Fact]
        public void DynamicRTreeCreationWithNullBalanceHeuristicFails()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         new DynamicRTree<BoundedInt32>(
                                                                         _factories.GeoFactory,
                                                                         new GuttmanQuadraticInsert<BoundedInt32>(_factories.GeoFactory),
                                                                         new GuttmanQuadraticSplit<BoundedInt32>(_factories.GeoFactory),
                                                                         null);
                                                     });
        }

        [Fact]
        public void InsertingItemsUnderMaxNodeItemsSucceeds()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            addEntries(rTree, null, nodeItemMax - 2);

            IExtents expected = _factories.GeoFactory.CreateExtents2D(-100,
                                                            -100,
                                                            5928.57523425,
                                                            3252.50803582);

            Assert.Equal(expected, rTree.Root.Bounds);

            rTree.Dispose();
        }

        [Fact]
        public void InsertingItemsOverMaxNodeItemsSucceeds()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            addEntries(rTree, null, nodeItemMax + 1);

            IExtents expected = _factories.GeoFactory.CreateExtents2D(-100,
                                                            -100,
                                                            5928.57523425,
                                                            3252.50803582);

            Assert.Equal(expected, rTree.Root.Bounds);

            rTree.Dispose();
        }

        [Fact]
        public void SearchingInIndexWithLessThanMaxNodeItemsSucceeds()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            addEntries(rTree, null, nodeItemMax - 2);

            List<Int32> resultsList = new List<Int32>();

            IExtents searchExtents = _factories.GeoFactory.CreateExtents2D(-100,
                                                            -100,
                                                            5928.57523425,
                                                            3252.50803582);
#if DOTNET35
            // Man, I can't wait.
            resultsList.AddRange(rTree.Query(searchExtents, x => x.Value));
#else
            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
                                                            {
                                                                return item.Value;
                                                            }));
#endif
            Assert.Equal(8, resultsList.Count);

            resultsList.Clear();

            searchExtents = _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100);

            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
            {
                return item.Value;
            }));

            Assert.Equal(6, resultsList.Count);
#if DOTNET35
            Assert.True(resultsList.Exists(x => x == 1));
            Assert.True(resultsList.Exists(x => x == 2));
            Assert.True(resultsList.Exists(x => x == 3));
            Assert.True(resultsList.Exists(x => x == 6));
            Assert.True(resultsList.Exists(x => x == 7));
            Assert.True(resultsList.Exists(x => x == 8));
#else
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 3; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 6; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
#endif
            resultsList.Clear();

            searchExtents = _factories.GeoFactory.CreateExtents2D(1500, 1500, 1500, 1500);
            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
                                                                                       {
                                                                                           return item.Value;
                                                                                       }));
            Assert.Equal(2, resultsList.Count);
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 4; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 5; }));
            resultsList.Clear();

            searchExtents = _factories.GeoFactory.CreateExtents2D(100, 100, 100, 100);
            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
                                                                                       {
                                                                                           return item.Value;
                                                                                       }));
            Assert.Equal(4, resultsList.Count);
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

        }

        [Fact]
        public void SearchingInIndexWithMoreThanMaxNodeItemsSucceeds()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            addEntries(rTree, null, nodeItemMax + 1);

            List<Int32> resultsList = new List<Int32>();

            IExtents searchExtents = _factories.GeoFactory.CreateExtents2D(-100, -100, 5928.57523425, 3252.50803582);

            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
            {
                return item.Value;
            }));

            Assert.Equal(11, resultsList.Count);

            resultsList.Clear();

            searchExtents = _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100);

            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
            {
                return item.Value;
            }));

            Assert.Equal(9, resultsList.Count);
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 3; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 6; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            resultsList.Clear();

            searchExtents = _factories.GeoFactory.CreateExtents2D(1500, 1500, 1500, 1500);
            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
                                                                                       {
                                                                                           return item.Value;
                                                                                       }));
            Assert.Equal(2, resultsList.Count);
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 4; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 5; }));
            resultsList.Clear();

            searchExtents = _factories.GeoFactory.CreateExtents2D(100, 100, 100, 100);
            resultsList.AddRange(rTree.Query(searchExtents, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
                                                                                       {
                                                                                           return item.Value;
                                                                                       }));
            Assert.Equal(7, resultsList.Count);
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 1; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 2; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 7; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 8; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 9; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 10; }));
            Assert.True(resultsList.Exists(delegate(Int32 match) { return match == 11; }));

            rTree.Dispose();
        }

        [Fact]
        public void RemovingItemSucceeds()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            List<BoundedInt32> entries = new List<BoundedInt32>();
            addEntries(rTree, entries, nodeItemMax + 1);

            rTree.Remove(entries[0]);

            Assert.Equal(entries.Count - 1, rTree.Root.TotalItemCount);
        }

        [Fact]
        public void InsertingAndRemovingItemsSucceeds()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            List<BoundedInt32> entries = new List<BoundedInt32>();
            addEntries(rTree, entries, nodeItemMax + 1);

            for (Int32 i = entries.Count - 1; i >= 0; i--)
            {
                rTree.Remove(entries[i]);
            }

            Assert.Equal(0, rTree.Root.TotalItemCount);
            Assert.Equal(0, rTree.Root.TotalNodeCount);
        }

        [Fact(Skip = "Temp")]
        public void LoadingWith100000ObjectsCompletesAndDoesntCrash()
        {
            Int32 nodeItemMax;
            DynamicRTree<BoundedInt32> rTree = createNewRTree(out nodeItemMax);

            List<Int32> resultsList = new List<Int32>();

            addRandomEntries(rTree);

            rTree.Dispose();
        }

        [Fact(Skip = "Saving index broken")]
        public void SaveIndexTest()
        {
            DynamicRTree<BoundedInt32> rTree = new DynamicRTree<BoundedInt32>(
                                                            _factories.GeoFactory,
                                                            new GuttmanQuadraticInsert<BoundedInt32>(_factories.GeoFactory),
                                                            new GuttmanQuadraticSplit<BoundedInt32>(_factories.GeoFactory),
                                                            new DynamicRTreeBalanceHeuristic());

            addRandomEntries(rTree);
            MemoryStream s = new MemoryStream();
            rTree.SaveIndex(s);
            rTree.Dispose();

            s.Position = 0;
            DynamicRTree<BoundedInt32> rTree2 = DynamicRTree<BoundedInt32>.FromStream(s, _factories.GeoFactory);
            List<Int32> results = new List<Int32>();
            results.AddRange(rTree2.Query(rTree2.Root.Bounds, (Func<BoundedInt32, Int32>)delegate(BoundedInt32 item)
                                                                                         {
                                                                                             return item.Value;
                                                                                         }));
            Assert.Equal(99990, results.Count);
        }

        private void addEntries(ISpatialIndex<IExtents, BoundedInt32> rTree, IList<BoundedInt32> entries, Int32 itemCount)
        {
            BoundedInt32 entry;

            if (itemCount > 0)
            {
                entry = new BoundedInt32(1, _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 1)
            {
                entry = new BoundedInt32(2, _factories.GeoFactory.CreateExtents2D(50, 50, 150, 150));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 2)
            {
                entry = new BoundedInt32(3, _factories.GeoFactory.CreateExtents2D(-100, -100, 0, 0));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 3)
            {
                entry = new BoundedInt32(4, _factories.GeoFactory.CreateExtents2D(1000, 1000, 2000, 2000));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 4)
            {
                entry = new BoundedInt32(5, _factories.GeoFactory.CreateExtents2D(346.23975, 424.5720832, 5928.57523425, 3252.50803582));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 5)
            {
                entry = new BoundedInt32(6, _factories.GeoFactory.CreateExtents2D(0, 0, 0, 0));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 6)
            {
                entry = new BoundedInt32(7, _factories.GeoFactory.CreateExtents2D(100, 100, 100, 100));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 7)
            {
                entry = new BoundedInt32(8, _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 8)
            {
                entry = new BoundedInt32(9, _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 9)
            {
                entry = new BoundedInt32(10, _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100));
                addEntry(entries, entry, rTree);
            }

            if (itemCount > 10)
            {
                entry = new BoundedInt32(11, _factories.GeoFactory.CreateExtents2D(0, 0, 100, 100));
                addEntry(entries, entry, rTree);
            }
        }

        private static void addEntry(IList<BoundedInt32> entries, BoundedInt32 entry, ISpatialIndex<IExtents, BoundedInt32> rTree)
        {
            if (entries != null)
            {
                entries.Add(entry);
            }

            rTree.Insert(entry);
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

                IExtents bounds = _factories.GeoFactory.CreateExtents2D(xMin, yMin, xMax, yMax);
                rTree.Insert(new BoundedInt32(i, bounds));
            }
        }

        private DynamicRTree<BoundedInt32> createNewRTree(out Int32 nodeItemMax)
        {
            DynamicRTreeBalanceHeuristic balanceHeuristic = new DynamicRTreeBalanceHeuristic();

            nodeItemMax = balanceHeuristic.NodeItemMaximumCount;

            return new DynamicRTree<BoundedInt32>(
                _factories.GeoFactory,
                new GuttmanQuadraticInsert<BoundedInt32>(_factories.GeoFactory),
                new GuttmanQuadraticSplit<BoundedInt32>(_factories.GeoFactory),
                balanceHeuristic);
        }
    }
}