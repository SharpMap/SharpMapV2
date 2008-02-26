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
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using GeoAPI.Indexing;

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
        //private readonly Func<TItem, IExtents> _bounder;
        private readonly IGeometryFactory _geoFactory;

        public GuttmanQuadraticSplit(IGeometryFactory geoFactory)
        {
            //_bounder = bounder;
            _geoFactory = geoFactory;
        }

        #region INodeSplitStrategy Members

        public ISpatialIndexNode<IExtents, TItem> SplitNode(ISpatialIndexNode<IExtents, TItem> node, IndexBalanceHeuristic heuristic)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            RTreeNode<TItem> rTreeNode = node as RTreeNode<TItem>;
            Debug.Assert(rTreeNode != null);

            if (rTreeNode.IsLeaf)
            {
                return splitLeafNode(rTreeNode, heuristic as DynamicRTreeBalanceHeuristic);
            }
            else
            {
                return splitBranchNode(rTreeNode, heuristic as DynamicRTreeBalanceHeuristic);
            }
        }

        #endregion

        private RTreeNode<TItem> splitLeafNode(RTreeNode<TItem> node, DynamicRTreeBalanceHeuristic heuristic)
        {
            List<TItem> entries = new List<TItem>(node.Items);
            List<TItem> group1 = new List<TItem>();
            List<TItem> group2 = new List<TItem>();

            pickSeeds(entries, group1, group2);

            distribute(entries, group1, group2, heuristic);

            if (entries.Count > 0)
            {
                if (group1.Count < heuristic.NodeItemMinimumCount)
                {
                    group1.AddRange(entries);
                }
                if (group2.Count < heuristic.NodeItemMinimumCount)
                {
                    group2.AddRange(entries);
                }
            }

            node.Clear();
            node.AddItems(group1);
            RTreeNode<TItem> sibling = (node.Index as RTree<TItem>).CreateNode();
            sibling.AddItems(group2);
            return sibling;
        }

        private RTreeNode<TItem> splitBranchNode(RTreeNode<TItem> branchNode, IndexBalanceHeuristic heuristic)
        {
            List<ISpatialIndexNode<IExtents, TItem>> children = new List<ISpatialIndexNode<IExtents, TItem>>(branchNode.Children);
            List<ISpatialIndexNode<IExtents, TItem>> group1 = new List<ISpatialIndexNode<IExtents, TItem>>();
            List<ISpatialIndexNode<IExtents, TItem>> group2 = new List<ISpatialIndexNode<IExtents, TItem>>();

            pickSeeds(children, group1, group2);
            distribute(children, group1, group2, heuristic);

            if (children.Count > 0)
            {
                if (group1.Count < heuristic.NodeItemMinimumCount)
                {
                    group1.AddRange(children);
                }
                if (group2.Count < heuristic.NodeItemMinimumCount)
                {
                    group2.AddRange(children);
                }
            }

            branchNode.Clear();
            branchNode.AddChildren(group1);
            RTreeNode<TItem> sibling = (branchNode.Index as RTree<TItem>).CreateNode();
            sibling.AddChildren(group2);
            return sibling;
        }

        private void pickSeeds(ICollection<TItem> entries, IList<TItem> group1, IList<TItem> group2)
        {
            Double largestWaste = Double.MinValue;

            foreach (TItem entry1 in entries)
            {
                foreach (TItem entry2 in entries)
                {
                    if (entry1.Equals(entry2))
                    {
                        continue;
                    }

                    IExtents entry1Bounds = entry1.Bounds; // _bounder(entry1);
                    IExtents entry2Bounds = entry2.Bounds; // _bounder(entry2);
                    IExtents minBoundingRectangle = _geoFactory.CreateExtents(entry1Bounds, entry2Bounds);
                    IExtents intersection = entry1Bounds.Intersection(entry2Bounds);

                    Double waste =
                        minBoundingRectangle.GetSize(Ordinates.X, Ordinates.Y)
                        - entry1Bounds.GetSize(Ordinates.X, Ordinates.Y)
                        - entry2Bounds.GetSize(Ordinates.X, Ordinates.Y)
                        + (intersection.IsEmpty 
                            ? 0
                            : intersection.GetSize(Ordinates.X, Ordinates.Y));

                    if (group1.Count == 0 && group2.Count == 0)
                    {
                        group1.Add(entry1);
                        group2.Add(entry2);
                        largestWaste = waste;
                        continue;
                    }

                    if (waste > largestWaste)
                    {
                        group1[0] = entry1;
                        group2[0] = entry2;
                        largestWaste = waste;
                    }
                }
            }

            entries.Remove(group1[0]);
            entries.Remove(group2[0]);
        }

        private void pickSeeds(ICollection<ISpatialIndexNode<IExtents, TItem>> nodes, IList<ISpatialIndexNode<IExtents, TItem>> group1, IList<ISpatialIndexNode<IExtents, TItem>> group2)
        {
            Double largestWaste = Double.MinValue;

            // Here's why it's known as a quadratic algorithm - O(n^2)
            foreach (ISpatialIndexNode<IExtents, TItem> node1 in nodes)
            {
                foreach (ISpatialIndexNode<IExtents, TItem> node2 in nodes)
                {
                    if (ReferenceEquals(node1, node2))
                    {
                        continue;
                    }

                    IExtents minBoundingRectangle = _geoFactory.CreateExtents(node1.Bounds, node2.Bounds);

                    IExtents intersection = node1.Bounds.Intersection(node2.Bounds);

                    Double waste =
                        minBoundingRectangle.GetSize(Ordinates.X, Ordinates.Y)
                        - node1.Bounds.GetSize(Ordinates.X, Ordinates.Y)
                        - node2.Bounds.GetSize(Ordinates.X, Ordinates.Y)
                        + intersection.GetSize(Ordinates.X, Ordinates.Y);

                    if (group1.Count == 0 && group2.Count == 0)
                    {
                        group1.Add(node1);
                        group2.Add(node2);
                        largestWaste = waste;
                        continue;
                    }

                    if (waste > largestWaste)
                    {
                        group1[0] = node1;
                        group2[0] = node2;
                        largestWaste = waste;
                    }
                }
            }

            nodes.Remove(group1[0]);
            nodes.Remove(group2[0]);
        }

        enum GroupBoxLeastEnlarged
        {
            Tie = 0,
            Group1,
            Group2
        }

        private void distribute(ICollection<TItem> entries, IList<TItem> group1, List<TItem> group2, IndexBalanceHeuristic heuristic)
        {
            if (entries.Count == 0)
            {
                return;
            }

            if (group1.Count == heuristic.TargetNodeCount || group2.Count == heuristic.TargetNodeCount)
            {
                return;
            }

            TItem entry;
            IExtents group1Box = computeBounds(group1);
            IExtents group2Box = computeBounds(group2);
            GroupBoxLeastEnlarged group = pickNext(entries, group1Box, group2Box, out entry);

            switch (group)
            {
                case GroupBoxLeastEnlarged.Group1:
                    group1.Add(entry);
                    break;

                case GroupBoxLeastEnlarged.Group2:
                    group2.Add(entry);
                    break;

                case GroupBoxLeastEnlarged.Tie:
                default:

                    Double group1BoxArea = group1Box.GetSize(Ordinates.X, Ordinates.Y);
                    Double group2BoxArea = group2Box.GetSize(Ordinates.X, Ordinates.Y);

                    if (group1BoxArea < group2BoxArea)
                    {
                        group1.Add(entry);
                    }
                    else if (group2BoxArea < group1BoxArea)
                    {
                        group2.Add(entry);
                    }
                    else if (group1.Count < group2.Count)
                    {
                        group1.Add(entry);
                    }
                    else if (group2.Count < group1.Count)
                    {
                        group2.Add(entry);
                    }
                    else if ((DateTime.Now.Ticks / 10) % 2 == 0)
                    {
                        group1.Add(entry);
                    }
                    else
                    {
                        group2.Add(entry);
                    }

                    break;
            }

            distribute(entries, group1, group2, heuristic);
        }

        private void distribute(IList<ISpatialIndexNode<IExtents, TItem>> nodes, List<ISpatialIndexNode<IExtents, TItem>> group1, List<ISpatialIndexNode<IExtents, TItem>> group2, IndexBalanceHeuristic heuristic)
        {
            if (nodes.Count == 0)
            {
                return;
            }

            if (group1.Count == heuristic.TargetNodeCount || group2.Count == heuristic.TargetNodeCount)
            {
                return;
            }

            ISpatialIndexNode<IExtents, TItem> node;
            IExtents group1Box = computeBounds(group1);
            IExtents group2Box = computeBounds(group2);
            GroupBoxLeastEnlarged group = pickNext(nodes, group1Box, group2Box, out node);

            switch (group)
            {
                case GroupBoxLeastEnlarged.Group1:
                    group1.Add(node);
                    break;

                case GroupBoxLeastEnlarged.Group2:
                    group2.Add(node);
                    break;

                case GroupBoxLeastEnlarged.Tie:
                default:
                    if (group1Box.GetSize(Ordinates.X, Ordinates.Y) < group2Box.GetSize(Ordinates.X, Ordinates.Y))
                    {
                        group1.Add(node);
                    }
                    else if (group2Box.GetSize(Ordinates.X, Ordinates.Y) < group1Box.GetSize(Ordinates.X, Ordinates.Y))
                    {
                        group2.Add(node);
                    }
                    else if (group1.Count < group2.Count)
                    {
                        group1.Add(node);
                    }
                    else if (group2.Count < group1.Count)
                    {
                        group2.Add(node);
                    }
                    else if ((DateTime.Now.Ticks / 10) % 2 == 0)
                    {
                        group1.Add(node);
                    }
                    else
                    {
                        group2.Add(node);
                    }

                    break;
            }

            distribute(nodes, group1, group2, heuristic);
        }

        private GroupBoxLeastEnlarged pickNext(ICollection<TItem> entries, IExtents group1Box, IExtents group2Box, out TItem entry)
        {
            Double maxArealDifference = Double.MinValue;
            GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
            TItem nextEntry = default(TItem);

            foreach (TItem e in entries)
            {
                IExtents bounds = e.Bounds; // _bounder(e);
                IExtents group1Join = _geoFactory.CreateExtents(group1Box, bounds);
                IExtents group2Join = _geoFactory.CreateExtents(group2Box, bounds);

                Double arealDifferenceGroup1 = Math.Abs(
                    group1Join.GetSize(Ordinates.X, Ordinates.Y)
                    - group1Box.GetSize(Ordinates.X, Ordinates.Y));

                Double arealDifferenceGroup2 = Math.Abs(
                    group2Join.GetSize(Ordinates.X, Ordinates.Y)
                    - group2Box.GetSize(Ordinates.X, Ordinates.Y));

                if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
                {
                    maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
                    nextEntry = e;

                    if (arealDifferenceGroup1 < arealDifferenceGroup2)
                    {
                        group = GroupBoxLeastEnlarged.Group1;
                    }
                    else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                    {
                        group = GroupBoxLeastEnlarged.Group2;
                    }
                }
            }

            entries.Remove(nextEntry);
            entry = nextEntry;
            return group;
        }

        private GroupBoxLeastEnlarged pickNext(ICollection<ISpatialIndexNode<IExtents, TItem>> nodes, IExtents group1Box, IExtents group2Box, out ISpatialIndexNode<IExtents, TItem> nextNode)
        {
            Double maxArealDifference = Double.MinValue;
            GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
            nextNode = null;

            foreach (ISpatialIndexNode<IExtents, TItem> node in nodes)
            {
                IExtents group1Join = _geoFactory.CreateExtents(group1Box, node.Bounds);
                IExtents group2Join = _geoFactory.CreateExtents(group2Box, node.Bounds);

                Double arealDifferenceGroup1 = Math.Abs(
                    group1Join.GetSize(Ordinates.X, Ordinates.Y)
                    - group1Box.GetSize(Ordinates.X, Ordinates.Y));

                Double arealDifferenceGroup2 = Math.Abs(
                    group2Join.GetSize(Ordinates.X, Ordinates.Y)
                    - group2Box.GetSize(Ordinates.X, Ordinates.Y));

                if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
                {
                    maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
                    nextNode = node;

                    if (arealDifferenceGroup1 < arealDifferenceGroup2)
                    {
                        group = GroupBoxLeastEnlarged.Group1;
                    }
                    else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                    {
                        group = GroupBoxLeastEnlarged.Group2;
                    }
                }
            }

            nodes.Remove(nextNode);
            return group;
        }

        private IExtents computeBounds(IEnumerable<TItem> entries)
        {
            IExtents bounds = null;

            foreach (TItem entry in entries)
            {
                if (bounds == null)
                {
                    bounds = entry.Bounds;
                }
                else
                {
                    bounds = bounds.Union(entry.Bounds);
                }
            }

            return bounds;
        }

        private static IExtents computeBounds(IEnumerable<ISpatialIndexNode<IExtents, TItem>> nodes)
        {
            IExtents bounds = null;

            foreach (ISpatialIndexNode<IExtents, TItem> node in nodes)
            {
                if (bounds == null)
                {
                    bounds = node.Bounds;
                }
                else
                {
                    bounds.ExpandToInclude(node.Bounds);
                }
            }

            return bounds;
        }
    }
}
