using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Index.Strtree;
using SharpMap.Indexing.RTree;
using Xunit;
#if DOTNET35
using Enumerable = System.Linq.Enumerable;
#else
using Enumerable = GeoAPI.DataStructures.Enumerable;
#endif

#if BUFFERED
using NetTopologySuite.Coordinates;
#else
using BufferedCoordinate = NetTopologySuite.Coordinates.Simple.Coordinate;
using BufferedCoordinateFactory = NetTopologySuite.Coordinates.Simple.CoordinateFactory;
using BufferedCoordinateSequence = NetTopologySuite.Coordinates.Simple.CoordinateSequence;
using BufferedCoordinateSequenceFactory = NetTopologySuite.Coordinates.Simple.CoordinateSequenceFactory;
#endif

namespace SharpMap.Tests.Indexing
{
    public class DynamicStrTreeTests
    {

        readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private readonly IGeometryFactory<BufferedCoordinate> _geometryFactory = new GeometryFactory<BufferedCoordinate>(
            new BufferedCoordinateSequenceFactory(
                new BufferedCoordinateFactory(PrecisionModelType.DoubleFloating)));


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

        [Fact]
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
                Assert.True(list.Bounds.Intersects(queryExtents), "a result from the index does not intersect the query bounds");
            }

        }

        [Fact]
        public void TestDynamicStrIndex()
        {
            //jd note: this tests an experimental class and is not necessary for any real process
            DynamicSTRtree<BufferedCoordinate, IGeometry<BufferedCoordinate>>
                index = new DynamicSTRtree<BufferedCoordinate, IGeometry<BufferedCoordinate>>(_geometryFactory, 10,
                    new GQInsertStrategy<IExtents<BufferedCoordinate>, IGeometry<BufferedCoordinate>>(_geometryFactory),
                    new NullRestructuringStrategy<IExtents<BufferedCoordinate>, IGeometry<BufferedCoordinate>>(),
                    new GQNodeSplitStrategy<IExtents<BufferedCoordinate>, IGeometry<BufferedCoordinate>>(_geometryFactory),
                    new DynamicRTreeBalanceHeuristic()
                    );

            index.BulkLoad(
                CreateTestGeometries(1000, 0.0, 0.0, 3000.0, 3000.0));
            index.Build();

            IExtents<BufferedCoordinate> queryExtents =
                (IExtents<BufferedCoordinate>)_geometryFactory.CreateExtents2D(100.0, 100.0, 120.0, 120.0);

            int i = 0;
            foreach (IGeometry<BufferedCoordinate> list in index.Query(queryExtents))
            {
                Assert.True(list.Bounds.Intersects(queryExtents), "a result from the index does not intersect the query bounds");
                i++;
            }

            foreach (var v in CreateTestGeometries(1000, 0.0, 0.0, 3000.0, 3000.0))
            {
                index.Insert(v);
            }

            int j = 0;
            foreach (IGeometry<BufferedCoordinate> list in index.Query(queryExtents))
            {
                Assert.True(list.Bounds.Intersects(queryExtents), "a result from the index does not intersect the query bounds");
                j++;
            }


            Assert.True(j > i);
        }

        public class GQNodeSplitStrategy<TBounds, TItem> : INodeSplitStrategy<TBounds, TItem>
            where TItem : IBoundable<TBounds>
            where TBounds : IExtents
        {
            private readonly IGeometryFactory _geometryFactory;
            public ISpatialIndexNodeFactory<TBounds, TItem> NodeFactory { get; set; }

            public GQNodeSplitStrategy(IGeometryFactory geometryFactory)
            {
                _geometryFactory = geometryFactory;

            }

            public ISpatialIndexNode<TBounds, TItem> SplitNode(ISpatialIndexNode<TBounds, TItem> node, IndexBalanceHeuristic heuristic)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }



                DynamicRTreeBalanceHeuristic rTreeHueristic
                    = heuristic as DynamicRTreeBalanceHeuristic;

                return doSplit(node, rTreeHueristic);
            }

            private ISpatialIndexNode<TBounds, TItem> doSplit(ISpatialIndexNode<TBounds, TItem> node, DynamicRTreeBalanceHeuristic heuristic)
            {
                Boolean isLeaf = node.IsLeaf;

                IEnumerable<IBoundable<TBounds>> boundables =
                   isLeaf
                   ? Caster.Upcast<IBoundable<TBounds>, TItem>(node.Items)
                   : Caster.Upcast<IBoundable<TBounds>, ISpatialIndexNode<TBounds, TItem>>(node.SubNodes);

                Int32 boundablesCount = isLeaf ? node.ItemCount : node.SubNodeCount;

                List<IBoundable<TBounds>> entries = new List<IBoundable<TBounds>>(boundablesCount);
                entries.AddRange(boundables);

                IList<IBoundable<TBounds>> group1 = new List<IBoundable<TBounds>>();
                IList<IBoundable<TBounds>> group2 = new List<IBoundable<TBounds>>();

                pickSeeds(entries, group1, group2);

                Int32 group1Count = 1, group2Count = 1;

                distribute(entries, group1, group2, heuristic, ref group1Count, ref group2Count);

                if (entries.Count > 0)
                {
                    if (group1Count < heuristic.NodeItemMinimumCount)
                    {
                        fillShortGroup(entries, group1, ref group1Count);
                    }
                    else // group2Count < heuristic.NodeItemMinimumCount
                    {
                        fillShortGroup(entries, group2, ref group2Count);
                    }
                }

                node.Clear();


                ISpatialIndexNode<TBounds, TItem> sibling = NodeFactory.CreateNode(node.Level);

                IEnumerable<IBoundable<TBounds>> g1Sized = Enumerable.Take(group1, group1Count);
                IEnumerable<IBoundable<TBounds>> g2Sized = Enumerable.Take(group2, group2Count);

                if (isLeaf)
                {
                    //IEnumerable<TItem> g1Cast = Caster.Downcast<TItem, IBoundable<TBounds>>(g1Sized);
                    node.AddRange(g1Sized);


                    //IEnumerable<TItem> g2Cast = Caster.Downcast<TItem, IBoundable<IExtents>>(g2Sized);
                    sibling.AddRange(g2Sized);
                }
                else
                {
                    //IEnumerable<ISpatialIndexNode<IExtents, TItem>> g1Cast
                    //    = Caster.Downcast<ISpatialIndexNode<IExtents, TItem>, IBoundable<IExtents>>(g1Sized);
                    node.AddRange(g1Sized);

                    //IEnumerable<ISpatialIndexNode<IExtents, TItem>> g2Cast
                    //    = Caster.Downcast<ISpatialIndexNode<IExtents, TItem>, IBoundable<IExtents>>(g2Sized);
                    sibling.AddRange(g2Sized);
                }

                return sibling;
            }

            private static void fillShortGroup(IEnumerable<IBoundable<TBounds>> entries,
                                               IList<IBoundable<TBounds>> shortGroup,
                                               ref Int32 shortGroupCount)
            {
                foreach (IBoundable<TBounds> entry in entries)
                {
                    shortGroup.Add(entry);
                    shortGroupCount++;
                }
            }

            enum GroupBoundsLeastEnlarged
            {
                Tie = 0,
                Group1,
                Group2
            }

            private static void pickSeeds(IList<IBoundable<TBounds>> items,
                                          IList<IBoundable<TBounds>> group1,
                                          IList<IBoundable<TBounds>> group2)
            {
                Int32 group1Count = 0, group2Count = 0;
                Double largestWaste = -1;

                TBounds[] allExtents = new TBounds[items.Count];

                Int32 itemCount = items.Count;

                // read the bounds into local structures only once
                // for speed
                for (Int32 i = 0; i < itemCount; i++)
                {
                    allExtents[i] = (TBounds)items[i].Bounds.Clone();
                }

                Int32 seed1Index = -1, seed2Index = -1;

                for (Int32 i = 0; i < itemCount; i++)
                {
                    for (int j = 0; j < itemCount; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        TBounds e1 = allExtents[i];
                        TBounds e2 = allExtents[j];
                        TBounds minBoundingRectangle = (TBounds)e1.Union(e2);
                        TBounds intersection = (TBounds)e1.Intersection(e2);

                        Double minBoundingArea = Area(minBoundingRectangle);
                        Double entry1Area = Area(e1);
                        Double entry2Area = Area(e2);

                        Double waste = (minBoundingArea - entry1Area - entry2Area)
                                        + Area(intersection);

                        if (group1Count == 0 && group2Count == 0)
                        {
                            group1[group1Count++] = items[i];
                            group2[group2Count++] = items[j];
                            seed1Index = i;
                            seed2Index = j;
                            largestWaste = waste;
                            continue;
                        }

                        if (waste > largestWaste)
                        {
                            group1[0] = items[i];
                            group2[0] = items[j];
                            seed1Index = i;
                            seed2Index = j;
                            largestWaste = waste;
                        }
                    }
                }

                if (seed1Index > seed2Index)
                {
                    items.RemoveAt(seed1Index);
                    items.RemoveAt(seed2Index);
                }
                else
                {
                    items.RemoveAt(seed2Index);
                    items.RemoveAt(seed1Index);
                }
            }

            Random _random = new Random(DateTime.Now.Millisecond);

            private void distribute(IList<IBoundable<TBounds>> entries,
                                    IList<IBoundable<TBounds>> group1,
                                    IList<IBoundable<TBounds>> group2,
                                    IndexBalanceHeuristic heuristic,
                                    ref Int32 group1Count,
                                    ref Int32 group2Count)
            {

                // recursion halting case #1
                if (entries.Count == 0)
                {
                    return;
                }

                // recursion halting case #2
                if (group1Count == heuristic.TargetNodeCount
                    || group2Count == heuristic.TargetNodeCount)
                {
                    return;
                }

                TItem entry;
                TBounds group1Bounds = computeBounds(group1, group1Count);
                TBounds group2Bounds = computeBounds(group2, group2Count);

                GroupBoundsLeastEnlarged group = pickNext(entries, group1Bounds,
                                                          group2Bounds, out entry);

                switch (group)
                {
                    case GroupBoundsLeastEnlarged.Group1:
                        group1[group1Count++] = entry;
                        break;

                    case GroupBoundsLeastEnlarged.Group2:
                        group2[group2Count++] = entry;
                        break;

                    case GroupBoundsLeastEnlarged.Tie:
                        Double group1BoundsArea = Area(group1Bounds);
                        Double group2BoundsArea = Area(group2Bounds);

                        if (group1BoundsArea < group2BoundsArea)
                        {
                            group1[group1Count++] = entry;
                        }
                        else if (group2BoundsArea < group1BoundsArea)
                        {
                            group2[group2Count++] = entry;
                        }
                        else if (group1Count < group2Count)
                        {
                            group1[group1Count++] = entry;
                        }
                        else if (group2Count < group1Count)
                        {
                            group2[group2Count++] = entry;
                        }
                        else if (_random.Next(0, 2) == 0) // generates 0 or 1 randomly
                        {
                            group1[group1Count++] = entry;
                        }
                        else
                        {
                            group2[group2Count++] = entry;
                        }

                        break;
                    default:
                        throw new InvalidOperationException("Unknown group.");
                }

                distribute(entries, group1, group2, heuristic, ref group1Count, ref group2Count);
            }

            private GroupBoundsLeastEnlarged pickNext(IList<IBoundable<TBounds>> entries,
                                                      TBounds group1Bounds,
                                                      TBounds group2Bounds,
                                                      out TItem entry)
            {
                Double maxArealDifference = -1;
                GroupBoundsLeastEnlarged group = GroupBoundsLeastEnlarged.Tie;
                TItem nextEntry = default(TItem);

                foreach (TItem e in entries)
                {
                    TBounds bounds = (TBounds)e.Bounds.Clone();
                    TBounds group1Join = (TBounds)bounds.Union(group1Bounds);
                    TBounds group2Join = (TBounds)bounds.Union(group2Bounds);

                    Double arealDifferenceGroup1 = Math.Abs(
                        Area(group1Join) - Area(group1Bounds));

                    Double arealDifferenceGroup2 = Math.Abs(
                        Area(group2Join) - Area(group2Bounds));

                    Double differenceInAreas = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);

                    if (differenceInAreas > maxArealDifference)
                    {
                        maxArealDifference = differenceInAreas;
                        nextEntry = e;

                        if (arealDifferenceGroup1 < arealDifferenceGroup2)
                        {
                            group = GroupBoundsLeastEnlarged.Group1;
                        }
                        else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                        {
                            group = GroupBoundsLeastEnlarged.Group2;
                        }
                    }
                }

                entries.Remove(nextEntry);
                entry = nextEntry;
                return group;
            }

            private TBounds computeBounds(IList<IBoundable<TBounds>> entries, Int32 count)
            {
                TBounds bounds = (TBounds)_geometryFactory.CreateExtents();

                for (Int32 i = 0; i < count; i++)
                {
                    if (bounds.IsEmpty)
                    {
                        bounds = entries[i].Bounds;
                    }
                    else
                    {
                        bounds = (TBounds)bounds.Union(entries[i].Bounds);
                    }
                }

                return bounds;
            }

            private static double Area(TBounds bounds)
            {
                double width = bounds.GetMax(Ordinates.X) - bounds.GetMin(Ordinates.X);
                double height = bounds.GetMax(Ordinates.Y) - bounds.GetMin(Ordinates.Y);
                return width * height;
            }

        }

        public class GQInsertStrategy<TBounds, TItem> : IItemInsertStrategy<TBounds, TItem>
            where TItem : IBoundable<TBounds>
            where TBounds : IExtents
        {


            private IGeometryFactory _geometryFactory;

            public GQInsertStrategy(IGeometryFactory geometryFactory)
            {
                _geometryFactory = geometryFactory;

            }


            #region IItemInsertStrategy<TBounds,TItem> Members

            public void Insert(TBounds bounds, TItem entry, ISpatialIndexNode<TBounds, TItem> node, INodeSplitStrategy<TBounds, TItem> nodeSplitStrategy, IndexBalanceHeuristic heuristic, out ISpatialIndexNode<TBounds, TItem> newSiblingFromSplit)
            {
                newSiblingFromSplit = null;

                //TODO: handle null node..

                // Terminating case
                if (node.IsLeaf)
                {
                    node.Add(entry);

                    // Handle node overflow
                    if (node.ItemCount > heuristic.NodeItemMaximumCount)
                    {
                        // Split the node using the given strategy
                        newSiblingFromSplit = nodeSplitStrategy.SplitNode(node, heuristic);

                        //if (++_tempSplitCount % 100 == 0)
                        //{
                        //    Debug.Print("Node split # {0}", _tempSplitCount);
                        //}
                    }
                }
                else
                {
                    // NOTE: Descending the tree recursively here
                    // can make for a very expensive build of a tree, 
                    // even for moderate amounts of data.

                    Double leastExpandedArea = Double.MaxValue;
                    Double leastExpandedChildArea = Double.MaxValue;

                    //TBounds currentBounds = new ComputationExtents(bounds);

                    ISpatialIndexNode<TBounds, TItem> leastExpandedChild;
                    leastExpandedChild = findLeastExpandedChild(node.SubNodes,
                                                                bounds,
                                                                leastExpandedArea,
                                                                leastExpandedChildArea);

                    Debug.Assert(leastExpandedChild != null);

                    // Found least expanded child node - insert into it
                    Insert(bounds, entry, leastExpandedChild, nodeSplitStrategy,
                        heuristic, out newSiblingFromSplit);


                    // Adjust this node...
                    node.Bounds.ExpandToInclude(bounds);

                    // Check for overflow and add to current node if it occured
                    if (newSiblingFromSplit != null)
                    {
                        // Add new sibling node to the current node
                        node.Add(newSiblingFromSplit);
                        newSiblingFromSplit = null;

                        // Split the current node, since the child count is too high, 
                        // and return the split to the caller
                        if (node.ItemCount > heuristic.NodeItemMaximumCount)
                        {
                            newSiblingFromSplit = nodeSplitStrategy.SplitNode(node, heuristic);
                        }
                    }
                }
            }

            private static ISpatialIndexNode<TBounds, TItem> findLeastExpandedChild(
                                              IEnumerable<ISpatialIndexNode<TBounds, TItem>> children,
                                              TBounds currentBounds,
                                              Double leastExpandedArea,
                                              Double leastExpandedChildArea)
            {
                ISpatialIndexNode<TBounds, TItem> leastExpandedChild = null;

                foreach (ISpatialIndexNode<TBounds, TItem> child in children)
                {
                    TBounds childBounds = child.Bounds;

                    TBounds candidateRegion = (TBounds)childBounds.Union(currentBounds);

                    Double candidateRegionArea = Area(candidateRegion);
                    Double childArea = Area(childBounds);
                    Double expandedArea = candidateRegionArea - childArea;

                    if (expandedArea < leastExpandedArea)
                    {
                        leastExpandedChild = child;
                        leastExpandedChildArea = childArea;
                        leastExpandedArea = expandedArea;
                    }
                    else if (expandedArea == leastExpandedArea
                             && childArea < leastExpandedChildArea)
                    {
                        leastExpandedChild = child;
                        leastExpandedChildArea = childArea;
                    }
                }

                return leastExpandedChild;
            }


            #endregion

            private static double Area(TBounds bounds)
            {
                double width = bounds.GetMax(Ordinates.X) - bounds.GetMin(Ordinates.X);
                double height = bounds.GetMax(Ordinates.Y) - bounds.GetMin(Ordinates.Y);
                return width * height;
            }


            #region IItemInsertStrategy<TBounds,TItem> Members


            public ISpatialIndexNodeFactory<TBounds, TItem> NodeFactory
            {
                get; set;
            }

            #endregion
        }
    }
}
