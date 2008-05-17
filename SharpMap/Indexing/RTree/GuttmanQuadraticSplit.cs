// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using NPack;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// Implements a common, well accepted R-Tree node splitting strategy.
    /// </summary>
    /// <typeparam name="TItem">The type of the value used in the entries.</typeparam>
    /// <remarks>
    /// This strategy follows Antonin Guttman's findings in his paper 
    /// entitled "R-Trees: A Dynamic Index Structure for Spatial Searching", 
    /// Proc. 1984 ACM SIGMOD International Conference on Management of Data, pp. 47-57.
    /// </remarks>
    public class GuttmanQuadraticSplit<TItem> : INodeSplitStrategy<IExtents, TItem>
        where TItem : IBoundable<IExtents>
    {
        private readonly IGeometryFactory _geoFactory;
        private readonly Random _random = new MersenneTwister();

        public GuttmanQuadraticSplit(IGeometryFactory geoFactory)
        {
            _geoFactory = geoFactory;
        }

        #region INodeSplitStrategy Members

        public ISpatialIndexNode<IExtents, TItem> SplitNode(
            ISpatialIndexNode<IExtents, TItem> node, IndexBalanceHeuristic heuristic)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            RTreeNode<TItem> rTreeNode = node as RTreeNode<TItem>;
            Debug.Assert(rTreeNode != null);

            DynamicRTreeBalanceHeuristic rTreeHueristic
                = heuristic as DynamicRTreeBalanceHeuristic;

            return doSplit(rTreeNode, rTreeHueristic);
        }

        #endregion

        private RTreeNode<TItem> doSplit(RTreeNode<TItem> node, DynamicRTreeBalanceHeuristic heuristic)
        {
            Boolean isLeaf = node.IsLeaf;

            IEnumerable<IBoundable<IExtents>> boundables =
               isLeaf
               ? Enumerable.Upcast<IBoundable<IExtents>, TItem>(node.Items)
               : Enumerable.Upcast<IBoundable<IExtents>, ISpatialIndexNode<IExtents, TItem>>(node.Children);
           
            Int32 boundablesCount = isLeaf ? node.ItemCount : node.ChildCount;

            List<IBoundable<IExtents>> entries = new List<IBoundable<IExtents>>(boundablesCount);
            entries.AddRange(boundables);

            IBoundable<IExtents>[] group1 = new IBoundable<IExtents>[heuristic.NodeItemMaximumCount];
            IBoundable<IExtents>[] group2 = new IBoundable<IExtents>[heuristic.NodeItemMaximumCount];

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

            RTreeNode<TItem> sibling = (node.Index as RTree<TItem>).CreateNode();

            IEnumerable<IBoundable<IExtents>> g1Sized = Enumerable.Take(group1, group1Count);
            IEnumerable<IBoundable<IExtents>> g2Sized = Enumerable.Take(group2, group2Count);

            if (isLeaf)
            {
                IEnumerable<TItem> g1Cast = Enumerable.Downcast<TItem, IBoundable<IExtents>>(g1Sized);
                node.AddItems(g1Cast);

                IEnumerable<TItem> g2Cast = Enumerable.Downcast<TItem, IBoundable<IExtents>>(g2Sized);
                sibling.AddItems(g2Cast);
            }
            else
            {
                IEnumerable<ISpatialIndexNode<IExtents, TItem>> g1Cast
                    = Enumerable.Downcast<ISpatialIndexNode<IExtents, TItem>, IBoundable<IExtents>>(g1Sized);
                node.AddChildren(g1Cast);

                IEnumerable<ISpatialIndexNode<IExtents, TItem>> g2Cast
                    = Enumerable.Downcast<ISpatialIndexNode<IExtents, TItem>, IBoundable<IExtents>>(g2Sized);
                sibling.AddChildren(g2Cast);
            }

            return sibling;
        }

        private static void fillShortGroup(IEnumerable<IBoundable<IExtents>> entries, 
                                           IBoundable<IExtents>[] shortGroup, 
                                           ref Int32 shortGroupCount)
        {
            foreach (IBoundable<IExtents> entry in entries)
            {
                shortGroup[shortGroupCount++] = entry;
            }
        }

        enum GroupBoundsLeastEnlarged
        {
            Tie = 0,
            Group1,
            Group2
        }

        private static void pickSeeds(IList<IBoundable<IExtents>> items,
                                      IBoundable<IExtents>[] group1,
                                      IBoundable<IExtents>[] group2)
        {
            Int32 group1Count = 0, group2Count = 0;
            Double largestWaste = -1;

            ComputationExtents[] allExtents = new ComputationExtents[items.Count];

            Int32 itemCount = items.Count;

            // read the bounds into local structures only once
            // for speed
            for (Int32 i = 0; i < itemCount; i++)
            {
                allExtents[i] = new ComputationExtents(items[i].Bounds);
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

                    ComputationExtents e1 = allExtents[i];
                    ComputationExtents e2 = allExtents[j];
                    ComputationExtents minBoundingRectangle = e1.Union(e2);
                    ComputationExtents intersection = e1.Intersection(e2);

                    Double minBoundingArea = minBoundingRectangle.Area;
                    Double entry1Area = e1.Area;
                    Double entry2Area = e2.Area;

                    Double waste = (minBoundingArea - entry1Area - entry2Area)
                                    + intersection.Area;

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

        private void distribute(IList<IBoundable<IExtents>> entries,
                                IBoundable<IExtents>[] group1,
                                IBoundable<IExtents>[] group2,
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
            ComputationExtents group1Bounds = computeBounds(group1, group1Count);
            ComputationExtents group2Bounds = computeBounds(group2, group2Count);

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
                    Double group1BoundsArea = group1Bounds.Area;
                    Double group2BoundsArea = group2Bounds.Area;

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

        private GroupBoundsLeastEnlarged pickNext(IList<IBoundable<IExtents>> entries,
                                                  ComputationExtents group1Bounds,
                                                  ComputationExtents group2Bounds,
                                                  out TItem entry)
        {
            Double maxArealDifference = -1;
            GroupBoundsLeastEnlarged group = GroupBoundsLeastEnlarged.Tie;
            TItem nextEntry = default(TItem);

            foreach (TItem e in entries)
            {
                ComputationExtents bounds = new ComputationExtents(e.Bounds);
                ComputationExtents group1Join = bounds.Union(group1Bounds);
                ComputationExtents group2Join = bounds.Union(group2Bounds);

                Double arealDifferenceGroup1 = Math.Abs(
                    group1Join.Area - group1Bounds.Area);

                Double arealDifferenceGroup2 = Math.Abs(
                    group2Join.Area - group2Bounds.Area);

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

        private ComputationExtents computeBounds(IBoundable<IExtents>[] entries, Int32 count)
        {
            ComputationExtents bounds = new ComputationExtents();

            for (Int32 i = 0; i < count; i++)
            {
                if (bounds.IsEmpty)
                {
                    bounds = new ComputationExtents(entries[i].Bounds);
                }
                else
                {
                    bounds = bounds.Union(new ComputationExtents(entries[i].Bounds));
                }
            }

            return bounds;
        }

        //private RTreeNode<TItem> splitBranchNode(RTreeNode<TItem> branchNode, IndexBalanceHeuristic heuristic)
        //{
        //    List<ISpatialIndexNode<IExtents, TItem>> children = new List<ISpatialIndexNode<IExtents, TItem>>(branchNode.Children);
        //    List<ISpatialIndexNode<IExtents, TItem>> group1 = new List<ISpatialIndexNode<IExtents, TItem>>();
        //    List<ISpatialIndexNode<IExtents, TItem>> group2 = new List<ISpatialIndexNode<IExtents, TItem>>();

        //    pickSeeds(children, group1, group2);
        //    distribute(children, group1, group2, heuristic);

        //    if (children.Count > 0)
        //    {
        //        if (group1.Count < heuristic.NodeItemMinimumCount)
        //        {
        //            group1.AddRange(children);
        //        }
        //        if (group2.Count < heuristic.NodeItemMinimumCount)
        //        {
        //            group2.AddRange(children);
        //        }
        //    }

        //    branchNode.Clear();
        //    branchNode.AddChildren(group1);
        //    RTreeNode<TItem> sibling = (branchNode.Index as RTree<TItem>).CreateNode();
        //    sibling.AddChildren(group2);
        //    return sibling;
        //}

        //private void pickSeeds(ICollection<TItem> entries, IList<TItem> group1, IList<TItem> group2)
        //{
        //    Double largestWaste = Double.MinValue;

        //    foreach (TItem entry1 in entries)
        //    {
        //        foreach (TItem entry2 in entries)
        //        {
        //            if (entry1.Equals(entry2))
        //            {
        //                continue;
        //            }

        //            IExtents entry1Bounds = entry1.Bounds; // _bounder(entry1);
        //            IExtents entry2Bounds = entry2.Bounds; // _bounder(entry2);
        //            IExtents minBoundingRectangle = _geoFactory.CreateExtents(entry1Bounds, entry2Bounds);
        //            IExtents intersection = entry1Bounds.Intersection(entry2Bounds);

        //            Double waste =
        //                minBoundingRectangle.GetSize(Ordinates.X, Ordinates.Y)
        //                - entry1Bounds.GetSize(Ordinates.X, Ordinates.Y)
        //                - entry2Bounds.GetSize(Ordinates.X, Ordinates.Y)
        //                + (intersection.IsEmpty
        //                    ? 0
        //                    : intersection.GetSize(Ordinates.X, Ordinates.Y));

        //            if (group1.Count == 0 && group2.Count == 0)
        //            {
        //                group1.Add(entry1);
        //                group2.Add(entry2);
        //                largestWaste = waste;
        //                continue;
        //            }

        //            if (waste > largestWaste)
        //            {
        //                group1[0] = entry1;
        //                group2[0] = entry2;
        //                largestWaste = waste;
        //            }
        //        }
        //    }

        //    entries.Remove(group1[0]);
        //    entries.Remove(group2[0]);
        //}

        //private void pickSeeds(ICollection<ISpatialIndexNode<IExtents, TItem>> nodes, IList<ISpatialIndexNode<IExtents, TItem>> group1, IList<ISpatialIndexNode<IExtents, TItem>> group2)
        //{
        //    Double largestWaste = Double.MinValue;

        //    // Here's why it's known as a quadratic algorithm - O(n^2)
        //    foreach (ISpatialIndexNode<IExtents, TItem> node1 in nodes)
        //    {
        //        foreach (ISpatialIndexNode<IExtents, TItem> node2 in nodes)
        //        {
        //            if (ReferenceEquals(node1, node2))
        //            {
        //                continue;
        //            }

        //            IExtents minBoundingRectangle = _geoFactory.CreateExtents(node1.Bounds, node2.Bounds);

        //            IExtents intersection = node1.Bounds.Intersection(node2.Bounds);

        //            Double waste =
        //                minBoundingRectangle.GetSize(Ordinates.X, Ordinates.Y)
        //                - node1.Bounds.GetSize(Ordinates.X, Ordinates.Y)
        //                - node2.Bounds.GetSize(Ordinates.X, Ordinates.Y)
        //                + intersection.GetSize(Ordinates.X, Ordinates.Y);

        //            if (group1.Count == 0 && group2.Count == 0)
        //            {
        //                group1.Add(node1);
        //                group2.Add(node2);
        //                largestWaste = waste;
        //                continue;
        //            }

        //            if (waste > largestWaste)
        //            {
        //                group1[0] = node1;
        //                group2[0] = node2;
        //                largestWaste = waste;
        //            }
        //        }
        //    }

        //    nodes.Remove(group1[0]);
        //    nodes.Remove(group2[0]);
        //}

        //private void distribute(IList<ISpatialIndexNode<IExtents, TItem>> nodes, List<ISpatialIndexNode<IExtents, TItem>> group1, List<ISpatialIndexNode<IExtents, TItem>> group2, IndexBalanceHeuristic heuristic)
        //{
        //    if (nodes.Count == 0)
        //    {
        //        return;
        //    }

        //    if (group1.Count == heuristic.TargetNodeCount || group2.Count == heuristic.TargetNodeCount)
        //    {
        //        return;
        //    }

        //    ISpatialIndexNode<IExtents, TItem> node;
        //    IExtents group1Box = computeBounds(group1);
        //    IExtents group2Box = computeBounds(group2);
        //    GroupBoxLeastEnlarged group = pickNext(nodes, group1Box, group2Box, out node);

        //    switch (group)
        //    {
        //        case GroupBoxLeastEnlarged.Group1:
        //            group1.Add(node);
        //            break;

        //        case GroupBoxLeastEnlarged.Group2:
        //            group2.Add(node);
        //            break;

        //        case GroupBoxLeastEnlarged.Tie:
        //        default:
        //            if (group1Box.GetSize(Ordinates.X, Ordinates.Y) < group2Box.GetSize(Ordinates.X, Ordinates.Y))
        //            {
        //                group1.Add(node);
        //            }
        //            else if (group2Box.GetSize(Ordinates.X, Ordinates.Y) < group1Box.GetSize(Ordinates.X, Ordinates.Y))
        //            {
        //                group2.Add(node);
        //            }
        //            else if (group1.Count < group2.Count)
        //            {
        //                group1.Add(node);
        //            }
        //            else if (group2.Count < group1.Count)
        //            {
        //                group2.Add(node);
        //            }
        //            else if ((DateTime.Now.Ticks / 10) % 2 == 0)
        //            {
        //                group1.Add(node);
        //            }
        //            else
        //            {
        //                group2.Add(node);
        //            }

        //            break;
        //    }

        //    distribute(nodes, group1, group2, heuristic);
        //}

        //private GroupBoxLeastEnlarged pickNext(ICollection<ISpatialIndexNode<IExtents, TItem>> nodes, IExtents group1Box, IExtents group2Box, out ISpatialIndexNode<IExtents, TItem> nextNode)
        //{
        //    Double maxArealDifference = Double.MinValue;
        //    GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
        //    nextNode = null;

        //    foreach (ISpatialIndexNode<IExtents, TItem> node in nodes)
        //    {
        //        IExtents group1Join = node.Bounds.Union(group1Box);
        //        IExtents group2Join = node.Bounds.Union(group2Box);

        //        Double group1JoinArea = group1Join.GetSize(Ordinates.X, Ordinates.Y);
        //        Double group1BoxArea = group1Box.GetSize(Ordinates.X, Ordinates.Y);
        //        Double arealDifferenceGroup1 = Math.Abs(group1JoinArea - group1BoxArea);

        //        Double group2JoinArea = group2Join.GetSize(Ordinates.X, Ordinates.Y);
        //        Double group2BoxArea = group2Box.GetSize(Ordinates.X, Ordinates.Y);
        //        Double arealDifferenceGroup2 = Math.Abs(group2JoinArea - group2BoxArea);

        //        if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
        //        {
        //            maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
        //            nextNode = node;

        //            if (arealDifferenceGroup1 < arealDifferenceGroup2)
        //            {
        //                group = GroupBoxLeastEnlarged.Group1;
        //            }
        //            else if (arealDifferenceGroup2 < arealDifferenceGroup1)
        //            {
        //                group = GroupBoxLeastEnlarged.Group2;
        //            }
        //        }
        //    }

        //    nodes.Remove(nextNode);
        //    return group;
        //}

        //private static IExtents computeBounds(IEnumerable<ISpatialIndexNode<IExtents, TItem>> nodes)
        //{
        //    IExtents bounds = null;

        //    foreach (ISpatialIndexNode<IExtents, TItem> node in nodes)
        //    {
        //        if (bounds == null)
        //        {
        //            bounds = node.Bounds;
        //        }
        //        else
        //        {
        //            bounds.ExpandToInclude(node.Bounds);
        //        }
        //    }

        //    return bounds;
        //}
    }
}
